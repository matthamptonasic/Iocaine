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
        public class RecastReadySpell : Condition
        {
            #region Enums
            #endregion Enums

            #region Private Members
            private ushort m_recastId = 0;
            #endregion Private Members

            #region Public Properties
            public ushort RecastId
            {
                get
                {
                    return m_recastId;
                }
            }
            #endregion Public Properties

            #region Constructor(s)
            public RecastReadySpell(ushort iRecastId)
                : base(CONDITION_TYPE.RECAST_SPELL, "RecastReadySpell_" + iRecastId)
            {
                m_recastId = iRecastId;
            }
            #endregion Constructor(s)

            #region Public Methods
            public override bool Evaluate()
            {
                if (MemReads.Self.Recast.Magic.get_time_remaining(m_recastId) == 0)
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