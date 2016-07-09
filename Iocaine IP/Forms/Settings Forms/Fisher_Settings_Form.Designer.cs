namespace Iocaine2.Settings
{
    partial class Fisher_Settings_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.OK_Button = new System.Windows.Forms.Button();
            this.Apply_Button = new System.Windows.Forms.Button();
            this.KillFishCheckBox = new System.Windows.Forms.CheckBox();
            this.FixedTimeRadioButton = new System.Windows.Forms.RadioButton();
            this.RandTimeRadioButton = new System.Windows.Forms.RadioButton();
            this.PropTimeRadioButton = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.KillFishPropTimeMin = new System.Windows.Forms.Label();
            this.KillFishPropTimeMax = new System.Windows.Forms.Label();
            this.KillFishRandTimeMax = new System.Windows.Forms.Label();
            this.KillFishRandTimeMin = new System.Windows.Forms.Label();
            this.KillFishFixedTime = new System.Windows.Forms.Label();
            this.PropTimeMaxTextBox = new System.Windows.Forms.MaskedTextBox();
            this.PropTimeMinTextBox = new System.Windows.Forms.MaskedTextBox();
            this.RandTimeMaxTextBox = new System.Windows.Forms.MaskedTextBox();
            this.RandTimeMinTextBox = new System.Windows.Forms.MaskedTextBox();
            this.FixedTimeTextBox = new System.Windows.Forms.MaskedTextBox();
            this.CommandTextBox = new System.Windows.Forms.TextBox();
            this.GiveCommandCheckBox = new System.Windows.Forms.CheckBox();
            this.WhenDoneFishingLabel = new System.Windows.Forms.Label();
            this.WhenInvFullLabel = new System.Windows.Forms.Label();
            this.DoneTripCB = new System.Windows.Forms.ComboBox();
            this.DoneActionCB = new System.Windows.Forms.ComboBox();
            this.DoneBrowseFileToPlayButton = new System.Windows.Forms.Button();
            this.DonePlaySoundLabel = new System.Windows.Forms.Label();
            this.DonePlaySoundTB = new System.Windows.Forms.TextBox();
            this.WhenFullBrowseFileToPlayButton = new System.Windows.Forms.Button();
            this.WhenFullPlaySoundLabel = new System.Windows.Forms.Label();
            this.WhenFullPlaySoundTB = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.DropMobsCheckBox = new System.Windows.Forms.CheckBox();
            this.DropItemsCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.FishByHPRadioButton = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.FishByIDRadioButton = new System.Windows.Forms.RadioButton();
            this.FishByHPMaxTextBox = new System.Windows.Forms.MaskedTextBox();
            this.FishByHPMinTextBox = new System.Windows.Forms.MaskedTextBox();
            this.NotationLabel = new System.Windows.Forms.Label();
            this.NoCatchLabel = new System.Windows.Forms.Label();
            this.NoCatchForm = new System.Windows.Forms.MaskedTextBox();
            this.FixedReleaseTextBox = new System.Windows.Forms.MaskedTextBox();
            this.FixedReleaseTimeLabel = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.RandomReleaseMaxTextBox = new System.Windows.Forms.MaskedTextBox();
            this.RandomReleaseRadioButton = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.RandomReleaseMinTextBox = new System.Windows.Forms.MaskedTextBox();
            this.FixedReleaseRadioButton = new System.Windows.Forms.RadioButton();
            this.TimedStartCheckBox = new System.Windows.Forms.CheckBox();
            this.TimedEndCheckBox = new System.Windows.Forms.CheckBox();
            this.TimedStartTextBox = new System.Windows.Forms.TextBox();
            this.TimedEndTextBox = new System.Windows.Forms.TextBox();
            this.DoneOpenFileToPlay = new System.Windows.Forms.OpenFileDialog();
            this.WhenFullOpenFileToPlay = new System.Windows.Forms.OpenFileDialog();
            this.FullTripCB = new System.Windows.Forms.ComboBox();
            this.FullActionCB = new System.Windows.Forms.ComboBox();
            this.FatiguedNavChkB = new System.Windows.Forms.CheckBox();
            this.FatigueThresholdUpDn = new System.Windows.Forms.NumericUpDown();
            this.FatiguedTripCB = new System.Windows.Forms.ComboBox();
            this.InvMovementChkBox = new System.Windows.Forms.CheckBox();
            this.RecastDelayLabel = new System.Windows.Forms.Label();
            this.RecastDelayUpDn = new System.Windows.Forms.NumericUpDown();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FatigueThresholdUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RecastDelayUpDn)).BeginInit();
            this.SuspendLayout();
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Button.Location = new System.Drawing.Point(314, 372);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(60, 23);
            this.Cancel_Button.TabIndex = 0;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // OK_Button
            // 
            this.OK_Button.Location = new System.Drawing.Point(380, 372);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(60, 23);
            this.OK_Button.TabIndex = 1;
            this.OK_Button.Text = "OK";
            this.OK_Button.UseVisualStyleBackColor = true;
            this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
            // 
            // Apply_Button
            // 
            this.Apply_Button.Location = new System.Drawing.Point(248, 372);
            this.Apply_Button.Name = "Apply_Button";
            this.Apply_Button.Size = new System.Drawing.Size(60, 23);
            this.Apply_Button.TabIndex = 2;
            this.Apply_Button.Text = "Apply";
            this.Apply_Button.UseVisualStyleBackColor = true;
            this.Apply_Button.Click += new System.EventHandler(this.Apply_Button_Click);
            // 
            // KillFishCheckBox
            // 
            this.KillFishCheckBox.AutoSize = true;
            this.KillFishCheckBox.Location = new System.Drawing.Point(12, 8);
            this.KillFishCheckBox.Name = "KillFishCheckBox";
            this.KillFishCheckBox.Size = new System.Drawing.Size(61, 17);
            this.KillFishCheckBox.TabIndex = 4;
            this.KillFishCheckBox.Text = "Kill Fish";
            this.KillFishCheckBox.UseVisualStyleBackColor = true;
            // 
            // FixedTimeRadioButton
            // 
            this.FixedTimeRadioButton.AutoSize = true;
            this.FixedTimeRadioButton.Checked = true;
            this.FixedTimeRadioButton.Location = new System.Drawing.Point(17, 12);
            this.FixedTimeRadioButton.Name = "FixedTimeRadioButton";
            this.FixedTimeRadioButton.Size = new System.Drawing.Size(76, 17);
            this.FixedTimeRadioButton.TabIndex = 5;
            this.FixedTimeRadioButton.TabStop = true;
            this.FixedTimeRadioButton.Text = "Fixed Time";
            this.FixedTimeRadioButton.UseVisualStyleBackColor = true;
            // 
            // RandTimeRadioButton
            // 
            this.RandTimeRadioButton.AutoSize = true;
            this.RandTimeRadioButton.Location = new System.Drawing.Point(17, 31);
            this.RandTimeRadioButton.Name = "RandTimeRadioButton";
            this.RandTimeRadioButton.Size = new System.Drawing.Size(91, 17);
            this.RandTimeRadioButton.TabIndex = 6;
            this.RandTimeRadioButton.Text = "Random Time";
            this.RandTimeRadioButton.UseVisualStyleBackColor = true;
            // 
            // PropTimeRadioButton
            // 
            this.PropTimeRadioButton.AutoSize = true;
            this.PropTimeRadioButton.Location = new System.Drawing.Point(17, 74);
            this.PropTimeRadioButton.Name = "PropTimeRadioButton";
            this.PropTimeRadioButton.Size = new System.Drawing.Size(81, 17);
            this.PropTimeRadioButton.TabIndex = 7;
            this.PropTimeRadioButton.Text = "Proportional";
            this.PropTimeRadioButton.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.KillFishPropTimeMin);
            this.panel1.Controls.Add(this.KillFishPropTimeMax);
            this.panel1.Controls.Add(this.KillFishRandTimeMax);
            this.panel1.Controls.Add(this.KillFishRandTimeMin);
            this.panel1.Controls.Add(this.KillFishFixedTime);
            this.panel1.Controls.Add(this.PropTimeMaxTextBox);
            this.panel1.Controls.Add(this.PropTimeMinTextBox);
            this.panel1.Controls.Add(this.RandTimeMaxTextBox);
            this.panel1.Controls.Add(this.RandTimeMinTextBox);
            this.panel1.Controls.Add(this.FixedTimeTextBox);
            this.panel1.Controls.Add(this.PropTimeRadioButton);
            this.panel1.Controls.Add(this.FixedTimeRadioButton);
            this.panel1.Controls.Add(this.RandTimeRadioButton);
            this.panel1.Location = new System.Drawing.Point(12, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(210, 122);
            this.panel1.TabIndex = 8;
            // 
            // KillFishPropTimeMin
            // 
            this.KillFishPropTimeMin.AutoSize = true;
            this.KillFishPropTimeMin.Location = new System.Drawing.Point(53, 95);
            this.KillFishPropTimeMin.Name = "KillFishPropTimeMin";
            this.KillFishPropTimeMin.Size = new System.Drawing.Size(24, 13);
            this.KillFishPropTimeMin.TabIndex = 14;
            this.KillFishPropTimeMin.Text = "Min";
            // 
            // KillFishPropTimeMax
            // 
            this.KillFishPropTimeMax.AutoSize = true;
            this.KillFishPropTimeMax.Location = new System.Drawing.Point(129, 95);
            this.KillFishPropTimeMax.Name = "KillFishPropTimeMax";
            this.KillFishPropTimeMax.Size = new System.Drawing.Size(27, 13);
            this.KillFishPropTimeMax.TabIndex = 13;
            this.KillFishPropTimeMax.Text = "Max";
            // 
            // KillFishRandTimeMax
            // 
            this.KillFishRandTimeMax.AutoSize = true;
            this.KillFishRandTimeMax.Location = new System.Drawing.Point(127, 55);
            this.KillFishRandTimeMax.Name = "KillFishRandTimeMax";
            this.KillFishRandTimeMax.Size = new System.Drawing.Size(27, 13);
            this.KillFishRandTimeMax.TabIndex = 12;
            this.KillFishRandTimeMax.Text = "Max";
            // 
            // KillFishRandTimeMin
            // 
            this.KillFishRandTimeMin.AutoSize = true;
            this.KillFishRandTimeMin.Location = new System.Drawing.Point(52, 55);
            this.KillFishRandTimeMin.Name = "KillFishRandTimeMin";
            this.KillFishRandTimeMin.Size = new System.Drawing.Size(24, 13);
            this.KillFishRandTimeMin.TabIndex = 11;
            this.KillFishRandTimeMin.Text = "Min";
            // 
            // KillFishFixedTime
            // 
            this.KillFishFixedTime.AutoSize = true;
            this.KillFishFixedTime.Location = new System.Drawing.Point(138, 12);
            this.KillFishFixedTime.Name = "KillFishFixedTime";
            this.KillFishFixedTime.Size = new System.Drawing.Size(49, 13);
            this.KillFishFixedTime.TabIndex = 10;
            this.KillFishFixedTime.Text = "Seconds";
            // 
            // PropTimeMaxTextBox
            // 
            this.PropTimeMaxTextBox.AsciiOnly = true;
            this.PropTimeMaxTextBox.Location = new System.Drawing.Point(92, 92);
            this.PropTimeMaxTextBox.Mask = "90";
            this.PropTimeMaxTextBox.Name = "PropTimeMaxTextBox";
            this.PropTimeMaxTextBox.PromptChar = ' ';
            this.PropTimeMaxTextBox.Size = new System.Drawing.Size(31, 20);
            this.PropTimeMaxTextBox.TabIndex = 9;
            this.PropTimeMaxTextBox.Text = "14";
            this.PropTimeMaxTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // PropTimeMinTextBox
            // 
            this.PropTimeMinTextBox.AsciiOnly = true;
            this.PropTimeMinTextBox.Location = new System.Drawing.Point(17, 92);
            this.PropTimeMinTextBox.Mask = "90";
            this.PropTimeMinTextBox.Name = "PropTimeMinTextBox";
            this.PropTimeMinTextBox.PromptChar = ' ';
            this.PropTimeMinTextBox.Size = new System.Drawing.Size(31, 20);
            this.PropTimeMinTextBox.TabIndex = 8;
            this.PropTimeMinTextBox.Text = "6";
            this.PropTimeMinTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // RandTimeMaxTextBox
            // 
            this.RandTimeMaxTextBox.AsciiOnly = true;
            this.RandTimeMaxTextBox.Location = new System.Drawing.Point(90, 52);
            this.RandTimeMaxTextBox.Mask = "90";
            this.RandTimeMaxTextBox.Name = "RandTimeMaxTextBox";
            this.RandTimeMaxTextBox.PromptChar = ' ';
            this.RandTimeMaxTextBox.Size = new System.Drawing.Size(31, 20);
            this.RandTimeMaxTextBox.TabIndex = 7;
            this.RandTimeMaxTextBox.Text = "14";
            this.RandTimeMaxTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // RandTimeMinTextBox
            // 
            this.RandTimeMinTextBox.AsciiOnly = true;
            this.RandTimeMinTextBox.Location = new System.Drawing.Point(15, 52);
            this.RandTimeMinTextBox.Mask = "90";
            this.RandTimeMinTextBox.Name = "RandTimeMinTextBox";
            this.RandTimeMinTextBox.PromptChar = ' ';
            this.RandTimeMinTextBox.Size = new System.Drawing.Size(31, 20);
            this.RandTimeMinTextBox.TabIndex = 6;
            this.RandTimeMinTextBox.Text = "8";
            this.RandTimeMinTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FixedTimeTextBox
            // 
            this.FixedTimeTextBox.AsciiOnly = true;
            this.FixedTimeTextBox.Location = new System.Drawing.Point(101, 9);
            this.FixedTimeTextBox.Mask = "90";
            this.FixedTimeTextBox.Name = "FixedTimeTextBox";
            this.FixedTimeTextBox.PromptChar = ' ';
            this.FixedTimeTextBox.Size = new System.Drawing.Size(31, 20);
            this.FixedTimeTextBox.TabIndex = 5;
            this.FixedTimeTextBox.Text = "8";
            this.FixedTimeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CommandTextBox
            // 
            this.CommandTextBox.Location = new System.Drawing.Point(219, 332);
            this.CommandTextBox.Name = "CommandTextBox";
            this.CommandTextBox.Size = new System.Drawing.Size(217, 20);
            this.CommandTextBox.TabIndex = 19;
            this.CommandTextBox.Text = "/ma Warp <me>";
            // 
            // GiveCommandCheckBox
            // 
            this.GiveCommandCheckBox.Location = new System.Drawing.Point(211, 309);
            this.GiveCommandCheckBox.Name = "GiveCommandCheckBox";
            this.GiveCommandCheckBox.Size = new System.Drawing.Size(225, 17);
            this.GiveCommandCheckBox.TabIndex = 0;
            this.GiveCommandCheckBox.Text = "Give cmd when logging out or stopping";
            this.GiveCommandCheckBox.UseVisualStyleBackColor = true;
            // 
            // WhenDoneFishingLabel
            // 
            this.WhenDoneFishingLabel.AutoSize = true;
            this.WhenDoneFishingLabel.Location = new System.Drawing.Point(230, 8);
            this.WhenDoneFishingLabel.Name = "WhenDoneFishingLabel";
            this.WhenDoneFishingLabel.Size = new System.Drawing.Size(107, 13);
            this.WhenDoneFishingLabel.TabIndex = 9;
            this.WhenDoneFishingLabel.Text = "- When Done Fishing";
            // 
            // WhenInvFullLabel
            // 
            this.WhenInvFullLabel.AutoSize = true;
            this.WhenInvFullLabel.Location = new System.Drawing.Point(230, 110);
            this.WhenInvFullLabel.Name = "WhenInvFullLabel";
            this.WhenInvFullLabel.Size = new System.Drawing.Size(115, 13);
            this.WhenInvFullLabel.TabIndex = 10;
            this.WhenInvFullLabel.Text = "- When Inventory\'s Full";
            // 
            // DoneTripCB
            // 
            this.DoneTripCB.FormattingEnabled = true;
            this.DoneTripCB.Location = new System.Drawing.Point(242, 48);
            this.DoneTripCB.Name = "DoneTripCB";
            this.DoneTripCB.Size = new System.Drawing.Size(162, 21);
            this.DoneTripCB.TabIndex = 37;
            // 
            // DoneActionCB
            // 
            this.DoneActionCB.FormattingEnabled = true;
            this.DoneActionCB.Location = new System.Drawing.Point(233, 23);
            this.DoneActionCB.Name = "DoneActionCB";
            this.DoneActionCB.Size = new System.Drawing.Size(171, 21);
            this.DoneActionCB.TabIndex = 36;
            // 
            // DoneBrowseFileToPlayButton
            // 
            this.DoneBrowseFileToPlayButton.Location = new System.Drawing.Point(409, 86);
            this.DoneBrowseFileToPlayButton.Name = "DoneBrowseFileToPlayButton";
            this.DoneBrowseFileToPlayButton.Size = new System.Drawing.Size(22, 21);
            this.DoneBrowseFileToPlayButton.TabIndex = 35;
            this.DoneBrowseFileToPlayButton.Text = "...";
            this.DoneBrowseFileToPlayButton.UseVisualStyleBackColor = true;
            this.DoneBrowseFileToPlayButton.Click += new System.EventHandler(this.DoneBrowseFileToPlayButton_Click);
            // 
            // DonePlaySoundLabel
            // 
            this.DonePlaySoundLabel.AutoSize = true;
            this.DonePlaySoundLabel.Location = new System.Drawing.Point(239, 71);
            this.DonePlaySoundLabel.Name = "DonePlaySoundLabel";
            this.DonePlaySoundLabel.Size = new System.Drawing.Size(128, 13);
            this.DonePlaySoundLabel.TabIndex = 34;
            this.DonePlaySoundLabel.Text = "Say Message or Play File:";
            // 
            // DonePlaySoundTB
            // 
            this.DonePlaySoundTB.Location = new System.Drawing.Point(233, 87);
            this.DonePlaySoundTB.Name = "DonePlaySoundTB";
            this.DonePlaySoundTB.Size = new System.Drawing.Size(171, 20);
            this.DonePlaySoundTB.TabIndex = 33;
            this.DonePlaySoundTB.Text = "Text to Say or File to Play";
            // 
            // WhenFullBrowseFileToPlayButton
            // 
            this.WhenFullBrowseFileToPlayButton.Location = new System.Drawing.Point(409, 189);
            this.WhenFullBrowseFileToPlayButton.Name = "WhenFullBrowseFileToPlayButton";
            this.WhenFullBrowseFileToPlayButton.Size = new System.Drawing.Size(22, 21);
            this.WhenFullBrowseFileToPlayButton.TabIndex = 38;
            this.WhenFullBrowseFileToPlayButton.Text = "...";
            this.WhenFullBrowseFileToPlayButton.UseVisualStyleBackColor = true;
            this.WhenFullBrowseFileToPlayButton.Click += new System.EventHandler(this.WhenFullBrowseFileToPlayButton_Click);
            // 
            // WhenFullPlaySoundLabel
            // 
            this.WhenFullPlaySoundLabel.AutoSize = true;
            this.WhenFullPlaySoundLabel.Location = new System.Drawing.Point(239, 173);
            this.WhenFullPlaySoundLabel.Name = "WhenFullPlaySoundLabel";
            this.WhenFullPlaySoundLabel.Size = new System.Drawing.Size(128, 13);
            this.WhenFullPlaySoundLabel.TabIndex = 37;
            this.WhenFullPlaySoundLabel.Text = "Say Message or Play File:";
            // 
            // WhenFullPlaySoundTB
            // 
            this.WhenFullPlaySoundTB.Location = new System.Drawing.Point(233, 190);
            this.WhenFullPlaySoundTB.Name = "WhenFullPlaySoundTB";
            this.WhenFullPlaySoundTB.Size = new System.Drawing.Size(171, 20);
            this.WhenFullPlaySoundTB.TabIndex = 36;
            this.WhenFullPlaySoundTB.Text = "Text to Say or File to Play";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.DropMobsCheckBox);
            this.panel2.Controls.Add(this.DropItemsCheckBox);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.FishByHPRadioButton);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.FishByIDRadioButton);
            this.panel2.Controls.Add(this.FishByHPMaxTextBox);
            this.panel2.Controls.Add(this.FishByHPMinTextBox);
            this.panel2.Location = new System.Drawing.Point(12, 145);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(210, 96);
            this.panel2.TabIndex = 21;
            // 
            // DropMobsCheckBox
            // 
            this.DropMobsCheckBox.AutoSize = true;
            this.DropMobsCheckBox.Location = new System.Drawing.Point(123, 26);
            this.DropMobsCheckBox.Name = "DropMobsCheckBox";
            this.DropMobsCheckBox.Size = new System.Drawing.Size(85, 17);
            this.DropMobsCheckBox.TabIndex = 20;
            this.DropMobsCheckBox.Text = "Drop Mobs *";
            this.DropMobsCheckBox.UseVisualStyleBackColor = true;
            // 
            // DropItemsCheckBox
            // 
            this.DropItemsCheckBox.AutoSize = true;
            this.DropItemsCheckBox.Location = new System.Drawing.Point(36, 26);
            this.DropItemsCheckBox.Name = "DropItemsCheckBox";
            this.DropItemsCheckBox.Size = new System.Drawing.Size(84, 17);
            this.DropItemsCheckBox.TabIndex = 19;
            this.DropItemsCheckBox.Text = "Drop Items *";
            this.DropItemsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(132, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Max";
            // 
            // FishByHPRadioButton
            // 
            this.FishByHPRadioButton.AutoSize = true;
            this.FishByHPRadioButton.Location = new System.Drawing.Point(17, 45);
            this.FishByHPRadioButton.Name = "FishByHPRadioButton";
            this.FishByHPRadioButton.Size = new System.Drawing.Size(77, 17);
            this.FishByHPRadioButton.TabIndex = 1;
            this.FishByHPRadioButton.TabStop = true;
            this.FishByHPRadioButton.Text = "Fish By HP";
            this.FishByHPRadioButton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(56, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Min";
            // 
            // FishByIDRadioButton
            // 
            this.FishByIDRadioButton.AutoSize = true;
            this.FishByIDRadioButton.Checked = true;
            this.FishByIDRadioButton.Location = new System.Drawing.Point(17, 6);
            this.FishByIDRadioButton.Name = "FishByIDRadioButton";
            this.FishByIDRadioButton.Size = new System.Drawing.Size(73, 17);
            this.FishByIDRadioButton.TabIndex = 0;
            this.FishByIDRadioButton.TabStop = true;
            this.FishByIDRadioButton.Text = "Fish By ID";
            this.FishByIDRadioButton.UseVisualStyleBackColor = true;
            // 
            // FishByHPMaxTextBox
            // 
            this.FishByHPMaxTextBox.AsciiOnly = true;
            this.FishByHPMaxTextBox.Location = new System.Drawing.Point(92, 63);
            this.FishByHPMaxTextBox.Mask = "0000";
            this.FishByHPMaxTextBox.Name = "FishByHPMaxTextBox";
            this.FishByHPMaxTextBox.PromptChar = ' ';
            this.FishByHPMaxTextBox.Size = new System.Drawing.Size(36, 20);
            this.FishByHPMaxTextBox.TabIndex = 16;
            this.FishByHPMaxTextBox.Text = "6500";
            this.FishByHPMaxTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FishByHPMinTextBox
            // 
            this.FishByHPMinTextBox.AsciiOnly = true;
            this.FishByHPMinTextBox.Location = new System.Drawing.Point(17, 63);
            this.FishByHPMinTextBox.Mask = "0000";
            this.FishByHPMinTextBox.Name = "FishByHPMinTextBox";
            this.FishByHPMinTextBox.PromptChar = ' ';
            this.FishByHPMinTextBox.Size = new System.Drawing.Size(36, 20);
            this.FishByHPMinTextBox.TabIndex = 15;
            this.FishByHPMinTextBox.Text = "2500";
            this.FishByHPMinTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // NotationLabel
            // 
            this.NotationLabel.AutoSize = true;
            this.NotationLabel.Location = new System.Drawing.Point(10, 245);
            this.NotationLabel.Name = "NotationLabel";
            this.NotationLabel.Size = new System.Drawing.Size(227, 13);
            this.NotationLabel.TabIndex = 22;
            this.NotationLabel.Text = "* Note: Drop Items/Mobs is not 100% accurate";
            // 
            // NoCatchLabel
            // 
            this.NoCatchLabel.AutoSize = true;
            this.NoCatchLabel.Location = new System.Drawing.Point(46, 361);
            this.NoCatchLabel.Name = "NoCatchLabel";
            this.NoCatchLabel.Size = new System.Drawing.Size(93, 13);
            this.NoCatchLabel.TabIndex = 24;
            this.NoCatchLabel.Text = "No Catch Timeout";
            // 
            // NoCatchForm
            // 
            this.NoCatchForm.Location = new System.Drawing.Point(13, 358);
            this.NoCatchForm.Mask = "90";
            this.NoCatchForm.Name = "NoCatchForm";
            this.NoCatchForm.PromptChar = ' ';
            this.NoCatchForm.Size = new System.Drawing.Size(27, 20);
            this.NoCatchForm.TabIndex = 25;
            this.NoCatchForm.Text = "20";
            this.NoCatchForm.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FixedReleaseTextBox
            // 
            this.FixedReleaseTextBox.Location = new System.Drawing.Point(100, 6);
            this.FixedReleaseTextBox.Mask = "0.0";
            this.FixedReleaseTextBox.Name = "FixedReleaseTextBox";
            this.FixedReleaseTextBox.PromptChar = ' ';
            this.FixedReleaseTextBox.Size = new System.Drawing.Size(27, 20);
            this.FixedReleaseTextBox.TabIndex = 26;
            this.FixedReleaseTextBox.Text = "28";
            this.FixedReleaseTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FixedReleaseTimeLabel
            // 
            this.FixedReleaseTimeLabel.AutoSize = true;
            this.FixedReleaseTimeLabel.Location = new System.Drawing.Point(133, 9);
            this.FixedReleaseTimeLabel.Name = "FixedReleaseTimeLabel";
            this.FixedReleaseTimeLabel.Size = new System.Drawing.Size(49, 13);
            this.FixedReleaseTimeLabel.TabIndex = 27;
            this.FixedReleaseTimeLabel.Text = "Seconds";
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.RandomReleaseMaxTextBox);
            this.panel3.Controls.Add(this.RandomReleaseRadioButton);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.RandomReleaseMinTextBox);
            this.panel3.Controls.Add(this.FixedReleaseRadioButton);
            this.panel3.Controls.Add(this.FixedReleaseTimeLabel);
            this.panel3.Controls.Add(this.FixedReleaseTextBox);
            this.panel3.Location = new System.Drawing.Point(13, 265);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(192, 65);
            this.panel3.TabIndex = 28;
            // 
            // RandomReleaseMaxTextBox
            // 
            this.RandomReleaseMaxTextBox.Location = new System.Drawing.Point(155, 29);
            this.RandomReleaseMaxTextBox.Mask = "0.0";
            this.RandomReleaseMaxTextBox.Name = "RandomReleaseMaxTextBox";
            this.RandomReleaseMaxTextBox.PromptChar = ' ';
            this.RandomReleaseMaxTextBox.Size = new System.Drawing.Size(27, 20);
            this.RandomReleaseMaxTextBox.TabIndex = 31;
            this.RandomReleaseMaxTextBox.Text = "42";
            this.RandomReleaseMaxTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // RandomReleaseRadioButton
            // 
            this.RandomReleaseRadioButton.AutoSize = true;
            this.RandomReleaseRadioButton.Checked = true;
            this.RandomReleaseRadioButton.Location = new System.Drawing.Point(15, 30);
            this.RandomReleaseRadioButton.Name = "RandomReleaseRadioButton";
            this.RandomReleaseRadioButton.Size = new System.Drawing.Size(95, 17);
            this.RandomReleaseRadioButton.TabIndex = 28;
            this.RandomReleaseRadioButton.TabStop = true;
            this.RandomReleaseRadioButton.Text = "Random Delay";
            this.RandomReleaseRadioButton.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(143, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "-";
            // 
            // RandomReleaseMinTextBox
            // 
            this.RandomReleaseMinTextBox.Location = new System.Drawing.Point(114, 29);
            this.RandomReleaseMinTextBox.Mask = "0.0";
            this.RandomReleaseMinTextBox.Name = "RandomReleaseMinTextBox";
            this.RandomReleaseMinTextBox.PromptChar = ' ';
            this.RandomReleaseMinTextBox.Size = new System.Drawing.Size(27, 20);
            this.RandomReleaseMinTextBox.TabIndex = 29;
            this.RandomReleaseMinTextBox.Text = "18";
            this.RandomReleaseMinTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FixedReleaseRadioButton
            // 
            this.FixedReleaseRadioButton.AutoSize = true;
            this.FixedReleaseRadioButton.Location = new System.Drawing.Point(14, 7);
            this.FixedReleaseRadioButton.Name = "FixedReleaseRadioButton";
            this.FixedReleaseRadioButton.Size = new System.Drawing.Size(80, 17);
            this.FixedReleaseRadioButton.TabIndex = 21;
            this.FixedReleaseRadioButton.Text = "Fixed Delay";
            this.FixedReleaseRadioButton.UseVisualStyleBackColor = true;
            // 
            // TimedStartCheckBox
            // 
            this.TimedStartCheckBox.AutoSize = true;
            this.TimedStartCheckBox.Location = new System.Drawing.Point(211, 271);
            this.TimedStartCheckBox.Name = "TimedStartCheckBox";
            this.TimedStartCheckBox.Size = new System.Drawing.Size(80, 17);
            this.TimedStartCheckBox.TabIndex = 29;
            this.TimedStartCheckBox.Text = "Timed Start";
            this.TimedStartCheckBox.UseVisualStyleBackColor = true;
            // 
            // TimedEndCheckBox
            // 
            this.TimedEndCheckBox.AutoSize = true;
            this.TimedEndCheckBox.Location = new System.Drawing.Point(211, 290);
            this.TimedEndCheckBox.Name = "TimedEndCheckBox";
            this.TimedEndCheckBox.Size = new System.Drawing.Size(77, 17);
            this.TimedEndCheckBox.TabIndex = 30;
            this.TimedEndCheckBox.Text = "Timed End";
            this.TimedEndCheckBox.UseVisualStyleBackColor = true;
            // 
            // TimedStartTextBox
            // 
            this.TimedStartTextBox.Location = new System.Drawing.Point(294, 267);
            this.TimedStartTextBox.Name = "TimedStartTextBox";
            this.TimedStartTextBox.Size = new System.Drawing.Size(68, 20);
            this.TimedStartTextBox.TabIndex = 31;
            this.TimedStartTextBox.Text = "8:00 am";
            // 
            // TimedEndTextBox
            // 
            this.TimedEndTextBox.Location = new System.Drawing.Point(294, 289);
            this.TimedEndTextBox.Name = "TimedEndTextBox";
            this.TimedEndTextBox.Size = new System.Drawing.Size(68, 20);
            this.TimedEndTextBox.TabIndex = 32;
            this.TimedEndTextBox.Text = "2:00 pm";
            // 
            // DoneOpenFileToPlay
            // 
            this.DoneOpenFileToPlay.FileName = "DoneOpenFileToPlay";
            // 
            // WhenFullOpenFileToPlay
            // 
            this.WhenFullOpenFileToPlay.FileName = "WhenFullOpenFileToPlay";
            // 
            // FullTripCB
            // 
            this.FullTripCB.FormattingEnabled = true;
            this.FullTripCB.Location = new System.Drawing.Point(242, 150);
            this.FullTripCB.Name = "FullTripCB";
            this.FullTripCB.Size = new System.Drawing.Size(162, 21);
            this.FullTripCB.TabIndex = 39;
            // 
            // FullActionCB
            // 
            this.FullActionCB.FormattingEnabled = true;
            this.FullActionCB.Location = new System.Drawing.Point(233, 125);
            this.FullActionCB.Name = "FullActionCB";
            this.FullActionCB.Size = new System.Drawing.Size(171, 21);
            this.FullActionCB.TabIndex = 38;
            // 
            // FatiguedNavChkB
            // 
            this.FatiguedNavChkB.AutoSize = true;
            this.FatiguedNavChkB.Location = new System.Drawing.Point(233, 214);
            this.FatiguedNavChkB.Name = "FatiguedNavChkB";
            this.FatiguedNavChkB.Size = new System.Drawing.Size(157, 17);
            this.FatiguedNavChkB.TabIndex = 21;
            this.FatiguedNavChkB.Text = "Run Trip When Fatigued @";
            this.FatiguedNavChkB.UseVisualStyleBackColor = true;
            // 
            // FatigueThresholdUpDn
            // 
            this.FatigueThresholdUpDn.Location = new System.Drawing.Point(388, 212);
            this.FatigueThresholdUpDn.Name = "FatigueThresholdUpDn";
            this.FatigueThresholdUpDn.Size = new System.Drawing.Size(42, 20);
            this.FatigueThresholdUpDn.TabIndex = 40;
            this.FatigueThresholdUpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.FatigueThresholdUpDn.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // FatiguedTripCB
            // 
            this.FatiguedTripCB.FormattingEnabled = true;
            this.FatiguedTripCB.Location = new System.Drawing.Point(242, 237);
            this.FatiguedTripCB.Name = "FatiguedTripCB";
            this.FatiguedTripCB.Size = new System.Drawing.Size(162, 21);
            this.FatiguedTripCB.TabIndex = 42;
            // 
            // InvMovementChkBox
            // 
            this.InvMovementChkBox.AutoSize = true;
            this.InvMovementChkBox.Location = new System.Drawing.Point(13, 336);
            this.InvMovementChkBox.Name = "InvMovementChkBox";
            this.InvMovementChkBox.Size = new System.Drawing.Size(200, 17);
            this.InvMovementChkBox.TabIndex = 43;
            this.InvMovementChkBox.Text = "Move Fish/Bait to/from sack/satchel";
            this.InvMovementChkBox.UseVisualStyleBackColor = true;
            // 
            // RecastDelayLabel
            // 
            this.RecastDelayLabel.AutoSize = true;
            this.RecastDelayLabel.Location = new System.Drawing.Point(79, 387);
            this.RecastDelayLabel.Name = "RecastDelayLabel";
            this.RecastDelayLabel.Size = new System.Drawing.Size(98, 13);
            this.RecastDelayLabel.TabIndex = 44;
            this.RecastDelayLabel.Text = "Initial Recast Delay";
            // 
            // RecastDelayUpDn
            // 
            this.RecastDelayUpDn.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.RecastDelayUpDn.Location = new System.Drawing.Point(13, 383);
            this.RecastDelayUpDn.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.RecastDelayUpDn.Name = "RecastDelayUpDn";
            this.RecastDelayUpDn.Size = new System.Drawing.Size(60, 20);
            this.RecastDelayUpDn.TabIndex = 45;
            this.RecastDelayUpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.RecastDelayUpDn.ThousandsSeparator = true;
            this.RecastDelayUpDn.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // Fisher_Settings_Form
            // 
            this.AcceptButton = this.OK_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_Button;
            this.ClientSize = new System.Drawing.Size(452, 407);
            this.Controls.Add(this.RecastDelayUpDn);
            this.Controls.Add(this.RecastDelayLabel);
            this.Controls.Add(this.InvMovementChkBox);
            this.Controls.Add(this.FatiguedTripCB);
            this.Controls.Add(this.FatigueThresholdUpDn);
            this.Controls.Add(this.FatiguedNavChkB);
            this.Controls.Add(this.FullTripCB);
            this.Controls.Add(this.DoneTripCB);
            this.Controls.Add(this.WhenFullBrowseFileToPlayButton);
            this.Controls.Add(this.TimedEndTextBox);
            this.Controls.Add(this.FullActionCB);
            this.Controls.Add(this.WhenFullPlaySoundLabel);
            this.Controls.Add(this.DoneActionCB);
            this.Controls.Add(this.WhenInvFullLabel);
            this.Controls.Add(this.TimedStartTextBox);
            this.Controls.Add(this.WhenFullPlaySoundTB);
            this.Controls.Add(this.DoneBrowseFileToPlayButton);
            this.Controls.Add(this.TimedEndCheckBox);
            this.Controls.Add(this.TimedStartCheckBox);
            this.Controls.Add(this.DonePlaySoundTB);
            this.Controls.Add(this.WhenDoneFishingLabel);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.NoCatchForm);
            this.Controls.Add(this.NoCatchLabel);
            this.Controls.Add(this.NotationLabel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.CommandTextBox);
            this.Controls.Add(this.GiveCommandCheckBox);
            this.Controls.Add(this.KillFishCheckBox);
            this.Controls.Add(this.Apply_Button);
            this.Controls.Add(this.OK_Button);
            this.Controls.Add(this.Cancel_Button);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.DonePlaySoundLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Location = new System.Drawing.Point(100, 100);
            this.MaximizeBox = false;
            this.Name = "Fisher_Settings_Form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Fisher Settings";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FatigueThresholdUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RecastDelayUpDn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.Button OK_Button;
        private System.Windows.Forms.Button Apply_Button;
        private System.Windows.Forms.CheckBox KillFishCheckBox;
        private System.Windows.Forms.RadioButton FixedTimeRadioButton;
        private System.Windows.Forms.RadioButton RandTimeRadioButton;
        private System.Windows.Forms.RadioButton PropTimeRadioButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MaskedTextBox FixedTimeTextBox;
        private System.Windows.Forms.MaskedTextBox PropTimeMaxTextBox;
        private System.Windows.Forms.MaskedTextBox PropTimeMinTextBox;
        private System.Windows.Forms.MaskedTextBox RandTimeMaxTextBox;
        private System.Windows.Forms.MaskedTextBox RandTimeMinTextBox;
        private System.Windows.Forms.Label KillFishPropTimeMin;
        private System.Windows.Forms.Label KillFishPropTimeMax;
        private System.Windows.Forms.Label KillFishRandTimeMax;
        private System.Windows.Forms.Label KillFishRandTimeMin;
        private System.Windows.Forms.Label KillFishFixedTime;
        private System.Windows.Forms.CheckBox GiveCommandCheckBox;
        private System.Windows.Forms.TextBox CommandTextBox;
        private System.Windows.Forms.Label WhenDoneFishingLabel;
        private System.Windows.Forms.Label WhenInvFullLabel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton FishByIDRadioButton;
        private System.Windows.Forms.CheckBox DropItemsCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton FishByHPRadioButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox FishByHPMaxTextBox;
        private System.Windows.Forms.MaskedTextBox FishByHPMinTextBox;
        private System.Windows.Forms.CheckBox DropMobsCheckBox;
        private System.Windows.Forms.Label NotationLabel;
        private System.Windows.Forms.Label NoCatchLabel;
        private System.Windows.Forms.MaskedTextBox NoCatchForm;
        private System.Windows.Forms.MaskedTextBox FixedReleaseTextBox;
        private System.Windows.Forms.Label FixedReleaseTimeLabel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton FixedReleaseRadioButton;
        private System.Windows.Forms.MaskedTextBox RandomReleaseMaxTextBox;
        private System.Windows.Forms.RadioButton RandomReleaseRadioButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MaskedTextBox RandomReleaseMinTextBox;
        private System.Windows.Forms.CheckBox TimedStartCheckBox;
        private System.Windows.Forms.CheckBox TimedEndCheckBox;
        private System.Windows.Forms.TextBox TimedStartTextBox;
        private System.Windows.Forms.TextBox TimedEndTextBox;
        private System.Windows.Forms.TextBox DonePlaySoundTB;
        private System.Windows.Forms.Label DonePlaySoundLabel;
        private System.Windows.Forms.Button DoneBrowseFileToPlayButton;
        private System.Windows.Forms.OpenFileDialog DoneOpenFileToPlay;
        private System.Windows.Forms.Button WhenFullBrowseFileToPlayButton;
        private System.Windows.Forms.Label WhenFullPlaySoundLabel;
        private System.Windows.Forms.TextBox WhenFullPlaySoundTB;
        private System.Windows.Forms.OpenFileDialog WhenFullOpenFileToPlay;
        private System.Windows.Forms.ComboBox DoneTripCB;
        private System.Windows.Forms.ComboBox DoneActionCB;
        private System.Windows.Forms.ComboBox FullTripCB;
        private System.Windows.Forms.ComboBox FullActionCB;
        private System.Windows.Forms.CheckBox FatiguedNavChkB;
        private System.Windows.Forms.NumericUpDown FatigueThresholdUpDn;
        private System.Windows.Forms.ComboBox FatiguedTripCB;
        private System.Windows.Forms.CheckBox InvMovementChkBox;
        private System.Windows.Forms.Label RecastDelayLabel;
        private System.Windows.Forms.NumericUpDown RecastDelayUpDn;
    }
}