using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Iocaine2
{
    public static partial class Server
    {
        public partial class PasswordEntry : Form
        {
            #region Members
            private String password = "";
            #endregion Members
            #region Delegates
            public delegate void PasswordUpdateHandler(object iSender, PasswordOkEventArgs e);
            public event PasswordUpdateHandler PasswordUpdate;
            #endregion Delegates
            #region Constructors
            public PasswordEntry()
            {
                InitializeComponent();
            }
            #endregion Constructors
            #region Utility Functions

            #endregion Utility Functions
            #region Event Handlers
            private void PasswordTB_TextChanged(object sender, EventArgs e)
            {
                password = PwdEntry_PasswordTB.Text;
            }
            private void CancelButton_Click(object sender, EventArgs e)
            {
                this.Dispose();
            }
            private void OkButton_Click(object sender, EventArgs e)
            {
                PasswordOkEventArgs evArg = new PasswordOkEventArgs(password);
                PasswordUpdate(this, evArg);
                this.Dispose();
            }
            #endregion Event Handlers
        }
        #region Event Args Class
        public class PasswordOkEventArgs : System.EventArgs
        {
            private String eUserPassword;
            public String UserPassword
            {
                get
                {
                    return eUserPassword;
                }
            }
            public PasswordOkEventArgs(String iUserPassword)
            {
                eUserPassword = iUserPassword;
            }
        }
        #endregion Event Args Class
    }
}
