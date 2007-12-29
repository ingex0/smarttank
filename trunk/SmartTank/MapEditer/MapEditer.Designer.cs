namespace MapEditer
{
    partial class MapEditer
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose ( bool disposing )
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent ()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( MapEditer ) );
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ToolStripMenuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemOpenScene = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemSaveScene = new System.Windows.Forms.ToolStripMenuItem();
            this.视图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.组编辑面板ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.地面编辑面板ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.创建物体面板ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.物体属性面板ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.网格线ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonMoveCamera = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonZoomCamera = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRotaCamera = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonGroup = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCreate = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonMove = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonZoom = new System.Windows.Forms.ToolStripButton();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonShowGroupPanel = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShowMapPanel = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShowObjCreatePanel = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShowObjPropertyPanel = new System.Windows.Forms.ToolStripButton();
            this.MenuItemNewScene = new System.Windows.Forms.ToolStripMenuItem();
            this.openSceneDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveSceneDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemFile,
            this.视图ToolStripMenuItem} );
            this.menuStrip1.Location = new System.Drawing.Point( 0, 0 );
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size( 860, 24 );
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ToolStripMenuItemFile
            // 
            this.ToolStripMenuItemFile.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemNewScene,
            this.MenuItemOpenScene,
            this.MenuItemSaveScene} );
            this.ToolStripMenuItemFile.Name = "ToolStripMenuItemFile";
            this.ToolStripMenuItemFile.Size = new System.Drawing.Size( 43, 20 );
            this.ToolStripMenuItemFile.Text = "文件";
            // 
            // MenuItemOpenScene
            // 
            this.MenuItemOpenScene.Name = "MenuItemOpenScene";
            this.MenuItemOpenScene.Size = new System.Drawing.Size( 152, 22 );
            this.MenuItemOpenScene.Text = "打开场景";
            this.MenuItemOpenScene.Click += new System.EventHandler( this.MenuItemOpenScene_Click );
            // 
            // MenuItemSaveScene
            // 
            this.MenuItemSaveScene.Name = "MenuItemSaveScene";
            this.MenuItemSaveScene.Size = new System.Drawing.Size( 152, 22 );
            this.MenuItemSaveScene.Text = "保存场景";
            this.MenuItemSaveScene.Click += new System.EventHandler( this.MenuItemSaveScene_Click );
            // 
            // 视图ToolStripMenuItem
            // 
            this.视图ToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.组编辑面板ToolStripMenuItem,
            this.地面编辑面板ToolStripMenuItem,
            this.创建物体面板ToolStripMenuItem,
            this.物体属性面板ToolStripMenuItem,
            this.toolStripSeparator3,
            this.网格线ToolStripMenuItem} );
            this.视图ToolStripMenuItem.Name = "视图ToolStripMenuItem";
            this.视图ToolStripMenuItem.Size = new System.Drawing.Size( 43, 20 );
            this.视图ToolStripMenuItem.Text = "视图";
            // 
            // 组编辑面板ToolStripMenuItem
            // 
            this.组编辑面板ToolStripMenuItem.Name = "组编辑面板ToolStripMenuItem";
            this.组编辑面板ToolStripMenuItem.Size = new System.Drawing.Size( 146, 22 );
            this.组编辑面板ToolStripMenuItem.Text = "组编辑面板";
            // 
            // 地面编辑面板ToolStripMenuItem
            // 
            this.地面编辑面板ToolStripMenuItem.Name = "地面编辑面板ToolStripMenuItem";
            this.地面编辑面板ToolStripMenuItem.Size = new System.Drawing.Size( 146, 22 );
            this.地面编辑面板ToolStripMenuItem.Text = "地面编辑面板";
            // 
            // 创建物体面板ToolStripMenuItem
            // 
            this.创建物体面板ToolStripMenuItem.Name = "创建物体面板ToolStripMenuItem";
            this.创建物体面板ToolStripMenuItem.Size = new System.Drawing.Size( 146, 22 );
            this.创建物体面板ToolStripMenuItem.Text = "创建物体面板";
            // 
            // 物体属性面板ToolStripMenuItem
            // 
            this.物体属性面板ToolStripMenuItem.Name = "物体属性面板ToolStripMenuItem";
            this.物体属性面板ToolStripMenuItem.Size = new System.Drawing.Size( 146, 22 );
            this.物体属性面板ToolStripMenuItem.Text = "物体属性面板";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size( 143, 6 );
            // 
            // 网格线ToolStripMenuItem
            // 
            this.网格线ToolStripMenuItem.Name = "网格线ToolStripMenuItem";
            this.网格线ToolStripMenuItem.Size = new System.Drawing.Size( 146, 22 );
            this.网格线ToolStripMenuItem.Text = "网格线";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add( this.statusStrip1 );
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add( this.dockPanel );
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size( 860, 375 );
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point( 0, 24 );
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size( 860, 447 );
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add( this.toolStrip3 );
            this.toolStripContainer1.TopToolStripPanel.Controls.Add( this.toolStrip2 );
            this.toolStripContainer1.TopToolStripPanel.Controls.Add( this.toolStrip1 );
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Location = new System.Drawing.Point( 0, 0 );
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size( 860, 22 );
            this.statusStrip1.TabIndex = 0;
            // 
            // dockPanel
            // 
            this.dockPanel.ActiveAutoHideContent = null;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingSdi;
            this.dockPanel.Location = new System.Drawing.Point( 0, 0 );
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size( 860, 375 );
            this.dockPanel.TabIndex = 0;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonMoveCamera,
            this.toolStripButtonZoomCamera,
            this.toolStripButtonRotaCamera} );
            this.toolStrip2.Location = new System.Drawing.Point( 69, 0 );
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size( 81, 25 );
            this.toolStrip2.TabIndex = 1;
            // 
            // toolStripButtonMoveCamera
            // 
            this.toolStripButtonMoveCamera.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMoveCamera.Image = ((System.Drawing.Image)(resources.GetObject( "toolStripButtonMoveCamera.Image" )));
            this.toolStripButtonMoveCamera.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMoveCamera.Name = "toolStripButtonMoveCamera";
            this.toolStripButtonMoveCamera.Size = new System.Drawing.Size( 23, 22 );
            this.toolStripButtonMoveCamera.Text = "toolStripButton1";
            this.toolStripButtonMoveCamera.ToolTipText = "平移摄像机";
            // 
            // toolStripButtonZoomCamera
            // 
            this.toolStripButtonZoomCamera.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZoomCamera.Image = ((System.Drawing.Image)(resources.GetObject( "toolStripButtonZoomCamera.Image" )));
            this.toolStripButtonZoomCamera.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoomCamera.Name = "toolStripButtonZoomCamera";
            this.toolStripButtonZoomCamera.Size = new System.Drawing.Size( 23, 22 );
            this.toolStripButtonZoomCamera.Text = "toolStripButton2";
            this.toolStripButtonZoomCamera.ToolTipText = "缩放摄像机";
            // 
            // toolStripButtonRotaCamera
            // 
            this.toolStripButtonRotaCamera.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRotaCamera.Image = ((System.Drawing.Image)(resources.GetObject( "toolStripButtonRotaCamera.Image" )));
            this.toolStripButtonRotaCamera.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRotaCamera.Name = "toolStripButtonRotaCamera";
            this.toolStripButtonRotaCamera.Size = new System.Drawing.Size( 23, 22 );
            this.toolStripButtonRotaCamera.Text = "toolStripButton3";
            this.toolStripButtonRotaCamera.ToolTipText = "旋转摄像机";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonGroup,
            this.toolStripSeparator1,
            this.toolStripButtonCreate,
            this.toolStripButtonDel,
            this.toolStripSeparator2,
            this.toolStripButtonMove,
            this.toolStripButtonZoom} );
            this.toolStrip1.Location = new System.Drawing.Point( 3, 25 );
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size( 139, 25 );
            this.toolStrip1.TabIndex = 0;
            // 
            // toolStripButtonGroup
            // 
            this.toolStripButtonGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonGroup.Image = ((System.Drawing.Image)(resources.GetObject( "toolStripButtonGroup.Image" )));
            this.toolStripButtonGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGroup.Name = "toolStripButtonGroup";
            this.toolStripButtonGroup.Size = new System.Drawing.Size( 23, 22 );
            this.toolStripButtonGroup.Text = "toolStripButton1";
            this.toolStripButtonGroup.ToolTipText = "编辑组";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size( 6, 25 );
            // 
            // toolStripButtonCreate
            // 
            this.toolStripButtonCreate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCreate.Image = ((System.Drawing.Image)(resources.GetObject( "toolStripButtonCreate.Image" )));
            this.toolStripButtonCreate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCreate.Name = "toolStripButtonCreate";
            this.toolStripButtonCreate.Size = new System.Drawing.Size( 23, 22 );
            this.toolStripButtonCreate.Text = "toolStripButton1";
            this.toolStripButtonCreate.ToolTipText = "创建新的场景物体";
            // 
            // toolStripButtonDel
            // 
            this.toolStripButtonDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDel.Image = ((System.Drawing.Image)(resources.GetObject( "toolStripButtonDel.Image" )));
            this.toolStripButtonDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDel.Name = "toolStripButtonDel";
            this.toolStripButtonDel.Size = new System.Drawing.Size( 23, 22 );
            this.toolStripButtonDel.Text = "toolStripButton1";
            this.toolStripButtonDel.ToolTipText = "删除当前场景物体";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size( 6, 25 );
            // 
            // toolStripButtonMove
            // 
            this.toolStripButtonMove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMove.Image = ((System.Drawing.Image)(resources.GetObject( "toolStripButtonMove.Image" )));
            this.toolStripButtonMove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMove.Name = "toolStripButtonMove";
            this.toolStripButtonMove.Size = new System.Drawing.Size( 23, 22 );
            this.toolStripButtonMove.Text = "toolStripButton2";
            this.toolStripButtonMove.ToolTipText = "移动";
            // 
            // toolStripButtonZoom
            // 
            this.toolStripButtonZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZoom.Image = ((System.Drawing.Image)(resources.GetObject( "toolStripButtonZoom.Image" )));
            this.toolStripButtonZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoom.Name = "toolStripButtonZoom";
            this.toolStripButtonZoom.Size = new System.Drawing.Size( 23, 22 );
            this.toolStripButtonZoom.Text = "toolStripButton3";
            this.toolStripButtonZoom.ToolTipText = "缩放";
            // 
            // toolStrip3
            // 
            this.toolStrip3.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip3.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonShowGroupPanel,
            this.toolStripButtonShowMapPanel,
            this.toolStripButtonShowObjCreatePanel,
            this.toolStripButtonShowObjPropertyPanel} );
            this.toolStrip3.Location = new System.Drawing.Point( 142, 25 );
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size( 104, 25 );
            this.toolStrip3.TabIndex = 2;
            // 
            // toolStripButtonShowGroupPanel
            // 
            this.toolStripButtonShowGroupPanel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowGroupPanel.Image = ((System.Drawing.Image)(resources.GetObject( "toolStripButtonShowGroupPanel.Image" )));
            this.toolStripButtonShowGroupPanel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowGroupPanel.Name = "toolStripButtonShowGroupPanel";
            this.toolStripButtonShowGroupPanel.Size = new System.Drawing.Size( 23, 22 );
            this.toolStripButtonShowGroupPanel.Text = "toolStripButton1";
            this.toolStripButtonShowGroupPanel.ToolTipText = "显示组编辑面板";
            this.toolStripButtonShowGroupPanel.Click += new System.EventHandler( this.toolStripButtonShowGroupPanel_Click );
            // 
            // toolStripButtonShowMapPanel
            // 
            this.toolStripButtonShowMapPanel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowMapPanel.Image = ((System.Drawing.Image)(resources.GetObject( "toolStripButtonShowMapPanel.Image" )));
            this.toolStripButtonShowMapPanel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowMapPanel.Name = "toolStripButtonShowMapPanel";
            this.toolStripButtonShowMapPanel.Size = new System.Drawing.Size( 23, 22 );
            this.toolStripButtonShowMapPanel.Text = "toolStripButton2";
            this.toolStripButtonShowMapPanel.ToolTipText = "显示地图编辑面板";
            this.toolStripButtonShowMapPanel.Click += new System.EventHandler( this.toolStripButtonShowBackGroundPanel_Click );
            // 
            // toolStripButtonShowObjCreatePanel
            // 
            this.toolStripButtonShowObjCreatePanel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowObjCreatePanel.Image = ((System.Drawing.Image)(resources.GetObject( "toolStripButtonShowObjCreatePanel.Image" )));
            this.toolStripButtonShowObjCreatePanel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowObjCreatePanel.Name = "toolStripButtonShowObjCreatePanel";
            this.toolStripButtonShowObjCreatePanel.Size = new System.Drawing.Size( 23, 22 );
            this.toolStripButtonShowObjCreatePanel.Text = "toolStripButton4";
            this.toolStripButtonShowObjCreatePanel.ToolTipText = "显示物体创建面板";
            this.toolStripButtonShowObjCreatePanel.Click += new System.EventHandler( this.toolStripButtonShowObjCreatePanel_Click );
            // 
            // toolStripButtonShowObjPropertyPanel
            // 
            this.toolStripButtonShowObjPropertyPanel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowObjPropertyPanel.Image = ((System.Drawing.Image)(resources.GetObject( "toolStripButtonShowObjPropertyPanel.Image" )));
            this.toolStripButtonShowObjPropertyPanel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowObjPropertyPanel.Name = "toolStripButtonShowObjPropertyPanel";
            this.toolStripButtonShowObjPropertyPanel.Size = new System.Drawing.Size( 23, 22 );
            this.toolStripButtonShowObjPropertyPanel.Text = "toolStripButton3";
            this.toolStripButtonShowObjPropertyPanel.ToolTipText = "显示物体属性面板";
            this.toolStripButtonShowObjPropertyPanel.Click += new System.EventHandler( this.toolStripButtonShowObjPropertyPanel_Click );
            // 
            // MenuItemNewScene
            // 
            this.MenuItemNewScene.Name = "MenuItemNewScene";
            this.MenuItemNewScene.Size = new System.Drawing.Size( 152, 22 );
            this.MenuItemNewScene.Text = "新建场景";
            this.MenuItemNewScene.Click += new System.EventHandler( this.MenuItemNewScene_Click );
            // 
            // openSceneDialog
            // 
            this.openSceneDialog.Filter = "\"Scene文件|*.scene\"";
            // 
            // MapEditer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 860, 471 );
            this.Controls.Add( this.toolStripContainer1 );
            this.Controls.Add( this.menuStrip1 );
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MapEditer";
            this.Text = "MapEditer";
            this.menuStrip1.ResumeLayout( false );
            this.menuStrip1.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout( false );
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout( false );
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout( false );
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout( false );
            this.toolStripContainer1.PerformLayout();
            this.toolStrip2.ResumeLayout( false );
            this.toolStrip2.PerformLayout();
            this.toolStrip1.ResumeLayout( false );
            this.toolStrip1.PerformLayout();
            this.toolStrip3.ResumeLayout( false );
            this.toolStrip3.PerformLayout();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemFile;
        private System.Windows.Forms.ToolStripButton toolStripButtonCreate;
        private System.Windows.Forms.ToolStripButton toolStripButtonDel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonMove;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoom;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonGroup;
        private System.Windows.Forms.ToolStripMenuItem 视图ToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButtonMoveCamera;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoomCamera;
        private System.Windows.Forms.ToolStripButton toolStripButtonRotaCamera;
        private System.Windows.Forms.ToolStripMenuItem 组编辑面板ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 地面编辑面板ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 创建物体面板ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 物体属性面板ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem 网格线ToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowGroupPanel;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowMapPanel;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowObjPropertyPanel;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowObjCreatePanel;
        private System.Windows.Forms.ToolStripMenuItem MenuItemOpenScene;
        private System.Windows.Forms.ToolStripMenuItem MenuItemSaveScene;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.ToolStripMenuItem MenuItemNewScene;
        private System.Windows.Forms.OpenFileDialog openSceneDialog;
        private System.Windows.Forms.SaveFileDialog saveSceneDialog;
    }
}