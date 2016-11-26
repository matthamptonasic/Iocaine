using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Memory;

namespace Iocaine2
{
    public static partial class Statics
    {
        public static class Enums
        {
            #region Enums
            public enum ONLINE_MODE : byte
            {
                ONLINE,
                OFFLINE,
                UNKNOWN
            }
            #endregion Enums
        }

        public static class FuncPtrs
        {
            #region Delegate Types
            #region Native Types
            public delegate void TD_Void_Void();
            public delegate void TD_Void_Int32(Int32 iValue);
            public delegate void TD_Void_UInt32(UInt32 iValue);
            public delegate void TD_Void_Float(float iValue);
            public delegate void TD_Void_String(String iText);
            public delegate void TD_Void_String_UInt16(String iText, ushort iVal);
            public delegate void TD_Void_Bool(Boolean iVal);
            public delegate void TD_Void_Bool_Bool(Boolean iVal0, Boolean iVal1);
            public delegate Boolean TD_Bool_Void();
            public delegate Boolean TD_Bool_String(String iText);
            public delegate Int16 TD_Int16_Void();
            public delegate Int32 TD_Int32_Void();
            public delegate UInt16 TD_UInt16_Void();
            public delegate UInt32 TD_UInt32_Void();
            public delegate String TD_String_Void();
            #endregion Native Types
            #region Dot-Net Types
            public delegate void TD_Void_ChkB_Boolean(CheckBox iChkB, Boolean iBool);
            public delegate void TD_Void_Color(Color iColor);
            public delegate void TD_Void_DateTime(DateTime iDate);
            public delegate void TD_Void_Image(Image iImage);
            public delegate void TD_Void_String_Color(String iText, Color iColor);
            public delegate void TD_Void_TabControl_Int32_TabPage(TabControl iTabControl, Int32 iIndex, TabPage iPageToAdd);
            public delegate void TD_Void_TabControl_String(TabControl iControl, String iKey);
            public delegate void TD_Void_TabControl_TabPage(TabControl iTabControl, TabPage iTabPage);
            #endregion Dot-Net Types
            #region Custom Types
            public delegate void TD_Void_BaitBoxItemList_Int32(List<BaitBoxItem> iBaitItems, Int32 iSelectIndex);
            public delegate void TD_Void_NpcInfoList(List<MemReads.NPCs.NPCInfoStruct> iNpcs);
            public delegate void TD_Void_OnlineMode(Statics.Enums.ONLINE_MODE iMode);
            #endregion Custom Types
            #endregion Delegate Types
            #region Private Members
            private static TD_Void_String_Color setStatusBoxPtr;
            private static TD_Void_UInt32 setTimeRemainingPtr;
            private static TD_Void_String_Color setSellerButtonPtr;
            private static TD_Void_String_Color setBuyerButtonPtr;
            private static TD_Void_String_Color setNavButtonPtr;
            #endregion Private Members
            #region Public Properties
            public static TD_Void_String_Color SetStatusBoxPtr
            {
                get
                {
                    if (setStatusBoxPtr == null)
                    {
                        MessageBox.Show("Statics::setStatusBoxPtr::get: Not set to a value.");
                        Logging.LoggingFunctions.Error("Statics::setStatusBoxPtr::get: Not set to a value.");
                    }
                    return setStatusBoxPtr;
                }
                set
                {
                    setStatusBoxPtr = value;
                }
            }
            public static TD_Void_UInt32 SetTimeRemainingPtr
            {
                get
                {
                    return setTimeRemainingPtr;
                }
                set
                {
                    setTimeRemainingPtr = value;
                }
            }
            public static TD_Void_String_Color SetSellerButtonPtr
            {
                get
                {
                    return setSellerButtonPtr;
                }
                set
                {
                    setSellerButtonPtr = value;
                }
            }
            public static TD_Void_String_Color SetBuyerButtonPtr
            {
                get
                {
                    return setBuyerButtonPtr;
                }
                set
                {
                    setBuyerButtonPtr = value;
                }
            }
            public static TD_Void_String_Color SetNavButtonPtr
            {
                get
                {
                    return setNavButtonPtr;
                }
                set
                {
                    setNavButtonPtr = value;
                }
            }
            #endregion Public Properties
        }

        public static class Files
        {
            #region Private Members
            private static String fisherFilesTopPath = @"Fisher\";
            private static String fishIdsLocalPath = @"Fisher\FishIDs\";
            private static String fishIdsFileName = @"FishIDs.xml";
            private static String fishStatsLocalPath = @"Fisher\FishStats\";
            #endregion Private Members
            #region Public Members
            public static String FisherFilesTopPath
            {
                get
                {
                    return fisherFilesTopPath;
                }
            }
            public static String FishIdsLocalPath
            {
                get
                {
                    return fishIdsLocalPath;
                }
            }
            public static String FishIdsFileName
            {
                get
                {
                    return fishIdsFileName;
                }
            }
            public static String FishStatsLocalPath
            {
                get
                {
                    return fishStatsLocalPath;
                }
            }
            #endregion Public Members
        }

        public static class Fields
        {
            #region Private Members
            private static Color blue = Color.PaleTurquoise;
            private static Color red = Color.LightCoral;
            private static Color yellow = Color.PaleGoldenrod;
            private static Color green = Color.LightGreen;
            private static Color grey = Color.LightGray;
            private static Color white = Color.White;
            #endregion Private Members
            #region Public Properties
            public static Color Blue
            {
                get
                {
                    return blue;
                }
            }
            public static Color Red
            {
                get
                {
                    return red;
                }
            }
            public static Color Yellow
            {
                get
                {
                    return yellow;
                }
            }
            public static Color Green
            {
                get
                {
                    return green;
                }
            }
            public static Color Grey
            {
                get
                {
                    return grey;
                }
            }
            public static Color White
            {
                get
                {
                    return white;
                }
            }
            #endregion Public Properties
        }

        public static class Flags
        {
            #region Private Members
            private static int processState = 0;
            private static bool versionCheckOk = false;
            private static uint iocaineVersion = 0xffffffff;
            #endregion Private Members
            #region Public Properties
            public static int ProcessState
            {
                get
                {
                    return processState;
                }
                set
                {
                    processState = value;
                }
            }
            public static uint IocaineVersion
            {
                get
                {
                    return iocaineVersion;
                }
                set
                {
                    iocaineVersion = value;
                }
            }
            public static bool VersionCheckOk
            {
                get
                {
                    return versionCheckOk;
                }
                set
                {
                    versionCheckOk = value;
                }
            }
            #endregion Public Properties
        }

        public static class Buttons
        {
            #region Private Members
            private static Color green = Color.Lime;
            private static Color yellow = Color.Yellow;
            private static Color red = Color.Red;
            #endregion Private Members
            #region Public Properties
            public static Color Green
            {
                get
                {
                    return green;
                }
            }
            public static Color Yellow
            {
                get
                {
                    return yellow;
                }
            }
            public static Color Red
            {
                get
                {
                    return red;
                }
            }
            #endregion Public Properties
        }

        public static class Constants
        {
            public static class Navigation
            {
                #region Private Members
                private static List<String> keystrokeStrings = new List<string>() { "Enter", "Escape", "Up Arrow", "Down Arrow", "Left Arrow", "Right Arrow" };
                private static Dictionary<String, Keys> keystrokesKeyMap = new Dictionary<string, Keys>() { {"Enter", Keys.Enter},
                                                                                                            {"Escape", Keys.Escape},
                                                                                                            {"Up Arrow", Keys.Up},
                                                                                                            {"Down Arrow", Keys.Down},
                                                                                                            {"Left Arrow", Keys.Left},
                                                                                                            {"Right Arrow", Keys.Right} };
                private static float speedWalking = 4.5f;
                private static double timeTargetNpc = 4d;
                private static double timeZone = 20d;
                private static double timeTrade = 6d;
                private static double timeKeystroke = 0.4d;
                private static double timeCommand = 1d;
                private static uint maxDistForRouteStart = 30;
                #endregion Private Members
                #region Public Properties
                public static List<String> KeystrokeStrings
                {
                    get
                    {
                        return keystrokeStrings;
                    }
                }
                public static Dictionary<String, Keys> KeystrokesKeyMap
                {
                    get
                    {
                        return keystrokesKeyMap;
                    }
                }
                public static float SpeedWalking
                {
                    get
                    {
                        return speedWalking;
                    }
                }
                public static double TimeTargetNpc
                {
                    get
                    {
                        return timeTargetNpc;
                    }
                }
                public static double TimeZone
                {
                    get
                    {
                        return timeZone;
                    }
                }
                public static double TimeTrade
                {
                    get
                    {
                        return timeTrade;
                    }
                }
                public static double TimeKeystroke
                {
                    get
                    {
                        return timeKeystroke;
                    }
                }
                public static double TimeCommand
                {
                    get
                    {
                        return timeCommand;
                    }
                }
                public static uint MaxDistForRouteStart
                {
                    get
                    {
                        return maxDistForRouteStart;
                    }
                }
                #endregion Public Properties
            }
        }

