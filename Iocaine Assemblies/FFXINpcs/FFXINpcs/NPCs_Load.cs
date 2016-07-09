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
        private static bool loadDataset()
        {
            if (db == null)
            {
                db = new NPC_Dataset();
            }
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
            return true;
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
            db.NPCs.Rows.Add(new object[] { "Babubu", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.PORT_WINDURST, FFXIEnums.GUILDS.FI });
            db.NPCs.Rows.Add(new object[] { "Graegham", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.SELBINA, FFXIEnums.GUILDS.FI });
            db.NPCs.Rows.Add(new object[] { "Mendoline", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.SELBINA, FFXIEnums.GUILDS.FI });
            db.NPCs.Rows.Add(new object[] { "Mep Nhapopoluko", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.BIBIKI_BAY, FFXIEnums.GUILDS.FI });
            db.NPCs.Rows.Add(new object[] { "Wahnid", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.AHT_URHGAN_WHITEGATE, FFXIEnums.GUILDS.FI });
            db.NPCs.Rows.Add(new object[] { "Rajmonda", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.SHIP_BOUND_FOR_SELBINA, FFXIEnums.GUILDS.FI });
            db.NPCs.Rows.Add(new object[] { "Lokhong", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.SHIP_BOUND_FOR_MHAURA, FFXIEnums.GUILDS.FI });
            db.NPCs.Rows.Add(new object[] { "Cehn Teyohngo", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.OPEN_SEA_ROUTE_TO_AL_ZAHBI, FFXIEnums.GUILDS.FI });
            db.NPCs.Rows.Add(new object[] { "Pashi Maccaleh", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.OPEN_SEA_ROUTE_TO_MHAURA, FFXIEnums.GUILDS.FI });
            db.NPCs.Rows.Add(new object[] { "Jidwahn", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.SILVER_SEA_ROUTE_TO_NASHMAU, FFXIEnums.GUILDS.FI });
            db.NPCs.Rows.Add(new object[] { "Yahliq", NPC_TYPE.GUILD_MERCH, FFXIEnums.ZONES.SILVER_SEA_ROUTE_TO_AL_ZAHBI, FFXIEnums.GUILDS.FI });
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
        #region Regional Merchants
        #endregion Regional Merchants
        #region Sell Any Merchants
        #endregion Sell Any Merchants
        #region Delivery NPC's
        #endregion Delivery NPC's
    }
}
