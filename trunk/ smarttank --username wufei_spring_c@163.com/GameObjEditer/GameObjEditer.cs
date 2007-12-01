using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Platform.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using GameBase.Graphics;

namespace GameObjEditer
{
    public partial class GameObjEditer : Form
    {
        #region TypeDef

        class TreeNodeObj : TreeNode
        {
            GameObjData obj;

            public GameObjData GameObj
            {
                get { return obj; }
            }

            public TreeNodeObj ( string objName )
                : base( objName )
            {
                obj = new GameObjData( objName );
                TreeNodeNode baseNode = new TreeNodeNode( "Base" );
                this.Nodes.Add( baseNode );
                TreeNodeNode.AddToChilds( this, baseNode );
            }

            public void Rename ( string newName )
            {
                if (newName == string.Empty)
                    return;

                Text = newName;
                obj.name = newName;
            }
        }

        class TreeNodeNode : TreeNode, IEnumerable<TreeNodeNode>
        {
            static public void AddToChilds ( TreeNode parent, TreeNodeNode child )
            {
                if (parent is TreeNodeObj)
                {
                    if (((TreeNodeObj)parent).GameObj.baseNode == null)
                    {
                        ((TreeNodeObj)parent).GameObj.baseNode = child.dateNode;
                        parent.Nodes.Add( child );
                    }
                }
                else if (parent is TreeNodeNode)
                {
                    ((TreeNodeNode)parent).dateNode.childNodes.Add( child.dateNode );
                    child.dateNode.parent = ((TreeNodeNode)parent).dateNode;
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
                get { return dateNode.visiKeyPoint; }
            }

            public List<Microsoft.Xna.Framework.Vector2> StructPoints
            {
                get { return dateNode.structKeyPoint; }
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

            public void AddTex ( string filePath, GraphicsDevice device )
            {
                textures.Add( Texture2D.FromFile( device, filePath ) );
                bitmaps.Add( new Bitmap( filePath ) );
                texNames.Add( Path.GetFileName( filePath ) );
                borderMaps.Add( null );
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
                dateNode.visiKeyPoint.Add( new Microsoft.Xna.Framework.Vector2( x, y ) );

                curVisiPointIndex = dateNode.visiKeyPoint.Count - 1;
            }

            public void DelVisiPoint ()
            {
                if (curVisiPointIndex < 0 || curVisiPointIndex >= dateNode.visiKeyPoint.Count)
                    return;

                dateNode.visiKeyPoint.RemoveAt( curVisiPointIndex );

                curVisiPointIndex--;
            }

            public void UpVisiPoint ()
            {
                if (curVisiPointIndex <= 0 || curVisiPointIndex >= dateNode.visiKeyPoint.Count)
                    return;

                Microsoft.Xna.Framework.Vector2 temp = dateNode.visiKeyPoint[curVisiPointIndex];
                dateNode.visiKeyPoint[curVisiPointIndex] = dateNode.visiKeyPoint[curVisiPointIndex - 1];
                dateNode.visiKeyPoint[curVisiPointIndex - 1] = temp;

                curVisiPointIndex--;
            }

            public void DownVisiPoint ()
            {
                if (curVisiPointIndex < 0 || curVisiPointIndex >= dateNode.visiKeyPoint.Count - 1)
                    return;

                Microsoft.Xna.Framework.Vector2 temp = dateNode.visiKeyPoint[curVisiPointIndex];
                dateNode.visiKeyPoint[curVisiPointIndex] = dateNode.visiKeyPoint[curVisiPointIndex + 1];
                dateNode.visiKeyPoint[curVisiPointIndex + 1] = temp;

                curVisiPointIndex++;
            }

            public void AddStructPoint ( float x, float y )
            {
                dateNode.structKeyPoint.Add( new Microsoft.Xna.Framework.Vector2( x, y ) );

                curStructPointIndex = dateNode.structKeyPoint.Count - 1;
            }

            public void DelStructiPoint ()
            {
                if (curStructPointIndex < 0 || curStructPointIndex >= dateNode.structKeyPoint.Count)
                    return;

                dateNode.structKeyPoint.RemoveAt( curStructPointIndex );

                curStructPointIndex--;
            }

            public void UpStructPoint ()
            {
                if (curStructPointIndex <= 0 || curStructPointIndex >= dateNode.structKeyPoint.Count)
                    return;

                Microsoft.Xna.Framework.Vector2 temp = dateNode.structKeyPoint[curStructPointIndex];
                dateNode.structKeyPoint[curStructPointIndex] = dateNode.structKeyPoint[curStructPointIndex - 1];
                dateNode.structKeyPoint[curStructPointIndex - 1] = temp;

                curStructPointIndex--;
            }

            public void DownStructPoint ()
            {
                if (curStructPointIndex < 0 || curStructPointIndex >= dateNode.structKeyPoint.Count - 1)
                    return;

                Microsoft.Xna.Framework.Vector2 temp = dateNode.structKeyPoint[curStructPointIndex];
                dateNode.structKeyPoint[curStructPointIndex] = dateNode.structKeyPoint[curStructPointIndex + 1];
                dateNode.structKeyPoint[curStructPointIndex + 1] = temp;

                curStructPointIndex++;
            }

            #region IEnumerable<TreeNodeEnum> ��Ա

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

            #region IEnumerable ��Ա

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

        TreeNode CurTreeNode
        {
            get { return treeView.SelectedNode; }
            set { treeView.SelectedNode = value; }
        }

        GraphicsDevice device;

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
                else
                {
                    return TabState.StructPoint;
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
            addNewObjLabel.Name = "���������";
            addNewObjLabel.Text = "���������";
            ToolStripMenuItem delObjLabel = new ToolStripMenuItem();
            delObjLabel.Name = "ɾ������";
            delObjLabel.Text = "ɾ������";
            ToolStripMenuItem addChildLabel = new ToolStripMenuItem();
            addChildLabel.Name = "����ӽڵ�";
            addChildLabel.Text = "����ӽڵ�";
            ToolStripMenuItem delNodeLabel = new ToolStripMenuItem();
            delNodeLabel.Name = "ɾ���ڵ�";
            delNodeLabel.Text = "ɾ���ڵ�";
            ToolStripMenuItem renameLabel = new ToolStripMenuItem();
            renameLabel.Name = "������";
            renameLabel.Text = "������";

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
            DialogResult result = MessageBox.Show( "�⽫ɾ����ǰ���ڱ༭�ĳ������壬�Ƿ������", "ȷ��ɾ��", MessageBoxButtons.YesNo );
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
            DialogResult result = MessageBox.Show( "�⽫ɾ����ǰ�Ľڵ㣬�Ƿ������", "ȷ��ɾ��", MessageBoxButtons.YesNo );
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
            treeMenuStrip.Items["ɾ������"].Enabled = true;
            treeMenuStrip.Items["����ӽڵ�"].Enabled = true;
            treeMenuStrip.Items["ɾ���ڵ�"].Enabled = true;
            treeMenuStrip.Items["������"].Enabled = true;

            if (CurTreeNode == null)
            {
                treeMenuStrip.Items["ɾ������"].Enabled = false;
                treeMenuStrip.Items["����ӽڵ�"].Enabled = false;
                treeMenuStrip.Items["ɾ���ڵ�"].Enabled = false;
                treeMenuStrip.Items["������"].Enabled = false;
            }
            else if (CurTreeNode is TreeNodeObj)
            {
                treeMenuStrip.Items["����ӽڵ�"].Enabled = false;
                treeMenuStrip.Items["ɾ���ڵ�"].Enabled = false;
            }
            else if (CurTreeNode is TreeNodeNode && CurTreeNode.Parent is TreeNodeObj)
            {
                treeMenuStrip.Items["ɾ���ڵ�"].Enabled = false;
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
            importTexLabel.Name = "������ͼ";
            importTexLabel.Text = "������ͼ";
            ToolStripMenuItem delTexLabel = new ToolStripMenuItem();
            delTexLabel.Name = "�Ƴ���ͼ";
            delTexLabel.Text = "�Ƴ���ͼ";
            ToolStripMenuItem upTexLabel = new ToolStripMenuItem();
            upTexLabel.Name = "����";
            upTexLabel.Text = "����";
            ToolStripMenuItem downTexLabel = new ToolStripMenuItem();
            downTexLabel.Name = "����";
            downTexLabel.Text = "����";
            ToolStripMenuItem checkBorderLabel = new ToolStripMenuItem();
            checkBorderLabel.Name = "��ȡ�߽�";
            checkBorderLabel.Text = "��ȡ�߽�";

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
                MessageBox.Show( "���ɱ߽�ɹ���" );
            else
                MessageBox.Show( "���ɱ߽�ʧ�ܣ�" );
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
            texListMenuStrip.Items["�Ƴ���ͼ"].Enabled = true;
            texListMenuStrip.Items["����"].Enabled = true;
            texListMenuStrip.Items["����"].Enabled = true;
            texListMenuStrip.Items["��ȡ�߽�"].Enabled = true;


            if (CurTexIndex == -1)
            {
                texListMenuStrip.Items["�Ƴ���ͼ"].Enabled = false;
                texListMenuStrip.Items["����"].Enabled = false;
                texListMenuStrip.Items["����"].Enabled = false;
                texListMenuStrip.Items["��ȡ�߽�"].Enabled = false;
            }
            if (CurTexIndex == 0)
            {
                texListMenuStrip.Items["����"].Enabled = false;
            }
            if (CurTexIndex == listViewTex.Items.Count - 1)
            {
                texListMenuStrip.Items["����"].Enabled = false;
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
            ((TreeNodeNode)CurTreeNode).AddTex( texPath, device );
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
            delPoint.Name = "ɾ���ؼ���";
            delPoint.Text = "ɾ���ؼ���";
            ToolStripMenuItem upPoint = new ToolStripMenuItem();
            upPoint.Name = "����";
            upPoint.Text = "����";
            ToolStripMenuItem downPoint = new ToolStripMenuItem();
            downPoint.Name = "�½�";
            downPoint.Text = "�½�";

            delPoint.Click += new EventHandler( delPoint_Click );
            upPoint.Click += new EventHandler( upPoint_Click );
            downPoint.Click += new EventHandler( downPoint_Click );

            pointMenuStrip.Items.AddRange( new ToolStripItem[] { delPoint, upPoint, downPoint } );

            listViewVisi.ContextMenuStrip = pointMenuStrip;
            listViewStruct.ContextMenuStrip = pointMenuStrip;
        }

        void UpdatePointContentMenu ()
        {
            pointMenuStrip.Items["ɾ���ؼ���"].Enabled = true;
            pointMenuStrip.Items["����"].Enabled = true;
            pointMenuStrip.Items["�½�"].Enabled = true;

            if (CurTreeNode == null)
            {
                pointMenuStrip.Items["ɾ���ؼ���"].Enabled = false;
                pointMenuStrip.Items["����"].Enabled = false;
                pointMenuStrip.Items["�½�"].Enabled = false;
            }
            else
            {
                if (CurTabState == TabState.VisiPoint)
                {
                    int index = ((TreeNodeNode)CurTreeNode).CurVisiPointIndex;
                    int count = ((TreeNodeNode)CurTreeNode).VisiPoints.Count;
                    if (index < 0 || index >= count)
                    {
                        pointMenuStrip.Items["ɾ���ؼ���"].Enabled = false;
                        pointMenuStrip.Items["����"].Enabled = false;
                        pointMenuStrip.Items["�½�"].Enabled = false;
                    }

                    if (index <= 0)
                    {
                        pointMenuStrip.Items["����"].Enabled = false;
                    }
                    if (index >= count - 1)
                    {
                        pointMenuStrip.Items["�½�"].Enabled = false;
                    }

                }
                else if (CurTabState == TabState.StructPoint)
                {
                    int index = ((TreeNodeNode)CurTreeNode).CurStructPointIndex;
                    int count = ((TreeNodeNode)CurTreeNode).StructPoints.Count;

                    if (index < 0 || index >= count)
                    {
                        pointMenuStrip.Items["ɾ���ؼ���"].Enabled = false;
                        pointMenuStrip.Items["����"].Enabled = false;
                        pointMenuStrip.Items["�½�"].Enabled = false;
                    }

                    if (index <= 0)
                    {
                        pointMenuStrip.Items["����"].Enabled = false;
                    }
                    if (index >= count - 1)
                    {
                        pointMenuStrip.Items["�½�"].Enabled = false;
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
            if (CurTreeNode != null)
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
        }

        void UpdateStructList ()
        {
            if (CurTreeNode != null)
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

        }

        private void OpenToolStripMenuItem_Click ( object sender, EventArgs e )
        {

        }

        private void BorderToolStripMenuItem_Click ( object sender, EventArgs e )
        {

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

        void InitialPictureBoxContentMenu ()
        {
            pictureMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem checkBorder = new ToolStripMenuItem();
            checkBorder.Name = "��ȡ�߽�";
            checkBorder.Text = "��ȡ�߽�";
            ToolStripMenuItem alphaMode = new ToolStripMenuItem();
            alphaMode.Name = "Alphaģʽ";
            alphaMode.Text = "�л���Alphaģʽ";
            ToolStripMenuItem editMode = new ToolStripMenuItem();
            editMode.Name = "�༭ģʽ";
            editMode.Text = "�л����༭ģʽ";
            ToolStripMenuItem pen = new ToolStripMenuItem();
            pen.Name = "����";
            pen.Text = "�л���͸������";
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
            if (penKind == PenKind.trans)
            {
                pictureMenuStrip.Items["����"].Text = "�л�����͸������";
            }
            else if (penKind == PenKind.half)
            {
                pictureMenuStrip.Items["����"].Text = "�л�����ɫ����";
            }
            else if (penKind == PenKind.black)
            {
                pictureMenuStrip.Items["����"].Text = "�л���͸������";
            }
        }

        void UpdatePictureMenuStrip ()
        {

            if (CurBitMap == null)
            {
                pictureMenuStrip.Items["�༭ģʽ"].Enabled = false;
                pictureMenuStrip.Items["��ȡ�߽�"].Enabled = false;
                pictureMenuStrip.Items["Alphaģʽ"].Enabled = false;
                pictureMenuStrip.Items["����"].Visible = false;
                pictureMenuStrip.Items["�༭ģʽ"].Text = "�л����༭ģʽ";
                editMode = false;
            }
            else
            {
                pictureMenuStrip.Items["��ȡ�߽�"].Enabled = true;
                pictureMenuStrip.Items["Alphaģʽ"].Enabled = true;
                pictureMenuStrip.Items["�༭ģʽ"].Enabled = true;
            }

            if (CurTabState == TabState.Texture)
            {
                pictureMenuStrip.Items["��ȡ�߽�"].Visible = true;
                pictureMenuStrip.Items["Alphaģʽ"].Visible = true;
            }
            else if (CurTabState == TabState.VisiPoint)
            {
                pictureMenuStrip.Items["��ȡ�߽�"].Visible = false;
                pictureMenuStrip.Items["Alphaģʽ"].Visible = false;
                pictureMenuStrip.Items["����"].Visible = false;
            }
            else if (CurTabState == TabState.StructPoint)
            {
                pictureMenuStrip.Items["��ȡ�߽�"].Visible = false;
                pictureMenuStrip.Items["Alphaģʽ"].Visible = false;
                pictureMenuStrip.Items["����"].Visible = false;
            }
        }

        void checkBorder_Click ( object sender, EventArgs e )
        {
            if (CheckBorder())
                MessageBox.Show( "���ɱ߽�ɹ���" );
            else
                MessageBox.Show( "���ɱ߽�ʧ�ܣ�" );
            pictureBox.Invalidate();
        }

        void alphaMode_Click ( object sender, EventArgs e )
        {
            pictureBox.AlphaMode = !pictureBox.AlphaMode;
            if (pictureBox.AlphaMode)
            {
                pictureMenuStrip.Items["Alphaģʽ"].Text = "ȡ��Alphaģʽ";
            }
            else
            {
                pictureMenuStrip.Items["Alphaģʽ"].Text = "�л���Alphaģʽ";
            }
            pictureBox.Invalidate();
        }

        void editMode_Click ( object sender, EventArgs e )
        {
            editMode = !editMode;

            if (editMode)
            {
                if (CurTabState == TabState.Texture)
                {
                    pictureMenuStrip.Items["����"].Visible = true;
                }
                pictureMenuStrip.Items["�༭ģʽ"].Text = "ȡ���༭ģʽ";
            }
            else
            {
                pictureMenuStrip.Items["����"].Visible = false;
                pictureMenuStrip.Items["�༭ģʽ"].Text = "�л����༭ģʽ";
            }
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

    }
}