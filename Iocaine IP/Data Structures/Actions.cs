using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Data.Structures
{
    #region Enums
    public enum ACTN_TYPE
    {
        Unknown = 0,
        Command = 1,
        Wait = 2,
        Target = 3,
        Follow = 4,
        Trade_Npc_Item = 5,
        Trade_Npc_Gil = 6,
        Trade_Pc_Item = 7,
        Trade_Pc_Gil = 8,
        Keystroke = 9,
        ArrowKeystroke = 10,
        Count = 11
    }
    #endregion Enums
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
                LoggingFunctions.Error("Tried to remove action " + iAction.Type.ToString() + " which was not in the actionList.");
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
                if (actn.Type == iType)
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
                switch (actn.Type)
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
    public abstract class Action
    {
        #region Constructor
        public Action(bool iIsBlocking, ACTN_TYPE iType)
        {
            isBlocking = iIsBlocking;
            type = iType;
        }
        #endregion Constructor
        #region Members
        private bool isBlocking = false;
        private ACTN_TYPE type = ACTN_TYPE.Unknown;
        private List<Condition> m_orConditions;
        private List<Condition> m_andConditions;
        #endregion Members
        #region Properties
        public bool IsBlocking
        {
            get
            {
                return isBlocking;
            }
        }
        public ACTN_TYPE Type
        {
            get
            {
                return type;
            }
        }
        #endregion Properties
        #region Functions
        public abstract void Show();
        public abstract new String SaveString();
        public bool Compare(Data.Structures.Action iAction)
        {
            if(iAction.type != this.type)
            {
                return false;
            }
            if(iAction.ToString() != this.ToString())
            {
                return false;
            }
            return true;
        }
        #endregion Functions

        public class Condition
        {
            public enum ConditionType : uint
            {
                JOB_LVL,
                INV_ITEM,
                KEY_ITEM
            }

            public class JobLevel
            {
                #region Private Members
                private Client.Jobs.JOBS_INFO m_job;
                private byte m_levelMin = 1;
                private byte m_levelMax = 99;
                #endregion Private Members

                #region Public Properties
                #endregion Public Properties

                #region Constructor(s)
                public JobLevel(Client.Jobs.JOBS_INFO iInfo, byte iLevelMin = 1, byte iLevelMax = 99)
                {
                    m_job = iInfo;
                    m_levelMin = iLevelMin;
                    m_levelMax = iLevelMax;
                }
                #endregion Constructor(s)
            }
        }
    }
    
    public class ActionWait : Data.Structures.Action
    {
        #region Constructors
        public ActionWait(UInt32 iWaitTimeMs = 1000)
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
        #region Members
        private UInt32 waitTime = 0;
        #endregion Members
        #region Properties
        public UInt32 WaitTime
        {
            get
            {
                return waitTime;
            }
        }
        #endregion Properties
        #region Methods
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