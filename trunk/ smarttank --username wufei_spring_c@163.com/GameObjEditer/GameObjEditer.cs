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

            #region IEnumerable<TreeNodeEnum> ��Ա

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

            #region IEnumerable ��Ա

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
            DialogResult result = MessageBox.Show( "�⽫ɾ����ǰ�Ľڵ㣬�Ƿ������", "ȷ��ɾ��", MessageBoxButtons.YesNo );
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

            treeMenuStrip.Items["ɾ������"].Enabled = true;
            treeMenuStrip.Items["����ӽڵ�"].Enabled = true;
            treeMenuStrip.Items["ɾ���ڵ�"].Enabled = true;
            treeMenuStrip.Items["������"].Enabled = true;

            if (curDataNode == null)
            {
                treeMenuStrip.Items["����ӽڵ�"].Enabled = false;
                treeMenuStrip.Items["ɾ���ڵ�"].Enabled = false;
                treeMenuStrip.Items["������"].Enabled = false;
            }
            else if (curDataNode.parent == null)
            {
                treeMenuStrip.Items["ɾ���ڵ�"].Enabled = false;
            }

            if (curObj == null)
            {
                treeMenuStrip.Items["ɾ������"].Enabled = false;
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