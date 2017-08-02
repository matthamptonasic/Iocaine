using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Iocaine2;
using Iocaine2.Data.Client;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Threading;
using Iocaine2.Tools;

namespace IocaineOffsetHelper
{
    public partial class IocaineOffsetsHelperForm : Form
    {
        #region Constructor
        public IocaineOffsetsHelperForm()
        {
            InitializeComponent();
            Iocaine_2_Form.setLogFile();
            mainThread = new Thread(new ThreadStart(runHelper));
            mainThread.IsBackground = true;
            mainThread.Start();
            //ProcessFunctions.enumModules(ProcessFunctions.getMainProcessByWindowName("Calculator"));
        }
        #endregion Constructor
        #region Enums
        public enum OFFSET
        {
            PLAYER_1,
            PLAYER_2_GEN,
            PLAYER_2_SKILLS,
            PLAYER_2_CRAFT,
            PLAYER_2_STATUS,
            PLAYER_3,
            PLAYER_5,
            TARGET,
            FISHING,
            WINDOWS,
            INVENTORY,
            CHAT_LOG,
            COMMAND,
            SERVER_LIST,
            BAG,
            EQUIP_TABLE,
            PC_MAP,
            NPC_MAP,
            PARTY,
            SHOP_GUILD,
            SHOP_NPC,
            CRAFT_WIN,
            TRADE_WIN,
            TRADE_PC_WIN,
            SPELL_RECAST,
            ABILITY_RECAST,
            NAVIGATION,
            TIME,
            NETWORK,
            MENU_TEXT,
            MENU_BUTTON,
            AH,
            LUA_INTF
        }
        #endregion Enums
        #region Members
        private Process[] processList = null;
        private Process mainProc = null;
        private ProcessModule mainMod = null;
        private Thread mainThread = null;
        private IocaineThread changeMonitorThread = null;
        private Byte job;
        private Byte subJob;
        private Byte jobLvl;
        private List<Spells.SPELL_INFO> spellInfoList;
        private List<UInt16> jaRecastIdList;    //This holds available recastID's from memory in structural order.
        private List<MemReads.NPCs.NPCInfoStruct> npcInfoList = new List<MemReads.NPCs.NPCInfoStruct>();
        private MemReads.NPCs.NPCInfoStruct npcInfo = new MemReads.NPCs.NPCInfoStruct();
        private ChatLoggerAsync chatlog = new ChatLoggerAsync();
        private uint playerPtr = 0;
        private uint playerPosPtr = 0;
        private float posX = 0f;
        private float posY = 0f;
        private float posZ = 0f;
        private float posH = 0f;
        private bool speedEnabled = false;
        private float speedNormal = 5.0f;
        private bool showSackData = false;
        private bool refresh = false;
        private bool commandExtended = false;
        #region Synchronization
        public delegate void SetProcessCBIndexDelegate(Int32 iIndex);
        public SetProcessCBIndexDelegate SetProcessCBIndexPtr;
        public delegate void AddItemToProcessCBDelegate(String iItem);
        public AddItemToProcessCBDelegate AddItemToProcessCBPtr;
        public delegate void SetOffsetCBIndexDelegate(Int32 iIndex);
        public SetOffsetCBIndexDelegate SetOffsetCBIndexPtr;
        public delegate void AddItemToOffsetCBDelegate(String iItem);
        public AddItemToOffsetCBDelegate AddItemToOffsetCBPtr;
        public delegate Int32 GetOffsetCBIndexDelegate();
        public GetOffsetCBIndexDelegate GetOffsetCBIndexPtr;
        public delegate void AppendTextDelegate(String iText);
        public AppendTextDelegate AppendTextPtr;
        public delegate void ClearTextDelegate();
        public ClearTextDelegate ClearTextPtr;
        public delegate void ScrollToCaretDelegate();
        public ScrollToCaretDelegate ScrollToCaretPtr;
        #endregion Synchronization
        #endregion Members
        #region Main Function
        private void runHelper()
        {
            #region Inits
            try
            {
                SetProcessCBIndexPtr = new SetProcessCBIndexDelegate(SetProcessCBIndexCBF);
                AddItemToProcessCBPtr = new AddItemToProcessCBDelegate(AddItemToProcessCBCBF);
                SetOffsetCBIndexPtr = new SetOffsetCBIndexDelegate(SetOffsetCBIndexCBF);
                AddItemToOffsetCBPtr = new AddItemToOffsetCBDelegate(AddItemToOffsetCBCBF);
                GetOffsetCBIndexPtr = new GetOffsetCBIndexDelegate(GetOffsetCBIndexCBF);
                AppendTextPtr = new AppendTextDelegate(AppendTextCBF);
                ClearTextPtr = new ClearTextDelegate(ClearTextCBF);
                ScrollToCaretPtr = new ScrollToCaretDelegate(ScrollToCaretCBF);

                //LoggingFunctions.AddDebugScope(LoggingFunctions.DBG_SCOPE.ALL);
                LoggingFunctions.AddDebugScope(LoggingFunctions.DBG_SCOPE.MEMREADS);
                LoggingFunctions.AddDebugScope(LoggingFunctions.DBG_SCOPE.MEM_SCANNER);
                processList = ProcessFunctions.GetAllProcessByProcessName("pol");
                if (processList.Length == 0)
                {
                    return;
                }
                Console.WriteLine("Initializing the processCB items.");
                foreach (Process proc in processList)
                {
                    Console.WriteLine("Adding process to CB: " + proc.MainWindowTitle);
                    AddItemToProcessCB(proc.MainWindowTitle);
                }
                Console.WriteLine("Finished adding to CB.");
                SetProcessCBIndex(0);
                
                if (MemReads.Set_FFXI_Pointers(mainProc, mainMod) == -1)
                {
                    return;
                }

                FfxiResource.init();

                //End mem scanner section
                AddItemToOffsetCB("Player_1");
                AddItemToOffsetCB("Player_2_Gen");
                AddItemToOffsetCB("Player_2_Skills");
                AddItemToOffsetCB("Player_2_Crafting");
                AddItemToOffsetCB("Player_2_Status_Effects");
                AddItemToOffsetCB("Player_3");
                AddItemToOffsetCB("Player_5");
                AddItemToOffsetCB("Target");
                AddItemToOffsetCB("Fishing");
                AddItemToOffsetCB("Windows");
                AddItemToOffsetCB("Inventory");
                AddItemToOffsetCB("Chat Log");
                AddItemToOffsetCB("Command");
                AddItemToOffsetCB("Server List");
                AddItemToOffsetCB("Bag");
                AddItemToOffsetCB("Equip Table");
                AddItemToOffsetCB("PC Info");
                AddItemToOffsetCB("NPC Info");
                AddItemToOffsetCB("Party");
                AddItemToOffsetCB("Shop - Guild");
                AddItemToOffsetCB("Shop - NPC");
                AddItemToOffsetCB("Craft Window");
                AddItemToOffsetCB("Trade Window");
                AddItemToOffsetCB("Trade PC Window");
                AddItemToOffsetCB("Spell Recast");
                AddItemToOffsetCB("Ability Recast");
                AddItemToOffsetCB("Navigation");
                AddItemToOffsetCB("Time");
                AddItemToOffsetCB("Network");
                AddItemToOffsetCB("Menu Text-Style");
                AddItemToOffsetCB("Menu Button-Style");
                AddItemToOffsetCB("Auction House");
                AddItemToOffsetCB("LUA Interface");
                SetOffsetCBIndex((int)OFFSET.LUA_INTF);
            }
            catch(Exception e)
            {
                MessageBox.Show("Couldn't find POL process." + e.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            #endregion Inits
            if ((mainProc != null) && (mainMod != null))
            {
                Iocaine2.Inventory.Containers.Init();
                ChangeMonitor.MainProc = mainProc;
                ChangeMonitor.MainModule = mainMod;
                changeMonitorThread = new IocaineThread("changeMonitorThread");
                changeMonitorThread.__RunMethod = ChangeMonitor.Run;
                ChangeMonitor.__CheckStatus = changeMonitorThread.__CheckState;
                changeMonitorThread.Start();
                while (true)
                {
                    ClearText();
                    AppendText("*********************  Mem Reads  ***********************\n");
                    switch (GetOffsetCBIndex())
                    {
                        #region Player 1
                        case (int)OFFSET.PLAYER_1:
                            Iocaine_2_Form.timestamp("=====  Info Player 1  =====");
                            AppendText("=====  Info Player 1  =====\n");
                            AppendText("Name w/  null: " + MemReads.Self.get_name(false));
                            AppendText("\nName w/o null: " + MemReads.Self.get_name(true));
                            AppendText("\nCurr HP: " + MemReads.Self.Vitals.get_hp_current());
                            AppendText("\nCurr MP: " + MemReads.Self.Vitals.get_mp_current());
                            AppendText("\nCurr TP: " + MemReads.Self.Vitals.get_tp_current());
                            AppendText("\nCurr HPP: " + MemReads.Self.Vitals.get_hp_percent());
                            AppendText("\nCurr MPP: " + MemReads.Self.Vitals.get_mp_percent());
                            AppendText("\nCurr Zone ID: " + MemReads.Self.get_zone_id());
                            AppendText("\nIn Mog House: " + MemReads.Self.get_in_mog_house());
                            break;
                        #endregion Player 1
                        #region Player 2 General
                        case (int)OFFSET.PLAYER_2_GEN:
                            AppendText("=====  Info Player 2  =====\n");
                            AppendText("Curr HP Max: " + MemReads.Self.Vitals.get_hp_max());
                            AppendText("\nCurr MP Max: " + MemReads.Self.Vitals.get_mp_max());
                            AppendText("\nMain Job: " + MemReads.Self.Job.get_main());
                            AppendText("\nMain Job Lvl: " + MemReads.Self.Job.get_main_lvl());
                            AppendText("\nSub Job: " + MemReads.Self.Job.get_sub());
                            AppendText("\nSub Job Lvl: " + MemReads.Self.Job.get_sub_lvl());
                            AppendText("\nXP Curr: " + MemReads.Self.XP.get_xp_current());
                            AppendText("\nXP Max: " + MemReads.Self.XP.get_xp_max());
                            AppendText("\nWeather ID: " + MemReads.Environment.get_weather_id());
                            AppendText("\nWeather Name: " + MemReads.Environment.get_weather_name());
                            AppendText("\nZoning Now: " + MemReads.Self.get_is_zoning());
                            AppendText("\nLimit Points: " + MemReads.Self.XP.get_mrt_limit_points());
                            AppendText("\nMerit Points: " + MemReads.Self.XP.get_mrt_merits_current());
                            AppendText("\nXP Mode: " + ((MemReads.Self.XP.get_xp_mode() == FFXIEnums.XP_MODE.MERIT) ? "Merit" : "XP"));
                            break;
                        #endregion Player 2 General
                        #region Player 2 Skills
                        case (int)OFFSET.PLAYER_2_SKILLS:
                            AppendText("\nIntelligence: " + MemReads.Self.Attributes.get_int_base());
                            AppendText("\nMind: " + MemReads.Self.Attributes.get_mnd_base());
                            AppendText("\nInt Bonus: " + MemReads.Self.Attributes.get_int_bonus());
                            AppendText("\nMind Bonus: " + MemReads.Self.Attributes.get_mnd_bonus());
                            AppendText("\nSkill H2H: " + MemReads.Self.Skills.Combat.get_hand_to_hand());
                            AppendText("\nCap'd H2H: " + MemReads.Self.Skills.Combat.get_hand_to_hand_capped());
                            AppendText("\nSkill Dagger: " + MemReads.Self.Skills.Combat.get_dagger());
                            AppendText("\nCap'd Dagger: " + MemReads.Self.Skills.Combat.get_dagger_capped());
                            AppendText("\nSkill Sword: " + MemReads.Self.Skills.Combat.get_sword());
                            AppendText("\nCap'd Sword: " + MemReads.Self.Skills.Combat.get_sword_capped());
                            AppendText("\nSkill Axe: " + MemReads.Self.Skills.Combat.get_axe());
                            AppendText("\nCap'd Axe: " + MemReads.Self.Skills.Combat.get_axe_capped());
                            AppendText("\nSkill Polearm: " + MemReads.Self.Skills.Combat.get_polearm());
                            AppendText("\nCap'd Polearm: " + MemReads.Self.Skills.Combat.get_polearm_capped());
                            AppendText("\nSkill Evasion: " + MemReads.Self.Skills.Combat.get_evasion());
                            AppendText("\nCap'd Evasion: " + MemReads.Self.Skills.Combat.get_evasion_capped());
                            AppendText("\nSkill Throwing: " + MemReads.Self.Skills.Combat.get_throwing());
                            AppendText("\nCap'd Throwing: " + MemReads.Self.Skills.Combat.get_throwing_capped());
                            break;
                        #endregion Player 2 Skills
                        #region Player 2 Crafts
                        case (int)OFFSET.PLAYER_2_CRAFT:
                            AppendText("\n********** Crafting *************");
                            AppendText("\nAlch Cap'd: " + MemReads.Self.Skills.Crafting.get_alch_capped());
                            AppendText("\nAlch Skill: " + MemReads.Self.Skills.Crafting.get_alch_skill());
                            AppendText("\nAlch Rank: " + MemReads.Self.Skills.Crafting.get_rank_id_to_string(MemReads.Self.Skills.Crafting.get_alch_rank_id()));
                            AppendText("\nBone Cap'd: " + MemReads.Self.Skills.Crafting.get_bone_capped());
                            AppendText("\nBone Skill: " + MemReads.Self.Skills.Crafting.get_bone_skill());
                            AppendText("\nBone Rank: " + MemReads.Self.Skills.Crafting.get_rank_id_to_string(MemReads.Self.Skills.Crafting.get_bone_rank_id()));
                            AppendText("\nCloth Cap'd: " + MemReads.Self.Skills.Crafting.get_cloth_capped());
                            AppendText("\nCloth Skill: " + MemReads.Self.Skills.Crafting.get_cloth_skill());
                            AppendText("\nCloth Rank: " + MemReads.Self.Skills.Crafting.get_rank_id_to_string(MemReads.Self.Skills.Crafting.get_cloth_rank_id()));
                            AppendText("\nCook Cap'd: " + MemReads.Self.Skills.Crafting.get_cook_capped());
                            AppendText("\nCook Skill: " + MemReads.Self.Skills.Crafting.get_cook_skill());
                            AppendText("\nCook Rank: " + MemReads.Self.Skills.Crafting.get_rank_id_to_string(MemReads.Self.Skills.Crafting.get_cook_rank_id()));
                            AppendText("\nFish Cap'd: " + MemReads.Self.Skills.Crafting.get_fish_capped());
                            AppendText("\nFish Skill: " + MemReads.Self.Skills.Crafting.get_fish_skill());
                            AppendText("\nFish Rank: " + MemReads.Self.Skills.Crafting.get_rank_id_to_string(MemReads.Self.Skills.Crafting.get_fish_rank_id()));
                            AppendText("\nGold Cap'd: " + MemReads.Self.Skills.Crafting.get_gold_capped());
                            AppendText("\nGold Skill: " + MemReads.Self.Skills.Crafting.get_gold_skill());
                            AppendText("\nGold Rank: " + MemReads.Self.Skills.Crafting.get_rank_id_to_string(MemReads.Self.Skills.Crafting.get_gold_rank_id()));
                            AppendText("\nLeather Cap'd: " + MemReads.Self.Skills.Crafting.get_leather_capped());
                            AppendText("\nLeather Skill: " + MemReads.Self.Skills.Crafting.get_leather_skill());
                            AppendText("\nLeather Rank: " + MemReads.Self.Skills.Crafting.get_rank_id_to_string(MemReads.Self.Skills.Crafting.get_leather_rank_id()));
                            AppendText("\nSmith Cap'd: " + MemReads.Self.Skills.Crafting.get_smith_capped());
                            AppendText("\nSmith Skill: " + MemReads.Self.Skills.Crafting.get_smith_skill());
                            AppendText("\nSmith Rank: " + MemReads.Self.Skills.Crafting.get_rank_id_to_string(MemReads.Self.Skills.Crafting.get_smith_rank_id()));
                            AppendText("\nWood Cap'd: " + MemReads.Self.Skills.Crafting.get_wood_capped());
                            AppendText("\nWood Skill: " + MemReads.Self.Skills.Crafting.get_wood_skill());
                            AppendText("\nWood Rank: " + MemReads.Self.Skills.Crafting.get_rank_id_to_string(MemReads.Self.Skills.Crafting.get_wood_rank_id()));
                            break;
                        #endregion Player 2 Crafts
                        #region Player 2 Status Effects
                        case (int)OFFSET.PLAYER_2_STATUS:
                            UInt16 [] statusArray = new UInt16 [32];
                            for(int kk=0; kk<32; kk++)
                            {
                                statusArray[kk] = 0xFFFF;
                            }
                            MemReads.Self.StatusEffects.get_effects(ref statusArray);
                            AppendText("********** Status Effects *************");
                            for (int ii = 0; ii < 32; ii++)
                            {
                                if (statusArray[ii] == 0xFFFF)
                                {
                                    break;
                                }
                                else
                                {
                                    AppendText("\n" + ii + ": " + StatusEffects.GetStatusEffectInfo(statusArray[ii]).Name + " (" + statusArray[ii] + ")");
                                }
                            }
                            break;
                        #endregion Player 2 Status Effects
                        #region Player 3
                        case (int)OFFSET.PLAYER_3:
                            AppendText("=====  Info Player 3  =====\n");
                            AppendText("Pos X: " + MemReads.Self.Position.get_x());
                            AppendText("\nPos Y: " + MemReads.Self.Position.get_y());
                            AppendText("\nPos Z: " + MemReads.Self.Position.get_z());
                            AppendText("\nPos H: " + MemReads.Self.Position.get_heading());
                            AppendText("\nMap Grid: " + MemReads.Self.Position.get_map_grid());
                            break;
                        #endregion Player 3
                        #region Player 5
                        case (int)OFFSET.PLAYER_5:
                            AppendText("=====  Info Player 5  =====\n");
                            AppendText("Player Status: " + MemReads.Self.get_status());
                            AppendText("\nPlayer view  : " + MemReads.Self.Camera.get_view_perspective());
                            break;
                        #endregion Player 5
                        #region Target
                        case (int)OFFSET.TARGET:
                            AppendText("=====  Info Target  =====\n");
                            AppendText("Target Name: " + MemReads.Target.get_name());
                            AppendText("\nTarget HPP: " + MemReads.Target.get_hp_perc().ToString());
                            AppendText("\nTarget Pos X: " + String.Format("{0:0.0#}", MemReads.Target.get_position_x()));
                            AppendText("\nTarget Pos Y: " + String.Format("{0:0.0#}",MemReads.Target.get_position_y()));
                            AppendText("\nTarget Pos Z: " + String.Format("{0:0.0#}",MemReads.Target.get_position_z()));
                            AppendText("\nTarget Pos H: " + String.Format("{0:0.0#}", MemReads.Target.get_position_angle()));
                            AppendText("\nTarget Distance: " + MemReads.Target.get_distance());
                            AppendText("\nTarget Status: " + MemReads.Target.get_status());
                            AppendText("\nTarget Locked: " + MemReads.Target.get_locked());
                            AppendText("\nTarget ID: " + String.Format("{0:X}", MemReads.Target.get_id()));
                            break;
                        #endregion Target
                        #region Fishing
                        case (int)OFFSET.FISHING:
                            AppendText("=====  Info Fishing  =====\n");
                            AppendText("Fish ID1: " + MemReads.Fishing.get_id1());
                            AppendText("\nFish ID2: " + MemReads.Fishing.get_id2());
                            AppendText("\nFish ID3: " + MemReads.Fishing.get_id3());
                            AppendText("\nFish Large: " + MemReads.Fishing.get_large());
                            AppendText("\nFish HP Cur: " + MemReads.Fishing.get_cur_hp());
                            AppendText("\nFish HP Max: " + MemReads.Fishing.get_max_hp());
                            AppendText("\nArrow Direction: " + MemReads.Fishing.get_arrow_direction().ToString());
                            AppendText("\nArrow Timer Zero: " + MemReads.Fishing.is_arrow_timer_zero().ToString());
                            AppendText("\nArrow Timer Value: " + MemReads.Fishing.get_arrow_timer_value().ToString());
                            while (MemReads.Self.get_status() == (Byte)FFXIEnums.STATUS.FISH_ON_HOOK)
                            {
                                UInt16 value = MemReads.Fishing.get_arrow_timer_value();
                                LoggingFunctions.Timestamp("Timer: " + value.ToString());
                                IocaineFunctions.delay(100);
                                LoggingFunctions.Timestamp("Fishing Structure:");
                                UIntPtr ptr = MemReads.Fishing.get_fishing_ptr();
                                MemReads.logMemoryBlock(mainMod, (uint)ptr - 64, (uint)ptr + 128, 1, true);
                            }
                            //AppendText("\nRod Position: " + MemReads.Fishing.get_rod_position());
                            //AppendText("\nRod Location: " + MemReads.Fishing.get_rod_location());
                            //AppendText("\nLast Catch: " + MemReads.Fishing.get_last_catch(MemReads.Self.get_name(false)));
                            break;
                        #endregion Fishing
                        #region Windows
                        case (int)OFFSET.WINDOWS:
                            AppendText("=====  Info Windows  =====\n");
                            AppendText("Help Text: " + MemReads.Windows.BannerText.get_help_text());
                            AppendText("\nTop/Left Text: " + MemReads.Windows.BannerText.get_top_left_text());
                            AppendText("\nShop Quan Cur: " + MemReads.Windows.Items.get_shop_quan_cur());
                            AppendText("\nShop Quan Max: " + MemReads.Windows.Items.get_shop_quan_max());
                            AppendText("\nItem Name: " + MemReads.Windows.Items.get_selected_item_name());
                            String leftText = MemReads.Windows.BannerText.get_top_left_text();
                            byte itemIdx = MemReads.Windows.Items.get_selection_index();
                            AppendText("\nItem index in inv: " + itemIdx);
                            byte itemQuan = 0;
                            bool itemEquipped = false;
                            byte structIdx = MemReads.Windows.Items.get_selected_item_inventory_index();
                            AppendText("\nItem index in inv structure: " + structIdx);
                            if (leftText == "Items")
                            {
                                itemQuan = MemReads.Self.Inventory.get_bag_item_quan(structIdx);
                                itemEquipped = MemReads.Self.Inventory.get_bag_item_equipped(structIdx);
                            }
                            else if (leftText == "Mog Sack")
                            {
                                itemQuan = MemReads.Self.Inventory.get_sack_item_quan(structIdx);
                                itemEquipped = MemReads.Self.Inventory.get_sack_item_equipped(structIdx);
                            }
                            else if (leftText == "Satchel")
                            {
                                itemQuan = MemReads.Self.Inventory.get_satchel_item_quan(structIdx);
                                itemEquipped = MemReads.Self.Inventory.get_satchel_item_equipped(structIdx);
                            }
                            else if (leftText == "Mog Safe")
                            {
                                itemQuan = MemReads.Self.Inventory.get_safe_item_quan(structIdx);
                            }
                            else if (leftText == "Storage")
                            {
                                itemQuan = MemReads.Self.Inventory.get_storage_item_quan(structIdx);
                            }
                            else if (leftText == "Locker")
                            {
                                itemQuan = MemReads.Self.Inventory.get_locker_item_quan(structIdx);
                            }
                            AppendText("\nItem Quantity: " + itemQuan);
                            AppendText("\nItem Equipped: " + itemEquipped);
                            break;
                        #endregion Windows
                        #region Inventory
                        case (int)OFFSET.INVENTORY:
                            AppendText("=====  Info Inventory  =====\n");
                            AppendText("Inv Count: " + MemReads.Windows.Items.get_count());
                            AppendText("\nInv Location: " + MemReads.Windows.Items.get_selection_index());
                            AppendText("\n2nd Wnd Open: " + MemReads.Windows.Items.get_sec_wnd_open());
                            AppendText("\n2nd Wnd Selected: " + MemReads.Windows.Items.get_left_wnd_selected());
                            AppendText("\n2nd Wnd Location: " + MemReads.Windows.Items.get_sec_wnd_selection_index());
                            AppendText("\n2nd Wnd Count: " + MemReads.Windows.Items.get_sec_wnd_count());
                            break;
                        #endregion Inventory
                        #region Chat Log
                        case (int)OFFSET.CHAT_LOG:
                            AppendText("=====  Info Chat Log  =====\n");
                            #region Old Method (no ChatLogger)
                            //uint lastLineNb = MemReads.Chat.get_index();
                            //uint currentLineNb = lastLineNb;
                            //int newLines = 0;
                            //String oLogicalLineNb = "";
                            //String lastLogLineNb = "";
                            #endregion Old Method (no ChatLogger)
                            chatlog.Reset();
                            while (GetOffsetCBIndex() == (int)OFFSET.CHAT_LOG)
                            {
                                #region Old Method (no ChatLogger)
                                //currentLineNb = MemReads.Chat.get_index();
                                //newLines = (int)currentLineNb - (int)lastLineNb;
                                //int textBoxLines = MainTextBox.Lines.Length;
                                //if (newLines < 0)
                                //{
                                //    newLines += 50;
                                //}
                                //if (newLines != 0)
                                //{
                                //    int code = 0;
                                //    for (int ii = newLines-1; ii >= 0; ii--)
                                //    {
                                //        try
                                //        {
                                //            String text = MemReads.Chat.get_lineX(ii, ref code, ref oLogicalLineNb);
                                //            if (oLogicalLineNb != lastLogLineNb)
                                //            {
                                //                AppendText("\nTotal: " + ++textBoxLines + " : Current: " + (currentLineNb - ii) + ": " + text + " w/ code: " + code);
                                //            }
                                //            else
                                //            {
                                //                AppendText("\nTotal: " + ++textBoxLines + " : Current: " + (currentLineNb - ii) + " (cont'd): " + text + " w/ code: " + code);
                                //            }
                                //            lastLogLineNb = oLogicalLineNb;
                                //            Iocaine_2_Form.debug("\n[FORM] Total: " + textBoxLines + " : Current: " + (currentLineNb - ii) + ": " + text + " w/ code: " + code);
                                //        }
                                //        catch(Exception Ex)
                                //        {
                                //            Iocaine_2_Form.timestamp("[ERROR] when writing to text box: " + Ex.ToString());
                                //        }
                                //        MainTextBox.ScrollToCaret();
                                //    }
                                //}
                                //lastLineNb = currentLineNb;
                                #endregion Old Method (no ChatLogger)
                                uint nbLines = 0;
                                if (chatlog.Update(ref nbLines))
                                {
                                    for (int pp = 0; pp < nbLines; pp++)
                                    {
                                        ChatLine chat = null;
                                        if (chatlog.Read(out chat))
                                        {
                                            AppendText("\n" + chat.ToString());
                                        }
                                    }
                                    ScrollToCaret();
                                }

                                Thread.Sleep(2000);
                            }
                            #region Display Last 10 Lines
                            //AppendText("Chat log -10: " + MemReads.info_chatlog_lineX(mainProc, mainMod, 10));
                            //AppendText("\nChat log -09: " + MemReads.info_chatlog_lineX(mainProc, mainMod, 9));
                            //AppendText("\nChat log -08: " + MemReads.info_chatlog_lineX(mainProc, mainMod, 8));
                            //AppendText("\nChat log -07: " + MemReads.info_chatlog_lineX(mainProc, mainMod, 7));
                            //AppendText("\nChat log -06: " + MemReads.info_chatlog_lineX(mainProc, mainMod, 6));
                            //AppendText("\nChat log -05: " + MemReads.info_chatlog_lineX(mainProc, mainMod, 5));
                            //AppendText("\nChat log -04: " + MemReads.info_chatlog_lineX(mainProc, mainMod, 4));
                            //AppendText("\nChat log -03: " + MemReads.info_chatlog_lineX(mainProc, mainMod, 3));
                            //AppendText("\nChat log -02: " + MemReads.info_chatlog_lineX(mainProc, mainMod, 2));
                            //AppendText("\nChat log -01: " + MemReads.info_chatlog_lineX(mainProc, mainMod, 1));
                            //AppendText("\nChat log -00: " + MemReads.info_chatlog_lineX(mainProc, mainMod, 0));
                            #endregion Display Last 10 Lines
                            break;
                        #endregion Chat Log
                        #region Equip Table
                        case (int)OFFSET.EQUIP_TABLE:
                            AppendText("=====  Info Equipment  =====\n");
                            AppendText("Eqp'd Main: " + MemReads.Self.Equipment.get_main_equipped());
                            ushort equ_id = MemReads.Self.Equipment.get_main_id();
                            AppendText("\nEq ID Main: (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            AppendText("\nEqp'd Sub: " + MemReads.Self.Equipment.get_sub_equipped());
                            equ_id = MemReads.Self.Equipment.get_sub_id();
                            AppendText("\nEq ID Sub : (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            equ_id = MemReads.Self.Equipment.get_range_id();
                            AppendText("\nEqp'd Rang: " + MemReads.Self.Equipment.get_range_equipped());
                            AppendText("\nEq ID Rang: (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            AppendText("\nEqp'd Ammo: " + MemReads.Self.Equipment.get_ammo_equipped());
                            equ_id = MemReads.Self.Equipment.get_ammo_id();
                            AppendText("\nEq ID Ammo: (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            AppendText("\nEq Qn Ammo: " + MemReads.Self.Equipment.get_ammo_quan());
                            AppendText("\nEqp'd Head: " + MemReads.Self.Equipment.get_head_equipped());
                            equ_id = MemReads.Self.Equipment.get_head_id();
                            AppendText("\nEq ID Head: (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            AppendText("\nEqp'd Neck: " + MemReads.Self.Equipment.get_neck_equipped());
                            equ_id = MemReads.Self.Equipment.get_neck_id();
                            AppendText("\nEq ID Neck: (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            AppendText("\nEqp'd EarL: " + MemReads.Self.Equipment.get_earL_equipped());
                            equ_id = MemReads.Self.Equipment.get_earL_id();
                            AppendText("\nEq ID EarL: (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            AppendText("\nEqp'd EarR: " + MemReads.Self.Equipment.get_earR_equipped());
                            equ_id = MemReads.Self.Equipment.get_earR_id();
                            AppendText("\nEq ID EarR: (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            AppendText("\nEqp'd Body: " + MemReads.Self.Equipment.get_body_equipped());
                            equ_id = MemReads.Self.Equipment.get_body_id();
                            AppendText("\nEq ID Body: (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            AppendText("\nEqp'd Hand: " + MemReads.Self.Equipment.get_hands_equipped());
                            equ_id = MemReads.Self.Equipment.get_hands_id();
                            AppendText("\nEq ID Hand: (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            AppendText("\nEqp'd RngL: " + MemReads.Self.Equipment.get_ringL_equipped());
                            equ_id = MemReads.Self.Equipment.get_ringL_id();
                            AppendText("\nEq ID RngL: (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            AppendText("\nEqp'd RngR: " + MemReads.Self.Equipment.get_ringR_equipped());
                            equ_id = MemReads.Self.Equipment.get_ringR_id();
                            AppendText("\nEq ID RngR: (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            AppendText("\nEqp'd Back: " + MemReads.Self.Equipment.get_back_equipped());
                            equ_id = MemReads.Self.Equipment.get_back_id();
                            AppendText("\nEq ID Back: (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            AppendText("\nEqp'd Wast: " + MemReads.Self.Equipment.get_waist_equipped());
                            equ_id = MemReads.Self.Equipment.get_waist_id();
                            AppendText("\nEq ID Wast: (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            AppendText("\nEqp'd Legs: " + MemReads.Self.Equipment.get_legs_equipped());
                            equ_id = MemReads.Self.Equipment.get_legs_id();
                            AppendText("\nEq ID Legs: (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            AppendText("\nEqp'd Feet: " + MemReads.Self.Equipment.get_feet_equipped());
                            equ_id = MemReads.Self.Equipment.get_feet_id();
                            AppendText("\nEq ID Feet: (" + equ_id + ") " + Things.GetNameFromId(equ_id));
                            break;
                        #endregion Equip Table
                        #region Server
                        case (int)OFFSET.SERVER_LIST:
                            AppendText("=====  Info Server  =====\n");
                            AppendText("Server Name: " + MemReads.Environment.get_server(MemReads.Self.get_name()));
                            break;
                        #endregion Server
                        #region Bag
                        case (int)OFFSET.BAG:
                            AppendText("=====  Info Bag  =====\n");
                            AppendText("\nGil              : " + MemReads.Self.Inventory.get_gil());
                            AppendText("\n=== Bag ===");
                            AppendText("\n        cnt: " + MemReads.Self.Inventory.get_bag_occupancy());
                            AppendText("\n        max: " + MemReads.Self.Inventory.get_max_bag());
                            AppendText("\n=== Safe ===");
                            AppendText("\n        cnt: " + MemReads.Self.Inventory.get_safe_occupancy());
                            AppendText("\n        max: " + MemReads.Self.Inventory.get_max_safe());
                            AppendText("\n=== Safe 2 ===");
                            AppendText("\n        cnt: " + MemReads.Self.Inventory.get_safe2_occupancy());
                            AppendText("\n        max: " + MemReads.Self.Inventory.get_max_safe2());
                            AppendText("\n=== Storage ===");
                            AppendText("\n        cnt: " + MemReads.Self.Inventory.get_storage_occupancy());
                            AppendText("\n        max: " + MemReads.Self.Inventory.get_max_storage());
                            AppendText("\n=== Locker ===");
                            AppendText("\n        cnt: " + MemReads.Self.Inventory.get_locker_occupancy());
                            AppendText("\n        max: " + MemReads.Self.Inventory.get_max_locker());
                            AppendText("\n=== Satchel ===");
                            AppendText("\n        cnt: " + MemReads.Self.Inventory.get_satchel_occupancy());
                            AppendText("\n        max: " + MemReads.Self.Inventory.get_max_satchel());
                            AppendText("\n=== Sack ===");
                            AppendText("\n        cnt: " + MemReads.Self.Inventory.get_sack_occupancy());
                            AppendText("\n        max: " + MemReads.Self.Inventory.get_max_sack());
                            AppendText("\n=== Case ===");
                            AppendText("\n        cnt: " + MemReads.Self.Inventory.get_case_occupancy());
                            AppendText("\n        max: " + MemReads.Self.Inventory.get_max_case());
                            AppendText("\n=== Wardrobe ===");
                            AppendText("\n        cnt: " + MemReads.Self.Inventory.get_wardrobe_occupancy());
                            AppendText("\n        max: " + MemReads.Self.Inventory.get_max_wardrobe());
                            AppendText("\n=== Wardrobe 2 ===");
                            AppendText("\n        cnt: " + MemReads.Self.Inventory.get_wardrobe2_occupancy());
                            AppendText("\n        max: " + MemReads.Self.Inventory.get_max_wardrobe2());
                            AppendText("\n=== Wardrobe 3 ===");
                            AppendText("\n        cnt: " + MemReads.Self.Inventory.get_wardrobe3_occupancy());
                            AppendText("\n        max: " + MemReads.Self.Inventory.get_max_wardrobe3());
                            AppendText("\n=== Wardrobe 4 ===");
                            AppendText("\n        cnt: " + MemReads.Self.Inventory.get_wardrobe4_occupancy());
                            AppendText("\n        max: " + MemReads.Self.Inventory.get_max_wardrobe4());
                            if (showSackData)
                            {
                                byte maxSackCnt = MemReads.Self.Inventory.get_max_sack();
                                for (byte ii = 0; ii < maxSackCnt; ii++)
                                {
                                    AppendText("\nMem[" + ii + "] = " + Things.GetNameFromId(MemReads.Self.Inventory.get_sack_item_id(ii)) + " x" + MemReads.Self.Inventory.get_sack_item_quan(ii));
                                }
                                while (!refresh)
                                {
                                    Thread.Sleep(500);
                                }
                                refresh = false;
                            }
                            break;
                        #endregion Bag
                        #region PC Map
                        case (int)OFFSET.PC_MAP:
                            AppendText("=====  Info PC Map  =====\n");
                            //UInt32 ptr = MemReads.info_pc_pointer(mainProc, mainMod, MemReads.info_player_name(mainProc, mainMod, true));
                            if (pcNameTB.Text != "")
                            {
                                UInt32 pcIdx;
                                UInt32 pcIdxAddr;
                                UInt32 ptr = MemReads.PCs.get_pointer(pcNameTB.Text, out pcIdx, out pcIdxAddr);
                                AppendText("PC Map Idx: " + pcIdx);
                                AppendText("\nPC IdxAddr: " + String.Format("{0:X}", pcIdxAddr));
                                AppendText("\nPC Pointer: " + String.Format("{0:X}", ptr));
                                AppendText("\nPC Name: " + MemReads.PCs.get_name(ptr));
                                AppendText("\nPC Pos X: " + String.Format("{0:0.0#}", MemReads.PCs.get_posx(ptr)));
                                AppendText("\nPC Pos Y: " + String.Format("{0:0.0#}", MemReads.PCs.get_posy(ptr)));
                                AppendText("\nPC Pos Z: " + String.Format("{0:0.0#}", MemReads.PCs.get_posz(ptr)));
                                AppendText("\nPC Pos H: " + String.Format("{0:0.0#}", MemReads.PCs.get_posh(ptr)));
                                AppendText("\nPC Dist: " + String.Format("{0:0.0#}", MemReads.PCs.get_distance(ptr)));
                                AppendText("\nPC HPP: " + MemReads.PCs.get_hp_perc(ptr));
                                AppendText("\nPC Status: " + MemReads.PCs.get_status(ptr));
                                AppendText("\nPC Active: " + MemReads.PCs.get_pointer_valid(ptr));
                            }
                            else
                            {
                                AppendText("\nEnter name in the top right text box.");
                            }
                            //while (OffsetComboBox.SelectedIndex == (int)OFFSET.PC_MAP)
                            //{
                            //    Thread.Sleep(1000);
                            //}
                            break;
                        #endregion PC Map
                        #region NPC Map
                        case (int)OFFSET.NPC_MAP:
                            AppendText("=====  Info NPC Map  =====\n");
                            Int16 id = npcInfo.ID;
                            MemReads.NPCs.get_NPCInfoStruct(ref npcInfo, id);

                            AppendText("\nName: '" + MemReads.NPCs.getName(npcInfo) + "'");
                            AppendText("\nAddr: 0x" + String.Format("{0:X}", (npcInfo.ID * 4 + MemReads.ProcessPointerList[MemReads.ProcessIndex].Info_MapNpcBegin)));
                            AppendText("\nID: " + npcInfo.ID + " (0x" + String.Format("{0:X}", npcInfo.ID) + ")");
                            AppendText("\nHPP: " + npcInfo.HPP);
                            AppendText("\nX: " + String.Format("{0:0.0#}", npcInfo.PosX));
                            AppendText("\nY: " + String.Format("{0:0.0#}", npcInfo.PosY));
                            AppendText("\nZ: " + String.Format("{0:0.0#}", npcInfo.PosZ));
                            AppendText("\nH: " + String.Format("{0:0.0#}", npcInfo.PosH));
                            Double distX = MemReads.Self.Position.get_x() - npcInfo.PosX;
                            Double distY = MemReads.Self.Position.get_y() - npcInfo.PosY;
                            Double distZ = MemReads.Self.Position.get_z() - npcInfo.PosZ;
                            Double dist = Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2) + Math.Pow(distZ, 2));
                            AppendText("\nDist: " + String.Format("{0:0.0#}", dist));
                            AppendText("\nActive: " + npcInfo.Active.ToString());
                            AppendText("\nCharmed: " + npcInfo.Charmed.ToString());
                            AppendText("\nFlag: " + npcInfo.Flag.ToString());
                            AppendText("\nInvisible: " + npcInfo.Invisible.ToString());
                            AppendText("\nSneak: " + npcInfo.Sneak.ToString());
                            String lastClaimStr = String.Format("{0:X}", npcInfo.LastClaimedID);
                            if (npcInfo.LastClaimedID != 0)
                            {
                                foreach(MemReads.NPCs.NPCInfoStruct npc in npcInfoList)
                                {
                                    if(npc.TargetCode == npcInfo.LastClaimedID)
                                    {
                                        lastClaimStr += " (" + MemReads.NPCs.getName(npc) + ")";
                                        break;
                                    }
                                }
                            }
                            AppendText("\nLast Claimed: " + lastClaimStr);
                            AppendText("\nModel: " + npcInfo.Model.ToString());
                            String petIdStr = String.Format("{0:X}", npcInfo.PetID);
                            if (npcInfo.PetID != 0)
                            {
                                MemReads.NPCs.NPCInfoStruct master = new MemReads.NPCs.NPCInfoStruct();
                                MemReads.NPCs.get_NPCInfoStruct(ref master, npcInfo.PetID);
                                petIdStr += " (" + MemReads.NPCs.getName(master) + ")";
                            }
                            AppendText("\nPedID: " + petIdStr);
                            AppendText("\nRace: " + npcInfo.Race.ToString());
                            AppendText("\nSpeed: " + String.Format("{0:0.#}", npcInfo.Speed));
                            AppendText("\nStatus: " + npcInfo.Status.ToString());
                            AppendText("\nTgtCode: 0x" + String.Format("{0:X}", npcInfo.TargetCode));
                            AppendText("\nTgtCode: " + npcInfo.TargetCode);
                            AppendText("\nType: " + npcInfo.Type.ToString());
                            break;
                        #endregion NPC Map
                        #region Party
                        case (int)OFFSET.PARTY:
                            AppendText("=====  Info Party  =====\n");
                            AppendText("\nParty member count: " + MemReads.Party.get_member_count());
                            if (MemReads.Party.get_member_count() > 1)
                            {
                                AppendText("\nShowing only the first party member:");
                                AppendText("\nName: " + MemReads.Party.get_member_name(1));
                                AppendText("\nHP/MP: " + MemReads.Party.get_member_hp(1) + " / " + MemReads.Party.get_member_mp(1));
                                AppendText("\nZone: " + MemReads.Party.get_member_zone(1));
                            }
                            break;
                        #endregion Party
                        #region Shop NPC
                        case (int)OFFSET.SHOP_NPC:
                            Dictionary<UInt16, UInt32> priceMap = null;
                            Dictionary<UInt16, UInt16> indexMap = null;
                            AppendText("=====  Info NPC Shop  =====\n");
                            MemReads.Windows.Shops.NPC.get_buy_id_to_price_map(ref priceMap, ref indexMap);
                            AppendText("\nNumber of items: " + priceMap.Count);
                            UInt16 curIdx = MemReads.Windows.Shops.ItemWindow.get_cur_idx();
                            AppendText("\nCurrent Item Index: " + curIdx);
                            foreach (KeyValuePair<UInt16, UInt32> kvp in priceMap)
                            {
                                AppendText("\nItem[" + kvp.Key + "] " + Things.GetNameFromId(kvp.Key) + " : " + kvp.Value);
                            }
                            break;
                        #endregion Shop NPC
                        #region Shop Guild
                        case (int)OFFSET.SHOP_GUILD:
                            AppendText("=====  Info Guild Shop  =====\n");
                            curIdx = MemReads.Windows.Shops.ItemWindow.get_cur_idx();
                            AppendText("\nCurrent Item Index: " + curIdx);
                            List<ushort> itemIds = MemReads.Windows.Shops.Guild.get_buy_item_ids();
                            int nbItems = itemIds.Count;
                            AppendText("\nNumber of items: " + nbItems);
                            for(int ii=0; ii<itemIds.Count; ii++)
                            {
                                AppendText("\nItem[" + itemIds[ii] + "] " + MemReads.Windows.Shops.Guild.get_buy_price((uint)ii) + " " + Things.GetNameFromId(itemIds[ii]) + ".");
                            }
                            break;
                        #endregion Shop Guild
                        #region Craft Window
                        case (int)OFFSET.CRAFT_WIN:
                            AppendText("=====  Info Craft Window  =====\n");
                            ushort itemID = 0;
                            byte quan = 0;
                            byte index = 0;
                            MemReads.Windows.Crafting.get_item(0, ref itemID, ref quan, ref index);
                            AppendText("\nItem 0: " + quan + " x " + itemID);
                            MemReads.Windows.Crafting.get_item(1, ref itemID, ref quan, ref index);
                            AppendText("\nItem 1: " + quan + " x " + itemID);
                            MemReads.Windows.Crafting.get_item(2, ref itemID, ref quan, ref index);
                            AppendText("\nItem 2: " + quan + " x " + itemID);
                            MemReads.Windows.Crafting.get_item(3, ref itemID, ref quan, ref index);
                            AppendText("\nItem 3: " + quan + " x " + itemID);
                            MemReads.Windows.Crafting.get_item(4, ref itemID, ref quan, ref index);
                            AppendText("\nItem 4: " + quan + " x " + itemID);
                            MemReads.Windows.Crafting.get_item(5, ref itemID, ref quan, ref index);
                            AppendText("\nItem 5: " + quan + " x " + itemID);
                            MemReads.Windows.Crafting.get_item(6, ref itemID, ref quan, ref index);
                            AppendText("\nItem 6: " + quan + " x " + itemID);
                            MemReads.Windows.Crafting.get_item(7, ref itemID, ref quan, ref index);
                            AppendText("\nItem 7: " + quan + " x " + itemID);
                            break;
                        #endregion Craft Window
                        #region NPC Trade Window
                        case (int)OFFSET.TRADE_WIN:
                            AppendText("=====  Info Trade Window  =====\n");
                            ushort tradeItemID = 0;
                            byte tradeItemQuan = 0;
                            byte tradeBagIndex = 0;
                            uint gilQuan = 0;
                            MemReads.Windows.Trading.NPC.get_item( 0, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 0: " + tradeItemQuan + " x " + tradeItemID);
                            MemReads.Windows.Trading.NPC.get_item(1, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 1: " + tradeItemQuan + " x " + tradeItemID);
                            MemReads.Windows.Trading.NPC.get_item(2, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 2: " + tradeItemQuan + " x " + tradeItemID);
                            MemReads.Windows.Trading.NPC.get_item(3, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 3: " + tradeItemQuan + " x " + tradeItemID);
                            MemReads.Windows.Trading.NPC.get_item(4, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 4: " + tradeItemQuan + " x " + tradeItemID);
                            MemReads.Windows.Trading.NPC.get_item(5, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 5: " + tradeItemQuan + " x " + tradeItemID);
                            MemReads.Windows.Trading.NPC.get_item(6, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 6: " + tradeItemQuan + " x " + tradeItemID);
                            MemReads.Windows.Trading.NPC.get_item(7, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 7: " + tradeItemQuan + " x " + tradeItemID);
                            MemReads.Windows.Trading.NPC.get_gil(ref gilQuan);
                            AppendText("\nGil: " + gilQuan);
                            break;
                        #endregion NPC Trade Window
                        #region PC Trade Window
                        case (int)OFFSET.TRADE_PC_WIN:
                            AppendText("=====  Info Trade Window  =====\n");
                            tradeItemID = 0;
                            tradeItemQuan = 0;
                            tradeBagIndex = 0;
                            MemReads.Windows.Trading.PC.get_item(0, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 0: " + tradeItemQuan + " x " + tradeItemID);
                            MemReads.Windows.Trading.PC.get_item(1, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 1: " + tradeItemQuan + " x " + tradeItemID);
                            MemReads.Windows.Trading.PC.get_item(2, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 2: " + tradeItemQuan + " x " + tradeItemID);
                            MemReads.Windows.Trading.PC.get_item(3, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 3: " + tradeItemQuan + " x " + tradeItemID);
                            MemReads.Windows.Trading.PC.get_item(4, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 4: " + tradeItemQuan + " x " + tradeItemID);
                            MemReads.Windows.Trading.PC.get_item(5, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 5: " + tradeItemQuan + " x " + tradeItemID);
                            MemReads.Windows.Trading.PC.get_item(6, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 6: " + tradeItemQuan + " x " + tradeItemID);
                            MemReads.Windows.Trading.PC.get_item(7, ref tradeItemID, ref tradeItemQuan, ref tradeBagIndex);
                            AppendText("\nItem 7: " + tradeItemQuan + " x " + tradeItemID);
                            break;
                        #endregion PC Trade Window
                        #region Spell Recast
                        case (int)OFFSET.SPELL_RECAST:
                            AppendText("=====  Info Spell Recast =====\n");
                            AppendText("\nIs Casting: " + MemReads.Self.Casting.is_casting());
                            foreach (Spells.SPELL_INFO info in spellInfoList)
                            {
                                UInt16 recast = MemReads.Self.Recast.Magic.get_time_remaining(info.ID);
                                if (recast != 0)
                                {
                                    AppendText("\n" + info.Name + " recast: " + recast);
                                }
                            }
                            break;
                        #endregion Spell Recast
                        #region Ability Recast
                        case (int)OFFSET.ABILITY_RECAST:
                            LoggingFunctions.Timestamp("Entering JA recast.");
                            AppendText("=====  Info Ability Recast =====\n");
                            List<JobAbilities.JA_INFO> jaInfoList = JobAbilities.GetAbilityInfo(job, subJob, jobLvl);
                            LoggingFunctions.Timestamp("Read info list from JA class. Has " + jaInfoList.Count + " items.");
                            LoggingFunctions.Timestamp("The jaRecastIdList has " + jaRecastIdList.Count + " items.");
                            for (int ii = 0; ii < jaRecastIdList.Count; ii++)
                            {
                                foreach (JobAbilities.JA_INFO info in jaInfoList)
                                {
                                    if (info.RecastID == jaRecastIdList[ii])
                                    {
                                        AppendText("\n[" + info.RecastID + "] " + info.Name + " = " + MemReads.Self.Recast.Abilities.get_time_remaining((Byte)ii));
                                    }
                                }
                            }
                            break;
                        #endregion Ability Recast
                        #region Navigation
                        case (int)OFFSET.NAVIGATION:
                            AppendText("=====  Navigation  ====\n");
                            AppendText("\nMy X: " + MemReads.Self.Position.get_x());
                            AppendText("\nMy Y: " + MemReads.Self.Position.get_y());
                            AppendText("\nMy Z: " + MemReads.Self.Position.get_z());
                            AppendText("\nTarget X: " + MemReads.Target.get_position_x());
                            AppendText("\nTarget Y: " + MemReads.Target.get_position_y());
                            AppendText("\nTarget Z: " + MemReads.Target.get_position_z());
                            AppendText("\nMy angle: " + MemReads.Self.Position.get_heading());
                            //AppendText("\nAngle to target: " + Navigation.AngleToTarget());
                            break;
                        #endregion Navigation
                        #region Time
                        case (int)OFFSET.TIME:
                            AppendText("=====  Time  ====\n");
                            if (!MemReads.PointersSet)
                            {
                                AppendText("\nPointers not set, can't display info.");
                                break;
                            }
                            else
                            {
                                //VanaTime.UseNtpTime = false;
                                VanaTime time = VanaTime.Now;
                                AppendText("\nTime from memory: " + MemReads.Environment.get_time());
                                AppendText("\nTime string: " + time.ToString());
                                AppendText("\nTime moon %: " + time.MoonPercent);
                                AppendText("\nTime moon ps: " + time.MoonPhase);
                                break;
                            }
                        #endregion Time
                        #region Network
                        case (int)OFFSET.NETWORK:
                            AppendText("=====  Network  ====\n");
                            AppendText("\nRcv: " + MemReads.Environment.info_network_receive());
                            AppendText("\nSnd: " + MemReads.Environment.info_network_send());
                            AppendText("\nPrc: " + MemReads.Environment.info_network_perc());
                            break;
                        #endregion Network
                        #region Menu Text-Style
                        case (int)OFFSET.MENU_TEXT:
                            AppendText("=====  Menu Info  ====\n");
                            List<String> strList = MemReads.Windows.Menus.TextStyle.get_items();
                            AppendText("\nCurrent index:   " + MemReads.Windows.Menus.TextStyle.get_curr_index());
                            AppendText("\nNumber of items: " + MemReads.Windows.Menus.TextStyle.get_item_count());
                            AppendText("\nMenu top text:   " + MemReads.Windows.Menus.TextStyle.get_top_text());
                            if (strList.Count == 0)
                            {
                                AppendText("\nNo Menu Items.");
                            }
                            else
                            {
                                for (int ii=0; ii<strList.Count; ii++)
                                {
                                    AppendText("\n[" + ii + "]: " + strList[ii]);
                                }
                            }
                            break;
                        #endregion Menu Text-Style
                        #region Menu Button-Style
                        case (int)OFFSET.MENU_BUTTON:
                            AppendText("=====  Menu Info  ====\n");
                            AppendText("\nCurrent index:   " + MemReads.Windows.Menus.ButtonStyle.get_curr_index());
                            //AppendText("\nNumber of items: " + MemReads.info_menu_btn_count());
                            break;
                        #endregion Menu Button-Style
                        #region AH
                        case (int)OFFSET.AH:
                            ushort l_list_count = MemReads.Windows.AH.get_list_count();
                            uint l_bid_amount = MemReads.Windows.AH.get_bid_price();
                            AppendText("=====  AH  ====\n");
                            AppendText("\nCurrent Bid:   " + MemReads.Windows.AH.get_bid_price());
                            AppendText("\nCurrent Cnt:   " + MemReads.Windows.AH.get_list_count());
                            AppendText("\nCurrent Unq:   " + MemReads.Windows.AH.get_list_unique_count());
                            List<MemReads.Windows.AH.Item> l_items = MemReads.Windows.AH.get_list_items();
                            if (l_items != null)
                            {
                                AppendText("\nItems:   ");
                                for (ushort ii = 0; ii < l_items.Count; ii++)
                                {
                                    string l_name = Things.GetNameFromId(l_items[ii].Id);
                                    string l_stackStr = "";
                                    if (l_items[ii].Stack)
                                    {
                                        l_stackStr += " x" + Things.GetStackSizeFromId(l_items[ii].Id);
                                    }
                                    AppendText("\n" + l_name + l_stackStr + "      [" + l_items[ii].NbAvailable + "]");
                                }
                            }
                            do
                            {
                                IocaineFunctions.delay(1000);
                            } while ((l_list_count == MemReads.Windows.AH.get_list_count()) && (l_bid_amount == MemReads.Windows.AH.get_bid_price()));

                            break;
                        #endregion AH
                        #region LUA Interface
                        case (int)OFFSET.LUA_INTF:
                            AppendText("=====  Info LUA Interface =====\n");
                            SocketServer.Start();
                            IocaineFunctions.keys("//lua reload eventFwd");
                            IocaineFunctions.delay(1000);
                            IocaineFunctions.keys("//lua invoke eventFwd setSocketPort " + SocketServer.Port.ToString());
                            AppendText("\n/echo Listening on port " + SocketServer.Port + "\n");
                            IocaineFunctions.delay(1000);
                            SocketServer._DataRecieved += AppendLine;
                            //IocaineFunctions.keys("//lua invoke eventFwd register_event \"gain buff\"");
                            //IocaineFunctions.keys("//lua invoke eventFwd register_event \"lose buff\"");
                            IocaineFunctions.keys("//lua invoke eventFwd register_event \"incoming chunk\"");
                            while (GetOffsetCBIndex() == (int)OFFSET.LUA_INTF)
                            {
                                ScrollToCaret();
                                Thread.Sleep(1000);
                            }
                            AppendText("\nStopping server.");
                            SocketServer.Stop();
                            AppendText("\nServer stopped.");
                            break;
                        #endregion LUA Interface
                        default:
                            AppendText("Error, unknown index selected.");
                            break;
                    }
                    Thread.Sleep(1000);
                }
            }
            else
            {
                MessageBox.Show("mainProc or mainMod was null.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }
        #endregion Main Function
        #region Event Handlers
        #region Process CB
        private void processCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("Changing index to " + processCB.SelectedIndex);
                mainProc = processList[processCB.SelectedIndex];
                mainMod = ProcessFunctions.GetMainModule(mainProc, "FFXiMain.dll");
                Iocaine_2_Form.mainProc = mainProc;
                Iocaine_2_Form.mainModule = mainMod;
                IocaineFunctions.setKbHelper(mainProc);
                MemReads.Set_FFXI_Pointers(mainProc, mainMod);
                job = MemReads.Self.Job.get_main();
                subJob = MemReads.Self.Job.get_sub();
                jobLvl = MemReads.Self.Job.get_main_lvl();
                FfxiResource.init();
                spellInfoList = Spells.GetSpellInfo(job, subJob, jobLvl);
                jaRecastIdList = MemReads.Self.Recast.Abilities.get_ability_indices();
            }
            catch (Exception ex)
            {
                Iocaine_2_Form.timestamp("Error changing process CB: " + ex.ToString());
            }
        }
        private void processCB_Click(object sender, EventArgs e)
        {
            processCB.BeginUpdate();
            processCB.Items.Clear();
            processList = ProcessFunctions.GetAllProcessByProcessName("pol");
            if (processList.Length == 0)
            {
                processCB.EndUpdate();
                return;
            }
            foreach (Process proc in processList)
            {
                processCB.Items.Add(proc.MainWindowTitle + proc.Id);
            }
            processCB.EndUpdate();
        }
        #endregion Process CB
        #region NPC CB
        private void NPCListCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 idx = NPCListCB.SelectedIndex;
            if (idx < npcInfoList.Count)
            {
                npcInfo = npcInfoList[idx];
            }
            if(OffsetCB.SelectedIndex != (Int32)OFFSET.NPC_MAP)
            {
                OffsetCB.SelectedIndex = (Int32)OFFSET.NPC_MAP;
            }
        }
        private void NPCListCB_Click(object sender, EventArgs e)
        {
            if(MemReads.PointersSet)
            {
                npcInfoList = MemReads.NPCs.get_NPCInfoStructList();
                NPCListCB.BeginUpdate();
                NPCListCB.Items.Clear();
                for(int ii=0; ii<npcInfoList.Count; ii++)
                {
                    MemReads.NPCs.NPCInfoStruct info = npcInfoList[ii];
                    NPCListCB.Items.Add(MemReads.NPCs.getName(info) + "[" + info.ID + "]");
                }
                NPCListCB.EndUpdate();
            }
        }
        #endregion NPC CB
        #region Inventory Check Boxes
        private void ShowSackChkB_CheckedChanged(object sender, EventArgs e)
        {
            showSackData = ShowSackChkB.Checked;
        }
        #endregion Inventory Check Boxes
        #region Buttons
        private void ManualRefreshButton_Click(object sender, EventArgs e)
        {
            refresh = true;
        }
        private void CommandBoxButton_Click(object sender, EventArgs e)
        {
            if (!commandExtended)
            {
                for (int ii = 0; ii < 20; ii++)
                {
                    this.Height += 4;
                    IocaineFunctions.delay(40);
                }
                CommandBoxButton.BackgroundImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                commandExtended = true;
            }
            else
            {
                for (int ii = 0; ii < 20; ii++)
                {
                    this.Height -= 4;
                    IocaineFunctions.delay(40);
                }
                CommandBoxButton.BackgroundImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                commandExtended = false;
            }
        }
        private void NPC_Item_Menu_Index_Button_Click(object sender, EventArgs e)
        {
            UInt16 idx = (UInt16)NPC_Item_Menu_Index_UpDn.Value;
            MemReads.Windows.Menus.ButtonStyle.set_curr_index(idx);
        }
        private void FisherNoArrowButton_Click(object sender, EventArgs e)
        {
            LoggingFunctions.Timestamp("No Arrows:");
            //printFisherBlocks();
        }
        private void FisherSilverLeftButton_Click(object sender, EventArgs e)
        {
            LoggingFunctions.Timestamp("Left Silver Arrow:");
            //printFisherBlocks();
        }
        private void FisherSilverRightButton_Click(object sender, EventArgs e)
        {
            LoggingFunctions.Timestamp("Right Silver Arrow:");
            //printFisherBlocks();
        }
        private void FisherGoldLeftButton_Click(object sender, EventArgs e)
        {
            LoggingFunctions.Timestamp("Left Gold Arrow:");
            //printFisherBlocks();
        }
        private void FisherGoldRightButton_Click(object sender, EventArgs e)
        {
            LoggingFunctions.Timestamp("Right Gold Arrow:");
            //printFisherBlocks();
        }
        private void printFisherBlocks()
        {
            //MemReads.logMemoryBlock(mainMod, MemReads.ProcessPointerList[MemReads.ProcessIndex].Info_Fishing - 64, MemReads.ProcessPointerList[MemReads.ProcessIndex].Info_Fishing + 128, 1, true);
            LoggingFunctions.Timestamp("Fishing Structure:");
            UIntPtr ptr = MemReads.Fishing.get_fishing_ptr();
            MemReads.logMemoryBlock(mainMod, (uint)ptr - 64, (uint)ptr + 128, 1, true);
        }
        #endregion Buttons
        #region Position Event Handlers
        private void PosX_UpDn_ValueChanged(object sender, EventArgs e)
        {
            posX = (float)PosX_UpDn.Value;
        }
        private void PosY_UpDn_ValueChanged(object sender, EventArgs e)
        {
            posY = (float)PosY_UpDn.Value;
        }
        private void PosZ_UpDn_ValueChanged(object sender, EventArgs e)
        {
            posZ = (float)PosZ_UpDn.Value;
        }
        private void PosH_UpDn_ValueChanged(object sender, EventArgs e)
        {
            posH = (float)PosH_UpDn.Value;
        }
        private void Get_Position_Button_Click(object sender, EventArgs e)
        {
            SetPlayerPointer();
            PosX_UpDn.Value = (decimal)MemReads.Self.Position.get_x();
            PosY_UpDn.Value = (decimal)MemReads.Self.Position.get_y();
            PosZ_UpDn.Value = (decimal)MemReads.Self.Position.get_z();
            PosH_UpDn.Value = (decimal)MemReads.Self.Position.get_heading();
        }
        private void Set_Position_Button_Click(object sender, EventArgs e)
        {
            SetPlayerPointer();
            MemReads.Self.Position.set_x((float)PosX_UpDn.Value, playerPtr);
            MemReads.Self.Position.set_y((float)PosY_UpDn.Value, playerPtr);
            MemReads.Self.Position.set_z((float)PosZ_UpDn.Value, playerPtr);
            MemReads.Self.Position.set_heading((float)PosH_UpDn.Value, playerPtr);
        }
        private void Reset_Pointers_Button_Click(object sender, EventArgs e)
        {
            playerPtr = MemReads.Self.get_player_struct_ptr();
            Player_Pointer_TB.Text = String.Format("{0:X}", playerPtr);
            playerPosPtr = MemReads.Self.Position.get_position_struct_ptr();
            Player_Position_Ptr_TB.Text = String.Format("{0:X}", playerPosPtr);

            Speed_UpDn.Value = (decimal)MemReads.Self.Speed.get_speed();
        }
        private void SetPlayerPointer()
        {
            if (playerPtr == 0)
            {
                playerPtr = MemReads.Self.get_player_struct_ptr();
                Player_Pointer_TB.Text = String.Format("{0:X}", playerPtr);
                playerPosPtr = MemReads.Self.Position.get_position_struct_ptr();
                Player_Position_Ptr_TB.Text = String.Format("{0:X}", playerPosPtr);
            }
        }
        private void Speed_Button_Click(object sender, EventArgs e)
        {
            if (speedEnabled == false)
            {
                speedNormal = MemReads.Self.Speed.get_speed();
                float speed = (float)Speed_UpDn.Value;
                MemReads.Self.Speed.set_speed(true, speed);
                Speed_Button.BackColor = Color.LimeGreen;
                speedEnabled = true;
            }
            else
            {
                MemReads.Self.Speed.set_speed(false, speedNormal);
                Speed_Button.BackColor = Color.Beige;
                speedEnabled = false;
            }
        }
        #endregion Position Event Handlers
        #region Synchronization
        public void SetProcessCBIndex(Int32 iIndex)
        {
            if (processCB.InvokeRequired)
            {
                processCB.Invoke(SetProcessCBIndexPtr, new object[] { iIndex });
            }
            else
            {
                SetProcessCBIndexCBF(iIndex);
            }
        }
        public void SetProcessCBIndexCBF(Int32 iIndex)
        {
            processCB.SelectedIndex = iIndex;
        }
        public void AddItemToProcessCB(String iItem)
        {
            if (processCB.InvokeRequired)
            {
                processCB.Invoke(AddItemToProcessCBPtr, new object[] { iItem });
            }
            else
            {
                AddItemToProcessCBCBF(iItem);
            }
        }
        public void AddItemToProcessCBCBF(String iItem)
        {
            processCB.Items.Add(iItem);
        }
        public void SetOffsetCBIndex(Int32 iIndex)
        {
            if (OffsetCB.InvokeRequired)
            {
                OffsetCB.Invoke(SetOffsetCBIndexPtr, new object[] { iIndex });
            }
            else
            {
                SetOffsetCBIndexCBF(iIndex);
            }
        }
        public void SetOffsetCBIndexCBF(Int32 iIndex)
        {
            OffsetCB.SelectedIndex = iIndex;
        }
        public void AddItemToOffsetCB(String iItem)
        {
            if (OffsetCB.InvokeRequired)
            {
                OffsetCB.Invoke(AddItemToOffsetCBPtr, new object[] { iItem });
            }
            else
            {
                AddItemToOffsetCBCBF(iItem);
            }
        }
        public void AddItemToOffsetCBCBF(String iItem)
        {
            OffsetCB.Items.Add(iItem);
        }
        public Int32 GetOffsetCBIndex()
        {
            if (OffsetCB.InvokeRequired)
            {
                return (Int32)OffsetCB.Invoke(GetOffsetCBIndexPtr);
            }
            else
            {
                return GetOffsetCBIndexCBF();
            }
        }
        public Int32 GetOffsetCBIndexCBF()
        {
            return OffsetCB.SelectedIndex;
        }
        public void AppendText(String iText)
        {
            if (MainTextBox.InvokeRequired)
            {
                MainTextBox.Invoke(AppendTextPtr, new object[] { iText });
            }
            else
            {
                AppendTextCBF(iText);
            }
        }
        public void AppendTextCBF(String iItem)
        {
            MainTextBox.AppendText(iItem);
        }
        public void AppendLine(String iText)
        {
            AppendText(iText + "\n");
        }
        public void ClearText()
        {
            if (MainTextBox.InvokeRequired)
            {
                MainTextBox.Invoke(ClearTextPtr);
            }
            else
            {
                ClearTextCBF();
            }
        }
        public void ClearTextCBF()
        {
            MainTextBox.Clear();
        }
        public void ScrollToCaret()
        {
            if (MainTextBox.InvokeRequired)
            {
                MainTextBox.Invoke(ScrollToCaretPtr);
            }
            else
            {
                ScrollToCaretCBF();
            }
        }
        public void ScrollToCaretCBF()
        {
            MainTextBox.ScrollToCaret();
        }
        #endregion Synchronization
        #endregion Event Handlers
    }
}
