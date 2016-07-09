using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Iocaine2.Memory
{
    public class ProcessFunctions
    {
        public static Process[] GetAllProcessByProcessName(string procName)
        {
            return Process.GetProcessesByName(procName);
        }
        public static Process GetMainProcessByProcessName(string procName)
        {
            Process[] procList = Process.GetProcessesByName(procName);
            if (procList.Count() > 0)
            {
                return procList[0];
            }
            else
            {
                return (Process)null;
            }
        }
        public static Process GetMainProcessByWindowName(string windowName)
        {
            Process[] procList = Process.GetProcesses();
            foreach (Process proc in procList)
            {
                if (proc.MainWindowTitle == windowName)
                {
                    return proc;
                }
            }
            return (Process)null;
        }
        public static int GetNumProcessesByProcessName(string procName)
        {
            Process[] procList = Process.GetProcessesByName(procName);
            return procList.Count();
        }
        public static ProcessModule GetMainModule(Process proc, string modName)
        {
            if (proc == null)
            {
                return (ProcessModule)null;
            }
            else
            {
                ProcessModuleCollection modList;
                try
                {
                    modList = proc.Modules;
                }
                catch
                {
                    return (ProcessModule)null;
                }
                if (modList.Count == 0)
                {
                    return (ProcessModule)null;
                }
                else
                {
                    foreach (ProcessModule mod in modList)
                    {
                        if (mod.ModuleName == modName)
                        {
                            return mod;
                        }
                    }
                    return (ProcessModule)null;  //if we didn't return while looping, we didn't find it
                }
            }
        }
        public static Boolean IsRunning(List<String> iProcNames)
        {
            Process[] allProcArray = null;
            foreach(String proc in iProcNames)
            {
                String procName = Path.GetFileNameWithoutExtension(proc);
                Process[] tempArray = Process.GetProcessesByName(procName);
                if(allProcArray == null)
                {
                    allProcArray = tempArray;
                    continue;
                }
                Process[] oldArray = allProcArray;
                allProcArray = new Process[tempArray.Length + oldArray.Length];
                Array.Copy(oldArray, allProcArray, oldArray.Length);
                Array.Copy(tempArray, 0, allProcArray, oldArray.Length, tempArray.Length);
            }

            foreach (Process proc in allProcArray)
            {
                if (iProcNames.Contains(proc.MainModule.FileName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
