using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Entry
{
    public class ComboBoxReturn<T> : ControlReturn
    {
        #region Private Members
        private T m_value;
        #endregion Private Members

        #region Public Properties
        public T Value
        {
            get
            {
                return m_value;
            }
        }
        #endregion Public Properties

        #region Constructor
        public ComboBoxReturn(T iValue)
        {
            m_value = iValue;
        }
        #endregion Constructor
    }
}
