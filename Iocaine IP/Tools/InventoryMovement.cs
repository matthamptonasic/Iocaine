using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Data.Structures;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Tools;

namespace Iocaine2.Inventory
{
    public class Movement
    {
        #region Private Members
        #endregion Private Members
        #region Public Properties
        #endregion Public Properties
        #region Public Methods
        #region Move Item
        public static List<ushort> MoveItem(List<Item> iItems, List<ushort> iQuanList, 
                                            ItemContainer iSource, ItemContainer iDest, 
                                            Statics.FuncPtrs.TD_Bool_Void iCheckStatus,
                                            bool iUseQuantities, Byte iSlots)
        {
            #region Error Checking
            String errMsg = "";
            for (int ii = 0; ii < iItems.Count; ii++)
            {
                if (!iSource.Contains(iItems[ii]))
                {
                    return new List<ushort>() { 0 };
                }
            }
            if (iDest.Occupancy == iDest.Capacity)
            {
                return new List<ushort>() { 0 };
            }

            // If the quantity passed is greater than the source container has available, set them equal.
            if (iUseQuantities)
            {
                for (int ii = 0; ii < iItems.Count; ii++)
                {
                    if (iQuanList[ii] > iSource.GetItemQuan(iItems[ii]))
                    {
                        iQuanList[ii] = iSource.GetItemQuan(iItems[ii]);
                    }
                }
            }

            // Check for invalid source <==> destination combinations.
            if ((iSource.StorageType != ItemContainer.STORAGE_TYPE.BAG) && (iDest.StorageType != ItemContainer.STORAGE_TYPE.BAG))
            {
                errMsg = "In MoveItem: Neither source nor destination were the gobbie bag:\n";
                errMsg += "Source = " + iSource.StorageType.ToString() + "\n";
                errMsg += "Destination = " + iDest.StorageType.ToString() + "\n";
                LoggingFunctions.Error(errMsg);
                MessageBox.Show(errMsg);
                return new List<ushort>() { 0 };
            }
            if (iSource == iDest)
            {
                errMsg = "In MoveItem: Source and Destination are the same:\n";
                errMsg += "Source = " + iSource.StorageType.ToString() + "\n";
                errMsg += "Destination = " + iDest.StorageType.ToString() + "\n";
                LoggingFunctions.Error(errMsg);
                MessageBox.Show(errMsg);
                return new List<ushort>() { 0 };
            }
            if ((iSource.StorageType == ItemContainer.STORAGE_TYPE.LOCKER) ||
                (iDest.StorageType == ItemContainer.STORAGE_TYPE.LOCKER) ||
                (iSource.StorageType == ItemContainer.STORAGE_TYPE.SAFE) ||
                (iDest.StorageType == ItemContainer.STORAGE_TYPE.SAFE) ||
                (iSource.StorageType == ItemContainer.STORAGE_TYPE.STORAGE) ||
                (iDest.StorageType == ItemContainer.STORAGE_TYPE.STORAGE))
            {
                errMsg = "In MoveItem: Moving in/out of MH containers isn't supported yet:\n";
                errMsg += "Source = " + iSource.StorageType.ToString() + "\n";
                errMsg += "Destination = " + iDest.StorageType.ToString() + "\n";
                LoggingFunctions.Error(errMsg);
                MessageBox.Show(errMsg);
                return new List<ushort>() { 0 };
            }

            bool moveToSack = iDest.StorageType == ItemContainer.STORAGE_TYPE.SACK;
            bool moveFromSack = iSource.StorageType == ItemContainer.STORAGE_TYPE.SACK;
            bool moveToSatchel = iDest.StorageType == ItemContainer.STORAGE_TYPE.SATCHEL;
            bool moveFromSatchel = iSource.StorageType == ItemContainer.STORAGE_TYPE.SATCHEL;
            bool moveToCase = iDest.StorageType == ItemContainer.STORAGE_TYPE.CASE;
            bool moveFromCase = iSource.StorageType == ItemContainer.STORAGE_TYPE.CASE;
            if (moveToSack || moveFromSack)
            {
                if (!MenuNavigation.OpenSack())
                {
                    errMsg = "Could not open Mog Sack.";
                    MessageBox.Show(errMsg);
                    errMsg = "In MoveItem: " + errMsg;
                    LoggingFunctions.Error(errMsg);
                    return new List<ushort>() { 0 };
                }
            }
            else if (moveToSatchel || moveFromSatchel)
            {
                if (!MenuNavigation.OpenSatchel())
                {
                    errMsg = "Could not open Satchel.";
                    MessageBox.Show(errMsg);
                    errMsg = "In MoveItem: " + errMsg;
                    LoggingFunctions.Error(errMsg);
                    return new List<ushort>() { 0 };
                }
            }
            else if (moveToCase || moveFromCase)
            {
                if (!MenuNavigation.OpenCase())
                {
                    errMsg = "Could not open Case.";
                    MessageBox.Show(errMsg);
                    errMsg = "In MoveItem: " + errMsg;
                    LoggingFunctions.Error(errMsg);
                    return new List<ushort>() { 0 };
                }
            }
            bool leftWndSel = MemReads.Windows.Items.get_left_wnd_selected();
            if (leftWndSel && (moveToSack || moveToSatchel || moveToCase))
            {
                //TBI Add retry if top left text isn't correct or we're still on the left window.
                IocaineFunctions.arrowKeyDown(Keys.Right, Statics.Settings.Top.KeyHoldTime);
            }
            else if (!leftWndSel && (moveFromSack || moveFromSatchel || moveFromCase))
            {
                //TBI Add retry if top left text isn't correct or we're still on the left window.
                IocaineFunctions.arrowKeyDown(Keys.Left, Statics.Settings.Top.KeyHoldTime);
            }
            #endregion Error Checking
            #region Set Up Lists and Menu
            //Now we begin finding the items in question and moving until we've moved enough OR our destination is full.
            
            iDest.RebuildLists();
            iSource.RebuildLists();

            bool destFull = false;
            List<ushort> quanRemainingList = new List<ushort>();
            List<ushort> quanMoved = new List<ushort>();
            List<ushort> destQuan = new List<ushort>();
            List<ushort> srceQuan = new List<ushort>();
            List<ushort> lastDestQuan = new List<ushort>();
            List<ushort> lastSrceQuan = new List<ushort>();
            Byte slotsRemaining = iSlots;
            Byte slotsMoved = 0;
            if (iUseQuantities)
            {
                for (int ii = 0; ii < iQuanList.Count; ii++)
                {
                    quanRemainingList.Add(iQuanList[ii]);
                    quanMoved.Add(0);
                    destQuan.Add(iDest.GetItemQuan(iItems[ii]));
                    srceQuan.Add(iSource.GetItemQuan(iItems[ii]));
                    lastDestQuan.Add(destQuan[ii]);
                    lastSrceQuan.Add(srceQuan[ii]);
                }
            }
            FFXIEnums.INVENTORY_MENU menu;
            if(moveFromSack)
            {
                menu = FFXIEnums.INVENTORY_MENU.SACK;
            }
            else if(moveFromSatchel)
            {
                menu = FFXIEnums.INVENTORY_MENU.SATCHEL;
            }
            else if (moveFromCase)
            {
                menu = FFXIEnums.INVENTORY_MENU.CASE;
            }
            else
            {
                menu = FFXIEnums.INVENTORY_MENU.BAG;
            }
            #endregion Set Up Lists and Menu
            #region Main Loop: Quantities
            if (iUseQuantities)
            {
                bool itemsRemaining = true;
                bool needToMoveMore = true;
                while (!destFull && itemsRemaining)
                {
                    itemsRemaining = false;
                    for (int kk = 0; kk < iItems.Count; kk++)
                    {
                        if (iSource.Contains(iItems[kk]))
                        {
                            itemsRemaining = true;
                        }
                    }
                    if (!itemsRemaining)
                    {
                        return quanMoved;
                    }

                    needToMoveMore = false;
                    for (int mm = 0; mm < quanRemainingList.Count; mm++)
                    {
                        if(quanRemainingList[mm] > 0)
                        {
                            needToMoveMore = true;
                        }
                    }
                    if(!needToMoveMore)
                    {
                        return quanMoved;
                    }

                    if ((iCheckStatus != null) && (!iCheckStatus()))
                    {
                        return quanMoved;
                    }
                    // Throw error if we can't find any of the listed items.
                    if (!SelectItem(iItems, menu, iCheckStatus))
                    {
                        if (iItems.Count == 1)
                        {
                            errMsg = "In MoveItem: Could not select item '" + iItems[0].Name + "'.";
                            errMsg += "\nReturning after moving " + quanMoved + " items.";
                        }
                        else
                        {
                            errMsg = "In MoveItem: Could not select any of these items '";
                            for (int mm = 0; mm < iItems.Count; mm++)
                            {
                                errMsg += iItems[mm].Name;
                                if (mm != (iItems.Count - 1))
                                {
                                    errMsg += ", ";
                                }
                            }
                            errMsg += "'\nReturning after moving " + quanMoved + " items.";
                        }
                        LoggingFunctions.Error(errMsg);
                        return quanMoved;
                    }
                    byte selItemQuan = MemReads.Windows.Items.get_selected_item_quan(menu);
                    IocaineFunctions.keyDown(Keys.Enter, Statics.Settings.Top.KeyHoldTime);
                    if (selItemQuan > 1)
                    {
                        IocaineFunctions.delay(100);
                        IocaineFunctions.arrowKeyDown(Keys.Left, Statics.Settings.Top.KeyHoldTime);
                        IocaineFunctions.delay(100);
                        IocaineFunctions.keyDown(Keys.Enter, Statics.Settings.Top.KeyHoldTime);
                    }
                    IocaineFunctions.delay(Statics.Settings.Top.MoveItemDelay);

                    iSource.RebuildLists();
                    iDest.RebuildLists();

                    List<ushort> nbAddedToDest = new List<ushort>();
                    List<ushort> nbMovedFromSource = new List<ushort>();
                    for (int kk = 0; kk < iQuanList.Count; kk++)
                    {
                        destQuan[kk] = iDest.GetItemQuan(iItems[kk]);
                        srceQuan[kk] = iSource.GetItemQuan(iItems[kk]);
                        nbAddedToDest.Add((ushort)(destQuan[kk] - lastDestQuan[kk]));
                        nbMovedFromSource.Add((ushort)(lastSrceQuan[kk] - srceQuan[kk]));

                        if (nbAddedToDest[kk] != nbMovedFromSource[kk])
                        {
                            errMsg = "In MoveItem: nbAddedToDest (" + nbAddedToDest[kk] + ") does not equal nbMovedFromSource (" + nbMovedFromSource[kk] + ")";
                            errMsg += "This may cause accounting errors.";
                            LoggingFunctions.Error(errMsg);
                        }

                        quanMoved[kk] += nbAddedToDest[kk];
                        if (quanRemainingList[kk] >= nbAddedToDest[kk])
                        {
                            quanRemainingList[kk] -= nbAddedToDest[kk];
                        }
                        else
                        {
                            quanRemainingList[kk] = 0;
                        }

                        lastDestQuan[kk] = destQuan[kk];
                        lastSrceQuan[kk] = srceQuan[kk];
                    }
                    destFull = iDest.Occupancy == iDest.Capacity;
                }
                return quanMoved;
            }
            #endregion Main Loop: Quantities
            #region Main Loop: Slots
            else
            {
                bool itemsRemaining = false;
                Byte lastSrcOcc = 0;
                Byte currSrcOcc = 0;
                while (!destFull && (slotsRemaining > 0))
                {
                    for (int kk = 0; kk < iItems.Count; kk++)
                    {
                        if (iSource.Contains(iItems[kk]))
                        {
                            itemsRemaining = true;
                        }
                    }
                    if (!itemsRemaining)
                    {
                        return new List<ushort>() { slotsMoved };
                    }
                    if ((iCheckStatus != null) && (!iCheckStatus()))
                    {
                        return new List<ushort>() { slotsMoved };
                    }

                    lastSrcOcc = iSource.Occupancy;
                    if (!SelectItem(iItems, menu, iCheckStatus))
                    {
                        if (iItems.Count == 1)
                        {
                            errMsg = "In MoveItem: Could not select item '" + iItems[0].Name + "'.";
                            errMsg += "\nReturning after moving " + quanMoved + " items.";
                        }
                        else
                        {
                            errMsg = "In MoveItem: Could not select any of these items: '";
                            for (int mm = 0; mm < iItems.Count; mm++)
                            {
                                errMsg += iItems[mm].Name;
                                if (mm != (iItems.Count - 1))
                                {
                                    errMsg += ", ";
                                }
                            }
                            errMsg += "'\nReturning after moving " + quanMoved + " items.";
                        }
                        LoggingFunctions.Error(errMsg);
                        return new List<ushort>() { slotsMoved };
                    }
                    byte selItemQuan = MemReads.Windows.Items.get_selected_item_quan(menu);
                    IocaineFunctions.keyDown(Keys.Enter, Statics.Settings.Top.KeyHoldTime);
                    if (selItemQuan > 1)
                    {
                        IocaineFunctions.delay(100);
                        IocaineFunctions.arrowKeyDown(Keys.Left, Statics.Settings.Top.KeyHoldTime);
                        IocaineFunctions.delay(100);
                        IocaineFunctions.keyDown(Keys.Enter, Statics.Settings.Top.KeyHoldTime);
                    }
                    IocaineFunctions.delay(Statics.Settings.Top.MoveItemDelay);

                    iSource.RebuildLists();
                    iDest.RebuildLists();

                    currSrcOcc = iSource.Occupancy;
                    if (currSrcOcc == lastSrcOcc)
                    {
                        errMsg = "In MoveItem: lastSrcOcc == currSrcOcc (" + lastSrcOcc + ")";
                        errMsg += "This may cause accounting errors.";
                        LoggingFunctions.Error(errMsg);
                    }
                    slotsMoved += (Byte)(lastSrcOcc - currSrcOcc);
                    slotsRemaining -= (Byte)(lastSrcOcc - currSrcOcc);

                    destFull = iDest.Occupancy == iDest.Capacity;
                }
                return new List<ushort>() { slotsMoved };
            }
            #endregion Main Loop: Slots
        }
        #region Overloads
        public static ushort MoveItem(Item iItem, ushort iQuan, ItemContainer iSource, ItemContainer iDest)
        {
            List<Item> itemList = new List<Item>() { iItem };
            List<ushort> quanList = new List<ushort>() { iQuan };
            return MoveItem(itemList, quanList, iSource, iDest, null, true, 0)[0];
        }
        public static ushort MoveItem(Item iItem, ushort iQuan, ItemContainer iSource, ItemContainer iDest, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            List<Item> itemList = new List<Item>() { iItem };
            List<ushort> quanList = new List<ushort>() { iQuan };
            return MoveItem(itemList, quanList, iSource, iDest, iCheckStatus, true, 0)[0];
        }
        public static ushort MoveItem(String iItemName, ushort iQuan, ItemContainer iSource, ItemContainer iDest, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            Item itm = new Item(iItemName, Things.GetIdFromName(iItemName), Item.ITEM_TYPE.UNKNOWN);
            List<Item> itemList = new List<Item>() { itm };
            List<ushort> quanList = new List<ushort>() { iQuan };
            return MoveItem(itemList, quanList, iSource, iDest, iCheckStatus, true, 0)[0];
        }
        public static ushort MoveItem(String iItemName, ushort iQuan, ItemContainer iSource, ItemContainer iDest)
        {
            return MoveItem(iItemName, iQuan, iSource, iDest, null);
        }
        public static ushort MoveItem(ushort iItemId, ushort iQuan, ItemContainer iSource, ItemContainer iDest, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            Item itm = new Item(Things.GetNameFromId(iItemId), iItemId, Item.ITEM_TYPE.UNKNOWN);
            List<Item> itemList = new List<Item>() { itm };
            List<ushort> quanList = new List<ushort>() { iQuan };
            return MoveItem(itemList, quanList, iSource, iDest, iCheckStatus, true, 0)[0];
        }
        public static ushort MoveItem(ushort iItemId, ushort iQuan, ItemContainer iSource, ItemContainer iDest)
        {
            return MoveItem(iItemId, iQuan, iSource, iDest, null);
        }
        public static Byte MoveItem(Item iItem, ushort iQuan, ItemContainer iSource, ItemContainer iDest, Statics.FuncPtrs.TD_Bool_Void iCheckStatus, bool iUseQuantities, Byte iSlots)
        {
            List<Item> itemList = new List<Item>() { iItem };
            List<ushort> quanList = new List<ushort>() { iQuan };
            return (Byte)(MoveItem(itemList, quanList, iSource, iDest, iCheckStatus, iUseQuantities, iSlots)[0]);
        }
        #endregion Overloads
        #endregion Move Item
        #region Select Item
        /// <summary>
        /// Select an item in inventory. Assumes that the inventory is already open.
        /// If there are dual inventory windows, it also assumes that the correct one is selected.
        /// </summary>
        /// <param name="iItemNames">Items to find.</param>
        /// <param name="iMinQuan">Minimum quantity of each item to find. That is, if the item is stackable, will look for a stack with at least this many in it.</param>
        /// <returns>True if found, false if not. Leaves the inventory window open.</returns>
        public static bool SelectItem(List<String> iItemNames, FFXIEnums.INVENTORY_MENU iMenu, ushort iMinQuan, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            bool secWndOpen = false;
            if (MenuNavigation.GetOpenInventoryMenu(out secWndOpen) == FFXIEnums.INVENTORY_MENU.NONE)
            {
                return false;
            }
            checkAutoSortOff();
            if ((iCheckStatus != null) && (!iCheckStatus()))
            {
                return false;
            }
            bool onLeftWnd = false;
            if (secWndOpen && MemReads.Windows.Items.get_left_wnd_selected())
            {
                onLeftWnd = true;
            }
            byte maxInvCnt = onLeftWnd ? MemReads.Windows.Items.get_sec_wnd_count() : MemReads.Windows.Items.get_count();
            byte position = onLeftWnd ? MemReads.Windows.Items.get_sec_wnd_selection_index() : MemReads.Windows.Items.get_selection_index();
            byte startPos = position;
            byte lastPos = startPos == 1 ? maxInvCnt : (byte)(startPos - 1);
            do
            {
                if ((iCheckStatus != null) && (!iCheckStatus()))
                {
                    return false;
                }
                checkAutoSortOff();
                position = onLeftWnd ? MemReads.Windows.Items.get_sec_wnd_selection_index() : MemReads.Windows.Items.get_selection_index();
                String selItem = MemReads.Windows.Items.get_selected_item_name();
                while (selItem == "N/A")
                {
                    IocaineFunctions.delay(100);
                    selItem = MemReads.Windows.Items.get_selected_item_name();
                }
                byte selIdx = MemReads.Windows.Items.get_selected_item_inventory_index();
                byte selQuan = MemReads.Windows.Items.get_selected_item_quan(iMenu);
                bool selEqpd = MemReads.Windows.Items.get_selected_item_is_equipped(iMenu);
                for (int ii = 0; ii < iItemNames.Count; ii++)
                {
                    if ((iItemNames[ii] == selItem) && (selQuan >= iMinQuan) && (!selEqpd))
                    {
                        return true;
                    }
                    else
                    {
                        if (position == maxInvCnt)
                        {
                            goToTopOfInv(secWndOpen);
                        }
                        else
                        {
                            IocaineFunctions.arrowKeyDown(Keys.Down);
                        }
                        IocaineFunctions.delay(Statics.Settings.Top.MoveUpDownDelay);
                        //position = onLeftWnd ? MemReads.info_inv_sec_wnd_location() : MemReads.info_inv_location();
                    }
                }
            }
            while (position != lastPos);
            return false;
        }
        #region Overloads
        public static bool SelectItem(String iItemName, FFXIEnums.INVENTORY_MENU iMenu, ushort iMinQuan)
        {
            List<String> itemNames = new List<string>() { iItemName };
            return SelectItem(itemNames, iMenu, iMinQuan, null);
        }
        public static bool SelectItem(Item iItem, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            return SelectItem(iItem.Name, iCheckStatus);
        }
        public static bool SelectItem(Item iItem)
        {
            return SelectItem(iItem.Name, null);
        }
        public static bool SelectItem(Item iItem, FFXIEnums.INVENTORY_MENU iMenu, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            return SelectItem(iItem.Name, iMenu, iCheckStatus);
        }
        public static bool SelectItem(List<Item> iItems, FFXIEnums.INVENTORY_MENU iMenu, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            List<String> itemNames = new List<string>();
            for (int ii = 0; ii < iItems.Count; ii++)
            {
                itemNames.Add(iItems[ii].Name);
            }
            return SelectItem(itemNames, iMenu, 0, iCheckStatus);
        }
        public static bool SelectItem(Item iItem, FFXIEnums.INVENTORY_MENU iMenu)
        {
            return SelectItem(iItem.Name, iMenu, null);
        }
        public static bool SelectItem(String iItemName, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            FFXIEnums.INVENTORY_MENU menu = MenuNavigation.GetOpenInventoryMenu();
            List<String> itemNames = new List<string>() { iItemName };
            return SelectItem(itemNames, menu, 0, iCheckStatus);
        }
        public static bool SelectItem(String iItemName)
        {
            FFXIEnums.INVENTORY_MENU menu = MenuNavigation.GetOpenInventoryMenu();
            List<String> itemNames = new List<string>() { iItemName };
            return SelectItem(itemNames, menu, 0, null);
        }
        public static bool SelectItem(String iItemName, FFXIEnums.INVENTORY_MENU iMenu, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            List<String> itemNames = new List<string>() { iItemName };
            return SelectItem(itemNames, iMenu, 0, iCheckStatus);
        }
        public static bool SelectItem(String iItemName, FFXIEnums.INVENTORY_MENU iMenu)
        {
            List<String> itemNames = new List<string>() { iItemName };
            return SelectItem(itemNames, iMenu, 0, null);
        }
        public static bool SelectItem(ushort iItemId, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            return SelectItem(Things.GetNameFromId(iItemId), iCheckStatus);
        }
        public static bool SelectItem(ushort iItemId)
        {
            return SelectItem(Things.GetNameFromId(iItemId), null);
        }
        public static bool SelectItem(ushort iItemId, FFXIEnums.INVENTORY_MENU iMenu, Statics.FuncPtrs.TD_Bool_Void iCheckStatus)
        {
            List<String> itemNames = new List<string>() { Things.GetNameFromId(iItemId) };
            return SelectItem(itemNames, iMenu, 0, iCheckStatus);
        }
        public static bool SelectItem(ushort iItemId, FFXIEnums.INVENTORY_MENU iMenu)
        {
            List<String> itemNames = new List<string>() { Things.GetNameFromId(iItemId) };
            return SelectItem(itemNames, iMenu, 0, null);
        }
        #endregion Overloads
        #endregion Select Item
        public static bool SortInventory(bool iExitOnDone)
        {
            Statics.FuncPtrs.SetStatusBoxPtr("Sorting inventory", Statics.Fields.Green);
            String topLeftText = "";
            byte cnt = 0;
            //Check what menu we're in.
            //If it's already the Item menu, just sort.
            //If it's the satchel or sack menu, go to the right menu and sort.
            //Otherwise, close check, open the item menu, and sort.
            FFXIEnums.INVENTORY_MENU openMenu = MenuNavigation.GetOpenInventoryMenu();
            if ((openMenu == FFXIEnums.INVENTORY_MENU.SACK) || (openMenu == FFXIEnums.INVENTORY_MENU.SATCHEL) || (openMenu == FFXIEnums.INVENTORY_MENU.CASE))
            {
                if (MemReads.Windows.Items.get_left_wnd_selected())
                {
                    IocaineFunctions.arrowKeyDown(Keys.Right);
                    IocaineFunctions.delay(400);
                }
                topLeftText = MemReads.Windows.BannerText.get_top_left_text();
                if (topLeftText != "Items")
                {
                    LoggingFunctions.Error("Could not sort inventory. Top left text was '" + topLeftText + "'.");
                    if (iExitOnDone)
                    {
                        MenuNavigation.CloseCheck(3);
                    }
                    return false;
                }
            }
            else if (openMenu != FFXIEnums.INVENTORY_MENU.BAG)
            {
                MenuNavigation.CloseCheck(3);
                IocaineFunctions.twoKeys(System.Windows.Forms.Keys.LControlKey, System.Windows.Forms.Keys.I, 250);
                IocaineFunctions.delay(250);
            }
            cnt = 0;
            topLeftText = MemReads.Windows.BannerText.get_top_left_text();
            while ((topLeftText != "Items") && (cnt < 10))
            {
                IocaineFunctions.delay(100);
                topLeftText = MemReads.Windows.BannerText.get_top_left_text();
                cnt++;
            }
            if (cnt == 10)
            {
                LoggingFunctions.Error("Could not sort inventory. Top left text was '" + topLeftText + "'.");
                if (iExitOnDone)
                {
                    MenuNavigation.CloseCheck(3);
                }
                return false;
            }
            IocaineFunctions.twoKeys(System.Windows.Forms.Keys.Alt, System.Windows.Forms.Keys.Add);
            IocaineFunctions.delay(200);
            if (MemReads.Windows.BannerText.get_help_text() == "Manual Sort")
            {
                IocaineFunctions.arrowKeyDown(System.Windows.Forms.Keys.Down);
                IocaineFunctions.delay(200);
            }
            IocaineFunctions.keyDown(System.Windows.Forms.Keys.Enter);
            IocaineFunctions.delay(200);
            IocaineFunctions.arrowKeyDown(System.Windows.Forms.Keys.Down);
            IocaineFunctions.delay(200);
            IocaineFunctions.keyDown(System.Windows.Forms.Keys.Enter);
            if (iExitOnDone)
            {
                MenuNavigation.CloseCheck(2);
            }
            return true;
        }
        #endregion Public Methods
        #region Private Methods
        private static void goToTopOfInv(bool secWndOpen)
        {
            bool secWndSelected = MemReads.Windows.Items.get_left_wnd_selected();
            if (secWndOpen && secWndSelected)
            {
                while (MemReads.Windows.Items.get_sec_wnd_selection_index() != 1)
                {
                    IocaineFunctions.arrowKeyDown(Keys.Up, 500);
                }
            }
            else if (secWndOpen && !secWndSelected)
            {
                while (MemReads.Windows.Items.get_selection_index() != 1)
                {
                    IocaineFunctions.arrowKeyDown(Keys.Up, 500);
                }
            }
            else
            {
                while (MemReads.Windows.Items.get_selection_index() != 1)
                {
                    IocaineFunctions.arrowKeyDown(Keys.Left, 500);
                }
            }
        }
        private static void checkAutoSortOff()
        {
            String topLeftText = MemReads.Windows.BannerText.get_top_left_text();
            String helpText = MemReads.Windows.BannerText.get_help_text();
            if ((helpText == "Select item to move.") || (helpText == "Sort items?"))
            {
                IocaineFunctions.keyDown(Keys.Escape, 200);
                IocaineFunctions.delay(250);
            }
            if ((topLeftText == "Auto-sort") || (topLeftText == "Manual Sort"))
            {
                IocaineFunctions.keyDown(Keys.Add, 200);
                IocaineFunctions.delay(250);
            }
        }
        #endregion Private Methods
    }
}
