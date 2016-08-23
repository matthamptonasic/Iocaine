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
    public class WeaponSkillCommand : Command
    {
        #region Private Members
        private Client.WeaponSkills.WS_INFO _WsInfo;
        private Boolean available = false;
        #endregion Private Members

        #region Public Properties
        public override Boolean Available
        {
            get
            {
                return available;
            }
            set
            {
                available = value;
            }
        }
        public override Boolean Ready
        {
            get
            {
                if (!available)
                {
                    return false;
                }
                else
                {
                    return MemReads.Self.Vitals.get_tp_current() >= 1000;
                }
            }
        }
        #endregion Public Properties

        #region Constructors
        public WeaponSkillCommand(String iName)
            : base(iName, CMD_TYPE.WEAPONSKILL, true)
        {
            _WsInfo = Client.WeaponSkills.GetWeaponSkillInfo(iName);
        }
        public WeaponSkillCommand(Client.WeaponSkills.WS_INFO iInfo)
            : base(iInfo.Name, CMD_TYPE.WEAPONSKILL, true)
        {
            _WsInfo = iInfo;
        }
        #endregion Constructors
        
        #region Public Methods
        public override Boolean Execute(String iTarget = "")
        {
            if (MemReads.Self.Casting.is_casting() || (MemReads.Self.get_status() != (Byte)FFXIEnums.STATUS.ATTACKING))
            {
                return false;
            }
            try
            {
                if (Ready)
                {
                    IocaineFunctions.keys(_WsInfo.Command + " <t>");
                    LoggingFunctions.Debug(_WsInfo.Command + " <t>", LoggingFunctions.DBG_SCOPE.COMMANDS);
                    // Wait a bit for the WS to proc.
                    IocaineFunctions.delay(1000);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("WeaponSkillCommand.doCommand caught exception!");
                LoggingFunctions.Error("Command: '" + _WsInfo.Command + "'");
                LoggingFunctions.Error("Name: '" + _WsInfo.Name + "'");
                LoggingFunctions.Error("Exception:\n" + e.ToString());
                return false;
            }
            return true;
        }
        public override void Show()
        {
            MessageBox.Show(this.ToString(), "WeaponSkillCommand::" + _WsInfo.Command, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override string SaveString()
        {
            return "WeaponSkillCommand;" + _WsInfo.Name;
        }
        public override String ToString()
        {
            return _WsInfo.Name;
        }
        #endregion Public Methods
    }
}
