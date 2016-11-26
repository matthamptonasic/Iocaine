using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Bots;
using Iocaine2.Data.Client;
using Iocaine2.Inventory;
using Iocaine2.Logging;
using Iocaine2.Memory.Interface;
using Iocaine2.Properties;
using Iocaine2.Settings;

namespace Iocaine2
{
    //This file is for functions directly related to the Trader
    partial class Iocaine_2_Form
    {
        #region Member Variables
        private static bool TR_StopOnItemChangedProg = false;
        internal static bool TR_TradeFromSack = true;
        internal static bool TR_TradeFromSatchel = true;
        internal static bool TR_TradeFromCase = true;
        #region Built In Scripts
        private static Dictionary<String, List<String>> TR_BuiltInScripts = new Dictionary<string, List<string>>()
        {
            { "Zaldon", 
                new List<String>{"Wait - 3000", "Enter", "Wait - 6500", "Enter", "Wait - 3500"} },
            { "Yoran-Oran", 
                new List<String>{"Wait - 3700", "Enter", "Wait - 1000", "Enter", "Wait - 1200"} },
            { "Melyon", 
                new List<String>{"Wait - 3500", "Enter", "Wait - 2400", "Enter", "Wait - 1200"} },
            { "Exoroche", 
                new List<String>{"Wait - 2000", "Enter", "Wait - 2000"} }
        };
        #endregion Built In Scripts
        #endregion Member Variables
        #region Thread Synchronization
        #region Delegate declarations
        private delegate void TR_rebuildTraderInventoryCheckboxDelegate();
        private delegate void TR_setStopOnItemChkBDelegate(bool iValue);
        #endregion Delegate declarations
        #region Delegate instances
        private TR_rebuildTraderInventoryCheckboxDelegate TR_rebuildTraderInventoryChkBPtr;
        private TR_setStopOnItemChkBDelegate TR_setStopOnItemChkBPtr;
        #endregion Delegate instances
        #region Delegate instantiations
        private void TR_createDelegates()
        {
            if (TR_rebuildTraderInventoryChkBPtr == null)
            {
                TR_rebuildTraderInventoryChkBPtr = new TR_rebuildTraderInventoryCheckboxDelegate(TR_rebuildTraderInventoryChkBCBF);
                TR_setStopOnItemChkBPtr = new TR_setStopOnItemChkBDelegate(TR_setStopOnItemChkBCBF);
            }
        }
        #endregion Delegate instantiations
        #endregion Thread Synchronization
        #region Initialization
        private bool doTRInits()
        {
            TR_createDelegates();
            rebuildTraderInventory();
            TR_LoadUserSettings();
            TR_LoadNPCNameAutoComplete();
            return true;
        }
        private void TR_LoadUserSettings()
        {
            Statics.Settings.Helpers.StopOnItem_Trader = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.HELPERS, "TR_StopOnItem"));
            TR_StopOnItemChangedProg = true;
            TR_setStopOnItemChkB(Statics.Settings.Helpers.StopOnItem_Trader);
        }
        private void TR_LoadNPCNameAutoComplete()
        {
            AutoCompleteStringCollection names = new AutoCompleteStringCollection();
            foreach(String str in TR_BuiltInScripts.Keys)
            {
                names.Add(str);
            }
            TR_NPC_Name_TextBox.AutoCompleteCustomSource = names;
            TR_NPC_Name_TextBox.AutoCompleteMode = AutoCompleteMode.Append;
            TR_NPC_Name_TextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }
        #endregion Initialization
        #region Event Handlers
        #region Buttons
        #region Start & Stop Buttons
        private void TR_Start_Button_Click(object sender, EventArgs e)
        {
            if (Statics.Flags.ProcessState == 0)
            {
                MessageBox.Show("Cannot find pol process of given name. Check name and/or dual box checkbox");
            }
            else
            {
                if (!m_TOP_trInitDone)
                {
                    return;
                }
                if ((Trader1 == null) || (Trader1.State == Bots.Trader.TRADER_STATE.STOPPED))
                {
                    Item selectedItem = getCurrentTradeItem();
                    if (selectedItem == null)
                    {
                        MessageBox.Show("Please select an item first.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    LoggingFunctions.Timestamp("Starting Trader");
                    int nbOf = (int)TR_Nb_Of_Trades_UpDown.Value;
                    int nbPer = (int)TR_Nb_Per_Trade_UpDown.Value;
                    String[] scriptList = TR_getScriptList();
                    rebuildTraderInventory();
                    LoggingFunctions.Debug("TopTR::TR_Start_Button_Click: ******************* Trader start info: ********************", LoggingFunctions.DBG_SCOPE.TOP);
                    LoggingFunctions.Debug("TopTR::TR_Start_Button_Click: Item Name: " + selectedItem.Name + ".", LoggingFunctions.DBG_SCOPE.TOP);
                    LoggingFunctions.Debug("TopTR::TR_Start_Button_Click: Item ID: " + selectedItem.ItemID + ".", LoggingFunctions.DBG_SCOPE.TOP);
                    LoggingFunctions.Debug("TopTR::TR_Start_Button_Click: Number Of: " + nbOf + ".", LoggingFunctions.DBG_SCOPE.TOP);
                    Trader1 = new Bots.Trader(ChangeMonitor.MainProc, ChangeMonitor.MainModule, nbOf, nbPer, selectedItem,
                                         TR_NPC_Name_TextBox.Text, scriptList, CB_Start_Button, new Statics.FuncPtrs.TD_Void_String_Color(updateTraderStartButtonCBF));
                    Trader1.doInits();
                    Trader1.TradeDone += new Bots.Trader.TradeDoneEvent(Trader1_TradeDone);

                    m_TOP_Thread_trader = new Thread(new ThreadStart(Trader1.StartTrader));
                    m_TOP_Thread_trader.Name = "traderThread";
                    m_TOP_Thread_trader.IsBackground = true;
                    m_TOP_Thread_trader.Start();

                    TR_Start_Button.UseMnemonic = true;
                    TR_Start_Button.Text = "&Pause";
                    TR_Start_Button.BackColor = System.Drawing.Color.Yellow;
                }
                else if (Trader1.State == Bots.Trader.TRADER_STATE.RUNNING)
                {
                    LoggingFunctions.Timestamp("Pausing Trader");
                    Trader1.PauseTrader();
                    TR_Start_Button.UseMnemonic = true;
                    TR_Start_Button.Text = "&Resume";
                    TR_Start_Button.BackColor = System.Drawing.Color.Lime;
                }
                else if ((Trader1.State == Bots.Trader.TRADER_STATE.PAUSED_PROG)
                     || (Trader1.State == Bots.Trader.TRADER_STATE.PAUSED_USER))
                {
                    LoggingFunctions.Timestamp("Resuming Trader");
                    Trader1.ResumeTrader();
                    TR_Start_Button.UseMnemonic = true;
                    TR_Start_Button.Text = "&Pause";
                    TR_Start_Button.BackColor = System.Drawing.Color.Yellow;
                }
            }
        }

        private void TR_Stop_Button_Click(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_trInitDone)
            {
                return;
            }
            if (Trader1 != null)
            {
                LoggingFunctions.Timestamp("Stopping Trader");
                Trader1.StopTrader();
                while (Trader1.State != Bots.Trader.TRADER_STATE.STOPPED)
                {
                    IocaineFunctions.delay(500);
                }

                Trader1 = null;
            }
            else
            {
                Containers.Bag.RebuildLists();
            }
            TR_Start_Button.UseMnemonic = true;
            TR_Start_Button.Text = "S&tart";
            TR_Start_Button.BackColor = Statics.Buttons.Green;
        }
        #endregion Start & Stop Buttons
        private void TR_Refresh_Button_Click(object sender, EventArgs e)
        {
            rebuildTraderInventory();
        }
        #endregion Buttons
        #region Text Box
        private void TR_NPC_Name_TextBox_TextChanged(object sender, EventArgs e)
        {

        }
        private void TR_NPC_Name_TextBox_Leave(object sender, EventArgs e)
        {
            //Check if we have a script for this NPC.
            String currentNpc = TR_NPC_Name_TextBox.Text;
            if(!TR_BuiltInScripts.ContainsKey(currentNpc))
            {
                currentNpc = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(currentNpc);
                if(TR_BuiltInScripts.ContainsKey(currentNpc))
                {
                    TR_NPC_Name_TextBox.Text = currentNpc;
                }
            }
            if (TR_BuiltInScripts.ContainsKey(currentNpc))
            {
                List<String> savedScript = TR_BuiltInScripts[currentNpc];
                DialogResult result = System.Windows.Forms.DialogResult.Yes;
                if (TR_Script_ListBox.Items.Count > 0)
                {
                    bool scriptsDiffer = false;
                    //Check if the script box is different than the one we have saved.
                    if(savedScript.Count != TR_Script_ListBox.Items.Count)
                    {
                        scriptsDiffer = true;
                    }
                    else
                    {
                        //Saved and displayed scripts have the same length. Check line by line.
                        for(int ii=0; ii<TR_Script_ListBox.Items.Count; ii++)
                        {
                            String boxText = (String)TR_Script_ListBox.Items[ii];
                            if(boxText != savedScript[ii])
                            {
                                scriptsDiffer = true;
                                break;
                            }
                        }
                    }
                    if (scriptsDiffer)
                    {
                        //Check if the user wants to reset the box.
                        result = MessageBox.Show("Clear the current script and load \nthe selected built-in script?", "Clear Script?", MessageBoxButtons.YesNo);
                    }
                }
                if(result == System.Windows.Forms.DialogResult.Yes)
                {
                    TR_Script_ListBox.Items.Clear();
                    List<String> script = TR_BuiltInScripts[currentNpc];
                    foreach(String str in script)
                    {
                        TR_Script_ListBox.Items.Add(str);
                    }
                }
            }
        }
        #endregion Text Box
        #region Check List Box
        private void TR_Item_Box_CheckBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int index = e.Index;
            int nbItems = TR_Item_Box_CheckBox.Items.Count;
            String chkItem = TR_Item_Box_CheckBox.Items[index].ToString(); ;
            for (int ii = 0; ii < nbItems; ii++)
            {
                if (ii == index)
                {
                    continue;
                }
                TR_Item_Box_CheckBox.SetItemChecked(ii, false);
            }
            TR_setTradeQuanValue(getCurrentTradeQuan(chkItem));
        }
        #endregion Check List Box
        #region Check Boxes
        private void TR_Stop_On_Item_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            Statics.Settings.Helpers.StopOnItem_Trader = TR_Stop_On_Item_ChkB.Checked;
            if (!TR_StopOnItemChangedProg)
            {
                UserSettings.SetValue(UserSettings.BOT.HELPERS, "TR_StopOnItem", Statics.Settings.Helpers.StopOnItem_Trader.ToString());
            }
        }
        private void TR_Trade_From_Sack_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            TR_TradeFromSack = ((CheckBox)sender).Checked;
            TR_rebuildTraderInventoryChkBPtr();
        }
        private void TR_Trade_From_Satchel_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            TR_TradeFromSatchel = ((CheckBox)sender).Checked;
            TR_rebuildTraderInventoryChkBPtr();
        }
        private void TR_Trade_From_Case_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            TR_TradeFromCase = ((CheckBox)sender).Checked;
            TR_rebuildTraderInventoryChkBPtr();
        }
        #endregion Check Boxes
        #region Script Box Handlers
        private void TR_Wait_Time_Button_Click(object sender, EventArgs e)
        {
            TR_Script_ListBox.Items.Add("Wait - " + TR_Wait_Time_UpDown.Value.ToString());
        }
        private void TR_Enter_Button_Click(object sender, EventArgs e)
        {
            TR_Script_ListBox.Items.Add("Enter");
        }
        private void TR_Delete_Button_Click(object sender, EventArgs e)
        {
            int nbItems = TR_Script_ListBox.Items.Count;
            for (int ii = nbItems - 1; ii >= 0; ii--)
            {
                if (TR_Script_ListBox.GetSelected(ii))
                {
                    TR_Script_ListBox.Items.RemoveAt(ii);
                }
            }
        }
        #endregion Script Box Handlers
        #region Trader Updates
        void Trader1_TradeDone(Item item, int numberSoFar)
        {
            rebuildTraderInventory();
            //updateQuantities(recipe, numberSoFar);
        }
        #endregion Trader Updates
        #endregion Event Handlers
        #region Utility Functions
        #region GUI Updates
        private void TR_rebuildTraderInventoryChkBCBF()
        {
            List<Item> inventory = Containers.Bag.GetSummaryItemList();
            List<ushort> inventoryQuan = Containers.Bag.GetSummaryItemQuanList();
            if (TR_TradeFromSack == true)
            {
                for (int ii = 0; ii < Containers.Sack.GetSummaryItemList().Count; ii++)
                {
                    int invCnt = inventory.Count;
                    for (int kk = 0; kk < invCnt; kk++)
                    {
                        if (inventory[kk].Name == Containers.Sack.GetSummaryItemList()[ii].Name)
                        {
                            //Item in Sack is already in our inventory. Add the quantity.
                            inventoryQuan[kk] += Containers.Sack.GetSummaryItemQuanList()[ii];
                        }
                    }
                }
            }
            if (TR_TradeFromSatchel == true)
            {
                for (int ii = 0; ii < Containers.Satchel.GetSummaryItemList().Count; ii++)
                {
                    int invCnt = inventory.Count;
                    for (int kk = 0; kk < invCnt; kk++)
                    {
                        if (inventory[kk].Name == Containers.Satchel.GetSummaryItemList()[ii].Name)
                        {
                            //Item in Satchel is already in our inventory. Add the quantity.
                            inventoryQuan[kk] += Containers.Satchel.GetSummaryItemQuanList()[ii];
                        }
                    }
                }
            }
            if (TR_TradeFromCase == true)
            {
                for (int ii = 0; ii < Containers.MCase.GetSummaryItemList().Count; ii++)
                {
                    int invCnt = inventory.Count;
                    for (int kk = 0; kk < invCnt; kk++)
                    {
                        if (inventory[kk].Name == Containers.MCase.GetSummaryItemList()[ii].Name)
                        {
                            //Item in MCase is already in our inventory. Add the quantity.
                            inventoryQuan[kk] += Containers.MCase.GetSummaryItemQuanList()[ii];
                        }
                    }
                }
            }

            String checkedItem = "";
            if (TR_Item_Box_CheckBox.CheckedItems.Count > 0)
            {
                checkedItem = TR_Item_Box_CheckBox.CheckedItems[0].ToString();
            }
            TR_Item_Box_CheckBox.BeginUpdate();
            TR_Item_Box_CheckBox.Items.Clear();
            int nbItems = inventory.Count;
            for (int ii = 0; ii < nbItems; ii++)
            {
                String textToAdd = "(" + inventoryQuan[ii].ToString() + ")";
                bool chk = (inventory[ii].Name == checkedItem);
                textToAdd = textToAdd.PadRight(7);
                textToAdd += inventory[ii].Name;
                TR_Item_Box_CheckBox.Items.Add(textToAdd, chk);
            }
            TR_Item_Box_CheckBox.EndUpdate();
        }
        private void TR_setStopOnItemChkB(bool iValue)
        {
            try
            {
                if (TR_Stop_On_Item_ChkB.InvokeRequired)
                {
                    TR_Stop_On_Item_ChkB.Invoke(TR_setStopOnItemChkBPtr, new object[] { iValue });
                }
                else
                {
                    TR_setStopOnItemChkBCBF(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In TR_setStopOnItemChkB: " + e.ToString());
            }
        }
        private void TR_setStopOnItemChkBCBF(bool iValue)
        {
            TR_Stop_On_Item_ChkB.Checked = iValue;
            TR_StopOnItemChangedProg = false;
        }
        private void TR_setTradeQuanValue(UInt32 iValue)
        {
            try
            {
                if (TR_Nb_Of_Trades_UpDown.InvokeRequired)
                {
                    TR_Nb_Of_Trades_UpDown.Invoke(new Statics.FuncPtrs.TD_Void_UInt32(TR_setTradeQuanValueCBF), new object[] { iValue });
                }
                else
                {
                    TR_setTradeQuanValueCBF(iValue);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("TR_setTradeQuanValue:\r\n" + e.ToString());
            }
        }
        private void TR_setTradeQuanValueCBF(UInt32 iValue)
        {
            TR_Nb_Of_Trades_UpDown.Value = (Decimal)iValue;
        }
        #endregion GUI Updates
        #region General
        private Item getCurrentTradeItem()
        {
            if(TR_Item_Box_CheckBox.CheckedItems.Count == 0)
            {
                return null;
            }
            String name = TR_Item_Box_CheckBox.SelectedItem.ToString();
            //Trim the quantity and space off beginning
            int secondParenIndex = name.IndexOf(')');
            name = name.Substring(secondParenIndex + 1);
            name = name.Trim();

            ushort id = Containers.Bag.GetItemId(name);
            if ((id == 0) && (TR_TradeFromSack == true))
            {
                id = Containers.Sack.GetItemId(name);
            }
            if ((id == 0) && (TR_TradeFromSatchel == true))
            {
                id = Containers.Satchel.GetItemId(name);
            }
            if ((id == 0) && (TR_TradeFromCase == true))
            {
                id = Containers.MCase.GetItemId(name);
            }
            if (id == 0)
            {
                return null;
            }
            Things.ITEM_TYPE type = Things.GetTypeFromId(id);
            Item item = new Item(name, id, type);
            return item;
        }
        private UInt16 getCurrentTradeQuan(String iChkBString)
        {
            String name = iChkBString;
            //Trim the item name and parenthesis.
            int secondParenIndex = name.IndexOf(')');
            name = name.Substring(1, secondParenIndex-1);
            UInt16 quan = 0;
            if(!UInt16.TryParse(name, out quan))
            {
                return 0;
            }
            return quan;
        }
        private void rebuildTraderInventory()
        {
            Containers.RebuildListsMobileOnly();
            rebuildTraderInventoryCheckbox();
        }
        private void rebuildTraderInventoryCheckbox()
        {
            try
            {
                if (TR_Item_Box_CheckBox.InvokeRequired)
                {
                    TR_Item_Box_CheckBox.Invoke(TR_rebuildTraderInventoryChkBPtr);
                }
                else
                {
                    TR_rebuildTraderInventoryChkBCBF();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Rebuilding Trader inventory box: " + e.ToString());
            }
        }
        private int calculateMaxTrades(Item itemToTrade)
        {
            LoggingFunctions.Debug("TopTR::calculateMaxTrades: Recalculating max trades.", LoggingFunctions.DBG_SCOPE.TOP);
            List<Item> bagSummary = Containers.Bag.GetSummaryItemList();
            List<ushort> bagSumQuan = Containers.Bag.GetSummaryItemQuanList();
            List<Item> sackSummary = Containers.Sack.GetSummaryItemList();
            List<ushort> sackSumQuan = Containers.Sack.GetSummaryItemQuanList();
            List<Item> satchelSummary = Containers.Satchel.GetSummaryItemList();
            List<ushort> satchelSumQuan = Containers.Satchel.GetSummaryItemQuanList();
            List<Item> caseSummary = Containers.MCase.GetSummaryItemList();
            List<ushort> caseSumQuan = Containers.MCase.GetSummaryItemQuanList();
            int totalCount = 0;
            int bagCount = bagSummary.Count;
            for (int ii = 0; ii < bagCount; ii++)
            {
                if (bagSummary[ii].Name == itemToTrade.Name)
                {
                    totalCount += bagSumQuan[ii];
                }
            }
            if (TR_TradeFromSack == true)
            {
                for (int ii = 0; ii < sackSummary.Count; ii++)
                {
                    if (sackSummary[ii].Name == itemToTrade.Name)
                    {
                        totalCount += sackSumQuan[ii];
                    }
                }
            }
            if (TR_TradeFromSatchel == true)
            {
                for (int ii = 0; ii < satchelSummary.Count; ii++)
                {
                    if (satchelSummary[ii].Name == itemToTrade.Name)
                    {
                        totalCount += satchelSumQuan[ii];
                    }
                }
            }
            if (TR_TradeFromCase == true)
            {
                for (int ii = 0; ii < caseSummary.Count; ii++)
                {
                    if (caseSummary[ii].Name == itemToTrade.Name)
                    {
                        totalCount += caseSumQuan[ii];
                    }
                }
            }
            return totalCount / (int)TR_Nb_Of_Trades_UpDown.Value;
        }
        private String[] TR_getScriptList()
        {
            int nbItems = TR_Script_ListBox.Items.Count;
            String[] list = new String[nbItems];
            for (int ii = 0; ii < nbItems; ii++)
            {
                LoggingFunctions.Debug("TopTR::TR_getScriptList: Adding " + (String)TR_Script_ListBox.Items[ii] + ".", LoggingFunctions.DBG_SCOPE.TOP);
                list[ii] = (String)TR_Script_ListBox.Items[ii];
            }
            return list;
        }
        #endregion General
        #endregion Utility Functions
    }
}