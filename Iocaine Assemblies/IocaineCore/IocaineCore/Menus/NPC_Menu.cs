using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public class NPC_Menu
    {
        #region Enums
        #endregion Enums

        #region Private Members
        private string m_topText;
        private bool m_isTop;
        private NPCs.NPC_TYPE m_npcType;
        private List<NPCs.NPC_INFO> m_npcInfo;
        private bool m_npcsAreUnique = true;
        private List<NPC_MenuNode> m_nodes;
        private NPC_MenuParsingParameters m_parsingParams;
        #endregion Private Members

        #region Public Properties
        public List<NPCs.NPC_INFO> NpcInfoList
        {
            get
            {
                return m_npcInfo;
            }
            set
            {
                m_npcInfo = value;
            }
        }
        /// <summary>
        /// Specifies whether the menu for all NPCs of this type are unique.
        /// Unique example is the task delegator NPCs in Adoulin, each menu is different.
        /// Non-Unique example is the RoE NPCs, each menu is the same as the rest.
        /// </summary>
        public bool NpcsAreUnique
        {
            get
            {
                return m_npcsAreUnique;
            }
            set
            {
                m_npcsAreUnique = value;
            }
        }
        public NPC_MenuParsingParameters ParsingParams
        {
            get
            {
                return m_parsingParams;
            }
            set
            {
                m_parsingParams = value;
            }
        }
        #endregion Public Properties

        #region Constructors
        public NPC_Menu(NPCs.NPC_TYPE iNpcType, string iTopText, bool iIsTopMenu = false)
        {
            m_npcType = iNpcType;
            m_topText = iTopText;
            m_isTop = iIsTopMenu;
        }
        #endregion Constructors

        #region Public Methods
        #region Get/Add
        public void Add(NPC_MenuNode iNode)
        {
            if (iNode != null)
            {
                m_nodes.Add(iNode);
            }
        }
        public NPC_MenuNode GetNode(string iText, bool iMatchPartial = true)
        {
            foreach (NPC_MenuNode node in m_nodes)
            {
                if (iMatchPartial)
                {
                    if (node.Text.Contains(iText))
                    {
                        return node;
                    }
                }
                else
                {
                    if (node.Text == iText)
                    {
                        return node;
                    }
                }
            }
            return null;
        }
        #endregion Get/Add
        #region Menu Parsing
        public static bool ParseMenu(NPC_Menu iMenu, string iNPC = "")
        {
            if (!ChangeMonitor.LoggedIn)
            {
                return false;
            }
            // If the iNPC is empty, find the one in this zone. Exit if none.

            // Check if the NPC is nearby.

            // Check if we're already in a menu. If so, try to exit out.

            // Target the nearby NPC.

            // Enter the menu at the top.

            // Parse it depth-first.

            return true;
        }
        #endregion Menu Parsing
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
