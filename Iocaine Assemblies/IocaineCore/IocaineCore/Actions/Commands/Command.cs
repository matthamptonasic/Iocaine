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
}
