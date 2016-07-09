using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Iocaine2.Data.Client;
using Iocaine2.Memory;

namespace Iocaine2.Data.Structures
{
    public static partial class CommandManager
    {
        #region Member Variables
        #region Skill Types
        private static UInt16 sk_Divine = Skills.GetSkillId("Divine Magic");
        private static UInt16 sk_Healing = Skills.GetSkillId("Healing Magic");
        private static UInt16 sk_Enhancing = Skills.GetSkillId("Enhancing Magic");
        private static UInt16 sk_Enfeebling = Skills.GetSkillId("Enfeebling Magic");
        private static UInt16 sk_Elemental = Skills.GetSkillId("Elemental Magic");
        private static UInt16 sk_Dark = Skills.GetSkillId("Dark Magic");
        private static UInt16 sk_Summoning = Skills.GetSkillId("Summoning Magic");
        private static UInt16 sk_Ninjutsu = Skills.GetSkillId("Ninjutsu");
        private static UInt16 sk_Singing = Skills.GetSkillId("Singing");
        private static UInt16 sk_Stringed = Skills.GetSkillId("Stringed Instrument");
        private static UInt16 sk_Wind = Skills.GetSkillId("Wind Instrument");
        private static UInt16 sk_Blue = Skills.GetSkillId("Blue Magic");
        private static UInt16 sk_Geomancy = Skills.GetSkillId("Geomancy");
        private static UInt16 sk_Handbell = Skills.GetSkillId("Handbell");
        #endregion Skill Types
        #region Flags
        private static object padlock = new object();
        private static Boolean allCmdsSet = false;
        #endregion Flags
        #region Command Lists
        private static List<Command> allCommands = new List<Command>();
        private static List<Command> currentCommands = new List<Command>();
        private static List<Command> pl_cureCommands = new List<Command>();
        private static List<Command> pl_partyBuffsCommands = new List<Command>();
        private static List<Command> pl_selfBuffsCommands = new List<Command>();
        private static List<Command> pl_healingCommands = new List<Command>();
        #endregion Command Lists
        #endregion Member Variables
        #region Properties
        public static Command Dummy
        {
            get
            {
                return SpellsManager.Dummy;
            }
        }
        public static List<Command> AllCommands
        {
            get
            {
                return allCommands;
            }
        }
        public static List<Command> CurrentCommands
        {
            get
            {
                return currentCommands;
            }
        }
        public static List<Command> PL_CureCommands
        {
            get
            {
                return pl_cureCommands;
            }
        }
        public static List<Command> PL_PartyBuffsCommands
        {
            get
            {
                return pl_partyBuffsCommands;
            }
        }
        public static List<Command> PL_SelfBuffsCommands
        {
            get
            {
                return pl_selfBuffsCommands;
            }
        }
        public static List<Command> PL_HealingCommands
        {
            get
            {
                return pl_healingCommands;
            }
        }
        #endregion Properties
        #region Interface Functions
        public static void Init()
        {
            if (!ChangeMonitor.LoggedIn)
            {
                return;
            }
            else
            {
                Monitor.Enter(padlock);   
                if (!allCmdsSet)
                {
                    loadAllCommands();
                    allCmdsSet = true;
                }
                loadCommandSets();
                Monitor.Exit(padlock);
            }
        }
        private static void loadAllCommands()
        {
            allCommands.Clear();
            foreach (SpellCommand cmd in SpellsManager.AllCommands)
            {
                allCommands.Add(cmd);
            }
            foreach(JobAbilCommand cmd in JAManager.AllCommands)
            {
                allCommands.Add(cmd);
            }
        }
        private static void loadCommandSets()
        {
            loadCurrentCmds();
            loadCureCmds();
            loadPartyBuffCmds();
            loadSelfBuffCmds();
            loadHealingCmds();
        }
        private static void loadCurrentCmds()
        {
            currentCommands.Clear();
            foreach(SpellCommand cmd in SpellsManager.CurrentCommands)
            {
                currentCommands.Add(cmd);
            }
            foreach(JobAbilCommand cmd in JAManager.CurrentCommands)
            {
                currentCommands.Add(cmd);
            }
        }
        private static void loadCureCmds()
        {
            pl_cureCommands.Clear();
            foreach(SpellCommand cmd in SpellsManager.PL_CureCommands)
            {
                pl_cureCommands.Add(cmd);
            }
            foreach(JobAbilCommand cmd in JAManager.PL_CureCommands)
            {
                pl_cureCommands.Add(cmd);
            }
        }
        private static void loadPartyBuffCmds()
        {
            pl_partyBuffsCommands.Clear();
            foreach (SpellCommand cmd in SpellsManager.PL_PartyBuffsCommands)
            {
                pl_partyBuffsCommands.Add(cmd);
            }
            foreach (JobAbilCommand cmd in JAManager.PL_PartyBuffsCommands)
            {
                pl_partyBuffsCommands.Add(cmd);
            }
        }
        private static void loadSelfBuffCmds()
        {
            pl_selfBuffsCommands.Clear();
            foreach (SpellCommand cmd in SpellsManager.PL_SelfBuffsCommands)
            {
                pl_selfBuffsCommands.Add(cmd);
            }
            foreach (JobAbilCommand cmd in JAManager.PL_SelfBuffsCommands)
            {
                pl_selfBuffsCommands.Add(cmd);
            }
        }
        private static void loadHealingCmds()
        {
            pl_healingCommands.Clear();
            foreach (SpellCommand cmd in SpellsManager.PL_HealingCommands)
            {
                pl_healingCommands.Add(cmd);
            }
            foreach (JobAbilCommand cmd in JAManager.PL_HealingCommands)
            {
                pl_healingCommands.Add(cmd);
            }
        }
        #endregion Interface Functions
    }
}
