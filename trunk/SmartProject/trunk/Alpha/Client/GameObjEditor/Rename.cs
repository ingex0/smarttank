using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GameObjEditor
{
    public partial class Rename : Form
    {
        public Rename ()
        {
            InitializeComponent();
        }

        private void acceptBtn_Click ( object sender, EventArgs e )
        {
            if (textBox.Text.Length != 0)
                this.Close();
            else
                System.Media.SystemSounds.Beep.Play();
        }

        public string NameText
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

    }
}