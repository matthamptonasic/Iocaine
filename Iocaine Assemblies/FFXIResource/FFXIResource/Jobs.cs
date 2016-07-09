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
            public Byte ID;
            public String Name;
            public String Abbr;
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
        public static JOBS_INFO GetJobInfo(Byte iId)
        {
            FfxiResource.init();
            String filterString = "ID = " + iId;
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
        public static Byte GetJobId(String iName, Boolean iAbbreviationPassed)
        {
            FfxiResource.init();
            String filterString = "";
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
        public static String GetJobName(Byte iId)
        {
            FfxiResource.init();
            String filterString = "ID = " + iId;
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
        public static String GetJobName(String iAbbreviation)
        {
            FfxiResource.init();
            String filterString = "Abbreviation = '" + iAbbreviation + "'";
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
        public static String GetJobAbbr(Byte iId)
        {
            FfxiResource.init();
            String filterString = "ID = " + iId;
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
        public static String GetJobAbbr(String iName)
        {
            FfxiResource.init();
            String filterString = "iName = '" + iName + "'";
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
