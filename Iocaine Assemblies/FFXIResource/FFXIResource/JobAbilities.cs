using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class JobAbilities
    {
        #region Structures
        public struct JA_INFO
        {
            public String Name;
            public UInt16 ID;
            public String Command;
            public Int16 Targets;
            public Byte Job;
            public Byte JobLevel;
            public bool AsSub;
            public UInt16 MP;
            public UInt16 TP;
            public UInt32 Duration;
            public Int16 Element;
            public UInt16 RecastID;
            public String Type;
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
        /// <summary>
        /// Gets all related job ability information based on the given job, sub job, and level.
        /// </summary>
        /// <param name="job">Main job.</param>
        /// <param name="subJob">Sub job (pass 0 if none).</param>
        /// <param name="level">Main job level.</param>
        /// <returns>List of JA_INFO structures that contain the relevant information.</returns>
        public static List<JA_INFO> GetAbilityInfo(Byte job, Byte subJob, Byte level)
        {
            FfxiResource.init();
            List<JA_INFO> jaInfoList = new List<JA_INFO>();
            try
            {
                String filterString = "((Job = " + job.ToString() + " AND JobLevel <= " + level.ToString() + ")";
                filterString += " OR (Job = " + subJob.ToString() + " AND JobLevel <= " + (level / 2).ToString();
                filterString += " AND AsSub = 'true'))";
                filterString += " AND (RecastId <> 0 OR (RecastId = 0 AND JobLevel = 1))";
                String sortString = "Name";
                MainDatabase.JARow[] jaRows = (MainDatabase.JARow[])FfxiResource.mainDb.JA.Select(filterString, sortString);
                foreach (MainDatabase.JARow row in jaRows)
                {
                    JA_INFO info = wrapInfo(row);
                    jaInfoList.Add(info);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("List<JA_INFO> JobAbilities::GetAbilityInfo: Failed: " + e.ToString());
            }
            return jaInfoList;
        }
        /// <summary>
        /// Gets all related job ability information based on the given job and sub job.
        /// </summary>
        /// <param name="job">Main job.</param>
        /// <param name="subJob">Sub job (pass 0 if none).</param>
        /// <returns>List of JA_INFO structures that contain the relevant information.</returns>
        public static List<JA_INFO> GetAbilityInfo(Byte job, Byte subJob)
        {
            return GetAbilityInfo(job, subJob, 100);
        }
        /// <summary>
        /// Gets all job abilities that do not have the job set to 0 and level less than 100.
        /// </summary>
        /// <returns>A list of all JA_INFO structures.</returns>
        public static List<JA_INFO> GetAbilityInfo()
        {
            FfxiResource.init();
            List<JA_INFO> jaInfoList = new List<JA_INFO>();
            try
            {
                String filterString = "Job <> 0 AND JobLevel < 100";
                String sortString = "Name";
                MainDatabase.JARow[] jaRows = (MainDatabase.JARow[])FfxiResource.mainDb.JA.Select(filterString, sortString);
                foreach (MainDatabase.JARow row in jaRows)
                {
                    JA_INFO info = wrapInfo(row);
                    jaInfoList.Add(info);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("List<JA_INFO> JobAbilities::GetAbilityInfo(): Failed: " + e.ToString());
            }
            return jaInfoList;
        }
        public static List<JA_INFO> GetAbilityInfo_Cures(Byte job, Byte subJob, Byte level)
        {
            FfxiResource.init();
            List<JA_INFO> jaInfoList = new List<JA_INFO>();
            try
            {
                String filterString = "(((Job = " + job.ToString() + " AND JobLevel <= " + level.ToString() + ")";
                filterString += " OR (Job = " + subJob.ToString() + " AND JobLevel <= " + (level / 2).ToString();
                filterString += " AND AsSub = 'true'))";
                filterString += " AND (RecastId <> 0 OR (RecastId = 0 AND JobLevel = 1))";
                filterString += " AND (Targets = " + ((short)Spells.TARGETS.ANY_PC_NPC).ToString() + ")";
                filterString += ")";
                String sortString = "Name";
                MainDatabase.JARow[] jaRows = (MainDatabase.JARow[])FfxiResource.mainDb.JA.Select(filterString, sortString);
                foreach (MainDatabase.JARow row in jaRows)
                {
                    JA_INFO info = wrapInfo(row);
                    jaInfoList.Add(info);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("List<JA_INFO> JobAbilities::GetAbilityInfo_Cures: Failed: " + e.ToString());
            }
            return jaInfoList;
        }
        public static List<JA_INFO> GetAbilityInfo_PartyAbility(Byte job, Byte subJob, Byte level)
        {
            FfxiResource.init();
            List<JA_INFO> jaInfoList = new List<JA_INFO>();
            try
            {
                String filterString = "((Job = " + job.ToString() + " AND JobLevel <= " + level.ToString() + ")";
                filterString += " OR (Job = " + subJob.ToString() + " AND JobLevel <= " + (level / 2).ToString();
                filterString += " AND AsSub = 'true'))";
                filterString += " AND (RecastId <> 0 OR (RecastId = 0 AND JobLevel = 1))";
                filterString += " AND (";
                filterString += "    (Name = 'Cover')";
                filterString += " OR (Name = 'Accomplice')";
                filterString += " OR (Name = 'Collaborator')";
                filterString += " OR (Name = 'Manawell')";
                filterString += " OR (Name = 'Spontaneity')";
                filterString += " OR (Name = 'Cutting Cards')";
                filterString += " OR (Name = 'Caper Emissarius')";
                filterString += "    )";
                String sortString = "Name";
                MainDatabase.JARow[] jaRows = (MainDatabase.JARow[])FfxiResource.mainDb.JA.Select(filterString, sortString);
                foreach (MainDatabase.JARow row in jaRows)
                {
                    JA_INFO info = wrapInfo(row);
                    jaInfoList.Add(info);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("List<JA_INFO> JobAbilities::GetAbilityInfo_PartyAbility: Failed: " + e.ToString());
            }
            return jaInfoList;
        }
        public static List<JA_INFO> GetAbilityInfo_SelfBuffs(Byte job, Byte subJob, Byte level)
        {
            FfxiResource.init();
            List<JA_INFO> jaInfoList = new List<JA_INFO>();
            try
            {
                String filterString = "((Job = " + job.ToString() + " AND JobLevel <= " + level.ToString() + ")";
                filterString += " OR (Job = " + subJob.ToString() + " AND JobLevel <= " + (level / 2).ToString();
                filterString += " AND AsSub = 'true'))";
                filterString += " AND (RecastId <> 0 OR (RecastId = 0 AND JobLevel = 1))";
                filterString += " AND ((Type = 'JobAbility') OR (Type = 'CorsairRoll') OR (Type LIKE 'Flourish%')";
                filterString += "    OR (Type = 'Jig') OR (Type = 'Waltz') OR (Type = 'Samba'))";
                filterString += " AND (Targets = " + ((short)Spells.TARGETS.ME_ONLY).ToString() + ")";
                filterString += " AND (Name <> 'Pet commands')";
                filterString += " AND (Name <> 'Convert')";
                filterString += " AND (Name NOT LIKE 'Blood Pact%')";
                filterString += " AND (Name <> 'Sambas')";
                filterString += " AND (Name <> 'Waltzes')";
                filterString += " AND (Name <> 'Jigs')";
                filterString += " AND (Name <> 'Steps')";
                filterString += " AND (Name <> 'Elemental Siphon')";
                filterString += " AND (Name NOT LIKE 'Flourishes%')";
                filterString += " AND (Name <> 'Steps')";
                String sortString = "Name";
                MainDatabase.JARow[] jaRows = (MainDatabase.JARow[])FfxiResource.mainDb.JA.Select(filterString, sortString);
                foreach (MainDatabase.JARow row in jaRows)
                {
                    JA_INFO info = wrapInfo(row);
                    jaInfoList.Add(info);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("List<JA_INFO> JobAbilities::GetAbilityInfo_PartyAbility: Failed: " + e.ToString());
            }
            return jaInfoList;
        }
        public static List<JA_INFO> GetAbilityInfo_HealingAbility(Byte job, Byte subJob, Byte level)
        {
            FfxiResource.init();
            List<JA_INFO> jaInfoList = new List<JA_INFO>();
            try
            {
                String filterString = "((Job = " + job.ToString() + " AND JobLevel <= " + level.ToString() + ")";
                filterString += " OR (Job = " + subJob.ToString() + " AND JobLevel <= " + (level / 2).ToString();
                filterString += " AND AsSub = 'true'))";
                filterString += " AND (RecastId <> 0 OR (RecastId = 0 AND JobLevel = 1))";
                filterString += " AND (";
                filterString += "     (Name = 'Healing Waltz')";
                filterString += " OR  (Name = 'Unused Option')";
                filterString += "     )";
                String sortString = "Name";
                MainDatabase.JARow[] jaRows = (MainDatabase.JARow[])FfxiResource.mainDb.JA.Select(filterString, sortString);
                foreach (MainDatabase.JARow row in jaRows)
                {
                    JA_INFO info = wrapInfo(row);
                    jaInfoList.Add(info);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("List<JA_INFO> JobAbilities::GetAbilityInfo_HealingAbility: Failed: " + e.ToString());
            }
            return jaInfoList;
        }
        /// <summary>
        /// Gets the job ability info structure of the given ability name.
        /// </summary>
        /// <param name="iName">Name of the ability.</param>
        /// <returns>The filled JA_INFO structure for the given job ability.</returns>
        public static JA_INFO GetAbilityInfo(String iName)
        {
            FfxiResource.init();
            JA_INFO info = new JA_INFO();
            try
            {
                String filterString = "Name = '" + iName + "'";
                MainDatabase.JARow[] jaRows = (MainDatabase.JARow[])FfxiResource.mainDb.JA.Select(filterString);
                if (jaRows.Length == 0)
                {
                    info.Name = "";
                    info.ID = 0xFFFF;
                    info.Command = "";
                    info.Targets = 0;
                    info.Job = 0;
                    info.JobLevel = 0;
                    info.AsSub = false;
                    info.MP = 0;
                    info.TP = 0;
                    info.Duration = 0;
                    info.Element = 0;
                    info.RecastID = 0;
                    info.Type = "";
                }
                else
                {
                    info.Name = FfxiResource.DecodeApostrophy(jaRows[0].Name);
                    info.ID = jaRows[0].ID;
                    info.Command = FfxiResource.DecodeApostrophy(jaRows[0].Command);
                    info.Targets = jaRows[0].Targets;
                    info.Job = jaRows[0].Job;
                    info.JobLevel = jaRows[0].JobLevel;
                    info.AsSub = jaRows[0].AsSub;
                    info.MP = jaRows[0].MP;
                    info.TP = jaRows[0].TP;
                    info.Duration = jaRows[0].Duration;
                    info.Element = jaRows[0].Element;
                    info.RecastID = jaRows[0].RecastID;
                    info.Type = jaRows[0].Type;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("JA_INFO JobAbilities::GetAbilityInfo: Failed: " + e.ToString());
            }
            return info;
        }
        /// <summary>
        /// Gets a Dictionary (map) of each ability recast timer (key) and ability names (value).
        /// </summary>
        /// <param name="job">Main job.</param>
        /// <param name="subJob">Sub job (pass 0 for none).</param>
        /// <param name="level">Main job level.</param>
        /// <returns>Dictionary (map) of each ability recast timer (key) and ability names (value).</returns>
        public static Dictionary<UInt16, List<String>> GetAbilityMap(Byte job, Byte subJob, Byte level)
        {
            FfxiResource.init();
            Dictionary<UInt16, List<String>> jaMap = new Dictionary<UInt16, List<String>>();
            String filterString = "((Job = " + job.ToString() + " AND JobLevel <= " + level.ToString() + ")";
            filterString += " OR (Job = " + subJob.ToString() + " AND JobLevel <= " + (level / 2).ToString();
            filterString += " AND AsSub = 'true'))";
            filterString += " AND (RecastId <> 0 OR (RecastId = 0 AND JobLevel = 1))";
            String sortString = "RecastId";
            MainDatabase.JARow[] jaRows = (MainDatabase.JARow[])FfxiResource.mainDb.JA.Select(filterString, sortString);
            foreach (MainDatabase.JARow row in jaRows)
            {
                String name = FfxiResource.DecodeApostrophy(row.Name);
                if (!jaMap.ContainsKey(row.RecastID))
                {
                    List<String> localList = new List<String>();
                    localList.Add(name);
                    jaMap.Add(row.RecastID, localList);
                }
                else
                {
                    List<String> localEntry = jaMap[row.RecastID];
                    localEntry.Add(name);
                    jaMap[row.RecastID] = localEntry;
                }
            }
            return jaMap;
        }
        /// <summary>
        /// Gets a Dictionary (map) of each ability index (key) and ability name (value).
        /// </summary>
        /// <param name="job">Main job.</param>
        /// <param name="subJob">Sub job (pass 0 for none).</param>
        /// <returns>Dictionary (map) of each ability index (key) and ability name (value).</returns>
        public static Dictionary<UInt16, List<String>> GetAbilityMap(Byte job, Byte subJob)
        {
            return GetAbilityMap(job, subJob, 100);
        }
        /// <summary>
        /// Gets a list of indices of job abilities for the job/subjob combination given.
        /// </summary>
        /// <param name="job">Main job as a byte.</param>
        /// <param name="subJob">Sub job as a byte. If no sub job, pass 0.</param>
        /// <param name="level">Main job level.</param>
        /// <returns></returns>
        public static List<UInt16> GetRecastTimerIDs(Byte job, Byte subJob, Byte level)
        {
            FfxiResource.init();
            List<UInt16> jaList = new List<UInt16>();
            String filterString = "((Job = " + job.ToString() + " AND JobLevel <= " + level.ToString() + ")";
            filterString += " OR (Job = " + subJob.ToString() + " AND JobLevel <= " + (level / 2).ToString();
            filterString += " AND AsSub = 'true'))";
            filterString += " AND (RecastID <> 0 OR (RecastID = 0 AND JobLevel = 1))";
            String sortString = "RecastID";
            MainDatabase.JARow[] jaRows = (MainDatabase.JARow[])FfxiResource.mainDb.JA.Select(filterString, sortString);
            foreach (MainDatabase.JARow row in jaRows)
            {
                if (!jaList.Contains(row.RecastID))
                {
                    jaList.Add(row.RecastID);
                }
            }
            return jaList;
        }
        /// <summary>
        /// Gets a list of indices of all job abilities for the job/subjob combination given.
        /// </summary>
        /// <param name="job">Main job as a byte.</param>
        /// <param name="subJob">Sub job as a byte. If no sub job, pass 0.</param>
        /// <returns></returns>
        public static List<UInt16> GetRecastTimerIDs(Byte job, Byte subJob)
        {
            return GetRecastTimerIDs(job, subJob, 100);
        }
        /// <summary>
        /// Gets the index of a specific ability given the ability name.
        /// </summary>
        /// <param name="abilityName">Ability Name to find the index for.</param>
        /// <returns>Index of the job ability with the given name.</returns>
        public static UInt16 GetAbilityIndex(String abilityName)
        {
            FfxiResource.init();
            String filterString = "Name = '" + FfxiResource.EncodeApostrophy(abilityName) + "'";
            MainDatabase.JARow[] jaRows = (MainDatabase.JARow[])FfxiResource.mainDb.JA.Select(filterString);
            if (jaRows.Length == 0)
            {
                return 0xFFFF;
            }
            else
            {
                return jaRows[0].RecastID;
            }
        }
        private static JA_INFO wrapInfo(MainDatabase.JARow iRow)
        {
            JA_INFO info = new JA_INFO();
            info.Name = FfxiResource.DecodeApostrophy(iRow.Name);
            info.ID = iRow.ID;
            info.Command = FfxiResource.DecodeApostrophy(iRow.Command);
            info.Targets = iRow.Targets;
            info.Job = iRow.Job;
            info.JobLevel = iRow.JobLevel;
            info.AsSub = iRow.AsSub;
            info.MP = iRow.MP;
            info.TP = iRow.TP;
            info.Duration = iRow.Duration;
            info.Element = iRow.Element;
            info.RecastID = iRow.RecastID;
            info.Type = iRow.Type;
            return info;
        }
        #endregion Get Functions
    }
}
