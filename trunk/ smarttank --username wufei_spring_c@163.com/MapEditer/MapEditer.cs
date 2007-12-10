using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WeifenLuo.WinFormsUI.Docking;

namespace MapEditer
{
    public partial class MapEditer : Form
    {
        ObjPropertyPanel objPropertyPanel;
        GroupPanel groupPanel;
        ObjDisplayPanel objDisplayPanel;
        BackGroundPanel backGroundPanel;

        public MapEditer ()
        {
            InitializeComponent();
            InitializeChildForm();
        }

        private void InitializeChildForm ()
        {
            objPropertyPanel = new ObjPropertyPanel();
            groupPanel = new GroupPanel();
            objDisplayPanel = new ObjDisplayPanel();
            backGroundPanel = new BackGroundPanel();

            objPropertyPanel.Show( dockPanel, DockState.DockRightAutoHide );
            groupPanel.Show( dockPanel, DockState.DockLeftAutoHide );
            objDisplayPanel.Show( dockPanel, DockState.DockLeftAutoHide );
            backGroundPanel.Show( dockPanel, DockState.DockRightAutoHide );

        }

        private void toolStripButtonShowGroupPanel_Click ( object sender, EventArgs e )
        {
            groupPanel.Show( dockPanel, DockState.DockLeftAutoHide );
        }

        private void toolStripButtonShowBackGroundPanel_Click ( object sender, EventArgs e )
        {
            backGroundPanel.Show( dockPanel, DockState.DockRightAutoHide );
        }

        private void toolStripButtonShowObjCreatePanel_Click ( object sender, EventArgs e )
        {
            objDisplayPanel.Show( dockPanel, DockState.DockLeftAutoHide );
        }

        private void toolStripButtonShowObjPropertyPanel_Click ( object sender, EventArgs e )
        {
            objPropertyPanel.Show( dockPanel, DockState.DockRightAutoHide );
        }


    }
}