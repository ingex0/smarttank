using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace DeviceCanvas
{
    public partial class DeviceCanvas : Control
    {
        Timer timer = new Timer();

        public new event EventHandler Paint;

        public DeviceCanvas ()
        {
            InitializeComponent();
            timer.Interval = 30;
            timer.Enabled = true;
            timer.Tick += new EventHandler( timer_Tick );
        }

        void timer_Tick ( object sender, EventArgs e )
        {
            if (Paint != null)
                Paint( this, EventArgs.Empty );
        }


    }
}
