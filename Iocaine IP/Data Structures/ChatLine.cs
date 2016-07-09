using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;

namespace Iocaine2.Tools
{
    /// <summary>
    /// Our data class, which is used to carry chatline information to our chat logger objects in our
    /// client classes. 
    /// 
    /// It can also parse the chat lines into various formats.
    /// </summary>
    public class ChatLine : ICloneable
    {
        #region Enums
        public enum CHAT_FLAGS : uint
        {
            NONE = 0x00000000,
            LEAVE_ITEM_DELINIATION = 0x00000001,
            LEAVE_TRAILING_NULL = 0x00000002,
            LEAVE_AUTOTRANSLATE = 0x00000004,
            LEAVE_LEADING_SPECIAL = 0x00000008,
            LEAVE_NAME_HILIGHTS = 0x00000010
        }
        #endregion Enums
        #region Private Members
        private List<String> rawStrings;
        private String processedLine;
        private FFXIEnums.CHAT_MODE chatMode;
        private String logicalLineNb;
        private CHAT_FLAGS flags;
        #region Statics
        private static String eos1 = new string(new char[] { '\u007f', (char)49, (char)0 });
        private static String eos2 = new string(new char[] { (char)32, (char)0 });
        private static List<String> eosList = new List<string> { eos1, eos2 };
        #endregion Statics
        #endregion Private Members
        #region Public Properties
        public List<String> RawStrings
        {
            get
            {
                return rawStrings;
            }
        }
        public String ProcessedLine
        {
            get
            {
                return processedLine;
            }
        }
        public FFXIEnums.CHAT_MODE Mode
        {
            get
            {
                return chatMode;
            }
        }
        public String LogicalLineNb
        {
            get
            {
                return logicalLineNb;
            }
        }
        public CHAT_FLAGS Flags
        {
            get
            {
                return flags;
            }
            set
            {
                flags = value;
            }
        }
        #endregion Public Properties
        #region Public Constants
        public const String PlayerTag = "<PL>";
        public const String NumberTag = "<N>";
        public const String ItemTag = "<IT>";
        public const String FireElementTag = "<FE>";
        public const String IceElementTag = "<IE>";
        public const String WindElementTag = "<AE>"; // A for air
        public const String EarthElementTag = "<EE>";
        public const String LightningElementTag = "<TE>"; // T for Thunder
        public const String WaterElementTag = "<WE>";
        public const String LightElementTag = "<LE>";
        public const String DarknessElementTag = "<DE>";
        #endregion Public Constants

        #region Constructors
        public ChatLine(String iRawString, FFXIEnums.CHAT_MODE iChatMode, String iLogicalLineNb, CHAT_FLAGS iFlags)
        {
            init(iRawString, iChatMode, iLogicalLineNb, iFlags);
        }
        public ChatLine(String iRawString, FFXIEnums.CHAT_MODE iChatMode, String iLogicalLineNb)
        {
            init(iRawString, iChatMode, iLogicalLineNb, CHAT_FLAGS.NONE);
        }
        public ChatLine()
        {
            rawStrings = new List<String>();
        }
        private void init(String iRawString, FFXIEnums.CHAT_MODE iChatMode, String iLogicalLineNb, CHAT_FLAGS iFlags)
        {
            rawStrings = new List<String>();
            rawStrings.Add(iRawString);
            chatMode = iChatMode;
            logicalLineNb = iLogicalLineNb;
            flags = iFlags;
            processedLine = ProcessString(iRawString, flags);
        }
        #endregion Constructors

