using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Iocaine2.Data.Client;
using Iocaine2.Inventory;
using Iocaine2.Logging;

namespace Iocaine2
{
    public class Buyer_Script
    {
        #region Constructors
        public Buyer_Script()
        {
        }
        public Buyer_Script(Buyer_Script iScript)
        {
            scriptName = iScript.ScriptName;
            npcName = iScript.NpcName;
            mode = iScript.mode;
            combinedTotalQuan = iScript.CombinedTotalQuan;
            percentageValue = iScript.PercentageValue;
            leaveOpenSlotsQuan = iScript.LeaveOpenSlotsQuan;
            includeSack = iScript.IncludeSack;
            includeSatchel = iScript.IncludeSatchel;
            itemList = iScript.ItemList;
            itemQuanList = iScript.ItemQuanList;
            pricePerItemList = iScript.PricePerItemList;
        }
        #endregion Constructors
        #region Private Members
        private String scriptName = "";
        private String npcName = "";
        private Byte mode = 0;
        private UInt16 combinedTotalQuan = 0;
        private Byte percentageValue = 0;
        private UInt16 leaveOpenSlotsQuan = 0;
        private bool includeSatchel = false;
        private bool includeSack = false;
        private List<Item> itemList = new List<Item>();
        private List<ushort> itemQuanList = new List<ushort>();
        private List<UInt32> pricePerItemList = new List<uint>();
        #endregion Private Members
        #region Public Members/Properties
        public String ScriptName
        {
            get
            {
                return scriptName;
            }
            set
            {
                scriptName = value;
            }
        }
        public String NpcName
        {
            get
            {
                return npcName;
            }
            set
            {
                npcName = value;
            }
        }
        public Byte Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
            }
        }
        public UInt16 CombinedTotalQuan
        {
            get
            {
                return combinedTotalQuan;
            }
            set
            {
                combinedTotalQuan = value;
            }
        }
        public Byte PercentageValue
        {
            get
            {
                return percentageValue;
            }
            set
            {
                percentageValue = value;
            }
        }
        public UInt16 LeaveOpenSlotsQuan
        {
            get
            {
                return leaveOpenSlotsQuan;
            }
            set
            {
                leaveOpenSlotsQuan = value;
            }
        }
        public bool IncludeSatchel
        {
            get
            {
                return includeSatchel;
            }
            set
            {
                includeSatchel = value;
            }
        }
        public bool IncludeSack
        {
            get
            {
                return includeSack;
            }
            set
            {
                includeSack = value;
            }
        }
        /// <summary>
        /// Gets a COPY of the Item List.
        /// </summary>
        public List<Item> ItemList
        {
            get
            {
                List<Item> list = new List<Item>();
                int nbItems = itemList.Count;
                for (int ii = 0; ii < nbItems; ii++)
                {
                    list.Add(new Item(itemList[ii]));
                }
                return list;
            }
        }
        /// <summary>
        /// Gets a COPY of the Item Quantity List.
        /// </summary>
        public List<ushort> ItemQuanList
        {
            get
            {
                List<ushort> list = new List<ushort>();
                int nbItems = itemQuanList.Count;
                for (int ii = 0; ii < nbItems; ii++)
                {
                    list.Add(itemQuanList[ii]);
                }
                return list;
            }
        }
        /// <summary>
        /// Gets a COPY of the Price Per Item List.
        /// </summary>
        public List<UInt32> PricePerItemList
        {
            get
            {
                List<UInt32> list = new List<uint>();
                int nbItems = pricePerItemList.Count;
                for (int ii = 0; ii < nbItems; ii++)
                {
                    list.Add(pricePerItemList[ii]);
                }
                return list;
            }
        }
        #endregion Public Members/Properties
        #region Private Functions
        #endregion Private Functions
        #region Public Functions
        public void AddItem(Item iItem, ushort iQuan, UInt32 iPrice)
        {
            try
            {
                itemList.Add(iItem);
                itemQuanList.Add(iQuan);
                pricePerItemList.Add(iPrice);
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer_Script::AddItem(Item, ushort, uint): " + e.ToString());
            }
        }
        public void AddItem(String iName, ushort iId, ushort iQuan, UInt32 iPrice)
        {
            try
            {
                itemList.Add(new Item(iName, iId, Things.ITEM_TYPE.UNKNOWN));
                itemQuanList.Add(iQuan);
                pricePerItemList.Add(iPrice);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Buyer_Script::AddItem(Item, ushort, ushort, uint): " + e.ToString());
            }
        }
        public bool RemoveItem(String iName, ushort iQuan)
        {
            try
            {
                for (int ii = 0; ii < itemList.Count; ii++)
                {
                    if ((itemList[ii].Name == iName) && (itemQuanList[ii] == iQuan))
                    {
                        itemList.RemoveAt(ii);
                        itemQuanList.RemoveAt(ii);
                        pricePerItemList.RemoveAt(ii);
                        return true;
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer_Script::RemoveItem(String, ushort): " + e.ToString());
            }
            return false;
        }
        public bool RemoveItem(ushort iItemId, ushort iQuan)
        {
            try
            {
                for (int ii = 0; ii < itemList.Count; ii++)
                {
                    if ((itemList[ii].ItemID == iItemId) && (itemQuanList[ii] == iQuan))
                    {
                        itemList.RemoveAt(ii);
                        itemQuanList.RemoveAt(ii);
                        pricePerItemList.RemoveAt(ii);
                        return true;
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer_Script::RemoveItem(ushort, ushort): " + e.ToString());
            }
            return false;
        }
        public void ClearLists()
        {
            try
            {
                itemList.Clear();
                itemQuanList.Clear();
                pricePerItemList.Clear();
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer_Script::ClearLists: " + e.ToString());
            }
        }
        #endregion Public Functions
    }
}
