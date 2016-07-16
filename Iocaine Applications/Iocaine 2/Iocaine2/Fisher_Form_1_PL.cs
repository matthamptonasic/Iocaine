using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Char;
using Iocaine2.Data.Structures;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Properties;
using Iocaine2.Settings;
using Iocaine2.Tools;

namespace Iocaine2
{
    //This file is for functions directly related to the PL bot
    //[System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    //[System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    partial class Iocaine_2_Form
    {
        #region Enums
        public enum PL_CHAR_GRID_COL : int
        {
            PRI = 0,
            NAME = 1,
            ACT = 2
        }
        public enum PL_CURE_GRID_COL : int
        {
            CURES = 0,
            WHICH = 1,
            CUREHPPERC = 2
        }
        public enum PL_BUFF_GRID_COL : int
        {
            BUFF = 0,
            EN = 1,
            RECAST = 2,
            CAST = 3
        }
        public enum PL_DEBUFF_GRID_COL : int
        {
            BUFF = 0,
            CAST = 1
        }
        public enum PL_SELF_BUFF_COL : int
        {
            BUFF = 0,
            EN = 1,
            RECAST = 2,
            CAST = 3
        }
        public enum PL_REST_ARR_INX : int
        {
            IN_FIGHT = 0,
            STAND_LOW = 1,
            STAND_HIGH = 2,
            MP_LOW = 3,
            MP_HIGH  = 4,
            LENGTH = 5
        }
        public enum PL_REST_PER_TYPE_ROW : int
        {
            THIRD_CURE = 0,
            SECOND_CURE = 1,
            FIRST_CURE = 2,
            PARTY_BUFFS = 3,
            SELF_BUFFS = 4,
            DEBUFFS = 5,
            WAKE_CURE = 6,
            ROW_COUNT = 7
        }
        #endregion
        #region Members
        #region Flags
        private static Boolean PL_FirstInitDone = false;
        private static Boolean PL_CuresInitDone = false;
        private static Boolean PL_PauseDebuffsParser = false;
        #endregion Flags
        #region Threads
        private Thread PL_EnqueueThread = null;
        private Thread PL_DequeueThread = null;
        private Thread PL_CheckActiveThread = null;
        private bool PL_PauseCheckActiveThread = false;
        #endregion Threads
        #region Containers
        private List<PLCharacter> PL_CharacterList = new List<PLCharacter>();
        private List<Command> PL_CureCmdList = new List<Command>();                 //Holds all of the available cure commands.
        private List<Command> PL_PartyBuffCmdList = new List<Command>();            //Holds a subset of buff commands that appear in the default grid.
        private List<UInt32> PL_PartyBuffRecastList = new List<UInt32>();           //Holds the corresponding recast time for the above commands.
        private List<Boolean> PL_PartyBuffEnableList = new List<Boolean>();         //Holds the corresponding enable for the above commands.
        private List<Command> PL_PartyBuffCmdListMaster = new List<Command>();
        private List<UInt32> PL_PartyBuffRecastListMaster = new List<UInt32>();
        private List<Boolean> PL_PartyBuffEnableListMaster = new List<Boolean>();
        private List<Command> PL_MyDebuffCmdList = new List<Command>();             //Holds the list of debuffs in the grid.
        private List<Command> PL_MyDebuffCmdListMaster = new List<Command>();
        private List<Task> PL_MySelfBuffTaskList = new List<Task>();                //Holds the list of self buff tasks in the grid.
        private List<Task> PL_MySelfBuffTaskListMaster = new List<Task>();
        #endregion Containers
        #region ChatLog
        private static ChatLoggerSync PL_chatDebuffParsing;
        #endregion ChatLog
        #region Default Values / Settings
        //Character buff related settings
        private String PL_CharacterSticky = ""; //This gets set when a player is added for the first time. After that, the PL will not be reset on logout/in, etc.
        private PLCharacter PL_FocusCharacter = null;
        private PLCharacter PL_FollowCharacter = null;
        private Command PL_WakeCureCommand;
        private Command PL_FirstCureDefCmd;
        private Command PL_SecondCureDefCmd;
        private Command PL_ThirdCureDefCmd;
        private Command PL_FirstCureDefCmdMaster;
        private Command PL_SecondCureDefCmdMaster;
        private Command PL_ThirdCureDefCmdMaster;
        private String PL_FirstCureDefault = "Cure II";
        private String PL_SecondCureDefault = "Cure III";
        private String PL_ThirdCureDefault = "Cure IV";
        private Byte PL_FirstCureDefaultPerc = 85;
        private Byte PL_SecondCureDefaultPerc = 65;
        private Byte PL_ThirdCureDefaultPerc = 35;
        #endregion Default Values / Settings
        #region GUI Member Parallels
        private bool PL_CuresAutoWake = true;
        #endregion GUI Member Parallels
        #endregion Members
        #region GUI Thread Synchronization
        private delegate bool PL_initCharacterGridDelegate();
        private PL_initCharacterGridDelegate PL_initCharacterGridPtr;
        private delegate bool PL_addCharacterDelegate(String iName);
        private PL_addCharacterDelegate PL_addCharacterPtr;
        private delegate void PL_removeCharacterDelegate(String iName, int iRowIndex);
        private PL_removeCharacterDelegate PL_removeCharacterPtr;
        private delegate bool PL_loadCuresGridDelegate(String iName);
        private PL_loadCuresGridDelegate PL_loadCuresGridPtr;
        private delegate void PL_clearCuresGridDelegate();
        private PL_clearCuresGridDelegate PL_clearCuresGridPtr;
        private delegate bool PL_loadBuffsGridDelegate(String iName);
        private PL_loadBuffsGridDelegate PL_loadBuffsGridPtr;
        private delegate void PL_clearBuffsGridDelegate();
        private PL_clearBuffsGridDelegate PL_clearBuffsGridPtr;
        private delegate bool PL_loadDebuffsGridDelegate();
        private PL_loadDebuffsGridDelegate PL_loadDebuffsGridPtr;
        private delegate void PL_clearDebuffGridDelegate();
        private PL_clearDebuffGridDelegate PL_clearDebuffGridPtr;
        private delegate bool PL_loadSelfBuffsGridDelegate();
        private PL_loadSelfBuffsGridDelegate PL_loadSelfBuffsGridPtr;
        private delegate void PL_clearSelfBuffsGridDelegate();
        private PL_clearSelfBuffsGridDelegate PL_clearSelfBuffsGridPtr;
        private delegate void PL_refreshListsDelegate();
        private PL_refreshListsDelegate PL_refreshListsPtr;
        private delegate bool PL_addMeDelegate();
        private PL_addMeDelegate PL_addMePtr;
        private delegate void PL_appendChatDelegate(String iText);
        private PL_appendChatDelegate PL_appendChatPtr;
        private PL_refreshListsDelegate PL_removeChatPtr;
        #endregion GUI Thread Synchronization
        #region Inits Section
        private bool PL_Init()
        {
            LoggingFunctions.Debug("PL_DoInits: Initializing the PL.", LoggingFunctions.DBG_SCOPE.PL);
            if (!PL_FirstInitDone)
            {
                //PL_Sub_Tabs.TabPages.RemoveByKey("PL_OffenseSubTab");
                //Main_Tab_Control.TabPages.RemoveByKey("PL_Events_Tab");
                PL_chatDebuffParsing = ChatLogManager.Access.GetSynchronousLogger("PL_chatDebuffParsing");
                ChatLogManager.Access._NewChatAvailable += new ChatLogManager.NewChatAvailable(PL_ChatParseDebuffs);
                PL_EventsChatLogRTB.WordWrap = false;
                //PL_EventsRTB.Font = new Font("Courier New", 10);
                //DebuffGrid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(PL_DebuffGrid_CellFormatting);
            }
            if ((!PL_FirstInitDone) || (PL_CharacterSticky == ""))
            {
                #region Reset Settings
                if (PL_FirstInitDone)
                {
                    PL_Reset();
                }
                PL_LoadSettings();
                if (!PL_InitRestSettings())
                {
                    return false;
                }
                #endregion Reset Settings
                #region PowerLevel Setup
                Bots.PowerLevel.Access.SetCharList(PL_CharacterList);
                Bots.PowerLevel.Access.SetCuresList(PL_CureCmdList);
                Bots.PowerLevel.Access.doInits();
                #endregion PowerLevel Setup
                #region Function Pointers
                if (!PL_FirstInitDone)
                {
                    PL_initCharacterGridPtr = new PL_initCharacterGridDelegate(PL_InitCharacterGridCBF);
                    PL_addCharacterPtr = new PL_addCharacterDelegate(PL_AddCharacterCBF);
                    PL_removeCharacterPtr = new PL_removeCharacterDelegate(PL_RemoveCharacter);
                    PL_loadCuresGridPtr = new PL_loadCuresGridDelegate(PL_LoadCuresGridCBF);
                    PL_clearCuresGridPtr = new PL_clearCuresGridDelegate(PL_ClearCuresGridCBF);
                    PL_loadBuffsGridPtr = new PL_loadBuffsGridDelegate(PL_LoadBuffsGridCBF);
                    PL_clearBuffsGridPtr = new PL_clearBuffsGridDelegate(PL_ClearBuffsGridCBF);
                    PL_loadDebuffsGridPtr = new PL_loadDebuffsGridDelegate(PL_LoadDebuffsGridCBF);
                    PL_clearDebuffGridPtr = new PL_clearDebuffGridDelegate(PL_ClearDebuffGridCBF);
                    PL_loadSelfBuffsGridPtr = new PL_loadSelfBuffsGridDelegate(PL_LoadSelfBuffsGridCBF);
                    PL_clearSelfBuffsGridPtr = new PL_clearSelfBuffsGridDelegate(PL_ClearSelfBuffsGridCBF);
                    PL_addMePtr = new PL_addMeDelegate(PL_AddMeCBF);
                    PL_refreshListsPtr = new PL_refreshListsDelegate(PL_RefreshListsCBF);
                    PL_appendChatPtr = new PL_appendChatDelegate(PL_AppendChatCBF);
                    PL_removeChatPtr = new PL_refreshListsDelegate(PL_RemoveChatCBF);
                }
                #endregion Function Pointers
            }
            
            if (PL_CharacterSticky == "")
            {
                //Not stuck to a particular login, so reset/init everything.
                PL_InitCharacterGrid();
                PL_RefreshLists();
                PL_AddMe();    //after PL is initialized, need to add yourself to char grid
                CharacterGrid.ClearSelection();
                CharacterGrid[(int)PL_CHAR_GRID_COL.NAME, 1].Selected = true;
                PL_CheckActiveThread = new Thread(new ThreadStart(PL_CheckPlayerActivityThreadRun));
                PL_CheckActiveThread.Name = "PLCheckActiveThread";
                PL_CheckActiveThread.IsBackground = true;
                PL_CheckActiveThread.Start();
            }
            else
            {
                if (MemReads.Self.get_name(true) == PL_CharacterSticky)
                {
                    TOP_Thread_resumePlThreads();
                    PL_RefreshLists();
                }
                else
                {
                    TOP_Thread_pausePlThreads();
                }
            }
            PL_ReloadGrids();
            return PL_FirstInitDone = true;
        }
        private void PL_LoadSettings()
        {
            #region Individual Values
            PL_FirstCureDefault = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.PL, "FirstCureDefault"));
            PL_SecondCureDefault = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.PL, "SecondCureDefault"));
            PL_ThirdCureDefault = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.PL, "ThirdCureDefault"));
            PL_FirstCureDefaultPerc = Convert.ToByte(UserSettings.GetValue(UserSettings.BOT.PL, "FirstCureDefaultPerc"));
            PL_SecondCureDefaultPerc = Convert.ToByte(UserSettings.GetValue(UserSettings.BOT.PL, "secondCureDefaultPerc"));
            PL_ThirdCureDefaultPerc = Convert.ToByte(UserSettings.GetValue(UserSettings.BOT.PL, "thirdCureDefaultPerc"));
            Statics.Settings.PowerLevel.PlCharActivePollFrequency = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "PlCharActivePollFrequency"));
            Statics.Settings.PowerLevel.MaxCastDistance = Convert.ToSingle(UserSettings.GetValue(UserSettings.BOT.PL, "MaxCastDistance"));
            Statics.Settings.PowerLevel.HealthUpdateFrequency = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "HealthUpdateFrequency"));
            Statics.Settings.PowerLevel.DequeuePollFrequency = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "DequeuePollFrequency"));
            Statics.Settings.PowerLevel.CastTimeModifier = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "CastTimeModifier"));
            Statics.Settings.PowerLevel.CastTimeModifierCures = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "CastTimeModifierCures"));
            Statics.Settings.PowerLevel.CastTimeMargin = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "CastTimeMargin"));
            Statics.Settings.PowerLevel.JaProcTime = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "JaProcTime"));
            Statics.Settings.PowerLevel.HealMpPercLow = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "HealMpPercLow"));
            Statics.Settings.PowerLevel.HealMpPercHigh = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "HealMpPercHigh"));
            Statics.Settings.PowerLevel.RestInFight = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "RestInFight"));
            Statics.Settings.PowerLevel.RestInFightAlways = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "RestInFightAlways"));
            Statics.Settings.PowerLevel.RestInFightLessUpper = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "RestInFightLessUpper"));
            Statics.Settings.PowerLevel.RestInFightLessLower = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "RestInFightLessLower"));
            Statics.Settings.PowerLevel.UpperMpMargin = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "UpperMpMargin"));
            Statics.Settings.PowerLevel.NeverRest = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "NeverRest"));
            Statics.Settings.PowerLevel.RestingStyle = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.PL, "RestingStyle"));
            Statics.Settings.PowerLevel.FollowDistance = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.PL, "FollowDistance"));
            Statics.Settings.PowerLevel.UseElasticFollowing = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "UseElasticFollowing"));
            Statics.Settings.PowerLevel.ElasticDistance = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.PL, "ElasticDistance"));
            Statics.Settings.PowerLevel.ZoneTimer = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "ZoneTimer"));
            Statics.Settings.PowerLevel.WakeCure = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.PL, "WakeCure"));
            Statics.Settings.PowerLevel.BuffQ = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.PL, "BuffQ"));
            Statics.Settings.PowerLevel.SelfBuffQ = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.PL, "SelfBuffQ"));
            Statics.Settings.PowerLevel.DebuffQ = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.PL, "DebuffQ"));
            Statics.Settings.PowerLevel.WakeCureQ = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.PL, "WakeCureQ"));
            Statics.Settings.PowerLevel.ThirdCureUseInFight = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "ThirdCureUseInFight"));
            Statics.Settings.PowerLevel.ThirdCureStopRestMpLow = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "ThirdCureStopRestMpLow"));
            Statics.Settings.PowerLevel.ThirdCureStopRestMpHigh = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "ThirdCureStopRestMpHigh"));
            Statics.Settings.PowerLevel.ThirdCureMpLow = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "ThirdCureMpLow"));
            Statics.Settings.PowerLevel.ThirdCureMpHigh = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "ThirdCureMpHigh"));
            Statics.Settings.PowerLevel.SecondCureUseInFight = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "SecondCureUseInFight"));
            Statics.Settings.PowerLevel.SecondCureStopRestMpLow = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "SecondCureStopRestMpLow"));
            Statics.Settings.PowerLevel.SecondCureStopRestMpHigh = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "SecondCureStopRestMpHigh"));
            Statics.Settings.PowerLevel.SecondCureMpLow = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "SecondCureMpLow"));
            Statics.Settings.PowerLevel.SecondCureMpHigh = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "SecondCureMpHigh"));
            Statics.Settings.PowerLevel.FirstCureUseInFight = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "FirstCureUseInFight"));
            Statics.Settings.PowerLevel.FirstCureStopRestMpLow = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "FirstCureStopRestMpLow"));
            Statics.Settings.PowerLevel.FirstCureStopRestMpHigh = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "FirstCureStopRestMpHigh"));
            Statics.Settings.PowerLevel.FirstCureMpLow = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "FirstCureMpLow"));
            Statics.Settings.PowerLevel.FirstCureMpHigh = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "FirstCureMpHigh"));
            Statics.Settings.PowerLevel.BuffUseInFight = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "BuffUseInFight"));
            Statics.Settings.PowerLevel.BuffStopRestMpLow = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "BuffStopRestMpLow"));
            Statics.Settings.PowerLevel.BuffStopRestMpHigh = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "BuffStopRestMpHigh"));
            Statics.Settings.PowerLevel.BuffMpLow = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "BuffMpLow"));
            Statics.Settings.PowerLevel.BuffMpHigh = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "BuffMpHigh"));
            Statics.Settings.PowerLevel.SelfBuffUseInFight = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "SelfBuffUseInFight"));
            Statics.Settings.PowerLevel.SelfBuffStopRestMpLow = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "SelfBuffStopRestMpLow"));
            Statics.Settings.PowerLevel.SelfBuffStopRestMpHigh = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "SelfBuffStopRestMpHigh"));
            Statics.Settings.PowerLevel.SelfBuffMpLow = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "SelfBuffMpLow"));
            Statics.Settings.PowerLevel.SelfBuffMpHigh = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "SelfBuffMpHigh"));
            Statics.Settings.PowerLevel.DebuffUseInFight = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "DebuffUseInFight"));
            Statics.Settings.PowerLevel.DebuffStopRestMpLow = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "DebuffStopRestMpLow"));
            Statics.Settings.PowerLevel.DebuffStopRestMpHigh = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "DebuffStopRestMpHigh"));
            Statics.Settings.PowerLevel.DebuffMpLow = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "DebuffMpLow"));
            Statics.Settings.PowerLevel.DebuffMpHigh = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "DebuffMpHigh"));
            Statics.Settings.PowerLevel.WakeCureUseInFight = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "WakeCureUseInFight"));
            Statics.Settings.PowerLevel.WakeCureStopRestMpLow = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "WakeCureStopRestMpLow"));
            Statics.Settings.PowerLevel.WakeCureStopRestMpHigh = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "WakeCureStopRestMpHigh"));
            Statics.Settings.PowerLevel.WakeCureMpLow = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "WakeCureMpLow"));
            Statics.Settings.PowerLevel.WakeCureMpHigh = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "WakeCureMpHigh"));
            Statics.Settings.PowerLevel.ConvertEnable = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "ConvertEnable"));
            Statics.Settings.PowerLevel.ConvertEnInFight = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "ConvertEnInFight"));
            Statics.Settings.PowerLevel.ConvertMpThreshold = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "ConvertMpThreshold"));
            Statics.Settings.PowerLevel.ConvertCureRestFullFirst = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "ConvertCureRestFullFirst"));
            Statics.Settings.PowerLevel.ConvertCureRestFullMargin = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "ConvertCureRestFullMargin"));
            Statics.Settings.PowerLevel.ChatMpReport = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "ChatMpReport"));
            Statics.Settings.PowerLevel.ChatMpReportCmd = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.PL, "ChatMpReportCmd"));
            Statics.Settings.PowerLevel.ChatMpReportTimer = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "ChatMpReportTimer"));
            Statics.Settings.PowerLevel.ChatHpReport = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "ChatHpReport"));
            Statics.Settings.PowerLevel.ChatHpReportCmd = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.PL, "ChatHpReportCmd"));
            Statics.Settings.PowerLevel.ChatHpReportLevel = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "ChatHpReportLevel"));
            Statics.Settings.PowerLevel.ChatCastTime = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "ChatCastTime"));
            Statics.Settings.PowerLevel.ChatCastTimeCmd = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.PL, "ChatCastTimeCmd"));
            Statics.Settings.PowerLevel.ChatCastTimeTime = Convert.ToDouble(UserSettings.GetValue(UserSettings.BOT.PL, "ChatCastTimeTime"));
            Statics.Settings.PowerLevel.ChatLost = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "ChatLost"));
            Statics.Settings.PowerLevel.ChatLostCmd = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.PL, "ChatLostCmd"));
            Statics.Settings.PowerLevel.Command1Enable = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "Command1Enable"));
            Statics.Settings.PowerLevel.Command1Cmd = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.PL, "Command1Cmd"));
            Statics.Settings.PowerLevel.Command1Timer = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "Command1Timer"));
            Statics.Settings.PowerLevel.Command1BeforeResting = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "Command1BeforeResting"));
            Statics.Settings.PowerLevel.Command2Enable = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "Command2Enable"));
            Statics.Settings.PowerLevel.Command2Cmd = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.PL, "Command2Cmd"));
            Statics.Settings.PowerLevel.Command2Timer = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "Command2Timer"));
            Statics.Settings.PowerLevel.Command2BeforeResting = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "Command2BeforeResting"));
            Statics.Settings.PowerLevel.Command3Enable = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "Command3Enable"));
            Statics.Settings.PowerLevel.Command3Cmd = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.PL, "Command3Cmd"));
            Statics.Settings.PowerLevel.Command3Timer = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "Command3Timer"));
            Statics.Settings.PowerLevel.Command3BeforeResting = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "Command3BeforeResting"));
            Statics.Settings.PowerLevel.Command4Enable = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "Command4Enable"));
            Statics.Settings.PowerLevel.Command4Cmd = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.PL, "Command4Cmd"));
            Statics.Settings.PowerLevel.Command4Timer = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.PL, "Command4Timer"));
            Statics.Settings.PowerLevel.Command4BeforeResting = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.PL, "Command4BeforeResting"));
            #endregion Individual Values
            #region Lists
            #region Party Buff List
            List<List<Object>> buffList = UserSettings.GetList(UserSettings.BOT.PL, UserSettings.LIST_TABLE.PL_PARTYBUFFS);
            if (buffList != null)
            {
                LoggingFunctions.Debug("PL_LoadSettings: There are " + buffList.Count + " items in the party buff list.", LoggingFunctions.DBG_SCOPE.PL);
                foreach (List<Object> row in buffList)
                {
                    Command command = null;
                    String commandName = Data.Client.FfxiResource.DecodeApostrophy(Convert.ToString(row[0]));
                    UInt32 commandRecast = Convert.ToUInt32(row[1]);
                    foreach (Command cmd in CommandManager.AllCommands)
                    {
                        if (cmd.Name == commandName)
                        {
                            command = cmd;
                            if (!PL_PartyBuffCmdListMaster.Contains(cmd))
                            {
                                PL_PartyBuffCmdListMaster.Add(cmd);
                                PL_PartyBuffRecastListMaster.Add(commandRecast);
                                PL_PartyBuffEnableListMaster.Add(false);
                            }
                            break;
                        }
                    }
                    LoggingFunctions.Debug("PL_LoadSettings: Adding user saved party buff to the master list: " + commandName + " recast: " + commandRecast + ".", LoggingFunctions.DBG_SCOPE.PL);
                }
            }
            #endregion Party Buff List
            #region Debuff List
            List<List<Object>> commandList = UserSettings.GetList(UserSettings.BOT.PL, UserSettings.LIST_TABLE.PL_DEBUFFS);
            if (commandList != null)
            {
                foreach (List<Object> strList in commandList)
                {
                    foreach (Object str in strList)
                    {
                        String cmdName = Data.Client.FfxiResource.DecodeApostrophy((String)str);
                        int debuffIndex = -1;
                        foreach (Command cmd in CommandManager.AllCommands)
                        {
                            if (cmd.Name == cmdName)
                            {
                                debuffIndex = CommandManager.AllCommands.IndexOf(cmd);
                            }
                        }
                        if (debuffIndex == -1)
                        {
                            continue;
                        }
                        //Add cure to the my debuff master list.
                        Command cmdToAdd = CommandManager.AllCommands[debuffIndex];
                        PL_MyDebuffCmdListMaster.Add(cmdToAdd);
                    }
                }
            }
            #endregion Debuff List
            #region Self Buff List
            List<List<Object>> selfBuffList = UserSettings.GetList(UserSettings.BOT.PL, UserSettings.LIST_TABLE.PL_SELFBUFFS);
            if (selfBuffList != null)
            {
                LoggingFunctions.Debug("PL_LoadSettings: There are " + selfBuffList.Count + " items in the self buff list.", LoggingFunctions.DBG_SCOPE.PL);
                foreach (List<Object> row in selfBuffList)
                {
                    String commandName = Data.Client.FfxiResource.DecodeApostrophy(Convert.ToString(row[0]));
                    UInt32 commandRecast = Convert.ToUInt32(row[1]);
                    Command cmdToAdd = null;
                    foreach (Command cmd in CommandManager.AllCommands)
                    {
                        if (cmd.Name == commandName)
                        {
                            cmdToAdd = cmd;
                            break;
                        }
                    }
                    if (cmdToAdd == null)
                    {
                        continue;
                    }
                    //Add task to the my debuff master list
                    Task tskToAdd = new Task(Task.TYPE.BUFF, PlayerCache.Vitals.Name, cmdToAdd, commandRecast, false, Statics.Settings.PowerLevel.SelfBuffQ);
                    PL_MySelfBuffTaskListMaster.Add(tskToAdd);
                }
            }
            #endregion Self Buff List
            #endregion Lists
        }
        private bool PL_InitRestSettings()
        {
            for (int kk = 0; kk < (int)PL_REST_PER_TYPE_ROW.ROW_COUNT; kk++)
            {
                bool[] restSettingsTypeArray = new bool[(int)PL_REST_ARR_INX.LENGTH];
                if (kk == (int)PL_REST_PER_TYPE_ROW.THIRD_CURE)
                {
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.IN_FIGHT] = Statics.Settings.PowerLevel.ThirdCureUseInFight;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.STAND_LOW] = Statics.Settings.PowerLevel.ThirdCureStopRestMpLow;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.STAND_HIGH] = Statics.Settings.PowerLevel.ThirdCureStopRestMpHigh;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.MP_LOW] = Statics.Settings.PowerLevel.ThirdCureMpLow;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.MP_HIGH] = Statics.Settings.PowerLevel.ThirdCureMpHigh;
                }
                else if (kk == (int)PL_REST_PER_TYPE_ROW.SECOND_CURE)
                {
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.IN_FIGHT] = Statics.Settings.PowerLevel.SecondCureUseInFight;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.STAND_LOW] = Statics.Settings.PowerLevel.SecondCureStopRestMpLow;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.STAND_HIGH] = Statics.Settings.PowerLevel.SecondCureStopRestMpHigh;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.MP_LOW] = Statics.Settings.PowerLevel.SecondCureMpLow;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.MP_HIGH] = Statics.Settings.PowerLevel.SecondCureMpHigh;
                }
                else if (kk == (int)PL_REST_PER_TYPE_ROW.FIRST_CURE)
                {
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.IN_FIGHT] = Statics.Settings.PowerLevel.FirstCureUseInFight;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.STAND_LOW] = Statics.Settings.PowerLevel.FirstCureStopRestMpLow;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.STAND_HIGH] = Statics.Settings.PowerLevel.FirstCureStopRestMpHigh;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.MP_LOW] = Statics.Settings.PowerLevel.FirstCureMpLow;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.MP_HIGH] = Statics.Settings.PowerLevel.FirstCureMpHigh;
                }
                else if (kk == (int)PL_REST_PER_TYPE_ROW.PARTY_BUFFS)
                {
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.IN_FIGHT] = Statics.Settings.PowerLevel.BuffUseInFight;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.STAND_LOW] = Statics.Settings.PowerLevel.BuffStopRestMpLow;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.STAND_HIGH] = Statics.Settings.PowerLevel.BuffStopRestMpHigh;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.MP_LOW] = Statics.Settings.PowerLevel.BuffMpLow;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.MP_HIGH] = Statics.Settings.PowerLevel.BuffMpHigh;
                }
                else if (kk == (int)PL_REST_PER_TYPE_ROW.SELF_BUFFS)
                {
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.IN_FIGHT] = Statics.Settings.PowerLevel.SelfBuffUseInFight;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.STAND_LOW] = Statics.Settings.PowerLevel.SelfBuffStopRestMpLow;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.STAND_HIGH] = Statics.Settings.PowerLevel.SelfBuffStopRestMpHigh;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.MP_LOW] = Statics.Settings.PowerLevel.SelfBuffMpLow;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.MP_HIGH] = Statics.Settings.PowerLevel.SelfBuffMpHigh;
                }
                else if (kk == (int)PL_REST_PER_TYPE_ROW.DEBUFFS)
                {
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.IN_FIGHT] = Statics.Settings.PowerLevel.DebuffUseInFight;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.STAND_LOW] = Statics.Settings.PowerLevel.DebuffStopRestMpLow;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.STAND_HIGH] = Statics.Settings.PowerLevel.DebuffStopRestMpHigh;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.MP_LOW] = Statics.Settings.PowerLevel.DebuffMpLow;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.MP_HIGH] = Statics.Settings.PowerLevel.DebuffMpHigh;
                }
                else if (kk == (int)PL_REST_PER_TYPE_ROW.WAKE_CURE)
                {
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.IN_FIGHT] = Statics.Settings.PowerLevel.WakeCureUseInFight;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.STAND_LOW] = Statics.Settings.PowerLevel.WakeCureStopRestMpLow;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.STAND_HIGH] = Statics.Settings.PowerLevel.WakeCureStopRestMpHigh;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.MP_LOW] = Statics.Settings.PowerLevel.WakeCureMpLow;
                    restSettingsTypeArray[(int)PL_REST_ARR_INX.MP_HIGH] = Statics.Settings.PowerLevel.WakeCureMpHigh;
                }
                Statics.Settings.PowerLevel.RestingValuePerTypeList.Add(restSettingsTypeArray);
            }
            for (int ii = 0; ii < Statics.Settings.PowerLevel.NbOfQueues; ii++)
            {
                LoggingFunctions.Debug("TopPL::initRestSettings: Initializing row " + ii + " of the restingValueList.", LoggingFunctions.DBG_SCOPE.PL);
                bool[] restSettingsArray = new bool[(int)PL_REST_ARR_INX.LENGTH];
                if (ii == Statics.Settings.PowerLevel.ThirdCureQ - 1)
                {
                    restSettingsArray = (bool[])Statics.Settings.PowerLevel.RestingValuePerTypeList[(int)PL_REST_PER_TYPE_ROW.THIRD_CURE];
                }
                else if (ii == Statics.Settings.PowerLevel.SecondCureQ - 1)
                {
                    restSettingsArray = (bool[])Statics.Settings.PowerLevel.RestingValuePerTypeList[(int)PL_REST_PER_TYPE_ROW.SECOND_CURE];
                }
                else if (ii == Statics.Settings.PowerLevel.FirstCureQ - 1)
                {
                    restSettingsArray = (bool[])Statics.Settings.PowerLevel.RestingValuePerTypeList[(int)PL_REST_PER_TYPE_ROW.FIRST_CURE];
                }
                else if (ii == Statics.Settings.PowerLevel.BuffQ - 1)
                {
                    restSettingsArray = (bool[])Statics.Settings.PowerLevel.RestingValuePerTypeList[(int)PL_REST_PER_TYPE_ROW.PARTY_BUFFS];
                }
                else if (ii == Statics.Settings.PowerLevel.SelfBuffQ - 1)
                {
                    restSettingsArray = (bool[])Statics.Settings.PowerLevel.RestingValuePerTypeList[(int)PL_REST_PER_TYPE_ROW.SELF_BUFFS];
                }
                else if (ii == Statics.Settings.PowerLevel.DebuffQ - 1)
                {
                    restSettingsArray = (bool[])Statics.Settings.PowerLevel.RestingValuePerTypeList[(int)PL_REST_PER_TYPE_ROW.DEBUFFS];
                }
                else if (ii == Statics.Settings.PowerLevel.WakeCureQ - 1)
                {
                    restSettingsArray = (bool[])Statics.Settings.PowerLevel.RestingValuePerTypeList[(int)PL_REST_PER_TYPE_ROW.WAKE_CURE];
                }
                else
                {
                    restSettingsArray[(int)PL_REST_ARR_INX.IN_FIGHT] = true;
                    restSettingsArray[(int)PL_REST_ARR_INX.STAND_LOW] = true;
                    restSettingsArray[(int)PL_REST_ARR_INX.STAND_HIGH] = true;
                    restSettingsArray[(int)PL_REST_ARR_INX.MP_LOW] = true;
                    restSettingsArray[(int)PL_REST_ARR_INX.MP_HIGH] = true;
                }
                LoggingFunctions.Debug("TopPL::initRestSettings: " + restSettingsArray[0] + ", " + restSettingsArray[1] + ", " + restSettingsArray[2] + ", " + restSettingsArray[3] + ", " + restSettingsArray[4] + ".", LoggingFunctions.DBG_SCOPE.PL);
                Statics.Settings.PowerLevel.RestingValueList.Add(restSettingsArray);
            }
            return true;
        }
        private bool PL_AddMe()
        {
            try
            {
                return (bool)CharacterGrid.Invoke(PL_addMePtr);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL_AddMe: Adding self to characters grid: " + e.ToString());
                return false;
            }
        }
        private bool PL_AddMeCBF()
        {
            PL_AddCharacter(PlayerCache.Vitals.Name);
            return true;
        }
        private void PL_Reset()
        {
            if (Bots.PowerLevel.Access.State != Bots.STATE.STOPPED)
            {
                Bots.PowerLevel.Access.Stop();
            }
            if (PL_CheckActiveThread != null)
            {
                PL_CheckActiveThread = null;
            }
            PL_chatDebuffParsing.Reset();
            PL_ClearDebuffGrid();
            PL_ClearBuffsGrid();
            PL_ClearSelfBuffsGrid();
            PL_ClearCuresGrid();
            Statics.Settings.PowerLevel.RestingValueList.Clear();
            Statics.Settings.PowerLevel.RestingValuePerTypeList.Clear();
            PL_ClearCharacterGrid();
            if (PL_CharacterList.Count > 0)
            {
                PL_CharacterList.Clear();
            }
            PL_FirstCureDefCmd = null;
            PL_SecondCureDefCmd = null;
            PL_ThirdCureDefCmd = null;
            PL_FirstCureDefCmdMaster = null;
            PL_SecondCureDefCmdMaster = null;
            PL_ThirdCureDefCmdMaster = null;
            PL_CuresInitDone = false;
            PL_PartyBuffCmdListMaster.Clear();
            PL_PartyBuffRecastListMaster.Clear();
            PL_PartyBuffEnableListMaster.Clear();
            PL_MyDebuffCmdListMaster.Clear();
            PL_MySelfBuffTaskListMaster.Clear();
        }
        private void PL_RefreshLists()
        {
            try
            {
                if (CuresGrid.InvokeRequired)
                {
                    Invoke(PL_refreshListsPtr);
                }
                else
                {
                    PL_RefreshListsCBF();
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL_RefreshLists:\n\r" + e.ToString());
            }
        }
        private void PL_RefreshListsCBF()
        {
            try
            {
                #region Save Player
                String savedPlayer = "-default-";
                DataGridViewSelectedCellCollection cellList = CharacterGrid.SelectedCells;
                foreach (DataGridViewCell cell in cellList)
                {
                    if (cell.ColumnIndex == (int)PL_CHAR_GRID_COL.NAME)
                    {
                        if ((String)cell.Value != "-default-")
                        {
                            savedPlayer = (String)cell.Value;
                            break;
                        }
                    }
                }
                #endregion Save Player
                #region Set Lists
                PL_CureCmdList = null;
                PL_CureCmdList = CommandManager.PL_CureCommands;
                if (PL_CureCmdList.Count == 0)
                {
                    PL_CureCmdList.Add(CommandManager.Dummy);
                }
                //Save the currently selected party buff in the combo box since we're resetting the list.
                //Command savedPartyBuffCmd = null;
                //if (PL_PartyBuffCB.DataSource != null)
                //{
                //    if (PL_PartyBuffCB.Items.Count > 0)
                //    {
                //        savedPartyBuffCmd = (Command)PL_PartyBuffCB.SelectedItem;
                //    }
                //}
                PL_PartyBuffCB.DataSource = null;
                PL_PartyBuffCmdList.Clear();
                PL_PartyBuffRecastList.Clear();
                PL_PartyBuffEnableList.Clear();
                PL_PartyBuffCB.DataSource = CommandManager.PL_PartyBuffsCommands;
                PL_PartyBuffCB.DisplayMember = "Name";
                //Command savedDebuffCmd = null;
                //if (PL_DebuffCB.DataSource != null)
                //{
                //    if (PL_DebuffCB.Items.Count > 0)
                //    {
                //        savedDebuffCmd = (Command)PL_DebuffCB.SelectedItem;
                //    }
                //}
                PL_MyDebuffCmdList.Clear();
                PL_DebuffCB.DataSource = null;
                PL_DebuffCB.DataSource = CommandManager.PL_HealingCommands;
                PL_DebuffCB.DisplayMember = "Name";
                //Command savedSelfBuffCmd = null;
                //if (PL_SelfBuffCB.DataSource != null)
                //{
                //    if (PL_SelfBuffCB.Items.Count > 0)
                //    {
                //        savedSelfBuffCmd = (Command)PL_SelfBuffCB.SelectedItem;
                //    }
                //}
                PL_SelfBuffCB.DataSource = null;
                PL_SelfBuffCB.DataSource = CommandManager.PL_SelfBuffsCommands;
                PL_SelfBuffCB.DisplayMember = "Name";
                PL_MySelfBuffTaskList.Clear();
                #endregion Set Lists
                #region Cures
                #region Init
                if (PL_CuresInitDone == false)
                {
                    //First set the ideal commands from the settings.
                    //Otherwise, if we log in on some other job, they'll be lost.
                    foreach (Command cmd in CommandManager.AllCommands)
                    {
                        if (cmd.Name == PL_FirstCureDefault)
                        {
                            PL_FirstCureDefCmdMaster = cmd;
                        }
                        if (cmd.Name == PL_SecondCureDefault)
                        {
                            PL_SecondCureDefCmdMaster = cmd;
                        }
                        if (cmd.Name == PL_ThirdCureDefault)
                        {
                            PL_ThirdCureDefCmdMaster = cmd;
                        }
                    }
                    //Now try to set the commands we'll use on this job.
                    foreach (Command cmd in CommandManager.PL_CureCommands)
                    {
                        if (cmd.Name == PL_FirstCureDefault)
                        {
                            PL_FirstCureDefCmd = cmd;
                        }
                        if (cmd.Name == PL_SecondCureDefault)
                        {
                            PL_SecondCureDefCmd = cmd;
                        }
                        if (cmd.Name == PL_ThirdCureDefault)
                        {
                            PL_ThirdCureDefCmd = cmd;
                        }
                    }
                    //Now set to dummy if any of them are still null.
                    if (PL_FirstCureDefCmdMaster == null)
                    {
                        PL_FirstCureDefCmdMaster = CommandManager.Dummy;
                    }
                    if (PL_SecondCureDefCmdMaster == null)
                    {
                        PL_SecondCureDefCmdMaster = CommandManager.Dummy;
                    }
                    if (PL_ThirdCureDefCmdMaster == null)
                    {
                        PL_ThirdCureDefCmdMaster = CommandManager.Dummy;
                    }
                    if (PL_FirstCureDefCmd == null)
                    {
                        PL_FirstCureDefCmd = CommandManager.Dummy;
                    }
                    if (PL_SecondCureDefCmd == null)
                    {
                        PL_SecondCureDefCmd = CommandManager.Dummy;
                    }
                    if (PL_ThirdCureDefCmd == null)
                    {
                        PL_ThirdCureDefCmd = CommandManager.Dummy;
                    }
                    PL_CuresInitDone = true;
                }
                if (PL_CureCmdList.Count == 0)
                {
                    PL_CureCmdList.Add(CommandManager.Dummy);
                }
                #endregion Init
                #region Default
                Boolean found_1st = false;
                Boolean found_2nd = false;
                Boolean found_3rd = false;
                foreach (Command cmd in PL_CureCmdList)
                {
                    if (cmd == PL_FirstCureDefCmdMaster)
                    {
                        found_1st = true;
                        PL_FirstCureDefCmd = cmd;
                        PL_FirstCureDefault = cmd.Name;
                    }
                    if (cmd == PL_SecondCureDefCmdMaster)
                    {
                        found_2nd = true;
                        PL_SecondCureDefCmd = cmd;
                        PL_SecondCureDefault = cmd.Name;
                    }
                    if (cmd == PL_ThirdCureDefCmdMaster)
                    {
                        found_3rd = true;
                        PL_ThirdCureDefCmd = cmd;
                        PL_ThirdCureDefault = cmd.Name;
                    }
                }
                if (!found_1st)
                {
                    //Our ideal first cure was not in our new list. Just select the last one.
                    PL_FirstCureDefCmd = PL_CureCmdList.Last();
                    PL_FirstCureDefault = PL_FirstCureDefCmd.Name;
                }
                if (!found_2nd)
                {
                    PL_SecondCureDefCmd = PL_CureCmdList.Last();
                    PL_SecondCureDefault = PL_FirstCureDefCmd.Name;
                }
                if (!found_3rd)
                {
                    PL_ThirdCureDefCmd = PL_CureCmdList.Last();
                    PL_ThirdCureDefault = PL_FirstCureDefCmd.Name;
                }
                #endregion Default
                #region Players
                foreach (PLCharacter chr in PL_CharacterList)
                {
                    found_1st = false;
                    found_2nd = false;
                    found_3rd = false;
                    foreach (Command cmd in PL_CureCmdList)
                    {
                        if (cmd == chr.FirstCureCommandMaster)
                        {
                            found_1st = true;
                            chr.FirstCureCommand = cmd;
                        }
                        if (cmd == chr.SecondCureCommandMaster)
                        {
                            found_2nd = true;
                            chr.SecondCureCommand = cmd;
                        }
                        if (cmd == chr.ThirdCureCommandMaster)
                        {
                            found_3rd = true;
                            chr.ThirdCureCommand = cmd;
                        }
                    }
                    if (!found_1st)
                    {
                        //Our ideal first cure was not in our new list. Just select the first one.
                        chr.FirstCureCommand = PL_CureCmdList.Last();
                    }
                    if (!found_2nd)
                    {
                        chr.SecondCureCommand = PL_CureCmdList.Last();
                    }
                    if (!found_3rd)
                    {
                        chr.ThirdCureCommand = PL_CureCmdList.Last();
                    }
                }
                #endregion Players
                #region Wake Cure
                //Set up wake cure command
                foreach (Command cmd in PL_CureCmdList)
                {
                    if (cmd.Name == Statics.Settings.PowerLevel.WakeCure)
                    {
                        PL_WakeCureCommand = cmd;
                        break;
                    }
                }
                if (PL_WakeCureCommand == null)
                {
                    PL_WakeCureCommand = CommandManager.Dummy;
                }
                #endregion Wake Cure
                #endregion Cures
                #region Party Buffs
                #region Default
                //Rebuild the default party buff lists based on the master lists.
                //The master lists came from the settings, so it should include everything we want.
                foreach (Command cmd in PL_PartyBuffCmdListMaster)
                {
                    if (CommandManager.PL_PartyBuffsCommands.Contains(cmd))
                    {
                        Int32 idx = PL_PartyBuffCmdListMaster.IndexOf(cmd);
                        PL_PartyBuffCmdList.Add(PL_PartyBuffCmdListMaster[idx]);
                        PL_PartyBuffRecastList.Add(PL_PartyBuffRecastListMaster[idx]);
                        PL_PartyBuffEnableList.Add(PL_PartyBuffEnableListMaster[idx]);
                    }
                }
                #endregion Default
                #region Players
                //Go through each player and prune their task list.
                foreach (PLCharacter chr in PL_CharacterList)
                {
                    for (int ii = 0; ii < chr.TaskListMaster.Count; ii++)
                    {
                        Boolean valid = CommandManager.PL_PartyBuffsCommands.Contains(chr.TaskListMaster[ii].Cmd);
                        Boolean found = false;
                        for (int kk = chr.TaskList.Count - 1; kk >= 0; kk--)
                        {
                            if (chr.TaskList[kk] == chr.TaskListMaster[ii])
                            {
                                found = true;
                                if (!valid)
                                {
                                    chr.TaskList.RemoveAt(kk);
                                }
                                break;
                            }
                        }
                        if (valid && !found)
                        {
                            chr.TaskList.Add(chr.TaskListMaster[ii]);
                        }
                    }
                }
                #endregion Players
                #endregion Party Buffs
                #region Debuffs
                foreach (Command debuffCmdMst in PL_MyDebuffCmdListMaster)
                {
                    foreach (Command debuffCmd in CommandManager.PL_HealingCommands)
                    {
                        if (debuffCmd == debuffCmdMst)
                        {
                            PL_MyDebuffCmdList.Add(debuffCmd);
                            continue;
                        }
                    }
                }
                #endregion Debuffs
                #region Self Buffs
                foreach (Task selfBuffTask in PL_MySelfBuffTaskListMaster)
                {
                    foreach (Command cmd in CommandManager.PL_SelfBuffsCommands)
                    {
                        if (cmd == selfBuffTask.Cmd)
                        {
                            PL_MySelfBuffTaskList.Add(selfBuffTask);
                            break;
                        }
                    }
                }
                #endregion Self Buffs
                #region Select Combo Box Items
                //if (PL_PartyBuffCB.Items.Count > 0)
                //{
                //    if (savedPartyBuffCmd != null)
                //    {
                //        if (CommandManager.PL_PartyBuffsCommands.Contains(savedPartyBuffCmd))
                //        {
                //            PL_PartyBuffCB.SelectedItem = savedPartyBuffCmd;
                //        }
                //    }
                //    else
                //    {
                //        PL_PartyBuffCB.SelectedIndex = 0;
                //    }
                //}
                //if (PL_DebuffCB.Items.Count > 0)
                //{
                //    if (savedDebuffCmd != null)
                //    {
                //        if (CommandManager.PL_HealingCommands.Contains(savedDebuffCmd))
                //        {
                //            PL_DebuffCB.SelectedItem = savedDebuffCmd;
                //        }
                //    }
                //    else
                //    {
                //        PL_DebuffCB.SelectedIndex = 0;
                //    }
                //}
                //if (PL_SelfBuffCB.Items.Count > 0)
                //{
                //    if (savedSelfBuffCmd != null)
                //    {
                //        if (CommandManager.PL_SelfBuffsCommands.Contains(savedSelfBuffCmd))
                //        {
                //            PL_SelfBuffCB.SelectedItem = savedSelfBuffCmd;
                //        }
                //    }
                //    else
                //    {
                //        PL_SelfBuffCB.SelectedIndex = 0;
                //    }
                //}
                #endregion Select Combo Box Items
                PL_ReloadGrids(savedPlayer);
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL_RefreshListsCBF:\n\r" + e.ToString());
            }
        }
        #endregion
        #region Character Grid Section
        private bool PL_InitCharacterGrid()
        {
            try
            {
                return (bool)CharacterGrid.Invoke(PL_initCharacterGridPtr);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL_InitCharacterGrid: Initializing character grid: " + e.ToString());
                return false;
            }
        }
        private bool PL_InitCharacterGridCBF()
        {
            //Init the character grid.  This is the parent grid of the others.
            PL_ClearCharacterGrid();
            foreach (DataGridViewColumn col in CharacterGrid.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            DataGridViewRow newRow = new DataGridViewRow();
            DataGridViewCell nameCell = new DataGridViewTextBoxCell();
            DataGridViewCell priCell = new DataGridViewTextBoxCell();
            DataGridViewCell actCell = new DataGridViewImageCell();

            nameCell.Value = "-default-";
            actCell.Value = Iocaine2.Properties.Resources.Blank;

            newRow.Cells.Insert((int)PL_CHAR_GRID_COL.PRI, priCell);
            newRow.Cells.Insert((int)PL_CHAR_GRID_COL.NAME, nameCell);
            newRow.Cells.Insert((int)PL_CHAR_GRID_COL.ACT, actCell);
            CharacterGrid.Rows.Add(newRow);

            CharacterGrid.EditMode = DataGridViewEditMode.EditOnEnter;  //required for edit on click.
            FollowUpDownBox.Minimum = 0;
            FollowUpDownBox.Maximum = 0;
            FollowDistUpDownBox.Value = Statics.Settings.PowerLevel.FollowDistance;
            FollowDistUpDownBox.Minimum = 1;
            FollowDistUpDownBox.Maximum = 20;
            return true;
        }
        private bool PL_AddCharacter(String iName)
        {
            try
            {
                if (CharacterGrid.InvokeRequired)
                {
                    return (bool)CharacterGrid.Invoke(PL_addCharacterPtr, new object[] { iName });
                }
                else
                {
                    return PL_AddCharacterCBF(iName);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL_AddCharacter: Adding character to grid: " + e.ToString());
                return false;
            }
        }
        private bool PL_AddCharacterCBF(String iName)
        {
            List<Task> localPartyBuffTaskList = new List<Task>();
            Int32 nbPartyBuffs = PL_PartyBuffCmdList.Count;
            for(int ii=0; ii<nbPartyBuffs; ii++)
            {
                localPartyBuffTaskList.Add(new Task(Task.TYPE.BUFF, iName, PL_PartyBuffCmdList[ii], PL_PartyBuffRecastList[ii], PL_PartyBuffEnableList[ii], Statics.Settings.PowerLevel.BuffQ));
            }
            PLCharacter newChar = new PLCharacter(iName, CharacterGrid.Rows.Count,
                                  PL_FirstCureDefCmdMaster, PL_SecondCureDefCmdMaster, PL_ThirdCureDefCmdMaster, PL_FirstCureDefaultPerc,
                                  PL_SecondCureDefaultPerc, PL_ThirdCureDefaultPerc, localPartyBuffTaskList);
            foreach (Command cmd in PL_PartyBuffCmdListMaster)
            {
                Boolean found = false;
                foreach(Task tsk in newChar.TaskList)
                {
                    if(tsk.CmdName == cmd.Name)
                    {
                        newChar.TaskListMaster.Add(tsk);
                        found = true;
                    }
                }
                if(!found)
                {
                    Int32 idx = PL_PartyBuffCmdListMaster.IndexOf(cmd);
                    Task taskToAdd = new Task(Task.TYPE.BUFF, iName, cmd, PL_PartyBuffRecastListMaster[idx], false, Statics.Settings.PowerLevel.BuffQ);
                    taskToAdd.Elapsed += new System.Timers.ElapsedEventHandler(Bots.PowerLevel.Access.RebuffTimer_Elapsed);
                    newChar.TaskListMaster.Add(taskToAdd);
                }
            }
            LoggingFunctions.Debug("TopPL::PL_AddCharacterCBF: Adding character " + newChar.Name + " with priority " + newChar.Priority.ToString() + ".", LoggingFunctions.DBG_SCOPE.PL);
            LoggingFunctions.Debug("TopPL::PL_AddCharacterCBF: 1st Cure: " + newChar.FirstCureCommand.ToString() + " at " + newChar.FirstCurePerc + "%", LoggingFunctions.DBG_SCOPE.PL);
            LoggingFunctions.Debug("TopPL::PL_AddCharacterCBF: 2nd Cure: " + newChar.SecondCureCommand.ToString() + " at " + newChar.SecondCurePerc + "%", LoggingFunctions.DBG_SCOPE.PL);
            LoggingFunctions.Debug("TopPL::PL_AddCharacterCBF: 3rd Cure: " + newChar.ThirdCureCommand.ToString() + " at " + newChar.ThirdCurePerc + "%", LoggingFunctions.DBG_SCOPE.PL);
            //if (iName != Statics.MyData.Name)
            //{
                foreach (Task task in newChar.TaskList)
                {
                    task.Elapsed += new System.Timers.ElapsedEventHandler(Bots.PowerLevel.Access.RebuffTimer_Elapsed);
                    LoggingFunctions.Debug("TopPL::PL_AddCharacterCBF: Adding " + task.Cmd.Name + " to event handler.", LoggingFunctions.DBG_SCOPE.PL);
                    LoggingFunctions.Debug("TopPL::PL_AddCharacterCBF: Timer interval is " + task.Interval + ".", LoggingFunctions.DBG_SCOPE.PL);
                    if (task.UseTimer)
                    {
                        Bots.PowerLevel.Access.EnqueueTask(task);  //Will get added to Q. When deq'd the timer will be restarted.
                    }
                }
            //}
            if (iName == PlayerCache.Vitals.Name)
            {
                foreach (Task tsk in PL_MySelfBuffTaskList)
                {
                    tsk.Elapsed += new System.Timers.ElapsedEventHandler(Bots.PowerLevel.Access.RebuffTimer_Elapsed);
                    LoggingFunctions.Debug("TopPL::addCharacterCallBackFunction: Adding " + tsk.Cmd.Name + " to event handler.", LoggingFunctions.DBG_SCOPE.PL);
                    LoggingFunctions.Debug("TopPL::addCharacterCallBackFunction: Timer interval is " + tsk.Interval.ToString() + ".", LoggingFunctions.DBG_SCOPE.PL);
                }
            }
            //Add character to character grid
            DataGridViewRow newRow = new DataGridViewRow();
            DataGridViewCell nameCell = new DataGridViewTextBoxCell();
            DataGridViewCell priCell = new DataGridViewTextBoxCell();
            DataGridViewCell actCell = new DataGridViewImageCell();
            int insertPoint;
            if (CharacterGrid.Rows.Count <= 2)
            {
                insertPoint = 0;
            }
            else
            {
                insertPoint = CharacterGrid.Rows.Count - 2;
            }
            priCell.Value = insertPoint + 1;
            nameCell.Value = newChar.Name;
            actCell.Value = newChar.Active ? Resources.GreenCheck : Resources.RedX;
            newRow.Cells.Insert((int)PL_CHAR_GRID_COL.PRI, priCell);
            newRow.Cells.Insert((int)PL_CHAR_GRID_COL.NAME, nameCell);
            newRow.Cells.Insert((int)PL_CHAR_GRID_COL.ACT, actCell);
            CharacterGrid.Rows.Insert(insertPoint, newRow);

            newChar.Index = insertPoint;

            //Add character to the character list
            PL_CharacterList.Add(newChar);
            //Make the host player sticky if list is > 1.
            if(PL_CharacterList.Count > 1)
            {
                PL_CharacterSticky = PlayerCache.Vitals.Name;
            }

            //Update the priority of the last person before default
            if (CharacterGrid.Rows.Count > 2)
            {
                foreach (PLCharacter chr in PL_CharacterList)
                {
                    if (chr.Name == (String)CharacterGrid[(int)PL_CHAR_GRID_COL.NAME, CharacterGrid.Rows.Count - 2].Value)
                    {
                        //LoggingFunctions.debug("Character " + chr.Name + " matched cell " + (String)CharacterGrid[(int)CHAR_GRID_COL.NAME, CharacterGrid.Rows.Count - 2].Value + " and setting priority cell value to " + (CharacterGrid.Rows.Count - 1).ToString());
                        CharacterGrid[(int)PL_CHAR_GRID_COL.PRI, CharacterGrid.Rows.Count - 2].Value = CharacterGrid.Rows.Count - 1;
                        chr.Priority = CharacterGrid.Rows.Count - 2;
                    }
                }
            }
            Statics.Settings.PowerLevel.CharacterGridRowCount = CharacterGrid.Rows.Count;
            FollowUpDownBox.Maximum = CharacterGrid.Rows.Count - 1;
            PL_SetFollowCharacter();
            PL_SetFocusCharacter();
            return true;
        }
        private void PL_CharacterGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            if ((e.RowIndex < CharacterGrid.Rows.Count) && (e.RowIndex >= 0) && (e.ColumnIndex < CharacterGrid.Columns.Count) && (e.ColumnIndex >= 0))
            {
                LoggingFunctions.Debug("PL_CharacterGrid_CellClick: Entering grid cell click.", LoggingFunctions.DBG_SCOPE.PL);
                int selectedIndex = e.RowIndex;
                LoggingFunctions.Debug("PL_CharacterGrid_CellClick: Row index is " + selectedIndex.ToString() + ".", LoggingFunctions.DBG_SCOPE.PL);
                String selectedChar = System.Convert.ToString(CharacterGrid[(int)PL_CHAR_GRID_COL.NAME, e.RowIndex].Value);
                LoggingFunctions.Debug("PL_CharacterGrid_CellClick: Found selected character " + selectedChar + ".", LoggingFunctions.DBG_SCOPE.PL);
                PL_ReloadGrids(selectedChar);
            }
        }
        private void PL_CharBoxAddTargetButton_Click(object sender, EventArgs e)
        {
            String charToAdd = MemReads.Target.get_name();
            //charToAdd = charToAdd.Substring(0, charToAdd.Length - 1);
            LoggingFunctions.Timestamp("Adding character: " + charToAdd);
            if (charToAdd == "")
            {
                LoggingFunctions.Timestamp("Could not add character to list, nothing targeted.");
            }
            else
            {
                PL_AddCharacter(charToAdd);
            }
        }
        private void PL_CharBoxAddButton_Click(object sender, EventArgs e)
        {
            String charToAdd = AddCharTextBox.Text;
            if ((charToAdd == "") || (charToAdd == "Enter name or just target and click Add Target"))
            {
                LoggingFunctions.Timestamp("Could not add character to list, nothing typed in text box");
            }
            else
            {
                AddCharTextBox.Text = "";
                charToAdd = charToAdd.ToLower();
                charToAdd = char.ToUpper(charToAdd[0]) + charToAdd.Substring(1);
                LoggingFunctions.Timestamp("Adding PL character: " + charToAdd);
                PL_AddCharacter(charToAdd);
            }
        }
        private void PL_CharBoxAddPartyButton_Click(object sender, EventArgs e)
        {
            List<String> memberList = MemReads.Party.get_members();
            foreach (String str in memberList)
            {
                LoggingFunctions.Timestamp("Adding PL character: '" + str + "'");
                PL_AddCharacter(str);
            }
        }
        private void PL_AddCharTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                PL_CharBoxAddButton_Click(sender, e);
                e.Handled = true;
            }
        }
        private void PL_AddCharTextBox_Click(object sender, EventArgs e)
        {
            AddCharTextBox.Text = "";
        }
        private void PL_CharBoxRemoveButton_Click(object sender, EventArgs e)
        {
            if (CharacterGrid.SelectedCells.Count > 0)
            {
                foreach (DataGridViewCell cell in CharacterGrid.SelectedCells)
                {
                    if (cell.ColumnIndex == (int)PL_CHAR_GRID_COL.NAME)
                    {
                        if ((String)cell.Value != "-default-")
                        {
                            PL_RemoveCharacterCBF((String)cell.Value, cell.RowIndex);
                            break;
                        }
                    }
                }
            }
        }
        private void PL_RemoveCharacter(String iName, int iRowIndex)
        {
            try
            {
                if (CharacterGrid.InvokeRequired)
                {
                    CharacterGrid.Invoke(PL_removeCharacterPtr, new object[] { iName, iRowIndex });
                }
                else
                {
                    PL_RemoveCharacterCBF(iName, iRowIndex);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL_RemoveCharacter: Removing character from grid: " + e.ToString());
            }
        }
        private void PL_RemoveCharacterCBF(String iName, int iRowIndex)
        {
            if (iName == "-default-")
            {
                CharacterGrid.Rows.RemoveAt(iRowIndex);
            }
            else
            {
                foreach (PLCharacter chr in PL_CharacterList)
                {
                    if (chr.Name == iName)
                    {
                        chr.DisposeTasks();
                        PL_CharacterList.Remove(chr);
                        CharacterGrid.Rows.RemoveAt(iRowIndex);
                        break;
                    }
                }
            }
            //Now for each row we need to insert the correct priority and change the character's priority, too
            foreach (DataGridViewRow row in CharacterGrid.Rows)
            {
                if ((String)row.Cells[(int)PL_CHAR_GRID_COL.NAME].Value != "-default-")
                {
                    row.Cells[(int)PL_CHAR_GRID_COL.PRI].Value = row.Index + 1;
                    foreach (PLCharacter chr in PL_CharacterList)
                    {
                        if (chr.Name == (String)row.Cells[(int)PL_CHAR_GRID_COL.NAME].Value)
                        {
                            chr.Priority = row.Index + 1;
                            LoggingFunctions.Debug("PL_RemoveCharacterCBF: " + chr.Name + "'s new pri is " + chr.Priority.ToString() + ".", LoggingFunctions.DBG_SCOPE.PL);
                        }
                    }
                }
            }
            //Unsticky the host player's name if we've removed all the other players.
            if(PL_CharacterList.Count <= 1)
            {
                PL_CharacterSticky = "";
            }
        }
        private void PL_ClearCharacterGrid()
        {
            int nbRows = CharacterGrid.Rows.Count;
            for (int ii = nbRows-1; ii >= 0; ii--)
            {
                PL_RemoveCharacter((String)CharacterGrid[(int)PL_CHAR_GRID_COL.NAME, ii].Value, ii);
            }
        }
        private void PL_Player_Up_Button_Click(object sender, EventArgs e)
        {
            //Get a group of selected cells & find first/last row indexes
            DataGridViewSelectedCellCollection cellList = CharacterGrid.SelectedCells;
            int firstRow = CharacterGrid.Rows.Count;
            int lastRow = 0;
            if (cellList.Count == 0)
            {
                return;
            }
            foreach (DataGridViewCell cell in cellList)
            {
                if (cell.ColumnIndex == (int)PL_CHAR_GRID_COL.NAME)
                {
                    if ((String)cell.Value != "-default-")
                    {
                        if (cell.RowIndex < firstRow)
                        {
                            firstRow = cell.RowIndex;
                        }
                        if (cell.RowIndex > lastRow)
                        {
                            lastRow = cell.RowIndex;
                        }
                    }
                }
            }
            if ((firstRow == 0) || (lastRow == CharacterGrid.Rows.Count - 1) || (firstRow == CharacterGrid.Rows.Count))
            {
                return;
            }
            //Find the row right on top of them, if first row selected, do nothing
            int nextAbove = firstRow - 1;
            //Move the row just in front to just behind
            DataGridViewRow rowToMove = (DataGridViewRow)CharacterGrid.Rows[nextAbove].Clone();
            CharacterGrid.Rows.InsertCopy(nextAbove, lastRow + 1);
            CharacterGrid.Rows[lastRow + 1].Cells[(int)PL_CHAR_GRID_COL.NAME].Value = CharacterGrid.Rows[nextAbove].Cells[(int)PL_CHAR_GRID_COL.NAME].Value;
            CharacterGrid.Rows[lastRow + 1].Cells[(int)PL_CHAR_GRID_COL.ACT].Value = CharacterGrid.Rows[nextAbove].Cells[(int)PL_CHAR_GRID_COL.ACT].Value;
            CharacterGrid.Rows.RemoveAt(nextAbove);
            //Now for each row we need to insert the correct priority and change the character's priority, too
            foreach (DataGridViewRow row in CharacterGrid.Rows)
            {
                if ((String)row.Cells[(int)PL_CHAR_GRID_COL.NAME].Value != "-default-")
                {
                    row.Cells[(int)PL_CHAR_GRID_COL.PRI].Value = row.Index + 1;
                    foreach (PLCharacter chr in PL_CharacterList)
                    {
                        if (chr.Name == (String)row.Cells[(int)PL_CHAR_GRID_COL.NAME].Value)
                        {
                            chr.Priority = row.Index + 1;
                            LoggingFunctions.Debug("PL_Player_Up_Button_Click: " + chr.Name + "'s new pri is " + chr.Priority.ToString() + ".", LoggingFunctions.DBG_SCOPE.PL);
                        }
                    }
                }
            }
        }
        private void PL_Player_Dn_Button_Click(object sender, EventArgs e)
        {
            //Get a group of selected cells & find first/last row indexes
            DataGridViewSelectedCellCollection cellList = CharacterGrid.SelectedCells;
            int firstRow = CharacterGrid.Rows.Count;
            int lastRow = 0;
            if (cellList.Count == 0)
            {
                return;
            }
            foreach (DataGridViewCell cell in cellList)
            {
                if (cell.ColumnIndex == (int)PL_CHAR_GRID_COL.NAME)
                {
                    if ((String)cell.Value != "-default-")
                    {
                        if (cell.RowIndex < firstRow)
                        {
                            firstRow = cell.RowIndex;
                        }
                        if (cell.RowIndex > lastRow)
                        {
                            lastRow = cell.RowIndex;
                        }
                    }
                }
            }
            if ((lastRow == CharacterGrid.Rows.Count - 2) || (lastRow == CharacterGrid.Rows.Count - 1) || (firstRow == CharacterGrid.Rows.Count))
            {
                return;
            }
            //Find the row right below them
            int nextAbove = firstRow;
            int nextBelow = lastRow + 1;
            //Move the cell just below to the index of the first cell in selection
            DataGridViewRow rowToMove = (DataGridViewRow)CharacterGrid.Rows[nextBelow].Clone();
            CharacterGrid.Rows.InsertCopy(nextBelow, firstRow);
            CharacterGrid.Rows[nextAbove].Cells[(int)PL_CHAR_GRID_COL.NAME].Value = CharacterGrid.Rows[nextBelow + 1].Cells[(int)PL_CHAR_GRID_COL.NAME].Value;
            CharacterGrid.Rows[nextAbove].Cells[(int)PL_CHAR_GRID_COL.ACT].Value = CharacterGrid.Rows[nextBelow + 1].Cells[(int)PL_CHAR_GRID_COL.ACT].Value;
            CharacterGrid.Rows.RemoveAt(nextBelow + 1);
            //Now for each row we need to insert the correct priority and change the character's priority, too
            foreach (DataGridViewRow row in CharacterGrid.Rows)
            {
                if ((String)row.Cells[(int)PL_CHAR_GRID_COL.NAME].Value != "-default-")
                {
                    row.Cells[(int)PL_CHAR_GRID_COL.PRI].Value = row.Index + 1;
                    foreach (PLCharacter chr in PL_CharacterList)
                    {
                        if (chr.Name == (String)row.Cells[(int)PL_CHAR_GRID_COL.NAME].Value)
                        {
                            chr.Priority = row.Index + 1;
                            LoggingFunctions.Debug("PL_Player_Dn_Button_Click: " + chr.Name + "'s new pri is " + chr.Priority.ToString() + ".", LoggingFunctions.DBG_SCOPE.PL);
                        }
                    }
                }
            }
        }
        private void PL_SetFollowCharacter()
        {
            if ((int)FollowUpDownBox.Value == 0)
            {
                PL_FollowCharacter = null;
                Statics.Settings.PowerLevel.FollowCharacterIndex = -1;
            }
            else
            {
                if (CharacterGrid.Rows.Count > 2)
                {
                    String name = (String)CharacterGrid[(int)PL_CHAR_GRID_COL.NAME, (int)FollowUpDownBox.Value-1].Value;
                    if (name == PlayerCache.Vitals.Name)
                    {
                        PL_FollowCharacter = null;
                        Statics.Settings.PowerLevel.FollowCharacterIndex = -1;
                        return;
                    }
                    foreach (PLCharacter chr in PL_CharacterList)
                    {
                        if (name == chr.Name)
                        {
                            PL_FollowCharacter = chr;
                            Statics.Settings.PowerLevel.FollowCharacterIndex = PL_CharacterList.IndexOf(chr);
                            break;
                        }
                    }
                }
                else
                {
                    PL_FollowCharacter = null;
                    Statics.Settings.PowerLevel.FollowCharacterIndex = -1;
                }
            }
        }
        private void PL_SetFocusCharacter()
        {
            if ((CharacterGrid.Rows.Count > 2) && (Statics.Settings.PowerLevel.FocusCharacterRow <= CharacterGrid.Rows.Count - 2))
            {
                String name = (String)CharacterGrid[(int)PL_CHAR_GRID_COL.NAME, (Statics.Settings.PowerLevel.FocusCharacterRow - 1)].Value;
                if (name == PlayerCache.Vitals.Name)
                {
                    name = (String)CharacterGrid[(int)PL_CHAR_GRID_COL.NAME, Statics.Settings.PowerLevel.FocusCharacterRow].Value;
                }
                foreach (PLCharacter chr in PL_CharacterList)
                {
                    if (name == chr.Name)
                    {
                        PL_FocusCharacter = chr;
                        Statics.Settings.PowerLevel.FocusCharacterIndex = PL_CharacterList.IndexOf(chr);
                        break;
                    }
                }
            }
            else
            {
                //Focus on 'me' (hopefully this works)
                PL_FocusCharacter = (PLCharacter)PL_CharacterList[0];
                Statics.Settings.PowerLevel.FocusCharacterIndex = 0;
                Statics.Settings.PowerLevel.FocusCharacterRow = 1;
            }
        }
        #endregion
        #region Cures Grid Section
        private bool PL_LoadCuresGrid(String iName = "-default-")
        {
            try
            {
                return (bool)CuresGrid.Invoke(PL_loadCuresGridPtr, new object[] { iName });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL_InitCuresGrid: Doing Cures Grid inits: " + e.ToString());
                return false;
            }
        }
        private bool PL_LoadCuresGridCBF(String iName)
        {
            //Load the Cures grid.
            PLCharacter selChar = null;
            Boolean loadDefault = iName == "-default-";
            Command local_1stCure = null;
            Command local_2ndCure = null;
            Command local_3rdCure = null;
            Byte local_1stCurePerc = 0;
            Byte local_2ndCurePerc = 0;
            Byte local_3rdCurePerc = 0;
            PL_ClearCuresGrid();
            if (!loadDefault)
            {
                foreach (PLCharacter chr in PL_CharacterList)
                {
                    if (chr.Name == iName)
                    {
                        selChar = chr;
                        local_1stCure = selChar.FirstCureCommand;
                        local_2ndCure = selChar.SecondCureCommand;
                        local_3rdCure = selChar.ThirdCureCommand;
                        local_1stCurePerc = selChar.FirstCurePerc;
                        local_2ndCurePerc = selChar.SecondCurePerc;
                        local_3rdCurePerc = selChar.ThirdCurePerc;
                        break;
                    }
                }
                if((local_1stCure == null) || (local_2ndCure == null) || (local_3rdCure == null))
                {
                    LoggingFunctions.Error("PL_InitCuresGridCBF: Could not find character " + iName + ".");
                    return false;
                }
            }
            else
            {
                local_1stCure = PL_FirstCureDefCmd;
                local_2ndCure = PL_SecondCureDefCmd;
                local_3rdCure = PL_ThirdCureDefCmd;
                local_1stCurePerc = PL_FirstCureDefaultPerc;
                local_2ndCurePerc = PL_SecondCureDefaultPerc;
                local_3rdCurePerc = PL_ThirdCureDefaultPerc;
                LoggingFunctions.Debug("TopPL::PL_InitCuresGridCBF: firstCureDefault: " + PL_FirstCureDefault + ".", LoggingFunctions.DBG_SCOPE.PL);
                LoggingFunctions.Debug("TopPL::PL_InitCuresGridCBF: secondCureDefault: " + PL_SecondCureDefault + ".", LoggingFunctions.DBG_SCOPE.PL);
                LoggingFunctions.Debug("TopPL::PL_InitCuresGridCBF: thirdCureDefault: " + PL_ThirdCureDefault + ".", LoggingFunctions.DBG_SCOPE.PL);
            }
            for (int ii = 1; ii <= 3; ii++)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                DataGridViewCell cureCell = new DataGridViewTextBoxCell();
                DataGridViewComboBoxCell whichCell = new DataGridViewComboBoxCell();
                DataGridViewCell cureHPPercCell = new DataGridViewTextBoxCell();

                foreach (Command cmd in PL_CureCmdList)
                {
                    whichCell.Items.Add(cmd);
                }
                whichCell.ValueMember = "Name";
                whichCell.DisplayMember = "Name";
                Int32 cureIndex = -1;
                switch (ii)
                {
                    case 1:
                        cureCell.Value = "1st Cure";
                        cureHPPercCell.Value = local_1stCurePerc;
                        cureIndex = whichCell.Items.IndexOf(local_1stCure);
                        if(cureIndex != -1)
                        {
                            whichCell.Value = whichCell.Items[cureIndex];
                        }
                        break;
                    case 2:
                        cureCell.Value = "2nd Cure";
                        cureHPPercCell.Value = local_2ndCurePerc;
                        cureIndex = whichCell.Items.IndexOf(local_2ndCure);
                        if(cureIndex != -1)
                        {
                            whichCell.Value = whichCell.Items[cureIndex];
                        }
                        break;
                    case 3:
                        cureCell.Value = "3rd Cure";
                        cureHPPercCell.Value = local_3rdCurePerc;
                        cureIndex = whichCell.Items.IndexOf(local_3rdCure);
                        if(cureIndex != -1)
                        {
                            whichCell.Value = whichCell.Items[cureIndex];
                        }
                        break;
                }
                newRow.Cells.Add(cureCell);
                newRow.Cells.Add(whichCell);
                newRow.Cells.Add(cureHPPercCell);
                CuresGrid.Rows.Add(newRow);
            }
            CuresGrid.EditMode = DataGridViewEditMode.EditOnEnter;  //required for edit on click.
            LoggingFunctions.Debug("TopPL::PL_InitCuresGridCBF: Cures grid initialized ok.", LoggingFunctions.DBG_SCOPE.PL);
            return true;
        }
        private void PL_CuresGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewSelectedCellCollection cellList = CharacterGrid.SelectedCells;
            foreach (DataGridViewCell cell in cellList)
            {
                if (cell.ColumnIndex == (int)PL_CHAR_GRID_COL.NAME)
                {
                    if ((String)cell.Value == "-default-")
                    {
                        if (e.ColumnIndex == (int)PL_CURE_GRID_COL.WHICH)
                        {
                            String newCmdName = (String)CuresGrid[(int)PL_CURE_GRID_COL.WHICH, e.RowIndex].Value;
                            Command newCmd = null;
                            foreach (Command cmd in PL_CureCmdList)
                            {
                                if (cmd.Name == newCmdName)
                                {
                                    newCmd = cmd;
                                    break;
                                }
                            }
                            if (newCmd == null)
                            {
                                LoggingFunctions.Error("PL_CuresGrid_CellValueChanged: Could not find cure default item: " + newCmdName + ".");
                                return;
                            }
                            LoggingFunctions.Debug("TopPL::CuresGrid_CellValueChanged: New value is " + newCmd.Name + ".", LoggingFunctions.DBG_SCOPE.PL);
                            if (e.RowIndex == 0)
                            {
                                PL_FirstCureDefault = newCmd.Name;
                                PL_FirstCureDefCmd = newCmd;
                                PL_FirstCureDefCmdMaster = newCmd;
                                UserSettings.SetValue(UserSettings.BOT.PL, "FirstCureDefault", PL_FirstCureDefault);
                                LoggingFunctions.Debug("TopPL::CuresGrid_CellValueChanged: New first cure index is " + PL_FirstCureDefCmd.ToString() + ".", LoggingFunctions.DBG_SCOPE.PL);
                            }
                            else if (e.RowIndex == 1)
                            {
                                PL_SecondCureDefault = newCmd.Name;
                                PL_SecondCureDefCmd = newCmd;
                                PL_SecondCureDefCmdMaster = newCmd;
                                UserSettings.SetValue(UserSettings.BOT.PL, "SecondCureDefault", PL_SecondCureDefault);
                                LoggingFunctions.Debug("TopPL::CuresGrid_CellValueChanged: New second cure index is " + PL_SecondCureDefCmd.ToString() + ".", LoggingFunctions.DBG_SCOPE.PL);
                            }
                            else if (e.RowIndex == 2)
                            {
                                PL_ThirdCureDefault = newCmd.Name;
                                PL_ThirdCureDefCmd = newCmd;
                                PL_ThirdCureDefCmdMaster = newCmd;
                                UserSettings.SetValue(UserSettings.BOT.PL, "ThirdCureDefault", PL_ThirdCureDefault);
                                LoggingFunctions.Debug("TopPL::CuresGrid_CellValueChanged: New third cure index is " + PL_ThirdCureDefCmd.ToString() + ".", LoggingFunctions.DBG_SCOPE.PL);
                            }
                        }
                        else if (e.ColumnIndex == (int)PL_CURE_GRID_COL.CUREHPPERC)
                        {
                            if (e.RowIndex == 0)
                            {
                                PL_FirstCureDefaultPerc = System.Convert.ToByte(CuresGrid[(int)PL_CURE_GRID_COL.CUREHPPERC, e.RowIndex].Value);
                                UserSettings.SetValue(UserSettings.BOT.PL, "FirstCureDefaultPerc", PL_FirstCureDefaultPerc.ToString());
                                LoggingFunctions.Timestamp("New hp perc value is " + PL_FirstCureDefaultPerc.ToString());
                            }
                            else if (e.RowIndex == 1)
                            {
                                PL_SecondCureDefaultPerc = System.Convert.ToByte(CuresGrid[(int)PL_CURE_GRID_COL.CUREHPPERC, e.RowIndex].Value);
                                UserSettings.SetValue(UserSettings.BOT.PL, "SecondCureDefaultPerc", PL_SecondCureDefaultPerc.ToString());
                                LoggingFunctions.Timestamp("New hp perc value is " + PL_SecondCureDefaultPerc.ToString());
                            }
                            else if (e.RowIndex == 2)
                            {
                                PL_ThirdCureDefaultPerc = System.Convert.ToByte(CuresGrid[(int)PL_CURE_GRID_COL.CUREHPPERC, e.RowIndex].Value);
                                UserSettings.SetValue(UserSettings.BOT.PL, "ThirdCureDefaultPerc", PL_ThirdCureDefaultPerc.ToString());
                                LoggingFunctions.Timestamp("New hp perc value is " + PL_ThirdCureDefaultPerc.ToString());
                            }
                        }
                    }
                    else
                    {
                        PLCharacter selChar = null;
                        foreach (PLCharacter player in PL_CharacterList)
                        {
                            if (player.Name == (String)cell.Value)
                            {
                                selChar = player;
                            }
                        }
                        if (e.ColumnIndex == (int)PL_CURE_GRID_COL.WHICH)
                        {
                            String newCmdName = (String)CuresGrid[(int)PL_CURE_GRID_COL.WHICH, e.RowIndex].Value;
                            Command newCmd = null;
                            foreach (Command cmd in PL_CureCmdList)
                            {
                                if (cmd.Name == newCmdName)
                                {
                                    newCmd = cmd;
                                    break;
                                }
                            }
                            if (newCmd == null)
                            {
                                LoggingFunctions.Error("PL_CuresGrid_CellValueChanged: Could not find cure default item: " + newCmdName + ".");
                                return;
                            }
                            if (e.RowIndex == 0)
                            {
                                selChar.FirstCureCommand = newCmd;
                                LoggingFunctions.Timestamp("New 1st cure cmd for " + selChar.Name + " is " + selChar.FirstCureCommand.Name);
                            }
                            else if (e.RowIndex == 1)
                            {
                                selChar.SecondCureCommand = newCmd;
                                LoggingFunctions.Timestamp("New 2nd cure cmd for " + selChar.Name + " is " + selChar.SecondCureCommand.Name);
                            }
                            else if (e.RowIndex == 2)
                            {
                                selChar.ThirdCureCommand = newCmd;
                                LoggingFunctions.Timestamp("New 3rd cure cmd for " + selChar.Name + " is " + selChar.ThirdCureCommand.Name);
                            }
                        }
                        else if (e.ColumnIndex == (int)PL_CURE_GRID_COL.CUREHPPERC)
                        {
                            if (e.RowIndex == 0)
                            {
                                selChar.FirstCurePerc = System.Convert.ToByte(CuresGrid[(int)PL_CURE_GRID_COL.CUREHPPERC, e.RowIndex].Value);
                                LoggingFunctions.Timestamp("New 1st hp perc value for " + selChar.Name + " is " + selChar.FirstCurePerc.ToString());
                            }
                            else if (e.RowIndex == 1)
                            {
                                selChar.SecondCurePerc = System.Convert.ToByte(CuresGrid[(int)PL_CURE_GRID_COL.CUREHPPERC, e.RowIndex].Value);
                                LoggingFunctions.Timestamp("New 2nd hp perc value for " + selChar.Name + " is " + selChar.SecondCurePerc.ToString());
                            }
                            else if (e.RowIndex == 2)
                            {
                                selChar.ThirdCurePerc = System.Convert.ToByte(CuresGrid[(int)PL_CURE_GRID_COL.CUREHPPERC, e.RowIndex].Value);
                                LoggingFunctions.Timestamp("New 3rd hp perc value for " + selChar.Name + " is " + selChar.ThirdCurePerc.ToString());
                            }
                        }
                    }

                }
            }
        }
        private void PL_ClearCuresGrid()
        {
            try
            {
                if (CuresGrid.InvokeRequired)
                {
                    CuresGrid.Invoke(PL_clearCuresGridPtr);
                }
                else
                {
                    PL_ClearCuresGridCBF();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL_ClearCuresGrid: Clearing cures grid: " + e.ToString());
            }
        }
        private void PL_ClearCuresGridCBF()
        {
            CuresGrid.Rows.Clear();
        }
        #endregion
        #region Buffs Grid Section
        private bool PL_LoadBuffsGrid(String iName = "-default-")
        {
            try
            {
                return (bool)PL_BuffsGrid.Invoke(PL_loadBuffsGridPtr, new object[] { iName });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL_LoadBuffsGrid: Doing Buffs Grid load: " + e.ToString());
                return false;
            }
        }
        private bool PL_LoadBuffsGridCBF(String iName)
        {
            PL_BuffsGrid.Rows.Clear();
            if (iName == "-default-")
            {
                for (int ii = 0; ii < PL_PartyBuffCmdList.Count; ii++)
                {
                    DataGridViewRow newRow = new DataGridViewRow();
                    DataGridViewCell partyBuffCell = new DataGridViewTextBoxCell();
                    DataGridViewCell enCell = new DataGridViewCheckBoxCell();
                    DataGridViewCell recastCell = new DataGridViewTextBoxCell();
                    int insertPoint = PL_BuffsGrid.Rows.Count;
                    partyBuffCell.Value = PL_PartyBuffCmdList[ii];
                    enCell.Value = PL_PartyBuffEnableList[ii];
                    recastCell.Value = PL_PartyBuffRecastList[ii] / 1000;

                    newRow.Cells.Insert((int)PL_SELF_BUFF_COL.BUFF, partyBuffCell);
                    newRow.Cells.Insert((int)PL_SELF_BUFF_COL.EN, enCell);
                    newRow.Cells.Insert((int)PL_SELF_BUFF_COL.RECAST, recastCell);
                    PL_BuffsGrid.Rows.Insert(insertPoint, newRow);
                }
                //PL_BuffsGrid.EditMode = DataGridViewEditMode.EditOnEnter;  //required for edit on click.
                LoggingFunctions.Debug("TopPL::PL_InitBuffsGridCBF: Buffs grid initialized ok.", LoggingFunctions.DBG_SCOPE.PL);
            }
            else
            {
                PLCharacter selChr = null;
                foreach (PLCharacter chr in PL_CharacterList)
                {
                    if (chr.Name == iName)
                    {
                        selChr = chr;
                        break;
                    }
                }
                if (selChr == null)
                {
                    LoggingFunctions.Error("PL_InitBuffsGridCBF: Could not find character: " + iName + ".");
                    return false;
                }
                foreach (Task tsk in selChr.TaskList)
                {
                    DataGridViewRow newRow = new DataGridViewRow();
                    DataGridViewCell partyBuffCell = new DataGridViewTextBoxCell();
                    DataGridViewCell enCell = new DataGridViewCheckBoxCell();
                    DataGridViewCell recastCell = new DataGridViewTextBoxCell();
                    int insertPoint = PL_BuffsGrid.Rows.Count;
                    partyBuffCell.Value = tsk.Cmd;
                    enCell.Value = tsk.UseTimer;
                    recastCell.Value = tsk.Interval / 1000;

                    newRow.Cells.Insert((int)PL_SELF_BUFF_COL.BUFF, partyBuffCell);
                    newRow.Cells.Insert((int)PL_SELF_BUFF_COL.EN, enCell);
                    newRow.Cells.Insert((int)PL_SELF_BUFF_COL.RECAST, recastCell);
                    PL_BuffsGrid.Rows.Insert(insertPoint, newRow);
                }
            }
            return true;
        }
        private void PL_PartyBuffAddButton_Click(object sender, EventArgs e)
        {
            if(PL_PartyBuffCB.SelectedIndex < 0)
            {
                return;
            }
            else if(PL_PartyBuffCB.SelectedItem == null)
            {
                return;
            }
            Boolean playerSelected = false;
            Boolean defaultSelected = false;
            PLCharacter selChar = null;
            PLCharacter firstSelChar = null;
            DataGridViewSelectedCellCollection cellList = CharacterGrid.SelectedCells;
            foreach (DataGridViewCell cell in cellList)
            {
                if (cell.ColumnIndex == (int)PL_CHAR_GRID_COL.NAME)
                {
                    //We are saving these new values to the user settings
                    if ((String)cell.Value == "-default-")
                    {
                        Command buffCmd = (Command)PL_PartyBuffCB.SelectedItem;
                        PL_PartyBuffCmdList.Add(buffCmd);
                        PL_PartyBuffRecastList.Add(buffCmd.Duration);
                        PL_PartyBuffEnableList.Add(false);
                        if (!PL_PartyBuffCmdListMaster.Contains(buffCmd))
                        {
                            PL_PartyBuffCmdListMaster.Add(buffCmd);
                            PL_PartyBuffRecastListMaster.Add(buffCmd.Duration);
                            PL_PartyBuffEnableListMaster.Add(false);
                        }
                        UserSettings.SetListValue(UserSettings.BOT.PL, UserSettings.LIST_TABLE.PL_PARTYBUFFS, new List<object>() { Data.Client.FfxiResource.EncodeApostrophy(buffCmd.Name), buffCmd.Duration.ToString() });
                        defaultSelected = true;
                    }
                    else
                    {
                        foreach (PLCharacter player in PL_CharacterList)
                        {
                            if (player.Name == (String)cell.Value)
                            {
                                selChar = player;
                                if(!playerSelected)
                                {
                                    playerSelected = true;
                                    firstSelChar = player;
                                }
                                Command buffCmd = (Command)PL_PartyBuffCB.SelectedItem;
                                Boolean exists = false;
                                for (int ii = 0; ii < selChar.TaskList.Count; ii++)
                                {
                                    if(selChar.TaskList[ii].Cmd.Name == buffCmd.Name)
                                    {
                                        LoggingFunctions.Timestamp("Could not add new buff '" + buffCmd.Name + "' to " + selChar.Name + "'s buff list (already exists).");
                                        exists = true;
                                        break;
                                    }
                                }
                                if(!exists)
                                {
                                    //Check the master list for the player. If it's there, just add the same task to the task list.
                                    //If it's not there either, create a new task and add it to both lists.
                                    Task taskToAdd = null;
                                    for (int ii = 0; ii < selChar.TaskListMaster.Count; ii++)
                                    {
                                        if (selChar.TaskListMaster[ii].Cmd.Name == buffCmd.Name)
                                        {
                                            exists = true;
                                            taskToAdd = selChar.TaskListMaster[ii];
                                            break;
                                        }
                                    }
                                    if (!exists || (taskToAdd == null))
                                    {
                                        taskToAdd = new Task(Task.TYPE.BUFF, selChar.Name, buffCmd, buffCmd.Duration, false, Statics.Settings.PowerLevel.BuffQ);
                                        taskToAdd.Elapsed += new System.Timers.ElapsedEventHandler(Bots.PowerLevel.Access.RebuffTimer_Elapsed);
                                    }
                                    selChar.TaskList.Add(taskToAdd);
                                }
                            }
                        }
                    }
                }
            }
            if(playerSelected)
            {
                PL_ReloadGrids(firstSelChar.Name);
            }
            else if(defaultSelected)
            {
                PL_ReloadGrids("-default-");
            }
        }
        private void PL_PartyBuffRemoveButton_Click(object sender, EventArgs e)
        {
            Boolean playerSelected = false;
            Boolean defaultSelected = false;
            PLCharacter firstSelChar = null;
            DataGridViewSelectedCellCollection charCellList = CharacterGrid.SelectedCells;
            DataGridViewSelectedCellCollection cellsToRemove = PL_BuffsGrid.SelectedCells;
            foreach (DataGridViewCell cell in charCellList)
            {
                if (cell.ColumnIndex == (int)PL_CHAR_GRID_COL.NAME)
                {
                    //Remove from the default party buff list.
                    if ((String)cell.Value == "-default-")
                    {
                        foreach (DataGridViewCell buffCell in cellsToRemove)
                        {
                            if (buffCell.ColumnIndex == (int)PL_SELF_BUFF_COL.BUFF)
                            {
                                Command buff = (Command)buffCell.Value;
                                Int32 idxOf = PL_PartyBuffCmdList.IndexOf(buff);
                                if (idxOf < 0)
                                {
                                    LoggingFunctions.Error("PL_PartyBuffRemoveButton_Click: Could not find index of " + buff.Name + " in the buff list.");
                                    continue;
                                }
                                PL_PartyBuffCmdList.RemoveAt(idxOf);
                                PL_PartyBuffRecastList.RemoveAt(idxOf);
                                PL_PartyBuffEnableList.RemoveAt(idxOf);
                                idxOf = PL_PartyBuffCmdListMaster.IndexOf(buff);
                                if (idxOf >= 0)
                                {
                                    PL_PartyBuffCmdListMaster.RemoveAt(idxOf);
                                    PL_PartyBuffRecastListMaster.RemoveAt(idxOf);
                                    PL_PartyBuffEnableListMaster.RemoveAt(idxOf);
                                }
                                UserSettings.RemoveListValue(UserSettings.BOT.PL, UserSettings.LIST_TABLE.PL_PARTYBUFFS, buff.Name);
                            }
                        }
                        defaultSelected = true;
                    }
                    else
                    {
                        foreach (PLCharacter player in PL_CharacterList)
                        {
                            if (player.Name == (String)cell.Value)
                            {
                                foreach (DataGridViewCell buffCell in cellsToRemove)
                                {
                                    if (buffCell.ColumnIndex == (int)PL_SELF_BUFF_COL.BUFF)
                                    {
                                        if (!playerSelected)
                                        {
                                            playerSelected = true;
                                            firstSelChar = player;
                                        }
                                        Command buff = (Command)buffCell.Value;
                                        for (int ii = 0; ii < player.TaskList.Count; ii++)
                                        {
                                            if (buff == player.TaskList[ii].Cmd)
                                            {
                                                Task toRemove = player.TaskList[ii];
                                                player.TaskList.RemoveAt(ii);
                                                toRemove.Kill();
                                                break;
                                            }
                                        }
                                        for (int ii = 0; ii < player.TaskListMaster.Count; ii++)
                                        {
                                            if (buff == player.TaskListMaster[ii].Cmd)
                                            {
                                                Task toRemove = player.TaskListMaster[ii];
                                                player.TaskListMaster.RemoveAt(ii);
                                                toRemove.Kill();
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (playerSelected)
            {
                PL_ReloadGrids(firstSelChar.Name);
            }
            else if (defaultSelected)
            {
                PL_ReloadGrids("-default-");
            }
        }
        private void PL_BuffsGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (PL_BuffsGrid.CurrentCell.ColumnIndex == (int)PL_BUFF_GRID_COL.EN)
            {
                if (PL_BuffsGrid.IsCurrentCellDirty)
                {
                    PL_BuffsGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }
        private void PL_BuffsGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewSelectedCellCollection cellList = CharacterGrid.SelectedCells;
            foreach (DataGridViewCell cell in cellList)
            {
                if (cell.ColumnIndex == (int)PL_CHAR_GRID_COL.NAME)
                {
                    #region Default
                    //We are saving these new values to the user settings
                    if ((String)cell.Value == "-default-")
                    {
                        if (e.ColumnIndex == (int)PL_BUFF_GRID_COL.RECAST)
                        {
                            //Find the buff for this row
                            Command buff = (Command)PL_BuffsGrid[(int)PL_BUFF_GRID_COL.BUFF, e.RowIndex].Value;
                            Int32 idxOf = PL_PartyBuffCmdList.IndexOf(buff);
                            if (idxOf < 0)
                            {
                                LoggingFunctions.Error("PL_BuffsGrid_CellValueChanged: Couldn't find buff " + buff.Name + " in the buff list.");
                                continue;
                            }
                            UInt32 recast = Convert.ToUInt32(PL_BuffsGrid[(int)PL_BUFF_GRID_COL.RECAST, e.RowIndex].Value.ToString()) * 1000;
                            PL_PartyBuffRecastList[idxOf] = recast;
                            idxOf = PL_PartyBuffCmdListMaster.IndexOf(buff);
                            if(idxOf >= 0)
                            {
                                PL_PartyBuffRecastListMaster[idxOf] = recast;
                            }
                            UserSettings.SetListValue(UserSettings.BOT.PL, UserSettings.LIST_TABLE.PL_PARTYBUFFS, new List<object>() { Data.Client.FfxiResource.EncodeApostrophy(buff.Name), PL_PartyBuffRecastList[idxOf].ToString() });
                        }
                        if (e.ColumnIndex == (int)PL_BUFF_GRID_COL.EN)
                        {
                            //Find the buff for this row
                            Command buff = (Command)PL_BuffsGrid[(int)PL_BUFF_GRID_COL.BUFF, e.RowIndex].Value;
                            Int32 idxOf = PL_PartyBuffCmdList.IndexOf(buff);
                            if (idxOf < 0)
                            {
                                LoggingFunctions.Error("PL_BuffsGrid_CellValueChanged: Couldn't find buff " + buff.Name + " in the buff list.");
                                continue;
                            }
                            Boolean enable = Convert.ToBoolean(PL_BuffsGrid[(int)PL_BUFF_GRID_COL.EN, e.RowIndex].Value);
                            PL_PartyBuffEnableList[idxOf] = enable;
                            idxOf = PL_PartyBuffCmdListMaster.IndexOf(buff);
                            if (idxOf >= 0)
                            {
                                PL_PartyBuffEnableListMaster[idxOf] = enable;
                            }
                        }
                    }
                    #endregion Default
                    #region Player
                    else
                    {
                        PLCharacter selChar = null;
                        foreach (PLCharacter player in PL_CharacterList)
                        {
                            if (player.Name == (String)cell.Value)
                            {
                                selChar = player;
                            }
                        }
                        if (e.ColumnIndex == (int)PL_BUFF_GRID_COL.EN)
                        {
                            Command buff = (Command)PL_BuffsGrid[(int)PL_BUFF_GRID_COL.BUFF, e.RowIndex].Value;
                            for (int ii = 0; ii < selChar.TaskList.Count; ii++)
                            {
                                if (buff == selChar.TaskList[ii].Cmd)
                                {
                                    selChar.TaskList[ii].UseTimer = Convert.ToBoolean(PL_BuffsGrid[(int)PL_BUFF_GRID_COL.EN, e.RowIndex].Value);
                                    if (selChar.TaskList[ii].UseTimer == true)
                                    {
                                        selChar.TaskList[ii].Stop();
                                        selChar.TaskList[ii].Enabled = false;
                                        Bots.PowerLevel.Access.EnqueueTask(selChar.TaskList[ii]);
                                        LoggingFunctions.Debug("TopPL::PL_BuffsGrid_CellValueChanged: Enquing task due to enable box being checked: " + selChar.TaskList[ii].Cmd.Name + ".", LoggingFunctions.DBG_SCOPE.PL);
                                    }
                                    else
                                    {
                                        selChar.TaskList[ii].Stop();
                                    }
                                    break;
                                }
                            }
                        }
                        else if (e.ColumnIndex == (int)PL_BUFF_GRID_COL.RECAST)
                        {
                            Command buff = (Command)PL_BuffsGrid[(int)PL_BUFF_GRID_COL.BUFF, e.RowIndex].Value;
                            for (int ii = 0; ii < selChar.TaskList.Count; ii++)
                            {
                                if (buff == selChar.TaskList[ii].Cmd)
                                {
                                    selChar.TaskList[ii].Interval = Convert.ToUInt32(PL_BuffsGrid[(int)PL_BUFF_GRID_COL.RECAST, e.RowIndex].Value) * 1000;
                                    if (selChar.TaskList[ii].UseTimer)
                                    {
                                        selChar.TaskList[ii].Reset();
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    #endregion Player
                }
            }
        }
        private void PL_BuffsGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            if (e.ColumnIndex == (int)PL_BUFF_GRID_COL.CAST)
            {
                DataGridViewSelectedCellCollection cellList = CharacterGrid.SelectedCells;

                foreach (DataGridViewCell cell in cellList)
                {
                    if (cell.ColumnIndex == (int)PL_CHAR_GRID_COL.NAME)
                    {
                        PLCharacter selChar = null;
                        foreach (PLCharacter chr in PL_CharacterList)
                        {
                            if (chr.Name == (String)cell.Value)
                            {
                                selChar = chr;
                            }
                        }
                        if (selChar != null)
                        {

                            Command buff = (Command)PL_BuffsGrid[(Int32)PL_BUFF_GRID_COL.BUFF, e.RowIndex].Value;
                            foreach (Task tsk in selChar.TaskList)
                            {
                                if (tsk.Cmd == buff)
                                {
                                    tsk.Reset();
                                    Bots.PowerLevel.Access.EnqueueTask(tsk);
                                    LoggingFunctions.Debug("TopPL::PL_BuffsGrid_CellContentClick: Enquing task " + tsk.Cmd.Name + " for " + selChar.Name + " due to 'Cast' button click.", LoggingFunctions.DBG_SCOPE.PL);
                                }
                            }
                        }
                    }
                }
            }
        }
        private void PL_ClearBuffsGrid()
        {
            try
            {
                if (PL_BuffsGrid.InvokeRequired)
                {
                    PL_BuffsGrid.Invoke(PL_clearBuffsGridPtr);
                }
                else
                {
                    PL_ClearBuffsGridCBF();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Clearing buffs grid: " + e.ToString());
            }
        }
        private void PL_ClearBuffsGridCBF()
        {
            PL_BuffsGrid.Rows.Clear();
        }
        #endregion
        #region Manual Cure Section
        private bool PL_LoadDebuffsGrid()
        {
            while (DebuffGrid == null)
            {
                IocaineFunctions.delay(10);
            }
            try
            {
                return (bool)DebuffGrid.Invoke(PL_loadDebuffsGridPtr);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL_LoadDebuffsGrid: Doing Debuffs Grid inits: " + e.ToString());
                return false;
            }
        }
        private bool PL_LoadDebuffsGridCBF()
        {
            LoggingFunctions.Debug("PL_LoadDebuffsGridCBF: Doing load of debuffs grid.", LoggingFunctions.DBG_SCOPE.PL);
            PL_ClearDebuffGrid();
            if(PL_MyDebuffCmdList == null)
            {
                return false;
            }
            
            foreach (Command cmdToAdd in PL_MyDebuffCmdList)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                DataGridViewCell buffCell = new DataGridViewTextBoxCell();
                int insertPoint = DebuffGrid.Rows.Count;
                buffCell.Value = cmdToAdd;
                newRow.Cells.Insert((int)PL_DEBUFF_GRID_COL.BUFF, buffCell);
                DebuffGrid.Rows.Insert(insertPoint, newRow);
                //object obj = buffCell.FormattedValue;
            }
            return true;
        }
        private void PL_ManualCureAddButton_Click(object sender, EventArgs e)
        {
            Command cmdToAdd = (Command)PL_DebuffCB.SelectedItem;
            //Add cure to debuffs grid
            DataGridViewRow newRow = new DataGridViewRow();
            DataGridViewCell buffCell = new DataGridViewTextBoxCell();
            int insertPoint = DebuffGrid.Rows.Count;
            buffCell.Value = cmdToAdd;
            newRow.Cells.Insert((int)PL_DEBUFF_GRID_COL.BUFF, buffCell);
            DebuffGrid.Rows.Insert(insertPoint, newRow);

            //Add cure to the my debuff list
            PL_MyDebuffCmdList.Add(cmdToAdd);
            if(!PL_MyDebuffCmdListMaster.Contains(cmdToAdd))
            {
                PL_MyDebuffCmdListMaster.Add(cmdToAdd);
            }

            //Add cure to the user settings profile
            UserSettings.AddListValue(UserSettings.BOT.PL, UserSettings.LIST_TABLE.PL_DEBUFFS, new List<Object>() { Data.Client.FfxiResource.EncodeApostrophy(cmdToAdd.Name) });
        }
        //private void PL_DebuffGrid_CellFormatting(object sender, System.Windows.Forms.DataGridViewCellFormattingEventArgs e)
        //{
        //    if (DebuffGrid.Columns[e.ColumnIndex].HeaderText.Equals("Trgt. Cure"))
        //    {
        //        if (e.Value != null)
        //        {
        //            Command cmd = (Command)e.Value;
        //            e.Value = cmd.Name;
        //            e.FormattingApplied = true;
        //        }
        //    }
        //}
        private void PL_ManualCureRemoveButton_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cellsToRemove = DebuffGrid.SelectedCells;
            foreach(DataGridViewCell cell in cellsToRemove)
            {
                if(cell.ColumnIndex == (int)PL_DEBUFF_GRID_COL.BUFF)
                {
                    Command cmdToRemove = (Command)cell.Value;
                    if (PL_MyDebuffCmdList.Contains(cmdToRemove))
                    {
                        PL_MyDebuffCmdList.Remove(cmdToRemove);
                    }
                    if (PL_MyDebuffCmdListMaster.Contains(cmdToRemove))
                    {
                        PL_MyDebuffCmdListMaster.Remove(cmdToRemove);
                    }
                    DebuffGrid.Rows.RemoveAt(cell.RowIndex);
                    UserSettings.RemoveListValue(UserSettings.BOT.PL, UserSettings.LIST_TABLE.PL_DEBUFFS, cmdToRemove.Name);
                }
            }
        }
        private void PL_DebuffGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex < 0)
            {
                return;
            }
            if (e.ColumnIndex == (int)PL_DEBUFF_GRID_COL.CAST)
            {
                DataGridViewSelectedCellCollection cellList = CharacterGrid.SelectedCells;
                foreach (DataGridViewCell cell in cellList)
                {
                    if (cell.ColumnIndex == (int)PL_CHAR_GRID_COL.NAME)
                    {
                        PLCharacter selChar = null;
                        foreach (PLCharacter chr in PL_CharacterList)
                        {
                            if (chr.Name == (String)cell.Value)
                            {
                                selChar = chr;
                            }
                        }
                        if (selChar != null)
                        {
                            Task cmdWrapper = new Task(Task.TYPE.CURE, selChar.Name, (Command)DebuffGrid[(int)PL_DEBUFF_GRID_COL.BUFF, e.RowIndex].Value, 0, false, Statics.Settings.PowerLevel.DebuffQ);
                            Bots.PowerLevel.Access.EnqueueTask(cmdWrapper);
                        }
                    }
                }
            }
        }
        private void PL_ClearDebuffGrid()
        {
            try
            {
                if (DebuffGrid.InvokeRequired)
                {
                    DebuffGrid.Invoke(PL_clearDebuffGridPtr);
                }
                else
                {
                    PL_ClearDebuffGridCBF();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL_ClearDebuffGrid: Clearing debuff grid: " + e.ToString());
            }
        }
        private void PL_ClearDebuffGridCBF()
        {
            DebuffGrid.Rows.Clear();
        }
        #endregion
        #region Self Buff Section
        private bool PL_LoadSelfBuffsGrid()
        {
            try
            {
                return (bool)PL_SelfBuffGrid.Invoke(PL_loadSelfBuffsGridPtr);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL_LoadSelfBuffsGrid: Doing Self Buff Grid inits: " + e.ToString());
                return false;
            }
        }
        private bool PL_LoadSelfBuffsGridCBF()
        {
            LoggingFunctions.Debug("PL_LoadSelfBuffsGridCBF: Doing init of self buffs grid.", LoggingFunctions.DBG_SCOPE.PL);
            PL_ClearSelfBuffsGrid();
            foreach (Task tskToAdd in PL_MySelfBuffTaskList)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                DataGridViewCell selfBuffCell = new DataGridViewTextBoxCell();
                DataGridViewCell enCell = new DataGridViewCheckBoxCell();
                DataGridViewCell recastCell = new DataGridViewTextBoxCell();
                int insertPoint = PL_SelfBuffGrid.Rows.Count;
                selfBuffCell.Value = tskToAdd;
                enCell.Value = tskToAdd.UseTimer;
                recastCell.Value = tskToAdd.Interval / 1000;
                newRow.Cells.Insert((int)PL_SELF_BUFF_COL.BUFF, selfBuffCell);
                newRow.Cells.Insert((int)PL_SELF_BUFF_COL.EN, enCell);
                newRow.Cells.Insert((int)PL_SELF_BUFF_COL.RECAST, recastCell);
                PL_SelfBuffGrid.Rows.Insert(insertPoint, newRow);
                LoggingFunctions.Debug("PL_LoadSelfBuffsGridCBF: Adding self buff row: " + tskToAdd.CmdName + " enable: " + tskToAdd.UseTimer + " recast: " + tskToAdd.Interval + ".", LoggingFunctions.DBG_SCOPE.PL);
            }
            return true;
        }
        private void PL_SelfBuffRemoveButton_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cellsToRemove = PL_SelfBuffGrid.SelectedCells;
            foreach (DataGridViewCell cell in cellsToRemove)
            {
                if (cell.ColumnIndex == (int)PL_SELF_BUFF_COL.BUFF)
                {
                    Task taskToRemove = (Task)cell.Value;
                    taskToRemove.Kill();
                    PL_MySelfBuffTaskList.Remove(taskToRemove);
                    PL_SelfBuffGrid.Rows.RemoveAt(cell.RowIndex);
                    UserSettings.RemoveListValue(UserSettings.BOT.PL, UserSettings.LIST_TABLE.PL_SELFBUFFS, taskToRemove.CmdName);
                }
            }
        }
        private void PL_SelfBuffAddButton_Click(object sender, EventArgs e)
        {
            //Add buff to self buffs grid
            DataGridViewRow newRow = new DataGridViewRow();
            DataGridViewCell buffCell = new DataGridViewTextBoxCell();
            DataGridViewCell enCell = new DataGridViewCheckBoxCell();
            DataGridViewCell recastCell = new DataGridViewTextBoxCell();
            Command cmdToAdd = (Command)PL_SelfBuffCB.SelectedItem;
            Task taskToAdd = new Task(Task.TYPE.BUFF, PlayerCache.Vitals.Name, cmdToAdd, cmdToAdd.Duration, false, Statics.Settings.PowerLevel.SelfBuffQ);
            int insertPoint = PL_SelfBuffGrid.Rows.Count;
            buffCell.Value = taskToAdd;
            enCell.Value = taskToAdd.UseTimer = false;
            recastCell.Value = cmdToAdd.Duration / 1000;
            newRow.Cells.Insert((int)PL_SELF_BUFF_COL.BUFF, buffCell);
            newRow.Cells.Insert((int)PL_SELF_BUFF_COL.EN, enCell);
            newRow.Cells.Insert((int)PL_SELF_BUFF_COL.RECAST, recastCell);
            PL_SelfBuffGrid.Rows.Insert(insertPoint, newRow);

            //Add buff to my buff list
            PL_MySelfBuffTaskList.Add(taskToAdd);
            taskToAdd.Elapsed += new System.Timers.ElapsedEventHandler(Bots.PowerLevel.Access.RebuffTimer_Elapsed);
            LoggingFunctions.Debug("SelfBuffAddButton_Click: Adding " + taskToAdd.Cmd.Name + " to event handler.", LoggingFunctions.DBG_SCOPE.PL);
            LoggingFunctions.Debug("SelfBuffAddButton_Click: Timer interval is " + taskToAdd.Interval.ToString() + ".", LoggingFunctions.DBG_SCOPE.PL);

            //Add buff to the user settings profile
            UserSettings.AddListValue(UserSettings.BOT.PL, UserSettings.LIST_TABLE.PL_SELFBUFFS, new List<object>() { Data.Client.FfxiResource.EncodeApostrophy(taskToAdd.CmdName), cmdToAdd.Duration.ToString() });
        }
        private void PL_SelfBuffGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (PL_SelfBuffGrid.CurrentCell.ColumnIndex == (int)PL_SELF_BUFF_COL.EN)
            {
                if (PL_SelfBuffGrid.IsCurrentCellDirty)
                {
                    PL_SelfBuffGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }
        private void PL_SelfBuffGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (PL_SelfBuffGrid.Rows.Count > 0)
            {
                LoggingFunctions.Debug("PL_SelfBuffGrid_CellValueChanged: SelfBuffGrid cell value changed.", LoggingFunctions.DBG_SCOPE.PL);
                Task editedTask = (Task)PL_SelfBuffGrid.Rows[e.RowIndex].Cells[(Int32)PL_SELF_BUFF_COL.BUFF].Value;
                if (e.ColumnIndex == (Int32)PL_SELF_BUFF_COL.EN)
                {
                    LoggingFunctions.Debug("PL_SelfBuffGrid_CellValueChanged: SelfBuffGrid enable changed for row " + e.RowIndex + ".", LoggingFunctions.DBG_SCOPE.PL);
                    bool newEnValue = (Boolean)PL_SelfBuffGrid[e.ColumnIndex, e.RowIndex].Value;
                    if (newEnValue && !editedTask.UseTimer)
                    {
                        LoggingFunctions.Debug("PL_SelfBuffGrid_CellValueChanged: Enabling task " + editedTask.CmdName + " and sending to Q.", LoggingFunctions.DBG_SCOPE.PL);
                        editedTask.UseTimer = newEnValue;
                        Bots.PowerLevel.Access.EnqueueTask(editedTask);
                    }
                    else
                    {
                        LoggingFunctions.Debug("PL_SelfBuffGrid_CellValueChanged: Stopping task " + editedTask.CmdName + ".", LoggingFunctions.DBG_SCOPE.PL);
                        editedTask.Stop();
                        editedTask.UseTimer = newEnValue;
                    }
                }
                else if (e.ColumnIndex == (Int32)PL_SELF_BUFF_COL.RECAST)
                {
                    LoggingFunctions.Debug("PL_SelfBuffGrid_CellValueChanged: SelfBuffGrid recast changed for row " + e.RowIndex + ".", LoggingFunctions.DBG_SCOPE.PL);
                    editedTask.Interval = (System.Convert.ToInt32(PL_SelfBuffGrid[e.ColumnIndex, e.RowIndex].Value) * 1000);
                    UserSettings.SetListValue(UserSettings.BOT.PL, UserSettings.LIST_TABLE.PL_SELFBUFFS, new List<object>() { Data.Client.FfxiResource.EncodeApostrophy(editedTask.CmdName), editedTask.Interval.ToString() });
                    if (editedTask.UseTimer)
                    {
                        editedTask.Reset();
                    }
                }
            }
        }
        private void PL_SelfBuffGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            if (e.ColumnIndex == (Int32)PL_SELF_BUFF_COL.CAST)
            {
                Task taskToDo = (Task)PL_SelfBuffGrid[(Int32)PL_SELF_BUFF_COL.BUFF, e.RowIndex].Value;
                taskToDo.Reset();
                Bots.PowerLevel.Access.EnqueueTask(taskToDo);
            }
        }
        private void PL_ClearSelfBuffsGrid()
        {
            try
            {
                if (PL_SelfBuffGrid.InvokeRequired)
                {
                    PL_SelfBuffGrid.Invoke(PL_clearSelfBuffsGridPtr);
                }
                else
                {
                    PL_ClearSelfBuffsGridCBF();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL_ClearSelfBuffsGrid: Clearing self buffs grid: " + e.ToString());
            }
        }
        private void PL_ClearSelfBuffsGridCBF()
        {
            PL_SelfBuffGrid.Rows.Clear();
        }
        #endregion
        #region Threads Section
        private void PL_CheckPlayerActivityThreadRun()
        {
            while (true)
            {
                IocaineFunctions.delay((uint)Statics.Settings.PowerLevel.PlCharActivePollFrequency);
                if (PL_PauseCheckActiveThread || (PL_CharacterList.Count == 0))
                {
                    continue;
                }
                else
                {
                    LoggingFunctions.Debug("PL_CheckPlayerActivityThreadRun: =====================================================", LoggingFunctions.DBG_SCOPE.BACKGROUND);
                    foreach (PLCharacter chr in PL_CharacterList)
                    {
                        LoggingFunctions.Debug("PL_CheckPlayerActivityThreadRun: Checking player " + chr.Name + ".", LoggingFunctions.DBG_SCOPE.BACKGROUND);
                        int charRow = 0;
                        foreach (DataGridViewRow row in CharacterGrid.Rows)
                        {
                            LoggingFunctions.Debug("PL_CheckPlayerActivityThreadRun: Comparing with grid row #" + row.Index + " w/ value " + (String)row.Cells[(int)PL_CHAR_GRID_COL.NAME].Value + ".", LoggingFunctions.DBG_SCOPE.BACKGROUND);
                            if ((String)row.Cells[(int)PL_CHAR_GRID_COL.NAME].Value == chr.Name)
                            {
                                LoggingFunctions.Debug("PL_CheckPlayerActivityThreadRun: Compare matched. Cell value: " + ((String)row.Cells[(int)PL_CHAR_GRID_COL.NAME].Value) + " chr.Name: " + chr.Name + ".", LoggingFunctions.DBG_SCOPE.BACKGROUND);
                                charRow = row.Index;
                                LoggingFunctions.Debug("PL_CheckPlayerActivityThreadRun: row.Index is " + row.Index + ".", LoggingFunctions.DBG_SCOPE.BACKGROUND);
                                break;
                            }
                        }
                        LoggingFunctions.Debug("PL_CheckPlayerActivityThreadRun: Character: " + chr.Name + " returns checkActive: " + chr.Check_Active().ToString() + ".", LoggingFunctions.DBG_SCOPE.BACKGROUND);
                        bool chrActive = chr.Check_Active();
                        bool inRange = false;
                        if (chr.Name != PlayerCache.Vitals.Name)
                        {
                            inRange = chr.Distance <= 46;
                        }
                        else
                        {
                            inRange = true;
                        }
                        if (!chrActive)
                        {
                            chrActive = chr.Update_Pointer();
                        }
                        if (chrActive && inRange)
                        {
                            CharacterGrid[(int)PL_CHAR_GRID_COL.ACT, charRow].Value = Resources.GreenCheck;
                        }
                        else if(chrActive && !inRange)
                        {
                            CharacterGrid[(int)PL_CHAR_GRID_COL.ACT, charRow].Value = Resources.BlueArrow;
                        }
                        else
                        {
                            CharacterGrid[(int)PL_CHAR_GRID_COL.ACT, charRow].Value = Resources.RedX;
                        }
                        LoggingFunctions.Debug("PL_CheckPlayerActivityThreadRun: Character " + chr.Name + " on row " + charRow.ToString() + " is now " + (chr.Active ? "active" : "NOT active") + " and " + (inRange ? "" : "not ") + "in range.", LoggingFunctions.DBG_SCOPE.BACKGROUND);
                    }
                }
            }
        }
        #endregion
        #region Utility Functions
        public void PL_ReloadGrids(String iName = "-default-")
        {
            PL_LoadCuresGrid(iName);
            PL_LoadBuffsGrid(iName);
            PL_LoadDebuffsGrid();
            PL_LoadSelfBuffsGrid();
        }
        private void PL_UpdateWakeCureCommand()
        {
            foreach (Command cmd in PL_CureCmdList)
            {
                if (cmd.Name == Statics.Settings.PowerLevel.WakeCure)
                {
                    PL_WakeCureCommand = cmd;
                }
            }
        }
        private void PL_UpdateAllCharacterBuffPriorites()
        {
            foreach (PLCharacter chr in PL_CharacterList)
            {
                foreach (Task tsk in chr.TaskList)
                {
                    tsk.Priority = Statics.Settings.PowerLevel.BuffQ;
                }
                foreach (Task tsk in chr.TaskListMaster)
                {
                    tsk.Priority = Statics.Settings.PowerLevel.BuffQ;
                }
            }
        }
        private void PL_UpdateAllSelfBuffPriorities()
        {
            foreach (Task tsk in PL_MySelfBuffTaskList)
            {
                tsk.Priority = Statics.Settings.PowerLevel.SelfBuffQ;
            }
        }
        private void PL_AppendChat(String iText)
        {
            try
            {
                if(PL_EventsChatLogRTB.InvokeRequired)
                {
                    PL_EventsChatLogRTB.Invoke(PL_appendChatPtr, new object[] { iText });
                }
                else
                {
                    PL_AppendChatCBF(iText);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL_AppendChat: " + e.ToString());
            }
        }
        private void PL_AppendChatCBF(String iText)
        {
            PL_EventsChatLogRTB.AppendText(iText);
            PL_EventsChatLogRTB.ScrollToCaret();
        }
        private void PL_RemoveChat()
        {
            try
            {
                if (PL_EventsChatLogRTB.InvokeRequired)
                {
                    PL_EventsChatLogRTB.Invoke(PL_removeChatPtr);
                }
                else
                {
                    PL_RemoveChatCBF();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL_RemoveChatCBF: " + e.ToString());
            }
        }
        private void PL_RemoveChatCBF()
        {
            while(PL_EventsChatLogRTB.Lines.Length > 501)
            {
                PL_EventsChatLogRTB.Select(0, PL_EventsChatLogRTB.GetFirstCharIndexFromLine(1));
                PL_EventsChatLogRTB.SelectedText = "";
            }
        }
        #endregion
        #region Non-Grid Event Handlers
        private void PL_Start_Button_Click(object sender, EventArgs e)
        {
            if (Statics.Flags.ProcessState == 0)
            {
                MessageBox.Show("Cannot find pol process of given name. Check name and/or dual box checkbox");
            }
            else
            {
                if (Bots.PowerLevel.Access.State == Bots.STATE.STOPPED)
                {
                    LoggingFunctions.Timestamp("Starting PL Bot");
                    PL_DequeueThread = new Thread(new ThreadStart(Bots.PowerLevel.Access.DequeueThreadFunction));
                    PL_DequeueThread.Name = "PLDequeueThread";
                    PL_DequeueThread.IsBackground = true;
                    PL_EnqueueThread = new Thread(new ThreadStart(Bots.PowerLevel.Access.EnqueueThreadFunction));
                    PL_EnqueueThread.Name = "PLEnqueueThread";
                    PL_EnqueueThread.IsBackground = true;
                    PL_DequeueThread.Start();
                    PL_EnqueueThread.Start();

                    PL_Start_Button.UseMnemonic = true;
                    PL_Start_Button.Text = "&Pause";
                    PL_Start_Button.BackColor = System.Drawing.Color.Yellow;
                }
                else if (Bots.PowerLevel.Access.State == Bots.STATE.RUNNING)
                {
                    LoggingFunctions.Timestamp("Pausing PL Bot");
                    Bots.PowerLevel.Access.Pause();
                    PL_Start_Button.UseMnemonic = true;
                    PL_Start_Button.Text = "&Resume";
                    PL_Start_Button.BackColor = System.Drawing.Color.Lime;
                }
                else
                {
                    LoggingFunctions.Timestamp("Resuming PL Bot");
                    Bots.PowerLevel.Access.Resume();
                    PL_Start_Button.UseMnemonic = true;
                    PL_Start_Button.Text = "&Pause";
                    PL_Start_Button.BackColor = System.Drawing.Color.Yellow;
                }
            }
        }
        private void PL_Stop_Button_Click(object sender, EventArgs e)
        {
            LoggingFunctions.Timestamp("Stopping PL Bot");
            Bots.PowerLevel.Access.Stop();
            while (Bots.PowerLevel.Access.State != Bots.STATE.STOPPED)
            {
                IocaineFunctions.delay(500);
            }
            PL_Start_Button.UseMnemonic = true;
            PL_Start_Button.Text = "S&tart";
            PL_Start_Button.BackColor = System.Drawing.Color.Lime;
        }
        private void PL_AllowAutoWakeChkB_CheckedChanged(object sender, EventArgs e)
        {
            PL_CuresAutoWake = ((CheckBox)sender).Checked;
            LoggingFunctions.Debug("PL_CuresAutoWake value is now " + PL_CuresAutoWake.ToString(), LoggingFunctions.DBG_SCOPE.PL);
        }
        private void PL_WakeButton_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = CharacterGrid.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                if (cell.ColumnIndex == (int)PL_CHAR_GRID_COL.NAME)
                {
                    foreach (PLCharacter chr in PL_CharacterList)
                    {
                        if ((chr.Name == (String)cell.Value) && (chr.Name != PlayerCache.Vitals.Name))
                        {
                            if (!chr.CureQueued)
                            {
                                Bots.PowerLevel.Access.EnqueueTask(Bots.PowerLevel.Access.WrapTask(PL_WakeCureCommand, chr, Statics.Settings.PowerLevel.WakeCureQ));
                            }
                        }
                    }
                }
            }
        }
        private void PL_WakeAllButton_Click(object sender, EventArgs e)
        {
            foreach (PLCharacter chr in PL_CharacterList)
            {
                if (chr.Name != PlayerCache.Vitals.Name)
                {
                    if (!chr.CureQueued)
                    {
                        Bots.PowerLevel.Access.EnqueueTask(Bots.PowerLevel.Access.WrapTask(PL_WakeCureCommand, chr, Statics.Settings.PowerLevel.WakeCureQ));
                    }
                }
            }
        }
        private void PL_FollowUpDownBox_ValueChanged(object sender, EventArgs e)
        {
            PL_SetFollowCharacter();
        }
        private void PL_FollowDistUpDownBox_ValueChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("PL_FollowDistUpDownBox_ValueChanged: Follow distance changed. Old distance: " + Statics.Settings.PowerLevel.FollowDistance + " and old upper distance: " + Statics.Settings.PowerLevel.FollowDistanceUpper + ".", LoggingFunctions.DBG_SCOPE.PL);
            Statics.Settings.PowerLevel.FollowDistance = (int)FollowDistUpDownBox.Value;
            Statics.Settings.PowerLevel.FollowDistanceUpper = Statics.Settings.PowerLevel.UseElasticFollowing ? (Statics.Settings.PowerLevel.FollowDistance + Statics.Settings.PowerLevel.ElasticDistance) : Statics.Settings.PowerLevel.FollowDistance;
            LoggingFunctions.Debug("PL_FollowDistUpDownBox_ValueChanged: New distance: " + Statics.Settings.PowerLevel.FollowDistance + " and new upper distance: " + Statics.Settings.PowerLevel.FollowDistanceUpper + ".", LoggingFunctions.DBG_SCOPE.PL);
            UserSettings.SetValue(UserSettings.BOT.PL, "FollowDistance", Statics.Settings.PowerLevel.FollowDistance.ToString());
        }
        private void PL_Settings_Apply_Button_Click(object sender, EventArgs e)
        {
            PL_SetFocusCharacter();
            PL_UpdateWakeCureCommand();
            PL_UpdateAllCharacterBuffPriorites();
            Bots.PowerLevel.Access.UpdateMpReportEnable();
            Bots.PowerLevel.Access.UpdateMpReportTime();
            Bots.PowerLevel.Access.UpdateMpReportCommand();
            Bots.PowerLevel.Access.UpdateCastTimeReportTime();
            Bots.PowerLevel.Access.UpdateCastTimeReportCommand();
            Bots.PowerLevel.Access.UpdateOurOfRangeCommandEnable();
            Bots.PowerLevel.Access.UpdateOutOfRangeCommand();
            Bots.PowerLevel.Access.UpdateCommand1Enable();
            Bots.PowerLevel.Access.UpdateCommand1Timer();
            Bots.PowerLevel.Access.UpdateCommand1Command();
            Bots.PowerLevel.Access.UpdateCommand2Enable();
            Bots.PowerLevel.Access.UpdateCommand2Timer();
            Bots.PowerLevel.Access.UpdateCommand2Command();
            Bots.PowerLevel.Access.UpdateCommand3Enable();
            Bots.PowerLevel.Access.UpdateCommand3Timer();
            Bots.PowerLevel.Access.UpdateCommand3Command();
            Bots.PowerLevel.Access.UpdateCommand4Enable();
            Bots.PowerLevel.Access.UpdateCommand4Timer();
            Bots.PowerLevel.Access.UpdateCommand4Command();
        }
        private void PL_Settings_OK_Button_Click(object sender, EventArgs e)
        {
            PL_SetFocusCharacter();
            PL_UpdateWakeCureCommand();
            PL_UpdateAllCharacterBuffPriorites();
            Bots.PowerLevel.Access.UpdateMpReportEnable();
            Bots.PowerLevel.Access.UpdateMpReportTime();
            Bots.PowerLevel.Access.UpdateMpReportCommand();
            Bots.PowerLevel.Access.UpdateCastTimeReportTime();
            Bots.PowerLevel.Access.UpdateCastTimeReportCommand();
            Bots.PowerLevel.Access.UpdateOurOfRangeCommandEnable();
            Bots.PowerLevel.Access.UpdateOutOfRangeCommand();
            Bots.PowerLevel.Access.UpdateCommand1Enable();
            Bots.PowerLevel.Access.UpdateCommand1Timer();
            Bots.PowerLevel.Access.UpdateCommand1Command();
            Bots.PowerLevel.Access.UpdateCommand2Enable();
            Bots.PowerLevel.Access.UpdateCommand2Timer();
            Bots.PowerLevel.Access.UpdateCommand2Command();
            Bots.PowerLevel.Access.UpdateCommand3Enable();
            Bots.PowerLevel.Access.UpdateCommand3Timer();
            Bots.PowerLevel.Access.UpdateCommand3Command();
            Bots.PowerLevel.Access.UpdateCommand4Enable();
            Bots.PowerLevel.Access.UpdateCommand4Timer();
            Bots.PowerLevel.Access.UpdateCommand4Command();
        }
        private void PL_Form_ResizeEnd(object sender, EventArgs e)
        {
            //Total height of char box and manual cures box is 150 + 88 = 238
            //Total control height is bottom of manual cures box - top of char box => 341 - 16 = 325
            //Remaining control between => 325 - 238 = 87
            //To get new height => new manual cures bottom - new characters top - 87 = new box total height
            //Character box height = new box total height * (150 / (150 + 238))
            //Manual cures box height = new total box height - new character box height
            //New button Y = character box bottom + 17
            //New TB Y = new button top + 29
            /*Int32 new_total_left_box_height = DebuffGrid.Bottom - CharacterGrid.Top - 87;
            Double char_box_scale_factor = 150d / (150d + 88d);
            CharacterGrid.Height = (Int32)((Double)new_total_left_box_height * char_box_scale_factor);
            DebuffGrid.Height = new_total_left_box_height - CharacterGrid.Height;
            DebuffGrid.Top = DebuffComboBox.Top - 6 - DebuffGrid.Height;
            DebuffGridLabel.Top = DebuffGrid.Top - 15;
            CharBoxAddButton.Top = CharacterGrid.Bottom + 17;
            CharBoxAddTargetButton.Top = CharacterGrid.Bottom + 17;
            CharBoxRemoveButton.Top = CharacterGrid.Bottom + 17;
            AddCharTextBox.Top = CharBoxAddButton.Top + 29;
            PL_Start_Button.Top = DebuffGrid.Top + 1;
            PL_Stop_Button.Top = PL_Start_Button.Top + 48;

            Int32 new_total_right_box_height = SelfBuffGrid.Bottom - CuresGrid.Top - 6;
            Double buff_box_scale_factor = 110d / (110d + 108d + 88d);
            SelfBuffGrid.Height = (Int32)(new_total_right_box_height * buff_box_scale_factor);
            BuffsGrid.Height = SelfBuffGrid.Height;
            CuresGrid.Height = new_total_right_box_height - SelfBuffGrid.Height - BuffsGrid.Height;
            BuffsGrid.Top = CuresGrid.Bottom + 3;
            SelfBuffGrid.Top = SelfBuffComboBox.Top - 6 - SelfBuffGrid.Height;

            //Mid-width is about the RHS of the wake button. Anchor the widths from there.
            //CharacterGrid width = new midpoint - wake button width - 34.
            CharacterGrid.Width = TOP_form_current_center_x - WakeButton.Width - 4 - CharacterGrid.Left;
            PL_Start_Button.Left = TOP_form_current_center_x - WakeButton.Width - 4 - PL_Start_Button.Width;
            PL_Stop_Button.Left = TOP_form_current_center_x - WakeButton.Width - 4 - PL_Stop_Button.Width;
            DebuffGrid.Width = PL_Start_Button.Left - 12;
            DebuffComboBox.Width = PL_Start_Button.Left - 12;

            WakeButton.Left = CharacterGrid.Right + 4;
            WakeAllButton.Left = CharacterGrid.Right + 4;
            PL_Player_Up_Button.Left = CharacterGrid.Right + 4;
            PL_Player_Dn_Button.Left = CharacterGrid.Right + 4;
            CuresGrid.Width = CuresGrid.Right - WakeButton.Right - 4;
            BuffsGrid.Width = CuresGrid.Width + WakeButton.Width + 4;
            SelfBuffGrid.Width = BuffsGrid.Width;
            SelfBuffComboBox.Width = SelfBuffGrid.Width;

            CuresGrid.Left = PL_Bot_Tab.Right - 3 - CuresGrid.Width;
            BuffsGrid.Left = PL_Bot_Tab.Right - 3 - BuffsGrid.Width;
            SelfBuffGrid.Left = PL_Bot_Tab.Right - 3 - SelfBuffGrid.Width;
            SelfBuffComboBox.Left = SelfBuffGrid.Left;
            SelfBuffAddButton.Left = SelfBuffGrid.Left;
            SelfBuffRemoveButton.Left = SelfBuffAddButton.Right + 6;*/
        }
        private void PL_ChatParseDebuffs()
        {
            //try
            //{
            //    Monitor.Enter(PL_chatDebuffParsing);
            //    if (!PL_PauseDebuffsParser)
            //    {
            //        String str = "";
            //        int code = 0;
            //        while (PL_chatDebuffParsing.Read(ref str, ref code))
            //        {
            //            //if(Bots.PowerLevel.Access.PLState != Bots.PowerLevel.Access.STATE.RUNNING)
            //            //{
            //            //    continue;
            //            //}
            //            PL_AppendChat("Code[" + code + "]: " + str + "\n");
            //            PL_RemoveChat();
            //            //LoggingFunctions.Timestamp("PL_ChatParseDebuffs: Got string '" + str + "'");
            //        }
            //    }

            //}
            //catch(Exception e)
            //{
            //    LoggingFunctions.Error("PL_ChatParseDebuffs: " + e.ToString());
            //}
            //finally
            //{
            //    Monitor.Exit(PL_chatDebuffParsing);
            //}
        }
        private void PL_EventsAddActionButton_Click(object sender, EventArgs e)
        {
            int lineNb = PL_EventsChatLogRTB.GetLineFromCharIndex(PL_EventsChatLogRTB.SelectionStart);
            MessageBox.Show("Selected line is: '" + PL_EventsChatLogRTB.Lines[lineNb] + "'");
        }
        private void PL_EventsRTB_SelectionChanged(object sender, EventArgs e)
        {
            ////Get selected line(s)
            //try
            //{
            //    //PL_EventsRTB.WordWrap = false;
            //    int startLine = PL_EventsChatLogRTB.GetLineFromCharIndex(PL_EventsChatLogRTB.SelectionStart);
            //    int endLine = PL_EventsChatLogRTB.GetLineFromCharIndex(PL_EventsChatLogRTB.SelectionStart + PL_EventsChatLogRTB.SelectionLength);
            //    if ((PL_EventsChatLogRTB.SelectedText.Length > 0) && (PL_EventsChatLogRTB.SelectedText.Last() == '\n'))
            //    {
            //        endLine--;
            //    }
            //    PL_ParsedEventRTB.Clear();
            //    for (int ii = startLine; ii <= endLine; ii++)
            //    {
            //        String str = PL_EventsChatLogRTB.Lines[ii] + "\n";

            //        int code = 0;
            //        Regex regex_code = new Regex("Code\\[([0-9]+)\\]: (.*)");
            //        Match match_code = regex_code.Match(str);
            //        if (match_code.Success)
            //        {
            //            code = Int32.Parse(match_code.Groups[1].ToString());
            //            str = match_code.Groups[2].ToString();
            //        }

            //        String target = "";
            //        foreach (PLCharacter chr in PL_CharacterList)
            //        {
            //            Regex regex_target = new Regex(chr.Name);
            //            Match match_target = regex_target.Match(str);
            //            if (match_target.Success)
            //            {
            //                target = chr.Name;
            //                break;
            //            }
            //        }
            //        if (target != "")
            //        {
            //            str = str.Replace(target, "$target");
            //        }
            //        PL_ParsedEventRTB.AppendText(str + " :: " + code + "\n");
            //    }
            //    //PL_EventsRTB.WordWrap = true;
            //}
            //catch (Exception ex)
            //{
            //    LoggingFunctions.Error("PL_EventsRTB_SelectionChanged: " + ex.ToString());
            //}
        }
        private void PL_EventsRTB_MouseEnter(object sender, EventArgs e)
        {
            PL_PauseDebuffsParser = true;
        }
        private void PL_EventsRTB_MouseLeave(object sender, EventArgs e)
        {
            PL_PauseDebuffsParser = false;
            PL_ChatParseDebuffs();
        }
        private void PL_ParsedEventRTB_TextChanged(object sender, EventArgs e)
        {
            //LoggingFunctions.Timestamp("Test: " + PL_ParsedEventRTB.Text);
        }
        #endregion Non-Grid Event Handlers
    }
}