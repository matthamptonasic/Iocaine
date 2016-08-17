using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Entry
{
    public class ComboBoxParameter : ControlParameter
    {
        #region Private Members
        private List<string> m_items;
        private int m_initialIndex;
        private bool m_saveOnEnter;
        #endregion Private Members

        #region Public Properties
        public List<string> Items
        {
            get
            {
                return m_items;
            }
        }
        public int InitialIndex
        {
            get
            {
                return m_initialIndex;
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
        public ComboBoxParameter(string iLabelText, List<string> iItems, int iInitialIndex = -1, bool iSaveOnEnter = true)
            : base(iLabelText)
        {
            m_items = iItems;
            m_initialIndex = iInitialIndex;
            m_saveOnEnter = iSaveOnEnter;
        }
        #endregion Constructor

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
