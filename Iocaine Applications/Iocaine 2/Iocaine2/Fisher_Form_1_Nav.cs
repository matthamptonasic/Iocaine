using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Bots;
using Iocaine2.Data.Structures;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Settings;

using Iocaine2.Data.Client;

namespace Iocaine2
{
    partial class Iocaine_2_Form
    {
        #region Enums
        internal enum NAV_NODE_TYPE : byte
        {
            POS_NODE = 0,
            POS_START = 1,
            POS_END = 2,
            POS_ZONE = 3,
            NPC_TARGET = 4,
            NPC_TRADE_ITEM = 5,
            NPC_TRADE_GIL = 6,
            COMMAND = 7,
            KEYSTROKE = 8,
            WAIT = 9
        }
        internal enum NAV_KEYSTROKES : byte
        {
            ENTER = 0,
            ESC = 1,
            UP = 2,
            DOWN = 3,
            LEFT = 4,
            RIGHT = 5
        }
        #endregion Enums

        #region Top Navigation
        #region Members
        #region File IO
        private String Nav_userRoutesFilePath = ".\\Routes\\";
        private String Nav_userTripsFilePath = ".\\Routes\\";
        private String Nav_userRoutesFileName = "User_Routes.xml";
        private String Nav_userTripsFileName = "User_Trips.xml";
        #endregion File IO
        #region Trip/Route ID Mapping
        //This holds the names of each individual route the user saved
        private List<String> Nav_userRouteNames = null;
        //This holds the RouteID of the routes as values with the route name as the key.
        private Dictionary<String, UInt32> Nav_userRoutesIdMap = null;
        //A complete list of all user routes tags.
        private List<String> Nav_userRoutesTagsList = null;
        //This holds the Route Tags of the routes as values with the route name as the key.
        private Dictionary<String, String> Nav_userRoutesTagsMap = null;
        //This holds a list of zones contained in each route with the route name as the key.
        private Dictionary<String, List<ushort>> Nav_userRoutesZoneMap = null;
        private Dictionary<String, UInt32> Nav_userTripsIdMap = null;
        private List<String> Nav_userTripsTagsList = null;
        private Dictionary<String, String> Nav_userTripsTagsMap = null;
        private Dictionary<String, List<ushort>> Nav_userTripsZoneMap = null;
        private Dictionary<String, List<ushort>> Nav_userTripsLastZoneMap = null;
        private String Nav_sortingUntaggedText = "Untagged";
        private String Nav_sortingNoZoneText = "* No Zones *";
        private UInt32 Nav_maxRouteId = 0;
        private UInt32 Nav_maxTripId = 0;
        #endregion Trip/Route ID Mapping
        #endregion Members
        #region Inits
        private void doNavInits()
        {
            Nav_initDB();
            Nav_loadUserRoutes();
            Nav_loadUserTrips();
            Nav_createUserRoutesLists();
            Nav_createUserTripsLists();
            Nav_setMaxRouteId();
            Nav_setMaxTripId();
            Nav_loadUserSettings();
            //Do GUI inits after all of the data is loaded.
            Nav_Rec_inits();
            Nav_Prc_inits();
        }
        private void Nav_initDB()
        {
            Statics.Datasets.InitRoutesDb();
            foreach (DataTable table in Statics.Datasets.RoutesDb.Tables)
            {
                table.Rows.Clear();
            }
        }
        private void Nav_createUserRoutesLists()
        {
            if (Nav_userRouteNames == null)
            {
                Nav_userRouteNames = new List<string>();
            }
            else
            {
                Nav_userRouteNames.Clear();
            }
            if (Nav_userRoutesIdMap == null)
            {
                Nav_userRoutesIdMap = new Dictionary<string, uint>();
            }
            else
            {
                Nav_userRoutesIdMap.Clear();
            }
            if (Nav_userRoutesTagsMap == null)
            {
                Nav_userRoutesTagsMap = new Dictionary<string, string>();
            }
            else
            {
                Nav_userRoutesTagsMap.Clear();
            }
            if (Nav_userRoutesTagsList == null)
            {
                Nav_userRoutesTagsList = new List<string>();
            }
            else
            {
                Nav_userRoutesTagsList.Clear();
            }
            if (Nav_userRoutesZoneMap == null)
            {
                Nav_userRoutesZoneMap = new Dictionary<string, List<ushort>>();
            }
            else
            {
                Nav_userRoutesZoneMap.Clear();
            }
            //Go thru each row in the table and add it to the list if the list doesn't already contain the route name.
            String filter = "NodeID=0";
            //String filter = "";
            String orderBy = "RouteName";
            Routes.UserRoutesRow[] routeStartNodes = (Routes.UserRoutesRow[])Statics.Datasets.RoutesDb._UserRoutes.Select(filter, orderBy);
            foreach (Routes.UserRoutesRow row in routeStartNodes)
            {
                if ((row.RouteName != "") && !Nav_userRouteNames.Contains(row.RouteName))
                {
                    Nav_userRouteNames.Add(row.RouteName);
                    Nav_userRoutesIdMap.Add(row.RouteName, row.RouteID);
                    if (row.IsRouteTagsNull())
                    {
                        Nav_userRoutesTagsMap.Add(row.RouteName, "");
                    }
                    else
                    {
                        Nav_userRoutesTagsMap.Add(row.RouteName, row.RouteTags);
                        Nav_mergeRouteTagsIntoList(row.RouteTags);
                    }
                }
                List<ushort> zoneList = Nav_getRouteZones(row.RouteName);
                Nav_userRoutesZoneMap[row.RouteName] = zoneList;
            }
        }
        private void Nav_createUserTripsLists()
        {
            
            Statics.Datasets.UserTripNames.Clear();
            if(Nav_userTripsIdMap == null)
            {
                Nav_userTripsIdMap = new Dictionary<string, uint>();
            }
            else
            {
                Nav_userTripsIdMap.Clear();
            }
            if (Nav_userTripsTagsList == null)
            {
                Nav_userTripsTagsList = new List<string>();
            }
            else
            {
                Nav_userTripsTagsList.Clear();
            }
            if (Nav_userTripsTagsMap == null)
            {
                Nav_userTripsTagsMap = new Dictionary<string, string>();
            }
            else
            {
                Nav_userTripsTagsMap.Clear();
            }
            if (Nav_userTripsZoneMap == null)
            {
                Nav_userTripsZoneMap = new Dictionary<string, List<ushort>>();
            }
            else
            {
                Nav_userTripsZoneMap.Clear();
            }
            if (Nav_userTripsLastZoneMap == null)
            {
                Nav_userTripsLastZoneMap = new Dictionary<string, List<ushort>>();
            }
            else
            {
                Nav_userTripsLastZoneMap.Clear();
            }
            //Go thru each row in the table and add it to the list if the list doesn't already contain the route name.
            String filter = "RouteSequenceID=0";
            //String filter = "";
            String orderBy = "TripName";
            Routes.UserTripsRow[] tripStartNodes = (Routes.UserTripsRow[])Statics.Datasets.RoutesDb.UserTrips.Select(filter, orderBy);
            foreach (Routes.UserTripsRow row in tripStartNodes)
            {
                if ((row.TripName != "") && !Statics.Datasets.UserTripNames.Contains(row.TripName))
                {
                    Statics.Datasets.UserTripNames.Add(row.TripName);
                    Nav_userTripsIdMap.Add(row.TripName, row.TripID);
                }
                List<ushort> zoneList = Nav_getTripZones(row.TripID);
                Nav_userTripsZoneMap[row.TripName] = zoneList;
                zoneList = Nav_getTripFinalZone(row.TripID);
                Nav_userTripsLastZoneMap[row.TripName] = zoneList;
                if (!row.IsTripTagsNull())
                {
                    Nav_mergeTripTagsIntoList(row.TripTags);
                    Nav_userTripsTagsMap[row.TripName] = row.TripTags;
                }
                else
                {
                    Nav_userTripsTagsMap[row.TripName] = "";
                }
            }
        }
        private void Nav_loadUserSettings()
        {
            Statics.Settings.Navigation.TripCompleteSound = (String)UserSettings.GetValue(UserSettings.BOT.NAV, "Nav_TripCompleteSound");
            Statics.Settings.Navigation.SortingTagsFirst = (bool)UserSettings.GetValue(UserSettings.BOT.NAV, "Nav_sortingTagsFirst");
            Statics.Settings.Navigation.SortingUseLastZoneOnly = (bool)UserSettings.GetValue(UserSettings.BOT.NAV, "Nav_sortingUseLastZoneOnly");
            Nav_Prc_loadUserSettings();
            Nav_Rec_loadUserSettings();
        }
        #endregion Inits
        #region Utility Functions
        #region File IO
        #region User Routes
        private void Nav_loadUserRoutes()
        {
            if (Directory.Exists(Nav_userRoutesFilePath))
            {
                if (File.Exists(Nav_userRoutesFilePath + Nav_userRoutesFileName))
                {
                    try
                    {
                        if (Statics.Datasets.RoutesDb == null)
                        {
                            Nav_initDB();
                        }
                        Statics.Datasets.RoutesDb._UserRoutes.ReadXml(Nav_userRoutesFilePath + Nav_userRoutesFileName);
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("In Nav_loadUserRoutes: " + e.ToString());
                    }
                }
            }
        }
        private void Nav_saveUserRoutes()
        {
            if (!Directory.Exists(Nav_userRoutesFilePath))
            {
                try
                {
                    Directory.CreateDirectory(Nav_userRoutesFilePath);
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("In Nav_saveUserRoutes creating directory: " + e.ToString());
                    return;
                }
            }
            try
            {
                if (Statics.Datasets.RoutesDb != null)
                {
                    Statics.Datasets.RoutesDb._UserRoutes.WriteXml(Nav_userRoutesFilePath + Nav_userRoutesFileName);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_saveUserRoutes writing file: " + e.ToString());
            }
        }
        #endregion User Routes
        #region User Trips
        private void Nav_loadUserTrips()
        {
            if (Directory.Exists(Nav_userTripsFilePath))
            {
                if (File.Exists(Nav_userTripsFilePath + Nav_userTripsFileName))
                {
                    try
                    {
                        if (Statics.Datasets.RoutesDb == null)
                        {
                            Nav_initDB();
                        }
                        Statics.Datasets.RoutesDb.UserTrips.ReadXml(Nav_userTripsFilePath + Nav_userTripsFileName);
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("In Nav_loadUserTrips: " + e.ToString());
                    }
                }
            }
        }
        private void Nav_saveUserTrips()
        {
            if (!Directory.Exists(Nav_userTripsFilePath))
            {
                try
                {
                    Directory.CreateDirectory(Nav_userTripsFilePath);
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("In Nav_saveUserTrips creating directory: " + e.ToString());
                    return;
                }
            }
            try
            {
                if (Statics.Datasets.RoutesDb != null)
                {
                    Statics.Datasets.RoutesDb.UserTrips.WriteXml(Nav_userTripsFilePath + Nav_userTripsFileName);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_saveUserTrips writing file: " + e.ToString());
            }
        }
        #endregion User Trips
        #endregion File IO
        #region Encoding/Decoding
        #region Node Details
        internal static String Nav_encodePosToString(double iPosX, double iPosY, float iPosH, UInt16 iZone)
        {
            String detailString = "GoTo " + iZone.ToString();
            detailString += " (" + String.Format("{0:0.0}", iPosX) + ", ";
            detailString += String.Format("{0:0.0}", iPosY) + ") ";
            detailString += "H: " + String.Format("{0:0.00}", iPosH);
            return detailString;
        }
        internal static bool Nav_decodeStringToPos(String iText, ref double oPosX, ref double oPosY, ref float oPosH, ref UInt16 oZone)
        {
            Regex regex1 = new Regex("GoTo ([0-9]*) \\(([-0-9.]*), ([-0-9.]*)\\) H: ([-0-9.]*)");
            Match match1;
            try
            {
                match1 = regex1.Match(iText);
                if (!match1.Success)
                {
                    return false;
                }
                else if (match1.Groups.Count != 5)
                {
                    return false;
                }
                else
                {
                    if (!UInt16.TryParse(match1.Groups[1].ToString(), out oZone))
                    {
                        return false;
                    }
                    else if (!double.TryParse(match1.Groups[2].ToString(), out oPosX))
                    {
                        return false;
                    }
                    else if (!double.TryParse(match1.Groups[3].ToString(), out oPosY))
                    {
                        return false;
                    }
                    else if (!float.TryParse(match1.Groups[4].ToString(), out oPosH))
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_decodeStringToPos: " + e.ToString());
                return false;
            }
        }
        internal static String Nav_encodeNameToString(String iName)
        {
            return "Target " + iName;
        }
        internal static bool Nav_decodeStringToNpcName(String iText, ref String oName)
        {
            if (iText.Contains("Target ") && (iText.Length >= 10))
            {
                oName = iText.Substring(7);
                return true;
            }
            else
            {
                return false;
            }
        }
        internal static String Nav_encodeItemToString(String iName, Byte iQuan)
        {
            return "Trade " + iName + " x" + iQuan;
        }
        internal static bool Nav_decodeStringToItem(String iText, ref String oName, ref Byte oQuan)
        {
            Regex regex1 = new Regex("Trade ([ A-Za-z]*) x([0-9]*)");
            Match match1;
            try
            {
                match1 = regex1.Match(iText);
                if (!match1.Success)
                {
                    return false;
                }
                else if (match1.Groups.Count != 3)
                {
                    //String text = "Match groups are: \n";
                    //for (int ii = 0; ii < match1.Groups.Count; ii++)
                    //{
                    //    text += ii.ToString() + ". " + match1.Groups[ii].ToString() + "\n";
                    //}
                    //MessageBox.Show(text);
                    return false;
                }
                else
                {
                    oName = match1.Groups[1].ToString();
                    if (!Byte.TryParse(match1.Groups[2].ToString(), out oQuan))
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_decodeStringToItem: " + e.ToString());
                return false;
            }
        }
        internal static String Nav_encodeGilToString(UInt32 iQuan)
        {
            return "Trade " + iQuan + " Gil";
        }
        internal static bool Nav_decodeStringToGil(String iText, ref UInt32 oQuan)
        {
            Regex regex1 = new Regex("Trade ([0-9]*) Gil");
            Match match1;
            try
            {
                match1 = regex1.Match(iText);
                if (!match1.Success)
                {
                    return false;
                }
                else if (match1.Groups.Count != 2)
                {
                    return false;
                }
                else
                {
                    if (!UInt32.TryParse(match1.Groups[1].ToString(), out oQuan))
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_decodeStringToGil: " + e.ToString());
                return false;
            }
        }
        internal static String Nav_encodeKeystrokeToString(String iKeys)
        {
            return "Press " + iKeys;
        }
        internal static bool Nav_decodeStringToKeystroke(String iText, ref String oKeys)
        {
            if (iText.Contains("Press ") && (iText.Length >= 7))
            {
                oKeys = iText.Substring(6);
                return true;
            }
            else
            {
                return false;
            }
        }
        internal static String Nav_encodeWaitToString(double iTime)
        {
            return "Wait " + iTime.ToString() + " seconds";
        }
        internal static bool Nav_decodeStringToWait(String iText, ref double oTime)
        {
            Regex regex1 = new Regex("Wait ([0-9\\.]*) seconds");
            Match match1;
            try
            {
                match1 = regex1.Match(iText);
                if (!match1.Success)
                {
                    return false;
                }
                else if (match1.Groups.Count != 2)
                {
                    return false;
                }
                else
                {
                    if (!double.TryParse(match1.Groups[1].ToString(), out oTime))
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_decodeStringToWait: " + e.ToString());
                return false;
            }
        }
        #endregion Node Details
        #region Tags
        private List<String> Nav_getTagList(String iTags)
        {
            List<String> tagList = new List<string>();
            String[] tags = iTags.Split(',');
            foreach (String tag in tags)
            {
                String trimmedTag = tag.Trim();
                if (!tagList.Contains(trimmedTag))
                {
                    tagList.Add(trimmedTag);
                }
            }
            return tagList;
        }
        #endregion Tags
        #endregion Encoding/Decoding
        #region Route Lists/Maps
        private void Nav_insertUserRouteName(String iName, uint iRouteID, String iRouteTags)
        {
            if (Nav_userRouteNames.Contains(iName))
            {
                Nav_userRoutesTagsMap[iName] = iRouteTags;
                return;
            }
            Nav_userRouteNames.Add(iName);
            Nav_userRoutesIdMap.Add(iName, iRouteID);
            Nav_userRoutesTagsMap.Add(iName, iRouteTags);
            Nav_userRouteNames.Sort();
        }
        private void Nav_mergeRouteTagsIntoList(String iTags)
        {
            String[] tags = iTags.Split(',');
            foreach (String tag in tags)
            {
                String trimmedTag = tag.Trim();
                if (!Nav_userRoutesTagsList.Contains(trimmedTag))
                {
                    Nav_userRoutesTagsList.Add(trimmedTag);
                }
            }
        }
        private void Nav_mergeRouteZonesIntoMap(Nav_Route iRoute)
        {
            List<ushort> zones = Nav_getRouteZones(iRoute.RouteNodes.ToArray());
            if (zones != null)
            {
                Nav_userRoutesZoneMap[iRoute.RouteName] = zones;
            }
        }
        #endregion Route Lists/Maps
        #region Trip Lists/Maps
        private void Nav_insertUserTripName(String iName, uint iTripID, String iTripTags)
        {
            if (Statics.Datasets.UserTripNames.Contains(iName))
            {
                Nav_userTripsTagsMap[iName] = iTripTags;
                return;
            }
            Statics.Datasets.UserTripNames.Add(iName);
            Nav_userTripsIdMap.Add(iName, iTripID);
            Nav_userTripsTagsMap.Add(iName, iTripTags);
            Statics.Datasets.UserTripNames.Sort();
        }
        private void Nav_mergeTripTagsIntoList(String iTags)
        {
            String[] tags = iTags.Split(',');
            foreach (String tag in tags)
            {
                String trimmedTag = tag.Trim();
                if (!Nav_userTripsTagsList.Contains(trimmedTag))
                {
                    Nav_userTripsTagsList.Add(trimmedTag);
                }
            }
        }
        private void Nav_mergeTripZonesIntoMap(Nav_Trip iTrip)
        {
            List<ushort> zones = Nav_getTripZones(iTrip.TripNodes.ToArray());
            if (zones != null)
            {
                Nav_userTripsZoneMap[iTrip.TripName] = zones;
            }
        }
        #endregion Trip Lists/Maps
        #region Route/Trip Formatting
        #region Routes
        private static Nav_Route Nav_cloneRoute(Nav_Route iRoute)
        {
            return Nav_cloneRoute(iRoute, 0, true);
        }
        private static Nav_Route Nav_cloneRoute(Nav_Route iRoute, int iStartNode, bool iIndexToEnd)
        {
            //iIndexToEnd means that we start and iStartNode index and copy to the end.
            //If it's false, we start at the beginning and go to iStartNode index (for reversed routes).
            List<Routes.UserRoutesRow> routeNodes = iRoute.RouteNodes;
            Nav_Route oRoute = new Nav_Route(iRoute.RouteName, iRoute.RouteID, iRoute.Direction);
            int nodeStartIdx = iIndexToEnd ? iStartNode : 0;
            int nodeEndIdx = iIndexToEnd ? routeNodes.Count - 1 : iStartNode;

            for (int ii = nodeStartIdx; ii <= nodeEndIdx; ii++)
            {
                Routes.UserRoutesRow row = Statics.Datasets.RoutesDb._UserRoutes.NewUserRoutesRow();
                row.ItemArray = (object[])routeNodes[ii].ItemArray.Clone();
                oRoute.RouteNodes.Add(row);
            }
            return oRoute;
        }
        private bool Nav_canReverseRoute(Nav_Route iRoute)
        {
            List<Routes.UserRoutesRow> routeNodes = iRoute.RouteNodes;
            for (int ii = 0; ii < routeNodes.Count; ii++)
            {
                if ((routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.KEYSTROKE)
                    || (routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.NPC_TARGET)
                    || (routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.NPC_TRADE_GIL)
                    || (routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.NPC_TRADE_ITEM))
                {
                    return false;
                }
            }
            return true;
        }
        private void Nav_setMaxRouteId()
        {
            //We have to assume that we've already loaded the user routes from the xml file (during the inits).
            //So we just need to do a select and sort by the route id descending order and take row 0.
            String orderBy = "RouteID DESC";
            Routes.UserRoutesRow[] sortedRows = (Routes.UserRoutesRow[])Statics.Datasets.RoutesDb._UserRoutes.Select("", orderBy);
            if (sortedRows.Length == 0)
            {
                Nav_maxRouteId = 0;
            }
            else
            {
                Nav_maxRouteId = sortedRows[0].RouteID;
            }
        }
        private bool Nav_formatRoute(Nav_Route iRoute,
                                     String iRouteName,
                                     String iRouteTags)
        {
            return Nav_formatRoute(iRoute, iRouteName, iRouteTags, false, false);
        }
        private bool Nav_formatRoute(Nav_Route iRoute, bool iReverseTypes, bool iFlipHeadings)
        {
            String tags;
            if (iRoute.RouteNodes[0].IsRouteTagsNull())
            {
                tags = "";
            }
            else
            {
                tags = iRoute.RouteNodes[0].RouteTags;
            }
            return Nav_formatRoute(iRoute, iRoute.RouteName, tags, iReverseTypes, iFlipHeadings);
        }
        private bool Nav_formatRoute(Nav_Route iRoute,
                                     String iRouteName,
                                     String iRouteTags,
                                     bool iReverseTypes,
                                     bool iFlipHeadings)
        {
            //iReverseTypes simply specifies the direction we're looping through the route.
            //iFlipHeadings specifies whether we're going to change the headings
            //by 180 degress or not.
            //For instance, if we're loading a route and the direction is reversed, both
            //parameters will be true.
            //But if the Forward RB was checked (route was previously reversed)
            //then we will go forward through the loop, but we will still need to
            //flip the headings by 180 degrees because they were in reverse direction to start with.
            List<Routes.UserRoutesRow> routeNodes = iRoute.RouteNodes;
            iRoute.RouteID = routeNodes[0].RouteID;
            //We need to go thru the entire route and make sure of a few things:
            //1. The first point has the strings saved to it.
            //2. Each beginning position node in a position string needs to be set as a start node.
            //3. Each final position node in a position string needs to be set as an end node.
            //4. Make sure we update any position nodes where the next node is in a different zone
            //   to make the previous one a zone node.
            if (routeNodes.Count == 0)
            {
                return false;
            }
            int firstNodeIdx = iReverseTypes ? iRoute.RouteNodes.Count - 1 : 0;
            int lastNodeIdx = iReverseTypes ? 0 : iRoute.RouteNodes.Count - 1;
            int previousNodeIdx = 0;
            int loopIncAmt = iReverseTypes ? -1 : 1;

            //Check for the case that we changed the name of a previously saved route.
            //In this case we need to change the name in the combo box and route map as well.
            if ((routeNodes[0].RouteName != "") && (routeNodes[0].RouteName != iRouteName))
            {
                //It's not new and we changed the name.
                Nav_userRouteNames.Remove(routeNodes[0].RouteName);
                Nav_userRoutesIdMap.Remove(routeNodes[0].RouteName);
                Nav_userRouteNames.Add(iRouteName);
                Nav_userRoutesIdMap.Add(iRouteName, iRoute.RouteID);
                Nav_userRouteNames.Sort();
            }
            //1. The first point has the strings saved to it.
            iRoute.RouteName = iRouteName;
            routeNodes[0].RouteName = iRouteName;
            routeNodes[0].RouteStartName = "";
            routeNodes[0].RouteEndName = "";
            routeNodes[0].RouteTags = iRouteTags;
            
            //2. Each beginning position node in a position string needs to be set as a start node.
            //3. Each final position node in a position string needs to be set as an end node.
            bool previousWasPosition = true;
            for (int ii = firstNodeIdx; ((iReverseTypes == false) && (ii <= lastNodeIdx)) || ((iReverseTypes == true) && (ii >= lastNodeIdx)); ii += loopIncAmt)
            {
                if ((routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_ZONE)
                    || (routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_START)
                    || (routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_NODE)
                    || (routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_END))
                {
                    //Start by clearing out any zone, start, or end types. We'll reset these later, but we want to start fresh.
                    routeNodes[ii].NodeType = (ushort)NAV_NODE_TYPE.POS_NODE;
                    //If we're flipping a route around (either was forward, now reverse
                    //or was reverse, now forward), we also need to reverse the headings by 180.
                    if (iFlipHeadings)
                    {
                        if (routeNodes[ii].NodePosHeading >= 0)
                        {
                            routeNodes[ii].NodePosHeading -= (float)Math.PI;
                        }
                        else
                        {
                            routeNodes[ii].NodePosHeading += (float)Math.PI;
                        }
                    }
                }
                if (ii == firstNodeIdx)
                {
                    if ((routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_NODE)
                     || (routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_END)
                     || (routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_START))
                    {
                        //First node in route was a position, so mark it as a start position.
                        routeNodes[ii].NodeType = (ushort)NAV_NODE_TYPE.POS_START;
                        previousWasPosition = true;
                    }
                    else
                    {
                        previousWasPosition = false;
                    }
                }
                else if ((ii == lastNodeIdx) && ((routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_NODE)
                                              || (routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_START)
                                              || (routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_END)))
                {
                    //Last node in route was a position, so mark it as an end position.
                    routeNodes[ii].NodeType = (ushort)NAV_NODE_TYPE.POS_END;
                }
                else if ((previousWasPosition == true) && (routeNodes[ii].NodeType != (ushort)NAV_NODE_TYPE.POS_NODE)
                                                       && (routeNodes[ii].NodeType != (ushort)NAV_NODE_TYPE.POS_START)
                                                       && (routeNodes[ii].NodeType != (ushort)NAV_NODE_TYPE.POS_END))
                {
                    //This is not a position node, but last one was, so mark the last one as an end position.
                    routeNodes[previousNodeIdx].NodeType = (ushort)NAV_NODE_TYPE.POS_END;
                    previousWasPosition = false;
                }
                else if ((previousWasPosition == false) && ((routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_NODE)
                                                         || (routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_END)
                                                         || (routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_START)))
                {
                    //This is a position node, but last one was not, so mark this one as a start position.
                    routeNodes[ii].NodeType = (ushort)NAV_NODE_TYPE.POS_START;
                    previousWasPosition = true;
                }
                else if (routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_NODE)
                {
                    //This is a position node, but so was the last one, so it's either a node in the middle of a route
                    //(and we'll leave the type as a POS_NODE) or it's an end point and we'll set it as an end point
                    //the next time through the loop when we see that the next one isn't a position node.
                    previousWasPosition = true;
                }
                else
                {
                    //This is not a position node and neither was the last one because it would have been caught above.
                    //So we leave the type as is and just set the flag to false.
                    previousWasPosition = false;
                }
                previousNodeIdx = ii;
            }

            //4. Make sure we update any position nodes where the next node is in a different zone
            //   to make the previous one a zone node.
            firstNodeIdx = iReverseTypes ? iRoute.RouteNodes.Count-2 : 1;
            lastNodeIdx = iReverseTypes ? 0 : iRoute.RouteNodes.Count-1;
            previousNodeIdx = iReverseTypes ? iRoute.RouteNodes.Count - 1 : 0;
            loopIncAmt = iReverseTypes ? -1 : 1;
            for (int ii = firstNodeIdx; ((iReverseTypes == false) && (ii <= lastNodeIdx)) || ((iReverseTypes == true) && (ii >= lastNodeIdx)); ii += loopIncAmt)
            {
                if (routeNodes[ii].NodeZoneID != routeNodes[previousNodeIdx].NodeZoneID)
                {
                    if (((routeNodes[previousNodeIdx].NodeType == (ushort)NAV_NODE_TYPE.POS_START)
                        || (routeNodes[previousNodeIdx].NodeType == (ushort)NAV_NODE_TYPE.POS_NODE)
                        || (routeNodes[previousNodeIdx].NodeType == (ushort)NAV_NODE_TYPE.POS_END))
                        && ((routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_START)
                        || (routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_NODE)
                        || (routeNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_END)))
                    {
                        //Both the last point and this point are position nodes
                        //AND the last zone is not the same as this one.
                        //Mark the previous node as a zone node.
                        routeNodes[previousNodeIdx].NodeType = (ushort)NAV_NODE_TYPE.POS_ZONE;
                    }
                }
                previousNodeIdx = ii;
            }
            return true;
        }
        private Nav_Route Nav_createRoute(String iRouteName, bool iDirection)
        {
            if (!Nav_userRouteNames.Contains(iRouteName))
            {
                MessageBox.Show("Could not find records for route '" + iRouteName + "'", "No Route Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            UInt32 routeId = Nav_userRoutesIdMap[iRouteName];
            return Nav_createRoute(routeId, iDirection);
        }
        private Nav_Route Nav_createRoute(UInt32 iRouteId, bool iDirection)
        {
            String filter = "RouteID=" + iRouteId.ToString();
            String orderBy = "NodeID";
            Routes.UserRoutesRow[] routeNodes = (Routes.UserRoutesRow[])Statics.Datasets.RoutesDb._UserRoutes.Select(filter, orderBy);
            if (routeNodes.Length <= 0)
            {
                MessageBox.Show("Could not find records for route ID " + iRouteId, "No Route Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            String routeName = routeNodes[0].RouteName;
            Nav_Route oRoute = new Nav_Route(routeName, iRouteId, iDirection);
            oRoute.RouteNodes.AddRange(routeNodes);
            return oRoute;
        }
        #endregion Routes
        #region Trips
        private void Nav_setMaxTripId()
        {
            //We have to assume that we've already loaded the user trips from the xml file (during the inits).
            //So we just need to do a select and sort by the trip id descending order and take row 0.
            String filter = "RouteSequenceID=0";
            String orderBy = "TripID DESC";
            Routes.UserTripsRow[] sortedRows = (Routes.UserTripsRow[])Statics.Datasets.RoutesDb.UserTrips.Select(filter, orderBy);
            if (sortedRows.Length == 0)
            {
                Nav_maxTripId = 0;
            }
            else
            {
                Nav_maxTripId = sortedRows[0].TripID;
            }
        }
        private bool Nav_checkForTripsWithReverseRoute(Nav_Route iRoute)
        {
            return Nav_checkForTripsWithReverseRoute(iRoute.RouteID);
        }
        private bool Nav_checkForTripsWithReverseRoute(UInt32 iRouteId)
        {
            String filter = "RouteID=" + iRouteId + " AND Direction=false";
            Routes.UserTripsRow[] trips = (Routes.UserTripsRow[])Statics.Datasets.RoutesDb.UserTrips.Select(filter);
            if (trips.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static bool Nav_checkTeleportationCommand(Nav_Trip iTrip, ref int oStartRoute, ref int oStartNode)
        {
            for (int ii = 0; ii < iTrip.TripRoutes.Count; ii++)
            {
                int nodeStartIdx = iTrip.TripRoutes[ii].Direction ? 0 : iTrip.TripRoutes[ii].RouteNodes.Count - 1;
                int nodeEndIdx = iTrip.TripRoutes[ii].Direction ? iTrip.TripRoutes[ii].RouteNodes.Count - 1 : 0;
                int loopIncValue = iTrip.TripRoutes[ii].Direction ? 1 : -1;
                for (int kk = nodeStartIdx; ((iTrip.TripRoutes[ii].Direction && (kk <= nodeEndIdx)) || (!iTrip.TripRoutes[ii].Direction && (kk >= nodeEndIdx))); kk += loopIncValue)
                {
                    if (iTrip.TripRoutes[ii].RouteNodes[kk].NodeType == (ushort)NAV_NODE_TYPE.COMMAND)
                    {
                        String detail = iTrip.TripRoutes[ii].RouteNodes[kk].NodeDetail;
                        if (detail.ToLower().Contains("teleport")
                            || detail.ToLower().Contains("recall")
                            || detail.ToLower().Contains("warp")
                            || detail.ToLower().Contains("retrace")
                            || detail.ToLower().Contains("escape"))
                        {
                            oStartRoute = ii;
                            oStartNode = kk;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private Nav_Trip Nav_getTrip(String iTripName)
        {
            //First we'll get the trip rows and add them to the new Nav_Trip object.
            //Then we'll create the Nav_Route's and add them to the Nav_Trip object.
            //Then we'll set the name and ID of the trip and return it.
            Nav_Trip localTrip = new Nav_Trip();
            #region Add Trip Rows
            Routes.UserTripsRow[] routes = Nav_getTripRows(iTripName);
            if (routes == null)
            {
                return null;
            }
            localTrip.TripNodes.AddRange(routes);
            #endregion Add Trip Rows
            #region Add Nav_Route's
            for (int ii = 0; ii < localTrip.TripNodes.Count; ii++)
            {
                UInt32 routeId = localTrip.TripNodes[ii].RouteID;
                bool direction = localTrip.TripNodes[ii].Direction;
                Nav_Route newRoute = Nav_cloneRoute(Nav_createRoute(routeId, direction));
                if (!Nav_formatRoute(newRoute, !newRoute.Direction, !newRoute.Direction))
                {
                    return null;
                }
                if (newRoute != null)
                {
                    localTrip.TripRoutes.Add(newRoute);
                }
            }
            #endregion Add Nav_Route's
            #region Set name, ID, & tags
            localTrip.TripName = localTrip.TripNodes[0].TripName;
            localTrip.TripID = localTrip.TripNodes[0].TripID;
            if (!localTrip.TripNodes[0].IsTripTagsNull())
            {
                localTrip.TripTags = localTrip.TripNodes[0].TripTags;
            }
            else
            {
                localTrip.TripTags = "";
            }
            #endregion Set name and ID
            return localTrip;
        }
        private Routes.UserTripsRow[] Nav_getTripRows(String iTripName)
        {
            String filter = "TripID=" + Nav_userTripsIdMap[iTripName];
            String orderby = "RouteSequenceID";
            Routes.UserTripsRow[] tripRows = (Routes.UserTripsRow[])Statics.Datasets.RoutesDb.UserTrips.Select(filter, orderby);
            return tripRows;
        }
        public static Nav_Trip Nav_getSubTrip(Nav_Trip iTrip, UInt16 iZoneId, float iPosX, float iPosY)
        {
            //This function allows us to start at any point along the trip without having to
            //go all the way back to the beginning. It basically prunes out any nodes in
            //the trip prior to the position we're at right now (passed into the function).
            int startRoute = 0;
            int startNode = 0;
            float minDistSoFar = 100f;
            float lastDist = 100f;
            bool foundNodeClose = false;
            for (int ii = 0; ii < iTrip.TripRoutes.Count; ii++)
            {
                if (foundNodeClose)
                {
                    break;
                }
                int nodeStartIdx = iTrip.TripRoutes[ii].Direction ? 0 : iTrip.TripRoutes[ii].RouteNodes.Count - 1;
                int nodeEndIdx = iTrip.TripRoutes[ii].Direction ? iTrip.TripRoutes[ii].RouteNodes.Count - 1 : 0;
                int loopIncValue = iTrip.TripRoutes[ii].Direction ? 1 : -1;
                for (int kk = nodeStartIdx; ((iTrip.TripRoutes[ii].Direction && (kk <= nodeEndIdx)) || (!iTrip.TripRoutes[ii].Direction && (kk >= nodeEndIdx))); kk += loopIncValue)
                {
                    //Weed out any non-position nodes.
                    if ((iTrip.TripRoutes[ii].RouteNodes[kk].NodeType != (ushort)NAV_NODE_TYPE.POS_END)
                        && (iTrip.TripRoutes[ii].RouteNodes[kk].NodeType != (ushort)NAV_NODE_TYPE.POS_NODE)
                        && (iTrip.TripRoutes[ii].RouteNodes[kk].NodeType != (ushort)NAV_NODE_TYPE.POS_START)
                        && (iTrip.TripRoutes[ii].RouteNodes[kk].NodeType != (ushort)NAV_NODE_TYPE.POS_ZONE))
                    {
                        continue;
                    }
                    //Weed out any points not in the zone.
                    if (iTrip.TripRoutes[ii].RouteNodes[kk].NodeZoneID != iZoneId)
                    {
                        continue;
                    }
                    //The idea here is that we will keep track of any points that are within the margin "Nav_maxDistForRouteStart".
                    //As we get into this range we'll find some minimum distance. As our distance starts getting larger
                    //we can stop there since we know we're moving away.
                    //At first I thought that we would start at the end of the route and go back to the beginning, finding
                    //the closest point to the end of the route in case we crossed the same area twice.
                    //However this would keep us from doing a round trip like if we're fishing => zoning => fishing.
                    //So we start at the beginning and once we find the closest point in range we create the sub trip and return.
                    float distToNode = (float)Nav_getDistance(iPosX, iTrip.TripRoutes[ii].RouteNodes[kk].NodePosX, iPosY, iTrip.TripRoutes[ii].RouteNodes[kk].NodePosY);
                    if (distToNode <= Statics.Constants.Navigation.MaxDistForRouteStart)
                    {
                        foundNodeClose = true;
                        if (distToNode <= minDistSoFar)
                        {
                            //we're getting closer to our location, record this point.
                            minDistSoFar = distToNode;
                            startRoute = ii;
                            startNode = kk;
                        }
                        else
                        {
                            //we're getting farther away from our location, stop here.
                            break;
                        }
                    }
                    lastDist = distToNode;
                }
            }
            if (!foundNodeClose)
            {
                //If we don't find any position nodes in range, check to see
                //if we have an teleportation commands and use that as the starting point.
                if (!Nav_checkTeleportationCommand(iTrip, ref startRoute, ref startNode))
                {
                    MessageBox.Show("Could not find a position node within range of your current location.", "Nothing in Range.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            //Add the routes and trip nodes (rows) to the new subtrip.
            Nav_Trip subTrip = new Nav_Trip();
            for (int ii = startRoute; ii < iTrip.TripRoutes.Count; ii++)
            {
                if (ii == startRoute)
                {
                    subTrip.TripRoutes.Add(Nav_cloneRoute(iTrip.TripRoutes[ii], startNode, iTrip.TripRoutes[ii].Direction));
                }
                else
                {
                    subTrip.TripRoutes.Add(Nav_cloneRoute(iTrip.TripRoutes[ii]));
                }
                //If we haven't yet saved the trip and we're trying to run it directly
                //from the create/edit trip box, there will be no nodes, so we need
                //to skip this part.
                if (iTrip.TripNodes.Count > 0)
                {
                    subTrip.TripNodes.Add(iTrip.TripNodes[ii]);
                }
            }
            subTrip.TripName = iTrip.TripName;
            subTrip.TripID = iTrip.TripID;
            subTrip.TripTags = iTrip.TripTags;
            return subTrip;
        }
        #endregion Trips
        #endregion Route/Trip Formatting
        #region Node Checks
        private bool Nav_nodeIsPosition(Routes.UserRoutesRow iNode)
        {
            if ((iNode.NodeType == (ushort)NAV_NODE_TYPE.POS_START)
             || (iNode.NodeType == (ushort)NAV_NODE_TYPE.POS_NODE)
             || (iNode.NodeType == (ushort)NAV_NODE_TYPE.POS_END)
             || (iNode.NodeType == (ushort)NAV_NODE_TYPE.POS_ZONE))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private List<ushort> Nav_getRouteZones(String iRouteName)
        {
            String filter = "RouteID=" + Nav_userRoutesIdMap[iRouteName];
            String orderby = "NodeID";
            Routes.UserRoutesRow[] routeNodes = (Routes.UserRoutesRow[])Statics.Datasets.RoutesDb._UserRoutes.Select(filter, orderby);
            return Nav_getRouteZones(routeNodes);
        }
        private List<ushort> Nav_getRouteZones(Routes.UserRoutesRow[] iRouteNodes)
        {
            List<ushort> zoneList = new List<ushort>();
            foreach (Routes.UserRoutesRow row in iRouteNodes)
            {
                if (Nav_nodeIsPosition(row))
                {
                    if (!row.IsNodeZoneIDNull() && !zoneList.Contains(row.NodeZoneID))
                    {
                        zoneList.Add(row.NodeZoneID);
                    }
                }
            }
            return zoneList;
        }
        private List<ushort> Nav_getRouteFinalZone(String iRouteName, bool iDirection)
        {
            String filter = "RouteID=" + Nav_userRoutesIdMap[iRouteName];
            String orderby = "NodeID";
            Routes.UserRoutesRow[] routeNodes = (Routes.UserRoutesRow[])Statics.Datasets.RoutesDb._UserRoutes.Select(filter, orderby);
            return Nav_getRouteFinalZone(routeNodes, iDirection);
        }
        private List<ushort> Nav_getRouteFinalZone(Routes.UserRoutesRow[] iRouteNodes, bool iDirection)
        {
            List<ushort> zoneList = new List<ushort>();
            if (iDirection == false)
            {
                for (int ii = 0; ii < iRouteNodes.Length; ii++)
                {
                    if (Nav_nodeIsPosition(iRouteNodes[ii]))
                    {
                        zoneList.Add(iRouteNodes[ii].NodeZoneID);
                        return zoneList;
                    }
                }
            }
            else
            {
                for (int ii = iRouteNodes.Length - 1; ii >= 0; ii--)
                {
                    if (Nav_nodeIsPosition(iRouteNodes[ii]))
                    {
                        zoneList.Add(iRouteNodes[ii].NodeZoneID);
                        return zoneList;
                    }
                }
            }
            return zoneList;
        }
        private List<ushort> Nav_getTripZones(String iTripName)
        {
            return Nav_getTripZones(Nav_userTripsIdMap[iTripName]);
        }
        private List<ushort> Nav_getTripZones(UInt32 iTripID)
        {
            String filter = "TripID=" + iTripID;
            String orderby = "RouteSequenceID";
            Routes.UserTripsRow[] trip = (Routes.UserTripsRow[])Statics.Datasets.RoutesDb.UserTrips.Select(filter, orderby);
            if (trip.Length == 0)
            {
                String msg = "[ERROR] Could not find info for TripID: " + iTripID;
                MessageBox.Show(msg);
                LoggingFunctions.Timestamp(msg);
                return new List<ushort>();
            }
            else
            {
                return Nav_getTripZones(trip);
            }
        }
        private List<ushort> Nav_getTripZones(Routes.UserTripsRow[] iRoutes)
        {
            List<ushort> routeZoneList;
            List<ushort> tripZoneList = new List<ushort>();
            foreach (Routes.UserTripsRow route in iRoutes)
            {
                String routeName;
                String filter = "RouteID=" + route.RouteID + " AND NodeID=0";
                Routes.UserRoutesRow[] routeRowZero = (Routes.UserRoutesRow[])Statics.Datasets.RoutesDb._UserRoutes.Select(filter);
                if (routeRowZero.Length != 1)
                {
                    String msg = "Could not find the route info for route ID: " + route.RouteID;
                    MessageBox.Show(msg);
                    LoggingFunctions.Timestamp(msg);
                    return tripZoneList;
                }
                else
                {
                    routeName = routeRowZero[0].RouteName;
                }
                if (!Nav_userRoutesZoneMap.TryGetValue(routeName, out routeZoneList))
                {
                    Nav_userRoutesZoneMap[routeName] = Nav_getRouteZones(routeName);
                    routeZoneList = Nav_userRoutesZoneMap[routeName];
                }
                //Now we have the route zone list for this route. Run through
                //all of the zones and add them to the trip zone list if they're not
                //already there.
                foreach (ushort zoneId in routeZoneList)
                {
                    if (!tripZoneList.Contains(zoneId))
                    {
                        tripZoneList.Add(zoneId);
                    }
                }
            }
            return tripZoneList;
        }
        private List<ushort> Nav_getTripFinalZone(UInt32 iTripID)
        {
            String filter = "TripID=" + iTripID;
            String orderby = "RouteSequenceID";
            Routes.UserTripsRow[] trip = (Routes.UserTripsRow[])Statics.Datasets.RoutesDb.UserTrips.Select(filter, orderby);
            if (trip.Length == 0)
            {
                String msg = "[ERROR] Could not find info for TripID: " + iTripID;
                MessageBox.Show(msg);
                LoggingFunctions.Timestamp(msg);
                return new List<ushort>();
            }
            else
            {
                return Nav_getTripFinalZone(trip);
            }
        }
        private List<ushort> Nav_getTripFinalZone(Routes.UserTripsRow[] iRoutes)
        {
            List<ushort> routeZoneList = null;
            int nbRoutes = iRoutes.Length;
            for (int ii = nbRoutes - 1; ii >= 0; ii--)
            {
                Routes.UserTripsRow route = iRoutes[ii];
                bool direction = route.Direction;
                String routeName;
                String filter = "RouteID=" + route.RouteID + " AND NodeID=0";
                Routes.UserRoutesRow[] routeRowZero = (Routes.UserRoutesRow[])Statics.Datasets.RoutesDb._UserRoutes.Select(filter);
                if (routeRowZero.Length != 1)
                {
                    String msg = "Could not find the route info for route ID: " + route.RouteID;
                    MessageBox.Show(msg);
                    LoggingFunctions.Timestamp(msg);
                    routeZoneList = new List<ushort>();
                    return routeZoneList;
                }
                else
                {
                    routeName = routeRowZero[0].RouteName;
                }
                routeZoneList = Nav_getRouteFinalZone(routeName, direction);
                if (routeZoneList.Count > 0)
                {
                    //We found a zone in this route, so just return it.
                    //Otherwise we'll keep searching through the rest of the routes.
                    return routeZoneList;
                }
            }
            routeZoneList = new List<ushort>();
            return routeZoneList;
        }
        #endregion Node Checks
        #region Distance & Time Calculations
        internal static double Nav_getRouteDistance(Nav_Route iRoute)
        {
            double totalDist = 0d;
            int nodeStartIdx = iRoute.Direction ? 0 : iRoute.RouteNodes.Count - 1;
            int nodeEndIdx = iRoute.Direction ? iRoute.RouteNodes.Count - 2 : 1;
            int loopInc = iRoute.Direction ? 1 : -1;
            int nextIdx = iRoute.Direction ? 1 : iRoute.RouteNodes.Count - 2;
            for (int ii = nodeStartIdx; ((iRoute.Direction && (ii <= nodeEndIdx)) || (!iRoute.Direction && (ii >= nodeEndIdx))); ii += loopInc)
            {
                //We will find the distance between the current node
                //and the next node if they are both position nodes.
                //If the current node is a zone node or a non-position
                //node, we'll skip it.
                if (((iRoute.RouteNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_START)
                    || (iRoute.RouteNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_NODE)
                    || (iRoute.RouteNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_END))
                    && ((iRoute.RouteNodes[nextIdx].NodeType == (ushort)NAV_NODE_TYPE.POS_START)
                    || (iRoute.RouteNodes[nextIdx].NodeType == (ushort)NAV_NODE_TYPE.POS_NODE)
                    || (iRoute.RouteNodes[nextIdx].NodeType == (ushort)NAV_NODE_TYPE.POS_END)
                    || (iRoute.RouteNodes[nextIdx].NodeType == (ushort)NAV_NODE_TYPE.POS_ZONE)))
                {
                    totalDist += Nav_getDistance(iRoute.RouteNodes[ii].NodePosX,
                                                 iRoute.RouteNodes[nextIdx].NodePosX,
                                                 iRoute.RouteNodes[ii].NodePosY,
                                                 iRoute.RouteNodes[nextIdx].NodePosY);
                }
                nextIdx += loopInc; ;
            }
            return totalDist;
        }
        internal static UInt32 Nav_getRouteTime(Nav_Route iRoute, double iRouteDistance)
        {
            double routeTime = iRouteDistance / Statics.Constants.Navigation.SpeedWalking;
            //Now we need to add time for each non-position changing node.
            for (int ii = 0; ii < iRoute.RouteNodes.Count; ii++)
            {
                if (iRoute.RouteNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.COMMAND)
                {
                    routeTime += Statics.Constants.Navigation.TimeCommand;
                }
                else if (iRoute.RouteNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.KEYSTROKE)
                {
                    routeTime += Statics.Constants.Navigation.TimeKeystroke;
                }
                else if (iRoute.RouteNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.NPC_TARGET)
                {
                    routeTime += Statics.Constants.Navigation.TimeTargetNpc;
                }
                else if ((iRoute.RouteNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.NPC_TRADE_GIL)
                    || (iRoute.RouteNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.NPC_TRADE_ITEM))
                {
                    routeTime += Statics.Constants.Navigation.TimeTrade;
                }
                else if (iRoute.RouteNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_ZONE)
                {
                    routeTime += Statics.Constants.Navigation.TimeZone;
                }
                else if (iRoute.RouteNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.WAIT)
                {
                    routeTime += iRoute.RouteNodes[ii].NodeData / 1000;
                }
            }
            return (UInt32)routeTime;
        }
        internal static UInt32 Nav_getRouteTime(Nav_Route iRoute)
        {
            double routeDistance = Nav_getRouteDistance(iRoute);
            return Nav_getRouteTime(iRoute, routeDistance);
        }
        internal static double Nav_getTripDistance(Nav_Trip iTrip)
        {
            double totalDist = 0d;
            for (int ii = 0; ii < iTrip.TripRoutes.Count; ii++)
            {
                totalDist += Nav_getRouteDistance(iTrip.TripRoutes[ii]);
            }
            return totalDist;
        }
        internal static UInt32 Nav_getTripTime(Nav_Trip iTrip)
        {
            UInt32 totalTime = 0;
            for (int ii = 0; ii < iTrip.TripRoutes.Count; ii++)
            {
                totalTime += Nav_getRouteTime(iTrip.TripRoutes[ii]);
            }
            return totalTime;
        }
        internal static UInt32 Nav_getWalkingTime(double iDistance)
        {
            return (UInt32)(iDistance / Statics.Constants.Navigation.SpeedWalking);
        }
        internal static double Nav_getDistance(double x0, double x1, double y0, double y1)
        {
            return Math.Sqrt(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2));
        }
        #endregion Distance & Time Calculations
        #region Printing
        internal static void Nav_PrintTrip(Nav_Trip iTrip, bool iOpenFile)
        {
            String fileName = "PrintedTrip_" + iTrip.TripRoutes[0].RouteName + ".txt";
            List<String> strList = new List<string>();
            for (int ii = 0; ii < iTrip.TripRoutes.Count; ii++)
            {
                String routeString = "Trip[" + ii + "]: " + (iTrip.TripRoutes[ii].Direction ? "Forward" : "Reverse") + "\n";
                strList.Add(routeString);
                for (int kk = 0; kk < iTrip.TripRoutes[ii].RouteNodes.Count; kk++)
                {
                    Routes.UserRoutesRow node = iTrip.TripRoutes[ii].RouteNodes[kk];
                    routeString = "node[" + kk + "] zone: " + node.NodeZoneID + ", pos(" + node.NodePosX + ", " + node.NodePosY + ") @" + node.NodePosHeading + ", Type: " + ((NAV_NODE_TYPE)node.NodeType).ToString();
                    strList.Add(routeString);
                }
                strList.Add("\n\n");
            }
            LoggingFunctions.WriteToFile(strList, fileName, iOpenFile);
        }
        internal static void Nav_PrintRoute(Nav_Route iRoute, bool iOpenFile)
        {
            String fileName = "PrintedRoute_" + iRoute.RouteName + ".txt";
            List<String> strList = new List<string>();
            strList.Add("Route '" + iRoute.RouteName + "': ");
            for (int kk = 0; kk < iRoute.RouteNodes.Count; kk++)
            {
                String routeString = "";
                Routes.UserRoutesRow node = iRoute.RouteNodes[kk];
                routeString = "node[" + kk + "] zone: " + node.NodeZoneID + ", pos(" + node.NodePosX + ", " + node.NodePosY + ") @" + node.NodePosHeading + ", Type: " + ((NAV_NODE_TYPE)node.NodeType).ToString();
                strList.Add(routeString);
            }
            LoggingFunctions.WriteToFile(strList, fileName, iOpenFile);
        }
        #endregion Printing
        #endregion Utility Functions
        #region Settings Event Handlers
        private void Nav_settingsFormOkClick(object sender, EventArgs e)
        {
            Nav_Prc_loadRouteTV();
            Nav_Prc_loadTripSelectionTV();
        }
        #endregion Settings Event Handlers
        #endregion Top Navigation

        #region Route Processing
        #region Enums
        private enum NAV_PRC_STATE : byte
        {
            STOPPED = 0,
            RUNNING = 1
        }
        #endregion Enums
        #region Members
        #region Misc
        private bool Nav_Prc_modified = false;
        private bool Nav_Prc_reverseCheckedProgrammatically = false;
        private bool Nav_Prc_exisingTripLoaded = false;
        private static int Nav_Prc_maxTVDepth = 4;
        #endregion Misc
        #region Default Values
        private int Nav_Prc_TVDefaultIndent = 4;
        private String Nav_Prc_TripNameTBDefText = "Trip Name";
        private String Nav_Prc_TripTagsTBDefText = "Comma Separated Tags";
        #endregion Default Values
        #region GUI Value Parallels
        private String Nav_Prc_TripName = "";
        private String Nav_Prc_TripTags = "";
        private bool Nav_Prc_Forward = true;
        internal UInt32 Nav_Prc_Loop_Cnt = 1;
        #endregion GUI Value Parallels
        #region Current Route/Trip Values
        private Nav_Trip Nav_Prc_CurrentTrip;
        private Nav_Route Nav_Prc_CurrentRoute;
        #endregion Current Route/Trip Values
        #region Tool Tips
        private ToolTip Nav_Prc_RouteTV_TT = new ToolTip();
        private const String Nav_Prc_RouteTV_TT_Title = "Route TreeView";
        private const String Nav_Prc_RouteTV_TT_Text = "Double click a route to\nadd it to the current trip.\n"
                                                     + "Right click a route or route node to\nprocess that route or node.";
        private ToolTip Nav_Prc_Trip_CreationTV_TT = new ToolTip();
        private const String Nav_Prc_Trip_CreationTV_TT_Title = "Trip Creation TreeView";
        private const String Nav_Prc_Trip_CreationTV_TT_Text = "Press the Delete key to remove the selected route.\n"
                                                             + "Press the Escape key to deselect any routes or route nodes.\n"
                                                             + "Select a route and click the Forward or Reverse radio buttons\n"
                                                             + "to change the direction of that route.\n"
                                                             + "Right click a route or route node to process that route or node.";
        #endregion Tool Tips
        #region Delegates
        #region Route TV
        private delegate void Nav_Prc_loadRouteTVDelegate();
        private delegate void Nav_Prc_setRouteTVNodeDelegate(TreeNode iNode);
        private delegate void Nav_Prc_deselectRouteTVNodeDelegate();
        private delegate void Nav_Prc_refreshRouteTVDelegate();
        private delegate void Nav_Prc_scrollRouteTVDelegate(TreeNode iNode);
        #endregion Route TV
        #region Trip Creation TV
        private delegate void Nav_Prc_loadTripCreationTVDelegate(Nav_Trip iTrip);
        private delegate void Nav_Prc_addTripCreationTVNodeDelegate(Nav_Route iRoute);
        private delegate void Nav_Prc_clearTripCreationTVDelegate();
        private delegate void Nav_Prc_deleteTripCreationTVNodeDelegate(TreeNode iNode);
        private delegate void Nav_Prc_setTripCreationTVNodeDelegate(TreeNode iNode);
        private delegate void Nav_Prc_deselectTripCreationTVNodeDelegate();
        private delegate void Nav_Prc_scrollTripCreationTVDelegate(TreeNode iNode);
        #endregion Trip Creation TV
        #region Trip Selection TV
        private delegate void Nav_Prc_loadTripSelectionTVDelegate();
        private delegate void Nav_Prc_setTripSelectionTVNodeDelegate(TreeNode iNode);
        private delegate void Nav_Prc_refreshTripSelectionTVDelegate();
        private delegate void Nav_Prc_deleteTripSelectionTVNodeDelegate(TreeNode[] iNodes);
        private delegate void Nav_Prc_scrollTripSelectionTVDelegate(TreeNode iNode);
        #endregion Trip Selection TV
        #region Others
        private delegate void Nav_Prc_setTripNameTBTextDelegate(String iText);
        private delegate void Nav_Prc_setTripTagsTBTextDelegate(String iText);
        private delegate void Nav_Prc_setForwardRBDelegate(bool iChk);
        private delegate void Nav_Prc_setReverseRBDelegate(bool iChk);
        internal delegate void Nav_Prc_setStartButtonDelegate(String iText, Color iColor);
        private delegate void Nav_Prc_setRouteDistanceTextDelegate(String iText);
        private delegate void Nav_Prc_setRouteTimeTextDelegate(String iText);
        private delegate void Nav_Prc_setTripDistanceTextDelegate(String iText);
        private delegate void Nav_Prc_setTripTimeTextDelegate(String iText);
        internal delegate void Nav_Prc_setTimeRemainingTextDelegate(UInt32 iTimeMs);
        public delegate void Nav_Prc_setLoopCountDelegate(UInt32 iCount);
        #endregion Others
        #endregion Delegates
        #region Function Pointers
        #region Route TV
        private Nav_Prc_loadRouteTVDelegate Nav_Prc_loadRouteTVPtr;
        private Nav_Prc_setRouteTVNodeDelegate Nav_Prc_setRouteTVNodePtr;
        private Nav_Prc_deselectRouteTVNodeDelegate Nav_Prc_deselectRouteTVNodePtr;
        private Nav_Prc_refreshRouteTVDelegate Nav_Prc_refreshRouteTVPtr;
        private Nav_Prc_scrollRouteTVDelegate Nav_Prc_scrollRouteTVPtr;
        #endregion Route TV
        #region Trip Creation TV
        private Nav_Prc_loadTripCreationTVDelegate Nav_Prc_loadTripCreationTVPtr;
        private Nav_Prc_addTripCreationTVNodeDelegate Nav_Prc_addTripCreationTVNodePtr;
        private Nav_Prc_clearTripCreationTVDelegate Nav_Prc_clearTripCreationTVPtr;
        private Nav_Prc_deleteTripCreationTVNodeDelegate Nav_Prc_deleteTripCreationTVNodePtr;
        private Nav_Prc_setTripCreationTVNodeDelegate Nav_Prc_setTripCreationTVNodePtr;
        private Nav_Prc_deselectTripCreationTVNodeDelegate Nav_Prc_deselectTripCreationTVNodePtr;
        private Nav_Prc_scrollTripCreationTVDelegate Nav_Prc_scrollTripCreationTVPtr;
        #endregion Trip Creation TV
        #region Trip Selection TV
        private Nav_Prc_loadTripSelectionTVDelegate Nav_Prc_loadTripSelectionTVPtr;
        private Nav_Prc_setTripSelectionTVNodeDelegate Nav_Prc_setTripSelectionTVNodePtr;
        private Nav_Prc_refreshTripSelectionTVDelegate Nav_Prc_refreshTripSelectionTVPtr;
        private Nav_Prc_deleteTripSelectionTVNodeDelegate Nav_Prc_deleteTripSelectionTVNodePtr;
        private Nav_Prc_scrollTripSelectionTVDelegate Nav_Prc_scrollTripSelectionTVPtr;
        #endregion Trip Selection TV
        #region Others
        private Nav_Prc_setTripNameTBTextDelegate Nav_Prc_setTripNameTBTextPtr;
        private Nav_Prc_setTripTagsTBTextDelegate Nav_Prc_setTripTagsTBTextPtr;
        private Nav_Prc_setForwardRBDelegate Nav_Prc_setForwardRBPtr;
        private Nav_Prc_setReverseRBDelegate Nav_Prc_setReverseRBPtr;
        private Nav_Prc_setStartButtonDelegate Nav_Prc_setStartButtonPtr;
        private Nav_Prc_setRouteDistanceTextDelegate Nav_Prc_setRouteDistanceTextPtr;
        private Nav_Prc_setRouteTimeTextDelegate Nav_Prc_setRouteTimeTextPtr;
        private Nav_Prc_setTripDistanceTextDelegate Nav_Prc_setTripDistanceTextPtr;
        private Nav_Prc_setTripTimeTextDelegate Nav_Prc_setTripTimeTextPtr;
        private Nav_Prc_setTimeRemainingTextDelegate Nav_Prc_setTimeRemainingTextPtr;
        public Nav_Prc_setLoopCountDelegate Nav_Prc_setLoopCountPtr;
        #endregion Others
        #endregion Function Pointers
        #endregion Members
        #region Inits
        private void Nav_Prc_inits()
        {
            Nav_Prc_createDelegates();
            Nav_Prc_CurrentTrip = new Nav_Trip();
            Nav_Prc_CurrentRoute = new Nav_Route();
            Nav_Prc_setTripNameTBText(Nav_Prc_TripNameTBDefText);
            Nav_Prc_setTripTagsTBText(Nav_Prc_TripTagsTBDefText);
            Nav_Prc_setForwardRB(true);
            Nav_Prc_initTreeViews();
            //Nav_Prc_createToolTips();
        }
        private void Nav_Prc_createDelegates()
        {
            if (Nav_Prc_loadTripSelectionTVPtr == null)
            {
                #region Route TV
                Nav_Prc_loadRouteTVPtr = new Nav_Prc_loadRouteTVDelegate(Nav_Prc_loadRouteTVCallBackFunction);
                Nav_Prc_setRouteTVNodePtr = new Nav_Prc_setRouteTVNodeDelegate(Nav_Prc_setRouteTVNodeCallBackFunction);
                Nav_Prc_deselectRouteTVNodePtr = new Nav_Prc_deselectRouteTVNodeDelegate(Nav_Prc_deselectRouteTVNodeCallBackFunction);
                Nav_Prc_refreshRouteTVPtr = new Nav_Prc_refreshRouteTVDelegate(Nav_Prc_refreshRouteTVCallBackFunction);
                Nav_Prc_scrollRouteTVPtr = new Nav_Prc_scrollRouteTVDelegate(Nav_Prc_scrollRouteTVCallBackFunction);
                #endregion Route TV
                #region Trip Creation TV
                Nav_Prc_loadTripCreationTVPtr = new Nav_Prc_loadTripCreationTVDelegate(Nav_Prc_loadTripCreationTVCallBackFunction);
                Nav_Prc_addTripCreationTVNodePtr = new Nav_Prc_addTripCreationTVNodeDelegate(Nav_Prc_addTripCreationTVNodeCallBackFunction);
                Nav_Prc_clearTripCreationTVPtr = new Nav_Prc_clearTripCreationTVDelegate(Nav_Prc_clearTripCreationTVCallBackFunction);
                Nav_Prc_deleteTripCreationTVNodePtr = new Nav_Prc_deleteTripCreationTVNodeDelegate(Nav_Prc_deleteTripCreationTVNodeCallBackFunction);
                Nav_Prc_setTripCreationTVNodePtr = new Nav_Prc_setTripCreationTVNodeDelegate(Nav_Prc_setTripCreationTVNodeCallBackFunction);
                Nav_Prc_deselectTripCreationTVNodePtr = new Nav_Prc_deselectTripCreationTVNodeDelegate(Nav_Prc_deselectTripCreationTVNodeCallBackFunction);
                Nav_Prc_scrollTripCreationTVPtr = new Nav_Prc_scrollTripCreationTVDelegate(Nav_Prc_scrollTripCreationTVCallBackFunction);
                #endregion Trip Creation TV
                #region Trip Selection TV
                Nav_Prc_loadTripSelectionTVPtr = new Nav_Prc_loadTripSelectionTVDelegate(Nav_Prc_loadTripSelectionTVCallBackFunction);
                Nav_Prc_setTripSelectionTVNodePtr = new Nav_Prc_setTripSelectionTVNodeDelegate(Nav_Prc_setTripSelectionTVNodeCallBackFunction);
                Nav_Prc_refreshTripSelectionTVPtr = new Nav_Prc_refreshTripSelectionTVDelegate(Nav_Prc_refreshTripSelectionTVCallBackFunction);
                Nav_Prc_scrollTripSelectionTVPtr = new Nav_Prc_scrollTripSelectionTVDelegate(Nav_Prc_scrollTripSelectionTVCallBackFunction);
                Nav_Prc_deleteTripSelectionTVNodePtr = new Nav_Prc_deleteTripSelectionTVNodeDelegate(Nav_Prc_deleteTripSelectionTVNodeCallBackFunction);
                #endregion Trip Selection TV
                #region Others
                Nav_Prc_setTripNameTBTextPtr = new Nav_Prc_setTripNameTBTextDelegate(Nav_Prc_setTripNameTBTextCallBackFunction);
                Nav_Prc_setTripTagsTBTextPtr = new Nav_Prc_setTripTagsTBTextDelegate(Nav_Prc_setTripTagsTBTextCallBackFunction);
                Nav_Prc_setForwardRBPtr = new Nav_Prc_setForwardRBDelegate(Nav_Prc_setForwardRBCallBackFunction);
                Nav_Prc_setReverseRBPtr = new Nav_Prc_setReverseRBDelegate(Nav_Prc_setReverseRBCallBackFunction);
                Nav_Prc_setStartButtonPtr = new Nav_Prc_setStartButtonDelegate(Nav_Prc_setStartButtonCallBackFunction);
                Nav_Prc_setRouteDistanceTextPtr = new Nav_Prc_setRouteDistanceTextDelegate(Nav_Prc_setRouteDistanceTextCallBackFunction);
                Nav_Prc_setRouteTimeTextPtr = new Nav_Prc_setRouteTimeTextDelegate(Nav_Prc_setRouteTimeTextCallBackFunction);
                Nav_Prc_setTripDistanceTextPtr = new Nav_Prc_setTripDistanceTextDelegate(Nav_Prc_setTripDistanceTextCallBackFunction);
                Nav_Prc_setTripTimeTextPtr = new Nav_Prc_setTripTimeTextDelegate(Nav_Prc_setTripTimeTextCallBackFunction);
                Nav_Prc_setTimeRemainingTextPtr = new Nav_Prc_setTimeRemainingTextDelegate(Nav_Prc_setTimeRemainingTextCallBackFunction);
                Statics.FuncPtrs.SetNavButtonPtr = new Statics.FuncPtrs.TD_Void_String_Color(Nav_Prc_setStartButton);
                Statics.FuncPtrs.SetTimeRemainingPtr = new Statics.FuncPtrs.TD_Void_UInt32(Nav_Prc_setTimeRemainingText);
                Nav_Prc_setLoopCountPtr = new Nav_Prc_setLoopCountDelegate(Nav_Prc_setLoopCount);
                #endregion Others
            }
        }
        private void Nav_Prc_loadUserSettings()
        {
            Statics.Settings.Navigation.PromptOnRightClick = (bool)UserSettings.GetValue(UserSettings.BOT.NAV, "Nav_Prc_PromptOnRightClick");
        }
        private void Nav_Prc_initTreeViews()
        {
            Nav_Prc_RouteTV.Indent = Nav_Prc_TVDefaultIndent;
            Nav_Prc_RouteTV.HideSelection = false;
            Nav_Prc_Trip_CreationTV.Indent = Nav_Prc_TVDefaultIndent;
            Nav_Prc_Trip_CreationTV.HideSelection = false;
            Nav_Prc_Trip_SelectionTV.Indent = Nav_Prc_TVDefaultIndent;
            Nav_Prc_Trip_SelectionTV.HideSelection = false;
            Nav_Prc_loadRouteTV();
            Nav_Prc_loadTripSelectionTV();
        }
        #region Tool Tips
        private void Nav_Prc_createToolTips()
        {
            if (Nav_Prc_RouteTV_TT.ToolTipTitle != Nav_Prc_RouteTV_TT_Title)
            {
                //default Autopop delay: 5000
                //default Auto delay: 500
                //default Initial delay: 500
                //default Reshow delay: 100

                //Route TV
                Nav_Prc_RouteTV_TT.AutoPopDelay = 10000;
                Nav_Prc_RouteTV_TT.InitialDelay = 200;
                Nav_Prc_RouteTV_TT.ToolTipIcon = ToolTipIcon.Info;
                Nav_Prc_RouteTV_TT.ToolTipTitle = Nav_Prc_RouteTV_TT_Title;
                Nav_Prc_RouteTV_TT.SetToolTip(Nav_Prc_RouteTV, Nav_Prc_RouteTV_TT_Text);
                //Trip Creation TV
                Nav_Prc_Trip_CreationTV_TT.AutoPopDelay = 10000;
                Nav_Prc_Trip_CreationTV_TT.InitialDelay = 200;
                Nav_Prc_Trip_CreationTV_TT.ToolTipIcon = ToolTipIcon.None;
                Nav_Prc_Trip_CreationTV_TT.ToolTipTitle = Nav_Prc_Trip_CreationTV_TT_Title;
                Nav_Prc_Trip_CreationTV_TT.SetToolTip(Nav_Prc_Trip_CreationTV, Nav_Prc_Trip_CreationTV_TT_Text);
            }
        }
        #endregion Tool Tips
        #endregion Inits
        #region Utility Functions
        #region GUI Updates
        #region Text Box Updates
        private void Nav_Prc_setTripNameTBText(String iText)
        {
            try
            {
                if (Nav_Prc_Trip_Name_TB.InvokeRequired)
                {
                    Nav_Prc_Trip_Name_TB.Invoke(Nav_Prc_setTripNameTBTextPtr, new object[] { iText });
                }
                else
                {
                    Nav_Prc_setTripNameTBTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_setTripNameTBText: " + e.ToString());
            }
        }
        private void Nav_Prc_setTripNameTBTextCallBackFunction(String iText)
        {
            Nav_Prc_Trip_Name_TB.Text = iText;
            if (iText != Nav_Prc_TripNameTBDefText)
            {
                Nav_Prc_Trip_Name_TB.ForeColor = Color.Black;
            }
            else
            {
                Nav_Prc_Trip_Name_TB.ForeColor = Color.Gray;
            }
        }
        private void Nav_Prc_setTripTagsTBText(String iText)
        {
            try
            {
                if (Nav_Prc_Trip_Tags_TB.InvokeRequired)
                {
                    Nav_Prc_Trip_Tags_TB.Invoke(Nav_Prc_setTripTagsTBTextPtr, new object[] { iText });
                }
                else
                {
                    Nav_Prc_setTripTagsTBTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_setTripTagsTBText: " + e.ToString());
            }
        }
        private void Nav_Prc_setTripTagsTBTextCallBackFunction(String iText)
        {
            Nav_Prc_Trip_Tags_TB.Text = iText;
            if (iText != Nav_Prc_TripTagsTBDefText)
            {
                Nav_Prc_Trip_Tags_TB.ForeColor = Color.Black;
            }
            else
            {
                Nav_Prc_Trip_Tags_TB.ForeColor = Color.Gray;
            }
        }
        #endregion Text Box Updates
        #region Tree View Updates
        #region Route TV
        private void Nav_Prc_loadRouteTV()
        {
            try
            {
                if (Nav_Prc_RouteTV.InvokeRequired)
                {
                    Nav_Prc_RouteTV.Invoke(Nav_Prc_loadRouteTVPtr);
                }
                else
                {
                    Nav_Prc_loadRouteTVCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_loadRouteTV: " + e.ToString());
            }
        }
        private void Nav_Prc_loadRouteTVCallBackFunction()
        {
            if (Nav_userRouteNames != null)
            {
                Nav_Prc_RouteTV.BeginUpdate();
                Nav_Prc_RouteTV.Nodes.Clear();
                List<ushort> zondIds;
                List<String> tagList;
                foreach (String name in Nav_userRouteNames)
                {
                    String tags;
                    if (Nav_userRoutesTagsMap.TryGetValue(name, out tags))
                    {
                        tagList = Nav_getTagList(tags);
                        if (Nav_userRoutesZoneMap.TryGetValue(name, out zondIds))
                        {
                            Nav_Route thisRoute = Nav_createRoute(name, true);
                            foreach (String tag in tagList)
                            {
                                if (zondIds.Count == 0)
                                {
                                    Nav_Prc_insertIntoRouteTV(thisRoute, tag, 0);
                                }
                                else
                                {
                                    foreach (ushort zone in zondIds)
                                    {
                                        Nav_Prc_insertIntoRouteTV(thisRoute, tag, zone);
                                    }
                                }
                            }
                        }
                        else
                        {
                            String msg = "[ERROR] In Nav_Prc_loadRouteTVCallBackFunction: Could not find the zoneIds for route name '" + name + "'";
                            LoggingFunctions.Timestamp(msg);
                            MessageBox.Show(msg);
                            Nav_Prc_RouteTV.EndUpdate();
                            return;
                        }
                    }
                    else
                    {
                        String msg = "[ERROR] In Nav_Prc_loadRouteTVCallBackFunction: Could not find the tags for route name '" + name + "'";
                        LoggingFunctions.Timestamp(msg);
                        MessageBox.Show(msg);
                        Nav_Prc_RouteTV.EndUpdate();
                        return;
                    }
                }
                Nav_Prc_RouteTV.EndUpdate();
            }
        }
        private void Nav_Prc_insertIntoRouteTV(Nav_Route iRoute, String iTag, ushort iZone)
        {
            TreeNode topNode = null;
            TreeNode bottomNode = null;
            TreeNode routeNode = null;
            String topString;
            String bottomString;
            String zoneShortName = Zones.GetZoneShortName(iZone);
            if (iTag == "")
            {
                iTag = Nav_sortingUntaggedText;
            }
            if (iZone == 0)
            {
                zoneShortName = Nav_sortingNoZoneText;
            }
            if (Statics.Settings.Navigation.SortingTagsFirst == true)
            {
                topString = iTag;
                bottomString = zoneShortName;
            }
            else
            {
                topString = zoneShortName;
                bottomString = iTag;
            }
            TreeNode[] firstLevel = Nav_Prc_RouteTV.Nodes.Find(topString, false);
            if (firstLevel.Length == 0)
            {
                //Need to find the index to insert this node alphabetically.
                int nbNodes = Nav_Prc_RouteTV.Nodes.Count;
                int insertIdx = nbNodes;
                for (int ii = 0; ii < nbNodes; ii++)
                {
                    if (topString.CompareTo(Nav_Prc_RouteTV.Nodes[ii].Text) <= 0)
                    {
                        insertIdx = ii;
                        break;
                    }
                }
                topNode = Nav_Prc_RouteTV.Nodes.Insert(insertIdx, topString, topString);
                topNode.Tag = topString;
            }
            else
            {
                topNode = firstLevel[0];
            }
            TreeNode[] secondLevel = topNode.Nodes.Find(bottomString, false);
            if (secondLevel.Length == 0)
            {
                //Need to find the index to insert this node alphabetically.
                int nbNodes = topNode.Nodes.Count;
                int insertIdx = nbNodes;
                for (int ii = 0; ii < nbNodes; ii++)
                {
                    if (bottomString.CompareTo(topNode.Nodes[ii].Text) <= 0)
                    {
                        insertIdx = ii;
                        break;
                    }
                }
                bottomNode = topNode.Nodes.Insert(insertIdx, bottomString, bottomString);
                bottomNode.Tag = bottomString;
            }
            else
            {
                bottomNode = secondLevel[0];
            }
            //Now we have the first 2 levels (full path), check if this route is already there somehow.
            TreeNode[] thirdLevel = bottomNode.Nodes.Find(iRoute.RouteName, false);
            if (thirdLevel.Length == 0)
            {
                //Insert this route at the bottom node.
                //Need to find the index to insert this node alphabetically.
                int nbNodes = bottomNode.Nodes.Count;
                int insertIdx = nbNodes;
                for (int ii = 0; ii < nbNodes; ii++)
                {
                    if (iRoute.RouteName.CompareTo(bottomNode.Nodes[ii].Text) <= 0)
                    {
                        insertIdx = ii;
                        break;
                    }
                }
                routeNode = bottomNode.Nodes.Insert(insertIdx, iRoute.RouteName, iRoute.RouteName);
                routeNode.Tag = iRoute;
                foreach (Routes.UserRoutesRow node in iRoute.RouteNodes)
                {
                    TreeNode nodeNode = routeNode.Nodes.Add(node.NodeDetail, node.NodeDetail);
                    nodeNode.Tag = node;
                }
            }
        }
        private void Nav_Prc_setRouteTVNode(TreeNode iNode)
        {
            try
            {
                if (Nav_Prc_RouteTV.InvokeRequired)
                {
                    Nav_Prc_RouteTV.Invoke(Nav_Prc_setRouteTVNodePtr, new object[] { iNode });
                }
                else
                {
                    Nav_Prc_setRouteTVNodeCallBackFunction(iNode);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_setRouteTVIndex: " + e.ToString());
            }
        }
        private void Nav_Prc_setRouteTVNodeCallBackFunction(TreeNode iNode)
        {
            //TreeNode[] foundNodes = Nav_Prc_RouteTV.Nodes.Find(iRoute.RouteName, true);
            //if ((foundNodes == null) || (foundNodes.Length == 0))
            //{
            //    return;
            //}
            //Nav_Prc_RouteTV.SelectedNode = foundNodes[0];
            Nav_Prc_RouteTV.SelectedNode = iNode;
        }
        private void Nav_Prc_deselectRouteTVNode()
        {
            try
            {
                if (Nav_Prc_RouteTV.InvokeRequired)
                {
                    Nav_Prc_RouteTV.Invoke(Nav_Prc_deselectRouteTVNodePtr);
                }
                else
                {
                    Nav_Prc_deselectRouteTVNodeCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_deselectRouteTVNode: " + e.ToString());
            }
        }
        private void Nav_Prc_deselectRouteTVNodeCallBackFunction()
        {
            Nav_Prc_RouteTV.SelectedNode = null;
        }
        private void Nav_Prc_refreshRouteTV()
        {
            try
            {
                if (Nav_Prc_RouteTV.InvokeRequired)
                {
                    Nav_Prc_RouteTV.Invoke(Nav_Prc_refreshRouteTVPtr);
                }
                else
                {
                    Nav_Prc_refreshRouteTVCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_refreshRouteTV: " + e.ToString());
            }
        }
        private void Nav_Prc_refreshRouteTVCallBackFunction()
        {
            Nav_Prc_loadRouteTVCallBackFunction();
        }
        private void Nav_Prc_scrollRouteTV()
        {
            TreeNode lastNode = Util_TV_GetLastVisibleNode(Nav_Prc_RouteTV);
            if (lastNode != null)
            {
                Nav_Prc_scrollRouteTV(lastNode);
            }
        }
        private void Nav_Prc_scrollRouteTV(TreeNode iNode)
        {
            try
            {
                if (Nav_Prc_RouteTV.InvokeRequired)
                {
                    Nav_Prc_RouteTV.Invoke(Nav_Prc_scrollRouteTVPtr, new object[] { iNode });
                }
                else
                {
                    Nav_Prc_scrollRouteTVCallBackFunction(iNode);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_scrollRouteTV: " + e.ToString());
            }
        }
        private void Nav_Prc_scrollRouteTVCallBackFunction(TreeNode iNode)
        {
            iNode.EnsureVisible();
        }
        #endregion Route TV
        #region Trip Creation TV
        private void Nav_Prc_loadTripCreationTV(Nav_Trip iTrip)
        {
            try
            {
                if (Nav_Prc_Trip_CreationTV.InvokeRequired)
                {
                    Nav_Prc_Trip_CreationTV.Invoke(Nav_Prc_loadTripCreationTVPtr, new object[] { iTrip });
                }
                else
                {
                    Nav_Prc_loadTripCreationTVCallBackFunction(iTrip);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_loadTripCreationTV: " + e.ToString());
            }
        }
        private void Nav_Prc_loadTripCreationTVCallBackFunction(Nav_Trip iTrip)
        {
            Nav_Prc_Trip_CreationTV.BeginUpdate();
            Nav_Prc_Trip_CreationTV.Nodes.Clear();
            foreach (Nav_Route route in iTrip.TripRoutes)
            {
                TreeNode routeNode = Nav_Prc_Trip_CreationTV.Nodes.Add(route.ToString(), route.ToString());
                routeNode.Tag = route;
                foreach (Routes.UserRoutesRow localRouteNode in route.RouteNodes)
                {
                    TreeNode nodeNode = routeNode.Nodes.Add(localRouteNode.NodeDetail, localRouteNode.NodeDetail);
                    nodeNode.Tag = localRouteNode;
                }
            }
            Nav_Prc_Trip_CreationTV.EndUpdate();
            Nav_Prc_exisingTripLoaded = true;
        }
        private void Nav_Prc_addTripCreationTVNode(Nav_Route iRoute)
        {
            try
            {
                if (Nav_Prc_Trip_CreationTV.InvokeRequired)
                {
                    Nav_Prc_Trip_CreationTV.Invoke(Nav_Prc_addTripCreationTVNodePtr, new object[] { iRoute });
                }
                else
                {
                    Nav_Prc_addTripCreationTVNodeCallBackFunction(iRoute);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_addTripCreationTVNode: " + e.ToString());
            }
        }
        private void Nav_Prc_addTripCreationTVNodeCallBackFunction(Nav_Route iRoute)
        {
            Nav_Prc_Trip_CreationTV.BeginUpdate();
            TreeNode routeNode = Nav_Prc_Trip_CreationTV.Nodes.Add(iRoute.ToString(), iRoute.ToString());
            routeNode.Tag = iRoute;
            foreach (Routes.UserRoutesRow localRouteNode in iRoute.RouteNodes)
            {
                TreeNode nodeNode = routeNode.Nodes.Add(localRouteNode.NodeDetail, localRouteNode.NodeDetail);
                nodeNode.Tag = localRouteNode;
            }
            Nav_Prc_Trip_CreationTV.EndUpdate();
        }
        private void Nav_Prc_clearTripCreationTV()
        {
            try
            {
                if (Nav_Prc_Trip_CreationTV.InvokeRequired)
                {
                    Nav_Prc_Trip_CreationTV.Invoke(Nav_Prc_clearTripCreationTVPtr);
                }
                else
                {
                    Nav_Prc_clearTripCreationTVCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_clearTripCreationTV: " + e.ToString());
            }
        }
        private void Nav_Prc_clearTripCreationTVCallBackFunction()
        {
            Nav_Prc_Trip_CreationTV.BeginUpdate();
            Nav_Prc_Trip_CreationTV.Nodes.Clear();
            Nav_Prc_Trip_CreationTV.EndUpdate();
        }
        private void Nav_Prc_deleteTripCreationTVNode(TreeNode iNode)
        {
            try
            {
                if (Nav_Prc_Trip_CreationTV.InvokeRequired)
                {
                    Nav_Prc_Trip_CreationTV.Invoke(Nav_Prc_deleteTripCreationTVNodePtr, new object[] { iNode });
                }
                else
                {
                    Nav_Prc_deleteTripCreationTVNodeCallBackFunction(iNode);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_deleteTripCreationTVNode: " + e.ToString());
            }
        }
        private void Nav_Prc_deleteTripCreationTVNodeCallBackFunction(TreeNode iNode)
        {
            Nav_Prc_Trip_CreationTV.BeginUpdate();
            if (iNode != null)
            {
                Nav_Prc_Trip_CreationTV.Nodes.Remove(iNode);
            }
            Nav_Prc_Trip_CreationTV.EndUpdate();
        }
        private void Nav_Prc_setTripCreationTVNode(TreeNode iNode)
        {
            try
            {
                if (Nav_Prc_Trip_CreationTV.InvokeRequired)
                {
                    Nav_Prc_Trip_CreationTV.Invoke(Nav_Prc_setTripCreationTVNodePtr, new object[] { iNode });
                }
                else
                {
                    Nav_Prc_setTripCreationTVNodeCallBackFunction(iNode);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_setTripCreationTVIndex: " + e.ToString());
            }
        }
        private void Nav_Prc_setTripCreationTVNodeCallBackFunction(TreeNode iNode)
        {
            Nav_Prc_Trip_CreationTV.SelectedNode = iNode;
            Nav_Prc_Trip_CreationTV.Select();
        }
        private void Nav_Prc_deselectTripCreationTVNode()
        {
            try
            {
                if (Nav_Prc_Trip_CreationTV.InvokeRequired)
                {
                    Nav_Prc_Trip_CreationTV.Invoke(Nav_Prc_deselectTripCreationTVNodePtr);
                }
                else
                {
                    Nav_Prc_deselectTripCreationTVNodeCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_deselectTripCreationTVNode: " + e.ToString());
            }
        }
        private void Nav_Prc_deselectTripCreationTVNodeCallBackFunction()
        {
            Nav_Prc_Trip_CreationTV.SelectedNode = null;
        }
        private void Nav_Prc_scrollTripCreationTV()
        {
            TreeNode lastNode = Util_TV_GetLastVisibleNode(Nav_Prc_Trip_CreationTV);
            if (lastNode != null)
            {
                Nav_Prc_scrollTripCreationTV(lastNode);
            }
        }
        private void Nav_Prc_scrollTripCreationTV(TreeNode iNode)
        {
            try
            {
                if (Nav_Prc_Trip_CreationTV.InvokeRequired)
                {
                    Nav_Prc_Trip_CreationTV.Invoke(Nav_Prc_scrollTripCreationTVPtr, new object[] { iNode });
                }
                else
                {
                    Nav_Prc_scrollTripCreationTVCallBackFunction(iNode);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_scrollTripCreationTV: " + e.ToString());
            }
        }
        private void Nav_Prc_scrollTripCreationTVCallBackFunction(TreeNode iNode)
        {
            iNode.EnsureVisible();
        }
        #endregion Trip Creation TV
        #region Trip Selection TV
        private void Nav_Prc_loadTripSelectionTV()
        {
            try
            {
                if (Nav_Prc_Trip_SelectionTV.InvokeRequired)
                {
                    Nav_Prc_Trip_SelectionTV.Invoke(Nav_Prc_loadTripSelectionTVPtr);
                }
                else
                {
                    Nav_Prc_loadTripSelectionTVCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_loadTripSelectionTV: " + e.ToString());
            }
        }
        private void Nav_Prc_loadTripSelectionTVCallBackFunction()
        {
            Nav_Prc_Trip_SelectionTV.BeginUpdate();
            Nav_Prc_Trip_SelectionTV.Nodes.Clear();
            List<ushort> zondIds;
            List<String> tagList;
            foreach (String name in Statics.Datasets.UserTripNames)
            {
                Nav_Trip tripToAdd = Nav_getTrip(name);
                if (tripToAdd == null)
                {
                    continue;
                }
                String tags;
                if (Nav_userTripsTagsMap.TryGetValue(name, out tags))
                {
                    tagList = Nav_getTagList(tags);
                    Dictionary<String, List<ushort>> localZoneMap = null;
                    if (Statics.Settings.Navigation.SortingUseLastZoneOnly)
                    {
                        localZoneMap = Nav_userTripsLastZoneMap;
                    }
                    else
                    {
                        localZoneMap = Nav_userTripsZoneMap;
                    }
                    if (localZoneMap.TryGetValue(name, out zondIds))
                    {
                        foreach (String tag in tagList)
                        {
                            if (zondIds.Count == 0)
                            {
                                Nav_Prc_insertIntoTripSelectionTV(tripToAdd, tag, 0);
                            }
                            else
                            {
                                foreach (ushort zone in zondIds)
                                {
                                    Nav_Prc_insertIntoTripSelectionTV(tripToAdd, tag, zone);
                                }
                            }
                        }
                    }
                    else
                    {
                        String msg = "[ERROR] In Nav_Prc_loadTripSelectionTVCallBackFunction: Could not find the zoneIds for trip name '" + name + "'";
                        LoggingFunctions.Timestamp(msg);
                        MessageBox.Show(msg);
                        Nav_Prc_Trip_SelectionTV.EndUpdate();
                        return;
                    }
                }
                else
                {
                    String msg = "[ERROR] In Nav_Prc_loadTripSelectionTVCallBackFunction: Could not find the tags for trip name '" + name + "'";
                    LoggingFunctions.Timestamp(msg);
                    MessageBox.Show(msg);
                    Nav_Prc_Trip_SelectionTV.EndUpdate();
                    return;
                }
            }
            Nav_Prc_Trip_SelectionTV.EndUpdate();
        }
        private void Nav_Prc_insertIntoTripSelectionTV(Nav_Trip iTrip, String iTag, ushort iZone)
        {
            if (iTrip == null)
            {
                return;
            }
            if (iTrip.TripNodes.Count == 0)
            {
                String msg = "[ERROR] Tried to insert an empty trip, returning.";
                LoggingFunctions.Timestamp(msg);
                MessageBox.Show(msg);
                return;
            }
            List<Nav_Route> tripList = iTrip.TripRoutes;
            if (tripList == null)
            {
                String msg = "[ERROR] Tried to insert an empty trip, returning.";
                LoggingFunctions.Timestamp(msg);
                MessageBox.Show(msg);
                return;
            }
            TreeNode topNode = null;
            TreeNode bottomNode = null;
            TreeNode tripNode = null;
            String topString;
            String bottomString;
            String zoneShortName = Zones.GetZoneShortName(iZone);
            if (iTag == "")
            {
                iTag = Nav_sortingUntaggedText;
            }
            if (iZone == 0)
            {
                zoneShortName = Nav_sortingNoZoneText;
            }
            if (Statics.Settings.Navigation.SortingTagsFirst == true)
            {
                topString = iTag;
                bottomString = zoneShortName;
            }
            else
            {
                topString = zoneShortName;
                bottomString = iTag;
            }
            TreeNode[] firstLevel = Nav_Prc_Trip_SelectionTV.Nodes.Find(topString, false);
            if (firstLevel.Length == 0)
            {
                //Need to find the index to insert this node alphabetically.
                int nbNodes = Nav_Prc_Trip_SelectionTV.Nodes.Count;
                int insertIdx = nbNodes;
                for (int ii = 0; ii < nbNodes; ii++)
                {
                    if (topString.CompareTo(Nav_Prc_Trip_SelectionTV.Nodes[ii].Text) <= 0)
                    {
                        insertIdx = ii;
                        break;
                    }
                }
                topNode = Nav_Prc_Trip_SelectionTV.Nodes.Insert(insertIdx, topString, topString);
                topNode.Tag = topString;
            }
            else
            {
                topNode = firstLevel[0];
            }
            TreeNode[] secondLevel = topNode.Nodes.Find(bottomString, false);
            if (secondLevel.Length == 0)
            {
                //Need to find the index to insert this node alphabetically.
                int nbNodes = topNode.Nodes.Count;
                int insertIdx = nbNodes;
                for (int ii = 0; ii < nbNodes; ii++)
                {
                    if (bottomString.CompareTo(topNode.Nodes[ii].Text) <= 0)
                    {
                        insertIdx = ii;
                        break;
                    }
                }
                bottomNode = topNode.Nodes.Insert(insertIdx, bottomString, bottomString);
                bottomNode.Tag = bottomString;
            }
            else
            {
                bottomNode = secondLevel[0];
            }
            //Now we have the first 2 levels (full path), check if this trip is already there somehow.
            TreeNode[] thirdLevel = bottomNode.Nodes.Find(iTrip.TripName, false);
            if (thirdLevel.Length == 0)
            {
                //Insert this trip at the bottom node.
                //Need to find the index to insert this node alphabetically.
                int nbNodes = bottomNode.Nodes.Count;
                int insertIdx = nbNodes;
                for (int ii = 0; ii < nbNodes; ii++)
                {
                    if (iTrip.TripName.CompareTo(bottomNode.Nodes[ii].Text) <= 0)
                    {
                        insertIdx = ii;
                        break;
                    }
                }
                tripNode = bottomNode.Nodes.Insert(insertIdx, iTrip.TripName, iTrip.TripName);
                tripNode.Tag = iTrip;
                foreach (Nav_Route route in iTrip.TripRoutes)
                {
                    TreeNode routeNode = tripNode.Nodes.Add(route.RouteName, route.ToString());
                    routeNode.Tag = route;
                    foreach (Routes.UserRoutesRow localRouteNode in route.RouteNodes)
                    {
                        TreeNode nodeNode = routeNode.Nodes.Add(localRouteNode.NodeDetail, localRouteNode.NodeDetail);
                        nodeNode.Tag = localRouteNode;
                    }
                }
            }
        }
        private void Nav_Prc_setTripSelectionTVNode(TreeNode iNode)
        {
            try
            {
                if (Nav_Prc_Trip_SelectionTV.InvokeRequired)
                {
                    Nav_Prc_Trip_SelectionTV.Invoke(Nav_Prc_setTripSelectionTVNodePtr, new object[] { iNode });
                }
                else
                {
                    Nav_Prc_setTripSelectionTVNodeCallBackFunction(iNode);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_setTripSelectionTVNode: " + e.ToString());
            }
        }
        private void Nav_Prc_setTripSelectionTVNodeCallBackFunction(TreeNode iNode)
        {
            Nav_Prc_Trip_SelectionTV.SelectedNode = iNode;
        }
        private void Nav_Prc_refreshTripSelectionTV()
        {
            try
            {
                if (Nav_Prc_Trip_SelectionTV.InvokeRequired)
                {
                    Nav_Prc_Trip_SelectionTV.Invoke(Nav_Prc_refreshTripSelectionTVPtr);
                }
                else
                {
                    Nav_Prc_refreshTripSelectionTVCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_refreshTripSelectionTV: " + e.ToString());
            }
        }
        private void Nav_Prc_refreshTripSelectionTVCallBackFunction()
        {
            Nav_Prc_loadTripSelectionTVCallBackFunction();
        }
        private void Nav_Prc_deleteTripSelectionTVNode(TreeNode[] iNodes)
        {
            try
            {
                if (Nav_Prc_Trip_SelectionTV.InvokeRequired)
                {
                    Nav_Prc_Trip_SelectionTV.Invoke(Nav_Prc_deleteTripSelectionTVNodePtr, new object[] { iNodes });
                }
                else
                {
                    Nav_Prc_deleteTripSelectionTVNodeCallBackFunction(iNodes);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_deleteTripSelectionTVNode: " + e.ToString());
            }
        }
        private void Nav_Prc_deleteTripSelectionTVNodeCallBackFunction(TreeNode[] iNodes)
        {
            if ((iNodes != null) && (iNodes.Length > 0))
            {
                Nav_Prc_Trip_SelectionTV.BeginUpdate();
                int nbNodes = iNodes.Length;
                for (int ii = 0; ii < nbNodes; ii++)
                {
                    Nav_Prc_Trip_SelectionTV.Nodes.Remove(iNodes[ii]);
                }
                Nav_Prc_Trip_SelectionTV.EndUpdate();
            }
        }
        private void Nav_Prc_scrollTripSelectionTV()
        {
            TreeNode lastNode = Util_TV_GetLastVisibleNode(Nav_Prc_Trip_SelectionTV);
            if (lastNode != null)
            {
                Nav_Prc_scrollTripSelectionTV(lastNode);
            }
        }
        private void Nav_Prc_scrollTripSelectionTV(TreeNode iNode)
        {
            try
            {
                if (Nav_Prc_Trip_SelectionTV.InvokeRequired)
                {
                    Nav_Prc_Trip_SelectionTV.Invoke(Nav_Prc_scrollTripSelectionTVPtr, new object[] { iNode });
                }
                else
                {
                    Nav_Prc_scrollTripSelectionTVCallBackFunction(iNode);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_scrollTripSelectionTV: " + e.ToString());
            }
        }
        private void Nav_Prc_scrollTripSelectionTVCallBackFunction(TreeNode iNode)
        {
            iNode.EnsureVisible();
        }
        private void Nav_Prc_updateTripSelectionTVNode(Nav_Trip iTrip, Nav_Route iRoute)
        {
            List<TreeNode> flattenedNodes = Util_TV_FlattenTree(Nav_Prc_Trip_SelectionTV, 3);
            foreach (TreeNode node in flattenedNodes)
            {
                if (node.Level == 2)
                {
                    Nav_Trip foundTrip = (Nav_Trip)node.Tag;
                    int routeIndexInTrip = iTrip.TripRoutes.IndexOf(iRoute);
                    if (routeIndexInTrip < 0)
                    {
                        MessageBox.Show("routeIndexInTrip was negative.");
                        return;
                    }
                    if (foundTrip == iTrip)
                    {
                        TreeNode routeNode = node.Nodes[routeIndexInTrip];
                        Nav_Route foundRoute = (Nav_Route)routeNode.Tag;
                        routeNode.Text = foundRoute.ToString();
                        routeNode.Name = foundRoute.ToString();
                    }
                }
            }
        }
        #endregion Trip Selection TV
        #endregion Tree View Updates
        #region Radio Button Updates
        private void Nav_Prc_setForwardRB(bool iChk)
        {
            try
            {
                if (Nav_Prc_Forward_RB.InvokeRequired)
                {
                    Nav_Prc_Forward_RB.Invoke(Nav_Prc_setForwardRBPtr, new object[] { iChk });
                }
                else
                {
                    Nav_Prc_setForwardRBCallBackFunction(iChk);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_setForwardRB: " + e.ToString());
            }
        }
        private void Nav_Prc_setForwardRBCallBackFunction(bool iChk)
        {
            if (Nav_Prc_Forward_RB.Checked != iChk)
            {
                Nav_Prc_reverseCheckedProgrammatically = true;
                Nav_Prc_Forward_RB.Checked = iChk;
            }
        }
        private void Nav_Prc_setReverseRB(bool iChk)
        {
            try
            {
                if (Nav_Prc_Reverse_RB.InvokeRequired)
                {
                    Nav_Prc_Reverse_RB.Invoke(Nav_Prc_setReverseRBPtr, new object[] { iChk });
                }
                else
                {
                    Nav_Prc_setReverseRBCallBackFunction(iChk);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_setReverseRB: " + e.ToString());
            }
        }
        private void Nav_Prc_setReverseRBCallBackFunction(bool iChk)
        {
            if (Nav_Prc_Reverse_RB.Checked != iChk)
            {
                Nav_Prc_reverseCheckedProgrammatically = true;
                Nav_Prc_Reverse_RB.Checked = iChk;
            }
        }
        #endregion Radio Button Updates
        #region Button Updates
        private void Nav_Prc_setStartButton(String iText, Color iColor)
        {
            try
            {
                if (Nav_Prc_Start_Button.InvokeRequired)
                {
                    Nav_Prc_Start_Button.Invoke(Nav_Prc_setStartButtonPtr, new object[] { iText, iColor });
                }
                else
                {
                    Nav_Prc_setStartButtonCallBackFunction(iText, iColor);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_setStartButton: " + e.ToString());
            }
        }
        private void Nav_Prc_setStartButtonCallBackFunction(String iText, Color iColor)
        {
            Nav_Prc_Start_Button.Text = iText;
            Nav_Prc_Start_Button.BackColor = iColor;
        }
        #endregion Button Updates
        #region Label Updates
        private void Nav_Prc_setRouteDistanceAndTimeText(Nav_Route iRoute)
        {
            double routeDist = Nav_getRouteDistance(iRoute);
            UInt32 routeTime = Nav_getRouteTime(iRoute, routeDist);
            UInt32 routeTimeMin = routeTime / 60;
            UInt32 routeTimeSec = routeTime % 60;
            Nav_Prc_setRouteDistanceText(((UInt32)routeDist).ToString());
            Nav_Prc_setRouteTimeText(routeTimeMin + " min " + routeTimeSec + " sec");
        }
        private void Nav_Prc_setRouteDistanceText(String iText)
        {
            try
            {

                if (Nav_Prc_Route_Distance_Text.InvokeRequired)
                {
                    Nav_Prc_Route_Distance_Text.Invoke(Nav_Prc_setRouteDistanceTextPtr, new object[] { iText });
                }
                else
                {
                    Nav_Prc_setRouteDistanceTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_setRouteDistanceText: " + e.ToString());
            }
        }
        private void Nav_Prc_setRouteDistanceTextCallBackFunction(String iText)
        {
            Nav_Prc_Route_Distance_Text.Text = iText;
        }
        private void Nav_Prc_setRouteTimeText(String iText)
        {
            try
            {

                if (Nav_Prc_Route_Time_Text.InvokeRequired)
                {
                    Nav_Prc_Route_Time_Text.Invoke(Nav_Prc_setRouteTimeTextPtr, new object[] { iText });
                }
                else
                {
                    Nav_Prc_setRouteTimeTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_setRouteTimeText: " + e.ToString());
            }
        }
        private void Nav_Prc_setRouteTimeTextCallBackFunction(String iText)
        {
            Nav_Prc_Route_Time_Text.Text = iText;
        }
        private void Nav_Prc_setTripDistanceAndTimeText(Nav_Trip iTrip)
        {
            UInt32 TripDist = (UInt32)Nav_getTripDistance(iTrip);
            UInt32 TripTime = Nav_getTripTime(iTrip);
            UInt32 TripTimeMin = TripTime / 60;
            UInt32 TripTimeSec = TripTime % 60;
            Nav_Prc_setTripDistanceText(TripDist.ToString());
            Nav_Prc_setTripTimeText(TripTimeMin + " min " + TripTimeSec + " sec");
        }
        private void Nav_Prc_setTripDistanceText(String iText)
        {
            try
            {

                if (Nav_Prc_Trip_Distance_Text.InvokeRequired)
                {
                    Nav_Prc_Trip_Distance_Text.Invoke(Nav_Prc_setTripDistanceTextPtr, new object[] { iText });
                }
                else
                {
                    Nav_Prc_setTripDistanceTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_setTripDistanceText: " + e.ToString());
            }
        }
        private void Nav_Prc_setTripDistanceTextCallBackFunction(String iText)
        {
            Nav_Prc_Trip_Distance_Text.Text = iText;
        }
        private void Nav_Prc_setTripTimeText(String iText)
        {
            try
            {

                if (Nav_Prc_Trip_Time_Text.InvokeRequired)
                {
                    Nav_Prc_Trip_Time_Text.Invoke(Nav_Prc_setTripTimeTextPtr, new object[] { iText });
                }
                else
                {
                    Nav_Prc_setTripTimeTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_setTripTimeText: " + e.ToString());
            }
        }
        private void Nav_Prc_setTripTimeTextCallBackFunction(String iText)
        {
            Nav_Prc_Trip_Time_Text.Text = iText;
        }
        private void Nav_Prc_setTimeRemainingText(UInt32 iTimeMs)
        {
            try
            {
                if (c_TimerLabel.InvokeRequired)
                {
                    c_TimerLabel.Invoke(Nav_Prc_setTimeRemainingTextPtr, new object[] { iTimeMs });
                }
                else
                {
                    Nav_Prc_setTimeRemainingTextCallBackFunction(iTimeMs);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_setTimeRemainingText: " + e.ToString());
            }
        }
        private void Nav_Prc_setTimeRemainingTextCallBackFunction(UInt32 iTimeMs)
        {
            //Monitor.Enter(m_TOP_TimerLabelLock);
            try
            {
                lock(m_TOP_TimerLabelLock)
                {
                    if ((m_TOP_TimerLabelUser == TIMER_USER.NONE) || (m_TOP_TimerLabelUser == TIMER_USER.NAVIGATION))
                    {
                        m_TOP_TimerLabelUser = TIMER_USER.NAVIGATION;
                        if (iTimeMs > 0)
                        {
                            UInt32 tenthSec = (iTimeMs / 100) % 10;
                            UInt32 sec = iTimeMs / 1000;
                            UInt32 min = sec / 60;
                            sec = sec % 60;
                            c_TimerLabel.Visible = true;
                            if (min > 0)
                            {
                                c_TimerLabel.Text = min.ToString() + " min " + sec.ToString() + "." + tenthSec.ToString() + " sec";
                            }
                            else
                            {
                                c_TimerLabel.Text = sec.ToString() + "." + tenthSec.ToString() + " sec";
                            }
                        }
                        else
                        {
                            c_TimerLabel.Visible = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                //Monitor.Exit(m_TOP_TimerLabelLock);
            }
        }
        #endregion Label Updates
        #region UpDown Updates
        public void Nav_Prc_setLoopCount(UInt32 iCount)
        {
            try
            {
                if (Nav_Prc_Loop_UpDn.InvokeRequired)
                {
                    Nav_Prc_Loop_UpDn.Invoke(Nav_Prc_setLoopCountPtr, new object[] { iCount });
                }
                else
                {
                    Nav_Prc_setLoopCountCallBackFunction(iCount);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Prc_setLoopCount: " + e.ToString());
            }
        }
        private void Nav_Prc_setLoopCountCallBackFunction(UInt32 iCount)
        {
            Nav_Prc_Loop_UpDn.Value = (Decimal)iCount;
        }
        #endregion UpDown Updates
        #endregion GUI Updates
        #region Treeview Utilities
        private static List<TreeNode> Util_TV_FlattenTree(TreeView iTree, int iLevel)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            Queue<TreeNode> queue = new Queue<TreeNode>();

            // Bang all the top nodes into the queue.
            foreach (TreeNode top in iTree.Nodes)
            {
                queue.Enqueue(top);
            }
            while (queue.Count > 0)
            {
                TreeNode node = queue.Dequeue();
                if (node != null)
                {
                    // Add the node to the list of nodes.
                    if (node.Level <= iLevel)
                    {
                        nodes.Add(node);
                        if (node.Nodes != null && node.Nodes.Count > 0)
                        {
                            // Enqueue the child nodes.
                            foreach (TreeNode child in node.Nodes)
                            {
                                queue.Enqueue(child);
                            }
                        }
                    }
                }
            }
            return nodes;
        }
        private static TreeNode Util_TV_GetLastVisibleNode(TreeView iTree)
        {
            List<TreeNode> flatNodes = Util_TV_FlattenTree(iTree, Nav_Prc_maxTVDepth);
            for (int ii = flatNodes.Count - 1; ii >= 0; ii--)
            {
                if (flatNodes[ii].IsVisible == true)
                {
                    return flatNodes[flatNodes.Count - 1];
                }
            }
            return null;
        }
        #endregion Treeview Utilities
        #region Trip Related
        #region Trip Loading/Sorting
        private void Nav_Prc_addRouteToTrip()
        {
            Nav_Prc_addRouteToTrip(Nav_Prc_CurrentRoute);
        }
        private void Nav_Prc_addRouteToTrip(Nav_Route iRoute)
        {
            if (iRoute != null)
            {
                if ((iRoute.Direction == false) && (Nav_canReverseRoute(iRoute) == false))
                {
                    String message = "You cannot reverse a route that has one of the following nodes:\n";
                    message += "1. Keystroke\n2. Target NPC\n3. Trade Item\n4. Trade Gil";
                    MessageBox.Show(message, "Cannot Reverse Route", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Nav_Prc_setForwardRB(true);
                    return;
                }
                else if (!Nav_Prc_canLinkRoute(iRoute))
                {
                    return;
                }
                Nav_Route clonedRoute = Nav_cloneRoute(iRoute);
                if (!Nav_formatRoute(clonedRoute, !clonedRoute.Direction, !clonedRoute.Direction))
                {
                    MessageBox.Show("The route you added contains no nodes.", "Could Not Add Route.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Nav_Prc_CurrentTrip.TripRoutes.Add(clonedRoute);
                Nav_Prc_addTripCreationTVNode(clonedRoute);
                Nav_Prc_modified = true;
            }
        }
        private bool Nav_Prc_canLinkRoute(Nav_Route iRoute)
        {
            if (Nav_Prc_CurrentTrip.TripRoutes.Count == 0)
            {
                return true;
            }
            Routes.UserRoutesRow latterRouteNode = null;
            Routes.UserRoutesRow formerRouteNode = null;
            if (iRoute.Direction == false)
            {
                latterRouteNode = iRoute.RouteNodes[iRoute.RouteNodes.Count - 1];
            }
            else
            {
                latterRouteNode = iRoute.RouteNodes[0];
            }
            if ((latterRouteNode.NodeType != (ushort)NAV_NODE_TYPE.POS_END)
                && (latterRouteNode.NodeType != (ushort)NAV_NODE_TYPE.POS_NODE)
                && (latterRouteNode.NodeType != (ushort)NAV_NODE_TYPE.POS_START)
                && (latterRouteNode.NodeType != (ushort)NAV_NODE_TYPE.POS_ZONE))
            {
                //We can only guarantee that 2 routes will or will not link if the linking nodes are position nodes.
                return true;
            }
            Nav_Route formerRoute = Nav_Prc_CurrentTrip.TripRoutes[Nav_Prc_CurrentTrip.TripRoutes.Count - 1];
            if (formerRoute.Direction == false)
            {
                formerRouteNode = formerRoute.RouteNodes[0];
            }
            else
            {
                formerRouteNode = formerRoute.RouteNodes[formerRoute.RouteNodes.Count - 1];
            }
            if ((formerRouteNode.NodeType != (ushort)NAV_NODE_TYPE.POS_END)
                && (formerRouteNode.NodeType != (ushort)NAV_NODE_TYPE.POS_NODE)
                && (formerRouteNode.NodeType != (ushort)NAV_NODE_TYPE.POS_START)
                && (formerRouteNode.NodeType != (ushort)NAV_NODE_TYPE.POS_ZONE))
            {
                //We can only guarantee that 2 routes will or will not link if the linking nodes are position nodes.
                return true;
            }
            //So now we know that they 2 linking nodes are both position nodes.
            //Check that they are in the same zone and within the max distance margin of each other.
            if(formerRouteNode.NodeZoneID != latterRouteNode.NodeZoneID)
            {
                MessageBox.Show("Cannot link this route due to the starting node not being\nin the same zone as the prior route's final node.", "Cannot Link", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            float distance = (float)Nav_getDistance(formerRouteNode.NodePosX, latterRouteNode.NodePosX, formerRouteNode.NodePosY, latterRouteNode.NodePosY);
            if (distance > Statics.Constants.Navigation.MaxDistForRouteStart)
            {
                MessageBox.Show("Cannot link this route due to the starting node being\ntoo far from the prior route's final node.", "Cannot Link", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            return true;
        }
        #endregion Trip Loading/Sorting
        #region Trip Deletion
        private UInt32 Nav_Prc_deleteTrip(String iTripName, bool iPromptUser, bool iSaveFile, bool iRemoveNameFromList)
        {
            if (!Statics.Datasets.UserTripNames.Contains(iTripName))
            {
                MessageBox.Show("Could not find trip '" + iTripName + "'", "Woops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0xffffffff;
            }
            if (iPromptUser)
            {
                DialogResult promptResult = MessageBox.Show("Are you sure you want to delete the Trip '" + iTripName + "'?", "Delete Trip?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (promptResult == System.Windows.Forms.DialogResult.No)
                {
                    return 0xffffffff;
                }
            }
            UInt32 tripID = Nav_userTripsIdMap[iTripName];
            Nav_Prc_deleteTrip(tripID);
            if (iRemoveNameFromList)
            {
                Statics.Datasets.UserTripNames.Remove(iTripName);
                Nav_userTripsIdMap.Remove(iTripName);
                Nav_userTripsTagsMap.Remove(iTripName);
                Nav_userTripsZoneMap.Remove(iTripName);
                Nav_userTripsLastZoneMap.Remove(iTripName);
            }
            if (iSaveFile)
            {
                Nav_saveUserTrips();
                //If we saved the file it means we've completed the save/update of the trip.
                //So we'll reload the TV in case there are less tags/zones.
                Nav_Prc_loadTripSelectionTV();
            }
            return tripID;
        }
        private void Nav_Prc_deleteTrip(UInt32 iTripID)
        {
            String filter = "TripID=" + iTripID.ToString();
            Routes.UserTripsRow[] rows = (Routes.UserTripsRow[])Statics.Datasets.RoutesDb.UserTrips.Select(filter);
            try
            {
                for (int ii = rows.Length - 1; ii >= 0; ii--)
                {
                    Statics.Datasets.RoutesDb.UserTrips.Rows.Remove(rows[ii]);
                }
            }
            catch (Exception e)
            {
                String msg = "[ERROR] Trying to delete a trip row from the DB: " + e.ToString();
                LoggingFunctions.Timestamp(msg);
                MessageBox.Show(msg);
                return;
            }
            Statics.Datasets.RoutesDb.UserTrips.AcceptChanges();
        }
        #endregion Trip Deletion
        #region Trip Saving
        private void Nav_Prc_saveTrip()
        {
            //Need to check a few things first.
            //1. If the Trip Creation TV is empty, prompt the user.
            //2. If we have * Create New Trip * selected, we must have a name entered in the TB.
            //3. If we have zero routes in the box, prompt the user.
            //4. If we are saving a new trip and the trip name given already exists, prompt the user.
            //5. If we are saving an existing trip and the user changed the name,
            //   if the new name given already exists, prompt the user.

            if ((Nav_Prc_CurrentTrip.TripRoutes.Count <= 0) || (Nav_Prc_Trip_CreationTV.Nodes.Count == 0))
            {
                MessageBox.Show("You must have at least 1 Route added\nto the Trip in order to save the Trip.", "Please Add a Route", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if ((Nav_Prc_TripName == Nav_Prc_TripNameTBDefText) || (Nav_Prc_TripName == ""))
            {
                MessageBox.Show("Please enter a name for the Trip in the text box at the bottom.", "Please Enter a Trip Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Now we'll do things differently whether we're saving an existing trip or creating a new one.
            String tripName = "";
            UInt32 tripId = 0;
            if (Nav_Prc_exisingTripLoaded == true)
            {
                //Check that the user hasn't entered a new name that already exists.
                if (Nav_Prc_CurrentTrip.TripName != Nav_Prc_TripName)
                {
                    if (Statics.Datasets.UserTripNames.Contains(Nav_Prc_TripName))
                    {
                        MessageBox.Show("The trip name you entered already exists,\nplease enter a different name and save again.");
                        return;
                    }
                }
                //We're updating an existing trip, so we first need to delete the old trip
                //then start from scratch with this new trip.
                //But we'll still keep the old tripId number and possibly
                //the tripName (if the user hasn't changed it in the text box).
                tripName = Nav_Prc_CurrentTrip.TripName;
                tripId = Nav_Prc_deleteTrip(tripName, false, false, true);
                if (tripId == 0xffffffff)
                {
                    return;
                }
                Nav_Prc_CurrentTrip.TripNodes.Clear();
            }
            else
            {
                if (Statics.Datasets.UserTripNames.Contains(Nav_Prc_TripName))
                {
                    MessageBox.Show("The trip name you entered already exists,\nplease enter a different name and save again.");
                    return;
                }
                //Otherwise, get a new tripId number.
                tripId = ++Nav_maxTripId;
            }

            //Set the trip name. The Nav_Prc_TripName will either be what the user typed or what was loaded from the existing trip.
            tripName = Nav_Prc_TripName;

            String tripTags = "";
            if ((Nav_Prc_TripTags != Nav_Prc_TripTagsTBDefText) && (Nav_Prc_TripTags != ""))
            {
                tripTags = Nav_Prc_TripTags;
            }
            //Now create a new trip from scratch (except for the tripName and tripId that we've saved so far).
            for (int ii = 0; ii < Nav_Prc_CurrentTrip.TripRoutes.Count; ii++)
            {
                Routes.UserTripsRow row = Statics.Datasets.RoutesDb.UserTrips.NewUserTripsRow();
                row.Direction = Nav_Prc_CurrentTrip.TripRoutes[ii].Direction;
                row.RouteID = Nav_Prc_CurrentTrip.TripRoutes[ii].RouteID;
                row.RouteSequenceID = (uint)ii;
                row.TripID = tripId;
                row.TripName = tripName;
                row.TripTags = tripTags;
                Statics.Datasets.RoutesDb.UserTrips.Rows.Add(row);
                Nav_Prc_CurrentTrip.TripNodes.Add(row);
            }
            Nav_Prc_CurrentTrip.TripName = tripName;
            Nav_Prc_CurrentTrip.TripTags = tripTags;
            Statics.Datasets.RoutesDb.UserTrips.AcceptChanges();
            Nav_saveUserTrips();
            Nav_insertUserTripName(tripName, tripId, tripTags);
            Nav_mergeTripZonesIntoMap(Nav_Prc_CurrentTrip);
            Nav_userTripsLastZoneMap[Nav_Prc_CurrentTrip.TripName] = Nav_getTripFinalZone(tripId);
            Nav_Prc_loadTripSelectionTV();
            TreeNode[] nodes = Nav_Prc_Trip_SelectionTV.Nodes.Find(tripName, true);
            if ((nodes != null) && (nodes.Length > 0))
            {
                Nav_Prc_setTripSelectionTVNode(nodes[0]);
            }
            Nav_Prc_modified = false;
        }
        #endregion Trip Saving
        #endregion Trip Related
        #region Route Related
        #region Route Loading/Sorting
        #endregion Route Loading/Sorting
        #region Route Deletion
        private void Nav_Prc_removeRouteFromTrip(Nav_Route iRoute)
        {
            int idx = Nav_Prc_CurrentTrip.TripRoutes.IndexOf(iRoute);
            if (idx >= 0)
            {
                Nav_Prc_CurrentTrip.TripRoutes.RemoveAt(idx);
                Nav_Prc_deleteTripCreationTVNode(Nav_Prc_Trip_CreationTV.Nodes[idx]);
                Nav_Prc_modified = true;
            }
        }
        #endregion Route Deletion
        #endregion Route Related
        #endregion Utility Functions
        #region Processing
        internal void Nav_Prc_FormatTripAndProcess(Nav_Trip iTrip)
        {
            Nav_Prc_FormatTripAndProcess(iTrip, true, Statics.Settings.Navigation.TripCompleteSound, Nav_Prc_Loop_Cnt, Nav_Prc_setLoopCountPtr);
        }
        internal void Nav_Prc_FormatTripAndProcess(Nav_Trip iTrip, bool iPlaySound, String iSound, UInt32 iLoopCount, Iocaine_2_Form.Nav_Prc_setLoopCountDelegate iSetLoopCountPtr)
        {
            //This function will check for the closest starting point in the trip to
            //where you're at and prune the trip accordingly.
            //It also reformats the route node types in case the route is reversed.
            if (iTrip == null)
            {
                return;
            }
            if (iTrip.TripRoutes.Count > 0)
            {
                Nav_Trip subTrip = Nav_getSubTrip(iTrip,
                                                  MemReads.Self.get_zone_id(),
                                                  MemReads.Self.Position.get_x(),
                                                  MemReads.Self.Position.get_y());
                if (subTrip == null)
                {
                    return;
                }
                //Nav_PrintTrip(subTrip, true);
                Nav_Prc_setTripDistanceAndTimeText(subTrip);
                Bots.Navigation.StartProcessing(subTrip, iTrip, iPlaySound, iSound, iLoopCount, iSetLoopCountPtr);
                Statics.FuncPtrs.SetNavButtonPtr("&Pause", Statics.Buttons.Yellow);
            }
            else
            {
                MessageBox.Show("No trip has been loaded.", "Select Trip", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        internal void Nav_Prc_FormatRouteAndProcess(Nav_Route iRoute)
        {
            Nav_Trip tripWrapper = new Nav_Trip();
            tripWrapper.TripID = 0xffffffff;
            tripWrapper.TripName = iRoute.ToString() + " Wrapper";
            tripWrapper.TripTags = "";
            tripWrapper.TripRoutes.Add(iRoute);
            Routes.UserTripsRow row = Statics.Datasets.RoutesDb.UserTrips.NewUserTripsRow();
            row.Direction = iRoute.Direction;
            row.RouteID = iRoute.RouteID;
            row.RouteSequenceID = 0;
            row.TripID = tripWrapper.TripID;
            row.TripName = tripWrapper.TripName;
            row.TripTags = tripWrapper.TripTags;
            tripWrapper.TripNodes.Add(row);
            Nav_Prc_FormatTripAndProcess(tripWrapper);
        }
        internal void Nav_Prc_FormatNodeAndProcess(Routes.UserRoutesRow iNode)
        {
            if (iNode.NodeType == (ushort)NAV_NODE_TYPE.COMMAND)
            {
                DialogResult promptResult = MessageBox.Show("Do you really want to perform this command?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (promptResult == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }
            else if ((iNode.NodeType == (ushort)NAV_NODE_TYPE.POS_END)
                || (iNode.NodeType == (ushort)NAV_NODE_TYPE.POS_NODE)
                || (iNode.NodeType == (ushort)NAV_NODE_TYPE.POS_START)
                || (iNode.NodeType == (ushort)NAV_NODE_TYPE.POS_ZONE))
            {
                UInt16 zoneId = MemReads.Self.get_zone_id();
                if (zoneId != iNode.NodeZoneID)
                {
                    MessageBox.Show("Node was not in this zone.", "Not in Zone.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                float distToNode = (float)Nav_getDistance(iNode.NodePosX, MemReads.Self.Position.get_x(),
                                                          iNode.NodePosY, MemReads.Self.Position.get_y());
                if (distToNode > Statics.Constants.Navigation.MaxDistForRouteStart)
                {
                    MessageBox.Show("Node was not within range.", "Not in Range.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            Bots.Navigation.ProcessNode(iNode);
        }
        #endregion Processing
        #region Event Handlers
        #region Text Boxes
        #region Trip Name TB
        private void Nav_Prc_Trip_Name_TB_TextChanged(object sender, EventArgs e)
        {
            Nav_Prc_TripName = Nav_Prc_Trip_Name_TB.Text;
        }
        private void Nav_Prc_Trip_Name_TB_Enter(object sender, EventArgs e)
        {
            if (Nav_Prc_Trip_Name_TB.Text == Nav_Prc_TripNameTBDefText)
            {
                Nav_Prc_Trip_Name_TB.Text = "";
                Nav_Prc_Trip_Name_TB.ForeColor = Color.Black;
            }
        }
        private void Nav_Prc_Trip_Name_TB_Leave(object sender, EventArgs e)
        {
            if (Nav_Prc_Trip_Name_TB.Text == "")
            {
                Nav_Prc_Trip_Name_TB.Text = Nav_Prc_TripNameTBDefText;
                Nav_Prc_Trip_Name_TB.ForeColor = Color.Gray;
            }
        }
        #endregion Trip Name TB
        #region Trip Tags TB
        private void Nav_Prc_Trip_Tags_TB_TextChanged(object sender, EventArgs e)
        {
            Nav_Prc_TripTags = Nav_Prc_Trip_Tags_TB.Text;
        }
        private void Nav_Prc_Trip_Tags_TB_Enter(object sender, EventArgs e)
        {
            if (Nav_Prc_Trip_Tags_TB.Text == Nav_Prc_TripTagsTBDefText)
            {
                Nav_Prc_Trip_Tags_TB.Text = "";
                Nav_Prc_Trip_Tags_TB.ForeColor = Color.Black;
            }
        }
        private void Nav_Prc_Trip_Tags_TB_Leave(object sender, EventArgs e)
        {
            if (Nav_Prc_Trip_Tags_TB.Text == "")
            {
                Nav_Prc_Trip_Tags_TB.Text = Nav_Prc_TripTagsTBDefText;
                Nav_Prc_Trip_Tags_TB.ForeColor = Color.Gray;
            }
        }
        #endregion Trip Tags TB
        #endregion Text Boxes
        #region Tree Views
        #region Route TV
        private void Nav_Prc_RouteTV_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null)
            {
                return;
            }
            if ((e.Node.Level == 2) && (e.Button == System.Windows.Forms.MouseButtons.Left))
            {
                Nav_Route selRoute = (Nav_Route)e.Node.Tag;
                Nav_Prc_setRouteDistanceAndTimeText(selRoute);
            }
            else if ((e.Node.Level == 2) && (e.Button == System.Windows.Forms.MouseButtons.Right))
            {
                DialogResult result = System.Windows.Forms.DialogResult.Yes;
                if (Statics.Settings.Navigation.PromptOnRightClick == true)
                {
                    //For routes, prompt user
                    result = MessageBox.Show("Do you want to start processing '" + e.Node.Text + "'?", "Start Navigation?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                if (result != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }
                if (ChangeMonitor.LoggedIn == false)
                {
                    return;
                }
                Nav_Prc_FormatRouteAndProcess((Nav_Route)e.Node.Tag);
            }
            else if ((e.Node.Level == 3) && (e.Button == System.Windows.Forms.MouseButtons.Right))
            {
                Nav_Prc_FormatNodeAndProcess((Routes.UserRoutesRow)e.Node.Tag);
            }
            Nav_Prc_setRouteTVNode(e.Node);
        }
        private void Nav_Prc_RouteTV_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode selectedNode = Nav_Prc_RouteTV.SelectedNode;
            if (selectedNode == null)
            {
                return;
            }
            if (selectedNode.Level == 2)
            {
                Nav_Route route = (Nav_Route)selectedNode.Tag;
                Nav_Prc_setRouteDistanceAndTimeText(route);
                route.Direction = Nav_Prc_Forward;
                Nav_Prc_addRouteToTrip(route);
                Nav_Prc_deselectRouteTVNode();
            }
        }
        #endregion Route TV
        #region Trip Creation TV
        private void Nav_Prc_Trip_CreationTV_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null)
            {
                return;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //right click on a route or route node. Prompt the user to start processing the route.
                DialogResult result = System.Windows.Forms.DialogResult.Yes;
                if ((e.Node.Level == 0) && (Statics.Settings.Navigation.PromptOnRightClick == true))
                {
                    //For routes, prompt user
                    result = MessageBox.Show("Do you want to start processing '" + e.Node.Text + "'?", "Start Navigation?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                if (result != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }
                if (ChangeMonitor.LoggedIn == false)
                {
                    return;
                }
                if (Bots.Navigation.ProcessingStatus != Bots.Navigation.PROCESSING_STATUS.STOPPED)
                {
                    MessageBox.Show("A navigation is already in progress, please stop it before beginning another.");
                    return;
                }
                else if (e.Node.Level == 0)
                {
                    Nav_Prc_FormatRouteAndProcess((Nav_Route)e.Node.Tag);
                }
                else if (e.Node.Level == 1)
                {
                    Nav_Prc_FormatNodeAndProcess((Routes.UserRoutesRow)e.Node.Tag);
                }
                Nav_Prc_setTripCreationTVNode(e.Node);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.Node.Level == 0)
                {
                    Nav_Route selectedRoute = (Nav_Route)e.Node.Tag;
                    if (selectedRoute != null)
                    {
                        if ((selectedRoute.Direction == true) && (Nav_Prc_Forward == false))
                        {
                            Nav_Prc_setForwardRB(true);
                        }
                        else if((selectedRoute.Direction == false) && (Nav_Prc_Forward == true))
                        {
                            Nav_Prc_setReverseRB(true);
                        }
                        Nav_Prc_setRouteDistanceAndTimeText(selectedRoute);
                    }
                }
            }
        }
        private void Nav_Prc_Trip_CreationTV_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                TreeNode selNode = Nav_Prc_Trip_CreationTV.SelectedNode;
                if ((selNode == null) || (selNode.Level != 0))
                {
                    return;
                }
                Nav_Route delRoute = (Nav_Route)selNode.Tag;
                Nav_Prc_removeRouteFromTrip(delRoute);
                //Nav_Prc_deleteTripCreationTVNode(selNode);
            }
            if (e.KeyData == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                Nav_Prc_deselectTripCreationTVNode();
            }
        }
        #endregion Trip Creation TV
        #region Trip Selection TV
        private void Nav_Prc_Trip_SelectionTV_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //On single click of a trip, we'll load the trip into the creation TV.
            //On single click of a route, we do nothing.
            //On single click of a route node, we do nothing.
            //On right click we perform the trip/route/node
            if (e.Node == null)
            {
                return;
            }
            if ((e.Node.Level == 2) && (e.Clicks == 1) && (e.Button == System.Windows.Forms.MouseButtons.Left))
            {
                //single click on a trip. If current trip is modified, prompt user. Then load into the creation TV.
                DialogResult result = System.Windows.Forms.DialogResult.Yes;
                if (Nav_Prc_modified)
                {
                    result = MessageBox.Show("You will lose any unsaved work. Do you wish to proceed?", "Load Trip?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                if (result != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }
                Nav_Prc_CurrentTrip = (Nav_Trip)e.Node.Tag;
                Nav_Prc_loadTripCreationTV(Nav_Prc_CurrentTrip);
                Nav_Prc_setTripNameTBText(Nav_Prc_CurrentTrip.TripName);
                if (Nav_Prc_CurrentTrip.TripTags != "")
                {
                    Nav_Prc_setTripTagsTBText(Nav_Prc_CurrentTrip.TripTags);
                }
                else
                {
                    Nav_Prc_setTripTagsTBText(Nav_Prc_TripTagsTBDefText);
                }
                Nav_Prc_setTripDistanceAndTimeText(Nav_Prc_CurrentTrip);
                Nav_Prc_modified = false;
            }
            else if ((e.Node.Level == 3) && (e.Clicks == 1) && (e.Button == System.Windows.Forms.MouseButtons.Left))
            {
                Nav_Route selectedRoute = (Nav_Route)e.Node.Tag;
                Nav_Prc_setRouteDistanceAndTimeText(selectedRoute);
            }
            else if ((e.Node.Level >= 2) && (e.Button == System.Windows.Forms.MouseButtons.Right))
            {
                //double click on a trip. Prompt the user to start processing the trip.
                DialogResult result = System.Windows.Forms.DialogResult.Yes;
                if ((e.Node.Level < 4) && (Statics.Settings.Navigation.PromptOnRightClick == true))
                {
                    //For trips and routes, prompt user
                    result = MessageBox.Show("Do you want to start processing '" + e.Node.Text + "'?", "Start Navigation?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                if (result != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }
                if (ChangeMonitor.LoggedIn == false)
                {
                    return;
                }
                if (Bots.Navigation.ProcessingStatus != Bots.Navigation.PROCESSING_STATUS.STOPPED)
                {
                    MessageBox.Show("A navigation is already in progress, please stop it before beginning another.");
                    return;
                }
                if (e.Node.Level == 2)
                {
                    Nav_Prc_FormatTripAndProcess((Nav_Trip)e.Node.Tag);
                }
                else if (e.Node.Level == 3)
                {
                    Nav_Prc_FormatRouteAndProcess((Nav_Route)e.Node.Tag);
                }
                else if (e.Node.Level == 4)
                {
                    Nav_Prc_FormatNodeAndProcess((Routes.UserRoutesRow)e.Node.Tag);
                }
                Nav_Prc_setTripSelectionTVNode(e.Node);
            }
        }
        private void Nav_Prc_Trip_SelectionTV_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                TreeNode selNode = Nav_Prc_Trip_SelectionTV.SelectedNode;
                if ((selNode == null) || (selNode.Level != 2))
                {
                    return;
                }
                Nav_Trip trip = (Nav_Trip)selNode.Tag;
                if (Nav_Prc_deleteTrip(trip.TripName, true, true, true) == 0xffffffff)
                {
                    return;
                }
                TreeNode[] allCopies = Nav_Prc_Trip_SelectionTV.Nodes.Find(selNode.Name, true);
                Nav_Prc_deleteTripSelectionTVNode(allCopies);
            }
        }
        #endregion Trip Selection TV
        #endregion Tree Views
        #region Radio Buttons
        private void Nav_Prc_Forward_RB_CheckedChanged(object sender, EventArgs e)
        {
            if (Nav_Prc_Forward_RB.Checked == true)
            {
                if (Nav_Prc_reverseCheckedProgrammatically == true)
                {
                    //If this check is due to the user selecting a route in the trip creation TV,
                    //then we don't want to make any route changes, just set the RB properly.
                    Nav_Prc_Forward = true;
                }
                else
                {
                    TreeNode selNode = Nav_Prc_Trip_CreationTV.SelectedNode;
                    //If we have a route selected (trip creationi tv) then we need to change the direction of that route
                    //AND all instances of that route (current route and possibly several instances in the Trip Selection TV).
                    if ((Nav_Prc_Trip_CreationTV.SelectedNode != null) && (Nav_Prc_Trip_CreationTV.SelectedNode.Level == 0))
                    {
                        //This is for the current trip's route, but all of the same routes in the trip selection TV
                        //are the same objects (they are not cloned), so we don't need to change them individually.
                        Nav_Route routeToFlip = Nav_Prc_CurrentTrip.FindRoute(((Nav_Route)Nav_Prc_Trip_CreationTV.SelectedNode.Tag).RouteID);
                        if (routeToFlip != null)
                        {
                            routeToFlip.Direction = true;
                            Nav_formatRoute(routeToFlip,             //iRoute
                                            !routeToFlip.Direction,  //iReverseNodes
                                            true);                   //iFlipHeadings
                            Nav_Prc_Trip_CreationTV.SelectedNode.Text = routeToFlip.ToString();
                            Nav_Prc_Trip_CreationTV.SelectedNode.Name = routeToFlip.ToString();
                            Nav_Prc_updateTripSelectionTVNode(Nav_Prc_CurrentTrip, routeToFlip);
                        }
                        Nav_Prc_setTripCreationTVNode(selNode);
                    }
                    //Otherwise the user is just changing the direction that the next added route will take.
                    //So just set the variable and that's it.
                    else
                    {
                        Nav_Prc_Forward = true;
                    }
                }
                Nav_Prc_reverseCheckedProgrammatically = false;
            }
        }
        private void Nav_Prc_Reverse_RB_CheckedChanged(object sender, EventArgs e)
        {
            if (Nav_Prc_Reverse_RB.Checked == true)
            {
                if (Nav_Prc_reverseCheckedProgrammatically == true)
                {
                    //If this check is due to the user selecting a route in the trip creation TV,
                    //then we don't want to make any route changes, just set the RB properly.
                    Nav_Prc_Forward = false;
                }
                else
                {
                    TreeNode selNode = Nav_Prc_Trip_CreationTV.SelectedNode;
                    //If we have a route selected (trip creationi tv) then we need to change the direction of that route
                    //AND all instances of that route (current route and possibly several instances in the Trip Selection TV).
                    if ((Nav_Prc_Trip_CreationTV.SelectedNode != null) && (Nav_Prc_Trip_CreationTV.SelectedNode.Level == 0))
                    {
                        //This is for the current trip's route, but all of the same routes in the trip selection TV
                        //are the same objects (they are not cloned), so we don't need to change them individually.
                        Nav_Route routeToFlip = Nav_Prc_CurrentTrip.FindRoute(((Nav_Route)Nav_Prc_Trip_CreationTV.SelectedNode.Tag).RouteID);
                        if (routeToFlip != null)
                        {
                            bool canReverse = Nav_canReverseRoute(routeToFlip);
                            if (canReverse)
                            {
                                routeToFlip.Direction = false;
                                Nav_formatRoute(routeToFlip,             //iRoute
                                                !routeToFlip.Direction,  //iReverseNodes
                                                true);                   //iFlipHeadings
                                Nav_Prc_Trip_CreationTV.SelectedNode.Text = routeToFlip.ToString();
                                Nav_Prc_Trip_CreationTV.SelectedNode.Name = routeToFlip.ToString();
                                Nav_Prc_updateTripSelectionTVNode(Nav_Prc_CurrentTrip, routeToFlip);
                            }
                            else
                            {
                                String message = "You cannot reverse a route that has one of the following nodes:\n";
                                message += "1. Keystroke\n2. Target NPC\n3. Trade Item\n4. Trade Gil";
                                MessageBox.Show(message, "Cannot Reverse Route", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Nav_Prc_setForwardRB(true);
                            }
                        }
                        Nav_Prc_setTripCreationTVNode(selNode);
                    }
                    //Otherwise the user is just changing the direction that the next added route will take.
                    //So just set the variable and that's it.
                    else
                    {
                        Nav_Prc_Forward = false;
                    }
                    //Nav_Prc_refreshTripSelectionTV();
                    Nav_Prc_Trip_CreationTV.Refresh();
                }
                Nav_Prc_reverseCheckedProgrammatically = false;
            }
        }
        #endregion Radio Buttons
        #region Buttons
        private void Nav_Prc_Start_Button_Click(object sender, EventArgs e)
        {
            if (ChangeMonitor.LoggedIn == false)
            {
                return;
            }
            if (Bots.Navigation.ProcessingStatus == Bots.Navigation.PROCESSING_STATUS.STOPPED)
            {
                Nav_Prc_FormatTripAndProcess(Nav_Prc_CurrentTrip);
            }
            else if (Bots.Navigation.ProcessingStatus == Bots.Navigation.PROCESSING_STATUS.RUNNING)
            {
                Bots.Navigation.PauseProcessing();
                Statics.FuncPtrs.SetNavButtonPtr("&Resume", Statics.Buttons.Green);
            }
            else if (Bots.Navigation.ProcessingStatus == Bots.Navigation.PROCESSING_STATUS.PAUSED)
            {
                Bots.Navigation.ResumeProcessing();
                Statics.FuncPtrs.SetNavButtonPtr("&Pause", Statics.Buttons.Yellow);
            }
        }
        private void Nav_Prc_Stop_Button_Click(object sender, EventArgs e)
        {
            Bots.Navigation.StopProcessing();
        }
        private void Nav_Prc_Save_Trip_Button_Click(object sender, EventArgs e)
        {
            Nav_Prc_saveTrip();
        }
        private void Nav_Prc_Clear_Trip_Button_Click(object sender, EventArgs e)
        {
            DialogResult promptResult;
            if (Nav_Rec_modified == true)
            {
                promptResult = promptResult = MessageBox.Show("You will lose any unsaved work. Do you wish to proceed?", "Load Trip?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else
            {
                promptResult = System.Windows.Forms.DialogResult.Yes;
            }
            if (promptResult != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }
            Nav_Prc_CurrentTrip = new Nav_Trip();
            Nav_Prc_clearTripCreationTV();
            Nav_Prc_setTripNameTBText(Nav_Prc_TripNameTBDefText);
            Nav_Prc_setTripTagsTBText(Nav_Prc_TripTagsTBDefText);
            Nav_Prc_exisingTripLoaded = false;
        }
        #endregion Buttons
        #region UpDowns
        private void Nav_Prc_Loop_UpDn_ValueChanged(object sender, EventArgs e)
        {
            Nav_Prc_Loop_Cnt = (UInt32)Nav_Prc_Loop_UpDn.Value;
        }
        #endregion UpDowns
        #region Resizing
        private void NAV_Form_ResizeEnd(object sender, EventArgs e)
        {
            Int32 new_tv_width = m_TOP_Form_currentWidth - Nav_Prc_RouteTV.Left - 14;
            Nav_Prc_RouteTV.Width = new_tv_width / 3;
            Nav_Prc_Trip_SelectionTV.Width = new_tv_width / 3;
            Nav_Prc_Trip_CreationTV.Width = new_tv_width - Nav_Prc_RouteTV.Width - Nav_Prc_Trip_SelectionTV.Width - 14;
            Nav_Prc_Trip_CreationTV.Left = Nav_Prc_RouteTV.Right + 7;
            Nav_Prc_Trip_SelectionTV.Left = Nav_Prc_Trip_CreationTV.Right + 7;
            Nav_Prc_Trip_Name_TB.Left = Nav_Prc_Trip_CreationTV.Left;
            Nav_Prc_Trip_Tags_TB.Left = Nav_Prc_Trip_CreationTV.Left;
            Nav_Prc_Trip_Name_TB.Width = Nav_Prc_Trip_CreationTV.Width;
            Nav_Prc_Trip_Tags_TB.Width = Nav_Prc_Trip_CreationTV.Width;
            Nav_Prc_Route_Distance_Label.Left = Nav_Prc_Trip_CreationTV.Left - 3;
            Nav_Prc_Route_Distance_Text.Left = Nav_Prc_Trip_CreationTV.Left + 63;
            Nav_Prc_Route_Time_Label.Left = Nav_Prc_Trip_CreationTV.Left - 3;
            Nav_Prc_Route_Time_Text.Left = Nav_Prc_Trip_CreationTV.Left + 63;
            Nav_Prc_Trip_Distance_Label.Left = Nav_Prc_Trip_CreationTV.Left - 3;
            Nav_Prc_Trip_Distance_Text.Left = Nav_Prc_Trip_CreationTV.Left + 63;
            Nav_Prc_Trip_Time_Label.Left = Nav_Prc_Trip_CreationTV.Left - 3;
            Nav_Prc_Trip_Time_Text.Left = Nav_Prc_Trip_CreationTV.Left + 63;

            Nav_Prc_RouteTV.Height = Nav_Prc_Forward_RB.Top - 32;
            Nav_Prc_Trip_SelectionTV.Height = Nav_Prc_RouteTV.Height;
            Nav_Prc_Trip_CreationTV.Height = Nav_Prc_RouteTV.Height - 52;
            Nav_Prc_Trip_Name_TB.Top = Nav_Prc_Trip_CreationTV.Bottom + 7;
            Nav_Prc_Trip_Tags_TB.Top = Nav_Prc_Trip_Name_TB.Top + 26;
            Nav_Prc_Route_Distance_Label.Top = Nav_Prc_Trip_Tags_TB.Top + 23;
            Nav_Prc_Route_Distance_Text.Top = Nav_Prc_Route_Distance_Label.Top;
            Nav_Prc_Route_Time_Label.Top = Nav_Prc_Route_Distance_Label.Top + 17;
            Nav_Prc_Route_Time_Text.Top = Nav_Prc_Route_Distance_Label.Top + 17;
            Nav_Prc_Trip_Distance_Label.Top = Nav_Prc_Route_Time_Label.Top + 17;
            Nav_Prc_Trip_Distance_Text.Top = Nav_Prc_Route_Time_Label.Top + 17;
            Nav_Prc_Trip_Time_Label.Top = Nav_Prc_Trip_Distance_Label.Top + 15;
            Nav_Prc_Trip_Time_Text.Top = Nav_Prc_Trip_Distance_Label.Top + 15;

            Nav_Prc_Trip_CreationTV_Label.Left = Nav_Prc_Trip_CreationTV.Left + 5;
            Nav_Prc_Trip_SelectionTV_Label.Left = Nav_Prc_Trip_SelectionTV.Left + 2;
        }
        #endregion Resizing
        #endregion Event Handlers
        #endregion Route Processing

        #region User Route Recording
        #region Enums
        private enum NAV_REC_STATE :byte
        {
            STOPPED = 0,
            RUNNING = 1
        }
        #endregion Enums
        #region Members
        #region Misc
        private NAV_REC_STATE Nav_Rec_State = NAV_REC_STATE.STOPPED;
        private Thread Nav_Rec_RecordingThread = null;
        private bool Nav_Rec_RecordingThreadrRunning = false;
        private Thread Nav_Rec_StartStopClickThread = null;
        private bool Nav_Rec_modified = false;
        private uint Nav_Rec_xyhUpdatePeriod = 500;
        #endregion Misc
        #region Default Values
        private String Nav_Rec_RouteNameTBDefText = "Route Name";
        private String Nav_Rec_RouteTagsTBDefText = "Comma Separated Tags";
        private String Nav_Rec_NpcNameTBDefText = "NPC Name";
        private String Nav_Rec_CommandTBDefText = "Command Text";
        private String Nav_Rec_ItemNameTBDefText = "Item Name";
        private double Nav_Rec_ItemQuanDefValue = 1D;
        private double Nav_Rec_GilQuanDefValue = 300D;
        private double Nav_Rec_PosXDefValue = 999.9D;
        private double Nav_Rec_PosYDefValue = 999.9D;
        private float Nav_Rec_PosHDefValue = 0.0f;
        private Byte Nav_Rec_PosZoneDefValue = 0;
        #endregion Default Values
        #region GUI Value Parallels
        private String Nav_Rec_RouteName = "";
        private String Nav_Rec_RouteTags = "";
        private String Nav_Rec_NpcName = "";
        private String Nav_Rec_CommandText = "";
        private String Nav_Rec_ItemName = "";
        private double Nav_Rec_Wait;
        private double Nav_Rec_ItemQuan;
        private double Nav_Rec_GilQuan;
        private double Nav_Rec_PosX;
        private double Nav_Rec_PosY;
        private float Nav_Rec_PosH;
        private UInt16 Nav_Rec_Zone;
        #endregion GUI Value Parallels
        #region Current Route Values
        private Nav_Route Nav_Rec_CurrentRoute;
        private Routes.UserRoutesRow Nav_Rec_CurrentNode;
        private uint Nav_Rec_CurrentRouteID = 0;
        private bool Nav_Rec_existingRouteLoaded = false;
        #endregion Current Route Values
        #region Delegates
        private delegate void Nav_Rec_setRouteNameTBTextDelegate(String iText);
        private delegate void Nav_Rec_setRouteTagsTBTextDelegate(String iText);
        private delegate void Nav_Rec_setCommandTextTBTextDelegate(String iText);
        private delegate void Nav_Rec_setItemNameTBTextDelegate(String iText);
        private delegate void Nav_Rec_setWaitValueDelegate(double iValue);
        private delegate void Nav_Rec_setItemQuanValueDelegate(double iValue);
        private delegate void Nav_Rec_setGilQuanValueDelegate(double iValue);
        private delegate void Nav_Rec_setPosXValueDelegate(double iValue);
        private delegate void Nav_Rec_setPosYValueDelegate(double iValue);
        private delegate void Nav_Rec_setPosHValueDelegate(float iValue);
        private delegate void Nav_Rec_setPosZoneValueDelegate(UInt16 iValue);
        private delegate void Nav_Rec_loadDeleteCBDelegate();
        private delegate void Nav_Rec_setDeleteCBIndexDelegate(int iIdx);
        private delegate void Nav_Rec_loadKeystrokesCBDelegate();
        private delegate void Nav_Rec_setKeystrokesCBIndexDelegate(int iIdx);
        private delegate void Nav_Rec_setStartButtonDelegate(String iText, Color iColor);
        private delegate void Nav_Rec_addRouteLBItemDelegate(Routes.UserRoutesRow iRow);
        private delegate void Nav_Rec_updateRouteLBItemDelegate(int iIdx, Routes.UserRoutesRow iRow);
        private delegate void Nav_Rec_removeRouteLBItemDelegate(int iIdx);
        private delegate void Nav_Rec_selectRouteLBItemDelegate(int iIdx);
        private delegate void Nav_Rec_clearRouteLBDelegate();
        private delegate void Nav_Rec_refreshRouteLBDelegate();
        private delegate void Nav_Rec_scrollRouteLBDelegate(int iIdx);
        #endregion Delegates
        #region Function Pointers
        private Nav_Rec_setRouteNameTBTextDelegate Nav_Rec_setRouteNameTBTextPtr;
        private Nav_Rec_setRouteTagsTBTextDelegate Nav_Rec_setRouteTagsTBTextPtr;
        private Nav_Rec_setCommandTextTBTextDelegate Nav_Rec_setCommandTextTBTextPtr;
        private Nav_Rec_setItemNameTBTextDelegate Nav_Rec_setItemNameTBTextPtr;
        private Nav_Rec_setWaitValueDelegate Nav_Rec_setWaitValuePtr;
        private Nav_Rec_setItemQuanValueDelegate Nav_Rec_setItemQuanValuePtr;
        private Nav_Rec_setGilQuanValueDelegate Nav_Rec_setGilQuanValuePtr;
        private Nav_Rec_setPosXValueDelegate Nav_Rec_setPosXValuePtr;
        private Nav_Rec_setPosYValueDelegate Nav_Rec_setPosYValuePtr;
        private Nav_Rec_setPosHValueDelegate Nav_Rec_setPosHValuePtr;
        private Nav_Rec_setPosZoneValueDelegate Nav_Rec_setPosZoneValuePtr;
        private Nav_Rec_loadDeleteCBDelegate Nav_Rec_loadDeleteCBPtr;
        private Nav_Rec_setDeleteCBIndexDelegate Nav_Rec_setDeleteCBIndexPtr;
        private Nav_Rec_loadKeystrokesCBDelegate Nav_Rec_loadKeystrokesCBPtr;
        private Nav_Rec_setKeystrokesCBIndexDelegate Nav_Rec_setKeystrokesCBIndexPtr;
        private Nav_Rec_setStartButtonDelegate Nav_Rec_setStartButtonPtr;
        private Nav_Rec_addRouteLBItemDelegate Nav_Rec_addRouteLBItemPtr;
        private Nav_Rec_removeRouteLBItemDelegate Nav_Rec_removeRouteLBItemPtr;
        private Nav_Rec_updateRouteLBItemDelegate Nav_Rec_updateRouteLBItemPtr;
        private Nav_Rec_selectRouteLBItemDelegate Nav_Rec_selectRouteLBItemPtr;
        private Nav_Rec_clearRouteLBDelegate Nav_Rec_clearRouteLBPtr;
        private Nav_Rec_refreshRouteLBDelegate Nav_Rec_refreshRouteLBPtr;
        private Nav_Rec_scrollRouteLBDelegate Nav_Rec_scrollRouteLBPtr;
        #endregion Function Pointers
        #endregion Members
        #region Inits
        private void Nav_Rec_inits()
        {
            Nav_Rec_createDelegates();
            Nav_Rec_CurrentRoute = new Nav_Route();
            Nav_Rec_loadDeleteCB();
            Nav_Rec_loadKeystrokesCB();
            Nav_Rec_loadTextBoxDefText();
            Nav_Rec_loadUpDnDefValues();
            Nav_Rec_clearGuiParallelValues();
            Nav_Rec_Route_LB.DataSource = Nav_Rec_CurrentRoute.RouteNodes;
            Nav_Rec_Route_LB.DisplayMember = "NodeDetail";
        }
        private void Nav_Rec_createDelegates()
        {
            if (Nav_Rec_setRouteNameTBTextPtr == null)
            {
                Nav_Rec_setRouteNameTBTextPtr = new Nav_Rec_setRouteNameTBTextDelegate(Nav_Rec_setRouteNameTBTextCallBackFunction);
                Nav_Rec_setRouteTagsTBTextPtr = new Nav_Rec_setRouteTagsTBTextDelegate(Nav_Rec_setRouteTagsTBTextCallBackFunction);
                Nav_Rec_setCommandTextTBTextPtr = new Nav_Rec_setCommandTextTBTextDelegate(Nav_Rec_setCommandTextTBTextCallBackFunction);
                Nav_Rec_setItemNameTBTextPtr = new Nav_Rec_setItemNameTBTextDelegate(Nav_Rec_setItemNameTBTextCallBackFunction);
                Nav_Rec_setWaitValuePtr = new Nav_Rec_setWaitValueDelegate(Nav_Rec_setWaitValueCallBackFunction);
                Nav_Rec_setItemQuanValuePtr = new Nav_Rec_setItemQuanValueDelegate(Nav_Rec_setItemQuanValueCallBackFunction);
                Nav_Rec_setGilQuanValuePtr = new Nav_Rec_setGilQuanValueDelegate(Nav_Rec_setGilQuanValueCallBackFunction);
                Nav_Rec_setPosXValuePtr = new Nav_Rec_setPosXValueDelegate(Nav_Rec_setPosXValueCallBackFunction);
                Nav_Rec_setPosYValuePtr = new Nav_Rec_setPosYValueDelegate(Nav_Rec_setPosYValueCallBackFunction);
                Nav_Rec_setPosHValuePtr = new Nav_Rec_setPosHValueDelegate(Nav_Rec_setPosHValueCallBackFunction);
                Nav_Rec_setPosZoneValuePtr = new Nav_Rec_setPosZoneValueDelegate(Nav_Rec_setPosZoneValueCallBackFunction);
                Nav_Rec_loadDeleteCBPtr = new Nav_Rec_loadDeleteCBDelegate(Nav_Rec_loadDeleteCBCallBackFunction);
                Nav_Rec_setDeleteCBIndexPtr = new Nav_Rec_setDeleteCBIndexDelegate(Nav_Rec_setDeleteCBIndexCallBackFunction);
                Nav_Rec_loadKeystrokesCBPtr = new Nav_Rec_loadKeystrokesCBDelegate(Nav_Rec_loadKeystrokesCBCallBackFunction);
                Nav_Rec_setKeystrokesCBIndexPtr = new Nav_Rec_setKeystrokesCBIndexDelegate(Nav_Rec_setKeystrokesCBIndexCallBackFunction);
                Nav_Rec_setStartButtonPtr = new Nav_Rec_setStartButtonDelegate(Nav_Rec_setStartButtonCallBackFunction);
                Nav_Rec_addRouteLBItemPtr = new Nav_Rec_addRouteLBItemDelegate(Nav_Rec_addRouteLBItemCallBackFunction);
                Nav_Rec_removeRouteLBItemPtr = new Nav_Rec_removeRouteLBItemDelegate(Nav_Rec_removeRouteLBItemCallBackFunction);
                Nav_Rec_updateRouteLBItemPtr = new Nav_Rec_updateRouteLBItemDelegate(Nav_Rec_updateRouteLBItemCallBackFunction);
                Nav_Rec_selectRouteLBItemPtr = new Nav_Rec_selectRouteLBItemDelegate(Nav_Rec_selectRouteLBItemCallBackFunction);
                Nav_Rec_clearRouteLBPtr = new Nav_Rec_clearRouteLBDelegate(Nav_Rec_clearRouteLBCallBackFunction);
                Nav_Rec_refreshRouteLBPtr = new Nav_Rec_refreshRouteLBDelegate(Nav_Rec_refreshRouteLBCallBackFunction);
                Nav_Rec_scrollRouteLBPtr = new Nav_Rec_scrollRouteLBDelegate(Nav_Rec_scrollRouteLBCallBackFunction);
            }
        }
        private void Nav_Rec_loadUserSettings()
        {
            Statics.Settings.Navigation.IntervalDefValue = (double)UserSettings.GetValue(UserSettings.BOT.NAV, "Nav_Rec_IntervalDefValue");
            Statics.Settings.Navigation.MinDistDefValue = (double)UserSettings.GetValue(UserSettings.BOT.NAV, "Nav_Rec_MinDistDefValue");
            Statics.Settings.Navigation.WaitDefValue = (double)UserSettings.GetValue(UserSettings.BOT.NAV, "Nav_Rec_WaitDefValue");
            Nav_Rec_loadUpDnDefValues();
        }
        private void Nav_Rec_loadTextBoxDefText()
        {
            Nav_Rec_setRouteNameTBText(Nav_Rec_RouteNameTBDefText);
            Nav_Rec_setRouteTagsTBText(Nav_Rec_RouteTagsTBDefText);
            Nav_Rec_setCommandTextTBText(Nav_Rec_CommandTBDefText);
            Nav_Rec_setItemNametTBText(Nav_Rec_ItemNameTBDefText);
        }
        private void Nav_Rec_loadUpDnDefValues()
        {
            Nav_Rec_setWaitValue(Statics.Settings.Navigation.WaitDefValue);
            Nav_Rec_setItemQuanValue(Nav_Rec_ItemQuanDefValue);
            Nav_Rec_setGilQuanValue(Nav_Rec_GilQuanDefValue);
            Nav_Rec_setPosXValue(Nav_Rec_PosXDefValue);
            Nav_Rec_setPosYValue(Nav_Rec_PosYDefValue);
            Nav_Rec_setPosHValue(Nav_Rec_PosHDefValue);
            Nav_Rec_setPosZoneValue(Nav_Rec_PosZoneDefValue);
        }
        private void Nav_Rec_clearGuiParallelValues()
        {
            Nav_Rec_RouteName = "";
            Nav_Rec_RouteTags = "";
            Nav_Rec_NpcName = "";
            Nav_Rec_CommandText = "";
            Nav_Rec_ItemName = "";
            Nav_Rec_Wait = Statics.Settings.Navigation.WaitDefValue;
            Nav_Rec_ItemQuan = Nav_Rec_ItemQuanDefValue;
            Nav_Rec_GilQuan = Nav_Rec_GilQuanDefValue;
            Nav_Rec_PosX = Nav_Rec_PosXDefValue;
            Nav_Rec_PosY = Nav_Rec_PosYDefValue;
            Nav_Rec_PosH = Nav_Rec_PosHDefValue;
            Nav_Rec_Zone = Nav_Rec_PosZoneDefValue;
        }
        #endregion Inits
        #region Utility Functions
        #region GUI Updates
        #region Text Box Updates
        private void Nav_Rec_setRouteNameTBText(String iText)
        {
            try
            {
                if (Nav_Rec_Route_Name_TB.InvokeRequired)
                {
                    Nav_Rec_Route_Name_TB.Invoke(Nav_Rec_setRouteNameTBTextPtr, new object[] { iText });
                }
                else
                {
                    Nav_Rec_setRouteNameTBTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_setRouteNameTBText: " + e.ToString());
            }
        }
        private void Nav_Rec_setRouteNameTBTextCallBackFunction(String iText)
        {
            Nav_Rec_Route_Name_TB.Text = iText;
            if (iText != Nav_Rec_RouteNameTBDefText)
            {
                Nav_Rec_Route_Name_TB.ForeColor = Color.Black;
            }
            else
            {
                Nav_Rec_Route_Name_TB.ForeColor = Color.Gray;
            }
        }
        private void Nav_Rec_setRouteTagsTBText(String iText)
        {
            try
            {
                if (Nav_Rec_Route_Tags_TB.InvokeRequired)
                {
                    Nav_Rec_Route_Tags_TB.Invoke(Nav_Rec_setRouteTagsTBTextPtr, new object[] { iText });
                }
                else
                {
                    Nav_Rec_setRouteTagsTBTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_setRouteTagsTBText: " + e.ToString());
            }
        }
        private void Nav_Rec_setRouteTagsTBTextCallBackFunction(String iText)
        {
            Nav_Rec_Route_Tags_TB.Text = iText;
            if (iText != Nav_Rec_RouteTagsTBDefText)
            {
                Nav_Rec_Route_Tags_TB.ForeColor = Color.Black;
            }
            else
            {
                Nav_Rec_Route_Tags_TB.ForeColor = Color.Gray;
            }
        }
        private void Nav_Rec_setCommandTextTBText(String iText)
        {
            try
            {
                if (Nav_Rec_Command_TB.InvokeRequired)
                {
                    Nav_Rec_Command_TB.Invoke(Nav_Rec_setCommandTextTBTextPtr, new object[] { iText });
                }
                else
                {
                    Nav_Rec_setCommandTextTBTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_setCommandTextTBText: " + e.ToString());
            }
        }
        private void Nav_Rec_setCommandTextTBTextCallBackFunction(String iText)
        {
            Nav_Rec_Command_TB.Text = iText;
            if (iText != Nav_Rec_CommandTBDefText)
            {
                Nav_Rec_Command_TB.ForeColor = Color.Black;
            }
            else
            {
                Nav_Rec_Command_TB.ForeColor = Color.Gray;
            }
        }
        private void Nav_Rec_setItemNametTBText(String iText)
        {
            try
            {
                if (Nav_Rec_Trade_Item_TB.InvokeRequired)
                {
                    Nav_Rec_Trade_Item_TB.Invoke(Nav_Rec_setItemNameTBTextPtr, new object[] { iText });
                }
                else
                {
                    Nav_Rec_setItemNameTBTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_setItemNameTBText: " + e.ToString());
            }
        }
        private void Nav_Rec_setItemNameTBTextCallBackFunction(String iText)
        {
            Nav_Rec_Trade_Item_TB.Text = iText;
            if (iText != Nav_Rec_ItemNameTBDefText)
            {
                Nav_Rec_Trade_Item_TB.ForeColor = Color.Black;
            }
            else
            {
                Nav_Rec_Trade_Item_TB.ForeColor = Color.Gray;
            }
        }
        #endregion Text Box Updates
        #region UpDown Value Updates
        private void Nav_Rec_setWaitValue(double iValue)
        {
            try
            {
                if (Nav_Rec_Wait_UpDn.InvokeRequired)
                {
                    Nav_Rec_Wait_UpDn.Invoke(Nav_Rec_setWaitValuePtr, new object[] { iValue });
                }
                else
                {
                    Nav_Rec_setWaitValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_setWaitValue: " + e.ToString());
            }
        }
        private void Nav_Rec_setWaitValueCallBackFunction(double iValue)
        {
            Nav_Rec_Wait_UpDn.Value = (decimal)iValue;
        }
        private void Nav_Rec_setItemQuanValue(double iValue)
        {
            try
            {
                if (Nav_Rec_Trade_Item_UpDn.InvokeRequired)
                {
                    Nav_Rec_Trade_Item_UpDn.Invoke(Nav_Rec_setItemQuanValuePtr, new object[] { iValue });
                }
                else
                {
                    Nav_Rec_setItemQuanValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_setItemQuanValue: " + e.ToString());
            }
        }
        private void Nav_Rec_setItemQuanValueCallBackFunction(double iValue)
        {
            Nav_Rec_Trade_Item_UpDn.Value = (decimal)iValue;
        }
        private void Nav_Rec_setGilQuanValue(double iValue)
        {
            try
            {
                if (Nav_Rec_Trade_Gil_UpDn.InvokeRequired)
                {
                    Nav_Rec_Trade_Gil_UpDn.Invoke(Nav_Rec_setGilQuanValuePtr, new object[] { iValue });
                }
                else
                {
                    Nav_Rec_setGilQuanValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_setGilQuanValue: " + e.ToString());
            }
        }
        private void Nav_Rec_setGilQuanValueCallBackFunction(double iValue)
        {
            Nav_Rec_Trade_Gil_UpDn.Value = (decimal)iValue;
        }
        private void Nav_Rec_setPosXValue(double iValue)
        {
            try
            {
                if (Nav_Rec_Position_X_UpDn.InvokeRequired)
                {
                    Nav_Rec_Position_X_UpDn.Invoke(Nav_Rec_setPosXValuePtr, new object[] { iValue });
                }
                else
                {
                    Nav_Rec_setPosXValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_setPosXValue: " + e.ToString());
            }
        }
        private void Nav_Rec_setPosXValueCallBackFunction(double iValue)
        {
            Nav_Rec_Position_X_UpDn.Value = (decimal)iValue;
        }
        private void Nav_Rec_setPosYValue(double iValue)
        {
            try
            {
                if (Nav_Rec_Position_Y_UpDn.InvokeRequired)
                {
                    Nav_Rec_Position_Y_UpDn.Invoke(Nav_Rec_setPosYValuePtr, new object[] { iValue });
                }
                else
                {
                    Nav_Rec_setPosYValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_setPosYValue: " + e.ToString());
            }
        }
        private void Nav_Rec_setPosYValueCallBackFunction(double iValue)
        {
            Nav_Rec_Position_Y_UpDn.Value = (decimal)iValue;
        }
        private void Nav_Rec_setPosHValue(float iValue)
        {
            try
            {
                if (Nav_Rec_Position_H_UpDn.InvokeRequired)
                {
                    Nav_Rec_Position_H_UpDn.Invoke(Nav_Rec_setPosHValuePtr, new object[] { iValue });
                }
                else
                {
                    Nav_Rec_setPosHValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_setPosHValue: " + e.ToString());
            }
        }
        private void Nav_Rec_setPosHValueCallBackFunction(float iValue)
        {
            Nav_Rec_Position_H_UpDn.Value = (decimal)iValue;
        }
        #endregion UpDown Value Updates
        #region ComboBox Updates
        private void Nav_Rec_loadDeleteCB()
        {
            try
            {
                if (Nav_Rec_Delete_CB.InvokeRequired)
                {
                    Nav_Rec_Delete_CB.Invoke(Nav_Rec_loadDeleteCBPtr);
                }
                else
                {
                    Nav_Rec_loadDeleteCBCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_loadDeleteCB: " + e.ToString());
            }
        }
        private void Nav_Rec_loadDeleteCBCallBackFunction()
        {
            if (Nav_userRouteNames != null)
            {
                Nav_Rec_Delete_CB.BeginUpdate();
                Nav_Rec_Delete_CB.Items.Clear();
                foreach (String str in Nav_userRouteNames)
                {
                    Nav_Rec_Delete_CB.Items.Add(str);
                }
                Nav_Rec_Delete_CB.EndUpdate();
            }
        }
        private void Nav_Rec_setDeleteCBIndex(int iIdx)
        {
            try
            {
                if (Nav_Rec_Delete_CB.InvokeRequired)
                {
                    Nav_Rec_Delete_CB.Invoke(Nav_Rec_setDeleteCBIndexPtr, new object[] { iIdx });
                }
                else
                {
                    Nav_Rec_setDeleteCBIndexCallBackFunction(iIdx);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_setDeleteCBIndex: " + e.ToString());
            }
        }
        private void Nav_Rec_setDeleteCBIndexCallBackFunction(int iIdx)
        {
            if (iIdx >= 0)
            {
                Nav_Rec_Delete_CB.SelectedIndex = iIdx;
            }
        }
        private void Nav_Rec_loadKeystrokesCB()
        {
            try
            {
                if (Nav_Rec_Key_Stroke_CB.InvokeRequired)
                {
                    Nav_Rec_Key_Stroke_CB.Invoke(Nav_Rec_loadKeystrokesCBPtr);
                }
                else
                {
                    Nav_Rec_loadKeystrokesCBCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_loadKeystrokesCB: " + e.ToString());
            }
        }
        private void Nav_Rec_loadKeystrokesCBCallBackFunction()
        {

            foreach (String str in Statics.Constants.Navigation.KeystrokeStrings)
            {
                Nav_Rec_Key_Stroke_CB.Items.Add(str);
            }
        }
        private void Nav_Rec_setKeystrokesCBIndex(int iIdx)
        {
            try
            {
                if (Nav_Rec_Key_Stroke_CB.InvokeRequired)
                {
                    Nav_Rec_Key_Stroke_CB.Invoke(Nav_Rec_setKeystrokesCBIndexPtr, new object[] { iIdx });
                }
                else
                {
                    Nav_Rec_setKeystrokesCBIndexCallBackFunction(iIdx);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_setKeystrokesCBIndex: " + e.ToString());
            }
        }
        private void Nav_Rec_setKeystrokesCBIndexCallBackFunction(int iIdx)
        {
            if (iIdx >= 0)
            {
                Nav_Rec_Key_Stroke_CB.SelectedIndex = iIdx;
            }
        }
        #endregion ComboBox Updates
        #region Button Updates
        private void Nav_Rec_setStartButton(String iText, Color iColor)
        {
            try
            {
                if (Nav_Rec_Start_Stop_Button.InvokeRequired)
                {
                    Nav_Rec_Start_Stop_Button.Invoke(Nav_Rec_setStartButtonPtr, new object[] { iText, iColor });
                }
                else
                {
                    Nav_Rec_setStartButtonCallBackFunction(iText, iColor);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_setStartButton: " + e.ToString());
            }
        }
        private void Nav_Rec_setStartButtonCallBackFunction(String iText, Color iColor)
        {
            Nav_Rec_Start_Stop_Button.Text = iText;
            Nav_Rec_Start_Stop_Button.BackColor = iColor;
        }
        #endregion Button Updates
        #region Label Updates
        private void Nav_Rec_setPosZoneValue(UInt16 iValue)
        {
            try
            {
                if (Nav_Rec_Zone_Text.InvokeRequired)
                {
                    Nav_Rec_Zone_Text.Invoke(Nav_Rec_setPosZoneValuePtr, new object[] { iValue });
                }
                else
                {
                    Nav_Rec_setPosZoneValueCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_setPosZoneValue: " + e.ToString());
            }
        }
        private void Nav_Rec_setPosZoneValueCallBackFunction(UInt16 iValue)
        {
            Nav_Rec_Zone_Text.Text = iValue.ToString();
        }
        #endregion Label Updates
        #region List Box Updates
        private void Nav_Rec_refreshRouteLB()
        {
            try
            {
                if (Nav_Rec_Route_LB.InvokeRequired)
                {
                    Nav_Rec_Route_LB.Invoke(Nav_Rec_refreshRouteLBPtr);
                }
                else
                {
                    Nav_Rec_refreshRouteLBCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_refreshRouteLB: " + e.ToString());
            }
        }
        private void Nav_Rec_refreshRouteLBCallBackFunction()
        {
            int topIdx = Nav_Rec_Route_LB.TopIndex;
            ((CurrencyManager)Nav_Rec_Route_LB.BindingContext[Nav_Rec_Route_LB.DataSource]).Refresh();
            Nav_Rec_scrollRouteLB(topIdx);
        }
        private void Nav_Rec_addRouteLBItem(Routes.UserRoutesRow iRow)
        {
            try
            {
                if (Nav_Rec_Route_LB.InvokeRequired)
                {
                    Nav_Rec_Route_LB.Invoke(Nav_Rec_addRouteLBItemPtr, new object[] { iRow });
                }
                else
                {
                    Nav_Rec_addRouteLBItemCallBackFunction(iRow);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_addRouteLBItem: " + e.ToString());
            }
        }
        private void Nav_Rec_addRouteLBItemCallBackFunction(Routes.UserRoutesRow iRow)
        {
            Nav_Rec_Route_LB.Items.Add(iRow);
        }
        private void Nav_Rec_removeRouteLBItem(int iIdx)
        {
            try
            {
                if (Nav_Rec_Route_LB.InvokeRequired)
                {
                    Nav_Rec_Route_LB.Invoke(Nav_Rec_removeRouteLBItemPtr, new object[] { iIdx });
                }
                else
                {
                    Nav_Rec_removeRouteLBItemCallBackFunction(iIdx);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_removeRouteLBItem: " + e.ToString());
            }
        }
        private void Nav_Rec_removeRouteLBItemCallBackFunction(int iIdx)
        {
            Nav_Rec_Route_LB.Items.RemoveAt(iIdx);
        }
        private void Nav_Rec_updateRouteLBItem(int iIdx, Routes.UserRoutesRow iRow)
        {
            try
            {
                if (Nav_Rec_Route_LB.InvokeRequired)
                {
                    Nav_Rec_Route_LB.Invoke(Nav_Rec_updateRouteLBItemPtr, new object[] { iIdx, iRow });
                }
                else
                {
                    Nav_Rec_updateRouteLBItemCallBackFunction(iIdx, iRow);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_updateRouteLBItem: " + e.ToString());
            }
        }
        private void Nav_Rec_updateRouteLBItemCallBackFunction(int iIdx, Routes.UserRoutesRow iRow)
        {
            ((Routes.UserRoutesRow)Nav_Rec_Route_LB.Items[iIdx]).RouteID = iRow.RouteID;
            ((Routes.UserRoutesRow)Nav_Rec_Route_LB.Items[iIdx]).RouteName = iRow.RouteName;
            ((Routes.UserRoutesRow)Nav_Rec_Route_LB.Items[iIdx]).RouteStartName = iRow.RouteStartName;
            ((Routes.UserRoutesRow)Nav_Rec_Route_LB.Items[iIdx]).RouteEndName = iRow.RouteEndName;
            ((Routes.UserRoutesRow)Nav_Rec_Route_LB.Items[iIdx]).NodeID = iRow.NodeID;
            ((Routes.UserRoutesRow)Nav_Rec_Route_LB.Items[iIdx]).NodeType = iRow.NodeType;
            ((Routes.UserRoutesRow)Nav_Rec_Route_LB.Items[iIdx]).NodeData = iRow.NodeData;
            ((Routes.UserRoutesRow)Nav_Rec_Route_LB.Items[iIdx]).NodeDetail = iRow.NodeDetail;
            ((Routes.UserRoutesRow)Nav_Rec_Route_LB.Items[iIdx]).NodePosX = iRow.NodePosX;
            ((Routes.UserRoutesRow)Nav_Rec_Route_LB.Items[iIdx]).NodePosY = iRow.NodePosY;
            ((Routes.UserRoutesRow)Nav_Rec_Route_LB.Items[iIdx]).NodePosHeading = iRow.NodePosHeading;
            ((Routes.UserRoutesRow)Nav_Rec_Route_LB.Items[iIdx]).NodeZoneID = iRow.NodeZoneID;
        }
        private void Nav_Rec_selectRouteLBItem(int iIdx)
        {
            try
            {
                if (Nav_Rec_Route_LB.InvokeRequired)
                {
                    Nav_Rec_Route_LB.Invoke(Nav_Rec_selectRouteLBItemPtr, new object[] { iIdx });
                }
                else
                {
                    Nav_Rec_selectRouteLBItemCallBackFunction(iIdx);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_selectRouteLBItem: " + e.ToString());
            }
        }
        private void Nav_Rec_selectRouteLBItemCallBackFunction(int iIdx)
        {
            Nav_Rec_Route_LB.SelectedIndex = iIdx;
        }
        private void Nav_Rec_clearRouteLB()
        {
            try
            {
                if (Nav_Rec_Route_LB.InvokeRequired)
                {
                    Nav_Rec_Route_LB.Invoke(Nav_Rec_clearRouteLBPtr);
                }
                else
                {
                    Nav_Rec_clearRouteLBCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_clearRouteLB: " + e.ToString());
            }
        }
        private void Nav_Rec_clearRouteLBCallBackFunction()
        {
            Nav_Rec_Route_LB.Items.Clear();
        }
        private void Nav_Rec_scrollRouteLB()
        {
            Nav_Rec_scrollRouteLB(Nav_Rec_Route_LB.Items.Count - 1);
        }
        private void Nav_Rec_scrollRouteLB(int iIdx)
        {
            try
            {
                if (Nav_Rec_Route_LB.InvokeRequired)
                {
                    Nav_Rec_Route_LB.Invoke(Nav_Rec_scrollRouteLBPtr, new object[] { iIdx });
                }
                else
                {
                    Nav_Rec_scrollRouteLBCallBackFunction(iIdx);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In Nav_Rec_scrollRouteLB: " + e.ToString());
            }
        }
        private void Nav_Rec_scrollRouteLBCallBackFunction(int iIdx)
        {
            Nav_Rec_Route_LB.TopIndex = iIdx;
        }
        #endregion List Box Updates
        #endregion GUI Updates
        #region Node Creation
        private UInt32 Nav_Rec_checkRouteIdInc()
        {
            if ((Nav_Rec_CurrentRoute.RouteNodes.Count == 0) && (Nav_Rec_existingRouteLoaded == false))
            {
                Nav_maxRouteId++;
                Nav_Rec_CurrentRouteID = Nav_maxRouteId;
            }
            return Nav_Rec_CurrentRouteID;
        }
        private UInt32 Nav_Rec_peekRouteIdInc()
        {
            if ((Nav_Rec_CurrentRoute.RouteNodes.Count == 0) && (Nav_Rec_existingRouteLoaded == false))
            {
                return Nav_maxRouteId + 1;
            }
            else
            {
                return Nav_Rec_CurrentRouteID;
            }
        }
        private bool Nav_Rec_checkAllFieldsFilledIn()
        {
            if (Nav_Rec_RouteName == Nav_Rec_RouteNameTBDefText)
            {
                MessageBox.Show("Please enter a Route Name");
                return false;
            }
            return true;
        }
        private bool Nav_Rec_checkNameExists()
        {
            if (Nav_Rec_existingRouteLoaded == true)
            {
                //We're working with an existing route, need to check if the name has changed.
                //If it has, check that the new name does not already exist.
                if (Nav_Rec_RouteName != Nav_Rec_CurrentRoute.RouteName)
                {
                    if (Nav_userRouteNames.Contains(Nav_Rec_RouteName))
                    {
                        MessageBox.Show("A route with that name already exists,\nplease enter a different name and save again.");
                        return true;
                    }
                }
            }
            else
            {
                //We're working with a new route, just check if the name already exists.

                if (Nav_userRouteNames.Contains(Nav_Rec_RouteName))
                {
                    MessageBox.Show("A route with that name already exists,\nplease enter a different name and save again.");
                    return true;
                }
            }
            return false;
        }
        private void Nav_Rec_addPosNode()
        {
            Nav_Rec_CurrentNode = Statics.Datasets.RoutesDb._UserRoutes.NewUserRoutesRow();
            Nav_Rec_CurrentNode.RouteID = Nav_Rec_checkRouteIdInc();
            Nav_Rec_CurrentNode.RouteName = "";
            Nav_Rec_CurrentNode.RouteStartName = "";
            Nav_Rec_CurrentNode.RouteEndName = "";
            Nav_Rec_CurrentNode.RouteTags = "";
            Nav_Rec_CurrentNode.NodeID = (UInt32)Nav_Rec_CurrentRoute.RouteNodes.Count;
            Nav_Rec_CurrentNode.NodeType = (Byte)NAV_NODE_TYPE.POS_NODE;
            Nav_Rec_CurrentNode.NodeData = 0;
            Nav_Rec_CurrentNode.NodePosX = (float)Nav_Rec_PosX;
            Nav_Rec_CurrentNode.NodePosY = (float)Nav_Rec_PosY;
            Nav_Rec_CurrentNode.NodePosHeading = Nav_Rec_PosH;
            Nav_Rec_CurrentNode.NodeZoneID = Nav_Rec_Zone;
            Nav_Rec_CurrentNode.NodeDetail = Nav_encodePosToString(Nav_Rec_PosX, Nav_Rec_PosY, Nav_Rec_PosH, Nav_Rec_Zone);
            Nav_Rec_CurrentRoute.RouteNodes.Add(Nav_Rec_CurrentNode);
            Nav_Rec_refreshRouteLB();
            Nav_Rec_scrollRouteLB();
            Nav_Rec_modified = true;
        }
        private void Nav_Rec_addNpcTargetNode()
        {
            if (ChangeMonitor.LoggedIn == false)
            {
                return;
            }

            Data.Entry.TextboxParameter param = new Data.Entry.TextboxParameter("NPC To Target", Nav_Rec_NpcNameTBDefText, true, true);
            Data.Entry.DataEntry form = new Data.Entry.DataEntry(this, new List<Data.Entry.ControlParameter> { param });
            DialogResult rslt = form.ShowDialog(this);
            if (rslt == DialogResult.OK)
            {
                Nav_Rec_NpcName = ((Data.Entry.TextboxReturn)form.ControlReturns[0]).Value;
            }
            else
            {
                return;
            }

            if ((Nav_Rec_NpcName != Nav_Rec_NpcNameTBDefText) && (Nav_Rec_NpcName != ""))
            {
                if (Nav_checkForTripsWithReverseRoute(Nav_Rec_peekRouteIdInc()))
                {
                    String message = "This route is part of a trip in the reverse direction.\n";
                    message += "You cannot use a route in reverse that contains NPC Target nodes.\n";
                    message += "Please either remove the route from the trip or change the direction to forward.";
                    MessageBox.Show(message);
                    return;
                }
                Nav_Rec_CurrentNode = Statics.Datasets.RoutesDb._UserRoutes.NewUserRoutesRow();
                Nav_Rec_CurrentNode.RouteID = Nav_Rec_checkRouteIdInc();
                Nav_Rec_CurrentNode.RouteName = "";
                Nav_Rec_CurrentNode.RouteStartName = "";
                Nav_Rec_CurrentNode.RouteEndName = "";
                Nav_Rec_CurrentNode.RouteTags = "";
                Nav_Rec_CurrentNode.NodeID = (UInt32)Nav_Rec_CurrentRoute.RouteNodes.Count;
                Nav_Rec_CurrentNode.NodeType = (Byte)NAV_NODE_TYPE.NPC_TARGET;
                Nav_Rec_CurrentNode.NodeData = 0;
                Nav_Rec_CurrentNode.NodePosX = (float)Nav_Rec_PosX;
                Nav_Rec_CurrentNode.NodePosY = (float)Nav_Rec_PosY;
                Nav_Rec_CurrentNode.NodePosHeading = Nav_Rec_PosH;
                Nav_Rec_CurrentNode.NodeZoneID = Nav_Rec_Zone;
                Nav_Rec_CurrentNode.NodeDetail = Nav_encodeNameToString(Nav_Rec_NpcName);
                Nav_Rec_CurrentRoute.RouteNodes.Add(Nav_Rec_CurrentNode);
                Nav_Rec_refreshRouteLB();
                Nav_Rec_scrollRouteLB();
                Nav_Rec_modified = true;
            }
            else
            {
                MessageBox.Show("Please enter an NPC name.");
            }
        }
        private void Nav_Rec_addNpcTradeItemNode()
        {
            if ((Nav_Rec_ItemName != Nav_Rec_ItemNameTBDefText) && (Nav_Rec_ItemName != ""))
            {
                if (Nav_checkForTripsWithReverseRoute(Nav_Rec_peekRouteIdInc()))
                {
                    String message = "This route is part of a trip in the reverse direction.\n";
                    message += "You cannot use a route in reverse that contains Trade Item nodes.\n";
                    message += "Please either remove the route from the trip or change the direction to forward.";
                    MessageBox.Show(message);
                    return;
                }
                Nav_Rec_CurrentNode = Statics.Datasets.RoutesDb._UserRoutes.NewUserRoutesRow();
                Nav_Rec_CurrentNode.RouteID = Nav_Rec_checkRouteIdInc();
                Nav_Rec_CurrentNode.RouteName = "";
                Nav_Rec_CurrentNode.RouteStartName = "";
                Nav_Rec_CurrentNode.RouteEndName = "";
                Nav_Rec_CurrentNode.RouteTags = "";
                Nav_Rec_CurrentNode.NodeID = (UInt32)Nav_Rec_CurrentRoute.RouteNodes.Count;
                Nav_Rec_CurrentNode.NodeType = (Byte)NAV_NODE_TYPE.NPC_TRADE_ITEM;
                Nav_Rec_CurrentNode.NodeData = (UInt32)Nav_Rec_ItemQuan;
                Nav_Rec_CurrentNode.NodePosX = (float)Nav_Rec_PosX;
                Nav_Rec_CurrentNode.NodePosY = (float)Nav_Rec_PosY;
                Nav_Rec_CurrentNode.NodePosHeading = Nav_Rec_PosH;
                Nav_Rec_CurrentNode.NodeZoneID = Nav_Rec_Zone;
                Nav_Rec_CurrentNode.NodeDetail = Nav_encodeItemToString(Nav_Rec_ItemName, (Byte)Nav_Rec_ItemQuan);
                Nav_Rec_CurrentRoute.RouteNodes.Add(Nav_Rec_CurrentNode);
                Nav_Rec_refreshRouteLB();
                Nav_Rec_scrollRouteLB();
                Nav_Rec_modified = true;
            }
            else
            {
                MessageBox.Show("Please enter an Item name");
            }
        }
        private void Nav_Rec_addNpcTradeGilNode()
        {
            if (Nav_checkForTripsWithReverseRoute(Nav_Rec_peekRouteIdInc()))
            {
                String message = "This route is part of a trip in the reverse direction.\n";
                message += "You cannot use a route in reverse that contains Trade Gil nodes.\n";
                message += "Please either remove the route from the trip or change the direction to forward.";
                MessageBox.Show(message);
                return;
            }
            Nav_Rec_CurrentNode = Statics.Datasets.RoutesDb._UserRoutes.NewUserRoutesRow();
            Nav_Rec_CurrentNode.RouteID = Nav_Rec_checkRouteIdInc();
            Nav_Rec_CurrentNode.RouteName = "";
            Nav_Rec_CurrentNode.RouteStartName = "";
            Nav_Rec_CurrentNode.RouteEndName = "";
            Nav_Rec_CurrentNode.RouteTags = "";
            Nav_Rec_CurrentNode.NodeID = (UInt32)Nav_Rec_CurrentRoute.RouteNodes.Count;
            Nav_Rec_CurrentNode.NodeType = (Byte)NAV_NODE_TYPE.NPC_TRADE_GIL;
            Nav_Rec_CurrentNode.NodeData = (UInt32)Nav_Rec_GilQuan;
            Nav_Rec_CurrentNode.NodePosX = (float)Nav_Rec_PosX;
            Nav_Rec_CurrentNode.NodePosY = (float)Nav_Rec_PosY;
            Nav_Rec_CurrentNode.NodePosHeading = Nav_Rec_PosH;
            Nav_Rec_CurrentNode.NodeZoneID = Nav_Rec_Zone;
            Nav_Rec_CurrentNode.NodeDetail = Nav_encodeGilToString((UInt32)Nav_Rec_GilQuan);
            Nav_Rec_CurrentRoute.RouteNodes.Add(Nav_Rec_CurrentNode);
            Nav_Rec_refreshRouteLB();
            Nav_Rec_scrollRouteLB();
            Nav_Rec_modified = true;
        }
        private void Nav_Rec_addCommandNode()
        {
            if ((Nav_Rec_CommandText != Nav_Rec_CommandTBDefText) && (Nav_Rec_CommandText != ""))
            {
                Nav_Rec_CurrentNode = Statics.Datasets.RoutesDb._UserRoutes.NewUserRoutesRow();
                Nav_Rec_CurrentNode.RouteID = Nav_Rec_checkRouteIdInc();
                Nav_Rec_CurrentNode.RouteName = "";
                Nav_Rec_CurrentNode.RouteStartName = "";
                Nav_Rec_CurrentNode.RouteEndName = "";
                Nav_Rec_CurrentNode.RouteTags = "";
                Nav_Rec_CurrentNode.NodeID = (UInt32)Nav_Rec_CurrentRoute.RouteNodes.Count;
                Nav_Rec_CurrentNode.NodeType = (Byte)NAV_NODE_TYPE.COMMAND;
                Nav_Rec_CurrentNode.NodeData = 0;
                Nav_Rec_CurrentNode.NodePosX = (float)Nav_Rec_PosX;
                Nav_Rec_CurrentNode.NodePosY = (float)Nav_Rec_PosY;
                Nav_Rec_CurrentNode.NodePosHeading = Nav_Rec_PosH;
                Nav_Rec_CurrentNode.NodeZoneID = Nav_Rec_Zone;
                Nav_Rec_CurrentNode.NodeDetail = Nav_Rec_CommandText;
                Nav_Rec_CurrentRoute.RouteNodes.Add(Nav_Rec_CurrentNode);
                Nav_Rec_refreshRouteLB();
                Nav_Rec_scrollRouteLB();
                Nav_Rec_modified = true;
            }
            else
            {
                MessageBox.Show("Please enter the Command text");
            }
        }
        private void Nav_Rec_addKeystrokeNode()
        {
            if (Nav_Rec_Key_Stroke_CB.SelectedIndex >= 0)
            {
                if (Nav_checkForTripsWithReverseRoute(Nav_Rec_peekRouteIdInc()))
                {
                    String message = "This route is part of a trip in the reverse direction.\n";
                    message += "You cannot use a route in reverse that contains Keystroke nodes.\n";
                    message += "Please either remove the route from the trip or change the direction to forward.";
                    MessageBox.Show(message);
                    return;
                }
                Nav_Rec_CurrentNode = Statics.Datasets.RoutesDb._UserRoutes.NewUserRoutesRow();
                Nav_Rec_CurrentNode.RouteID = Nav_Rec_checkRouteIdInc();
                Nav_Rec_CurrentNode.RouteName = "";
                Nav_Rec_CurrentNode.RouteStartName = "";
                Nav_Rec_CurrentNode.RouteEndName = "";
                Nav_Rec_CurrentNode.RouteTags = "";
                Nav_Rec_CurrentNode.NodeID = (UInt32)Nav_Rec_CurrentRoute.RouteNodes.Count;
                Nav_Rec_CurrentNode.NodeType = (Byte)NAV_NODE_TYPE.KEYSTROKE;
                Nav_Rec_CurrentNode.NodeData = (UInt32)Nav_Rec_Key_Stroke_CB.SelectedIndex;
                Nav_Rec_CurrentNode.NodePosX = (float)Nav_Rec_PosX;
                Nav_Rec_CurrentNode.NodePosY = (float)Nav_Rec_PosY;
                Nav_Rec_CurrentNode.NodePosHeading = Nav_Rec_PosH;
                Nav_Rec_CurrentNode.NodeZoneID = Nav_Rec_Zone;
                Nav_Rec_CurrentNode.NodeDetail = Nav_encodeKeystrokeToString(Statics.Constants.Navigation.KeystrokeStrings[Nav_Rec_Key_Stroke_CB.SelectedIndex]);
                Nav_Rec_CurrentRoute.RouteNodes.Add(Nav_Rec_CurrentNode);
                Nav_Rec_refreshRouteLB();
                Nav_Rec_scrollRouteLB();
                Nav_Rec_modified = true;
            }
        }
        private void Nav_Rec_addWaitNode()
        {
            Nav_Rec_CurrentNode = Statics.Datasets.RoutesDb._UserRoutes.NewUserRoutesRow();
            Nav_Rec_CurrentNode.RouteID = Nav_Rec_checkRouteIdInc();
            Nav_Rec_CurrentNode.RouteName = "";
            Nav_Rec_CurrentNode.RouteStartName = "";
            Nav_Rec_CurrentNode.RouteEndName = "";
            Nav_Rec_CurrentNode.RouteTags = "";
            Nav_Rec_CurrentNode.NodeID = (UInt32)Nav_Rec_CurrentRoute.RouteNodes.Count;
            Nav_Rec_CurrentNode.NodeType = (Byte)NAV_NODE_TYPE.WAIT;
            Nav_Rec_CurrentNode.NodeData = (UInt32)(Nav_Rec_Wait * 1000);
            Nav_Rec_CurrentNode.NodePosX = (float)Nav_Rec_PosX;
            Nav_Rec_CurrentNode.NodePosY = (float)Nav_Rec_PosY;
            Nav_Rec_CurrentNode.NodePosHeading = Nav_Rec_PosH;
            Nav_Rec_CurrentNode.NodeZoneID = Nav_Rec_Zone;
            Nav_Rec_CurrentNode.NodeDetail = Nav_encodeWaitToString(Nav_Rec_Wait);
            Nav_Rec_CurrentRoute.RouteNodes.Add(Nav_Rec_CurrentNode);
            Nav_Rec_refreshRouteLB();
            Nav_Rec_scrollRouteLB();
            Nav_Rec_modified = true;
        }
        #endregion Node Creation
        #region Node Deletion/Editing
        private void Nav_Rec_deleteNode()
        {
            if (Nav_Rec_Route_LB.SelectedItems.Count > 0)
            {
                int selectedIdx = Nav_Rec_Route_LB.SelectedIndex;
                if (Nav_Rec_CurrentRoute.RouteNodes[selectedIdx].RowState != DataRowState.Detached)
                {
                    Statics.Datasets.RoutesDb._UserRoutes.Rows.Remove(Nav_Rec_CurrentRoute.RouteNodes[selectedIdx]);
                }
                Nav_Rec_CurrentRoute.RouteNodes.RemoveAt(selectedIdx);
                
                //Need to go thru each successive node and set the nodeId appropriately.
                for (int ii = selectedIdx; ii < Nav_Rec_CurrentRoute.RouteNodes.Count; ii++)
                {
                    Routes.UserRoutesRow row = Nav_Rec_CurrentRoute.RouteNodes[ii];
                    row.NodeID = (uint)ii;
                }
                int topIndex = Nav_Rec_Route_LB.TopIndex;
                Nav_Rec_refreshRouteLB();
                Nav_Rec_scrollRouteLB(topIndex);
            }
        }
        private void Nav_Rec_loadFormWithNodeData(int iIdx)
        {
            // TBD - This all needs to be updated.
            if (iIdx < 0)
            {
                return;
            }
            if (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.COMMAND)
            {
                Nav_Rec_setCommandTextTBText(Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeDetail);
            }
            else if (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.KEYSTROKE)
            {
                String keystroke = "";
                if (!Nav_decodeStringToKeystroke(Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeDetail, ref keystroke))
                {
                    return;
                }
                int index = Statics.Constants.Navigation.KeystrokeStrings.IndexOf(keystroke);
                if (index >= 0)
                {
                    Nav_Rec_setKeystrokesCBIndex(index);
                }
            }
            else if (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.NPC_TRADE_GIL)
            {
                uint gilQuan = Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeData;
                Nav_Rec_setGilQuanValue((double)gilQuan);
            }
            else if (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.NPC_TRADE_ITEM)
            {
                String itemName = "";
                byte itemQuan = 0;
                if (!Nav_decodeStringToItem(Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeDetail, ref itemName, ref itemQuan))
                {
                    return;
                }
                Nav_Rec_setItemNametTBText(itemName);
                Nav_Rec_setItemQuanValue((double)itemQuan);
            }
            else if (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.WAIT)
            {
                Nav_Rec_setWaitValue((double)Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeData / (double)1000);
            }
            else if ((Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.POS_NODE)
                || (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.POS_START)
                || (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.POS_END)
                || (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.POS_ZONE))
            {
                double posx = 0;
                double posy = 0;
                float posh = 0;
                UInt16 zone = 0;
                if (!Nav_decodeStringToPos(Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeDetail, ref posx, ref posy, ref posh, ref zone))
                {
                    return;
                }
                Nav_Rec_setPosXValue(posx);
                Nav_Rec_setPosYValue(posy);
                Nav_Rec_setPosHValue(posh);
                Nav_Rec_setPosZoneValue(zone);
                Nav_Rec_Zone = zone;
            }
        }
        private void Nav_Rec_saveNode(int iIdx)
        {
            if (iIdx < 0)
            {
                return;
            }
            if (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.COMMAND)
            {
                if ((Nav_Rec_CommandText != Nav_Rec_CommandTBDefText) && (Nav_Rec_CommandText != ""))
                {
                    Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeDetail = Nav_Rec_CommandText;
                }
                else
                {
                    MessageBox.Show("Please enter the Command text");
                    return;
                }
            }
            else if (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.KEYSTROKE)
            {
                Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeData = (UInt32)Nav_Rec_Key_Stroke_CB.SelectedIndex;
                Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeDetail = Nav_encodeKeystrokeToString(Statics.Constants.Navigation.KeystrokeStrings[Nav_Rec_Key_Stroke_CB.SelectedIndex]);
            }
            else if (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.NPC_TARGET)
            {
                // TBD - This needs to be updated.
                if ((Nav_Rec_NpcName != Nav_Rec_NpcNameTBDefText) && (Nav_Rec_NpcName != ""))
                {
                    Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeDetail = Nav_encodeNameToString(Nav_Rec_NpcName);
                }
                else
                {
                    MessageBox.Show("Please enter an NPC name");
                    return;
                }
            }
            else if (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.NPC_TRADE_GIL)
            {
                Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeData = (UInt32)Nav_Rec_GilQuan;
                Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeDetail = Nav_encodeGilToString((UInt32)Nav_Rec_GilQuan);
                
            }
            else if (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.NPC_TRADE_ITEM)
            {
                if ((Nav_Rec_ItemName != Nav_Rec_ItemNameTBDefText) && (Nav_Rec_ItemName != ""))
                {
                    Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeData = (UInt32)Nav_Rec_ItemQuan;
                    Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeDetail = Nav_encodeItemToString(Nav_Rec_ItemName, (Byte)Nav_Rec_ItemQuan);
                }
                else
                {
                    MessageBox.Show("Please enter an Item name");
                    return;
                }
            }
            else if (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.WAIT)
            {
                Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeData = (UInt32)(Nav_Rec_Wait * 1000);
                Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeDetail = Nav_encodeWaitToString(Nav_Rec_Wait);
            }
            else if ((Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.POS_NODE)
                || (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.POS_START)
                || (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.POS_END)
                || (Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeType == (ushort)NAV_NODE_TYPE.POS_ZONE))
            {
                Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodePosX = (float)Nav_Rec_PosX;
                Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodePosY = (float)Nav_Rec_PosY;
                Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodePosHeading = Nav_Rec_PosH;
                Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeZoneID = Nav_Rec_Zone;
                Nav_Rec_CurrentRoute.RouteNodes[iIdx].NodeDetail = Nav_encodePosToString(Nav_Rec_PosX, Nav_Rec_PosY, Nav_Rec_PosH, Nav_Rec_Zone);
            }
            Nav_Rec_refreshRouteLB();
            Nav_Rec_modified = true;
        }
        private void Nav_Rec_loadRoute()
        {
            if (Nav_Rec_Delete_CB.SelectedIndex >= 0)
            {
                DialogResult promptResult;
                if (Nav_Rec_modified == true)
                {
                    promptResult = promptResult = MessageBox.Show("You will lose any unsaved work. Do you wish to proceed?", "Load Route?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                else
                {
                    promptResult = System.Windows.Forms.DialogResult.Yes;
                }
                if (promptResult != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }
                Nav_Rec_clearForm(false);
                
                uint routeId = 0;
                if (!Nav_userRoutesIdMap.TryGetValue(Nav_Rec_Delete_CB.SelectedItem.ToString(), out routeId))
                {
                    MessageBox.Show("Could not find route '" + Nav_Rec_Delete_CB.SelectedItem.ToString() + "'");
                    return;
                }
                LoggingFunctions.Debug("TopNAV::Nav_Rec_loadRoute: Loading route routeId " + routeId + ".", LoggingFunctions.DBG_SCOPE.TOP);
                String filter = "RouteID=" + routeId.ToString();
                String orderBy = "NodeID";
                Routes.UserRoutesRow[] rows = (Routes.UserRoutesRow[])Statics.Datasets.RoutesDb._UserRoutes.Select(filter, orderBy);
                if (rows.Length == 0)
                {
                    MessageBox.Show("Could not find route '" + Nav_Rec_Delete_CB.SelectedItem.ToString() + "' with RouteID = " + routeId);
                    return;
                }
                Nav_Rec_CurrentRoute.RouteNodes.AddRange(rows);
                Nav_Rec_CurrentRoute.RouteID = Nav_Rec_CurrentRoute.RouteNodes[0].RouteID;
                Nav_Rec_CurrentRoute.RouteName = Nav_Rec_CurrentRoute.RouteNodes[0].RouteName;
                Nav_Rec_refreshRouteLB();
                if (Nav_Rec_CurrentRoute.RouteNodes.Count > 0)
                {
                    Nav_Rec_setRouteNameTBText(Nav_Rec_CurrentRoute.RouteNodes[0].RouteName);
                    //Since we added tags after initial beta release, we need to check for a null value first.
                    if (Nav_Rec_CurrentRoute.RouteNodes[0].IsRouteTagsNull())
                    {
                        Nav_Rec_setRouteTagsTBText(Nav_Rec_RouteTagsTBDefText);
                    }
                    else if (Nav_Rec_CurrentRoute.RouteNodes[0].RouteTags == "")
                    {
                        Nav_Rec_setRouteTagsTBText(Nav_Rec_RouteTagsTBDefText);
                    }
                    else
                    {
                        Nav_Rec_setRouteTagsTBText(Nav_Rec_CurrentRoute.RouteNodes[0].RouteTags);
                    }
                }
                Nav_Rec_CurrentRouteID = Nav_Rec_CurrentRoute.RouteID;
                LoggingFunctions.Debug("TopNAV::Nav_Rec_loadRoute: Current route ID: " + Nav_Rec_CurrentRouteID + ".", LoggingFunctions.DBG_SCOPE.TOP);
                Nav_Rec_existingRouteLoaded = true;
                Nav_Rec_modified = false;
            }
        }
        #endregion Node Deletion/Editing
        #region Route Sorting/Parsing
        private bool Nav_Rec_checkRouteFormat()
        {
            if (!Nav_Rec_checkAllFieldsFilledIn())
            {
                return false;
            }
            if (Nav_Rec_checkNameExists())
            {
                return false;
            }
            String tagsParam;
            if (Nav_Rec_RouteTags == Nav_Rec_RouteTagsTBDefText)
            {
                tagsParam = "";
            }
            else
            {
                tagsParam = Nav_Rec_RouteTags;
            }
            if (tagsParam.Contains("'"))
            {
                MessageBox.Show("Tags cannot contain the ' (apostrophe) character.");
                return false;
            }
            if (Nav_formatRoute(Nav_Rec_CurrentRoute, Nav_Rec_RouteName, tagsParam))
            {
                Nav_Rec_refreshRouteLB();
                return true;
            }
            else
            {
                Nav_Rec_refreshRouteLB();
                return false;
            }
        }
        private void Nav_Rec_addRouteToDB()
        {
            if (Statics.Datasets.RoutesDb == null)
            {
                LoggingFunctions.Error("Route DB was null when trying to add a route.");
                MessageBox.Show("[ERROR] Route DB was null when trying to add a route.");
                return;
            }
            //Nav_PrintRoute(Nav_Rec_CurrentRoute, true);
            for (int ii = 0; ii < Nav_Rec_CurrentRoute.RouteNodes.Count; ii++)
            {
                LoggingFunctions.Debug("TopNAV::Nav_Rec_addRouteToDB: Saving route[" + Nav_Rec_CurrentRoute.RouteNodes[ii].RouteID + "] node[" + Nav_Rec_CurrentRoute.RouteNodes[ii].NodeID + "] @" + Nav_Rec_CurrentRoute.RouteNodes[ii].NodePosHeading + " row state: " + Nav_Rec_CurrentRoute.RouteNodes[ii].RowState.ToString() + ".", LoggingFunctions.DBG_SCOPE.TOP);
                if (Nav_Rec_CurrentRoute.RouteNodes[ii].RowState == DataRowState.Detached)
                {
                    Statics.Datasets.RoutesDb._UserRoutes.Rows.Add(Nav_Rec_CurrentRoute.RouteNodes[ii]);
                }
                else
                {
                    Nav_Rec_CurrentRoute.RouteNodes[ii].AcceptChanges();
                }
                LoggingFunctions.Debug("TopNAV::Nav_Rec_addRouteToDB: After route[" + Nav_Rec_CurrentRoute.RouteNodes[ii].RouteID + "] node[" + Nav_Rec_CurrentRoute.RouteNodes[ii].NodeID + "] @" + Nav_Rec_CurrentRoute.RouteNodes[ii].NodePosHeading + " row state: " + Nav_Rec_CurrentRoute.RouteNodes[ii].RowState.ToString() + ".", LoggingFunctions.DBG_SCOPE.TOP);
            }
            Nav_insertUserRouteName(Nav_Rec_CurrentRoute.RouteName, Nav_Rec_CurrentRoute.RouteID, Nav_Rec_CurrentRoute.RouteNodes[0].RouteTags);
            Nav_mergeRouteZonesIntoMap(Nav_Rec_CurrentRoute);
        }
        private void Nav_Rec_deleteRoute(String iRouteName)
        {
            if (!Nav_userRouteNames.Contains(iRouteName))
            {
                MessageBox.Show("Could not find user route name '" + iRouteName + "'");
                return;
            }
            else
            {
                DialogResult promptResult = MessageBox.Show("Are you sure you want to permenantly delete route " + iRouteName + "?", "Delete Route?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (promptResult == System.Windows.Forms.DialogResult.Yes)
                {
                    uint routeId = Nav_userRoutesIdMap[iRouteName];
                    Routes.UserRoutesRow[] rows = (Routes.UserRoutesRow[])Statics.Datasets.RoutesDb._UserRoutes.Select("RouteID=" + routeId.ToString());
                    if (rows.Length == 0)
                    {
                        MessageBox.Show("Could not find any routes matching '" + iRouteName + "'.");
                        return;
                    }
                    else
                    {
                        if (Nav_Rec_CurrentRoute.RouteNodes.Count > 0)
                        {
                            if (Nav_Rec_CurrentRoute.RouteNodes[0].RouteID == routeId)
                            {
                                if (Nav_userRouteNames.Count > 1)
                                {
                                    if (Nav_Rec_Delete_CB.SelectedIndex > 0)
                                    {
                                        Nav_Rec_setDeleteCBIndex(Nav_Rec_Delete_CB.SelectedIndex - 1);
                                    }
                                    else
                                    {
                                        Nav_Rec_setDeleteCBIndex(Nav_Rec_Delete_CB.SelectedIndex + 1);
                                    }
                                }
                                else
                                {
                                    Nav_Rec_clearForm(false);
                                }
                            }
                        }
                        int nbRows = rows.Length;
                        for (int ii = nbRows - 1; ii >= 0; ii--)
                        {
                            Statics.Datasets.RoutesDb._UserRoutes.Rows.Remove(rows[ii]);
                        }
                        Statics.Datasets.RoutesDb._UserRoutes.AcceptChanges();
                        Nav_userRouteNames.Remove(iRouteName);
                        Nav_userRoutesIdMap.Remove(iRouteName);
                        Nav_userRoutesTagsMap.Remove(iRouteName);
                        Nav_userRoutesZoneMap.Remove(iRouteName);
                        Nav_saveUserRoutes();
                        int selIdx = Nav_Rec_Delete_CB.SelectedIndex;
                        Nav_Rec_loadDeleteCB();
                    }
                }
            }
        }
        private void Nav_Rec_clearForm()
        {
            Nav_Rec_clearForm(true);
        }
        private void Nav_Rec_clearForm(bool iPromptUser)
        {
            DialogResult promptResult;
            if ((Nav_Rec_modified == true) && (iPromptUser == true))
            {
                promptResult = MessageBox.Show("You will lose any unsaved work. Do you wish to proceed?", "Clear Form?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else
            {
                promptResult = System.Windows.Forms.DialogResult.Yes;
            }
            if (promptResult == System.Windows.Forms.DialogResult.Yes)
            {
                Nav_Rec_clearGuiParallelValues();
                Nav_Rec_loadTextBoxDefText();
                Nav_Rec_loadUpDnDefValues();
                Nav_Rec_setKeystrokesCBIndex(0);
                Nav_Rec_CurrentRoute.Clear();
                Nav_Rec_refreshRouteLB();
                Nav_Rec_existingRouteLoaded = false;
                Nav_Rec_modified = false;
            }
        }
        #endregion Route Sorting/Parsing
        #region Recording Process
        private void Nav_Rec_StartRecording()
        {
            if (ChangeMonitor.LoggedIn == true)
            {
                if (Nav_Rec_RecordingThreadrRunning == true)
                {
                    Nav_Rec_State = NAV_REC_STATE.STOPPED;
                    TOP_updateStatusBox("Waiting for previous recording thread to finish.", Statics.Fields.Red);
                    while (Nav_Rec_RecordingThreadrRunning == true)
                    {
                        IocaineFunctions.delay(100);
                    }
                }
                Nav_Rec_setStartButton("Stop", Statics.Buttons.Red);
                Nav_Rec_State = NAV_REC_STATE.RUNNING;
                TOP_updateStatusBox("Recording route...", Statics.Fields.Green);
                Nav_Rec_RecordingThread = new Thread(new ThreadStart(Nav_Rec_recordThreadFunction));
                Nav_Rec_RecordingThread.Name = "Nav_Rec_RecordingThread";
                Nav_Rec_RecordingThread.IsBackground = true;
                Nav_Rec_RecordingThread.Start();
            }
        }
        private void Nav_Rec_StopRecording()
        {
            Nav_Rec_setStartButton("Start", Statics.Buttons.Green);
            Nav_Rec_State = NAV_REC_STATE.STOPPED;
            TOP_updateStatusBox("Waiting for previous recording thread to finish.", Statics.Fields.Red);
            while (Nav_Rec_RecordingThreadrRunning == true)
            {
                IocaineFunctions.delay(100);
            }
            Nav_Rec_RecordingThread = null;
            TOP_updateStatusBox("Recording stopped.", Statics.Fields.Blue);
        }
        private void Nav_Rec_updateXYHUpDn()
        {
            if (ChangeMonitor.LoggedIn == true)
            {
                Nav_Rec_setPosXValue(MemReads.Self.Position.get_x());
                Nav_Rec_setPosYValue(MemReads.Self.Position.get_y());
                Nav_Rec_setPosHValue(MemReads.Self.Position.get_heading());
            }
        }
        private void Nav_Rec_clearXYHUpDn()
        {
            Nav_Rec_setPosXValue(0d);
            Nav_Rec_setPosYValue(0d);
            Nav_Rec_setPosHValue(0f);
        }
        private bool Nav_Rec_distIsGreater(float iLastX, float iLastY)
        {
            //d = sqrt ( ( x1 - x0 )^2 + ( y1 - y0 )^2 )
            double dist = Math.Sqrt(Math.Pow(Nav_Rec_PosX - iLastX, 2) + Math.Pow(Nav_Rec_PosY - iLastY, 2));
            if (dist >= Statics.Settings.Navigation.MinDistDefValue)
            {
                if ((Nav_Rec_PosX == 0) && (Nav_Rec_PosY == 0) && (Nav_Rec_PosH == 0))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        private void Nav_Rec_editLastPositionHeading(float iNewHeading)
        {
            //Go backwards thru the route until we find a position node.
            //If the distance from that node to where we are now is less than
            //the min distance, set the heading of that node to our current heading.
            int nbNodes = Nav_Rec_CurrentRoute.RouteNodes.Count;
            for (int ii = nbNodes - 1; ii >= 0; ii--)
            {
                if ((Nav_Rec_CurrentRoute.RouteNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_NODE)
                    || (Nav_Rec_CurrentRoute.RouteNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_START)
                    || (Nav_Rec_CurrentRoute.RouteNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_END)
                    || (Nav_Rec_CurrentRoute.RouteNodes[ii].NodeType == (ushort)NAV_NODE_TYPE.POS_ZONE))
                {
                    if (Nav_Rec_distIsGreater(Nav_Rec_CurrentRoute.RouteNodes[ii].NodePosX, Nav_Rec_CurrentRoute.RouteNodes[ii].NodePosY) == false)
                    {
                        Nav_Rec_CurrentRoute.RouteNodes[ii].NodePosHeading = iNewHeading;
                    }
                    break;
                }
            }
            Nav_Rec_refreshRouteLB();
            Nav_Rec_scrollRouteLB();
        }
        #endregion Recording Process
        #endregion Utility Functions
        #region Recording Thread
        private void Nav_Rec_recordThreadFunction()
        {
            Nav_Rec_RecordingThreadrRunning = true;
            //We're going to update the x/y box every Nav_Rec_xyhUpdatePeriod ms, but we only want to record
            //a new point after the whole interval.
            //So we'll have an outter loop that runs every 100ms and a counter
            //that is checked to see if we want to record this time thru.
            uint loopsPerRecording = (uint)Statics.Settings.Navigation.IntervalDefValue / Nav_Rec_xyhUpdatePeriod;
            uint loopCounter = 0;
            bool firstTimeThru = true;
            float lastX = MemReads.Self.Position.get_x();
            float lastY = MemReads.Self.Position.get_y();
            UInt16 lastZone = MemReads.Self.get_zone_id();
            Nav_Rec_setPosZoneValue(lastZone);
            while (Nav_Rec_State == NAV_REC_STATE.RUNNING)
            {
                if (ChangeMonitor.LoggedIn == false)
                {
                    Nav_Rec_StopRecording();
                    continue;
                }
                if (MemReads.Self.get_is_zoning() == true)
                {
                    Nav_Rec_clearXYHUpDn();
                    Nav_Rec_Zone = 0;
                    lastZone = 0;
                    Nav_Rec_setPosZoneValue(0);
                    IocaineFunctions.delay(Nav_Rec_xyhUpdatePeriod);
                    continue;
                }
                else
                {
                    //Update the X/Y/H updn boxes.
                    Nav_Rec_updateXYHUpDn();
                }
                if (loopCounter == 0)
                {
                    //This is where we do the actual checking/recording of a new position point.
                    Nav_Rec_Zone = MemReads.Self.get_zone_id();
                    if (Nav_Rec_Zone == 0)
                    {
                        IocaineFunctions.delay(1000);
                        continue;
                    }
                    if (Nav_Rec_Zone != lastZone)
                    {
                        Nav_Rec_setPosZoneValue(Nav_Rec_Zone);
                    }
                    if (firstTimeThru == true)
                    {
                        Nav_Rec_addPosNode();
                        firstTimeThru = false;
                        loopCounter++;
                        IocaineFunctions.delay(Nav_Rec_xyhUpdatePeriod);
                        continue;
                    }
                    //Get the distance from the last point saved to our current point.
                    //If the distance is greater than the min distance setting, record a node.
                    if (Nav_Rec_distIsGreater(lastX, lastY) == true)
                    {
                        Nav_Rec_addPosNode();
                    }
                    //Finally, set the last x/y/h values
                    lastX = MemReads.Self.Position.get_x();
                    lastY = MemReads.Self.Position.get_y();
                    loopCounter++;
                }
                else if (loopCounter == loopsPerRecording)
                {
                    loopCounter = 0;
                }
                else
                {
                    loopCounter++;
                }
                IocaineFunctions.delay(Nav_Rec_xyhUpdatePeriod);
            }
            //Check the distance from the last point to where we are.
            //If it's greater than the min distance, record a node.
            if (Nav_Rec_distIsGreater(lastX, lastY) == true)
            {
                Nav_Rec_addPosNode();
            }
            else
            {
                Nav_Rec_editLastPositionHeading(Nav_Rec_PosH);
            }
            Nav_Rec_RecordingThreadrRunning = false;
        }
        #endregion Recording Thread
        #region Event Handlers
        #region Text Boxes
        #region Route Name TB
        private void Nav_Rec_Route_Name_TB_Enter(object sender, EventArgs e)
        {
            if (Nav_Rec_Route_Name_TB.Text == Nav_Rec_RouteNameTBDefText)
            {
                Nav_Rec_Route_Name_TB.Text = "";
                Nav_Rec_Route_Name_TB.ForeColor = Color.Black;
            }
        }
        private void Nav_Rec_Route_Name_TB_Leave(object sender, EventArgs e)
        {
            if (Nav_Rec_Route_Name_TB.Text == "")
            {
                Nav_Rec_Route_Name_TB.Text = Nav_Rec_RouteNameTBDefText;
                Nav_Rec_Route_Name_TB.ForeColor = Color.Gray;
            }
        }
        private void Nav_Rec_Route_Name_TB_TextChanged(object sender, EventArgs e)
        {
            Nav_Rec_RouteName = Nav_Rec_Route_Name_TB.Text;
        }
        #endregion Route Name TB
        #region Route Tags TB
        private void Nav_Rec_Route_Tags_TB_TextChanged(object sender, EventArgs e)
        {
            Nav_Rec_RouteTags = Nav_Rec_Route_Tags_TB.Text;
        }
        private void Nav_Rec_Route_Tags_TB_Enter(object sender, EventArgs e)
        {
            if (Nav_Rec_Route_Tags_TB.Text == Nav_Rec_RouteTagsTBDefText)
            {
                Nav_Rec_Route_Tags_TB.Text = "";
                Nav_Rec_Route_Tags_TB.ForeColor = Color.Black;
            }
        }
        private void Nav_Rec_Route_Tags_TB_Leave(object sender, EventArgs e)
        {
            if (Nav_Rec_Route_Tags_TB.Text == "")
            {
                Nav_Rec_Route_Tags_TB.Text = Nav_Rec_RouteTagsTBDefText;
                Nav_Rec_Route_Tags_TB.ForeColor = Color.Gray;
            }
        }
        #endregion Route Tags TB
        #region Command Text TB
        private void Nav_Rec_Command_TB_Enter(object sender, EventArgs e)
        {
            if (Nav_Rec_Command_TB.Text == Nav_Rec_CommandTBDefText)
            {
                Nav_Rec_Command_TB.Text = "";
                Nav_Rec_Command_TB.ForeColor = Color.Black;
            }
        }
        private void Nav_Rec_Command_TB_Leave(object sender, EventArgs e)
        {
            if (Nav_Rec_Command_TB.Text == "")
            {
                Nav_Rec_Command_TB.Text = Nav_Rec_CommandTBDefText;
                Nav_Rec_Command_TB.ForeColor = Color.Gray;
            }
        }
        private void Nav_Rec_Command_TB_TextChanged(object sender, EventArgs e)
        {
            Nav_Rec_CommandText = Nav_Rec_Command_TB.Text;
        }
        private void Nav_Rec_Command_TB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                Nav_Rec_addCommandNode();
            }
        }
        #endregion Command Text TB
        #region Trade Item TB
        private void Nav_Rec_Trade_Item_TB_Enter(object sender, EventArgs e)
        {
            if (Nav_Rec_Trade_Item_TB.Text == Nav_Rec_ItemNameTBDefText)
            {
                Nav_Rec_Trade_Item_TB.Text = "";
                Nav_Rec_Trade_Item_TB.ForeColor = Color.Black;
            }
        }
        private void Nav_Rec_Trade_Item_TB_Leave(object sender, EventArgs e)
        {
            if (Nav_Rec_Trade_Item_TB.Text == "")
            {
                Nav_Rec_Trade_Item_TB.Text = Nav_Rec_ItemNameTBDefText;
                Nav_Rec_Trade_Item_TB.ForeColor = Color.Gray;
            }
        }
        private void Nav_Rec_Trade_Item_TB_TextChanged(object sender, EventArgs e)
        {
            Nav_Rec_ItemName = Nav_Rec_Trade_Item_TB.Text;
        }
        private void Nav_Rec_Trade_Item_TB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                Nav_Rec_addNpcTradeItemNode();
            }
        }
        #endregion Trade Item TB
        #endregion Text Boxes
        #region UpDn Boxes
        private void Nav_Rec_Wait_UpDn_ValueChanged(object sender, EventArgs e)
        {
            Nav_Rec_Wait = (double)Nav_Rec_Wait_UpDn.Value;
        }
        private void Nav_Rec_Wait_UpDn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                Nav_Rec_addWaitNode();
            }
        }
        private void Nav_Rec_Trade_Item_UpDn_ValueChanged(object sender, EventArgs e)
        {
            Nav_Rec_ItemQuan = (double)Nav_Rec_Trade_Item_UpDn.Value;
        }
        private void Nav_Rec_Trade_Item_UpDn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                Nav_Rec_addNpcTradeItemNode();
            }
        }
        private void Nav_Rec_Trade_Gil_UpDn_ValueChanged(object sender, EventArgs e)
        {
            Nav_Rec_GilQuan = (double)Nav_Rec_Trade_Gil_UpDn.Value;
        }
        private void Nav_Rec_Trade_Gil_UpDn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                Nav_Rec_addNpcTradeGilNode();
            }
        }
        private void Nav_Rec_Position_X_UpDn_ValueChanged(object sender, EventArgs e)
        {
            Nav_Rec_PosX = (double)Nav_Rec_Position_X_UpDn.Value;
        }
        private void Nav_Rec_Position_Y_UpDn_ValueChanged(object sender, EventArgs e)
        {
            Nav_Rec_PosY = (double)Nav_Rec_Position_Y_UpDn.Value;
        }
        private void Nav_Rec_Position_H_UpDn_ValueChanged(object sender, EventArgs e)
        {
            Nav_Rec_PosH = (float)Nav_Rec_Position_H_UpDn.Value;
        }
        #endregion UpDn Boxes
        #region Buttons
        private void Nav_Rec_Target_NPC_Button_Click(object sender, EventArgs e)
        {
            Nav_Rec_addNpcTargetNode();
        }
        private void Nav_Rec_Wait_Button_Click(object sender, EventArgs e)
        {
            Nav_Rec_addWaitNode();
        }
        private void Nav_Rec_Command_Button_Click(object sender, EventArgs e)
        {
            Nav_Rec_addCommandNode();
        }
        private void Nav_Rec_Key_Stroke_Button_Click(object sender, EventArgs e)
        {
            Nav_Rec_addKeystrokeNode();
        }
        private void Nav_Rec_Trade_Item_Button_Click(object sender, EventArgs e)
        {
            Nav_Rec_addNpcTradeItemNode();
        }
        private void Nav_Rec_Trade_Gil_Button_Click(object sender, EventArgs e)
        {
            Nav_Rec_addNpcTradeGilNode();
        }
        private void Nav_Rec_Save_Point_Button_Click(object sender, EventArgs e)
        {
            Nav_Rec_addPosNode();
        }
        private void Nav_Rec_Delete_Node_Button_Click(object sender, EventArgs e)
        {
            Nav_Rec_deleteNode();
        }
        private void Nav_Rec_Start_Stop_Button_Click(object sender, EventArgs e)
        {
            if (Nav_Rec_State == NAV_REC_STATE.STOPPED)
            {
                Nav_Rec_StartStopClickThread = new Thread(new ThreadStart(Nav_Rec_StartRecording));
                Nav_Rec_StartStopClickThread.Name = "Nav_Rec_StartStopClickThread";
                Nav_Rec_StartStopClickThread.IsBackground = true;
                Nav_Rec_StartStopClickThread.Start();
            }
            else if (Nav_Rec_State == NAV_REC_STATE.RUNNING)
            {
                Nav_Rec_StartStopClickThread = new Thread(new ThreadStart(Nav_Rec_StopRecording));
                Nav_Rec_StartStopClickThread.Name = "Nav_Rec_StartStopClickThread";
                Nav_Rec_StartStopClickThread.IsBackground = true;
                Nav_Rec_StartStopClickThread.Start();
            }
        }
        private void Nav_Rec_Save_Button_Click(object sender, EventArgs e)
        {
            if (Nav_Rec_CurrentRoute.RouteNodes.Count == 0)
            {
                MessageBox.Show("You cannot save an empty route, save not completed.");
                return;
            }
            if (Nav_Rec_checkRouteFormat())
            {
                Nav_Rec_addRouteToDB();
                Nav_saveUserRoutes();
                Nav_Rec_loadDeleteCB();
                Nav_Rec_modified = false;
                Nav_Prc_loadRouteTV();
            }
        }
        private void Nav_Rec_Clear_Button_Click(object sender, EventArgs e)
        {
            Nav_Rec_clearForm();
        }
        private void Nav_Rec_Delete_Button_Click(object sender, EventArgs e)
        {
            if (Nav_Rec_Delete_CB.SelectedIndex >= 0)
            {
                Nav_Rec_deleteRoute((String)Nav_Rec_Delete_CB.SelectedItem);
            }
        }
        private void Nav_Rec_Update_Node_Button_Click(object sender, EventArgs e)
        {
            //If we're not recording, update the selected node with the
            //current data on the form.
            if (Nav_Rec_State == NAV_REC_STATE.STOPPED)
            {
                Nav_Rec_saveNode(Nav_Rec_Route_LB.SelectedIndex);
            }
        }
        #endregion Buttons
        #region ComboBoxes
        private void Nav_Rec_Key_Stroke_CB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                Nav_Rec_addKeystrokeNode();
            }
        }
        private void Nav_Rec_Delete_CB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Nav_Rec_Delete_CB.SelectedIndex >= 0)
            {
                Nav_Rec_loadRoute();
            }
        }
        #endregion ComboBoxes
        #region List Boxes
        private void Nav_Rec_Route_LB_Click(object sender, EventArgs e)
        {
            //On click we want to see if we're currently recording.
            //If not, we'll load the form with the information from
            //the selected node.
            if (Nav_Rec_State == NAV_REC_STATE.STOPPED)
            {
                Nav_Rec_loadFormWithNodeData(Nav_Rec_Route_LB.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Please stop recording to enable route editing.");
            }
        }
        private void Nav_Rec_Route_LB_DoubleClick(object sender, EventArgs e)
        {
            //On double click we want to perform the action of the node we
            //clicked on.  If it's a command we should probably prompt the
            //user if they actually want to execute the command. Or at least
            //prompt if it's a teleportation command.
            //We should also update the status box.

            if (Navigation.ProcessingStatus != Navigation.PROCESSING_STATUS.STOPPED)
            {
                MessageBox.Show("You cannot perform that action while a navigation process is running.", "Please stop first", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            if (Nav_Rec_Route_LB.SelectedIndex >= 0)
            {
                Routes.UserRoutesRow node = Nav_Rec_CurrentRoute.RouteNodes[Nav_Rec_Route_LB.SelectedIndex];
                if (node.NodeType == (ushort)NAV_NODE_TYPE.COMMAND)
                {
                    DialogResult promptResult = MessageBox.Show("Do you really want to perform this command?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (promptResult == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                }
                else if ((node.NodeType == (ushort)NAV_NODE_TYPE.POS_END)
                    || (node.NodeType == (ushort)NAV_NODE_TYPE.POS_NODE)
                    || (node.NodeType == (ushort)NAV_NODE_TYPE.POS_START)
                    || (node.NodeType == (ushort)NAV_NODE_TYPE.POS_ZONE))
                {
                    UInt16 zoneId = MemReads.Self.get_zone_id();
                    if (zoneId != node.NodeZoneID)
                    {
                        MessageBox.Show("Node was not in this zone.", "Not in Zone.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    float distToNode = (float)Nav_getDistance(node.NodePosX, MemReads.Self.Position.get_x(),
                                                              node.NodePosY, MemReads.Self.Position.get_y());
                    if (distToNode > Statics.Constants.Navigation.MaxDistForRouteStart)
                    {
                        MessageBox.Show("Node was not within range.", "Not in Range.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                Navigation.ProcessNode(node);
            }
        }
        private void Nav_Rec_Route_LB_MouseDown(object sender, MouseEventArgs e)
        {
            int idx = Nav_Rec_Route_LB.IndexFromPoint(e.X, e.Y);
            if (idx >= 0)
            {
                //An item was selected, start the drag/drop.
                if (e.Clicks == 1)
                {
                    Nav_Rec_Route_LB.DoDragDrop(Nav_Rec_Route_LB.SelectedItem, DragDropEffects.Move | DragDropEffects.Scroll);
                }
            }
            else
            {
                //No selection made, do nothing.
            }
        }
        private void Nav_Rec_Route_LB_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move | DragDropEffects.Scroll;
        }
        private void Nav_Rec_Route_LB_DragDrop(object sender, DragEventArgs e)
        {
            Point pt = Nav_Rec_Route_LB.PointToClient(new Point(e.X, e.Y));
            int idx = Nav_Rec_Route_LB.IndexFromPoint(pt);
            if (idx < 0)
            {
                idx = Nav_Rec_Route_LB.Items.Count - 1;
            }
            Routes.UserRoutesRow data = (Routes.UserRoutesRow)e.Data.GetData(typeof(Routes.UserRoutesRow));
            Nav_Rec_CurrentRoute.RouteNodes.Remove(data);
            Nav_Rec_CurrentRoute.RouteNodes.Insert(idx, data);
            int nbNodes = Nav_Rec_CurrentRoute.RouteNodes.Count;
            for (int ii = 0; ii < nbNodes; ii++)
            {
                Nav_Rec_CurrentRoute.RouteNodes[ii].NodeID = (uint)ii;
            }
            Nav_Rec_refreshRouteLB();
        }
        #endregion List Boxes
        #endregion Event Handlers
        #endregion User Route Recording
    }
}