using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;

using Iocaine2.Logging;

namespace Iocaine2.Memory
{
    internal class MemScanner
    {
        #region Structures
        internal class ScanResult
        {
            public ScanResult(byte[] iResultPattern, uint iResultAddress, int iResultOffset = 0)
            {
                ResultPattern = iResultPattern;
                ResultAddress = iResultAddress;
                resultOffset = iResultOffset;
            }
            public static ScanResult NullResult = new ScanResult(new byte[1] { 0 }, 0);
            public byte[] ResultPattern;
            public uint ResultAddress;
            private int resultOffset; //Only applies to 32-bit pointers
            public uint UInt32
            {
                get
                {
                    int nbBytes = ResultPattern.Length > 4 ? 4 : ResultPattern.Length;
                    uint result = 0;
                    for (int ii = 0; ii < nbBytes; ii++)
                    {
                        result |= (uint)(ResultPattern[nbBytes - ii - 1] << (24 - (ii * 8)));
                    }
                    return (uint)(result + resultOffset);
                }
            }
            public ushort UInt16
            {
                get
                {
                    int nbBytes = ResultPattern.Length > 2 ? 2 : ResultPattern.Length;
                    ushort result = 0;
                    for (int ii = 0; ii < nbBytes; ii++)
                    {
                        result |= (ushort)(ResultPattern[nbBytes - ii - 1] << (8 - (ii * 8)));
                    }
                    return result;
                }
            }
            public byte Byte
            {
                get
                {
                    return (byte)(ResultPattern.Length > 0 ? ResultPattern[ResultPattern.Length - 1] : 0);
                }
            }
            public bool Success
            {
                get
                {
                    return (ResultPattern.Length > 0) && (ResultAddress != 0);
                }
            }
        }
        #endregion Structures
        #region Constructors
        internal MemScanner(Process iProc, ProcessModule iMod)
        {
            ChangeProcess(iProc, iMod);
        }
        #endregion Constructors
        #region Member Variables
        #region Public
        internal bool Dumped = false;
        #endregion Public
        #region Private
        private Process mainProc = null;
        private ProcessModule mainMod = null;
        private UInt32 moduleBase = 0;
        private UInt32 moduleSize = 0;
        private UInt32 moduleEnd = 0;
        private Int32 textSegSize = 0x2C4000;
        private byte[] memDump = null;
        String memString = "";
        #endregion Private
        #endregion Member Variable
        #region Public Member Functions
        internal bool DumpMemory()
        {
            if (mainProc == null || mainMod == null)
            {
                MessageBox.Show("Process not set.");
                return false;
            }
            memDump = new byte[moduleSize];
            try
            {
                uint blockSize = 20480;
                uint nbReads = moduleSize / blockSize;
                uint lastReadSize = moduleSize % blockSize;
                uint currentBase = moduleBase;
                byte[] blockDump = null;
                for (int ii = 0; ii < nbReads; ii++)
                {
                    blockDump = new byte[blockSize];
                    try
                    {
                        blockDump = MemoryFunctions.ReadBlock((IntPtr)mainProc.Handle, currentBase, blockDump, blockSize);
                        System.Buffer.BlockCopy(blockDump, 0, memDump, (int)(ii * blockSize), (int)blockSize);
                        currentBase += blockSize;
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Timestamp("Error reading memory block: " + currentBase + ", Exception: " + e.ToString());
                    }
                }
                if (lastReadSize > 0)
                {
                    blockDump = new byte[lastReadSize];
                    blockDump = MemoryFunctions.ReadBlock((IntPtr)mainProc.Handle, currentBase, blockDump, lastReadSize);
                    System.Buffer.BlockCopy(blockDump, 0, memDump, (int)(nbReads * blockSize), (int)lastReadSize);
                }
                memString = BitConverter.ToString(memDump);
                Dumped = true;
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Timestamp("Error dumping memory: " + e.ToString());
                return false;
            }
        }
        internal ScanResult ScanPattern(MemReads.Signature iSignature)
        {
            byte[] pattern = null;
            uint address = 0;
            string matchByte = "-([0-9|A-F][0-9|A-F])";
            string matchPattern = "";
            for (int ii = 0; ii < iSignature.PatternLength; ii++)
            {
                matchPattern += matchByte;
            }
            matchPattern += "-";
            string prefixModified = iSignature.Prefix;
            for (int ii = 0; ii < iSignature.PatternOffset; ii++)
            {
                prefixModified += "-..";
            }
            if (mainProc == null || mainMod == null)
            {
                LoggingFunctions.Timestamp("Process not set.");
                return ScanResult.NullResult;
            }
            if (!Dumped)
            {
                LoggingFunctions.Timestamp("Use DumpMemory() before scanning for pattern.");
                return ScanResult.NullResult;
            }
            string searchString = prefixModified + matchPattern + iSignature.Postfix;
            Regex regex1 = new Regex(searchString);
            Match match1;
            try
            {
                match1 = regex1.Match(memString);
                if (!match1.Success)
                {
                    LoggingFunctions.Timestamp("Could not find pattern \"" + searchString + "\" in memory.");
                    return ScanResult.NullResult;
                }
                else
                {
                    address = (UInt32)(match1.Groups[1].Index / 3) + moduleBase;
                    int nbMatches = match1.Groups.Count;
                    pattern = new byte[nbMatches-1];
                    for (int ii = 0; ii < nbMatches-1; ii++)
                    {
                        pattern[ii] = Byte.Parse(match1.Groups[ii+1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    }
                    return new ScanResult(pattern, address, iSignature.ResultOffset);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Failed trying to match pattern \"" + searchString + "\" with exception: " + e.ToString());
                return ScanResult.NullResult;
            }
        }
        internal void ChangeProcess(Process iProc, ProcessModule iMod)
        {
            mainProc = iProc;
            mainMod = iMod;
            moduleBase = (UInt32)mainMod.BaseAddress;
            //moduleEnd = (uint)(moduleBase + MemoryFunctions.ReadMem(mainProc.Handle, moduleBase, textSegEndOffset, 4));
            moduleEnd = (uint)(moduleBase + textSegSize);
            moduleSize = moduleEnd - moduleBase;
            LoggingFunctions.Debug("Module base: " + String.Format("{0:X}", moduleBase), LoggingFunctions.DBG_SCOPE.MEM_SCANNER);
            LoggingFunctions.Debug("Module end:  " + String.Format("{0:X}", moduleEnd), LoggingFunctions.DBG_SCOPE.MEM_SCANNER);
            LoggingFunctions.Debug("Module size: " + String.Format("{0:X}", moduleSize), LoggingFunctions.DBG_SCOPE.MEM_SCANNER);
            Dumped = false;
        }
        #endregion Public Member Functions
    }
}
