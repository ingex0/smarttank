namespace GameObjEditor
{
    partial class EnterYourName
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Enter = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 29, 40 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 41, 12 );
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point( 31, 93 );
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size( 191, 21 );
            this.textBox1.TabIndex = 1;
            // 
            // Enter
            // 
            this.Enter.Location = new System.Drawing.Point( 143, 144 );
            this.Enter.Name = "Enter";
            this.Enter.Size = new System.Drawing.Size( 79, 23 );
            this.Enter.TabIndex = 2;
            this.Enter.Text = "确定";
            this.Enter.UseVisualStyleBackColor = true;
            this.Enter.Click += new System.EventHandler( this.Enter_Click );
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point( 228, 144 );
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size( 75, 23 );
            this.Cancel.TabIndex = 3;
            this.Cancel.Text = "取消";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler( this.Cancel_Click );
            // 
            // EnterYourName
            // 
            this.AcceptButton = this.Enter;
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size( 334, 179 );
            this.Controls.Add( this.Cancel );
            this.Controls.Add( this.Enter );
            this.Controls.Add( this.textBox1 );
            this.Controls.Add( this.label1 );
            this.Name = "EnterYourName";
            this.Text = "EnterYourName";
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button Enter;
        private System.Windows.Forms.Button Cancel;
    }
}