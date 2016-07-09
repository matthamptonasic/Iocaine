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
    public partial class ManualSpellUpdater : Form
    {
        #region Constructor
        public ManualSpellUpdater()
        {
            InitializeComponent();
            doingInit = true;
            FFXIEnums.ELEMENT element;
            for (int ii = 0; ii <= 8; ii++)
            {
                element = (FFXIEnums.ELEMENT)ii;
                SpellUpdaterElementCB.Items.Add(element.ToString());
            }
            doingInit = false;
        }
        #endregion Constructor
        #region Members
        private bool doingInit = false;
        private DataRow[] rowArray = null;
        private int currentIndex = 0;
        #endregion Members
        #region Member Functions
        public void setItems(DataRow[] iRowArray)
        {
            rowArray = iRowArray;
            loadRecord(0);
        }
        private void loadRecord(int index)
        {
            SpellUpdaterNameTB.Text = rowArray[index]["Name"].ToString();
            SpellUpdaterCommandTB.Text = rowArray[index]["Command"].ToString();
            SpellUpdaterIDTB.Text = rowArray[index]["ID"].ToString();
            SpellUpdaterIndexTB.Text = rowArray[index]["Idx"].ToString();
            SpellUpdaterTypeTB.Text= rowArray[index]["Type"].ToString();
            SpellUpdaterElementCB.SelectedIndex = Convert.ToInt32(rowArray[index]["Element"]);
            SpellUpdaterMpTB.Text = rowArray[index]["MP"].ToString();
            SpellUpdaterCastTimeTB.Text = rowArray[index]["CastTime"].ToString();
            SpellUpdaterDurationTB.Text = rowArray[index]["Duration"].ToString();
            SpellUpdaterMeOnlyChkB.Checked = Convert.ToBoolean(rowArray[index]["MeOnly"]);
            SpellUpdaterOutsideChkB.Checked = Convert.ToBoolean(rowArray[index]["Outside"]);

            UInt32 jobs = Convert.ToUInt32(rowArray[index]["Jobs"]);
            Console.WriteLine("Found jobs: " + jobs);
            for (int ii = 1; ii <= 20; ii++)
            {
                //find the proper checkbox and check it.
                CheckBox chkB = (CheckBox)SpellUpdaterMainJobsPanel.Controls[SpellUpdaterMainJobsPanel.Controls.IndexOfKey("SpellUpdaterJob" + ii + "ChkB")];
                //Console.WriteLine("Checkbox name: " + chkB.Name);
                chkB.Checked = ((jobs & (0x1 << ii)) != 0) ? true : false;
            }
            SpellUpdaterAsSubChkB.Checked = Convert.ToBoolean(rowArray[index]["AsSub"]);
            for (int ii = 1; ii <= 20; ii++)
            {
                //fill in each job level
                TextBox txtBox = (TextBox)SpellUpdaterMainJobsPanel.Controls[SpellUpdaterMainJobsPanel.Controls.IndexOfKey("SpellUpdaterJob" + ii + "TB")];
                //Console.WriteLine("Checkbox name: " + chkB.Name);
                txtBox.Text = rowArray[index][("LevelJob" + ii)].ToString();
            }
        }
        #endregion Member Functions
        #region Event Handlers
        private void SpellUpdaterCloseButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void SpellUpdaterPreviousButton_Click(object sender, EventArgs e)
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
        private void SpellUpdaterNextButton_Click(object sender, EventArgs e)
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
        private void SpellUpdaterSaveCompleteButton_Click(object sender, EventArgs e)
        {
            rowArray[currentIndex]["RecComplete"] = true;
            if (currentIndex < rowArray.Length - 1)
            {
                loadRecord(++currentIndex);
            }
        }
        private void SpellUpdaterElementCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            rowArray[currentIndex]["Element"] = SpellUpdaterElementCB.SelectedIndex;
        }
        private void SpellUpdaterMpTB_TextChanged(object sender, EventArgs e)
        {
            if (SpellUpdaterMpTB.Text != "")
            {
                try
                {
                    rowArray[currentIndex]["MP"] = SpellUpdaterMpTB.Text;
                }
                catch
                { }
            }
        }
        private void SpellUpdaterCastTimeTB_TextChanged(object sender, EventArgs e)
        {
            if (SpellUpdaterCastTimeTB.Text != "")
            {
                try
                {
                    rowArray[currentIndex]["CastTime"] = SpellUpdaterCastTimeTB.Text;
                }
                catch
                { }
            }
        }
        private void SpellUpdaterDurationTB_TextChanged(object sender, EventArgs e)
        {
            if (SpellUpdaterDurationTB.Text != "")
            {
                try
                {
                    rowArray[currentIndex]["Duration"] = SpellUpdaterDurationTB.Text;
                }
                catch
                { }
            }
        }
        private void SpellUpdaterMeOnlyChkB_CheckedChanged(object sender, EventArgs e)
        {
            rowArray[currentIndex]["MeOnly"] = SpellUpdaterMeOnlyChkB.Checked;
        }
        private void SpellUpdaterOutsideChkB_CheckedChanged(object sender, EventArgs e)
        {
            rowArray[currentIndex]["Outside"] = SpellUpdaterOutsideChkB.Checked;
        }
        private void SpellUpdaterAsSubChkB_CheckedChanged(object sender, EventArgs e)
        {
            rowArray[currentIndex]["AsSub"] = SpellUpdaterAsSubChkB.Checked;
        }
        private void SpellUpdaterJobXChkB_CheckedChanged(object sender, EventArgs e)
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
                rowArray[currentIndex]["Jobs"] = (UInt32)rowArray[currentIndex]["Jobs"] | (UInt32)(1 << boxIndex);
                
            }
            else
            {
                rowArray[currentIndex]["Jobs"] = (UInt32)rowArray[currentIndex]["Jobs"] & (UInt32)~(1 << boxIndex);
            }
        }
        private void SpellUpdaterJobXTB_TextChanged(object sender, EventArgs e)
        {
            if (doingInit)
            {
                return;
            }
            TextBox sentFromTB = (TextBox)sender;
            int startIndex = 15;
            int endIndex = sentFromTB.Name.IndexOf("TB");
            String boxIndex = sentFromTB.Name.Substring(startIndex, endIndex - startIndex);
            if (sentFromTB.Text != "")
            {
                try
                {
                    rowArray[currentIndex][("LevelJob" + boxIndex)] = Byte.Parse(sentFromTB.Text);
                }
                catch
                { }
            }
        }
        #endregion Event Handlers
    }
}
