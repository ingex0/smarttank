using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using SmartTank.Helpers.DependInject;
using SmartTank.Rule;
using SmartTank.AI;

namespace AssetRegister
{
    public partial class AssetRegister : Form
    {
        readonly string rulesPath = "Rule";
        readonly string rulesListPath = "Rule\\List.xml";

        readonly string AIPath = "AIs";
        readonly string AIListPath = "AIs\\List.xml";

        AssetList rulelist;
        AssetList AIlist;

        public AssetRegister ()
        {
            InitializeComponent();
            InitialRuleList();
            InitialAIList();
        }

        private void InitialRuleList ()
        {
            Directory.SetCurrentDirectory( Application.StartupPath );
            if (!Directory.Exists( rulesPath ))
            {
                Directory.CreateDirectory( rulesPath );
            }
            rulelist = AssetList.Load( File.Open( rulesListPath, FileMode.OpenOrCreate ) );

            if (rulelist == null) rulelist = new AssetList();

            ListViewItem myItem = new ListViewItem();
            listViewRule.Items.Clear();
            foreach (AssetItem item in rulelist.list)
            {
                foreach (NameAndTypeName item1 in item.names)
                {
                    myItem = listViewRule.Items.Add( item.DLLName );
                    myItem.SubItems.Add( item1.name );
                    myItem.SubItems.Add( item1.typeNames );
                }
            }
        }

        private void InitialAIList ()
        {
            Directory.SetCurrentDirectory( Application.StartupPath );
            if (!Directory.Exists( AIPath ))
            {
                Directory.CreateDirectory( AIPath );
            }
            AIlist = AssetList.Load( File.Open( AIListPath, FileMode.OpenOrCreate ) );
            if (AIlist == null) AIlist = new AssetList();

            ListViewItem myItem = new ListViewItem();
            listViewAI.Items.Clear();
            foreach (AssetItem item in AIlist.list)
            {
                foreach (NameAndTypeName item1 in item.names)
                {
                    myItem = listViewAI.Items.Add( item.DLLName );
                    myItem.SubItems.Add( item1.name );
                    myItem.SubItems.Add( item1.typeNames );
                }
            }
        }

        #region AddRule

