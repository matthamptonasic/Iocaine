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
        public static ActionSequence GetSequence(string iSequenceName)
        {
            foreach (ActionSequence seq in m_sequences)
            {
                if (seq.SequenceName == iSequenceName)
                {
                    return seq;
                }
            }
            return null;
        }
        #endregion Public Methods

        #region Private Methods
        private static void load_sneak_inv()
        {
            ActionSequence seq = new ActionSequence("Sneak_Invisible");

            // Cancel segment
            seq.AddAction(new ActionCancelBuff("invisible"));
            seq.AddAction(new ActionCancelBuff("sneak"));
            seq.AddAction(new ActionWait());

            // Snk/Inv choices
            ActionSequenceUnion snkInvUnion = new ActionSequenceUnion();
            ActionSequence jig = new ActionSequence();
            jig.AddAction(CommandManager.JAManager.GetCommand("Spectral Jig"));

            ActionSequence magic = new ActionSequence();
            magic.AddAction(CommandManager.SpellsManager.GetCommand("Sneak"));
            magic.AddAction(new ActionWait());
            magic.AddAction(CommandManager.SpellsManager.GetCommand("Invisible"));
            magic.AddAction(new ActionWait());

            ActionSequence item = new ActionSequence();
            Action.ConditionTree staticConditions = new Action.ConditionTree();
            item.AddAction(new UseItem("Silent Oil", "<me>", 2000));
            item.AddAction(new UseItem("Prism Powder", "<me>", 2000));

            snkInvUnion.AddSequence(jig);
            snkInvUnion.AddSequence(magic);
            snkInvUnion.AddSequence(item);

            seq.AddAction(snkInvUnion);

            m_sequences.Add(seq);
        }
        private static bool run_sneak_inv()
        {
            ActionSequence seq = GetSequence("Sneak_Invisible");
            if ((seq != null) && (seq.CanPerform()))
            {
                return seq.Execute("<me>");
            }
            return false;
        }
        #endregion Private Methods
    }
}