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
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
