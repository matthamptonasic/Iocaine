using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Logging;
using Iocaine2.Memory.Interface;

namespace Iocaine2.Tools
{
    public partial class CommandLine : Form
    {
        #region Constructors
        public CommandLine(Process iMainProc, ArrayList iReplyList, int iMaxHistory, MODE iMode)
        {
            InitializeComponent();
            mainProc = iMainProc;
            replyList = iReplyList;
            history = new ArrayList();
            maxHistory = iMaxHistory;
            historyIndex = -1;
            commandRegister = "";
            mode = iMode;
            replyIndex = 0;
            listOfCommands = new ArrayList();
            loadCommandList();
            CLTextBox.MaxLength = 120;
        }
        #endregion Constructors

        #region Enums
        public enum MODE
        {
            ENTER_CMD = 0,
            RAISE_EVENT = 1
        }
        private enum CHAT_MODE
        {
            REPLY = 0,
            LINKSHELL = 1,
            PARTY = 2,
            TELL = 3,
            SAY = 4
        }
        #endregion Enums

        #region Public Members
        public delegate void CommandSentEvent(object sender, CommandSentEventArgs e);
        public event CommandSentEvent CommandSent;
        #endregion Public Members

        #region Private Members
        private Process mainProc;
        private ArrayList replyList;
        private int maxHistory;
        private ArrayList history;
        private int historyIndex;
        private String commandRegister;
        private MODE mode;
        private int replyIndex;
        private ArrayList listOfCommands;
        #endregion Private Members

        #region Public Methods
        public void AddReplyName(String name)
        {
            if (!replyList.Contains(name))
            {
                replyList.Insert(0, name);
            }
            else
            {
                replyList.Remove(name);
                replyList.Insert(0, name);
            }
        }
        #endregion Public Methods

