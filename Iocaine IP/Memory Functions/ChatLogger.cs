using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using Iocaine2.Logging;
using Iocaine2.Memory;

namespace Iocaine2.Tools
{
    abstract public class ChatLogger
    {
        #region Enums
        public enum TYPE : byte
        {
            SYNC = 0,
            ASYNC = 1
        }
        #endregion Enums

        #region Private Members
        protected object padlock = new object();
        protected Queue chatLineQ;
        protected int maxSize;
        protected bool hasNewLines;
        protected uint linesSinceLastRead;
        protected uint lastChatPosition;
        protected ChatLine.CHAT_FLAGS flags;
        public TYPE Type;
        #endregion Private Members
        
        #region Public Properties
        /// <summary>
        /// Get or Set the maximum number of chat lines the buffer will store.
        /// </summary>
        public int MaxSize
        {
            get
            {
                return maxSize;
            }
            set
            {
                maxSize = value;
            }
        }
        /// <summary>
        /// Indicates whether new chat lines have been stored since the last time
        /// the read() function was invoked.
        /// </summary>
        public bool HasNewLines
        {
            get
            {
                return hasNewLines;
            }
        }
        /// <summary>
        /// Indicates the number of lines stored since the last read() function was invoked.
        /// </summary>
        public uint LinesSinceLastRead
        {
            get
            {
                return linesSinceLastRead;
            }
        }
        public ChatLine.CHAT_FLAGS Flags
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

        #region Constructors
        /// <summary>
        /// Initializes the ChatLogger object.
        /// max_size of buffer defaults to 50 lines of chat log.
        /// Class is NOT thread safe. Calling update() and read()
        /// or write(..) from different threads could cause data corruption.
        /// </summary>
        /// <param name="iMaxSize">Maximum number of chat lines to buffer</param>
        /// /// <param name="iFlags">Sets the chat log filter flags. See ChatLine.CHAT_FLAGS</param>
        public ChatLogger(int iMaxSize = 50, ChatLine.CHAT_FLAGS iFlags = ChatLine.CHAT_FLAGS.NONE)
        {
            chatLineQ = new Queue();
            maxSize = iMaxSize;
            hasNewLines = false;
            linesSinceLastRead = 0;
            flags = iFlags;
            if (MemReads.PointersSet && ChangeMonitor.LoggedIn)
            {
                lastChatPosition = MemReads.Chat.get_index();
            }
        }
        #endregion Constructors

