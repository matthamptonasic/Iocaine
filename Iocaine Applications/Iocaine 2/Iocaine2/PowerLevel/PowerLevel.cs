using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Char;
using Iocaine2.Data.Structures;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Properties;

namespace Iocaine2.Bots
{
    public sealed partial class PowerLevel : Bot
    {
        #region Enums
        private enum TASK_CODE
        {
            OK = 0,
            DISTANCE = 1,
            NOT_READY = 2,
            MP = 3,
            TP = 4,
            FIGHTING = 5,
            INVALID_PLAYER = 6,
            OTHER = 7
        }
        #endregion Enums

        #region Template Members
        private static PowerLevel instance = null;
        private static readonly Object padlock = new Object();
        #endregion Template Members

        #region Bot Specific Member
        private Boolean consumeItemEnable = true;
        private ArrayList QList = new ArrayList();
        private UInt32 lastHPP = 100;
        private List<PLCharacter> characterList;
        private List<Command> cureCmdList;
        private UInt16 currentZone;
        private Task convertTask;
        private Task chatMpReportTask;
        private Task chatHpReportTask;
        private Task chatCastTimeTask;
        private Task chatOutOfRangeTask;
        private Task command1Task;
        private Boolean command1DoNextRest = false;
        private Task command2Task;
        private Boolean command2DoNextRest = false;
        private Task command3Task;
        private Boolean command3DoNextRest = false;
        private Task command4Task;
        private Boolean command4DoNextRest = false;
        #endregion Bot Specific Member

        #region Constructor
        private PowerLevel()
        {
            base.name = "Powerlevel";
            instanceInit();
        }
        #endregion Constructor

