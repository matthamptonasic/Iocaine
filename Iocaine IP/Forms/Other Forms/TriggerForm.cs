using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Char;
using Iocaine2.Data.Client;
using Iocaine2.Data.Structures;
using Iocaine2.Logging;

namespace Iocaine2
{
    internal partial class TriggerForm : Form
    {
        #region Constructors
        internal TriggerForm(List<Character> iPlayerList, FFXIEnums.JOBS iMainJob, FFXIEnums.JOBS iSubJob, Byte iMainJobLevel, Byte iSubJobLevel)
        {
            mainJob = iMainJob;
            subJob = iSubJob;
            mainJobLevel = iMainJobLevel;
            subJobLevel = iSubJobLevel;
            InitializeComponent();
            init(iPlayerList);
        }
        #endregion Constructors
        #region Inits
        private void init(List<Character> iPlayerList)
        {
            doingInits = true;
            playerList = iPlayerList;

            //Set up trigger type CB
            TF_TriggerTypeCB.Items.Add("None");
            for (int ii = 1; ii < (UInt16)TRIG_TYPE.Count; ii++)
            {
                TF_TriggerTypeCB.Items.Add(((TRIG_TYPE)ii).ToString().Replace('_', ' '));
            }
            TF_TriggerTypeCB.SelectedIndex = 0;

            //Set up action type CB
            TF_ActionTypeCB.Items.Add("None");
            for (int ii = 1; ii < (UInt16)ACTN_TYPE.Count; ii++)
            {
                TF_ActionTypeCB.Items.Add(((ACTN_TYPE)ii).ToString().Replace('_', ' '));
            }
            TF_ActionTypeCB.SelectedIndex = 0;

            panelDefaultSize = TF_TriggerPanel.Size;
            windowDefaultHeight = this.Size.Height;
            initPlayerStringList();
            initStatusInfo();
            initTargetStrings();
            Data.Client.FfxiResource.init();
            doingInits = false;
        }
        private void initPlayerStringList()
        {
            playerStringList = new List<string>();
            foreach (Character chr in playerList)
            {
                playerStringList.Add(chr.Name);
            }
        }
        private void initStatusInfo()
        {
            playerStatusStrings.Add("Any");
            playerStatusMap.Add("Any", 255);
            playerStatusStrings.Add("Idle");
            playerStatusMap.Add("Idle", (Byte)FFXIEnums.STATUS.NORMAL);
            playerStatusStrings.Add("Attacking");
            playerStatusMap.Add("Attacking", (Byte)FFXIEnums.STATUS.ATTACKING);
            playerStatusStrings.Add("KO");
            playerStatusMap.Add("KO", (Byte)FFXIEnums.STATUS.KO1);
            playerStatusStrings.Add("Chocobo");
            playerStatusMap.Add("Chocobo", (Byte)FFXIEnums.STATUS.CHOCO);
            playerStatusStrings.Add("Resting");
            playerStatusMap.Add("Resting", (Byte)FFXIEnums.STATUS.HEALING);
            playerStatusStrings.Add("Synthing");
            playerStatusMap.Add("Synthing", (Byte)FFXIEnums.STATUS.SYNTHING);
            playerStatusStrings.Add("Fishing");
            playerStatusMap.Add("Fishing", (Byte)FFXIEnums.STATUS.FISHING);
            playerStatusStrings.Add("Sitting");
            playerStatusMap.Add("Sitting", (Byte)FFXIEnums.STATUS.SITTING);
        }
        private void initTargetStrings()
        {
            targetStringList = new List<string>();
            targetStringList.Add("<t>");
            targetStringList.Add("<me>");
            targetStringList.Add("<bt>");
            targetStringList.AddRange(playerStringList);
        }
        #endregion Inits
        #region Member Variables
        bool doingInits;
        internal delegate void TriggerSaveFunction(Trigger iTrigger);
        internal event TriggerSaveFunction _SaveTrigger;
        private FFXIEnums.JOBS mainJob;
        private FFXIEnums.JOBS subJob;
        private Byte mainJobLevel;
        private Byte subJobLevel;
        private List<Character> playerList;
        private List<String> playerStringList;
        private Size panelDefaultSize;
        private int windowDefaultHeight;
        private int labelHeight = 16;
        private int boxHeight = 22;
        private List<String> playerStatusStrings = new List<string>();
        private Dictionary<String, Byte> playerStatusMap = new Dictionary<string, byte>();
        private List<String> targetStringList = new List<string>();
        private ActionSequence sequence = new ActionSequence();
        #endregion Member Variables


