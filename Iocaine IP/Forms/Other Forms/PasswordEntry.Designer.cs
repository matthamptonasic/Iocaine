using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Iocaine2
{
    public static partial class Server
    {
        public partial class PasswordEntry : Form
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
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasswordEntry));
                this.PwdEntry_PasswordTB = new System.Windows.Forms.TextBox();
                this.PwdEntry_OkButton = new System.Windows.Forms.Button();
                this.PwdEntry_CancelButton = new System.Windows.Forms.Button();
                this.SuspendLayout();
                // 
                // PwdEntry_PasswordTB
                // 
                this.PwdEntry_PasswordTB.Location = new System.Drawing.Point(12, 12);
                this.PwdEntry_PasswordTB.Name = "PwdEntry_PasswordTB";
                this.PwdEntry_PasswordTB.Size = new System.Drawing.Size(197, 20);
                this.PwdEntry_PasswordTB.TabIndex = 0;
                this.PwdEntry_PasswordTB.TextChanged += new System.EventHandler(this.PasswordTB_TextChanged);
                // 
                // PwdEntry_OkButton
                // 
                this.PwdEntry_OkButton.Location = new System.Drawing.Point(113, 38);
                this.PwdEntry_OkButton.Name = "PwdEntry_OkButton";
                this.PwdEntry_OkButton.Size = new System.Drawing.Size(75, 23);
                this.PwdEntry_OkButton.TabIndex = 2;
                this.PwdEntry_OkButton.Text = "OK";
                this.PwdEntry_OkButton.UseVisualStyleBackColor = true;
                this.PwdEntry_OkButton.Click += new System.EventHandler(this.OkButton_Click);
                // 
                // PwdEntry_CancelButton
                // 
                this.PwdEntry_CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.PwdEntry_CancelButton.Location = new System.Drawing.Point(32, 38);
                this.PwdEntry_CancelButton.Name = "PwdEntry_CancelButton";
                this.PwdEntry_CancelButton.Size = new System.Drawing.Size(75, 23);
                this.PwdEntry_CancelButton.TabIndex = 3;
                this.PwdEntry_CancelButton.Text = "Cancel";
                this.PwdEntry_CancelButton.UseVisualStyleBackColor = true;
                this.PwdEntry_CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
                // 
                // PasswordEntry
                // 
                this.AcceptButton = this.PwdEntry_OkButton;
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.CancelButton = this.PwdEntry_CancelButton;
                this.ClientSize = new System.Drawing.Size(221, 69);
                this.Controls.Add(this.PwdEntry_CancelButton);
                this.Controls.Add(this.PwdEntry_OkButton);
                this.Controls.Add(this.PwdEntry_PasswordTB);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
                this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "PasswordEntry";
                this.ShowInTaskbar = false;
                this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                this.Text = "Password Entry";
                this.TopMost = true;
                this.ResumeLayout(false);
                this.PerformLayout();

            }

            #endregion

            private System.Windows.Forms.TextBox PwdEntry_PasswordTB;
            private System.Windows.Forms.Button PwdEntry_OkButton;
            private System.Windows.Forms.Button PwdEntry_CancelButton;
        }
    }
}