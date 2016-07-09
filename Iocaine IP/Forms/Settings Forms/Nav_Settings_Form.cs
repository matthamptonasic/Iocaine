using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Iocaine2.Settings
{
    public partial class Nav_Settings_Form : Form
    {
        #region Members
        private double localIntervalDefValue = 0d;
        private double localMinDistDefValue = 0d;
        private double localWaitDefValue = 0d;
        private String localSoundToPlay = "";
        private bool localPromptBeforeProcessing = true;
        private bool localSortByTags = true;
        private bool localSortByLastZone = true;
        #endregion Members
        #region Constructor
        public Nav_Settings_Form(int iStartX, int iStartY)
        {
            InitializeComponent();
            this.SetDesktopLocation(iStartX - (this.Size.Width / 2), iStartY - (this.Size.Height / 2));
            initForm();
        }
        #endregion Constructor
        #region Inits
        private void initForm()
        {
            localIntervalDefValue = (double)UserSettings.GetValue(UserSettings.BOT.NAV, "Nav_Rec_IntervalDefValue");
            localMinDistDefValue = (double)UserSettings.GetValue(UserSettings.BOT.NAV, "Nav_Rec_MinDistDefValue");
            localWaitDefValue = (double)UserSettings.GetValue(UserSettings.BOT.NAV, "Nav_Rec_WaitDefValue");
            localSoundToPlay = (String)UserSettings.GetValue(UserSettings.BOT.NAV, "Nav_TripCompleteSound");
            localPromptBeforeProcessing = (bool)UserSettings.GetValue(UserSettings.BOT.NAV, "Nav_Prc_PromptOnRightClick");
            localSortByTags = (bool)UserSettings.GetValue(UserSettings.BOT.NAV, "Nav_sortingTagsFirst");
            localSortByLastZone = (bool)UserSettings.GetValue(UserSettings.BOT.NAV, "Nav_sortingUseLastZoneOnly");
            Nav_Settings_Rec_Interval_UpDn.Value = (decimal)localIntervalDefValue;
            Nav_Settings_Rec_Min_Dist_UpDn.Value = (decimal)localMinDistDefValue;
            Nav_Settings_Rec_Wait_UpDn.Value = (decimal)localWaitDefValue;
            Nav_Settings_Done_Play_Sound_TB.Text = localSoundToPlay;
            Nav_Settings_Prompt_Before_Processing_ChkB.Checked = localPromptBeforeProcessing;
            Nav_Settings_Sort_Tags_First_ChkB.Checked = localSortByTags;
            Nav_Settings_Sort_By_Last_Zone_ChkB.Checked = localSortByLastZone;
        }
        #endregion Inits
        #region Event Handlers
        #region Up/Dns
        private void Nav_Settings_Rec_Interval_UpDn_ValueChanged(object sender, EventArgs e)
        {
            localIntervalDefValue = (double)Nav_Settings_Rec_Interval_UpDn.Value;
        }
        private void Nav_Settings_Rec_Min_Dist_UpDn_ValueChanged(object sender, EventArgs e)
        {
            localMinDistDefValue = (double)Nav_Settings_Rec_Min_Dist_UpDn.Value;
        }
        private void Nav_Settings_Rec_Wait_UpDn_ValueChanged(object sender, EventArgs e)
        {
            localWaitDefValue = (double)Nav_Settings_Rec_Wait_UpDn.Value;
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
        private void Nav_Settings_Done_Browse_File_To_Play_Button_Click(object sender, EventArgs e)
        {
            Nav_Settings_Done_Open_File_To_Play.Filter = "Waveform Audio File (*.wav) |*.wav;";
            Nav_Settings_Done_Open_File_To_Play.Multiselect = false;
            Nav_Settings_Done_Open_File_To_Play.ShowDialog();
            if (Nav_Settings_Done_Open_File_To_Play.FileName != "Nav_Settings_Done_Open_File_To_Play")
            {
                Nav_Settings_Done_Play_Sound_TB.Text = Nav_Settings_Done_Open_File_To_Play.FileName;
            }
        }
        #endregion Buttons
        #region Textboxes
        private void Nav_Settings_Done_Play_Sound_TB_TextChanged(object sender, EventArgs e)
        {
            localSoundToPlay = Nav_Settings_Done_Play_Sound_TB.Text;
        }
        #endregion Textboxes
        #region Checkboxes
        private void Nav_Settings_Prompt_Before_Processing_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            localPromptBeforeProcessing = Nav_Settings_Prompt_Before_Processing_ChkB.Checked;
        }
        private void Nav_Settings_Sort_Tags_First_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            localSortByTags = Nav_Settings_Sort_Tags_First_ChkB.Checked;
        }
        private void Nav_Settings_Sort_By_Last_Zone_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            localSortByLastZone = Nav_Settings_Sort_By_Last_Zone_ChkB.Checked;
        }
        #endregion Checkboxes
        #endregion Event Handlers
        #region Utility Functions
        private void SaveValues()
        {
            UserSettings.SetValue(UserSettings.BOT.NAV, "Nav_Rec_IntervalDefValue", localIntervalDefValue.ToString());
            Statics.Settings.Navigation.IntervalDefValue = localIntervalDefValue;
            UserSettings.SetValue(UserSettings.BOT.NAV, "Nav_Rec_MinDistDefValue", localMinDistDefValue.ToString());
            Statics.Settings.Navigation.MinDistDefValue = localMinDistDefValue;
            UserSettings.SetValue(UserSettings.BOT.NAV, "Nav_Rec_WaitDefValue", localWaitDefValue.ToString());
            Statics.Settings.Navigation.WaitDefValue = localWaitDefValue;
            UserSettings.SetValue(UserSettings.BOT.NAV, "Nav_TripCompleteSound", localSoundToPlay);
            Statics.Settings.Navigation.TripCompleteSound = localSoundToPlay;
            UserSettings.SetValue(UserSettings.BOT.NAV, "Nav_Prc_PromptOnRightClick", localPromptBeforeProcessing.ToString());
            Statics.Settings.Navigation.PromptOnRightClick = localPromptBeforeProcessing;
            UserSettings.SetValue(UserSettings.BOT.NAV, "Nav_sortingTagsFirst", localSortByTags.ToString());
            Statics.Settings.Navigation.SortingTagsFirst = localSortByTags;
            UserSettings.SetValue(UserSettings.BOT.NAV, "Nav_sortingUseLastZoneOnly", localSortByLastZone.ToString());
            Statics.Settings.Navigation.SortingUseLastZoneOnly = localSortByLastZone;
            UserSettings.SaveSettings();
        }
        #endregion Utility Functions
    }
}