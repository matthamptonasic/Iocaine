using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Iocaine2.Logging;

namespace Iocaine2.Parsing
{
    public static partial class Lua
    {
        #region Private Members
        private static bool m_initDone = false;
        private static string m_luaPath = "";
        private const string m_hookDllName = "Hook.dll";
        private const string m_folderName = "res";
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Inits
        public static bool Init_Process()
        {
            if (m_initDone)
            {
                return true;
            }

            // Find the location of the 'res' folder within Windower.
            if (!init_setFilePath())
            {
                return false;
            }
            if (!Items.Init_Process(m_luaPath))
            {
                return false;
            }
            if (!ItemDescriptions.Init_Process(m_luaPath))
            {
                return false;
            }

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
        #endregion Public Methods

        #region Private Methods
        private static void replaceCharacters(ref string ioText)
        {
            //ioText = ioText.Replace("&amp;", "&");
            ioText = ioText.Replace("♂", " Male");
            ioText = ioText.Replace("♀", " Female");
        }
        #endregion Private Methods
    }
}
