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
        Unknown,
        Command,
        Wait,
        Target,
        Follow,
        Trade_Npc_Item,
        Trade_Npc_Gil,
        Trade_Pc_Item,
        Trade_Pc_Gil,
        Keystroke,
        Keystroke_Arrow,
        Cancel_Buff,
        Union,
        Count
    }
    #endregion Enums
    public abstract partial class Action
    {
        #region Private Members
        private bool isBlocking = false;
        private ACTN_TYPE aType = ACTN_TYPE.Unknown;
        private List<Condition> m_orConditions;
        protected List<Condition> m_andConditions;
        private string conditionExpression = "";
        #endregion Private Members

        #region Public Properties
        public bool IsBlocking
        {
            get
            {
                return isBlocking;
            }
        }
        public ACTN_TYPE AType
        {
            get
            {
                return aType;
            }
        }
        #endregion Public Properties

        #region Constructor
        public Action(bool iIsBlocking, ACTN_TYPE iType)
        {
            isBlocking = iIsBlocking;
            aType = iType;
        }
        #endregion Constructor

        #region Public Methods
        public abstract Boolean Execute(String iTarget = "");
        public abstract void Show();
        public abstract string SaveString();
        public bool Compare(Data.Structures.Action iAction)
        {
            if(iAction.aType != this.aType)
            {
                return false;
            }
            if(iAction.ToString() != this.ToString())
            {
                return false;
            }
            return true;
        }
        public bool ConditionsMet()
        {
            if ((m_andConditions != null) && (m_andConditions.Count > 0))
            {
                bool met = true;
                foreach (Condition cndn in m_andConditions)
                {
                    met &= cndn.IsSatisfied();
                }
                return met;
            }
            else if ((m_orConditions != null) && (m_orConditions.Count > 0))
            {
                bool met = false;
                foreach (Condition cndn in m_andConditions)
                {
                    met |= cndn.IsSatisfied();
                }
                return met;
            }
            return false;
        }
        #endregion Public Methods
    }
}
