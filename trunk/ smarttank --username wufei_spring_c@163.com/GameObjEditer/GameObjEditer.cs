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

            public TreeNodeNode ( string text )
                : base( text )
            {
                textures = new List<Texture2D>();
                bitmaps = new List<Bitmap>();
                texNames = new List<string>();
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

            public int CurTexIndex
            {
                get { return curTexIndex; }
                set { curTexIndex = value; }
            }

            public Texture2D CurXNATex
            {
                get
                {
                    if (textures.Count > 0)
                        return textures[curTexIndex];
                    else
                        return null;
                }
            }

            public Bitmap CurBitMap
            {
                get
                {
                    if (bitmaps.Count > 0)
                        return bitmaps[curTexIndex];
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

            public void AddTex ( string filePath, GraphicsDevice device )
            {
                textures.Add( Texture2D.FromFile( device, filePath ) );
                bitmaps.Add( new Bitmap( filePath ) );
                texNames.Add( Path.GetFileName( filePath ) );
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
                dateNode.texPaths.RemoveAt( index );
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

        #endregion

        public GameObjEditer ()
        {
            InitializeComponent();
            InitialTreeViewContentMenu();
            InitialTexContentMenu();
            InitialGraphicsDevice();

            InitialTexContentMenu();
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
        }

        private void UpdatePictureBox ()
        {
            if (CurBitMap != null)
                pictureBox.LoadPicture( CurBitMap );

            if (CurTexIndex == -1)
                pictureBox.ClearPicture();

            pictureBox.Invalidate();
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
            showError = false;
        }

        void delTexLabel_Click ( object sender, EventArgs e )
        {
            DelTex();
            UpdateComponent();
            pictureBox.Invalidate();
            showError = false;
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
            CheckBorder();
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
                CurTexIndex--;


            }
        }

        void UpTex ()
        {
            if (CurTexIndex > 0 && CurTreeNode is TreeNodeNode)
            {
                ((TreeNodeNode)CurTreeNode).UpTex( CurTexIndex );
                CurTexIndex--;
            }
        }

        void DownTex ()
        {
            if (CurTexIndex >= 0 && CurTreeNode is TreeNodeNode)
            {
                ((TreeNodeNode)CurTreeNode).downTex( CurTexIndex );
                CurTexIndex++;
            }
        }

        #endregion

        #region CheckBorder

        SpriteBorder.BorderMap borderMap;
        Point errorPoint;

        bool showError = false;

        void CheckBorder ()
        {
            if (CurXNATex != null)
            {
                showError = false;
                try
                {
                    Sprite.CheckBorder( CurXNATex );
                }
                catch (BorderBulidException e)
                {
                    borderMap = e.borderMap;
                    errorPoint = new Point( e.curPoint.X, e.curPoint.Y );
                    showError = true;
                    pictureBox.TexFocusPos = errorPoint;
                    pictureBox.Invalidate();
                }

                if (!showError)
                    MessageBox.Show( "���ɱ߽�ɹ���" );
                else
                    MessageBox.Show( "���ɱ߽�ʧ�ܣ�" );
            }
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

        private void pictureBox_Paint ( object sender, PaintEventArgs e )
        {
            if (showError)
            {
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
                e.Graphics.FillRectangle( new SolidBrush( System.Drawing.Color.Red ), pictureBox.RectAtPos( errorPoint.X, errorPoint.Y ) );
            }
        }

    }
}