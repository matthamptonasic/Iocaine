using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

using Iocaine2.Logging;

namespace Iocaine2.Memory.Interface
{
    public static class IocaineFunctions
    {
        #region Private Members
        private static kbHelper myKeyBoard = null;
        #endregion Private Members
        #region Public Members
        #endregion Public Members
        #region Public Methods
        public static bool delay(UInt32 ms)
        {
            Thread.Sleep((int)ms);
            return true;
        }
        public static bool keyDown(Keys key)
        {
            return keyDown(key, Statics.Settings.Top.KeyHoldTime);
        }
        public static bool keyDown(Keys key, UInt32 time)
        {
            if (myKeyBoard == null)
            {
                LoggingFunctions.Debug("You must first specify a key board helper before using key functions", LoggingFunctions.DBG_SCOPE.WIN_API);
                return false;
            }
            try
            {
                UInt32 scanCode = WinApi.MapVirtualKey((UInt32)key, (UInt32)MAP_VK.MAPVK_VK_TO_VSC);
                kbHelper.setKey(myKeyBoard.kbHandle, (byte)scanCode, true);
                delay(time);
                kbHelper.setKey(myKeyBoard.kbHandle, (byte)scanCode, false);
                return true;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("In keyDown: " + e.ToString());
                return false;
            }
        }
        public static bool arrowKeyDown(Keys key)
        {
            return arrowKeyDown(key, Statics.Settings.Top.KeyHoldTime);
        }
        public static bool arrowKeyDown(Keys key, UInt32 time)
        {
            if (myKeyBoard == null)
            {
                LoggingFunctions.Debug("You must first specify a key board helper before using key functions", LoggingFunctions.DBG_SCOPE.WIN_API);
                return false;
            }
            try
            {
                UInt32 scanCode;
                switch (key)
                {
                    case Keys.Right:
                        scanCode = 0xCD;
                        break;
                    case Keys.Left:
                        scanCode = 0xCB;
                        break;
                    case Keys.Down:
                        scanCode = 0xD0;
                        break;
                    case Keys.Up:
                        scanCode = 0xC8;
                        break;
                    default:
                        LoggingFunctions.Error("Non-arrow key passed to arrowkey function.");
                        scanCode = 0x1; //escape key
                        break;
                }
                kbHelper.setKey(myKeyBoard.kbHandle, (byte)scanCode, true);
                delay(time);
                kbHelper.setKey(myKeyBoard.kbHandle, (byte)scanCode, false);
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In arrowKeyDown: " + e.ToString());
                return false;
            }
        }
        public static bool keys(string str, bool enterAtEnd)
        {
            if (myKeyBoard == null)
            {
                LoggingFunctions.Debug("You must first specify a key board helper before using key functions", LoggingFunctions.DBG_SCOPE.WIN_API);
                return false;
            }
            try
            {
                kbHelper.sendString(myKeyBoard.kbHandle, str);
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In keys: " + e.ToString());
                return false;
            }
        }
        public static bool keys(string str)
        {
            if (myKeyBoard == null)
            {
                LoggingFunctions.Debug("You must first specify a key board helper before using key functions", LoggingFunctions.DBG_SCOPE.WIN_API);
                return false;
            }
            try
            {
                return keys(str, true);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In keys: " + e.ToString());
                return false;
            }
        }
        public static bool holdKey(Keys key)
        {
            if (myKeyBoard == null)
            {
                LoggingFunctions.Debug("You must first specify a key board helper before using key functions", LoggingFunctions.DBG_SCOPE.WIN_API);
                return false;
            }
            try
            {
                UInt32 scanCode = WinApi.MapVirtualKey((UInt32)key, (UInt32)MAP_VK.MAPVK_VK_TO_VSC);
                kbHelper.setKey(myKeyBoard.kbHandle, (byte)scanCode, true);
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In holdKey: " + e.ToString());
                return false;
            }
        }
        public static bool releaseKey(Keys key)
        {
            if (myKeyBoard == null)
            {
                LoggingFunctions.Debug("You must first specify a key board helper before using key functions", LoggingFunctions.DBG_SCOPE.WIN_API);
                return false;
            }
            try
            {
                UInt32 scanCode = WinApi.MapVirtualKey((UInt32)key, (UInt32)MAP_VK.MAPVK_VK_TO_VSC);
                kbHelper.setKey(myKeyBoard.kbHandle, (byte)scanCode, false);
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In releaseKey: " + e.ToString());
                return false;
            }
        }
        public static bool twoKeys(Keys key1, Keys key2)
        {
            return twoKeys(key1, key2, Statics.Settings.Top.KeyHoldTime);
        }
        public static bool twoKeys(Keys key1, Keys key2, UInt32 time)
        {
            if (myKeyBoard == null)
            {
                LoggingFunctions.Debug("You must first specify a key board helper before using key functions", LoggingFunctions.DBG_SCOPE.WIN_API);
                return false;
            }
            try
            {
                UInt32 scanCode1 = WinApi.MapVirtualKey((UInt32)key1, (UInt32)MAP_VK.MAPVK_VK_TO_VSC);
                UInt32 scanCode2 = WinApi.MapVirtualKey((UInt32)key2, (UInt32)MAP_VK.MAPVK_VK_TO_VSC);
                kbHelper.setKey(myKeyBoard.kbHandle, (byte)scanCode1, true);
                kbHelper.setKey(myKeyBoard.kbHandle, (byte)scanCode2, true);
                delay(time);
                kbHelper.setKey(myKeyBoard.kbHandle, (byte)scanCode1, false);
                kbHelper.setKey(myKeyBoard.kbHandle, (byte)scanCode2, false);
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In twoKeys: " + e.ToString());
                return false;
            }
        }
        public static bool setKbHelper(Process iProc)
        {
            if (iProc == null)
            {
                return false;
            }
            try
            {
                String kbName = "WindowerMMFKeyboardHandler" + "_" + iProc.Id.ToString();
                myKeyBoard = new kbHelper(kbName);
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In setKbHelper: " + e.ToString());
                return false;
            }
        }
        public static void deleteKbHelper()
        {
            if (myKeyBoard != null)
            {
                try
                {
                    kbHelper.Delete(myKeyBoard.kbHandle);
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("In deleteKbHelper: " + e.ToString());
                }
            }
        }
        #endregion Public Methods
        #region Private Methods
        #endregion Private Methods
    }
}
