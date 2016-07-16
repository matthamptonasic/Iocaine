using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Inventory;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Properties;
using Iocaine2.Tools;

namespace Iocaine2.Bots
{
    public class Trader
    {
        #region Enums
        public enum TRADER_STATE
        {
            STOPPED = 0,
            RUNNING = 1,
            PAUSED_USER = 2,
            PAUSED_PROG = 3
        }
        #endregion Enums

        #region Constructors
        public Trader(Process iMainProc, ProcessModule iMainMod, int iNumberOfTrades,
                       int iNumberPerTrade, Item iItem, String iNPC, String[] iScriptList,
                       Button iStartButton, Statics.FuncPtrs.TD_Void_String_Color iUpdateTraderStartButtonCallBack)
        {
            mainProc = iMainProc;
            mainMod = iMainMod;
            item = iItem;
            npc = iNPC;
            scriptList = iScriptList;
            numberOfTrades = iNumberOfTrades;
            numberPerTrade = iNumberPerTrade;
            StartButton = iStartButton;
            updateTraderStartButtonCallBack = iUpdateTraderStartButtonCallBack;
            chatLog = ChatLogManager.Access.GetAsynchronousLogger("Trader");
            commandQueue = new Queue<string>();
        }
        #endregion Constructors
        
        #region Member Variables
        private Process mainProc;
        private ProcessModule mainMod;
        private Item item;
        private String npc;
        private int numberOfTrades;
        private int numberPerTrade;
        private int counter;
        private String[] scriptList;
        #region Controls
        private Button StartButton;
        private Statics.FuncPtrs.TD_Void_String_Color updateTraderStartButtonCallBack;
        #endregion Controls
        #region Helper Objects
        private ChatLoggerAsync chatLog;
        private Queue<String> commandQueue;
        #endregion Helper Objects
        #region State Variables
        private TRADER_STATE state;
        public TRADER_STATE State
        {
            get
            {
                return state;
            }
        }
        #endregion State Variables
        #region Delegates and Events
        public delegate void TradeDoneEvent(Item item, int numberSoFar);
        public event TradeDoneEvent TradeDone;
        #endregion Delegates and Events
        #endregion Member Variables

