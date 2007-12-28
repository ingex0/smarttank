using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using SmartTank.GameObjects;
using TankEngine2D.Graphics;
using SmartTank.Helpers;

namespace GameObjEditer
{
    public partial class GameObjEditer : Form
    {
        #region TypeDef

        class TreeNodeObj : TreeNode
        {
            public GameObjData obj;

            public GameObjData GameObj
            {
                get { return obj; }
            }

            public TreeNodeObj ( string objName )
                : base( objName )
            {
                obj = new GameObjData( objName );
                TreeNodeNode baseNode = new TreeNodeNode( "Base" );
                TreeNodeNode.AddToChilds( this, baseNode );
                obj.creater = "your name";
                obj.year = DateTime.Today.Year;
                obj.month = DateTime.Today.Month;
                obj.day = DateTime.Today.Day;
            }

            public TreeNodeObj ( GameObjData data, string path )
                : base( data.name )
            {
                obj = data;
                TreeNodeNode baseNode = new TreeNodeNode( data.baseNode, path );
                TreeNodeNode.AddToChilds( this, baseNode, false );
            }

            public void Rename ( string newName )
            {
                if (newName == string.Empty)
                    return;

                Text = newName;
                obj.name = newName;
            }

            public TreeNodeNode BaseNode
            {
                get
                {
                    if (Nodes.Count == 1)
                        return (TreeNodeNode)this.Nodes[0];
                    else
                        return null;
                }
            }
        }

        class TreeNodeNode : TreeNode, IEnumerable<TreeNodeNode>
        {
            static public void AddToChilds ( TreeNode parent, TreeNodeNode child )
            {
                AddToChilds( parent, child, true );
            }

            static public void AddToChilds ( TreeNode parent, TreeNodeNode child, bool addDataNodeChild )
            {
                if (parent is TreeNodeObj)
                {
                    if (((TreeNodeObj)parent).BaseNode == null)
                    {
                        if (addDataNodeChild)
                            ((TreeNodeObj)parent).GameObj.baseNode = child.dateNode;
                        parent.Nodes.Add( child );
                    }
                }
                else if (parent is TreeNodeNode)
                {
                    if (addDataNodeChild)
                    {
                        ((TreeNodeNode)parent).dateNode.childNodes.Add( child.dateNode );
                        child.dateNode.parent = ((TreeNodeNode)parent).dateNode;
                    }
                    parent.Nodes.Add( child );
                }
            }

            static public void RemoveChild ( TreeNodeNode parent, TreeNodeNode child )
            {
                parent.Nodes.Remove( child );
                parent.dateNode.childNodes.Remove( child.dateNode );
            }

            GameObjDataNode dateNode;

            List<string> texNames;

            List<Texture2D> textures;
            List<Bitmap> bitmaps;

            int curTexIndex = -1;
            int curVisiPointIndex = -1;
            int curStructPointIndex = -1;

            public TreeNodeNode ( string text )
                : base( text )
            {
                textures = new List<Texture2D>();
                bitmaps = new List<Bitmap>();
                texNames = new List<string>();
                borderMaps = new List<SpriteBorder.BorderMap>();
                dateNode = new GameObjDataNode( text );
            }

            public TreeNodeNode ( GameObjDataNode dateNode, string path )
                : this( dateNode.nodeName )
            {
                this.dateNode = dateNode;

                foreach (string texName in dateNode.texPaths)
                {
                    AddTex( Path.Combine( path, texName ), GameObjEditer.device, false );
                }

                foreach (GameObjDataNode child in dateNode.childNodes)
                {
                    TreeNodeNode childNode = new TreeNodeNode( child, path );
                    AddToChilds( this, childNode, false );
                }

            }

            public void Rename ( string newName )
            {
                if (newName == string.Empty)
                    return;

                Text = newName;
                dateNode.nodeName = newName;
            }

            public List<string> TexNames
            {
                get
                {
                    return texNames;
                }
            }

            public List<Microsoft.Xna.Framework.Vector2> VisiPoints
            {
                get { return dateNode.visiKeyPoints; }
            }

            public List<Microsoft.Xna.Framework.Vector2> StructPoints
            {
                get { return dateNode.structKeyPoints; }
            }

            public List<Texture2D> Textures
            {
                get { return textures; }
            }

            public int CurTexIndex
            {
                get { return curTexIndex; }
                set { curTexIndex = value; }
            }

            public int CurVisiPointIndex
            {
                get { return curVisiPointIndex; }
                set { curVisiPointIndex = value; }
            }

            public int CurStructPointIndex
            {
                get { return curStructPointIndex; }
                set { curStructPointIndex = value; }
            }

            public Texture2D CurXNATex
            {
                get
                {
                    if (curTexIndex >= 0 && curTexIndex < bitmaps.Count)
                        return textures[curTexIndex];
                    else
                        return null;
                }
            }

            public Bitmap CurBitMap
            {
                get
                {
                    if (curTexIndex >= 0 && curTexIndex < bitmaps.Count)
                        return bitmaps[curTexIndex];
                    else
                        return null;
                }
            }

            public SpriteBorder.BorderMap CurBorderMap
            {
                get
                {
                    if (curTexIndex >= 0 && curTexIndex < bitmaps.Count)
                        return borderMaps[curTexIndex];
                    else
                        return null;
                }
            }

            public TreeNodeObj TreeNodeObj
            {
                get
                {
                    TreeNode obj = this;
                    while (obj.Parent != null)
                    {
                        obj = obj.Parent;
                        if (obj is TreeNodeObj)
                            return (TreeNodeObj)obj;
                    }
                    return null;
                }
            }

            public List<SpriteBorder.BorderMap> borderMaps;

            public void AddTex ( string filePath, GraphicsDevice device, bool addDataNodeTexPath )
            {
                textures.Add( Texture2D.FromFile( device, filePath ) );
                bitmaps.Add( new Bitmap( filePath ) );
                texNames.Add( Path.GetFileName( filePath ) );
                borderMaps.Add( null );
                if (addDataNodeTexPath)
                    dateNode.texPaths.Add( Path.GetFileName( filePath ) );
                curTexIndex = textures.Count - 1;
            }

            public void DelTex ( int index )
            {
                if (index < 0 || index >= textures.Count)
                    return;

                textures.RemoveAt( index );
                bitmaps.RemoveAt( index );
                texNames.RemoveAt( index );
                borderMaps.RemoveAt( index );
                dateNode.texPaths.RemoveAt( index );
                curTexIndex--;
            }

            public void SetBorderMap ( SpriteBorder.BorderMap borderMap )
            {
                borderMaps[curTexIndex] = borderMap;
            }