        public static class Settings
        {
            public static class Top
            {
                #region Private Members
                #region Misc
                private static Boolean enableResizeEffects = true;
                private static Boolean flashFishStatsButton = true;
                #endregion Misc
                #region GM Reaction
                private static bool killOnGmTell = false;
                private static bool stopAllOnGmTell = false;
                private static bool stopFisherOnGmTell = true;
                #endregion GM Reaction
                #region Menu Navigation
                private static UInt32 moveUpDownDelay = 500;
                private static UInt32 moveItemDelay = 1100;
                private static UInt32 keyHoldTime = 150;
                #endregion Menu Navigation
                #region Debug Settings
                private static bool retainSettings = false;
                private static UInt32 debugScope = 0x0;
                #endregion Debug Settings
                #region Maps
                private static String mapsPath = @".\Maps\Images\";
                private static Boolean showNpcs = true;
                private static Boolean showNpcNames = false;
                private static Boolean showMobs = true;
                private static Boolean showMobNames = true;
                private static Boolean showPets = false;
                private static Boolean showPetNames = false;
                private static Boolean showPcs = true;
                private static Boolean showPcNames = false;
                private static Boolean showRangeCircle = true;
                #endregion Maps
                #region Fish Stats Box
                private static bool autoReset = true;
                private static bool thisZoneOnly = false;
                #endregion Fish Stats Box
                #region Parser
                private static bool parseItemDescOnStartup = false;
                #endregion Parser
                #endregion Private Members
                #region Public Properties
                #region Misc
                public static Boolean EnableResizeEffects
                {
                    get
                    {
                        return enableResizeEffects;
                    }
                    set
                    {
                        enableResizeEffects = value;
                    }
                }
                public static Boolean FlashFishStatsButton
                {
                    get
                    {
                        return flashFishStatsButton;
                    }
                    set
                    {
                        flashFishStatsButton = value;
                    }
                }
                #endregion Misc
                #region GM Reaction
                public static bool KillOnGmTell
                {
                    get
                    {
                        return killOnGmTell;
                    }
                    set
                    {
                        killOnGmTell = value;
                    }
                }
                public static bool StopAllOnGmTell
                {
                    get
                    {
                        return stopAllOnGmTell;
                    }
                    set
                    {
                        stopAllOnGmTell = value;
                    }
                }
                public static bool StopFisherOnGmTell
                {
                    get
                    {
                        return stopFisherOnGmTell;
                    }
                    set
                    {
                        stopFisherOnGmTell = value;
                    }
                }
                #endregion GM Reaction
                #region Menu Navigation
                public static UInt32 MoveUpDownDelay
                {
                    get
                    {
                        return moveUpDownDelay;
                    }
                    set
                    {
                        moveUpDownDelay = value;
                    }
                }
                public static UInt32 MoveItemDelay
                {
                    get
                    {
                        return moveItemDelay;
                    }
                    set
                    {
                        moveItemDelay = value;
                    }
                }
                public static UInt32 KeyHoldTime
                {
                    get
                    {
                        return keyHoldTime;
                    }
                    set
                    {
                        keyHoldTime = value;
                    }
                }
                #endregion Menu Navigation
                #region Debug Settings
                public static bool RetainSettings
                {
                    get
                    {
                        return retainSettings;
                    }
                    set
                    {
                        retainSettings = value;
                    }
                }
                public static UInt32 DebugScope
                {
                    get
                    {
                        return debugScope;
                    }
                    set
                    {
                        debugScope = value;
                    }
                }
                #endregion Debug Settings
                #region Maps
                public static String MapsPath
                {
                    get
                    {
                        return mapsPath;
                    }
                    set
                    {
                        mapsPath = value;
                    }
                }
                public static Boolean ShowNpcs
                {
                    get
                    {
                        return showNpcs;
                    }
                    set
                    {
                        showNpcs = value;
                    }
                }
                public static Boolean ShowNpcNames
                {
                    get
                    {
                        return showNpcNames;
                    }
                    set
                    {
                        showNpcNames = value;
                    }
                }
                public static Boolean ShowMobs
                {
                    get
                    {
                        return showMobs;
                    }
                    set
                    {
                        showMobs = value;
                    }
                }
                public static Boolean ShowMobNames
                {
                    get
                    {
                        return showMobNames;
                    }
                    set
                    {
                        showMobNames = value;
                    }
                }
                public static Boolean ShowPets
                {
                    get
                    {
                        return showPets;
                    }
                    set
                    {
                        showPets = value;
                    }
                }
                public static Boolean ShowPetNames
                {
                    get
                    {
                        return showPetNames;
                    }
                    set
                    {
                        showPetNames = value;
                    }
                }
                public static Boolean ShowPcs
                {
                    get
                    {
                        return showPcs;
                    }
                    set
                    {
                        showPcs = value;
                    }
                }
                public static Boolean ShowPcNames
                {
                    get
                    {
                        return showPcNames;
                    }
                    set
                    {
                        showPcNames = value;
                    }
                }
                public static Boolean ShowRangeCircle
                {
                    get
                    {
                        return showRangeCircle;
                    }
                    set
                    {
                        showRangeCircle = value;
                    }
                }
                #endregion Maps
                #region Fish Stats Box
                public static bool AutoReset
                {
                    get
                    {
                        return autoReset;
                    }
                    set
                    {
                        autoReset = value;
                    }
                }
                public static bool ThisZoneOnly
                {
                    get
                    {
                        return thisZoneOnly;
                    }
                    set
                    {
                        thisZoneOnly = value;
                    }
                }
                #endregion Fish Stats Box
                #region Parser
                public static bool ParseItemDescOnStartup
                {
                    get
                    {
                        return parseItemDescOnStartup;
                    }
                    set
                    {
                        parseItemDescOnStartup = value;
                    }
                }
                #endregion Parser
                #endregion Public Properties
            }