        private void OpenRuleDLLFileDialog_FileOk ( object sender, CancelEventArgs e )
        {
            List<AssetItem> rulesList = new List<AssetItem>();

            List<string> vaildDLL = new List<string>();

            int countVaildClass = 0;

            foreach (string fileName in OpenRuleDLLFileDialog.FileNames)
            {
                Assembly assembly = DIHelper.GetAssembly( fileName );

                AssetItem curItem;
                bool findGameRule = false;
                curItem = new AssetItem();
                curItem.DLLName = assembly.FullName;// Path.GetFileName( fileName );
                List<NameAndTypeName> names = new List<NameAndTypeName>();
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        foreach (Type infer in type.GetInterfaces())
                        {
                            if (infer.Name == "IGameRule")
                            {
                                object[] attributes = type.GetCustomAttributes( typeof( RuleAttribute ), false );
                                foreach (object attri in attributes)
                                {
                                    if (attri is RuleAttribute)
                                    {
                                        findGameRule = true;
                                        string ruleName = ((RuleAttribute)attri).Name;
                                        names.Add( new NameAndTypeName( ruleName, type.FullName ) );
                                        countVaildClass++;
                                        break;
                                    }
                                }
                                if (findGameRule)
                                    break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    findGameRule = false;
                }
                if (findGameRule)
                {
                    curItem.names = names.ToArray();
                    rulesList.Add( curItem );
                    vaildDLL.Add( fileName );
                }
            }

            if (rulesList.Count != 0)
            {
                Directory.SetCurrentDirectory( Application.StartupPath );
                if (!Directory.Exists( rulesPath ))
                {
                    Directory.CreateDirectory( rulesPath );
                }
                AssetList list = AssetList.Load( File.Open( rulesListPath, FileMode.OpenOrCreate ) );
                if (list == null)
                    list = new AssetList();


                foreach (AssetItem item in rulesList)
                {
                    int index = list.IndexOf( item.DLLName );
                    if (index != -1)
                        list.list[index] = item;
                    else
                        list.list.Add( item );
                }
                AssetList.Save( File.Open( rulesListPath, FileMode.OpenOrCreate ), list );

                foreach (string DLLPath in vaildDLL)
                {
                    string copyPath = Path.Combine( rulesPath, Path.GetFileName( DLLPath ) );
                    if (!Path.GetFullPath( copyPath ).Equals( DLLPath ))
                    {
                        if (File.Exists( copyPath ))
                        {
                            DialogResult result = MessageBox.Show( Path.GetFileName( DLLPath ) + "已存在于" + rulesPath + "中。是否覆盖？", "文件已存在", MessageBoxButtons.YesNo );
                            if (result == DialogResult.Yes)
                            {
                                File.Copy( DLLPath, copyPath, true );
                            }
                        }
                        else
                            File.Copy( DLLPath, copyPath );
                    }
                }
                MessageBox.Show( "添加规则程序集成功！共添加" + vaildDLL.Count + "个有效程序集，" + countVaildClass + "个有效规则类", "成功" );
            }
            else
                MessageBox.Show( "未找到有效规则程序集！", "失败" );

            InitialRuleList();
        }

        private void AddRuleBtn_Click ( object sender, EventArgs e )
        {
            OpenRuleDLLFileDialog.ShowDialog();
        }

        #endregion

        private void DelRuleBtn_Click ( object sender, EventArgs e )
        {
            if (listViewRule.SelectedItems.Count == 0)
            {
                MessageBox.Show( "请在左边选择一个要删除的条目！", "提示窗口",
                MessageBoxButtons.OK, MessageBoxIcon.Information );
                return;
            }
            foreach (AssetItem aa in rulelist.list)
            {
                foreach (NameAndTypeName bb in aa.names)
                {
                    foreach (ListViewItem listitem in listViewRule.SelectedItems)
                    {
                        this.Text += bb.name;
                        if (aa.DLLName == listitem.SubItems[0].Text && bb.name == listitem.SubItems[1].Text
                              && bb.typeNames == listitem.SubItems[2].Text)
                        {
                            rulelist.list.Remove( aa );
                            File.Delete( rulesListPath );
                            AssetList.Save( File.Open( rulesListPath, FileMode.OpenOrCreate ), rulelist );
                            InitialRuleList();
                            return;
                        }
                    }
                }
            }

        }



        private void AddAIBtn_Click ( object sender, EventArgs e )
        {
            OpenAIDLLFileDialog.ShowDialog();
        }

        private void OpenAIDLLFileDialog_FileOk ( object sender, CancelEventArgs e )
        {
            List<AssetItem> AIList = new List<AssetItem>();

            List<string> vaildDLL = new List<string>();

            int countVaildClass = 0;

            foreach (string fileName in OpenAIDLLFileDialog.FileNames)
            {
                Assembly assembly = DIHelper.GetAssembly( fileName );

                AssetItem curItem;
                bool findGameAI = false;
                curItem = new AssetItem();
                curItem.DLLName = Path.GetFileName( fileName );
                List<NameAndTypeName> names = new List<NameAndTypeName>();
                try
                {
                    Directory.SetCurrentDirectory( Application.StartupPath );
                    AppDomain.CurrentDomain.AppendPrivatePath( "Rules" );
                    foreach (Type type in assembly.GetTypes())
                    {
                        foreach (Type infer in type.GetInterfaces())
                        {
                            if (infer.Name == "IAI")
                            {
                                object[] attributes = type.GetCustomAttributes( typeof( AIAttribute ), false );
                                foreach (object attri in attributes)
                                {
                                    if (attri is AIAttribute)
                                    {
                                        findGameAI = true;
                                        string AIName = ((AIAttribute)attri).Name;
                                        names.Add( new NameAndTypeName( AIName, type.FullName ) );
                                        countVaildClass++;
                                        break;
                                    }
                                }
                                if (findGameAI)
                                    break;
                            }
                        }
                    }
                    AppDomain.CurrentDomain.ClearPrivatePath();
                }
                catch (Exception ex)
                {
                    findGameAI = false;
                }
                if (findGameAI)
                {
                    curItem.names = names.ToArray();
                    AIList.Add( curItem );
                    vaildDLL.Add( fileName );
                }
            }

            if (AIList.Count != 0)
            {
                Directory.SetCurrentDirectory( Application.StartupPath );
                if (!Directory.Exists( AIPath ))
                {
                    Directory.CreateDirectory( AIPath );
                }
                AssetList list = AssetList.Load( File.Open( AIListPath, FileMode.OpenOrCreate ) );
                if (list == null)
                    list = new AssetList();


                foreach (AssetItem item in AIList)
                {
                    int index = list.IndexOf( item.DLLName );
                    if (index != -1)
                        list.list[index] = item;
                    else
                        list.list.Add( item );
                }
                AssetList.Save( File.Open( AIListPath, FileMode.OpenOrCreate ), list );

                foreach (string DLLPath in vaildDLL)
                {
                    string copyPath = Path.Combine( AIPath, Path.GetFileName( DLLPath ) );
                    if (!Path.GetFullPath( copyPath ).Equals( DLLPath ))
                    {
                        if (File.Exists( copyPath ))
                        {
                            DialogResult result = MessageBox.Show( Path.GetFileName( DLLPath ) + "已存在于" + rulesPath + "中。是否覆盖？", "文件已存在", MessageBoxButtons.YesNo );
                            if (result == DialogResult.Yes)
                            {
                                File.Copy( DLLPath, copyPath, true );
                            }
                        }
                        else
                            File.Copy( DLLPath, copyPath );
                    }
                }
                MessageBox.Show( "添加规则程序集成功！共添加" + vaildDLL.Count + "个有效程序集，" + countVaildClass + "个有效规则类", "成功" );
            }
            else
                MessageBox.Show( "未找到有效规则程序集！", "失败" );

            InitialAIList();
        }

        private void DelAIBtn_Click ( object sender, EventArgs e )
        {
            if (listViewAI.SelectedItems.Count == 0)
            {
                MessageBox.Show( "请在左边选择一个要删除的条目！", "提示窗口",
                MessageBoxButtons.OK, MessageBoxIcon.Information );
                return;
            }
            foreach (AssetItem aa in AIlist.list)
            {
                foreach (NameAndTypeName bb in aa.names)
                {
                    foreach (ListViewItem listitem in listViewAI.SelectedItems)
                    {
                        this.Text += bb.name;
                        if (aa.DLLName == listitem.SubItems[0].Text && bb.name == listitem.SubItems[1].Text
                              && bb.typeNames == listitem.SubItems[2].Text)
                        {
                            AIlist.list.Remove( aa );
                            File.Delete( AIListPath );
                            AssetList.Save( File.Open( AIListPath, FileMode.OpenOrCreate ), AIlist );
                            InitialAIList();
                            return;
                        }
                    }
                }
            }
        }


    }
}