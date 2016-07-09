using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace Iocaine2.Memory
{
    internal class MemoryFunctions
    {
        internal static Int32 ReadMem(IntPtr hProcess, UInt32 address, Int32 structOffset, UInt32 nbBytes)
        {
            Int32 pass = 0;
            IntPtr dataRead = (IntPtr)0;
            IntPtr bytesRead = (IntPtr)0;
            if (hProcess == (IntPtr)0)
            {
                return -1;
            }
            try
            {
                pass = WinApi.ReadProcessMemory(hProcess, (UInt32) (address + structOffset), ref dataRead, nbBytes, ref bytesRead);
            }
            catch
            {
                MessageBox.Show("Error while reading from process memory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            if (pass != 0)
            {
                return (Int32) dataRead;
            }
            else
            {
                return (Int32) 0;
            }
        }
        internal static Single ReadMem(IntPtr hProcess, UInt32 address, Int32 structOffset)
        {
            Int32 pass = 0;
            Single dataRead = (Single) 0;
            IntPtr bytesRead = (IntPtr) 0;
            if (hProcess == (IntPtr)0)
            {
                return -1;
            }
            try
            {
                pass = WinApi.ReadProcessMemory(hProcess, (UInt32) (address + structOffset), ref dataRead, 4, ref bytesRead);
            }
            catch
            {
                MessageBox.Show("Error while reading a float from process memory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            if (pass != 0)
            {
                return dataRead;
            }
            else
            {
                return (Single) 0;
            }
        }
        internal static byte[] ReadBlock(IntPtr hProcess, UInt32 address, byte[] buffer, UInt32 nbBytes)
        {
            Int32 pass = 0;
            IntPtr bytesRead = (IntPtr)0;
            if (hProcess == (IntPtr)0)
            {
                return null;
            }
            try
            {
                pass = WinApi.ReadProcessMemory(hProcess, (UInt32)address, buffer, nbBytes, ref bytesRead);
            }
            catch
            {
                MessageBox.Show("Error while reading from process memory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            if (pass != 0)
            {
                return buffer;
            }
            else
            {
                return null;
            }
        }
        internal static String ReadString(IntPtr hProcess, UInt32 address, Int32 structOffset)
        {
            Int32 pass = 0;
            String stringRead = null;
            IntPtr dataRead = (IntPtr)0;
            IntPtr bytesRead = (IntPtr)0;
            if (hProcess == (IntPtr)0)
            {
                return "";
            }
            do
            {
                try
                {
                    pass = WinApi.ReadProcessMemory(hProcess, (UInt32)(address + structOffset), ref dataRead, 1, ref bytesRead);
                }
                catch
                {
                    MessageBox.Show("Error while reading string from process memory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                structOffset++;
                stringRead += (char)dataRead;
            }
            while (((int)dataRead != 0) && ((int)dataRead != 127) && (pass != 0));

            if (pass != 0)
            {
                return stringRead;
            }
            else
            {
                return null;
            }

        }
        internal static String ReadStringChatFFXI(IntPtr hProcess, UInt32 address, Int32 structOffset)
        {
            Int32 pass = 0;
            String stringRead = "";
            IntPtr dataRead = (IntPtr)0;
            IntPtr bytesRead = (IntPtr)0;
            Boolean hitSOL = false;
            Boolean hit30_1 = false;
            Boolean hit1_1 = false;
            Boolean hit30_2 = false;
            Boolean hit129 = false;
            Boolean hit64 = false;
            Int32 maxOffset = 1000 + structOffset;
            Int32 currentOffset = structOffset;
            if (hProcess == (IntPtr)0)
            {
                return "";
            }
            //First clear out all of the meta-data.
            //The meta-data ends with 30 1 30 1 (byte values)
            //If the line is a continuation from the previous, it may look like:
            //30 1 129 64 30 1
            //Also seen:
            //30 8 30 1  You have undertaken the limited-time Records of Eminence challenge "Spoils
            //30 1 30 8  (Seals)."
            //I'm guessing this means to indent.
            do
            {
                UInt32 effectiveAddr = (UInt32)(address + currentOffset);
                try
                {
                    pass = WinApi.ReadProcessMemory(hProcess, effectiveAddr, ref dataRead, 1, ref bytesRead);
                }
                catch
                {
                    MessageBox.Show("Error while reading string from process memory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                //Logging.LoggingFunctions.Debug("Read '" + (char)dataRead + "' [" + (Byte)dataRead + "] from 0x " + String.Format("{0:x}", effectiveAddr), Logging.LoggingFunctions.DBG_SCOPE.MEMREADS);
                if (hit30_1 && hit1_1 && hit30_2 && (((Byte)dataRead == 1) || ((Byte)dataRead == 8)))
                {
                    hitSOL = true;
                }
                else if (hit30_1 && hit1_1 && !hit30_2 && ((Byte)dataRead == 30))
                {
                    hit30_2 = true;
                }
                else if (hit30_1 && hit1_1 && hit129 && hit64 && !hit30_2 && ((Byte)dataRead == 30))
                {
                    hit30_2 = true;
                }
                else if (hit30_1 && hit1_1 && !hit64 && !hit129 && !hit30_2 && ((Byte)dataRead == 129))
                {
                    hit129 = true;
                }
                else if (hit30_1 && hit1_1 && hit129 && !hit30_2 && ((Byte)dataRead == 64))
                {
                    hit64 = true;
                }
                else if (hit30_1 && !hit1_1 && (((Byte)dataRead == 1) || ((Byte)dataRead == 8)))
                {
                    hit1_1 = true;
                }
                else if (!hit30_1 && ((Byte)dataRead == 30))
                {
                    hit30_1 = true;
                }
                else
                {
                    hit30_1 = hit1_1 = hit30_2 = hit129 = hit64 = hitSOL = false;
                }
                currentOffset++;
            }
            while ((pass != 0) && !hitSOL && (currentOffset < maxOffset));

            if ((pass == 0) || (currentOffset >= maxOffset))
            {
                if (currentOffset >= maxOffset)
                {
                    Logging.LoggingFunctions.Error("Could not find start of string after " + maxOffset + " iterations.");
                    byte[] blockDump = null;
                    uint blockSize = (uint)(currentOffset - structOffset);
                    UInt32 startAddr = (UInt32)(address + structOffset);
                    blockDump = new byte[blockSize];
                    blockDump = MemoryFunctions.ReadBlock(hProcess, startAddr, blockDump, blockSize);
                    String dumpBytesString = BitConverter.ToString(blockDump);
                    String dumpCharString = "";
                    for(int ii=0; ii<blockDump.Length; ii++)
                    {
                        dumpCharString += (char)blockDump[ii];
                        //dumpBytesString += blockDump[ii].ToString();
                    }
                    //Logging.LoggingFunctions.Timestamp("Dumped characters:\n" + dumpCharString);
                    //Logging.LoggingFunctions.Timestamp("Dumped bytes:\n" + dumpBytesString);
                    //MessageBox.Show("Hit string error.");
                }
                return "";
            }

            do
            {
                UInt32 effectiveAddr = (UInt32)(address + currentOffset);
                try
                {
                    pass = WinApi.ReadProcessMemory(hProcess, effectiveAddr, ref dataRead, 1, ref bytesRead);
                }
                catch
                {
                    MessageBox.Show("Error while reading string from process memory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                //Logging.LoggingFunctions.Debug(":Read '" + (char)dataRead + "' [" + (Byte)dataRead + "] from 0x " + String.Format("{0:x}", effectiveAddr), Logging.LoggingFunctions.DBG_SCOPE.MEMREADS);
                currentOffset++;
                stringRead += (char)dataRead;
            }
            while ((pass != 0) && ((Byte)dataRead != 0));

            if (pass != 0)
            {
                return stringRead;
            }
            else
            {
                return "";
            }

        }
        internal static String ReadStringUniFfxi(IntPtr hProcess, UInt32 address, Int32 structOffset, Int32 offToNbChar)
        {
            Int32 pass = 0;
            String stringRead = null;
            IntPtr dataRead = IntPtr.Zero;
            IntPtr bytesRead = IntPtr.Zero;
            Int32 nbChar = 0;
            List<Byte> filterList = new List<byte>();
            filterList.Add(0xfd);
            filterList.Add(0xfe);
            filterList.Add(0xff);
            if (hProcess == IntPtr.Zero)
            {
                return "";
            }
            if (WinApi.ReadProcessMemory(hProcess, (UInt32)(address + offToNbChar), ref dataRead, 1, ref bytesRead) != 0)
            {
                nbChar = (Int32)dataRead;
            }
            for (int ii = structOffset; ii < structOffset + (nbChar * 2); ii+=2)
            {
                try
                {
                    pass = WinApi.ReadProcessMemory(hProcess, (UInt32)(address + ii), ref dataRead, 1, ref bytesRead);
                }
                catch
                {
                    MessageBox.Show("Error while reading string from process memory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                if (!filterList.Contains((Byte)dataRead))
                {
                    stringRead += (char)((char)dataRead + 0x20);
                }
            }

            if (pass != 0)
            {
                return stringRead.Trim();
            }
            else
            {
                return null;
            }

        }
        internal static UIntPtr GetPointer(IntPtr hProcess, UInt32 address, Int32 offset)
        {
            return (UIntPtr) GetPointer(hProcess, (uint)(address + offset));
        }
        internal static UIntPtr GetPointer(IntPtr hProcess, UInt32 address)
        {
            Int32 pass = 0;
            UIntPtr dataRead = (UIntPtr)0;
            IntPtr bytesRead = (IntPtr)0;
            if (hProcess == (IntPtr)0)
            {
                return (UIntPtr)0;
            }
            try
            {
                pass = WinApi.ReadProcessMemory(hProcess, (UInt32)(address), ref dataRead, 4, ref bytesRead);
            }
            catch
            {
                MessageBox.Show("Error while reading pointer from process memory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            if (pass != 0)
            {
                return (UIntPtr)dataRead;
            }
            else
            {
                return (UIntPtr)0;
            }
        }
        internal static bool WriteMem(IntPtr hProcess, UInt32 address, Int32 data, Int32 structOffset, UInt32 nbBytes)
        {
            bool pass = false;
            IntPtr dataBuffer = (IntPtr) data;
            IntPtr bytesWritten = (IntPtr)0;
            if (hProcess == (IntPtr)0)
            {
                return false;
            }
            try
            {
                pass = WinApi.WriteProcessMemory(hProcess, (UInt32)(address + structOffset), ref dataBuffer, nbBytes, ref bytesWritten);
            }
            catch
            {
                MessageBox.Show("Error while reading from process memory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            return pass;
        }
        internal static bool WriteMem(IntPtr hProcess, UInt32 address, UInt32 data, Int32 structOffset, UInt32 nbBytes)
        {
            bool pass = false;
            UIntPtr dataBuffer = (UIntPtr)data;
            IntPtr bytesWritten = (IntPtr)0;
            if (hProcess == (IntPtr)0)
            {
                return false;
            }
            try
            {
                pass = WinApi.WriteProcessMemory(hProcess, (UInt32)(address + structOffset), ref dataBuffer, nbBytes, ref bytesWritten);
            }
            catch
            {
                MessageBox.Show("Error while reading from process memory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            return pass;
        }
        internal static bool WriteMem(IntPtr hProcess, UInt32 address, float data, Int32 structOffset, UInt32 nbBytes)
        {
            bool pass = false;
            float dataBuffer = data;
            IntPtr bytesWritten = (IntPtr)0;
            if (hProcess == (IntPtr)0)
            {
                return false;
            }
            try
            {
                pass = WinApi.WriteProcessMemory(hProcess, (UInt32)(address + structOffset), ref dataBuffer, 4, ref bytesWritten);
            }
            catch
            {
                MessageBox.Show("Error while reading from process memory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            return pass;
        }
        internal static bool WriteMem(IntPtr hProcess, UInt32 address, byte[] buffer, Int32 structOffset, UInt32 nbBytes, ref UInt32 oBytesWritten)
        {
            bool pass = false;
            IntPtr bytesWritten = (IntPtr)oBytesWritten;
            if (hProcess == (IntPtr)0)
            {
                return false;
            }
            try
            {
                pass = WinApi.WriteProcessMemory(hProcess, (UInt32)(address + structOffset), buffer, nbBytes, ref bytesWritten);
            }
            catch
            {
                MessageBox.Show("Error while reading from process memory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            return pass;
        }
    }
}
