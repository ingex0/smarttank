using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GameObjEditor
{
    public partial class EnterYourName : Form
    {
        bool selectYes;

        public EnterYourName (string labelText)
        {
            InitializeComponent();
            label1.Text = labelText;
        }

        public string CreatorName
        {
            get { return textBox1.Text; }
        }

        public bool SelectYes
        {
            get { return selectYes; }
        }

        private void Enter_Click ( object sender, EventArgs e )
        {
            selectYes = true;
            this.Close();
        }

        private void Cancel_Click ( object sender, EventArgs e )
        {
            selectYes = false;
            this.Close();
        }


    }
}