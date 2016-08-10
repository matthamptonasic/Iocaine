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
    public static partial class ActionManager
    {
        #region Private Members
        private static Dictionary<ushort, ActionSequence> m_idToSequenceMap = new Dictionary<ushort, ActionSequence>();
        private static Dictionary<string, ushort> m_nameToIdMap = new Dictionary<string, ushort>();
        private static Dictionary<string, ActionSequence> m_nameToSequenceMap = new Dictionary<string, ActionSequence>();
        #endregion Private Members

        #region Public Properties
        public static List<ActionSequence> AllSequences
        {
            get
            {
                return m_idToSequenceMap.Values.ToList();
            }
        }
        #endregion Public Properties

        #region Inits
        public static void Init_Iocaine()
        {
            load_sequences();
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
        public static ActionSequence GetSequence(string iSequenceName)
        {
            if (m_nameToSequenceMap.ContainsKey(iSequenceName))
            {
                return m_nameToSequenceMap[iSequenceName];
            }
            else
            {
                return null;
            }
        }
        public static ActionSequence GetSequence(ushort iSequenceId)
        {
            if (m_idToSequenceMap.ContainsKey(iSequenceId))
            {
                return m_idToSequenceMap[iSequenceId];
            }
            else
            {
                return null;
            }
        }
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}