using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;

namespace Iocaine2.Data.Structures
{
    // NOTE: The System.Threading.Timers may be a better choice.
    //       That timer allows you to start a timer and have it go off
    //       some delay later, but not the timer interval/period,
    //       which is a separate value.
    public class Task : Timer
    {
        public enum TYPE : byte
        {
            CURE = 1,
            BUFF = 2,
            JA = 3,
            WS = 4,
            RAW_CMD = 5
        }
        public Task(TYPE iType, String iTargetName, Command iCmd, UInt32 iRecast, bool iEnable, int iPriority)
        {
            type = iType;
            player = iTargetName;
            cmd = iCmd;
            useTimer = iEnable;
            priority = iPriority;

            BeginInit();
            AutoReset = false;
            Enabled = false;
            if (iRecast == 0)
            {
                Interval = Int32.MaxValue;
            }
            else
            {
                Interval = iRecast;
            }
            EndInit();
        }

        private TYPE type;
        public TYPE Type
        {
            get
            {
                return type;
            }
        }
        private String player;
        public String Player
        {
            get
            {
                return player;
            }
        }
        private Command cmd;
        public Command Cmd
        {
            get
            {
                return cmd;
            }
        }
        [Obfuscation(Feature = "renaming")]
        public String CmdName
        {
            get
            {
                return cmd.Name;
            }
        }
        private bool useTimer;
        public bool UseTimer
        {
            get
            {
                return useTimer;
            }
            set
            {
                useTimer = value;
            }
        }
        private int priority;
        public int Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
            }
        }
        public void Reset()
        {
            Boolean prevUseTimer = useTimer;
            Boolean prevEnabled = Enabled;
            Enabled = false;
            useTimer = false;
            this.Stop();
            Enabled = prevEnabled;
            useTimer = prevUseTimer;
            this.Start();
        }
        override public String ToString()
        {
            return cmd.Name;
        }
        public void Kill()
        {
            this.Enabled = false;
            this.useTimer = false;
            this.Stop();
            this.Dispose();
        }
    }
}
