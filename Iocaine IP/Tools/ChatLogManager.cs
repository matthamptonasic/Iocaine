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
using Iocaine2.Threading;

namespace Iocaine2.Tools
{
    /// <summary>
    /// A container and management class for chat logger objects.
    /// Will manage synchronous chat loggers that are automatically
    /// populated from a master chat logger (client does not call .update).
    /// Will manage asynchronous chat loggers that must be updated
    /// by the client in order to populate the text from the game chat log.
    /// </summary>
    public class ChatLogManager : IocaineThread
    {
        #region Enums
        #endregion Enums
        #region Private Members
        private static ChatLogManager m_instance = null;
        private ChatLoggerAsync m_masterLogger = new ChatLoggerAsync();
        private List<ChatLoggerSync> m_syncChatLogList = new List<ChatLoggerSync>();
        private List<String> m_syncChatLogNameList = new List<string>();
        private List<ChatLoggerAsync> m_asyncChatLogList = new List<ChatLoggerAsync>();
        private List<String> m_asyncChatLogNameList = new List<string>();
        private const String m_chatLogDefName = "Anonymous";
        private UInt32 m_chatPollingPeriod = 500;
        private Int32 m_maxDepth = 50;
        private ChatLine.CHAT_FLAGS m_flags = ChatLine.CHAT_FLAGS.NONE;
        private static readonly object m_padlock_state = new object();
        private static readonly object m_padlock_inst = new object();
        #endregion Private Members
        #region Public Members/Properties
        public static ChatLogManager Access
        {
            get
            {
                lock(m_padlock_inst)
                {
                    if (m_instance == null)
                    {
                        m_instance = new ChatLogManager();
                    }
                    return m_instance;
                }
            }
        }
        public delegate void NewChatAvailable();
        /// <summary>
        /// Will notify the client when new chat has been pushed into the logger queue.
        /// </summary>
        public NewChatAvailable _NewChatAvailable;
        public delegate void NewChat(ChatEvArgs e);
        public NewChat _NewChat;
        /// <summary>
        /// Get or set the polling period for the synchronous chat loggers.
        /// </summary>
        public UInt32 PollingPeriod
        {
            get
            {
                return m_chatPollingPeriod;
            }
            set
            {
                m_chatPollingPeriod = value;
            }
        }
        /// <summary>
        /// Get or Set the max depth of the chat logger queues.
        /// </summary>
        public Int32 MaxDepth
        {
            get
            {
                return m_maxDepth;
            }
            set
            {
                if (value > 0)
                {
                    m_maxDepth = value;
                    foreach (ChatLogger cl in m_syncChatLogList)
                    {
                        cl.MaxSize = value;
                    }
                    foreach (ChatLogger cl in m_asyncChatLogList)
                    {
                        cl.MaxSize = value;
                    }
                }
            }
        }
        /// <summary>
        /// Get or Set the chat logger flags.
        /// See the chat logger class for flag definitions.
        /// </summary>
        public ChatLine.CHAT_FLAGS Flags
        {
            get
            {
                return m_flags;
            }
            set
            {
                m_flags = value;
                foreach (ChatLogger cl in m_syncChatLogList)
                {
                    cl.Flags = m_flags;
                }
                foreach (ChatLogger cl in m_asyncChatLogList)
                {
                    cl.Flags = m_flags;
                }
            }
        }
        #endregion Public Members/Properties

        #region Constructor
        private ChatLogManager()
            : base("MasterChatLoggerThread")
        {
            base.__RunMethod = this.runThread;
        }
        #endregion Constructor

