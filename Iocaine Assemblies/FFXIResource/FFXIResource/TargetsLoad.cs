using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Targets
    {
        private static void loadData()
        {
            MainDatabase.TargetsRow row;

            row = FfxiResource.mainDb.Targets.NewTargetsRow();
            row.ID = 1;
            row.Self = true;
            row.Party = false;
            row.Alliance = false;
            row.Anyone = false;
            row.Monster = false;
            row.Trust = false;
            row.Fellow = false;
            row.Dead = false;
            FfxiResource.mainDb.Targets.AddTargetsRow(row);

            row = FfxiResource.mainDb.Targets.NewTargetsRow();
            row.ID = 5;
            row.Self = true;
            row.Party = true;
            row.Alliance = true;
            row.Anyone = false;
            row.Monster = false;
            row.Trust = false;
            row.Fellow = false;
            row.Dead = false;
            FfxiResource.mainDb.Targets.AddTargetsRow(row);

            row = FfxiResource.mainDb.Targets.NewTargetsRow();
            row.ID = 29;
            row.Self = true;
            row.Party = true;
            row.Alliance = true;
            row.Anyone = true;
            row.Monster = false;
            row.Trust = false;
            row.Fellow = true;
            row.Dead = false;
            FfxiResource.mainDb.Targets.AddTargetsRow(row);

            row = FfxiResource.mainDb.Targets.NewTargetsRow();
            row.ID = 32;
            row.Self = false;
            row.Party = false;
            row.Alliance = false;
            row.Anyone = false;
            row.Monster = true;
            row.Trust = false;
            row.Fellow = false;
            row.Dead = false;
            FfxiResource.mainDb.Targets.AddTargetsRow(row);

            row = FfxiResource.mainDb.Targets.NewTargetsRow();
            row.ID = 63;
            row.Self = true;
            row.Party = true;
            row.Alliance = true;
            row.Anyone = true;
            row.Monster = true;
            row.Trust = true;
            row.Fellow = true;
            row.Dead = false;
            FfxiResource.mainDb.Targets.AddTargetsRow(row);

            row = FfxiResource.mainDb.Targets.NewTargetsRow();
            row.ID = 157;
            row.Self = false;
            row.Party = true;
            row.Alliance = true;
            row.Anyone = true;
            row.Monster = false;
            row.Trust = false;
            row.Fellow = false;
            row.Dead = true;
            FfxiResource.mainDb.Targets.AddTargetsRow(row);

            FfxiResource.mainDb.Targets.AcceptChanges();
        }
    }
}