        #region Member Functions
        public void doInits()
        {

        }
        public void StartTrader()
        {
            try
            {
                state = TRADER_STATE.RUNNING;
                chatLog.Reset();
                Statics.FuncPtrs.SetStatusBoxPtr("Starting Trader. Trading " + numberOfTrades + " times.", Statics.Fields.Green);
                LoggingFunctions.Timestamp("Starting trader. Trading " + numberOfTrades + " times.");
                sendACommand();

                bool firstTimeThru = true;
                while (checkState() && checkStatus())
                {
                    if (!firstTimeThru)
                    {
                        Statics.FuncPtrs.SetStatusBoxPtr("Trading " + (counter + 1) + " of " + numberOfTrades + ".", Statics.Fields.Green);
                    }

                    //Check that the trade item is in our bag with sufficient quantity.
                    //If not, move inventory around.
                    if (!Containers.Bag.Contains(item))
                    {
                        byte openSpace = (byte)(Containers.Bag.LiveCapacity - Containers.Bag.LiveOccupancy);
                        if (openSpace < 2)
                        {
                            LoggingFunctions.Error("Trader :: Less than 2 open bag slots, cannot move more items to bag.");
                            break;
                        }

                        ushort nbSlotsToMove = (ushort)(openSpace - 1);
                        ushort nbItemsToMove = (ushort)(nbSlotsToMove * Data.Client.Things.GetStackSizeFromId(item.ItemID));
                        if ((Iocaine_2_Form.TR_TradeFromSack == true) && (Containers.Sack.Contains(item)))
                        {
                            //Move items
                            ushort itemQuan = Containers.Sack.GetItemQuan(item);
                            if (nbItemsToMove > itemQuan)
                            {
                                nbItemsToMove = itemQuan;
                            }
                            Movement.MoveItem(item, nbItemsToMove, Containers.Sack, Containers.Bag);
                        }
                        else if ((Iocaine_2_Form.TR_TradeFromSatchel == true) && (Containers.Satchel.Contains(item)))
                        {
                            //Move items
                            ushort itemQuan = Containers.Satchel.GetItemQuan(item);
                            if (nbItemsToMove > itemQuan)
                            {
                                nbItemsToMove = itemQuan;
                            }
                            Movement.MoveItem(item, nbItemsToMove, Containers.Satchel, Containers.Bag);
                        }
                        else if ((Iocaine_2_Form.TR_TradeFromCase == true) && (Containers.MCase.Contains(item)))
                        {
                            //Move items
                            ushort itemQuan = Containers.MCase.GetItemQuan(item);
                            if (nbItemsToMove > itemQuan)
                            {
                                nbItemsToMove = itemQuan;
                            }
                            Movement.MoveItem(item, nbItemsToMove, Containers.MCase, Containers.Bag);
                        }
                        else
                        {
                            Statics.FuncPtrs.SetStatusBoxPtr("No more items. Traded " + counter + " items.", Statics.Fields.Blue);
                            StopTrader();
                            continue;
                        }
                    }

                    //Target npc
                    if (!Interaction.TargetNPC(npc))
                    {
                        MessageBox.Show("Could not find NPC by name " + npc);
                        StopTrader();
                        continue;
                    }
                    sendAllCommands();
                    IocaineFunctions.delay(1000);
                    //Go to trade menu
                    MenuNavigation.GotoMenuItem("Trade", true);
                    sendAllCommands();

                    //Enter item to trade
                    setItems();

                    //Hit OK
                    MenuNavigation.HitOK();
                    sendAllCommands();

                    //Hit given number of Enters to clear dialog
                    try
                    {
                        runScript();
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("Running script after trading on trade #" + (counter + 1).ToString());
                        LoggingFunctions.Error(e.ToString());
                    }

                    counter++;
                    firstTimeThru = false;
                    TradeDone(item, counter);
                    if (obtainedItem())
                    {
                        LoggingFunctions.Timestamp("Obtained item after " + counter + " trades.");
                        if (Statics.Settings.Helpers.StopOnItem_Trader)
                        {
                            StopTrader();
                        }
                        continue;
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trader::StartTrader: " + e.ToString());
            }
        }
        #endregion Member Functions

        #region Trader State Utility Functions
        private bool checkState()
        {
            try
            {
                LoggingFunctions.Debug("TR::checkState: Entering TR checkstate.", LoggingFunctions.DBG_SCOPE.TRADER);
                switch (state)
                {
                    case TRADER_STATE.STOPPED:
                        updateStartButton("S&tart", Statics.Buttons.Green);
                        Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot", Statics.Fields.Blue);
                        return false;
                    case TRADER_STATE.RUNNING:
                        return true;
                    case TRADER_STATE.PAUSED_PROG:
                        goto case TRADER_STATE.PAUSED_USER;
                    case TRADER_STATE.PAUSED_USER:
                        Statics.FuncPtrs.SetStatusBoxPtr("Pausing bot", Statics.Fields.Yellow);
                        while (state != TRADER_STATE.RUNNING)
                        {
                            if (state == TRADER_STATE.STOPPED)
                            {
                                updateStartButton("S&tart", Statics.Buttons.Green);
                                Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot", Statics.Fields.Blue);
                                LoggingFunctions.Debug("TR::checkState: Returning false from TR checkstate.", LoggingFunctions.DBG_SCOPE.TRADER);
                                return false;
                            }
                            IocaineFunctions.delay(1000);
                            if ((mainProc == null) || (mainProc.HasExited == true))
                            {
                                state = TRADER_STATE.STOPPED;
                            }
                        }
                        chatLog.Reset();
                        LoggingFunctions.Debug("TR::checkState: Returning true from TR checkstate.", LoggingFunctions.DBG_SCOPE.TRADER);
                        return true;
                    default:
                        LoggingFunctions.Debug("TR::checkState: Returning false from TR checkstate.", LoggingFunctions.DBG_SCOPE.TRADER);
                        return false;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trader::checkState: " + e.ToString());
                return false;
            }
        }
        private bool checkStatus()
        {
            try
            {
                if (counter < numberOfTrades)
                {
                    return true;
                }
                else
                {
                    StopTrader();
                    updateStartButton("S&tart", Statics.Buttons.Green);
                    Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot", Statics.Fields.Blue);
                    LoggingFunctions.Debug("TR::checkStatus: Returning false from TR checkstatus.", LoggingFunctions.DBG_SCOPE.TRADER);
                    return false;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trader::checkStatus: " + e.ToString());
                return false;
            }
        }
        public void PauseTrader()
        {
            state = TRADER_STATE.PAUSED_USER;
        }
        public void StopTrader()
        {
            state = TRADER_STATE.STOPPED;
        }
        public void ResumeTrader()
        {
            state = TRADER_STATE.RUNNING;
        }
        #endregion Trader State Utility Functions
        
        #region Utility Functions
        #region interaction functions
        private void setItems()
        {
            try
            {
                byte index = MemReads.Self.Inventory.get_bag_index(item.ItemID, (byte)numberPerTrade);
                if (index == 0)
                {
                    LoggingFunctions.Error("Could not find item " + item.ItemID + " in inventory.");
                    MessageBox.Show("[ERROR] Could not find item " + item.ItemID + " in inventory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                LoggingFunctions.Debug("TR::setItems: Item 1: " + item.Name + ": " + item.ItemID + " : " + index + ".", LoggingFunctions.DBG_SCOPE.TRADER);
                MemReads.Windows.Trading.NPC.set_item(0, item.ItemID, (byte)numberPerTrade, index);

                LoggingFunctions.Debug("TR::setItems: Setting ingredients...", LoggingFunctions.DBG_SCOPE.TRADER);
                for (byte ii = 0; ii < 1; ii++)
                {
                    ushort id = 0;
                    byte quan = 0;
                    byte idx = 0;
                    MemReads.Windows.Trading.NPC.get_item(ii, ref id, ref quan, ref idx);
                    LoggingFunctions.Debug("TR::setItems: Actually read ID: " + id + ", quan: " + quan + ", index: " + idx + ".", LoggingFunctions.DBG_SCOPE.TRADER);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trader::setItems: " + e.ToString());
            }
        }
        private void runScript()
        {
            try
            {
                int nbCommands = scriptList.Length;
                for (int ii = 0; ii < nbCommands; ii++)
                {
                    if (scriptList[ii].Contains("Wait"))
                    {
                        int waitTime = 0;
                        String waitString = scriptList[ii].Substring(scriptList[ii].IndexOf('-') + 2);
                        waitTime = Convert.ToInt32(waitString);
                        IocaineFunctions.delay((uint)waitTime);
                        LoggingFunctions.Debug("TR::runScript: Script waiting for " + waitTime + " ms.", LoggingFunctions.DBG_SCOPE.TRADER);
                    }
                    else if (scriptList[ii].Contains("Enter"))
                    {
                        LoggingFunctions.Debug("TR::runScript: Script hitting enter.", LoggingFunctions.DBG_SCOPE.TRADER);
                        IocaineFunctions.keyDown(Keys.Enter, 75);
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trader::runScript: " + e.ToString());
            }
        }
        #endregion interaction functions
        #region control methods
        #endregion control methods
        #region GUI synchronization
        private void updateStartButton(String text, System.Drawing.Color color)
        {
            try
            {
                LoggingFunctions.Debug("TR::updateStartButton: Trying to update TR start button.", LoggingFunctions.DBG_SCOPE.TRADER);
                StartButton.BeginInvoke(updateTraderStartButtonCallBack, new object[] { text, color });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trader::udpateStartButton: " + e.ToString());
            }
        }
        #endregion GUI synchronization
        #region Chatlog Functions
        private bool obtainedItem()
        {
            String strObtained = "Obtained";
            String strGil = " gil.";
            uint nbLines = 0;
            try
            {
                if (chatLog.Update(ref nbLines))
                {
                    LoggingFunctions.Debug("TR::obtainedItem: Chat log has " + nbLines + " lines.", LoggingFunctions.DBG_SCOPE.TRADER);
                    bool gotItem = false;
                    bool error = false;
                    for (int ii = 0; ii < nbLines; ii++)
                    {
                        ChatLine chatString = null;
                        if (!chatLog.Read(out chatString))
                        {
                            error = true;
                        }
                        LoggingFunctions.Debug("TR::obtainedItem: Line[" + ii + "]: " + chatString + ", Code: " + chatString.Mode.ToString() + ".", LoggingFunctions.DBG_SCOPE.TRADER);
                        if (chatString.ProcessedLine.Contains(strObtained) && !chatString.ProcessedLine.Contains(strGil))
                        {
                            gotItem = true;
                            LoggingFunctions.Debug("TR::obtainedItem: Line[" + ii + "] gotItem = true.", LoggingFunctions.DBG_SCOPE.TRADER);
                        }
                    }
                    if (error)
                    {
                        LoggingFunctions.Error("Could not read chat log from Trader");
                    }
                    LoggingFunctions.Debug("TR::obtainedItem: Returning " + gotItem.ToString() + ".", LoggingFunctions.DBG_SCOPE.TRADER);
                    return gotItem;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trader::obtainedItem: " + e.ToString());
            }
            return false;
        }
        #endregion Chatlog Functions
        #region Command entry
        public void queueCommand(String cmd)
        {
            try
            {
                commandQueue.Enqueue(cmd);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trader::queueCommand: " + e.ToString());
            }
        }
        private void sendAllCommands()
        {
            try
            {
                uint delayBetween = 2000;
                int nbCmds = commandQueue.Count;
                for (int ii = 0; ii < nbCmds; ii++)
                {
                    IocaineFunctions.keys(commandQueue.Dequeue());
                    IocaineFunctions.delay(delayBetween);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trader::sendAllCommands: " + e.ToString());
            }
        }
        private bool sendACommand()
        {
            try
            {
                int nbCmds = commandQueue.Count;
                if (nbCmds > 0)
                {
                    IocaineFunctions.keys(commandQueue.Dequeue());
                    return true;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trader::sendACommand: " + e.ToString());
            }
            return false;
        }
        #endregion Command entry
        #endregion Utility Functions
    }
}
