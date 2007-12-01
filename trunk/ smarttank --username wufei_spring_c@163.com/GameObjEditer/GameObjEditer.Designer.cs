namespace GameObjEditer
{
    partial class GameObjEditer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( GameObjEditer ) );
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.texToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.borderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.texTab = new System.Windows.Forms.TabPage();
            this.listViewTex = new System.Windows.Forms.ListView();
            this.texIndex = new System.Windows.Forms.ColumnHeader();
            this.texName = new System.Windows.Forms.ColumnHeader();
            this.visiPointTab = new System.Windows.Forms.TabPage();
            this.listViewVisi = new System.Windows.Forms.ListView();
            this.visiIndex = new System.Windows.Forms.ColumnHeader();
            this.visiPos = new System.Windows.Forms.ColumnHeader();
            this.structPointTab = new System.Windows.Forms.TabPage();
            this.listViewStruct = new System.Windows.Forms.ListView();
            this.structIndex = new System.Windows.Forms.ColumnHeader();
            this.structPos = new System.Windows.Forms.ColumnHeader();
            this.pictureBox = new PictureBoxGird.PictureBoxGird();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.openTexDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.RightToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.texTab.SuspendLayout();
            this.visiPointTab.SuspendLayout();
            this.structPointTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.texToolStripMenuItem} );
            this.menuStrip.Location = new System.Drawing.Point( 0, 0 );
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size( 788, 24 );
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem} );
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size( 43, 20 );
            this.fileToolStripMenuItem.Text = "文件";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size( 152, 22 );
            this.openToolStripMenuItem.Text = "打开";
            this.openToolStripMenuItem.Click += new System.EventHandler( this.OpenToolStripMenuItem_Click );
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size( 152, 22 );
            this.saveToolStripMenuItem.Text = "保存";
            this.saveToolStripMenuItem.Click += new System.EventHandler( this.SaveToolStripMenuItem_Click );
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size( 152, 22 );
            this.exitToolStripMenuItem.Text = "退出";
            this.exitToolStripMenuItem.Click += new System.EventHandler( this.ExitToolStripMenuItem_Click );
            // 
            // texToolStripMenuItem
            // 
            this.texToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.borderToolStripMenuItem} );
            this.texToolStripMenuItem.Name = "texToolStripMenuItem";
            this.texToolStripMenuItem.Size = new System.Drawing.Size( 43, 20 );
            this.texToolStripMenuItem.Text = "贴图";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size( 152, 22 );
            this.importToolStripMenuItem.Text = "导入";
            this.importToolStripMenuItem.Click += new System.EventHandler( this.ImportToolStripMenuItem_Click );
            // 
            // borderToolStripMenuItem
            // 
            this.borderToolStripMenuItem.Name = "borderToolStripMenuItem";
            this.borderToolStripMenuItem.Size = new System.Drawing.Size( 152, 22 );
            this.borderToolStripMenuItem.Text = "提取边界";
            this.borderToolStripMenuItem.Click += new System.EventHandler( this.BorderToolStripMenuItem_Click );
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add( this.statusStrip );
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add( this.splitContainer1 );
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size( 762, 486 );
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point( 0, 24 );
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.RightToolStripPanel
            // 
            this.toolStripContainer1.RightToolStripPanel.Controls.Add( this.toolStrip );
            this.toolStripContainer1.Size = new System.Drawing.Size( 788, 508 );
            this.toolStripContainer1.TabIndex = 4;
            this.toolStripContainer1.Text = "toolStripContainer1";
            this.toolStripContainer1.TopToolStripPanelVisible = false;
            // 
            // statusStrip
            // 
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip.Location = new System.Drawing.Point( 0, 0 );
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size( 788, 22 );
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point( 0, 0 );
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add( this.splitContainer2 );
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add( this.pictureBox );
            this.splitContainer1.Size = new System.Drawing.Size( 762, 486 );
            this.splitContainer1.SplitterDistance = 236;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point( 0, 0 );
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add( this.treeView );
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add( this.tabControl1 );
            this.splitContainer2.Size = new System.Drawing.Size( 236, 486 );
            this.splitContainer2.SplitterDistance = 219;
            this.splitContainer2.TabIndex = 1;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.HideSelection = false;
            this.treeView.HotTracking = true;
            this.treeView.Location = new System.Drawing.Point( 0, 0 );
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size( 236, 219 );
            this.treeView.TabIndex = 0;
            this.treeView.MouseClick += new System.Windows.Forms.MouseEventHandler( this.treeView_MouseClick );
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler( this.treeView_AfterSelect );
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add( this.texTab );
            this.tabControl1.Controls.Add( this.visiPointTab );
            this.tabControl1.Controls.Add( this.structPointTab );
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point( 0, 0 );
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size( 236, 263 );
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler( this.tabControl1_SelectedIndexChanged );
            // 
            // texTab
            // 
            this.texTab.Controls.Add( this.listViewTex );
            this.texTab.Location = new System.Drawing.Point( 4, 21 );
            this.texTab.Name = "texTab";
            this.texTab.Padding = new System.Windows.Forms.Padding( 3 );
            this.texTab.Size = new System.Drawing.Size( 228, 238 );
            this.texTab.TabIndex = 0;
            this.texTab.Text = "贴图";
            this.texTab.UseVisualStyleBackColor = true;
            // 
            // listViewTex
            // 
            this.listViewTex.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewTex.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.texIndex,
            this.texName} );
            this.listViewTex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewTex.FullRowSelect = true;
            this.listViewTex.HideSelection = false;
            this.listViewTex.Location = new System.Drawing.Point( 3, 3 );
            this.listViewTex.MultiSelect = false;
            this.listViewTex.Name = "listViewTex";
            this.listViewTex.Size = new System.Drawing.Size( 222, 232 );
            this.listViewTex.TabIndex = 0;
            this.listViewTex.UseCompatibleStateImageBehavior = false;
            this.listViewTex.View = System.Windows.Forms.View.Details;
            this.listViewTex.MouseClick += new System.Windows.Forms.MouseEventHandler( this.listViewTex_MouseClick );
            // 
            // texIndex
            // 
            this.texIndex.Text = "索引";
            this.texIndex.Width = 49;
            // 
            // texName
            // 
            this.texName.Text = "文件名";
            this.texName.Width = 139;
            // 
            // visiPointTab
            // 
            this.visiPointTab.Controls.Add( this.listViewVisi );
            this.visiPointTab.Location = new System.Drawing.Point( 4, 21 );
            this.visiPointTab.Name = "visiPointTab";
            this.visiPointTab.Padding = new System.Windows.Forms.Padding( 3 );
            this.visiPointTab.Size = new System.Drawing.Size( 228, 238 );
            this.visiPointTab.TabIndex = 1;
            this.visiPointTab.Text = "可视关键点";
            this.visiPointTab.UseVisualStyleBackColor = true;
            // 
            // listViewVisi
            // 
            this.listViewVisi.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewVisi.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.visiIndex,
            this.visiPos} );
            this.listViewVisi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewVisi.FullRowSelect = true;
            this.listViewVisi.HideSelection = false;
            this.listViewVisi.Location = new System.Drawing.Point( 3, 3 );
            this.listViewVisi.MultiSelect = false;
            this.listViewVisi.Name = "listViewVisi";
            this.listViewVisi.Size = new System.Drawing.Size( 222, 232 );
            this.listViewVisi.TabIndex = 0;
            this.listViewVisi.UseCompatibleStateImageBehavior = false;
            this.listViewVisi.View = System.Windows.Forms.View.Details;
            this.listViewVisi.MouseClick += new System.Windows.Forms.MouseEventHandler( this.listViewVisi_MouseClick );
            // 
            // visiIndex
            // 
            this.visiIndex.Text = "索引";
            // 
            // visiPos
            // 
            this.visiPos.Text = "坐标";
            this.visiPos.Width = 147;
            // 
            // structPointTab
            // 
            this.structPointTab.Controls.Add( this.listViewStruct );
            this.structPointTab.Location = new System.Drawing.Point( 4, 21 );
            this.structPointTab.Name = "structPointTab";
            this.structPointTab.Padding = new System.Windows.Forms.Padding( 3 );
            this.structPointTab.Size = new System.Drawing.Size( 228, 238 );
            this.structPointTab.TabIndex = 2;
            this.structPointTab.Text = "结构关键点";
            this.structPointTab.UseVisualStyleBackColor = true;
            // 
            // listViewStruct
            // 
            this.listViewStruct.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewStruct.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.structIndex,
            this.structPos} );
            this.listViewStruct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewStruct.FullRowSelect = true;
            this.listViewStruct.HideSelection = false;
            this.listViewStruct.Location = new System.Drawing.Point( 3, 3 );
            this.listViewStruct.MultiSelect = false;
            this.listViewStruct.Name = "listViewStruct";
            this.listViewStruct.Size = new System.Drawing.Size( 222, 232 );
            this.listViewStruct.TabIndex = 0;
            this.listViewStruct.UseCompatibleStateImageBehavior = false;
            this.listViewStruct.View = System.Windows.Forms.View.Details;
            this.listViewStruct.MouseClick += new System.Windows.Forms.MouseEventHandler( this.listViewStruct_MouseClick );
            // 
            // structIndex
            // 
            this.structIndex.Text = "索引";
            // 
            // structPos
            // 
            this.structPos.Text = "坐标";
            this.structPos.Width = 145;
            // 
            // pictureBox
            // 
            this.pictureBox.AlphaMode = false;
            this.pictureBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject( "pictureBox.BackgroundImage" )));
            this.pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point( 0, 0 );
            this.pictureBox.MoveFactor = 1F;
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Scale = 0F;
            this.pictureBox.Size = new System.Drawing.Size( 522, 486 );
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.TexFocusPos = ((System.Drawing.PointF)(resources.GetObject( "pictureBox.TexFocusPos" )));
            this.pictureBox.ZoomFactor = 1F;
            this.pictureBox.ZoomWheelFactor = 1F;
            this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler( this.pictureBox_Paint );
            this.pictureBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler( this.pictureBox_MouseDoubleClick );
            this.pictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler( this.pictureBox_MouseClick );
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.Location = new System.Drawing.Point( 0, 3 );
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size( 26, 111 );
            this.toolStrip.TabIndex = 0;
            // 
            // openTexDialog
            // 
            this.openTexDialog.Filter = "\"png文件|*.png";
            this.openTexDialog.FileOk += new System.ComponentModel.CancelEventHandler( this.openTexDialog_FileOk );
            // 
            // GameObjEditer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 788, 532 );
            this.Controls.Add( this.toolStripContainer1 );
            this.Controls.Add( this.menuStrip );
            this.MainMenuStrip = this.menuStrip;
            this.Name = "GameObjEditer";
            this.Text = "GameObjEditer";
            this.menuStrip.ResumeLayout( false );
            this.menuStrip.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout( false );
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout( false );
            this.toolStripContainer1.RightToolStripPanel.ResumeLayout( false );
            this.toolStripContainer1.RightToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout( false );
            this.toolStripContainer1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout( false );
            this.splitContainer1.Panel2.ResumeLayout( false );
            this.splitContainer1.ResumeLayout( false );
            this.splitContainer2.Panel1.ResumeLayout( false );
            this.splitContainer2.Panel2.ResumeLayout( false );
            this.splitContainer2.ResumeLayout( false );
            this.tabControl1.ResumeLayout( false );
            this.texTab.ResumeLayout( false );
            this.visiPointTab.ResumeLayout( false );
            this.structPointTab.ResumeLayout( false );
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem texToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem borderToolStripMenuItem;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage texTab;
        private System.Windows.Forms.TabPage visiPointTab;
        private System.Windows.Forms.TabPage structPointTab;
        private System.Windows.Forms.ListView listViewTex;
        private System.Windows.Forms.ColumnHeader texIndex;
        private System.Windows.Forms.ColumnHeader texName;
        private System.Windows.Forms.ListView listViewVisi;
        private System.Windows.Forms.ColumnHeader visiIndex;
        private System.Windows.Forms.ColumnHeader visiPos;
        private System.Windows.Forms.ListView listViewStruct;
        private System.Windows.Forms.ColumnHeader structIndex;
        private System.Windows.Forms.ColumnHeader structPos;
        private System.Windows.Forms.OpenFileDialog openTexDialog;
        private PictureBoxGird.PictureBoxGird pictureBox;
    }
}