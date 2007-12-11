namespace MapEditer
{
    partial class ObjDisplayPanel
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
            this.canvas = new DeviceCanvas.DeviceCanvas();
            this.SuspendLayout();
            // 
            // canvas
            // 
            this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvas.Location = new System.Drawing.Point( 0, 0 );
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size( 253, 267 );
            this.canvas.TabIndex = 0;
            this.canvas.Text = "deviceCanvas1";
            // 
            // ObjDisplayPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 253, 267 );
            this.Controls.Add( this.canvas );
            this.Name = "ObjDisplayPanel";
            this.TabText = "ObjDisplayPanel";
            this.Text = "ObjDisplayPanel";
            this.ResumeLayout( false );

        }

        #endregion

        public DeviceCanvas.DeviceCanvas canvas;


    }
}