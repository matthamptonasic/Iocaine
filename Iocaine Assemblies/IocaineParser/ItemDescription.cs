using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Iocaine2.Logging;

namespace Iocaine2.Parsing
{
    public static class ItemDescription
    {
        #region Private Members
        private static bool m_initDone = false;
        private const string m_folderName = "res";
        private const string m_fileName = "item_descriptions.lua";
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Inits
        public static bool Init_Iocaine()
        {
            if (m_initDone)
            {
                return true;
            }

            // Find the location of the 'res' folder within Windower.
            Process l_proc = ChangeMonitor.MainProc;
            LoggingFunctions.Timestamp("======================= Listing all modules in MainProc =======================");
            foreach (ProcessModule mod in l_proc.Modules)
            {
                LoggingFunctions.Timestamp(mod.ModuleName + " :: " + mod.FileName);
            }

            m_initDone = true;
            return true;
        }
        #endregion Inits

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
