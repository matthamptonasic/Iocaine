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
    public class IocaineComboBox : ComboBox
    {
        #region Private Members
        private bool m_onEnterFireEvent = false;
        #endregion Private Members

        #region Public Properties
        [ 
            Category("Behavior"), 
            Description("Determines whether an 'Entry Complete' event is fired upon the user pressing the Enter or Return keys.")
        ]
        public bool OnEnterFireEvent
        {
            get
            {
                return m_onEnterFireEvent;
            }
            set
            {
                m_onEnterFireEvent = value;
            }
        }
        #endregion Public Properties

        #region Events
        public event Statics.FuncPtrs.TD_Void_Void _DataEntered;
        #endregion Events

        #region Constructor
        public IocaineComboBox()
        {
            Click += IocaineUpDown_Click;
            Enter += IocaineUpDown_Enter;
            KeyPress += IocaineUpDown_KeyPress;
        }
        #endregion Constructor

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        private void IocaineUpDown_Click(object sender, EventArgs e)
        {
            this.Select(0, this.Text.Length - 1);
        }
        private void IocaineUpDown_Enter(object sender, EventArgs e)
        {
            this.Select(0, this.Text.Length);
        }
        private void IocaineUpDown_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                fireDataEnteredEvent();
                e.Handled = true;
            }
        }
        private void fireDataEnteredEvent()
        {
            if (m_onEnterFireEvent)
            {
                if (_DataEntered != null)
                {
                    _DataEntered();
                }
            }
        }
        #endregion Private Methods
    }
}
