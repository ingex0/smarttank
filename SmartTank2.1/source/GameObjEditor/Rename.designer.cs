namespace GameObjEditor
{
    partial class Rename
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
            this.acceptBtn = new System.Windows.Forms.Button();
            this.textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // acceptBtn
            // 
            this.acceptBtn.Location = new System.Drawing.Point( 214, 17 );
            this.acceptBtn.Name = "acceptBtn";
            this.acceptBtn.Size = new System.Drawing.Size( 58, 23 );
            this.acceptBtn.TabIndex = 0;
            this.acceptBtn.Text = "确定";
            this.acceptBtn.UseVisualStyleBackColor = true;
            this.acceptBtn.Click += new System.EventHandler( this.acceptBtn_Click );
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point( 12, 18 );
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size( 196, 21 );
            this.textBox.TabIndex = 1;
            // 
            // Rename
            // 
            this.AcceptButton = this.acceptBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 284, 52 );
            this.Controls.Add( this.textBox );
            this.Controls.Add( this.acceptBtn );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Rename";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rename";
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button acceptBtn;
        private System.Windows.Forms.TextBox textBox;
    }
}