namespace IocaineUpdater
{
    partial class DisplayBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplayBox));
            this.updaterProgressBar = new System.Windows.Forms.ProgressBar();
            this.updaterTextLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // updaterProgressBar
            // 
            this.updaterProgressBar.ForeColor = System.Drawing.Color.Lime;
            this.updaterProgressBar.Location = new System.Drawing.Point(12, 31);
            this.updaterProgressBar.Name = "updaterProgressBar";
            this.updaterProgressBar.Size = new System.Drawing.Size(276, 23);
            this.updaterProgressBar.TabIndex = 0;
            // 
            // updaterTextLabel
            // 
            this.updaterTextLabel.AutoSize = true;
            this.updaterTextLabel.Location = new System.Drawing.Point(13, 12);
            this.updaterTextLabel.Name = "updaterTextLabel";
            this.updaterTextLabel.Size = new System.Drawing.Size(114, 13);
            this.updaterTextLabel.TabIndex = 1;
            this.updaterTextLabel.Text = "Connecting to server...";
            // 
            // DisplayBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 66);
            this.Controls.Add(this.updaterTextLabel);
            this.Controls.Add(this.updaterProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DisplayBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Iocaine Updater";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar updaterProgressBar;
        private System.Windows.Forms.Label updaterTextLabel;

    }
}