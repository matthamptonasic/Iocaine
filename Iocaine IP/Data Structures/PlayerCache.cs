using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Memory;

namespace Iocaine2
{
    /// <summary>
    /// There is some data that we keep a cache of.
    /// To keep a cache here, the data needs to meet the following:
    /// 1. Must be quasi-static, not changing more than once in appx 5 seconds.
    ///    It may change more frequently in some situations (like armor),
    ///    but can generally be considered static.
    ///    This excludes things like HP, MP, TP, position, etc.
    /// 2. Must not be data that would be need to be read on demand (could not be polled).
    /// 3. It must be likely that the data will be used in more than one place.
    ///    If the data were only used in 1 place (very specific purpose), it probably
    ///    would not make sense to poll it in the change monitor.
    /// </summary>
    public static partial class PlayerCache
    {
        public static class Vitals
        {
            #region Private Members
            private static String name = "";
            private static Byte mainJob = 0;
            private static Byte mainJobLvl = 1;
            private static Byte subJob = 0;
            private static Byte subJobLvl = 0;
            #endregion Private Members
            #region Public Properties
            public static String Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }
            public static Byte MainJob
            {
                get
                {
                    return mainJob;
                }
                set
                {
                    mainJob = value;
                }
            }
            public static Byte MainJobLvl
            {
                get
                {
                    return mainJobLvl;
                }
                set
                {
                    mainJobLvl = value;
                }
            }
            public static Byte SubJob
            {
                get
                {
                    return subJob;
                }
                set
                {
                    subJob = value;
                }
            }
            public static Byte SubJobLvl
            {
                get
                {
                    return subJobLvl;
                }
                set
                {
                    subJobLvl = value;
                }
            }
            #endregion Public Properties
        }
        public static class Equipment
        {
            #region Private Members
            private static UInt16 main = 0;
            private static UInt16 sub = 0;
            private static UInt16 range = 0;
            private static UInt16 ammo = 0;
            private static byte ammoQuan = 0;
            #endregion Private Members
            #region Public Properties
            public static UInt16 Main
            {
                get
                {
                    return main;
                }
                set
                {
                    main = value;
                }
            }
            public static UInt16 Sub
            {
                get
                {
                    return sub;
                }
                set
                {
                    sub = value;
                }
            }
            public static UInt16 Range
            {
                get
                {
                    return range;
                }
                set
                {
                    range = value;
                }
            }
            public static UInt16 Ammo
            {
                get
                {
                    return ammo;
                }
                set
                {
                    ammo = value;
                }
            }
            public static byte AmmoQuan
            {
                get
                {
                    return ammoQuan;
                }
                set
                {
                    ammoQuan = value;
                }
            }
            #endregion Public Properties
        }
        public static class Skills
        {
            #region Private Members
            private static Byte combatSkillType = 0;
            private static Int32 combatSkillCurrentLvl = 0;
            #endregion Private Members
            #region Public Properties
            public static Byte CombatSkillType
            {
                get
                {
                    return combatSkillType;
                }
                set
                {
                    combatSkillType = value;
                }
            }
            public static Int32 CombatSkillCurrentLvl
            {
                get
                {
                    return combatSkillCurrentLvl;
                }
                set
                {
                    combatSkillCurrentLvl = value;
                }
            }
            #endregion Public Properties
        }
        public static class Environment
        {
            #region Private Members
            private static byte serverId = 0;
            private static string serverName = "";
            private static ushort zoneId = 0;
            private static ushort areaId = 0;
            private static ushort zoneAlias = 0;
            private static string zoneName = "";
            private static string areaName = "";
            private static bool inMogHouse = false;
            private static byte weatherId = 0;
            private static string weatherName = "";
            #endregion Private Members
            #region Public Properties
            public static byte ServerId
            {
                get
                {
                    return serverId;
                }
                set
                {
                    serverId = value;
                }
            }
            public static string ServerName
            {
                get
                {
                    return serverName;
                }
                set
                {
                    serverName = value;
                }
            }
            public static ushort ZoneId
            {
                get
                {
                    return zoneId;
                }
                set
                {
                    zoneId = value;
                }
            }
            public static ushort AreaId
            {
                get
                {
                    return areaId;
                }
                set
                {
                    areaId = value;
                }
            }
            public static ushort ZoneAlias
            {
                get
                {
                    return zoneAlias;
                }
                set
                {
                    zoneAlias = value;
                }
            }
            public static string ZoneName
            {
                get
                {
                    return zoneName;
                }
                set
                {
                    zoneName = value;
                }
            }
            public static string AreaName
            {
                get
                {
                    return areaName;
                }
                set
                {
                    areaName = value;
                }
            }
            public static bool InMogHouse
            {
                get
                {
                    return inMogHouse;
                }
                set
                {
                    inMogHouse = value;
                }
            }
            public static byte WeatherId
            {
                get
                {
                    return weatherId;
                }
                set
                {
                    weatherId = value;
                }
            }
            public static string WeatherName
            {
                get
                {
                    return weatherName;
                }
                set
                {
                    weatherName = value;
                }
            }
            #endregion Public Properties
        }
    }
}
