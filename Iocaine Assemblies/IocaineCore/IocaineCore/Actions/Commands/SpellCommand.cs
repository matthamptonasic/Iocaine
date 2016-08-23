using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;
using System.Windows.Forms;

using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;

namespace Iocaine2.Data.Structures
{
    public class SpellCommand : Command
    {
        #region Private Members
        private Client.Spells.SPELL_INFO _SpellInfo;
        private bool available = false;
        private static ushort m_ninjutsuSkillId;
        private static byte m_ninjaJobId;
        private static bool m_ninjaToolsLoaded = false;
        private static Dictionary<ushort, List<ushort>> m_ninjutsuIdToToolIdMap = new Dictionary<ushort, List<ushort>>();
        private static List<string> m_ninjaToolNames = new List<string> {
            "Chonofuda", "Inoshishinofuda", "Shikanofuda",
            "Soshi", "Jinko", "Jusatsu", "Kaginawa", "Sairui-Ran", "Kodoku",
            "Uchitake", "Tsurara", "Kawahori-Ogi", "Makibishi", "Hiraishin", "Mizu-Deppo",
            "Shihei", "Shinobi-Tabi", "Sanjaku-Tenugui", "Kabenro", "Mokujin", "Ranka", "Furusumi", "Ryuno"
        };
        private static Dictionary<string, ushort> m_ninjaToolNameToIdMap = new Dictionary<string, ushort>();
        private static Dictionary<string, List<string>> m_ninjutsuNameToToolNamesMap = new Dictionary<string, List<string>> {
            // Enfeebling
            { "Aisha: Ichi", new List<string> { "Soshi", "Chonofuda" } },
            { "Yurin: Ichi", new List<string> { "Jinko", "Chonofuda" } },
            { "Jubaku: Ichi", new List<string> { "Jusatsu", "Chonofuda" } },
            { "Jubaku: Ni", new List<string> { "Jusatsu", "Chonofuda" } },
            { "Hojo: Ichi", new List<string> { "Kaginawa", "Chonofuda" } },
            { "Hojo: Ni", new List<string> { "Kaginawa", "Chonofuda" } },
            { "Hojo: San", new List<string> { "Kaginawa", "Chonofuda" } },
            { "Kurayami: Ichi", new List<string> { "Sairui-Ran", "Chonofuda" } },
            { "Kurayami: Ni", new List<string> { "Sairui-Ran", "Chonofuda" } },
            { "Kurayami: San", new List<string> { "Sairui-Ran", "Chonofuda" } },
            { "Dokumori: Ichi", new List<string> { "Kodoku", "Chonofuda" } },
            { "Dokumori: Ni", new List<string> { "Kodoku", "Chonofuda" } },
            // Elemental
            { "Katon: Ichi", new List<string> { "Uchitake", "Inoshishinofuda" } },
            { "Katon: Ni", new List<string> { "Uchitake", "Inoshishinofuda" } },
            { "Katon: San", new List<string> { "Uchitake", "Inoshishinofuda" } },
            { "Hyoton: Ichi", new List<string> { "Tsurara", "Inoshishinofuda" } },
            { "Hyoton: Ni", new List<string> { "Tsurara", "Inoshishinofuda" } },
            { "Hyoton: San", new List<string> { "Tsurara", "Inoshishinofuda" } },
            { "Huton: Ichi", new List<string> { "Kawahori-Ogi", "Inoshishinofuda" } },
            { "Huton: Ni", new List<string> { "Kawahori-Ogi", "Inoshishinofuda" } },
            { "Huton: San", new List<string> { "Kawahori-Ogi", "Inoshishinofuda" } },
            { "Doton: Ichi", new List<string> { "Makibishi", "Inoshishinofuda" } },
            { "Doton: Ni", new List<string> { "Makibishi", "Inoshishinofuda" } },
            { "Doton: San", new List<string> { "Makibishi", "Inoshishinofuda" } },
            { "Raiton: Ichi", new List<string> { "Hiraishin", "Inoshishinofuda" } },
            { "Raiton: Ni", new List<string> { "Hiraishin", "Inoshishinofuda" } },
            { "Raiton: San", new List<string> { "Hiraishin", "Inoshishinofuda" } },
            { "Suiton: Ichi", new List<string> { "Mizu-Deppo", "Inoshishinofuda" } },
            { "Suiton: Ni", new List<string> { "Mizu-Deppo", "Inoshishinofuda" } },
            { "Suiton: San", new List<string> { "Mizu-Deppo", "Inoshishinofuda" } },
            // Enhancing
            { "Utsusemi: Ichi", new List<string> { "Shihei", "Shikanofuda" } },
            { "Utsusemi: Ni", new List<string> { "Shihei", "Shikanofuda" } },
            { "Utsusemi: San", new List<string> { "Shihei", "Shikanofuda" } },
            { "Tonko: Ichi", new List<string> { "Shinobi-Tabi", "Shikanofuda" } },
            { "Tonko: Ni", new List<string> { "Shinobi-Tabi", "Shikanofuda" } },
            { "Tonko: San", new List<string> { "Shinobi-Tabi", "Shikanofuda" } },
            { "Monomi: Ichi", new List<string> { "Sanjaku-Tenugui", "Shikanofuda" } },
            { "Myoshu: Ichi", new List<string> { "Kabenro", "Shikanofuda" } },
            { "Migawari: Ichi", new List<string> { "Mokujin", "Shikanofuda" } },
            { "Gekka: Ichi", new List<string> { "Ranka", "Shikanofuda" } },
            { "Yain: Ichi", new List<string> { "Furusumi", "Shikanofuda" } },
            { "Kakka: Ichi", new List<string> { "Ryuno", "Shikanofuda" } }
        };
        #endregion Private Members

