using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Iocaine2.Data.Entry
{
    public partial class DataEntry : Form
    {
        #region Private Members
        private List<ControlParameter> m_controlParams;
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Constructor
        public DataEntry(List<ControlParameter> iControlParams)
        {
            m_controlParams = iControlParams;
            InitializeComponent();
        }
        #endregion Constructor

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
