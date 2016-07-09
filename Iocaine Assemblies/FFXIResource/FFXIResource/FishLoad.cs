using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Fish
    {
        private static void loadData()
        {
            MainDatabase.FishRow row;

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "1 gil";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 1;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "100 gil";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 100;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "abaia";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5476;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "ahtapot";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5455;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "alabaligi";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5461;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "ancient carp";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6373;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "apkallufa";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5534;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "armored pisces";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4316;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "arrowwood log";
            row.Large = true;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 688;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "aurora bass";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5818;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Barnacle";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5954;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Bastore bream";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4461;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "bastore sardine";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4360;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "bastore sardines 2";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 11;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "bastore sardines 3";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 8;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Bastore sweeper";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5473;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Betta";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5139;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Bhefhel marlin";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4479;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Bibiki slug";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5122;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Bibiki urchin";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4318;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Bibikibo";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4314;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "black bubble-eye";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4311;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "black eel";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4429;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "black ghost";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5138;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Black Prawn";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5948;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "black sole";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4384;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "bladefish";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4471;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "blindfish";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4313;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Bloodblotch";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5951;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Blowfish";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5812;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "bluetail";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4399;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "bone chip";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 880;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Bonefish";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6336;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "brass loach";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5469;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "bugbear mask";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 1624;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "ca cuong";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5474;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Caedarva frog";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5465;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "calico comet";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5715;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "cameroceras";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6338;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "cave cherax";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4309;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Cheval salmon";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4379;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Clotflagration";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6001;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "clump of Adoulinian kelp";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 3965;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "clump of Pamtam kelp";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 624;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "cobalt jellyfish";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4443;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "cone calamary";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5128;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "cone calamary 2";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 13;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "cone calamary 3";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 14;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Contortacle";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5962;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Contortopus";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5961;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "copper frog";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4515;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "copper ring";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 13454;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "coral butterfly";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4580;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "coral fragment";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 887;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "crayfish";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4472;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "crescent fish";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4473;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Crocodilos";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5814;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "crystal bass";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4528;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "damp scroll";
            row.Large = false;
            row.Type = 1;
            row.DropType = 1;
            row.ItemID = 1210;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "dark bass";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4428;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Deademoiselle";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5535;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "denizanasi";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5447;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "devil manta";
            row.Large = true;
            row.Type = 2;
            row.DropType = 0;
            row.ItemID = 20;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "dil";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5457;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Dorado Gar";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5813;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "dragoneye";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4048;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Dragonfish";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5959;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "dragons tabernacle";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6374;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Duskcrawler";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 9077;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "dwarf remora";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6145;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Elshimo frog";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4290;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Elshimo newt";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4579;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "emperor fish";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4454;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "fat greedie";
            row.Large = false;
            row.Type = 0;
            row.DropType = 1;
            row.ItemID = 4501;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "fish scale shield";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 12316;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "forest carp";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4289;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Frigorifish";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6144;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "garpike";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5472;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "gavial fish";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4477;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "generic item";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 2;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "gerrothorax";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5471;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "giant catfish";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4469;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "giant chirai";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4308;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "giant donko";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4306;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "gigant octopus";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5475;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "gigant squid";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4474;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "gold carp";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4427;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "gold lobster";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4383;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "gold ring";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 13445;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "greedie";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4500;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "grimmonite";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4304;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Gugru tuna";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4480;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Gugrusaurus";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5127;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "gurnard";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5132;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "hakuryu";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5539;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "hamsi";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5449;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "hamsi 2";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 17;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "hamsi 3";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 18;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "icefish";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4470;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "icefish 2";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "icefish 3";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "istakoz";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5453;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "istavrit";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5136;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "istiridye";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5456;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "jacknife";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5123;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "jungle catfish";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4307;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "kalamar";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5448;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "kalamar 2";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 15;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "kalamar 3";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 16;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "kalkanbaligi";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5140;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "kaplumbaga";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5464;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "kayabaligi";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5460;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "kilicbaligi";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5451;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "King Perch";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5816;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Kokuryu";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5540;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "lakerda";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5450;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "lamp marimo";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 2216;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Lik";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5129;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "lionhead";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4312;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "lord of Ulbuka";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6372;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "lungfish";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4315;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Mackerel";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5950;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Malicious Perch";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5995;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "matsya";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5468;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "megalodon";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5467;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "mercanbaligi";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5454;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "moat carp";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4401;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Moblin mask";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 1638;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "mola mola";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5134;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "monke-onke";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4462;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Monster";
            row.Large = true;
            row.Type = 2;
            row.DropType = 0;
            row.ItemID = 3;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "moorish idol";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5121;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "morinabaligi";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5462;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "muddy siredon";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5266;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Mussel";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5949;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "mythril dagger";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 16451;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Nahn";
            row.Large = true;
            row.Type = 2;
            row.DropType = 0;
            row.ItemID = 21;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "nebimonite";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4459;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "noble lady";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4485;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Norg shell";
            row.Large = false;
            row.Type = 1;
            row.DropType = 1;
            row.ItemID = 1135;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Nosteau herring";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4482;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "ogre eel";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4481;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "pair of rusty leggings";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 14117;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "pearlscale";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5714;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Pelazoea";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5815;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Phanauet newt";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5125;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "phantom serpent";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6375;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "pipira";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4464;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "pirarucu";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5470;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "pterygotus";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5133;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "quicksilver blade";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6371;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "quus";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4514;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "quus 2";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "quus 3";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 12;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "RaKaznar Shellfish";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6334;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "red bubble-eye";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5446;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "red terrapin";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4402;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "remora";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6146;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "rhinochimera";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5135;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "ripped cap";
            row.Large = false;
            row.Type = 1;
            row.DropType = 1;
            row.ItemID = 591;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Ruddy Seema";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5952;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "rusty bucket";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 90;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "rusty cap";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 12522;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "rusty greatsword";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 16606;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Rusty Kunai";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 19283;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "rusty pick";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 16655;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Rusty Spear";
            row.Large = true;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 19308;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "rusty subligar";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 14242;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "rusty zaghnal";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 18962;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "ryugu titan";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4305;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "sandfish";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4291;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "sandfish 2";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 10;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "sandfish 3";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 9;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "sazanbaligi";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5459;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "sea zombie";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4475;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Sekiryu";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5538;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Senroh frog";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5993;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Senroh Sardine";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5963;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Senroh sardines 2";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 22;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Senroh sardines 3";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 23;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "shall shell";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4484;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Shen";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5997;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "shining trout";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4354;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "silver ring";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 13456;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "silver shark";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4451;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Soryu";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5537;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "takitaro";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4463;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Tavnazian goby";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5130;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "three-eyed fish";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4478;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Thysanopeltis";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6337;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "tiger cod";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4483;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "tiger shark";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5817;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "tiny goldfish";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4310;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "titanic sawfish";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5120;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "titanictus";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4476;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Translucent Salpa";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6333;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "tricolored carp";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4426;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "tricorn";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4319;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "trilobite";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4317;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "tropical clam";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5124;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "trumpet shell";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5466;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "turnabaligi";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5137;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "tusoteuthis longa";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6376;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "ulbuconut";
            row.Large = false;
            row.Type = 1;
            row.DropType = 0;
            row.ItemID = 5966;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Ulbukan Lobster";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5960;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "unknown";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 0;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "uskumru";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5452;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Veydal wrasse";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5141;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "vongola clam";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5131;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "White Lobster";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 6335;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Yawning Catfish";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5955;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "yayinbaligi";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5463;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "yellow globe";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4403;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "yellow globes 2";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 19;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "yellow globes 3";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 7;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "yilanbaligi";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5458;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "yorchete";
            row.Large = true;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 5536;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "Zafmlug bass";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4385;
            FfxiResource.mainDb.Fish.AddFishRow(row);

            row = FfxiResource.mainDb.Fish.NewFishRow();
            row.FishName = "zebra eel";
            row.Large = false;
            row.Type = 0;
            row.DropType = 0;
            row.ItemID = 4288;
            FfxiResource.mainDb.Fish.AddFishRow(row);
        }
    }
}
