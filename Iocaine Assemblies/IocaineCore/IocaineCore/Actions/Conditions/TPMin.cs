using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;
using Iocaine2.Memory;

namespace Iocaine2.Data.Structures
{
    public abstract partial class Action
    {
        public class TPMin : Condition
        {
            #region Enums
            #endregion Enums

            #region Private Members
            private ushort m_minTp = 0;
            #endregion Private Members

            #region Public Properties
            public ushort MinTP
            {
                get
                {
                    return m_minTp;
                }
            }
            #endregion Public Properties

            #region Constructor(s)
            public TPMin(ushort iMinTpRequired)
                : base(CONDITION_TYPE.TP_MIN, "TpMin_" + iMinTpRequired)
            {
                m_minTp = iMinTpRequired;
            }
            #endregion Constructor(s)

            #region Public Methods
            public override bool Evaluate()
            {
                if (MemReads.Self.Vitals.get_tp_current() >= m_minTp)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            #endregion Public Methods

            #region Private Methods
            #endregion Private Methods
        }
    }
}