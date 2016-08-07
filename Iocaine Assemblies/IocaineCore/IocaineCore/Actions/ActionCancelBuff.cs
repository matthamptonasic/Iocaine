using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Data.Structures
{
    public class ActionCancelBuff : Action
    {
        #region Private Members
        private StatusEffects.STATUS_EFFECT_INFO m_buff;
        private const uint mc_delayAfterCancel = 1500;
        #endregion Private Members

        #region Public Properties
        public StatusEffects.STATUS_EFFECT_INFO Buff
        {
            get
            {
                return m_buff;
            }
        }
        #endregion Public Properties

        #region Constructors
        public ActionCancelBuff(ushort iBuffId)
            : base(true, ACTN_TYPE.Cancel_Buff)
        {
            m_buff = StatusEffects.GetStatusEffectInfo(iBuffId);
            setConditionTrees();
        }
        public ActionCancelBuff(string iBuffName)
            : base(true, ACTN_TYPE.Cancel_Buff)
        {
            m_buff = StatusEffects.GetStatusEffectInfo(iBuffName);
            setConditionTrees();
        }
        public ActionCancelBuff(ActionCancelBuff iAction)
            : base(true, ACTN_TYPE.Cancel_Buff)
        {
            m_buff = iAction.Buff;
            setConditionTrees();
        }
        #endregion Constructors

        #region Public Methods
        public override bool Execute(string iTarget = "")
        {
            ushort[] buffs = null;
            Memory.MemReads.Self.StatusEffects.get_effects(ref buffs);
            if (buffs.Contains(m_buff.Index))
            {
                Memory.Interface.IocaineFunctions.keys("//cancel " + m_buff.Index);
                Memory.Interface.IocaineFunctions.delay(mc_delayAfterCancel);
            }
            return true;
        }
        public override void Show()
        {
            MessageBox.Show(this.ToString(), "ActionCancelBuff::" + m_buff.Index, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override string SaveString()
        {
            return "ActionCancelBuff;" + m_buff.Index;
        }
        public override string ToString()
        {
            return m_buff.Index.ToString();
        }
        #endregion Public Methods

        #region Private Methods
        private void setConditionTrees()
        {
            setConditions(new ConditionTree(), new ConditionTree());
        }
        #endregion Private Methods
    }
}