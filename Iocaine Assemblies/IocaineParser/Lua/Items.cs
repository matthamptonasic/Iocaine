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
        public static class Items
        {
            #region Structs
            internal struct ItemInfo
            {
                public ushort m_id;
                public byte m_type;
                public byte m_level;
                public ushort m_item_level;
                public uint m_jobs;
                public uint m_flags;
                public string m_name;
                public List<Categorizer.AttrValue> m_attributes;
            }
            #endregion Structs

            #region Private Members
            private const string m_fileName = "items.lua";
            private static string m_filePath = "";
            private static bool m_parsed = false;
            private const string m_specialFileName = @"Parsing\Gear_Special_Case_Names.txt";
            private static Dictionary<ushort, ItemInfo> m_items;
            private static Dictionary<string, ushort> m_ids;
            private static List<ushort> m_armorIds;
            private static List<ushort> m_weaponIds;
            private static Dictionary<ushort, string> m_desc; //References the one in ItemDescriptions.
            private static Dictionary<string, List<ItemInfo>> m_postProcessingItems;
            private static Dictionary<ushort, string> m_specialCases;
            #endregion Private Members

            #region Public Properties
            public static int Count
            {
                get
                {
                    if (!m_parsed)
                    {
                        parse();
                    }
                    if (m_items == null)
                    {
                        return 0;
                    }
                    return m_items.Count();
                }
            }
            public static int ArmorCount
            {
                get
                {
                    if (!m_parsed)
                    {
                        parse();
                    }
                    if (m_armorIds == null)
                    {
                        return 0;
                    }
                    return m_armorIds.Count();
                }
            }
            public static int WeaponsCount
            {
                get
                {
                    if (!m_parsed)
                    {
                        parse();
                    }
                    if (m_weaponIds == null)
                    {
                        return 0;
                    }
                    return m_weaponIds.Count();
                }
            }
            #endregion Public Properties

            #region Inits
            internal static bool Init_Process(string iFilePath, bool iWipeClean = false)
            {
                if (iWipeClean)
                {
                    m_parsed = false;
                }
                m_filePath = Path.Combine(iFilePath, m_fileName);
                loadSpecialCases();
                parse();
                return true;
            }
            #endregion Inits

            #region Internal Methods
            internal static void GetItemsCollection(out Dictionary<ushort, ItemInfo> oItems)
            {
                if (!m_parsed)
                {
                    parse();
                }
                oItems = m_items;
            }
            internal static void GetArmorList(out List<ushort> oArmor)
            {
                if (!m_parsed)
                {
                    parse();
                }
                oArmor = m_armorIds;
            }
            internal static void GetWeaponsList(out List<ushort> oWeapons)
            {
                if (!m_parsed)
                {
                    parse();
                }
                oWeapons = m_weaponIds;
            }
            internal static void PostProcess()
            {
                ItemDescriptions.GetItemDescriptions(out m_desc);
                string l_newName;
                List<string> l_allAdded = new List<string>();
                foreach (string i_name in m_postProcessingItems.Keys)
                {
                    // Go through each item name (list of ItemInfo's) and determine if
                    // we should create a single entry for the lowest item ID,
                    // or create different names for each ID AND one that is not unique that points to the lowest ID.
                    ushort l_lowId = 0xffff;
                    List<ItemInfo> l_temp = m_postProcessingItems[i_name];
                    foreach (ItemInfo i_info in m_postProcessingItems[i_name])
                    {
                        // 1. If the level or item_level's are different, append the level to name. e.g. Aegis(80).
                        // 2. If the level and item_level's are the same, but the description contains 'Afterglow', append 'Afterglow'.
                        //    This means we need to postpone all of this processing until the description parsing phase.
                        // 3. If 1 and 2 are not satisfied, append the item ID to both/all items, 
                        //    but also create an entry with no suffix that points to the first item.
                        l_newName = i_name;
                        bool l_sameLevel = false;
                        bool l_hasAfterglow = false;
                        bool l_otherHasAfterglow = false;
                        if (i_info.m_id < l_lowId)
                        {
                            l_lowId = i_info.m_id;
                        }
                        l_hasAfterglow = m_desc.ContainsKey(i_info.m_id) && m_desc[i_info.m_id].Contains("Afterglow");
                        foreach (ItemInfo i_infoCompare in m_postProcessingItems[i_name])
                        {
                            if (i_infoCompare.m_id == i_info.m_id)
                            {
                                continue;
                            }
                            if ((i_info.m_item_level > 0) || (i_infoCompare.m_item_level > 0))
                            {
                                // Compare item levels, not levels.
                                l_sameLevel = (i_info.m_item_level == i_infoCompare.m_item_level);
                            }
                            else if ((i_info.m_level > 0) && (i_infoCompare.m_level > 0))
                            {
                                // Compare levels.
                                l_sameLevel = (i_info.m_level == i_infoCompare.m_level);
                            }
                            else if ((i_info.m_level == 0) && (i_infoCompare.m_level == 0))
                            {
                                // No levels for this item. Call them the same which will cause us to make no change to the name.
                                l_sameLevel = true;
                            }
                            if (l_sameLevel)
                            {
                                l_otherHasAfterglow = m_desc.ContainsKey(i_infoCompare.m_id) && m_desc[i_infoCompare.m_id].Contains("Afterglow");
                                break;
                            }
                        }
                        if (!l_sameLevel || l_hasAfterglow || l_otherHasAfterglow)
                        {
                            // Add level to name.
                            l_newName += "(";
                            if (i_info.m_item_level > 0)
                            {
                                l_newName += i_info.m_item_level;
                            }
                            else
                            {
                                l_newName += i_info.m_level;
                            }
                            l_newName += ")";
                        }
                        if (l_hasAfterglow)
                        {
                            // Add 'Afterglow' to name.
                            l_newName += "Afterglow";
                        }
                        if (l_newName == i_name)
                        {
                            // We made no changes which means the item is ambiguous. We'll only add a single entry outside of the loop.
                            // But we still need to check the other items just in case, so don't break, just continue.
                            continue;
                        }

                        if (m_ids.ContainsKey(l_newName))
                        {
                            // If we get to this point and we're still not unique, we may be on the last stage of REM weapon upgrades.
                            // Both are 119 and have afterglow. To tell the difference at this point, we'll have to look at the damage.
                            // The item with the higher damage will have " Final" added to the name.
                            // At the time of writing this, these weapons are the only case where this is being hit.
                            ushort l_dmgFirst = parseDmg(m_ids[l_newName]);
                            ushort l_dmgCurr = parseDmg(i_info.m_id);
                            if ((l_dmgFirst == 0) || (l_dmgCurr == 0))
                            {
                                MessageBox.Show("Could not figure out what to do with item '" + l_newName + "' (" + i_info.m_id + ")");
                                continue;
                            }
                            if (l_dmgCurr >= l_dmgFirst)
                            {
                                l_newName += " Final";
                                if (m_ids.ContainsKey(l_newName))
                                {
                                    MessageBox.Show("We're really hosed with item '" + l_newName + "' (" + i_info.m_id + ")");
                                }
                                else
                                {
                                    m_ids.Add(l_newName, i_info.m_id);
                                    l_allAdded.Add(l_newName + "[" + i_info.m_id + "]");
                                }
                            }
                            else
                            {
                                // We have to change the key of the previous item to have the " Final" suffix.
                                // To do this we add a key with the new suffix pointing to the old item ID.
                                // Then we go change the previous key's value to be the new item ID.
                                string l_oldName = l_newName;
                                l_newName += " Final";
                                if (m_ids.ContainsKey(l_newName))
                                {
                                    MessageBox.Show("We're really hosed with item '" + l_newName + "' (" + i_info.m_id + ") when swapping with " + m_ids[l_oldName] + ".");
                                }
                                else
                                {
                                    m_ids.Add(l_newName, m_ids[l_oldName]);
                                    m_ids[l_oldName] = i_info.m_id;
                                    l_allAdded.Add(l_newName + "[" + i_info.m_id + "]");
                                }
                            }
                        }
                        else
                        {
                            m_ids.Add(l_newName, i_info.m_id);
                            l_allAdded.Add(l_newName + "[" + i_info.m_id + "]");
                        }
                    }
                    // Add 1 entry with the common name.
                    if (!m_ids.ContainsKey(i_name))
                    {
                        m_ids.Add(i_name, l_lowId);
                        l_allAdded.Add(i_name + "[" + l_lowId + "]");
                    }
                    else
                    {
                        MessageBox.Show("The item '" + i_name + "' was already in the m_ids map.");
                    }
                }

                //string l_allNewNames = "";
                //foreach (string i_str in l_allAdded)
                //{
                //    l_allNewNames += "'" + i_str + "'\n";
                //}
                //LoggingFunctions.Timestamp(l_allNewNames);
                //Process.Start(LoggingFunctions.Name);
            }
            #endregion Internal Methods

            #region Private Methods
            private static void parse()
            {
                if (m_parsed)
                {
                    return;
                }
                m_items = new Dictionary<ushort, ItemInfo>();
                m_ids = new Dictionary<string, ushort>();
                m_armorIds = new List<ushort>();
                m_weaponIds = new List<ushort>();
                m_postProcessingItems = new Dictionary<string, List<ItemInfo>>();

                List<ushort> l_idsToRemove = new List<ushort>();

                ItemInfo l_info;
                try
                {
                    if (!File.Exists(m_filePath))
                    {
                        return;
                    }
                    StreamReader l_reader = new StreamReader(m_filePath, UTF8Encoding.UTF8);
                    string l_line = "";
                    // The m_ids map is so that we can get the item ID from the user's statement of the item name.
                    // Some items have the same name for multiple ID's.
                    // Some of these are ambiguous to us. For instance, Dnc. Casaque +1 has a male and female version.
                    // Since we only care about the difference in certain fields, we may ignore this and only point
                    // to the first item.
                    // This will be the case when:
                    // 1. The names are the same.
                    // 2. The level's are the same (if applicable).
                    // 3. The item_level's are the same (if applicable).
                    // 4. The item descriptions do not contain the word 'Afterglow'.

                    // Others have differences that we need to categorize.
                    // 1. If the level or item_level's are different, append the level to name. e.g. Aegis(80).
                    // 2. If the level and item_level's are the same, but the description contains 'Afterglow', append 'Afterglow'.
                    //    This means we need to postpone all of this processing until the description parsing phase.
                    // 3. If 1 and 2 are not satisfied, append the item ID to both/all items, 
                    //    but also create an entry with no suffix that points to the first item.

                    // Postprocessing means we will:
                    // When we find a duplicate, we put the new ItemInfo into a list for later.
                    // When we get done with all items, we also go back and remove the original id from the m_ids
                    // and push the corresponding ItemInfo into the same list as above.
                    //
                    // Then when we finish processing all of the items, and all of the ItemDescriptions are done,
                    // we'll go back through our list and create the necessary entries.
                    while (!l_reader.EndOfStream)
                    {
                        l_line = l_reader.ReadLine();
                        if (processLine(ref l_line, out l_info))
                        {
                            replaceCharacters(ref l_info.m_name);
                            if (m_specialCases.ContainsKey(l_info.m_id))
                            {
                                l_info.m_name = m_specialCases[l_info.m_id];
                            }
                            m_items.Add(l_info.m_id, l_info);
                            if (!m_ids.ContainsKey(l_info.m_name))
                            {
                                m_ids.Add(l_info.m_name, l_info.m_id);
                            }
                            else
                            {
                                // Push the new item into the post processing list.
                                pushPostProcessItem(l_info);
                                // If the original item is not already scheduled to be removed, do it now.
                                if (!l_idsToRemove.Contains(m_ids[l_info.m_name]))
                                {
                                    l_idsToRemove.Add(m_ids[l_info.m_name]);
                                }
                            }
                            if (l_info.m_type == (byte)Things.ITEM_TYPE.ARMOR)
                            {
                                m_armorIds.Add(l_info.m_id);
                            }
                            else if (l_info.m_type == (byte)Things.ITEM_TYPE.WEAPON)
                            {
                                m_weaponIds.Add(l_info.m_id);
                            }
                        }
                    }
                    l_reader.Close();
                    foreach (ushort i_id in l_idsToRemove)
                    {
                        pushPostProcessItem(m_items[i_id]);
                        m_ids.Remove(m_items[i_id].m_name);
                    }
                    int cnt = m_postProcessingItems.Count;
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error(e.ToString());
                }

                Categorizer.SetItems(ref m_items);
                Categorizer.SetIds(ref m_ids);
                Categorizer.SetArmorIds(ref m_armorIds);
                Categorizer.SetWeaponIds(ref m_weaponIds);
                m_parsed = true;
            }
            private static bool processLine(ref string iLine, out ItemInfo oInfo)
            {
                oInfo = new ItemInfo();
                oInfo.m_attributes = null;

                string l_patternNum = "=([^,}]*)[,}]";
                string l_patternStr = "=\"([^\"\\\\]*(?:\\\\.[^\"\\\\]*)*)";  //"=\"([^\"]*)\"";
                Regex l_idRegex = new Regex("id" + l_patternNum);
                Match l_idMatch = l_idRegex.Match(iLine);
                if (l_idMatch.Groups.Count != 2)
                {
                    return false;
                }
                if (!ushort.TryParse(l_idMatch.Groups[1].ToString(), out oInfo.m_id))
                {
                    LoggingFunctions.Error("Malformed 'id' when parsing line '" + iLine + "'");
                    return false;
                }
                if (oInfo.m_id == Things.invalidID)
                {
                    return false;
                }
                Regex l_nameRegex = new Regex("en" + l_patternStr);
                Match l_nameMatch = l_nameRegex.Match(iLine);
                if (l_nameMatch.Success)
                {
                    oInfo.m_name = l_nameMatch.Groups[1].ToString();
                }
                else
                {
                    return false;
                }

                Regex l_typeRegex = new Regex("type" + l_patternNum);
                Match l_typeMatch = l_typeRegex.Match(iLine);
                if (l_typeMatch.Success)
                {
                    if (!byte.TryParse(l_typeMatch.Groups[1].ToString(), out oInfo.m_type))
                    {
                        LoggingFunctions.Error("Malformed 'type' when parsing item '" + oInfo.m_name + "'");
                        return false;
                    }
                }
                else
                {
                    oInfo.m_type = 0xff;
                }

                Regex l_levelRegex = new Regex("[^_]level" + l_patternNum);
                Match l_levelMatch = l_levelRegex.Match(iLine);
                if (l_levelMatch.Success)
                {
                    if (!byte.TryParse(l_levelMatch.Groups[1].ToString(), out oInfo.m_level))
                    {
                        LoggingFunctions.Error("Malformed 'level' when parsing item '" + oInfo.m_name + "'");
                        return false;
                    }
                }
                else
                {
                    oInfo.m_level = 0;
                }

                Regex l_item_levelRegex = new Regex("item_level" + l_patternNum);
                Match l_item_levelMatch = l_item_levelRegex.Match(iLine);
                if (l_item_levelMatch.Success)
                {
                    if (!ushort.TryParse(l_item_levelMatch.Groups[1].ToString(), out oInfo.m_item_level))
                    {
                        LoggingFunctions.Error("Malformed 'item_level' when parsing item '" + oInfo.m_name + "'");
                        return false;
                    }
                }
                else
                {
                    oInfo.m_item_level = 0;
                }

                Regex l_jobsRegex = new Regex("jobs" + l_patternNum);
                Match l_jobsMatch = l_jobsRegex.Match(iLine);
                if (l_jobsMatch.Success)
                {
                    if (!uint.TryParse(l_jobsMatch.Groups[1].ToString(), out oInfo.m_jobs))
                    {
                        LoggingFunctions.Error("Malformed 'jobs' when parsing item '" + oInfo.m_name + "'");
                        return false;
                    }
                }
                else
                {
                    oInfo.m_jobs = 0;
                }

                Regex l_flagsRegex = new Regex("flags" + l_patternNum);
                Match l_flagsMatch = l_flagsRegex.Match(iLine);
                if (l_flagsMatch.Success)
                {
                    if (!uint.TryParse(l_flagsMatch.Groups[1].ToString(), out oInfo.m_flags))
                    {
                        LoggingFunctions.Error("Malformed 'flags' when parsing item '" + oInfo.m_name + "'");
                        return false;
                    }
                }
                else
                {
                    oInfo.m_flags = 0;
                }

                return true;
            }
            private static void pushPostProcessItem(ItemInfo iInfo)
            {
                if (!m_postProcessingItems.ContainsKey(iInfo.m_name))
                {
                    m_postProcessingItems.Add(iInfo.m_name, new List<ItemInfo>());
                }
                m_postProcessingItems[iInfo.m_name].Add(iInfo);
            }
            private static ushort parseDmg(ushort iItemId)
            {
                string l_desc = "";
                string l_dmgPattern = @"DMG:?\+?(\d+)"; // : is optional due to a typo on item 20930.
                string l_dmgStr = "";
                ushort l_retVal = 0;
                if (m_desc.ContainsKey(iItemId))
                {
                    l_desc = m_desc[iItemId];
                    if (Regex.IsMatch(l_desc, l_dmgPattern))
                    {
                        l_dmgStr = Regex.Match(l_desc, l_dmgPattern).Groups[1].ToString();
                        ushort.TryParse(l_dmgStr, out l_retVal);
                    }
                }
                return l_retVal;
            }
            private static void loadSpecialCases()
            {
                m_specialCases = new Dictionary<ushort, string>();
                if (File.Exists(m_specialFileName))
                {
                    StreamReader l_reader = new StreamReader(m_specialFileName, UTF8Encoding.UTF8);
                    string l_line = "";
                    while (!l_reader.EndOfStream)
                    {
                        l_line = l_reader.ReadLine();
                        if (isCommented(l_line))
                        {
                            continue;
                        }
                        string l_pattern = "(.*)=(.*)";
                        Regex l_regex = new Regex(l_pattern);
                        Match l_match = l_regex.Match(l_line);
                        if (l_match.Groups.Count != 3)
                        {
                            MessageBox.Show("Could not parse special item '" + l_line + "'");
                            return;
                        }
                        ushort l_id;
                        string l_itemName = l_match.Groups[2].ToString();
                        if (!ushort.TryParse(l_match.Groups[1].ToString(), out l_id))
                        {
                            MessageBox.Show("Could not parse special item id from line '" + l_line + "'");
                            return;
                        }
                        else
                        {
                            if (!m_specialCases.ContainsKey(l_id))
                            {
                                m_specialCases.Add(l_id, l_itemName);
                            }
                        }
                    }
                    l_reader.Close();
                }
                return;
            }
            #endregion Private Methods
        }
    }
}
