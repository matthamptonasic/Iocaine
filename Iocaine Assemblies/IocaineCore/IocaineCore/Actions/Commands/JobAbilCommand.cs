using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;
using System.Windows.Forms;

using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;

namespace Iocaine2.Data.Structures
{
    public class JobAbilCommand : Command
    {
        #region Private Members
        private Client.JobAbilities.JA_INFO _AbilityInfo;
        private Byte recastStructIndex = 0;
        private Boolean available = false;
        #endregion Private Members

        #region Public Properties
        public Byte RecastStructIndex
        {
            set
            {
                recastStructIndex = value;
            }
        }
        public override Boolean Available
        {
            get
            {
                return available;
            }
            set
            {
                available = value;
            }
        }
        public override bool Ready
        {
            get
            {
                if (!available)
                {
                    return false;
                }
                else
                {
                    return TimeRemaining == 0;
                }
            }
        }
        public UInt32 TimeRemaining
        {
            get
            {
                return MemReads.Self.Recast.Abilities.get_time_remaining(recastStructIndex);
            }
        }
        public override UInt32 Duration
        {
            get
            {
                return _AbilityInfo.Duration;
            }
        }
        public override UInt16 MP
        {
            get
            {
                return _AbilityInfo.MP;
            }
        }
        public override UInt16 TP
        {
            get
            {
                return _AbilityInfo.TP;
            }
        }
        public Client.Spells.TARGETS Targets
        {
            get
            {
                return (Client.Spells.TARGETS)_AbilityInfo.Targets;
            }
        }
        public Int16 Element
        {
            get
            {
                return _AbilityInfo.Element;
            }
        }
        #endregion Public Properties

        #region Constructors
        public JobAbilCommand(String iName)
            : base(iName, CMD_TYPE.JOB_ABIL, true)
        {
            _AbilityInfo = Client.JobAbilities.GetAbilityInfo(iName);
            setConditionTrees();
        }
        public JobAbilCommand(Client.JobAbilities.JA_INFO iInfo)
            : base(iInfo.Name, CMD_TYPE.JOB_ABIL, true)
        {
            _AbilityInfo = iInfo;
            setConditionTrees();
        }
        #endregion Constructors
        
        #region Public Methods
        public override Boolean Execute(String iTarget)
        {
            if (MemReads.Self.Casting.is_casting())
            {
                return false;
            }
            try
            {
                if (Ready && CanPerform())
                {
                    IocaineFunctions.keys(_AbilityInfo.Command + " " + iTarget);
                    LoggingFunctions.Debug(_AbilityInfo.Command + " " + iTarget, LoggingFunctions.DBG_SCOPE.COMMANDS);
                    // Wait a bit for the JA to proc.
                    IocaineFunctions.delay(Statics.Settings.PowerLevel.JaProcTime);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("JobAbilCommand.doCommand caught exception!");
                LoggingFunctions.Error("Command: '" + _AbilityInfo.Command + "'");
                LoggingFunctions.Error("Target: '" + iTarget + "'");
                LoggingFunctions.Error("Name: '" + _AbilityInfo.Name + "'");
                LoggingFunctions.Error("Exception:\n" + e.ToString());
                return false;
            }
            return true;
        }
        public override void Show()
        {
            MessageBox.Show(this.ToString(), "JobAbilCommand::" + _AbilityInfo.Command, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override string SaveString()
        {
            return "JobAbilCommand;" + _AbilityInfo.Name;
        }
        public override String ToString()
        {
            return _AbilityInfo.Name;
        }
        #endregion Public Methods

        #region Private Methods
        private void setConditionTrees()
        {
            // Static:
            //  - Job + level + asSub
            ConditionTree treeStatic = new ConditionTree();
            ConditionTree treeDynamic = new ConditionTree();
            if ((_AbilityInfo.Job >= Client.Jobs.MinID) && (_AbilityInfo.Job <= Client.Jobs.MaxID))
            {
                treeStatic.PushOr(new JobLevel(Client.Jobs.InfoMap[_AbilityInfo.Job], _AbilityInfo.JobLevel, 99, _AbilityInfo.AsSub ? JobLevel.MAIN_SUB.EITHER : JobLevel.MAIN_SUB.MAIN_ONLY));
            }

            // Dynamic:
            //  - Target, MP, TP, Recast

            setConditions(treeStatic, treeDynamic);
        }
        #endregion Private Methods
    }
}
