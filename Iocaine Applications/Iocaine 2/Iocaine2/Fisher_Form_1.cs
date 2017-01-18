using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Bots;
using Iocaine2.Data.Client;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Settings;
using Iocaine2.Threading;
using Iocaine2.Tools;

namespace Iocaine2
{
    public partial class Iocaine_2_Form : Form
    {
        #region Enums
        // TBD : These will all be moved elsewhere once bots are loaded dynamically.
        [System.Reflection.ObfuscationAttribute(Feature = "renaming")]
        public enum TAB_KEYS_MAIN
        {
            Fish_Bot_Tab,
            PL_Bot_Tab,
            PL_Events_Tab,
            Skill_Up_Bot_Tab,
            Crafter_Tab,
            TA_Tab,
            Nav_Tab,
            Helpers_Tab,
            WMS_Tab,
            Alert_Tab,
            About_Tab
        }
        [System.Reflection.ObfuscationAttribute(Feature = "renaming")]
        public enum BOT
        {
            FISHER,
            PL,
            SU,
            CRAFTER,
            TA,
            NAV,
            TRADER,
            BUYER,
            SELLER,
            UNKNOWN
        }
        public enum BOT_STATE
        {
            STOPPED,
            RUNNING,
            PAUSED
        }
        [System.Reflection.ObfuscationAttribute(Feature = "renaming")]
        private enum TAB_KEYS_HELPERS
        {
            Buyer_Tab = 0,
            Seller_Tab = 1,
            Trader_Tab = 2
        }
        [System.Reflection.ObfuscationAttribute(Feature = "renaming")]
        private enum TAB_KEYS_PL
        {
            PL_CuresSubTab,
            PL_BuffsSubTab,
            PL_OffenseSubTab
        }
        private enum TIMER_USER
        {
            NONE,
            ERROR_LOG,
            NAVIGATION
        }
        #endregion Enums

        #region Type Defs
        private delegate void TD_Void_String_TIMERUSER(string iText, TIMER_USER iUser);
        #endregion Type Defs

        #region Members
        #region Form State
        //private static bool m_TOP_Form_active = true; //For flashing the window.
        private static volatile object m_TOP_TimerLabelLock = new object();
        private static TIMER_USER m_TOP_TimerLabelUser = TIMER_USER.NONE;
        private static uint m_TOP_LogErrorsFlashTimerShort = 1000;
        #endregion Form State
        #region Form Layout Settings
        private static int m_TOP_Form_initialWidth;
        private static int m_TOP_Form_initialHeight;
        private static int m_TOP_Form_currentWidth;
        private static int m_TOP_Form_currentHeight;
        private static int m_TOP_Form_currentCenterX;
        private static int m_TOP_Form_currentCenterY;
        private static int m_TOP_Form_previousWidth;
        private static int m_TOP_Form_previousHeight;
        private static int m_TOP_Form_previousCenterX;
        private static int m_TOP_Form_previousCenterY;
        private static int m_TOP_Form_widthWithMap;
        private static int m_TOP_Form_extRtBtnInitialX;
        private const int m_TOP_Form_resizeSteps = 20;
        private const int m_TOP_Form_resizeDelay = 10;
        private const int m_TOP_Form_mapAddWidth = 460;
        private const int m_TOP_Form_mapPosXOff = -5; //-20;
        private const int m_TOP_Form_mapPosYOff = 30;
        private static bool m_TOP_Form_mapExtended = false;
        #endregion Form Layout Settings
        #region Map
        #region PictureBox
        private const uint m_MAP_pollPeriod = 250;
        private static List<MemReads.NPCs.NPCInfoStruct> m_MAP_npcList = new List<MemReads.NPCs.NPCInfoStruct>();
        private static Object m_MAP_npcLock = new object();
        private PictureBox c_MAP_mapPB = null;
        private const int m_MAP_pbSize = 450;
        private static bool m_MAP_mapCurrSet = false;
        private static byte m_MAP_currentMapId = 0;
        private static Maps.MapSet m_MAP_mapCurrMapSet = null;
        private static Maps.MapSet m_MAP_MapCurrMapSet
        {
            get
            {
                return m_MAP_mapCurrMapSet;
            }
            set
            {
                m_MAP_mapCurrMapSet = value;
            }
        }
        private static bool m_MAP_mapIsPanning = false;
        private static PointF m_MAP_mapDragStartPoint = Point.Empty;
        private static PointF m_MAP_mapDragLastRedrawPoint = Point.Empty;
        private static PointF m_MAP_mapStartCenter = Point.Empty;
        #endregion PictureBox
        #region Buttons
        private static bool m_MAP_centerLocked = false;
        private static bool m_MAP_centerZoomLocked = false;
        private static bool MAP_CenterOnce = false;
        private Button c_MAP_centerButton = null;
        private const int m_MAP_centerButtonPosXOff = 25;
        private const int m_MAP_centerButtonWidth = 15;
        private const int m_MAP_centerButtonHeight = 20;
        private Button c_MAP_lockButton = null;
        private const int m_MAP_lockButtonPosXOff = 44;
        private const int m_MAP_lockButtonWidth = 20;
        private const int m_MAP_lockButtonHeight = 20;
        private Button c_MAP_extentsButton = null;
        private const int m_MAP_extentsButtonPosXOff = 1;
        private const int m_MAP_extentsButtonWidth = 20;
        private const int m_MAP_extentsButtonHeight = 20;
        #endregion Buttons
        #region CheckBoxes
        private bool m_MAP_doingInit = false;
        private CheckBox c_MAP_showNpcsChkB = null;
        private const int m_MAP_showNpcsChkBXOff = 2;
        private const int m_MAP_showNpcsChkBYOff = 476;
        private CheckBox c_MAP_showNpcNamesChkB = null;
        private const int m_MAP_showNpcNamesChkBXOff = m_MAP_showNpcsChkBXOff + 48;
        private const int m_MAP_showNpcNamesChkBYOff = m_MAP_showNpcsChkBYOff;
        private CheckBox c_MAP_showMobsChkB = null;
        private const int m_MAP_showMobsChkBXOff = m_MAP_showNpcNamesChkBXOff + 58;
        private const int m_MAP_showMobsChkBYOff = m_MAP_showNpcsChkBYOff;
        private CheckBox c_MAP_showMobNamesChkB = null;
        private const int m_MAP_showMobNamesChkBXOff = m_MAP_showMobsChkBXOff + 48;
        private const int m_MAP_showMobNamesChkBYOff = m_MAP_showNpcsChkBYOff;
        private CheckBox c_MAP_showPcsChkB = null;
        private const int m_MAP_showPcsChkBXOff = m_MAP_showMobNamesChkBXOff + 58;
        private const int m_MAP_showPcsChkBYOff = m_MAP_showNpcsChkBYOff;
        private CheckBox c_MAP_showPcNamesChkB = null;
        private const int m_MAP_showPcNamesChkBXOff = m_MAP_showPcsChkBXOff + 48;
        private const int m_MAP_showPcNamesChkBYOff = m_MAP_showNpcsChkBYOff;
        private CheckBox c_MAP_showPetsChkB = null;
        private const int m_MAP_showPetsChkBXOff = m_MAP_showPcNamesChkBXOff + 58;
        private const int m_MAP_showPetsChkBYOff = m_MAP_showNpcsChkBYOff;
        private CheckBox c_MAP_showPetNamesChkB = null;
        private const int m_MAP_showPetNamesChkBXOff = m_MAP_showPetsChkBXOff + 48;
        private const int m_MAP_showPetNamesChkBYOff = m_MAP_showNpcsChkBYOff;
        #endregion CheckBoxes
        #endregion Map
        #region Server Related
        #endregion Server Related
        #region Current Game Values
        private ushort m_TOP_currentZone;
        #endregion Current Game Values
        #region Chat
        private ArrayList m_TOP_replyNamesList = null;
        private ChatLoggerSync m_TOP_chatLog = null;
        #endregion Chat
        // TBD : Will go away once bots all inherit from Bot class and instances are controled through the BotController.
        #region Bot Instances
        private Bots.Crafter Crafter1 = null;
        private Bots.Trader Trader1 = null;
        #endregion Bot Instances
        #region Threads
        private Thread m_TOP_Thread_crafter = null;
        private Thread m_TOP_Thread_trader = null;
        private IocaineThread m_TOP_IocThread_ChangeMonitor = null;
        private IocaineThread m_TOP_IocThread_map = null;
        private IocaineThread m_TOP_Thread_wms = null;
        #endregion Threads
        #region Process Related
        #region Flag File Info
        private const string m_TOP_File_mainFileName = "Main.exe";
        private const string m_TOP_File_mainFile = @".\" + m_TOP_File_mainFileName;
        private const string m_TOP_File_updaterFileName = "Iocaine2.exe";
        private const string m_TOP_File_updaterFile = @".\" + m_TOP_File_updaterFileName;
        private const string m_TOP_File_versionFileName = "UpdateData.txt";
        private const string m_TOP_File_changeLogFileName = "Change Log.txt";
        private const string m_TOP_File_updatingFileName = "updating";
        private const string m_TOP_File_updatingFile = @".\" + m_TOP_File_updatingFileName;
        private const string m_TOP_File_skipIpCheckFileName = "NO_IP_CHECK.txt";
        private const string m_TOP_File_skipIpCheckFile = @".\" + m_TOP_File_skipIpCheckFileName;
        #endregion Flag File Info
        #region Process Flags
        private static bool m_TOP_cbInitDone = false;
        private static bool m_TOP_taInitDone = false;
        private static bool m_TOP_trInitDone = false;
        private static bool m_TOP_wmsGuiInitDone = false;
        private static string m_TOP_iocaineTitleBarText = "";
        #endregion Process Flags
        #region Debug Settings
        private static bool m_TOP_File_keepPidLogFile = false;
        #endregion Debug Settings
        #endregion Process Related
        #region Tool Tips
        private ToolTip c_TOP_panicButtonTT = new ToolTip();
        private ToolTip c_TOP_statsButtonTT = new ToolTip();
        private ToolTip c_TOP_errorLightTT = new ToolTip();
        #endregion Tool Tips
        #region XML
        private const string m_TOP_Xml_quote = "&quot;";
        private const string m_TOP_Xml_amp = "&amp;";
        private const string m_TOP_Xml_apos = "&apos;";
        private const string m_TOP_Xml_less = "&lt;";
        private const string m_TOP_Xml_greater = "&gt;";
        #endregion XML
        #endregion Members

