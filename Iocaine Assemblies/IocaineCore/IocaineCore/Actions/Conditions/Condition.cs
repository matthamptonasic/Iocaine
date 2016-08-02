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
                JOB_LVL,
                INV_ITEM,
                KEY_ITEM
            }
            #endregion Enums

            #region Private Members
            protected CONDITION_TYPE m_conditionType;
            private string m_name;
            #endregion Private Members

            #region Public Properties
            public string Name
            {
                get
                {
                    return m_name;
                }
            }
            #endregion Public Properties

            public Condition(CONDITION_TYPE iType, string iName)
            {
                m_conditionType = iType;
                m_name = iName;
            }

            #region Public Methods
            public abstract bool IsSatisfied();
            #endregion Public Methods
        }
    }
}