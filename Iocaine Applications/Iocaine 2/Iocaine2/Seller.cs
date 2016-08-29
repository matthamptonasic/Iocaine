using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Inventory;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Tools;

namespace Iocaine2.Bots
{
    public static class Seller
    {
        #region Enums
        public enum STATE
        {
            STOPPED = 0,
            RUNNING = 1,
            PAUSED_USER = 2,
            PAUSED_PROG = 3
        }
        #endregion Enums

        #region Private Members
        private static STATE state = STATE.STOPPED;
        private static Thread runThread = null;
        private static Seller_Script script = null;
        private static Audio player = new Audio();
        private static String soundOnDone = "";
        private static byte nbSellingRetries = 3;
        //Master lists
        private static List<Item> itemRemainingList = new List<Item>();
        private static List<ushort> quanRemainingList = new List<ushort>();
        //Individual container lists
        private static List<Item> bagRemainingList = new List<Item>();
        private static List<ushort> bagQuanRemainingList = new List<ushort>();
        private static List<Item> sackRemainingList = new List<Item>();
        private static List<ushort> sackQuanRemainingList = new List<ushort>();
        private static List<Item> satchelRemainingList = new List<Item>();
        private static List<ushort> satchelQuanRemainingList = new List<ushort>();
        private static List<Item> caseRemainingList = new List<Item>();
        private static List<ushort> caseQuanRemainingList = new List<ushort>();
        private static ushort totalItemsSold = 0;
        private static UInt32 gilAtStart = 0;
        private static UInt32 gilAtEnd = 0;
        #endregion Private Members

        #region Public Members/Properties
        public static STATE State
        {
            get
            {
                return state;
            }
        }
        #endregion Public Members/Properties

