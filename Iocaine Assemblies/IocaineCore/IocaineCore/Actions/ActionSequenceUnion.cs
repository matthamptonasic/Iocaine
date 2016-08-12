using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Data.Structures
{
    public class ActionSequenceUnion : IExecutableAction
    {
        #region Private Members
        private bool m_isBlocking;
        private List<ActionSequence> m_sequenceList;
        #endregion Private Members

        #region Public Properties
        public bool IsBlocking
        {
            get
            {
                return m_isBlocking;
            }
        }
        public int SequenceCount
        {
            get
            {
                return m_sequenceList.Count;
            }
        }
        #endregion Public Properties

        #region Constructor(s)
        public ActionSequenceUnion()
        {
            m_isBlocking = false;
            m_sequenceList = new List<ActionSequence>();
        }
        #endregion Constructor(s)

        #region Public Methods
        #region List Manipulation
        public void AddSequence(ActionSequence iSequence)
        {
            if (iSequence != null)
            {
                m_sequenceList.Add(iSequence);
                if (ChangeMonitor.LoggedIn)
                {
                    // Setting the active sequence only makes sense if we're logged in.
                    setSequence();
                }
            }
        }
        public void RemoveAction(ActionSequence iSequence)
        {
            if (m_sequenceList.Contains(iSequence))
            {
                m_sequenceList.Remove(iSequence);
                m_isBlocking = false;
                setSequence();
            }
            else
            {
                LoggingFunctions.Error("Tried to remove sequence " + iSequence.ToString() + " which was not in the sequenceList.");
            }
        }
        public void RemoveAction(int iIndex)
        {
            ActionSequence seq = null;
            if (m_sequenceList.Count >= iIndex + 1)
            {
                seq = m_sequenceList[iIndex];
                RemoveAction(seq);
            }
            else
            {
                LoggingFunctions.Error("Tried to remove action index " + iIndex + " which was out of range. Count is: " + m_sequenceList.Count);
            }
        }
        #endregion List Manipulation
        #region List Checking
        public ActionSequence GetSequence(int index)
        {
            return m_sequenceList[index];
        }
        public ActionSequence GetSetSequence()
        {
            return setSequence();
        }
        #endregion List Checking
        #region IExecutableAction
        public bool IsCapable()
        {
            if (!ChangeMonitor.LoggedIn)
            {
                return false;
            }
            foreach (ActionSequence seq in m_sequenceList)
            {
                if (seq.IsCapable())
                {
                    return true;
                }
            }
            return false;
        }
        public bool CanPerform()
        {
            if (!ChangeMonitor.LoggedIn)
            {
                return false;
            }
            if (setSequence() == null)
            {
                return false;
            }
            return true;
        }
        public bool Execute(string iTarget = "")
        {
            ActionSequence aseq = setSequence();
            if (aseq == null)
            {
                return false;
            }
            return aseq.Execute(iTarget);
        }
        #endregion IExecutableAction
        #endregion Public Methods

        #region Private Methods
        private ActionSequence setSequence()
        {
            m_isBlocking = false;
            foreach (ActionSequence seq in m_sequenceList)
            {
                if (seq.IsCapable() && seq.CanPerform())
                {
                    m_isBlocking = seq.IsBlocking;
                    return seq;
                }
            }
            return null;
        }
        #endregion Private Methods
    }
}