        #region Template Functions
        public static PowerLevel Access
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new PowerLevel();
                    }
                    return instance;
                }
            }
        }
        public override void SetParams(Bots.BotParam iParam)
        {
            try
            {
                if (iParam == null)
                {
                    return;
                }
                Bots.PLParam prm = (Bots.PLParam)iParam;
                c_startButton = prm.StartButton;
                initParamsSet = true;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::SetParams: " + e.ToString());
            }
        }
        #endregion Template Functions

        #region Initialization
        private void instanceInit()
        {
            try
            {

            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::instanceInit: " + e.ToString());
            }
        }
        public void doInits()
        {
            try
            {
                if ((QList == null) || (QList.Count == 0))
                {
                    for (int ii = 0; ii < Statics.Settings.PowerLevel.NbOfQueues; ii++)
                    {
                        QList.Add(new Queue());
                    }
                }
                currentZone = MemReads.Self.get_zone_id();
                //Create task for Convert
                JobAbilCommand convertCmd = CommandManager.JAManager.GetCommand("Convert");
                if (convertCmd != null)
                {
                    if (convertTask != null)
                    {
                        convertTask.Kill();
                        convertTask = null;
                    }
                    convertTask = new Task(Task.TYPE.JA, MemReads.Self.get_name(true), convertCmd, 0,
                                                true, Statics.Settings.PowerLevel.SelfBuffQ);
                }
                //Create task for Mp Report
                RawCommand chatMpReportCommand = new RawCommand("MP Report", Statics.Settings.PowerLevel.ChatMpReportCmd);
                if (chatMpReportTask != null)
                {
                    chatMpReportTask.Kill();
                    chatMpReportTask = null;
                }
                chatMpReportTask = new Task(Task.TYPE.RAW_CMD, PlayerCache.Vitals.Name,
                                            chatMpReportCommand, (UInt32)Statics.Settings.PowerLevel.ChatMpReportTimer,
                                            Statics.Settings.PowerLevel.ChatMpReport, Statics.Settings.PowerLevel.NbOfQueues - 1);
                chatMpReportTask.Elapsed += new System.Timers.ElapsedEventHandler(RebuffTimer_Elapsed);
                if (Statics.Settings.PowerLevel.ChatMpReport)
                {
                    EnqueueTask(chatMpReportTask);
                }
                //Create task for Hp Report
                RawCommand chatHpReportCommand = new RawCommand("HP Report", Statics.Settings.PowerLevel.ChatHpReportCmd);
                if (chatHpReportTask != null)
                {
                    chatHpReportTask.Kill();
                    chatHpReportTask = null;
                }
                chatHpReportTask = new Task(Task.TYPE.RAW_CMD, MemReads.Self.get_name(true),
                                            chatHpReportCommand, 1000 * 60 * 5, false, Statics.Settings.PowerLevel.NbOfQueues - 1);
                //Create task for chat if cast time is long
                RawCommand chatCastTimeCommand = new RawCommand("Long Cast", Statics.Settings.PowerLevel.ChatCastTimeCmd);
                if (chatCastTimeTask != null)
                {
                    chatCastTimeTask.Kill();
                    chatCastTimeTask = null;
                }
                chatCastTimeTask = new Task(Task.TYPE.RAW_CMD, MemReads.Self.get_name(true),
                                            chatCastTimeCommand, 2000,
                                            Statics.Settings.PowerLevel.ChatCastTime, Statics.Settings.PowerLevel.NbOfQueues - 1);
                //Create task for chat if out of range
                RawCommand chatOutOfRangeCommand = new RawCommand("Out Of Range", Statics.Settings.PowerLevel.ChatLostCmd);
                if (chatOutOfRangeTask != null)
                {
                    chatOutOfRangeTask.Kill();
                    chatOutOfRangeTask = null;
                }
                chatOutOfRangeTask = new Task(Task.TYPE.RAW_CMD, MemReads.Self.get_name(true),
                                            chatOutOfRangeCommand, 2000,
                                            Statics.Settings.PowerLevel.ChatLost, Statics.Settings.PowerLevel.NbOfQueues - 1);
                //Create task for user defined Command 1
                RawCommand command1Command = new RawCommand("Command1", Statics.Settings.PowerLevel.Command1Cmd);
                if (command1Task != null)
                {
                    command1Task.Kill();
                    command1Task = null;
                }
                command1Task = new Task(Task.TYPE.RAW_CMD, MemReads.Self.get_name(true),
                                            command1Command, (UInt32)Statics.Settings.PowerLevel.Command1Timer,
                                            Statics.Settings.PowerLevel.Command1Enable, Statics.Settings.PowerLevel.NbOfQueues - 1);
                command1Task.Elapsed += new System.Timers.ElapsedEventHandler(RebuffTimer_Elapsed);
                if (Statics.Settings.PowerLevel.Command1Enable)
                {
                    EnqueueTask(command1Task);
                }
                //Create task for user defined Command 2
                RawCommand command2Command = new RawCommand("Command2", Statics.Settings.PowerLevel.Command2Cmd);
                if (command2Task != null)
                {
                    command2Task.Kill();
                    command2Task = null;
                }
                command2Task = new Task(Task.TYPE.RAW_CMD, MemReads.Self.get_name(true),
                                            command2Command, (UInt32)Statics.Settings.PowerLevel.Command2Timer,
                                            Statics.Settings.PowerLevel.Command2Enable, Statics.Settings.PowerLevel.NbOfQueues - 1);
                command2Task.Elapsed += new System.Timers.ElapsedEventHandler(RebuffTimer_Elapsed);
                if (Statics.Settings.PowerLevel.Command2Enable)
                {
                    EnqueueTask(command2Task);
                }
                //Create task for user defined Command 3
                RawCommand command3Command = new RawCommand("Command3", Statics.Settings.PowerLevel.Command3Cmd);
                if (command3Task != null)
                {
                    command3Task.Kill();
                    command3Task = null;
                }
                command3Task = new Task(Task.TYPE.RAW_CMD, MemReads.Self.get_name(true),
                                            command3Command, (UInt32)Statics.Settings.PowerLevel.Command3Timer,
                                            Statics.Settings.PowerLevel.Command3Enable, Statics.Settings.PowerLevel.NbOfQueues - 1);
                command3Task.Elapsed += new System.Timers.ElapsedEventHandler(RebuffTimer_Elapsed);
                if (Statics.Settings.PowerLevel.Command3Enable)
                {
                    EnqueueTask(command3Task);
                }
                //Create task for user defined Command 4
                RawCommand command4Command = new RawCommand("Command4", Statics.Settings.PowerLevel.Command4Cmd);
                if (command4Task != null)
                {
                    command4Task.Kill();
                    command4Task = null;
                }
                command4Task = new Task(Task.TYPE.RAW_CMD, MemReads.Self.get_name(true),
                                            command4Command, (UInt32)Statics.Settings.PowerLevel.Command4Timer,
                                            Statics.Settings.PowerLevel.Command4Enable, Statics.Settings.PowerLevel.NbOfQueues - 1);
                command4Task.Elapsed += new System.Timers.ElapsedEventHandler(RebuffTimer_Elapsed);
                if (Statics.Settings.PowerLevel.Command4Enable)
                {
                    EnqueueTask(command4Task);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::doInits: " + e.ToString());
            }
        }
        public void SetCharList(List<PLCharacter> iCharacterList)
        {
            try
            {
                characterList = iCharacterList;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::SetChatList: " + e.ToString());
            }
        }
        public void SetCuresList(List<Command> iCuresList)
        {
            try
            {
                cureCmdList = iCuresList;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::SetCuresList: " + e.ToString());
            }
        }
        #endregion Initialization

        #region Status Related
        public override void Pause(Boolean iNow = false)
        {
            try
            {
                Statics.FuncPtrs.SetStatusBoxPtr("Pausing bot", Statics.Fields.Yellow);
                base.Pause(iNow);
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::Pause: " + e.ToString());
            }
        }
        public override void Resume()
        {
            try
            {
                Statics.FuncPtrs.SetStatusBoxPtr("Resuming...", Statics.Fields.Green);
                base.Resume();
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL::Resume: " + e.ToString());
            }
        }
        public override void Stop()
        {
            try
            {
                Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot", Statics.Fields.Blue);
                base.Stop();
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL::Stop: " + e.ToString());
            }
        }
        protected override void runThreadFunction()
        {
            throw new NotImplementedException();
        }
        protected override bool checkStatus()
        {
            return true;
        }
        #endregion Status Related

        #region Main Threads
        public void EnqueueThreadFunction()
        {
            try
            {
                state = STATE.RUNNING;
                while (checkState())
                {
                    checkZoneChange();
                    LoggingFunctions.Debug("Entering the enqueue thread while loop", LoggingFunctions.DBG_SCOPE.PL);
                    PLCharacter worstNeed = null;
                    int worstNeedLevel = 0;
                    bool moreThanOneNeeded = false;
                    foreach (PLCharacter chr in characterList)
                    {
                        LoggingFunctions.Debug("Checking player " + chr.Name + "'s HP", LoggingFunctions.DBG_SCOPE.PL);
                        if (!chr.CureQueued)
                        {
                            LoggingFunctions.Debug("chr.CureQ'd is false", LoggingFunctions.DBG_SCOPE.PL);
                            LoggingFunctions.Debug("chr.Distance is " + chr.Distance.ToString(), LoggingFunctions.DBG_SCOPE.PL);
                            chr.Update_HP_Perc();
                            int cureNb = 0;
                            if ((chr.HpPerc <= chr.FirstCurePerc) && chr.Active && chr.Check_In_Range())
                            {
                                cureNb = 1;
                                if (chr.HpPerc <= chr.SecondCurePerc)
                                {
                                    cureNb = 2;
                                    if (chr.HpPerc <= chr.ThirdCurePerc)
                                    {
                                        if (chr.HpPerc != 0)
                                        {
                                            cureNb = 3;
                                        }
                                        else
                                        {
                                            cureNb = 0;
                                            //Add call to add raise here
                                        }
                                    }
                                }
                            }
                            if (cureNb == worstNeedLevel)
                            {
                                if (worstNeed != null)
                                {
                                    moreThanOneNeeded = true;
                                    if (worstNeed.Priority < chr.Priority)
                                    {
                                        worstNeed = chr;
                                    }
                                }
                            }
                            else if (cureNb > worstNeedLevel)
                            {
                                if (worstNeedLevel != 0)
                                {
                                    moreThanOneNeeded = true;
                                }
                                worstNeed = chr;
                                worstNeedLevel = cureNb;
                            }
                        }
                    }
                    if (worstNeed == null)
                    {
                        LoggingFunctions.Debug("No one needed cure, worstNeed is null", LoggingFunctions.DBG_SCOPE.PL);
                    }
                    else
                    {
                        LoggingFunctions.Debug("WorstNeedLevel is " + worstNeedLevel.ToString() + " which is from " + worstNeed.Name, LoggingFunctions.DBG_SCOPE.PL);
                    }
                    if (worstNeedLevel > 0)
                    {
                        //Someone is in need, so queue a cure
                        worstNeed.CureQueued = true;
                        int Qnb = 3 - worstNeedLevel;
                        Queue localQ = (Queue)QList[Qnb];
                        //Monitor.Enter(localQ);
                        switch (worstNeedLevel)
                        {
                            case 1:
                                localQ.Enqueue(WrapTask(worstNeed.FirstCureCommand, worstNeed, Qnb));
                                LoggingFunctions.Debug("Enquing cure level " + worstNeedLevel.ToString() + " of command " + worstNeed.FirstCureCommand.ToString() + " for player " + worstNeed.Name, LoggingFunctions.DBG_SCOPE.PL);
                                break;
                            case 2:
                                localQ.Enqueue(WrapTask(worstNeed.SecondCureCommand, worstNeed, Qnb));
                                LoggingFunctions.Debug("Enquing cure level " + worstNeedLevel.ToString() + " of command " + worstNeed.SecondCureCommand.ToString() + " for player " + worstNeed.Name, LoggingFunctions.DBG_SCOPE.PL);
                                break;
                            case 3:
                                localQ.Enqueue(WrapTask(worstNeed.ThirdCureCommand, worstNeed, Qnb));
                                LoggingFunctions.Debug("Enquing cure level " + worstNeedLevel.ToString() + " of command " + worstNeed.ThirdCureCommand.ToString() + " for player " + worstNeed.Name, LoggingFunctions.DBG_SCOPE.PL);
                                break;
                            default:
                                LoggingFunctions.Error("while enquing cure command, worstNeedLevel was " + worstNeedLevel.ToString());
                                break;
                        }
                        //Monitor.Exit(localQ);
                    }
                    if (!moreThanOneNeeded)
                    {
                        LoggingFunctions.Debug("End of enqueue loop, delaying " + Statics.Settings.PowerLevel.HealthUpdateFrequency + " ms", LoggingFunctions.DBG_SCOPE.PL);
                        IocaineFunctions.delay(Statics.Settings.PowerLevel.HealthUpdateFrequency);
                        LoggingFunctions.Debug("End of enqueue loop, done with delay.", LoggingFunctions.DBG_SCOPE.PL);
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL::EnqueueThreadFunction: " + e.ToString());
                MessageBox.Show("[ERROR] PL::EnqueueThreadFunction:\n" + e.ToString());
            }
        }
        public void DequeueThreadFunction()
        {
            try
            {
                state = STATE.RUNNING;
                lastHPP = MemReads.Self.Vitals.get_hp_percent();
                while (checkState())
                {
                    LoggingFunctions.Debug("Entering the dequeue thread while loop", LoggingFunctions.DBG_SCOPE.PL);
                    Statics.FuncPtrs.SetStatusBoxPtr("Checking for needed casts", Statics.Fields.Green);
                    Task readTask = null;
                    bool castCompleted = false;
                    //bool noCast_Distance = false;
                    //bool noCast_MP = false;
                    bool noCast_NotReady = false;
                    //bool noCast_Fighting = false;
                    //bool noCast_Other = false;
                    bool deferred = false;
                    bool allEmpty = true;
                    int taskCode = 0;
                    for (int ii = 0; ii < Statics.Settings.PowerLevel.NbOfQueues; ii++)
                    {
                        try
                        {
                            Queue localQ = (Queue)QList[ii];
                            //Monitor.Enter(localQ);
                            if (deferred && (ii == Statics.Settings.PowerLevel.FirstCureQ + 1))
                            {
                                //If we deferred 1 of the cure Q's and are going on to buffs, we should stop and check on cures again
                                LoggingFunctions.Debug("We deferred one of the cure Q's, so we're breaking and starting again.", LoggingFunctions.DBG_SCOPE.PL);
                                break;
                            }
                            deferred = false;
                            LoggingFunctions.Debug("Checking Q[" + ii.ToString() + "] which has " + localQ.Count.ToString() + " items", LoggingFunctions.DBG_SCOPE.PL);
                            if (localQ.Count > 0)
                            {
                                allEmpty = false;
                                int count = localQ.Count;
                                try
                                {
                                    for (int kk = 1; kk <= count; kk++)
                                    {
                                        checkZoneChange();
                                        readTask = (Task)localQ.Dequeue();
                                        taskCode = doTask(readTask);
                                        if (taskCode == (int)TASK_CODE.OK) //So we executed the command ok
                                        {
                                            LoggingFunctions.Debug("Executed command successfully.", LoggingFunctions.DBG_SCOPE.PL);
                                            if (readTask.Type == Task.TYPE.CURE)
                                            {
                                                LoggingFunctions.Debug("Task type was Cure, finding character to set CureQueued to false.", LoggingFunctions.DBG_SCOPE.PL);
                                                foreach (PLCharacter chr in characterList)
                                                {
                                                    if (chr.Name == readTask.Player)
                                                    {
                                                        chr.CureQueued = false;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                LoggingFunctions.Debug("Task was not of Cure type", LoggingFunctions.DBG_SCOPE.PL);
                                            }
                                            if (deferred)   //Need to bring the Q back to original order
                                            {
                                                LoggingFunctions.Debug("A task was deferred, going thru Q to reorder to original order.", LoggingFunctions.DBG_SCOPE.PL);
                                                //Q count is 1 less than it was to begin with, so we need to add 1 to it
                                                int tasksToRequeue = localQ.Count - kk + 1;
                                                for (int mm = 0; mm < tasksToRequeue; mm++)
                                                {
                                                    localQ.Enqueue(localQ.Dequeue());   //pop and push to end
                                                }
                                                LoggingFunctions.Debug("Q was reordered correctly.", LoggingFunctions.DBG_SCOPE.PL);
                                            }
                                            else
                                            {
                                                LoggingFunctions.Debug("No tasks were deferred, no re-ordering necessary.", LoggingFunctions.DBG_SCOPE.PL);
                                            }
                                            castCompleted = true;
                                            LoggingFunctions.Debug("All task casting post-processing completed, breaking loop.", LoggingFunctions.DBG_SCOPE.PL);
                                            break;
                                        }
                                        else if (taskCode == (int)TASK_CODE.INVALID_PLAYER)
                                        {
                                            LoggingFunctions.Timestamp("[WARNING] Attempted to perform " + readTask.CmdName + " on " + readTask.Player + ". Removing task.");
                                            //readTask.UseTask = false;
                                            readTask.Dispose();
                                            castCompleted = false;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                LoggingFunctions.Debug("Command was not executed correctly, re-Q'ing the task and setting the deferred flag.", LoggingFunctions.DBG_SCOPE.PL);
                                                localQ.Enqueue(readTask);
                                                deferred = true;
                                                if (taskCode == (int)TASK_CODE.DISTANCE)
                                                {
                                                    //noCast_Distance = true;
                                                }
                                                else if (taskCode == (int)TASK_CODE.MP)
                                                {
                                                    //noCast_MP = true;
                                                }
                                                else if (taskCode == (int)TASK_CODE.NOT_READY)
                                                {
                                                    noCast_NotReady = true;
                                                }
                                                else if (taskCode == (int)TASK_CODE.FIGHTING)
                                                {
                                                    //noCast_Fighting = true;
                                                }
                                                else if (taskCode == (int)TASK_CODE.OTHER)
                                                {
                                                    //noCast_Other = true;
                                                }
                                            }
                                            catch
                                            {
                                                LoggingFunctions.Error("re-enQ'ing task " + readTask.CmdName);
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    LoggingFunctions.Error("Caught exception in deQ while checking each task in Q");
                                    LoggingFunctions.Error("Qnb: " + ii);
                                    LoggingFunctions.Error("Q Count: " + count);
                                    LoggingFunctions.Error("Task: " + readTask.CmdName);
                                    LoggingFunctions.Error("Player: " + readTask.Player);
                                    LoggingFunctions.Error("Task Code: " + taskCode);
                                }
                            }
                            //Monitor.Exit(localQ);
                            if (castCompleted)
                            {
                                LoggingFunctions.Debug("Cast was completed, breaking from outter loop.", LoggingFunctions.DBG_SCOPE.PL);
                                break;
                            }
                        }
                        catch
                        {
                            LoggingFunctions.Error("Caught exception while checking each Q");
                            LoggingFunctions.Error("Qnb: " + ii);
                        }
                    }
                    LoggingFunctions.Debug("Checking if we need to follow.", LoggingFunctions.DBG_SCOPE.PL);
                    if (Statics.Settings.PowerLevel.ChatHpReport)
                    {
                        short tempHPP = MemReads.Self.Vitals.get_hp_percent();
                        LoggingFunctions.Debug("Checking HPP. Last: " + lastHPP + " Current: " + tempHPP, LoggingFunctions.DBG_SCOPE.PL);
                        if ((tempHPP < Statics.Settings.PowerLevel.ChatHpReportLevel) && (lastHPP >= Statics.Settings.PowerLevel.ChatHpReportLevel))
                        {
                            EnqueueTask(chatHpReportTask);
                        }
                        lastHPP = (uint)tempHPP;
                    }
                    consumeItem();
                    doFollow();
                    checkConvert();
                    //Our situations are:
                    //1. Didn't cast because a task wasn't ready yet
                    //  - We don't want to rest in this case because it will be ready soon
                    //2. Didn't cast because player was fighting and settings prevented
                    //  - We can rest in this case
                    //3. Didn't cast because player was out of range
                    //  - We want to rest in this case cause we don't know how long they'll be gone.
                    //4. Didn't cast because of MP (settings)
                    //  - We want to rest in this case.
                    if (!castCompleted)
                    {
                        if (!noCast_NotReady)
                        {
                            LoggingFunctions.Debug("Checking if we need to rest.", LoggingFunctions.DBG_SCOPE.PL);
                            checkHeal();
                        }
                        LoggingFunctions.Debug("No cast completed, waiting for " + Statics.Settings.PowerLevel.DequeuePollFrequency.ToString() + " ms", LoggingFunctions.DBG_SCOPE.PL);
                        IocaineFunctions.delay(Statics.Settings.PowerLevel.DequeuePollFrequency);
                        if (allEmpty)
                        {
                            IocaineFunctions.delay(Statics.Settings.PowerLevel.HealthUpdateFrequency);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL::DequeueThreadFunction: " + e.ToString());
                MessageBox.Show("[ERROR] PL::DequeueThreadFunction:\n" + e.ToString());
            }
        }
        #endregion Main Threads

        #region Task Related Functions
        public Task WrapTask(Command iCommand, PLCharacter iCharacter, int iPriority)
        {
            Task newTask = null;
            try
            {
                newTask = new Task(Task.TYPE.CURE, iCharacter.Name, iCommand, 0, true, iPriority);
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::WrapTask: " + e.ToString());
            }
            return newTask;
        }
        public void RebuffTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Task localTask = (Task)sender;
                Queue localQ = (Queue)QList[localTask.Priority - 1];
                LoggingFunctions.Debug("Timer elapsed for task " + localTask.CmdName, LoggingFunctions.DBG_SCOPE.PL);
                if (localTask.Cmd.Type == Command.CMD_TYPE.RAW_CMD)
                {
                    if (Statics.Settings.PowerLevel.ChatMpReport)
                    {
                        localQ.Enqueue(localTask);
                        LoggingFunctions.Debug("Q'ing task " + localTask.Cmd.Name + " to Q" + (localTask.Priority - 1).ToString(), LoggingFunctions.DBG_SCOPE.PL);
                    }
                }
                else if (((localTask == command1Task) && Statics.Settings.PowerLevel.Command1BeforeResting))
                {
                    command1DoNextRest = true;
                }
                else if (((localTask == command2Task) && Statics.Settings.PowerLevel.Command2BeforeResting))
                {
                    command2DoNextRest = true;
                }
                else if (((localTask == command3Task) && Statics.Settings.PowerLevel.Command3BeforeResting))
                {
                    command3DoNextRest = true;
                }
                else if (((localTask == command4Task) && Statics.Settings.PowerLevel.Command4BeforeResting))
                {
                    command4DoNextRest = true;
                }
                else
                {
                    if (localTask.UseTimer)
                    {
                        localQ.Enqueue(localTask);
                        LoggingFunctions.Debug("Q'ing task " + localTask.Cmd.Name + " to Q" + (localTask.Priority - 1).ToString(), LoggingFunctions.DBG_SCOPE.PL);
                    }
                    else
                    {
                        LoggingFunctions.Debug("Not Q'ing task " + localTask.Cmd.Name + " to Q" + (localTask.Priority - 1).ToString() + " because the UseTask property is " + localTask.UseTimer.ToString(), LoggingFunctions.DBG_SCOPE.PL);
                    }
                }
            }
            catch(Exception ex)
            {
                LoggingFunctions.Error("PL::RebuffTimer_Elapsed: " + ex.ToString());
            }
        }
        public void EnqueueTask(Task iTask)
        {
            try
            {
                if ((iTask.Priority > 0) && (iTask.Priority < 9))
                {
                    Queue localQ = (Queue)QList[iTask.Priority - 1];
                    localQ.Enqueue(iTask);
                }
                else
                {
                    LoggingFunctions.Error("When enquing task " + iTask.Cmd.Name + " of player " + iTask.Player + " with Q #" + iTask.Priority.ToString());
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::EnqueueTask: " + e.ToString());
            }
        }
        private int doTask(Task iTask)
        {
            try
            {
                checkZoneChange();
                LoggingFunctions.Debug("Entering doTask for " + iTask.CmdName, LoggingFunctions.DBG_SCOPE.PL);
                PLCharacter focusChar = (PLCharacter)characterList[Statics.Settings.PowerLevel.FocusCharacterIndex];
                uint currentMpp = (uint)MemReads.Self.Vitals.get_mp_percent();
                uint currentMp = (uint)MemReads.Self.Vitals.get_mp_current();
                bool lowMp = currentMpp <= Statics.Settings.PowerLevel.HealMpPercLow;
                bool midMp = currentMpp <= Statics.Settings.PowerLevel.HealMpPercHigh;
                bool highMp = !lowMp && !midMp;
                bool fighting = (focusChar.Status == (uint)FFXIEnums.STATUS.ATTACKING);
                bool[] localRestSettingsArray = (bool[])Statics.Settings.PowerLevel.RestingValueList[iTask.Priority];
                LoggingFunctions.Debug("currentMp: " + currentMp + ", currentMpp: " + currentMpp + ", lowMp: " + lowMp + ", midMp: " + midMp + ", highMp: " + highMp + ", fighting: " + fighting, LoggingFunctions.DBG_SCOPE.PL);
                LoggingFunctions.Debug("restSettings: inFight: " + localRestSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] +
                    ", standMpLow: " + localRestSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW] +
                    ", standMpHigh: " + localRestSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH] +
                    ", MpLow: " + localRestSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW] +
                    ", MpHigh: " + localRestSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH], LoggingFunctions.DBG_SCOPE.PL);
                if ((iTask.Type == Task.TYPE.CURE) || (iTask.Type == Task.TYPE.RAW_CMD))
                {
                    try
                    {
                        bool result = iTask.Cmd.Execute(iTask.Player);
                        if (iTask.UseTimer)
                        {
                            iTask.Start();
                            LoggingFunctions.Debug("Starting timer for task " + iTask.CmdName, LoggingFunctions.DBG_SCOPE.PL);
                        }
                        if (iTask == command1Task)
                        {
                            command1DoNextRest = false;
                        }
                        else if (iTask == command2Task)
                        {
                            command2DoNextRest = false;
                        }
                        else if (iTask == command3Task)
                        {
                            command3DoNextRest = false;
                        }
                        else if (iTask == command4Task)
                        {
                            command4DoNextRest = false;
                        }
                        return result ? (int)TASK_CODE.OK : (int)TASK_CODE.OTHER;
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("doTask: Caught exception while trying to doCommand a chat or command type:\n" + e.ToString());
                        LoggingFunctions.Error("doTask: Failed to do task '" + iTask.CmdName + "' on target '" + iTask.Player + "'.");
                        return (int)TASK_CODE.OTHER;
                    }
                }
                else
                {
                    try
                    {
                        if ((fighting && localRestSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT]) || !fighting)
                        {
                            LoggingFunctions.Debug("Either fighting and set to cast during fight or not fighting", LoggingFunctions.DBG_SCOPE.PL);
                            if ((lowMp && localRestSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_LOW])
                                || (midMp && localRestSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.MP_HIGH])
                                || highMp)
                            {
                                LoggingFunctions.Debug("mp settings such that the task can be executed.", LoggingFunctions.DBG_SCOPE.PL);
                                if (currentMp >= iTask.Cmd.MP)
                                {
                                    LoggingFunctions.Debug("Sufficient mp to cast. MP needed is " + iTask.Cmd.MP + ", current MP is " + currentMp, LoggingFunctions.DBG_SCOPE.PL);
                                    LoggingFunctions.Debug("convertTask.player: " + convertTask.Player, LoggingFunctions.DBG_SCOPE.PL);
                                    LoggingFunctions.Debug("iTask.player: " + iTask.Player + ", chr[0]: " + ((PLCharacter)characterList[0]).Name, LoggingFunctions.DBG_SCOPE.PL);
                                    PLCharacter taskPlayer = null;
                                    try
                                    {
                                        foreach (PLCharacter chr in characterList)
                                        {
                                            if (chr.Name == iTask.Player)
                                            {
                                                taskPlayer = chr;
                                            }
                                        }
                                        if (taskPlayer == null)
                                        {
                                            return (int)TASK_CODE.INVALID_PLAYER;
                                        }
                                        try
                                        {
                                            if (taskPlayer.Distance <= Statics.Settings.PowerLevel.MaxCastDistance)
                                            {
                                                LoggingFunctions.Debug("Character " + taskPlayer.Name + " is within the max cast range of " + Statics.Settings.PowerLevel.MaxCastDistance, LoggingFunctions.DBG_SCOPE.PL);
                                                //If we're doing a cure, check for cure upgrade
                                                if (iTask.Type == Task.TYPE.CURE)
                                                {
                                                    LoggingFunctions.Debug("Cast is a cure, doing upgrade checks", LoggingFunctions.DBG_SCOPE.PL);
                                                    taskPlayer.Update_HP_Perc();
                                                    int neededCureNb = 0;
                                                    Command neededCureCmd = null;
                                                    if (taskPlayer.HpPerc <= taskPlayer.FirstCurePerc)
                                                    {
                                                        neededCureNb = 1;
                                                        neededCureCmd = taskPlayer.FirstCureCommand;
                                                    }
                                                    if (taskPlayer.HpPerc <= taskPlayer.SecondCurePerc)
                                                    {
                                                        neededCureNb = 2;
                                                        neededCureCmd = taskPlayer.SecondCureCommand;
                                                    }
                                                    if (taskPlayer.HpPerc <= taskPlayer.ThirdCurePerc)
                                                    {
                                                        neededCureNb = 3;
                                                        neededCureCmd = taskPlayer.ThirdCureCommand;
                                                    }
                                                    if (neededCureNb > iTask.Priority)
                                                    {
                                                        iTask = WrapTask(neededCureCmd, taskPlayer, neededCureNb);
                                                    }
                                                }
                                                stand();
                                                Statics.FuncPtrs.SetStatusBoxPtr("Attempting to cast " + iTask.CmdName + " on " + iTask.Player, Statics.Fields.Green);
                                                LoggingFunctions.Timestamp("Attempting to cast " + iTask.CmdName + " on " + iTask.Player);
                                                //If enabled and this cast is long, send chat message first
                                                if ((iTask.Cmd.ExecTime >= Statics.Settings.PowerLevel.ChatCastTimeTime) && Statics.Settings.PowerLevel.ChatCastTime)
                                                {
                                                    doTask(chatCastTimeTask);
                                                }
                                                try
                                                {
                                                    if (iTask.Cmd.Execute(iTask.Player))
                                                    {
                                                        LoggingFunctions.Debug("task.cmd.doCommand was successful, checking if we need to restart the task.", LoggingFunctions.DBG_SCOPE.PL);
                                                        if (iTask.Type == Task.TYPE.CURE)   //We're curing someone
                                                        {
                                                            LoggingFunctions.Debug("It was a cure, nothing to restart", LoggingFunctions.DBG_SCOPE.PL);
                                                        }
                                                        else if (iTask.Type == Task.TYPE.BUFF)  //We're buffing someone, will need to reset timer after if enabled
                                                        {
                                                            if (iTask.UseTimer)
                                                            {
                                                                iTask.Start();
                                                                LoggingFunctions.Debug("Starting timer for task " + iTask.CmdName, LoggingFunctions.DBG_SCOPE.PL);
                                                            }
                                                        }
                                                        return (int)TASK_CODE.OK;
                                                    }
                                                    else
                                                    {
                                                        Statics.FuncPtrs.SetStatusBoxPtr(iTask.CmdName + " not ready, waiting...", Statics.Fields.Green);
                                                        return (int)TASK_CODE.NOT_READY;
                                                    }
                                                }
                                                catch
                                                {
                                                    LoggingFunctions.Error("doTask: Caught exception in inner section.");
                                                    LoggingFunctions.Error("doTask: Failed to do task " + iTask.CmdName);
                                                    return (int)TASK_CODE.OTHER;
                                                }
                                            }
                                            else
                                            {
                                                LoggingFunctions.Debug("Powerlevel.doTask(" + iTask.CmdName + ") returned false because chr " + taskPlayer.Name + " is " + taskPlayer.Distance + " away", LoggingFunctions.DBG_SCOPE.PL);
                                                return (int)TASK_CODE.DISTANCE;
                                            }
                                        }
                                        catch
                                        {
                                            LoggingFunctions.Error("doTask: Caught exception after selecting character.");
                                            LoggingFunctions.Error("doTask: Failed to do task " + iTask.CmdName);
                                            LoggingFunctions.Error("doTask: Player casting on is " + iTask.Player);
                                            return (int)TASK_CODE.OTHER;
                                        }
                                    }
                                    catch
                                    {
                                        LoggingFunctions.Error("doTask: Caught exception in outter character selection block.");
                                        LoggingFunctions.Error("doTask: Failed to do task " + iTask.CmdName);
                                        return (int)TASK_CODE.OTHER;
                                    }
                                }
                                //Not enough mp to cast, try to downgrade
                                else
                                {
                                    if (iTask.Type == Task.TYPE.CURE)
                                    {
                                        LoggingFunctions.Debug("Trying to downgrade " + iTask.CmdName, LoggingFunctions.DBG_SCOPE.PL);
                                        Task newTask = downGradeCure(iTask);
                                        if ((newTask == iTask) || !(currentMp >= newTask.Cmd.MP))
                                        {
                                            //If the downgrade function returned same task, we can't downgrade so we have to heal (not enough mp to cast smallest cure)
                                            Statics.FuncPtrs.SetStatusBoxPtr("Not enough MP to cast " + iTask.CmdName + ". Resting to " + iTask.Cmd.MP, Statics.Fields.Green);
                                            checkHeal(iTask.Cmd.MP);
                                            if (iTask.Cmd.Execute(iTask.Player))
                                            {
                                                return (int)TASK_CODE.OK;
                                            }
                                            else
                                            {
                                                return (int)TASK_CODE.OTHER;
                                            }
                                        }
                                        else
                                        {
                                            LoggingFunctions.Debug("Downgraded cure to " + newTask.CmdName, LoggingFunctions.DBG_SCOPE.PL);
                                            if (iTask.Cmd.Execute(iTask.Player))
                                            {
                                                return (int)TASK_CODE.OK;
                                            }
                                            else
                                            {
                                                return (int)TASK_CODE.OTHER;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return (int)TASK_CODE.OTHER;
                                    }
                                }
                            }
                            else
                            {
                                LoggingFunctions.Debug("Powerlevel.doTask(" + iTask.CmdName + ") returned false because mp is " + currentMp + " and settings are such that Q" + iTask.Priority + " will not cast at this level", LoggingFunctions.DBG_SCOPE.PL);
                                return (int)TASK_CODE.MP;
                            }
                        }
                        else
                        {
                            LoggingFunctions.Debug("Powerlevel.doTask(" + iTask.CmdName + ") returned false because character is fighting and this task is set to not be cast while fighting.", LoggingFunctions.DBG_SCOPE.PL);
                            return (int)TASK_CODE.FIGHTING;
                        }
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("doTask: Caught exception in main block");
                        LoggingFunctions.Error("doTask: Failed to do task " + iTask.CmdName);
                        LoggingFunctions.Error("doTask: Exception: " + e.ToString());
                        MessageBox.Show("[ERROR] Caught exception in Powerlevel.doTask main block\nFailed to do task " + iTask.CmdName + "\n" + e.ToString());
                        return (int)TASK_CODE.OTHER;
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::doTask: " + e.ToString());
                return (int)TASK_CODE.OTHER;
            }
        }
        #endregion Task Related Functions

        #region Task Update Procedures
        public void UpdateMpReportEnable()
        {
            try
            {
                if (!chatMpReportTask.UseTimer && Statics.Settings.PowerLevel.ChatMpReport)
                {
                    EnqueueTask(chatMpReportTask);
                }
                chatMpReportTask.UseTimer = Statics.Settings.PowerLevel.ChatMpReport;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateMpReportEnable: " + e.ToString());
            }
        }
        public void UpdateMpReportTime()
        {
            try
            {
                if (chatMpReportTask.Interval != Statics.Settings.PowerLevel.ChatMpReportTimer)
                {
                    chatMpReportTask.Interval = Statics.Settings.PowerLevel.ChatMpReportTimer > 0 ? Statics.Settings.PowerLevel.ChatMpReportTimer : 3 * 60 * 1000;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateMpReportTime: " + e.ToString());
            }
        }
        public void UpdateMpReportCommand()
        {
            try
            {
                ((RawCommand)chatMpReportTask.Cmd).Text = Statics.Settings.PowerLevel.ChatMpReportCmd;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateMpReportCommand: " + e.ToString());
            }
        }
        public void UpdateCastTimeReportCommand()
        {
            try
            {
                ((RawCommand)chatCastTimeTask.Cmd).Text = Statics.Settings.PowerLevel.ChatCastTimeCmd;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateCastTimeReportCommand: " + e.ToString());
            }
        }
        public void UpdateCastTimeReportTime()
        {
            try
            {
                if (chatCastTimeTask.Interval != Statics.Settings.PowerLevel.ChatCastTimeTime)
                {
                    chatCastTimeTask.Interval = Statics.Settings.PowerLevel.ChatCastTimeTime > 0 ? Statics.Settings.PowerLevel.ChatCastTimeTime : 3 * 60 * 1000;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateCastTimeReportTime: " + e.ToString());
            }
        }
        public void UpdateOutOfRangeCommand()
        {
            try
            {
                ((RawCommand)chatOutOfRangeTask.Cmd).Text = Statics.Settings.PowerLevel.ChatLostCmd;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateOutOfRangeCommand: " + e.ToString());
            }
        }
        public void UpdateOurOfRangeCommandEnable()
        {
            try
            {
                chatOutOfRangeTask.UseTimer = Statics.Settings.PowerLevel.ChatLost;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateOurOfRangeCommandEnable: " + e.ToString());
            }
        }
        public void UpdateCommand1Enable()
        {
            try
            {
                bool needToEnqueue = (!command1Task.UseTimer && Statics.Settings.PowerLevel.Command1Enable);
                command1Task.UseTimer = Statics.Settings.PowerLevel.Command1Enable;
                if (needToEnqueue)
                {
                    if (Statics.Settings.PowerLevel.Command1BeforeResting)
                    {
                        command1DoNextRest = true;
                    }
                    else
                    {
                        EnqueueTask(command1Task);
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateCommand1Enable: " + e.ToString());
            }
        }
        public void UpdateCommand1Command()
        {
            try
            {
                ((RawCommand)command1Task.Cmd).Text = Statics.Settings.PowerLevel.Command1Cmd;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateCommand1Command: " + e.ToString());
            }
        }
        public void UpdateCommand1Timer()
        {
            try
            {
                if (command1Task.Interval != Statics.Settings.PowerLevel.Command1Timer)
                {
                    command1Task.Interval = Statics.Settings.PowerLevel.Command1Timer > 0 ? Statics.Settings.PowerLevel.Command1Timer : 3 * 60 * 1000;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateCommand1Timer: " + e.ToString());
            }
        }
        public void UpdateCommand2Enable()
        {
            try
            {
                bool needToEnqueue = (!command2Task.UseTimer && Statics.Settings.PowerLevel.Command2Enable);
                command2Task.UseTimer = Statics.Settings.PowerLevel.Command2Enable;
                if (needToEnqueue)
                {
                    if (Statics.Settings.PowerLevel.Command2BeforeResting)
                    {
                        command2DoNextRest = true;
                    }
                    else
                    {
                        EnqueueTask(command2Task);
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateCommand2Enable: " + e.ToString());
            }
        }
        public void UpdateCommand2Command()
        {
            try
            {
                ((RawCommand)command2Task.Cmd).Text = Statics.Settings.PowerLevel.Command2Cmd;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateCommand2Command: " + e.ToString());
            }
        }
        public void UpdateCommand2Timer()
        {
            try
            {
                if (command2Task.Interval != Statics.Settings.PowerLevel.Command2Timer)
                {
                    command2Task.Interval = Statics.Settings.PowerLevel.Command2Timer > 0 ? Statics.Settings.PowerLevel.Command2Timer : 3 * 60 * 1000;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateCommand2Timer: " + e.ToString());
            }
        }
        public void UpdateCommand3Enable()
        {
            try
            {
                bool needToEnqueue = (!command3Task.UseTimer && Statics.Settings.PowerLevel.Command3Enable);
                command3Task.UseTimer = Statics.Settings.PowerLevel.Command3Enable;
                if (needToEnqueue)
                {
                    if (Statics.Settings.PowerLevel.Command3BeforeResting)
                    {
                        command3DoNextRest = true;
                    }
                    else
                    {
                        EnqueueTask(command3Task);
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateCommand3Enable: " + e.ToString());
            }
        }
        public void UpdateCommand3Command()
        {
            try
            {
                ((RawCommand)command3Task.Cmd).Text = Statics.Settings.PowerLevel.Command3Cmd;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateCommand3Command: " + e.ToString());
            }
        }
        public void UpdateCommand3Timer()
        {
            try
            {
                if (command3Task.Interval != Statics.Settings.PowerLevel.Command3Timer)
                {
                    command3Task.Interval = Statics.Settings.PowerLevel.Command3Timer > 0 ? Statics.Settings.PowerLevel.Command3Timer : 3 * 60 * 1000;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateCommand3Timer: " + e.ToString());
            }
        }
        public void UpdateCommand4Enable()
        {
            try
            {
                bool needToEnqueue = (!command4Task.UseTimer && Statics.Settings.PowerLevel.Command4Enable);
                command4Task.UseTimer = Statics.Settings.PowerLevel.Command4Enable;
                if (needToEnqueue)
                {
                    if (Statics.Settings.PowerLevel.Command4BeforeResting)
                    {
                        command4DoNextRest = true;
                    }
                    else
                    {
                        EnqueueTask(command4Task);
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateCommand4Enable: " + e.ToString());
            }
        }
        public void UpdateCommand4Command()
        {
            try
            {
                ((RawCommand)command4Task.Cmd).Text = Statics.Settings.PowerLevel.Command4Cmd;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateCommand4Command: " + e.ToString());
            }
        }
        public void UpdateCommand4Timer()
        {
            try
            {
                if (command4Task.Interval != Statics.Settings.PowerLevel.Command4Timer)
                {
                    command4Task.Interval = Statics.Settings.PowerLevel.Command4Timer > 0 ? Statics.Settings.PowerLevel.Command4Timer : 3 * 60 * 1000;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::UpdateCommand4Timer: " + e.ToString());
            }
        }
        #endregion Task Update Procedures

        #region Utility Functions
        private Task downGradeCure(Task iCureTask)
        {
            Command newCure;
            try
            {
                foreach (PLCharacter chr in characterList)
                {
                    if (chr.Name == iCureTask.Player)
                    {
                        if (iCureTask.Cmd == chr.ThirdCureCommand)
                        {
                            newCure = chr.SecondCureCommand;
                            return WrapTask(newCure, chr, iCureTask.Priority);
                        }
                        else if (iCureTask.Cmd == chr.SecondCureCommand)
                        {
                            newCure = chr.FirstCureCommand;
                            return WrapTask(newCure, chr, iCureTask.Priority);
                        }
                        else
                        {
                            return iCureTask;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::downGradeCure: " + e.ToString());
            }
            return iCureTask;
        }
        private void checkHeal(int iMinMpNeeded)
        {
            try
            {
                checkZoneChange();
                LoggingFunctions.Debug("Doing forced checkHeal to get " + iMinMpNeeded + " mp", LoggingFunctions.DBG_SCOPE.PL);
                if (command1DoNextRest && Statics.Settings.PowerLevel.Command1Enable)
                {
                    doTask(command1Task);
                }
                if (command2DoNextRest && Statics.Settings.PowerLevel.Command2Enable)
                {
                    doTask(command2Task);
                }
                if (command3DoNextRest && Statics.Settings.PowerLevel.Command3Enable)
                {
                    doTask(command3Task);
                }
                if (command4DoNextRest && Statics.Settings.PowerLevel.Command4Enable)
                {
                    doTask(command4Task);
                }
                while ((MemReads.Self.Vitals.get_mp_current() < iMinMpNeeded) && checkState())
                {
                    IocaineFunctions.delay(200);
                    if (MemReads.Self.get_status() != (int)FFXIEnums.STATUS.HEALING)
                    {
                        IocaineFunctions.keys("/heal on");
                    }
                    IocaineFunctions.delay(1500);
                }
                stand();
                IocaineFunctions.delay(1500);
                return;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::checkHeal(int): " + e.ToString());
            }
        }
        private void checkHeal()
        {
            try
            {
                LoggingFunctions.Debug("Entering the checkHeal() function.", LoggingFunctions.DBG_SCOPE.PL);
                PLCharacter focusChar = (PLCharacter)characterList[Statics.Settings.PowerLevel.FocusCharacterIndex];
                int currentMpp;
                currentMpp = MemReads.Self.Vitals.get_mp_percent();
                LoggingFunctions.Debug("currentMpp is " + currentMpp, LoggingFunctions.DBG_SCOPE.PL);
                bool lowMp = currentMpp <= Statics.Settings.PowerLevel.HealMpPercLow;
                LoggingFunctions.Debug("lowMp is " + lowMp, LoggingFunctions.DBG_SCOPE.PL);
                bool midMp = currentMpp <= Statics.Settings.PowerLevel.HealMpPercHigh;
                LoggingFunctions.Debug("midMp is " + midMp, LoggingFunctions.DBG_SCOPE.PL);
                bool fighting = focusChar.Status == (int)FFXIEnums.STATUS.ATTACKING;
                LoggingFunctions.Debug("fighting is " + fighting, LoggingFunctions.DBG_SCOPE.PL);
                bool rest = false;
                bool needToCast = false;
                bool taskIsChat = false;
                LoggingFunctions.Debug("Checking for heal when mp is " + currentMpp + "% and focus char " + focusChar.Name + " fighting = " + fighting, LoggingFunctions.DBG_SCOPE.PL);

                //First, check if its ok to rest in this situation based on rest settings.
                if (Statics.Settings.PowerLevel.NeverRest)
                {
                    rest = false;
                }
                else if ((fighting && Statics.Settings.PowerLevel.RestInFight) || !fighting)
                {
                    if ((lowMp && Statics.Settings.PowerLevel.RestInFightLessLower) || (midMp && Statics.Settings.PowerLevel.RestInFightLessUpper) || Statics.Settings.PowerLevel.RestInFightAlways)
                    {
                        if ((MemReads.Self.Vitals.get_mp_max() - MemReads.Self.Vitals.get_mp_current()) > Statics.Settings.PowerLevel.UpperMpMargin)
                        {
                            rest = true;
                        }
                        else
                        {
                            rest = false;
                        }
                    }
                }
                needToCast = false;
                if (rest && !needToCast)
                {
                    if (!checkZoneChange())
                    {
                        //heal
                        if (command1DoNextRest && Statics.Settings.PowerLevel.Command1Enable)
                        {
                            doTask(command1Task);
                        }
                        if (command2DoNextRest && Statics.Settings.PowerLevel.Command2Enable)
                        {
                            doTask(command2Task);
                        }
                        if (command3DoNextRest && Statics.Settings.PowerLevel.Command3Enable)
                        {
                            doTask(command3Task);
                        }
                        if (command4DoNextRest && Statics.Settings.PowerLevel.Command4Enable)
                        {
                            doTask(command4Task);
                        }
                        Statics.FuncPtrs.SetStatusBoxPtr("Resting", Statics.Fields.Green);
                        while ((MemReads.Self.Vitals.get_mp_percent() < 100) && checkState())
                        {
                            LoggingFunctions.Debug("Entering first heal while loop", LoggingFunctions.DBG_SCOPE.PL);
                            IocaineFunctions.delay(200);
                            if (MemReads.Self.get_status() != (int)FFXIEnums.STATUS.HEALING)
                            {
                                IocaineFunctions.keys("/heal on");
                            }
                            IocaineFunctions.delay(1500);
                            //Check Q's for ppl in need
                            for (int ii = 0; ii < Statics.Settings.PowerLevel.NbOfQueues; ii++)
                            {
                                bool[] localRestSettingsArray = (bool[])Statics.Settings.PowerLevel.RestingValueList[ii];
                                Queue localQ = (Queue)QList[ii];
                                LoggingFunctions.Debug("Q" + ii + " count is " + localQ.Count + " while healing", LoggingFunctions.DBG_SCOPE.PL);
                                if (localQ.Count > 0)
                                {
                                    //Check to see if we need to stand and cast for this Q
                                    if ((localRestSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.IN_FIGHT] && fighting) || !fighting)
                                    {
                                        if ((lowMp && localRestSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_LOW])
                                            || (midMp && localRestSettingsArray[(int)Iocaine_2_Form.PL_REST_ARR_INX.STAND_HIGH])
                                            || (!lowMp && !midMp))
                                        {
                                            int count = localQ.Count;
                                            for (int mm = 0; mm < count; mm++)
                                            {
                                                //need to check if they're in range
                                                //Monitor.Enter(localQ);
                                                Task localTask = (Task)localQ.Peek();
                                                if (localTask.Type == Task.TYPE.RAW_CMD)
                                                {
                                                    needToCast = true;
                                                    taskIsChat = true;
                                                    break;
                                                }
                                                PLCharacter localChar = null;
                                                foreach (PLCharacter chr in characterList)
                                                {
                                                    if (chr.Name == localTask.Player)
                                                    {
                                                        localChar = chr;
                                                        break;
                                                    }
                                                }
                                                LoggingFunctions.Debug("Character " + localChar.Name + " is in range, checking mp for cast", LoggingFunctions.DBG_SCOPE.PL);
                                                LoggingFunctions.Debug("MP for cast " + localTask.CmdName + " is " + localTask.Cmd.MP + ", current is " + MemReads.Self.Vitals.get_mp_current(), LoggingFunctions.DBG_SCOPE.PL);
                                                if ((localChar.Distance <= Statics.Settings.PowerLevel.MaxCastDistance) && (localTask.Cmd.MP < MemReads.Self.Vitals.get_mp_current()))
                                                {
                                                    needToCast = true;
                                                    break;
                                                }
                                                else
                                                {
                                                    localTask = (Task)localQ.Dequeue();
                                                    localQ.Enqueue(localTask);
                                                }
                                                //Monitor.Exit(localQ);
                                            }
                                            if (!needToCast)
                                            {
                                                discardCures(ii);
                                            }
                                        }
                                        else
                                        {
                                            LoggingFunctions.Debug("Not getting up to cast due to character settings being to not stand and cast with this mp level", LoggingFunctions.DBG_SCOPE.PL);
                                            discardCures(ii);
                                        }
                                    }
                                    else
                                    {
                                        LoggingFunctions.Debug("Not getting up to cast due to character fighting and settings to not stand and cast this task while fighing", LoggingFunctions.DBG_SCOPE.PL);
                                        //We discard any cures so that if first cure is Q'd and we don't cast now
                                        // and player gets hit worse, we can enqueue another higher cure
                                        discardCures(ii);
                                    }
                                }
                                if (needToCast)
                                {
                                    break;
                                }
                            }
                            if (needToCast)
                            {
                                break;
                            }
                            if (doFollow())
                            {
                                break;
                            }
                        }
                    }
                    if (!taskIsChat)
                    {
                        stand();
                        IocaineFunctions.delay(1500);
                    }
                    return;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("PL::checkHeal: " + e.ToString());
            }
        }
        private void discardCures(int Qnb)
        {
            Task tsk;
            try
            {
                Queue localQ = (Queue)QList[Qnb];
                int Qcount = localQ.Count;
                LoggingFunctions.Debug("Checking " + Qcount + " items in Q" + Qnb + " to discard Cures", LoggingFunctions.DBG_SCOPE.PL);
                for (int ii = 0; ii < Qcount; ii++)
                {
                    //Monitor.Enter(localQ);
                    tsk = (Task)localQ.Dequeue();
                    if (tsk.Type == Task.TYPE.CURE)
                    {
                        foreach (PLCharacter chr in characterList)
                        {
                            if (chr.Name == tsk.Player)
                            {
                                chr.CureQueued = false;
                            }
                        }
                        LoggingFunctions.Debug("Removing cure command " + tsk.CmdName + " from Q" + Qnb, LoggingFunctions.DBG_SCOPE.PL);
                    }
                    else
                    {
                        localQ.Enqueue(tsk);
                    }
                    //Monitor.Exit(localQ);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::discardCures: " + e.ToString());
            }
        }
        private bool checkConvert()
        {
            try
            {
                if (!checkHasConvert())
                {
                    return false;
                }
                else
                {
                    if (Statics.Settings.PowerLevel.ConvertEnable)
                    {
                        if (MemReads.Self.Vitals.get_mp_current() <= Statics.Settings.PowerLevel.ConvertMpThreshold)
                        {
                            PLCharacter focusChar = (PLCharacter)characterList[Statics.Settings.PowerLevel.FocusCharacterIndex];
                            bool fighting = focusChar.Status == (int)FFXIEnums.STATUS.ATTACKING;
                            if (!fighting || (fighting && Statics.Settings.PowerLevel.ConvertEnInFight))
                            {
                                if (convertTask.Cmd.Ready)
                                {
                                    //doingConvert = true;
                                    //This means we're ready to convert.
                                    //Now we'll rest/cure to full and convert
                                    int hpCurrent = MemReads.Self.Vitals.get_hp_current();
                                    int hpMax = MemReads.Self.Vitals.get_hp_max();
                                    if (Statics.Settings.PowerLevel.ConvertCureRestFullFirst
                                        && (hpCurrent < (hpMax - Statics.Settings.PowerLevel.ConvertCureRestFullMargin)))
                                    {
                                        int mpCurrent = MemReads.Self.Vitals.get_mp_current();
                                        //So we want to get full HP first
                                        while ((hpCurrent < (hpMax - Statics.Settings.PowerLevel.ConvertCureRestFullMargin)) && checkState())
                                        {
                                            String toDo = "nothing";
                                            int neededMp = 0;
                                            if (((hpMax - hpCurrent) > 200) && (PlayerCache.Vitals.MainJobLvl >= 48))
                                            {
                                                //Do a cure 4
                                                toDo = "Cure IV";
                                                neededMp = 88;
                                            }
                                            else if ((hpMax - hpCurrent) > 100)
                                            {
                                                //Do a cure 3
                                                toDo = "Cure III";
                                                neededMp = 46;
                                            }
                                            else if ((hpMax - hpCurrent) > 35)
                                            {
                                                //do a cure 2
                                                toDo = "Cure II";
                                                neededMp = 24;
                                            }
                                            else
                                            {
                                                //Do a cure 1
                                                toDo = "Cure";
                                                neededMp = 8;
                                            }
                                            if (mpCurrent < neededMp)
                                            {
                                                checkHeal(neededMp);
                                            }
                                            Command localCure = (Command)cureCmdList[0];
                                            foreach (Command cmd in cureCmdList)
                                            {
                                                if (cmd.Name == toDo)
                                                {
                                                    localCure = cmd;
                                                    break;
                                                }
                                            }
                                            localCure.Execute(MemReads.Self.get_name(true));
                                            hpCurrent = MemReads.Self.Vitals.get_hp_current();
                                            mpCurrent = MemReads.Self.Vitals.get_mp_current();
                                        }
                                    }
                                    if (checkState())
                                    {
                                        //doingConvert = false;
                                        int result = doTask(convertTask);
                                        if (result == (int)TASK_CODE.OK)
                                        {
                                            return true;
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::checkConvert: " + e.ToString());
                return false;
            }
        }
        private bool checkHasConvert()
        {
            try
            {
                if ((PlayerCache.Vitals.MainJob == (short)FFXIEnums.JOBS.RDM) && (PlayerCache.Vitals.MainJobLvl >= 40))
                {
                    return true;
                }
                else if ((PlayerCache.Vitals.SubJob == (short)FFXIEnums.JOBS.RDM) && (PlayerCache.Vitals.SubJobLvl >= 40))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::checkHasConvert: " + e.ToString());
                return false;
            }
        }
        private bool doFollow()
        {
            bool moved = false;
            try
            {
                if (Statics.Settings.PowerLevel.FollowCharacterIndex != -1)
                {
                    PLCharacter charToFollow = (PLCharacter)characterList[Statics.Settings.PowerLevel.FollowCharacterIndex];
                    target(charToFollow.Name);
                    //Now he should be targeted
                    if (MemReads.Target.get_distance() > Statics.Settings.PowerLevel.FollowDistanceUpper)
                    {
                        moved = true;
                        stand();
                        float lastPosX = MemReads.Self.Position.get_x();
                        Statics.FuncPtrs.SetStatusBoxPtr("Following " + charToFollow.Name + ", distance = " + charToFollow.Distance, Statics.Fields.Green);
                        IocaineFunctions.keys("/follow " + charToFollow.Name);
                        do
                        {
                            checkZoneChange();
                            stand();
                            IocaineFunctions.delay(400);
                            float posX = MemReads.Self.Position.get_x();
                            if ((posX == lastPosX) || (MemReads.Target.get_name() != charToFollow.Name))
                            {
                                target(charToFollow.Name);
                                IocaineFunctions.keys("/follow " + charToFollow.Name);
                            }
                            lastPosX = posX;
                            LoggingFunctions.Debug("Distance to target is now " + MemReads.Target.get_distance(), LoggingFunctions.DBG_SCOPE.PL);
                        }
                        while ((MemReads.Target.get_distance() > Statics.Settings.PowerLevel.FollowDistance) && checkState());
                        IocaineFunctions.keyDown(Keys.NumPad7, 400);
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::doFollow: " + e.ToString());
            }
            return moved;
        }
        private void target(String targetName)
        {
            LoggingFunctions.Debug("Targetting " + targetName, LoggingFunctions.DBG_SCOPE.PL);
            uint timeout = 10;
            uint cnt = 0;
            try
            {
                while (true)
                {
                    String tempName = MemReads.Target.get_name();
                    if (tempName != targetName)
                    {
                        IocaineFunctions.keys("/target " + targetName);
                        IocaineFunctions.delay(300);
                    }
                    tempName = MemReads.Target.get_name();
                    LoggingFunctions.Debug("Trying to target " + targetName + " and read target name of " + tempName, LoggingFunctions.DBG_SCOPE.PL);
                    if (tempName != targetName)
                    {
                        if (checkZoneChange())
                        {
                            cnt = 0;
                        }
                        if ((cnt == timeout) && chatOutOfRangeTask.UseTimer)
                        {
                            //indicate a "lost" procedure
                            EnqueueTask(chatOutOfRangeTask);
                            break;
                        }
                        IocaineFunctions.delay(1000);
                        cnt++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::target: " + e.ToString());
            }
        }
        private void stand()
        {
            try
            {
                LoggingFunctions.Debug("Standing up.", LoggingFunctions.DBG_SCOPE.PL);
                if (MemReads.Self.get_status() == (int)FFXIEnums.STATUS.HEALING)
                {
                    do
                    {
                        IocaineFunctions.keyDown(Keys.NumPad8, 500);
                    } while (MemReads.Self.get_status() == (int)FFXIEnums.STATUS.HEALING);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::stand: " + e.ToString());
            }
        }
        private bool checkZoneChange()
        {
            try
            {
                UInt16 newZone = MemReads.Self.get_zone_id();
                if (currentZone != newZone)
                {
                    Statics.FuncPtrs.SetStatusBoxPtr(("Zone change, delaying " + Statics.Settings.PowerLevel.ZoneTimer + " seconds..."), Statics.Fields.Yellow);
                    IocaineFunctions.delay(Statics.Settings.PowerLevel.ZoneTimer * 1000);
                    Statics.FuncPtrs.SetStatusBoxPtr("Resuming", Statics.Fields.Green);
                    currentZone = newZone;
                    foreach (PLCharacter chr in characterList)
                    {
                        chr.Update_Pointer();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::checkZoneChange: " + e.ToString());
                return false;
            }
        }
        private void consumeItem()
        {
            try
            {
                if (consumeItemEnable)
                {
                    Inventory.Containers.Bag.RebuildLists();
                    foreach (Inventory.Item itm in Statics.Datasets.IdleConsumables)
                    {
                        if (Inventory.Containers.Bag.Contains(itm))
                        {
                            IocaineFunctions.keys("/item \"" + itm.Name + "\" <me>");
                            IocaineFunctions.delay(5000);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("PL::consumeItem: " + e.ToString());
            }
        }
        #endregion Utility Functions
    }
}