            public void UpTex ( int index )
            {
                if (index <= 0 || index >= textures.Count)
                    return;

                Texture2D tempTex = textures[index];
                textures[index] = textures[index - 1];
                textures[index - 1] = tempTex;

                Bitmap tempBitmap = bitmaps[index];
                bitmaps[index] = bitmaps[index - 1];
                bitmaps[index - 1] = tempBitmap;

                string tempName = texNames[index];
                texNames[index] = texNames[index - 1];
                texNames[index - 1] = tempName;

                string tempPath = dateNode.texPaths[index];
                dateNode.texPaths[index] = dateNode.texPaths[index - 1];
                dateNode.texPaths[index - 1] = tempPath;

                curTexIndex--;
            }

            public void downTex ( int index )
            {
                if (index < 0 || index >= textures.Count - 1)
                    return;

                Texture2D tempTex = textures[index];
                textures[index] = textures[index + 1];
                textures[index + 1] = tempTex;

                Bitmap tempBitmap = bitmaps[index];
                bitmaps[index] = bitmaps[index + 1];
                bitmaps[index + 1] = tempBitmap;

                string tempName = texNames[index];
                texNames[index] = texNames[index + 1];
                texNames[index + 1] = tempName;

                string tempPath = dateNode.texPaths[index];
                dateNode.texPaths[index] = dateNode.texPaths[index + 1];
                dateNode.texPaths[index + 1] = tempPath;

                curTexIndex++;
            }

            public void AddVisiPoint ( float x, float y )
            {
                dateNode.visiKeyPoints.Add( new Microsoft.Xna.Framework.Vector2( x, y ) );

                curVisiPointIndex = dateNode.visiKeyPoints.Count - 1;
            }

            public void DelVisiPoint ()
            {
                if (curVisiPointIndex < 0 || curVisiPointIndex >= dateNode.visiKeyPoints.Count)
                    return;

                dateNode.visiKeyPoints.RemoveAt( curVisiPointIndex );

                curVisiPointIndex--;
            }

            public void UpVisiPoint ()
            {
                if (curVisiPointIndex <= 0 || curVisiPointIndex >= dateNode.visiKeyPoints.Count)
                    return;

                Microsoft.Xna.Framework.Vector2 temp = dateNode.visiKeyPoints[curVisiPointIndex];
                dateNode.visiKeyPoints[curVisiPointIndex] = dateNode.visiKeyPoints[curVisiPointIndex - 1];
                dateNode.visiKeyPoints[curVisiPointIndex - 1] = temp;

                curVisiPointIndex--;
            }

            public void DownVisiPoint ()
            {
                if (curVisiPointIndex < 0 || curVisiPointIndex >= dateNode.visiKeyPoints.Count - 1)
                    return;

                Microsoft.Xna.Framework.Vector2 temp = dateNode.visiKeyPoints[curVisiPointIndex];
                dateNode.visiKeyPoints[curVisiPointIndex] = dateNode.visiKeyPoints[curVisiPointIndex + 1];
                dateNode.visiKeyPoints[curVisiPointIndex + 1] = temp;

                curVisiPointIndex++;
            }

            public void AddStructPoint ( float x, float y )
            {
                dateNode.structKeyPoints.Add( new Microsoft.Xna.Framework.Vector2( x, y ) );

                curStructPointIndex = dateNode.structKeyPoints.Count - 1;
            }

            public void DelStructiPoint ()
            {
                if (curStructPointIndex < 0 || curStructPointIndex >= dateNode.structKeyPoints.Count)
                    return;

                dateNode.structKeyPoints.RemoveAt( curStructPointIndex );

                curStructPointIndex--;
            }

            public void UpStructPoint ()
            {
                if (curStructPointIndex <= 0 || curStructPointIndex >= dateNode.structKeyPoints.Count)
                    return;

                Microsoft.Xna.Framework.Vector2 temp = dateNode.structKeyPoints[curStructPointIndex];
                dateNode.structKeyPoints[curStructPointIndex] = dateNode.structKeyPoints[curStructPointIndex - 1];
                dateNode.structKeyPoints[curStructPointIndex - 1] = temp;

                curStructPointIndex--;
            }

            public void DownStructPoint ()
            {
                if (curStructPointIndex < 0 || curStructPointIndex >= dateNode.structKeyPoints.Count - 1)
                    return;

                Microsoft.Xna.Framework.Vector2 temp = dateNode.structKeyPoints[curStructPointIndex];
                dateNode.structKeyPoints[curStructPointIndex] = dateNode.structKeyPoints[curStructPointIndex + 1];
                dateNode.structKeyPoints[curStructPointIndex + 1] = temp;

                curStructPointIndex++;
            }

            public List<int> IntDatas
            {
                get { return dateNode.intDatas; }
            }

            public List<float> FloatDatas
            {
                get { return dateNode.floatDatas; }
            }

            public void AddIntData ( int value )
            {
                dateNode.intDatas.Add( value );
            }

            public void AddFloatData ( float value )
            {
                dateNode.floatDatas.Add( value );
            }

            public void DelIntData ( int index )
            {
                if (dateNode.intDatas.Count > index)
                {
                    dateNode.intDatas.RemoveAt( index );
                }
            }

            public void DelFloatData ( int index )
            {
                if (dateNode.floatDatas.Count > index)
                {
                    dateNode.floatDatas.RemoveAt( index );
                }
            }

            #region IEnumerable<TreeNodeEnum> 成员

            public IEnumerator<TreeNodeNode> GetEnumerator ()
            {
                yield return this;

                foreach (TreeNode childNode in Nodes)
                {
                    if (!(childNode is TreeNodeNode))
                        throw new Exception( "childNode isn't a TreeNodeEnum." );

                    foreach (TreeNodeNode downNode in (TreeNodeNode)childNode)
                    {
                        yield return downNode;
                    }
                }
            }

            #endregion

            #region IEnumerable 成员

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
            {
                return GetEnumerator();
            }

            #endregion
        }

        #endregion

        #region Variables

        ContextMenuStrip treeMenuStrip;
        ContextMenuStrip texListMenuStrip;
        ContextMenuStrip pictureMenuStrip;
        ContextMenuStrip pointMenuStrip;
        ContextMenuStrip dataMenuStrip;


        TreeNode CurTreeNode
        {
            get { return treeView.SelectedNode; }
            set { treeView.SelectedNode = value; }
        }

        TreeNodeNode CurNodeNode
        {
            get
            {
                if (CurTreeNode != null && CurTreeNode is TreeNodeNode)
                {
                    return (TreeNodeNode)CurTreeNode;
                }
                else
                    return null;
            }
        }

        TreeNodeObj CurTreeObj
        {
            get
            {
                if (CurTreeNode != null)
                {
                    if (CurTreeNode is TreeNodeObj)
                    {
                        return (TreeNodeObj)CurTreeNode;
                    }
                    else
                    {
                        return ((TreeNodeNode)CurTreeNode).TreeNodeObj;
                    }
                }
                else
                    return null;
            }
        }

