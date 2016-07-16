using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;

using Iocaine2.Properties;
using Iocaine2.Settings;

namespace Iocaine2
{
    //This file is for functions directly related to Fish Stats
    partial class Iocaine_2_Form
    {
        #region Settings
        private ArrayList BaitFilterList = new ArrayList();
        private ArrayList RodFilterList = new ArrayList();
        private int FSFishCBIndex = 0;
        private int FSZoneCBIndex = 0;
        private int FSZoneRodCBIndex = 0;
        private int FSZoneBaitCBIndex = 0;
        private int FSRodCBIndex = 0;
        private int FSBaitCBIndex = 0;
        #endregion Settings
        #region User Settings
        private void UpdateStatsSettings(object sender, StatsSettingsUpdateEventArgs e)
        {
            FSFishCBIndex = e.FishCBIndex;
            FSZoneCBIndex = e.ZoneCBIndex;
            FSZoneRodCBIndex = e.ZoneRodCBIndex;
            FSZoneBaitCBIndex = e.ZoneBaitCBIndex;
            FSRodCBIndex = e.RodCBIndex;
            FSBaitCBIndex = e.BaitCBIndex;
            UserSettings.SetValue(UserSettings.BOT.FISHSTATS, "FishFishComboBox", FSFishCBIndex.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHSTATS, "ZoneZoneComboBox", FSZoneCBIndex.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHSTATS, "ZoneRodComboBox", FSZoneRodCBIndex.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHSTATS, "ZoneBaitComboBox", FSZoneBaitCBIndex.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHSTATS, "RodRodComboBox", FSRodCBIndex.ToString());
            UserSettings.SetValue(UserSettings.BOT.FISHSTATS, "BaitBaitComboBox", FSBaitCBIndex.ToString());
        }
        #endregion User Settings
    }
}