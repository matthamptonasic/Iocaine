using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using Iocaine2;
using Iocaine2.Logging;

namespace IocaineUpdater
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            const bool PromptForUpdates = false;
            const String mainFile = ".\\Main.exe";

            //Set up log file.
            LoggingFunctions.SetLogFile("Updater Debug Log.txt");
            LoggingFunctions.Timestamp("Beginning Iocaine Update.");

            Server.Update.Init_Iocaine();

            try
            {
                DisplayBox updateDisplay = new DisplayBox(PromptForUpdates);
                updateDisplay.ShowDialog();
            }
            catch (Exception Ex)
            {
                LoggingFunctions.Error("While updating files using display box: " + Ex);
            }

            String mainFileAbsolute = Path.GetFullPath(mainFile);
            if (File.Exists(mainFileAbsolute))
            {
                try
                {
                    Process IocaineMain = new Process();
                    IocaineMain.StartInfo.FileName = mainFileAbsolute;
                    IocaineMain.Start();
                }
                catch (Exception Ex)
                {
                    LoggingFunctions.Error("Starting main process: " + Ex);
                }
            }
            else
            {
                MessageBox.Show("Could not find " + mainFileAbsolute + "!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
