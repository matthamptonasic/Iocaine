namespace Iocaine2.Settings
{
    partial class Nav_Settings_Form
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
            this.Nav_Settings_Rec_Interval_UpDn = new System.Windows.Forms.NumericUpDown();
            this.Nav_Settings_Rec_Interval_Label = new System.Windows.Forms.Label();
            this.Nav_Settings_Rec_Min_Dist_UpDn = new System.Windows.Forms.NumericUpDown();
            this.Nav_Settings_Rec_Min_Dist_Label = new System.Windows.Forms.Label();
            this.Nav_Settings_Rec_Wait_UpDn = new System.Windows.Forms.NumericUpDown();
            this.Nav_Settings_Rec_Wait_Label = new System.Windows.Forms.Label();
            this.Nav_Settings_Done_Browse_File_To_Play_Button = new System.Windows.Forms.Button();
            this.Nav_Settings_Done_Play_Sound_Label = new System.Windows.Forms.Label();
            this.Nav_Settings_Done_Play_Sound_TB = new System.Windows.Forms.TextBox();
            this.Nav_Settings_Done_Open_File_To_Play = new System.Windows.Forms.OpenFileDialog();
            this.Nav_Settings_Prompt_Before_Processing_ChkB = new System.Windows.Forms.CheckBox();
            this.Nav_Settings_Sort_Tags_First_ChkB = new System.Windows.Forms.CheckBox();
            this.Nav_Settings_Sort_By_Last_Zone_ChkB = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.Nav_Settings_Rec_Interval_UpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Nav_Settings_Rec_Min_Dist_UpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Nav_Settings_Rec_Wait_UpDn)).BeginInit();
            this.SuspendLayout();
            // 
            // Apply_Button
            // 
            this.Apply_Button.Location = new System.Drawing.Point(246, 305);
            this.Apply_Button.Name = "Apply_Button";
            this.Apply_Button.Size = new System.Drawing.Size(60, 23);
            this.Apply_Button.TabIndex = 8;
            this.Apply_Button.Text = "Apply";
            this.Apply_Button.UseVisualStyleBackColor = true;
            this.Apply_Button.Click += new System.EventHandler(this.Apply_Button_Click);
            // 
            // OK_Button
            // 
            this.OK_Button.Location = new System.Drawing.Point(378, 305);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(60, 23);
            this.OK_Button.TabIndex = 7;
            this.OK_Button.Text = "OK";
            this.OK_Button.UseVisualStyleBackColor = true;
            this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Button.Location = new System.Drawing.Point(312, 305);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(60, 23);
            this.Cancel_Button.TabIndex = 6;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // Nav_Settings_Rec_Interval_UpDn
            // 
            this.Nav_Settings_Rec_Interval_UpDn.Increment = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.Nav_Settings_Rec_Interval_UpDn.Location = new System.Drawing.Point(12, 12);
            this.Nav_Settings_Rec_Interval_UpDn.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.Nav_Settings_Rec_Interval_UpDn.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.Nav_Settings_Rec_Interval_UpDn.Name = "Nav_Settings_Rec_Interval_UpDn";
            this.Nav_Settings_Rec_Interval_UpDn.Size = new System.Drawing.Size(60, 20);
            this.Nav_Settings_Rec_Interval_UpDn.TabIndex = 9;
            this.Nav_Settings_Rec_Interval_UpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Nav_Settings_Rec_Interval_UpDn.ThousandsSeparator = true;
            this.Nav_Settings_Rec_Interval_UpDn.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.Nav_Settings_Rec_Interval_UpDn.ValueChanged += new System.EventHandler(this.Nav_Settings_Rec_Interval_UpDn_ValueChanged);
            // 
            // Nav_Settings_Rec_Interval_Label
            // 
            this.Nav_Settings_Rec_Interval_Label.AutoSize = true;
            this.Nav_Settings_Rec_Interval_Label.Location = new System.Drawing.Point(78, 14);
            this.Nav_Settings_Rec_Interval_Label.Name = "Nav_Settings_Rec_Interval_Label";
            this.Nav_Settings_Rec_Interval_Label.Size = new System.Drawing.Size(153, 13);
            this.Nav_Settings_Rec_Interval_Label.TabIndex = 10;
            this.Nav_Settings_Rec_Interval_Label.Text = "Default Recording Interval (ms)";
            // 
            // Nav_Settings_Rec_Min_Dist_UpDn
            // 
            this.Nav_Settings_Rec_Min_Dist_UpDn.DecimalPlaces = 1;
            this.Nav_Settings_Rec_Min_Dist_UpDn.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Nav_Settings_Rec_Min_Dist_UpDn.Location = new System.Drawing.Point(12, 38);
            this.Nav_Settings_Rec_Min_Dist_UpDn.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.Nav_Settings_Rec_Min_Dist_UpDn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Nav_Settings_Rec_Min_Dist_UpDn.Name = "Nav_Settings_Rec_Min_Dist_UpDn";
            this.Nav_Settings_Rec_Min_Dist_UpDn.Size = new System.Drawing.Size(60, 20);
            this.Nav_Settings_Rec_Min_Dist_UpDn.TabIndex = 11;
            this.Nav_Settings_Rec_Min_Dist_UpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Nav_Settings_Rec_Min_Dist_UpDn.ThousandsSeparator = true;
            this.Nav_Settings_Rec_Min_Dist_UpDn.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.Nav_Settings_Rec_Min_Dist_UpDn.ValueChanged += new System.EventHandler(this.Nav_Settings_Rec_Min_Dist_UpDn_ValueChanged);
            // 
            // Nav_Settings_Rec_Min_Dist_Label
            // 
            this.Nav_Settings_Rec_Min_Dist_Label.AutoSize = true;
            this.Nav_Settings_Rec_Min_Dist_Label.Location = new System.Drawing.Point(78, 40);
            this.Nav_Settings_Rec_Min_Dist_Label.Name = "Nav_Settings_Rec_Min_Dist_Label";
            this.Nav_Settings_Rec_Min_Dist_Label.Size = new System.Drawing.Size(165, 13);
            this.Nav_Settings_Rec_Min_Dist_Label.TabIndex = 12;
            this.Nav_Settings_Rec_Min_Dist_Label.Text = "Default Min. Dist. Between Points";
            // 
            // Nav_Settings_Rec_Wait_UpDn
            // 
            this.Nav_Settings_Rec_Wait_UpDn.DecimalPlaces = 1;
            this.Nav_Settings_Rec_Wait_UpDn.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Nav_Settings_Rec_Wait_UpDn.Location = new System.Drawing.Point(12, 64);
            this.Nav_Settings_Rec_Wait_UpDn.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.Nav_Settings_Rec_Wait_UpDn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Nav_Settings_Rec_Wait_UpDn.Name = "Nav_Settings_Rec_Wait_UpDn";
            this.Nav_Settings_Rec_Wait_UpDn.Size = new System.Drawing.Size(60, 20);
            this.Nav_Settings_Rec_Wait_UpDn.TabIndex = 15;
            this.Nav_Settings_Rec_Wait_UpDn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Nav_Settings_Rec_Wait_UpDn.ThousandsSeparator = true;
            this.Nav_Settings_Rec_Wait_UpDn.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.Nav_Settings_Rec_Wait_UpDn.ValueChanged += new System.EventHandler(this.Nav_Settings_Rec_Wait_UpDn_ValueChanged);
            // 
            // Nav_Settings_Rec_Wait_Label
            // 
            this.Nav_Settings_Rec_Wait_Label.AutoSize = true;
            this.Nav_Settings_Rec_Wait_Label.Location = new System.Drawing.Point(78, 67);
            this.Nav_Settings_Rec_Wait_Label.Name = "Nav_Settings_Rec_Wait_Label";
            this.Nav_Settings_Rec_Wait_Label.Size = new System.Drawing.Size(141, 13);
            this.Nav_Settings_Rec_Wait_Label.TabIndex = 16;
            this.Nav_Settings_Rec_Wait_Label.Text = "Default Wait Time (seconds)";
            // 
            // Nav_Settings_Done_Browse_File_To_Play_Button
            // 
            this.Nav_Settings_Done_Browse_File_To_Play_Button.Location = new System.Drawing.Point(219, 102);
            this.Nav_Settings_Done_Browse_File_To_Play_Button.Name = "Nav_Settings_Done_Browse_File_To_Play_Button";
            this.Nav_Settings_Done_Browse_File_To_Play_Button.Size = new System.Drawing.Size(22, 21);
            this.Nav_Settings_Done_Browse_File_To_Play_Button.TabIndex = 38;
            this.Nav_Settings_Done_Browse_File_To_Play_Button.Text = "...";
            this.Nav_Settings_Done_Browse_File_To_Play_Button.UseVisualStyleBackColor = true;
            this.Nav_Settings_Done_Browse_File_To_Play_Button.Click += new System.EventHandler(this.Nav_Settings_Done_Browse_File_To_Play_Button_Click);
            // 
            // Nav_Settings_Done_Play_Sound_Label
            // 
            this.Nav_Settings_Done_Play_Sound_Label.AutoSize = true;
            this.Nav_Settings_Done_Play_Sound_Label.Location = new System.Drawing.Point(12, 87);
            this.Nav_Settings_Done_Play_Sound_Label.Name = "Nav_Settings_Done_Play_Sound_Label";
            this.Nav_Settings_Done_Play_Sound_Label.Size = new System.Drawing.Size(189, 13);
            this.Nav_Settings_Done_Play_Sound_Label.TabIndex = 37;
            this.Nav_Settings_Done_Play_Sound_Label.Text = "Say Message or Play File When Done:";
            // 
            // Nav_Settings_Done_Play_Sound_TB
            // 
            this.Nav_Settings_Done_Play_Sound_TB.Location = new System.Drawing.Point(12, 103);
            this.Nav_Settings_Done_Play_Sound_TB.Name = "Nav_Settings_Done_Play_Sound_TB";
            this.Nav_Settings_Done_Play_Sound_TB.Size = new System.Drawing.Size(201, 20);
            this.Nav_Settings_Done_Play_Sound_TB.TabIndex = 36;
            this.Nav_Settings_Done_Play_Sound_TB.Text = "Text to Say or File to Play";
            this.Nav_Settings_Done_Play_Sound_TB.TextChanged += new System.EventHandler(this.Nav_Settings_Done_Play_Sound_TB_TextChanged);
            // 
            // Nav_Settings_Done_Open_File_To_Play
            // 
            this.Nav_Settings_Done_Open_File_To_Play.FileName = "DoneOpenFileToPlay";
            // 
            // Nav_Settings_Prompt_Before_Processing_ChkB
            // 
            this.Nav_Settings_Prompt_Before_Processing_ChkB.AutoSize = true;
            this.Nav_Settings_Prompt_Before_Processing_ChkB.Location = new System.Drawing.Point(12, 130);
            this.Nav_Settings_Prompt_Before_Processing_ChkB.Name = "Nav_Settings_Prompt_Before_Processing_ChkB";
            this.Nav_Settings_Prompt_Before_Processing_ChkB.Size = new System.Drawing.Size(217, 17);
            this.Nav_Settings_Prompt_Before_Processing_ChkB.TabIndex = 39;
            this.Nav_Settings_Prompt_Before_Processing_ChkB.Text = "Prompt Before Processing on Right Click";
            this.Nav_Settings_Prompt_Before_Processing_ChkB.UseVisualStyleBackColor = true;
            this.Nav_Settings_Prompt_Before_Processing_ChkB.CheckedChanged += new System.EventHandler(this.Nav_Settings_Prompt_Before_Processing_ChkB_CheckedChanged);
            // 
            // Nav_Settings_Sort_Tags_First_ChkB
            // 
            this.Nav_Settings_Sort_Tags_First_ChkB.AutoSize = true;
            this.Nav_Settings_Sort_Tags_First_ChkB.Location = new System.Drawing.Point(12, 153);
            this.Nav_Settings_Sort_Tags_First_ChkB.Name = "Nav_Settings_Sort_Tags_First_ChkB";
            this.Nav_Settings_Sort_Tags_First_ChkB.Size = new System.Drawing.Size(153, 17);
            this.Nav_Settings_Sort_Tags_First_ChkB.TabIndex = 40;
            this.Nav_Settings_Sort_Tags_First_ChkB.Text = "Sort by Tags Before Zones";
            this.Nav_Settings_Sort_Tags_First_ChkB.UseVisualStyleBackColor = true;
            this.Nav_Settings_Sort_Tags_First_ChkB.CheckedChanged += new System.EventHandler(this.Nav_Settings_Sort_Tags_First_ChkB_CheckedChanged);
            // 
            // Nav_Settings_Sort_By_Last_Zone_ChkB
            // 
            this.Nav_Settings_Sort_By_Last_Zone_ChkB.AutoSize = true;
            this.Nav_Settings_Sort_By_Last_Zone_ChkB.Location = new System.Drawing.Point(12, 176);
            this.Nav_Settings_Sort_By_Last_Zone_ChkB.Name = "Nav_Settings_Sort_By_Last_Zone_ChkB";
            this.Nav_Settings_Sort_By_Last_Zone_ChkB.Size = new System.Drawing.Size(160, 17);
            this.Nav_Settings_Sort_By_Last_Zone_ChkB.TabIndex = 41;
            this.Nav_Settings_Sort_By_Last_Zone_ChkB.Text = "Sort Trips by Last Zone Only";
            this.Nav_Settings_Sort_By_Last_Zone_ChkB.UseVisualStyleBackColor = true;
            this.Nav_Settings_Sort_By_Last_Zone_ChkB.CheckedChanged += new System.EventHandler(this.Nav_Settings_Sort_By_Last_Zone_ChkB_CheckedChanged);
            // 
            // Nav_Settings_Form
            // 
            this.AcceptButton = this.OK_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_Button;
            this.ClientSize = new System.Drawing.Size(450, 340);
            this.Controls.Add(this.Nav_Settings_Sort_By_Last_Zone_ChkB);
            this.Controls.Add(this.Nav_Settings_Sort_Tags_First_ChkB);
            this.Controls.Add(this.Nav_Settings_Prompt_Before_Processing_ChkB);
            this.Controls.Add(this.Nav_Settings_Done_Browse_File_To_Play_Button);
            this.Controls.Add(this.Nav_Settings_Done_Play_Sound_Label);
            this.Controls.Add(this.Nav_Settings_Done_Play_Sound_TB);
            this.Controls.Add(this.Nav_Settings_Rec_Wait_UpDn);
            this.Controls.Add(this.Nav_Settings_Rec_Wait_Label);
            this.Controls.Add(this.Nav_Settings_Rec_Min_Dist_UpDn);
            this.Controls.Add(this.Nav_Settings_Rec_Min_Dist_Label);
            this.Controls.Add(this.Nav_Settings_Rec_Interval_Label);
            this.Controls.Add(this.Nav_Settings_Rec_Interval_UpDn);
            this.Controls.Add(this.Apply_Button);
            this.Controls.Add(this.OK_Button);
            this.Controls.Add(this.Cancel_Button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "Nav_Settings_Form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Navigation Settings";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.Nav_Settings_Rec_Interval_UpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Nav_Settings_Rec_Min_Dist_UpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Nav_Settings_Rec_Wait_UpDn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button Apply_Button;
        public System.Windows.Forms.Button OK_Button;
        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.NumericUpDown Nav_Settings_Rec_Interval_UpDn;
        private System.Windows.Forms.Label Nav_Settings_Rec_Interval_Label;
        private System.Windows.Forms.NumericUpDown Nav_Settings_Rec_Min_Dist_UpDn;
        private System.Windows.Forms.Label Nav_Settings_Rec_Min_Dist_Label;
        private System.Windows.Forms.NumericUpDown Nav_Settings_Rec_Wait_UpDn;
        private System.Windows.Forms.Label Nav_Settings_Rec_Wait_Label;
        private System.Windows.Forms.Button Nav_Settings_Done_Browse_File_To_Play_Button;
        private System.Windows.Forms.Label Nav_Settings_Done_Play_Sound_Label;
        private System.Windows.Forms.TextBox Nav_Settings_Done_Play_Sound_TB;
        private System.Windows.Forms.OpenFileDialog Nav_Settings_Done_Open_File_To_Play;
        private System.Windows.Forms.CheckBox Nav_Settings_Prompt_Before_Processing_ChkB;
        private System.Windows.Forms.CheckBox Nav_Settings_Sort_Tags_First_ChkB;
        private System.Windows.Forms.CheckBox Nav_Settings_Sort_By_Last_Zone_ChkB;
    }
}