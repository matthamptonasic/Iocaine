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
        public class RecastReadyAbility : Condition
        {
            #region Enums
            #endregion Enums

            #region Private Members
            private byte m_recastStructIdx = 0;
            #endregion Private Members

            #region Public Properties
            public byte RecastStructIdx
            {
                get
                {
                    return m_recastStructIdx;
                }
            }
            #endregion Public Properties

            #region Constructor(s)
            public RecastReadyAbility(byte iRecastStructIdx)
                : base(CONDITION_TYPE.RECAST_ABILITY, "RecastReadyAbility_" + iRecastStructIdx)
            {
                m_recastStructIdx = iRecastStructIdx;
            }
            #endregion Constructor(s)

            #region Public Methods
            public override bool Evaluate()
            {
                if (MemReads.Self.Recast.Abilities.get_time_remaining(m_recastStructIdx) == 0)
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