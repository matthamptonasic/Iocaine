using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Entry
{
    public class ComboBoxParameter<T> : ControlParameter
    {
        #region Private Members
        private List<T> m_items;
        #endregion Private Members

        #region Public Properties
        public List<T> Items
        {
            get
            {
                return m_items;
            }
        }
        #endregion Public Properties

        #region Constructor
        public ComboBoxParameter(string iLabelText, List<T> iItems)
            : base(iLabelText)
        {
            m_items = iItems;
        }
        #endregion Constructor

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
