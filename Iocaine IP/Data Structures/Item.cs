using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Iocaine2.Data.Client;

namespace Iocaine2.Inventory
{
    public class Item
    {
        #region Enums
        #endregion Enums

        #region Private Members
        private String name;
        private ushort itemID;
        private Things.ITEM_TYPE type;
        #endregion Private Members

        #region Public Properties
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public ushort ItemID
        {
            get
            {
                return itemID;
            }
            set
            {
                itemID = value;
            }
        }
        public Things.ITEM_TYPE Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        #endregion Public Properties

        #region Constructors
        public Item(String iName, ushort iID, Things.ITEM_TYPE iType)
        {
            name = iName;
            itemID = iID;
            type = iType;
        }
        public Item(Item iItem)
        {
            name = iItem.Name;
            itemID = iItem.ItemID;
            type = iItem.Type;
        }
        #endregion Constructors
        
        #region Methods
        #endregion Methods
    }
}
