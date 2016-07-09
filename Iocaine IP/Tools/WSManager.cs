using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Iocaine2.Data;
using Iocaine2.Memory;

namespace Iocaine2.Data.Structures
{
    public static partial class CommandManager
    {
        public static class WSManager
        {
            #region Member Variables
            private static Boolean allWeaponSkillsSet = false;
            private static List<WeaponSkillCommand> allCommands = new List<WeaponSkillCommand>();
            private static List<WeaponSkillCommand> curCommands = new List<WeaponSkillCommand>();
            private static List<Client.WeaponSkills.WS_INFO> infoList = new List<Client.WeaponSkills.WS_INFO>();
            private static UInt16 currentWeaponType = 0;
            #endregion Member Variables
            #region Properties
            public static List<WeaponSkillCommand> CurrentCommands
            {
                get
                {
                    return curCommands;
                }
            }
            internal static List<WeaponSkillCommand> AllCommands
            {
                get
                {
                    return allCommands;
                }
            }
            #endregion Properties
            #region Interface Functions
            public static void Init()
            {
                if (ChangeMonitor.LoggedIn)
                {
                    Monitor.Enter(padlock);
                    if (!allWeaponSkillsSet)
                    {
                        loadAllCommands();
                        allWeaponSkillsSet = true;
                    }

                    //if (!curWeaponSkillsSet ||
                    //    (Statics.MyData.MainJob != MemReads.Self.Job.get_main()) ||
                    //    (Statics.MyData.MainJobLvl != MemReads.Self.Job.get_main_lvl()) ||
                    //    (Statics.MyData.SubJob != MemReads.Self.Job.get_sub()) ||
                    //    (Statics.MyData.SubJobLvl != MemReads.Self.Job.get_sub_lvl()) ||
                    //    (Statics.MyData.EquipMain != MemReads.Self.Equipment.get_main_id()) ||
                    //    (Statics.MyData.CombatSkillCurrentLvl != MemReads.Self.Skills.get_skill(currentWeaponType)))
                    //{
                    //    Statics.MyData.MainJob = MemReads.Self.Job.get_main();
                    //    Statics.MyData.MainJobLvl = MemReads.Self.Job.get_main_lvl();
                    //    Statics.MyData.SubJob = MemReads.Self.Job.get_sub();
                    //    Statics.MyData.SubJobLvl = MemReads.Self.Job.get_sub_lvl();
                    //    Statics.MyData.EquipMain = MemReads.Self.Equipment.get_main_id();
                    currentWeaponType = Client.Things.GetSkillFromId(PlayerCache.Equipment.Main);
                    PlayerCache.Skills.CombatSkillCurrentLvl = MemReads.Self.Skills.get_skill(currentWeaponType);
                    SetWeaponSkills();
                    //}
                    Monitor.Exit(padlock);
                }
            }
            private static void loadAllCommands()
            {
                List<Client.WeaponSkills.WS_INFO> allWsInfo = Client.WeaponSkills.GetWeaponSkillInfo();
                foreach (Client.WeaponSkills.WS_INFO info in allWsInfo)
                {
                    WeaponSkillCommand cmd = new WeaponSkillCommand(info);
                    allCommands.Add(cmd);
                }
            }
            public static void SetWeaponSkills()
            {
                infoList = Client.WeaponSkills.GetWeaponSkillInfo((Byte)currentWeaponType, PlayerCache.Vitals.MainJob, PlayerCache.Vitals.SubJob, (UInt16)PlayerCache.Skills.CombatSkillCurrentLvl);

                curCommands.Clear();
                foreach (WeaponSkillCommand cmd in allCommands)
                {
                    cmd.Available = false;

                    foreach (Client.WeaponSkills.WS_INFO info in infoList)
                    {
                        if (info.Name == cmd.Name)
                        {
                            curCommands.Add(cmd);
                            cmd.Available = true;
                        }
                    }
                }
            }
            #endregion Interface Functions
        }
    }
}