        static GraphicsDevice device;

        int CurTexIndex
        {
            get
            {
                if (CurTreeNode != null && CurTreeNode is TreeNodeNode)
                    return ((TreeNodeNode)CurTreeNode).CurTexIndex;
                else
                    return -1;
            }
            set
            {
                if (CurTreeNode != null && CurTreeNode is TreeNodeNode)
                    ((TreeNodeNode)CurTreeNode).CurTexIndex = value;
            }
        }

        Texture2D CurXNATex
        {
            get
            {
                if (CurTreeNode != null && CurTreeNode is TreeNodeNode)
                {
                    return ((TreeNodeNode)CurTreeNode).CurXNATex;
                }
                else
                    return null;
            }
        }

        Bitmap CurBitMap
        {
            get
            {
                if (CurTreeNode != null && CurTreeNode is TreeNodeNode)
                    return ((TreeNodeNode)CurTreeNode).CurBitMap;
                else
                    return null;
            }
        }

        SpriteBorder.BorderMap CurBorderMap
        {
            get
            {
                if (CurXNATex != null)
                    return ((TreeNodeNode)CurTreeNode).CurBorderMap;
                else
                    return null;
            }
        }

        enum TabState
        {
            Texture,
            VisiPoint,
            StructPoint,
            IntData,
            FloatData,
        }

        TabState CurTabState
        {
            get
            {
                if (tabControl1.SelectedTab == texTab)
                {
                    return TabState.Texture;
                }
                else if (tabControl1.SelectedTab == visiPointTab)
                {
                    return TabState.VisiPoint;
                }
                else if (tabControl1.SelectedTab == structPointTab)
                {
                    return TabState.StructPoint;
                }
                else if (tabControl1.SelectedTab == intDataTab)
                {
                    return TabState.IntData;
                }
                else
                {
                    return TabState.FloatData;
                }
            }
        }


        Bitmap visiPointMap;
        Bitmap structPointMap;

        #endregion

        public GameObjEditer ()
        {
            visiPointMap = new Bitmap( "Content\\pointBlue.png" );
            structPointMap = new Bitmap( "Content\\pointRed.png" );

            InitializeComponent();
            InitialTreeViewContentMenu();
            InitialTexContentMenu();
            InitialGraphicsDevice();
            InitialTexContentMenu();
            InitialPictureBoxContentMenu();
            InitialPointContentMenu();

            InitialDataListContentMenu();

            pictureBox.LastPaint += new PaintEventHandler( pictureBox_LastPaint );

        }

        private void InitialGraphicsDevice ()
        {
            PresentationParameters pp = new PresentationParameters();
            pp.BackBufferCount = 1;
            pp.IsFullScreen = false;
            pp.SwapEffect = SwapEffect.Discard;
            pp.BackBufferWidth = this.Width;
            pp.BackBufferHeight = this.Height;

            pp.AutoDepthStencilFormat = DepthFormat.Depth24Stencil8;
            pp.EnableAutoDepthStencil = true;
            pp.PresentationInterval = PresentInterval.Default;
            pp.BackBufferFormat = SurfaceFormat.Unknown;
            pp.MultiSampleType = MultiSampleType.None;

            device = new GraphicsDevice( GraphicsAdapter.DefaultAdapter,
                DeviceType.Hardware, this.Handle,
                CreateOptions.HardwareVertexProcessing,
                pp );
        }

        private void UpdateComponent ()
        {
            UpdateTreeContentMenu();
            UpdateTexPathList();
            UpdateTexListContentMenu();
            UpdateMenu();
            UpdatePictureBox();
            UpdatePictureMenuStrip();
            UpdateVisiList();
            UpdateStructList();
            UpdatePointContentMenu();
            UpdateStatusBar();
            UpdateDataList();
        }

        private void tabControl1_SelectedIndexChanged ( object sender, EventArgs e )
        {
            UpdateComponent();
        }

        #region TreeViewContentMenu

        void InitialTreeViewContentMenu ()
        {
            treeMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem addNewObjLabel = new ToolStripMenuItem();
            addNewObjLabel.Name = "添加新物体";
            addNewObjLabel.Text = "添加新物体";
            ToolStripMenuItem delObjLabel = new ToolStripMenuItem();
            delObjLabel.Name = "删除物体";
            delObjLabel.Text = "删除物体";
            ToolStripMenuItem addChildLabel = new ToolStripMenuItem();
            addChildLabel.Name = "添加子节点";
            addChildLabel.Text = "添加子节点";
            ToolStripMenuItem delNodeLabel = new ToolStripMenuItem();
            delNodeLabel.Name = "删除节点";
            delNodeLabel.Text = "删除节点";
            ToolStripMenuItem renameLabel = new ToolStripMenuItem();
            renameLabel.Name = "重命名";
            renameLabel.Text = "重命名";

            treeMenuStrip.Items.AddRange( new ToolStripMenuItem[] { addNewObjLabel, delObjLabel, addChildLabel, delNodeLabel, renameLabel } );

            treeView.ContextMenuStrip = treeMenuStrip;

            addNewObjLabel.Click += new EventHandler( addNewObjLabel_Click );
            delObjLabel.Click += new EventHandler( delObjLabel_Click );
            addChildLabel.Click += new EventHandler( addChildLabel_Click );
            delNodeLabel.Click += new EventHandler( delNodeLabel_Click );
            renameLabel.Click += new EventHandler( renameLabel_Click );

            AddNewObj();

        }

        void addNewObjLabel_Click ( object sender, EventArgs e )
        {
            AddNewObj();
            UpdateComponent();
        }

        void delObjLabel_Click ( object sender, EventArgs e )
        {
            DialogResult result = MessageBox.Show( "这将删除当前正在编辑的场景物体，是否继续？", "确定删除", MessageBoxButtons.YesNo );
            if (result == DialogResult.No)
                return;
            DelObj();
            UpdateComponent();
        }

        void addChildLabel_Click ( object sender, EventArgs e )
        {
            AddNode();
            UpdateComponent();
        }

        void delNodeLabel_Click ( object sender, EventArgs e )
        {
            DialogResult result = MessageBox.Show( "这将删除当前的节点，是否继续？", "确定删除", MessageBoxButtons.YesNo );
            if (result == DialogResult.No)
                return;
            DelNode();
            UpdateComponent();
        }

        void renameLabel_Click ( object sender, EventArgs e )
        {
            Rename rename = new Rename();
            rename.NameText = CurTreeNode.Text;
            rename.ShowDialog();

            if (CurTreeNode is TreeNodeObj)
            {
                ((TreeNodeObj)CurTreeNode).Rename( rename.NameText );
            }
            else if (CurTreeNode is TreeNodeNode)
            {
                ((TreeNodeNode)CurTreeNode).Rename( rename.NameText );
            }
            UpdateComponent();
        }

