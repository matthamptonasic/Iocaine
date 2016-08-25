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
            TASK_DELEGATOR,
            DELIVERY,
            MOOGLE_PORTER,
            MOOGLE_GREEN_THUMB
        }
        #endregion Enums

        #region Private Members
        private static bool m_initDone = false;
        private static NPC_Dataset db = new NPC_Dataset();
        private static AutoCompleteStringCollection npcNameCollection = new AutoCompleteStringCollection();
        #endregion Private Members

        #region Public Properties
        public static bool Init_Done
        {
            get
            {
                return m_initDone;
            }
        }
        public static AutoCompleteStringCollection NpcNameCollection
        {
            get
            {
                return npcNameCollection;
            }
        }
        #endregion Public Properties

        #region Inits
        public static bool Init_Iocaine()
        {
            if (m_initDone == false)
            {
                loadDataset();
                loadNpcNameCollection();
                m_initDone = true;
            }
            return true;
        }
        #endregion Inits

        #region Public Methods
        public static uint GetCount()
        {
            Init_Iocaine();
            return (uint)db.NPCs.Rows.Count;
        }
        public static uint GetCount(NPC_TYPE iType)
        {
            Init_Iocaine();
            string filter = "Type=" + iType;
            NPC_Dataset.NPCsRow[] rows = (NPC_Dataset.NPCsRow[])db.NPCs.Select(filter);
            return (uint)rows.Length;
        }
        public static bool NpcExists(string iName)
        {
            Init_Iocaine();
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
        public static NPC_TYPE GetNPCType(string iName)
        {
            Init_Iocaine();
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
        public static ushort GetZone(string iName)
        {
            Init_Iocaine();
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
        public static bool GetGuildInfo(FFXIEnums.GUILDS iGuild, FFXIEnums.ZONES iZone, ref byte oHourOpen, ref byte oHourClose, ref byte oDayOff)
        {
            Init_Iocaine();
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
        public static bool GetGuildInfo(string iName, ref byte oHourOpen, ref byte oHourClose, ref byte oDayOff)
        {
            Init_Iocaine();
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

        #region Private Methods
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
        #endregion Private Methods
    }
}
