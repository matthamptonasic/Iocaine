﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2
{
    public class FFXIEnums
    {
        #region Game Enums
        public enum ZONES : ushort
        {
            UNKNOWN = 0,
			PHANAUET_CHANNEL = 1,
			CARPENTERS_LANDING = 2,
			MANACLIPPER = 3,
			BIBIKI_BAY = 4,
			ULEGUERAND_RANGE = 5,
			BEARCLAW_PINNACLE = 6,
			ATTOHWA_CHASM = 7,
			BONEYARD_GULLY = 8,
			PSO_XJA = 9,
			SHROUDED_MAW = 10,
			OLDTON_MOVALPOLOS = 11,
			NEWTON_MOVALPOLOS = 12,
			MINESHAFT_2716 = 13,
			HALL_OF_TRANSFERENCE = 14,
            ABYSSEA_KONSCHTAT = 15,
			PROMYVION_HOLLA = 16,
			SPIRE_OF_HOLLA = 17,
			PROMYVION_DEM = 18,
			SPIRE_OF_DEM = 19,
			PROMYVION_MEA = 20,
			SPIRE_OF_MEA = 21,
			PROMYVION_VAHZL = 22,
			SPIRE_OF_VAZHL = 23,
			LUFAISE_MEADOWS = 24,
			MISAREAUX_COAST = 25,
			TAVNAZIAN_SAFEHOLD = 26,
			PHOMIUNA_AQUEDUCTS = 27,
			SACRARIUM = 28,
			RIVERNE_SITE_B01 = 29,
			RIVERNE_SITE_A01 = 30,
			MONARCH_LINN = 31,
			SEALIONS_DEN = 32,
			AL_TAIEU = 33,
			GRAND_PALACE_OF_HU_XZOI = 34,
			THE_GARDEN_OF_RU_HMET = 35,
			EMPYREAL_PARADOX = 36,
			TEMENOS = 37,
			APOLLYON = 38,
			DYNAMIS_VALKURM = 39,
			DYNAMIS_BUBERIMU = 40,
			DYNAMIS_QUFIM = 41,
			DYNAMIS_TAVNAZIA = 42,
			DIORAMA_ABDHALJS_GHELSBA = 43,
			ABDHALJS_ISLE_PURGONORGO = 44,
            ABYSSEA_TAHRONGI = 45,
            OPEN_SEA_ROUTE_TO_AL_ZAHBI = 46,
            OPEN_SEA_ROUTE_TO_MHAURA = 47,
			AL_ZAHBI = 48,
            NONE = 49,
			AHT_URHGAN_WHITEGATE = 50,
			WAJAOM_WOODLANDS = 51,
			BHAFLAU_THICKETS = 52,
			NASHMAU = 53,
			ARRAPAGO_REEF = 54,
			ILRUSI_ATOLL = 55,
			PERIQIA = 56,
			TALACCA_COVE = 57,
			SILVER_SEA_ROUTE_TO_NASHMAU = 58,
			SILVER_SEA_ROUTE_TO_AL_ZAHBI = 59,
			ASHU_TALIF = 60,
			MOUNT_ZHAYOLM = 61,
			HALVUNG = 62,
			LEBROS_CAVERN = 63,
			NAVUKGO_EXECUTION_CHAMBER = 64,
			MAMOOK = 65,
			MAMOOL_JA_TRAINING_GROUNDS = 66,
			JADE_SEPULCHER = 67,
			AYDEEWA_SUBTERRANE = 68,
			LEUJAOAM_SANCTUM = 69,
			CHOCOBO_CIRCUIT = 70,
			THE_COLOSSEUM = 71,
			ALZADAAL_UNDERSEA_RUINS = 72,
			ZHAYOLM_REMNANTS = 73,
			ARRAPAGO_REMNANTS = 74,
			BHAFLAU_REMNANTS = 75,
			SILVER_SEA_REMNANTS = 76,
			NYZUL_ISLE = 77,
            HAZHALM_TESTING_GROUNDS = 78,
			CAEDARVA_MIRE = 79,
			SOUTHERN_SAN_DORIA_S = 80,
			EAST_RONFAURE_S = 81,
			JUGNER_FOREST_S = 82,
			VUNKERL_INLET_S = 83,
			BATALLIA_DOWNS_S = 84,
			LA_VAULE_S = 85,
			EVERBLOOM_HOLLOW = 86,
			BASTOK_MARKETS_S = 87,
			NORTH_GUSTABERG_S = 88,
			GRAUBERG_S = 89,
			PASHHOW_MARSHLANDS_S = 90,
			ROLANBERRY_FIELDS_S = 91,
			BEADEAUX_S = 92,
			RUHOTZ_SILVERMINES = 93,
			WINDURST_WATERS_S = 94,
			WEST_SARUTABARUTA_S = 95,
			FORT_KARUGO_NARUGO_S = 96,
			MERIPHATAUD_MOUNTAINS_S = 97,
			SAUROMUGUE_CHAMPAIGN_S = 98,
			CASTLE_OZTROJA_S = 99,
			WEST_RONFAURE = 100,
			EAST_RONFAURE = 101,
			LA_THEINE_PLATEAU = 102,
			VALKURM_DUNES = 103,
			JUGNER_FOREST = 104,
			BATALLIA_DOWNS = 105,
			NORTH_GUSTABERG = 106,
			SOUTH_GUSTABERG = 107,
			KONSCHTAT_HIGHLANDS = 108,
			PASHHOW_MARSHLANDS = 109,
			ROLANBERRY_FIELDS = 110,
			BEAUCEDINE_GLACIER = 111,
			XARCABARD = 112,
			CAPE_TERIGGAN = 113,
			EASTERN_ALTEPA_DESERT = 114,
			WEST_SARUTABARUTA = 115,
			EAST_SARUTABARUTA = 116,
			TAHRONGI_CANYON = 117,
			BUBURIMU_PENINSULA = 118,
			MERIPHATAUD_MOUNTAINS = 119,
			SAUROMUGUE_CHAMPAIGN = 120,
			SANCTUARY_OF_ZI_TAH = 121,
			RO_MAEVE = 122,
			YUHTUNGA_JUNGLE = 123,
			YHOATOR_JUNGLE = 124,
			WESTERN_ALTEPA_DESERT = 125,
			QUFIM_ISLAND = 126,
			BEHEMOTHS_DOMINION = 127,
			VALLEY_OF_SORROWS = 128,
			GHOYUS_REVERIE = 129,
			RU_AUN_GARDENS = 130,
			MORDION_GAOL = 131,
            ABYSSEA_LA_THEINE = 132,
			DYNAMIS_BEAUCEDINE = 134,
			DYNAMIS_XARCABARD = 135,
			BEAUCEDINE_GLACIER_S = 136,
			XARCABARD_S = 137,
			CASTLE_ZVAHL_BAILEYS_S = 138,
			HORLAIS_PEAK = 139,
			GHELSBA_OUTPOST = 140,
			FORT_GHELSBA = 141,
			YUGHOTT_GROTTO = 142,
			PALBOROUGH_MINES = 143,
			WAUGHROON_SHRINE = 144,
			GIDDEUS = 145,
			BALGAS_DAIS = 146,
			BEADEAUX = 147,
			QULUN_DOME = 148,
			DAVOI = 149,
			MONASTIC_CAVERN = 150,
			CASTLE_OZTROJA = 151,
			ALTAR_ROOM = 152,
			THE_BOYAHDA_TREE = 153,
			DRAGONS_AERY = 154,
			CASTLE_ZVAHL_KEEP_S = 155,
			THRONE_ROOM_S = 156,
			MIDDLE_DELKFUTTS_TOWER = 157,
			UPPER_DELKFUTTS_TOWER = 158,
			TEMPLE_OF_UGGALEPI = 159,
			DEN_OF_RANCOR = 160,
			CASTLE_ZVAHL_BAILEYS = 161,
			CASTLE_ZVAHL_KEEP = 162,
			SACRIFICIAL_CHAMBER = 163,
			GARLAIGE_CITADEL_S = 164,
			THRONE_ROOM = 165,
			RANGUEMONT_PASS = 166,
			BOSTAUNIEUX_OUBLIETTE = 167,
			CHAMBER_OF_ORACLES = 168,
			TORAIMARAI_CANAL = 169,
			FULL_MOON_FOUNTAIN = 170,
			CRAWLERS_NEST_S = 171,
			ZERUHN_MINES = 172,
			KORROLOKA_TUNNEL = 173,
			KUFTAL_TUNNEL = 174,
			ELDIEME_NECROPOLIS_S = 175,
			SEA_SERPENT_GROTTO = 176,
			VE_LUGANNON_PALACE = 177,
			THE_SHRINE_OF_RUAVITAU = 178,
			STELLAR_FULCRUM = 179,
			LA_LOFF_AMPHITHEATER = 180,
            THE_CELESTIAL_NEXUS = 181,
			WALK_OF_ECHOES = 182,
			//THE_LAST_STAND = 183,
            MAQUETTE_ABDHALJS_LEGION = 183,
			LOWER_DELKFUTTS_TOWER = 184,
			DYNAMIS_SAN_DORIA = 185,
			DYNAMIS_BASTOK = 186,
			DYNAMIS_WINDURST = 187,
			DYNAMIS_JEUNO = 188,
			//THE_CELESTIAL_NEXUS = 189,
			KING_RANPERRES_TOMB = 190,
			DANGRUF_WADI = 191,
			INNER_HORUTOTO_RUINS = 192,
			ORDELLES_CAVES = 193,
			OUTER_HORUTOTO_RUINS = 194,
			THE_ELDIEME_NECROPOLIS = 195,
			GUSGEN_MINES = 196,
			CRAWLERS_NEST = 197,
			MAZE_OF_SHAKHRAMI = 198,
			GARLAIGE_CITADEL = 200,
			CLOISTER_OF_GALES = 201,
			CLOISTER_OF_STORMS = 202,
			CLOISTER_OF_FROST = 203,
			FEI_YIN = 204,
			IFRITS_CAULDRON = 205,
			QU_BIA_ARENA = 206,
			CLOISTER_OF_FLAMES = 207,
			QUICKSAND_CAVES = 208,
			CLOISTER_OF_TREMORS = 209,
			CLOISTER_OF_TIDES = 211,
			GUSTAV_TUNNEL = 212,
			LABYRINTH_OF_ONZOZO = 213,
			ABYSSEA_ATTOHWA = 215,
			//SAN_DORIA_RESIDENTIAL_AREA = 216,
            ABYSSEA_MISAREAUX = 216,
            ABYSSEA_VUNKERL = 217,
			ABYSSEA_ALTEPA = 218,
            SHIP_BOUND_FOR_SELBINA = 220,
            SHIP_BOUND_FOR_MHAURA = 221,
            PROVENANCE = 222,
            SAN_DORIA_JEUNO_AIRSHIP = 223,
			BASTOK_JEUNO_AIRSHIP = 224,
			WINDURST_JEUNO_AIRSHIP = 225,
			KAZHAM_JEUNO_AIRSHIP = 226,
            SHIP_BOUND_FOR_SELBINA_PIRATES = 227,
            SHIP_BOUND_FOR_MHAURA_PIRATES = 228,
			SOUTHERN_SAN_DORIA = 230,
			NORTHERN_SAN_DORIA = 231,
			PORT_SAN_DORIA = 232,
			CHATEAU_DORAGUILLE = 233,
			BASTOK_MINES = 234,
			BASTOK_MARKETS = 235,
			PORT_BASTOK = 236,
			METALWORKS = 237,
			WINDURST_WATERS = 238,
			WINDURST_WALLS = 239,
			PORT_WINDURST = 240,
			WINDURST_WOODS = 241,
			HEAVENS_TOWER = 242,
			RU_LUDE_GARDENS = 243,
			UPPER_JEUNO = 244,
			LOWER_JEUNO = 245,
			PORT_JEUNO = 246,
			RABAO = 247,
			SELBINA = 248,
			MHAURA = 249,
			KAZHAM = 250,
			HALL_OF_THE_GODS = 251,
			NORG = 252,
            ABYSSEA_ULEGUERAND = 253,
            ABYSSEA_GRAUBERG = 254,
            ABYSSEA_EMPYREAL_PARADOX = 255,
            WESTERN_ADOULIN = 256,
            EASTERN_ADOULIN = 257,
            RALA_WATERWAYS = 258,
            RALA_WATERWAYS_U = 259,
            YAHSE_HUNTING_GROUNDS = 260,
            CEIZAK_BATTLEGROUNDS = 261,
            FORET_DE_HENNETIEL = 262,
            YORCIA_WEALD = 263,
            YORICA_WEALD_U = 264,
            MORIMAR_BASALT_FIELDS = 265,
            MARJAMI_RAVINE = 266,
            KAMIHR_DRIFTS = 267,
            SIH_GATES = 268,
            MOH_GATES = 269,
            CIRDAS_CAVERNS = 270,
            CIRDAS_CAVERNS_U = 271,
            DHO_GATES = 272,
            WOH_GATES = 273,
            OUTER_RA_KAZNA = 274,
            OUTER_RA_KAZNA_U = 275,
            MOG_GARDEN = 280,
            SILVER_KNIFE = 283,
            CELENNIA_WEXWORTH_MEMORIAL_LIBRARY = 284,
            FERETORY = 285
        }
        public enum WEATHER : byte
        {
            CLEAR = 0,
            SUNNY = 1,
            CLOUDS = 2,
            FOGGY = 3,
            FIRE = 4,
            FIRE_x2 = 5,
            WATER = 6,
            WATER_x2 = 7,
            EARTH = 8,
            EARTH_x2 = 9,
            WIND = 10,
            WIND_x2 = 11,
            ICE = 12,
            ICE_x2 = 13,
            THUNDER = 14,
            THUNDER_x2 = 15,
            LIGHT = 16,
            LIGHT_x2 = 17,
            DARK = 18,
            DARK2 = 19
        }
        public enum DAY : byte
        {
            FIRESDAY = 0,
            EARTHSDAY = 1,
            WATERSDAY = 2,
            WINDSDAY = 3,
            ICEDAY = 4,
            LIGHTNINGDAY = 5,
            LIGHTSDAY = 6,
            DARKSDAY = 7
        }
        public enum ELEMENT : byte
        {
            FIRE = 0,
            EARTH = 1,
            WATER = 2,
            WIND = 3,
            ICE = 4,
            LIGHTNING = 5,
            LIGHT = 6,
            DARK = 7,
            UNKNOWN = 8,
            PHYSICAL = 0xFF
        }
        public enum WS_ATTRIBUTES : byte
        {
            NONE = 0,
            COMPRESSION = 1,
            DARKNESS = 2,
            DETONATION = 3,
            DISTORTION = 4,
            FRAGMENTATION = 5,
            FUSION = 6,
            GRAVITATION = 7,
            IMPACTION = 8,
            INDURATION = 9,
            LIGHT = 10,
            LIQUIFICATION = 11,
            REVERBERATION = 12,
            SCISSION = 13,
            TRANSFIXION = 14
        }
        public enum STATUS : byte
        {
            NORMAL = 0,
            ATTACKING = 1,
            KO1 = 2,
            KO2 = 3,
            SYSTEM_WAIT = 4,
            CHOCO = 5,
            HEALING = 33,
            FISH_ON_HOOK = 57,
            FISH_CAUGHT = 58,  //Also "obtained" according to pyrol
            ROD_BREAK = 59,
            LINE_BREAK = 60,
            FISH_LOST = 62,     //Line Break according to pyrol
            CAUGHT_MOB = 61,    //According to pyrol
            SYNTHING = 44,      //According to pyrol
            SITTING = 47,
            FISHING = 56
        }
        public enum STATUS_EFFECT : ushort
        {
            KO = 0,
            Weakness = 1,
            Sleep = 2,
            Poison = 3,
            Paralysis = 4,
            Blindness = 5,
            Silence = 6,
            Petrification = 7,
            Disease = 8,
            Curse = 9,
            Stun = 10,
            Bind = 11,
            Weight = 12,
            Slow = 13,
            Charm1 = 14,
            Doom = 15,
            Amnesia = 16,
            Charm2 = 17,
            Weight_To_Petrified = 18,
            Sleep2 = 19,
            Terror = 28,
            Mute = 29,
            Bane = 30,
            Plague = 31,
            Flee = 32,
            Haste = 33,
            Blaze_Spikes = 34,
            Ice_Spikes = 35,
            Blink = 36,
            Stoneskin = 37,
            Shock_Spikes = 38,
            Aquaveil = 39,
            Protect = 40,
            Shell = 41,
            Regen = 42,
            Refresh = 43,
            Mighty_Strikes = 44,
            Boost = 45,
            Hundred_Fists = 46,
            Manafont = 47,
            Chainspell = 48,
            Perfect_Dodge = 49,
            Invincible = 50,
            Blood_Weapon = 51,
            Soul_Voice = 52,
            Eagle_Eye_Shot = 53,
            Meikyo_Shisui = 54,
            Astral_Flow = 55,
            Berserk = 56,
            Defender = 57,
            Aggressor = 58,
            Focus = 59,
            Dodge = 60,
            Counterstance = 61,
            Sentinel = 62,
            Souleater = 63,
            Last_Resort = 64,
            Sneak_Attack = 65,
            Copy_Image = 66,
            Third_Eye = 67,
            Warcry = 68,
            Invisible = 69,
            Deodorize = 70,
            Sneak = 71,
            Sharpshot = 72,
            Barrage = 73,
            Holy_Circle = 74,
            Arcane_Circle = 75,
            Hide = 76,
            Camouflage = 77,
            Divine_Seal = 78,
            Elemental_Seal = 79,
            STR_Boost1 = 80,
            DEX_Boost1 = 81,
            VIT_Boost1 = 82,
            AGI_Boost1 = 83,
            INT_Boost1 = 84,
            MND_Boost1 = 85,
            CHR_Boost1 = 86,
            Trick_Attack = 87,
            Max_HP_Boost = 88,
            Max_MP_Boost = 89,
            Accuracy_Boost = 90,
            Attack_Boost = 91,
            Evasion_Boost = 92,
            Defense_Boost = 93,
            Enfire = 94,
            Enblizzard = 95,
            Enaero = 96,
            Enstone = 97,
            Enthunder = 98,
            Enwater = 99,
            Barfire = 100,
            Barblizzard = 101,
            Baraero = 102,
            Barstone = 103,
            Barthunder = 104,
            Barwater = 105,
            Barsleep = 106,
            Barpoison = 107,
            Barparalyze = 108,
            Barblind = 109,
            Barsilence = 110,
            Barpetrify = 111,
            Barvirus = 112,
            Reraise = 113,
            Cover = 114,
            Unlimited_Shot = 115,
            Phalanx = 116,
            Warding_Circle = 117,
            Ancient_Circle = 118,
            STR_Boost2 = 119,
            DEX_Boost2 = 120,
            VIT_Boost2 = 121,
            AGI_Boost2 = 122,
            INT_Boost2 = 123,
            MND_Boost2 = 124,
            CHR_Boost2 = 125,
            Spirit_Surge = 126,
            Costume = 127,
            Burn = 128,
            Frost = 129,
            Choke = 130,
            Rasp = 131,
            Shock = 132,
            Drown = 133,
            Dia = 134,
            Bio = 135,
            STR_Down = 136,
            DEX_Down = 137,
            VIT_Down = 138,
            AGI_Down = 139,
            INT_Down = 140,
            MND_Down = 141,
            CHR_Down = 142,
            Level_Restriction = 143,
            Max_HP_Down = 144,
            Max_MP_Down = 145,
            Accuracy_Down = 146,
            Attack_Down = 147,
            Evasion_Down = 148,
            Defense_Down = 149,
            Physical_Shield = 150,
            Arrow_Shield = 151,
            Magic_Shield1 = 152,
            Damage_Spikes = 153,
            Shining_Ruby = 154,
            Medicine = 155,
            Flash = 156,
            Subjob_Restriction = 157,
            Provoke = 158,
            Penalty = 159,
            Preparations = 160,
            Sprint = 161,
            Enchantment = 162,
            Azure_Lore = 163,
            Chain_Affinity = 164,
            Burst_Affinity = 165,
            Overdrive = 166,
            Magic_Def_Down = 167,
            Inhibit_TP = 168,
            Potency = 169,
            Regain = 170,
            Pax = 171,
            Magic_Shield2 = 189,
            Magic_Atk_Boost = 190,
            Magic_Def_Boost = 191,
            Requiem = 192,
            Lullaby = 193,
            Elegy = 194,
            Paeon = 195,
            Ballad = 196,
            Minne = 197,
            Minuet = 198,
            Madrigal = 199,
            Prelude = 200,
            Mambo = 201,
            Aubade = 202,
            Pastoral = 203,
            Hum = 204,
            Fantasia = 205,
            Operetta = 206,
            Capriccio = 207,
            Serenade = 208,
            Round = 209,
            Gavotte = 210,
            Fugue = 211,
            Rhapsody = 212,
            Aria = 213,
            March = 214,
            Etude = 215,
            Carol = 216,
            Threnody = 217,
            Hymnus = 218,
            Mazurka = 219,
            Sirvente = 220,
            Auto_Regen = 233,
            Auto_Refresh = 234,
            Fishing_Imagery = 235,
            Woodworking = 236,
            Smithing = 237,
            Goldsmithing = 238,
            Clothcraft = 239,
            Leathercraft = 240,
            Bonecraft = 241,
            Alchemy = 242,
            Cooking = 243,
            Dedication = 249,
            Ef_Badge = 250,
            Food = 251,
            Chocobo = 252,
            Signet = 253,
            Battlefield = 254,
            Sanction = 256,
            Besieged = 257,
            Illusion = 258,
            No_Weapons_Armor = 259,
            No_Support_Job = 260,
            No_Job_Abilities = 261,
            No_Magic_Casting = 262,
            Penalty_to_Attribute_s_ = 263,
            Overload = 299,
            Fire_Maneuver = 300,
            Ice_Maneuver = 301,
            Wind_Maneuver = 302,
            Earth_Maneuver = 303,
            Thunder_Maneuver = 304,
            Water_Maneuver = 305,
            Light_Maneuver = 306,
            Dark_Maneuver = 307,
            Doubleup_Chance = 308,
            Bust = 309,
            Fighters_Roll = 310,
            Monks_Roll = 311,
            Healers_Roll = 312,
            Wizards_Roll = 313,
            Warlocks_Roll = 314,
            Rogues_Roll = 315,
            Gallants_Roll = 316,
            Chaos_Roll = 317,
            Beast_Roll = 318,
            Choral_Roll = 319,
            Hunters_Roll = 320,
            Samurai_Roll = 321,
            Ninja_Roll = 322,
            Drachen_Roll = 323,
            Evokers_Roll = 324,
            Maguss_Roll = 325,
            Corsairs_Roll = 326,
            Puppet_Roll = 327,
            Warriors_Charge = 340,
            Formless_Strikes = 341,
            Assassins_Charge = 342,
            Feint = 343,
            Fealty = 344,
            Dark_Seal = 345,
            Diabolic_Eye = 346,
            Nightingale = 347,
            Troubadour = 348,
            Killer_Instinct = 349,
            Stealth_Shot = 350,
            Flashy_Shot = 351,
            Sange = 352,
            Hasso = 353,
            Seigan = 354
        }
        public enum MAGIC_TYPE : byte
        {
            UNKNOWN = 0,
            BLUE = 1,
            SONG = 2,
            NINJUTSU = 3,
            BLACK = 4,
            WHITE = 5,
            SUMMON = 6
        }
        public enum CRAFT_RANK : byte
        {
            AMATEUR = 0,
            RECRUIT = 1,
            INITIATE = 2,
            NOVICE = 3,
            APPRENTICE = 4,
            JOURNEYMAN = 5,
            CRAFTSMAN = 6,
            ARTISAN = 7,
            ADEPT = 8,
            VETERAN = 9,
            EXPERT = 10
        }
        public enum CRAFTS : byte
        {
            NONE = 0,
            FI = 1,
            WW = 2,
            SM = 3,
            GS = 4,
            CC = 5,
            LC = 6,
            BC = 7,
            AL = 8,
            CK = 9
        }
        public enum GUILDS : byte
        {
            NONE = 0,
            FI = 1,
            WW = 2,
            SM = 3,
            GS = 4,
            CC = 5,
            LC = 6,
            BC = 7,
            AL = 8,
            CK = 9,
            TE = 10
        }
        public enum CRYSTALS : byte
        {
            FIRE = 0,
            EARTH = 1,
            WATER = 2,
            WIND = 3,
            ICE = 4,
            LIGHTNING = 5,
            LIGHT = 6,
            DARK = 7,
            UNKNOWN = 8
        }
        public enum JOBS : byte
        {
            NONE = 0,
            WAR = 1,
            MNK = 2,
            WHM = 3,
            BLM = 4,
            RDM = 5,
            THF = 6,
            PLD = 7,
            DRK = 8,
            BST = 9,
            BRD = 10,
            RNG = 11,
            SAM = 12,
            NIN = 13,
            DRG = 14,
            SMN = 15,
            BLU = 16,
            COR = 17,
            PUP = 18,
            DNC = 19,
            SCH = 20,
            GEO = 21,
            RUN = 22
        }
        public enum XP_MODE : byte
        {
            XP = 96,
            MERIT = 224
        }
        public enum EQUIP_SLOT : byte
        {
            MAIN = 0,
            SUB = 1,
            RANGE = 2,
            AMMO = 3,
            HEAD = 4,
            BODY = 5,
            HANDS = 6,
            LEGS = 7,
            FEET = 8,
            NECK = 9,
            WAIST = 10,
            EAR_LEFT = 11,
            EAR_RIGHT = 12,
            RING_LEFT = 13,
            RING_RIGHT = 14,
            BACK = 15
        }
        public enum WEAPON_SKILL_TYPE : byte
        {
            UNKNOWN = 0,
            HAND_TO_HAND = 1,
            DAGGER = 2,
            SWORD = 3,
            G_SWORD = 4,
            AXE = 5,
            G_AXE = 6,
            SCYTHE = 7,
            POLE_ARM = 8,
            KATANA = 9,
            G_KATANA = 10,
            CLUB = 11,
            STAFF = 12,
            ARCHERY = 0x19,
            MARKSMANSHIP = 0x1A,
            THROWING = 0x1B
        }
        public enum ITEM_ID : ushort
        {
            FISHERMANS_TUNICA = 13808,
            FISHERMANS_GLOVES = 14070,
            FISHERMANS_HOSE = 14292,
            FISHERMANS_BOOTS = 14171,
            ANGLERS_TUNICA = 13809,
            ANGLERS_GLOVES = 14071,
            ANGLERS_HOSE = 14293,
            ANGLERS_BOOTS = 14172,
            WADERS = 14195,
            FISHERMANS_APRON = 14400,
            FISHING_HOLE_MAP = 191,
            THE_BIG_ONE = 195,
            FISHERMANS_SIGNBOARD = 340,
            RUSTY_BUCKET = 90,
            BLUE_BAMBOO_GRASS = 324,
            GREEN_BAMBOO_GRASS = 325,
            RED_BAMBOO_GRASS = 323,
            PELICAN_RING = 15554,
            ALBATROSS_RING = 15555,
            PENGUIN_RING = 15556
        }
        public enum KEY_ITEM_ID : ushort
        {
            SERPENT_RUMORS = 10,
            FROG_FISHING = 11,
            MOOCHING = 12
        }
        public enum CHAT_MODE : byte
        {
            ZONE_ENTERED = 0x0,
            TITLE_WHEN_CHECKED = 0x0,
            SAY_OUT1 = 0x1,
            SAY_OUT2 = 0x2,
            TELL_OUT = 0x4,
            PARTY_OUT = 0x5,
            LINKSHELL_OUT = 0x6,
            EMOTE_OUT_UNTARGETED = 0x7,
            SAY_INC1 = 0x9,
            SAY_INC2 = 0xa,
            TELL_INC1 = 0xc,
            TELL_INC2 = 0xd,
            LINKSHELL_INC = 0xe,
            EMOTE_INC_TARGETED = 0xf,
            ME_TAKES_DMG = 0x1c,
            ME_AVOIDS_DMG = 0x1d,
            ME_RECOVERS_HP_MP_FROM_MOB = 0x1e,
            ME_RECOVERS_HP_CURE = 0x1f,
            SYNTH_MADE_ITEM = 0x79,
            SYNTH_LOST_ITEM = 0x79,
            ME_BUFF_WEARING = 0x7b,
            TREASURE_OBTAINED = 0x7f,
            SKILL_COMBAT_GAINED = 0x81,
            XP_GAINED = 0x83,
            INVITE_TO_PARTY = 0x87,
            FISH_CAUGHT = 0x92,
            FISH_MESSAGE = 0x94,
            CHOCO_DIGGING_RESULT = 0x94,
            OBTAINED_ITEM_GIL = 0x94,
            NPC_CHAT = 0x98,
            CONQUEST_UPDATE = 0xa1,
            SYNERGY_MESSAGE = 0xbe,
            ME_BUFF_WORE_OFF = 0xbf,
            ME_EXAMINED = 0xd0
        }
        #endregion Game Enums
        #region Iocaine Enums
        #region Fishing
        //This enum is for the fisher.
        //I only put it in a global space so that I could access the same
        //enum from anywhere.
        //This enum is used to represent the bit vector that represents
        //items and equipment that affect fishing such as
        //Fisherman's tunica and Waders
        //It also stores the killfish setting when a fish is caught
        //and will hopefully one day hold key items and furniture
        public enum FISH_INFO_VECTOR_BIT : byte
        {
            FISHERMANS_TUNICA = 0,
            FISHERMANS_GLOVES = 1,
            FISHERMANS_HOSE = 2,
            FISHERMANS_BOOTS = 3,
            ANGLERS_TUNICA = 4,
            ANGLERS_GLOVES = 5,
            ANGLERS_HOSE = 6,
            ANGLERS_BOOTS = 7,
            WADERS = 8,
            FISHERMANS_APRON = 9,
            SERPENT_RUMORS = 10,
            FROG_FISHING = 11,
            MOOCHING = 12,
            FISHING_HOLE_MAP = 13,
            THE_BIG_ONE = 14,
            FISHERMANS_SIGNBOARD = 15,
            RUSTY_BUCKET = 16,
            BLUE_BAMBOO_GRASS = 17,
            GREEN_BAMBOO_GRASS = 18,
            RED_BAMBOO_GRASS = 19,
            SKILL_UP_0 = 23,
            SKILL_UP_1 = 24,
            SKILL_UP_2 = 25,
            IMAGERY = 26,
            PELICAN_RING = 27,
            ALBATROSS_RING = 28,
            PENGUIN_RING = 29,
            KILL_FISH = 30
        }
        public enum FISHING_RESULT : byte
        {
            CAUGHT_FISH = 1,
            CAUGHT_ITEM = 2,
            CAUGHT_MONSTER = 3,
            GAVE_UP = 4,
            DIDNT_CATCH_ANYTHING = 5,
            LINE_BROKE = 6,
            RELEASED_INV_FULL = 7,
            NOT_ENOUGH_SKILL = 8,
            GOT_AWAY = 9,
            ROD_BROKE = 10,
            TOO_SMALL = 11,
            TOO_LARGE = 12,
            UNKNOWN = 13
        }
        public enum CATCH_TYPE : byte
        {
            FISH = 0,
            ITEM = 1,
            MONSTER = 2
        }
        #endregion Fishing
        #region Items
        public enum ITEM_DROP_TYPE : byte
        {
            NORMAL = 0,
            RARE_EX = 1
        }
        public enum ITEM_IDS_CRYSTALS : ushort
        {
            Fire_Crystal = 4096,
            Ice_Crystal = 4097,
            Wind_Crystal = 4098,
            Earth_Crystal = 4099,
            Lightning_Crystal = 4100,
            Water_Crystal = 4101,
            Light_Crystal = 4102,
            Dark_Crystal = 4103
        }
        public enum ITEM_IDS_FISH : ushort
        {
            gil_1 = 1,
            gil_100 = 100,
            abaia = 5476,
            ahtapot = 5455,
            alabaligi = 5461,
            ancient_carp = 6373,
            apkallufa = 5534,
            armored_pisces = 4316,
            arrowwood_log = 688,
            Barnacle = 5954,
            bastore_bream = 4461,
            bastore_sardine = 4360,
            bastore_sardines_2 = 11,
            bastore_sardines_3 = 8,
            Bastore_sweeper = 5473,
            Betta = 5139,
            Bhefhel_marlin = 4479,
            Bibiki_urchin = 4318,
            Bibikibo = 4314,
            black_bubble_eye = 4311,
            black_eel = 4429,
            black_ghost = 5138,
            Black_Prawn = 5948,
            black_sole = 4384,
            bladefish = 4471,
            blindfish = 4313,
            Bloodblotch = 5951,
            Blowfish = 5812,
            bluetail = 4399,
            Bonefish = 6336,
            brass_loach = 5469,
            bugbear_mask = 1624,
            ca_cuong = 5474,
            Caedarva_frog = 5465,
            cameroceras = 6338,
            cave_cherax = 4309,
            Cheval_salmon = 4379,
            Clotflagration = 6001,
            clump_of_Pamtam_kelp = 624,
            cobalt_jellyfish = 4443,
            cone_calamary = 5128,
            cone_calamary_2 = 13,
            cone_calamary_3 = 14,
            Contortacle = 5962,
            Contortopus = 5961,
            copper_frog = 4515,
            copper_ring = 13454,
            coral_butterfly = 4580,
            coral_fragment = 887,
            crayfish = 4472,
            crescent_fish = 4473,
            Crocodilos = 5814,
            crystal_bass = 4528,
            damp_scroll = 1210,
            dark_bass = 4428,
            Deademoiselle = 5535,
            denizanasi = 5447,
            devil_manta = 20,
            dil = 5457,
            Dorado_Gar = 5813,
            dragoneye = 4048,
            Dragonfish = 5959,
            dragons_tabernacle = 6374,
            Duskcrawler = 9077,
            dwarf_remora = 6145,
            Elshimo_frog = 4290,
            Elshimo_newt = 4579,
            emperor_fish = 4454,
            fat_greedie = 4501,
            fish_scale_shield = 12316,
            forest_carp = 4289,
            Frigorifish = 6144,
            garpike = 5472,
            gavial_fish = 4477,
            generic_item = 2,
            gerrothorax = 5471,
            giant_catfish = 4469,
            giant_chirai = 4308,
            giant_donko = 4306,
            gigant_octopus = 5475,
            gigant_squid = 4474,
            gold_carp = 4427,
            gold_lobster = 4383,
            gold_ring = 13445,
            greedie = 4500,
            grimmonite = 4304,
            Gugru_tuna = 4480,
            Gugrusaurus = 5127,
            gurnard = 5132,
            hakuryu = 5539,
            hamsi = 5449,
            hamsi_2 = 17,
            hamsi_3 = 18,
            icefish = 4470,
            icefish_2 = 4,
            icefish_3 = 5,
            istakoz = 5453,
            istavrit = 5136,
            istiridye = 5456,
            jungle_catfish = 4307,
            kalamar = 5448,
            kalamar_2 = 15,
            kalamar_3 = 16,
            kalkanbaligi = 5140,
            kaplumbaga = 5464,
            kayabaligi = 5460,
            kilicbaligi = 5451,
            King_Perch = 5816,
            Kokuryu = 5540,
            lakerda = 5450,
            lamp_marimo = 2216,
            Lik = 5129,
            lord_of_Ulbuka = 6372,
            lungfish = 4315,
            Mackerel = 5950,
            Malicious_Perch = 5995,
            matsya = 5468,
            megalodon = 5467,
            mercanbaligi = 5454,
            moat_carp = 4401,
            Moblin_mask = 1638,
            mola_mola = 5134,
            monke_onke = 4462,
            Monster = 3,
            moorish_idol = 5121,
            morinabaligi = 5462,
            muddy_siredon = 5266,
            Mussel = 5949,
            mythril_dagger = 16451,
            nebimonite = 4459,
            noble_lady = 4485,
            Norg_shell = 1135,
            Nosteau_herring = 4482,
            ogre_eel = 4481,
            pair_of_rusty_leggings = 14117,
            Pelazoea = 5815,
            Phanauet_newt = 5125,
            phantom_serpent = 6375,
            pipira = 4464,
            pirarucu = 5470,
            pterygotus = 5133,
            quicksilver_blade = 6371,
            quus = 4514,
            quus_2 = 6,
            quus_3 = 12,
            RaKaznar_Shellfish = 6334,
            red_bubble_eye = 5446,
            red_terrapin = 4402,
            remora = 6146,
            rhinochimera = 5135,
            ripped_cap = 591,
            Ruddy_Seema = 5952,
            rusty_bucket = 90,
            rusty_cap = 12522,
            rusty_greatsword = 16606,
            Rusty_Kunai = 19283,
            rusty_pick = 16655,
            Rusty_Spear = 19308,
            rusty_subligar = 14242,
            ryugu_titan = 4305,
            sandfish = 4291,
            sandfish_2 = 10,
            sandfish_3 = 9,
            sazanbaligi = 5459,
            sea_zombie = 4475,
            Sekiryu = 5538,
            Senroh_frog = 5993,
            Senroh_Sardine = 5963,
            Senroh_Sardine_2 = 22,
            Senroh_Sardine_3 = 23,
            shall_shell = 4484,
            shining_trout = 4354,
            silver_ring = 13456,
            silver_shark = 4451,
            Soryu = 5537,
            takitaro = 4463,
            Tavnazian_goby = 5130,
            three_eyed_fish = 4478,
            Thysanopeltis = 6337,
            tiger_cod = 4483,
            tiger_shark = 5817,
            tiny_goldfish = 4310,
            titanic_sawfish = 5120,
            titanictus = 4476,
            Translucent_Salpa = 6333,
            tricolored_carp = 4426,
            tricorn = 4319,
            trilobite = 4317,
            trumpet_shell = 5466,
            turnabaligi = 5137,
            tusoteuthis_longa = 6376,
            Ulbukan_Lobster = 5960,
            unknown = 0,
            uskumru = 5452,
            Veydal_wrasse = 5141,
            vongola_clam = 5131,
            White_Lobster = 6335,
            Yawning_Catfish = 5955,
            yayinbaligi = 5463,
            yellow_globe = 4403,
            yellow_globes_2 = 19,
            yellow_globes_3 = 7,
            yilanbaligi = 5458,
            yorchete = 5536,
            Zafmlug_bass = 4385,
            zebra_eel = 4288
        }
        public enum ITEM_IDS_BAIT : ushort
        {
            Crayfish_Ball = 16997,
            Drill_Calamary = 17006,
            Dwarf_Pugil = 17007,
            Fly_Lure = 17405,
            Frog_Lure = 17403,
            Giant_Shell_Bug = 17001,
            Insect_Ball = 16998,
            Little_Worm = 17396,
            Lizard_Lure = 17401,
            Lufaise_Fly = 17005,
            Lugworm = 17395,
            Meatball = 17000,
            Minnow = 17407,
            Peeled_Crayfish = 16993,
            Peeled_Lobster = 17394,
            Robber_Rig = 17002,
            Rogue_Rig = 17398,
            Rotten_Meat = 16995,
            Sabiki_Rig = 17399,
            Sardine_Ball = 16996,
            Sea_Dragon_Liver = 19326,
            Shell_Bug = 17397,
            Shrimp_Lure = 17402,
            Sinking_Minnow = 17400,
            Slice_Of_Bluetail = 16992,
            Slice_Of_Carp = 16994,
            Sliced_Cod = 17393,
            Sliced_Sardine = 17392,
            Trout_Ball = 16999,
            Worm_Lure = 17404
        }
        public enum ITEM_IDS_RODS : ushort
        {
            Bamboo_Fishing_Rod = 17389,
            Carbon_Fishing_Rod = 17384,
            Clothespole = 17383,
            Composite_Fishing_Rod = 17381,
            Ebisu_Fishing_Rod = 17011,
            Ebisu_Fishing_Rod_Plus_1 = 19321,
            Fastwater_Fishing_Rod = 17388,
            Glass_Fiber_Fishing_Rod = 17385,
            Halcyon_Rod = 17015,
            Hume_Fishing_Rod = 17014,
            Lu_Shangs_Fishing_Rod = 17386,
            Lu_Shangs_Fishing_Rod_Plus_1 = 19320,
            Mithran_Fishing_Rod = 17380,
            Single_Hook_Fishing_Rod = 17382,
            Tarutaru_Fishing_Rod = 17387,
            Willow_Fishing_Rod = 17391,
            Yew_Fishing_Rod = 17390
        }
        #endregion Items
        #region Menus
        public enum INVENTORY_MENU
        {
            NONE = 0,
            BAG = 1,
            SACK = 2,
            SATCHEL = 3,
            CASE = 4,
            SAFE = 5,
            STORAGE = 6,
            LOCKER = 7,
            UNKNOWN = 8
        }
        #endregion Menus
        #endregion Iocaine Enums
        #region Global Enums
        public enum OSVersion
        {
            XP = 5,
            Vista = 6,
            Seven = 7,
            Eight = 8
        }
        #endregion Global Enums
    }
}
