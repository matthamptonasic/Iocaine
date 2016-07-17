﻿using System;
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

using Iocaine2.Inventory;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Properties;
using Iocaine2.Settings;

namespace Iocaine2
{
    //This file is for functions directly related to the WMS Tab
    partial class Iocaine_2_Form
    {
        #region Member Variables
        private bool WMS_objectsCreated = false;
        private ItemContainer WMS_bag;
        private ItemContainer WMS_satchel;
        private ItemContainer WMS_sack;
        private ItemContainer WMS_case;
        private ItemContainer WMS_safe;
        private ItemContainer WMS_storage;
        private ItemContainer WMS_locker;
        private ItemContainer WMS_wardrobe;
        private List<ItemContainer> WMS_allContainers;
        private WMSDataSet WMS_dataset;
        private WMSDataSet WMS_dataset_temp;
        private bool WMS_poolInventory = true;
        private bool WMS_separateInventory = false;
        private bool WMS_allCharacters = false;
        private String WMS_dataDirectory = ".\\WMS_Data";
        private String WMS_dataFile = "WMS_Dataset";
        private String WMS_selectedChar = "";
        private String WMS_lastCharOnRebuild = "";
        private List<String> WMS_inventoryTypes = new List<string>(new string[] { "Bag", "Satchel", "Sack", "Case", "Safe", "Storage", "Locker", "Wardrobe" });
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
                WMS_updateSingleLBPtr = new WMS_updateSingleLBDelegate(WMS_updateSingleLBCallBackFunction);
            }
            if (WMS_updatePooledLBPtr == null)
            {
                WMS_updatePooledLBPtr = new WMS_updatePooledLBDelegate(WMS_updatePooledLBCallBackFunction);
            }
            if (WMS_updateLabelTextPtr == null)
            {
                WMS_updateLabelTextPtr = new WMS_updateLabelTextDelegate(WMS_updateLabelTextCallBackFunction);
            }
            if (WMS_updateControlVisibilityPtr == null)
            {
                WMS_updateControlVisibilityPtr = new WMS_updateControlVisibilityDelegate(WMS_updateControlVisibilityCallBackFunction);
            }
            if (WMS_sendControlToPtr == null)
            {
                WMS_sendControlToPtr = new WMS_sendControlToDelegate(WMS_sendControlToCallBackFunction);
            }
            if (WMS_loadCharacterCBPtr == null)
            {
                WMS_loadCharacterCBPtr = new WMS_loadCharacterCBDelegate(WMS_loadCharacterCBCallBackFunction);
            }
            if (WMS_setFirstInventoryLocationPtr == null)
            {
                WMS_setFirstInventoryLocationPtr = new WMS_setFirstInventoryLocationDelegate(WMS_setFirstInventoryLocationCallBackFunction);
            }
        }
        #endregion Delegate instantiations
        #region Call Back Functions
        private void WMS_updateSingleLBCallBackFunction(ItemContainer container, WMSDataSet.ItemsRow[] itemRows)
        {
            try
            {
                Monitor.Enter(container);
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
                Monitor.Exit(container);
            }
        }
        private void WMS_updatePooledLBCallBackFunction(WMSDataSet.ItemsRow[] itemRows)
        {
            try
            {
                foreach (ItemContainer container in WMS_allContainers)
                {
                    Monitor.Enter(container);
                }
                ListBox lb = WMS_PooledLB;
                if (itemRows == null)
                {
                    List<Item> itemList;
                    List<ushort> itemQuan;
                    lb.BeginUpdate();
                    lb.Items.Clear();
                    lb.Sorted = true;
                    lb.ColumnWidth = WMS_pooledColumnWidthLow;
                    foreach (ItemContainer container in WMS_allContainers)
                    {
                        itemList = container.GetSummaryItemList();
                        itemQuan = container.GetSummaryItemQuanList();
                        int cnt = itemList.Count;
                        for (int ii = 0; ii < cnt; ii++)
                        {
                            lb.Items.Add(itemList[ii].Name + " (" + itemQuan[ii] + ") [" + container.TypeString + "]");
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
                foreach (ItemContainer container in WMS_allContainers)
                {
                    Monitor.Exit(container);
                }
            }
        }
        private void WMS_updateLabelTextCallBackFunction(Label iLabel, String iText)
        {
            iLabel.Text = iText;
        }
        private void WMS_updateControlVisibilityCallBackFunction(Control iControl, bool iVisible)
        {
            iControl.Visible = iVisible;
        }
        private void WMS_sendControlToCallBackFunction(Control iControl, bool iToFront)
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
        private void WMS_setFirstInventoryLocationCallBackFunction(Int32 iPosX)
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
                    WMS_updateSingleLBCallBackFunction(container, itemRows);
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
                    WMS_updatePooledLBCallBackFunction(itemRows);
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
                    WMS_updateLabelTextCallBackFunction(iLabel, iText);
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
                    WMS_updateControlVisibilityCallBackFunction(iControl, iVisibile);
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
                    WMS_sendControlToCallBackFunction(iControl, iToFront);
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
                    WMS_setFirstInventoryLocationCallBackFunction(iPosX);
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
        private bool doWMSInits()
        {
            if (ChangeMonitor.MainProc != null && ChangeMonitor.MainModule != null)
            {
                WMS_LoadSettings();
                if (WMS_allContainers != null)
                {
                    WMS_allContainers.Clear();
                    WMS_allContainers = null;
                }
                WMS_bag = Inventory.Containers.Bag;
                WMS_satchel = Inventory.Containers.Satchel;
                WMS_sack = Inventory.Containers.Sack;
                WMS_case = Inventory.Containers.MCase;
                WMS_safe = Inventory.Containers.Safe;
                WMS_storage = Inventory.Containers.Storage;
                WMS_locker = Inventory.Containers.Locker;
                WMS_wardrobe = Inventory.Containers.Wardrobe;
                WMS_allContainers = new List<ItemContainer>();
                WMS_allContainers.Add(WMS_bag);
                WMS_allContainers.Add(WMS_satchel);
                WMS_allContainers.Add(WMS_sack);
                WMS_allContainers.Add(WMS_case);
                WMS_allContainers.Add(WMS_safe);
                WMS_allContainers.Add(WMS_storage);
                WMS_allContainers.Add(WMS_locker);
                WMS_allContainers.Add(WMS_wardrobe);
                WMS_objectsCreated = true;
            }
            else
            {
                return false;
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
            return true;
        }
        private void WMS_LoadSettings()
        {
            Statics.Settings.WMS.BackgroundUpdate = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.WMS, "WMSBackgroundUpdate"));
            Statics.Settings.WMS.BackgroundScanPeriod = Convert.ToUInt32(UserSettings.GetValue(UserSettings.BOT.WMS, "WMSBackgroundScanPeriod"));
        }
        private void doWMSGuiInits()
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
                WMS_updateControlVisibility(WMS_StorageOccLabel, false);
                WMS_updateControlVisibility(WMS_LockerOccLabel, false);
                WMS_updateControlVisibility(WMS_WardrobeOccLabel, false);
            }
            else
            {
                WMS_updateLabelText(WMS_BagLabel, "Gobbie Bag");
            }
            return;
        }
        private void doWMSDatasetInits()
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
                        charRow.StorageOcc = row.StorageOcc;
                        charRow.StorageCap = row.StorageCap;
                        charRow.LockerOcc = row.LockerOcc;
                        charRow.LockerCap = row.LockerCap;
                        charRow.WardrobeOcc = row.WardrobeOcc;
                        charRow.WardrobeCap = row.WardrobeCap;
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
            String currChar = MemReads.Self.get_name(true);
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
            foreach (ItemContainer container in WMS_allContainers)
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
                foreach (ItemContainer container in WMS_allContainers)
                {
                    Monitor.Enter(container);
                }
                String currChar = MemReads.Self.get_name(true);
                WMSDataSet.CharacterInfoRow[] localCharRows = (WMSDataSet.CharacterInfoRow[])WMS_dataset.CharacterInfo.Select("Name='" + currChar + "'");
                if (localCharRows.Length == 0)
                {
                    WMSDataSet.CharacterInfoRow rowToAdd = WMS_dataset.CharacterInfo.NewCharacterInfoRow();
                    rowToAdd.Name = currChar;
                    rowToAdd.BagOcc = WMS_bag.Occupancy;
                    rowToAdd.BagCap = WMS_bag.Capacity;
                    rowToAdd.SatchelOcc = WMS_satchel.Occupancy;
                    rowToAdd.SatchelCap = WMS_satchel.Capacity;
                    rowToAdd.SackOcc = WMS_sack.Occupancy;
                    rowToAdd.SackCap = WMS_sack.Capacity;
                    rowToAdd.CaseOcc = WMS_case.Occupancy;
                    rowToAdd.CaseCap = WMS_case.Capacity;
                    rowToAdd.SafeOcc = WMS_safe.Occupancy;
                    rowToAdd.SafeCap = WMS_safe.Capacity;
                    rowToAdd.StorageOcc = WMS_storage.Occupancy;
                    rowToAdd.StorageCap = WMS_storage.Capacity;
                    rowToAdd.LockerOcc = WMS_locker.Occupancy;
                    rowToAdd.LockerCap = WMS_locker.Capacity;
                    rowToAdd.WardrobeOcc = WMS_wardrobe.Occupancy;
                    rowToAdd.WardrobeCap = WMS_wardrobe.Capacity;
                    rowToAdd.DateSaved = DateTime.Now;
                    WMS_dataset.CharacterInfo.Rows.Add(rowToAdd);
                    WMS_dataset.CharacterInfo.AcceptChanges();
                }
                else
                {
                    WMSDataSet.CharacterInfoRow rowToUpdate = localCharRows[0];
                    rowToUpdate.BagOcc = WMS_bag.Occupancy;
                    rowToUpdate.BagCap = WMS_bag.Capacity;
                    rowToUpdate.SatchelOcc = WMS_satchel.Occupancy;
                    rowToUpdate.SatchelCap = WMS_satchel.Capacity;
                    rowToUpdate.SackOcc = WMS_sack.Occupancy;
                    rowToUpdate.SackCap = WMS_sack.Capacity;
                    rowToUpdate.CaseOcc = WMS_case.Occupancy;
                    rowToUpdate.CaseCap = WMS_case.Capacity;
                    rowToUpdate.SafeOcc = WMS_safe.Occupancy;
                    rowToUpdate.SafeCap = WMS_safe.Capacity;
                    rowToUpdate.StorageOcc = WMS_storage.Occupancy;
                    rowToUpdate.StorageCap = WMS_storage.Capacity;
                    rowToUpdate.LockerOcc = WMS_locker.Occupancy;
                    rowToUpdate.LockerCap = WMS_locker.Capacity;
                    rowToUpdate.WardrobeOcc = WMS_wardrobe.Occupancy;
                    rowToUpdate.WardrobeCap = WMS_wardrobe.Capacity;
                    rowToUpdate.DateSaved = DateTime.Now;
                    WMS_dataset.CharacterInfo.AcceptChanges();
                }
                WMSDataSet.ItemsRow[] localItemRows = (WMSDataSet.ItemsRow[])WMS_dataset.Items.Select("Character='" + currChar + "'");
                foreach (WMSDataSet.ItemsRow rowToRemove in localItemRows)
                {
                    WMS_dataset.Items.RemoveItemsRow(rowToRemove);
                }
                foreach (ItemContainer container in WMS_allContainers)
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
                foreach (ItemContainer container in WMS_allContainers)
                {
                    Monitor.Exit(container);
                }
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
                WMS_updateControlVisibility(WMS_StorageLB, false);
                WMS_updateControlVisibility(WMS_StorageLabel, false);
                WMS_updateControlVisibility(WMS_LockerLB, false);
                WMS_updateControlVisibility(WMS_LockerLabel, false);
                WMS_updateControlVisibility(WMS_WardrobeLB, false);
                WMS_updateControlVisibility(WMS_WardrobeLabel, false);
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
                WMS_updateControlVisibility(WMS_StorageLB, true);
                WMS_updateControlVisibility(WMS_StorageLabel, true);
                WMS_updateControlVisibility(WMS_LockerLB, true);
                WMS_updateControlVisibility(WMS_LockerLabel, true);
                WMS_updateControlVisibility(WMS_WardrobeLB, true);
                WMS_updateControlVisibility(WMS_WardrobeLabel, true);
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
                WMS_updateControlVisibility(WMS_StorageLB, false);
                WMS_updateControlVisibility(WMS_StorageLabel, false);
                WMS_updateControlVisibility(WMS_LockerLB, false);
                WMS_updateControlVisibility(WMS_LockerLabel, false);
                WMS_updateControlVisibility(WMS_WardrobeLB, false);
                WMS_updateControlVisibility(WMS_WardrobeLabel, false);
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
                    case ItemContainer.STORAGE_TYPE.STORAGE:
                        lb = WMS_StorageLB;
                        break;
                    case ItemContainer.STORAGE_TYPE.LOCKER:
                        lb = WMS_LockerLB;
                        break;
                    case ItemContainer.STORAGE_TYPE.WARDROBE:
                        lb = WMS_WardrobeLB;
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
                    case "Storage":
                        lb = WMS_StorageLB;
                        break;
                    case "Locker":
                        lb = WMS_LockerLB;
                        break;
                    case "Wardrobe":
                        lb = WMS_WardrobeLB;
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
            foreach (ItemContainer container in WMS_allContainers)
            {
                Monitor.Enter(container);
                container.RebuildLists();
                Monitor.Exit(container);
            }
        }
        private void WMS_setOccLabels(WMSDataSet.CharacterInfoRow charRow)
        {
            if (WMS_poolInventory)
            {
                WMS_updateControlVisibility(WMS_SatchelOccLabel, false);
                WMS_updateControlVisibility(WMS_SackOccLabel, false);
                WMS_updateControlVisibility(WMS_CaseOccLabel, false);
                WMS_updateControlVisibility(WMS_SafeOccLabel, false);
                WMS_updateControlVisibility(WMS_StorageOccLabel, false);
                WMS_updateControlVisibility(WMS_LockerOccLabel, false);
                WMS_updateControlVisibility(WMS_WardrobeLabel, false);
                if (charRow != null)
                {
                    int totalOcc = charRow.BagOcc + charRow.SatchelOcc + charRow.SackOcc + charRow.CaseOcc + charRow.SafeOcc + charRow.StorageOcc + charRow.LockerOcc + charRow.WardrobeOcc;
                    int totalSpace = charRow.BagCap + charRow.SatchelCap + charRow.SackCap + charRow.CaseCap + charRow.SafeCap + charRow.StorageCap + charRow.LockerCap + charRow.WardrobeCap;
                    WMS_updateLabelText(WMS_BagOccLabel, totalOcc.ToString() + " / " + totalSpace.ToString());
                }
                else if (ChangeMonitor.MainProc == null || ChangeMonitor.MainModule == null)
                {
                    WMS_updateLabelText(WMS_BagOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_SatchelOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_SackOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_CaseOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_SafeOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_StorageOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_LockerOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_WardrobeOccLabel, "0 / 0");
                }
                else
                {
                    int totalOcc = 0;
                    int totalSpace = 0;
                    foreach (ItemContainer container in WMS_allContainers)
                    {
                        Monitor.Enter(container);
                        totalOcc += container.Occupancy;
                        totalSpace += container.Capacity;
                        Monitor.Exit(container);
                    }
                    WMS_updateLabelText(WMS_BagOccLabel, totalOcc.ToString() + " / " + totalSpace.ToString());
                }
            }
            else if(WMS_separateInventory)
            {
                WMS_updateControlVisibility(WMS_SatchelOccLabel, true);
                WMS_updateControlVisibility(WMS_SackOccLabel, true);
                WMS_updateControlVisibility(WMS_CaseOccLabel, true);
                WMS_updateControlVisibility(WMS_SafeOccLabel, true);
                WMS_updateControlVisibility(WMS_StorageOccLabel, true);
                WMS_updateControlVisibility(WMS_LockerOccLabel, true);
                WMS_updateControlVisibility(WMS_WardrobeOccLabel, true);
                if (charRow != null)
                {
                    WMS_updateLabelText(WMS_BagOccLabel, charRow.BagOcc + " / " + charRow.BagCap);
                    WMS_updateLabelText(WMS_SatchelOccLabel, charRow.SatchelOcc + " / " + charRow.SatchelCap);
                    WMS_updateLabelText(WMS_SackOccLabel, charRow.SackOcc + " / " + charRow.SackCap);
                    WMS_updateLabelText(WMS_CaseOccLabel, charRow.CaseOcc + " / " + charRow.CaseCap);
                    WMS_updateLabelText(WMS_SafeOccLabel, charRow.SafeOcc + " / " + charRow.SafeCap);
                    WMS_updateLabelText(WMS_StorageOccLabel, charRow.StorageOcc + " / " + charRow.StorageCap);
                    WMS_updateLabelText(WMS_LockerOccLabel, charRow.LockerOcc + " / " + charRow.LockerCap);
                    WMS_updateLabelText(WMS_WardrobeOccLabel, charRow.WardrobeOcc + " / " + charRow.WardrobeCap);
                }
                else if (ChangeMonitor.MainProc == null || ChangeMonitor.MainModule == null)
                {
                    WMS_updateLabelText(WMS_BagOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_SatchelOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_SackOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_CaseOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_SafeOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_StorageOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_LockerOccLabel, "0 / 0");
                    WMS_updateLabelText(WMS_WardrobeOccLabel, "0 / 0");
                }
                else
                {
                    WMS_updateLabelText(WMS_BagOccLabel, WMS_bag.Occupancy + " / " + WMS_bag.Capacity);
                    WMS_updateLabelText(WMS_SatchelOccLabel, WMS_satchel.Occupancy + " / " + WMS_satchel.Capacity);
                    WMS_updateLabelText(WMS_SackOccLabel, WMS_sack.Occupancy + " / " + WMS_sack.Capacity);
                    WMS_updateLabelText(WMS_CaseOccLabel, WMS_case.Occupancy + " / " + WMS_case.Capacity);
                    WMS_updateLabelText(WMS_SafeOccLabel, WMS_safe.Occupancy + " / " + WMS_safe.Capacity);
                    WMS_updateLabelText(WMS_StorageOccLabel, WMS_storage.Occupancy + " / " + WMS_storage.Capacity);
                    WMS_updateLabelText(WMS_LockerOccLabel, WMS_locker.Occupancy + " / " + WMS_locker.Capacity);
                    WMS_updateLabelText(WMS_WardrobeOccLabel, WMS_wardrobe.Occupancy + " / " + WMS_wardrobe.Capacity);
                }
            }
            else if (WMS_allCharacters)
            {
                WMS_updateControlVisibility(WMS_SatchelOccLabel, false);
                WMS_updateControlVisibility(WMS_SackOccLabel, false);
                WMS_updateControlVisibility(WMS_CaseOccLabel, false);
                WMS_updateControlVisibility(WMS_SafeOccLabel, false);
                WMS_updateControlVisibility(WMS_StorageOccLabel, false);
                WMS_updateControlVisibility(WMS_LockerOccLabel, false);
                WMS_updateControlVisibility(WMS_WardrobeOccLabel, false);
                if (WMS_dataset != null)
                {
                    WMSDataSet.CharacterInfoRow[] charRows = (WMSDataSet.CharacterInfoRow[])WMS_dataset.CharacterInfo.Select();
                    int totalOcc = 0;
                    int totalSpace = 0;
                    foreach (WMSDataSet.CharacterInfoRow row in charRows)
                    {
                        totalOcc += row.BagOcc + row.SatchelOcc + row.SackOcc + row.CaseOcc + row.SafeOcc + row.StorageOcc + row.LockerOcc + row.WardrobeOcc;
                        totalSpace += row.BagCap + row.SatchelCap + row.SackCap + row.CaseCap + row.SafeCap + row.StorageCap + row.LockerCap + row.WardrobeCap;
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
                        charRow.StorageOcc = row.StorageOcc;
                        charRow.StorageCap = row.StorageCap;
                        charRow.LockerOcc = row.LockerOcc;
                        charRow.LockerCap = row.LockerCap;
                        charRow.WardrobeOcc = row.WardrobeOcc;
                        charRow.WardrobeCap = row.WardrobeCap;
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
                    byte bagCap = WMS_bag.LiveCapacity;
                    if ((bagCap < 30) || (bagCap > Containers.AbsMaxBagCount))
                    {
                        return true;
                    }
                    byte satchelCap = WMS_satchel.LiveCapacity;
                    if (satchelCap > Containers.AbsMaxSatchelCount)
                    {
                        return true;
                    }
                    byte sackCap = WMS_sack.LiveCapacity;
                    if (sackCap > Containers.AbsMaxSackCount)
                    {
                        return true;
                    }
                    byte caseCap = WMS_case.LiveCapacity;
                    if (caseCap > Containers.AbsMaxCaseCount)
                    {
                        return true;
                    }
                    byte safeCap = WMS_safe.LiveCapacity;
                    if ((safeCap < 30) || (safeCap > Containers.AbsMaxSafeCount))
                    {
                        return true;
                    }
                    byte storageCap = WMS_storage.LiveCapacity;
                    if (storageCap > Containers.AbsMaxStorageCount)
                    {
                        return true;
                    }
                    byte lockerCap = WMS_locker.LiveCapacity;
                    if (lockerCap > Containers.AbsMaxLockerCount)
                    {
                        return true;
                    }
                    byte wardrobeCap = WMS_wardrobe.LiveCapacity;
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
                        if ((TOP_getMainTabSelectionName() != TAB_KEYS_MAIN.WMS_Tab.ToString()) || !WMS_initialLBUpdateDone)
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