        #region Private Functions
        private void runThread()
        {
            Reset();
            while (checkState())
            {
                if (doRead())
                {
                    if (_NewChatAvailable != null)
                    {
                        _NewChatAvailable();
                    }
                }
                IocaineFunctions.delay(m_chatPollingPeriod);
            }
        }
        private bool doRead()
        {
            uint nbLines = 0;
            if (m_masterLogger.Update(ref nbLines))
            {
                pipeLoggers();
                return true;
            }
            else
            {
                return false;
            }
        }
        private void pipeLoggers()
        {
            ChatLine chat;
            List<ChatLine> allChat = new List<ChatLine>();
            while (m_masterLogger.Read(out chat))
            {
                foreach (ChatLoggerSync cl in m_syncChatLogList)
                {
                    cl.Write(chat);
                }
                allChat.Add(chat);
            }
            if(_NewChat != null)
            {
                _NewChat(new ChatEvArgs(allChat));
            }
        }
        #endregion Private Functions
        #region Public Functions
        #region Processing
        /// <summary>
        /// Begin the polling process of reading chat from the POL process.
        /// </summary>
        //public void Start()
        //{
        //    if ((m_state != THREAD_STATE.UNSTARTED) && (m_state != THREAD_STATE.STOPPED))
        //    {
        //        return;
        //    }
        //    m_chatPollingThread = new Thread(new ThreadStart(runThread));
        //    m_chatPollingThread.Name = "MasterChatLoggerThread";
        //    m_chatPollingThread.IsBackground = true;
        //    m_chatPollingThread.Start();
        //    return;
        //}
        /// <summary>
        /// Stop the polling process.
        /// </summary>
        //public void Stop()
        //{
        //    Monitor.Enter(m_padlock_state);
        //    if (m_state == THREAD_STATE.FROZEN)
        //    {
        //        m_chatPollingThread.Abort();
        //    }
        //    else
        //    {
        //        m_state = THREAD_STATE.STOP_REQUESTED;
        //    }
        //    Monitor.Exit(m_padlock_state);
        //}
        //public void Freeze()
        //{
        //    m_state = THREAD_STATE.FREEZE_REQUESTED;
        //}
        //public void Thaw()
        //{
        //    Monitor.Enter(m_padlock_state);
        //    if (m_state == THREAD_STATE.FROZEN)
        //    {
        //        m_chatPollingThread.Interrupt();
        //    }
        //    else
        //    {
        //        m_state = THREAD_STATE.THAW_REQUESTED;
        //    }
        //    Monitor.Exit(m_padlock_state);
        //}
        /// <summary>
        /// Reset all chat loggers.
        /// </summary>
        public void Reset()
        {
            m_masterLogger.Reset();
            foreach (ChatLogger cl in m_syncChatLogList)
            {
                Monitor.Enter(cl);
                try
                {
                    cl.Reset();
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("In ChatLogManager.Reset: " + e.ToString());
                }
                finally
                {
                    Monitor.Exit(cl);
                }
            }
            foreach (ChatLogger cl in m_asyncChatLogList)
            {
                Monitor.Enter(cl);
                try
                {
                    cl.Reset();
                }
                catch (Exception e)
                {
                    LoggingFunctions.Error("In ChatLogManager.Reset: " + e.ToString());
                }
                finally
                {
                    Monitor.Exit(cl);
                }
            }
        }
        #endregion Processing
        #region Get/Removes
        /// <summary>
        /// Gets a new chat logger object that will be automatically populated
        /// while the polling process is active. Do NOT use the .update method for this object.
        /// You must call the corresponding remove method when disposing the logger.
        /// </summary>
        /// <returns>The newly created ChatLogger object.</returns>
        public ChatLoggerSync GetSynchronousLogger()
        {
            return GetSynchronousLogger(m_chatLogDefName);
        }
        /// <summary>
        /// Gets a new chat logger object that will be automatically populated
        /// while the polling process is active. Do NOT use the .update method for this object.
        /// You must call the corresponding remove method when disposing the logger.
        /// </summary>
        /// <param name="iLoggerName">The name to give the chat logger.</param>
        /// <returns>The newly created ChatLogger object.</returns>
        public ChatLoggerSync GetSynchronousLogger(String iLoggerName)
        {
            ChatLoggerSync logger = new ChatLoggerSync(m_maxDepth);
            m_syncChatLogList.Add(logger);
            m_syncChatLogNameList.Add(iLoggerName);
            return logger;
        }
        /// <summary>
        /// Gets a new chat logger object that must be updated by the calling client.
        /// You must call the corresponding remove method when disposing the logger.
        /// </summary>
        /// <returns>The newly created ChatLogger object.</returns>
        public ChatLogger GetAsynchronousLogger()
        {
            return GetAsynchronousLogger(m_chatLogDefName);
        }
        /// <summary>
        /// Gets a new chat logger object that must be updated by the calling client.
        /// You must call the corresponding remove method when disposing the logger.
        /// </summary>
        /// <param name="iLoggerName">The name to give the chat logger.</param>
        /// <returns>The newly created ChatLogger object.</returns>
        public ChatLoggerAsync GetAsynchronousLogger(String iLoggerName)
        {
            ChatLoggerAsync logger = new ChatLoggerAsync(m_maxDepth);
            m_asyncChatLogList.Add(logger);
            m_asyncChatLogNameList.Add(iLoggerName);
            return logger;
        }
        /// <summary>
        /// Remove the given ChatLogger object from the internal lists.
        /// Once the client's pointer to the object is disposed of, the
        /// object will be garbage collected.
        /// </summary>
        /// <param name="iLogger">The object to remove.</param>
        /// <returns>True if the object was found and removed. False if it was not found.</returns>
        public bool RemoveLogger(ChatLogger iLogger)
        {
            if (iLogger.Type == ChatLogger.TYPE.SYNC)
            {
                if (m_syncChatLogList.Contains((ChatLoggerSync)iLogger))
                {
                    int idx = m_syncChatLogList.IndexOf((ChatLoggerSync)iLogger);
                    m_syncChatLogList.RemoveAt(idx);
                    m_syncChatLogNameList.RemoveAt(idx);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (iLogger.Type == ChatLogger.TYPE.ASYNC)
            {
                if (m_asyncChatLogList.Contains((ChatLoggerAsync)iLogger))
                {
                    int idx = m_asyncChatLogList.IndexOf((ChatLoggerAsync)iLogger);
                    m_asyncChatLogList.RemoveAt(idx);
                    m_asyncChatLogNameList.RemoveAt(idx);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Remove the given ChatLogger object from the internal lists.
        /// Once the client's pointer to the object is disposed of, the
        /// object will be garbage collected.
        /// </summary>
        /// <param name="iLoggerName">The name of the ChatLogger passed upon creation.
        /// If more than one object share the same name, the first one created will be removed.</param>
        /// <returns>True if the object was found and removed. False if it was not found.</returns>
        public bool RemoveLogger(String iLoggerName)
        {
            if (m_syncChatLogNameList.Contains(iLoggerName))
            {
                int idx = m_syncChatLogNameList.IndexOf(iLoggerName);
                m_syncChatLogList.RemoveAt(idx);
                m_syncChatLogNameList.RemoveAt(idx);
                return true;
            }
            else if (m_asyncChatLogNameList.Contains(iLoggerName))
            {
                int idx = m_asyncChatLogNameList.IndexOf(iLoggerName);
                m_asyncChatLogList.RemoveAt(idx);
                m_asyncChatLogNameList.RemoveAt(idx);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion Get/Removes
        #endregion Public Functions
    }

    public sealed class SynergyChatLogManager
    {
        #region BackGroundLogger
        /// <summary>
        /// This is the only class that interacts with the memory manager. It continiously reads the chatlog. It can copy the past 50
        /// lines to the sync chat logger, to emulate legacy behavior.
       
        ///  Using http://msdn.microsoft.com/en-us/library/7a2f3ay4%28v=vs.80%29.aspx for thread stop mechanism.
        /// </summary>

        private Thread _backgroundlogger_thread;
        private uint _backgroundlogger_chat_polling_period;

        public void Start(uint chat_polling_period)
        {
            lock (syncRoot)
            {
                if (_backgroundlogger_thread == null)
                {
                    _backgroundlogger_stop_loop = false;
                    _backgroundlogger_chat_polling_period = chat_polling_period;
                    _backgroundlogger_thread = new Thread(new ThreadStart(pollingLoop));
                    _backgroundlogger_thread.Name = "BackGroundLoggerThread";
                    _backgroundlogger_thread.IsBackground = true;
                    _backgroundlogger_thread.Start();
                }
            }
        }

        public void Stop()
        {
            lock (syncRoot)
            {
                if (_backgroundlogger_thread != null)
                {
                    _backgroundlogger_stop_loop = true;
                    _backgroundlogger_thread.Join();
                    _backgroundlogger_thread = null;
                }
            }
        }


        private volatile bool _backgroundlogger_stop_loop;
        private byte _backgroundlogger_previous_chat_position;
        private void pollingLoop()
        {
            while (!_backgroundlogger_stop_loop)
            {
                //if (Iocaine2.Memory.MemReads.PointersSet && ChangeMonitor.LoggedIn)
                //{
                //    List<ChatLine> newChatLines = new List<ChatLine>();
                //    List<ChatLine> lines = Iocaine2.Memory.MemReads.Chat.get_new_lines(ref _backgroundlogger_previous_chat_position);

                //    // lines -> new_chat_lines;
                //    foreach( Iocaine2.Memory.MemReads.Chat.ChatLine line in lines )
                //    {
                //        ChatLine newline = new ChatLine(line);
                //        newChatLines.Add( newline );
                //     //   LoggingFunctions.Timestamp("New Chatline: " + newline.ToString() );
                //    }

                //    if (newChatLines.Count() > 0)
                //    {
                //        OnChatLoggerUpdate(newChatLines);
                //    }
                //}
                IocaineFunctions.delay(_backgroundlogger_chat_polling_period);
            }
        }

        #endregion BackGroundLogger
        #region ChatLoggerBase
        /// <summary>
        /// The SyncChatLogger is a logger that is held by the client. It only contains a set number of lines
        /// the client is never notified of new chatlog data. This class is for you if you cannot exit your 
        /// code. When the client is done, it calls remove on the logger.
        /// </summary>
        public class ChatLoggerBase 
        {
            private String _name;

            // Input queue (must be locked to use)
            private Queue<ChatLine> _new_lines_queue;

            private uint _max_queue_length;

            public ChatLoggerBase(String iName)
            {
                if (iName == null)
                {
                    _name = "Unnamed Chatlogger";
                }
                else
                {
                    _name = iName;
                }
                _new_lines_queue = new Queue<ChatLine>();
                SynergyChatLogManager.Instance.ChatLoggerUpdate += new SynergyChatLogManager.ChatLoggerUpdateHandler(processNewChatLines);

                _max_queue_length = 500;
            }

            ~ChatLoggerBase()
            {
                SynergyChatLogManager.Instance.ChatLoggerUpdate -= new SynergyChatLogManager.ChatLoggerUpdateHandler(processNewChatLines);
            }

            public List<ChatLine> read()
            {
                List<ChatLine> chat_log = new List<ChatLine>();
                // Queue the new lines queue behind the current one, while dequeueing the old queue.
                lock (_new_lines_queue)
                {
                    if (_new_lines_queue.Count > 0)
                    {
                        int count = _new_lines_queue.Count;
                        for (int index = 0; index < count; index++)
                        {
                            chat_log.Add(_new_lines_queue.Dequeue());
                        }
                    }
                }
                return chat_log;
            }
            public void reset()
            {
                lock (_new_lines_queue)
                {
                    _new_lines_queue.Clear();
                }
            }

            // Thread-safe queueing of new chat lines.
            public void processNewChatLines(Object sender, ChatLineListEventArgs chatLines)
            {
                lock (_new_lines_queue)
                {
                    foreach (ChatLine chatLine in chatLines._Lines)
                    {
                        ChatLine clone = (ChatLine)chatLine.Clone();
                        _new_lines_queue.Enqueue(clone);

                        // Remove lines when our buffer overflows.
                        while (_new_lines_queue.Count > _max_queue_length)
                        {
                            _new_lines_queue.Dequeue();
                        }
                    }
                }
            }

            public uint MaxQueueLength
            {
                get
                {
                    return _max_queue_length;
                }
                set
                {
                    _max_queue_length = value;
                }
            }
        }
        #endregion SyncChatLogger
        #region ASyncChatLogger
        /// <summary>
        /// Functions as the SyncChatLogger, but it has a callback function for the client. Which does a deferred procedure
        /// call with a copy of the chatlines.
        /// </summary>
        public class ASyncChatLogger : ChatLoggerBase
        {
            // Have to add custom event handling?
            public ASyncChatLogger(String iName) : base( iName )
            {
                
            }
        }
        #endregion ASyncChatLogger
        #region SingletonDeclaration
        // Thread safe. We need an object anchor us to our parent thread.
        private static volatile SynergyChatLogManager instance;
        private static object syncRoot = new Object(); 

        public static SynergyChatLogManager Instance
        {
            get
            {
                if (instance == null )
                {
                    lock( syncRoot )
                    {
                        if( instance == null)
                        {
                            instance = new SynergyChatLogManager();
                        }
                        instance.Start(_chatPollingPeriod);
                    }
                }
                return instance;
            }
        }
        #endregion SingletonDeclaration
        #region ChatLineListEventArgs
        public class ChatLineListEventArgs : EventArgs
        {
            public ChatLineListEventArgs()
            {
                _Lines = new List<ChatLine>();
            }
            public List<ChatLine> _Lines;
        }
        #endregion ChatLineListEventArgs
        #region Events
        public delegate void ChatLoggerUpdateHandler(object Sender, ChatLineListEventArgs newLines);
        public event ChatLoggerUpdateHandler ChatLoggerUpdate;
        private void OnChatLoggerUpdate(List<ChatLine> newlines)
        {
            if (ChatLoggerUpdate != null)
            {
                ChatLineListEventArgs event_args = new ChatLineListEventArgs();
                event_args._Lines = newlines;
                ChatLoggerUpdate(this, event_args);
            }
        }
        #endregion Events
        #region Private Members
        private const UInt32 _chatPollingPeriod = 500;
        #endregion Private Members
        #region Constructor
        private SynergyChatLogManager()
        {
        }
        #endregion Constructor
        #region Public Members/Properties
        public UInt32 PollingPeriod
        {
            get
            {
                return _backgroundlogger_chat_polling_period;
            }
            set
            {
                _backgroundlogger_chat_polling_period = value;
            }
        }
        
        #endregion Public Members/Properties
    }
}
