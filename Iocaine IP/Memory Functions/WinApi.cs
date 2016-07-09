using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Iocaine2
{
    enum MESSAGES
    {
        WM_KEYDOWN =    0x100,
        WM_KEYUP =      0x101,
        WM_CHAR =       0x102
    }

    enum PROCESS_ACCESS
    {
        PROCESS_TERMINATE = 0x0001,
        PROCESS_CREATE_THREAD = 0x0002,
        PROCESS_SET_SESSIONID = 0x0004,
        PROCESS_VM_OPERATION = 0x0008,
        PROCESS_VM_READ = 0x0010,
        PROCESS_VM_WRITE = 0x0020,
        PROCESS_CREATE_PROCESS = 0x0080,
        PROCESS_SET_QUOTA = 0x0100,
        PROCESS_SET_INFORMATION = 0x0200,
        PROCESS_QUERY_INFORMATION = 0x0400,
        PROCESS_SUSPEND_RESUME = 0x0800,
        PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,
        PROCESS_ALL_ACCESS = 0x1F0FFF
    }
    enum MAP_VK
    {
        MAPVK_VK_TO_VSC = 0,
        MAPVK_VSC_TO_VK = 1,
        MAPVK_VK_TO_CHAR = 2,
        MAPVK_VSC_TO_VK_EX = 3
    }

    public class WinApi
    {
        [DllImport("Kernel32.dll")]
        public static extern UInt32 GetLastError();

        [DllImport("Kernel32.dll")]
        public static extern void SetLastError(UInt32 lastError);

        [DllImport("Kernel32.dll")]
        unsafe internal static extern Int32 ReadProcessMemory(IntPtr hProcess, UInt32 baseAddress, ref IntPtr buffer, UInt32 nsize, ref IntPtr nBytesRead);

        [DllImport("Kernel32.dll")]
        unsafe internal static extern Int32 ReadProcessMemory(IntPtr hProcess, UInt32 baseAddress, ref Single buffer, UInt32 nsize, ref IntPtr nBytesRead);

        [DllImport("Kernel32.dll")]
        unsafe internal static extern Int32 ReadProcessMemory(IntPtr hProcess, UInt32 baseAddress, ref UIntPtr buffer, UInt32 nsize, ref IntPtr nBytesRead);

        [DllImport("Kernel32.dll")]
        unsafe internal static extern Int32 ReadProcessMemory(IntPtr hProcess, UInt32 baseAddress, [In, Out] byte[] buffer, UInt32 nsize, ref IntPtr nBytesRead);

        [DllImport("Kernel32.dll")]
        unsafe internal static extern bool WriteProcessMemory(IntPtr hProcess, UInt32 baseAddress, ref IntPtr buffer, UInt32 nsize, ref IntPtr nBytesWritten);

        [DllImport("Kernel32.dll")]
        unsafe internal static extern bool WriteProcessMemory(IntPtr hProcess, UInt32 baseAddress, ref Single buffer, UInt32 nsize, ref IntPtr nBytesWritten);

        [DllImport("Kernel32.dll")]
        unsafe internal static extern bool WriteProcessMemory(IntPtr hProcess, UInt32 baseAddress, ref UIntPtr buffer, UInt32 nsize, ref IntPtr nBytesWritten);

        [DllImport("Kernel32.dll")]
        unsafe internal static extern bool WriteProcessMemory(IntPtr hProcess, UInt32 baseAddress, byte[] buffer, UInt32 nsize, ref IntPtr nBytesWritten);

        public const UInt32 FLASHW_STOP = 0;
        public const UInt32 FLASHW_ALL = 3;
        public const UInt32 FLASHW_TIMERNOFG = 12;
        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        unsafe public static extern bool FlashWindowEx(ref FLASHWINFO pwfi);
        
        [DllImport("user32.dll")]
        internal static extern Int32 TranslateMessage(ref Message msg);

        [DllImport("user32.dll")]
        internal static extern Int32 PostMessage(IntPtr hWnd, UInt32 msg, UInt32 wParam, UInt32 lParam);

        [DllImport("user32.dll")]
        internal static extern Int32 FindWindowEx(IntPtr hWndParent, IntPtr hWndChild, String className, String windowName);

        [DllImport("user32.dll")]
        internal static extern Int32 FindWindow(String className, String windowName);

        [DllImport("user32.dll")]
        internal static extern UInt32 MapVirtualKey(UInt32 code, UInt32 mapType);
    }
}
