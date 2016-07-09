using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Jobs
    {
        private static void loadData()
        {
            MainDatabase.JobsRow row;

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "None";
            row.ID = 0;
            row.Abbreviation = "NON";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Warrior";
            row.ID = 1;
            row.Abbreviation = "WAR";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Monk";
            row.ID = 2;
            row.Abbreviation = "MNK";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "White Mage";
            row.ID = 3;
            row.Abbreviation = "WHM";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Black Mage";
            row.ID = 4;
            row.Abbreviation = "BLM";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Red Mage";
            row.ID = 5;
            row.Abbreviation = "RDM";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Thief";
            row.ID = 6;
            row.Abbreviation = "THF";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Paladin";
            row.ID = 7;
            row.Abbreviation = "PLD";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Dark Knight";
            row.ID = 8;
            row.Abbreviation = "DRK";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Beastmaster";
            row.ID = 9;
            row.Abbreviation = "BST";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Bard";
            row.ID = 10;
            row.Abbreviation = "BRD";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Ranger";
            row.ID = 11;
            row.Abbreviation = "RNG";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Samurai";
            row.ID = 12;
            row.Abbreviation = "SAM";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Ninja";
            row.ID = 13;
            row.Abbreviation = "NIN";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Dragoon";
            row.ID = 14;
            row.Abbreviation = "DRG";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Summoner";
            row.ID = 15;
            row.Abbreviation = "SMN";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Blue Mage";
            row.ID = 16;
            row.Abbreviation = "BLU";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Corsair";
            row.ID = 17;
            row.Abbreviation = "COR";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Puppetmaster";
            row.ID = 18;
            row.Abbreviation = "PUP";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Dancer";
            row.ID = 19;
            row.Abbreviation = "DNC";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Scholar";
            row.ID = 20;
            row.Abbreviation = "SCH";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Geomancer";
            row.ID = 21;
            row.Abbreviation = "GEO";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Rune Fencer";
            row.ID = 22;
            row.Abbreviation = "RUN";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            row = FfxiResource.mainDb.Jobs.NewJobsRow();
            row.Name = "Monipulator";
            row.ID = 23;
            row.Abbreviation = "MON";
            FfxiResource.mainDb.Jobs.AddJobsRow(row);

            FfxiResource.mainDb.Jobs.AcceptChanges();
        }
    }
}
