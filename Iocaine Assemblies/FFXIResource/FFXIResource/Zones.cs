using System;
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
            public String Zone;
            public UInt16 ZoneID;
            public Byte MultiAreas;
            public String AreaName;
            public Byte AreaID;
            public UInt16 AliasID;
            public Int16 XMin;
            public Int16 XMax;
            public Int16 YMin;
            public Int16 YMax;
            public String ShortName;
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
        public static ZONE_INFO GetZoneInfo(UInt16 iAliasId)
        {
            FfxiResource.init();
            String filter = "AliasID = " + iAliasId;
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
        public static ZONE_INFO GetZoneInfo(String iZoneName)
        {
            FfxiResource.init();
            String filter = "Zone = '" + iZoneName + "' AND AreaID = 0";
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
        public static List<ZONE_INFO> GetAllZoneInfo(UInt16 iZoneId)
        {
            FfxiResource.init();
            String filter = "ZoneID = " + iZoneId;
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
        public static List<ZONE_INFO> GetAllZoneInfo(String iOrderBy = "Zone, AreaID")
        {
            FfxiResource.init();
            String filter = "";
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
        public static ZONE_INFO GetZoneInfo(UInt16 iZoneID, Single iXPos, Single iYPos)
        {
            FfxiResource.init();
            String filter = "ZoneID = " + iZoneID;
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
        public static String GetZoneName(UInt16 iZoneId)
        {
            FfxiResource.init();
            String filter = "AliasID = " + iZoneId;
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
        public static String GetZoneShortName(UInt16 iZoneId)
        {
            FfxiResource.init();
            String filter = "AliasID = " + iZoneId;
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
        public static UInt16 GetZoneID(String iZoneName, Boolean iParamIsShortName = false)
        {
            FfxiResource.init();
            String filter = "";
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
