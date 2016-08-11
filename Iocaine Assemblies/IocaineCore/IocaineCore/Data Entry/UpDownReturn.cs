using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Entry
{
    public class UpDownReturn : ControlReturn
    {
        #region Private Members
        private decimal m_value;
        #endregion Private Members

        #region Public Properties
        public decimal Value
        {
            get
            {
                return m_value;
            }
        }
        #endregion Public Properties

        #region Constructor
        public UpDownReturn(decimal iValue)
        {
            m_value = iValue;
        }
        #endregion Constructor
    }
}
