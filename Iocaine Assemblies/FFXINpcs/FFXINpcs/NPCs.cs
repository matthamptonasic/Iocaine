using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Iocaine2.Data.Client
{
    public static partial class NPCs
    {
        #region Enums
        public enum NPC_TYPE : byte
        {
            UNKNOWN = 0,
            SELL_ALL = 1,
            GUILD_MERCH = 2,
            REGIONAL = 3,
            OUTPOST = 4,
            ROE = 5,
            CRUOR = 6,
            SIGIL_WIN = 7,
            SIGIL_BAS = 8,
            SIGIL_SAN = 9,
            SANCTION = 10,
            SIGNET_WIN = 11,
            SIGNET_BAS = 12,
            SIGNET_SAN = 13,
            SIGNET_JEU = 14,
            IONIS = 15
        }
        #endregion Enums
        #region Members
        #region Private Members
        private static bool init_done = false;
        private static NPC_Dataset db = new NPC_Dataset();
        private static AutoCompleteStringCollection npcNameCollection = new AutoCompleteStringCollection();
        #endregion Private Members
        #region Public Members
        public static bool Init_Done
        {
            get
            {
                return init_done;
            }
        }
        public static AutoCompleteStringCollection NpcNameCollection
        {
            get
            {
                return npcNameCollection;
            }
        }
        #endregion Public Members
        #endregion Members
        #region Methods
        #region Inits
        public static bool Init()
        {
            return InitInt();
        }
        private static bool InitInt()
        {
            if (init_done == false)
            {
                loadDataset();
                loadNpcNameCollection();
            }
            init_done = true;
            return true;
        }
        #endregion Inits
        #region Utility Functions
        private static void loadNpcNameCollection()
        {
            npcNameCollection.Clear();
            //String filter = "Type=" + ((byte)NPC_TYPE.GUILD_MERCH).ToString();
            String filter = "";
            String orderBy = "Name ASC";
            NPC_Dataset.NPCsRow[] rows = (NPC_Dataset.NPCsRow[])db.NPCs.Select(filter, orderBy);
            foreach (NPC_Dataset.NPCsRow row in rows)
            {
                npcNameCollection.Add(row.Name);
            }
        }
        #endregion Utility Functions
        #region Public Methods
        public static UInt32 Count()
        {
            return CountInt();
        }
        private static UInt32 CountInt()
        {
            if (db == null)
            {
                loadDataset();
            }
            return (UInt32)db.NPCs.Rows.Count;
        }
        public static UInt32 Count(NPC_TYPE iType)
        {
            return CountInt(iType);
        }
        private static UInt32 CountInt(NPC_TYPE iType)
        {
            if (db == null)
            {
                loadDataset();
            }
            String filter = "Type=" + iType;
            NPC_Dataset.NPCsRow[] rows = (NPC_Dataset.NPCsRow[])db.NPCs.Select(filter);
            return (UInt32)rows.Length;
        }
        public static bool NpcExists(String iName)
        {
            return NpcExistsInt(iName);
        }
        private static bool NpcExistsInt(String iName)
        {
            if (db == null)
            {
                loadDataset();
            }
            String filter = "Name='" + iName + "'";
            NPC_Dataset.NPCsRow[] rows = (NPC_Dataset.NPCsRow[])db.NPCs.Select(filter);
            if (rows.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static NPC_TYPE Type(String iName)
        {
            return TypeInt(iName);
        }
        private static NPC_TYPE TypeInt(String iName)
        {
            if (db == null)
            {
                loadDataset();
            }
            String filter = "Name='" + iName + "'";
            NPC_Dataset.NPCsRow[] rows = (NPC_Dataset.NPCsRow[])db.NPCs.Select(filter);
            if (rows.Length == 0)
            {
                return NPC_TYPE.UNKNOWN;
            }
            else
            {
                return (NPC_TYPE)rows[0].Type;
            }
        }
        public static UInt16 Zone(String iName)
        {
            return ZoneInt(iName);
        }
        private static UInt16 ZoneInt(String iName)
        {
            if (db == null)
            {
                loadDataset();
            }
            String filter = "Name='" + iName + "'";
            NPC_Dataset.NPCsRow[] rows = (NPC_Dataset.NPCsRow[])db.NPCs.Select(filter);
            if (rows.Length == 0)
            {
                return 0;
            }
            else
            {
                return rows[0].Zone;
            }
        }
        public static bool GuildInfo(FFXIEnums.GUILDS iGuild, FFXIEnums.ZONES iZone, ref Byte oHourOpen, ref Byte oHourClose, ref Byte oDayOff)
        {
            return GuildInfoInt(iGuild, iZone, ref oHourOpen, ref oHourClose, ref oDayOff);
        }
        private static bool GuildInfoInt(FFXIEnums.GUILDS iGuild, FFXIEnums.ZONES iZone, ref Byte oHourOpen, ref Byte oHourClose, ref Byte oDayOff)
        {
            if (db == null)
            {
                loadDataset();
            }
            String filter = "Guild=" + ((Byte)iGuild).ToString() + " AND Zone=" + ((UInt16)iZone).ToString();
            NPC_Dataset.GuildHoursRow[] rows = (NPC_Dataset.GuildHoursRow[])db.GuildHours.Select(filter);
            if (rows.Length == 0)
            {
                oHourOpen = 0xff;
                oHourClose = 0xff;
                oDayOff = 0xff;
                return false;
            }
            else
            {
                oHourOpen = rows[0].HourOpen;
                oHourClose = rows[0].HourClose;
                oDayOff = rows[0].DayOff;
                return true;
            }
        }
        public static bool GuildInfo(String iName, ref Byte oHourOpen, ref Byte oHourClose, ref Byte oDayOff)
        {
            return GuildInfoInt(iName, ref oHourOpen, ref oHourClose, ref oDayOff);
        }
        private static bool GuildInfoInt(String iName, ref Byte oHourOpen, ref Byte oHourClose, ref Byte oDayOff)
        {
            if (db == null)
            {
                loadDataset();
            }
            String filter = "Name='" + iName + "'";
            NPC_Dataset.NPCsRow[] rows = (NPC_Dataset.NPCsRow[])db.NPCs.Select(filter);
            if (rows.Length == 0)
            {
                oHourOpen = 0xff;
                oHourClose = 0xff;
                oDayOff = 0xff;
                return false;
            }
            else
            {
                UInt16 zone = rows[0].Zone;
                UInt32 guild = rows[0].Data;
                filter = "Zone=" + zone + " AND Guild=" + guild;
                NPC_Dataset.GuildHoursRow[] gRows = (NPC_Dataset.GuildHoursRow[])db.GuildHours.Select(filter);
                if (gRows.Length == 0)
                {
                    oHourOpen = 0xff;
                    oHourClose = 0xff;
                    oDayOff = 0xff;
                    return false;
                }
                else
                {
                    oHourOpen = gRows[0].HourOpen;
                    oHourClose = gRows[0].HourClose;
                    oDayOff = gRows[0].DayOff;
                    return true;
                }
            }
        }
        #endregion Public Methods
        #endregion Methods
    }
}
