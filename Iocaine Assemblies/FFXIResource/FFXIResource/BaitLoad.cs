using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Bait
    {
        private static void loadData()
        {
            MainDatabase.BaitRow row;

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Crayfish Ball";
            row.BaitNameShort = "Cray. Ball";
            row.ItemID = 16997;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Drill Calamary";
            row.BaitNameShort = "Drl. Calamary";
            row.ItemID = 17006;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Dwarf Pugil";
            row.BaitNameShort = "Dwarf Pugil";
            row.ItemID = 17007;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Fly Lure";
            row.BaitNameShort = "Fly Lure";
            row.ItemID = 17405;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Frog Lure";
            row.BaitNameShort = "Frog Lure";
            row.ItemID = 17403;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Giant Shell Bug";
            row.BaitNameShort = "G. Shell Bug";
            row.ItemID = 17001;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Insect Ball";
            row.BaitNameShort = "Insect Ball";
            row.ItemID = 16998;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Large MMM Ball";
            row.BaitNameShort = "L MMM Ball";
            row.ItemID = 17009;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Little Worm";
            row.BaitNameShort = "Ltl. Worm";
            row.ItemID = 17396;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Lizard Lure";
            row.BaitNameShort = "Liz. Lure";
            row.ItemID = 17401;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Lufaise Fly";
            row.BaitNameShort = "Lufaise Fly";
            row.ItemID = 17005;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Lugworm";
            row.BaitNameShort = "Lugworm";
            row.ItemID = 17395;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Meatball";
            row.BaitNameShort = "Meatball";
            row.ItemID = 17000;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Minnow";
            row.BaitNameShort = "Minnow";
            row.ItemID = 17407;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Peeled Crayfish";
            row.BaitNameShort = "Pld Crayfish";
            row.ItemID = 16993;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Peeled Lobster";
            row.BaitNameShort = "Pld. Lobster";
            row.ItemID = 17394;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Reg. MMM Ball";
            row.BaitNameShort = "R MMM Ball";
            row.ItemID = 17008;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Robber Rig";
            row.BaitNameShort = "Robber Rig";
            row.ItemID = 17002;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Rogue Rig";
            row.BaitNameShort = "Rogue Rig";
            row.ItemID = 17398;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Rotten Meat";
            row.BaitNameShort = "Rotten Meat";
            row.ItemID = 16995;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Sabiki Rig";
            row.BaitNameShort = "Sabiki Rig";
            row.ItemID = 17399;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Sardine Ball";
            row.BaitNameShort = "Srdn. Ball";
            row.ItemID = 16996;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Sea Dragon Liver";
            row.BaitNameShort = "Sea Drg. Lv.";
            row.ItemID = 19326;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Shell Bug";
            row.BaitNameShort = "Shell Bug";
            row.ItemID = 17397;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Shrimp Lure";
            row.BaitNameShort = "Shr. Lure";
            row.ItemID = 17402;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Sinking Minnow";
            row.BaitNameShort = "Snk. Mnw.";
            row.ItemID = 17400;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Slice of Bluetail";
            row.BaitNameShort = "Slc. Bluetail";
            row.ItemID = 16992;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Slice of Carp";
            row.BaitNameShort = "Slc. Carp";
            row.ItemID = 16994;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Sliced Cod";
            row.BaitNameShort = "Slc. Cod";
            row.ItemID = 17393;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Sliced Sardine";
            row.BaitNameShort = "Slc. Sardine";
            row.ItemID = 17392;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Trout Ball";
            row.BaitNameShort = "Trout Ball";
            row.ItemID = 16999;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            row = FfxiResource.mainDb.Bait.NewBaitRow();
            row.BaitName = "Worm Lure";
            row.BaitNameShort = "Worm Lure";
            row.ItemID = 17404;
            FfxiResource.mainDb.Bait.AddBaitRow(row);

            FfxiResource.mainDb.Bait.AcceptChanges();
        }
    }
}
