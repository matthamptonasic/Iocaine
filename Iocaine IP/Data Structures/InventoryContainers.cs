using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Inventory
{
    public static class Containers
    {
        #region Enums
        public enum STORAGE_INDEX : byte
        {
            BAG = 0,
            SATCHEL = 1,
            SACK = 2,
            CASE = 3,
            SAFE = 4,
            STORAGE = 5,
            LOCKER = 6,
            WARDROBE = 7
        }
        #endregion Enums
        #region Private Members
        private static ItemContainer bag;
        private static ItemContainer satchel;
        private static ItemContainer sack;
        private static ItemContainer mcase;
        private static ItemContainer safe;
        private static ItemContainer storage;
        private static ItemContainer locker;
        private static ItemContainer wardrobe;
        private static List<ItemContainer> containerList = new List<ItemContainer>() { bag, satchel, sack, mcase, safe, storage, locker, wardrobe };
        private static List<ItemContainer> containerListMobile = new List<ItemContainer>() { bag, satchel, sack, mcase, wardrobe };
        private static short absMaxBagCount = 80;
        private static short absMaxSatchelCount = absMaxBagCount;
        private static short absMaxSackCount = absMaxBagCount;
        private static short absMaxCaseCount = absMaxBagCount;
        private static short absMaxSafeCount = 80;
        private static short absMaxStorageCount = 80;
        private static short absMaxLockerCount = 80;
        private static short absMaxWardrobeCount = absMaxBagCount;
        private static List<short> absMaxCountList = new List<short>() { absMaxBagCount, absMaxSatchelCount, absMaxSackCount, absMaxCaseCount, absMaxSafeCount, absMaxStorageCount, absMaxLockerCount, absMaxWardrobeCount };
        private static List<Item> summaryItemList;
        private static List<ushort> summaryItemListQuan;
        private static List<Item> summaryItemListMobile;
        private static List<ushort> summaryItemListQuanMobile;
        private static List<Item> summaryItemListHouse;
        private static List<ushort> summaryItemListQuanHouse;
        private static AutoCompleteStringCollection itemAutoCompleteList;
        private static bool initDone = false;
        #endregion Private Members
        #region Public Members/Properties
        #region Containers
        public static ItemContainer Bag
        {
            get
            {
                return bag;
            }
        }
        public static ItemContainer Satchel
        {
            get
            {
                return satchel;
            }
        }
        public static ItemContainer Sack
        {
            get
            {
                return sack;
            }
        }
        public static ItemContainer MCase
        {
            get
            {
                return mcase;
            }
        }
        public static ItemContainer Safe
        {
            get
            {
                return safe;
            }
        }
        public static ItemContainer Storage
        {
            get
            {
                return storage;
            }
        }
        public static ItemContainer Locker
        {
            get
            {
                return locker;
            }
        }
        public static ItemContainer Wardrobe
        {
            get
            {
                return wardrobe;
            }
        }
        #endregion Containers
        #region Absolue Max Counts
        public static short AbsMaxBagCount
        {
            get
            {
                return absMaxBagCount;
            }
        }
        public static short AbsMaxSatchelCount
        {
            get
            {
                return absMaxSatchelCount;
            }
        }
        public static short AbsMaxSackCount
        {
            get
            {
                return absMaxSackCount;
            }
        }
        public static short AbsMaxCaseCount
        {
            get
            {
                return absMaxCaseCount;
            }
        }
        public static short AbsMaxSafeCount
        {
            get
            {
                return absMaxSafeCount;
            }
        }
        public static short AbsMaxStorageCount
        {
            get
            {
                return absMaxStorageCount;
            }
        }
        public static short AbsMaxLockerCount
        {
            get
            {
                return absMaxLockerCount;
            }
        }
        public static short AbsMaxWardrobeCount
        {
            get
            {
                return absMaxWardrobeCount;
            }
        }
        #endregion Absolue Max Counts
        #region List Methods
        public static List<Item> SummaryItemList
        {
            get
            {
                return summaryItemList;
            }
        }
        public static List<ushort> SummaryItemQuanList
        {
            get
            {
                return summaryItemListQuan;
            }
        }
        public static List<Item> SummaryItemListMobile
        {
            get
            {
                return summaryItemListMobile;
            }
        }
        public static List<ushort> SummaryItemQuanListMobile
        {
            get
            {
                return summaryItemListQuanMobile;
            }
        }
        public static List<Item> SummaryItemListHouse
        {
            get
            {
                return summaryItemListHouse;
            }
        }
        public static List<ushort> SummaryItemQuanListHouse
        {
            get
            {
                return summaryItemListQuanHouse;
            }
        }
        #endregion List Methods
        #region Auto Complete List
        public static AutoCompleteStringCollection ItemAutoCompleteList
        {
            get
            {
                if (ChangeMonitor.LoggedIn)
                {
                    RebuildLists();
                    rebuildItemACList();
                    return itemAutoCompleteList;
                }
                else
                {
                    return new AutoCompleteStringCollection();
                }
            }
            set
            {
                itemAutoCompleteList = value;
            }
        }
        #endregion Auto Complete List
        #region Delegates/Events
        public delegate void MaxCapacityValuesUpdated();
        public static event MaxCapacityValuesUpdated _MaxCapacityValuesUpdated;
        public delegate void CurrentCapcityValuesUpdated();
        public static event CurrentCapcityValuesUpdated _CurrentCapacityValuesUpdated;
        #endregion Delegates/Events
        #endregion Public Members/Properties
        #region Private Methods
        #region Auto Complete List
        private static void rebuildItemACList()
        {
            try
            {
                Monitor.Enter(itemAutoCompleteList);
                Monitor.Enter(summaryItemList);
                itemAutoCompleteList.Clear();
                for (int ii = 0; ii < summaryItemList.Count; ii++)
                {
                    itemAutoCompleteList.Add(summaryItemList[ii].Name);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In rebuildItemACList: " + e.ToString());
            }
            finally
            {
                Monitor.Exit(summaryItemList);
                Monitor.Exit(itemAutoCompleteList);
            }
        }
        #endregion Auto Complete List
        #region Index Of
        private static int indexOf(Item iItem)
        {
            foreach (Item item in summaryItemList)
            {
                if (item.ItemID == iItem.ItemID)
                {
                    return summaryItemList.IndexOf(item);
                }
            }
            return -1;
        }
        private static int indexOfHouse(Item iItem)
        {
            foreach (Item item in summaryItemListHouse)
            {
                if (item.ItemID == iItem.ItemID)
                {
                    return summaryItemListHouse.IndexOf(item);
                }
            }
            return -1;
        }
        private static int indexOfMobile(Item iItem)
        {
            foreach (Item item in summaryItemListMobile)
            {
                if (item.ItemID == iItem.ItemID)
                {
                    return summaryItemListMobile.IndexOf(item);
                }
            }
            return -1;
        }
        #endregion Index Of
        #endregion Private Methods
        #region Public Methods
        #region Init
        public static void Init()
        {
            bag = new ItemContainer(ItemContainer.STORAGE_TYPE.BAG);
            satchel = new ItemContainer(ItemContainer.STORAGE_TYPE.SATCHEL);
            sack = new ItemContainer(ItemContainer.STORAGE_TYPE.SACK);
            mcase = new ItemContainer(ItemContainer.STORAGE_TYPE.CASE);
            safe = new ItemContainer(ItemContainer.STORAGE_TYPE.SAFE);
            storage = new ItemContainer(ItemContainer.STORAGE_TYPE.STORAGE);
            locker = new ItemContainer(ItemContainer.STORAGE_TYPE.LOCKER);
            wardrobe = new ItemContainer(ItemContainer.STORAGE_TYPE.WARDROBE);
            summaryItemList = new List<Item>();
            summaryItemListHouse = new List<Item>();
            summaryItemListMobile = new List<Item>();
            summaryItemListQuan = new List<ushort>();
            summaryItemListQuanHouse = new List<ushort>();
            summaryItemListQuanMobile = new List<ushort>();
            itemAutoCompleteList = new AutoCompleteStringCollection();
            if (_MaxCapacityValuesUpdated != null)
            {
                _MaxCapacityValuesUpdated();
            }
            initDone = true;
        }
        #endregion Init
        #region Contains
        #region All
        public static bool Contains(String iItemName)
        {
            Monitor.Enter(summaryItemList);
            foreach (Item item in summaryItemList)
            {
                if (item.Name == iItemName)
                {
                    Monitor.Exit(summaryItemList);
                    return true;
                }
            }
            Monitor.Exit(summaryItemList);
            return false;
        }
        public static bool Contains(short iItemID)
        {
            Monitor.Enter(summaryItemList);
            foreach (Item item in summaryItemList)
            {
                if (item.ItemID == iItemID)
                {
                    Monitor.Exit(summaryItemList);
                    return true;
                }
            }
            Monitor.Exit(summaryItemList);
            return false;
        }
        public static bool Contains(Item iItem)
        {
            Monitor.Enter(summaryItemList);
            foreach (Item localItem in summaryItemList)
            {
                if ((iItem.ItemID == localItem.ItemID) && (iItem.Name == localItem.Name))
                {
                    Monitor.Exit(summaryItemList);
                    return true;
                }
            }
            Monitor.Exit(summaryItemList);
            return false;
        }
        #endregion All
        #region House
        public static bool ContainsHouse(String iItemName)
        {
            Monitor.Enter(summaryItemListHouse);
            foreach (Item item in summaryItemListHouse)
            {
                if (item.Name == iItemName)
                {
                    Monitor.Exit(summaryItemListHouse);
                    return true;
                }
            }
            Monitor.Exit(summaryItemListHouse);
            return false;
        }
        public static bool ContainsHouse(short iItemID)
        {
            Monitor.Enter(summaryItemListHouse);
            foreach (Item item in summaryItemListHouse)
            {
                if (item.ItemID == iItemID)
                {
                    Monitor.Exit(summaryItemListHouse);
                    return true;
                }
            }
            Monitor.Exit(summaryItemListHouse);
            return false;
        }
        public static bool ContainsHouse(Item iItem)
        {
            Monitor.Enter(summaryItemListHouse);
            foreach (Item localItem in summaryItemListHouse)
            {
                if ((iItem.ItemID == localItem.ItemID) && (iItem.Name == localItem.Name))
                {
                    Monitor.Exit(summaryItemListHouse);
                    return true;
                }
            }
            Monitor.Exit(summaryItemListHouse);
            return false;
        }
        #endregion House
        #region Mobile
        public static bool ContainsMobile(String iItemName)
        {
            Monitor.Enter(summaryItemListMobile);
            foreach (Item item in summaryItemListMobile)
            {
                if (item.Name == iItemName)
                {
                    Monitor.Exit(summaryItemListMobile);
                    return true;
                }
            }
            Monitor.Exit(summaryItemListMobile);
            return false;
        }
        public static bool ContainsMobile(short iItemID)
        {
            Monitor.Enter(summaryItemListMobile);
            foreach (Item item in summaryItemListMobile)
            {
                if (item.ItemID == iItemID)
                {
                    Monitor.Exit(summaryItemListMobile);
                    return true;
                }
            }
            Monitor.Exit(summaryItemListMobile);
            return false;
        }
        public static bool ContainsMobile(Item iItem)
        {
            Monitor.Enter(summaryItemListMobile);
            foreach (Item localItem in summaryItemListMobile)
            {
                if ((iItem.ItemID == localItem.ItemID) && (iItem.Name == localItem.Name))
                {
                    Monitor.Exit(summaryItemListMobile);
                    return true;
                }
            }
            Monitor.Exit(summaryItemListMobile);
            return false;
        }
        #endregion Mobile
        #endregion Contains
        #region Item Quantity
        #region All
        public static ushort GetItemQuan(Item iItem)
        {
            if (!Contains(iItem))
            {
                return 0;
            }
            int idx = indexOf(iItem);
            Monitor.Enter(summaryItemListQuan);
            ushort quan = summaryItemListQuan[idx];
            Monitor.Exit(summaryItemListQuan);
            return quan;
        }
        public static ushort GetItemQuan(String iItemName)
        {
            ushort id = GetItemId(iItemName);
            if (id == 0)
            {
                id = Things.GetIdFromName(iItemName);
            }
            Item itm = new Item(iItemName, id, Item.ITEM_TYPE.UNKNOWN);
            return GetItemQuan(itm);
        }
        public static ushort GetItemQuan(ushort iItemId)
        {
            String name = GetItemName(iItemId);
            if (name == "")
            {
                name = Things.GetNameFromId(iItemId);
            }
            Item itm = new Item(name, iItemId, Item.ITEM_TYPE.UNKNOWN);
            return GetItemQuan(itm);
        }
        #endregion All
        #region House
        public static ushort GetItemQuanHouse(Item iItem)
        {
            if (!ContainsHouse(iItem))
            {
                return 0;
            }
            int idx = indexOfHouse(iItem);
            Monitor.Enter(summaryItemListQuanHouse);
            ushort quan = summaryItemListQuanHouse[idx];
            Monitor.Exit(summaryItemListQuanHouse);
            return quan;
        }
        public static ushort GetItemQuanHouse(String iItemName)
        {
            ushort id = GetItemId(iItemName);
            if (id == 0)
            {
                id = Things.GetIdFromName(iItemName);
            }
            Item itm = new Item(iItemName, id, Item.ITEM_TYPE.UNKNOWN);
            return GetItemQuanHouse(itm);
        }
        public static ushort GetItemQuanHouse(ushort iItemId)
        {
            String name = GetItemName(iItemId);
            if (name == "")
            {
                name = Things.GetNameFromId(iItemId);
            }
            Item itm = new Item(name, iItemId, Item.ITEM_TYPE.UNKNOWN);
            return GetItemQuanHouse(itm);
        }
        #endregion House
        #region Mobile
        public static ushort GetItemQuanMobile(Item iItem)
        {
            if (!ContainsMobile(iItem))
            {
                return 0;
            }
            int idx = indexOfMobile(iItem);
            Monitor.Enter(summaryItemListQuanMobile);
            ushort quan = summaryItemListQuanMobile[idx];
            Monitor.Exit(summaryItemListQuanMobile);
            return quan;
        }
        public static ushort GetItemQuanMobile(String iItemName)
        {
            ushort id = GetItemId(iItemName);
            if (id == 0)
            {
                id = Things.GetIdFromName(iItemName);
            }
            Item itm = new Item(iItemName, id, Item.ITEM_TYPE.UNKNOWN);
            return GetItemQuanMobile(itm);
        }
        public static ushort GetItemQuanMobile(ushort iItemId)
        {
            String name = GetItemName(iItemId);
            if (name == "")
            {
                name = Things.GetNameFromId(iItemId);
            }
            Item itm = new Item(name, iItemId, Item.ITEM_TYPE.UNKNOWN);
            return GetItemQuanMobile(itm);
        }
        #endregion Mobile
        #endregion Item Quantity
        #region Get Item ID/Name
        public static ushort GetItemId(String iItemName)
        {
            ushort id = bag.GetItemId(iItemName);
            if (id != 0)
            {
                return id;
            }
            id = wardrobe.GetItemId(iItemName);
            if (id != 0)
            {
                return id;
            }
            id = satchel.GetItemId(iItemName);
            if (id != 0)
            {
                return id;
            }
            id = sack.GetItemId(iItemName);
            if (id != 0)
            {
                return id;
            }
            id = mcase.GetItemId(iItemName);
            if (id != 0)
            {
                return id;
            }
            id = safe.GetItemId(iItemName);
            if (id != 0)
            {
                return id;
            }
            id = storage.GetItemId(iItemName);
            if (id != 0)
            {
                return id;
            }
            id = locker.GetItemId(iItemName);
            return id;
        }
        public static String GetItemName(ushort iItemId)
        {
            String itemName = bag.GetItemName(iItemId);
            if (itemName != "")
            {
                return itemName;
            }
            itemName = wardrobe.GetItemName(iItemId);
            if (itemName != "")
            {
                return itemName;
            }
            itemName = satchel.GetItemName(iItemId);
            if (itemName != "")
            {
                return itemName;
            }
            itemName = sack.GetItemName(iItemId);
            if (itemName != "")
            {
                return itemName;
            }
            itemName = mcase.GetItemName(iItemId);
            if (itemName != "")
            {
                return itemName;
            }
            itemName = safe.GetItemName(iItemId);
            if (itemName != "")
            {
                return itemName;
            }
            itemName = storage.GetItemName(iItemId);
            if (itemName != "")
            {
                return itemName;
            }
            itemName = locker.GetItemName(iItemId);
            return itemName;
        }
        #endregion Get Item ID/Name
        #region Rebuild Lists
        public static void RebuildLists()
        {
            if (!ChangeMonitor.LoggedIn || !initDone)
            {
                return;
            }
            RebuildListsMobileOnly();
            RebuildListsHouseOnly();

            Monitor.Enter(summaryItemList);
            Monitor.Enter(summaryItemListQuan);
            Monitor.Enter(summaryItemListHouse);
            Monitor.Enter(summaryItemListQuanHouse);
            summaryItemList.Clear();
            summaryItemListQuan.Clear();
            summaryItemList.AddRange(summaryItemListMobile);
            summaryItemListQuan.AddRange(summaryItemListQuanMobile);
            for (int ii = 0; ii < summaryItemListHouse.Count; ii++)
            {
                bool foundItem = false;
                foreach (Item itm in summaryItemList)
                {
                    if (itm.ItemID == summaryItemListHouse[ii].ItemID)
                    {
                        //We already have a record of this item, so just add the quantity to the quantity list.
                        int idx = summaryItemList.IndexOf(itm);
                        summaryItemListQuan[idx] += summaryItemListQuanHouse[ii];
                        foundItem = true;
                        break;
                    }
                }
                if (!foundItem)
                {
                    //We do not have this item yet, just add the item and quantity.
                    summaryItemList.Add(summaryItemListHouse[ii]);
                    summaryItemListQuan.Add(summaryItemListQuanHouse[ii]);
                }
            }
            Monitor.Exit(summaryItemList);
            Monitor.Exit(summaryItemListQuan);
            Monitor.Exit(summaryItemListHouse);
            Monitor.Exit(summaryItemListQuanHouse);
        }
        public static void RebuildListsMobileOnly()
        {
            try
            {
                bag.RebuildLists();
                sack.RebuildLists();
                satchel.RebuildLists();
                mcase.RebuildLists();
                wardrobe.RebuildLists();

                Monitor.Enter(summaryItemListMobile);
                Monitor.Enter(summaryItemListQuanMobile);
                summaryItemListMobile.Clear();
                summaryItemListQuanMobile.Clear();
                List<Item> bagList = bag.GetSummaryItemList();
                List<ushort> bagQuanList = bag.GetSummaryItemQuanList();
                //Go thru each item in the container list and check if it's in our summary list.
                //If it's in the list, just add the quantity to the existing value.
                //If it's not in the list, add it to the list.
                for (int ii = 0; ii < bagList.Count; ii++)
                {
                    bool foundItem = false;
                    foreach (Item itm in summaryItemListMobile)
                    {
                        if (itm.ItemID == bagList[ii].ItemID)
                        {
                            //We already have a record of this item, so just add the quantity to the quantity list.
                            int idx = summaryItemListMobile.IndexOf(itm);
                            summaryItemListQuanMobile[idx] += bagQuanList[ii];
                            foundItem = true;
                            break;
                        }

                    }
                    if (!foundItem)
                    {
                        //We do not have this item yet, just add the item and quantity.
                        summaryItemListMobile.Add(bagList[ii]);
                        summaryItemListQuanMobile.Add(bagQuanList[ii]);
                    }
                }

                List<Item> sackList = sack.GetSummaryItemList();
                List<ushort> sackQuanList = sack.GetSummaryItemQuanList();
                for (int ii = 0; ii < sackList.Count; ii++)
                {
                    bool foundItem = false;
                    foreach (Item itm in summaryItemListMobile)
                    {
                        if (itm.ItemID == sackList[ii].ItemID)
                        {
                            //We already have a record of this item, so just add the quantity to the quantity list.
                            int idx = summaryItemListMobile.IndexOf(itm);
                            summaryItemListQuanMobile[idx] += sackQuanList[ii];
                            foundItem = true;
                            break;
                        }
                    }
                    if (!foundItem)
                    {
                        //We do not have this item yet, just add the item and quantity.
                        summaryItemListMobile.Add(sackList[ii]);
                        summaryItemListQuanMobile.Add(sackQuanList[ii]);
                    }
                }

                List<Item> satchelList = satchel.GetSummaryItemList();
                List<ushort> satchelQuanList = satchel.GetSummaryItemQuanList();
                for (int ii = 0; ii < satchelList.Count; ii++)
                {
                    bool foundItem = false;
                    foreach (Item itm in summaryItemListMobile)
                    {
                        if (itm.ItemID == satchelList[ii].ItemID)
                        {
                            //We already have a record of this item, so just add the quantity to the quantity list.
                            int idx = summaryItemListMobile.IndexOf(itm);
                            summaryItemListQuanMobile[idx] += satchelQuanList[ii];
                            foundItem = true;
                            break;
                        }
                    }
                    if (!foundItem)
                    {
                        //We do not have this item yet, just add the item and quantity.
                        summaryItemListMobile.Add(satchelList[ii]);
                        summaryItemListQuanMobile.Add(satchelQuanList[ii]);
                    }
                }

                List<Item> caseList = mcase.GetSummaryItemList();
                List<ushort> caseQuanList = mcase.GetSummaryItemQuanList();
                for (int ii = 0; ii < caseList.Count; ii++)
                {
                    bool foundItem = false;
                    foreach (Item itm in summaryItemListMobile)
                    {
                        if (itm.ItemID == caseList[ii].ItemID)
                        {
                            //We already have a record of this item, so just add the quantity to the quantity list.
                            int idx = summaryItemListMobile.IndexOf(itm);
                            summaryItemListQuanMobile[idx] += caseQuanList[ii];
                            foundItem = true;
                            break;
                        }
                    }
                    if (!foundItem)
                    {
                        //We do not have this item yet, just add the item and quantity.
                        summaryItemListMobile.Add(caseList[ii]);
                        summaryItemListQuanMobile.Add(caseQuanList[ii]);
                    }
                }

                List<Item> wardrobeList = wardrobe.GetSummaryItemList();
                List<ushort> wardrobeQuanList = wardrobe.GetSummaryItemQuanList();
                for (int ii = 0; ii < wardrobeList.Count; ii++)
                {
                    bool foundItem = false;
                    foreach (Item itm in summaryItemListMobile)
                    {
                        if(itm == null)
                        {
                            LoggingFunctions.Error("Found null item in summary item list mobile.");
                            continue;
                        }
                        else if(wardrobeList == null)
                        {
                            LoggingFunctions.Error("Found wardrobe list to be null.");
                            continue;
                        }
                        else if(wardrobeList[ii] == null)
                        {
                            LoggingFunctions.Error("Found null wardrobe list item.");
                            continue;
                        }
                        if (itm.ItemID == wardrobeList[ii].ItemID)
                        {
                            //We already have a record of this item, so just add the quantity to the quantity list.
                            int idx = summaryItemListMobile.IndexOf(itm);
                            summaryItemListQuanMobile[idx] += wardrobeQuanList[ii];
                            foundItem = true;
                            break;
                        }
                    }
                    if (!foundItem)
                    {
                        //We do not have this item yet, just add the item and quantity.
                        summaryItemListMobile.Add(wardrobeList[ii]);
                        summaryItemListQuanMobile.Add(wardrobeQuanList[ii]);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingFunctions.Error("RebuildListsMobileOnly: Error adding individual summaries to mobile summary: " + ex.ToString());
            }
            finally
            {
                Monitor.Exit(summaryItemListMobile);
                Monitor.Exit(summaryItemListQuanMobile);
                if (_CurrentCapacityValuesUpdated != null)
                {
                    _CurrentCapacityValuesUpdated();
                }
            }
        }
        public static void RebuildListsHouseOnly()
        {
            try
            {
                safe.RebuildLists();
                storage.RebuildLists();
                locker.RebuildLists();

                Monitor.Enter(summaryItemListHouse);
                Monitor.Enter(summaryItemListQuanHouse);
                summaryItemListHouse.Clear();
                summaryItemListQuanHouse.Clear();
                List<Item> safeList = safe.GetSummaryItemList();
                List<ushort> safeQuanList = safe.GetSummaryItemQuanList();
                for (int ii = 0; ii < safeList.Count; ii++)
                {
                    bool foundItem = false;
                    foreach (Item itm in summaryItemListHouse)
                    {
                        if (itm.ItemID == safeList[ii].ItemID)
                        {
                            //We already have a record of this item, so just add the quantity to the quantity list.
                            int idx = summaryItemListHouse.IndexOf(itm);
                            summaryItemListQuanHouse[idx] += safeQuanList[ii];
                            foundItem = true;
                            break;
                        }
                    }
                    if (!foundItem)
                    {
                        //We do not have this item yet, just add the item and quantity.
                        summaryItemListHouse.Add(safeList[ii]);
                        summaryItemListQuanHouse.Add(safeQuanList[ii]);
                    }
                }

                List<Item> storageList = storage.GetSummaryItemList();
                List<ushort> storageQuanList = storage.GetSummaryItemQuanList();
                for (int ii = 0; ii < storageList.Count; ii++)
                {
                    bool foundItem = false;
                    foreach (Item itm in summaryItemListHouse)
                    {
                        if (itm.ItemID == storageList[ii].ItemID)
                        {
                            //We already have a record of this item, so just add the quantity to the quantity list.
                            int idx = summaryItemListHouse.IndexOf(itm);
                            summaryItemListQuanHouse[idx] += storageQuanList[ii];
                            foundItem = true;
                            break;
                        }
                    }
                    if (!foundItem)
                    {
                        //We do not have this item yet, just add the item and quantity.
                        summaryItemListHouse.Add(storageList[ii]);
                        summaryItemListQuanHouse.Add(storageQuanList[ii]);
                    }
                }

                List<Item> lockerList = locker.GetSummaryItemList();
                List<ushort> lockerQuanList = locker.GetSummaryItemQuanList();
                for (int ii = 0; ii < lockerList.Count; ii++)
                {
                    bool foundItem = false;
                    foreach (Item itm in summaryItemListHouse)
                    {
                        if (itm.ItemID == lockerList[ii].ItemID)
                        {
                            //We already have a record of this item, so just add the quantity to the quantity list.
                            int idx = summaryItemListHouse.IndexOf(itm);
                            summaryItemListQuanHouse[idx] += lockerQuanList[ii];
                            foundItem = true;
                            break;
                        }
                    }
                    if (!foundItem)
                    {
                        //We do not have this item yet, just add the item and quantity.
                        summaryItemListHouse.Add(lockerList[ii]);
                        summaryItemListQuanHouse.Add(lockerQuanList[ii]);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingFunctions.Error("RebuildListsHouseOnly: Error adding individual summaries to house summary: " + ex.ToString());
            }
            finally
            {
                Monitor.Exit(summaryItemListHouse);
                Monitor.Exit(summaryItemListQuanHouse);
                if (_CurrentCapacityValuesUpdated != null)
                {
                    _CurrentCapacityValuesUpdated();
                }
            }
        }
        #endregion Rebuild Lists
        #endregion Public Methods
    }
}