        #region Public Methods
        public override String ToString()
        {
            String retVal = processedLine;
            retVal += " Code: " + chatMode.ToString();
            return retVal;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public void Append(String iText)
        {
            rawStrings.Add(iText);
            String cleanStr = (String)iText.Clone();
            cleanStr = ProcessString(cleanStr, flags);
            //Put a space at front of line if we're continuing from the previous line and the new line doesn't start with a space.
            if ((rawStrings.Count > 1) && (cleanStr.Length > 0) && (cleanStr[0] != ' '))
            {
                processedLine += " ";
            }
            processedLine += cleanStr;
        }

        public void Set(String iText, FFXIEnums.CHAT_MODE iMode, String iLogicalLineNb)
        {
            chatMode = iMode;
            logicalLineNb = (String)iLogicalLineNb.Clone();
            rawStrings.Clear();
            Append(iText);
        }

        public String ProcessString(String iText, CHAT_FLAGS iFlags)
        {
            String plainstring = (String)iText.Clone();

            if ((iFlags & CHAT_FLAGS.LEAVE_TRAILING_NULL) == 0)
            {
                plainstring = formatRemoveTrailingNull(plainstring);
            }
            if ((iFlags & CHAT_FLAGS.LEAVE_ITEM_DELINIATION) == 0)
            {
                plainstring = formatRemoveItemDeliniation(plainstring);
            }
            if ((iFlags & CHAT_FLAGS.LEAVE_AUTOTRANSLATE) == 0)
            {
                plainstring = formatRemoveAutoTranslate(plainstring);
            }
            if ((iFlags & CHAT_FLAGS.LEAVE_LEADING_SPECIAL) == 0)
            {
                plainstring = formatRemoveSpecialLeadingChars(plainstring);
            }
            if ((iFlags & CHAT_FLAGS.LEAVE_NAME_HILIGHTS) == 0)
            {
                plainstring = formatRemoveSpecialNameChars(plainstring);
            }

            LoggingFunctions.Debug("ChatLine::ProcessString: '" + plainstring + "'", LoggingFunctions.DBG_SCOPE.CHAT);
            return plainstring;
        }

        public String ReprocessStrings(CHAT_FLAGS iFlags)
        {
            return processStrings(iFlags);
        }

        /// <summary>
        /// Returns a string that should be the same for all players and for all situations. 
        /// "Dirk has received 12 damage." -> "[PlayerTag] has received [NumberTag] damage."
        /// This way we can do an easy search and later on parse the relevant data.
        /// </summary>
        /// <returns>NeutralAnnotation string</returns>
        public List<String> ToNeutralAnotationStringList()
        {
            List<String> output = new List<String>();
            foreach (String raw_string in rawStrings)
            {
                String str = (String)raw_string.Clone();
                str = replacePlayerName(str);
                str = replaceNumbers(str);
                str = replaceElementTags(str);
                str = replaceItems(str);
                str = ProcessString(str, CHAT_FLAGS.NONE);
                output.Add(str);
            }
            return output;
        }

        public String ToNeutralAnotationString(int index = 0)
        {
            String str = (String)rawStrings[index].Clone();
            str = replacePlayerName(str);
            str = replaceNumbers(str);
            str = replaceItems(str);
            str = replaceElementTags(str);
            str = ProcessString(str, CHAT_FLAGS.NONE);
            return str;
        }
        #endregion Public Methods

        #region Private Methods
        #region Process Strings
        private String processStrings(CHAT_FLAGS iFlags, bool iFirstonly = false)
        {
            String retval = "";
            String plainstring = "";
            foreach (String raw_string in rawStrings)
            {
                plainstring = (String)raw_string.Clone();
                retval += ProcessString(plainstring, iFlags);

                if (iFirstonly == true)
                {
                    break;
                }
            }
            LoggingFunctions.Debug("[processString] String: " + retval, LoggingFunctions.DBG_SCOPE.CHAT);
            return retval;
        }
        #endregion Process Strings
        #region Format Character Removal
        //char 127 is what SE uses as the trailing null at the end of a string.
        //Since April 2016, the end of string looks like "127 49 0".  49 is just the number 1 which is valid.
        //That's if it's the end of a full line. If it's the end of a broken line it may end with "32 0".
        private String formatRemoveTrailingNull(String iText)
        {
            String oStr = iText;
            foreach (String eos in eosList)
            {
                oStr = oStr.Replace(eos, "");
            }
            return oStr;
        }

        //Item names in log appear colored. This is due to the special characters around them.
        //The chat log of an item looks like:
        // (30) (2) ( ASCII Item Text ) (30) (1)
        private String formatRemoveItemDeliniation(String iText)
        {
            if (iText.Length > 2)
            {
                while (iText.Contains((char)30))
                {
                    int index = iText.IndexOf((char)30);
                    iText = iText.Remove(index, 2);
                }
            }
            return iText;
        }

        //Auto-Translated text appear with colors '(' and ')' around them.
        //The chat log of auto-translated text looks like:
        // (239) (39) ( ASCII Text ) (239) (40)
        private String formatRemoveAutoTranslate(String iText)
        {
            if (iText.Length > 2)
            {
                while (iText.Contains((char)239))
                {
                    int index = iText.IndexOf((char)239);
                    iText = iText.Remove(index, 2);
                }
            }
            return iText;
        }

        //Certain text may appear with leading special characters.
        //At this time the only ones that I know about are:
        //"... synthesized a..."
        //"There are no party members."
        //The chat log of this looks like:
        // (31) (123) ( ASCII Text )
        private String formatRemoveSpecialLeadingChars(String iText)
        {
            if (((byte)iText[0] == 31) && (iText.Length > 2))
            {
                return iText.Substring(2, iText.Length - 2);
            }
            return iText;
        }

        //Incoming chat log name has special characters around is after the 01/15/15 update.
        //This is when chat log names got the ability to be selected for reply, etc.
        //Could be why there are now special characters around them.
        //">>Asic : yo!"
        //The chat log of this looks like:
        // (127) (252) ( ASCII Text ) (127) (251)
        
        private String formatRemoveSpecialNameChars(String iText)
        {
            if (iText.Length > 2)
            {
                iText = iText.Replace(String.Concat((char)127, (char)252), "");
                iText = iText.Replace(String.Concat((char)127, (char)251), "");
            }
            return iText;
        }
        #endregion Format Character Removal
        #region Tag Insertion/Replacement
        private String replaceElementTags(String iText)
        {
            Regex ElementPatternFireRegex = new Regex(@"\xEF\x1F");
            Regex ElementPatternIceRegex = new Regex(@"\xEF\x20");
            Regex ElementPatternWindRegex = new Regex(@"\xEF\x21");
            Regex ElementPatternEarthRegex = new Regex(@"\xEF\x22");
            Regex ElementPatternLightningRegex = new Regex(@"\xEF\x23");
            Regex ElementPatternWaterRegex = new Regex(@"\xEF\x24");
            Regex ElementPatternLightRegex = new Regex(@"\xEF\x25");
            Regex ElementPatternDarknessRegex = new Regex(@"\xEF\x26");

            Match m = ElementPatternEarthRegex.Match(iText);
            if (m.Success)
            {
                Logging.LoggingFunctions.Debug("input", LoggingFunctions.DBG_SCOPE.CHAT);
            }

            String str = ElementPatternFireRegex.Replace(iText, FireElementTag);
            str = ElementPatternIceRegex.Replace(str, IceElementTag);
            str = ElementPatternWindRegex.Replace(str, WindElementTag);
            str = ElementPatternEarthRegex.Replace(str, EarthElementTag);
            str = ElementPatternLightningRegex.Replace(str, LightningElementTag);
            str = ElementPatternWaterRegex.Replace(str, WaterElementTag);
            str = ElementPatternLightRegex.Replace(str, LightElementTag);
            return ElementPatternDarknessRegex.Replace(str, DarknessElementTag);
        }

        private String replacePlayerName(String iText)
        {
            String playerName = MemReads.Self.get_name(true);
            return iText.Replace(playerName, PlayerTag);
        }

        private String replaceNumbers(String iText)
        {
            Regex numberRegex = new Regex(@"\d+");
            return numberRegex.Replace(iText, NumberTag);
        }

        private String replaceItems(String iText)
        {
            Regex itemRegex = new Regex(@"(\x1E\x02)(.*?)(\x1E\x01)");
            return itemRegex.Replace(iText, ItemTag);
        }
        #endregion Tag Insertion/Replacement
        #endregion Private Methods
    }
}
