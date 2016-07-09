using Iocaine2.Data.Client;

namespace ResourceParser
{
    public partial class ParserForm
    {
        private void loadTargets()
        {
            //Cure:     63          0_0110_0011
            //Protect   29  haste   0_0010_1001
            //Refresh   5   erase   0_0000_0101
            //Baraero   1           0_0000_0001
            //Paralyze  32          0_0011_0010
            //Raise     157         1_0101_0111
            // *note* can party buff fellow npc.
            //Bit 0 = self
            //Bit 2 = PC inside party/alliance (refresh/erase)
            //Bit 1 = mob (can cure mob, can paralyze mob)
            //Bit 3 = PC outside + inside of party/alliance?
            //Bit 5 = any PC?
            //Bit 6 = Trust?
            //Bit 8 = only on dead people
            MainDatabase.TargetsRow row = mainDB.Targets.NewTargetsRow();
            row.ID = 1;
            row.Self = true;
            row.Party = false;
            row.Alliance = false;
            row.Anyone = false;
            row.Monster = false;
            row.Trust = false;
            row.Fellow = false;
            row.Dead = false;
            mainDB.Targets.AddTargetsRow(row);

            row = mainDB.Targets.NewTargetsRow();
            row.ID = 5;
            row.Self = true;
            row.Party = true;
            row.Alliance = true;
            row.Anyone = false;
            row.Monster = false;
            row.Trust = false;
            row.Fellow = false;
            row.Dead = false;
            mainDB.Targets.AddTargetsRow(row);

            row = mainDB.Targets.NewTargetsRow();
            row.ID = 29;
            row.Self = true;
            row.Party = true;
            row.Alliance = true;
            row.Anyone = true;
            row.Monster = false;
            row.Trust = false;
            row.Fellow = true;
            row.Dead = false;
            mainDB.Targets.AddTargetsRow(row);

            row = mainDB.Targets.NewTargetsRow();
            row.ID = 32;
            row.Self = false;
            row.Party = false;
            row.Alliance = false;
            row.Anyone = false;
            row.Monster = true;
            row.Trust = false;
            row.Fellow = false;
            row.Dead = false;
            mainDB.Targets.AddTargetsRow(row);

            row = mainDB.Targets.NewTargetsRow();
            row.ID = 63;
            row.Self = true;
            row.Party = true;
            row.Alliance = true;
            row.Anyone = true;
            row.Monster = true;
            row.Trust = true;
            row.Fellow = true;
            row.Dead = false;
            mainDB.Targets.AddTargetsRow(row);

            row = mainDB.Targets.NewTargetsRow();
            row.ID = 157;
            row.Self = false;
            row.Party = true;
            row.Alliance = true;
            row.Anyone = true;
            row.Monster = false;
            row.Trust = false;
            row.Fellow = false;
            row.Dead = true;
            mainDB.Targets.AddTargetsRow(row);
        }
    }
}