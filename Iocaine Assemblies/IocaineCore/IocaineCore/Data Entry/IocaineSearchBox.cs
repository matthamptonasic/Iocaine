using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Memory;

namespace Iocaine2.Data.Entry
{
    public class IocaineSearchBox : IocaineTextbox
    {
        #region Private Members
        RegexPatternState m_regexPattern;
        #endregion Private Members

        #region Public Properties
        [
            Category("Iocaine"),
            Description("Current valid regex pattern (read-only).")
        ]
        public string Pattern
        {
            get
            {
                return m_regexPattern.Pattern;
            }
        }
        #endregion Public Properties

        #region Events
        #endregion Events

        #region Constructor
        public IocaineSearchBox() : base()
        {
            m_regexPattern = new RegexPatternState();
        }
        #endregion Constructor

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        protected override void IocaineTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            base.IocaineTextbox_KeyPress(sender, e);
        }
        protected override void IocaineTextbox_TextChanged(object sender, EventArgs e)
        {
            base.IocaineTextbox_TextChanged(sender, e);
            m_regexPattern.UpdatePattern(Text);
        }
        #endregion Private Methods
    }
}
