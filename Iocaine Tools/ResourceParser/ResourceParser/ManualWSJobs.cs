using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResourceParser
{
    public partial class ParserForm
    {
        internal class WSManualInfo
        {
            internal UInt16 SkillLevel;
            internal List<String> JobsThatCanUse;
            internal List<String> SubJobsThatCanUse; //i.e. THF/MNK can use Raging Fists, but THF/DNC cannot.
            internal String Special; //i.e. Relic, Mythic, Quest, Merit, Empyrean, etc.
            internal WSManualInfo()
            {
            }
        };
        private Dictionary<String, WSManualInfo> mapWS2JobsList = new Dictionary<string, WSManualInfo>()
        {
            {"Combo", new WSManualInfo{
                    SkillLevel = 5,
                    JobsThatCanUse = new List<string>{"MNK", "PUP", "DNC", "WAR", "THF", "NIN"},
                    SubJobsThatCanUse = new List<string>{"MNK", "PUP", "DNC", "WAR", "THF", "NIN"},
                    Special=""}},
            {"Shoulder Tackle", new WSManualInfo{
                    SkillLevel = 40,
                    JobsThatCanUse = new List<string>{"MNK", "PUP", "DNC", "WAR", "THF", "NIN"},
                    SubJobsThatCanUse = new List<string>{"MNK", "PUP", "DNC", "WAR", "THF", "NIN"},
                    Special=""}},
            {"One Inch Punch", new WSManualInfo{
                    SkillLevel = 75,
                    JobsThatCanUse = new List<string>{"MNK", "PUP", "DNC", "WAR", "THF", "NIN"},
                    SubJobsThatCanUse = new List<string>{"MNK", "PUP", "DNC", "WAR", "THF", "NIN"},
                    Special=""}},
            {"Backhand Blow", new WSManualInfo{
                    SkillLevel = 100,
                    JobsThatCanUse = new List<string>{"MNK", "PUP", "DNC", "WAR", "THF", "NIN"},
                    SubJobsThatCanUse = new List<string>{"MNK", "PUP", "DNC", "WAR", "THF", "NIN"},
                    Special=""}},
            {"Raging Fists", new WSManualInfo{
                    SkillLevel = 125,
                    JobsThatCanUse = new List<string>{"MNK", "PUP"},
                    SubJobsThatCanUse = new List<string>{"MNK", "PUP"},
                    Special=""}},
            {"Spinning Attack", new WSManualInfo{
                    SkillLevel = 150,
                    JobsThatCanUse = new List<string>{"MNK", "PUP", "DNC", "WAR", "THF", "NIN"},
                    SubJobsThatCanUse = new List<string>{"MNK", "PUP", "DNC", "WAR", "THF", "NIN"},
                    Special=""}},
            {"Howling Fist", new WSManualInfo{
                    SkillLevel = 200,
                    JobsThatCanUse = new List<string>{"MNK", "PUP"},
                    SubJobsThatCanUse = new List<string>{},
                    Special=""}},
            {"Dragon Kick", new WSManualInfo{
                    SkillLevel = 225,
                    JobsThatCanUse = new List<string>{"MNK", "PUP"},
                    SubJobsThatCanUse = new List<string>{},
                    Special=""}},
            {"Asuran Fists", new WSManualInfo{
                    SkillLevel = 250,
                    JobsThatCanUse = new List<string>{"MNK", "PUP"},
                    SubJobsThatCanUse = new List<string>{},
                    Special="Quest"}},
            {"Final Heaven", new WSManualInfo{
                    SkillLevel = 0,
                    JobsThatCanUse = new List<string>{"MNK", "PUP"},
                    SubJobsThatCanUse = new List<string>{},
                    Special="Relic"}},
            {"Ascetic's Fury", new WSManualInfo{
                    SkillLevel = 0,
                    JobsThatCanUse = new List<string>{"MNK"},
                    SubJobsThatCanUse = new List<string>{},
                    Special="Mythic"}},
            {"Stringing Pummel", new WSManualInfo{
                    SkillLevel = 0,
                    JobsThatCanUse = new List<string>{"PUP"},
                    SubJobsThatCanUse = new List<string>{},
                    Special="Mythic"}},
            {"Tornado Kick", new WSManualInfo{
                    SkillLevel = 300,
                    JobsThatCanUse = new List<string>{"MNK", "PUP"},
                    SubJobsThatCanUse = new List<string>{"MNK", "PUP"},
                    Special=""}},
            {"Victory Smite", new WSManualInfo{
                    SkillLevel = 0,
                    JobsThatCanUse = new List<string>{"MNK", "PUP"},
                    SubJobsThatCanUse = new List<string>{},
                    Special="Empyrean"}},
            {"Shijin Spiral", new WSManualInfo{
                    SkillLevel = 357,
                    JobsThatCanUse = new List<string>{"MNK", "PUP"},
                    SubJobsThatCanUse = new List<string>{},
                    Special="Merit"}
            }
        };
    }
}
