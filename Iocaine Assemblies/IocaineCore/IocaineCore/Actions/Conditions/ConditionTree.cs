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

            public ConditionTree()
            {
                m_root = null;
            }

            #region Public Methods
            public bool Evaluate()
            {
                return true;
            }
            #endregion Public Methods
        }
    }
}
