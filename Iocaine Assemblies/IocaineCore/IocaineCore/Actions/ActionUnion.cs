using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Data.Structures
{
    public class ActionUnion
    {
        #region Private Members
        private List<Action> m_actionList;
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Constructors
        public ActionUnion()
        {
            m_actionList = new List<Action>();
        }
        public ActionUnion(List<Action> iActions)
        {
            m_actionList = iActions;
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
                if (actn.ConditionsMet())
                {
                    oActn = actn;
                    return true;
                }
            }
            oActn = null;
            return false;
        }
        #endregion Public Methods
    }
}