using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Structures;
using Iocaine2.Logging;

namespace Iocaine2.Char
{
    public class PLCharacter : Character
    {
        public PLCharacter(String iName, int iPriority,
                           Command iFirstCureCommand, Command iSecondCureCommand, Command iThirdCureCommand,
                           Byte iFirstCurePerc, Byte iSecondCurePerc, Byte iThirdCurePerc, List<Task> iBuffsList) :
            base (iName)
        {
            priority = iPriority;
            firstCureCommand = iFirstCureCommand;
            secondCureCommand = iSecondCureCommand;
            thirdCureCommand = iThirdCureCommand;
            firstCureCommandMaster = iFirstCureCommand;
            secondCureCommandMaster = iSecondCureCommand;
            thirdCureCommandMaster = iThirdCureCommand;
            firstCurePerc = iFirstCurePerc;
            secondCurePerc = iSecondCurePerc;
            thirdCurePerc = iThirdCurePerc;
            taskList = new List<Task>();
            taskListMaster = new List<Task>();
            foreach (Task buff in iBuffsList)
            {
                taskList.Add(buff);
                LoggingFunctions.Debug("Adding " + buff.Cmd.Name + " to " + iName + "'s task list with recast of " + buff.Interval.ToString(), LoggingFunctions.DBG_SCOPE.PL);
            }
        }

        public void DisposeTasks()
        {
            int taskCount = taskList.Count;
            for (int ii = taskCount - 1; ii >= 0; ii--)
            {
                ((Task)taskList[ii]).Kill();
                taskList.RemoveAt(ii);
            }
        }
        private List<Task> taskList;
        public List<Task> TaskList
        {
            get
            {
                return taskList;
            }
        }
        private List<Task> taskListMaster;
        public List<Task> TaskListMaster
        {
            get
            {
                return taskListMaster;
            }
        }
        private int priority = 1;
        public int Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
            }
        }
        private bool cureQueued = false;
        public bool CureQueued
        {
            get
            {
                return cureQueued;
            }
            set
            {
                cureQueued = value;
            }
        }
        private Command firstCureCommand;
        public Command FirstCureCommand
        {
            get
            {
                return firstCureCommand;
            }
            set
            {
                firstCureCommand = value;
            }
        }
        private Command secondCureCommand;
        public Command SecondCureCommand
        {
            get
            {
                return secondCureCommand;
            }
            set
            {
                secondCureCommand = value;
            }
        }
        private Command thirdCureCommand;
        public Command ThirdCureCommand
        {
            get
            {
                return thirdCureCommand;
            }
            set
            {
                thirdCureCommand = value;
            }
        }
        private Command firstCureCommandMaster;
        public Command FirstCureCommandMaster
        {
            get
            {
                return firstCureCommandMaster;
            }
            set
            {
                firstCureCommandMaster = value;
            }
        }
        private Command secondCureCommandMaster;
        public Command SecondCureCommandMaster
        {
            get
            {
                return secondCureCommandMaster;
            }
            set
            {
                secondCureCommandMaster = value;
            }
        }
        private Command thirdCureCommandMaster;
        public Command ThirdCureCommandMaster
        {
            get
            {
                return thirdCureCommandMaster;
            }
            set
            {
                thirdCureCommandMaster = value;
            }
        }
        private Byte firstCurePerc = 85;
        public Byte FirstCurePerc
        {
            get
            {
                return firstCurePerc;
            }
            set
            {
                firstCurePerc = value;
            }
        }
        private Byte secondCurePerc = 65;
        public Byte SecondCurePerc
        {
            get
            {
                return secondCurePerc;
            }
            set
            {
                secondCurePerc = value;
            }
        }
        private Byte thirdCurePerc = 35;
        public Byte ThirdCurePerc
        {
            get
            {
                return thirdCurePerc;
            }
            set
            {
                thirdCurePerc = value;
            }
        }
    }
}