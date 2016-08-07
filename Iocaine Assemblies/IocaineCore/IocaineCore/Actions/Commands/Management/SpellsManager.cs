using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Iocaine2.Data.Client;
using Iocaine2.Logging;
using Iocaine2.Memory;

namespace Iocaine2.Data.Structures
{
    public static partial class CommandManager
    {
        public static class SpellsManager
        {
            #region Private Members
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
            private static Boolean allSpellsSet = false;
            #endregion Flags
            #region Command Lists
            private static SpellCommand dummyCommand = new SpellCommand("Dummy");
            private static List<SpellCommand> allCommands = new List<SpellCommand>();
            private static List<SpellCommand> curCommands = new List<SpellCommand>();
            private static List<SpellCommand> pl_cureCommands = new List<SpellCommand>();
            private static List<SpellCommand> pl_partyBuffsCommands = new List<SpellCommand>();
            private static List<SpellCommand> pl_selfBuffsCommands = new List<SpellCommand>();
            private static List<SpellCommand> pl_healingCommands = new List<SpellCommand>();
            private static List<SpellCommand> su_enhancingCommands = new List<SpellCommand>();
            private static List<SpellCommand> su_healingCommands = new List<SpellCommand>();
            private static List<SpellCommand> su_summoningCommands = new List<SpellCommand>();
            private static List<SpellCommand> su_blueCommands = new List<SpellCommand>();
            private static List<SpellCommand> su_ninjutsuCommands = new List<SpellCommand>();
            private static List<SpellCommand> su_singingCommands = new List<SpellCommand>();
            private static List<SpellCommand> su_stringCommands = new List<SpellCommand>();
            private static List<SpellCommand> su_windCommands = new List<SpellCommand>();
            private static List<SpellCommand> su_geoCommands = new List<SpellCommand>();
            #endregion Command Lists
            #endregion Private Members

            #region Public Properties
            public static SpellCommand Dummy
            {
                get
                {
                    return dummyCommand;
                }
            }
            public static List<SpellCommand> AllCommands
            {
                get
                {
                    return allCommands;
                }
            }
            public static List<SpellCommand> CurrentCommands
            {
                get
                {
                    return curCommands;
                }
            }
            public static List<SpellCommand> PL_CureCommands
            {
                get
                {
                    return pl_cureCommands;
                }
            }
            public static List<SpellCommand> PL_PartyBuffsCommands
            {
                get
                {
                    return pl_partyBuffsCommands;
                }
            }
            public static List<SpellCommand> PL_SelfBuffsCommands
            {
                get
                {
                    return pl_selfBuffsCommands;
                }
            }
            public static List<SpellCommand> PL_HealingCommands
            {
                get
                {
                    return pl_healingCommands;
                }
            }
            public static List<SpellCommand> SU_EnhancingCommands
            {
                get
                {
                    return su_enhancingCommands;
                }
            }
            public static List<SpellCommand> SU_HealingCommands
            {
                get
                {
                    return su_healingCommands;
                }
            }
            public static List<SpellCommand> SU_SummoningCommands
            {
                get
                {
                    return su_summoningCommands;
                }
            }
            public static List<SpellCommand> SU_BlueCommands
            {
                get
                {
                    return su_blueCommands;
                }
            }
            public static List<SpellCommand> SU_NinjutsuCommands
            {
                get
                {
                    return su_ninjutsuCommands;
                }
            }
            public static List<SpellCommand> SU_SingingCommands
            {
                get
                {
                    return su_singingCommands;
                }
            }
            public static List<SpellCommand> SU_StringCommands
            {
                get
                {
                    return su_stringCommands;
                }
            }
            public static List<SpellCommand> SU_WindCommands
            {
                get
                {
                    return su_windCommands;
                }
            }
            public static List<SpellCommand> SU_GeoCommands
            {
                get
                {
                    return su_geoCommands;
                }
            }
            #endregion Public Properties

            #region Inits
            public static void Init()
            {
                if (!ChangeMonitor.LoggedIn)
                {
                    return;
                }
                else
                {
                    Monitor.Enter(padlock);
                    try
                    {
                        if (!allSpellsSet)
                        {
                            loadAllCommands();
                            allSpellsSet = true;
                        }

                        SetSpells();
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error(e.ToString());
                    }
                    finally
                    {
                        Monitor.Exit(padlock);
                    }
                }
            }
            #endregion Inits

