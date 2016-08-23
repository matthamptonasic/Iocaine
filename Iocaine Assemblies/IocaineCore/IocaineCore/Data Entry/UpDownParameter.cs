using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Entry
{
    public class UpDownParameter : ControlParameter
    {
        #region Private Members
        private decimal m_defaultValue;
        private int m_nbDecimals;
        private decimal m_min;
        private decimal m_max;
        private decimal m_increment;
        private bool m_saveOnEnter;
        #endregion Private Members

        #region Public Properties
        public decimal DefaultValue
        {
            get
            {
                return m_defaultValue;
            }
        }
        public int NbDecimals
        {
            get
            {
                return m_nbDecimals;
            }
        }
        public decimal Min
        {
            get
            {
                return m_min;
            }
        }
        public decimal Max
        {
            get
            {
                return m_max;
            }
        }
        public decimal Increment
        {
            get
            {
                return m_increment;
            }
        }
        public bool SaveOnEnter
        {
            get
            {
                return m_saveOnEnter;
            }
        }
        #endregion Public Properties

        #region Constructor
        public UpDownParameter(string iLabelText, decimal iDefaultValue, int iNbDecimals, decimal iMin, decimal iMax, decimal iIncrement, bool iSaveOnEnter = true)
            : base(iLabelText)
        {
            m_defaultValue = iDefaultValue;
            m_nbDecimals = iNbDecimals;
            m_min = iMin;
            m_max = iMax;
            m_increment = iIncrement;
            m_saveOnEnter = iSaveOnEnter;
        }
        #endregion Constructor

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
