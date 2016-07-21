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
        private uint waitTime = 0;
        #endregion Private Members
        
        #region Public Properties
        public uint WaitTime
        {
            get
            {
                return waitTime;
            }
        }
        #endregion Public Properties

        #region Constructors
        public ActionWait(uint iWaitTimeMs = 1000)
            : base(true, ACTN_TYPE.Wait)
        {
            waitTime = iWaitTimeMs;
        }
        public ActionWait(ActionWait iAction)
            : base(true, ACTN_TYPE.Wait)
        {
            waitTime = iAction.waitTime;
        }
        #endregion Constructors
        
        #region Public Methods
        override public void Show()
        {
            MessageBox.Show(this.ToString(), "ActionWait::" + waitTime, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override string SaveString()
        {
            return "ActionWait;" + waitTime;
        }
        public override string ToString()
        {
            return waitTime.ToString();
        }
        #endregion Methods
    }
}