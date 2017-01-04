using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Iocaine2.Logging;

namespace Iocaine2.Parsing
{
    public static partial class Lua
    {
        #region Description
        /*
            End goal:
            To be able to make queries of all gear (armor & weapons) based on level, job, ra/ex (flags), as
            well as any attribute such as STR, MP+, Refresh, Double Attack, etc, and present the resulting
            selection in order for the user.
            This will require at least the following:
            1.  Grouping all armor and weapons into their own list/dictionaries.
                - List of keys to full Dictionary of item descriptions should suffice.
            2.  Creating a queriable collection of all attributes. This will be quite extensive.
                This also needs to be easily modifiable so that we can add attributes with little effort.
                This will also be a focus area of work. Many items do not give quantitative descriptions.
                For example, "Enhances critical hit damage.".  We'll have to be able to fill these in 
                with our own values from the wiki or forums.  Meaning, we need a simple way to make this
                substituation (item_id + string value => string value changed to noted value).
                These MUST be in a text file format that we parse. No more hidden values.
                - Read text file into a table. This way we can easily sync it to the server.
                - The text file should be item name, not ID. Do the substitution/lookup for the user.
                - Perform each substitution on the actual list of descriptions? Or do we need to make a copy?


                - The final/master data structure needs to be a table.
                    - This means that each attribute needs to be it's own column.
                    - If we create the table ourselves, it will not be strongly typed which is less than ideal.
                    - We could use the dataset wizard to create the table. Any other data related to the column
                      must be kept elsewhere.
                      For instance, each column will need some kind of parsing pattern to match. We should keep a 
                      dictionary of each column to a structure containing all metadata needed.
            3.  Creating a way in which users can edit the above mentioned substitions and have the
                edits propogate to others.
            4.  Similar to above, we need to have a system of adding augment possibilities to items (multiple per item).
                This also needs to propogate to other users.
        */
        #endregion Description
        #region Private Members
        private static bool m_initDone = false;
        private static string m_luaPath = "";
        private const string m_hookDllName = "Hook.dll";
        private const string m_folderName = "res";
        private const string m_commentChars = "//";
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Inits
        public static bool Init_Process(bool iWipeClean = false)
        {
            if (iWipeClean)
            {
                m_initDone = false;
                Categorizer.DeleteBookmark();
            }
            if (m_initDone)
            {
                return true;
            }

            // Find the location of the 'res' folder within Windower.
            if (!init_setFilePath())
            {
                return false;
            }
            if (!Items.Init_Process(m_luaPath, iWipeClean))
            {
                return false;
            }
            if (!ItemDescriptions.Init_Process(m_luaPath, iWipeClean))
            {
                return false;
            }

            Items.PostProcess();

            m_initDone = true;
            return true;
        }
        private static bool init_setFilePath()
        {
            Process l_proc = ChangeMonitor.MainProc;
            LoggingFunctions.Timestamp("======================= Listing all modules in MainProc =======================");
            foreach (ProcessModule mod in l_proc.Modules)
            {
                if (mod.ModuleName == m_hookDllName)
                {
                    string l_location = "";
                    FileInfo info = new FileInfo(mod.FileName);
                    l_location = info.DirectoryName + @"\" + m_folderName + @"\";
                    if (!Directory.Exists(l_location))
                    {
                        LoggingFunctions.Warning("Could not location the '" + m_folderName + "' folder in your Windower area: " + l_location);
                        return false;
                    }
                    m_luaPath = l_location;
                    return true;
                }
            }
            return true;
        }
        #endregion Inits

        #region Public Methods
        public static List<string> GetItemAttributes(string iFilter)
        {
            List<string> l_retVal = new List<string>();
            Items.ItemInfo l_info = Items.GetItem(ref iFilter);
            if (l_info.m_id == 0)
            {
                l_retVal.Add("Error");
                return l_retVal;
            }
            l_retVal.Add(l_info.m_name);
            l_retVal.Add(ItemDescriptions.GetDescription(l_info.m_id));
            string l_attrStr = "";
            if (l_info.m_attributes == null)
            {
                return l_retVal;
            }
            foreach (Categorizer.AttrValue i_attr in l_info.m_attributes)
            {
                l_attrStr = "";
                if (i_attr.m_restricted)
                {
                    l_attrStr += Categorizer.GetAttributeName(i_attr.m_restrAttrId) + ": ";
                }
                if (i_attr.m_values == null)
                {
                    l_attrStr += "* " + Categorizer.GetAttributeName(i_attr.m_attrId) + " *";
                    l_retVal.Add(l_attrStr);
                    continue;
                }
                l_attrStr += Categorizer.GetAttributeName(i_attr.m_attrId) + " =";
                foreach (short i_val in i_attr.m_values)
                {
                    if (i_attr.m_type == Categorizer.ValueType.SHORT)
                    {
                        l_attrStr += " " + i_val;
                    }
                    else
                    {
                        float l_tmpFloat = i_val / 10f;
                        l_attrStr += " " + l_tmpFloat.ToString("1");
                    }
                }
                l_retVal.Add(l_attrStr);
            }
            return l_retVal;
        }
        #endregion Public Methods

        #region Private Methods
        private static void replaceCharacters(ref string ioText)
        {
            //ioText = ioText.Replace("&amp;", "&");
            ioText = ioText.Replace(@"\n", " ");
            ioText = ioText.Replace(@"\", "");
            ioText = ioText.Replace("♂", " Male");
            ioText = ioText.Replace("♀", " Female");
        }
        private static bool isCommented(string iLine)
        {
            Regex l_regex_comment = new Regex(@"^\s*" + m_commentChars);
            Regex l_regex_blank = new Regex(@"^\s*$");
            return l_regex_comment.IsMatch(iLine) || l_regex_blank.IsMatch(iLine);
        }
        #endregion Private Methods
    }
}
