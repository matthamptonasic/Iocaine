using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Data.Structures;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Properties;

namespace Iocaine2.Bots
{
    public class SkillUp : Bot
    {
        #region Enums
        private enum STOP_REASON
        {
            SKILL_CAPPED = 0,
            SKILL_REACHED = 1,
            COUNT_COMPLETE = 2,
            USER = 3
        }
        #endregion Enums

        #region Template Members
        private static SkillUp instance = null;
        private static readonly Object padlock = new Object();
        #endregion Template Members

        #region Bot Specific Members
        private TextBox botStatusBox;
        private Button startButton;
        private ListBox commandListBox;
        private byte currentSkill;
        private bool goToCap;
        private int skillLevel;
        private bool stopAtGivenSkill;
        private bool doOnly;
        private int doOnlyCount;
        private bool doneStop;
        private bool doneLogout;
        private bool doneShutdown;
        private bool giveLogoutCommand;
        private String logoutCommand;
        private bool giveRestCommand;
        private String restCommand;
        private JobAbilCommand smnReleaseCommand;
        private int skillLevelNow;
        private bool delayBetweenCasts;
        private uint delayValueBetweenCasts;
        private Iocaine_2_Form.SU_SetSUStartButtonDelegate setStartButtonCallBack;
        #region Reives
        private static List<String> targetList = new List<string>() {"Arboreal Bastion",
                                                                     "Avian Roost",
                                                                     "Banespore",
                                                                     "Dimensional Tether",
                                                                     "Grimy Boulders",
                                                                     "Sere Stump",
                                                                     "Wasp Nest",
                                                                     "Wintry Cave"
                                                                    };
        private static String targetName = "";
        private static List<Command> cmdList = Data.Structures.CommandManager.CurrentCommands;
        private static SpellCommand elementalCmd = null;
        private static SpellCommand refreshCmd = null;
        private static SpellCommand enhancingCmd = null;
        private static SpellCommand enfeeblingCmd = null;
        private static SpellCommand darkCmd = null;
        private static SpellCommand divineCmd = null;
        #endregion Reives
        #region Controls
        private static String elementalCastName = "Stone";
        private static String refreshCastName = "Refresh";
        private static String refreshStatusName = "Refresh";
        private static String enhancingCastName = "Gain-INT";
        private static String enhancingStatusName = "INT Boost";
        private static String enfeeblingCastName = "Frazzle";
        private static String darkCastName = "Bio";
        private static String divineCastName = "Banish";
        private static bool doLairReives = false;
        private static bool doRefresh = false;
        private static bool doEnhancing = false;
        private static bool doEnfeebling = false;
        private static bool doDark = false;
        private static bool doDivine = false;
        private static UInt32 castCountLimit = 20;
        private static UInt32 castCount = castCountLimit;
        #endregion Controls
        #region Properties
        #region Control Parallels
        #region Enables
        public static bool SU_DoLairReives
        {
            get
            {
                return doLairReives;
            }
            set
            {
                doLairReives = value;
            }
        }
        public static bool SU_DoRefresh
        {
            get
            {
                return doRefresh;
            }
            set
            {
                doRefresh = value;
            }
        }
        public static bool SU_DoEnhancing
        {
            get
            {
                return doEnhancing;
            }
            set
            {
                doEnhancing = value;
            }
        }
        public static bool SU_DoEnfeebling
        {
            get
            {
                return doEnfeebling;
            }
            set
            {
                doEnfeebling = value;
            }
        }
        public static bool SU_DoDark
        {
            get
            {
                return doDark;
            }
            set
            {
                doDark = value;
            }
        }
        public static bool SU_DoDivine
        {
            get
            {
                return doDivine;
            }
            set
            {
                doDivine = value;
            }
        }
        #endregion Enables
        #region Commands
        public static String SU_ElementalCastName
        {
            get
            {
                return elementalCastName;
            }
            set
            {
                elementalCastName = value;
            }
        }
        public static String SU_RefreshCastName
        {
            get
            {
                return refreshCastName;
            }
            set
            {
                refreshCastName = value;
            }
        }
        public static String SU_EnhancingCastName
        {
            get
            {
                return enhancingCastName;
            }
            set
            {
                enhancingCastName = value;
            }
        }
        public static String SU_EnhancingStatusName
        {
            get
            {
                return enhancingStatusName;
            }
            set
            {
                enhancingStatusName = value;
            }
        }
        public static String SU_EnfeeblingCastName
        {
            get
            {
                return enfeeblingCastName;
            }
            set
            {
                enfeeblingCastName = value;
            }
        }
        public static String SU_DarkCastName
        {
            get
            {
                return darkCastName;
            }
            set
            {
                darkCastName = value;
            }
        }
        public static String SU_DivineCastName
        {
            get
            {
                return divineCastName;
            }
            set
            {
                divineCastName = value;
            }
        }
        #endregion Commands
        public static UInt32 SU_CastCountLimit
        {
            get
            {
                return castCountLimit;
            }
            set
            {
                castCountLimit = value;
            }
        }
        #endregion Control Parallels
        #endregion Properties
        #endregion //Members Section

