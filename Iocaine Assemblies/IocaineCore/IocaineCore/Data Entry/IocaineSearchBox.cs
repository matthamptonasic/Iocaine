﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

using Iocaine2.Memory;

namespace Iocaine2.Data.Entry
{
    public class IocaineSearchBox : IocaineTextbox
    {
        #region Private Members
        // Properties
        private RegexPatternState m_regexPattern;
        private List<string> m_stringList;
        private string m_string = "";
        private List<string> m_filteredList;
        private int m_maxMatchDepth = 10;
        private int m_minCharToSuggest = 2;
        private bool m_caseSenstivie = false;
        private RegexOptions m_regexOptions;

        // State
        private int m_lastCharCnt;
        private Popup m_suggestBox;
        private System.Windows.Controls.RichTextBox m_rtfBox;
        private int m_selectIndex = -1;
        private string m_selectedText = "";
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
        [
            Category("Iocaine"),
            Description("Search is case-sensitive.")
        ]
        public bool CaseSensitive
        {
            get
            {
                return m_caseSenstivie;
            }
            set
            {
                m_caseSenstivie = value;
                updateRegexOptions();
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
            KeyDown += IocaineSearchBox_KeyDown;

            m_lastCharCnt = Text.Length;
            OnEnterFireEvent = true;
            updateRegexOptions();
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
        public void SetString(ref string iString)
        {
            m_string = iString;
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
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                if (m_selectIndex >= 0)
                {
                    this.Text = m_selectedText.Trim();
                    destroySuggestedBox();
                }
                e.Handled = true;
            }
            else if (e.KeyChar == (char)Keys.Escape)
            {
                destroySuggestedBox();
                this.Text = "";
                e.Handled = true;
            }
            base.IocaineTextbox_KeyPress(sender, e);
        }
        protected void IocaineSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((m_rtfBox == null) || (m_suggestBox == null) || (!m_suggestBox.IsOpen))
            {
                return;
            }

            int l_nbLines = m_rtfBox.Document.Blocks.Count;
            switch (e.KeyCode)
            {
                case Keys.Down:
                    e.Handled = true;
                    if (m_selectIndex < (l_nbLines - 1))
                    {
                        m_selectIndex++;
                    }
                    highlightSuggestedBoxRow(m_selectIndex);
                    break;
                case Keys.Up:
                    e.Handled = true;
                    if (m_selectIndex > 0)
                    {
                        m_selectIndex--;
                    }
                    highlightSuggestedBoxRow(m_selectIndex);
                    break;
            }
        }
        protected override void IocaineTextbox_TextChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch l_sw = System.Diagnostics.Stopwatch.StartNew();

            base.IocaineTextbox_TextChanged(sender, e);
            m_regexPattern.UpdatePattern(this.Text);
            bool l_textShrunk = this.Text.Length < m_lastCharCnt;
            bool l_defText = this.Text == DefaultText;
            bool l_lessThanMin = this.Text.Length < m_minCharToSuggest;
            bool l_upToMin = (this.Text.Length == m_minCharToSuggest) && (m_lastCharCnt == (m_minCharToSuggest - 1));
            m_lastCharCnt = this.Text.Length;
            filterStringList(l_textShrunk || l_upToMin, l_defText || l_lessThanMin);

            long l_filterMs = l_sw.ElapsedMilliseconds;

