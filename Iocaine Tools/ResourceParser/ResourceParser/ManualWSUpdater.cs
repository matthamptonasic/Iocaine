using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2;

namespace ResourceParser
{
    public partial class ManualWSUpdater : Form
    {
        #region Constructor
        public ManualWSUpdater()
        {
            InitializeComponent();
            doingInit = true;
            FFXIEnums.WEAPON_SKILL_TYPE type;
            for (int ii = 0; ii <= 12; ii++)
            {
                type = (FFXIEnums.WEAPON_SKILL_TYPE)ii;
                WSUpdaterSkillTypeCB.Items.Add(type.ToString());
                skillTypeMap.Add((Byte)type);
            }
            WSUpdaterSkillTypeCB.Items.Add(FFXIEnums.WEAPON_SKILL_TYPE.ARCHERY.ToString());
            skillTypeMap.Add((Byte)FFXIEnums.WEAPON_SKILL_TYPE.ARCHERY);
            WSUpdaterSkillTypeCB.Items.Add(FFXIEnums.WEAPON_SKILL_TYPE.MARKSMANSHIP.ToString());
            skillTypeMap.Add((Byte)FFXIEnums.WEAPON_SKILL_TYPE.MARKSMANSHIP);
            WSUpdaterSkillTypeCB.Items.Add(FFXIEnums.WEAPON_SKILL_TYPE.THROWING.ToString());
            skillTypeMap.Add((Byte)FFXIEnums.WEAPON_SKILL_TYPE.THROWING);
            FFXIEnums.WS_ATTRIBUTES attr;
            for (int ii = 0; ii <= 14; ii++)
            {
                attr = (FFXIEnums.WS_ATTRIBUTES)ii;
                WSUpdaterAttrACB.Items.Add(attr.ToString());
                WSUpdaterAttrBCB.Items.Add(attr.ToString());
                WSUpdaterAttrCCB.Items.Add(attr.ToString());
            }
            doingInit = false;
        }
        #endregion Constructor
        #region Members
        private bool doingInit = false;
        private DataRow[] rowArray = null;
        private int currentIndex = 0;
        private List<Byte> skillTypeMap = new List<byte>();
        #endregion Members
        #region Member Functions
        public void setItems(DataRow[] iRowArray)
        {
            rowArray = iRowArray;
            loadRecord(0);
        }
        private void loadRecord(int index)
        {
            WSUpdaterNameTB.Text = rowArray[index]["Name"].ToString();
            WSUpdaterCommandTB.Text = rowArray[index]["Command"].ToString();
            WSUpdaterIDTB.Text = rowArray[index]["ID"].ToString();
            WSUpdaterSkillLevelTB.Text = rowArray[index]["SkillLevel"].ToString();
            WSUpdaterSkillTypeCB.SelectedIndex = skillTypeMap.IndexOf(Convert.ToByte(rowArray[index]["SkillType"]));
            WSUpdaterAttrACB.SelectedIndex = Convert.ToInt32(rowArray[index]["AttrA"]);
            WSUpdaterAttrBCB.SelectedIndex = Convert.ToInt32(rowArray[index]["AttrB"]);
            WSUpdaterAttrCCB.SelectedIndex = Convert.ToInt32(rowArray[index]["AttrC"]);
            UInt32 jobs = Convert.ToUInt32(rowArray[index]["Jobs"]);
            Console.WriteLine("Found jobs: " + jobs);
            for (int ii = 1; ii <= 20; ii++)
            {
                //find the proper checkbox and check it.
                CheckBox chkB = (CheckBox)WSUpdaterMainJobsPanel.Controls[WSUpdaterMainJobsPanel.Controls.IndexOfKey("WSUpdaterJob" + ii + "ChkB")];
                //Console.WriteLine("Checkbox name: " + chkB.Name);
                chkB.Checked = ((jobs & (0x1 << ii)) != 0) ? true : false;
            }
            UInt32 subJobs = Convert.ToUInt32(rowArray[index]["JobsSubs"]);
            Console.WriteLine("Found sub jobs: " + subJobs);
            for (int ii = 1; ii <= 20; ii++)
            {
                //find the proper checkbox and check it.
                CheckBox chkB = (CheckBox)WSUpdaterSubJobsPanel.Controls[WSUpdaterSubJobsPanel.Controls.IndexOfKey("WSUpdaterSubJob" + ii + "ChkB")];
                //Console.WriteLine("Checkbox name: " + chkB.Name);
                chkB.Checked = ((subJobs & (0x1 << ii)) != 0) ? true : false;
            }
        }
        #endregion Member Functions
        #region Event Handlers
        private void WSUpdaterCloseButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void WSUpdaterPreviousButton_Click(object sender, EventArgs e)
        {
            if (currentIndex > 0)
            {
                loadRecord(--currentIndex);
            }
            else
            {
                currentIndex = rowArray.Length - 1;
                loadRecord(currentIndex);
            }
        }
        private void WSUpdaterNextButton_Click(object sender, EventArgs e)
        {
            if (currentIndex < rowArray.Length - 1)
            {
                loadRecord(++currentIndex);
            }
            else
            {
                currentIndex = 0;
                loadRecord(currentIndex);
            }
        }
        private void WSUpdaterSaveCompleteButton_Click(object sender, EventArgs e)
        {
            rowArray[currentIndex]["RecComplete"] = true;
            if (currentIndex < rowArray.Length - 1)
            {
                loadRecord(++currentIndex);
            }
        }
        private void WSUpdaterSkillLevelTB_TextChanged(object sender, EventArgs e)
        {
            if (WSUpdaterSkillLevelTB.Text != "")
            {
                rowArray[currentIndex]["SkillLevel"] = WSUpdaterSkillLevelTB.Text;
            }
        }

        private void WSUpdaterSkillTypeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            rowArray[currentIndex]["SkillType"] = skillTypeMap[WSUpdaterSkillTypeCB.SelectedIndex];
        }

        private void WSUpdaterAttrACB_SelectedIndexChanged(object sender, EventArgs e)
        {
            rowArray[currentIndex]["AttrA"] = WSUpdaterAttrACB.SelectedIndex;
        }

        private void WSUpdaterAttrBCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            rowArray[currentIndex]["AttrB"] = WSUpdaterAttrBCB.SelectedIndex;
        }

        private void WSUpdaterAttrCCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            rowArray[currentIndex]["AttrC"] = WSUpdaterAttrCCB.SelectedIndex;
        }

        private void WSUpdaterJobXChkB_CheckedChanged(object sender, EventArgs e)
        {
            if (doingInit)
            {
                return;
            }
            CheckBox sentFromChkB = (CheckBox)sender;
            bool chkd = sentFromChkB.Checked;
            int startIndex = 12;
            int endIndex = sentFromChkB.Name.IndexOf("ChkB");
            Byte boxIndex = Byte.Parse(sentFromChkB.Name.Substring(startIndex, endIndex - startIndex));
            if (chkd)
            {
                rowArray[currentIndex]["Jobs"] = (UInt32)rowArray[currentIndex]["Jobs"] | (UInt32)(1 << boxIndex);
                
            }
            else
            {
                rowArray[currentIndex]["Jobs"] = (UInt32)rowArray[currentIndex]["Jobs"] & (UInt32)~(1 << boxIndex);
                //Also uncheck the sub job box.
                String subJobBoxName = sentFromChkB.Name.Replace("Job", "SubJob");
                Console.WriteLine("Box we clicked was: " + sentFromChkB.Name + ", box we're trying to check is: " + subJobBoxName);
                ((CheckBox)WSUpdaterSubJobsPanel.Controls[WSUpdaterSubJobsPanel.Controls.IndexOfKey(subJobBoxName)]).Checked = false;
            }
        }
        //WSUpdaterSubJob15ChkB
        private void WSUpdaterSubJobXChkB_CheckedChanged(object sender, EventArgs e)
        {
            if (doingInit)
            {
                return;
            }
            CheckBox sentFromChkB = (CheckBox)sender;
            bool chkd = sentFromChkB.Checked;
            int startIndex = 15;
            int endIndex = sentFromChkB.Name.IndexOf("ChkB");
            Byte boxIndex = Byte.Parse(sentFromChkB.Name.Substring(startIndex, endIndex - startIndex));
            if (chkd)
            {
                rowArray[currentIndex]["JobsSubs"] = (UInt32)rowArray[currentIndex]["JobsSubs"] | (UInt32)(1 << boxIndex);
                //Also clear the main job box
                String mainJobBoxName = sentFromChkB.Name.Replace("SubJob", "Job");
                ((CheckBox)WSUpdaterMainJobsPanel.Controls[WSUpdaterMainJobsPanel.Controls.IndexOfKey(mainJobBoxName)]).Checked = true;
            }
            else
            {
                rowArray[currentIndex]["JobsSubs"] = (UInt32)rowArray[currentIndex]["JobsSubs"] & (UInt32)~(1 << boxIndex);
            }
        }
        #endregion Event Handlers
    }
}