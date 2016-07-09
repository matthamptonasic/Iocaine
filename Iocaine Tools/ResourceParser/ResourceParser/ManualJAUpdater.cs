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
    public partial class ManualJAUpdater : Form
    {
        #region Constructor
        public ManualJAUpdater()
        {
            InitializeComponent();
            FFXIEnums.JOBS job;
            for(int ii=0; ii<=20; ii++)
            {
                job = (FFXIEnums.JOBS)ii;
                JAUpdaterJobCB.Items.Add(job.ToString());
            }
        }
        #endregion Constructor
        #region Members
        private DataRow[] rowArray = null;
        private int currentIndex = 0;
        #endregion Members
        #region Member Functions
        public void setItems(DataRow [] iRowArray)
        {
            rowArray = iRowArray;
            loadRecord(0);
        }
        private void loadRecord(int index)
        {
            JAUpdaterNameTB.Text = ParserForm.decodeApostrophy(rowArray[index]["Name"].ToString());
            JAUpdaterIDTB.Text = rowArray[index]["ID"].ToString();
            JAUpdaterIndexTB.Text = rowArray[index]["Idx"].ToString();
            JAUpdaterCommandTB.Text = ParserForm.decodeApostrophy(rowArray[index]["Command"].ToString());
            JAUpdaterMeOnlyChkB.Checked = Convert.ToBoolean(rowArray[index]["MeOnly"]);
            JAUpdaterOutsideChkB.Checked = Convert.ToBoolean(rowArray[index]["Outside"]);
            JAUpdaterTargetTB.Text = rowArray[index]["Target"].ToString();
            JAUpdaterJobCB.SelectedIndex = Convert.ToInt32(rowArray[index]["Job"]);
            JAUpdaterJobLevelTB.Text = rowArray[index]["JobLevel"].ToString();
            JAUpdaterAsSubChkB.Checked = Convert.ToBoolean(rowArray[index]["AsSub"]);
            JAUpdaterMPTB.Text = rowArray[index]["MP"].ToString();
            JAUpdaterDurationTB.Text = rowArray[index]["Duration"].ToString();
        }
        #endregion Member Functions
        #region Event Handlers
        private void JAUpdaterCloseButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void JAUpdaterPreviousButton_Click(object sender, EventArgs e)
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
        private void JAUpdaterNextButton_Click(object sender, EventArgs e)
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
        private void JAUpdaterSaveCompleteButton_Click(object sender, EventArgs e)
        {
            rowArray[currentIndex]["RecComplete"] = true;
            if (currentIndex < rowArray.Length - 1)
            {
                loadRecord(++currentIndex);
            }
        }
        private void JAUpdaterCommandTB_TextChanged(object sender, EventArgs e)
        {
            if (JAUpdaterCommandTB.Text != "")
            {
                rowArray[currentIndex]["Command"] = ParserForm.encodeApostrophy(JAUpdaterCommandTB.Text);
            }
        }
        private void JAUpdaterMeOnlyChkB_CheckedChanged(object sender, EventArgs e)
        {
            rowArray[currentIndex]["MeOnly"] = JAUpdaterMeOnlyChkB.Checked;
        }
        private void JAUpdaterOutsideChkB_CheckedChanged(object sender, EventArgs e)
        {
            rowArray[currentIndex]["Outside"] = JAUpdaterOutsideChkB.Checked;
        }
        private void JAUpdaterTargetTB_TextChanged(object sender, EventArgs e)
        {
            if (JAUpdaterTargetTB.Text != "")
            {
                rowArray[currentIndex]["Target"] = JAUpdaterTargetTB.Text;
            }
        }
        private void JAUpdaterJobLevelTB_TextChanged(object sender, EventArgs e)
        {
            if (JAUpdaterJobLevelTB.Text != "")
            {
                rowArray[currentIndex]["JobLevel"] = JAUpdaterJobLevelTB.Text;
            }
        }
        private void JAUpdaterJobCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            rowArray[currentIndex]["Job"] = JAUpdaterJobCB.SelectedIndex;
        }
        private void JAUpdaterAsSubChkB_CheckedChanged(object sender, EventArgs e)
        {
            rowArray[currentIndex]["AsSub"] = JAUpdaterAsSubChkB.Checked;
        }
        private void JAUpdaterMPTB_TextChanged(object sender, EventArgs e)
        {
            if (JAUpdaterMPTB.Text != "")
            {
                rowArray[currentIndex]["MP"] = JAUpdaterMPTB.Text;
            }
        }
        private void JAUpdaterDurationTB_TextChanged(object sender, EventArgs e)
        {
            if (JAUpdaterDurationTB.Text != "")
            {
                rowArray[currentIndex]["Duration"] = JAUpdaterDurationTB.Text;
            }
        }
        #endregion Event Handlers
    }
}
