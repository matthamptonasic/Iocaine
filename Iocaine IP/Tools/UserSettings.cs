using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Logging;

namespace Iocaine2.Settings
{
    public static class UserSettings
    {
        #region Enums
        public enum BOT
        {
            FISHER = 0,
            PL = 1,
            SU = 2,
            CRAFTER = 3,
            TA = 4,
            TRADER = 5,
            WMS = 6,
            FISHSTATS = 7,
            NAV = 8,
            TOP = 9,
            HELPERS = 10,
            POS = 11
        }
        public enum LIST_TABLE
        {
            FISHER_CATCHBOX = 0,
            FISHER_DROPBOX = 1,
            FISHER_BAITBOX = 2,
            PL_DEBUFFS = 3,
            PL_PARTYBUFFS = 4,
            PL_SELFBUFFS = 5,
            SU_COMMANDS = 6,
            FISHSTATS_RODFILTERS = 7,
            ALR_WHITELIST = 8
        }
        #endregion Enums
        #region Private Member Data
        private static bool initDone = false;
        private static String charName = "";
        private static USDataSet[] usDataSets;
        private static String commonSettingsBotName = "Top";
        private static String commonSettingsFileName = "Common_Settings";
        private static String[] botNames = { "Fisher", "PL", "SU", "Crafter", "TA", "Trader", "WMS", "FishStats", "NAV", "Top", "Helpers", "POS" };
        private static String[] tableNames = { "CatchBox", "DropBox", "BaitBox", "Debuffs", "PartyBuffs", "SelfBuffs", "Commands", "RodFilters", "Whitelist" };
        private static List<List<String>>[] listTables = { new List<List<String>>() { 
                                                               new List<String>() {"CatchBox", "Fish", "System.String"}, 
                                                               new List<String>() {"DropBox", "Item", "System.String", "Checked", "System.Boolean"},
                                                               new List<String>() {"BaitBox", "Bait", "System.String", "Checked", "System.Boolean", "Priority", "System.Byte"} },
                                                           new List<List<String>>() { 
                                                               new List<String>() {"Debuffs", "Name", "System.String"},
                                                               new List<String>() {"PartyBuffs", "Name", "System.String", "Recast", "System.UInt32"},
                                                               new List<String>() {"SelfBuffs", "Name", "System.String", "Recast", "System.UInt32"} },
                                                           new List<List<String>>() {
                                                               new List<String>() {"Commands", "Name", "System.String" } },
                                                           null,                                          //Crafter
                                                           null,                                          //TA
                                                           null,                                          //Trader
                                                           null,                                          //WMS
                                                           new List<List<String>>() {                     //Fish Stats
                                                               new List<String>() {"RodFilters", "Name", "System.String"} },
                                                           null,                                          //NAV
                                                           new List<List<String>>() {                     //Top
                                                               new List<String>() {"Whitelist", "Name", "System.String"} },
                                                           null,                                          //Helpers
                                                           null                                           //POS
                                                         };
        private static String[] fileNamesPrefix = { "Settings_",
                                                    "Settings_",
                                                    "Settings_",
                                                    "Settings_",
                                                    "Settings_",
                                                    "Settings_",
                                                    "Settings_",
                                                    "Settings_",
                                                    "Settings_",
                                                    "Settings_",
                                                    "Settings_",
                                                    "Settings_" };
        private static String[] fileNamesSuffix = { "_Fisher",
                                                    "_PL",
                                                    "_SU",
                                                    "_Crafter",
                                                    "_TA",
                                                    "_Trader",
                                                    "_WMS",
                                                    "_FishStats",
                                                    "_NAV",
                                                    "_Top",
                                                    "_Helpers",
                                                    "_POS" };
        private static String filePath = ".\\Settings\\";
        #region Default Values/Types Init
        #region Fisher
        private static Dictionary<String, String> fisherDefaultValues = new Dictionary<string, string>()
        {
            {"KillFish", "false"},
            {"KillFishFixed", "false"},
            {"KillFishFixedTime", "10"},
            {"KillFishRand", "false"},
            {"KillFishRandTimeMin", "8"},
            {"KillFishRandTimeMax", "15"},
            {"KillFishProp", "true"},
            {"KillFishPropTimeMin", "11"},
            {"KillFishPropTimeMax", "18"},
            {"DoneWait", "true"},
            {"DoneShutdown", "false"},
            {"DoneStop", "false"},
            {"DoneChange", "false"},
            {"DoneNav", "false"},
            {"DoneNavTrip", ""},
            {"DonePlaySound", "Fishing Complete."},
            {"FullStop", "true"},
            {"FullShutdown", "false"},
            {"FullChange", "false"},
            {"FullContinue", "false"},
            {"FullNav", "false"},
            {"FullNavTrip", ""},
            {"FullPlaySound", "Inventory Full."},
            {"FatiguedNav", "false"},
            {"FatiguedNavTrip", ""},
            {"FatigueThreshold", "30"},
            {"GiveLogoutCommand", "false"},
            {"LogoutCommand", "/ma Warp <me>"},
            {"FishByID", "true"},
            {"FishByHP", "false"},
            {"DropItems", "false"},
            {"DropMobs", "true"},
            {"FishByHPMin", "4500"},
            {"FishByHPMax", "9000"},
            {"NoCatchTimeout", "25"},
            {"ReleaseFixed", "false"},
            {"ReleaseTime", "2.8"},
            {"ReleaseRandom", "true"},
            {"ReleaseTimeRandomMin", "1.5"},
            {"ReleaseTimeRandomMax", "3.1"},
            {"TimedStart", "false"},
            {"StartTime", "2010-05-26T08:00:00-07:00"},
            {"TimedEnd", "false"},
            {"EndTime", "2010-05-26T10:00:00-07:00"},
            {"CatchAll", "false"},
            {"CatchAllExcept", "true"},
            {"MoveInv", "true"},
            {"InitWaitDelay", "1000"}
        };
        private static Dictionary<String, String> fisherDefaultTypes = new Dictionary<string, string>()
        {
            {"KillFish", "bool"},
            {"KillFishFixed", "bool"},
            {"KillFishFixedTime", "int"},
            {"KillFishRand", "bool"},
            {"KillFishRandTimeMin", "int"},
            {"KillFishRandTimeMax", "int"},
            {"KillFishProp", "bool"},
            {"KillFishPropTimeMin", "int"},
            {"KillFishPropTimeMax", "int"},
            {"DoneWait", "bool"},
            {"DoneShutdown", "bool"},
            {"DoneStop", "bool"},
            {"DoneChange", "bool"},
            {"DoneNav", "bool"},
            {"DoneNavTrip", "String"},
            {"DonePlaySound", "String"},
            {"FullStop", "bool"},
            {"FullShutdown", "bool"},
            {"FullChange", "bool"},
            {"FullContinue", "bool"},
            {"FullNav", "bool"},
            {"FullNavTrip", "String"},
            {"FullPlaySound", "String"},
            {"FatiguedNav", "bool"},
            {"FatiguedNavTrip", "String"},
            {"FatigueThreshold", "ushort"},
            {"GiveLogoutCommand", "bool"},
            {"LogoutCommand", "String"},
            {"FishByID", "bool"},
            {"FishByHP", "bool"},
            {"DropItems", "bool"},
            {"DropMobs", "bool"},
            {"FishByHPMin", "int"},
            {"FishByHPMax", "int"},
            {"NoCatchTimeout", "int"},
            {"ReleaseFixed", "bool"},
            {"ReleaseTime", "double"},
            {"ReleaseRandom", "bool"},
            {"ReleaseTimeRandomMin", "double"},
            {"ReleaseTimeRandomMax", "double"},
            {"TimedStart", "bool"},
            {"StartTime", "DateTime"},
            {"TimedEnd", "bool"},
            {"EndTime", "DateTime"},
            {"CatchAll", "bool"},
            {"CatchAllExcept", "bool"},
            {"MoveInv", "bool"},
            {"InitWaitDelay", "int"}
        };
        #endregion Fisher
        #region PL
        private static Dictionary<String, String> plDefaultValues = new Dictionary<string, string>()
        {
            {"FirstCureDefault", "Cure II"},
            {"SecondCureDefault", "Cure III"},
            {"ThirdCureDefault", "Cure IV"},
            {"FirstCureDefaultPerc", "85"},
            {"SecondCureDefaultPerc", "65"},
            {"ThirdCureDefaultPerc", "35"},
            {"ProtectDefault", "Protect II"},
            {"ShellDefault", "Shell II"},
            {"HasteDefault", "Haste"},
            {"RefreshDefault", "Refresh"},
            {"ProtectDefaultEnable", "false"},
            {"ShellDefaultEnable", "false"},
            {"HasteDefaultEnable", "false"},
            {"RefreshDefaultEnable", "false"},
            {"ProtectDefaultRecast", "1740000"},
            {"ShellDefaultRecast", "1740000"},
            {"HasteDefaultRecast", "180000"},
            {"RefreshDefaultRecast", "150000"},
            {"PlCharActivePollFrequency", "3000"},
            {"MaxCastDistance", "19"},
            {"HealthUpdateFrequency", "300"},
            {"DequeuePollFrequency", "400"},
            {"CastTimeModifier", "105"},
            {"CastTimeModifierCures", "100"},
            {"CastTimeMargin", "3500"},
            {"JaProcTime", "7000"},
            {"HealMpPercLow", "15"},
            {"HealMpPercHigh", "80"},
            {"RestInFight", "true"},
            {"RestInFightAlways", "false"},
            {"RestInFightLessUpper", "true"},
            {"RestInFightLessLower", "true"},
            {"UpperMpMargin", "40"},
            {"NeverRest", "false"},
            {"RestingStyle", "3"},
            {"FollowDistance", "1"},
            {"UseElasticFollowing", "true"},
            {"ElasticDistance", "1"},
            {"ZoneTimer", "10"},
            {"WakeCure", "Cure"},
            {"BuffQ", "4"},
            {"SelfBuffQ", "5"},
            {"DebuffQ", "2"},
            {"WakeCureQ", "2"},
            {"ThirdCureUseInFight", "true"},
            {"ThirdCureStopRestMpLow", "true"},
            {"ThirdCureStopRestMpHigh", "true"},
            {"ThirdCureMpLow", "true"},
            {"ThirdCureMpHigh", "true"},
            {"SecondCureUseInFight", "true"},
            {"SecondCureStopRestMpLow", "true"},
            {"SecondCureStopRestMpHigh", "true"},
            {"SecondCureMpLow", "true"},
            {"SecondCureMpHigh", "true"},
            {"FirstCureUseInFight", "true"},
            {"FirstCureStopRestMpLow", "false"},
            {"FirstCureStopRestMpHigh", "false"},
            {"FirstCureMpLow", "true"},
            {"FirstCureMpHigh", "true"},
            {"BuffUseInFight", "true"},
            {"BuffStopRestMpLow", "false"},
            {"BuffStopRestMpHigh", "false"},
            {"BuffMpLow", "false"},
            {"BuffMpHigh", "true"},
            {"SelfBuffUseInFight", "true"},
            {"SelfBuffStopRestMpLow", "false"},
            {"SelfBuffStopRestMpHigh", "true"},
            {"SelfBuffMpLow", "true"},
            {"SelfBuffMpHigh", "true"},
            {"DebuffUseInFight", "true"},
            {"DebuffStopRestMpLow", "true"},
            {"DebuffStopRestMpHigh", "true"},
            {"DebuffMpLow", "true"},
            {"DebuffMpHigh", "true"},
            {"WakeCureUseInFight", "true"},
            {"WakeCureStopRestMpLow", "true"},
            {"WakeCureStopRestMpHigh", "true"},
            {"WakeCureMpLow", "true"},
            {"WakeCureMpHigh", "true"},
            {"ConvertEnable", "true"},
            {"ConvertEnInFight", "false"},
            {"ConvertMpThreshold", "100"},
            {"ConvertCureRestFullFirst", "true"},
            {"ConvertCureRestFullMargin", "25"},
            {"ChatMpReport", "false"},
            {"ChatMpReportCmd", "/tell -Name MP is <mp> <mpp>"},
            {"ChatMpReportTimer", "300000"},
            {"ChatHpReport", "false"},
            {"ChatHpReportCmd", "/tell -Name Help! HP is <hp> <hpp>"},
            {"ChatHpReportLevel", "35"},
            {"ChatCastTime", "false"},
            {"ChatCastTimeCmd", "/tell -Name Hang on one second"},
            {"ChatCastTimeTime", "5500"},
            {"ChatLost", "false"},
            {"ChatLostCmd", "/tell -Name Where'd you go?"},
            {"Command1Enable", "false"},
            {"Command1Cmd", "/ja \"Composure\" <me>"},
            {"Command1Timer", "7200000"},
            {"Command1BeforeResting", "false"},
            {"Command2Enable", "false"},
            {"Command2Cmd", "/item \"Yagudo Drink\" <me>"},
            {"Command2Timer", "180000"},
            {"Command2BeforeResting", "false"},
            {"Command3Enable", "false"},
            {"Command3Cmd", "/ja \"Afflatus Solace\" <me>"},
            {"Command3Timer", "6000000"},
            {"Command3BeforeResting", "false"},
            {"Command4Enable", "false"},
            {"Command4Cmd", "/item \"Fries\" <me>"},
            {"Command4Timer", "180000"},
            {"Command4BeforeResting", "false"}
        };
        private static Dictionary<String, String> plDefaultTypes = new Dictionary<string, string>()
        {
            {"FirstCureDefault", "String"},
            {"SecondCureDefault", "String"},
            {"ThirdCureDefault", "String"},
            {"FirstCureDefaultPerc", "int"},
            {"SecondCureDefaultPerc", "int"},
            {"ThirdCureDefaultPerc", "int"},
            {"ProtectDefault", "String"},
            {"ShellDefault", "String"},
            {"HasteDefault", "String"},
            {"RefreshDefault", "String"},
            {"ProtectDefaultEnable", "bool"},
            {"ShellDefaultEnable", "bool"},
            {"HasteDefaultEnable", "bool"},
            {"RefreshDefaultEnable", "bool"},
            {"ProtectDefaultRecast", "int"},
            {"ShellDefaultRecast", "int"},
            {"HasteDefaultRecast", "int"},
            {"RefreshDefaultRecast", "int"},
            {"PlCharActivePollFrequency", "uint"},
            {"MaxCastDistance", "float"},
            {"HealthUpdateFrequency", "uint"},
            {"DequeuePollFrequency", "uint"},
            {"CastTimeModifier", "uint"},
            {"CastTimeModifierCures", "uint"},
            {"CastTimeMargin", "uint"},
            {"JaProcTime", "uint"},
            {"HealMpPercLow", "uint"},
            {"HealMpPercHigh", "uint"},
            {"RestInFight", "bool"},
            {"RestInFightAlways", "bool"},
            {"RestInFightLessUpper", "bool"},
            {"RestInFightLessLower", "bool"},
            {"UpperMpMargin", "uint"},
            {"NeverRest", "bool"},
            {"RestingStyle", "int"},
            {"FollowDistance", "int"},
            {"UseElasticFollowing", "bool"},
            {"ElasticDistance", "int"},
            {"ZoneTimer", "uint"},
            {"WakeCure", "String"},
            {"BuffQ", "int"},
            {"SelfBuffQ", "int"},
            {"DebuffQ", "int"},
            {"WakeCureQ", "int"},
            {"ThirdCureUseInFight", "bool"},
            {"ThirdCureStopRestMpLow", "bool"},
            {"ThirdCureStopRestMpHigh", "bool"},
            {"ThirdCureMpLow", "bool"},
            {"ThirdCureMpHigh", "bool"},
            {"SecondCureUseInFight", "bool"},
            {"SecondCureStopRestMpLow", "bool"},
            {"SecondCureStopRestMpHigh", "bool"},
            {"SecondCureMpLow", "bool"},
            {"SecondCureMpHigh", "bool"},
            {"FirstCureUseInFight", "bool"},
            {"FirstCureStopRestMpLow", "bool"},
            {"FirstCureStopRestMpHigh", "bool"},
            {"FirstCureMpLow", "bool"},
            {"FirstCureMpHigh", "bool"},
            {"BuffUseInFight", "bool"},
            {"BuffStopRestMpLow", "bool"},
            {"BuffStopRestMpHigh", "bool"},
            {"BuffMpLow", "bool"},
            {"BuffMpHigh", "bool"},
            {"SelfBuffUseInFight", "bool"},
            {"SelfBuffStopRestMpLow", "bool"},
            {"SelfBuffStopRestMpHigh", "bool"},
            {"SelfBuffMpLow", "bool"},
            {"SelfBuffMpHigh", "bool"},
            {"DebuffUseInFight", "bool"},
            {"DebuffStopRestMpLow", "bool"},
            {"DebuffStopRestMpHigh", "bool"},
            {"DebuffMpLow", "bool"},
            {"DebuffMpHigh", "bool"},
            {"WakeCureUseInFight", "bool"},
            {"WakeCureStopRestMpLow", "bool"},
            {"WakeCureStopRestMpHigh", "bool"},
            {"WakeCureMpLow", "bool"},
            {"WakeCureMpHigh", "bool"},
            {"ConvertEnable", "bool"},
            {"ConvertEnInFight", "bool"},
            {"ConvertMpThreshold", "uint"},
            {"ConvertCureRestFullFirst", "bool"},
            {"ConvertCureRestFullMargin", "uint"},
            {"ChatMpReport", "bool"},
            {"ChatMpReportCmd", "String"},
            {"ChatMpReportTimer", "uint"},
            {"ChatHpReport", "bool"},
            {"ChatHpReportCmd", "String"},
            {"ChatHpReportLevel", "uint"},
            {"ChatCastTime", "bool"},
            {"ChatCastTimeCmd", "String"},
            {"ChatCastTimeTime", "double"},
            {"ChatLost", "bool"},
            {"ChatLostCmd", "String"},
            {"Command1Enable", "bool"},
            {"Command1Cmd", "String"},
            {"Command1Timer", "uint"},
            {"Command1BeforeResting", "bool"},
            {"Command2Enable", "bool"},
            {"Command2Cmd", "String"},
            {"Command2Timer", "uint"},
            {"Command2BeforeResting", "bool"},
            {"Command3Enable", "bool"},
            {"Command3Cmd", "String"},
            {"Command3Timer", "uint"},
            {"Command3BeforeResting", "bool"},
            {"Command4Enable", "bool"},
            {"Command4Cmd", "String"},
            {"Command4Timer", "uint"},
            {"Command4BeforeResting", "bool"}
        };
        #endregion PL
        #region SU
        private static Dictionary<String, String> suDefaultValues = new Dictionary<string, string>()
        {
            {"SUCurrentSkill", "0"},
            {"SUSkillLevel", "0"},
            {"SUStopAtGivenSkill", "false"},
            {"SUGoToCap", "true"},
            {"SUDoOnly", "false"},
            {"SUDoOnlyCount", "10"},
            {"SUDoneStop", "true"},
            {"SUDoneLogout", "false"},
            {"SUDoneShutdown", "false"},
            {"SUGiveRestCommand", "false"},
            {"SURestCommand", "/item \"Ginger Cookie\" <me>"},
            {"SUGiveLogoutCommand", "false"},
            {"SULogoutCommand", "/ma Warp <me>"},
            {"SUDelayBetweenCasts", "false"},
            {"SUDelayValueBetweenCasts", "0"}
        };
        private static Dictionary<String, String> suDefaultTypes = new Dictionary<string, string>()
        {
            {"SUCurrentSkill", "byte"},
            {"SUSkillLevel", "int"},
            {"SUStopAtGivenSkill", "bool"},
            {"SUGoToCap", "bool"},
            {"SUDoOnly", "bool"},
            {"SUDoOnlyCount", "int"},
            {"SUDoneStop", "bool"},
            {"SUDoneLogout", "bool"},
            {"SUDoneShutdown", "bool"},
            {"SUGiveRestCommand", "bool"},
            {"SURestCommand", "String"},
            {"SUGiveLogoutCommand", "bool"},
            {"SULogoutCommand", "String"},
            {"SUDelayBetweenCasts", "bool"},
            {"SUDelayValueBetweenCasts", "uint"}
        };
        #endregion SU
        #region Crafter
        private static Dictionary<String, String> crafterDefaultValues = new Dictionary<string, string>()
        {
            {"CrafterPlaySound", "Crafting complete."}
        };
        private static Dictionary<String, String> crafterDefaultTypes = new Dictionary<string, string>()
        {
            {"CrafterPlaySound", "String"}
        };
        #endregion Crafter
        #region TA
        private static Dictionary<String, String> taDefaultValues = new Dictionary<string, string>()
        {
        };
        private static Dictionary<String, String> taDefaultTypes = new Dictionary<string, string>()
        {
        };
        #endregion TA
        #region Trader
        private static Dictionary<String, String> traderDefaultValues = new Dictionary<string, string>()
        {
        };
        private static Dictionary<String, String> traderDefaultTypes = new Dictionary<string, string>()
        {
        };
        #endregion Trader
        #region WMS
        private static Dictionary<String, String> wmsDefaultValues = new Dictionary<string, string>()
        {
            {"WMSBackgroundScanPeriod", "300"},
            {"WMSBackgroundUpdate", "true"}
        };
        private static Dictionary<String, String> wmsDefaultTypes = new Dictionary<string, string>()
        {
            {"WMSBackgroundScanPeriod", "uint"},
            {"WMSBackgroundUpdate", "bool"}
        };
        #endregion WMS
        #region FishStats
        private static Dictionary<String, String> fishStatsDefaultValues = new Dictionary<string, string>()
        {
            {"FishFishComboBox", "0"},
            {"ZoneZoneComboBox", "0"},
            {"ZoneRodComboBox", "0"},
            {"ZoneBaitComboBox", "0"},
            {"RodRodComboBox", "0"},
            {"BaitBaitComboBox", "0"},
            {"UseHexFileNames", "false"}
        };
        private static Dictionary<String, String> fishStatsDefaultTypes = new Dictionary<string, string>()
        {
            {"FishFishComboBox", "int"},
            {"ZoneZoneComboBox", "int"},
            {"ZoneRodComboBox", "int"},
            {"ZoneBaitComboBox", "int"},
            {"RodRodComboBox", "int"},
            {"BaitBaitComboBox", "int"},
            {"UseHexFileNames", "bool"}
        };
        #endregion FishStats
        #region NAV
        private static Dictionary<String, String> navDefaultValues = new Dictionary<string, string>()
        {
            {"Nav_Rec_IntervalDefValue", "2000"},
            {"Nav_Rec_MinDistDefValue", "2"},
            {"Nav_Rec_WaitDefValue", "3"},
            {"Nav_TripCompleteSound", "You have arrived at your destination."},
            {"Nav_Prc_PromptOnRightClick", "true"},
            {"Nav_sortingTagsFirst", "true"},
            {"Nav_sortingUseLastZoneOnly", "true"}
        };
        private static Dictionary<String, String> navDefaultTypes = new Dictionary<string, string>()
        {
            {"Nav_Rec_IntervalDefValue", "double"},
            {"Nav_Rec_MinDistDefValue", "double"},
            {"Nav_Rec_WaitDefValue", "double"},
            {"Nav_TripCompleteSound", "String"},
            {"Nav_Prc_PromptOnRightClick", "bool"},
            {"Nav_sortingTagsFirst", "bool"},
            {"Nav_sortingUseLastZoneOnly", "bool"}
        };
        #endregion NAV
        #region Top
        private static Dictionary<String, String> topDefaultValues = new Dictionary<string, string>()
        {
            {"TOP_KillOnGmTell", "false"},
            {"TOP_StopAllOnGmTell", "false"},
            {"TOP_StopFisherOnGmTell", "true"},
            {"MoveUpDownDelay", "700"},
            {"MoveItemDelay", "1200"},
            {"KeyHoldTime", "150"},
            {"DebugScope", "0"},
            {"RetainSettings", "false"},
            {"FisherLeftArrowKey", "NumPad4"},
            {"FisherRightArrowKey", "NumPad6"},
            {"FlashFishStatsButton", "true"},
            {"EnableResizeEffects", "true"},
            {"ShowNpcs", "true"},
            {"ShowNpcNames", "false"},
            {"ShowMobs", "true"},
            {"ShowMobNames", "true"},
            {"ShowPets", "false"},
            {"ShowPetNames", "false"},
            {"ShowPcs", "true"},
            {"ShowPcNames", "false"},
            {"ShowRangeCircle", "true"},
            {"MapsPath", @".\Maps\Images\"},
            {"ALR_Enable", "false"},
            {"ALR_AlwaysAlert", "false"},
            {"ALR_PlayMessage", "false"},
            {"ALR_MessageText", ""},
            {"ALR_FlashTaskBar", "false"},
            {"ALR_PauseBots", "false"},
            {"AutoReset", "true" },
            {"ThisZoneOnly", "false" }
        };
        private static Dictionary<String, String> topDefaultTypes = new Dictionary<string, string>()
        {
            {"TOP_KillOnGmTell", "bool"},
            {"TOP_StopAllOnGmTell", "bool"},
            {"TOP_StopFisherOnGmTell", "bool"},
            {"MoveUpDownDelay", "UInt32"},
            {"MoveItemDelay", "UInt32"},
            {"KeyHoldTime", "UInt32"},
            {"DebugScope", "UInt32"},
            {"RetainSettings", "bool"},
            {"FisherLeftArrowKey", "String"},
            {"FisherRightArrowKey", "String"},
            {"FlashFishStatsButton", "bool"},
            {"EnableResizeEffects", "bool"},
            {"ShowNpcs", "bool"},
            {"ShowNpcNames", "bool"},
            {"ShowMobs", "bool"},
            {"ShowMobNames", "bool"},
            {"ShowPets", "bool"},
            {"ShowPetNames", "bool"},
            {"ShowPcs", "bool"},
            {"ShowPcNames", "bool"},
            {"ShowRangeCircle", "bool"},
            {"MapsPath", "String"},
            {"ALR_Enable", "bool"},
            {"ALR_AlwaysAlert", "bool"},
            {"ALR_PlayMessage", "bool"},
            {"ALR_MessageText", "String"},
            {"ALR_FlashTaskBar", "bool"},
            {"ALR_PauseBots", "bool"},
            {"AutoReset", "bool" },
            {"ThisZoneOnly", "bool" }
        };
        #endregion Top
        #region Helpers
        private static Dictionary<String, String> helpersDefaultValues = new Dictionary<string, string>()
        {
            {"TR_StopOnItem", "false"},
            {"SL_EnterNpcMenuDelay", "2500"},
            {"SL_EnterToSellDelay", "300"},
            {"SL_CheckHelpTextDelay", "700"},
            {"SL_ChangeSellQuanDelay", "100"},
            {"SL_PressEnterToSellDelay", "300"},
            {"SL_WhenDoneSound", "Selling complete."},
            {"SL_SellFromBagFirst", "false"},
            {"SL_SortBeforeSelling", "false"},
            {"BY_WhenDoneSound", "Buying complete."},
            {"BY_GuildWaitTime", "800"},
            {"BY_GuildSetIndex", "false"}
        };
        private static Dictionary<String, String> helpersDefaultTypes = new Dictionary<string, string>()
        {
            {"TR_StopOnItem", "bool"},
            {"SL_EnterNpcMenuDelay", "UInt32"},
            {"SL_EnterToSellDelay", "UInt32"},
            {"SL_CheckHelpTextDelay", "UInt32"},
            {"SL_ChangeSellQuanDelay", "UInt32"},
            {"SL_PressEnterToSellDelay", "UInt32"},
            {"SL_WhenDoneSound", "String"},
            {"SL_SellFromBagFirst", "bool"},
            {"SL_SortBeforeSelling", "bool"},
            {"BY_WhenDoneSound", "String"},
            {"BY_GuildWaitTime", "UInt32"},
            {"BY_GuildSetIndex", "bool"}
        };
        #endregion Helpers
        #region POS
        private static Dictionary<String, String> posDefaultValues = new Dictionary<string, string>()
        {
            {"POS_DistancePerNudge", "6.0"},
            {"POS_Speed", "6.25"},
            {"POS_SpeedDisable", "5.0"},
            {"POS_DisableOnAlert", "true"},
            {"POS_EnableOnStartup", "false"}
        };
        private static Dictionary<String, String> posDefaultTypes = new Dictionary<string, string>()
        {
            {"POS_DistancePerNudge", "float"},
            {"POS_Speed", "float"},
            {"POS_SpeedDisable", "float"},
            {"POS_DisableOnAlert", "bool"},
            {"POS_EnableOnStartup", "bool"}
        };
        #endregion POS
        private static Dictionary<String, String>[] botDefaultValues = { fisherDefaultValues,
                                                                         plDefaultValues,
                                                                         suDefaultValues,
                                                                         crafterDefaultValues,
                                                                         taDefaultValues,
                                                                         traderDefaultValues,
                                                                         wmsDefaultValues,
                                                                         fishStatsDefaultValues,
                                                                         navDefaultValues,
                                                                         topDefaultValues,
                                                                         helpersDefaultValues,
                                                                         posDefaultValues };
        private static Dictionary<String, String>[] botDefaultTypes = { fisherDefaultTypes,
                                                                        plDefaultTypes,
                                                                        suDefaultTypes,
                                                                        crafterDefaultTypes,
                                                                        taDefaultTypes,
                                                                        traderDefaultTypes,
                                                                        wmsDefaultTypes,
                                                                        fishStatsDefaultTypes,
                                                                        navDefaultTypes,
                                                                        topDefaultTypes,
                                                                        helpersDefaultTypes,
                                                                        posDefaultTypes };
        #endregion Default Values/Types Init
        #endregion Private Member Data
        #region Public Member Data
        #endregion Public Member Data
        #region Private Methods
        private static void init()
        {
            if (!initDone)
            {
                usDataSets = new USDataSet[botNames.Length];
                for(int ii=0; ii<botNames.Length; ii++) {
                    usDataSets[ii] = new USDataSet(botNames[ii], 
                                                   fileNamesPrefix[ii],
                                                   fileNamesSuffix[ii],
                                                   filePath,
                                                   botDefaultTypes[ii],
                                                   botDefaultValues[ii],
                                                   listTables[ii]);
                }

                initDone = true;
            }
        }
        private static String getTableName(LIST_TABLE iTable)
        {
            return tableNames[(byte)iTable];
        }
        #endregion Private Methods
        #region Public Methods
        public static String GetCharacter()
        {
            return charName;
        }
        public static void ChangeCharacter(String iCharName)
        {
            charName = iCharName;
            foreach (USDataSet ds in usDataSets)
            {
                ds.CharacterName = charName;
            }
            LoadSettings();
        }
        public static void LoadDefaultSettings()
        {
            init();
            foreach (USDataSet ds in usDataSets)
            {
                ((USValueDataTable)ds.Tables[0]).LoadDefaultValues();
            }
        }
        public static void LoadSettings()
        {
            init();
            foreach (USDataSet ds in usDataSets)
            {
                ds.LoadSettings();
            }
        }
        public static void LoadCommonSettings()
        {
            init();
            foreach (USDataSet ds in usDataSets)
            {
                if (ds.BotName == commonSettingsBotName)
                {
                    ds.LoadSettings();
                    break;
                }
            }
        }
        public static void SaveSettings()
        {
            init();
            LoggingFunctions.Debug("Saving settings for " + charName, LoggingFunctions.DBG_SCOPE.SETTINGS);
            foreach (USDataSet ds in usDataSets)
            {
                ds.SaveSettings();
            }
        }
        public static void SaveCommonSettings()
        {
            init();
            foreach (USDataSet ds in usDataSets)
            {
                if (ds.BotName == commonSettingsBotName)
                {
                    ds.SaveSettings();
                    break;
                }
            }
        }
        public static Object GetValue(BOT iBot, String iName)
        {
            init();
            return ((USDataSet)usDataSets[(byte)iBot]).GetValue(iName);
        }
        public static Object GetDefaultValue(BOT iBot, String iName)
        {
            init();
            return ((USDataSet)usDataSets[(byte)iBot]).GetDefaultValue(iName);
        }
        public static void SetValue(BOT iBot, String iName, String iValue)
        {
            init();
            ((USDataSet)usDataSets[(byte)iBot]).SetValue(iName, iValue);
        }
        public static List<Object> GetListValue(BOT iBot, LIST_TABLE iTable, Object iValue)
        {
            return usDataSets[(byte)iBot].GetListValue(getTableName(iTable), iValue);
        }
        public static void AddListValue(BOT iBot, LIST_TABLE iTable, List<Object> iValues)
        {
            usDataSets[(byte)iBot].AddListValue(getTableName(iTable), iValues);
        }
        public static void RemoveListValue(BOT iBot, LIST_TABLE iTable, Object iValue)
        {
            usDataSets[(byte)iBot].RemoveListValue(getTableName(iTable), iValue);
        }
        public static void SetListValue(BOT iBot, LIST_TABLE iTable, List<Object> iValues)
        {
            usDataSets[(byte)iBot].SetListValue(getTableName(iTable), iValues);
        }
        public static List<List<Object>> GetList(BOT iBot, LIST_TABLE iTable)
        {
            return usDataSets[(byte)iBot].GetValuesList(getTableName(iTable));
        }
        public static void SetList(BOT iBot, LIST_TABLE iTable, List<List<Object>> iValues)
        {
            usDataSets[(byte)iBot].SetValuesList(getTableName(iTable), iValues);
        }
        #endregion Public Methods

        #region Classes
        private class USDataSet : System.Data.DataSet
        {
            #region Constructor
            public USDataSet(String iBotName, 
                               String iFileNamePrefix,
                               String iFileNameSuffix,
                               String iFilePath, 
                               Dictionary<String, String> iDefaultVariableTypes,
                               Dictionary<String, String> iDefaultVariableValues,
                               List<List<String>> iListTables)
            {
                this.DataSetName = iBotName + "Table";
                botName = iBotName;
                fileNamePrefix = iFileNamePrefix;
                fileNameSuffix = iFileNameSuffix;
                filePath = iFilePath;
                this.Tables.Add(new USValueDataTable(iBotName, iDefaultVariableTypes, iDefaultVariableValues));
                if (iListTables != null)
                {
                    for (int ii = 0; ii < iListTables.Count; ii++)
                    {
                        //Add new list table.
                        if (iListTables[ii] != null)
                        {
                            this.Tables.Add(new USListDataTable(iBotName, iListTables[ii]));
                        }
                    }
                }
            }
            #endregion Constructor
            #region Private Member Data
            private String botName;
            private String characterName;
            private String fileNamePrefix;
            private String fileNameSuffix;
            private String filePath;
            #endregion Private Member Data
            #region Public Member Data
            public String BotName
            {
                get
                {
                    return botName;
                }
            }
            public String CharacterName
            {
                get
                {
                    return characterName;
                }
                set
                {
                    characterName = value;
                }
            }
            public String FileNamePrefix
            {
                get
                {
                    return fileNamePrefix;
                }
            }
            public String FileNameSuffix
            {
                get
                {
                    return fileNameSuffix;
                }
            }
            public String FilePath
            {
                get
                {
                    return filePath;
                }
            }
            #endregion Public Member Data
            #region Private Methods
            private bool checkFilePathOk()
            {
                if (this.Tables.Count == 0)
                {
                    LoggingFunctions.Timestamp("Could not save " + botName + " settings to file, 0 tables.");
                    return false;
                }
                if (fileNamePrefix == "" && fileNameSuffix == "")
                {
                    LoggingFunctions.Timestamp("Could not save " + botName + " settings to file, file name empty.");
                    return false;
                }
                if (filePath == "")
                {
                    LoggingFunctions.Timestamp("Could not save " + botName + " settings to file, file path empty.");
                    return false;
                }
                if (characterName == "")
                {
                    LoggingFunctions.Timestamp("Could not save " + botName + " settings to file, character name empty.");
                    return false;
                }
                if (!System.IO.Directory.Exists(filePath))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(filePath);
                        return true;
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("Could not create directory '" + filePath + "' : " + e.ToString());
                        return false;
                    }
                }
                return true;
            }
            private void loadDefaultValues()
            {
                ((USValueDataTable)this.Tables[0]).LoadDefaultValues();
            }
            #endregion Private Methods
            #region Public Methods
            public bool SaveSettings()
            {
                if (!checkFilePathOk())
                {
                    return false;
                }
                try
                {
                    if (this.Tables.Count == 0)
                    {
                        return true;
                    }
                    bool modified = false;
                    modified = ((USValueDataTable)this.Tables[0]).State != DataRowState.Unchanged;
                    for (int ii = 1; ii < this.Tables.Count; ii++)
                    {
                        if (((USListDataTable)this.Tables[ii]).State != DataRowState.Unchanged)
                        {
                            modified = true;
                        }
                    }
                    if (modified)
                    {
                        String fileName = filePath;
                        if(botName == UserSettings.commonSettingsBotName)
                        {
                            fileName += UserSettings.commonSettingsFileName;
                            //Clean up old file with character name.
                            if(System.IO.File.Exists(filePath + fileNamePrefix + characterName + fileNameSuffix + ".xml"))
                            {
                                System.IO.File.Delete(filePath + fileNamePrefix + characterName + fileNameSuffix + ".xml");
                            }
                        }
                        else
                        {
                            fileName += fileNamePrefix + characterName + fileNameSuffix;
                        }
                        fileName += ".xml";
                        this.WriteXml(fileName);
                    }
                    return true;
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("Exception while writing " + botName + " settings file " + filePath + fileNamePrefix + characterName
                                             + fileNameSuffix + ".xml: " + e.ToString());
                    return false;
                }
            }
            public bool LoadSettings()
            {
                foreach (DataTable table in this.Tables)
                {
                    table.Rows.Clear();
                }
                if (!checkFilePathOk())
                {
                    return false;
                }
                String filename = "";
                if (botName == UserSettings.commonSettingsBotName)
                {
                    if(System.IO.File.Exists(filePath + UserSettings.commonSettingsFileName + ".xml"))
                    {
                        filename = filePath + UserSettings.commonSettingsFileName + ".xml";
                    }
                    else if(System.IO.File.Exists(filePath + fileNamePrefix + characterName + fileNameSuffix + ".xml"))
                    {
                        filename = filePath + fileNamePrefix + characterName + fileNameSuffix + ".xml";
                    }
                }
                else if (System.IO.File.Exists(filePath + fileNamePrefix + characterName + fileNameSuffix + ".xml"))
                {
                    filename = filePath + fileNamePrefix + characterName + fileNameSuffix + ".xml";
                }
                if (filename == "")
                {
                    try
                    {
                        loadDefaultValues();
                        return true;
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("Exception while loading default values for " + botName + ": " + e.ToString());
                        return false;
                    }
                }
                try
                {
                    this.ReadXml(filename);
                    //Now check that each setting value from the default list is in the table.
                    //This is needed in case we add a new value that's not in the xml.
                    ((USValueDataTable)this.Tables[0]).MergeDefaultValues();
                    return true;
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("Exception while reading " + botName + " settings file " + filename + ": " + e.ToString());
                    return false;
                }
            }
            public Object GetValue(String iName)
            {
                return ((USValueDataTable)this.Tables[0]).GetValue(iName);
            }
            public Object GetDefaultValue(String iName)
            {
                return ((USValueDataTable)this.Tables[0]).GetDefaultValue(iName);
            }
            public void SetValue(String iName, Object iValue)
            {
                ((USValueDataTable)this.Tables[0]).SetValue(iName, iValue.ToString());
            }
            public List<Object> GetListValue(String iTable, Object iValue)
            {
                if (this.Tables.Contains(iTable))
                {
                    return ((USListDataTable)this.Tables[iTable]).GetValue(iValue);
                }
                else
                {
                    MessageBox.Show("Could not find table " + iTable + " in dataset " + this.DataSetName);
                    return null;
                }
            }
            public void AddListValue(String iTable, List<Object> iValues)
            {
                if (this.Tables.Contains(iTable))
                {
                    ((USListDataTable)this.Tables[iTable]).AddValue(iValues);
                }
                else
                {
                    MessageBox.Show("Could not find table " + iTable + " in dataset " + this.DataSetName);
                }
            }
            public void RemoveListValue(String iTable, Object iValue)
            {
                if (this.Tables.Contains(iTable))
                {
                    ((USListDataTable)this.Tables[iTable]).RemoveValue(iValue);
                }
                else
                {
                    MessageBox.Show("Could not find table " + iTable + " in dataset " + this.DataSetName);
                }
            }
            public void SetListValue(String iTable, List<Object> iValues)
            {
                if (this.Tables.Contains(iTable))
                {
                    ((USListDataTable)this.Tables[iTable]).SetValue(iValues);
                }
                else
                {
                    MessageBox.Show("Could not find table " + iTable + " in dataset " + this.DataSetName);
                }
            }
            public List<List<Object>> GetValuesList(String iTable)
            {
                if (this.Tables.Contains(iTable))
                {
                    return ((USListDataTable)this.Tables[iTable]).GetValuesList();
                }
                else
                {
                    MessageBox.Show("Could not find talbe " + iTable + " in dataset " + this.DataSetName);
                    return null;
                }
            }
            public void SetValuesList(String iTable, List<List<Object>> iValues)
            {
                if (this.Tables.Contains(iTable))
                {
                    ((USListDataTable)this.Tables[iTable]).SetValuesList(iValues);
                }
                else
                {
                    MessageBox.Show("Could not find talbe " + iTable + " in dataset " + this.DataSetName);
                }
            }
            #endregion Public Methods
        }
        private class USValueDataTable : System.Data.DataTable
        {
            #region Constructor
            public USValueDataTable(String iBotName,
                                      Dictionary<String, String> iDefaultTypes,
                                      Dictionary<String, String> iDefaultValues)
            {
                //Set private members
                botName = iBotName;
                defaultTypes = iDefaultTypes;
                defaultValues = iDefaultValues;
                this.TableName = "Variables";

                //Value data table has columns:
                //1. Name (String)
                //2. Type (String which represents System.Type)
                //3. Value (String of data type given in 2.
                this.Columns.Add("Name", System.Type.GetType("System.String"));
                this.Columns.Add("Type", System.Type.GetType("System.String"));
                this.Columns.Add("Value", System.Type.GetType("System.String"));
                
                loadDefaultValues();
                this.state = DataRowState.Unchanged;
            }
            #endregion Constructor
            #region Private Member Data
            private String botName;
            private Dictionary<String, String> defaultTypes;
            private Dictionary<String, String> defaultValues;
            private DataRowState state;
            #endregion Private Member Data
            #region Public Member Data
            public String BotName
            {
                get
                {
                    return botName;
                }
            }
            public DataRowState State
            {
                get
                {
                    return state;
                }
                set
                {
                    state = value;
                }
            }
            #endregion Public Member Data
            #region Private Methods
            private void loadDefaultValues()
            {
                this.BeginLoadData();
                foreach (KeyValuePair<String, String> kvp in defaultValues)
                {
                    String type = defaultTypes[kvp.Key];
                    setValue(kvp.Key, type, kvp.Value);
                }
                this.EndLoadData();
            }
            private void addValue(String iName, String iType, String iValue)
            {
                DataRow row = this.NewRow();
                row["Name"] = iName;
                row["Type"] = iType;
                row["Value"] = iValue;
                this.Rows.Add(row);
                this.state = DataRowState.Added;
            }
            private void setValue(String iName, String iType, String iValue)
            {
                DataRow oRow = null;
                if (!checkValueInTable(iName, ref oRow))
                {
                    addValue(iName, iType, iValue);
                }
                else
                {
                    oRow["Value"] = iValue;
                }
                this.state = DataRowState.Modified;
            }
            private bool checkValueInTable(String iName, ref DataRow oRow)
            {
                DataRow[] rows = this.Select("Name='" + iName + "'");
                if (rows.Length > 0)
                {
                    oRow = rows[0];
                    return true;
                }
                else
                {
                    oRow = null;
                    return false;
                }
            }
            #endregion Private Methods
            #region Public Methods
            public void SetValue(String iName, String iValue)
            {
                DataRow oRow = null;
                if (!checkValueInTable(iName, ref oRow))
                {
                    LoggingFunctions.Error("Could not set " + botName + " value '" + iName + "', value not found.");
                }
                else
                {
                    if (oRow["Value"].ToString() != iValue)
                    {
                        oRow["Value"] = iValue;
                        this.state = DataRowState.Modified;
                    }
                }
            }
            public Object GetValue(String iName)
            {
                DataRow oRow = null;
                if (!checkValueInTable(iName, ref oRow))
                {
                    LoggingFunctions.Error("Getting settings value '" + iName + "' from " + botName + ", value not found.");
                    return null;
                }
                else
                {
                    bool isNull = false;
                    if (oRow["Value"].GetType() == System.Type.GetType("System.DBNull"))
                    {
                        isNull = true;
                    }
                    String valType = oRow["Type"].ToString();
                    if ((valType == "string") || (valType == "String"))
                    {
                        if (!isNull)
                        {
                            return oRow["Value"].ToString();
                        }
                        else
                        {
                            return defaultValues[iName];
                        }
                    }
                    else if ((valType == "bool") || (valType == "Boolean"))
                    {
                        if (!isNull)
                        {
                            return Convert.ToBoolean(oRow["Value"]);
                        }
                        else
                        {
                            return Convert.ToBoolean(defaultValues[iName]);
                        }
                    }
                    else if ((valType == "Byte") || (valType == "byte"))
                    {
                        if (!isNull)
                        {
                            return Convert.ToByte(oRow["Value"]);
                        }
                        else
                        {
                            return Convert.ToByte(defaultValues[iName]);
                        }
                    }
                    else if ((valType == "short") || (valType == "Int16"))
                    {
                        if (!isNull)
                        {
                            return Convert.ToInt16(oRow["Value"]);
                        }
                        else
                        {
                            return Convert.ToInt16(defaultValues[iName]);
                        }
                    }
                    else if ((valType == "int") || (valType == "Int32"))
                    {
                        if (!isNull)
                        {
                            return Convert.ToInt32(oRow["Value"]);
                        }
                        else
                        {
                            return Convert.ToInt32(defaultValues[iName]);
                        }
                    }
                    else if ((valType == "long") || (valType == "Int64"))
                    {
                        if (!isNull)
                        {
                            return Convert.ToInt64(oRow["Value"]);
                        }
                        else
                        {
                            return Convert.ToInt64(defaultValues[iName]);
                        }
                    }
                    else if ((valType == "ushort") || (valType == "UInt16"))
                    {
                        if (!isNull)
                        {
                            return Convert.ToUInt16(oRow["Value"]);
                        }
                        else
                        {
                            return Convert.ToUInt16(defaultValues[iName]);
                        }
                    }
                    else if ((valType == "uint") || (valType == "UInt32"))
                    {
                        if (!isNull)
                        {
                            return Convert.ToUInt32(oRow["Value"]);
                        }
                        else
                        {
                            return Convert.ToUInt32(defaultValues[iName]);
                        }
                    }
                    else if ((valType == "ulong") || (valType == "UInt64"))
                    {
                        if (!isNull)
                        {
                            return Convert.ToUInt64(oRow["Value"]);
                        }
                        else
                        {
                            return Convert.ToUInt64(defaultValues[iName]);
                        }
                    }
                    else if ((valType == "float") || (valType == "Single"))
                    {
                        if (!isNull)
                        {
                            //return Convert.ToSingle(oRow["Value"]);
                            Single value;
                            if (Single.TryParse(oRow["Value"].ToString(), out value))
                            {
                                return value;
                            }
                            else if (Single.TryParse(oRow["Value"].ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out value))
                            {
                                return value;
                            }
                            else
                            {
                                LoggingFunctions.Error("Could not convert '" + oRow["Value"].ToString() + "' to a float value.");
                                return Convert.ToSingle(defaultValues[iName], new System.Globalization.CultureInfo("en-US"));
                            }
                        }
                        else
                        {
                            return Convert.ToSingle(defaultValues[iName]);
                        }
                    }
                    else if ((valType == "double") || (valType == "Double"))
                    {
                        if (!isNull)
                        {
                            double value;
                            if (double.TryParse(oRow["Value"].ToString(), out value))
                            {
                                return value;
                            }
                            else if (double.TryParse(oRow["Value"].ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out value))
                            {
                                return value;
                            }
                            else
                            {
                                LoggingFunctions.Error("Could not convert '" + oRow["Value"].ToString() + "' to a double value.");
                                return Convert.ToDouble(defaultValues[iName], new System.Globalization.CultureInfo("en-US"));
                            }
                        }
                        else
                        {
                            return Convert.ToDouble(defaultValues[iName]);
                        }
                    }
                    else if (valType == "DateTime")
                    {
                        if (!isNull)
                        {
                            DateTime value;
                            if (DateTime.TryParse(oRow["Value"].ToString(), out value))
                            {
                                return value;
                            }
                            else if (DateTime.TryParse(oRow["Value"].ToString(), System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out value))
                            {
                                return value;
                            }
                            else
                            {
                                LoggingFunctions.Error("Could not convert '" + oRow["Value"].ToString() + "' to a DateTime value.");
                                return Convert.ToDateTime(defaultValues[iName], new System.Globalization.CultureInfo("en-US"));
                            }
                        }
                        else
                        {
                            return Convert.ToDateTime(defaultValues[iName]);
                        }
                    }
                    else
                    {
                        LoggingFunctions.Error("Could not convert value '" + iName + "' to type '" + valType + ".");
                        return null;
                    }
                }
            }
            public Object GetDefaultValue(String iName)
            {
                if (this.defaultValues.ContainsKey(iName))
                {
                    String valType = this.defaultTypes[iName];
                    if ((valType == "string") || (valType == "String"))
                    {
                        return defaultValues[iName];
                    }
                    else if ((valType == "bool") || (valType == "Boolean"))
                    {
                        return Convert.ToBoolean(defaultValues[iName]);
                    }
                    else if ((valType == "Byte") || (valType == "byte"))
                    {
                        return Convert.ToByte(defaultValues[iName]);
                    }
                    else if ((valType == "short") || (valType == "Int16"))
                    {
                        return Convert.ToInt16(defaultValues[iName]);
                    }
                    else if ((valType == "int") || (valType == "Int32"))
                    {
                        return Convert.ToInt32(defaultValues[iName]);
                    }
                    else if ((valType == "long") || (valType == "Int64"))
                    {
                        return Convert.ToInt64(defaultValues[iName]);
                    }
                    else if ((valType == "ushort") || (valType == "UInt16"))
                    {
                        return Convert.ToUInt16(defaultValues[iName]);
                    }
                    else if ((valType == "uint") || (valType == "UInt32"))
                    {
                        return Convert.ToUInt32(defaultValues[iName]);
                    }
                    else if ((valType == "ulong") || (valType == "UInt64"))
                    {
                        return Convert.ToUInt64(defaultValues[iName]);
                    }
                    else if ((valType == "float") || (valType == "Single"))
                    {
                        return Convert.ToSingle(defaultValues[iName]);
                    }
                    else if ((valType == "double") || (valType == "Double"))
                    {
                        return Convert.ToDouble(defaultValues[iName]);
                    }
                    else if (valType == "DateTime")
                    {
                        return Convert.ToDateTime(defaultValues[iName]);
                    }
                    else
                    {
                        LoggingFunctions.Error("Could not convert value '" + iName + "' to type '" + valType + ".");
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            public void LoadDefaultValues()
            {
                this.Rows.Clear();
                loadDefaultValues();
                this.state = DataRowState.Modified;
            }
            public void MergeDefaultValues()
            {
                foreach (String val in defaultValues.Keys)
                {
                    DataRow oRow = null;
                    if (!checkValueInTable(val, ref oRow))
                    {
                        //Value not in the table, add the default value.
                        addValue(val, defaultTypes[val], defaultValues[val]);
                    }
                }
            }
            #endregion Public Methods
        }
        private class USListDataTable : System.Data.DataTable
        {
            #region Constructor
            public USListDataTable(String iBotName, List<String> iColumns)
            {
                botName = iBotName;
                if ((iColumns != null) && (iColumns.Count != 0))
                {
                    this.TableName = iColumns[0];
                    if ((iColumns.Count % 2) != 1)
                    {
                        MessageBox.Show("List Table for bot " + iBotName + " column count = " + iColumns.Count + ", Count%2=0");
                        return;
                    }
                    for (int ii = 1; ii < iColumns.Count; ii+=2)
                    {
                        this.Columns.Add(iColumns[ii], Type.GetType(iColumns[ii + 1]));
                    }
                }
                this.state = DataRowState.Unchanged;
            }
            #endregion Constructor
            #region Private Member Data
            private String botName;
            private DataRowState state;
            #endregion Private Member Data
            #region Public Member Data
            public String BotName
            {
                get
                {
                    return botName;
                }
            }
            public DataRowState State
            {
                get
                {
                    return state;
                }
                set
                {
                    state = value;
                }
            }
            #endregion Public Member Data
            #region Private Methods
            private bool checkValueCount(List<Object> iValues)
            {
                if (iValues.Count != this.Columns.Count)
                {
                    MessageBox.Show("Illegal number of values(" + iValues.Count + ") passed to table " + this.TableName + " with " + this.Columns.Count + " columns.");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            #endregion Private Methods
            #region Public Methods
            public List<Object> GetValue(Object iValue)
            {
                try
                {
                    //Column 0 value should always be unique. Check that the value exists.
                    String filter = "";
                    if (this.Columns[0].DataType == System.Type.GetType("System.String"))
                    {
                        filter = this.Columns[0].ColumnName + "='" + iValue.ToString() + "'";
                    }
                    else
                    {
                        filter = this.Columns[0].ColumnName + "=" + iValue.ToString();
                    }
                    DataRow[] rows = this.Select(filter);
                    if (rows.Length == 0)
                    {
                        return null;
                    }
                    List<Object> outValues = new List<object>();
                    for (int ii = 0; ii < this.Columns.Count; ii++)
                    {
                        outValues.Add(rows[0][ii]);
                    }
                    return outValues;
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("Getting value (" + iValue.ToString() + ") in table " + botName + ":" + this.TableName + ": " + e.ToString());
                    return null;
                }
            }
            public void AddValue(List<Object> iValues)
            {
                try
                {
                    if (!checkValueCount(iValues))
                    {
                        return;
                    }
                    //Column 0 value should always be unique. Check that the value doesn't already exist.
                    String filter = "";
                    if (this.Columns[0].DataType == System.Type.GetType("System.String"))
                    {
                        filter = this.Columns[0].ColumnName + "='" + iValues[0].ToString() + "'";
                    }
                    else
                    {
                        filter = this.Columns[0].ColumnName + "=" + iValues[0].ToString();
                    }
                    DataRow[] rows = this.Select(filter);
                    if (rows.Length > 0)
                    {
                        //This value already exists, call the set method to change the values.
                        SetValue(iValues);
                        return;
                    }
                    DataRow rowToAdd = this.NewRow();
                    for (int ii = 0; ii < iValues.Count; ii++)
                    {
                        rowToAdd[ii] = iValues[ii];
                    }
                    this.Rows.Add(rowToAdd);
                    this.state = DataRowState.Added;
                    return;
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("Adding value (" + iValues[0].ToString() + ") in table " + botName + ":" + this.TableName + ": " + e.ToString());
                }
            }
            public void RemoveValue(Object iValue)
            {
                try
                {
                    //Column 0 value should always be unique. Check that the value doesn't already exist.
                    String filter = "";
                    if (this.Columns[0].DataType == System.Type.GetType("System.String"))
                    {
                        filter = this.Columns[0].ColumnName + "='" + iValue.ToString() + "'";
                    }
                    else
                    {
                        filter = this.Columns[0].ColumnName + "=" + iValue.ToString();
                    }
                    DataRow[] rows = this.Select(filter);
                    if (rows.Length == 0)
                    {
                        //This value doesn't exist, nothing to do.
                        return;
                    }
                    rows[0].Delete();
                    this.state = DataRowState.Deleted;
                    return;
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("Removing value (" + iValue.ToString() + ") in table " + botName + ":" + this.TableName + ": " + e.ToString());
                }
            }
            public void SetValue(List<Object> iValues)
            {
                try
                {
                    if (!checkValueCount(iValues))
                    {
                        return;
                    }
                    //Column 0 value should always be unique. Check that the value exists.
                    String filter = "";
                    if (this.Columns[0].DataType == System.Type.GetType("System.String"))
                    {
                        filter = this.Columns[0].ColumnName + "='" + iValues[0].ToString() + "'";
                    }
                    else
                    {
                        filter = this.Columns[0].ColumnName + "=" + iValues[0].ToString();
                    }
                    DataRow[] rows = this.Select(filter);
                    if (rows.Length == 0)
                    {
                        //This value doesn't exist, call the add method to add it.
                        AddValue(iValues);
                        return;
                    }
                    bool modified = false;
                    for (int ii = 0; ii < iValues.Count; ii++)
                    {
                        if (rows[0][ii] != iValues[ii])
                        {
                            rows[0][ii] = iValues[ii];
                            modified = true;
                        }
                    }
                    if (modified)
                    {
                        this.state = DataRowState.Modified;
                    }
                    return;
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("Setting value (" + iValues[0].ToString() + ") in table " + botName + ":" + this.TableName + ": " + e.ToString());
                }
            }
            public List<List<Object>> GetValuesList()
            {
                if (this.Rows.Count == 0)
                {
                    return null;
                }
                List<List<Object>> values = new List<List<object>>();
                foreach (DataRow row in this.Rows)
                {
                    List<Object> rowList = new List<object>();
                    for (int ii = 0; ii < this.Columns.Count; ii++)
                    {
                        rowList.Add(row[ii]);
                    }
                    values.Add(rowList);
                }
                return values;
            }
            public void SetValuesList(List<List<Object>> iValues)
            {
                this.Rows.Clear();
                foreach (List<Object> iRow in iValues)
                {
                    if(iRow.Count != this.Columns.Count)
                    {
                        MessageBox.Show("[ERROR] SetValuesList, inputs list had " + iRow.Count + " items, table has " + this.Columns.Count + " + columns.");
                        return;
                    }
                    DataRow rowToAdd = this.NewRow();
                    for (int ii = 0; ii < iRow.Count; ii++)
                    {
                        rowToAdd[ii] = iRow[ii];
                    }
                    this.Rows.Add(rowToAdd);
                }
                this.state = DataRowState.Added;
            }
            #endregion Public Methods
        }
        #endregion Classes
    }
}
