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
    public class IocaineTextbox : TextBox
    {
        #region Private Members
        private string m_defaultText = "Enter Value";
        private bool m_grayTextIfDefault = true;
        private bool m_onEnterSetTarget = false;
        private bool m_onEnterFireEvent = false;
        private bool m_selectTextOnClick = true;
        #endregion Private Members

        #region Public Properties
        [
            Category("Iocaine"),
            Description("Text compared against when deciding if text will be Gray or Black.")
        ]
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
        [
            Category("Iocaine"),
            Description("Text becomes gray when it is the same as DefaultText.")
        ]
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
        [
            Category("Iocaine"),
            Description("When true, will set the text value to the name of the currently selected PC/NPC in game.")
        ]
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
        [
            Category("Iocaine"),
            Description("Fires the \"DataEntered\" event upon recieving the Enter or Return keys.")
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
        [
            Category("Iocaine"),
            Description("If true, will select all of the text when the textbox is clicked on.")
        ]
        public bool SelectTextOnClick
        {
            get
            {
                return m_selectTextOnClick;
            }
            set
            {
                m_selectTextOnClick = value;
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
        protected virtual void IocaineTextbox_Click(object sender, EventArgs e)
        {
            if ((Text == m_defaultText) && m_selectTextOnClick)
            {
                SelectAll();
            }
        }
        protected virtual void IocaineTextbox_Enter(object sender, EventArgs e)
        {
            if (Text == m_defaultText)
            {
                SelectAll();
                //ForeColor = Color.Black;
            }
        }
        protected virtual void IocaineTextbox_Leave(object sender, EventArgs e)
        {
            if ((Text == "") || (Text == m_defaultText))
            {
                Text = m_defaultText;
                //ForeColor = Color.Gray;
            }
        }
        protected virtual void IocaineTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                checkAndSetTarget();
                e.Handled = true;
            }
        }
        protected virtual void IocaineTextbox_TextChanged(object sender, EventArgs e)
        {
            if ((Text == m_defaultText) && m_grayTextIfDefault)
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
