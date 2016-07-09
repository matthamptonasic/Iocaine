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
    public partial class Fisher_Settings_Form : Form
    {
        #region Events/Delegates
        public delegate void DataUpdateHandler();
        public event DataUpdateHandler DataUpdated;
        #endregion Events/Delegates
        #region Members
        private List<String> DoneActionStrings = new List<string>() { "Wait For Next Day", "Stop", "Shutdown", "Run Navigation Trip" };
        private List<String> FullActionStrings = new List<string>() { "Stop", "Shutdown", "Continue Fishing", "Run Navigation Trip" };
        #endregion Members
        #region Inits
        public Fisher_Settings_Form(
            int iStartX,
            int iStartY,
            List<String> NavTrips
            )
        {
            InitializeComponent();
            this.SetDesktopLocation(iStartX - (this.Size.Width / 2), iStartY - (this.Size.Height / 2));
            KillFishCheckBox.Checked = Statics.Settings.Fisher.KillFish;
            FixedTimeRadioButton.Checked = Statics.Settings.Fisher.KillFishFixed;
            FixedTimeTextBox.Text = Statics.Settings.Fisher.KillFishFixedTime.ToString();
            RandTimeRadioButton.Checked = Statics.Settings.Fisher.KillFishRand;
            RandTimeMinTextBox.Text = Statics.Settings.Fisher.KillFishRandTimeMin.ToString();
            RandTimeMaxTextBox.Text = Statics.Settings.Fisher.KillFishRandTimeMax.ToString();
            PropTimeRadioButton.Checked = Statics.Settings.Fisher.KillFishProp;
            PropTimeMinTextBox.Text = Statics.Settings.Fisher.KillFishPropTimeMin.ToString();
            PropTimeMaxTextBox.Text = Statics.Settings.Fisher.KillFishPropTimeMax.ToString();
            DoneActionCB.Items.AddRange(DoneActionStrings.ToArray());
            if (Statics.Settings.Fisher.DoneWait)
            {
                DoneActionCB.SelectedItem = "Wait For Next Day";
            }
            else if (Statics.Settings.Fisher.DoneStop)
            {
                DoneActionCB.SelectedItem = "Stop";
            }
            else if (Statics.Settings.Fisher.DoneShutdown)
            {
                DoneActionCB.SelectedItem = "Shutdown";
            }
            else if (Statics.Settings.Fisher.DoneNav)
            {
                DoneActionCB.SelectedItem = "Run Navigation Trip";
            }
            else
            {
                DoneActionCB.SelectedItem = "Stop";
            }
            DoneTripCB.Items.AddRange(NavTrips.ToArray());
            if (DoneTripCB.Items.Contains(Statics.Settings.Fisher.DoneNavTrip))
            {
                DoneTripCB.SelectedItem = Statics.Settings.Fisher.DoneNavTrip;
            }
            FullActionCB.Items.AddRange(FullActionStrings.ToArray());
            if (Statics.Settings.Fisher.FullStop)
            {
                FullActionCB.SelectedItem = "Stop";
            }
            else if (Statics.Settings.Fisher.FullShutdown)
            {
                FullActionCB.SelectedItem = "Shutdown";
            }
            else if (Statics.Settings.Fisher.FullContinue)
            {
                FullActionCB.SelectedItem = "Continue Fishing";
            }
            else if (Statics.Settings.Fisher.FullNav)
            {
                FullActionCB.SelectedItem = "Run Navigation Trip";
            }
            else
            {
                FullActionCB.SelectedItem = "Stop";
            }
            FullTripCB.Items.AddRange(NavTrips.ToArray());
            if (FullTripCB.Items.Contains(Statics.Settings.Fisher.FullNavTrip))
            {
                FullTripCB.SelectedItem = Statics.Settings.Fisher.FullNavTrip;
            }
            FatiguedNavChkB.Checked = Statics.Settings.Fisher.FatiguedNav;
            FatigueThresholdUpDn.Value = (decimal)Statics.Settings.Fisher.FatigueThreshold;
            FatiguedTripCB.Items.AddRange(NavTrips.ToArray());
            if (FatiguedTripCB.Items.Contains(Statics.Settings.Fisher.FatiguedNavTrip))
            {
                FatiguedTripCB.SelectedItem = Statics.Settings.Fisher.FatiguedNavTrip;
            }
            GiveCommandCheckBox.Checked = Statics.Settings.Fisher.GiveLogoutCommand;
            CommandTextBox.Text = Statics.Settings.Fisher.LogoutCommand;
            FishByIDRadioButton.Checked = Statics.Settings.Fisher.FishByID;
            FishByHPRadioButton.Checked = Statics.Settings.Fisher.FishByHP;
            DropItemsCheckBox.Checked = Statics.Settings.Fisher.DropItems;
            DropMobsCheckBox.Checked = Statics.Settings.Fisher.DropMobs;
            FishByHPMinTextBox.Text = Statics.Settings.Fisher.FishByHPMin.ToString();
            FishByHPMaxTextBox.Text = Statics.Settings.Fisher.FishByHPMax.ToString();
            NoCatchForm.Text = Statics.Settings.Fisher.NoCatchTimeout.ToString();
            FixedReleaseRadioButton.Checked = Statics.Settings.Fisher.ReleaseFixed;
            FixedReleaseTextBox.Text = Statics.Settings.Fisher.ReleaseTime.ToString();
            RandomReleaseRadioButton.Checked = Statics.Settings.Fisher.ReleaseRandom;
            RandomReleaseMinTextBox.Text = Statics.Settings.Fisher.ReleaseTimeRandomMin.ToString();
            RandomReleaseMaxTextBox.Text = Statics.Settings.Fisher.ReleaseTimeRandomMax.ToString();
            TimedStartCheckBox.Checked = Statics.Settings.Fisher.TimedStart;
            TimedStartTextBox.Text = Statics.Settings.Fisher.StartTime.ToShortTimeString();
            TimedEndCheckBox.Checked = Statics.Settings.Fisher.TimedEnd;
            TimedEndTextBox.Text = Statics.Settings.Fisher.EndTime.ToShortTimeString();
            if (Statics.Settings.Fisher.FisherDonePlaySound == "")
            {
                DonePlaySoundTB.Text = "None";
            }
            else
            {
                DonePlaySoundTB.Text = Statics.Settings.Fisher.FisherDonePlaySound;
            }
            if (Statics.Settings.Fisher.FisherFullPlaySound == "")
            {
                WhenFullPlaySoundTB.Text = "None";
            }
            else
            {
                WhenFullPlaySoundTB.Text = Statics.Settings.Fisher.FisherFullPlaySound;
            }
            InvMovementChkBox.Checked = Statics.Settings.Fisher.MoveInv;
            Logging.LoggingFunctions.Timestamp("Recast is " + Statics.Settings.Fisher.RecastDelay + ".");
            RecastDelayUpDn.Value = Convert.ToDecimal(Statics.Settings.Fisher.RecastDelay);
            Statics.Settings.Fisher.RecastDelay = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHER, "InitWaitDelay"));
        }
        #endregion Inits
        #region GUI Event Handlers
        private void OK_Button_Click(object sender, EventArgs e)
        {
            Apply_Button_Click(sender, e);
            this.Dispose();
        }
        private void Apply_Button_Click(object sender, EventArgs e)
        {
            Statics.Settings.Fisher.KillFish = KillFishCheckBox.Checked;
            Statics.Settings.Fisher.KillFishFixed =  FixedTimeRadioButton.Checked;
            Statics.Settings.Fisher.KillFishFixedTime = System.Convert.ToInt32(FixedTimeTextBox.Text);
            Statics.Settings.Fisher.KillFishRand = RandTimeRadioButton.Checked;
            Statics.Settings.Fisher.KillFishRandTimeMin = System.Convert.ToInt32(RandTimeMinTextBox.Text);
            Statics.Settings.Fisher.KillFishRandTimeMax = System.Convert.ToInt32(RandTimeMaxTextBox.Text);
            Statics.Settings.Fisher.KillFishProp = PropTimeRadioButton.Checked;
            Statics.Settings.Fisher.KillFishPropTimeMin = System.Convert.ToInt32(PropTimeMinTextBox.Text);
            Statics.Settings.Fisher.KillFishPropTimeMax = System.Convert.ToInt32(PropTimeMaxTextBox.Text);
            Statics.Settings.Fisher.DoneWait = ((String)DoneActionCB.SelectedItem == "Wait For Next Day");
            Statics.Settings.Fisher.DoneStop = ((String)DoneActionCB.SelectedItem == "Stop");
            Statics.Settings.Fisher.DoneShutdown = ((String)DoneActionCB.SelectedItem == "Shutdown");
            Statics.Settings.Fisher.DoneChange = false;
            Statics.Settings.Fisher.DoneNav = ((String)DoneActionCB.SelectedItem == "Run Navigation Trip");
            Statics.Settings.Fisher.DoneNavTrip = (DoneTripCB.SelectedIndex >= 0) ? DoneTripCB.SelectedItem.ToString() : "";
            Statics.Settings.Fisher.FullStop = ((String)FullActionCB.SelectedItem == "Stop");
            Statics.Settings.Fisher.FullShutdown = ((String)FullActionCB.SelectedItem == "Shutdown");
            Statics.Settings.Fisher.FullChange = false;
            Statics.Settings.Fisher.FullContinue = ((String)FullActionCB.SelectedItem == "Continue Fishing");
            Statics.Settings.Fisher.FullNav = ((String)FullActionCB.SelectedItem == "Run Navigation Trip");
            Statics.Settings.Fisher.FullNavTrip = (FullTripCB.SelectedIndex >= 0) ? FullTripCB.SelectedItem.ToString() : "";
            Statics.Settings.Fisher.FatiguedNav = FatiguedNavChkB.Checked;
            Statics.Settings.Fisher.FatiguedNavTrip = (FatiguedTripCB.SelectedIndex >= 0) ? FatiguedTripCB.SelectedItem.ToString() : "";
            Statics.Settings.Fisher.FatigueThreshold = (ushort)FatigueThresholdUpDn.Value;
            Statics.Settings.Fisher.GiveLogoutCommand = GiveCommandCheckBox.Checked;
            Statics.Settings.Fisher.LogoutCommand = CommandTextBox.Text;
            Statics.Settings.Fisher.FishByID = FishByIDRadioButton.Checked;
            Statics.Settings.Fisher.FishByHP = FishByHPRadioButton.Checked;
            Statics.Settings.Fisher.DropItems = DropItemsCheckBox.Checked;
            Statics.Settings.Fisher.DropMobs = DropMobsCheckBox.Checked;
            Statics.Settings.Fisher.FishByHPMin = System.Convert.ToInt32(FishByHPMinTextBox.Text);
            Statics.Settings.Fisher.FishByHPMax = System.Convert.ToInt32(FishByHPMaxTextBox.Text);
            Statics.Settings.Fisher.NoCatchTimeout = System.Convert.ToInt32(NoCatchForm.Text);
            Statics.Settings.Fisher.ReleaseFixed = FixedReleaseRadioButton.Checked;
            Statics.Settings.Fisher.ReleaseTime = System.Convert.ToDouble(FixedReleaseTextBox.Text);
            Statics.Settings.Fisher.ReleaseRandom = RandomReleaseRadioButton.Checked;
            Statics.Settings.Fisher.ReleaseTimeRandomMin = System.Convert.ToDouble(RandomReleaseMinTextBox.Text);
            Statics.Settings.Fisher.ReleaseTimeRandomMax = System.Convert.ToDouble(RandomReleaseMaxTextBox.Text);
            Statics.Settings.Fisher.TimedStart = TimedStartCheckBox.Checked;
            Statics.Settings.Fisher.StartTime = parseTimeString(TimedStartTextBox.Text);
            Statics.Settings.Fisher.TimedEnd = TimedEndCheckBox.Checked;
            Statics.Settings.Fisher.EndTime = parseTimeString(TimedEndTextBox.Text);
            Statics.Settings.Fisher.MoveInv = InvMovementChkBox.Checked;

            //System.Console.WriteLine("Firing update event with dropItems {0} and dropMobs {1}", DropItemsCheckBox.Checked, DropMobsCheckBox.Checked);
            if (DonePlaySoundTB.Text == "")
            {
                Statics.Settings.Fisher.FisherDonePlaySound = "None";
            }
            else
            {
                Statics.Settings.Fisher.FisherDonePlaySound = DonePlaySoundTB.Text;
            }
            if (WhenFullPlaySoundTB.Text == "")
            {
                Statics.Settings.Fisher.FisherFullPlaySound = "None";
            }
            else
            {
                Statics.Settings.Fisher.FisherFullPlaySound = WhenFullPlaySoundTB.Text;
            }
            DataUpdated();
            Statics.Settings.Fisher.RecastDelay = Convert.ToInt32(RecastDelayUpDn.Value);
            UserSettings.SetValue(UserSettings.BOT.FISHER, "InitWaitDelay", RecastDelayUpDn.Value.ToString());
            Logging.LoggingFunctions.Timestamp("Saving recast of " + Statics.Settings.Fisher.RecastDelay.ToString());
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void DoneBrowseFileToPlayButton_Click(object sender, EventArgs e)
        {
            DoneOpenFileToPlay.Filter = "Waveform Audio File (*.wav) |*.wav;";
            DoneOpenFileToPlay.Multiselect = false;
            DoneOpenFileToPlay.ShowDialog();
            if (DoneOpenFileToPlay.FileName != "DoneOpenFileToPlay")
            {
                DonePlaySoundTB.Text = DoneOpenFileToPlay.FileName;
            }
        }
        private void WhenFullBrowseFileToPlayButton_Click(object sender, EventArgs e)
        {
            WhenFullOpenFileToPlay.Filter = "Waveform Audio File (*.wav) |*.wav;";
            WhenFullOpenFileToPlay.Multiselect = false;
            WhenFullOpenFileToPlay.ShowDialog();
            if (WhenFullOpenFileToPlay.FileName != "WhenFullOpenFileToPlay")
            {
                WhenFullPlaySoundTB.Text = WhenFullOpenFileToPlay.FileName;
            }
        }
        #endregion GUI Event Handlers
        #region Utility Functions
        private DateTime parseTimeString(String timeString)
        {
            DateTime parsedTime;
            uint hourNb;
            uint minuteNb;
            if (timeString.Contains(':'))
            {
                int colonIndex = timeString.IndexOf(':');
                String hour = timeString.Substring(0, colonIndex);
                hour.Trim();
                String minute = timeString.Substring(colonIndex + 1, 2);
                minute.Trim();
                bool isPm = false;
                hourNb = Convert.ToUInt32(hour);
                minuteNb = Convert.ToUInt32(minute);
                if (hourNb > 23)
                {
                    hourNb = 23;
                    isPm = true;
                }
                else if (hourNb > 11)
                {
                    isPm = true;
                }
                else
                {
                    isPm = timeString.Contains('p') || timeString.Contains('P');
                    if (isPm)
                    {
                        hourNb += 12;
                    }
                }
                if (minuteNb > 59)
                {
                    minuteNb %= 60;
                }
            }
            else
            {
                int timeBeginIndex = 0;
                int timeEndIndex = 0;
                bool isPm = false;
                char[] searchValues = "0123456789".ToCharArray();
                timeBeginIndex = timeString.IndexOfAny(searchValues);
                timeEndIndex = timeString.LastIndexOfAny(searchValues);
                String tempTimeString = timeString.Substring(timeBeginIndex, timeEndIndex - timeBeginIndex + 1);
                if (tempTimeString.Length == 4)
                {
                    String hourSubString = tempTimeString.Substring(0, 2);
                    String minuteSubString = tempTimeString.Substring(2, 2);
                    hourNb = Convert.ToUInt32(hourSubString);
                    minuteNb = Convert.ToUInt32(minuteSubString);
                }
                else if (tempTimeString.Length == 3)
                {
                    String hourSubString = tempTimeString.Substring(0, 1);
                    String minuteSubString = tempTimeString.Substring(1, 2);
                    hourNb = Convert.ToUInt32(hourSubString);
                    minuteNb = Convert.ToUInt32(minuteSubString);
                }
                else
                {
                    hourNb = 8;
                    minuteNb = 0;
                }
                if (hourNb > 23)
                {
                    hourNb = hourNb % 24;
                    isPm = (hourNb > 11);
                }
                else if (hourNb > 11)
                {
                    isPm = true;
                }
                else
                {
                    isPm = timeString.Contains('p') || timeString.Contains('P');
                    if (isPm)
                    {
                        hourNb += 12;
                    }
                }
                if (minuteNb > 59)
                {
                    minuteNb %= 60;
                }
            }
            parsedTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, (int)hourNb, (int)minuteNb, 0);
            return parsedTime;
        }
        #endregion Utility Functions
    }

    public class DataUpdateEventArgs : System.EventArgs
    {
        private bool evKillFish;
        private bool evKillFishFixed;
        private int evKillFishFixedTime;
        private bool evKillFishRand;
        private int evKillFishRandTimeMin;
        private int evKillFishRandTimeMax;
        private bool evKillFishProp;
        private int evKillFishPropTimeMin;
        private int evKillFishPropTimeMax;
        private bool evDoneWait;
        private bool evDoneStop;
        private bool evDoneShutdown;
        private bool evDoneChange;
        private bool evDoneNav;
        private String evDoneNavTrip;
        private bool evFullStop;
        private bool evFullShutdown;
        private bool evFullChange;
        private bool evFullContinue;
        private bool evFullNav;
        private String evFullNavTrip;
        private bool evFatiguedNav;
        private String evFatiguedNavTrip;
        private ushort evFatigueThreshold;
        private bool evGiveLogoutCommand;
        private String evLogoutCommand;
        private bool evFishByID;
        private bool evFishByHP;
        private bool evDropItems;
        private bool evDropMobs;
        private int evFishByHPMin;
        private int evFishByHPMax;
        private int evNoCatchTimeout;
        private bool evReleaseFixed;
        private double evReleaseTime;
        private bool evReleaseRandom;
        private double evReleaseTimeRandomMin;
        private double evReleaseTimeRandomMax;
        private bool evTimedStart;
        private DateTime evStartTime;
        private bool evTimedEnd;
        private DateTime evEndTime;
        private bool evMoveInv;

        public DataUpdateEventArgs(
            bool KillFish,
            bool KillFishFixed,
            int KillFishFixedTime,
            bool KillFishRand,
            int KillFishRandTimeMin,
            int KillFishRandTimeMax,
            bool KillFishProp,
            int KillFishPropTimeMin,
            int KillFishPropTimeMax,
            bool DoneWait,
            bool DoneStop,
            bool DoneShutdown,
            bool DoneChange,
            bool DoneNav,
            String DoneNavTrip,
            bool FullStop,
            bool FullShutdown,
            bool FullChange,
            bool FullContinue,
            bool FullNav,
            String FullNavTrip,
            bool FatiguedNav,
            String FatiguedNavTrip,
            ushort FatigueThreshold,
            bool GiveLogoutCommand,
            String LogoutCommand,
            bool FishByID,
            bool FishByHP,
            bool DropItems,
            bool DropMobs,
            int FishByHPMin,
            int FishByHPMax,
            int NoCatchTimeout,
            bool ReleaseFixed,
            double ReleaseTime,
            bool ReleaseRandom,
            double ReleaseTimeRandomMin,
            double ReleaseTimeRandomMax,
            bool TimedStart,
            DateTime StartTime,
            bool TimedEnd,
            DateTime EndTime,
            bool MoveInv
            )
        {
            evKillFish = KillFish;
            evKillFishFixed = KillFishFixed;
            evKillFishFixedTime = KillFishFixedTime;
            evKillFishRand = KillFishRand;
            evKillFishRandTimeMin = KillFishRandTimeMin;
            evKillFishRandTimeMax = KillFishRandTimeMax;
            evKillFishProp = KillFishProp;
            evKillFishPropTimeMin = KillFishPropTimeMin;
            evKillFishPropTimeMax = KillFishPropTimeMax;
            evDoneWait = DoneWait;
            evDoneStop = DoneStop;
            evDoneShutdown = DoneShutdown;
            evDoneChange = DoneChange;
            evDoneNav = DoneNav;
            evDoneNavTrip = DoneNavTrip;
            evFullStop = FullStop;
            evFullShutdown = FullShutdown;
            evFullChange = FullChange;
            evFullContinue = FullContinue;
            evFullNav = FullNav;
            evFullNavTrip = FullNavTrip;
            evFatiguedNav = FatiguedNav;
            evFatiguedNavTrip = FatiguedNavTrip;
            evFatigueThreshold = FatigueThreshold;
            evGiveLogoutCommand = GiveLogoutCommand;
            evLogoutCommand = LogoutCommand;
            evFishByID = FishByID;
            evFishByHP = FishByHP;
            evDropItems = DropItems;
            evDropMobs = DropMobs;
            evFishByHPMin = FishByHPMin;
            evFishByHPMax = FishByHPMax;
            evNoCatchTimeout = NoCatchTimeout;
            evReleaseFixed = ReleaseFixed;
            evReleaseTime = ReleaseTime;
            evReleaseRandom = ReleaseRandom;
            evReleaseTimeRandomMin = ReleaseTimeRandomMin;
            evReleaseTimeRandomMax = ReleaseTimeRandomMax;
            evTimedStart = TimedStart;
            evStartTime = StartTime;
            evTimedEnd = TimedEnd;
            evEndTime = EndTime;
            evMoveInv = MoveInv;
        }
        public bool killFish
        {
            get
            {
                return evKillFish;
            }
        }
        public bool killFishFixed
        {
            get
            {
                return evKillFishFixed;
            }
        }
        public int killFishFixedTime
        {
            get
            {
                return evKillFishFixedTime;
            }
        }
        public bool killFishRand
        {
            get
            {
                return evKillFishRand;
            }
        }
        public int killFishRandTimeMin
        {
            get
            {
                return evKillFishRandTimeMin;
            }
        }
        public int killFishRandTimeMax
        {
            get
            {
                return evKillFishRandTimeMax;
            }
        }
        public bool killFishProp
        {
            get
            {
                return evKillFishProp;
            }
        }
        public int killFishPropTimeMin
        {
            get
            {
                return evKillFishPropTimeMin;
            }
        }
        public int killFishPropTimeMax
        {
            get
            {
                return evKillFishPropTimeMax;
            }
        }
        public bool doneWait
        {
            get
            {
                return evDoneWait;
            }
        }
        public bool doneStop
        {
            get
            {
                return evDoneStop;
            }
        }
        public bool doneShutdown
        {
            get
            {
                return evDoneShutdown;
            }
        }
        public bool doneChange
        {
            get
            {
                return evDoneChange;
            }
        }
        public bool doneNav
        {
            get
            {
                return evDoneNav;
            }
        }
        public String doneNavTrip
        {
            get
            {
                return evDoneNavTrip;
            }
        }
        public bool fullStop
        {
            get
            {
                return evFullStop;
            }
        }
        public bool fullShutdown
        {
            get
            {
                return evFullShutdown;
            }
        }
        public bool fullChange
        {
            get
            {
                return evFullChange;
            }
        }
        public bool fullContinue
        {
            get
            {
                return evFullContinue;
            }
        }
        public bool fullNav
        {
            get
            {
                return evFullNav;
            }
        }
        public String fullNavTrip
        {
            get
            {
                return evFullNavTrip;
            }
        }
        public bool fatiguedNav
        {
            get
            {
                return evFatiguedNav;
            }
        }
        public String fatiguedNavTrip
        {
            get
            {
                return evFatiguedNavTrip;
            }
        }
        public ushort fatigueThreshold
        {
            get
            {
                return evFatigueThreshold;
            }
        }
        public bool giveLogoutCommand
        {
            get
            {
                return evGiveLogoutCommand;
            }
        }
        public String logoutCommand
        {
            get
            {
                return evLogoutCommand;
            }
        }
        public bool fishByID
        {
            get
            {
                return evFishByID;
            }
        }
        public bool fishByHP
        {
            get
            {
                return evFishByHP;
            }
        }
        public bool dropItems
        {
            get
            {
                return evDropItems;
            }
        }
        public bool dropMobs
        {
            get
            {
                return evDropMobs;
            }
        }
        public int fishByHPMin
        {
            get
            {
                return evFishByHPMin;
            }
        }
        public int fishByHPMax
        {
            get
            {
                return evFishByHPMax;
            }
        }
        public int noCatchTimeout
        {
            get
            {
                return evNoCatchTimeout;
            }
        }
        public bool releaseFixed
        {
            get
            {
                return evReleaseFixed;
            }
        }
        public double releaseTime
        {
            get
            {
                return evReleaseTime;
            }
        }
        public bool releaseRandom
        {
            get
            {
                return evReleaseRandom;
            }
        }
        public double releaseTimeRandomMin
        {
            get
            {
                return evReleaseTimeRandomMin;
            }
        }
        public double releaseTimeRandomMax
        {
            get
            {
                return evReleaseTimeRandomMax;
            }
        }
        public bool timedStart
        {
            get
            {
                return evTimedStart;
            }
        }
        public DateTime startTime
        {
            get
            {
                return evStartTime;
            }
        }
        public bool timedEnd
        {
            get
            {
                return evTimedEnd;
            }
        }
        public DateTime endTime
        {
            get
            {
                return evEndTime;
            }
        }
        public bool moveInv
        {
            get
            {
                return evMoveInv;
            }
        }
    }
}
