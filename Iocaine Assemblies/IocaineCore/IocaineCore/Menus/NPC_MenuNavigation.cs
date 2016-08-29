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
    public static class NPC_MenuNavigation
    {
        #region Enums
        #endregion Enums

        #region Private Members
        private static uint m_keytime_enter = 150;
        private static uint m_keytime_escape = 350;
        private static uint m_keytime_arrow = 100;
        private static uint m_keytime_general = 150;
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Public Methods
        #region General Functions
        public static bool ExitMenu(NPC_Menu iMenu)
        {
            if (iMenu.ParsingParams.EscapeExits)
            {
                IocaineFunctions.keyDown(Keys.Escape, m_keytime_escape);
                IocaineFunctions.delay(100);
                return true;
            }

            // Loop through each level of menu until we get to the top,
            // looking for exit text.
            while (true)
            {
                // Check if the current menu contains any of the exit all text.
                List<string> items = MemReads.Windows.Menus.TextStyle.get_items();
                foreach (string exitText in iMenu.ParsingParams.ExitAllText)
                {
                    if (items.Contains(exitText))
                    {
                        // The current menu contains the text to exit the menu.
                        // Navigate to that item and select it.
                        return SelectItem(exitText);
                    }
                }

                // If the Exit All text wasn't found, see if there is any Link Up text.
                foreach (string linkUpText in iMenu.ParsingParams.LinkUpNodeText)
                {
                    if (items.Contains(linkUpText))
                    {
                        if (SelectItem(linkUpText))
                        {
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                // No exit or link up text. Can't get out of menu.
                LoggingFunctions.Error("Could not get out of menu.");
                printCurrentMenu();
                return false;
            }

        }
        public static bool SelectItem(string iItemText)
        {
            if (!MemReads.Windows.Menus.TextStyle.is_open())
            {
                return false;
            }

            List<string> items = MemReads.Windows.Menus.TextStyle.get_items();
            if (!items.Contains(iItemText))
            {
                return false;
            }
            int destIdx = items.IndexOf(iItemText);

            int idx = MemReads.Windows.Menus.TextStyle.get_curr_index();
            if (idx == -1)
            {
                return false;
            }

            while (idx != destIdx)
            {
                Keys pushKey;
                if (idx < destIdx)
                {
                    // Most menus have 3 or less items in the visible window.
                    // If our current index is more than 2 less than the destination index, page down.
                    if ((destIdx - idx) > 2)
                    {
                        pushKey = Keys.Right;
                    }
                    else
                    {
                        pushKey = Keys.Down;
                    }
                }
                else
                {
                    if ((idx - destIdx) > 2)
                    {
                        pushKey = Keys.Left;
                    }
                    else
                    {
                        pushKey = Keys.Up;
                    }
                }
                IocaineFunctions.delay(100);
                IocaineFunctions.arrowKeyDown(pushKey, m_keytime_arrow);
                idx = MemReads.Windows.Menus.TextStyle.get_curr_index();
            }

            IocaineFunctions.delay(100);
            IocaineFunctions.keyDown(Keys.Enter, m_keytime_enter);
            return true;
        }
        #endregion General Functions
        #endregion Public Methods

        #region Private Methods
        private static void printCurrentMenu()
        {
            if (MemReads.Windows.Menus.TextStyle.is_open())
            {
                LoggingFunctions.Timestamp(MemReads.Windows.Menus.TextStyle.get_top_text());
                List<string> items = MemReads.Windows.Menus.TextStyle.get_items();
                foreach (string item in items)
                {
                    LoggingFunctions.Timestamp("  " + item);
                }
            }
        }
        #endregion Private Methods
    }
}