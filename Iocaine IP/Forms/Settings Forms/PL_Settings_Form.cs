using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Logging;

namespace Iocaine2.Settings
{
    public partial class PL_Settings_Form : Form
    {
        #region Enums
        public enum REST_GRID_COL
        {
            PRI = 0,
            IN_FIGHT = 1,
            STAND_LOW = 2,
            STAND_HIGH = 3,
            MP_LOW = 4,
            MP_HIGH = 5
        }
        public enum REST_STYLE
        {
            AGGRESSIVE = 0,
            NORMAL = 1,
            LIGHT = 2,
            CUSTOM = 3
        }
        #endregion
        #region Members
        private int restStyle = 1;
        private bool doingInit = true;
        private ArrayList localRestingSettingsPerTypeList = new ArrayList();
        private ArrayList restSettingsTranslationList = new ArrayList();
        #endregion Members
        #region Inits
        public PL_Settings_Form(int iStartX, int iStartY)
        {
            InitializeComponent();
            this.SetDesktopLocation(iStartX - (this.Size.Width / 2), iStartY - (this.Size.Height / 2));
            initForm();
        }
        private void initForm()
        {
            doingInit = true;
            //copy over the rest settings array/list
            restStyle = Statics.Settings.PowerLevel.RestingStyle;
            localRestingSettingsPerTypeList.Clear();
            for(int ii=0; ii<(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.ROW_COUNT; ii++)
            {
                localRestingSettingsPerTypeList.Add(Statics.Settings.PowerLevel.RestingValuePerTypeList[ii]);
            }
            //init first settings tab
            HealMpPercLowTextBox.Text = Statics.Settings.PowerLevel.HealMpPercLow.ToString();
            HealMpPercHighTextBox.Text = Statics.Settings.PowerLevel.HealMpPercHigh.ToString();
            UpperMpMarginTextBox.Text = Statics.Settings.PowerLevel.UpperMpMargin.ToString();
            NeverRestCheckBox.Checked = Statics.Settings.PowerLevel.NeverRest;
            RestInFightCheckBox.Checked = Statics.Settings.PowerLevel.RestInFight;
            RestInFightAlwaysCheckBox.Checked = Statics.Settings.PowerLevel.RestInFightAlways;
            RestInFightMpUpperCheckBox.Checked = Statics.Settings.PowerLevel.RestInFightLessUpper;
            RestInFightMpLowerCheckBox.Checked = Statics.Settings.PowerLevel.RestInFightLessLower;
            switch (Statics.Settings.PowerLevel.RestingStyle)
            {
                case (int)REST_STYLE.AGGRESSIVE:
                    RestAggressiveRadioButton.Checked = true;
                    RestNormalRadioButton.Checked = false;
                    RestLightRadioButton.Checked = false;
                    RestCustomRadioButton.Checked = false;
                    break;
                case (int)REST_STYLE.NORMAL:
                    RestAggressiveRadioButton.Checked = false;
                    RestNormalRadioButton.Checked = true;
                    RestLightRadioButton.Checked = false;
                    RestCustomRadioButton.Checked = false;
                    break;
                case (int)REST_STYLE.LIGHT:
                    RestAggressiveRadioButton.Checked = false;
                    RestNormalRadioButton.Checked = false;
                    RestLightRadioButton.Checked = true;
                    RestCustomRadioButton.Checked = false;
                    break;
                case (int)REST_STYLE.CUSTOM:
                    RestAggressiveRadioButton.Checked = false;
                    RestNormalRadioButton.Checked = false;
                    RestLightRadioButton.Checked = false;
                    RestCustomRadioButton.Checked = true;
                    break;
            }
            FocusCharIndexUpDown.Value = Statics.Settings.PowerLevel.FocusCharacterRow;
            FocusCharIndexUpDown.Minimum = 1;
            FocusCharIndexUpDown.Maximum = Statics.Settings.PowerLevel.CharacterGridRowCount - 1;
            MaxCastDistanceUpDown.Value = (decimal)Statics.Settings.PowerLevel.MaxCastDistance;
            WakeCureCommandTextBox.Text = Statics.Settings.PowerLevel.WakeCure;
            ElasticFollowCheckBox.Checked = Statics.Settings.PowerLevel.UseElasticFollowing;
            ElasticFollowTextBox.Text = (Statics.Settings.PowerLevel.FollowDistanceUpper - Statics.Settings.PowerLevel.FollowDistance).ToString();
            CastTimeUpDown.Value = Statics.Settings.PowerLevel.CastTimeModifier;
            CastTimeCuresUpDown.Value = Statics.Settings.PowerLevel.CastTimeModifierCures;
            CastMarginUpDown.Value = Statics.Settings.PowerLevel.CastTimeMargin;
            JaProcTimeUpDown.Value = Statics.Settings.PowerLevel.JaProcTime;
            HealthCheckPollPeriodUpDown.Value = Statics.Settings.PowerLevel.HealthUpdateFrequency;
            CastCheckPollPeriodUpDown.Value = Statics.Settings.PowerLevel.DequeuePollFrequency;
            ActivityUpdateUpDown.Value = Statics.Settings.PowerLevel.PlCharActivePollFrequency;
            
            //init up down Q numbers
            FirstCureQUpDown.Minimum = 1;
            FirstCureQUpDown.Maximum = Statics.Settings.PowerLevel.NbOfQueues;
            FirstCureQUpDown.Value = Statics.Settings.PowerLevel.FirstCureQ;
            FirstCureQUpDown.ReadOnly = true;
            SecondCureQUpDown.Minimum = 1;
            SecondCureQUpDown.Maximum = Statics.Settings.PowerLevel.NbOfQueues;
            SecondCureQUpDown.Value = Statics.Settings.PowerLevel.SecondCureQ;
            SecondCureQUpDown.ReadOnly = true;
            ThirdCureQUpDown.Minimum = 1;
            ThirdCureQUpDown.Maximum = Statics.Settings.PowerLevel.NbOfQueues;
            ThirdCureQUpDown.Value = Statics.Settings.PowerLevel.ThirdCureQ;
            ThirdCureQUpDown.ReadOnly = true;
            WakeCureQUpDown.Minimum = 1;
            WakeCureQUpDown.Maximum = Statics.Settings.PowerLevel.NbOfQueues;
            WakeCureQUpDown.Value = Statics.Settings.PowerLevel.WakeCureQ;
            WakeCureQUpDown.ReadOnly = false;
            PartyBuffsQUpDown.Minimum = 1;
            PartyBuffsQUpDown.Maximum = Statics.Settings.PowerLevel.NbOfQueues;
            PartyBuffsQUpDown.Value = Statics.Settings.PowerLevel.BuffQ;
            PartyBuffsQUpDown.ReadOnly = false;
            SelfBuffsQUpDown.Minimum = 1;
            SelfBuffsQUpDown.Maximum = Statics.Settings.PowerLevel.NbOfQueues;
            SelfBuffsQUpDown.Value = Statics.Settings.PowerLevel.SelfBuffQ;
            SelfBuffsQUpDown.ReadOnly = false;
            DebuffsQUpDown.Minimum = 1;
            DebuffsQUpDown.Maximum = Statics.Settings.PowerLevel.NbOfQueues;
            DebuffsQUpDown.Value = Statics.Settings.PowerLevel.DebuffQ;
            DebuffsQUpDown.ReadOnly = false;
            //init rest settings grid box
            loadRestingGrid();
            doingInit = false;
            //init Chat settings
            chatMpReportCheckBox.Checked = Statics.Settings.PowerLevel.ChatMpReport;
            chatMpReportCmdTextBox.Text = Statics.Settings.PowerLevel.ChatMpReportCmd;
            chatMpReportTimerTextBox.Text = (Statics.Settings.PowerLevel.ChatMpReportTimer / 1000).ToString();
            chatHpReportCheckBox.Checked = Statics.Settings.PowerLevel.ChatHpReport;
            chatHpReportCmdTextBox.Text = Statics.Settings.PowerLevel.ChatHpReportCmd;
            chatHpReportHPPercTextBox.Text = Statics.Settings.PowerLevel.ChatHpReportLevel.ToString();
            chatCastTimeCheckBox.Checked = Statics.Settings.PowerLevel.ChatCastTime;
            chatCastTimeCmdTextBox.Text = Statics.Settings.PowerLevel.ChatCastTimeCmd;
            chatCastTimeTimeTextBox.Text = (Statics.Settings.PowerLevel.ChatCastTimeTime / 1000).ToString();
            chatLostCheckBox.Checked = Statics.Settings.PowerLevel.ChatLost;
            chatLostCmdTextBox.Text = Statics.Settings.PowerLevel.ChatLostCmd;
            //init Command settings
            command1CheckBox.Checked = Statics.Settings.PowerLevel.Command1Enable;
            command1CmdTextBox.Text = Statics.Settings.PowerLevel.Command1Cmd;
            command1TimerTextBox.Text = (Statics.Settings.PowerLevel.Command1Timer / 1000).ToString();
            command1BeforeRestingCheckBox.Checked = Statics.Settings.PowerLevel.Command1BeforeResting;
            command2CheckBox.Checked = Statics.Settings.PowerLevel.Command2Enable;
            command2CmdTextBox.Text = Statics.Settings.PowerLevel.Command2Cmd;
            command2TimerTextBox.Text = (Statics.Settings.PowerLevel.Command2Timer / 1000).ToString();
            command2BeforeRestingCheckBox.Checked = Statics.Settings.PowerLevel.Command2BeforeResting;
            command3CheckBox.Checked = Statics.Settings.PowerLevel.Command3Enable;
            command3CmdTextBox.Text = Statics.Settings.PowerLevel.Command3Cmd;
            command3TimerTextBox.Text = (Statics.Settings.PowerLevel.Command3Timer / 1000).ToString();
            command3BeforeRestingCheckBox.Checked = Statics.Settings.PowerLevel.Command3BeforeResting;
            command4CheckBox.Checked = Statics.Settings.PowerLevel.Command4Enable;
            command4CmdTextBox.Text = Statics.Settings.PowerLevel.Command4Cmd;
            command4TimerTextBox.Text = (Statics.Settings.PowerLevel.Command4Timer / 1000).ToString();
            command4BeforeRestingCheckBox.Checked = Statics.Settings.PowerLevel.Command4BeforeResting;
            //init Convert settings
            convertEnableCheckBox.Checked = Statics.Settings.PowerLevel.ConvertEnable;
            convertEnInFightCheckBox.Checked = Statics.Settings.PowerLevel.ConvertEnInFight;
            convertMpThresholdTextBox.Text = Statics.Settings.PowerLevel.ConvertMpThreshold.ToString();
            convertCureRestFullFirstCheckBox.Checked = Statics.Settings.PowerLevel.ConvertCureRestFullFirst;
            convertCureRestFullMarginTextBox.Text = Statics.Settings.PowerLevel.ConvertCureRestFullMargin.ToString();
        }
        #endregion
        #region Utility Functions
        private void loadRestingGrid()
        {
            RestingGrid.Rows.Clear();
            generateRestingQArray();

            LoggingFunctions.Debug("PLSett::loadRestingGrid: ***Dumping contents of the Translation List BEFORE setting restingGrid values***", LoggingFunctions.DBG_SCOPE.SETTINGS);
            for (int kk = 0; kk < Statics.Settings.PowerLevel.NbOfQueues; kk++)
            {
                bool[] tempArray = (bool[])restSettingsTranslationList[kk];
                LoggingFunctions.Debug("PLSett::loadRestingGrid: Row: " + kk + "  " + tempArray[0] + ", " + tempArray[1] + ", " + tempArray[2] +
                    ", " + tempArray[3] + ", " + tempArray[4] + ".", LoggingFunctions.DBG_SCOPE.SETTINGS);
            }
            for (int ii = 0; ii < Statics.Settings.PowerLevel.NbOfQueues; ii++)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                DataGridViewCell priorityCell = new DataGridViewTextBoxCell();
                DataGridViewCell useInFightCell = new DataGridViewCheckBoxCell();
                DataGridViewCell stopRestMpLessLowCell = new DataGridViewCheckBoxCell();
                DataGridViewCell stopRestMpLessHighCell = new DataGridViewCheckBoxCell();
                DataGridViewCell mpLessLowCell = new DataGridViewCheckBoxCell();
                DataGridViewCell mpLessHighCell = new DataGridViewCheckBoxCell();
                bool[] settingsArray = (bool[])restSettingsTranslationList[ii];
                priorityCell.Value = ii + 1;
                useInFightCell.Value = settingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT];
                stopRestMpLessLowCell.Value = settingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW];
                stopRestMpLessHighCell.Value = settingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH];
                mpLessLowCell.Value = settingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW];
                mpLessHighCell.Value = settingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH];
                newRow.Cells.Add(priorityCell);
                newRow.Cells.Add(useInFightCell);
                newRow.Cells.Add(stopRestMpLessLowCell);
                newRow.Cells.Add(stopRestMpLessHighCell);
                newRow.Cells.Add(mpLessLowCell);
                newRow.Cells.Add(mpLessHighCell);
                RestingGrid.Rows.Add(newRow);
                LoggingFunctions.Debug("PLSett::loadRestingGrid: Row " + ii + " values are: " + useInFightCell.Value + ", " + stopRestMpLessLowCell.Value + ", " +
                    stopRestMpLessHighCell.Value + ", " + mpLessLowCell.Value + ", " + mpLessHighCell.Value + ".", LoggingFunctions.DBG_SCOPE.SETTINGS);
            }
        }
        private void generateRestingQArray()
        {
            restSettingsTranslationList.Clear();
            for (int ii = 0; ii < Statics.Settings.PowerLevel.NbOfQueues; ii++)
            {
                LoggingFunctions.Debug("PLSett::generateRestingQArray: Initializing row " + ii + " of the restSettingsArray.", LoggingFunctions.DBG_SCOPE.SETTINGS);
                bool[] restSettingsArray = new bool[(int)Iocaine_2_Form.PL_REST_ARR_INX.LENGTH];
                if (ii == ThirdCureQUpDown.Value - 1)
                {
                    restSettingsArray = (bool[])localRestingSettingsPerTypeList[(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.THIRD_CURE];
                }
                else if (ii == SecondCureQUpDown.Value - 1)
                {
                    restSettingsArray = (bool[])localRestingSettingsPerTypeList[(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.SECOND_CURE];
                }
                else if (ii == FirstCureQUpDown.Value - 1)
                {
                    restSettingsArray = (bool[])localRestingSettingsPerTypeList[(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.FIRST_CURE];
                }
                else if (ii == PartyBuffsQUpDown.Value - 1)
                {
                    restSettingsArray = (bool[])localRestingSettingsPerTypeList[(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.PARTY_BUFFS];
                }
                else if (ii == SelfBuffsQUpDown.Value - 1)
                {
                    restSettingsArray = (bool[])localRestingSettingsPerTypeList[(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.SELF_BUFFS];
                }
                else if (ii == DebuffsQUpDown.Value - 1)
                {
                    restSettingsArray = (bool[])localRestingSettingsPerTypeList[(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.DEBUFFS];
                }
                else if (ii == WakeCureQUpDown.Value - 1)
                {
                    restSettingsArray = (bool[])localRestingSettingsPerTypeList[(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.WAKE_CURE];
                }
                else
                {
                    restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                    restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                    restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                    restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                    restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                }
                restSettingsTranslationList.Add(restSettingsArray);
            }
        }
        private void updateLocalRestSettingsArray()
        {
            for (int ii = 0; ii < Statics.Settings.PowerLevel.NbOfQueues; ii++)
            {
                DataGridViewRow localRow = RestingGrid.Rows[ii];
                bool[] localArray = (bool[])restSettingsTranslationList[ii];
                localArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = (bool)localRow.Cells[(int)REST_GRID_COL.IN_FIGHT].Value;
                localArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = (bool)localRow.Cells[(int)REST_GRID_COL.STAND_LOW].Value;
                localArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = (bool)localRow.Cells[(int)REST_GRID_COL.STAND_HIGH].Value;
                localArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = (bool)localRow.Cells[(int)REST_GRID_COL.MP_LOW].Value;
                localArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = (bool)localRow.Cells[(int)REST_GRID_COL.MP_HIGH].Value;

                //Now we translate this row to a matching row in the Type Array
                if (ThirdCureQUpDown.Value-1 == ii)
                {
                    localRestingSettingsPerTypeList[(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.THIRD_CURE] = localArray;
                }
                else if (SecondCureQUpDown.Value - 1 == ii)
                {
                    localRestingSettingsPerTypeList[(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.SECOND_CURE] = localArray;
                }
                else if (FirstCureQUpDown.Value - 1 == ii)
                {
                    localRestingSettingsPerTypeList[(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.FIRST_CURE] = localArray;
                }
                else if (PartyBuffsQUpDown.Value - 1 == ii)
                {
                    localRestingSettingsPerTypeList[(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.PARTY_BUFFS] = localArray;
                }
                else if (SelfBuffsQUpDown.Value - 1 == ii)
                {
                    localRestingSettingsPerTypeList[(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.SELF_BUFFS] = localArray;
                }
                else if (WakeCureQUpDown.Value - 1 == ii)
                {
                    localRestingSettingsPerTypeList[(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.WAKE_CURE] = localArray;
                }
                else if (DebuffsQUpDown.Value - 1 == ii)
                {
                    localRestingSettingsPerTypeList[(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.DEBUFFS] = localArray;
                }
            }
            LoggingFunctions.Debug("PLSett::updateLocalRestSettingsArray: Dumping contents of the Translation List AFTER setting restingGrid values.", LoggingFunctions.DBG_SCOPE.SETTINGS);
            for (int kk = 0; kk < Statics.Settings.PowerLevel.NbOfQueues; kk++)
            {
                bool[] tempArray = (bool[])restSettingsTranslationList[kk];
                LoggingFunctions.Debug("PLSett::updateLocalRestSettingsArray: Row: " + kk + "  " + tempArray[0] + ", " + tempArray[1] + ", " + tempArray[2] +
                    ", " + tempArray[3] + ", " + tempArray[4] + ".", LoggingFunctions.DBG_SCOPE.SETTINGS);
            }
        }
        #endregion
        #region Event Handlers
        private void OK_Button_Click(object sender, EventArgs e)
        {
            Apply_Button_Click(sender, e);
            this.Dispose();
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void Apply_Button_Click(object sender, EventArgs e)
        {
            Statics.Settings.PowerLevel.RestingValuePerTypeList.Clear();
            for(int ii=0; ii<(int)Iocaine_2_Form.PL_REST_PER_TYPE_ROW.ROW_COUNT; ii++)
            {
                Statics.Settings.PowerLevel.RestingValuePerTypeList.Add((bool[])localRestingSettingsPerTypeList[ii]);
            }
            Statics.Settings.PowerLevel.RestingValueList.Clear();
            for (int kk = 0; kk < Statics.Settings.PowerLevel.NbOfQueues; kk++)
            {
                Statics.Settings.PowerLevel.RestingValueList.Add((bool[])restSettingsTranslationList[kk]);
            }
            Statics.Settings.PowerLevel.WakeCureQ = (int)WakeCureQUpDown.Value;
            UserSettings.SetValue(UserSettings.BOT.PL, "WakeCureQ", Statics.Settings.PowerLevel.WakeCureQ.ToString());
            Statics.Settings.PowerLevel.BuffQ = (int)PartyBuffsQUpDown.Value;
            UserSettings.SetValue(UserSettings.BOT.PL, "BuffQ", Statics.Settings.PowerLevel.BuffQ.ToString());
            Statics.Settings.PowerLevel.SelfBuffQ = (int)SelfBuffsQUpDown.Value;
            UserSettings.SetValue(UserSettings.BOT.PL, "SelfBuffQ", Statics.Settings.PowerLevel.SelfBuffQ.ToString());
            Statics.Settings.PowerLevel.DebuffQ = (int)DebuffsQUpDown.Value;
            UserSettings.SetValue(UserSettings.BOT.PL, "DebuffQ", Statics.Settings.PowerLevel.DebuffQ.ToString());
            try
            {
                Statics.Settings.PowerLevel.HealMpPercLow = System.Convert.ToUInt32(HealMpPercLowTextBox.Text);
                UserSettings.SetValue(UserSettings.BOT.PL, "HealMpPercLow", Statics.Settings.PowerLevel.HealMpPercLow.ToString());
            }
            catch { }
            try
            {
                Statics.Settings.PowerLevel.HealMpPercHigh = System.Convert.ToUInt32(HealMpPercHighTextBox.Text);
                UserSettings.SetValue(UserSettings.BOT.PL, "HealMpPercHigh", Statics.Settings.PowerLevel.HealMpPercHigh.ToString());
            }
            catch { }
            try
            {
                Statics.Settings.PowerLevel.UpperMpMargin = System.Convert.ToUInt32(UpperMpMarginTextBox.Text);
                UserSettings.SetValue(UserSettings.BOT.PL, "UpperMpMargin", Statics.Settings.PowerLevel.UpperMpMargin.ToString());
            }
            catch { }
            Statics.Settings.PowerLevel.NeverRest = NeverRestCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "NeverRest", Statics.Settings.PowerLevel.NeverRest.ToString());
            Statics.Settings.PowerLevel.RestInFight = RestInFightCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "RestInFight", Statics.Settings.PowerLevel.RestInFight.ToString());
            Statics.Settings.PowerLevel.RestInFightAlways = RestInFightAlwaysCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "RestInFightAlways", Statics.Settings.PowerLevel.RestInFightAlways.ToString());
            Statics.Settings.PowerLevel.RestInFightLessUpper = RestInFightMpUpperCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "RestInFightLessUpper", Statics.Settings.PowerLevel.RestInFightLessUpper.ToString());
            Statics.Settings.PowerLevel.RestInFightLessLower = RestInFightMpLowerCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "RestInFightLessLower", Statics.Settings.PowerLevel.RestInFightLessLower.ToString());
            Statics.Settings.PowerLevel.RestingStyle = restStyle;
            UserSettings.SetValue(UserSettings.BOT.PL, "RestingStyle", Statics.Settings.PowerLevel.RestingStyle.ToString());
            Statics.Settings.PowerLevel.FocusCharacterRow = (int)FocusCharIndexUpDown.Value;
            //UserSettings.SetValue(UserSettings.BOT.PL, "FocusCharacterRow", Iocaine_2_Form.focusCharacterRow.ToString());
            Statics.Settings.PowerLevel.MaxCastDistance = (Single)MaxCastDistanceUpDown.Value;
            UserSettings.SetValue(UserSettings.BOT.PL, "MaxCastDistance", Statics.Settings.PowerLevel.MaxCastDistance.ToString());
            Statics.Settings.PowerLevel.WakeCure = WakeCureCommandTextBox.Text;
            UserSettings.SetValue(UserSettings.BOT.PL, "WakeCure", Statics.Settings.PowerLevel.WakeCure);
            Statics.Settings.PowerLevel.UseElasticFollowing = ElasticFollowCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "UseElasticFollowing", Statics.Settings.PowerLevel.UseElasticFollowing.ToString());
            try
            {
                Statics.Settings.PowerLevel.ElasticDistance = System.Convert.ToInt32(ElasticFollowTextBox.Text);
                Statics.Settings.PowerLevel.FollowDistanceUpper = Statics.Settings.PowerLevel.FollowDistance
                            + (ElasticFollowCheckBox.Checked ? Statics.Settings.PowerLevel.ElasticDistance : 0);
                UserSettings.SetValue(UserSettings.BOT.PL, "ElasticDistance", Statics.Settings.PowerLevel.ElasticDistance.ToString());
            }
            catch { }
            Statics.Settings.PowerLevel.CastTimeModifier = (uint)CastTimeUpDown.Value;
            UserSettings.SetValue(UserSettings.BOT.PL, "CastTimeModifier", Statics.Settings.PowerLevel.CastTimeModifier.ToString());
            Statics.Settings.PowerLevel.CastTimeModifierCures = (uint)CastTimeCuresUpDown.Value;
            UserSettings.SetValue(UserSettings.BOT.PL, "CastTimeModifierCures", Statics.Settings.PowerLevel.CastTimeModifierCures.ToString());
            Statics.Settings.PowerLevel.CastTimeMargin = (uint)CastMarginUpDown.Value;
            UserSettings.SetValue(UserSettings.BOT.PL, "CastTimeMargin", Statics.Settings.PowerLevel.CastTimeMargin.ToString());
            Statics.Settings.PowerLevel.JaProcTime = (uint)JaProcTimeUpDown.Value;
            UserSettings.SetValue(UserSettings.BOT.PL, "JaProcTime", Statics.Settings.PowerLevel.JaProcTime.ToString());
            Statics.Settings.PowerLevel.HealthUpdateFrequency = (uint)HealthCheckPollPeriodUpDown.Value;
            UserSettings.SetValue(UserSettings.BOT.PL, "HealthUpdateFrequency", Statics.Settings.PowerLevel.HealthUpdateFrequency.ToString());
            Statics.Settings.PowerLevel.DequeuePollFrequency = (uint)CastCheckPollPeriodUpDown.Value;
            UserSettings.SetValue(UserSettings.BOT.PL, "DequeuePollFrequency", Statics.Settings.PowerLevel.DequeuePollFrequency.ToString());
            Statics.Settings.PowerLevel.PlCharActivePollFrequency = (uint)ActivityUpdateUpDown.Value;
            UserSettings.SetValue(UserSettings.BOT.PL, "PlCharActivePollFrequency", Statics.Settings.PowerLevel.PlCharActivePollFrequency.ToString());
            //Set Chat settings
            Statics.Settings.PowerLevel.ChatMpReport = chatMpReportCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "ChatMpReport", Statics.Settings.PowerLevel.ChatMpReport.ToString());
            Statics.Settings.PowerLevel.ChatMpReportCmd = chatMpReportCmdTextBox.Text;
            UserSettings.SetValue(UserSettings.BOT.PL, "ChatMpReportCmd", Statics.Settings.PowerLevel.ChatMpReportCmd);
            Statics.Settings.PowerLevel.ChatMpReportTimer = 1000 * Convert.ToUInt32(chatMpReportTimerTextBox.Text);
            UserSettings.SetValue(UserSettings.BOT.PL, "ChatMpReportTimer", Statics.Settings.PowerLevel.ChatMpReportTimer.ToString());
            Statics.Settings.PowerLevel.ChatHpReport = chatHpReportCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "ChatHpReport", Statics.Settings.PowerLevel.ChatHpReport.ToString());
            Statics.Settings.PowerLevel.ChatHpReportCmd = chatHpReportCmdTextBox.Text;
            UserSettings.SetValue(UserSettings.BOT.PL, "ChatHpReportCmd", Statics.Settings.PowerLevel.ChatHpReportCmd);
            Statics.Settings.PowerLevel.ChatHpReportLevel = Convert.ToUInt32(chatHpReportHPPercTextBox.Text);
            UserSettings.SetValue(UserSettings.BOT.PL, "ChatHpReportLevel", Statics.Settings.PowerLevel.ChatHpReportLevel.ToString());
            Statics.Settings.PowerLevel.ChatCastTime = chatCastTimeCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "ChatCastTime", Statics.Settings.PowerLevel.ChatCastTime.ToString());
            Statics.Settings.PowerLevel.ChatCastTimeCmd = chatCastTimeCmdTextBox.Text;
            UserSettings.SetValue(UserSettings.BOT.PL, "ChatCastTimeCmd", Statics.Settings.PowerLevel.ChatCastTimeCmd);
            Statics.Settings.PowerLevel.ChatCastTimeTime = 1000 * Convert.ToDouble(chatCastTimeTimeTextBox.Text);
            UserSettings.SetValue(UserSettings.BOT.PL, "ChatCastTimeTime", Statics.Settings.PowerLevel.ChatCastTimeTime.ToString());
            Statics.Settings.PowerLevel.ChatLost = chatLostCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "ChatLost", Statics.Settings.PowerLevel.ChatLost.ToString());
            Statics.Settings.PowerLevel.ChatLostCmd = chatLostCmdTextBox.Text;
            UserSettings.SetValue(UserSettings.BOT.PL, "ChatLostCmd", Statics.Settings.PowerLevel.ChatLostCmd);
            //Set Command settings
            Statics.Settings.PowerLevel.Command1Enable = command1CheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "Command1Enable", Statics.Settings.PowerLevel.Command1Enable.ToString());
            Statics.Settings.PowerLevel.Command1Cmd = command1CmdTextBox.Text;
            UserSettings.SetValue(UserSettings.BOT.PL, "Command1Cmd", Statics.Settings.PowerLevel.Command1Cmd);
            Statics.Settings.PowerLevel.Command1Timer = 1000 * Convert.ToUInt32(command1TimerTextBox.Text);
            UserSettings.SetValue(UserSettings.BOT.PL, "Command1Timer", Statics.Settings.PowerLevel.Command1Timer.ToString());
            Statics.Settings.PowerLevel.Command1BeforeResting = command1BeforeRestingCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "Command1BeforeResting", Statics.Settings.PowerLevel.Command1BeforeResting.ToString());
            Statics.Settings.PowerLevel.Command2Enable = command2CheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "Command2Enable", Statics.Settings.PowerLevel.Command2Enable.ToString());
            Statics.Settings.PowerLevel.Command2Cmd = command2CmdTextBox.Text;
            UserSettings.SetValue(UserSettings.BOT.PL, "Command2Cmd", Statics.Settings.PowerLevel.Command2Cmd);
            Statics.Settings.PowerLevel.Command2Timer = 1000 * Convert.ToUInt32(command2TimerTextBox.Text);
            UserSettings.SetValue(UserSettings.BOT.PL, "Command2Timer", Statics.Settings.PowerLevel.Command2Timer.ToString());
            Statics.Settings.PowerLevel.Command2BeforeResting = command2BeforeRestingCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "Command2BeforeResting", Statics.Settings.PowerLevel.Command2BeforeResting.ToString());
            Statics.Settings.PowerLevel.Command3Enable = command3CheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "Command3Enable", Statics.Settings.PowerLevel.Command3Enable.ToString());
            Statics.Settings.PowerLevel.Command3Cmd = command3CmdTextBox.Text;
            UserSettings.SetValue(UserSettings.BOT.PL, "Command3Cmd", Statics.Settings.PowerLevel.Command3Cmd);
            Statics.Settings.PowerLevel.Command3Timer = 1000 * Convert.ToUInt32(command3TimerTextBox.Text);
            UserSettings.SetValue(UserSettings.BOT.PL, "Command3Timer", Statics.Settings.PowerLevel.Command3Timer.ToString());
            Statics.Settings.PowerLevel.Command3BeforeResting = command3BeforeRestingCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "Command3BeforeResting", Statics.Settings.PowerLevel.Command3BeforeResting.ToString());
            Statics.Settings.PowerLevel.Command4Enable = command4CheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "Command4Enable", Statics.Settings.PowerLevel.Command4Enable.ToString());
            Statics.Settings.PowerLevel.Command4Cmd = command4CmdTextBox.Text;
            UserSettings.SetValue(UserSettings.BOT.PL, "Command4Cmd", Statics.Settings.PowerLevel.Command4Cmd);
            Statics.Settings.PowerLevel.Command4Timer = 1000 * Convert.ToUInt32(command4TimerTextBox.Text);
            UserSettings.SetValue(UserSettings.BOT.PL, "Command4Timer", Statics.Settings.PowerLevel.Command4Timer.ToString());
            Statics.Settings.PowerLevel.Command4BeforeResting = command4BeforeRestingCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "Command4BeforeResting", Statics.Settings.PowerLevel.Command4BeforeResting.ToString());
            //Set Convert settings
            Statics.Settings.PowerLevel.ConvertEnable = convertEnableCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "ConvertEnable", Statics.Settings.PowerLevel.ConvertEnable.ToString());
            Statics.Settings.PowerLevel.ConvertEnInFight = convertEnInFightCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "ConvertEnInFight", Statics.Settings.PowerLevel.ConvertEnInFight.ToString());
            Statics.Settings.PowerLevel.ConvertMpThreshold = Convert.ToUInt32(convertMpThresholdTextBox.Text);
            UserSettings.SetValue(UserSettings.BOT.PL, "ConvertMpThreshold", Statics.Settings.PowerLevel.ConvertMpThreshold.ToString());
            Statics.Settings.PowerLevel.ConvertCureRestFullFirst = convertCureRestFullFirstCheckBox.Checked;
            UserSettings.SetValue(UserSettings.BOT.PL, "ConvertCureRestFullFirst", Statics.Settings.PowerLevel.ConvertCureRestFullFirst.ToString());
            Statics.Settings.PowerLevel.ConvertCureRestFullMargin = Convert.ToUInt32(convertCureRestFullMarginTextBox.Text);
            UserSettings.SetValue(UserSettings.BOT.PL, "ConvertCureRestFullMargin", Statics.Settings.PowerLevel.ConvertCureRestFullMargin.ToString());
        }
        private void ThirdCureQUpDown_ValueChanged(object sender, EventArgs e)
        {
            ThirdCureQUpDown.Value = Statics.Settings.PowerLevel.ThirdCureQ;
        }
        private void SecondCureQUpDown_ValueChanged(object sender, EventArgs e)
        {
            SecondCureQUpDown.Value = Statics.Settings.PowerLevel.SecondCureQ;
        }
        private void FirstCureQUpDown_ValueChanged(object sender, EventArgs e)
        {
            FirstCureQUpDown.Value = Statics.Settings.PowerLevel.FirstCureQ;
        }
        private void WakeCureQUpDown_ValueChanged(object sender, EventArgs e)
        {
            if(!doingInit)
            {
                LoggingFunctions.Debug("PLSett::WakeCureQUpDown_ValueChanged: Reloading grid due to wake cure Q change.", LoggingFunctions.DBG_SCOPE.SETTINGS);
                generateRestingQArray();
                loadRestingGrid();
            }
        }
        private void PartyBuffsQUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!doingInit)
            {
                LoggingFunctions.Debug("PLSett::PartyBuffsQUpDown_ValueChanged: Reloading grid due to party buffs Q change.", LoggingFunctions.DBG_SCOPE.SETTINGS);
                generateRestingQArray();
                loadRestingGrid();
            }
        }
        private void SelfBuffsQUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!doingInit)
            {
                LoggingFunctions.Debug("PLSett::SelfBuffsQUpDown_ValueChanged: Reloading grid due to self buffs Q change.", LoggingFunctions.DBG_SCOPE.SETTINGS);
                generateRestingQArray();
                loadRestingGrid();
            }
        }
        private void DebuffsQUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!doingInit)
            {
                LoggingFunctions.Debug("PLSett::DebuffsQUpDown_ValueChanged Reloading grid due to debuffs Q change.", LoggingFunctions.DBG_SCOPE.SETTINGS);
                generateRestingQArray();
                loadRestingGrid();
            }
        }
        private void RestingGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!doingInit)
            {
                RestAggressiveRadioButton.Checked = false;
                RestNormalRadioButton.Checked = false;
                RestLightRadioButton.Checked = false;
                RestCustomRadioButton.Checked = true;
                restStyle = (int)REST_STYLE.CUSTOM;
                updateLocalRestSettingsArray();
            }
        }
        private void RestAggressiveRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!doingInit)
            {
                if (RestAggressiveRadioButton.Checked)
                {
                    for (int ii = 0; ii < Statics.Settings.PowerLevel.NbOfQueues; ii++)
                    {
                        bool[] restSettingsArray = (bool[])restSettingsTranslationList[ii];
                        if (ii == ThirdCureQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == SecondCureQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == FirstCureQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = false;
                        }
                        else if (ii == PartyBuffsQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == SelfBuffsQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == DebuffsQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == WakeCureQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                    }
                    loadRestingGrid();
                    RestInFightCheckBox.Checked = true;
                    RestInFightAlwaysCheckBox.Checked = true;
                    RestInFightMpUpperCheckBox.Checked = true;
                    RestInFightMpLowerCheckBox.Checked = true;
                    restStyle = (int)REST_STYLE.AGGRESSIVE;
                }
            }
        }
        private void RestNormalRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!doingInit)
            {
                if (RestNormalRadioButton.Checked)
                {
                    for (int ii = 0; ii < Statics.Settings.PowerLevel.NbOfQueues; ii++)
                    {
                        bool[] restSettingsArray = (bool[])restSettingsTranslationList[ii];
                        if (ii == ThirdCureQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == SecondCureQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == FirstCureQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == PartyBuffsQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == SelfBuffsQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == DebuffsQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == WakeCureQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                    }
                    if (!doingInit)
                    {
                        loadRestingGrid();
                    }
                    RestInFightCheckBox.Checked = true;
                    RestInFightAlwaysCheckBox.Checked = false;
                    RestInFightMpUpperCheckBox.Checked = true;
                    RestInFightMpLowerCheckBox.Checked = true;
                    restStyle = (int)REST_STYLE.NORMAL;
                }
            }
        }
        private void RestLightRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!doingInit)
            {
                if (RestLightRadioButton.Checked)
                {
                    for (int ii = 0; ii < Statics.Settings.PowerLevel.NbOfQueues; ii++)
                    {
                        bool[] restSettingsArray = (bool[])restSettingsTranslationList[ii];
                        if (ii == ThirdCureQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == SecondCureQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == FirstCureQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == PartyBuffsQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = false;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == SelfBuffsQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == DebuffsQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else if (ii == WakeCureQUpDown.Value - 1)
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                        else
                        {
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] = true;
                            restSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH] = true;
                        }
                    }
                    if (!doingInit)
                    {
                        loadRestingGrid();
                    }
                    RestInFightCheckBox.Checked = true;
                    RestInFightAlwaysCheckBox.Checked = false;
                    RestInFightMpUpperCheckBox.Checked = false;
                    RestInFightMpLowerCheckBox.Checked = true;
                    restStyle = (int)REST_STYLE.LIGHT;
                }
            }
        }
        private void NeverRestCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (NeverRestCheckBox.Checked)
            {
                RestInFightCheckBox.Checked = false;
                RestInFightCheckBox.Enabled = false;
                RestInFightAlwaysCheckBox.Checked = false;
                RestInFightAlwaysCheckBox.Enabled = false;
                RestInFightMpUpperCheckBox.Checked = false;
                RestInFightMpUpperCheckBox.Enabled = false;
                RestInFightMpLowerCheckBox.Checked = false;
                RestInFightMpLowerCheckBox.Enabled = false;
            }
            else
            {
                RestInFightCheckBox.Enabled = true;
                RestInFightAlwaysCheckBox.Enabled = true;
                RestInFightMpUpperCheckBox.Enabled = true;
                RestInFightMpLowerCheckBox.Enabled = true;
            }
        }
        private void NeverRestCheckBox_Click(object sender, EventArgs e)
        {
            if (NeverRestCheckBox.Enabled)
            {
                RestAggressiveRadioButton.Checked = false;
                RestNormalRadioButton.Checked = false;
                RestLightRadioButton.Checked = false;
                RestCustomRadioButton.Checked = true;
                restStyle = (int)REST_STYLE.CUSTOM;
            }
        }
        private void RestInFightCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!RestInFightCheckBox.Checked)
            {
                RestInFightAlwaysCheckBox.Checked = false;
                RestInFightAlwaysCheckBox.Enabled = false;
                RestInFightMpUpperCheckBox.Checked = false;
                RestInFightMpUpperCheckBox.Enabled = false;
                RestInFightMpLowerCheckBox.Checked = false;
                RestInFightMpLowerCheckBox.Enabled = false;
            }
            else
            {
                RestInFightAlwaysCheckBox.Enabled = true;
                RestInFightMpUpperCheckBox.Enabled = true;
                RestInFightMpLowerCheckBox.Enabled = true;
            }
        }
        private void RestInFightCheckBox_Click(object sender, EventArgs e)
        {
            if (RestInFightCheckBox.Enabled)
            {
                RestAggressiveRadioButton.Checked = false;
                RestNormalRadioButton.Checked = false;
                RestLightRadioButton.Checked = false;
                RestCustomRadioButton.Checked = true;
                restStyle = (int)REST_STYLE.CUSTOM;
            }
        }
        private void RestInFightAlwaysCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (RestInFightAlwaysCheckBox.Checked)
            {
                RestInFightMpUpperCheckBox.Checked = true;
                RestInFightMpUpperCheckBox.Enabled = false;
                RestInFightMpLowerCheckBox.Checked = true;
                RestInFightMpLowerCheckBox.Enabled = false;
            }
            else
            {
                RestInFightMpUpperCheckBox.Enabled = true;
                RestInFightMpLowerCheckBox.Enabled = false;
            }
        }
        private void RestInFightAlwaysCheckBox_Click(object sender, EventArgs e)
        {
            if (RestInFightAlwaysCheckBox.Enabled)
            {
                RestAggressiveRadioButton.Checked = false;
                RestNormalRadioButton.Checked = false;
                RestLightRadioButton.Checked = false;
                RestCustomRadioButton.Checked = true;
                restStyle = (int)REST_STYLE.CUSTOM;
            }
        }
        private void RestInFightMpUpperCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (RestInFightMpUpperCheckBox.Checked)
            {
                RestInFightMpLowerCheckBox.Checked = true;
                RestInFightMpLowerCheckBox.Enabled = false;
            }
            else
            {
                RestInFightMpLowerCheckBox.Enabled = true;
            }
        }
        private void RestInFightMpUpperCheckBox_Click(object sender, EventArgs e)
        {
            if (RestInFightMpUpperCheckBox.Enabled)
            {
                RestAggressiveRadioButton.Checked = false;
                RestNormalRadioButton.Checked = false;
                RestLightRadioButton.Checked = false;
                RestCustomRadioButton.Checked = true;
                restStyle = (int)REST_STYLE.CUSTOM;
            }
        }
        private void RestInFightMpLowerCheckBox_Click(object sender, EventArgs e)
        {
            if (RestInFightMpLowerCheckBox.Enabled)
            {
                RestAggressiveRadioButton.Checked = false;
                RestNormalRadioButton.Checked = false;
                RestLightRadioButton.Checked = false;
                RestCustomRadioButton.Checked = true;
                restStyle = (int)REST_STYLE.CUSTOM;
            }
        }
        #endregion
    }
}