        #region Constructor
        private SkillUp()
        {
            base.name = "SkillUp";
        }
        #endregion Constructor

        #region Template Functions
        public static SkillUp Access
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SkillUp();
                    }
                    return instance;
                }
            }
        }
        public override void SetParams(Bots.BotParam iParam)
        {
            if (iParam == null)
            {
                return;
            }
            Bots.SUParam prm = (Bots.SUParam)iParam;
            c_startButton = prm.StartButton;
            botStatusBox = prm.StatusBox;
            startButton = prm.StartButton;
            commandListBox = prm.CommandListBox;
            currentSkill = prm.CurrentSkil;
            goToCap = prm.GoToCap;
            skillLevel = prm.SkillLevel;
            stopAtGivenSkill = prm.StopAtGivenSkill;
            doOnly = prm.DoOnly;
            doOnlyCount = prm.DoOnlyCount;
            doneStop = prm.DoneStop;
            doneLogout = prm.DoneLogout;
            doneShutdown = prm.DoneShutdown;
            giveLogoutCommand = prm.GiveLogoutCommand;
            logoutCommand = prm.LogoutCommand;
            giveRestCommand = prm.GiveRestCommand;
            restCommand = prm.RestCommand;
            delayBetweenCasts = prm.DelayBetweenCasts;
            delayValueBetweenCasts = prm.DelayValueBetweenCasts;
            skillLevelNow = currentSkillLevel();
            setStartButtonCallBack = prm.SetStartButtonCallBack;
            
            initParamsSet = true;
        }
        #endregion Template Functions
        
        #region Status Related
        public override void Pause(Boolean iNow = false)
        {
            Statics.FuncPtrs.SetStatusBoxPtr("Pausing bot", Statics.Fields.Yellow);
            base.Pause(iNow);
        }
        public override void Resume()
        {
            Statics.FuncPtrs.SetStatusBoxPtr("Resuming...", Statics.Fields.Green);
            base.Resume();
        }
        public override void Stop()
        {
            StopSU((int)STOP_REASON.USER);
        }
        private void StopSU(int stopReason)
        {
            try
            {
                base.Stop();
                switch (stopReason)
                {
                    case (int)STOP_REASON.USER:
                        Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot", Statics.Fields.Blue);
                        break;
                    case (int)STOP_REASON.COUNT_COMPLETE:
                        Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot due to reaching given count", Statics.Fields.Blue);
                        break;
                    case (int)STOP_REASON.SKILL_CAPPED:
                        Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot due to skill cap being reached", Statics.Fields.Blue);
                        break;
                    case (int)STOP_REASON.SKILL_REACHED:
                        Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot due to given skill level being reached", Statics.Fields.Blue);
                        break;
                    default:
                        Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot", Statics.Fields.Blue);
                        break;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("SkillUp::StopSU: " + e.ToString());
            }
        }
        protected override void runThreadFunction()
        {
            publicRunThreadFunction();
        }
        //private override Boolean checkState()
        //{
        //    LoggingFunctions.Debug("SU::checkState: Entering SU checkstate.", LoggingFunctions.DBG_SCOPE.SU);
        //    switch (suState)
        //    {
        //        case 0: // stopped
        //            return false;
        //        case 1: // running
        //            return true;
        //        case 2: // user paused
        //            while (suState != 1)
        //            {
        //                if (suState == 0)
        //                {
        //                    LoggingFunctions.Debug("SU::checkState: Returning false from SU checkstate.", LoggingFunctions.DBG_SCOPE.SU);
        //                    return false;
        //                }
        //                IocaineFunctions.delay(1000);
        //                if (!ChangeMonitor.LoggedIn)
        //                {
        //                    suState = 0;
        //                }
        //            }
        //            LoggingFunctions.Debug("SU::checkState: Returning true from SU checkstate.", LoggingFunctions.DBG_SCOPE.SU);
        //            return true;
        //        default: // program paused
        //            LoggingFunctions.Debug("SU::checkState: Returning false from SU checkstate.", LoggingFunctions.DBG_SCOPE.SU);
        //            return false;
        //    }
        //}
        protected override bool checkStatus()
        {
            return true;
        }
        #endregion Status Related

        #region Main Thread
        public void publicRunThreadFunction()
        {
            state = STATE.RUNNING;
            int currentCmdIdx = 0;
            int currentCount = 0;
            try
            {
                int tempSkillLevel = currentSkillLevel();

                #region Reives
                if (doLairReives)
                {
                    elementalCmd = null;
                    foreach (Command localCmd in cmdList)
                    {
                        if (localCmd.Name == elementalCastName)
                        {
                            elementalCmd = (SpellCommand)localCmd;
                            break;
                        }
                    }
                    refreshCmd = null;
                    if (doRefresh)
                    {
                        foreach (Command localCmd in cmdList)
                        {
                            if (localCmd.Name == refreshCastName)
                            {
                                refreshCmd = (SpellCommand)localCmd;
                                break;
                            }
                        }
                    }
                    enfeeblingCmd = null;
                    if (doEnfeebling)
                    {
                        foreach (Command localCmd in cmdList)
                        {
                            if (localCmd.Name == enfeeblingCastName)
                            {
                                enfeeblingCmd = (SpellCommand)localCmd;
                                break;
                            }
                        }
                    }
                    darkCmd = null;
                    if (doDark)
                    {
                        foreach (Command localCmd in cmdList)
                        {
                            if (localCmd.Name == darkCastName)
                            {
                                darkCmd = (SpellCommand)localCmd;
                                break;
                            }
                        }
                    }
                    divineCmd = null;
                    if (doDivine)
                    {
                        foreach (Command localCmd in cmdList)
                        {
                            if (localCmd.Name == divineCastName)
                            {
                                divineCmd = (SpellCommand)localCmd;
                                break;
                            }
                        }
                    }
                    enhancingCmd = null;
                    if (doEnhancing)
                    {
                        foreach (Command localCmd in cmdList)
                        {
                            if (localCmd.Name == enhancingCastName)
                            {
                                enhancingCmd = (SpellCommand)localCmd;
                                break;
                            }
                        }
                    }
                    if (elementalCmd == null)
                    {
                        MessageBox.Show("Could not find elemental spell to cast.");
                        return;
                    }
                }
                #endregion Reives
                #region Summoning
                smnReleaseCommand = null;
                foreach (Command localCmd in cmdList)
                {
                    if (localCmd.Name == "Release")
                    {
                        smnReleaseCommand = (JobAbilCommand)localCmd;
                        break;
                    }
                }
                #endregion Summoning

                while (checkState())
                {
                    #region Reives
                    if (doLairReives)
                    {
                        List<MemReads.NPCs.NPCInfoStruct> npcList = MemReads.NPCs.get_NPCInfoStructList(true);
                        MemReads.NPCs.NPCInfoStruct mobInfo = new MemReads.NPCs.NPCInfoStruct();
                        foreach (MemReads.NPCs.NPCInfoStruct npc in npcList)
                        {
                            String npcName = MemReads.NPCs.getName(npc);
                            if (targetList.Contains(npcName))
                            {
                                mobInfo = npc;
                                break;
                            }
                        }
                        if (mobInfo.ID == 0)
                        {
                            MessageBox.Show("No Lair Reive found nearby.");
                            Stop();
                            break;
                        }
                        Int16 mobId = mobInfo.ID;
                        String mobName = MemReads.NPCs.getName(mobInfo);

                        castCount = castCountLimit;

                        while (mobInfo.Active != MemReads.NPCs.eActive.MobDrawn)
                        {
                            IocaineFunctions.delay(2000);
                            if (!checkState())
                            {
                                return;
                            }
                            MemReads.NPCs.get_NPCInfoStruct(ref mobInfo, mobId);
                            continue;
                        }
                        if (!Iocaine2.Tools.Interaction.TargetNPC(mobName, checkState))
                        {
                            IocaineFunctions.delay(2000);
                            continue;
                        }
                        targetName = MemReads.Target.get_name();
                        while (checkState() && (mobInfo.HPP > 0)) // && (MemReads.Target.get_hp_perc() > 0) && (MemReads.Target.get_name() != ""))
                        {
                            if (!checkTargetMob(mobName, mobInfo))
                            {
                                break;
                            }
                            if (!checkStatusActive(refreshStatusName) && (refreshCmd != null))
                            {
                                refreshCmd.Execute("<me>");
                            }
                            if (!checkStatusActive(enhancingStatusName) && (enhancingCmd != null))
                            {
                                enhancingCmd.Execute("<me>");
                            }
                            if (MemReads.Self.Vitals.get_mp_current() < elementalCmd.MP)
                            {
                                rest();
                                // check the state, because it might have been changed during resting.
                                if (!checkState())
                                {
                                    return;
                                }
                            }
                            while (!elementalCmd.Ready)
                            {
                                IocaineFunctions.delay(500);
                            }
                            if (!checkTargetMob(mobName, mobInfo))
                            {
                                break;
                            }
                            elementalCmd.Execute("<t>");
                            if (++castCount >= castCountLimit)
                            {
                                if (enfeeblingCmd != null)
                                {
                                    if (!checkTargetMob(mobName, mobInfo))
                                    {
                                        break;
                                    }
                                    enfeeblingCmd.Execute("<t>");
                                }
                                if (divineCmd != null)
                                {
                                    if (!checkTargetMob(mobName, mobInfo))
                                    {
                                        break;
                                    }
                                    divineCmd.Execute("<t>");
                                }
                                if (darkCmd != null)
                                {
                                    if (!checkTargetMob(mobName, mobInfo))
                                    {
                                        break;
                                    }
                                    darkCmd.Execute("<t>");
                                }
                                castCount = 0;
                            }
                        }
                        //for (int ii = 0; ii < 60 * (uint)respawnTime; ii++)
                        //{
                        //    IocaineFunctions.delay(1000);
                        //    if(!checkState())
                        //    {
                        //        return;
                        //    }
                        //}
                        continue;
                    }
                    #endregion Reives
                    #region Traditional SU
                    if (goToCap && currentSkillCapped())
                    {
                        LoggingFunctions.Debug("SU::runThreadFunction: quitting because we're capped.", LoggingFunctions.DBG_SCOPE.SU);
                        quit((int)STOP_REASON.SKILL_CAPPED);
                        break;
                    }
                    else if (stopAtGivenSkill && (currentSkillLevel() >= skillLevel))
                    {
                        LoggingFunctions.Debug("SU::runThreadFunction: quitting because we're at given skill level.", LoggingFunctions.DBG_SCOPE.SU);
                        quit((int)STOP_REASON.SKILL_REACHED);
                        break;
                    }
                    else if (doOnly && (currentCount >= doOnlyCount))
                    {
                        LoggingFunctions.Debug("SU::runThreadFunction: quitting because we've reached our count.", LoggingFunctions.DBG_SCOPE.SU);
                        quit((int)STOP_REASON.COUNT_COMPLETE);
                        break;
                    }
                    else
                    {
                        if (commandListBox.Items.Count == 0)
                        {
                            Statics.FuncPtrs.SetStatusBoxPtr("Waiting to add commands", Statics.Fields.Yellow);
                            while ((commandListBox.Items.Count == 0) && checkState())
                            {
                                IocaineFunctions.delay(1000);
                            }
                        }
                        if (currentCmdIdx >= commandListBox.Items.Count)
                        {
                            currentCmdIdx = 0;
                        }
                        SpellCommand localCmd = (SpellCommand)commandListBox.Items[currentCmdIdx];
                        if (MemReads.Self.Vitals.get_mp_current() < localCmd.MP)
                        {
                            rest();
                            // check the state, because it might have been changed during resting.
                            if (checkState() == false)
                            {
                                LoggingFunctions.Debug("SU::runThreadFunction: stopping because user stopped or paniced during resting.", LoggingFunctions.DBG_SCOPE.SU);
                                break;
                            }
                        }
                        if (!localCmd.Ready)
                        {
                            Statics.FuncPtrs.SetStatusBoxPtr("Waiting for recast timer of " + localCmd.Name + " (" + localCmd.TimeRemaining + ")", Statics.Fields.Green);
                            while (!localCmd.Ready && checkState())
                            {
                                IocaineFunctions.delay(500);
                            }
                        }
                        Statics.FuncPtrs.SetStatusBoxPtr("Attempting to cast " + localCmd.Name, Statics.Fields.Green);
                        stand();
                        if (localCmd.Execute("<me>"))
                        {
                            //IocaineFunctions.delay((uint)localCmd.CastTime);
                            //IocaineFunctions.delay((uint)1000);
                            currentCmdIdx++;
                            currentCount++;
                            UInt16 smnSkill = Data.Client.Skills.GetSkillId("Summoning Magic");
                            if (localCmd.Skill == smnSkill)
                            {
                                while (!smnReleaseCommand.Ready && checkState())
                                {
                                    IocaineFunctions.delay(500);
                                }
                                smnReleaseCommand.Execute("<me>");
                            }
                            if (delayBetweenCasts == true)
                            {
                                IocaineFunctions.delay(delayValueBetweenCasts);
                            }
                        }
                        tempSkillLevel = currentSkillLevel();
                        if (tempSkillLevel > skillLevelNow)
                        {
                            LoggingFunctions.Timestamp("Skill level change from " + skillLevelNow + " to " + tempSkillLevel);
                            skillLevelNow = tempSkillLevel;
                        }
                    }
                    #endregion Traditional SU
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("SkillUp::PublicRunThreadFunction: " + e.ToString());
            }
        }
        #endregion Main Thread
        #region Utility Procedures
        private void rest()
        {
            try
            {
                LoggingFunctions.Debug("SU::rest: Resting.", LoggingFunctions.DBG_SCOPE.SU);
                if (giveRestCommand)
                {
                    Statics.FuncPtrs.SetStatusBoxPtr(restCommand, Statics.Fields.Green);
                    IocaineFunctions.keys(restCommand);
                    IocaineFunctions.delay(8000);
                }
                Statics.FuncPtrs.SetStatusBoxPtr("Resting", Statics.Fields.Green);
                while ((MemReads.Self.get_status() != (int)FFXIEnums.STATUS.HEALING) && checkState())
                {
                    IocaineFunctions.keys("/heal on");
                    IocaineFunctions.delay(3000);
                }
                while ((MemReads.Self.Vitals.get_mp_percent() < 100) && checkState())
                {
                    if (MemReads.Self.get_status() != (int)FFXIEnums.STATUS.HEALING)
                    {
                        IocaineFunctions.keys("/heal on");
                        IocaineFunctions.delay(3000);
                    }
                    IocaineFunctions.delay(1000);
                }
                stand();
                IocaineFunctions.delay(500);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("SkillUp::rest: " + e.ToString());
            }
        }
        private void stand()
        {
            try
            {
                LoggingFunctions.Debug("SU::stand: Standing.", LoggingFunctions.DBG_SCOPE.SU);
                if (MemReads.Self.get_status() == (int)FFXIEnums.STATUS.HEALING)
                {
                    do
                    {
                        IocaineFunctions.keyDown(Keys.NumPad8, 500);
                    } while ((MemReads.Self.get_status() == (int)FFXIEnums.STATUS.HEALING) && checkState());
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("SkillUp::stand: " + e.ToString());
            }
        }
        private void quit(int stopReason)
        {
            try
            {
                LoggingFunctions.Debug("SU::quit: Quitting.", LoggingFunctions.DBG_SCOPE.SU);
                StopSU(stopReason);
                if (doneStop)
                {
                    return;
                }
                else if (doneLogout)
                {
                    IocaineFunctions.delay(5 * 1000);
                    if (giveLogoutCommand)
                    {
                        IocaineFunctions.delay(10 * 1000);
                        IocaineFunctions.keys(logoutCommand);
                        IocaineFunctions.delay(60 * 1000);
                    }
                    IocaineFunctions.keys("/logout");
                    return;
                }
                else if (doneShutdown)
                {
                    IocaineFunctions.delay(5 * 1000);
                    if (giveLogoutCommand)
                    {
                        IocaineFunctions.delay(10 * 1000);
                        IocaineFunctions.keys(logoutCommand);
                        IocaineFunctions.delay(60 * 1000);
                    }
                    IocaineFunctions.keys("/shutdown");
                    return;
                }
                else
                {
                    LoggingFunctions.Error("No \"Done\" method selected.");
                    return;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("SkillUp::quit: " + e.ToString());
            }
        }
        private bool currentSkillCapped()
        {
            try
            {
                LoggingFunctions.Debug("SU::currentSkillCapped: currentSkill is " + currentSkill + ".", LoggingFunctions.DBG_SCOPE.SU);
                switch (currentSkill)
                {
                    case (int)Iocaine_2_Form.SU_TYPE.BLUE:
                        return MemReads.Self.Skills.Magic.get_blue_capped();
                    case (int)Iocaine_2_Form.SU_TYPE.ENHANCING:
                        return MemReads.Self.Skills.Magic.get_enhancing_capped();
                    case (int)Iocaine_2_Form.SU_TYPE.HEALING:
                        return MemReads.Self.Skills.Magic.get_healing_capped();
                    case (int)Iocaine_2_Form.SU_TYPE.NINJUTSU:
                        return MemReads.Self.Skills.Magic.get_ninjutsu_capped();
                    case (int)Iocaine_2_Form.SU_TYPE.SINGING:
                        return MemReads.Self.Skills.Musical.get_singing_capped();
                    case (int)Iocaine_2_Form.SU_TYPE.STRING_INSTR:
                        return MemReads.Self.Skills.Musical.get_string_capped();
                    case (int)Iocaine_2_Form.SU_TYPE.SUMMONING:
                        return MemReads.Self.Skills.Magic.get_summoning_capped();
                    case (int)Iocaine_2_Form.SU_TYPE.WIND_INSTR:
                        return MemReads.Self.Skills.Musical.get_wind_capped();
                    default:
                        LoggingFunctions.Error("Unexpected skill selected (" + currentSkill + ")");
                        return false;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("SkillUp::currentSkillCapped: " + e.ToString());
                return false;
            }
        }
        private int currentSkillLevel()
        {
            try
            {
                LoggingFunctions.Debug("SU::currentSkillLevel: currentSkill is " + currentSkill + ".", LoggingFunctions.DBG_SCOPE.SU);
                switch (currentSkill)
                {
                    case (int)Iocaine_2_Form.SU_TYPE.BLUE:
                        return MemReads.Self.Skills.Magic.get_blue();
                    case (int)Iocaine_2_Form.SU_TYPE.ENHANCING:
                        return MemReads.Self.Skills.Magic.get_enhancing();
                    case (int)Iocaine_2_Form.SU_TYPE.HEALING:
                        return MemReads.Self.Skills.Magic.get_healing();
                    case (int)Iocaine_2_Form.SU_TYPE.NINJUTSU:
                        return MemReads.Self.Skills.Magic.get_ninjutsu();
                    case (int)Iocaine_2_Form.SU_TYPE.SINGING:
                        return MemReads.Self.Skills.Musical.get_singing();
                    case (int)Iocaine_2_Form.SU_TYPE.STRING_INSTR:
                        return MemReads.Self.Skills.Musical.get_string();
                    case (int)Iocaine_2_Form.SU_TYPE.SUMMONING:
                        return MemReads.Self.Skills.Magic.get_summoning();
                    case (int)Iocaine_2_Form.SU_TYPE.WIND_INSTR:
                        return MemReads.Self.Skills.Musical.get_wind();
                    default:
                        LoggingFunctions.Error("Unexpected skill selected (" + currentSkill + ")");
                        return 400;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("SkillUp::currentSkillLevel: " + e.ToString());
                return 400;
            }
        }
        private Boolean checkTargetMob(String iMobName, MemReads.NPCs.NPCInfoStruct iMobInfo)
        {
            try
            {
                #region Check Mob Active
                Int16 mobId = iMobInfo.ID;
                if (!MemReads.NPCs.get_NPCInfoStruct(ref iMobInfo, mobId))
                {
                    return false;
                }
                if ((iMobInfo.Active != MemReads.NPCs.eActive.MobDrawn) || (iMobInfo.HPP == 0))
                {
                    return false;
                }
                #endregion Check Mob Active
                #region Check Target
                String mobName = MemReads.NPCs.getName(iMobInfo);
                if (MemReads.Target.get_name() != mobName)
                {
                    if (!Iocaine2.Tools.Interaction.TargetNPC(mobName, checkState))
                    {
                        return false;
                    }
                }
                if (!MemReads.Target.get_locked())
                {
                    if (MemReads.Target.get_name() == mobName)
                    {
                        IocaineFunctions.keyDown(Keys.Multiply, 100);
                    }
                    else
                    {
                        //Should never get here.
                        return false;
                    }
                }
                return true;
                #endregion Check Target
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("SkillUp::checkTargetMob: " + e.ToString());
                return false;
            }
        }
        #endregion //Utility Procedures
        #region Update Procedures
        public bool updateCurrentSkill(byte newSkill)
        {
            try
            {
                currentSkill = newSkill;
                skillLevelNow = currentSkillLevel();
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("SkillUp::updateCurrentSkill: " + e.ToString());
                return false;
            }
        }
        public bool updateStopSettings(bool iGoToCap, bool iStopAtGivenSkill, int iSkillLevel, bool iDoOnly, int iDoOnlyCount)
        {
            try
            {
                goToCap = iGoToCap;
                stopAtGivenSkill = iStopAtGivenSkill;
                skillLevel = iSkillLevel;
                doOnly = iDoOnly;
                doOnlyCount = iDoOnlyCount;
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("SkillUp::updateStopSettings: " + e.ToString());
                return false;
            }
        }
        public bool updateWhenDoneSettings(bool iDoneStop, bool iDoneLogout, bool iDoneShutdown)
        {
            doneStop = iDoneStop;
            doneLogout = iDoneLogout;
            doneShutdown = iDoneShutdown;
            return true;
        }
        public bool updateGiveCommandBeforeResting(bool iGiveRestCommand)
        {
            giveRestCommand = iGiveRestCommand;
            return true;
        }
        public bool updateCommandBeforeResting(String iRestCommand)
        {
            restCommand = iRestCommand;
            return true;
        }
        public bool updateGiveCommandBeforeLogout(bool iGiveLogoutCommand)
        {
            giveLogoutCommand = iGiveLogoutCommand;
            return true;
        }
        public bool updateCommandBeforeLogout(String iLogoutCommand)
        {
            logoutCommand = iLogoutCommand;
            return true;
        }
        public bool updateDelayBetweenCasts(bool iValue)
        {
            delayBetweenCasts = iValue;
            return true;
        }
        public bool updateDelayValueBetweenCasts(uint iValue)
        {
            delayValueBetweenCasts = iValue;
            return true;
        }
        #endregion //Update Procedures

        //HACK ADD HERE
        private static bool checkStatusActive(String iStatusName)
        {
            try
            {
                ushort[] myStatusArray = null;
                MemReads.Self.StatusEffects.get_effects(ref myStatusArray);
                ushort[] inputStatusArray = Data.Client.StatusEffects.GetStatusEffectID(iStatusName);
                foreach (ushort inputID in inputStatusArray)
                {
                    if (myStatusArray.Contains(inputID))
                    {
                        //Status is active, return true.
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("SkillUp::checkStatusActive: " + e.ToString());
                return false;
            }
        }
    }
}
