using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Skills
    {
        #region Structures
        public struct SKILLS_INFO
        {
            public UInt16 ID;
            public String Name;
            public String Category;
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
        /// Gets the status effect index with the given name.
        /// </summary>
        /// <param name="iName">Name of the status effect to look up.
        /// Do NOT use quotation marks around the name.</param>
        /// <returns>Index of the status effect with the given name.</returns>
        public static UInt16 GetSkillId(String iName)
        {
            FfxiResource.init();
            String filterString = "Name = '" + FfxiResource.EncodeApostrophy(iName) + "'";
            MainDatabase.SkillsRow[] skillsRows = (MainDatabase.SkillsRow[])FfxiResource.mainDb.Skills.Select(filterString);
            if (skillsRows.Length == 0)
            {
                return 0;
            }
            else
            {
                return skillsRows[0].ID;
            }
        }
        /// <summary>
        /// Gets the skill (combat/magic/craft) name of the given ID.
        /// </summary>
        /// <param name="iId">Skill ID.</param>
        /// <returns>Name of the skill.</returns>
        public static String GetSkillName(UInt16 iId)
        {
            FfxiResource.init();
            String filterString = "ID = " + iId;
            MainDatabase.SkillsRow[] skillsRows = (MainDatabase.SkillsRow[])FfxiResource.mainDb.Skills.Select(filterString);
            if (skillsRows.Length == 0)
            {
                return "Unknown";
            }
            else
            {
                return skillsRows[0].Name;
            }
        }
        public static String GetSkillCategory(UInt16 iId)
        {
            FfxiResource.init();
            String filterString = "ID = " + iId;
            MainDatabase.SkillsRow[] skillsRows = (MainDatabase.SkillsRow[])FfxiResource.mainDb.Skills.Select(filterString);
            if (skillsRows.Length == 0)
            {
                return "Unknown";
            }
            else
            {
                return skillsRows[0].Category;
            }
        }
        #endregion Get Functions
    }
}
