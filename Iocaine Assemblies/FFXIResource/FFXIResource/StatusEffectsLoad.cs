using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class StatusEffects
    {
        private static void loadData()
        {
            MainDatabase.StatusEffectsRow row;

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "KO";
            row.ID = 0;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "weakness";
            row.ID = 1;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "sleep";
            row.ID = 2;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "poison";
            row.ID = 3;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "paralysis";
            row.ID = 4;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "blindness";
            row.ID = 5;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "silence";
            row.ID = 6;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "petrification";
            row.ID = 7;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "disease";
            row.ID = 8;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "curse";
            row.ID = 9;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "stun";
            row.ID = 10;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "bind";
            row.ID = 11;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "weight";
            row.ID = 12;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "slow";
            row.ID = 13;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "charm";
            row.ID = 14;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "doom";
            row.ID = 15;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "amnesia";
            row.ID = 16;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "charm";
            row.ID = 17;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "gradual petrification";
            row.ID = 18;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "sleep";
            row.ID = 19;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "curse";
            row.ID = 20;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "addle";
            row.ID = 21;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "intimidate";
            row.ID = 22;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Kaustra";
            row.ID = 23;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "ST24";
            row.ID = 24;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "ST25";
            row.ID = 25;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "ST26";
            row.ID = 26;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "ST27";
            row.ID = 27;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "terror";
            row.ID = 28;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "mute";
            row.ID = 29;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "bane";
            row.ID = 30;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "plague";
            row.ID = 31;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Flee";
            row.ID = 32;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Haste";
            row.ID = 33;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Blaze Spikes";
            row.ID = 34;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Ice Spikes";
            row.ID = 35;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Blink";
            row.ID = 36;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Stoneskin";
            row.ID = 37;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Shock Spikes";
            row.ID = 38;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Aquaveil";
            row.ID = 39;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Protect";
            row.ID = 40;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Shell";
            row.ID = 41;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Regen";
            row.ID = 42;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Refresh";
            row.ID = 43;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Mighty Strikes";
            row.ID = 44;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Boost";
            row.ID = 45;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Hundred Fists";
            row.ID = 46;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Manafont";
            row.ID = 47;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Chainspell";
            row.ID = 48;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Perfect Dodge";
            row.ID = 49;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Invincible";
            row.ID = 50;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Blood Weapon";
            row.ID = 51;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Soul Voice";
            row.ID = 52;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Eagle Eye Shot";
            row.ID = 53;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Meikyo Shisui";
            row.ID = 54;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Astral Flow";
            row.ID = 55;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Berserk";
            row.ID = 56;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Defender";
            row.ID = 57;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Aggressor";
            row.ID = 58;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Focus";
            row.ID = 59;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Dodge";
            row.ID = 60;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Counterstance";
            row.ID = 61;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sentinel";
            row.ID = 62;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Souleater";
            row.ID = 63;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Last Resort";
            row.ID = 64;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sneak Attack";
            row.ID = 65;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Copy Image";
            row.ID = 66;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Third Eye";
            row.ID = 67;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Warcry";
            row.ID = 68;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Invisible";
            row.ID = 69;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Deodorize";
            row.ID = 70;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sneak";
            row.ID = 71;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sharpshot";
            row.ID = 72;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Barrage";
            row.ID = 73;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Holy Circle";
            row.ID = 74;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Arcane Circle";
            row.ID = 75;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Hide";
            row.ID = 76;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Camouflage";
            row.ID = 77;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Divine Seal";
            row.ID = 78;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Elemental Seal";
            row.ID = 79;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "STR Boost";
            row.ID = 80;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "DEX Boost";
            row.ID = 81;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "VIT Boost";
            row.ID = 82;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "AGI Boost";
            row.ID = 83;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "INT Boost";
            row.ID = 84;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "MND Boost";
            row.ID = 85;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "CHR Boost";
            row.ID = 86;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Trick Attack";
            row.ID = 87;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Max HP Boost";
            row.ID = 88;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Max MP Boost";
            row.ID = 89;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Accuracy Boost";
            row.ID = 90;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Attack Boost";
            row.ID = 91;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Evasion Boost";
            row.ID = 92;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Defense Boost";
            row.ID = 93;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enfire";
            row.ID = 94;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enblizzard";
            row.ID = 95;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enaero";
            row.ID = 96;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enstone";
            row.ID = 97;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enthunder";
            row.ID = 98;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enwater";
            row.ID = 99;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Barfire";
            row.ID = 100;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Barblizzard";
            row.ID = 101;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Baraero";
            row.ID = 102;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Barstone";
            row.ID = 103;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Barthunder";
            row.ID = 104;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Barwater";
            row.ID = 105;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Barsleep";
            row.ID = 106;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Barpoison";
            row.ID = 107;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Barparalyze";
            row.ID = 108;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Barblind";
            row.ID = 109;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Barsilence";
            row.ID = 110;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Barpetrify";
            row.ID = 111;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Barvirus";
            row.ID = 112;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Reraise";
            row.ID = 113;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Cover";
            row.ID = 114;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Unlimited Shot";
            row.ID = 115;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Phalanx";
            row.ID = 116;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Warding Circle";
            row.ID = 117;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Ancient Circle";
            row.ID = 118;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "STR Boost";
            row.ID = 119;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "DEX Boost";
            row.ID = 120;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "VIT Boost";
            row.ID = 121;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "AGI Boost";
            row.ID = 122;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "INT Boost";
            row.ID = 123;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "MND Boost";
            row.ID = 124;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "CHR Boost";
            row.ID = 125;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Spirit Surge";
            row.ID = 126;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Costume";
            row.ID = 127;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Burn";
            row.ID = 128;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Frost";
            row.ID = 129;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Choke";
            row.ID = 130;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Rasp";
            row.ID = 131;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Shock";
            row.ID = 132;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Drown";
            row.ID = 133;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Dia";
            row.ID = 134;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Bio";
            row.ID = 135;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "STR Down";
            row.ID = 136;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "DEX Down";
            row.ID = 137;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "VIT Down";
            row.ID = 138;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "AGI Down";
            row.ID = 139;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "INT Down";
            row.ID = 140;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "MND Down";
            row.ID = 141;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "CHR Down";
            row.ID = 142;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Level Restriction";
            row.ID = 143;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Max HP Down";
            row.ID = 144;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Max MP Down";
            row.ID = 145;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Accuracy Down";
            row.ID = 146;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Attack Down";
            row.ID = 147;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Evasion Down";
            row.ID = 148;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Defense Down";
            row.ID = 149;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Physical Shield";
            row.ID = 150;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Arrow Shield";
            row.ID = 151;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Shield";
            row.ID = 152;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Damage Spikes";
            row.ID = 153;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Shining Ruby";
            row.ID = 154;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "medicine";
            row.ID = 155;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Flash";
            row.ID = 156;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "SJ Restriction";
            row.ID = 157;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Provoke";
            row.ID = 158;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "penalty";
            row.ID = 159;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "preparations";
            row.ID = 160;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sprint";
            row.ID = 161;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "enchantment";
            row.ID = 162;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Azure Lore";
            row.ID = 163;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Chain Affinity";
            row.ID = 164;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Burst Affinity";
            row.ID = 165;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Overdrive";
            row.ID = 166;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Def. Down";
            row.ID = 167;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Inhibit TP";
            row.ID = 168;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Potency";
            row.ID = 169;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Regain";
            row.ID = 170;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Pax";
            row.ID = 171;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Intension";
            row.ID = 172;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Dread Spikes";
            row.ID = 173;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Acc. Down";
            row.ID = 174;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Atk. Down";
            row.ID = 175;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "quickening";
            row.ID = 176;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "encumbrance";
            row.ID = 177;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Firestorm";
            row.ID = 178;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Hailstorm";
            row.ID = 179;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Windstorm";
            row.ID = 180;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sandstorm";
            row.ID = 181;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Thunderstorm";
            row.ID = 182;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Rainstorm";
            row.ID = 183;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Aurorastorm";
            row.ID = 184;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Voidstorm";
            row.ID = 185;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Helix";
            row.ID = 186;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sublimation: Activated";
            row.ID = 187;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sublimation: Complete";
            row.ID = 188;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Max TP Down";
            row.ID = 189;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Atk. Boost";
            row.ID = 190;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Def. Boost";
            row.ID = 191;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Requiem";
            row.ID = 192;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Lullaby";
            row.ID = 193;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Elegy";
            row.ID = 194;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Paeon";
            row.ID = 195;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Ballad";
            row.ID = 196;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Minne";
            row.ID = 197;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Minuet";
            row.ID = 198;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Madrigal";
            row.ID = 199;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Prelude";
            row.ID = 200;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Mambo";
            row.ID = 201;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Aubade";
            row.ID = 202;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Pastoral";
            row.ID = 203;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Hum";
            row.ID = 204;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Fantasia";
            row.ID = 205;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Operetta";
            row.ID = 206;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Capriccio";
            row.ID = 207;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Serenade";
            row.ID = 208;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Round";
            row.ID = 209;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Gavotte";
            row.ID = 210;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Fugue";
            row.ID = 211;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Rhapsody";
            row.ID = 212;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Aria";
            row.ID = 213;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "March";
            row.ID = 214;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Etude";
            row.ID = 215;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Carol";
            row.ID = 216;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Threnody";
            row.ID = 217;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Hymnus";
            row.ID = 218;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Mazurka";
            row.ID = 219;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sirvente";
            row.ID = 220;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Dirge";
            row.ID = 221;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Scherzo";
            row.ID = 222;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Nocturne";
            row.ID = 223;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "ST224";
            row.ID = 224;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "ST225";
            row.ID = 225;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "ST226";
            row.ID = 226;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Store TP";
            row.ID = 227;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Embrava";
            row.ID = 228;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Manawell";
            row.ID = 229;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Spontaneity";
            row.ID = 230;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Marcato";
            row.ID = 231;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "(N/A)";
            row.ID = 232;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Auto-Regen";
            row.ID = 233;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Auto-Refresh";
            row.ID = 234;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Fishing Imagery";
            row.ID = 235;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Woodworking Imagery";
            row.ID = 236;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Smithing Imagery";
            row.ID = 237;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Goldsmithing Imagery";
            row.ID = 238;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Clothcraft Imagery";
            row.ID = 239;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Leathercraft Imagery";
            row.ID = 240;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Bonecraft Imagery";
            row.ID = 241;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Alchemy Imagery";
            row.ID = 242;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Cooking Imagery";
            row.ID = 243;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Dedication";
            row.ID = 249;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "EF Badge";
            row.ID = 250;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Food";
            row.ID = 251;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Mounted";
            row.ID = 252;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Signet";
            row.ID = 253;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Battlefield";
            row.ID = 254;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sanction";
            row.ID = 256;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Besieged";
            row.ID = 257;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Illusion";
            row.ID = 258;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "encumbrance";
            row.ID = 259;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Obliviscence";
            row.ID = 260;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "impairment";
            row.ID = 261;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Omerta";
            row.ID = 262;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "debilitation";
            row.ID = 263;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Pathos";
            row.ID = 264;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Flurry";
            row.ID = 265;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Concentration";
            row.ID = 266;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Allied Tags";
            row.ID = 267;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sigil";
            row.ID = 268;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Level Sync";
            row.ID = 269;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Aftermath: Lv.1";
            row.ID = 270;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Aftermath: Lv.2";
            row.ID = 271;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Aftermath: Lv.3";
            row.ID = 272;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Aftermath";
            row.ID = 273;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enlight";
            row.ID = 274;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Auspice";
            row.ID = 275;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Confrontation";
            row.ID = 276;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enfire II";
            row.ID = 277;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enblizzard II";
            row.ID = 278;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enaero II";
            row.ID = 279;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enstone II";
            row.ID = 280;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enthunder II";
            row.ID = 281;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enwater II";
            row.ID = 282;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Perfect Defense";
            row.ID = 283;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Egg";
            row.ID = 284;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Visitant";
            row.ID = 285;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Baramnesia";
            row.ID = 286;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Atma";
            row.ID = 287;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Endark";
            row.ID = 288;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enmity Boost";
            row.ID = 289;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Subtle Blow Plus";
            row.ID = 290;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enmity Down";
            row.ID = 291;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Pennant";
            row.ID = 292;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Negate Petrify";
            row.ID = 293;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Negate Terror";
            row.ID = 294;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Negate Amnesia";
            row.ID = 295;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Negate Doom";
            row.ID = 296;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Negate Poison";
            row.ID = 297;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "critical hit evasion down";
            row.ID = 298;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Overload";
            row.ID = 299;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Fire Maneuver";
            row.ID = 300;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Ice Maneuver";
            row.ID = 301;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Wind Maneuver";
            row.ID = 302;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Earth Maneuver";
            row.ID = 303;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Thunder Maneuver";
            row.ID = 304;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Water Maneuver";
            row.ID = 305;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Light Maneuver";
            row.ID = 306;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Dark Maneuver";
            row.ID = 307;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Double-Up Chance";
            row.ID = 308;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Bust";
            row.ID = 309;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Fighter&apsts Roll";
            row.ID = 310;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Monk&apsts Roll";
            row.ID = 311;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Healer&apsts Roll";
            row.ID = 312;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Wizard&apsts Roll";
            row.ID = 313;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Warlock&apsts Roll";
            row.ID = 314;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Rogue&apsts Roll";
            row.ID = 315;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Gallant&apsts Roll";
            row.ID = 316;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Chaos Roll";
            row.ID = 317;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Beast Roll";
            row.ID = 318;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Choral Roll";
            row.ID = 319;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Hunter&apsts Roll";
            row.ID = 320;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Samurai Roll";
            row.ID = 321;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Ninja Roll";
            row.ID = 322;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Drachen Roll";
            row.ID = 323;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Evoker&apsts Roll";
            row.ID = 324;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magus&apsts Roll";
            row.ID = 325;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Corsair&apsts Roll";
            row.ID = 326;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Puppet Roll";
            row.ID = 327;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Dancer&apsts Roll";
            row.ID = 328;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Scholar&apsts Roll";
            row.ID = 329;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Bolter&apsts Roll";
            row.ID = 330;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Caster&apsts Roll";
            row.ID = 331;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Courser&apsts Roll";
            row.ID = 332;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Blitzer&apsts Roll";
            row.ID = 333;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Tactician&apsts Roll";
            row.ID = 334;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Allies&apst Roll";
            row.ID = 335;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Miser&apsts Roll";
            row.ID = 336;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Companion&apsts Roll";
            row.ID = 337;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Avenger&apsts Roll";
            row.ID = 338;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Naturalist&apsts Roll";
            row.ID = 339;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Warrior&apsts Charge";
            row.ID = 340;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Formless Strikes";
            row.ID = 341;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Assassin&apsts Charge";
            row.ID = 342;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Feint";
            row.ID = 343;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Fealty";
            row.ID = 344;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Dark Seal";
            row.ID = 345;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Diabolic Eye";
            row.ID = 346;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Nightingale";
            row.ID = 347;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Troubadour";
            row.ID = 348;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Killer Instinct";
            row.ID = 349;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Stealth Shot";
            row.ID = 350;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Flashy Shot";
            row.ID = 351;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sange";
            row.ID = 352;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Hasso";
            row.ID = 353;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Seigan";
            row.ID = 354;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Convergence";
            row.ID = 355;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Diffusion";
            row.ID = 356;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Snake Eye";
            row.ID = 357;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Light Arts";
            row.ID = 358;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Dark Arts";
            row.ID = 359;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Penury";
            row.ID = 360;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Parsimony";
            row.ID = 361;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Celerity";
            row.ID = 362;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Alacrity";
            row.ID = 363;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Rapture";
            row.ID = 364;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Ebullience";
            row.ID = 365;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Accession";
            row.ID = 366;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Manifestation";
            row.ID = 367;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Drain Samba";
            row.ID = 368;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Aspir Samba";
            row.ID = 369;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Haste Samba";
            row.ID = 370;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Velocity Shot";
            row.ID = 371;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Building Flourish";
            row.ID = 375;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Trance";
            row.ID = 376;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Tabula Rasa";
            row.ID = 377;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Drain Daze";
            row.ID = 378;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Aspir Daze";
            row.ID = 379;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Haste Daze";
            row.ID = 380;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Finishing Move 1";
            row.ID = 381;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Finishing Move 2";
            row.ID = 382;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Finishing Move 3";
            row.ID = 383;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Finishing Move 4";
            row.ID = 384;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Finishing Move 5";
            row.ID = 385;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Lethargic Daze 1";
            row.ID = 386;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Lethargic Daze 2";
            row.ID = 387;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Lethargic Daze 3";
            row.ID = 388;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Lethargic Daze 4";
            row.ID = 389;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Lethargic Daze 5";
            row.ID = 390;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sluggish Daze 1";
            row.ID = 391;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sluggish Daze 2";
            row.ID = 392;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sluggish Daze 3";
            row.ID = 393;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sluggish Daze 4";
            row.ID = 394;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sluggish Daze 5";
            row.ID = 395;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Weakened Daze 1";
            row.ID = 396;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Weakened Daze 2";
            row.ID = 397;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Weakened Daze 3";
            row.ID = 398;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Weakened Daze 4";
            row.ID = 399;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Weakened Daze 5";
            row.ID = 400;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Addendum: White";
            row.ID = 401;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Addendum: Black";
            row.ID = 402;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Reprisal";
            row.ID = 403;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Evasion Down";
            row.ID = 404;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Retaliation";
            row.ID = 405;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Footwork";
            row.ID = 406;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Klimaform";
            row.ID = 407;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sekkanoki";
            row.ID = 408;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Pianissimo";
            row.ID = 409;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Saber Dance";
            row.ID = 410;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Fan Dance";
            row.ID = 411;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Altruism";
            row.ID = 412;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Focalization";
            row.ID = 413;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Tranquility";
            row.ID = 414;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Equanimity";
            row.ID = 415;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enlightenment";
            row.ID = 416;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Afflatus Solace";
            row.ID = 417;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Afflatus Misery";
            row.ID = 418;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Composure";
            row.ID = 419;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Yonin";
            row.ID = 420;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Innin";
            row.ID = 421;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Carbuncle&apsts Favor";
            row.ID = 422;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Ifrit&apsts Favor";
            row.ID = 423;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Shiva&apsts Favor";
            row.ID = 424;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Garuda&apsts Favor";
            row.ID = 425;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Titan&apsts Favor";
            row.ID = 426;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Ramuh&apsts Favor";
            row.ID = 427;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Leviathan&apsts Favor";
            row.ID = 428;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Fenrir&apsts Favor";
            row.ID = 429;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Diabolos&apsts Favor";
            row.ID = 430;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Avatar&apsts Favor";
            row.ID = 431;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Multi Strikes";
            row.ID = 432;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Double Shot";
            row.ID = 433;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Transcendency";
            row.ID = 434;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Restraint";
            row.ID = 435;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Perfect Counter";
            row.ID = 436;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Mana Wall";
            row.ID = 437;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Divine Emblem";
            row.ID = 438;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Nether Void";
            row.ID = 439;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sengikori";
            row.ID = 440;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Futae";
            row.ID = 441;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Presto";
            row.ID = 442;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Climactic Flourish";
            row.ID = 443;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Copy Image (2)";
            row.ID = 444;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Copy Image (3)";
            row.ID = 445;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Copy Image (4+)";
            row.ID = 446;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Multi Shots";
            row.ID = 447;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Bewildered Daze 1";
            row.ID = 448;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Bewildered Daze 2";
            row.ID = 449;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Bewildered Daze 3";
            row.ID = 450;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Bewildered Daze 4";
            row.ID = 451;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Bewildered Daze 5";
            row.ID = 452;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Divine Caress";
            row.ID = 453;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Saboteur";
            row.ID = 454;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Tenuto";
            row.ID = 455;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Spur";
            row.ID = 456;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Efflux";
            row.ID = 457;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Earthen Armor";
            row.ID = 458;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Divine Caress";
            row.ID = 459;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Blood Rage";
            row.ID = 460;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Impetus";
            row.ID = 461;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Conspirator";
            row.ID = 462;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sepulcher";
            row.ID = 463;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Arcane Crest";
            row.ID = 464;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Hamanoha";
            row.ID = 465;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Dragon Breaker";
            row.ID = 466;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Triple Shot";
            row.ID = 467;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Striking Flourish";
            row.ID = 468;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Perpetuance";
            row.ID = 469;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Immanence";
            row.ID = 470;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Migawari";
            row.ID = 471;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Ternary Flourish";
            row.ID = 472;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "muddle";
            row.ID = 473;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Prowess";
            row.ID = 474;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Voidwatcher";
            row.ID = 475;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Ensphere";
            row.ID = 476;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sacrosanctity";
            row.ID = 477;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Palisade";
            row.ID = 478;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Scarlet Delirium";
            row.ID = 479;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Scarlet Delirium";
            row.ID = 480;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Abdhaljs Seal";
            row.ID = 481;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Decoy Shot";
            row.ID = 482;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Hagakure";
            row.ID = 483;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Issekigan";
            row.ID = 484;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Unbridled Learning";
            row.ID = 485;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Counter Boost";
            row.ID = 486;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Endrain";
            row.ID = 487;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Enaspir";
            row.ID = 488;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Afterglow";
            row.ID = 489;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Brazen Rush";
            row.ID = 490;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Inner Strength";
            row.ID = 491;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Asylum";
            row.ID = 492;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Subtle Sorcery";
            row.ID = 493;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Stymie";
            row.ID = 494;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Intervene";
            row.ID = 496;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Soul Enslavement";
            row.ID = 497;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Unleash";
            row.ID = 498;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Clarion Call";
            row.ID = 499;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Overkill";
            row.ID = 500;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Yaegasumi";
            row.ID = 501;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Mikage";
            row.ID = 502;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Fly High";
            row.ID = 503;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Astral Conduit";
            row.ID = 504;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Unbridled Wisdom";
            row.ID = 505;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Grand Pas";
            row.ID = 507;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Widened Compass";
            row.ID = 508;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Odyllic Subterfuge";
            row.ID = 509;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Ergon Might";
            row.ID = 510;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Reive Mark";
            row.ID = 511;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Ionis";
            row.ID = 512;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Bolster";
            row.ID = 513;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Lasting Emanation";
            row.ID = 515;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Ecliptic Attrition";
            row.ID = 516;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Collimated Fervor";
            row.ID = 517;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Dematerialize";
            row.ID = 518;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Theurgic Focus";
            row.ID = 519;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Elemental Sforzo";
            row.ID = 522;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Ignis";
            row.ID = 523;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Gelus";
            row.ID = 524;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Flabra";
            row.ID = 525;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Tellus";
            row.ID = 526;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sulpor";
            row.ID = 527;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Unda";
            row.ID = 528;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Lux";
            row.ID = 529;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Tenebrae";
            row.ID = 530;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Vallation";
            row.ID = 531;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Swordplay";
            row.ID = 532;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Pflug";
            row.ID = 533;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Embolden";
            row.ID = 534;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Valiance";
            row.ID = 535;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Gambit";
            row.ID = 536;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Liement";
            row.ID = 537;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "One for All";
            row.ID = 538;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Regen";
            row.ID = 539;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "poison";
            row.ID = 540;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Refresh";
            row.ID = 541;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "STR Boost";
            row.ID = 542;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "DEX Boost";
            row.ID = 543;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "VIT Boost";
            row.ID = 544;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "AGI Boost";
            row.ID = 545;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "INT Boost";
            row.ID = 546;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "MND Boost";
            row.ID = 547;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "CHR Boost";
            row.ID = 548;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Attack Boost";
            row.ID = 549;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Defense Boost";
            row.ID = 550;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Atk. Boost";
            row.ID = 551;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Def. Boost";
            row.ID = 552;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Accuracy Boost";
            row.ID = 553;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Evasion Boost";
            row.ID = 554;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Acc. Boost";
            row.ID = 555;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Evasion Boost";
            row.ID = 556;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Attack Down";
            row.ID = 557;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Defense Down";
            row.ID = 558;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Atk. Down";
            row.ID = 559;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Def. Down";
            row.ID = 560;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Accuracy Down";
            row.ID = 561;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Evasion Down";
            row.ID = 562;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Acc. Down";
            row.ID = 563;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Evasion Down";
            row.ID = 564;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "slow";
            row.ID = 565;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "paralysis";
            row.ID = 566;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "weight";
            row.ID = 567;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Foil";
            row.ID = 568;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Blaze of Glory";
            row.ID = 569;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Battuta";
            row.ID = 570;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Rayke";
            row.ID = 571;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Avoidance Down";
            row.ID = 572;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Deluge Spikes";
            row.ID = 573;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Fast Cast";
            row.ID = 574;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "gestation";
            row.ID = 575;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Doubt";
            row.ID = 576;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Cait Sith&apsts Favor";
            row.ID = 577;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Fishy Intuition";
            row.ID = 578;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Commitment";
            row.ID = 579;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Haste";
            row.ID = 580;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Flurry";
            row.ID = 581;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Contradance";
            row.ID = 582;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Apogee";
            row.ID = 583;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Entrust";
            row.ID = 584;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Costume";
            row.ID = 585;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Curing Conduit";
            row.ID = 586;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "TP Bonus";
            row.ID = 587;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Finishing Move (6+)";
            row.ID = 588;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Firestorm";
            row.ID = 589;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Hailstorm";
            row.ID = 590;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Windstorm";
            row.ID = 591;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Sandstorm";
            row.ID = 592;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Thunderstorm";
            row.ID = 593;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Rainstorm";
            row.ID = 594;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Aurorastorm";
            row.ID = 595;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Voidstorm";
            row.ID = 596;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Inundation";
            row.ID = 597;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Cascade";
            row.ID = 598;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Consume Mana";
            row.ID = 599;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Runeist&apsts Roll";
            row.ID = 600;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Crooked Cards";
            row.ID = 601;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Vorseal";
            row.ID = 602;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Elvorseal";
            row.ID = 603;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Mighty Guard";
            row.ID = 604;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Gale Spikes";
            row.ID = 605;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Clod Spikes";
            row.ID = 606;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Glint Spikes";
            row.ID = 607;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Negate Virus";
            row.ID = 608;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Negate Curse";
            row.ID = 609;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Negate Charm";
            row.ID = 610;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Magic Evasion Boost";
            row.ID = 611;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();
            row.Name = "Colure Active";
            row.ID = 612;
            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);

            FfxiResource.mainDb.StatusEffects.AcceptChanges();
        }
    }
}
