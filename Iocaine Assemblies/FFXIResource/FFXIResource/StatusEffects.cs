using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Iocaine2.Data.Client
{
    public partial class StatusEffects
    {
        #region Structures
        public struct STATUS_EFFECT_INFO
        {
            public ushort Index;
            public string Name;
        }
        #endregion Structures

        #region Private Members
        private static bool initDone = false;
        private static AutoCompleteStringCollection buffNameCollection = new AutoCompleteStringCollection();
        #endregion Private Members

        #region Public Properties
        public static AutoCompleteStringCollection StatusEffectNameCollection
        {
            get
            {
                return buffNameCollection;
            }
        }
        #endregion Public Properties

        #region Inits
        internal static void init()
        {
            if (!initDone)
            {
                loadData();
                initDone = true;
            }
        }
        private static void init_autoCompleteCollection()
        {
            buffNameCollection.Clear();
            string filterString = "";
            string sortString = "Name";
            MainDatabase.StatusEffectsRow[] statusEffectRows = (MainDatabase.StatusEffectsRow[])FfxiResource.mainDb.StatusEffects.Select(filterString, sortString);
            foreach (MainDatabase.StatusEffectsRow row in statusEffectRows)
            {
                buffNameCollection.Add(row.Name);
            }
        }
        #endregion Inits

        #region Get Functions
        /// <summary>
        /// Gets the status effect index with the given name.
        /// </summary>
        /// <param name="iName">Name of the status effect to look up.
        /// Do NOT use quotation marks around the name.</param>
        /// <returns>Index of the status effect with the given name.</returns>
        public static ushort[] GetStatusEffectID(string iName)
        {
            FfxiResource.init();
            ushort[] tempArray;
            string filterString = "Name = '" + FfxiResource.EncodeApostrophy(iName) + "'";
            MainDatabase.StatusEffectsRow[] statusEffectRows = (MainDatabase.StatusEffectsRow[])FfxiResource.mainDb.StatusEffects.Select(filterString);
            if (statusEffectRows.Length == 0)
            {
                tempArray = new ushort[1] { 0xFFFF };
                return tempArray;
            }
            else
            {
                tempArray = new ushort[statusEffectRows.Length];
                for (int ii = 0; ii < statusEffectRows.Length; ii++)
                {
                    tempArray[ii] = statusEffectRows[ii].ID;
                }
                return tempArray;
            }
        }
        /// <summary>
        /// Gets a data structure containing all stored information of the status effect with the given index.
        /// </summary>
        /// <param name="iID">Index of the status effect to look up.</param>
        /// <returns>Data structure with all of the stored info for the given status effect.</returns>
        public static STATUS_EFFECT_INFO GetStatusEffectInfo(ushort iID)
        {
            FfxiResource.init();
            STATUS_EFFECT_INFO info = new STATUS_EFFECT_INFO();

            string filterString = "ID = " + iID.ToString();
            string sortString = "ID";
            MainDatabase.StatusEffectsRow[] statusEffectRows = (MainDatabase.StatusEffectsRow[])FfxiResource.mainDb.StatusEffects.Select(filterString, sortString);
            if (statusEffectRows.Length == 0)
            {
                info.Index = 0xFFFF;
                info.Name = "";
            }
            else
            {
                info.Index = iID;
                info.Name = FfxiResource.DecodeApostrophy(statusEffectRows[0].Name);
            }
            return info;
        }
        public static STATUS_EFFECT_INFO GetStatusEffectInfo(string iName)
        {
            FfxiResource.init();
            STATUS_EFFECT_INFO info = new STATUS_EFFECT_INFO();

            string filterString = "Name = '" + FfxiResource.EncodeApostrophy(iName) + "'";
            string sortString = "ID";
            MainDatabase.StatusEffectsRow[] statusEffectRows = (MainDatabase.StatusEffectsRow[])FfxiResource.mainDb.StatusEffects.Select(filterString, sortString);
            if (statusEffectRows.Length == 0)
            {
                info.Index = 0xFFFF;
                info.Name = "";
            }
            else
            {
                info.Index = statusEffectRows[0].ID;
                info.Name = iName;
            }
            return info;
        }
        #endregion Get Functions
    }
}
