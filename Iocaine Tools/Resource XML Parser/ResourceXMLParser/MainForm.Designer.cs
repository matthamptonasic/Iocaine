namespace ResourceXMLParser
{
    partial class MainForm
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
            this.FileSearchListBox = new System.Windows.Forms.ListBox();
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.AddFileButton = new System.Windows.Forms.Button();
            this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ProcessButton = new System.Windows.Forms.Button();
            this.ItemNameCheckBox = new System.Windows.Forms.CheckBox();
            this.ItemIDCheckBox = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DescriptionCheckBox = new System.Windows.Forms.CheckBox();
            this.JugSizeCheckBox = new System.Windows.Forms.CheckBox();
            this.CombatSkillCheckBox = new System.Windows.Forms.CheckBox();
            this.DpsCheckBox = new System.Windows.Forms.CheckBox();
            this.DelayCheckBox = new System.Windows.Forms.CheckBox();
            this.DamageCheckBox = new System.Windows.Forms.CheckBox();
            this.StorageSlotsCheckBox = new System.Windows.Forms.CheckBox();
            this.ElementCheckBox = new System.Windows.Forms.CheckBox();
            this.ReuseDelayCheckBox = new System.Windows.Forms.CheckBox();
            this.UseDelayCheckBox = new System.Windows.Forms.CheckBox();
            this.CastTimeCheckBox = new System.Windows.Forms.CheckBox();
            this.ShieldSizeCheckBox = new System.Windows.Forms.CheckBox();
            this.ImageCheckBox = new System.Windows.Forms.CheckBox();
            this.JobsCheckBox = new System.Windows.Forms.CheckBox();
            this.RacesCheckBox = new System.Windows.Forms.CheckBox();
            this.SlotsCheckBox = new System.Windows.Forms.CheckBox();
            this.LevelCheckBox = new System.Windows.Forms.CheckBox();
            this.LogNamePCheckBox = new System.Windows.Forms.CheckBox();
            this.LogNameSCheckBox = new System.Windows.Forms.CheckBox();
            this.ValidTargetsCheckBox = new System.Windows.Forms.CheckBox();
            this.ResourceIdCheckBox = new System.Windows.Forms.CheckBox();
            this.TypeCheckBox = new System.Windows.Forms.CheckBox();
            this.StackSizeCheckBox = new System.Windows.Forms.CheckBox();
            this.FlagsCheckBox = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FileSearchListBox
            // 
            this.FileSearchListBox.FormattingEnabled = true;
            this.FileSearchListBox.HorizontalScrollbar = true;
            this.FileSearchListBox.Location = new System.Drawing.Point(12, 12);
            this.FileSearchListBox.Name = "FileSearchListBox";
            this.FileSearchListBox.ScrollAlwaysVisible = true;
            this.FileSearchListBox.Size = new System.Drawing.Size(462, 82);
            this.FileSearchListBox.TabIndex = 0;
            // 
            // OpenFileDialog
            // 
            this.OpenFileDialog.FileName = "*.xml";
            this.OpenFileDialog.Filter = "XML file|*.xml";
            this.OpenFileDialog.InitialDirectory = "..\\..\\..\\..\\..\\Iocaine IP\\Game Info";
            this.OpenFileDialog.Multiselect = true;
            this.OpenFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialog_FileOk);
            // 
            // AddFileButton
            // 
            this.AddFileButton.Location = new System.Drawing.Point(12, 230);
            this.AddFileButton.Name = "AddFileButton";
            this.AddFileButton.Size = new System.Drawing.Size(75, 23);
            this.AddFileButton.TabIndex = 1;
            this.AddFileButton.Text = "Add";
            this.AddFileButton.UseVisualStyleBackColor = true;
            this.AddFileButton.Click += new System.EventHandler(this.AddFileButton_Click);
            // 
            // SaveFileDialog
            // 
            this.SaveFileDialog.FileName = "Things.cs";
            this.SaveFileDialog.Filter = "C# files|*.cs";
            this.SaveFileDialog.InitialDirectory = "..\\..\\..\\..\\..\\Iocaine IP\\FFXIThingsDll\\FFXIThings";
            this.SaveFileDialog.OverwritePrompt = false;
            this.SaveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.SaveFileDialog_FileOk);
            // 
            // ProcessButton
            // 
            this.ProcessButton.Location = new System.Drawing.Point(94, 230);
            this.ProcessButton.Name = "ProcessButton";
            this.ProcessButton.Size = new System.Drawing.Size(75, 23);
            this.ProcessButton.TabIndex = 2;
            this.ProcessButton.Text = "Process";
            this.ProcessButton.UseVisualStyleBackColor = true;
            this.ProcessButton.Click += new System.EventHandler(this.ProcessButton_Click);
            // 
            // ItemNameCheckBox
            // 
            this.ItemNameCheckBox.AutoSize = true;
            this.ItemNameCheckBox.Checked = true;
            this.ItemNameCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ItemNameCheckBox.Enabled = false;
            this.ItemNameCheckBox.Location = new System.Drawing.Point(3, 3);
            this.ItemNameCheckBox.Name = "ItemNameCheckBox";
            this.ItemNameCheckBox.Size = new System.Drawing.Size(77, 17);
            this.ItemNameCheckBox.TabIndex = 3;
            this.ItemNameCheckBox.Text = "Item Name";
            this.ItemNameCheckBox.UseVisualStyleBackColor = true;
            // 
            // ItemIDCheckBox
            // 
            this.ItemIDCheckBox.AutoSize = true;
            this.ItemIDCheckBox.Checked = true;
            this.ItemIDCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ItemIDCheckBox.Enabled = false;
            this.ItemIDCheckBox.Location = new System.Drawing.Point(3, 21);
            this.ItemIDCheckBox.Name = "ItemIDCheckBox";
            this.ItemIDCheckBox.Size = new System.Drawing.Size(60, 17);
            this.ItemIDCheckBox.TabIndex = 4;
            this.ItemIDCheckBox.Text = "Item ID";
            this.ItemIDCheckBox.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.DescriptionCheckBox);
            this.panel1.Controls.Add(this.JugSizeCheckBox);
            this.panel1.Controls.Add(this.CombatSkillCheckBox);
            this.panel1.Controls.Add(this.DpsCheckBox);
            this.panel1.Controls.Add(this.DelayCheckBox);
            this.panel1.Controls.Add(this.DamageCheckBox);
            this.panel1.Controls.Add(this.StorageSlotsCheckBox);
            this.panel1.Controls.Add(this.ElementCheckBox);
            this.panel1.Controls.Add(this.ReuseDelayCheckBox);
            this.panel1.Controls.Add(this.UseDelayCheckBox);
            this.panel1.Controls.Add(this.CastTimeCheckBox);
            this.panel1.Controls.Add(this.ShieldSizeCheckBox);
            this.panel1.Controls.Add(this.ImageCheckBox);
            this.panel1.Controls.Add(this.JobsCheckBox);
            this.panel1.Controls.Add(this.RacesCheckBox);
            this.panel1.Controls.Add(this.SlotsCheckBox);
            this.panel1.Controls.Add(this.LevelCheckBox);
            this.panel1.Controls.Add(this.LogNamePCheckBox);
            this.panel1.Controls.Add(this.LogNameSCheckBox);
            this.panel1.Controls.Add(this.ValidTargetsCheckBox);
            this.panel1.Controls.Add(this.ResourceIdCheckBox);
            this.panel1.Controls.Add(this.TypeCheckBox);
            this.panel1.Controls.Add(this.StackSizeCheckBox);
            this.panel1.Controls.Add(this.FlagsCheckBox);
            this.panel1.Controls.Add(this.ItemIDCheckBox);
            this.panel1.Controls.Add(this.ItemNameCheckBox);
            this.panel1.Location = new System.Drawing.Point(12, 107);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(462, 117);
            this.panel1.TabIndex = 5;
            // 
            // DescriptionCheckBox
            // 
            this.DescriptionCheckBox.AutoSize = true;
            this.DescriptionCheckBox.Location = new System.Drawing.Point(3, 93);
            this.DescriptionCheckBox.Name = "DescriptionCheckBox";
            this.DescriptionCheckBox.Size = new System.Drawing.Size(79, 17);
            this.DescriptionCheckBox.TabIndex = 28;
            this.DescriptionCheckBox.Text = "Description";
            this.DescriptionCheckBox.UseVisualStyleBackColor = true;
            // 
            // JugSizeCheckBox
            // 
            this.JugSizeCheckBox.AutoSize = true;
            this.JugSizeCheckBox.Location = new System.Drawing.Point(376, 75);
            this.JugSizeCheckBox.Name = "JugSizeCheckBox";
            this.JugSizeCheckBox.Size = new System.Drawing.Size(66, 17);
            this.JugSizeCheckBox.TabIndex = 27;
            this.JugSizeCheckBox.Text = "Jug Size";
            this.JugSizeCheckBox.UseVisualStyleBackColor = true;
            // 
            // CombatSkillCheckBox
            // 
            this.CombatSkillCheckBox.AutoSize = true;
            this.CombatSkillCheckBox.Checked = true;
            this.CombatSkillCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CombatSkillCheckBox.Location = new System.Drawing.Point(376, 57);
            this.CombatSkillCheckBox.Name = "CombatSkillCheckBox";
            this.CombatSkillCheckBox.Size = new System.Drawing.Size(84, 17);
            this.CombatSkillCheckBox.TabIndex = 26;
            this.CombatSkillCheckBox.Text = "Combat Skill";
            this.CombatSkillCheckBox.UseVisualStyleBackColor = true;
            // 
            // DpsCheckBox
            // 
            this.DpsCheckBox.AutoSize = true;
            this.DpsCheckBox.Location = new System.Drawing.Point(376, 39);
            this.DpsCheckBox.Name = "DpsCheckBox";
            this.DpsCheckBox.Size = new System.Drawing.Size(48, 17);
            this.DpsCheckBox.TabIndex = 25;
            this.DpsCheckBox.Text = "DPS";
            this.DpsCheckBox.UseVisualStyleBackColor = true;
            // 
            // DelayCheckBox
            // 
            this.DelayCheckBox.AutoSize = true;
            this.DelayCheckBox.Location = new System.Drawing.Point(376, 21);
            this.DelayCheckBox.Name = "DelayCheckBox";
            this.DelayCheckBox.Size = new System.Drawing.Size(53, 17);
            this.DelayCheckBox.TabIndex = 24;
            this.DelayCheckBox.Text = "Delay";
            this.DelayCheckBox.UseVisualStyleBackColor = true;
            // 
            // DamageCheckBox
            // 
            this.DamageCheckBox.AutoSize = true;
            this.DamageCheckBox.Location = new System.Drawing.Point(376, 3);
            this.DamageCheckBox.Name = "DamageCheckBox";
            this.DamageCheckBox.Size = new System.Drawing.Size(66, 17);
            this.DamageCheckBox.TabIndex = 23;
            this.DamageCheckBox.Text = "Damage";
            this.DamageCheckBox.UseVisualStyleBackColor = true;
            // 
            // StorageSlotsCheckBox
            // 
            this.StorageSlotsCheckBox.AutoSize = true;
            this.StorageSlotsCheckBox.Location = new System.Drawing.Point(285, 75);
            this.StorageSlotsCheckBox.Name = "StorageSlotsCheckBox";
            this.StorageSlotsCheckBox.Size = new System.Drawing.Size(63, 17);
            this.StorageSlotsCheckBox.TabIndex = 22;
            this.StorageSlotsCheckBox.Text = "Storage";
            this.StorageSlotsCheckBox.UseVisualStyleBackColor = true;
            // 
            // ElementCheckBox
            // 
            this.ElementCheckBox.AutoSize = true;
            this.ElementCheckBox.Location = new System.Drawing.Point(285, 57);
            this.ElementCheckBox.Name = "ElementCheckBox";
            this.ElementCheckBox.Size = new System.Drawing.Size(64, 17);
            this.ElementCheckBox.TabIndex = 21;
            this.ElementCheckBox.Text = "Element";
            this.ElementCheckBox.UseVisualStyleBackColor = true;
            // 
            // ReuseDelayCheckBox
            // 
            this.ReuseDelayCheckBox.AutoSize = true;
            this.ReuseDelayCheckBox.Location = new System.Drawing.Point(285, 39);
            this.ReuseDelayCheckBox.Name = "ReuseDelayCheckBox";
            this.ReuseDelayCheckBox.Size = new System.Drawing.Size(87, 17);
            this.ReuseDelayCheckBox.TabIndex = 20;
            this.ReuseDelayCheckBox.Text = "Reuse Delay";
            this.ReuseDelayCheckBox.UseVisualStyleBackColor = true;
            // 
            // UseDelayCheckBox
            // 
            this.UseDelayCheckBox.AutoSize = true;
            this.UseDelayCheckBox.Location = new System.Drawing.Point(285, 21);
            this.UseDelayCheckBox.Name = "UseDelayCheckBox";
            this.UseDelayCheckBox.Size = new System.Drawing.Size(75, 17);
            this.UseDelayCheckBox.TabIndex = 19;
            this.UseDelayCheckBox.Text = "Use Delay";
            this.UseDelayCheckBox.UseVisualStyleBackColor = true;
            // 
            // CastTimeCheckBox
            // 
            this.CastTimeCheckBox.AutoSize = true;
            this.CastTimeCheckBox.Location = new System.Drawing.Point(285, 3);
            this.CastTimeCheckBox.Name = "CastTimeCheckBox";
            this.CastTimeCheckBox.Size = new System.Drawing.Size(73, 17);
            this.CastTimeCheckBox.TabIndex = 18;
            this.CastTimeCheckBox.Text = "Cast Time";
            this.CastTimeCheckBox.UseVisualStyleBackColor = true;
            // 
            // ShieldSizeCheckBox
            // 
            this.ShieldSizeCheckBox.AutoSize = true;
            this.ShieldSizeCheckBox.Location = new System.Drawing.Point(199, 75);
            this.ShieldSizeCheckBox.Name = "ShieldSizeCheckBox";
            this.ShieldSizeCheckBox.Size = new System.Drawing.Size(78, 17);
            this.ShieldSizeCheckBox.TabIndex = 17;
            this.ShieldSizeCheckBox.Text = "Shield Size";
            this.ShieldSizeCheckBox.UseVisualStyleBackColor = true;
            // 
            // ImageCheckBox
            // 
            this.ImageCheckBox.AutoSize = true;
            this.ImageCheckBox.Location = new System.Drawing.Point(95, 75);
            this.ImageCheckBox.Name = "ImageCheckBox";
            this.ImageCheckBox.Size = new System.Drawing.Size(55, 17);
            this.ImageCheckBox.TabIndex = 16;
            this.ImageCheckBox.Text = "Image";
            this.ImageCheckBox.UseVisualStyleBackColor = true;
            // 
            // JobsCheckBox
            // 
            this.JobsCheckBox.AutoSize = true;
            this.JobsCheckBox.Location = new System.Drawing.Point(199, 57);
            this.JobsCheckBox.Name = "JobsCheckBox";
            this.JobsCheckBox.Size = new System.Drawing.Size(48, 17);
            this.JobsCheckBox.TabIndex = 15;
            this.JobsCheckBox.Text = "Jobs";
            this.JobsCheckBox.UseVisualStyleBackColor = true;
            // 
            // RacesCheckBox
            // 
            this.RacesCheckBox.AutoSize = true;
            this.RacesCheckBox.Location = new System.Drawing.Point(199, 39);
            this.RacesCheckBox.Name = "RacesCheckBox";
            this.RacesCheckBox.Size = new System.Drawing.Size(57, 17);
            this.RacesCheckBox.TabIndex = 14;
            this.RacesCheckBox.Text = "Races";
            this.RacesCheckBox.UseVisualStyleBackColor = true;
            // 
            // SlotsCheckBox
            // 
            this.SlotsCheckBox.AutoSize = true;
            this.SlotsCheckBox.Location = new System.Drawing.Point(199, 21);
            this.SlotsCheckBox.Name = "SlotsCheckBox";
            this.SlotsCheckBox.Size = new System.Drawing.Size(49, 17);
            this.SlotsCheckBox.TabIndex = 13;
            this.SlotsCheckBox.Text = "Slots";
            this.SlotsCheckBox.UseVisualStyleBackColor = true;
            // 
            // LevelCheckBox
            // 
            this.LevelCheckBox.AutoSize = true;
            this.LevelCheckBox.Location = new System.Drawing.Point(199, 3);
            this.LevelCheckBox.Name = "LevelCheckBox";
            this.LevelCheckBox.Size = new System.Drawing.Size(52, 17);
            this.LevelCheckBox.TabIndex = 12;
            this.LevelCheckBox.Text = "Level";
            this.LevelCheckBox.UseVisualStyleBackColor = true;
            // 
            // LogNamePCheckBox
            // 
            this.LogNamePCheckBox.AutoSize = true;
            this.LogNamePCheckBox.Checked = true;
            this.LogNamePCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.LogNamePCheckBox.Location = new System.Drawing.Point(95, 57);
            this.LogNamePCheckBox.Name = "LogNamePCheckBox";
            this.LogNamePCheckBox.Size = new System.Drawing.Size(93, 17);
            this.LogNamePCheckBox.TabIndex = 11;
            this.LogNamePCheckBox.Text = "Log Name Plr.";
            this.LogNamePCheckBox.UseVisualStyleBackColor = true;
            // 
            // LogNameSCheckBox
            // 
            this.LogNameSCheckBox.AutoSize = true;
            this.LogNameSCheckBox.Checked = true;
            this.LogNameSCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.LogNameSCheckBox.Location = new System.Drawing.Point(95, 39);
            this.LogNameSCheckBox.Name = "LogNameSCheckBox";
            this.LogNameSCheckBox.Size = new System.Drawing.Size(100, 17);
            this.LogNameSCheckBox.TabIndex = 10;
            this.LogNameSCheckBox.Text = "Log Name Sng.";
            this.LogNameSCheckBox.UseVisualStyleBackColor = true;
            // 
            // ValidTargetsCheckBox
            // 
            this.ValidTargetsCheckBox.AutoSize = true;
            this.ValidTargetsCheckBox.Location = new System.Drawing.Point(95, 21);
            this.ValidTargetsCheckBox.Name = "ValidTargetsCheckBox";
            this.ValidTargetsCheckBox.Size = new System.Drawing.Size(88, 17);
            this.ValidTargetsCheckBox.TabIndex = 9;
            this.ValidTargetsCheckBox.Text = "Valid Targets";
            this.ValidTargetsCheckBox.UseVisualStyleBackColor = true;
            // 
            // ResourceIdCheckBox
            // 
            this.ResourceIdCheckBox.AutoSize = true;
            this.ResourceIdCheckBox.Location = new System.Drawing.Point(95, 3);
            this.ResourceIdCheckBox.Name = "ResourceIdCheckBox";
            this.ResourceIdCheckBox.Size = new System.Drawing.Size(86, 17);
            this.ResourceIdCheckBox.TabIndex = 8;
            this.ResourceIdCheckBox.Text = "Resource ID";
            this.ResourceIdCheckBox.UseVisualStyleBackColor = true;
            // 
            // TypeCheckBox
            // 
            this.TypeCheckBox.AutoSize = true;
            this.TypeCheckBox.Checked = true;
            this.TypeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TypeCheckBox.Location = new System.Drawing.Point(3, 75);
            this.TypeCheckBox.Name = "TypeCheckBox";
            this.TypeCheckBox.Size = new System.Drawing.Size(50, 17);
            this.TypeCheckBox.TabIndex = 7;
            this.TypeCheckBox.Text = "Type";
            this.TypeCheckBox.UseVisualStyleBackColor = true;
            // 
            // StackSizeCheckBox
            // 
            this.StackSizeCheckBox.AutoSize = true;
            this.StackSizeCheckBox.Checked = true;
            this.StackSizeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StackSizeCheckBox.Location = new System.Drawing.Point(3, 57);
            this.StackSizeCheckBox.Name = "StackSizeCheckBox";
            this.StackSizeCheckBox.Size = new System.Drawing.Size(77, 17);
            this.StackSizeCheckBox.TabIndex = 6;
            this.StackSizeCheckBox.Text = "Stack Size";
            this.StackSizeCheckBox.UseVisualStyleBackColor = true;
            // 
            // FlagsCheckBox
            // 
            this.FlagsCheckBox.AutoSize = true;
            this.FlagsCheckBox.Location = new System.Drawing.Point(3, 39);
            this.FlagsCheckBox.Name = "FlagsCheckBox";
            this.FlagsCheckBox.Size = new System.Drawing.Size(51, 17);
            this.FlagsCheckBox.TabIndex = 5;
            this.FlagsCheckBox.Text = "Flags";
            this.FlagsCheckBox.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 265);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ProcessButton);
            this.Controls.Add(this.AddFileButton);
            this.Controls.Add(this.FileSearchListBox);
            this.Name = "MainForm";
            this.Text = "Pebbles Parser";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox FileSearchListBox;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
        private System.Windows.Forms.Button AddFileButton;
        private System.Windows.Forms.SaveFileDialog SaveFileDialog;
        private System.Windows.Forms.Button ProcessButton;
        private System.Windows.Forms.CheckBox ItemNameCheckBox;
        private System.Windows.Forms.CheckBox ItemIDCheckBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox FlagsCheckBox;
        private System.Windows.Forms.CheckBox StackSizeCheckBox;
        private System.Windows.Forms.CheckBox TypeCheckBox;
        private System.Windows.Forms.CheckBox ResourceIdCheckBox;
        private System.Windows.Forms.CheckBox ValidTargetsCheckBox;
        private System.Windows.Forms.CheckBox LogNameSCheckBox;
        private System.Windows.Forms.CheckBox LogNamePCheckBox;
        private System.Windows.Forms.CheckBox LevelCheckBox;
        private System.Windows.Forms.CheckBox SlotsCheckBox;
        private System.Windows.Forms.CheckBox RacesCheckBox;
        private System.Windows.Forms.CheckBox JobsCheckBox;
        private System.Windows.Forms.CheckBox ImageCheckBox;
        private System.Windows.Forms.CheckBox ShieldSizeCheckBox;
        private System.Windows.Forms.CheckBox CastTimeCheckBox;
        private System.Windows.Forms.CheckBox UseDelayCheckBox;
        private System.Windows.Forms.CheckBox ReuseDelayCheckBox;
        private System.Windows.Forms.CheckBox ElementCheckBox;
        private System.Windows.Forms.CheckBox StorageSlotsCheckBox;
        private System.Windows.Forms.CheckBox DamageCheckBox;
        private System.Windows.Forms.CheckBox DelayCheckBox;
        private System.Windows.Forms.CheckBox DpsCheckBox;
        private System.Windows.Forms.CheckBox CombatSkillCheckBox;
        private System.Windows.Forms.CheckBox JugSizeCheckBox;
        private System.Windows.Forms.CheckBox DescriptionCheckBox;
    }
}

