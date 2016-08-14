using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Iocaine2.Data.Entry
{
    public partial class DataEntry : Form
    {
        #region Private Members
        private List<ControlParameter> m_controlParams;
        private List<ControlReturn> m_controlReturns = new List<ControlReturn>();
        #region Form
        private int m_formWidth = 200;
        private int m_formHeight = 70;
        private int m_lastX = m_labelStartX;
        private int m_lastY = 12;
        private int Ht
        {
            get
            {
                return this.Height;
            }
            set
            {
                int temp = value;
                m_lastY += temp - this.Height;
                this.Height = temp;
            }
        }
        private int m_controlCount = 0;
        #endregion Form
        #region Buttons
        private const int m_buttonWidth = 75;
        private const int m_buttonHeight = 25;
        private const int m_buttonTotalHeight = 30;
        private const string m_buttonOkStr = "OK";
        private const string m_buttonCancelStr = "Cancel";
        private Button m_ok;
        private Button m_cancel;
        private bool m_focusOk = false;
        #endregion Buttons
        #region Labels
        private const int m_labelHeight = 20;
        private const int m_labelStartX = 25;
        #endregion Labels
        #region Textboxes
        private const int m_textboxHeight = 20;
        private const int m_textboxWidth = 130;
        #endregion Textboxes
        #region Combo Boxes
        private const int m_comboBoxHeight = 20;
        private const int m_comboBoxWidth = 130;
        #endregion Combo Boxes
        #region Up Downs
        private const int m_upDownHeight = 20;
        private const int m_upDownWidth = 130;
        #endregion Up Downs
        #endregion Private Members

        #region Public Properties
        public List<ControlReturn> ControlReturns
        {
            get
            {
                return m_controlReturns;
            }
        }
        #endregion Public Properties

        #region Constructor
        public DataEntry(Form iOwner, List<ControlParameter> iControlParams)
        {
            m_controlParams = iControlParams;
            this.Owner = iOwner;
            initFormSettings();

            foreach (ControlParameter param in iControlParams)
            {
                switch (param.GetType().Name)
                {
                    case "ComboBoxParameter":
                        break;
                    case "TextboxParameter":
                        addTextbox((TextboxParameter)param);
                        break;
                    case "UpDownParameter":
                        break;
                    default:
                        break;
                }
            }
            addButtons();

            InitializeComponent();
        }
        #endregion Constructor

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        private void initFormSettings()
        {
            this.Width = m_formWidth;
            this.Height = m_formHeight;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            int startX = this.Owner.Location.X + (this.Owner.Width + 20) / 2 - (this.Width + 20) / 2;
            int startY = this.Owner.Location.Y + (this.Owner.Height + 20) / 2 - (this.Height + 20) / 2;
            this.StartPosition = FormStartPosition.CenterParent;
        }
        private void addButtons()
        {
            m_ok = new Button();
            m_ok.Size = new Size(m_buttonWidth, m_buttonHeight);
            m_ok.Text = m_buttonOkStr;
            m_ok.Location = new Point(this.Width / 2 + 2 - 10, this.Height - m_buttonTotalHeight - 14);
            this.Controls.Add(m_ok);
            m_ok.Click += ok_Click;

            m_cancel = new Button();
            m_cancel.Size = new Size(m_buttonWidth, m_buttonHeight);
            m_cancel.Text = m_buttonCancelStr;
            m_cancel.Location = new Point(this.Width / 2 - m_buttonWidth - 2 - 10, this.Height - m_buttonTotalHeight - 14);
            this.CancelButton = m_cancel;
            this.Controls.Add(m_cancel);

            Ht += m_buttonTotalHeight;

            if (m_focusOk)
            {
                dataSet();
            }
        }
        private void addComboBox()
        {

        }
        private void addTextbox(TextboxParameter iParam)
        {
            Label lbl = new Label();
            lbl.Text = iParam.LabelText;
            lbl.Location = new Point(m_lastX, m_lastY);
            this.Controls.Add(lbl);
            lbl.SendToBack();
            Ht += m_labelHeight;

            IocaineTextbox tb = new IocaineTextbox();
            tb.TabIndex = m_controlCount++;
            tb.TabStop = true;
            tb.DefaultText = iParam.DefaultText;
            tb.GrayTextIfDefault = iParam.TextGreyed;
            if (iParam.OnEnterSetTarget)
            {
                tb._DataEntered += dataSet;
            }
            tb.OnEnterSetTarget = iParam.OnEnterSetTarget;
            tb.Width = m_textboxWidth;
            tb.Location = new Point(m_lastX, m_lastY);
            this.Controls.Add(tb);
            tb.BringToFront();
            Ht += m_textboxHeight;

            iParam.Control = tb;

            return;
        }
        private void addUpDown(UpDownParameter iParam)
        {

        }
        private void dataSet()
        {
            if (m_ok != null)
            {
                // .Focus only works if the form is already drawn.
                // If we're trying to set the focus BEFORE it is drawn, we have
                // to set the tab index to 0 so that when it is drawn, it will be focused.
                bubbleOkTabStop();
                m_ok.Focus();
                m_focusOk = false;
            }
            else
            {
                m_focusOk = true;
            }
        }
        private void bubbleOkTabStop()
        {
            if (Controls[0].TabIndex > 0)
            {
                return;
            }
            foreach (Control ctrl in Controls)
            {
                if (ctrl == m_ok)
                {
                    ctrl.TabIndex = 0;
                    continue;
                }
                if (ctrl.TabStop)
                {
                    ctrl.TabIndex++;
                }
            }
        }
        private void ok_Click(object sender, EventArgs e)
        {
            foreach (ControlParameter param in m_controlParams)
            {
                switch (param.GetType().Name)
                {
                    case "ComboBoxParameter":
                        break;
                    case "TextboxParameter":
                        TextboxReturn tbRet = new TextboxReturn(((IocaineTextbox)param.Control).Text);
                        m_controlReturns.Add(tbRet);
                        break;
                    case "UpDownParameter":
                        break;
                    default:
                        break;
                }
            }
            this.DialogResult = DialogResult.OK;
        }
        #endregion Private Methods
    }
}
