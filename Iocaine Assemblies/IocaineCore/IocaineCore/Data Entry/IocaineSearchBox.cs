using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
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
        private bool m_boxDisplayed = false;
        private int m_lastCharCnt;
        private Popup m_suggestBox;
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
            m_lastCharCnt = Text.Length;
        }
        private void initListBox()
        {
            m_suggestBox = new Popup();
            TextBlock popupText = new TextBlock();
            popupText.Text = "Popup Text";
            popupText.Background = System.Windows.Media.Brushes.LightBlue;
            popupText.Foreground = System.Windows.Media.Brushes.Blue;
            //m_suggestBox.MaxHeight = 100d;
            //m_suggestBox.MinHeight = 100d;
            m_suggestBox.MaxWidth = this.Width;
            m_suggestBox.MinWidth = this.Width;
            m_suggestBox.Child = popupText;
            m_suggestBox.Placement = PlacementMode.AbsolutePoint;
            Point l_bottomLeft = this.PointToScreen(Point.Empty);
            l_bottomLeft.Y += this.Height;
            m_suggestBox.PlacementRectangle = new System.Windows.Rect(new System.Windows.Point(l_bottomLeft.X, l_bottomLeft.Y), new System.Windows.Point(l_bottomLeft.X + this.Width, l_bottomLeft.Y + 175));
            m_suggestBox.IsOpen = true;
        }
        #endregion Constructor

        #region Public Methods
        public void SetStringList(ref List<string> iStringList)
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
        protected override void IocaineTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            base.IocaineTextbox_KeyPress(sender, e);
        }
        protected override void IocaineTextbox_TextChanged(object sender, EventArgs e)
        {
            base.IocaineTextbox_TextChanged(sender, e);
            m_regexPattern.UpdatePattern(Text);
            bool l_textShrunk = Text.Length < m_lastCharCnt;
            m_lastCharCnt = Text.Length;
            filterStringList(l_textShrunk);
        }
        protected override void IocaineTextbox_Click(object sender, EventArgs e)
        {
            base.IocaineTextbox_Click(sender, e);
            initListBox();
        }
        private void filterStringList(bool iReset = false)
        {
            if (iReset)
            {
                m_filteredList.Clear();
                return;
            }

        }
        private void updateSuggestedBox()
        {
            // Check if we should have the box displayed.
        }
        #endregion Private Methods
    }
}
