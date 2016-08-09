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
        private byte recastStructIndex = 0;
        private bool available = false;
        #endregion Private Members

        #region Public Properties
        public byte RecastStructIndex
        {
            set
            {
                recastStructIndex = value;
                setDynamicConditionTree();
            }
        }
        public override bool Available
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
        public uint TimeRemaining
        {
            get
            {
                return MemReads.Self.Recast.Abilities.get_time_remaining(recastStructIndex);
            }
        }
        public override uint Duration
        {
            get
            {
                return _AbilityInfo.Duration;
            }
        }
        public override ushort MP
        {
            get
            {
                return _AbilityInfo.MP;
            }
        }
        public override ushort TP
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
        public short Element
        {
            get
            {
                return _AbilityInfo.Element;
            }
        }
        #endregion Public Properties

        #region Constructors
        public JobAbilCommand(string iName)
            : base(iName, CMD_TYPE.JOB_ABIL, true)
        {
            _AbilityInfo = Client.JobAbilities.GetAbilityInfo(iName);
            setStaticConditionTree();
        }
        public JobAbilCommand(Client.JobAbilities.JA_INFO iInfo)
            : base(iInfo.Name, CMD_TYPE.JOB_ABIL, true)
        {
            _AbilityInfo = iInfo;
            setStaticConditionTree();
        }
        #endregion Constructors
        
        #region Public Methods
        public override bool Execute(string iTarget)
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
        public override string ToString()
        {
            return _AbilityInfo.Name;
        }
        #endregion Public Methods

        #region Private Methods
        private void setStaticConditionTree()
        {
            // Static:
            //  - Job + level + asSub
            ConditionTree treeStatic = new ConditionTree();
            if ((_AbilityInfo.Job >= Client.Jobs.MinID) && (_AbilityInfo.Job <= Client.Jobs.MaxID))
            {
                treeStatic.PushOr(new JobLevel(Client.Jobs.InfoMap[_AbilityInfo.Job], _AbilityInfo.JobLevel, 99, _AbilityInfo.AsSub ? JobLevel.MAIN_SUB.EITHER : JobLevel.MAIN_SUB.MAIN_ONLY));
            }
            setConditions(treeStatic, null);
        }
        private void setDynamicConditionTree()
        {
            ConditionTree treeDynamic = new ConditionTree();
            // Dynamic:
            //  - MP, TP, Recast
            treeDynamic.PushAnd(new MPCurrentMin(_AbilityInfo.MP));
            treeDynamic.PushAnd(new TPMin(_AbilityInfo.TP));
            treeDynamic.PushAnd(new RecastReadyAbility(recastStructIndex));

            setConditions(treeDynamic);
        }
        #endregion Private Methods
    }
}
