using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;
using System.Windows.Forms;

using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;

namespace Iocaine2.Data.Structures
{
    public class UseItem : Command
    {
        #region Private Members
        private string m_itemName = "";
        private string m_target = "<t>";
        private uint m_execTime = 1000;
        #endregion Private Members

        #region Public Properties
        public string ItemName
        {
            get
            {
                return m_itemName;
            }
        }
        public string Target
        {
            get
            {
                return m_target;
            }
            set
            {
                m_target = value;
            }
        }
        public override bool Ready
        {
            get
            {
                return CanPerform();
            }
        }
        public override uint ExecTime
        {
            get
            {
                return m_execTime;
            }
        }
        #endregion Public Properties

        #region Constructors
        public UseItem(string iItemName, string iTarget, uint iCastTime)
            : base(iItemName, CMD_TYPE.USE_ITEM, true)
        {
            m_itemName = iItemName;
            m_target = iTarget;
            m_execTime = iCastTime;
            setConditionTrees();
        }
        #endregion Constructors

        #region Public Methods
        public override bool Execute(string iTarget = "")
        {
            if (MemReads.Self.Casting.is_casting())
            {
                return false;
            }
            try
            {
                if (CanPerform())
                {
                    string text = "/item \"" + m_itemName + "\" " + (iTarget == "" ? m_target : iTarget);
                    LoggingFunctions.Debug(text, LoggingFunctions.DBG_SCOPE.COMMANDS);
                    IocaineFunctions.keys(text);
                    IocaineFunctions.delay(m_execTime);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error(e.ToString());
                return false;
            }
            return true;
        }

        public override void Show()
        {
            MessageBox.Show(this.ToString(), "SpellCommand::" + Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override string SaveString()
        {
            return "UseItem;" + m_itemName + ";" + m_target + ";" + m_execTime;
        }
        public override string ToString()
        {
            return m_itemName;
        }
        #endregion Public Methods

        #region Private Methods
        private void setConditionTrees()
        {
            ConditionTree treeDynamic = new ConditionTree();
            try
            {
                treeDynamic.PushAnd(new InventoryItem(m_itemName, Data.Client.Things.GetIdFromName(m_itemName)));
                setConditions(null, treeDynamic);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error(e.ToString());
            }
        }
        #endregion Private Methods
    }
}
