namespace Iocaine2.Tools
{
    partial class MessageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageForm));
            this.OKButton = new System.Windows.Forms.Button();
            this.WarningMessageLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OKButton.Location = new System.Drawing.Point(104, 86);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 0;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // WarningMessageLabel
            // 
            this.WarningMessageLabel.AutoSize = true;
            this.WarningMessageLabel.Font = new System.Drawing.Font("Arial Black", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WarningMessageLabel.Location = new System.Drawing.Point(26, 35);
            this.WarningMessageLabel.Name = "WarningMessageLabel";
            this.WarningMessageLabel.Size = new System.Drawing.Size(230, 30);
            this.WarningMessageLabel.TabIndex = 1;
            this.WarningMessageLabel.Text = "Warning Message";
            this.WarningMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.WarningMessageLabel.UseMnemonic = false;
            // 
            // MessageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightCoral;
            this.CancelButton = this.OKButton;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(282, 121);
            this.Controls.Add(this.WarningMessageLabel);
            this.Controls.Add(this.OKButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MessageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Attention!";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Label WarningMessageLabel;
    }
}