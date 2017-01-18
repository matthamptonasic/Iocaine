using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Iocaine2.Memory;

namespace Iocaine2.Data.Entry
{
    public class IocaineSearchBox : IocaineTextbox
    {
        #region Private Members
        // Properties
        private RegexPatternState m_regexPattern;
        private List<string> m_stringList;
        private List<string> m_filteredList;
        private int m_maxMatchDepth = 10;
        private int m_minCharToSuggest = 2;

        // State
        private int m_lastCharCnt;
        private Popup m_suggestBox;
        private System.Windows.Controls.RichTextBox m_rtfBox;
        #endregion Private Members

        #region Public Properties
        [
            Browsable(false)
        ]
        public string Pattern
        {
            get
            {
                return m_regexPattern.Pattern;
            }
        }
        [
            Category("Iocaine"),
            Description("Max depth of the suggested match box.")
        ]
        public int MaxMatchDepth
        {
            get
            {
                return m_maxMatchDepth;
            }
            set
            {
                m_maxMatchDepth = value;
            }
        }
        [
            Category("Iocaine"),
            Description("Minimum number of entered characters before box displays suggestions.")
        ]
        public int MinCharToSuggest
        {
            get
            {
                return m_minCharToSuggest;
            }
            set
            {
                m_minCharToSuggest = value;
            }
        }
        #endregion Public Properties

        #region Events
        #endregion Events

        #region Constructor
        public IocaineSearchBox() : base()
        {
            init(new List<string>());
        }
        public IocaineSearchBox(List<string> iStringList) : base()
        {
            init(iStringList);
        }
        private void init(List<string> iStringList)
        {
            m_regexPattern = new RegexPatternState();
            m_stringList = iStringList;
            m_filteredList = new List<string>();
            Leave += IocaineSearchBox_Leave;
            Enter += IocaineSearchBox_Enter;

            m_lastCharCnt = Text.Length;
        }
        #endregion Constructor

        #region Public Methods
        public void SetStringList(List<string> iStringList)
        {
            if (iStringList == null)
            {
                m_stringList = new List<string>();
                return;
            }
            m_stringList = iStringList;
        }
        #endregion Public Methods

        #region Private Methods
        protected virtual void IocaineSearchBox_Enter(object sender, EventArgs e)
        {
            if ((this.Text != DefaultText) && (this.Text != "") && (this.Text.Length >= m_minCharToSuggest))
            {
                createSuggestedBox();
                updateSuggestedBox();
            }
        }
        protected virtual void IocaineSearchBox_Leave(object sender, EventArgs e)
        {
            destroySuggestedBox();
        }
        protected override void IocaineTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            base.IocaineTextbox_KeyPress(sender, e);
        }
        protected override void IocaineTextbox_TextChanged(object sender, EventArgs e)
        {
            base.IocaineTextbox_TextChanged(sender, e);
            m_regexPattern.UpdatePattern(this.Text);
            bool l_textShrunk = this.Text.Length < m_lastCharCnt;
            bool l_defText = this.Text == DefaultText;
            bool l_lessThanMin = this.Text.Length < m_minCharToSuggest;
            m_lastCharCnt = this.Text.Length;
            filterStringList(l_textShrunk, l_defText || l_lessThanMin);

            if (this.Text.Length < m_minCharToSuggest)
            {
                destroySuggestedBox();
            }
            else if (!l_lessThanMin && (this.Text != DefaultText))
            {
                createSuggestedBox();
            }
            updateSuggestedBox();
        }
        protected override void IocaineTextbox_Click(object sender, EventArgs e)
        {
            base.IocaineTextbox_Click(sender, e);
        }
        private void filterStringList(bool iReset = false, bool iKeepClear = false)
        {
            // 1. Text shrunk - need to reset list to full list and try again.
            // 2. From 1 char to 2 char - Need to reset list to full list and try again.
            // 3. Text is default - Need to clear list and keep reset.
            // 4. Has 0 or 1 char - Need to clear list and keep reset.
            // 5. 
            if (m_stringList == null)
            {
                return;
            }
            if (m_filteredList == null)
            {
                m_filteredList = new List<string>();
            }

            if (iReset || iKeepClear)
            {
                m_filteredList.Clear();
                if (iKeepClear)
                {
                    return;
                }
                m_filteredList.AddRange(m_stringList);
            }
            if ((m_filteredList.Count == 0) && (this.Text.Length == m_minCharToSuggest))
            {
                m_filteredList.AddRange(m_stringList);
            }

            int l_origCnt = m_filteredList.Count;
            for (int ii = l_origCnt - 1; ii >= 0; ii--)
            {
                Regex l_idRegex = new Regex(m_regexPattern.Pattern);
                Match l_idMatch = l_idRegex.Match(m_filteredList[ii]);
                if (!l_idMatch.Success)
                {
                    m_filteredList.RemoveAt(ii);
                }
            }
        }
        private void createSuggestedBox()
        {
            if (m_suggestBox != null)
            {
                if (m_suggestBox.IsOpen == false)
                {
                    m_suggestBox.IsOpen = true;
                }
                return;
            }
            m_suggestBox = new Popup();
            m_rtfBox = new System.Windows.Controls.RichTextBox();
            m_rtfBox.Foreground = System.Windows.Media.Brushes.Gray;
            m_rtfBox.Document.PageWidth = 200;

            System.Windows.Style noSpaceStyle = new System.Windows.Style(typeof(System.Windows.Documents.Paragraph));
            noSpaceStyle.Setters.Add(new System.Windows.Setter(System.Windows.Documents.Paragraph.MarginProperty, new System.Windows.Thickness(0)));
            m_rtfBox.Resources.Add(typeof(System.Windows.Documents.Paragraph), noSpaceStyle);

            m_suggestBox.MaxWidth = this.Width;
            m_suggestBox.MinWidth = this.Width;
            m_suggestBox.MaxHeight = m_maxMatchDepth * 16;
            m_suggestBox.Child = m_rtfBox;
            m_suggestBox.Placement = PlacementMode.AbsolutePoint;
            Point l_bottomLeft = this.PointToScreen(Point.Empty);
            l_bottomLeft.Y += this.Height;
            l_bottomLeft.X--;
            m_suggestBox.PlacementRectangle = new System.Windows.Rect(new System.Windows.Point(l_bottomLeft.X, l_bottomLeft.Y), new System.Windows.Point(l_bottomLeft.X + this.Width, l_bottomLeft.Y + 175));
            m_suggestBox.IsOpen = true;
        }
        private void updateSuggestedBox()
        {
            // Check if we should have the box displayed.
            if ((m_suggestBox == null) || (m_suggestBox.IsOpen == false))
            {
                return;
            }

            if ((m_filteredList == null) || (m_filteredList.Count == 0))
            {
                return;
            }
            m_rtfBox.Document.Blocks.Clear();
            for (int ii=0; ii<m_filteredList.Count; ii++)
            {
                string l_str = "";
                if (ii != 0)
                {
                    l_str = "\n";
                }
                l_str += m_filteredList[ii];
                m_rtfBox.AppendText(l_str);
            }
        }
        private void destroySuggestedBox()
        {
            if (m_suggestBox != null)
            {
                m_suggestBox.IsOpen = false;
                m_suggestBox = null;
            }
        }
        #endregion Private Methods
    }
}
