﻿using System;
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
        private Boolean available = false;
        #endregion Private Members

        #region Public Properties
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
        #endregion Public Properties

        #region Constructors
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
                if (Ready)
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
                LoggingFunctions.Error("SpellCommand.doCommand caught exception!");
                LoggingFunctions.Error("Command: '" + _SpellInfo.Command + "'");
                LoggingFunctions.Error("Target: '" + iTarget + "'");
                LoggingFunctions.Error("Name: '" + _SpellInfo.Name + "'");
                LoggingFunctions.Error("Exception:\n" + e.ToString());
                return false;
            }
            return true;
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
        #endregion Public Methods

        #region Private Methods
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
        #endregion Private Methods
    }
}