using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;

using Iocaine2.Data.Client;
using Iocaine2.Inventory;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Settings;
using Iocaine2.Tools;

namespace Iocaine2.Bots
{
    public sealed partial class PowerLevel : Bot
    {
        #region Private Members
        #region Control Declarations
        #endregion Control Declarations
        #region Flags
        #endregion Flags
        #endregion Private Members

        #region Inits
        private void Init_Controls()
        {

        }
        #endregion Inits

        #region Control Updates and Call-Backs
        
        #endregion Control Updates and Call-Backs

        #region Control Event Handlers
        #region Start/Stop
        private void c_startButton_Click(object sender, EventArgs e)
        {
            if (state == Bots.STATE.RUNNING)
            {
                Pause(false);
            }
            else if (state == Bots.STATE.STOPPED)
            {
                Start();
            }
            else
            {
                Resume();
            }
        }
        private void c_stopButton_Click(object sender, EventArgs e)
        {
            Stop();
        }
        #endregion Start/Stop
        #endregion Control Event Handlers
    }
}