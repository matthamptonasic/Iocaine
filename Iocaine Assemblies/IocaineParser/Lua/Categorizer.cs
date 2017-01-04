using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

                This table would be at least 500 columns wide and 13k+ deep. Each value will probably be stored as 32-bits
                which gives a data structure of over 26MB.  That's not the best use of resources, especially considering
                each item will only be using maybe 20 bytes (5 attributes average), not 2k bytes.

                * So let's create a dictionary per filter/attribute and stuff an itemID + value pair in each.
                When this is done, we'll have a list of every filter/attribute and each item + value for that filter/attribute.

                * To make this work with restricted (sub-category) items, we'll need to keep track of un/restricted attributes
                separately so that we can select either.
                The restricted attribute can be the total of any unrestricted + restricted attributes of the same ID.
                This way we do not need to keep another list for the total value.

                We'll also probably want a list of attribute + value for each itemID so that we can look at the categorized item.
                This will make it much easier to check that we're categorizing correctly as well as maybe letting the user
                link to a displayed item by clicking on the attributes.

                To fully understand the dictionary scheme above, we'll need to know every kind of 'value' we're storing.
                Most will be signed short's. Some will be boolean which we can either save as short's or use a union.
                - To know each value type, we can simply do a full parse and dump every match value into a file to view.
                - Print out the filter name as well so we know what we're dealing with.

                => We may be parsing:
                1. signed shorts
                2. boolean (match, but only 1 group)
                3. float (at least in case of PDToTP)
                4. string (most sub-categories, restrictions like "cannot equip...", Teleport, Dispense)
                    - Sub-categories should be able to be broken down further until no strings remain.
                    - The others are not quantifiable, so we may as well NOT capture and display the whole match to the user.
                    - These two should allow us to remove the string match after all parsing is done.

                The information we need to encapsulate is:
                1. Attribute ID - identify which attribute we're specifying a value for.
                2. Item ID - the item being categorized.
                3. The value(s) of the attribute.
                4. Whether this value is restricted or unrestricted (e.g. Salvage, Legion, etc).
                5. Whether this value includes augmented values or not.

                ===============================================================================================================
                We'll also have to figure out how we're going to add in augmentation options.
                Example 1: Adhemar Bonnet (Fixed sets of augments)
                A. AGI+10, DEX+10, Accuracy+15
                B. STR+10, DEX+10, Attack+15
                C. AGI+10, Ranged Accuracy+15, Ranged Attack+15
                D. HP+80, Attack+10, PDT-3%

                This will create 4 different items, each with the same itemID, but having different AugmentIDs.

                Example 2: Herculean Helm
                A. STR+15
                B. DEX+15
                ...
                H. Accuracy+40
                I. Attack+40
                ...
                N. WS Dmg+5%
                O. Critical Hit Rate +5%
                P. Store TP +5
                ...
                HH. "Regen"+4

                Again, each will have the same itemID, but unique AugmentIDs.
            */
            /*
                Thought - Should we make the filters hierarchical so we can sort them in a tree-like structure?
                Or so that the user can select a broader category like all job abilities?
                Seems like a lot of extra work for a little payoff.
            */
            /*
                == User Perspective ==
                The user wants to be able to select and filter at least the following:
                
                - List gear in descending order of values if applicable.
                    1. Any attribute/filter.
                    2. Any job(s).
                    3. Any slot(s).
                    4. With or without restrictions (latent, assault, etc).
                    5. With and/or without flags (rare, exclusive, etc).
                    6. With and/or without augments.
                - Once a filter is selected, list any related filters as well.
                  e.g. HP+ and HPP+

                - Search all items by name with wildcards or regex.
                - Search all items by description with wildcards or regex.
                - Search all filters/categories by name with wildcards or regex.
            */
            #endregion Description

            #region Enums
            public enum ValueType : byte
            {
                UNKNOWN,
                SHORT,
                FLOAT,
                SET
            }
            #endregion Enums

            #region Structs
            private struct SubstitutionPair
            {
                public ushort m_id;
                public string m_old;
                public string m_new;
            }
            public struct AttrValue
            {
                public ValueType m_type;
                public ushort m_attrId;
                public ushort m_itemId;
                public bool m_restricted;
                public ushort m_restrAttrId;
                public bool m_augmented;
                public List<short> m_values;
                public string m_str;
            }
            public struct RestrictedVal
            {
                public ushort m_attrId;
                public ushort m_itemId;
                public string m_str;
            }
            #endregion Structs

            #region Private Members
            // Master Maps & Lists
            private static Dictionary<ushort, string> m_desc;
            private static Dictionary<ushort, Items.ItemInfo> m_items;
            private static Dictionary<string, ushort> m_ids;
            private static List<ushort> m_armorIds;
            private static List<ushort> m_weaponIds;

            // Substitutions
            private const string m_subsFileName = @"Parsing\Gear_Attribute_Substitutions.txt";
            private static List<SubstitutionPair> m_substitutions;
            private static List<SubstitutionPair> m_globalSubs;

            // Filters
            private const string m_filterFileName = @"Parsing\Gear_Attribute_Filters.txt";
            private static Dictionary<string, Dictionary<string, List<string>>> m_subCategories; // <sub-cat <column_name, List<filter_string> > >
            private static List<ValueType> m_attrTypes; // Index is the attribute ID.
            private const string m_noSubCatName = "";
            private const string m_tempFileName = @"Parsing\tempDescription.txt";
            private static Dictionary<ushort, List<RestrictedVal>> m_subAttributes;

            // Attributes (Parsed)
            private static Dictionary<string, ushort> m_attrToId;
            private static Dictionary<ushort, string> m_idToAttr;

            private static bool m_loaded = false;

            // Processing flags
            private static bool m_parseArmor = true;
            private static bool m_parseWeapons = true;

            // Parsing Bookmark & Progress
            private static float m_percentDone = 0f;
            private const string m_bookmarkFileName = @"Parsing\Parsing_Bookmark.txt";
            private static ushort m_startParsingAt = 0;
            private static ushort m_endParsingAt = 10295; // after chocobo shirt
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
                loadBookmark();
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
                    saveBookmark();
                    return;
                }
                saveBookmark();
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
            internal static void DeleteBookmark()
            {
                if (File.Exists(m_bookmarkFileName))
                {
                    File.Delete(m_bookmarkFileName);
                }
                m_startParsingAt = 0;
            }
            internal static string GetAttributeName(ushort iId)
            {
                if (m_idToAttr.ContainsKey(iId))
                {
                    return m_idToAttr[iId];
                }
                return "Error";
            }
            internal static List<RestrictedVal> GetSubAttributes(ushort iItemId)
            {
                if (m_subAttributes.ContainsKey(iItemId))
                {
                    return m_subAttributes[iItemId];
                }
                else
                {
                    return null;
                }
            }
            #endregion Internal Methods

            #region Private Methods
            private static bool loadFilters()
            {
                ushort l_attrId = 0;
                m_idToAttr = new Dictionary<ushort, string>();
                m_attrToId = new Dictionary<string, ushort>();
                m_subCategories = new Dictionary<string, Dictionary<string, List<string>>>();
                m_attrTypes = new List<ValueType>();

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
                            string l_keyName;
                            if (l_subCategory != m_noSubCatName)
                            {
                                l_keyName = l_subCategory + "::" + l_colName;
                            }
                            else
                            {
                                l_keyName = l_colName;
                            }
                            m_idToAttr.Add(l_attrId, l_keyName);
                            m_attrToId.Add(l_keyName, l_attrId);
                            l_attrId++;

                            // Now set the expected data type.
                            // If the filter contains the string (\d+\.?\d*) it is a float (unique case).
                            // Set filters look like '({.*})'.
                            // Some string filters look like:
                            // '([^\.]*)\.'
                            // '(\w+)'
                            // '(\w+day)/(\w+)'
                            // '(.+)'
                            // '([\w ]+)'
                            // Use this search to find these filters: ^[^S].*\([^d\?]+\)
                            // The index of m_attrTypes should follow l_attrId so they match up when we're done.
                            ValueType l_type = ValueType.SHORT;
                            if (l_filter.Contains("(\\d+\\.?\\d*)"))
                            {
                                l_type = ValueType.FLOAT;
                            }
                            else if (l_filter.Contains("({.*?})"))
                            {
                                l_type = ValueType.SET;
                            }
                            m_attrTypes.Add(l_type);
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
                            l_reader.Close();
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
            private static void loadBookmark()
            {
                if (!File.Exists(m_bookmarkFileName))
                {
                    return;
                }
                StreamReader l_reader = new StreamReader(m_bookmarkFileName);
                string l_line = "";
                l_line = l_reader.ReadLine();
                ushort.TryParse(l_line, out m_startParsingAt);
                l_reader.Close();
            }
            private static void saveBookmark()
            {
                StreamWriter l_writer = new StreamWriter(m_bookmarkFileName, false);
                l_writer.WriteLine(m_startParsingAt);
                l_writer.Close();
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
                        string l_itemNameUser = m_items[i_pair.m_id].m_name;
                        foreach (KeyValuePair<string, ushort> i_kvp in m_ids)
                        {
                            if (i_kvp.Value == i_pair.m_id)
                            {
                                l_itemNameUser = i_kvp.Key;
                                break;
                            }
                        }
                        l_msg += "Item: " + l_itemNameUser + " [" + i_pair.m_id + "]\n";
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
                m_subAttributes = new Dictionary<ushort, List<RestrictedVal>>();
                // TBD - remove dump file when finished testing.
                StreamWriter l_writer = new StreamWriter(@"Parsing\dump.txt", false);
                l_writer.AutoFlush = false;

                m_percentDone = 0f;
                List<ushort> l_itemIds;
                getListOfItemsToRun(out l_itemIds);
                float l_totalItems = l_itemIds.Count;

                ushort l_cnt = 0;
                foreach (ushort i_id in l_itemIds)
                {
                    if (i_id == 10293)
                    {
                        l_cnt = l_cnt;
                    }
                    l_cnt++;
                    if (i_id < m_startParsingAt)
                    {
                        continue;
                    }
                    string l_desc = "";
                    if (!m_desc.ContainsKey(i_id))
                    {
                        // TBD - For now do nothing.
                        // Once we have augments to parse, we'll need to do that.
                        continue;
                    }
                    l_desc = m_desc[i_id];
                    // First run any global substitutions on the description.
                    foreach (SubstitutionPair i_sub in m_globalSubs)
                    {
                        l_desc = Regex.Replace(l_desc, i_sub.m_old, i_sub.m_new);
                    }

                    // Run all of the conditional bucketing and save them for later. Do this first.
                    // Stuff them into a dictionary so we can rerun the main filters at the end.
                    // Then run the main filters on everything until clean.
                    // Then run the main filters on the saved results from above. These we'll mark as conditional.
                    if (!runFiltersAll(i_id, ref l_desc))
                    {
                        printItemNotClearMsg(i_id, ref l_desc, l_cnt, l_totalItems);
                        l_writer.Flush();
                        l_writer.Close();
                        return false;
                    }

                    // Go through each of the parsed sub-category filters and check if they have more sub-cat. matches.
                    // Keep parsing until we find no matches.
                    if (m_subAttributes.ContainsKey(i_id))
                    {
                        //List<RestrictedVal> l_rvalList = m_subAttributes[i_id];
                        for(int ii=0; ii < m_subAttributes[i_id].Count; ii++)
                        {
                            string l_str = m_subAttributes[i_id][ii].m_str;
                            while (runFiltersCategory("Sub", i_id, ref l_str))
                            {
                                RestrictedVal l_rval = m_subAttributes[i_id][ii];
                                l_rval.m_str = l_str;
                                m_subAttributes[i_id][ii] = l_rval;
                                continue;
                            }
                        }

                        // Now go back through the sub-category descriptions and run each through the main filters.
                        for (int ii = 0; ii < m_subAttributes[i_id].Count; ii++)
                        {
                            string l_str = m_subAttributes[i_id][ii].m_str;
                            if (!runFiltersMain(i_id, ref l_str, true, m_subAttributes[i_id][ii].m_attrId))
                            {
                                // I believe this means that we couldn't categorize the sub-attribute,
                                // so we should print the error message just like we did when parsing the normal attributes.
                                printItemNotClearMsg(i_id, ref l_str, l_cnt, l_totalItems);
                                l_writer.Flush();
                                l_writer.Close();
                                return false;
                            }
                        }
                    }

                    m_startParsingAt = i_id;
                    if (i_id >= m_endParsingAt)
                    {
                        l_writer.Flush();
                        l_writer.Close();
                        return true;
                    }
                }
                l_writer.Flush();
                l_writer.Close();
                return true;
            }
            private static bool runFiltersAll(ushort iItemId, ref string iDesc)
            {
                // First run through the restriction categories so we filter out those larger sections.
                foreach (string i_subCat in m_subCategories.Keys)
                {
                    if (i_subCat == m_noSubCatName)
                    {
                        continue;
                    }
                    runFiltersCategory(i_subCat, iItemId, ref iDesc);
                    if (checkOnlyWhitespace(ref iDesc))
                    {
                        return true;
                    }
                }
                if (!runFiltersMain(iItemId, ref iDesc, false))
                {
                    return false;
                }
                return true;
            }
            private static bool runFiltersCategory(string iSubCat, ushort iItemId, ref string iDesc)
            {
                bool l_retVal = false; // Returns true if we match a filter and parse it sucessfully.
                foreach (string i_column in m_subCategories[iSubCat].Keys)
                {
                    List<string> l_filterList = m_subCategories[iSubCat][i_column];
                    foreach (string i_filter in l_filterList)
                    {
                        Regex l_regex = new Regex(i_filter);
                        Match l_match = l_regex.Match(iDesc);
                        if (l_match.Success)
                        {
                            ushort l_attrId = 0;
                            string l_attrKey = iSubCat + "::" + i_column;
                            if (!m_attrToId.ContainsKey(l_attrKey))
                            {
                                MessageBox.Show("Error looking up attribute ID of " + l_attrKey + ".");
                                return false;
                            }
                            l_attrId = m_attrToId[l_attrKey];

                            // We matched something. Save the string (if captured) and move on.
                            RestrictedVal l_val = new RestrictedVal();
                            l_val.m_attrId = l_attrId;
                            l_val.m_itemId = iItemId;
                            l_val.m_str = "";
                            if (l_match.Groups.Count > 1)
                            {
                                l_val.m_str = l_match.Groups[1].ToString();
                            }
                            if (!m_subAttributes.ContainsKey(iItemId))
                            {
                                m_subAttributes.Add(iItemId, new List<RestrictedVal> { l_val });
                            }
                            else
                            {
                                m_subAttributes[iItemId].Add(l_val);
                            }

                            // Replace the whole match with ""
                            iDesc = l_regex.Replace(iDesc, "");
                            l_retVal = true;
                        }

                        if (Regex.IsMatch(iDesc, @"^\s*$"))
                        {
                            return l_retVal;
                        }
                    }
                }
                return l_retVal;
            }
            private static bool runFiltersMain(ushort iItemId, ref string iDesc, bool iRestricted, ushort iRestrAttrId = 0)
            {
                // Category here is assumed to be unrestricted (m_noSubCatName)
                // This is where we run through each of the main filters and bucket whatever we find.
                foreach (string i_column in m_subCategories[m_noSubCatName].Keys)
                {
                    List<string> l_filterList = m_subCategories[m_noSubCatName][i_column];
                    foreach (string i_filter in l_filterList)
                    {
                        Regex l_regex = new Regex(i_filter);
                        Match l_match = l_regex.Match(iDesc);
                        if (l_match.Success)
                        {
                            // This is the main list of filters (no sub-category/restriction).
                            // We only set values based on these filters.
                            ushort l_attrId = 0;
                            if (!m_attrToId.ContainsKey(i_column))
                            {
                                MessageBox.Show("Error looking up attribute ID of " + i_column + ".");
                                return false;
                            }
                            l_attrId = m_attrToId[i_column];

                            ValueType l_type = m_attrTypes[l_attrId];
                            AttrValue l_value = new AttrValue();
                            l_value.m_type = l_type;
                            l_value.m_attrId = l_attrId;
                            l_value.m_augmented = false;
                            l_value.m_restricted = iRestricted;
                            l_value.m_restrAttrId = iRestrAttrId;
                            l_value.m_itemId = iItemId;
                            if (l_match.Groups.Count > 1)
                            {
                                l_value.m_values = new List<short>();
                                l_value.m_str = null;

                                // Set attribute values for each capture above index 0.
                                for (int ii = 1; ii < l_match.Groups.Count; ii++)
                                {
                                    if (l_match.Groups[ii].ToString() == "")
                                    {
                                        continue;
                                    }
                                    short l_val;
                                    if (l_type == ValueType.SHORT)
                                    {
                                        if (!short.TryParse(l_match.Groups[ii].ToString(), out l_val))
                                        {
                                            MessageBox.Show("Error parsing '" + l_match.Groups[ii] + "' into a short.\n" + i_filter + "\n" + iDesc);
                                            continue;
                                        }
                                    }
                                    else if (l_type == ValueType.FLOAT)
                                    {
                                        float l_floatVal;
                                        if (!float.TryParse(l_match.Groups[ii].ToString(), out l_floatVal))
                                        {
                                            MessageBox.Show("Error parsing '" + l_match.Groups[ii] + "' into a float.");
                                            continue;
                                        }
                                        l_val = (short)(l_floatVal * 10f);
                                    }
                                    else if (l_type == ValueType.SET)
                                    {
                                        // TBD - parse the set values and push them into their own values.
                                        l_val = 0;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Cannot parse a value of unknown type from '" + l_match.Groups[ii] + "'");
                                        continue;
                                    }
                                    l_value.m_values.Add(l_val);
                                }
                            }
                            Items.ItemInfo l_info = m_items[iItemId];
                            if (m_items[iItemId].m_attributes == null)
                            {
                                l_info.m_attributes = new List<AttrValue>();
                            }
                            l_info.m_attributes.Add(l_value);
                            m_items[iItemId] = l_info;

                            // Replace the whole match with ""
                            iDesc = l_regex.Replace(iDesc, "");
                        }

                        if (checkOnlyWhitespace(ref iDesc))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            private static void getListOfItemsToRun(out List<ushort> oIds)
            {
                oIds = new List<ushort>();
                if (m_parseArmor)
                {
                    oIds.AddRange(m_armorIds);
                }
                if (m_parseWeapons)
                {
                    oIds.AddRange(m_weaponIds);
                }
                oIds.Sort();
            }
            private static void printItemNotClearMsg(ushort iId, ref string iDesc, int iCount, float iTotalItems)
            {
                string l_tmp = m_desc[iId];
                string l_msg = "Item not clear after all filters.\n";
                l_msg += "Item[" + iId + "]: " + m_items[iId].m_name + "\n";
                l_msg += "Orig. Desc: '" + m_desc[iId] + "'\n";
                l_msg += "Desc: '" + iDesc + "'\n";
                l_msg += m_items[iId].m_name + "::" + iDesc.Trim() + "=>\n\n";
                l_msg += "Description is " + l_tmp.Length + " characters long. UTF-16 codes:\n\n";
                for (int ii = 0; ii < l_tmp.Length; ii++)
                {
                    l_msg += ((ushort)l_tmp[ii]).ToString("X4") + " ";
                    if (((ii + 1) % 8) == 0)
                    {
                        l_msg += "\n";
                    }
                }
                m_percentDone = iCount / iTotalItems * 100;
                l_msg += "\n\nPercent Complete: " + m_percentDone.ToString("f2") + "%";
                DialogResult l_dialRslt = MessageBox.Show(l_msg + "\n\nOpen this in text editor?", "Item Description Parser Miss", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (l_dialRslt == DialogResult.Yes)
                {
                    StreamWriter l_ostream = new StreamWriter(m_tempFileName, false);
                    l_ostream.Write(l_msg);
                    l_ostream.Close();
                    Process.Start(m_tempFileName);
                }
            }
            private static bool checkOnlyWhitespace(ref string iText)
            {
                if (Regex.IsMatch(iText, @"^\s*$"))
                {
                    return true;
                }
                return false;
            }
            #endregion Private Methods
        }
    }
}
