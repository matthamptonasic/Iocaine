using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Parsing
{
    public static partial class Lua
    {
        public static class ItemDescriptions
        {
            #region Private Members
            private const string m_fileName = "item_descriptions.lua";
            private static string m_filePath = "";
            private static bool m_parsed = false;
            private static Dictionary<ushort, string> m_desc;
            #endregion Private Members

            #region Public Properties
            #endregion Public Properties

            #region Inits
            internal static bool Init_Process(string iFilePath)
            {
                m_filePath = Path.Combine(iFilePath, m_fileName);
                parse();
                return true;
            }
            #endregion Inits

            #region Internal Methods
            internal static void GetItemDescriptions(out Dictionary<ushort, string> oDesc)
            {
                if (!m_parsed)
                {
                    parse();
                }
                oDesc = m_desc;
            }
            #endregion Internal Methods

            #region Private Methods
            private static void parse()
            {
                if (m_desc != null)
                {
                    return;
                }
                // TBD - Add status message to user.
                m_desc = new Dictionary<ushort, string>();
                if (!File.Exists(m_filePath))
                {
                    return;
                }
                StreamReader l_reader = File.OpenText(m_filePath);
                string l_line = "";
                while ((l_line = l_reader.ReadLine()) != null)
                {
                    ushort l_id;
                    string l_desc;
                    if (processLine(ref l_line, out l_id, out l_desc))
                    {
                        replaceCharacters(ref l_desc);
                        m_desc[l_id] = l_desc;
                    }
                }
                l_reader.Close();

                Categorizer.SetDescription(ref m_desc);
                m_parsed = true;
            }
            private static bool processLine(ref string iLine, out ushort oId, out string oDesc)
            {
                oId = Things.invalidID;
                oDesc = "";

                string l_patternNum = "=([^,}]*)[,}]";
                string l_patternStr = "=\"([^\"]*)\"";
                Regex l_idRegex = new Regex("id" + l_patternNum);
                Match l_idMatch = l_idRegex.Match(iLine);
                if (l_idMatch.Groups.Count != 2)
                {
                    return false;
                }
                oId = ushort.Parse(l_idMatch.Groups[1].ToString());
                if (oId == Things.invalidID)
                {
                    return false;
                }
                Regex l_descRegex = new Regex("en" + l_patternStr);
                Match l_descMatch = l_descRegex.Match(iLine);
                oDesc = l_descMatch.Groups[1].ToString();

                return true;
            }
            #endregion Private Methods
        }
    }
}
