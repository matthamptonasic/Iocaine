using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Memory;

namespace Iocaine2.Data.Entry
{
    public class IocaineTextbox : TextBox
    {
        #region Private Members
        private string m_defaultText = "Enter Value";
        private bool m_grayTextIfDefault = true;
        private bool m_onEnterSetTarget = false;
        private bool m_onEnterFireEvent = false;
        #endregion Private Members

        #region Public Properties
        public string DefaultText
        {
            get
            {
                return m_defaultText;
            }
            set
            {
                m_defaultText = value;
                Text = m_defaultText;
            }
        }
        public bool GrayTextIfDefault
        {
            get
            {
                return m_grayTextIfDefault;
            }
            set
            {
                m_grayTextIfDefault = value;
            }
        }
        public bool OnEnterSetTarget
        {
            get
            {
                return m_onEnterSetTarget;
            }
            set
            {
                m_onEnterSetTarget = value;
                checkAndSetTarget();
            }
        }
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
        public IocaineTextbox()
        {
            Click += IocaineTextbox_Click;
            Enter += IocaineTextbox_Enter;
            Leave += IocaineTextbox_Leave;
            KeyPress += IocaineTextbox_KeyPress;
            TextChanged += IocaineTextbox_TextChanged;
        }
        #endregion Constructor

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        private void IocaineTextbox_Click(object sender, EventArgs e)
        {
            if (Text == m_defaultText)
            {
                SelectAll();
            }
        }
        private void IocaineTextbox_Enter(object sender, EventArgs e)
        {
            if (Text == m_defaultText)
            {
                SelectAll();
                //ForeColor = Color.Black;
            }
        }
        private void IocaineTextbox_Leave(object sender, EventArgs e)
        {
            if ((Text == "") || (Text == m_defaultText))
            {
                Text = m_defaultText;
                //ForeColor = Color.Gray;
            }
        }
        private void IocaineTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                checkAndSetTarget();
                e.Handled = true;
            }
        }
        private void IocaineTextbox_TextChanged(object sender, EventArgs e)
        {
            if (Text == m_defaultText)
            {
                ForeColor = Color.Gray;
            }
            else
            {
                ForeColor = Color.Black;
            }
        }
        private void checkAndSetTarget()
        {
            if (m_onEnterSetTarget && ChangeMonitor.LoggedIn)
            {
                string target = MemReads.Target.get_name();
                if (target != "")
                {
                    Text = target;
                    ForeColor = Color.Black;
                    if (_DataEntered != null)
                    {
                        _DataEntered();
                    }
                }
            }
            else if (m_onEnterFireEvent)
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
