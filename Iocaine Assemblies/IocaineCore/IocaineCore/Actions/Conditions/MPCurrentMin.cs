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
        public class MPCurrentMin : Condition
        {
            #region Enums
            #endregion Enums

            #region Private Members
            private ushort m_minMp = 0;
            #endregion Private Members

            #region Public Properties
            public ushort MinMP
            {
                get
                {
                    return m_minMp;
                }
            }
            #endregion Public Properties

            #region Constructor(s)
            public MPCurrentMin(ushort iMinMpRequired)
                : base(CONDITION_TYPE.MP_CURR_MIN, "MpCurrentMin_" + iMinMpRequired)
            {
                m_minMp = iMinMpRequired;
            }
            #endregion Constructor(s)

            #region Public Methods
            public override bool Evaluate()
            {
                if (MemReads.Self.Vitals.get_mp_current() >= m_minMp)
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