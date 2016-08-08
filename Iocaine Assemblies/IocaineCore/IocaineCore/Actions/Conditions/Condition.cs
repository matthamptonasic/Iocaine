using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Data.Structures
{
    public abstract partial class Action
    {
        public abstract class Condition
        {
            #region Enums
            public enum CONDITION_TYPE : uint
            {
                AND,
                OR,
                JOB_LVL,
                INV_ITEM,
                KEY_ITEM,
                MP_CURR_MIN,
                RECAST_SPELL
            }
            #endregion Enums

            #region Private Members
            protected CONDITION_TYPE m_conditionType;
            private string m_name;
            private bool m_not;
            #endregion Private Members

            #region Public Properties
            public string Name
            {
                get
                {
                    return m_name;
                }
            }
            protected bool Not
            {
                get
                {
                    return m_not;
                }
            }
            #endregion Public Properties

            public Condition(CONDITION_TYPE iType, string iName, bool iInvert = false)
            {
                m_conditionType = iType;
                m_name = iName;
                m_not = iInvert;
            }

            #region Public Methods
            public abstract bool Evaluate();
            #endregion Public Methods
        }
    }
}