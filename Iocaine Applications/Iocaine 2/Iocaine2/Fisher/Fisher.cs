using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;

using Iocaine2.Data.Client;
using Iocaine2.Inventory;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Settings;
using Iocaine2.Threading;
using Iocaine2.Tools;

namespace Iocaine2.Bots
{
    public sealed partial class Fisher : Bot
    {
        #region Enums
        private enum LV_COL
        {
            CNT = 0,
            CATCH = 1,
            PERC = 2
        }
        #endregion

        #region Structures
        private struct FishStatEntry
        {
            public byte result;
            public byte day;
            public short time;
            public byte moon;
            public byte weather;
            public byte skill;
            public byte fatigue;
            public short fishHp;
            public short zone;
            public DateTime date;
            public short fish;
            public short rod;
            public short bait;
            public int info;
            public short xPos;
            public short yPos;
            public int version;
        }
        #endregion Structures

        #region Template Members
        private static Fisher instance = null;
        private static readonly Object padlock = new Object();
        #endregion Template Members

        #region Bot Specific Members
        private DateTime fisherStartTimeStamp;
        private DateTime m_lastJpMidnight;
        private List<string> m_ServerInitDone = new List<string>();
        private Statics.Enums.ONLINE_MODE m_onlineStatus = Statics.Enums.ONLINE_MODE.UNKNOWN;
        #region Fish Stats Related
        private List<FishStatsDataSet.FishIDsLocalRow> fishIdsRows = new List<FishStatsDataSet.FishIDsLocalRow>();
        private List<FishStatsDataSet.FishStatsLocalRow> fishStatsRows = new List<FishStatsDataSet.FishStatsLocalRow>();
        private List<FishStatsDataSet.FishStatsLocalRow> fishStatsRows_master = new List<FishStatsDataSet.FishStatsLocalRow>();
        private FishStatEntry fishStatEntry;
        private Int32 statsInfoData = 0;
        private Boolean statsFishermansTunica = false;
        private Boolean statsFishermansGloves = false;
        private Boolean statsFishermansHose = false;
        private Boolean statsFishermansBoots = false;
        private Boolean statsAnglersTunica = false;
        private Boolean statsAnglersGloves = false;
        private Boolean statsAnglersHose = false;
        private Boolean statsAnglersBoots = false;
        private Boolean statsWaders = false;
        private Boolean statsFishermansApron = false;
        private Boolean statsSerpentRumors = false;
        private Boolean statsFrogFishing = false;
        private Boolean statsMooching = false;
        private Boolean statsFishingHoleMap = false;
        private Boolean statsTheBigOne = false;
        private Boolean statsFishermansSignBoard = false;
        private Boolean statsRustyBucket = false;
        private Boolean statsBlueBambooGrass = false;
        private Boolean statsGreenBambooGrass = false;
        private Boolean statsRedBambooGrass = false;
        private Boolean statsImagery = false;
        private Boolean statsPelicanRing = false;
        private Boolean statsAlbatrossRing = false;
        private Boolean statsPenguinRing = false;
        private Byte statsSkillUp = 0;
        #endregion Fish Stats Related
        #region State Related
        private uint recastDelay = 1000;
        private Int32 badFishCount = 0;
        private Int32 noCatchCount = 0;
        private const Int32 incWaitDelay = 200;
        private String lastCatch;
        private bool waitForNextDay = false;
        #endregion State Related
        #region Player/Environment Info
        private ushort rodId = 0;
        private string rodMacroText = "";
        private BaitBoxItem currentBaitItem = null;
        private List<BaitBoxItem> baitList = Statics.Settings.Fisher.BaitBoxItems;
        private Int32 currentFatigue = 0;
        #endregion Player/Environment Info
        #region Events/Function Pointers
        public event Statics.FuncPtrs.TD_Void_Void _FishingDone;
        public event Statics.FuncPtrs.TD_Void_Void _FisherFatigued;
        #endregion Events/Function Pointers
        #region Background Threads
        private Thread killFishThread = null;
        private Thread saveFishIDsThread = null;
        private Thread saveFishStatsThread = null;
        private IocaineThread vanaTimeThread = null;
        #endregion Background Threads
        #region Data Structures
        private ChatLoggerAsync chatLog; // Used for fatigue and last catch parsing.
        private ChatLoggerSync chatLog2; // Used for 'nearing xxx' and fishing skill parsing.
        private ChatLoggerAsync chatLog3; // Used for checking if monster is on the line.
        private Random randVal = new Random();
        private Audio player = new Audio();
        #endregion Data Structures
        #region Datasets
        #endregion Datasets
        #endregion Bot Specific Members

        #region Constructor
        private Fisher()
        {
            base.name = "Fisher";
        }
        #endregion Constructor

