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
            public byte LevelJob1;
            public byte LevelJob2;
            public byte LevelJob3;
            public byte LevelJob4;
            public byte LevelJob5;
            public byte LevelJob6;
            public byte LevelJob7;
            public byte LevelJob8;
            public byte LevelJob9;
            public byte LevelJob10;
            public byte LevelJob11;
            public byte LevelJob12;
            public byte LevelJob13;
            public byte LevelJob14;
            public byte LevelJob15;
            public byte LevelJob16;
            public byte LevelJob17;
            public byte LevelJob18;
            public byte LevelJob19;
            public byte LevelJob20;
            public byte LevelJob21;
            public byte LevelJob22;
            public byte LevelJob23;
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
                return 0xFFFF;
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
                info.ID = 0xFFFF;
                info.Name = "";
                info.Type = "";
                info.Skill = 0;
                info.Command = "";
                info.Element = 0;
                info.MP = 0;
                info.RecastID = 0;
                info.CastTime = 0;
                info.Duration = 0;
                info.Range = 0;
                info.Targets = 0;
                info.AsSub = false;
                info.LevelJob1 = 0;
                info.LevelJob2 = 0;
                info.LevelJob3 = 0;
                info.LevelJob4 = 0;
                info.LevelJob5 = 0;
                info.LevelJob6 = 0;
                info.LevelJob7 = 0;
                info.LevelJob8 = 0;
                info.LevelJob9 = 0;
                info.LevelJob10 = 0;
                info.LevelJob11 = 0;
                info.LevelJob12 = 0;
                info.LevelJob13 = 0;
                info.LevelJob14 = 0;
                info.LevelJob15 = 0;
                info.LevelJob16 = 0;
                info.LevelJob17 = 0;
                info.LevelJob18 = 0;
                info.LevelJob19 = 0;
                info.LevelJob20 = 0;
                info.LevelJob21 = 0;
                info.LevelJob22 = 0;
                info.LevelJob23 = 0;
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
                info.LevelJob1 = spellRows[0].LevelJob1;
                info.LevelJob2 = spellRows[0].LevelJob2;
                info.LevelJob3 = spellRows[0].LevelJob3;
                info.LevelJob4 = spellRows[0].LevelJob4;
                info.LevelJob5 = spellRows[0].LevelJob5;
                info.LevelJob6 = spellRows[0].LevelJob6;
                info.LevelJob7 = spellRows[0].LevelJob7;
                info.LevelJob8 = spellRows[0].LevelJob8;
                info.LevelJob9 = spellRows[0].LevelJob9;
                info.LevelJob10 = spellRows[0].LevelJob10;
                info.LevelJob11 = spellRows[0].LevelJob11;
                info.LevelJob12 = spellRows[0].LevelJob12;
                info.LevelJob13 = spellRows[0].LevelJob13;
                info.LevelJob14 = spellRows[0].LevelJob14;
                info.LevelJob15 = spellRows[0].LevelJob15;
                info.LevelJob16 = spellRows[0].LevelJob16;
                info.LevelJob17 = spellRows[0].LevelJob17;
                info.LevelJob18 = spellRows[0].LevelJob18;
                info.LevelJob19 = spellRows[0].LevelJob19;
                info.LevelJob20 = spellRows[0].LevelJob20;
                info.LevelJob21 = spellRows[0].LevelJob21;
                info.LevelJob22 = spellRows[0].LevelJob22;
                info.LevelJob23 = spellRows[0].LevelJob23;
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
                info.LevelJob1 = row.LevelJob1;
                info.LevelJob2 = row.LevelJob2;
                info.LevelJob3 = row.LevelJob3;
                info.LevelJob4 = row.LevelJob4;
                info.LevelJob5 = row.LevelJob5;
                info.LevelJob6 = row.LevelJob6;
                info.LevelJob7 = row.LevelJob7;
                info.LevelJob8 = row.LevelJob8;
                info.LevelJob9 = row.LevelJob9;
                info.LevelJob10 = row.LevelJob10;
                info.LevelJob11 = row.LevelJob11;
                info.LevelJob12 = row.LevelJob12;
                info.LevelJob13 = row.LevelJob13;
                info.LevelJob14 = row.LevelJob14;
                info.LevelJob15 = row.LevelJob15;
                info.LevelJob16 = row.LevelJob16;
                info.LevelJob17 = row.LevelJob17;
                info.LevelJob18 = row.LevelJob18;
                info.LevelJob19 = row.LevelJob19;
                info.LevelJob20 = row.LevelJob20;
                info.LevelJob21 = row.LevelJob21;
                info.LevelJob22 = row.LevelJob22;
                info.LevelJob23 = row.LevelJob23;
                infoList.Add(info);
            }
            return infoList;
        }
        #endregion Get Functions
    }
}
