using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2.Inventory
{
    public class Item
    {
        #region Constructors
        public Item(String iName, ushort iID, ITEM_TYPE iType)
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
        #region Enums
        public enum ITEM_TYPE : byte
        {
            MISC_TEMP_ITEMS = 0x1,  //Mostly things like fanatics drink, or dusty items, but also distilled water
            MISC_TESTIMONY = 0x2,   //Also various orbs
            FISH = 0x3,
            WEAPON = 0x4,
            ARMOR = 0x5,
            LINKPEARL = 0x6,
            MISC_KEY = 0x7, //Also sickle, hatchet, orbs, cards, ammo pouches, some fish
            CRYSTALS = 0x8,
            FURNISHING = 0xA,
            SEEDS = 0xB,
            FLOWER_POT = 0xC,
            MANNEQUIN = 0xE,
            CODEX = 0xF,    //Various book pages
            CHOCO_ITEMS_A = 0x10,
            CHOCO_ITEMS_B = 0x11,
            SOUL_PLATE = 0x12,
            SOUL_REFLECTOR = 0x13,
            ASSAULT_ITEMS = 0x14,
            MOG_MARBLE = 0x15,
            UNKNOWN = 0xFF
        }
        #endregion Enums
        #region Member Variables
        #region Private Members
        private String name;
        private ushort itemID;
        private ITEM_TYPE type;
        #endregion Private Members
        #region Public Members
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
        public ITEM_TYPE Type
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
        #endregion Public Members
        #endregion Member Variables
        #region Methods
        #endregion Methods
    }
}
