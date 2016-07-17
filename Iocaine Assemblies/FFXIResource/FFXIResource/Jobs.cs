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
        #endregion Get Functions
    }
}
