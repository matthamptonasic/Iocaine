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
            UNKNOWN,
            SELL_ALL,
            GUILD_MERCH,
            REGIONAL,
            OUTPOST,
            ROE,
            CRUOR,
            SIGIL_WIN,
            SIGIL_BAS,
            SIGIL_SAN,
            SANCTION,
            SIGNET_WIN,
            SIGNET_BAS,
            SIGNET_SAN,
            SIGNET_JEU,
            IONIS,
            TASK_DELEGATOR
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
            string filter = "";
            string orderBy = "Name ASC";
            NPC_Dataset.NPCsRow[] rows = (NPC_Dataset.NPCsRow[])db.NPCs.Select(filter, orderBy);
            foreach (NPC_Dataset.NPCsRow row in rows)
            {
                npcNameCollection.Add(row.Name);
            }
        }
        #endregion Utility Functions
        #region Public Methods
        public static uint Count()
        {
            return CountInt();
        }
        private static uint CountInt()
        {
            if (db == null)
            {
                loadDataset();
            }
            return (uint)db.NPCs.Rows.Count;
        }
        public static uint Count(NPC_TYPE iType)
        {
            return CountInt(iType);
        }
        private static uint CountInt(NPC_TYPE iType)
        {
            if (db == null)
            {
                loadDataset();
            }
            string filter = "Type=" + iType;
            NPC_Dataset.NPCsRow[] rows = (NPC_Dataset.NPCsRow[])db.NPCs.Select(filter);
            return (uint)rows.Length;
        }
        public static bool NpcExists(string iName)
        {
            return NpcExistsInt(iName);
        }
        private static bool NpcExistsInt(string iName)
        {
            if (db == null)
            {
                loadDataset();
            }
            string filter = "Name='" + iName + "'";
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
        public static NPC_TYPE Type(string iName)
        {
            return TypeInt(iName);
        }
        private static NPC_TYPE TypeInt(string iName)
        {
            if (db == null)
            {
                loadDataset();
            }
            string filter = "Name='" + iName + "'";
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
        public static ushort Zone(string iName)
        {
            return ZoneInt(iName);
        }
        private static ushort ZoneInt(string iName)
        {
            if (db == null)
            {
                loadDataset();
            }
            string filter = "Name='" + iName + "'";
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
        public static bool GuildInfo(FFXIEnums.GUILDS iGuild, FFXIEnums.ZONES iZone, ref byte oHourOpen, ref byte oHourClose, ref byte oDayOff)
        {
            return GuildInfoInt(iGuild, iZone, ref oHourOpen, ref oHourClose, ref oDayOff);
        }
        private static bool GuildInfoInt(FFXIEnums.GUILDS iGuild, FFXIEnums.ZONES iZone, ref byte oHourOpen, ref byte oHourClose, ref byte oDayOff)
        {
            if (db == null)
            {
                loadDataset();
            }
            string filter = "Guild=" + ((byte)iGuild).ToString() + " AND Zone=" + ((ushort)iZone).ToString();
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
        public static bool GuildInfo(string iName, ref byte oHourOpen, ref byte oHourClose, ref byte oDayOff)
        {
            return GuildInfoInt(iName, ref oHourOpen, ref oHourClose, ref oDayOff);
        }
        private static bool GuildInfoInt(string iName, ref byte oHourOpen, ref byte oHourClose, ref byte oDayOff)
        {
            if (db == null)
            {
                loadDataset();
            }
            string filter = "Name='" + iName + "'";
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
                ushort zone = rows[0].Zone;
                uint guild = rows[0].Data;
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