        private void treeView_AfterSelect ( object sender, TreeViewEventArgs e )
        {
            UpdateComponent();
        }

        private void treeView_MouseClick ( object sender, MouseEventArgs e )
        {
            TreeNode node = treeView.GetNodeAt( e.X, e.Y );
            CurTreeNode = node;
            UpdateComponent();
        }


        private void UpdateTreeContentMenu ()
        {
            treeMenuStrip.Items["删除物体"].Enabled = true;
            treeMenuStrip.Items["添加子节点"].Enabled = true;
            treeMenuStrip.Items["删除节点"].Enabled = true;
            treeMenuStrip.Items["重命名"].Enabled = true;

            if (CurTreeNode == null)
            {
                treeMenuStrip.Items["删除物体"].Enabled = false;
                treeMenuStrip.Items["添加子节点"].Enabled = false;
                treeMenuStrip.Items["删除节点"].Enabled = false;
                treeMenuStrip.Items["重命名"].Enabled = false;
            }
            else if (CurTreeNode is TreeNodeObj)
            {
                treeMenuStrip.Items["添加子节点"].Enabled = false;
                treeMenuStrip.Items["删除节点"].Enabled = false;
            }
            else if (CurTreeNode is TreeNodeNode && CurTreeNode.Parent is TreeNodeObj)
            {
                treeMenuStrip.Items["删除节点"].Enabled = false;
            }
        }

        void AddNewObj ()
        {
            TreeNodeObj newObjNode = new TreeNodeObj( "NewGameObj" );
            treeView.Nodes.Add( newObjNode );
            newObjNode.Expand();
            CurTreeNode = newObjNode.Nodes[0];
        }

        void DelObj ()
        {
            if (CurTreeNode is TreeNodeObj)
            {
                treeView.Nodes.Remove( CurTreeNode );
            }
            else if (CurTreeNode is TreeNodeNode)
            {
                treeView.Nodes.Remove( ((TreeNodeNode)CurTreeNode).TreeNodeObj );
            }
        }

        void AddNode ()
        {
            if (CurTreeNode == null)
                return;

            TreeNodeNode newTreeNode = new TreeNodeNode( "newNode" );
            TreeNodeNode.AddToChilds( CurTreeNode, newTreeNode );
            CurTreeNode = newTreeNode;
        }

        void DelNode ()
        {
            if (CurTreeNode == null)
                return;

            if (CurTreeNode is TreeNodeNode && CurTreeNode.Parent is TreeNodeNode)
            {
                TreeNodeNode.RemoveChild( (TreeNodeNode)CurTreeNode.Parent, (TreeNodeNode)CurTreeNode );
            }
        }

        #endregion

        #region TexListContentMenu

        void InitialTexContentMenu ()
        {
            texListMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem importTexLabel = new ToolStripMenuItem();
            importTexLabel.Name = "导入贴图";
            importTexLabel.Text = "导入贴图";
            ToolStripMenuItem delTexLabel = new ToolStripMenuItem();
            delTexLabel.Name = "移除贴图";
            delTexLabel.Text = "移除贴图";
            ToolStripMenuItem upTexLabel = new ToolStripMenuItem();
            upTexLabel.Name = "上移";
            upTexLabel.Text = "上移";
            ToolStripMenuItem downTexLabel = new ToolStripMenuItem();
            downTexLabel.Name = "下移";
            downTexLabel.Text = "下移";
            ToolStripMenuItem checkBorderLabel = new ToolStripMenuItem();
            checkBorderLabel.Name = "提取边界";
            checkBorderLabel.Text = "提取边界";

            texListMenuStrip.Items.AddRange( new ToolStripItem[] { importTexLabel, delTexLabel, upTexLabel, downTexLabel, checkBorderLabel } );

            importTexLabel.Click += new EventHandler( importTexLabel_Click );
            delTexLabel.Click += new EventHandler( delTexLabel_Click );
            upTexLabel.Click += new EventHandler( upTexLabel_Click );
            downTexLabel.Click += new EventHandler( downTexLabel_Click );
            checkBorderLabel.Click += new EventHandler( checkBorderLabel_Click );

            listViewTex.ContextMenuStrip = texListMenuStrip;
        }

        void importTexLabel_Click ( object sender, EventArgs e )
        {
            LoadTex();
            UpdateComponent();
            pictureBox.Invalidate();
        }

        void delTexLabel_Click ( object sender, EventArgs e )
        {
            DelTex();
            UpdateComponent();
            pictureBox.Invalidate();
        }

        void upTexLabel_Click ( object sender, EventArgs e )
        {
            UpTex();
            UpdateComponent();
            pictureBox.Invalidate();
        }

        void downTexLabel_Click ( object sender, EventArgs e )
        {
            DownTex();
            UpdateComponent();
            pictureBox.Invalidate();
        }

        void checkBorderLabel_Click ( object sender, EventArgs e )
        {
            if (CheckBorder())
                MessageBox.Show( "生成边界成功！" );
            else
                MessageBox.Show( "生成边界失败！" );
            pictureBox.Invalidate();
        }

        void UpdateTexPathList ()
        {
            listViewTex.Items.Clear();

            if (CurTreeNode == null || CurTreeNode is TreeNodeObj)
                return;

            int i = 0;
            foreach (string texName in ((TreeNodeNode)CurTreeNode).TexNames)
            {
                ListViewItem item = listViewTex.Items.Add( i.ToString() );
                item.SubItems.Add( texName );
                i++;
            }

            if (CurTexIndex != -1 && CurTexIndex < listViewTex.Items.Count)
                listViewTex.Items[CurTexIndex].Selected = true;
        }

        private void listViewTex_MouseClick ( object sender, MouseEventArgs e )
        {
            CurTexIndex = listViewTex.GetItemAt( e.X, e.Y ).Index;
            UpdateComponent();
        }

        void UpdateTexListContentMenu ()
        {
            texListMenuStrip.Items["移除贴图"].Enabled = true;
            texListMenuStrip.Items["上移"].Enabled = true;
            texListMenuStrip.Items["下移"].Enabled = true;
            texListMenuStrip.Items["提取边界"].Enabled = true;


            if (CurTexIndex == -1)
            {
                texListMenuStrip.Items["移除贴图"].Enabled = false;
                texListMenuStrip.Items["上移"].Enabled = false;
                texListMenuStrip.Items["下移"].Enabled = false;
                texListMenuStrip.Items["提取边界"].Enabled = false;
            }
            if (CurTexIndex == 0)
            {
                texListMenuStrip.Items["上移"].Enabled = false;
            }
            if (CurTexIndex == listViewTex.Items.Count - 1)
            {
                texListMenuStrip.Items["下移"].Enabled = false;
            }
        }

