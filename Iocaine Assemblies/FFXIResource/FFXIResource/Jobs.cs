using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Jobs
    {
        #region Structures
        public struct JOBS_INFO
        {
            public byte ID;
            public string Name;
            public string Abbr;
        }
        #endregion Structures

        #region Private Members
        private static bool initDone = false;
        private const byte minId = 1;
        private const byte maxId = 22;
        private static Dictionary<byte, JOBS_INFO> infoMap;
        #endregion Private Members

        #region Public Properties
        public static byte MinID
        {
            get
            {
                return minId;
            }
        }
        public static byte MaxID
        {
            get
            {
                return maxId;
            }
        }
        public static Dictionary<byte, JOBS_INFO> InfoMap
        {
            get
            {
                return infoMap;
            }
        }
        #endregion Public Properties

        #region Init
        internal static void init()
        {
            if (!initDone)
            {
                loadData();
                loadInfoMap();
                initDone = true;
            }
        }
        #endregion Init

        #region Public Methods
        public static JOBS_INFO GetJobInfo(byte iId)
        {
            FfxiResource.init();
            string filterString = "ID = " + iId;
            MainDatabase.JobsRow[] jobsRows = (MainDatabase.JobsRow[])FfxiResource.mainDb.Jobs.Select(filterString);
            JOBS_INFO info = new JOBS_INFO();
            if (jobsRows.Length == 0)
            {
                info.ID = 0;
                info.Name = "Unknown";
                info.Abbr = "UNK";
            }
            else
            {
                info.ID = jobsRows[0].ID;
                info.Name = jobsRows[0].Name;
                info.Abbr = jobsRows[0].Abbreviation;
            }
            return info;
        }
        public static byte GetJobId(string iName, bool iAbbreviationPassed)
        {
            FfxiResource.init();
            string filterString = "";
            if(iAbbreviationPassed)
            {
                filterString += "Abbreviation = '";
            }
            else
            {
                filterString += "Name = '";
            }
            filterString += iName + "'";
            MainDatabase.JobsRow[] jobsRows = (MainDatabase.JobsRow[])FfxiResource.mainDb.Jobs.Select(filterString);
            if (jobsRows.Length == 0)
            {
                return 0xFF;
            }
            else
            {
                return jobsRows[0].ID;
            }
        }
        public static string GetJobName(byte iId)
        {
            FfxiResource.init();
            string filterString = "ID = " + iId;
            MainDatabase.JobsRow[] jobsRows = (MainDatabase.JobsRow[])FfxiResource.mainDb.Jobs.Select(filterString);
            if (jobsRows.Length == 0)
            {
                return "Unknown";
            }
            else
            {
                return jobsRows[0].Name;
            }
        }
        public static string GetJobName(string iAbbreviation)
        {
            FfxiResource.init();
            string filterString = "Abbreviation = '" + iAbbreviation + "'";
            MainDatabase.JobsRow[] jobsRows = (MainDatabase.JobsRow[])FfxiResource.mainDb.Jobs.Select(filterString);
            if (jobsRows.Length == 0)
            {
                return "Unknown";
            }
            else
            {
                return jobsRows[0].Name;
            }
        }
        public static string GetJobAbbr(byte iId)
        {
            FfxiResource.init();
            string filterString = "ID = " + iId;
            MainDatabase.JobsRow[] jobsRows = (MainDatabase.JobsRow[])FfxiResource.mainDb.Jobs.Select(filterString);
            if (jobsRows.Length == 0)
            {
                return "Unknown";
            }
            else
            {
                return jobsRows[0].Abbreviation;
            }
        }
        public static string GetJobAbbr(string iName)
        {
            FfxiResource.init();
            string filterString = "iName = '" + iName + "'";
            MainDatabase.JobsRow[] jobsRows = (MainDatabase.JobsRow[])FfxiResource.mainDb.Jobs.Select(filterString);
            if (jobsRows.Length == 0)
            {
                return "UNK";
            }
            else
            {
                return jobsRows[0].Abbreviation;
            }
        }
        #endregion Public Methods

        #region Private Methods
        private static void loadInfoMap()
        {
            infoMap = new Dictionary<byte, JOBS_INFO>();

            MainDatabase.JobsRow[] jobsRows = (MainDatabase.JobsRow[])FfxiResource.mainDb.Jobs.Select();
            foreach (MainDatabase.JobsRow row in jobsRows)
            {
                infoMap.Add(row.ID, new JOBS_INFO { ID = row.ID, Name = row.Name, Abbr = row.Abbreviation });
            }
        }
        #endregion Private Methods
    }
}
