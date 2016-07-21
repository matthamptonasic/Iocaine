using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Inventory;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Settings;
using Iocaine2.Threading;

namespace Iocaine2
{
    //This file is for functions directly related to the WMS Tab
    partial class Iocaine_2_Form
    {
        #region Member Variables
        private bool WMS_objectsCreated = false;
        private WMSDataSet WMS_dataset;
        private WMSDataSet WMS_dataset_temp;
        private bool WMS_poolInventory = true;
        private bool WMS_separateInventory = false;
        private bool WMS_allCharacters = false;
        private String WMS_dataDirectory = ".\\WMS_Data";
        private String WMS_dataFile = "WMS_Dataset";
        private String WMS_selectedChar = "";
        private String WMS_lastCharOnRebuild = "";
        private List<String> WMS_inventoryTypes = new List<string>(new string[] { "Bag", "Satchel", "Sack", "Case", "Safe", "Safe2", "Storage", "Locker", "Wardrobe", "Wardrobe2", "Wardrobe3", "Wardrobe4" });
        private bool WMS_initialLBUpdateDone = false;
        private int WMS_pooledColumnWidthLow = 175;
        private int WMS_pooledColumnWidthHi = 225;
        #endregion Member Variables
        #region Thread Synchronization
        #region Delegate declarations
        private delegate void WMS_updateSingleLBDelegate(ItemContainer container, WMSDataSet.ItemsRow[] itemRows);
        private delegate void WMS_updatePooledLBDelegate(WMSDataSet.ItemsRow[] itemRows);
        private delegate void WMS_updateLabelTextDelegate(Label iLabel, String iText);
        private delegate void WMS_updateControlVisibilityDelegate(Control iControl, bool iVisible);
        private delegate void WMS_sendControlToDelegate(Control iControl, bool iToFront);
        private delegate void WMS_loadCharacterCBDelegate();
        private delegate void WMS_setFirstInventoryLocationDelegate(Int32 iPosX);
        #endregion Delegate declarations
        #region Delegate instances
        private WMS_updateSingleLBDelegate WMS_updateSingleLBPtr;
        private WMS_updatePooledLBDelegate WMS_updatePooledLBPtr;
        private WMS_updateLabelTextDelegate WMS_updateLabelTextPtr;
        private WMS_updateControlVisibilityDelegate WMS_updateControlVisibilityPtr;
        private WMS_sendControlToDelegate WMS_sendControlToPtr;
        private WMS_loadCharacterCBDelegate WMS_loadCharacterCBPtr;
        private WMS_setFirstInventoryLocationDelegate WMS_setFirstInventoryLocationPtr;
        #endregion Delegate instances
        #region Delegate instantiations
        private void WMS_createDelegates()
        {
            if (WMS_updateSingleLBPtr == null)
            {
                WMS_updateSingleLBPtr = new WMS_updateSingleLBDelegate(WMS_updateSingleLB_CBF);
            }
            if (WMS_updatePooledLBPtr == null)
            {
                WMS_updatePooledLBPtr = new WMS_updatePooledLBDelegate(WMS_updatePooledLB_CBF);
            }
            if (WMS_updateLabelTextPtr == null)
            {
                WMS_updateLabelTextPtr = new WMS_updateLabelTextDelegate(WMS_updateLabelText_CBF);
            }
            if (WMS_updateControlVisibilityPtr == null)
            {
                WMS_updateControlVisibilityPtr = new WMS_updateControlVisibilityDelegate(WMS_updateControlVisibility_CBF);
            }
            if (WMS_sendControlToPtr == null)
            {
                WMS_sendControlToPtr = new WMS_sendControlToDelegate(WMS_sendControlTo_CBF);
            }
            if (WMS_loadCharacterCBPtr == null)
            {
                WMS_loadCharacterCBPtr = new WMS_loadCharacterCBDelegate(WMS_loadCharacterCBCallBackFunction);
            }
            if (WMS_setFirstInventoryLocationPtr == null)
            {
                WMS_setFirstInventoryLocationPtr = new WMS_setFirstInventoryLocationDelegate(WMS_setFirstInventoryLocation_CBF);
            }
        }
        #endregion Delegate instantiations
        #region Call Back Functions
        private void WMS_updateSingleLB_CBF(ItemContainer container, WMSDataSet.ItemsRow[] itemRows)
        {
            try
            {
                Monitor.Enter(Inventory.Containers.Padlock);
                ListBox lb = WMS_getListbox(container, itemRows);
                if (container != null)
                {
                    List<Item> itemList = container.GetSummaryItemList();
                    List<ushort> itemQuan = container.GetSummaryItemQuanList();
                    int cnt = itemList.Count;
                    lb.BeginUpdate();
                    lb.Items.Clear();
                    lb.Sorted = true;
                    for (int ii = 0; ii < cnt; ii++)
                    {
                        lb.Items.Add(itemList[ii].Name + " (" + itemQuan[ii] + ")");
                    }
                    lb.EndUpdate();
                }
                else if ((itemRows != null) && (itemRows.Length > 0))
                {
                    int cnt = itemRows.Length;
                    lb.BeginUpdate();
                    lb.Items.Clear();
                    lb.Sorted = true;
                    for (int ii = 0; ii < cnt; ii++)
                    {
                        lb.Items.Add(itemRows[ii].Name + " (" + itemRows[ii].Quantity + ")");
                    }
                    lb.EndUpdate();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("WMS_updateSingleLBCallBackFunction: " + e.ToString());
            }
            finally
            {
                Monitor.Exit(Inventory.Containers.Padlock);
            }
        }
        private void WMS_updatePooledLB_CBF(WMSDataSet.ItemsRow[] itemRows)
        {
            try
            {
                Monitor.Enter(Inventory.Containers.Padlock);
                ListBox lb = WMS_PooledLB;
                if (itemRows == null)
                {
                    List<Item> itemList;
                    List<ushort> itemQuan;
                    lb.BeginUpdate();
                    lb.Items.Clear();
                    lb.Sorted = true;
                    lb.ColumnWidth = WMS_pooledColumnWidthLow;
                    foreach (ItemContainer container in Containers.All)
                    {
                        itemList = container.GetSummaryItemList();
                        itemQuan = container.GetSummaryItemQuanList();
                        int cnt = itemList.Count;
                        for (int ii = 0; ii < cnt; ii++)
                        {
                            lb.Items.Add(itemList[ii].Name + " (" + itemQuan[ii] + ") [" + container.TypeStringAbbr + "]");
                        }
                    }
                    lb.EndUpdate();
                }
                else if (itemRows.Length > 0)
                {
                    int cnt = itemRows.Length;
                    lb.BeginUpdate();
                    lb.Items.Clear();
                    lb.Sorted = true;
                    if (!WMS_allCharacters)
                    {
                        lb.ColumnWidth = WMS_pooledColumnWidthLow;
                    }
                    else
                    {
                        lb.ColumnWidth = WMS_pooledColumnWidthHi;
                    }
                    for (int ii = 0; ii < cnt; ii++)
                    {
                        if (WMS_allCharacters)
                        {
                            lb.Items.Add(itemRows[ii].Name + " (" + itemRows[ii].Quantity + ") [" + itemRows[ii].Character + "'s " + itemRows[ii].Location + "]");
                        }
                        else
                        {
                            lb.Items.Add(itemRows[ii].Name + " (" + itemRows[ii].Quantity + ") [" + itemRows[ii].Location + "]");
                        }
                    }
                    lb.EndUpdate();
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("WMS_updatePooledLBCallBackFunction: " + e.ToString());
            }
            finally
            {
                Monitor.Exit(Inventory.Containers.Padlock);
            }
        }
        private void WMS_updateLabelText_CBF(Label iLabel, String iText)
        {
            iLabel.Text = iText;
        }
        private void WMS_updateControlVisibility_CBF(Control iControl, bool iVisible)
        {
            iControl.Visible = iVisible;
        }
        private void WMS_sendControlTo_CBF(Control iControl, bool iToFront)
        {
            if (iToFront)
            {
                iControl.BringToFront();
            }
            else
            {
                iControl.SendToBack();
            }
        }
        private void WMS_setFirstInventoryLocation_CBF(Int32 iPosX)
        {
            WMS_BagOccLabel.Left = iPosX;
        }
        #endregion Call Back Functions
        #region Update Wrapper Functions
        private void WMS_updateSingleLB(ItemContainer container, WMSDataSet.ItemsRow[] itemRows)
        {
            ListBox lb = WMS_getListbox(container, itemRows);
            try
            {
                if (lb.InvokeRequired)
                {
                    lb.Invoke(WMS_updateSingleLBPtr, new object[] { container, itemRows });
                }
                else
                {
                    WMS_updateSingleLB_CBF(container, itemRows);
                }
            }
            catch (Exception ex)
            {
                LoggingFunctions.Error("Trying to update the " + lb.Name + " : " + ex.ToString());
            }
        }
        private void WMS_updatePooledLB(WMSDataSet.ItemsRow[] itemRows)
        {
            try
            {
                if (WMS_PooledLB.InvokeRequired)
                {
                    WMS_PooledLB.Invoke(WMS_updatePooledLBPtr, new object[] { itemRows });
                }
                else
                {
                    WMS_updatePooledLB_CBF(itemRows);
                }
            }
            catch (Exception ex)
            {
                LoggingFunctions.Error("Trying to update the Pooled Listbox : " + ex.ToString());
            }
        }
        private void WMS_updateLabelText(Label iLabel, String iText)
        {
            try
            {
                if (iLabel.InvokeRequired)
                {
                    iLabel.Invoke(WMS_updateLabelTextPtr, new object[] {iLabel, iText});
                }
                else
                {
                    WMS_updateLabelText_CBF(iLabel, iText);
                }
            }
            catch (Exception ex)
            {
                LoggingFunctions.Error("Trying to update the " + iLabel.Name + " label text : " + ex.ToString());
            }
        }
        private void WMS_updateControlVisibility(Control iControl, bool iVisibile)
        {
            try
            {
                if (iControl.InvokeRequired)
                {
                    iControl.Invoke(WMS_updateControlVisibilityPtr, new object[] { iControl, iVisibile });
                }
                else
                {
                    WMS_updateControlVisibility_CBF(iControl, iVisibile);
                }
            }
            catch (Exception ex)
            {
                LoggingFunctions.Error("Trying to update the " + iControl.Name + " Control visibility : " + ex.ToString());
            }
        }
        private void WMS_sendControlTo(Control iControl, bool iToFront)
        {
            try
            {
                if (iControl.InvokeRequired)
                {
                    iControl.Invoke(WMS_sendControlToPtr, new object[] { iControl, iToFront });
                }
                else
                {
                    WMS_sendControlTo_CBF(iControl, iToFront);
                }
            }
            catch (Exception ex)
            {
                LoggingFunctions.Error("Trying to update the " + iControl.Name + " control to front/back : " + ex.ToString());
            }
        }
        private void WMS_setFirstInventoryLocation(Int32 iPosX)
        {
            try
            {
                if (WMS_BagOccLabel.InvokeRequired)
                {
                    WMS_BagOccLabel.Invoke(WMS_setFirstInventoryLocationPtr, new object[] { iPosX });
                }
                else
                {
                    WMS_setFirstInventoryLocation_CBF(iPosX);
                }
            }
            catch (Exception ex)
            {
                LoggingFunctions.Error("Trying to move WMS_BagOccLabel to " + iPosX + " : " + ex.ToString());
            }
        }
        #endregion Update Wrapper Functions
        #endregion Thread Synchronization
        #region Initialization
        private bool WMS_Init_LoggedIn()
        {
            WMS_createDelegates();
            WMS_Init_Dataset();
            WMS_Init_GUI();

            if (ChangeMonitor.MainProc != null && ChangeMonitor.MainModule != null)
            {
                WMS_LoadSettings();
                WMS_objectsCreated = true;
            }
            if (PlayerCache.Vitals.Name != "")
            {
                WMSDataSet.CharacterInfoRow[] localCharRow = (WMSDataSet.CharacterInfoRow[])WMS_dataset.CharacterInfo.Select("Name='" + PlayerCache.Vitals.Name + "'");
                if (localCharRow.Length == 0)
                {
                    Statics.Settings.WMS.SaveThisCharOffline = false;
                }
                else
                {
                    Statics.Settings.WMS.SaveThisCharOffline = true;
                }
            }

            if (m_TOP_Thread_wms == null)
            {
                m_TOP_Thread_wms = new IocaineThread("wmsThread");
                m_TOP_Thread_wms.__RunMethod = WMS_BackgroundScanThreadFunction;
            }
            m_TOP_Thread_wms.Start();

            return true;
        }
        private void WMS_LoadSettings()
        {
            Statics.Settings.WMS.BackgroundUpdate = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.WMS, "WMSBackgroundUpdate"));
            Statics.Settings.WMS.BackgroundScanPeriod = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.WMS, "WMSBackgroundScanPeriod"));
        }
        private void WMS_Init_GUI()
        {
            if (m_TOP_wmsGuiInitDone)
            {
                return;
            }
            if (WMS_poolInventory)
            {
                WMS_BagLabel.Text = "Pooled Inventory";
                WMS_sendControlTo(WMS_PooledLB, true);
                WMS_updateControlVisibility(WMS_SatchelOccLabel, false);
                WMS_updateControlVisibility(WMS_SackOccLabel, false);
                WMS_updateControlVisibility(WMS_CaseOccLabel, false);
                WMS_updateControlVisibility(WMS_SafeOccLabel, false);
                WMS_updateControlVisibility(WMS_Safe2OccLabel, false);
                WMS_updateControlVisibility(WMS_StorageOccLabel, false);
                WMS_updateControlVisibility(WMS_LockerOccLabel, false);
                WMS_updateControlVisibility(WMS_WardrobeOccLabel, false);
                WMS_updateControlVisibility(WMS_Wardrobe2OccLabel, false);
                WMS_updateControlVisibility(WMS_Wardrobe3OccLabel, false);
                WMS_updateControlVisibility(WMS_Wardrobe4OccLabel, false);
            }
            else
            {
                WMS_updateLabelText(WMS_BagLabel, "Gobbie Bag");
            }
            return;
        }
        private void WMS_Init_Dataset()
        {
            //Save settings if this isn't the first time doing the inits (we've been logged in already)
            LoggingFunctions.Debug("TopWMS::doWMSDatasetInits: wms char = " + PlayerCache.Vitals.Name + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (WMS_dataset != null)
            {
                LoggingFunctions.Debug("TopWMS::doWMSDatasetInits: nb db rows is " + WMS_dataset.Items.Rows.Count + ".", LoggingFunctions.DBG_SCOPE.TOP);
            }
            //WMS_writeDatasetToXML();

            if (WMS_dataset != null)
            {
                WMS_dataset.Clear();
            }
            WMS_dataset = null;
            WMS_dataset = new WMSDataSet();
            if (WMS_dataset_temp != null)
            {
                WMS_dataset_temp.Clear();
            }
            WMS_dataset_temp = null;
            WMS_dataset_temp = new WMSDataSet();
            if (Directory.Exists(WMS_dataDirectory))
            {
                //Get a list of the WMS files (per character) and load each one.
                String[] fileList = Directory.GetFiles(WMS_dataDirectory);
                foreach (String file in fileList)
                {
                    LoggingFunctions.Debug("Loading WMS XML file " + file + ".", LoggingFunctions.DBG_SCOPE.WMS);
                    WMS_dataset_temp.Clear();
                    WMS_dataset_temp.ReadXml(file);
                    foreach (WMSDataSet.CharacterInfoRow row in WMS_dataset_temp.CharacterInfo.Rows)
                    {
                        LoggingFunctions.Debug("Adding WMS Character " + row["Name"] + " to the main DB.", LoggingFunctions.DBG_SCOPE.WMS);
                        WMSDataSet.CharacterInfoRow charRow = WMS_dataset.CharacterInfo.NewCharacterInfoRow();
                        charRow.Name = row.Name;
                        charRow.BagOcc = row.BagOcc;
                        charRow.BagCap = row.BagCap;
                        charRow.SatchelOcc = row.SatchelOcc;
                        charRow.SatchelCap = row.SatchelCap;
                        charRow.SackOcc = row.SackOcc;
                        charRow.SackCap = row.SackCap;
                        charRow.CaseOcc = row.CaseOcc;
                        charRow.CaseCap = row.CaseCap;
                        charRow.SafeOcc = row.SafeOcc;
                        charRow.SafeCap = row.SafeCap;
                        charRow.Safe2Occ = row.Safe2Occ;
                        charRow.Safe2Cap = row.Safe2Cap;
                        charRow.StorageOcc = row.StorageOcc;
                        charRow.StorageCap = row.StorageCap;
                        charRow.LockerOcc = row.LockerOcc;
                        charRow.LockerCap = row.LockerCap;
                        charRow.WardrobeOcc = row.WardrobeOcc;
                        charRow.WardrobeCap = row.WardrobeCap;
                        charRow.Wardrobe2Occ = row.Wardrobe2Occ;
                        charRow.Wardrobe2Cap = row.Wardrobe2Cap;
                        charRow.Wardrobe3Occ = row.Wardrobe3Occ;
                        charRow.Wardrobe3Cap = row.Wardrobe3Cap;
                        charRow.Wardrobe4Occ = row.Wardrobe4Occ;
                        charRow.Wardrobe4Cap = row.Wardrobe4Cap;
                        charRow.DateSaved = row.DateSaved;
                        WMS_dataset.CharacterInfo.Rows.Add(charRow);
                    }
                    WMS_dataset.CharacterInfo.AcceptChanges();

                    foreach (WMSDataSet.ItemsRow row in WMS_dataset_temp.Items.Rows)
                    {
                        LoggingFunctions.Debug("Adding WMS Item " + row["Name"] + " to the main DB.", LoggingFunctions.DBG_SCOPE.WMS);
                        WMSDataSet.ItemsRow itemRow = WMS_dataset.Items.NewItemsRow();
                        itemRow.Character = row.Character;
                        itemRow.Name = row.Name;
                        itemRow.Quantity = row.Quantity;
                        itemRow.Location = row.Location;
                        WMS_dataset.Items.Rows.Add(itemRow);
                    }
                    WMS_dataset.Items.AcceptChanges();
                }
            }
            WMS_loadCharacterCB();
        }
        #endregion Initialization
        #region Event Handlers
        private void WMS_RefreshButton_Click(object sender, EventArgs e)
        {
            WMS_rebuildLists();
            WMS_UpdateListboxes();
            WMS_UpdateDataset();
        }
        private void WMS_UpdateListboxes()
        {
            //First check if
            //1) we're pooling everyone
            //or 2) we've selected an offline character to display.
            //If either of these are the case, we don't need to have the inits all done and the objects created.
            //However, we DO need the DB to be initialized. This should be done already if we're on the WMS tab.

            //mahampto - Based on the criteria above, if #1 or #2 are true, we don't need to be locked to a POL process, right?
            //           Maybe we need a different update function specifically for offline use? So we can bypass all of the memreads.
            if ((ChangeMonitor.MainProc == null) || (ChangeMonitor.MainModule == null) || !MemReads.PointersSet)
            {
                return;
            }
            String currChar = PlayerCache.Vitals.Name;
            if (WMS_dataset != null)
            {
                //If we're pooling all characters inventory, select everything.
                if (WMS_allCharacters)
                {
                    WMSDataSet.ItemsRow[] itemRows;
                    itemRows = (WMSDataSet.ItemsRow[])WMS_dataset.Items.Select();
                    WMS_updatePooledLB(itemRows);
                    WMS_setOccLabels(null);
                    return;
                }
                else if ((WMS_selectedChar != "") && (currChar != WMS_selectedChar))
                {
                    WMSDataSet.ItemsRow[] itemRows;
                    foreach (String invType in WMS_inventoryTypes)
                    {
                        itemRows = (WMSDataSet.ItemsRow[])WMS_dataset.Items.Select("Character='" + WMS_selectedChar + "' AND Location='" + invType + "'");
                        if (itemRows.Length > 0)
                        {
                            WMS_updateSingleLB(null, itemRows);
                        }
                    }
                    itemRows = (WMSDataSet.ItemsRow[])WMS_dataset.Items.Select("Character='" + WMS_selectedChar + "'");
                    WMS_updatePooledLB(itemRows);
                    WMSDataSet.CharacterInfoRow[] infoRows = (WMSDataSet.CharacterInfoRow[])WMS_dataset.CharacterInfo.Select("Name='" + WMS_selectedChar + "'");
                    if (infoRows.Length > 0)
                    {
                        WMS_setOccLabels(infoRows[0]);
                    }
                    return;
                }
            }

            if (!WMS_objectsCreated)
            {
                return;
            }
            foreach (ItemContainer container in Containers.All)
            {
                WMS_updateSingleLB(container, null);
            }
            WMS_updatePooledLB(null);
            WMS_setOccLabels(null);
        }
        private void WMS_UpdateDataset()
        {
            try
            {
                if ((ChangeMonitor.MainProc == null) || (ChangeMonitor.MainModule == null) || !MemReads.PointersSet)
                {
                    return;
                }
                Monitor.Enter(Inventory.Containers.Padlock);
                String currChar = MemReads.Self.get_name(true);
                WMSDataSet.CharacterInfoRow[] localCharRows = (WMSDataSet.CharacterInfoRow[])WMS_dataset.CharacterInfo.Select("Name='" + currChar + "'");
                if (localCharRows.Length == 0)
                {
                    WMSDataSet.CharacterInfoRow rowToAdd = WMS_dataset.CharacterInfo.NewCharacterInfoRow();
                    rowToAdd.Name = currChar;
                    rowToAdd.BagOcc = Containers.Bag.Occupancy;
                    rowToAdd.BagCap = Containers.Bag.Capacity;
                    rowToAdd.SatchelOcc = Containers.Satchel.Occupancy;
                    rowToAdd.SatchelCap = Containers.Satchel.Capacity;
                    rowToAdd.SackOcc = Containers.Sack.Occupancy;
                    rowToAdd.SackCap = Containers.Sack.Capacity;
                    rowToAdd.CaseOcc = Containers.MCase.Occupancy;
                    rowToAdd.CaseCap = Containers.MCase.Capacity;
                    rowToAdd.SafeOcc = Containers.Safe.Occupancy;
                    rowToAdd.SafeCap = Containers.Safe.Capacity;
                    rowToAdd.Safe2Occ = Containers.Safe2.Occupancy;
                    rowToAdd.Safe2Cap = Containers.Safe2.Capacity;
                    rowToAdd.StorageOcc = Containers.Storage.Occupancy;
                    rowToAdd.StorageCap = Containers.Storage.Capacity;
                    rowToAdd.LockerOcc = Containers.Locker.Occupancy;
                    rowToAdd.LockerCap = Containers.Locker.Capacity;
                    rowToAdd.WardrobeOcc = Containers.Wardrobe.Occupancy;
                    rowToAdd.WardrobeCap = Containers.Wardrobe.Capacity;
                    rowToAdd.Wardrobe2Occ = Containers.Wardrobe2.Occupancy;
                    rowToAdd.Wardrobe2Cap = Containers.Wardrobe2.Capacity;
                    rowToAdd.Wardrobe3Occ = Containers.Wardrobe3.Occupancy;
                    rowToAdd.Wardrobe3Cap = Containers.Wardrobe3.Capacity;
                    rowToAdd.Wardrobe4Occ = Containers.Wardrobe4.Occupancy;
                    rowToAdd.Wardrobe4Cap = Containers.Wardrobe4.Capacity;
                    rowToAdd.DateSaved = DateTime.Now;
                    WMS_dataset.CharacterInfo.Rows.Add(rowToAdd);
                    WMS_dataset.CharacterInfo.AcceptChanges();
                }
                else
                {
                    WMSDataSet.CharacterInfoRow rowToUpdate = localCharRows[0];
                    rowToUpdate.BagOcc = Containers.Bag.Occupancy;
                    rowToUpdate.BagCap = Containers.Bag.Capacity;
                    rowToUpdate.SatchelOcc = Containers.Satchel.Occupancy;
                    rowToUpdate.SatchelCap = Containers.Satchel.Capacity;
                    rowToUpdate.SackOcc = Containers.Sack.Occupancy;
                    rowToUpdate.SackCap = Containers.Sack.Capacity;
                    rowToUpdate.CaseOcc = Containers.MCase.Occupancy;
                    rowToUpdate.CaseCap = Containers.MCase.Capacity;
                    rowToUpdate.SafeOcc = Containers.Safe.Occupancy;
                    rowToUpdate.SafeCap = Containers.Safe.Capacity;
                    rowToUpdate.Safe2Occ = Containers.Safe2.Occupancy;
                    rowToUpdate.Safe2Cap = Containers.Safe2.Capacity;
                    rowToUpdate.StorageOcc = Containers.Storage.Occupancy;
                    rowToUpdate.StorageCap = Containers.Storage.Capacity;
                    rowToUpdate.LockerOcc = Containers.Locker.Occupancy;
                    rowToUpdate.LockerCap = Containers.Locker.Capacity;
                    rowToUpdate.WardrobeOcc = Containers.Wardrobe.Occupancy;
                    rowToUpdate.WardrobeCap = Containers.Wardrobe.Capacity;
                    rowToUpdate.Wardrobe2Occ = Containers.Wardrobe2.Occupancy;
                    rowToUpdate.Wardrobe2Cap = Containers.Wardrobe2.Capacity;
                    rowToUpdate.Wardrobe3Occ = Containers.Wardrobe3.Occupancy;
                    rowToUpdate.Wardrobe3Cap = Containers.Wardrobe3.Capacity;
                    rowToUpdate.Wardrobe4Occ = Containers.Wardrobe4.Occupancy;
                    rowToUpdate.Wardrobe4Cap = Containers.Wardrobe4.Capacity;
                    rowToUpdate.DateSaved = DateTime.Now;
                    WMS_dataset.CharacterInfo.AcceptChanges();
                }
                WMSDataSet.ItemsRow[] localItemRows = (WMSDataSet.ItemsRow[])WMS_dataset.Items.Select("Character='" + currChar + "'");
                foreach (WMSDataSet.ItemsRow rowToRemove in localItemRows)
                {
                    WMS_dataset.Items.RemoveItemsRow(rowToRemove);
                }
                foreach (ItemContainer container in Containers.All)
                {
                    List<Item> itemList = container.GetSummaryItemList();
                    List<ushort> itemQuan = container.GetSummaryItemQuanList();
                    int cnt = itemList.Count;
                    for (int ii = 0; ii < cnt; ii++)
                    {
                        WMSDataSet.ItemsRow rowToAdd = WMS_dataset.Items.NewItemsRow();
                        rowToAdd.Character = currChar;
                        rowToAdd.Name = itemList[ii].Name;
                        rowToAdd.Quantity = itemQuan[ii];
                        rowToAdd.Location = container.TypeString;
                        WMS_dataset.Items.Rows.Add(rowToAdd);
                    }
                }
                WMS_dataset.Items.AcceptChanges();
                LoggingFunctions.Debug("WMS_UpdateDataset: Writing to XML.", LoggingFunctions.DBG_SCOPE.WMS);
                WMS_writeDatasetToXML();
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("WMS_UpdateDataset: " + e.ToString());
            }
            finally
            {
                Monitor.Exit(Inventory.Containers.Padlock);
            }
        }
        private void WMS_PoolInventoryRB_CheckedChanged(object sender, EventArgs e)
        {
            if (WMS_PoolInventoryRB.Checked)
            {
                WMS_poolInventory = true;
                WMS_separateInventory = false;
                WMS_allCharacters = false;
                WMS_updateControlVisibility(WMS_BagLB, false);
                WMS_updateControlVisibility(WMS_SatchelLB, false);
                WMS_updateControlVisibility(WMS_SatchelLabel, false);
                WMS_updateControlVisibility(WMS_SackLB, false);
                WMS_updateControlVisibility(WMS_SackLabel, false);
                WMS_updateControlVisibility(WMS_CaseLB, false);
                WMS_updateControlVisibility(WMS_CaseLabel, false);
                WMS_updateControlVisibility(WMS_SafeLB, false);
                WMS_updateControlVisibility(WMS_SafeLabel, false);
                WMS_updateControlVisibility(WMS_Safe2LB, false);
                WMS_updateControlVisibility(WMS_Safe2Label, false);
                WMS_updateControlVisibility(WMS_StorageLB, false);
                WMS_updateControlVisibility(WMS_StorageLabel, false);
                WMS_updateControlVisibility(WMS_LockerLB, false);
                WMS_updateControlVisibility(WMS_LockerLabel, false);
                WMS_updateControlVisibility(WMS_WardrobeLB, false);
                WMS_updateControlVisibility(WMS_WardrobeLabel, false);
                WMS_updateControlVisibility(WMS_Wardrobe2LB, false);
                WMS_updateControlVisibility(WMS_Wardrobe2Label, false);
                WMS_updateControlVisibility(WMS_Wardrobe3LB, false);
                WMS_updateControlVisibility(WMS_Wardrobe3Label, false);
                WMS_updateControlVisibility(WMS_Wardrobe4LB, false);
                WMS_updateControlVisibility(WMS_Wardrobe4Label, false);
                WMS_updateLabelText(WMS_BagLabel, "Pooled Inventory");
                WMS_updateControlVisibility(WMS_PooledLB, true);
                WMS_rebuildLists();
                WMS_UpdateListboxes();
                if ((WMS_CharacterCB.Text != "") && (WMS_selectedChar != PlayerCache.Vitals.Name))
                {
                    WMSDataSet.CharacterInfoRow[] infoRows = (WMSDataSet.CharacterInfoRow[])WMS_dataset.CharacterInfo.Select("Name='" + WMS_selectedChar + "'");
                    if (infoRows.Length > 0)
                    {
                        WMS_setOccLabels(infoRows[0]);
                    }
                    else
                    {
                        WMS_loadCharacterCB();
                        infoRows = (WMSDataSet.CharacterInfoRow[])WMS_dataset.CharacterInfo.Select("Name='" + WMS_selectedChar + "'");
                        if (infoRows.Length > 0)
                        {
                            WMS_CharacterCB.SelectedItem = PlayerCache.Vitals.Name;
                        }
                        else if (WMS_CharacterCB.Items.Count > 0)
                        {
                            WMS_CharacterCB.SelectedIndex = 0;
                        }
                    }
                }
                else
                {
                    WMS_setOccLabels(null);
                }
                WMS_sendControlTo(WMS_PooledLB, true);
                WMS_setFirstInventoryLocation(96);
            }
        }
        private void WMS_SeparateInventoryRB_CheckedChanged(object sender, EventArgs e)
        {
            if (WMS_SeparateInventoryRB.Checked)
            {
                WMS_poolInventory = false;
                WMS_separateInventory = true;
                WMS_allCharacters = false;
                WMS_updateControlVisibility(WMS_PooledLB, false);
                WMS_updateLabelText(WMS_BagLabel, "Gobbie Bag");
                WMS_updateControlVisibility(WMS_BagLB, true);
                WMS_updateControlVisibility(WMS_SatchelLB, true);
                WMS_updateControlVisibility(WMS_SatchelLabel, true);
                WMS_updateControlVisibility(WMS_SackLB, true);
                WMS_updateControlVisibility(WMS_SackLabel, true);
                WMS_updateControlVisibility(WMS_CaseLB, true);
                WMS_updateControlVisibility(WMS_CaseLabel, true);
                WMS_updateControlVisibility(WMS_SafeLB, true);
                WMS_updateControlVisibility(WMS_SafeLabel, true);
                WMS_updateControlVisibility(WMS_Safe2LB, true);
                WMS_updateControlVisibility(WMS_Safe2Label, true);
                WMS_updateControlVisibility(WMS_StorageLB, true);
                WMS_updateControlVisibility(WMS_StorageLabel, true);
                WMS_updateControlVisibility(WMS_LockerLB, true);
                WMS_updateControlVisibility(WMS_LockerLabel, true);
                WMS_updateControlVisibility(WMS_WardrobeLB, true);
                WMS_updateControlVisibility(WMS_WardrobeLabel, true);
                WMS_updateControlVisibility(WMS_Wardrobe2LB, true);
                WMS_updateControlVisibility(WMS_Wardrobe2Label, true);
                WMS_updateControlVisibility(WMS_Wardrobe3LB, true);
                WMS_updateControlVisibility(WMS_Wardrobe3Label, true);
                WMS_updateControlVisibility(WMS_Wardrobe4LB, true);
                WMS_updateControlVisibility(WMS_Wardrobe4Label, true);
                WMS_rebuildLists();
                WMS_UpdateListboxes();
                if ((WMS_CharacterCB.Text != "") && (WMS_selectedChar != PlayerCache.Vitals.Name))
                {
                    WMSDataSet.CharacterInfoRow[] infoRows = (WMSDataSet.CharacterInfoRow[])WMS_dataset.CharacterInfo.Select("Name='" + WMS_selectedChar + "'");
                    if (infoRows.Length > 0)
                    {
                        WMS_setOccLabels(infoRows[0]);
                    }
                    else
                    {
                        WMS_loadCharacterCB();
                        infoRows = (WMSDataSet.CharacterInfoRow[])WMS_dataset.CharacterInfo.Select("Name='" + WMS_selectedChar + "'");
                        if (infoRows.Length > 0)
                        {
                            WMS_CharacterCB.SelectedItem = PlayerCache.Vitals.Name;
                        }
                        else if (WMS_CharacterCB.Items.Count > 0)
                        {
                            WMS_CharacterCB.SelectedIndex = 0;
                        }
                    }
                }
                else
                {
                    WMS_setOccLabels(null);
                }
                WMS_setFirstInventoryLocation(70);
            }
        }
        private void WMS_AllCharactersRB_CheckedChanged(object sender, EventArgs e)
        {
            if (WMS_AllCharactersRB.Checked)
            {
                WMS_poolInventory = false;
                WMS_separateInventory = false;
                WMS_allCharacters = true;
                WMS_updateControlVisibility(WMS_BagLB, false);
                WMS_updateControlVisibility(WMS_SatchelLB, false);
                WMS_updateControlVisibility(WMS_SatchelLabel, false);
                WMS_updateControlVisibility(WMS_SackLB, false);
                WMS_updateControlVisibility(WMS_SackLabel, false);
                WMS_updateControlVisibility(WMS_CaseLB, false);
                WMS_updateControlVisibility(WMS_CaseLabel, false);
                WMS_updateControlVisibility(WMS_SafeLB, false);
                WMS_updateControlVisibility(WMS_SafeLabel, false);
                WMS_updateControlVisibility(WMS_Safe2LB, false);
                WMS_updateControlVisibility(WMS_Safe2Label, false);
                WMS_updateControlVisibility(WMS_StorageLB, false);
                WMS_updateControlVisibility(WMS_StorageLabel, false);
                WMS_updateControlVisibility(WMS_LockerLB, false);
                WMS_updateControlVisibility(WMS_LockerLabel, false);
                WMS_updateControlVisibility(WMS_WardrobeLB, false);
                WMS_updateControlVisibility(WMS_WardrobeLabel, false);
                WMS_updateControlVisibility(WMS_Wardrobe2LB, false);
                WMS_updateControlVisibility(WMS_Wardrobe2Label, false);
                WMS_updateControlVisibility(WMS_Wardrobe3LB, false);
                WMS_updateControlVisibility(WMS_Wardrobe3Label, false);
                WMS_updateControlVisibility(WMS_Wardrobe4LB, false);
                WMS_updateControlVisibility(WMS_Wardrobe4Label, false);
                WMS_updateLabelText(WMS_BagLabel, "All Inventory");
                WMS_updateControlVisibility(WMS_PooledLB, true);
                WMS_rebuildLists();
                WMS_UpdateListboxes();
                WMS_setOccLabels(null);
                WMS_sendControlTo(WMS_PooledLB, true);
            }
        }
        private void WMS_CharacterCB_Click(object sender, EventArgs e)
        {
            WMS_loadCharacterCB();
        }
        private void WMS_CharacterCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(WMS_CharacterCB.Text != "")
            {
                WMS_selectedChar = WMS_CharacterCB.Text;
            }
            WMS_rebuildLists();
            WMS_UpdateListboxes();
            WMS_initialLBUpdateDone = true;
        }
        private void WMS_Form_ResizeEnd(object sender, EventArgs e)
        {
            Int32 wms_tab_x_diff = 549 - 525;
            Int32 wms_tab_y_diff = 535 - 403;
            Int32 pooled_lb_x_diff = 549 - 525;
            Int32 pooled_lb_y_diff = 535 - 355;
            UInt32 nb_boxes_wide = 4;
            UInt32 nb_boxes_high = 2;
            Int32 buffer_between_x = 6;
            Int32 buffer_between_y = 17;
            Int32 buffer_left = 0;
            Int32 buffer_right = 0;
            Int32 buffer_top = 38;
            Int32 buffer_bottom = 10;
            Int32 new_box_width = (m_TOP_Form_currentWidth - wms_tab_x_diff - buffer_left - buffer_right - (((Int32)nb_boxes_wide - 1) * buffer_between_x)) / (Int32)nb_boxes_wide;
            Int32 new_box_height = (m_TOP_Form_currentWidth - wms_tab_y_diff - buffer_top - buffer_bottom - (((Int32)nb_boxes_high - 1) * buffer_between_y)) / (Int32)nb_boxes_high;

            WMS_Tab.Width = m_TOP_Form_currentWidth - wms_tab_x_diff;
            WMS_Tab.Height = m_TOP_Form_currentWidth - wms_tab_y_diff;

            WMS_PooledLB.Width = m_TOP_Form_currentWidth - pooled_lb_x_diff;
            WMS_PooledLB.Height = m_TOP_Form_currentWidth - pooled_lb_y_diff;

            WMS_BagLB.Width = new_box_width;
            WMS_BagLB.Height = new_box_height;

            WMS_SatchelLB.Width = new_box_width;
            WMS_SatchelLB.Height = new_box_height;
            WMS_SatchelLB.Left = buffer_left + (new_box_width * 1) + (buffer_between_x * 1);
            WMS_SatchelLabel.Left = WMS_SatchelLB.Left + 3;
            WMS_SatchelOccLabel.Left = WMS_SatchelLabel.Left + 52;

            WMS_SackLB.Width = new_box_width;
            WMS_SackLB.Height = new_box_height;
            WMS_SackLB.Left = buffer_left + (new_box_width * 2) + (buffer_between_x * 2);
            WMS_SackLabel.Left = WMS_SackLB.Left + 3;
            WMS_SackOccLabel.Left = WMS_SackLabel.Left + 41;

            WMS_CaseLB.Width = new_box_width;
            WMS_CaseLB.Height = new_box_height;
            WMS_CaseLB.Left = buffer_left + (new_box_width * 3) + (buffer_between_x * 3);
            WMS_CaseLabel.Left = WMS_CaseLB.Left + 3;
            WMS_CaseOccLabel.Left = WMS_CaseLabel.Left + 42;

            WMS_SafeLB.Width = new_box_width;
            WMS_SafeLB.Height = new_box_height;
            WMS_SafeLB.Top = buffer_top + (new_box_height * 1) + (buffer_between_y * 1);
            WMS_SafeLabel.Top = WMS_SafeLB.Top - 14;
            WMS_SafeOccLabel.Top = WMS_SafeLabel.Top;

            WMS_StorageLB.Width = new_box_width;
            WMS_StorageLB.Height = new_box_height;
            WMS_StorageLB.Left = buffer_left + (new_box_width * 1) + (buffer_between_x * 1);
            WMS_StorageLB.Top = buffer_top + (new_box_height * 1) + (buffer_between_y * 1);
            WMS_StorageLabel.Left = WMS_StorageLB.Left + 3;
            WMS_StorageLabel.Top = WMS_StorageLB.Top - 14;
            WMS_StorageOccLabel.Left = WMS_StorageLabel.Left + 53;
            WMS_StorageOccLabel.Top = WMS_StorageLabel.Top;

            WMS_LockerLB.Width = new_box_width;
            WMS_LockerLB.Height = new_box_height;
            WMS_LockerLB.Left = buffer_left + (new_box_width * 2) + (buffer_between_x * 2);
            WMS_LockerLB.Top = buffer_top + (new_box_height * 1) + (buffer_between_y * 1);
            WMS_LockerLabel.Left = WMS_LockerLB.Left + 3;
            WMS_LockerLabel.Top = WMS_LockerLB.Top - 14;
            WMS_LockerOccLabel.Left = WMS_LockerLabel.Left + 49;
            WMS_LockerOccLabel.Top = WMS_LockerLabel.Top;

            WMS_WardrobeLB.Width = new_box_width;
            WMS_WardrobeLB.Height = new_box_height;
            WMS_WardrobeLB.Left = buffer_left + (new_box_width * 3) + (buffer_between_x * 3);
            WMS_WardrobeLB.Top = buffer_top + (new_box_height * 1) + (buffer_between_y * 1);
            WMS_WardrobeLabel.Left = WMS_WardrobeLB.Left + 3;
            WMS_WardrobeLabel.Top = WMS_WardrobeLB.Top - 14;
            WMS_WardrobeOccLabel.Left = WMS_WardrobeLabel.Left + 63;
            WMS_WardrobeOccLabel.Top = WMS_WardrobeLabel.Top;
        }
        #endregion Event Handlers
        #region Utility Functions
        private ListBox WMS_getListbox(ItemContainer container, WMSDataSet.ItemsRow[] itemRows)
        {
            ListBox lb = null;
            if (container != null)
            {
                switch (container.StorageType)
                {
                    case ItemContainer.STORAGE_TYPE.BAG:
                        lb = WMS_BagLB;
                        break;
                    case ItemContainer.STORAGE_TYPE.SATCHEL:
                        lb = WMS_SatchelLB;
                        break;
                    case ItemContainer.STORAGE_TYPE.SACK:
                        lb = WMS_SackLB;
                        break;
                    case ItemContainer.STORAGE_TYPE.CASE:
                        lb = WMS_CaseLB;
                        break;
                    case ItemContainer.STORAGE_TYPE.SAFE:
                        lb = WMS_SafeLB;
                        break;
                    case ItemContainer.STORAGE_TYPE.SAFE2:
                        lb = WMS_Safe2LB;
                        break;
                    case ItemContainer.STORAGE_TYPE.STORAGE:
                        lb = WMS_StorageLB;
                        break;
                    case ItemContainer.STORAGE_TYPE.LOCKER:
                        lb = WMS_LockerLB;
                        break;
                    case ItemContainer.STORAGE_TYPE.WARDROBE:
                        lb = WMS_WardrobeLB;
                        break;
                    case ItemContainer.STORAGE_TYPE.WARDROBE2:
                        lb = WMS_Wardrobe2LB;
                        break;
                    case ItemContainer.STORAGE_TYPE.WARDROBE3:
                        lb = WMS_Wardrobe3LB;
                        break;
                    case ItemContainer.STORAGE_TYPE.WARDROBE4:
                        lb = WMS_Wardrobe4LB;
                        break;
                    default:
                        lb = WMS_BagLB;
                        break;
                }
            }
            else if ((itemRows != null) && (itemRows.Length > 0))
            {
                WMSDataSet.ItemsRow row = itemRows[0];
                switch (row.Location)
                {
                    case "Bag":
                        lb = WMS_BagLB;
                        break;
                    case "Satchel":
                        lb = WMS_SatchelLB;
                        break;
                    case "Sack":
                        lb = WMS_SackLB;
                        break;
                    case "Case":
                        lb = WMS_CaseLB;
                        break;
                    case "Safe":
                        lb = WMS_SafeLB;
                        break;
                    case "Safe2":
                        lb = WMS_Safe2LB;
                        break;
                    case "Storage":
                        lb = WMS_StorageLB;
                        break;
                    case "Locker":
                        lb = WMS_LockerLB;
                        break;
                    case "Wardrobe":
                        lb = WMS_WardrobeLB;
                        break;
                    case "Wardrobe2":
                        lb = WMS_Wardrobe2LB;
                        break;
                    case "Wardrobe3":
                        lb = WMS_Wardrobe3LB;
                        break;
                    case "Wardrobe4":
                        lb = WMS_Wardrobe4LB;
                        break;
                    default:
                        lb = WMS_BagLB;
                        break;
                }
            }
            return lb;
        }
        private void WMS_rebuildLists()
        {
            if (!WMS_objectsCreated)
            {
                return;
            }
            if (WMS_checkBadInventory())
            {
                return;
            }
            Containers.RebuildLists();
        }
        private void WMS_setOccLabels(WMSDataSet.CharacterInfoRow charRow)
        {
            if (WMS_poolInventory)
            {
                WMS_updateControlVisibility(WMS_SatchelOccLabel, false);
                WMS_updateControlVisibility(WMS_SackOccLabel, false);
                WMS_updateControlVisibility(WMS_CaseOccLabel, false);
                WMS_updateControlVisibility(WMS_SafeOccLabel, false);
                WMS_updateControlVisibility(WMS_Safe2OccLabel, false);
                WMS_updateControlVisibility(WMS_StorageOccLabel, false);
                WMS_updateControlVisibility(WMS_LockerOccLabel, false);
                WMS_updateControlVisibility(WMS_WardrobeLabel, false);
                WMS_updateControlVisibility(WMS_Wardrobe2Label, false);
                WMS_updateControlVisibility(WMS_Wardrobe3Label, false);
                WMS_updateControlVisibility(WMS_Wardrobe4Label, false);
                if (charRow != null)
                {
                    int totalOcc = charRow.BagOcc + charRow.SatchelOcc + charRow.SackOcc + charRow.CaseOcc + charRow.SafeOcc + charRow.Safe2Occ;
                    totalOcc += charRow.StorageOcc + charRow.LockerOcc + charRow.WardrobeOcc + charRow.Wardrobe2Occ + charRow.Wardrobe3Occ + charRow.Wardrobe4Occ;
                    int totalSpace = charRow.BagCap + charRow.SatchelCap + charRow.SackCap + charRow.CaseCap + charRow.SafeCap + charRow.Safe2Cap;
                    totalSpace += charRow.StorageCap + charRow.LockerCap + charRow.WardrobeCap + charRow.Wardrobe2Cap + charRow.Wardrobe3Cap + charRow.Wardrobe4Cap;
                    WMS_updateLabelText(WMS_BagOccLabel, totalOcc.ToString() + " / " + totalSpace.ToString());
                }
                else if (ChangeMonitor.MainProc == null || ChangeMonitor.MainModule == null)
                {
                    WMS_updateLabelText(WMS_BagOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_SatchelOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_SackOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_CaseOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_SafeOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_Safe2OccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_StorageOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_LockerOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_WardrobeOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_Wardrobe2OccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_Wardrobe3OccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_Wardrobe4OccLabel, "0 / 0");
                }
                else
                {
                    int totalOcc = 0;
                    int totalSpace = 0;
                    Monitor.Enter(Inventory.Containers.Padlock);
                    foreach (ItemContainer container in Containers.All)
                    {
                        totalOcc += container.Occupancy;
                        totalSpace += container.Capacity;
                    }
                    Monitor.Exit(Inventory.Containers.Padlock);
                    WMS_updateLabelText(WMS_BagOccLabel, totalOcc.ToString() + " / " + totalSpace.ToString());
                }
            }
            else if(WMS_separateInventory)
            {
                WMS_updateControlVisibility(WMS_SatchelOccLabel, true);
                WMS_updateControlVisibility(WMS_SackOccLabel, true);
                WMS_updateControlVisibility(WMS_CaseOccLabel, true);
                WMS_updateControlVisibility(WMS_SafeOccLabel, true);
                WMS_updateControlVisibility(WMS_Safe2OccLabel, true);
                WMS_updateControlVisibility(WMS_StorageOccLabel, true);
                WMS_updateControlVisibility(WMS_LockerOccLabel, true);
                WMS_updateControlVisibility(WMS_WardrobeOccLabel, true);
                WMS_updateControlVisibility(WMS_Wardrobe2OccLabel, true);
                WMS_updateControlVisibility(WMS_Wardrobe3OccLabel, true);
                WMS_updateControlVisibility(WMS_Wardrobe4OccLabel, true);
                if (charRow != null)
                {
                    WMS_updateLabelText(WMS_BagOccLabel, charRow.BagOcc + " / " + charRow.BagCap);
                    WMS_updateLabelText(WMS_SatchelOccLabel, charRow.SatchelOcc + " / " + charRow.SatchelCap);
                    WMS_updateLabelText(WMS_SackOccLabel, charRow.SackOcc + " / " + charRow.SackCap);
                    WMS_updateLabelText(WMS_CaseOccLabel, charRow.CaseOcc + " / " + charRow.CaseCap);
                    WMS_updateLabelText(WMS_SafeOccLabel, charRow.SafeOcc + " / " + charRow.SafeCap);
                    WMS_updateLabelText(WMS_Safe2OccLabel, charRow.Safe2Occ + " / " + charRow.Safe2Cap);
                    WMS_updateLabelText(WMS_StorageOccLabel, charRow.StorageOcc + " / " + charRow.StorageCap);
                    WMS_updateLabelText(WMS_LockerOccLabel, charRow.LockerOcc + " / " + charRow.LockerCap);
                    WMS_updateLabelText(WMS_WardrobeOccLabel, charRow.WardrobeOcc + " / " + charRow.WardrobeCap);
                    WMS_updateLabelText(WMS_Wardrobe2OccLabel, charRow.Wardrobe2Occ + " / " + charRow.Wardrobe2Cap);
                    WMS_updateLabelText(WMS_Wardrobe3OccLabel, charRow.Wardrobe3Occ + " / " + charRow.Wardrobe3Cap);
                    WMS_updateLabelText(WMS_Wardrobe4OccLabel, charRow.Wardrobe4Occ + " / " + charRow.Wardrobe4Cap);
                }
                else if (ChangeMonitor.MainProc == null || ChangeMonitor.MainModule == null)
                {
                    WMS_updateLabelText(WMS_BagOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_SatchelOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_SackOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_CaseOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_SafeOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_Safe2OccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_StorageOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_LockerOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_WardrobeOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_Wardrobe2OccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_Wardrobe3OccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_Wardrobe4OccLabel, "0 / 0");
                }
                else
                {
                    WMS_updateLabelText(WMS_BagOccLabel, Containers.Bag.Occupancy + " / " + Containers.Bag.Capacity);
                    WMS_updateLabelText(WMS_SatchelOccLabel, Containers.Satchel.Occupancy + " / " + Containers.Satchel.Capacity);
                    WMS_updateLabelText(WMS_SackOccLabel, Containers.Sack.Occupancy + " / " + Containers.Sack.Capacity);
                    WMS_updateLabelText(WMS_CaseOccLabel, Containers.MCase.Occupancy + " / " + Containers.MCase.Capacity);
                    WMS_updateLabelText(WMS_SafeOccLabel, Containers.Safe.Occupancy + " / " + Containers.Safe.Capacity);
                    WMS_updateLabelText(WMS_Safe2OccLabel, Containers.Safe2.Occupancy + " / " + Containers.Safe2.Capacity);
                    WMS_updateLabelText(WMS_StorageOccLabel, Containers.Storage.Occupancy + " / " + Containers.Storage.Capacity);
                    WMS_updateLabelText(WMS_LockerOccLabel, Containers.Locker.Occupancy + " / " + Containers.Locker.Capacity);
                    WMS_updateLabelText(WMS_WardrobeOccLabel, Containers.Wardrobe.Occupancy + " / " + Containers.Wardrobe.Capacity);
                    WMS_updateLabelText(WMS_Wardrobe2OccLabel, Containers.Wardrobe2.Occupancy + " / " + Containers.Wardrobe2.Capacity);
                    WMS_updateLabelText(WMS_Wardrobe3OccLabel, Containers.Wardrobe3.Occupancy + " / " + Containers.Wardrobe3.Capacity);
                    WMS_updateLabelText(WMS_Wardrobe4OccLabel, Containers.Wardrobe4.Occupancy + " / " + Containers.Wardrobe4.Capacity);
                }
            }
            else if (WMS_allCharacters)
            {
                WMS_updateControlVisibility(WMS_SatchelOccLabel, false);
                WMS_updateControlVisibility(WMS_SackOccLabel, false);
                WMS_updateControlVisibility(WMS_CaseOccLabel, false);
                WMS_updateControlVisibility(WMS_SafeOccLabel, false);
                WMS_updateControlVisibility(WMS_Safe2OccLabel, false);
                WMS_updateControlVisibility(WMS_StorageOccLabel, false);
                WMS_updateControlVisibility(WMS_LockerOccLabel, false);
                WMS_updateControlVisibility(WMS_WardrobeOccLabel, false);
                WMS_updateControlVisibility(WMS_Wardrobe2OccLabel, false);
                WMS_updateControlVisibility(WMS_Wardrobe3OccLabel, false);
                WMS_updateControlVisibility(WMS_Wardrobe4OccLabel, false);
                if (WMS_dataset != null)
                {
                    WMSDataSet.CharacterInfoRow[] charRows = (WMSDataSet.CharacterInfoRow[])WMS_dataset.CharacterInfo.Select();
                    int totalOcc = 0;
                    int totalSpace = 0;
                    foreach (WMSDataSet.CharacterInfoRow row in charRows)
                    {
                        totalOcc += row.BagOcc + row.SatchelOcc + row.SackOcc + row.CaseOcc + row.SafeOcc + row.Safe2Occ;
                        totalOcc += row.StorageOcc + row.LockerOcc + row.WardrobeOcc + row.Wardrobe2Occ + row.Wardrobe3Occ + row.Wardrobe4Occ;
                        totalSpace += row.BagCap + row.SatchelCap + row.SackCap + row.CaseCap + row.SafeCap + row.Safe2Cap;
                        totalSpace += row.StorageCap + row.LockerCap + row.WardrobeCap + row.Wardrobe2Cap + row.Wardrobe3Cap + row.Wardrobe4Cap;
                        WMS_updateLabelText(WMS_BagOccLabel, totalOcc.ToString() + " / " + totalSpace.ToString());
                    }
                }
            }
        }
        private void WMS_writeDatasetToXML()
        {
            if ((WMS_dataset != null) && (PlayerCache.Vitals.Name != ""))
            {
                try
                {
                    LoggingFunctions.Debug("WMSTop::WMS_writeDatasetToXML: Trying to write the WMS dataset to a file. WMS_currentChar: '" + PlayerCache.Vitals.Name + "'.", LoggingFunctions.DBG_SCOPE.WMS);
                    if (!Directory.Exists(WMS_dataDirectory))
                    {
                        Directory.CreateDirectory(WMS_dataDirectory);
                    }
                    LoggingFunctions.Debug("WMS dataset char rows: " + WMS_dataset.CharacterInfo.Rows.Count + ".", LoggingFunctions.DBG_SCOPE.WMS);
                    LoggingFunctions.Debug("WMS dataset item rows: " + WMS_dataset.Items.Rows.Count + ".", LoggingFunctions.DBG_SCOPE.WMS);
                    WMS_dataset_temp.CharacterInfo.Rows.Clear();
                    WMS_dataset_temp.Items.Rows.Clear();
                    WMS_dataset_temp.AcceptChanges();
                    LoggingFunctions.Debug("WMS dataset_temp char rows after clearing: " + WMS_dataset_temp.CharacterInfo.Rows.Count + ".", LoggingFunctions.DBG_SCOPE.WMS);
                    LoggingFunctions.Debug("WMS dataset_temp item rows after clearing: " + WMS_dataset_temp.Items.Rows.Count + ".", LoggingFunctions.DBG_SCOPE.WMS);
                    String myName = MemReads.Self.get_name(true);
                    String filter = "Name='" + myName + "'";
                    WMSDataSet.CharacterInfoRow[] topCharRow = (WMSDataSet.CharacterInfoRow[])WMS_dataset.CharacterInfo.Select(filter);
                    LoggingFunctions.Debug("WMS_writeDatasetToXML: temp DB select string for char info table is " + filter + ".", LoggingFunctions.DBG_SCOPE.WMS);
                    LoggingFunctions.Debug("WMS_writeDataSetToXML: CharacterInfo row count: " + WMS_dataset.CharacterInfo.Rows.Count + ".", LoggingFunctions.DBG_SCOPE.WMS);

                    foreach (WMSDataSet.CharacterInfoRow row in topCharRow)
                    {
                        LoggingFunctions.Debug("Adding WMS Character " + row["Name"] + " to the temp DB for writing.", LoggingFunctions.DBG_SCOPE.WMS);
                        WMSDataSet.CharacterInfoRow charRow = WMS_dataset_temp.CharacterInfo.NewCharacterInfoRow();
                        charRow.Name = row.Name;
                        charRow.BagOcc = row.BagOcc;
                        charRow.BagCap = row.BagCap;
                        charRow.SatchelOcc = row.SatchelOcc;
                        charRow.SatchelCap = row.SatchelCap;
                        charRow.SackOcc = row.SackOcc;
                        charRow.SackCap = row.SackCap;
                        charRow.CaseOcc = row.CaseOcc;
                        charRow.CaseCap = row.CaseCap;
                        charRow.SafeOcc = row.SafeOcc;
                        charRow.SafeCap = row.SafeCap;
                        charRow.Safe2Occ = row.Safe2Occ;
                        charRow.Safe2Cap = row.Safe2Cap;
                        charRow.StorageOcc = row.StorageOcc;
                        charRow.StorageCap = row.StorageCap;
                        charRow.LockerOcc = row.LockerOcc;
                        charRow.LockerCap = row.LockerCap;
                        charRow.WardrobeOcc = row.WardrobeOcc;
                        charRow.WardrobeCap = row.WardrobeCap;
                        charRow.Wardrobe2Occ = row.Wardrobe2Occ;
                        charRow.Wardrobe2Cap = row.Wardrobe2Cap;
                        charRow.Wardrobe3Occ = row.Wardrobe3Occ;
                        charRow.Wardrobe3Cap = row.Wardrobe3Cap;
                        charRow.Wardrobe4Occ = row.Wardrobe4Occ;
                        charRow.Wardrobe4Cap = row.Wardrobe4Cap;
                        charRow.DateSaved = row.DateSaved;
                        WMS_dataset_temp.CharacterInfo.Rows.Add(charRow);
                    }
                    WMS_dataset_temp.CharacterInfo.AcceptChanges();
                    LoggingFunctions.Debug("WMS_writeDatasetToXML: temp DB CharacterInfo rows: " + WMS_dataset_temp.CharacterInfo.Rows.Count + ".", LoggingFunctions.DBG_SCOPE.WMS);

                    filter = "Character='" + myName + "'";
                    WMSDataSet.ItemsRow[] topItemRows = (WMSDataSet.ItemsRow[])WMS_dataset.Items.Select(filter);
                    LoggingFunctions.Debug("WMS_writeDatasetToXML: temp DB select string for items table is " + filter + ".", LoggingFunctions.DBG_SCOPE.WMS);

                    foreach (WMSDataSet.ItemsRow row in topItemRows)
                    {
                        LoggingFunctions.Debug("Adding WMS Item " + row["Name"] + " to the temp DB for writing.", LoggingFunctions.DBG_SCOPE.WMS);
                        WMSDataSet.ItemsRow itemRow = WMS_dataset_temp.Items.NewItemsRow();
                        itemRow.Character = row.Character;
                        itemRow.Name = row.Name;
                        itemRow.Quantity = row.Quantity;
                        itemRow.Location = row.Location;
                        WMS_dataset_temp.Items.Rows.Add(itemRow);
                    }
                    WMS_dataset_temp.Items.AcceptChanges();

                    LoggingFunctions.Debug("Writing the XML file for " + myName + " now.", LoggingFunctions.DBG_SCOPE.WMS);
                    LoggingFunctions.Debug("The char  table has " + WMS_dataset_temp.CharacterInfo.Rows.Count + " rows.", LoggingFunctions.DBG_SCOPE.WMS);
                    LoggingFunctions.Debug("The items table has " + WMS_dataset_temp.Items.Rows.Count + " rows.", LoggingFunctions.DBG_SCOPE.WMS);
                    WMS_dataset_temp.WriteXml(WMS_dataDirectory + "\\" + WMS_dataFile + "_" + myName + ".xml");
                    
                    LoggingFunctions.Debug("Done writing the XML file for " + myName + " now.", LoggingFunctions.DBG_SCOPE.WMS);
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("While writing WMS data file: " + e.ToString());
                }
            }
        }
        private bool WMS_checkWindowNameValid()
        {
            if (!ChangeMonitor.LoggedIn)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void WMS_loadCharacterCB()
        {
            try
            {
                if (WMS_CharacterCB.InvokeRequired)
                {
                    WMS_CharacterCB.Invoke(WMS_loadCharacterCBPtr);
                }
                else
                {
                    WMS_loadCharacterCBCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Loading WMS character CB: " + e.ToString());
            }
        }
        private void WMS_loadCharacterCBCallBackFunction()
        {
            WMS_CharacterCB.BeginUpdate();
            WMS_CharacterCB.Items.Clear();
            LoggingFunctions.Debug("TopWMS::WMS_loadCharacterCBCallBackFunction: Doing wms loadCharCB.", LoggingFunctions.DBG_SCOPE.TOP);
            if (WMS_dataset != null)
            {
                bool addCurrChar = WMS_checkWindowNameValid();
                bool currentCharAdded = false;
                WMSDataSet.CharacterInfoRow[] charInfoRows = (WMSDataSet.CharacterInfoRow[])WMS_dataset.CharacterInfo.Select();
                foreach (WMSDataSet.CharacterInfoRow row in charInfoRows)
                {
                    if (addCurrChar && row.Name == PlayerCache.Vitals.Name)
                    {
                        currentCharAdded = true;
                    }
                    WMS_CharacterCB.Items.Add(row.Name);
                }
                if (addCurrChar && !currentCharAdded)
                {
                    WMS_CharacterCB.Items.Add(PlayerCache.Vitals.Name);
                }
            }
            WMS_CharacterCB.EndUpdate();
        }
        private bool WMS_checkBadInventory()
        {
            //first time thru, last char name is ""
            //else, if it's valid and the new one is valid but different,
            //then we'll reload cause it's a different character.
            //if it's valid and the same, we'll go thru our checks below.
            String newName = "";
            bool wndNameValid = WMS_checkWindowNameValid();
            if (!wndNameValid)
            {
                return true;
            }
            else if (WMS_lastCharOnRebuild == "")
            {
                //First time through, need to rebuild, return false
                WMS_lastCharOnRebuild = MemReads.Self.get_name(true);
                return false;
            }
            else
            {
                newName = MemReads.Self.get_name(true);
                if (WMS_lastCharOnRebuild != newName)
                {
                    WMS_lastCharOnRebuild = newName;
                    //Logged into new character, need to rebuild, return false
                    return false;
                }
                else
                {
                    //Logged into same character, do inventory checks to make sure we're not zoning.
                    byte bagCap = Containers.Bag.LiveCapacity;
                    if ((bagCap < 30) || (bagCap > Containers.AbsMaxBagCount))
                    {
                        return true;
                    }
                    byte satchelCap = Containers.Satchel.LiveCapacity;
                    if (satchelCap > Containers.AbsMaxSatchelCount)
                    {
                        return true;
                    }
                    byte sackCap = Containers.Sack.LiveCapacity;
                    if (sackCap > Containers.AbsMaxSackCount)
                    {
                        return true;
                    }
                    byte caseCap = Containers.MCase.LiveCapacity;
                    if (caseCap > Containers.AbsMaxCaseCount)
                    {
                        return true;
                    }
                    byte safeCap = Containers.Safe.LiveCapacity;
                    if ((safeCap < 30) || (safeCap > Containers.AbsMaxSafeCount))
                    {
                        return true;
                    }
                    byte safe2Cap = Containers.Safe2.LiveCapacity;
                    if ((safe2Cap < 30) || (safe2Cap > Containers.AbsMaxSafeCount))
                    {
                        return true;
                    }
                    byte storageCap = Containers.Storage.LiveCapacity;
                    if (storageCap > Containers.AbsMaxStorageCount)
                    {
                        return true;
                    }
                    byte lockerCap = Containers.Locker.LiveCapacity;
                    if (lockerCap > Containers.AbsMaxLockerCount)
                    {
                        return true;
                    }
                    byte wardrobeCap = Containers.Wardrobe.LiveCapacity;
                    if (wardrobeCap > Containers.AbsMaxWardrobeCount)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void WMSSettingsForm_Disposed(object sender, EventArgs e)
        {
            WMS_loadCharacterCB();
        }
        #endregion Utility Functions
        #region Background Scanning
        private void WMS_BackgroundScanThreadFunction()
        {
            uint loopCnt = 0;
            while (m_TOP_Thread_wms.__CheckState())
            {
                if (!ChangeMonitor.LoggedIn)
                {
                    IocaineFunctions.delay(1000);
                    continue;
                }
                try
                {
                    if (loopCnt++ >= Statics.Settings.WMS.BackgroundScanPeriod)
                    {
                        loopCnt = 0;
                        WMS_rebuildLists();
                        string selectedTab = TOP_getMainTabSelectionName();
                        if ((selectedTab != TAB_KEYS_MAIN.WMS_Tab.ToString()) || !WMS_initialLBUpdateDone)
                        {
                            WMS_UpdateListboxes();
                            WMS_initialLBUpdateDone = true;
                        }
                        if (Statics.Settings.WMS.SaveThisCharOffline)
                        {
                            WMS_UpdateDataset();
                        }
                    }
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("in WMS_BackgroundScanThreadFunction: " + e.ToString());
                }
                finally
                {
                    IocaineFunctions.delay(1000);
                }
            }
        }
        #endregion Background Scanning
    }
}