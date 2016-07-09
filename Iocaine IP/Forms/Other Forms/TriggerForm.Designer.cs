namespace Iocaine2
{
    partial class TriggerForm
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
            this.TF_TriggerTypeCB = new System.Windows.Forms.ComboBox();
            this.TF_ApplyButton = new System.Windows.Forms.Button();
            this.TF_OKButton = new System.Windows.Forms.Button();
            this.TF_CancelButton = new System.Windows.Forms.Button();
            this.TF_ActionTypeCB = new System.Windows.Forms.ComboBox();
            this.TF_TriggerTypeLabel = new System.Windows.Forms.Label();
            this.TF_ActionTypeLabel = new System.Windows.Forms.Label();
            this.TF_TriggerPanel = new System.Windows.Forms.Panel();
            this.TF_ActionPanel = new System.Windows.Forms.Panel();
            this.TF_ActionSequenceLB = new System.Windows.Forms.ListBox();
            this.TF_ActionSequenceLabel = new System.Windows.Forms.Label();
            this.TF_AddActionButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TF_TriggerTypeCB
            // 
            this.TF_TriggerTypeCB.FormattingEnabled = true;
            this.TF_TriggerTypeCB.Location = new System.Drawing.Point(13, 19);
            this.TF_TriggerTypeCB.Name = "TF_TriggerTypeCB";
            this.TF_TriggerTypeCB.Size = new System.Drawing.Size(147, 21);
            this.TF_TriggerTypeCB.TabIndex = 0;
            this.TF_TriggerTypeCB.SelectedIndexChanged += new System.EventHandler(this.TF_TriggerTypeCB_SelectedIndexChanged);
            // 
            // TF_ApplyButton
            // 
            this.TF_ApplyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TF_ApplyButton.Location = new System.Drawing.Point(124, 86);
            this.TF_ApplyButton.Name = "TF_ApplyButton";
            this.TF_ApplyButton.Size = new System.Drawing.Size(50, 23);
            this.TF_ApplyButton.TabIndex = 1;
            this.TF_ApplyButton.Text = "Apply";
            this.TF_ApplyButton.UseVisualStyleBackColor = true;
            this.TF_ApplyButton.Click += new System.EventHandler(this.TF_ApplyButton_Click);
            // 
            // TF_OKButton
            // 
            this.TF_OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TF_OKButton.Location = new System.Drawing.Point(68, 86);
            this.TF_OKButton.Name = "TF_OKButton";
            this.TF_OKButton.Size = new System.Drawing.Size(50, 23);
            this.TF_OKButton.TabIndex = 2;
            this.TF_OKButton.Text = "OK";
            this.TF_OKButton.UseVisualStyleBackColor = true;
            this.TF_OKButton.Click += new System.EventHandler(this.TF_OKButton_Click);
            // 
            // TF_CancelButton
            // 
            this.TF_CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TF_CancelButton.Location = new System.Drawing.Point(12, 86);
            this.TF_CancelButton.Name = "TF_CancelButton";
            this.TF_CancelButton.Size = new System.Drawing.Size(50, 23);
            this.TF_CancelButton.TabIndex = 3;
            this.TF_CancelButton.Text = "Cancel";
            this.TF_CancelButton.UseVisualStyleBackColor = true;
            this.TF_CancelButton.Click += new System.EventHandler(this.TF_CancelButton_Click);
            // 
            // TF_ActionTypeCB
            // 
            this.TF_ActionTypeCB.FormattingEnabled = true;
            this.TF_ActionTypeCB.Location = new System.Drawing.Point(165, 19);
            this.TF_ActionTypeCB.Name = "TF_ActionTypeCB";
            this.TF_ActionTypeCB.Size = new System.Drawing.Size(147, 21);
            this.TF_ActionTypeCB.TabIndex = 4;
            this.TF_ActionTypeCB.SelectedIndexChanged += new System.EventHandler(this.TF_ActionTypeCB_SelectedIndexChanged);
            // 
            // TF_TriggerTypeLabel
            // 
            this.TF_TriggerTypeLabel.AutoSize = true;
            this.TF_TriggerTypeLabel.Location = new System.Drawing.Point(13, 4);
            this.TF_TriggerTypeLabel.Name = "TF_TriggerTypeLabel";
            this.TF_TriggerTypeLabel.Size = new System.Drawing.Size(67, 13);
            this.TF_TriggerTypeLabel.TabIndex = 5;
            this.TF_TriggerTypeLabel.Text = "Trigger Type";
            // 
            // TF_ActionTypeLabel
            // 
            this.TF_ActionTypeLabel.AutoSize = true;
            this.TF_ActionTypeLabel.Location = new System.Drawing.Point(165, 4);
            this.TF_ActionTypeLabel.Name = "TF_ActionTypeLabel";
            this.TF_ActionTypeLabel.Size = new System.Drawing.Size(64, 13);
            this.TF_ActionTypeLabel.TabIndex = 6;
            this.TF_ActionTypeLabel.Text = "Action Type";
            // 
            // TF_TriggerPanel
            // 
            this.TF_TriggerPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.TF_TriggerPanel.Location = new System.Drawing.Point(13, 47);
            this.TF_TriggerPanel.Name = "TF_TriggerPanel";
            this.TF_TriggerPanel.Size = new System.Drawing.Size(147, 30);
            this.TF_TriggerPanel.TabIndex = 7;
            // 
            // TF_ActionPanel
            // 
            this.TF_ActionPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.TF_ActionPanel.Location = new System.Drawing.Point(166, 46);
            this.TF_ActionPanel.Name = "TF_ActionPanel";
            this.TF_ActionPanel.Size = new System.Drawing.Size(147, 30);
            this.TF_ActionPanel.TabIndex = 8;
            // 
            // TF_ActionSequenceLB
            // 
            this.TF_ActionSequenceLB.FormattingEnabled = true;
            this.TF_ActionSequenceLB.Location = new System.Drawing.Point(319, 19);
            this.TF_ActionSequenceLB.Name = "TF_ActionSequenceLB";
            this.TF_ActionSequenceLB.Size = new System.Drawing.Size(160, 82);
            this.TF_ActionSequenceLB.TabIndex = 9;
            // 
            // TF_ActionSequenceLabel
            // 
            this.TF_ActionSequenceLabel.AutoSize = true;
            this.TF_ActionSequenceLabel.Location = new System.Drawing.Point(318, 4);
            this.TF_ActionSequenceLabel.Name = "TF_ActionSequenceLabel";
            this.TF_ActionSequenceLabel.Size = new System.Drawing.Size(89, 13);
            this.TF_ActionSequenceLabel.TabIndex = 10;
            this.TF_ActionSequenceLabel.Text = "Action Sequence";
            // 
            // TF_AddActionButton
            // 
            this.TF_AddActionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TF_AddActionButton.Location = new System.Drawing.Point(202, 82);
            this.TF_AddActionButton.Name = "TF_AddActionButton";
            this.TF_AddActionButton.Size = new System.Drawing.Size(74, 23);
            this.TF_AddActionButton.TabIndex = 11;
            this.TF_AddActionButton.Text = "Add Action";
            this.TF_AddActionButton.UseVisualStyleBackColor = true;
            this.TF_AddActionButton.Click += new System.EventHandler(this.TF_AddActionButton_Click);
            // 
            // TriggerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 121);
            this.Controls.Add(this.TF_AddActionButton);
            this.Controls.Add(this.TF_ActionSequenceLabel);
            this.Controls.Add(this.TF_ActionSequenceLB);
            this.Controls.Add(this.TF_ActionPanel);
            this.Controls.Add(this.TF_TriggerPanel);
            this.Controls.Add(this.TF_ActionTypeCB);
            this.Controls.Add(this.TF_CancelButton);
            this.Controls.Add(this.TF_OKButton);
            this.Controls.Add(this.TF_ApplyButton);
            this.Controls.Add(this.TF_TriggerTypeCB);
            this.Controls.Add(this.TF_TriggerTypeLabel);
            this.Controls.Add(this.TF_ActionTypeLabel);
            this.Name = "TriggerForm";
            this.Text = "Trigger Form";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox TF_TriggerTypeCB;
        private System.Windows.Forms.Button TF_ApplyButton;
        private System.Windows.Forms.Button TF_OKButton;
        private System.Windows.Forms.Button TF_CancelButton;
        private System.Windows.Forms.ComboBox TF_ActionTypeCB;
        private System.Windows.Forms.Label TF_TriggerTypeLabel;
        private System.Windows.Forms.Label TF_ActionTypeLabel;
        private System.Windows.Forms.Panel TF_TriggerPanel;
        private System.Windows.Forms.Panel TF_ActionPanel;
        private System.Windows.Forms.ListBox TF_ActionSequenceLB;
        private System.Windows.Forms.Label TF_ActionSequenceLabel;
        private System.Windows.Forms.Button TF_AddActionButton;
    }
}