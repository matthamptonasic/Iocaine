using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Data.Server;
using Iocaine2.Logging;
using ZedGraph;

namespace Iocaine2
{
    public partial class Stats_Form : Form
    {
        #region Members
        #region Private Members
        //Filter lists - Anything in these lists should be filtered from the result tables.
        //               This means that anything in these lists should NOT be checked in the checkedListBoxes
        ArrayList baitFilterList;
        ArrayList rodFilterList;
        ArrayList graphColorList;
        Byte FSCurrentMapInCurrentZone = 1;
        Byte FSNbMapsInCurrentZone = 1;
        bool FSZoneTabDoingInits = false;
        private static Boolean dbStatusMsgShown = false;
        //Initial index settings
        private int initFishCBIndex;
        private int initZoneCBIndex;
        private int initZoneRodCBIndex;
        private int initZoneBaitCBIndex;
        private int initRodCBIndex;
        private int initBaitCBIndex;
        //Some little flags to tell if we're initializing
        //the checkedListBoxes or if the check is from the user
        private bool doingListBoxInits = false;
        private bool constructing = true;
        private Dictionary<String, UInt16> fullZoneNameToIdMap = null;
        private Dictionary<String, UInt16> fullZoneNameToAliasMap = null;
        #endregion Private Members
        #region Event Related Members
        public delegate void SettingsChangedHandler(object sender, StatsSettingsUpdateEventArgs e);
        public event SettingsChangedHandler SettingsChanged;
        #endregion Event Related Members
        #endregion Members
        #region Constructor
        public Stats_Form(ArrayList iBaitFiltersList, ArrayList iRodFiltersList, int fishCBIndex, int zoneCBIndex,
                          int zoneRodCBIndex, int zoneBaitCBIndex, int rodCBIndex, int baitCBIndex)
        {
            InitializeComponent();
            baitFilterList = iBaitFiltersList;
            rodFilterList = iRodFiltersList;
            initFishCBIndex = fishCBIndex;
            initZoneCBIndex = zoneCBIndex;
            initZoneRodCBIndex = zoneRodCBIndex;
            initZoneBaitCBIndex = zoneBaitCBIndex;
            initRodCBIndex = rodCBIndex;
            initBaitCBIndex = baitCBIndex;
            constructing = true;
            doFSInits();
            constructing = false;
            Cursor.Current = Cursors.Default;
        }
        #endregion Constructor
        #region Inits
        #region Top Inits
        private void doFSInits()
        {
            LoggingFunctions.Debug("FS::doFSInits: Initializing Stats Form.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            FSZoneTabDoingInits = true;
            Data.Client.FishImages.init();
            Data.Client.BaitImages.init();
            Data.Client.RodImages.init();
            
            //Init Fish tab
            FSLoadFishComboBox();
            StatsFishZoneListBox.DataSource = fishStatsDataSet.ZoneTable;
            StatsFishZoneListBox.DisplayMember = "Zone";
            StatsFishZoneListBox.ValueMember = "Checked";
            for (int ii = 0; ii < StatsFishZoneListBox.Items.Count; ii++)
            {
                StatsFishZoneListBox.SetItemChecked(ii, true);
            }
            FSLoadFishRodsListBox();
            FSReloadLocalZoneCatchTable();
            
            //Init Zone tab
            FSInitZoneTabGraph();
            FSLoadZoneComboBox();

            //Init Rods tab
            FSLoadRodComboBox();

            //Init Bait tab
            FSLoadBaitComboBox();
            FSInitZoneTabValues();

            checkProcessingStatus();
        }
        private static void checkProcessingStatus()
        {
            DataTable progressTable;
            if(Server.SQL.FISHREFERENCEDB.FishStatsProcessingStatus(out progressTable))
            {
                if (progressTable.Rows.Count > 0)
                {
                    Double percDone = (Double)progressTable.Rows[0]["PercentDone"];
                    if ((percDone < 95f) && !dbStatusMsgShown)
                    {
                        String msg = "The database is only " + percDone.ToString("0.0#") + "% processed.\n\n";
                        msg += "There are currently " + String.Format("{0:n0}", progressTable.Rows[0]["Total"]) + " records in the DB ";
                        msg += "which takes a lot of time to process.\n";
                        msg += "So if the information you're looking for isn't here, please check back soon.\n\n";
                        msg += "Also check out the new features like the dots on the zone maps where things are caught.\n";
                        msg += "And you can double click on fish, zones, bait, and rods to show you more information on that item.";
                        MessageBox.Show(msg);
                        dbStatusMsgShown = true;
                    }
                }
            }
        }
        #endregion Top Inits
        #region Fish Tab Inits
        private void FSLoadFishComboBox()
        {
            LoggingFunctions.Debug("FS::FSLoadFishComboBox: Loading Stats Form Fish Combo Box.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            FSFishComboBox.BeginUpdate();
            List<Data.Client.Fish.FISH_INFO> infoList = Data.Client.Fish.GetAllFishInfo(false);
            for (int ii = 0; ii < infoList.Count; ii++)
            {
                String fishName = infoList[ii].FishName;
                if((fishName == "1 gil") 
                || (fishName == "100 gil") 
                || (fishName == "aurora bass") 
                || (fishName == "generic item") 
                || (fishName == "Monster")
                || (fishName == "unknown")
                )
                {
                    continue;
                }
                FSFishComboBox.Items.Add(infoList[ii].FishName);
            }
            if (initFishCBIndex < FSFishComboBox.Items.Count)
            {
                FSFishComboBox.SelectedIndex = initFishCBIndex;
            }
            else
            {
                FSFishComboBox.SelectedIndex = 0;
            }
            FSFishComboBox.EndUpdate();
        }
        private void FSLoadFishRodsListBox()
        {
            StatsFishRodsListBox.BeginUpdate();

            LoggingFunctions.Debug("FS::FSLoadFishRodsListBox: Loading Stats Form, Rods Combo Box.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            StatsFishRodsListBox.BeginUpdate();
            List<Rods.ROD_INFO> rodList = Rods.GetAllRodInfo();
            foreach(Rods.ROD_INFO info in rodList)
            {
                StatsFishRodsListBox.Items.Add(info.RodName);
            }

            int itemCnt = StatsFishRodsListBox.Items.Count;
            for (int ii = 0; ii < itemCnt; ii++)
            {
                bool filterOn = false;
                if (rodFilterList.Contains(StatsFishRodsListBox.GetItemText(StatsFishRodsListBox.Items[ii])))
                {
                    filterOn = true;
                }
                StatsFishRodsListBox.SetItemChecked(ii, !filterOn);
            }
            StatsFishRodsListBox.EndUpdate();
        }
        #endregion Fish Tab Inits
        #region Zone Tab Inits
        private void FSLoadZoneComboBox()
        {
            LoggingFunctions.Debug("FS::FSLoadZoneComboBox: Loading Stats Form Zone Combo Box.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            FSZoneComboBox.BeginUpdate();
            FSZoneComboBox.Items.Clear();
            fullZoneNameToIdMap = new Dictionary<string, ushort>();
            fullZoneNameToAliasMap = new Dictionary<string, ushort>();
            if (fishStatsDataSet.ZoneTableFull.Rows.Count == 0)
            {
                fishStatsDataSet.ZoneTableFull.Rows.Clear();
                List<Zones.ZONE_INFO> zoneInfoList = Zones.GetAllZoneInfo();
                for(int ii=0; ii<zoneInfoList.Count; ii++)
                {
                    FishStatsDataSet.ZoneTableFullRow row = fishStatsDataSet.ZoneTableFull.NewZoneTableFullRow();
                    row.ZoneName = zoneInfoList[ii].Zone;
                    row.ZoneID = zoneInfoList[ii].ZoneID;
                    row.MultiAreas = zoneInfoList[ii].MultiAreas;
                    row.AreaName = zoneInfoList[ii].AreaName;
                    row.AreaID = zoneInfoList[ii].AreaID;
                    row.AliasID = zoneInfoList[ii].AliasID;
                    row.XMin = zoneInfoList[ii].XMin;
                    row.XMax = zoneInfoList[ii].XMax;
                    row.YMin = zoneInfoList[ii].YMin;
                    row.YMax = zoneInfoList[ii].YMax;
                    fishStatsDataSet.ZoneTableFull.Rows.Add(row);
                }
            }
            LoggingFunctions.Debug("========= Loading Zone Combo Box ==========", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            Int32 cnt = 0;
            foreach (FishStatsDataSet.ZoneTableFullRow row in fishStatsDataSet.ZoneTableFull.Rows)
            {
                String ZoneName = row.ZoneName;
                if ((row.MultiAreas == 1) && (row.AreaID != 0))
                {
                    ZoneName += " " + row.AreaName;
                }
                if (!FSZoneComboBox.Items.Contains(ZoneName) && !((row.MultiAreas == 1) && (row.AreaID == 0)))
                {
                    FSZoneComboBox.Items.Add(ZoneName);
                    fullZoneNameToIdMap[ZoneName] = row.ZoneID;
                    fullZoneNameToAliasMap[ZoneName] = row.AliasID;
                }
                LoggingFunctions.Debug("[" + cnt++ + "]: " + ZoneName, LoggingFunctions.DBG_SCOPE.FISH_STAT);
            }
            LoggingFunctions.Debug("Zone items in box: " + FSZoneComboBox.Items.Count + ".", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            LoggingFunctions.Debug("========= Done Loading Zone Combo Box ==========", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            FSZoneComboBox.EndUpdate();
        }
        private void FSInitZoneTabValues()
        {
            //We don't want the first two to query the DB, so leave the DoingInits true until then after.
            LoggingFunctions.Debug("FS::FSInitZoneTabValues: Setting initial combo box indices for Zone Tab.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            FSZoneRodComboBox.SelectedIndex = initZoneRodCBIndex;
            FSZoneBaitComboBox.SelectedIndex = initZoneBaitCBIndex;
            FSZoneTabDoingInits = false;
            FSZoneComboBox.SelectedIndex = initZoneCBIndex;
            LoggingFunctions.Debug("FS::FSInitZoneTabValues: Set Zone combo box index to 0.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
        }
        private void FSInitZoneTabGraph()
        {
            FSZoneGraph.GraphPane.Title.Text = "Bite Rate";
            FSZoneGraph.GraphPane.Title.FontSpec.Size = 28;
            FSZoneGraph.GraphPane.Legend.IsVisible = false;
            //FSZoneGraph.BackgroundImage = Properties.Resources.Fishing_Guild;
            //FSZoneGraph.BackgroundImageLayout = ImageLayout.Stretch;
            FSZoneGraph.GraphPane.Chart.Fill = new Fill(Color.LightGray, Color.DarkGray);
            FSZoneGraph.GraphPane.Fill = new Fill(Color.LightGray, Color.DarkGray);
            graphColorList = new ArrayList();
            graphColorList.Add(Color.Blue);
            graphColorList.Add(Color.Green);
            graphColorList.Add(Color.Red);
            graphColorList.Add(Color.Yellow);
            graphColorList.Add(Color.Purple);
            graphColorList.Add(Color.Orange);
            graphColorList.Add(Color.Cyan);
            graphColorList.Add(Color.PaleGreen);
            graphColorList.Add(Color.Pink);
            graphColorList.Add(Color.Gold);
            graphColorList.Add(Color.Violet);
            graphColorList.Add(Color.Brown);
            graphColorList.Add(Color.Teal);
            graphColorList.Add(Color.Aqua);
            graphColorList.Add(Color.BlueViolet);
            graphColorList.Add(Color.Crimson);
            graphColorList.Add(Color.DarkGoldenrod);
            graphColorList.Add(Color.DarkOrchid);
            graphColorList.Add(Color.Tan);
            graphColorList.Add(Color.Chocolate);
        }
        #endregion Zone Tab Inits
        #region Rods Tab Inits
        private void FSLoadRodComboBox()
        {
            LoggingFunctions.Debug("FS::FSLoadRodComboBox: Loading Stats Form Rods Combo Box.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            FSRodComboBox.BeginUpdate();
            FSZoneRodComboBox.BeginUpdate();
            FSRodComboBox.Items.Clear();
            FSZoneRodComboBox.Items.Clear();
            List<Rods.ROD_INFO> rodInfoList = Rods.GetAllRodInfo();
            for (int ii = 0; ii < rodInfoList.Count; ii++)
            {
                if(rodInfoList[ii].ItemID == 0)
                {
                    continue;
                }
                FSRodComboBox.Items.Add(rodInfoList[ii].RodName);
                FSZoneRodComboBox.Items.Add(rodInfoList[ii].RodName);
            }
            FSRodComboBox.SelectedIndex = initRodCBIndex;
            FSRodComboBox.EndUpdate();
            FSZoneRodComboBox.EndUpdate();
        }
        #endregion Rods Tab Inits
        #region Bait Tab Inits
        private void FSLoadBaitComboBox()
        {
            LoggingFunctions.Debug("FS::FSLoadBaitComboBox: Loading Stats Form Bait Combo Box.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            FSBaitComboBox.BeginUpdate();
            FSZoneBaitComboBox.BeginUpdate();
            FSBaitComboBox.Items.Clear();
            FSZoneBaitComboBox.Items.Clear();
            List<Bait.BAIT_INFO> baitInfoList = Bait.GetAllBaitInfo();
            for (int ii = 0; ii < baitInfoList.Count; ii++)
            {
                FSBaitComboBox.Items.Add(baitInfoList[ii].BaitName);
                FSZoneBaitComboBox.Items.Add(baitInfoList[ii].BaitName);
            }
            FSBaitComboBox.SelectedIndex = initBaitCBIndex;
            FSBaitComboBox.EndUpdate();
            FSZoneBaitComboBox.EndUpdate();
        }
        #endregion Bait Tab Inits
        #endregion Initialization
        #region Page Updates
        #region Fish Tab Page Updates
        private void FSFishLoadImage()
        {
            LoggingFunctions.Debug("FS::FSFishLoadImage: Loading fish image of index: " + FSFishComboBox.SelectedIndex + ".", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            String fishName = (String)FSFishComboBox.SelectedItem;
            UInt16 fishId = Data.Client.Fish.GetFishInfo(fishName).ItemID;
            FSFishPictureBox.Image = Data.Client.FishImages.getImageByID(fishId);
        }
        private void FSReloadLocalZoneCatchTable()
        {
            fishStatsDataSet.LocalZoneCatchTable.BeginLoadData();
            fishStatsDataSet.LocalZoneCatchTable.Rows.Clear();
            String filter = "";
            int zoneCount = StatsFishZoneListBox.Items.Count;
            for (int ii = 0; ii < zoneCount; ii++)
            {
                if (StatsFishZoneListBox.GetItemChecked(ii))
                {
                    continue;
                }
                if (filter != "")
                {
                    filter += " AND ";
                }
                filter += ("Zone <> '" + StatsFishZoneListBox.GetItemText(StatsFishZoneListBox.Items[ii]) + "'");
            }
            int baitCount = StatsFishBaitListBox.Items.Count;
            for (int kk = 0; kk < baitCount; kk++)
            {
                if (StatsFishBaitListBox.GetItemChecked(kk))
                {
                    continue;
                }
                if (filter != "")
                {
                    filter += " AND ";
                }
                filter += ("Bait <> '" + StatsFishBaitListBox.GetItemText(StatsFishBaitListBox.Items[kk]) + "'");
            }
            int rodCount = StatsFishRodsListBox.Items.Count;
            for (int mm = 0; mm < rodCount; mm++)
            {
                if (StatsFishRodsListBox.GetItemChecked(mm))
                {
                    continue;
                }
                if (filter != "")
                {
                    filter += " AND ";
                }
                filter += ("Rod <> '" + StatsFishRodsListBox.GetItemText(StatsFishRodsListBox.Items[mm]) + "'");
            }

            LoggingFunctions.Debug("FS::FSReloadLocalZoneCatchTable: Filter to load localZoneCatchTable is " + filter + ".", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            if (fishStatsDataSet.ZoneCatchTable.Rows.Count > 0)
            {
                DataRow[] selectedRows = fishStatsDataSet.ZoneCatchTable.Select(filter);
                foreach (DataRow row in selectedRows)
                {
                    fishStatsDataSet.LocalZoneCatchTable.ImportRow(row);
                }
            }
            fishStatsDataSet.LocalZoneCatchTable.EndLoadData();
            LoggingFunctions.Debug("FS::FSReloadLocalZoneCatchTable: Loaded " + fishStatsDataSet.LocalZoneCatchTable.Rows.Count + " rows into localZoneCatchDT.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
        }
        private void FSReloadLocalRodResultsTable()
        {
            fishStatsDataSet.LocalRodResultsTable.BeginLoadData();
            fishStatsDataSet.LocalRodResultsTable.Rows.Clear();
            String filter = "";
            int rodCount = StatsFishRodsListBox.Items.Count;
            for (int mm = 0; mm < rodCount; mm++)
            {
                if (StatsFishRodsListBox.GetItemChecked(mm))
                {
                    continue;
                }
                if (filter != "")
                {
                    filter += " AND ";
                }
                filter += ("Rod <> '" + StatsFishRodsListBox.GetItemText(StatsFishRodsListBox.Items[mm]) + "'");
            }
            if (fishStatsDataSet.RodResultsTable.Rows.Count > 0)
            {
                DataRow[] selectedRows = fishStatsDataSet.RodResultsTable.Select(filter);
                foreach (DataRow row in selectedRows)
                {
                    fishStatsDataSet.LocalRodResultsTable.ImportRow(row);
                }
            }
            fishStatsDataSet.LocalRodResultsTable.EndLoadData();
        }
        private void FSFishLoadServerData()
        {
            

            String selectedFish = (String)FSFishComboBox.SelectedItem;
            UInt16 selectedID = Data.Client.Fish.GetFishInfo(selectedFish).ItemID;

            FishStatsDataSet.ZoneCatchTableDataTable localZoneCatchTable = null;
            FishStatsDataSet.RodResultsTableDataTable localRodResultsTable = null;
            FishStatsDataSet.ZoneTableDataTable localZoneTable = null;
            if (!Server.SQL.FISHREFERENCEDB.LoadServerData(selectedID, out localZoneCatchTable, out localRodResultsTable, out localZoneTable))
            {
                LoggingFunctions.Error("Stats_Form::FSFishLoadServerData: Could not load tables from server.");
                return;
            }
            
            //Merge the tables created with the main DB's tables.
            fishStatsDataSet.ZoneCatchTable.Clear();
            fishStatsDataSet.RodResultsTable.Clear();
            fishStatsDataSet.ZoneTable.Clear();
            fishStatsDataSet.ZoneCatchTable.Merge(localZoneCatchTable);
            fishStatsDataSet.RodResultsTable.Merge(localRodResultsTable);
            fishStatsDataSet.ZoneTable.Merge(localZoneTable);

            LoggingFunctions.Debug("FS::FSFishLoadServerData: Loaded " + fishStatsDataSet.ZoneCatchTable.Rows.Count + " rows into zoneCatchDT.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            LoggingFunctions.Debug("FS::FSFishLoadServerData: Loaded " + fishStatsDataSet.RodResultsTable.Rows.Count + " rows into rodResultsDT.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            LoggingFunctions.Debug("FS::FSFishLoadServerData: Loaded " + fishStatsDataSet.ZoneTable.Rows.Count + " rows into zoneTable.", LoggingFunctions.DBG_SCOPE.FISH_STAT);

            doingListBoxInits = true;
            for (int ii = 0; ii < StatsFishZoneListBox.Items.Count; ii++)
            {
                StatsFishZoneListBox.SetItemChecked(ii, true);
            }

            //Load Bait List
            StatsFishBaitListBox.Items.Clear();
            if (fishStatsDataSet.ZoneCatchTable.Rows.Count > 0)
            {
                DataRow[] selectedRows = fishStatsDataSet.ZoneCatchTable.Select();
                foreach (DataRow row in selectedRows)
                {
                    LoggingFunctions.Debug("FS::FSFishLoadServerData: Bait in table: " + row["Bait"] + ".", LoggingFunctions.DBG_SCOPE.FISH_STAT);
                    if (!StatsFishBaitListBox.Items.Contains(row["Bait"]))
                    {
                        LoggingFunctions.Debug("FS::FSFishLoadServerData: Adding that bait to the list.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
                        StatsFishBaitListBox.Items.Add(row["Bait"]);
                    }

                }
            }
            int nbBaitListed = StatsFishBaitListBox.Items.Count;
            for (int ii = 0; ii < nbBaitListed; ii++)
            {
                if (baitFilterList.Contains(StatsFishBaitListBox.GetItemText(StatsFishBaitListBox.Items[ii])))
                {
                    StatsFishBaitListBox.SetItemChecked(ii, false);
                }
                else
                {
                    StatsFishBaitListBox.SetItemChecked(ii, true);
                }
            }
            StatsFishBaitListBox.Sorted = true;
            doingListBoxInits = false;
        }
        private void FSFishLoadFish(String iFishName)
        {
            if(FSFishComboBox.Items.Contains(iFishName))
            {
                this.FishStatsTabControl.SelectedIndex = this.FishStatsTabControl.TabPages.IndexOfKey("FSFishTab");
                FSFishComboBox.SelectedItem = iFishName;
            }
        }
        #endregion Fish Tab Page Updates
        #region Zone Tab Page Updates
        private void FSZoneLoadServerDataFiltered()
        {
            //Get current selected ids
            String baitName = (String)FSZoneBaitComboBox.SelectedItem;
            UInt16 baitID = Bait.GetBaitInfo(baitName).ItemID;
            String rodName = (String)FSZoneRodComboBox.SelectedItem;
            UInt16 rodID = Rods.GetRodInfo(rodName).ItemID;
            String zoneName = (String)FSZoneComboBox.SelectedItem;
            UInt16 zoneID = fullZoneNameToAliasMap[zoneName];
            LoggingFunctions.Debug("FS::FSZoneLoadServerDataFiltered: Reloading ZoneFishDGV with zoneID: " + zoneID + ", rodID: " + rodID + ", baitID: " + baitID + ".", LoggingFunctions.DBG_SCOPE.FISH_STAT);

            FishStatsDataSet.ZoneTabCatchTableDataTable localTable = null;
            if (!Server.SQL.FISHREFERENCEDB.LoadServerZoneDataFiltered(zoneID, rodID, baitID, out localTable))
            {
                LoggingFunctions.Error("Stats_Form::FSZoneLoadServerDataFiltered: Loading server data failed.");
                return;
            }

            fishStatsDataSet.ZoneTabCatchTable.Clear();
            fishStatsDataSet.ZoneTabCatchTable.Merge(localTable);
            LoggingFunctions.Debug("FS::FSZoneLoadServerDataFiltered: Loaded " + fishStatsDataSet.ZoneTabCatchTable.Rows.Count + " rows into ZoneTabCatchTable.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            
            //Load the total casts into the FSZoneAttemptsValue
            int nbCasts;
            if (fishStatsDataSet.ZoneTabCatchTable.Rows.Count != 0)
            {
                nbCasts = (int)fishStatsDataSet.ZoneTabCatchTable[0]["Attempts"];
            }
            else
            {
                nbCasts = 0;
            }
            FSZoneAttemptsValue.Text = nbCasts.ToString();
        }
        private void FSZoneLoadServerData()
        {
            //Get all fish for this zone/area
            String zoneName = (String)FSZoneComboBox.SelectedItem;
            UInt16 zoneID = fullZoneNameToAliasMap[zoneName];

            FishStatsDataSet.ZoneTabCatchTableUnfilteredDataTable localZoneTabCatchTable = null;
            FishStatsDataSet.PositionCatchDataTable localPositionCatchTable = null;

            if (!Server.SQL.FISHREFERENCEDB.LoadServerZoneData(zoneID, out localZoneTabCatchTable, out localPositionCatchTable))
            {
                LoggingFunctions.Error("Stats_Form::FSZoneLoadServerData: Could not load tables from server.");
                return;
            }
            fishStatsDataSet.ZoneTabCatchTableUnfiltered.Clear();
            fishStatsDataSet.PositionCatch.Clear();
            fishStatsDataSet.ZoneTabCatchTableUnfiltered.Merge(localZoneTabCatchTable);
            fishStatsDataSet.PositionCatch.Merge(localPositionCatchTable);

            LoggingFunctions.Debug("FS::FSZoneLoadServerData: Loaded " + fishStatsDataSet.ZoneTabCatchTableUnfiltered.Rows.Count + " rows into ZoneTabCatchTableUnfiltered.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            LoggingFunctions.Debug("FS::FSZoneLoadServerData: Loaded " + fishStatsDataSet.PositionCatch.Rows.Count + " rows into PositionCatch.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
        }
        private void FSZoneTabGraphReload()
        {
            //Add a slice for each row in the ZoneTabCatchTable
            FSZoneGraph.GraphPane.CurveList.Clear();
            double percLeft = 100.0;
            int nbCatches = fishStatsDataSet.ZoneTabCatchTable.Rows.Count;
            for (int ii = 0; ii < nbCatches; ii++)
            {
                float rate = Convert.ToSingle(fishStatsDataSet.ZoneTabCatchTable.Rows[ii]["BiteRate"]);
                percLeft -= rate;
                String fishName = Convert.ToString(fishStatsDataSet.ZoneTabCatchTable.Rows[ii]["Fish"]);
                PieItem slice = FSZoneGraph.GraphPane.AddPieSlice(rate, (Color)graphColorList[ii%graphColorList.Count], Color.White, 45, .03, fishName);
                slice.LabelDetail.FontSpec.Size = 24;
                slice.LabelType = PieLabelType.Name_Percent;
                slice.LabelDetail.IsClippedToChartRect = false;
            }
            if (percLeft > 0)
            {
                PieItem noCatchSlice = FSZoneGraph.GraphPane.AddPieSlice(percLeft, (Color)graphColorList[nbCatches], Color.White, 45, 0, "No Catch");
                noCatchSlice.LabelDetail.FontSpec.Size = 24;
                noCatchSlice.LabelType = PieLabelType.Name_Percent;
            }
            FSZoneGraph.GraphPane.GraphObjList.Clear();
            TextObj totalBox = new TextObj("Total Casts:\n" + FSZoneAttemptsValue.Text, .2f, 1.15f);
            totalBox.Location.AlignH = AlignH.Center;
            totalBox.Location.AlignV = AlignV.Center;
            totalBox.FontSpec.Size = 24;
            totalBox.FontSpec.Border.IsVisible = true;
            totalBox.FontSpec.Fill = new Fill(Color.White, Color.Red, 30);
            totalBox.FontSpec.StringAlignment = StringAlignment.Center;
            FSZoneGraph.GraphPane.GraphObjList.Add(totalBox);
            //Create a shadow for it
            TextObj totalBoxShadow = new TextObj(totalBox);
            totalBoxShadow.FontSpec.Fill = new Fill(Color.DarkGray);
            totalBoxShadow.FontSpec.FontColor = Color.DarkGray;
            totalBoxShadow.FontSpec.Border.IsVisible = false;
            totalBoxShadow.Location.X += .02f;
            totalBoxShadow.Location.Y -= .02f;
            FSZoneGraph.GraphPane.GraphObjList.Add(totalBoxShadow);
            FSZoneGraph.AxisChange();
            FSZoneGraph.Refresh();
        }
        private void FSZoneLoadSetup(String iZoneName, String iFishName, String iBaitName, String iRodName)
        {
            if(!FSZoneComboBox.Items.Contains(iZoneName))
            {
                return;
            }
            FSZoneComboBox.SelectedItem = iZoneName;
            Boolean foundFish = false;
            Int32 fishIdx = -1;
            foreach(DataGridViewRow row in FSZoneFishUnFilteredDGV.Rows)
            {
                String fish = (String)row.Cells[0].Value;
                if(fish == iFishName)
                {
                    foundFish = true;
                    fishIdx = row.Index;
                }
                else
                {
                    row.Selected = false;
                }
            }
            if(!foundFish)
            {
                return;
            }
            if(!FSZoneBaitComboBox.Items.Contains(iBaitName))
            {
                return;
            }
            if(!FSZoneRodComboBox.Items.Contains(iRodName))
            {
                return;
            }
            FSZoneBaitComboBox.SelectedItem = iBaitName;
            FSZoneRodComboBox.SelectedItem = iRodName;
            this.FishStatsTabControl.SelectedIndex = this.FishStatsTabControl.TabPages.IndexOfKey("FSZoneTab");
            foreach (DataGridViewRow row in FSZoneFishUnFilteredDGV.Rows)
            {
                row.Selected = false;
            }
            if (fishIdx >= 0)
            {
                FSZoneFishUnFilteredDGV.Rows[fishIdx].Selected = true;
                FSZoneFishUnFilteredDGV.FirstDisplayedScrollingRowIndex = fishIdx;
                FSZoneLoadMapByFish(0, fishIdx);
            }
            //Now do the same for the ZoneFishDGV (filtered list).
            fishIdx = -1;
            foreach(DataGridViewRow row in FSZoneFishDGV.Rows)
            {
                String fish = (String)row.Cells[0].Value;
                if(fish == iFishName)
                {
                    fishIdx = row.Index;
                }
                row.Selected = false;
            }
            if(fishIdx >= 0)
            {
                FSZoneFishDGV.Rows[fishIdx].Selected = true;
                FSZoneFishDGV.FirstDisplayedScrollingRowIndex = fishIdx;
            }

        }
        private void FSZoneLoadMapByFish(Int32 iColIdx, Int32 iRowIdx)
        {
            DataGridViewSelectedCellCollection selCells = FSZoneFishUnFilteredDGV.SelectedCells;
            String zoneName = (String)FSZoneComboBox.SelectedItem;
            UInt16 zoneId = fullZoneNameToIdMap[zoneName];
            if (selCells.Count == 0)
            {
                FSZoneLoadMap(zoneId, false, true, "");
            }
            else
            {
                String selFish = (String)FSZoneFishUnFilteredDGV[iColIdx, iRowIdx].Value;
                FSZoneLoadMap(zoneId, false, false, selFish);
            }
        }
        #endregion Zone Tab Page Updates
        #region Rods Tab Page Updates
        private void FSRodsLoadImage()
        {
            LoggingFunctions.Debug("FS::FSRodsLoadImage Loading rod image of index: " + FSRodComboBox.SelectedIndex + ".", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            String rodName = (String)FSRodComboBox.SelectedItem;
            UInt16 rodId = Rods.GetRodInfo(rodName).ItemID;
            FSRodPictureBox.Image = Data.Client.RodImages.getImageByID(rodId);
        }
        private void FSRodsLoadServerData()
        {
            //Load Rods Fish Table
            String rodName = (String)FSRodComboBox.SelectedItem;
            UInt16 rodId = Rods.GetRodInfo(rodName).ItemID;

            FishStatsDataSet.RodsFishTableDataTable localRodsFishTable = null;

            if (!Server.SQL.FISHREFERENCEDB.LoadServerRodsData(rodId, out localRodsFishTable))
            {
                LoggingFunctions.Error("Stats_Form::FSZoneLoadServerData: Could not load tables from server.");
                return;
            }
            fishStatsDataSet.RodsFishTable.Clear();
            fishStatsDataSet.RodsFishTable.Merge(localRodsFishTable);

            LoggingFunctions.Debug("FS::FSRodsLoadServerData: Loaded " + fishStatsDataSet.RodsFishTable.Rows.Count + " rows into RodsFishTable.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
        }
        private void FSRodsLoadRod(String iRodName, String iFishName)
        {
            if(FSRodComboBox.Items.Contains(iRodName))
            {
                FSRodComboBox.SelectedItem = iRodName;
                this.FishStatsTabControl.SelectedIndex = this.FishStatsTabControl.TabPages.IndexOfKey("FSRodTab");
                Int32 fishIdx = -1;
                foreach(DataGridViewRow row in FSRodDGV.Rows)
                {
                    String fish = (String)row.Cells[0].Value;
                    if(fish == iFishName)
                    {
                        row.Selected = true;
                        fishIdx = row.Index;
                    }
                    else
                    {
                        row.Selected = false;
                    }
                }
                if (fishIdx >= 0)
                {
                    FSRodDGV.FirstDisplayedScrollingRowIndex = fishIdx;
                }
            }
        }
        #endregion Rods Tab Page Updates
        #region Bait Tab Page Updates
        private void FSBaitLoadImage()
        {
            LoggingFunctions.Debug("FS::FSBaitLoadImage: Loading bait image of index: " + FSBaitComboBox.SelectedIndex + ".", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            String baitName = (String)FSBaitComboBox.SelectedItem;
            UInt16 baitId = Bait.GetBaitInfo(baitName).ItemID;
            FSBaitPictureBox.Image = Data.Client.BaitImages.getImageByID(baitId);
        }
        private void FSBaitLoadServerData()
        {
            String baitName = (String)FSBaitComboBox.SelectedItem;
            UInt16 baitId = Bait.GetBaitInfo(baitName).ItemID;

            FishStatsDataSet.BaitFishWillCatchDataTable localBaitFishWillCatch = null;
            FishStatsDataSet.BaitFishWontCatchDataTable localBaitFishWontCatch = null;
            FishStatsDataSet.BaitFishMayCatchDataTable localBaitFishMayCatch = null;
            FishStatsDataSet.BaitZoneWillCatchDataTable localBaitZoneWillCatch = null;
            FishStatsDataSet.BaitZoneWontCatchDataTable localBaitZoneWontCatch = null;
            FishStatsDataSet.BaitZoneMayCatchDataTable localBaitZoneMayCatch = null;
            if (!Server.SQL.FISHREFERENCEDB.LoadServerBaitData(baitId, out localBaitFishWillCatch, out localBaitFishWontCatch, out localBaitFishMayCatch,
                                                                       out localBaitZoneWillCatch, out localBaitZoneWontCatch, out localBaitZoneMayCatch))
            {
                LoggingFunctions.Error("Stats_Form::FSBaitLoadServerData: Could not load tables from server.");
                return;
            }
            fishStatsDataSet.BaitFishWillCatch.Clear();
            fishStatsDataSet.BaitFishWontCatch.Clear();
            fishStatsDataSet.BaitFishMayCatch.Clear();
            fishStatsDataSet.BaitZoneWillCatch.Clear();
            fishStatsDataSet.BaitZoneWontCatch.Clear();
            fishStatsDataSet.BaitZoneMayCatch.Clear();
            fishStatsDataSet.BaitFishWillCatch.Merge(localBaitFishWillCatch);
            fishStatsDataSet.BaitFishWontCatch.Merge(localBaitFishWontCatch);
            fishStatsDataSet.BaitFishMayCatch.Merge(localBaitFishMayCatch);
            fishStatsDataSet.BaitZoneWillCatch.Merge(localBaitZoneWillCatch);
            fishStatsDataSet.BaitZoneWontCatch.Merge(localBaitZoneWontCatch);
            fishStatsDataSet.BaitZoneMayCatch.Merge(localBaitZoneMayCatch);

            LoggingFunctions.Debug("FS::FSBaitLoadServerData: Loaded " + fishStatsDataSet.BaitFishWillCatch.Rows.Count + " rows into BaitFishWillCatch.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            LoggingFunctions.Debug("FS::FSBaitLoadServerData: Loaded " + fishStatsDataSet.BaitFishWontCatch.Rows.Count + " rows into BaitFishWontCatch.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            LoggingFunctions.Debug("FS::FSBaitLoadServerData: Loaded " + fishStatsDataSet.BaitFishMayCatch.Rows.Count + " rows into BaitFishMayCatch.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            LoggingFunctions.Debug("FS::FSBaitLoadServerData: Loaded " + fishStatsDataSet.BaitZoneWillCatch.Rows.Count + " rows into BaitZoneWillCatch.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            LoggingFunctions.Debug("FS::FSBaitLoadServerData: Loaded " + fishStatsDataSet.BaitZoneWontCatch.Rows.Count + " rows into BaitZoneWontCatch.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            LoggingFunctions.Debug("FS::FSBaitLoadServerData: Loaded " + fishStatsDataSet.BaitZoneMayCatch.Rows.Count + " rows into BaitZoneMayCatch.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
        }
        private void FSBaitLoadBait(String iBaitName)
        {
            if(FSBaitComboBox.Items.Contains(iBaitName))
            {
                FSBaitComboBox.SelectedItem = iBaitName;
                this.FishStatsTabControl.SelectedIndex = this.FishStatsTabControl.TabPages.IndexOfKey("FSBaitTab");
            }
        }
        #endregion Bait Tab Page Updates
        #endregion Page Updates
        #region Events
        #region Top Events
        private void StatsCloseButton_Click(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("FS::StatsCloseButton_Click: Disposing Stats Form.", LoggingFunctions.DBG_SCOPE.FISH_STAT);
            this.Dispose();
        }
        public void UpdateSettings()
        {
            StatsSettingsUpdateEventArgs args = new StatsSettingsUpdateEventArgs(
                                                    FSFishComboBox.SelectedIndex, FSZoneComboBox.SelectedIndex,
                                                    FSZoneRodComboBox.SelectedIndex, FSZoneBaitComboBox.SelectedIndex,
                                                    FSRodComboBox.SelectedIndex, FSBaitComboBox.SelectedIndex,
                                                    Statics.Settings.Top.MapsPath);
            if (SettingsChanged != null)
            {
                SettingsChanged(this, args);
            }
        }
        #endregion Top Events
        #region Fish Tab Events
        private void FSFishComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            FSFishLoadImage();
            FSFishLoadServerData();
            FSReloadLocalZoneCatchTable();
            FSReloadLocalRodResultsTable();
            Cursor.Current = Cursors.Default;
            if (!constructing)
            {
                UpdateSettings();
                //Thread.Sleep(1000);
            }
        }
        private void StatsFishZoneListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!doingListBoxInits)
            {
                FSReloadLocalRodResultsTable();
                FSReloadLocalZoneCatchTable();
            }
        }
        private void StatsFishBaitListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!doingListBoxInits)
            {
                updateBaitFilterList();
                FSReloadLocalZoneCatchTable();
                if (!constructing)
                {
                    UpdateSettings();
                }
            }
        }
        private void updateBaitFilterList()
        {
            int nbBaitListed = StatsFishBaitListBox.Items.Count;
            for (int ii = 0; ii < nbBaitListed; ii++)
            {
                if (StatsFishBaitListBox.GetItemChecked(ii))
                {
                    if (baitFilterList.Contains(StatsFishBaitListBox.GetItemText(StatsFishBaitListBox.Items[ii])))
                    {
                        baitFilterList.Remove(StatsFishBaitListBox.GetItemText(StatsFishBaitListBox.Items[ii]));
                    }
                }
                else
                {
                    if (!baitFilterList.Contains(StatsFishBaitListBox.GetItemText(StatsFishBaitListBox.Items[ii])))
                    {
                        baitFilterList.Add(StatsFishBaitListBox.GetItemText(StatsFishBaitListBox.Items[ii]));
                    }
                }
            }
        }
        private void StatsFishRodsListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            int itemCnt = StatsFishRodsListBox.Items.Count;
            rodFilterList.Clear();
            for (int ii = 0; ii < itemCnt; ii++)
            {
                if (!StatsFishRodsListBox.GetItemChecked(ii))
                {
                    rodFilterList.Add(StatsFishRodsListBox.GetItemText(StatsFishRodsListBox.Items[ii]));
                }
            }
            FSReloadLocalRodResultsTable();
            FSReloadLocalZoneCatchTable();
            if (!doingListBoxInits && !FSZoneTabDoingInits)
            {
                if (!constructing)
                {
                    UpdateSettings();
                }
            }
        }
        private void FSFishSetupDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            String fishName = (String)FSFishComboBox.SelectedItem;
            String zoneName = (String)FSFishSetupDGV[0, e.RowIndex].Value;
            String rodName = (String)FSFishSetupDGV[1, e.RowIndex].Value;
            String baitName = (String)FSFishSetupDGV[2, e.RowIndex].Value;
            switch (e.ColumnIndex)
            {
                case 0:
                    //Zone
                    FSZoneLoadSetup(zoneName, fishName, baitName, rodName);
                    break;
                case 1:
                    //Rod
                    FSRodsLoadRod(rodName, fishName);
                    break;
                case 2:
                    //Bait
                    FSBaitLoadBait(baitName);
                    break;
                default:
                    break;
            }
        }
        private void FSFishRodDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            String rodName = (String)FSFishRodDGV[0, e.RowIndex].Value;
            String fishName = (String)FSFishComboBox.SelectedItem;
            FSRodsLoadRod(rodName, fishName);
        }
        #endregion Fish Tab Events
        #region Zone Tab Events
        private void FSZoneComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!FSZoneTabDoingInits)
            {
                Cursor.Current = Cursors.WaitCursor;
                LoggingFunctions.Debug("FS::FSZoneComboBox_SelectedIndexChanged: Loading zone image of index: " + FSZoneComboBox.SelectedIndex + ".", LoggingFunctions.DBG_SCOPE.FISH_STAT);
                String zoneName = (String)FSZoneComboBox.SelectedItem;
                UInt16 zoneId = fullZoneNameToIdMap[zoneName];
                FSZoneLoadServerDataFiltered();
                FSZoneLoadServerData();
                FSZoneTabGraphReload();
                FSCurrentMapInCurrentZone = 1;
                FSZoneLoadMap(zoneId, false, true, "");
                if (!constructing)
                {
                    UpdateSettings();
                    //Thread.Sleep(1000);
                }
                Cursor.Current = Cursors.Default;
                //FSZoneFishUnFilteredDGV.ClearSelection();
            }
        }
        private void FSZoneLoadMap(UInt16 iZoneId, Boolean iClearMap, Boolean iAllFish, String iFishName)
        {
            List<Int16> posXList = new List<short>();
            List<Int16> posYList = new List<short>();
            if(!iClearMap)
            {
                String filter = "";
                if(!iAllFish)
                {
                    UInt16 fishId = Data.Client.Fish.GetFishInfo(iFishName).ItemID;
                    filter = "Fish=" + fishId;
                }
                FishStatsDataSet.PositionCatchRow[] posRows = (FishStatsDataSet.PositionCatchRow[])fishStatsDataSet.PositionCatch.Select(filter);
                for(int ii=0; ii<posRows.Length; ii++)
                {
                    posXList.Add(posRows[ii].PosX);
                    posYList.Add(posRows[ii].PosY);
                }
            }
            FSNbMapsInCurrentZone = Maps.GetNumberOfMapsByID(iZoneId);
            Maps.MapSet ms = null;
            if (FSNbMapsInCurrentZone == 1)
            {
                ms = Maps.GetMap(iZoneId);
            }
            else
            {
                ms = Maps.GetMap(iZoneId, (Byte)(FSCurrentMapInCurrentZone - 1));
            }
            Maps.MapOverlay.ItemType tp = iAllFish ? Maps.MapOverlay.ItemType.ELLIPSE_ALL_FISH : Maps.MapOverlay.ItemType.ELLIPSE_ONE_FISH;
            ms.AddDotPositions(tp, posXList, posYList);
            FSZonePictureBox.Image = ms.StaticMap;
            FSZoneMapNbLabel.Text = FSCurrentMapInCurrentZone.ToString() + " / " + FSNbMapsInCurrentZone.ToString();
        }
        private void FSZoneMapLeftButton_Click(object sender, EventArgs e)
        {
            if (FSCurrentMapInCurrentZone > 1)
            {
                String zoneName = (String)FSZoneComboBox.SelectedItem;
                UInt16 zoneId = fullZoneNameToIdMap[zoneName];
                FSCurrentMapInCurrentZone--;
                //FSNbMapsInCurrentZone = Maps.GetNumberOfMapsByID(zoneId);
                FSZoneLoadMap(zoneId, false, true, "");
            }
        }
        private void FSZoneMapRightButton_Click(object sender, EventArgs e)
        {
            if (FSCurrentMapInCurrentZone < FSNbMapsInCurrentZone)
            {
                String zoneName = (String)FSZoneComboBox.SelectedItem;
                UInt16 zoneId = fullZoneNameToIdMap[zoneName];
                FSCurrentMapInCurrentZone++;
                //FSNbMapsInCurrentZone = Maps.GetNumberOfMapsByID(zoneId);
                FSZoneLoadMap(zoneId, false, true, "");
            }
        }
        private void FSZoneFishUnFilteredDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            FSZoneLoadMapByFish(e.ColumnIndex, e.RowIndex);
        }
        private void FSZoneFishUnFilteredDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            String fishName = (String)FSZoneFishUnFilteredDGV[e.ColumnIndex, e.RowIndex].Value;
            FSFishLoadFish(fishName);
        }
        private void FSZoneRodComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!FSZoneTabDoingInits)
            {
                Cursor.Current = Cursors.WaitCursor;
                FSZoneLoadServerDataFiltered();
                FSZoneTabGraphReload();
                if (!constructing)
                {
                    UpdateSettings();
                    //Thread.Sleep(1000);
                }
                Cursor.Current = Cursors.Default;
            }
        }
        private void FSZoneBaitComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!FSZoneTabDoingInits)
            {
                Cursor.Current = Cursors.WaitCursor;
                FSZoneLoadServerDataFiltered();
                FSZoneTabGraphReload();
                if (!constructing)
                {
                    UpdateSettings();
                    //Thread.Sleep(1000);
                }
                Cursor.Current = Cursors.Default;
            }
        }

        #endregion Zone Tab Events
        #region Rods Tab Events
        private void FSRodComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            FSRodsLoadImage();
            FSRodsLoadServerData();
            if (!constructing)
            {
                UpdateSettings();
                //Thread.Sleep(1000);
            }
            Cursor.Current = Cursors.Default;
        }
        private void FSRodDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            String fishName = (String)FSRodDGV[0, e.RowIndex].Value;
            if(FSFishComboBox.Items.Contains(fishName))
            {
                FSFishLoadFish(fishName);
            }
        }
        #endregion Rods Tab Events
        #region Bait Tab Events
        private void FSBaitComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            FSBaitLoadImage();
            FSBaitLoadServerData();
            if (!constructing)
            {
                UpdateSettings();
                //Thread.Sleep(1000);
            }
            Cursor.Current = Cursors.Default;
        }
        private void FSBaitFishWillDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            String fishName = (String)FSBaitFishWillDGV[0, e.RowIndex].Value;
            if(FSFishComboBox.Items.Contains(fishName))
            {
                FSFishLoadFish(fishName);
            }
        }
        private void FSBaitZoneWillDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            String zoneName = (String)FSBaitZoneWillDGV[0, e.RowIndex].Value;
            if(FSZoneComboBox.Items.Contains(zoneName))
            {
                FSZoneComboBox.SelectedItem = zoneName;
                this.FishStatsTabControl.SelectedIndex = this.FishStatsTabControl.TabPages.IndexOfKey("FSZoneTab");
            }
        }
        #endregion Bait Tab Events
        #endregion Events
    }
    #region Event Args Class
    public class StatsSettingsUpdateEventArgs : System.EventArgs
    {
        private int evFishCBIndex;
        private int evZoneCBIndex;
        private int evZoneRodCBIndex;
        private int evZoneBaitCBIndex;
        private int evRodCBIndex;
        private int evBaitCBIndex;
        private String evMapsPath;

        public StatsSettingsUpdateEventArgs(
            int FishCBIndex,
            int ZoneCBIndex,
            int ZoneRodCBIndex,
            int ZoneBaitCBIndex,
            int RodCBIndex,
            int BaitCBIndex,
            String MapsPath
            )
        {
            evFishCBIndex = FishCBIndex;
            evZoneCBIndex = ZoneCBIndex;
            evZoneRodCBIndex = ZoneRodCBIndex;
            evZoneBaitCBIndex = ZoneBaitCBIndex;
            evRodCBIndex = RodCBIndex;
            evBaitCBIndex = BaitCBIndex;
            evMapsPath = MapsPath;
        }
        public int FishCBIndex
        {
            get
            {
                return evFishCBIndex;
            }
        }
        public int ZoneCBIndex
        {
            get
            {
                return evZoneCBIndex;
            }
        }
        public int ZoneRodCBIndex
        {
            get
            {
                return evZoneRodCBIndex;
            }
        }
        public int ZoneBaitCBIndex
        {
            get
            {
                return evZoneBaitCBIndex;
            }
        }
        public int RodCBIndex
        {
            get
            {
                return evRodCBIndex;
            }
        }
        public int BaitCBIndex
        {
            get
            {
                return evBaitCBIndex;
            }
        }
        public String MapsPath
        {
            get
            {
                return evMapsPath;
            }
        }
    }
    #endregion Event Args Class
}
