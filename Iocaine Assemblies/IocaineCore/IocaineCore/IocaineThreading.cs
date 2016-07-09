using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Iocaine2.Threading
{
    /// <summary>
    /// For use with background threads (not bot threads) that need to be paused
    /// when the selected POL process is changed via the user.
    /// The class can be instantiated as is. In this case the __CheckState
    /// function pointer must be set and used in the __RunMethod's loop.
    /// The class can be used as a base class.  In this case the __CheckState
    /// should be used as is within the __RunMethod's loop.
    /// In both cases the user must specify the __RunMethod just like a normal Thread object.
    /// </summary>
    public class IocaineThread : IThreadable
    {
        // Within the thread's run method, we'll do a pause check every iteration of the loop.
        // We'll publish a 'Freeze' method that sets the status to 'FreezeRequested'.
        // Once we enter the pause check area, if the status is 'FreezeRequested' we enter
        // the 'Frozen' state.
        // There we wait until we get a 'Thaw' or 'Resume' call which will interrupt the thread
        // using the method covered here:
        // https://msdn.microsoft.com/en-us/library/tttdef8x(v=vs.110).aspx

        #region Private Members
        private Thread m_thread = null;
        private string m_threadName;
        protected volatile THREAD_STATE m_state = THREAD_STATE.UNSTARTED;
        private Statics.FuncPtrs.TD_Void_Void __runMethod;
        private static readonly object m_padlock_state = new object();
        #endregion Private Members

        #region Public Properties
        public THREAD_STATE State
        {
            get
            {
                return m_state;
            }
        }
        public Statics.FuncPtrs.TD_Void_Void __RunMethod
        {
            set
            {
                __runMethod = value;
            }
        }
        public Statics.FuncPtrs.TD_Bool_Void __CheckState
        {
            get
            {
                return checkState;
            }
        }
        #endregion Public Properties

        #region Constructor
        public IocaineThread(string iName)
        {
            m_threadName = iName;
            ThreadManager.RegisterObject(this);
        }
        #endregion Constructor

        #region IThreadable Methods
        public void Start()
        {
            if ((m_state != THREAD_STATE.UNSTARTED) && (m_state != THREAD_STATE.STOPPED))
            {
                return;
            }
            if (__runMethod == null)
            {
                return;
            }
            if (m_thread == null)
            {
                m_thread = new Thread(new ThreadStart(__runMethod));
                m_thread.IsBackground = true;
                m_thread.Name = m_threadName;
                m_state = THREAD_STATE.RUNNING;
                m_thread.Start();
            }
            return;
        }
        public void Stop()
        {
            Monitor.Enter(m_padlock_state);
            switch (m_state)
            {
                case THREAD_STATE.FROZEN:
                    m_thread.Abort();
                    break;
                case THREAD_STATE.UNSTARTED:
                case THREAD_STATE.STOPPED:
                    break;
                default:
                    m_state = THREAD_STATE.STOP_REQUESTED;
                    break;
            }
            Monitor.Exit(m_padlock_state);
        }
        public void Freeze()
        {
            Monitor.Enter(m_padlock_state);
            switch (m_state)
            {
                case THREAD_STATE.UNSTARTED:
                case THREAD_STATE.STOPPED:
                case THREAD_STATE.FROZEN:
                case THREAD_STATE.FREEZE_REQUESTED:
                    break;
                default:
                    m_state = THREAD_STATE.FREEZE_REQUESTED;
                    break;
            }
            Monitor.Exit(m_padlock_state);
        }
        public void Thaw()
        {
            Monitor.Enter(m_padlock_state);
            switch (m_state)
            {
                case THREAD_STATE.FROZEN:
                    m_thread.Interrupt();
                    break;
                case THREAD_STATE.UNSTARTED:
                case THREAD_STATE.STOPPED:
                case THREAD_STATE.THAW_REQUESTED:
                    break;
                default:
                    m_state = THREAD_STATE.THAW_REQUESTED;
                    break;
            }
            Monitor.Exit(m_padlock_state);
        }
        #endregion IThreadable Methods

        #region Private Methods
        protected bool checkState()
        {
            Monitor.Enter(m_padlock_state);
            switch (m_state)
            {
                case THREAD_STATE.RUNNING:
                    Monitor.Exit(m_padlock_state);
                    return true;
                case THREAD_STATE.THAW_REQUESTED:
                    m_state = THREAD_STATE.RUNNING;
                    Monitor.Exit(m_padlock_state);
                    return true;
                case THREAD_STATE.FROZEN:
                case THREAD_STATE.FREEZE_REQUESTED:
                    m_state = THREAD_STATE.FROZEN;
                    try
                    {
                        Monitor.Exit(m_padlock_state);
                        Thread.Sleep(Timeout.Infinite);
                    }
                    catch (ThreadInterruptedException)
                    {
                        m_state = THREAD_STATE.RUNNING;
                        return true;
                    }
                    catch (ThreadAbortException)
                    {
                        m_state = THREAD_STATE.STOPPED;
                        // The thread ends here, will not continue after the catch block (except for 'finally' if exists).
                    }
                    break;
                case THREAD_STATE.STOPPED:
                case THREAD_STATE.STOP_REQUESTED:
                case THREAD_STATE.UNSTARTED:
                    Monitor.Exit(m_padlock_state);
                    return false;
            }
            Monitor.Exit(m_padlock_state);
            return false;
        }
        #endregion Private Methods
    }

    public enum THREAD_STATE : byte
    {
        UNSTARTED,
        RUNNING,
        FREEZE_REQUESTED,
        FROZEN,
        THAW_REQUESTED,
        STOP_REQUESTED,
        STOPPED
    }
    public interface IThreadable
    {
        void Start();
        void Stop();
        void Freeze();
        void Thaw();
        THREAD_STATE State
        {
            get;
        }
    }
    public static class ThreadManager
    {
        #region Private Members
        private static List<IThreadable> m_bgThreadList = new List<IThreadable>();
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Public Methods
        public static void RegisterObject(IThreadable iObject)
        {
            if (!m_bgThreadList.Contains(iObject))
            {
                m_bgThreadList.Add(iObject);
            }
        }
        public static void FreezeAll(bool iBlock = true)
        {
            foreach (IThreadable obj in m_bgThreadList)
            {
                obj.Freeze();
            }
            if (iBlock)
            {
                foreach(IThreadable obj in m_bgThreadList)
                {
                    bool notRunning = (obj.State == THREAD_STATE.FROZEN) || (obj.State == THREAD_STATE.STOPPED) || (obj.State == THREAD_STATE.UNSTARTED);
                    while (notRunning == false)
                    {
                        Thread.Sleep(10);
                        notRunning = (obj.State == THREAD_STATE.FROZEN) || (obj.State == THREAD_STATE.STOPPED) || (obj.State == THREAD_STATE.UNSTARTED);
                    }
                }
            }
        }
        public static void ThawAll()
        {
            foreach (IThreadable obj in m_bgThreadList)
            {
                obj.Thaw();
            }
        }
        #endregion Public Methods
    }
}
