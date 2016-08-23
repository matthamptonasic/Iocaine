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
    public class RawCommand : Command
    {
        #region Private Members
        private String text = "";
        #endregion Private Members

        #region Public Properties
        public String Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }
        public override Boolean Ready
        {
            get
            {
                return true;
            }
        }
        #endregion Public Properties

        #region Constructors
        public RawCommand(string iName = "", string iText = "", bool iSendNow = false)
            : base(iName, CMD_TYPE.RAW_CMD, false)
        {
            setConditions(null, null);
            init(iText, iSendNow);
        }
        public RawCommand(ConditionTree iStaticConditions, ConditionTree iDynamicConditions, string iName = "", string iText = "", bool iSendNow = false)
            : base(iName, CMD_TYPE.RAW_CMD, false)
        {
            setConditions(iStaticConditions, iDynamicConditions);
            init(iText, iSendNow);
        }
        #endregion Constructors

        #region Inits
        private void init(string iText, bool iSendNow)
        {
            text = iText;
            if (iSendNow)
            {
                Enabled = true;
                Execute();
            }
        }
        #endregion Inits

        #region Public Methods
        public override Boolean Execute(String iTarget = "")
        {
            try
            {
                if (Enabled)
                {
                    IocaineFunctions.keys(text);
                    LoggingFunctions.Debug(text, LoggingFunctions.DBG_SCOPE.COMMANDS);
                    IocaineFunctions.delay(50);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("RawCommand.doCommand caught exception!");
                LoggingFunctions.Error("Command: '" + text + "'");
                LoggingFunctions.Error("Enabled: '" + Enabled + "'");
                LoggingFunctions.Error("Exception:\n" + e.ToString());
                return false;
            }
            return true;
        }
        public override void Show()
        {
            MessageBox.Show(this.ToString(), "RawCommand::" + Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override string SaveString()
        {
            return "RawCommand;" + Text;
        }
        public override String ToString()
        {
            return Text;
        }
        #endregion Public Methods
    }
}
