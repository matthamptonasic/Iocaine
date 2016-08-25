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
        private NPCs.NPC_TYPE m_npcType;
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Constructors
        public NPC_Menu(NPCs.NPC_TYPE iNpcType, string iTopText)
        {
            m_npcType = iNpcType;
            m_topText = iTopText;
        }
        #endregion Constructors

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
