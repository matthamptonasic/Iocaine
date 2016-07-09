using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Iocaine2.Inventory;
using Iocaine2.Logging;

namespace Iocaine2
{
    public class Seller_Script
    {
        #region Constructors
        public Seller_Script()
        {
        }
        public Seller_Script(Seller_Script iScript)
        {
            scriptName = iScript.ScriptName;
            npcName = iScript.NpcName;
            sellAll = iScript.SellAll;
            sellFromSack = iScript.SellFromSack;
            sellFromSatchel = iScript.SellFromSatchel;
            sellFromCase = iScript.SellFromCase;
            itemList = iScript.ItemList;
            itemQuanList = iScript.ItemQuanList;
        }
        #endregion Constructors
        #region Private Members
        private String scriptName = "";
        private String npcName = "";
        private bool sellAll = false;
        private bool sellFromSatchel = false;
        private bool sellFromSack = false;
        private bool sellFromCase = false;
        private List<Item> itemList = new List<Item>();
        private List<ushort> itemQuanList = new List<ushort>();
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
        public bool SellAll
        {
            get
            {
                return sellAll;
            }
            set
            {
                sellAll = value;
            }
        }
        public bool SellFromSatchel
        {
            get
            {
                return sellFromSatchel;
            }
            set
            {
                sellFromSatchel = value;
            }
        }
        public bool SellFromSack
        {
            get
            {
                return sellFromSack;
            }
            set
            {
                sellFromSack = value;
            }
        }
        public bool SellFromCase
        {
            get
            {
                return sellFromCase;
            }
            set
            {
                sellFromCase = value;
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
        #endregion Public Members/Properties
        #region Private Functions
        #endregion Private Functions
        #region Public Functions
        public void AddItem(Item iItem, ushort iQuan)
        {
            try
            {
                itemList.Add(iItem);
                itemQuanList.Add(iQuan);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller_Script::AddItem(Item, ushort): " + e.ToString());
            }
        }
        public void AddItem(String iName, ushort iId, ushort iQuan)
        {
            try
            {
                itemList.Add(new Item(iName, iId, Item.ITEM_TYPE.UNKNOWN));
                itemQuanList.Add(iQuan);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller_Script::AddItem(String, ushort, ushort): " + e.ToString());
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
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller_Script::RemoveItem(String, ushort): " + e.ToString());
                return false;
            }
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
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller_Script::RemoveItem(ushort, ushort): " + e.ToString());
                return false;
            }
        }
        public void ClearLists()
        {
            try
            {
                itemList.Clear();
                itemQuanList.Clear();
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Seller_Script::ClearLists: " + e.ToString());
            }
        }
        #endregion Public Functions
    }
}
