namespace Iocaine2.Settings
{
    partial class Helpers_Settings_Form
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
            this.TopTabControl = new System.Windows.Forms.TabControl();
            this.DelayControlTabPage = new System.Windows.Forms.TabPage();
            this.LoadDefaultDelaysButton = new System.Windows.Forms.Button();
            this.DelayDecreaseButton = new System.Windows.Forms.Button();
            this.DelayIncreaseButton = new System.Windows.Forms.Button();
            this.PressEnterToSellDelayLabel = new System.Windows.Forms.Label();
            this.PressEnterToSellDelayUpDn = new System.Windows.Forms.NumericUpDown();
            this.ChangeSellQuanDelayLabel = new System.Windows.Forms.Label();
            this.ChangeSellQuanDelayUpDn = new System.Windows.Forms.NumericUpDown();
            this.CheckHelpTextDelayLabel = new System.Windows.Forms.Label();
            this.CheckHelpTextDelayUpDn = new System.Windows.Forms.NumericUpDown();
            this.EnterToSellDelayLabel = new System.Windows.Forms.Label();
            this.EnterToSellDelayUpDn = new System.Windows.Forms.NumericUpDown();
            this.EnterNpcMenuDelayLabel = new System.Windows.Forms.Label();
            this.EnterNpcMenuDelayUpDn = new System.Windows.Forms.NumericUpDown();
            this.SellerSettingsTab = new System.Windows.Forms.TabPage();
            this.SL_SortBeforeSellingChkB = new System.Windows.Forms.CheckBox();
            this.SL_SellFromBagFirstChkB = new System.Windows.Forms.CheckBox();
            this.SL_WhenDoneBrowseFileToPlayButton = new System.Windows.Forms.Button();
            this.SL_WhenDonePlaySoundLabel = new System.Windows.Forms.Label();
            this.SL_WhenDonePlaySoundTB = new System.Windows.Forms.TextBox();
            this.BuyerSettingsTab = new System.Windows.Forms.TabPage();
            this.BY_GuildSetIndexChkB = new System.Windows.Forms.CheckBox();
            this.BY_WhenDoneBrowseFileToPlayButton = new System.Windows.Forms.Button();
            this.BY_WhenDonePlaySoundLabel = new System.Windows.Forms.Label();
            this.BY_WhenDonePlaySoundTB = new System.Windows.Forms.TextBox();
            this.TopTabControl.SuspendLayout();
            this.DelayControlTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PressEnterToSellDelayUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChangeSellQuanDelayUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckHelpTextDelayUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EnterToSellDelayUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EnterNpcMenuDelayUpDn)).BeginInit();
            this.SellerSettingsTab.SuspendLayout();
            this.BuyerSettingsTab.SuspendLayout();
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
            // TopTabControl
            // 
            this.TopTabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.TopTabControl.Controls.Add(this.DelayControlTabPage);
            this.TopTabControl.Controls.Add(this.SellerSettingsTab);
            this.TopTabControl.Controls.Add(this.BuyerSettingsTab);
            this.TopTabControl.Location = new System.Drawing.Point(1, 0);
            this.TopTabControl.Name = "TopTabControl";
            this.TopTabControl.SelectedIndex = 0;
            this.TopTabControl.Size = new System.Drawing.Size(448, 295);
            this.TopTabControl.TabIndex = 45;
            // 
            // DelayControlTabPage
            // 
            this.DelayControlTabPage.Controls.Add(this.LoadDefaultDelaysButton);
            this.DelayControlTabPage.Controls.Add(this.DelayDecreaseButton);
            this.DelayControlTabPage.Controls.Add(this.DelayIncreaseButton);
            this.DelayControlTabPage.Controls.Add(this.PressEnterToSellDelayLabel);
            this.DelayControlTabPage.Controls.Add(this.PressEnterToSellDelayUpDn);
            this.DelayControlTabPage.Controls.Add(this.ChangeSellQuanDelayLabel);
            this.DelayControlTabPage.Controls.Add(this.ChangeSellQuanDelayUpDn);
            this.DelayControlTabPage.Controls.Add(this.CheckHelpTextDelayLabel);
            this.DelayControlTabPage.Controls.Add(this.CheckHelpTextDelayUpDn);
            this.DelayControlTabPage.Controls.Add(this.EnterToSellDelayLabel);
            this.DelayControlTabPage.Controls.Add(this.EnterToSellDelayUpDn);
            this.DelayControlTabPage.Controls.Add(this.EnterNpcMenuDelayLabel);
            this.DelayControlTabPage.Controls.Add(this.EnterNpcMenuDelayUpDn);
            this.DelayControlTabPage.Location = new System.Drawing.Point(4, 25);
            this.DelayControlTabPage.Name = "DelayControlTabPage";
            this.DelayControlTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.DelayControlTabPage.Size = new System.Drawing.Size(440, 266);
            this.DelayControlTabPage.TabIndex = 0;
            this.DelayControlTabPage.Text = "Delay Control";
            this.DelayControlTabPage.UseVisualStyleBackColor = true;
            // 
            // LoadDefaultDelaysButton
            // 
            this.LoadDefaultDelaysButton.Location = new System.Drawing.Point(153, 136);
            this.LoadDefaultDelaysButton.Name = "LoadDefaultDelaysButton";
            this.LoadDefaultDelaysButton.Size = new System.Drawing.Size(69, 23);
            this.LoadDefaultDelaysButton.TabIndex = 16;
            this.LoadDefaultDelaysButton.Text = "Default";
            this.LoadDefaultDelaysButton.UseVisualStyleBackColor = true;
            this.LoadDefaultDelaysButton.Click += new System.EventHandler(this.LoadDefaultDelaysButton_Click);
            // 
            // DelayDecreaseButton
            // 
            this.DelayDecreaseButton.Location = new System.Drawing.Point(78, 136);
            this.DelayDecreaseButton.Name = "DelayDecreaseButton";
            this.DelayDecreaseButton.Size = new System.Drawing.Size(69, 23);
            this.DelayDecreaseButton.TabIndex = 15;
            this.DelayDecreaseButton.Text = "Decrease";
            this.DelayDecreaseButton.UseVisualStyleBackColor = true;
            this.DelayDecreaseButton.Click += new System.EventHandler(this.DelayDecreaseButton_Click);
            // 
            // DelayIncreaseButton
            // 
            this.DelayIncreaseButton.Location = new System.Drawing.Point(0, 136);
            this.DelayIncreaseButton.Name = "DelayIncreaseButton";
            this.DelayIncreaseButton.Size = new System.Drawing.Size(69, 23);
            this.DelayIncreaseButton.TabIndex = 14;
            this.DelayIncreaseButton.Text = "Increase";
            this.DelayIncreaseButton.UseVisualStyleBackColor = true;
            this.DelayIncreaseButton.Click += new System.EventHandler(this.DelayIncreaseButton_Click);
            // 
            // PressEnterToSellDelayLabel
            // 
            this.PressEnterToSellDelayLabel.AutoSize = true;
            this.PressEnterToSellDelayLabel.Location = new System.Drawing.Point(75, 112);
            this.PressEnterToSellDelayLabel.Name = "PressEnterToSellDelayLabel";
            this.PressEnterToSellDelayLabel.Size = new System.Drawing.Size(333, 13);
            this.PressEnterToSellDelayLabel.TabIndex = 13;
            this.PressEnterToSellDelayLabel.Text = "Delay between pressing up to select \'Sell\' and pressing enter to sell it.";
            // 
            // PressEnterToSellDelayUpDn
            // 
            this.PressEnterToSellDelayUpDn.Increment = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.PressEnterToSellDelayUpDn.Location = new System.Drawing.Point(0, 110);
            this.PressEnterToSellDelayUpDn.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.PressEnterToSellDelayUpDn.Minimum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.PressEnterToSellDelayUpDn.Name = "PressEnterToSellDelayUpDn";
            this.PressEnterToSellDelayUpDn.Size = new System.Drawing.Size(69, 20);
            this.PressEnterToSellDelayUpDn.TabIndex = 12;
            this.PressEnterToSellDelayUpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.PressEnterToSellDelayUpDn.ThousandsSeparator = true;
            this.PressEnterToSellDelayUpDn.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.PressEnterToSellDelayUpDn.ValueChanged += new System.EventHandler(this.PressEnterToSellDelayUpDn_ValueChanged);
            // 
            // ChangeSellQuanDelayLabel
            // 
            this.ChangeSellQuanDelayLabel.AutoSize = true;
            this.ChangeSellQuanDelayLabel.Location = new System.Drawing.Point(75, 86);
            this.ChangeSellQuanDelayLabel.Name = "ChangeSellQuanDelayLabel";
            this.ChangeSellQuanDelayLabel.Size = new System.Drawing.Size(274, 13);
            this.ChangeSellQuanDelayLabel.TabIndex = 11;
            this.ChangeSellQuanDelayLabel.Text = "Delay between key presses when changing sell quantity.";
            // 
            // ChangeSellQuanDelayUpDn
            // 
            this.ChangeSellQuanDelayUpDn.Increment = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.ChangeSellQuanDelayUpDn.Location = new System.Drawing.Point(0, 84);
            this.ChangeSellQuanDelayUpDn.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.ChangeSellQuanDelayUpDn.Minimum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.ChangeSellQuanDelayUpDn.Name = "ChangeSellQuanDelayUpDn";
            this.ChangeSellQuanDelayUpDn.Size = new System.Drawing.Size(69, 20);
            this.ChangeSellQuanDelayUpDn.TabIndex = 10;
            this.ChangeSellQuanDelayUpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ChangeSellQuanDelayUpDn.ThousandsSeparator = true;
            this.ChangeSellQuanDelayUpDn.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.ChangeSellQuanDelayUpDn.ValueChanged += new System.EventHandler(this.ChangeSellQuanDelayUpDn_ValueChanged);
            // 
            // CheckHelpTextDelayLabel
            // 
            this.CheckHelpTextDelayLabel.AutoSize = true;
            this.CheckHelpTextDelayLabel.Location = new System.Drawing.Point(75, 60);
            this.CheckHelpTextDelayLabel.Name = "CheckHelpTextDelayLabel";
            this.CheckHelpTextDelayLabel.Size = new System.Drawing.Size(346, 13);
            this.CheckHelpTextDelayLabel.TabIndex = 9;
            this.CheckHelpTextDelayLabel.Text = "Delay between pressing enter to sell an item and checking the help text.";
            // 
            // CheckHelpTextDelayUpDn
            // 
            this.CheckHelpTextDelayUpDn.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.CheckHelpTextDelayUpDn.Location = new System.Drawing.Point(0, 58);
            this.CheckHelpTextDelayUpDn.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.CheckHelpTextDelayUpDn.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.CheckHelpTextDelayUpDn.Name = "CheckHelpTextDelayUpDn";
            this.CheckHelpTextDelayUpDn.Size = new System.Drawing.Size(69, 20);
            this.CheckHelpTextDelayUpDn.TabIndex = 8;
            this.CheckHelpTextDelayUpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.CheckHelpTextDelayUpDn.ThousandsSeparator = true;
            this.CheckHelpTextDelayUpDn.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.CheckHelpTextDelayUpDn.ValueChanged += new System.EventHandler(this.CheckHelpTextDelayUpDn_ValueChanged);
            // 
            // EnterToSellDelayLabel
            // 
            this.EnterToSellDelayLabel.AutoSize = true;
            this.EnterToSellDelayLabel.Location = new System.Drawing.Point(75, 34);
            this.EnterToSellDelayLabel.Name = "EnterToSellDelayLabel";
            this.EnterToSellDelayLabel.Size = new System.Drawing.Size(320, 13);
            this.EnterToSellDelayLabel.TabIndex = 7;
            this.EnterToSellDelayLabel.Text = "Delay between reading an item\'s name and pressing enter to sell it.";
            // 
            // EnterToSellDelayUpDn
            // 
            this.EnterToSellDelayUpDn.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.EnterToSellDelayUpDn.Location = new System.Drawing.Point(0, 32);
            this.EnterToSellDelayUpDn.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.EnterToSellDelayUpDn.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.EnterToSellDelayUpDn.Name = "EnterToSellDelayUpDn";
            this.EnterToSellDelayUpDn.Size = new System.Drawing.Size(69, 20);
            this.EnterToSellDelayUpDn.TabIndex = 6;
            this.EnterToSellDelayUpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.EnterToSellDelayUpDn.ThousandsSeparator = true;
            this.EnterToSellDelayUpDn.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.EnterToSellDelayUpDn.ValueChanged += new System.EventHandler(this.EnterToSellDelayUpDn_ValueChanged);
            // 
            // EnterNpcMenuDelayLabel
            // 
            this.EnterNpcMenuDelayLabel.AutoSize = true;
            this.EnterNpcMenuDelayLabel.Location = new System.Drawing.Point(75, 8);
            this.EnterNpcMenuDelayLabel.Name = "EnterNpcMenuDelayLabel";
            this.EnterNpcMenuDelayLabel.Size = new System.Drawing.Size(311, 13);
            this.EnterNpcMenuDelayLabel.TabIndex = 5;
            this.EnterNpcMenuDelayLabel.Text = "Delay between pressing enter on an NPC and selecting buy/sell.";
            // 
            // EnterNpcMenuDelayUpDn
            // 
            this.EnterNpcMenuDelayUpDn.Increment = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.EnterNpcMenuDelayUpDn.Location = new System.Drawing.Point(0, 6);
            this.EnterNpcMenuDelayUpDn.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.EnterNpcMenuDelayUpDn.Minimum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.EnterNpcMenuDelayUpDn.Name = "EnterNpcMenuDelayUpDn";
            this.EnterNpcMenuDelayUpDn.Size = new System.Drawing.Size(69, 20);
            this.EnterNpcMenuDelayUpDn.TabIndex = 4;
            this.EnterNpcMenuDelayUpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.EnterNpcMenuDelayUpDn.ThousandsSeparator = true;
            this.EnterNpcMenuDelayUpDn.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.EnterNpcMenuDelayUpDn.ValueChanged += new System.EventHandler(this.EnterNpcMenuDelayUpDn_ValueChanged);
            // 
            // SellerSettingsTab
            // 
            this.SellerSettingsTab.Controls.Add(this.SL_SortBeforeSellingChkB);
            this.SellerSettingsTab.Controls.Add(this.SL_SellFromBagFirstChkB);
            this.SellerSettingsTab.Controls.Add(this.SL_WhenDoneBrowseFileToPlayButton);
            this.SellerSettingsTab.Controls.Add(this.SL_WhenDonePlaySoundLabel);
            this.SellerSettingsTab.Controls.Add(this.SL_WhenDonePlaySoundTB);
            this.SellerSettingsTab.Location = new System.Drawing.Point(4, 25);
            this.SellerSettingsTab.Name = "SellerSettingsTab";
            this.SellerSettingsTab.Size = new System.Drawing.Size(440, 266);
            this.SellerSettingsTab.TabIndex = 1;
            this.SellerSettingsTab.Text = "Seller Settings";
            this.SellerSettingsTab.UseVisualStyleBackColor = true;
            // 
            // SL_SortBeforeSellingChkB
            // 
            this.SL_SortBeforeSellingChkB.AutoSize = true;
            this.SL_SortBeforeSellingChkB.Location = new System.Drawing.Point(153, 168);
            this.SL_SortBeforeSellingChkB.Name = "SL_SortBeforeSellingChkB";
            this.SL_SortBeforeSellingChkB.Size = new System.Drawing.Size(135, 17);
            this.SL_SortBeforeSellingChkB.TabIndex = 43;
            this.SL_SortBeforeSellingChkB.Text = "Sort Bag Before Selling";
            this.SL_SortBeforeSellingChkB.UseVisualStyleBackColor = true;
            this.SL_SortBeforeSellingChkB.CheckedChanged += new System.EventHandler(this.SortBeforeSellingChkB_CheckedChanged);
            // 
            // SL_SellFromBagFirstChkB
            // 
            this.SL_SellFromBagFirstChkB.AutoSize = true;
            this.SL_SellFromBagFirstChkB.Location = new System.Drawing.Point(154, 145);
            this.SL_SellFromBagFirstChkB.Name = "SL_SellFromBagFirstChkB";
            this.SL_SellFromBagFirstChkB.Size = new System.Drawing.Size(113, 17);
            this.SL_SellFromBagFirstChkB.TabIndex = 42;
            this.SL_SellFromBagFirstChkB.Text = "Sell From Bag First";
            this.SL_SellFromBagFirstChkB.UseVisualStyleBackColor = true;
            this.SL_SellFromBagFirstChkB.CheckedChanged += new System.EventHandler(this.SellFromBagFirstChkB_CheckedChanged);
            // 
            // SL_WhenDoneBrowseFileToPlayButton
            // 
            this.SL_WhenDoneBrowseFileToPlayButton.Location = new System.Drawing.Point(297, 96);
            this.SL_WhenDoneBrowseFileToPlayButton.Name = "SL_WhenDoneBrowseFileToPlayButton";
            this.SL_WhenDoneBrowseFileToPlayButton.Size = new System.Drawing.Size(22, 21);
            this.SL_WhenDoneBrowseFileToPlayButton.TabIndex = 41;
            this.SL_WhenDoneBrowseFileToPlayButton.Text = "...";
            this.SL_WhenDoneBrowseFileToPlayButton.UseVisualStyleBackColor = true;
            this.SL_WhenDoneBrowseFileToPlayButton.Click += new System.EventHandler(this.SL_WhenDoneBrowseFileToPlayButton_Click);
            // 
            // SL_WhenDonePlaySoundLabel
            // 
            this.SL_WhenDonePlaySoundLabel.AutoSize = true;
            this.SL_WhenDonePlaySoundLabel.Location = new System.Drawing.Point(127, 80);
            this.SL_WhenDonePlaySoundLabel.Name = "SL_WhenDonePlaySoundLabel";
            this.SL_WhenDonePlaySoundLabel.Size = new System.Drawing.Size(128, 13);
            this.SL_WhenDonePlaySoundLabel.TabIndex = 40;
            this.SL_WhenDonePlaySoundLabel.Text = "Say Message or Play File:";
            // 
            // SL_WhenDonePlaySoundTB
            // 
            this.SL_WhenDonePlaySoundTB.Location = new System.Drawing.Point(121, 97);
            this.SL_WhenDonePlaySoundTB.Name = "SL_WhenDonePlaySoundTB";
            this.SL_WhenDonePlaySoundTB.Size = new System.Drawing.Size(171, 20);
            this.SL_WhenDonePlaySoundTB.TabIndex = 39;
            this.SL_WhenDonePlaySoundTB.Text = "Text to Say or File to Play";
            this.SL_WhenDonePlaySoundTB.TextChanged += new System.EventHandler(this.SL_WhenDonePlaySoundTB_TextChanged);
            this.SL_WhenDonePlaySoundTB.Enter += new System.EventHandler(this.SL_WhenDonePlaySoundTB_Enter);
            this.SL_WhenDonePlaySoundTB.Leave += new System.EventHandler(this.SL_WhenDonePlaySoundTB_Leave);
            // 
            // BuyerSettingsTab
            // 
            this.BuyerSettingsTab.Controls.Add(this.BY_GuildSetIndexChkB);
            this.BuyerSettingsTab.Controls.Add(this.BY_WhenDoneBrowseFileToPlayButton);
            this.BuyerSettingsTab.Controls.Add(this.BY_WhenDonePlaySoundLabel);
            this.BuyerSettingsTab.Controls.Add(this.BY_WhenDonePlaySoundTB);
            this.BuyerSettingsTab.Location = new System.Drawing.Point(4, 25);
            this.BuyerSettingsTab.Name = "BuyerSettingsTab";
            this.BuyerSettingsTab.Size = new System.Drawing.Size(440, 266);
            this.BuyerSettingsTab.TabIndex = 2;
            this.BuyerSettingsTab.Text = "Buyer Settings";
            this.BuyerSettingsTab.UseVisualStyleBackColor = true;
            // 
            // BY_GuildSetIndexChkB
            // 
            this.BY_GuildSetIndexChkB.AutoSize = true;
            this.BY_GuildSetIndexChkB.Location = new System.Drawing.Point(154, 145);
            this.BY_GuildSetIndexChkB.Name = "BY_GuildSetIndexChkB";
            this.BY_GuildSetIndexChkB.Size = new System.Drawing.Size(262, 17);
            this.BY_GuildSetIndexChkB.TabIndex = 46;
            this.BY_GuildSetIndexChkB.Text = "When selecting the item to buy, write it to memory.";
            this.BY_GuildSetIndexChkB.UseVisualStyleBackColor = true;
            this.BY_GuildSetIndexChkB.CheckedChanged += new System.EventHandler(this.BY_GuildSetIndexChkB_CheckedChanged);
            // 
            // BY_WhenDoneBrowseFileToPlayButton
            // 
            this.BY_WhenDoneBrowseFileToPlayButton.Location = new System.Drawing.Point(297, 96);
            this.BY_WhenDoneBrowseFileToPlayButton.Name = "BY_WhenDoneBrowseFileToPlayButton";
            this.BY_WhenDoneBrowseFileToPlayButton.Size = new System.Drawing.Size(22, 21);
            this.BY_WhenDoneBrowseFileToPlayButton.TabIndex = 45;
            this.BY_WhenDoneBrowseFileToPlayButton.Text = "...";
            this.BY_WhenDoneBrowseFileToPlayButton.UseVisualStyleBackColor = true;
            this.BY_WhenDoneBrowseFileToPlayButton.Click += new System.EventHandler(this.BY_WhenDoneBrowseFileToPlayButton_Click);
            // 
            // BY_WhenDonePlaySoundLabel
            // 
            this.BY_WhenDonePlaySoundLabel.AutoSize = true;
            this.BY_WhenDonePlaySoundLabel.Location = new System.Drawing.Point(127, 80);
            this.BY_WhenDonePlaySoundLabel.Name = "BY_WhenDonePlaySoundLabel";
            this.BY_WhenDonePlaySoundLabel.Size = new System.Drawing.Size(128, 13);
            this.BY_WhenDonePlaySoundLabel.TabIndex = 44;
            this.BY_WhenDonePlaySoundLabel.Text = "Say Message or Play File:";
            // 
            // BY_WhenDonePlaySoundTB
            // 
            this.BY_WhenDonePlaySoundTB.Location = new System.Drawing.Point(121, 97);
            this.BY_WhenDonePlaySoundTB.Name = "BY_WhenDonePlaySoundTB";
            this.BY_WhenDonePlaySoundTB.Size = new System.Drawing.Size(171, 20);
            this.BY_WhenDonePlaySoundTB.TabIndex = 43;
            this.BY_WhenDonePlaySoundTB.Text = "Text to Say or File to Play";
            this.BY_WhenDonePlaySoundTB.TextChanged += new System.EventHandler(this.BY_WhenDonePlaySoundTB_TextChanged);
            this.BY_WhenDonePlaySoundTB.Enter += new System.EventHandler(this.BY_WhenDonePlaySoundTB_Enter);
            this.BY_WhenDonePlaySoundTB.Leave += new System.EventHandler(this.BY_WhenDonePlaySoundTB_Leave);
            // 
            // Helpers_Settings_Form
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
            this.Name = "Helpers_Settings_Form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Helpers Settings";
            this.TopMost = true;
            this.TopTabControl.ResumeLayout(false);
            this.DelayControlTabPage.ResumeLayout(false);
            this.DelayControlTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PressEnterToSellDelayUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChangeSellQuanDelayUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckHelpTextDelayUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EnterToSellDelayUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EnterNpcMenuDelayUpDn)).EndInit();
            this.SellerSettingsTab.ResumeLayout(false);
            this.SellerSettingsTab.PerformLayout();
            this.BuyerSettingsTab.ResumeLayout(false);
            this.BuyerSettingsTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button Apply_Button;
        public System.Windows.Forms.Button OK_Button;
        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.TabControl TopTabControl;
        private System.Windows.Forms.TabPage DelayControlTabPage;
        private System.Windows.Forms.Label EnterNpcMenuDelayLabel;
        private System.Windows.Forms.NumericUpDown EnterNpcMenuDelayUpDn;
        private System.Windows.Forms.Label CheckHelpTextDelayLabel;
        private System.Windows.Forms.NumericUpDown CheckHelpTextDelayUpDn;
        private System.Windows.Forms.Label EnterToSellDelayLabel;
        private System.Windows.Forms.NumericUpDown EnterToSellDelayUpDn;
        private System.Windows.Forms.Label PressEnterToSellDelayLabel;
        private System.Windows.Forms.NumericUpDown PressEnterToSellDelayUpDn;
        private System.Windows.Forms.Label ChangeSellQuanDelayLabel;
        private System.Windows.Forms.NumericUpDown ChangeSellQuanDelayUpDn;
        private System.Windows.Forms.Button DelayDecreaseButton;
        private System.Windows.Forms.Button DelayIncreaseButton;
        private System.Windows.Forms.Button LoadDefaultDelaysButton;
        private System.Windows.Forms.TabPage SellerSettingsTab;
        private System.Windows.Forms.Button SL_WhenDoneBrowseFileToPlayButton;
        private System.Windows.Forms.Label SL_WhenDonePlaySoundLabel;
        private System.Windows.Forms.TextBox SL_WhenDonePlaySoundTB;
        private System.Windows.Forms.CheckBox SL_SellFromBagFirstChkB;
        private System.Windows.Forms.CheckBox SL_SortBeforeSellingChkB;
        private System.Windows.Forms.TabPage BuyerSettingsTab;
        private System.Windows.Forms.CheckBox BY_GuildSetIndexChkB;
        private System.Windows.Forms.Button BY_WhenDoneBrowseFileToPlayButton;
        private System.Windows.Forms.Label BY_WhenDonePlaySoundLabel;
        private System.Windows.Forms.TextBox BY_WhenDonePlaySoundTB;
    }
}