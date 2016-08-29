using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using Iocaine2.Data.Client;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Tools;

namespace Iocaine2.Inventory
{
    public class ItemContainer
    {

        #region Enums
        public enum STORAGE_TYPE : byte
        {
            BAG = 0,
            SATCHEL = 1,
            SACK = 2,
            CASE = 3,
            SAFE = 4,
            STORAGE = 5,
            LOCKER = 6,
            WARDROBE = 7,
            SAFE2 = 8,
            WARDROBE2 = 9,
            WARDROBE3 = 10,
            WARDROBE4 = 11
        }
        public enum CHILD_TYPE : byte
        {
            GENERAL,
            EQUIP,
            BOTH
        }
        #endregion Enums
        #region Constructors
        public ItemContainer(STORAGE_TYPE iType)
        {
            init(iType);
        }
        private void init(STORAGE_TYPE iType)
        {
            fullItemList = new List<Item>();
            fullItemListQuan = new List<ushort>();
            summaryItemList = new List<Item>();
            summaryItemListQuan = new List<ushort>();
            itemNameHistory = new List<string>();
            itemIdHistory = new List<ushort>();
            type = iType;
            setCapacityMax();
            setTypeStrings();
            setDelegates();
        }
        #endregion Constructors
        #region Member Variables
        #region Private Members
        private STORAGE_TYPE type = STORAGE_TYPE.BAG;
        protected CHILD_TYPE childType = CHILD_TYPE.GENERAL;
        private String typeString = "Bag";
        private String typeStringAbbr = "Bag";
        private List<Item> fullItemList;
        private List<ushort> fullItemListQuan;
        private List<Item> summaryItemList;
        private List<ushort> summaryItemListQuan;
        private List<String> itemNameHistory;
        private List<ushort> itemIdHistory;
        private byte capacityMax = 0;
        private byte occupancy = 0;
        private delegate ushort info_item_id_delegate(short structIndex);
        private delegate byte info_item_quan_delegate(short structIndex);
        private info_item_id_delegate info_item_id_ptr;
        private info_item_quan_delegate info_item_quan_ptr;
        #endregion Private Members
        #region Public Members
        #endregion Public Members
        #endregion Member Variables
        #region Methods/Properties
        #region Contains Methods
        public bool Contains(String iItemName, bool iLock = true)
        {
            if (iLock)
            {
                Monitor.Enter(Inventory.Containers.Padlock);
            }
            foreach (Item item in summaryItemList)
            {
                if (item.Name == iItemName)
                {
                    if (iLock)
                    {
                        Monitor.Exit(Inventory.Containers.Padlock);
                    }
                    return true;
                }
            }
            if (iLock)
            {
                Monitor.Exit(Inventory.Containers.Padlock);
            }
            return false;
        }
        public bool Contains(short iItemID, bool iLock = true)
        {
            if (iLock)
            {
                Monitor.Enter(Inventory.Containers.Padlock);
            }
            foreach (Item item in summaryItemList)
            {
                if (item.ItemID == iItemID)
                {
                    if (iLock)
                    {
                        Monitor.Exit(Inventory.Containers.Padlock);
                    }
                    return true;
                }
            }
            if (iLock)
            {
                Monitor.Exit(Inventory.Containers.Padlock);
            }
            return false;
        }
        public bool Contains(Item iItem, bool iLock = true)
        {
            try
            {
                if (iLock)
                {
                    Monitor.Enter(Inventory.Containers.Padlock);
                }
                foreach (Item localItem in summaryItemList)
                {
                    if (iItem.ItemID == localItem.ItemID)
                    {
                        if (iLock)
                        {
                            Monitor.Exit(Inventory.Containers.Padlock);
                        }
                        return true;
                    }
                }
                if (iLock)
                {
                    Monitor.Exit(Inventory.Containers.Padlock);
                }
                return false;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error(this.type.ToString() + ".Contains: " + e.ToString());
                Monitor.Exit(Inventory.Containers.Padlock);
                return false;
            }
        }
        #endregion Contains Methods
        #region Get Item ID, Name, Quantity
        /// <summary>
        /// Returns the item ID for the given item name. Returns 0 if ID not found.
        /// </summary>
        /// <param name="iName"></param>
        /// <returns></returns>
        public ushort GetItemId(String iName)
        {
            Monitor.Enter(Inventory.Containers.Padlock);
            if (itemNameHistory.Contains(iName))
            {
                Monitor.Exit(Inventory.Containers.Padlock);
                return (ushort)itemIdHistory[itemNameHistory.IndexOf(iName)];
            }
            else
            {
                Monitor.Exit(Inventory.Containers.Padlock);
                LoggingFunctions.Timestamp("Could not find item: " + iName + " in memory.");
                return 0;
            }
        }
        /// <summary>
        /// Returns the name of the given item ID, returns empty string if not found.
        /// </summary>
        /// <param name="iItemId"></param>
        /// <returns></returns>
        public String GetItemName(ushort iItemId)
        {
            Monitor.Enter(Inventory.Containers.Padlock);
            if (itemIdHistory.Contains(iItemId))
            {
                String itemName = itemNameHistory[itemIdHistory.IndexOf(iItemId)];
                Monitor.Exit(Inventory.Containers.Padlock);
                return itemName;
            }
            else
            {
                Monitor.Exit(Inventory.Containers.Padlock);
                return "";
            }
        }
        /// <summary>
        /// Gets the total number of items specified in this container.
        /// </summary>
        /// <param name="iItem">The item you wish to know the count of.</param>
        /// <returns>The toal quantity of the item in this container.</returns>
        public ushort GetItemQuan(Item iItem)
        {
            if (!Contains(iItem))
            {
                return 0;
            }
            int idx = indexOf(iItem);
            Monitor.Enter(Inventory.Containers.Padlock);
            ushort quan = summaryItemListQuan[idx];
            Monitor.Exit(Inventory.Containers.Padlock);
            return quan;
        }
        /// <summary>
        /// Gets the total number of items specified in this container.
        /// </summary>
        /// <param name="iItemName">The item you wish to know the count of.</param>
        /// <returns>The toal quantity of the item in this container.</returns>
        public ushort GetItemQuan(String iItemName)
        {
            ushort id = GetItemId(iItemName);
            if (id == 0)
            {
                id = Things.GetIdFromName(iItemName);
            }
            Item itm = new Item(iItemName, id, Item.ITEM_TYPE.UNKNOWN);
            return GetItemQuan(itm);
        }
        /// <summary>
        /// Gets the total number of items specified in this container.
        /// </summary>
        /// <param name="iItemId">The item you wish to know the count of.</param>
        /// <returns>The toal quantity of the item in this container.</returns>
        public ushort GetItemQuan(ushort iItemId)
        {
            String name = GetItemName(iItemId);
            if(name == "")
            {
                name = Things.GetNameFromId(iItemId);
            }
            Item itm = new Item(name, iItemId, Item.ITEM_TYPE.UNKNOWN);
            return GetItemQuan(itm);
        }
        #endregion Get Item ID, Name, Quantity
        #region List Methods
        /// <summary>
        /// Gets a summary of items in this container (aggregated list).
        /// </summary>
        /// <returns>Returns the summary list of items for this container (aggregated list).</returns>
        public List<Item> GetSummaryItemList()
        {
            return summaryItemList;
        }
        /// <summary>
        /// Gets a summary list of item quantities (aggregated list) corresponding to the summary item list.
        /// </summary>
        /// <returns>Returns the summary list of item quantities for this container (aggregated list).</returns>
        public List<ushort> GetSummaryItemQuanList()
        {
            return summaryItemListQuan;
        }
        /// <summary>
        /// Gets the full list of items in this container.
        /// </summary>
        /// <returns>Returns a list of all items (non-aggregated) for this container.</returns>
        public List<Item> GetFullItemList()
        {
            return fullItemList;
        }
        /// <summary>
        /// Gets the full list of item quantities in this container.
        /// </summary>
        /// <returns>Returns a list of all item quantities (non-aggregated) corresponding to the full item list for this container.</returns>
        public List<ushort> GetFullItemQuanList()
        {
            return fullItemListQuan;
        }
        /// <summary>
        /// Rebuilds the internal list structures from POL memory.
        /// </summary>
        public void RebuildLists()
        {
            Monitor.Enter(Inventory.Containers.Padlock);
            //First we'll build a summary list of items, but no quantities.
            summaryItemList.Clear();
            summaryItemListQuan.Clear();
            fullItemList.Clear();
            fullItemListQuan.Clear();

            //Then we'll go thru each item location in the bag and get the quantity.
            //From this we'll keep a running total for the summary and add each 1 for the full list.
            LoggingFunctions.Debug("Checking all " + typeString + " locations.", LoggingFunctions.DBG_SCOPE.INVENTORY);
            setCapacityMax();
            occupancy = 0;
            for (short ii = 1; ii <= capacityMax; ii++)
            {
                ushort itemID = 0;
                byte itemQuan = 0;
                String itemName = "";
                ushort itemType = 0;
                int indexInSummary = 0;
                Item item = null;
                itemID = info_item_id_ptr(ii);
                if (itemID == 0)
                {
                    continue;
                }
                occupancy++;
                itemQuan = info_item_quan_ptr(ii);
                itemType = Things.GetTypeFromId(itemID);
                itemName = Things.GetNameFromId(itemID);
                if (itemName == "." || itemName == "")
                {
                    itemName = "Unknown Item";
                }
                item = new Item(itemName, itemID, (Item.ITEM_TYPE)itemType);
                if (!itemIdHistory.Contains(itemID))
                {
                    itemIdHistory.Add(itemID);
                    itemNameHistory.Add(itemName);
                }
                bool foundInSummary = false;
                foreach (Item localItem in summaryItemList)
                {
                    if (localItem.ItemID == itemID)
                    {
                        indexInSummary = summaryItemList.IndexOf(localItem);
                        summaryItemListQuan[indexInSummary] += itemQuan;
                        foundInSummary = true;
                        break;
                    }
                }
                if (!foundInSummary)
                {
                    summaryItemList.Add(item);
                    summaryItemListQuan.Add(itemQuan);
                }
                fullItemList.Add(item);
                fullItemListQuan.Add(itemQuan);
            }
            int nbItems = fullItemList.Count;
            LoggingFunctions.Debug("All items found =================================", LoggingFunctions.DBG_SCOPE.INVENTORY);
            for (int ii = 0; ii < nbItems; ii++)
            {
                LoggingFunctions.Debug("Item: " + fullItemList[ii].Name + " :: " + fullItemListQuan[ii], LoggingFunctions.DBG_SCOPE.INVENTORY);
            }
            nbItems = summaryItemList.Count;
            LoggingFunctions.Debug("Item summary =================================", LoggingFunctions.DBG_SCOPE.INVENTORY);
            for (int ii = 0; ii < nbItems; ii++)
            {
                LoggingFunctions.Debug("Item: " + summaryItemList[ii].Name + " :: " + summaryItemListQuan[ii], LoggingFunctions.DBG_SCOPE.INVENTORY);
            }
            Monitor.Exit(Inventory.Containers.Padlock);
        }
        public bool GetPrunedItemList(List<ushort> iItemIDsToPrune, ref List<Item> oPrunedItems, ref List<ushort> oPrunedItemQuan)
        {
            bool oItemsPruned = false;      //Return value if we pruned any items.

            bool thisItemPruned = false;    //Set to break out of the outer loop.
            for (int ii = 0; ii < fullItemList.Count; ii++)
            {
                foreach (ushort id in iItemIDsToPrune)
                {
                    if (id == fullItemList[ii].ItemID)
                    {
                        oItemsPruned = true;
                        thisItemPruned = true;
                        break;
                    }
                }
                if (thisItemPruned == true)
                {
                    thisItemPruned = false;
                }
                else
                {
                    oPrunedItems.Add(fullItemList[ii]);
                    oPrunedItemQuan.Add(fullItemListQuan[ii]);
                }
            }

            return oItemsPruned;
        }
        #endregion List Methods
        #region Occupancy Methods/Properties
        /// <summary>
        /// Gets the maximum capacity of the container at the last time it was checked.
        /// (During initialization or when the lists are rebuilt).
        /// </summary>
        public byte Capacity
        {
            get
            {
                return capacityMax;
            }
        }
        /// <summary>
        /// Gets the current maximum capacity of the container.
        /// </summary>
        public byte LiveCapacity
        {
            get
            {
                return getCapacityMax();
            }
        }
        /// <summary>
        /// Returns the occupancy at the last list rebuild time.
        /// </summary>
        public byte Occupancy
        {
            get
            {
                return occupancy;
            }
        }
        /// <summary>
        /// Rebuilds the lists and returns current occupancy.
        /// </summary>
        public byte LiveOccupancy
        {
            get
            {
                RebuildLists();
                return occupancy;
            }
        }
        /// <summary>
        /// Returns whether the container was full at the last list rebuild time.
        /// </summary>
        public bool Full
        {
            get
            {
                return occupancy == capacityMax;
            }
        }
        /// <summary>
        /// Rebuilds the lists and returns whether the container is currently full.
        /// </summary>
        public bool LiveFull
        {
            get
            {
                RebuildLists();
                return occupancy == capacityMax;
            }
        }
        #endregion Occupancy Methods/Properties
        #region Get Type
        public STORAGE_TYPE StorageType
        {
            get
            {
                return type;
            }
        }
        public String TypeString
        {
            get
            {
                return typeString;
            }
        }
        public String TypeStringAbbr
        {
            get
            {
                return typeStringAbbr;
            }
        }
        #endregion Get Type
        #region Utility Methods
        private void setCapacityMax()
        {
            switch (type)
            {
                case STORAGE_TYPE.BAG:
                    capacityMax = MemReads.Self.Inventory.get_max_bag();
                    
                    break;
                case STORAGE_TYPE.SAFE:
                    capacityMax = MemReads.Self.Inventory.get_max_safe();
                    break;
                case STORAGE_TYPE.STORAGE:
                    capacityMax = MemReads.Self.Inventory.get_max_storage();
                    break;
                case STORAGE_TYPE.LOCKER:
                    capacityMax = MemReads.Self.Inventory.get_max_locker();
                    break;
                case STORAGE_TYPE.SATCHEL:
                    capacityMax = MemReads.Self.Inventory.get_max_satchel();
                    break;
                case STORAGE_TYPE.SACK:
                    capacityMax = MemReads.Self.Inventory.get_max_sack();
                    break;
                case STORAGE_TYPE.CASE:
                    capacityMax = MemReads.Self.Inventory.get_max_case();
                    break;
                case STORAGE_TYPE.SAFE2:
                    capacityMax = MemReads.Self.Inventory.get_max_safe2();
                    break;
                case STORAGE_TYPE.WARDROBE:
                    capacityMax = MemReads.Self.Inventory.get_max_wardrobe();
                    break;
                case STORAGE_TYPE.WARDROBE2:
                    capacityMax = MemReads.Self.Inventory.get_max_wardrobe2();
                    break;
                case STORAGE_TYPE.WARDROBE3:
                    capacityMax = MemReads.Self.Inventory.get_max_wardrobe3();
                    break;
                case STORAGE_TYPE.WARDROBE4:
                    capacityMax = MemReads.Self.Inventory.get_max_wardrobe4();
                    break;
                default:
                    capacityMax = MemReads.Self.Inventory.get_max_bag();
                    break;
            }
        }
        private byte getCapacityMax()
        {
            byte cap = 0;
            switch (type)
            {
                case STORAGE_TYPE.BAG:
                    cap = MemReads.Self.Inventory.get_max_bag();
                    break;
                case STORAGE_TYPE.SAFE:
                    cap = MemReads.Self.Inventory.get_max_safe();
                    break;
                case STORAGE_TYPE.STORAGE:
                    cap = MemReads.Self.Inventory.get_max_storage();
                    break;
                case STORAGE_TYPE.LOCKER:
                    cap = MemReads.Self.Inventory.get_max_locker();
                    break;
                case STORAGE_TYPE.SATCHEL:
                    cap = MemReads.Self.Inventory.get_max_satchel();
                    break;
                case STORAGE_TYPE.SACK:
                    cap = MemReads.Self.Inventory.get_max_sack();
                    break;
                case STORAGE_TYPE.CASE:
                    cap = MemReads.Self.Inventory.get_max_case();
                    break;
                case STORAGE_TYPE.SAFE2:
                    cap = MemReads.Self.Inventory.get_max_safe2();
                    break;
                case STORAGE_TYPE.WARDROBE:
                    cap = MemReads.Self.Inventory.get_max_wardrobe();
                    break;
                case STORAGE_TYPE.WARDROBE2:
                    cap = MemReads.Self.Inventory.get_max_wardrobe2();
                    break;
                case STORAGE_TYPE.WARDROBE3:
                    cap = MemReads.Self.Inventory.get_max_wardrobe3();
                    break;
                case STORAGE_TYPE.WARDROBE4:
                    cap = MemReads.Self.Inventory.get_max_wardrobe4();
                    break;
                default:
                    cap = MemReads.Self.Inventory.get_max_bag();
                    break;
            }
            return cap;
        }
        private void setTypeStrings()
        {
            switch (type)
            {
                case STORAGE_TYPE.BAG:
                    typeString = "Bag";
                    typeStringAbbr = "Bag";
                    break;
                case STORAGE_TYPE.SAFE:
                    typeString = "Safe";
                    typeStringAbbr = "Safe";
                    break;
                case STORAGE_TYPE.STORAGE:
                    typeString = "Storage";
                    typeStringAbbr = "Strg";
                    break;
                case STORAGE_TYPE.LOCKER:
                    typeString = "Locker";
                    typeStringAbbr = "Lckr";
                    break;
                case STORAGE_TYPE.SATCHEL:
                    typeString = "Satchel";
                    typeStringAbbr = "Schl";
                    break;
                case STORAGE_TYPE.SACK:
                    typeString = "Sack";
                    typeStringAbbr = "Sack";
                    break;
                case STORAGE_TYPE.CASE:
                    typeString = "Case";
                    typeStringAbbr = "Case";
                    break;
                case STORAGE_TYPE.SAFE2:
                    typeString = "Safe2";
                    typeStringAbbr = "Safe2";
                    break;
                case STORAGE_TYPE.WARDROBE:
                    typeString = "Wardrobe";
                    typeStringAbbr = "Wrd";
                    break;
                case STORAGE_TYPE.WARDROBE2:
                    typeString = "Wardrobe2";
                    typeStringAbbr = "Wrd2";
                    break;
                case STORAGE_TYPE.WARDROBE3:
                    typeString = "Wardrobe3";
                    typeStringAbbr = "Wrd3";
                    break;
                case STORAGE_TYPE.WARDROBE4:
                    typeString = "Wardrobe4";
                    typeStringAbbr = "Wrd4";
                    break;
                default:
                    typeString = "Bag";
                    typeStringAbbr = "Bag";
                    break;
            }
        }
        private void setDelegates()
        {
            switch (type)
            {
                case STORAGE_TYPE.BAG:
                    info_item_id_ptr = new info_item_id_delegate(MemReads.Self.Inventory.get_bag_item_id);
                    info_item_quan_ptr = new info_item_quan_delegate(MemReads.Self.Inventory.get_bag_item_quan);
                    break;
                case STORAGE_TYPE.SAFE:
                    info_item_id_ptr = new info_item_id_delegate(MemReads.Self.Inventory.get_safe_item_id);
                    info_item_quan_ptr = new info_item_quan_delegate(MemReads.Self.Inventory.get_safe_item_quan);
                    break;
                case STORAGE_TYPE.STORAGE:
                    info_item_id_ptr = new info_item_id_delegate(MemReads.Self.Inventory.get_storage_item_id);
                    info_item_quan_ptr = new info_item_quan_delegate(MemReads.Self.Inventory.get_storage_item_quan);
                    break;
                case STORAGE_TYPE.LOCKER:
                    info_item_id_ptr = new info_item_id_delegate(MemReads.Self.Inventory.get_locker_item_id);
                    info_item_quan_ptr = new info_item_quan_delegate(MemReads.Self.Inventory.get_locker_item_quan);
                    break;
                case STORAGE_TYPE.SATCHEL:
                    info_item_id_ptr = new info_item_id_delegate(MemReads.Self.Inventory.get_satchel_item_id);
                    info_item_quan_ptr = new info_item_quan_delegate(MemReads.Self.Inventory.get_satchel_item_quan);
                    break;
                case STORAGE_TYPE.SACK:
                    info_item_id_ptr = new info_item_id_delegate(MemReads.Self.Inventory.get_sack_item_id);
                    info_item_quan_ptr = new info_item_quan_delegate(MemReads.Self.Inventory.get_sack_item_quan);
                    break;
                case STORAGE_TYPE.CASE:
                    info_item_id_ptr = new info_item_id_delegate(MemReads.Self.Inventory.get_case_item_id);
                    info_item_quan_ptr = new info_item_quan_delegate(MemReads.Self.Inventory.get_case_item_quan);
                    break;
                case STORAGE_TYPE.SAFE2:
                    info_item_id_ptr = new info_item_id_delegate(MemReads.Self.Inventory.get_safe2_item_id);
                    info_item_quan_ptr = new info_item_quan_delegate(MemReads.Self.Inventory.get_safe2_item_quan);
                    break;
                case STORAGE_TYPE.WARDROBE:
                    info_item_id_ptr = new info_item_id_delegate(MemReads.Self.Inventory.get_wardrobe_item_id);
                    info_item_quan_ptr = new info_item_quan_delegate(MemReads.Self.Inventory.get_wardrobe_item_quan);
                    break;
                case STORAGE_TYPE.WARDROBE2:
                    info_item_id_ptr = new info_item_id_delegate(MemReads.Self.Inventory.get_wardrobe2_item_id);
                    info_item_quan_ptr = new info_item_quan_delegate(MemReads.Self.Inventory.get_wardrobe2_item_quan);
                    break;
                case STORAGE_TYPE.WARDROBE3:
                    info_item_id_ptr = new info_item_id_delegate(MemReads.Self.Inventory.get_wardrobe3_item_id);
                    info_item_quan_ptr = new info_item_quan_delegate(MemReads.Self.Inventory.get_wardrobe3_item_quan);
                    break;
                case STORAGE_TYPE.WARDROBE4:
                    info_item_id_ptr = new info_item_id_delegate(MemReads.Self.Inventory.get_wardrobe4_item_id);
                    info_item_quan_ptr = new info_item_quan_delegate(MemReads.Self.Inventory.get_wardrobe4_item_quan);
                    break;
                default:
                    info_item_id_ptr = new info_item_id_delegate(MemReads.Self.Inventory.get_wardrobe_item_id);
                    info_item_quan_ptr = new info_item_quan_delegate(MemReads.Self.Inventory.get_wardrobe_item_quan);
                    break;
            }
        }
        private void openCloseBag()
        {
            Player_MenuNavigation.CloseCheck();
            IocaineFunctions.twoKeys(System.Windows.Forms.Keys.LControlKey, System.Windows.Forms.Keys.I, 250);
            IocaineFunctions.delay(500);
            int cnt = 0;
            while (MemReads.Windows.BannerText.get_top_left_text() != "Items")
            {
                if (cnt >= 20)
                {
                    LoggingFunctions.Error("Could not sort inventory properly. Top left text was: " + MemReads.Windows.BannerText.get_top_left_text());
                    return;
                }
                cnt++;
                IocaineFunctions.delay(100);
            }
            IocaineFunctions.delay(300);
            Player_MenuNavigation.CloseCheck();
        }
        private int indexOf(Item iItem)
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
        #endregion Utility Methods
        #endregion Methods/Properties
    }

    public class EquipmentContainer : ItemContainer
    {
        public EquipmentContainer(STORAGE_TYPE iType, byte iEquipLocation)
            : base(iType)
        {
            equipLocation = iEquipLocation;
            if (iType == STORAGE_TYPE.BAG)
            {
                childType = CHILD_TYPE.BOTH;
            }
            else
            {
                childType = CHILD_TYPE.EQUIP;
            }
        }
        private byte equipLocation = 0;
        public byte EquipLocation
        {
            get
            {
                return equipLocation;
            }
        }
    }
}
