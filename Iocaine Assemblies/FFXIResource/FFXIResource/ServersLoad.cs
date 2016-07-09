using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Servers
    {
        private static void loadData()
        {
            MainDatabase.ServersRow row;

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Bahamut";
            row.ServerID = 100;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Shiva";
            row.ServerID = 101;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Titan";
            row.ServerID = 102;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Ramuh";
            row.ServerID = 103;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Phoenix";
            row.ServerID = 104;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Carbuncle";
            row.ServerID = 105;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Fenrir";
            row.ServerID = 106;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Sylph";
            row.ServerID = 107;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Valefor";
            row.ServerID = 108;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Alexander";
            row.ServerID = 109;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Leviathan";
            row.ServerID = 110;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Odin";
            row.ServerID = 111;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Ifrit";
            row.ServerID = 112;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Diabolos";
            row.ServerID = 113;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Caitsith";
            row.ServerID = 114;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Quetzalcoatl";
            row.ServerID = 115;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Siren";
            row.ServerID = 116;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Unicorn";
            row.ServerID = 117;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Gilgamesh";
            row.ServerID = 118;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Ragnarok";
            row.ServerID = 119;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Pandemonium";
            row.ServerID = 120;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Garuda";
            row.ServerID = 121;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Cerberus";
            row.ServerID = 122;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Kujata";
            row.ServerID = 123;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Bismarck";
            row.ServerID = 124;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Seraph";
            row.ServerID = 125;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Lakshmi";
            row.ServerID = 126;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Asura";
            row.ServerID = 127;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Midgardsormr";
            row.ServerID = 128;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Fairy";
            row.ServerID = 129;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Remora";
            row.ServerID = 130;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            row = FfxiResource.mainDb.Servers.NewServersRow();
            row.ServerName = "Hades";
            row.ServerID = 131;
            FfxiResource.mainDb.Servers.AddServersRow(row);

            FfxiResource.mainDb.Servers.AcceptChanges();
        }
    }
}
