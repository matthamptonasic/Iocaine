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
        public class JobLevel : Condition
        {
            #region Enums
            public enum MAIN_SUB : byte
            {
                MAIN_ONLY,
                SUB_ONLY,
                EITHER,
                NEITHER
            }
            #endregion Enums

            #region Private Members
            private Client.Jobs.JOBS_INFO m_job;
            private byte m_levelMin = 1;
            private byte m_levelMax = 99;
            private MAIN_SUB m_main_sub;
            #endregion Private Members

            #region Public Properties
            public Client.Jobs.JOBS_INFO Job
            {
                get
                {
                    return m_job;
                }
            }
            public byte LevelMin
            {
                get
                {
                    return m_levelMin;
                }
            }
            public byte LevelMax
            {
                get
                {
                    return m_levelMax;
                }
            }
            #endregion Public Properties

            #region Constructor(s)
            public JobLevel(Client.Jobs.JOBS_INFO iInfo, byte iLevelMin = 1, byte iLevelMax = 99, MAIN_SUB iMainSub = MAIN_SUB.EITHER)
                : base(CONDITION_TYPE.JOB_LVL, "JobLevel")
            {
                m_job = iInfo;
                m_levelMin = iLevelMin;
                m_levelMax = iLevelMax;
                m_main_sub = iMainSub;
            }
            #endregion Constructor(s)

            #region Public Methods
            public override bool Evaluate()
            {
                switch (m_main_sub)
                {
                    case MAIN_SUB.MAIN_ONLY:
                        return main_only();
                    case MAIN_SUB.SUB_ONLY:
                        return sub_only();
                    case MAIN_SUB.EITHER:
                        return main_only() || sub_only();
                    case MAIN_SUB.NEITHER:
                        return !main_only() && !sub_only();
                    default:
                        return false;
                }
            }
            #endregion Public Methods

            #region Private Methods
            private bool main_only()
            {
                return (PlayerCache.Vitals.MainJob == m_job.ID) && (PlayerCache.Vitals.MainJobLvl >= m_levelMin) && (PlayerCache.Vitals.MainJobLvl <= m_levelMax);
            }
            private bool sub_only()
            {
                return (PlayerCache.Vitals.SubJob == m_job.ID) && (PlayerCache.Vitals.SubJobLvl >= m_levelMin) && (PlayerCache.Vitals.SubJobLvl <= m_levelMax);
            }
            #endregion Private Methods
        }
    }
}