            public static class Fisher
            {
                #region Private Members
                private static String fisherDonePlaySound = "Fishing Complete.";
                private static String fisherFullPlaySound = "Inventory Full.";
                private static List<BaitBoxItem> baitBoxItems = new List<BaitBoxItem>();
                private static Keys leftArrowKey = Keys.NumPad4;
                private static Keys rightArrowKey = Keys.NumPad6;
                private static Int32 recastDelay = 1000;
                private static Int32 noCatchTimeout = 25;
                private static Boolean killFish = false;
                private static Boolean killFishFixed = true;
                private static Int32 killFishFixedTime = 8;
                private static Boolean killFishRand = false;
                private static Int32 killFishRandTimeMin = 8;
                private static Int32 killFishRandTimeMax = 14;
                private static Boolean killFishProp = false;
                private static Int32 killFishPropTimeMin = 6;
                private static Int32 killFishPropTimeMax = 14;
                private static Boolean doneWait = true;
                private static Boolean doneStop = false;
                private static Boolean doneShutdown = false;
                private static Boolean doneChange = false;
                private static Boolean doneNav = false;
                private static String doneNavTrip = "";
                private static Boolean fullStop = true;
                private static Boolean fullShutdown = false;
                private static Boolean fullChange = false;
                private static Boolean fullContinue = false;
                private static Boolean fullNav = false;
                private static String fullNavTrip = "";
                private static Boolean fatiguedNav = false;
                private static String fatiguedNavTrip = "";
                private static UInt16 fatigueThreshold = 30;
                private static Boolean giveLogoutCommand = false;
                private static String logoutCommand = "/ma Warp <me>";
                private static Boolean fishByID = true;
                private static Boolean fishByHP = false;
                private static Boolean dropItems = false;
                private static Boolean dropMobs = true;
                private static Int32 fishByHPMin = 6000;
                private static Int32 fishByHPMax = 2500;
                private static Boolean catchAll = false;
                private static Boolean catchAllExcept = false;
                private static Boolean releaseFixed = false;
                private static Double releaseTime = 2.8;
                private static Boolean releaseRandom = true;
                private static Double releaseTimeRandomMin = 1.8;
                private static Double releaseTimeRandomMax = 4.2;
                private static Boolean timedStart = false;
                private static DateTime startTime;
                private static Boolean timedEnd = false;
                private static DateTime endTime;
                private static Boolean moveInv = true;
                #endregion Private Members
                #region Public Properties
                public static String FisherDonePlaySound
                {
                    get
                    {
                        return fisherDonePlaySound;
                    }
                    set
                    {
                        fisherDonePlaySound = value;
                    }
                }
                public static String FisherFullPlaySound
                {
                    get
                    {
                        return fisherFullPlaySound;
                    }
                    set
                    {
                        fisherFullPlaySound = value;
                    }
                }
                public static List<BaitBoxItem> BaitBoxItems
                {
                    get
                    {
                        return baitBoxItems;
                    }
                    set
                    {
                        baitBoxItems = value;
                    }
                }
                public static Keys LeftArrowKey
                {
                    get
                    {
                        return leftArrowKey;
                    }
                    set
                    {
                        leftArrowKey = value;
                    }
                }
                public static Keys RightArrowKey
                {
                    get
                    {
                        return rightArrowKey;
                    }
                    set
                    {
                        rightArrowKey = value;
                    }
                }
                public static Int32 RecastDelay
                {
                    get
                    {
                        return recastDelay;
                    }
                    set
                    {
                        recastDelay = value;
                    }
                }
                public static Int32 NoCatchTimeout
                {
                    get
                    {
                        return noCatchTimeout;
                    }
                    set
                    {
                        noCatchTimeout = value;
                    }
                }
                public static Boolean KillFish
                {
                    get
                    {
                        return killFish;
                    }
                    set
                    {
                        killFish = value;
                    }
                }
                public static Boolean KillFishFixed
                {
                    get
                    {
                        return killFishFixed;
                    }
                    set
                    {
                        killFishFixed = value;
                    }
                }
                public static Int32 KillFishFixedTime
                {
                    get
                    {
                        return killFishFixedTime;
                    }
                    set
                    {
                        killFishFixedTime = value;
                    }
                }
                public static Boolean KillFishRand
                {
                    get
                    {
                        return killFishRand;
                    }
                    set
                    {
                        killFishRand = value;
                    }
                }
                public static Int32 KillFishRandTimeMin
                {
                    get
                    {
                        return killFishRandTimeMin;
                    }
                    set
                    {
                        killFishRandTimeMin = value;
                    }
                }
                public static Int32 KillFishRandTimeMax
                {
                    get
                    {
                        return killFishRandTimeMax;
                    }
                    set
                    {
                        killFishRandTimeMax = value;
                    }
                }
                public static Boolean KillFishProp
                {
                    get
                    {
                        return killFishProp;
                    }
                    set
                    {
                        killFishProp = value;
                    }
                }
                public static Int32 KillFishPropTimeMin
                {
                    get
                    {
                        return killFishPropTimeMin;
                    }
                    set
                    {
                        killFishPropTimeMin = value;
                    }
                }
                public static Int32 KillFishPropTimeMax
                {
                    get
                    {
                        return killFishPropTimeMax;
                    }
                    set
                    {
                        killFishPropTimeMax = value;
                    }
                }
                public static Boolean DoneWait
                {
                    get
                    {
                        return doneWait;
                    }
                    set
                    {
                        doneWait = value;
                    }
                }
                public static Boolean DoneStop
                {
                    get
                    {
                        return doneStop;
                    }
                    set
                    {
                        doneStop = value;
                    }
                }
                public static Boolean DoneShutdown
                {
                    get
                    {
                        return doneShutdown;
                    }
                    set
                    {
                        doneShutdown = value;
                    }
                }
                public static Boolean DoneChange
                {
                    get
                    {
                        return doneChange;
                    }
                    set
                    {
                        doneChange = value;
                    }
                }
                public static Boolean DoneNav
                {
                    get
                    {
                        return doneNav;
                    }
                    set
                    {
                        doneNav = value;
                    }
                }
                public static String DoneNavTrip
                {
                    get
                    {
                        return doneNavTrip;
                    }
                    set
                    {
                        doneNavTrip = value;
                    }
                }
                public static Boolean FullStop
                {
                    get
                    {
                        return fullStop;
                    }
                    set
                    {
                        fullStop = value;
                    }
                }
                public static Boolean FullShutdown
                {
                    get
                    {
                        return fullShutdown;
                    }
                    set
                    {
                        fullShutdown = value;
                    }
                }
                public static Boolean FullChange
                {
                    get
                    {
                        return fullChange;
                    }
                    set
                    {
                        fullChange = value;
                    }
                }
                public static Boolean FullContinue
                {
                    get
                    {
                        return fullContinue;
                    }
                    set
                    {
                        fullContinue = value;
                    }
                }
                public static Boolean FullNav
                {
                    get
                    {
                        return fullNav;
                    }
                    set
                    {
                        fullNav = value;
                    }
                }
                public static String FullNavTrip
                {
                    get
                    {
                        return fullNavTrip;
                    }
                    set
                    {
                        fullNavTrip = value;
                    }
                }
                public static Boolean FatiguedNav
                {
                    get
                    {
                        return fatiguedNav;
                    }
                    set
                    {
                        fatiguedNav = value;
                    }
                }
                public static String FatiguedNavTrip
                {
                    get
                    {
                        return fatiguedNavTrip;
                    }
                    set
                    {
                        fatiguedNavTrip = value;
                    }
                }
                public static UInt16 FatigueThreshold
                {
                    get
                    {
                        return fatigueThreshold;
                    }
                    set
                    {
                        fatigueThreshold = value;
                    }
                }
                public static Boolean GiveLogoutCommand
                {
                    get
                    {
                        return giveLogoutCommand;
                    }
                    set
                    {
                        giveLogoutCommand = value;
                    }
                }
                public static String LogoutCommand
                {
                    get
                    {
                        return logoutCommand;
                    }
                    set
                    {
                        logoutCommand = value;
                    }
                }
                public static Boolean FishByID
                {
                    get
                    {
                        return fishByID;
                    }
                    set
                    {
                        fishByID = value;
                    }
                }
                public static Boolean FishByHP
                {
                    get
                    {
                        return fishByHP;
                    }
                    set
                    {
                        fishByHP = value;
                    }
                }
                public static Boolean DropItems
                {
                    get
                    {
                        return dropItems;
                    }
                    set
                    {
                        dropItems = value;
                    }
                }
                public static Boolean DropMobs
                {
                    get
                    {
                        return dropMobs;
                    }
                    set
                    {
                        dropMobs = value;
                    }
                }
                public static Int32 FishByHPMin
                {
                    get
                    {
                        return fishByHPMin;
                    }
                    set
                    {
                        fishByHPMin = value;
                    }
                }
                public static Int32 FishByHPMax
                {
                    get
                    {
                        return fishByHPMax;
                    }
                    set
                    {
                        fishByHPMax = value;
                    }
                }
                public static Boolean CatchAll
                {
                    get
                    {
                        return catchAll;
                    }
                    set
                    {
                        catchAll = value;
                    }
                }
                public static Boolean CatchAllExcept
                {
                    get
                    {
                        return catchAllExcept;
                    }
                    set
                    {
                        catchAllExcept = value;
                    }
                }
                public static Boolean ReleaseFixed
                {
                    get
                    {
                        return releaseFixed;
                    }
                    set
                    {
                        releaseFixed = value;
                    }
                }
                public static Double ReleaseTime
                {
                    get
                    {
                        return releaseTime;
                    }
                    set
                    {
                        releaseTime = value;
                    }
                }
                public static Boolean ReleaseRandom
                {
                    get
                    {
                        return releaseRandom;
                    }
                    set
                    {
                        releaseRandom = value;
                    }
                }
                public static Double ReleaseTimeRandomMin
                {
                    get
                    {
                        return releaseTimeRandomMin;
                    }
                    set
                    {
                        releaseTimeRandomMin = value;
                    }
                }
                public static Double ReleaseTimeRandomMax
                {
                    get
                    {
                        return releaseTimeRandomMax;
                    }
                    set
                    {
                        releaseTimeRandomMax = value;
                    }
                }
                public static Boolean TimedStart
                {
                    get
                    {
                        return timedStart;
                    }
                    set
                    {
                        timedStart = value;
                    }
                }
                public static DateTime StartTime
                {
                    get
                    {
                        return startTime;
                    }
                    set
                    {
                        startTime = value;
                    }
                }
                public static Boolean TimedEnd
                {
                    get
                    {
                        return timedEnd;
                    }
                    set
                    {
                        timedEnd = value;
                    }
                }
                public static DateTime EndTime
                {
                    get
                    {
                        return endTime;
                    }
                    set
                    {
                        endTime = value;
                    }
                }
                public static Boolean MoveInv
                {
                    get
                    {
                        return moveInv;
                    }
                    set
                    {
                        moveInv = value;
                    }
                }
                #endregion Public Properties
            }

