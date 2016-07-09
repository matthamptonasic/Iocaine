namespace Iocaine2.Tools
{
    partial class CommandLine
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommandLine));
            this.CLTextBox = new System.Windows.Forms.TextBox();
            this.CLCloseButton = new System.Windows.Forms.Button();
            this.CLSendButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CLTextBox
            // 
            this.CLTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CLTextBox.Location = new System.Drawing.Point(13, 13);
            this.CLTextBox.Name = "CLTextBox";
            this.CLTextBox.Size = new System.Drawing.Size(307, 20);
            this.CLTextBox.TabIndex = 0;
            this.CLTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CLTextBox_KeyDown);
            this.CLTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CLTextBox_KeyUp);
            // 
            // CLCloseButton
            // 
            this.CLCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CLCloseButton.Location = new System.Drawing.Point(245, 39);
            this.CLCloseButton.Name = "CLCloseButton";
            this.CLCloseButton.Size = new System.Drawing.Size(75, 23);
            this.CLCloseButton.TabIndex = 1;
            this.CLCloseButton.Text = "Close";
            this.CLCloseButton.UseVisualStyleBackColor = true;
            this.CLCloseButton.Click += new System.EventHandler(this.CLCloseButton_Click);
            // 
            // CLSendButton
            // 
            this.CLSendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CLSendButton.Location = new System.Drawing.Point(164, 39);
            this.CLSendButton.Name = "CLSendButton";
            this.CLSendButton.Size = new System.Drawing.Size(75, 23);
            this.CLSendButton.TabIndex = 2;
            this.CLSendButton.Text = "Send";
            this.CLSendButton.UseVisualStyleBackColor = true;
            this.CLSendButton.Click += new System.EventHandler(this.CLSendButton_Click);
            // 
            // CommandLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 73);
            this.Controls.Add(this.CLSendButton);
            this.Controls.Add(this.CLCloseButton);
            this.Controls.Add(this.CLTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CommandLine";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Iocaine Command Entry";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox CLTextBox;
        private System.Windows.Forms.Button CLCloseButton;
        private System.Windows.Forms.Button CLSendButton;
    }
}