namespace Iocaine2.Settings
{
    partial class Top_Settings_Form
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
            this.Apply_Button = new System.Windows.Forms.Button();
            this.OK_Button = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB = new System.Windows.Forms.CheckBox();
            this.Top_Settings_Kill_POL_On_Gm_Tell_ChkB = new System.Windows.Forms.CheckBox();
            this.Top_Settings_Stop_Fisher_On_GM_Tell_ChkB = new System.Windows.Forms.CheckBox();
            this.TopTabControl = new System.Windows.Forms.TabControl();
            this.GMTabPage = new System.Windows.Forms.TabPage();
            this.MenuNavTabPage = new System.Windows.Forms.TabPage();
            this.KeyHoldTimeLabel = new System.Windows.Forms.Label();
            this.MoveItemDelayLabel = new System.Windows.Forms.Label();
            this.MoveUpDownLabel = new System.Windows.Forms.Label();
            this.KeyHoldTimeUpDn = new System.Windows.Forms.NumericUpDown();
            this.MoveItemDelayUpDn = new System.Windows.Forms.NumericUpDown();
            this.MoveUpDownUpDn = new System.Windows.Forms.NumericUpDown();
            this.DebugTabPage = new System.Windows.Forms.TabPage();
            this.DebugRetainSettingsChkB = new System.Windows.Forms.CheckBox();
            this.DebugUnfilteredChkB = new System.Windows.Forms.CheckBox();
            this.DebugAllChkB = new System.Windows.Forms.CheckBox();
            this.DebugIocaineStructuresGroup = new System.Windows.Forms.GroupBox();
            this.DebugSettingsChkB = new System.Windows.Forms.CheckBox();
            this.DebugInteractionChkB = new System.Windows.Forms.CheckBox();
            this.DebugServerChkB = new System.Windows.Forms.CheckBox();
            this.DebugTimeChkB = new System.Windows.Forms.CheckBox();
            this.DebugCommandsChkB = new System.Windows.Forms.CheckBox();
            this.DebugGameStructuresGroup = new System.Windows.Forms.GroupBox();
            this.DebugNpcPcChkB = new System.Windows.Forms.CheckBox();
            this.DebugInventoryChkB = new System.Windows.Forms.CheckBox();
            this.DebugChatLogChkB = new System.Windows.Forms.CheckBox();
            this.DebugMemoryGroup = new System.Windows.Forms.GroupBox();
            this.DebugChangeMonChkB = new System.Windows.Forms.CheckBox();
            this.DebugWinAPIsChkB = new System.Windows.Forms.CheckBox();
            this.DebugMemScannerChkB = new System.Windows.Forms.CheckBox();
            this.DebugMemReadsChkB = new System.Windows.Forms.CheckBox();
            this.DebugBotsGroup = new System.Windows.Forms.GroupBox();
            this.DebugFishStatsChkB = new System.Windows.Forms.CheckBox();
            this.DebugSynergyChkB = new System.Windows.Forms.CheckBox();
            this.DebugBackgroundChkB = new System.Windows.Forms.CheckBox();
            this.DebugTopChkB = new System.Windows.Forms.CheckBox();
            this.DebugWMSChkB = new System.Windows.Forms.CheckBox();
            this.DebugSellerChkB = new System.Windows.Forms.CheckBox();
            this.DebugBuyerChkB = new System.Windows.Forms.CheckBox();
            this.DebugTraderChkB = new System.Windows.Forms.CheckBox();
            this.DebugNavChkB = new System.Windows.Forms.CheckBox();
            this.DebugTAChkB = new System.Windows.Forms.CheckBox();
            this.DebugCrafterChkB = new System.Windows.Forms.CheckBox();
            this.DebugSUChkB = new System.Windows.Forms.CheckBox();
            this.DebugPLChkB = new System.Windows.Forms.CheckBox();
            this.DebugFisherChkB = new System.Windows.Forms.CheckBox();
            this.KeysTabPage = new System.Windows.Forms.TabPage();
            this.FishingLeftArrowKeyCB = new System.Windows.Forms.ComboBox();
            this.FishingLeftArrowKeyLabel = new System.Windows.Forms.Label();
            this.FishingRightArrowKeyLabel = new System.Windows.Forms.Label();
            this.FishingRightArrowKeyCB = new System.Windows.Forms.ComboBox();
            this.TopTabControl.SuspendLayout();
            this.GMTabPage.SuspendLayout();
            this.MenuNavTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.KeyHoldTimeUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveItemDelayUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveUpDownUpDn)).BeginInit();
            this.DebugTabPage.SuspendLayout();
            this.DebugIocaineStructuresGroup.SuspendLayout();
            this.DebugGameStructuresGroup.SuspendLayout();
            this.DebugMemoryGroup.SuspendLayout();
            this.DebugBotsGroup.SuspendLayout();
            this.KeysTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // Apply_Button
            // 
            this.Apply_Button.Location = new System.Drawing.Point(244, 295);
            this.Apply_Button.Name = "Apply_Button";
            this.Apply_Button.Size = new System.Drawing.Size(60, 23);
            this.Apply_Button.TabIndex = 11;
            this.Apply_Button.Text = "Apply";
            this.Apply_Button.UseVisualStyleBackColor = true;
            this.Apply_Button.Click += new System.EventHandler(this.Apply_Button_Click);
            // 
            // OK_Button
            // 
            this.OK_Button.Location = new System.Drawing.Point(376, 295);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(60, 23);
            this.OK_Button.TabIndex = 10;
            this.OK_Button.Text = "OK";
            this.OK_Button.UseVisualStyleBackColor = true;
            this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Button.Location = new System.Drawing.Point(310, 295);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(60, 23);
            this.Cancel_Button.TabIndex = 9;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB
            // 
            this.Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB.AutoSize = true;
            this.Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB.Location = new System.Drawing.Point(6, 29);
            this.Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB.Name = "Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB";
            this.Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB.Size = new System.Drawing.Size(143, 17);
            this.Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB.TabIndex = 43;
            this.Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB.Text = "Stop All Bots On GM Tell";
            this.Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB.UseVisualStyleBackColor = true;
            this.Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB.CheckedChanged += new System.EventHandler(this.Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB_CheckedChanged);
            // 
            // Top_Settings_Kill_POL_On_Gm_Tell_ChkB
            // 
            this.Top_Settings_Kill_POL_On_Gm_Tell_ChkB.AutoSize = true;
            this.Top_Settings_Kill_POL_On_Gm_Tell_ChkB.Location = new System.Drawing.Point(6, 6);
            this.Top_Settings_Kill_POL_On_Gm_Tell_ChkB.Name = "Top_Settings_Kill_POL_On_Gm_Tell_ChkB";
            this.Top_Settings_Kill_POL_On_Gm_Tell_ChkB.Size = new System.Drawing.Size(120, 17);
            this.Top_Settings_Kill_POL_On_Gm_Tell_ChkB.TabIndex = 42;
            this.Top_Settings_Kill_POL_On_Gm_Tell_ChkB.Text = "Kill POL On GM Tell";
            this.Top_Settings_Kill_POL_On_Gm_Tell_ChkB.UseVisualStyleBackColor = true;
            this.Top_Settings_Kill_POL_On_Gm_Tell_ChkB.CheckedChanged += new System.EventHandler(this.Top_Settings_Kill_POL_On_Gm_Tell_ChkB_CheckedChanged);
            // 
            // Top_Settings_Stop_Fisher_On_GM_Tell_ChkB
            // 
            this.Top_Settings_Stop_Fisher_On_GM_Tell_ChkB.AutoSize = true;
            this.Top_Settings_Stop_Fisher_On_GM_Tell_ChkB.Location = new System.Drawing.Point(6, 52);
            this.Top_Settings_Stop_Fisher_On_GM_Tell_ChkB.Name = "Top_Settings_Stop_Fisher_On_GM_Tell_ChkB";
            this.Top_Settings_Stop_Fisher_On_GM_Tell_ChkB.Size = new System.Drawing.Size(136, 17);
            this.Top_Settings_Stop_Fisher_On_GM_Tell_ChkB.TabIndex = 44;
            this.Top_Settings_Stop_Fisher_On_GM_Tell_ChkB.Text = "Stop Fisher On GM Tell";
            this.Top_Settings_Stop_Fisher_On_GM_Tell_ChkB.UseVisualStyleBackColor = true;
            this.Top_Settings_Stop_Fisher_On_GM_Tell_ChkB.CheckedChanged += new System.EventHandler(this.Top_Settings_Stop_Fisher_On_GM_Tell_ChkB_CheckedChanged);
            // 
            // TopTabControl
            // 
            this.TopTabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.TopTabControl.Controls.Add(this.GMTabPage);
            this.TopTabControl.Controls.Add(this.MenuNavTabPage);
            this.TopTabControl.Controls.Add(this.KeysTabPage);
            this.TopTabControl.Controls.Add(this.DebugTabPage);
            this.TopTabControl.Location = new System.Drawing.Point(1, 0);
            this.TopTabControl.Name = "TopTabControl";
            this.TopTabControl.SelectedIndex = 0;
            this.TopTabControl.Size = new System.Drawing.Size(448, 295);
            this.TopTabControl.TabIndex = 45;
            // 
            // GMTabPage
            // 
            this.GMTabPage.Controls.Add(this.Top_Settings_Kill_POL_On_Gm_Tell_ChkB);
            this.GMTabPage.Controls.Add(this.Top_Settings_Stop_Fisher_On_GM_Tell_ChkB);
            this.GMTabPage.Controls.Add(this.Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB);
            this.GMTabPage.Location = new System.Drawing.Point(4, 25);
            this.GMTabPage.Name = "GMTabPage";
            this.GMTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.GMTabPage.Size = new System.Drawing.Size(440, 266);
            this.GMTabPage.TabIndex = 0;
            this.GMTabPage.Text = "GM Reaction";
            this.GMTabPage.UseVisualStyleBackColor = true;
            // 
            // MenuNavTabPage
            // 
            this.MenuNavTabPage.Controls.Add(this.KeyHoldTimeLabel);
            this.MenuNavTabPage.Controls.Add(this.MoveItemDelayLabel);
            this.MenuNavTabPage.Controls.Add(this.MoveUpDownLabel);
            this.MenuNavTabPage.Controls.Add(this.KeyHoldTimeUpDn);
            this.MenuNavTabPage.Controls.Add(this.MoveItemDelayUpDn);
            this.MenuNavTabPage.Controls.Add(this.MoveUpDownUpDn);
            this.MenuNavTabPage.Location = new System.Drawing.Point(4, 25);
            this.MenuNavTabPage.Name = "MenuNavTabPage";
            this.MenuNavTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.MenuNavTabPage.Size = new System.Drawing.Size(440, 266);
            this.MenuNavTabPage.TabIndex = 1;
            this.MenuNavTabPage.Text = "Menu Nav";
            this.MenuNavTabPage.UseVisualStyleBackColor = true;
            // 
            // KeyHoldTimeLabel
            // 
            this.KeyHoldTimeLabel.AutoSize = true;
            this.KeyHoldTimeLabel.Location = new System.Drawing.Point(83, 61);
            this.KeyHoldTimeLabel.Name = "KeyHoldTimeLabel";
            this.KeyHoldTimeLabel.Size = new System.Drawing.Size(283, 13);
            this.KeyHoldTimeLabel.TabIndex = 5;
            this.KeyHoldTimeLabel.Text = "Key hold time when pressing control keys (Enter, Esc, etc).";
            // 
            // MoveItemDelayLabel
            // 
            this.MoveItemDelayLabel.AutoSize = true;
            this.MoveItemDelayLabel.Location = new System.Drawing.Point(83, 35);
            this.MoveItemDelayLabel.Name = "MoveItemDelayLabel";
            this.MoveItemDelayLabel.Size = new System.Drawing.Size(256, 13);
            this.MoveItemDelayLabel.TabIndex = 4;
            this.MoveItemDelayLabel.Text = "Delay when moving an item to/from your gobbie bag.";
            // 
            // MoveUpDownLabel
            // 
            this.MoveUpDownLabel.AutoSize = true;
            this.MoveUpDownLabel.Location = new System.Drawing.Point(83, 9);
            this.MoveUpDownLabel.Name = "MoveUpDownLabel";
            this.MoveUpDownLabel.Size = new System.Drawing.Size(244, 13);
            this.MoveUpDownLabel.TabIndex = 3;
            this.MoveUpDownLabel.Text = "Delay when moving up or down through inventory.";
            // 
            // KeyHoldTimeUpDn
            // 
            this.KeyHoldTimeUpDn.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.KeyHoldTimeUpDn.Location = new System.Drawing.Point(8, 59);
            this.KeyHoldTimeUpDn.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.KeyHoldTimeUpDn.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.KeyHoldTimeUpDn.Name = "KeyHoldTimeUpDn";
            this.KeyHoldTimeUpDn.Size = new System.Drawing.Size(69, 20);
            this.KeyHoldTimeUpDn.TabIndex = 2;
            this.KeyHoldTimeUpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.KeyHoldTimeUpDn.ThousandsSeparator = true;
            this.KeyHoldTimeUpDn.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.KeyHoldTimeUpDn.ValueChanged += new System.EventHandler(this.KeyHoldTimeUpDn_ValueChanged);
            // 
            // MoveItemDelayUpDn
            // 
            this.MoveItemDelayUpDn.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.MoveItemDelayUpDn.Location = new System.Drawing.Point(8, 33);
            this.MoveItemDelayUpDn.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.MoveItemDelayUpDn.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.MoveItemDelayUpDn.Name = "MoveItemDelayUpDn";
            this.MoveItemDelayUpDn.Size = new System.Drawing.Size(69, 20);
            this.MoveItemDelayUpDn.TabIndex = 1;
            this.MoveItemDelayUpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MoveItemDelayUpDn.ThousandsSeparator = true;
            this.MoveItemDelayUpDn.Value = new decimal(new int[] {
            1100,
            0,
            0,
            0});
            this.MoveItemDelayUpDn.ValueChanged += new System.EventHandler(this.MoveItemDelayUpDn_ValueChanged);
            // 
            // MoveUpDownUpDn
            // 
            this.MoveUpDownUpDn.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.MoveUpDownUpDn.Location = new System.Drawing.Point(8, 7);
            this.MoveUpDownUpDn.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.MoveUpDownUpDn.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.MoveUpDownUpDn.Name = "MoveUpDownUpDn";
            this.MoveUpDownUpDn.Size = new System.Drawing.Size(69, 20);
            this.MoveUpDownUpDn.TabIndex = 0;
            this.MoveUpDownUpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MoveUpDownUpDn.ThousandsSeparator = true;
            this.MoveUpDownUpDn.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.MoveUpDownUpDn.ValueChanged += new System.EventHandler(this.MoveUpDownUpDn_ValueChanged);
            // 
            // DebugTabPage
            // 
            this.DebugTabPage.Controls.Add(this.DebugRetainSettingsChkB);
            this.DebugTabPage.Controls.Add(this.DebugUnfilteredChkB);
            this.DebugTabPage.Controls.Add(this.DebugAllChkB);
            this.DebugTabPage.Controls.Add(this.DebugIocaineStructuresGroup);
            this.DebugTabPage.Controls.Add(this.DebugGameStructuresGroup);
            this.DebugTabPage.Controls.Add(this.DebugMemoryGroup);
            this.DebugTabPage.Controls.Add(this.DebugBotsGroup);
            this.DebugTabPage.Location = new System.Drawing.Point(4, 25);
            this.DebugTabPage.Name = "DebugTabPage";
            this.DebugTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.DebugTabPage.Size = new System.Drawing.Size(440, 266);
            this.DebugTabPage.TabIndex = 2;
            this.DebugTabPage.Text = "Debug";
            this.DebugTabPage.UseVisualStyleBackColor = true;
            // 
            // DebugRetainSettingsChkB
            // 
            this.DebugRetainSettingsChkB.AutoSize = true;
            this.DebugRetainSettingsChkB.Location = new System.Drawing.Point(165, 4);
            this.DebugRetainSettingsChkB.Name = "DebugRetainSettingsChkB";
            this.DebugRetainSettingsChkB.Size = new System.Drawing.Size(150, 17);
            this.DebugRetainSettingsChkB.TabIndex = 6;
            this.DebugRetainSettingsChkB.Text = "Retain Settings on Restart";
            this.DebugRetainSettingsChkB.UseVisualStyleBackColor = true;
            this.DebugRetainSettingsChkB.CheckedChanged += new System.EventHandler(this.DebugRetainSettingsChkB_CheckedChanged);
            // 
            // DebugUnfilteredChkB
            // 
            this.DebugUnfilteredChkB.AutoSize = true;
            this.DebugUnfilteredChkB.Location = new System.Drawing.Point(76, 4);
            this.DebugUnfilteredChkB.Name = "DebugUnfilteredChkB";
            this.DebugUnfilteredChkB.Size = new System.Drawing.Size(71, 17);
            this.DebugUnfilteredChkB.TabIndex = 5;
            this.DebugUnfilteredChkB.Text = "Unfiltered";
            this.DebugUnfilteredChkB.UseVisualStyleBackColor = true;
            this.DebugUnfilteredChkB.CheckedChanged += new System.EventHandler(this.DebugUnfilteredChkB_CheckedChanged);
            // 
            // DebugAllChkB
            // 
            this.DebugAllChkB.AutoSize = true;
            this.DebugAllChkB.Location = new System.Drawing.Point(11, 4);
            this.DebugAllChkB.Name = "DebugAllChkB";
            this.DebugAllChkB.Size = new System.Drawing.Size(37, 17);
            this.DebugAllChkB.TabIndex = 4;
            this.DebugAllChkB.Text = "All";
            this.DebugAllChkB.UseVisualStyleBackColor = true;
            this.DebugAllChkB.CheckedChanged += new System.EventHandler(this.DebugAllChkB_CheckedChanged);
            // 
            // DebugIocaineStructuresGroup
            // 
            this.DebugIocaineStructuresGroup.Controls.Add(this.DebugSettingsChkB);
            this.DebugIocaineStructuresGroup.Controls.Add(this.DebugInteractionChkB);
            this.DebugIocaineStructuresGroup.Controls.Add(this.DebugServerChkB);
            this.DebugIocaineStructuresGroup.Controls.Add(this.DebugTimeChkB);
            this.DebugIocaineStructuresGroup.Controls.Add(this.DebugCommandsChkB);
            this.DebugIocaineStructuresGroup.Location = new System.Drawing.Point(172, 31);
            this.DebugIocaineStructuresGroup.Name = "DebugIocaineStructuresGroup";
            this.DebugIocaineStructuresGroup.Size = new System.Drawing.Size(127, 106);
            this.DebugIocaineStructuresGroup.TabIndex = 3;
            this.DebugIocaineStructuresGroup.TabStop = false;
            this.DebugIocaineStructuresGroup.Text = "Iocaine Structures";
            // 
            // DebugSettingsChkB
            // 
            this.DebugSettingsChkB.AutoSize = true;
            this.DebugSettingsChkB.Location = new System.Drawing.Point(7, 83);
            this.DebugSettingsChkB.Name = "DebugSettingsChkB";
            this.DebugSettingsChkB.Size = new System.Drawing.Size(64, 17);
            this.DebugSettingsChkB.TabIndex = 7;
            this.DebugSettingsChkB.Text = "Settings";
            this.DebugSettingsChkB.UseVisualStyleBackColor = true;
            this.DebugSettingsChkB.CheckedChanged += new System.EventHandler(this.DebugSettingsChkB_CheckedChanged);
            // 
            // DebugInteractionChkB
            // 
            this.DebugInteractionChkB.AutoSize = true;
            this.DebugInteractionChkB.Location = new System.Drawing.Point(7, 15);
            this.DebugInteractionChkB.Name = "DebugInteractionChkB";
            this.DebugInteractionChkB.Size = new System.Drawing.Size(76, 17);
            this.DebugInteractionChkB.TabIndex = 2;
            this.DebugInteractionChkB.Text = "Interaction";
            this.DebugInteractionChkB.UseVisualStyleBackColor = true;
            this.DebugInteractionChkB.CheckedChanged += new System.EventHandler(this.DebugInteractionChkB_CheckedChanged);
            // 
            // DebugServerChkB
            // 
            this.DebugServerChkB.AutoSize = true;
            this.DebugServerChkB.Location = new System.Drawing.Point(7, 66);
            this.DebugServerChkB.Name = "DebugServerChkB";
            this.DebugServerChkB.Size = new System.Drawing.Size(57, 17);
            this.DebugServerChkB.TabIndex = 6;
            this.DebugServerChkB.Text = "Server";
            this.DebugServerChkB.UseVisualStyleBackColor = true;
            this.DebugServerChkB.CheckedChanged += new System.EventHandler(this.DebugServerChkB_CheckedChanged);
            // 
            // DebugTimeChkB
            // 
            this.DebugTimeChkB.AutoSize = true;
            this.DebugTimeChkB.Location = new System.Drawing.Point(7, 32);
            this.DebugTimeChkB.Name = "DebugTimeChkB";
            this.DebugTimeChkB.Size = new System.Drawing.Size(49, 17);
            this.DebugTimeChkB.TabIndex = 3;
            this.DebugTimeChkB.Text = "Time";
            this.DebugTimeChkB.UseVisualStyleBackColor = true;
            this.DebugTimeChkB.CheckedChanged += new System.EventHandler(this.DebugTimeChkB_CheckedChanged);
            // 
            // DebugCommandsChkB
            // 
            this.DebugCommandsChkB.AutoSize = true;
            this.DebugCommandsChkB.Location = new System.Drawing.Point(7, 49);
            this.DebugCommandsChkB.Name = "DebugCommandsChkB";
            this.DebugCommandsChkB.Size = new System.Drawing.Size(78, 17);
            this.DebugCommandsChkB.TabIndex = 5;
            this.DebugCommandsChkB.Text = "Commands";
            this.DebugCommandsChkB.UseVisualStyleBackColor = true;
            this.DebugCommandsChkB.CheckedChanged += new System.EventHandler(this.DebugCommandsChkB_CheckedChanged);
            // 
            // DebugGameStructuresGroup
            // 
            this.DebugGameStructuresGroup.Controls.Add(this.DebugNpcPcChkB);
            this.DebugGameStructuresGroup.Controls.Add(this.DebugInventoryChkB);
            this.DebugGameStructuresGroup.Controls.Add(this.DebugChatLogChkB);
            this.DebugGameStructuresGroup.Location = new System.Drawing.Point(306, 31);
            this.DebugGameStructuresGroup.Name = "DebugGameStructuresGroup";
            this.DebugGameStructuresGroup.Size = new System.Drawing.Size(128, 72);
            this.DebugGameStructuresGroup.TabIndex = 2;
            this.DebugGameStructuresGroup.TabStop = false;
            this.DebugGameStructuresGroup.Text = "Game Structures";
            // 
            // DebugNpcPcChkB
            // 
            this.DebugNpcPcChkB.AutoSize = true;
            this.DebugNpcPcChkB.Location = new System.Drawing.Point(7, 49);
            this.DebugNpcPcChkB.Name = "DebugNpcPcChkB";
            this.DebugNpcPcChkB.Size = new System.Drawing.Size(67, 17);
            this.DebugNpcPcChkB.TabIndex = 4;
            this.DebugNpcPcChkB.Text = "NPC/PC";
            this.DebugNpcPcChkB.UseVisualStyleBackColor = true;
            this.DebugNpcPcChkB.CheckedChanged += new System.EventHandler(this.DebugNpcPcChkB_CheckedChanged);
            // 
            // DebugInventoryChkB
            // 
            this.DebugInventoryChkB.AutoSize = true;
            this.DebugInventoryChkB.Location = new System.Drawing.Point(7, 32);
            this.DebugInventoryChkB.Name = "DebugInventoryChkB";
            this.DebugInventoryChkB.Size = new System.Drawing.Size(70, 17);
            this.DebugInventoryChkB.TabIndex = 1;
            this.DebugInventoryChkB.Text = "Inventory";
            this.DebugInventoryChkB.UseVisualStyleBackColor = true;
            this.DebugInventoryChkB.CheckedChanged += new System.EventHandler(this.DebugInventoryChkB_CheckedChanged);
            // 
            // DebugChatLogChkB
            // 
            this.DebugChatLogChkB.AutoSize = true;
            this.DebugChatLogChkB.Location = new System.Drawing.Point(7, 15);
            this.DebugChatLogChkB.Name = "DebugChatLogChkB";
            this.DebugChatLogChkB.Size = new System.Drawing.Size(69, 17);
            this.DebugChatLogChkB.TabIndex = 0;
            this.DebugChatLogChkB.Text = "Chat Log";
            this.DebugChatLogChkB.UseVisualStyleBackColor = true;
            this.DebugChatLogChkB.CheckedChanged += new System.EventHandler(this.DebugChatLogChkB_CheckedChanged);
            // 
            // DebugMemoryGroup
            // 
            this.DebugMemoryGroup.Controls.Add(this.DebugChangeMonChkB);
            this.DebugMemoryGroup.Controls.Add(this.DebugWinAPIsChkB);
            this.DebugMemoryGroup.Controls.Add(this.DebugMemScannerChkB);
            this.DebugMemoryGroup.Controls.Add(this.DebugMemReadsChkB);
            this.DebugMemoryGroup.Location = new System.Drawing.Point(306, 106);
            this.DebugMemoryGroup.Name = "DebugMemoryGroup";
            this.DebugMemoryGroup.Size = new System.Drawing.Size(128, 89);
            this.DebugMemoryGroup.TabIndex = 1;
            this.DebugMemoryGroup.TabStop = false;
            this.DebugMemoryGroup.Text = "Memory";
            // 
            // DebugChangeMonChkB
            // 
            this.DebugChangeMonChkB.AutoSize = true;
            this.DebugChangeMonChkB.Location = new System.Drawing.Point(7, 66);
            this.DebugChangeMonChkB.Name = "DebugChangeMonChkB";
            this.DebugChangeMonChkB.Size = new System.Drawing.Size(101, 17);
            this.DebugChangeMonChkB.TabIndex = 3;
            this.DebugChangeMonChkB.Text = "Change Monitor";
            this.DebugChangeMonChkB.UseVisualStyleBackColor = true;
            this.DebugChangeMonChkB.CheckedChanged += new System.EventHandler(this.DebugChangeMonChkB_CheckedChanged);
            // 
            // DebugWinAPIsChkB
            // 
            this.DebugWinAPIsChkB.AutoSize = true;
            this.DebugWinAPIsChkB.Location = new System.Drawing.Point(7, 49);
            this.DebugWinAPIsChkB.Name = "DebugWinAPIsChkB";
            this.DebugWinAPIsChkB.Size = new System.Drawing.Size(72, 17);
            this.DebugWinAPIsChkB.TabIndex = 2;
            this.DebugWinAPIsChkB.Text = "Win API\'s";
            this.DebugWinAPIsChkB.UseVisualStyleBackColor = true;
            this.DebugWinAPIsChkB.CheckedChanged += new System.EventHandler(this.DebugWinAPIsChkB_CheckedChanged);
            // 
            // DebugMemScannerChkB
            // 
            this.DebugMemScannerChkB.AutoSize = true;
            this.DebugMemScannerChkB.Location = new System.Drawing.Point(7, 32);
            this.DebugMemScannerChkB.Name = "DebugMemScannerChkB";
            this.DebugMemScannerChkB.Size = new System.Drawing.Size(92, 17);
            this.DebugMemScannerChkB.TabIndex = 1;
            this.DebugMemScannerChkB.Text = "Mem Scanner";
            this.DebugMemScannerChkB.UseVisualStyleBackColor = true;
            this.DebugMemScannerChkB.CheckedChanged += new System.EventHandler(this.DebugMemScannerChkB_CheckedChanged);
            // 
            // DebugMemReadsChkB
            // 
            this.DebugMemReadsChkB.AutoSize = true;
            this.DebugMemReadsChkB.Location = new System.Drawing.Point(7, 15);
            this.DebugMemReadsChkB.Name = "DebugMemReadsChkB";
            this.DebugMemReadsChkB.Size = new System.Drawing.Size(83, 17);
            this.DebugMemReadsChkB.TabIndex = 0;
            this.DebugMemReadsChkB.Text = "Mem Reads";
            this.DebugMemReadsChkB.UseVisualStyleBackColor = true;
            this.DebugMemReadsChkB.CheckedChanged += new System.EventHandler(this.DebugMemReadsChkB_CheckedChanged);
            // 
            // DebugBotsGroup
            // 
            this.DebugBotsGroup.Controls.Add(this.DebugFishStatsChkB);
            this.DebugBotsGroup.Controls.Add(this.DebugSynergyChkB);
            this.DebugBotsGroup.Controls.Add(this.DebugBackgroundChkB);
            this.DebugBotsGroup.Controls.Add(this.DebugTopChkB);
            this.DebugBotsGroup.Controls.Add(this.DebugWMSChkB);
            this.DebugBotsGroup.Controls.Add(this.DebugSellerChkB);
            this.DebugBotsGroup.Controls.Add(this.DebugBuyerChkB);
            this.DebugBotsGroup.Controls.Add(this.DebugTraderChkB);
            this.DebugBotsGroup.Controls.Add(this.DebugNavChkB);
            this.DebugBotsGroup.Controls.Add(this.DebugTAChkB);
            this.DebugBotsGroup.Controls.Add(this.DebugCrafterChkB);
            this.DebugBotsGroup.Controls.Add(this.DebugSUChkB);
            this.DebugBotsGroup.Controls.Add(this.DebugPLChkB);
            this.DebugBotsGroup.Controls.Add(this.DebugFisherChkB);
            this.DebugBotsGroup.Location = new System.Drawing.Point(4, 31);
            this.DebugBotsGroup.Name = "DebugBotsGroup";
            this.DebugBotsGroup.Size = new System.Drawing.Size(162, 155);
            this.DebugBotsGroup.TabIndex = 0;
            this.DebugBotsGroup.TabStop = false;
            this.DebugBotsGroup.Text = "Bots";
            // 
            // DebugFishStatsChkB
            // 
            this.DebugFishStatsChkB.AutoSize = true;
            this.DebugFishStatsChkB.Location = new System.Drawing.Point(7, 132);
            this.DebugFishStatsChkB.Name = "DebugFishStatsChkB";
            this.DebugFishStatsChkB.Size = new System.Drawing.Size(72, 17);
            this.DebugFishStatsChkB.TabIndex = 13;
            this.DebugFishStatsChkB.Text = "Fish Stats";
            this.DebugFishStatsChkB.UseVisualStyleBackColor = true;
            this.DebugFishStatsChkB.CheckedChanged += new System.EventHandler(this.DebugFishStatsChkB_CheckedChanged);
            // 
            // DebugSynergyChkB
            // 
            this.DebugSynergyChkB.AutoSize = true;
            this.DebugSynergyChkB.Location = new System.Drawing.Point(72, 15);
            this.DebugSynergyChkB.Name = "DebugSynergyChkB";
            this.DebugSynergyChkB.Size = new System.Drawing.Size(64, 17);
            this.DebugSynergyChkB.TabIndex = 12;
            this.DebugSynergyChkB.Text = "Synergy";
            this.DebugSynergyChkB.UseVisualStyleBackColor = true;
            this.DebugSynergyChkB.CheckedChanged += new System.EventHandler(this.DebugSynergyChkB_CheckedChanged);
            // 
            // DebugBackgroundChkB
            // 
            this.DebugBackgroundChkB.AutoSize = true;
            this.DebugBackgroundChkB.Location = new System.Drawing.Point(7, 115);
            this.DebugBackgroundChkB.Name = "DebugBackgroundChkB";
            this.DebugBackgroundChkB.Size = new System.Drawing.Size(126, 17);
            this.DebugBackgroundChkB.TabIndex = 11;
            this.DebugBackgroundChkB.Text = "Background Threads";
            this.DebugBackgroundChkB.UseVisualStyleBackColor = true;
            this.DebugBackgroundChkB.CheckedChanged += new System.EventHandler(this.DebugBackgroundChkB_CheckedChanged);
            // 
            // DebugTopChkB
            // 
            this.DebugTopChkB.AutoSize = true;
            this.DebugTopChkB.Location = new System.Drawing.Point(7, 15);
            this.DebugTopChkB.Name = "DebugTopChkB";
            this.DebugTopChkB.Size = new System.Drawing.Size(45, 17);
            this.DebugTopChkB.TabIndex = 10;
            this.DebugTopChkB.Text = "Top";
            this.DebugTopChkB.UseVisualStyleBackColor = true;
            this.DebugTopChkB.CheckedChanged += new System.EventHandler(this.DebugTopChkB_CheckedChanged);
            // 
            // DebugWMSChkB
            // 
            this.DebugWMSChkB.AutoSize = true;
            this.DebugWMSChkB.Location = new System.Drawing.Point(72, 98);
            this.DebugWMSChkB.Name = "DebugWMSChkB";
            this.DebugWMSChkB.Size = new System.Drawing.Size(53, 17);
            this.DebugWMSChkB.TabIndex = 9;
            this.DebugWMSChkB.Text = "WMS";
            this.DebugWMSChkB.UseVisualStyleBackColor = true;
            this.DebugWMSChkB.CheckedChanged += new System.EventHandler(this.DebugWMSChkB_CheckedChanged);
            // 
            // DebugSellerChkB
            // 
            this.DebugSellerChkB.AutoSize = true;
            this.DebugSellerChkB.Location = new System.Drawing.Point(72, 82);
            this.DebugSellerChkB.Name = "DebugSellerChkB";
            this.DebugSellerChkB.Size = new System.Drawing.Size(52, 17);
            this.DebugSellerChkB.TabIndex = 8;
            this.DebugSellerChkB.Text = "Seller";
            this.DebugSellerChkB.UseVisualStyleBackColor = true;
            this.DebugSellerChkB.CheckedChanged += new System.EventHandler(this.DebugSellerChkB_CheckedChanged);
            // 
            // DebugBuyerChkB
            // 
            this.DebugBuyerChkB.AutoSize = true;
            this.DebugBuyerChkB.Location = new System.Drawing.Point(72, 65);
            this.DebugBuyerChkB.Name = "DebugBuyerChkB";
            this.DebugBuyerChkB.Size = new System.Drawing.Size(53, 17);
            this.DebugBuyerChkB.TabIndex = 7;
            this.DebugBuyerChkB.Text = "Buyer";
            this.DebugBuyerChkB.UseVisualStyleBackColor = true;
            this.DebugBuyerChkB.CheckedChanged += new System.EventHandler(this.DebugBuyerChkB_CheckedChanged);
            // 
            // DebugTraderChkB
            // 
            this.DebugTraderChkB.AutoSize = true;
            this.DebugTraderChkB.Location = new System.Drawing.Point(72, 48);
            this.DebugTraderChkB.Name = "DebugTraderChkB";
            this.DebugTraderChkB.Size = new System.Drawing.Size(57, 17);
            this.DebugTraderChkB.TabIndex = 6;
            this.DebugTraderChkB.Text = "Trader";
            this.DebugTraderChkB.UseVisualStyleBackColor = true;
            this.DebugTraderChkB.CheckedChanged += new System.EventHandler(this.DebugTraderChkB_CheckedChanged);
            // 
            // DebugNavChkB
            // 
            this.DebugNavChkB.AutoSize = true;
            this.DebugNavChkB.Location = new System.Drawing.Point(72, 32);
            this.DebugNavChkB.Name = "DebugNavChkB";
            this.DebugNavChkB.Size = new System.Drawing.Size(46, 17);
            this.DebugNavChkB.TabIndex = 5;
            this.DebugNavChkB.Text = "Nav";
            this.DebugNavChkB.UseVisualStyleBackColor = true;
            this.DebugNavChkB.CheckedChanged += new System.EventHandler(this.DebugNavChkB_CheckedChanged);
            // 
            // DebugTAChkB
            // 
            this.DebugTAChkB.AutoSize = true;
            this.DebugTAChkB.Location = new System.Drawing.Point(7, 98);
            this.DebugTAChkB.Name = "DebugTAChkB";
            this.DebugTAChkB.Size = new System.Drawing.Size(40, 17);
            this.DebugTAChkB.TabIndex = 4;
            this.DebugTAChkB.Text = "TA";
            this.DebugTAChkB.UseVisualStyleBackColor = true;
            this.DebugTAChkB.CheckedChanged += new System.EventHandler(this.DebugTAChkB_CheckedChanged);
            // 
            // DebugCrafterChkB
            // 
            this.DebugCrafterChkB.AutoSize = true;
            this.DebugCrafterChkB.Location = new System.Drawing.Point(7, 82);
            this.DebugCrafterChkB.Name = "DebugCrafterChkB";
            this.DebugCrafterChkB.Size = new System.Drawing.Size(57, 17);
            this.DebugCrafterChkB.TabIndex = 3;
            this.DebugCrafterChkB.Text = "Crafter";
            this.DebugCrafterChkB.UseVisualStyleBackColor = true;
            this.DebugCrafterChkB.CheckedChanged += new System.EventHandler(this.DebugCrafterChkB_CheckedChanged);
            // 
            // DebugSUChkB
            // 
            this.DebugSUChkB.AutoSize = true;
            this.DebugSUChkB.Location = new System.Drawing.Point(7, 66);
            this.DebugSUChkB.Name = "DebugSUChkB";
            this.DebugSUChkB.Size = new System.Drawing.Size(59, 17);
            this.DebugSUChkB.TabIndex = 2;
            this.DebugSUChkB.Text = "SkillUp";
            this.DebugSUChkB.UseVisualStyleBackColor = true;
            this.DebugSUChkB.CheckedChanged += new System.EventHandler(this.DebugSUChkB_CheckedChanged);
            // 
            // DebugPLChkB
            // 
            this.DebugPLChkB.AutoSize = true;
            this.DebugPLChkB.Location = new System.Drawing.Point(7, 49);
            this.DebugPLChkB.Name = "DebugPLChkB";
            this.DebugPLChkB.Size = new System.Drawing.Size(39, 17);
            this.DebugPLChkB.TabIndex = 1;
            this.DebugPLChkB.Text = "PL";
            this.DebugPLChkB.UseVisualStyleBackColor = true;
            this.DebugPLChkB.CheckedChanged += new System.EventHandler(this.DebugPLChkB_CheckedChanged);
            // 
            // DebugFisherChkB
            // 
            this.DebugFisherChkB.AutoSize = true;
            this.DebugFisherChkB.Location = new System.Drawing.Point(7, 32);
            this.DebugFisherChkB.Name = "DebugFisherChkB";
            this.DebugFisherChkB.Size = new System.Drawing.Size(54, 17);
            this.DebugFisherChkB.TabIndex = 0;
            this.DebugFisherChkB.Text = "Fisher";
            this.DebugFisherChkB.UseVisualStyleBackColor = true;
            this.DebugFisherChkB.CheckedChanged += new System.EventHandler(this.DebugFisherChkB_CheckedChanged);
            // 
            // KeysTabPage
            // 
            this.KeysTabPage.Controls.Add(this.FishingRightArrowKeyLabel);
            this.KeysTabPage.Controls.Add(this.FishingRightArrowKeyCB);
            this.KeysTabPage.Controls.Add(this.FishingLeftArrowKeyLabel);
            this.KeysTabPage.Controls.Add(this.FishingLeftArrowKeyCB);
            this.KeysTabPage.Location = new System.Drawing.Point(4, 25);
            this.KeysTabPage.Name = "KeysTabPage";
            this.KeysTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.KeysTabPage.Size = new System.Drawing.Size(440, 266);
            this.KeysTabPage.TabIndex = 3;
            this.KeysTabPage.Text = "Keys";
            this.KeysTabPage.UseVisualStyleBackColor = true;
            // 
            // FishingLeftArrowKeyCB
            // 
            this.FishingLeftArrowKeyCB.FormattingEnabled = true;
            this.FishingLeftArrowKeyCB.Location = new System.Drawing.Point(6, 24);
            this.FishingLeftArrowKeyCB.Name = "FishingLeftArrowKeyCB";
            this.FishingLeftArrowKeyCB.Size = new System.Drawing.Size(121, 21);
            this.FishingLeftArrowKeyCB.TabIndex = 0;
            this.FishingLeftArrowKeyCB.SelectedIndexChanged += new System.EventHandler(this.FishingLeftArrowKeyCB_SelectedIndexChanged);
            // 
            // FishingLeftArrowKeyLabel
            // 
            this.FishingLeftArrowKeyLabel.AutoSize = true;
            this.FishingLeftArrowKeyLabel.Location = new System.Drawing.Point(7, 8);
            this.FishingLeftArrowKeyLabel.Name = "FishingLeftArrowKeyLabel";
            this.FishingLeftArrowKeyLabel.Size = new System.Drawing.Size(112, 13);
            this.FishingLeftArrowKeyLabel.TabIndex = 4;
            this.FishingLeftArrowKeyLabel.Text = "Fishing Left Arrow Key";
            // 
            // FishingRightArrowKeyLabel
            // 
            this.FishingRightArrowKeyLabel.AutoSize = true;
            this.FishingRightArrowKeyLabel.Location = new System.Drawing.Point(7, 48);
            this.FishingRightArrowKeyLabel.Name = "FishingRightArrowKeyLabel";
            this.FishingRightArrowKeyLabel.Size = new System.Drawing.Size(119, 13);
            this.FishingRightArrowKeyLabel.TabIndex = 6;
            this.FishingRightArrowKeyLabel.Text = "Fishing Right Arrow Key";
            // 
            // FishingRightArrowKeyCB
            // 
            this.FishingRightArrowKeyCB.FormattingEnabled = true;
            this.FishingRightArrowKeyCB.Location = new System.Drawing.Point(6, 64);
            this.FishingRightArrowKeyCB.Name = "FishingRightArrowKeyCB";
            this.FishingRightArrowKeyCB.Size = new System.Drawing.Size(121, 21);
            this.FishingRightArrowKeyCB.TabIndex = 5;
            this.FishingRightArrowKeyCB.SelectedIndexChanged += new System.EventHandler(this.FishingRightArrowKeyCB_SelectedIndexChanged);
            // 
            // Top_Settings_Form
            // 
            this.AcceptButton = this.OK_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_Button;
            this.ClientSize = new System.Drawing.Size(448, 330);
            this.Controls.Add(this.TopTabControl);
            this.Controls.Add(this.Apply_Button);
            this.Controls.Add(this.OK_Button);
            this.Controls.Add(this.Cancel_Button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "Top_Settings_Form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Iocaine Settings";
            this.TopMost = true;
            this.TopTabControl.ResumeLayout(false);
            this.GMTabPage.ResumeLayout(false);
            this.GMTabPage.PerformLayout();
            this.MenuNavTabPage.ResumeLayout(false);
            this.MenuNavTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.KeyHoldTimeUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveItemDelayUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveUpDownUpDn)).EndInit();
            this.DebugTabPage.ResumeLayout(false);
            this.DebugTabPage.PerformLayout();
            this.DebugIocaineStructuresGroup.ResumeLayout(false);
            this.DebugIocaineStructuresGroup.PerformLayout();
            this.DebugGameStructuresGroup.ResumeLayout(false);
            this.DebugGameStructuresGroup.PerformLayout();
            this.DebugMemoryGroup.ResumeLayout(false);
            this.DebugMemoryGroup.PerformLayout();
            this.DebugBotsGroup.ResumeLayout(false);
            this.DebugBotsGroup.PerformLayout();
            this.KeysTabPage.ResumeLayout(false);
            this.KeysTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button Apply_Button;
        public System.Windows.Forms.Button OK_Button;
        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.CheckBox Top_Settings_Stop_All_Bots_On_Gm_Tell_ChkB;
        private System.Windows.Forms.CheckBox Top_Settings_Kill_POL_On_Gm_Tell_ChkB;
        private System.Windows.Forms.CheckBox Top_Settings_Stop_Fisher_On_GM_Tell_ChkB;
        private System.Windows.Forms.TabControl TopTabControl;
        private System.Windows.Forms.TabPage GMTabPage;
        private System.Windows.Forms.TabPage MenuNavTabPage;
        private System.Windows.Forms.NumericUpDown MoveUpDownUpDn;
        private System.Windows.Forms.NumericUpDown KeyHoldTimeUpDn;
        private System.Windows.Forms.NumericUpDown MoveItemDelayUpDn;
        private System.Windows.Forms.Label KeyHoldTimeLabel;
        private System.Windows.Forms.Label MoveItemDelayLabel;
        private System.Windows.Forms.Label MoveUpDownLabel;
        private System.Windows.Forms.TabPage DebugTabPage;
        private System.Windows.Forms.GroupBox DebugBotsGroup;
        private System.Windows.Forms.CheckBox DebugSUChkB;
        private System.Windows.Forms.CheckBox DebugPLChkB;
        private System.Windows.Forms.CheckBox DebugFisherChkB;
        private System.Windows.Forms.CheckBox DebugCrafterChkB;
        private System.Windows.Forms.CheckBox DebugTraderChkB;
        private System.Windows.Forms.CheckBox DebugNavChkB;
        private System.Windows.Forms.CheckBox DebugTAChkB;
        private System.Windows.Forms.CheckBox DebugWMSChkB;
        private System.Windows.Forms.CheckBox DebugSellerChkB;
        private System.Windows.Forms.CheckBox DebugBuyerChkB;
        private System.Windows.Forms.GroupBox DebugMemoryGroup;
        private System.Windows.Forms.CheckBox DebugBackgroundChkB;
        private System.Windows.Forms.CheckBox DebugTopChkB;
        private System.Windows.Forms.CheckBox DebugMemReadsChkB;
        private System.Windows.Forms.CheckBox DebugMemScannerChkB;
        private System.Windows.Forms.CheckBox DebugWinAPIsChkB;
        private System.Windows.Forms.GroupBox DebugGameStructuresGroup;
        private System.Windows.Forms.CheckBox DebugChatLogChkB;
        private System.Windows.Forms.CheckBox DebugChangeMonChkB;
        private System.Windows.Forms.CheckBox DebugInteractionChkB;
        private System.Windows.Forms.CheckBox DebugInventoryChkB;
        private System.Windows.Forms.CheckBox DebugSynergyChkB;
        private System.Windows.Forms.CheckBox DebugTimeChkB;
        private System.Windows.Forms.CheckBox DebugCommandsChkB;
        private System.Windows.Forms.CheckBox DebugNpcPcChkB;
        private System.Windows.Forms.CheckBox DebugServerChkB;
        private System.Windows.Forms.CheckBox DebugSettingsChkB;
        private System.Windows.Forms.GroupBox DebugIocaineStructuresGroup;
        private System.Windows.Forms.CheckBox DebugFishStatsChkB;
        private System.Windows.Forms.CheckBox DebugUnfilteredChkB;
        private System.Windows.Forms.CheckBox DebugAllChkB;
        private System.Windows.Forms.CheckBox DebugRetainSettingsChkB;
        private System.Windows.Forms.TabPage KeysTabPage;
        private System.Windows.Forms.Label FishingRightArrowKeyLabel;
        private System.Windows.Forms.ComboBox FishingRightArrowKeyCB;
        private System.Windows.Forms.Label FishingLeftArrowKeyLabel;
        private System.Windows.Forms.ComboBox FishingLeftArrowKeyCB;
    }
}