            public static class Fish_Stats
            {
                #region Private Members
                #endregion Private Members
                #region Public Members
                #endregion Public Members
            }

            public static class Crafter
            {
                #region Private Members
                private static int extraTime = 0;
                private static String donePlaySound = "Crafting complete.";
                #endregion Private Members
                #region Public Properties
                public static int ExtraTime
                {
                    get
                    {
                        return extraTime;
                    }
                    set
                    {
                        extraTime = value;
                    }
                }
                public static String DonePlaySound
                {
                    get
                    {
                        return donePlaySound;
                    }
                    set
                    {
                        donePlaySound = value;
                    }
                }
                #endregion Public Properties
            }

            public static class Navigation
            {
                #region Private Members
                private static bool sortingTagsFirst = true;
                private static bool sortingUseLastZoneOnly = true;
                private static String tripCompleteSound = "";
                private static bool promptOnRightClick = true;
                private static double intervalDefValue = 2000D;
                private static double minDistDefValue = 2D;
                private static double waitDefValue = 3D;
                #endregion Private Members
                #region Public Properties
                public static bool SortingTagsFirst
                {
                    get
                    {
                        return sortingTagsFirst;
                    }
                    set
                    {
                        sortingTagsFirst = value;
                    }
                }
                public static bool SortingUseLastZoneOnly
                {
                    get
                    {
                        return sortingUseLastZoneOnly;
                    }
                    set
                    {
                        sortingUseLastZoneOnly = value;
                    }
                }
                public static String TripCompleteSound
                {
                    get
                    {
                        return tripCompleteSound;
                    }
                    set
                    {
                        tripCompleteSound = value;
                    }
                }
                public static bool PromptOnRightClick
                {
                    get
                    {
                        return promptOnRightClick;
                    }
                    set
                    {
                        promptOnRightClick = value;
                    }
                }
                public static double IntervalDefValue
                {
                    get
                    {
                        return intervalDefValue;
                    }
                    set
                    {
                        intervalDefValue = value;
                    }
                }
                public static double MinDistDefValue
                {
                    get
                    {
                        return minDistDefValue;
                    }
                    set
                    {
                        minDistDefValue = value;
                    }
                }
                public static double WaitDefValue
                {
                    get
                    {
                        return waitDefValue;
                    }
                    set
                    {
                        waitDefValue = value;
                    }
                }
                #endregion Public Properties
            }

