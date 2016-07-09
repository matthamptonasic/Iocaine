using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace Iocaine2.Memory.Interface
{
    internal class kbHelper
    {
        public kbHelper(String kbName)
        {
            kbHandle = Create(kbName);
            //System.Console.WriteLine("my kb handle is {0}", kbHandle);
        }
        ~kbHelper()
        {
            Delete(kbHandle);
        }

        [DllImport("WindowerHelper.dll", EntryPoint = "CreateKeyboardHelper")]
        public static extern int Create(string name);
        [DllImport("WindowerHelper.dll", EntryPoint = "DeleteKeyboardHelper")]
        public static extern void Delete(int helper);

        [DllImport("WindowerHelper.dll", EntryPoint = "CKHSetKey")]
        public static extern void setKey(int helper, byte key, bool down);
        [DllImport("WindowerHelper.dll", EntryPoint = "CKHSendString")]
        public static extern void sendString(int helper, string str);
        
        [DllImport("WindowerHelper.dll", EntryPoint = "CKHBlockInput")]
        public static extern void blockInput(int helper, bool block);

        public int kbHandle;
    }
}
