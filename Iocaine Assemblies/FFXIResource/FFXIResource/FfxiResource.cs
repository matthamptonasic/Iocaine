using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    /// <summary>
    /// Class that handles initialization of the JobAbilities, WeaponSkills, Spells, and StatusEffects data classes.
    /// </summary>
    public class FfxiResource
    {
        #region Member Variables
        #region Private/Internal Members
        private static bool initDone = false;
        internal static String apostrophy = "&apst";
        internal static MainDatabase mainDb;
        #endregion Private/Internal Members

        #region Public Properties
        //public static MainDatabase.BaitDataTable baitTable = mainDb.Bait;
        #endregion Public Properties
        #endregion Member Variables

        #region Public Methods
        #region Init
        public static void init()
        {
            if (!initDone)
            {
                if (mainDb == null)
                {
                    mainDb = new MainDatabase();
                }
                JobAbilities.init();
                WeaponSkills.init();
                Spells.init();
                StatusEffects.init();
                Elements.init();
                Skills.init();
                Targets.init();
                Jobs.init();
                Bait.init();
                Fish.init();
                Rods.init();
                Servers.init();
                Zones.init();
                mainDb.CaseSensitive = false;
                initDone = true;
            }
        }
        #endregion Init

        #region Apostrophies
        public static String EncodeApostrophy(String str)
        {
            if (str.Contains("'"))
            {
                return str.Replace("'", apostrophy);
            }
            else
            {
                return str;
            }
        }
        public static String DecodeApostrophy(String str)
        {
            if (str.Contains(apostrophy))
            {
                return str.Replace(apostrophy, "'");
            }
            else
            {
                return str;
            }
        }
        #endregion Apostrophies
        #endregion Public Methods
    }
}