        #region Public Properties
        public override bool Available
        {
            get
            {
                return available;
            }
            set
            {
                available = value;
            }
        }
        public override bool Ready
        {
            get
            {
                return (TimeRemaining == 0) && available;
            }
        }
        public uint TimeRemaining
        {
            get
            {
                return MemReads.Self.Recast.Magic.get_time_remaining(_SpellInfo.ID);
            }
        }
        public override ushort MP
        {
            get
            {
                return _SpellInfo.MP;
            }
        }
        public Client.Spells.TARGETS Targets
        {
            get
            {
                return (Client.Spells.TARGETS)_SpellInfo.Targets;
            }
        }
        public byte Skill
        {
            get
            {
                return _SpellInfo.Skill;
            }
        }
        public short Element
        {
            get
            {
                return _SpellInfo.Element;
            }
        }
        public ushort Range
        {
            get
            {
                return _SpellInfo.Range;
            }
        }
        public override uint ExecTime
        {
            get
            {
                return (uint)Math.Ceiling(_SpellInfo.CastTime);
            }
        }
        public override uint Duration
        {
            get
            {
                return _SpellInfo.Duration;
            }
        }
        public string SpellType
        {
            get
            {
                return _SpellInfo.Type;
            }
        }
        public static ushort NinjutsuSkillId
        {
            set
            {
                m_ninjutsuSkillId = value;
            }
        }
        #endregion Public Properties

        #region Constructors
        public SpellCommand(string iName)
            : base(iName, CMD_TYPE.SPELL, true)
        {
            if (iName != "Dummy")
            {
                ushort Id = Client.Spells.GetSpellID(iName);
                _SpellInfo = Client.Spells.GetSpellInfo(Id);
                setConditionTrees();
            }
            else
            {
                setDummyInfo();
            }
        }
        public SpellCommand(Client.Spells.SPELL_INFO iInfo)
            : base(iInfo.Name, CMD_TYPE.SPELL, true)
        {
            _SpellInfo = Iocaine2.Data.Client.Spells.GetSpellInfo(iInfo.ID);
            setConditionTrees();
        }
        #endregion Constructors
        
