using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Elements
    {
        private static void loadData()
        {
            MainDatabase.ElementsRow row;

            row = FfxiResource.mainDb.Elements.NewElementsRow();
            row.Name = "Physical";
            row.ID = -1;
            FfxiResource.mainDb.Elements.AddElementsRow(row);

            row = FfxiResource.mainDb.Elements.NewElementsRow();
            row.Name = "Fire";
            row.ID = 0;
            FfxiResource.mainDb.Elements.AddElementsRow(row);

            row = FfxiResource.mainDb.Elements.NewElementsRow();
            row.Name = "Ice";
            row.ID = 1;
            FfxiResource.mainDb.Elements.AddElementsRow(row);

            row = FfxiResource.mainDb.Elements.NewElementsRow();
            row.Name = "Wind";
            row.ID = 2;
            FfxiResource.mainDb.Elements.AddElementsRow(row);

            row = FfxiResource.mainDb.Elements.NewElementsRow();
            row.Name = "Earth";
            row.ID = 3;
            FfxiResource.mainDb.Elements.AddElementsRow(row);

            row = FfxiResource.mainDb.Elements.NewElementsRow();
            row.Name = "Lightning";
            row.ID = 4;
            FfxiResource.mainDb.Elements.AddElementsRow(row);

            row = FfxiResource.mainDb.Elements.NewElementsRow();
            row.Name = "Water";
            row.ID = 5;
            FfxiResource.mainDb.Elements.AddElementsRow(row);

            row = FfxiResource.mainDb.Elements.NewElementsRow();
            row.Name = "Light";
            row.ID = 6;
            FfxiResource.mainDb.Elements.AddElementsRow(row);

            row = FfxiResource.mainDb.Elements.NewElementsRow();
            row.Name = "Dark";
            row.ID = 7;
            FfxiResource.mainDb.Elements.AddElementsRow(row);

            row = FfxiResource.mainDb.Elements.NewElementsRow();
            row.Name = "None";
            row.ID = 15;
            FfxiResource.mainDb.Elements.AddElementsRow(row);

            FfxiResource.mainDb.Elements.AcceptChanges();
        }
    }
}
