using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Memory;

namespace Iocaine2.Data.Entry
{
    public class IocaineUpDown : NumericUpDown
    {
        #region Private Members
        private bool m_onEnterFireEvent = false;
        private bool m_keydownFromForm = false;
        #endregion Private Members

        #region Public Properties
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
        public IocaineUpDown()
        {
            Click += IocaineUpDown_Click;
            Enter += IocaineUpDown_Enter;
            KeyDown += IocaineUpDown_KeyDown;
            KeyUp += IocaineUpDown_KeyUp;
            TextAlign = HorizontalAlignment.Center;
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
        private void IocaineUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                m_keydownFromForm = true;
            }
        }
        private void IocaineUpDown_KeyUp(object sender, KeyEventArgs e)
        {
            if (!m_keydownFromForm)
            {
                return;
            }
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
            {
                fireDataEnteredEvent();
                e.SuppressKeyPress = true;
                e.Handled = true;
                m_keydownFromForm = false;
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
