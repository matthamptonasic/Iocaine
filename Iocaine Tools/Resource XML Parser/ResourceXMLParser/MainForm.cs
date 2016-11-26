using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ResourceXMLParser
{
    public partial class MainForm : Form
    {
        #region Form Initialization
        public MainForm()
        {
            InitializeComponent();
            StreamWriter logFile = null;
            logFile = new StreamWriter("Iocaine Log.txt");
            logFile.AutoFlush = true;
            Console.SetOut(logFile);
            Console.SetError(logFile);
        }
        #endregion Form Initialization
        #region Enums
        private enum FIELD
        {
            ITEM_OPEN = 0,
            ICON_OPEN = 2,
            DESC_OPEN = 29,
            DESC_CLOSE = 30,
            THING_CLOSE = 3,
            ITEM_ID = 4,
            ITEM_NAME = 5,
            ITEM_FLAGS = 6,
            ITEM_STACK_SIZE = 7,
            ITEM_TYPE = 8,
            ITEM_RESOURCE_ID = 9,
            ITEM_VALID_TARGETS = 10,
            ITEM_LOG_NAME_S = 11,
            ITEM_LOG_NAME_P = 12,
            ITEM_LEVEL = 13,
            ITEM_SLOTS = 14,
            ITEM_RACES = 15,
            ITEM_JOBS = 16,
            ITEM_IMAGE = 17,
            ITEM_SHIELD_SIZE = 18,
            ITEM_CAST_TIME = 19,
            ITEM_USE_DELAY = 20,
            ITEM_REUSE_DELAY = 21,
            ITEM_ELEMENT = 22,
            ITEM_STORAGE_SLOTS = 23,
            ITEM_DAMAGE = 24,
            ITEM_DELAY = 25,
            ITEM_DPS = 26,
            ITEM_SKILL = 27,
            ITEM_JUG_SIZE = 28,
            NONE = 255
        }
        #endregion Enums
        #region Member Objects/Variables
        private ushort maxItemID = 0;
        //All fields:
        //1. id - ushort (DONE)
        //2. name - string (DONE)
        //3. flags - ushort (DONE)
        //4. stack-size - byte (DONE)
        //5. type - ushort (DONE)
        //6. resource-id - uint (DONE)
        //7. valid-targets - ushort (DONE)
        //8. description - string (DONE)
        //9. log-name-singular - string (DONE)
        //10. log-name-plural - string (DONE)
        //11. level - byte (DONE)
        //12. slots - ushort (DONE)
        //13. races - ushort (DONE)
        //14. jobs - uint (DONE)
        //15. shield-size - byte (DONE)
        //16. casting-time - ushort (DONE)
        //17. use-delay - ushort (DONE)
        //18. reuse-delay - ushort (DONE)
        //19. icon - string (DONE)
        //20. element - byte (DONE)
        //21. storage-slots - byte (DONE)
        //22. damage - ushort (DONE)
        //23. delay - ushort (DONE)
        //24. dps - ushort (DONE)
        //25. skill - byte (DONE)
        //26. jug-size - ushort (DONE)
        //Initial lists created while finding max item ID
        private List<ushort> itemIdList = new List<ushort>();
        private List<string> itemNameList = new List<string>();
        private List<ushort> itemFlagsList = new List<ushort>();
        private List<byte> itemStackSizeList = new List<byte>();
        private List<ushort> itemTypeList = new List<ushort>();
        private List<uint> itemResourceIdList = new List<uint>();
        private List<ushort> itemValidTargetsList = new List<ushort>();
        private List<string> itemLogNameSList = new List<string>();
        private List<string> itemLogNamePList = new List<string>();
        private List<byte> itemLevelList = new List<byte>();
        private List<ushort> itemSlotsList = new List<ushort>();
        private List<ushort> itemRacesList = new List<ushort>();
        private List<uint> itemJobsList = new List<uint>();
        private List<string> itemImageList = new List<string>();
        private List<byte> itemShieldSizeList = new List<byte>();
        private List<ushort> itemCastTimeList = new List<ushort>();
        private List<ushort> itemUseDelayList = new List<ushort>();
        private List<ushort> itemReuseDelayList = new List<ushort>();
        private List<byte> itemElementList = new List<byte>();
        private List<byte> itemStorageSlotsList = new List<byte>();
        private List<ushort> itemDamageList = new List<ushort>();
        private List<ushort> itemDelayList = new List<ushort>();
        private List<ushort> itemDpsList = new List<ushort>();
        private List<byte> itemSkillList = new List<byte>();
        private List<ushort> itemJugSizeList = new List<ushort>();
        private List<string> itemDescriptionList = new List<string>();
        //Final arrays that list all info, indexed by the item ID with all 0xF or "" string
        //inserted where no item ID was found.
        private string[] itemNameArray;
        private ushort[] itemFlagsArray;
        private byte[] itemStackSizeArray;
        private ushort[] itemTypeArray;
        private uint[] itemResourceIdArray;
        private ushort[] itemValidTargetsArray;
        private string[] itemLogNameSArray;
        private string[] itemLogNamePArray;
        private byte[] itemLevelArray;
        private ushort[] itemSlotsArray;
        private ushort[] itemRacesArray;
        private uint[] itemJobsArray;
        private string[] itemImageArray;
        private byte[] itemShieldSizeArray;
        private ushort[] itemCastTimeArray;
        private ushort[] itemUseDelayArray;
        private ushort[] itemReuseDelayArray;
        private byte[] itemElementArray;
        private byte[] itemStorageSlotsArray;
        private ushort[] itemDamageArray;
        private ushort[] itemDelayArray;
        private ushort[] itemDpsArray;
        private byte[] itemSkillArray;
        private ushort[] itemJugSizeArray;
        private string[] itemDescriptionArray;
        #endregion Member Objects/Variables
        #region Event Handlers
        private void AddFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog.ShowDialog();
        }
        private void ProcessButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog.ShowDialog();
        }
        private void OpenFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            String [] files = OpenFileDialog.FileNames;
            foreach (String str in files)
            {
                FileSearchListBox.Items.Add(str);
            }
        }
        private void SaveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            string outputFileName = SaveFileDialog.FileName;
            parseAllFiles();
            createFullArrays();
            dumpParsedData(outputFileName);
        }
        #endregion Event Handlers
        #region Utility Functions
        #region input parsing
        private void parseAllFiles()
        {
            emptyLists();
            foreach (string str in FileSearchListBox.Items)
            {
                parseFile(str);
            }
        }
        private void parseFile(string absoluteFileName)
        {
            StreamReader fileReader = new StreamReader(absoluteFileName);
            int lineNb = 1;
            bool itemOpen = false;
            bool iconOpen = false;
            bool descOpen = false;
            bool thingClose = false;
            bool foundID = false;
            bool foundName = false;
            bool foundFlags = false;
            bool foundStackSize = false;
            bool foundType = false;
            bool foundResourceId = false;
            bool foundValidTargets = false;
            bool foundLogNameS = false;
            bool foundLogNameP = false;
            bool foundLevel = false;
            bool foundSlots = false;
            bool foundRaces = false;
            bool foundJobs = false;
            bool foundImage = false;
            bool foundShieldSize = false;
            bool foundCastTime = false;
            bool foundUseDelay = false;
            bool foundReuseDelay = false;
            bool foundElement = false;
            bool foundStorageSlots = false;
            bool foundDamage = false;
            bool foundDelay = false;
            bool foundDps = false;
            bool foundSkill = false;
            bool foundJugSize = false;
            bool foundDescription = false;
            while (!fileReader.EndOfStream)
            //for (int ii = 0; ii < 600; ii++)
            {
                String tempString = fileReader.ReadLine();

                Regex regexXMLVersion = new Regex(".*xml version.*");
                Regex regexItems = new Regex("items>");
                Match matchXMLVersion = regexXMLVersion.Match(tempString);
                Match matchItems = regexItems.Match(tempString);
                if (matchXMLVersion.Success || matchItems.Success)
                {
                    continue;
                }

                tempString = tempString.Replace("&amp;", "&");
                tempString = tempString.Replace("♂", " Male");
                tempString = tempString.Replace("♀", " Female");
                
                String searchMatch = "=\"([^\"]*)\"";
                Regex regexLocal = new Regex("id" + searchMatch);
                Match matchLocal = regexLocal.Match(tempString);
                if(matchLocal.Groups.Count != 2)
                {
                    MessageBox.Show("Could not find id on line '" + tempString + "'");
                    continue;
                }
                ushort itemId = ushort.Parse(matchLocal.Groups[1].ToString());
                if (itemId == 65535)
                {
                    continue;
                }
                itemIdList.Add(itemId);
                regexLocal = new Regex("en" + searchMatch);
                matchLocal = regexLocal.Match(tempString);
                itemNameList.Add(matchLocal.Groups[1].ToString());
                if (FlagsCheckBox.Checked)
                {
                    regexLocal = new Regex("flags" + searchMatch);
                    matchLocal = regexLocal.Match(tempString);
                    itemFlagsList.Add(ushort.Parse(matchLocal.Groups[1].ToString()));
                }
                if (StackSizeCheckBox.Checked)
                {
                    regexLocal = new Regex("stack" + searchMatch);
                    matchLocal = regexLocal.Match(tempString);
                    itemStackSizeList.Add(byte.Parse(matchLocal.Groups[1].ToString()));
                }
                if (!foundType && TypeCheckBox.Checked)
                {
                    regexLocal = new Regex("type" + searchMatch);
                    matchLocal = regexLocal.Match(tempString);
                    itemTypeList.Add(ushort.Parse(matchLocal.Groups[1].ToString()));
                }
                if (LogNameSCheckBox.Checked)
                {
                    regexLocal = new Regex("enl" + searchMatch);
                    matchLocal = regexLocal.Match(tempString);
                    itemLogNameSList.Add(matchLocal.Groups[1].ToString());
                }
                if (LogNamePCheckBox.Checked)
                {
                    regexLocal = new Regex("enlp" + searchMatch);
                    matchLocal = regexLocal.Match(tempString);
                    itemLogNamePList.Add(matchLocal.Groups[1].ToString());
                }
                if (CombatSkillCheckBox.Checked)
                {
                    regexLocal = new Regex("skill=\"([0-9]*)\".*category=\"[Ww]eapon\"");
                    matchLocal = regexLocal.Match(tempString);
                    if (matchLocal.Success)
                    {
                        itemSkillList.Add(Byte.Parse(matchLocal.Groups[1].ToString()));
                    }
                    else
                    {
                        itemSkillList.Add(0);
                    }
                }
                //if (!foundLevel && LevelCheckBox.Checked)
                //{
                //    itemLevelList.Add(0);
                //}
                //if (!foundSlots && SlotsCheckBox.Checked)
                //{
                //    itemSlotsList.Add(0);
                //}
                //if (!foundRaces && RacesCheckBox.Checked)
                //{
                //    itemRacesList.Add(0);
                //}
                //if (!foundJobs && JobsCheckBox.Checked)
                //{
                //    itemJobsList.Add(0xFFFFFFFF);
                //}
                //if (!foundShieldSize && ShieldSizeCheckBox.Checked)
                //{
                //    itemShieldSizeList.Add(0);
                //}
                //if (!foundCastTime && CastTimeCheckBox.Checked)
                //{
                //    itemCastTimeList.Add(0);
                //}
                //if (!foundUseDelay && UseDelayCheckBox.Checked)
                //{
                //    itemUseDelayList.Add(0);
                //}
                //if (!foundReuseDelay && ReuseDelayCheckBox.Checked)
                //{
                //    itemReuseDelayList.Add(0);
                //}
                //if (!foundElement && ElementCheckBox.Checked)
                //{
                //    itemElementList.Add(0xFF);
                //}
                //if (!foundStorageSlots && StorageSlotsCheckBox.Checked)
                //{
                //    itemStorageSlotsList.Add(0);
                //}
                //if (!foundDamage && DamageCheckBox.Checked)
                //{
                //    itemDamageList.Add(0);
                //}
                //if (!foundDelay && DelayCheckBox.Checked)
                //{
                //    itemDelayList.Add(0);
                //}
                //if (!foundDps && DpsCheckBox.Checked)
                //{
                //    itemDpsList.Add(0);
                //}
                //if (!foundJugSize && JugSizeCheckBox.Checked)
                //{
                //    itemJugSizeList.Add(0);
                //}
                //if (!foundDescription && DescriptionCheckBox.Checked)
                //{
                //    itemDescriptionList.Add("");
                //}
            }
            Console.WriteLine("Item names found:");
            foreach (string str in itemNameList)
            {
                Console.WriteLine("Item Name: " + str);
            }
            Console.WriteLine("Item IDs found:");
            foreach (ushort id in itemIdList)
            {
                Console.WriteLine("Item ID:   " + id);
            }
            foreach (ushort flags in itemFlagsList)
            {
                Console.WriteLine("Item Flags: " + flags);
            }
            foreach (byte stackSize in itemStackSizeList)
            {
                Console.WriteLine("Item Stack Size: " + stackSize);
            }
            foreach (ushort type in itemTypeList)
            {
                Console.WriteLine("Item Type: " + type);
            }
            foreach (uint resourceID in itemResourceIdList)
            {
                Console.WriteLine("Item Resource ID: " + resourceID);
            }
            foreach (ushort validTarget in itemValidTargetsList)
            {
                Console.WriteLine("Item Valid Target: " + validTarget);
            }
            foreach (string str in itemLogNameSList)
            {
                Console.WriteLine("Item Log Name Sng: " + str);
            }
            foreach (string str in itemLogNamePList)
            {
                Console.WriteLine("Item Log Name Plr: " + str);
            }
            foreach (byte level in itemLevelList)
            {
                Console.WriteLine("Item Level: " + level);
            }
            foreach (ushort slots in itemSlotsList)
            {
                Console.WriteLine("Item Slots: " + slots);
            }
            foreach (ushort races in itemRacesList)
            {
                Console.WriteLine("Item Races: " + races);
            }
            foreach (uint jobs in itemJobsList)
            {
                Console.WriteLine("Item Jobs: " + jobs);
            }
            foreach (byte shieldSize in itemShieldSizeList)
            {
                Console.WriteLine("Item Shield Size: " + shieldSize);
            }
            foreach (ushort time in itemCastTimeList)
            {
                Console.WriteLine("Item Cast Time: " + time);
            }
            foreach (ushort time in itemUseDelayList)
            {
                Console.WriteLine("Item Use Delay: " + time);
            }
            foreach (ushort time in itemReuseDelayList)
            {
                Console.WriteLine("Item Reuse Delay: " + time);
            }
            foreach (byte element in itemElementList)
            {
                Console.WriteLine("Item Element: " + element);
            }
            foreach (byte slots in itemStorageSlotsList)
            {
                Console.WriteLine("Item Storage Slots: " + slots);
            }
            foreach (ushort dmg in itemDamageList)
            {
                Console.WriteLine("Item Damage: " + dmg);
            }
            foreach (ushort delay in itemDelayList)
            {
                Console.WriteLine("Item Delay: " + delay);
            }
            foreach (ushort dps in itemDpsList)
            {
                Console.WriteLine("Item DPS: " + dps);
            }
            foreach (byte skill in itemSkillList)
            {
                Console.WriteLine("Item Skill: " + skill);
            }
            foreach (ushort size in itemJugSizeList)
            {
                Console.WriteLine("Item Jug Size: " + size);
            }
            foreach (string desc in itemDescriptionList)
            {
                Console.WriteLine("Item Description: " + desc);
            }
            fileReader.Close();
        }

        private void emptyLists()
        {
            itemIdList.Clear();
            itemNameList.Clear();
            itemFlagsList.Clear();
            itemStackSizeList.Clear();
            itemTypeList.Clear();
            itemResourceIdList.Clear();
            itemValidTargetsList.Clear();
            itemLogNameSList.Clear();
            itemLogNamePList.Clear();
            itemLevelList.Clear();
            itemSlotsList.Clear();
            itemRacesList.Clear();
            itemJobsList.Clear();
            itemImageList.Clear();
            itemShieldSizeList.Clear();
            itemCastTimeList.Clear();
            itemUseDelayList.Clear();
            itemReuseDelayList.Clear();
            itemElementList.Clear();
            itemStorageSlotsList.Clear();
            itemDamageList.Clear();
            itemDelayList.Clear();
            itemDpsList.Clear();
            itemSkillList.Clear();
            itemJugSizeList.Clear();
            itemDescriptionList.Clear();
        }
        #endregion input parsing
        #region array handling
        private void createFullArrays()
        {
            maxItemID = itemIdList.Max<ushort>();

            //Create, initialize, and fill the item name array
            itemNameArray = new string[maxItemID+1];
            for (int kk = 0; kk <= maxItemID; kk++)
            {
                itemNameArray[kk] = "";
            }
            int nbOfItems = itemIdList.Count;
            for (int ii = 0; ii < nbOfItems; ii++)
            {
                itemNameArray[itemIdList[ii]] = itemNameList[ii];
            }

            if (FlagsCheckBox.Checked)
            {
                //Create, initialize, and fill the item flags array
                itemFlagsArray = new ushort[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemFlagsArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemFlagsArray[itemIdList[kk]] = itemFlagsList[kk];
                }
            }
            if (StackSizeCheckBox.Checked)
            {
                //Create, initialize, and fill the item stack size array
                itemStackSizeArray = new byte[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemStackSizeArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemStackSizeArray[itemIdList[kk]] = itemStackSizeList[kk];
                }
            }
            if (TypeCheckBox.Checked)
            {
                //Create, initialize, and fill the item type array
                itemTypeArray = new ushort[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemTypeArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemTypeArray[itemIdList[kk]] = itemTypeList[kk];
                }
            }
            if (ResourceIdCheckBox.Checked)
            {
                //Create, initialize, and fill the item resource ID array
                itemResourceIdArray = new uint[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemResourceIdArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemResourceIdArray[itemIdList[kk]] = itemResourceIdList[kk];
                }
            }
            if (ValidTargetsCheckBox.Checked)
            {
                //Create, initialize, and fill the item resource ID array
                itemValidTargetsArray = new ushort[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemValidTargetsArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemValidTargetsArray[itemIdList[kk]] = itemValidTargetsList[kk];
                }
            }
            if (LogNameSCheckBox.Checked)
            {
                //Create, initialize, and fill the item singular log name array
                itemLogNameSArray = new string[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemLogNameSArray[ii] = "";
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemLogNameSArray[itemIdList[kk]] = itemLogNameSList[kk];
                }
            }
            if (LogNamePCheckBox.Checked)
            {
                //Create, initialize, and fill the item plural log name array
                itemLogNamePArray = new string[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemLogNamePArray[ii] = "";
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemLogNamePArray[itemIdList[kk]] = itemLogNamePList[kk];
                }
            }
            if (LevelCheckBox.Checked)
            {
                //Create, initialize, and fill the item level array
                itemLevelArray = new byte[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemLevelArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemLevelArray[itemIdList[kk]] = itemLevelList[kk];
                }
            }
            if (SlotsCheckBox.Checked)
            {
                //Create, initialize, and fill the item slots array
                itemSlotsArray = new ushort[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemSlotsArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemSlotsArray[itemIdList[kk]] = itemSlotsList[kk];
                }
            }
            if (RacesCheckBox.Checked)
            {
                //Create, initialize, and fill the item races array
                itemRacesArray = new ushort[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemRacesArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemRacesArray[itemIdList[kk]] = itemRacesList[kk];
                }
            }
            if (JobsCheckBox.Checked)
            {
                //Create, initialize, and fill the item jobs array
                itemJobsArray = new uint[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemJobsArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemJobsArray[itemIdList[kk]] = itemJobsList[kk];
                }
            }
            if (ImageCheckBox.Checked)
            {
                //Create, initialize, and fill the item image array
                itemImageArray = new string[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemImageArray[ii] = "";
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemImageArray[itemIdList[kk]] = itemImageList[kk];
                }
            }
            if (ShieldSizeCheckBox.Checked)
            {
                //Create, initialize, and fill the item shield size array
                itemShieldSizeArray = new byte[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemShieldSizeArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemShieldSizeArray[itemIdList[kk]] = itemShieldSizeList[kk];
                }
            }
            if (CastTimeCheckBox.Checked)
            {
                //Create, initialize, and fill the item cast time array
                itemCastTimeArray = new ushort[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemCastTimeArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemCastTimeArray[itemIdList[kk]] = itemCastTimeList[kk];
                }
            }
            if (UseDelayCheckBox.Checked)
            {
                //Create, initialize, and fill the item use delay array
                itemUseDelayArray = new ushort[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemUseDelayArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemUseDelayArray[itemIdList[kk]] = itemUseDelayList[kk];
                }
            }
            if (ReuseDelayCheckBox.Checked)
            {
                //Create, initialize, and fill the item reuse delay array
                itemReuseDelayArray = new ushort[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemReuseDelayArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemReuseDelayArray[itemIdList[kk]] = itemReuseDelayList[kk];
                }
            }
            if (ElementCheckBox.Checked)
            {
                //Create, initialize, and fill the item element array
                itemElementArray = new byte[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemElementArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemElementArray[itemIdList[kk]] = itemElementList[kk];
                }
            }
            if (StorageSlotsCheckBox.Checked)
            {
                //Create, initialize, and fill the item storage slots array
                itemStorageSlotsArray = new byte[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemStorageSlotsArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemStorageSlotsArray[itemIdList[kk]] = itemStorageSlotsList[kk];
                }
            }
            if (DamageCheckBox.Checked)
            {
                //Create, initialize, and fill the item damage array
                itemDamageArray = new ushort[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemDamageArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemDamageArray[itemIdList[kk]] = itemDamageList[kk];
                }
            }
            if (DelayCheckBox.Checked)
            {
                //Create, initialize, and fill the item delay array
                itemDelayArray = new ushort[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemDelayArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemDelayArray[itemIdList[kk]] = itemDelayList[kk];
                }
            }
            if (DpsCheckBox.Checked)
            {
                //Create, initialize, and fill the item dps array
                itemDpsArray = new ushort[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemDpsArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemDpsArray[itemIdList[kk]] = itemDpsList[kk];
                }
            }
            if (CombatSkillCheckBox.Checked)
            {
                //Create, initialize, and fill the item skill array
                itemSkillArray = new byte[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemSkillArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemSkillArray[itemIdList[kk]] = itemSkillList[kk];
                }
            }
            if (JugSizeCheckBox.Checked)
            {
                //Create, initialize, and fill the item jug size array
                itemJugSizeArray = new ushort[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemJugSizeArray[ii] = 0;
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemJugSizeArray[itemIdList[kk]] = itemJugSizeList[kk];
                }
            }
            if (DescriptionCheckBox.Checked)
            {
                //Create, initialize, and fill the item description array
                itemDescriptionArray = new string[maxItemID + 1];
                for (int ii = 0; ii <= maxItemID; ii++)
                {
                    itemDescriptionArray[ii] = "";
                }
                for (int kk = 0; kk < nbOfItems; kk++)
                {
                    itemDescriptionArray[itemIdList[kk]] = itemDescriptionList[kk];
                }
            }
        }
        #endregion array handling
        #region output file handling
        #region file generation function
        private void dumpParsedData(string outputFileName)
        {
            StreamWriter outputFile = new StreamWriter(outputFileName);
            writeFileHeader(outputFile);
            writeItemNameArray(outputFile);
            if (FlagsCheckBox.Checked)
            {
                writeItemFlagsArray(outputFile);
            }
            if (StackSizeCheckBox.Checked)
            {
                writeItemStackSizeArray(outputFile);
            }
            if (TypeCheckBox.Checked)
            {
                writeItemTypeArray(outputFile);
            }
            if (ResourceIdCheckBox.Checked)
            {
                writeItemResourceIdArray(outputFile);
            }
            if (ValidTargetsCheckBox.Checked)
            {
                writeItemValidTargetsArray(outputFile);
            }
            if (LogNameSCheckBox.Checked)
            {
                writeItemLogNameSArray(outputFile);
            }
            if (LogNamePCheckBox.Checked)
            {
                writeItemLogNamePArray(outputFile);
            }
            if (LevelCheckBox.Checked)
            {
                writeItemLevelArray(outputFile);
            }
            if (SlotsCheckBox.Checked)
            {
                writeItemSlotsArray(outputFile);
            }
            if (RacesCheckBox.Checked)
            {
                writeItemRacesArray(outputFile);
            }
            if (JobsCheckBox.Checked)
            {
                writeItemJobsArray(outputFile);
            }
            if (ImageCheckBox.Checked)
            {
                writeItemImageArray(outputFile);
            }
            if (ShieldSizeCheckBox.Checked)
            {
                writeItemShieldSizeArray(outputFile);
            }
            if (CastTimeCheckBox.Checked)
            {
                writeItemCastTimeArray(outputFile);
            }
            if (UseDelayCheckBox.Checked)
            {
                writeItemUseDelayArray(outputFile);
            }
            if (ReuseDelayCheckBox.Checked)
            {
                writeItemReuseDelayArray(outputFile);
            }
            if (ElementCheckBox.Checked)
            {
                writeItemElementArray(outputFile);
            }
            if (StorageSlotsCheckBox.Checked)
            {
                writeItemStorageSlotsArray(outputFile);
            }
            if (DamageCheckBox.Checked)
            {
                writeItemDamageArray(outputFile);
            }
            if (DelayCheckBox.Checked)
            {
                writeItemDelayArray(outputFile);
            }
            if (DpsCheckBox.Checked)
            {
                writeItemDpsArray(outputFile);
            }
            if (CombatSkillCheckBox.Checked)
            {
                writeItemSkillArray(outputFile);
            }
            if (JugSizeCheckBox.Checked)
            {
                writeItemJugSizeArray(outputFile);
            }
            if (DescriptionCheckBox.Checked)
            {
                writeItemDescriptionArray(outputFile);
            }
            writeFileFunctions(outputFile);
            writeFileClosing(outputFile);
            outputFile.Close();
        }
        #endregion file generation function
        #region header and function generation
        private void writeFileHeader(StreamWriter outputFile)
        {
            outputFile.WriteLine("using System;");
            outputFile.WriteLine("using System.Collections.Generic;");
            outputFile.WriteLine("using System.Drawing;");
            outputFile.WriteLine("using System.Linq;");
            outputFile.WriteLine("using System.Text;");
            outputFile.WriteLine("");
            outputFile.WriteLine("namespace Iocaine2.Data.Client");
            outputFile.WriteLine("{");
            outputFile.WriteLine("    public partial class Things");
            outputFile.WriteLine("    {");
            outputFile.WriteLine("        public static ushort invalidID = 0;");
            outputFile.WriteLine("        public static int maxID = " + maxItemID + ";");
        }
        private void writeFileFunctions(StreamWriter outputFile)
        {
            outputFile.WriteLine("        #region Public Functions");
            outputFile.WriteLine("        public static ushort GetIdFromName(string thingName)");
            outputFile.WriteLine("        {");
            outputFile.WriteLine("            int index = Array.IndexOf<string>(thingNames, thingName);");
            outputFile.WriteLine("            if (index == -1)");
            outputFile.WriteLine("            {");
            outputFile.WriteLine("                return 0;");
            outputFile.WriteLine("            }");
            outputFile.WriteLine("            else");
            outputFile.WriteLine("            {");
            outputFile.WriteLine("                return (ushort)index;");
            outputFile.WriteLine("            }");
            outputFile.WriteLine("        }");
            outputFile.WriteLine("        public static string GetNameFromId(ushort thingId)");
            outputFile.WriteLine("        {");
            outputFile.WriteLine("            if (thingId > maxID)");
            outputFile.WriteLine("            {");
            outputFile.WriteLine("                return \"\";");
            outputFile.WriteLine("            }");
            outputFile.WriteLine("            else");
            outputFile.WriteLine("            {");
            outputFile.WriteLine("                return thingNames[(int)thingId];");
            outputFile.WriteLine("            }");
            outputFile.WriteLine("        }");
            if (FlagsCheckBox.Checked)
            {
                outputFile.WriteLine("        public static ushort GetFlagsFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingFlags[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (StackSizeCheckBox.Checked)
            {
                outputFile.WriteLine("        public static byte GetStackSizeFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingStackSize[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (TypeCheckBox.Checked)
            {
                outputFile.WriteLine("        public static ITEM_TYPE GetTypeFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return (ITEM_TYPE)thingType[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (ResourceIdCheckBox.Checked)
            {
                outputFile.WriteLine("        public static uint GetResourceIdFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingResourceId[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (ValidTargetsCheckBox.Checked)
            {
                outputFile.WriteLine("        public static ushort GetValidTargetsFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingValidTargets[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (LogNameSCheckBox.Checked)
            {
                outputFile.WriteLine("        public static string GetLogNameSFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return \"\";");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingLogNameS[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (LogNamePCheckBox.Checked)
            {
                outputFile.WriteLine("        public static string GetLogNamePFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return \"\";");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingLogNameP[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (LevelCheckBox.Checked)
            {
                outputFile.WriteLine("        public static byte GetLevelFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingLevel[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (SlotsCheckBox.Checked)
            {
                outputFile.WriteLine("        public static ushort GetSlotsFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingSlots[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (RacesCheckBox.Checked)
            {
                outputFile.WriteLine("        public static ushort GetRacesFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingRaces[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (JobsCheckBox.Checked)
            {
                outputFile.WriteLine("        public static uint GetJobsFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingJobs[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (ImageCheckBox.Checked)
            {
                outputFile.WriteLine("        public static Image GetImageFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return null;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return Base64ToImage.ConvertThis(thingImage[(int)thingId]);");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (ShieldSizeCheckBox.Checked)
            {
                outputFile.WriteLine("        public static byte GetShieldSizeFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingShieldSize[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (CastTimeCheckBox.Checked)
            {
                outputFile.WriteLine("        public static ushort GetCastTimeFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingCastTime[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (UseDelayCheckBox.Checked)
            {
                outputFile.WriteLine("        public static ushort GetUseDelayFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingUseDelay[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (ReuseDelayCheckBox.Checked)
            {
                outputFile.WriteLine("        public static ushort GetReuseDelayFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingReuseDelay[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (ElementCheckBox.Checked)
            {
                outputFile.WriteLine("        public static byte GetElementFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingElement[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (StorageSlotsCheckBox.Checked)
            {
                outputFile.WriteLine("        public static byte GetStorageSlotsFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingStorageSlots[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (DamageCheckBox.Checked)
            {
                outputFile.WriteLine("        public static ushort GetDamageFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingDamage[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (DelayCheckBox.Checked)
            {
                outputFile.WriteLine("        public static ushort GetDelayFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingDelay[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (DpsCheckBox.Checked)
            {
                outputFile.WriteLine("        public static ushort GetDpsFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingDps[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (CombatSkillCheckBox.Checked)
            {
                outputFile.WriteLine("        public static byte GetSkillFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingSkill[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (JugSizeCheckBox.Checked)
            {
                outputFile.WriteLine("        public static ushort GetJugSizeFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return 0;");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingJugSize[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            if (DescriptionCheckBox.Checked)
            {
                outputFile.WriteLine("        public static string GetDescriptionFromId(ushort thingId)");
                outputFile.WriteLine("        {");
                outputFile.WriteLine("            if (thingId > maxID)");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return \"\";");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("            else");
                outputFile.WriteLine("            {");
                outputFile.WriteLine("                return thingDescription[(int)thingId];");
                outputFile.WriteLine("            }");
                outputFile.WriteLine("        }");
            }
            outputFile.WriteLine("        #endregion Public Functions");
        }
        private void writeFileClosing(StreamWriter outputFile)
        {
            outputFile.WriteLine("    }");
            outputFile.WriteLine("}");
        }
        #endregion header and function generation
        #region array generation
        private void writeItemNameArray(StreamWriter outputFile)
        {
            if (itemNameArray == null)
            {
                Console.WriteLine("[ERROR] itemNameArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingNames Array");
            outputFile.WriteLine("        private static string[] thingNames = new string[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      \"" + itemNameArray[ii] + "\",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      \"" + itemNameArray[ii] + "\"};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingNames Array");
        }
        private void writeItemFlagsArray(StreamWriter outputFile)
        {
            if (itemFlagsArray == null)
            {
                Console.WriteLine("[ERROR] itemFlagsArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingFlags Array");
            outputFile.WriteLine("        private static ushort[] thingFlags = new ushort[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemFlagsArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemFlagsArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingFlags Array");
        }
        private void writeItemStackSizeArray(StreamWriter outputFile)
        {
            if (itemStackSizeArray == null)
            {
                Console.WriteLine("[ERROR] itemStackSizeArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingStackSize Array");
            outputFile.WriteLine("        private static byte[] thingStackSize = new byte[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemStackSizeArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemStackSizeArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingStackSize Array");
        }
        private void writeItemTypeArray(StreamWriter outputFile)
        {
            if (itemTypeArray == null)
            {
                Console.WriteLine("[ERROR] itemTypeArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingType Array");
            outputFile.WriteLine("        private static byte[] thingType = new byte[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemTypeArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemTypeArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingType Array");
        }
        private void writeItemResourceIdArray(StreamWriter outputFile)
        {
            if (itemResourceIdArray == null)
            {
                Console.WriteLine("[ERROR] itemResourceIdArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingResourceId Array");
            outputFile.WriteLine("        private static uint[] thingResourceId = new uint[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemResourceIdArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemResourceIdArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingResourceId Array");
        }
        private void writeItemValidTargetsArray(StreamWriter outputFile)
        {
            if (itemValidTargetsArray == null)
            {
                Console.WriteLine("[ERROR] itemValidTargetsArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingValidTargets Array");
            outputFile.WriteLine("        private static ushort[] thingValidTargets = new ushort[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemValidTargetsArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemValidTargetsArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingValidTargets Array");
        }
        private void writeItemLogNameSArray(StreamWriter outputFile)
        {
            if (itemLogNameSArray == null)
            {
                Console.WriteLine("[ERROR] itemLogNameSArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingLogNameS Array");
            outputFile.WriteLine("        private static string[] thingLogNameS = new string[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      \"" + itemLogNameSArray[ii] + "\",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      \"" + itemLogNameSArray[ii] + "\"};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingLogNameS Array");
        }
        private void writeItemLogNamePArray(StreamWriter outputFile)
        {
            if (itemLogNamePArray == null)
            {
                Console.WriteLine("[ERROR] itemLogNamePArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingLogNameP Array");
            outputFile.WriteLine("        private static string[] thingLogNameP = new string[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      \"" + itemLogNamePArray[ii] + "\",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      \"" + itemLogNamePArray[ii] + "\"};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingLogNameP Array");
        }
        private void writeItemLevelArray(StreamWriter outputFile)
        {
            if (itemLevelArray == null)
            {
                Console.WriteLine("[ERROR] itemLevelArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingLevel Array");
            outputFile.WriteLine("        private static byte[] thingLevel = new byte[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemLevelArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemLevelArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingLevel Array");
        }
        private void writeItemSlotsArray(StreamWriter outputFile)
        {
            if (itemSlotsArray == null)
            {
                Console.WriteLine("[ERROR] itemSlotsArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingSlots Array");
            outputFile.WriteLine("        private static ushort[] thingSlots = new ushort[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemSlotsArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemSlotsArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingSlots Array");
        }
        private void writeItemRacesArray(StreamWriter outputFile)
        {
            if (itemRacesArray == null)
            {
                Console.WriteLine("[ERROR] itemRacesArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingRaces Array");
            outputFile.WriteLine("        private static ushort[] thingRaces = new ushort[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemRacesArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemRacesArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingRaces Array");
        }
        private void writeItemJobsArray(StreamWriter outputFile)
        {
            if (itemJobsArray == null)
            {
                Console.WriteLine("[ERROR] itemJobsArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingJobs Array");
            outputFile.WriteLine("        private static uint[] thingJobs = new uint[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemJobsArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemJobsArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingJobs Array");
        }
        private void writeItemImageArray(StreamWriter outputFile)
        {
            if (itemImageArray == null)
            {
                Console.WriteLine("[ERROR] itemImageArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingJobs Array");
            outputFile.WriteLine("        private static string[] thingImage = new string[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      \"" + itemImageArray[ii] + "\",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      \"" + itemImageArray[ii] + "\"};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingImage Array");
        }
        private void writeItemShieldSizeArray(StreamWriter outputFile)
        {
            if (itemShieldSizeArray == null)
            {
                Console.WriteLine("[ERROR] itemShieldSizeArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingShieldSize Array");
            outputFile.WriteLine("        private static byte[] thingShieldSize = new byte[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemShieldSizeArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemShieldSizeArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingShieldSize Array");
        }
        private void writeItemCastTimeArray(StreamWriter outputFile)
        {
            if (itemCastTimeArray == null)
            {
                Console.WriteLine("[ERROR] itemCastTimeArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingCastTime Array");
            outputFile.WriteLine("        private static ushort[] thingCastTime = new ushort[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemCastTimeArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemCastTimeArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingCastTime Array");
        }
        private void writeItemUseDelayArray(StreamWriter outputFile)
        {
            if (itemUseDelayArray == null)
            {
                Console.WriteLine("[ERROR] itemUseDelayArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingUseDelay Array");
            outputFile.WriteLine("        private static ushort[] thingUseDelay = new ushort[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemUseDelayArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemUseDelayArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingUseDelay Array");
        }
        private void writeItemReuseDelayArray(StreamWriter outputFile)
        {
            if (itemReuseDelayArray == null)
            {
                Console.WriteLine("[ERROR] itemReuseDelayArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingReuseDelay Array");
            outputFile.WriteLine("        private static ushort[] thingReuseDelay = new ushort[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemReuseDelayArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemReuseDelayArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingReuseDelay Array");
        }
        private void writeItemElementArray(StreamWriter outputFile)
        {
            if (itemElementArray == null)
            {
                Console.WriteLine("[ERROR] itemElementArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingElement Array");
            outputFile.WriteLine("        private static byte[] thingElement = new byte[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemElementArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemElementArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingElement Array");
        }
        private void writeItemStorageSlotsArray(StreamWriter outputFile)
        {
            if (itemStorageSlotsArray == null)
            {
                Console.WriteLine("[ERROR] itemStorageSlotsArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingStorageSlots Array");
            outputFile.WriteLine("        private static byte[] thingStorageSlots = new byte[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemStorageSlotsArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemStorageSlotsArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingStorageSlots Array");
        }
        private void writeItemDamageArray(StreamWriter outputFile)
        {
            if (itemDamageArray == null)
            {
                Console.WriteLine("[ERROR] itemDamageArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingDamage Array");
            outputFile.WriteLine("        private static ushort[] thingDamage = new ushort[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemDamageArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemDamageArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingDamage Array");
        }
        private void writeItemDelayArray(StreamWriter outputFile)
        {
            if (itemDelayArray == null)
            {
                Console.WriteLine("[ERROR] itemDelayArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingDelay Array");
            outputFile.WriteLine("        private static ushort[] thingDelay = new ushort[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemDelayArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemDelayArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingDelay Array");
        }
        private void writeItemDpsArray(StreamWriter outputFile)
        {
            if (itemDpsArray == null)
            {
                Console.WriteLine("[ERROR] itemDpsArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingDps Array");
            outputFile.WriteLine("        private static ushort[] thingDps = new ushort[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemDpsArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemDpsArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingDps Array");
        }
        private void writeItemSkillArray(StreamWriter outputFile)
        {
            if (itemSkillArray == null)
            {
                Console.WriteLine("[ERROR] itemSkillArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingSkill Array");
            outputFile.WriteLine("        private static byte[] thingSkill = new byte[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemSkillArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemSkillArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingSkill Array");
        }
        private void writeItemJugSizeArray(StreamWriter outputFile)
        {
            if (itemJugSizeArray == null)
            {
                Console.WriteLine("[ERROR] itemJugSizeArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingJugSize Array");
            outputFile.WriteLine("        private static ushort[] thingJugSize = new ushort[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      " + itemJugSizeArray[ii] + ",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      " + itemJugSizeArray[ii] + "};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingJugSize Array");
        }
        private void writeItemDescriptionArray(StreamWriter outputFile)
        {
            if (itemDescriptionArray == null)
            {
                Console.WriteLine("[ERROR] itemDescriptionArray not set to an instance");
                return;
            }
            outputFile.WriteLine("        #region thingDescription Array");
            outputFile.WriteLine("        private static string[] thingDescription = new string[" + (maxItemID + 1) + "] {");
            for (int ii = 0; ii <= maxItemID; ii++)
            {
                if (ii != maxItemID)
                {
                    outputFile.WriteLine("                                      \"" + itemDescriptionArray[ii] + "\",\t\t//ID=" + ii);
                }
                else
                {
                    outputFile.WriteLine("                                      \"" + itemDescriptionArray[ii] + "\"};\t\t//ID=" + ii);
                }
            }
            outputFile.WriteLine("        #endregion thingDescription Array");
        }
        #endregion array generation
        #endregion output file handling
        #endregion Utility Functions
    }
}