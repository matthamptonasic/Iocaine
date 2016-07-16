using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Char;
using Iocaine2.Data.Client;
using Iocaine2.Data.Structures;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;

namespace Iocaine2.Bots
{
    public static class TeachersAssistant
    {
        #region Enums
        public enum PlayerState
        {
            Active = 0,
            Inactive = 1,
            OutOfRange = 2
        }
        #endregion Enums
        #region Init
        public static void init(Iocaine_2_Form.TA_redrawPlayerLBDelegate iRedrawPlayerLBCallBack)
        {
            redrawPlayerLBCallBack = iRedrawPlayerLBCallBack;
        }
        #endregion Init
        #region Members
        #region Functional Members
        private static byte taState = 0;  //0: stopped, 1: running, 2: user paused, 3: program paused
        private static Iocaine_2_Form.TA_redrawPlayerLBDelegate redrawPlayerLBCallBack;
        private static uint playerActivePollPeriod = 2500;
        private static Thread playerStatusCheckThread = null;
        private static Boolean consumeItemEnable = true;
        #endregion Functional Members
        #region Settings Members
        private static String playerName = "";
        private static List<Character> playerList = new List<Character>();
        private static List<PlayerState> playerStateList = new List<PlayerState>();
        private static UInt32 playerPtr = 0;
        private static bool playerActive = false;
        private static bool follow = false;
        private static int followDistance = 0;
        private static int wshpp = 10;
        private static String wsCommand = "";
        #endregion Settings Members
        #region Trigger Lists
        private static List<TriggerPlayerStatusChange> triggerPlayerStatusChangeList = new List<TriggerPlayerStatusChange>();
        #endregion Trigger Lists
        #region Trigger Queues
        private static Queue<Trigger> blockingTriggerQueue = new Queue<Trigger>();
        private static Queue<Trigger> nonBlockingTriggerQueue = new Queue<Trigger>();
        #endregion Trigger Queues
        #region Properties
        public static byte TAState
        {
            get
            {
                return taState;
            }
        }
        public static List<Character> PlayerList
        {
            get
            {
                return playerList;
            }
        }
        #endregion Properties
        #endregion Members
        #region Main Run Thread
        private static void run()
        {
            try
            {
                Statics.FuncPtrs.SetStatusBoxPtr("Starting the assistant...", Statics.Fields.Green);
                while (checkState())
                {
                    //bool spentTime = false;
                    //if ((nonBlockingTriggerQueue.Count > 0) || (blockingTriggerQueue.Count > 0))
                    //{
                    //    timestamp("Q count is non-zero, parsing triggers.");
                    //    spentTime = triggerParser(true);
                    //}
                    if (playerPtr == 0)
                    {
                        Statics.FuncPtrs.SetStatusBoxPtr("Can't find player '" + playerName + "'.", Statics.Fields.Yellow);
                        IocaineFunctions.delay(500);
                        playerPtr = MemReads.PCs.get_pointer(playerName);
                        playerActive = (playerPtr == 0) ? false : MemReads.PCs.get_pointer_valid(playerPtr);
                        if (playerActive)
                        {
                            Statics.FuncPtrs.SetStatusBoxPtr("Found player '" + playerName + "', resuming...", Statics.Fields.Green);
                        }
                        continue;
                    }
                    else
                    {
                        playerActive = MemReads.PCs.get_pointer_valid(playerPtr);
                        if (!playerActive)
                        {
                            //They could have D/C'd or warped, but in case they zoned, we'll move for 3 seconds to try to go with them.
                            //Navigation.moveForward(3000);
                            playerPtr = 0;
                            continue;
                        }
                    }
                    float distanceToPlayer = MemReads.PCs.get_distance(playerPtr);
                    if (follow && (distanceToPlayer > followDistance))
                    {
                        Statics.FuncPtrs.SetStatusBoxPtr("Following " + playerName + ", distance = " + String.Format("{0:0.##}", distanceToPlayer), Statics.Fields.Green);
                        Navigation.FollowPlayer(playerName, playerPtr, followDistance);
                        distanceToPlayer = MemReads.PCs.get_distance(playerPtr);
                    }
                    Byte plyrStatus = MemReads.PCs.get_status(playerPtr);
                    if ((distanceToPlayer < 20) && (plyrStatus == (Byte)FFXIEnums.STATUS.ATTACKING))
                    {
                        Statics.FuncPtrs.SetStatusBoxPtr("Assisting " + playerName + "...", Statics.Fields.Green);
                        fight(playerName, playerPtr);
                        consumeItem();
                    }
                    IocaineFunctions.delay(500);
                    //if (!spentTime)
                    //{
                    //    IocaineFunctions.delay(500);
                    //}
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::run: " + e.ToString());
            }
        }
        #endregion Main Run Thread
        //#region FFXI Event Handlers
        //public static void TAPlayerStatusChangedHandler(FFXIEventArgs e)
        //{
        //    PlayerStatusChangedEvArgs localArgs = (PlayerStatusChangedEvArgs)e;
        //    timestamp("player status change event handler, checking each trigger in the list.");
        //    foreach (TriggerPlayerStatusChange trig in triggerPlayerStatusChangeList)
        //    {
        //        timestamp("Checking: " + trig.ToString());
        //        if (checkTriggerValid(trig, localArgs))
        //        {
        //            timestamp("Trigger valid was true.");
        //            if (enqueueCheck(trig))
        //            {
        //                timestamp("Enqueue check was true.");
        //                Queue<Trigger> localQ = null;
        //                if (trig.Sequence.IsBlocking)
        //                {
        //                    localQ = blockingTriggerQueue;
        //                }
        //                else
        //                {
        //                    localQ = nonBlockingTriggerQueue;
        //                }
        //                lock (localQ)
        //                {
        //                    //Go thru the Q to make sure the same thing isn't already Q'd.
        //                    foreach (Trigger queuedTrig in localQ)
        //                    {
        //                        if (trig.Compare(queuedTrig))
        //                        {
        //                            //If it's already Q'd, just exit.
        //                            return;
        //                        }
        //                    }
        //                    //We went thru the whole list and didn't find anything, so go ahead and EnQ.
        //                    localQ.Enqueue(trig);
        //                    timestamp("Enq'ing the trigger.");
        //                }
        //            }
        //        }
        //    }
        //}
        //private static bool checkTriggerValid(Trigger trig, FFXIEventArgs args)
        //{
        //    switch (trig.Type)
        //    {
        //        case TriggerType.Player_Status_Change:
        //            TriggerPlayerStatusChange localTrig = (TriggerPlayerStatusChange)trig;
        //            PlayerStatusChangedEvArgs localArgs = (PlayerStatusChangedEvArgs)args;
        //            if (localTrig.PlayerName == localArgs.Name)
        //            {
        //                if (compareStatus(localTrig.OldStatus, localArgs.OldStatus) && (compareStatus(localTrig.NewStatus, localArgs.NewStatus)))
        //                {
        //                    return true;
        //                }
        //            }
        //            return false;

        //        default:
        //            return false;
        //    }
        //}
        //private static bool checkTriggerValidFinal(Trigger trig)
        //{
        //    //We only want to check a few things at this point.
        //    //This check is just to avoid doing unneccessary casting, abilities, or trying to fight
        //    //when the assist player is no longer fighting. This usually happens after a pause.
        //    switch (trig.Type)
        //    {
        //        case TriggerType.Player_Status_Change:
        //            //Check that the player's status is still what we triggered on.
        //            TriggerPlayerStatusChange localTrig = (TriggerPlayerStatusChange)trig;
        //            if (getCharacter(localTrig.PlayerName).Status == localTrig.NewStatus)
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                timestamp("TA: Throwing out trigger: " + localTrig.ToString() + " because " + localTrig.PlayerName + "'s status is no longer " + ((FFXIEnums.STATUS)localTrig.NewStatus).ToString());
        //                return false;
        //            }
        //        default:
        //            return true;
        //    }
        //}
        //private static bool enqueueCheck(Trigger trig)
        //{
        //    if (taState == 1)
        //    {
        //        return true;
        //    }
        //    else if (taState == 0)
        //    {
        //        //If we're user paused, we only want to enqueue certain triggers
        //        //and certain sequences.
        //        //We want to enqueue 1) casts, 2) JA's, 3) generic commands (not sure what's best in this case)
        //        //We don't want to Q 1) follow, 2) fighting
        //        if (trig.Sequence.Contains(ActionType.Follow) || trig.Sequence.Contains(ActionType.Assist))
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //#endregion FFXI Event Handlers
        //#region Command Parsing
        //private static bool triggerParser(bool iDoNonBlocking)
        //{
        //    Trigger firstAvailableTrigger = null;
        //    ActionSequence seq = null;
        //    if (iDoNonBlocking)
        //    {
        //        timestamp("Checking the non-blocking Q for a trigger");
        //        firstAvailableTrigger = getFirstAvailableTrigger(nonBlockingTriggerQueue);
        //    }
        //    if (firstAvailableTrigger == null)
        //    {
        //        timestamp("Checking the blocking Q for a trigger");
        //        firstAvailableTrigger = getFirstAvailableTrigger(blockingTriggerQueue);
        //    }
        //    if (firstAvailableTrigger == null)
        //    {
        //        timestamp("Could not find an available trigger in either Q.");
        //        return false;
        //    }
        //    else
        //    {
        //        timestamp("Found a trigger to perform.");
        //        timestamp("Found a valid trigger to perform: " + firstAvailableTrigger.ToString());
        //        //We have a valid trigger to perform.
        //        //Go thru each action in the sequence and do it.
        //        seq = firstAvailableTrigger.Sequence;
        //        int nbActions = seq.ActionCount;
        //        for (int ii = 0; ii < nbActions; ii++)
        //        {
        //            Data.Structures.Action action = seq.GetAction(ii);
        //            timestamp("Action " + ii + " to perform is type: " + action.Type.ToString());
        //            switch (action.Type)
        //            {
        //                case ActionType.Cast:
        //                    cast((ActionCast)action);
        //                    break;
        //                case ActionType.Ability:
        //                    jobAbility((ActionAbility)action);
        //                    break;
        //                case ActionType.Assist:
        //                    //fight((
        //                    break;
        //                case ActionType.Follow:
        //                    break;
        //                case ActionType.Wait:
        //                    //IocaineFunctions.delay(
        //                    break;
        //                case ActionType.Generic_Command:
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        return true;
        //    }
        //}
        //private static Trigger getFirstAvailableTrigger(Queue<Trigger> iQueue)
        //{
        //    Queue<Trigger> bufferQ = new Queue<Trigger>();
        //    Trigger localTrig = null;
        //    Trigger triggerToReturn = null;
        //    lock (iQueue)
        //    {
        //        int nbTriggersInMainQ = iQueue.Count;
        //        for (int ii = 0; ii < nbTriggersInMainQ; ii++)
        //        {
        //            localTrig = iQueue.Dequeue();
        //            if (checkTriggerValidFinal(localTrig))
        //            {
        //                //Trigger is still valid, check if we can perform the sequence at this time.
        //                if (checkSequenceAvailable(localTrig.Sequence))
        //                {
        //                    //We can perform this, so reshuffle the Q's if needed and return.
        //                    triggerToReturn = localTrig;
        //                    break;
        //                }
        //                else
        //                {
        //                    //We can't perform, so push it into the buffer and continue looking.
        //                    bufferQ.Enqueue(localTrig);
        //                }
        //            }
        //        }
        //        //Reshuffle Q's if needed and return
        //        if (bufferQ.Count > 0)
        //        {
        //            int iQCnt = iQueue.Count;
        //            for (int ii = 0; ii < iQCnt; ii++)
        //            {
        //                bufferQ.Enqueue(iQueue.Dequeue());
        //            }
        //            int bQCnt = bufferQ.Count;
        //            for (int ii = 0; ii < bQCnt; ii++)
        //            {
        //                iQueue.Enqueue(bufferQ.Dequeue());
        //            }
        //        }
        //    }
        //    return triggerToReturn;
        //}
        //private static bool checkSequenceAvailable(ActionSequence iSeq)
        //{
        //    int nbActions = iSeq.ActionCount;
        //    for (int ii = 0; ii < nbActions; ii++)
        //    {
        //        Data.Structures.Action localAction = iSeq.GetAction(ii);
        //        switch (localAction.Type)
        //        {
        //            case ActionType.Cast:
        //                if (!checkCastAvailable((ActionCast)localAction))
        //                {
        //                    return false;
        //                }
        //                break;
        //            case ActionType.Ability:
        //                if (!checkAbilityAvailable((ActionAbility)localAction))
        //                {
        //                    return false;
        //                }
        //                break;
        //            case ActionType.Assist:
        //                //Check to see if there is anything preventing us from fighting.

        //                break;
        //            case ActionType.Follow:
        //                //Check to see if there is anything preventing us from following.

        //                break;
        //            case ActionType.Generic_Command:
        //                //No way to tell if this is ok, so just assume we can do it now.
        //                break;
        //            case ActionType.Wait:
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    return true;
        //}
        //private static bool checkCastAvailable(ActionCast iAction)
        //{
        //    if (mainProc != null)
        //    {
        //        //Check that recast is up.
        //        if (MemReads.Self.Recast.Magic.get_time_remaining((short)iAction.SpellIndex) == 0)
        //        {
        //            //Check that we have enough MP.
        //            Spells.SPELL_INFO info = Spells.GetSpellInfo(iAction.SpellIndex);
        //            if (MemReads.Self.Vitals.get_mp_current() >= info.MP)
        //            {
        //                //Check that the target is in range.
        //                if (checkTargetInRange(iAction.Target))
        //                {
        //                    //Make sure we're not silenced or asleep.
        //                    if (!checkStatusActive("silence") && !checkStatusActive("sleep"))
        //                    {
        //                        return true;
        //                    }
        //                    else
        //                    {
        //                        return false;
        //                    }
        //                }
        //                else
        //                {
        //                    return false;
        //                }
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //private static bool checkAbilityAvailable(ActionAbility iAction)
        //{
        //    if (mainProc != null)
        //    {
        //        if (!checkStatusActive("amnesia"))
        //        {
        //            if (CommandManager.JAManager.GetRecastTime(iAction.AbilityName) == 0)
        //            {
        //                //TBD - real parsing here.
        //                //We have to check for target in range.
        //                //We have to check if this is a DNC ability that we have TP.
        //                //We have to check if this is a SMN ability that we have MP.
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //private static bool checkTargetInRange(String iTarget)
        //{
        //    if(mainProc == null)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        //If the target is my name or <me>, we're ok.
        //        if ((iTarget == "<me>") || (iTarget == MemReads.Self.get_name(true)))
        //        {
        //            return true;
        //        }
        //        //If the target is <bt>, just go ahead and cast.
        //        else if (iTarget == "<bt>")
        //        {
        //            return true;
        //        }
        //        //If the target is <t>, check your target's distance.
        //        else if (iTarget == "<t>")
        //        {
        //            if (MemReads.Target.get_distance() <= maxCastDistance)
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        //If the target is a player's name, get the Character and check distance.
        //        else
        //        {
        //            Character chr = getCharacter(iTarget);
        //            if (chr == null)
        //            {
        //                return false;
        //            }
        //            else
        //            {
        //                if (chr.Check_Active() && (chr.Distance <= maxCastDistance))
        //                {
        //                    return true;
        //                }
        //                else
        //                {
        //                    return false;
        //                }
        //            }
        //        }
        //    }
        //}
        //private static bool checkStatusActive(String iStatusName)
        //{
        //    if (mainProc != null)
        //    {
        //        ushort[] myStatusArray = null;
        //        MemReads.Self.StatusEffects.get_effects(ref myStatusArray);
        //        ushort[] inputStatusArray = StatusEffects.GetStatusEffectID("iStatusName");
        //        foreach (ushort inputID in inputStatusArray)
        //        {
        //            if (myStatusArray.Contains(inputID))
        //            {
        //                //Status is active, return true.
        //                return true;
        //            }
        //        }
        //        return false;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //#endregion Command Parsing
        #region Action Functions
        private static void fight(String iPlayerName, UInt32 iPlayerPtr)
        {
            //1. assist the player
            //2. wait
            //3. attack on
            //4. check target lock, set if it's not
            //5. line up with target
            //5. check for tp
            //7. check for end of fight
            try
            {
                while (checkState() && (MemReads.Self.get_status() != (Byte)FFXIEnums.STATUS.ATTACKING))
                {
                    Navigation.Stand();
                    LoggingFunctions.Timestamp("Trying to assist, status is: " + MemReads.Self.get_status());
                    LoggingFunctions.Timestamp("Sending command " + "/assist " + iPlayerName);
                    IocaineFunctions.keys("/assist " + iPlayerName);
                    IocaineFunctions.delay(1500);
                    if (MemReads.Target.get_name() == "")
                    {
                        LoggingFunctions.Timestamp("Target name is still empty");
                        return;
                    }
                    LoggingFunctions.Timestamp("Target name is '" + MemReads.Target.get_name() + "'");
                    LoggingFunctions.Timestamp("Sending command: " + "/attack on");
                    IocaineFunctions.keys("/attack on");
                    IocaineFunctions.delay(2000);
                    if (!MemReads.Target.get_locked())
                    {
                        LoggingFunctions.Timestamp("Target isn't locked, trying to set lock");
                        IocaineFunctions.keyDown(Keys.Multiply, 300);
                        if (!MemReads.Target.get_locked())
                        {
                            return;
                        }
                    }
                }
                //Should now be fighting
                byte mobHpp = MemReads.Target.get_hp_perc();
                while (checkState() && (mobHpp > 0))
                {
                    Navigation.MoveToTarget(Statics.Settings.TA.FightingDistance);
                    Navigation.FaceTarget();
                    LoggingFunctions.Timestamp("mob hpp is " + mobHpp);
                    if ((mobHpp > wshpp) && (MemReads.Self.Vitals.get_tp_current() >= 1000))
                    {
                        if ((wsCommand != "") && (wsCommand != "Weapon Skill Name"))
                        {
                            IocaineFunctions.keys("/ws \"" + wsCommand + "\" <t>");
                        }
                    }
                    IocaineFunctions.delay(1000);
                    if (MemReads.Target.get_name() == "")
                    {
                        break;
                    }
                    mobHpp = MemReads.Target.get_hp_perc();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::fight: " + e.ToString());
            }
        }
        //private static void cast(ActionCast iAction)
        //{
        //    //Navigation.stand();
        //    MessageBox.Show("Casting " + iAction.SpellName + " on " + iAction.Target);
        //}
        //private static void jobAbility(ActionAbility iAction)
        //{

        //}
        private static void consumeItem()
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
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::consumeItem: " + e.ToString());
            }
        }
        #endregion Action Functions
        #region State Changes and Checking
        private static bool checkState()
        {
            try
            {
                LoggingFunctions.Debug("Entering TA checkstate", LoggingFunctions.DBG_SCOPE.TA);
                switch (taState)
                {
                    case 0:
                        Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot.", Statics.Fields.Blue);
                        return false;
                    case 1:
                        return true;
                    default:
                        LoggingFunctions.Debug("Returning false from TA checkstate", LoggingFunctions.DBG_SCOPE.TA);
                        return false;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::checkState: " + e.ToString());
                return false;
            }
        }
        public static void Start()
        {
            taState = 1;
            run();
        }
        public static void Stop()
        {
            taState = 0;
        }
        #endregion Start and Stop
        #region Update Functions
        #region Outgoing
        public static Character GetPlayer(int iIndex)
        {
            try
            {
                if ((iIndex < playerStateList.Count) && (iIndex >= 0))
                {
                    return playerList[iIndex];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::GetPlayer: " + e.ToString());
                return null;
            }
        }
        public static Character GetPlayer(String iPlayerName)
        {
            return getCharacter(iPlayerName);
        }
        public static PlayerState GetPlayerState(int iIndex)
        {
            try
            {
                if ((iIndex < playerStateList.Count) && (iIndex >= 0))
                {
                    return playerStateList[iIndex];
                }
                else
                {
                    return PlayerState.Inactive;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::GetPlayerState(int): " + e.ToString());
                return PlayerState.Inactive;
            }
        }
        public static PlayerState GetPlayerState(String iPlayerName)
        {
            try
            {
                int idx = playerList.IndexOf(getCharacter(iPlayerName));
                if (idx >= 0)
                {
                    return GetPlayerState(idx);
                }
                else
                {
                    return PlayerState.Inactive;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::GetPlayerState(String): " + e.ToString());
                return PlayerState.Inactive;
            }
        }
        #endregion Outgoing
        #region Incoming
        public static void UpdatePlayerName(String iPlayerName)
        {
            try
            {
                playerName = iPlayerName;
                playerPtr = MemReads.PCs.get_pointer(iPlayerName);
                playerActive = (playerPtr == 0) ? false : MemReads.PCs.get_pointer_valid(playerPtr);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::UpdatePlayerName: " + e.ToString());
            }
        }
        public static void UpdateFollow(bool iFollow)
        {
            follow = iFollow;
        }
        public static void UpdateFollowDistance(int iFollowDistance)
        {
            followDistance = iFollowDistance;
        }
        public static void UpdateWSHPP(int iWSHPP)
        {
            wshpp = iWSHPP;
        }
        public static void UpdateWSCommand(String iWSCommand)
        {
            wsCommand = iWSCommand;
        }
        public static void AddTrigger(Trigger iTrigger)
        {
            //    iTrigger.Show();
            //    switch (iTrigger.Type)
            //    {
            //        case TriggerType.Player_Status_Change:
            //            triggerPlayerStatusChangeList.Add((TriggerPlayerStatusChange)iTrigger);
            //            if (ChangeMonitor._playerStatusChangedCount == 0)
            //            {
            //                timestamp("Adding playerstatuschanged event subscription.");
            //                ChangeMonitor._playerStatusChangedCount++;
            //                ChangeMonitor._playerStatusChanged += new ChangeMonitor.CM_Delegate_FFXIEvent(TAPlayerStatusChangedHandler);
            //            }
            //            break;
            //        default:
            //            break;
            //    }
        }
        public static void AddPlayer(String iPlayerName)
        {
            try
            {
                Character chr = new Character(iPlayerName);
                playerList.Add(chr);
                playerStateList.Add(PlayerState.OutOfRange);
                updatePlayerState(iPlayerName);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::AddPlayer: " + e.ToString());
            }
        }
        public static void RemovePlayer(String iPlayerName)
        {
            try
            {
                int playerIndex = playerList.IndexOf(getCharacter(iPlayerName));
                if (playerIndex >= 0)
                {
                    playerList.RemoveAt(playerIndex);
                    playerStateList.RemoveAt(playerIndex);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::RemovePlayer: " + e.ToString());
            }
        }
        #endregion Incoming
        #endregion Update Functions
        #region Utility Functions
        private static void updatePlayerState(String iPlayerName)
        {
            try
            {
                Character localChr = getCharacter(iPlayerName);
                if (localChr != null)
                {
                    int idx = playerList.IndexOf(localChr);
                    if (localChr.Active)
                    {
                        playerStateList[idx] = PlayerState.Active;
                    }
                    else
                    {
                        playerStateList[idx] = PlayerState.Inactive;
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::updatePlayerState: " + e.ToString());
            }
        }
        private static void updateAllPlayerStates()
        {
            for (int ii = 0; ii < playerList.Count; ii++)
            {
                updatePlayerState(playerList[ii].Name);
            }
        }
        private static Character getCharacter(String iPlayerName)
        {
            Character localChr = null;
            try
            {
                foreach (Character chr in playerList)
                {
                    if (chr.Name == iPlayerName)
                    {
                        localChr = chr;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::getCharacter: " + e.ToString());
            }
            return localChr;
        }
        private static String getPlayerListString()
        {
            String playerString = "";
            try
            {
                foreach (Character chr in playerList)
                {
                    playerString += chr.Name + "\n";
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::getPlayerListString: " + e.ToString());
            }
            return playerString;
        }
        private static bool compareStatus(Byte status1, Byte status2)
        {
            try
            {
                if (status1 == status2)
                {
                    return true;
                }
                else if ((status1 == 255) || (status2 == 255))
                {
                    //255 = ANY, so they match
                    return true;
                }
                else if (((status1 == (Byte)FFXIEnums.STATUS.KO1) && (status2 == (Byte)FFXIEnums.STATUS.KO2))
                     || ((status1 == (Byte)FFXIEnums.STATUS.KO2) && (status2 == (Byte)FFXIEnums.STATUS.KO1)))
                {
                    //Both KO's mean KO
                    return true;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::compareStatus: " + e.ToString());
            }
            return false;
        }
        #endregion Utility Functions
        #region Threads Section
        public static void StartStatusChecking()
        {
            try
            {
                if (playerStatusCheckThread == null)
                {
                    playerStatusCheckThread = new Thread(checkPlayerActivityThread);
                    playerStatusCheckThread.Name = "playerStatusCheckThread";
                    playerStatusCheckThread.IsBackground = true;
                    playerStatusCheckThread.Start();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::StartStatusChecking: " + e.ToString());
            }
        }
        private static void checkPlayerActivityThread()
        {
            try
            {
                while (true)
                {
                    IocaineFunctions.delay(playerActivePollPeriod);
                    int nbPlayers = playerList.Count;
                    if (nbPlayers == 0)
                    {
                        continue;
                    }
                    else
                    {
                        try
                        {
                            for (int ii = 0; ii < nbPlayers; ii++)
                            {
                                Character chr = playerList[ii];
                                bool chrActive = chr.Check_Active();
                                if (chrActive)
                                {
                                    if (playerStateList[ii] != PlayerState.Active)
                                    {
                                        playerStateList[ii] = PlayerState.Active;
                                        redrawPlayerLBCallBack(ii);
                                    }
                                }
                                else
                                {
                                    if (!chr.Check_Pointer_Valid())
                                    {
                                        if (playerStateList[ii] != PlayerState.Inactive)
                                        {
                                            playerStateList[ii] = PlayerState.Inactive;
                                            redrawPlayerLBCallBack(ii);
                                        }
                                        chr.Update_Pointer();
                                    }
                                    else if (chr.Distance >= 46)
                                    {
                                        if (playerStateList[ii] != PlayerState.OutOfRange)
                                        {
                                            playerStateList[ii] = PlayerState.OutOfRange;
                                            redrawPlayerLBCallBack(ii);
                                        }
                                    }
                                    else
                                    {
                                        if (playerStateList[ii] != PlayerState.Inactive)
                                        {
                                            playerStateList[ii] = PlayerState.Inactive;
                                            redrawPlayerLBCallBack(ii);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggingFunctions.Error("In the TA player status update thread: " + ex.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TeachersAssistant::checkPlayerActivity: " + e.ToString());
            }
        }
        #endregion
    }
}