        void LoadTex ()
        {
            if (CurTreeNode != null && CurTreeNode is TreeNodeNode)
                openTexDialog.ShowDialog();
        }

        private void openTexDialog_FileOk ( object sender, CancelEventArgs e )
        {
            string texPath = openTexDialog.FileName;
            ((TreeNodeNode)CurTreeNode).AddTex( texPath, device, true );
            UpdateComponent();
        }

        void DelTex ()
        {
            if (CurTexIndex != -1 && CurTreeNode is TreeNodeNode)
            {
                ((TreeNodeNode)CurTreeNode).DelTex( CurTexIndex );
            }
        }

        void UpTex ()
        {
            if (CurTexIndex > 0 && CurTreeNode is TreeNodeNode)
            {
                ((TreeNodeNode)CurTreeNode).UpTex( CurTexIndex );
            }
        }

        void DownTex ()
        {
            if (CurTexIndex >= 0 && CurTreeNode is TreeNodeNode)
            {
                ((TreeNodeNode)CurTreeNode).downTex( CurTexIndex );
            }
        }

        #endregion

        #region CheckBorder

        Point errorPoint;
        bool showError = false;

        bool CheckBorder ()
        {
            if (CurXNATex != null)
            {
                bool success = true; ;
                try
                {
                    SpriteBorder.BorderMap borderMap;
                    Sprite.CheckBorder( CurXNATex, out borderMap );
                    ((TreeNodeNode)CurTreeNode).SetBorderMap( borderMap );
                }
                catch (BorderBulidException e)
                {
                    ((TreeNodeNode)CurTreeNode).SetBorderMap( e.borderMap );
                    success = false;
                    errorPoint = new Point( e.curPoint.X, e.curPoint.Y );
                    pictureBox.TexFocusPos = errorPoint;
                    pictureBox.Invalidate();
                }
                showError = !success;

                return success;
            }
            return false;
        }

        #endregion

        #region PointContentMenu

        void InitialPointContentMenu ()
        {
            pointMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem delPoint = new ToolStripMenuItem();
            delPoint.Name = "删除关键点";
            delPoint.Text = "删除关键点";
            ToolStripMenuItem upPoint = new ToolStripMenuItem();
            upPoint.Name = "上升";
            upPoint.Text = "上升";
            ToolStripMenuItem downPoint = new ToolStripMenuItem();
            downPoint.Name = "下降";
            downPoint.Text = "下降";

            delPoint.Click += new EventHandler( delPoint_Click );
            upPoint.Click += new EventHandler( upPoint_Click );
            downPoint.Click += new EventHandler( downPoint_Click );

            pointMenuStrip.Items.AddRange( new ToolStripItem[] { delPoint, upPoint, downPoint } );

            listViewVisi.ContextMenuStrip = pointMenuStrip;
            listViewStruct.ContextMenuStrip = pointMenuStrip;
        }

        void UpdatePointContentMenu ()
        {
            pointMenuStrip.Items["删除关键点"].Enabled = true;
            pointMenuStrip.Items["上升"].Enabled = true;
            pointMenuStrip.Items["下降"].Enabled = true;

            if (CurTreeNode == null || CurTreeNode is TreeNodeObj)
            {
                pointMenuStrip.Items["删除关键点"].Enabled = false;
                pointMenuStrip.Items["上升"].Enabled = false;
                pointMenuStrip.Items["下降"].Enabled = false;
            }
            else
            {
                if (CurTabState == TabState.VisiPoint)
                {
                    int index = ((TreeNodeNode)CurTreeNode).CurVisiPointIndex;
                    int count = ((TreeNodeNode)CurTreeNode).VisiPoints.Count;
                    if (index < 0 || index >= count)
                    {
                        pointMenuStrip.Items["删除关键点"].Enabled = false;
                        pointMenuStrip.Items["上升"].Enabled = false;
                        pointMenuStrip.Items["下降"].Enabled = false;
                    }

                    if (index <= 0)
                    {
                        pointMenuStrip.Items["上升"].Enabled = false;
                    }
                    if (index >= count - 1)
                    {
                        pointMenuStrip.Items["下降"].Enabled = false;
                    }

                }
                else if (CurTabState == TabState.StructPoint)
                {
                    int index = ((TreeNodeNode)CurTreeNode).CurStructPointIndex;
                    int count = ((TreeNodeNode)CurTreeNode).StructPoints.Count;

                    if (index < 0 || index >= count)
                    {
                        pointMenuStrip.Items["删除关键点"].Enabled = false;
                        pointMenuStrip.Items["上升"].Enabled = false;
                        pointMenuStrip.Items["下降"].Enabled = false;
                    }

                    if (index <= 0)
                    {
                        pointMenuStrip.Items["上升"].Enabled = false;
                    }
                    if (index >= count - 1)
                    {
                        pointMenuStrip.Items["下降"].Enabled = false;
                    }
                }
            }
        }

        void delPoint_Click ( object sender, EventArgs e )
        {
            if (CurTreeNode == null)
                return;

            if (CurTabState == TabState.VisiPoint)
            {
                ((TreeNodeNode)CurTreeNode).DelVisiPoint();
            }
            else if (CurTabState == TabState.StructPoint)
            {
                ((TreeNodeNode)CurTreeNode).DelStructiPoint();
            }
            UpdateComponent();
        }

        void upPoint_Click ( object sender, EventArgs e )
        {
            if (CurTreeNode == null)
                return;

            if (CurTabState == TabState.VisiPoint)
            {
                ((TreeNodeNode)CurTreeNode).UpVisiPoint();
            }
            else if (CurTabState == TabState.StructPoint)
            {
                ((TreeNodeNode)CurTreeNode).UpStructPoint();
            }
            UpdateComponent();
        }

        void downPoint_Click ( object sender, EventArgs e )
        {
            if (CurTreeNode == null)
                return;

            if (CurTabState == TabState.VisiPoint)
            {
                ((TreeNodeNode)CurTreeNode).DownVisiPoint();
            }
            else if (CurTabState == TabState.StructPoint)
            {
                ((TreeNodeNode)CurTreeNode).DownStructPoint();
            }
            UpdateComponent();
        }

        void UpdateVisiList ()
        {
            if (CurTreeNode != null && CurTreeNode is TreeNodeNode)
            {
                listViewVisi.Items.Clear();
                int i = 0;
                foreach (Microsoft.Xna.Framework.Vector2 pos in ((TreeNodeNode)CurTreeNode).VisiPoints)
                {
                    ListViewItem item = new ListViewItem( i.ToString() );
                    listViewVisi.Items.Add( item );
                    item.SubItems.Add( "( " + pos.X + ", " + pos.Y + " )" );
                    i++;
                }
                int selectIndex = ((TreeNodeNode)CurTreeNode).CurVisiPointIndex;
                if (selectIndex >= 0 && selectIndex < i)
                    listViewVisi.Items[selectIndex].Selected = true;
            }
            else
            {
                listViewVisi.Items.Clear();
            }
        }

