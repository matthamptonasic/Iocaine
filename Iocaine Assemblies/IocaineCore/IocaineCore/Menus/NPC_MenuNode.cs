using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public class NPC_MenuNode
    {
        #region Enums
        #endregion Enums

        #region Private Members
        private string m_text;
        private bool m_finalAction = false;
        private bool m_confirmationPositive = false;
        private bool m_confirmationNegative = false;
        private bool m_pagedListItem = false;
        private bool m_exitsAll = false;
        private bool m_movesUp = false;
        private bool m_movesDown = false;
        private bool m_movesLeft = false;
        private bool m_movesRight = false;
        private bool m_jumps = false;
        private bool m_isItem = false;
        private bool m_isKeyItem = false;
        private NPC_Menu m_owner;
        private byte m_index = 0;
        private NPC_Menu m_linkTo;
        #endregion Private Members

        #region Public Properties
        public string Text
        {
            get
            {
                return m_text;
            }
        }
        public bool FinalAction
        {
            get
            {
                return m_finalAction;
            }
        }
        public bool ConfirmationPositive
        {
            get
            {
                return m_confirmationPositive;
            }
        }
        public bool ConfirmationNegative
        {
            get
            {
                return m_confirmationNegative;
            }
        }
        public bool PagedListItem
        {
            get
            {
                return m_pagedListItem;
            }
        }
        public bool ExitsAll
        {
            get
            {
                return m_exitsAll;
            }
        }
        public bool MovesUp
        {
            get
            {
                return m_movesUp;
            }
        }
        public bool MovesDown
        {
            get
            {
                return m_movesDown;
            }
        }
        public bool MovesLeft
        {
            get
            {
                return m_movesLeft;
            }
        }
        public bool MovesRight
        {
            get
            {
                return m_movesRight;
            }
        }
        public bool Jumps
        {
            get
            {
                return m_jumps;
            }
        }
        public bool IsItem
        {
            get
            {
                return m_isItem;
            }
        }
        public bool IsKeyItem
        {
            get
            {
                return m_isKeyItem;
            }
        }
        public NPC_Menu Owner
        {
            get
            {
                return m_owner;
            }
        }
        public byte Index
        {
            get
            {
                return m_index;
            }
            set
            {
                m_index = value;
            }
        }
        public NPC_Menu LinkTo
        {
            get
            {
                return m_linkTo;
            }
            set
            {
                m_linkTo = value;
            }
        }
        #endregion Public Properties

        #region Constructor
        public NPC_MenuNode(NPC_Menu iOwner, string iText, byte iIndex,
                            bool iFinalAction, bool iConfirmationPositive,
                            bool iConfirmationNegative, bool iPagedListItem,
                            bool iExitsAll, bool iMovesUp, bool iMovesDown,
                            bool iMovesLeft, bool iMovesRight, bool iJumps,
                            bool iIsItem, bool iIsKeyItem, NPC_Menu iLinkTo = null)
        {
            m_owner = iOwner;
            m_text = iText;
            m_index = iIndex;
            m_finalAction = iFinalAction;
            m_confirmationPositive = iConfirmationPositive;
            m_confirmationNegative = iConfirmationNegative;
            m_pagedListItem = iPagedListItem;
            m_exitsAll = iExitsAll;
            m_movesUp = iMovesUp;
            m_movesDown = iMovesDown;
            m_movesLeft = iMovesLeft;
            m_movesRight = iMovesRight;
            m_jumps = iJumps;
            m_isItem = iIsItem;
            m_isKeyItem = iIsKeyItem;
            if (iLinkTo != null)
            {
                m_linkTo = iLinkTo;
            }
        }

        #endregion Constructor

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
