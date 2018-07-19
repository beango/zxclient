using ZXClient.control;

namespace ZXClient
{
    partial class ConfigSet
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigSet));
            this.tabControl1 = new ZXClient.control.CustomTabControl();
            this.tpConfig = new System.Windows.Forms.TabPage();
            this.tbServerPort = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblDevice = new System.Windows.Forms.Label();
            this.cbConnType = new System.Windows.Forms.ComboBox();
            this.tbDeviceIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbServerIP = new System.Windows.Forms.TextBox();
            this.lblNoLogin = new System.Windows.Forms.Label();
            this.cbNoLogin = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tpConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpConfig);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.ItemSize = new System.Drawing.Size(0, 16);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(9, 0);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(385, 271);
            this.tabControl1.TabIndex = 19;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
            // 
            // tpConfig
            // 
            this.tpConfig.Controls.Add(this.cbNoLogin);
            this.tpConfig.Controls.Add(this.lblNoLogin);
            this.tpConfig.Controls.Add(this.tbServerPort);
            this.tpConfig.Controls.Add(this.label7);
            this.tpConfig.Controls.Add(this.btnSave);
            this.tpConfig.Controls.Add(this.lblDevice);
            this.tpConfig.Controls.Add(this.cbConnType);
            this.tpConfig.Controls.Add(this.tbDeviceIP);
            this.tpConfig.Controls.Add(this.label3);
            this.tpConfig.Controls.Add(this.label2);
            this.tpConfig.Controls.Add(this.tbServerIP);
            this.tpConfig.Location = new System.Drawing.Point(4, 20);
            this.tpConfig.Name = "tpConfig";
            this.tpConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tpConfig.Size = new System.Drawing.Size(377, 247);
            this.tpConfig.TabIndex = 0;
            this.tpConfig.Text = "参数配置";
            this.tpConfig.UseVisualStyleBackColor = true;
            // 
            // tbServerPort
            // 
            this.tbServerPort.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbServerPort.Location = new System.Drawing.Point(85, 47);
            this.tbServerPort.Name = "tbServerPort";
            this.tbServerPort.Size = new System.Drawing.Size(100, 26);
            this.tbServerPort.TabIndex = 18;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 12);
            this.label7.TabIndex = 17;
            this.label7.Text = "服务器端口:";
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSave.Location = new System.Drawing.Point(85, 195);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 35);
            this.btnSave.TabIndex = 16;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblDevice
            // 
            this.lblDevice.AutoSize = true;
            this.lblDevice.Location = new System.Drawing.Point(4, 119);
            this.lblDevice.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDevice.Name = "lblDevice";
            this.lblDevice.Size = new System.Drawing.Size(65, 12);
            this.lblDevice.TabIndex = 4;
            this.lblDevice.Text = "评价器IP：";
            // 
            // cbConnType
            // 
            this.cbConnType.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbConnType.FormattingEnabled = true;
            this.cbConnType.Location = new System.Drawing.Point(85, 80);
            this.cbConnType.Margin = new System.Windows.Forms.Padding(2);
            this.cbConnType.Name = "cbConnType";
            this.cbConnType.Size = new System.Drawing.Size(92, 24);
            this.cbConnType.TabIndex = 12;
            this.cbConnType.TextChanged += new System.EventHandler(this.cbConnType_TextChanged);
            // 
            // tbDeviceIP
            // 
            this.tbDeviceIP.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbDeviceIP.Location = new System.Drawing.Point(85, 112);
            this.tbDeviceIP.Margin = new System.Windows.Forms.Padding(2);
            this.tbDeviceIP.Name = "tbDeviceIP";
            this.tbDeviceIP.Size = new System.Drawing.Size(165, 26);
            this.tbDeviceIP.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 87);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "连接方式：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 22);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "服务器IP：";
            // 
            // tbServerIP
            // 
            this.tbServerIP.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbServerIP.Location = new System.Drawing.Point(85, 15);
            this.tbServerIP.Margin = new System.Windows.Forms.Padding(2);
            this.tbServerIP.Name = "tbServerIP";
            this.tbServerIP.Size = new System.Drawing.Size(165, 26);
            this.tbServerIP.TabIndex = 10;
            // 
            // lblNoLogin
            // 
            this.lblNoLogin.AutoSize = true;
            this.lblNoLogin.Location = new System.Drawing.Point(4, 152);
            this.lblNoLogin.Name = "lblNoLogin";
            this.lblNoLogin.Size = new System.Drawing.Size(77, 12);
            this.lblNoLogin.TabIndex = 19;
            this.lblNoLogin.Text = "是否免登录：";
            // 
            // cbNoLogin
            // 
            this.cbNoLogin.AutoSize = true;
            this.cbNoLogin.Location = new System.Drawing.Point(88, 152);
            this.cbNoLogin.Name = "cbNoLogin";
            this.cbNoLogin.Size = new System.Drawing.Size(36, 16);
            this.cbNoLogin.TabIndex = 20;
            this.cbNoLogin.Text = "是";
            this.cbNoLogin.UseVisualStyleBackColor = true;
            // 
            // ConfigSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleTurquoise;
            this.ClientSize = new System.Drawing.Size(385, 271);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ConfigSet";
            this.Text = "参数配置";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConfigSet_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tpConfig.ResumeLayout(false);
            this.tpConfig.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblDevice;
        private System.Windows.Forms.TextBox tbDeviceIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbServerIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbConnType;
        private System.Windows.Forms.Button btnSave;
        private CustomTabControl tabControl1;
        private System.Windows.Forms.TabPage tpConfig;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbServerPort;
        private System.Windows.Forms.Label lblNoLogin;
        private System.Windows.Forms.CheckBox cbNoLogin;
    }
}

