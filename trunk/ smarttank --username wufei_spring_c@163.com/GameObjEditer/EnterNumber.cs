using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GameObjEditer
{
    public partial class EnterNumber : Form
    {
        public EnterNumber ()
        {
            InitializeComponent();
        }

        public string NumberText
        {
            get { return textBox1.Text; }
        }

        private void enterBtn_Click ( object sender, EventArgs e )
        {
            Close();
        }
    }
}