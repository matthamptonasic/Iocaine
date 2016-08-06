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
        public class ConditionOr : Condition
        {
            #region Enums
            #endregion Enums

            #region Private Members
            private Condition m_condition0;
            private Condition m_condition1;
            #endregion Private Members

            #region Public Properties
            #endregion Public Properties

            public ConditionOr(Condition iCond0, Condition iCond1, string iName, bool iInvert = false)
                : base(CONDITION_TYPE.OR, iName, iInvert)
            {
                m_condition0 = iCond0;
                m_condition1 = iCond1;
            }

            #region Public Methods
            public override bool Evaluate()
            {
                bool retVal = false;
                if ((m_condition0 == null) && (m_condition1 == null))
                {
                    return false;
                }
                else if ((m_condition0 == null) || (m_condition1 == null))
                {
                    if (m_condition0 == null)
                    {
                        retVal = m_condition1.Evaluate();
                    }
                    else
                    {
                        retVal = m_condition0.Evaluate();
                    }
                }
                else
                {
                    retVal = m_condition0.Evaluate() || m_condition1.Evaluate();
                }
                return Not ? !retVal : retVal;
            }
            #endregion Public Methods
        }
    }
}
