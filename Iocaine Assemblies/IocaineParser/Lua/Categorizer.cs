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
            //private static Dictionary<string, List<string>> m_filters; // <column_name, List<filter_strings> >
            private const string m_subsFileName = @"Parsing\Gear_Attribute_Substitutions.txt";
            private static List<SubstitutionPair> m_substitutions;
            private static List<SubstitutionPair> m_globalSubs;
            private const string m_tempFileName = @"Parsing\tempDescription.txt";
            private static Dictionary<string, Dictionary<string, List<string>>> m_subCategories; // <sub-cat <column_name, List<filter_string> > >
            private const string m_noSubCatName = "";

            private static bool m_loaded = false;

            private static bool m_parseArmor = true;
            private static bool m_parseWeapons = true;
            #endregion Private Members

            #region Public Properties
            #endregion Public Properties

            #region Inits
            #endregion Inits

            #region Internal Methods
            public static void Load()
            {
                if (m_loaded)
                {
                    ItemDescriptions.Reparse();
                }
                m_loaded = true;
                if (!loadFilters())
                {
                    return;
                }
                if (!loadSubstitutions())
                {
                    return;
                }
                // Run through each substitution, replacing the description in place.
                if (!runSubstitutions())
                {
                    return;
                }
                // Start bucketing the descriptions.
                // On each item, run through the filters, replacing the matching text with " ", until the entire line is whitespace.
                // If we find an item that still has text after the entire filter list is exhausted, notify the user of the remaining text.
                // Manually update the filters and restart.
                if (!runCategorization())
                {
                    return;
                }
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
            private static bool loadFilters()
            {
                //m_filters = new Dictionary<string, List<string>>();
                m_subCategories = new Dictionary<string, Dictionary<string, List<string>>>();
                if (File.Exists(m_filterFileName))
                {
                    StreamReader l_reader = new StreamReader(m_filterFileName, UTF8Encoding.UTF8);
                    string l_line = "";
                    string l_subCategory = m_noSubCatName;
                    while (!l_reader.EndOfStream)
                    {
                        l_line = l_reader.ReadLine();
                        if (isCommented(l_line))
                        {
                            continue;
                        }

                        string l_pattern = "([^=]*)=(.*)$";
                        Regex l_regex = new Regex(l_pattern);
                        Match l_match = l_regex.Match(l_line);
                        if (l_match.Groups.Count != 3)
                        {
                            MessageBox.Show("Could not parse filter '" + l_line + "'");
                            l_reader.Close();
                            return false;
                        }

                        string l_colName = l_match.Groups[1].ToString();

                        // Check for prefix
                        l_pattern = "(.*)::(.*)";
                        l_regex = new Regex(l_pattern);
                        Match l_prefixMatch = l_regex.Match(l_colName);
                        if (l_prefixMatch.Groups.Count == 3)
                        {
                            // Add any new prefixes here. So far, only subcategories like "Latent" or "Salvage".
                            l_subCategory = l_prefixMatch.Groups[1].ToString();
                            l_colName = l_prefixMatch.Groups[2].ToString();
                        }

                        string l_filter = l_match.Groups[2].ToString();
                        if (!m_subCategories.ContainsKey(l_subCategory))
                        {
                            m_subCategories[l_subCategory] = new Dictionary<string, List<string>>();
                        }
                        if (m_subCategories[l_subCategory].ContainsKey(l_colName))
                        {
                            // Already contains, add to the list.
                            m_subCategories[l_subCategory][l_colName].Add(l_filter);
                        }
                        else
                        {
                            m_subCategories[l_subCategory][l_colName] = new List<string> { l_filter };
                        }
                        l_subCategory = m_noSubCatName;
                    }
                    l_reader.Close();
                    return true;
                }
                return false;
            }
            private static bool loadSubstitutions()
            {
                m_substitutions = new List<SubstitutionPair>();
                m_globalSubs = new List<SubstitutionPair>();
                if (File.Exists(m_subsFileName))
                {
                    StreamReader l_reader = new StreamReader(m_subsFileName, UTF8Encoding.UTF8);
                    string l_line = "";
                    while (!l_reader.EndOfStream)
                    {
                        l_line = l_reader.ReadLine();
                        if (isCommented(l_line))
                        {
                            continue;
                        }
                        string l_pattern = "(.*)::(.*)=>(.*)$";
                        Regex l_regex = new Regex(l_pattern);
                        Match l_match = l_regex.Match(l_line);
                        if (l_match.Groups.Count != 4)
                        {
                            MessageBox.Show("Could not parse substitution '" + l_line + "'");
                            return false;
                        }
                        SubstitutionPair l_pair = new SubstitutionPair();
                        string l_itemName = l_match.Groups[1].ToString();
                        if (l_itemName == "*")
                        {
                            // Add to global substitutions
                            l_pair.m_id = 0xffff;
                            l_pair.m_old = l_match.Groups[2].ToString();
                            l_pair.m_new = l_match.Groups[3].ToString();
                            m_globalSubs.Add(l_pair);
                        }
                        else
                        {
                            if (!m_ids.ContainsKey(l_itemName))
                            {
                                MessageBox.Show("Could not find item '" + l_itemName + "' in the database.");
                                l_reader.Close();
                                return false;
                            }
                            l_pair.m_id = m_ids[l_itemName];
                            l_pair.m_old = l_match.Groups[2].ToString();
                            l_pair.m_new = l_match.Groups[3].ToString();
                            m_substitutions.Add(l_pair);
                        }
                    }
                    l_reader.Close();

                    //string msg = "";
                    //foreach (SubstitutionPair i_pair in m_substitutions)
                    //{
                    //    msg += i_pair.m_id + " === " + i_pair.m_old + " === " + i_pair.m_new + "\n";
                    //}
                    //MessageBox.Show(msg);
                    return true;
                }
                return false;
            }
            private static bool runSubstitutions()
            {
                foreach (SubstitutionPair i_pair in m_substitutions)
                {
                    Regex l_regex = new Regex(i_pair.m_old);
                    string l_desc = m_desc[i_pair.m_id];
                    if (!l_regex.IsMatch(l_desc))
                    {
                        string l_msg = "No match when running substitutions.\n";
                        l_msg += "Item: " + m_items[i_pair.m_id].m_name + "\n";
                        l_msg += "Desc: " + l_desc + "\n";
                        l_msg += "Pattern: " + i_pair.m_old + "\n";
                        l_msg += "Subst.:  " + i_pair.m_new;
                        MessageBox.Show(l_msg);
                        continue;
                    }
                    l_desc = l_regex.Replace(l_desc, i_pair.m_new);
                    m_desc[i_pair.m_id] = l_desc;
                }

                return true;
            }
            private static bool runCategorization()
            {
                List<ushort> l_itemIds = new List<ushort>();
                if (m_parseArmor)
                {
                    l_itemIds.AddRange(m_armorIds);
                }
                if (m_parseWeapons)
                {
                    l_itemIds.AddRange(m_weaponIds);
                }

                bool l_itemClear = false;
                foreach (ushort i_id in l_itemIds)
                {
                    l_itemClear = false;
                    string l_desc = m_desc[i_id];
                    // First run any global substitutions on the description.
                    foreach (SubstitutionPair i_sub in m_globalSubs)
                    {
                        l_desc = Regex.Replace(l_desc, i_sub.m_old, i_sub.m_new);
                    }
                    foreach (string i_subCat in m_subCategories.Keys)
                    {
                        foreach (string column in m_subCategories[i_subCat].Keys)
                        {
                            List<string> l_filterList = m_subCategories[i_subCat][column];
                            foreach (string filter in l_filterList)
                            {
                                Regex l_regex = new Regex(filter);
                                Match l_match = l_regex.Match(l_desc);
                                if (l_match.Groups.Count > 1)
                                {
                                    int l_val = 0;
                                    if (int.TryParse(l_match.Groups[1].ToString(), out l_val))
                                    {
                                        // Assign value in table based on column.
                                        // TBD
                                    }
                                }
                                if (l_match.Groups.Count > 0)
                                {
                                    // Replace that match with ""
                                    l_desc = l_regex.Replace(l_desc, "");
                                }

                                if (Regex.IsMatch(l_desc, @"^\s*$"))
                                {
                                    l_itemClear = true;
                                }
                            }
                            if (l_itemClear)
                            {
                                break;
                            }
                        }
                    }
                    if (!l_itemClear)
                    {
                        string l_tmp = m_desc[i_id];
                        string l_msg = "Item not clear after all filters.\n";
                        l_msg += "Item[" + i_id + "]: " + m_items[i_id].m_name + "\n";
                        l_msg += "Orig. Desc: '" + m_desc[i_id] + "'\n";
                        l_msg += "Desc: '" + l_desc + "'\n\n";
                        l_msg += "Description is " + l_tmp.Length + " characters long. UTF-16 codes:\n\n";
                        for (int ii = 0; ii < l_tmp.Length; ii++)
                        {
                            l_msg += ((ushort)l_tmp[ii]).ToString("X4") + " ";
                            if (((ii + 1) % 8) == 0)
                            {
                                l_msg += "\n";
                            }
                        }
                        DialogResult l_dialRslt = MessageBox.Show(l_msg + "\n\nOpen this in text editor?", "Item Description Parser Miss", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                        if (l_dialRslt == DialogResult.Yes)
                        {
                            StreamWriter l_ostream = new StreamWriter(m_tempFileName, false);
                            l_ostream.Write(l_msg);
                            l_ostream.Close();
                            Process.Start(m_tempFileName);
                        }
                        return false;
                    }
                }

                return true;
            }
            #endregion Private Methods
        }
    }
}
