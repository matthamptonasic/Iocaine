using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;

namespace Iocaine2.Tools
{
    public static class Interaction
    {
        #region Enums
        public enum BUY_RETURN_CODE : byte
        {
            NORMAL = 0,
            NO_STOCK = 1,
            CLOSED = 2,
            WINDOW_ISSUE = 3,
            STOPPED = 4,
            INV_FULL = 5
        }
        #endregion Enums
        #region Targeting
        public static bool TargetPlayer(String iPlayerName, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            LoggingFunctions.Debug("Targetting " + iPlayerName, LoggingFunctions.DBG_SCOPE.INTERACTION);
            uint timeout = 10;
            uint cnt = 0;
            while (true)
            {
                if ((iCheckStatus != null) && (!iCheckStatus()))
                {
                    return false;
                }
                String tempName = MemReads.Target.get_name();
                if (tempName != iPlayerName)
                {
                    IocaineFunctions.keys("/target " + iPlayerName);
                    IocaineFunctions.delay(300);
                }
                tempName = MemReads.Target.get_name();
                LoggingFunctions.Debug("Trying to target " + iPlayerName + " and read target name of " + tempName, LoggingFunctions.DBG_SCOPE.INTERACTION);
                if (tempName != iPlayerName)
                {
                    if (cnt == timeout)
                    {
                        return false;
                    }
                    IocaineFunctions.delay(1000);
                    cnt++;
                }
                else
                {
                    return true;
                }
            }
        }
        public static bool TargetPlayer(String iPlayerName)
        {
            return TargetPlayer(iPlayerName, null);
        }
        public static bool TargetNPC(String iNpcName, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            return TargetNPC(new List<String>() { iNpcName }, iCheckStatus);
        }
        public static bool TargetNPC(List<String> iNpcNameList, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            LoggingFunctions.Debug("Entering TargetNPC", LoggingFunctions.DBG_SCOPE.INTERACTION);
            String currentTarget = MemReads.Target.get_name();
            UInt16 currentTargetId = 0;
            if (iNpcNameList.Contains(currentTarget))
            {
                return true;
            }
            Dictionary<UInt16, String> namesTargeted = new Dictionary<UInt16, String>();
            Dictionary<UInt16, Byte> timesTargeted = new Dictionary<UInt16, Byte>();
            Player_MenuNavigation.CloseCheck();
            //First need to try to target anything to put something in list
            IocaineFunctions.keyDown(Keys.Tab, 100);
            IocaineFunctions.delay(400);
            currentTarget = MemReads.Target.get_name();
            currentTargetId = MemReads.Target.get_id();
            //If the target name is less than 3 characters
            LoggingFunctions.Debug("Initial target is " + currentTarget, LoggingFunctions.DBG_SCOPE.INTERACTION);
            if (currentTarget.Length < 3)
            {
                //AND if the target distance is 0, then you couldn't target anything, return false
                if (MemReads.Target.get_distance() == 0)
                {
                    return false;
                }
            }
            else if (iNpcNameList.Contains(currentTarget))
            {
                return true;
            }
            namesTargeted[currentTargetId] = currentTarget;
            timesTargeted[currentTargetId] = 1;
            bool allTargetedTwice = false;
            while (!allTargetedTwice)
            {
                if ((iCheckStatus != null) && (!iCheckStatus()))
                {
                    return false;
                }
                IocaineFunctions.keyDown(Keys.Tab, 100);
                IocaineFunctions.delay(250);
                currentTarget = MemReads.Target.get_name();
                currentTargetId = MemReads.Target.get_id();
                LoggingFunctions.Debug("Next target is " + currentTarget, LoggingFunctions.DBG_SCOPE.INTERACTION);
                if (iNpcNameList.Contains(currentTarget))
                {
                    LoggingFunctions.Debug("Found our target, returning", LoggingFunctions.DBG_SCOPE.INTERACTION);
                    return true;
                }
                else
                {
                    //If we haven't targeted this object before, add to the list
                    if (!namesTargeted.Keys.Contains(currentTargetId))
                    {
                        namesTargeted[currentTargetId] = currentTarget;
                        timesTargeted[currentTargetId] = 1;
                    }
                    //If we HAVE targeted this object before, increment that object's count
                    else
                    {
                        timesTargeted[currentTargetId]++;
                    }

                    //Now check to see if everyone has been targeted twice
                    allTargetedTwice = true;
                    foreach(UInt16 id in namesTargeted.Keys)
                    {
                        if (timesTargeted[id] == 1)
                        {
                            allTargetedTwice = false;
                            break;
                        }
                    }
                }
            }
            return false;
        }
        public static bool TargetNPC(String iNpcName)
        {
            return TargetNPC(iNpcName, null);
        }
        /// <summary>
        /// Targets an NPC, identifying it by an ID. 
        /// Indirect mode used (uses keyboard commands to toggle through targets, instead of writing the target into the memory.
        /// </summary>
        /// <param name="targetNpcId">The ID of the target NPC</param>
        /// <returns>bool indicating success</returns>
        public static bool TargetNPCIndirectById(ushort targetNpcId, Statics.FuncPtrs.TD_Bool_Void iCheckStatus = null )
        {
            LoggingFunctions.Debug("Entering TargetNPCIndirectlyById", LoggingFunctions.DBG_SCOPE.INTERACTION);
            if (targetNpcId == MemReads.NPCs.InvalidNpcId)
            {
                return false;
            }

            MemReads.Target.TargetLockStruct TargetLockInfo = new MemReads.Target.TargetLockStruct();
            if (MemReads.Target.get_target_lock_struct(ref TargetLockInfo) == false)
            {
                return false;
            }
            if (TargetLockInfo.StructID == targetNpcId)
            {
                return true;
            }

            Dictionary<ushort, byte> targetedList = new Dictionary<ushort,byte>();

            Player_MenuNavigation.CloseCheck();
            //First need to try to target anything to put something in list
            IocaineFunctions.keyDown(Keys.Tab, 100);
            IocaineFunctions.delay(400);

            if (MemReads.Target.get_target_lock_struct(ref TargetLockInfo) == false)
            {
                return false;
            }
            // Check whether we are targetting anything.
            if (TargetLockInfo.StructID == MemReads.Target.InvalidStructId)
            {
                return false;
            }
            targetedList[TargetLockInfo.StructID] = 1;
            bool allTargetedTwice = false;
            while (allTargetedTwice == false)
            {
                if ((iCheckStatus != null) && (!iCheckStatus()))
                {
                    return false;
                }
                IocaineFunctions.keyDown(Keys.Tab, 100);
                IocaineFunctions.delay(250);

                if (MemReads.Target.get_target_lock_struct(ref TargetLockInfo) == false)
                {
                    return false;
                }
                if (TargetLockInfo.StructID == MemReads.Target.InvalidStructId )
                {
                    return false;
                }

                LoggingFunctions.Debug("Next target ID is " + TargetLockInfo.StructID, LoggingFunctions.DBG_SCOPE.INTERACTION);
                if (TargetLockInfo.StructID == targetNpcId)
                {
                    return true;
                }
                else
                {
                    if( targetedList.ContainsKey(TargetLockInfo.StructID) )
                    {
                        targetedList[TargetLockInfo.StructID]++;
                    }
                    else
                    {
                        targetedList[TargetLockInfo.StructID] = 1;
                    }
                    //Now check to see if everyone has been targeted twice
                    allTargetedTwice = (targetedList.ContainsValue(1) == false && targetedList.Count() > 0);
                }
            }
            return false;
        }
        #endregion Targeting
        #region Trading
        public static bool TradeItemToNpc(UInt16 iItemId, byte iItemQuan, String iNpcName)
        {
            //This function trades 1 item at a time to an npc.
            if (!Interaction.TargetNPC(iNpcName))
            {
                return false;
            }
            else
            {
                IocaineFunctions.delay(1000);
                return TradeItemToNpc(iItemId, iItemQuan);
            }
        }
        public static bool TradeItemToNpc(UInt16 iItemId, byte iItemQuan)
        {
            //This function trades 1 item at a time to an npc.
            List<UInt16> itemList = new List<ushort>() { iItemId };
            List<byte> itemQuanList = new List<byte>() { iItemQuan };
            return TradeItemToNpc(itemList, itemQuanList);
        }
        public static bool TradeItemToNpc(List<UInt16> iItemIdList, List<byte> iItemQuanList, String iNpcName)
        {
            //This function trade all given items to an npc at once (8 items max).
            if (!Interaction.TargetNPC(iNpcName))
            {
                return false;
            }
            else
            {
                IocaineFunctions.delay(1000);
                return TradeItemToNpc(iItemIdList, iItemQuanList);
            }
        }
        public static bool TradeItemToNpc(List<UInt16> iItemIdList, List<byte> iItemQuanList)
        {
            //This function trade all given items to an npc at once (8 items max).
            if ((iItemIdList.Count > 8) || (iItemIdList.Count != iItemQuanList.Count))
            {
                LoggingFunctions.Error("In TradeItemToNpc: Lists were not equal in length or length was greater than 8.");
                return false;
            }
            if (!Player_MenuNavigation.GotoMenuItem("Trade", true))
            {
                LoggingFunctions.Error("In TradeItemToNpc: Could not find trade menu item.");
                return false;
            }
            if (!Player_MenuNavigation.SetNpcTradeItems(iItemIdList, iItemQuanList))
            {
                LoggingFunctions.Error("In TradeItemToNpc: Could not set trade window items.");
                return false;
            }
            Player_MenuNavigation.HitOK();
            return true;
        }
        public static bool TradeGilNpc(UInt32 iGilQuan, String iNpcName)
        {
            if (!Interaction.TargetNPC(iNpcName))
            {
                return false;
            }
            else
            {
                return TradeGilNpc(iGilQuan);
            }
        }
        public static bool TradeGilNpc(UInt32 iGilQuan)
        {
            if (!Player_MenuNavigation.GotoMenuItem("Trade", true))
            {
                LoggingFunctions.Error("In TradeGilNpc: Could not find trade menu item.");
                return false;
            }
            if (!Player_MenuNavigation.SetNpcTradeGil(iGilQuan))
            {
                LoggingFunctions.Error("In TradeGilNpc: Could not set trade window gil.");
                return false;
            }
            Player_MenuNavigation.HitOK();
            return true;
        }
        #endregion Trading
        #region Buy/Sell
        public static bool GetToBuySellDialog(bool iBuy, bool iGuildNpc, String iNpcName, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            String errMsg = "";
            if ((iBuy == false) && (iGuildNpc == true))
            {
                errMsg = "Selling to a guild npc is currently not supported.";
                LoggingFunctions.Error(errMsg);
                MessageBox.Show(errMsg);
                return false;
            }
            String topLeft = MemReads.Windows.BannerText.get_top_left_text();
            if (((topLeft != "Shop") && !iGuildNpc) || ((topLeft != "Guild Shop") && iGuildNpc))
            {
                Statics.FuncPtrs.SetStatusBoxPtr("Opening merchant dialog window.", Statics.Fields.Green);
                if (topLeft != "N/A")
                {
                    Player_MenuNavigation.CloseCheck(2);
                }
                if (!TargetNPC(iNpcName, iCheckStatus))
                {
                    errMsg = "Could not target NPC '" + iNpcName + "'";
                    LoggingFunctions.Error(errMsg);
                    MessageBox.Show(errMsg);
                    return false;
                }
                if ((iCheckStatus != null) && (!iCheckStatus()))
                {
                    return false;
                }
                //Enter on NPC
                IocaineFunctions.keyDown(Keys.Enter, Player_MenuNavigation.KeyDownTimeEnter);
                IocaineFunctions.delay(Statics.Settings.Helpers.EnterNpcMenuDelay);
                //Buy or Sell should be displayed now.
                Int16 menuIdx = MemReads.Windows.Menus.ButtonStyle.get_curr_index();
                if ((iBuy && (menuIdx != 0)) || (!iBuy && (menuIdx != 1)))
                {
                    //We're not on the right button. Hit down.
                    IocaineFunctions.arrowKeyDown(Keys.Down, Player_MenuNavigation.KeyDownTimeArrow);
                    IocaineFunctions.delay(200);
                }
                //There's no way to tell which is selected, so just select one and if it's the wrong one we'll back out.
                IocaineFunctions.keyDown(Keys.Enter, Player_MenuNavigation.KeyDownTimeEnter);
                IocaineFunctions.delay(250);
                //We should now either be in the buy or sell menu.
                topLeft = MemReads.Windows.BannerText.get_top_left_text();
                if (((topLeft != "Shop") && !iGuildNpc) || ((topLeft != "Guild Shop") && iGuildNpc))
                {
                    errMsg = "Expected to be inside a buy or sell dialog, but the top left window text was not '";
                    if (iGuildNpc)
                    {
                        errMsg += "Guild Shop'";
                    }
                    else
                    {
                        errMsg += "Shop'";
                    }
                    LoggingFunctions.Error(errMsg);
                    MessageBox.Show(errMsg);
                    return false;
                }
                if ((iCheckStatus != null) && (!iCheckStatus()))
                {
                    return false;
                }
                IocaineFunctions.delay(500);
                byte inventoryIndex = MemReads.Windows.Items.get_selected_item_inventory_index();
                if (((inventoryIndex == 0) && !iBuy) || ((inventoryIndex != 0) && iBuy))
                {
                    //If the selected item has an inventory index of 0, it means that we're in the buy menu.
                    //Or if the index is not 0, we're in our inventory, so it's the sell menu.
                    //So go back 1 menu and arrow down to get to the other menu.
                    IocaineFunctions.keyDown(System.Windows.Forms.Keys.Escape, Player_MenuNavigation.KeyDownTimeEscape);
                    IocaineFunctions.delay(250);
                    IocaineFunctions.arrowKeyDown(Keys.Down, Player_MenuNavigation.KeyDownTimeArrow);
                    IocaineFunctions.delay(250);
                    IocaineFunctions.keyDown(Keys.Enter, Player_MenuNavigation.KeyDownTimeEnter);
                    IocaineFunctions.delay(250);
                    if ((iCheckStatus != null) && (!iCheckStatus()))
                    {
                        return false;
                    }
                }
                //We should be back into the OTHER buy or sell menu.
                topLeft = MemReads.Windows.BannerText.get_top_left_text();
                if (((topLeft != "Shop") && !iGuildNpc) || ((topLeft != "Guild Shop") && iGuildNpc))
                {
                    errMsg = "Expected to be inside a buy or sell dialog, but the top left window text was not '";
                    if (iGuildNpc)
                    {
                        errMsg += "Guild Shop'";
                    }
                    else
                    {
                        errMsg += "Shop'";
                    }
                    LoggingFunctions.Error(errMsg);
                    MessageBox.Show(errMsg);
                    return false;
                }
                inventoryIndex = MemReads.Windows.Items.get_selected_item_inventory_index();
                if (((inventoryIndex == 0) && !iBuy) || ((inventoryIndex != 0) && iBuy))
                {
                    errMsg = "Expected to be inside a buy or sell dialog, but the memory reads do not show that we are. Inventory Index is " + inventoryIndex;
                    LoggingFunctions.Error(errMsg);
                    MessageBox.Show(errMsg);
                    return false;
                }
                if ((iCheckStatus != null) && (!iCheckStatus()))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool GetToBuySellDialog(bool iBuy, bool iGuildNpc, String iNpcName)
        {
            return GetToBuySellDialog(iBuy, iGuildNpc, iNpcName, null);
        }
        public static bool SellSelectedItem(String iItemName, ushort iQuan, bool iFirstItemBeingSold, ref byte oActualSold, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            UInt32 prevGil = MemReads.Self.Inventory.get_gil();
            if ((iCheckStatus != null) && (!iCheckStatus()))
            {
                return false;
            }
            //It seems that previous to the October update of 2013, the "Sell merchandise" banner text
            //was not displayed until you actually get to the Sell/Cancel menu options.
            //Now it is displayed earlier, so we can't use that to indicate that we're
            //in the Sell/Cancel menu or not.
            //The button menu index now indicates the current bag location you are at.
            //So if we think we're in the Sell/Cancel menu, and we either:
            //  1. Above an index of 1, we're not in the Sell/Cancel menu.
            //  2. We press down when we are 1 and it goes back to 0, we ARE in the Sell/Cancel menu.
            //Implementing that here and in getToSellMenuButton().
            String helpText = MemReads.Windows.BannerText.get_help_text();
            if (iFirstItemBeingSold)
            {
                IocaineFunctions.keyDown(Keys.Enter, Player_MenuNavigation.KeyDownTimeEnter);
                IocaineFunctions.delay(Statics.Settings.Helpers.PressEnterToSellDelay);
            }
            String itemName = MemReads.Windows.Items.get_selected_item_name();
            if ((itemName != iItemName) && (itemName != "N/A"))
            {
                return false;
            }

            IocaineFunctions.keyDown(Keys.Enter, Player_MenuNavigation.KeyDownTimeEnter);
            IocaineFunctions.delay(Statics.Settings.Helpers.CheckHelpTextDelay);

            //Now we need to determine if we are selling more than 1 or not so we
            //can get to the Sell/Cancel menu as expected.
            byte currentCnt = MemReads.Windows.Items.get_shop_quan_cur();
            byte maxCnt = MemReads.Windows.Items.get_shop_quan_max();
            if (maxCnt > 1)
            {
                if (iQuan >= maxCnt)
                {
                    IocaineFunctions.arrowKeyDown(Keys.Left, Player_MenuNavigation.KeyDownTimeArrow);
                    IocaineFunctions.delay(Statics.Settings.Helpers.ChangeSellQuanDelay);
                }
                else
                {
                    //We have multiple to sell, so keep hitting up until we get the number we need or we reach the max.
                    while ((currentCnt < iQuan) && (currentCnt < maxCnt))
                    {
                        if ((iCheckStatus != null) && (!iCheckStatus()))
                        {
                            return false;
                        }
                        IocaineFunctions.arrowKeyDown(Keys.Up, Player_MenuNavigation.KeyDownTimeArrow);
                        IocaineFunctions.delay(Statics.Settings.Helpers.ChangeSellQuanDelay);
                        currentCnt = MemReads.Windows.Items.get_shop_quan_cur();
                    }
                }
                oActualSold = MemReads.Windows.Items.get_shop_quan_cur();
                IocaineFunctions.keyDown(Keys.Enter, Player_MenuNavigation.KeyDownTimeEnter);
                //Use same as check help text. That will make the delay same as if we were selling non-stackable item.
                IocaineFunctions.delay(Statics.Settings.Helpers.CheckHelpTextDelay);
            }
            else
            {
                oActualSold = 1;
            }

            if (!getToSellMenuButton())
            {
                //We're not in the Sell/Cancel menu. Try pressing Enter again and rechecking.
                IocaineFunctions.keyDown(Keys.Enter, Player_MenuNavigation.KeyDownTimeEnter);
                IocaineFunctions.delay(Statics.Settings.Helpers.CheckHelpTextDelay);
                if (!getToSellMenuButton())
                {
                    //If we're still not in the menu, there's something wrong. Exit out with an error.
                    String errMsg = "SellSelectedItem::Couldn't get to the Sell/Cancel Menu.";
                    LoggingFunctions.Error(errMsg);
                    return false;
                }
            }

            if ((iCheckStatus != null) && (!iCheckStatus()))
            {
                return false;
            }

            if ((iCheckStatus != null) && (!iCheckStatus()))
            {
                return false;
            }
            IocaineFunctions.keyDown(Keys.Enter, Player_MenuNavigation.KeyDownTimeEnter);
            IocaineFunctions.delay(Statics.Settings.Top.MoveUpDownDelay);
            UInt32 afterGil = MemReads.Self.Inventory.get_gil();
            if (afterGil == prevGil)
            {
                oActualSold = 0;
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool SellSelectedItem(String iItemName, ushort iQuan, bool iFirstItem, ref byte oActualSold)
        {
            return SellSelectedItem(iItemName, iQuan, iFirstItem, ref oActualSold, null);
        }
        private static bool getToSellMenuButton()
        {
            Int16 menuIdx = MemReads.Windows.Menus.ButtonStyle.get_curr_index();
            if (menuIdx > 1)
            {
                //We're obviously not in the Sell menu.
                String errMsg = "getToSellMenuButton::Not in the Sell/Cancel Menu.";
                LoggingFunctions.Debug(errMsg, LoggingFunctions.DBG_SCOPE.SELLER);
                return false;
            }
            else if (menuIdx == 1)
            {
                //Pressing down should wrap up to 0.
                IocaineFunctions.arrowKeyDown(Keys.Down, Player_MenuNavigation.KeyDownTimeArrow);
                IocaineFunctions.delay(Statics.Settings.Helpers.CheckHelpTextDelay);
                menuIdx = MemReads.Windows.Menus.ButtonStyle.get_curr_index();
                if (menuIdx != 0)
                {
                    //If it's 2, we may still be in the Bag, so press up once to get us back on the item.
                    IocaineFunctions.arrowKeyDown(Keys.Up, Player_MenuNavigation.KeyDownTimeArrow);
                    IocaineFunctions.delay(Statics.Settings.Helpers.CheckHelpTextDelay);
                    //We're obviously not in the Sell menu.
                    String errMsg = "getToSellMenuButton::Pressing down from 1 did not go to 0.";
                    LoggingFunctions.Debug(errMsg, LoggingFunctions.DBG_SCOPE.SELLER);
                    return false;
                }
                return true;
            }
            else
            {
                //To make sure we're really in the menu, press up once and the index should wrap up to 1.
                IocaineFunctions.arrowKeyDown(Keys.Up, Player_MenuNavigation.KeyDownTimeArrow);
                IocaineFunctions.delay(Statics.Settings.Helpers.CheckHelpTextDelay);
                menuIdx = MemReads.Windows.Menus.ButtonStyle.get_curr_index();
                if (menuIdx != 1)
                {
                    //We're obviously not in the Sell menu.
                    String errMsg = "getToSellMenuButton::Pressing up from 0 did not go to 1.";
                    LoggingFunctions.Debug(errMsg, LoggingFunctions.DBG_SCOPE.SELLER);
                    return false;
                }
                else
                {
                    //We ARE in the menu. Just hit back up again to get to 0 and return.
                    IocaineFunctions.arrowKeyDown(Keys.Up, Player_MenuNavigation.KeyDownTimeArrow);
                    IocaineFunctions.delay(Statics.Settings.Helpers.CheckHelpTextDelay);
                    return true;
                }
            }
            
        }
        public static UInt16 BuyItemFromNPC(bool iGuild, UInt16 iItemId, UInt16 iQuan, UInt16 iItemIndex, bool iWriteIndex, ChatLoggerAsync iChatLogger, Statics.FuncPtrs.TD_Bool_Void iCheckStatus, ref BUY_RETURN_CODE oCode)
        {
            #region Summary
            //This function assumes that you are already in the proper NPC Buy menu.
            //It will go through the menu and:
            //1. Select the item based on the iItemId.
            //2. Purchase the item.
            //3. Repeat until there is no more stock (guild only) or iQuan is reached.
            //It will parse the chat log for:
            //1. "Please wait longer before making another purchase." (guild only)
            //  In this case it will wait a short amount of time and try again.
            //  It will also bump up the Statics.Settings.Helpers.GuildWaitTime_Buyer value by 100ms.
            //2. "You were unable to carry out that transaction.".
            //  In this case it will assume that there is no more stock of this item (guild only) and return.
            //3. "The shop is closed."
            //  In this case we will just return.
            #endregion Summary
            #region Select Item
            if (Inventory.Containers.Bag.Full)
            {
                oCode = BUY_RETURN_CODE.INV_FULL;
                return 0;
            }
            UInt16 quanPurchased = 0;
            String chatWaitLonger = "Please wait longer before making another purchase";
            String chatUnable = "You were unable to carry out that transaction";
            String chatClosed = "The shop is closed.";
            String chatFull = "Your inventory is full.";
            //Make sure we're currently in the right window.
            String topLeft = MemReads.Windows.BannerText.get_top_left_text();
            if ((iGuild && (topLeft != "Guild Shop")) || (!iGuild && (topLeft != "Shop")))
            {
                oCode = BUY_RETURN_CODE.WINDOW_ISSUE;
                return quanPurchased;
            }
            String selectedItemName = MemReads.Windows.Items.get_selected_item_name();
            if (selectedItemName == "N/A")
            {
                oCode = BUY_RETURN_CODE.WINDOW_ISSUE;
                return quanPurchased;
            }
            if (iWriteIndex)
            {
                MemReads.Windows.Shops.ItemWindow.set_cur_idx(iItemIndex);
                IocaineFunctions.delay(Statics.Settings.Top.MoveUpDownDelay);
            }
            selectedItemName = MemReads.Windows.Items.get_selected_item_name();
            String desiredItemName = Things.GetNameFromId(iItemId);
            if ((iCheckStatus != null) && (!iCheckStatus()))
            {
                oCode = BUY_RETURN_CODE.STOPPED;
                return quanPurchased;
            }
            if (!iWriteIndex || (selectedItemName != desiredItemName))
            {
                //Manually go to the item in question.
                //Start by jumping screens to move 10 at a time, then move down 1 at a time based on the given index.
                //If this doesn't work, we'll move back up to the top and go 1 by 1.
                //If we hit the bottom after going 1 at a time and we didn't find the item, just return.
                Byte nbScreensToJump = (Byte)(iItemIndex / 10);
                for (Byte ii = 0; ii < nbScreensToJump; ii++)
                {
                    IocaineFunctions.arrowKeyDown(Keys.Right, Player_MenuNavigation.KeyDownTimeArrow);
                    IocaineFunctions.delay(100);
                }
                if ((iCheckStatus != null) && (!iCheckStatus()))
                {
                    oCode = BUY_RETURN_CODE.STOPPED;
                    return quanPurchased;
                }
                //If the number of items is not a integer multiple of 10, then when you use the 'right' key to skip a page
                //and you get to the bottom, the index of the top item will be the number of items - 9.
                UInt16 curIdx = MemReads.Windows.Shops.ItemWindow.get_cur_idx();
                //Int16 nbDownPresses = (Int16)(iItemIndex - (nbScreensToJump * 10) - 1);
                Int16 nbDownPresses = (Int16)(iItemIndex - curIdx);
                for (int ii = 0; ii < nbDownPresses; ii++)
                {
                    IocaineFunctions.arrowKeyDown(Keys.Down, Player_MenuNavigation.KeyDownTimeArrow);
                    IocaineFunctions.delay(75);
                    if ((iCheckStatus != null) && (!iCheckStatus()))
                    {
                        oCode = BUY_RETURN_CODE.STOPPED;
                        return quanPurchased;
                    }
                }
                curIdx = MemReads.Windows.Shops.ItemWindow.get_cur_idx();
                UInt16 nbItems = MemReads.Windows.Shops.ItemWindow.get_nb_listed_items();
                selectedItemName = MemReads.Windows.Items.get_selected_item_name();
                bool atBottom = curIdx == nbItems;
                bool foundItem = selectedItemName == desiredItemName;
                while (!atBottom && !foundItem)
                {
                    IocaineFunctions.arrowKeyDown(Keys.Down, Player_MenuNavigation.KeyDownTimeArrow);
                    IocaineFunctions.delay(Statics.Settings.Top.MoveUpDownDelay);
                    curIdx = MemReads.Windows.Shops.ItemWindow.get_cur_idx();
                    selectedItemName = MemReads.Windows.Items.get_selected_item_name();
                    atBottom = curIdx == nbItems;
                    foundItem = selectedItemName == desiredItemName;
                    if ((iCheckStatus != null) && (!iCheckStatus()))
                    {
                        oCode = BUY_RETURN_CODE.STOPPED;
                        return quanPurchased;
                    }
                }
                if (!foundItem)
                {
                    //Start back at the top and walk through them all 1 at a time.
                    int timeout = 50;
                    while ((MemReads.Windows.Shops.ItemWindow.get_cur_idx() != 1) && (timeout > 0))
                    {
                        IocaineFunctions.arrowKeyDown(Keys.Left, Player_MenuNavigation.KeyDownTimeArrow);
                        IocaineFunctions.delay(100);
                        timeout--;
                        if ((iCheckStatus != null) && (!iCheckStatus()))
                        {
                            oCode = BUY_RETURN_CODE.STOPPED;
                            return quanPurchased;
                        }
                    }
                    curIdx = MemReads.Windows.Shops.ItemWindow.get_cur_idx();
                    for (int kk = curIdx; kk < nbItems; kk++)
                    {
                        selectedItemName = MemReads.Windows.Items.get_selected_item_name();
                        while (selectedItemName == "N/A")
                        {
                            selectedItemName = MemReads.Windows.Items.get_selected_item_name();
                            IocaineFunctions.delay(10);
                        }
                        if (selectedItemName == desiredItemName)
                        {
                            foundItem = true;
                            break;
                        }
                        IocaineFunctions.arrowKeyDown(Keys.Down, Player_MenuNavigation.KeyDownTimeArrow);
                        IocaineFunctions.delay(Statics.Settings.Top.MoveUpDownDelay);
                        if ((iCheckStatus != null) && (!iCheckStatus()))
                        {
                            oCode = BUY_RETURN_CODE.STOPPED;
                            return quanPurchased;
                        }
                    }
                    if (!foundItem)
                    {
                        oCode = BUY_RETURN_CODE.WINDOW_ISSUE;
                        return quanPurchased;
                    }
                }
            }
            if ((iCheckStatus != null) && (!iCheckStatus()))
            {
                oCode = BUY_RETURN_CODE.STOPPED;
                return quanPurchased;
            }
            #endregion Select Item
            #region Buy Item
            //By now we should have the desired item selected, so start buying.
            if (iChatLogger == null)
            {
                iChatLogger = ChatLogManager.Access.GetAsynchronousLogger("Buyer");
            }
            iChatLogger.Reset();
            UInt32 prevGil = MemReads.Self.Inventory.get_gil();
            while (quanPurchased < iQuan)
            {
                if (Inventory.Containers.Bag.LiveFull)
                {
                    oCode = BUY_RETURN_CODE.INV_FULL;
                    return quanPurchased;
                }
                UInt16 quanRemaining = (UInt16)(iQuan - quanPurchased);
                UInt16 tempQuanPurchased = 0;
                IocaineFunctions.keyDown(Keys.Enter, Player_MenuNavigation.KeyDownTimeEnter);
                IocaineFunctions.delay(Statics.Settings.Helpers.CheckHelpTextDelay);
                if ((iCheckStatus != null) && (!iCheckStatus()))
                {
                    oCode = BUY_RETURN_CODE.STOPPED;
                    return quanPurchased;
                }
                String helpText = MemReads.Windows.BannerText.get_help_text();
                if (helpText != "Purchase merchandise.")
                {
                    //This should only happen if the guild is out of stock of this item.
                    //We'll try hitting enter again. If that doesn't work, just return.
                    IocaineFunctions.keyDown(Keys.Enter, Player_MenuNavigation.KeyDownTimeEnter);
                    IocaineFunctions.delay(Statics.Settings.Helpers.CheckHelpTextDelay);
                    if (helpText != "Purchase merchandise.")
                    {
                        String errMsg = "Could not get the 'Purchase merchandise.' help text to display for item '" + desiredItemName + "'.";
                        LoggingFunctions.Timestamp(errMsg);
                        oCode = BUY_RETURN_CODE.WINDOW_ISSUE;
                        return quanPurchased;
                    }
                }
                //Now we need to determine if we are buying more than 1 or not.
                byte currentCnt = MemReads.Windows.Items.get_shop_quan_cur();
                byte maxCnt = MemReads.Windows.Items.get_shop_quan_max();
                if (maxCnt > 1)
                {
                    if (quanRemaining >= maxCnt)
                    {
                        if ((iCheckStatus != null) && (!iCheckStatus()))
                        {
                            oCode = BUY_RETURN_CODE.STOPPED;
                            return quanPurchased;
                        }
                        //Buy the maximum amount.
                        IocaineFunctions.arrowKeyDown(Keys.Left, Player_MenuNavigation.KeyDownTimeArrow);
                        IocaineFunctions.delay(Statics.Settings.Helpers.ChangeSellQuanDelay);
                    }
                    else
                    {
                        //We have multiple to buy, but not the max amount, so keep hitting up until we get the number we need or we reach the max.
                        while ((currentCnt < quanRemaining) && (currentCnt < maxCnt))
                        {
                            if ((iCheckStatus != null) && (!iCheckStatus()))
                            {
                                oCode = BUY_RETURN_CODE.STOPPED;
                                return quanPurchased;
                            }
                            IocaineFunctions.arrowKeyDown(Keys.Up, Player_MenuNavigation.KeyDownTimeArrow);
                            IocaineFunctions.delay(Statics.Settings.Helpers.ChangeSellQuanDelay);
                            currentCnt = MemReads.Windows.Items.get_shop_quan_cur();
                        }
                    }
                    tempQuanPurchased = MemReads.Windows.Items.get_shop_quan_cur();
                    IocaineFunctions.keyDown(Keys.Enter, Player_MenuNavigation.KeyDownTimeEnter);
                    //Use same as check help text. That will make the delay same as if we were selling non-stackable item.
                    IocaineFunctions.delay(Statics.Settings.Helpers.CheckHelpTextDelay);
                }
                else
                {
                    tempQuanPurchased = 1;
                }
                if ((iCheckStatus != null) && (!iCheckStatus()))
                {
                    oCode = BUY_RETURN_CODE.STOPPED;
                    return (UInt16)(quanPurchased + tempQuanPurchased);
                }
                //Move from 'Cancel' to 'Buy'
                IocaineFunctions.arrowKeyDown(Keys.Up, Player_MenuNavigation.KeyDownTimeArrow);
                IocaineFunctions.delay(Statics.Settings.Helpers.PressEnterToSellDelay);
                //Hit 'Enter' to buy.
                IocaineFunctions.keyDown(Keys.Enter, Player_MenuNavigation.KeyDownTimeEnter);
                IocaineFunctions.delay(Statics.Settings.Top.MoveUpDownDelay);
                
                //Now parse the chat log for either of the phrases mentioned at the top of the function.
                bool quickRetry = false;
                ChatLine line = null;
                uint nbNewLines = 0;
                iChatLogger.Update(ref nbNewLines);
                while(iChatLogger.Read(out line))
                {
                    // Parse line into set of plain lines.
                    String chat = line.ReprocessStrings( ChatLine.CHAT_FLAGS.LEAVE_AUTOTRANSLATE);
                
                    FFXIEnums.CHAT_MODE code = line.Mode;
                    
                    if (chat.Contains(chatWaitLonger))
                    {
                        //We just need to wait a short period and try again.
                        quickRetry = true;
                        iChatLogger.Reset();
                        break;
                    }
                    else if (chat.Contains(chatUnable))
                    {
                        //We're done with this item since it's out of stock.
                        iChatLogger.Reset();
                        oCode = BUY_RETURN_CODE.NO_STOCK;
                        return quanPurchased;
                    }
                    else if (chat.Contains(chatClosed))
                    {
                        iChatLogger.Reset();
                        oCode = BUY_RETURN_CODE.CLOSED;
                        return quanPurchased;
                    }
                    else if (chat.Contains(chatFull))
                    {
                        iChatLogger.Reset();
                        oCode = BUY_RETURN_CODE.INV_FULL;
                        return quanPurchased;
                    }
                }
                if (quickRetry)
                {
                    IocaineFunctions.delay(500);
                    Statics.Settings.Helpers.GuildWaitTime_Buyer += 100;
                    continue;
                }
                UInt32 afterGil = MemReads.Self.Inventory.get_gil();
                if (afterGil == prevGil)
                {
                    tempQuanPurchased = 0;
                }
                quanPurchased += tempQuanPurchased;
                if (iGuild)
                {
                    IocaineFunctions.delay(Statics.Settings.Helpers.GuildWaitTime_Buyer);
                }
                else
                {
                    //Tune this later.
                    IocaineFunctions.delay(100);
                }
            }
            oCode = BUY_RETURN_CODE.NORMAL;
            return quanPurchased;
            #endregion Buy Item
        }
        #endregion Buy/Sell
    }
}
