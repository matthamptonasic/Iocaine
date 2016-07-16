using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Iocaine2.Synergy
{
    public partial class RecipeRequirementsOverviewControl : UserControl
    {
        private delegate void updateDelegate(FFXIEnums.CRAFT_RANK rank, String Name, ushort Quantity, ElementalBalance balance);
        private updateDelegate _updateDelegate;

        public RecipeRequirementsOverviewControl()
        {
            InitializeComponent();

            _updateDelegate = new updateDelegate(updateCB);
        }

        public void update(FFXIEnums.CRAFT_RANK rank, String Name, ushort Quantity, ElementalBalance balance)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(_updateDelegate, new object[] { rank, Name, Quantity, balance });
                }
                else
                {
                    updateCB(rank, Name, Quantity, balance);
                }
            }
            catch (Exception e)
            {
                Iocaine2.Logging.LoggingFunctions.Error("Clearing Recipe in Manager: " + e.ToString());
            }
        }

        private void updateCB(FFXIEnums.CRAFT_RANK rank, String Name, ushort Quantity, ElementalBalance balance)
        {
            elementalOverviewControl.updateLevels(balance);
            RankLabel.Text = Iocaine2.Memory.MemReads.Self.Skills.Crafting.get_rank_id_to_string(rank);
            RecipeResultLabel.Text = "" + Quantity.ToString() + " x " + Name;
        }
    }
}
