using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Rods
    {
        #region Enums
        public enum INFO_ORDER : byte
        {
            NAME,
            ID
        }
        #endregion Enums
        #region Structures
        public struct ROD_INFO
        {
            public String RodName;
            public UInt16 ItemID;
        }
        #endregion Structures
        #region Member Variables
        private static bool initDone = false;
        private static string invalidName = "Unknown";
        private static ushort invalidID = 0;
        public static string InvalidName
        {
            get
            {
                return invalidName;
            }
        }
        public static ushort InvalidID
        {
            get
            {
                return invalidID;
            }
        }
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
        public static ROD_INFO GetRodInfo(UInt16 iItemId)
        {
            FfxiResource.init();
            String filter = "ItemID = " + iItemId;
            MainDatabase.RodsRow[] rodRows = (MainDatabase.RodsRow[])FfxiResource.mainDb.Rods.Select(filter);
            ROD_INFO info = new ROD_INFO();
            if (rodRows.Length == 0)
            {
                getNullRodInfo(ref info);
            }
            else
            {
                MainDatabase.RodsRow rodRow = rodRows[0];
                info.RodName = rodRow.RodName;
                info.ItemID = rodRow.ItemID;
            }
            return info;
        }
        public static ROD_INFO GetRodInfo(String iRodName)
        {
            FfxiResource.init();
            String filter = "RodName = '" + iRodName + "'";
            MainDatabase.RodsRow[] rodRows = (MainDatabase.RodsRow[])FfxiResource.mainDb.Rods.Select(filter);
            ROD_INFO info = new ROD_INFO();
            if (rodRows.Length == 0)
            {
                getNullRodInfo(ref info);
            }
            else
            {
                MainDatabase.RodsRow rodRow = rodRows[0];
                info.RodName = rodRow.RodName;
                info.ItemID = rodRow.ItemID;
            }
            return info;
        }
        public static String GetRodName(UInt16 iRodId)
        {
            ROD_INFO info = GetRodInfo(iRodId);
            return info.RodName;
        }
        public static List<ROD_INFO> GetAllRodInfo(INFO_ORDER iOrderBy = INFO_ORDER.NAME)
        {
            List<ROD_INFO> infoList = new List<ROD_INFO>();
            FfxiResource.init();
            String filter = "";
            String orderBy = "";
            if (iOrderBy == INFO_ORDER.NAME)
            {
                orderBy = "RodName";
            }
            else
            {
                orderBy = "ItemID";
            }
            MainDatabase.RodsRow[] rodRows = (MainDatabase.RodsRow[])FfxiResource.mainDb.Rods.Select(filter, orderBy);
            if (rodRows.Length == 0)
            {
                infoList.Add(getNullRodInfo());
            }
            else
            {
                for (int ii = 0; ii < rodRows.Length; ii++)
                {
                    MainDatabase.RodsRow rodRow = rodRows[ii];
                    ROD_INFO info = new ROD_INFO();
                    info.RodName = rodRow.RodName;
                    info.ItemID = rodRow.ItemID;
                    infoList.Add(info);
                }
            }
            return infoList;
        }
        #region Nulls
        private static ROD_INFO getNullRodInfo()
        {
            ROD_INFO info = new ROD_INFO();
            info.RodName = invalidName;
            info.ItemID = invalidID;
            return info;
        }
        private static void getNullRodInfo(ref ROD_INFO oInfo)
        {
            oInfo.RodName = invalidName;
            oInfo.ItemID = invalidID;
        }
        #endregion Nulls
        #region Data Table Copies
        public static MainDatabase.RodsDataTable GetRodsTableCopy()
        {
            FfxiResource.init();
            return (MainDatabase.RodsDataTable)FfxiResource.mainDb.Rods.Copy();
        }
        #endregion Data Table Copies
        #endregion Get Functions
    }
}
