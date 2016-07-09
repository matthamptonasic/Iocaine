namespace Iocaine2
{
    partial class WMS_Settings_Form
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
            this.ContinuousScanChkB = new System.Windows.Forms.CheckBox();
            this.ScanPeriodUpDown = new System.Windows.Forms.NumericUpDown();
            this.ScanPeriodLabel = new System.Windows.Forms.Label();
            this.SaveCharOfflineChkB = new System.Windows.Forms.CheckBox();
            this.CharacterCB = new System.Windows.Forms.ComboBox();
            this.DeleteButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ScanPeriodUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // Apply_Button
            // 
            this.Apply_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Apply_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Apply_Button.Location = new System.Drawing.Point(13, 124);
            this.Apply_Button.Name = "Apply_Button";
            this.Apply_Button.Size = new System.Drawing.Size(60, 23);
            this.Apply_Button.TabIndex = 8;
            this.Apply_Button.Text = "Apply";
            this.Apply_Button.UseVisualStyleBackColor = true;
            this.Apply_Button.Click += new System.EventHandler(this.Apply_Button_Click);
            // 
            // OK_Button
            // 
            this.OK_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK_Button.Location = new System.Drawing.Point(145, 124);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(60, 23);
            this.OK_Button.TabIndex = 7;
            this.OK_Button.Text = "OK";
            this.OK_Button.UseVisualStyleBackColor = true;
            this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Button.Location = new System.Drawing.Point(79, 124);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(60, 23);
            this.Cancel_Button.TabIndex = 6;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // ContinuousScanChkB
            // 
            this.ContinuousScanChkB.AutoSize = true;
            this.ContinuousScanChkB.Location = new System.Drawing.Point(13, 13);
            this.ContinuousScanChkB.Name = "ContinuousScanChkB";
            this.ContinuousScanChkB.Size = new System.Drawing.Size(107, 17);
            this.ContinuousScanChkB.TabIndex = 9;
            this.ContinuousScanChkB.Text = "Continuous Scan";
            this.ContinuousScanChkB.UseVisualStyleBackColor = true;
            // 
            // ScanPeriodUpDown
            // 
            this.ScanPeriodUpDown.Location = new System.Drawing.Point(13, 37);
            this.ScanPeriodUpDown.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.ScanPeriodUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ScanPeriodUpDown.Name = "ScanPeriodUpDown";
            this.ScanPeriodUpDown.Size = new System.Drawing.Size(47, 20);
            this.ScanPeriodUpDown.TabIndex = 10;
            this.ScanPeriodUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ScanPeriodUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // ScanPeriodLabel
            // 
            this.ScanPeriodLabel.AutoSize = true;
            this.ScanPeriodLabel.Location = new System.Drawing.Point(67, 40);
            this.ScanPeriodLabel.Name = "ScanPeriodLabel";
            this.ScanPeriodLabel.Size = new System.Drawing.Size(49, 13);
            this.ScanPeriodLabel.TabIndex = 11;
            this.ScanPeriodLabel.Text = "Seconds";
            // 
            // SaveCharOfflineChkB
            // 
            this.SaveCharOfflineChkB.AutoSize = true;
            this.SaveCharOfflineChkB.Location = new System.Drawing.Point(13, 64);
            this.SaveCharOfflineChkB.Name = "SaveCharOfflineChkB";
            this.SaveCharOfflineChkB.Size = new System.Drawing.Size(156, 17);
            this.SaveCharOfflineChkB.TabIndex = 12;
            this.SaveCharOfflineChkB.Text = "Save This Character Offline";
            this.SaveCharOfflineChkB.UseVisualStyleBackColor = true;
            // 
            // CharacterCB
            // 
            this.CharacterCB.FormattingEnabled = true;
            this.CharacterCB.Location = new System.Drawing.Point(13, 88);
            this.CharacterCB.Name = "CharacterCB";
            this.CharacterCB.Size = new System.Drawing.Size(120, 21);
            this.CharacterCB.TabIndex = 13;
            // 
            // DeleteButton
            // 
            this.DeleteButton.Location = new System.Drawing.Point(139, 87);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(60, 23);
            this.DeleteButton.TabIndex = 14;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // WMS_Settings_Form
            // 
            this.AcceptButton = this.OK_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_Button;
            this.ClientSize = new System.Drawing.Size(217, 159);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.CharacterCB);
            this.Controls.Add(this.SaveCharOfflineChkB);
            this.Controls.Add(this.ScanPeriodLabel);
            this.Controls.Add(this.ScanPeriodUpDown);
            this.Controls.Add(this.ContinuousScanChkB);
            this.Controls.Add(this.Apply_Button);
            this.Controls.Add(this.OK_Button);
            this.Controls.Add(this.Cancel_Button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "WMS_Settings_Form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "WMS Settings";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.ScanPeriodUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button Apply_Button;
        public System.Windows.Forms.Button OK_Button;
        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.CheckBox ContinuousScanChkB;
        private System.Windows.Forms.NumericUpDown ScanPeriodUpDown;
        private System.Windows.Forms.Label ScanPeriodLabel;
        private System.Windows.Forms.CheckBox SaveCharOfflineChkB;
        private System.Windows.Forms.ComboBox CharacterCB;
        private System.Windows.Forms.Button DeleteButton;
    }
}