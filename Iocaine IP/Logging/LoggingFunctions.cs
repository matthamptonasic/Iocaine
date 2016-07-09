using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

using Iocaine2.Data.Client;
using Iocaine2.Settings;

namespace Iocaine2.Logging
{
    public static class LoggingFunctions
    {
        #region Enums
        public enum DBG_SCOPE : uint
        {
            TOP = 0x1,              //Top level form related only.
            FISHER = 0x2,
            PL = 0x4,
            SU = 0x8,
            CRAFTER = 0x10,
            TA = 0x20,
            NAV = 0x40,
            TRADER = 0x80,
            SELLER = 0x100,
            BUYER = 0x200,
            WMS = 0x400,
            SETTINGS = 0x800,       //To do with any settings forms and/or loading/saving user settings.
            MEMREADS = 0x1000,      //All memory reads not including the memory scanner.
            CHAT = 0x2000,          //All chat log functions above the base memory reads including chat loggers.
            CH_MON = 0x4000,        //To do with the change monitor class.
            ALL = 0x8000,           //Will dump everything.
            FISH_STAT = 0x10000,
            BACKGROUND = 0x20000,   //To do with any and all background threads not tied to more specific debug scope.
            INVENTORY = 0x40000,    //Anything to do with inventory above the memory reads level.
            MEM_SCANNER = 0x80000,  //Only to do with the memory scanner (NOT mem reads).
            WIN_API = 0x100000,     //Base level Windows API's
            INTERACTION = 0x200000, //Any interaction with an NPC/PC (targeting, trading, follow, etc).
            TIME = 0x400000,        //Anything to do with Vana time or the NTS (network time server) functions.
            NPC_PC = 0x800000,      //Everything with NPC/PC that's not interaction-related (pos, status, etc).
            COMMANDS = 0x1000000,   //Anything to do with command entry and/or the command classes.
            SERVER = 0x2000000,     //Anything to do with server interaction.
            SYNERGY = 0x4000000,    //Anything to do with the syngery bot.
            UNFILTERED = 0x80000000,//Catch all for any Debug function called without a debug scope specified.
            SCOPE_COUNT = 28        //Used to end a loop of bits
        }
        #endregion Enums

        #region Private Members
        private static StreamWriter logFile = null;
        private static String logFileName = "";
        private static UInt32 nbWarnings = 0;
        private static UInt32 nbErrors = 0;
        #region Function Pointers
        private static timestamp_delegate timestampPtr = Timestamp;
        private static debug_delegate debugPtr = Debug;
        #endregion Function Pointers
        #endregion Private Members

        #region Public Members
        #region Events
        public static event Statics.FuncPtrs.TD_Void_Void _Warning;
        public static event Statics.FuncPtrs.TD_Void_Void _Error;
        #endregion Events
        #region Delegates
        public delegate void timestamp_delegate(String iText);
        public delegate void debug_delegate(String iText, DBG_SCOPE iScope);
        #endregion Delegates
        #region Properties
        public static StreamWriter LogFile
        {
            get
            {
                return logFile;
            }
            set
            {
                logFile = value;
            }
        }
        public static String Name
        {
            get
            {
                return logFileName;
            }
        }
        public static UInt32 NbWarnings
        {
            get
            {
                return nbWarnings;
            }
            set
            {
                nbWarnings = value;
            }
        }
        public static UInt32 NbErrors
        {
            get
            {
                return nbErrors;
            }
            set
            {
                nbErrors = value;
            }
        }
        public static uint Dbg
        {
            get
            {
                return (uint)((Statics.Settings.Top.DebugScope != 0) ? 1 : 0);
            }
        }
        /// <summary>
        /// In case the user wants to log with their own timestamp function, set this function pointer
        /// with the desired function and use it to log instead of the timestamp function in this class.
        /// </summary>
        public static timestamp_delegate TimestampPtr
        {
            get
            {
                return timestampPtr;
            }
            set
            {
                timestampPtr = value;
            }
        }
        /// <summary>
        /// In case the user wants to log with their own debug function, set this function pointer
        /// with the desired function and use it to log instead of the debug function in this class.
        /// </summary>
        public static debug_delegate DebugPtr
        {
            get
            {
                return debugPtr;
            }
            set
            {
                debugPtr = value;
            }
        }
        #endregion Properties
        #endregion Public Members