            public static class PowerLevel
            {
                #region Private Members
                #region Resting Value Lists
                private static ArrayList restingValueList = new ArrayList();
                private static ArrayList restingValuePerTypeList = new ArrayList();
                #endregion Resting Value Lists
                #region Main Settings
                private static int defaultDebuffIndex = 0;
                private static int defaultSelfBuffIndex = 0;
                private static uint plCharActivePollFrequency = 3000;
                private static float maxCastDistance = 23;
                private static uint healthUpdateFrequency = 500;
                private static uint dequeuePollFrequency = 500;
                private static uint castTimeModifier = 100;
                private static uint castTimeModifierCures = 100;
                private static uint castTimeMargin = 3500;
                private static uint jaProcTime = 6000;
                private static uint healMpPercLow = 15;
                private static uint healMpPercHigh = 85;
                private static bool restInFight = true;
                private static bool restInFightAlways = false;
                private static bool restInFightLessUpper = true;
                private static bool restInFightLessLower = true;
                private static uint upperMpMargin = 50;
                private static bool neverRest = false;
                private static int restingStyle = 1;
                private static int focusCharacterIndex = 0;
                private static int focusCharacterRow = 1;
                private static int characterGridRowCount = 2;
                private static int followCharacterIndex = 0;
                private static int followDistance = 10;
                private static bool useElasticFollowing = true;
                private static int elasticDistance = 6;
                private static int followDistanceUpper = useElasticFollowing ? (followDistance + elasticDistance) : followDistance;
                private static uint zoneTimer = 10;
                #endregion Main Settings
                #region Queue Assignments
                private static int nbOfQueues = 6;
                private static int thirdCureQ = 1;
                private static int secondCureQ = 2;
                private static int firstCureQ = 3;
                private static int buffQ = 4;
                private static int selfBuffQ = 5;
                private static int debuffQ = 2;
                private static int wakeCureQ = 2;
                private static bool thirdCureUseInFight = true;
                private static bool thirdCureStopRestMpLow = true;
                private static bool thirdCureStopRestMpHigh = true;
                private static bool thirdCureMpLow = true;
                private static bool thirdCureMpHigh = true;
                private static bool secondCureUseInFight = true;
                private static bool secondCureStopRestMpLow = true;
                private static bool secondCureStopRestMpHigh = true;
                private static bool secondCureMpLow = true;
                private static bool secondCureMpHigh = true;
                private static bool firstCureUseInFight = true;
                private static bool firstCureStopRestMpLow = false;
                private static bool firstCureStopRestMpHigh = false;
                private static bool firstCureMpLow = true;
                private static bool firstCureMpHigh = true;
                private static bool buffUseInFight = true;
                private static bool buffStopRestMpLow = false;
                private static bool buffStopRestMpHigh = false;
                private static bool buffMpLow = false;
                private static bool buffMpHigh = true;
                private static bool selfBuffUseInFight = true;
                private static bool selfBuffStopRestMpLow = false;
                private static bool selfBuffStopRestMpHigh = true;
                private static bool selfBuffMpLow = true;
                private static bool selfBuffMpHigh = true;
                private static bool debuffUseInFight = true;
                private static bool debuffStopRestMpLow = true;
                private static bool debuffStopRestMpHigh = true;
                private static bool debuffMpLow = true;
                private static bool debuffMpHigh = true;
                private static bool wakeCureUseInFight = true;
                private static bool wakeCureStopRestMpLow = true;
                private static bool wakeCureStopRestMpHigh = true;
                private static bool wakeCureMpLow = true;
                private static bool wakeCureMpHigh = true;
                #endregion Queue Assignments
                #region Chat Related
                private static bool chatMpReport = false;
                private static String chatMpReportCmd = "/tell -Name MP is <mp> <mpp>";
                private static uint chatMpReportTimer = 300 * 1000;
                private static bool chatHpReport = false;
                private static String chatHpReportCmd = "/tell -Name Help! HP is <hp> <hpp>";
                private static uint chatHpReportLevel = 35;
                private static bool chatCastTime = false;
                private static String chatCastTimeCmd = "/tell -Name Hang on one second";
                private static double chatCastTimeTime = 5.5 * 1000;
                private static bool chatLost = false;
                private static String chatLostCmd = "/tell -Name Where'd you go?";
                #endregion Chat Related
                #region Command Settings
                public static bool command1Enable = false;
                public static String command1Cmd = "/item \"Ginger Cookie\" <me>";
                public static uint command1Timer = 180 * 1000;
                public static bool command1BeforeResting = false;
                public static bool command2Enable = false;
                public static String command2Cmd = "/item \"Yagudo Drink\" <me>";
                public static uint command2Timer = 180 * 1000;
                public static bool command2BeforeResting = false;
                public static bool command3Enable = false;
                public static String command3Cmd = "/item \"Burger\" <me>";
                public static uint command3Timer = 180 * 1000;
                public static bool command3BeforeResting = false;
                public static bool command4Enable = false;
                public static String command4Cmd = "/item \"Fries\" <me>";
                public static uint command4Timer = 180 * 1000;
                public static bool command4BeforeResting = false;
                #endregion Command Settings
                #region Convert Settings
                private static bool convertEnable = false;
                private static bool convertEnInFight = false;
                private static uint convertMpThreshold = 50;
                private static bool convertCureRestFullFirst = true;
                private static uint convertCureRestFullMargin = 25;
                #endregion Convert Settings
                #region Buffs Settings
                private static String wakeCure = "Cure";
                #endregion Misc Settings
                #endregion Private Members
                #region Public Properties
                #region Resting Value Lists
                public static ArrayList RestingValueList
                {
                    get
                    {
                        return restingValueList;
                    }
                    set
                    {
                        restingValueList = value;
                    }
                }
                public static ArrayList RestingValuePerTypeList
                {
                    get
                    {
                        return restingValuePerTypeList;
                    }
                    set
                    {
                        restingValuePerTypeList = value;
                    }
                }
                #endregion Resting Value Lists
                #region Main Settings
                public static int DefaultDebuffIndex
                {
                    get
                    {
                        return defaultDebuffIndex;
                    }
                    set
                    {
                        defaultDebuffIndex = value;
                    }
                }
                public static int DefaultSelfBuffIndex
                {
                    get
                    {
                        return defaultSelfBuffIndex;
                    }
                    set
                    {
                        defaultSelfBuffIndex = value;
                    }
                }
                public static uint PlCharActivePollFrequency
                {
                    get
                    {
                        return plCharActivePollFrequency;
                    }
                    set
                    {
                        plCharActivePollFrequency = value;
                    }
                }
                public static float MaxCastDistance
                {
                    get
                    {
                        return maxCastDistance;
                    }
                    set
                    {
                        maxCastDistance = value;
                    }
                }
                public static uint HealthUpdateFrequency
                {
                    get
                    {
                        return healthUpdateFrequency;
                    }
                    set
                    {
                        healthUpdateFrequency = value;
                    }
                }
                public static uint DequeuePollFrequency
                {
                    get
                    {
                        return dequeuePollFrequency;
                    }
                    set
                    {
                        dequeuePollFrequency = value;
                    }
                }
                public static uint CastTimeModifier
                {
                    get
                    {
                        return castTimeModifier;
                    }
                    set
                    {
                        castTimeModifier = value;
                    }
                }
                public static uint CastTimeModifierCures
                {
                    get
                    {
                        return castTimeModifierCures;
                    }
                    set
                    {
                        castTimeModifierCures = value;
                    }
                }
                public static uint CastTimeMargin
                {
                    get
                    {
                        return castTimeMargin;
                    }
                    set
                    {
                        castTimeMargin = value;
                    }
                }
                public static uint JaProcTime
                {
                    get
                    {
                        return jaProcTime;
                    }
                    set
                    {
                        jaProcTime = value;
                    }
                }
                public static uint HealMpPercLow
                {
                    get
                    {
                        return healMpPercLow;
                    }
                    set
                    {
                        healMpPercLow = value;
                    }
                }
                public static uint HealMpPercHigh
                {
                    get
                    {
                        return healMpPercHigh;
                    }
                    set
                    {
                        healMpPercHigh = value;
                    }
                }
                public static bool RestInFight
                {
                    get
                    {
                        return restInFight;
                    }
                    set
                    {
                        restInFight = value;
                    }
                }
                public static bool RestInFightAlways
                {
                    get
                    {
                        return restInFightAlways;
                    }
                    set
                    {
                        restInFightAlways = value;
                    }
                }
                public static bool RestInFightLessUpper
                {
                    get
                    {
                        return restInFightLessUpper;
                    }
                    set
                    {
                        restInFightLessUpper = value;
                    }
                }
                public static bool RestInFightLessLower
                {
                    get
                    {
                        return restInFightLessLower;
                    }
                    set
                    {
                        restInFightLessLower = value;
                    }
                }
                public static uint UpperMpMargin
                {
                    get
                    {
                        return upperMpMargin;
                    }
                    set
                    {
                        upperMpMargin = value;
                    }
                }
                public static bool NeverRest
                {
                    get
                    {
                        return neverRest;
                    }
                    set
                    {
                        neverRest = value;
                    }
                }
                public static int RestingStyle
                {
                    get
                    {
                        return restingStyle;
                    }
                    set
                    {
                        restingStyle = value;
                    }
                }
                public static int FocusCharacterIndex
                {
                    get
                    {
                        return focusCharacterIndex;
                    }
                    set
                    {
                        focusCharacterIndex = value;
                    }
                }
                public static int FocusCharacterRow
                {
                    get
                    {
                        return focusCharacterRow;
                    }
                    set
                    {
                        focusCharacterRow = value;
                    }
                }
                public static int CharacterGridRowCount
                {
                    get
                    {
                        return characterGridRowCount;
                    }
                    set
                    {
                        characterGridRowCount = value;
                    }
                }
                public static int FollowCharacterIndex
                {
                    get
                    {
                        return followCharacterIndex;
                    }
                    set
                    {
                        followCharacterIndex = value;
                    }
                }
                public static int FollowDistance
                {
                    get
                    {
                        return followDistance;
                    }
                    set
                    {
                        followDistance = value;
                    }
                }
                public static bool UseElasticFollowing
                {
                    get
                    {
                        return useElasticFollowing;
                    }
                    set
                    {
                        useElasticFollowing = value;
                    }
                }
                public static int ElasticDistance
                {
                    get
                    {
                        return elasticDistance;
                    }
                    set
                    {
                        elasticDistance = value;
                    }
                }
                public static int FollowDistanceUpper
                {
                    get
                    {
                        return followDistanceUpper;
                    }
                    set
                    {
                        followDistanceUpper = value;
                    }
                }
                public static uint ZoneTimer
                {
                    get
                    {
                        return zoneTimer;
                    }
                    set
                    {
                        zoneTimer = value;
                    }
                }
                #endregion Main Settings
                #region Queue Assignments
                public static int NbOfQueues
                {
                    get
                    {
                        return nbOfQueues;
                    }
                    set
                    {
                        nbOfQueues = value;
                    }
                }
                public static int ThirdCureQ
                {
                    get
                    {
                        return thirdCureQ;
                    }
                    set
                    {
                        thirdCureQ = value;
                    }
                }
                public static int SecondCureQ
                {
                    get
                    {
                        return secondCureQ;
                    }
                    set
                    {
                        secondCureQ = value;
                    }
                }
                public static int FirstCureQ
                {
                    get
                    {
                        return firstCureQ;
                    }
                    set
                    {
                        firstCureQ = value;
                    }
                }
                public static int BuffQ
                {
                    get
                    {
                        return buffQ;
                    }
                    set
                    {
                        buffQ = value;
                    }
                }
                public static int SelfBuffQ
                {
                    get
                    {
                        return selfBuffQ;
                    }
                    set
                    {
                        selfBuffQ = value;
                    }
                }
                public static int DebuffQ
                {
                    get
                    {
                        return debuffQ;
                    }
                    set
                    {
                        debuffQ = value;
                    }
                }
                public static int WakeCureQ
                {
                    get
                    {
                        return wakeCureQ;
                    }
                    set
                    {
                        wakeCureQ = value;
                    }
                }
                public static bool ThirdCureUseInFight
                {
                    get
                    {
                        return thirdCureUseInFight;
                    }
                    set
                    {
                        thirdCureUseInFight = value;
                    }
                }
                public static bool ThirdCureStopRestMpLow
                {
                    get
                    {
                        return thirdCureStopRestMpLow;
                    }
                    set
                    {
                        thirdCureStopRestMpLow = value;
                    }
                }
                public static bool ThirdCureStopRestMpHigh
                {
                    get
                    {
                        return thirdCureStopRestMpHigh;
                    }
                    set
                    {
                        thirdCureStopRestMpHigh = value;
                    }
                }
                public static bool ThirdCureMpLow
                {
                    get
                    {
                        return thirdCureMpLow;
                    }
                    set
                    {
                        thirdCureMpLow = value;
                    }
                }
                public static bool ThirdCureMpHigh
                {
                    get
                    {
                        return thirdCureMpHigh;
                    }
                    set
                    {
                        thirdCureMpHigh = value;
                    }
                }
                public static bool SecondCureUseInFight
                {
                    get
                    {
                        return secondCureUseInFight;
                    }
                    set
                    {
                        secondCureUseInFight = value;
                    }
                }
                public static bool SecondCureStopRestMpLow
                {
                    get
                    {
                        return secondCureStopRestMpLow;
                    }
                    set
                    {
                        secondCureStopRestMpLow = value;
                    }
                }
                public static bool SecondCureStopRestMpHigh
                {
                    get
                    {
                        return secondCureStopRestMpHigh;
                    }
                    set
                    {
                        secondCureStopRestMpHigh = value;
                    }
                }
                public static bool SecondCureMpLow
                {
                    get
                    {
                        return secondCureMpLow;
                    }
                    set
                    {
                        secondCureMpLow = value;
                    }
                }
                public static bool SecondCureMpHigh
                {
                    get
                    {
                        return secondCureMpHigh;
                    }
                    set
                    {
                        secondCureMpHigh = value;
                    }
                }
                public static bool FirstCureUseInFight
                {
                    get
                    {
                        return firstCureUseInFight;
                    }
                    set
                    {
                        firstCureUseInFight = value;
                    }
                }
                public static bool FirstCureStopRestMpLow
                {
                    get
                    {
                        return firstCureStopRestMpLow;
                    }
                    set
                    {
                        firstCureStopRestMpLow = value;
                    }
                }
                public static bool FirstCureStopRestMpHigh
                {
                    get
                    {
                        return firstCureStopRestMpHigh;
                    }
                    set
                    {
                        firstCureStopRestMpHigh = value;
                    }
                }
                public static bool FirstCureMpLow
                {
                    get
                    {
                        return firstCureMpLow;
                    }
                    set
                    {
                        firstCureMpLow = value;
                    }
                }
                public static bool FirstCureMpHigh
                {
                    get
                    {
                        return firstCureMpHigh;
                    }
                    set
                    {
                        firstCureMpHigh = value;
                    }
                }
                public static bool BuffUseInFight
                {
                    get
                    {
                        return buffUseInFight;
                    }
                    set
                    {
                        buffUseInFight = value;
                    }
                }
                public static bool BuffStopRestMpLow
                {
                    get
                    {
                        return buffStopRestMpLow;
                    }
                    set
                    {
                        buffStopRestMpLow = value;
                    }
                }
                public static bool BuffStopRestMpHigh
                {
                    get
                    {
                        return buffStopRestMpHigh;
                    }
                    set
                    {
                        buffStopRestMpHigh = value;
                    }
                }
                public static bool BuffMpLow
                {
                    get
                    {
                        return buffMpLow;
                    }
                    set
                    {
                        buffMpLow = value;
                    }
                }
                public static bool BuffMpHigh
                {
                    get
                    {
                        return buffMpHigh;
                    }
                    set
                    {
                        buffMpHigh = value;
                    }
                }
                public static bool SelfBuffUseInFight
                {
                    get
                    {
                        return selfBuffUseInFight;
                    }
                    set
                    {
                        selfBuffUseInFight = value;
                    }
                }
                public static bool SelfBuffStopRestMpLow
                {
                    get
                    {
                        return selfBuffStopRestMpLow;
                    }
                    set
                    {
                        selfBuffStopRestMpLow = value;
                    }
                }
                public static bool SelfBuffStopRestMpHigh
                {
                    get
                    {
                        return selfBuffStopRestMpHigh;
                    }
                    set
                    {
                        selfBuffStopRestMpHigh = value;
                    }
                }
                public static bool SelfBuffMpLow
                {
                    get
                    {
                        return selfBuffMpLow;
                    }
                    set
                    {
                        selfBuffMpLow = value;
                    }
                }
                public static bool SelfBuffMpHigh
                {
                    get
                    {
                        return selfBuffMpHigh;
                    }
                    set
                    {
                        selfBuffMpHigh = value;
                    }
                }
                public static bool DebuffUseInFight
                {
                    get
                    {
                        return debuffUseInFight;
                    }
                    set
                    {
                        debuffUseInFight = value;
                    }
                }
                public static bool DebuffStopRestMpLow
                {
                    get
                    {
                        return debuffStopRestMpLow;
                    }
                    set
                    {
                        debuffStopRestMpLow = value;
                    }
                }
                public static bool DebuffStopRestMpHigh
                {
                    get
                    {
                        return debuffStopRestMpHigh;
                    }
                    set
                    {
                        debuffStopRestMpHigh = value;
                    }
                }
                public static bool DebuffMpLow
                {
                    get
                    {
                        return debuffMpLow;
                    }
                    set
                    {
                        debuffMpLow = value;
                    }
                }
                public static bool DebuffMpHigh
                {
                    get
                    {
                        return debuffMpHigh;
                    }
                    set
                    {
                        debuffMpHigh = value;
                    }
                }
                public static bool WakeCureUseInFight
                {
                    get
                    {
                        return wakeCureUseInFight;
                    }
                    set
                    {
                        wakeCureUseInFight = value;
                    }
                }
                public static bool WakeCureStopRestMpLow
                {
                    get
                    {
                        return wakeCureStopRestMpLow;
                    }
                    set
                    {
                        wakeCureStopRestMpLow = value;
                    }
                }
                public static bool WakeCureStopRestMpHigh
                {
                    get
                    {
                        return wakeCureStopRestMpHigh;
                    }
                    set
                    {
                        wakeCureStopRestMpHigh = value;
                    }
                }
                public static bool WakeCureMpLow
                {
                    get
                    {
                        return wakeCureMpLow;
                    }
                    set
                    {
                        wakeCureMpLow = value;
                    }
                }
                public static bool WakeCureMpHigh
                {
                    get
                    {
                        return wakeCureMpHigh;
                    }
                    set
                    {
                        wakeCureMpHigh = value;
                    }
                }
                #endregion Queue Assignments
                #region Chat Related
                public static bool ChatMpReport
                {
                    get
                    {
                        return chatMpReport;
                    }
                    set
                    {
                        chatMpReport = value;
                    }
                }
                public static String ChatMpReportCmd
                {
                    get
                    {
                        return chatMpReportCmd;
                    }
                    set
                    {
                        chatMpReportCmd = value;
                    }
                }
                public static uint ChatMpReportTimer
                {
                    get
                    {
                        return chatMpReportTimer;
                    }
                    set
                    {
                        chatMpReportTimer = value;
                    }
                }
                public static bool ChatHpReport
                {
                    get
                    {
                        return chatHpReport;
                    }
                    set
                    {
                        chatHpReport = value;
                    }
                }
                public static String ChatHpReportCmd
                {
                    get
                    {
                        return chatHpReportCmd;
                    }
                    set
                    {
                        chatHpReportCmd = value;
                    }
                }
                public static uint ChatHpReportLevel
                {
                    get
                    {
                        return chatHpReportLevel;
                    }
                    set
                    {
                        chatHpReportLevel = value;
                    }
                }
                public static bool ChatCastTime
                {
                    get
                    {
                        return chatCastTime;
                    }
                    set
                    {
                        chatCastTime = value;
                    }
                }
                public static String ChatCastTimeCmd
                {
                    get
                    {
                        return chatCastTimeCmd;
                    }
                    set
                    {
                        chatCastTimeCmd = value;
                    }
                }
                public static double ChatCastTimeTime
                {
                    get
                    {
                        return chatCastTimeTime;
                    }
                    set
                    {
                        chatCastTimeTime = value;
                    }
                }
                public static bool ChatLost
                {
                    get
                    {
                        return chatLost;
                    }
                    set
                    {
                        chatLost = value;
                    }
                }
                public static String ChatLostCmd
                {
                    get
                    {
                        return chatLostCmd;
                    }
                    set
                    {
                        chatLostCmd = value;
                    }
                }
                #endregion Chat Related
                #region Command Settings
                public static bool Command1Enable
                {
                    get
                    {
                        return command1Enable;
                    }
                    set
                    {
                        command1Enable = value;
                    }
                }
                public static String Command1Cmd
                {
                    get
                    {
                        return command1Cmd;
                    }
                    set
                    {
                        command1Cmd = value;
                    }
                }
                public static uint Command1Timer
                {
                    get
                    {
                        return command1Timer;
                    }
                    set
                    {
                        command1Timer = value;
                    }
                }
                public static bool Command1BeforeResting
                {
                    get
                    {
                        return command1BeforeResting;
                    }
                    set
                    {
                        command1BeforeResting = value;
                    }
                }
                public static bool Command2Enable
                {
                    get
                    {
                        return command2Enable;
                    }
                    set
                    {
                        command2Enable = value;
                    }
                }
                public static String Command2Cmd
                {
                    get
                    {
                        return command2Cmd;
                    }
                    set
                    {
                        command2Cmd = value;
                    }
                }
                public static uint Command2Timer
                {
                    get
                    {
                        return command2Timer;
                    }
                    set
                    {
                        command2Timer = value;
                    }
                }
                public static bool Command2BeforeResting
                {
                    get
                    {
                        return command2BeforeResting;
                    }
                    set
                    {
                        command2BeforeResting = value;
                    }
                }
                public static bool Command3Enable
                {
                    get
                    {
                        return command3Enable;
                    }
                    set
                    {
                        command3Enable = value;
                    }
                }
                public static String Command3Cmd
                {
                    get
                    {
                        return command3Cmd;
                    }
                    set
                    {
                        command3Cmd = value;
                    }
                }
                public static uint Command3Timer
                {
                    get
                    {
                        return command3Timer;
                    }
                    set
                    {
                        command3Timer = value;
                    }
                }
                public static bool Command3BeforeResting
                {
                    get
                    {
                        return command3BeforeResting;
                    }
                    set
                    {
                        command3BeforeResting = value;
                    }
                }
                public static bool Command4Enable
                {
                    get
                    {
                        return command4Enable;
                    }
                    set
                    {
                        command4Enable = value;
                    }
                }
                public static String Command4Cmd
                {
                    get
                    {
                        return command4Cmd;
                    }
                    set
                    {
                        command4Cmd = value;
                    }
                }
                public static uint Command4Timer
                {
                    get
                    {
                        return command4Timer;
                    }
                    set
                    {
                        command4Timer = value;
                    }
                }
                public static bool Command4BeforeResting
                {
                    get
                    {
                        return command4BeforeResting;
                    }
                    set
                    {
                        command4BeforeResting = value;
                    }
                }
                #endregion Command Settings
                #region Convert Settings
                public static bool ConvertEnable
                {
                    get
                    {
                        return convertEnable;
                    }
                    set
                    {
                        convertEnable = value;
                    }
                }
                public static bool ConvertEnInFight
                {
                    get
                    {
                        return convertEnInFight;
                    }
                    set
                    {
                        convertEnInFight = value;
                    }
                }
                public static uint ConvertMpThreshold
                {
                    get
                    {
                        return convertMpThreshold;
                    }
                    set
                    {
                        convertMpThreshold = value;
                    }
                }
                public static bool ConvertCureRestFullFirst
                {
                    get
                    {
                        return convertCureRestFullFirst;
                    }
                    set
                    {
                        convertCureRestFullFirst = value;
                    }
                }
                public static uint ConvertCureRestFullMargin
                {
                    get
                    {
                        return convertCureRestFullMargin;
                    }
                    set
                    {
                        convertCureRestFullMargin = value;
                    }
                }
                #endregion Convert Settings
                #region Buffs Settings
                public static String WakeCure
                {
                    get
                    {
                        return wakeCure;
                    }
                    set
                    {
                        wakeCure = value;
                    }
                }
                #endregion Misc Settings
                #endregion Public Properties
            }

