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
    public abstract partial class Action
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