using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Iocaine2.Threading;

namespace Iocaine2.Bots
{
    public sealed partial class PowerLevel : Bot
    {
        private class PowerLevelBGThread
        {
            private Thread m_thread = null;
            private THREAD_STATE state = THREAD_STATE.UNSTARTED;

            public THREAD_STATE State
            {
                get
                {
                    return state;
                }
            }

            public PowerLevelBGThread(Statics.FuncPtrs.TD_Void_Void iRunFunction)
            {

            }

            public bool CheckState()
            {
                return true;
            }

        }
    }
}
