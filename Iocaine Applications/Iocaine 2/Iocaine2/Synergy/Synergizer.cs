using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using System.ComponentModel;

using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Tools;

/// Todo:
/// Add distance check to furnace.

namespace Iocaine2.Synergy
{
    #region ElementalBalance
    public class ElementalBalance
    {
        #region Private Members
        private int _Fire;
        private int _Ice;
        private int _Wind;
        private int _Earth;
        private int _Lightning;
        private int _Water;
        private int _Light;
        private int _Dark;
        #endregion Private Members

        #region Public Properties
        public int Fire
        {
            get
            {
                return _Fire;
            }
            set
            {
                _Fire = value;
            }
        }
        public int Ice
        {
            get
            {
                return _Ice;
            }
            set
            {
                _Ice = value;
            }
        }
        public int Wind
        {
            get
            {
                return _Wind;
            }
            set
            {
                _Wind = value;
            }
        }
        public int Earth
        {
            get
            {
                return _Earth;
            }
            set
            {
                _Earth = value;
            }
        }
        public int Lightning
        {
            get
            {
                return _Lightning;
            }
            set
            {
                _Lightning = value;
            }
        }
        public int Water
        {
            get
            {
                return _Water;
            }
            set
            {
                _Water = value;
            }
        }
        public int Light
        {
            get
            {
                return _Light;
            }
            set
            {
                _Light = value;
            }
        }
        public int Dark
        {
            get
            {
                return _Dark;
            }
            set
            {
                _Dark = value;
            }
        }
        #endregion Public Properties

        #region Constructors
        public ElementalBalance() : this( 0, 0, 0, 0, 0, 0, 0, 0 )
        {
        }

        public ElementalBalance(int fire, int ice, int wind, int earth, int lightning, int water, int light, int dark)
        {
            _Fire = fire;
            _Ice = ice;
            _Wind = wind;
            _Earth = earth;
            _Lightning = lightning;
            _Water = water;
            _Light = light;
            _Dark = dark;
        }
        #endregion Constructors

        #region Public Methods
        public void Clear()
        {
            _Fire = 0;
            _Ice = 0;
            _Wind = 0;
            _Earth = 0;
            _Lightning = 0;
            _Water = 0;
            _Light = 0;
            _Dark = 0;
        }

        public void Set(FFXIEnums.ELEMENT element, int value)
        {
            switch (element)
            {
                case FFXIEnums.ELEMENT.FIRE:
                    _Fire = value;
                    break;
                case FFXIEnums.ELEMENT.ICE:
                    _Ice = value;
                    break;
                case FFXIEnums.ELEMENT.WIND:
                    _Wind = value;
                    break;
                case FFXIEnums.ELEMENT.EARTH:
                    _Earth = value;
                    break;
                case FFXIEnums.ELEMENT.LIGHTNING:
                    _Lightning = value;
                    break;
                case FFXIEnums.ELEMENT.WATER:
                    _Water = value;
                    break;
                case FFXIEnums.ELEMENT.LIGHT:
                    _Light = value;
                    break;
                case FFXIEnums.ELEMENT.DARK:
                    _Dark = value;
                    break;
            }
        }

        public int Get(FFXIEnums.ELEMENT element)
        {
            switch (element)
            {
                case FFXIEnums.ELEMENT.FIRE:
                    return _Fire;
                case FFXIEnums.ELEMENT.ICE:
                    return _Ice;
                case FFXIEnums.ELEMENT.WIND:
                    return _Wind;
                case FFXIEnums.ELEMENT.EARTH:
                    return _Earth;
                case FFXIEnums.ELEMENT.LIGHTNING:
                    return _Lightning;
                case FFXIEnums.ELEMENT.WATER:
                    return _Water;
                case FFXIEnums.ELEMENT.LIGHT:
                    return _Light;
                case FFXIEnums.ELEMENT.DARK:
                    return _Dark;
                default:
                    return 0;
            }
        }

        public static ElementalBalance operator-(ElementalBalance lhs, ElementalBalance rhs)
        {
            ElementalBalance retval = new ElementalBalance();
            retval.Fire = lhs.Fire - rhs.Fire;
            retval.Ice = lhs.Ice - rhs.Ice;
            retval.Wind = lhs.Wind - rhs.Wind;
            retval.Earth = lhs.Earth - rhs.Earth;
            retval.Lightning = lhs.Lightning - rhs.Lightning;
            retval.Water = lhs.Water - rhs.Water;
            retval.Light = lhs.Light - rhs.Light;
            retval.Dark = lhs.Dark - rhs.Dark;
            return retval;
        }
        #endregion Public Methods
    }
    #endregion ElementalBalance

    #region SyneDictionairy
    public sealed class SyneDictionairy
    {
        #region Enums
        public enum Codes 
        {
            NO_ACCESS_TO_SYNERGY,
            FURNACE_CLAIMED,
            FURNACE_ALREADY_CLAIMED,
            TEN_SECOND_WARNING,
            FURNACE_LOST,
            OUT_OF_RANGE,
            FEWELL_LEVELS,
            RECIPE_CONFIRMATION,
            COMMENCE_SYNERGY,
            FEWELL_FED,
            INTERNAL_PRESSURE_IMPURITY_RATIO,
            INTERNAL_ELEMENTAL_BALANCE,
            EVENT_SKIPPED,
            FURNACE_OVERLOAD,
            SYNERGY_COMPLETE,
            THWACK_FAILED,
            EXPLOSION_AVERTED,
            STATUS_EFFECT_SLOW,
            STATUS_EFFECT_POISON,
            STATUS_EFFECT_PLAGUE,
            TEXTBOX_RELINQUISH_CLAIM,
            TEXTBOX_PERFORM_ACTION,
            TEXTBOX_SELECT_ACTION,
            TEXTBOX_FEED_WHICH_FEWELL,
            TEXTBOX_COMMENCE_SYNERGY,
            TEXTBOX_CONFIRM_ACTION,
            SYNERGY_FAILED,
            INGREDIENTS_REMOVED,
            OUT_OF_FEWELL,
            SYNERGY_SUCCESS,
            FEWELL_LEAK_FIRE,
            FEWELL_LEAK_ICE,
            FEWELL_LEAK_WIND,
            FEWELL_LEAK_EARTH,
            FEWELL_LEAK_LIGHTNING,
            FEWELL_LEAK_WATER,
            FEWELL_LEAK_LIGHT,
            FEWELL_LEAK_DARKNESS,
            FEWELL_LEAK_STOPPED,
            UNKNOWN
        }
        #endregion Enums

        #region Private Members
        private Dictionary<String, Codes> _dictionary;
        private Dictionary<FFXIEnums.ELEMENT, System.Text.RegularExpressions.Regex> _elementalLevelRegex;
        private System.Text.RegularExpressions.Regex _objectiveItemRegex;
        private System.Text.RegularExpressions.Regex _objectiveCountRegex;
        private System.Text.RegularExpressions.Regex _objectiveRankRegex;
        private System.Text.RegularExpressions.Regex _pressureImpurityRegex;
        private Dictionary<String, FFXIEnums.CRAFT_RANK> _craftRankDictionary;
        #region Constants
        private const String _ElementPatternFire = @"\xEF\x1F";
        private const String _ElementPatternIce = @"\xEF\x20";
        private const String _ElementPatternWind = @"\xEF\x21";
        private const String _ElementPatternEarth = @"\xEF\x22";
        private const String _ElementPatternLightning = @"\xEF\x23";
        private const String _ElementPatternWater = @"\xEF\x24";
        private const String _ElementPatternLight = @"\xEF\x25";
        private const String _ElementPatternDark = @"\xEF\x26";
        #endregion Constants
        #endregion Private Members

