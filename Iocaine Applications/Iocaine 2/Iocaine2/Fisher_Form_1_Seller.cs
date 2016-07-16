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
        private enum SL_PRC_STATE : byte
        {
            STOPPED = 0,
            RUNNING = 1
        }
        #endregion Enums
        #region Members
        #region Settings
        #endregion Settings
        #region Misc
        private static bool SL_itemAutoCompleteSetSinceLogin = false;
        private static bool SL_itemAutoCompleteLock = false;
        #endregion Misc
        #region DB
        private Seller_Scripts_DS SL_scriptsDb;
        private List<Seller_Script> SL_scriptList;
        #endregion DB
        #region Bot Data
        private List<ushort> SL_itemIdList = new List<ushort>();
        private List<String> SL_itemNameList = new List<String>();
        private List<ushort> SL_itemQuanList = new List<ushort>();
        private String SL_scriptLoadedName = "";
        private bool SL_scriptLoaded = false;
        private bool SL_scriptModified = false;
        #endregion Bot Data
        #region File I/O
        private String SL_scriptFilePath = ".\\Scripts\\";
        private String SL_scriptFileName = "Seller_Scripts.xml";
        #endregion File I/O
        #region Time Related
        #endregion Time Related
        #region Default Values
        private String SL_npcNameTBDefText = "NPC Name";
        private String SL_itemNameTBDefText = "Item Name";
        private String SL_scriptNameTBDefText = "Script Name";
        private UInt16 SL_itemQuanUpDnDefValue = 1;
        private bool SL_sellAllDefValue = false;
        private bool SL_sellFromSatchelDefValue = false;
        private bool SL_sellFromSackDefValue = false;
        #endregion Default Values
        #region GUI Value Parallels
        private String SL_npcName = "";
        private String SL_itemName = "";
        private String SL_scriptName = "";
        private UInt16 SL_itemQuan = 1;
        private bool SL_sellAll = false;
        private bool SL_sellFromSatchel = false;
        private bool SL_sellFromSack = false;
        private bool SL_sellFromCase = false;
        #endregion GUI Value Parallels
        #region Tool Tips
        #endregion Tool Tips
        #region Delegates
        private delegate void SL_setNpcNameTBTextDelegate(String iText);
        private delegate void SL_setItemNameTBTextDelegate(String iText);
        private delegate void SL_setScriptNameTBTextDelegate(String iText);
        private delegate void SL_setItemQuanUpDnValueDelegate(UInt16 iValue);
        private delegate void SL_setSellAllChkBValueDelegate(bool iChecked);
        private delegate void SL_setSellFromSatchelChkBValueDelegate(bool iChecked);
        private delegate void SL_setSellFromSackChkBValueDelegate(bool iChecked);
        private delegate void SL_setSellFromCaseChkBValueDelegate(bool iChecked);
        private delegate void SL_loadScriptCBDelegate();
        private delegate void SL_setStartButtonDelegate(String iText, Color iColor);
        #endregion Delegates
        #region Function Pointers
        private SL_setNpcNameTBTextDelegate SL_setNpcNameTBTextPtr;
        private SL_setItemNameTBTextDelegate SL_setItemNameTBTextPtr;
        private SL_setScriptNameTBTextDelegate SL_setScriptNameTBTextPtr;
        private SL_setItemQuanUpDnValueDelegate SL_setItemQuanUpDnValuePtr;
        private SL_setSellAllChkBValueDelegate SL_setSellAllChkBValuePtr;
        private SL_setSellFromSatchelChkBValueDelegate SL_setSellFromSatchelChkBValuePtr;
        private SL_setSellFromSackChkBValueDelegate SL_setSellFromSackChkBValuePtr;
        private SL_setSellFromCaseChkBValueDelegate SL_setSellFromCaseChkBValuePtr;
        private SL_setStartButtonDelegate SL_setStartButtonPtr;
        private SL_loadScriptCBDelegate SL_loadScriptCBPtr;
        #endregion Function Pointers
        #endregion Members
        #region Inits
        private void SL_Prc_inits()
        {
            SL_createDelegates();
            SL_initDb();
            SL_initLists();
            SL_loadDefaultValues();
            //SL_setNpcNameAutoComplete();
            SL_LoadScriptDB();
            SL_LoadScriptsListFromDb();
            SL_loadScriptCB();
        }
        private void SL_Login_inits()
        {
            SL_loadUserSettings();
            SL_itemAutoCompleteSetSinceLogin = false;
        }
        private void SL_createDelegates()
        {
            if (SL_setNpcNameTBTextPtr == null)
            {
                SL_setNpcNameTBTextPtr = new SL_setNpcNameTBTextDelegate(SL_setNpcNameTBTextCallBackFunction);
                SL_setItemNameTBTextPtr = new SL_setItemNameTBTextDelegate(SL_setItemNameTBTextCallBackFunction);
                SL_setScriptNameTBTextPtr = new SL_setScriptNameTBTextDelegate(SL_setScriptNameTBTextCallBackFunction);
                SL_setItemQuanUpDnValuePtr = new SL_setItemQuanUpDnValueDelegate(SL_setItemQuanUpDnValueCallBackFunction);
                SL_setSellAllChkBValuePtr = new SL_setSellAllChkBValueDelegate(SL_setSellAllChkBValueCallBackFunction);
                SL_setSellFromSatchelChkBValuePtr = new SL_setSellFromSatchelChkBValueDelegate(SL_setSellFromSatchelChkBValueCallBackFunction);
                SL_setSellFromSackChkBValuePtr = new SL_setSellFromSackChkBValueDelegate(SL_setSellFromSackChkBValueCallBackFunction);
                SL_setSellFromCaseChkBValuePtr = new SL_setSellFromCaseChkBValueDelegate(SL_setSellFromCaseChkBValueCallBackFunction);
                SL_setStartButtonPtr = new SL_setStartButtonDelegate(SL_setStartButtonCallBackFunction);
                SL_loadScriptCBPtr = new SL_loadScriptCBDelegate(SL_loadScriptCBCallBackFunction);
                Statics.FuncPtrs.SetSellerButtonPtr = new Statics.FuncPtrs.TD_Void_String_Color(SL_setStartButton);
            }
        }
        private void SL_initDb()
        {
            if (SL_scriptsDb == null)
            {
                SL_scriptsDb = new Seller_Scripts_DS();
            }
            else
            {
                SL_scriptsDb.Clear();
            }
        }
        private void SL_initLists()
        {
            if (SL_scriptList == null)
            {
                SL_scriptList = new List<Seller_Script>();
            }
            else
            {
                SL_scriptList.Clear();
            }
        }
        private void SL_loadUserSettings()
        {
            Statics.Settings.Helpers.EnterNpcMenuDelay = (UInt32)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_EnterNpcMenuDelay");
            Statics.Settings.Helpers.EnterToSellDelay = (UInt32)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_EnterToSellDelay");
            Statics.Settings.Helpers.CheckHelpTextDelay = (UInt32)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_CheckHelpTextDelay");
            Statics.Settings.Helpers.ChangeSellQuanDelay = (UInt32)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_ChangeSellQuanDelay");
            Statics.Settings.Helpers.PressEnterToSellDelay = (UInt32)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_PressEnterToSellDelay");
            Statics.Settings.Helpers.WhenDoneSound_Seller = (String)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_WhenDoneSound");
            Statics.Settings.Helpers.SellFromBagFirst = (bool)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_SellFromBagFirst");
            Statics.Settings.Helpers.SortBeforeSelling = (bool)UserSettings.GetValue(UserSettings.BOT.HELPERS, "SL_SortBeforeSelling");
        }
        private void SL_loadDefaultValues()
        {
            SL_setNpcNameTBText(SL_npcNameTBDefText);
            SL_setItemNameTBText(SL_itemNameTBDefText);
            SL_setScriptNameTBText(SL_scriptNameTBDefText);
            SL_setItemQuanUpDnValue(SL_itemQuanUpDnDefValue);
            SL_setSellAllChkBValue(SL_sellAllDefValue);
            SL_setSellFromSatchelChkBValue(SL_sellFromSatchelDefValue);
            SL_setSellFromSackChkBValue(SL_sellFromSackDefValue);
        }
        private void SL_setNpcNameAutoComplete()
        {
            SL_NPC_Name_TB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            SL_NPC_Name_TB.AutoCompleteSource = AutoCompleteSource.CustomSource;
            SL_NPC_Name_TB.AutoCompleteCustomSource = NPCs.NpcNameCollection;
        }
        private void SL_setItemAutoComplete()
        {
            if (SL_itemAutoCompleteSetSinceLogin)
            {
                return;
            }
            AutoCompleteStringCollection itemACList = Containers.ItemAutoCompleteList;
            SL_itemAutoCompleteLock = true;
            if (itemACList.Count > 0)
            {
                SL_Item_Name_TB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                SL_Item_Name_TB.AutoCompleteSource = AutoCompleteSource.CustomSource;
                SL_Item_Name_TB.AutoCompleteCustomSource = itemACList;
                SL_itemAutoCompleteSetSinceLogin = true;
            }
            else
            {
                SL_Item_Name_TB.AutoCompleteMode = AutoCompleteMode.None;
                SL_Item_Name_TB.AutoCompleteSource = AutoCompleteSource.None;
            }
            SL_itemAutoCompleteLock = false;
        }
        #endregion Inits
        #region Utility Functions
        #region GUI Updates
        #region Text Box Updates
        private void SL_setNpcNameTBText(String iText)
        {
            try
            {
                if (SL_NPC_Name_TB.InvokeRequired)
                {
                    SL_NPC_Name_TB.Invoke(SL_setNpcNameTBTextPtr, new object[] { iText });
                }
                else
                {
                    SL_setNpcNameTBTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In SL_setNpcNameTBText: " + e.ToString());
            }
        }
        private void SL_setNpcNameTBTextCallBackFunction(String iText)
        {
            SL_NPC_Name_TB.Text = iText;
            if (iText != SL_npcNameTBDefText)
            {
                SL_NPC_Name_TB.ForeColor = Color.Black;
            }
            else
            {
                SL_NPC_Name_TB.ForeColor = Color.Gray;
            }
        }
        private void SL_setItemNameTBText(String iText)
        {
            try
            {
                if (SL_Item_Name_TB.InvokeRequired)
                {
                    SL_Item_Name_TB.Invoke(SL_setItemNameTBTextPtr, new object[] { iText });
                }
                else
                {
                    SL_setItemNameTBTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In SL_setItemNameTBText: " + e.ToString());
            }
        }
        private void SL_setItemNameTBTextCallBackFunction(String iText)
        {
            SL_Item_Name_TB.Text = iText;
            if (iText != SL_itemNameTBDefText)
            {
                SL_Item_Name_TB.ForeColor = Color.Black;
            }
            else
            {
                SL_Item_Name_TB.ForeColor = Color.Gray;
            }
        }
        private void SL_setScriptNameTBText(String iText)
        {
            try
            {
                if (SL_Script_Name_TB.InvokeRequired)
                {
                    SL_Script_Name_TB.Invoke(SL_setScriptNameTBTextPtr, new object[] { iText });
                }
                else
                {
                    SL_setScriptNameTBTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In SL_setScriptNameTBText: " + e.ToString());
            }
        }
        private void SL_setScriptNameTBTextCallBackFunction(String iText)
        {
            SL_Script_Name_TB.Text = iText;
            if (iText != SL_scriptNameTBDefText)
            {
                SL_Script_Name_TB.ForeColor = Color.Black;
            }
            else
            {
                SL_Script_Name_TB.ForeColor = Color.Gray;
            }
        }
        #endregion Text Box Updates
        #region UpDown Updates
        private void SL_setItemQuanUpDnValue(UInt16 iValue)
        {
            try
            {
                if(SL_Item_Quan_UpDn.InvokeRequired)
                {
                    SL_Item_Quan_UpDn.Invoke(SL_setItemQuanUpDnValuePtr, new object[] { iValue });
                }
                else
                {
                    SL_setItemQuanUpDnValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In SL_setItemQuanUpDnValue: " + e.ToString());
            }
        }
        private void SL_setItemQuanUpDnValueCallBackFunction(UInt16 iValue)
        {
            SL_Item_Quan_UpDn.Value = (decimal)iValue;
        }
        #endregion UpDown Updates
        #region Combo Box Updates
        private void SL_loadScriptCB()
        {
            try
            {

                if (SL_Script_CB.InvokeRequired)
                {
                    SL_Script_CB.Invoke(SL_loadScriptCBPtr);
                }
                else
                {
                    SL_loadScriptCBCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In SL_loadScriptCB: " + e.ToString());
            }
        }
        private void SL_loadScriptCBCallBackFunction()
        {
            SL_Script_CB.Items.Clear();
            foreach (Seller_Script script in SL_scriptList)
            {
                SL_Script_CB.Items.Add(script.ScriptName);
            }
        }
        #endregion Combo Box Updates
        #region Check Box Updates
        private void SL_setSellAllChkBValue(bool iValue)
        {
            try
            {
                if(SL_Sell_All_ChkB.InvokeRequired)
                {
                    SL_Sell_All_ChkB.Invoke(SL_setSellAllChkBValuePtr, new object[] { iValue });
                }
                else
                {
                    SL_setSellAllChkBValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In SL_setSellAllChkBValue: " + e.ToString());
            }
        }
        private void SL_setSellAllChkBValueCallBackFunction(bool iValue)
        {
            SL_Sell_All_ChkB.Checked = iValue;
        }
        private void SL_setSellFromSatchelChkBValue(bool iValue)
        {
            try
            {
                if(SL_Sell_From_Satchel_ChkB.InvokeRequired)
                {
                    SL_Sell_From_Satchel_ChkB.Invoke(SL_setSellFromSatchelChkBValuePtr, new object[] { iValue });
                }
                else
                {
                    SL_setSellFromSatchelChkBValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In SL_setSellFromSatchelChkBValue: " + e.ToString());
            }
        }
        private void SL_setSellFromSatchelChkBValueCallBackFunction(bool iValue)
        {
            SL_Sell_From_Satchel_ChkB.Checked = iValue;
        }
        private void SL_setSellFromSackChkBValue(bool iValue)
        {
            try
            {
                if (SL_Sell_From_Sack_ChkB.InvokeRequired)
                {
                    SL_Sell_From_Sack_ChkB.Invoke(SL_setSellFromSackChkBValuePtr, new object[] { iValue });
                }
                else
                {
                    SL_setSellFromSackChkBValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In SL_setSellFromSackChkBValue: " + e.ToString());
            }
        }
        private void SL_setSellFromSackChkBValueCallBackFunction(bool iValue)
        {
            SL_Sell_From_Sack_ChkB.Checked = iValue;
        }
        private void SL_setSellFromCaseChkBValue(bool iValue)
        {
            try
            {
                if (SL_Sell_From_Case_ChkB.InvokeRequired)
                {
                    SL_Sell_From_Case_ChkB.Invoke(SL_setSellFromCaseChkBValuePtr, new object[] { iValue });
                }
                else
                {
                    SL_setSellFromCaseChkBValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In SL_setSellFromCaseChkBValue: " + e.ToString());
            }
        }
        private void SL_setSellFromCaseChkBValueCallBackFunction(bool iValue)
        {
            SL_Sell_From_Case_ChkB.Checked = iValue;
        }
        #endregion Check Box Updates
        #region Button Updates
        private void SL_setStartButton(String iText, Color iColor)
        {
            try
            {
                if (SL_Start_Button.InvokeRequired)
                {
                    SL_Start_Button.Invoke(SL_setStartButtonPtr, new object[] { iText, iColor });
                }
                else
                {
                    SL_setStartButtonCallBackFunction(iText, iColor);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In SL_setStartButton: " + e.ToString());
            }
        }
        private void SL_setStartButtonCallBackFunction(String iText, Color iColor)
        {
            SL_Start_Button.Text = iText;
            SL_Start_Button.BackColor = iColor;
        }
        #endregion Button Updates
        #endregion GUI Updates
        #region Script Related
        #region File I/O
        private void SL_LoadScriptDB()
        {
            if (Directory.Exists(SL_scriptFilePath))
            {
                if (File.Exists(SL_scriptFilePath + SL_scriptFileName))
                {
                    try
                    {
                        if (SL_scriptsDb == null)
                        {
                            SL_initDb();
                        }
                        SL_scriptsDb.ReadXml(SL_scriptFilePath + SL_scriptFileName);
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("In SL_loadScriptDB: " + e.ToString());
                    }
                }
            }
        }
        private void SL_LoadScriptsListFromDb()
        {
            Seller_Scripts_DS.Script_TableRow[] scriptRows = (Seller_Scripts_DS.Script_TableRow[])SL_scriptsDb.Script_Table.Select();
            foreach (Seller_Scripts_DS.Script_TableRow row in scriptRows)
            {
                Seller_Script script = SL_CreateScript(row);
                SL_scriptList.Add(script);
            }
        }
        private void SL_SaveScriptDB()
        {
            if (!Directory.Exists(SL_scriptFilePath))
            {
                try
                {
                    Directory.CreateDirectory(SL_scriptFilePath);
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("In SL_saveScriptDB creating directory: " + e.ToString());
                    return;
                }
            }
            try
            {
                if (SL_scriptsDb != null)
                {
                    SL_scriptsDb.WriteXml(SL_scriptFilePath + SL_scriptFileName);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In SL_saveScriptDB writing file: " + e.ToString());
            }
        }
        #endregion File I/O
        #region Script Loading
        private void SL_LoadScript(String iScriptName)
        {
            foreach (Seller_Script script in SL_scriptList)
            {
                if (script.ScriptName == iScriptName)
                {
                    SL_setScriptNameTBText(iScriptName);
                    SL_setNpcNameTBText(script.NpcName);
                    SL_setSellAllChkBValue(script.SellAll);
                    SL_setSellFromSackChkBValue(script.SellFromSack);
                    SL_setSellFromSatchelChkBValue(script.SellFromSatchel);
                    SL_setSellFromCaseChkBValue(script.SellFromCase);
                    SL_Item_LB.Items.Clear();
                    SL_itemIdList.Clear();
                    SL_itemNameList.Clear();
                    SL_itemQuanList.Clear();
                    List<Item> itemList = script.ItemList;
                    List<ushort> quanList = script.ItemQuanList;
                    for (int ii = 0; ii < itemList.Count; ii++)
                    {
                        SL_Item_LB.Items.Add(SL_GetItemListBoxString(itemList[ii].Name, quanList[ii]));
                        SL_itemIdList.Add(itemList[ii].ItemID);
                        SL_itemNameList.Add(itemList[ii].Name);
                        if (SL_sellAll)
                        {
                            SL_itemQuanList.Add(0);
                        }
                        else
                        {
                            SL_itemQuanList.Add(SL_itemQuan);
                        }
                    }
                    SL_scriptLoaded = true;
                    SL_scriptLoadedName = iScriptName;
                    return;
                }
            }
        }
        #endregion Script Loading
        #region Script Deletion
        private void SL_RemoveScript(String iScriptName)
        {
            int nbRows = SL_scriptsDb.Script_Table.Rows.Count;
            for (int ii = nbRows - 1; ii >= 0; ii--)
            {
                Seller_Scripts_DS.Script_TableRow row_ii = (Seller_Scripts_DS.Script_TableRow)SL_scriptsDb.Script_Table.Rows[ii];
                if (row_ii.ScriptName == iScriptName)
                {
                    LoggingFunctions.Debug("Removing a row from the Script Table, " + row_ii.ScriptName + ".", LoggingFunctions.DBG_SCOPE.SELLER);
                    SL_scriptsDb.Script_Table.Rows.Remove(row_ii);
                }
            }

            nbRows = SL_scriptsDb.Item_Table.Rows.Count;
            for (int ii = nbRows - 1; ii >= 0; ii--)
            {
                Seller_Scripts_DS.Item_TableRow row_ii = (Seller_Scripts_DS.Item_TableRow)SL_scriptsDb.Item_Table.Rows[ii];
                if (row_ii.ScriptName == iScriptName)
                {
                    LoggingFunctions.Debug("Removing a row from the Item Table, " + row_ii.ItemName + ".", LoggingFunctions.DBG_SCOPE.SELLER);
                    SL_scriptsDb.Item_Table.Rows.Remove(row_ii);
                }
            }
            int nbScripts = SL_scriptList.Count;
            for (int kk = nbScripts - 1; kk >= 0; kk--)
            {
                LoggingFunctions.Debug("Checking script in SL_scriptList[" + kk + "]: \"" + SL_scriptList[kk].ScriptName + "\".", LoggingFunctions.DBG_SCOPE.SELLER);
                if (SL_scriptList[kk].ScriptName == iScriptName)
                {
                    LoggingFunctions.Debug("Removing it.", LoggingFunctions.DBG_SCOPE.SELLER);
                    SL_scriptList.RemoveAt(kk);
                }
            }
        }
        #endregion Script Deletion
        #region Script Saving
        private void SL_AddScript()
        {
            Seller_Script script = SL_CreateScript();
            if (script != null)
            {
                SL_scriptList.Add(script);
            }
            SL_AddScriptToDb(script);
        }
        private Seller_Script SL_CreateScript()
        {
            if (SL_scriptName.Contains("'"))
            {
                MessageBox.Show("Script name cannot contain an apostrophe.");
                return null;
            }
            Seller_Script script = new Seller_Script();
            script.ScriptName = SL_scriptName;
            script.NpcName = SL_npcName;
            script.SellAll = SL_sellAll;
            script.SellFromSack = SL_sellFromSack;
            script.SellFromSatchel = SL_sellFromSatchel;
            script.SellFromCase = SL_sellFromCase;
            int nbItems = SL_itemNameList.Count;
            for (int ii = 0; ii < nbItems; ii++)
            {
                script.AddItem(SL_itemNameList[ii], SL_itemIdList[ii], SL_itemQuanList[ii]);
            }
            return script;
        }
        private Seller_Script SL_CreateScript(Seller_Scripts_DS.Script_TableRow iRow)
        {
            //Create a script that already exists in the database.
            Seller_Script script = new Seller_Script();
            script.ScriptName = iRow.ScriptName;
            script.NpcName = iRow.NpcName;
            script.SellAll = iRow.SellAll;
            script.SellFromSack = iRow.SellFromSack;
            script.SellFromSatchel = iRow.SellFromSatchel;
            script.SellFromCase = iRow.SellFromCase;

            String filterString = "ScriptName = '" + iRow.ScriptName + "'";
            Seller_Scripts_DS.Item_TableRow[] itemRows = (Seller_Scripts_DS.Item_TableRow[])SL_scriptsDb.Item_Table.Select(filterString);
            foreach(Seller_Scripts_DS.Item_TableRow itemRow in itemRows)
            {
                script.AddItem(new Item(itemRow.ItemName, itemRow.ItemId, Item.ITEM_TYPE.UNKNOWN), itemRow.ItemQuan);
                //script.ItemQuanList.Add(itemRow.ItemQuan);
            }
            return script;
        }
        private void SL_UpdateScript()
        {
            //To update a script we need to do the following:
            //1. Remove all traces of the script from the DB (item table and script table).
            //2. Remove the script object from the scripts list.
            //3. Create a new script based on what's loaded.
            //4. Add the new script to the DB.
            if (SL_scriptName.Contains("'"))
            {
                MessageBox.Show("Script name cannot contain an apostrophe.");
                return;
            }
            SL_RemoveScript(SL_scriptLoadedName);
            SL_AddScript();
        }
        private void SL_AddScriptToDb(Seller_Script iScript)
        {
            Seller_Scripts_DS.Script_TableRow scriptRow = SL_scriptsDb.Script_Table.NewScript_TableRow();
            scriptRow.ScriptName = iScript.ScriptName;
            scriptRow.NpcName = iScript.NpcName;
            scriptRow.SellAll = iScript.SellAll;
            scriptRow.SellFromSack = iScript.SellFromSack;
            scriptRow.SellFromSatchel = iScript.SellFromSatchel;
            scriptRow.SellFromCase = iScript.SellFromCase;
            SL_scriptsDb.Script_Table.Rows.Add(scriptRow);

            List<Item> itemList = iScript.ItemList;
            List<ushort> itemQuanList = iScript.ItemQuanList;
            int nbItems = itemList.Count;
            for (int ii = 0; ii < nbItems; ii++)
            {
                Seller_Scripts_DS.Item_TableRow itemRow = SL_scriptsDb.Item_Table.NewItem_TableRow();
                itemRow.ScriptName = iScript.ScriptName;
                itemRow.ItemName = itemList[ii].Name;
                itemRow.ItemId = itemList[ii].ItemID;
                itemRow.ItemQuan = itemQuanList[ii];
                SL_scriptsDb.Item_Table.Rows.Add(itemRow);
            }
        }
        #endregion Script Saving
        #endregion Script Related
        #region Parsing
        private String SL_GetItemListBoxString(String iName, ushort iQuan)
        {
            if ((iName != "") && (iName != SL_itemNameTBDefText))
            {
                if (iQuan == 0)
                {
                    return (iName + " (All)");
                }
                else
                {
                    return (iName + " (" + iQuan + ")");
                }
            }
            else
            {
                return "";
            }
        }
        private String SL_GetItemListBoxString()
        {
            if ((SL_itemName != "") && (SL_itemName != SL_itemNameTBDefText))
            {
                if (SL_sellAll)
                {
                    return SL_GetItemListBoxString(SL_itemName, 0);
                }
                else
                {
                    return SL_GetItemListBoxString(SL_itemName, SL_itemQuan);
                }
            }
            else
            {
                return "";
            }
        }
        private String SL_ParseItemName(String iText)
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
        private ushort SL_ParseItemQuan(String iText)
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
                if (quanString == "All")
                {
                    return 0;
                }
                else
                {
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
        }
        #endregion Parsing
        #endregion Utility Functions
        #region Processing
        private void SL_RunScript(Seller_Script iScript)
        {
            if (iScript.ItemList.Count == 0)
            {
                MessageBox.Show("Please add at least 1 item to the list.");
                return;
            }
            else if ((iScript.NpcName == "") || (iScript.NpcName == SL_npcNameTBDefText))
            {
                MessageBox.Show("Please enter an NPC name to sell to.");
                return;
            }
            Bots.Seller.Start(iScript, Statics.Settings.Helpers.WhenDoneSound_Seller);
        }
        private void SL_RunScript()
        {
            if (SL_scriptLoaded && !SL_scriptModified)
            {
                foreach (Seller_Script scr in SL_scriptList)
                {
                    if (scr.ScriptName == SL_scriptLoadedName)
                    {
                        SL_RunScript(scr);
                    }
                }
            }
            else
            {
                Seller_Script scr2 = SL_CreateScript();
                if (scr2 != null)
                {
                    SL_RunScript(scr2);
                }
            }
        }
        #endregion Processing
        #region Event Handlers
        #region Text Boxes
        #region NPC Name TB
        private void SL_NPC_Name_TB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                if (ChangeMonitor.LoggedIn == false)
                {
                    return;
                }
                String targetName = MemReads.Target.get_name();
                if ((SL_NPC_Name_TB.Text == "") && (targetName != ""))
                {
                    SL_NPC_Name_TB.Text = targetName;
                }
            }
        }
        private void SL_NPC_Name_TB_TextChanged(object sender, EventArgs e)
        {
            SL_npcName = SL_NPC_Name_TB.Text;
            if (SL_scriptLoaded)
            {
                SL_scriptModified = true;
            }
        }
        private void SL_NPC_Name_TB_Enter(object sender, EventArgs e)
        {
            if (SL_NPC_Name_TB.Text == SL_npcNameTBDefText)
            {
                SL_NPC_Name_TB.Text = "";
                SL_NPC_Name_TB.ForeColor = Color.Black;
            }
        }
        private void SL_NPC_Name_TB_Leave(object sender, EventArgs e)
        {
            if (SL_NPC_Name_TB.Text == "")
            {
                SL_NPC_Name_TB.Text = SL_npcNameTBDefText;
                SL_NPC_Name_TB.ForeColor = Color.Gray;
            }
        }
        #endregion NPC Name TB
        #region Item Name TB
        private void SL_Item_Name_TB_TextChanged(object sender, EventArgs e)
        {
            SL_itemName = SL_Item_Name_TB.Text;
        }
        private void SL_Item_Name_TB_Enter(object sender, EventArgs e)
        {
            if (SL_itemAutoCompleteLock)
            {
                return;
            }
            if (SL_Item_Name_TB.Text == SL_itemNameTBDefText)
            {
                SL_setItemAutoComplete();
                SL_Item_Name_TB.Text = "";
                SL_Item_Name_TB.ForeColor = Color.Black;
            }
        }
        private void SL_Item_Name_TB_Leave(object sender, EventArgs e)
        {
            if (SL_Item_Name_TB.Text == "")
            {
                SL_Item_Name_TB.Text = SL_itemNameTBDefText;
                SL_Item_Name_TB.ForeColor = Color.Gray;
            }
            else
            {
                String itemText = SL_Item_Name_TB.Text;
                itemText = itemText.Substring(0, 1).ToUpper() + itemText.Substring(1, itemText.Length - 1);
                SL_Item_Name_TB.Text = itemText;
            }
        }
        #endregion Item Name TB
        #region Script Name TB
        private void SL_Script_Name_TB_TextChanged(object sender, EventArgs e)
        {
            SL_scriptName = SL_Script_Name_TB.Text;
            if (SL_scriptLoaded)
            {
                SL_scriptModified = true;
            }
        }
        private void SL_Script_Name_TB_Enter(object sender, EventArgs e)
        {
            if (SL_Script_Name_TB.Text == SL_scriptNameTBDefText)
            {
                SL_Script_Name_TB.Text = "";
                SL_Script_Name_TB.ForeColor = Color.Black;
            }
        }
        private void SL_Script_Name_TB_Leave(object sender, EventArgs e)
        {
            if (SL_Script_Name_TB.Text == "")
            {
                SL_Script_Name_TB.Text = SL_scriptNameTBDefText;
                SL_Script_Name_TB.ForeColor = Color.Gray;
            }
        }
        #endregion Script Name TB
        #endregion Text Boxes
        #region Check Boxes
        private void SL_Sell_All_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            SL_sellAll = SL_Sell_All_ChkB.Checked;
        }
        private void SL_Sell_From_Satchel_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            SL_sellFromSatchel = SL_Sell_From_Satchel_ChkB.Checked;
            if (SL_scriptLoaded)
            {
                SL_scriptModified = true;
            }
        }
        private void SL_Sell_From_Sack_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            SL_sellFromSack = SL_Sell_From_Sack_ChkB.Checked;
            if (SL_scriptLoaded)
            {
                SL_scriptModified = true;
            }
        }
        private void SL_Sell_From_Case_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            SL_sellFromCase = SL_Sell_From_Case_ChkB.Checked;
            if (SL_scriptLoaded)
            {
                SL_scriptModified = true;
            }
        }
        #endregion Check Boxes
        #region UpDowns
        private void SL_Item_Quan_UpDn_ValueChanged(object sender, EventArgs e)
        {
            SL_itemQuan = Convert.ToUInt16(SL_Item_Quan_UpDn.Value);
        }
        #endregion UpDowns
        #region Combo Boxes
        private void SL_Script_CB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SL_Script_CB.SelectedIndex >= 0)
            {
                SL_scriptName = (String)SL_Script_CB.SelectedItem;
                SL_scriptLoaded = true;
                SL_scriptLoadedName = SL_scriptName;
                SL_LoadScript(SL_scriptName);
                SL_scriptModified = false;
            }
            else
            {
                SL_scriptLoaded = false;
                SL_scriptLoadedName = "";
            }
        }
        #endregion Combo Boxes
        #region Buttons
        private void SL_Add_Button_Click(object sender, EventArgs e)
        {
            ushort enteredItemId = 0;
            enteredItemId = Things.GetIdFromName(SL_itemName);
            if (enteredItemId == 0)
            {
                MessageBox.Show("Could not find item '" + SL_itemName + "' in database.");
                return;
            }
            else
            {
                SL_itemIdList.Add(enteredItemId);
                SL_itemNameList.Add(SL_itemName);
                if (SL_sellAll)
                {
                    SL_itemQuanList.Add(0);
                }
                else
                {
                    SL_itemQuanList.Add(SL_itemQuan);
                }
                SL_Item_LB.Items.Add(SL_GetItemListBoxString());
                if (SL_scriptLoaded)
                {
                    SL_scriptModified = true;
                }
            }
        }
        private void SL_Remove_Button_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection selectedItems = SL_Item_LB.SelectedItems;
            if (selectedItems.Count == 0)
            {
                return;
            }
            else
            {
                int nbSelected = selectedItems.Count;
                for(int ii=nbSelected-1; ii>=0; ii--)
                {
                    int idx = SL_Item_LB.Items.IndexOf(selectedItems[ii]);
                    SL_itemIdList.RemoveAt(idx);
                    SL_itemNameList.RemoveAt(idx);
                    SL_itemQuanList.RemoveAt(idx);
                    SL_Item_LB.Items.RemoveAt(idx);
                }
                if (SL_scriptLoaded)
                {
                    SL_scriptModified = true;
                }
            }
        }
        private void SL_Clear_Button_Click(object sender, EventArgs e)
        {
            SL_loadDefaultValues();
            SL_Item_LB.Items.Clear();
            SL_itemIdList.Clear();
            SL_itemNameList.Clear();
            SL_itemQuanList.Clear();
            SL_scriptLoaded = false;
            SL_scriptLoadedName = "";
            SL_Script_CB.SelectedIndex = -1;
            SL_scriptModified = false;
        }
        private void SL_Save_Script_Button_Click(object sender, EventArgs e)
        {
            if (SL_scriptLoaded)
            {
                SL_UpdateScript();
                SL_SaveScriptDB();
            }
            else
            {
                SL_AddScript();
                SL_SaveScriptDB();
                SL_scriptLoaded = true;
                SL_scriptLoadedName = SL_scriptName;
            }
            SL_scriptModified = false;
            SL_loadScriptCB();
            SL_Script_CB.SelectedItem = SL_scriptName;
        }
        private void SL_Delete_Script_Button_Click(object sender, EventArgs e)
        {
            if (SL_Script_CB.SelectedIndex >= 0)
            {
                SL_RemoveScript((String)SL_Script_CB.Items[SL_Script_CB.SelectedIndex]);
                SL_SaveScriptDB();
            }
            SL_loadScriptCB();
            if (SL_Script_CB.Items.Count > 0)
            {
                SL_Script_CB.SelectedIndex = 0;
            }
            else
            {
                SL_Script_CB.SelectedIndex = -1;
            }
            SL_scriptModified = false;
        }
        private void SL_Start_Button_Click(object sender, EventArgs e)
        {
            if (Bots.Seller.State == Bots.Seller.STATE.STOPPED)
            {
                SL_RunScript();
            }
            else if (Bots.Seller.State == Bots.Seller.STATE.RUNNING)
            {
                Bots.Seller.Pause();
            }
            else
            {
                Bots.Seller.Resume();
            }
        }
        private void SL_Stop_Button_Click(object sender, EventArgs e)
        {
            Bots.Seller.Stop();
        }
        #endregion Buttons
        #endregion Event Handlers
    }
}