            public static class TA
            {
                #region Private Members
                private static float fightingDistance = 3.0f;
                #endregion Private Members
                #region Public Properties
                public static float FightingDistance
                {
                    get
                    {
                        return fightingDistance;
                    }
                    set
                    {
                        fightingDistance = value;
                    }
                }
                #endregion Public Properties
            }

            public static class WMS
            {
                #region Private Members
                private static bool backgroundUpdate = true;
                private static uint backgroundScanPeriod = 5;
                private static bool saveThisCharOffline = true;
                #endregion Private Members
                #region Public Properties
                public static bool BackgroundUpdate
                {
                    get
                    {
                        return backgroundUpdate;
                    }
                    set
                    {
                        backgroundUpdate = value;
                    }
                }
                public static uint BackgroundScanPeriod
                {
                    get
                    {
                        return backgroundScanPeriod;
                    }
                    set
                    {
                        backgroundScanPeriod = value;
                    }
                }
                public static bool SaveThisCharOffline
                {
                    get
                    {
                        return saveThisCharOffline;
                    }
                    set
                    {
                        saveThisCharOffline = value;
                    }
                }
                #endregion Public Properties
            }

            public static class Helpers
            {
                #region Private Members
                #region Trader
                private static bool stopOnItem_Trader = false;
                #endregion Trader
                #region Seller Delays
                private static UInt32 enterNpcMenuDelay = 2000;
                private static UInt32 enterToSellDelay = 200;
                private static UInt32 checkHelpTextDelay = 500;
                private static UInt32 changeSellQuanDelay = 100;
                private static UInt32 pressEnterToSellDelay = 200;
                #endregion Seller Delays
                #region Seller, Other Settings
                private static String whenDoneSound_Seller = "Selling Complete.";
                private static bool sellFromBagFirst = false;
                private static bool sortBeforeSelling = true;
                #endregion Seller, Other Settings
                #region Buyer
                private static String whenDoneSound_Buyer = "Buying Complete.";
                private static UInt32 guildWaitTime_Buyer = 800;
                private static bool guildSetIndex_Buyer = false;
                #endregion Buyer
                #endregion Private Members
                #region Public Properties
                #region Trader
                public static bool StopOnItem_Trader
                {
                    get
                    {
                        return stopOnItem_Trader;
                    }
                    set
                    {
                        stopOnItem_Trader = value;
                    }
                }
                #endregion Trader
                #region Seller Delays
                public static UInt32 EnterNpcMenuDelay
                {
                    get
                    {
                        return enterNpcMenuDelay;
                    }
                    set
                    {
                        enterNpcMenuDelay = value;
                    }
                }
                public static UInt32 EnterToSellDelay
                {
                    get
                    {
                        return enterToSellDelay;
                    }
                    set
                    {
                        enterToSellDelay = value;
                    }
                }
                public static UInt32 CheckHelpTextDelay
                {
                    get
                    {
                        return checkHelpTextDelay;
                    }
                    set
                    {
                        checkHelpTextDelay = value;
                    }
                }
                public static UInt32 ChangeSellQuanDelay
                {
                    get
                    {
                        return changeSellQuanDelay;
                    }
                    set
                    {
                        changeSellQuanDelay = value;
                    }
                }
                public static UInt32 PressEnterToSellDelay
                {
                    get
                    {
                        return pressEnterToSellDelay;
                    }
                    set
                    {
                        pressEnterToSellDelay = value;
                    }
                }
                #endregion Seller Delays
                #region Seller, Other Settings
                public static String WhenDoneSound_Seller
                {
                    get
                    {
                        return whenDoneSound_Seller;
                    }
                    set
                    {
                        whenDoneSound_Seller = value;
                    }
                }
                public static bool SellFromBagFirst
                {
                    get
                    {
                        return sellFromBagFirst;
                    }
                    set
                    {
                        sellFromBagFirst = value;
                    }
                }
                public static bool SortBeforeSelling
                {
                    get
                    {
                        return sortBeforeSelling;
                    }
                    set
                    {
                        sortBeforeSelling = value;
                    }
                }
                #endregion Seller, Other Settings
                #region Buyer
                public static String WhenDoneSound_Buyer
                {
                    get
                    {
                        return whenDoneSound_Buyer;
                    }
                    set
                    {
                        whenDoneSound_Buyer = value;
                    }
                }
                public static UInt32 GuildWaitTime_Buyer
                {
                    get
                    {
                        return guildWaitTime_Buyer;
                    }
                    set
                    {
                        guildWaitTime_Buyer = value;
                    }
                }
                public static bool GuildSetIndex_Buyer
                {
                    get
                    {
                        return guildSetIndex_Buyer;
                    }
                    set
                    {
                        guildSetIndex_Buyer = value;
                    }
                }
                #endregion Buyer
                #endregion Public Properties
            }

