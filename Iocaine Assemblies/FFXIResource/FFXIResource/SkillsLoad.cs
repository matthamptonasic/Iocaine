using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Skills
    {
        private static void loadData()
        {
            MainDatabase.SkillsRow row;

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "(N/A)";
            row.ID = 0;
            row.Category = "None";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Hand-to-Hand";
            row.ID = 1;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Dagger";
            row.ID = 2;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Sword";
            row.ID = 3;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Great Sword";
            row.ID = 4;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Axe";
            row.ID = 5;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Great Axe";
            row.ID = 6;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Scythe";
            row.ID = 7;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Polearm";
            row.ID = 8;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Katana";
            row.ID = 9;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Great Katana";
            row.ID = 10;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Club";
            row.ID = 11;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Staff";
            row.ID = 12;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Automaton Melee";
            row.ID = 22;
            row.Category = "Puppet";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Automaton Archery";
            row.ID = 23;
            row.Category = "Puppet";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Automaton Magic";
            row.ID = 24;
            row.Category = "Puppet";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Archery";
            row.ID = 25;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Marksmanship";
            row.ID = 26;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Throwing";
            row.ID = 27;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Guard";
            row.ID = 28;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Evasion";
            row.ID = 29;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Shield";
            row.ID = 30;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Parrying";
            row.ID = 31;
            row.Category = "Combat";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Divine Magic";
            row.ID = 32;
            row.Category = "Magic";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Healing Magic";
            row.ID = 33;
            row.Category = "Magic";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Enhancing Magic";
            row.ID = 34;
            row.Category = "Magic";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Enfeebling Magic";
            row.ID = 35;
            row.Category = "Magic";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Elemental Magic";
            row.ID = 36;
            row.Category = "Magic";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Dark Magic";
            row.ID = 37;
            row.Category = "Magic";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Summoning Magic";
            row.ID = 38;
            row.Category = "Magic";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Ninjutsu";
            row.ID = 39;
            row.Category = "Magic";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Singing";
            row.ID = 40;
            row.Category = "Magic";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Stringed Instrument";
            row.ID = 41;
            row.Category = "Magic";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Wind Instrument";
            row.ID = 42;
            row.Category = "Magic";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Blue Magic";
            row.ID = 43;
            row.Category = "Magic";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Geomancy";
            row.ID = 44;
            row.Category = "Magic";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Handbell";
            row.ID = 45;
            row.Category = "Magic";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Fishing";
            row.ID = 48;
            row.Category = "Synthesis";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Woodworking";
            row.ID = 49;
            row.Category = "Synthesis";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Smithing";
            row.ID = 50;
            row.Category = "Synthesis";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Goldsmithing";
            row.ID = 51;
            row.Category = "Synthesis";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Clothcraft";
            row.ID = 52;
            row.Category = "Synthesis";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Leathercraft";
            row.ID = 53;
            row.Category = "Synthesis";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Bonecraft";
            row.ID = 54;
            row.Category = "Synthesis";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Alchemy";
            row.ID = 55;
            row.Category = "Synthesis";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Cooking";
            row.ID = 56;
            row.Category = "Synthesis";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            row = FfxiResource.mainDb.Skills.NewSkillsRow();
            row.Name = "Synergy";
            row.ID = 57;
            row.Category = "Synthesis";
            FfxiResource.mainDb.Skills.AddSkillsRow(row);

            FfxiResource.mainDb.Skills.AcceptChanges();
        }
    }
}
