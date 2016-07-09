using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Logging;

namespace Iocaine2.Settings
{
    public partial class Top_Settings_Form : Form
    {
        #region Members
        private bool localKillPolOnGmTell = false;
        private bool localStopAllBotsOnGmTell = false;
        private bool localStopFisherOnGmTell = true;
        private UInt32 localMoveUpDownDelay = 500;
        private UInt32 localMoveItemDelay = 1100;
        private UInt32 localKeyHoldTime = 150;
        private UInt32 localDebugScope = 0x0;
        private bool localRetainSettings = false;
        private Keys localFishingLeftArrowKey = Keys.NumPad4;
        private Keys localFishingRightArrowKey = Keys.NumPad6;
        #endregion Members
        #region Constructor
        public Top_Settings_Form(int iStartX, int iStartY)
        {
            InitializeComponent();
            this.SetDesktopLocation(iStartX - (this.Size.Width / 2), iStartY - (this.Size.Height / 2));
            initForm();
        }
        #endregion Constructor
        #region Inits
        private void initForm()
        {
            #region GM Reaction
            localKillPolOnGmTell = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "TOP_KillOnGmTell");
            localStopAllBotsOnGmTell = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "TOP_StopAllOnGmTell");
            localStopFisherOnGmTell = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "TOP_StopFisherOnGmTell");
            Top_Settings_Kill_POL_On_Gm_Tell_ChkB.Checked = localKillPolOnGmTell;
            Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB.Checked = localStopAllBotsOnGmTell;
            Top_Settings_Stop_Fisher_On_GM_Tell_ChkB.Checked = localStopFisherOnGmTell;
            #endregion GM Reaction
            #region Menu Navigation
            localMoveUpDownDelay = (UInt32)UserSettings.GetValue(UserSettings.BOT.TOP, "MoveUpDownDelay");
            localMoveItemDelay = (UInt32)UserSettings.GetValue(UserSettings.BOT.TOP, "MoveItemDelay");
            localKeyHoldTime = (UInt32)UserSettings.GetValue(UserSettings.BOT.TOP, "KeyHoldTime");
            MoveUpDownUpDn.Value = (decimal)localMoveUpDownDelay;
            MoveItemDelayUpDn.Value = (decimal)localMoveItemDelay;
            KeyHoldTimeUpDn.Value = (decimal)localKeyHoldTime;
            #endregion Menu Navigation
            #region Debug Settings
            localRetainSettings = Statics.Settings.Top.RetainSettings;
            localDebugScope = Statics.Settings.Top.DebugScope;
            DebugRetainSettingsChkB.Checked = localRetainSettings;
            loadDebugScopeCheckboxes();
            #endregion Debug Settings
            #region Keys
            FishingLeftArrowKeyCB.Items.Add("4");
            FishingLeftArrowKeyCB.Items.Add("A");
            FishingRightArrowKeyCB.Items.Add("6");
            FishingRightArrowKeyCB.Items.Add("D");
            loadKeys();
            #endregion Keys
        }
        private void loadDebugScopeCheckboxes()
        {
            DebugAllChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.ALL);
            DebugUnfilteredChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.UNFILTERED);
            DebugTopChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.TOP);
            DebugFisherChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.FISHER);
            DebugPLChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.PL);
            DebugSUChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.SU);
            DebugCrafterChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.CRAFTER);
            DebugTAChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.TA);
            DebugSynergyChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.SYNERGY);
            DebugNavChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.NAV);
            DebugTraderChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.TRADER);
            DebugBuyerChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.BUYER);
            DebugSellerChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.SELLER);
            DebugWMSChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.WMS);
            DebugBackgroundChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.BACKGROUND);
            DebugFishStatsChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.FISH_STAT);
            DebugInteractionChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.INTERACTION);
            DebugTimeChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.TIME);
            DebugCommandsChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.COMMANDS);
            DebugServerChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.SERVER);
            DebugSettingsChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.SETTINGS);
            DebugChatLogChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.CHAT);
            DebugInventoryChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.INVENTORY);
            DebugNpcPcChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.NPC_PC);
            DebugMemReadsChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.MEMREADS);
            DebugMemScannerChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.MEM_SCANNER);
            DebugWinAPIsChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.WIN_API);
            DebugChangeMonChkB.Checked = GetDebugScope(LoggingFunctions.DBG_SCOPE.CH_MON);
        }
        private void loadKeys()
        {
            switch (Statics.Settings.Fisher.LeftArrowKey)
            {
                case Keys.NumPad4:
                    FishingLeftArrowKeyCB.SelectedIndex = 0;
                    break;
                case Keys.A:
                    FishingLeftArrowKeyCB.SelectedIndex = 1;
                    break;
                default:
                    FishingLeftArrowKeyCB.SelectedIndex = 0;
                    break;
            }
            switch (Statics.Settings.Fisher.RightArrowKey)
            {
                case Keys.NumPad6:
                    FishingRightArrowKeyCB.SelectedIndex = 0;
                    break;
                case Keys.D:
                    FishingRightArrowKeyCB.SelectedIndex = 1;
                    break;
                default:
                    FishingRightArrowKeyCB.SelectedIndex = 0;
                    break;
            }
        }
        #endregion Inits
        #region Event Handlers
        #region Up/Dns
        private void MoveUpDownUpDn_ValueChanged(object sender, EventArgs e)
        {
            localMoveUpDownDelay = (UInt32)MoveUpDownUpDn.Value;
        }
        private void MoveItemDelayUpDn_ValueChanged(object sender, EventArgs e)
        {
            localMoveItemDelay = (UInt32)MoveItemDelayUpDn.Value;
        }
        private void KeyHoldTimeUpDn_ValueChanged(object sender, EventArgs e)
        {
            localKeyHoldTime = (UInt32)KeyHoldTimeUpDn.Value;
        }
        #endregion Up/Dns
        #region Buttons
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void Apply_Button_Click(object sender, EventArgs e)
        {
            SaveValues();
        }
        private void OK_Button_Click(object sender, EventArgs e)
        {
            SaveValues();
            this.Dispose();
        }
        #endregion Buttons
        #region Textboxes
        #endregion Textboxes
        #region Checkboxes
        #region GM Reaction
        private void Top_Settings_Kill_POL_On_Gm_Tell_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            localKillPolOnGmTell = Top_Settings_Kill_POL_On_Gm_Tell_ChkB.Checked;
        }
        private void Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            localStopAllBotsOnGmTell = Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB.Checked;
        }
        private void Top_Settings_Stop_Fisher_On_GM_Tell_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            localStopFisherOnGmTell = Top_Settings_Stop_Fisher_On_GM_Tell_ChkB.Checked;
        }
        #endregion GM Reaction
        #region Debug Settings
        private void DebugRetainSettingsChkB_CheckedChanged(object sender, EventArgs e)
        {
            localRetainSettings = ((CheckBox)sender).Checked;
        }
        private void DebugAllChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.ALL, ((CheckBox)sender).Checked);
        }
        private void DebugUnfilteredChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.UNFILTERED, ((CheckBox)sender).Checked);
        }
        private void DebugTopChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.TOP, ((CheckBox)sender).Checked);
        }
        private void DebugFisherChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.FISHER, ((CheckBox)sender).Checked);
        }
        private void DebugPLChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.PL, ((CheckBox)sender).Checked);
        }
        private void DebugSUChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.SU, ((CheckBox)sender).Checked);
        }
        private void DebugCrafterChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.CRAFTER, ((CheckBox)sender).Checked);
        }
        private void DebugTAChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.TA, ((CheckBox)sender).Checked);
        }
        private void DebugSynergyChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.SYNERGY, ((CheckBox)sender).Checked);
        }
        private void DebugNavChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.NAV, ((CheckBox)sender).Checked);
        }
        private void DebugTraderChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.TRADER, ((CheckBox)sender).Checked);
        }
        private void DebugBuyerChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.BUYER, ((CheckBox)sender).Checked);
        }
        private void DebugSellerChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.SELLER, ((CheckBox)sender).Checked);
        }
        private void DebugWMSChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.WMS, ((CheckBox)sender).Checked);
        }
        private void DebugBackgroundChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.BACKGROUND, ((CheckBox)sender).Checked);
        }
        private void DebugFishStatsChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.FISH_STAT, ((CheckBox)sender).Checked);
        }
        private void DebugInteractionChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.INTERACTION, ((CheckBox)sender).Checked);
        }
        private void DebugTimeChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.TIME, ((CheckBox)sender).Checked);
        }
        private void DebugCommandsChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.COMMANDS, ((CheckBox)sender).Checked);
        }
        private void DebugServerChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.SERVER, ((CheckBox)sender).Checked);
        }
        private void DebugSettingsChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.SETTINGS, ((CheckBox)sender).Checked);
        }
        private void DebugChatLogChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.CHAT, ((CheckBox)sender).Checked);
        }
        private void DebugInventoryChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.INVENTORY, ((CheckBox)sender).Checked);
        }
        private void DebugNpcPcChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.NPC_PC, ((CheckBox)sender).Checked);
        }
        private void DebugMemReadsChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.MEMREADS, ((CheckBox)sender).Checked);
        }
        private void DebugMemScannerChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.MEM_SCANNER, ((CheckBox)sender).Checked);
        }
        private void DebugWinAPIsChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.WIN_API, ((CheckBox)sender).Checked);
        }
        private void DebugChangeMonChkB_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugScope(LoggingFunctions.DBG_SCOPE.CH_MON, ((CheckBox)sender).Checked);
        }
        #endregion Debug Settings
        #endregion Checkboxes
        #region Comboboxes
        private void FishingLeftArrowKeyCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FishingLeftArrowKeyCB.SelectedIndex == 0)
            {
                localFishingLeftArrowKey = Keys.NumPad4;
            }
            else if (FishingLeftArrowKeyCB.SelectedIndex == 1)
            {
                localFishingLeftArrowKey = Keys.A;
            }
            else
            {
                localFishingLeftArrowKey = Keys.NumPad4;
            }
        }
        private void FishingRightArrowKeyCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FishingRightArrowKeyCB.SelectedIndex == 0)
            {
                localFishingRightArrowKey = Keys.NumPad6;
            }
            else if (FishingRightArrowKeyCB.SelectedIndex == 1)
            {
                localFishingRightArrowKey = Keys.D;
            }
            else
            {
                localFishingRightArrowKey = Keys.NumPad6;
            }
        }
        #endregion Comboboxes
        #endregion Event Handlers
        #region Utility Functions
        #region Save Values
        private void SaveValues()
        {
            UserSettings.SetValue(UserSettings.BOT.TOP, "TOP_KillOnGmTell", localKillPolOnGmTell.ToString());
            Statics.Settings.Top.KillOnGmTell = localKillPolOnGmTell;
            UserSettings.SetValue(UserSettings.BOT.TOP, "TOP_StopAllOnGmTell", localStopAllBotsOnGmTell.ToString());
            Statics.Settings.Top.StopAllOnGmTell = localStopAllBotsOnGmTell;
            UserSettings.SetValue(UserSettings.BOT.TOP, "TOP_StopFisherOnGmTell", localStopFisherOnGmTell.ToString());
            Statics.Settings.Top.StopFisherOnGmTell = localStopFisherOnGmTell;
            UserSettings.SetValue(UserSettings.BOT.TOP, "MoveUpDownDelay", localMoveUpDownDelay.ToString());
            Statics.Settings.Top.MoveUpDownDelay = localMoveUpDownDelay;
            UserSettings.SetValue(UserSettings.BOT.TOP, "MoveItemDelay", localMoveItemDelay.ToString());
            Statics.Settings.Top.MoveItemDelay = localMoveItemDelay;
            UserSettings.SetValue(UserSettings.BOT.TOP, "KeyHoldTime", localKeyHoldTime.ToString());
            Statics.Settings.Top.KeyHoldTime = localKeyHoldTime;
            UserSettings.SetValue(UserSettings.BOT.TOP, "RetainSettings", localRetainSettings.ToString());
            Statics.Settings.Top.RetainSettings = localRetainSettings;
            UserSettings.SetValue(UserSettings.BOT.TOP, "DebugScope", localDebugScope.ToString());
            Statics.Settings.Top.DebugScope = localDebugScope;
            UserSettings.SetValue(UserSettings.BOT.TOP, "FisherLeftArrowKey", localFishingLeftArrowKey.ToString());
            Statics.Settings.Fisher.LeftArrowKey = localFishingLeftArrowKey;
            UserSettings.SetValue(UserSettings.BOT.TOP, "FisherRightArrowKey", localFishingRightArrowKey.ToString());
            Statics.Settings.Fisher.RightArrowKey = localFishingRightArrowKey;
            UserSettings.SaveSettings();
        }
        #endregion Save Values
        #region Debug Settings Functions
        private void AddDebugScope(LoggingFunctions.DBG_SCOPE iScope)
        {
            localDebugScope |= (uint)iScope;
        }
        private void RemoveDebugScope(LoggingFunctions.DBG_SCOPE iScope)
        {
            if ((localDebugScope & (uint)iScope) != 0)
            {
                localDebugScope &= ~((uint)iScope);
            }
        }
        private bool GetDebugScope(LoggingFunctions.DBG_SCOPE iScope)
        {
            uint value = localDebugScope & (uint)iScope;
            if (value != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void SetDebugScope(LoggingFunctions.DBG_SCOPE iScope, bool iValue)
        {
            if (iValue == true)
            {
                AddDebugScope(iScope);
            }
            else
            {
                RemoveDebugScope(iScope);
            }
        }
        #endregion Debug Settings Functions
        #endregion Utility Functions
    }
}
