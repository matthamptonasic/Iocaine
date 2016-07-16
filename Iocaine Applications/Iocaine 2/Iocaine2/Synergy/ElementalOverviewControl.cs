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
    public partial class ElementalOverviewControl : UserControl
    {
        private delegate void updateDelegate(ElementalBalance balance);
        private updateDelegate _updateDelegate;
        private int _fewell_max;
        private int _fewell_min;

        public ElementalOverviewControl()
        {
            InitializeComponent();

            _fewell_max = 99;
            _fewell_min = -99;

            _updateDelegate = new updateDelegate(updateCB);
        }
        public int Fewell_Max
        {
            get
            {
                return _fewell_max;
            }
            set
            {
                _fewell_max = value;
            }
        }
        public int Fewell_Min
        {
            get
            {
                return _fewell_min;
            }
            set
            {
                _fewell_min = value;
            }
        }
        public int Fire
        {
            set
            {
                int clipped_value = Math.Min(_fewell_max, Math.Max(_fewell_min, value));
                valueFire.Text = clipped_value.ToString();
            }
        }
        public int Ice
        {
            set
            {
                int clipped_value = Math.Min(_fewell_max, Math.Max(_fewell_min, value));
                valueIce.Text = clipped_value.ToString();
            }
        }
        public int Wind
        {
            set
            {
                int clipped_value = Math.Min(_fewell_max, Math.Max(_fewell_min, value));
                valueWind.Text = clipped_value.ToString();
            }
        }
        public int Earth
        {
            set
            {
                int clipped_value = Math.Min(_fewell_max, Math.Max(_fewell_min, value));
                valueEarth.Text = clipped_value.ToString();
            }
        }
        public int Lightning
        {
            set
            {
                int clipped_value = Math.Min(_fewell_max, Math.Max(_fewell_min, value));
                valueLightning.Text = clipped_value.ToString();
            }
        }
        public int Water
        {
            set
            {
                int clipped_value = Math.Min(_fewell_max, Math.Max(_fewell_min, value));
                valueWater.Text = clipped_value.ToString();
            }
        }
        public int Light
        {
            set
            {
                int clipped_value = Math.Min(_fewell_max, Math.Max(_fewell_min, value));
                valueLight.Text = clipped_value.ToString();
            }
        }
        public int Dark
        {
            set
            {
                int clipped_value = Math.Min(_fewell_max, Math.Max(_fewell_min, value));
                valueDark.Text = clipped_value.ToString();
            }
        }

        private void updateCB(ElementalBalance balance)
        {
            Fire = balance.Fire;
            Ice = balance.Ice;
            Wind = balance.Wind;
            Earth = balance.Earth;
            Lightning = balance.Lightning;
            Water = balance.Water;
            Light = balance.Light;
            Dark = balance.Dark;
        }

        public void updateLevels( ElementalBalance balance )
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(_updateDelegate, new object[] { balance });
                }
                else
                {
                    updateCB( balance );
                }
            }
            catch (Exception e)
            {
               Iocaine2.Logging.LoggingFunctions.Error("Clearing Recipe in Manager: " + e.ToString());
            }
        }

    }
}
