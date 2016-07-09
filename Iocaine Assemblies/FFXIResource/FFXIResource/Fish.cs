using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Fish
    {
        #region Enums
        public enum FISH_TYPE : byte
        {
            FISH = 0,
            ITEM = 1,
            MONSTER = 2
        }
        public enum INFO_ORDER : byte
        {
            NAME,
            ID
        }
        #endregion Enums
        #region Structures
        public struct FISH_INFO
        {
            public String FishName;
            public Boolean Large;
            public Byte Type;
            public Byte DropType;
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
        public static FISH_INFO GetFishInfo(UInt16 iItemId)
        {
            FfxiResource.init();
            String filter = "ItemID = " + iItemId;
            MainDatabase.FishRow[] fishRows = (MainDatabase.FishRow[])FfxiResource.mainDb.Fish.Select(filter);
            FISH_INFO info = new FISH_INFO();
            if (fishRows.Length == 0)
            {
                getNullFishInfo(ref info);
            }
            else
            {
                MainDatabase.FishRow fishRow = fishRows[0];
                info.FishName = fishRow.FishName;
                info.Large = fishRow.Large;
                info.Type = fishRow.Type;
                info.DropType = fishRow.DropType;
                info.ItemID = fishRow.ItemID;
            }
            return info;
        }
        public static FISH_INFO GetFishInfo(String iFishName)
        {
            FfxiResource.init();
            String filter = "FishName = '" + iFishName + "'";
            MainDatabase.FishRow[] fishRows = (MainDatabase.FishRow[])FfxiResource.mainDb.Fish.Select(filter);
            FISH_INFO info = new FISH_INFO();
            if (fishRows.Length == 0)
            {
                getNullFishInfo(ref info);
            }
            else
            {
                MainDatabase.FishRow fishRow = fishRows[0];
                info.FishName = fishRow.FishName;
                info.Large = fishRow.Large;
                info.Type = fishRow.Type;
                info.DropType = fishRow.DropType;
                info.ItemID = fishRow.ItemID;
            }
            return info;
        }
        public static String GetFishName(UInt16 iFishId)
        {
            FISH_INFO info = GetFishInfo(iFishId);
            return info.FishName;
        }
        public static List<FISH_INFO> GetAllFishInfo(Boolean iOnlyFish = true, INFO_ORDER iOrderBy = INFO_ORDER.NAME)
        {
            List<FISH_INFO> infoList = new List<FISH_INFO>();
            FfxiResource.init();
            String filter = "";
            if (iOnlyFish)
            {
                filter = "Type = 0";
            }
            string orderBy = "";
            if (iOrderBy == INFO_ORDER.NAME)
            {
                orderBy = "FishName";
            }
            else
            {
                orderBy = "ItemID";
            }

            MainDatabase.FishRow[] fishRows = (MainDatabase.FishRow[])FfxiResource.mainDb.Fish.Select(filter, orderBy);
            if (fishRows.Length == 0)
            {
                infoList.Add(getNullFishInfo());
            }
            else
            {
                for (int ii = 0; ii < fishRows.Length; ii++)
                {
                    MainDatabase.FishRow fishRow = fishRows[ii];
                    FISH_INFO info = new FISH_INFO();
                    info.FishName = fishRow.FishName;
                    info.Large = fishRow.Large;
                    info.Type = fishRow.Type;
                    info.DropType = fishRow.DropType;
                    info.ItemID = fishRow.ItemID;
                    infoList.Add(info);
                }
            }
            return infoList;
        }
        #region Nulls
        private static FISH_INFO getNullFishInfo()
        {
            FISH_INFO info = new FISH_INFO();
            info.FishName = invalidName;
            info.Large = false;
            info.Type = 0;
            info.DropType = 0;
            info.ItemID = invalidID;
            return info;
        }
        private static void getNullFishInfo(ref FISH_INFO oInfo)
        {
            oInfo.FishName = invalidName;
            oInfo.Large = false;
            oInfo.Type = 0;
            oInfo.DropType = 0;
            oInfo.ItemID = invalidID;
        }
        #endregion Nulls
        #region Table
        public static MainDatabase.FishDataTable GetFishTableCopy()
        {
            FfxiResource.init();
            return (MainDatabase.FishDataTable)FfxiResource.mainDb.Fish.Copy();
        }
        #endregion Table
        #endregion Get Functions
    }
}
