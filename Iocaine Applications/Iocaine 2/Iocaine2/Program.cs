using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.Diagnostics;


namespace Iocaine2
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Iocaine_2_Form mainForm = new Iocaine_2_Form();
            try
            {
                WinApi.SetLastError(0);

                mainForm.ShowDialog();
            }
            catch (Exception ex)
            {
                string errMsg = "Iocaine has experienced a failure that it could not handle.\nIf this error persists, please send me a PM with the details.\n";
                if (!mainForm.IsDisposed)
                {
                    errMsg += "The error details are given below.\n\n";
                    errMsg += ex.ToString();
                    MessageBox.Show(mainForm, errMsg, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    errMsg += "The error was caught too late to record all of the details.\nPlease check the log file for any error messages.";
                    MessageBox.Show(errMsg, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