            if (this.Text.Length < m_minCharToSuggest)
            {
                destroySuggestedBox();
            }
            else if (!l_lessThanMin && (this.Text != DefaultText))
            {
                createSuggestedBox();
            }
            updateSuggestedBox();
            long l_updateMs = l_sw.ElapsedMilliseconds - l_filterMs;
            //MessageBox.Show("Filter: " + l_filterMs + " ==> Update: " + l_updateMs);
        }
        protected override void IocaineTextbox_Click(object sender, EventArgs e)
        {
            base.IocaineTextbox_Click(sender, e);
        }
        private void filterStringList(bool iReset = false, bool iKeepClear = false)
        {
            // 1. Text shrunk - Need to reset the filtered list based on the full list/string.
            // 2. From 1 char to 2 char - Need to reset the filtered list based on the full list/string.
            // 3. Text is default - Need to clear list and keep reset.
            // 4. Has 0 or 1 char - Need to clear list and keep reset.

            // === m_stringList case ===
            // We have a full list (FL) of item names.
            // Whenever the user 1) reduces the pattern size or 2) reaches min pattern size
            //  the FL must be used to populate the filtered list again (iReset = true, iKeepClear = false).
            // Whenever 3) the text is default or 4) the pattern is < min size,
            //  we do not do anything. Just keep the filtered list clear (iReset = true, iKeepClear = true).
            if ((m_stringList == null) && (m_string == ""))
            {
                return;
            }
            if (m_filteredList == null)
            {
                m_filteredList = new List<string>();
            }

            Regex l_regex = new Regex(m_regexPattern.Pattern, m_regexOptions);
            Match l_match;
            if (iReset || iKeepClear)
            {
                m_filteredList.Clear();
                if (iKeepClear)
                {
                    return;
                }

                // If we get here it means we're repopulating the list from the full list.
                // Move this outside of this 'if' and down to a new "if(iReset)"
                //if (m_stringList != null)
                //{
                //    m_filteredList.AddRange(m_stringList);
                //}
                //else
                //{
                //    l_match = l_regex.Match(m_string);
                //    GroupCollection l_groups = l_match.Groups;
                //    for (int ii = 0; ii < l_groups.Count; ii++)
                //    {
                //        m_filteredList.Add(l_groups[ii].ToString());
                //    }
                //}
            }

            // If we reset, we're going through the entire list again.
            // If we did not reset, we're only removing any non-matching items from the existing filtered list.
            if (iReset)
            {
                if ((m_stringList != null) && (m_stringList.Count > 0))
                {
                    for (int ii = 0; ii < m_stringList.Count; ii++)
                    {
                        l_match = l_regex.Match(m_stringList[ii]);
                        if (l_match.Success)
                        {
                            m_filteredList.Add(m_stringList[ii]);
                        }
                    }
                }
                else if (m_string != "")
                {
                    string l_fullPattern = m_regexPattern.Pattern;
                    if (l_fullPattern[0] != '^')
                    {
                        l_fullPattern = "^.*" + l_fullPattern;
                    }
                    l_fullPattern = l_fullPattern[0] + "(" + l_fullPattern.Substring(1, l_fullPattern.Length - 1);
                    if (l_fullPattern.Last() != '$')
                    {
                        l_fullPattern += ".*$";
                    }
                    l_fullPattern = l_fullPattern.Substring(0, l_fullPattern.Length - 1) + ")" + l_fullPattern[l_fullPattern.Length - 1];
                    MatchCollection l_matches = Regex.Matches(m_string, l_fullPattern, m_regexOptions | RegexOptions.Multiline);
                    foreach (Match i_match in l_matches)
                    {
                        m_filteredList.Add(i_match.Value);
                    }
                }
            }
            else
            {
                int l_origCnt = m_filteredList.Count;
                if (l_origCnt == 0)
                {
                    return;
                }
                for (int ii = l_origCnt - 1; ii >= 0; ii--)
                {
                    l_match = l_regex.Match(m_filteredList[ii]);
                    if (!l_match.Success)
                    {
                        m_filteredList.RemoveAt(ii);
                    }
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

            m_selectIndex = -1;
            m_selectedText = "";
            m_suggestBox = new Popup();
            m_rtfBox = new System.Windows.Controls.RichTextBox();
            m_rtfBox.Foreground = System.Windows.Media.Brushes.Gray;
            m_rtfBox.Document.PageWidth = 200;
            m_rtfBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            m_rtfBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;

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
            //System.Diagnostics.Stopwatch l_sw = System.Diagnostics.Stopwatch.StartNew();
            m_rtfBox.BeginChange();
            m_rtfBox.Document.Blocks.Clear();
            Regex l_regex = new Regex(m_regexPattern.Pattern, m_regexOptions);
            try
            {
                TextPointer l_tpLineEnd;
                Match l_match;
                TextPointer l_tpLineStart;
                TextPointer l_tpMatchStart;
                TextPointer l_tpMatchEnd;
                TextRange l_tr;
                TextRange l_trEoL;
                bool l_deboldNext = false;
                for (int ii = 0; ii < m_filteredList.Count; ii++)
                {

                    string l_str = "";
                    if (ii != 0)
                    {
                        l_str = "\n";
                    }
                    l_str += m_filteredList[ii];
                    m_rtfBox.AppendText(l_str);
                    l_match = l_regex.Match(l_str);
                    int l_matchStartIdx = (ii == 0) ? l_match.Index + 1 : l_match.Index;
                    int l_matchEndIdx = l_matchStartIdx + l_match.Length;
                    l_tpLineEnd = m_rtfBox.Document.ContentEnd;
                    l_tpLineStart = l_tpLineEnd.GetLineStartPosition(0);

                    if (l_deboldNext)
                    {
                        l_tr = new TextRange(l_tpLineStart, l_tpLineEnd);
                        l_tr.ApplyPropertyValue(TextElement.FontWeightProperty, System.Windows.FontWeights.Normal);
                    }

                    l_tpMatchStart = l_tpLineStart.GetPositionAtOffset(l_matchStartIdx);
                    l_tpMatchEnd = l_tpLineStart.GetPositionAtOffset(l_matchEndIdx);
                    l_tr = new TextRange(l_tpMatchStart, l_tpMatchEnd);

                    l_tr.ApplyPropertyValue(TextElement.FontWeightProperty, System.Windows.FontWeights.Bold);

                    l_trEoL = new TextRange(l_tpMatchEnd, l_tpLineEnd);
                    l_trEoL.ApplyPropertyValue(TextElement.FontWeightProperty, System.Windows.FontWeights.Normal);
                    l_deboldNext = !l_tpMatchEnd.IsAtInsertionPosition;

                    if ((m_selectedText != "") && (m_selectedText == l_str.Trim()))
                    {
                        m_selectIndex = ii;
                    }
                }
            }
            catch (Exception e)
            {
                Logging.LoggingFunctions.Error(e.ToString());
            }
            //l_sw.Stop();
            //MessageBox.Show("Update: " + l_sw.ElapsedMilliseconds);
            if ((m_selectIndex < 0) && (m_filteredList.Count > 0))
            {
                m_selectIndex = 0;
                m_selectedText = m_filteredList.First();
            }
            else if (m_selectIndex >= m_filteredList.Count)
            {
                m_selectIndex = m_filteredList.Count - 1;
                m_selectedText = m_filteredList.Last();
            }
            m_rtfBox.EndChange();
            highlightSuggestedBoxRow(m_selectIndex);
        }
        private void destroySuggestedBox()
        {
            if (m_suggestBox != null)
            {
                m_suggestBox.IsOpen = false;
                m_suggestBox = null;
            }
        }
        private void highlightSuggestedBoxRow(int iIndex)
        {
            if ((m_suggestBox == null) || (m_suggestBox.IsOpen == false))
            {
                return;
            }

            clearHighlightedRows();

            if ((iIndex >= m_rtfBox.Document.Blocks.Count) || (iIndex < 0))
            {
                return;
            }

            Block l_blk = m_rtfBox.Document.Blocks.ToList()[iIndex];
            TextPointer l_tpDocStart = m_rtfBox.Document.ContentStart;
            TextPointer l_tpLineStart = l_blk.ContentStart;
            TextPointer l_tpLineEnd = l_blk.ContentEnd;
            TextRange l_tr = new TextRange(l_tpLineStart, l_tpLineEnd);
            l_tr.ApplyPropertyValue(TextElement.BackgroundProperty, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Yellow));
            m_selectedText = l_tr.Text;
        }
        private void clearHighlightedRows()
        {
            if ((m_suggestBox == null) || (m_suggestBox.IsOpen == false))
            {
                return;
            }
            TextRange l_tr = new TextRange(m_rtfBox.Document.ContentStart, m_rtfBox.Document.ContentEnd);
            l_tr.ApplyPropertyValue(TextElement.BackgroundProperty, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent));
        }
        private int getLineCount(ref System.Windows.Controls.RichTextBox iRtb)
        {
            TextRange l_tr = new TextRange(iRtb.Document.ContentStart, iRtb.Document.ContentEnd);
            int l_retVal = l_tr.Text.Split('\n').Length;
            return l_retVal;
        }
        private void updateRegexOptions()
        {
            if (!m_caseSenstivie)
            {
                m_regexOptions |= RegexOptions.IgnoreCase;
            }
            else
            {
                m_regexOptions &= ~RegexOptions.IgnoreCase;
            }
        }
        #endregion Private Methods
    }
}
