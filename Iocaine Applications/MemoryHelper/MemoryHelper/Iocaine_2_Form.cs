using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using IocaineOffsetHelper;
using Iocaine2.Logging;

namespace Iocaine2
{
    partial class Iocaine_2_Form
    {
        public static Process mainProc = null;
        public static ProcessModule mainModule = null;
        public static int MaxInventoryCount = 80;
        internal static int dbg = 1;
        public static StreamWriter logFile = null;
        public delegate void statusBoxUpdateDelegate(String text, System.Drawing.Color color);
        //private statusBoxUpdateDelegate updateStatusBoxCallBack;
        internal static statusBoxUpdateDelegate updateStatusBoxGlobalPtr;
        #region Colors
        public static System.Drawing.Color boxBlue = System.Drawing.Color.PaleTurquoise;
        public static System.Drawing.Color boxRed = System.Drawing.Color.LightCoral;
        public static System.Drawing.Color boxYellow = System.Drawing.Color.PaleGoldenrod;
        public static System.Drawing.Color boxGreen = System.Drawing.Color.LightGreen;
        public static System.Drawing.Color boxGrey = System.Drawing.Color.LightGray;
        public static System.Drawing.Color buttonGreen = System.Drawing.Color.Lime;
        public static System.Drawing.Color buttonYellow = System.Drawing.Color.Yellow;
        public static System.Drawing.Color buttonRed = System.Drawing.Color.Red;
        #endregion Colors
        public static void setLogFile()
        {
            LoggingFunctions.SetLogFile("Iocaine Log.txt");
            logFile = LoggingFunctions.LogFile;
        }
        public static void debug(String text)
        {
            try
            {
                Monitor.Enter(logFile);
                Console.WriteLine("{0} [DEBUG_{1}] {2}", DateTime.Now.ToString(), dbg, text);
                Monitor.Exit(logFile);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Error trying to write a debug message to the output file.");
            }
        }

        public static void timestamp(String text)
        {
            try
            {
                Monitor.Enter(logFile);
                Console.WriteLine("{0} {1}", DateTime.Now.ToString(), text);
                Monitor.Exit(logFile);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Error trying to write a timestamp message to the output file.");
            }
        }
        #region Stubs from iocaine2
        internal static statusBoxUpdateDelegate sptr_updateStatusBoxGlobalPtr;
        public static void playSound(String iText)
        {

        }
        #endregion Stubs from iocaine2
    }
}
