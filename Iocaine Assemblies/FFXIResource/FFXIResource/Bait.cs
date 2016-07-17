using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Bait
    {
        #region Enums
        public enum INFO_ORDER : byte
        {
            NAME,
            ID
        }
        #endregion Enums
        #region Structures
        public struct BAIT_INFO
        {
            public string BaitName;
            public string BaitNameShort;
            public ushort ItemID;
        }
        #endregion Structures
        #region Member Variables
        private static bool initDone = false;
        private const string invalidName = "Unknown";
        private const ushort invalidID = 0;
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
        public static BAIT_INFO GetBaitInfo(ushort iItemId)
        {
            FfxiResource.init();
            string filter = "ItemID = " + iItemId;
            MainDatabase.BaitRow[] baitRows = (MainDatabase.BaitRow[])FfxiResource.mainDb.Bait.Select(filter);
            BAIT_INFO info = new BAIT_INFO();
            if (baitRows.Length == 0)
            {
                getNullBaitInfo(ref info);
            }
            else
            {
                MainDatabase.BaitRow baitRow = baitRows[0];
                info.BaitName = baitRow.BaitName;
                info.BaitNameShort = baitRow.BaitNameShort;
                info.ItemID = baitRow.ItemID;
            }
            return info;
        }
        public static BAIT_INFO GetBaitInfo(string iBaitName)
        {
            FfxiResource.init();
            string filter = "BaitName = '" + iBaitName + "'";
            MainDatabase.BaitRow[] baitRows = (MainDatabase.BaitRow[])FfxiResource.mainDb.Bait.Select(filter);
            BAIT_INFO info = new BAIT_INFO();
            if (baitRows.Length == 0)
            {
                getNullBaitInfo(ref info);
            }
            else
            {
                MainDatabase.BaitRow baitRow = baitRows[0];
                info.BaitName = baitRow.BaitName;
                info.BaitNameShort = baitRow.BaitNameShort;
                info.ItemID = baitRow.ItemID;
            }
            return info;
        }
        public static string GetBaitName(ushort iBaitId)
        {
            BAIT_INFO info = GetBaitInfo(iBaitId);
            return info.BaitName;
        }
        public static string GetBaitNameShort(ushort iBaitId)
        {
            BAIT_INFO info = GetBaitInfo(iBaitId);
            return info.BaitNameShort;
        }
        public static List<BAIT_INFO> GetAllBaitInfo(INFO_ORDER iOrderBy = INFO_ORDER.NAME)
        {
            List<BAIT_INFO> infoList = new List<BAIT_INFO>();
            FfxiResource.init();
            string filter = "";
            string orderBy = "";
            if (iOrderBy == INFO_ORDER.NAME)
            {
                orderBy = "BaitName";
            }
            else
            {
                orderBy = "ItemID";
            }
            MainDatabase.BaitRow[] baitRows = (MainDatabase.BaitRow[])FfxiResource.mainDb.Bait.Select(filter, orderBy);
            if (baitRows.Length == 0)
            {
                infoList.Add(getNullBaitInfo());
            }
            else
            {
                for (int ii = 0; ii < baitRows.Length; ii++)
                {
                    MainDatabase.BaitRow baitRow = baitRows[ii];
                    BAIT_INFO info = new BAIT_INFO();
                    info.BaitName = baitRow.BaitName;
                    info.BaitNameShort = baitRow.BaitNameShort;
                    info.ItemID = baitRow.ItemID;
                    infoList.Add(info);
                }
            }
            return infoList;
        }
        #region Nulls
        private static BAIT_INFO getNullBaitInfo()
        {
            BAIT_INFO info = new BAIT_INFO();
            info.BaitName = invalidName;
            info.BaitNameShort = invalidName;
            info.ItemID = invalidID;
            return info;
        }
        private static void getNullBaitInfo(ref BAIT_INFO oInfo)
        {
            oInfo.BaitName = invalidName;
            oInfo.BaitNameShort = invalidName;
            oInfo.ItemID = invalidID;
        }
        #endregion Nulls
        #region Table
        public static MainDatabase.BaitDataTable GetBaitTableCopy()
        {
            FfxiResource.init();
            return (MainDatabase.BaitDataTable)FfxiResource.mainDb.Bait.Copy();
        }
        #endregion Table
        #endregion Get Functions
    }
}
