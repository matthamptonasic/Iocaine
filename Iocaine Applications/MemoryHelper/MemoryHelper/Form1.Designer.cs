namespace IocaineOffsetHelper
{
    partial class IocaineOffsetsHelperForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IocaineOffsetsHelperForm));
            this.OffsetCB = new System.Windows.Forms.ComboBox();
            this.MainTextBox = new System.Windows.Forms.RichTextBox();
            this.processCB = new System.Windows.Forms.ComboBox();
            this.pcNameTB = new System.Windows.Forms.TextBox();
            this.Get_Position_Button = new System.Windows.Forms.Button();
            this.PosX_UpDn = new System.Windows.Forms.NumericUpDown();
            this.PosX_Label = new System.Windows.Forms.Label();
            this.PosY_Label = new System.Windows.Forms.Label();
            this.PosZ_Label = new System.Windows.Forms.Label();
            this.PosH_Label = new System.Windows.Forms.Label();
            this.Set_Position_Button = new System.Windows.Forms.Button();
            this.PosH_UpDn = new System.Windows.Forms.NumericUpDown();
            this.PosZ_UpDn = new System.Windows.Forms.NumericUpDown();
            this.PosY_UpDn = new System.Windows.Forms.NumericUpDown();
            this.Player_Pointer_TB = new System.Windows.Forms.TextBox();
            this.Player_Position_Ptr_TB = new System.Windows.Forms.TextBox();
            this.Reset_Pointers_Button = new System.Windows.Forms.Button();
            this.Player_Pointer_Label = new System.Windows.Forms.Label();
            this.Player_Position_Pointer_Label = new System.Windows.Forms.Label();
            this.ShowSackChkB = new System.Windows.Forms.CheckBox();
            this.ManualRefreshButton = new System.Windows.Forms.Button();
            this.CommandBoxButton = new System.Windows.Forms.Button();
            this.NPC_Item_Menu_Index_Button = new System.Windows.Forms.Button();
            this.NPC_Item_Menu_Index_UpDn = new System.Windows.Forms.NumericUpDown();
            this.Set_Target_Button = new System.Windows.Forms.Button();
            this.Set_Target_TB = new System.Windows.Forms.TextBox();
            this.FisherNoArrowButton = new System.Windows.Forms.Button();
            this.FisherSilverLeftButton = new System.Windows.Forms.Button();
            this.FisherSilverRightButton = new System.Windows.Forms.Button();
            this.FisherGoldLeftButton = new System.Windows.Forms.Button();
            this.FisherGoldRightButton = new System.Windows.Forms.Button();
            this.NPCListCB = new System.Windows.Forms.ComboBox();
            this.Speed_UpDn = new System.Windows.Forms.NumericUpDown();
            this.Speed_Button = new System.Windows.Forms.Button();
            this.Speed_Label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PosX_UpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PosH_UpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PosZ_UpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PosY_UpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NPC_Item_Menu_Index_UpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Speed_UpDn)).BeginInit();
            this.SuspendLayout();
            // 
            // OffsetCB
            // 
            this.OffsetCB.FormattingEnabled = true;
            this.OffsetCB.Location = new System.Drawing.Point(13, 13);
            this.OffsetCB.Name = "OffsetCB";
            this.OffsetCB.Size = new System.Drawing.Size(161, 21);
            this.OffsetCB.TabIndex = 1;
            // 
            // MainTextBox
            // 
            this.MainTextBox.Location = new System.Drawing.Point(13, 40);
            this.MainTextBox.Name = "MainTextBox";
            this.MainTextBox.Size = new System.Drawing.Size(280, 513);
            this.MainTextBox.TabIndex = 0;
            this.MainTextBox.Text = "";
            // 
            // processCB
            // 
            this.processCB.DropDownHeight = 250;
            this.processCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.processCB.FormattingEnabled = true;
            this.processCB.IntegralHeight = false;
            this.processCB.Location = new System.Drawing.Point(180, 13);
            this.processCB.Name = "processCB";
            this.processCB.Size = new System.Drawing.Size(113, 21);
            this.processCB.TabIndex = 2;
            this.processCB.SelectedIndexChanged += new System.EventHandler(this.processCB_SelectedIndexChanged);
            this.processCB.Click += new System.EventHandler(this.processCB_Click);
            // 
            // pcNameTB
            // 
            this.pcNameTB.Location = new System.Drawing.Point(299, 14);
            this.pcNameTB.Name = "pcNameTB";
            this.pcNameTB.Size = new System.Drawing.Size(100, 20);
            this.pcNameTB.TabIndex = 3;
            // 
            // Get_Position_Button
            // 
            this.Get_Position_Button.Location = new System.Drawing.Point(423, 122);
            this.Get_Position_Button.Name = "Get_Position_Button";
            this.Get_Position_Button.Size = new System.Drawing.Size(75, 23);
            this.Get_Position_Button.TabIndex = 4;
            this.Get_Position_Button.Text = "Get Pos";
            this.Get_Position_Button.UseVisualStyleBackColor = true;
            this.Get_Position_Button.Click += new System.EventHandler(this.Get_Position_Button_Click);
            // 
            // PosX_UpDn
            // 
            this.PosX_UpDn.DecimalPlaces = 1;
            this.PosX_UpDn.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.PosX_UpDn.Location = new System.Drawing.Point(341, 76);
            this.PosX_UpDn.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.PosX_UpDn.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.PosX_UpDn.Name = "PosX_UpDn";
            this.PosX_UpDn.Size = new System.Drawing.Size(65, 20);
            this.PosX_UpDn.TabIndex = 5;
            this.PosX_UpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.PosX_UpDn.ValueChanged += new System.EventHandler(this.PosX_UpDn_ValueChanged);
            // 
            // PosX_Label
            // 
            this.PosX_Label.AutoSize = true;
            this.PosX_Label.Location = new System.Drawing.Point(300, 78);
            this.PosX_Label.Name = "PosX_Label";
            this.PosX_Label.Size = new System.Drawing.Size(35, 13);
            this.PosX_Label.TabIndex = 6;
            this.PosX_Label.Text = "Pos X";
            // 
            // PosY_Label
            // 
            this.PosY_Label.AutoSize = true;
            this.PosY_Label.Location = new System.Drawing.Point(300, 104);
            this.PosY_Label.Name = "PosY_Label";
            this.PosY_Label.Size = new System.Drawing.Size(35, 13);
            this.PosY_Label.TabIndex = 7;
            this.PosY_Label.Text = "Pos Y";
            // 
            // PosZ_Label
            // 
            this.PosZ_Label.AutoSize = true;
            this.PosZ_Label.Location = new System.Drawing.Point(300, 130);
            this.PosZ_Label.Name = "PosZ_Label";
            this.PosZ_Label.Size = new System.Drawing.Size(35, 13);
            this.PosZ_Label.TabIndex = 8;
            this.PosZ_Label.Text = "Pos Z";
            // 
            // PosH_Label
            // 
            this.PosH_Label.AutoSize = true;
            this.PosH_Label.Location = new System.Drawing.Point(300, 156);
            this.PosH_Label.Name = "PosH_Label";
            this.PosH_Label.Size = new System.Drawing.Size(36, 13);
            this.PosH_Label.TabIndex = 9;
            this.PosH_Label.Text = "Pos H";
            // 
            // Set_Position_Button
            // 
            this.Set_Position_Button.Location = new System.Drawing.Point(423, 151);
            this.Set_Position_Button.Name = "Set_Position_Button";
            this.Set_Position_Button.Size = new System.Drawing.Size(75, 23);
            this.Set_Position_Button.TabIndex = 13;
            this.Set_Position_Button.Text = "Set Pos";
            this.Set_Position_Button.UseVisualStyleBackColor = true;
            this.Set_Position_Button.Click += new System.EventHandler(this.Set_Position_Button_Click);
            // 
            // PosH_UpDn
            // 
            this.PosH_UpDn.DecimalPlaces = 2;
            this.PosH_UpDn.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.PosH_UpDn.Location = new System.Drawing.Point(341, 154);
            this.PosH_UpDn.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            65536});
            this.PosH_UpDn.Minimum = new decimal(new int[] {
            32,
            0,
            0,
            -2147418112});
            this.PosH_UpDn.Name = "PosH_UpDn";
            this.PosH_UpDn.Size = new System.Drawing.Size(65, 20);
            this.PosH_UpDn.TabIndex = 14;
            this.PosH_UpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.PosH_UpDn.ValueChanged += new System.EventHandler(this.PosH_UpDn_ValueChanged);
            // 
            // PosZ_UpDn
            // 
            this.PosZ_UpDn.DecimalPlaces = 1;
            this.PosZ_UpDn.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.PosZ_UpDn.Location = new System.Drawing.Point(341, 128);
            this.PosZ_UpDn.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.PosZ_UpDn.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.PosZ_UpDn.Name = "PosZ_UpDn";
            this.PosZ_UpDn.Size = new System.Drawing.Size(65, 20);
            this.PosZ_UpDn.TabIndex = 15;
            this.PosZ_UpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.PosZ_UpDn.ValueChanged += new System.EventHandler(this.PosZ_UpDn_ValueChanged);
            // 
            // PosY_UpDn
            // 
            this.PosY_UpDn.DecimalPlaces = 1;
            this.PosY_UpDn.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.PosY_UpDn.Location = new System.Drawing.Point(341, 102);
            this.PosY_UpDn.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.PosY_UpDn.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.PosY_UpDn.Name = "PosY_UpDn";
            this.PosY_UpDn.Size = new System.Drawing.Size(65, 20);
            this.PosY_UpDn.TabIndex = 16;
            this.PosY_UpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.PosY_UpDn.ValueChanged += new System.EventHandler(this.PosY_UpDn_ValueChanged);
            // 
            // Player_Pointer_TB
            // 
            this.Player_Pointer_TB.Location = new System.Drawing.Point(453, 75);
            this.Player_Pointer_TB.Name = "Player_Pointer_TB";
            this.Player_Pointer_TB.Size = new System.Drawing.Size(75, 20);
            this.Player_Pointer_TB.TabIndex = 17;
            // 
            // Player_Position_Ptr_TB
            // 
            this.Player_Position_Ptr_TB.Location = new System.Drawing.Point(453, 99);
            this.Player_Position_Ptr_TB.Name = "Player_Position_Ptr_TB";
            this.Player_Position_Ptr_TB.Size = new System.Drawing.Size(75, 20);
            this.Player_Position_Ptr_TB.TabIndex = 18;
            // 
            // Reset_Pointers_Button
            // 
            this.Reset_Pointers_Button.Location = new System.Drawing.Point(381, 180);
            this.Reset_Pointers_Button.Name = "Reset_Pointers_Button";
            this.Reset_Pointers_Button.Size = new System.Drawing.Size(75, 23);
            this.Reset_Pointers_Button.TabIndex = 19;
            this.Reset_Pointers_Button.Text = "Reset Ptrs";
            this.Reset_Pointers_Button.UseVisualStyleBackColor = true;
            this.Reset_Pointers_Button.Click += new System.EventHandler(this.Reset_Pointers_Button_Click);
            // 
            // Player_Pointer_Label
            // 
            this.Player_Pointer_Label.AutoSize = true;
            this.Player_Pointer_Label.Location = new System.Drawing.Point(412, 78);
            this.Player_Pointer_Label.Name = "Player_Pointer_Label";
            this.Player_Pointer_Label.Size = new System.Drawing.Size(40, 13);
            this.Player_Pointer_Label.TabIndex = 20;
            this.Player_Pointer_Label.Text = "Plyr Ptr";
            // 
            // Player_Position_Pointer_Label
            // 
            this.Player_Position_Pointer_Label.AutoSize = true;
            this.Player_Position_Pointer_Label.Location = new System.Drawing.Point(412, 102);
            this.Player_Position_Pointer_Label.Name = "Player_Position_Pointer_Label";
            this.Player_Position_Pointer_Label.Size = new System.Drawing.Size(41, 13);
            this.Player_Position_Pointer_Label.TabIndex = 21;
            this.Player_Position_Pointer_Label.Text = "Pos Ptr";
            // 
            // ShowSackChkB
            // 
            this.ShowSackChkB.AutoSize = true;
            this.ShowSackChkB.Location = new System.Drawing.Point(303, 244);
            this.ShowSackChkB.Name = "ShowSackChkB";
            this.ShowSackChkB.Size = new System.Drawing.Size(107, 17);
            this.ShowSackChkB.TabIndex = 22;
            this.ShowSackChkB.Text = "Show Sack Data";
            this.ShowSackChkB.UseVisualStyleBackColor = true;
            this.ShowSackChkB.CheckedChanged += new System.EventHandler(this.ShowSackChkB_CheckedChanged);
            // 
            // ManualRefreshButton
            // 
            this.ManualRefreshButton.Location = new System.Drawing.Point(315, 267);
            this.ManualRefreshButton.Name = "ManualRefreshButton";
            this.ManualRefreshButton.Size = new System.Drawing.Size(75, 23);
            this.ManualRefreshButton.TabIndex = 23;
            this.ManualRefreshButton.Text = "Refresh";
            this.ManualRefreshButton.UseVisualStyleBackColor = true;
            this.ManualRefreshButton.Click += new System.EventHandler(this.ManualRefreshButton_Click);
            // 
            // CommandBoxButton
            // 
            this.CommandBoxButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(57)))), ((int)(((byte)(92)))));
            this.CommandBoxButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CommandBoxButton.BackgroundImage")));
            this.CommandBoxButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.CommandBoxButton.Location = new System.Drawing.Point(299, 530);
            this.CommandBoxButton.Name = "CommandBoxButton";
            this.CommandBoxButton.Size = new System.Drawing.Size(32, 18);
            this.CommandBoxButton.TabIndex = 24;
            this.CommandBoxButton.UseVisualStyleBackColor = false;
            this.CommandBoxButton.Click += new System.EventHandler(this.CommandBoxButton_Click);
            // 
            // NPC_Item_Menu_Index_Button
            // 
            this.NPC_Item_Menu_Index_Button.Location = new System.Drawing.Point(396, 303);
            this.NPC_Item_Menu_Index_Button.Name = "NPC_Item_Menu_Index_Button";
            this.NPC_Item_Menu_Index_Button.Size = new System.Drawing.Size(75, 23);
            this.NPC_Item_Menu_Index_Button.TabIndex = 25;
            this.NPC_Item_Menu_Index_Button.Text = "Set Index";
            this.NPC_Item_Menu_Index_Button.UseVisualStyleBackColor = true;
            this.NPC_Item_Menu_Index_Button.Click += new System.EventHandler(this.NPC_Item_Menu_Index_Button_Click);
            // 
            // NPC_Item_Menu_Index_UpDn
            // 
            this.NPC_Item_Menu_Index_UpDn.Location = new System.Drawing.Point(315, 305);
            this.NPC_Item_Menu_Index_UpDn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NPC_Item_Menu_Index_UpDn.Name = "NPC_Item_Menu_Index_UpDn";
            this.NPC_Item_Menu_Index_UpDn.Size = new System.Drawing.Size(75, 20);
            this.NPC_Item_Menu_Index_UpDn.TabIndex = 26;
            this.NPC_Item_Menu_Index_UpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.NPC_Item_Menu_Index_UpDn.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // Set_Target_Button
            // 
            this.Set_Target_Button.Location = new System.Drawing.Point(396, 332);
            this.Set_Target_Button.Name = "Set_Target_Button";
            this.Set_Target_Button.Size = new System.Drawing.Size(75, 23);
            this.Set_Target_Button.TabIndex = 27;
            this.Set_Target_Button.Text = "Set Target";
            this.Set_Target_Button.UseVisualStyleBackColor = true;
            // 
            // Set_Target_TB
            // 
            this.Set_Target_TB.Location = new System.Drawing.Point(315, 334);
            this.Set_Target_TB.Name = "Set_Target_TB";
            this.Set_Target_TB.Size = new System.Drawing.Size(75, 20);
            this.Set_Target_TB.TabIndex = 28;
            // 
            // FisherNoArrowButton
            // 
            this.FisherNoArrowButton.Location = new System.Drawing.Point(360, 374);
            this.FisherNoArrowButton.Name = "FisherNoArrowButton";
            this.FisherNoArrowButton.Size = new System.Drawing.Size(75, 23);
            this.FisherNoArrowButton.TabIndex = 29;
            this.FisherNoArrowButton.Text = "No Arrow";
            this.FisherNoArrowButton.UseVisualStyleBackColor = true;
            this.FisherNoArrowButton.Click += new System.EventHandler(this.FisherNoArrowButton_Click);
            // 
            // FisherSilverLeftButton
            // 
            this.FisherSilverLeftButton.Location = new System.Drawing.Point(315, 403);
            this.FisherSilverLeftButton.Name = "FisherSilverLeftButton";
            this.FisherSilverLeftButton.Size = new System.Drawing.Size(75, 23);
            this.FisherSilverLeftButton.TabIndex = 30;
            this.FisherSilverLeftButton.Text = "Silver Left";
            this.FisherSilverLeftButton.UseVisualStyleBackColor = true;
            this.FisherSilverLeftButton.Click += new System.EventHandler(this.FisherSilverLeftButton_Click);
            // 
            // FisherSilverRightButton
            // 
            this.FisherSilverRightButton.Location = new System.Drawing.Point(406, 403);
            this.FisherSilverRightButton.Name = "FisherSilverRightButton";
            this.FisherSilverRightButton.Size = new System.Drawing.Size(75, 23);
            this.FisherSilverRightButton.TabIndex = 31;
            this.FisherSilverRightButton.Text = "Silver Right";
            this.FisherSilverRightButton.UseVisualStyleBackColor = true;
            this.FisherSilverRightButton.Click += new System.EventHandler(this.FisherSilverRightButton_Click);
            // 
            // FisherGoldLeftButton
            // 
            this.FisherGoldLeftButton.Location = new System.Drawing.Point(315, 432);
            this.FisherGoldLeftButton.Name = "FisherGoldLeftButton";
            this.FisherGoldLeftButton.Size = new System.Drawing.Size(75, 23);
            this.FisherGoldLeftButton.TabIndex = 32;
            this.FisherGoldLeftButton.Text = "Gold Left";
            this.FisherGoldLeftButton.UseVisualStyleBackColor = true;
            this.FisherGoldLeftButton.Click += new System.EventHandler(this.FisherGoldLeftButton_Click);
            // 
            // FisherGoldRightButton
            // 
            this.FisherGoldRightButton.Location = new System.Drawing.Point(406, 432);
            this.FisherGoldRightButton.Name = "FisherGoldRightButton";
            this.FisherGoldRightButton.Size = new System.Drawing.Size(75, 23);
            this.FisherGoldRightButton.TabIndex = 33;
            this.FisherGoldRightButton.Text = "Gold Right";
            this.FisherGoldRightButton.UseVisualStyleBackColor = true;
            this.FisherGoldRightButton.Click += new System.EventHandler(this.FisherGoldRightButton_Click);
            // 
            // NPCListCB
            // 
            this.NPCListCB.DropDownHeight = 250;
            this.NPCListCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NPCListCB.FormattingEnabled = true;
            this.NPCListCB.IntegralHeight = false;
            this.NPCListCB.Location = new System.Drawing.Point(406, 14);
            this.NPCListCB.Name = "NPCListCB";
            this.NPCListCB.Size = new System.Drawing.Size(113, 21);
            this.NPCListCB.TabIndex = 34;
            this.NPCListCB.SelectedIndexChanged += new System.EventHandler(this.NPCListCB_SelectedIndexChanged);
            this.NPCListCB.Click += new System.EventHandler(this.NPCListCB_Click);
            // 
            // Speed_UpDn
            // 
            this.Speed_UpDn.DecimalPlaces = 2;
            this.Speed_UpDn.Increment = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.Speed_UpDn.Location = new System.Drawing.Point(341, 212);
            this.Speed_UpDn.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.Speed_UpDn.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.Speed_UpDn.Name = "Speed_UpDn";
            this.Speed_UpDn.Size = new System.Drawing.Size(65, 20);
            this.Speed_UpDn.TabIndex = 37;
            this.Speed_UpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Speed_UpDn.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // Speed_Button
            // 
            this.Speed_Button.Location = new System.Drawing.Point(423, 209);
            this.Speed_Button.Name = "Speed_Button";
            this.Speed_Button.Size = new System.Drawing.Size(75, 23);
            this.Speed_Button.TabIndex = 36;
            this.Speed_Button.Text = "Set Speed";
            this.Speed_Button.UseVisualStyleBackColor = true;
            this.Speed_Button.Click += new System.EventHandler(this.Speed_Button_Click);
            // 
            // Speed_Label
            // 
            this.Speed_Label.AutoSize = true;
            this.Speed_Label.Location = new System.Drawing.Point(300, 214);
            this.Speed_Label.Name = "Speed_Label";
            this.Speed_Label.Size = new System.Drawing.Size(38, 13);
            this.Speed_Label.TabIndex = 35;
            this.Speed_Label.Text = "Speed";
            // 
            // IocaineOffsetsHelperForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 565);
            this.Controls.Add(this.Speed_UpDn);
            this.Controls.Add(this.Speed_Button);
            this.Controls.Add(this.Speed_Label);
            this.Controls.Add(this.NPCListCB);
            this.Controls.Add(this.FisherGoldRightButton);
            this.Controls.Add(this.FisherGoldLeftButton);
            this.Controls.Add(this.FisherSilverRightButton);
            this.Controls.Add(this.FisherSilverLeftButton);
            this.Controls.Add(this.FisherNoArrowButton);
            this.Controls.Add(this.Set_Target_TB);
            this.Controls.Add(this.Set_Target_Button);
            this.Controls.Add(this.NPC_Item_Menu_Index_UpDn);
            this.Controls.Add(this.NPC_Item_Menu_Index_Button);
            this.Controls.Add(this.CommandBoxButton);
            this.Controls.Add(this.ManualRefreshButton);
            this.Controls.Add(this.ShowSackChkB);
            this.Controls.Add(this.Reset_Pointers_Button);
            this.Controls.Add(this.Player_Position_Ptr_TB);
            this.Controls.Add(this.Player_Pointer_TB);
            this.Controls.Add(this.PosY_UpDn);
            this.Controls.Add(this.PosZ_UpDn);
            this.Controls.Add(this.PosH_UpDn);
            this.Controls.Add(this.Set_Position_Button);
            this.Controls.Add(this.PosX_UpDn);
            this.Controls.Add(this.Get_Position_Button);
            this.Controls.Add(this.pcNameTB);
            this.Controls.Add(this.processCB);
            this.Controls.Add(this.OffsetCB);
            this.Controls.Add(this.MainTextBox);
            this.Controls.Add(this.PosH_Label);
            this.Controls.Add(this.PosZ_Label);
            this.Controls.Add(this.PosY_Label);
            this.Controls.Add(this.PosX_Label);
            this.Controls.Add(this.Player_Position_Pointer_Label);
            this.Controls.Add(this.Player_Pointer_Label);
            this.Name = "IocaineOffsetsHelperForm";
            this.Text = "MemReads";
            ((System.ComponentModel.ISupportInitialize)(this.PosX_UpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PosH_UpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PosZ_UpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PosY_UpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NPC_Item_Menu_Index_UpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Speed_UpDn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox OffsetCB;
        private System.Windows.Forms.RichTextBox MainTextBox;
        private System.Windows.Forms.ComboBox processCB;
        private System.Windows.Forms.TextBox pcNameTB;
        private System.Windows.Forms.Button Get_Position_Button;
        private System.Windows.Forms.NumericUpDown PosX_UpDn;
        private System.Windows.Forms.Label PosX_Label;
        private System.Windows.Forms.Label PosY_Label;
        private System.Windows.Forms.Label PosZ_Label;
        private System.Windows.Forms.Label PosH_Label;
        private System.Windows.Forms.Button Set_Position_Button;
        private System.Windows.Forms.NumericUpDown PosH_UpDn;
        private System.Windows.Forms.NumericUpDown PosZ_UpDn;
        private System.Windows.Forms.NumericUpDown PosY_UpDn;
        private System.Windows.Forms.TextBox Player_Pointer_TB;
        private System.Windows.Forms.TextBox Player_Position_Ptr_TB;
        private System.Windows.Forms.Button Reset_Pointers_Button;
        private System.Windows.Forms.Label Player_Pointer_Label;
        private System.Windows.Forms.Label Player_Position_Pointer_Label;
        private System.Windows.Forms.CheckBox ShowSackChkB;
        private System.Windows.Forms.Button ManualRefreshButton;
        private System.Windows.Forms.Button CommandBoxButton;
        private System.Windows.Forms.Button NPC_Item_Menu_Index_Button;
        private System.Windows.Forms.NumericUpDown NPC_Item_Menu_Index_UpDn;
        private System.Windows.Forms.Button Set_Target_Button;
        private System.Windows.Forms.TextBox Set_Target_TB;
        private System.Windows.Forms.Button FisherNoArrowButton;
        private System.Windows.Forms.Button FisherSilverLeftButton;
        private System.Windows.Forms.Button FisherSilverRightButton;
        private System.Windows.Forms.Button FisherGoldLeftButton;
        private System.Windows.Forms.Button FisherGoldRightButton;
        private System.Windows.Forms.ComboBox NPCListCB;
        private System.Windows.Forms.NumericUpDown Speed_UpDn;
        private System.Windows.Forms.Button Speed_Button;
        private System.Windows.Forms.Label Speed_Label;
    }
}

