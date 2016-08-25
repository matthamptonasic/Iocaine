using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Data.Structures;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;

namespace Iocaine2.Tools
{
    public static class MenuNavigation
    {
        #region Enums
        public enum MENU_TYPE
        {
            NONE = 0,
            MAIN_MENU_1 = 1,
            MAIN_MENU_2 = 2,
            COMMAND = 3,
            UNKNOWN = 4
        }
        #endregion Enums
        #region Default Key Times
        #region Private Members
        private static UInt32 enterDownTime = 150;
        private static UInt32 escapeDownTime = 350;
        private static UInt32 arrowDownTime = 100;
        private static UInt32 generalDownTime = 150;
        #endregion Private Members
        #region Public Properties
        public static UInt32 KeyDownTimeEnter
        {
            get
            {
                return enterDownTime;
            }
        }
        public static UInt32 KeyDownTimeEscape
        {
            get
            {
                return escapeDownTime;
            }
        }
        public static UInt32 KeyDownTimeArrow
        {
            get
            {
                return arrowDownTime;
            }
        }
        public static UInt32 KeyDownTimeGeneral
        {
            get
            {
                return generalDownTime;
            }
        }
        #endregion Public Properties
        #endregion Default Key Times
        #region Other Default Values
        private static Int32 closeCheckDefaultLoops = 4;
        #endregion Other Default Values
        #region Menu Text Arrays
        public static String[] MainMenu_1_Help_Text = new String[12] { 
            "View status.",
            "Equip weapons and armor.",
            "View spell and song lists.",
            "View current inventory.",
            "View the list of synthesis-related options.",
            "View job abilities, weapon skills, etc.",
            "Organize party settings.",
            "Trade items or money with current target.",
            "Search for other players according to various conditions.",
            "Set up and equip linkshell items.",
            "View the balance of power for each region.",
            "View map of current area."
            //"Open the Mog House menu."
        };
        public static String[] MainMenu_2_Help_Text = new String[12] {
            "View list of current missions.",
            "View list of current quests.",
            "View list of key items. Key items cannot be traded or dropped.",
            "View contents of Mog House and Mog Case, and other such storage.",
            "Select merchandise from your inventory to place on sale in your bazaar.",
            "Edit equipment sets and the user-defined macros in your macro palettes.",
            "Change the game's options.",
            "Search for solutions to past problems or report new instances of trouble.",
            "Display current time in Vana'diel and on Earth. Also toggles the on-screen clock and weekday display.",
            "Display the Friend List and the Emote List.",
            "Log out from FINAL FANTASY XI and PlayOnline.",
            "Quit FINAL FANTASY XI and return to title screen."
        };
        public static String[] MainMenu_1_Left_Text = new String[12] {
            "Status",
            "Equipment",
            "Magic List",
            "Items",
            "Sythesis",
            "Abilities",
            "Party",
            "Trade",
            "Search",
            "Linkshell",
            "Region Info",
            //"Mog House"
            "Map"
        };
        public static String[] MainMenu_2_Left_Text = new String[12] {
            "Missions",
            "Quests",
            "Key Items",
            "View House",
            "Set Bazaar",
            "MacroPalette",
            "Config",
            "Help Desk",
            "Current Time",
            "Communication",
            "Shut Down",
            "Log Out"
        };
        public static String[] CommandMenu_Items = new String[6] {
            "Chat",
            "Magic",
            "Abilities",
            "Items",
            "Trade",
            "Check"
        };
        public static String[] CommandMenu_Help_Text = new String[6] {
            "Chat in \"tell\" mode. Press right directional button to change chat modes.",
            "Use magic. Press right directional button to list spells and songs by type.",
            "Display abilities and skills listed by type.",
            "Use an item.",
            "Trade with target.",
            "Examine target's equipment."
        };
        public static String[] CommandMenu_Left_Text = new String[6] {
            "Commands",
            "Commands",
            "Commands",
            "Commands",
            "Commands",
            "Commands"
        };
        public static String[] Inventory_Left_Text = new String[8] {
            "None",
            "Items",
            "Mog Sack",
            "Satchel",
            "Mog Case",
            "Mog Safe",
            "Storage",
            "Locker"
        };
        #endregion Menu Text Arrays
        #region Menu Navigation Functions
        #region Actual Navigation
        public static bool GotoMenuItem(String menuLeftText, bool hitEnter)
        {
            MENU_TYPE whichMenu = MENU_TYPE.NONE;
            byte menuIndex = 12;
            if (MainMenu_1_Left_Text.Contains(menuLeftText))
            {
                whichMenu = MENU_TYPE.MAIN_MENU_1;
            }
            else if (MainMenu_2_Left_Text.Contains(menuLeftText))
            {
                whichMenu = MENU_TYPE.MAIN_MENU_2;
            }
            else
            {
                whichMenu = 0;
                return false;
            }
            for (byte ii = 0; ii < MainMenu_1_Left_Text.Length; ii++)
            {
                if ((whichMenu == MENU_TYPE.MAIN_MENU_1) && (MainMenu_1_Left_Text[ii] == menuLeftText))
                {
                    menuIndex = ii;
                    break;
                }
                else if ((whichMenu == MENU_TYPE.MAIN_MENU_2) && (MainMenu_2_Left_Text[ii] == menuLeftText))
                {
                    menuIndex = ii;
                }
            }
            if (menuIndex >= MainMenu_1_Left_Text.Length)
            {
                return false;
            }
            else
            {
                return GotoMenuItem(whichMenu, menuIndex, hitEnter);
            }
        }
        public static bool GotoMenuItem(MENU_TYPE type, byte itemIndex, bool hitEnter)
        {
            //Check to see if any window is open and if so, if its main menu 1 or 2
            MENU_TYPE currentMenu = GetCurrentOpenMenu();

            //If the window is something other than what we want, do a close check first.
            if ((currentMenu != MENU_TYPE.NONE) && (currentMenu != type))
            {
                CloseCheck();
            }

            //======== Begin Menu Navigation ========
            switch (type)
            {
                case MENU_TYPE.NONE:
                    return true;
                case MENU_TYPE.UNKNOWN:
                    return false;
                case MENU_TYPE.MAIN_MENU_1:
                    IocaineFunctions.keyDown(System.Windows.Forms.Keys.Subtract, 40);
                    IocaineFunctions.delay(400);
                    break;
                case MENU_TYPE.MAIN_MENU_2:
                    IocaineFunctions.keyDown(System.Windows.Forms.Keys.Subtract, 40);
                    IocaineFunctions.delay(500);
                    IocaineFunctions.keyDown(System.Windows.Forms.Keys.Subtract, 40);
                    IocaineFunctions.delay(400);
                    break;
                case MENU_TYPE.COMMAND:
                    IocaineFunctions.keyDown(System.Windows.Forms.Keys.F1, 50);
                    IocaineFunctions.delay(500);
                    IocaineFunctions.keyDown(System.Windows.Forms.Keys.Enter, 50);
                    IocaineFunctions.delay(400);
                    break;
                default:
                    return false;
            }

            //Check again to make sure we now have the correct menu open
            currentMenu = GetCurrentOpenMenu();
            if (currentMenu != type)
            {
                return false;
            }

            //Now that we have the correct menu open, find the current index
            byte currentIndex = getCurrentMenuIndex(currentMenu);
            byte maxIndex = getMaxIndex(currentMenu);

            //Now find shortest path to get to that command
            bool goDown = true;
            bool foundItem = false;
            if (currentIndex != itemIndex)
            {
                if (currentIndex > itemIndex)
                {
                    if ((currentIndex - itemIndex) > (maxIndex / 2))
                    {
                        //We're currently towards the bottom and the destination is towards the top
                        goDown = true;
                    }
                    else
                    {
                        //We're currently close to the destination which is above us
                        goDown = false;
                    }
                }
                else
                {
                    if ((itemIndex - currentIndex) > (maxIndex / 2))
                    {
                        //We're currently towards the top and the destination is towards the bottom
                        goDown = false;
                    }
                    else
                    {
                        //We're currently close to the destination which is below us
                        goDown = true;
                    }
                }
                byte counter = 0;
                while (!foundItem && (counter < maxIndex))
                {
                    if (goDown)
                    {
                        IocaineFunctions.arrowKeyDown(System.Windows.Forms.Keys.Down, 100);
                    }
                    else
                    {
                        IocaineFunctions.arrowKeyDown(System.Windows.Forms.Keys.Up, 100);
                    }
                    IocaineFunctions.delay(400);
                    if (itemIndex == getCurrentMenuIndex(type))
                    {
                        foundItem = true;
                    }
                }
            }
            else
            {
                foundItem = true;
            }

            if (!foundItem)
            {
                return false;
            }

            if (hitEnter)
            {
                IocaineFunctions.keyDown(System.Windows.Forms.Keys.Enter, 75);
                IocaineFunctions.delay(400);
            }
            return true;
        }
        #endregion Actual Navigation
        #region Information Functions
        public static MENU_TYPE GetCurrentOpenMenu()
        {
            String currentLeftText = currentLeftText = MemReads.Windows.BannerText.get_top_left_text();
            if (currentLeftText == "N/A")
            {
                return MENU_TYPE.NONE;
            }

            //Check if the window that's open is Main Menu 1 or Main Menu 2
            for (byte ii = 0; ii < MainMenu_1_Left_Text.Length; ii++)
            {
                if (MainMenu_1_Left_Text[ii] == currentLeftText)
                {
                    return MENU_TYPE.MAIN_MENU_1;
                }
                else if (MainMenu_2_Left_Text[ii] == currentLeftText)
                {
                    return MENU_TYPE.MAIN_MENU_2;
                }
            }

            //Check if the window that's open is the Command Menu
            if (currentLeftText == CommandMenu_Left_Text[0])
            {
                return MENU_TYPE.COMMAND;
            }
            return MENU_TYPE.UNKNOWN;
        }
        private static byte getCurrentMenuIndex(MENU_TYPE type)
        {
            String currentLeftText = MemReads.Windows.BannerText.get_top_left_text();
            String currentHelpText = MemReads.Windows.BannerText.get_help_text();
            byte index = 0;
            switch (type)
            {
                case MENU_TYPE.MAIN_MENU_1:
                    index = (byte)MainMenu_1_Left_Text.Length;
                    for (byte ii = 0; ii < index; ii++)
                    {
                        if (MainMenu_1_Left_Text[ii] == currentLeftText)
                        {
                            return ii;
                        }
                    }
                    return index;
                case MENU_TYPE.MAIN_MENU_2:
                    index = (byte)MainMenu_2_Left_Text.Length;
                    for (byte ii = 0; ii < index; ii++)
                    {
                        if (MainMenu_2_Left_Text[ii] == currentLeftText)
                        {
                            return ii;
                        }
                    }
                    return index;
                case MENU_TYPE.COMMAND:
                    index = (byte)CommandMenu_Help_Text.Length;
                    for (byte ii = 0; ii < index; ii++)
                    {
                        if (CommandMenu_Help_Text[ii] == currentHelpText)
                        {
                            return ii;
                        }
                    }
                    return index;
                default:
                    return 100;
            }
        }
        private static byte getMaxIndex(MENU_TYPE type)
        {
            switch (type)
            {
                case MENU_TYPE.COMMAND:
                    return (byte)CommandMenu_Help_Text.Length;
                case MENU_TYPE.MAIN_MENU_1:
                    return (byte)MainMenu_1_Help_Text.Length;
                case MENU_TYPE.MAIN_MENU_2:
                    return (byte)MainMenu_2_Help_Text.Length;
                default:
                    return 0;
            }
        }
        public static FFXIEnums.INVENTORY_MENU GetOpenInventoryMenu(out bool oDoubleWindow)
        {
            String leftText = MemReads.Windows.BannerText.get_top_left_text();
            if (leftText == Inventory_Left_Text[(int)FFXIEnums.INVENTORY_MENU.SACK])
            {
                oDoubleWindow = true;
                return FFXIEnums.INVENTORY_MENU.SACK;
            }
            else if(leftText == Inventory_Left_Text[(int)FFXIEnums.INVENTORY_MENU.SATCHEL])
            {
                oDoubleWindow = true;
                return FFXIEnums.INVENTORY_MENU.SATCHEL;
            }
            else if (leftText == Inventory_Left_Text[(int)FFXIEnums.INVENTORY_MENU.CASE])
            {
                oDoubleWindow = true;
                return FFXIEnums.INVENTORY_MENU.CASE;
            }
            else if(leftText == Inventory_Left_Text[(int)FFXIEnums.INVENTORY_MENU.SAFE])
            {
                oDoubleWindow = MemReads.Windows.Items.get_sec_wnd_open();
                return FFXIEnums.INVENTORY_MENU.SAFE;
            }
            else if(leftText == Inventory_Left_Text[(int)FFXIEnums.INVENTORY_MENU.STORAGE])
            {
                oDoubleWindow = MemReads.Windows.Items.get_sec_wnd_open();
                return FFXIEnums.INVENTORY_MENU.STORAGE;
            }
            else if (leftText == Inventory_Left_Text[(int)FFXIEnums.INVENTORY_MENU.LOCKER])
            {
                oDoubleWindow = MemReads.Windows.Items.get_sec_wnd_open();
                return FFXIEnums.INVENTORY_MENU.LOCKER;
            }
            else if (leftText == Inventory_Left_Text[(int)FFXIEnums.INVENTORY_MENU.BAG])
            {
                oDoubleWindow = MemReads.Windows.Items.get_sec_wnd_open();
                return FFXIEnums.INVENTORY_MENU.BAG;
            }
            else
            {
                oDoubleWindow = false;
                return FFXIEnums.INVENTORY_MENU.NONE;
            }
        }
        public static FFXIEnums.INVENTORY_MENU GetOpenInventoryMenu()
        {
            bool secWndOpen = false;
            return GetOpenInventoryMenu(out secWndOpen);
        }
        #endregion Information Functions
        #endregion Menu Navigation Functions
        #region Window Navigation Functions
        #region General Functions
        public static void CloseCheck(int nbLoops)
        {
            for (int ii = 0; ii < nbLoops; ii++)
            {
                IocaineFunctions.keyDown(System.Windows.Forms.Keys.Escape, KeyDownTimeEscape);
                IocaineFunctions.delay(200);
            }
        }
        public static void CloseCheck()
        {
            CloseCheck(closeCheckDefaultLoops);
        }
        public static void HitOK()
        {
            IocaineFunctions.arrowKeyDown(Keys.Right, 2000);
            IocaineFunctions.delay(100);
            IocaineFunctions.keyDown(Keys.Enter, KeyDownTimeEnter);
        }
        public static bool OpenBag()
        {
            bool doubleWindow = false;
            FFXIEnums.INVENTORY_MENU menu = GetOpenInventoryMenu(out doubleWindow);
            if ((menu != FFXIEnums.INVENTORY_MENU.BAG) || (doubleWindow == true))
            {
                String leftText = MemReads.Windows.BannerText.get_top_left_text();
                if ((leftText != "") && (leftText != "N/A"))
                {
                    CloseCheck();
                }
                IocaineFunctions.twoKeys(System.Windows.Forms.Keys.LControlKey, System.Windows.Forms.Keys.I, 250);
                IocaineFunctions.delay(500);
            }
            if (MemReads.Windows.BannerText.get_top_left_text() == Inventory_Left_Text[(int)FFXIEnums.INVENTORY_MENU.BAG])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool OpenSatchel()
        {
            bool doubleWindow = false;
            String leftText = "";
            FFXIEnums.INVENTORY_MENU menu = GetOpenInventoryMenu(out doubleWindow);
            if ((menu == FFXIEnums.INVENTORY_MENU.BAG) && doubleWindow)
            {
                //A double window is open, but we don't know which one.
                IocaineFunctions.arrowKeyDown(Keys.Left);
                menu = GetOpenInventoryMenu(out doubleWindow);
                IocaineFunctions.delay(200);
            }
            if (((menu != FFXIEnums.INVENTORY_MENU.BAG) && (menu != FFXIEnums.INVENTORY_MENU.SATCHEL)) || (doubleWindow == false))
            {
                leftText = MemReads.Windows.BannerText.get_top_left_text();
                if ((leftText != "") && (leftText != "N/A"))
                {
                    CloseCheck();
                }
                IocaineFunctions.keys("/satchel");
                IocaineFunctions.delay(250);

            }
            leftText = MemReads.Windows.BannerText.get_top_left_text();
            if(leftText == Inventory_Left_Text[(int)FFXIEnums.INVENTORY_MENU.SATCHEL])
            {
                return true;
            }
            if ((leftText == Inventory_Left_Text[(int)FFXIEnums.INVENTORY_MENU.BAG]) && MemReads.Windows.Items.get_sec_wnd_open())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool OpenSack()
        {
            bool doubleWindow = false;
            String leftText = "";
            FFXIEnums.INVENTORY_MENU menu = GetOpenInventoryMenu(out doubleWindow);
            if ((menu == FFXIEnums.INVENTORY_MENU.BAG) && doubleWindow)
            {
                //A double window is open, but we don't know which one.
                IocaineFunctions.arrowKeyDown(Keys.Left);
                menu = GetOpenInventoryMenu(out doubleWindow);
                IocaineFunctions.delay(200);
            }
            if (((menu != FFXIEnums.INVENTORY_MENU.BAG) && (menu != FFXIEnums.INVENTORY_MENU.SACK)) || (doubleWindow == false))
            {
                leftText = MemReads.Windows.BannerText.get_top_left_text();
                if ((leftText != "") && (leftText != "N/A"))
                {
                    CloseCheck();
                }
                IocaineFunctions.keys("/sack");
                IocaineFunctions.delay(250);

            }
            leftText = MemReads.Windows.BannerText.get_top_left_text();
            if(leftText == Inventory_Left_Text[(int)FFXIEnums.INVENTORY_MENU.SACK])
            {
                return true;
            }
            if ((leftText == Inventory_Left_Text[(int)FFXIEnums.INVENTORY_MENU.BAG]) && MemReads.Windows.Items.get_sec_wnd_open())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool OpenCase()
        {
            bool doubleWindow = false;
            String leftText = "";
            FFXIEnums.INVENTORY_MENU menu = GetOpenInventoryMenu(out doubleWindow);
            if ((menu == FFXIEnums.INVENTORY_MENU.BAG) && doubleWindow)
            {
                //A double window is open, but we don't know which one.
                IocaineFunctions.arrowKeyDown(Keys.Left);
                menu = GetOpenInventoryMenu(out doubleWindow);
                IocaineFunctions.delay(200);
            }
            if (((menu != FFXIEnums.INVENTORY_MENU.BAG) && (menu != FFXIEnums.INVENTORY_MENU.CASE)) || (doubleWindow == false))
            {
                leftText = MemReads.Windows.BannerText.get_top_left_text();
                if ((leftText != "") && (leftText != "N/A"))
                {
                    CloseCheck();
                }
                IocaineFunctions.keys("/case");
                IocaineFunctions.delay(250);

            }
            leftText = MemReads.Windows.BannerText.get_top_left_text();
            if (leftText == Inventory_Left_Text[(int)FFXIEnums.INVENTORY_MENU.CASE])
            {
                return true;
            }
            if ((leftText == Inventory_Left_Text[(int)FFXIEnums.INVENTORY_MENU.BAG]) && MemReads.Windows.Items.get_sec_wnd_open())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion General Functions
        #region Trading
        public static bool SetNpcTradeItems(List<UInt16> iItemIds, List<Byte> iItemQuan)
        {
            List<String> itemNames = new List<string>();
            for (int ii = 0; ii < iItemIds.Count; ii++)
            {
                String itemName = Things.GetNameFromId(iItemIds[ii]);
                if (itemName == "")
                {
                    MessageBox.Show("Could not get the item name for ID: " + iItemIds[ii]);
                    return false;
                }
                itemNames.Add(itemName);
            }
            return SetNpcTradeItems(itemNames, iItemIds, iItemQuan);

        }
        public static bool SetNpcTradeItems(List<String> iItemNames, List<UInt16> iItemIds, List<Byte> iItemQuan)
        {
            List<byte> usedIndexList = new List<byte>();
            byte index = 0;
            for (int ii = 0; ii < iItemIds.Count; ii++)
            {
                if (iItemQuan[ii] > 0)
                {
                    index = MemReads.Self.Inventory.get_bag_index(iItemIds[ii], iItemQuan[ii]);
                    while (usedIndexList.Contains(index) && (index != 0))
                    {
                        index = MemReads.Self.Inventory.get_bag_index(iItemIds[ii], iItemQuan[ii], (byte)(index + 1));
                    }
                    if (index != 0)
                    {
                        usedIndexList.Add(index);
                    }
                    else
                    {
                        LoggingFunctions.Error("Could not find item " + iItemNames[ii] + " in inventory.");
                        MessageBox.Show("[ERROR] Could not find item " + iItemNames[ii] + " in inventory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    LoggingFunctions.Debug("Item " + (ii + 1).ToString() + ": " + iItemNames[ii] + ": " + iItemIds[ii] + " : " + index, LoggingFunctions.DBG_SCOPE.INTERACTION);
                    MemReads.Windows.Trading.NPC.set_item((byte)ii, iItemIds[ii], iItemQuan[ii], index);
                }
            }
            LoggingFunctions.Debug("Setting ingredients...", LoggingFunctions.DBG_SCOPE.INTERACTION);
            if (LoggingFunctions.Dbg > 0)
            {
                for (byte ii = 0; ii < 1; ii++)
                {
                    ushort id = 0;
                    byte quan = 0;
                    byte idx = 0;
                    MemReads.Windows.Trading.NPC.get_item(ii, ref id, ref quan, ref idx);
                    LoggingFunctions.Debug("Actually read ID: " + id + ", quan: " + quan + ", index: " + idx, LoggingFunctions.DBG_SCOPE.INTERACTION);
                }
            }
            return true;
        }
        public static bool SetNpcTradeGil(UInt32 iGilQuan)
        {
            try
            {
                MemReads.Windows.Trading.NPC.set_gil(iGilQuan);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In SetNpcTradeGil: " + e.ToString());
                return false;
            }
            return true;
        }
        public static bool SetPlayerTradeItems(List<String> iItemNames, List<UInt16> iItemIds, List<Byte> iItemQuan)
        {
            //stub
            return true;
        }
        public static bool SetPlayerTradeGil(UInt32 iGilQuan)
        {
            //stub
            return true;
        }
        #endregion Trading
        #region Crafting
        public static bool SetCraftItems(Recipe iRecipe)
        {
            List<String> itemNames = new List<string>();
            List<UInt16> itemIds = new List<ushort>();
            List<Byte> itemQuan = new List<byte>();

            iRecipe.createUnstackedLists(ref itemNames, ref itemIds, ref itemQuan);
            return SetCraftItems(itemNames, itemIds, itemQuan);
        }
        public static bool SetCraftItems(List<UInt16> iItemIds, List<Byte> iItemQuan)
        {
            List<String> itemNames = new List<string>();
            for (int ii = 0; ii < iItemIds.Count; ii++)
            {
                String itemName = Things.GetNameFromId(iItemIds[ii]);
                if (itemName == "")
                {
                    MessageBox.Show("Could not get the item name for ID: " + iItemIds[ii]);
                    return false;
                }
                itemNames.Add(itemName);
            }
            return SetCraftItems(itemNames, iItemIds, iItemQuan);
        }
        public static bool SetCraftItems(List<String> iItemNames, List<UInt16> iItemIds, List<Byte> iItemQuan)
        {
            List<byte> usedIndexList = new List<byte>();
            byte index = MemReads.Self.Inventory.get_bag_index(iItemIds[0], iItemQuan[0]);
            usedIndexList.Add(index);
            if (index == 0)
            {
                LoggingFunctions.Error("Could not find item " + iItemNames[0] + " in inventory.");
                MessageBox.Show("[ERROR] Could not find item " + iItemNames[0] + " in inventory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            LoggingFunctions.Debug("Ingredient 1: " + iItemNames[0] + ": " + iItemIds[0] + " : " + index, LoggingFunctions.DBG_SCOPE.CRAFTER);
            MemReads.Windows.Crafting.set_item(0, iItemIds[0], iItemQuan[0], index);
            for (int ii = 1; ii < iItemIds.Count; ii++)
            {
                if (iItemQuan[ii] != 0)
                {
                    index = MemReads.Self.Inventory.get_bag_index(iItemIds[ii], iItemQuan[ii]);
                    while (usedIndexList.Contains(index) && (index != 0))
                    {
                        index = MemReads.Self.Inventory.get_bag_index(iItemIds[ii], iItemQuan[ii], (byte)(index + 1));
                    }
                    if (index != 0)
                    {
                        usedIndexList.Add(index);
                    }
                    else
                    {
                        LoggingFunctions.Error("Could not find item " + iItemNames[ii] + " in inventory.");
                        MessageBox.Show("[ERROR] Could not find item " + iItemNames[ii] + " in inventory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    LoggingFunctions.Debug("Ingredient " + (ii + 1).ToString() + ": " + iItemNames[ii] + ": " + iItemIds[ii] + " : " + index, LoggingFunctions.DBG_SCOPE.CRAFTER);
                    MemReads.Windows.Crafting.set_item(1, iItemIds[ii], iItemQuan[ii], index);
                }
            }
            LoggingFunctions.Debug("Setting ingredients...", LoggingFunctions.DBG_SCOPE.CRAFTER);
            if (LoggingFunctions.Dbg > 0)
            {
                for (byte ii = 0; ii < iItemIds.Count; ii++)
                {
                    ushort id = 0;
                    byte quan = 0;
                    byte idx = 0;
                    MemReads.Windows.Crafting.get_item(ii, ref id, ref quan, ref idx);
                    LoggingFunctions.Debug("Actuall read ID: " + id + ", quan: " + quan + ", index: " + idx, LoggingFunctions.DBG_SCOPE.CRAFTER);
                }
            }
            return true;
        }
        #endregion Crafting
        #endregion Window Navigation Functions
    }
}