        #region Private Methods
        #region Event Handlers
        private void CLCloseButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void CLSendButton_Click(object sender, EventArgs e)
        {
            if (mode == MODE.ENTER_CMD)
            {
                IocaineFunctions.keys(CLTextBox.Text);
            }
            else if(mode == MODE.RAISE_EVENT)
            {
                CommandSentEventArgs args = new CommandSentEventArgs(CLTextBox.Text);
                CommandSent(this, args);
            }
            LoggingFunctions.Debug("Sending command \"" + CLTextBox.Text + "\" to FFXI.", LoggingFunctions.DBG_SCOPE.COMMANDS);
            saveToHistory(CLTextBox.Text);
            CLTextBox.Text = "";
            replyIndex = 0;
            historyIndex = -1;
        }
        private void CLTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if ((e.KeyData == Keys.Enter) || (e.KeyData == Keys.Return))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                LoggingFunctions.Debug("Enter/Return key was pressed.", LoggingFunctions.DBG_SCOPE.COMMANDS);
                CLSendButton_Click(sender, e);
            }
            else if (e.KeyData == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                replyIndex = 0;
                historyIndex = -1;
                CLTextBox.Clear();
            }
            else if (e.KeyData == Keys.Up)
            {
                moveUpHistory();
            }
            else if (e.KeyData == Keys.Down)
            {
                moveDownHistory();
            }
            else if (e.Alt || e.Control)
            {
                LoggingFunctions.Debug("Found key press while ALT or CTL was pressed.", LoggingFunctions.DBG_SCOPE.COMMANDS);
                if (e.KeyValue == (int)Keys.R)
                {
                    //Reply to last person in reply list
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    LoggingFunctions.Debug("Reply key pressed.", LoggingFunctions.DBG_SCOPE.COMMANDS);
                    changeMode(CHAT_MODE.REPLY);
                }
                else if (e.KeyValue == (int)Keys.L)
                {
                    //Change mode to linkshell
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    LoggingFunctions.Debug("Linkshell key pressed.", LoggingFunctions.DBG_SCOPE.COMMANDS);
                    changeMode(CHAT_MODE.LINKSHELL);
                }
                else if (e.KeyValue == (int)Keys.P)
                {
                    //Change mode to party
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    LoggingFunctions.Debug("Party key pressed.", LoggingFunctions.DBG_SCOPE.COMMANDS);
                    changeMode(CHAT_MODE.PARTY);
                }
                else if (e.KeyValue == (int)Keys.T)
                {
                    //Change mode to tell
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    LoggingFunctions.Debug("Tell key pressed.", LoggingFunctions.DBG_SCOPE.COMMANDS);
                    changeMode(CHAT_MODE.TELL);
                }
                else if (e.KeyValue == (int)Keys.S)
                {
                    //Change mode to say
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    LoggingFunctions.Debug("Say key pressed.", LoggingFunctions.DBG_SCOPE.COMMANDS);
                    changeMode(CHAT_MODE.SAY);
                }
                else
                {
                    LoggingFunctions.Debug("No command key was found with the ALT/CTL press.", LoggingFunctions.DBG_SCOPE.COMMANDS);
                }
            }
            else if ((CLTextBox.Text.Length >= 120) && (e.KeyData != Keys.Back) && (e.KeyData != Keys.Delete))
            {
                e.SuppressKeyPress = true;
            }
            else
            {
                e.SuppressKeyPress = false;
            }
        }
        private void CLTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyData == Keys.Up) || (e.KeyData == Keys.Down))
            {
                CLTextBox.SelectionStart = CLTextBox.Text.Length;
            }
        }
        #endregion Event Handlers
        #region Utilities
        private void loadCommandList()
        {
            listOfCommands.Add("/l");
            listOfCommands.Add("/linkshell");
            listOfCommands.Add("/p");
            listOfCommands.Add("/party");
            listOfCommands.Add("/say");
            listOfCommands.Add("/s");
            listOfCommands.Add("/tell");
            listOfCommands.Add("/t");
        }
        private void changeMode(CHAT_MODE mode)
        {
            String currentText = CLTextBox.Text;
            String cmdPrefix = "";
            bool replyNameAvailable = false;
            switch (mode)
            {
                case CHAT_MODE.LINKSHELL:
                    cmdPrefix = "/linkshell ";
                    break;
                case CHAT_MODE.PARTY:
                    cmdPrefix = "/party ";
                    break;
                case CHAT_MODE.REPLY:
                    cmdPrefix = "/tell ";
                    break;
                case CHAT_MODE.SAY:
                    cmdPrefix = "/say ";
                    break;
                case CHAT_MODE.TELL:
                    cmdPrefix = "/tell ";
                    break;
            }
            LoggingFunctions.Debug("Command prefix is set to: " + cmdPrefix + ".", LoggingFunctions.DBG_SCOPE.COMMANDS);
            if (mode == CHAT_MODE.REPLY)
            {
                if (replyIndex >= replyList.Count)
                {
                    replyIndex = 0;
                }
                //Go to previous person in list
                int replyCount = replyList.Count;
                if (replyCount == 0)
                {
                    //We have no people to reply to, just keep the same cmdPrefix
                    replyNameAvailable = false;
                }
                else if ((replyIndex + 1) > replyCount)
                {
                    //We have gone beyond the end of the reply list,
                    //and should not get to this point, but just use 1st person
                    cmdPrefix += ((String)replyList[0] + " ");
                    replyNameAvailable = true;
                }
                else
                {
                    cmdPrefix += ((String)replyList[replyIndex] + " ");
                    replyNameAvailable = true;
                }
                replyIndex++;
            }
            if (currentText == "")
            {
                //If nothing typed yet, just enter text for user
                currentText = cmdPrefix;
            }
            else
            {
                //If they've typed something, we need to break it apart to analyze what's already there
                char[] splitChar = new char[1];
                splitChar[0] = ' ';
                String[] pieces = currentText.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
                for (int ii = 0; ii < pieces.Length; ii++)
                {
                    if (pieces[ii] == " ")
                    {
                        pieces[ii] = "";
                    }
                    LoggingFunctions.Debug(pieces[ii], LoggingFunctions.DBG_SCOPE.COMMANDS);
                }
                if (pieces[0].Contains("/"))
                {
                    if (listOfCommands.Contains(pieces[0]))
                    {
                        if ((pieces[0] == "/t") || (pieces[0] == "/tell"))
                        {
                            //We knew they've already started a tell.
                            //Check if they've already entered more text than that
                            if (pieces.Length > 1)
                            {
                                //They've already entered at least 1 more word
                                //We'll assume that this word is a name
                                currentText = cmdPrefix;
                                int firstWord = replyNameAvailable ? 2 : 1;
                                for (int ii = 2; ii < pieces.Length; ii++)
                                {
                                    currentText += (pieces[ii] + " ");
                                }
                            }
                            else
                            {
                                currentText = cmdPrefix;
                            }
                        }
                        else
                        {
                            pieces[0] = cmdPrefix;
                            currentText = "";
                            for (int ii = 0; ii < pieces.Length; ii++)
                            {
                                currentText += pieces[ii];
                                //if ((ii != (pieces.Length - 1)) || if(CLTextBox.Text[CLTextBox.Text.Length-1] == ' '))
                                if (pieces[ii][pieces[ii].Length - 1] != ' ')
                                {
                                    currentText += " ";
                                }
                            }
                        }
                    }
                    else
                    {
                        //This was not a known command such as "/hello" or "/sit"
                        //So we'll just move the text over and insert our command there
                        currentText = cmdPrefix + pieces[0].Substring(1) + " ";
                        for (int ii = 1; ii < pieces.Length; ii++)
                        {
                            currentText += (pieces[ii] + " ");
                        }
                    }
                }
                else
                {
                    //They just started typing without setting a command prefix
                    //so add the prefix and trail with the other text
                    currentText = cmdPrefix;
                    for (int ii = 0; ii < pieces.Length; ii++)
                    {
                        currentText += (pieces[ii] + " ");
                    }
                }
            }
            currentText = currentText.Trim();
            currentText += " ";
            CLTextBox.Text = currentText;
            CLTextBox.SelectionStart = CLTextBox.Text.Length;
        }
        private void saveToHistory(String cmd)
        {
            bool inHistory = history.Contains(cmd);
            if ((history.Count < maxHistory) || inHistory)
            {
                if (inHistory)
                {
                    history.Remove(cmd);
                }
                history.Insert(0, cmd);
            }
            else
            {
                //We're full up so we need to pop oldest entry, then insert
                history.RemoveAt(history.Count - 1);
                history.Insert(0, cmd);
            }
            for (int ii = 0; ii < history.Count; ii++)
            {
                LoggingFunctions.Debug("Command[" + ii + "]: " + (String)history[ii], LoggingFunctions.DBG_SCOPE.COMMANDS);
            }
        }
        private void moveUpHistory()
        {
            if (historyIndex == -1)
            {
                commandRegister = CLTextBox.Text;
            }
            if (historyIndex < (history.Count - 1))
            {
                CLTextBox.Text = (String)history[++historyIndex];
            }
        }
        private void moveDownHistory()
        {
            if (historyIndex == -1)
            {
                return;
            }
            else if (historyIndex == 0)
            {
                CLTextBox.Text = commandRegister;
                historyIndex--;
            }
            else
            {
                CLTextBox.Text = (String)history[--historyIndex];
            }
        }
        #endregion Utilities
        #endregion Private Methods
    }

    public class CommandSentEventArgs : System.EventArgs
    {
        private String evCommandSent;

        public CommandSentEventArgs(String CommandSent)
        {
            evCommandSent = CommandSent;
        }
        public String CommandSent
        {
            get
            {
                return evCommandSent;
            }
        }
    }
}
