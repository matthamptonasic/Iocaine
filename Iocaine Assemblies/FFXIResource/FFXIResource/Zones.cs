﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Zones
    {
        #region Structures
        public struct ZONE_INFO
        {
            public string Zone;
            public ushort ZoneID;
            public byte MultiAreas;
            public string AreaName;
            public byte AreaID;
            public ushort AliasID;
            public short XMin;
            public short XMax;
            public short YMin;
            public short YMax;
            public string ShortName;
        }
        #endregion Structures
        #region Member Variables
        private static bool initDone = false;
        #endregion Member Variables
        #region Init
        internal static void init()
        {
            if (!initDone)
            {
                loadData();
                initDone = true;
            }
        }
        #endregion Init
        #region Get Functions
        public static ZONE_INFO GetZoneInfo(ushort iAliasId)
        {
            FfxiResource.init();
            string filter = "AliasID = " + iAliasId;
            MainDatabase.ZonesRow[] zonesRows = (MainDatabase.ZonesRow[])FfxiResource.mainDb.Zones.Select(filter);
            ZONE_INFO info = new ZONE_INFO();
            if (zonesRows.Length == 0)
            {
                getNullZoneInfo(ref info);
            }
            else
            {
                MainDatabase.ZonesRow zonesRow = zonesRows[0];
                info.Zone = zonesRow.Zone;
                info.ZoneID = zonesRow.ZoneID;
                info.MultiAreas = zonesRow.MultiAreas;
                info.AreaName = zonesRow.AreaName;
                info.AreaID = zonesRow.AreaID;
                info.AliasID = zonesRow.AliasID;
                info.XMin = zonesRow.XMin;
                info.XMax = zonesRow.XMax;
                info.YMin = zonesRow.YMin;
                info.YMax = zonesRow.YMax;
                info.ShortName = zonesRow.ShortName;
            }
            return info;
        }
        public static ZONE_INFO GetZoneInfo(string iZoneName)
        {
            FfxiResource.init();
            string filter = "Zone = '" + iZoneName + "' AND AreaID = 0";
            MainDatabase.ZonesRow[] zonesRows = (MainDatabase.ZonesRow[])FfxiResource.mainDb.Zones.Select(filter);
            ZONE_INFO info = new ZONE_INFO();
            if (zonesRows.Length == 0)
            {
                getNullZoneInfo(ref info);
            }
            else
            {
                MainDatabase.ZonesRow zonesRow = zonesRows[0];
                info.Zone = zonesRow.Zone;
                info.ZoneID = zonesRow.ZoneID;
                info.MultiAreas = zonesRow.MultiAreas;
                info.AreaName = zonesRow.AreaName;
                info.AreaID = zonesRow.AreaID;
                info.AliasID = zonesRow.AliasID;
                info.XMin = zonesRow.XMin;
                info.XMax = zonesRow.XMax;
                info.YMin = zonesRow.YMin;
                info.YMax = zonesRow.YMax;
                info.ShortName = zonesRow.ShortName;
            }
            return info;
        }
        public static List<ZONE_INFO> GetAllZoneInfo(ushort iZoneId)
        {
            FfxiResource.init();
            string filter = "ZoneID = " + iZoneId;
            MainDatabase.ZonesRow[] zonesRows = (MainDatabase.ZonesRow[])FfxiResource.mainDb.Zones.Select(filter);
            List<ZONE_INFO> infoList = new List<ZONE_INFO>();
            if (zonesRows.Length == 0)
            {
                return infoList;
            }
            else
            {
                for (int ii = 0; ii < zonesRows.Length; ii++)
                {
                    ZONE_INFO info = new ZONE_INFO();
                    MainDatabase.ZonesRow zonesRow = zonesRows[ii];
                    info.Zone = zonesRow.Zone;
                    info.ZoneID = zonesRow.ZoneID;
                    info.MultiAreas = zonesRow.MultiAreas;
                    info.AreaName = zonesRow.AreaName;
                    info.AreaID = zonesRow.AreaID;
                    info.AliasID = zonesRow.AliasID;
                    info.XMin = zonesRow.XMin;
                    info.XMax = zonesRow.XMax;
                    info.YMin = zonesRow.YMin;
                    info.YMax = zonesRow.YMax;
                    info.ShortName = zonesRow.ShortName;
                    infoList.Add(info);
                }
            }
            return infoList;
        }
        public static List<ZONE_INFO> GetAllZoneInfo(string iOrderBy = "Zone, AreaID")
        {
            FfxiResource.init();
            string filter = "";
            MainDatabase.ZonesRow[] zonesRows = (MainDatabase.ZonesRow[])FfxiResource.mainDb.Zones.Select(filter, iOrderBy);
            List<ZONE_INFO> infoList = new List<ZONE_INFO>();
            if (zonesRows.Length == 0)
            {
                return infoList;
            }
            else
            {
                for (int ii = 0; ii < zonesRows.Length; ii++)
                {
                    ZONE_INFO info = new ZONE_INFO();
                    MainDatabase.ZonesRow zonesRow = zonesRows[ii];
                    info.Zone = zonesRow.Zone;
                    info.ZoneID = zonesRow.ZoneID;
                    info.MultiAreas = zonesRow.MultiAreas;
                    info.AreaName = zonesRow.AreaName;
                    info.AreaID = zonesRow.AreaID;
                    info.AliasID = zonesRow.AliasID;
                    info.XMin = zonesRow.XMin;
                    info.XMax = zonesRow.XMax;
                    info.YMin = zonesRow.YMin;
                    info.YMax = zonesRow.YMax;
                    info.ShortName = zonesRow.ShortName;
                    infoList.Add(info);
                }
            }
            return infoList;
        }
        public static ZONE_INFO GetZoneInfo(ushort iZoneID, float iXPos, float iYPos)
        {
            FfxiResource.init();
            string filter = "ZoneID = " + iZoneID;
            filter += " AND XMin <= " + ((short)iXPos).ToString();
            filter += " AND XMax >= " + ((short)iXPos).ToString();
            filter += " AND YMin <= " + ((short)iYPos).ToString();
            filter += " AND YMax >= " + ((short)iYPos).ToString();
            MainDatabase.ZonesRow[] zonesRows = (MainDatabase.ZonesRow[])FfxiResource.mainDb.Zones.Select(filter);
            ZONE_INFO info = new ZONE_INFO();
            if (zonesRows.Length == 0)
            {
                //Alias ID will be same as Zone ID when MultiAreas = 0.
                return GetZoneInfo(iZoneID);
            }
            else
            {
                MainDatabase.ZonesRow zonesRow = zonesRows[0];
                info.Zone = zonesRow.Zone;
                info.ZoneID = zonesRow.ZoneID;
                info.MultiAreas = zonesRow.MultiAreas;
                info.AreaName = zonesRow.AreaName;
                info.AreaID = zonesRow.AreaID;
                info.AliasID = zonesRow.AliasID;
                info.XMin = zonesRow.XMin;
                info.XMax = zonesRow.XMax;
                info.YMin = zonesRow.YMin;
                info.YMax = zonesRow.YMax;
                info.ShortName = zonesRow.ShortName;
            }
            return info;
        }
        public static string GetZoneName(ushort iZoneId)
        {
            FfxiResource.init();
            string filter = "AliasID = " + iZoneId;
            MainDatabase.ZonesRow[] zonesRows = (MainDatabase.ZonesRow[])FfxiResource.mainDb.Zones.Select(filter);
            if (zonesRows.Length == 0)
            {
                return "Unknown";
            }
            else
            {
                MainDatabase.ZonesRow zonesRow = zonesRows[0];
                return zonesRow.Zone;
            }
        }
        public static string GetZoneShortName(ushort iZoneId)
        {
            FfxiResource.init();
            string filter = "AliasID = " + iZoneId;
            MainDatabase.ZonesRow[] zonesRows = (MainDatabase.ZonesRow[])FfxiResource.mainDb.Zones.Select(filter);
            if (zonesRows.Length == 0)
            {
                return "Unknown";
            }
            else
            {
                MainDatabase.ZonesRow zonesRow = zonesRows[0];
                return zonesRow.ShortName;
            }
        }
        public static ushort GetZoneID(string iZoneName, bool iParamIsShortName = false)
        {
            FfxiResource.init();
            string filter = "";
            if(iParamIsShortName)
            {
                filter = "ShortName='";
            }
            else
            {
                filter = "Zone='";
            }
            filter += iZoneName + "' AND AreaID=0";
            MainDatabase.ZonesRow[] zonesRows = (MainDatabase.ZonesRow[])FfxiResource.mainDb.Zones.Select(filter);
            if (zonesRows.Length == 0)
            {
                return 0;
            }
            else
            {
                MainDatabase.ZonesRow zonesRow = zonesRows[0];
                return zonesRow.ZoneID;
            }
        }

        #region Nulls
        private static ZONE_INFO getNullZoneInfo()
        {
            ZONE_INFO info = new ZONE_INFO();
            info.Zone = "Unknown";
            info.ZoneID = 0;
            info.MultiAreas = 0;
            info.AreaName = "Unknown";
            info.AreaID = 0;
            info.AliasID = 0;
            info.XMin = 0;
            info.XMax = 0;
            info.YMin = 0;
            info.YMax = 0;
            info.ShortName = "Unknown";
            return info;
        }
        private static void getNullZoneInfo(ref ZONE_INFO oInfo)
        {
            oInfo.Zone = "Unknown";
            oInfo.ZoneID = 0;
            oInfo.MultiAreas = 0;
            oInfo.AreaName = "Unknown";
            oInfo.AreaID = 0;
            oInfo.AliasID = 0;
            oInfo.XMin = 0;
            oInfo.XMax = 0;
            oInfo.YMin = 0;
            oInfo.YMax = 0;
            oInfo.ShortName = "Unknown";
        }
        #endregion Nulls
        #region Table
        public static MainDatabase.ZonesDataTable GetZoneTableCopy()
        {
            FfxiResource.init();
            return (MainDatabase.ZonesDataTable)FfxiResource.mainDb.Zones.Copy();
        }
        #endregion Table
        #endregion Get Functions
    }
}
