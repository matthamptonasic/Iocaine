using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Entry
{
    public class ComboBoxReturn : ControlReturn
    {
        #region Private Members
        private string m_value;
        #endregion Private Members

        #region Public Properties
        public string Value
        {
            get
            {
                return m_value;
            }
        }
        #endregion Public Properties

        #region Constructor
        public ComboBoxReturn(string iValue)
        {
            m_value = iValue;
        }
        #endregion Constructor
    }
}
