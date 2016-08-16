using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Data.Structures
{
    public class ActionSequence : IExecutableAction
    {
        #region Private Members
        private bool m_isBlocking;
        private List<IExecutableAction> m_actionList;
        private string m_sequenceName;
        private string m_defaultTarget = "<me>";
        #endregion Private Members

        #region Public Properties
        public bool IsBlocking
        {
            get
            {
                return m_isBlocking;
            }
        }
        public string SequenceName
        {
            get
            {
                return m_sequenceName;
            }
        }
        public string DefaultTarget
        {
            get
            {
                return m_defaultTarget;
            }
            set
            {
                m_defaultTarget = value;
            }
        }
        #endregion Public Properties

        #region Constructor(s)
        public ActionSequence(string iSequenceName = "")
        {
            m_isBlocking = false;
            m_actionList = new List<IExecutableAction>();
            m_sequenceName = iSequenceName;
        }
        #endregion Constructor(s)

        #region Public Methods
        #region List Manipulation
        public void AddAction(IExecutableAction iAction)
        {
            if (iAction != null)
            {
                m_actionList.Add(iAction);
                if (iAction.IsBlocking)
                {
                    m_isBlocking = true;
                }
            }
        }
        public void RemoveAction(IExecutableAction iAction)
        {
            if (m_actionList.Contains(iAction))
            {
                m_actionList.Remove(iAction);
                m_isBlocking = false;
                foreach (Data.Structures.Action actn in m_actionList)
                {
                    if (actn.IsBlocking)
                    {
                        m_isBlocking = true;
                        break;
                    }
                }
            }
            else
            {
                LoggingFunctions.Error("Tried to remove action " + iAction.ToString() + " which was not in the actionList.");
            }
        }
        public void RemoveAction(int iIndex)
        {
            IExecutableAction actn = null;
            if (m_actionList.Count >= iIndex + 1)
            {
                actn = m_actionList[iIndex];
                RemoveAction(actn);
            }
            else
            {
                LoggingFunctions.Error("Tried to remove action index " + iIndex + " which was out of range. Count is: " + m_actionList.Count);
            }
        }
        #endregion List Manipulation
        #region List Checking
        public IExecutableAction GetAction(int index)
        {
            return m_actionList[index];
        }
        public void ShowActions()
        {
            int nbActions = m_actionList.Count;
            string display = "";
            for (int ii = 0; ii < nbActions; ii++)
            {
                display += "[" + ii + "] " + m_actionList[ii].ToString() + "\n";
            }
            MessageBox.Show(display, "Action Sequence:", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion List Checking
        #region Duplication
        #endregion Duplication
        #region IExecutableAction
        public bool IsCapable()
        {
            if (!ChangeMonitor.LoggedIn)
            {
                return false;
            }
            foreach (IExecutableAction actn in m_actionList)
            {
                if (!actn.IsCapable())
                {
                    return false;
                }
            }
            return true;
        }
        public bool CanPerform()
        {
            if (!ChangeMonitor.LoggedIn)
            {
                return false;
            }
            foreach (IExecutableAction actn in m_actionList)
            {
                if (!actn.CanPerform())
                {
                    return false;
                }
            }
            return true;
        }
        public bool Execute(string iTarget = "")
        {
            if (!ChangeMonitor.LoggedIn)
            {
                return false;
            }
            bool retVal = true;
            string target = (iTarget == "") ? m_defaultTarget : iTarget;
            foreach (IExecutableAction actn in m_actionList)
            {
                retVal &= actn.Execute(target);
            }
            return retVal;
        }
        #endregion IExecutableAction
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