            public static class Alert
            {
                #region Private Members
                private static Boolean enable = false;
                private static Boolean alwaysAlert = false;
                private static Boolean playMessage = false;
                private static String messageText = "";
                private static bool loopMessage = false;
                private static Boolean flashTaskBar = false;
                private static Boolean pauseBots = false;
                private static List<String> whitelist = new List<string>();
                #endregion Private Members
                #region Public Properties
                public static Boolean Enable
                {
                    get
                    {
                        return enable;
                    }
                    set
                    {
                        enable = value;
                    }
                }
                public static Boolean AlwaysAlert
                {
                    get
                    {
                        return alwaysAlert;
                    }
                    set
                    {
                        alwaysAlert = value;
                    }
                }
                public static Boolean PlayMessage
                {
                    get
                    {
                        return playMessage;
                    }
                    set
                    {
                        playMessage = value;
                    }
                }
                public static String MessageText
                {
                    get
                    {
                        return messageText;
                    }
                    set
                    {
                        messageText = value;
                    }
                }
                public static bool LoopMessage
                {
                    get
                    {
                        return loopMessage;
                    }
                    set
                    {
                        loopMessage = value;
                    }
                }
                public static Boolean FlashTaskBar
                {
                    get
                    {
                        return flashTaskBar;
                    }
                    set
                    {
                        flashTaskBar = value;
                    }
                }
                public static Boolean PauseBots
                {
                    get
                    {
                        return pauseBots;
                    }
                    set
                    {
                        pauseBots = value;
                    }
                }
                public static List<String> Whitelist
                {
                    get
                    {
                        return whitelist;
                    }
                    set
                    {
                        whitelist = value;
                    }
                }
                #endregion Public Properties
            }

