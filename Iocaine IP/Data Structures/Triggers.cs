using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Iocaine2.Data.Structures
{
    #region Enums
    public enum TRIG_TYPE
    {
        Unknown = 0,
        Chat_Match = 1,
        Player_Status_Change = 2,
        Player_HPP_Change = 3,
        Count = 4
    }
    #endregion Enums
    public abstract class Trigger
    {
        public Trigger(TRIG_TYPE iType)
        {
            type = iType;
        }
        protected TRIG_TYPE type = TRIG_TYPE.Unknown;
        public TRIG_TYPE Type
        {
            get
            {
                return type;
            }
        }
        public abstract bool CheckTrigger(FFXIEventArgs e);
        public abstract void Show();
        public abstract new String ToString();
        public bool Compare(Trigger iTrig)
        {
            if (iTrig.ToString() == this.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class TriggerPlayerStatusChange : Trigger
    {
        #region Constructor
        public TriggerPlayerStatusChange(String iPlayerName, Byte iOldStatus, Byte iNewStatus, Boolean iCheckOldStatus, Boolean iCheckNewStatus)
            : base(TRIG_TYPE.Player_Status_Change)
        {
            playerName = iPlayerName;
            oldStatus = iOldStatus;
            newStatus = iNewStatus;
            checkOldStatus = iCheckOldStatus;
            checkNewStatus = iCheckNewStatus;
        }
        #endregion Constructor
        #region Members
        private String playerName;
        private Byte oldStatus;
        private Byte newStatus;
        private Boolean checkOldStatus;
        private Boolean checkNewStatus;
        #endregion Members
        #region Properties
        public String PlayerName
        {
            get
            {
                return playerName;
            }
        }
        public Byte OldStatus
        {
            get
            {
                return oldStatus;
            }
        }
        public Byte NewStatus
        {
            get
            {
                return newStatus;
            }
        }
        public Boolean CheckOldStatus
        {
            get
            {
                return checkOldStatus;
            }
        }
        public Boolean CheckNewStatus
        {
            get
            {
                return checkNewStatus;
            }
        }
        #endregion Properties
        #region Methods
        public override bool CheckTrigger(FFXIEventArgs e)
        {
            if(e.Type != ChangeMonitor.EVENT_TYPE.PLAYER_STATUS_CHANGED)
            {
                return false;
            }
            PlayerStatusChangedEvArgs localArgs = (PlayerStatusChangedEvArgs)e;
            if(localArgs.Name != this.playerName)
            {
                return false;
            }
            Byte localOld = localArgs.OldStatus;
            Byte localNew = localArgs.NewStatus;
            if(checkOldStatus && (localOld != this.oldStatus))
            {
                return false;
            }
            if(checkNewStatus && (localNew != this.newStatus))
            {
                return false;
            }
            return true;
        }
        public override void Show()
        {
            MessageBox.Show(this.ToString(), "Trigger Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override string ToString()
        {
            String display = "TriggerPlayerStatusChange;";
            display += oldStatus + ";";
            display += (checkOldStatus.ToString()).ToLower() + ";";
            display += newStatus + ";";
            display += (checkNewStatus.ToString()).ToLower();
            return display;
        }
        #endregion Methods
    }

    public class TriggerChatMatch : Trigger
    {
        #region Constructor
        public TriggerChatMatch(List<String> iFiltersChat, List<Int32> iFiltersCodes, UInt32 iRequiredMatchCount, Boolean iUseRegex, List<Char.Character> iPlayerList, Boolean iMatchCodes = true, UInt32 iMaxHistory = 50)
            : base(TRIG_TYPE.Chat_Match)
        {
            filtersChat = iFiltersChat;
            filtersCodes = iFiltersCodes;
            requiredMatchCount = iRequiredMatchCount;
            useRegex = iUseRegex;
            playerList = iPlayerList;
            matchCodes = iMatchCodes;
            maxHistory = iMaxHistory;
        }
        #endregion Constructor
        #region Members
        private List<String> filtersChat = null;
        private List<Int32> filtersCodes = null;
        private Queue<Tools.ChatLine> historyChat = new Queue<Tools.ChatLine>();
        private UInt32 maxHistory;
        private UInt32 requiredMatchCount;
        private Boolean useRegex;
        private Boolean matchCodes;
        private List<Char.Character> playerList = null;
        private List<String> matches = new List<String>();
        public const String MatchParty = "$party";
        public const String MatchMob = "$monster";
        //public const String MatchStatus = "$status";
        #endregion Members
        #region Properties
        public List<String> FiltersChat
        {
            get
            {
                return filtersChat;
            }
            set
            {
                filtersChat = value;
            }
        }
        public List<Int32> FiltersCodes
        {
            get
            {
                return filtersCodes;
            }
            set
            {
                filtersCodes = value;
            }
        }
        public UInt32 RequiredMatchCount
        {
            get
            {
                return requiredMatchCount;
            }
            set
            {
                requiredMatchCount = value;
            }
        }
        public List<String> Matches
        {
            get
            {
                return matches;
            }
        }
        #endregion Properties
        #region Methods
        public override bool CheckTrigger(FFXIEventArgs e)
        {
            if (e.Type != ChangeMonitor.EVENT_TYPE.CHAT)
            {
                return false;
            }
            ChatEvArgs localArgs = (ChatEvArgs)e;
            List<Tools.ChatLine> localChat = localArgs.Chat;
            
            //Push the new chat/codes into the history queues and trim if over the max history.
            for (int ii = 0; ii < localChat.Count; ii++)
            {
                historyChat.Enqueue(localChat[ii]);
            }
            if(historyChat.Count > maxHistory)
            {
                Int32 nbToPop = historyChat.Count - (Int32)maxHistory;
                for (int ii = 0; ii < nbToPop; ii++)
                {
                    historyChat.Dequeue();
                }
            }

            //foreach(String filterList in filtersChat)
            //{
            //    //Go through the filter and see if it matches the history.
            //    Int32 nbChatLines = historyChat.Count;
                
            //}
            Int32 lastHit = -1;
            for (int ii = 0; ii < filtersChat.Count; ii++)
            {
                for(int kk=lastHit+1; kk<historyChat.Count; kk++)
                {
                    //If the regex hits, save the lastHit and break.
                }
            }
                return true;
        }
        private String convertToRegex(String iFilter)
        {
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
            iFilter = Regex.Replace(iFilter, @"([^\\]|^)\?", @"$1.");
            iFilter = Regex.Replace(iFilter, @"([^\\]|^)\#", @"$1[0-9]");
            iFilter = Regex.Replace(iFilter, @"([^\\]|^)\*", @"$1.*");
            iFilter = Regex.Replace(iFilter, @"([^\\]|^)\[\!", @"$1[^");
            return iFilter;
        }
        public override void Show()
        {
            MessageBox.Show(this.ToString(), "Trigger Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override string ToString()
        {
            String display = "TriggerChatMatch;";
            return display;
        }
        #endregion Methods
    }
}