        #region Constructor
        private SyneDictionairy()
        {
            try
            {
                _elementalLevelRegex = new Dictionary<FFXIEnums.ELEMENT, System.Text.RegularExpressions.Regex>();
                _elementalLevelRegex.Add(FFXIEnums.ELEMENT.FIRE, new System.Text.RegularExpressions.Regex(@"\xEF\x1F(-?\d+)"));
                _elementalLevelRegex.Add(FFXIEnums.ELEMENT.ICE, new System.Text.RegularExpressions.Regex(@"\xEF\x20(-?\d+)"));
                _elementalLevelRegex.Add(FFXIEnums.ELEMENT.WIND, new System.Text.RegularExpressions.Regex(@"\xEF\x21(-?\d+)"));
                _elementalLevelRegex.Add(FFXIEnums.ELEMENT.EARTH, new System.Text.RegularExpressions.Regex(@"\xEF\x22(-?\d+)"));
                _elementalLevelRegex.Add(FFXIEnums.ELEMENT.LIGHTNING, new System.Text.RegularExpressions.Regex(@"\xEF\x23(-?\d+)"));
                _elementalLevelRegex.Add(FFXIEnums.ELEMENT.WATER, new System.Text.RegularExpressions.Regex(@"\xEF\x24(-?\d+)"));
                _elementalLevelRegex.Add(FFXIEnums.ELEMENT.LIGHT, new System.Text.RegularExpressions.Regex(@"\xEF\x25(-?\d+)"));
                _elementalLevelRegex.Add(FFXIEnums.ELEMENT.DARK, new System.Text.RegularExpressions.Regex(@"\xEF\x26(-?\d+)"));

                _objectiveItemRegex = new System.Text.RegularExpressions.Regex(@"(?<=\x1E\x02)(.*?)(?=\x1E\x01)");
                _objectiveCountRegex = new System.Text.RegularExpressions.Regex(@"(?<=Objective: )(\d+)");
                _objectiveRankRegex = new System.Text.RegularExpressions.Regex(@"(?<=Synergy rank: )(.*)(?=\x00)");
                _pressureImpurityRegex = new System.Text.RegularExpressions.Regex(@"(\d+)");


                _craftRankDictionary = new Dictionary<string, FFXIEnums.CRAFT_RANK>();
                _craftRankDictionary.Add("Amateur", FFXIEnums.CRAFT_RANK.AMATEUR);
                _craftRankDictionary.Add("Recruit", FFXIEnums.CRAFT_RANK.RECRUIT);
                _craftRankDictionary.Add("Initiate", FFXIEnums.CRAFT_RANK.INITIATE);
                _craftRankDictionary.Add("Novice", FFXIEnums.CRAFT_RANK.NOVICE);
                _craftRankDictionary.Add("Apprentice", FFXIEnums.CRAFT_RANK.APPRENTICE);
                _craftRankDictionary.Add("Journeyman", FFXIEnums.CRAFT_RANK.JOURNEYMAN);
                _craftRankDictionary.Add("Craftsman", FFXIEnums.CRAFT_RANK.CRAFTSMAN);
                _craftRankDictionary.Add("Artisan", FFXIEnums.CRAFT_RANK.ARTISAN);
                _craftRankDictionary.Add("Adept", FFXIEnums.CRAFT_RANK.ADEPT);
                _craftRankDictionary.Add("Veteran", FFXIEnums.CRAFT_RANK.VETERAN);

                _dictionary = new Dictionary<String, Codes>();
                _dictionary.Add("Possession of a synergy crucible is required to use a synergy furnace.", Codes.NO_ACCESS_TO_SYNERGY);
                _dictionary.Add("Synergy crucible set. You now have claim over the synergy furnace.", Codes.FURNACE_CLAIMED);
                _dictionary.Add("You currently have claim over the synergy furnace.", Codes.FURNACE_ALREADY_CLAIMED);
                _dictionary.Add("Your claim over the synergy furnace will expire in ten seconds.", Codes.FURNACE_ALREADY_CLAIMED);
                _dictionary.Add("Your claim to the synergy furnace has been relinquished.", Codes.FURNACE_LOST);
                _dictionary.Add("Your claim over the synergy furnace has expired.", Codes.FURNACE_LOST);
                _dictionary.Add("You failed to produce the intended item.", Codes.SYNERGY_FAILED);
                _dictionary.Add("The operation failed to produce a significant quantity of cinder.", Codes.SYNERGY_FAILED);
                _dictionary.Add("Target out of range.", Codes.OUT_OF_RANGE);
                _dictionary.Add("You are too far away to operate the synergy furnace.", Codes.OUT_OF_RANGE);
                _dictionary.Add("Total fewell fed:", Codes.FEWELL_LEVELS);
                _dictionary.Add("Synergy rank: Amateur", Codes.RECIPE_CONFIRMATION);
                _dictionary.Add("Synergy rank: Recruit", Codes.RECIPE_CONFIRMATION);
                _dictionary.Add("Synergy rank: Initiate", Codes.RECIPE_CONFIRMATION);
                _dictionary.Add("Synergy rank: Novice", Codes.RECIPE_CONFIRMATION);
                _dictionary.Add("Synergy rank: Apprentice", Codes.RECIPE_CONFIRMATION);
                _dictionary.Add("Synergy rank: Journeyman", Codes.RECIPE_CONFIRMATION);
                _dictionary.Add("Synergy rank: Craftsman", Codes.RECIPE_CONFIRMATION);
                _dictionary.Add("Synergy rank: Artisan", Codes.RECIPE_CONFIRMATION);
                _dictionary.Add("Synergy rank: Adept", Codes.RECIPE_CONFIRMATION);
                _dictionary.Add("Synergy rank: Veteran", Codes.RECIPE_CONFIRMATION);
                _dictionary.Add("Commencing synergy process.", Codes.COMMENCE_SYNERGY);
                _dictionary.Add("You feed the furnace <N> portion of  fewell.", Codes.FEWELL_FED);
                _dictionary.Add("Internal elemental balance:", Codes.INTERNAL_ELEMENTAL_BALANCE);
                _dictionary.Add("Event skipped.", Codes.EVENT_SKIPPED);
                _dictionary.Add("The Synergy Furnace readies Fire Overload.", Codes.FURNACE_OVERLOAD);
                _dictionary.Add("The Synergy Furnace readies Ice Overload.", Codes.FURNACE_OVERLOAD);
                _dictionary.Add("The Synergy Furnace readies Wind Overload.", Codes.FURNACE_OVERLOAD);
                _dictionary.Add("The Synergy Furnace readies Earth Overload.", Codes.FURNACE_OVERLOAD);
                _dictionary.Add("The Synergy Furnace readies Lightning Overload.", Codes.FURNACE_OVERLOAD);
                _dictionary.Add("The Synergy Furnace readies Water Overload.", Codes.FURNACE_OVERLOAD);
                _dictionary.Add("The Synergy Furnace readies Light Overload.", Codes.FURNACE_OVERLOAD);
                _dictionary.Add("The Synergy Furnace readies Dark Overload.", Codes.FURNACE_OVERLOAD);
                _dictionary.Add("Synergy complete!", Codes.SYNERGY_COMPLETE);
                _dictionary.Add("You successfully prevent an explosion!", Codes.EXPLOSION_AVERTED);
                _dictionary.Add("Nothing happens...", Codes.THWACK_FAILED);
                _dictionary.Add("<PLAYER> is afflicted by the effect of slow and cannot operate the furnace.", Codes.STATUS_EFFECT_SLOW);
                _dictionary.Add("Relinquish claim?", Codes.TEXTBOX_RELINQUISH_CLAIM);
                _dictionary.Add("Perform action?", Codes.TEXTBOX_CONFIRM_ACTION);
                _dictionary.Add("Select an action.", Codes.TEXTBOX_SELECT_ACTION);
                _dictionary.Add("Select an action/skill.", Codes.TEXTBOX_PERFORM_ACTION);
                _dictionary.Add("Feed which fewell?", Codes.TEXTBOX_FEED_WHICH_FEWELL);
                _dictionary.Add("Commence synergy?", Codes.TEXTBOX_COMMENCE_SYNERGY);
                _dictionary.Add("You remove a  from the furnace", Codes.INGREDIENTS_REMOVED);
                _dictionary.Add("You do not have enough fewell.", Codes.OUT_OF_FEWELL);
                _dictionary.Add("Internal pressure: <N> Pz/Im", Codes.INTERNAL_PRESSURE_IMPURITY_RATIO);
                _dictionary.Add("You remove a <ITEM> from the furnace.", Codes.INGREDIENTS_REMOVED);
                _dictionary.Add("You remove an <ITEM> from the furnace.", Codes.INGREDIENTS_REMOVED);
                _dictionary.Add("The synergy image has taken form!", Codes.SYNERGY_SUCCESS);
                _dictionary.Add(ChatLine.FireElementTag + " elemental power has begun leaking from the furnace.", Codes.FEWELL_LEAK_FIRE);
                _dictionary.Add(ChatLine.IceElementTag + " elemental power has begun leaking from the furnace.", Codes.FEWELL_LEAK_ICE);
                _dictionary.Add(ChatLine.WindElementTag + " elemental power has begun leaking from the furnace.", Codes.FEWELL_LEAK_WIND);
                _dictionary.Add(ChatLine.EarthElementTag + " elemental power has begun leaking from the furnace.", Codes.FEWELL_LEAK_EARTH);
                _dictionary.Add(ChatLine.LightningElementTag + " elemental power has begun leaking from the furnace.", Codes.FEWELL_LEAK_LIGHTNING);
                _dictionary.Add(ChatLine.WaterElementTag + " elemental power has begun leaking from the furnace.", Codes.FEWELL_LEAK_WATER);
                _dictionary.Add(ChatLine.LightElementTag + " elemental power has begun leaking from the furnace.", Codes.FEWELL_LEAK_LIGHT);
                _dictionary.Add(ChatLine.DarknessElementTag + " elemental power has begun leaking from the furnace.", Codes.FEWELL_LEAK_DARKNESS);
                _dictionary.Add(ChatLine.FireElementTag + " elemental power is no longer leaking.", Codes.FEWELL_LEAK_STOPPED);
                _dictionary.Add(ChatLine.IceElementTag + " elemental power is no longer leaking.", Codes.FEWELL_LEAK_STOPPED);
                _dictionary.Add(ChatLine.WindElementTag + " elemental power is no longer leaking.", Codes.FEWELL_LEAK_STOPPED);
                _dictionary.Add(ChatLine.EarthElementTag + " elemental power is no longer leaking.", Codes.FEWELL_LEAK_STOPPED);
                _dictionary.Add(ChatLine.LightningElementTag + " elemental power is no longer leaking.", Codes.FEWELL_LEAK_STOPPED);
                _dictionary.Add(ChatLine.WaterElementTag + " elemental power is no longer leaking.", Codes.FEWELL_LEAK_STOPPED);
                _dictionary.Add(ChatLine.LightElementTag + " elemental power is no longer leaking.", Codes.FEWELL_LEAK_STOPPED);
                _dictionary.Add(ChatLine.DarknessElementTag + " elemental power is no longer leaking.", Codes.FEWELL_LEAK_STOPPED);
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("SyneDictionairy::SyneDictionairy(): " + e.ToString());
            }
        }
        #endregion Constructor

