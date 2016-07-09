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
    public partial class Helpers_Settings_Form : Form
    {
        #region Members
        #region Delays
        private UInt32 localEnterNpcMenuDelay = 0;
        private UInt32 localEnterToSellDelay = 0;
        private UInt32 localCheckHelpTextDelay = 0;
        private UInt32 localChangeSellQuanDelay = 0;
        private UInt32 localPressEnterToSellDelay = 0;
        #endregion Delays
        #region Seller Settings
        private String localSL_WhenDoneSound = "";
        private bool localSL_SellFromBagFirst = false;
        private bool localSL_SortBeforeSelling = false;
        #endregion Seller Settings
        #region Buyer Settings
        private String localBY_WhenDoneSound = "";
        private bool localBY_GuildSetIndex = false;
        #endregion Buyer Settings
        #region Misc
        private float delayChangePerc = 0.05f;
        #endregion Misc
        #region Default Values
        private String WhenDonePlaySoundTBDefText = "Text to Say or File to Play.";
        #endregion Default Values
        #endregion Members
        #region Constructor
        public Helpers_Settings_Form(int iStartX, int iStartY)
        {
            InitializeComponent();
            this.SetDesktopLocation(iStartX - (this.Size.Width / 2), iStartY - (this.Size.Height / 2));
            initForm();
        }
        #endregion Constructor
        #region Inits
        private void initForm()
        {
            #region Delays
            localEnterNpcMenuDelay = (UInt32)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_EnterNpcMenuDelay");
            EnterNpcMenuDelayUpDn.Value = (decimal)localEnterNpcMenuDelay;
            localEnterToSellDelay = (UInt32)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_EnterToSellDelay");
            EnterToSellDelayUpDn.Value = (decimal)localEnterToSellDelay;
            localCheckHelpTextDelay = (UInt32)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_CheckHelpTextDelay");
            CheckHelpTextDelayUpDn.Value = (decimal)localCheckHelpTextDelay;
            localChangeSellQuanDelay = (UInt32)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_ChangeSellQuanDelay");
            ChangeSellQuanDelayUpDn.Value = (decimal)localChangeSellQuanDelay;
            localPressEnterToSellDelay = (UInt32)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_PressEnterToSellDelay");
            PressEnterToSellDelayUpDn.Value = (decimal)localPressEnterToSellDelay;
            #endregion Delays
            #region Seller settings
            localSL_WhenDoneSound = (String)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_WhenDoneSound");
            SL_WhenDonePlaySoundTB.Text = localSL_WhenDoneSound;
            localSL_SellFromBagFirst = (bool)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_SellFromBagFirst");
            SL_SellFromBagFirstChkB.Checked = localSL_SellFromBagFirst;
            localSL_SortBeforeSelling = (bool)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_SortBeforeSelling");
            SL_SortBeforeSellingChkB.Checked = localSL_SortBeforeSelling;
            #endregion Seller settings
            #region Buyer settings
            localBY_WhenDoneSound = (String)UserSettings.GetValue(UserSettings.BOT.HELPERS, "BY_WhenDoneSound");
            BY_WhenDonePlaySoundTB.Text = localBY_WhenDoneSound;
            localBY_GuildSetIndex = (bool)UserSettings.GetValue(UserSettings.BOT.HELPERS, "BY_GuildSetIndex");
            BY_GuildSetIndexChkB.Checked = localBY_GuildSetIndex;
            #endregion Buyer settings
        }
        #endregion Inits
        #region Event Handlers
        #region Up/Dns
        #region Delays
        private void EnterNpcMenuDelayUpDn_ValueChanged(object sender, EventArgs e)
        {
            localEnterNpcMenuDelay = (UInt32)EnterNpcMenuDelayUpDn.Value;
        }
        private void EnterToSellDelayUpDn_ValueChanged(object sender, EventArgs e)
        {
            localEnterToSellDelay = (UInt32)EnterToSellDelayUpDn.Value;
        }
        private void CheckHelpTextDelayUpDn_ValueChanged(object sender, EventArgs e)
        {
            localCheckHelpTextDelay = (UInt32)CheckHelpTextDelayUpDn.Value;
        }
        private void ChangeSellQuanDelayUpDn_ValueChanged(object sender, EventArgs e)
        {
            localChangeSellQuanDelay = (UInt32)ChangeSellQuanDelayUpDn.Value;
        }
        private void PressEnterToSellDelayUpDn_ValueChanged(object sender, EventArgs e)
        {
            localPressEnterToSellDelay = (UInt32)PressEnterToSellDelayUpDn.Value;
        }
        #endregion Delays
        #endregion Up/Dns
        #region Buttons
        #region Delays
        private void DelayIncreaseButton_Click(object sender, EventArgs e)
        {
            localEnterNpcMenuDelay = (UInt32)((localEnterNpcMenuDelay * delayChangePerc) + localEnterNpcMenuDelay);
            localEnterToSellDelay = (UInt32)((localEnterToSellDelay * delayChangePerc) + localEnterToSellDelay);
            localCheckHelpTextDelay = (UInt32)((localCheckHelpTextDelay * delayChangePerc) + localCheckHelpTextDelay);
            localChangeSellQuanDelay = (UInt32)((localChangeSellQuanDelay * delayChangePerc) + localChangeSellQuanDelay);
            localPressEnterToSellDelay = (UInt32)((localPressEnterToSellDelay * delayChangePerc) + localPressEnterToSellDelay);
            UpdateDelayUpDnValues();
        }
        private void DelayDecreaseButton_Click(object sender, EventArgs e)
        {
            localEnterNpcMenuDelay = (UInt32)(localEnterNpcMenuDelay - (localEnterNpcMenuDelay * delayChangePerc));
            localEnterToSellDelay = (UInt32)(localEnterToSellDelay - (localEnterToSellDelay * delayChangePerc));
            localCheckHelpTextDelay = (UInt32)(localCheckHelpTextDelay - (localCheckHelpTextDelay * delayChangePerc));
            localChangeSellQuanDelay = (UInt32)(localChangeSellQuanDelay - (localChangeSellQuanDelay * delayChangePerc));
            localPressEnterToSellDelay = (UInt32)(localPressEnterToSellDelay - (localPressEnterToSellDelay * delayChangePerc));
            UpdateDelayUpDnValues();
        }
        private void LoadDefaultDelaysButton_Click(object sender, EventArgs e)
        {
            LoadDelayDefaultValues();
            UpdateDelayUpDnValues();
        }
        #endregion Delays
        #region Seller
        private void SL_WhenDoneBrowseFileToPlayButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog DoneOpenFileToPlay = new OpenFileDialog();
            DoneOpenFileToPlay.FileName = "FileToPlay";
            DoneOpenFileToPlay.Filter = "Waveform Audio File (*.wav) |*.wav;";
            DoneOpenFileToPlay.Multiselect = false;
            DoneOpenFileToPlay.ShowDialog();
            if (DoneOpenFileToPlay.FileName != "FileToPlay")
            {
                SL_WhenDonePlaySoundTB.Text = DoneOpenFileToPlay.FileName;
            }
        }
        #endregion Seller
        #region Buyer
        private void BY_WhenDoneBrowseFileToPlayButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog DoneOpenFileToPlay = new OpenFileDialog();
            DoneOpenFileToPlay.FileName = "FileToPlay";
            DoneOpenFileToPlay.Filter = "Waveform Audio File (*.wav) |*.wav;";
            DoneOpenFileToPlay.Multiselect = false;
            DoneOpenFileToPlay.ShowDialog();
            if (DoneOpenFileToPlay.FileName != "FileToPlay")
            {
                BY_WhenDonePlaySoundTB.Text = DoneOpenFileToPlay.FileName;
            }
        }
        #endregion Buyer
        #region Top
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
        #endregion Top
        #endregion Buttons
        #region Textboxes
        #region Seller Done Play Sound TB
        private void SL_WhenDonePlaySoundTB_TextChanged(object sender, EventArgs e)
        {
            if ((SL_WhenDonePlaySoundTB.Text != "") && (SL_WhenDonePlaySoundTB.Text != WhenDonePlaySoundTBDefText))
            {
                localSL_WhenDoneSound = SL_WhenDonePlaySoundTB.Text;
            }
        }
        private void SL_WhenDonePlaySoundTB_Enter(object sender, EventArgs e)
        {
            if (SL_WhenDonePlaySoundTB.Text == WhenDonePlaySoundTBDefText)
            {
                SL_WhenDonePlaySoundTB.Text = "";
                SL_WhenDonePlaySoundTB.ForeColor = Color.Black;
            }
        }
        private void SL_WhenDonePlaySoundTB_Leave(object sender, EventArgs e)
        {
            if (SL_WhenDonePlaySoundTB.Text == "")
            {
                SL_WhenDonePlaySoundTB.Text = WhenDonePlaySoundTBDefText;
                SL_WhenDonePlaySoundTB.ForeColor = Color.Gray;
            }
        }
        #endregion Seller Done Play Sound TB
        #region Buyer Done Play Sound TB
        private void BY_WhenDonePlaySoundTB_TextChanged(object sender, EventArgs e)
        {
            if ((BY_WhenDonePlaySoundTB.Text != "") && (BY_WhenDonePlaySoundTB.Text != WhenDonePlaySoundTBDefText))
            {
                localBY_WhenDoneSound = BY_WhenDonePlaySoundTB.Text;
            }
        }
        private void BY_WhenDonePlaySoundTB_Enter(object sender, EventArgs e)
        {
            if (BY_WhenDonePlaySoundTB.Text == WhenDonePlaySoundTBDefText)
            {
                BY_WhenDonePlaySoundTB.Text = "";
                BY_WhenDonePlaySoundTB.ForeColor = Color.Black;
            }
        }
        private void BY_WhenDonePlaySoundTB_Leave(object sender, EventArgs e)
        {
            if (BY_WhenDonePlaySoundTB.Text == "")
            {
                BY_WhenDonePlaySoundTB.Text = WhenDonePlaySoundTBDefText;
                BY_WhenDonePlaySoundTB.ForeColor = Color.Gray;
            }
        }
        #endregion Buyer Done Play Sound TB
        #endregion Textboxes
        #region Checkboxes
        #region Seller
        private void SellFromBagFirstChkB_CheckedChanged(object sender, EventArgs e)
        {
            localSL_SellFromBagFirst = SL_SellFromBagFirstChkB.Checked;
        }
        private void SortBeforeSellingChkB_CheckedChanged(object sender, EventArgs e)
        {
            localSL_SortBeforeSelling = SL_SortBeforeSellingChkB.Checked;
        }
        #endregion Seller
        #region Buyer
        private void BY_GuildSetIndexChkB_CheckedChanged(object sender, EventArgs e)
        {
            localBY_GuildSetIndex = BY_GuildSetIndexChkB.Checked;
        }
        #endregion Buyer
        #endregion Checkboxes
        #endregion Event Handlers
        #region Utility Functions
        private void SaveValues()
        {
            #region Delays
            UserSettings.SetValue(UserSettings.BOT.HELPERS, "SL_EnterNpcMenuDelay", localEnterNpcMenuDelay.ToString());
            Statics.Settings.Helpers.EnterNpcMenuDelay = localEnterNpcMenuDelay;
            UserSettings.SetValue(UserSettings.BOT.HELPERS, "SL_EnterToSellDelay", localEnterToSellDelay.ToString());
            Statics.Settings.Helpers.EnterToSellDelay = localEnterToSellDelay;
            UserSettings.SetValue(UserSettings.BOT.HELPERS, "SL_CheckHelpTextDelay", localCheckHelpTextDelay.ToString());
            Statics.Settings.Helpers.CheckHelpTextDelay = localCheckHelpTextDelay;
            UserSettings.SetValue(UserSettings.BOT.HELPERS, "SL_ChangeSellQuanDelay", localChangeSellQuanDelay.ToString());
            Statics.Settings.Helpers.ChangeSellQuanDelay = localChangeSellQuanDelay;
            UserSettings.SetValue(UserSettings.BOT.HELPERS, "SL_PressEnterToSellDelay", localPressEnterToSellDelay.ToString());
            Statics.Settings.Helpers.PressEnterToSellDelay = localPressEnterToSellDelay;
            #endregion Delays
            #region Seller
            UserSettings.SetValue(UserSettings.BOT.HELPERS, "SL_WhenDoneSound", localSL_WhenDoneSound);
            Statics.Settings.Helpers.WhenDoneSound_Seller = localSL_WhenDoneSound;
            UserSettings.SetValue(UserSettings.BOT.HELPERS, "SL_SellFromBagFirst", localSL_SellFromBagFirst.ToString());
            Statics.Settings.Helpers.SellFromBagFirst = localSL_SellFromBagFirst;
            UserSettings.SetValue(UserSettings.BOT.HELPERS, "SL_SortBeforeSelling", localSL_SortBeforeSelling.ToString());
            Statics.Settings.Helpers.SortBeforeSelling = localSL_SortBeforeSelling;
            #endregion Seller
            #region Buyer
            UserSettings.SetValue(UserSettings.BOT.HELPERS, "BY_WhenDoneSound", localBY_WhenDoneSound);
            Statics.Settings.Helpers.WhenDoneSound_Buyer = localBY_WhenDoneSound;
            UserSettings.SetValue(UserSettings.BOT.HELPERS, "BY_GuildSetIndex", localBY_GuildSetIndex.ToString());
            Statics.Settings.Helpers.GuildSetIndex_Buyer = localBY_GuildSetIndex;
            #endregion Buyer
            UserSettings.SaveSettings();
        }
        private void CheckDelayValues()
        {
            if (localEnterNpcMenuDelay < (UInt32)EnterNpcMenuDelayUpDn.Minimum)
            {
                localEnterNpcMenuDelay = (UInt32)EnterNpcMenuDelayUpDn.Minimum;
            }
            else if (localEnterNpcMenuDelay > (UInt32)EnterNpcMenuDelayUpDn.Maximum)
            {
                localEnterNpcMenuDelay = (UInt32)EnterNpcMenuDelayUpDn.Maximum;
            }
            if (localEnterToSellDelay < (UInt32)EnterToSellDelayUpDn.Minimum)
            {
                localEnterToSellDelay = (UInt32)EnterToSellDelayUpDn.Minimum;
            }
            else if (localEnterToSellDelay > (UInt32)EnterToSellDelayUpDn.Maximum)
            {
                localEnterToSellDelay = (UInt32)EnterToSellDelayUpDn.Maximum;
            }
            if (localCheckHelpTextDelay < (UInt32)CheckHelpTextDelayUpDn.Minimum)
            {
                localCheckHelpTextDelay = (UInt32)CheckHelpTextDelayUpDn.Minimum;
            }
            else if (localCheckHelpTextDelay > (UInt32)CheckHelpTextDelayUpDn.Maximum)
            {
                localCheckHelpTextDelay = (UInt32)CheckHelpTextDelayUpDn.Maximum;
            }
            if (localChangeSellQuanDelay < (UInt32)ChangeSellQuanDelayUpDn.Minimum)
            {
                localChangeSellQuanDelay = (UInt32)ChangeSellQuanDelayUpDn.Minimum;
            }
            else if (localChangeSellQuanDelay > (UInt32)ChangeSellQuanDelayUpDn.Maximum)
            {
                localChangeSellQuanDelay = (UInt32)ChangeSellQuanDelayUpDn.Maximum;
            }
            if (localPressEnterToSellDelay < (UInt32)PressEnterToSellDelayUpDn.Minimum)
            {
                localPressEnterToSellDelay = (UInt32)PressEnterToSellDelayUpDn.Minimum;
            }
            else if (localPressEnterToSellDelay > (UInt32)PressEnterToSellDelayUpDn.Maximum)
            {
                localPressEnterToSellDelay = (UInt32)PressEnterToSellDelayUpDn.Maximum;
            }
        }
        private void UpdateDelayUpDnValues()
        {
            CheckDelayValues();
            EnterNpcMenuDelayUpDn.Value = (decimal)localEnterNpcMenuDelay;
            EnterToSellDelayUpDn.Value = (decimal)localEnterToSellDelay;
            CheckHelpTextDelayUpDn.Value = (decimal)localCheckHelpTextDelay;
            ChangeSellQuanDelayUpDn.Value = (decimal)localChangeSellQuanDelay;
            PressEnterToSellDelayUpDn.Value = (decimal)localPressEnterToSellDelay;
        }
        private void LoadDelayDefaultValues()
        {
            localEnterNpcMenuDelay = (UInt32)UserSettings.GetDefaultValue(UserSettings.BOT.HELPERS, "SL_EnterNpcMenuDelay");
            localEnterToSellDelay = (UInt32)UserSettings.GetDefaultValue(UserSettings.BOT.HELPERS, "SL_EnterToSellDelay");
            localCheckHelpTextDelay = (UInt32)UserSettings.GetDefaultValue(UserSettings.BOT.HELPERS, "SL_CheckHelpTextDelay");
            localChangeSellQuanDelay = (UInt32)UserSettings.GetDefaultValue(UserSettings.BOT.HELPERS, "SL_ChangeSellQuanDelay");
            localPressEnterToSellDelay = (UInt32)UserSettings.GetDefaultValue(UserSettings.BOT.HELPERS, "SL_PressEnterToSellDelay");
        }
        #endregion Utility Functions
    }
}
