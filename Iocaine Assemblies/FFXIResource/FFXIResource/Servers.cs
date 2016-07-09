using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Servers
    {
        #region Structures
        public struct SERVER_INFO
        {
            public String ServerName;
            public Byte ServerID;
        }
        #endregion Structures
        #region Member Variables
        private static bool initDone = false;
        #endregion Member Variables
        #region Init
        internal static void init()
        {
            if (!initDone)
            {
                loadData();
                initDone = true;
            }
        }
        #endregion Init
        #region Get Functions
        public static SERVER_INFO GetServerInfo(Byte iServerId)
        {
            FfxiResource.init();
            String filter = "ServerID = " + iServerId;
            MainDatabase.ServersRow[] serverRows = (MainDatabase.ServersRow[])FfxiResource.mainDb.Servers.Select(filter);
            SERVER_INFO info = new SERVER_INFO();
            if (serverRows.Length == 0)
            {
                getNullServerInfo(ref info);
            }
            else
            {
                MainDatabase.ServersRow serverRow = serverRows[0];
                info.ServerName = serverRow.ServerName;
                info.ServerID = serverRow.ServerID;
            }
            return info;
        }
        public static SERVER_INFO GetServerInfo(String iServerName)
        {
            FfxiResource.init();
            String filter = "ServerName = '" + iServerName + "'";
            MainDatabase.ServersRow[] serverRows = (MainDatabase.ServersRow[])FfxiResource.mainDb.Servers.Select(filter);
            SERVER_INFO info = new SERVER_INFO();
            if (serverRows.Length == 0)
            {
                getNullServerInfo(ref info);
            }
            else
            {
                MainDatabase.ServersRow serverRow = serverRows[0];
                info.ServerName = serverRow.ServerName;
                info.ServerID = serverRow.ServerID;
            }
            return info;
        }
        public static String GetServerName(Byte iServerID)
        {
            SERVER_INFO info = GetServerInfo(iServerID);
            return info.ServerName;
        }
        public static Byte GetServerID(String iServerName)
        {
            SERVER_INFO info = GetServerInfo(iServerName);
            return info.ServerID;
        }
        public static List<SERVER_INFO> GetAllServerInfo()
        {
            List<SERVER_INFO> infoList = new List<SERVER_INFO>();
            FfxiResource.init();
            String filter = "";
            MainDatabase.ServersRow[] serverRows = (MainDatabase.ServersRow[])FfxiResource.mainDb.Servers.Select(filter);
            if (serverRows.Length == 0)
            {
                infoList.Add(getNullServerInfo());
            }
            else
            {
                for (int ii = 0; ii < serverRows.Length; ii++)
                {
                    MainDatabase.ServersRow serverRow = serverRows[ii];
                    SERVER_INFO info = new SERVER_INFO();
                    info.ServerName = serverRow.ServerName;
                    info.ServerID = serverRow.ServerID;
                    infoList.Add(info);
                }
            }
            return infoList;
        }
        #region Nulls
        private static SERVER_INFO getNullServerInfo()
        {
            SERVER_INFO info = new SERVER_INFO();
            info.ServerName = "Unknown";
            info.ServerID = 0;
            return info;
        }
        private static void getNullServerInfo(ref SERVER_INFO oInfo)
        {
            oInfo.ServerName = "Unknown";
            oInfo.ServerID = 0;
        }
        #endregion Nulls
        #endregion Get Functions
    }
}
