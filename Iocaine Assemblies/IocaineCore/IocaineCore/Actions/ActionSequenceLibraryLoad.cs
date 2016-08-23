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
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Load Sequences
        private static void load_sequences()
        {
            ushort id = 0;
            string name = "";
            ActionSequence seq = null;

            #region Sneak_Invisible
            name = "Sneak_Invisible";
            seq = new ActionSequence(name);

            // Cancel segment
            seq.AddAction(new ActionCancelBuff("invisible"));
            seq.AddAction(new ActionCancelBuff("sneak"));
            seq.AddAction(new ActionWait());

            // Snk/Inv choices
            ActionSequenceUnion mainUnion = new ActionSequenceUnion();
            ActionSequence jig = new ActionSequence();
            jig.AddAction(CommandManager.JAManager.GetCommand("Spectral Jig"));

            ActionSequence magic = new ActionSequence();
            magic.AddAction(CommandManager.SpellsManager.GetCommand("Sneak"));
            magic.AddAction(CommandManager.SpellsManager.GetCommand("Invisible"));

            ActionSequence ninjutsu = new ActionSequence();
            ninjutsu.AddAction(CommandManager.SpellsManager.GetCommand("Monomi: Ichi"));
            ActionSequenceUnion tonkoUnion = new ActionSequenceUnion();
            ActionSequence tonkoIchi = new ActionSequence("Tonko: Ichi");
            tonkoIchi.AddAction(CommandManager.SpellsManager.GetCommand("Tonko: Ichi"));
            ActionSequence tonkoNi = new ActionSequence("Tonko: Ni");
            tonkoNi.AddAction(CommandManager.SpellsManager.GetCommand("Tonko: Ni"));
            tonkoUnion.AddSequence(tonkoNi);
            tonkoUnion.AddSequence(tonkoIchi);
            ninjutsu.AddAction(tonkoUnion);

            ActionSequence item = new ActionSequence();
            item.AddAction(new UseItem("Silent Oil", "<me>", 2000));
            item.AddAction(new UseItem("Prism Powder", "<me>", 2000));

            mainUnion.AddSequence(jig);
            mainUnion.AddSequence(magic);
            mainUnion.AddSequence(ninjutsu);
            mainUnion.AddSequence(item);

            seq.AddAction(mainUnion);

            m_nameToIdMap.Add(name, id);
            m_idToSequenceMap.Add(id, seq);
            m_nameToSequenceMap.Add(name, seq);
            id++;
            #endregion Sneak_Invisible

            #region Sneak
            name = "Sneak";
            seq = new ActionSequence(name);

            // Cancel segment
            seq.AddAction(new ActionCancelBuff("sneak"));
            seq.AddAction(new ActionWait());

            // Snk choices
            mainUnion = new ActionSequenceUnion();
            jig = new ActionSequence();
            jig.AddAction(CommandManager.JAManager.GetCommand("Spectral Jig"));

            magic = new ActionSequence();
            magic.AddAction(CommandManager.SpellsManager.GetCommand("Sneak"));
            magic.AddAction(new ActionWait());

            ninjutsu = new ActionSequence();
            ninjutsu.AddAction(CommandManager.SpellsManager.GetCommand("Monomi: Ichi"));

            item = new ActionSequence();
            item.AddAction(new UseItem("Silent Oil", "<me>", 2000));

            mainUnion.AddSequence(jig);
            mainUnion.AddSequence(magic);
            mainUnion.AddSequence(ninjutsu);
            mainUnion.AddSequence(item);

            seq.AddAction(mainUnion);

            m_nameToIdMap.Add(name, id);
            m_idToSequenceMap.Add(id, seq);
            m_nameToSequenceMap.Add(name, seq);
            id++;
            #endregion Sneak

            #region Invisible
            name = "Invisible";
            seq = new ActionSequence(name);

            // Cancel segment
            seq.AddAction(new ActionCancelBuff("invisible"));
            seq.AddAction(new ActionWait());

            // Snk/Inv choices
            mainUnion = new ActionSequenceUnion();
            jig = new ActionSequence();
            jig.AddAction(CommandManager.JAManager.GetCommand("Spectral Jig"));

            magic = new ActionSequence();
            magic.AddAction(CommandManager.SpellsManager.GetCommand("Invisible"));
            magic.AddAction(new ActionWait());

            ninjutsu = new ActionSequence();
            tonkoUnion.AddSequence(tonkoNi);
            tonkoUnion.AddSequence(tonkoIchi);
            ninjutsu.AddAction(tonkoUnion);

            item = new ActionSequence();
            item.AddAction(new UseItem("Prism Powder", "<me>", 2000));

            mainUnion.AddSequence(jig);
            mainUnion.AddSequence(magic);
            mainUnion.AddSequence(ninjutsu);
            mainUnion.AddSequence(item);

            seq.AddAction(mainUnion);

            m_nameToIdMap.Add(name, id);
            m_idToSequenceMap.Add(id, seq);
            m_nameToSequenceMap.Add(name, seq);
            id++;
            #endregion Invisible
        }
        #endregion Private Methods
    }
}