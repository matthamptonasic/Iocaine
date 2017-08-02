using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Iocaine2
{
    public static class eventFwdParser
    {
        #region Enums
        #endregion Enums

        #region Private Members
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Events & Delegates
        #endregion Events & Delegates

        #region Public Methods
        #endregion Public Methods
        public static void Parse(string iData)
        {
            string l_data = iData;
            ushort l_eventId = 0xffff;
            bool l_injected = false;
            bool l_blocked = false;
            Regex l_re = new Regex(@"(\d+) (.+) (true|false) (true|false)");
            Match l_match = l_re.Match(l_data);
            if (l_match.Success)
            {
                Logging.LoggingFunctions.Debug("Found: '" + l_match.Groups[1].Value + "'", Logging.LoggingFunctions.DBG_SCOPE.BACKGROUND);
                l_eventId = Convert.ToUInt16(l_match.Groups[1].Value);
                l_data = l_match.Groups[2].Value;
                l_injected = Convert.ToBoolean(l_match.Groups[3].Value);
                l_blocked = Convert.ToBoolean(l_match.Groups[4].Value);
                Logging.LoggingFunctions.Debug("Inj: " + l_injected + ", Blk: " + l_blocked, Logging.LoggingFunctions.DBG_SCOPE.BACKGROUND);
            }
            else
            {
                Logging.LoggingFunctions.Debug("Did not find event ID.");
                return;
            }

            try
            {
                l_re = new Regex(@"(\d+) (.+)");
                l_match = l_re.Match(l_data);
                ushort l_pktId = 0xffff;
                if (l_match.Success)
                {
                    Logging.LoggingFunctions.Debug("Found: '" + l_match.Groups[1].Value + "'", Logging.LoggingFunctions.DBG_SCOPE.BACKGROUND);
                    l_pktId = Convert.ToUInt16(l_match.Groups[1].Value);
                    l_data = l_match.Groups[2].Value;
                }
                else
                {
                    Logging.LoggingFunctions.Debug("Did not find event ID.", Logging.LoggingFunctions.DBG_SCOPE.BACKGROUND);
                    return;
                }
                if (l_pktId == 0x17)
                {
                    byte[] l_raw = Encoding.ASCII.GetBytes(l_data);
                    byte[] l_modeArr = new byte[4];
                    Array.Copy(l_raw, 0, l_modeArr, 0, 4);
                    uint l_chatMode = BitConverter.ToUInt32(l_raw, 0);
                    Logging.LoggingFunctions.Debug("2nd Got chat mode '" + l_chatMode + "'", Logging.LoggingFunctions.DBG_SCOPE.BACKGROUND);
                }
                else if (l_pktId == 0xE)
                {
                    byte[] l_raw = Encoding.ASCII.GetBytes(l_data);
                    Logging.LoggingFunctions.Debug("Data is " + l_raw.Length + " bytes long.", Logging.LoggingFunctions.DBG_SCOPE.BACKGROUND);
                }
                else
                {
                    Logging.LoggingFunctions.Debug("pkt ID was " + l_pktId, Logging.LoggingFunctions.DBG_SCOPE.BACKGROUND);
                }
            }
            catch (Exception e)
            {
                Logging.LoggingFunctions.Error(e.ToString());
            }

        }
        #region Private Methods
        #endregion Private Methods
    }
}
