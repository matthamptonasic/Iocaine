using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Entry
{
    public class TextboxParameter : ControlParameter
    {
        #region Private Members
        private string m_defaultText;
        private bool m_textGreyed;
        #endregion Private Members

        #region Public Properties
        public string DefaultText
        {
            get
            {
                return m_defaultText;
            }
        }
        public bool TextGreyed
        {
            get
            {
                return m_textGreyed;
            }
        }
        #endregion Public Properties

        #region Constructor
        public TextboxParameter(string iLabelText, string iDefaultText, bool iTextGreyed = true)
            : base(iLabelText)
        {
            m_defaultText = iDefaultText;
            m_textGreyed = iTextGreyed;
        }
        #endregion Constructor

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
