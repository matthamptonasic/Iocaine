using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    #region Enums
    public enum CURRENCIES : ushort
    {
        GIL,
        CONQUEST_POINTS_SAN,
        CONQUEST_POINTS_BAS,
        CONQUEST_POINTS_WIN,
        VALOR_POINTS,
        GUILD_POINTS_FI,
        GUILD_POINTS_WW,
        GUILD_POINTS_SM,
        GUILD_POINTS_GS,
        GUILD_POINTS_CC,
        GUILD_POINTS_LT,
        GUILD_POINTS_BC,
        GUILD_POINTS_AL,
        GUILD_POINTS_CK,
        SPARKS,
        COPPER_VOUCHERS,
        IMPERIAL_STANDING,
        ASSAULT_POINTS_LS,
        ASSUALT_POINTS_MJ,
        ASSAULT_POINTS_LC,
        ASSAULT_POINTS_PE,
        ASSAULT_POINTS_IA,
        NYZUL_TOKENS,
        ZENI,
        THERION_ICHOR,
        ALLIED_NOTES,
        CRUOR,
        UNITY_ACCOLADES,
        LOGIN_POINTS,
        BAYLD,
        COALITION_IMPRIMATURS,
        KINETIC_UNITS,
        MWEYA_PLASM,
        ESCHA_SILT,
        ESCHA_BEADS,
        POTPOURRI,
        HALLMARKS,
        TOTAL_HALLMARKS,
        BADGES_OF_GALLARNTRY
    }
    #endregion Enums
    public class NPC_MenuParsingParameters
    {
        #region Private Members
        private List<string> m_finalActionTopText = new List<string>();
        private List<string> m_exitAllText = new List<string>();
        private List<string> m_linkUpNodeText = new List<string>();
        private List<string> m_linkLeftNodeText = new List<string>();
        private List<string> m_linkRightNodeText = new List<string>();
        private List<string> m_skipNodeText = new List<string>();
        private List<string> m_confirmAffirmativeText = new List<string>();
        private List<string> m_confirmNegativeText = new List<string>();
        private Dictionary<string, CURRENCIES> m_currencyStringToEnumMap = new Dictionary<string, CURRENCIES>();
        #endregion Private Members

        #region Public Properties
        public List<string> FinalActionTopText
        {
            get
            {
                return m_finalActionTopText;
            }
        }
        public List<string> ExitAllText
        {
            get
            {
                return m_exitAllText;
            }
        }
        public List<string> LinkUpNodeText
        {
            get
            {
                return m_linkUpNodeText;
            }
        }
        public List<string> LinkLeftNodeText
        {
            get
            {
                return m_linkLeftNodeText;
            }
        }
        public List<string> LinkRightNodeText
        {
            get
            {
                return m_linkRightNodeText;
            }
        }
        public List<string> SkipNodeText
        {
            get
            {
                return m_skipNodeText;
            }
        }
        public List<string> ConfirmAffirmativeText
        {
            get
            {
                return m_confirmAffirmativeText;
            }
        }
        public List<string> ConfirmNegativeText
        {
            get
            {
                return m_confirmNegativeText;
            }
        }
        public Dictionary<string, CURRENCIES> CurrencyMap
        {
            get
            {
                return m_currencyStringToEnumMap;
            }
        }
        #endregion Public Properties

        #region Constructors
        public NPC_MenuParsingParameters()
        {

        }
        #endregion Constructors

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
