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
        #region TypeDef
        class TreeNodeEnum : TreeNode, IEnumerable<TreeNodeEnum>
        {

            public TreeNodeEnum ( string text )
                : base( text )
            {
            }

            public TreeNodeEnum ()
                : base()
            {
            }

            #region IEnumerable<TreeNodeEnum> 成员

            public IEnumerator<TreeNodeEnum> GetEnumerator ()
            {
                yield return this;

                foreach (TreeNode childNode in Nodes)
                {
                    if (!(childNode is TreeNodeEnum))
                        throw new Exception( "childNode isn't a TreeNodeEnum." );

                    foreach (TreeNodeEnum downNode in (TreeNodeEnum)childNode)
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
        List<GameObjData> gameObjs;

        ContextMenuStrip treeMenuStrip;

        TreeNodeEnum CurTreeNode
        {
            get { return (TreeNodeEnum)treeView.SelectedNode; }
            set { treeView.SelectedNode = value; }
        }
        GameObjData curObj;
        GameObjDataNode curDataNode; 
        #endregion

        public GameObjEditer ()
        {
            InitializeComponent();
            InitialTreeView();
        }

        void InitialTreeView ()
        {
            gameObjs = new List<GameObjData>();
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

        #region ContentMenu

        void addNewObjLabel_Click ( object sender, EventArgs e )
        {
            AddNewObj();
        }

        void delObjLabel_Click ( object sender, EventArgs e )
        {
            DelObj();
        }

        void addChildLabel_Click ( object sender, EventArgs e )
        {
            AddNode();
        }

        void delNodeLabel_Click ( object sender, EventArgs e )
        {
            DialogResult result = MessageBox.Show( "这将删除当前的节点，是否继续？", "确定删除", MessageBoxButtons.YesNo );
            if (result == DialogResult.No)
                return;
            DelNode();
        }

        void renameLabel_Click ( object sender, EventArgs e )
        {
            throw new Exception( "The method or operation is not implemented." );
        } 

        private void treeView_MouseClick ( object sender, MouseEventArgs e )
        {
            TreeNodeEnum node = (TreeNodeEnum)treeView.GetNodeAt( e.X, e.Y );

            if (node == null)
                return;

            CurTreeNode = node;
            FindCurNode();
        }

        private void FindCurNode ()
        {
            if (CurTreeNode == null)
            {
                curObj = null;
                curDataNode = null;
            }
            else
            {

                TreeNodeEnum treeRootNode = CurTreeNode;
                while (treeRootNode.Parent != null)
                {
                    treeRootNode = (TreeNodeEnum)treeRootNode.Parent;
                }

                int indexRoot = treeView.Nodes.IndexOf( treeRootNode );
                curObj = gameObjs[indexRoot];

                int indexCur = -1;
                foreach (TreeNodeEnum downNode in treeRootNode)
                {
                    if (downNode == CurTreeNode)
                        break;
                    indexCur++;
                }

                curDataNode = curObj[indexCur];
            }

            treeMenuStrip.Items["删除物体"].Enabled = true;
            treeMenuStrip.Items["添加子节点"].Enabled = true;
            treeMenuStrip.Items["删除节点"].Enabled = true;
            treeMenuStrip.Items["重命名"].Enabled = true;

            if (curDataNode == null)
            {
                treeMenuStrip.Items["添加子节点"].Enabled = false;
                treeMenuStrip.Items["删除节点"].Enabled = false;
                treeMenuStrip.Items["重命名"].Enabled = false;
            }
            else if (curDataNode.parent == null)
            {
                treeMenuStrip.Items["删除节点"].Enabled = false;
            }

            if (curObj == null)
            {
                treeMenuStrip.Items["删除物体"].Enabled = false;
            }
        }

        void AddNewObj ()
        {
            TreeNodeEnum newTreeNode = new TreeNodeEnum( "NewGameObj" );
            treeView.Nodes.Add( newTreeNode );
            GameObjData newGameObj = new GameObjData( "NewGameObj" );
            gameObjs.Add( newGameObj );

            newTreeNode.Nodes.Add( new TreeNodeEnum( newGameObj.baseNode.nodeName ) );
            CurTreeNode = (TreeNodeEnum)newTreeNode.Nodes[0];
            curObj = newGameObj;
            curDataNode = newGameObj.baseNode;

            newTreeNode.Expand();

            FindCurNode();
        }

        void DelObj ()
        {
            if (curObj == null)
                return;

            int index = gameObjs.IndexOf( curObj );

            gameObjs.Remove( curObj );

            treeView.Nodes.RemoveAt( index );

            int nextIndex = -1;

            if (index == 0)
            {
                if (gameObjs.Count == 0)
                {
                    curObj = null;
                }
                else
                {
                    curObj = gameObjs[0];
                    nextIndex = 0;
                }
            }
            else
            {
                nextIndex = index - 1;
                curObj = gameObjs[nextIndex];
            }

            if (curObj != null)
            {
                CurTreeNode = (TreeNodeEnum)treeView.Nodes[nextIndex].Nodes[0];
            }
            else
            {
                CurTreeNode = null;
            }

            FindCurNode();
        }

        void AddNode ()
        {
            if (curDataNode == null || CurTreeNode == null)
                return;

            TreeNodeEnum newTreeNode = new TreeNodeEnum( "newNode" );
            CurTreeNode.Nodes.Add( newTreeNode );
            GameObjDataNode newDataNode = new GameObjDataNode( "newNode", curDataNode );
            curDataNode.childNodes.Add( newDataNode );

            CurTreeNode = newTreeNode;
            FindCurNode();
        }

        void DelNode ()
        {
            if (curDataNode == null || curDataNode.parent == null)
                return;

            GameObjDataNode parent = curDataNode.parent;
            parent.childNodes.Remove( curDataNode );
            TreeNodeEnum parentTree = (TreeNodeEnum)CurTreeNode.Parent;
            parentTree.Nodes.Remove( CurTreeNode );
            CurTreeNode = parentTree;

            FindCurNode();
        }

        #endregion

    }
}