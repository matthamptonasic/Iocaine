using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Data.Structures
{
    public class ActionUnion : Action
    {
        #region Private Members
        private List<Action> m_actionList;
        private Action m_setAction = null;
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Constructors
        public ActionUnion(bool iIsBlocking, Action iAction)
            : base(iIsBlocking, ACTN_TYPE.Union, "", "") // TBD - fix how union conditions are set/updated.
        {
            m_actionList = new List<Action>() { iAction };
        }
        #endregion Constructors

        #region Public Methods
        public void Add(Action iAction)
        {
            foreach (Action actn in m_actionList)
            {
                if (actn.Compare(iAction))
                {
                    // Don't add duplicate actions.
                    return;
                }
            }
            m_actionList.Add(iAction);
        }
        public bool CanPerform(out Action oActn)
        {
            foreach (Action actn in m_actionList)
            {
                if (actn.CanPerform())
                {
                    oActn = actn;
                    m_setAction = actn;
                    return true;
                }
            }
            oActn = null;
            return false;
        }
        public override bool Execute(string iTarget = "")
        {
            if (CanPerform(out m_setAction))
            {
                return m_setAction.Execute(iTarget);
            }
            else
            {
                return false;
            }
        }
        public override void Show()
        {
            string showStr = "";
            foreach (Action actn in m_actionList)
            {
                showStr += actn.ToString() + "\n";
            }
            MessageBox.Show(showStr, "ActionUnion", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override string SaveString()
        {
            // TBD
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            string str = "";
            foreach (Action actn in m_actionList)
            {
                str += actn.ToString() + "\n";
            }
            return str;
        }
        #endregion Public Methods
    }
}