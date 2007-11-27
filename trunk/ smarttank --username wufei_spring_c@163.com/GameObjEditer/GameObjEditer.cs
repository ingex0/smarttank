using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Platform.GameObjects;

namespace GameObjEditer
{
    public partial class GameObjEditer : Form
    {
        List<GameObjData> gameObjs;

        public GameObjEditer ()
        {
            InitializeComponent();
            gameObjs = new List<GameObjData>();
        }

        internal void UpdateTreeView ()
        {
            if (gameObjs.Count == 0)
                return;

            foreach (GameObjData obj in gameObjs)
            {
                //treeView.Nodes.Add( obj.name );
            }
        }
    }
}