using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Bots;
using Iocaine2.Data.Client;
using Iocaine2.Inventory;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Properties;
using Iocaine2.Settings;

namespace Iocaine2
{
    //This file is for functions directly related to the Seller
    partial class Iocaine_2_Form
    {
        #region Enums
        #endregion Enums
        #region Members
        #region Settings
        #endregion Settings
        #region Misc
        private static bool BY_doingInits = false;
        private static bool BY_itemAutoCompleteSetSinceLogin = false;
        private static bool BY_itemAutoCompleteLock = false;
        #endregion Misc
        #region DB
        private Buyer_Scripts_DS BY_scriptsDb;
        private List<Buyer_Script> BY_scriptList;
        #endregion DB
        #region Bot Data
        private List<ushort> BY_itemIdList = new List<ushort>();
        private List<String> BY_itemNameList = new List<String>();
        private List<ushort> BY_itemQuanList = new List<ushort>();
        private List<UInt32> BY_priceList = new List<uint>();
        private String BY_scriptLoadedName = "";
        private bool BY_scriptLoaded = false;
        private bool BY_scriptModified = false;
        #endregion Bot Data
        #region File I/O
        private String BY_scriptFilePath = ".\\Scripts\\";
        private String BY_scriptFileName = "Buyer_Scripts.xml";
        #endregion File I/O
        #region Time Related
        #endregion Time Related
        #region Default Values
        private String BY_npcNameTBDefText = "NPC Name";
        private String BY_itemNameTBDefText = "Item Name";
        private String BY_scriptNameTBDefText = "Script Name";
        private UInt16 BY_itemQuanUpDnDefValue = 1;
        private Byte BY_modeDefValue = 0;
        private UInt16 BY_combinedTotalQuanDefValue = 40;
        private Byte BY_percentageDefValue = 60;
        private UInt16 BY_leaveSlotsOpenQuanDefValue = 10;
        private bool BY_includeSatchelDefValue = true;
        private bool BY_includeSackDefValue = true;
        #endregion Default Values
        #region GUI Value Parallels
        private String BY_npcName = "";
        private String BY_itemName = "";
        private String BY_scriptName = "";
        private UInt16 BY_itemQuan = 1;
        private Byte BY_mode = 0;
        private UInt16 BY_combinedTotalQuan = 40;
        private Byte BY_percentageValue = 60;
        private UInt16 BY_leaveSlotsOpenQuan = 10;
        private UInt32 BY_pricePerItem = 4000;
        private bool BY_includeSatchel = true;
        private bool BY_includeSack = true;
        #endregion GUI Value Parallels
        #region Tool Tips
        #endregion Tool Tips
        #region Delegates
        private delegate void BY_setNpcNameTBTextDelegate(String iText);
        private delegate void BY_setItemNameTBTextDelegate(String iText);
        private delegate void BY_setScriptNameTBTextDelegate(String iText);
        private delegate void BY_setItemQuanUpDnValueDelegate(UInt16 iValue);
        private delegate void BY_setPricerPerItemUpDnValueDelegate(UInt32 iValue);
        private delegate void BY_setCombinedTotalRBValueDelegate(bool iChecked);
        private delegate void BY_setCombinedTotalUpDnValueDelegate(UInt16 iQuan);
        private delegate void BY_setExactQuanRBValueDelegate(bool iChecked);
        private delegate void BY_setPercentageRBValueDelegate(bool iChecked);
        private delegate void BY_setPercentageUpDnValueDelegate(Byte iPerc);
        private delegate void BY_setLeaveSlotsOpenRBValueDelegate(bool iChecked);
        private delegate void BY_setLeaveSlotsOpenUpDnValueDelegate(UInt16 iQuan);
        private delegate void BY_setIncludeSatchelChkBValueDelegate(bool iChecked);
        private delegate void BY_setIncludeSackChkBValueDelegate(bool iChecked);
        private delegate void BY_setGuildWaitTimeUpDnDelegate(UInt32 iValue);
        private delegate void BY_loadScriptCBDelegate();
        private delegate void BY_setStartButtonDelegate(String iText, Color iColor);
        #endregion Delegates
        #region Function Pointers
        private BY_setNpcNameTBTextDelegate BY_setNpcNameTBTextPtr;
        private BY_setItemNameTBTextDelegate BY_setItemNameTBTextPtr;
        private BY_setScriptNameTBTextDelegate BY_setScriptNameTBTextPtr;
        private BY_setItemQuanUpDnValueDelegate BY_setItemQuanUpDnValuePtr;
        private BY_setPricerPerItemUpDnValueDelegate BY_setPricePerItemUpDnValuePtr;
        private BY_setCombinedTotalRBValueDelegate BY_setCombinedTotalRBValuePtr;
        private BY_setCombinedTotalUpDnValueDelegate BY_setCombinedTotalUpDnValuePtr;
        private BY_setExactQuanRBValueDelegate BY_setExactQuanRBValuePtr;
        private BY_setPercentageRBValueDelegate BY_setPercentageRBValuePtr;
        private BY_setPercentageUpDnValueDelegate BY_setPercentageUpDnValuePtr;
        private BY_setLeaveSlotsOpenRBValueDelegate BY_setLeaveSlotsOpenRBValuePtr;
        private BY_setLeaveSlotsOpenUpDnValueDelegate BY_setLeaveSlotsOpenUpDnValuePtr;
        private BY_setIncludeSatchelChkBValueDelegate BY_setIncludeSatchelChkBValuePtr;
        private BY_setIncludeSackChkBValueDelegate BY_setIncludeSackChkBValuePtr;
        private BY_setGuildWaitTimeUpDnDelegate BY_setGuildWaitTimeUpDnPtr;
        private BY_setStartButtonDelegate BY_setStartButtonPtr;
        private BY_loadScriptCBDelegate BY_loadScriptCBPtr;
        #endregion Function Pointers
        #endregion Members
        #region Inits
        private void BY_Prc_inits()
        {
            BY_doingInits = true;
            BY_createDelegates();
            BY_initDb();
            BY_initLists();
            BY_loadDefaultValues();
            BY_LoadScriptDB();
            BY_LoadScriptsListFromDb();
            BY_loadScriptCB();
            BY_setNpcNameAutoComplete();
            Buyer.Init_Iocaine();
            BY_doingInits = false;
        }
        private void BY_Login_inits()
        {
            BY_doingInits = true;
            //SL_setItemAutoComplete();
            BY_loadUserSettings();
            BY_itemAutoCompleteSetSinceLogin = false;
            BY_doingInits = false;
        }
        private void BY_createDelegates()
        {
            if (BY_setNpcNameTBTextPtr == null)
            {
                BY_setNpcNameTBTextPtr = new BY_setNpcNameTBTextDelegate(BY_setNpcNameTBTextCallBackFunction);
                BY_setItemNameTBTextPtr = new BY_setItemNameTBTextDelegate(BY_setItemNameTBTextCallBackFunction);
                BY_setScriptNameTBTextPtr = new BY_setScriptNameTBTextDelegate(BY_setScriptNameTBTextCallBackFunction);
                BY_setItemQuanUpDnValuePtr = new BY_setItemQuanUpDnValueDelegate(BY_setItemQuanUpDnValueCallBackFunction);
                BY_setPricePerItemUpDnValuePtr = new BY_setPricerPerItemUpDnValueDelegate(BY_setPricePerItemUpDnValueCallBackFunction);
                BY_setCombinedTotalRBValuePtr = new BY_setCombinedTotalRBValueDelegate(BY_setCombinedTotalRBValueCallBackFunction);
                BY_setCombinedTotalUpDnValuePtr = new BY_setCombinedTotalUpDnValueDelegate(BY_setCombinedTotalUpDnValueCallBackFunction);
                BY_setExactQuanRBValuePtr = new BY_setExactQuanRBValueDelegate(BY_setExactQuanRBValueCallBackFunction);
                BY_setPercentageRBValuePtr = new BY_setPercentageRBValueDelegate(BY_setPercentageRBValueCallBackFunction);
                BY_setPercentageUpDnValuePtr = new BY_setPercentageUpDnValueDelegate(BY_setPercentageUpDnValueCallBackFunction);
                BY_setLeaveSlotsOpenRBValuePtr = new BY_setLeaveSlotsOpenRBValueDelegate(BY_setLeaveSlotsOpenRBValueCallBackFunction);
                BY_setLeaveSlotsOpenUpDnValuePtr = new BY_setLeaveSlotsOpenUpDnValueDelegate(BY_setLeaveSlotsOpenUpDnValueCallBackFunction);
                BY_setIncludeSatchelChkBValuePtr = new BY_setIncludeSatchelChkBValueDelegate(BY_setIncludeSatchelChkBValueCallBackFunction);
                BY_setIncludeSackChkBValuePtr = new BY_setIncludeSackChkBValueDelegate(BY_setIncludeSackChkBValueCallBackFunction);
                BY_setGuildWaitTimeUpDnPtr = new BY_setGuildWaitTimeUpDnDelegate(BY_setGuildWaitTimeUpDnCallBackFunction);
                BY_setStartButtonPtr = new BY_setStartButtonDelegate(BY_setStartButtonCallBackFunction);
                BY_loadScriptCBPtr = new BY_loadScriptCBDelegate(BY_loadScriptCBCallBackFunction);
                Statics.FuncPtrs.SetBuyerButtonPtr = new Statics.FuncPtrs.TD_Void_String_Color(BY_setStartButton);
            }
        }
        private void BY_initDb()
        {
            if (BY_scriptsDb == null)
            {
                BY_scriptsDb = new Buyer_Scripts_DS();
            }
            else
            {
                BY_scriptsDb.Clear();
            }
        }
        private void BY_initLists()
        {
            if (BY_scriptList == null)
            {
                BY_scriptList = new List<Buyer_Script>();
            }
            else
            {
                BY_scriptList.Clear();
            }
        }
        private void BY_loadUserSettings()
        {
            Statics.Settings.Helpers.WhenDoneSound_Buyer = (String)UserSettings.GetValue(UserSettings.BOT.HELPERS, "BY_WhenDoneSound");
            Statics.Settings.Helpers.GuildWaitTime_Buyer = (UInt32)UserSettings.GetValue(UserSettings.BOT.HELPERS, "BY_GuildWaitTime");
            Statics.Settings.Helpers.GuildSetIndex_Buyer = (bool)UserSettings.GetValue(UserSettings.BOT.HELPERS, "BY_GuildSetIndex");
            BY_setGuildWaitTimeUpDn(Statics.Settings.Helpers.GuildWaitTime_Buyer);
        }
        private void BY_loadDefaultValues()
        {
            BY_setNpcNameTBText(BY_npcNameTBDefText);
            BY_setItemNameTBText(BY_itemNameTBDefText);
            BY_setScriptNameTBText(BY_scriptNameTBDefText);
            BY_setItemQuanUpDnValue(BY_itemQuanUpDnDefValue);
            if (BY_modeDefValue == (Byte)Bots.Buyer.MODES.BUY_COMBINED_TOTAL)
            {
                BY_setCombinedTotalRBValue(true);
                BY_setExactQuanRBValue(false);
                BY_setPercentageRBValue(false);
                BY_setLeaveSlotsOpenRBValue(false);
            }
            else if (BY_modeDefValue == (Byte)Bots.Buyer.MODES.BUY_EXACT_QUAN)
            {
                BY_setCombinedTotalRBValue(false);
                BY_setExactQuanRBValue(true);
                BY_setPercentageRBValue(false);
                BY_setLeaveSlotsOpenRBValue(false);
            }
            else if (BY_modeDefValue == (Byte)Bots.Buyer.MODES.BUY_PERC_OPEN_SLOTS)
            {
                BY_setCombinedTotalRBValue(false);
                BY_setExactQuanRBValue(false);
                BY_setPercentageRBValue(true);
                BY_setLeaveSlotsOpenRBValue(false);
            }
            else if (BY_modeDefValue == (Byte)Bots.Buyer.MODES.LEAVE_NUMBER_SLOTS_OPEN)
            {
                BY_setCombinedTotalRBValue(false);
                BY_setExactQuanRBValue(false);
                BY_setPercentageRBValue(false);
                BY_setLeaveSlotsOpenRBValue(true);
            }
            BY_setCombinedTotalUpDnValue(BY_combinedTotalQuanDefValue);
            BY_setPercentageUpDnValue(BY_percentageDefValue);
            BY_setLeaveSlotsOpenUpDnValue(BY_leaveSlotsOpenQuanDefValue);
            BY_setIncludeSatchelChkBValue(BY_includeSatchelDefValue);
            BY_setIncludeSackChkBValue(BY_includeSackDefValue);
        }
        private void BY_setNpcNameAutoComplete()
        {
            BY_NPC_Name_TB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            BY_NPC_Name_TB.AutoCompleteSource = AutoCompleteSource.CustomSource;
            BY_NPC_Name_TB.AutoCompleteCustomSource = NPCs.NpcNameCollection;
        }
        private void BY_setItemAutoComplete()
        {
            if (BY_itemAutoCompleteSetSinceLogin)
            {
                return;
            }
            AutoCompleteStringCollection itemACList = Containers.ItemAutoCompleteList;
            BY_itemAutoCompleteLock = true;
            if (itemACList.Count > 0)
            {
                BY_Item_Name_TB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                BY_Item_Name_TB.AutoCompleteSource = AutoCompleteSource.CustomSource;
                BY_Item_Name_TB.AutoCompleteCustomSource = itemACList;
                BY_itemAutoCompleteSetSinceLogin = true;
            }
            else
            {
                BY_Item_Name_TB.AutoCompleteMode = AutoCompleteMode.None;
                BY_Item_Name_TB.AutoCompleteSource = AutoCompleteSource.None;
            }
            BY_itemAutoCompleteLock = false;
        }
        #endregion Inits
        #region Utility Functions
        #region GUI Updates
        #region Text Box Updates
        private void BY_setNpcNameTBText(String iText)
        {
            try
            {
                if (BY_NPC_Name_TB.InvokeRequired)
                {
                    BY_NPC_Name_TB.Invoke(BY_setNpcNameTBTextPtr, new object[] { iText });
                }
                else
                {
                    BY_setNpcNameTBTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setNpcNameTBText: " + e.ToString());
            }
        }
        private void BY_setNpcNameTBTextCallBackFunction(String iText)
        {
            BY_NPC_Name_TB.Text = iText;
            if (iText != BY_npcNameTBDefText)
            {
                BY_NPC_Name_TB.ForeColor = Color.Black;
            }
            else
            {
                BY_NPC_Name_TB.ForeColor = Color.Gray;
            }
        }
        private void BY_setItemNameTBText(String iText)
        {
            try
            {
                if (BY_Item_Name_TB.InvokeRequired)
                {
                    BY_Item_Name_TB.Invoke(BY_setItemNameTBTextPtr, new object[] { iText });
                }
                else
                {
                    BY_setItemNameTBTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setItemNameTBText: " + e.ToString());
            }
        }
        private void BY_setItemNameTBTextCallBackFunction(String iText)
        {
            BY_Item_Name_TB.Text = iText;
            if (iText != BY_itemNameTBDefText)
            {
                BY_Item_Name_TB.ForeColor = Color.Black;
            }
            else
            {
                BY_Item_Name_TB.ForeColor = Color.Gray;
            }
        }
        private void BY_setScriptNameTBText(String iText)
        {
            try
            {
                if (BY_Script_Name_TB.InvokeRequired)
                {
                    BY_Script_Name_TB.Invoke(BY_setScriptNameTBTextPtr, new object[] { iText });
                }
                else
                {
                    BY_setScriptNameTBTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setScriptNameTBText: " + e.ToString());
            }
        }
        private void BY_setScriptNameTBTextCallBackFunction(String iText)
        {
            BY_Script_Name_TB.Text = iText;
            if (iText != BY_scriptNameTBDefText)
            {
                BY_Script_Name_TB.ForeColor = Color.Black;
            }
            else
            {
                BY_Script_Name_TB.ForeColor = Color.Gray;
            }
        }
        #endregion Text Box Updates
        #region UpDown Updates
        private void BY_setItemQuanUpDnValue(UInt16 iValue)
        {
            try
            {
                if (BY_Item_Quan_UpDn.InvokeRequired)
                {
                    BY_Item_Quan_UpDn.Invoke(BY_setItemQuanUpDnValuePtr, new object[] { iValue });
                }
                else
                {
                    BY_setItemQuanUpDnValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setItemQuanUpDnValue: " + e.ToString());
            }
        }
        private void BY_setItemQuanUpDnValueCallBackFunction(UInt16 iValue)
        {
            BY_Item_Quan_UpDn.Value = (decimal)iValue;
        }
        private void BY_setCombinedTotalUpDnValue(UInt16 iValue)
        {
            try
            {
                if (BY_Combined_Total_UpDn.InvokeRequired)
                {
                    BY_Combined_Total_UpDn.Invoke(BY_setCombinedTotalUpDnValuePtr, new object[] { iValue });
                }
                else
                {
                    BY_setCombinedTotalUpDnValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setCombinedTotalUpDnValue: " + e.ToString());
            }
        }
        private void BY_setCombinedTotalUpDnValueCallBackFunction(UInt16 iValue)
        {
            BY_Combined_Total_UpDn.Value = (decimal)iValue;
        }
        private void BY_setPercentageUpDnValue(Byte iValue)
        {
            try
            {
                if (BY_Percentage_UpDn.InvokeRequired)
                {
                    BY_Percentage_UpDn.Invoke(BY_setPercentageUpDnValuePtr, new object[] { iValue });
                }
                else
                {
                    BY_setPercentageUpDnValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setPercentageUpDnValue: " + e.ToString());
            }
        }
        private void BY_setPercentageUpDnValueCallBackFunction(Byte iValue)
        {
            BY_Percentage_UpDn.Value = (decimal)iValue;
        }
        private void BY_setLeaveSlotsOpenUpDnValue(UInt16 iValue)
        {
            try
            {
                if (BY_Leave_Slots_Open_UpDn.InvokeRequired)
                {
                    BY_Leave_Slots_Open_UpDn.Invoke(BY_setLeaveSlotsOpenUpDnValuePtr, new object[] { iValue });
                }
                else
                {
                    BY_setLeaveSlotsOpenUpDnValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setLeaveSlotsOpenUpDnValue: " + e.ToString());
            }
        }
        private void BY_setLeaveSlotsOpenUpDnValueCallBackFunction(UInt16 iValue)
        {
            BY_Leave_Slots_Open_UpDn.Value = (decimal)iValue;
        }
        private void BY_setGuildWaitTimeUpDn(UInt32 iValue)
        {
            try
            {
                if (BY_Guild_Wait_Time_UpDn.InvokeRequired)
                {
                    BY_Guild_Wait_Time_UpDn.Invoke(BY_setGuildWaitTimeUpDnPtr, new object[] { iValue });
                }
                else
                {
                    BY_setGuildWaitTimeUpDnCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setGuildWaitTimeUpDnValue: " + e.ToString());
            }
        }
        private void BY_setGuildWaitTimeUpDnCallBackFunction(UInt32 iValue)
        {
            BY_Guild_Wait_Time_UpDn.Value = (decimal)iValue;
        }
        private void BY_setPricePerItemUpDnValue(UInt32 iValue)
        {
            try
            {
                if (BY_Price_Per_Item_UpDn.InvokeRequired)
                {
                    BY_Price_Per_Item_UpDn.Invoke(BY_setPricePerItemUpDnValuePtr, new object[] { iValue });
                }
                else
                {
                    BY_setPricePerItemUpDnValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setPricePerItemUpDnValue: " + e.ToString());
            }
        }
        private void BY_setPricePerItemUpDnValueCallBackFunction(UInt32 iValue)
        {
            BY_Price_Per_Item_UpDn.Value = (decimal)iValue;
        }
        #endregion UpDown Updates
        #region Combo Box Updates
        private void BY_loadScriptCB()
        {
            try
            {

                if (BY_Script_CB.InvokeRequired)
                {
                    BY_Script_CB.Invoke(BY_loadScriptCBPtr);
                }
                else
                {
                    BY_loadScriptCBCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_loadScriptCB: " + e.ToString());
            }
        }
        private void BY_loadScriptCBCallBackFunction()
        {
            BY_Script_CB.Items.Clear();
            foreach (Buyer_Script script in BY_scriptList)
            {
                BY_Script_CB.Items.Add(script.ScriptName);
            }
        }
        #endregion Combo Box Updates
        #region Radio Button Updates
        private void BY_setCombinedTotalRBValue(bool iValue)
        {
            try
            {
                if (BY_Combined_Total_RB.InvokeRequired)
                {
                    BY_Combined_Total_RB.Invoke(BY_setCombinedTotalRBValuePtr, new object[] { iValue });
                }
                else
                {
                    BY_setCombinedTotalRBValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setCombinedTotalRBValue: " + e.ToString());
            }
        }
        private void BY_setCombinedTotalRBValueCallBackFunction(bool iValue)
        {
            BY_Combined_Total_RB.Checked = iValue;
        }
        private void BY_setExactQuanRBValue(bool iValue)
        {
            try
            {
                if (BY_Exact_Quan_RB.InvokeRequired)
                {
                    BY_Exact_Quan_RB.Invoke(BY_setExactQuanRBValuePtr, new object[] { iValue });
                }
                else
                {
                    BY_setExactQuanRBValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setExactQuanRBValue: " + e.ToString());
            }
        }
        private void BY_setExactQuanRBValueCallBackFunction(bool iValue)
        {
            BY_Exact_Quan_RB.Checked = iValue;
        }
        private void BY_setPercentageRBValue(bool iValue)
        {
            try
            {
                if (BY_Percentage_RB.InvokeRequired)
                {
                    BY_Percentage_RB.Invoke(BY_setPercentageRBValuePtr, new object[] { iValue });
                }
                else
                {
                    BY_setPercentageRBValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setPercentageRBValue: " + e.ToString());
            }
        }
        private void BY_setPercentageRBValueCallBackFunction(bool iValue)
        {
            BY_Percentage_RB.Checked = iValue;
        }
        private void BY_setLeaveSlotsOpenRBValue(bool iValue)
        {
            try
            {
                if (BY_Leave_Slots_Open_RB.InvokeRequired)
                {
                    BY_Leave_Slots_Open_RB.Invoke(BY_setLeaveSlotsOpenRBValuePtr, new object[] { iValue });
                }
                else
                {
                    BY_setLeaveSlotsOpenRBValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setLeaveSlotsOpenRBValue: " + e.ToString());
            }
        }
        private void BY_setLeaveSlotsOpenRBValueCallBackFunction(bool iValue)
        {
            BY_Leave_Slots_Open_RB.Checked = iValue;
        }
        #endregion Radio Button Updates
        #region Check Box Updates
        private void BY_setIncludeSatchelChkBValue(bool iValue)
        {
            try
            {
                if (BY_Include_Satchel_ChkB.InvokeRequired)
                {
                    BY_Include_Satchel_ChkB.Invoke(BY_setIncludeSatchelChkBValuePtr, new object[] { iValue });
                }
                else
                {
                    BY_setIncludeSatchelChkBValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setIncludeSatchelChkBValue: " + e.ToString());
            }
        }
        private void BY_setIncludeSatchelChkBValueCallBackFunction(bool iValue)
        {
            BY_Include_Satchel_ChkB.Checked = iValue;
        }
        private void BY_setIncludeSackChkBValue(bool iValue)
        {
            try
            {
                if (BY_Include_Sack_ChkB.InvokeRequired)
                {
                    BY_Include_Sack_ChkB.Invoke(BY_setIncludeSackChkBValuePtr, new object[] { iValue });
                }
                else
                {
                    BY_setIncludeSackChkBValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setIncludeSackChkBValue: " + e.ToString());
            }
        }
        private void BY_setIncludeSackChkBValueCallBackFunction(bool iValue)
        {
            BY_Include_Sack_ChkB.Checked = iValue;
        }
        #endregion Check Box Updates
        #region Button Updates
        private void BY_setStartButton(String iText, Color iColor)
        {
            try
            {
                if (BY_Start_Button.InvokeRequired)
                {
                    BY_Start_Button.Invoke(BY_setStartButtonPtr, new object[] { iText, iColor });
                }
                else
                {
                    BY_setStartButtonCallBackFunction(iText, iColor);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_setStartButton: " + e.ToString());
            }
        }
        private void BY_setStartButtonCallBackFunction(String iText, Color iColor)
        {
            BY_Start_Button.Text = iText;
            BY_Start_Button.BackColor = iColor;
        }
        #endregion Button Updates
        #endregion GUI Updates
        #region Script Related
        #region File I/O
        private void BY_LoadScriptDB()
        {
            if (Directory.Exists(BY_scriptFilePath))
            {
                if (File.Exists(BY_scriptFilePath + BY_scriptFileName))
                {
                    try
                    {
                        if (BY_scriptsDb == null)
                        {
                            BY_initDb();
                        }
                        BY_scriptsDb.ReadXml(BY_scriptFilePath + BY_scriptFileName);
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("In BY_loadScriptDB: " + e.ToString());
                    }
                }
            }
        }
        private void BY_LoadScriptsListFromDb()
        {
            Buyer_Scripts_DS.Script_TableRow[] scriptRows = (Buyer_Scripts_DS.Script_TableRow[])BY_scriptsDb.Script_Table.Select();
            foreach (Buyer_Scripts_DS.Script_TableRow row in scriptRows)
            {
                Buyer_Script script = BY_CreateScript(row);
                BY_scriptList.Add(script);
            }
        }
        private void BY_SaveScriptDB()
        {
            if (!Directory.Exists(BY_scriptFilePath))
            {
                try
                {
                    Directory.CreateDirectory(BY_scriptFilePath);
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("In BY_saveScriptDB creating directory: " + e.ToString());
                    return;
                }
            }
            try
            {
                if (BY_scriptsDb != null)
                {
                    BY_scriptsDb.WriteXml(BY_scriptFilePath + BY_scriptFileName);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In BY_saveScriptDB writing file: " + e.ToString());
            }
        }
        #endregion File I/O
        #region Script Loading
        private void BY_LoadScript(String iScriptName)
        {
            foreach (Buyer_Script script in BY_scriptList)
            {
                if (script.ScriptName == iScriptName)
                {
                    //We found the script in the list that we want to load.
                    //Now go through all of the controls and set their values.
                    BY_setScriptNameTBText(iScriptName);
                    BY_setNpcNameTBText(script.NpcName);
                    if (script.Mode == (Byte)Bots.Buyer.MODES.BUY_COMBINED_TOTAL)
                    {
                        BY_setCombinedTotalRBValue(true);
                        BY_setExactQuanRBValue(false);
                        BY_setPercentageRBValue(false);
                        BY_setLeaveSlotsOpenRBValue(false);
                    }
                    else if (script.Mode == (Byte)Bots.Buyer.MODES.BUY_PERC_OPEN_SLOTS)
                    {
                        BY_setCombinedTotalRBValue(false);
                        BY_setExactQuanRBValue(false);
                        BY_setPercentageRBValue(true);
                        BY_setLeaveSlotsOpenRBValue(false);
                    }
                    else if (script.Mode == (Byte)Bots.Buyer.MODES.LEAVE_NUMBER_SLOTS_OPEN)
                    {
                        BY_setCombinedTotalRBValue(false);
                        BY_setExactQuanRBValue(false);
                        BY_setPercentageRBValue(false);
                        BY_setLeaveSlotsOpenRBValue(true);
                    }
                    else
                    {
                        BY_setCombinedTotalRBValue(false);
                        BY_setExactQuanRBValue(true);
                        BY_setPercentageRBValue(false);
                        BY_setLeaveSlotsOpenRBValue(false);
                    }
                    BY_setCombinedTotalUpDnValue(script.CombinedTotalQuan);
                    BY_setPercentageUpDnValue(script.PercentageValue);
                    BY_setLeaveSlotsOpenUpDnValue(script.LeaveOpenSlotsQuan);
                    BY_setIncludeSackChkBValue(script.IncludeSack);
                    BY_setIncludeSatchelChkBValue(script.IncludeSatchel);
                    BY_Item_LB.Items.Clear();
                    BY_itemIdList.Clear();
                    BY_itemNameList.Clear();
                    BY_itemQuanList.Clear();
                    BY_priceList.Clear();
                    List<Item> itemList = script.ItemList;
                    List<ushort> quanList = script.ItemQuanList;
                    List<UInt32> priceList = script.PricePerItemList;
                    for (int ii = 0; ii < itemList.Count; ii++)
                    {
                        BY_Item_LB.Items.Add(BY_GetItemListBoxString(itemList[ii].Name, quanList[ii], priceList[ii]));
                        BY_itemIdList.Add(itemList[ii].ItemID);
                        BY_itemNameList.Add(itemList[ii].Name);
                        BY_itemQuanList.Add(quanList[ii]);
                        BY_priceList.Add(priceList[ii]);
                    }
                    BY_scriptLoaded = true;
                    BY_scriptLoadedName = iScriptName;
                    return;
                }
            }
        }
        #endregion Script Loading
        #region Script Deletion
        private void BY_RemoveScript(String iScriptName)
        {
            String filterString = "ScriptName='" + iScriptName + "'";
            Buyer_Scripts_DS.Script_TableRow[] scriptRows = (Buyer_Scripts_DS.Script_TableRow[])BY_scriptsDb.Script_Table.Select(filterString);
            int nbRows = scriptRows.Length;
            for (int ii = nbRows - 1; ii >= 0; ii--)
            {
                BY_scriptsDb.Script_Table.Rows.RemoveAt(ii);
            }

            Buyer_Scripts_DS.Item_TableRow[] itemRows = (Buyer_Scripts_DS.Item_TableRow[])BY_scriptsDb.Item_Table.Select(filterString);
            nbRows = itemRows.Length;
            for (int ii = nbRows - 1; ii >= 0; ii--)
            {
                BY_scriptsDb.Item_Table.Rows.RemoveAt(ii);
            }
            int nbScripts = BY_scriptList.Count;
            for (int kk = nbScripts - 1; kk >= 0; kk--)
            {
                if (BY_scriptList[kk].ScriptName == iScriptName)
                {
                    BY_scriptList.RemoveAt(kk);
                }
            }
        }
        #endregion Script Deletion
        #region Script Saving
        private void BY_AddScript()
        {
            Buyer_Script script = BY_CreateScript();
            if (script != null)
            {
                BY_scriptList.Add(script);
            }
            BY_AddScriptToDb(script);
        }
        private Buyer_Script BY_CreateScript()
        {
            if (BY_scriptName.Contains("'"))
            {
                MessageBox.Show("Script name cannot contain an apostrophe.");
                return null;
            }
            Buyer_Script script = new Buyer_Script();
            script.ScriptName = BY_scriptName;
            script.NpcName = BY_npcName;
            script.Mode = BY_mode;
            script.CombinedTotalQuan = BY_combinedTotalQuan;
            script.PercentageValue = BY_percentageValue;
            script.LeaveOpenSlotsQuan = BY_leaveSlotsOpenQuan;
            script.IncludeSatchel = BY_includeSatchel;
            script.IncludeSack = BY_includeSack;
            int nbItems = BY_itemNameList.Count;
            for (int ii = 0; ii < nbItems; ii++)
            {
                script.AddItem(BY_itemNameList[ii], BY_itemIdList[ii], BY_itemQuanList[ii], BY_priceList[ii]);
            }
            return script;
        }
        private Buyer_Script BY_CreateScript(Buyer_Scripts_DS.Script_TableRow iRow)
        {
            //Create a script that already exists in the database.
            Buyer_Script script = new Buyer_Script();
            script.ScriptName = iRow.ScriptName;
            script.NpcName = iRow.NpcName;
            script.Mode = iRow.Mode;
            script.CombinedTotalQuan = iRow.CombinedTotalQuan;
            script.PercentageValue = iRow.PercentageValue;
            script.LeaveOpenSlotsQuan = iRow.LeaveOpenSlotsQuan;
            script.IncludeSatchel = iRow.IncludeSatchel;
            script.IncludeSack = iRow.IncludeSack;

            String filterString = "ScriptName = '" + iRow.ScriptName + "'";
            Buyer_Scripts_DS.Item_TableRow[] itemRows = (Buyer_Scripts_DS.Item_TableRow[])BY_scriptsDb.Item_Table.Select(filterString);
            foreach (Buyer_Scripts_DS.Item_TableRow itemRow in itemRows)
            {
                script.AddItem(new Item(itemRow.ItemName, itemRow.ItemId, Things.ITEM_TYPE.UNKNOWN), itemRow.ItemQuan, itemRow.ItemPrice);
            }
            return script;
        }
        private void BY_UpdateScript()
        {
            //To update a script we need to do the following:
            //1. Remove all traces of the script from the DB (item table and script table).
            //2. Remove the script object from the scripts list.
            //3. Create a new script based on what's loaded.
            //4. Add the new script to the DB.
            if (BY_scriptName.Contains("'"))
            {
                MessageBox.Show("Script name cannot contain an apostrophe.");
                return;
            }
            BY_RemoveScript(BY_scriptLoadedName);
            BY_AddScript();
        }
        private void BY_AddScriptToDb(Buyer_Script iScript)
        {
            Buyer_Scripts_DS.Script_TableRow scriptRow = BY_scriptsDb.Script_Table.NewScript_TableRow();
            scriptRow.ScriptName = iScript.ScriptName;
            scriptRow.NpcName = iScript.NpcName;
            scriptRow.Mode = iScript.Mode;
            scriptRow.CombinedTotalQuan = iScript.CombinedTotalQuan;
            scriptRow.PercentageValue = iScript.PercentageValue;
            scriptRow.LeaveOpenSlotsQuan = iScript.LeaveOpenSlotsQuan;
            scriptRow.IncludeSatchel = iScript.IncludeSatchel;
            scriptRow.IncludeSack = iScript.IncludeSack;
            BY_scriptsDb.Script_Table.Rows.Add(scriptRow);

            List<Item> itemList = iScript.ItemList;
            List<ushort> itemQuanList = iScript.ItemQuanList;
            List<UInt32> priceList = iScript.PricePerItemList;
            int nbItems = itemList.Count;
            for (int ii = 0; ii < nbItems; ii++)
            {
                Buyer_Scripts_DS.Item_TableRow itemRow = BY_scriptsDb.Item_Table.NewItem_TableRow();
                itemRow.ScriptName = iScript.ScriptName;
                itemRow.ItemName = itemList[ii].Name;
                itemRow.ItemId = itemList[ii].ItemID;
                itemRow.ItemQuan = itemQuanList[ii];
                itemRow.ItemPrice = priceList[ii];
                BY_scriptsDb.Item_Table.Rows.Add(itemRow);
            }
        }
        #endregion Script Saving
        #endregion Script Related
        #region Parsing
        private String BY_GetItemListBoxString(String iName, ushort iQuan, UInt32 iPrice)
        {
            if ((iName != "") && (iName != BY_itemNameTBDefText))
            {
                return (iName + " (" + iQuan + ") [" + iPrice + "]");
            }
            else
            {
                return "";
            }
        }
        private String BY_GetItemListBoxString()
        {
            if ((BY_itemName != "") && (BY_itemName != BY_itemNameTBDefText))
            {
                return BY_GetItemListBoxString(BY_itemName, BY_itemQuan, BY_pricePerItem);
            }
            else
            {
                return "";
            }
        }
        private String BY_ParseItemName(String iText)
        {
            int parenIdx = iText.IndexOf('(');
            if (parenIdx <= 1)
            {
                LoggingFunctions.Error("Could not parse item name from '" + iText + "'");
                return "[ERROR]";
            }
            else
            {
                return iText.Substring(0, parenIdx - 1);
            }
        }
        private ushort BY_ParseItemQuan(String iText)
        {
            int parenIdxOpen = iText.IndexOf('(');
            int parenIdxClose = iText.IndexOf(')');
            if ((parenIdxOpen <= 1) || (parenIdxClose <= 2))
            {
                LoggingFunctions.Error("Could not parse item quantity from '" + iText + "'");
                return 0xffff;
            }
            else
            {
                String quanString = iText.Substring(parenIdxOpen + 1, parenIdxClose - parenIdxOpen - 1);
                ushort parseData = 0;
                bool parseResult = ushort.TryParse(quanString, out parseData);
                if (parseResult)
                {
                    return parseData;
                }
                else
                {
                    LoggingFunctions.Error("Could not parse item quantity from '" + iText + "'");
                    return 0xffff;
                }
            }
        }
        private UInt32 BY_ParseItemPrice(String iText)
        {
            int bracketIdxOpen = iText.IndexOf('[');
            int bracketIdxClose = iText.IndexOf(']');
            if ((bracketIdxOpen <= 1) || (bracketIdxClose <= 2))
            {
                LoggingFunctions.Error("Could not parse item price from '" + iText + "'");
                return 0xffffffff;
            }
            else
            {
                String priceString = iText.Substring(bracketIdxOpen + 1, bracketIdxClose - bracketIdxOpen - 1);
                UInt32 parseData = 0;
                bool parseResult = UInt32.TryParse(priceString, out parseData);
                if (parseResult)
                {
                    return parseData;
                }
                else
                {
                    LoggingFunctions.Error("Could not parse item price from '" + iText + "'");
                    return 0xffffffff;
                }
            }
        }
        #endregion Parsing
        #endregion Utility Functions
        #region Processing
        private void BY_RunScript(Buyer_Script iScript)
        {
            if (iScript.ItemList.Count == 0)
            {
                MessageBox.Show("Please add at least 1 item to the list.");
                return;
            }
            else if ((iScript.NpcName == "") || (iScript.NpcName == BY_npcNameTBDefText))
            {
                MessageBox.Show("Please enter an NPC name to sell to.");
                return;
            }
            Bots.Buyer.Start(iScript, Statics.Settings.Helpers.WhenDoneSound_Buyer);
        }
        private void BY_RunScript()
        {
            if (BY_scriptLoaded && !BY_scriptModified)
            {
                foreach (Buyer_Script scr in BY_scriptList)
                {
                    if (scr.ScriptName == BY_scriptLoadedName)
                    {
                        BY_RunScript(scr);
                    }
                }
            }
            else
            {
                Buyer_Script scr2 = BY_CreateScript();
                if (scr2 != null)
                {
                    BY_RunScript(scr2);
                }
            }
        }
        #endregion Processing
        #region Event Handlers
        #region Text Boxes
        #region NPC Name TB
        private void BY_NPC_Name_TB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                if (ChangeMonitor.LoggedIn == false)
                {
                    return;
                }
                String targetName = MemReads.Target.get_name();
                if ((BY_NPC_Name_TB.Text == "") && (targetName != ""))
                {
                    BY_NPC_Name_TB.Text = targetName;
                }
            }
        }
        private void BY_NPC_Name_TB_TextChanged(object sender, EventArgs e)
        {
            BY_npcName = BY_NPC_Name_TB.Text;
            if (BY_scriptLoaded)
            {
                BY_scriptModified = true;
            }
        }
        private void BY_NPC_Name_TB_Enter(object sender, EventArgs e)
        {
            if (BY_NPC_Name_TB.Text == BY_npcNameTBDefText)
            {
                BY_NPC_Name_TB.Text = "";
                BY_NPC_Name_TB.ForeColor = Color.Black;
            }
        }
        private void BY_NPC_Name_TB_Leave(object sender, EventArgs e)
        {
            if (BY_NPC_Name_TB.Text == "")
            {
                BY_NPC_Name_TB.Text = BY_npcNameTBDefText;
                BY_NPC_Name_TB.ForeColor = Color.Gray;
            }
            else
            {
                String userText = BY_NPC_Name_TB.Text;
                BY_NPC_Name_TB.Text = userText.Substring(0, 1).ToUpper() + userText.Substring(1, userText.Length - 1).ToLower();
            }
        }
        #endregion NPC Name TB
        #region Item Name TB
        private void BY_Item_Name_TB_TextChanged(object sender, EventArgs e)
        {
            BY_itemName = BY_Item_Name_TB.Text;
        }
        private void BY_Item_Name_TB_Enter(object sender, EventArgs e)
        {
            if (BY_itemAutoCompleteLock)
            {
                return;
            }
            if (BY_Item_Name_TB.Text == BY_itemNameTBDefText)
            {
                BY_setItemAutoComplete();
                BY_Item_Name_TB.Text = "";
                BY_Item_Name_TB.ForeColor = Color.Black;
            }
        }
        private void BY_Item_Name_TB_Leave(object sender, EventArgs e)
        {
            if (BY_Item_Name_TB.Text == "")
            {
                BY_Item_Name_TB.Text = BY_itemNameTBDefText;
                BY_Item_Name_TB.ForeColor = Color.Gray;
            }
            else
            {
                String itemText = BY_Item_Name_TB.Text;
                itemText = itemText.Substring(0, 1).ToUpper() + itemText.Substring(1, itemText.Length - 1);
                BY_Item_Name_TB.Text = itemText;
            }
        }
        #endregion Item Name TB
        #region Script Name TB
        private void BY_Script_Name_TB_TextChanged(object sender, EventArgs e)
        {
            BY_scriptName = BY_Script_Name_TB.Text;
            if (BY_scriptLoaded)
            {
                BY_scriptModified = true;
            }
        }
        private void BY_Script_Name_TB_Enter(object sender, EventArgs e)
        {
            if (BY_Script_Name_TB.Text == BY_scriptNameTBDefText)
            {
                BY_Script_Name_TB.Text = "";
                BY_Script_Name_TB.ForeColor = Color.Black;
            }
        }
        private void BY_Script_Name_TB_Leave(object sender, EventArgs e)
        {
            if (BY_Script_Name_TB.Text == "")
            {
                BY_Script_Name_TB.Text = BY_scriptNameTBDefText;
                BY_Script_Name_TB.ForeColor = Color.Gray;
            }
        }
        #endregion Script Name TB
        #endregion Text Boxes
        #region Radio Buttons
        private void BY_Combined_Total_RB_CheckedChanged(object sender, EventArgs e)
        {
            if (BY_doingInits)
            {
                return;
            }
            if (BY_Combined_Total_RB.Checked)
            {
                BY_mode = (Byte)Bots.Buyer.MODES.BUY_COMBINED_TOTAL;
            }
        }
        private void BY_Exact_Quan_RB_CheckedChanged(object sender, EventArgs e)
        {
            if (BY_doingInits)
            {
                return;
            }
            if (BY_Exact_Quan_RB.Checked)
            {
                BY_mode = (Byte)Bots.Buyer.MODES.BUY_EXACT_QUAN;
            }
        }
        private void BY_Percentage_RB_CheckedChanged(object sender, EventArgs e)
        {
            if (BY_doingInits)
            {
                return;
            }
            if (BY_Percentage_RB.Checked)
            {
                BY_mode = (Byte)Bots.Buyer.MODES.BUY_PERC_OPEN_SLOTS;
            }
        }
        private void BY_Leave_Slots_Open_RB_CheckedChanged(object sender, EventArgs e)
        {
            if (BY_doingInits)
            {
                return;
            }
            if (BY_Leave_Slots_Open_RB.Checked)
            {
                BY_mode = (Byte)Bots.Buyer.MODES.LEAVE_NUMBER_SLOTS_OPEN;
            }
        }
        #endregion Radio Buttons
        #region Check Boxes
        private void BY_Include_Satchel_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            BY_includeSatchel = BY_Include_Satchel_ChkB.Checked;
        }
        private void BY_Include_Sack_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            BY_includeSack = BY_Include_Sack_ChkB.Checked;
        }
        #endregion Check Boxes
        #region UpDowns
        private void BY_Item_Quan_UpDn_ValueChanged(object sender, EventArgs e)
        {
            BY_itemQuan = Convert.ToUInt16(BY_Item_Quan_UpDn.Value);
        }
        private void BY_Combined_Total_UpDn_ValueChanged(object sender, EventArgs e)
        {
            BY_combinedTotalQuan = Convert.ToUInt16(BY_Combined_Total_UpDn.Value);
        }
        private void BY_Percentage_UpDn_ValueChanged(object sender, EventArgs e)
        {
            BY_percentageValue = Convert.ToByte(BY_Percentage_UpDn.Value);
        }
        private void BY_Leave_Slots_Open_UpDn_ValueChanged(object sender, EventArgs e)
        {
            BY_leaveSlotsOpenQuan = Convert.ToUInt16(BY_Leave_Slots_Open_UpDn.Value);
        }
        private void BY_Guild_Wait_Time_UpDn_ValueChanged(object sender, EventArgs e)
        {
            Statics.Settings.Helpers.GuildWaitTime_Buyer = Convert.ToUInt32(BY_Guild_Wait_Time_UpDn.Value);
            UserSettings.SetValue(UserSettings.BOT.HELPERS, "BY_GuildWaitTime", Statics.Settings.Helpers.GuildWaitTime_Buyer.ToString());
        }
        private void BY_Item_Price_UpDn_ValueChanged(object sender, EventArgs e)
        {
            BY_pricePerItem = Convert.ToUInt32(BY_Price_Per_Item_UpDn.Value);
        }
        #endregion UpDowns
        #region Combo Boxes
        private void BY_Script_CB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BY_Script_CB.SelectedIndex >= 0)
            {
                BY_scriptName = BY_Script_CB.SelectedItem.ToString();
                BY_scriptLoaded = true;
                BY_scriptLoadedName = BY_scriptName;
                BY_LoadScript(BY_scriptName);
                BY_scriptModified = false;
            }
            else
            {
                BY_scriptLoaded = false;
                BY_scriptLoadedName = "";
            }
        }
        #endregion Combo Boxes
        #region Buttons
        private void BY_Add_Button_Click(object sender, EventArgs e)
        {
            ushort enteredItemId = 0;
            enteredItemId = Things.GetIdFromName(BY_itemName);
            if (enteredItemId == 0)
            {
                MessageBox.Show("Could not find item '" + BY_itemName + "' in database.");
                return;
            }
            else
            {
                BY_itemIdList.Add(enteredItemId);
                BY_itemNameList.Add(BY_itemName);
                BY_itemQuanList.Add(BY_itemQuan);
                BY_priceList.Add(BY_pricePerItem);
                BY_Item_LB.Items.Add(BY_GetItemListBoxString());
                if (BY_scriptLoaded)
                {
                    BY_scriptModified = true;
                }
            }
        }
        private void BY_Remove_Button_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection selectedItems = BY_Item_LB.SelectedItems;
            if (selectedItems.Count == 0)
            {
                return;
            }
            else
            {
                int nbSelected = selectedItems.Count;
                for (int ii = nbSelected - 1; ii >= 0; ii--)
                {
                    int idx = BY_Item_LB.Items.IndexOf(selectedItems[ii]);
                    BY_itemIdList.RemoveAt(idx);
                    BY_itemNameList.RemoveAt(idx);
                    BY_itemQuanList.RemoveAt(idx);
                    BY_priceList.RemoveAt(idx);
                    BY_Item_LB.Items.RemoveAt(idx);
                }
                if (BY_scriptLoaded)
                {
                    BY_scriptModified = true;
                }
            }
        }
        private void BY_Clear_Button_Click(object sender, EventArgs e)
        {
            BY_loadDefaultValues();
            BY_Item_LB.Items.Clear();
            BY_itemIdList.Clear();
            BY_itemNameList.Clear();
            BY_itemQuanList.Clear();
            BY_priceList.Clear();
            BY_scriptLoaded = false;
            BY_scriptLoadedName = "";
            BY_Script_CB.SelectedIndex = -1;
            BY_scriptModified = false;
        }
        private void BY_Save_Script_Button_Click(object sender, EventArgs e)
        {
            if (BY_scriptLoaded)
            {
                BY_UpdateScript();
                BY_SaveScriptDB();
            }
            else
            {
                BY_AddScript();
                BY_SaveScriptDB();
                BY_scriptLoaded = true;
                BY_scriptLoadedName = BY_scriptName;
            }
            BY_scriptModified = false;
            BY_loadScriptCB();
            BY_Script_CB.SelectedItem = BY_scriptName;
        }
        private void BY_Delete_Script_Button_Click(object sender, EventArgs e)
        {
            if (BY_Script_CB.SelectedIndex >= 0)
            {
                BY_RemoveScript(BY_Script_CB.SelectedItem.ToString());
                BY_SaveScriptDB();
            }
            BY_loadScriptCB();
            BY_Script_CB.SelectedIndex = -1;
            BY_scriptModified = false;
        }
        private void BY_Start_Button_Click(object sender, EventArgs e)
        {
            if (Bots.Buyer.State == Bots.Buyer.STATE.STOPPED)
            {
                BY_RunScript();
            }
            else if (Bots.Buyer.State == Bots.Buyer.STATE.RUNNING)
            {
                Bots.Buyer.Pause();
            }
            else
            {
                Bots.Buyer.Resume();
            }
        }
        private void BY_Stop_Button_Click(object sender, EventArgs e)
        {
            Bots.Buyer.Stop();
        }
        #endregion Buttons
        #endregion Event Handlers
    }
}
