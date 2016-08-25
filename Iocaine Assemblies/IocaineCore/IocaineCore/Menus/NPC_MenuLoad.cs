using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public static class NPC_MenuLoad
    {
        #region Private Members
        private static bool m_initDone = false;
        private static List<NPC_Menu> m_menuList;
        #endregion Private Members

        #region Public Properties
        public static bool InitDone
        {
            get
            {
                return m_initDone;
            }
        }
        #endregion Public Properties

        #region Inits
        public static void Init_Iocaine()
        {
            if (!m_initDone)
            {
                loadMenus();
                m_initDone = true;
            }
        }
        #endregion Inits

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        private static void loadMenus()
        {
            m_menuList = new List<NPC_Menu>();

            #region RoE Menus
            NPC_Menu topMenu = new NPC_Menu(NPCs.NPC_TYPE.ROE, @"Exchange for what? (Sparks: ${SPARKS})", iIsTopMenu: true);

            // Set the parsing parameters.
            NPC_MenuParsingParameters param = new NPC_MenuParsingParameters();
            param.FinalActionTopText.Add(@"Make the exchange?");
            param.FinalActionTopText.Add(@"Recieve how many? (${SPARKS} sparks)");
            param.FinalActionTopText.Add(@"Exchange how many points? (On hand: ${COPPER_VOUCHERS})");

            param.LinkUpNodeText.Add("None.");
            param.LinkUpNodeText.Add("Nothing for the nonce.");

            param.LinkRightNodeText.Add("Next page.");

            param.LinkLeftNodeText.Add("Previous page.");

            param.CurrencyMap.Add("Sparks", CURRENCIES.SPARKS);
            param.CurrencyMap.Add("On hand", CURRENCIES.COPPER_VOUCHERS);

            param.SkipNodeText.Add("Wise words on Records of Eminence.");

            // TBD - there are more than this.
            param.ConfirmAffirmativeText.Add("Yes.");

            param.ConfirmNegativeText.Add("No.");

            topMenu.ParsingParams = param;
            #endregion RoE Menus
        }
        #endregion Private Methods
    }
}
