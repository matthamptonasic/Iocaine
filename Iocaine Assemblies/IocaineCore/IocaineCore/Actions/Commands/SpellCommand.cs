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
    public class SpellCommand : Command
    {
        #region Private Members
        private Client.Spells.SPELL_INFO _SpellInfo;
        private bool available = false;
        #endregion Private Members

        #region Public Properties
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
                return (TimeRemaining == 0) && available;
            }
        }
        public uint TimeRemaining
        {
            get
            {
                return MemReads.Self.Recast.Magic.get_time_remaining(_SpellInfo.ID);
            }
        }
        public override ushort MP
        {
            get
            {
                return _SpellInfo.MP;
            }
        }
        public Client.Spells.TARGETS Targets
        {
            get
            {
                return (Client.Spells.TARGETS)_SpellInfo.Targets;
            }
        }
        public byte Skill
        {
            get
            {
                return _SpellInfo.Skill;
            }
        }
        public short Element
        {
            get
            {
                return _SpellInfo.Element;
            }
        }
        public ushort Range
        {
            get
            {
                return _SpellInfo.Range;
            }
        }
        public override uint ExecTime
        {
            get
            {
                return (uint)Math.Ceiling(_SpellInfo.CastTime);
            }
        }
        public override uint Duration
        {
            get
            {
                return _SpellInfo.Duration;
            }
        }
        public string SpellType
        {
            get
            {
                return _SpellInfo.Type;
            }
        }
        #endregion Public Properties

        #region Constructors
        public SpellCommand(string iName)
            : base(iName, CMD_TYPE.SPELL, true)
        {
            if (iName != "Dummy")
            {
                ushort Id = Client.Spells.GetSpellID(iName);
                _SpellInfo = Client.Spells.GetSpellInfo(Id);
                setConditionTrees();
            }
            else
            {
                setDummyInfo();
            }
        }
        public SpellCommand(Client.Spells.SPELL_INFO iInfo)
            : base(iInfo.Name, CMD_TYPE.SPELL, true)
        {
            _SpellInfo = Iocaine2.Data.Client.Spells.GetSpellInfo(iInfo.ID);
            setConditionTrees();
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
                if (CanPerform())
                {
                    IocaineFunctions.keys(_SpellInfo.Command + " " + iTarget);
                    LoggingFunctions.Debug(_SpellInfo.Command + " " + iTarget, LoggingFunctions.DBG_SCOPE.COMMANDS);
                    // Wait a bit for the spell to start.
                    IocaineFunctions.delay(1000);
                    do
                    {
                        IocaineFunctions.delay(100);
                    }
                    while (MemReads.Self.Casting.is_casting());
                    IocaineFunctions.delay(Statics.Settings.PowerLevel.CastTimeMargin);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                string msg = "SpellCommand.Execute caught an exception!";
                msg += " Command: '" + _SpellInfo.Command + "'";
                msg += ", Target: '" + iTarget + "'";
                msg += ", Name: '" + _SpellInfo.Name + "'";
                msg += "\n" + e.ToString();
                LoggingFunctions.Error(msg);
                return false;
            }
            return true;
        }
        
        public override void Show()
        {
            MessageBox.Show(this.ToString(), "SpellCommand::" + Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override string SaveString()
        {
            return "SpellCommand;" + _SpellInfo.Name;
        }
        public override string ToString()
        {
            return _SpellInfo.Name;
        }
        #endregion Public Methods

        #region Private Methods
        private void setConditionTrees()
        {
            ConditionTree treeStatic = new ConditionTree();
            ConditionTree treeDynamic = new ConditionTree();
            try
            {
                for (byte ii = Client.Jobs.MinID; ii <= Client.Jobs.MaxID; ii++)
                {
                    if (_SpellInfo.JobLevels[ii] != 0)
                    {
                        treeStatic.PushOr(new JobLevel(Client.Jobs.InfoMap[ii], _SpellInfo.JobLevels[ii], 99, _SpellInfo.AsSub ? JobLevel.MAIN_SUB.EITHER : JobLevel.MAIN_SUB.MAIN_ONLY));
                    }
                }

                treeDynamic.PushAnd(new MPCurrentMin(_SpellInfo.MP));
                treeDynamic.PushAnd(new RecastReadySpell(_SpellInfo.ID));

                setConditions(treeStatic, treeDynamic);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error(e.ToString());
            }
        }
        private void setDummyInfo()
        {
            _SpellInfo = new Client.Spells.SPELL_INFO();
            _SpellInfo.AsSub = false;
            _SpellInfo.CastTime = 1;
            _SpellInfo.Command = "/echo Dummy command";
            _SpellInfo.Duration = 0;
            _SpellInfo.Element = 0;
            _SpellInfo.ID = 0;
            _SpellInfo.JobLevels = new Dictionary<byte, byte>() { { 1, 0 }, { 2, 0 },
                                                                  { 3, 0 }, { 4, 0 },
                                                                  { 5, 0 }, { 6, 0 },
                                                                  { 7, 0 }, { 8, 0 },
                                                                  { 9, 0 }, { 10, 0 },
                                                                  { 11, 0 }, { 12, 0 },
                                                                  { 13, 0 }, { 14, 0 },
                                                                  { 15, 0 }, { 16, 0 },
                                                                  { 17, 0 }, { 18, 0 },
                                                                  { 19, 0 }, { 20, 0 },
                                                                  { 21, 0 }, { 22, 0 },
                                                                  { 23, 0 } };
            _SpellInfo.MP = 0;
            _SpellInfo.Name = "Dummy";
            _SpellInfo.Range = 0;
            _SpellInfo.RecastID = 0xFFFF;
            _SpellInfo.Skill = 0;
            _SpellInfo.Targets = 0;
            _SpellInfo.Type = "Dummy";
        }
        #endregion Private Methods
    }
}
