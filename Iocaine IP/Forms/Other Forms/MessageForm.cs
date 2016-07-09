using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace Iocaine2.Tools
{
    public partial class MessageForm : Form
    {
        public MessageForm(FORM_MODE mode, String text, double blinkPeriod, int xPos, int yPos)
        {
            InitializeComponent();
            this.Location = new Point(xPos, yPos);
            blinkTimer = new System.Timers.Timer(blinkPeriod);
            this.WarningMessageLabel.Text = text;
            resizeText();

            if (mode == FORM_MODE.BLINK_MSG)
            {
                blinkTimer.Elapsed += new ElapsedEventHandler(blinkTimer_Elapsed);
                blinkTimer.AutoReset = true;
                blinkTimer.Start();
            }
        }

        #region Private Members
        private System.Timers.Timer blinkTimer;
        #endregion Private Members
        #region Public Members
        public enum FORM_MODE
        {
            BLINK_MSG = 0,
            DISPLAY_MSG = 1
        }
        #endregion Public Members
        #region Utilities
        private void resizeText()
        {
            int textWidth = this.WarningMessageLabel.Size.Width;
            int formWidth = this.Size.Width;
            int margin = (formWidth - textWidth) / 2;
            this.WarningMessageLabel.Location = new Point(margin, this.WarningMessageLabel.Location.Y);
        }
        #endregion Utilities
        #region Events
        private void OKButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        void blinkTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Color tempColor = this.WarningMessageLabel.ForeColor;
            this.WarningMessageLabel.ForeColor = this.BackColor;
            this.OKButton.ForeColor = this.BackColor;
            this.BackColor = tempColor;
        }
        #endregion Events
    }
}
