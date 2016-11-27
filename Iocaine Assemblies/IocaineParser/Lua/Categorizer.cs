using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Parsing
{
    public static partial class Lua
    {
        public static class Categorizer
        {
            #region Description
            /*
                This is where we run through each gear item and break down the item description into categories
                and then build the ParsedDataset.GearAttributes table.

            */
            #endregion Description

            #region Structs
            private struct SubstitutionPair
            {
                public ushort m_id;
                public string m_old;
                public string m_new;
            }
            #endregion Structs

            #region Private Members
            private static Dictionary<ushort, string> m_desc;
            private static Dictionary<ushort, Items.ItemInfo> m_items;
            private static Dictionary<string, ushort> m_ids;
            private static List<ushort> m_armorIds;
            private static List<ushort> m_weaponIds;

            private const string m_filterFileName = @"Parsing\Gear_Attribute_Filters.txt";
            private static Dictionary<string, List<string>> m_filters; // <column_name, List<filter_strings>>
            private const string m_subsFileName = @"Parsing\Gear_Attribute_Substitutions.txt";
            private static List<SubstitutionPair> m_substitutions;
            #endregion Private Members

            #region Public Properties
            #endregion Public Properties

            #region Inits
            #endregion Inits

            #region Internal Methods
            public static void Load()
            {
                loadFilters();
            }
            internal static void SetDescription(ref Dictionary<ushort, string> iDesc)
            {
                m_desc = iDesc;
            }
            internal static void SetItems(ref Dictionary<ushort, Items.ItemInfo> iItems)
            {
                m_items = iItems;
            }
            internal static void SetIds(ref Dictionary<string, ushort> iIds)
            {
                m_ids = iIds;
            }
            internal static void SetArmorIds(ref List<ushort> iArmorIds)
            {
                m_armorIds = iArmorIds;
            }
            internal static void SetWeaponIds(ref List<ushort> iWeaponIds)
            {
                m_weaponIds = iWeaponIds;
            }
            #endregion Internal Methods

            #region Private Methods
            private static void loadFilters()
            {
                m_filters = new Dictionary<string, List<string>>();
                if (File.Exists(m_filterFileName))
                {
                    StreamReader l_reader = new StreamReader(m_filterFileName, UTF8Encoding.UTF8);
                    string l_line = "";
                    while (!l_reader.EndOfStream)
                    {
                        l_line = l_reader.ReadLine();


                        string l_pattern = "([^=]*)=(.*)$";
                        Regex l_regex = new Regex(l_pattern);
                        Match l_match = l_regex.Match(l_line);
                        if (l_match.Groups.Count != 3)
                        {
                            MessageBox.Show("Could not parse filter '" + l_line + "'");
                            return;
                        }
                        string l_colName = l_match.Groups[1].ToString();
                        string l_filter = l_match.Groups[2].ToString();
                        if (m_filters.ContainsKey(l_colName))
                        {
                            // Already contains, add to the list.
                            m_filters[l_colName].Add(l_filter);
                        }
                        else
                        {
                            m_filters[l_colName] = new List<string> { l_filter };
                        }
                    }
                    l_reader.Close();
                }
            }
            private static void loadSubstitutions()
            {

            }
            #endregion Private Methods
        }
    }
}
