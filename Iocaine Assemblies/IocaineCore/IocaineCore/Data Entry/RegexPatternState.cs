using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Memory;

namespace Iocaine2.Data.Entry
{
    /// <summary>
    /// Keeps a valid Regex pattern while the user enters it.
    /// That is, if the user has only entered part of a pattern,
    /// the filter will only return the last known good pattern.
    /// Ex. pattern is ".*Tunic \+[0-3]", but the user has only entered
    ///                ".*Tunic \+[0"     the filter returned will be
    ///                ".*Tunic \+"       since [0 is not valid.
    /// It can also convert wildcard expressions to Regex.
    /// </summary>
    public class RegexPatternState
    {
        #region Private Members
        private string m_lastGoodPattern;
        #endregion Private Members

        #region Public Properties
        public string Pattern
        {
            get
            {
                return m_lastGoodPattern;
            }
        }
        #endregion Public Properties

        #region Constructor
        public RegexPatternState()
        {
            m_lastGoodPattern = "";
        }
        #endregion Constructor

        #region Public Methods
        public void Reset()
        {
            m_lastGoodPattern = "";
        }
        public string UpdatePattern(string iText)
        {
            string l_retVal = iText;

            // ? matches single character.
            // # matches single number.
            // [! ] matches a single character that is not in the set.
            // * matches one or more characters.
            // [ ] matches any one of the characters specified in the set.
            //
            // ? converts to .
            // # converts to [0-9]
            // [! ] converts to [^ ]
            // * converts to .*
            // [ ] no conversion needed
            l_retVal = System.Text.RegularExpressions.Regex.Replace(l_retVal, @"([^\\]|^)\?", @"$1.");
            l_retVal = System.Text.RegularExpressions.Regex.Replace(l_retVal, @"([^\\]|^)\#", @"$1[0-9]");
            l_retVal = System.Text.RegularExpressions.Regex.Replace(l_retVal, @"([^\\]|^)\*", @"$1.*");
            l_retVal = System.Text.RegularExpressions.Regex.Replace(l_retVal, @"([^\\]|^)\[\!", @"$1[^");
            if (isValidRegex(l_retVal))
            {
                m_lastGoodPattern = l_retVal;
                return l_retVal;
            }
            else
            {
                return m_lastGoodPattern;
            }
        }
        #endregion Public Methods

        #region Private Methods
        private bool isValidRegex(string iFilter)
        {
            if (string.IsNullOrEmpty(iFilter))
            {
                return false;
            }
            try
            {
                System.Text.RegularExpressions.Regex.Match("", iFilter);
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }
        #endregion Private Methods
    }
}
