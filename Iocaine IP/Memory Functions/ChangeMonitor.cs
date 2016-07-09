using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Char;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Threading;

namespace Iocaine2
{
    public static class ChangeMonitor
    {
        #region Enums
        //public enum MONITOR_STATE
        //{
        //    STOPPED = 0,
        //    RUNNING = 1,
        //    PAUSED_USER = 2,
        //    PAUSED_PROG = 3
        //}
        public enum EVENT_TYPE
        {
            PLAYER_STATUS_CHANGED = 0,
            PLAYER_HPP_CHANGED = 1,
            CHAT = 2
        }
        #endregion Enums
        #region Member Variables
        #region Process & Settings
        public static object Padlock = new object();
        private static Process mainProc = null;
        private static ProcessModule mainMod = null;
        //private static MONITOR_STATE state = MONITOR_STATE.STOPPED;
        private static Statics.FuncPtrs.TD_Bool_Void __checkStatus;
        public static Statics.FuncPtrs.TD_Bool_Void __CheckStatus
        {
            set
            {
                __checkStatus = value;
            }
        }
        private static UInt32 loopDelay = 1000;
        #endregion Process & Settings
        #region POL Related
        private static Boolean firstPolFound = false;
        private static Boolean ffxiValid = false;
        private static Boolean loggedIn = false;
        private static String loggedInName = "";
        private static String polWindowTitle = "";
        private static Boolean polProcJustChanged = false;
        #endregion POL Related
        #region Zone Related
        private static Byte mapId = 0;
        private static Boolean mapSet = false;
        private static Boolean forceMapUpdate = false;
        #endregion Zone Related
        #region Inventory Related
        public static UInt32 nbSecondsPerInventoryUpdate = 1;
        #endregion Inventory Related
        #region Misc Info
        #endregion Misc Info
        #region Character Info Related
        private static List<Character> playerList = new List<Character>();
        private static Dictionary<String, Byte> lastPlayerStatus = new Dictionary<string, byte>();
        private static Dictionary<String, Byte> lastPlayerHPP = new Dictionary<string, byte>();
        #endregion Character Info Related
        #endregion Member Variables
        #region Type Defs
        public delegate Boolean CM_Bool_UInt16_Int16_Int16_Int16_out_Byte_TypeDef(UInt16 iZoneID, Int16 iX, Int16 iY, Int16 iZ, out Byte oMapID);
        public static CM_Bool_UInt16_Int16_Int16_Int16_out_Byte_TypeDef GetMapId_FctnPtr = null;
        #endregion Type Defs
        #region Delegates
        #region Process Related
        public delegate void CM_Delegate_POLProcessChanged(bool POLValid);
        public static event CM_Delegate_POLProcessChanged _polProcessChanged;
        public delegate void CM_Delegate_POLLoginChanged(bool LoggedIn, String newCharName);
        public static event CM_Delegate_POLLoginChanged _polLoginChanged;
        public delegate void CM_Delegate_FirstPOLSeen(Process[] PolProcList);
        public static event CM_Delegate_FirstPOLSeen _firstPolSeen;
        public delegate void CM_Delegate_POLWindowTitleChanged(String newTitle);
        public static event CM_Delegate_POLWindowTitleChanged _polWindowTitleChanged;
        #endregion Process Related
        #region My Vitals
        public delegate void CM_Delegate_Vitals_JobChanged();
        public static event CM_Delegate_Vitals_JobChanged _vitals_JobChanged;
        public delegate void CM_Delegate_Vitals_JobLevelChanged();
        public static event CM_Delegate_Vitals_JobLevelChanged _vitals_JobLevelChanged;
        public delegate void CM_Delegate_Vitals_SubJobChanged();
        public static event CM_Delegate_Vitals_SubJobChanged _vitals_SubJobChanged;
        public delegate void CM_Delegate_Vitals_SubJobLevelChanged();
        public static event CM_Delegate_Vitals_SubJobLevelChanged _vitals_SubJobLevelChanged;
        public delegate void CM_Delegate_Vitals_AnyJobChanged();
        public static event CM_Delegate_Vitals_AnyJobChanged _vitals_AnyJobChanged;
        #endregion My Vitals
        #region Zone Related
        public delegate void CM_Delegate_ZoneChanged(ushort newZoneId, ushort oldZoneId);
        public static event CM_Delegate_ZoneChanged _zoneChanged;
        public delegate void CM_Delegate_AreaChanged(ushort newAreaId, ushort oldAreaId, ushort newZoneAliasId, ushort oldZoneAliasId);
        public static event CM_Delegate_AreaChanged _areaChanged;
        public delegate void CM_Delegate_MapChanged(Boolean mapSet, Byte newMapId);
        public static event CM_Delegate_MapChanged _mapChanged;
        public delegate void CM_Delegate_InMogChanged();
        public static event CM_Delegate_InMogChanged _inMogChanged;
        #endregion Zone Related
        #region Inventory Related
        #endregion Inventory Related
        #region Gear Related
        public delegate void CM_Delegate_Equ_CombatSkillChanged();
        public static event CM_Delegate_Equ_CombatSkillChanged _equ_CombatSkillChanged;
        public delegate void CM_Delegate_Equ_MainChanged(ushort newID, ushort oldID);
        public static event CM_Delegate_Equ_MainChanged _equ_MainChanged;
        public delegate void CM_Delegate_Equ_SubChanged(ushort newID, ushort oldID);
        public static event CM_Delegate_Equ_SubChanged _equ_SubChanged;
        public delegate void CM_Delegate_Equ_RangedChanged(ushort newID, ushort oldID);
        public static event CM_Delegate_Equ_RangedChanged _equ_RangeChanged;
        public delegate void CM_Delegate_Equ_AmmoChanged(ushort newID, ushort oldID);
        public static event CM_Delegate_Equ_AmmoChanged _equ_AmmoChanged;
        public delegate void CM_Delegate_Equ_AmmoQuanChanged(byte newID, byte oldID);
        public static event CM_Delegate_Equ_AmmoQuanChanged _equ_AmmoQuanChanged;
        #endregion Gear Related
        #region Misc Info
        public delegate void CM_Delegate_WeatherChanged();
        public static event CM_Delegate_WeatherChanged _weatherChanged;
        #endregion Misc Info
        #region Character Info Related
        public delegate void CM_Delegate_FFXIEvent(FFXIEventArgs e);
        public static event CM_Delegate_FFXIEvent _playerStatusChanged;
        public static event CM_Delegate_FFXIEvent _playerHPPChanged;
        public delegate void CM_Delegate_PlayerNameChanged(String iNewName, String iOldName);
        #endregion Character Info Related
        #endregion Delegates
        #region Subscription Counts
        public static int _playerStatusChangedCount = 0;
        public static int _playerHPPChangedCount = 0;
        #endregion Subscription Counts
        #region Interface Functions/Properties
        #region POL Related
        public static Process MainProc
        {
            get
            {
                return mainProc;
            }
            set
            {
                mainProc = value;
            }
        }
        public static ProcessModule MainModule
        {
            get
            {
                return mainMod;
            }
            set
            {
                mainMod = value;
            }
        }
        public static bool FfxiValid
        {
            get
            {
                return ffxiIsValid();
            }
        }
        public static bool LoggedIn
        {
            get
            {
                if (firstPolFound)
                {
                    //return loggedInCheck();
                    return loggedIn;
                }
                else
                {
                    return false;
                }
            }
        }
        public static bool PolProcJustChanged
        {
            set
            {
                polProcJustChanged = value;
            }
        }
        #endregion POL Related
        /// <summary>
        /// Gets or Sets the delay between each read of the POL memory.
        /// </summary>
        public static uint LoopDelay
        {
            get
            {
                return loopDelay;
            }
            set
            {
                loopDelay = value;
            }
        }
        /// <summary>
        /// Will begin running the monitor. Should be spawned on a new thread after initializing settings.
        /// </summary>
        public static void Run()
        {
            runMonitor();
        }
        public static void AddPlayer(String iPlayerName)
        {
            Character chr = new Character(iPlayerName);
            playerList.Add(chr);
            if (!chr.Check_Active())
            {
                chr.Update_Pointer();
            }
            lastPlayerStatus.Add(chr.Name, chr.Status);
        }
        public static bool RemovePlayer(String iPlayerName)
        {
            Character chr = getCharacter(iPlayerName);
            if (chr == null)
            {
                return false;
            }
            playerList.Remove(chr);
            lastPlayerStatus.Remove(chr.Name);
            return true;
        }
        #endregion Interface Functions/Properties
        #region POL Functions
        private static bool firstPolCheck()
        {
            try
            {
                int nbPolProces = ProcessFunctions.GetNumProcessesByProcessName("pol");
                if (nbPolProces != 0)
                {
                    Process[] polProcs = ProcessFunctions.GetAllProcessByProcessName("pol");
                    if (_firstPolSeen != null)
                    {
                        LoggingFunctions.Debug("Firing first pol seen event.", LoggingFunctions.DBG_SCOPE.CH_MON);
                        _firstPolSeen(polProcs);
                    }
                    firstPolFound = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In firstPolCheck: " + e.ToString());
                return false;
            }
        }
        private static void checkWindowTitleChange()
        {
            try
            {
                if (firstPolFound == false)
                {
                    return;
                }
                if (!mainProc.HasExited && (polWindowTitle != mainProc.MainWindowTitle))
                {
                    String oldVal = polWindowTitle;
                    polWindowTitle = mainProc.MainWindowTitle;
                    if ((polProcJustChanged == false) && (_polWindowTitleChanged != null))
                    {
                        LoggingFunctions.Debug("Firing window title changed event. Old=" + oldVal + ", new=" + polWindowTitle, LoggingFunctions.DBG_SCOPE.CH_MON);
                        _polWindowTitleChanged(polWindowTitle);
                    }
                    else
                    {
                        polProcJustChanged = false;
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In checkWindowTitleChange: " + e.ToString());
            }
        }
        private static bool ffxiIsValid()
        {
            try
            {
                bool lastFfxiIsValid = ffxiValid;
                if (mainProc == null)
                {
                    ffxiValid = false;
                }
                else if (mainMod == null)
                {
                    ffxiValid = false;
                }
                else if (!MemReads.PointersSet)
                {
                    ffxiValid = false;
                }
                else
                {
                    ffxiValid = true;
                }
                if ((lastFfxiIsValid != ffxiValid) && (_polProcessChanged != null))
                {
                    LoggingFunctions.Debug("Firing process changed event.", LoggingFunctions.DBG_SCOPE.CH_MON);
                    _polProcessChanged(ffxiValid);
                }
                //LoggingFunctions.debug("ffxiValid: " + ffxiValid);
                return ffxiValid;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In ffxiIsValid: " + e.ToString());
                return false;
            }
        }
        private static bool loggedInCheck()
        {
            //Monitor.Enter(Padlock);
            try
            {
                bool localLoggedIn = false;
                bool lastLogin = loggedIn;
                String lastLoginName = loggedInName;
                if (ffxiIsValid())
                {
                    if (mainProc.MainWindowHandle == (IntPtr)0)
                    {
                        localLoggedIn = false;
                    }
                    else if (mainProc.MainWindowTitle == null)
                    {
                        //The reason for this is because the main window title seems to occasionally return null even when it's not.
                        //At least that's what I assume because the below else if (mainProc.MainWindowTitle.... was throwing a null
                        //reference exception while the process was logged in for a long time.
                        //So I'm assuming we're logged in since we established above that the main window handle isn't null.
                        return true;
                    }
                    else if (mainProc.MainWindowTitle.ToString().ToLower().Contains("final fantasy xi"))
                    {
                        localLoggedIn = false;
                    }
                    else if (mainProc.MainWindowTitle.ToString().ToLower().Contains("playonline"))
                    {
                        localLoggedIn = false;
                    }
                    else if (mainProc.MainWindowTitle.ToString() == "")
                    {
                        localLoggedIn = false;
                    }
                    else
                    {
                        localLoggedIn = true;
                    }
                }
                else
                {
                    localLoggedIn = false;
                }
                if (localLoggedIn)
                {
                    loggedInName = mainProc.MainWindowTitle;
                    if (loggedInName == "")
                    {
                        IocaineFunctions.delay(100);
                        loggedInName = mainProc.MainWindowTitle;
                    }
                }

                // Update internal status before activating event handlers.
                loggedIn = localLoggedIn;

                // Check for login state change. If so, notify the event handler subscriptions.
                if (((lastLogin != localLoggedIn) || ((lastLoginName != loggedInName) && (loggedInName != ""))) && (_polLoginChanged != null))
                {
                    if (localLoggedIn)
                    {
                        // Since we have the name, the server information linked with that name should be cached as well.
                        try
                        {
                            PlayerCache.Vitals.Name = loggedInName;
                            PlayerCache.Environment.ServerName = MemReads.Environment.get_server(loggedInName);
                            PlayerCache.Environment.ServerId = Data.Client.Servers.GetServerID(PlayerCache.Environment.ServerName);
                            LoggingFunctions.Debug("Firing login changed event (login).", LoggingFunctions.DBG_SCOPE.CH_MON);
                            _polLoginChanged(localLoggedIn, loggedInName);
                        }
                        catch (Exception e)
                        {
                            LoggingFunctions.Error("In loggedInCheck (inner): " + e.ToString());
                            loggedIn = false;
                        }
                    }
                    else
                    {
                        LoggingFunctions.Debug("Firing login changed event (logout).", LoggingFunctions.DBG_SCOPE.CH_MON);
                        _polLoginChanged(localLoggedIn, "");
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In loggedInCheck: " + e.ToString());
                loggedIn = false;
            }
            finally
            {
                //Monitor.Exit(Padlock);
            }
            return loggedIn;
        }
        #endregion POL Functions
        #region My Vitals
        private static Boolean checkJobChange()
        {
            try
            {
                Byte localJobId = MemReads.Self.Job.get_main();
                if ((localJobId != PlayerCache.Vitals.MainJob) && (localJobId != 0))
                {
                    LoggingFunctions.Debug("Detected main job change from " + PlayerCache.Vitals.MainJob + " to " + localJobId, LoggingFunctions.DBG_SCOPE.CH_MON);
                    PlayerCache.Vitals.MainJob = localJobId;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trying to read main job from change monitor: " + e.ToString());
                return false;
            }
        }
        private static Boolean checkJobLevelChange()
        {
            try
            {
                Byte localJobLevel = MemReads.Self.Job.get_main_lvl();
                if ((localJobLevel != PlayerCache.Vitals.MainJobLvl) && (localJobLevel != 0))
                {
                    LoggingFunctions.Debug("Detected main job level change from " + PlayerCache.Vitals.MainJobLvl + " to " + localJobLevel, LoggingFunctions.DBG_SCOPE.CH_MON);
                    PlayerCache.Vitals.MainJobLvl = localJobLevel;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trying to read main job level from change monitor: " + e.ToString());
                return false;
            }
        }
        private static Boolean checkSubJobChange()
        {
            try
            {
                Byte localSubJobId = MemReads.Self.Job.get_sub();
                if ((localSubJobId != PlayerCache.Vitals.SubJob) && (localSubJobId != 0))
                {
                    LoggingFunctions.Debug("Detected sub job change from " + PlayerCache.Vitals.SubJob + " to " + localSubJobId, LoggingFunctions.DBG_SCOPE.CH_MON);
                    PlayerCache.Vitals.SubJob = localSubJobId;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trying to read sub job from change monitor: " + e.ToString());
                return false;
            }
        }
        private static Boolean checkSubJobLevelChange()
        {
            try
            {
                Byte localSubJobLevel = MemReads.Self.Job.get_sub_lvl();
                if ((localSubJobLevel != PlayerCache.Vitals.SubJobLvl) && (localSubJobLevel != 0))
                {
                    LoggingFunctions.Debug("Detected sub job level change from " + PlayerCache.Vitals.SubJobLvl + " to " + localSubJobLevel, LoggingFunctions.DBG_SCOPE.CH_MON);
                    PlayerCache.Vitals.SubJobLvl = localSubJobLevel;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trying to read sub job level from change monitor: " + e.ToString());
                return false;
            }
        }
        private static Boolean checkAnyJobChange()
        {
            Boolean jobChanged = checkJobChange();
            Boolean lvlChanged = checkJobLevelChange();
            Boolean subJobChanged = checkSubJobChange();
            Boolean subLvlChanged = checkSubJobLevelChange();
            if (jobChanged && (_vitals_JobChanged != null))
            {
                _vitals_JobChanged();
            }
            if (lvlChanged && (_vitals_JobLevelChanged != null))
            {
                _vitals_JobLevelChanged();
            }
            if (subJobChanged && (_vitals_SubJobChanged != null))
            {
                _vitals_SubJobChanged();
            }
            if (subLvlChanged && (_vitals_SubJobLevelChanged != null))
            {
                _vitals_SubJobLevelChanged();
            }
            if (jobChanged || lvlChanged || subJobChanged || subLvlChanged)
            {
                if (_vitals_AnyJobChanged != null)
                {
                    _vitals_AnyJobChanged();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion My Vitals
        #region Zone Change Functions
        private static void checkZoneAreaChange()
        {
            LoggingFunctions.Debug("checkZoneAreaChange: Enter.", LoggingFunctions.DBG_SCOPE.CH_MON);
            checkZoneChange();
            checkAreaChange();
        }
        private static void checkZoneChange()
        {
            UInt16 localZone = 0;
            Boolean localInMog = false;
            try
            {
                localZone = MemReads.Self.get_zone_id();
                if ((localZone != PlayerCache.Environment.ZoneId) && (localZone != 0))
                {
                    ushort prevId = PlayerCache.Environment.ZoneId;
                    forceMapUpdate = true;
                    PlayerCache.Environment.ZoneId = localZone;
                    if (_zoneChanged != null)
                    {
                        LoggingFunctions.Debug("Detected zone change to " + PlayerCache.Environment.ZoneId, LoggingFunctions.DBG_SCOPE.CH_MON);
                        _zoneChanged(PlayerCache.Environment.ZoneId, prevId);
                    }
                }
                localInMog = MemReads.Self.get_in_mog_house();
                if(localInMog != PlayerCache.Environment.InMogHouse)
                {
                    PlayerCache.Environment.InMogHouse = localInMog;
                    if (_inMogChanged != null)
                    {
                        LoggingFunctions.Debug("Detected inMog change to " + PlayerCache.Environment.InMogHouse, LoggingFunctions.DBG_SCOPE.CH_MON);
                        _inMogChanged();
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trying to read zone id from change monitor: " + e.ToString());
                LoggingFunctions.Timestamp("localZone: " + localZone + ", zoneID: " + PlayerCache.Environment.ZoneId);
                return;
            }
        }
        private static void checkAreaChange()
        {
            //Since we have to use the zone table, we must assume that,
            //if we zoned from the last check to now, that the checkZoneChange
            //callback function updated the zone table for us and that it
            //now contains the data from the current zone.
            try
            {
                Single xPos = MemReads.Self.Position.get_x();
                Single yPos = MemReads.Self.Position.get_y();
                Single zPos = MemReads.Self.Position.get_z();
                Data.Client.Zones.ZONE_INFO zoneInfo;
                
                UInt16 localAreaId = 0;
                UInt16 localZoneAlias = 0;

                Byte localMapId = 0;
                Boolean localMapSet = false;

                zoneInfo = Data.Client.Zones.GetZoneInfo(PlayerCache.Environment.ZoneId, xPos, yPos);
                localAreaId = zoneInfo.AreaID;
                localZoneAlias = zoneInfo.AliasID;
                if (zoneInfo.AreaName == "NA")
                {
                    PlayerCache.Environment.AreaName = "";
                }
                else
                {
                    PlayerCache.Environment.AreaName = zoneInfo.AreaName;
                }
                PlayerCache.Environment.ZoneName = zoneInfo.Zone;
                
                if (localZoneAlias != PlayerCache.Environment.ZoneAlias)
                {
                    ushort prevAreaId = PlayerCache.Environment.AreaId;
                    ushort prevZoneAlias = PlayerCache.Environment.ZoneAlias;
                    PlayerCache.Environment.AreaId = localAreaId;
                    PlayerCache.Environment.ZoneAlias = localZoneAlias;
                    if (_areaChanged != null)
                    {
                        LoggingFunctions.Debug("Detected area change from to " + PlayerCache.Environment.AreaName, LoggingFunctions.DBG_SCOPE.CH_MON);
                        _areaChanged(PlayerCache.Environment.AreaId, prevAreaId, PlayerCache.Environment.ZoneAlias, prevZoneAlias);
                    }
                }

                localMapSet = GetMapId_FctnPtr(PlayerCache.Environment.ZoneId, (Int16)xPos, (Int16)yPos, (Int16)zPos, out localMapId);
                if(localMapSet != mapSet)
                {
                    mapSet = localMapSet;
                    if (!localMapSet)
                    {
                        if (_mapChanged != null)
                        {
                            _mapChanged(mapSet, 0);
                        }
                    }
                    else
                    {
                        mapId = localMapId;
                        if (_mapChanged != null)
                        {
                            _mapChanged(mapSet, mapId);
                        }
                    }
                }
                else if(mapSet && (localMapId != mapId))
                {
                    mapId = localMapId;
                    if (_mapChanged != null)
                    {
                        _mapChanged(mapSet, mapId);
                    }
                }
                else if(forceMapUpdate)
                {
                    _mapChanged(mapSet, mapId);
                }
                forceMapUpdate = false;
            }
            catch
            {
                //LoggingFunctions.Error("Checking for area in the change monitor: " + e.ToString());
            }
        }
        #endregion Zone Change Functions
        #region Gear Update Functions
        private static void checkEquCombatSkillChange(ushort iNewID, ushort iOldID)
        {
            //checkEquMainChange was already run, so the main slot ID should be set in the statics class.
            try
            {
                if(PlayerCache.Equipment.Main == 0)
                {
                    return;
                }
                Byte currentWeaponType = Data.Client.Things.GetSkillFromId(PlayerCache.Equipment.Main);
                Int32 currentCombatSkillLvl = MemReads.Self.Skills.get_skill(currentWeaponType);
                Boolean typeChanged = false;
                Boolean skillLvlChanged = false;
                if (currentWeaponType != PlayerCache.Skills.CombatSkillType)
                {
                    LoggingFunctions.Debug("Detected combat skill type change from " + PlayerCache.Skills.CombatSkillType + " to " + currentWeaponType, LoggingFunctions.DBG_SCOPE.CH_MON);
                    PlayerCache.Skills.CombatSkillType = currentWeaponType;
                    typeChanged = true;
                }
                if(currentCombatSkillLvl != PlayerCache.Skills.CombatSkillCurrentLvl)
                {
                    LoggingFunctions.Debug("Detected combat skill level change from " + PlayerCache.Skills.CombatSkillType + " to " + currentWeaponType, LoggingFunctions.DBG_SCOPE.CH_MON);
                    PlayerCache.Skills.CombatSkillCurrentLvl = currentCombatSkillLvl;
                    skillLvlChanged = true;
                }
                if (typeChanged || skillLvlChanged)
                {
                    if (_equ_CombatSkillChanged != null)
                    {
                        _equ_CombatSkillChanged();
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("checkEquCombatSkillChange: " + e.ToString());
                return;
            }
        }
        private static void checkEquMainChange()
        {
            try
            {
                ushort localMainId = (ushort)MemReads.Self.Equipment.get_main_id();
                if ((localMainId != PlayerCache.Equipment.Main) && (localMainId != 0))
                {
                    ushort prevId = PlayerCache.Equipment.Main;
                    PlayerCache.Equipment.Main = localMainId;
                    LoggingFunctions.Debug("Detected main equipment id change from " + prevId + " to " + PlayerCache.Equipment.Main, LoggingFunctions.DBG_SCOPE.CH_MON);
                    if (_equ_MainChanged != null)
                    {
                        _equ_MainChanged(PlayerCache.Equipment.Main, prevId);
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trying to read main equipment id from change monitor: " + e.ToString());
                return;
            }
        }
        private static void checkEquSubChange()
        {
            try
            {
                ushort localSubId = (ushort)MemReads.Self.Equipment.get_sub_id();
                if ((localSubId != PlayerCache.Equipment.Sub) && (localSubId != 0))
                {
                    ushort prevId = PlayerCache.Equipment.Sub;
                    PlayerCache.Equipment.Sub = localSubId;
                    LoggingFunctions.Debug("Detected sub equipment id change from " + prevId + " to " + PlayerCache.Equipment.Sub, LoggingFunctions.DBG_SCOPE.CH_MON);
                    if (_equ_SubChanged != null)
                    {
                        _equ_SubChanged(PlayerCache.Equipment.Sub, prevId);
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trying to read sub equipment id from change monitor: " + e.ToString());
                return;
            }
        }
        private static void checkEquRangedChange()
        {
            try
            {
                ushort localRangedId = (ushort)MemReads.Self.Equipment.get_range_id();
                if (localRangedId != PlayerCache.Equipment.Range)
                {
                    ushort prevId = PlayerCache.Equipment.Range;
                    PlayerCache.Equipment.Range = localRangedId;
                    LoggingFunctions.Debug("Detected ranged equipment id change from " + prevId + " to " + localRangedId, LoggingFunctions.DBG_SCOPE.CH_MON);
                    if (_equ_RangeChanged != null)
                    {
                        _equ_RangeChanged(PlayerCache.Equipment.Range, prevId);
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trying to read ranged equipment id from change monitor: " + e.ToString());
                return;
            }
        }
        private static void checkEquAmmoChange()
        {
            try
            {
                ushort localAmmoId = (ushort)MemReads.Self.Equipment.get_ammo_id();
                if (localAmmoId != PlayerCache.Equipment.Ammo)
                {
                    ushort prevId = PlayerCache.Equipment.Ammo;
                    PlayerCache.Equipment.Ammo = localAmmoId;
                    LoggingFunctions.Debug("Detected ammo equipment id change from " + prevId + " to " + localAmmoId, LoggingFunctions.DBG_SCOPE.CH_MON);
                    if (_equ_AmmoChanged != null)
                    {
                        _equ_AmmoChanged(PlayerCache.Equipment.Ammo, prevId);
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trying to read ammo equipment id from change monitor: " + e.ToString());
                return;
            }
        }
        private static void checkEquAmmoQuanChange()
        {
            try
            {
                byte localAmmoQuan = MemReads.Self.Equipment.get_ammo_quan();
                if (localAmmoQuan != PlayerCache.Equipment.AmmoQuan)
                {
                    byte prevQuan = PlayerCache.Equipment.AmmoQuan;
                    PlayerCache.Equipment.AmmoQuan = localAmmoQuan;
                    LoggingFunctions.Debug("Detected ammo equipment quantity change from " + prevQuan + " to " + localAmmoQuan, LoggingFunctions.DBG_SCOPE.CH_MON);
                    if (_equ_AmmoQuanChanged != null)
                    {
                        _equ_AmmoQuanChanged(PlayerCache.Equipment.AmmoQuan, prevQuan);
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trying to read ammo equipment quantity from change monitor: " + e.ToString());
                return;
            }
        }
        #endregion Gear Update Functions
        #region Misc Update Functions
        private static void checkWeatherChange()
        {
            try
            {
                byte localWeatherId = (byte)MemReads.Environment.get_weather_id();
                if (localWeatherId != PlayerCache.Environment.WeatherId)
                {
                    if (_weatherChanged != null)
                    {
                        LoggingFunctions.Debug("Detected weather change from " + PlayerCache.Environment.WeatherId + " to " + localWeatherId, LoggingFunctions.DBG_SCOPE.CH_MON);
                        PlayerCache.Environment.WeatherId = localWeatherId;
                        PlayerCache.Environment.WeatherName = MemReads.Environment.get_weather_name();
                        _weatherChanged();
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Trying to read weather id from change monitor: " + e.ToString());
                return;
            }
        }
        #endregion Misc Update Functions
        #region Utility Functions
        private static Character getCharacter(String iPlayerName)
        {
            Character localChr = null;
            foreach (Character chr in playerList)
            {
                if (chr.Name == iPlayerName)
                {
                    localChr = chr;
                    break;
                }
            }
            return localChr;
        }
        #endregion Utility Functions
        #region Monitor Status
        //public static MONITOR_STATE State
        //{
        //    get
        //    {
        //        return state;
        //    }
        //}
        //private static bool checkStatus()
        //{
        //    switch(state)
        //    {
        //        case MONITOR_STATE.RUNNING:
        //            return true;
        //        case MONITOR_STATE.STOPPED:
        //            return false;
        //        case MONITOR_STATE.PAUSED_PROG:
        //        case MONITOR_STATE.PAUSED_USER:
        //            while(state != MONITOR_STATE.RUNNING)
        //            {
        //                if(state == MONITOR_STATE.STOPPED)
        //                {
        //                    return false;
        //                }
        //                if(mainProc == null)
        //                {
        //                    return false;
        //                }
        //                IocaineFunctions.delay(1000);
        //            }
        //            return true;
        //        default:
        //            return false;
        //    }
        //}
        #endregion Monitor Status
        #region Monitor
        private static void runMonitor()
        {
            UInt32 loopCnt = 0;
            if (__checkStatus == null)
            {
                return;
            }
            _equ_MainChanged += new CM_Delegate_Equ_MainChanged(checkEquCombatSkillChange);
            //state = MONITOR_STATE.RUNNING;
            LoggingFunctions.Debug("Entering monitor loop.", LoggingFunctions.DBG_SCOPE.CH_MON);
            while (__checkStatus())
            {
                #region Process Related
                if (mainProc != null)
                {
                    try
                    {
                        mainProc.Refresh();
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("Trying to refresh mainProc from the change monitor: " + e.ToString());
                        IocaineFunctions.delay(loopDelay);
                        continue;
                    }
                }
                if (!firstPolFound)
                {
                    if (!firstPolCheck())
                    {
                        IocaineFunctions.delay(loopDelay);
                        continue;
                    }
                }
                checkWindowTitleChange();
                if (!loggedInCheck())
                {
                    IocaineFunctions.delay(loopDelay);
                    continue;
                }
                #endregion Process Related
                #region Zone/Info Related
                checkZoneAreaChange();
                checkWeatherChange();
                #endregion Zone/Info Related
                #region Inventory Related
                LoggingFunctions.Debug("loopCnt = " + loopCnt + ".", LoggingFunctions.DBG_SCOPE.CH_MON);
                if (loopCnt % nbSecondsPerInventoryUpdate == 0)
                {
                    Inventory.Containers.RebuildLists();
                }
                #endregion Inventory Related
                #region Gear Related
                checkEquMainChange();
                checkEquRangedChange();
                checkEquAmmoChange();
                checkEquAmmoQuanChange();
                #endregion Gear Related
                #region Vitals
                checkAnyJobChange();
                #endregion Vitals
                #region Player Related
                //LoggingFunctions.debug("_playerStatusChanged Count = " + _playerStatusChangedCount);
                //First check if ANY of the player checks are invoked and if so, make sure all the player pointers are updated.
                if ((_playerStatusChangedCount > 0) ||
                    (_playerHPPChangedCount > 0))
                {
                    foreach (Character chr in playerList)
                    {
                        if (!chr.Check_Active())
                        {
                            chr.Update_Pointer();
                        }
                    }
                }
                if (_playerStatusChangedCount > 0)
                {
                    LoggingFunctions.Debug("_playerStatusChanged Count > 0, checking each player's status for a change.", LoggingFunctions.DBG_SCOPE.CH_MON);
                    foreach (Character chr in playerList)
                    {
                        if (chr.Active)
                        {
                            Byte status = chr.Status;
                            LoggingFunctions.Debug("New status is: " + status + ".", LoggingFunctions.DBG_SCOPE.CH_MON);
                            if (status != lastPlayerStatus[chr.Name])
                            {
                                LoggingFunctions.Debug("Found status change; old: " + lastPlayerStatus[chr.Name] + ", new: " + status + ".", LoggingFunctions.DBG_SCOPE.CH_MON);
                                FFXIEventArgs e = new PlayerStatusChangedEvArgs(chr.Name, playerList.IndexOf(chr), lastPlayerStatus[chr.Name], status);
                                _playerStatusChanged(e);
                                lastPlayerStatus[chr.Name] = status;
                            }
                        }
                    }
                }
                if (_playerHPPChangedCount > 0)
                {
                    foreach (Character chr in playerList)
                    {
                        if (chr.Active)
                        {
                            Byte HPP = chr.HpPerc;
                            if (HPP != lastPlayerHPP[chr.Name])
                            {
                                FFXIEventArgs e = new PlayerHPPChangedEvArgs(chr.Name, playerList.IndexOf(chr), lastPlayerHPP[chr.Name], chr.HpPerc);
                                _playerHPPChanged(e);
                            }
                        }
                    }
                }
                #endregion Player Related
                loopCnt++;
                IocaineFunctions.delay(loopDelay);
            }
        }
        #endregion Monitor
    }
    #region Event Args Classes
    public class FFXIEventArgs : EventArgs
    {
        public FFXIEventArgs(ChangeMonitor.EVENT_TYPE iType)
        {
            type = iType;
        }
        private ChangeMonitor.EVENT_TYPE type;
        public ChangeMonitor.EVENT_TYPE Type
        {
            get
            {
                return type;
            }
        }
    }
    public class PlayerStatusChangedEvArgs : FFXIEventArgs
    {
        #region Constructor
        public PlayerStatusChangedEvArgs(String iName, int iIndex, Byte iOldStatus, Byte iNewStatus)
            : base(ChangeMonitor.EVENT_TYPE.PLAYER_STATUS_CHANGED)
        {
            name = iName;
            playerIndex = iIndex;
            oldStatus = iOldStatus;
            newStatus = iNewStatus;
        }
        #endregion Constructor
        #region Members
        private String name;
        private int playerIndex;
        private Byte oldStatus;
        private Byte newStatus;
        #endregion Members
        #region Properties
        public String Name
        {
            get
            {
                return name;
            }
        }
        public int PlayerIndex
        {
            get
            {
                return playerIndex;
            }
        }
        public Byte OldStatus
        {
            get
            {
                return oldStatus;
            }
        }
        public Byte NewStatus
        {
            get
            {
                return newStatus;
            }
        }
        #endregion Properties
    }
    public class PlayerHPPChangedEvArgs : FFXIEventArgs
    {
        #region Constructor
        public PlayerHPPChangedEvArgs(String iName, int iIndex, Byte iOldHPP, Byte iNewHPP)
            : base(ChangeMonitor.EVENT_TYPE.PLAYER_HPP_CHANGED)
        {
            name = iName;
            playerIndex = iIndex;
            oldHPP = iOldHPP;
            newHPP = iNewHPP;
        }
        #endregion Constructor
        #region Members
        private String name;
        private int playerIndex;
        private Byte oldHPP;
        private Byte newHPP;
        #endregion Members
        #region Properties
        public String Name
        {
            get
            {
                return name;
            }
        }
        public int PlayerIndex
        {
            get
            {
                return playerIndex;
            }
        }
        public Byte OldHPP
        {
            get
            {
                return oldHPP;
            }
        }
        public Byte NewHPP
        {
            get
            {
                return newHPP;
            }
        }
        #endregion Properties
    }
    public class ChatEvArgs : FFXIEventArgs
    {
        #region Constructor
        public ChatEvArgs(List<Iocaine2.Tools.ChatLine> iChat)
            : base(ChangeMonitor.EVENT_TYPE.CHAT)
        {
            chat = iChat;
        }
        #endregion Constructor
        #region Members
        private List<Iocaine2.Tools.ChatLine> chat;
        #endregion Members
        #region Properties
        public List<Iocaine2.Tools.ChatLine> Chat
        {
            get
            {
                return chat;
            }
        }
        #endregion Properties
    }
    #endregion Event Args Classes
}
