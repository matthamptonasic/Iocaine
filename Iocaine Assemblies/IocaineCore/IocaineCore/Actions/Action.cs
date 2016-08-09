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
    public abstract partial class Action : IExecutableAction
    {
        #region Private Members
        private bool isBlocking = false;
        private ACTN_TYPE aType = ACTN_TYPE.Unknown;
        private ConditionTree m_conditionsStatic;
        //private string m_conditionsStaticStr = "";
        private ConditionTree m_conditionsDynamic;
        //private string m_conditionsDynamicStr = "";
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
        public abstract bool Execute(string iTarget = "");
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
        public bool CanPerform()
        {
            if (!IsCapable())
            {
                return false;
            }
            if (m_conditionsDynamic == null)
            {
                return true;
            }
            return m_conditionsDynamic.Evaluate();
        }
        public bool IsCapable()
        {
            if (m_conditionsStatic == null)
            {
                return true;
            }
            return m_conditionsStatic.Evaluate();
        }
        #endregion Public Methods

        #region Private Methods
        protected void setConditions(ConditionTree iStaticConditions, ConditionTree iDynamicConditions)
        {
            m_conditionsStatic = iStaticConditions;
            m_conditionsDynamic = iDynamicConditions;
        }
        protected void setConditions(ConditionTree iDynamicConditions)
        {
            m_conditionsDynamic = iDynamicConditions;
        }
        //protected void setConditions(string iStaticConditions, string iDynamicConditions)
        //{
        //    m_conditionsStaticStr = iStaticConditions;
        //    m_conditionsDynamicStr = iDynamicConditions;
        //}
        #endregion Private Methods
    }
}