        #region Public Methods
        public override bool Execute(string iTarget)
        {
            if (MemReads.Self.Casting.is_casting())
            {
                return false;
            }
            try
            {
                if (CanPerform())
                {
                    IocaineFunctions.keys(_SpellInfo.Command + " " + iTarget);
                    LoggingFunctions.Debug(_SpellInfo.Command + " " + iTarget, LoggingFunctions.DBG_SCOPE.COMMANDS);
                    // Wait a bit for the spell to start.
                    IocaineFunctions.delay(1000);
                    do
                    {
                        IocaineFunctions.delay(100);
                    }
                    while (MemReads.Self.Casting.is_casting());
                    IocaineFunctions.delay(Statics.Settings.PowerLevel.CastTimeMargin);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                string msg = "SpellCommand.Execute caught an exception!";
                msg += " Command: '" + _SpellInfo.Command + "'";
                msg += ", Target: '" + iTarget + "'";
                msg += ", Name: '" + _SpellInfo.Name + "'";
                msg += "\n" + e.ToString();
                LoggingFunctions.Error(msg);
                return false;
            }
            return true;
        }
        public override void Show()
        {
            MessageBox.Show(this.ToString(), "SpellCommand::" + Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override string SaveString()
        {
            return "SpellCommand;" + _SpellInfo.Name;
        }
        public override string ToString()
        {
            return _SpellInfo.Name;
        }
        #endregion Public Methods

        #region Private Methods
        private void setConditionTrees()
        {
            ConditionTree treeStatic = new ConditionTree();
            ConditionTree treeDynamic = new ConditionTree();
            try
            {
                for (byte ii = Client.Jobs.MinID; ii <= Client.Jobs.MaxID; ii++)
                {
                    if (_SpellInfo.JobLevels[ii] != 0)
                    {
                        treeStatic.PushOr(new JobLevel(Client.Jobs.InfoMap[ii], _SpellInfo.JobLevels[ii], 99, _SpellInfo.AsSub ? JobLevel.MAIN_SUB.EITHER : JobLevel.MAIN_SUB.MAIN_ONLY));
                    }
                }

                treeDynamic.PushAnd(new MPCurrentMin(_SpellInfo.MP));
                treeDynamic.PushAnd(new RecastReadySpell(_SpellInfo.ID));

                if (!m_ninjaToolsLoaded)
                {
                    loadNinjaTools();
                }
                if ((_SpellInfo.Skill == (byte)m_ninjutsuSkillId) && (_SpellInfo.JobLevels[m_ninjaJobId] != 0))
                {
                    // Make sure we have tools for it.
                    treeDynamic.PushAnd(new ConditionOr(new InventoryItem(m_ninjutsuNameToToolNamesMap[_SpellInfo.Name][0], m_ninjutsuIdToToolIdMap[_SpellInfo.ID][0]),
                                                     new InventoryItem(m_ninjutsuNameToToolNamesMap[_SpellInfo.Name][1], m_ninjutsuIdToToolIdMap[_SpellInfo.ID][1]),
                                                     _SpellInfo.Name + "_Tools"));
                }

                setConditions(treeStatic, treeDynamic);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error(e.ToString());
            }
        }
        private void setDummyInfo()
        {
            _SpellInfo = new Client.Spells.SPELL_INFO();
            _SpellInfo.AsSub = false;
            _SpellInfo.CastTime = 1;
            _SpellInfo.Command = "/echo Dummy command";
            _SpellInfo.Duration = 0;
            _SpellInfo.Element = 0;
            _SpellInfo.ID = 0;
            _SpellInfo.JobLevels = new Dictionary<byte, byte>() { { 1, 0 }, { 2, 0 },
                                                                  { 3, 0 }, { 4, 0 },
                                                                  { 5, 0 }, { 6, 0 },
                                                                  { 7, 0 }, { 8, 0 },
                                                                  { 9, 0 }, { 10, 0 },
                                                                  { 11, 0 }, { 12, 0 },
                                                                  { 13, 0 }, { 14, 0 },
                                                                  { 15, 0 }, { 16, 0 },
                                                                  { 17, 0 }, { 18, 0 },
                                                                  { 19, 0 }, { 20, 0 },
                                                                  { 21, 0 }, { 22, 0 },
                                                                  { 23, 0 } };
            _SpellInfo.MP = 0;
            _SpellInfo.Name = "Dummy";
            _SpellInfo.Range = 0;
            _SpellInfo.RecastID = 0xFFFF;
            _SpellInfo.Skill = 0;
            _SpellInfo.Targets = 0;
            _SpellInfo.Type = "Dummy";
        }
        private static void loadNinjaTools()
        {
            // Load the tool ID's.
            foreach (string tool in m_ninjaToolNames)
            {
                m_ninjaToolNameToIdMap.Add(tool, Client.Things.GetIdFromName(tool));
            }

            m_ninjutsuSkillId = Client.Skills.GetSkillId("Ninjutsu");
            List<Client.Spells.SPELL_INFO> _info = Client.Spells.GetSpellInfoBySkill(m_ninjutsuSkillId);
            m_ninjaJobId = Client.Jobs.GetJobId("Ninja", false);
            foreach (Client.Spells.SPELL_INFO info in _info)
            {
                if (info.JobLevels[m_ninjaJobId] == 0)
                {
                    continue;
                }
                List<string> toolNames;
                if (m_ninjutsuNameToToolNamesMap.ContainsKey(info.Name))
                {
                    toolNames = m_ninjutsuNameToToolNamesMap[info.Name];
                }
                else
                {
                    LoggingFunctions.Error("Could not find ninjutsu => tool name for '" + info.Name + "'");
                    continue;
                }
                List<ushort> toolIds = new List<ushort>();
                foreach (string toolName in toolNames)
                {
                    if (m_ninjaToolNameToIdMap.ContainsKey(toolName))
                    {
                        toolIds.Add(m_ninjaToolNameToIdMap[toolName]);
                    }
                    else
                    {
                        LoggingFunctions.Error("Could not find ninja tool name => tool id for '" + toolName + "'");
                        continue;
                    }
                }
                m_ninjutsuIdToToolIdMap.Add(info.ID, toolIds);
            }

            m_ninjaToolsLoaded = true;
        }
        #endregion Private Methods
    }
}
