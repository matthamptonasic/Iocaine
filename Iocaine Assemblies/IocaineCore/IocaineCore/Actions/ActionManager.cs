using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Data.Structures
{
    #region Enums
    #endregion Enums
    public static class ActionManager
    {
        #region Private Members
        private static List<ActionSequence> m_sequences = new List<ActionSequence>();
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Inits
        public static void Init_Iocaine()
        {

        }
        public static void Init_Process()
        {
            //Stub
        }
        public static void Init_LoggedIn()
        {
            //Stub
        }
        #endregion Inits

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        private static void load_sneak_inv()
        {
            ActionSequence seq = new ActionSequence();
            seq.AddAction(new ActionCancelBuff("invisible"));
            seq.AddAction(new ActionCancelBuff("sneak"));
            seq.AddAction(new ActionWait());

        }
        #endregion Private Methods
    }
}