        #region Main Form Initialization
        #region Main Inits
        #region Summary
        // The main init sequence is:
        // 1. Constructor (which calls InitializeComponent)
        //   - Form Shown event will be fired which calls:
        // 2. Init_Form - Control/Form-based inits.
        // 3. Init_Iocaine - Does the non-Form type inits (threads, dll, and class inits).
        // 4. Init_Process - Called when the first POL process is locked onto.
        // 5. Init_LoggedIn - Called when a character is logged in.
        #endregion Summary
        #region Init Form
        public Iocaine_2_Form()
        {
            this.Shown += new EventHandler(Iocaine_2_Form_Shown);
            InitializeComponent();
        }
        private void Iocaine_2_Form_Shown(System.Object sender, System.EventArgs e)
        {
            // This event call will start the bot initialization _after_ the dialog is shown.
            TOP_Init_Form();
            this.Activated += Iocaine_2_Form_Activated;
            this.Deactivate += Iocaine_2_Form_Deactivate;
        }
        private void TOP_Init_Form()
        {
            Application.DoEvents();
            LoggingFunctions.AddDebugScope(LoggingFunctions.DBG_SCOPE.ALL);
            TOP_Init_setSizing();
            TOP_Init_setMap();
            Inits.Init(this);
        }
        private void TOP_Init_setSizing()
        {
            m_TOP_Form_initialWidth = this.Size.Width;
            m_TOP_Form_initialHeight = this.Size.Height;
            m_TOP_Form_currentWidth = m_TOP_Form_initialWidth;
            m_TOP_Form_currentHeight = m_TOP_Form_initialHeight;
            m_TOP_Form_currentCenterX = m_TOP_Form_currentWidth / 2;
            m_TOP_Form_currentCenterY = m_TOP_Form_currentHeight / 2;
            m_TOP_Form_previousWidth = m_TOP_Form_initialWidth;
            m_TOP_Form_previousHeight = m_TOP_Form_initialHeight;
            m_TOP_Form_previousCenterX = m_TOP_Form_previousWidth / 2;
            m_TOP_Form_previousCenterY = m_TOP_Form_previousHeight / 2;
            m_TOP_Form_widthWithMap = m_TOP_Form_initialWidth + m_TOP_Form_mapAddWidth;
            m_TOP_Form_extRtBtnInitialX = c_ExtendRightButton.Location.X;
        }
        private void TOP_Init_setMap()
        {
            c_MAP_mapPB = new PictureBox();
            c_MAP_mapPB.Size = new Size(m_MAP_pbSize, m_MAP_pbSize);
            c_MAP_mapPB.Location = new Point(m_TOP_Form_initialWidth + m_TOP_Form_mapPosXOff, m_TOP_Form_mapPosYOff);
            c_MAP_mapPB.BorderStyle = BorderStyle.Fixed3D;
            c_MAP_mapPB.MouseEnter += c_MAP_mapPB_MouseEnter;
            c_MAP_mapPB.MouseDown += c_MAP_mapPB_MouseDown;
            c_MAP_mapPB.MouseUp += c_MAP_mapPB_MouseUp;
            c_MAP_mapPB.MouseMove += c_MAP_mapPB_MouseMove;
            c_MAP_mapPB.MouseWheel += c_MAP_mapPB_MouseWheel;
            this.Controls.Add(c_MAP_mapPB);

            c_MAP_lockButton = new Button();
            c_MAP_lockButton.BackColor = System.Drawing.SystemColors.Control;
            c_MAP_lockButton.BackgroundImage = global::Iocaine2.Properties.Resources.MapCenterLock;
            c_MAP_lockButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            c_MAP_lockButton.FlatAppearance.BorderSize = 0;
            c_MAP_lockButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            c_MAP_lockButton.Location = new System.Drawing.Point(m_TOP_Form_initialWidth + m_MAP_lockButtonPosXOff, 5);
            c_MAP_lockButton.Name = "MAP_LockButton";
            c_MAP_lockButton.Size = new System.Drawing.Size(m_MAP_lockButtonWidth, m_MAP_lockButtonHeight);
            c_MAP_lockButton.Click += c_MAP_lockButton_Click;
            this.Controls.Add(c_MAP_lockButton);

            c_MAP_centerButton = new Button();
            c_MAP_centerButton.BackColor = System.Drawing.SystemColors.Control;
            c_MAP_centerButton.BackgroundImage = global::Iocaine2.Properties.Resources.MapCenter;
            c_MAP_centerButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            c_MAP_centerButton.FlatAppearance.BorderSize = 0;
            c_MAP_centerButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            c_MAP_centerButton.Location = new System.Drawing.Point(m_TOP_Form_initialWidth + m_MAP_centerButtonPosXOff, 5);
            c_MAP_centerButton.Name = "MAP_CenterButton";
            c_MAP_centerButton.Size = new System.Drawing.Size(m_MAP_centerButtonWidth, m_MAP_centerButtonHeight);
            c_MAP_centerButton.Click += c_MAP_centerButton_Click;
            this.Controls.Add(c_MAP_centerButton);

            c_MAP_extentsButton = new Button();
            c_MAP_extentsButton.BackColor = System.Drawing.SystemColors.Control;
            c_MAP_extentsButton.BackgroundImage = global::Iocaine2.Properties.Resources.MapExtents;
            c_MAP_extentsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            c_MAP_extentsButton.FlatAppearance.BorderSize = 0;
            c_MAP_extentsButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            c_MAP_extentsButton.Location = new System.Drawing.Point(m_TOP_Form_initialWidth + m_MAP_extentsButtonPosXOff, 5);
            c_MAP_extentsButton.Name = "MAP_ExtentsButton";
            c_MAP_extentsButton.Size = new System.Drawing.Size(m_MAP_extentsButtonWidth, m_MAP_extentsButtonHeight);
            c_MAP_extentsButton.Click += c_MAP_extentsButton_Click;
            this.Controls.Add(c_MAP_extentsButton);

            c_MAP_showNpcsChkB = new CheckBox();
            c_MAP_showNpcsChkB.Name = "MAP_ShowNpcsChkB";
            c_MAP_showNpcsChkB.Text = "NPCs";
            c_MAP_showNpcsChkB.FlatStyle = FlatStyle.Popup;
            c_MAP_showNpcsChkB.FlatAppearance.BorderSize = 0;
            c_MAP_showNpcsChkB.Location = new Point(c_MAP_mapPB.Location.X + m_MAP_showNpcsChkBXOff, m_MAP_showNpcsChkBYOff);
            c_MAP_showNpcsChkB.Width = 55;
            c_MAP_showNpcsChkB.CheckedChanged += c_MAP_showNpcsChkB_CheckedChanged;
            this.Controls.Add(c_MAP_showNpcsChkB);

            c_MAP_showNpcNamesChkB = new CheckBox();
            c_MAP_showNpcNamesChkB.Name = "MAP_ShowNpcNamesChkB";
            c_MAP_showNpcNamesChkB.Text = "Names";
            c_MAP_showNpcNamesChkB.FlatStyle = FlatStyle.Popup;
            c_MAP_showNpcNamesChkB.FlatAppearance.BorderSize = 0;
            c_MAP_showNpcNamesChkB.Location = new Point(c_MAP_mapPB.Location.X + m_MAP_showNpcNamesChkBXOff, m_MAP_showNpcNamesChkBYOff);
            c_MAP_showNpcNamesChkB.Width = 60;
            c_MAP_showNpcNamesChkB.CheckedChanged += c_MAP_showNpcNamesChkB_CheckedChanged;
            this.Controls.Add(c_MAP_showNpcNamesChkB);
            c_MAP_showNpcNamesChkB.BringToFront();

            c_MAP_showMobsChkB = new CheckBox();
            c_MAP_showMobsChkB.Name = "MAP_ShowMobsChkB";
            c_MAP_showMobsChkB.Text = "Mobs";
            c_MAP_showMobsChkB.FlatStyle = FlatStyle.Popup;
            c_MAP_showMobsChkB.FlatAppearance.BorderSize = 0;
            c_MAP_showMobsChkB.Location = new Point(c_MAP_mapPB.Location.X + m_MAP_showMobsChkBXOff, m_MAP_showMobsChkBYOff);
            c_MAP_showMobsChkB.Width = 55;
            c_MAP_showMobsChkB.CheckedChanged += c_MAP_showMobsChkB_CheckedChanged;
            this.Controls.Add(c_MAP_showMobsChkB);
            c_MAP_showMobsChkB.BringToFront();

            c_MAP_showMobNamesChkB = new CheckBox();
            c_MAP_showMobNamesChkB.Name = "MAP_ShowMobNamesChkB";
            c_MAP_showMobNamesChkB.Text = "Names";
            c_MAP_showMobNamesChkB.FlatStyle = FlatStyle.Popup;
            c_MAP_showMobNamesChkB.FlatAppearance.BorderSize = 0;
            c_MAP_showMobNamesChkB.Location = new Point(c_MAP_mapPB.Location.X + m_MAP_showMobNamesChkBXOff, m_MAP_showMobNamesChkBYOff);
            c_MAP_showMobNamesChkB.Width = 60;
            c_MAP_showMobNamesChkB.CheckedChanged += c_MAP_showMobNamesChkB_CheckedChanged;
            this.Controls.Add(c_MAP_showMobNamesChkB);
            c_MAP_showMobNamesChkB.BringToFront();

            c_MAP_showPcsChkB = new CheckBox();
            c_MAP_showPcsChkB.Name = "MAP_ShowPcsChkB";
            c_MAP_showPcsChkB.Text = "Pcs";
            c_MAP_showPcsChkB.FlatStyle = FlatStyle.Popup;
            c_MAP_showPcsChkB.FlatAppearance.BorderSize = 0;
            c_MAP_showPcsChkB.Location = new Point(c_MAP_mapPB.Location.X + m_MAP_showPcsChkBXOff, m_MAP_showPcsChkBYOff);
            c_MAP_showPcsChkB.Width = 55;
            c_MAP_showPcsChkB.CheckedChanged += c_MAP_showPcsChkB_CheckedChanged;
            this.Controls.Add(c_MAP_showPcsChkB);
            c_MAP_showPcsChkB.BringToFront();

            c_MAP_showPcNamesChkB = new CheckBox();
            c_MAP_showPcNamesChkB.Name = "MAP_ShowPcNamesChkB";
            c_MAP_showPcNamesChkB.Text = "Names";
            c_MAP_showPcNamesChkB.FlatStyle = FlatStyle.Popup;
            c_MAP_showPcNamesChkB.FlatAppearance.BorderSize = 0;
            c_MAP_showPcNamesChkB.Location = new Point(c_MAP_mapPB.Location.X + m_MAP_showPcNamesChkBXOff, m_MAP_showPcNamesChkBYOff);
            c_MAP_showPcNamesChkB.Width = 60;
            c_MAP_showPcNamesChkB.CheckedChanged += c_MAP_showPcNamesChkB_CheckedChanged;
            this.Controls.Add(c_MAP_showPcNamesChkB);
            c_MAP_showPcNamesChkB.BringToFront();

            c_MAP_showPetsChkB = new CheckBox();
            c_MAP_showPetsChkB.Name = "MAP_ShowPetsChkB";
            c_MAP_showPetsChkB.Text = "Pets";
            c_MAP_showPetsChkB.FlatStyle = FlatStyle.Popup;
            c_MAP_showPetsChkB.FlatAppearance.BorderSize = 0;
            c_MAP_showPetsChkB.Location = new Point(c_MAP_mapPB.Location.X + m_MAP_showPetsChkBXOff, m_MAP_showPetsChkBYOff);
            c_MAP_showPetsChkB.Width = 55;
            c_MAP_showPetsChkB.CheckedChanged += c_MAP_showPetsChkB_CheckedChanged;
            this.Controls.Add(c_MAP_showPetsChkB);
            c_MAP_showPetsChkB.BringToFront();

            c_MAP_showPetNamesChkB = new CheckBox();
            c_MAP_showPetNamesChkB.Name = "MAP_ShowPetNamesChkB";
            c_MAP_showPetNamesChkB.Text = "Names";
            c_MAP_showPetNamesChkB.FlatStyle = FlatStyle.Popup;
            c_MAP_showPetNamesChkB.FlatAppearance.BorderSize = 0;
            c_MAP_showPetNamesChkB.Location = new Point(c_MAP_mapPB.Location.X + m_MAP_showPetNamesChkBXOff, m_MAP_showPetNamesChkBYOff);
            c_MAP_showPetNamesChkB.Width = 60;
            c_MAP_showPetNamesChkB.CheckedChanged += c_MAP_showPetNamesChkB_CheckedChanged;
            this.Controls.Add(c_MAP_showPetNamesChkB);
            c_MAP_showPetNamesChkB.BringToFront();

            c_MAP_mapPB.BringToFront();
        }
        #endregion Init Form
        #region Init Iocaine
        private bool TOP_Init_Iocaine_Begin()
        {
            bool retValue = true;
            // Add any Iocaine Form level inits to be done before the main function is run.
            // Most likely, this will remain empty.
            // If any fatal errors occur, set the retValue to false.

            return retValue;
        }
        private bool TOP_Init_Iocaine_End()
        {
            bool retValue = true;
            // Add any Iocaine Form level inits to be done after the main function is run.
            // Bot form level inits should go here.
            // If any fatal errors occur, set the retValue to false.

            Data.Structures.CommandManager.Init_Iocaine();
            Data.Structures.ActionManager.Init_Iocaine();

            retValue &= Fisher.Access.Init_Iocaine();   //Fisher inits
            SL_Prc_inits();                             //Seller inits
            BY_Prc_inits();                             //Buyer inits
            retValue &= POS_Init_Iocaine();             //<pos> inits

            return retValue;
        }
        #endregion Init Iocaine
        #region Init Process
        private void TOP_Init_Process_Begin(Object iPolProc)
        {
            // This is called BEFORE the ChangeMonitor.MainProc is updated to this new process.
            LoggingFunctions.Debug("TOP_Init_Process_Begin", LoggingFunctions.DBG_SCOPE.TOP);
            Process polProc = (Process)iPolProc;
            if (polProc == null)
            {
                return;
            }
        }
        private void TOP_Init_Process_ThreadsFrozen()
        {
            // This is called after the process has changed and while the
            // background threads are still frozen.
            // If you need to do anything before resuming the IocaineThreads, do it here.
            LoggingFunctions.Debug("TOP_Init_Process_ThreadsFrozen", LoggingFunctions.DBG_SCOPE.TOP);
            PlayerCache.Reset();
        }
        private void TOP_Init_Process_End()
        {
            // This is called AFTER all other process inits are done.
            // At this point, the background threads are running again and memory reads
            // of the new process may be performed safely.
            LoggingFunctions.Debug("TOP_Init_Process_End", LoggingFunctions.DBG_SCOPE.TOP);
            Fisher.Access.Init_Process();
            #region TA Inits
            m_TOP_taInitDone = doTAInits();
            Bots.TeachersAssistant.init(TA_redrawPlayerLBWrapperCallBack);
            #endregion TA Inits
            Parsing.Lua.Init_Process();
        }
        #endregion Init Process
        #region Init LoggedIn
        private bool TOP_Init_LoggedIn_Begin(string iName)
        {
            // This is called BEFORE all other "LoggedIn" inits are done.
            // At this point, no user settings have been loaded, no PlayerCache
            // data has been updated, nothing like that.
            // Also, this only gets called when logging IN, not logging OUT.
            // Set retVal to false on any fatal errors.
            bool retVal = true;

            return retVal;
        }
        private bool TOP_Init_LoggedIn_End()
        {
            // This is called AFTER all other "LoggedIn" inits are done.
            // At this point, you can refer to the PlayerCache data, assume
            // the user settings have been loaded, etc.
            // Also, this only gets called when logging IN, not logging OUT.
            // Set retVal to false on any fatal errors.
            bool retVal = true;
            #region Fisher Inits / Load Settings
            Fisher.Access.Init_LoggedIn();
            LoggingFunctions.Debug("Fisher inits done.", LoggingFunctions.DBG_SCOPE.FISHER);
            #endregion Fisher Inits / Load Settings
            #region PL Inits / Load Settings
            PL_Init();
            PowerLevel.Access.Init_LoggedIn();
            #endregion PL Inits / Load Settings
            #region SU Inits / Load Settings
            doSUInits();
            SkillUp.Access.Init_LoggedIn();
            #endregion SU Inits / Load Settings
            #region Crafter Inits / Load Settings
            LoggingFunctions.Debug("Initializing the crafter.", LoggingFunctions.DBG_SCOPE.CRAFTER);
            m_TOP_cbInitDone = doCBInits();
            #endregion Crafter Inits / Load Settings
            #region TA Inits / Load Settings
            //Stub for now. Add settings load here.
            #endregion TA Inits / Load Settings
            #region Helpers Inits / Load Settings
            LoggingFunctions.Debug("Initializing the helpers.", LoggingFunctions.DBG_SCOPE.TRADER);
            m_TOP_trInitDone = doTRInits();
            SL_Login_inits();
            BY_Login_inits();
            #endregion Helpers Inits / Load Settings
            #region WMS Inits / Load Settings
            LoggingFunctions.Debug("Initializing the WMS", LoggingFunctions.DBG_SCOPE.WMS);
            WMS_Init_LoggedIn();
            #endregion WMS Inits / Load Settings
            #region Fish Stats Inits / Load Settings
            loadFishStatsSettings();
            #endregion Fish Stats Inits / Load Settings
            POS_Init_LoggedIn();
            #region Map Inits
            Maps.SetValues(Statics.Settings.Top.MapsPath,
                           Server.FTP.DownloadDirectory,
                           Server.FTP.DownloadFile,
                           LoggingFunctions.Timestamp,
                           LoggingFunctions.Error,
                           LoggingFunctions.Debug,
                           TOP_Init_setMapsPath);
            this.Invoke(new Maps.MapsInitTypeDef(Maps.init));
            #endregion Map Inits
            #region NAV Load Settings
            Nav_loadUserSettings();
            #endregion NAV Load Settings
            #region Top Load Settings
            ALR_doInits();
            #endregion Top Load Settings
            #region Flashing Stuff
            TOP_Form_flashStuff();
            #endregion Flashing Stuff
            #region Map
            TOP_MAP_startMapThread();
            #endregion Map

            return retVal;
        }
        #endregion Init LoggedIn
        #endregion Main Inits
        #region ChangeMon Inits
        private void TOP_Init_changeMonitor()
        {
            ChangeMonitor.GetMapId_FctnPtr = Data.Client.Maps.GetMapId;
            ChangeMonitor._zoneChanged += TOP_changeMonitor__zoneChanged;
            ChangeMonitor._mapChanged += TOP_changeMonitor__mapChanged;
            ChangeMonitor._inMogChanged += TOP_changeMonitor__inMogChanged;
            ChangeMonitor._polProcessChanged += TOP_Init_processFfxiProcChange;
            ChangeMonitor._polLoginChanged += new ChangeMonitor.CM_Delegate_POLLoginChanged(TOP_updateTitlebarText);
            ChangeMonitor._polWindowTitleChanged += new ChangeMonitor.CM_Delegate_POLWindowTitleChanged(TOP_changeProcessWindowTitleText);
            #region Command Management
            ChangeMonitor._equ_CombatSkillChanged += new ChangeMonitor.CM_Delegate_Equ_CombatSkillChanged(Data.Structures.CommandManager.Init_CombatSkillChange);
            ChangeMonitor._vitals_AnyJobChanged += new ChangeMonitor.CM_Delegate_Vitals_AnyJobChanged(TOP_vitalsOrAnyJobChangedHandler);
            #endregion Command Management
            Inventory.Containers._CurrentCapacityValuesUpdated += new Inventory.Containers.CurrentCapcityValuesUpdated(TOP_setTopInvCurLabelText);
            Inventory.Containers._MaxCapacityValuesUpdated += new Inventory.Containers.MaxCapacityValuesUpdated(TOP_setTopInvMaxLabelText);
            if (m_TOP_IocThread_ChangeMonitor == null)
            {
                m_TOP_IocThread_ChangeMonitor = new IocaineThread("changeMonitorThread");
                m_TOP_IocThread_ChangeMonitor.__RunMethod = ChangeMonitor.Run;
                ChangeMonitor.__CheckStatus = m_TOP_IocThread_ChangeMonitor.__CheckState;
                m_TOP_IocThread_ChangeMonitor.Start();
            }
        }
        #endregion ChangeMon Inits
        #region Tool Tips
        private void TOP_Init_createToolTips()
        {
            if (c_TOP_panicButtonTT.ToolTipTitle != "Panic Button")
            {
                //default Autopop delay: 5000
                //default Auto delay: 500
                //default Initial delay: 500
                //default Reshow delay: 100

                //Panic Button
                c_TOP_panicButtonTT.AutoPopDelay = 10000;
                c_TOP_panicButtonTT.InitialDelay = 200;
                c_TOP_panicButtonTT.ToolTipIcon = ToolTipIcon.Warning;
                c_TOP_panicButtonTT.ToolTipTitle = "Panic Button";
                c_TOP_panicButtonTT.SetToolTip(c_PanicButton, "Stops all bots and releases any\nmovement keys currently down.");
                //Stats Button
                c_TOP_statsButtonTT.AutoPopDelay = 10000;
                c_TOP_statsButtonTT.InitialDelay = 200;
                c_TOP_statsButtonTT.ToolTipIcon = ToolTipIcon.None;
                c_TOP_statsButtonTT.ToolTipTitle = "Stats Button";
                c_TOP_statsButtonTT.SetToolTip(c_StatsButton, "Displays recorded statistics (from all users)\nabout different fishing setups.");

                c_TOP_errorLightTT.AutoPopDelay = 2000;
                c_TOP_errorLightTT.InitialDelay = 800;
                c_TOP_errorLightTT.ToolTipIcon = ToolTipIcon.None;
                c_TOP_errorLightTT.ToolTipTitle = "Errors Logged";
                c_TOP_errorLightTT.SetToolTip(c_ErrorLightPB, "Click to clear,\nRight click to open log file.");
            }
        }
        #endregion Tool Tips
        #region Init Utilities
        private void TOP_Settings_loadTop()
        {
            Statics.Settings.Top.FlashFishStatsButton = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "FlashFishStatsButton");
            Statics.Settings.Top.EnableResizeEffects = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "EnableResizeEffects");
            Statics.Settings.Top.KillOnGmTell = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "TOP_KillOnGmTell");
            Statics.Settings.Top.StopAllOnGmTell = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "TOP_StopAllOnGmTell");
            Statics.Settings.Top.StopFisherOnGmTell = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "TOP_StopFisherOnGmTell");
            Statics.Settings.Top.MoveUpDownDelay = (uint)UserSettings.GetValue(UserSettings.BOT.TOP, "MoveUpDownDelay");
            Statics.Settings.Top.MoveItemDelay = (uint)UserSettings.GetValue(UserSettings.BOT.TOP, "MoveItemDelay");
            Statics.Settings.Top.KeyHoldTime = (uint)UserSettings.GetValue(UserSettings.BOT.TOP, "KeyHoldTime");
            string FisherLeftArrowKey = (string)UserSettings.GetValue(UserSettings.BOT.TOP, "FisherLeftArrowKey");
            if (FisherLeftArrowKey == "NumPad4")
            {
                Statics.Settings.Fisher.LeftArrowKey = Keys.NumPad4;
            }
            else if (FisherLeftArrowKey == "A")
            {
                Statics.Settings.Fisher.LeftArrowKey = Keys.A;
            }
            else
            {
                Statics.Settings.Fisher.LeftArrowKey = Keys.NumPad4;
            }
            string FisherRightArrowKey = (string)UserSettings.GetValue(UserSettings.BOT.TOP, "FisherRightArrowKey");
            if (FisherRightArrowKey == "NumPad6")
            {
                Statics.Settings.Fisher.RightArrowKey = Keys.NumPad6;
            }
            else if (FisherRightArrowKey == "D")
            {
                Statics.Settings.Fisher.RightArrowKey = Keys.D;
            }
            else
            {
                Statics.Settings.Fisher.RightArrowKey = Keys.NumPad6;
            }
            m_MAP_doingInit = true;
            Statics.Settings.Top.ShowNpcs = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "ShowNpcs");
            Statics.Settings.Top.ShowNpcNames = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "ShowNpcNames");
            Statics.Settings.Top.ShowMobs = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "ShowMobs");
            Statics.Settings.Top.ShowMobNames = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "ShowMobNames");
            Statics.Settings.Top.ShowPcs = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "ShowPcs");
            Statics.Settings.Top.ShowPcNames = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "ShowPcNames");
            Statics.Settings.Top.ShowPets = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "ShowPets");
            Statics.Settings.Top.ShowPetNames = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "ShowPetNames");
            Statics.Settings.Top.ShowRangeCircle = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "ShowRangeCircle");

            //Load the checkboxes
            TOP_setChkBValue(c_MAP_showNpcsChkB, Statics.Settings.Top.ShowNpcs);
            TOP_setChkBValue(c_MAP_showNpcNamesChkB, Statics.Settings.Top.ShowNpcNames);
            TOP_setChkBValue(c_MAP_showMobsChkB, Statics.Settings.Top.ShowMobs);
            TOP_setChkBValue(c_MAP_showMobNamesChkB, Statics.Settings.Top.ShowMobNames);
            TOP_setChkBValue(c_MAP_showPcsChkB, Statics.Settings.Top.ShowPcs);
            TOP_setChkBValue(c_MAP_showPcNamesChkB, Statics.Settings.Top.ShowPcNames);
            TOP_setChkBValue(c_MAP_showPetsChkB, Statics.Settings.Top.ShowPets);
            TOP_setChkBValue(c_MAP_showPetNamesChkB, Statics.Settings.Top.ShowPetNames);

            m_MAP_doingInit = false;

            Statics.Settings.Top.AutoReset = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "AutoReset");
            Statics.Settings.Top.ThisZoneOnly = (bool)UserSettings.GetValue(UserSettings.BOT.TOP, "ThisZoneOnly");
            TOP_setChkBValue(StatsAutoResetChkB, Statics.Settings.Top.AutoReset);
            TOP_setChkBValue(StatsThisZoneOnlyChkB, Statics.Settings.Top.ThisZoneOnly);
        }
        private void TOP_Init_processFfxiProcChange(bool polIsValid)
        {
            LoggingFunctions.Debug("Top::processFfxiProcChange: Recieved the proc valid change: " + polIsValid + ".", LoggingFunctions.DBG_SCOPE.TOP);
        }
        private void TOP_Init_setFPtrs()
        {
            Statics.FuncPtrs.SetStatusBoxPtr = new Statics.FuncPtrs.TD_Void_String_Color(TOP_updateStatusBox);
            CB_createDelegates();
        }
        private void TOP_Init_modifyTabs()
        {
            //Make any changes to the tabs here.
            TOP_removeTabPage(c_MainTabControl, TAB_KEYS_MAIN.PL_Events_Tab.ToString());
            TOP_removeTabPage(PL_Sub_Tabs, TAB_KEYS_PL.PL_OffenseSubTab.ToString());
        }
        private void TOP_Init_setMapsPath(string iPath)
        {
            Statics.Settings.Top.MapsPath = iPath;
            UserSettings.SetValue(UserSettings.BOT.TOP, "MapsPath", Statics.Settings.Top.MapsPath);
            UserSettings.SaveSettings();
        }
        #endregion Init Utilities
        #region Set Bot Params
        private void TOP_FisherSetParams()
        {
            #region Fisher Inits
            // TBD : This should be temporary. After the fisher tab goes into the Fisher class/namespace, most of this will go away.
            Bots.FisherParam prm = new Bots.FisherParam();

            prm.StartButton = this.Start_Button;
            prm.StopButton = this.Stop_Button;
            prm.TimerLabel = this.c_TimerLabel;
            prm.StatusBox = this.c_StatusBoxTB;
            prm.StartButton = Start_Button;
            prm.StopButton = Stop_Button;
            prm.ReleaseButton = FisherReleaseButton;
            prm.TimeDateTB = TimeDateForm;
            prm.DayTB = DayForm;
            prm.MoonTB = MoonForm;
            prm.WeatherTB = WeatherForm;
            prm.ZoneLabel = ZoneForm;
            prm.LastCatchLabel = Form_Last_Catch;
            prm.FishNameLabel = FishNameForm;
            prm.CurHPLabel = CurHPForm;
            prm.FatigueLabel = FatigueForm;
            prm.Id1Label = ID1Form;
            prm.Id2Label = ID2Form;
            prm.Id3Label = ID3Form;
            prm.LargeLabel = LargeForm;
            prm.HpProgressBar = HPProgressBar;
            prm.DropListBox = DropListBox;
            prm.DropItemTB = DropItemTextBox;
            prm.DropBoxAddButton = DropBoxAddButton;
            prm.DropBoxRemoveButton = DropBoxRemoveButton;
            prm.BaitListBox = BaitBoxLB;
            prm.BaitBoxUpButton = BaitBoxUpButton;
            prm.BaitBoxDownButton = BaitBoxDownButton;
            prm.BaitBoxRefreshButton = BaitBoxRefreshButton;
            prm.FishListBox = FishListBox;
            prm.StatsListBox = StatsListBox;
            prm.RodName = RodForm;
            prm.BaitName = BaitNameForm;
            prm.BaitQuan = BaitQuanForm;
            prm.LeftArrow = LeftArrowIcon;
            prm.RightArrow = RightArrowIcon;
            prm.FishingDoneHandler = Fisher_OnDoneHandler;
            prm.FisherFatiguedHandler = Fisher_OnFatiguedHandler;
            prm.AutoResetChkB = StatsAutoResetChkB;
            prm.ThisZoneOnlyChkB = StatsThisZoneOnlyChkB;
            Fisher.Access.SetParams(prm);
            #endregion Fisher Inits
        }
        #endregion Set Bot Params
        #endregion Main Form Initialization

        #region Error Alert
        private void TOP_LogHasErrors()
        {
            try
            {
                if (c_ErrorLightPB.InvokeRequired)
                {
                    c_ErrorLightPB.Invoke(new Statics.FuncPtrs.TD_Void_Void(TOP_LogHasErrorsCBF));
                }
                else
                {
                    TOP_LogHasErrorsCBF();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("TOP_LogHasErrors: " + e.ToString());
            }
        }
        private void TOP_LogHasErrorsCBF()
        {
            Thread temp = new Thread(new ThreadStart(TOP_LogErrorsFlashThread));
            temp.IsBackground = true;
            temp.Start();
        }
        private void TOP_LogErrorsFlashThread()
        {
            //Monitor.Enter(m_TOP_TimerLabelLock);
            try
            {
                string msg = "Log has " + LoggingFunctions.NbErrors + " error";
                if (LoggingFunctions.NbErrors != 1)
                {
                    msg += "s";
                }
                msg += "!";
                TOP_setTimerLabelText(msg, TIMER_USER.ERROR_LOG);
                for (uint ii = 0; ii < LoggingFunctions.NbErrors; ii++)
                {
                    TOP_LogErrorsSetLight(true, false);
                    if (LoggingFunctions.NbErrors > 0)
                    {
                        IocaineFunctions.delay(m_TOP_LogErrorsFlashTimerShort);
                    }
                    TOP_LogErrorsSetLight(false, false);
                    if (LoggingFunctions.NbErrors == 0)
                    {
                        break;
                    }
                    IocaineFunctions.delay(m_TOP_LogErrorsFlashTimerShort);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("TOP_LogErrorsFlashThread: " + e.ToString());
            }
            finally
            {
                //Monitor.Exit(m_TOP_TimerLabelLock);
            }
        }
        private void TOP_LogErrorsSetLight(bool iLightOn, bool iDisable)
        {
            try
            {
                if (c_ErrorLightPB.InvokeRequired)
                {
                    c_ErrorLightPB.Invoke(new Statics.FuncPtrs.TD_Void_Bool_Bool(TOP_LogErrorsSetLightCBF), new object[] { iLightOn, iDisable });
                }
                else
                {
                    TOP_LogErrorsSetLightCBF(iLightOn, iDisable);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("TOP_LogErrorsSetLight: " + e.ToString());
            }
        }
        private void TOP_LogErrorsSetLightCBF(bool iLightOn, bool iDisable)
        {
            try
            {
                if (iLightOn)
                {
                    c_ErrorLightPB.Image = Iocaine2.Properties.Resources.Red_Light_On;
                }
                else
                {
                    c_ErrorLightPB.Image = Iocaine2.Properties.Resources.Red_Light_Off;
                }
                c_ErrorLightPB.Visible = !iDisable;
                c_ErrorLightPB.Refresh();
            }
            catch (Exception e)
            {
                MessageBox.Show("TOP_LogErrorsSetLightCBF: " + e.ToString());
            }
        }
        private void TOP_LogErrorsClear()
        {
            try
            {
                LoggingFunctions.NbErrors = 0;
                TOP_LogErrorsSetLight(false, true);

                TOP_setTimerLabelText("", TIMER_USER.ERROR_LOG);
                m_TOP_TimerLabelUser = TIMER_USER.NONE;
            }
            catch (Exception e)
            {
                MessageBox.Show("TOP_LogErrorsClear: " + e.ToString());
            }
        }
        private void c_ErrorLightPB_MouseClick(object sender, MouseEventArgs e)
        {
            TOP_LogErrorsClear();
            if (e.Button != MouseButtons.Left)
            {
                if (LoggingFunctions.Name != "" && File.Exists(LoggingFunctions.Name))
                {
                    System.Diagnostics.Process.Start(LoggingFunctions.Name);
                }
            }
        }
        #endregion Error Alert

        #region Timer Text
        private void TOP_setTimerLabelText(string iText, TIMER_USER iSender)
        {
            try
            {
                if (c_TimerLabel.InvokeRequired)
                {
                    c_TimerLabel.Invoke(new TD_Void_String_TIMERUSER(TOP_setTimerLabelTextCBF), new object[] { iText, iSender });
                }
                else
                {
                    TOP_setTimerLabelTextCBF(iText, iSender);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TOP_setTimerLabelText: " + e.ToString());
            }
        }
        private void TOP_setTimerLabelTextCBF(string iText, TIMER_USER iSender)
        {
            lock(m_TOP_TimerLabelLock)
            {
                if (iSender == TIMER_USER.ERROR_LOG)
                {
                    m_TOP_TimerLabelUser = iSender;
                }
                if ((m_TOP_TimerLabelUser == TIMER_USER.NONE) || (m_TOP_TimerLabelUser == iSender) || (iSender == TIMER_USER.ERROR_LOG))
                {
                    c_TimerLabel.Text = iText;
                    c_TimerLabel.Visible = true;
                    c_TimerLabel.Refresh();
                }
            }
        }
        #endregion Timer Text

        #region Chat Log Parsing
        private void TOP_Chat_startChatLogger()
        {
            if (m_TOP_replyNamesList == null)
            {
                m_TOP_replyNamesList = new ArrayList();
            }
            if (m_TOP_chatLog == null)
            {
                m_TOP_chatLog = ChatLogManager.Access.GetSynchronousLogger("ChatParser");
                ChatLogManager.Access._NewChatAvailable += new ChatLogManager.NewChatAvailable(TOP_Chat_chatParser);
                ChatLogManager.Access.Start();
            }
        }
        private void TOP_Chat_chatParser()
        {
            //Updated, now do any parsing required
            try
            {
                Monitor.Enter(m_TOP_chatLog);
                uint nbLines = m_TOP_chatLog.LinesSinceLastRead;
                for (int ii = 0; ii < nbLines; ii++)
                {
                    ChatLine iNextLine = null;
                    if (m_TOP_chatLog.Read(out iNextLine))
                    {
                        if ((iNextLine.Mode == FFXIEnums.CHAT_MODE.TELL_INC1) || (iNextLine.Mode == FFXIEnums.CHAT_MODE.TELL_INC2))
                        {
                            string incName;
                            //if the /tell is broken over multiple lines, we won't have a >> and it will crash
                            //because incName will be null.
                            if (iNextLine.ProcessedLine.Contains(">>"))
                            {
                                incName = iNextLine.ProcessedLine.Substring(0, iNextLine.ProcessedLine.IndexOf(">>"));

                                incName = TOP_Chat_checkForTimestamp(incName);

                                TOP_Chat_checkGMTellKill(incName);

                                LoggingFunctions.Debug("chatParser: Adding name " + incName + " to reply list.", LoggingFunctions.DBG_SCOPE.CHAT);
                                if (m_TOP_replyNamesList.Contains(incName))
                                {
                                    m_TOP_replyNamesList.Remove(incName);
                                }
                                m_TOP_replyNamesList.Insert(0, incName);
                                for (int kk = 0; kk < m_TOP_replyNamesList.Count; kk++)
                                {
                                    LoggingFunctions.Debug("chatParserThreadFunction: replyList[" + kk + "]: " + (string)m_TOP_replyNamesList[kk] + ".", LoggingFunctions.DBG_SCOPE.CHAT);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("chatParser: " + e.ToString());
            }
            finally
            {
                Monitor.Exit(m_TOP_chatLog);
            }
        }
        /*
         * Method uses ":" character to check if LoggingFunctions.timestamp is enabled.
         * Returns name after reducing LoggingFunctions.timestamp information.
         * @param contactName - incoming name of persone sending /tell (incName)
         * @author Justin Walega
         */
        private string TOP_Chat_checkForTimestamp(string contactName)
        {
            if (contactName.Contains(":"))
            {
                contactName = contactName.Substring(11, contactName.Length - 11);
            }
            return contactName;
        }
        /*
         * Method determines if received /tell is from a GM. If yes, kills POL process.
         * @param contactName - incoming name of persone sending /tell (incName)
         * TODO - output character information to inform player of reason for crash.
         * @author Justin Walega
         */
        private void TOP_Chat_checkGMTellKill(string contactName)
        {
            if (contactName.Contains("[GM]"))
            {
                if (Statics.Settings.Top.KillOnGmTell == true)
                {
                    IocaineFunctions.keys("/t " + contactName.Substring(4, contactName.Length - 4) + " hi");
                    IocaineFunctions.delay(5000);
                    ChangeMonitor.MainProc.Kill();
                    TOP_closeMainForm();
                }
                else if (Statics.Settings.Top.StopAllOnGmTell == true)
                {
                    TOP_stopAllBots(true);
                    IocaineFunctions.delay(10 * 1000);
                    Fisher.Access.Stop();
                }
                else if (Statics.Settings.Top.StopFisherOnGmTell == true)
                {
                    Fisher.Access.Pause(true);
                    IocaineFunctions.delay(10 * 1000);
                    Fisher.Access.Stop();
                }
            }
        }
        #endregion Chat Log Parsing
        
        #region Updater
        private void TOP_Srv_updaterThreadFunction()
        {
            LoggingFunctions.Debug("Top::updaterThreadFunction: Beginning update thread.", LoggingFunctions.DBG_SCOPE.TOP);
            ArrayList versionList = null; ;
            if (!File.Exists(m_TOP_File_updaterFile))
            {
                try
                {
                    Server.Update.UpdateFile(m_TOP_File_updaterFileName);
                }
                catch (Exception Ex)
                {
                    LoggingFunctions.Error("updaterThreadFunction: Updating Iocaine2.exe: " + Ex.ToString());
                }
            }
            else
            {
                try
                {
                    //Update the updater (Iocaine2.exe).
                    versionList = Server.Update.GetFileList(m_TOP_File_versionFileName);
                    string fileInfo = "";
                    foreach (string str in versionList)
                    {
                        if (str.Contains(m_TOP_File_updaterFileName))
                        {
                            fileInfo = str;
                            break;
                        }
                    }
                    if (Server.Update.CheckUpdateAvailable(fileInfo))
                    {
                        LoggingFunctions.Debug("Top::updaterThreadFunction: Updating " + fileInfo + ".", LoggingFunctions.DBG_SCOPE.TOP);
                        try
                        {
                            Server.Update.UpdateFile(m_TOP_File_updaterFileName);
                        }
                        catch (Exception ex)
                        {
                            LoggingFunctions.Debug("Top::updaterThreadFunction:\n\r" + ex.ToString() + ".", LoggingFunctions.DBG_SCOPE.TOP);
                        }
                    }
                    else
                    {
                        LoggingFunctions.Debug("Top::updaterThreadFunction: " + fileInfo + ": Up to date.", LoggingFunctions.DBG_SCOPE.TOP);
                    }
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("Error getting version list from server:\n\r" + e.ToString());
                }
            }

            //Now check the rest of the files.
            try
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                string version = asm.GetName().Version.ToString();
                if (versionList != null)
                {
                    LoggingFunctions.Debug("Top::updaterThreadFunction: Version list has " + versionList.Count + " items.", LoggingFunctions.DBG_SCOPE.TOP);
                }
                else
                {
                    versionList = Server.Update.GetFileList(m_TOP_File_versionFileName);
                }
                bool updateNeeded = false;
                foreach (string str in versionList)
                {
                    if (str.Contains(m_TOP_File_changeLogFileName) || str.Contains(m_TOP_File_updaterFileName))
                    {
                        continue;
                    }
                    LoggingFunctions.Debug("Top::updaterThreadFunction: Checking list item: " + str + " : if it is Main.exe.", LoggingFunctions.DBG_SCOPE.TOP);
                    if (Server.Update.CheckUpdateAvailable(str))
                    {
                        updateNeeded = true;
                        break;
                    }
                }
                bool ignoreUpdateExists = File.Exists(".\\NO_UPDATE.txt");
                bool debugFileExists = File.Exists(".\\DEBUG.txt");
                bool debugLogFileExists = File.Exists(".\\DEBUG_PRC_LOG.txt");
                if (debugFileExists)
                {
                    Iocaine2.Logging.LoggingFunctions.AddDebugScope(LoggingFunctions.DBG_SCOPE.ALL);
                }
                if (debugLogFileExists)
                {
                    m_TOP_File_keepPidLogFile = true;
                }
                if (updateNeeded && !ignoreUpdateExists)
                {
                    //Need to be able to break this cycle here.
                    //If the user isn't getting a download or the version update info isn't right,
                    //they could get stuck in a loop.
                    //Allow the user to cancel instead of updating.
                    // Or a fancier solution (where the user isn't prompted at all)
                    // could leave a retry cookie in the folder when exiting here.
                    // Then if they try to update and it's already there, bail out.
                    if (File.Exists(@".\" + m_TOP_File_updatingFileName))
                    {
                        //We've already gone through this once.
                        //Prompt the user to try updating or cancel out/exit.
                        string msg = "Iocaine has files that need to be updated,\n";
                        msg += "but the updater may be stuck in a loop.\n";
                        msg += "Click Retry to try updating.";
                        DialogResult rslt = MessageBox.Show(msg, "Update Required", MessageBoxButtons.RetryCancel);
                        if (rslt == System.Windows.Forms.DialogResult.Cancel)
                        {
                            this.Invoke(new Statics.FuncPtrs.TD_Void_Void(this.Close));
                            return;
                        }
                        //If the user didn't Cancel out, fall through to the updater spawn below.
                    }
                    string updateFileAbsolute = Path.GetFullPath(m_TOP_File_updaterFile);
                    if (File.Exists(updateFileAbsolute))
                    {
                        try
                        {
                            File.Create(m_TOP_File_updatingFile).Dispose();
                            Process Updater = new Process();
                            Updater.StartInfo.FileName = updateFileAbsolute;
                            Updater.Start();
                        }
                        catch (Exception Ex)
                        {
                            LoggingFunctions.Error("updaterThreadFunction: Starting updater process:\n\r" + Ex);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Could not find the udpater file " + m_TOP_File_updaterFileName + ". Exiting.");
                    }
                    this.Invoke(new Statics.FuncPtrs.TD_Void_Void(this.Close));
                }
                else
                {
                    if (File.Exists(m_TOP_File_updatingFile))
                    {
                        File.Delete(m_TOP_File_updatingFile);
                    }
                    Statics.Flags.VersionCheckOk = true;
                }
            }
            catch (Exception Ex)
            {
                LoggingFunctions.Timestamp("Error checking file versions: " + Ex.ToString());
            }
            LoggingFunctions.Debug("Top::updaterThreadFunction: Finished update thread.", LoggingFunctions.DBG_SCOPE.TOP);
        }
        #endregion Updater

        #region Bot Controller
        private void TOP_stopAllBots()
        {
            TOP_stopAllBots(false);
        }
        private void TOP_stopAllBots(bool iReleaseFisher)
        {
            // TBD : Eventually go into the bot controller.
            if (Fisher.Access.State == STATE.RUNNING)
            {
                Fisher.Access.Pause(iReleaseFisher);
            }
            else if (Fisher.Access.State != STATE.STOPPED)
            {
                Fisher.Access.Stop();
            }

            if (PowerLevel.Access.State != STATE.STOPPED)
            {
                Bots.PowerLevel.Access.Stop();
                updatePLStartButton("S&tart", Statics.Buttons.Green);
            }

            if (SkillUp.Access.State != STATE.STOPPED)
            {
                Bots.SkillUp.Access.Stop();
                updateSUStartButton("S&tart", Statics.Buttons.Green);
                ALR_previousStateMap[BOT.SU] = BOT_STATE.STOPPED;
            }

            if (Crafter1 != null)
            {
                Crafter1.StopCrafter();
                updateCrafterStartButton("S&tart", Statics.Buttons.Green);
                ALR_previousStateMap[BOT.CRAFTER] = BOT_STATE.STOPPED;
            }
            if (TeachersAssistant.TAState != 0)
            {
                TeachersAssistant.Stop();
                updateTAStartButton("S&tart", Statics.Buttons.Green);
            }
            if (Trader1 != null)
            {
                Trader1.StopTrader();
                updateTraderStartButton("S&tart", Statics.Buttons.Green);
            }
            if (Navigation.ProcessingStatus == Navigation.PROCESSING_STATUS.RUNNING)
            {
                Navigation.StopProcessing();
            }
            if (Seller.State != Seller.STATE.STOPPED)
            {
                Seller.Stop();
            }
            if (Buyer.State != Buyer.STATE.STOPPED)
            {
                Buyer.Stop();
            }

            Iocaine2.Synergy.Synergizer.Instance.Abort_Synergize();

        }
        private void TOP_Thread_pausePlThreads()
        {
            LoggingFunctions.Debug("Top::pausePlThreads: Pausing the PL check active thread.", LoggingFunctions.DBG_SCOPE.TOP);
            PL_CheckActiveThread.Freeze();
        }
        private void TOP_Thread_resumePlThreads()
        {
            LoggingFunctions.Debug("Top::resumePlThreads: Resuming the PL check active thread.", LoggingFunctions.DBG_SCOPE.TOP);
            PL_CheckActiveThread.Thaw();
        }
        private void TOP_vitalsOrAnyJobChangedHandler()
        {
            //The order of these is crutial since PL_RefreshLists invokes onto the GUI thread.
            //So if they were all assigned to the same event (as was previously done),
            //and the event called the PL_RefreshLists first, it would cause errors.
            try
            {
                Data.Structures.CommandManager.Init_JobChange();
                PL_RefreshLists();
            }
            catch (Exception e)
            {
                LoggingFunctions.Error(e.ToString());
            }
        }
        #region Fisher Event Handlers
        private void Fisher_OnDoneHandler()
        {
            if (Statics.Settings.Fisher.DoneNav && (Statics.Settings.Fisher.DoneNavTrip != ""))
            {
                Thread navThread = new Thread(new ParameterizedThreadStart(Fisher_DoNavAndFish));
                navThread.Name = "navThread";
                navThread.IsBackground = true;
                navThread.Start(Statics.Settings.Fisher.DoneNavTrip);
            }
        }
        private void Fisher_OnFatiguedHandler()
        {
            if (Statics.Settings.Fisher.FatiguedNav && (Statics.Settings.Fisher.FatiguedNavTrip != ""))
            {
                Thread navThread = new Thread(new ParameterizedThreadStart(Fisher_DoNavAndFish));
                navThread.Name = "navThread";
                navThread.IsBackground = true;
                navThread.Start(Statics.Settings.Fisher.FatiguedNavTrip);
            }
        }
        private void Fisher_DoNavAndFish(Object iTripName)
        {
            string tripName = (string)iTripName;
            if (!Statics.Datasets.UserTripNames.Contains(tripName))
            {
                MessageBox.Show("[ERROR] Could not find trip named '" + tripName + "'");
                LoggingFunctions.Error("Could not find trip named '" + tripName + "'");
                return;
            }
            Nav_Prc_FormatTripAndProcess(Nav_getTrip(tripName), false, "", 1, null);
            while (Bots.Navigation.ProcessingStatus != Bots.Navigation.PROCESSING_STATUS.STOPPED)
            {
                IocaineFunctions.delay(200);
            }
            //This should take us to the end of our navigation.
            //Now we'll go back to fishing.
            Start_Button.PerformClick();
        }
        #endregion Fisher Event Handlers
        #endregion Bot Controller

        #region Map
        #region Thread
        private void TOP_MAP_startMapThread()
        {
            if (m_TOP_IocThread_map == null)
            {
                m_TOP_IocThread_map = new IocaineThread("mapThread");
                m_TOP_IocThread_map.__RunMethod = TOP_MAP_threadFunction;
            }
            m_TOP_IocThread_map.Start();
        }
        private void TOP_MAP_threadFunction()
        {
            while (m_TOP_IocThread_map.__CheckState())
            {
                IocaineFunctions.delay(m_MAP_pollPeriod);
                try
                {
                    TOP_updateNpcList();
                    if (!m_MAP_mapIsPanning)
                    {
                        TOP_MAP_drawMap();
                    }
                    ALR_setCurrentPcs(m_MAP_npcList);
                }
                catch (InvalidOperationException ioe)
                {
                    LoggingFunctions.Warning("mapThreadFunction:\r\n" + ioe.ToString());
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("mapThreadFunction:\r\n" + e.ToString());
                    continue;
                }
            }
        }
        #endregion Thread
        #region Change Monitor Event Handlers
        private void TOP_changeMonitor__zoneChanged(ushort iNewZoneId, ushort iOldZoneId)
        {
            m_TOP_currentZone = iNewZoneId;
        }
        private void TOP_changeMonitor__mapChanged(bool mapSet, byte newMapId)
        {
            m_MAP_mapCurrSet = mapSet;
            if (mapSet)
            {
                m_MAP_currentMapId = newMapId;
            }
            if(!MemReads.Self.get_in_mog_house())
            {
                TOP_MAP_setMap(m_TOP_currentZone, newMapId);
            }
        }
        private void TOP_changeMonitor__inMogChanged()
        {
            if (PlayerCache.Environment.InMogHouse)
            {
                TOP_MAP_setMap(0, 0);
            }
            else
            {
                TOP_MAP_setMap(m_TOP_currentZone, m_MAP_currentMapId);
            }
        }
        #endregion Change Monitor Event Handlers
        #region Draw Map
        private void TOP_MAP_drawMap()
        {
            if (c_MAP_mapPB == null)
            {
                return;
            }
            if (c_MAP_mapPB.InvokeRequired)
            {
                c_MAP_mapPB.Invoke(new Statics.FuncPtrs.TD_Void_Void(TOP_MAP_drawMapCBF));
            }
            else
            {
                TOP_MAP_drawMapCBF();
            }
        }
        private void TOP_MAP_drawMapCBF()
        {
            if (m_MAP_MapCurrMapSet == null)
            {
                return;
            }
            //Set arrow position...
            float myX = MemReads.Self.Position.get_x();
            float myY = MemReads.Self.Position.get_y();
            float myH = MemReads.Self.Position.get_heading();
            if (m_MAP_centerLocked || MAP_CenterOnce)
            {
                m_MAP_MapCurrMapSet.CenterPosition(myX, myY);
                if (MAP_CenterOnce)
                {
                    MAP_CenterOnce = false;
                }
            }
            m_MAP_MapCurrMapSet.SetArrowValues(myX, myY, myH);
            m_MAP_MapCurrMapSet.SuppressArrow = false;

            Monitor.Enter(m_MAP_npcLock);
            try
            {
                m_MAP_MapCurrMapSet.SetNPCPositions(m_MAP_npcList);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("MAP_drawMapCBF:\r\n" + e.ToString());
            }
            finally
            {
                Monitor.Exit(m_MAP_npcLock);
            }
            m_MAP_MapCurrMapSet.AddRangeCircle(50, myX, myY);

            ushort tgtId = MemReads.Target.get_id();
            if (tgtId != 0)
            {
                MemReads.NPCs.NPCInfoStruct tgtInfo = new MemReads.NPCs.NPCInfoStruct();
                if (MemReads.NPCs.get_NPCInfoStruct(ref tgtInfo, tgtId))
                {
                    m_MAP_MapCurrMapSet.AddTarget(myX, myY, tgtInfo);
                }
            }

            c_MAP_mapPB.SizeMode = PictureBoxSizeMode.StretchImage;
            c_MAP_mapPB.Image = m_MAP_MapCurrMapSet.DynamicMap;
        }
        private void TOP_MAP_setMap(ushort iZoneId, byte iMapId)
        {
            byte mapIdx;
            float currZoom = 1;
            if (m_MAP_mapCurrMapSet != null)
            {
                currZoom = m_MAP_MapCurrMapSet.CurrentZoom;
            }
            Data.Client.Maps.GetMapIndex(iZoneId, iMapId, out mapIdx);
            m_MAP_MapCurrMapSet = Maps.GetMap(iZoneId, mapIdx);
            m_MAP_MapCurrMapSet.CommitStaticChanges();
            if (m_MAP_centerZoomLocked)
            {
                TOP_MAP_centerZoom(100);
            }
            else if (m_MAP_centerLocked)
            {
                m_MAP_MapCurrMapSet.CurrentZoom = currZoom;
                TOP_MAP_centerNoZoom();
            }
            else
            {
                MAP_CenterOnce = true;
                //MAP_MapCurrMapSet.CurrentZoom = currZoom;  //Add back to keep zoom when zoning without auto-center.
                m_MAP_MapCurrMapSet.CurrentZoom = 1;
            }
            TOP_MAP_drawMap();
        }
        private void TOP_updateNpcList()
        {
            try
            {
                Monitor.Enter(m_MAP_npcLock);
                m_MAP_npcList = MemReads.NPCs.get_NPCInfoStructList();
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TOP_updateNpcList:\r\n" + e.ToString());
            }
            finally
            {
                Monitor.Exit(m_MAP_npcLock);
            }
        }
        #endregion Draw Map
        #region Mouse
        private void c_MAP_mapPB_MouseEnter(object sender, EventArgs e)
        {
            c_MAP_mapPB.Focus();
        }
        private void c_MAP_mapPB_MouseDown(object sender, MouseEventArgs e)
        {
            m_MAP_mapIsPanning = true;
            m_MAP_mapDragStartPoint = new PointF(e.Location.X, e.Location.Y);
            m_MAP_mapDragLastRedrawPoint = new PointF(e.Location.X, e.Location.Y);
            m_MAP_mapStartCenter = m_MAP_MapCurrMapSet.CurrentCenter;
            this.Cursor = Cursors.Hand;
        }
        private void c_MAP_mapPB_MouseUp(object sender, MouseEventArgs e)
        {
            m_MAP_mapIsPanning = false;
            this.Cursor = Cursors.Default;
            m_MAP_centerLocked = false;
            m_MAP_centerZoomLocked = false;
            TOP_MAP_drawMap();
        }
        private void c_MAP_mapPB_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_MAP_mapIsPanning)
            {
                float diffX = (e.Location.X - m_MAP_mapDragStartPoint.X) / m_MAP_MapCurrMapSet.CurrentZoom;
                float diffY = (e.Location.Y - m_MAP_mapDragStartPoint.Y) / m_MAP_MapCurrMapSet.CurrentZoom;
                m_MAP_MapCurrMapSet.CurrentCenter = new PointF(m_MAP_mapStartCenter.X - diffX, m_MAP_mapStartCenter.Y - diffY);
                float dist = (float)Math.Sqrt(Math.Pow(e.X - m_MAP_mapDragLastRedrawPoint.X, 2) + Math.Pow(e.Y - m_MAP_mapDragLastRedrawPoint.Y, 2));
                if(dist > 5)
                {
                    m_MAP_mapDragLastRedrawPoint = new PointF(e.X, e.Y);
                    TOP_MAP_drawMap();
                }
            }
        }
        private void c_MAP_mapPB_MouseWheel(object sender, MouseEventArgs e)
        {
            // Location is x/y of where the mouse is relative to the 0,0 of the picturebox.
            float mouseX = (float)e.Location.X;
            float mouseY = (float)e.Location.Y;
            int zoomSwitch = e.Delta > 0 ? 1 : -1;
            // We want the same map pixel to be under the pointer once the zoom is complete.

            float currPxlCnt = 512 / m_MAP_MapCurrMapSet.CurrentZoom;
            float mapLeft = m_MAP_MapCurrMapSet.CurrentCenter.X - currPxlCnt / 2;
            float mapTop = m_MAP_MapCurrMapSet.CurrentCenter.Y - currPxlCnt / 2;
            float mapXUnderCursor = mouseX / c_MAP_mapPB.Width * currPxlCnt + mapLeft;
            float mapYUnderCursor = mouseY / c_MAP_mapPB.Height * currPxlCnt + mapTop;
            float currCursorXToCenterX = m_MAP_MapCurrMapSet.CurrentCenter.X - mapXUnderCursor;
            float currCursorYToCenterY = m_MAP_MapCurrMapSet.CurrentCenter.Y - mapYUnderCursor;

            float newPxlCnt = currPxlCnt - currPxlCnt / Maps.MapSet.PercZoomPerScroll * zoomSwitch;
            float newCenterX = mapXUnderCursor + currCursorXToCenterX * newPxlCnt / currPxlCnt;
            float newCenterY = mapYUnderCursor + currCursorYToCenterY * newPxlCnt / currPxlCnt;
            float newZoom = m_MAP_MapCurrMapSet.CurrentZoom * (1 + (zoomSwitch / Maps.MapSet.PercZoomPerScroll));
            PointF newCenter = new PointF(newCenterX, newCenterY);
            m_MAP_centerLocked = false;
            m_MAP_centerZoomLocked = false;
            m_MAP_MapCurrMapSet.SetZoom(newZoom, newCenter);
            TOP_MAP_drawMap();
        }
        #endregion Mouse
        #region Buttons
        private void c_MAP_extentsButton_Click(object sender, EventArgs e)
        {
            m_MAP_MapCurrMapSet.CurrentZoom = 1.0f;
            m_MAP_MapCurrMapSet.CurrentCenter = new PointF(256, 256);
            m_MAP_centerLocked = false;
            m_MAP_centerZoomLocked = false;
            TOP_MAP_drawMap();
        }
        private void c_MAP_lockButton_Click(object sender, EventArgs e)
        {
            TOP_MAP_centerZoom(100);
        }
        private void c_MAP_centerButton_Click(object sender, EventArgs e)
        {
            TOP_MAP_centerNoZoom();
        }
        private void TOP_MAP_centerZoom(float iPosWidth)
        {
            m_MAP_MapCurrMapSet.SetPosWidth(100);
            m_MAP_centerZoomLocked = true;
            m_MAP_centerLocked = true;
            TOP_MAP_drawMap();
        }
        private void TOP_MAP_centerNoZoom()
        {
            float myX = MemReads.Self.Position.get_x();
            float myY = MemReads.Self.Position.get_y();
            m_MAP_MapCurrMapSet.CenterPosition(myX, myY);
            m_MAP_centerLocked = true;
            TOP_MAP_drawMap();
        }
        #endregion Buttons
        #region CheckBoxes
        private void c_MAP_showNpcsChkB_CheckedChanged(object sender, EventArgs e)
        {
            Statics.Settings.Top.ShowNpcs = ((CheckBox)sender).Checked;
            if (!m_MAP_doingInit)
            {
                UserSettings.SetValue(UserSettings.BOT.TOP, "ShowNpcs", Statics.Settings.Top.ShowNpcs.ToString());
                UserSettings.SaveSettings();
            }
        }
        private void c_MAP_showNpcNamesChkB_CheckedChanged(object sender, EventArgs e)
        {
            Statics.Settings.Top.ShowNpcNames = ((CheckBox)sender).Checked;
            if (!m_MAP_doingInit)
            {
                UserSettings.SetValue(UserSettings.BOT.TOP, "ShowNpcNames", Statics.Settings.Top.ShowNpcNames.ToString());
                UserSettings.SaveSettings();
            }
        }
        private void c_MAP_showMobsChkB_CheckedChanged(object sender, EventArgs e)
        {
            Statics.Settings.Top.ShowMobs = ((CheckBox)sender).Checked;
            if (!m_MAP_doingInit)
            {
                UserSettings.SetValue(UserSettings.BOT.TOP, "ShowMobs", Statics.Settings.Top.ShowMobs.ToString());
                UserSettings.SaveSettings();
            }
        }
        private void c_MAP_showMobNamesChkB_CheckedChanged(object sender, EventArgs e)
        {
            Statics.Settings.Top.ShowMobNames = ((CheckBox)sender).Checked;
            if (!m_MAP_doingInit)
            {
                UserSettings.SetValue(UserSettings.BOT.TOP, "ShowMobNames", Statics.Settings.Top.ShowMobNames.ToString());
                UserSettings.SaveSettings();
            }
        }
        private void c_MAP_showPcsChkB_CheckedChanged(object sender, EventArgs e)
        {
            Statics.Settings.Top.ShowPcs = ((CheckBox)sender).Checked;
            if (!m_MAP_doingInit)
            {
                UserSettings.SetValue(UserSettings.BOT.TOP, "ShowPcs", Statics.Settings.Top.ShowPcs.ToString());
                UserSettings.SaveSettings();
            }
        }
        private void c_MAP_showPcNamesChkB_CheckedChanged(object sender, EventArgs e)
        {
            Statics.Settings.Top.ShowPcNames = ((CheckBox)sender).Checked;
            if (!m_MAP_doingInit)
            {
                UserSettings.SetValue(UserSettings.BOT.TOP, "ShowPcNames", Statics.Settings.Top.ShowPcNames.ToString());
                UserSettings.SaveSettings();
            }
        }
        private void c_MAP_showPetsChkB_CheckedChanged(object sender, EventArgs e)
        {
            Statics.Settings.Top.ShowPets = ((CheckBox)sender).Checked;
            if (!m_MAP_doingInit)
            {
                UserSettings.SetValue(UserSettings.BOT.TOP, "ShowPets", Statics.Settings.Top.ShowPets.ToString());
                UserSettings.SaveSettings();
            }
        }
        private void c_MAP_showPetNamesChkB_CheckedChanged(object sender, EventArgs e)
        {
            Statics.Settings.Top.ShowPetNames = ((CheckBox)sender).Checked;
            if (!m_MAP_doingInit)
            {
                UserSettings.SetValue(UserSettings.BOT.TOP, "ShowPetNames", Statics.Settings.Top.ShowPetNames.ToString());
                UserSettings.SaveSettings();
            }
        }
        #endregion CheckBoxes
        #region Form Resizing
        private void c_ExtendRightButton_Click(object sender, EventArgs e)
        {
            if (m_TOP_Form_mapExtended)
            {
                TOP_RetranctMap();
            }
            else
            {
                TOP_popOutMap();
            }
        }
        private void TOP_popOutMap()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Statics.FuncPtrs.TD_Void_Void(TOP_PopOutMapCBF));
                }
                else
                {
                    TOP_PopOutMapCBF();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TOP_PopOutMap:\r\n" + e.ToString());
            }
        }
        private void TOP_PopOutMapCBF()
        {
            if (m_TOP_Form_mapExtended)
            {
                return;
            }
            int buttonXFromRight = this.Width - c_ExtendRightButton.Left;
            int addWidth = m_TOP_Form_widthWithMap - m_TOP_Form_initialWidth;
            if (Statics.Settings.Top.EnableResizeEffects)
            {
                for (int ii = 0; ii < m_TOP_Form_resizeSteps; ii++)
                {
                    this.Width += addWidth / m_TOP_Form_resizeSteps;
                    c_ExtendRightButton.Left += addWidth / m_TOP_Form_resizeSteps;
                    IocaineFunctions.delay(m_TOP_Form_resizeDelay);
                }
            }
            this.Width = m_TOP_Form_widthWithMap;
            c_ExtendRightButton.Left = this.Width - buttonXFromRight;
            c_ExtendRightButton.BackgroundImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
            c_ExtendRightButton.BringToFront();
            m_TOP_Form_mapExtended = true;
        }
        private void TOP_RetranctMap()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Statics.FuncPtrs.TD_Void_Void(TOP_RetranctMapCBF));
                }
                else
                {
                    TOP_RetranctMapCBF();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TOP_RetranctMap:\r\n" + e.ToString());
            }
        }
        private void TOP_RetranctMapCBF()
        {
            if (!m_TOP_Form_mapExtended)
            {
                return;
            }
            int buttonXFromRight = this.Width - c_ExtendRightButton.Left;
            int addWidth = m_TOP_Form_widthWithMap - m_TOP_Form_initialWidth;
            if (Statics.Settings.Top.EnableResizeEffects)
            {
                for (int ii = 0; ii < m_TOP_Form_resizeSteps; ii++)
                {
                    this.Width -= addWidth / m_TOP_Form_resizeSteps;
                    c_ExtendRightButton.Left -= addWidth / m_TOP_Form_resizeSteps;
                    IocaineFunctions.delay(m_TOP_Form_resizeDelay);
                }
            }
            this.Width = m_TOP_Form_initialWidth;
            c_ExtendRightButton.Left = this.Width - buttonXFromRight;
            c_ExtendRightButton.BackgroundImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
            m_TOP_Form_mapExtended = false;
        }
        #endregion Form Resizing
        #endregion Map

        #region Process Combo Box
        private void TOP_changeProcessWindowTitleText(string newText)
        {
            while (!c_ProcessCB.IsHandleCreated)
            {
                LoggingFunctions.Debug("Top::changeProcessWindowTitleText: Waiting for processCB to be created.", LoggingFunctions.DBG_SCOPE.TOP);
                IocaineFunctions.delay(10);
            }
            try
            {
                if (c_ProcessCB.InvokeRequired)
                {
                    c_ProcessCB.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_changeProcessWindowTitleTextCBF), new object[] { newText });
                }
                else
                {
                    TOP_changeProcessWindowTitleTextCBF(newText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting process combo box item text: " + e.ToString());
            }
        }
        private void TOP_changeProcessWindowTitleTextCBF(string newTitle)
        {
            c_ProcessCB.Text = newTitle + " (" + ChangeMonitor.MainProc.Id + ")";
        }
        #endregion Process Combo Box
        
        #region Tab Pages
        private void TOP_addTabPage(TabControl iControlToAddTo, int iIndex, TabPage iPageToAdd)
        {
            try
            {
                if (iControlToAddTo.InvokeRequired)
                {
                    iControlToAddTo.Invoke(new Statics.FuncPtrs.TD_Void_TabControl_Int32_TabPage(TOP_addTabPageCBF), new object[] { iControlToAddTo, iIndex, iPageToAdd });
                }
                else
                {
                    TOP_addTabPageCBF(iControlToAddTo, iIndex, iPageToAdd);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In addTabPage: " + e.ToString());
            }
        }
        private void TOP_addTabPageCBF(TabControl iControlToAddTo, int iIndex, TabPage iPageToAdd)
        {
            if (iControlToAddTo.TabPages.Count <= iIndex)
            {
                iControlToAddTo.TabPages.Add(iPageToAdd);
            }
            else
            {
                //This is stupid, but for the insert method to work, the tab control's handle must first be created.
                //http:// stackoverflow.com/questions/1532301/visual-studio-tabcontrol-tabpages-insert-not-working
                IntPtr hndl = iControlToAddTo.Handle;
                iControlToAddTo.TabPages.Insert(iIndex, iPageToAdd);
            }
            iControlToAddTo.SelectedTab = iControlToAddTo.TabPages[0];
        }
        private void TOP_removeTabPage(TabControl iControlToRemoveFrom, TabPage iPageToRemove)
        {
            try
            {
                if (iControlToRemoveFrom.InvokeRequired)
                {
                    iControlToRemoveFrom.Invoke(new Statics.FuncPtrs.TD_Void_TabControl_TabPage(TOP_removeTabPageCBF), new object[] { iControlToRemoveFrom, iPageToRemove });
                }
                else
                {
                    TOP_removeTabPageCBF(iControlToRemoveFrom, iPageToRemove);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In removeTabPage: " + e.ToString());
            }
        }
        private void TOP_removeTabPageCBF(TabControl iControlToRemoveFrom, TabPage iPageToRemove)
        {
            if (iControlToRemoveFrom.TabPages.Contains(iPageToRemove))
            {
                iControlToRemoveFrom.TabPages.Remove(iPageToRemove);
            }
        }
        private void TOP_removeTabPage(TabControl iControl, string iKey)
        {
            try
            {
                if (iControl.InvokeRequired)
                {
                    iControl.Invoke(new Statics.FuncPtrs.TD_Void_TabControl_String(TOP_removeTabPageCBF), new object[] { iControl, iKey });
                }
                else
                {
                    TOP_removeTabPageCBF(iControl, iKey);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("TOP_RemoveTab:\r\t" + e.ToString());
            }
        }
        private void TOP_removeTabPageCBF(TabControl iControl, string iKey)
        {
            if (iControl.TabPages.ContainsKey(iKey))
            {
                iControl.TabPages.RemoveByKey(iKey);
            }
        }
        private int TOP_getMainTabSelectionIndexCBF()
        {
            return this.c_MainTabControl.SelectedIndex;
        }
        private string TOP_getMainTabSelectionName()
        {
            try
            {
                if (c_MainTabControl.InvokeRequired)
                {
                    return (string)c_MainTabControl.Invoke(new Statics.FuncPtrs.TD_String_Void(TOP_getMainTabSelectionNameCBF));
                }
                else
                {
                    return TOP_getMainTabSelectionNameCBF();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("GetMainTabSelectedName:\r\n" + e.ToString());
                return "";
            }
        }
        private string TOP_getMainTabSelectionNameCBF()
        {
            return this.c_MainTabControl.SelectedTab.Name;
        }
        #endregion Tab Pages

        #region Status Box
        private void TOP_updateStatusBox(string newTxt, System.Drawing.Color color)
        {
            try
            {
                if (c_StatusBoxTB == null)
                {
                    return;
                }
                while (!c_StatusBoxTB.IsHandleCreated)
                {
                    LoggingFunctions.Debug("Top::updateStatusBox: Waiting for status box to be created.", LoggingFunctions.DBG_SCOPE.TOP);
                    IocaineFunctions.delay(10);
                }
                if (c_StatusBoxTB.InvokeRequired)
                {
                    c_StatusBoxTB.Invoke(new Statics.FuncPtrs.TD_Void_String_Color(TOP_updateStatusBoxCBF), new object[] { newTxt, color });
                }
                else
                {
                    TOP_updateStatusBoxCBF(newTxt, color);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating status box: " + e.ToString());
            }
        }
        private void TOP_updateStatusBoxCBF(string newTxt, System.Drawing.Color boxColor)
        {
            c_StatusBoxTB.BackColor = boxColor;
            c_StatusBoxTB.Text = newTxt;
        }
        #endregion Status Box

        #region Buttons
        #region Settings Button
        private void c_SettingsButton_Click(object sender, EventArgs e)
        {
            string tabName = c_MainTabControl.SelectedTab.Name;
            if (tabName == TAB_KEYS_MAIN.Fish_Bot_Tab.ToString())
            {
                Fisher.Access.ShowSettingsDialog(this);
                return;
            }
            else if (tabName == TAB_KEYS_MAIN.PL_Bot_Tab.ToString())
            {
                if (ChangeMonitor.LoggedIn == true)
                {
                    PL_Settings_Form PLSettingsForm = new PL_Settings_Form(this.Location.X + (this.Size.Width / 2), this.Location.Y + (this.Size.Height / 2));
                    PLSettingsForm.Apply_Button.Click += new EventHandler(PL_Settings_Apply_Button_Click);
                    PLSettingsForm.OK_Button.Click += new EventHandler(PL_Settings_OK_Button_Click);
                    PLSettingsForm.Show();
                }
                else
                {
                    MessageBox.Show("Must be logged in to modify settings.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                return;
            }
            else if (tabName == TAB_KEYS_MAIN.Nav_Tab.ToString())
            {
                if (ChangeMonitor.LoggedIn == true)
                {
                    Nav_Settings_Form NAVSettingsForm = new Nav_Settings_Form(this.Location.X + (this.Size.Width / 2), this.Location.Y + (this.Size.Height / 2));
                    NAVSettingsForm.Apply_Button.Click += new EventHandler(Nav_settingsFormOkClick);
                    NAVSettingsForm.OK_Button.Click += new EventHandler(Nav_settingsFormOkClick);
                    NAVSettingsForm.Show();
                }
                else
                {
                    MessageBox.Show("Must be logged in to modify settings.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                return;
            }
            else if (tabName == TAB_KEYS_MAIN.WMS_Tab.ToString())
            {
                WMS_Settings_Form WMSSettingsForm = new WMS_Settings_Form(this.Location.X + (this.Size.Width / 2), this.Location.Y + (this.Size.Height / 2), WMS_dataset);
                WMSSettingsForm.Show();
                WMSSettingsForm.Disposed += new EventHandler(WMSSettingsForm_Disposed);
                return;
            }
            else if (tabName == TAB_KEYS_MAIN.About_Tab.ToString())
            {
                if (ChangeMonitor.LoggedIn == true)
                {
                    Top_Settings_Form TopSettingsForm = new Top_Settings_Form(this.Location.X + (this.Size.Width / 2), this.Location.Y + (this.Size.Height / 2));
                    TopSettingsForm.Show();
                }
                else
                {
                    MessageBox.Show("Must be logged in to modify settings.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                return;
            }
            else if (tabName == TAB_KEYS_MAIN.Helpers_Tab.ToString())
            {
                if (ChangeMonitor.LoggedIn == true)
                {
                    Helpers_Settings_Form HelpersSettingsForm = new Helpers_Settings_Form(this.Location.X + (this.Size.Width / 2), this.Location.Y + (this.Size.Height / 2));
                    HelpersSettingsForm.Show();
                }
                else
                {
                    MessageBox.Show("Must be logged in to modify settings.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                return;
            }
        }
        #endregion Settings Button
        #region Stats Button
        private void c_StatsButton_Click(object sender, EventArgs e)
        {
            string tabName = c_MainTabControl.SelectedTab.Name;
            if (tabName == TAB_KEYS_MAIN.Fish_Bot_Tab.ToString())
            {
                //MessageBox.Show("Fish Stats is currrently broken.\nNo ETA on fixing it.\nSorry for the inconvenience.", "Broken :(", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                Cursor.Current = Cursors.WaitCursor;
                Stats_Form StatsForm = new Stats_Form(BaitFilterList, RodFilterList, FSFishCBIndex, FSZoneCBIndex,
                                                      FSZoneRodCBIndex, FSZoneBaitCBIndex, FSRodCBIndex, FSBaitCBIndex);
                StatsForm.SettingsChanged += new Stats_Form.SettingsChangedHandler(UpdateStatsSettings);
                StatsForm.Show();
                StatsForm.UpdateSettings();
                Statics.Settings.Top.FlashFishStatsButton = false;
                UserSettings.SetValue(UserSettings.BOT.TOP, "FlashFishStatsButton", "false");
                return;
            }
        }
        private void TOP_setStatsButtonColorCBF(Color iColor)
        {
            c_StatsButton.BackColor = iColor;
        }
        #endregion Stats Button
        #region Panic Button
        private void c_PanicButton_Click(object sender, EventArgs e)
        {
            TOP_stopAllBots(true);
            Navigation.ReleaseAllMovementKeys(true);
            Navigation.ReleaseAllMovementKeys(false);
            Navigation.ReleaseAllCameraKeys(true);
            Navigation.ReleaseAllCameraKeys(false);
            Navigation.ReleaseControlKeys();
        }
        #endregion Panic Button
        #region Command Entry
        private void c_ChatButton_Click(object sender, EventArgs e)
        {
            if ((ChangeMonitor.MainProc != null) && (m_TOP_replyNamesList != null))
            {
                CommandLine cmdLine = new CommandLine(ChangeMonitor.MainProc, m_TOP_replyNamesList, 100, CommandLine.MODE.RAISE_EVENT);
                cmdLine.Location = new Point(this.Location.X + 25, this.Location.Y + 300);
                cmdLine.CommandSent += new CommandLine.CommandSentEvent(cmdLine_CommandSent);
                cmdLine.Show();
            }
        }
        private void cmdLine_CommandSent(object sender, CommandSentEventArgs e)
        {
            IocaineFunctions.keys(e.CommandSent);
        }
        #endregion Command Entry
        #endregion Buttons

        #region Check Boxes
        #region Event Handlers
        #endregion Event Handlers
        #region Updates
        private void TOP_setChkBValue(CheckBox iChkB, bool iValue)
        {
            try
            {
                if (iChkB.InvokeRequired)
                {
                    iChkB.Invoke(new Statics.FuncPtrs.TD_Void_ChkB_Boolean(TOP_setChkBValueCBF), new object[] { iChkB, iValue });
                }
                else
                {
                    TOP_setChkBValueCBF(iChkB, iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error(e.ToString());
            }
        }
        private void TOP_setChkBValueCBF(CheckBox iChkB, bool iValue)
        {
            iChkB.Checked = iValue;
        }
        #endregion Updates
        #endregion Check Boxes

        #region Inventory Labels
        #region Update All
        private void TOP_setTopInvCurLabelText()
        {
            TOP_setTopInvBagCurLabelText(Inventory.Containers.Bag.Occupancy.ToString());
            TOP_setTopInvSatchelCurLabelText(Inventory.Containers.Satchel.Occupancy.ToString());
            TOP_setTopInvSackCurLabelText(Inventory.Containers.Sack.Occupancy.ToString());
            TOP_setTopInvCaseCurLabelText(Inventory.Containers.MCase.Occupancy.ToString());
            TOP_setTopInvSafeCurLabelText(Inventory.Containers.Safe.Occupancy.ToString());
            TOP_setTopInvStorageCurLabelText(Inventory.Containers.Storage.Occupancy.ToString());
            TOP_setTopInvLockerCurLabelText(Inventory.Containers.Locker.Occupancy.ToString());
        }
        private void TOP_setTopInvMaxLabelText()
        {
            TOP_setTopInvBagMaxLabelText("/ " + Inventory.Containers.Bag.Capacity.ToString());
            TOP_setTopInvSatchelMaxLabelText("/ " + Inventory.Containers.Satchel.Capacity.ToString());
            TOP_setTopInvSackMaxLabelText("/ " + Inventory.Containers.Sack.Capacity.ToString());
            TOP_setTopInvCaseMaxLabelText("/ " + Inventory.Containers.MCase.Capacity.ToString());
            TOP_setTopInvSafeMaxLabelText("/ " + Inventory.Containers.Safe.Capacity.ToString());
            TOP_setTopInvStorageMaxLabelText("/ " + Inventory.Containers.Storage.Capacity.ToString());
            TOP_setTopInvLockerMaxLabelText("/ " + Inventory.Containers.Locker.Capacity.ToString());
        }
        #endregion Update All
        #region Bag
        private void TOP_setTopInvBagCurLabelText(string iText)
        {
            try
            {
                if (c_InvBagCurLabel.InvokeRequired)
                {
                    c_InvBagCurLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_setTopInvBagCurLabelTextCBF), new object[] { iText });
                }
                else
                {
                    TOP_setTopInvBagCurLabelTextCBF(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In setTopInvBagCurLabelText: " + e.ToString());
            }
        }
        private void TOP_setTopInvBagCurLabelTextCBF(string iText)
        {
            c_InvBagCurLabel.Text = iText;
        }
        private void TOP_setTopInvBagMaxLabelText(string iText)
        {
            try
            {
                if (c_InvBagMaxLabel.InvokeRequired)
                {
                    c_InvBagMaxLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_setTopInvBagMaxLabelTextCBF), new object[] { iText });
                }
                else
                {
                    TOP_setTopInvBagMaxLabelTextCBF(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In setTopInvBagMaxLabelText: " + e.ToString());
            }
        }
        private void TOP_setTopInvBagMaxLabelTextCBF(string iText)
        {
            c_InvBagMaxLabel.Text = iText;
        }
        #endregion Bag
        #region Satchel
        private void TOP_setTopInvSatchelCurLabelText(string iText)
        {
            try
            {
                if (c_InvSatchelCurLabel.InvokeRequired)
                {
                    c_InvSatchelCurLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_setTopInvSatchelCurLabelTextCBF), new object[] { iText });
                }
                else
                {
                    TOP_setTopInvSatchelCurLabelTextCBF(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In setTopInvSatchelCurLabelText: " + e.ToString());
            }
        }
        private void TOP_setTopInvSatchelCurLabelTextCBF(string iText)
        {
            c_InvSatchelCurLabel.Text = iText;
        }
        private void TOP_setTopInvSatchelMaxLabelText(string iText)
        {
            try
            {
                if (c_InvSatchelMaxLabel.InvokeRequired)
                {
                    c_InvSatchelMaxLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_setTopInvSatchelMaxLabelTextCBF), new object[] { iText });
                }
                else
                {
                    TOP_setTopInvSatchelMaxLabelTextCBF(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In setTopInvSatchelMaxLabelText: " + e.ToString());
            }
        }
        private void TOP_setTopInvSatchelMaxLabelTextCBF(string iText)
        {
            c_InvSatchelMaxLabel.Text = iText;
        }
        #endregion Satchel
        #region Sack
        private void TOP_setTopInvSackCurLabelText(string iText)
        {
            try
            {
                if (c_InvSackCurLabel.InvokeRequired)
                {
                    c_InvSackCurLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_setTopInvSackCurLabelTextCBF), new object[] { iText });
                }
                else
                {
                    TOP_setTopInvSackCurLabelTextCBF(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In setTopInvSackCurLabelText: " + e.ToString());
            }
        }
        private void TOP_setTopInvSackCurLabelTextCBF(string iText)
        {
            c_InvSackCurLabel.Text = iText;
        }
        private void TOP_setTopInvSackMaxLabelText(string iText)
        {
            try
            {
                if (c_InvSackMaxLabel.InvokeRequired)
                {
                    c_InvSackMaxLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_setTopInvSackMaxLabelTextCBF), new object[] { iText });
                }
                else
                {
                    TOP_setTopInvSackMaxLabelTextCBF(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In setTopInvSackMaxLabelText: " + e.ToString());
            }
        }
        private void TOP_setTopInvSackMaxLabelTextCBF(string iText)
        {
            c_InvSackMaxLabel.Text = iText;
        }
        #endregion Sack
        #region Case
        private void TOP_setTopInvCaseCurLabelText(string iText)
        {
            try
            {
                if (c_InvCaseCurLabel.InvokeRequired)
                {
                    c_InvCaseCurLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_setTopInvCaseCurLabelTextCBF), new object[] { iText });
                }
                else
                {
                    TOP_setTopInvCaseCurLabelTextCBF(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In setTopInvCaseCurLabelText: " + e.ToString());
            }
        }
        private void TOP_setTopInvCaseCurLabelTextCBF(string iText)
        {
            c_InvCaseCurLabel.Text = iText;
        }
        private void TOP_setTopInvCaseMaxLabelText(string iText)
        {
            try
            {
                if (c_InvCaseMaxLabel.InvokeRequired)
                {
                    c_InvCaseMaxLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_setTopInvCaseMaxLabelTextCBF), new object[] { iText });
                }
                else
                {
                    TOP_setTopInvCaseMaxLabelTextCBF(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In setTopInvCaseMaxLabelText: " + e.ToString());
            }
        }
        private void TOP_setTopInvCaseMaxLabelTextCBF(string iText)
        {
            c_InvCaseMaxLabel.Text = iText;
        }
        #endregion Case
        #region Safe
        private void TOP_setTopInvSafeCurLabelText(string iText)
        {
            try
            {
                if (c_InvSafeCurLabel.InvokeRequired)
                {
                    c_InvSafeCurLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_setTopInvSafeCurLabelTextCBF), new object[] { iText });
                }
                else
                {
                    TOP_setTopInvSafeCurLabelTextCBF(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In setTopInvSafeCurLabelText: " + e.ToString());
            }
        }
        private void TOP_setTopInvSafeCurLabelTextCBF(string iText)
        {
            c_InvSafeCurLabel.Text = iText;
        }
        private void TOP_setTopInvSafeMaxLabelText(string iText)
        {
            try
            {
                if (c_InvSafeMaxLabel.InvokeRequired)
                {
                    c_InvSafeMaxLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_setTopInvSafeMaxLabelTextCBF), new object[] { iText });
                }
                else
                {
                    TOP_setTopInvSafeMaxLabelTextCBF(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In setTopInvSafeMaxLabelText: " + e.ToString());
            }
        }
        private void TOP_setTopInvSafeMaxLabelTextCBF(string iText)
        {
            c_InvSafeMaxLabel.Text = iText;
        }
        #endregion Safe
        #region Storage
        private void TOP_setTopInvStorageCurLabelText(string iText)
        {
            try
            {
                if (c_InvStorageCurLabel.InvokeRequired)
                {
                    c_InvStorageCurLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_setTopInvStorageCurLabelTextCBF), new object[] { iText });
                }
                else
                {
                    TOP_setTopInvStorageCurLabelTextCBF(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In setTopInvStorageCurLabelText: " + e.ToString());
            }
        }
        private void TOP_setTopInvStorageCurLabelTextCBF(string iText)
        {
            c_InvStorageCurLabel.Text = iText;
        }
        private void TOP_setTopInvStorageMaxLabelText(string iText)
        {
            try
            {
                if (c_InvStorageMaxLabel.InvokeRequired)
                {
                    c_InvStorageMaxLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_setTopInvStorageMaxLabelTextCBF), new object[] { iText });
                }
                else
                {
                    TOP_setTopInvStorageMaxLabelTextCBF(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In setTopInvStorageMaxLabelText: " + e.ToString());
            }
        }
        private void TOP_setTopInvStorageMaxLabelTextCBF(string iText)
        {
            c_InvStorageMaxLabel.Text = iText;
        }
        #endregion Storage
        #region Locker
        private void TOP_setTopInvLockerCurLabelText(string iText)
        {
            try
            {
                if (c_InvLockerCurLabel.InvokeRequired)
                {
                    c_InvLockerCurLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_setTopInvLockerCurLabelTextCBF), new object[] { iText });
                }
                else
                {
                    TOP_setTopInvLockerCurLabelTextCBF(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In setTopInvLockerCurLabelText: " + e.ToString());
            }
        }
        private void TOP_setTopInvLockerCurLabelTextCBF(string iText)
        {
            c_InvLockerCurLabel.Text = iText;
        }
        private void TOP_setTopInvLockerMaxLabelText(string iText)
        {
            try
            {
                if (c_InvLockerMaxLabel.InvokeRequired)
                {
                    c_InvLockerMaxLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_setTopInvLockerMaxLabelTextCBF), new object[] { iText });
                }
                else
                {
                    TOP_setTopInvLockerMaxLabelTextCBF(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In setTopInvLockerMaxLabelText: " + e.ToString());
            }
        }
        private void TOP_setTopInvLockerMaxLabelTextCBF(string iText)
        {
            c_InvLockerMaxLabel.Text = iText;
        }
        #endregion Locker
        #endregion Inventory Labels

        #region Form Resizing (Depricated/Unused)
        private void Iocaine_2_Form_ResizeEnd(object sender, EventArgs e)
        {
            m_TOP_Form_previousWidth = m_TOP_Form_currentWidth;
            m_TOP_Form_previousHeight = m_TOP_Form_currentHeight;
            m_TOP_Form_previousCenterX = m_TOP_Form_currentCenterX;
            m_TOP_Form_previousCenterY = m_TOP_Form_currentCenterY;
            m_TOP_Form_currentWidth = this.Size.Width;
            m_TOP_Form_currentHeight = this.Size.Height;
            m_TOP_Form_currentCenterX = m_TOP_Form_currentWidth / 2;
            m_TOP_Form_currentCenterY = m_TOP_Form_currentHeight / 2;

            TOP_Form_ResizeEnd(sender, e);
            Fisher_Tab_ResizeEnd(sender, e);
            PL_Form_ResizeEnd(sender, e);
            NAV_Form_ResizeEnd(sender, e);
            WMS_Form_ResizeEnd(sender, e);
        }
        private void TOP_Form_ResizeEnd(object sender, EventArgs e)
        {
            int top_tab_x_diff = 549 - 525;
            int top_tab_y_diff = 535 - 432;
            c_MainTabControl.Width = m_TOP_Form_currentWidth - top_tab_x_diff + 10;
            c_MainTabControl.Height = m_TOP_Form_currentHeight - top_tab_y_diff;

            #region Shift Inventory Totals
            const int nb_label_groups = 7;
            const int bag_label_size = 25;
            const int bag_occ_label_size = 17;
            const int bag_max_label_size = 27;
            const int bag_group_size = bag_label_size + bag_occ_label_size + bag_max_label_size;
            const int satchel_label_size = 27;
            const int satchel_occ_label_size = 17;
            const int satchel_max_label_size = 27;
            const int satchel_group_size = satchel_label_size + satchel_occ_label_size + satchel_max_label_size;
            const int sack_label_size = 30;
            const int sack_occ_label_size = 17;
            const int sack_max_label_size = 27;
            const int sack_group_size = sack_label_size + sack_occ_label_size + sack_max_label_size;
            const int case_label_size = 29;
            const int case_occ_label_size = 17;
            const int case_max_label_size = 27;
            const int case_group_size = case_label_size + case_occ_label_size + case_max_label_size;
            const int safe_label_size = 27;
            const int safe_occ_label_size = 17;
            const int safe_max_label_size = 27;
            const int safe_group_size = safe_label_size + safe_occ_label_size + safe_max_label_size;
            const int storage_label_size = 24;
            const int storage_occ_label_size = 17;
            const int storage_max_label_size = 27;
            const int storage_group_size = storage_label_size + storage_occ_label_size + storage_max_label_size;
            const int locker_label_size = 27;
            const int locker_occ_label_size = 17;
            const int locker_max_label_size = 27;
            const int locker_group_size = locker_label_size + locker_occ_label_size + locker_max_label_size;
            const int left_buffer = 4;
            const int right_buffer = 4;
            const int inv_label_top_diff = 549 - 483;
            int inv_label_top = m_TOP_Form_currentHeight - inv_label_top_diff + 14;
            int between_buffer = 0;

            int inv_label_total_x = bag_group_size + satchel_group_size + sack_group_size + case_group_size + safe_group_size + storage_group_size + locker_group_size;
            int inv_label_x_remaining = m_TOP_Form_currentWidth - left_buffer - right_buffer - inv_label_total_x;
            if (inv_label_x_remaining <= 0)
            {
                between_buffer = 0;
            }
            else
            {
                between_buffer = (int)(inv_label_x_remaining / (nb_label_groups - 1));
                for (int ii = 0; ii < 2; ii++)
                {
                    if (between_buffer >= 1)
                    {
                        between_buffer--;
                    }
                }
            }

            int current_x = left_buffer;
            c_InvBagLabel.Top = inv_label_top;
            c_InvBagCurLabel.Top = inv_label_top;
            c_InvBagMaxLabel.Top = inv_label_top;

            current_x += bag_group_size + between_buffer;
            c_InvSatchelLabel.Top = inv_label_top;
            c_InvSatchelLabel.Left = current_x;
            current_x += satchel_label_size;
            c_InvSatchelCurLabel.Top = inv_label_top;
            c_InvSatchelCurLabel.Left = current_x;
            current_x += satchel_occ_label_size;
            c_InvSatchelMaxLabel.Top = inv_label_top;
            c_InvSatchelMaxLabel.Left = current_x;
            current_x += satchel_max_label_size + between_buffer;
            c_InvSackLabel.Top = inv_label_top;
            c_InvSackLabel.Left = current_x;
            current_x += sack_label_size;
            c_InvSackCurLabel.Top = inv_label_top;
            c_InvSackCurLabel.Left = current_x;
            current_x += sack_occ_label_size;
            c_InvSackMaxLabel.Top = inv_label_top;
            c_InvSackMaxLabel.Left = current_x;
            current_x += sack_max_label_size + between_buffer;
            c_InvCaseLabel.Top = inv_label_top;
            c_InvCaseLabel.Left = current_x;
            current_x += case_label_size;
            c_InvCaseCurLabel.Top = inv_label_top;
            c_InvCaseCurLabel.Left = current_x;
            current_x += case_occ_label_size;
            c_InvCaseMaxLabel.Top = inv_label_top;
            c_InvCaseMaxLabel.Left = current_x;
            current_x += case_max_label_size + between_buffer;
            c_InvSafeLabel.Top = inv_label_top;
            c_InvSafeLabel.Left = current_x;
            current_x += safe_label_size;
            c_InvSafeCurLabel.Top = inv_label_top;
            c_InvSafeCurLabel.Left = current_x;
            current_x += safe_occ_label_size;
            c_InvSafeMaxLabel.Top = inv_label_top;
            c_InvSafeMaxLabel.Left = current_x;
            current_x += safe_max_label_size + between_buffer;
            c_InvStorageLabel.Top = inv_label_top;
            c_InvStorageLabel.Left = current_x;
            current_x += storage_label_size;
            c_InvStorageCurLabel.Top = inv_label_top;
            c_InvStorageCurLabel.Left = current_x;
            current_x += storage_occ_label_size;
            c_InvStorageMaxLabel.Top = inv_label_top;
            c_InvStorageMaxLabel.Left = current_x;
            current_x += storage_max_label_size + between_buffer;
            c_InvLockerLabel.Top = inv_label_top;
            c_InvLockerLabel.Left = current_x;
            current_x += locker_label_size;
            c_InvLockerCurLabel.Top = inv_label_top;
            c_InvLockerCurLabel.Left = current_x;
            current_x += locker_occ_label_size;
            c_InvLockerMaxLabel.Top = inv_label_top;
            c_InvLockerMaxLabel.Left = current_x;
            current_x += locker_max_label_size + between_buffer;
            #endregion Shift Inventory Totals
        }
        private void Fisher_Tab_ResizeEnd(object sender, EventArgs e)
        {
            //Move the centered, constant sized items
            Start_Button.Left = m_TOP_Form_currentCenterX - (int)(Start_Button.Width / 2);
            FisherReleaseButton.Left = m_TOP_Form_currentCenterX - (int)(Start_Button.Width / 2);
            Stop_Button.Left = m_TOP_Form_currentCenterX - (int)(Start_Button.Width / 2);
            TimeDateForm.Left = m_TOP_Form_currentCenterX - (int)(Start_Button.Width / 2) - (TimeDateForm.Width / 2) + 37;
            DayForm.Left = m_TOP_Form_currentCenterX - (int)(Start_Button.Width / 2) - (DayForm.Width / 2) + 37;
            MoonForm.Left = m_TOP_Form_currentCenterX - (int)(Start_Button.Width / 2) - (MoonForm.Width / 2) + 37;
            WeatherForm.Left = m_TOP_Form_currentCenterX - (int)(Start_Button.Width / 2) - (WeatherForm.Width / 2) + 37;

            //Stretch the left controls
            const int initial_start_button_left = 232;
            const int initial_catch_box_width = 205;
            //const int initial_catch_box_height = 194;
            //catch box left = 10, start button left 232 => new width = start button left - 232 + 205
            FishListBox.Width = Start_Button.Left - initial_start_button_left + initial_catch_box_width;

            //bait and drop boxes are 100 wide with 5 buffer between.
            //So new widths are fish list box width - 5 / 2.
            DropListBox.Width = (FishListBox.Width - 5) / 2;
            BaitBoxLB.Width = DropListBox.Width;
            BaitBoxLB.Left = 10 + 5 + DropListBox.Width;
            DropItemTextBox.Width = DropListBox.Width;
            BaitBoxUpButton.Left = BaitBoxLB.Left;
            BaitBoxRefreshButton.Left = BaitBoxUpButton.Left + 38;
            BaitBoxDownButton.Left = BaitBoxRefreshButton.Left + 27;
            //Original tab height is 403.
            //Original fish box height is 194
            //Original drop box height is 109
            //Original bait box height is 139
            //Fish box top buffer is 19
            //Bottom of drop box is 323
            //232 - 19 - 194 = space between bottom of fish box and top of drop box = 19
            //194 / (194 + 139) ~ .58 => keep that proportion for the fish box.
            //When resized, get the difference between bottom of bait box and top of fish box.
            //Subtract 19 to get total box size.  Distribute ~.58 to fish box, ~.42 to bait box.
            int total_box_height = BaitBoxUpButton.Top - 3 - FishListBox.Top - 19;
            FishListBox.Height = (int)(total_box_height * 0.58);
            BaitBoxLB.Height = (int)(total_box_height * 0.42);
            DropListBox.Height = BaitBoxLB.Height - 30;
            DropListBox.Top = FishListBox.Top + FishListBox.Height + 19;
            BaitBoxLB.Top = DropListBox.Top;
            DropBoxLabel.Top = DropListBox.Top - 16;
            BaitBoxLabel.Top = BaitBoxLB.Top - 16;

            InfoBoxPanel.Width = InfoBoxPanel.Width + (InfoBoxPanel.Left - Start_Button.Left - 91);
            //Info box left = start button left + 91
            InfoBoxPanel.Left = Start_Button.Left + 91;
            InfoBoxLabel.Left = InfoBoxPanel.Left + 7;

            //StatsBoxLabel.Left = Start_Button.Left - 8;
            StatsListBox.Width = StatsListBox.Width + (StatsListBox.Left - Start_Button.Left + 5);
            StatsListBox.Left = Start_Button.Left - 5;
            //229 / 19
            StatsListBox.Height = StatsListBox.Height + (StatsListBox.Top - InfoBoxPanel.Top - 210);
            StatsListBox.Top = InfoBoxPanel.Top + 221;
            StatsBoxLabel.Left = StatsListBox.Left - 3;
            StatsBoxLabel.Top = StatsListBox.Top - 15;
        }
        #endregion Form Resizing (Depricated/Unused)

        #region Close Form
        private void TOP_closeMainForm()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Statics.FuncPtrs.TD_Void_Void(TOP_closeMainFormCBF));
                }
                else
                {
                    TOP_closeMainFormCBF();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In closeMainForm: " + e.ToString());
            }

        }
        private void TOP_closeMainFormCBF()
        {
            this.Close();
        }
        private void Iocaine_2_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ChangeMonitor.LoggedIn)
            {
                UserSettings.SaveSettings();
                WMS_writeDatasetToXML();
            }
            POS_ClearSpeed();
            TOP_Log_closeLogFile();
        }
        private static void TOP_Log_closeLogFile()
        {
            LoggingFunctions.CloseLogFile();
            //Delete process instance log file if we're not in LoggingFunctions.debug mode.
            if (m_TOP_File_keepPidLogFile == false)
            {
                LoggingFunctions.Debug("Top::closeLogFile: LoggingFunctions.debug was 0, clearing old log file. This should never get printed.", LoggingFunctions.DBG_SCOPE.TOP);
                string fileName = ".\\Iocaine_Log_" + Process.GetCurrentProcess().Id.ToString() + ".txt";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }
        #endregion Close Form

        #region Top Form Utilities
        #region Title Bar
        private void TOP_updateTitlebarText()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string version = asm.GetName().Version.Major.ToString() + "." + asm.GetName().Version.Minor.ToString();
            m_TOP_iocaineTitleBarText = "Iocaine " + version;
            if (this.InvokeRequired)
            {
                this.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_updateTitlebarTextCBF), new object[] { m_TOP_iocaineTitleBarText });
            }
            else
            {
                TOP_updateTitlebarTextCBF(m_TOP_iocaineTitleBarText);
            }
        }
        private void TOP_updateTitlebarText(bool iLoggedIn, string iPlayerName)
        {
            string text = "";
            if (iLoggedIn)
            {
                text = iPlayerName + " - " + m_TOP_iocaineTitleBarText;
            }
            else
            {
                text = m_TOP_iocaineTitleBarText;
            }
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Statics.FuncPtrs.TD_Void_String(TOP_updateTitlebarTextCBF), new object[] { text });
                }
                else
                {
                    TOP_updateTitlebarTextCBF(text);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In updateMainWindowText: " + e.ToString());
            }
        }
        private void TOP_updateTitlebarTextCBF(string iText)
        {
            this.Text = iText;
        }
        #endregion Title Bar
        #region Flashing Stuff
        private static bool TOP_Form_FlashWindowEx(Form iForm, bool iEnable = true)
        {
            IntPtr hWnd = iForm.Handle;
            WinApi.FLASHWINFO fInfo = new WinApi.FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;
            if (iEnable)
            {
                fInfo.dwFlags = WinApi.FLASHW_ALL | WinApi.FLASHW_TIMERNOFG;
            }
            else
            {
                fInfo.dwFlags = WinApi.FLASHW_STOP;
            }
            fInfo.uCount = uint.MaxValue;
            fInfo.dwTimeout = 0;

            return WinApi.FlashWindowEx(ref fInfo);
        }
        private void Iocaine_2_Form_Activated(object sender, EventArgs e)
        {
            //m_TOP_Form_active = true;
            //ALR_clearAlerts();
        }
        private void Iocaine_2_Form_Deactivate(object sender, EventArgs e)
        {
            //m_TOP_Form_active = false;
        }
        private void TOP_Form_showBlinkingMessage(string text)
        {
            MessageForm msgFrm = new MessageForm(MessageForm.FORM_MODE.BLINK_MSG, text, 1500,
                                                 this.Location.X + 125, this.Location.Y + 195);
            msgFrm.Show();
        }
        private void TOP_Form_flashStuff()
        {
            bool flashFishStatsButton = Statics.Settings.Top.FlashFishStatsButton;
            //Add other conditions here.

            if (flashFishStatsButton)
            {
                Thread flashThread = new Thread(new ThreadStart(TOP_flashingLoopCBF));
                flashThread.IsBackground = true;
                flashThread.Name = "FlashingThread";
                flashThread.Start();
            }
        }
        private void TOP_flashingLoopCBF()
        {
            while (true)
            {
                bool flashFishStatsButton = Statics.Settings.Top.FlashFishStatsButton;
                //Add other conditions here.

                if (flashFishStatsButton && ((int)this.Invoke(new Statics.FuncPtrs.TD_Int32_Void(TOP_getMainTabSelectionIndexCBF)) == 0))
                {
                    this.Invoke(new Statics.FuncPtrs.TD_Void_Color(TOP_setStatsButtonColorCBF), new object[] { SystemColors.MenuHighlight });
                }
                Thread.Sleep(1000);

                //  Now the other half.
                if (flashFishStatsButton)
                {
                    this.Invoke(new Statics.FuncPtrs.TD_Void_Color(TOP_setStatsButtonColorCBF), new object[] { SystemColors.Control });
                }
                Thread.Sleep(1000);
            }
        }
        #endregion Flashing Stuff
        #endregion Top Form Utilities

        // TBD : Move XML to another class in IocaineCore.
        #region Global Utilities
        #region XML Functions (Global)
        internal static string XmlEncodeSpecialCharacters(string iText)
        {
            string localText = iText;
            while (localText.Contains("\"")
                || localText.Contains("&")
                || localText.Contains("'")
                || localText.Contains("<")
                || localText.Contains(">"))
            {
                if (localText.Contains("\""))
                {
                    localText = localText.Replace("\"", m_TOP_Xml_quote);
                }
                if (localText.Contains("&"))
                {
                    localText = localText.Replace("&", m_TOP_Xml_amp);
                }
                if (localText.Contains("'"))
                {
                    localText = localText.Replace("'", m_TOP_Xml_apos);
                }
                if (localText.Contains("<"))
                {
                    localText = localText.Replace("<", m_TOP_Xml_less);
                }
                if (localText.Contains(">"))
                {
                    localText = localText.Replace(">", m_TOP_Xml_greater);
                }
            }
            return localText;
        }
        internal static string XmlDecodeSpecialCharacters(string iText)
        {
            string localText = iText;
            while (localText.Contains(m_TOP_Xml_quote)
                || localText.Contains(m_TOP_Xml_amp)
                || localText.Contains(m_TOP_Xml_apos)
                || localText.Contains(m_TOP_Xml_less)
                || localText.Contains(m_TOP_Xml_greater))
            {
                if (localText.Contains(m_TOP_Xml_quote))
                {
                    localText = localText.Replace(m_TOP_Xml_quote, "\"");
                }
                if (localText.Contains(m_TOP_Xml_amp))
                {
                    localText = localText.Replace(m_TOP_Xml_amp, "&");
                }
                if (localText.Contains(m_TOP_Xml_apos))
                {
                    localText = localText.Replace(m_TOP_Xml_apos, "'");
                }
                if (localText.Contains(m_TOP_Xml_less))
                {
                    localText = localText.Replace(m_TOP_Xml_less, "<");
                }
                if (localText.Contains(m_TOP_Xml_greater))
                {
                    localText = localText.Replace(m_TOP_Xml_greater, ">");
                }
            }
            return localText;
        }
        #endregion XML Functions
        #endregion Global Utilities

        #region To Be Moved (To various bot pages)
        #region Start Buttons
        // TBD : Move to various bot tabs.
        private void updatePLStartButton(string text, System.Drawing.Color color)
        {
            try
            {
                PL_Start_Button.Invoke(new Statics.FuncPtrs.TD_Void_String_Color(updatePLStartButtonCBF), new object[] { text, color });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating PL Start Button: " + e.ToString());
            }
        }
        private void updateSUStartButton(string text, System.Drawing.Color color)
        {
            try
            {
                SU_Start_Button.Invoke(new Statics.FuncPtrs.TD_Void_String_Color(updateSUStartButtonCBF), new object[] { text, color });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating SU Start Button: " + e.ToString());
            }
        }
        private void updateCrafterStartButton(string text, System.Drawing.Color color)
        {
            try
            {
                CB_Start_Button.Invoke(new Statics.FuncPtrs.TD_Void_String_Color(updateCrafterStartButtonCBF), new object[] { text, color });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating Crafter Start Button: " + e.ToString());
            }
        }
        private void updateTAStartButton(string text, System.Drawing.Color color)
        {
            try
            {
                TA_Start_Button.Invoke(new Statics.FuncPtrs.TD_Void_String_Color(updateTAStartButtonCBF), new object[] { text, color });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating TA Start Button: " + e.ToString());
            }
        }
        private void updateTraderStartButton(string text, System.Drawing.Color color)
        {
            try
            {
                TR_Start_Button.Invoke(new Statics.FuncPtrs.TD_Void_String_Color(updateTraderStartButtonCBF), new object[] { text, color });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating Trader Start Button: " + e.ToString());
            }
        }
        private void updatePLStartButtonCBF(string text, System.Drawing.Color color)
        {
            PL_Start_Button.UseMnemonic = true;
            PL_Start_Button.Text = text;
            PL_Start_Button.BackColor = color;
        }
        private void updateSUStartButtonCBF(string text, System.Drawing.Color color)
        {
            SU_Start_Button.UseMnemonic = true;
            SU_Start_Button.Text = text;
            SU_Start_Button.BackColor = color;
        }
        private void updateCrafterStartButtonCBF(string text, System.Drawing.Color color)
        {
            CB_Start_Button.UseMnemonic = true;
            CB_Start_Button.Text = text;
            CB_Start_Button.BackColor = color;
        }
        private void updateTAStartButtonCBF(string text, System.Drawing.Color color)
        {
            TA_Start_Button.UseMnemonic = true;
            TA_Start_Button.Text = text;
            TA_Start_Button.BackColor = color;
        }
        private void updateTraderStartButtonCBF(string text, System.Drawing.Color color)
        {
            TR_Start_Button.UseMnemonic = true;
            TR_Start_Button.Text = text;
            TR_Start_Button.BackColor = color;
        }
        #endregion Start Buttons
        #region Fish Stats
        private void loadFishStatsSettings()
        {
            FSFishCBIndex = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHSTATS, "FishFishComboBox"));
            FSZoneCBIndex = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHSTATS, "ZoneZoneComboBox"));
            FSZoneRodCBIndex = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHSTATS, "ZoneRodComboBox"));
            FSZoneBaitCBIndex = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHSTATS, "ZoneBaitComboBox"));
            FSRodCBIndex = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHSTATS, "RodRodComboBox"));
            FSBaitCBIndex = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.FISHSTATS, "BaitBaitComboBox"));
            Statics.Settings.Top.MapsPath = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.TOP, "MapsPath"));
        }
        #endregion Fish Stats

        #endregion To Be Moved (To various bot pages)

        private void button1_Click(object sender, EventArgs e)
        {
            Parsing.Lua.Categorizer.Load();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Parsing.Lua.Init_Process(true);
            Parsing.Lua.Categorizer.Load();

            AttrSearchBox.SetStringList(Parsing.Lua.GetAttributeList());
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                if (textBox2.Text != "")
                {
                    // Search for item based on given filter.
                    List<string> l_attr = Parsing.Lua.GetItemAttributes(textBox2.Text);
                    if (l_attr != null)
                    {
                        listBox1.Items.Clear();
                        listBox1.Items.AddRange(l_attr.ToArray());
                    }
                }
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                if (textBox3.Text != "")
                {

                }
                e.Handled = true;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            string l_filter = parser_get_regex_filter(textBox3.Text);
            List<string> l_matches = Parsing.Lua.GetAttributeList(l_filter);
            listBox2.Items.Clear();
            listBox2.Items.AddRange(l_matches.ToArray());
        }

        private string lastParserFilter = "";
        private string parser_get_regex_filter(string iText)
        {
            string l_retVal = iText;

            // ? matches single character.
            // # matches single number.
            // [! ] matches a single character that is not in the set.
            // * matches one or more characters.
            // [ ] matches any one of the characters specified in the set.
            //
            // ? converts to .
            // # converts to [0-9]
            // [! ] converts to [^ ]
            // * converts to .*
            // [ ] no conversion needed
            l_retVal = System.Text.RegularExpressions.Regex.Replace(l_retVal, @"([^\\]|^)\?", @"$1.");
            l_retVal = System.Text.RegularExpressions.Regex.Replace(l_retVal, @"([^\\]|^)\#", @"$1[0-9]");
            l_retVal = System.Text.RegularExpressions.Regex.Replace(l_retVal, @"([^\\]|^)\*", @"$1.*");
            l_retVal = System.Text.RegularExpressions.Regex.Replace(l_retVal, @"([^\\]|^)\[\!", @"$1[^");
            if (isValidRegex(l_retVal))
            {
                lastParserFilter = l_retVal;
                return l_retVal;
            }
            else
            {
                return lastParserFilter;
            }
        }
        private bool isValidRegex(string iFilter)
        {
            if (string.IsNullOrEmpty(iFilter))
            {
                return false;
            }
            try
            {
                System.Text.RegularExpressions.Regex.Match("", iFilter);
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }

        private void AttrSearchBox_TextChanged(object sender, EventArgs e)
        {
            //List<string> l_matches = Parsing.Lua.GetAttributeList(AttrSearchBox.Pattern);
            //listBox2.Items.Clear();
            //listBox2.Items.AddRange(l_matches.ToArray());
        }
    }
}