        #region Singleton Members/Property
        private static volatile SyneDictionairy instance;
        private static object syncRoot = new Object();

        public static SyneDictionairy Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new SyneDictionairy();
                        }

                    }
                }
                return instance;
            }
        }
        #endregion Singleton Members/Property

        #region Public Methods
        #region Parsing
        public bool parseElementalLine(String elementalLine, ref ElementalBalance balance)
        {
            bool retval = false;
            try
            {
                balance.Clear();

                foreach (KeyValuePair<FFXIEnums.ELEMENT, System.Text.RegularExpressions.Regex> keyvalue in _elementalLevelRegex)
                {
                    System.Text.RegularExpressions.Match match = keyvalue.Value.Match(elementalLine);
                    if (match.Success && match.Groups.Count > 1)
                    {
                        balance.Set(keyvalue.Key, int.Parse(match.Groups[1].Value));
                        retval = true;
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("SyneDictionairy::parseElementalLine: " + e.ToString());
            }
            return retval;
        }
        public bool parseObjectiveLine(String objectiveLine, ref ushort quantity, ref String itemName)
        {
            bool retval = false;
            try
            {
                System.Text.RegularExpressions.Match itemNameMatch = _objectiveItemRegex.Match(objectiveLine);
                System.Text.RegularExpressions.Match itemQuanMatch = _objectiveCountRegex.Match(objectiveLine);

                if (itemNameMatch.Success == true && itemQuanMatch.Success == true)
                {
                    quantity = ushort.Parse(itemQuanMatch.Groups[0].Value);
                    itemName = itemNameMatch.Groups[0].Value;
                    retval = true;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("SyneDictionairy::parseObjectiveLine: " + e.ToString());
            }
            return retval;
        }
        public bool parseRankLine(String rankLine, ref FFXIEnums.CRAFT_RANK rank)
        {
            bool retval = false;
            try
            {
                System.Text.RegularExpressions.Match rankMatch = _objectiveRankRegex.Match(rankLine);

                if (rankMatch.Success)
                {
                    try
                    {
                        rank = _craftRankDictionary[rankMatch.Groups[0].Value];
                        retval = true;
                    }
                    catch (KeyNotFoundException)
                    {
                        Logging.LoggingFunctions.Timestamp("SyneDictionairy::parseRankLine: Could not parse rank: '" + rankMatch.Groups[0].Value + "'");
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("SyneDictionairy::parseRankLine: " + e.ToString());
            }
            return retval;
        }
        public int parsePressureLine( String line )
        {
            int retval = 0;
            try
            {
                System.Text.RegularExpressions.Match pressureMatch = _pressureImpurityRegex.Match(line);
                if (pressureMatch.Success)
                {
                    try
                    {
                        int.TryParse(pressureMatch.Groups[0].Value, out retval);
                    }
                    catch (Exception)
                    {
                        Logging.LoggingFunctions.Timestamp("SyneDictionairy::parsePressureLine: Could not parse pressure: '" + pressureMatch.Groups[0].Value + "'");
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("SyneDictionairy::parsePressureLine: " + e.ToString());
            }
            return retval;
        }
        public int parseImpurityRatioLine( String line )
        {
            int retval = 0;
            try
            {
                System.Text.RegularExpressions.Match impurityRatioMatch = _pressureImpurityRegex.Match(line);
                if (impurityRatioMatch.Success)
                {
                    try
                    {
                        int.TryParse(impurityRatioMatch.Groups[0].Value, out retval);
                    }
                    catch (Exception)
                    {
                        Logging.LoggingFunctions.Timestamp("SyneDictionairy::parseImpurityRatioLine: Could not parse Impurity Ratio: '" + impurityRatioMatch.Groups[0].Value + "'");
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("SyneDictionairy::parseImpurityRatioLine: " + e.ToString());
            }
            return retval;
        }
        #endregion Parsing
        public Codes getCode(String line)
        {
            Logging.LoggingFunctions.Debug("SyneDictionairy::getCode " + line, LoggingFunctions.DBG_SCOPE.SYNERGY);
            Codes retval = Codes.UNKNOWN;
            try
            {
                IEnumerable<String> likeKeys = _dictionary.Keys.Where(x => line.Contains(x));
                if (likeKeys.Count() > 0)
                {
                    retval = _dictionary[likeKeys.First()];
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("SyneDictionairy::getCode: " + e.ToString());
            }
            return retval;
        }
        #endregion Public Methods
    }
    #endregion SyneDictionary

    public sealed class Synergizer
    {
        #region Enums
        private enum SynergyState
        {
            INITIALIZED,
            STARTING_THREAD,
            CHECK_PLAYER_STATE,
            SUMMON_PORTAFURNACE,
            CLAIMING_FURNACE,
            CLAIMING_FURNACE_REPLY,
            CHECK_FEWELL,
            CHECK_FEWELL_REPLY,
            TRADE_FEWELL,
            TRADE_FEWELL_REPLY,
            TRADE_INGREDIENTS,
            TRADE_INGREDIENTS_REPLY,
            TARGET_FURNACE_FIRST_TIME,
            RETARGET_FURNACE,
            FEED_FEWELL,
            FEED_FEWELL_REPLY,
            THWACK_FURNACE,
            THWACK_FURNACE_REPLY,
            REMOVE_INGREDIENTS,
            REMOVE_INGREDIENTS_REPLY,
            FORCE_END_SYNERGY,
            SYNERGY_END
        };
        public enum SynergyPot
        {
            FURNACE,
            PORTAFURNACE
        };
        #endregion Enums

        #region Singleton Members/Property
        private static volatile Synergizer _instance;
        private static object _syncroot = new Object();
        public static Synergizer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncroot)
                    {
                        if (_instance == null)
                        {
                            _instance = new Synergizer();
                        }

                    }
                }
                return _instance;
            }
        }
        #endregion Singleton Members/Property

        #region Private Members
        private BackgroundWorker _WorkerThread = new BackgroundWorker();
        
        private SyneRecipe _Recipe;
        private ChatLoggerAsync _ChatLogger;

        private SynergyState _State;

        private bool _Resources_Sufficient;

        private ElementalBalanceUpdateDelegate _FewelLevelsUpdate;
        private RecipeRequirementsUpdateDelegate _RecipeRequirementsUpdate;
        private SynergyEndDelegate _SynergyEnd;
        private ElementalBalanceUpdateDelegate _InternalBalanceUpdate;
        private StatusUpdateDelegate _StatusUpdate;
        private PressureImpurityUpdateDelegate _PressureImpurityUpdate;

        private ElementalBalance _Fewell_Level;
        private ElementalBalance _Internal_Elemental_Balance;
        private ElementalBalance _TargetBalance;
        private int _Pressure;
        private int _Impurity_Ratio;
        private FFXIEnums.ELEMENT _LeakingElement;
        private bool _IsLeaking;
        private bool _Cfg_PowerLevelMode;

        private ushort _furnaceNpcId;

        private bool _AllowFireFewell;
        private bool _AllowIceFewell;
        private bool _AllowWindFewell;
        private bool _AllowEarthFewell;
        private bool _AllowLightningFewell;
        private bool _AllowWaterFewell;
        private bool _AllowLightFewell;
        private bool _AllowDarkFewell;

        private bool _FixLeaks;
        #endregion Private Members

        #region Public Delegates
        public delegate void SynergyEndDelegate(bool successfull, bool resources_sufficient);
        public delegate void RecipeRequirementsUpdateDelegate(FFXIEnums.CRAFT_RANK rank, String Name, ushort Quantity, ElementalBalance balance);
        public delegate void ElementalBalanceUpdateDelegate(ElementalBalance balance);
        public delegate void StatusUpdateDelegate(String status, System.Drawing.Color color);
        public delegate void PressureImpurityUpdateDelegate(int pressure, int impurity);
        #endregion Public Delegates

        #region Public Properties
        public ElementalBalanceUpdateDelegate FewelLevelsUpdate
        {
            set
            {
                _FewelLevelsUpdate = value;
            }
        }
        public RecipeRequirementsUpdateDelegate RecipeRequirementsUpdate
        {
            set
            {
                _RecipeRequirementsUpdate = value;
            }
        }
        public SynergyEndDelegate SynergyEnd
        {
            set
            {
                _SynergyEnd = value;
            }
        }
        public ElementalBalanceUpdateDelegate InternalBalanceUpdate
        {
            set
            {
                _InternalBalanceUpdate = value;
            }
        }
        public StatusUpdateDelegate StatusUpdate
        {
            set
            {
                _StatusUpdate = value;
            }
        }
        public PressureImpurityUpdateDelegate PressureImpurityUpdate
        {
            set
            {
                _PressureImpurityUpdate = value;
            }
        }
        public bool AllowFireFewell
        {
            set
            {
                _AllowFireFewell = value;
            }
        }
        public bool AllowIceFewell
        {
            set
            {
                _AllowIceFewell = value;
            }
        }
        public bool AllowWindFewell
        {
            set
            {
                _AllowWindFewell = value;
            }
        }
        public bool AllowEarthFewell
        {
            set
            {
                _AllowEarthFewell = value;
            }
        }
        public bool AllowLightningFewell
        {
            set
            {
                _AllowLightningFewell = value;
            }
        }
        public bool AllowWaterFewell
        {
            set
            {
                _AllowWaterFewell = value;
            }
        }
        public bool AllowLightFewell
        {
            set
            {
                _AllowLightFewell = value;
            }
        }
        public bool AllowDarkFewell
        {
            set
            {
                _AllowDarkFewell = value;
            }
        }
        public bool FixLeaks
        {
            set
            {
                _FixLeaks = value;
            }
        }
        #endregion Public Properties

        #region Constructor
        private Synergizer()
        {
            _WorkerThread.WorkerReportsProgress = false;
            _WorkerThread.WorkerSupportsCancellation = true;
            _WorkerThread.DoWork += new DoWorkEventHandler(this.ThreadStart);
            _WorkerThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.ThreadCompleted);

            _Resources_Sufficient = true;

            _Fewell_Level = new ElementalBalance();
            _Internal_Elemental_Balance = new ElementalBalance();
            _TargetBalance = new ElementalBalance();

            _furnaceNpcId = MemReads.NPCs.InvalidNpcId;

            _AllowFireFewell = true;
            _AllowIceFewell = true;
            _AllowWindFewell = true;
            _AllowEarthFewell = true;
            _AllowLightningFewell = true;
            _AllowWaterFewell = true;
            _AllowLightFewell = true;
            _AllowDarkFewell = true;

            _FixLeaks = true;

            _ChatLogger = ChatLogManager.Access.GetAsynchronousLogger("Synergizer");
        }
        #endregion Constructor

        #region Public Methods
        public bool Synergize(SyneRecipe Recipe, bool PL_Mode = false)
        {
            try
            {
                if (_WorkerThread.IsBusy != true)
                {
                    _Recipe = Recipe;
                    _Cfg_PowerLevelMode = PL_Mode;
                    _WorkerThread.RunWorkerAsync();
                }
                else
                {
                    Logging.LoggingFunctions.Debug("Synergizer::Synergize: Attempting to start synergizer while active!", Logging.LoggingFunctions.DBG_SCOPE.SYNERGY);
                    return false;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Synergizer::Synergize: " + e.ToString());
            }
            return false;
        }
        public void Abort_Synergize()
        {
            try
            {
                if (_WorkerThread.WorkerSupportsCancellation == true)
                {
                    _WorkerThread.CancelAsync();
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Synergizer::Abort_Synergize: " + e.ToString());
            }
        }
        #endregion Public Methods

        #region Private Methods
        private void UpdateStatus(String status, System.Drawing.Color color)
        {
            try
            {
                if (_StatusUpdate != null)
                {
                    _StatusUpdate(status, color);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Syenrgizer::UpdateStatus: " + e.ToString());
            }
        }
        private void updateFewellLevels(ElementalBalance balance)
        {
            try
            {
                if (_FewelLevelsUpdate != null)
                {
                    _FewelLevelsUpdate(balance);
                }

                _Fewell_Level = balance;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Synergizer::updateFewellLevels: " + e.ToString());
            }
        }
        private void updateRecipeTarget(FFXIEnums.CRAFT_RANK rank, String itemName, ushort quantity, ElementalBalance balance)
        {
            try
            {
                _TargetBalance = balance;
                if (_RecipeRequirementsUpdate != null)
                {
                    _RecipeRequirementsUpdate(rank, itemName, quantity, balance);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Synergizer::updateRecipeTarget: " + e.ToString());
            }
        }
        private void updateInternalElementalBalance(ElementalBalance balance)
        {
            try
            {
                _Internal_Elemental_Balance = balance;
                if (_InternalBalanceUpdate != null)
                {
                    _InternalBalanceUpdate(balance);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Synergizer::updateInternalElementalBalance: " + e.ToString());
            }
        }
        private void updatePressureImpurityRatio(int pressure, int impurityRatio)
        {
            try
            {
                _Pressure = pressure;
                _Impurity_Ratio = impurityRatio;

                if (_PressureImpurityUpdate != null)
                {
                    _PressureImpurityUpdate(pressure, impurityRatio);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Synergizer::updatePressureImpurityRatio: " + e.ToString());
            }
        }
        private List<FFXIEnums.ELEMENT> checkFewellLevels()
        {
            List<FFXIEnums.ELEMENT> retval = new List<FFXIEnums.ELEMENT>();
            try
            {
                // If only you made the elemental balance IENumerable...
                if (_Fewell_Level.Fire < 50)
                {
                    retval.Add(FFXIEnums.ELEMENT.FIRE);
                }
                if (_Fewell_Level.Ice < 50)
                {
                    retval.Add(FFXIEnums.ELEMENT.ICE);
                }
                if (_Fewell_Level.Wind < 50)
                {
                    retval.Add(FFXIEnums.ELEMENT.WIND);
                }
                if (_Fewell_Level.Earth < 50)
                {
                    retval.Add(FFXIEnums.ELEMENT.EARTH);
                }
                if (_Fewell_Level.Lightning < 50)
                {
                    retval.Add(FFXIEnums.ELEMENT.LIGHTNING);
                }
                if (_Fewell_Level.Water < 50)
                {
                    retval.Add(FFXIEnums.ELEMENT.WATER);
                }
                if (_Fewell_Level.Light < 50)
                {
                    retval.Add(FFXIEnums.ELEMENT.LIGHT);
                }
                if (_Fewell_Level.Dark < 50)
                {
                    retval.Add(FFXIEnums.ELEMENT.DARK);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Synergizer::checkFewellLevels: " + e.ToString());
            }
            return retval;
        }
        private bool BalanceWithinLimits()
        {
            try
            {
                if (_Cfg_PowerLevelMode)
                {
                    return false;
                }

                ElementalBalance difference_balance = _TargetBalance - _Internal_Elemental_Balance;
                return ((difference_balance.Fire <= 3) &&
                   (difference_balance.Ice <= 3) &&
                   (difference_balance.Wind <= 3) &&
                   (difference_balance.Earth <= 3) &&
                   (difference_balance.Lightning <= 3) &&
                   (difference_balance.Water <= 3) &&
                   (difference_balance.Light <= 3) &&
                   (difference_balance.Dark <= 3));
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Synergizer::BalanceWithinLimits: " + e.ToString());
            }
            return true;
        }
        private FFXIEnums.ELEMENT SelectFewell()
        {
            FFXIEnums.ELEMENT element = FFXIEnums.ELEMENT.UNKNOWN;
            try
            {
                // If there is a leak, we fix it first.
                if (_IsLeaking && _FixLeaks)
                {
                    switch (_LeakingElement)
                    {
                        case FFXIEnums.ELEMENT.FIRE:
                            return FFXIEnums.ELEMENT.ICE;
                        case FFXIEnums.ELEMENT.ICE:
                            return FFXIEnums.ELEMENT.WIND;
                        case FFXIEnums.ELEMENT.WIND:
                            return FFXIEnums.ELEMENT.EARTH;
                        case FFXIEnums.ELEMENT.EARTH:
                            return FFXIEnums.ELEMENT.LIGHTNING;
                        case FFXIEnums.ELEMENT.LIGHTNING:
                            return FFXIEnums.ELEMENT.WATER;
                        case FFXIEnums.ELEMENT.WATER:
                            return FFXIEnums.ELEMENT.FIRE;
                        case FFXIEnums.ELEMENT.LIGHT:
                            return FFXIEnums.ELEMENT.DARK;
                        case FFXIEnums.ELEMENT.DARK:
                            return FFXIEnums.ELEMENT.LIGHT;
                    }
                }

                // If not, feed the fewell with the largest gap
                ElementalBalance difference_balance = _TargetBalance - _Internal_Elemental_Balance;
                if (_AllowFireFewell == false)
                {
                    difference_balance.Fire = -999;
                }
                if (_AllowIceFewell == false)
                {
                    difference_balance.Ice = -999;
                }
                if (_AllowWindFewell == false)
                {
                    difference_balance.Wind = -999;
                }
                if (_AllowEarthFewell == false)
                {
                    difference_balance.Earth = -999;
                }
                if (_AllowLightningFewell == false)
                {
                    difference_balance.Lightning = -999;
                }
                if (_AllowWaterFewell == false)
                {
                    difference_balance.Water = -999;
                }
                if (_AllowLightFewell == false)
                {
                    difference_balance.Light = -999;
                }
                if (_AllowDarkFewell == false)
                {
                    difference_balance.Dark = -999;
                }

                element = FFXIEnums.ELEMENT.FIRE;
                int diff = difference_balance.Fire;
                // I should make elemental balance IEnumerable with <ELEMENT,Value> keyvaluepairs.
                if (difference_balance.Ice > diff)
                {
                    diff = difference_balance.Ice;
                    element = FFXIEnums.ELEMENT.ICE;
                }

                if (difference_balance.Wind > diff)
                {
                    diff = difference_balance.Wind;
                    element = FFXIEnums.ELEMENT.WIND;
                }

                if (difference_balance.Earth > diff)
                {
                    diff = difference_balance.Earth;
                    element = FFXIEnums.ELEMENT.EARTH;
                }

                if (difference_balance.Lightning > diff)
                {
                    diff = difference_balance.Lightning;
                    element = FFXIEnums.ELEMENT.LIGHTNING;
                }

                if (difference_balance.Water > diff)
                {
                    diff = difference_balance.Water;
                    element = FFXIEnums.ELEMENT.WATER;
                }

                if (difference_balance.Light > diff)
                {
                    diff = difference_balance.Light;
                    element = FFXIEnums.ELEMENT.LIGHT;
                }

                if (difference_balance.Dark > diff)
                {
                    element = FFXIEnums.ELEMENT.DARK;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Synergizer::SelectFewell: " + e.ToString());
            }
            return element;
        }
        private void MoveToIndex(int index)
        {
            try
            {
                while (Memory.MemReads.Windows.Menus.TextStyle.is_open() == true && Memory.MemReads.Windows.Menus.TextStyle.get_curr_index() > index)
                {
                    IocaineFunctions.arrowKeyDown(Keys.Up, 100);
                    IocaineFunctions.delay(100);
                }
                while (Memory.MemReads.Windows.Menus.TextStyle.is_open() == true && Memory.MemReads.Windows.Menus.TextStyle.get_curr_index() < index)
                {
                    IocaineFunctions.arrowKeyDown(Keys.Down, 100);
                    IocaineFunctions.delay(100);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Synergizer::MoveToIndex: " + e.ToString());
            }
        }
        private void ThreadStart(object sender, DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker worker = sender as BackgroundWorker;

                // Start synergy.
                _State = SynergyState.CHECK_PLAYER_STATE;
                _IsLeaking = false;

                // Debug.
                List<ChatLine> pendingLines = new List<ChatLine>();
                // Clear the log for now, we can't clear it once synergy has started.
                _ChatLogger.Reset();
                while (_State != SynergyState.SYNERGY_END)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }

                    // Check if we are in a textchat box.
                    if (Memory.MemReads.Windows.Menus.TextStyle.is_open())
                    {

                        String title = Memory.MemReads.Windows.Menus.TextStyle.get_top_text();
                        Logging.LoggingFunctions.Timestamp("Encountered box: " + title);
                        switch (SyneDictionairy.Instance.getCode(title))
                        {
                            case SyneDictionairy.Codes.TEXTBOX_RELINQUISH_CLAIM:
                                IocaineFunctions.arrowKeyDown(Keys.Down, 100);
                                IocaineFunctions.delay(100);
                                IocaineFunctions.keyDown(Keys.Enter);
                                break;
                            case SyneDictionairy.Codes.TEXTBOX_SELECT_ACTION:
                                switch (_State)
                                {
                                    case SynergyState.FEED_FEWELL:
                                        MoveToIndex(0);
                                        IocaineFunctions.keyDown(Keys.Enter);
                                        break;
                                    case SynergyState.THWACK_FURNACE:
                                        MoveToIndex(1);
                                        IocaineFunctions.keyDown(Keys.Enter);
                                        break;
                                    case SynergyState.FORCE_END_SYNERGY:
                                        MoveToIndex(3);
                                        IocaineFunctions.keyDown(Keys.Enter);
                                        _State = SynergyState.REMOVE_INGREDIENTS;
                                        IocaineFunctions.delay(3000);
                                        break;
                                }
                                break;
                            case SyneDictionairy.Codes.TEXTBOX_FEED_WHICH_FEWELL:
                                switch (SelectFewell())
                                {
                                    case FFXIEnums.ELEMENT.FIRE:
                                        MoveToIndex(0);
                                        break;
                                    case FFXIEnums.ELEMENT.ICE:
                                        MoveToIndex(1);
                                        break;
                                    case FFXIEnums.ELEMENT.WIND:
                                        MoveToIndex(2);
                                        break;
                                    case FFXIEnums.ELEMENT.EARTH:
                                        MoveToIndex(3);
                                        break;
                                    case FFXIEnums.ELEMENT.LIGHTNING:
                                        MoveToIndex(4);
                                        break;
                                    case FFXIEnums.ELEMENT.WATER:
                                        MoveToIndex(5);
                                        break;
                                    case FFXIEnums.ELEMENT.LIGHT:
                                        MoveToIndex(6);
                                        break;
                                    case FFXIEnums.ELEMENT.DARK:
                                        MoveToIndex(7);
                                        break;
                                };
                                IocaineFunctions.keyDown(Keys.Enter);
                                _State = SynergyState.FEED_FEWELL_REPLY;

                                break;
                            case SyneDictionairy.Codes.TEXTBOX_PERFORM_ACTION:
                                switch (_State)
                                {
                                    case SynergyState.THWACK_FURNACE:
                                        MoveToIndex(0);
                                        IocaineFunctions.keyDown(Keys.Enter);
                                        _State = SynergyState.THWACK_FURNACE_REPLY;
                                        break;
                                }
                                break;
                            case SyneDictionairy.Codes.TEXTBOX_CONFIRM_ACTION:
                            case SyneDictionairy.Codes.TEXTBOX_COMMENCE_SYNERGY:
                                MoveToIndex(0);
                                IocaineFunctions.keyDown(Keys.Enter);
                                break;

                            default:
                                Logging.LoggingFunctions.Timestamp("Unrecognized menu: " + title);
                                break;
                        }
                    }

                    // Do something, if we need it, then await the reply.
                    switch (_State)
                    {
                        case SynergyState.CHECK_PLAYER_STATE:
                            //     ushort x = MemReads.Self.Skills.Crafting.get_synergy_skill();
                            UInt16[] status_array = new UInt16[32];
                            MemReads.Self.StatusEffects.get_effects(ref status_array);

                            // Cast Refresh
                            if (status_array.Contains((ushort)Iocaine2.FFXIEnums.STATUS_EFFECT.Refresh) == false)
                            {
                                Iocaine2.Data.Structures.SpellCommand cmd_refresh = new Iocaine2.Data.Structures.SpellCommand("Refresh");
                                cmd_refresh.Execute("<me>");
                                //delay
                                IocaineFunctions.delay(500);
                            }
                            // Cast Cures

                            if (MemReads.Self.Vitals.get_hp_percent() < 50)
                            {
                                Iocaine2.Data.Structures.SpellCommand cmd_cure_v = new Iocaine2.Data.Structures.SpellCommand("Cure III");
                                cmd_cure_v.Execute("<me>");
                                //delay
                                IocaineFunctions.delay(500);
                            }

                            // Cast Stoneskin
                            MemReads.Self.StatusEffects.get_effects(ref status_array);

                            if (status_array.Contains((ushort)Iocaine2.FFXIEnums.STATUS_EFFECT.Stoneskin) == false)
                            {
                                Iocaine2.Data.Structures.SpellCommand cmd_stoneskin = new Iocaine2.Data.Structures.SpellCommand("Stoneskin");
                                cmd_stoneskin.Execute("<me>");
                                //delay
                                IocaineFunctions.delay(500);
                            }

                            // Cast Haste
                            MemReads.Self.StatusEffects.get_effects(ref status_array);
                            if (status_array.Contains((ushort)Iocaine2.FFXIEnums.STATUS_EFFECT.Haste) == false)
                            {
                                Iocaine2.Data.Structures.SpellCommand cmd_stoneskin = new Iocaine2.Data.Structures.SpellCommand("Haste");
                                cmd_stoneskin.Execute("<me>");
                            }

                            _State = SynergyState.CLAIMING_FURNACE;
                            break;
                        case SynergyState.CLAIMING_FURNACE:
                            {
                                UpdateStatus("Claiming Furnace", Statics.Fields.Blue);
                                KeyValuePair<ushort, double> furnaceNPC = MemReads.NPCs.get_NPCIndexClosest("Synergy Furnace");
                                //KeyValuePair<ushort, double> furnaceNPC = new KeyValuePair<ushort, double>();
                                if (furnaceNPC.Key != MemReads.NPCs.InvalidNpcId && Interaction.TargetNPCIndirectById(furnaceNPC.Key))
                                //if ((MemReads.NPCs.get_NPCIndex("Synergy Furnace").Count() > 0)&& (Interaction.TargetNPC("Synergy Furnace") == true))
                                {
                                    _furnaceNpcId = furnaceNPC.Key;
                                    IocaineFunctions.delay(100);
                                    IocaineFunctions.keyDown(Keys.Enter);
                                    // Wait until the reply comes in.
                                    _State = SynergyState.CLAIMING_FURNACE_REPLY;
                                }
                                else
                                {
                                    UpdateStatus("Claiming Furnace Failed", Statics.Fields.Red);
                                    _State = SynergyState.SYNERGY_END;
                                }
                            }
                            break;
                        case SynergyState.CHECK_FEWELL:
                            UpdateStatus("Checking Fewell", Statics.Fields.Blue);
                            if (Interaction.TargetNPCIndirectById(_furnaceNpcId) == false)
                            {
                                _State = SynergyState.SYNERGY_END;
                                break;
                            }
                            IocaineFunctions.delay(100);
                            IocaineFunctions.keyDown(Keys.Enter);
                            _State = SynergyState.CHECK_FEWELL_REPLY;
                            break;
                        case SynergyState.TRADE_FEWELL:
                            {
                                UpdateStatus("Trading Fewell", Statics.Fields.Blue);
                                List<FFXIEnums.ELEMENT> elements = checkFewellLevels();
                                List<ushort> itemIdList = new List<ushort>();
                                List<byte> itemQuanList = new List<byte>();

                                foreach (FFXIEnums.ELEMENT element in elements)
                                {
                                    ushort itemId = Iocaine2.Data.Client.Things.invalidID;
                                    switch (element)
                                    {
                                        case FFXIEnums.ELEMENT.FIRE:
                                            itemIdList.Add(Iocaine2.Data.Client.Things.GetIdFromName("Fire Fewell"));
                                            itemQuanList.Add(1);
                                            break;
                                        case FFXIEnums.ELEMENT.ICE:
                                            itemIdList.Add(Iocaine2.Data.Client.Things.GetIdFromName("Ice Fewell"));
                                            itemQuanList.Add(1);
                                            break;
                                        case FFXIEnums.ELEMENT.WIND:
                                            itemIdList.Add(Iocaine2.Data.Client.Things.GetIdFromName("Wind Fewell"));
                                            itemQuanList.Add(1);
                                            break;
                                        case FFXIEnums.ELEMENT.EARTH:
                                            itemIdList.Add(Iocaine2.Data.Client.Things.GetIdFromName("Earth Fewell"));
                                            itemQuanList.Add(1);
                                            break;
                                        case FFXIEnums.ELEMENT.LIGHTNING:
                                            itemIdList.Add(Iocaine2.Data.Client.Things.GetIdFromName("Lightning Fewell"));
                                            itemQuanList.Add(1);
                                            break;
                                        case FFXIEnums.ELEMENT.WATER:
                                            itemIdList.Add(Iocaine2.Data.Client.Things.GetIdFromName("Water Fewell"));
                                            itemQuanList.Add(1);
                                            break;
                                        case FFXIEnums.ELEMENT.LIGHT:
                                            itemIdList.Add(Iocaine2.Data.Client.Things.GetIdFromName("Light Fewell"));
                                            itemQuanList.Add(1);
                                            break;
                                        case FFXIEnums.ELEMENT.DARK:
                                            itemIdList.Add(Iocaine2.Data.Client.Things.GetIdFromName("Dark Fewell"));
                                            itemQuanList.Add(1);
                                            break;
                                    }
                                    _Resources_Sufficient = true;
                                    if (itemIdList.Count > 0 && Interaction.TradeItemToNpc(itemIdList, itemQuanList, "Synergy Furnace") == false)
                                    {
                                        _Resources_Sufficient = false;
                                        _State = SynergyState.SYNERGY_END;
                                    }
                                    // Wait a second for trading again?
                                    IocaineFunctions.delay(1000);
                                }
                                _State = SynergyState.TRADE_INGREDIENTS;
                            }
                            break;
                        case SynergyState.TRADE_INGREDIENTS:
                            {
                                UpdateStatus("Trading Ingredients", Statics.Fields.Blue);
                                List<ushort> itemIdList = _Recipe.IdList;
                                List<byte> itemQuanList = _Recipe.QuantityList;
                                _Resources_Sufficient = true;
                                if (Interaction.TradeItemToNpc(itemIdList, itemQuanList, "Synergy Furnace") == true)
                                {
                                    _State = SynergyState.TRADE_INGREDIENTS_REPLY;
                                }
                                else
                                {
                                    _Resources_Sufficient = false;
                                    _State = SynergyState.SYNERGY_END;
                                }
                            }
                            break;
                        case SynergyState.TARGET_FURNACE_FIRST_TIME:
                            UpdateStatus("Targetting Furnace", Statics.Fields.Blue);
                            if (Interaction.TargetNPCIndirectById(_furnaceNpcId) == false)
                            {
                                _State = SynergyState.SYNERGY_END;
                                break;
                            }
                            IocaineFunctions.keyDown(Keys.Enter);
                            //    IocaineFunctions.delay(3500);
                            _State = SynergyState.FEED_FEWELL;
                            break;
                        case SynergyState.RETARGET_FURNACE:
                            UpdateStatus("Targetting Furnace", Statics.Fields.Blue);
                            if (Interaction.TargetNPCIndirectById(_furnaceNpcId) == false)
                            {
                                _State = SynergyState.SYNERGY_END;
                                break;
                            }
                            IocaineFunctions.keyDown(Keys.Enter);
                            //      IocaineFunctions.delay(2000);
                            _State = SynergyState.FEED_FEWELL;
                            break;
                        case SynergyState.REMOVE_INGREDIENTS:
                            UpdateStatus("Removing Ingredients", Statics.Fields.Blue);
                            if (Interaction.TargetNPCIndirectById(_furnaceNpcId) == false)
                            {
                                _State = SynergyState.SYNERGY_END;
                                break;
                            }
                            IocaineFunctions.keyDown(Keys.Enter);
                            _State = SynergyState.REMOVE_INGREDIENTS_REPLY;
                            break;
                    }

                    // Parse Chatlines
                    uint nbLines = 0;
                    _ChatLogger.Update(ref nbLines);
                    for (int ii = 0; ii < nbLines; ii++)
                    {
                        ChatLine chatStr = null;
                        _ChatLogger.Read(out chatStr);
                        //String chatString = chatline.PlainString(Iocaine2.Memory.MemReads.CHAT_FLAGS.NONE, true);

                        SyneDictionairy.Codes chatCode = SyneDictionairy.Instance.getCode(chatStr.ProcessedLine);

                        // Process for lines that are valid for all states.
                        switch (chatCode)
                        {
                            case SyneDictionairy.Codes.OUT_OF_RANGE:
                                _State = SynergyState.SYNERGY_END;
                                break;
                            case SyneDictionairy.Codes.NO_ACCESS_TO_SYNERGY:
                                MessageBox.Show("No Access to Synergy");
                                _State = SynergyState.SYNERGY_END;
                                break;
                            case SyneDictionairy.Codes.FURNACE_LOST:
                                _State = SynergyState.SYNERGY_END;
                                break;
                            case SyneDictionairy.Codes.FEWELL_LEVELS:
                                {
                                    // New fewell level information.
                                    ElementalBalance balance = new ElementalBalance();
                                    if (SyneDictionairy.Instance.parseElementalLine(chatStr.RawStrings[1], ref balance) == true)
                                    {
                                        updateFewellLevels(balance);
                                    }
                                }
                                break;
                            case SyneDictionairy.Codes.INTERNAL_PRESSURE_IMPURITY_RATIO:
                                {
                                    int pressure = SyneDictionairy.Instance.parsePressureLine(chatStr.RawStrings[0]);
                                    int impurityRatio = SyneDictionairy.Instance.parseImpurityRatioLine(chatStr.RawStrings[1]);
                                    updatePressureImpurityRatio(pressure, impurityRatio);
                                }
                                break;
                            case SyneDictionairy.Codes.FURNACE_ALREADY_CLAIMED:
                                break;
                            case SyneDictionairy.Codes.RECIPE_CONFIRMATION:
                                // Parse the recipe chatline
                                {
                                    FFXIEnums.CRAFT_RANK rank = FFXIEnums.CRAFT_RANK.INITIATE;
                                    ushort quantity = 0;
                                    String itemName = "Unknown";
                                    ElementalBalance balance = new ElementalBalance();
                                    if (SyneDictionairy.Instance.parseRankLine(chatStr.RawStrings[0], ref rank) &&
                                        SyneDictionairy.Instance.parseObjectiveLine(chatStr.RawStrings[1], ref quantity, ref itemName) &&
                                        SyneDictionairy.Instance.parseElementalLine(chatStr.RawStrings[2], ref balance))
                                    {
                                        updateRecipeTarget(rank, itemName, quantity, balance);
                                    }
                                    IocaineFunctions.keyDown(Keys.Enter);
                                    /*
                                                                        IocaineFunctions.delay(500);
                                                                        IocaineFunctions.keyDown(Keys.Up);
                                                                        IocaineFunctions.delay(100);
                                                                        IocaineFunctions.keyDown(Keys.Enter);

                                     */
                                }
                                break;
                            case SyneDictionairy.Codes.INTERNAL_ELEMENTAL_BALANCE:
                                {
                                    ElementalBalance internal_balance = new ElementalBalance();
                                    if (chatStr.RawStrings.Count > 1 && SyneDictionairy.Instance.parseElementalLine(chatStr.RawStrings[1], ref internal_balance) == true)
                                    {
                                        updateInternalElementalBalance(internal_balance);
                                    }
                                }
                                break;
                            case SyneDictionairy.Codes.EVENT_SKIPPED:
                                {
                                    _State = SynergyState.RETARGET_FURNACE;
                                }
                                break;
                            case SyneDictionairy.Codes.SYNERGY_FAILED:
                                {
                                    _State = SynergyState.REMOVE_INGREDIENTS;
                                }
                                break;
                            case SyneDictionairy.Codes.SYNERGY_SUCCESS:
                                {
                                    if (_Cfg_PowerLevelMode == false)
                                    {
                                        _State = SynergyState.FORCE_END_SYNERGY;
                                    }
                                }
                                break;
                            case SyneDictionairy.Codes.OUT_OF_FEWELL:
                                {
                                    _State = SynergyState.TRADE_FEWELL;
                                };
                                break;
                            case SyneDictionairy.Codes.FEWELL_LEAK_FIRE:
                                {
                                    UpdateStatus("Fire Element is Leaking", Statics.Fields.Yellow);
                                    _LeakingElement = FFXIEnums.ELEMENT.FIRE;
                                    _IsLeaking = true;
                                }
                                break;
                            case SyneDictionairy.Codes.FEWELL_LEAK_ICE:
                                {
                                    UpdateStatus("Ice Element is Leaking", Statics.Fields.Yellow);
                                    _LeakingElement = FFXIEnums.ELEMENT.ICE;
                                    _IsLeaking = true;
                                }
                                break;
                            case SyneDictionairy.Codes.FEWELL_LEAK_WIND:
                                {
                                    UpdateStatus("Wind Element is Leaking", Statics.Fields.Yellow);
                                    _LeakingElement = FFXIEnums.ELEMENT.WIND;
                                    _IsLeaking = true;
                                }
                                break;
                            case SyneDictionairy.Codes.FEWELL_LEAK_EARTH:
                                {
                                    UpdateStatus("Earth Element is Leaking", Statics.Fields.Yellow);
                                    _LeakingElement = FFXIEnums.ELEMENT.EARTH;
                                    _IsLeaking = true;
                                }
                                break;
                            case SyneDictionairy.Codes.FEWELL_LEAK_LIGHTNING:
                                {
                                    UpdateStatus("Lightning Element is Leaking", Statics.Fields.Yellow);
                                    _LeakingElement = FFXIEnums.ELEMENT.LIGHTNING;
                                    _IsLeaking = true;
                                }
                                break;
                            case SyneDictionairy.Codes.FEWELL_LEAK_WATER:
                                {
                                    UpdateStatus("Water Element is Leaking", Statics.Fields.Yellow);
                                    _LeakingElement = FFXIEnums.ELEMENT.WATER;
                                    _IsLeaking = true;
                                }
                                break;
                            case SyneDictionairy.Codes.FEWELL_LEAK_LIGHT:
                                {
                                    UpdateStatus("Light Element is Leaking", Statics.Fields.Yellow);
                                    _LeakingElement = FFXIEnums.ELEMENT.LIGHT;
                                    _IsLeaking = true;
                                }
                                break;
                            case SyneDictionairy.Codes.FEWELL_LEAK_DARKNESS:
                                {
                                    UpdateStatus("Darkness Element is Leaking", Statics.Fields.Yellow);
                                    _LeakingElement = FFXIEnums.ELEMENT.DARK;
                                    _IsLeaking = true;
                                }
                                break;
                            case SyneDictionairy.Codes.FEWELL_LEAK_STOPPED:
                                {
                                    UpdateStatus("Leaking is stopped", Statics.Fields.Yellow);
                                    _IsLeaking = false;
                                }
                                break;
                        }



                        // If need be, process state specific cases.
                        switch (_State)
                        {
                            case SynergyState.SYNERGY_END:
                                break;
                            case SynergyState.CLAIMING_FURNACE_REPLY:
                                UpdateStatus("Claiming Furnace Reply", Statics.Fields.Blue);
                                // Parse the response
                                switch (chatCode)
                                {
                                    case SyneDictionairy.Codes.FURNACE_CLAIMED:
                                        UpdateStatus("Claimed Furnace", Statics.Fields.Blue);
                                        _State = SynergyState.CHECK_FEWELL;
                                        break;
                                    case SyneDictionairy.Codes.FURNACE_ALREADY_CLAIMED:
                                        // Exit the menu
                                        UpdateStatus("Claimed Furnace", Statics.Fields.Blue);
                                        _State = SynergyState.CHECK_FEWELL;
                                        break;
                                    default:
                                        // wait;
                                        break;
                                }
                                break;
                            case SynergyState.CHECK_FEWELL_REPLY:
                                UpdateStatus("Checking Fewell Reply", Statics.Fields.Blue);
                                // Scan the chatlog for task specific data.
                                switch (chatCode)
                                {
                                    case SyneDictionairy.Codes.FEWELL_LEVELS:
                                        // Check fewel levels
                                        if (checkFewellLevels().Count == 0)
                                        {
                                            _State = SynergyState.TRADE_INGREDIENTS;
                                        }
                                        else
                                        {
                                            _State = SynergyState.TRADE_FEWELL;
                                        }
                                        break;

                                }
                                break;

                            case SynergyState.TRADE_FEWELL_REPLY:
                                _State = SynergyState.SYNERGY_END;
                                break;
                            case SynergyState.TRADE_INGREDIENTS_REPLY:
                                UpdateStatus("Trading Ingredients Reply", Statics.Fields.Blue);
                                switch (chatCode)
                                {
                                    case SyneDictionairy.Codes.COMMENCE_SYNERGY:
                                        _State = SynergyState.TARGET_FURNACE_FIRST_TIME;
                                        break;
                                }
                                break;
                            case SynergyState.FEED_FEWELL_REPLY:
                                UpdateStatus("Trading Ingredients Reply", Statics.Fields.Blue);
                                switch (chatCode)
                                {
                                    case SyneDictionairy.Codes.INTERNAL_ELEMENTAL_BALANCE:
                                        if (BalanceWithinLimits())
                                        {
                                            _State = SynergyState.FORCE_END_SYNERGY;
                                        }
                                        else
                                        {
                                            _State = SynergyState.FEED_FEWELL;
                                        }
                                        break;
                                    case SyneDictionairy.Codes.FURNACE_OVERLOAD:
                                        _State = SynergyState.THWACK_FURNACE;
                                        break;
                                }
                                break;
                            case SynergyState.THWACK_FURNACE_REPLY:
                                UpdateStatus("THWACK!!! Parsing result", Statics.Fields.Blue);
                                switch (chatCode)
                                {
                                    case SyneDictionairy.Codes.EXPLOSION_AVERTED:
                                        _State = SynergyState.FEED_FEWELL;
                                        break;
                                    case SyneDictionairy.Codes.THWACK_FAILED:
                                        _State = SynergyState.THWACK_FURNACE;
                                        break;
                                    case SyneDictionairy.Codes.STATUS_EFFECT_SLOW:
                                        _State = SynergyState.THWACK_FURNACE;
                                        break;
                                }
                                break;
                            case SynergyState.REMOVE_INGREDIENTS_REPLY:
                                // parse the lines
                                _State = SynergyState.SYNERGY_END;
                                break;
                            default:
                                // Exit just to be sure.
                                //_State = SynergyState.SYNERGY_END;
                                break;
                        }
                    }
                    IocaineFunctions.delay(500);
                }
                UpdateStatus("Synergy End", Statics.Fields.Blue);
            }
            catch(Exception Ex)
            {
                LoggingFunctions.Error("Synergizer::DoWork: " + Ex.ToString());
            }
        }
        private void ThreadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (_SynergyEnd != null)
                {
                    _SynergyEnd(false, _Resources_Sufficient);
                }
            }
            catch(Exception Ex)
            {
                LoggingFunctions.Error("Synergizer::ThreadCompleted: " + Ex.ToString());
            }
        }
        private bool checkState()
        {
            return true;
        }
        #endregion Private Methods
    }
}
