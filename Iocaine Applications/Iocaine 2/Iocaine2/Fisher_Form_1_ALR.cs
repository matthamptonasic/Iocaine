using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Settings;

namespace Iocaine2
{
    //This file is for functions directly related to the Alert tab.
    partial class Iocaine_2_Form
    {
        #region Private Members
        private const String ALR_whitelistTBDefText = "Type Name, Hit Enter to Add";
        private const String ALR_playMessageTBDefText = "Text to Say or File to Play";
        private static Boolean ALR_loadingSettings = false;
        private static BindingSource ALR_whitelistSavedBS;
        private static List<String> ALR_whitelistTemp = new List<string>();
        private static BindingSource ALR_whitelistTempBS;
        private static List<String> ALR_currentList = new List<string>();
        private static BindingSource ALR_currentBS;
        private static Boolean ALR_currentHovering = false;
        private static Dictionary<BOT, BOT_STATE> ALR_previousStateMap = new Dictionary<BOT, BOT_STATE>();
        private static Boolean ALR_alerting = false;
        private static bool ALR_clearAlwaysAlert = false;
        private static event Statics.FuncPtrs.TD_Void_Bool _ALR_AlertChanged;
        #endregion Private Members
        #region Inits
        private void ALR_doInits()
        {
            ALR_loadUserSettings();
            ALR_whitelistSavedBS = new BindingSource(Statics.Settings.Alert.Whitelist, null);
            ALR_WhitelistSavedLB.DataSource = ALR_whitelistSavedBS;
            ALR_whitelistTempBS = new BindingSource(ALR_whitelistTemp, null);
            ALR_WhitelistTempLB.DataSource = ALR_whitelistTempBS;
            ALR_currentBS = new BindingSource(ALR_currentList, null);
            ALR_CurrentPCsLB.DataSource = ALR_currentBS;
            ALR_previousStateMap[BOT.FISHER] = BOT_STATE.STOPPED;
            ALR_previousStateMap[BOT.SU] = BOT_STATE.STOPPED;
            ALR_previousStateMap[BOT.CRAFTER] = BOT_STATE.STOPPED;
        }
        private void ALR_loadUserSettings()
        {
            Statics.Settings.Alert.Enable = (Boolean)UserSettings.GetValue(UserSettings.BOT.TOP, "ALR_Enable");
            Statics.Settings.Alert.AlwaysAlert = (Boolean)UserSettings.GetValue(UserSettings.BOT.TOP, "ALR_AlwaysAlert");
            Statics.Settings.Alert.PlayMessage = (Boolean)UserSettings.GetValue(UserSettings.BOT.TOP, "ALR_PlayMessage");
            Statics.Settings.Alert.MessageText = (String)UserSettings.GetValue(UserSettings.BOT.TOP, "ALR_MessageText");
            Statics.Settings.Alert.FlashTaskBar = (Boolean)UserSettings.GetValue(UserSettings.BOT.TOP, "ALR_FlashTaskBar");
            Statics.Settings.Alert.PauseBots = (Boolean)UserSettings.GetValue(UserSettings.BOT.TOP, "ALR_PauseBots");
            ALR_loadingSettings = true;
            ALR_setEnabledChkB(Statics.Settings.Alert.Enable);
            ALR_setAlwaysAlertChkB(Statics.Settings.Alert.AlwaysAlert);
            ALR_setPlayMessageChkB(Statics.Settings.Alert.PlayMessage);
            ALR_setMessageTextTB(Statics.Settings.Alert.MessageText);
            ALR_setFlashTaskbarChkB(Statics.Settings.Alert.FlashTaskBar);
            ALR_setPauseBotsChkB(Statics.Settings.Alert.PauseBots);

            Statics.Settings.Alert.Whitelist.Clear();
            List<List<Object>> whitelistSettings = UserSettings.GetList(UserSettings.BOT.TOP, UserSettings.LIST_TABLE.ALR_WHITELIST);
            if (whitelistSettings != null)
            {
                //Outer list are the rows.
                //Inner list are the cells. In this case, just 1 column, just 1 cell, the PC's name.
                for (int ii = 0; ii < whitelistSettings.Count; ii++)
                {
                    Statics.Settings.Alert.Whitelist.Add(whitelistSettings[ii][0].ToString());
                }
            }
            ALR_loadingSettings = false;
        }
        #endregion Inits
        #region Event Handlers
        #region Text Boxes
        private void ALR_WhitelistSavedEntryTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                if (ALR_WhitelistSavedEntryTB.Text != "")
                {
                    ALR_whitelistSavedBS.Add(ALR_WhitelistSavedEntryTB.Text);
                    UserSettings.AddListValue(UserSettings.BOT.TOP, UserSettings.LIST_TABLE.ALR_WHITELIST, new List<Object>(){ALR_WhitelistSavedEntryTB.Text});
                    UserSettings.SaveSettings();
                    ALR_WhitelistSavedEntryTB.Text = "";
                }
                e.Handled = true;
            }
        }
        private void ALR_WhitelistSavedEntryTB_Enter(object sender, EventArgs e)
        {
            if(ALR_WhitelistSavedEntryTB.Text == ALR_whitelistTBDefText)
            {
                ALR_WhitelistSavedEntryTB.Text = "";
            }
        }
        private void ALR_WhitelistSavedEntryTB_Leave(object sender, EventArgs e)
        {
            if (ALR_WhitelistSavedEntryTB.Text == "")
            {
                ALR_WhitelistSavedEntryTB.Text = ALR_whitelistTBDefText;
            }
        }
        private void ALR_WhitelistTempEntryTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                if (ALR_WhitelistSavedEntryTB.Text != "")
                {
                    ALR_whitelistTempBS.Add(ALR_WhitelistTempEntryTB.Text);
                    ALR_WhitelistSavedEntryTB.Text = "";
                }
                e.Handled = true;
            }
        }
        private void ALR_WhitelistTempEntryTB_Enter(object sender, EventArgs e)
        {
            if (ALR_WhitelistTempEntryTB.Text == ALR_whitelistTBDefText)
            {
                ALR_WhitelistTempEntryTB.Text = "";
            }
        }
        private void ALR_WhitelistTempEntryTB_Leave(object sender, EventArgs e)
        {
            if (ALR_WhitelistTempEntryTB.Text == "")
            {
                ALR_WhitelistTempEntryTB.Text = ALR_whitelistTBDefText;
            }
        }
        private void ALR_PlaySoundTB_Enter(object sender, EventArgs e)
        {
            if (ALR_PlaySoundTB.Text == ALR_playMessageTBDefText)
            {
                ALR_PlaySoundTB.Text = "";
            }
        }
        private void ALR_PlaySoundTB_Leave(object sender, EventArgs e)
        {
            if (ALR_PlaySoundTB.Text == "")
            {
                ALR_PlaySoundTB.Text = ALR_playMessageTBDefText;
            }
            else
            {
                Statics.Settings.Alert.MessageText = ALR_PlaySoundTB.Text;
                UserSettings.SetValue(UserSettings.BOT.TOP, "ALR_MessageText", Statics.Settings.Alert.MessageText);
                UserSettings.SaveSettings();
            }
        }
        #endregion Text Boxes
        #region Check Boxes
        private void ALR_EnableAlertsChkB_CheckedChanged(object sender, EventArgs e)
        {
            if (!ALR_loadingSettings)
            {
                if (!ALR_EnableAlertsChkB.Checked)
                {
                    Tools.Audio.StopSound();
                }
                Statics.Settings.Alert.Enable = ALR_EnableAlertsChkB.Checked;
                UserSettings.SetValue(UserSettings.BOT.TOP, "ALR_Enable", Statics.Settings.Alert.Enable.ToString());
                UserSettings.SaveSettings();
            }
        }
        private void ALR_AlwaysAlertChkB_CheckedChanged(object sender, EventArgs e)
        {
            if (!ALR_loadingSettings)
            {
                Statics.Settings.Alert.AlwaysAlert = ALR_AlwaysAlertChkB.Checked;
                UserSettings.SetValue(UserSettings.BOT.TOP, "ALR_AlwaysAlert", Statics.Settings.Alert.AlwaysAlert.ToString());
                UserSettings.SaveSettings();
                if (!Statics.Settings.Alert.AlwaysAlert && ALR_alerting)
                {
                    ALR_clearAlwaysAlert = true;
                }
            }
        }
        private void ALR_PlayMessageChkB_CheckedChanged(object sender, EventArgs e)
        {
            if (!ALR_loadingSettings)
            {
                Statics.Settings.Alert.PlayMessage = ALR_PlayMessageChkB.Checked;
                UserSettings.SetValue(UserSettings.BOT.TOP, "ALR_PlayMessage", Statics.Settings.Alert.PlayMessage.ToString());
                UserSettings.SaveSettings();
            }
        }
        private void ALR_FlashTaskBarChkB_CheckedChanged(object sender, EventArgs e)
        {
            if (!ALR_loadingSettings)
            {
                Statics.Settings.Alert.FlashTaskBar = ALR_FlashTaskBarChkB.Checked;
                UserSettings.SetValue(UserSettings.BOT.TOP, "ALR_FlashTaskBar", Statics.Settings.Alert.FlashTaskBar.ToString());
                UserSettings.SaveSettings();
            }
        }
        private void ALR_PauseBotsChkB_CheckedChanged(object sender, EventArgs e)
        {
            if (!ALR_loadingSettings)
            {
                Statics.Settings.Alert.PauseBots = ALR_PauseBotsChkB.Checked;
                UserSettings.SetValue(UserSettings.BOT.TOP, "ALR_PauseBots", Statics.Settings.Alert.PauseBots.ToString());
                UserSettings.SaveSettings();
            }
        }
        #endregion Check Boxes
        #region Buttons
        private void ALR_BrowseFileToPlayButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Waveform Audio File (*.wav) |*.wav;";
            ofd.Multiselect = false;
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                Statics.Settings.Alert.MessageText = ofd.FileName;
                ALR_PlaySoundTB.Text = ofd.FileName;
                UserSettings.SetValue(UserSettings.BOT.TOP, "ALR_MessageText", Statics.Settings.Alert.MessageText);
                UserSettings.SaveSettings();
            }
        }
        #endregion Buttons
        #region List Boxes
        private void ALR_WhitelistSavedLB_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right)
            {
                return;
            }
            Int32 idx = ALR_WhitelistSavedLB.IndexFromPoint(e.Location);
            if (idx != System.Windows.Forms.ListBox.NoMatches)
            {
                UserSettings.RemoveListValue(UserSettings.BOT.TOP, UserSettings.LIST_TABLE.ALR_WHITELIST, Statics.Settings.Alert.Whitelist[idx]);
                ALR_whitelistSavedBS.RemoveAt(idx);
            }
        }
        private void ALR_WhitelistTempLB_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right)
            {
                return;
            }
            Int32 idx = ALR_WhitelistTempLB.IndexFromPoint(e.Location);
            if (idx != System.Windows.Forms.ListBox.NoMatches)
            {
                ALR_whitelistTempBS.RemoveAt(idx);
            }
        }
        private void ALR_WhitelistTempLB_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Int32 idx = ALR_WhitelistTempLB.IndexFromPoint(e.Location);
            if (idx != System.Windows.Forms.ListBox.NoMatches)
            {
                String item = (String)ALR_whitelistTempBS[idx];
                ALR_whitelistSavedBS.Add(item);
                UserSettings.AddListValue(UserSettings.BOT.TOP, UserSettings.LIST_TABLE.ALR_WHITELIST, new List<object>() { item });
                UserSettings.SaveSettings();
                ALR_whitelistTempBS.RemoveAt(idx);
            }
        }
        private void ALR_CurrentPCsLB_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Int32 idx = ALR_CurrentPCsLB.IndexFromPoint(e.Location);
            if (idx != System.Windows.Forms.ListBox.NoMatches)
            {
                String item = (String)ALR_currentBS[idx];
                ALR_whitelistTempBS.Add(item);
            }
        }
        private void ALR_CurrentPCsLB_MouseEnter(object sender, EventArgs e)
        {
            ALR_currentHovering = true;
        }
        private void ALR_CurrentPCsLB_MouseLeave(object sender, EventArgs e)
        {
            ALR_currentHovering = false;
        }
        #endregion List Boxes
        #endregion Event Handlers
        #region Updates
        private void ALR_setCurrentPcs(List<MemReads.NPCs.NPCInfoStruct> iNpcs)
        {
            try
            {
                if(ALR_CurrentPCsLB.InvokeRequired)
                {
                    ALR_CurrentPCsLB.Invoke(new Statics.FuncPtrs.TD_Void_NpcInfoList(ALR_setCurrentPcsCBF), new object[] { iNpcs });
                }
                else
                {
                    ALR_setCurrentPcsCBF(iNpcs);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("ALR_setCurrentPcs:\r\n" + e.ToString());
            }
        }
        public void ALR_setCurrentPcsCBF(List<MemReads.NPCs.NPCInfoStruct> iNpcs)
        {
            //Go through the list and pull out the PC's into a new list.
            Monitor.Enter(m_MAP_npcLock);
            List<MemReads.NPCs.NPCInfoStruct> PcInfoList = new List<MemReads.NPCs.NPCInfoStruct>();
            try
            {
                foreach (MemReads.NPCs.NPCInfoStruct npc in iNpcs)
                {
                    String name = MemReads.NPCs.getName(npc);
                    if (npc.Type == MemReads.NPCs.eType.Player)
                    {
                        if (PlayerCache.Vitals.Name != name)
                        {
                            PcInfoList.Add(npc);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("ALR_setCurrentPcsCBF:\r\n" + e.ToString());
            }
            finally
            {
                Monitor.Exit(m_MAP_npcLock);
            }
            // If there are any PC's not in the whitelists and Alerts are enabled, go through the actions.
            // Check the current action state first. If the action is already being performed, ignore.
            // Also check if we're running bots. If we're not running anything, skip unless set to always alert.
            Boolean callActions = false;
            String pcName = "";
            if (ALR_botsRunning() || ALR_alerting || Statics.Settings.Alert.AlwaysAlert)
            {
                if (!ALR_botsRunning() && ALR_alerting && ALR_clearAlwaysAlert)
                {
                    ALR_clearAlwaysAlert = false;
                }
                else
                {
                    foreach (MemReads.NPCs.NPCInfoStruct pc in PcInfoList)
                    {
                        pcName = MemReads.NPCs.getName(pc);
                        if (!Statics.Settings.Alert.Whitelist.Contains(pcName) && !ALR_whitelistTemp.Contains(pcName))
                        {
                            // PC is not in our lists. Call actions if enabled.
                            if (Statics.Settings.Alert.Enable)
                            {
                                callActions = true;
                                break;
                            }
                        }
                    }
                }
            }
            if(!callActions && ALR_alerting)
            {
                // We were alerting, but now no one is in range, so reset the aleart status.
                ALR_clearAlerts();
                TOP_Form_FlashWindowEx(this, false);
                LoggingFunctions.Timestamp("Clearing alert.");
            }
            else if (callActions && !ALR_alerting)
            {
                LoggingFunctions.Timestamp("Alerting due to PC '" + pcName + "' coming into range.");
                if(Statics.Settings.Alert.PlayMessage)
                {
                    Tools.Audio.PlaySound(Statics.Settings.Alert.MessageText);
                    ALR_alerting = true;
                }
                if (Statics.Settings.Alert.FlashTaskBar)
                {
                    TOP_Form_FlashWindowEx(this);
                    ALR_alerting = true;
                }
                if(Statics.Settings.Alert.PauseBots)
                {
                    ALR_pauseBots();
                    ALR_alerting = true;
                }
                if (_ALR_AlertChanged != null)
                {
                    _ALR_AlertChanged(true);
                }
            }

            if(!ALR_currentHovering)
            {
                //Update the current LB items.
                ALR_currentBS.Clear();
                foreach(MemReads.NPCs.NPCInfoStruct pc in PcInfoList)
                {
                    ALR_currentBS.Add(MemReads.NPCs.getName(pc));
                }
            }
        }
        public void ALR_setEnabledChkB(Boolean iValue)
        {
            try
            {
                if (ALR_EnableAlertsChkB.InvokeRequired)
                {
                    ALR_EnableAlertsChkB.Invoke(new Statics.FuncPtrs.TD_Void_Bool(ALR_setEnabledChkBCBF), new object[] { iValue });
                }
                else
                {
                    ALR_setEnabledChkBCBF(iValue);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("ALR_setEnabledChkB:\r\n" + e.ToString());
            }
        }
        public void ALR_setEnabledChkBCBF(Boolean iValue)
        {
            ALR_EnableAlertsChkB.Checked = iValue;
        }
        public void ALR_setAlwaysAlertChkB(Boolean iValue)
        {
            try
            {
                if (ALR_AlwaysAlertChkB.InvokeRequired)
                {
                    ALR_AlwaysAlertChkB.Invoke(new Statics.FuncPtrs.TD_Void_Bool(ALR_setAlwaysAlertChkBCBF), new object[] { iValue });
                }
                else
                {
                    ALR_setAlwaysAlertChkBCBF(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("ALR_setAlwaysAlertChkB:\r\n" + e.ToString());
            }
        }
        public void ALR_setAlwaysAlertChkBCBF(Boolean iValue)
        {
            ALR_AlwaysAlertChkB.Checked = iValue;
        }
        public void ALR_setPlayMessageChkB(Boolean iValue)
        {
            try
            {
                if(ALR_PlayMessageChkB.InvokeRequired)
                {
                    ALR_PlayMessageChkB.Invoke(new Statics.FuncPtrs.TD_Void_Bool(ALR_setPlayMessageChkBCBF), new object[] { iValue });
                }
                else
                {
                    ALR_setPlayMessageChkBCBF(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("ALR_setPlayMessageChkB:\r\n" + e.ToString());
            }
        }
        public void ALR_setPlayMessageChkBCBF(Boolean iValue)
        {
            ALR_PlayMessageChkB.Checked = iValue;
        }
        public void ALR_setFlashTaskbarChkB(Boolean iValue)
        {
            try
            {
                if (ALR_FlashTaskBarChkB.InvokeRequired)
                {
                    ALR_FlashTaskBarChkB.Invoke(new Statics.FuncPtrs.TD_Void_Bool(ALR_setFlashTaskbarChkBCBF), new object[] { iValue });
                }
                else
                {
                    ALR_setFlashTaskbarChkBCBF(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("ALR_setFlashTaskbarChkB:\r\n" + e.ToString());
            }
        }
        public void ALR_setFlashTaskbarChkBCBF(Boolean iValue)
        {
            ALR_FlashTaskBarChkB.Checked = iValue;
        }
        public void ALR_setPauseBotsChkB(Boolean iValue)
        {
            try
            {
                if (ALR_PauseBotsChkB.InvokeRequired)
                {
                    ALR_PauseBotsChkB.Invoke(new Statics.FuncPtrs.TD_Void_Bool(ALR_setPauseBotsChkBCBF), new object[] { iValue });
                }
                else
                {
                    ALR_setPauseBotsChkBCBF(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("ALR_setPauseBotsChkB:\r\n" + e.ToString());
            }
        }
        public void ALR_setPauseBotsChkBCBF(Boolean iValue)
        {
            ALR_PauseBotsChkB.Checked = iValue;
        }
        public void ALR_setMessageTextTB(String iText)
        {
            try
            {
                if (ALR_PlaySoundTB.InvokeRequired)
                {
                    ALR_PlaySoundTB.Invoke(new Statics.FuncPtrs.TD_Void_String(ALR_setMessageTextTBCBF), new object[] { iText });
                }
                else
                {
                    ALR_setMessageTextTBCBF(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("ALR_setMessageTextTBCBF:\r\n" + e.ToString());
            }
        }
        public void ALR_setMessageTextTBCBF(String iText)
        {
            if(iText == "")
            {
                ALR_PlaySoundTB.Text = ALR_playMessageTBDefText;
            }
            else
            {
                ALR_PlaySoundTB.Text = iText;
            }
        }
        #endregion Updates
        #region Utilities
        private void ALR_pauseBots()
        {
            //Set the previous state map so we can return afterwards.
            if (Bots.Fisher.Access.State == Bots.STATE.RUNNING)
            {
                ALR_previousStateMap[BOT.FISHER] = BOT_STATE.RUNNING;
                Bots.Fisher.Access.PauseAlert();
            }
            if (Bots.SkillUp.Access.State == Bots.STATE.RUNNING)
            {
                ALR_previousStateMap[BOT.SU] = BOT_STATE.RUNNING;
                Bots.SkillUp.Access.PauseAlert();
            }
            if (Crafter1 == null)
            {
                ALR_previousStateMap[BOT.CRAFTER] = BOT_STATE.STOPPED;
            }
            else if (Crafter1.State == Bots.Crafter.CRAFTER_STATE.RUNNING)
            {
                ALR_previousStateMap[BOT.CRAFTER] = BOT_STATE.RUNNING;
                Crafter1.PauseCrafter();
            }
        }
        private Boolean ALR_botsRunning()
        {
            if (Bots.Fisher.Access.State == Bots.STATE.RUNNING)
            {
                return true;
            }
            else if (Bots.SkillUp.Access.State == Bots.STATE.RUNNING)
            {
                return true;
            }
            else if ((Crafter1 != null) && (Crafter1.State == Bots.Crafter.CRAFTER_STATE.RUNNING))
            {
                return true;
            }
            else if (Statics.Settings.POS.Enable && Statics.Settings.POS.DisableOnAlert)
            {
                return true;
            }
            return false;
        }
        private void ALR_clearAlerts()
        {
            if (ALR_alerting)
            {
                ALR_alerting = false;
                if(Statics.Settings.Alert.PauseBots)
                {
                    if(Bots.Fisher.Access.State == Bots.STATE.PAUSED_ALR)
                    {
                        Bots.Fisher.Access.Resume();
                    }
                    if(Bots.SkillUp.Access.State == Bots.STATE.PAUSED_ALR)
                    {
                        Bots.SkillUp.Access.Resume();
                    }
                    if(ALR_previousStateMap[BOT.CRAFTER] == BOT_STATE.RUNNING)
                    {
                        Crafter1.ResumeCrafter();
                    }
                }
                if (_ALR_AlertChanged != null)
                {
                    _ALR_AlertChanged(false);
                }
            }
        }
        #endregion Utilities
    }
}