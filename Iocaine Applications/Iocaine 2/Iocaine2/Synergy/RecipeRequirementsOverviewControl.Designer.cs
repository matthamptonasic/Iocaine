namespace Iocaine2.Synergy
{
    partial class RecipeRequirementsOverviewControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.RecipeResultLabel = new System.Windows.Forms.Label();
            this.RankLabel = new System.Windows.Forms.Label();
            this.elementalOverviewControl = new Iocaine2.Synergy.ElementalOverviewControl();
            this.SuspendLayout();
            // 
            // RecipeResultLabel
            // 
            this.RecipeResultLabel.AutoSize = true;
            this.RecipeResultLabel.Location = new System.Drawing.Point(3, 24);
            this.RecipeResultLabel.Name = "RecipeResultLabel";
            this.RecipeResultLabel.Size = new System.Drawing.Size(16, 13);
            this.RecipeResultLabel.TabIndex = 1;
            this.RecipeResultLabel.Text = "---";
            // 
            // RankLabel
            // 
            this.RankLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RankLabel.Location = new System.Drawing.Point(207, 24);
            this.RankLabel.Name = "RankLabel";
            this.RankLabel.Size = new System.Drawing.Size(127, 13);
            this.RankLabel.TabIndex = 2;
            this.RankLabel.Text = "---";
            this.RankLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // elementalOverviewControl
            // 
            this.elementalOverviewControl.Fewell_Max = 99;
            this.elementalOverviewControl.Fewell_Min = -99;
            this.elementalOverviewControl.Location = new System.Drawing.Point(3, 3);
            this.elementalOverviewControl.MaximumSize = new System.Drawing.Size(331, 18);
            this.elementalOverviewControl.MinimumSize = new System.Drawing.Size(331, 18);
            this.elementalOverviewControl.Name = "elementalOverviewControl";
            this.elementalOverviewControl.Size = new System.Drawing.Size(331, 18);
            this.elementalOverviewControl.TabIndex = 0;
            // 
            // RecipeRequirementsOverviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RankLabel);
            this.Controls.Add(this.RecipeResultLabel);
            this.Controls.Add(this.elementalOverviewControl);
            this.Name = "RecipeRequirementsOverviewControl";
            this.Size = new System.Drawing.Size(340, 40);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ElementalOverviewControl elementalOverviewControl;
        private System.Windows.Forms.Label RecipeResultLabel;
        private System.Windows.Forms.Label RankLabel;
    }
}