            #region Public Methods
            public static void SetSpells()
            {
                List<Spells.SPELL_INFO> infoListCurrent = Spells.GetSpellInfo(PlayerCache.Vitals.MainJob, PlayerCache.Vitals.SubJob, PlayerCache.Vitals.MainJobLvl);
                List<Spells.SPELL_INFO> infoListCures = Spells.GetCuresInfo(PlayerCache.Vitals.MainJob, PlayerCache.Vitals.SubJob, PlayerCache.Vitals.MainJobLvl);
                List<Spells.SPELL_INFO> infoListPartyBuffs = Spells.GetPartyBuffsInfo(PlayerCache.Vitals.MainJob, PlayerCache.Vitals.SubJob, PlayerCache.Vitals.MainJobLvl);
                List<Spells.SPELL_INFO> infoListSelfBuffs = Spells.GetSelfBuffsInfo(PlayerCache.Vitals.MainJob, PlayerCache.Vitals.SubJob, PlayerCache.Vitals.MainJobLvl);
                List<Spells.SPELL_INFO> infoListHealing = Spells.GetHealingInfo(PlayerCache.Vitals.MainJob, PlayerCache.Vitals.SubJob, PlayerCache.Vitals.MainJobLvl);

                curCommands.Clear();
                pl_cureCommands.Clear();
                pl_partyBuffsCommands.Clear();
                pl_selfBuffsCommands.Clear();
                pl_healingCommands.Clear();
                su_enhancingCommands.Clear();
                foreach (SpellCommand cmd in allCommands)
                {
                    #region Current List
                    cmd.Available = false;
                    foreach (Spells.SPELL_INFO info in infoListCurrent)
                    {
                        if (info.Name == cmd.Name)
                        {
                            curCommands.Add(cmd);
                            cmd.Available = true;
                        }
                    }
                    #endregion Current List
                    #region PL Lists
                    foreach (Spells.SPELL_INFO info in infoListCures)
                    {
                        if (info.Name == cmd.Name)
                        {
                            pl_cureCommands.Add(cmd);
                        }
                    }
                    foreach (Spells.SPELL_INFO info in infoListPartyBuffs)
                    {
                        if (info.Name == cmd.Name)
                        {
                            pl_partyBuffsCommands.Add(cmd);
                        }
                    }
                    foreach (Spells.SPELL_INFO info in infoListSelfBuffs)
                    {
                        if (info.Name == cmd.Name)
                        {
                            pl_selfBuffsCommands.Add(cmd);
                        }
                    }
                    foreach (Spells.SPELL_INFO info in infoListHealing)
                    {
                        if (info.Name == cmd.Name)
                        {
                            pl_healingCommands.Add(cmd);
                        }
                    }
                    #endregion PL Lists
                    #region SU Lists
                    if (cmd.Skill == sk_Enhancing)
                    {
                        su_enhancingCommands.Add(cmd);
                    }
                    if ((cmd.Skill == sk_Healing) && (cmd.Targets != Spells.TARGETS.DEAD_PC) && (cmd.Targets != Spells.TARGETS.MOB))// && (cmd.SpellType != "Trust"))
                    {
                        su_healingCommands.Add(cmd);
                    }
                    if (cmd.Skill == sk_Summoning)
                    {
                        su_summoningCommands.Add(cmd);
                    }
                    if ((cmd.Skill == sk_Blue) && (cmd.Targets != Spells.TARGETS.MOB))
                    {
                        su_blueCommands.Add(cmd);
                    }
                    if ((cmd.Skill == sk_Ninjutsu) && (cmd.Targets != Spells.TARGETS.MOB))
                    {
                        su_ninjutsuCommands.Add(cmd);
                    }
                    if ((cmd.Skill == sk_Singing) && (cmd.Targets != Spells.TARGETS.MOB))
                    {
                        su_singingCommands.Add(cmd);
                    }
                    if ((cmd.Skill == sk_Singing) && (cmd.Targets != Spells.TARGETS.MOB))
                    {
                        su_stringCommands.Add(cmd);
                    }
                    if ((cmd.Skill == sk_Singing) && (cmd.Targets != Spells.TARGETS.MOB))
                    {
                        su_windCommands.Add(cmd);
                    }
                    if ((cmd.Skill == sk_Geomancy) && (cmd.Targets != Spells.TARGETS.MOB))
                    {
                        su_geoCommands.Add(cmd);
                    }
                    #endregion SU Lists
                }
            }
            #endregion Public Methods

            #region Private Methods
            private static void loadAllCommands()
            {
                List<Spells.SPELL_INFO> allSpellsInfo = Spells.GetSpellInfo();
                foreach (Spells.SPELL_INFO info in allSpellsInfo)
                {
                    SpellCommand cmd = new SpellCommand(info);
                    allCommands.Add(cmd);
                }
            }
            #endregion Private Methods
        }
    }
}
