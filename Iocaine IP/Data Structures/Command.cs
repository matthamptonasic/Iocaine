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
    abstract public class Command : Action
    {
        #region Enums
        public enum CMD_TYPE : byte
        {
            SPELL = 0,
            JOB_ABIL = 1,
            WEAPONSKILL = 2,
            USE_ITEM = 3,
            RAW_CMD = 4
        }
        #endregion Enums
        #region Constructor
        public Command(String iName, CMD_TYPE iType, Boolean iIsBlocking)
            : base(iIsBlocking, ACTN_TYPE.Command)
        {
            name = iName;
            type = iType;
        }
        #endregion Constructor
        #region Member Variables
        private String name;
        private CMD_TYPE type;
        private Boolean useDuringBattle = false;
        private Boolean enabled = true;
        #endregion Member Variables
        #region Member Properties
        public String Name
        {
            get
            {
                return name;
            }
        }
        public CMD_TYPE Type
        {
            get
            {
                return type;
            }
        }
        public Boolean UseDuringBattle
        {
            get
            {
                return useDuringBattle;
            }
            set
            {
                useDuringBattle = value;
            }
        }
        public abstract Boolean Ready
        {
            get;
        }
        public virtual UInt16 MP
        {
            get
            {
                return 0;
            }
        }
        public virtual UInt16 TP
        {
            get
            {
                return 0;
            }
        }
        public virtual UInt32 ExecTime
        {
            get
            {
                return 0;
            }
        }
        public virtual Boolean Available
        {
            get
            {
                return true;
            }
            set { }
        }
        public virtual UInt32 Duration
        {
            get
            {
                return Int32.MaxValue;
            }
        }
        public Boolean Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }
        #endregion Member Properties
        #region Functions
        public abstract Boolean Execute(String iTarget);
        //{
        //    try
        //    {
        //        if (ready)
        //        {
        //            if (enable)
        //            {
        //                //First section does the command
        //                if ((type == (short)CMD_TYPE.CHAT) || (type == (short)CMD_TYPE.ITEM))
        //                {
        //                    IocaineFunctions.keys(text);
        //                    LoggingFunctions.Debug(text, LoggingFunctions.DBG_SCOPE.COMMANDS);
        //                }
        //                else
        //                {
        //                    IocaineFunctions.keys(text + " " + target);
        //                    LoggingFunctions.Debug(text + " " + target, LoggingFunctions.DBG_SCOPE.COMMANDS);
        //                }
        //                //Second section delays before returning and modifies the cast time
        //                uint castTimeMod = 100;
        //                if ((type != (short)CMD_TYPE.JOB_ABILITY) && (type != (short)CMD_TYPE.CHAT) && (type != (short)CMD_TYPE.ITEM))
        //                {
        //                    castTimeMod = Statics.Settings.PowerLevel.CastTimeModifier;
        //                    if (type == (short)CMD_TYPE.HEALING_CURE)
        //                    {
        //                        castTimeMod = (uint)(Statics.Settings.PowerLevel.CastTimeModifierCures * castTimeMod / 100);
        //                    }
        //                }
        //                //IocaineFunctions.delay((uint)(castTime * castTimeMod / 100) + Iocaine_2_Form.castTimeMargin);
        //                IocaineFunctions.delay((uint)(castTime * castTimeMod / 100));
        //                ready = false;
        //                recastTimer.Start();
        //                IocaineFunctions.delay(Statics.Settings.PowerLevel.CastTimeMargin);
        //            }
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch
        //    {
        //        LoggingFunctions.Error("Command.doCommand caught exception.");
        //        LoggingFunctions.Error("Target: " + target);
        //        LoggingFunctions.Error("Name: " + name);
        //        LoggingFunctions.Error("Ready: " + ready);
        //        return false;
        //    }
        //}
        //public bool changeCommandText(String newCommandText)
        //{
        //    if ((type == (int)CMD_TYPE.CHAT) || (type == (int)CMD_TYPE.ITEM))
        //    {
        //        text = newCommandText;
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public override String ToString()
        {
            return this.Name;
        }
        public override void Show()
        {
            throw new NotImplementedException();
        }
        #endregion Functions
    }

    public class SpellCommand : Command
    {
        #region Constructor
        public SpellCommand(String iName)
            : base(iName, CMD_TYPE.SPELL, true)
        {
            if (iName != "Dummy")
            {
                UInt16 Id = Iocaine2.Data.Client.Spells.GetSpellID(iName);
                _SpellInfo = Iocaine2.Data.Client.Spells.GetSpellInfo(Id);
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
        }
        #endregion Constructor
        #region Member Variables
        private Client.Spells.SPELL_INFO _SpellInfo;
        private Boolean available = false;
        #endregion Member Variables
        #region Member Properties
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
        public override Boolean Ready
        {
            get
            {
                return (TimeRemaining == 0) && available;
            }
        }
        public UInt32 TimeRemaining
        {
            get
            {
                return MemReads.Self.Recast.Magic.get_time_remaining((Int16)_SpellInfo.ID);
            }
        }
        public override UInt16 MP
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
        public Byte Skill
        {
            get
            {
                return _SpellInfo.Skill;
            }
        }
        public Int16 Element
        {
            get
            {
                return _SpellInfo.Element;
            }
        }
        public UInt16 Range
        {
            get
            {
                return _SpellInfo.Range;
            }
        }
        public override UInt32 ExecTime
        {
            get
            {
                return (UInt32)Math.Ceiling(_SpellInfo.CastTime);
            }
        }
        public override UInt32 Duration
        {
            get
            {
                return _SpellInfo.Duration;
            }
        }
        public String SpellType
        {
            get
            {
                return _SpellInfo.Type;
            }
        }
        #endregion Member Properties
        #region Methods
        public override Boolean Execute(String iTarget)
        {
            if(MemReads.Self.Casting.is_casting())
            {
                return false;
            }
            try
            {
                if (Ready)
                {
                    IocaineFunctions.keys( _SpellInfo.Command + " " + iTarget);
                    LoggingFunctions.Debug( _SpellInfo.Command + " " + iTarget, LoggingFunctions.DBG_SCOPE.COMMANDS);
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
            catch(Exception e)
            {
                LoggingFunctions.Error("SpellCommand.doCommand caught exception!");
                LoggingFunctions.Error("Command: '" + _SpellInfo.Command + "'");
                LoggingFunctions.Error("Target: '" + iTarget + "'");
                LoggingFunctions.Error("Name: '" + _SpellInfo.Name + "'");
                LoggingFunctions.Error("Exception:\n" + e.ToString());
                return false;
            }
            return true;
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
            _SpellInfo.LevelJob1 = 0;
            _SpellInfo.LevelJob2 = 0;
            _SpellInfo.LevelJob3 = 0;
            _SpellInfo.LevelJob4 = 0;
            _SpellInfo.LevelJob5 = 0;
            _SpellInfo.LevelJob6 = 0;
            _SpellInfo.LevelJob7 = 0;
            _SpellInfo.LevelJob8 = 0;
            _SpellInfo.LevelJob9 = 0;
            _SpellInfo.LevelJob10 = 0;
            _SpellInfo.LevelJob11 = 0;
            _SpellInfo.LevelJob12 = 0;
            _SpellInfo.LevelJob13 = 0;
            _SpellInfo.LevelJob14 = 0;
            _SpellInfo.LevelJob15 = 0;
            _SpellInfo.LevelJob16 = 0;
            _SpellInfo.LevelJob17 = 0;
            _SpellInfo.LevelJob18 = 0;
            _SpellInfo.LevelJob19 = 0;
            _SpellInfo.LevelJob20 = 0;
            _SpellInfo.LevelJob21 = 0;
            _SpellInfo.LevelJob22 = 0;
            _SpellInfo.LevelJob23 = 0;
            _SpellInfo.MP = 0;
            _SpellInfo.Name = "Dummy";
            _SpellInfo.Range = 0;
            _SpellInfo.RecastID = 0xFFFF;
            _SpellInfo.Skill = 0;
            _SpellInfo.Targets = 0;
            _SpellInfo.Type = "Dummy";
        }
        public override void Show()
        {
            MessageBox.Show(this.ToString(), "SpellCommand::" + Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override String SaveString()
        {
            return "SpellCommand;" + _SpellInfo.Name;
        }
        public override String ToString()
        {
            return _SpellInfo.Name;
        }
        #endregion Methods
    }

    public class JobAbilCommand : Command
    {
        #region Constructor
        public JobAbilCommand(String iName)
            : base(iName, CMD_TYPE.JOB_ABIL, true)
        {
            _AbilityInfo = Client.JobAbilities.GetAbilityInfo(iName);
        }
        public JobAbilCommand(Client.JobAbilities.JA_INFO iInfo)
            : base(iInfo.Name, CMD_TYPE.JOB_ABIL, true)
        {
            _AbilityInfo = iInfo;
        }
        #endregion Constructor
        #region Member Variables
        private Client.JobAbilities.JA_INFO _AbilityInfo;
        private Byte recastStructIndex = 0;
        private Boolean available = false;
        #endregion Member Variables
        #region Member Properties
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
        #endregion Member Properties
        #region Methods
        public override Boolean Execute(String iTarget)
        {
            if (MemReads.Self.Casting.is_casting())
            {
                return false;
            }
            try
            {
                if (Ready)
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
        #endregion Methods
    }

    public class WeaponSkillCommand : Command
    {
        #region Constructor
        public WeaponSkillCommand(String iName)
            : base(iName, CMD_TYPE.WEAPONSKILL, true)
        {
            _WsInfo = Client.WeaponSkills.GetWeaponSkillInfo(iName);
        }
        public WeaponSkillCommand(Client.WeaponSkills.WS_INFO iInfo)
            : base(iInfo.Name, CMD_TYPE.WEAPONSKILL, true)
        {
            _WsInfo = iInfo;
        }
        #endregion Constructor
        #region Member Variables
        private Client.WeaponSkills.WS_INFO _WsInfo;
        private Boolean available = false;
        #endregion Member Variables
        #region Member Properties
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
        public override Boolean Ready
        {
            get
            {
                if (!available)
                {
                    return false;
                }
                else
                {
                    return MemReads.Self.Vitals.get_tp_current() >= 1000;
                }
            }
        }
        #endregion Member Properties
        #region Methods
        public override Boolean Execute(String iTarget = "")
        {
            if (MemReads.Self.Casting.is_casting() || (MemReads.Self.get_status() != (Byte)FFXIEnums.STATUS.ATTACKING))
            {
                return false;
            }
            try
            {
                if (Ready)
                {
                    IocaineFunctions.keys(_WsInfo.Command + " <t>");
                    LoggingFunctions.Debug(_WsInfo.Command + " <t>", LoggingFunctions.DBG_SCOPE.COMMANDS);
                    // Wait a bit for the WS to proc.
                    IocaineFunctions.delay(1000);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("WeaponSkillCommand.doCommand caught exception!");
                LoggingFunctions.Error("Command: '" + _WsInfo.Command + "'");
                LoggingFunctions.Error("Name: '" + _WsInfo.Name + "'");
                LoggingFunctions.Error("Exception:\n" + e.ToString());
                return false;
            }
            return true;
        }
        public override void Show()
        {
            MessageBox.Show(this.ToString(), "WeaponSkillCommand::" + _WsInfo.Command, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override string SaveString()
        {
            return "WeaponSkillCommand;" + _WsInfo.Name;
        }
        public override String ToString()
        {
            return _WsInfo.Name;
        }
        #endregion Methods
    }

    public class RawCommand : Command
    {
        #region Constructor
        public RawCommand(String iName = "", String iText = "", Boolean iSendNow = false)
            : base(iName, CMD_TYPE.RAW_CMD, false)
        {
            text = iText;
            if (iSendNow)
            {
                Enabled = true;
                Execute();
            }
        }
        #endregion Constructor
        #region Member Variables
        private String text = "";
        #endregion Member Variables
        #region Member Properties
        public String Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }
        public override Boolean Ready
        {
            get
            {
                return true;
            }
        }
        #endregion Member Properties
        #region Methods
        public override Boolean Execute(String iTarget = "")
        {
            try
            {
                if (Enabled)
                {
                    IocaineFunctions.keys(text);
                    LoggingFunctions.Debug(text, LoggingFunctions.DBG_SCOPE.COMMANDS);
                    IocaineFunctions.delay(50);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("RawCommand.doCommand caught exception!");
                LoggingFunctions.Error("Command: '" + text + "'");
                LoggingFunctions.Error("Enabled: '" + Enabled + "'");
                LoggingFunctions.Error("Exception:\n" + e.ToString());
                return false;
            }
            return true;
        }
        public override void Show()
        {
            MessageBox.Show(this.ToString(), "RawCommand::" + Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override string SaveString()
        {
            return "RawCommand;" + Text;
        }
        public override String ToString()
        {
            return Text;
        }
        #endregion Methods
    }
}
