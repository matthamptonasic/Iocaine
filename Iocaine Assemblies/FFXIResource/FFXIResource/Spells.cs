using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Spells
    {
        #region Enums
        public enum TARGETS : short
        {
            ME_ONLY = 1,
            PARTY = 5,
            ANY_PC = 29,
            MOB = 32,
            ANY_PC_NPC = 63,
            DEAD_PC = 157
        }
        #endregion Enums

        #region Structures
        public struct SPELL_INFO
        {
            public ushort ID;
            public string Name;
            public string Type;
            public byte Skill;
            public string Command;
            public short Element;
            public ushort MP;
            public ushort RecastID;
            public float CastTime;
            public uint Duration;
            public ushort Range;
            public short Targets;
            public bool AsSub;
            public Dictionary<byte, byte> JobLevels;
        }
        #endregion Structures

        #region Private Members
        private static bool initDone = false;
        private const ushort invalidId = 0xffff;
        private const string invalidName = "Unknown";
        private const string invalidType = "Unknown";
        private const byte invalidSkill = 0xff;
        #endregion Private Members

        #region Public Properties
        private static ushort InvalidId
        {
            get
            {
                return invalidId;
            }
        }
        private static string InvalidName
        {
            get
            {
                return invalidName;
            }
        }
        private static string InvalidType
        {
            get
            {
                return invalidType;
            }
        }
        private static byte InvalidSkill
        {
            get
            {
                return invalidSkill;
            }
        }
        #endregion Public Properties

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

        #region Public Methods
        /// <summary>
        /// Gets the spell index with the given name.
        /// </summary>
        /// <param name="iName">Name of the spell to look up.
        /// Do NOT use quotation marks around the name.</param>
        /// <returns>Index of the spell with the given name.</returns>
        public static ushort GetSpellID(string iName)
        {
            FfxiResource.init();
            string filterString = "Name = '" + FfxiResource.EncodeApostrophy(iName) + "'";
            MainDatabase.SpellsRow[] spellRows = (MainDatabase.SpellsRow[])FfxiResource.mainDb.Spells.Select(filterString);
            if (spellRows.Length == 0)
            {
                return invalidId;
            }
            else
            {
                return spellRows[0].ID;
            }
        }
        /// <summary>
        /// Gets a data structure containing all stored information of the spell with the given index.
        /// </summary>
        /// <param name="iId">Index of the spell to look up.</param>
        /// <returns>Data structure with all of the stored info for the given spell.</returns>
        public static SPELL_INFO GetSpellInfo(ushort iId)
        {
            FfxiResource.init();
            SPELL_INFO info = new SPELL_INFO();

            string filterString = "ID = " + iId.ToString();
            string sortString = "ID";
            MainDatabase.SpellsRow[] spellRows = (MainDatabase.SpellsRow[])FfxiResource.mainDb.Spells.Select(filterString, sortString);
            if (spellRows.Length == 0)
            {
                getNullInfo(ref info);
            }
            else
            {
                info.ID = iId;
                info.Name = FfxiResource.DecodeApostrophy(spellRows[0].Name);
                info.Type = spellRows[0].Type;
                info.Skill = spellRows[0].Skill;
                info.Command = FfxiResource.DecodeApostrophy(spellRows[0].Command);
                info.Element = spellRows[0].Element;
                info.MP = spellRows[0].MP;
                info.RecastID = spellRows[0].RecastID;
                info.CastTime = spellRows[0].CastTime;
                info.Duration = spellRows[0].Duration;
                info.Range = spellRows[0].Range;
                info.Targets = spellRows[0].Targets;
                info.AsSub = spellRows[0].AsSub;
                setJobLevels(spellRows[0], ref info);
            }
            return info;
        }
        /// <summary>
        /// Gets a data structure containing all stored information of the spell with the given name.
        /// </summary>
        /// <param name="iName">Name of the spell to look up.</param>
        /// <returns>Data structure with all of the stored info for the given spell.</returns>
        public static SPELL_INFO GetSpellInfo(string iName)
        {
            return GetSpellInfo(GetSpellID(iName));
        }
        /// <summary>
        /// Gets a list of SPELL_INFO structures containing all of the spells that can be used
        /// by the given job and subjob at the given level.
        /// </summary>
        /// <param name="iJob">Main job.</param>
        /// <param name="iSubJob">Sub job (pass 0 if none).</param>
        /// <param name="iJobLevel">Main job level.</param>
        /// <returns></returns>
        public static List<SPELL_INFO> GetSpellInfo(byte iJob, byte iSubJob, byte iJobLevel)
        {
            FfxiResource.init();
            string filterString;
            string sortString;
            byte subJobLevel = (iJobLevel == 1) ? (byte)1 : (byte)(iJobLevel / 2);
            filterString = "(LevelJob" + iJob.ToString() + " <> 0 AND LevelJob" + iJob.ToString() + " <= " + iJobLevel.ToString() + ")";
            if (iSubJob != 0)
            {
                filterString += " OR (LevelJob" + iSubJob.ToString() + " <> 0 AND LevelJob" + iSubJob.ToString() + " <= " + subJobLevel.ToString() + ")";
            }
            sortString = "Name";

            MainDatabase.SpellsRow[] spellRows = (MainDatabase.SpellsRow[])FfxiResource.mainDb.Spells.Select(filterString, sortString);
            return wrapRows(spellRows);
        }
        /// <summary>
        /// Gets a list of SPELL_INFO structures for all spells.
        /// </summary>
        /// <returns></returns>
        public static List<SPELL_INFO> GetSpellInfo()
        {
            FfxiResource.init();
            string filterString;
            string sortString;
            filterString = "";
            sortString = "Name";

            MainDatabase.SpellsRow[] spellRows = (MainDatabase.SpellsRow[])FfxiResource.mainDb.Spells.Select(filterString, sortString);
            return wrapRows(spellRows);
        }
        public static List<SPELL_INFO> GetSpellInfoBySkill(ushort iSkillId)
        {
            FfxiResource.init();
            string filterString;
            string sortString;
            filterString = "Skill=" + iSkillId;
            sortString = "Name";

            MainDatabase.SpellsRow[] spellRows = (MainDatabase.SpellsRow[])FfxiResource.mainDb.Spells.Select(filterString, sortString);
            return wrapRows(spellRows);
        }
        public static List<SPELL_INFO> GetCuresInfo(byte iJob, byte iSubJob, byte iJobLevel)
        {
            FfxiResource.init();
            string filterString;
            string sortString;
            byte subJobLevel = (iJobLevel == 1) ? (byte)1 : (byte)(iJobLevel / 2);
            filterString = "(LevelJob" + iJob.ToString() + " <> 0 AND LevelJob" + iJob.ToString() + " <= " + iJobLevel.ToString() + ")";
            if (iSubJob != 0)
            {
                filterString = filterString.Insert(0, "(");
                filterString += " OR (LevelJob" + iSubJob.ToString() + " <> 0 AND LevelJob" + iSubJob.ToString() + " <= " + subJobLevel.ToString() + "))";
            }
            filterString += " AND (Targets = " + (ushort)TARGETS.ANY_PC_NPC + " AND Skill = 33)"; //TBD - make skill based on Skill table.
            //TBD - Add BLU curing.
            sortString = "Name";

            MainDatabase.SpellsRow[] spellRows = (MainDatabase.SpellsRow[])FfxiResource.mainDb.Spells.Select(filterString, sortString);
            return wrapRows(spellRows);
        }
        public static List<SPELL_INFO> GetPartyBuffsInfo(byte iJob, byte iSubJob, byte iJobLevel)
        {
            FfxiResource.init();
            string filterString;
            string sortString;
            byte subJobLevel = (iJobLevel == 1) ? (byte)1 : (byte)(iJobLevel / 2);
            filterString = "(LevelJob" + iJob.ToString() + " <> 0 AND LevelJob" + iJob.ToString() + " <= " + iJobLevel.ToString() + ")";
            if (iSubJob != 0)
            {
                filterString = filterString.Insert(0, "(");
                filterString += " OR (LevelJob" + iSubJob.ToString() + " <> 0 AND LevelJob" + iSubJob.ToString() + " <= " + subJobLevel.ToString() + "))";
            }
            filterString += " AND (Targets = " + (ushort)TARGETS.PARTY;
            filterString += " OR Targets = " + (ushort)TARGETS.ANY_PC + ")";
            filterString += " AND Type <> 'Trust'";
            filterString += " AND Skill <> 33"; //TBD - make skill based on Skill table.
            filterString += " AND Name <> 'Erase'";
            filterString += " AND Name <> 'Invisible'";
            filterString += " AND Name <> 'Sneak'";
            filterString += " AND Name <> 'Deodorize'";
            filterString += " AND Name <> 'Retrace'";
            filterString += " AND Name <> 'Warp II'";
            //TBD - Remove BLU curing.
            sortString = "Name";

            MainDatabase.SpellsRow[] spellRows = (MainDatabase.SpellsRow[])FfxiResource.mainDb.Spells.Select(filterString, sortString);
            return wrapRows(spellRows);
        }
        public static List<SPELL_INFO> GetSelfBuffsInfo(byte iJob, byte iSubJob, byte iJobLevel)
        {
            FfxiResource.init();
            string filterString;
            string sortString;
            byte subJobLevel = (iJobLevel == 1) ? (byte)1 : (byte)(iJobLevel / 2);
            filterString = "(LevelJob" + iJob.ToString() + " <> 0 AND LevelJob" + iJob.ToString() + " <= " + iJobLevel.ToString() + ")";
            if (iSubJob != 0)
            {
                filterString = filterString.Insert(0, "(");
                filterString += " OR (LevelJob" + iSubJob.ToString() + " <> 0 AND LevelJob" + iSubJob.ToString() + " <= " + subJobLevel.ToString() + "))";
            }
            filterString += " AND (Targets = " + (ushort)TARGETS.ME_ONLY;
            filterString += " OR Targets = " + (ushort)TARGETS.PARTY;
            filterString += " OR Targets = " + (ushort)TARGETS.ANY_PC + ")";
            filterString += " AND Type <> 'Trust'";
            filterString += " AND (Skill <> 33 OR Name LIKE 'Reraise%')"; //TBD - make skill based on Skill table.
            filterString += " AND Name <> 'Hastega'";
            filterString += " AND Name <> 'Erase'";
            filterString += " AND Name <> 'Invisible'";
            filterString += " AND Name <> 'Sneak'";
            filterString += " AND Name <> 'Deodorize'";
            filterString += " AND Name <> 'Escape'";
            filterString += " AND Name <> 'Retrace'";
            filterString += " AND Name NOT LIKE 'Teleport-%'";
            filterString += " AND Name NOT LIKE 'Recall-%'";
            filterString += " AND Name NOT LIKE 'Warp%'";
            //TBD - Remove BLU curing.
            sortString = "Name";

            MainDatabase.SpellsRow[] spellRows = (MainDatabase.SpellsRow[])FfxiResource.mainDb.Spells.Select(filterString, sortString);
            return wrapRows(spellRows);
        }
        public static List<SPELL_INFO> GetHealingInfo(byte iJob, byte iSubJob, byte iJobLevel)
        {
            FfxiResource.init();
            string filterString;
            string sortString;
            byte subJobLevel = (iJobLevel == 1) ? (byte)1 : (byte)(iJobLevel / 2);
            filterString = "(LevelJob" + iJob.ToString() + " <> 0 AND LevelJob" + iJob.ToString() + " <= " + iJobLevel.ToString() + ")";
            if (iSubJob != 0)
            {
                filterString = filterString.Insert(0, "(");
                filterString += " OR (LevelJob" + iSubJob.ToString() + " <> 0 AND LevelJob" + iSubJob.ToString() + " <= " + subJobLevel.ToString() + "))";
            }
            filterString += " AND ((Targets = " + (ushort)TARGETS.ANY_PC + " AND Skill = 33)"; //TBD - make skill based on Skill table.
            filterString += " OR Targets = " + (ushort)TARGETS.DEAD_PC; //Raise, arise, tractor.
            filterString += " OR Name = 'Esuna'";
            filterString += " OR Name = 'Sacrifice'";
            filterString += " OR Name = 'Erase'";
            filterString += ")";
            sortString = "Name";

            MainDatabase.SpellsRow[] spellRows = (MainDatabase.SpellsRow[])FfxiResource.mainDb.Spells.Select(filterString, sortString);
            return wrapRows(spellRows);
        }
        #endregion Public Methods

        #region Private Methods
        private static List<SPELL_INFO> wrapRows(MainDatabase.SpellsRow[] iSpellRows)
        {
            List<SPELL_INFO> infoList = new List<SPELL_INFO>();
            foreach (MainDatabase.SpellsRow row in iSpellRows)
            {
                SPELL_INFO info = new SPELL_INFO();
                info.ID = row.ID;
                info.Name = FfxiResource.DecodeApostrophy(row.Name);
                info.Type = row.Type;
                info.Skill = row.Skill;
                info.Command = FfxiResource.DecodeApostrophy(row.Command);
                info.Element = row.Element;
                info.MP = row.MP;
                info.RecastID = row.RecastID;
                info.CastTime = row.CastTime;
                info.Duration = row.Duration;
                info.Range = row.Range;
                info.Targets = row.Targets;
                info.AsSub = row.AsSub;
                setJobLevels(row, ref info);
                infoList.Add(info);
            }
            return infoList;
        }
        private static void setJobLevels(MainDatabase.SpellsRow iRow, ref SPELL_INFO oInfo)
        {
            oInfo.JobLevels = new Dictionary<byte, byte>();
            oInfo.JobLevels.Add(1, iRow.LevelJob1);
            oInfo.JobLevels.Add(2, iRow.LevelJob2);
            oInfo.JobLevels.Add(3, iRow.LevelJob3);
            oInfo.JobLevels.Add(4, iRow.LevelJob4);
            oInfo.JobLevels.Add(5, iRow.LevelJob5);
            oInfo.JobLevels.Add(6, iRow.LevelJob6);
            oInfo.JobLevels.Add(7, iRow.LevelJob7);
            oInfo.JobLevels.Add(8, iRow.LevelJob8);
            oInfo.JobLevels.Add(9, iRow.LevelJob9);
            oInfo.JobLevels.Add(10, iRow.LevelJob10);
            oInfo.JobLevels.Add(11, iRow.LevelJob11);
            oInfo.JobLevels.Add(12, iRow.LevelJob12);
            oInfo.JobLevels.Add(13, iRow.LevelJob13);
            oInfo.JobLevels.Add(14, iRow.LevelJob14);
            oInfo.JobLevels.Add(15, iRow.LevelJob15);
            oInfo.JobLevels.Add(16, iRow.LevelJob16);
            oInfo.JobLevels.Add(17, iRow.LevelJob17);
            oInfo.JobLevels.Add(18, iRow.LevelJob18);
            oInfo.JobLevels.Add(19, iRow.LevelJob19);
            oInfo.JobLevels.Add(20, iRow.LevelJob20);
            oInfo.JobLevels.Add(21, iRow.LevelJob21);
            oInfo.JobLevels.Add(22, iRow.LevelJob22);
            oInfo.JobLevels.Add(23, iRow.LevelJob23);
        }
        private static SPELL_INFO getNullInfo()
        {
            SPELL_INFO info = new SPELL_INFO();
            info.ID = invalidId;
            info.Name = invalidName;
            info.Type = invalidType;
            info.Skill = Skills.InvalidSkill;
            info.Command = "";
            info.Element = Elements.InvalidElement;
            info.MP = 0;
            info.RecastID = 0;
            info.CastTime = 0;
            info.Duration = 0;
            info.Range = 0;
            info.Targets = Targets.InvalidId;
            info.AsSub = false;
            info.JobLevels = getNullDictionary();
            return info;
        }
        private static void getNullInfo(ref SPELL_INFO oInfo)
        {
            oInfo.ID = invalidId;
            oInfo.Name = invalidName;
            oInfo.Type = invalidType;
            oInfo.Skill = Skills.InvalidSkill;
            oInfo.Command = "";
            oInfo.Element = Elements.InvalidElement;
            oInfo.MP = 0;
            oInfo.RecastID = 0;
            oInfo.CastTime = 0;
            oInfo.Duration = 0;
            oInfo.Range = 0;
            oInfo.Targets = Targets.InvalidId;
            oInfo.AsSub = false;
            oInfo.JobLevels = getNullDictionary();
        }
        private static Dictionary<byte, byte> getNullDictionary()
        {
            return new Dictionary<byte, byte>() { { 1, 0 }, { 2, 0 },
                                                  { 3, 0 }, { 4, 0 },
                                                  { 5, 0 }, { 6, 0 },
                                                  { 7, 0 }, { 8, 0 },
                                                  { 9, 0 }, { 10, 0 },
                                                  { 11, 0 }, { 12, 0 },
                                                  { 13, 0 }, { 14, 0 },
                                                  { 15, 0 }, { 16, 0 },
                                                  { 17, 0 }, { 18, 0 },
                                                  { 19, 0 }, { 20, 0 },
                                                  { 21, 0 }, { 22, 0 },
                                                  { 23, 0 } };
        }
        #endregion Private Methods
    }
}