            public static class POS
            {
                #region Private Members
                private static float distancePerNudge = 6.0f;
                private static float speed = 6.25f;
                private static float speedDisable = 5.0f;
                private static bool enable = false;
                private static bool disableOnAlert = true;
                private static bool enableOnStartup = true;
                #endregion Private Members
                #region Public Properties
                public static float DistancePerNudge
                {
                    get
                    {
                        return distancePerNudge;
                    }
                    set
                    {
                        distancePerNudge = value;
                    }
                }
                public static float Speed
                {
                    get
                    {
                        return speed;
                    }
                    set
                    {
                        speed = value;
                    }
                }
                public static float SpeedDisable
                {
                    get
                    {
                        return speedDisable;
                    }
                    set
                    {
                        speedDisable = value;
                    }
                }
                public static bool Enable
                {
                    get
                    {
                        return enable;
                    }
                    set
                    {
                        enable = value;
                    }
                }
                public static bool DisableOnAlert
                {
                    get
                    {
                        return disableOnAlert;
                    }
                    set
                    {
                        disableOnAlert = value;
                    }
                }
                public static bool EnableOnStartup
                {
                    get
                    {
                        return enableOnStartup;
                    }
                    set
                    {
                        enableOnStartup = value;
                    }
                }
                #endregion Public Properties
            }
        }

        public static class Datasets
        {
            #region Private Members
            private static MainDB mainDb = null;
            private static bool mainDbInitDone = false;
            private static Routes routesDb = null;
            private static List<String> idleConsumablesNames = new List<string>() { "P. Ygg. Shard I", "P. Ygg. Shard II", 
                                                                                    "P. Ygg. Shard III", "P. Ygg. Shard IV", 
                                                                                    "P. Ygg. Shard V",
                                                                                    "R. Ygg. Shard I", "R. Ygg. Shard II", 
                                                                                    "R. Ygg. Shard III", "R. Ygg. Shard IV", 
                                                                                    "R. Ygg. Shard V",
                                                                                    "As. Ygg. Sh. I", "As. Ygg. Sh. II", 
                                                                                    "As. Ygg. Sh. III", "As. Ygg. Sh. IV", 
                                                                                    "As. Ygg. Sh. V",
                                                                                    "C. Ygg. Shard I", "C. Ygg. Shard II", 
                                                                                    "C. Ygg. Shard III", "C. Ygg. Shard IV", 
                                                                                    "C. Ygg. Shard V",
                                                                                    "Z. Ygg. Shard I", "Z. Ygg. Shard II", 
                                                                                    "Z. Ygg. Shard III", "Z. Ygg. Shard IV", 
                                                                                    "Z. Ygg. Shard V",
                                                                                    "A. Ygg. Shard I", "A. Ygg. Shard II", 
                                                                                    "A. Ygg. Shard III", "A. Ygg. Shard IV", 
                                                                                    "A. Ygg. Shard V",
                                                                                    "Celadon Yggzi I", "Celadon Yggzi II",	
                                                                                    "Celadon Yggzi III", "Celadon Yggzi IV", 
                                                                                    "Celadon Yggzi V",	
                                                                                    "Zaffre Yggzi I", "Zaffre Yggzi II",	
                                                                                    "Zaffre Yggzi III", "Zaffre Yggzi IV", 
                                                                                    "Zaffre Yggzi V",
                                                                                    "Alizarin Yggzi I",	"Alizarin Yggzi II",	
                                                                                    "Alizarin Yggzi III", "Alizarin Yggzi IV", 
                                                                                    "Alizarin Yggzi V",
                                                                                    "Phlox Yggzi I", "Phlox Yggzi II",	
                                                                                    "Phlox Yggzi III", "Phlox Yggzi IV", 
                                                                                    "Phlox Yggzi V",
                                                                                    "Russet Yggzi I", "Russet Yggzi II",	
                                                                                    "Russet Yggzi III", "Russet Yggzi IV", 
                                                                                    "Russet Yggzi V",
                                                                                    "Aster Yggzi I", "Aster Yggzi II",	
                                                                                    "Aster Yggzi III", "Aster Yggzi IV", 
                                                                                    "Aster Yggzi V"
                                                                                    };
            private static List<Inventory.Item> idleConsumables = new List<Inventory.Item>();
            private static Boolean idleConsumablesInitDone = false;
            private static List<String> userTripNames = new List<string>();
            #endregion Private Members
            #region Public Properties
            public static MainDB MainDb
            {
                get
                {
                    return mainDb;
                }
                set
                {
                    mainDb = value;
                }
            }
            public static Routes RoutesDb
            {
                get
                {
                    return routesDb;
                }
                set
                {
                    routesDb = value;
                }
            }
            public static List<Inventory.Item> IdleConsumables
            {
                get
                {
                    if(!idleConsumablesInitDone)
                    {
                        initIdleConumables();
                    }
                    return idleConsumables;
                }
            }
            public static List<String> UserTripNames
            {
                get
                {
                    return userTripNames;
                }
            }
            #endregion Public Properties
            #region Private Methods
            private static void initIdleConumables()
            {
                foreach(String str in idleConsumablesNames)
                {
                    UInt16 id = Things.GetIdFromName(str);
                    if (id != Things.invalidID)
                    {
                        idleConsumables.Add(new Inventory.Item(str, id, Things.ITEM_TYPE.UNKNOWN));
                    }
                }
                idleConsumablesInitDone = true;
            }
            #endregion Private Methods
            #region Public Methods
            public static void InitMainDb()
            {
                if (mainDb == null)
                {
                    mainDb = new MainDB();
                }
                if (!mainDbInitDone)
                {

                }
                mainDbInitDone = true;
            }
            public static void InitRoutesDb()
            {
                if (routesDb == null)
                {
                    routesDb = new Routes();
                }
                foreach (DataTable table in routesDb.Tables)
                {
                    table.Rows.Clear();
                }
            }
            #endregion Public Methods
        }
    }
}
