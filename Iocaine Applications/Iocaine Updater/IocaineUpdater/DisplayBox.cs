using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Iocaine2;
using Iocaine2.Logging;

namespace IocaineUpdater
{
    public partial class DisplayBox : Form
    {
        private Thread startupThread = null;
        private delegate void updateLabelTextPtr(String iText);
        private delegate void updateProgressBarPtr(Int32 iPerc);
        private delegate DialogResult promptChangeLogPtr(String iText);
        private delegate void updateFormPtr();
        private delegate void closeFormPtr();
        public DisplayBox(bool promptForUpdates)
        {
            this.PromptForUpdates = promptForUpdates;
            InitializeComponent();
            this.BringToFront();
            startupThread = new Thread(new ThreadStart(runStartup));
            startupThread.IsBackground = true;
            startupThread.Start();
        }
        private bool PromptForUpdates = false;
        private void runStartup()
        {
            ArrayList fileList = null;
            ArrayList updateList = new ArrayList();
            bool waitForClose = false;

            UpdateProgressBar(0);
            UpdateLabelText("Getting update data from server...");
            try
            {
                fileList = Server.Update.GetFileList("UpdateData.txt");
            }
            catch (Exception Ex)
            {
                LoggingFunctions.Error("Getting file list from server: " + Ex);
            }
            if (fileList == null)
            {
                UpdateLabelText("Error connecting to Iocaine Update server.");
                Thread.Sleep(3000);
            }
            else
            {
                UpdateProgressBar(10);
                bool updateAvailable = false;
                foreach (String str in fileList)
                {
                    String[] fileInfo = str.Split(',');
                    String fileName = fileInfo[0];
                    if (fileName == "Iocaine2.exe")
                    {
                        continue;
                    }
                    if (Server.Update.CheckUpdateAvailable(str))
                    {
                        UpdateLabelText(fileName + " update available");
                        updateAvailable = true;
                        updateList.Add(fileName);
                    }
                }
                UpdateProgressBar(20);
                if (updateAvailable)
                {
                    //Check if the Main.exe (or Main.vshost.exe) is running in this folder.
                    String currExe = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    String currPath = Path.GetDirectoryName(currExe);
                    String mainFullName = Path.Combine(currPath, "Main.exe");
                    String mainBuildFullName = Path.Combine(currPath, "Main.vshost.exe");
                    while (Iocaine2.Memory.ProcessFunctions.IsRunning(new List<string> { mainFullName, mainBuildFullName }))
                    {
                        UpdateLabelText("Updating files. You must exit Iocaine to continue.\r\nWaiting for Iocaine to exit...");
                        Thread.Sleep(1000);
                    }

                    int nbFiles = updateList.Count;
                    int percPerFile = 80 / (nbFiles > 0 ? nbFiles : 1);

                    UpdateForm();
                    bool updatedOk = true;
                    int cnt = 1;
                    foreach (String str in updateList)
                    {
                        String[] fileInfo = str.Split(',');
                        String fileName = fileInfo[0];
                        UpdateLabelText("Downloading " + fileName);
                        if (!Server.Update.UpdateFile(fileName))
                        {
                            updatedOk = false;
                        }
                        else
                        {
                            UpdateLabelText(fileName + " update OK");
                        }
                        UpdateProgressBar(20 + (cnt * percPerFile));
                        cnt++;
                    }
                    if (updatedOk)
                    {
                        //After updating, prompt to see the change log.
                        String updateListString = "";
                        foreach (String str in updateList)
                        {
                            String[] fileInfo = str.Split(',');
                            String fileName = fileInfo[0];
                            updateListString += (fileName + "\n");
                        }
                        updateListString += "\nWould you like to view the Change Log?";
                        DialogResult promptForChangeLog;
                        promptForChangeLog = PromptChangeLog(updateListString);
                        UpdateForm();
                        if (promptForChangeLog == DialogResult.Yes)
                        {
                            if (File.Exists("./Change Log.txt"))
                            {
                                Process.Start("notepad.exe", "Change Log.txt");
                            }
                            else
                            {
                                MessageBox.Show("No change log available.");
                            }
                        }
                        UpdateForm();
                    }
                    UpdateForm();
                }
                else
                {
                    UpdateLabelText("All files up to date.");
                    UpdateProgressBar(100);
                    Thread.Sleep(500);
                }
            }
            if (!waitForClose)
            {
                CloseForm();
            }
        }

        public void UpdateLabelText(String iText)
        {
            try
            {
                if (updaterTextLabel.InvokeRequired)
                {
                    updaterTextLabel.Invoke(new updateLabelTextPtr(UpdateLabelTextCallBackFunction), new object[] {iText});
                }
                else
                {
                    UpdateLabelTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In UpdateLabelText: " + e.ToString());
            }
        }
        private void UpdateLabelTextCallBackFunction(String newLine)
        {
            updaterTextLabel.Text = newLine;
            this.Refresh();
        }
        private void UpdateProgressBar(Int32 iValue)
        {
            try
            {
                if (updaterProgressBar.InvokeRequired)
                {
                    updaterProgressBar.Invoke(new updateProgressBarPtr(UpdateProgressBarCallBackFunction), new object[] { iValue });
                }
                else
                {
                    UpdateProgressBarCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In UpdateProgressBar: " + e.ToString());
            }
        }
        private void UpdateProgressBarCallBackFunction(Int32 iValue)
        {
            updaterProgressBar.Value = iValue;
            this.Refresh();
        }
        private DialogResult PromptChangeLog(String iText)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    return (DialogResult)this.Invoke(new promptChangeLogPtr(PromptChangeLogCallBackFunction), new object[] { iText });
                }
                else
                {
                    return PromptChangeLogCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In PromptChangeLog: " + e.ToString());
                return System.Windows.Forms.DialogResult.None;
            }
        }
        private DialogResult PromptChangeLogCallBackFunction(String iText)
        {
            return MessageBox.Show((DisplayBox)this, "Updated the following files successfully:\n\n" + iText, "Iocaine Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
        private void UpdateForm()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new updateFormPtr(this.Update));
                }
                else
                {
                    this.Update();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In UpdateForm: " + e.ToString());
            }
        }
        private void CloseForm()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new closeFormPtr(this.Close));
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In CloseForm: " + e.ToString());
            }
        }
    }
}
