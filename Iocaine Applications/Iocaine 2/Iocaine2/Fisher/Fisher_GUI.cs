using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;

using Iocaine2.Data.Client;
using Iocaine2.Inventory;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Settings;
using Iocaine2.Tools;

#pragma warning disable 618

namespace Iocaine2.Bots
{
    public sealed partial class Fisher : Bot
    {
        #region Private Members
        #region Control Declarations
        private Button c_releaseButton;
        private TextBox c_timeDateTB;
        private TextBox c_dayTB;
        private TextBox c_moonTB;
        private TextBox c_weatherTB;
        private Label c_zoneLabel;
        private Label c_lastCatchLabel;
        private Label c_fishNameLabel;
        private Label c_fishCurHPLabel;
        private Label c_fishMaxHPLabel;
        private Label c_fatigueLabel;
        private Label c_id1Label;
        private Label c_id2Label;
        private Label c_id3Label;
        private Label c_largeLabel;
        private ProgressBar c_hpProgressBar;
        private CheckedListBox c_dropLB;
        private TextBox c_dropItemTB;
        private Button c_dropAddButton;
        private Button c_dropRemoveButton;
        private CheckedListBox c_baitLB;
        private Button c_baitUpButton;
        private Button c_baitDownButton;
        private Button c_baitRefreshButton;
        private CheckedListBox c_fishLB;
        private ListView c_statsLB;
        private Label c_rodNameLabel;
        private Label c_baitNameLabel;
        private Label c_baitQuanLabel;
        private PictureBox c_leftArrowPB;
        private PictureBox c_rightArrowPB;
        private CheckBox c_autoReset;
        private CheckBox c_thisZoneOnly;
        #endregion Control Declarations
        #region Flags
        private bool m_baitLB_InitDone = false;
        #endregion Flags
        #endregion Private Members

        #region Inits
        private void Init_Controls()
        {
            c_startButton.Click += c_startButton_Click;
            c_stopButton.Click += c_stopButton_Click;
            c_releaseButton.Click += c_releaseButton_Click;
            c_fishLB.ItemCheck += c_fishLB_ItemCheck;
            c_dropLB.ItemCheck += c_dropLB_ItemCheck;
            c_dropAddButton.Click += c_dropAddButton_Click;
            c_dropRemoveButton.Click += c_dropRemoveButton_Click;
            c_baitLB.ItemCheck += c_baitLB_ItemCheck;
            c_baitUpButton.Click += c_baitUpButton_Click;
            c_baitDownButton.Click += c_baitDownButton_Click;
            c_baitRefreshButton.Click += c_baitRefreshButton_Click;
            c_autoReset.CheckedChanged += c_autoResetChkB_CheckedChanged;
            c_thisZoneOnly.CheckedChanged += c_thisZoneOnlyChkB_CheckedChanged;
        }
        #endregion Inits

