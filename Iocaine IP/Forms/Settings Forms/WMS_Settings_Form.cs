using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Settings;

namespace Iocaine2
{
    public partial class WMS_Settings_Form : Form
    {
        #region Members
        WMSDataSet dataset;
        #endregion Members
        #region Inits
        public WMS_Settings_Form(int iStartX, int iStartY, WMSDataSet iDataset)
        {
            InitializeComponent();
            this.SetDesktopLocation(iStartX - (this.Size.Width / 2), iStartY - (this.Size.Height / 2));
            dataset = iDataset;
            init_form();
        }
        private void init_form()
        {
            ContinuousScanChkB.Checked = Statics.Settings.WMS.BackgroundUpdate;
            ScanPeriodUpDown.Value = Statics.Settings.WMS.BackgroundScanPeriod;
            SaveCharOfflineChkB.Checked = Statics.Settings.WMS.SaveThisCharOffline;
            WMSDataSet.CharacterInfoRow[] charRows = (WMSDataSet.CharacterInfoRow[])dataset.CharacterInfo.Select();
            foreach (WMSDataSet.CharacterInfoRow row in charRows)
            {
                CharacterCB.Items.Add(row.Name);
            }
            if (CharacterCB.Items.Count > 0)
            {
                CharacterCB.SelectedIndex = 0;
            }
        }
        #endregion Inits
        #region Event Handlers
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void Apply_Button_Click(object sender, EventArgs e)
        {
            Statics.Settings.WMS.BackgroundUpdate = ContinuousScanChkB.Checked;
            UserSettings.SetValue(UserSettings.BOT.WMS, "WMSBackgroundUpdate", Statics.Settings.WMS.BackgroundUpdate.ToString());
            Statics.Settings.WMS.BackgroundScanPeriod = (uint)ScanPeriodUpDown.Value;
            UserSettings.SetValue(UserSettings.BOT.WMS, "WMSBackgroundScanPeriod", Statics.Settings.WMS.BackgroundScanPeriod.ToString());
            Statics.Settings.WMS.SaveThisCharOffline = SaveCharOfflineChkB.Checked;
        }
        private void OK_Button_Click(object sender, EventArgs e)
        {
            Apply_Button_Click(sender, e);
            this.Dispose();
        }
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if ((CharacterCB.Items.Count > 0) && (CharacterCB.Text != ""))
            {
                WMSDataSet.ItemsRow[] itemRows = (WMSDataSet.ItemsRow[])dataset.Items.Select("Character='" + CharacterCB.Text + "'");
                foreach (WMSDataSet.ItemsRow row in itemRows)
                {
                    row.Delete();
                }
                WMSDataSet.CharacterInfoRow[] charRows = (WMSDataSet.CharacterInfoRow[])dataset.CharacterInfo.Select("Name='" + CharacterCB.Text + "'");
                if (charRows.Length > 0)
                {
                    dataset.CharacterInfo.Rows.Remove(charRows[0]);
                }
                dataset.AcceptChanges();
            }
        }
        #endregion Event Handlers
    }
}
