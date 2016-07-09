using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Rods
    {
        private static void loadData()
        {
            MainDatabase.RodsRow row;

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Bamboo Fishing Rod";
            row.ItemID = 17389;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Carbon Fishing Rod";
            row.ItemID = 17384;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Clothespole";
            row.ItemID = 17383;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Composite Fishing Rod";
            row.ItemID = 17381;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Ebisu F. Rod +1";
            row.ItemID = 19321;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Ebisu Fishing Rod";
            row.ItemID = 17011;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Fastwater Fishing Rod";
            row.ItemID = 17388;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Glass Fiber Fishing Rod";
            row.ItemID = 17385;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Halcyon Rod";
            row.ItemID = 17015;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Hume Fishing Rod";
            row.ItemID = 17014;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Lu Sh. F. Rod +1";
            row.ItemID = 19320;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Lu Shangs Fishing Rod";
            row.ItemID = 17386;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Mithran Fishing Rod";
            row.ItemID = 17380;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Single Hook Fishing Rod";
            row.ItemID = 17382;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Tarutaru Fishing Rod";
            row.ItemID = 17387;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "unknown";
            row.ItemID = 0;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Willow Fishing Rod";
            row.ItemID = 17391;
            FfxiResource.mainDb.Rods.AddRodsRow(row);

            row = FfxiResource.mainDb.Rods.NewRodsRow();
            row.RodName = "Yew Fishing Rod";
            row.ItemID = 17390;
            FfxiResource.mainDb.Rods.AddRodsRow(row);
        }
    }
}
