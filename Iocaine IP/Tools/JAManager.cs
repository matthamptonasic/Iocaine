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
        public static class JAManager
        {
            #region Notes
            /*
         * The JA timers have 3 values:
         * 1. A unique ID, 1 per job ability which is in the resource files.
         * 2. A recast ID, some abilities share this. Also in the resource files.
         * 3. A structure index. This is the relative offset of the JA timer from the
         *    JA timers pointer. These values are found in memory, unknown statically.
         *    The structure looks like this:
         *    Addr: Ptr + 3         Ptr + 7         Ptr + 11
         *    Val:  1-hr JA (0)     RecastID A      RecastID B  etc...
         *                       ................
         *    Addr: Ptr + 124       Ptr + 128       Ptr + 132
         *    Val:  1-hr timer      Timer for A     Timer for B  etc...
         *    
         * So we use mem-reads to fill a list of recast ID's.
         * The index of the list is the structure index of the ID listed.
         * We also create maps for:
         * 1. Name to Structure Index
        */
            #endregion Notes
            #region Member Variables
            private static Boolean jaTimersInitDone;
            private static List<JobAbilCommand> allCommands = new List<JobAbilCommand>();
            private static List<JobAbilCommand> curCommands = new List<JobAbilCommand>();
            private static List<JobAbilCommand> pl_cureCommands = new List<JobAbilCommand>();
            private static List<JobAbilCommand> pl_partyBuffsCommands = new List<JobAbilCommand>();
            private static List<JobAbilCommand> pl_selfBuffsCommands = new List<JobAbilCommand>();
            private static List<JobAbilCommand> pl_healingCommands = new List<JobAbilCommand>();
            private static List<JobAbilities.JA_INFO> infoList;
            private static List<UInt16> RecastIDsList; //Holds current JA's in order of mem-reads.
            private static Dictionary<String, Byte> nameToStrctIndexMap; //Holds the recast timer structure ID for each JA name.
            private static Dictionary<String, UInt16> nameToRecastIDMap; //Holds the list of recasts ID's for each JA name (build time).
            #endregion Member Variables
            #region Properties
            internal static List<JobAbilCommand> AllCommands
            {
                get
                {
                    return allCommands;
                }
            }
            public static List<JobAbilCommand> CurrentCommands
            {
                get
                {
                    return curCommands;
                }
            }
            internal static List<JobAbilCommand> PL_CureCommands
            {
                get
                {
                    return pl_cureCommands;
                }
            }
            internal static List<JobAbilCommand> PL_PartyBuffsCommands
            {
                get
                {
                    return pl_partyBuffsCommands;
                }
            }
            internal static List<JobAbilCommand> PL_SelfBuffsCommands
            {
                get
                {
                    return pl_selfBuffsCommands;
                }
            }
            internal static List<JobAbilCommand> PL_HealingCommands
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
                    if (!jaTimersInitDone)
                    {
                        loadAllCommands();
                        jaTimersInitDone = true;
                    }
                    SetAbilities(PlayerCache.Vitals.MainJob, PlayerCache.Vitals.SubJob, PlayerCache.Vitals.MainJobLvl);
                    load_subSets();
                    Monitor.Exit(padlock);
                }
            }
            private static void loadAllCommands()
            {
                List<JobAbilities.JA_INFO> allJaInfo = JobAbilities.GetAbilityInfo();
                foreach (JobAbilities.JA_INFO info in allJaInfo)
                {
                    JobAbilCommand cmd = new JobAbilCommand(info);
                    allCommands.Add(cmd);
                }
            }
            private static void load_subSets()
            {
                pl_cureCommands.Clear();
                pl_partyBuffsCommands.Clear();
                pl_selfBuffsCommands.Clear();
                pl_healingCommands.Clear();
                List<JobAbilities.JA_INFO> pl_cureInfo = JobAbilities.GetAbilityInfo_Cures(PlayerCache.Vitals.MainJob, PlayerCache.Vitals.SubJob, PlayerCache.Vitals.MainJobLvl);
                List<JobAbilities.JA_INFO> pl_healingInfo = JobAbilities.GetAbilityInfo_HealingAbility(PlayerCache.Vitals.MainJob, PlayerCache.Vitals.SubJob, PlayerCache.Vitals.MainJobLvl);
                List<JobAbilities.JA_INFO> pl_partyBuffsInfo = JobAbilities.GetAbilityInfo_PartyAbility(PlayerCache.Vitals.MainJob, PlayerCache.Vitals.SubJob, PlayerCache.Vitals.MainJobLvl);
                List<JobAbilities.JA_INFO> pl_selfBuffsInfo = JobAbilities.GetAbilityInfo_SelfBuffs(PlayerCache.Vitals.MainJob, PlayerCache.Vitals.SubJob, PlayerCache.Vitals.MainJobLvl);

                foreach (JobAbilCommand cmd in allCommands)
                {
                    foreach (JobAbilities.JA_INFO info in pl_cureInfo)
                    {
                        if (cmd.Name == info.Name)
                        {
                            pl_cureCommands.Add(cmd);
                        }
                    }
                    foreach (JobAbilities.JA_INFO info in pl_partyBuffsInfo)
                    {
                        if (cmd.Name == info.Name)
                        {
                            pl_partyBuffsCommands.Add(cmd);
                        }
                    }
                    foreach (JobAbilities.JA_INFO info in pl_selfBuffsInfo)
                    {
                        if (cmd.Name == info.Name)
                        {
                            pl_selfBuffsCommands.Add(cmd);
                        }
                    }
                    foreach (JobAbilities.JA_INFO info in pl_healingInfo)
                    {
                        if (cmd.Name == info.Name)
                        {
                            pl_healingCommands.Add(cmd);
                        }
                    }
                }
            }
            private static void SetAbilities(Byte Job, Byte SubJob, Byte JobLevel)
            {
                FfxiResource.init();
                if (infoList == null)
                {
                    infoList = new List<JobAbilities.JA_INFO>();
                }
                else
                {
                    infoList.Clear();
                }
                infoList = JobAbilities.GetAbilityInfo(Job, SubJob, JobLevel);
                if (nameToRecastIDMap == null)
                {
                    nameToRecastIDMap = new Dictionary<String, UInt16>();
                }
                else
                {
                    nameToRecastIDMap.Clear();
                }
                foreach (JobAbilities.JA_INFO info in infoList)
                {
                    nameToRecastIDMap.Add(info.Name, (UInt16)info.RecastID);
                }
                if (ChangeMonitor.LoggedIn)
                {
                    RecastIDsList = MemReads.Self.Recast.Abilities.get_ability_indices();
                }
                else
                {
                    return;
                }
                if (nameToStrctIndexMap == null)
                {
                    nameToStrctIndexMap = new Dictionary<String, Byte>();
                }
                else
                {
                    nameToStrctIndexMap.Clear();
                }
                foreach (JobAbilities.JA_INFO info in infoList)
                {
                    nameToStrctIndexMap.Add(info.Name, (Byte)RecastIDsList.IndexOf((UInt16)info.RecastID));
                }
                curCommands.Clear();
                foreach (JobAbilCommand cmd in allCommands)
                {
                    cmd.Available = false;
                    if (nameToStrctIndexMap.ContainsKey(cmd.Name))
                    {
                        curCommands.Add(cmd);
                        cmd.Available = true;
                        cmd.RecastStructIndex = nameToStrctIndexMap[cmd.Name];
                    }
                }
            }
            public static UInt32 GetRecastTime(String AbilityName)
            {
                if (ChangeMonitor.LoggedIn)
                {
                    Init();
                    return MemReads.Self.Recast.Abilities.get_time_remaining((Byte)nameToStrctIndexMap[AbilityName]);
                }
                else
                {
                    return 0;
                }
            }
            public static UInt32 GetRecastTime(UInt16 RecastID)
            {
                if (ChangeMonitor.LoggedIn)
                {
                    Init();
                    return MemReads.Self.Recast.Abilities.get_time_remaining((Byte)RecastIDsList.IndexOf(RecastID));
                }
                else
                {
                    return 0;
                }
            }
            public static JobAbilCommand GetCommand(String iName)
            {
                foreach (JobAbilCommand cmd in allCommands)
                {
                    if (iName == cmd.Name)
                    {
                        return cmd;
                    }
                }
                return null;
            }
            #endregion Interface Functions
        }
    }
}