        #region Public Methods
        /// <summary>
        /// Clears the Queues and resets variables.
        /// </summary>
        public void Reset()
        {
            LoggingFunctions.Debug("Resetting chatlogger", LoggingFunctions.DBG_SCOPE.CHAT);
            Monitor.Enter(padlock);
            try
            {
                chatLineQ.Clear();
                hasNewLines = false;
                linesSinceLastRead = 0;
                if (MemReads.PointersSet && ChangeMonitor.LoggedIn)
                {
                    lastChatPosition = MemReads.Chat.get_index();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("ChatLogger::Reset: " + e.ToString());
            }
            finally
            {
                Monitor.Exit(padlock);
            }
        }
        /// <summary>
        /// Dequeues the last String of the chat log.
        /// </summary>
        /// <param name="chatLine">Next ChatLine object in the buffer. Null if queue is empty.</param>
        /// <returns>Returns false if no chat line available.</returns>
        public bool Read(out ChatLine oChatLine)
        {
            bool read = false;
            oChatLine = null;
            Monitor.Enter(padlock);
            try
            {
                linesSinceLastRead = 0;
                if (chatLineQ.Count <= 1)
                {
                    hasNewLines = false;
                }
                if (chatLineQ.Count > 0)
                {
                    oChatLine = (ChatLine)chatLineQ.Dequeue();
                    read = true;
                }
                else
                {
                    oChatLine = null;
                    read = false;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("ChatLogger::Read: " + e.ToString());
            }
            finally
            {
                Monitor.Exit(padlock);
            }
            return read;
        }
        #endregion Public Methods
    }

    public class ChatLoggerSync : ChatLogger
    {
        #region Constructors
        /// <summary>
        /// Initializes the ChatLogger object.
        /// max_size of buffer defaults to 50 lines of chat log.
        /// Class is NOT thread safe. Calling update() and read()
        /// or write(..) from different threads could cause data corruption.
        /// </summary>
        /// <param name="iMaxSize">Maximum number of chat lines to buffer</param>
        /// /// <param name="iFlags">Sets the chat log filter flags. See ChatLine.CHAT_FLAGS</param>
        public ChatLoggerSync(int iMaxSize = 50, ChatLine.CHAT_FLAGS iFlags = ChatLine.CHAT_FLAGS.NONE) : base(iMaxSize, iFlags)
        {
            Type = TYPE.SYNC;
        }
        #endregion Constructors
        #region Public Methods
        /// <summary>
        /// Writes chat information into the Queues.
        /// This should only be used if you are NOT using the read function.
        /// An example where this would be appropriate is if you are using 2 ChatLogger
        /// objects for two different parsing purposes. You might use the update function in the first
        /// object, then read the info, parse the log, and write it to the second object.
        /// The second object could then be read from without using the update function.
        /// This way the MemReads of the chat log will only happen once which is much more efficient
        /// than doing it two times.
        /// </summary>
        public void Write(ChatLine iChatLine)
        {
            hasNewLines = true;
            linesSinceLastRead++;
            Monitor.Enter(padlock);
            try
            {
                chatLineQ.Enqueue(iChatLine);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("ChatLogger::Write: " + e.ToString());
            }
            finally
            {
                Monitor.Exit(padlock);
            }
        }
        #endregion Public Methods
    }

    public class ChatLoggerAsync : ChatLogger
    {
        #region Constructors
        /// <summary>
        /// Initializes the ChatLogger object.
        /// max_size of buffer defaults to 50 lines of chat log.
        /// Class is NOT thread safe. Calling update() and read()
        /// or write(..) from different threads could cause data corruption.
        /// </summary>
        /// <param name="iMaxSize">Maximum number of chat lines to buffer</param>
        /// <param name="iFlags">Sets the chat log filter flags. See ChatLine.CHAT_FLAGS</param>
        public ChatLoggerAsync(int iMaxSize = 50, ChatLine.CHAT_FLAGS iFlags = ChatLine.CHAT_FLAGS.NONE) : base(iMaxSize, iFlags)
        {
            Type = TYPE.ASYNC;
        }
        #endregion Constructors
        #region Functions
        /// <summary>
        /// Updates the chat log Queues if new chat lines are found in the chat log.
        /// </summary>
        /// <param name="nbNewLines">int Buffer to store the number of new lines added.</param>
        /// <returns>True if new lines found, false if no new lines.</returns>
        public bool Update(ref uint nbNewLines)
        {
            if (!MemReads.PointersSet || !ChangeMonitor.LoggedIn)
            {
                return false;
            }
            Monitor.Enter(padlock);
            try
            {
                UInt32 new_chat_position = MemReads.Chat.get_index();
                //LoggingFunctions.debug("Updating chat logger, old chat pos: " + last_chat_position + ", new pos: " + new_chat_position);
                if (new_chat_position != lastChatPosition)
                {
                    hasNewLines = true;
                    if (new_chat_position > lastChatPosition)
                    {
                        linesSinceLastRead = new_chat_position - lastChatPosition;
                    }
                    else
                    {
                        linesSinceLastRead = new_chat_position - lastChatPosition + 50;
                    }
                    uint linesOverCapacity = 0;
                    //if ( ( current Q count + nb new lines) goes over the buffer size )
                    //  then linesOverCapacity = how much over we will go.
                    if ((chatLineQ.Count + linesSinceLastRead) > maxSize)
                    {
                        linesOverCapacity = (uint)(chatLineQ.Count + linesSinceLastRead - maxSize);
                    }
                    nbNewLines = 0;
                    ChatLine chatLine = null;
                    FFXIEnums.CHAT_MODE chat_code = 0;
                    String logicalLineNb = "";
                    //In order to concatinate logical lines that are broken up over multiple chat lines
                    //we have to hold off on writing the chat string AND chat code into the queues until
                    //1. We get a logical line number that does not equal the previous logical line number.
                    //2. We have read our last line of chat for this update.

                    //The for loop goes down from the ( nb of lines we're reading -1 ) => either 0 OR the nb lines we will be over the buffer size.
                    //Ex 1: Max buffer = 50, we have 46, we're reading 3 new lines.
                    //      The loop goes ii = 2, 1, 0.
                    //Ex 2: Max buffer = 50, we have 47, we're reading 6 new lines.
                    //      linesOverCapacity will be 3 (Q.cnt + newLines - max = 47 + 6 - 50 = 3).
                    //      The loop goes ii = 5, 4, 3.
                    //      We stop reading there so we don't over fill the buffer (tail drop).
                    int loopStart = (int)linesSinceLastRead - 1;
                    int loopEnd = (int)linesOverCapacity;
                    int nbIterations = loopStart - loopEnd + 1;
                    bool heldLastLine = false;
                    LoggingFunctions.Debug("NbLines: " + linesSinceLastRead + ", loopStart: " + loopStart + ", loopEnd: " + loopEnd, LoggingFunctions.DBG_SCOPE.CHAT);
                    for (int ii = loopStart; ii >= loopEnd; ii--)
                    {
                        try
                        {
                            Monitor.Enter(chatLineQ);
                            if (chatLineQ.Count < maxSize)
                            {
                                String chat = MemReads.Chat.get_lineX(ii, ref chat_code, ref logicalLineNb);
                                if (heldLastLine)
                                {
                                    if (logicalLineNb == chatLine.LogicalLineNb)
                                    {
                                        //Just concat with the previous line and discard the code (should be the same).
                                        chatLine.Append(chat);
                                        if (ii == loopEnd)
                                        {
                                            //This is our last time through the loop so we have to push what we have.
                                            chatLineQ.Enqueue(chatLine);
                                            nbNewLines++;
                                            LoggingFunctions.Debug("Chat code: " + chatLine.Mode.ToString() + ", String: " + chatLine.ProcessedLine, LoggingFunctions.DBG_SCOPE.CHAT);
                                        }
                                    }
                                    else
                                    {
                                        //Push the last one and hold the new one.
                                        chatLineQ.Enqueue(chatLine);
                                        nbNewLines++;
                                        chatLine = new ChatLine(chat, chat_code, logicalLineNb, flags);

                                        LoggingFunctions.Debug("Chat code: " + chatLine.Mode.ToString() + ", String: " + chatLine.ProcessedLine, LoggingFunctions.DBG_SCOPE.CHAT);
                                        if (ii == loopEnd)
                                        {
                                            //This is our last time through the loop so we have to push what we have.
                                            chatLineQ.Enqueue(chatLine);
                                            nbNewLines++;
                                            LoggingFunctions.Debug("Chat code: " + chatLine.Mode.ToString() + ", String: " + chatLine.ProcessedLine, LoggingFunctions.DBG_SCOPE.CHAT);
                                        }
                                    }
                                }
                                else
                                {
                                    if (ii == loopEnd)
                                    {
                                        //This is our last time through the loop so we have to push what we have.
                                        chatLine = new ChatLine(chat, chat_code, logicalLineNb, flags);
                                        chatLineQ.Enqueue(chatLine);
                                        nbNewLines++;
                                        LoggingFunctions.Debug("Chat code: " + chat_code.ToString() + ", String: " + chat, LoggingFunctions.DBG_SCOPE.CHAT);
                                    }
                                    else
                                    {
                                        //The temp variables are free to be used and this isn't our last time through,
                                        //so just hold on to the new info.
                                        if ((chatLine == null) || (chatLine.RawStrings.Count == 0))
                                        {
                                            chatLine = new ChatLine(chat, chat_code, logicalLineNb, flags);
                                        }
                                        else
                                        {
                                            chatLine.Append(chat);
                                        }
                                        heldLastLine = true;
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (Exception Ex)
                        {
                            LoggingFunctions.Error("ChatLogger::Update: " + Ex.ToString());
                        }
                        finally
                        {
                            Monitor.Exit(chatLineQ);
                        }
                    }
                    lastChatPosition = new_chat_position;
                    return true;
                }
                else
                {
                    nbNewLines = linesSinceLastRead;
                    return false;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("ChatLogger::Update: " + e.ToString());
            }
            finally
            {
                Monitor.Exit(padlock);
            }
            return false;
        }
        #endregion Public Methods
    }
}
