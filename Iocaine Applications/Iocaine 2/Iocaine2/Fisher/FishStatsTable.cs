using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Bots
{
    public sealed partial class Fisher : Bot
    {
        #region Fishing Dataset
        private partial class FishStatsDataSet : System.Data.DataSet
        {
            #region Notes
            /*
            All Tables:
            1. DONE: Bait - Imported from resource. Not cached to file.
            2. NA:   Day - Probably not required. Just a byte taken from the vana-time.day % 8. Mod 8 prevents bad values.
            3. FishIDs      - Keeps 3 tables of this type. 1 for local cache, 1 for server data, and 1 to hold the local selection (from local or server).
                            - Keep a cache file (common to all characters) with all ID's for every rod/zone the player fishes with/in.
                                - This means that when we're connected, every time the fisher starts we need to merge the 
                                  server-selected ID's with what we load in our local table.
                            - If mode=server, load both tables and select from server.
                            - If mode=local, load local table only and use it to select from.
                            - Like fish stats, we need a metadata column to indicate if we've saved to the server or not.
                            - Every time we start (instance init) we'll load the local table and check if it has unsaved rows.
                                - If so and we're online, push them and mark them as saved.
                            === Maybe for the local table, we inherit the server-type table and add 1 more column to it. We'll never pass this table to our SQL functions.
            4. FishStats    - Locally created table.
                            - Use server/local.
                            - On init, if server connected, select from server. Else, load local copy.
                            - Save server/local.
                            - If server connected save there, else save local.
                            - Clean up local?
                            - Keep a file per JP day. When we save to the server, clean up everything before the current day.
                            - Keeping the current day saved locally even if it's been saved to the server means
                              that we need to keep extra metadata (a Boolean column) whether it's been saved or not.
            5. DONE: Fish - Imported from resource. Not cached to file.
            6. NA:   Moon - Moon phase table would only be used to convert from percent to phase name. Not used in the actual database.
            7. NA:   Player - This obviously can't be obtained if we start off in local mode.
                        - This should really just be filled in on the fly when the stats are saved to the server.
                            - That way the player field can be hidden in the "server" dll when we move stuff there.
                            - When we switch between local/unk => server, this value will be set in the dll (or locally for now).
                        - When we're in local mode, just leave the field blank (empty string).
                        - To keep track of who the records belong to, the file with the records will keep
                          the player name and server ID in the file name.
            8. DONE: Result - It's only required for constraint purposes. The value saved is from ffxienum.
            9. DONE: Rods - Imported from resource. Not cached to file.
            10. DONE: Server - It's required to convert from server name (read from memory) to ID. The ID is required to get the character ID.
                            - This should probably be done elsewhere, in the resources dll.
                            - Then it is not used here, it is used in the server/security dll.
            11. DONE: ZoneNames - Merged into ZoneTable below.
            12. DONE: ZoneTable - Imported from resource. Not cached to file.

            === Local Mode Flag ===
            - On init, check server connection on a background thread.
                - Launch the thread where the tables are currently being loaded???
                - Move all of the table loads and stats box update to this thread.
                - Keep a server/local/unk enum flag that gets set in this thread.
                    - This flag will prevent the fisher from starting if it is 'unk'.
            - If flag=server, do nothing further.
            - If flag=local, launch a thread to check for connection every 5 minutes.
                - Upon connection, change flag to 'server'.
            - When saving the fish stats, catch exceptions and change flag=local.
                - Launch the thread mentioned above.
            */
            #endregion Notes

            #region Private Members
            private MainDatabase.BaitDataTable baitTable = null;
            private MainDatabase.FishDataTable fishTable = null;
            private MainDatabase.RodsDataTable rodsTable = null;
            private MainDatabase.ZonesDataTable zoneTable = null;
            private FishStatsTable fishStatsServerTable = null;
            private FishStatsLocalTable fishStatsLocalTable = null;
            private FishStatsLocalTable fishStatsSelectTable = null;
            private ResultTable resultTable = null;
            private FishIDsTable fishIDsServerTable = null;
            private FishIDsLocalTable fishIDsLocalTable = null;
            private FishIDsLocalTable fishIDsSelectTable = null;
            private Statics.Enums.ONLINE_MODE onlineStatus = Statics.Enums.ONLINE_MODE.UNKNOWN;
            private bool initIocaineDone = false;
            private bool fishIdsPushed = false;
            private List<string> fishStatsPushed = new List<string>();
            private string player = "";
            private string server = "";
            private byte serverId = 0;
            private static FishStatsDataSet instance = new FishStatsDataSet();
            private static object padlock = new object();
            #endregion Private Members

            #region Public Properties
            public static FishStatsDataSet Access
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new FishStatsDataSet();
                    }
                    return instance;
                }
            }
            #endregion Public Properties

            #region Constructors
            private FishStatsDataSet()
            {
                Init_Iocaine();
            }
            #endregion Constructors

            #region Inits
            private void Init_Iocaine()
            {
                this.DataSetName = "FishStatsDataSet";
                this.Prefix = "";
                this.Namespace = "http://tempuri.org/FishStatsDataSet.xsd";
                this.EnforceConstraints = true;
                this.SchemaSerializationMode = global::System.Data.SchemaSerializationMode.IncludeSchema;

                baitTable = Bait.GetBaitTableCopy();
                base.Tables.Add(baitTable);
                fishTable = Fish.GetFishTableCopy();
                base.Tables.Add(fishTable);
                rodsTable = Rods.GetRodsTableCopy();
                base.Tables.Add(rodsTable);
                zoneTable = Zones.GetZoneTableCopy();
                base.Tables.Add(zoneTable);

                this.fishStatsServerTable = new FishStatsTable();
                base.Tables.Add(this.fishStatsServerTable);
                this.fishStatsLocalTable = new FishStatsLocalTable();
                base.Tables.Add(this.fishStatsLocalTable);
                this.fishStatsSelectTable = new FishStatsLocalTable();
                base.Tables.Add(this.fishStatsSelectTable);
                this.resultTable = new ResultTable();
                base.Tables.Add(this.resultTable);
                this.resultTable.loadData();

                this.fishIDsServerTable = new FishIDsTable();
                base.Tables.Add(this.fishIDsServerTable);
                this.fishIDsLocalTable = new FishIDsLocalTable();
                base.Tables.Add(this.fishIDsLocalTable);
                this.fishIDsSelectTable = new FishIDsLocalTable();
                base.Tables.Add(this.fishIDsSelectTable);

                // Load the local fish IDs table (common to all characters) and push to the server if we're online and something has not been pushed.
                SetOnlineStatus(Server.OnlineStatus);
                fishIDsLocalTable.Load();
                cleanupLocal();
                initIocaineDone = true;
            }
            public void Init_LoggedIn()
            {
                // This will reset the player/server values.
                player = PlayerCache.Vitals.Name;
                server = PlayerCache.Environment.ServerName;
                serverId = PlayerCache.Environment.ServerId;
                // It will:
                // 0. Save and clear any remaining stats and IDs.
                clearStats();
                clearIds();
                // 2. Fill server stats.
                //  We want the stats to be loaded into the server table already so we just merge the
                //  local stats with them. This way if there is an error saving later, we will still
                //  write them all locally when done.
                //fishStatsServerTable.Load();
                // 3. Purge/push stats for this character.
                cleanupLocal();
                // 4. Fill selected stats table.
                //SelectFishStats();
            }
            #endregion Inits

            #region Public Methods
            public void SetOnlineStatus(Statics.Enums.ONLINE_MODE iMode)
            {
                onlineStatus = iMode;
                this.fishStatsServerTable.OnlineStatus = iMode;
                this.fishStatsLocalTable.OnlineStatus = iMode;
                this.fishIDsServerTable.OnlineStatus = iMode;
                this.fishIDsLocalTable.OnlineStatus = iMode;
            }
            #region Fish IDs
            public List<FishIDsLocalRow> SelectFishIDs(ushort iRodId, ushort iZoneId)
            {
                List<FishIDsLocalRow> retVal = null;
                Monitor.Enter(Tables.SyncRoot);
                string select = "Rod = " + iRodId + " AND Zone = " + iZoneId;
                try
                {
                    fishIDsSelectTable.Rows.Clear();
                    if (onlineStatus == Statics.Enums.ONLINE_MODE.OFFLINE)
                    {
                        FishIDsLocalRow[] rows = (FishIDsLocalRow[])fishIDsLocalTable.Select(select);
                        foreach (FishIDsLocalRow row in rows)
                        {
                            FishIDsLocalRow selRow = fishIDsSelectTable.NewFishIDsLocalRow();
                            fishIDsSelectTable.copyRow(row, ref selRow);
                            selRow.Synced = row.Synced;
                            selRow.Catch = row.Catch;
                            fishIDsSelectTable.AddFishIDsLocalRow(selRow);
                        }
                    }
                    else
                    {
                        bool success = Server.SQL.FISHINGDATABASE.FillFishIDs(fishIDsServerTable, iRodId, iZoneId);
                        fishIDsSelectTable.MergeToLocal(fishIDsServerTable);
                        fishIDsServerTable.AcceptChanges();
                        fishIDsSelectTable.AcceptChanges();
                    }
                    FishIDsLocalRow[] selRows = (FishIDsLocalRow[])fishIDsSelectTable.Select();
                    retVal = selRows.ToList();
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("FishStatsDataset::SelectFishIDs: " + e.ToString());
                    if (retVal == null)
                    {
                        retVal = new List<FishIDsLocalRow>();
                    }
                }
                finally
                {
                    Monitor.Exit(Tables.SyncRoot);
                }
                return retVal;
            }
            public void MergeFishIDsToLocal()
            {
                // Merge records from the server to the local cache and save if new entries are created.
                Monitor.Enter(Tables.SyncRoot);
                try
                {
                    if (fishIDsLocalTable.MergeToLocal(fishIDsSelectTable))
                    {
                        fishIDsLocalTable.Save();
                    }
                    fishIDsLocalTable.AcceptChanges();
                    fishIDsSelectTable.AcceptChanges();
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("FishStatsDataset::MergeFishIDsToLocal: " + e.ToString());
                }
                finally
                {
                    Monitor.Exit(Tables.SyncRoot);
                }
            }
            public FishIDsLocalRow NewFishIDsRow()
            {
                return fishIDsSelectTable.NewFishIDsLocalRow();
            }
            public void AddFishIDs(FishIDsLocalRow iRow)
            {
                // Paramter row is from the selection table.
                // Add to the select table no matter what.
                // Add to the local table no matter what.
                // Add to the server table if we're online.
                Monitor.Enter(Tables.SyncRoot);
                try
                {
                    FishIDsLocalRow lclRow = fishIDsLocalTable.NewFishIDsLocalRow();
                    FishIDsRow srvRow = fishIDsServerTable.NewFishIDsRow();
                    fishIDsLocalTable.copyRow(iRow, ref lclRow);
                    fishIDsServerTable.copyRow(iRow, ref srvRow);
                    if (onlineStatus != Statics.Enums.ONLINE_MODE.OFFLINE)
                    {
                        fishIDsServerTable.AddFishIDsRow(srvRow);
                        lclRow.Synced = true;
                        iRow.Synced = true;
                    }
                    else
                    {
                        lclRow.Synced = false;
                        iRow.Synced = false;
                    }
                    fishIDsLocalTable.AddFishIDsLocalRow(lclRow);
                    fishIDsSelectTable.AddFishIDsLocalRow(iRow);
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("FishStatsDataset::AddFishIDs: " + e.ToString());
                }
                finally
                {
                    Monitor.Exit(Tables.SyncRoot);
                }
            }
            public void SaveFishIDs(out bool oRefillTable)
            {
                oRefillTable = false;
                Monitor.Enter(Tables.SyncRoot);
                try
                {
                    if (onlineStatus != Statics.Enums.ONLINE_MODE.OFFLINE)
                    {
                        //Save server data.
                        // We'll be handling the offline mode stuff all in the server dll.
                        // If the server dll determines we are offline, it will pop it's own event.
                        // This event will set the fisher online status (from the same thread).
                        // So by the time we get to a return value of the save command here,
                        // the online/offline status should be set to offline.
                        // If that's the case we'll get a bad return value (or an exception?)
                        // and save it locally, change the synced column in the last local table row to false, etc.
                        if (!fishIDsServerTable.Save(out oRefillTable))
                        {
                            // Go through every record in the server table who's state is not 'unchanged'
                            // and find the corresponding records in the local table.
                            // Mark that row's synced column as false.
                            string filter = "";
                            foreach (FishIDsRow row in fishIDsServerTable.Rows)
                            {
                                if (row.RowState != DataRowState.Unchanged)
                                {
                                    filter = fishIDsServerTable.FilterGen.GetFilter(row);
                                    FishIDsLocalRow[] lclRows = (FishIDsLocalRow[])fishIDsLocalTable.Select(filter);
                                    if (lclRows.Length > 0)
                                    {
                                        if (lclRows[0].Synced == true)
                                        {
                                            //Don't modify it unless it's required.
                                            lclRows[0].Synced = false;
                                        }
                                    }
                                    else
                                    {
                                        LoggingFunctions.Warning("FishStatsDataSet::SaveFishIDs: Could not find local row with filter \"" + filter + "\"");
                                    }
                                }
                            }
                            fishIDsLocalTable.Save();
                        }
                    }
                    else
                    {
                        fishIDsLocalTable.Save();
                    }
                    fishIDsSelectTable.AcceptChanges();
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("FishStatsDataset::SaveFishIDs: " + e.ToString());
                }
                finally
                {
                    Monitor.Exit(Tables.SyncRoot);
                }
            }
            public void UpdateFishIDs(FishIDsLocalRow iRow)
            {
                // The argument row is from the selection table.
                // So we need to find that row in the other table(s) and update them.
                FishIDsLocalTable tbl = (FishIDsLocalTable)iRow.Table;
                List<DataColumn> modCols = null;
                Monitor.Enter(Tables.SyncRoot);
                try
                {
                    string filter = tbl.FilterGen.GetFilter(iRow, out modCols);

                    if (onlineStatus != Statics.Enums.ONLINE_MODE.OFFLINE)
                    {
                        FishIDsRow[] serverRows = (FishIDsRow[])fishIDsServerTable.Select(filter);
                        foreach (FishIDsRow row in serverRows)
                        {
                            foreach (DataColumn col in modCols)
                            {
                                row[col.ColumnName] = iRow[col.ColumnName];
                            }
                        }
                    }

                    FishIDsLocalRow[] localRows = (FishIDsLocalRow[])fishIDsLocalTable.Select(filter);
                    foreach (FishIDsLocalRow row in localRows)
                    {
                        foreach (DataColumn col in modCols)
                        {
                            row[col.ColumnName] = iRow[col.ColumnName];
                        }
                    }
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("FishStatsDataset::UpdateFishIDs: " + e.ToString());
                }
                finally
                {
                    Monitor.Exit(Tables.SyncRoot);
                }
            }
            #endregion Fish IDs
            #region Fish Stats
            public List<FishStatsLocalRow> SelectFishStats()
            {
                List<FishStatsLocalRow> retVal = null;
                Monitor.Enter(Tables.SyncRoot);
                try
                {
                    if (fishStatsServerTable.Rows.Count > 0)
                    {
                        fishStatsServerTable.Clear();
                    }
                    string filter = "Date >= '" + FishStatsLocalTable.getLastJpMidnightUTC().ToString() + "'";

                    if (fishStatsLocalTable.Rows.Count == 0)
                    {
                        fishStatsLocalTable.Load(player, server);
                    }
                    if (onlineStatus != Statics.Enums.ONLINE_MODE.OFFLINE)
                    {
                        Server.SQL.FISHINGDATABASE.FillFishStats(fishStatsServerTable);
                        FishStatsRow[] serverRows = (FishStatsRow[])fishStatsServerTable.Select(filter);
                        fishStatsSelectTable.MergeFromServer(serverRows);
                        fishStatsLocalTable.MergeFromServer(serverRows);
                    }
                    if (onlineStatus == Statics.Enums.ONLINE_MODE.OFFLINE)
                    {
                        //We'll still catch this if we were online before and failed to get the stats above.
                        FishStatsLocalRow[] localRows = (FishStatsLocalRow[])fishStatsLocalTable.Select(filter);
                        fishStatsSelectTable.MergeFromLocal(localRows);
                    }
                    retVal = ((FishStatsLocalRow[])fishStatsSelectTable.Select()).ToList();
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("FishStatsDataset::SelectFishStats: " + e.ToString());
                    if (retVal == null)
                    {
                        retVal = new List<FishStatsLocalRow>();
                    }
                }
                finally
                {
                    Monitor.Exit(Tables.SyncRoot);
                }
                return retVal;
            }
            public FishStatsLocalRow NewFishStatsRow()
            {
                return fishStatsLocalTable.NewFishStatsLocalRow();
            }
            public void AddFishStats(FishStatsLocalRow iRow)
            {
                // * Passed in row is from the local table *
                // Add to the select table no matter what.
                // Add to the local table no matter what.
                // Add to the server table if we're online.
                FishStatsLocalRow selRow = fishStatsSelectTable.NewFishStatsLocalRow();
                FishStatsRow srvRow = fishStatsServerTable.NewFishStatsTableRow();
                Monitor.Enter(Tables.SyncRoot);
                try
                {
                    fishStatsSelectTable.copyRow(iRow, ref selRow);
                    fishStatsServerTable.copyRow(iRow, ref srvRow);
                    if (onlineStatus != Statics.Enums.ONLINE_MODE.OFFLINE)
                    {
                        selRow.Synced = true;
                        iRow.Synced = true;
                    }
                    else
                    {
                        selRow.Synced = false;
                        iRow.Synced = false;
                    }
                    fishStatsServerTable.AddFishStatsTableRow(srvRow);
                    fishStatsSelectTable.AddFishStatsLocalRow(selRow);
                    fishStatsLocalTable.AddFishStatsLocalRow(iRow);
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("FishStatsDataset::AddFishStats: " + e.ToString());
                }
                finally
                {
                    Monitor.Exit(Tables.SyncRoot);
                }
            }
            public void SaveFishStats()
            {
                Monitor.Enter(Tables.SyncRoot);
                try
                {
                    if (onlineStatus != Statics.Enums.ONLINE_MODE.OFFLINE)
                    {
                        //Save server data.
                        // We'll be handling the offline mode stuff all in the server dll.
                        // If the server dll determines we are offline, it will pop it's own event.
                        // This event will set the fisher online status (from the same thread).
                        // So by the time we get to a return value of the save command here,
                        // the online/offline status should be set to offline.
                        // If that's the case we'll get a bad return value (or an exception?)
                        // and save it locally, change the synced column in the last local table row to false, etc.
                        if (!fishStatsServerTable.Save())
                        {
                            // Go through every record in the server table who's state is not 'unchanged'
                            // and find the corresponding records in the local table.
                            // Mark that row's synced column as false.
                            string filter = "";
                            foreach (FishStatsRow row in fishStatsServerTable.Rows)
                            {
                                if (row.RowState != DataRowState.Unchanged)
                                {
                                    filter = "Date='" + row.Date.ToString() + "'";
                                    FishStatsLocalRow[] lclRows = (FishStatsLocalRow[])fishStatsLocalTable.Select(filter);
                                    if (lclRows.Length > 0)
                                    {
                                        if (lclRows[0].Synced == true)
                                        {
                                            //Don't modify it unless it's required.
                                            lclRows[0].Synced = false;
                                        }
                                    }
                                    else
                                    {
                                        LoggingFunctions.Warning("FishStatsDataSet::SaveFishStats: Could not find local row with filter \"" + filter + "\"");
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Everything was successfully saved to the server. If there are any local rows marked unsynced, set synced=true
                            string filter = "Synced='false'";
                            FishStatsLocalRow[] lclRows = (FishStatsLocalRow[])fishStatsLocalTable.Select(filter);
                            foreach (FishStatsLocalRow row in lclRows)
                            {
                                row.Synced = true;
                            }
                        }
                    }
                    fishStatsLocalTable.Save(player, server);
                    fishStatsLocalTable.AcceptChanges();
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("FishStatsDataset::SaveFishStats: " + e.ToString());
                }
                finally
                {
                    Monitor.Exit(Tables.SyncRoot);
                }
            }
            #endregion Fish Stats
            #endregion Public Methods

            #region Private Methods
            private void clearStats()
            {
                if (fishStatsServerTable.Rows.Count > 0)
                {
                    fishStatsServerTable.Save();
                    fishStatsServerTable.Clear();
                }
                if (fishStatsLocalTable.Rows.Count > 0)
                {
                    fishStatsLocalTable.Save();
                    fishStatsLocalTable.Clear();
                }
                fishStatsSelectTable.Clear();
            }
            private void clearIds()
            {
                if (fishIDsServerTable.Rows.Count > 0)
                {
                    bool dummy;
                    fishIDsServerTable.Save(out dummy);
                    fishIDsServerTable.Clear();
                }
                fishIDsSelectTable.Clear();
            }
            private void cleanupLocal()
            {
                //We do this on first init (for fish IDs) and on every login for fish stats.
                //We can't do fish stats on first login because we need the player to be logged in
                //for the player column to be filled in with the encrypted version of the current character.
                if (!fishIdsPushed)
                {
                    //Clean up locally saved fish IDs.
                    if (fishIDsLocalTable.HasUnsavedRows() && (onlineStatus != Statics.Enums.ONLINE_MODE.OFFLINE))
                    {
                        //We use the server table as a temporary buffer while they're pushed to the server.
                        fishIDsServerTable.Clear();
                        for (int ii = 0; ii < fishIDsLocalTable.Rows.Count; ii++)
                        {
                            if (fishIDsLocalTable[ii].Synced == false)
                            {
                                fishIDsServerTable.AddFishIDsLocalRow(fishIDsLocalTable[ii]);
                            }
                        }
                        if (fishIDsServerTable.Rows.Count > 0)
                        {
                            //If the save is sucessful, mark all local rows as synced.
                            //If not, go through each server row and check the state.
                            //If the state is 'unchanged' we can assume it was saved. Find that record in the local table and mark it as synced.
                            // Since we're just using the server table as a temp buffer, we can ignore the "refill table" output.
                            bool dummy;
                            if (fishIDsServerTable.Save(out dummy))
                            {
                                for (int ii = 0; ii < fishIDsLocalTable.Rows.Count; ii++)
                                {
                                    fishIDsLocalTable[ii].Synced = true;
                                }
                            }
                            else
                            {
                                for (int ii = 0; ii < fishIDsServerTable.Rows.Count; ii++)
                                {
                                    FishIDsRow row = fishIDsServerTable[ii];
                                    if (row.RowState == DataRowState.Unchanged)
                                    {
                                        string filter = "Fish=" + row.Fish + " AND ID1=" + row.ID1;
                                        filter += " AND ID2=" + row.ID2 + " AND ID3=" + row.ID3;
                                        filter += " AND Large=" + row.Large + " AND Rod=" + row.Rod;
                                        filter += " AND Zone=" + row.Zone;
                                        FishIDsLocalRow[] localRows = (FishIDsLocalRow[])fishIDsLocalTable.Select(filter);
                                        if (localRows.Length != 1)
                                        {
                                            LoggingFunctions.Error("FishStatsDataSet::cleanupLocal: fishIDsLocalTable.Select(" + filter + ") returned " + localRows.Length + " rows.");
                                            continue;
                                        }
                                        localRows[0].Synced = true;
                                    }
                                }
                            }
                            fishIDsLocalTable.Save();
                        }
                        fishIDsServerTable.Clear();
                        fishIdsPushed = true;
                    }
                }

                if (!initIocaineDone || (player == ""))
                {
                    return;
                }
                fishStatsLocalTable.Push(player, server, fishStatsServerTable);
            }
            #endregion Private Methods

            #region Data Tables/Rows
            #region Fish Stats Table
            #region Data Row
            internal partial class FishStatsRow : DataRow
            {
                #region Private Members
                private FishStatsTable tableFishStatsTable;
                #endregion Private Members

                #region Public Properties
                #endregion Public Properties

                #region Constructor
                internal FishStatsRow(DataRowBuilder rb) :
                        base(rb)
                {
                    this.tableFishStatsTable = ((FishStatsTable)(this.Table));
                }
                #endregion Constructor

                #region Columns
                public byte Result
                {
                    get
                    {
                        return ((byte)(this[this.tableFishStatsTable.ResultColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.ResultColumn] = value;
                    }
                }
                public int Player
                {
                    get
                    {
                        return ((int)(this[this.tableFishStatsTable.PlayerColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.PlayerColumn] = value;
                    }
                }
                public byte Day
                {
                    get
                    {
                        return ((byte)(this[this.tableFishStatsTable.DayColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.DayColumn] = value;
                    }
                }
                public short Time
                {
                    get
                    {
                        return ((short)(this[this.tableFishStatsTable.TimeColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.TimeColumn] = value;
                    }
                }
                public byte Moon
                {
                    get
                    {
                        return ((byte)(this[this.tableFishStatsTable.MoonColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.MoonColumn] = value;
                    }
                }
                public byte Weather
                {
                    get
                    {
                        return ((byte)(this[this.tableFishStatsTable.WeatherColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.WeatherColumn] = value;
                    }
                }
                public byte Skill
                {
                    get
                    {
                        return ((byte)(this[this.tableFishStatsTable.SkillColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.SkillColumn] = value;
                    }
                }
                public byte Fatigue
                {
                    get
                    {
                        return ((byte)(this[this.tableFishStatsTable.FatigueColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.FatigueColumn] = value;
                    }
                }
                public short FishHP
                {
                    get
                    {
                        return ((short)(this[this.tableFishStatsTable.FishHPColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.FishHPColumn] = value;
                    }
                }
                public Int64 Record
                {
                    get
                    {
                        return ((Int64)(this[this.tableFishStatsTable.RecordColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.RecordColumn] = value;
                    }
                }
                public short Zone
                {
                    get
                    {
                        return ((short)(this[this.tableFishStatsTable.ZoneColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.ZoneColumn] = value;
                    }
                }
                public System.DateTime Date
                {
                    get
                    {
                        return ((System.DateTime)(this[this.tableFishStatsTable.DateColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.DateColumn] = value;
                    }
                }
                public short Fish
                {
                    get
                    {
                        return ((short)(this[this.tableFishStatsTable.FishColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.FishColumn] = value;
                    }
                }
                public short Rod
                {
                    get
                    {
                        return ((short)(this[this.tableFishStatsTable.RodColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.RodColumn] = value;
                    }
                }
                public short Bait
                {
                    get
                    {
                        return ((short)(this[this.tableFishStatsTable.BaitColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.BaitColumn] = value;
                    }
                }
                public int Info
                {
                    get
                    {
                        return ((int)(this[this.tableFishStatsTable.InfoColumn]));
                    }
                    set
                    {
                        this[this.tableFishStatsTable.InfoColumn] = value;
                    }
                }
                public short XPos
                {
                    get
                    {
                        try
                        {
                            return ((short)(this[this.tableFishStatsTable.XPosColumn]));
                        }
                        catch (System.InvalidCastException e)
                        {
                            throw new System.Data.StrongTypingException("The value for column \'XPos\' in table \'FishStatsTable\' is DBNull.", e);
                        }
                    }
                    set
                    {
                        this[this.tableFishStatsTable.XPosColumn] = value;
                    }
                }
                public short YPos
                {
                    get
                    {
                        try
                        {
                            return ((short)(this[this.tableFishStatsTable.YPosColumn]));
                        }
                        catch (System.InvalidCastException e)
                        {
                            throw new System.Data.StrongTypingException("The value for column \'YPos\' in table \'FishStatsTable\' is DBNull.", e);
                        }
                    }
                    set
                    {
                        this[this.tableFishStatsTable.YPosColumn] = value;
                    }
                }
                public int Version
                {
                    get
                    {
                        try
                        {
                            return ((int)(this[this.tableFishStatsTable.VersionColumn]));
                        }
                        catch (System.InvalidCastException e)
                        {
                            throw new System.Data.StrongTypingException("The value for column \'Version\' in table \'FishStatsTable\' is DBNull.", e);
                        }
                    }
                    set
                    {
                        this[this.tableFishStatsTable.VersionColumn] = value;
                    }
                }
                #endregion Columns

                #region Null Checks
                public bool IsXPosNull()
                {
                    return this.IsNull(this.tableFishStatsTable.XPosColumn);
                }
                public void SetXPosNull()
                {
                    this[this.tableFishStatsTable.XPosColumn] = System.Convert.DBNull;
                }
                public bool IsYPosNull()
                {
                    return this.IsNull(this.tableFishStatsTable.YPosColumn);
                }
                public void SetYPosNull()
                {
                    this[this.tableFishStatsTable.YPosColumn] = System.Convert.DBNull;
                }
                public bool IsVersionNull()
                {
                    return this.IsNull(this.tableFishStatsTable.VersionColumn);
                }
                public void SetVersionNull()
                {
                    this[this.tableFishStatsTable.VersionColumn] = System.Convert.DBNull;
                }
                #endregion Null Checks
            }
            #endregion Data Row
            #region Data Table
            [System.Serializable()]
            [System.Xml.Serialization.XmlSchemaProviderAttribute("GetTypedTableSchema")]
            internal class FishStatsTable : System.Data.TypedTableBase<FishStatsRow>
            {
                #region Private Members
                private Statics.Enums.ONLINE_MODE onlineStatus = Statics.Enums.ONLINE_MODE.UNKNOWN;
                private FilterGenerator filterGen;
                private DataColumn columnResult;
                private DataColumn columnPlayer;
                private DataColumn columnDay;
                private DataColumn columnTime;
                private DataColumn columnMoon;
                private DataColumn columnWeather;
                private DataColumn columnSkill;
                private DataColumn columnFatigue;
                private DataColumn columnFishHP;
                private DataColumn columnRecord;
                private DataColumn columnZone;
                private DataColumn columnDate;
                private DataColumn columnFish;
                private DataColumn columnRod;
                private DataColumn columnBait;
                private DataColumn columnInfo;
                private DataColumn columnXPos;
                private DataColumn columnYPos;
                private DataColumn columnVersion;
                #endregion Private Members

                #region Public Properties
                public Statics.Enums.ONLINE_MODE OnlineStatus
                {
                    get
                    {
                        return onlineStatus;
                    }
                    set
                    {
                        onlineStatus = value;
                    }
                }
                public FilterGenerator FilterGen
                {
                    get
                    {
                        if (filterGen == null)
                        {
                            filterGen = new FilterGenerator();
                        }
                        return filterGen;
                    }
                }
                public DataColumn ResultColumn
                {
                    get
                    {
                        return this.columnResult;
                    }
                }
                public DataColumn PlayerColumn
                {
                    get
                    {
                        return this.columnPlayer;
                    }
                }
                public DataColumn DayColumn
                {
                    get
                    {
                        return this.columnDay;
                    }
                }
                public DataColumn TimeColumn
                {
                    get
                    {
                        return this.columnTime;
                    }
                }
                public DataColumn MoonColumn
                {
                    get
                    {
                        return this.columnMoon;
                    }
                }
                public DataColumn WeatherColumn
                {
                    get
                    {
                        return this.columnWeather;
                    }
                }
                public DataColumn SkillColumn
                {
                    get
                    {
                        return this.columnSkill;
                    }
                }
                public DataColumn FatigueColumn
                {
                    get
                    {
                        return this.columnFatigue;
                    }
                }
                public DataColumn FishHPColumn
                {
                    get
                    {
                        return this.columnFishHP;
                    }
                }
                public DataColumn RecordColumn
                {
                    get
                    {
                        return this.columnRecord;
                    }
                }
                public DataColumn ZoneColumn
                {
                    get
                    {
                        return this.columnZone;
                    }
                }
                public DataColumn DateColumn
                {
                    get
                    {
                        return this.columnDate;
                    }
                }
                public DataColumn FishColumn
                {
                    get
                    {
                        return this.columnFish;
                    }
                }
                public DataColumn RodColumn
                {
                    get
                    {
                        return this.columnRod;
                    }
                }
                public DataColumn BaitColumn
                {
                    get
                    {
                        return this.columnBait;
                    }
                }
                public DataColumn InfoColumn
                {
                    get
                    {
                        return this.columnInfo;
                    }
                }
                public DataColumn XPosColumn
                {
                    get
                    {
                        return this.columnXPos;
                    }
                }
                public DataColumn YPosColumn
                {
                    get
                    {
                        return this.columnYPos;
                    }
                }
                public DataColumn VersionColumn
                {
                    get
                    {
                        return this.columnVersion;
                    }
                }
                [System.ComponentModel.Browsable(false)]
                public int Count
                {
                    get
                    {
                        return this.Rows.Count;
                    }
                }

                public FishStatsRow this[int index]
                {
                    get
                    {
                        return ((FishStatsRow)(this.Rows[index]));
                    }
                }
                #endregion Public Properties

                #region Constructors
                public FishStatsTable()
                {
                    this.TableName = "FishStatsTable";
                    this.BeginInit();
                    this.InitClass();
                    this.EndInit();
                }

                public FishStatsTable(System.Runtime.Serialization.SerializationInfo iInfo, System.Runtime.Serialization.StreamingContext iContext) :
                        base(iInfo, iContext)
                {
                    this.InitVars();
                }
                #endregion Constructors

                #region Events/Delegates
                public class FishStatsTableRowChangeEvent : System.EventArgs
                {
                    #region Private Members
                    private FishStatsRow eventRow;
                    private DataRowAction eventAction;
                    #endregion Private Members
                    #region Public Properties
                    public FishStatsRow Row
                    {
                        get
                        {
                            return this.eventRow;
                        }
                    }
                    public DataRowAction Action
                    {
                        get
                        {
                            return this.eventAction;
                        }
                    }
                    #endregion Public Properties
                    #region Constructor
                    public FishStatsTableRowChangeEvent(FishStatsRow iRow, DataRowAction iAction)
                    {
                        this.eventRow = iRow;
                        this.eventAction = iAction;
                    }
                    #endregion Constructor
                }

                public delegate void FishStatsTableRowChangeEventHandler(object sender, FishStatsTableRowChangeEvent e);
                public event FishStatsTableRowChangeEventHandler FishStatsTableRowChanging;
                public event FishStatsTableRowChangeEventHandler FishStatsTableRowChanged;
                public event FishStatsTableRowChangeEventHandler FishStatsTableRowDeleting;
                public event FishStatsTableRowChangeEventHandler FishStatsTableRowDeleted;
                #region Event Handlers
                protected override void OnRowChanged(System.Data.DataRowChangeEventArgs e)
                {
                    base.OnRowChanged(e);
                    if ((this.FishStatsTableRowChanged != null))
                    {
                        this.FishStatsTableRowChanged(this, new FishStatsTableRowChangeEvent(((FishStatsRow)(e.Row)), e.Action));
                    }
                }
                protected override void OnRowChanging(System.Data.DataRowChangeEventArgs e)
                {
                    base.OnRowChanging(e);
                    if ((this.FishStatsTableRowChanging != null))
                    {
                        this.FishStatsTableRowChanging(this, new FishStatsTableRowChangeEvent(((FishStatsRow)(e.Row)), e.Action));
                    }
                }
                protected override void OnRowDeleted(System.Data.DataRowChangeEventArgs e)
                {
                    base.OnRowDeleted(e);
                    if ((this.FishStatsTableRowDeleted != null))
                    {
                        this.FishStatsTableRowDeleted(this, new FishStatsTableRowChangeEvent(((FishStatsRow)(e.Row)), e.Action));
                    }
                }
                protected override void OnRowDeleting(System.Data.DataRowChangeEventArgs e)
                {
                    base.OnRowDeleting(e);
                    if ((this.FishStatsTableRowDeleting != null))
                    {
                        this.FishStatsTableRowDeleting(this, new FishStatsTableRowChangeEvent(((FishStatsRow)(e.Row)), e.Action));
                    }
                }
                #endregion Event Handlers
                #endregion Events/Delegates

                #region Public Methods
                #region Load/Save
                public void Load()
                {
                    try
                    {
                        if (onlineStatus != Statics.Enums.ONLINE_MODE.OFFLINE)
                        {
                            Server.SQL.FISHINGDATABASE.FillFishStats(this);
                        }
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("FishStatsTable::Load(): " + e.ToString());
                    }
                }
                public void Load(DateTime iTimeStart)
                {
                    try
                    {
                        if (onlineStatus != Statics.Enums.ONLINE_MODE.OFFLINE)
                        {
                            Server.SQL.FISHINGDATABASE.FillFishStats(this, iTimeStart);
                        }
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("FishStatsTable::Load(DateTime): " + e.ToString());
                    }
                }
                public bool Save()
                {
                    try
                    {
                        if (onlineStatus != Statics.Enums.ONLINE_MODE.OFFLINE)
                        {
                            return Server.SQL.FISHINGDATABASE.SaveFishStats(this);
                        }
                        return false;
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("FishStatsTable::Save: " + e.ToString());
                        return false;
                    }
                }
                // We only use either the server table or the local table to display to the user, NOT both.
                // If we're online, check if we have unsynced records to push to the server.
                //  - This is a seperate process. We don't do merging.
                //  - If we start up online and see records unsynced, push them to the server and delete from local (or mark synced if current).
                //  - Once we push to the server, do a new selection from the server for today's records.
                // If we're offline, save everything to file and mark new records as unsynced.
                //  - Load from file based on date.
                // If we're unstable...
                //  - Just treat everything as offline and unsynced.
                //  - This means that we can only mark a local record as synced when we actually save that record to the server.
                //  === Ex ===
                //  We're online. We go to save a record and it catches an exception.
                //  This record is now marked as unsynced in the local table.
                //  From now on we're offline and we stop trying to save to the server.


                //protected bool compare(FishStatsLocalRow iLocalRow, FishStatsRow iServerRow)
                //{
                //    try
                //    {
                //        if (iLocalRow.Date != iServerRow.Date)
                //        {
                //            return false;
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        LoggingFunctions.Error("FishIDsLocalTable::compare: " + e.ToString());
                //        return false;
                //    }
                //    return true;
                //}
                #endregion Load/Save
                #region Autogen
                public void AddFishStatsTableRow(FishStatsRow row)
                {
                    this.Rows.Add(row);
                }
                public override DataTable Clone()
                {
                    FishStatsTable cln = ((FishStatsTable)(base.Clone()));
                    cln.InitVars();
                    return cln;
                }
                protected override DataTable CreateInstance()
                {
                    return new FishStatsTable();
                }

                internal virtual void InitVars()
                {
                    this.columnResult = base.Columns["Result"];
                    this.columnPlayer = base.Columns["Player"];
                    this.columnDay = base.Columns["Day"];
                    this.columnTime = base.Columns["Time"];
                    this.columnMoon = base.Columns["Moon"];
                    this.columnWeather = base.Columns["Weather"];
                    this.columnSkill = base.Columns["Skill"];
                    this.columnFatigue = base.Columns["Fatigue"];
                    this.columnFishHP = base.Columns["FishHP"];
                    this.columnRecord = base.Columns["Record"];
                    this.columnZone = base.Columns["Zone"];
                    this.columnDate = base.Columns["Date"];
                    this.columnFish = base.Columns["Fish"];
                    this.columnRod = base.Columns["Rod"];
                    this.columnBait = base.Columns["Bait"];
                    this.columnInfo = base.Columns["Info"];
                    this.columnXPos = base.Columns["XPos"];
                    this.columnYPos = base.Columns["YPos"];
                    this.columnVersion = base.Columns["Version"];
                }
                public FishStatsRow NewFishStatsTableRow()
                {
                    return ((FishStatsRow)(this.NewRow()));
                }
                public void RemoveFishStatsTableRow(FishStatsRow row)
                {
                    this.Rows.Remove(row);
                }
                internal void copyRow(FishStatsRow iRow, ref FishStatsRow oRow)
                {
                    oRow.Bait = iRow.Bait;
                    oRow.Date = iRow.Date;
                    oRow.Day = iRow.Day;
                    oRow.Fatigue = iRow.Fatigue;
                    oRow.Fish = iRow.Fish;
                    oRow.FishHP = iRow.FishHP;
                    oRow.Info = iRow.Info;
                    oRow.Moon = iRow.Moon;
                    oRow.Player = iRow.Player;
                    oRow.Result = iRow.Result;
                    oRow.Rod = iRow.Rod;
                    oRow.Skill = iRow.Skill;
                    oRow.Time = iRow.Time;
                    oRow.Version = iRow.Version;
                    oRow.Weather = iRow.Weather;
                    oRow.XPos = iRow.XPos;
                    oRow.YPos = iRow.YPos;
                    oRow.Zone = iRow.Zone;
                }
                #endregion Autogen
                #endregion Public Methods

                #region Private Methods
                private void InitClass()
                {
                    this.columnResult = new DataColumn("Result", typeof(byte), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnResult);
                    this.columnPlayer = new DataColumn("Player", typeof(int), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnPlayer);
                    this.columnDay = new DataColumn("Day", typeof(byte), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnDay);
                    this.columnTime = new DataColumn("Time", typeof(short), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnTime);
                    this.columnMoon = new DataColumn("Moon", typeof(byte), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnMoon);
                    this.columnWeather = new DataColumn("Weather", typeof(byte), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnWeather);
                    this.columnSkill = new DataColumn("Skill", typeof(byte), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnSkill);
                    this.columnFatigue = new DataColumn("Fatigue", typeof(byte), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnFatigue);
                    this.columnFishHP = new DataColumn("FishHP", typeof(short), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnFishHP);
                    this.columnRecord = new DataColumn("Record", typeof(long), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnRecord);
                    this.columnZone = new DataColumn("Zone", typeof(short), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnZone);
                    this.columnDate = new DataColumn("Date", typeof(System.DateTime), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnDate);
                    this.columnFish = new DataColumn("Fish", typeof(short), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnFish);
                    this.columnRod = new DataColumn("Rod", typeof(short), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnRod);
                    this.columnBait = new DataColumn("Bait", typeof(short), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnBait);
                    this.columnInfo = new DataColumn("Info", typeof(int), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnInfo);
                    this.columnXPos = new DataColumn("XPos", typeof(short), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnXPos);
                    this.columnYPos = new DataColumn("YPos", typeof(short), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnYPos);
                    this.columnVersion = new DataColumn("Version", typeof(int), null, System.Data.MappingType.Element);
                    base.Columns.Add(this.columnVersion);
                    this.columnResult.AllowDBNull = false;
                    this.columnPlayer.AllowDBNull = false;
                    this.columnDay.AllowDBNull = false;
                    this.columnTime.AllowDBNull = false;
                    this.columnMoon.AllowDBNull = false;
                    this.columnWeather.AllowDBNull = false;
                    this.columnSkill.AllowDBNull = false;
                    this.columnFatigue.AllowDBNull = false;
                    this.columnFishHP.AllowDBNull = false;
                    this.columnRecord.AutoIncrement = true;
                    this.columnRecord.AutoIncrementSeed = -1;
                    this.columnRecord.AutoIncrementStep = -1;
                    this.columnRecord.AllowDBNull = false;
                    this.columnRecord.ReadOnly = true;
                    this.columnZone.AllowDBNull = false;
                    this.columnDate.AllowDBNull = false;
                    this.columnFish.AllowDBNull = false;
                    this.columnRod.AllowDBNull = false;
                    this.columnBait.AllowDBNull = false;
                    this.columnInfo.AllowDBNull = false;
                    this.filterGen = new FilterGenerator();
                }
                protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
                {
                    return new FishStatsRow(builder);
                }
                protected override System.Type GetRowType()
                {
                    return typeof(FishStatsRow);
                }
                #endregion Private Methods
            }
            #endregion Data Table
            #endregion Fish Stats Table

            #region Fish Stats Local Table (Child)
            #region Data Row
            internal partial class FishStatsLocalRow : FishStatsRow
            {
                private FishStatsLocalTable tableFishStatsLocal;

                internal FishStatsLocalRow(global::System.Data.DataRowBuilder rb) :
                        base(rb)
                {
                    this.tableFishStatsLocal = ((FishStatsLocalTable)(this.Table));
                }
                public bool Synced
                {
                    get
                    {
                        return ((bool)(this[this.tableFishStatsLocal.Synced]));
                    }
                    set
                    {
                        this[this.tableFishStatsLocal.Synced] = value;
                    }
                }
            }
            #endregion Data Row
            #region Data Table
            [global::System.Serializable()]
            [global::System.Xml.Serialization.XmlSchemaProviderAttribute("GetTypedTableSchema")]
            internal partial class FishStatsLocalTable : FishStatsTable
            {
                #region Private Members
                private global::System.Data.DataColumn columnSynced;
                #endregion Private Members

                #region Public Properties
                public global::System.Data.DataColumn Synced
                {
                    get
                    {
                        return this.columnSynced;
                    }
                }
                public new FishStatsLocalRow this[int index]
                {
                    get
                    {
                        return ((FishStatsLocalRow)(this.Rows[index]));
                    }
                }
                #endregion Public Properties

                #region Constructor
                public FishStatsLocalTable() : base()
                {
                    this.TableName = "FishStatsLocal";
                    this.columnSynced = new global::System.Data.DataColumn("Synced", typeof(bool), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnSynced);
                    base.FilterGen.SkipCols.Add(this.columnSynced);
                }
                #endregion Constructor

                #region Public Methods
                #region Load/Save
                // We only use either the server table or the local table to display to the user, NOT both.
                // If we're online, check if we have unsynced records to push to the server.
                //  - This is a seperate process. We don't do merging.
                //  - If we start up online and see records unsynced, push them to the server and delete from local (or mark synced if current).
                //  - Once we push to the server, do a new selection from the server for today's records.
                // If we're offline, save everything to file and mark new records as unsynced.
                //  - Load from file based on date.
                // If we're unstable...
                //  - Just treat everything as offline and unsynced.
                //  - This means that we can only mark a local record as synced when we actually save that record to the server.
                //  === Ex ===
                //  We're online. We go to save a record and it catches an exception.
                //  This record is now marked as unsynced in the local table.
                //  From now on we're offline and we stop trying to save to the server.

                public void Load(string iPlayer, string iServer, bool iAllFiles = true)
                {
                    try
                    {
                        if (iAllFiles)
                        {
                            List<string> files = getFileList(iPlayer, iServer);
                            foreach (string file in files)
                            {
                                loadFile(file);
                            }
                        }
                        else
                        {
                            loadFile(getTodaysFile(iPlayer, iServer));
                        }
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("FishStatsLocalTable::Load: " + e.ToString());
                    }
                }
                public new void Save(string iPlayer, string iServer)
                {
                    try
                    {
                        //When saving, we could potentially be loaded with records spanning multiple days.
                        //First, check if it's only since JP midnight.
                        //  If so, we just write the XML to a single file with today's JP date.
                        //If we have more, we'll filter them per day going backwards until we get zero records remaining.
                        //  Create a new table for each day and save the records.
                        //  For now, even save old records where synced = true.  We'll prune them out next time we're loaded.
                        DateTime lastJpMidnightUTC = Tools.VanaTime.LastJapaneseMidnightUTC;
                        FishStatsLocalRow[] rows = (FishStatsLocalRow[])this.Select("Date < '" + lastJpMidnightUTC.ToString() + "'");
                        checkFiles(iPlayer, iServer);
                        if (rows.Length == 0)
                        {
                            //Only today's records. Save with XML write.
                            this.WriteXml(Statics.Files.FishStatsLocalPath + getTodaysFile(iPlayer, iServer));
                            this.AcceptChanges();
                        }
                        else
                        {
                            //More than 1 day. Save 1 day at a time.
                            int cnt = 0;
                            bool today = true;
                            bool hasUnsynced = false;
                            DateTime jpDate = Tools.VanaTime.LastJapaneseMidnight;
                            DateTime startTime = lastJpMidnightUTC;
                            DateTime endTime = DateTime.UtcNow;
                            FishStatsLocalTable tempTable = new FishStatsLocalTable();
                            this.DataSet.Tables.Add(tempTable);
                            do
                            {
                                FishStatsLocalRow[] tempRows = (FishStatsLocalRow[])this.Select("Date >= '" + startTime.ToString() + "' AND Date < '" + endTime.ToString() + "'");
                                if (tempRows.Length > 0)
                                {
                                    tempTable.Clear();
                                    hasUnsynced = false;
                                    foreach (FishStatsLocalRow row in tempRows)
                                    {
                                        FishStatsLocalRow rowCpy = tempTable.NewFishStatsLocalRow();
                                        copyRow(row, ref rowCpy);
                                        rowCpy.Synced = row.Synced;
                                        if (row.Synced == false)
                                        {
                                            hasUnsynced = true;
                                        }
                                        tempTable.AddFishStatsLocalRow(rowCpy);
                                    }
                                    if (today || hasUnsynced)
                                    {
                                        string fileName = iPlayer + "_" + iServer + "_" + jpDate.ToString("yy_MM_dd") + ".xml";
                                        tempTable.WriteXml(Statics.Files.FishStatsLocalPath + fileName);
                                    }
                                }
                                cnt = this.Select("Date < '" + startTime.ToString() + "'").Length;

                                jpDate = jpDate.AddDays(-1);
                                endTime = startTime;
                                startTime = startTime.AddDays(-1);
                                today = false;
                            }
                            while (cnt > 0);
                            this.DataSet.Tables.Remove(tempTable);
                        }
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("FishStatsLocalTable::Save: " + e.ToString());
                    }
                }
                public void Push(string iPlayer, string iServer, FishStatsTable iDsStatsTable)
                {
                    try
                    {
                        //Pushes all unsynced records to the server if connected.
                        if (OnlineStatus == Statics.Enums.ONLINE_MODE.OFFLINE)
                        {
                            return;
                        }
                        // This should only be called at startup/login, so we shouldn't have anything loaded.
                        // Throw an error if we have records.
                        if (this.Rows.Count > 0)
                        {
                            LoggingFunctions.Error("FishStatsLocalTable::Push: Table is not empty.");
                            return;
                        }
                        List<string> files = getFileList(iPlayer, iServer);
                        for (int ii = 0; ii < files.Count; ii++)
                        {
                            //Load each file 1 by 1 and push any unsynced files to the server.
                            this.loadFile(files[ii], (files[ii] != getTodaysFile(iPlayer, iServer)));
                            foreach (FishStatsLocalRow row in this.Rows)
                            {
                                //If it's already synced, ignore.
                                //If it's not synced:
                                //  Add it to the server table. We'll save the table in the end.
                                if (row.Synced == true)
                                {
                                    continue;
                                }
                                FishStatsRow srvRow = iDsStatsTable.NewFishStatsTableRow();
                                srvRow.Bait = row.Bait;
                                srvRow.Date = row.Date;
                                srvRow.Day = row.Day;
                                srvRow.Fatigue = row.Fatigue;
                                srvRow.Fish = row.Fish;
                                srvRow.FishHP = row.FishHP;
                                srvRow.Info = row.Info;
                                srvRow.Moon = row.Moon;
                                srvRow.Result = row.Result;
                                srvRow.Rod = row.Rod;
                                srvRow.Skill = row.Skill;
                                srvRow.Time = row.Time;
                                srvRow.Version = row.Version;
                                srvRow.Weather = row.Weather;
                                srvRow.XPos = row.XPos;
                                srvRow.YPos = row.YPos;
                                srvRow.Zone = row.Zone;
                                srvRow.Player = 0;
                                iDsStatsTable.AddFishStatsTableRow(srvRow);

                                //Mark it as synced even though we haven't actually saved it to the server.
                                //If there's an error in saving, we'll try to handle it gracefully, but
                                //we may have to settle for lost records.

                                //What should happen is that we first add all of the records to the server table,
                                //then we purge all of the old files.
                                //We then save the server table.  If there is an error in saving,
                                //it will simply set us to offline mode and save all of the records to file again with synced = false.
                                row.Synced = true;
                                row.AcceptChanges();
                            }
                        }
                        if (iDsStatsTable.Save())
                        {
                            this.Save(iPlayer, iServer);
                        }
                        this.Clear();
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("FishStatsLocalTable::Push: " + e.ToString());
                    }
                }
                internal void MergeFromLocal(FishStatsLocalRow[] iRows)
                {
                    foreach (FishStatsLocalRow row in iRows)
                    {
                        string filter = "Date='" + row.Date.ToString() + "'";
                        FishStatsLocalRow[] selRows = (FishStatsLocalRow[])this.Select(filter);
                        if (selRows.Length == 0)
                        {
                            FishStatsLocalRow cpy = this.NewFishStatsLocalRow();
                            copyRow(row, ref cpy);
                            cpy.Synced = row.Synced;
                            this.AddFishStatsLocalRow(cpy);
                        }
                    }
                    this.AcceptChanges();
                }
                internal void MergeFromServer(FishStatsRow[] iRows)
                {
                    foreach (FishStatsRow row in iRows)
                    {
                        string filter = "Date='" + row.Date.ToString() + "'";
                        FishStatsLocalRow[] selRows = (FishStatsLocalRow[])this.Select(filter);
                        if (selRows.Length == 0)
                        {
                            FishStatsLocalRow cpy = this.NewFishStatsLocalRow();
                            copyRow(row, ref cpy);
                            cpy.Synced = (row.RowState == DataRowState.Unchanged);
                            this.AddFishStatsLocalRow(cpy);
                        }
                    }
                    this.AcceptChanges();
                }
                internal void copyRow(FishStatsRow iRow, ref FishStatsLocalRow oRow)
                {
                    oRow.Bait = iRow.Bait;
                    oRow.Date = iRow.Date;
                    oRow.Day = iRow.Day;
                    oRow.Fatigue = iRow.Fatigue;
                    oRow.Fish = iRow.Fish;
                    oRow.FishHP = iRow.FishHP;
                    oRow.Info = iRow.Info;
                    oRow.Moon = iRow.Moon;
                    oRow.Player = iRow.Player;
                    oRow.Result = iRow.Result;
                    oRow.Rod = iRow.Rod;
                    oRow.Skill = iRow.Skill;
                    oRow.Time = iRow.Time;
                    oRow.Version = iRow.Version;
                    oRow.Weather = iRow.Weather;
                    oRow.XPos = iRow.XPos;
                    oRow.YPos = iRow.YPos;
                    oRow.Zone = iRow.Zone;
                }
                private bool loadFile(string iFile, bool iDeleteAfter=false)
                {
                    try
                    {
                        if (this.Rows.Count == 0)
                        {
                            //Simply read in the XML.
                            this.ReadXml(iFile);
                        }
                        else
                        {
                            //Create another copy of this table to load the XML into.
                            //Then copy each row of that one into this table.
                            FishStatsLocalTable tempTable = new FishStatsLocalTable();
                            this.DataSet.Tables.Add(tempTable); //Because the XML refers to the dataset, we have to add the temp table to the dataset.
                            tempTable.ReadXml(iFile);
                            this.Merge(tempTable);
                            this.DataSet.Tables.Remove(tempTable);
                            tempTable.Dispose();
                        }
                        this.AcceptChanges();
                        if (iDeleteAfter)
                        {
                            File.Delete(iFile);
                        }
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("FishStatsLocalTable::loadFile: " + e.ToString());
                        return false;
                    }
                    return true;
                }
                private string getTodaysFile(string iPlayer, string iServer)
                {
                    string file = iPlayer + "_" + iServer + "_";
                    DateTime jpTime = getJpTime();
                    file += jpTime.ToString("yy_MM_dd") + ".xml";
                    return file;
                }
                private List<string> getFileList(string iPlayer, string iServer)
                {
                    List<string> files = new List<string>();
                    if (!Directory.Exists(Statics.Files.FishStatsLocalPath))
                    {
                        return files;
                    }
                    foreach (string file in Directory.GetFiles(Statics.Files.FishStatsLocalPath))
                    {
                        if (Path.GetFileNameWithoutExtension(file).Contains(iPlayer + "_" + iServer + "_"))
                        {
                            files.Add(file);
                        }
                    }
                    return files;
                }
                private static DateTime getJpTime()
                {
                    return Tools.VanaTime.Now.ToEarth().AddHours(9);
                }
                internal static DateTime getLastJpMidnight()
                {
                    DateTime localJpTime = getJpTime();
                    return new DateTime(localJpTime.Year, localJpTime.Month, localJpTime.Day, 0, 0, 0);
                }
                internal static DateTime getLastJpMidnightUTC()
                {
                    return getLastJpMidnight().AddHours(-9);
                }
                //protected bool compare(FishStatsLocalRow iLocalRow, FishStatsRow iServerRow)
                //{
                //    try
                //    {
                //        if (iLocalRow.Date != iServerRow.Date)
                //        {
                //            return false;
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        LoggingFunctions.Error("FishIDsLocalTable::compare: " + e.ToString());
                //        return false;
                //    }
                //    return true;
                //}
                #endregion Load/Save
                #endregion Public Methods

                #region Private Methods
                #region Checks
                private bool checkFiles(string iPlayer, string iServer)
                {
                    try
                    {
                        if (!Directory.Exists(Statics.Files.FisherFilesTopPath))
                        {
                            Directory.CreateDirectory(Statics.Files.FisherFilesTopPath);
                        }
                        if (!Directory.Exists(Statics.Files.FishStatsLocalPath))
                        {
                            Directory.CreateDirectory(Statics.Files.FishStatsLocalPath);
                        }
                        string pattern = iPlayer + "_" + iServer + "_";
                        if (Directory.GetFiles(Statics.Files.FishStatsLocalPath, pattern).Length == 0)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    catch (Exception e)
                    {
                        Logging.LoggingFunctions.Error("FishIDsLocalTable::checkFiles: " + e.ToString());
                        return false;
                    }
                }
                #endregion Checks
                internal void AddFishStatsLocalRow(FishStatsLocalRow row)
                {
                    this.Rows.Add(row);
                }
                public override global::System.Data.DataTable Clone()
                {
                    FishStatsLocalTable cln = ((FishStatsLocalTable)(base.Clone()));
                    cln.InitVars();
                    return cln;
                }
                internal FishStatsLocalRow NewFishStatsLocalRow()
                {
                    return ((FishStatsLocalRow)(this.NewRow()));
                }
                internal void RemoveFishStatsLocalRow(FishStatsLocalRow row)
                {
                    this.Rows.Remove(row);
                }
                internal override void InitVars()
                {
                    base.InitVars();
                    this.columnSynced = base.Columns["Synced"];
                }
                private void InitClass()
                {
                    this.columnSynced = new global::System.Data.DataColumn("Synced", typeof(bool), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnSynced);
                    this.columnSynced.AllowDBNull = false;
                }
                protected override global::System.Data.DataRow NewRowFromBuilder(global::System.Data.DataRowBuilder builder)
                {
                    return new FishStatsLocalRow(builder);
                }
                protected override global::System.Type GetRowType()
                {
                    return typeof(FishStatsLocalRow);
                }
                #endregion Private Methods
            }
            #endregion Data Table
            #endregion Fish IDs Local Table (Child)

            #region Results Table
            #region Data Row
            public partial class ResultTableRow : System.Data.DataRow
            {
                private ResultTable tableResultTable;

                internal ResultTableRow(global::System.Data.DataRowBuilder rb) :
                        base(rb)
                {
                    this.tableResultTable = ((ResultTable)(this.Table));
                }
                public string Result
                {
                    get
                    {
                        return ((string)(this[this.tableResultTable.ResultColumn]));
                    }
                    set
                    {
                        this[this.tableResultTable.ResultColumn] = value;
                    }
                }
                public byte ID
                {
                    get
                    {
                        return ((byte)(this[this.tableResultTable.IDColumn]));
                    }
                    set
                    {
                        this[this.tableResultTable.IDColumn] = value;
                    }
                }
            }
            #endregion Data Row
            #region Data Table
            [global::System.Serializable()]
            [global::System.Xml.Serialization.XmlSchemaProviderAttribute("GetTypedTableSchema")]
            public partial class ResultTable : System.Data.TypedTableBase<ResultTableRow>
            {
                private global::System.Data.DataColumn columnResult;
                private global::System.Data.DataColumn columnID;

                public ResultTable()
                {
                    this.TableName = "ResultTable";
                    this.BeginInit();
                    this.InitClass();
                    this.EndInit();
                }
                internal ResultTable(global::System.Data.DataTable table)
                {
                    this.TableName = table.TableName;
                    if ((table.CaseSensitive != table.DataSet.CaseSensitive))
                    {
                        this.CaseSensitive = table.CaseSensitive;
                    }
                    if ((table.Locale.ToString() != table.DataSet.Locale.ToString()))
                    {
                        this.Locale = table.Locale;
                    }
                    if ((table.Namespace != table.DataSet.Namespace))
                    {
                        this.Namespace = table.Namespace;
                    }
                    this.Prefix = table.Prefix;
                    this.MinimumCapacity = table.MinimumCapacity;
                }
                protected ResultTable(global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context) :
                        base(info, context)
                {
                    this.InitVars();
                }

                public global::System.Data.DataColumn ResultColumn
                {
                    get
                    {
                        return this.columnResult;
                    }
                }

                public global::System.Data.DataColumn IDColumn
                {
                    get
                    {
                        return this.columnID;
                    }
                }

                [global::System.ComponentModel.Browsable(false)]
                public int Count
                {
                    get
                    {
                        return this.Rows.Count;
                    }
                }

                public ResultTableRow this[int index]
                {
                    get
                    {
                        return ((ResultTableRow)(this.Rows[index]));
                    }
                }

                public event ResultTableRowChangeEventHandler ResultTableRowChanging;
                public event ResultTableRowChangeEventHandler ResultTableRowChanged;
                public event ResultTableRowChangeEventHandler ResultTableRowDeleting;
                public event ResultTableRowChangeEventHandler ResultTableRowDeleted;

                public void AddResultTableRow(ResultTableRow row)
                {
                    this.Rows.Add(row);
                }
                public ResultTableRow AddResultTableRow(string Result, byte ID)
                {
                    ResultTableRow rowResultTableRow = ((ResultTableRow)(this.NewRow()));
                    object[] columnValuesArray = new object[] {
                        Result,
                        ID};
                    rowResultTableRow.ItemArray = columnValuesArray;
                    this.Rows.Add(rowResultTableRow);
                    return rowResultTableRow;
                }
                public ResultTableRow FindByID(byte ID)
                {
                    return ((ResultTableRow)(this.Rows.Find(new object[] { ID })));
                }
                public override global::System.Data.DataTable Clone()
                {
                    ResultTable cln = ((ResultTable)(base.Clone()));
                    cln.InitVars();
                    return cln;
                }
                protected override global::System.Data.DataTable CreateInstance()
                {
                    return new ResultTable();
                }
                internal void InitVars()
                {
                    this.columnResult = base.Columns["Result"];
                    this.columnID = base.Columns["ID"];
                }
                private void InitClass()
                {
                    this.columnResult = new global::System.Data.DataColumn("Result", typeof(string), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnResult);
                    this.columnID = new global::System.Data.DataColumn("ID", typeof(byte), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnID);
                    this.Constraints.Add(new global::System.Data.UniqueConstraint("Constraint1", new global::System.Data.DataColumn[] {
                                this.columnID}, true));
                    this.columnResult.AllowDBNull = false;
                    this.columnResult.MaxLength = 100;
                    this.columnID.AllowDBNull = false;
                    this.columnID.ReadOnly = true;
                    this.columnID.Unique = true;
                }
                public ResultTableRow NewResultTableRow()
                {
                    return ((ResultTableRow)(this.NewRow()));
                }
                protected override global::System.Data.DataRow NewRowFromBuilder(global::System.Data.DataRowBuilder builder)
                {
                    return new ResultTableRow(builder);
                }
                protected override global::System.Type GetRowType()
                {
                    return typeof(ResultTableRow);
                }
                protected override void OnRowChanged(global::System.Data.DataRowChangeEventArgs e)
                {
                    base.OnRowChanged(e);
                    if ((this.ResultTableRowChanged != null))
                    {
                        this.ResultTableRowChanged(this, new ResultTableRowChangeEvent(((ResultTableRow)(e.Row)), e.Action));
                    }
                }
                protected override void OnRowChanging(global::System.Data.DataRowChangeEventArgs e)
                {
                    base.OnRowChanging(e);
                    if ((this.ResultTableRowChanging != null))
                    {
                        this.ResultTableRowChanging(this, new ResultTableRowChangeEvent(((ResultTableRow)(e.Row)), e.Action));
                    }
                }
                protected override void OnRowDeleted(global::System.Data.DataRowChangeEventArgs e)
                {
                    base.OnRowDeleted(e);
                    if ((this.ResultTableRowDeleted != null))
                    {
                        this.ResultTableRowDeleted(this, new ResultTableRowChangeEvent(((ResultTableRow)(e.Row)), e.Action));
                    }
                }
                protected override void OnRowDeleting(global::System.Data.DataRowChangeEventArgs e)
                {
                    base.OnRowDeleting(e);
                    if ((this.ResultTableRowDeleting != null))
                    {
                        this.ResultTableRowDeleting(this, new ResultTableRowChangeEvent(((ResultTableRow)(e.Row)), e.Action));
                    }
                }
                public void RemoveResultTableRow(ResultTableRow row)
                {
                    this.Rows.Remove(row);
                }
            }
            #region Row Events
            public delegate void ResultTableRowChangeEventHandler(object sender, ResultTableRowChangeEvent e);
            public class ResultTableRowChangeEvent : global::System.EventArgs
            {

                private ResultTableRow eventRow;

                private global::System.Data.DataRowAction eventAction;

                public ResultTableRowChangeEvent(ResultTableRow row, global::System.Data.DataRowAction action)
                {
                    this.eventRow = row;
                    this.eventAction = action;
                }

                public ResultTableRow Row
                {
                    get
                    {
                        return this.eventRow;
                    }
                }

                public global::System.Data.DataRowAction Action
                {
                    get
                    {
                        return this.eventAction;
                    }
                }
            }
            #endregion Row Events
            #endregion Data Table
            #endregion Results Table

            #region Fish IDs Table
            #region Data Row
            internal partial class FishIDsRow : global::System.Data.DataRow
            {
                #region Private Members
                private FishIDsTable tableFishIDs;
                #endregion Private Members

                #region Public Properties
                public int Fish
                {
                    get
                    {
                        return ((int)(this[this.tableFishIDs.FishColumn]));
                    }
                    set
                    {
                        this[this.tableFishIDs.FishColumn] = value;
                    }
                }
                public short ID1
                {
                    get
                    {
                        return ((short)(this[this.tableFishIDs.ID1Column]));
                    }
                    set
                    {
                        this[this.tableFishIDs.ID1Column] = value;
                    }
                }
                public short ID2
                {
                    get
                    {
                        return ((short)(this[this.tableFishIDs.ID2Column]));
                    }
                    set
                    {
                        this[this.tableFishIDs.ID2Column] = value;
                    }
                }
                public short ID3
                {
                    get
                    {
                        return ((short)(this[this.tableFishIDs.ID3Column]));
                    }
                    set
                    {
                        this[this.tableFishIDs.ID3Column] = value;
                    }
                }
                public short Zone
                {
                    get
                    {
                        return ((short)(this[this.tableFishIDs.ZoneColumn]));
                    }
                    set
                    {
                        this[this.tableFishIDs.ZoneColumn] = value;
                    }
                }
                public int minHP
                {
                    get
                    {
                        return ((int)(this[this.tableFishIDs.minHPColumn]));
                    }
                    set
                    {
                        this[this.tableFishIDs.minHPColumn] = value;
                    }
                }
                public int maxHP
                {
                    get
                    {
                        return ((int)(this[this.tableFishIDs.maxHPColumn]));
                    }
                    set
                    {
                        this[this.tableFishIDs.maxHPColumn] = value;
                    }
                }
                public int Rod
                {
                    get
                    {
                        return ((int)(this[this.tableFishIDs.RodColumn]));
                    }
                    set
                    {
                        this[this.tableFishIDs.RodColumn] = value;
                    }
                }
                public int IDCount
                {
                    get
                    {
                        return ((int)(this[this.tableFishIDs.IDCountColumn]));
                    }
                    set
                    {
                        this[this.tableFishIDs.IDCountColumn] = value;
                    }
                }
                public bool Large
                {
                    get
                    {
                        return ((bool)(this[this.tableFishIDs.LargeColumn]));
                    }
                    set
                    {
                        this[this.tableFishIDs.LargeColumn] = value;
                    }
                }
                #endregion Public Properties

                #region Constructor
                internal FishIDsRow(global::System.Data.DataRowBuilder rb) :
                        base(rb)
                {
                    this.tableFishIDs = ((FishIDsTable)(this.Table));
                }
                #endregion Constructor
            }
            #endregion Data Row
            #region Data Table
            [global::System.Serializable()]
            [global::System.Xml.Serialization.XmlSchemaProviderAttribute("GetTypedTableSchema")]
            internal partial class FishIDsTable : global::System.Data.TypedTableBase<FishIDsRow>
            {
                #region Private Members
                private Statics.Enums.ONLINE_MODE onlineStatus = Statics.Enums.ONLINE_MODE.UNKNOWN;
                private FilterGenerator filterGen;
                private global::System.Data.DataColumn columnFish;
                private global::System.Data.DataColumn columnID1;
                private global::System.Data.DataColumn columnID2;
                private global::System.Data.DataColumn columnID3;
                private global::System.Data.DataColumn columnZone;
                private global::System.Data.DataColumn columnminHP;
                private global::System.Data.DataColumn columnmaxHP;
                private global::System.Data.DataColumn columnRod;
                private global::System.Data.DataColumn columnIDCount;
                private global::System.Data.DataColumn columnLarge;
                #endregion Private Members

                #region Public Properties
                public Statics.Enums.ONLINE_MODE OnlineStatus
                {
                    get
                    {
                        return onlineStatus;
                    }
                    set
                    {
                        onlineStatus = value;
                    }
                }
                public FilterGenerator FilterGen
                {
                    get
                    {
                        if (filterGen == null)
                        {
                            filterGen = new FilterGenerator();
                        }
                        return filterGen;
                    }
                }
                public global::System.Data.DataColumn FishColumn
                {
                    get
                    {
                        return this.columnFish;
                    }
                }
                public global::System.Data.DataColumn ID1Column
                {
                    get
                    {
                        return this.columnID1;
                    }
                }
                public global::System.Data.DataColumn ID2Column
                {
                    get
                    {
                        return this.columnID2;
                    }
                }
                public global::System.Data.DataColumn ID3Column
                {
                    get
                    {
                        return this.columnID3;
                    }
                }
                public global::System.Data.DataColumn ZoneColumn
                {
                    get
                    {
                        return this.columnZone;
                    }
                }
                public global::System.Data.DataColumn minHPColumn
                {
                    get
                    {
                        return this.columnminHP;
                    }
                }
                public global::System.Data.DataColumn maxHPColumn
                {
                    get
                    {
                        return this.columnmaxHP;
                    }
                }
                public global::System.Data.DataColumn RodColumn
                {
                    get
                    {
                        return this.columnRod;
                    }
                }
                public global::System.Data.DataColumn IDCountColumn
                {
                    get
                    {
                        return this.columnIDCount;
                    }
                }
                public global::System.Data.DataColumn LargeColumn
                {
                    get
                    {
                        return this.columnLarge;
                    }
                }
                public int Count
                {
                    get
                    {
                        return this.Rows.Count;
                    }
                }
                public FishIDsRow this[int index]
                {
                    get
                    {
                        return ((FishIDsRow)(this.Rows[index]));
                    }
                }
                #endregion Public Properties

                #region Constructors
                public FishIDsTable()
                {
                    this.TableName = "FishIDs";
                    this.BeginInit();
                    this.InitClass();
                    this.EndInit();
                }
                internal FishIDsTable(global::System.Data.DataTable table)
                {
                    this.TableName = table.TableName;
                    if ((table.CaseSensitive != table.DataSet.CaseSensitive))
                    {
                        this.CaseSensitive = table.CaseSensitive;
                    }
                    if ((table.Locale.ToString() != table.DataSet.Locale.ToString()))
                    {
                        this.Locale = table.Locale;
                    }
                    if ((table.Namespace != table.DataSet.Namespace))
                    {
                        this.Namespace = table.Namespace;
                    }
                    this.Prefix = table.Prefix;
                    this.MinimumCapacity = table.MinimumCapacity;
                }
                protected FishIDsTable(global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context) :
                        base(info, context)
                {
                    this.InitVars();
                }
                #endregion Constructors

                #region Public Methods
                public virtual void AddFishIDsLocalRow(FishIDsLocalRow iRow)
                {
                    FishIDsRow row = this.NewFishIDsRow();
                    row.Fish = iRow.Fish;
                    row.ID1 = iRow.ID1;
                    row.ID2 = iRow.ID2;
                    row.ID3 = iRow.ID3;
                    row.Large = iRow.Large;
                    row.maxHP = iRow.maxHP;
                    row.minHP = iRow.minHP;
                    row.Rod = iRow.Rod;
                    row.Zone = iRow.Zone;
                    this.AddFishIDsRow(row);
                }
                #region Load/Save
                public bool Load(ushort iRod, ushort iZone)
                {
                    try
                    {
                        return Server.SQL.FISHINGDATABASE.FillFishIDs(this, iRod, iZone);
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("FishIDsTable::Load: " + e.ToString());
                        return false;
                    }
                }
                public bool Save(out bool oRefillTable)
                {
                    oRefillTable = false;
                    try
                    {
                        return Server.SQL.FISHINGDATABASE.SaveFishIDs(this, out oRefillTable);
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("FishIDsTable::Save: " + e.ToString());
                        return false;
                    }
                }
                public void MergeFromLocal(FishIDsLocalTable iTable)
                {
                    try
                    {
                        if (iTable == null)
                        {
                            return;
                        }
                        bool found = false;
                        foreach (FishIDsLocalRow lclRow in iTable.Rows)
                        {
                            foreach (FishIDsRow srvRow in this.Rows)
                            {
                                if (compare(lclRow, srvRow))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found)
                            {
                                found = false;
                                continue;
                            }
                            else
                            {
                                //Copy from local to server
                                FishIDsRow row = this.NewFishIDsRow();
                                foreach (DataColumn col in lclRow.Table.Columns)
                                {
                                    if (col.ColumnName == "Synced")
                                    {
                                        lclRow[col] = true;
                                        continue;
                                    }
                                    row[col.ColumnName] = lclRow[col.ColumnName];
                                }
                                this.AddFishIDsRow(row);
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("FishIDsTable::MergeFromLocal: " + e.ToString());
                    }

                }
                protected bool compare(FishIDsLocalRow iLocalRow, FishIDsRow iServerRow)
                {
                    try
                    {
                        if ((iLocalRow == null) || (iServerRow == null))
                        {
                            return false;
                        }
                        int nbCols = iLocalRow.Table.Columns.Count;
                        for (int ii = 0; ii < nbCols; ii++)
                        {
                            string colName = iLocalRow.Table.Columns[ii].ColumnName;
                            if (colName == "Synced")
                            {
                                continue;
                            }
                            if ((iLocalRow[colName] == DBNull.Value) || (iServerRow[colName] == DBNull.Value))
                            {
                                return false;
                            }
                            if (iLocalRow[colName].ToString() != iServerRow[colName].ToString())
                            {
                                return false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("FishIDsLocalTable::compare: " + e.ToString());
                        return false;
                    }
                    return true;
                }
                #endregion Load/Save
                #region Autogen
                public void AddFishIDsRow(FishIDsRow row)
                {
                    this.Rows.Add(row);
                }
                public FishIDsRow FindByIDCount(int IDCount)
                {
                    return ((FishIDsRow)(this.Rows.Find(new object[] { IDCount })));
                }
                public override global::System.Data.DataTable Clone()
                {
                    FishIDsTable cln = ((FishIDsTable)(base.Clone()));
                    cln.InitVars();
                    return cln;
                }
                public FishIDsRow NewFishIDsRow()
                {
                    return ((FishIDsRow)(this.NewRow()));
                }
                public void RemoveFishIDsRow(FishIDsRow row)
                {
                    this.Rows.Remove(row);
                }
                #endregion Autogen
                #endregion Public Methods

                #region Private Methods
                protected override global::System.Data.DataTable CreateInstance()
                {
                    return new FishIDsTable();
                }
                internal virtual void InitVars()
                {
                    this.columnFish = base.Columns["Fish"];
                    this.columnID1 = base.Columns["ID1"];
                    this.columnID2 = base.Columns["ID2"];
                    this.columnID3 = base.Columns["ID3"];
                    this.columnZone = base.Columns["Zone"];
                    this.columnminHP = base.Columns["minHP"];
                    this.columnmaxHP = base.Columns["maxHP"];
                    this.columnRod = base.Columns["Rod"];
                    this.columnIDCount = base.Columns["IDCount"];
                    this.columnLarge = base.Columns["Large"];
                }
                private void InitClass()
                {
                    this.columnFish = new global::System.Data.DataColumn("Fish", typeof(int), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnFish);
                    this.columnID1 = new global::System.Data.DataColumn("ID1", typeof(short), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnID1);
                    this.columnID2 = new global::System.Data.DataColumn("ID2", typeof(short), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnID2);
                    this.columnID3 = new global::System.Data.DataColumn("ID3", typeof(short), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnID3);
                    this.columnZone = new global::System.Data.DataColumn("Zone", typeof(short), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnZone);
                    this.columnminHP = new global::System.Data.DataColumn("minHP", typeof(int), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnminHP);
                    this.columnmaxHP = new global::System.Data.DataColumn("maxHP", typeof(int), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnmaxHP);
                    this.columnRod = new global::System.Data.DataColumn("Rod", typeof(int), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnRod);
                    this.columnIDCount = new global::System.Data.DataColumn("IDCount", typeof(int), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnIDCount);
                    this.columnLarge = new global::System.Data.DataColumn("Large", typeof(bool), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnLarge);
                    //this.Constraints.Add(new global::System.Data.UniqueConstraint("Constraint1", new global::System.Data.DataColumn[] {
                    //            this.columnIDCount}, true));
                    this.columnFish.AllowDBNull = false;
                    this.columnID1.AllowDBNull = false;
                    this.columnID2.AllowDBNull = false;
                    this.columnID3.AllowDBNull = false;
                    this.columnZone.AllowDBNull = false;
                    this.columnminHP.AllowDBNull = false;
                    this.columnmaxHP.AllowDBNull = false;
                    this.columnRod.AllowDBNull = false;
                    this.columnIDCount.AutoIncrement = true;
                    this.columnIDCount.AutoIncrementSeed = 1;
                    this.columnIDCount.AllowDBNull = false;
                    this.columnIDCount.Unique = false;
                    this.columnLarge.AllowDBNull = false;
                    this.filterGen = new FilterGenerator();
                }
                protected override global::System.Data.DataRow NewRowFromBuilder(global::System.Data.DataRowBuilder builder)
                {
                    return new FishIDsRow(builder);
                }
                protected override global::System.Type GetRowType()
                {
                    return typeof(FishIDsRow);
                }
                #region Utils
                internal void copyRow(FishIDsRow iRow, ref FishIDsRow oRow)
                {
                    oRow.Fish = iRow.Fish;
                    oRow.ID1 = iRow.ID1;
                    oRow.ID2 = iRow.ID2;
                    oRow.ID3 = iRow.ID3;
                    oRow.IDCount = iRow.IDCount;
                    oRow.Large = iRow.Large;
                    oRow.maxHP = iRow.maxHP;
                    oRow.minHP = iRow.minHP;
                    oRow.Rod = iRow.Rod;
                    oRow.Zone = iRow.Zone;
                }
                #endregion Utils
                #endregion Private Methods

                #region Events
                public class FishIDsRowChangeEvent : global::System.EventArgs
                {
                    private FishIDsRow eventRow;
                    private global::System.Data.DataRowAction eventAction;

                    public FishIDsRowChangeEvent(FishIDsRow row, global::System.Data.DataRowAction action)
                    {
                        this.eventRow = row;
                        this.eventAction = action;
                    }
                    public FishIDsRow Row
                    {
                        get
                        {
                            return this.eventRow;
                        }
                    }
                    public global::System.Data.DataRowAction Action
                    {
                        get
                        {
                            return this.eventAction;
                        }
                    }
                }
                public delegate void FishIDsRowChangeEventHandler(object sender, FishIDsRowChangeEvent e);
                public event FishIDsRowChangeEventHandler FishIDsRowChanging;
                public event FishIDsRowChangeEventHandler FishIDsRowChanged;
                public event FishIDsRowChangeEventHandler FishIDsRowDeleting;
                public event FishIDsRowChangeEventHandler FishIDsRowDeleted;
                #endregion Events

                #region Handlers
                protected override void OnRowChanged(global::System.Data.DataRowChangeEventArgs e)
                {
                    base.OnRowChanged(e);
                    if ((this.FishIDsRowChanged != null))
                    {
                        this.FishIDsRowChanged(this, new FishIDsRowChangeEvent(((FishIDsRow)(e.Row)), e.Action));
                    }
                }
                protected override void OnRowChanging(global::System.Data.DataRowChangeEventArgs e)
                {
                    base.OnRowChanging(e);
                    if ((this.FishIDsRowChanging != null))
                    {
                        this.FishIDsRowChanging(this, new FishIDsRowChangeEvent(((FishIDsRow)(e.Row)), e.Action));
                    }
                }
                protected override void OnRowDeleted(global::System.Data.DataRowChangeEventArgs e)
                {
                    base.OnRowDeleted(e);
                    if ((this.FishIDsRowDeleted != null))
                    {
                        this.FishIDsRowDeleted(this, new FishIDsRowChangeEvent(((FishIDsRow)(e.Row)), e.Action));
                    }
                }
                protected override void OnRowDeleting(global::System.Data.DataRowChangeEventArgs e)
                {
                    base.OnRowDeleting(e);
                    if ((this.FishIDsRowDeleting != null))
                    {
                        this.FishIDsRowDeleting(this, new FishIDsRowChangeEvent(((FishIDsRow)(e.Row)), e.Action));
                    }
                }
                #endregion Handlers
            }
            #endregion Data Table
            #endregion Fish IDs Table

            #region Fish IDs Local Table (Child)
            #region Data Row
            internal partial class FishIDsLocalRow : FishIDsRow
            {
                private FishIDsLocalTable tableFishIDsLocal;

                internal FishIDsLocalRow(global::System.Data.DataRowBuilder rb) :
                        base(rb)
                {
                    this.tableFishIDsLocal = ((FishIDsLocalTable)(this.Table));
                }
                public bool Synced
                {
                    get
                    {
                        return ((bool)(this[this.tableFishIDsLocal.Synced]));
                    }
                    set
                    {
                        this[this.tableFishIDsLocal.Synced] = value;
                    }
                }
                public bool Catch
                {
                    get
                    {
                        try
                        {
                            return ((bool)(this[this.tableFishIDsLocal.CatchColumn]));
                        }
                        catch (global::System.InvalidCastException e)
                        {
                            throw new global::System.Data.StrongTypingException("The value for column \'Catch\' in table \'FishIDsLocal\' is DBNull.", e);
                        }
                    }
                    set
                    {
                        this[this.tableFishIDsLocal.CatchColumn] = value;
                    }
                }
                public bool IsCatchNull()
                {
                    return this.IsNull(this.tableFishIDsLocal.CatchColumn);
                }
                public void SetCatchNull()
                {
                    this[this.tableFishIDsLocal.CatchColumn] = global::System.Convert.DBNull;
                }
            }
            #endregion Data Row
            #region Data Table
            [global::System.Serializable()]
            [global::System.Xml.Serialization.XmlSchemaProviderAttribute("GetTypedTableSchema")]
            internal partial class FishIDsLocalTable : FishIDsTable
            {
                #region Private Members
                private global::System.Data.DataColumn columnSynced;
                private global::System.Data.DataColumn columnCatch;
                #endregion Private Members

                #region Public Properties
                public global::System.Data.DataColumn Synced
                {
                    get
                    {
                        return this.columnSynced;
                    }
                }
                public global::System.Data.DataColumn CatchColumn
                {
                    get
                    {
                        return this.columnCatch;
                    }
                }
                public new FishIDsLocalRow this[int index]
                {
                    get
                    {
                        return ((FishIDsLocalRow)(this.Rows[index]));
                    }
                }
                #endregion Public Properties

                #region Constructor
                public FishIDsLocalTable() : base()
                {
                    this.TableName = "FishIDsLocal";
                    InitClass();
                }
                #endregion Constructor

                #region Public Methods
                #region Load/Save
                public new void Load()
                {
                    try
                    {
                        if (!checkFiles())
                        {
                            return;
                        }
                        this.ReadXml(Statics.Files.FishIdsLocalPath + Statics.Files.FishIdsFileName);
                        this.AcceptChanges();
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("FishIDsLocalTable::Load: " + e.ToString());
                    }
                }
                public new void Save()
                {
                    try
                    {
                        checkFiles();
                        this.WriteXml(Statics.Files.FishIdsLocalPath + Statics.Files.FishIdsFileName);
                        this.AcceptChanges();
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("FishIDsLocalTable::Save: " + e.ToString());
                    }
                }
                public bool MergeToLocal(FishIDsTable iTable)
                {
                    bool addedToLocal = false;
                    try
                    {
                        if (iTable == null)
                        {
                            return false;
                        }
                        bool found = false;
                        foreach (FishIDsRow srvRow in iTable.Rows)
                        {
                            foreach (FishIDsLocalRow lclRow in this.Rows)
                            {
                                if (compare(lclRow, srvRow))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found)
                            {
                                found = false;
                                continue;
                            }
                            else
                            {
                                //Copy from server to local.
                                FishIDsLocalRow row = this.NewFishIDsLocalRow();
                                foreach (DataColumn col in srvRow.Table.Columns)
                                {
                                    row[col.ColumnName] = srvRow[col.ColumnName];
                                }
                                row.Synced = true;
                                this.AddFishIDsLocalRow(row);
                                addedToLocal = true;
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("FishIDsLocalTable::MergeToLocal: " + e.ToString());
                    }
                    return addedToLocal;
                }
                internal bool HasUnsavedRows()
                {
                    if (this == null)
                    {
                        return false;
                    }
                    for (int ii = 0; ii < this.Rows.Count; ii++)
                    {
                        if (this[ii].Synced == false)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                #endregion Load/Save
                #region Autogen
                public override void AddFishIDsLocalRow(FishIDsLocalRow row)
                {
                    this.Rows.Add(row);
                }
                public new FishIDsLocalRow FindByIDCount(int IDCount)
                {
                    return ((FishIDsLocalRow)(this.Rows.Find(new object[] { IDCount })));
                }
                public override System.Data.DataTable Clone()
                {
                    FishIDsTable cln = ((FishIDsTable)(base.Clone()));
                    cln.InitVars();
                    return cln;
                }
                public FishIDsLocalRow NewFishIDsLocalRow()
                {
                    return ((FishIDsLocalRow)(this.NewRow()));
                }
                public void RemoveFishIDsLocalRow(FishIDsLocalRow row)
                {
                    this.Rows.Remove(row);
                }
                #endregion Autogen
                #endregion Public Methods

                #region Private Methods
                #region Utils
                internal void copyRow(FishIDsRow iRow, ref FishIDsLocalRow oRow)
                {
                    oRow.Fish = iRow.Fish;
                    oRow.ID1 = iRow.ID1;
                    oRow.ID2 = iRow.ID2;
                    oRow.ID3 = iRow.ID3;
                    oRow.IDCount = iRow.IDCount;
                    oRow.Large = iRow.Large;
                    oRow.maxHP = iRow.maxHP;
                    oRow.minHP = iRow.minHP;
                    oRow.Rod = iRow.Rod;
                    oRow.Zone = iRow.Zone;
                }
                #endregion Utils
                #region Checks
                private bool checkFiles()
                {
                    try
                    {
                        if (!Directory.Exists(Statics.Files.FisherFilesTopPath))
                        {
                            Directory.CreateDirectory(Statics.Files.FisherFilesTopPath);
                        }
                        if (!Directory.Exists(Statics.Files.FishIdsLocalPath))
                        {
                            Directory.CreateDirectory(Statics.Files.FishIdsLocalPath);
                        }
                        if (!File.Exists(Statics.Files.FishIdsLocalPath + Statics.Files.FishIdsFileName))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    catch (Exception e)
                    {
                        Logging.LoggingFunctions.Error("FishIDsLocalTable::checkFiles: " + e.ToString());
                        return false;
                    }
                }
                #endregion Checks
                #region Autogen
                internal override void InitVars()
                {
                    base.InitVars();
                    this.columnSynced = base.Columns["Synced"];
                    this.columnCatch = base.Columns["Catch"];
                }
                private void InitClass()
                {
                    this.columnSynced = new global::System.Data.DataColumn("Synced", typeof(bool), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnSynced);
                    this.columnSynced.AllowDBNull = false;
                    this.columnCatch = new global::System.Data.DataColumn("Catch", typeof(bool), null, global::System.Data.MappingType.Element);
                    base.Columns.Add(this.columnCatch);
                    this.columnCatch.AllowDBNull = true;
                    this.columnCatch.DefaultValue = ((bool)(false));
                    base.FilterGen.SkipCols.Add(this.CatchColumn);
                    base.FilterGen.SkipCols.Add(this.Synced);
                }
                protected override global::System.Data.DataRow NewRowFromBuilder(global::System.Data.DataRowBuilder builder)
                {
                    return new FishIDsLocalRow(builder);
                }
                protected override global::System.Type GetRowType()
                {
                    return typeof(FishIDsLocalRow);
                }
                #endregion Autogen
                #endregion Private Methods
            }
            #endregion Data Table
            #endregion Fish IDs Local Table (Child)

            #region Filter Generator (Table Helper)
            internal class FilterGenerator
            {
                private bool skipAutoIncCol = true;
                private List<DataColumn> skipCols = new List<DataColumn>();
                public bool SkipAutoIncCol
                {
                    get
                    {
                        return skipAutoIncCol;
                    }
                    set
                    {
                        skipAutoIncCol = value;
                    }
                }
                public List<DataColumn> SkipCols
                {
                    get
                    {
                        return skipCols;
                    }
                    set
                    {
                        skipCols = value;
                    }
                }

                private void sortModColumns(DataRow iRow, out List<DataColumn> oModCols, out List<DataColumn> oUnmodCols)
                {
                    oModCols = new List<DataColumn>();
                    oUnmodCols = new List<DataColumn>();
                    if (iRow.RowState != DataRowState.Modified)
                    {
                        return;
                    }
                    foreach (DataColumn col in iRow.Table.Columns)
                    {
                        bool hasCur = iRow.HasVersion(DataRowVersion.Current);
                        bool hasPro = iRow.HasVersion(DataRowVersion.Proposed);
                        bool hasOri = iRow.HasVersion(DataRowVersion.Original);
                        object curVal = iRow[col, DataRowVersion.Current];
                        object oriVal = iRow[col, DataRowVersion.Original];
                        if ((skipCols != null) && skipCols.Contains(col))
                        {
                            continue;
                        }
                        if ((skipAutoIncCol == true) && (col.AutoIncrement == true))
                        {
                            continue;
                        }
                        if (hasCur && (iRow[col, DataRowVersion.Current].ToString() != iRow[col, DataRowVersion.Original].ToString()))
                        {
                            oModCols.Add(col);
                        }
                        else
                        {
                            oUnmodCols.Add(col);
                        }
                    }
                    return;
                }
                public string GetFilter(DataRow iRow)
                {
                    List<DataColumn> dummy = null;
                    return GetFilter(iRow, out dummy);
                }
                public string GetFilter(DataRow iRow, out List<DataColumn> oModCols)
                {
                    string filter = "";
                    oModCols = null;
                    if (iRow.Table == null)
                    {
                        LoggingFunctions.Error("RowStatusChecker::GetFilter: Row table must not be null.");
                        return "";
                    }
                    List<DataColumn> unmodCols = null;
                    sortModColumns(iRow, out oModCols, out unmodCols);
                    for (int ii = 0; ii < unmodCols.Count; ii++)
                    {
                        if (ii != 0)
                        {
                            filter += " AND ";
                        }
                        filter += unmodCols[ii].ColumnName + " = ";
                        if (unmodCols[ii].DataType == Type.GetType("System.String") ||
                            unmodCols[ii].DataType == Type.GetType("System.DateTime") ||
                            unmodCols[ii].DataType == Type.GetType("System.Boolean"))
                        {
                            filter += "'" + iRow[unmodCols[ii]] + "'";
                        }
                        else
                        {
                            filter += iRow[unmodCols[ii]];
                        }
                    }
                    return filter;
                }
            }
            #endregion Filter Generator (Table Helper)
            #endregion Data Tables/Rows
        }
        #endregion Fishing Dataset
    }
}
