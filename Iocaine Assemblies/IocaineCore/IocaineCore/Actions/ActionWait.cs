using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Data.Structures
{
    public class ActionWait : Data.Structures.Action
    {
        #region Private Members
        private uint m_waitTime = 0;
        #endregion Private Members
        
        #region Public Properties
        public uint WaitTime
        {
            get
            {
                return m_waitTime;
            }
        }
        #endregion Public Properties

        #region Constructors
        public ActionWait(uint iWaitTimeMs = 1000)
            : base(true, ACTN_TYPE.Wait)
        {
            m_waitTime = iWaitTimeMs;
        }
        public ActionWait(ActionWait iAction)
            : base(true, ACTN_TYPE.Wait)
        {
            m_waitTime = iAction.m_waitTime;
        }
        #endregion Constructors

        #region Public Methods
        public override bool Execute(string iTarget = "")
        {
            if (m_waitTime > 0)
            {
                Memory.Interface.IocaineFunctions.delay(m_waitTime);
            }
            return true;
        }
        public override void Show()
        {
            MessageBox.Show(this.ToString(), "ActionWait::" + m_waitTime, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override string SaveString()
        {
            return "ActionWait;" + m_waitTime;
        }
        public override string ToString()
        {
            return m_waitTime.ToString() + "ms";
        }
        #endregion Public Methods
    }
}