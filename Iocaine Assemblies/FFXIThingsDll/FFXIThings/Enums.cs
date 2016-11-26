using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Things
    {
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
    }
}