        void UpdateStructList ()
        {
            if (CurTreeNode != null && CurTreeNode is TreeNodeNode)
            {
                listViewStruct.Items.Clear();
                int i = 0;
                foreach (Microsoft.Xna.Framework.Vector2 pos in ((TreeNodeNode)CurTreeNode).StructPoints)
                {
                    ListViewItem item = new ListViewItem( i.ToString() );
                    listViewStruct.Items.Add( item );
                    item.SubItems.Add( "( " + pos.X + ", " + pos.Y + " )" );
                    i++;
                }

                int selectIndex = ((TreeNodeNode)CurTreeNode).CurStructPointIndex;
                if (selectIndex >= 0 && selectIndex < i)
                    listViewStruct.Items[selectIndex].Selected = true;
            }
            else
            {
                listViewStruct.Items.Clear();
            }
        }

        private void listViewVisi_MouseClick ( object sender, MouseEventArgs e )
        {
            ((TreeNodeNode)CurTreeNode).CurVisiPointIndex = listViewVisi.GetItemAt( e.X, e.Y ).Index;
            UpdateComponent();
        }

        private void listViewStruct_MouseClick ( object sender, MouseEventArgs e )
        {
            ((TreeNodeNode)CurTreeNode).CurStructPointIndex = listViewStruct.GetItemAt( e.X, e.Y ).Index;
            UpdateComponent();
        }

        #endregion

        #region Menu

        private void UpdateMenu ()
        {
            menuStrip.Items["texToolStripMenuItem"].Enabled = true;
            saveToolStripMenuItem.Enabled = true;

            if (CurTreeNode == null || !(CurTreeNode is TreeNodeNode))
            {
                menuStrip.Items["texToolStripMenuItem"].Enabled = false;
            }

            if (CurTreeNode == null)
            {
                saveToolStripMenuItem.Enabled = false;
            }
        }

        private void ImportToolStripMenuItem_Click ( object sender, EventArgs e )
        {
            LoadTex();
            pictureBox.Invalidate();
        }

        private void ExitToolStripMenuItem_Click ( object sender, EventArgs e )
        {

        }

        private void SaveToolStripMenuItem_Click ( object sender, EventArgs e )
        {
            Save();
        }

        private void OpenToolStripMenuItem_Click ( object sender, EventArgs e )
        {
            Open();
        }

        private void BorderToolStripMenuItem_Click ( object sender, EventArgs e )
        {

        }

        void Save ()
        {
            TreeNodeObj curObj;
            if (CurTreeNode is TreeNodeObj)
            {
                curObj = (TreeNodeObj)CurTreeNode;
            }
            else
            {
                curObj = ((TreeNodeNode)CurTreeNode).TreeNodeObj;
            }

            if (curObj.BaseNode == null)
                return;

            EnterYourName enterName = new EnterYourName( "保存" + curObj.obj.name + "场景物体中，请输入您的名字以便储存。" );

            enterName.ShowDialog();
            if (enterName.SelectYes)
            {
                curObj.obj.creater = enterName.CreatorName;



                string savePath = Path.Combine( Directories.GameObjsDirectory, curObj.obj.creater );

                if (!Directory.Exists( savePath ))
                {
                    Directory.CreateDirectory( savePath );
                }

                savePath = Path.Combine( savePath, curObj.obj.name );

                if (!Directory.Exists( savePath ))
                {
                    Directory.CreateDirectory( savePath );
                }

                foreach (TreeNodeNode node in curObj.BaseNode)
                {
                    int i = 0;
                    foreach (Texture2D texture in node.Textures)
                    {
                        string saveFile = Path.Combine( savePath, node.TexNames[i] );
                        if (File.Exists( saveFile ))
                        {
                            DialogResult result = MessageBox.Show( "贴图文件已存在，是否覆盖？", "文件已存在", MessageBoxButtons.YesNo );
                            if (result == DialogResult.No)
                                return;
                            else
                                File.Delete( saveFile );
                        }
                        texture.Save( Path.Combine( savePath, node.TexNames[i] ), ImageFileFormat.Png );
                        i++;
                    }
                }

                string filePath = Path.Combine( savePath, curObj.obj.name + ".xml" );
                if (File.Exists( filePath ))
                {
                    DialogResult result = MessageBox.Show( "XML文件已存在，是否覆盖？", "文件已存在", MessageBoxButtons.YesNo );
                    if (result == DialogResult.No)
                        return;
                    else
                        File.Delete( filePath );
                }

                FileStream stream = File.Create( filePath );

                GameObjData.Save( stream, curObj.obj );
            }
        }

        void Open ()
        {
            openGameObjDialog.ShowDialog();
        }

