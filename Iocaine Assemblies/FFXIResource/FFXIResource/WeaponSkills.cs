using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class WeaponSkills
    {
        #region Structures
        public struct WS_INFO
        {
            public UInt16 ID;
            public String Name;
            public String Command;
            public Byte SkillType;
            public UInt16 SkillLevel;
            public String Special;
            public UInt32 Jobs;
            public UInt32 SubJobs;
            public String AttributeA;
            public String AttributeB;
            public String AttributeC;
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
        /// Gets a list of weapon skill ID's based on the skill type, job, subjob, and weapon skill level given.
        /// </summary>
        /// <param name="iSkillType">Skill type (ie Sword, Axe, etc).</param>
        /// <param name="iJob">Main job.</param>
        /// <param name="iSubJob">Sub job. Pass 0 if no sub job used.</param>
        /// <param name="iSkillLevel">Weapon skill level.</param>
        /// <returns></returns>
        public static List<UInt16> GetWeaponSkillIDs(Byte iSkillType, Byte iJob, Byte iSubJob, UInt16 iSkillLevel)
        {
            FfxiResource.init();
            List<UInt16> idList = new List<UInt16>();
            MainDatabase.WSRow[] wsRows = selectWsRows(iSkillType, iSkillLevel);
            foreach (MainDatabase.WSRow row in wsRows)
            {
                if ((((row.Jobs >> iJob) & 0x1) == 1)
                    || (((row.JobsSubs >> iSubJob) & 0x1) == 1))
                {
                    idList.Add(row.ID);
                }
            }
            return idList;
        }
        /// <summary>
        /// Gets a list of weapon skill ID's based on the skill type and weapon skill level given.
        /// </summary>
        /// <param name="iSkillType">Skill type (ie Sword, Axe, etc).</param>
        /// <param name="iSkillLevel">Weapon skill level.</param>
        /// <returns></returns>
        public static List<UInt16> GetWeaponSkillIDs(Byte iSkillType, UInt16 iSkillLevel)
        {
            FfxiResource.init();
            List<UInt16> idList = new List<UInt16>();
            MainDatabase.WSRow[] wsRows = selectWsRows(iSkillType, iSkillLevel);
            foreach (MainDatabase.WSRow row in wsRows)
            {
                idList.Add(row.ID);
            }
            return idList;
        }
        /// <summary>
        /// Gets a list of weapon skill ID's based on the skill type given.
        /// </summary>
        /// <param name="iSkillType"></param>
        /// <returns></returns>
        public static List<UInt16> GetWeaponSkillIDs(Byte iSkillType)
        {
            FfxiResource.init();
            List<UInt16> idList = new List<UInt16>();
            MainDatabase.WSRow[] wsRows = selectWsRows(iSkillType, 1000);
            foreach (MainDatabase.WSRow row in wsRows)
            {
                idList.Add(row.ID);
            }
            return idList;
        }
        /// <summary>
        /// Gets the weapon skill ID of the given weapon skill name.
        /// </summary>
        /// <param name="iName">Name to get the ID of.</param>
        /// <returns>Returns 0xFFFF if not found.</returns>
        public static UInt16 GetWeaponSkillID(String iName)
        {
            FfxiResource.init();
            String filterString = "Name = '" + FfxiResource.EncodeApostrophy(iName) + "'";
            MainDatabase.WSRow[] wsRows = (MainDatabase.WSRow[])FfxiResource.mainDb.WS.Select(filterString);
            if (wsRows.Length == 0)
            {
                return 0xFFFF;
            }
            else
            {
                return wsRows[0].ID;
            }
        }
        /// <summary>
        /// Gets the weapon skill name for the given ID.
        /// </summary>
        /// <param name="iWsId">ID to get the corresponding name for.</param>
        /// <returns></returns>
        public static String GetWeaponSkillName(UInt16 iWsId)
        {
            FfxiResource.init();
            String filterString = "ID=" + iWsId.ToString();
            MainDatabase.WSRow[] wsRows = (MainDatabase.WSRow[])FfxiResource.mainDb.WS.Select(filterString);
            if (wsRows.Length == 0)
            {
                return "";
            }
            else
            {
                return FfxiResource.DecodeApostrophy(wsRows[0].Name);
            }
        }
        /// <summary>
        /// Gets the weapon skill command for the given ID.
        /// </summary>
        /// <param name="iWsId">ID to get the corresponding command for.</param>
        /// <returns></returns>
        public static String GetWeaponSkillCommand(UInt16 iWsId)
        {
            FfxiResource.init();
            String filterString = "ID=" + iWsId.ToString();
            MainDatabase.WSRow[] wsRows = (MainDatabase.WSRow[])FfxiResource.mainDb.WS.Select(filterString);
            if (wsRows.Length == 0)
            {
                return "";
            }
            else
            {
                return FfxiResource.DecodeApostrophy(wsRows[0].Command);
            }
        }
        /// <summary>
        /// Gets the weapon skill level for the given ID.
        /// </summary>
        /// <param name="iWsId">ID to get the corresponding level for.</param>
        /// <returns></returns>
        public static UInt16 GetWeaponSkillLevel(UInt16 iWsId)
        {
            FfxiResource.init();
            String filterString = "ID=" + iWsId.ToString();
            MainDatabase.WSRow[] wsRows = (MainDatabase.WSRow[])FfxiResource.mainDb.WS.Select(filterString);
            if (wsRows.Length == 0)
            {
                return 0xFFFF;
            }
            else
            {
                return wsRows[0].SkillLevel;
            }
        }
        /// <summary>
        /// Gets bit vector list of jobs which can use the weapon skill with the given ID.
        /// </summary>
        /// <param name="iWsId">ID of weapon skill for which to get the job/subjob vectors.</param>
        /// <param name="oJobs">Output bitvector of jobs that can use the weapon skill with the given ID.
        /// ex. If bit 1 is set, the WAR job can use this weapon skill. Bit 0 is always 0.</param>
        /// <param name="oSubJobs">Output bitvector of sub jobs that can use the weapon skill with the given ID.
        /// ex. If bit 2 is set, the MNK job can use this weapon skill. Bit 0 is always 0.</param>
        public static void GetWeaponSkillJobs(UInt16 iWsId, ref UInt32 oJobs, ref UInt32 oSubJobs)
        {
            FfxiResource.init();
            String filterString = "ID=" + iWsId.ToString();
            MainDatabase.WSRow[] wsRows = (MainDatabase.WSRow[])FfxiResource.mainDb.WS.Select(filterString);
            if (wsRows.Length == 0)
            {
                oJobs = 0;
                oSubJobs = 0;
            }
            else
            {
                oJobs = wsRows[0].Jobs;
                oSubJobs = wsRows[0].JobsSubs;
            }
        }
        /// <summary>
        /// Gets the A, B, and C (if applicable) skill chain attributes for the given weapon skill ID.
        /// </summary>
        /// <param name="iWsId">ID of the weapon skill to get the skill chain attributes for.</param>
        /// <param name="oAttrA">Skill chain attribute A (0 if none).</param>
        /// <param name="oAttrB">Skill chain attribute B (0 if none).</param>
        /// <param name="oAttrC">Skill chain attribute C (0 if none).</param>
        public static void GetWeaponSkillAttributes(UInt16 iWsId, out String oAttrA, out String oAttrB, out String oAttrC)
        {
            FfxiResource.init();
            String filterString = "ID=" + iWsId.ToString();
            MainDatabase.WSRow[] wsRows = (MainDatabase.WSRow[])FfxiResource.mainDb.WS.Select(filterString);
            if (wsRows.Length == 0)
            {
                oAttrA = "";
                oAttrB = "";
                oAttrC = "";
            }
            else
            {
                oAttrA = wsRows[0].AttrA;
                oAttrB = wsRows[0].AttrB;
                oAttrC = wsRows[0].AttrC;
            }
        }
        /// <summary>
        /// Gets a full structure of all of the information related to the given WS ID.
        /// </summary>
        /// <param name="iWsId">ID of the weapon skill for which to get the information.</param>
        /// <returns>A structure with all stored information for the given weapon skill.</returns>
        public static WS_INFO GetWeaponSkillInfo(UInt16 iWsId)
        {
            FfxiResource.init();
            WS_INFO info = new WS_INFO();
            String filterString = "ID=" + iWsId.ToString();
            MainDatabase.WSRow[] wsRows = (MainDatabase.WSRow[])FfxiResource.mainDb.WS.Select(filterString);
            if (wsRows.Length == 0)
            {
                info.ID = 0xFFFF;
                info.Name = "";
                info.Command = "";
                info.SkillType = 0;
                info.SkillLevel = 0;
                info.Special = "";
                info.Jobs = 0;
                info.SubJobs = 0;
                info.AttributeA = "";
                info.AttributeB = "";
                info.AttributeC = "";
            }
            else
            {
                info.ID = iWsId;
                info.Name = FfxiResource.DecodeApostrophy(wsRows[0].Name);
                info.Command = FfxiResource.DecodeApostrophy(wsRows[0].Command);
                info.SkillType = wsRows[0].SkillType;
                info.SkillLevel = wsRows[0].SkillLevel;
                info.Special = wsRows[0].Special;
                info.Jobs = wsRows[0].Jobs;
                info.SubJobs = wsRows[0].JobsSubs;
                info.AttributeA = wsRows[0].AttrA;
                info.AttributeB = wsRows[0].AttrB;
                info.AttributeC = wsRows[0].AttrC;
            }
            return info;
        }
        /// <summary>
        /// Gets a full structure of all of the information related to the given WS name.
        /// </summary>
        /// <param name="iName">Name of the weapon skill for which to get the information.</param>
        /// <returns>A structure with all stored information for the given weapon skill.</returns>
        public static WS_INFO GetWeaponSkillInfo(String iName)
        {
            UInt16 wsId = GetWeaponSkillID(iName);
            return GetWeaponSkillInfo(wsId);
        }
        public static List<WS_INFO> GetWeaponSkillInfo()
        {
            FfxiResource.init();
            List<WS_INFO> infoList = new List<WS_INFO>();
            String filterString = "";
            MainDatabase.WSRow[] wsRows = (MainDatabase.WSRow[])FfxiResource.mainDb.WS.Select(filterString);
            foreach(MainDatabase.WSRow row in wsRows)
            {
                WS_INFO info = new WS_INFO();
                info.Name = row.Name;
                info.ID = row.ID;
                info.Command = row.Command;
                info.SkillType = row.SkillType;
                info.SkillLevel = row.SkillLevel;
                info.Special = row.Special;
                info.Jobs = row.Jobs;
                info.SubJobs = row.JobsSubs;
                info.AttributeA = row.AttrA;
                info.AttributeB = row.AttrB;
                info.AttributeC = row.AttrC;
                infoList.Add(info);
            }
            return infoList;
        }
        public static List<WS_INFO> GetWeaponSkillInfo(Byte iSkillType, Byte iJob = 0, Byte iSubJob = 0, UInt16 iSkillLevel = 0)
        {
            FfxiResource.init();
            List<WS_INFO> infoList = new List<WS_INFO>();
            String filterString = "SkillType = " + iSkillType;
            if(iSkillLevel > 0)
            {
                filterString += " AND SkillLevel = " + iSkillLevel;
            }
            MainDatabase.WSRow[] wsRows = (MainDatabase.WSRow[])FfxiResource.mainDb.WS.Select(filterString);
            foreach (MainDatabase.WSRow row in wsRows)
            {
                Boolean jobOk = false;
                Boolean subOk = false;
                if(iJob > 0)
                {
                    if(((row.Jobs >> iJob) & 1) == 1)
                    {
                        jobOk = true;
                    }
                    else if(iSubJob > 0)
                    {
                        if(((row.JobsSubs >> iSubJob) & 1) == 1)
                        {
                            subOk = true;
                        }
                    }
                }
                else
                {
                    jobOk = true;
                }

                if (jobOk || subOk)
                {
                    WS_INFO info = new WS_INFO();
                    info.Name = row.Name;
                    info.ID = row.ID;
                    info.Command = row.Command;
                    info.SkillType = row.SkillType;
                    info.SkillLevel = row.SkillLevel;
                    info.Special = row.Special;
                    info.Jobs = row.Jobs;
                    info.SubJobs = row.JobsSubs;
                    info.AttributeA = row.AttrA;
                    info.AttributeB = row.AttrB;
                    info.AttributeC = row.AttrC;
                    infoList.Add(info);
                }
            }
            return infoList;
        }
        #endregion Get Functions
        #region Private Functions
        private static MainDatabase.WSRow[] selectWsRows(Byte iSkillType, ushort iSkillLevel)
        {
            String filterString = "SkillType = " + iSkillType.ToString() + " AND SkillLevel <= " + iSkillLevel.ToString();
            String sortString = "SkillLevel";
            return (MainDatabase.WSRow[])FfxiResource.mainDb.WS.Select(filterString, sortString);
        }
        #endregion Private Functions
    }
}
