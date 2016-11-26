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
                public uint m_jobs;
                public uint m_flags;
                public string m_name;
            }
            #endregion Structs

            #region Private Members
            private const string m_fileName = "items.lua";
            private static string m_filePath = "";
            private static bool m_parsed = false;
            private static Dictionary<ushort, ItemInfo> m_items;
            private static List<ushort> m_armorIds;
            private static List<ushort> m_weaponIds;
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
            internal static bool Init_Process(string iFilePath)
            {
                m_filePath = Path.Combine(iFilePath, m_fileName);
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
            #endregion Internal Methods

            #region Private Methods
            private static void parse()
            {
                if (m_parsed)
                {
                    return;
                }
                // TBD - Add status message to user.
                m_items = new Dictionary<ushort, ItemInfo>();
                m_armorIds = new List<ushort>();
                m_weaponIds = new List<ushort>();

                if (!File.Exists(m_filePath))
                {
                    return;
                }
                StreamReader l_reader = new StreamReader(m_filePath, UTF8Encoding.UTF8);
                string l_line = "";
                while (!l_reader.EndOfStream)
                {
                    l_line = l_reader.ReadLine();
                    ItemInfo l_info;
                    if (processLine(ref l_line, out l_info))
                    {
                        replaceCharacters(ref l_info.m_name);
                        m_items.Add(l_info.m_id, l_info);
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

                m_parsed = true;
            }
            private static bool processLine(ref string iLine, out ItemInfo oInfo)
            {
                oInfo = new ItemInfo();

                String l_patternNum = "=([^,}]*)[,}]";
                String l_patternStr = "=\"([^\"]*)\"";
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
            #endregion Private Methods
        }
    }
}
