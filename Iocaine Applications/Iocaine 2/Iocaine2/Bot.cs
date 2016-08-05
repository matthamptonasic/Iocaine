using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Inventory;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Tools;

namespace Iocaine2.Bots
{
    #region Enums
    public enum STATE
    {
        STOPPED,
        RUNNING,
        PAUSED_USER_NOW,
        PAUSED_USER_WAIT,
        PAUSED_PROG,
        PAUSED_ALR
    }
    #endregion Enums
    public abstract class Bot
    {
        #region Constructor
        // Inherited classes must be singletons and implement a private constructor with zero parameters.
        //private Bot() {}
        #endregion Constructor
        #region Private Members
        // Inherited classes must be singletons and implement a private instance of the bot type and a lock object.
        //private static BotType instance = null;
        //private static readonly Object padlock = new Object();
        protected bool initIocaineDone = false;
        protected bool initProcessDone = false;
        protected bool initLoginDone = false;
        protected string initLoginChar = "";
        protected bool initParamsSet = false;
        protected STATE state = STATE.STOPPED;
        protected Thread runThread = null;
        protected bool abortThread = false;
        protected bool threadStopped = true;
        protected string name;
        protected Button c_startButton = null;
        protected Button c_stopButton = null;
        /// <summary>
        /// Set to false will cause the Start button text to stay as 'Pause' when the Stop button is clicked.
        /// The user can then set the button when activity is actually stopped. This is up to the bot designer's preference.
        /// Set to true will immediately set the Start button text to say 'Start' when the Stop button is clicked.
        /// </summary>
        protected Boolean toggleStartOnStopClick = true;
        #endregion Private Members
        #region Public Properties
        // Inherited classes must be singletons and implement a property to give access to the Bot instance.
        //public static BotType Access
        //{
        //    get
        //    {
        //        lock(padlock)
        //        {
        //            if(instance == null)
        //            {
        //                instance = new BotType();
        //            }
        //            return instance;
        //        }
        //    }
        //}
        public STATE State
        {
            get
            {
                return state;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
        }
        #endregion Public Properties
        #region Private Methods
        protected abstract void runThreadFunction();
        protected virtual bool checkState()
        {
            try
            {
                if (!ChangeMonitor.LoggedIn)
                {
                    Stop();
                    return false;
                }
                switch (state)
                {
                    case STATE.RUNNING:
                        return true;
                    case STATE.STOPPED:
                        return false;
                    case STATE.PAUSED_USER_WAIT:
                    case STATE.PAUSED_USER_NOW:
                    case STATE.PAUSED_PROG:
                    case STATE.PAUSED_ALR:
                        while (state == STATE.PAUSED_USER_WAIT
                            || state == STATE.PAUSED_USER_NOW
                            || state == STATE.PAUSED_PROG
                            || state == STATE.PAUSED_ALR)
                        {
                            IocaineFunctions.delay(500);
                        }
                        if (state == STATE.RUNNING)
                        {
                            return true;
                        }
                        if (state == STATE.STOPPED)
                        {
                            return false;
                        }
                        break;
                }
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Bot[" + name + "]::checkState: " + e.ToString());
                return false;
            }
        }
        protected abstract bool checkStatus();
        protected void setStartButton(string iText, System.Drawing.Color iColor)
        {
            try
            {
                if (c_startButton == null)
                {
                    return;
                }
                if (iColor == null)
                {
                    return;
                }
                if (c_startButton.InvokeRequired)
                {
                    c_startButton.Invoke(new Statics.FuncPtrs.TD_Void_String_Color(setStartButtonCBF), new object[] { iText, iColor });
                }
                else
                {
                    setStartButtonCBF(iText, iColor);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Bot[" + name + "]::setStartButton: " + e.ToString());
            }
        }
        private void setStartButtonCBF(string iText, System.Drawing.Color iColor)
        {
            try
            {
                c_startButton.UseMnemonic = true;
                c_startButton.Text = iText;
                c_startButton.BackColor = iColor;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Bot[" + name + "]::setStartButtonCBF: " + e.ToString());
            }
        }
        #endregion Private Methods
        #region Public Methods
        #region Inits
        public abstract void SetParams(Bots.BotParam iParam);
        public virtual bool Init_Iocaine()
        {
            initIocaineDone = true;
            return true;
        }
        public virtual bool Init_Process()
        {
            initProcessDone = true;
            return true;
        }
        public virtual bool Init_LoggedIn()
        {
            initLoginChar = PlayerCache.Vitals.Name;
            initLoginDone = true;
            return true;
        }
        protected virtual bool init_checkDone()
        {
            return (initIocaineDone & initProcessDone & initLoginDone & (initLoginChar == PlayerCache.Vitals.Name) & initParamsSet);
        }
        #endregion Inits
        #region Status
        public virtual bool Start()
        {
            try
            {
                if (!init_checkDone() || (state != STATE.STOPPED))
                {
                    return false;
                }
                // TBD: Do something like this. Make these pointers more generic.
                //Statics.FuncPtrs.SetSellerButtonPtr("&Pause", Statics.Buttons.Yellow);
                //Statics.FuncPtrs.SetStatusBoxPtr("Starting the Seller.", Statics.Fields.Green);
                abortThread = true;
                while (threadStopped == false)
                {
                    IocaineFunctions.delay(10);
                }
                abortThread = false;
                runThread = new Thread(new ThreadStart(runThreadFunction));
                runThread.Name = name + "RunThread";
                runThread.IsBackground = true;
                state = STATE.RUNNING;
                runThread.Start();
                setStartButton("&Pause", Statics.Buttons.Yellow);
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Bot[" + name + "]::Start: " + e.ToString());
                return false;
            }
        }
        public virtual void Stop()
        {
            try
            {
                if (state != STATE.STOPPED)
                {
                    state = STATE.STOPPED;
                    LoggingFunctions.Timestamp("Stopping the " + name + ".");
                    if (toggleStartOnStopClick)
                    {
                        setStartButton("S&tart", Statics.Buttons.Green);
                    }
                }   
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Bot[" + name + "]::Stop: " + e.ToString());
            }
        }
        public virtual void Pause(bool iNow = false)
        {
            try
            {
                if (iNow)
                {
                    state = STATE.PAUSED_USER_NOW;
                }
                else
                {
                    state = STATE.PAUSED_USER_WAIT;
                }
                setStartButton("&Resume", Statics.Buttons.Green);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Bot[" + name + "]::Pause: " + e.ToString());
            }
        }
        public virtual void PauseAlert()
        {
            try
            {
                state = STATE.PAUSED_ALR;
                setStartButton("&Resume", Statics.Buttons.Green);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Bot[" + name + "]::PauseAlert: " + e.ToString());
            }
        }
        public virtual void Resume()
        {
            try
            {
                state = STATE.RUNNING;
                setStartButton("&Pause", Statics.Buttons.Yellow);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Bot[" + name + "]::Resume: " + e.ToString());
            }
        }
        #endregion Status
        #region Settings
        public virtual void ShowSettingsDialog(Iocaine2.Iocaine_2_Form iParent)
        {
            // Default is no settings form.
            // Override to display your custom settings form.
        }
        #endregion Settings
        #endregion Public Methods
    }
}
