using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Iocaine2.Logging;

namespace Iocaine2.Data.Structures
{
    public static class Events
    {
        public class FfxiEvent
        {
            #region Constructors
            public FfxiEvent()
            {

            }
            public FfxiEvent(Trigger iTrigger, ActionSequence iSequence, bool iEnabled)
            {
                trigger = iTrigger;
                sequence = iSequence;
                enabled = iEnabled;
            }
            #endregion Constructors
            #region Private Members
            private ActionSequence sequence = null;
            private Trigger trigger = null;
            private bool enabled = false;
            #endregion Private Members
            #region Properties
            public ActionSequence Sequence
            {
                get
                {
                    return sequence;
                }
                set
                {
                    sequence = value;
                }
            }
            public Trigger Trig
            {
                get
                {
                    return trigger;
                }
                set
                {
                    trigger = value;
                }
            }
            public bool Enabled
            {
                get
                {
                    return enabled;
                }
                set
                {
                    enabled = value;
                }
            }
            #endregion Properties
            #region Methods
            public bool CheckEvent(FFXIEventArgs e)
            {
                if(!enabled)
                {
                    return false;
                }
                if((trigger != null) && (sequence != null))
                {
                    if(trigger.CheckTrigger(e))
                    {
                        //Do sequence
                        return true;
                    }
                }
                return false;
            }
            #endregion Methods
        }
    }
}