        #region Control Updates and Call-Backs
        #region Date & Weather Textboxes
        private void c_timeDateTB_Update(String text)
        {
            if (!c_timeDateTB.IsHandleCreated)
            {
                return;
            }
            try
            {
                if (c_timeDateTB.InvokeRequired)
                {
                    c_timeDateTB.Invoke(new Statics.FuncPtrs.TD_Void_String(c_timeDateTB_UpdateCBF), new object[] { text });
                }
                else
                {
                    c_timeDateTB_UpdateCBF(text);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_timeDateTB_Update: " + e.ToString());
            }
            return;
        }
        private void c_timeDateTB_UpdateCBF(String text)
        {
            c_timeDateTB.Text = text;
        }
        private void c_dayTB_Update(String text)
        {
            try
            {
                if (c_dayTB.InvokeRequired)
                {
                    c_dayTB.Invoke(new Statics.FuncPtrs.TD_Void_String(c_dayTB_UpdateCBF), new object[] { text });
                }
                else
                {
                    c_dayTB_UpdateCBF(text);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_dayTB_Update: " + e.ToString());
            }
        }
        private void c_dayTB_UpdateCBF(String text)
        {
            c_dayTB.Text = text;
        }
        private void c_moonTB_Update(String text)
        {
            try
            {
                if (c_moonTB.InvokeRequired)
                {
                    c_moonTB.Invoke(new Statics.FuncPtrs.TD_Void_String(c_moonTB_UpdateCBF), new object[] { text });
                }
                else
                {
                    c_moonTB_UpdateCBF(text);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_moonTB_Update: " + e.ToString());
            }
        }
        private void c_moonTB_UpdateCBF(String text)
        {
            c_moonTB.Text = text;
        }
        private void c_weatherTB_Update(String text)
        {
            try
            {
                c_weatherTB.Invoke(new Statics.FuncPtrs.TD_Void_String(c_weatherTB_UpdateCBF), new object[] { text });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_weatherTB_Update: " + e.ToString());
            }
        }
        private void c_weatherTB_UpdateCBF(String text)
        {
            c_weatherTB.Text = text;
        }
        #endregion Date & Weather Textboxes
        #region Info Box Labels
        private void c_zoneLabel_Update(String iText)
        {
            try
            {
                if (c_zoneLabel == null)
                {
                    return;
                }
                c_zoneLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(c_zoneLabel_UpdateCBF), new object[] { iText });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_zoneLabel_Update: " + e.ToString());
            }
        }
        private void c_zoneLabel_UpdateCBF(String iText)
        {
            try
            {
                c_zoneLabel.Text = iText;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_zoneLabel_UpdateCBF: " + e.ToString());
            }
        }
        private void c_rodNameLabel_Update(String iText)
        {
            try
            {
                if (c_rodNameLabel == null)
                {
                    return;
                }
                c_rodNameLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(c_rodNameLabel_UpdateCBF), new object[] { iText });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_rodNameLabel_Update: " + e.ToString());
            }
        }
        private void c_rodNameLabel_UpdateCBF(String iText)
        {
            try
            {
                c_rodNameLabel.Text = iText;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_rodNameLabel_UpdateCBF: " + e.ToString());
            }
        }
        private void c_baitNameLabel_Update(String iText)
        {
            try
            {
                if (c_baitNameLabel == null)
                {
                    return;
                }
                c_baitNameLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(c_baitNameLabel_UpdateCBF), new object[] { iText });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_baitNameLabel_Update: " + e.ToString());
            }
        }
        private void c_baitNameLabel_UpdateCBF(String iText)
        {
            try
            {
                c_baitNameLabel.Text = iText;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_baitNameLabel_UpdateCBF: " + e.ToString());
            }
        }
        private void c_baitQuanLabel_Update(String iText)
        {
            try
            {
                if (c_baitQuanLabel == null)
                {
                    return;
                }
                c_baitQuanLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(c_baitQuanLabel_UpdateCBF), new object[] { iText });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_baitQuanLabel_Update: " + e.ToString());
            }
        }
        private void c_baitQuanLabel_UpdateCBF(String iText)
        {
            try
            {
                c_baitQuanLabel.Text = iText;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_baitQuanLabel_UpdateCBF: " + e.ToString());
            }
        }
        private void c_lastCatchLabel_Update(String iText)
        {
            try
            {
                if (c_lastCatchLabel == null)
                {
                    return;
                }
                c_lastCatchLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(c_lastCatchLabel_UpdateCBF), new object[] { iText });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_lastCatchLabel_Update: " + e.ToString());
            }
        }
        private void c_lastCatchLabel_UpdateCBF(String iText)
        {
            try
            {
                c_lastCatchLabel.Text = iText;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_lastCatchLabel_UpdateCBF: " + e.ToString());
            }
        }
        private void c_fishNameLabel_Update(String iText)
        {
            try
            {
                if (c_fishNameLabel == null)
                {
                    return;
                }
                c_fishNameLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(c_fishNameLabel_UpdateCBF), new object[] { iText });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_fishNameLabel_Update: " + e.ToString());
            }
        }
        private void c_fishNameLabel_UpdateCBF(String iText)
        {
            try
            {
                c_fishNameLabel.Text = iText;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_fishNameLabel_UpdateCBF: " + e.ToString());
            }
        }
        private void c_fishCurHPLabel_Update(String iText)
        {
            try
            {
                if (c_fishCurHPLabel == null)
                {
                    return;
                }
                c_fishCurHPLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(c_fishCurHPLabel_UpdateCBF), new object[] { iText });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_fishCurHPLabel_Update: " + e.ToString());
            }
        }
        private void c_fishCurHPLabel_UpdateCBF(String iText)
        {
            try
            {
                c_fishCurHPLabel.Text = iText;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_fishCurHPLabel_UpdateCBF: " + e.ToString());
            }
        }
        private void c_fishMaxHPLabel_Update(String iText)
        {
            try
            {
                if (c_fishMaxHPLabel == null)
                {
                    return;
                }
                c_fishMaxHPLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(c_fishMaxHPLabel_UpdateCBF), new object[] { iText });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_fishMaxHPLabel_Update: " + e.ToString());
            }
        }
        private void c_fishMaxHPLabel_UpdateCBF(String iText)
        {
            try
            {
                c_fishMaxHPLabel.Text = iText;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_fishMaxHPLabel_Update: " + e.ToString());
            }
        }
        private void c_fatigueLabel_Update(String iText)
        {
            try
            {
                if (c_fatigueLabel == null)
                {
                    return;
                }
                c_fatigueLabel.Invoke(new Statics.FuncPtrs.TD_Void_String(c_fatigueLabel_UpdateCBF), new object[] { iText });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_fatigueLabel_Update: " + e.ToString());
            }
        }
        private void c_fatigueLabel_UpdateCBF(String iText)
        {
            try
            {
                c_fatigueLabel.Text = iText;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_fatigueLabel_UpdateCBF: " + e.ToString());
            }
        }
        private UInt16 c_fatigueLabel_GetValue()
        {
            try
            {
                if (c_fatigueLabel == null)
                {
                    return 0;
                }
                return (UInt16)c_fatigueLabel.Invoke(new Statics.FuncPtrs.TD_UInt16_Void(c_fatigueLabel_GetValueCBF));
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_fatigueLabel_GetValue: " + e.ToString());
                return 0;
            }
        }
        private UInt16 c_fatigueLabel_GetValueCBF()
        {
            try
            {
                return Convert.ToUInt16(c_fatigueLabel.Text);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_fatigueLabel_GetValueCBF: " + e.ToString());
                return 0;
            }
        }
        private void c_id1Label_Update(String iText)
        {
            try
            {
                if (c_id1Label == null)
                {
                    return;
                }
                c_id1Label.Invoke(new Statics.FuncPtrs.TD_Void_String(c_id1Label_UpdateCBF), new object[] { iText });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_id1Label_Update: " + e.ToString());
            }
        }
        private void c_id1Label_UpdateCBF(String iText)
        {
            try
            {
                c_id1Label.Text = iText;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_id1Label_UpdateCBF: " + e.ToString());
            }
        }
        private void c_id2Label_Update(String iText)
        {
            try
            {
                if (c_id2Label == null)
                {
                    return;
                }
                c_id2Label.Invoke(new Statics.FuncPtrs.TD_Void_String(c_id2Label_UpdateCBF), new object[] { iText });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_id2Label_Update: " + e.ToString());
            }
        }
        private void c_id2Label_UpdateCBF(String iText)
        {
            try
            {
                c_id2Label.Text = iText;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_id2Label_UpdateCBF: " + e.ToString());
            }
        }
        private void c_id3Label_Update(String iText)
        {
            try
            {
                if (c_id3Label == null)
                {
                    return;
                }
                c_id3Label.Invoke(new Statics.FuncPtrs.TD_Void_String(c_id3Label_UpdateCBF), new object[] { iText });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_id3Label_Update: " + e.ToString());
            }
        }
        private void c_id3Label_UpdateCBF(String iText)
        {
            try
            {
                c_id3Label.Text = iText;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_id3Label_UpdateCBF: " + e.ToString());
            }
        }
        private void c_largeLabel_Update(Int32 iVal)
        {
            try
            {
                if (c_zoneLabel == null)
                {
                    return;
                }
                Statics.FuncPtrs.TD_Void_String fPtr = new Statics.FuncPtrs.TD_Void_String(c_largeLabel_UpdateCBF);
                switch (iVal)
                {
                    case 0:
                        c_largeLabel.Invoke(fPtr, new object[] { "No" });
                        break;
                    case 1:
                        c_largeLabel.Invoke(fPtr, new object[] { "Yes" });
                        break;
                    default:
                        c_largeLabel.Invoke(fPtr, new object[] { "--" });
                        break;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_largeLabel_Update: " + e.ToString());
            }
        }
        private void c_largeLabel_UpdateCBF(String iText)
        {
            try
            {
                c_largeLabel.Text = iText;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_largeLabel_UpdateCBF: " + e.ToString());
            }
        }
        #endregion Info Box Labels
        #region Info Box Progress Bar & Arrows
        private void c_hpProgressBar_Update(Int32 curHP, Int32 maxHP)
        {
            int newPerc = 100 * curHP / maxHP;
            try
            {
                if (c_hpProgressBar == null)
                {
                    return;
                }
                c_hpProgressBar.Invoke(new Statics.FuncPtrs.TD_Void_Int32(c_hpProgressBar_UpdateCBF), new object[] { newPerc });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_hpProgressBar_Update: " + e.ToString());
            }
        }
        private void c_hpProgressBar_UpdateCBF(Int32 iValue)
        {
            try
            {
                c_hpProgressBar.Value = iValue;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_hpProgressBar_UpdateCBF: " + e.ToString());
            }
        }
        private void c_leftArrowImage_Update(System.Drawing.Image iImage)
        {
            try
            {
                if (c_leftArrowPB == null)
                {
                    return;
                }
                c_leftArrowPB.Invoke(new Statics.FuncPtrs.TD_Void_Image(c_leftArrowImage_UpdateCBF), new object[] { iImage });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_leftArrowImage_Update: " + e.ToString());
            }
        }
        private void c_leftArrowImage_UpdateCBF(System.Drawing.Image iImage)
        {
            try
            {
                c_leftArrowPB.Image = iImage;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_leftArrowImage_UpdateCBF: " + e.ToString());
            }
        }
        private void c_rightArrowImage_Update(System.Drawing.Image iImage)
        {
            try
            {
                if (c_rightArrowPB == null)
                {
                    return;
                }
                c_rightArrowPB.Invoke(new Statics.FuncPtrs.TD_Void_Image(c_rightArrowImage_UpdateCBF), new object[] { iImage });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_rightArrowImage_Update: " + e.ToString());
            }
        }
        private void c_rightArrowImage_UpdateCBF(System.Drawing.Image iImage)
        {
            try
            {
                c_rightArrowPB.Image = iImage;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_rightArrowImage_UpdateCBF: " + e.ToString());
            }
        }
        #endregion Info Box Progress Bar & Arrows
        #region Drop Box
        private void c_dropLB_Load()
        {
            try
            {
                if (c_dropLB.InvokeRequired)
                {
                    c_dropLB.Invoke(new Statics.FuncPtrs.TD_Void_Void(c_dropLB_LoadCBF));
                }
                else
                {
                    c_dropLB_LoadCBF();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_dropLB_Load: Updating Drop List Box: " + e.ToString());
            }
        }
        private void c_dropLB_LoadCBF()
        {
            try
            {
                c_dropLB.Items.Clear();
                List<List<Object>> dropSettings = UserSettings.GetList(UserSettings.BOT.FISHER, UserSettings.LIST_TABLE.FISHER_DROPBOX);
                if (dropSettings != null)
                {
                    for (int ii = 0; ii < dropSettings.Count; ii++)
                    {
                        c_dropLB.Items.Add(dropSettings[ii][0].ToString(), Convert.ToBoolean(dropSettings[ii][1]));
                        IocaineFunctions.delay(1);
                        Application.DoEvents();
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_dropLB_LoadCBF: Updating Drop List Box: " + e.ToString());
            }
        }
        private bool c_dropLB_Contains(string item)
        {
            try
            {
                if (c_dropLB == null)
                {
                    return false;
                }
                return (bool)c_dropLB.Invoke(new Statics.FuncPtrs.TD_Bool_String(c_dropLB_ContainsCBF), new object[] { item });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_dropLB_Contains: " + e.ToString());
                return false;
            }
        }
        private bool c_dropLB_ContainsCBF(String iItem)
        {
            try
            {
                return c_dropLB.CheckedItems.Contains(iItem);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_dropLB_ContainsCBF: " + e.ToString());
                return false;
            }
        }
        #endregion Drop Box
        #region Bait Box
        private void c_baitLB_Update(List<BaitBoxItem> iBaitItems, int iSelectIndex)
        {
            try
            {
                if (c_baitLB.InvokeRequired)
                {
                    c_baitLB.Invoke(new Statics.FuncPtrs.TD_Void_BaitBoxItemList_Int32(c_baitLB_UpdateCBF), new object[] { iBaitItems, iSelectIndex });
                }
                else
                {
                    c_baitLB_UpdateCBF(iBaitItems, iSelectIndex);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_baitLB_Update: " + e.ToString());
            }
        }
        private void c_baitLB_UpdateCBF(List<BaitBoxItem> iBaitItems, int iSelectIndex)
        {
            try
            {
                if (iBaitItems == null)
                {
                    return;
                }
                c_baitLB.BeginUpdate();
                c_baitLB.Items.Clear();
                for (int ii = 0; ii < iBaitItems.Count; ii++)
                {
                    c_baitLB.Items.Add(iBaitItems[ii], iBaitItems[ii].Use);
                }
                if ((iSelectIndex >= 0) && (iSelectIndex < c_baitLB.Items.Count))
                {
                    c_baitLB.SelectedIndex = iSelectIndex;
                }
                c_baitLB.EndUpdate();
                m_baitLB_InitDone = true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_baitLB_UpdateCBF: " + e.ToString());
            }
        }
        private void c_baitLB_Refresh()
        {
            c_baitLB_Refresh(true, true);
        }
        private void c_baitLB_Refresh(bool iWaitForInvSettle, bool iRebuildOnce)
        {
            while (!ChangeMonitor.LoggedIn)
            {
                IocaineFunctions.delay(1000);
            }
            //We need to make sure the inventory is stable before we actually load the box.
            //Once the inventory is stable (mobile inv count > 0 && total count is stable for 2 inv updates),
            //we'll create a list of 1: bait names, 2: checked values, 3: priority.
            //Bait names will be based on what's in the inventory.
            //Checked values and priority will be based on the user settings.
            if (!iWaitForInvSettle && iRebuildOnce)
            {
                Inventory.Containers.RebuildListsMobileOnly();
            }
            if (iWaitForInvSettle)
            {
                ushort invCount = 0;
                ushort lastCount = 0;
                bool countMatched = false;
                ushort timeout = 60;
                ushort tempCnt = 0;
                while ((invCount == 0) || !countMatched)
                {
                    LoggingFunctions.Debug("Fisher::c_baitLB_Refresh: tempCnt: " + tempCnt + " timeout = " + timeout + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                    Inventory.Containers.RebuildListsMobileOnly();
                    invCount = Inventory.Containers.Bag.Occupancy;
                    invCount += Inventory.Containers.Satchel.Occupancy;
                    invCount += Inventory.Containers.Sack.Occupancy;
                    LoggingFunctions.Debug("Fisher::c_baitLB_Refresh: invCount = " + invCount + " timeout = " + timeout + ".", LoggingFunctions.DBG_SCOPE.FISHER);
                    countMatched = invCount == lastCount;
                    lastCount = invCount;
                    timeout -= 1;
                    if (timeout == 0)
                    {
                        LoggingFunctions.Error("Fisher::c_baitLB_Refresh: Timed out waiting for inventory to settle.");
                        LoggingFunctions.Error("Fisher::c_baitLB_Refresh: " + invCount + ", last count: " + lastCount + ".");
                        return;
                    }
                    if (!countMatched)
                    {
                        IocaineFunctions.delay(5000);
                    }
                    tempCnt++;
                }
            }

            //Go through each row in the MainDB.Bait table and check whether we have any in our mobile inventory.
            List<List<Object>> baitSettings = UserSettings.GetList(UserSettings.BOT.FISHER, UserSettings.LIST_TABLE.FISHER_BAITBOX);
            List<BaitBoxItem> baitBoxItems = Statics.Settings.Fisher.BaitBoxItems;
            baitBoxItems.Clear();
            List<Data.Client.Bait.BAIT_INFO> infoList = Data.Client.Bait.GetAllBaitInfo();
            int nbRows = infoList.Count;
            for (int ii = 0; ii < nbRows; ii++)
            {
                //This will loop thru every possible bait in the main db.
                //Get the item ID from the main DB.
                ushort itemId = infoList[ii].ItemID;
                //Get the quantity from the mobile containers.
                ushort itemQuan = Inventory.Containers.GetItemQuanMobile(itemId);
                //We only add the row to the bait box if the quantity is greater than 0.
                if (itemQuan > 0)
                {
                    //We should insert/add the bait name and check into the list based on the priority.
                    //If there is no priority from the user settings, just add it to the end.

                    BaitBoxItem newBaitItem = new BaitBoxItem();
                    bool localChecked = false;
                    byte localPri = 255;
                    //Now go thru the user settings to see if we've saved a checked value or a priority for this bait before.
                    if (baitSettings != null)
                    {
                        for (int kk = 0; kk < baitSettings.Count; kk++)
                        {
                            String localBaitName = Convert.ToString(baitSettings[kk][0]);
                            if (localBaitName == infoList[ii].BaitName)
                            {
                                localChecked = Convert.ToBoolean(baitSettings[kk][1]);
                                localPri = Convert.ToByte(baitSettings[kk][2]);
                                break;
                            }
                        }
                    }
                    //We can now set all of the fields of the new bait box item.
                    newBaitItem.BaitName = infoList[ii].BaitName;
                    newBaitItem.BaitNameShort = infoList[ii].BaitNameShort;
                    newBaitItem.Priority = localPri;
                    newBaitItem.Quantity = itemQuan;

                    //If the check is false at this point, we need to see if it's equipped and check it if so.
                    if (!localChecked && MemReads.Self.Equipment.get_ammo_equipped() && ((ushort)MemReads.Self.Equipment.get_ammo_id() == itemId))
                    {
                        newBaitItem.Use = true;
                        //We have to save this to the settings. Otherwise, if the bait list is updated from the fisher
                        //while we don't have bait equipped and the bait we've been using isn't checked in the settings,
                        //the fisher won't select this bait from that point on.
                        List<Object> newItem = new List<object>();
                        newItem.Add(newBaitItem.BaitName);
                        newItem.Add(newBaitItem.Use);
                        newItem.Add(newBaitItem.Priority);
                        UserSettings.SetListValue(UserSettings.BOT.FISHER, UserSettings.LIST_TABLE.FISHER_BAITBOX, newItem);
                    }
                    else
                    {
                        newBaitItem.Use = localChecked;
                    }

                    //Insert it into the list based on the priority.
                    int listCnt = baitBoxItems.Count;
                    bool inserted = false;
                    for (int kk = 0; kk < listCnt; kk++)
                    {
                        if (localPri < baitBoxItems[kk].Priority)
                        {
                            baitBoxItems.Insert(kk, newBaitItem);
                            inserted = true;
                            break;
                        }
                    }
                    if (!inserted)
                    {
                        baitBoxItems.Add(newBaitItem);
                    }
                }
            }
            if (baitBoxItems.Count == 0)
            {
                LoggingFunctions.Debug("Fisher::c_baitLB_Refresh: bait count 0, returning.", LoggingFunctions.DBG_SCOPE.FISHER);
                return;
            }
            //Go through the list and set any priority that's default (255) to the relative priority.
            //So if they're all 255, start at 0.
            //If the 1st one is not 255, set the subsequent priorities relative to the first one.
            byte basePriority = baitBoxItems[0].Priority;
            for (byte ii = 0; ii < baitBoxItems.Count; ii++)
            {
                if (basePriority == 255)
                {
                    baitBoxItems[ii].Priority = ii;
                }
                else
                {
                    baitBoxItems[ii].Priority = (byte)(basePriority + ii);
                }
            }

            LoggingFunctions.Debug("Fisher::c_baitLB_Refresh: Invoking updateBaitBox.", LoggingFunctions.DBG_SCOPE.FISHER);
            c_baitLB_Update(baitBoxItems, -1);
        }
        #endregion Bait Box
        #region Fish Box
        private void c_loadFishLB()
        {
            try
            {
                if (c_fishLB.InvokeRequired)
                {
                    c_fishLB.Invoke(new Statics.FuncPtrs.TD_Void_Void(c_loadFishLBCBF));
                }
                else
                {
                    c_loadFishLBCBF();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_loadFishLB: " + e.ToString());
            }
        }
        private void c_loadFishLBCBF()
        {
            c_fishLB.BeginUpdate();
            c_fishLB.Items.Clear();
            try
            {
                Statics.Settings.Fisher.CatchAll = Convert.ToBoolean(Settings.UserSettings.GetValue(Settings.UserSettings.BOT.FISHER, "CatchAll"));
                Statics.Settings.Fisher.CatchAllExcept = Convert.ToBoolean(Settings.UserSettings.GetValue(Settings.UserSettings.BOT.FISHER, "CatchAllExcept"));
                if (!(c_fishLB.Items.Contains("* Catch All *")))
                {
                    c_fishLB.Items.Add("* Catch All *", Statics.Settings.Fisher.CatchAll);
                }

                if (!(c_fishLB.Items.Contains("* Catch All Except *")))
                {
                    c_fishLB.Items.Add("* Catch All Except *", Statics.Settings.Fisher.CatchAllExcept);
                }
                if (fishIdsRows != null)
                {
                    foreach (FishStatsDataSet.FishIDsLocalRow row in fishIdsRows)
                    {
                        //In the event that a new ID is found and we change the "Catch" column
                        //it will change the row state to modified from added which will
                        //cause the table adapter to Update the row instead of Inserting it
                        //which throws an exception and kills the new ID.
                        //So we take the original state of the row and set it back accordingly after
                        //we change the "Catch" column value.
                        bool rowWasAdded = row.RowState == DataRowState.Added;
                        bool rowWasModified = row.RowState == DataRowState.Modified;
                        try
                        {
                            Fish.FISH_INFO info = Fish.GetFishInfo((ushort)row.Fish);
                            List<Object> fishSetting = Settings.UserSettings.GetListValue(Settings.UserSettings.BOT.FISHER, Settings.UserSettings.LIST_TABLE.FISHER_CATCHBOX, info.FishName);
                            bool settingTrue = (fishSetting != null);
                            row.Catch = settingTrue;
                            row.Large = info.Large;
                            if (!c_fishLB.Items.Contains(info.FishName))
                            {
                                c_fishLB.Items.Add(info.FishName, settingTrue);
                            }
                        }
                        catch (Exception Ex)
                        {
                            LoggingFunctions.Error("Fisher::c_loadFishLBCBF: Filling catch box: " + Ex);
                        }
                        if (rowWasAdded)
                        {
                            row.AcceptChanges();
                            row.SetAdded();
                        }
                        else if (rowWasModified)
                        {
                            row.AcceptChanges();
                            row.SetModified();
                        }
                        else
                        {
                            row.AcceptChanges();
                        }
                    }
                }
                IocaineFunctions.delay(1);
                Application.DoEvents();
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_loadFishLBCBF: " + e.ToString());
            }
            c_fishLB.EndUpdate();
        }
        #endregion Fish Box
        #region Stats Box
        private void c_autoResetChkB_CheckedChanged(object sender, EventArgs e)
        {
            Statics.Settings.Top.AutoReset = c_autoReset.Checked;
            UserSettings.SetValue(UserSettings.BOT.TOP, "AutoReset", Statics.Settings.Top.AutoReset.ToString());
            UserSettings.SaveCommonSettings();
            filterFishStatsRows();
            c_StatsBoxLoad();
        }
        private void c_thisZoneOnlyChkB_CheckedChanged(object sender, EventArgs e)
        {
            Statics.Settings.Top.ThisZoneOnly = c_thisZoneOnly.Checked;
            UserSettings.SetValue(UserSettings.BOT.TOP, "ThisZoneOnly", Statics.Settings.Top.ThisZoneOnly.ToString());
            UserSettings.SaveCommonSettings();
            filterFishStatsRows();
            c_StatsBoxLoad();
        }
        private void c_StatsBoxLoad()
        {
            try
            {
                if (c_statsLB.InvokeRequired)
                {
                    c_statsLB.Invoke(new Statics.FuncPtrs.TD_Void_Void(c_StatsBoxLoadCBF));
                }
                else
                {
                    c_StatsBoxLoadCBF();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_StatsBoxLoad: " + e.ToString());
            }
        }
        private void c_StatsBoxLoadCBF()
        {
            c_statsLB.BeginUpdate();
            c_statsLB.Items.Clear();
            try
            {
                Monitor.Enter(fishStatsRows);
                int totalRecords = fishStatsRows.Count;
                int[] resultCounters = new int[(byte)FFXIEnums.FISHING_RESULT.UNKNOWN + 1];
                string[] resultEnumString = new string[(byte)FFXIEnums.FISHING_RESULT.UNKNOWN + 1]
                    {"", "* Total Fish Catches *", "* Total Item Catches *", "Monster",
                     "Unwanted", "Didn't catch anything", "Line broke", "Released",
                     "Not enough skill", "Catch got away", "Rod broke", "Too small",
                     "Too large", "Unknown"};
                Dictionary<short, int> catchFishCounts = new Dictionary<short, int>();
                Dictionary<short, int> catchItemsCounts = new Dictionary<short, int>();

                //First we'll go through and add each type of result and total casts.
                //Add Total Casts
                System.Windows.Forms.ListViewItem newItem =
                    new System.Windows.Forms.ListViewItem(new string[] {
                                                            String.Format("{0:0000}", totalRecords),
                                                            "* Total Casts *", "100"}, -1);
                c_statsLB.Items.Insert(0, newItem);

                //Go through and bucket each record into result categories.
                for (int ii = 0; ii < fishStatsRows.Count; ii++)
                {
                    resultCounters[fishStatsRows[ii].Result]++;
                    if (fishStatsRows[ii].Result == (byte)FFXIEnums.FISHING_RESULT.CAUGHT_FISH)
                    {
                        if (catchFishCounts.ContainsKey(fishStatsRows[ii].Fish))
                        {
                            catchFishCounts[fishStatsRows[ii].Fish]++;
                        }
                        else
                        {
                            catchFishCounts.Add(fishStatsRows[ii].Fish, 1);
                        }
                    }
                    else if (fishStatsRows[ii].Result == (byte)FFXIEnums.FISHING_RESULT.CAUGHT_ITEM)
                    {
                        if (catchItemsCounts.ContainsKey(fishStatsRows[ii].Fish))
                        {
                            catchItemsCounts[fishStatsRows[ii].Fish]++;
                        }
                        else
                        {
                            catchItemsCounts.Add(fishStatsRows[ii].Fish, 1);
                        }
                    }
                }
                for (int kk = 1; kk <= (byte)FFXIEnums.FISHING_RESULT.UNKNOWN; kk++)
                {
                    c_addStatsBoxItem(resultCounters[kk], totalRecords, resultEnumString[kk]);
                    if (kk == (int)FFXIEnums.FISHING_RESULT.CAUGHT_FISH)
                    {
                        //Go through the key-value pairs and add each to the box.
                        foreach (KeyValuePair<short, int> kvp in catchFishCounts)
                        {
                            string fishName = Fish.GetFishName((ushort)kvp.Key);
                            c_addStatsBoxItem(kvp.Value, totalRecords, fishName);
                        }
                    }
                    else if (kk == (int)FFXIEnums.FISHING_RESULT.CAUGHT_ITEM)
                    {
                        //Go through the key-value pairs and add each to the box.
                        foreach (KeyValuePair<short, int> kvp in catchItemsCounts)
                        {
                            string itemName = Fish.GetFishName((ushort)kvp.Key);
                            c_addStatsBoxItem(kvp.Value, totalRecords, itemName);
                        }
                    }
                }

                c_statsLB.Sort();
                c_statsLB.EndUpdate();
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_StatsBoxLoadCBF: " + e.ToString());
            }
            finally
            {
                Monitor.Exit(fishStatsRows);
            }
        }
        private void c_addStatsBoxItem(int iCount, int iTotalRecords, string iText)
        {
            float perc = 0;
            if (iCount > 0)
            {
                perc = 100 * iCount / (float)iTotalRecords;
            }
            else
            {
                perc = 0;
            }
            string percString = String.Format("{0:0.#}", perc);
            c_statsLB.Items.Insert(c_statsLB.Items.Count,
                                      new ListViewItem(new string[] {
                                          String.Format("{0:0000}", iCount),
                                          iText, percString}, -1));
            LoggingFunctions.Debug("Fisher::c_addStatsBoxItem: Added '" + iCount + "  " + iText + " " + percString + "'", LoggingFunctions.DBG_SCOPE.FISHER);
        }
        private void c_updateStatsBox(String iLastCatch)
        {
            c_updateStatsBox(iLastCatch, 0);
        }
        private void c_updateStatsBox(String iLastCatch, ushort iItemId)
        {
            try
            {
                if (c_statsLB == null)
                {
                    return;
                }
                c_statsLB.Invoke(new Statics.FuncPtrs.TD_Void_String_UInt16(c_updateStatsBoxCBF), new object[] { iLastCatch, iItemId });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_updateStatsBox: " + e.ToString());
            }
        }
        private void c_updateStatsBoxCBF(String lastCatch, ushort iItemId)
        {
            try
            {
                Monitor.Enter(fishStatsRows);
                bool gotCatch = !((lastCatch == "Didn't catch anything")
                              || (lastCatch == "Line broke")
                              || (lastCatch == "Monster")
                              || (lastCatch == "Release")
                              || (lastCatch == "Not enough skill")
                              || (lastCatch == "Catch got away")
                              || (lastCatch == "Rod broke")
                              || (lastCatch == "Too small")
                              || (lastCatch == "Too large")
                              || (lastCatch == "Unknown")
                              || (lastCatch == "Unwanted"));
                if (gotCatch)
                {
                    Fish.FISH_INFO info = Fish.GetFishInfo(iItemId);
                    string itemText = "";
                    if (info.Type == (byte)Fish.FISH_TYPE.FISH)
                    {
                        itemText = "* Total Fish Catches *";
                    }
                    else if (info.Type == (byte)Fish.FISH_TYPE.ITEM)
                    {
                        itemText = "* Total Item Catches *";
                    }
                    if ((info.ItemID != Fish.InvalidID) && (itemText != ""))
                    {
                        ListViewItem TotalCatchesItem = c_statsLB.FindItemWithText(itemText, true, 0);
                        int totalCatchesIndex = c_statsLB.Items.IndexOf(TotalCatchesItem);
                        int totalCatches = Convert.ToInt32(c_statsLB.Items[totalCatchesIndex].SubItems[0].Text);
                        c_statsLB.Items[totalCatchesIndex].SubItems[0].Text = String.Format("{0:0000}", ++totalCatches);
                    }
                }
                c_statsLB.BeginUpdate();
                bool foundItem = false;
                ListViewItem TotalCastsItem = c_statsLB.FindItemWithText("* Total Casts *", true, 0);
                int totalCastsIndex = c_statsLB.Items.IndexOf(TotalCastsItem);
                int totalCasts = Convert.ToInt32(c_statsLB.Items[totalCastsIndex].SubItems[0].Text);
                for (int ii = 0; ii < c_statsLB.Items.Count; ii++)
                {
                    if (c_statsLB.Items[ii].SubItems[(int)LV_COL.CATCH].Text == lastCatch)
                    {
                        foundItem = true;
                        int count = Convert.ToInt32(c_statsLB.Items[ii].SubItems[(int)LV_COL.CNT].Text);
                        c_statsLB.Items[ii].SubItems[(int)LV_COL.CNT].Text = String.Format("{0:0000}", ++count);
                        float perc = 100 * count / (float)totalCasts;
                        //String percString = perc.ToString();
                        String percString = String.Format("{0:0.#}", perc);
                        c_statsLB.Items[ii].SubItems[(int)LV_COL.PERC].Text = percString;
                    }
                    else if (c_statsLB.Items[ii].SubItems[(int)LV_COL.CNT].Text != "0")
                    {
                        int count = Convert.ToInt32(c_statsLB.Items[ii].SubItems[(int)LV_COL.CNT].Text);
                        float perc = 100 * count / (float)totalCasts;
                        //String percString = perc.ToString();
                        String percString = String.Format("{0:0.#}", perc);
                        c_statsLB.Items[ii].SubItems[(int)LV_COL.PERC].Text = percString;
                    }
                }
                if (foundItem == false)
                {
                    float perc = 100 / (float)totalCasts;
                    //String percString = perc.ToString();
                    //String.Format("{0:0.#}", percString);
                    String percString = String.Format("{0:0.#}", perc);
                    System.Windows.Forms.ListViewItem newItem =
                        new System.Windows.Forms.ListViewItem(new string[] {
                                            "0001", lastCatch, percString}, -1);
                    c_statsLB.Items.Insert(1, newItem);
                }
                c_statsLB.Sort();
                c_statsLB.EndUpdate();
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_updateStatsBoxCBF: " + e.ToString());
            }
            finally
            {
                Monitor.Exit(fishStatsRows);
            }
        }
        private void incCasts()
        {
            c_changeNumberOfCasts(1);
        }
        private void decCasts()
        {
            c_changeNumberOfCasts(-1);
        }
        private void c_changeNumberOfCasts(int nbToChange)
        {
            try
            {
                if (c_statsLB == null)
                {
                    return;
                }
                c_statsLB.BeginInvoke(new Statics.FuncPtrs.TD_Void_Int32(c_changeNumberOfCastsCBF), new object[] { nbToChange });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::c_changeNumberOfCasts: " + e.ToString());
            }
        }
        private void c_changeNumberOfCastsCBF(Int32 iNbToChange)
        {
            try
            {
                Monitor.Enter(fishStatsRows);
                ListViewItem itemArray = c_statsLB.FindItemWithText("* Total casts *", true, 0);
                int totalCastsIndex = c_statsLB.Items.IndexOf(itemArray);
                int totalCasts = Convert.ToInt32(c_statsLB.Items[totalCastsIndex].SubItems[0].Text);
                totalCasts += iNbToChange;
                c_statsLB.Items[totalCastsIndex].SubItems[0].Text = String.Format("{0:0000}", totalCasts);
                c_statsLB.Sort();
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Fisher::changeNumberOfCastsCBF: " + e.ToString());
            }
            finally
            {
                Monitor.Exit(fishStatsRows);
            }
        }
        #endregion Stats Box
        #endregion Control Updates and Call-Backs

        #region Control Event Handlers
        #region Start/Stop
        private void c_startButton_Click(object sender, EventArgs e)
        {
            if (state == Bots.STATE.RUNNING)
            {
                Pause(false);
            }
            else if (state == Bots.STATE.STOPPED)   //stopped from within Fisher
            {
                Start();
            }
            else                                      //2, 3, 4: paused
            {
                Resume();
            }
        }
        private void c_stopButton_Click(object sender, EventArgs e)
        {
            Stop();
        }
        private void c_releaseButton_Click(object sender, EventArgs e)
        {
            if (state != Bots.STATE.STOPPED)
            {
                Pause(true);
            }
        }
        #endregion Start/Stop
        #region Fish Box
        private void c_fishLB_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            bool tempCheck = e.NewValue.ToString() == "Checked" ? true : false;
            String tempItem = c_fishLB.GetItemText(c_fishLB.Items[e.Index]);
            if (tempItem == "* Catch All *")
            {
                Statics.Settings.Fisher.CatchAll = tempCheck;
                if (init_checkDone())
                {
                    UserSettings.SetValue(UserSettings.BOT.FISHER, "CatchAll", Statics.Settings.Fisher.CatchAll.ToString());
                }
                if (tempCheck && Statics.Settings.Fisher.CatchAllExcept)
                {
                    Statics.Settings.Fisher.CatchAllExcept = false;
                    if (init_checkDone())
                    {
                        UserSettings.SetValue(UserSettings.BOT.FISHER, "CatchAllExcept", Statics.Settings.Fisher.CatchAllExcept.ToString());
                    }
                    c_fishLB.SetItemChecked(c_fishLB.Items.IndexOf("* Catch All Except *"), false);
                }
            }
            else if (tempItem == "* Catch All Except *")
            {
                Statics.Settings.Fisher.CatchAllExcept = tempCheck;
                if (init_checkDone())
                {
                    UserSettings.SetValue(UserSettings.BOT.FISHER, "CatchAllExcept", Statics.Settings.Fisher.CatchAllExcept.ToString());
                }
                if (tempCheck && Statics.Settings.Fisher.CatchAll)
                {
                    Statics.Settings.Fisher.CatchAll = false;
                    if (init_checkDone())
                    {
                        UserSettings.SetValue(UserSettings.BOT.FISHER, "CatchAll", Statics.Settings.Fisher.CatchAll.ToString());
                    }
                    c_fishLB.SetItemChecked(c_fishLB.Items.IndexOf("* Catch All *"), false);
                }
            }
            else
            {
                ushort selFishID = Fish.GetFishInfo(tempItem.ToString()).ItemID;
                List<FishStatsDataSet.FishIDsLocalRow> localIdsRows = new List<FishStatsDataSet.FishIDsLocalRow>();
                foreach (FishStatsDataSet.FishIDsLocalRow row in fishIdsRows)
                {
                    if (row.Fish == selFishID)
                    {
                        localIdsRows.Add(row);
                    }
                }
                //In the event that a new ID is found and we change the "Catch" column
                //it will change the row state to modified from added which will
                //cause the table adapter to Update the row instead of Inserting it
                //which throws an exception and kills the new ID.
                //So we take the original state of the row and set it back accordingly after
                //we change the "Catch" column value.

                // We do this for each row returned, because some fish have multiple IDs
                // The single row option reflects all these IDs. (they are different to SE
                // but the same to the user, because they yield the same fish after catching)

                for (int ii = 0; ii < localIdsRows.Count; ii++)
                {
                    bool rowWasAdded = localIdsRows[ii].RowState == DataRowState.Added;
                    bool rowWasModified = localIdsRows[ii].RowState == DataRowState.Modified;
                    localIdsRows[ii].Catch = tempCheck;
                    if (rowWasAdded)
                    {
                        localIdsRows[ii].AcceptChanges();
                        localIdsRows[ii].SetAdded();
                    }
                    else if (rowWasModified)
                    {
                        localIdsRows[ii].AcceptChanges();
                        localIdsRows[ii].SetModified();
                    }
                    else
                    {
                        localIdsRows[ii].AcceptChanges();
                    }
                }
                //Update user settings
                if (init_checkDone())
                {
                    if (tempCheck == true)
                    {
                        List<Object> newItem = new List<object>();
                        newItem.Add(tempItem);
                        UserSettings.AddListValue(UserSettings.BOT.FISHER, UserSettings.LIST_TABLE.FISHER_CATCHBOX, newItem);
                    }
                    else
                    {
                        UserSettings.RemoveListValue(UserSettings.BOT.FISHER, UserSettings.LIST_TABLE.FISHER_CATCHBOX, tempItem);
                    }
                }
            }
        }
        #endregion Fish Box
        #region Drop Box
        private void c_dropLB_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            bool checkValue = e.NewValue == CheckState.Checked ? true : false;
            String itemName = c_dropLB.Items[e.Index].ToString();
            if (init_checkDone())
            {
                List<Object> newItem = new List<object>();
                newItem.Add(itemName);
                newItem.Add(checkValue);
                UserSettings.SetListValue(UserSettings.BOT.FISHER, UserSettings.LIST_TABLE.FISHER_DROPBOX, newItem);
            }
        }
        private void c_dropAddButton_Click(object sender, EventArgs e)
        {
            if ((c_dropItemTB.Text != "") && (c_dropItemTB.Text != "Enter Item Name"))
            {
                c_dropLB.BeginUpdate();
                c_dropLB.Items.Add(c_dropItemTB.Text);
                List<Object> dropItem = new List<object>();
                dropItem.Add(c_dropItemTB.Text);
                dropItem.Add(false);
                UserSettings.AddListValue(UserSettings.BOT.FISHER, UserSettings.LIST_TABLE.FISHER_DROPBOX, dropItem);
                c_dropLB.EndUpdate();
            }
        }
        private void c_dropRemoveButton_Click(object sender, EventArgs e)
        {
            if (c_dropLB.SelectedItems.Count > 0)
            {
                UserSettings.RemoveListValue(UserSettings.BOT.FISHER, UserSettings.LIST_TABLE.FISHER_DROPBOX, c_dropLB.SelectedItem.ToString());
                c_dropLB.Items.Remove(c_dropLB.SelectedItem);
            }
        }
        #endregion Drop Box
        #region Bait Box
        private void c_baitLB_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            bool tempCheck = e.NewValue == CheckState.Checked ? true : false;
            BaitBoxItem boxItem = (BaitBoxItem)c_baitLB.Items[e.Index];
            boxItem.Use = tempCheck;
            if (m_baitLB_InitDone == true)
            {
                List<Object> newItem = new List<object>();
                newItem.Add(boxItem.BaitName);
                newItem.Add(boxItem.Use);
                newItem.Add(boxItem.Priority);
                UserSettings.SetListValue(UserSettings.BOT.FISHER, UserSettings.LIST_TABLE.FISHER_BAITBOX, newItem);
            }
        }
        private void c_baitUpButton_Click(object sender, EventArgs e)
        {
            if (c_baitLB.SelectedItems.Count <= 0)
            {
                return;
            }
            BaitBoxItem selItem = (BaitBoxItem)c_baitLB.SelectedItem;
            int selIdx = c_baitLB.SelectedIndex;
            if ((selIdx == 0) || (c_baitLB.Items.Count <= 1))
            {
                return;
            }
            Statics.Settings.Fisher.BaitBoxItems.Remove(selItem);
            Statics.Settings.Fisher.BaitBoxItems.Insert(selIdx - 1, selItem);
            Statics.Settings.Fisher.BaitBoxItems[selIdx - 1].Priority = (byte)(selIdx - 1);
            Statics.Settings.Fisher.BaitBoxItems[selIdx].Priority = (byte)selIdx;

            //Save these 2 updated priotiries to the user settings.
            //Lower indexed item:
            BaitBoxItem item = Statics.Settings.Fisher.BaitBoxItems[selIdx - 1];
            List<Object> newItem = new List<object>();
            newItem.Add(item.BaitName);
            newItem.Add(item.Use);
            newItem.Add(item.Priority);
            UserSettings.SetListValue(UserSettings.BOT.FISHER, UserSettings.LIST_TABLE.FISHER_BAITBOX, newItem);
            //Upper indexed item:
            item = Statics.Settings.Fisher.BaitBoxItems[selIdx];
            newItem = new List<object>();
            newItem.Add(item.BaitName);
            newItem.Add(item.Use);
            newItem.Add(item.Priority);
            UserSettings.SetListValue(UserSettings.BOT.FISHER, UserSettings.LIST_TABLE.FISHER_BAITBOX, newItem);

            c_baitLB_Update(Statics.Settings.Fisher.BaitBoxItems, selIdx - 1);
        }
        private void c_baitDownButton_Click(object sender, EventArgs e)
        {
            if (c_baitLB.SelectedItems.Count <= 0)
            {
                return;
            }
            BaitBoxItem selItem = (BaitBoxItem)c_baitLB.SelectedItem;
            int selIdx = c_baitLB.SelectedIndex;
            if ((selIdx == c_baitLB.Items.Count - 1) || (c_baitLB.Items.Count <= 1))
            {
                return;
            }
            BaitBoxItem selItemPlusOne = (BaitBoxItem)c_baitLB.Items[selIdx + 1];
            //Remove the higher indexed item and insert it at the selected item index.
            Statics.Settings.Fisher.BaitBoxItems.Remove(selItemPlusOne);
            Statics.Settings.Fisher.BaitBoxItems.Insert(selIdx, selItemPlusOne);
            Statics.Settings.Fisher.BaitBoxItems[selIdx].Priority = (byte)selIdx;
            Statics.Settings.Fisher.BaitBoxItems[selIdx + 1].Priority = (byte)(selIdx + 1);

            //Save these 2 updated priotiries to the user settings.
            //Lower indexed item:
            BaitBoxItem item = Statics.Settings.Fisher.BaitBoxItems[selIdx];
            List<Object> newItem = new List<object>();
            newItem.Add(item.BaitName);
            newItem.Add(item.Use);
            newItem.Add(item.Priority);
            UserSettings.SetListValue(UserSettings.BOT.FISHER, UserSettings.LIST_TABLE.FISHER_BAITBOX, newItem);
            //Upper indexed item:
            item = Statics.Settings.Fisher.BaitBoxItems[selIdx + 1];
            newItem = new List<object>();
            newItem.Add(item.BaitName);
            newItem.Add(item.Use);
            newItem.Add(item.Priority);
            UserSettings.SetListValue(UserSettings.BOT.FISHER, UserSettings.LIST_TABLE.FISHER_BAITBOX, newItem);

            c_baitLB_Update(Statics.Settings.Fisher.BaitBoxItems, selIdx + 1);
        }
        private void c_baitRefreshButton_Click(object sender, EventArgs e)
        {
            c_baitLB_Refresh(false, true);
        }
        #endregion Bait Box
        #endregion Control Event Handlers
    }
}
