using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Data.Structures
{
    public class ActionSequence
    {
        public ActionSequence()
        {
            isBlocking = false;
            actionList = new List<Data.Structures.Action>();
        }
        private bool isBlocking;
        private List<Data.Structures.Action> actionList;
        public bool IsBlocking
        {
            get
            {
                return isBlocking;
            }
        }
        public int ActionCount
        {
            get
            {
                return actionList.Count;
            }
        }
        public Data.Structures.Action GetAction(int index)
        {
            return actionList[index];
        }
        public void AddAction(Data.Structures.Action iAction)
        {
            if (iAction != null)
            {
                actionList.Add(iAction);
                if (iAction.IsBlocking)
                {
                    isBlocking = true;
                }
            }
        }
        public void RemoveAction(Data.Structures.Action iAction)
        {
            if (actionList.Contains(iAction))
            {
                actionList.Remove(iAction);
                isBlocking = false;
                foreach (Data.Structures.Action actn in actionList)
                {
                    if (actn.IsBlocking)
                    {
                        isBlocking = true;
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
            if (actionList.Count >= iIndex + 1)
            {
                actn = actionList[iIndex];
                RemoveAction(actn);
            }
            else
            {
                LoggingFunctions.Error("Tried to remove action index " + iIndex + " which was out of range. Count is: " + actionList.Count);
            }
        }
        public void ShowActions()
        {
            int nbActions = actionList.Count;
            String display = "";
            for (int ii = 0; ii < nbActions; ii++)
            {
                display += "[" + ii + "] " + actionList[ii].ToString() + "\n";
            }
            MessageBox.Show(display, "Action Sequence:", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public bool Contains(ACTN_TYPE iType)
        {
            foreach (Data.Structures.Action actn in actionList)
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
                    if (!this.actionList[ii].Compare(seq.GetAction(ii)))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public Data.Structures.Action GetActionCopy(int iIndex)
        {
            if (iIndex > actionList.Count)
            {
                return null;
            }
            else
            {
                Data.Structures.Action actn = actionList[iIndex];
                Data.Structures.Action copyAction = null;
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
            copySeq.isBlocking = this.isBlocking;
            int nbActions = this.actionList.Count;
            for (int ii = 0; ii < nbActions; ii++)
            {
                copySeq.actionList.Add(this.GetActionCopy(ii));
            }
            return copySeq;
        }
    }
}