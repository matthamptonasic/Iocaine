using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Data.Structures
{
    #region Enums
    public enum ACTN_TYPE
    {
        Unknown = 0,
        Command = 1,
        Wait = 2,
        Target = 3,
        Follow = 4,
        Trade_Npc_Item = 5,
        Trade_Npc_Gil = 6,
        Trade_Pc_Item = 7,
        Trade_Pc_Gil = 8,
        Keystroke = 9,
        ArrowKeystroke = 10,
        Count = 11
    }
    #endregion Enums
    public abstract partial class Action
    {
        #region Private Members
        private bool isBlocking = false;
        private ACTN_TYPE type = ACTN_TYPE.Unknown;
        private List<Condition> m_orConditions;
        private List<Condition> m_andConditions;
        #endregion Private Members

        #region Public Properties
        public bool IsBlocking
        {
            get
            {
                return isBlocking;
            }
        }
        public ACTN_TYPE Type
        {
            get
            {
                return type;
            }
        }
        #endregion Public Properties

        #region Constructor
        public Action(bool iIsBlocking, ACTN_TYPE iType)
        {
            isBlocking = iIsBlocking;
            type = iType;
        }
        #endregion Constructor
        
        #region Public Methods
        public abstract void Show();
        public abstract new String SaveString();
        public bool Compare(Data.Structures.Action iAction)
        {
            if(iAction.type != this.type)
            {
                return false;
            }
            if(iAction.ToString() != this.ToString())
            {
                return false;
            }
            return true;
        }
        #endregion Public Methods
    }
}