        #region Private Methods
        #region Main Run Thread
        private static void runThreadFunction()
        {
            String errMsg = "";
            totalItemsSold = 0;
            try
            {
                gilAtStart = MemReads.Self.Inventory.get_gil();
                while (checkState() && checkStatus())
                {
                    //First check if we can move more items into our bag.
                    bool bagNotFull = Containers.Bag.LiveOccupancy < Containers.Bag.Capacity;
                    bool itemsLeftInSack = sackRemainingList.Count > 0;
                    bool itemsLeftInSatchel = satchelRemainingList.Count > 0;
                    bool itemsLeftInCase = caseRemainingList.Count > 0;
                    bool sellFromBagFirst = hasSellableItems(Containers.Bag) && Statics.Settings.Helpers.SellFromBagFirst;
                    if (bagNotFull && (itemsLeftInSack || itemsLeftInSatchel || itemsLeftInCase) && !sellFromBagFirst)
                    {
                        if (sackRemainingList.Count > 0)
                        {
                            //Move stuff over from the sack.
                            if (!moveItemsToBag(ItemContainer.STORAGE_TYPE.SACK))
                            {
                                errMsg = "Could not move needed items from Mog Sack.";
                                LoggingFunctions.Error(errMsg);
                                MessageBox.Show(errMsg);
                                break;
                            }
                        }
                        else if (caseRemainingList.Count > 0)
                        {
                            //Move stuff over from the mog case.
                            if (!moveItemsToBag(ItemContainer.STORAGE_TYPE.CASE))
                            {
                                errMsg = "Could not move needed items from Mog Case.";
                                LoggingFunctions.Error(errMsg);
                                MessageBox.Show(errMsg);
                                break;
                            }
                        }
                        else if (satchelRemainingList.Count > 0)
                        {
                            //Move stuff over from the satchel.
                            if (!moveItemsToBag(ItemContainer.STORAGE_TYPE.SATCHEL))
                            {
                                errMsg = "Could not move needed items from Satchel.";
                                LoggingFunctions.Error(errMsg);
                                MessageBox.Show(errMsg);
                                break;
                            }
                        }
                    }
                    else
                    {
                        //Our bag is full, so try to sell some crap.
                        //Make sure there's actually something in our bag that we can sell.
                        if (hasSellableItems(Containers.Bag))
                        {
                            //Implement a retry system in case something goes wrong once or twice.
                            byte cnt = 0;
                            while ((sellBagItems() == false) && (cnt < nbSellingRetries))
                            {
                                if (!checkState())
                                {
                                    break;
                                }
                                Statics.FuncPtrs.SetStatusBoxPtr("Could not sell some items, trying again.", Statics.Fields.Red);
                                IocaineFunctions.delay(2000);
                                Player_MenuNavigation.CloseCheck();
                                cnt++;
                            }
                            if (cnt == nbSellingRetries)
                            {
                                Stop();
                                continue;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            //Our bag is full but has no items to sell.
                            //We're not going to support moving items back into other containers, so just stop.
                            break;
                        }
                    }

                    IocaineFunctions.delay(1000);
                    Inventory.Containers.RebuildListsMobileOnly();
                }
                Player_MenuNavigation.CloseCheck();
                if (soundOnDone != "")
                {
                    player.PlaySound(soundOnDone);
                }
                Stop();
                gilAtEnd = MemReads.Self.Inventory.get_gil();
                String statusString = "Selling complete. Sold " + totalItemsSold + " item";
                if (totalItemsSold != 1)
                {
                    statusString += "s";
                }
                statusString += " for " + String.Format("{0:0,0}", (int)(gilAtEnd - gilAtStart)) + " gil.";
                Statics.FuncPtrs.SetStatusBoxPtr(statusString, Statics.Fields.Blue);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller::runThreadFunction: " + e.ToString());
            }
        }
        #endregion Main Run Thread
        #region Status
        private static bool checkState()
        {
            try
            {
                switch (state)
                {
                    case STATE.RUNNING:
                        return true;
                    case STATE.STOPPED:
                        return false;
                    case STATE.PAUSED_USER:
                        while (state == STATE.PAUSED_USER)
                        {
                            IocaineFunctions.delay(500);
                        }
                        if (state == STATE.RUNNING)
                        {
                            return true;
                        }
                        if (state == STATE.STOPPED)
                        {
                            return false;
                        }
                        break;
                    case STATE.PAUSED_PROG:
                        while (state == STATE.PAUSED_USER)
                        {
                            IocaineFunctions.delay(500);
                        }
                        if (state == STATE.RUNNING)
                        {
                            return true;
                        }
                        if (state == STATE.STOPPED)
                        {
                            return false;
                        }
                        break;
                }
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller::checkState: " + e.ToString());
                return false;
            }
        }
        private static bool checkStatus()
        {
            try
            {
                if (itemRemainingList.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller::checkStatus: " + e.ToString());
                return false;
            }
        }
        #endregion Status
        #region Quantity Tracking
        private static void setActualQuantities()
        {
            try
            {
                for (int ii = 0; ii < quanRemainingList.Count; ii++)
                {
                    if (quanRemainingList[ii] == 0)
                    {
                        quanRemainingList[ii] = Containers.Bag.GetItemQuan(itemRemainingList[ii]);
                        if (script.SellFromSack)
                        {
                            quanRemainingList[ii] += Containers.Sack.GetItemQuan(itemRemainingList[ii]);
                        }
                        if (script.SellFromSatchel)
                        {
                            quanRemainingList[ii] += Containers.Satchel.GetItemQuan(itemRemainingList[ii]);
                        }
                        if (script.SellFromCase)
                        {
                            quanRemainingList[ii] += Containers.MCase.GetItemQuan(itemRemainingList[ii]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller::setActualQuantities: " + e.ToString());
            }
        }
        private static void setLists()
        {
            try
            {
                //This function sets up a list of items and quantities that we need to sell from each container individually.
                itemRemainingList = script.ItemList;
                quanRemainingList = script.ItemQuanList;
                bagRemainingList.Clear();
                bagQuanRemainingList.Clear();
                sackRemainingList.Clear();
                sackQuanRemainingList.Clear();
                satchelRemainingList.Clear();
                satchelQuanRemainingList.Clear();
                caseRemainingList.Clear();
                caseQuanRemainingList.Clear();
                //If an item quantity is set to 0 it means 'sell all', so fix this now to be the actual quantity of what we have.
                setActualQuantities();
                //Go through each item in the list.
                //If the item is set to 'sell all' and the script is set to 'sell from sack' and/or 'sell from satchel'
                //simply add all of each item in each container to the containers' list.
                int nbItems = itemRemainingList.Count;
                for (int ii = 0; ii < nbItems; ii++)
                {
                    //Set the bag list first.
                    ushort nbOfThisItem = Containers.Bag.GetItemQuan(itemRemainingList[ii]);
                    if (nbOfThisItem > 0)
                    {
                        //Our bag contains this item.
                        bagRemainingList.Add(itemRemainingList[ii]);
                        if (quanRemainingList[ii] == 0)
                        {
                            //Sell all of them. Add the total number in the bag to the list.
                            bagQuanRemainingList.Add(nbOfThisItem);
                        }
                        else if (quanRemainingList[ii] > nbOfThisItem)
                        {
                            //We're trying to sell more than what are in the bag, so set however many are in our bag.
                            bagQuanRemainingList.Add(nbOfThisItem);
                            quanRemainingList[ii] -= nbOfThisItem;
                        }
                        else
                        {
                            //We're trying to sell less than or as many as what we have, so just set the number.
                            bagQuanRemainingList.Add(quanRemainingList[ii]);
                            quanRemainingList[ii] = 0;
                            continue;
                        }
                    }
                    //Set the sack list.
                    nbOfThisItem = Containers.Sack.GetItemQuan(itemRemainingList[ii]);
                    if ((nbOfThisItem > 0) && (script.SellFromSack == true))
                    {
                        sackRemainingList.Add(itemRemainingList[ii]);
                        if (quanRemainingList[ii] == 0)
                        {
                            //Sell all of them. Add the total number in the sack to the list.
                            sackQuanRemainingList.Add(nbOfThisItem);
                        }
                        else if (quanRemainingList[ii] > nbOfThisItem)
                        {
                            //We're trying to sell more than what are in the sack, so set however many are in our sack.
                            sackQuanRemainingList.Add(nbOfThisItem);
                            quanRemainingList[ii] -= nbOfThisItem;
                        }
                        else
                        {
                            //We're trying to sell less than or as many as what we have, so just set the number.
                            sackQuanRemainingList.Add(quanRemainingList[ii]);
                            quanRemainingList[ii] = 0;
                            continue;
                        }
                    }
                    //Set the satchel list.
                    nbOfThisItem = Containers.Satchel.GetItemQuan(itemRemainingList[ii]);
                    if ((nbOfThisItem > 0) && (script.SellFromSatchel == true))
                    {
                        satchelRemainingList.Add(itemRemainingList[ii]);
                        if (quanRemainingList[ii] == 0)
                        {
                            //Sell all of them. Add the total number in the sack to the list.
                            satchelQuanRemainingList.Add(nbOfThisItem);
                        }
                        else if (quanRemainingList[ii] > nbOfThisItem)
                        {
                            //We're trying to sell more than what are in the sack, so set however many are in our sack.
                            satchelQuanRemainingList.Add(nbOfThisItem);
                            quanRemainingList[ii] -= nbOfThisItem;
                        }
                        else
                        {
                            //We're trying to sell less than or as many as what we have, so just set the number.
                            satchelQuanRemainingList.Add(quanRemainingList[ii]);
                            quanRemainingList[ii] = 0;
                            continue;
                        }
                    }
                    //Set the case list.
                    nbOfThisItem = Containers.MCase.GetItemQuan(itemRemainingList[ii]);
                    if ((nbOfThisItem > 0) && (script.SellFromCase == true))
                    {
                        caseRemainingList.Add(itemRemainingList[ii]);
                        if (quanRemainingList[ii] == 0)
                        {
                            //Sell all of them. Add the total number in the sack to the list.
                            caseQuanRemainingList.Add(nbOfThisItem);
                        }
                        else if (quanRemainingList[ii] > nbOfThisItem)
                        {
                            //We're trying to sell more than what are in the sack, so set however many are in our sack.
                            caseQuanRemainingList.Add(nbOfThisItem);
                            quanRemainingList[ii] -= nbOfThisItem;
                        }
                        else
                        {
                            //We're trying to sell less than or as many as what we have, so just set the number.
                            caseQuanRemainingList.Add(quanRemainingList[ii]);
                            quanRemainingList[ii] = 0;
                            continue;
                        }
                    }
                }
                //Since we modified this along the way, reset it now.
                quanRemainingList = script.ItemQuanList;
                setActualQuantities();
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller::setLists: " + e.ToString());
            }
        }
        private static void reduceItem(ItemContainer.STORAGE_TYPE iContainer, Item iItem, ushort iQuanReduced)
        {
            try
            {
                List<Item> localList;
                List<ushort> localQuanList;
                switch (iContainer)
                {
                    case ItemContainer.STORAGE_TYPE.BAG:
                        localList = bagRemainingList;
                        localQuanList = bagQuanRemainingList;
                        break;
                    case ItemContainer.STORAGE_TYPE.SACK:
                        localList = sackRemainingList;
                        localQuanList = sackQuanRemainingList;
                        break;
                    case ItemContainer.STORAGE_TYPE.CASE:
                        localList = caseRemainingList;
                        localQuanList = caseQuanRemainingList;
                        break;
                    default:
                        localList = satchelRemainingList;
                        localQuanList = satchelQuanRemainingList;
                        break;
                }
                for (int ii = 0; ii < localList.Count; ii++)
                {
                    if (localList[ii].ItemID == iItem.ItemID)
                    {
                        if (localQuanList[ii] <= iQuanReduced)
                        {
                            localQuanList.RemoveAt(ii);
                            localList.RemoveAt(ii);
                        }
                        else
                        {
                            localQuanList[ii] -= iQuanReduced;
                        }
                        break;
                    }
                }
                for (int ii = 0; ii < itemRemainingList.Count; ii++)
                {
                    if (itemRemainingList[ii].ItemID == iItem.ItemID)
                    {
                        if (quanRemainingList[ii] <= iQuanReduced)
                        {
                            quanRemainingList.RemoveAt(ii);
                            itemRemainingList.RemoveAt(ii);
                        }
                        else
                        {
                            quanRemainingList[ii] -= iQuanReduced;
                        }
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller::reduceItem: " + e.ToString());
            }
        }
        private static void moveItem(ItemContainer.STORAGE_TYPE iSrc, ItemContainer.STORAGE_TYPE iDst, Item iItem, ushort iQuanMoved)
        {
            try
            {
                List<Item> srcItems;
                List<ushort> srcQuan;
                List<Item> dstItems;
                List<ushort> dstQuan;
                switch (iSrc)
                {
                    case ItemContainer.STORAGE_TYPE.BAG:
                        srcItems = bagRemainingList;
                        srcQuan = bagQuanRemainingList;
                        break;
                    case ItemContainer.STORAGE_TYPE.SACK:
                        srcItems = sackRemainingList;
                        srcQuan = sackQuanRemainingList;
                        break;
                    case ItemContainer.STORAGE_TYPE.CASE:
                        srcItems = caseRemainingList;
                        srcQuan = caseQuanRemainingList;
                        break;
                    default:
                        srcItems = satchelRemainingList;
                        srcQuan = satchelQuanRemainingList;
                        break;
                }
                switch (iDst)
                {
                    case ItemContainer.STORAGE_TYPE.BAG:
                        dstItems = bagRemainingList;
                        dstQuan = bagQuanRemainingList;
                        break;
                    case ItemContainer.STORAGE_TYPE.SACK:
                        dstItems = sackRemainingList;
                        dstQuan = sackQuanRemainingList;
                        break;
                    case ItemContainer.STORAGE_TYPE.CASE:
                        dstItems = caseRemainingList;
                        dstQuan = caseQuanRemainingList;
                        break;
                    default:
                        dstItems = satchelRemainingList;
                        dstQuan = satchelQuanRemainingList;
                        break;
                }
                for (int ii = 0; ii < srcItems.Count; ii++)
                {
                    if (srcItems[ii].ItemID == iItem.ItemID)
                    {
                        if (srcQuan[ii] <= iQuanMoved)
                        {
                            srcItems.RemoveAt(ii);
                            srcQuan.RemoveAt(ii);
                        }
                        else
                        {
                            srcQuan[ii] -= iQuanMoved;
                        }
                        break;
                    }
                }
                bool foundItem = false;
                for (int ii = 0; ii < dstItems.Count; ii++)
                {
                    if (dstItems[ii].ItemID == iItem.ItemID)
                    {
                        dstQuan[ii] += iQuanMoved;
                        foundItem = true;
                        break;
                    }
                }
                if (!foundItem)
                {
                    dstItems.Add(iItem);
                    dstQuan.Add(iQuanMoved);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller::moveItem: " + e.ToString());
            }
        }
        #endregion Quantity Tracking
        #region General
        private static bool hasSellableItems(ItemContainer iContainer)
        {
            try
            {
                for (int ii = 0; ii < itemRemainingList.Count; ii++)
                {
                    if (iContainer.Contains(itemRemainingList[ii]))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller::checkState: " + e.ToString());
                return false;
            }
        }
        private static bool sellBagItems()
        {
            try
            {
                //We're just going to get in the selling dialog and go through each item and see if we need to sell it or not.
                //First check if we're already in a shop or guild shop dialog.
                String errMsg = "";
                if (Statics.Settings.Helpers.SortBeforeSelling)
                {
                    Movement.SortInventory(false);
                }
                if (!Interaction.GetToBuySellDialog(false, false, script.NpcName, new Statics.FuncPtrs.TD_Bool_Void(checkState)))
                {
                    return false;
                }
                //We should now be in the sell dialog at the top of the list.
                byte itemIdx = MemReads.Windows.Items.get_selected_item_inventory_index();
                byte lastItemIdx = 0;
                byte twoItemsAgo = 0;
                bool atBottom = false;
                bool firstItemBeingSold = true;
                while (!atBottom && checkState() && checkStatus())
                {
                    Statics.FuncPtrs.SetStatusBoxPtr("Searching for sellable items.", Statics.Fields.Green);
                    if (firstItemBeingSold)
                    {
                        IocaineFunctions.delay(Statics.Settings.Top.MoveUpDownDelay);
                    }
                    String selItem = MemReads.Windows.Items.get_selected_item_name();
                    byte errCnt = 0;
                    while ((selItem == "N/A") && (errCnt < 10))
                    {
                        //Often glitches.
                        IocaineFunctions.delay(50);
                        selItem = MemReads.Windows.Items.get_selected_item_name();
                        errCnt++;
                    }
                    if (errCnt == 10)
                    {
                        errMsg = "In sellBagItems: Could not read the item name from memory, keeps coming back empty.";
                        LoggingFunctions.Error(errMsg);
                        MessageBox.Show(errMsg);
                        return false;
                    }
                    //Now we have the proper item name set. Check if that item is in our list.
                    bool sellThisItem = false;
                    int idx = 0;
                    for (int ii = 0; ii < bagRemainingList.Count; ii++)
                    {
                        if (selItem == bagRemainingList[ii].Name)
                        {
                            sellThisItem = true;
                            idx = ii;
                            break;
                        }
                    }
                    if (sellThisItem && (!MemReads.Windows.Items.get_selected_item_is_equipped(FFXIEnums.INVENTORY_MENU.BAG)))
                    {
                        Statics.FuncPtrs.SetStatusBoxPtr("Selling item.", Statics.Fields.Green);
                        //Get the selected item quantity and if it's more than what we need to sell, pass the lower amount.
                        //byte itemQuan = MemReads.info_win_item_select_quan(FFXIEnums.INVENTORY_MENU.BAG);
                        byte itemQuan = 99;
                        byte actualSold = 0;
                        if (itemQuan > bagQuanRemainingList[idx])
                        {
                            itemQuan = (byte)bagQuanRemainingList[idx];
                        }
                        if (!Interaction.SellSelectedItem(bagRemainingList[idx].Name, itemQuan, firstItemBeingSold, ref actualSold, new Statics.FuncPtrs.TD_Bool_Void(checkState)))
                        {
                            LoggingFunctions.Warning("Discrepancy when selling item '" + selItem + ". Going on to next item.");
                        }
                        else
                        {
                            //Update the bag and master lists.
                            reduceItem(ItemContainer.STORAGE_TYPE.BAG, bagRemainingList[idx], actualSold);
                            totalItemsSold += actualSold;
                            firstItemBeingSold = false;
                        }
                    }
                    else
                    {
                        //This item was not for sale. Move down one and continue.
                        IocaineFunctions.arrowKeyDown(Keys.Down, Player_MenuNavigation.KeyDownTimeArrow);
                        IocaineFunctions.delay(Statics.Settings.Top.MoveUpDownDelay);
                    }
                    twoItemsAgo = lastItemIdx;
                    lastItemIdx = itemIdx;
                    itemIdx = MemReads.Windows.Items.get_selected_item_inventory_index();
                    if (twoItemsAgo == itemIdx)
                    {
                        atBottom = true;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller::sellBagItems: " + e.ToString());
                return false;
            }
        }
        private static bool moveItemsToBag(ItemContainer.STORAGE_TYPE iSrc)
        {
            try
            {
                String statusString = "Moving items from your ";
                List<Item> srcItems;
                List<ushort> srcQuan;
                ItemContainer srcContainer;
                if (iSrc == ItemContainer.STORAGE_TYPE.SACK)
                {
                    srcItems = sackRemainingList;
                    srcQuan = sackQuanRemainingList;
                    srcContainer = Containers.Sack;
                    statusString += "Mog Sack.";
                }
                else if (iSrc == ItemContainer.STORAGE_TYPE.CASE)
                {
                    srcItems = caseRemainingList;
                    srcQuan = caseQuanRemainingList;
                    srcContainer = Containers.MCase;
                    statusString += "Mog Case.";
                }
                else
                {
                    srcItems = satchelRemainingList;
                    srcQuan = satchelQuanRemainingList;
                    srcContainer = Containers.Satchel;
                    statusString += "Satchel.";
                }
                Statics.FuncPtrs.SetStatusBoxPtr(statusString, Statics.Fields.Green);
                ushort nbMoved = 0;
                for (int ii = 0; ii < srcItems.Count; ii++)
                {
                    srcContainer.RebuildLists();
                    Containers.Bag.RebuildLists();
                    nbMoved = Movement.MoveItem(srcItems[ii], srcQuan[ii], srcContainer, Containers.Bag, checkState);
                    moveItem(iSrc, ItemContainer.STORAGE_TYPE.BAG, srcItems[ii], nbMoved);
                    if (Containers.Bag.LiveOccupancy == Containers.Bag.Capacity)
                    {
                        break;
                    }
                    if (!checkState())
                    {
                        break;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller::moveItemsToBag: " + e.ToString());
                return false;
            }
        }
        #endregion General
        #endregion Private Methods

        #region Public Methods
        #region Status
        public static void Start(Seller_Script iScript, String iAudioOnDone)
        {
            try
            {
                if ((state != STATE.STOPPED) || (iScript == null))
                {
                    return;
                }
                Statics.FuncPtrs.SetSellerButtonPtr("&Pause", Statics.Buttons.Yellow);
                Statics.FuncPtrs.SetStatusBoxPtr("Starting the Seller.", Statics.Fields.Green);
                state = STATE.RUNNING;
                script = new Seller_Script(iScript);
                soundOnDone = iAudioOnDone;
                Inventory.Containers.RebuildListsMobileOnly();
                setLists();
                runThread = new Thread(new ThreadStart(runThreadFunction));
                runThread.Name = "runThread";
                runThread.IsBackground = true;
                runThread.Start();
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller::Start(Seller_Script, String): " + e.ToString());
            }
        }
        public static void Start(Seller_Script iScript)
        {
            Start(iScript, "");
            Statics.FuncPtrs.SetSellerButtonPtr("&Pause", Statics.Buttons.Yellow);
        }
        public static void Stop()
        {
            state = STATE.STOPPED;
            Statics.FuncPtrs.SetSellerButtonPtr("S&tart", Statics.Buttons.Green);
        }
        public static void Pause()
        {
            state = STATE.PAUSED_USER;
            Statics.FuncPtrs.SetSellerButtonPtr("&Resume", Statics.Buttons.Green);
        }
        public static void Resume()
        {
            state = STATE.RUNNING;
            Statics.FuncPtrs.SetSellerButtonPtr("&Pause", Statics.Buttons.Yellow);
        }
        #endregion Status
        #endregion Public Methods
    }
}
