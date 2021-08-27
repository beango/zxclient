namespace ZXClient
{
    partial class LoginWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginWindow));
            this.tbCard = new System.Windows.Forms.TextBox();
            this.tbPwd = new System.Windows.Forms.TextBox();
            this.cbMember = new System.Windows.Forms.CheckBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lblErrMsg = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnLogin)).BeginInit();
            this.SuspendLayout();
            // 
            // tbCard
            // 
            this.tbCard.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCard.Font = new System.Drawing.Font("SimSun", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbCard.Location = new System.Drawing.Point(163, 148);
            this.tbCard.Margin = new System.Windows.Forms.Padding(2);
            this.tbCard.Name = "tbCard";
            this.tbCard.Size = new System.Drawing.Size(182, 23);
            this.tbCard.TabIndex = 1;
            this.tbCard.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbCard_KeyDown);
            // 
            // tbPwd
            // 
            this.tbPwd.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbPwd.Font = new System.Drawing.Font("SimSun", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbPwd.Location = new System.Drawing.Point(164, 182);
            this.tbPwd.Margin = new System.Windows.Forms.Padding(2);
            this.tbPwd.Name = "tbPwd";
            this.tbPwd.PasswordChar = '*';
            this.tbPwd.Size = new System.Drawing.Size(182, 23);
            this.tbPwd.TabIndex = 3;
            this.tbPwd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPwd_KeyDown);
            // 
            // cbMember
            // 
            this.cbMember.AutoSize = true;
            this.cbMember.BackColor = System.Drawing.Color.Transparent;
            this.cbMember.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.cbMember.Font = new System.Drawing.Font("SimSun", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbMember.Location = new System.Drawing.Point(130, 248);
            this.cbMember.Margin = new System.Windows.Forms.Padding(2);
            this.cbMember.Name = "cbMember";
            this.cbMember.Size = new System.Drawing.Size(82, 18);
            this.cbMember.TabIndex = 5;
            this.cbMember.Text = "自动登录";
            this.cbMember.UseVisualStyleBackColor = false;
            this.cbMember.CheckedChanged += new System.EventHandler(this.cbMember_CheckedChanged);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // lblErrMsg
            // 
            this.lblErrMsg.AutoSize = true;
            this.lblErrMsg.BackColor = System.Drawing.Color.Transparent;
            this.lblErrMsg.ForeColor = System.Drawing.Color.Crimson;
            this.lblErrMsg.Location = new System.Drawing.Point(217, 252);
            this.lblErrMsg.Name = "lblErrMsg";
            this.lblErrMsg.Size = new System.Drawing.Size(0, 12);
            this.lblErrMsg.TabIndex = 7;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(428, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(30, 30);
            this.button2.TabIndex = 8;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnLogin.BackgroundImage")));
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.Location = new System.Drawing.Point(129, 216);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(216, 27);
            this.btnLogin.TabIndex = 9;
            this.btnLogin.TabStop = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // LoginWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleTurquoise;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(465, 313);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.lblErrMsg);
            this.Controls.Add(this.cbMember);
            this.Controls.Add(this.tbPwd);
            this.Controls.Add(this.tbCard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "LoginWindow";
            this.Text = "登录";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LoginWindow_FormClosed);
            this.Load += new System.EventHandler(this.LoginWindow_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginWindow_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginWindow_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.btnLogin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tbCard;
        private System.Windows.Forms.TextBox tbPwd;
        private System.Windows.Forms.CheckBox cbMember;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        public System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label lblErrMsg;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox btnLogin;
    }
}