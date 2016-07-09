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

using Iocaine2.Data.Client;

namespace ResourceParser
{
    public partial class ParserForm : Form
    {
        #region Form Initialization
        public ParserForm()
        {
            InitializeComponent();
            StreamWriter logFile = null;
            logFile = new StreamWriter("Iocaine Log.txt");
            logFile.AutoFlush = true;
            mainDB = new MainDatabase();
            Console.SetOut(logFile);
            Console.SetError(logFile);
        }
        #endregion Form Initialization
        #region Enums
        private enum TYPE
        {
            NONE=0,
            JA=1,
            MAGIC=2
        }
        #endregion Enums
        #region Member Objects/Variables
        public static MainDatabase mainDB = null;
        public static String apostrophy = "&apst";
        public const String unknown = "Unknown";
        #region Flags
        private static Boolean savingResources = false;
        private static Boolean savingMapsInfo = false;
        private static Boolean parsedJA = false;
        private static Boolean parsedSpells = false;
        private static Boolean parsedWS = false;
        private static Boolean parsedBuffs = false;
        private static Boolean parsedElements = false;
        private static Boolean parsedSkillTypes = false;
        private static Boolean parsedJobs = false;
        private static Boolean parsedTargets = true;
        private static Boolean parsedZones = false;
        private static Boolean parsedMapInfo = false;
        #endregion Flags
        #region Map Info State Info
        //private static Boolean inZone = false;
        //private static Boolean inMaps = false;
        //private static Boolean inMap = false;
        //private static Boolean inBoxes = false;
        //private static Boolean finishedMap = false;
        //private static UInt16 currentZone = 0;
        //private static Byte currentMap = 0;
        //private static Single zoneMult = 0;
        //private static Int16 zoneX = 0;
        #endregion Map Info State Info
        #endregion Member Objects/Variables
        #region Event Handlers
        private void AddFileButton_Click(object sender, EventArgs e)
        {
            String exePath = Path.GetDirectoryName(Application.ExecutablePath);
            String combinedPath = "";
            combinedPath = exePath + @"\..\..\..\..\..\Iocaine IP\Game Info\";
            combinedPath = Path.GetFullPath(combinedPath);
            OpenFileDialog.InitialDirectory = combinedPath;

            OpenFileDialog.ShowDialog();
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            parseAllFiles();
            String exePath = Path.GetDirectoryName(Application.ExecutablePath);
            String combinedPath = "";
            if(savingResources)
            {
                combinedPath = exePath + @"\..\..\..\..\..\Iocaine Assemblies\FFXIResource\FFXIResource\";
                combinedPath = Path.GetFullPath(combinedPath);
                SaveResourcesFileDialog.InitialDirectory = combinedPath;
                SaveResourcesFileDialog.ShowDialog();
            }

            if (savingMapsInfo)
            {
                combinedPath = exePath + @"\..\..\..\..\..\Iocaine Assemblies\MapDll\MapDll\";
                combinedPath = Path.GetFullPath(combinedPath);
                SaveMapDataFileDialog.InitialDirectory = combinedPath;
                SaveMapDataFileDialog.FileName = "MapInfoLoad.cs";
                SaveMapDataFileDialog.OverwritePrompt = false;
                SaveMapDataFileDialog.ShowDialog();
            }
        }
        private void OpenFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            String[] files = OpenFileDialog.FileNames;
            foreach (String str in files)
            {
                FileSearchListBox.Items.Add(str);
            }
        }
        private void SaveResourcesFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            string outputFileName = SaveResourcesFileDialog.FileName;
            FileInfo fileInfo = new FileInfo(outputFileName);
            string outputFilePath = fileInfo.Directory.FullName;
            //parseAllFiles();
            if (JAChkB.Checked && parsedJA)
            {
                dumpJAData(outputFilePath);
            }
            if (WSChkB.Checked && parsedWS)
            {
                dumpWSData(outputFilePath);
            }
            if (SpellsChkB.Checked && parsedSpells)
            {
                dumpSpellsData(outputFilePath);
            }
            if (StatusEffectsChkB.Checked && parsedBuffs)
            {
                dumpStatusEffectsData(outputFilePath);
            }
            if(ElementsChkB.Checked && parsedElements)
            {
                dumpElementsData(outputFilePath);
            }
            if(SkillsChkB.Checked && parsedSkillTypes)
            {
                dumpSkillsData(outputFilePath);
            }
            if(JobsChkB.Checked && parsedJobs)
            {
                dumpJobsData(outputFilePath);
            }
            if(TargetsChkB.Checked && parsedTargets)
            {
                dumpTargetsData(outputFilePath);
            }
            if(ZonesChkB.Checked && parsedZones)
            {
                dumpZonesData(outputFilePath);
            }
            this.Cursor = Cursors.Arrow;
        }
        private void SaveMapDataFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            string outputFileName = SaveMapDataFileDialog.FileName;
            FileInfo fileInfo = new FileInfo(outputFileName);
            string outputFilePath = fileInfo.Directory.FullName;
            //parseAllFiles();
            if (MapInfoChkB.Checked && parsedMapInfo)
            {
                dumpMapInfoData(outputFilePath);
            }
            this.Cursor = Cursors.Arrow;
        }
        #endregion Event Handlers
        #region Utility Functions
        #region input parsing
        private void parseAllFiles()
        {
            loadTargets();
            //Jobs needs to be parsed first.
            bool foundJobsFile = false;
            foreach (string str in FileSearchListBox.Items)
            {
                if(str.Contains("jobs.lua"))
                {
                    parseFile(str);
                    foundJobsFile = true;
                    FileSearchListBox.Items.Remove(str);
                    parsedJobs = true;
                    if(JobsChkB.Checked)
                    {
                        savingResources = true;
                    }
                    break;
                }
            }
            foreach (string str in FileSearchListBox.Items)
            {
                if (str.Contains("job_abilities.lua") && !foundJobsFile)
                {
                    MessageBox.Show("To parse job abilities, the jobs.lua file is needed first. Skipping JA's.");
                    continue;
                }
                else if (str.Contains("weapon_skills.lua") && !foundJobsFile)
                {
                    MessageBox.Show("To parse weapon skills, the jobs.lua file is needed first. Skipping WS's.");
                    continue;
                }
                else
                {
                    if (str.Contains("buffs.lua") && StatusEffectsChkB.Checked)
                    {
                        parsedBuffs = true;
                        savingResources = true;
                    }
                    else if(str.Contains("elements.lua") && ElementsChkB.Checked)
                    {
                        parsedElements = true;
                        savingResources = true;
                    }
                    else if (str.Contains("job_abilities.lua") && JAChkB.Checked)
                    {
                        parsedJA = true;
                        savingResources = true;
                    }
                    else if (str.Contains("\\skills.lua") && SkillsChkB.Checked)
                    {
                        parsedSkillTypes = true;
                        savingResources = true;
                    }
                    else if (str.Contains("spells.lua") && SpellsChkB.Checked)
                    {
                        parsedSpells = true;
                        savingResources = true;
                    }
                    else if(str.Contains("zones.lua") && ZonesChkB.Checked)
                    {
                        parsedZones = true;
                        savingResources = true;
                    }
                    else if (str.Contains("weapon_skills.lua") && WSChkB.Checked)
                    {
                        parsedWS = true;
                        savingResources = true;
                    }
                    else if (str.Contains("map_data.txt") && MapInfoChkB.Checked)
                    {
                        parsedMapInfo = true;
                        savingMapsInfo = true;
                        parseMapInfoFile(str);
                        continue;
                    }
                    parseFile(str);
                }
            }
        }
        private void parseFile(string absoluteFileName)
        {
            StreamReader fileReader = new StreamReader(absoluteFileName);
            while (!fileReader.EndOfStream)
            {
                parseLine(fileReader.ReadLine(), absoluteFileName);
            }
            fileReader.Close();
        }
        private void parseLine(string text, string filename)
        {
            #region JA
            if (filename.Contains("job_abilities.lua"))
            {
                if (JAChkB.Checked && (text.Contains("prefix=\"/jobability\"")
                    || text.Contains("prefix=\"/pet\"")))
                {
                    MainDatabase.JARow row = mainDB.JA.NewJARow();
                    row.Name = getEn(text);
                    row.ID = getId(text);
                    row.Command = getPrefix(text) + " \"" + row.Name + "\"";
                    row.Targets = getTargets(text);
                    Byte jobLevel = 0;
                    row.Job = getJAJobAndLevel(row.Name, out jobLevel);
                    row.JobLevel = jobLevel;
                    row.MP = getMp(text);
                    row.TP = getTp(text);
                    row.Duration = getDuration(text);
                    row.Element = getElement(text);
                    row.RecastID = getRecastId(text);
                    if (row.RecastID == 0)
                    {
                        //1-hour ability
                        row.AsSub = false;
                    }
                    else
                    {
                        row.AsSub = true; //TBD
                    }
                    row.Type = getType(text);
                    mainDB.JA.AddJARow(row);
                    Console.WriteLine("Adding JA " + row.Name);
                }
            }
            #endregion JA
            #region Spells
            else if (filename.Contains("spells.lua"))
            {
                if (SpellsChkB.Checked && (text.Contains("prefix=\"/magic\"")
                || text.Contains("prefix=\"/song\"")
                || text.Contains("prefix=\"/ninjutsu\"")))
                {
                    MainDatabase.SpellsRow row = mainDB.Spells.NewSpellsRow();
                    row.Name = getEn(text);
                    row.ID = getId(text);
                    row.Type = getType(text);
                    row.Skill = getSkillType(text);
                    row.Command = getPrefix(text) + " \"" + row.Name + "\"";
                    row.Element = getElement(text);
                    row.MP = getMp(text);
                    row.RecastID = getRecastId(text);
                    row.CastTime = getCastTime(text);
                    row.Duration = getDuration(text);
                    row.Range = getRange(text);
                    row.Targets = getTargets(text);
                    row.AsSub = true; //TBD
                    for (Byte ii = 1; ii <= 23; ii++)
                    {
                        row[("LevelJob" + ii.ToString())] = getJobLevel(text, ii);
                    }
                    mainDB.Spells.AddSpellsRow(row);
                    Console.WriteLine("Adding Spell " + row.Name);
                }
            }
            #endregion Spells
            #region WS
            else if (filename.Contains("weapon_skills.lua"))
            {
                if (WSChkB.Checked && (text.Contains("prefix=\"/weaponskill\"")))
                {
                    MainDatabase.WSRow row = mainDB.WS.NewWSRow();
                    row.Name = getEn(text);
                    row.ID = getId(text);
                    row.Command = getPrefix(text) + " \"" + row.Name + "\"";
                    row.SkillType = getSkillType(text);
                    UInt16 skillLevel = 0;
                    UInt32 jobs = 0;
                    UInt32 subs = 0;
                    String special = "";
                    getWSInfo(row.Name, out skillLevel, out jobs, out subs, out special);
                    row.SkillLevel = skillLevel;
                    row.Special = special;
                    row.Jobs = jobs;
                    row.JobsSubs = subs;
                    row.AttrA = getWsAttribute(text, 'a');
                    row.AttrB = getWsAttribute(text, 'b');
                    row.AttrC = getWsAttribute(text, 'c');
                    mainDB.WS.AddWSRow(row);
                    Console.WriteLine("Adding WS " + row.Name);
                }
            }
            #endregion WS
            #region Buffs
            else if (filename.Contains("buffs.lua"))
            {
                if (StatusEffectsChkB.Checked && text.Contains("id="))
                {
                    MainDatabase.StatusEffectsRow row = mainDB.StatusEffects.NewStatusEffectsRow();
                    row.ID = getId(text);
                    row.Name = getEn(text);
                    Console.WriteLine("StatusEffect: " + row.Name + ", " + row.ID);
                    mainDB.StatusEffects.AddStatusEffectsRow(row);
                }
            }
            #endregion Buffs
            #region Elements
            else if (filename.Contains("elements.lua"))
            {
                if (ElementsChkB.Checked && text.Contains("id="))
                {
                    MainDatabase.ElementsRow row = mainDB.Elements.NewElementsRow();
                    row.ID = getIdInt(text);
                    row.Name = getEn(text);
                    Console.WriteLine("Element: " + row.Name + ", " + row.ID);
                    mainDB.Elements.AddElementsRow(row);
                }
            }
            #endregion Elements
            #region Skills
            else if (filename.Contains("skills.lua"))
            {
                if (SkillsChkB.Checked && text.Contains("id="))
                {
                    MainDatabase.SkillsRow row = mainDB.Skills.NewSkillsRow();
                    row.ID = getId(text);
                    row.Name = getEn(text);
                    row.Category = getCategory(text);
                    Console.WriteLine("Skill: " + row.Name + ", " + row.ID);
                    mainDB.Skills.AddSkillsRow(row);
                }
            }
            #endregion Skills
            #region Jobs
            else if (filename.Contains("jobs.lua"))
            {
                if (JobsChkB.Checked && text.Contains("id="))
                {
                    MainDatabase.JobsRow row = mainDB.Jobs.NewJobsRow();
                    row.ID = (Byte)getId(text);
                    row.Name = getEn(text);
                    row.Abbreviation = getEns(text);
                    Console.WriteLine("Job: " + row.Name + ", " + row.ID);
                    mainDB.Jobs.AddJobsRow(row);
                }
            }
            #endregion Jobs
            else
            {
                return;
            }
        }
        private void parseMapInfoFile(String iAbsFilename)
        {
                    //String filter = "en=\"([^\"]*)\"";
                    //return doRegex(str, filter);
            String line = "";
            String filter = "";
            String result = "";
            UInt16 zoneID = 0;
            Byte mapID = 0;
            Int16 boxX1 = 0;
            Int16 boxX2 = 0;
            Int16 boxY1 = 0;
            Int16 boxY2 = 0;
            Int16 boxZ1 = 0;
            Int16 boxZ2 = 0;
            Single mapMult = 0;
            Single mapX = 0;
            Single mapY = 0;
            Boolean error = false;
            StreamReader fileReader = new StreamReader(iAbsFilename);
            List<String> fileContents = new List<string>();
            while (!fileReader.EndOfStream)
            {
                fileContents.Add(fileReader.ReadLine());
            }
            int lineNb = 0;
            while (lineNb < fileContents.Count)
            {
                //Zone ID "1:"
                line = fileContents[lineNb++];
                filter = "^([0-9]+):";
                result = doRegex(line, filter);
                if (!UInt16.TryParse(result, out zoneID))
                {
                    MessageBox.Show("Could not parse zoneID '" + result + "' from line '" + line + "'");
                    parsedMapInfo = false;
                    return;
                }
                //Discard the zone name
                lineNb++;

                //Check if there are no maps, i.e. "maps: {}"
                
                line = fileContents[lineNb++];
                filter = @"(maps: {})";
                result = doRegex(line, filter);
                if(result != unknown)
                {
                    //This zone has no maps or boxes, discard and continue.
                    continue;
                }

                Boolean doneWithMaps = false;
                while (!doneWithMaps)
                {
                    //Map ID "    0:"
                    line = fileContents[lineNb++];
                    filter = @"\s+([0-9]+):";
                    result = doRegex(line, filter);
                    if (!Byte.TryParse(result, out mapID))
                    {
                        MessageBox.Show("Could not parse mapID '" + result + "' from line '" + line + "'");
                        parsedMapInfo = false;
                        return;
                    }

                    //Discard the "boxes:"
                    lineNb++;

                    Boolean doneWithBoxes = false;
                    UInt16 boxCnt = 0;
                    line = fileContents[lineNb++];
                    while (!doneWithBoxes)
                    {
                        //- {x1: -1280, x2: 1280, y1: -1280, y2: 1280, z1: -10000, z2: 10000}
                        filter = @"\s- {x1: ([-0-9]*), x2: ([-0-9]*), y1: ([-0-9]*), y2: ([-0-9]*), z1: ([-0-9]*), z2: ([-0-9]*)}";
                        List<String> results = doRegexMulti(line, filter);
                        if(results.Count != 7)
                        {
                            MessageBox.Show("Did not get 6 coordinates from '" + line + "'");
                            parsedMapInfo = false;
                            return;
                        }
                        error = !Int16.TryParse(results[1], out boxX1);
                        error = !Int16.TryParse(results[2], out boxX2) || error;
                        error = !Int16.TryParse(results[3], out boxY1) || error;
                        error = !Int16.TryParse(results[4], out boxY2) || error;
                        error = !Int16.TryParse(results[5], out boxZ1) || error;
                        error = !Int16.TryParse(results[6], out boxZ2) || error;
                        MainDatabase.MapInfoBoxesRow boxRow = mainDB.MapInfoBoxes.NewMapInfoBoxesRow();
                        boxRow.ZoneID = zoneID;
                        boxRow.MapID = mapID;
                        boxRow.BoxID = boxCnt;
                        boxRow.X1 = boxX1;
                        boxRow.X2 = boxX2;
                        boxRow.Y1 = boxY1;
                        boxRow.Y2 = boxY2;
                        boxRow.Z1 = boxZ1;
                        boxRow.Z2 = boxZ2;
                        mainDB.MapInfoBoxes.Rows.Add(boxRow);


                        line = fileContents[lineNb];
                        filter = @"mult: ([0-9\.]+)";
                        result = doRegex(line, filter);
                        if(result != unknown)
                        {
                            doneWithBoxes = true;
                        }
                        else
                        {
                            boxCnt++;
                            lineNb++;
                        }
                    }
                    line = fileContents[lineNb++];
                    if (!Single.TryParse(result, out mapMult))
                    {
                        MessageBox.Show("Could not parse mult. '" + result + "' from line '" + line + "'");
                        parsedMapInfo = false;
                        return;
                    }
                    line = fileContents[lineNb++];
                    filter = @"\sx: ([-0-9\.]+)";
                    result = doRegex(line, filter);
                    if (!Single.TryParse(result, out mapX))
                    {
                        MessageBox.Show("Could not parse mapX '" + result + "' from line '" + line + "'");
                        parsedMapInfo = false;
                        return;
                    }
                    line = fileContents[lineNb++];
                    filter = @"\sy: ([-0-9\.]+)";
                    result = doRegex(line, filter);
                    if (!Single.TryParse(result, out mapY))
                    {
                        MessageBox.Show("Could not parse mapY '" + result + "' from line '" + line + "'");
                        parsedMapInfo = false;
                        return;
                    }
                    MainDatabase.MapInfoMapsRow mapRow = mainDB.MapInfoMaps.NewMapInfoMapsRow();
                    mapRow.ZoneID = zoneID;
                    mapRow.MapID = mapID;
                    mapRow.Multiplier = mapMult;
                    mapRow.X = mapX;
                    mapRow.Y = mapY;
                    mainDB.MapInfoMaps.Rows.Add(mapRow);

                    if(lineNb >= fileContents.Count)
                    {
                        return;
                    }
                    line = fileContents[lineNb];
                    filter = @"^([0-9]*):";
                    result = doRegex(line, filter);
                    if (result != unknown)
                    {
                        doneWithMaps = true;
                    }
                }
            }
            fileReader.Close();
        }
        private String doRegex(String iStr, String iFilter)
        {
            Regex regex1 = new Regex(iFilter);
            Match match1;
            match1 = regex1.Match(iStr);
            if (!match1.Success)
            {
                Console.WriteLine("//Could not find match(" + iFilter + ") in line '" + iStr + "'");
                return "Unknown";
            }
            else
            {
                return encodeApostrophy(match1.Groups[1].ToString());
            }
        }
        private List<String> doRegexMulti(String iStr, String iFilter)
        {
            List<String> results = new List<string>();
            Regex regex = new Regex(iFilter);
            Match match = regex.Match(iStr);
            if (match.Groups.Count == 0)
            {
                Console.WriteLine("Could not find match(" + iFilter + ") in line '" + iStr + "'");
                results.Add(unknown);
                return results;
            }
            else
            {
                foreach (Group grp in match.Groups)
                {
                    results.Add(encodeApostrophy(grp.ToString()));
                }
                return results;
            }
        }
        private String getEn(String str)
        {
            //[1] = {id=1,en="Cure",....
            String filter = "en=\"([^\"]*)\"";
            return doRegex(str, filter);
        }
        private String getEns(String str)
        {
            String filter = "ens=\"([^\"]*)\"";
            return doRegex(str, filter);
        }
        private UInt16 getId(String str)
        {
            //[430] = {id=430,en="Diabolos's Favor",
            String filter = "[^_]id=([0-9]*),";
            return UInt16.Parse(doRegex(str, filter));
        }
        private Int16 getIdInt(String str)
        {
            //{id=-1,en="Physical"
            String filter = "[^_]id=(-*[0-9]*),";
            return Int16.Parse(doRegex(str, filter));
        }
        private String getPrefix(String str)
        {
            //mp_cost=0,prefix="/jobability",range=0
            String filter = "prefix=\"([^\"]*)\"";
            return doRegex(str, filter);
        }
        private Int16 getTargets(String str)
        {
            //recast_id=0,targets=1,tp_cost=0
            String filter = "targets=([0-9]*),";
            return Int16.Parse(doRegex(str, filter));
        }
        private String getType(String str)
        {
            //type="JobAbility"
            String filter = "type=\"([^\"]*)\"";
            return doRegex(str, filter);
        }
        private UInt16 getMp(String str)
        {
            //icon_id=66,mp_cost=0,prefix=
            String filter = "mp_cost=([0-9]*),";
            return UInt16.Parse(doRegex(str, filter));
        }
        private UInt16 getTp(String str)
        {
            //icon_id=66,mp_cost=0,prefix=
            String filter = "tp_cost=([0-9]*),";
            return UInt16.Parse(doRegex(str, filter));
        }
        private UInt16 getDuration(String str)
        {
            //duration=45,
            String filter = "duration=([0-9]*),";
            String match = doRegex(str, filter);
            if (match == "Unknown")
            {
                return 0;
            }
            else
            {
                return UInt16.Parse(match);
            }
        }
        private Int16 getElement(String str)
        {
            //,element=4,
            String filter = "element=([-]*[0-9]*),";
            return Int16.Parse(doRegex(str, filter));
        }
        private UInt16 getRecastId(String str)
        {
            //,recast_id=0,
            String filter = "recast_id=([0-9]*),";
            return UInt16.Parse(doRegex(str, filter));
        }
        private Byte getSkillType(String str)
        {
            //,skill=1,
            String filter = "skill=([0-9]*),";
            String match = doRegex(str, filter);
            if (match == "Unknown")
            {
                return 0;
            }
            else
            {
                return Byte.Parse(match);
            }
        }
        private String getWsAttribute(String str, char whichAttr)
        {
            //skillchain_a="Impaction",skillchain_b="",skillchain_c="",
            String filter = "skillchain_" + whichAttr + "=\"([^\"]*)\"";
            return doRegex(str, filter);
        }
        private Single getCastTime(String str)
        {
            //,cast_time=8,
            String filter = "cast_time=([0-9\\.]*),";
            return Single.Parse(doRegex(str, filter));
        }
        private UInt16 getRange(String str)
        {
            //,range=12,
            String filter = "range=([0-9]*),";
            return UInt16.Parse(doRegex(str, filter));
        }
        private Byte getJobLevel(String str, Byte jobIdx)
        {
            //,levels={[3]=14,[20]=17},
            String filter = "levels={[^}]*\\[" + jobIdx + "\\]=([0-9]*)[^}]*},";
            String match = doRegex(str, filter);
            if(match == "Unknown")
            {
                return 0;
            }
            else
            {
                Byte lvl = 0;
                if (Byte.TryParse(match, out lvl))
                {
                    return lvl;
                }
                else
                {
                    return 0xff;
                }
            }
        }
        private String getCategory(String str)
        {
            String filter = "category=\"([^\"]*)\"";
            return doRegex(str, filter);
        }
        private Byte getJAJobAndLevel(String iAbilityName, out Byte oLevel)
        {
            JobAndLevelPair jalp;
            if (mapJobAbil2Job.ContainsKey(iAbilityName))
            {
                jalp = mapJobAbil2Job[iAbilityName];
                //Now get the jobId from the abbreviation.
                MainDatabase.JobsRow[] rows = (MainDatabase.JobsRow[])mainDB.Jobs.Select("Abbreviation='" + jalp.Job + "'");
                if (rows.Length == 0)
                {
                    oLevel = 0;
                    return 0;
                }
                oLevel = jalp.Level;
                return rows[0].ID;
            }
            else
            {
                oLevel = 0;
                return 0;
            }
        }
        private void getWSInfo(String iName, out UInt16 oLevel, out UInt32 oJobs, out UInt32 oSubJobs, out String oSpecial)
        {
            WSManualInfo info;
            String localName = decodeApostrophy(iName);
            if (!mapWS2JobsList.ContainsKey(localName))
            {
                oLevel = 0;
                oJobs = 0;
                oSubJobs = 0;
                oSpecial = "";
                return;
            }
            info = mapWS2JobsList[localName];
            oLevel = info.SkillLevel;
            oSpecial = info.Special;
            List<String> jobsList = info.JobsThatCanUse;
            List<String> subJobsList = info.SubJobsThatCanUse;
            oJobs = 0;
            foreach(String job in jobsList)
            {
                MainDatabase.JobsRow[] rows = (MainDatabase.JobsRow[])mainDB.Jobs.Select("Abbreviation='" + job + "'");
                if(rows.Length == 0)
                {
                    continue;
                }
                Byte jobId = rows[0].ID;
                oJobs |= (UInt32)(1 << jobId);
            }
            oSubJobs = 0;
            foreach (String job in subJobsList)
            {
                MainDatabase.JobsRow[] rows = (MainDatabase.JobsRow[])mainDB.Jobs.Select("Abbreviation='" + job + "'");
                if (rows.Length == 0)
                {
                    continue;
                }
                Byte jobId = rows[0].ID;
                oSubJobs |= (UInt32)(1 << jobId);
            }
        }
        #endregion input parsing
        #region output handling
        private void dumpJAData(string outputFilePath)
        {
            String outputFileName = outputFilePath + "\\JobAbilitiesLoad.cs";
            StreamWriter outputFile = new StreamWriter(outputFileName);
            outputFile.WriteLine("using System;");
            outputFile.WriteLine("using System.Collections.Generic;");
            outputFile.WriteLine("using System.Data;");
            outputFile.WriteLine("using System.Linq;");
            outputFile.WriteLine("using System.Text;");
            outputFile.WriteLine("");
            outputFile.WriteLine("namespace Iocaine2.Data.Client");
            outputFile.WriteLine("{");
            outputFile.WriteLine("    public partial class JobAbilities");
            outputFile.WriteLine("    {");
            outputFile.WriteLine("        private static void loadData()");
            outputFile.WriteLine("        {");
            outputFile.WriteLine("            MainDatabase.JARow jaRow;");
            outputFile.WriteLine("");

            Int32 nbJAs = mainDB.JA.Rows.Count;
            for (int ii = 0; ii < nbJAs; ii++)
            {
                MainDatabase.JARow row = mainDB.JA[ii];
                outputFile.WriteLine("            jaRow = FfxiResource.mainDb.JA.NewJARow();");
                outputFile.WriteLine("            jaRow.Name = \"" + row.Name + "\";");
                outputFile.WriteLine("            jaRow.ID = " + row.ID + ";");
                outputFile.WriteLine("            jaRow.Command = \"" + row.Command.Replace("\"", "\\\"") + "\";");
                outputFile.WriteLine("            jaRow.Targets = " + row.Targets + ";");
                outputFile.WriteLine("            jaRow.Job = " + row.Job + ";");
                outputFile.WriteLine("            jaRow.JobLevel = " + row.JobLevel + ";");
                outputFile.WriteLine("            jaRow.AsSub = " + row.AsSub.ToString().ToLower() + ";");
                outputFile.WriteLine("            jaRow.MP = " + row.MP + ";");
                outputFile.WriteLine("            jaRow.TP = " + row.TP + ";");
                outputFile.WriteLine("            jaRow.Duration = " + (row.Duration * 1000).ToString() + ";");
                outputFile.WriteLine("            jaRow.Element = " + row.Element + ";");
                outputFile.WriteLine("            jaRow.RecastID = " + row.RecastID + ";");
                outputFile.WriteLine("            jaRow.Type = \"" + row.Type + "\";");
                outputFile.WriteLine("            FfxiResource.mainDb.JA.AddJARow(jaRow);");
                outputFile.WriteLine("");
            }
            outputFile.WriteLine("            FfxiResource.mainDb.JA.AcceptChanges();");
            outputFile.WriteLine("        }");
            outputFile.WriteLine("    }");
            outputFile.WriteLine("}");
            outputFile.Close();

            // This was for a clean slate (1-off) to work with on the manual job ability sheet.

            //outputFileName = outputFilePath + "\\ManualJobAbilities.cs";
            //outputFile = new StreamWriter(outputFileName);
            //outputFile.WriteLine("using System;");
            //outputFile.WriteLine("using System.Collections.Generic;");
            //outputFile.WriteLine("using System.Data;");
            //outputFile.WriteLine("using System.Linq;");
            //outputFile.WriteLine("using System.Text;");
            //outputFile.WriteLine("");
            //outputFile.WriteLine("namespace ResourceParser");
            //outputFile.WriteLine("{");
            //outputFile.WriteLine("    public partial class ParserForm");
            //outputFile.WriteLine("    {");
            //outputFile.WriteLine("        internal class JobAndLevelPair");
            //outputFile.WriteLine("        {");
            //outputFile.WriteLine("            internal String Job;");
            //outputFile.WriteLine("            internal Byte Level;");
            //outputFile.WriteLine("            internal JobAndLevelPair()");
            //outputFile.WriteLine("            {");
            //outputFile.WriteLine("            }");
            //outputFile.WriteLine("        }");
            //outputFile.WriteLine("        private Dictionary<String, JobAndLevelPair> mapJobAbil2Job = new Dictionary<string, JobAndLevelPair>()");
            //outputFile.WriteLine("        {");

            //for (int ii = 0; ii < nbJAs; ii++)
            //{
            //    MainDatabase.JARow row = mainDB.JA[ii];
            //    String temp = "            {\"" + row.Name + "\", new JobAndLevelPair{Job=\"UNK\", Level=1}}";
            //    if(ii != (nbJAs - 1))
            //    {
            //        temp += ",";
            //    }
            //    outputFile.WriteLine(temp);
            //}
            //outputFile.WriteLine("        };");
            //outputFile.WriteLine("    }");
            //outputFile.WriteLine("}");
            //outputFile.WriteLine("");
            //outputFile.Close();
        }
        private void dumpWSData(string outputFilePath)
        {
            String outputFileName = outputFilePath + "\\WeaponSkillsLoad.cs";
            StreamWriter outputFile = new StreamWriter(outputFileName);
            outputFile.WriteLine("using System;");
            outputFile.WriteLine("using System.Collections.Generic;");
            outputFile.WriteLine("using System.Data;");
            outputFile.WriteLine("using System.Linq;");
            outputFile.WriteLine("using System.Text;");
            outputFile.WriteLine("");
            outputFile.WriteLine("namespace Iocaine2.Data.Client");
            outputFile.WriteLine("{");
            outputFile.WriteLine("    public partial class WeaponSkills");
            outputFile.WriteLine("    {");
            outputFile.WriteLine("        private static void loadData()");
            outputFile.WriteLine("        {");
            outputFile.WriteLine("            MainDatabase.WSRow wsRow;");
            outputFile.WriteLine("");

            Int32 nbWSs = mainDB.WS.Rows.Count;
            for (int ii = 0; ii < nbWSs; ii++)
            {
                MainDatabase.WSRow row = mainDB.WS[ii];
                outputFile.WriteLine("            wsRow = FfxiResource.mainDb.WS.NewWSRow();");
                outputFile.WriteLine("            wsRow.Name = \"" + row.Name + "\";");
                outputFile.WriteLine("            wsRow.ID = " + row.ID + ";");
                outputFile.WriteLine("            wsRow.Command = \"" + row.Command.Replace("\"", "\\\"") + "\";");
                outputFile.WriteLine("            wsRow.SkillType = " + row.SkillType + ";");
                outputFile.WriteLine("            wsRow.SkillLevel = " + row.SkillLevel + ";");
                outputFile.WriteLine("            wsRow.Special = \"" + row.Special.Replace("\"", "\\\"") + "\";");
                outputFile.WriteLine("            wsRow.Jobs = 0x" + String.Format("{0:X}", row.Jobs) + ";");
                outputFile.WriteLine("            wsRow.JobsSubs = 0x" + String.Format("{0:X}", row.JobsSubs) + ";");
                outputFile.WriteLine("            wsRow.AttrA = \"" + row.AttrA + "\";");
                outputFile.WriteLine("            wsRow.AttrB = \"" + row.AttrB + "\";");
                outputFile.WriteLine("            wsRow.AttrC = \"" + row.AttrC + "\";");
                outputFile.WriteLine("            FfxiResource.mainDb.WS.AddWSRow(wsRow);");
                outputFile.WriteLine("");
            }
            outputFile.WriteLine("            FfxiResource.mainDb.WS.AcceptChanges();");
            outputFile.WriteLine("        }");
            outputFile.WriteLine("    }");
            outputFile.WriteLine("}");
            outputFile.Close();
        }
        private void dumpSpellsData(string outputFilePath)
        {
            String outputFileName = outputFilePath + "\\SpellsLoad.cs";
            StreamWriter outputFile = new StreamWriter(outputFileName);
            outputFile.WriteLine("using System;");
            outputFile.WriteLine("using System.Collections.Generic;");
            outputFile.WriteLine("using System.Data;");
            outputFile.WriteLine("using System.Linq;");
            outputFile.WriteLine("using System.Text;");
            outputFile.WriteLine("");
            outputFile.WriteLine("namespace Iocaine2.Data.Client");
            outputFile.WriteLine("{");
            outputFile.WriteLine("    public partial class Spells");
            outputFile.WriteLine("    {");
            outputFile.WriteLine("        private static void loadData()");
            outputFile.WriteLine("        {");
            outputFile.WriteLine("            MainDatabase.SpellsRow spellsRow;");
            outputFile.WriteLine("");

            Int32 nbSpellss = mainDB.Spells.Rows.Count;
            for (int ii = 0; ii < nbSpellss; ii++)
            {
                MainDatabase.SpellsRow row = mainDB.Spells[ii];
                outputFile.WriteLine("            spellsRow = FfxiResource.mainDb.Spells.NewSpellsRow();");
                outputFile.WriteLine("            spellsRow.Name = \"" + row.Name + "\";");
                outputFile.WriteLine("            spellsRow.ID = " + row.ID + ";");
                outputFile.WriteLine("            spellsRow.Type = \"" + row.Type + "\";");
                outputFile.WriteLine("            spellsRow.Skill = " + row.Skill + ";");
                outputFile.WriteLine("            spellsRow.Command = \"" + row.Command.Replace("\"", "\\\"") + "\";");
                outputFile.WriteLine("            spellsRow.Element = " + row.Element + ";");
                outputFile.WriteLine("            spellsRow.MP = " + row.MP + ";");
                outputFile.WriteLine("            spellsRow.RecastID = " + row.RecastID + ";");
                outputFile.WriteLine("            spellsRow.CastTime = " + (row.CastTime * 1000).ToString() + ";");
                outputFile.WriteLine("            spellsRow.Duration = " + (row.Duration * 1000).ToString() + ";");
                outputFile.WriteLine("            spellsRow.Range = " + row.Range + ";");
                outputFile.WriteLine("            spellsRow.Targets = " + row.Targets + ";");
                outputFile.WriteLine("            spellsRow.AsSub = " + row.AsSub.ToString().ToLower() + ";");
                outputFile.WriteLine("            spellsRow.LevelJob1 = " + row.LevelJob1 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob2 = " + row.LevelJob2 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob3 = " + row.LevelJob3 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob4 = " + row.LevelJob4 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob5 = " + row.LevelJob5 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob6 = " + row.LevelJob6 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob7 = " + row.LevelJob7 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob8 = " + row.LevelJob8 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob9 = " + row.LevelJob9 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob10 = " + row.LevelJob10 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob11 = " + row.LevelJob11 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob12 = " + row.LevelJob12 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob13 = " + row.LevelJob13 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob14 = " + row.LevelJob14 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob15 = " + row.LevelJob15 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob16 = " + row.LevelJob16 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob17 = " + row.LevelJob17 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob18 = " + row.LevelJob18 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob19 = " + row.LevelJob19 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob20 = " + row.LevelJob20 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob21 = " + row.LevelJob21 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob22 = " + row.LevelJob22 + ";");
                outputFile.WriteLine("            spellsRow.LevelJob23 = " + row.LevelJob23 + ";");
                outputFile.WriteLine("            FfxiResource.mainDb.Spells.AddSpellsRow(spellsRow);");
                outputFile.WriteLine("");
            }
            outputFile.WriteLine("            FfxiResource.mainDb.Spells.AcceptChanges();");
            outputFile.WriteLine("        }");
            outputFile.WriteLine("    }");
            outputFile.WriteLine("}");
            outputFile.Close();
        }
        private void dumpStatusEffectsData(string outputFilePath)
        {
            String outputFileName = outputFilePath + "\\StatusEffectsLoad.cs";
            StreamWriter outputFile = new StreamWriter(outputFileName);
            outputFile.WriteLine("using System;");
            outputFile.WriteLine("using System.Collections.Generic;");
            outputFile.WriteLine("using System.Data;");
            outputFile.WriteLine("using System.Linq;");
            outputFile.WriteLine("using System.Text;");
            outputFile.WriteLine("");
            outputFile.WriteLine("namespace Iocaine2.Data.Client");
            outputFile.WriteLine("{");
            outputFile.WriteLine("    public partial class StatusEffects");
            outputFile.WriteLine("    {");
            outputFile.WriteLine("        private static void loadData()");
            outputFile.WriteLine("        {");
            outputFile.WriteLine("            MainDatabase.StatusEffectsRow row;");
            outputFile.WriteLine("");

            Int32 nbSEs = mainDB.StatusEffects.Rows.Count;
            for (int ii = 0; ii < nbSEs; ii++)
            {
                MainDatabase.StatusEffectsRow row = mainDB.StatusEffects[ii];
                outputFile.WriteLine("            row = FfxiResource.mainDb.StatusEffects.NewStatusEffectsRow();");
                outputFile.WriteLine("            row.Name = \"" + row.Name + "\";");
                outputFile.WriteLine("            row.ID = " + row.ID + ";");
                outputFile.WriteLine("            FfxiResource.mainDb.StatusEffects.AddStatusEffectsRow(row);");
                outputFile.WriteLine("");
            }
            outputFile.WriteLine("            FfxiResource.mainDb.StatusEffects.AcceptChanges();");
            outputFile.WriteLine("        }");
            outputFile.WriteLine("    }");
            outputFile.WriteLine("}");
            outputFile.Close();
        }
        private void dumpElementsData(string outputFilePath)
        {
            String outputFileName = outputFilePath + "\\ElementsLoad.cs";
            StreamWriter outputFile = new StreamWriter(outputFileName);
            outputFile.WriteLine("using System;");
            outputFile.WriteLine("using System.Collections.Generic;");
            outputFile.WriteLine("using System.Data;");
            outputFile.WriteLine("using System.Linq;");
            outputFile.WriteLine("using System.Text;");
            outputFile.WriteLine("");
            outputFile.WriteLine("namespace Iocaine2.Data.Client");
            outputFile.WriteLine("{");
            outputFile.WriteLine("    public partial class Elements");
            outputFile.WriteLine("    {");
            outputFile.WriteLine("        private static void loadData()");
            outputFile.WriteLine("        {");
            outputFile.WriteLine("            MainDatabase.ElementsRow row;");
            outputFile.WriteLine("");

            Int32 nbRows = mainDB.Elements.Rows.Count;
            for (int ii = 0; ii < nbRows; ii++)
            {
                MainDatabase.ElementsRow row = mainDB.Elements[ii];
                outputFile.WriteLine("            row = FfxiResource.mainDb.Elements.NewElementsRow();");
                outputFile.WriteLine("            row.Name = \"" + row.Name + "\";");
                outputFile.WriteLine("            row.ID = " + row.ID + ";");
                outputFile.WriteLine("            FfxiResource.mainDb.Elements.AddElementsRow(row);");
                outputFile.WriteLine("");
            }
            outputFile.WriteLine("            FfxiResource.mainDb.Elements.AcceptChanges();");
            outputFile.WriteLine("        }");
            outputFile.WriteLine("    }");
            outputFile.WriteLine("}");
            outputFile.Close();
        }
        private void dumpSkillsData(string outputFilePath)
        {
            String outputFileName = outputFilePath + "\\SkillsLoad.cs";
            StreamWriter outputFile = new StreamWriter(outputFileName);
            outputFile.WriteLine("using System;");
            outputFile.WriteLine("using System.Collections.Generic;");
            outputFile.WriteLine("using System.Data;");
            outputFile.WriteLine("using System.Linq;");
            outputFile.WriteLine("using System.Text;");
            outputFile.WriteLine("");
            outputFile.WriteLine("namespace Iocaine2.Data.Client");
            outputFile.WriteLine("{");
            outputFile.WriteLine("    public partial class Skills");
            outputFile.WriteLine("    {");
            outputFile.WriteLine("        private static void loadData()");
            outputFile.WriteLine("        {");
            outputFile.WriteLine("            MainDatabase.SkillsRow row;");
            outputFile.WriteLine("");

            Int32 nbRows = mainDB.Skills.Rows.Count;
            for (int ii = 0; ii < nbRows; ii++)
            {
                MainDatabase.SkillsRow row = mainDB.Skills[ii];
                outputFile.WriteLine("            row = FfxiResource.mainDb.Skills.NewSkillsRow();");
                outputFile.WriteLine("            row.Name = \"" + row.Name + "\";");
                outputFile.WriteLine("            row.ID = " + row.ID + ";");
                outputFile.WriteLine("            row.Category = \"" + row.Category + "\";");
                outputFile.WriteLine("            FfxiResource.mainDb.Skills.AddSkillsRow(row);");
                outputFile.WriteLine("");
            }
            outputFile.WriteLine("            FfxiResource.mainDb.Skills.AcceptChanges();");
            outputFile.WriteLine("        }");
            outputFile.WriteLine("    }");
            outputFile.WriteLine("}");
            outputFile.Close();
        }
        private void dumpTargetsData(string outputFilePath)
        {
            String outputFileName = outputFilePath + "\\TargetsLoad.cs";
            StreamWriter outputFile = new StreamWriter(outputFileName);
            outputFile.WriteLine("using System;");
            outputFile.WriteLine("using System.Collections.Generic;");
            outputFile.WriteLine("using System.Data;");
            outputFile.WriteLine("using System.Linq;");
            outputFile.WriteLine("using System.Text;");
            outputFile.WriteLine("");
            outputFile.WriteLine("namespace Iocaine2.Data.Client");
            outputFile.WriteLine("{");
            outputFile.WriteLine("    public partial class Targets");
            outputFile.WriteLine("    {");
            outputFile.WriteLine("        private static void loadData()");
            outputFile.WriteLine("        {");
            outputFile.WriteLine("            MainDatabase.TargetsRow row;");
            outputFile.WriteLine("");

            Int32 nbRows = mainDB.Targets.Rows.Count;
            for (int ii = 0; ii < nbRows; ii++)
            {
                MainDatabase.TargetsRow row = mainDB.Targets[ii];
                outputFile.WriteLine("            row = FfxiResource.mainDb.Targets.NewTargetsRow();");
                outputFile.WriteLine("            row.ID = " + row.ID + ";");
                outputFile.WriteLine("            row.Self = " + row.Self.ToString().ToLower() + ";");
                outputFile.WriteLine("            row.Party = " + row.Party.ToString().ToLower() + ";");
                outputFile.WriteLine("            row.Alliance = " + row.Alliance.ToString().ToLower() + ";");
                outputFile.WriteLine("            row.Anyone = " + row.Anyone.ToString().ToLower() + ";");
                outputFile.WriteLine("            row.Monster = " + row.Monster.ToString().ToLower() + ";");
                outputFile.WriteLine("            row.Trust = " + row.Trust.ToString().ToLower() + ";");
                outputFile.WriteLine("            row.Fellow = " + row.Fellow.ToString().ToLower() + ";");
                outputFile.WriteLine("            row.Dead = " + row.Dead.ToString().ToLower() + ";");
                outputFile.WriteLine("            FfxiResource.mainDb.Targets.AddTargetsRow(row);");
                outputFile.WriteLine("");
            }
            outputFile.WriteLine("            FfxiResource.mainDb.Targets.AcceptChanges();");
            outputFile.WriteLine("        }");
            outputFile.WriteLine("    }");
            outputFile.WriteLine("}");
            outputFile.Close();
        }
        private void dumpJobsData(string outputFilePath)
        {
            String outputFileName = outputFilePath + "\\JobsLoad.cs";
            StreamWriter outputFile = new StreamWriter(outputFileName);
            outputFile.WriteLine("using System;");
            outputFile.WriteLine("using System.Collections.Generic;");
            outputFile.WriteLine("using System.Data;");
            outputFile.WriteLine("using System.Linq;");
            outputFile.WriteLine("using System.Text;");
            outputFile.WriteLine("");
            outputFile.WriteLine("namespace Iocaine2.Data.Client");
            outputFile.WriteLine("{");
            outputFile.WriteLine("    public partial class Jobs");
            outputFile.WriteLine("    {");
            outputFile.WriteLine("        private static void loadData()");
            outputFile.WriteLine("        {");
            outputFile.WriteLine("            MainDatabase.JobsRow row;");
            outputFile.WriteLine("");

            Int32 nbRows = mainDB.Jobs.Rows.Count;
            for (int ii = 0; ii < nbRows; ii++)
            {
                MainDatabase.JobsRow row = mainDB.Jobs[ii];
                outputFile.WriteLine("            row = FfxiResource.mainDb.Jobs.NewJobsRow();");
                outputFile.WriteLine("            row.Name = \"" + row.Name + "\";");
                outputFile.WriteLine("            row.ID = " + row.ID + ";");
                outputFile.WriteLine("            row.Abbreviation = \"" + row.Abbreviation + "\";");
                outputFile.WriteLine("            FfxiResource.mainDb.Jobs.AddJobsRow(row);");
                outputFile.WriteLine("");
            }
            outputFile.WriteLine("            FfxiResource.mainDb.Jobs.AcceptChanges();");
            outputFile.WriteLine("        }");
            outputFile.WriteLine("    }");
            outputFile.WriteLine("}");
            outputFile.Close();
        }
        private void dumpZonesData(string outputFilePath)
        {
            String outputFileName = outputFilePath + "\\TargetsLoad.cs";
            StreamWriter outputFile = new StreamWriter(outputFileName);
            outputFile.WriteLine("using System;");
            outputFile.WriteLine("using System.Collections.Generic;");
            outputFile.WriteLine("using System.Data;");
            outputFile.WriteLine("using System.Linq;");
            outputFile.WriteLine("using System.Text;");
            outputFile.WriteLine("");
            outputFile.WriteLine("namespace Iocaine2.Data.Client");
            outputFile.WriteLine("{");
            outputFile.WriteLine("    public partial class Targets");
            outputFile.WriteLine("    {");
            outputFile.WriteLine("        private static void loadData()");
            outputFile.WriteLine("        {");
            outputFile.WriteLine("            MainDatabase.TargetsRow row;");
            outputFile.WriteLine("");

            Int32 nbRows = mainDB.Targets.Rows.Count;
            for (int ii = 0; ii < nbRows; ii++)
            {
                MainDatabase.TargetsRow row = mainDB.Targets[ii];
                outputFile.WriteLine("            row = FfxiResource.mainDb.Targets.NewTargetsRow();");
                outputFile.WriteLine("            row.ID = " + row.ID + ";");
                outputFile.WriteLine("            row.Self = " + row.Self.ToString().ToLower() + ";");
                outputFile.WriteLine("            row.Party = " + row.Party.ToString().ToLower() + ";");
                outputFile.WriteLine("            row.Alliance = " + row.Alliance.ToString().ToLower() + ";");
                outputFile.WriteLine("            row.Anyone = " + row.Anyone.ToString().ToLower() + ";");
                outputFile.WriteLine("            row.Monster = " + row.Monster.ToString().ToLower() + ";");
                outputFile.WriteLine("            row.Trust = " + row.Trust.ToString().ToLower() + ";");
                outputFile.WriteLine("            row.Fellow = " + row.Fellow.ToString().ToLower() + ";");
                outputFile.WriteLine("            row.Dead = " + row.Dead.ToString().ToLower() + ";");
                outputFile.WriteLine("            FfxiResource.mainDb.Targets.AddTargetsRow(row);");
                outputFile.WriteLine("");
            }
            outputFile.WriteLine("            FfxiResource.mainDb.Targets.AcceptChanges();");
            outputFile.WriteLine("        }");
            outputFile.WriteLine("    }");
            outputFile.WriteLine("}");
            outputFile.Close();
        }
        private void dumpMapInfoData(string outputFilePath)
        {
            String outputFileName = outputFilePath + "\\MapInfoLoad.cs";
            StreamWriter outputFile = new StreamWriter(outputFileName);
            outputFile.WriteLine("using System;");
            outputFile.WriteLine("using System.Collections.Generic;");
            outputFile.WriteLine("using System.Data;");
            outputFile.WriteLine("using System.Linq;");
            outputFile.WriteLine("using System.Text;");
            outputFile.WriteLine("");
            outputFile.WriteLine("namespace Iocaine2.Data.Client");
            outputFile.WriteLine("{");
            outputFile.WriteLine("    public static partial class Maps");
            outputFile.WriteLine("    {");
            outputFile.WriteLine("        private static void loadData()");
            outputFile.WriteLine("        {");
            outputFile.WriteLine("            #region Boxes");
            outputFile.WriteLine("            mapInfoDS.MapInfoBoxes.Rows.Clear();");
            outputFile.WriteLine("            MapInfoDS.MapInfoBoxesRow rowBoxes = null;");
            outputFile.WriteLine("");

            Int32 nbBoxes = mainDB.MapInfoBoxes.Rows.Count;
            for (int ii = 0; ii < nbBoxes; ii++)
            {
                MainDatabase.MapInfoBoxesRow row = mainDB.MapInfoBoxes[ii];
                outputFile.WriteLine("            rowBoxes = mapInfoDS.MapInfoBoxes.NewMapInfoBoxesRow();");
                outputFile.WriteLine("            rowBoxes.ZoneID = " + row.ZoneID + ";");
                outputFile.WriteLine("            rowBoxes.MapID = " + row.MapID + ";");
                outputFile.WriteLine("            rowBoxes.BoxID = " + row.BoxID + ";");
                outputFile.WriteLine("            rowBoxes.X1 = " + row.X1 + ";");
                outputFile.WriteLine("            rowBoxes.X2 = " + row.X2 + ";");
                outputFile.WriteLine("            rowBoxes.Y1 = " + row.Y1 + ";");
                outputFile.WriteLine("            rowBoxes.Y2 = " + row.Y2 + ";");
                outputFile.WriteLine("            rowBoxes.Z1 = " + row.Z1 + ";");
                outputFile.WriteLine("            rowBoxes.Z2 = " + row.Z2 + ";");
                outputFile.WriteLine("            mapInfoDS.MapInfoBoxes.Rows.Add(rowBoxes);");
                outputFile.WriteLine("");
            }
            outputFile.WriteLine("            mapInfoDS.MapInfoBoxes.AcceptChanges();");
            outputFile.WriteLine("            #endregion Boxes");
            outputFile.WriteLine("            #region Maps");
            outputFile.WriteLine("            mapInfoDS.MapInfoMaps.Rows.Clear();");
            outputFile.WriteLine("            MapInfoDS.MapInfoMapsRow rowMaps = null;");
            outputFile.WriteLine("");
            Int32 nbMaps = mainDB.MapInfoMaps.Rows.Count;
            for (int ii = 0; ii < nbMaps; ii++)
            {
                MainDatabase.MapInfoMapsRow row = mainDB.MapInfoMaps[ii];
                outputFile.WriteLine("            rowMaps = mapInfoDS.MapInfoMaps.NewMapInfoMapsRow();");
                outputFile.WriteLine("            rowMaps.ZoneID = " + row.ZoneID + ";");
                outputFile.WriteLine("            rowMaps.MapID = " + row.MapID + ";");
                outputFile.WriteLine("            rowMaps.Multiplier = " + row.Multiplier + "f;");
                outputFile.WriteLine("            rowMaps.X = " + row.X + "f;");
                outputFile.WriteLine("            rowMaps.Y = " + row.Y + "f;");
                outputFile.WriteLine("            mapInfoDS.MapInfoMaps.Rows.Add(rowMaps);");
                outputFile.WriteLine("");
            }
            outputFile.WriteLine("            mapInfoDS.MapInfoMaps.AcceptChanges();");
            outputFile.WriteLine("            #endregion Maps");
            outputFile.WriteLine("        }");
            outputFile.WriteLine("    }");
            outputFile.WriteLine("}");
            outputFile.Close();
        }
        private static bool saveToDatabase()
        {
            //return DBFunctions.saveTables();
            return true;
        }
        #endregion output file handling
        #region apostrophies
        public static String encodeApostrophy(String str)
        {
            if (str.Contains("'"))
            {
                return str.Replace("'", apostrophy);
            }
            else
            {
                return str;
            }
        }
        public static String decodeApostrophy(String str)
        {
            if (str.Contains(apostrophy))
            {
                return str.Replace(apostrophy, "'");
            }
            else
            {
                return str;
            }
        }
        #endregion apostrophies
        #endregion Utility Functions
    }
}
