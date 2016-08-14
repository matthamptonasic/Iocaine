using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Iocaine2.Data.Entry
{
    public abstract class ControlParameter
    {
        #region Private Members
        private string m_labelText;
        private Control m_control;
        #endregion Private Members

        #region Public Properties
        public string LabelText
        {
            get
            {
                return m_labelText;
            }
        }
        public Control Control
        {
            get
            {
                return m_control;
            }
            set
            {
                m_control = value;
            }
        }
        #endregion Public Properties

        #region Constructor
        public ControlParameter(string iLabelText)
        {
            m_labelText = iLabelText;
        }
        #endregion Constructor

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