        private void openGameObjDialog_FileOk ( object sender, CancelEventArgs e )
        {
            try
            {
                FileStream stream = File.OpenRead( openGameObjDialog.FileName );
                GameObjData newObj = GameObjData.Load( stream );

                string path = Path.GetDirectoryName( openGameObjDialog.FileName );
                TreeNodeObj newObjNode = new TreeNodeObj( newObj, path );
                treeView.Nodes.Add( newObjNode );
                newObjNode.Expand();
                CurTreeNode = newObjNode.Nodes[0];
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region PictureBox

        bool editMode = false;

        private void UpdatePictureBox ()
        {
            if (CurBitMap != null && pictureBox.CurBitMap != CurBitMap)
                pictureBox.LoadPicture( CurBitMap );

            if (CurTexIndex == -1)
                pictureBox.ClearPicture();

            pictureBox.Invalidate();
        }

        private void pictureBox_Paint ( object sender, PaintEventArgs e )
        {
            if (CurBorderMap != null)
            {
                SpriteBorder.BorderMap borderMap = CurBorderMap;

                for (int y = -2; y < borderMap.Height - 2; y++)
                {
                    for (int x = -2; x < borderMap.Width - 2; x++)
                    {
                        if (borderMap[x, y])
                        {
                            e.Graphics.FillRectangle( new SolidBrush( System.Drawing.Color.Green ), pictureBox.RectAtPos( x, y ) );
                        }
                    }
                }
                if (showError)
                    e.Graphics.FillRectangle( new SolidBrush( System.Drawing.Color.Red ), pictureBox.RectAtPos( errorPoint.X, errorPoint.Y ) );
            }
        }

        void pictureBox_LastPaint ( object sender, PaintEventArgs e )
        {
            if (CurTreeNode != null && CurTreeNode is TreeNodeNode)
            {

                List<Microsoft.Xna.Framework.Vector2> visiPoints = ((TreeNodeNode)CurTreeNode).VisiPoints;
                PointF[] visiPos = new PointF[visiPoints.Count];
                for (int i = 0; i < visiPos.Length; i++)
                {
                    visiPos[i] = pictureBox.ScrnPos( visiPoints[i].X, visiPoints[i].Y );
                }

                List<Microsoft.Xna.Framework.Vector2> structPoints = ((TreeNodeNode)CurTreeNode).StructPoints;
                PointF[] structPos = new PointF[structPoints.Count];
                for (int i = 0; i < structPos.Length; i++)
                {
                    structPos[i] = pictureBox.ScrnPos( structPoints[i].X, structPoints[i].Y );
                }

                if (visiPos.Length > 0)
                {
                    foreach (PointF point in visiPos)
                    {
                        e.Graphics.DrawImage( visiPointMap, new PointF( point.X - 20, point.Y - 20 ) );
                    }
                }

                if (structPos.Length > 0)
                {
                    foreach (PointF point in structPos)
                    {
                        e.Graphics.DrawImage( structPointMap, new PointF( point.X - 20, point.Y - 20 ) );
                    }
                }
            }
        }

        void InitialPictureBoxContentMenu ()
        {
            pictureMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem checkBorder = new ToolStripMenuItem();
            checkBorder.Name = "提取边界";
            checkBorder.Text = "提取边界";
            ToolStripMenuItem alphaMode = new ToolStripMenuItem();
            alphaMode.Name = "Alpha模式";
            alphaMode.Text = "切换到Alpha模式";
            ToolStripMenuItem editMode = new ToolStripMenuItem();
            editMode.Name = "编辑模式";
            editMode.Text = "切换到编辑模式";
            ToolStripMenuItem pen = new ToolStripMenuItem();
            pen.Name = "画笔";
            pen.Text = "切换到透明画笔";
            pen.Visible = false;

            checkBorder.Click += new EventHandler( checkBorder_Click );
            alphaMode.Click += new EventHandler( alphaMode_Click );
            editMode.Click += new EventHandler( editMode_Click );
            pen.Click += new EventHandler( pen_Click );

            pictureMenuStrip.Items.AddRange( new ToolStripItem[] { checkBorder, alphaMode, pen, editMode } );
            pictureBox.ContextMenuStrip = pictureMenuStrip;
        }

        PenKind penKind = PenKind.black;
        enum PenKind
        {
            trans,
            half,
            black,
        }

        void pen_Click ( object sender, EventArgs e )
        {
            penKind = (PenKind)((((int)penKind) + 1) % 3);

            UpdateComponent();
        }

        void UpdatePictureMenuStrip ()
        {

            if (CurBitMap == null)
            {
                pictureMenuStrip.Items["编辑模式"].Enabled = false;
                pictureMenuStrip.Items["提取边界"].Enabled = false;
                pictureMenuStrip.Items["Alpha模式"].Enabled = false;
                pictureMenuStrip.Items["画笔"].Visible = false;
                pictureMenuStrip.Items["编辑模式"].Text = "切换到编辑模式";
                editMode = false;
            }
            else
            {
                pictureMenuStrip.Items["提取边界"].Enabled = true;
                pictureMenuStrip.Items["Alpha模式"].Enabled = true;
                pictureMenuStrip.Items["编辑模式"].Enabled = true;
            }

            if (CurTabState == TabState.Texture)
            {
                pictureMenuStrip.Items["提取边界"].Visible = true;
            }
            else if (CurTabState == TabState.VisiPoint)
            {
                pictureMenuStrip.Items["提取边界"].Visible = false;
                pictureMenuStrip.Items["画笔"].Visible = false;
            }
            else if (CurTabState == TabState.StructPoint)
            {
                pictureMenuStrip.Items["提取边界"].Visible = false;
                pictureMenuStrip.Items["画笔"].Visible = false;
            }

            if (editMode)
            {
                if (CurTabState == TabState.Texture)
                {
                    pictureMenuStrip.Items["画笔"].Visible = true;

                    if (penKind == PenKind.trans)
                    {
                        pictureMenuStrip.Items["画笔"].Text = "切换到半透明画笔";
                    }
                    else if (penKind == PenKind.half)
                    {
                        pictureMenuStrip.Items["画笔"].Text = "切换到黑色画笔";
                    }
                    else if (penKind == PenKind.black)
                    {
                        pictureMenuStrip.Items["画笔"].Text = "切换到透明画笔";
                    }


                }
                pictureMenuStrip.Items["编辑模式"].Text = "取消编辑模式";


            }
            else
            {
                pictureMenuStrip.Items["画笔"].Visible = false;
                pictureMenuStrip.Items["编辑模式"].Text = "切换到编辑模式";
            }

            if (pictureBox.AlphaMode)
            {
                pictureMenuStrip.Items["Alpha模式"].Text = "取消Alpha模式";
            }
            else
            {
                pictureMenuStrip.Items["Alpha模式"].Text = "切换到Alpha模式";
            }
        }

        void checkBorder_Click ( object sender, EventArgs e )
        {
            if (CheckBorder())
                MessageBox.Show( "生成边界成功！" );
            else
                MessageBox.Show( "生成边界失败！" );
            pictureBox.Invalidate();
        }

        void alphaMode_Click ( object sender, EventArgs e )
        {
            pictureBox.AlphaMode = !pictureBox.AlphaMode;
            UpdateComponent();
        }

        void editMode_Click ( object sender, EventArgs e )
        {
            editMode = !editMode;
            UpdateComponent();
        }

        private void pictureBox_MouseClick ( object sender, MouseEventArgs e )
        {
            if (editMode && !pictureBox.Controlling)
            {
                PointF texPos = pictureBox.TexPos( e.X, e.Y );
                if (CurTabState == TabState.Texture)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (penKind == PenKind.black)
                            SetAlpha( (int)texPos.X, (int)texPos.Y, 255 );
                        else if (penKind == PenKind.trans)
                            SetAlpha( (int)texPos.X, (int)texPos.Y, 0 );
                        else if (penKind == PenKind.half)
                            SetAlpha( (int)texPos.X, (int)texPos.Y, SpriteBorder.minBlockAlpha );
                    }
                }
            }
        }