        #region Template Functions
        public static Fisher Access
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Fisher();
                    }
                    return instance;
                }
            }
        }
        public override void SetParams(Bots.BotParam iParam)
        {
            if(iParam == null)
            {
                return;
            }
            Bots.FisherParam prm = (Bots.FisherParam)iParam;
            c_startButton = prm.StartButton;
            c_stopButton = prm.StopButton;
            c_releaseButton = prm.ReleaseButton;
            c_timeDateTB = prm.TimeDateTB;
            c_dayTB = prm.DayTB;
            c_moonTB = prm.MoonTB;
            c_weatherTB = prm.WeatherTB;
            c_zoneLabel = prm.ZoneLabel;
            c_lastCatchLabel = prm.LastCatchLabel;
            c_fishNameLabel = prm.FishNameLabel;
            c_fishCurHPLabel = prm.CurHPLabel;
            c_fishMaxHPLabel = prm.MaxHPLabel;
            c_fatigueLabel = prm.FatigueLabel;
            c_id1Label = prm.Id1Label;
            c_id2Label = prm.Id2Label;
            c_id3Label = prm.Id3Label;
            c_largeLabel = prm.LargeLabel;
            c_hpProgressBar = prm.HpProgressBar;
            c_dropLB = prm.DropListBox;
            c_dropItemTB = prm.DropItemTB;
            c_dropAddButton = prm.DropBoxAddButton;
            c_dropRemoveButton = prm.DropBoxRemoveButton;
            c_baitLB = prm.BaitListBox;
            c_baitUpButton = prm.BaitBoxUpButton;
            c_baitDownButton = prm.BaitBoxDownButton;
            c_baitRefreshButton = prm.BaitBoxRefreshButton;
            c_fishLB = prm.FishListBox;
            c_statsLB = prm.StatsListBox;
            c_rodNameLabel = prm.RodName;
            c_baitNameLabel = prm.BaitName;
            c_baitQuanLabel = prm.BaitQuan;
            c_leftArrowPB = prm.LeftArrow;
            c_rightArrowPB = prm.RightArrow;
            _FishingDone += new Statics.FuncPtrs.TD_Void_Void(prm.FishingDoneHandler);
            _FisherFatigued += new Statics.FuncPtrs.TD_Void_Void(prm.FisherFatiguedHandler);
            c_autoReset = prm.AutoResetChkB;
            c_thisZoneOnly = prm.ThisZoneOnlyChkB;
            initParamsSet = true;
        }
        #endregion Template Functions

        #region Initialization
        public override bool Init_Iocaine()
        {
            // This will only be called once.
            try
            {
                Init_Controls();
                fisherStartTimeStamp = DateTime.Now;
                chatLog = ChatLogManager.Access.GetAsynchronousLogger("Fisher1");
                chatLog2 = ChatLogManager.Access.GetSynchronousLogger("Fisher2");
                chatLog3 = ChatLogManager.Access.GetAsynchronousLogger("Fisher3");
                ChangeMonitor._zoneChanged += updateZone;
                ChangeMonitor._areaChanged += updateArea;
                ChangeMonitor._equ_RangeChanged += updateRod;
                ChangeMonitor._equ_AmmoChanged += updateBait;
                ChangeMonitor._equ_AmmoQuanChanged += updateBaitQuan;
                startVanaTimeThread();
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::instanceInit: " + e.ToString());
                return false;
            }
            return base.Init_Iocaine();
        }
        public override bool Init_Process()
        {
            ChangeMonitor._weatherChanged -= updateWeather;
            ChangeMonitor._weatherChanged += updateWeather;
            return base.Init_Process();
        }
        public override bool Init_LoggedIn()
        {
            Init_UserSettings();
            c_dropLB_Load();
            init_LoggedIn_Server();
            Thread baitBoxInitThread = new Thread(new ThreadStart(c_baitLB_Refresh));
            baitBoxInitThread.Name = "BaitBoxInitThread";
            baitBoxInitThread.IsBackground = true;
            baitBoxInitThread.Start();
            return base.Init_LoggedIn();
        }
        private void init_LoggedIn_Server()
        {
            FishStatsDataSet.Access.Init_LoggedIn(); //Clean up local.
            fishStatsRows_master = FishStatsDataSet.Access.SelectFishStats(); //Fills local if empty, then server, then merges.
            filterFishStatsRows();
            c_StatsBoxLoad();
            if ((m_onlineStatus == Statics.Enums.ONLINE_MODE.ONLINE) && !m_ServerInitDone.Contains(PlayerCache.Vitals.Name))
            {
                m_ServerInitDone.Add(PlayerCache.Vitals.Name);
            }
        }
        private bool threadInit()
        {
            // This is called every time the fisher is started.
            resetFatigue();
            try
            {
                LoggingFunctions.Debug("Fisher::threadInit: Updating bait.", LoggingFunctions.DBG_SCOPE.FISHER);
                setCurrentBaitBoxItem(true);
                Bait.BAIT_INFO info = Bait.GetBaitInfo(PlayerCache.Equipment.Ammo);
                if ((info.ItemID == Bait.InvalidID) && (currentBaitItem != null))
                {
                    equipBait();
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::threadInit: Setting bait: " + e.ToString());
            }
            try
            {
                setFatigue(c_fatigueLabel_GetValue());
                chatLog.Reset();
                chatLog2.Reset();
                chatLog3.Reset();
                m_lastJpMidnight = VanaTime.LastJapaneseMidnightUTC;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::threadInit: setFatigue: " + e.ToString());
            }
            return true;
        }
        public void SetOnlineStatus(Statics.Enums.ONLINE_MODE iMode)
        {
            try
            {
                FishStatsDataSet.Access.SetOnlineStatus(iMode);
                m_onlineStatus = iMode;
                if (ChangeMonitor.LoggedIn && (iMode == Statics.Enums.ONLINE_MODE.ONLINE))
                {
                    if (!m_ServerInitDone.Contains(PlayerCache.Vitals.Name))
                    {
                        init_LoggedIn_Server();
                    }
                    updateFishIDs();
                    if (state != STATE.STOPPED)
                    {
                        FishStatsDataSet.Access.MergeFishIDsToLocal();
                    }
                }

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::SetOnlineStatus: " + e.ToString());
            }
        }
        #endregion Initialization

        #region User Settings
        public override void ShowSettingsDialog(Iocaine2.Iocaine_2_Form iParent)
        {
            if (ChangeMonitor.LoggedIn == true)
            {
                Fisher_Settings_Form FisherSettingsForm = new Fisher_Settings_Form(
                iParent.Location.X + (iParent.Size.Width / 2), iParent.Location.Y + (iParent.Size.Height / 2), Statics.Datasets.UserTripNames);
                FisherSettingsForm.DataUpdated -= Update_UserSettings;
                FisherSettingsForm.DataUpdated += Update_UserSettings;
                FisherSettingsForm.Show();
            }
            else
            {
                MessageBox.Show("Must be logged in to modify settings.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }
        private void Init_UserSettings()
        {
            Statics.Settings.Fisher.KillFish = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "KillFish"));
            Statics.Settings.Fisher.KillFishFixed = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "KillFishFixed"));
            Statics.Settings.Fisher.KillFishFixedTime = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHER, "KillFishFixedTime"));
            Statics.Settings.Fisher.KillFishRand = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "KillFishRand"));
            Statics.Settings.Fisher.KillFishRandTimeMin = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHER, "KillFishRandTimeMin"));
            Statics.Settings.Fisher.KillFishRandTimeMax = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHER, "KillFishRandTimeMax"));
            Statics.Settings.Fisher.KillFishProp = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "KillFishProp"));
            Statics.Settings.Fisher.KillFishPropTimeMin = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHER, "KillFishPropTimeMin"));
            Statics.Settings.Fisher.KillFishPropTimeMax = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHER, "KillFishPropTimeMax"));
            Statics.Settings.Fisher.DoneWait = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "DoneWait"));
            Statics.Settings.Fisher.DoneStop = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "DoneStop"));
            Statics.Settings.Fisher.DoneShutdown = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "DoneShutdown"));
            Statics.Settings.Fisher.DoneChange = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "DoneChange"));
            Statics.Settings.Fisher.DoneNav = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "DoneNav"));
            Statics.Settings.Fisher.DoneNavTrip = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.FISHER, "DoneNavTrip"));
            Statics.Settings.Fisher.FullStop = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "FullStop"));
            Statics.Settings.Fisher.FullShutdown = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "FullShutdown"));
            Statics.Settings.Fisher.FullChange = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "FullChange"));
            Statics.Settings.Fisher.FullContinue = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "FullContinue"));
            Statics.Settings.Fisher.FullNav = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "FullNav"));
            Statics.Settings.Fisher.FullNavTrip = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.FISHER, "FullNavTrip"));
            Statics.Settings.Fisher.FatiguedNav = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "FatiguedNav"));
            Statics.Settings.Fisher.FatiguedNavTrip = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.FISHER, "FatiguedNavTrip"));
            Statics.Settings.Fisher.FatigueThreshold = Convert.ToUInt16(UserSettings.GetValue(UserSettings.BOT.FISHER, "FatigueThreshold"));
            Statics.Settings.Fisher.GiveLogoutCommand = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "GiveLogoutCommand"));
            Statics.Settings.Fisher.LogoutCommand = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.FISHER, "LogoutCommand"));
            Statics.Settings.Fisher.FishByID = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "FishByID"));
            Statics.Settings.Fisher.FishByHP = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "FishByHP"));
            Statics.Settings.Fisher.DropItems = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "DropItems"));
            Statics.Settings.Fisher.DropMobs = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "DropMobs"));
            Statics.Settings.Fisher.FishByHPMin = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHER, "FishByHPMin"));
            Statics.Settings.Fisher.FishByHPMax = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHER, "FishByHPMax"));
            Statics.Settings.Fisher.NoCatchTimeout = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHER, "NoCatchTimeout"));
            Statics.Settings.Fisher.ReleaseFixed = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "ReleaseFixed"));
            Statics.Settings.Fisher.ReleaseTime = Convert.ToDouble(UserSettings.GetValue(UserSettings.BOT.FISHER, "ReleaseTime"));
            Statics.Settings.Fisher.ReleaseRandom = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "ReleaseRandom"));
            Statics.Settings.Fisher.ReleaseTimeRandomMin = Convert.ToDouble(UserSettings.GetValue(UserSettings.BOT.FISHER, "ReleaseTimeRandomMin"));
            Statics.Settings.Fisher.ReleaseTimeRandomMax = Convert.ToDouble(UserSettings.GetValue(UserSettings.BOT.FISHER, "ReleaseTimeRandomMax"));
            Statics.Settings.Fisher.TimedStart = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "TimedStart"));
            Statics.Settings.Fisher.StartTime = Convert.ToDateTime(UserSettings.GetValue(UserSettings.BOT.FISHER, "StartTime"));
            Statics.Settings.Fisher.TimedEnd = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "TimedEnd"));
            Statics.Settings.Fisher.EndTime = Convert.ToDateTime(UserSettings.GetValue(UserSettings.BOT.FISHER, "EndTime"));
            Statics.Settings.Fisher.FisherDonePlaySound = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.FISHER, "DonePlaySound"));
            Statics.Settings.Fisher.FisherFullPlaySound = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.FISHER, "FullPlaySound"));
            Statics.Settings.Fisher.MoveInv = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.FISHER, "MoveInv"));
            Statics.Settings.Fisher.RecastDelay = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHER, "InitWaitDelay"));
        }
        private void Update_UserSettings()
        {
            UserSettings.SetValue(UserSettings.BOT.FISHER, "KillFish", Statics.Settings.Fisher.KillFish.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "KillFishFixed", Statics.Settings.Fisher.KillFishFixed.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "KillFishFixedTime", Statics.Settings.Fisher.KillFishFixedTime.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "KillFishRand", Statics.Settings.Fisher.KillFishRand.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "KillFishRandTimeMin", Statics.Settings.Fisher.KillFishRandTimeMin.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "KillFishRandTimeMax", Statics.Settings.Fisher.KillFishRandTimeMax.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "KillFishProp", Statics.Settings.Fisher.KillFishProp.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "KillFishPropTimeMin", Statics.Settings.Fisher.KillFishPropTimeMin.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "KillFishPropTimeMax", Statics.Settings.Fisher.KillFishPropTimeMax.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "DoneWait", Statics.Settings.Fisher.DoneWait.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "DoneStop", Statics.Settings.Fisher.DoneStop.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "DoneShutdown", Statics.Settings.Fisher.DoneShutdown.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "DoneChange", Statics.Settings.Fisher.DoneChange.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "DoneNav", Statics.Settings.Fisher.DoneNav.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "DoneNavTrip", Statics.Settings.Fisher.DoneNavTrip.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "FullStop", Statics.Settings.Fisher.FullStop.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "FullShutdown", Statics.Settings.Fisher.FullShutdown.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "FullChange", Statics.Settings.Fisher.FullChange.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "FullContinue", Statics.Settings.Fisher.FullContinue.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "FullNav", Statics.Settings.Fisher.FullNav.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "FullNavTrip", Statics.Settings.Fisher.FullNavTrip.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "FatiguedNav", Statics.Settings.Fisher.FatiguedNav.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "FatiguedNavTrip", Statics.Settings.Fisher.FatiguedNavTrip.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "FatigueThreshold", Statics.Settings.Fisher.FatigueThreshold.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "GiveLogoutCommand", Statics.Settings.Fisher.GiveLogoutCommand.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "LogoutCommand", Statics.Settings.Fisher.LogoutCommand.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "FishByID", Statics.Settings.Fisher.FishByID.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "FishByHP", Statics.Settings.Fisher.FishByHP.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "DropItems", Statics.Settings.Fisher.DropItems.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "DropMobs", Statics.Settings.Fisher.DropMobs.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "FishByHPMin", Statics.Settings.Fisher.FishByHPMin.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "FishByHPMax", Statics.Settings.Fisher.FishByHPMax.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "NoCatchTimeout", Statics.Settings.Fisher.NoCatchTimeout.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "ReleaseFixed", Statics.Settings.Fisher.ReleaseFixed.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "ReleaseTime", Statics.Settings.Fisher.ReleaseTime.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "ReleaseRandom", Statics.Settings.Fisher.ReleaseRandom.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "ReleaseTimeRandomMin", Statics.Settings.Fisher.ReleaseTimeRandomMin.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "ReleaseTimeRandomMax", Statics.Settings.Fisher.ReleaseTimeRandomMax.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "TimedStart", Statics.Settings.Fisher.TimedStart.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "StartTime", Statics.Settings.Fisher.StartTime.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "TimedEnd", Statics.Settings.Fisher.TimedEnd.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "EndTime", Statics.Settings.Fisher.EndTime.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "DonePlaySound", Statics.Settings.Fisher.FisherDonePlaySound);
            UserSettings.SetValue(UserSettings.BOT.FISHER, "FullPlaySound", Statics.Settings.Fisher.FisherFullPlaySound);
            UserSettings.SetValue(UserSettings.BOT.FISHER, "MoveInv", Statics.Settings.Fisher.MoveInv.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHER, "InitWaitDelay", Statics.Settings.Fisher.RecastDelay.ToString());
        }
        #endregion User Settings

        #region Status Related
        public override bool Start()
        {
            if (!Statics.Flags.VersionCheckOk)
            {
                MessageBox.Show("Please update before proceding");
                return false;
            }
            if (base.Start())
            {
                UpdateTimeStamps();
                FishStatsDataSet.Access.MergeFishIDsToLocal();
                return true;
            }
            return false;
        }
        public override void Resume()
        {
            try
            {
                setCurrentBaitBoxItem(true);
                base.Resume();
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::Resume: " + e.ToString());
            }
        }
        protected override void runThreadFunction()
        {
            try
            {
                startFishing();
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::runThreadFunction: " + e.ToString());
            }
        }
        protected override bool checkState()
        {
            try
            {
                if (VanaTime.LastJapaneseMidnightUTC > m_lastJpMidnight)
                {
                    if (Statics.Settings.Top.AutoReset)
                    {
                        filterFishStatsRows();
                        c_StatsBoxLoad();
                    }
                    m_lastJpMidnight = VanaTime.LastJapaneseMidnightUTC;
                }
                switch (state)
                {
                    case Bots.STATE.STOPPED:
                        Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot", Statics.Fields.Blue);
                        return false;
                    case Bots.STATE.RUNNING:
                        //Check for timed start and/or end
                        if (!Statics.Settings.Fisher.TimedStart && !Statics.Settings.Fisher.TimedEnd)
                        {
                            return true;
                        }
                        else if (!Statics.Settings.Fisher.TimedStart && Statics.Settings.Fisher.TimedEnd)
                        {
                            if (DateTime.Now >= Statics.Settings.Fisher.EndTime)
                            {
                                if (Statics.Settings.Fisher.DoneStop || Statics.Settings.Fisher.DoneWait)
                                {
                                    Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot due to Timed End settings", Statics.Fields.Blue);
                                    LoggingFunctions.Timestamp("Stopping bot due to Timed End settings");
                                    if (Statics.Settings.Fisher.GiveLogoutCommand)
                                    {
                                        IocaineFunctions.delay(10000);
                                        LoggingFunctions.Timestamp(Statics.Settings.Fisher.LogoutCommand);
                                        IocaineFunctions.keys(Statics.Settings.Fisher.LogoutCommand);
                                        IocaineFunctions.delay(30000);
                                    }
                                }
                                else if (Statics.Settings.Fisher.DoneShutdown)
                                {
                                    Statics.FuncPtrs.SetStatusBoxPtr("Shutting down bot due to Timed End settings.", Statics.Fields.Blue);
                                    LoggingFunctions.Timestamp("Shutting down bot due to Timed End settings.");
                                    if (Statics.Settings.Fisher.GiveLogoutCommand)
                                    {
                                        IocaineFunctions.delay(10000);
                                        LoggingFunctions.Timestamp(Statics.Settings.Fisher.LogoutCommand);
                                        IocaineFunctions.keys(Statics.Settings.Fisher.LogoutCommand);
                                        IocaineFunctions.delay(30000);
                                    }
                                    IocaineFunctions.delay(10000);
                                    IocaineFunctions.keys("/shutdown");
                                }
                                else if (Statics.Settings.Fisher.DoneChange)
                                {
                                    // Not implemented //
                                    LoggingFunctions.Timestamp("Changing characters due to bad fish count = " + badFishCount.ToString());
                                    MessageBox.Show("Changing characters function not yet supported... stopping bot.");
                                }
                                else if (Statics.Settings.Fisher.DoneNav)
                                {
                                    LoggingFunctions.Timestamp("DoneNav set, firing event to top level form to begin navigation.");
                                    _FishingDone();
                                    Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot after navigation is complete.", Statics.Fields.Blue);
                                }
                                Stop();
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (Statics.Settings.Fisher.TimedStart && !Statics.Settings.Fisher.TimedEnd)
                        {
                            if (Statics.Settings.Fisher.StartTime > DateTime.Now)
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Waiting for fishing Start Time...", Statics.Fields.Yellow);
                                while (Statics.Settings.Fisher.StartTime > DateTime.Now)
                                {
                                    IocaineFunctions.delay(2500);
                                    if (state == Bots.STATE.STOPPED)
                                    {
                                        Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot", Statics.Fields.Blue);
                                        return false;
                                    }
                                }
                                Statics.FuncPtrs.SetStatusBoxPtr("Starting time reached, beginning to fish", Statics.Fields.Blue);
                                chatLog.Reset();
                                chatLog2.Reset();
                                chatLog3.Reset();
                                return true;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (Statics.Settings.Fisher.TimedStart && Statics.Settings.Fisher.TimedEnd)
                        {
                            if ((DateTime.Now >= Statics.Settings.Fisher.StartTime) && (DateTime.Now < Statics.Settings.Fisher.EndTime))
                            {
                                return true;
                            }
                            else if (DateTime.Now < Statics.Settings.Fisher.StartTime)
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Waiting for fishing Start Time...", Statics.Fields.Yellow);
                                while (Statics.Settings.Fisher.StartTime > DateTime.Now)
                                {
                                    IocaineFunctions.delay(2500);
                                    if (state == Bots.STATE.STOPPED)
                                    {
                                        Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot", Statics.Fields.Blue);
                                        return false;
                                    }
                                }
                                Statics.FuncPtrs.SetStatusBoxPtr("Starting time reached, beginning to fish", Statics.Fields.Blue);
                                chatLog.Reset();
                                chatLog2.Reset();
                                chatLog3.Reset();
                                return true;
                            }
                            else if (DateTime.Now >= Statics.Settings.Fisher.EndTime)
                            {
                                Statics.Settings.Fisher.StartTime = Statics.Settings.Fisher.StartTime.AddDays(1);
                                Statics.Settings.Fisher.EndTime = Statics.Settings.Fisher.EndTime.AddDays(1);
                                Statics.FuncPtrs.SetStatusBoxPtr("Waiting for fishing Start Time...", Statics.Fields.Yellow);
                                while (Statics.Settings.Fisher.StartTime > DateTime.Now)
                                {
                                    IocaineFunctions.delay(2500);
                                    if (state == Bots.STATE.STOPPED)
                                    {
                                        Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot", Statics.Fields.Blue);
                                        return false;
                                    }
                                }
                                Statics.FuncPtrs.SetStatusBoxPtr("Starting time reached, beginning to fish", Statics.Fields.Blue);
                                chatLog.Reset();
                                chatLog2.Reset();
                                chatLog3.Reset();
                                return true;
                            }
                        }
                        return true;
                    case STATE.PAUSED_USER_NOW:
                    case STATE.PAUSED_USER_WAIT:
                    case STATE.PAUSED_ALR:
                        Statics.FuncPtrs.SetStatusBoxPtr("Pausing bot", Statics.Fields.Yellow);
                        LoggingFunctions.Timestamp("Pausing bot.");
                        while (state != Bots.STATE.RUNNING)
                        {
                            if (state == Bots.STATE.STOPPED)
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot", Statics.Fields.Blue);
                                return false;
                            }
                            IocaineFunctions.delay(1000);
                            if (ChangeMonitor.MainProc.HasExited)
                            {
                                state = Bots.STATE.STOPPED;
                            }
                        }
                        chatLog.Reset();
                        chatLog2.Reset();
                        chatLog3.Reset();
                        return true;
                    case Bots.STATE.PAUSED_PROG:
                        while (state != Bots.STATE.RUNNING)
                        {
                            if (waitForNextDay)     //System pause for no catch timeout
                            {
                                while (((DateTime.UtcNow.Hour != 15) || (DateTime.UtcNow.Minute < 2)) && ((state != Bots.STATE.RUNNING) && (state != Bots.STATE.STOPPED)))
                                {
                                    IocaineFunctions.delay(60 * 1000);
                                }
                                if (state == Bots.STATE.STOPPED)
                                {
                                    //Bot was stopped by user
                                    Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot", Statics.Fields.Blue);
                                    waitForNextDay = false;
                                    return false;
                                }
                                else
                                {
                                    //Bot was resumed by user or we hit JP midnight, either way, resume.
                                    LoggingFunctions.Timestamp("Resuming fishing");
                                    state = Bots.STATE.RUNNING;
                                    waitForNextDay = false;
                                    badFishCount = 0;
                                    noCatchCount = 0;
                                }
                            }
                            else    //System paused for error
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Pausing bot", Statics.Fields.Yellow);
                                //setStartButton("&Resume", Statics.Buttons.Green);
                                IocaineFunctions.delay(1000);
                            }
                            if (ChangeMonitor.MainProc.HasExited)
                            {
                                state = Bots.STATE.STOPPED;
                                return false;
                            }
                        }
                        chatLog.Reset();
                        chatLog2.Reset();
                        chatLog3.Reset();
                        return true;
                    default:
                        return false;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::checkState: Outer: " + e.ToString());
                return false;
            }
        }
        protected override bool checkStatus()
        {
            // Check for end conditions here (for future additions like 'catch only 4 three-eyed fish').
            // This would sort of take over the "onDoneActions" below. Need to reorganize how this is done.
            return true;
        }
        /// <summary>
        /// Perform actions when fishing is done.
        /// </summary>
        /// <returns>True if some action was taken and we should exit any loops we're in, false otherwise.</returns>
        private bool onDoneActions(bool iNoRod, bool iNoBait, bool iNoBiteCount, bool iInvFull)
        {
            String statusBoxAction = "";
            String statusBoxReason = " ";
            String statusBoxText = "";
            try
            {
                if (iNoRod == true)
                {
                    statusBoxReason += "not having a rod equipped.";
                }
                else if (iNoBait == true)
                {
                    statusBoxReason += "not having bait equipped.";
                }
                else if (iNoBiteCount == true)
                {
                    statusBoxReason += "not getting a bite in " + Statics.Settings.Fisher.NoCatchTimeout + " casts.";
                }
                else if (iInvFull == true)
                {
                    statusBoxReason += "inventory being full.";
                }
                else
                {
                    statusBoxReason = "unknown reason.";
                }
                if ((iNoBait || iInvFull) && Statics.Settings.Fisher.MoveInv)
                {
                    if (swapFishAndBait(currentBaitItem.BaitName, true))
                    {
                        if (iNoBait)
                        {
                            equipBait();
                            if (MemReads.Self.Equipment.get_ammo_equipped())
                            {
                                return false;
                            }
                        }
                        else if (iInvFull)
                        {
                            if (Containers.Bag.Occupancy < Containers.Bag.Capacity)
                            {
                                return false;
                            }
                        }
                    }
                }
                if ((!iInvFull && Statics.Settings.Fisher.DoneStop) || (iInvFull && Statics.Settings.Fisher.FullStop))
                {
                    statusBoxAction = "Stopping bot due to";
                    statusBoxText = statusBoxAction + statusBoxReason;
                    Statics.FuncPtrs.SetStatusBoxPtr(statusBoxText, Statics.Fields.Blue);
                    LoggingFunctions.Timestamp(statusBoxText);
                    player.PlaySound(Statics.Settings.Fisher.FisherDonePlaySound);
                    if (Statics.Settings.Fisher.GiveLogoutCommand)
                    {
                        IocaineFunctions.delay(10 * 1000);
                        LoggingFunctions.Timestamp(Statics.Settings.Fisher.LogoutCommand);
                        IocaineFunctions.keys(Statics.Settings.Fisher.LogoutCommand);
                        IocaineFunctions.delay(30 * 1000);
                    }
                    Stop();
                    return true;
                }
                else if (!iInvFull && Statics.Settings.Fisher.DoneWait)
                {
                    statusBoxAction = "Pausing bot due to";
                    statusBoxText = statusBoxAction + statusBoxReason;
                    Statics.FuncPtrs.SetStatusBoxPtr(statusBoxText, Statics.Fields.Blue);
                    LoggingFunctions.Timestamp(statusBoxText);
                    player.PlaySound(Statics.Settings.Fisher.FisherDonePlaySound);
                    waitForNextDay = true;
                    state = Bots.STATE.PAUSED_PROG;    //system pause
                    return true;
                }
                else if ((!iInvFull && Statics.Settings.Fisher.DoneShutdown) || (iInvFull && Statics.Settings.Fisher.FullShutdown))
                {
                    statusBoxAction = "Shutting down due to";
                    statusBoxText = statusBoxAction + statusBoxReason;
                    Statics.FuncPtrs.SetStatusBoxPtr(statusBoxText, Statics.Fields.Blue);
                    LoggingFunctions.Timestamp(statusBoxText);
                    player.PlaySound(Statics.Settings.Fisher.FisherDonePlaySound);
                    if (Statics.Settings.Fisher.GiveLogoutCommand)
                    {
                        IocaineFunctions.delay(10 * 1000);
                        LoggingFunctions.Timestamp(Statics.Settings.Fisher.LogoutCommand);
                        IocaineFunctions.keys(Statics.Settings.Fisher.LogoutCommand);
                        IocaineFunctions.delay(30 * 1000);
                    }
                    IocaineFunctions.delay(10 * 1000);
                    IocaineFunctions.keys("/shutdown");
                    Stop();
                    return true;
                }
                else if ((!iInvFull && Statics.Settings.Fisher.DoneChange) || (iInvFull && Statics.Settings.Fisher.FullChange))
                {
                    statusBoxAction = "Changing characters due to";
                    statusBoxText = statusBoxAction + statusBoxReason;
                    Statics.FuncPtrs.SetStatusBoxPtr(statusBoxText, Statics.Fields.Blue);
                    LoggingFunctions.Timestamp(statusBoxText);
                    player.PlaySound(Statics.Settings.Fisher.FisherDonePlaySound);
                    Stop();
                    return true;
                }
                else if ((!iInvFull && Statics.Settings.Fisher.DoneNav) || (iInvFull && Statics.Settings.Fisher.FullNav))
                {
                    statusBoxAction = "Performing navigation due to";
                    statusBoxText = statusBoxAction + statusBoxReason;
                    Statics.FuncPtrs.SetStatusBoxPtr(statusBoxText, Statics.Fields.Blue);
                    LoggingFunctions.Timestamp(statusBoxText);
                    _FishingDone();
                    Stop();
                    return true;
                }
                else if (iInvFull && Statics.Settings.Fisher.FullContinue)
                {
                    //Should not get to this point. We should not enter this function if full continue is set.
                    return true;
                }
                else
                {
                    statusBoxAction = "Unexpected setting when";
                    statusBoxText = statusBoxAction + statusBoxReason;
                    Statics.FuncPtrs.SetStatusBoxPtr(statusBoxText, Statics.Fields.Blue);
                    LoggingFunctions.Timestamp(statusBoxText);
                    LoggingFunctions.Error("In onDoneAction: statusBoxText = " + statusBoxText);
                    state = Bots.STATE.PAUSED_PROG;
                    return false;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::onDoneActions: " + e.ToString());
                return false;
            }
        }
        #endregion Status Related

        #region Time & Weather
        private void startVanaTimeThread()
        {
            if (vanaTimeThread == null)
            {
                vanaTimeThread = new IocaineThread("vanaTimeThread");
                vanaTimeThread.__RunMethod = vanaTimeThreadFunction;
            }
            vanaTimeThread.Start();
        }
        private void vanaTimeThreadFunction()
        {
            VanaTime initTime = VanaTime.Now;
            int formHour = initTime.Hour;
            int formMin = initTime.Minute;
            int formMonth = initTime.Month;
            int formDay = initTime.DayMonth;
            int formYear = initTime.Year;
            String formDayName = initTime.DayName;
            int formMoonPerc = initTime.MoonPercent;
            String formMoonPhase = initTime.MoonPhase;
            while (vanaTimeThread.__CheckState())
            {
                try
                {
                    VanaTime current = VanaTime.Now;
                    formHour = current.Hour;
                    formMin = current.Minute;
                    formMonth = current.Month;
                    formDay = current.DayMonth;
                    formYear = current.Year;
                    formDayName = current.DayName;
                    formMoonPerc = current.MoonPercent;
                    formMoonPhase = current.MoonPhase;
                    c_timeDateTB_Update(String.Format("{0:0#}:{1:0#}   {2}/{3}/{4}", formHour, formMin, formMonth, formDay, formYear));
                    c_dayTB_Update(formDayName);
                    c_moonTB_Update(formMoonPerc.ToString() + "%  " + formMoonPhase);
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("In vanaTimeThreadFunction: " + e.ToString());
                }
                IocaineFunctions.delay(1200);
            }
        }
        private void updateWeather()
        {
            c_weatherTB_Update(PlayerCache.Environment.WeatherName);
        }
        #endregion Time & Weather

        #region Fishing Functions
        private void startFishing()
        {
            try
            {
                state = Bots.STATE.RUNNING;
                Statics.FuncPtrs.SetStatusBoxPtr("Initializing fisher...", Statics.Fields.Red);
                threadInit();
                badFishCount = 0;
                noCatchCount = 0;
                Statics.FuncPtrs.SetStatusBoxPtr("Starting to fish", Statics.Fields.Green);
                bool alreadyCast = false;
                chatLog.Reset();
                chatLog2.Reset();
                chatLog3.Reset();
                while (alreadyCast || checkState())
                {
                    alreadyCast = false;
                    byte status = MemReads.Self.get_status();
                    ////38:	Fish on hook
                    ////50:	Fishing
                    if ((status == (int)FFXIEnums.STATUS.FISHING) || (status == (int)FFXIEnums.STATUS.FISH_ON_HOOK))
                    {
                        findFish();
                        IocaineFunctions.delay(500);
                        continue;
                    }
                    if (badFishCount >= 2)
                    {
                        equipBait();
                        if (badFishCount >= 3)
                        {
                            equipRod();
                        }
                    }
                    if (badFishCount >= 6)
                    {
                        bool noRod = !MemReads.Self.Equipment.get_range_equipped();
                        bool noBait = !MemReads.Self.Equipment.get_ammo_equipped();
                        if (onDoneActions(noRod, noBait, false, false) == true)
                        {
                            continue;
                        }
                    }
                    if (noCatchCount >= Statics.Settings.Fisher.NoCatchTimeout)
                    {
                        if (onDoneActions(false, false, true, false) == true)
                        {
                            continue;
                        }
                    }
                    while (MemReads.Self.get_status() != (int)FFXIEnums.STATUS.NORMAL)
                    {
                        IocaineFunctions.delay(500);
                    }
                    IocaineFunctions.delay(recastDelay);
                    if (checkState())
                    {
                        //Check to make sure we have rod/bait and we're not full before casting.
                        bool noRod = !MemReads.Self.Equipment.get_range_equipped();
                        bool noBait = !MemReads.Self.Equipment.get_ammo_equipped();
                        bool invFull = Containers.Bag.LiveCapacity == Containers.Bag.LiveOccupancy;
                        if (noRod == true)
                        {
                            equipRod();
                            noRod = !MemReads.Self.Equipment.get_range_equipped();
                        }
                        if (noBait == true)
                        {
                            equipBait();
                            noBait = !MemReads.Self.Equipment.get_ammo_equipped();
                        }
                        if (noRod || noBait || (invFull && !Statics.Settings.Fisher.FullContinue))
                        {
                            if (onDoneActions(noRod, noBait, false, invFull))
                            {
                                continue;
                            }
                        }
                        Statics.FuncPtrs.SetStatusBoxPtr("Casting", Statics.Fields.Green);
                        IocaineFunctions.keys("/fish");
                        IocaineFunctions.delay(3000);
                        bool fatigueIncreased = parseChatLogForFatigue();
                        alreadyCast = true;
                        if (fatigueIncreased == false)
                        {
                            badFishCount++;
                        }
                        else
                        {
                            //Check for fatigue above threshold.
                            if (c_fatigueLabel_GetValue() == Statics.Settings.Fisher.FatigueThreshold)
                            {
                                if (Statics.Settings.Fisher.FatiguedNav)
                                {
                                    alreadyCast = false;
                                    Stop();
                                }
                                _FisherFatigued();
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::startFishing: " + e.ToString());
            }
        }
        private void findFish()
        {
            try
            {
                Statics.FuncPtrs.SetStatusBoxPtr("Waiting for a bite", Statics.Fields.Green);
                badFishCount = 0;
                incCasts();
                bool releaseButtonPressed = false;
                fishStatEntry = new FishStatEntry();
                fishStatEntry.bait = (short)PlayerCache.Equipment.Ammo;
                fishStatEntry.rod = (short)PlayerCache.Equipment.Range;
                fishStatEntry.version = (int)Statics.Flags.IocaineVersion;
                fishStatEntry.weather = (byte)MemReads.Environment.get_weather_id();
                fishStatEntry.xPos = (short)MemReads.Self.Position.get_x();
                fishStatEntry.yPos = (short)MemReads.Self.Position.get_y();
                fishStatEntry.zone = (short)PlayerCache.Environment.ZoneAlias;
                while (MemReads.Self.get_status() == (int)FFXIEnums.STATUS.FISHING)
                {
                    IocaineFunctions.delay(250);
                    if ((state == STATE.PAUSED_USER_NOW) || (state == STATE.PAUSED_ALR))
                    {
                        releaseButtonPressed = true;
                        MenuNavigation.CloseCheck();
                        IocaineFunctions.delay(1000);
                        break;
                    }
                }
                if (MemReads.Self.get_status() == (int)FFXIEnums.STATUS.FISH_ON_HOOK)
                {
                    fightFish();
                    return;
                }
                else
                {
                    if (releaseButtonPressed)
                    {
                        decCasts();
                        LoggingFunctions.Timestamp("Reeled in due to \"Release\" button press");
                        Statics.FuncPtrs.SetStatusBoxPtr("Gave up and reeled in, pausing...", Statics.Fields.Yellow);
                        IocaineFunctions.delay(5 * 1000 + 200);
                    }
                    else
                    {
                        LoggingFunctions.Timestamp("Didn't catch anything");
                        Statics.FuncPtrs.SetStatusBoxPtr("Didn't get a bite, reeling in", Statics.Fields.Green);
                        c_updateStatsBox("Didn't catch anything");
                        addFishStat(FFXIEnums.FISHING_RESULT.DIDNT_CATCH_ANYTHING);
                        noCatchCount++;
                        IocaineFunctions.delay(5 * 1000 + 200);
                    }
                    return;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::findFish: " + e.ToString());
            }
        }
        private void fightFish()
        {
            bool attemptCatch = false;
            bool isMonster = false;
            int currentID1 = 0;
            int currentID2 = 0;
            int currentID3 = 0;
            int currentLarge = 0;
            int maxFishHP = 0;
            int fishItemID = 0;
            try
            {
                noCatchCount = 0;
                currentID1 = MemReads.Fishing.get_id1();
                currentID2 = MemReads.Fishing.get_id2();
                currentID3 = MemReads.Fishing.get_id3();
                currentLarge = MemReads.Fishing.get_large();
                maxFishHP = MemReads.Fishing.get_max_hp();
                FFXIEnums.FISHING_RESULT fishingResult = FFXIEnums.FISHING_RESULT.UNKNOWN;
                isMonster = checkIsMonster();
                String fishOnLine = fishIDtoNameConversion(currentID1, currentID2, currentID3, currentLarge, maxFishHP, isMonster, ref fishItemID);
                LoggingFunctions.Debug("Fisher::fightFish: FishOnLine: " + fishOnLine + ", FishItemID: " + fishItemID + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                if ((fishItemID == 0) && (currentID1 == 60))
                {
                    fishItemID = 2;  //2 is "generic item"
                }
                else if (isMonster == true)
                {
                    fishItemID = 3;  //3 is "monster"
                }
                c_id1Label_Update(currentID1.ToString());
                c_id2Label_Update(currentID2.ToString());
                c_id3Label_Update(currentID3.ToString());
                c_largeLabel_Update(currentLarge);
                c_fishCurHPLabel_Update(maxFishHP.ToString());
                c_fishMaxHPLabel_Update(maxFishHP.ToString());
                c_hpProgressBar_Update(maxFishHP, maxFishHP);
                c_fishNameLabel_Update(fishOnLine);

                LoggingFunctions.Timestamp("ID1: " + currentID1 + ", ID2: " + currentID2 + ", ID3: " + currentID3 + ", large: " + currentLarge + ", Zone: " + PlayerCache.Environment.ZoneId + ", HP: " + maxFishHP);
                if (Statics.Settings.Fisher.CatchAll == true)
                {
                    if ((Statics.Settings.Fisher.DropItems == true) && (currentID1 == 60))
                    {
                        attemptCatch = false;
                    }
                    else if ((Statics.Settings.Fisher.DropMobs == true) && (isMonster == true))
                    {
                        attemptCatch = false;
                    }
                    else
                    {
                        attemptCatch = true;
                    }
                }
                else if (Statics.Settings.Fisher.FishByID == true)
                {
                    attemptCatch = checkForIDCatch(currentID1, currentID2, currentID3, currentLarge, PlayerCache.Environment.ZoneId, maxFishHP, isMonster);
                }
                else if (Statics.Settings.Fisher.FishByHP == true)
                {
                    attemptCatch = checkForHPCatch(maxFishHP) && !((Statics.Settings.Fisher.DropMobs == true) && (isMonster == true));
                }
                else
                {
                    MessageBox.Show("Error: FishByID & FishByHP are both false, check settings.");
                    attemptCatch = false;
                    state = Bots.STATE.PAUSED_PROG;
                    Pause(true);
                }
                if (attemptCatch == false)
                {
                    Statics.FuncPtrs.SetStatusBoxPtr("Undesired catch, letting go", Statics.Fields.Green);
                    LoggingFunctions.Timestamp("Undesired catch, letting go....");
                    releaseCatch();
                    IocaineFunctions.delay(4500);
                    c_id1Label_Update("--");
                    c_id2Label_Update("--");
                    c_id3Label_Update("--");
                    c_largeLabel_Update(2);
                    c_fishCurHPLabel_Update("----");
                    c_fishMaxHPLabel_Update("----");
                    c_hpProgressBar_Update(0, 100);
                    c_fishNameLabel_Update("None");
                    c_updateStatsBox("Unwanted");
                    clearChatLog1();
                    addFishStat(FFXIEnums.FISHING_RESULT.GAVE_UP, fishItemID, maxFishHP);
                    c_baitLB_Refresh(false, true);
                    if (!checkBait())
                    {
                        equipBait();
                        if (!checkBait())
                        {
                            onDoneActions(false, true, false, false);
                        }
                    }
                    return;
                }
                else
                {
                    Statics.FuncPtrs.SetStatusBoxPtr("Reeling in catch", Statics.Fields.Green);
                    LoggingFunctions.Timestamp("Catch will be realed in.");
                    if (Statics.Settings.Fisher.KillFish)
                    {
                        killFishThread = new Thread(new ThreadStart(killFish));
                        killFishThread.Name = "killFishThread";
                        killFishThread.IsBackground = true;
                        killFishThread.Start();
                    }

                    int currHP = MemReads.Fishing.get_cur_hp();
                    UInt16 lastTimer = MemReads.Fishing.get_arrow_timer_value();
                    bool justPressed = false;
                    while (currHP != 0)
                    {
                        if ((state == STATE.PAUSED_USER_NOW) || (state == STATE.PAUSED_ALR))
                        {
                            c_leftArrowImage_Update(Iocaine2.Properties.Resources.Left_arrow);
                            c_rightArrowImage_Update(Iocaine2.Properties.Resources.Right_arrow);
                            if (killFishThread != null)
                            {
                                if (killFishThread.IsAlive)
                                {
                                    killFishThread.Abort();
                                }
                                killFishThread = null;
                            }
                            MenuNavigation.CloseCheck();
                            IocaineFunctions.delay(5 * 1000);
                            addFishStat(FFXIEnums.FISHING_RESULT.GAVE_UP, fishItemID, maxFishHP);
                            Statics.FuncPtrs.SetStatusBoxPtr("Gave up and reeled in, pausing...", Statics.Fields.Yellow);
                            LoggingFunctions.Timestamp("Reeled in due to \"Release\" button press");
                            c_updateStatsBox("Unwanted");
                            c_id1Label_Update("--");
                            c_id2Label_Update("--");
                            c_id2Label_Update("--");
                            c_largeLabel_Update(2);
                            c_fishCurHPLabel_Update("----");
                            c_fishMaxHPLabel_Update("----");
                            c_hpProgressBar_Update(0, 100);
                            c_fishNameLabel_Update("None");
                            c_baitLB_Refresh(false, true);
                            if (!checkBait())
                            {
                                equipBait();
                            }
                            if (!checkRod())
                            {
                                equipRod();
                            }
                            return;
                        }
                        MemReads.FISHING_ARROW_DIR direction = MemReads.Fishing.get_arrow_direction();
                        UInt16 curTimer = MemReads.Fishing.get_arrow_timer_value();
                        bool maskKeyPress = false;
                        if ((curTimer == lastTimer) || justPressed)
                        {
                            maskKeyPress = true;
                            try
                            {
                                IocaineFunctions.delay(100);
                                justPressed = false;
                            }
                            catch
                            {
                                LoggingFunctions.Timestamp("Couldn't load images");
                            }
                        }
                        if ((direction == MemReads.FISHING_ARROW_DIR.LEFT) && (maskKeyPress == false))
                        {

                            try
                            {
                                IocaineFunctions.keyDown(Statics.Settings.Fisher.LeftArrowKey);
                                justPressed = true;
                                c_leftArrowImage_Update(Iocaine2.Properties.Resources.Left_arrow_go);
                                c_rightArrowImage_Update(Iocaine2.Properties.Resources.Right_arrow);
                                IocaineFunctions.delay(100);
                                IocaineFunctions.releaseKey(Statics.Settings.Fisher.LeftArrowKey);
                                c_leftArrowImage_Update(Iocaine2.Properties.Resources.Left_arrow);
                                IocaineFunctions.delay(100);
                            }
                            catch (Exception ex)
                            {
                                LoggingFunctions.Error("Fisher::fightFish: Error pressing left arrow key: \n" + ex.ToString());
                            }
                        }
                        else if (maskKeyPress == false)
                        {
                            try
                            {
                                IocaineFunctions.keyDown(Statics.Settings.Fisher.RightArrowKey);
                                justPressed = true;
                                c_leftArrowImage_Update(Iocaine2.Properties.Resources.Left_arrow);
                                c_rightArrowImage_Update(Iocaine2.Properties.Resources.Right_arrow_go);
                                IocaineFunctions.delay(100);
                                IocaineFunctions.releaseKey(Statics.Settings.Fisher.RightArrowKey);
                                c_rightArrowImage_Update(Iocaine2.Properties.Resources.Right_arrow);
                                IocaineFunctions.delay(100);
                            }
                            catch (Exception ex)
                            {
                                LoggingFunctions.Error("Fisher::fightFish: Error pressing right arrow key: \n" + ex.ToString());
                            }

                        }
                        lastTimer = curTimer;
                        currHP = MemReads.Fishing.get_cur_hp();
                        c_fishCurHPLabel_Update(currHP.ToString());
                        c_hpProgressBar_Update(currHP, maxFishHP);
                    }
                    c_leftArrowImage_Update(Iocaine2.Properties.Resources.Left_arrow);
                    c_rightArrowImage_Update(Iocaine2.Properties.Resources.Right_arrow);
                    if (killFishThread != null)
                    {
                        if (killFishThread.IsAlive)
                        {
                            killFishThread.Join();
                        }
                        killFishThread = null;
                    }
                    for (int kk = 0; kk < 4; kk++)
                    {
                        IocaineFunctions.delay(350);
                        IocaineFunctions.keyDown(System.Windows.Forms.Keys.Enter, 250);
                    }
                    IocaineFunctions.delay(2000);
                    if (MemReads.Self.get_status() != (int)FFXIEnums.STATUS.FISH_CAUGHT) //not a catch
                    {
                        IocaineFunctions.delay(5 * 1000);
                        //Do some accounting here
                        String tempCatch = getLastCatch(ref fishingResult);
                        addFishStat(fishingResult, fishItemID, maxFishHP);
                        Statics.FuncPtrs.SetStatusBoxPtr("Updating counts", Statics.Fields.Green);
                        LoggingFunctions.Timestamp("Last catch was a " + tempCatch);
                        c_updateStatsBox(tempCatch);
                        c_id1Label_Update("--");
                        c_id2Label_Update("--");
                        c_id3Label_Update("--");
                        c_largeLabel_Update(2);
                        c_fishCurHPLabel_Update("----");
                        c_fishMaxHPLabel_Update("----");
                        c_hpProgressBar_Update(0, 100);
                        c_fishNameLabel_Update("None");
                        c_baitLB_Refresh(false, true);
                        if (!checkBait())
                        {
                            equipBait();
                        }
                        if (!checkRod())
                        {
                            equipRod();
                        }
                        return;
                    }
                    else    //Caught something
                    {
                        //Update counters, info box, and fish IDs
                        IocaineFunctions.delay(5 * 1000 + 400);
                        Statics.FuncPtrs.SetStatusBoxPtr("Updating counts", Statics.Fields.Green);
                        lastCatch = getLastCatch(ref fishingResult);
                        LoggingFunctions.Debug("Fisher::fightFish: Last catch was: " + lastCatch + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                        if (fishOnLine == "Unknown")
                        {
                            Fish.FISH_INFO info = Fish.GetFishInfo(lastCatch);
                            fishItemID = info.ItemID;
                        }
                        addFishStat(fishingResult, fishItemID, maxFishHP);
                        if ((fishOnLine == "Unknown") || (lastCatch != "Unknown"))
                        {
                            c_lastCatchLabel_Update(lastCatch);
                            LoggingFunctions.Timestamp("Last catch was a " + lastCatch);
                            c_updateStatsBox(lastCatch, (ushort)fishItemID);
                            bool inventoryFull = ((Containers.Bag.LiveCapacity == Containers.Bag.LiveOccupancy) || (lastCatch == "Release"));
                            if (inventoryFull)
                            {
                                onDoneActions(false, false, false, true);
                            }
                        }
                        else
                        {
                            c_lastCatchLabel_Update(fishOnLine);
                            c_updateStatsBox(fishOnLine);
                            LoggingFunctions.Timestamp("Last catch was a " + fishOnLine);
                        }
                        c_id1Label_Update("--");
                        c_id2Label_Update("--");
                        c_id3Label_Update("--");
                        c_largeLabel_Update(2);
                        c_fishCurHPLabel_Update("----");
                        c_fishMaxHPLabel_Update("----");
                        c_hpProgressBar_Update(0, 100);
                        c_fishNameLabel_Update("None");
                        if ((lastCatch != "Release") && (lastCatch != "1 gil") && (lastCatch != "100 gil"))
                        {
                            fishIDsUpdate(currentID1, currentID2, currentID3, currentLarge, PlayerCache.Environment.ZoneId, maxFishHP);
                        }
                        while (MemReads.Self.get_status() != (int)FFXIEnums.STATUS.NORMAL)
                        {
                            IocaineFunctions.delay(500);
                        }
                        c_baitLB_Refresh(false, true);
                        if (!checkBait())
                        {
                            equipBait();
                        }

                        //Check for item to drop
                        if ((fishOnLine == "Unknown") || (lastCatch != "Unknown"))
                        {
                            if (c_dropLB_Contains(lastCatch))
                            {
                                dropInventory(lastCatch);
                            }
                        }
                        else
                        {
                            if (c_dropLB_Contains(fishOnLine))
                            {
                                dropInventory(fishOnLine);
                            }
                        }
                        return;
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::fightFish: " + e.ToString());
            }
        }
        #endregion Fishing Functions

        #region Data Handling
        #region Table Interactions
        private void updateFishIDs()  //Reasons: zone, rod
        {
            try
            {
                if (checkRod())
                {
                    fishIdsRows = FishStatsDataSet.Access.SelectFishIDs(PlayerCache.Equipment.Range, PlayerCache.Environment.ZoneId);
                }
                else
                {
                    fishIdsRows = null;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::updateFishIDs: SelectFishIDs: " + e.ToString());
            }
            try
            {
                c_loadFishLB();
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::updateFishIDs: _FishIDsUpdated event: " + e.ToString());
            }
        }
        private void saveFishIDs(bool iNow = false)
        {
            try
            {
                if (!init_checkDone())
                {
                    return;
                }
                if (iNow)
                {
                    saveFishIDsCBF();
                }
                else
                {
                    if (saveFishIDsThread == null)
                    {
                        saveFishIDsThread = new Thread(new ThreadStart(saveFishIDsCBF));
                        saveFishIDsThread.Name = "Fisher.saveFishIDsThread";
                        saveFishIDsThread.IsBackground = true;
                        saveFishIDsThread.Start();
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::saveFishIDs: " + e.ToString());
            }
        }
        private void saveFishIDsCBF()
        {
            try
            {
                bool refillAfter;
                FishStatsDataSet.Access.SaveFishIDs(out refillAfter);
                if (refillAfter)
                {
                    updateFishIDs();
                }
                LoggingFunctions.Debug("Saved FishID's to server.", LoggingFunctions.DBG_SCOPE.FISHER);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::saveFishIDsCBF: " + e.ToString());
            }
            finally
            {
                saveFishIDsThread = null;
            }
            return;
        }
        private void saveFishStats(bool iNow = false)
        {
            try
            {
                if (!init_checkDone())
                {
                    return;
                }
                if (iNow)
                {
                    saveFishStatssCBF();
                }
                else
                {
                    if (saveFishStatsThread == null)
                    {
                        saveFishStatsThread = new Thread(new ThreadStart(saveFishStatssCBF));
                        saveFishStatsThread.Name = "Fisher.saveFishStatsThread";
                        saveFishStatsThread.IsBackground = true;
                        saveFishStatsThread.Start();
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::saveFishStats: " + e.ToString());
            }
        }
        private void saveFishStatssCBF()
        {
            try
            {
                FishStatsDataSet.Access.SaveFishStats();
                LoggingFunctions.Debug("Saved FishStats to server.", LoggingFunctions.DBG_SCOPE.FISHER);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::saveFishStatssCBF: " + e.ToString());
            }
            finally
            {
                saveFishStatsThread = null;
            }
            return;
        }
        #endregion Table Interactions
        #region Fish Stats
        private String fishIDtoNameConversion(int ID1, int ID2, int ID3, int Large, int MaxHP, bool IsMonster, ref int fishItemID)
        {
            try
            {
                if (IsMonster == true)
                {
                    return "Monster";
                }
                if (fishIdsRows == null)
                {
                    fishItemID = 0;
                    return "Error - No Fish Found";
                }
                FishStatsDataSet.FishIDsLocalRow localFishIdsRow = null;
                foreach (FishStatsDataSet.FishIDsLocalRow row in fishIdsRows)
                {
                    if ((row.ID1 == ID1) && (row.ID2 == ID2) && (row.Large == (Large == 1)))
                    {
                        localFishIdsRow = row;
                        break;
                    }
                }
                if (localFishIdsRow == null)
                {
                    fishItemID = 0;
                    return "Unknown";
                }
                else
                {
                    fishItemID = localFishIdsRow.Fish;
                    Fish.FISH_INFO info = Fish.GetFishInfo((ushort)fishItemID);
                    return info.FishName;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::fishIDtoNameConversion: " + e.ToString());
                return "Error";
            }
        }
        private bool checkForIDCatch(int ID1, int ID2, int ID3, int Large, int Zone, int maxFishHP, bool isMonster)
        {
            try
            {
                FishStatsDataSet.FishIDsLocalRow localFishIdsRow = null;
                foreach (FishStatsDataSet.FishIDsLocalRow row in fishIdsRows)
                {
                    if ((row.ID1 == ID1) && (row.ID2 == ID2) && (row.Large == (Large == 1)) && (row.Zone == Zone))
                    {
                        localFishIdsRow = row;
                        break;
                    }
                }
                Boolean itsAMob = false;
                if ((Statics.Settings.Fisher.DropMobs == true) && (isMonster == true))
                {
                    itsAMob = true;
                }
                if (localFishIdsRow != null)
                {
                    return Statics.Settings.Fisher.CatchAllExcept ? !localFishIdsRow.Catch : localFishIdsRow.Catch;
                }
                else
                {
                    return Statics.Settings.Fisher.CatchAllExcept ? !itsAMob : false;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::checkForIDCatch: " + e.ToString());
                return false;
            }
        }
        private bool checkForHPCatch(int HP)
        {
            try
            {
                return ((HP >= Statics.Settings.Fisher.FishByHPMin) && (HP <= Statics.Settings.Fisher.FishByHPMax));
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::checkForHPCatch: " + e.ToString());
                return false;
            }
        }
        private bool checkIsMonster()
        {
            ChatLine readString = null;
            bool isMonster = false;
            uint nbLines = 0;
            try
            {
                chatLog3.Update(ref nbLines);
                while (chatLog3.Read(out readString))
                {
                    LoggingFunctions.Debug("Fisher::CheckIsMonster: " + readString.Mode.ToString() + ": " + readString + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                    if (readString.Mode == FFXIEnums.CHAT_MODE.FISH_MESSAGE)
                    {
                        if (readString.ProcessedLine.Contains("Something clamps onto your line ferociously!"))
                        {
                            isMonster = true;
                            continue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::checkIsMonster: " + e.ToString());
                return true;
            }
            return isMonster;
        }
        private void fishIDsUpdate(int ID1, int ID2, int ID3, int Large, int Zone, int HP)
        {
            try
            {
                FishStatsDataSet.FishIDsLocalRow localFishIdsRow = null;
                int idCnt = 0;
                foreach (FishStatsDataSet.FishIDsLocalRow row in fishIdsRows)
                {
                    if ((row.ID1 == ID1) && (row.ID2 == ID2) && (row.Large == (Large == 1)) && (row.Zone == Zone))
                    {
                        //We only update the 1st matching row. Not sure why we would have more than 1.
                        if (localFishIdsRow == null)
                        {
                            localFishIdsRow = row;
                        }
                        idCnt++;
                    }
                }
                switch (idCnt)
                {
                    case 0:
                        //Update with new row
                        Fish.FISH_INFO info = Fish.GetFishInfo(lastCatch);
                        if (info.ItemID == Fish.InvalidID)
                        {
                            LoggingFunctions.Error("Updating fish ID's table, last catch was a " + lastCatch);
                        }
                        else
                        {
                            FishStatsDataSet.FishIDsLocalRow row = FishStatsDataSet.Access.NewFishIDsRow();
                            row.Fish = info.ItemID;
                            row.ID1 = (short)ID1;
                            row.ID2 = (short)ID2;
                            row.ID3 = (short)ID3;
                            row.Zone = (short)Zone;
                            row.minHP = HP;
                            row.maxHP = HP;
                            row.Rod = PlayerCache.Equipment.Range;
                            row.Catch = false;
                            row.Large = (Large == 1);

                            fishIdsRows.Add(row);
                            FishStatsDataSet.Access.AddFishIDs(row);

                            //Add row to the list box on main form
                            c_loadFishLB();
                            LoggingFunctions.Timestamp("Added new fishID to list. Thanks for helping!");
                        }
                        break;
                    default:
                        //Check HP, update if needed
                        if (localFishIdsRow.minHP > HP)
                        {
                            LoggingFunctions.Timestamp("Fish ID's update, setting new minHP from " + localFishIdsRow.minHP + " to " + HP);
                            localFishIdsRow.minHP = HP;
                            FishStatsDataSet.Access.UpdateFishIDs(localFishIdsRow);
                        }
                        else if (localFishIdsRow.maxHP < HP)
                        {
                            LoggingFunctions.Timestamp("Fish ID's update, setting new maxHP from " + localFishIdsRow.maxHP + " to " + HP);
                            localFishIdsRow.maxHP = HP;
                            FishStatsDataSet.Access.UpdateFishIDs(localFishIdsRow);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::fishIDsUpdate: Settings values: " + e.ToString());
            }
            try
            {
                saveFishIDs();
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::fishIDsUpdate: Saving tables: " + e.ToString());
            }
        }
        private void addFishStat(FFXIEnums.FISHING_RESULT iResult)
        {
            try
            {
                fishStatEntry.date = VanaTime.Now.ToEarth();
                fishStatEntry.day = (byte)(VanaTime.Now.Day % 8);
                fishStatEntry.fatigue = (byte)currentFatigue;
                fishStatEntry.fish = 0;
                fishStatEntry.fishHp = 0;
                fishStatEntry.moon = (byte)VanaTime.Now.MoonPercentFullSwing;
                fishStatEntry.result = (byte)iResult;
                fishStatEntry.skill = (byte)MemReads.Self.Skills.Crafting.get_fish_skill();
                fishStatEntry.time = (short)((VanaTime.Now.Hour * 60) + VanaTime.Now.Minute);
                clearChatLog1();
                addFishStat(fishStatEntry);
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::addFishStat(FISHING_RESULT): " + e.ToString());
            }
        }
        private void addFishStat(FFXIEnums.FISHING_RESULT result, int fishID, int fishHP)
        {
            if (result == FFXIEnums.FISHING_RESULT.CAUGHT_FISH)
            {
                try
                {
                    Fish.FISH_INFO info = Fish.GetFishInfo((ushort)fishID);
                    if (info.ItemID != Fish.InvalidID)
                    {
                        if (info.Type == (byte)FFXIEnums.CATCH_TYPE.FISH)
                        {
                            result = FFXIEnums.FISHING_RESULT.CAUGHT_FISH;
                        }
                        else if (info.Type == (byte)FFXIEnums.CATCH_TYPE.ITEM)
                        {
                            result = FFXIEnums.FISHING_RESULT.CAUGHT_ITEM;
                        }
                        else if (info.Type == (byte)FFXIEnums.CATCH_TYPE.MONSTER)
                        {
                            result = FFXIEnums.FISHING_RESULT.CAUGHT_MONSTER;
                        }
                        else
                        {
                            result = FFXIEnums.FISHING_RESULT.UNKNOWN;
                        }
                    }
                    else
                    {
                        result = FFXIEnums.FISHING_RESULT.UNKNOWN;
                    }
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("Fisher::addFishStat(FISHING_RESULT, int, int): Trying to update caught result: " + e.ToString());
                }
            }
            try
            {
                fishStatEntry.date = VanaTime.Now.ToEarth();
                fishStatEntry.day = (byte)(VanaTime.Now.Day % 8);
                fishStatEntry.fatigue = (byte)currentFatigue;
                fishStatEntry.fish = (short)fishID;
                fishStatEntry.fishHp = (short)fishHP;
                fishStatEntry.moon = (byte)VanaTime.Now.MoonPercentFullSwing;
                fishStatEntry.result = (byte)result;
                fishStatEntry.skill = (byte)MemReads.Self.Skills.Crafting.get_fish_skill();
                fishStatEntry.time = (short)((VanaTime.Now.Hour * 60) + VanaTime.Now.Minute);

                addFishStat(fishStatEntry);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::addFishStat(FISHING_RESULT, int, int): Trying to call main add fish stat: " + e.ToString());
            }
        }
        private void addFishStat(FishStatEntry iFishStatEntry)
        {
            try
            {
                Monitor.Enter(fishStatsRows);
                parseChatLog();
                FishStatsDataSet.FishStatsLocalRow statsRow = FishStatsDataSet.Access.NewFishStatsRow();
                statsRow.Bait = iFishStatEntry.bait;
                statsRow.Date = iFishStatEntry.date;
                statsRow.Day = iFishStatEntry.day;
                statsRow.Fatigue = iFishStatEntry.fatigue;
                statsRow.Fish = iFishStatEntry.fish;
                statsRow.FishHP = iFishStatEntry.fishHp;
                statsRow.Info = updateFishStatsInfoData();
                statsRow.Moon = iFishStatEntry.moon;
                statsRow.Player = 0;
                statsRow.Record = 0;
                statsRow.Result = iFishStatEntry.result;
                statsRow.Rod = iFishStatEntry.rod;
                statsRow.Skill = iFishStatEntry.skill;
                statsRow.Synced = false;
                statsRow.Time = iFishStatEntry.time;
                statsRow.Version = iFishStatEntry.version;
                statsRow.Weather = iFishStatEntry.weather;
                statsRow.XPos = iFishStatEntry.xPos;
                statsRow.YPos = iFishStatEntry.yPos;
                statsRow.Zone = iFishStatEntry.zone;
                fishStatsRows.Add(statsRow);
                FishStatsDataSet.Access.AddFishStats(statsRow);

                LoggingFunctions.Debug("Fisher::addFishStat: =====================", LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: Adding fishstats row:", LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: Result:      " + statsRow.Result, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: Fish:        " + statsRow.Fish, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: Rod:         " + statsRow.Rod, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: Bait:        " + statsRow.Bait, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: Day:         " + statsRow.Day, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: Time:        " + statsRow.Time, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: Moon:        " + statsRow.Moon, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: Weather:     " + statsRow.Weather, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: Skill:       " + statsRow.Skill, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: Fatigue:     " + statsRow.Fatigue, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: FishHP:      " + statsRow.FishHP, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: Info:        " + statsRow.Info, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: Zone:        " + statsRow.Zone, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: XPos:        " + statsRow.XPos, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: YPos:        " + statsRow.YPos, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: Date:        " + statsRow.Date, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: Version:     " + statsRow.Version, LoggingFunctions.DBG_SCOPE.FISHER);
                LoggingFunctions.Debug("Fisher::addFishStat: ======================", LoggingFunctions.DBG_SCOPE.FISHER);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::addFishStat(FishStatEntry): Could not update Fish Stats with new record: " + e);
            }
            finally
            {
                Monitor.Exit(fishStatsRows);
            }
            statsSkillUp = 0;
            try
            {
                saveFishStats();
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::addFishStat(FishStatEntry): Saving fish tables: " + e);
            }
        }
        private void filterFishStatsRows()
        {
            //By this point the Statics.Settings.Top.ThisZoneOnly value should be set properly.
            //Either by the top level inits that happend way before this
            //or the checkbox value changed event which calls this after it sets the value.
            try
            {
                Monitor.Enter(fishStatsRows);
                DateTime lastJpMidnight = VanaTime.LastJapaneseMidnightUTC;
                FishStatsDataSet.FishStatsLocalRow[] temp = new FishStatsDataSet.FishStatsLocalRow[fishStatsRows_master.Count];
                fishStatsRows_master.CopyTo(temp);
                fishStatsRows.Clear();
                if (!Statics.Settings.Top.AutoReset && !Statics.Settings.Top.ThisZoneOnly)
                {
                    // No filtering at all, blindly copy everything.
                    fishStatsRows.AddRange(temp);
                }
                else
                {
                    foreach (FishStatsDataSet.FishStatsLocalRow row in temp)
                    {
                        if (Statics.Settings.Top.ThisZoneOnly && (row.Zone != PlayerCache.Environment.ZoneAlias))
                        {
                            continue;
                        }
                        if (Statics.Settings.Top.AutoReset && (row.Date < lastJpMidnight))
                        {
                            continue;
                        }
                        fishStatsRows.Add(row);
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error(e.ToString());
            }
            finally
            {
                Monitor.Exit(fishStatsRows);
            }
        }
        #endregion Fish Stats
        #endregion Data Handling

        #region Utility Functions
        #region Fishing Actions
        private void killFish()
        {
            try
            {
                if (Statics.Settings.Fisher.KillFishFixed)
                {
                    IocaineFunctions.delay((uint)Statics.Settings.Fisher.KillFishFixedTime * 1000);
                }
                else if (Statics.Settings.Fisher.KillFishRand)
                {
                    double waitTime = randVal.NextDouble() + randVal.Next(Statics.Settings.Fisher.KillFishRandTimeMin, Statics.Settings.Fisher.KillFishRandTimeMax - 1);
                    waitTime *= 1000;
                    IocaineFunctions.delay((uint)waitTime);
                }
                else if (Statics.Settings.Fisher.KillFishProp)
                {
                    Int16 maxHP = MemReads.Fishing.get_max_hp();
                    Int16 hpDiff = (Int16)(maxHP - 1000);
                    double percOverMin = (double)hpDiff / (double)(9999 - 1000);
                    double waitTime = (((Statics.Settings.Fisher.KillFishPropTimeMax - Statics.Settings.Fisher.KillFishPropTimeMin) * percOverMin) + Statics.Settings.Fisher.KillFishPropTimeMin);
                    waitTime *= 1000;
                    IocaineFunctions.delay((uint)waitTime);
                }
                else
                {
                    return;
                }
                Statics.FuncPtrs.SetStatusBoxPtr("Killing fish now", Statics.Fields.Green);
                MemReads.Fishing.set_cur_hp((short)0);
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::killFish: " + e.ToString());
            }
        }
        private void releaseCatch()
        {
            try
            {
                if (Statics.Settings.Fisher.ReleaseFixed)
                {
                    IocaineFunctions.delay((uint)(Statics.Settings.Fisher.ReleaseTime * 1000));
                }
                else
                {
                    if (!Statics.Settings.Fisher.ReleaseRandom)
                    {
                        LoggingFunctions.Error("No release radio button was selected on settings form!!!");
                    }
                    double delay;
                    delay = randVal.Next((int)(Statics.Settings.Fisher.ReleaseTimeRandomMin * 1000), (int)(Statics.Settings.Fisher.ReleaseTimeRandomMax * 1000));
                    IocaineFunctions.delay((uint)(delay));
                }
                MenuNavigation.CloseCheck();
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::releaseCatch: " + e.ToString());
            }
        }
        #endregion Fishing Actions
        #region Data Updates
        private void updateZone(ushort iNewZoneId, ushort iOldZoneId)
        {
            try
            {
                Stop();
                saveFishIDs(true);
                //Update fish IDs table & list box
                updateFishIDs();
                if (iOldZoneId != 0)
                {
                    Statics.FuncPtrs.SetStatusBoxPtr("Catch box updated due to zone change", Statics.Fields.Green);
                }
                resetFatigue();
                LoggingFunctions.Debug("Fisher::updateZone: Finished zone update.", LoggingFunctions.DBG_SCOPE.TOP);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::updateZone: Outer: lastZone = " + iOldZoneId + ": " + e.ToString());
            }
        }
        private void updateArea(ushort iNewAreaId, ushort iOldAreaId, ushort iNewZoneAliasId, ushort iOldZoneAliasId)
        {
            try
            {
                Zones.ZONE_INFO info = Zones.GetZoneInfo(iNewZoneAliasId);
                string fullName = info.Zone;
                if (info.AreaName != "NA")
                {
                    fullName += " " + info.AreaName;
                }
                c_zoneLabel_Update(fullName);
                if (Statics.Settings.Top.ThisZoneOnly)
                {
                    filterFishStatsRows();
                    c_StatsBoxLoad();
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::updateArea: " + e.ToString());
            }
        }
        #endregion Data Updates
        #region Rod/Bait Functions
        #region Rod
        private void updateRod(ushort iNewId, ushort iOldId)
        {
            try
            {
                Rods.ROD_INFO rodInfo = Rods.GetRodInfo(PlayerCache.Equipment.Range);
                if (rodInfo.ItemID != Data.Client.Rods.InvalidID)
                {
                    c_rodNameLabel_Update(rodInfo.RodName);
                    saveFishIDs(true);
                    if (iOldId != 0)
                    {
                        Statics.FuncPtrs.SetStatusBoxPtr("Catch box updated due to rod change", Statics.Fields.Green);
                    }
                    updateFishIDs();
                    if (rodInfo.ItemID == 17386)
                    {
                        rodMacroText = "Lu Shang's Fishing Rod";
                    }
                    else
                    {
                        rodMacroText = rodInfo.RodName;
                    }
                    rodId = iNewId;
                    LoggingFunctions.Debug("Fisher::updateRod: Finished rod update.", LoggingFunctions.DBG_SCOPE.TOP);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::updateRod: " + e.ToString());
            }
        }
        private bool checkRod()
        {
            try
            {
                Rods.ROD_INFO info = Rods.GetRodInfo(PlayerCache.Equipment.Range);
                if (info.ItemID == Rods.InvalidID)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::checkRod: " + e.ToString());
                return false;
            }
        }
        private void equipRod()
        {
            try
            {
                byte location = 0xff;
                foreach (EquipmentContainer cntnr in Inventory.Containers.Equipment)
                {
                    if (cntnr.Contains((short)rodId))
                    {
                        location = cntnr.EquipLocation;
                    }
                }
                if (location != 0xff)
                {
                    Statics.FuncPtrs.SetStatusBoxPtr("Equipping new rod", Statics.Fields.Yellow);
                    IocaineFunctions.delay(100);
                    string macro = "/equip range \"" + rodMacroText + "\" " + location + " <me>";
                    IocaineFunctions.keys(macro);
                    LoggingFunctions.Debug("Fisher::equipRod: Rod equip command is " + macro, LoggingFunctions.DBG_SCOPE.FISHER);
                    IocaineFunctions.delay(100);
                }
                else
                {
                    Statics.FuncPtrs.SetStatusBoxPtr("Could not find a '" + Data.Client.Things.GetNameFromId(rodId) + "' to equip.", Statics.Fields.Red);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::equipRod: " + e.ToString());
            }
        }
        #endregion Rod
        #region Bait
        private void updateBait(ushort iNewId, ushort iOldId)
        {
            try
            {
                bool baitMatch = false;
                Bait.BAIT_INFO baitInfo = Bait.GetBaitInfo(PlayerCache.Equipment.Ammo);
                baitMatch = baitInfo.ItemID != Bait.InvalidID;
                if (baitMatch)
                {
                    c_baitNameLabel_Update(baitInfo.BaitName);
                }
                else
                {
                    if (this.state != STATE.STOPPED)
                    {
                        Statics.FuncPtrs.SetStatusBoxPtr("Could not find any selected bait equipped or in inventory.", Statics.Fields.Red);
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::updateBait: " + e.ToString());
            }
        }
        private void updateBaitQuan(byte iNewQuan, byte iOldQuan)
        {
            try
            {
                c_baitQuanLabel_Update(PlayerCache.Equipment.AmmoQuan.ToString());
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::updateBaitQuan: " + e.ToString());
            }
        }
        private bool checkBait()
        {
            try
            {
                Bait.BAIT_INFO info = Bait.GetBaitInfo(PlayerCache.Equipment.Ammo);
                if (info.ItemID == Bait.InvalidID)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::checkBait: " + e.ToString());
                return false;
            }
        }
        private void equipBait()
        {
            try
            {
                //Rebuild our mobile inventory lists.
                Containers.RebuildListsMobileOnly();

                //First check if we have the currentBaitItem in our inventory.
                bool foundBait = false;
                BaitBoxItem baitItem = null;
                bool inEquipable = false;
                byte location = 0;
                foreach (EquipmentContainer cntnr in Containers.Equipment)
                {
                    if (cntnr.Contains(currentBaitItem.BaitName))
                    {
                        inEquipable = true;
                        location = cntnr.EquipLocation;
                        break;
                    }
                }
                if (Containers.Bag.Contains(currentBaitItem.BaitName))
                {
                    foundBait = true;
                    baitItem = currentBaitItem;
                }
                else if (Statics.Settings.Fisher.MoveInv && (Containers.Satchel.Contains(currentBaitItem.BaitName)
                                                          || Containers.Sack.Contains(currentBaitItem.BaitName)
                                                          || Containers.MCase.Contains(currentBaitItem.BaitName)))
                {
                    swapFishAndBait(currentBaitItem.BaitName, false);
                    foundBait = true;
                    baitItem = currentBaitItem;
                }
                else if (inEquipable)
                {
                    foundBait = true;
                    baitItem = currentBaitItem;
                    baitItem.BaitLocation = location;
                }
                else
                {
                    //If we don't have any more of the currentBaitItem, then
                    //go through the list until we find a bait that we actually have, either in Containers.Bag or s/s.
                    for (int ii = 0; ii < baitList.Count; ii++)
                    {
                        if (!baitList[ii].Use)
                        {
                            continue;
                        }
                        string name = baitList[ii].BaitName;
                        foreach (EquipmentContainer cntnr in Containers.Equipment)
                        {
                            if (cntnr.Contains(name))
                            {
                                inEquipable = true;
                                location = cntnr.EquipLocation;
                                break;
                            }
                        }
                        if (Containers.Bag.Contains(name))
                        {
                            baitItem = baitList[ii];
                            foundBait = true;
                            break;
                        }
                        else if (Statics.Settings.Fisher.MoveInv && (Containers.Satchel.Contains(name) || Containers.Sack.Contains(name) || Containers.MCase.Contains(name)))
                        {
                            baitItem = baitList[ii];
                            swapFishAndBait(name, false);
                            foundBait = true;
                            break;
                        }
                        else if (inEquipable)
                        {
                            baitItem = baitList[ii];
                            baitItem.BaitLocation = location;
                            foundBait = true;
                            break;
                        }
                    }
                }
                if (foundBait && baitItem.Use)
                {
                    Statics.FuncPtrs.SetStatusBoxPtr("Equipping new bait", Statics.Fields.Green);
                    IocaineFunctions.delay(100);
                    IocaineFunctions.keys(baitItem.EquipString);
                    setCurrentBaitBoxItem(false);
                }
                else
                {
                    //Can't fish without bait. We actually handle the bot stopping outside of this function, so just return.
                }
                IocaineFunctions.delay(1000);
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::equipBait: " + e.ToString());
            }
        }
        private void setCurrentBaitBoxItem(bool iRebuildInventory)
        {
            try
            {
                //Sets the current bait item based on:
                //1. What's currently equipped.
                //2. If nothing's equipped, based on the highest priority in the bait list (and what we have).
                if (iRebuildInventory)
                {
                    Containers.RebuildListsMobileOnly();
                }
                c_baitLB_Refresh(false, false);
                //First check whether we have a bait/lure equipped.
                //This always takes precedence over what we have in the bait box items list.
                bool baitEquipped = false;
                ushort ammoId = PlayerCache.Equipment.Ammo;
                String baitName = "";
                if (ammoId > 0)
                {
                    baitName = Data.Client.Bait.GetBaitName(ammoId);
                    if (baitName != Data.Client.Bait.InvalidName)
                    {
                        baitEquipped = true;
                    }
                }
                if (baitEquipped)
                {
                    //Find the row in the bait box list that has our currently equipped bait and set that as current.
                    bool foundInList = false;
                    for (int ii = 0; ii < baitList.Count; ii++)
                    {
                        if (baitName == baitList[ii].BaitName)
                        {
                            currentBaitItem = baitList[ii];
                            foundInList = true;
                            return;
                        }
                    }
                    if (!foundInList && (baitList.Count > 0))
                    {
                        currentBaitItem = baitList[0];
                    }
                    else if (baitList.Count == 0)
                    {
                        LoggingFunctions.Error("Found ammo equipped with ID " + ammoId + " but the bait list had zero elements.");
                        currentBaitItem = null;
                    }
                }
                else
                {
                    //If we don't have a bait/lure equipped, then run thru the list to find the one with
                    //the highest priority that we have in our inventory.
                    bool foundInList = false;
                    for (int ii = 0; ii < baitList.Count; ii++)
                    {
                        String name = baitList[ii].BaitName;
                        if (!baitList[ii].Use)
                        {
                            continue;
                        }
                        if (Containers.Bag.Contains(name) || Inventory.Containers.Satchel.Contains(name) || Inventory.Containers.Sack.Contains(name) || Inventory.Containers.MCase.Contains(name))
                        {
                            currentBaitItem = baitList[ii];
                            foundInList = true;
                            break;
                        }
                        else
                        {
                            foreach (EquipmentContainer cntnr in Containers.Equipment)
                            {
                                if (cntnr.Contains(currentBaitItem.BaitName))
                                {
                                    currentBaitItem = baitList[ii];
                                    foundInList = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!foundInList)
                    {
                        LoggingFunctions.Error("Found ammo equipped with ID " + ammoId + " but could not find a bait list item with that bait name.");
                        currentBaitItem = null;
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::setCurrentBaitBoxItem: " + e.ToString());
            }
        }
        #endregion Bait
        #endregion Rod/Bait Functions
        #region Fish Stats
        private int updateFishStatsInfoData()
        {
            try
            {
                statsFishermansTunica = (MemReads.Self.Equipment.get_body_equipped() && (MemReads.Self.Equipment.get_body_id() == (int)FFXIEnums.ITEM_ID.FISHERMANS_TUNICA));
                LoggingFunctions.Debug("Fisher::updateFishStatsInfoData: statsFishermansTunica: " + statsFishermansTunica + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                statsFishermansGloves = (MemReads.Self.Equipment.get_hands_equipped() && (MemReads.Self.Equipment.get_hands_id() == (int)FFXIEnums.ITEM_ID.FISHERMANS_GLOVES));
                LoggingFunctions.Debug("Fisher::updateFishStatsInfoData: statsFishermansGloves: " + statsFishermansGloves + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                statsFishermansHose = (MemReads.Self.Equipment.get_legs_equipped() && (MemReads.Self.Equipment.get_legs_id() == (int)FFXIEnums.ITEM_ID.FISHERMANS_HOSE));
                LoggingFunctions.Debug("Fisher::updateFishStatsInfoData: statsFishermansHose: " + statsFishermansHose + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                statsFishermansBoots = (MemReads.Self.Equipment.get_feet_equipped() && (MemReads.Self.Equipment.get_feet_id() == (int)FFXIEnums.ITEM_ID.FISHERMANS_BOOTS));
                LoggingFunctions.Debug("Fisher::updateFishStatsInfoData: statsFishermansBoots: " + statsFishermansBoots + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                statsAnglersTunica = (MemReads.Self.Equipment.get_body_equipped() && (MemReads.Self.Equipment.get_body_id() == (int)FFXIEnums.ITEM_ID.ANGLERS_TUNICA));
                LoggingFunctions.Debug("Fisher::updateFishStatsInfoData: statsAnglersTunica: " + statsAnglersTunica + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                statsAnglersGloves = (MemReads.Self.Equipment.get_hands_equipped() && (MemReads.Self.Equipment.get_hands_id() == (int)FFXIEnums.ITEM_ID.ANGLERS_GLOVES));
                LoggingFunctions.Debug("Fisher::updateFishStatsInfoData: statsAnglersGloves: " + statsAnglersGloves + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                statsAnglersHose = (MemReads.Self.Equipment.get_legs_equipped() && (MemReads.Self.Equipment.get_legs_id() == (int)FFXIEnums.ITEM_ID.ANGLERS_HOSE));
                LoggingFunctions.Debug("Fisher::updateFishStatsInfoData: statsAnglersHose: " + statsAnglersHose + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                statsAnglersBoots = (MemReads.Self.Equipment.get_feet_equipped() && (MemReads.Self.Equipment.get_feet_id() == (int)FFXIEnums.ITEM_ID.ANGLERS_BOOTS));
                LoggingFunctions.Debug("Fisher::updateFishStatsInfoData: statsAnglersBoots: " + statsAnglersBoots + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                statsWaders = (MemReads.Self.Equipment.get_legs_equipped() && (MemReads.Self.Equipment.get_legs_id() == (int)FFXIEnums.ITEM_ID.WADERS));
                LoggingFunctions.Debug("Fisher::updateFishStatsInfoData: statsWaders: " + statsWaders + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                statsFishermansApron = (MemReads.Self.Equipment.get_body_equipped() && (MemReads.Self.Equipment.get_body_id() == (int)FFXIEnums.ITEM_ID.FISHERMANS_APRON));
                LoggingFunctions.Debug("Fisher::updateFishStatsInfoData: statsFishermansApron: " + statsFishermansApron + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                statsSerpentRumors = false;
                statsFrogFishing = false;
                statsMooching = false;
                statsFishingHoleMap = false;
                statsTheBigOne = false;
                statsFishermansSignBoard = false;
                statsRustyBucket = false;
                statsBlueBambooGrass = false;
                statsGreenBambooGrass = false;
                statsRedBambooGrass = false;
                bool enchantment = false;
                statsImagery = false;
                doIHaveFishingEffects(ref enchantment, ref statsImagery);
                UInt16 ringL = MemReads.Self.Equipment.get_ringL_id();
                UInt16 ringR = MemReads.Self.Equipment.get_ringR_id();
                statsPelicanRing = (enchantment && ((ringL == (short)FFXIEnums.ITEM_ID.PELICAN_RING) || (ringR == (short)FFXIEnums.ITEM_ID.PELICAN_RING)));
                LoggingFunctions.Debug("Fisher::updateFishStatsInfoData: statsPelicanRing: " + statsPelicanRing + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                statsAlbatrossRing = (enchantment && ((ringL == (short)FFXIEnums.ITEM_ID.ALBATROSS_RING) || (ringR == (short)FFXIEnums.ITEM_ID.ALBATROSS_RING)));
                LoggingFunctions.Debug("Fisher::updateFishStatsInfoData: statsAlbatrossRing: " + statsAlbatrossRing + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                statsPenguinRing = (enchantment && ((ringL == (short)FFXIEnums.ITEM_ID.PENGUIN_RING) || (ringR == (short)FFXIEnums.ITEM_ID.PENGUIN_RING)));
                LoggingFunctions.Debug("Fisher::updateFishStatsInfoData: statsPenguinRing: " + statsPenguinRing + ".", LoggingFunctions.DBG_SCOPE.FISHER);

                //Now we'll set the actual bit vector that we'll save to the database
                statsInfoData = 0;
                statsInfoData |= (int)((statsFishermansTunica ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.FISHERMANS_TUNICA);
                statsInfoData |= (int)((statsFishermansGloves ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.FISHERMANS_GLOVES);
                statsInfoData |= (int)((statsFishermansHose ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.FISHERMANS_HOSE);
                statsInfoData |= (int)((statsFishermansBoots ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.FISHERMANS_BOOTS);
                statsInfoData |= (int)((statsAnglersTunica ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.ANGLERS_TUNICA);
                statsInfoData |= (int)((statsAnglersGloves ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.ANGLERS_GLOVES);
                statsInfoData |= (int)((statsAnglersHose ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.ANGLERS_HOSE);
                statsInfoData |= (int)((statsAnglersBoots ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.ANGLERS_BOOTS);
                statsInfoData |= (int)((statsWaders ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.WADERS);
                statsInfoData |= (int)((statsFishermansApron ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.FISHERMANS_APRON);
                statsInfoData |= (int)((statsSerpentRumors ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.SERPENT_RUMORS);
                statsInfoData |= (int)((statsFrogFishing ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.FROG_FISHING);
                statsInfoData |= (int)((statsMooching ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.MOOCHING);
                statsInfoData |= (int)((statsFishingHoleMap ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.FISHING_HOLE_MAP);
                statsInfoData |= (int)((statsTheBigOne ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.THE_BIG_ONE);
                statsInfoData |= (int)((statsFishermansSignBoard ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.FISHERMANS_SIGNBOARD);
                statsInfoData |= (int)((statsRustyBucket ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.RUSTY_BUCKET);
                statsInfoData |= (int)((statsBlueBambooGrass ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.BLUE_BAMBOO_GRASS);
                statsInfoData |= (int)((statsGreenBambooGrass ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.GREEN_BAMBOO_GRASS);
                statsInfoData |= (int)((statsRedBambooGrass ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.RED_BAMBOO_GRASS);
                statsInfoData |= (int)((statsImagery ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.IMAGERY);
                statsInfoData |= (int)((statsPelicanRing ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.PELICAN_RING);
                statsInfoData |= (int)((statsAlbatrossRing ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.ALBATROSS_RING);
                statsInfoData |= (int)((statsPenguinRing ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.PENGUIN_RING);
                statsInfoData |= (int)((Statics.Settings.Fisher.KillFish ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.KILL_FISH);
                //Skill up section
                bool skillBit0 = ((statsSkillUp & 0x1) == 0x1);
                bool skillBit1 = ((statsSkillUp & 0x2) == 0x2);
                bool skillBit2 = ((statsSkillUp & 0x4) == 0x4);
                statsInfoData |= (int)((skillBit0 ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.SKILL_UP_0);
                statsInfoData |= (int)((skillBit1 ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.SKILL_UP_1);
                statsInfoData |= (int)((skillBit2 ? 1 : 0) << (int)FFXIEnums.FISH_INFO_VECTOR_BIT.SKILL_UP_2);
                LoggingFunctions.Debug("Fisher::updateFishStatsInfoData: statsInfoData: " + statsInfoData.ToString("X") + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                return statsInfoData;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::updateFishStatsInfoData: " + e.ToString());
                return 0;
            }
        }
        private void doIHaveFishingEffects(ref bool enchantment, ref bool imagery)
        {
            try
            {
                UInt16[] statusArray = new UInt16[32];
                MemReads.Self.StatusEffects.get_effects(ref statusArray);
                enchantment = imagery = false;
                for (int ii = 0; ii < 32; ii++)
                {
                    if (statusArray[ii] == 0xFFFF)
                    {
                        return;
                    }
                    else if (statusArray[ii] == (UInt16)FFXIEnums.STATUS_EFFECT.Enchantment)
                    {
                        enchantment = true;
                    }
                    else if (statusArray[ii] == (UInt16)FFXIEnums.STATUS_EFFECT.Fishing_Imagery)
                    {
                        imagery = true;
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::doIHaveFishingEffects: " + e.ToString());
            }
        }
        #endregion Fish Stats
        #region Inventory
        private bool swapFishAndBait(String iBait, bool iInitialRebuild)
        {
            bool movedSomething = false;
            try
            {
                if (!Statics.Settings.Fisher.MoveInv)
                {
                    return movedSomething;
                }
                if (iInitialRebuild)
                {
                    Inventory.Containers.RebuildListsMobileOnly();
                }
                //1. Move fish in the fishCaught list into s/s, as many as possible.
                //2. Depending on how many inventory slots we have open, move bait to bag.
                //  >30 : Move nbSlotsAvailable - 25.
                //  20-29 : Move 10.
                //  6-19  : Move nbSlotsAvailable / 2.
                //  4-5   : Move 2.
                //  <4    : Move 1.
                //3. If there are fish remaining the the bag, move them into s/s.

                /////////////////////////////
                // Step 1 - Move fish to s/s.
                /////////////////////////////
                byte nbAvailBag = 0;
                byte nbAvailSatchel = 0;
                byte nbAvailSack = 0;
                byte nbAvailCase = 0;
                for (int zz = 0; zz < 2; zz++)
                {
                    //Do this before and after we move the bait (zz==0 and zz==1).
                    //Move ANY item or bait that's in the main db bait table.
                    List<Data.Client.Fish.FISH_INFO> infoList = Data.Client.Fish.GetAllFishInfo();
                    for (int ii = 0; ii < infoList.Count; ii++)
                    {
                        nbAvailBag = (byte)(Containers.Bag.Capacity - Containers.Bag.Occupancy);
                        nbAvailSatchel = (byte)(Inventory.Containers.Satchel.Capacity - Inventory.Containers.Satchel.Occupancy);
                        nbAvailSack = (byte)(Inventory.Containers.Sack.Capacity - Inventory.Containers.Sack.Occupancy);
                        nbAvailCase = (byte)(Inventory.Containers.MCase.Capacity - Inventory.Containers.MCase.Occupancy);
                        if ((nbAvailSack == 0) && (nbAvailSatchel == 0) && (nbAvailCase == 0))
                        {
                            Inventory.Containers.RebuildListsMobileOnly();
                            break;
                        }

                        ushort fishId = infoList[ii].ItemID;
                        ushort nbFish = Containers.Bag.GetItemQuan(fishId);
                        if (nbFish == 0)
                        {
                            continue;
                        }
                        byte stackSize = Data.Client.Things.GetStackSizeFromId(fishId);
                        ushort nbFishSlots = (ushort)(nbFish / stackSize);
                        if (nbFish % stackSize != 0)
                        {
                            nbFishSlots++;
                        }
                        ushort nbMoved = 0;
                        // CASE //
                        if (nbFishSlots <= nbAvailCase)
                        {
                            Statics.FuncPtrs.SetStatusBoxPtr("Moving fish/items into case.", Statics.Fields.Green);
                            nbMoved += Inventory.Movement.MoveItem(fishId, (ushort)((nbFishSlots * stackSize) - nbMoved), Containers.Bag, Inventory.Containers.MCase, checkState);
                        }
                        else if (nbAvailCase > 0)
                        {
                            Statics.FuncPtrs.SetStatusBoxPtr("Moving fish/items into case.", Statics.Fields.Green);
                            nbMoved += Inventory.Movement.MoveItem(fishId, (ushort)((nbAvailCase * stackSize) - nbMoved), Containers.Bag, Inventory.Containers.MCase, checkState);
                        }
                        if (!checkState())
                        {
                            return (nbMoved != 0);
                        }

                        // SACK //
                        if (nbFishSlots <= nbAvailSack)
                        {
                            Statics.FuncPtrs.SetStatusBoxPtr("Moving fish/items into sack.", Statics.Fields.Green);
                            nbMoved += Inventory.Movement.MoveItem(fishId, (ushort)((nbFishSlots * stackSize) - nbMoved), Containers.Bag, Inventory.Containers.Sack, checkState);
                        }
                        else if (nbAvailSack > 0)
                        {
                            Statics.FuncPtrs.SetStatusBoxPtr("Moving fish/items into sack.", Statics.Fields.Green);
                            nbMoved += Inventory.Movement.MoveItem(fishId, (ushort)((nbAvailSack * stackSize) - nbMoved), Containers.Bag, Inventory.Containers.Sack, checkState);
                        }
                        if (!checkState())
                        {
                            return (nbMoved != 0);
                        }

                        // SATCHEL //
                        if (nbFishSlots <= nbAvailSatchel)
                        {
                            Statics.FuncPtrs.SetStatusBoxPtr("Moving fish/items into satchel.", Statics.Fields.Green);
                            nbMoved += Inventory.Movement.MoveItem(fishId, (ushort)(nbFishSlots * stackSize), Containers.Bag, Inventory.Containers.Satchel, checkState);
                        }
                        else if (nbAvailSatchel > 0)
                        {
                            Statics.FuncPtrs.SetStatusBoxPtr("Moving fish/items into satchel.", Statics.Fields.Green);
                            nbMoved += Inventory.Movement.MoveItem(fishId, (ushort)(nbAvailSatchel * stackSize), Containers.Bag, Inventory.Containers.Satchel, checkState);
                        }
                        Inventory.Containers.RebuildListsMobileOnly();
                        if (nbMoved != 0)
                        {
                            movedSomething = true;
                        }
                        if (!checkState())
                        {
                            return movedSomething;
                        }
                    }

                    /////////////////////////////
                    // Step 2 - Move bait to bag.
                    /////////////////////////////
                    if (zz == 0)
                    {
                        nbAvailBag = (byte)(Containers.Bag.Capacity - Containers.Bag.Occupancy);
                        nbAvailSatchel = (byte)(Inventory.Containers.Satchel.Capacity - Inventory.Containers.Satchel.Occupancy);
                        nbAvailSack = (byte)(Inventory.Containers.Sack.Capacity - Inventory.Containers.Sack.Occupancy);
                        nbAvailCase = (byte)(Inventory.Containers.MCase.Capacity - Inventory.Containers.MCase.Occupancy);
                        ushort nbBaitInSatchel = Inventory.Containers.Satchel.GetItemQuan(iBait);
                        ushort nbBaitInSack = Inventory.Containers.Sack.GetItemQuan(iBait);
                        ushort nbBaitInCase = Inventory.Containers.MCase.GetItemQuan(iBait);
                        ushort nbMoved = Containers.Bag.GetItemQuan(iBait);
                        if ((nbBaitInSack == 0) && (nbBaitInSatchel == 0) && (nbBaitInCase == 0))
                        {
                            Inventory.Containers.RebuildListsMobileOnly();
                            continue;
                        }
                        if (nbAvailBag >= 30)
                        {
                            //Move nb slots - 25.
                            if ((nbMoved < (nbAvailBag - 25)) && (nbBaitInCase > 0))
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Moving bait into bag from case.", Statics.Fields.Green);
                                nbMoved += Inventory.Movement.MoveItem(iBait, (ushort)(nbAvailBag - 25), Inventory.Containers.MCase, Containers.Bag, checkState);
                            }
                            if (!checkState())
                            {
                                return movedSomething;
                            }
                            if ((nbMoved < (nbAvailBag - 25)) && (nbBaitInSack > 0))
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Moving bait into bag from sack.", Statics.Fields.Green);
                                nbMoved += Inventory.Movement.MoveItem(iBait, (ushort)(nbAvailBag - 25), Inventory.Containers.Sack, Containers.Bag, checkState);
                            }
                            if (!checkState())
                            {
                                return movedSomething;
                            }
                            if ((nbMoved < (nbAvailBag - 25)) && (nbBaitInSatchel > 0))
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Moving bait into bag from satchel.", Statics.Fields.Green);
                                nbMoved += Inventory.Movement.MoveItem(iBait, (ushort)(nbAvailBag - 25 - nbMoved), Inventory.Containers.Satchel, Containers.Bag, checkState);
                            }
                            if (!checkState())
                            {
                                return movedSomething;
                            }
                        }
                        else if (nbAvailBag >= 20)
                        {
                            //Move 10.
                            if ((nbMoved < 10) && (nbBaitInCase > 0))
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Moving bait into bag from case.", Statics.Fields.Green);
                                nbMoved += Inventory.Movement.MoveItem(iBait, 10, Inventory.Containers.MCase, Containers.Bag, checkState);
                            }
                            if (!checkState())
                            {
                                return movedSomething;
                            }
                            if ((nbMoved < 10) && (nbBaitInSack > 0))
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Moving bait into bag from sack.", Statics.Fields.Green);
                                nbMoved += Inventory.Movement.MoveItem(iBait, 10, Inventory.Containers.Sack, Containers.Bag, checkState);
                            }
                            if (!checkState())
                            {
                                return movedSomething;
                            }
                            if ((nbMoved < 10) && (nbBaitInSatchel > 0))
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Moving bait into bag from satchel.", Statics.Fields.Green);
                                nbMoved += Inventory.Movement.MoveItem(iBait, (ushort)(10 - nbMoved), Inventory.Containers.Satchel, Containers.Bag, checkState);
                            }
                            if (!checkState())
                            {
                                return movedSomething;
                            }
                        }
                        else if (nbAvailBag > 1)
                        {
                            //Move nb slots / 2.
                            if ((nbMoved < (nbAvailBag / 2)) && (nbBaitInCase > 0))
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Moving bait into bag from case.", Statics.Fields.Green);
                                nbMoved += Inventory.Movement.MoveItem(iBait, (ushort)(nbAvailBag / 2), Inventory.Containers.MCase, Containers.Bag, checkState);
                            }
                            if (!checkState())
                            {
                                return movedSomething;
                            }
                            if ((nbMoved < (nbAvailBag / 2)) && (nbBaitInSack > 0))
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Moving bait into bag from sack.", Statics.Fields.Green);
                                nbMoved += Inventory.Movement.MoveItem(iBait, (ushort)(nbAvailBag / 2), Inventory.Containers.Sack, Containers.Bag, checkState);
                            }
                            if (!checkState())
                            {
                                return movedSomething;
                            }
                            if ((nbMoved < (nbAvailBag / 2)) && (nbBaitInSatchel > 0))
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Moving bait into bag from satchel.", Statics.Fields.Green);
                                nbMoved += Inventory.Movement.MoveItem(iBait, (ushort)((nbAvailBag / 2) - nbMoved), Inventory.Containers.Satchel, Containers.Bag, checkState);
                            }
                            if (!checkState())
                            {
                                return movedSomething;
                            }
                        }
                        Inventory.Containers.RebuildListsMobileOnly();
                        if (nbMoved != 0)
                        {
                            movedSomething = true;
                        }
                        if (!checkState())
                        {
                            return movedSomething;
                        }
                    }
                }
                if (MenuNavigation.GetCurrentOpenMenu() != MenuNavigation.MENU_TYPE.NONE)
                {
                    MenuNavigation.CloseCheck(3);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::swapFishAndBait: " + e.ToString());
            }
            return movedSomething;
        }
        private void dropInventory(String dropItem)
        {
            try
            {
                Statics.FuncPtrs.SetStatusBoxPtr("Dropping item", Statics.Fields.Green);
                MenuNavigation.CloseCheck();                   //Open item window
                IocaineFunctions.twoKeys(System.Windows.Forms.Keys.LControlKey, System.Windows.Forms.Keys.I, 250);
                IocaineFunctions.delay(500);    //Go to bottom
                IocaineFunctions.arrowKeyDown(Keys.Right, 1500);
                IocaineFunctions.delay(500);
                if (true)
                //if(dropItem == MemReads.info_win_item_name(mainProc, ChangeMonitor.MainModule))
                {
                    //Bring up options box (use/drop)
                    IocaineFunctions.keyDown(System.Windows.Forms.Keys.Enter, 100);
                    IocaineFunctions.delay(500);    //Go down to "drop"
                    IocaineFunctions.arrowKeyDown(System.Windows.Forms.Keys.Down, 100);
                    IocaineFunctions.delay(500);    //Select drop, brings up yes/no
                    IocaineFunctions.keyDown(System.Windows.Forms.Keys.Enter, 100);
                    IocaineFunctions.delay(500);    //Go up to "yes"
                    IocaineFunctions.arrowKeyDown(System.Windows.Forms.Keys.Up, 100);
                    IocaineFunctions.delay(500);    //Select yes to drop
                    IocaineFunctions.keyDown(System.Windows.Forms.Keys.Enter, 100);
                    IocaineFunctions.delay(500);
                }
                MenuNavigation.CloseCheck();
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::dropInventory: " + e.ToString());
            }
        }
        #endregion Inventory
        #region Fatigue
        private void incFatigue()
        {
            try
            {
                int fatigue = c_fatigueLabel_GetValue();
                fatigue++;
                c_fatigueLabel_Update(fatigue.ToString());
                recastDelay += incWaitDelay;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::incFatigue: " + e.ToString());
            }
        }
        private bool resetFatigue()
        {
            try
            {
                setFatigue(0);
                return true;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::resetFatigue: " + e.ToString());
                return false;
            }
        }
        private void setFatigue(ushort iFatigue)
        {
            try
            {
                recastDelay = (UInt32)Statics.Settings.Fisher.RecastDelay + ((uint)incWaitDelay * iFatigue);
                c_fatigueLabel_Update(iFatigue.ToString());
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::setFatigue: " + e.ToString());
            }
        }
        #endregion Fatigue
        #region Misc
        public bool UpdateTimeStamps()
        {
            try
            {
                if (Statics.Settings.Fisher.StartTime < fisherStartTimeStamp)
                {
                    DateTime tempTime = new DateTime(fisherStartTimeStamp.Year, fisherStartTimeStamp.Month,
                                                     fisherStartTimeStamp.Day, Statics.Settings.Fisher.StartTime.Hour, Statics.Settings.Fisher.StartTime.Minute, 0);
                    if (tempTime < fisherStartTimeStamp)
                    {
                        tempTime = tempTime.AddDays(1);
                    }
                    Statics.Settings.Fisher.StartTime = tempTime;
                }
                if (Statics.Settings.Fisher.EndTime < fisherStartTimeStamp)
                {
                    DateTime tempTime = new DateTime(fisherStartTimeStamp.Year, fisherStartTimeStamp.Month,
                                                     fisherStartTimeStamp.Day, Statics.Settings.Fisher.EndTime.Hour, Statics.Settings.Fisher.EndTime.Minute, 0);
                    if (tempTime < fisherStartTimeStamp)
                    {
                        tempTime = tempTime.AddDays(1);
                    }
                    Statics.Settings.Fisher.EndTime = tempTime;
                }
                if (Statics.Settings.Fisher.StartTime >= Statics.Settings.Fisher.EndTime)
                {
                    Statics.Settings.Fisher.EndTime = Statics.Settings.Fisher.EndTime.AddDays(1);
                }
                return true;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::UpdateTimeStamps: " + e.ToString());
                return false;
            }
        }
        #endregion Misc
        #region Chat Log
        private String getLastCatch(ref FFXIEnums.FISHING_RESULT fishingResult)
        {
            try
            {
                String fisherman = PlayerCache.Vitals.Name;
                String tempResult = "";
                LoggingFunctions.Debug("Fisher::getLastCatch: Fisher name: " + fisherman + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                uint nbLines = 0;
                chatLog.Update(ref nbLines);
                bool foundResult = false;
                for (int ii = 0; ii < nbLines; ii++)
                {
                    ChatLine tempString = null;
                    if (chatLog.Read(out tempString) && !foundResult)
                    {
                        LoggingFunctions.Debug("Fisher::getLastCatch: Chatlog string read: " + tempString.ProcessedLine + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                        LoggingFunctions.Debug("Fisher::getLastCatch: Chatlog code read: " + tempString.Mode.ToString() + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                        bool hit_caught = tempString.ProcessedLine.Contains(fisherman + " caught") && !tempString.ProcessedLine.Contains("but cannot") && !tempString.ProcessedLine.Contains("monster");
                        LoggingFunctions.Debug("Fisher::getLastCatch: Hit_caught is " + hit_caught + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                        if (hit_caught)
                        {
                            int hit_index = tempString.ProcessedLine.IndexOf("caught");
                            hit_index += 9;
                            if (tempString.ProcessedLine.Contains("caught an"))
                            {
                                hit_index++;
                            }
                            String anotherTempString = tempString.ProcessedLine.Replace("'", "");
                            int temp_len = anotherTempString.Length - hit_index - 1;  //hit index is beginning of fish name, -1 to remove the '!'
                            String name = anotherTempString.Substring(hit_index, temp_len);
                            if (anotherTempString.Contains("caught 2"))
                            {
                                name = name + " 2";
                            }
                            else if (anotherTempString.Contains("caught 3"))
                            {
                                name = name + " 3";
                            }
                            LoggingFunctions.Debug("Fisher::getLastCatch: Returning " + name + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                            tempResult = name;
                            fishingResult = FFXIEnums.FISHING_RESULT.CAUGHT_FISH;
                            foundResult = true;
                        }
                        else if (tempString.ProcessedLine.Contains("didn't catch anything"))
                        {
                            foundResult = true;
                            fishingResult = FFXIEnums.FISHING_RESULT.DIDNT_CATCH_ANYTHING;
                            tempResult = "Didn't catch anything";
                        }
                        else if (tempString.ProcessedLine.Contains("line breaks"))
                        {
                            foundResult = true;
                            fishingResult = FFXIEnums.FISHING_RESULT.LINE_BROKE;
                            tempResult = "Line broke";
                        }
                        else if (tempString.ProcessedLine.Contains(fisherman + " caught a monster"))
                        {
                            foundResult = true;
                            fishingResult = FFXIEnums.FISHING_RESULT.CAUGHT_MONSTER;
                            tempResult = "Monster";
                        }
                        else if (tempString.ProcessedLine.Contains("fishing skill rises"))
                        {
                            LoggingFunctions.Debug("Fisher::getLastCatch: Skill up string is: " + tempString + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                            foundResult = false;
                            int skillUpIndex = tempString.ProcessedLine.IndexOf("0.");
                            skillUpIndex += 2;
                            char skillUpChar = tempString.ProcessedLine[skillUpIndex];
                            LoggingFunctions.Debug("Fisher::getLastCatch: skillUpChar is: " + skillUpChar.ToString() + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                            statsSkillUp = Convert.ToByte(skillUpChar);
                            LoggingFunctions.Debug("Fisher::getLastCatch: statsSkillUp is: " + statsSkillUp + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                        }
                        else if ((tempString.ProcessedLine.Contains("regretfully releases")) || (tempString.ProcessedLine.Contains("but cannot")))
                        {
                            foundResult = true;
                            fishingResult = FFXIEnums.FISHING_RESULT.RELEASED_INV_FULL;
                            tempResult = "Release";
                        }
                        else if (tempString.ProcessedLine.Contains("obtains 1 gil."))
                        {
                            foundResult = true;
                            fishingResult = FFXIEnums.FISHING_RESULT.CAUGHT_ITEM;
                            tempResult = "1 gil";
                        }
                        else if (tempString.ProcessedLine.Contains("lost your catch due to your lack of skill"))
                        {
                            foundResult = true;
                            fishingResult = FFXIEnums.FISHING_RESULT.NOT_ENOUGH_SKILL;
                            tempResult = "Not enough skill";
                        }
                        else if (tempString.ProcessedLine.Contains("lost your catch"))
                        {
                            foundResult = true;
                            fishingResult = FFXIEnums.FISHING_RESULT.GOT_AWAY;
                            tempResult = "Catch got away";
                        }
                        else if (tempString.ProcessedLine.Contains("rod breaks"))
                        {
                            foundResult = true;
                            fishingResult = FFXIEnums.FISHING_RESULT.ROD_BROKE;
                            tempResult = "Rod broke";
                        }
                        else if (tempString.ProcessedLine.Contains("too small"))
                        {
                            foundResult = true;
                            fishingResult = FFXIEnums.FISHING_RESULT.TOO_SMALL;
                            tempResult = "Too small";
                        }
                        else if (tempString.ProcessedLine.Contains("too large"))
                        {
                            foundResult = true;
                            fishingResult = FFXIEnums.FISHING_RESULT.TOO_LARGE;
                            tempResult = "Too large";
                        }
                        else if (tempString.ProcessedLine.Contains("obtains 100 gil."))
                        {
                            foundResult = true;
                            fishingResult = FFXIEnums.FISHING_RESULT.CAUGHT_ITEM;
                            tempResult = "100 gil";
                        }
                        else
                        {
                            fishingResult = FFXIEnums.FISHING_RESULT.UNKNOWN;
                            tempResult = "Unknown";
                        }
                    }
                    else if (foundResult)
                    {
                        LoggingFunctions.Debug("Fisher::getLastCatch: Reading but not parsing: " + tempString + ".", LoggingFunctions.DBG_SCOPE.FISHER);

                    }
                }
                LoggingFunctions.Debug("Fisher::getLastCatch: Returning: " + tempResult + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                return tempResult;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::getLastCatch: " + e.ToString());
                return "Unknown";
            }

        }
        private void clearChatLog1()
        {
            uint nbLines = 0;
            try
            {
                chatLog.Update(ref nbLines);
                ChatLine tempString = null;
                for (int ii = 0; ii < nbLines; ii++)
                {
                    chatLog.Read(out tempString);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Fisher::clearChatLog: " + e.ToString());
            }
        }
        private void parseChatLog()
        {
            ChatLine readString = null;
            try
            {
                while (chatLog2.Read(out readString))
                {
                    LoggingFunctions.Debug("Fisher::parseChatLog: " + readString.Mode + ": " + readString.ProcessedLine + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                    //Check for "We are nearing" to pause bot
                    if ((Byte)readString.Mode == 148)
                    {
                        if (readString.ProcessedLine.Contains("We are nearing "))
                        {
                            LoggingFunctions.Timestamp("Pausing bot due to ferry nearing harbor.");
                            Stop();
                            continue;
                        }
                    }
                    //Check for skill up
                    if (readString.ProcessedLine.Contains("fishing skill rises"))
                    {
                        LoggingFunctions.Debug("Fisher::parseChatLog: Skill up string is: " + readString + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                        int skillUpIndex = readString.ProcessedLine.IndexOf("0.");
                        skillUpIndex += 2;
                        char skillUpChar = readString.ProcessedLine[skillUpIndex];
                        LoggingFunctions.Debug("Fisher::parseChatLog: skillUpChar is: " + skillUpChar.ToString() + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                        statsSkillUp = (byte)(Convert.ToByte(skillUpChar) - 48);
                        if (statsSkillUp > 7)
                        {
                            statsSkillUp = 0;
                        }
                        LoggingFunctions.Debug("Fisher::parseChatLog: statsSkillUp is: " + statsSkillUp + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                        LoggingFunctions.Timestamp("Fishing Skill Up!!! Your skill rose 0." + statsSkillUp);
                        continue;
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::parseChatLog: " + e.ToString());
            }
        }
        private bool parseChatLogForFatigue()
        {
            bool fatigued = false;
            uint nbLines = 0;
            ChatLine readString = null;
            try
            {
                chatLog.Update(ref nbLines);
                for (int ii = 0; ii < nbLines; ii++)
                {
                    if (!chatLog.Read(out readString))
                    {
                        continue;
                    }
                    LoggingFunctions.Debug("Fisher::parseChatLogForFatigue: " + readString.Mode + ": " + readString.ProcessedLine + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                    //Check for "You must wait" message
                    if (readString.Mode == FFXIEnums.CHAT_MODE.ME_BUFF_WEARING)
                    {
                        if (readString.ProcessedLine.Contains("You must wait"))
                        {
                            LoggingFunctions.Timestamp("You must wait longer to perform that action. Incrementing fatigue.");
                            incFatigue();
                            fatigued = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::parseChatLogForFatigue: readString: \"" + readString.ProcessedLine + "\", readCode: " + readString.Mode.ToString() + ": " + e.ToString());
            }
            return fatigued;
        }
        #endregion Chat Log
        #endregion Utility Functions
    }
}
