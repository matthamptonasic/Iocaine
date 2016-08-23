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
        public class ConditionTree
        {
            #region Enums
            #endregion Enums

            #region Private Members
            private Condition m_root;
            #endregion Private Members

            #region Public Properties
            #endregion Public Properties

            #region Constructor
            public ConditionTree()
            {
                m_root = null;
            }
            #endregion Constructor

            #region Public Methods
            public bool Evaluate()
            {
                if (m_root == null)
                {
                    return true;
                }
                else
                {
                    return m_root.Evaluate();
                }
            }
            public void PushAnd(Condition iCondition, bool iInvert = false)
            {
                if (m_root == null)
                {
                    pushRoot(iCondition);
                }
                else
                {
                    ConditionAnd and = new ConditionAnd(m_root, iCondition, iCondition.Name + "_AND", iInvert);
                    m_root = and;
                }
            }
            public void PushOr(Condition iCondition, bool iInvert = false)
            {
                if (m_root == null)
                {
                    pushRoot(iCondition);
                }
                else
                {
                    ConditionOr and = new ConditionOr(m_root, iCondition, iCondition.Name + "_OR", iInvert);
                    m_root = and;
                }
            }
            #endregion Public Methods

            #region Private Methods
            private void pushRoot(Condition iCondition)
            {
                m_root = iCondition;
            }
            #endregion Private Methods
        }
    }
}
