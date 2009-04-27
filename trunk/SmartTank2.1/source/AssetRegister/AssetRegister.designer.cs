namespace AssetRegister
{
    partial class AssetRegister
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
            this.AddRuleBtn = new System.Windows.Forms.Button();
            this.OpenRuleDLLFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.RulePage = new System.Windows.Forms.TabPage();
            this.DelRuleBtn = new System.Windows.Forms.Button();
            this.listViewRule = new System.Windows.Forms.ListView();
            this.DllName_ = new System.Windows.Forms.ColumnHeader();
            this.Name_ = new System.Windows.Forms.ColumnHeader();
            this.TypeName_ = new System.Windows.Forms.ColumnHeader();
            this.AIPage = new System.Windows.Forms.TabPage();
            this.AddAIBtn = new System.Windows.Forms.Button();
            this.OpenAIDLLFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.listViewAI = new System.Windows.Forms.ListView();
            this.AIDLL = new System.Windows.Forms.ColumnHeader();
            this.AIName = new System.Windows.Forms.ColumnHeader();
            this.TypeName = new System.Windows.Forms.ColumnHeader();
            this.DelAIBtn = new System.Windows.Forms.Button();
            this.TabControl.SuspendLayout();
            this.RulePage.SuspendLayout();
            this.AIPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // AddRuleBtn
            // 
            this.AddRuleBtn.Location = new System.Drawing.Point( 500, 63 );
            this.AddRuleBtn.Name = "AddRuleBtn";
            this.AddRuleBtn.Size = new System.Drawing.Size( 133, 23 );
            this.AddRuleBtn.TabIndex = 0;
            this.AddRuleBtn.Text = "注册新的规则程序集";
            this.AddRuleBtn.UseVisualStyleBackColor = true;
            this.AddRuleBtn.Click += new System.EventHandler( this.AddRuleBtn_Click );
            // 
            // OpenRuleDLLFileDialog
            // 
            this.OpenRuleDLLFileDialog.DefaultExt = "dll";
            this.OpenRuleDLLFileDialog.Filter = "DLL files (*.dll)|*.dll";
            this.OpenRuleDLLFileDialog.InitialDirectory = "Rules";
            this.OpenRuleDLLFileDialog.Multiselect = true;
            this.OpenRuleDLLFileDialog.RestoreDirectory = true;
            this.OpenRuleDLLFileDialog.FileOk += new System.ComponentModel.CancelEventHandler( this.OpenRuleDLLFileDialog_FileOk );
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add( this.RulePage );
            this.TabControl.Controls.Add( this.AIPage );
            this.TabControl.HotTrack = true;
            this.TabControl.Location = new System.Drawing.Point( 12, 12 );
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size( 647, 359 );
            this.TabControl.TabIndex = 1;
            // 
            // RulePage
            // 
            this.RulePage.Controls.Add( this.DelRuleBtn );
            this.RulePage.Controls.Add( this.listViewRule );
            this.RulePage.Controls.Add( this.AddRuleBtn );
            this.RulePage.Location = new System.Drawing.Point( 4, 21 );
            this.RulePage.Name = "RulePage";
            this.RulePage.Padding = new System.Windows.Forms.Padding( 3 );
            this.RulePage.Size = new System.Drawing.Size( 639, 334 );
            this.RulePage.TabIndex = 0;
            this.RulePage.Text = "RuleRegistration";
            this.RulePage.UseVisualStyleBackColor = true;
            // 
            // DelRuleBtn
            // 
            this.DelRuleBtn.Location = new System.Drawing.Point( 500, 101 );
            this.DelRuleBtn.Name = "DelRuleBtn";
            this.DelRuleBtn.Size = new System.Drawing.Size( 133, 23 );
            this.DelRuleBtn.TabIndex = 2;
            this.DelRuleBtn.Text = "删除规则程序集";
            this.DelRuleBtn.UseVisualStyleBackColor = true;
            this.DelRuleBtn.Click += new System.EventHandler( this.DelRuleBtn_Click );
            // 
            // listViewRule
            // 
            this.listViewRule.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.DllName_,
            this.Name_,
            this.TypeName_} );
            this.listViewRule.FullRowSelect = true;
            this.listViewRule.GridLines = true;
            this.listViewRule.Location = new System.Drawing.Point( 6, 6 );
            this.listViewRule.Name = "listViewRule";
            this.listViewRule.Size = new System.Drawing.Size( 488, 320 );
            this.listViewRule.TabIndex = 1;
            this.listViewRule.UseCompatibleStateImageBehavior = false;
            this.listViewRule.View = System.Windows.Forms.View.Details;
            // 
            // DllName_
            // 
            this.DllName_.Text = "RuleDllName";
            this.DllName_.Width = 168;
            // 
            // Name_
            // 
            this.Name_.Text = "RuleName";
            this.Name_.Width = 125;
            // 
            // TypeName_
            // 
            this.TypeName_.Text = "TypeName";
            this.TypeName_.Width = 157;
            // 
            // AIPage
            // 
            this.AIPage.Controls.Add( this.DelAIBtn );
            this.AIPage.Controls.Add( this.listViewAI );
            this.AIPage.Controls.Add( this.AddAIBtn );
            this.AIPage.Location = new System.Drawing.Point( 4, 21 );
            this.AIPage.Name = "AIPage";
            this.AIPage.Padding = new System.Windows.Forms.Padding( 3 );
            this.AIPage.Size = new System.Drawing.Size( 639, 334 );
            this.AIPage.TabIndex = 1;
            this.AIPage.Text = "AIRegistration";
            this.AIPage.UseVisualStyleBackColor = true;
            // 
            // AddAIBtn
            // 
            this.AddAIBtn.Location = new System.Drawing.Point( 509, 61 );
            this.AddAIBtn.Name = "AddAIBtn";
            this.AddAIBtn.Size = new System.Drawing.Size( 124, 23 );
            this.AddAIBtn.TabIndex = 0;
            this.AddAIBtn.Text = "添加新的AI程序集";
            this.AddAIBtn.UseVisualStyleBackColor = true;
            this.AddAIBtn.Click += new System.EventHandler( this.AddAIBtn_Click );
            // 
            // OpenAIDLLFileDialog
            // 
            this.OpenAIDLLFileDialog.Filter = "DLL files (*.dll)|*.dll";
            this.OpenAIDLLFileDialog.InitialDirectory = "AIs";
            this.OpenAIDLLFileDialog.RestoreDirectory = true;
            this.OpenAIDLLFileDialog.FileOk += new System.ComponentModel.CancelEventHandler( this.OpenAIDLLFileDialog_FileOk );
            // 
            // listViewAI
            // 
            this.listViewAI.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.AIDLL,
            this.AIName,
            this.TypeName} );
            this.listViewAI.FullRowSelect = true;
            this.listViewAI.Location = new System.Drawing.Point( 6, 6 );
            this.listViewAI.Name = "listViewAI";
            this.listViewAI.Size = new System.Drawing.Size( 497, 322 );
            this.listViewAI.TabIndex = 1;
            this.listViewAI.UseCompatibleStateImageBehavior = false;
            this.listViewAI.View = System.Windows.Forms.View.Details;
            // 
            // AIDLL
            // 
            this.AIDLL.Text = "DLLName";
            this.AIDLL.Width = 141;
            // 
            // AIName
            // 
            this.AIName.Text = "AIName";
            this.AIName.Width = 142;
            // 
            // TypeName
            // 
            this.TypeName.Text = "TypeName";
            this.TypeName.Width = 153;
            // 
            // DelAIBtn
            // 
            this.DelAIBtn.Location = new System.Drawing.Point( 509, 100 );
            this.DelAIBtn.Name = "DelAIBtn";
            this.DelAIBtn.Size = new System.Drawing.Size( 124, 23 );
            this.DelAIBtn.TabIndex = 2;
            this.DelAIBtn.Text = "删除AI程序集";
            this.DelAIBtn.UseVisualStyleBackColor = true;
            this.DelAIBtn.Click += new System.EventHandler( this.DelAIBtn_Click );
            // 
            // AssetRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 670, 383 );
            this.Controls.Add( this.TabControl );
            this.MaximizeBox = false;
            this.Name = "AssetRegister";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AssetRegister";
            this.TabControl.ResumeLayout( false );
            this.RulePage.ResumeLayout( false );
            this.AIPage.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.Button AddRuleBtn;
        private System.Windows.Forms.OpenFileDialog OpenRuleDLLFileDialog;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage AIPage;
        private System.Windows.Forms.TabPage RulePage;
        private System.Windows.Forms.ListView listViewRule;
        private System.Windows.Forms.ColumnHeader DllName_;
        private System.Windows.Forms.ColumnHeader Name_;
        private System.Windows.Forms.ColumnHeader TypeName_;
        private System.Windows.Forms.Button DelRuleBtn;
        private System.Windows.Forms.Button AddAIBtn;
        private System.Windows.Forms.OpenFileDialog OpenAIDLLFileDialog;
        private System.Windows.Forms.ListView listViewAI;
        private System.Windows.Forms.ColumnHeader AIDLL;
        private System.Windows.Forms.ColumnHeader AIName;
        private System.Windows.Forms.ColumnHeader TypeName;
        private System.Windows.Forms.Button DelAIBtn;
    }
}

