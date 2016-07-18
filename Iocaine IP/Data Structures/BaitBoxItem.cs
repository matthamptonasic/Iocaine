using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2
{
    public class BaitBoxItem
    {
        #region Constructors
        public BaitBoxItem()
        {

        }
        #endregion Constructors
        #region Private Members
        private string baitName;
        private byte baitLocation = 0;
        private string baitNameShort;
        private string equipString;
        private bool use;
        private byte priority;
        private ushort quantity;
        #endregion Private Members
        #region Public Properties
        public string BaitName
        {
            get
            {
                return baitName;
            }
            set
            {
                baitName = value;
                equipString = "/equip Ammo \"" + baitName + "\" " + baitLocation + " <me>";
            }
        }
        public byte BaitLocation
        {
            get
            {
                return baitLocation;
            }
            set
            {
                baitLocation = value;
                BaitName = baitName;
            }
        }
        public string BaitNameShort
        {
            get
            {
                return baitNameShort;
            }
            set
            {
                baitNameShort = value;
            }
        }
        public string EquipString
        {
            get
            {
                return equipString;
            }
            set
            {
                equipString = value;
            }
        }
        public bool Use
        {
            get
            {
                return use;
            }
            set
            {
                use = value;
            }
        }
        public byte Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
            }
        }
        public ushort Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                quantity = value;
            }
        }
        #endregion Public Properties
        #region Private Methods
        #endregion Private Methods
        #region Public Methods
        public override string ToString()
        {
            return baitNameShort + " (" + quantity + ")";
        }
        #endregion Public Methods
    }
}