        private void pictureBox_MouseDoubleClick ( object sender, MouseEventArgs e )
        {
            if (editMode && !pictureBox.Controlling)
            {
                PointF texPos = pictureBox.TexPos( e.X, e.Y );

                if (CurTabState == TabState.VisiPoint)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        ((TreeNodeNode)CurTreeNode).AddVisiPoint( texPos.X, texPos.Y );
                    }
                }
                else if (CurTabState == TabState.StructPoint)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        ((TreeNodeNode)CurTreeNode).AddStructPoint( texPos.X, texPos.Y );
                    }
                }
            }
            pictureBox.Invalidate();
            UpdateVisiList();
            UpdateStructList();
        }

        private void pictureBox_MouseMove ( object sender, MouseEventArgs e )
        {
            if (pictureBox.CurBitMap != null)
            {
                PointF curMousePos = pictureBox.TexPos( e.X, e.Y );
                statusStrip.Items["toolStripStatusMousePos"].Text = "当前鼠标位置：" + curMousePos.ToString();
            }
        }

        void SetAlpha ( int x, int y, byte alpha )
        {
            if (CurBitMap != null && CurXNATex != null &&
                x >= 0 && x < CurBitMap.Width &&
                y >= 0 && y < CurBitMap.Height)
            {
                System.Drawing.Color preColor = CurBitMap.GetPixel( x, y );
                CurBitMap.SetPixel( x, y, System.Drawing.Color.FromArgb( alpha, preColor ) );

                Microsoft.Xna.Framework.Graphics.Color[] data = new Microsoft.Xna.Framework.Graphics.Color[CurXNATex.Width * CurXNATex.Height];
                CurXNATex.GetData<Microsoft.Xna.Framework.Graphics.Color>( data );
                Microsoft.Xna.Framework.Graphics.Color preXNAColor = data[y * CurXNATex.Width + x];
                data[y * CurXNATex.Width + x] = new Microsoft.Xna.Framework.Graphics.Color( preXNAColor.R, preXNAColor.G, preXNAColor.B, alpha );
                CurXNATex.SetData<Microsoft.Xna.Framework.Graphics.Color>( data );
                pictureBox.Invalidate();
            }
        }

        #endregion

        #region StatusBar

        void UpdateStatusBar ()
        {
            if (CurTreeObj != null)
            {
                statusStrip.Items["toolStripStatusBaseInfo"].Text = CurTreeObj.obj.name + " / " + CurTreeObj.obj.creater + " / " + CurTreeObj.obj.year + ":" + CurTreeObj.obj.month + ":" + CurTreeObj.obj.day;
                if (editMode)
                {
                    statusStrip.Items["toolStripStatusLabelEdit"].Text = "编辑模式";
                    statusStrip.Items["toolStripStatusLabelEditState"].Visible = true;
                    if (CurTabState == TabState.Texture)
                    {
                        if (penKind == PenKind.black)
                            statusStrip.Items["toolStripStatusLabelEditState"].Text = "黑色画笔";
                        else if (penKind == PenKind.half)
                            statusStrip.Items["toolStripStatusLabelEditState"].Text = "半透明画笔";
                        else
                            statusStrip.Items["toolStripStatusLabelEditState"].Text = "透明画笔";



                    }
                    else if (CurTabState == TabState.VisiPoint)
                    {
                        statusStrip.Items["toolStripStatusLabelEditState"].Text = "双击添加可视关键点";
                    }
                    else if (CurTabState == TabState.StructPoint)
                    {
                        statusStrip.Items["toolStripStatusLabelEditState"].Text = "双击添加结构关键点";
                    }
                }
                else
                {
                    statusStrip.Items["toolStripStatusLabelEdit"].Text = "查看模式";
                    statusStrip.Items["toolStripStatusLabelEditState"].Visible = false;
                }

                if (pictureBox.AlphaMode)
                {
                    statusStrip.Items["toolStripStatusAlpha"].Text = "Alpha模式";
                }
                else
                {
                    statusStrip.Items["toolStripStatusAlpha"].Text = "颜色模式";
                }
            }


        }

        #endregion

        #region IntList

        private void InitialDataListContentMenu ()
        {
            dataMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem addData = new ToolStripMenuItem();
            addData.Name = "添加新数据";
            addData.Text = "添加新数据";

            ToolStripMenuItem delData = new ToolStripMenuItem();
            delData.Name = "删除数据";
            delData.Text = "删除数据";

            addData.Click += new EventHandler( addData_Click );
            delData.Click += new EventHandler( delData_Click );

            dataMenuStrip.Items.AddRange( new ToolStripItem[] { addData, delData } );

            listViewInt.ContextMenuStrip = dataMenuStrip;
            listViewFloat.ContextMenuStrip = dataMenuStrip;
        }

        void delData_Click ( object sender, EventArgs e )
        {
            if (CurTabState == TabState.IntData)
            {
                if (listViewInt.SelectedIndices.Count > 0 && CurNodeNode != null)
                {
                    int delIndex = listViewInt.SelectedIndices[0];
                    CurNodeNode.DelIntData( delIndex );
                }
            }
            else if (CurTabState == TabState.FloatData)
            {
                if (listViewFloat.SelectedIndices.Count > 0 && CurNodeNode != null)
                {
                    int delIndex = listViewFloat.SelectedIndices[0];
                    CurNodeNode.DelFloatData( delIndex );
                }
            }
            UpdateComponent();
        }

        void addData_Click ( object sender, EventArgs e )
        {
            if (CurTabState == TabState.IntData)
            {
                if (CurNodeNode != null)
                {
                    EnterNumber enter = new EnterNumber();
                    enter.ShowDialog();
                    int value = new int();
                    try
                    {
                        value = int.Parse( enter.NumberText );
                    }
                    catch (Exception)
                    {
                        MessageBox.Show( "输入格式错误！" );
                        return;
                    }
                    CurNodeNode.AddIntData( value );
                }
            }
            else if (CurTabState == TabState.FloatData)
            {
                if (CurNodeNode != null)
                {
                    EnterNumber enter = new EnterNumber();
                    enter.ShowDialog();
                    float value = new float();
                    try
                    {
                        value = float.Parse( enter.NumberText );
                    }
                    catch (Exception)
                    {
                        MessageBox.Show( "输入格式错误！" );
                        return;
                    }
                    CurNodeNode.AddFloatData( value );
                }
            }
            UpdateComponent();
        }

        void UpdateDataList ()
        {
            listViewInt.Items.Clear();

            if (CurNodeNode != null)
            {
                int index = 0;
                foreach (int i in CurNodeNode.IntDatas)
                {
                    ListViewItem newItem = new ListViewItem( index.ToString() );
                    newItem.SubItems.Add( i.ToString() );
                    listViewInt.Items.Add( newItem );
                    index++;
                }
            }

            listViewFloat.Items.Clear();

            if (CurNodeNode != null)
            {
                int index = 0;
                foreach (float f in CurNodeNode.FloatDatas)
                {
                    ListViewItem newItem = new ListViewItem( index.ToString() );
                    newItem.SubItems.Add( f.ToString() );
                    listViewFloat.Items.Add( newItem );
                    index++;
                }
            }

        }

        #endregion

    }
}