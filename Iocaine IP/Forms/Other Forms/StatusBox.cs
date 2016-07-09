using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Iocaine2.Tools
{
    public partial class StatusBox : Form
    {
        #region Members
        private delegate void SetLabelTextDelegate(String iText);
        private SetLabelTextDelegate SetLabelTextPtr;
        private delegate void MoveLabelDelegate(Point iPosition);
        private MoveLabelDelegate MoveLabelPtr;
        private delegate int GetDimensionDelegate();
        private GetDimensionDelegate GetFormWidthPtr;
        private GetDimensionDelegate GetLabelWidthPtr;
        private GetDimensionDelegate GetLabelLocationYPtr;
        #endregion Members
        #region Constructor
        public StatusBox()
        {
            InitializeComponent();
            SetLabelTextPtr = new SetLabelTextDelegate(SetLabelTextCallBackFunction);
            MoveLabelPtr = new MoveLabelDelegate(MoveLabelCallBackFunction);
            GetFormWidthPtr = new GetDimensionDelegate(GetFormWidthCallBackFunction);
            GetLabelWidthPtr = new GetDimensionDelegate(GetLabelWidthCallBackFunction);
            GetLabelLocationYPtr = new GetDimensionDelegate(GetLabelLocationYCallBackFunction);
        }
        #endregion Constructor
        #region Private Methods
        private void centerLabel()
        {
            int labelWidth = 0;
            int formWidth = 0;
            int totalMargin = 0;
            int singleMargin = 0;
            int finalLabelX = 0;
            formWidth = GetFormWidth();
            labelWidth = GetLabelWidth();
            totalMargin = formWidth - labelWidth;
            singleMargin = totalMargin / 2;
            finalLabelX = singleMargin - 3;
            MoveLabel(new Point(finalLabelX, GetLabelLocationY()));
        }
        #region GUI Data Accesses
        private int GetFormWidth()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    return (int)this.Invoke(GetFormWidthPtr);
                }
                else
                {
                    return GetFormWidthCallBackFunction();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("In GetFormWidth: " + e.ToString());
                return 0;
            }
        }
        private int GetFormWidthCallBackFunction()
        {
            return this.Size.Width;
        }
        private int GetLabelWidth()
        {
            try
            {
                if (MainLabel.InvokeRequired)
                {
                    return (int)MainLabel.Invoke(GetLabelWidthPtr);
                }
                else
                {
                    return GetLabelWidthCallBackFunction();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("In GetLabelWidth: " + e.ToString());
                return 0;
            }
        }
        private int GetLabelWidthCallBackFunction()
        {
            return MainLabel.Size.Width;
        }
        private int GetLabelLocationY()
        {
            try
            {
                if (MainLabel.InvokeRequired)
                {
                    return (int)MainLabel.Invoke(GetLabelLocationYPtr);
                }
                else
                {
                    return GetLabelLocationYCallBackFunction();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("In GetLabelLocationY: " + e.ToString());
                return 0;
            }
        }
        private int GetLabelLocationYCallBackFunction()
        {
            return MainLabel.Location.Y;
        }
        #endregion GUI Data Accesses
        #region GUI Updates
        private void SetLabelText(String iText)
        {
            try
            {
                if (MainLabel.InvokeRequired)
                {
                    MainLabel.Invoke(SetLabelTextPtr, new object[] { iText });
                }
                else
                {
                    SetLabelTextCallBackFunction(iText);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Trying to set main label text in Iocaine status box: " + e.ToString());
            }
        }
        private void SetLabelTextCallBackFunction(String iText)
        {
            MainLabel.Text = iText;
            this.Refresh();
        }
        private void MoveLabel(Point iPosition)
        {
            try
            {
                if (MainLabel.InvokeRequired)
                {
                    MainLabel.Invoke(MoveLabelPtr, new object[] { iPosition });
                }
                else
                {
                    MoveLabelCallBackFunction(iPosition);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Trying to move main label text in Iocaine status box: " + e.ToString());
            }
        }
        private void MoveLabelCallBackFunction(Point iPosition)
        {
            MainLabel.Location = iPosition;
            this.Refresh();
        }
        #endregion GUI Updates
        #endregion Private Methods
        #region Public Methods
        public void SetText(String iText)
        {
            ClearText();
            SetLabelText(iText);
            centerLabel();
        }
        public void AppendText(String iText)
        {

        }
        public void ClearText()
        {
            SetLabelText("");
        }
        #endregion Public Methods
    }
}