        #region Private Methods
        private static String parseRelativeFileName(String iAbsoluteFileName)
        {
            String relativeFileName = "";
            int position = -1;
            if (iAbsoluteFileName.Contains("Applications"))
            {
                position = iAbsoluteFileName.IndexOf("Applications");
            }
            else if (iAbsoluteFileName.Contains("Iocaine_IP"))
            {
                position = iAbsoluteFileName.IndexOf("Iocaine_IP");
            }
            else if (iAbsoluteFileName.Length < 3)
            {
                return "Unknown";
            }
            else
            {
                position = iAbsoluteFileName.LastIndexOf("\\") + 1;
            }
            relativeFileName = iAbsoluteFileName.Substring(position);
            return relativeFileName;
        }
        private static uint checkDebugScope(DBG_SCOPE iScope)
        {
            uint value = Statics.Settings.Top.DebugScope & (uint)iScope;
            if (value != 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        #endregion Private Methods

        #region Public Methods
        #region Log File
        public static void SetLogFile(String iFileName)
        {
            if ((logFile == null) || (logFileName != iFileName))
            {
                logFile = new StreamWriter(iFileName);
                logFile.AutoFlush = true;
                Console.SetOut(logFile);
                Console.SetError(logFile);
                logFileName = iFileName;
            }
        }
        public static void SetLogFile()
        {
            SetLogFile("Iocaine Log.txt");
        }
        public static void CloseLogFile()
        {
            if (logFile != null)
            {
                logFile.Flush();
                logFile.Close();
                logFile = null;
            }
        }
        #endregion Log File
        #region Debug Scope
        public static void LoadDebugScope()
        {
            Statics.Settings.Top.RetainSettings = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "RetainSettings");
            if (Statics.Settings.Top.RetainSettings)
            {
                Statics.Settings.Top.DebugScope = (uint)UserSettings.GetValue(UserSettings.BOT.TOP, "DebugScope");
            }
            else
            {
                Statics.Settings.Top.DebugScope = 0x0;
            }
            UserSettings.SetValue(UserSettings.BOT.TOP, "DebugScope", Statics.Settings.Top.DebugScope.ToString());
        }
        public static void AddDebugScope(DBG_SCOPE iScope)
        {
            Statics.Settings.Top.DebugScope |= (uint)iScope;
            UserSettings.SetValue(UserSettings.BOT.TOP, "DebugScope", Statics.Settings.Top.DebugScope.ToString());
        }
        public static bool RemoveDebugScope(DBG_SCOPE iScope)
        {
            if ((Statics.Settings.Top.DebugScope & (uint)iScope) != 0)
            {
                Statics.Settings.Top.DebugScope &= ~((uint)iScope);
                UserSettings.SetValue(UserSettings.BOT.TOP, "DebugScope", Statics.Settings.Top.DebugScope.ToString());
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void OutputDebugScope()
        {
            Timestamp("================ Debug Scopes =================");
            for (UInt32 ii = 0; ii < (UInt32)DBG_SCOPE.SCOPE_COUNT-1; ii++)
            {
                UInt32 oneHot = (UInt32)1 << (Int32)ii;
                Timestamp(((DBG_SCOPE)(oneHot)).ToString() + " = " + checkDebugScope((DBG_SCOPE)(ii)).ToString());
            }
            Timestamp(DBG_SCOPE.UNFILTERED.ToString() + " = " + checkDebugScope(DBG_SCOPE.UNFILTERED).ToString());
            Timestamp("===============================================");
        }
        #endregion Debug Scope
        #region Logging
        #region Debug
        public static void Debug(String iText)
        {
            Debug(iText, DBG_SCOPE.UNFILTERED);
        }
        public static void Debug(String iText, DBG_SCOPE iDbgScope)
        {
            if (logFile == null)
            {
                return;
            }
            //If the input scope is not set AND the ALL bit is not set, don't print.
            if (((((uint)iDbgScope) & Statics.Settings.Top.DebugScope) == 0) && ((Statics.Settings.Top.DebugScope & (uint)DBG_SCOPE.ALL) == 0))
            {
                return;
            }
            try
            {
                Monitor.Enter(logFile);
                Console.WriteLine("{0} [DEBUG_{1}] {2}", DateTime.Now.ToString(), iDbgScope.ToString(), iText);
                Monitor.Exit(logFile);
            }
            catch
            {
                //System.Windows.Forms.MessageBox.Show("Error trying to write a debug message to the output file.");
            }
        }
        #endregion Debug
        #region Timestamp
        public static void Timestamp(String text)
        {
            if (logFile == null)
            {
                return;
            }
            try
            {
                Monitor.Enter(logFile);
                Console.WriteLine("{0} {1}", DateTime.Now.ToString(), text);
                Monitor.Exit(logFile);
            }
            catch
            {
                //System.Windows.Forms.MessageBox.Show("Error trying to write a timestamp message to the output file.");
            }
        }
        #endregion Timestamp
        #region Warning
        public static void Warning(String iText)
        {
            //System.Runtime.CompilerServices.
            if (logFile == null)
            {
                return;
            }
            try
            {
                Monitor.Enter(logFile);
                Console.WriteLine(DateTime.Now.ToString() + " [WARN] : " + iText);
                Monitor.Exit(logFile);
                nbWarnings++;
                if (_Warning != null)
                {
                    _Warning();
                }
            }
            catch
            {
                //System.Windows.Forms.MessageBox.Show("Error trying to write a debug message to the output file.");
            }
        }
        #endregion Warning
        #region Error
        public static void Error(String text)
        {
            //System.Runtime.CompilerServices.
            if (logFile == null)
            {
                return;
            }
            try
            {
                StackFrame CallStack = new StackFrame(1, true);
                String fileName = parseRelativeFileName(CallStack.GetFileName());
                int lineNumber = CallStack.GetFileLineNumber();
                String methodName = CallStack.GetMethod().Name;
                Monitor.Enter(logFile);
                Console.WriteLine(DateTime.Now.ToString() + " [ERROR] File: " + fileName + " Line: " + lineNumber + " Method: " + methodName + ": " + text);
                Monitor.Exit(logFile);
                nbErrors++;
                if (_Error != null)
                {
                    _Error();
                }
            }
            catch
            {
                //System.Windows.Forms.MessageBox.Show("Error trying to write a debug message to the output file.");
            }
        }
        #endregion Error
        #endregion Logging
        #endregion Public Methods
    }
}
