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
        private List<Action> m_actionList;
        #endregion Private Members

        #region Public Properties
        public bool IsBlocking
        {
            get
            {
                return m_isBlocking;
            }
        }
        public int ActionCount
        {
            get
            {
                return m_actionList.Count;
            }
        }
        #endregion Public Properties

        #region Constructor(s)
        public ActionSequence()
        {
            m_isBlocking = false;
            m_actionList = new List<Action>();
        }
        #endregion Constructor(s)

        #region Public Methods
        #region List Manipulation
        public void AddAction(Action iAction)
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
        public void RemoveAction(Data.Structures.Action iAction)
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
                LoggingFunctions.Error("Tried to remove action " + iAction.AType.ToString() + " which was not in the actionList.");
            }
        }
        public void RemoveAction(int iIndex)
        {
            Data.Structures.Action actn = null;
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
        public Action GetAction(int index)
        {
            return m_actionList[index];
        }
        public bool Contains(ACTN_TYPE iType)
        {
            foreach (Data.Structures.Action actn in m_actionList)
            {
                if (actn.AType == iType)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Compare(ActionSequence seq)
        {
            if (seq.ActionCount != this.ActionCount)
            {
                return false;
            }
            else
            {
                for (int ii = 0; ii < this.ActionCount; ii++)
                {
                    if (!this.m_actionList[ii].Compare(seq.GetAction(ii)))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public void ShowActions()
        {
            int nbActions = m_actionList.Count;
            String display = "";
            for (int ii = 0; ii < nbActions; ii++)
            {
                display += "[" + ii + "] " + m_actionList[ii].ToString() + "\n";
            }
            MessageBox.Show(display, "Action Sequence:", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion List Checking
        #region Duplication
        public Action GetActionCopy(int iIndex)
        {
            if (iIndex > m_actionList.Count)
            {
                return null;
            }
            else
            {
                Action actn = m_actionList[iIndex];
                Action copyAction = null;
                switch (actn.AType)
                {
                    case ACTN_TYPE.Wait:
                        copyAction = new ActionWait((ActionWait)actn);
                        return copyAction;
                    default:
                        return null;
                }
            }
        }
        public ActionSequence GetSequenceCopy()
        {
            ActionSequence copySeq = new ActionSequence();
            copySeq.m_isBlocking = this.m_isBlocking;
            int nbActions = this.m_actionList.Count;
            for (int ii = 0; ii < nbActions; ii++)
            {
                copySeq.m_actionList.Add(this.GetActionCopy(ii));
            }
            return copySeq;
        }
        #endregion Duplication
        #region IExecutableAction
        public bool IsCapable()
        {
            foreach (Action actn in m_actionList)
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
            foreach (Action actn in m_actionList)
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
            bool retVal = true;
            foreach (Action actn in m_actionList)
            {
                retVal &= actn.Execute(iTarget);
            }
            return retVal;
        }
        #endregion IExecutableAction
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