        #region Functions
        #region Trigger Creation & Parsing
        #region Trigger Parsing
        private Trigger saveNewTrigger()
        {
            switch (TF_TriggerTypeCB.SelectedIndex)
            {
                case (int)TRIG_TYPE.Unknown:
                    return null;
                case (int)TRIG_TYPE.Player_Status_Change:
                    return parsePlayerStatusChangeInfo();
                default:
                    return null;
            }
        }
        private TriggerPlayerStatusChange parsePlayerStatusChangeInfo()
        {
            String playerName = findStringFromCBInPanel(TF_TriggerPanel, "Player");
            Byte oldStatus = findStatusInPanel(TF_TriggerPanel, "PreviousStatus");
            Byte newStatus = findStatusInPanel(TF_TriggerPanel, "NewStatus");
            TriggerPlayerStatusChange trigger = new TriggerPlayerStatusChange(playerName, oldStatus, newStatus, true, true);
            return trigger;
        }
        #endregion Trigger Parsing
        #region Action Parsing
        private Data.Structures.Action parseActionInfo()
        {
            Data.Structures.Action actn = null;
            switch (TF_ActionTypeCB.SelectedIndex)
            {
                //case (int)ActionType.Cast:
                //    String spellName = findStringFromCBInPanel(TF_ActionPanel, "Spell");
                //    String target = findStringFromCBInPanel(TF_ActionPanel, "Target");
                //    actn = new ActionCast(spellName, target);
                //    return actn;
                default:
                    return null;
            }
        }
        #endregion Action Parsing
        #region Form Parsing
        private String findStringFromCBInPanel(Panel iPanel, String iText)
        {
            Control[] controlArray = iPanel.Controls.Find(iText + "CB", false);
            if (controlArray.Length != 1)
            {
                LoggingFunctions.Error("Found unexpected number " + controlArray.Length + " of controls named " + iText + "CB");
                return "";
            }
            else
            {
                ComboBox stringCB = (ComboBox)controlArray[0];
                return (String)stringCB.SelectedItem;
            }
        }
        private Byte findStatusInPanel(Panel iPanel, String iText)
        {
            String statusName = findStringFromCBInPanel(iPanel, iText);
            if (statusName != "")
            {
                return playerStatusMap[statusName];
            }
            else
            {
                return 254;
            }
        }
        #endregion Form Parsing
        #endregion Trigger Creation & Parsing
        #region Trigger Controls
        private void setTriggerDefaultControls()
        {
            TF_TriggerPanel.Controls.Clear();
            TF_ActionPanel.Size = panelDefaultSize;
            recalculateSizes();
        }
        private void setTriggerPlayerStatusChangeControls()
        {
            int currentInsertionPoint = 5;
            //Clear out any controls in the panel.
            setTriggerDefaultControls();
            //Create Players Controls
            addCBToPanel(TF_TriggerPanel, "Player", ref currentInsertionPoint, playerStringList);
            //Add Old Status
            addCBToPanel(TF_TriggerPanel, "Previous Status", ref currentInsertionPoint, playerStatusStrings);
            //Add New Status
            addCBToPanel(TF_TriggerPanel, "New Status", ref currentInsertionPoint, playerStatusStrings);
            //Finally, resize everything to fit.
            recalculateSizes();
        }
        #endregion Trigger Controls
        #region Action Controls
        private void setActionDefaultControls()
        {
            TF_ActionPanel.Controls.Clear();
            TF_ActionPanel.Size = panelDefaultSize;
            recalculateSizes();
        }
        private void setActionCastControls()
        {
            int currentInsertionPoint = 5;
            //Clear out any controls in the panel.
            setActionDefaultControls();
            List<Spells.SPELL_INFO> spellInfoList = Spells.GetSpellInfo((Byte)mainJob, (Byte)subJob, mainJobLevel);
            List<String> spellNameList = new List<string>();
            foreach (Spells.SPELL_INFO info in spellInfoList)
            {
                spellNameList.Add(info.Name);
            }
            addCBToPanel(TF_ActionPanel, "Spell", ref currentInsertionPoint, spellNameList);
            addCBToPanel(TF_ActionPanel, "Target", ref currentInsertionPoint, targetStringList);

            recalculateSizes();
        }
        #endregion Action Controls
        #region Resizing
        private void recalculateSizes()
        {
            int triggerPanelHeight = 10;
            int actionPanelHeight = 10;
            foreach (Control cntrl in TF_TriggerPanel.Controls)
            {
                //LoggingFunctions.timestamp("cntrl name: " + cntrl.Name + ", type: " + cntrl.GetType());
                if ((cntrl is ComboBox) || (cntrl is TextBox))
                {
                    triggerPanelHeight += 44;
                }
            }
            foreach (Control cntrl in TF_ActionPanel.Controls)
            {
                //LoggingFunctions.timestamp("cntrl name: " + cntrl.Name + ", type: " + cntrl.GetType());
                if ((cntrl is ComboBox) || (cntrl is TextBox))
                {
                    actionPanelHeight += 44;
                }
            }
            int largerHeight = triggerPanelHeight > actionPanelHeight ? triggerPanelHeight : actionPanelHeight;
            if (largerHeight > panelDefaultSize.Height)
            {
                TF_TriggerPanel.Size = new Size(TF_TriggerPanel.Size.Width, largerHeight);
                TF_ActionPanel.Size = new Size(TF_ActionPanel.Size.Width, largerHeight);
                this.Size = new Size(this.Size.Width, windowDefaultHeight + (largerHeight - panelDefaultSize.Height));
            }
            else
            {
                TF_TriggerPanel.Size = panelDefaultSize;
                TF_ActionPanel.Size = panelDefaultSize;
                this.Size = new Size(this.Size.Width, windowDefaultHeight);
            }
        }
        #endregion Resizing
        #region Control Utility Functions
        private void addStatusToCB(ComboBox iComboBox)
        {
            iComboBox.Items.AddRange(playerStatusStrings.ToArray());
        }
        private void addPlayersToCB(ComboBox iComboBox)
        {
            foreach (Character chr in playerList)
            {
                iComboBox.Items.Add(chr.Name);
            }
        }
        private void addCBToPanel(Panel iPanel, String iText, ref int iCurrentInsertionPoint, List<String> iContents)
        {
            Label newLabel = new Label();
            newLabel.Text = iText;
            newLabel.Name = iText.Replace(" ", "") + "Label";
            newLabel.Location = new Point(5, iCurrentInsertionPoint);
            iCurrentInsertionPoint += labelHeight;
            iPanel.Controls.Add(newLabel);
            ComboBox newCB = new ComboBox();
            newCB.Items.AddRange(iContents.ToArray());
            newCB.Name = iText.Replace(" ", "") + "CB";
            newCB.Location = new Point(5, iCurrentInsertionPoint);
            iCurrentInsertionPoint += boxHeight;
            if (newCB.Items.Count > 0)
            {
                newCB.SelectedIndex = 0;
            }
            iPanel.Controls.Add(newCB);
            newCB.BringToFront();
        }
        #endregion Control Utility Functions
        #endregion Functions
        #region Events
        private void TF_CancelButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void TF_OKButton_Click(object sender, EventArgs e)
        {
            _SaveTrigger(saveNewTrigger());
            this.Dispose();
        }
        private void TF_ApplyButton_Click(object sender, EventArgs e)
        {
            _SaveTrigger(saveNewTrigger());
        }
        private void TF_AddActionButton_Click(object sender, EventArgs e)
        {
            Data.Structures.Action actn = parseActionInfo();
            sequence.AddAction(actn);
            TF_ActionSequenceLB.Items.Add(actn.ToString());
        }
        private void TF_TriggerTypeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!doingInits)
            {
                switch (TF_TriggerTypeCB.SelectedIndex)
                {
                    case (int)TRIG_TYPE.Unknown:
                        setTriggerDefaultControls();
                        break;
                    case (int)TRIG_TYPE.Player_Status_Change:
                        setTriggerPlayerStatusChangeControls();
                        break;
                    default:
                        break;
                }
            }
        }
        private void TF_ActionTypeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!doingInits)
            {
                switch (TF_ActionTypeCB.SelectedIndex)
                {
                    case (int)ACTN_TYPE.Unknown:
                        setActionDefaultControls();
                        break;
                    //case (int)ActionType.Cast:
                    //    setActionCastControls();
                    //    break;
                    default:
                        break;
                }
            }
        }
        #endregion Events
    }
}
