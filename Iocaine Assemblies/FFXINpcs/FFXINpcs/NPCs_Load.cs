using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Iocaine2;

namespace Iocaine2.Data.Client
{
    public static partial class NPCs
    {
        #region Top Load Method
        private static bool loadDataset()
        {
            if (db == null)
            {
                db = new NPC_Dataset();
            }
            loadGuildMerchants();
            loadRoENpcs();
            return true;
        }
        #endregion Top Load Method

        #region Individual Load Methods
        #region Sell All Merchants
        #endregion Sell All Merchants

        #region Guild
        private static void loadGuildMerchants()
        {
            loadGuildFishermans();
            loadGuildCarpenters();
            loadGuildSmithing();
            loadGuildGoldsmithing();
            loadGuildWeavers();
            loadGuildTanners();
            loadGuildBoneworkers();
            loadGuildAlchemists();
            loadGuildCooking();
            loadGuildTenshodo();
            loadGuildHours();
        }
        #region Guild Hours
        private static void loadGuildHours()
        {
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.FI, FFXIEnums.ZONES.PORT_WINDURST, 3, 18, FFXIEnums.DAY.LIGHTNINGDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.FI, FFXIEnums.ZONES.AHT_URHGAN_WHITEGATE, 1, 18, FFXIEnums.DAY.LIGHTSDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.FI, FFXIEnums.ZONES.BIBIKI_BAY, 1, 18, FFXIEnums.DAY.LIGHTNINGDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.FI, FFXIEnums.ZONES.SELBINA, 3, 18, FFXIEnums.DAY.LIGHTNINGDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.FI, FFXIEnums.ZONES.SHIP_BOUND_FOR_SELBINA, 1, 23, FFXIEnums.DAY.LIGHTNINGDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.FI, FFXIEnums.ZONES.SHIP_BOUND_FOR_MHAURA, 1, 23, FFXIEnums.DAY.LIGHTNINGDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.FI, FFXIEnums.ZONES.OPEN_SEA_ROUTE_TO_AL_ZAHBI, 1, 23, FFXIEnums.DAY.LIGHTNINGDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.FI, FFXIEnums.ZONES.OPEN_SEA_ROUTE_TO_MHAURA, 1, 23, FFXIEnums.DAY.LIGHTNINGDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.FI, FFXIEnums.ZONES.SILVER_SEA_ROUTE_TO_AL_ZAHBI, 1, 23, FFXIEnums.DAY.LIGHTNINGDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.FI, FFXIEnums.ZONES.SILVER_SEA_ROUTE_TO_NASHMAU, 1, 23, FFXIEnums.DAY.LIGHTNINGDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.WW, FFXIEnums.ZONES.NORTHERN_SAN_DORIA, 6, 21, FFXIEnums.DAY.FIRESDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.WW, FFXIEnums.ZONES.CARPENTERS_LANDING, 5, 22, FFXIEnums.DAY.FIRESDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.WW, FFXIEnums.ZONES.AL_ZAHBI, 6, 21, FFXIEnums.DAY.FIRESDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.SM, FFXIEnums.ZONES.NORTHERN_SAN_DORIA, 8, 23, FFXIEnums.DAY.WATERSDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.SM, FFXIEnums.ZONES.MHAURA, 8, 23, FFXIEnums.DAY.WATERSDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.SM, FFXIEnums.ZONES.METALWORKS, 8, 23, FFXIEnums.DAY.WATERSDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.SM, FFXIEnums.ZONES.AL_ZAHBI, 8, 23, FFXIEnums.DAY.WATERSDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.GS, FFXIEnums.ZONES.BASTOK_MARKETS, 8, 23, FFXIEnums.DAY.ICEDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.GS, FFXIEnums.ZONES.MHAURA, 8, 23, FFXIEnums.DAY.ICEDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.GS, FFXIEnums.ZONES.AL_ZAHBI, 8, 23, FFXIEnums.DAY.ICEDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.CC, FFXIEnums.ZONES.AL_ZAHBI, 6, 21, FFXIEnums.DAY.FIRESDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.CC, FFXIEnums.ZONES.SELBINA, 6, 21, FFXIEnums.DAY.FIRESDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.CC, FFXIEnums.ZONES.WINDURST_WOODS, 6, 21, FFXIEnums.DAY.FIRESDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.LC, FFXIEnums.ZONES.SOUTHERN_SAN_DORIA, 3, 18, FFXIEnums.DAY.ICEDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.BC, FFXIEnums.ZONES.WINDURST_WOODS, 8, 23, FFXIEnums.DAY.WINDSDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.AL, FFXIEnums.ZONES.BASTOK_MINES, 8, 23, FFXIEnums.DAY.LIGHTSDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.AL, FFXIEnums.ZONES.AHT_URHGAN_WHITEGATE, 8, 23, FFXIEnums.DAY.LIGHTNINGDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.CK, FFXIEnums.ZONES.WINDURST_WATERS, 5, 20, FFXIEnums.DAY.DARKSDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.TE, FFXIEnums.ZONES.PORT_BASTOK, 1, 23, FFXIEnums.DAY.ICEDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.TE, FFXIEnums.ZONES.LOWER_JEUNO, 1, 23, FFXIEnums.DAY.EARTHSDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.TE, FFXIEnums.ZONES.NORG, 9, 23, FFXIEnums.DAY.DARKSDAY });
            db.GuildHours.Rows.Add(new object[] { FFXIEnums.GUILDS.TE, FFXIEnums.ZONES.NASHMAU, 1, 23, FFXIEnums.DAY.DARKSDAY });
        }
        #endregion Guild Hours
        #region Guild Merchants
        #region Fishing
        private static void loadGuildFishermans()
        {
            NPC_Dataset.NPCsRow row = null;

            row = db.NPCs.NewNPCsRow();
            row.Name = "Babubu";
            row.Type = (byte)NPC_TYPE.GUILD_MERCH;
            row.Zone = (ushort)FFXIEnums.ZONES.PORT_WINDURST;
            row.Data = (uint)FFXIEnums.GUILDS.FI;
            db.NPCs.Rows.Add(row);

            row = db.NPCs.NewNPCsRow();
            row.Name = "Graegham";
            row.Type = (byte)NPC_TYPE.GUILD_MERCH;
            row.Zone = (ushort)FFXIEnums.ZONES.SELBINA;
            row.Data = (uint)FFXIEnums.GUILDS.FI;
            db.NPCs.Rows.Add(row);

            row = db.NPCs.NewNPCsRow();
            row.Name = "Mendoline";
            row.Type = (byte)NPC_TYPE.GUILD_MERCH;
            row.Zone = (ushort)FFXIEnums.ZONES.SELBINA;
            row.Data = (uint)FFXIEnums.GUILDS.FI;
            db.NPCs.Rows.Add(row);

            row = db.NPCs.NewNPCsRow();
            row.Name = "Mep Nhapopoluko";
            row.Type = (byte)NPC_TYPE.GUILD_MERCH;
            row.Zone = (ushort)FFXIEnums.ZONES.BIBIKI_BAY;
            row.Data = (uint)FFXIEnums.GUILDS.FI;
            db.NPCs.Rows.Add(row);

            row = db.NPCs.NewNPCsRow();
            row.Name = "Wahnid";
            row.Type = (byte)NPC_TYPE.GUILD_MERCH;
            row.Zone = (ushort)FFXIEnums.ZONES.AHT_URHGAN_WHITEGATE;
            row.Data = (uint)FFXIEnums.GUILDS.FI;
            db.NPCs.Rows.Add(row);

            row = db.NPCs.NewNPCsRow();
            row.Name = "Rajmonda";
            row.Type = (byte)NPC_TYPE.GUILD_MERCH;
            row.Zone = (ushort)FFXIEnums.ZONES.SHIP_BOUND_FOR_SELBINA;
            row.Data = (uint)FFXIEnums.GUILDS.FI;
            db.NPCs.Rows.Add(row);

            row = db.NPCs.NewNPCsRow();
            row.Name = "Lokhong";
            row.Type = (byte)NPC_TYPE.GUILD_MERCH;
            row.Zone = (ushort)FFXIEnums.ZONES.SHIP_BOUND_FOR_MHAURA;
            row.Data = (uint)FFXIEnums.GUILDS.FI;
            db.NPCs.Rows.Add(row);

            row = db.NPCs.NewNPCsRow();
            row.Name = "Cehn Teyohngo";
            row.Type = (byte)NPC_TYPE.GUILD_MERCH;
            row.Zone = (ushort)FFXIEnums.ZONES.OPEN_SEA_ROUTE_TO_AL_ZAHBI;
            row.Data = (uint)FFXIEnums.GUILDS.FI;
            db.NPCs.Rows.Add(row);

            row = db.NPCs.NewNPCsRow();
            row.Name = "Pashi Maccaleh";
            row.Type = (byte)NPC_TYPE.GUILD_MERCH;
            row.Zone = (ushort)FFXIEnums.ZONES.OPEN_SEA_ROUTE_TO_MHAURA;
            row.Data = (uint)FFXIEnums.GUILDS.FI;
            db.NPCs.Rows.Add(row);

            row = db.NPCs.NewNPCsRow();
            row.Name = "Jidwahn";
            row.Type = (byte)NPC_TYPE.GUILD_MERCH;
            row.Zone = (ushort)FFXIEnums.ZONES.SILVER_SEA_ROUTE_TO_NASHMAU;
            row.Data = (uint)FFXIEnums.GUILDS.FI;
            db.NPCs.Rows.Add(row);

            row = db.NPCs.NewNPCsRow();
            row.Name = "Yahliq";
            row.Type = (byte)NPC_TYPE.GUILD_MERCH;
            row.Zone = (ushort)FFXIEnums.ZONES.SILVER_SEA_ROUTE_TO_AL_ZAHBI;
            row.Data = (uint)FFXIEnums.GUILDS.FI;
            db.NPCs.Rows.Add(row);

            db.NPCs.AcceptChanges();
        }
        #endregion Fishing
        #region Carpenter's
        private static void loadGuildCarpenters()
        {
            db.NPCs.Rows.Add(new object[] { "Cauzeriste", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.NORTHERN_SAN_DORIA, FFXIEnums.GUILDS.WW });
            db.NPCs.Rows.Add(new object[] { "Chaupire", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.NORTHERN_SAN_DORIA, FFXIEnums.GUILDS.WW });
            db.NPCs.Rows.Add(new object[] { "Beugungel", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.CARPENTERS_LANDING, FFXIEnums.GUILDS.WW });
            db.NPCs.Rows.Add(new object[] { "Dehbi Moshal", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.AL_ZAHBI, FFXIEnums.GUILDS.WW });
        }
        #endregion Carpenter's
        #region Smithing
        private static void loadGuildSmithing()
        {
            db.NPCs.Rows.Add(new object[] { "Doggomehr", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.NORTHERN_SAN_DORIA, FFXIEnums.GUILDS.SM });
            db.NPCs.Rows.Add(new object[] { "Lucretia", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.NORTHERN_SAN_DORIA, FFXIEnums.GUILDS.SM });
            db.NPCs.Rows.Add(new object[] { "Mololo", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.MHAURA, FFXIEnums.GUILDS.SM });
            db.NPCs.Rows.Add(new object[] { "Kamilah", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.MHAURA, FFXIEnums.GUILDS.SM });
            db.NPCs.Rows.Add(new object[] { "Amulya", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.METALWORKS, FFXIEnums.GUILDS.SM });
            db.NPCs.Rows.Add(new object[] { "Vicious Eye", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.METALWORKS, FFXIEnums.GUILDS.SM });
            db.NPCs.Rows.Add(new object[] { "Ndego", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.AL_ZAHBI, FFXIEnums.GUILDS.SM });
        }
        #endregion Smithing
        #region Goldsmithing
        private static void loadGuildGoldsmithing()
        {
            db.NPCs.Rows.Add(new object[] { "Teerth", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.BASTOK_MARKETS, FFXIEnums.GUILDS.GS });
            db.NPCs.Rows.Add(new object[] { "Visala", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.BASTOK_MARKETS, FFXIEnums.GUILDS.GS });
            db.NPCs.Rows.Add(new object[] { "Yabby Tanmikey", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.MHAURA, FFXIEnums.GUILDS.GS });
            db.NPCs.Rows.Add(new object[] { "Celestina", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.MHAURA, FFXIEnums.GUILDS.GS });
            db.NPCs.Rows.Add(new object[] { "Bornahn", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.AL_ZAHBI, FFXIEnums.GUILDS.GS });
        }
        #endregion Goldsmithing
        #region Weaver's
        private static void loadGuildWeavers()
        {
            db.NPCs.Rows.Add(new object[] { "Taten-Bilten", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.AL_ZAHBI, FFXIEnums.GUILDS.CC });
            db.NPCs.Rows.Add(new object[] { "Tilala", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.SELBINA, FFXIEnums.GUILDS.CC });
            db.NPCs.Rows.Add(new object[] { "Gibol", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.SELBINA, FFXIEnums.GUILDS.CC });
            db.NPCs.Rows.Add(new object[] { "Kuzah Hpirohpon", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.WINDURST_WOODS, FFXIEnums.GUILDS.CC });
            db.NPCs.Rows.Add(new object[] { "Meriri", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.WINDURST_WOODS, FFXIEnums.GUILDS.CC });
        }
        #endregion Weaver's
        #region Tanner's
        private static void loadGuildTanners()
        {
            db.NPCs.Rows.Add(new object[] { "Cletae", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.SOUTHERN_SAN_DORIA, FFXIEnums.GUILDS.LC });
            db.NPCs.Rows.Add(new object[] { "Kueh Igunahmori", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.SOUTHERN_SAN_DORIA, FFXIEnums.GUILDS.LC });
        }
        #endregion Tanner's
        #region Boneworker's
        private static void loadGuildBoneworkers()
        {
            db.NPCs.Rows.Add(new object[] { "Shih Tayuun", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.WINDURST_WOODS, FFXIEnums.GUILDS.BC });
            db.NPCs.Rows.Add(new object[] { "Retto-Marutto", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.WINDURST_WOODS, FFXIEnums.GUILDS.BC });
        }
        #endregion Boneworker's
        #region Alchemist's
        private static void loadGuildAlchemists()
        {
            db.NPCs.Rows.Add(new object[] { "Odoba", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.BASTOK_MINES, FFXIEnums.GUILDS.AL });
            db.NPCs.Rows.Add(new object[] { "Maymunah", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.BASTOK_MINES, FFXIEnums.GUILDS.AL });
            db.NPCs.Rows.Add(new object[] { "Wahraga", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.AHT_URHGAN_WHITEGATE, FFXIEnums.GUILDS.AL });
            db.NPCs.Rows.Add(new object[] { "Gathweeda", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.AHT_URHGAN_WHITEGATE, FFXIEnums.GUILDS.AL });
        }
        #endregion Alchemist's
        #region Cooking
        private static void loadGuildCooking()
        {
            db.NPCs.Rows.Add(new object[] { "Chomo Jinjahl", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.WINDURST_WATERS, FFXIEnums.GUILDS.CK });
            db.NPCs.Rows.Add(new object[] { "Kopopo", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.WINDURST_WATERS, FFXIEnums.GUILDS.CK });
        }
        #endregion Cooking
        #region Tenshodo
        private static void loadGuildTenshodo()
        {
            db.NPCs.Rows.Add(new object[] { "Jabbar", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.PORT_BASTOK, FFXIEnums.GUILDS.TE });
            db.NPCs.Rows.Add(new object[] { "Silver Owl", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.PORT_BASTOK, FFXIEnums.GUILDS.TE });
            db.NPCs.Rows.Add(new object[] { "Akamafula", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.LOWER_JEUNO, FFXIEnums.GUILDS.TE });
            db.NPCs.Rows.Add(new object[] { "Amalasanda", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.LOWER_JEUNO, FFXIEnums.GUILDS.TE });
            db.NPCs.Rows.Add(new object[] { "Achika", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.NORG, FFXIEnums.GUILDS.TE });
            db.NPCs.Rows.Add(new object[] { "Chiyo", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.NORG, FFXIEnums.GUILDS.TE });
            db.NPCs.Rows.Add(new object[] { "Jirokichi", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.NORG, FFXIEnums.GUILDS.TE });
            db.NPCs.Rows.Add(new object[] { "Vuliaie", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.NORG, FFXIEnums.GUILDS.TE });
            db.NPCs.Rows.Add(new object[] { "Tsutsuroon", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.NASHMAU, FFXIEnums.GUILDS.TE });
        }
        #endregion Tenshodo
        #endregion Guild Merchants
        #endregion Guild

        #region Regional Merchants
        #endregion Regional Merchants

        #region Records of Eminence (RoE)
        private static void loadRoENpcs()
        {
            NPC_Dataset.NPCsRow row = null;

            row = db.NPCs.NewNPCsRow();
            row.Name = "Rolandienne";
            row.Type = (byte)NPC_TYPE.ROE;
            row.Zone = (ushort)FFXIEnums.ZONES.SOUTHERN_SAN_DORIA;
            row.Data = 0;
            db.NPCs.Rows.Add(row);

            row = db.NPCs.NewNPCsRow();
            row.Name = "Isakoth";
            row.Type = (byte)NPC_TYPE.ROE;
            row.Zone = (ushort)FFXIEnums.ZONES.BASTOK_MARKETS;
            row.Data = 0;
            db.NPCs.Rows.Add(row);

            row = db.NPCs.NewNPCsRow();
            row.Name = "Fhelm Jobeizat";
            row.Type = (byte)NPC_TYPE.ROE;
            row.Zone = (ushort)FFXIEnums.ZONES.WINDURST_WOODS;
            row.Data = 0;
            db.NPCs.Rows.Add(row);

            row = db.NPCs.NewNPCsRow();
            row.Name = "Eternal Flame";
            row.Type = (byte)NPC_TYPE.ROE;
            row.Zone = (ushort)FFXIEnums.ZONES.WESTERN_ADOULIN;
            row.Data = 0;
            db.NPCs.Rows.Add(row);

            db.NPCs.AcceptChanges();
        }
        #endregion Records of Eminence (RoE)

        #region Delivery NPC's
        #endregion Delivery NPC's
        #endregion Individual Load Methods
    }
}
