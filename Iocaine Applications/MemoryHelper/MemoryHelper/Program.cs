using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Iocaine2.Memory;

namespace IocaineOffsetHelper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new IocaineOffsetsHelperForm());

            MemReads.OS_Version = Environment.OSVersion.Version.Major;
            IocaineOffsetsHelperForm mainForm = new IocaineOffsetsHelperForm();
            mainForm.ShowDialog();
        }
    }
}
