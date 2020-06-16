namespace ZXClient
{
    partial class HotKeyFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HotKeyFrm));
            this.button1 = new System.Windows.Forms.Button();
            this.tbVoiceKey = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbEvalKey = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbPauseKey = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.txtCutVideo = new System.Windows.Forms.TextBox();
            this.lblCutVideo = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.txtCutImg = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbWelKey = new ZXClient.control.TextBoxXP();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 27);
            this.button1.TabIndex = 34;
            this.button1.Text = "保存";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbVoiceKey
            // 
            this.tbVoiceKey.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbVoiceKey.Location = new System.Drawing.Point(70, 4);
            this.tbVoiceKey.Name = "tbVoiceKey";
            this.tbVoiceKey.Size = new System.Drawing.Size(100, 26);
            this.tbVoiceKey.TabIndex = 33;
            this.tbVoiceKey.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbWelKey_KeyPress);
            this.tbVoiceKey.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbVoiceKey_KeyUp);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 32;
            this.label6.Text = "语音播报:";
            // 
            // tbEvalKey
            // 
            this.tbEvalKey.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbEvalKey.Location = new System.Drawing.Point(70, 4);
            this.tbEvalKey.Name = "tbEvalKey";
            this.tbEvalKey.Size = new System.Drawing.Size(100, 26);
            this.tbEvalKey.TabIndex = 31;
            this.tbEvalKey.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbWelKey_KeyPress);
            this.tbEvalKey.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbEvalKey_KeyUp);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 30;
            this.label5.Text = "服务评价:";
            // 
            // tbPauseKey
            // 
            this.tbPauseKey.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbPauseKey.Location = new System.Drawing.Point(70, 2);
            this.tbPauseKey.Name = "tbPauseKey";
            this.tbPauseKey.Size = new System.Drawing.Size(100, 26);
            this.tbPauseKey.TabIndex = 29;
            this.tbPauseKey.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbWelKey_KeyPress);
            this.tbPauseKey.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbPauseKey_KeyUp);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 28;
            this.label4.Text = "暂停服务:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 26;
            this.label1.Text = "欢迎光临:";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Controls.Add(this.panel2);
            this.flowLayoutPanel1.Controls.Add(this.panel3);
            this.flowLayoutPanel1.Controls.Add(this.panel4);
            this.flowLayoutPanel1.Controls.Add(this.panel7);
            this.flowLayoutPanel1.Controls.Add(this.panel6);
            this.flowLayoutPanel1.Controls.Add(this.panel5);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(308, 276);
            this.flowLayoutPanel1.TabIndex = 35;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.tbWelKey);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(173, 36);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.tbPauseKey);
            this.panel2.Location = new System.Drawing.Point(3, 45);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(173, 31);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.Controls.Add(this.tbEvalKey);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Location = new System.Drawing.Point(3, 82);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(173, 33);
            this.panel3.TabIndex = 2;
            // 
            // panel4
            // 
            this.panel4.AutoSize = true;
            this.panel4.Controls.Add(this.tbVoiceKey);
            this.panel4.Controls.Add(this.label6);
            this.panel4.Location = new System.Drawing.Point(3, 121);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(173, 33);
            this.panel4.TabIndex = 3;
            // 
            // panel6
            // 
            this.panel6.AutoSize = true;
            this.panel6.Controls.Add(this.txtCutVideo);
            this.panel6.Controls.Add(this.lblCutVideo);
            this.panel6.Location = new System.Drawing.Point(3, 198);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(173, 32);
            this.panel6.TabIndex = 5;
            // 
            // txtCutVideo
            // 
            this.txtCutVideo.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtCutVideo.Location = new System.Drawing.Point(70, 3);
            this.txtCutVideo.Name = "txtCutVideo";
            this.txtCutVideo.Size = new System.Drawing.Size(100, 26);
            this.txtCutVideo.TabIndex = 35;
            this.txtCutVideo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbWelKey_KeyPress);
            this.txtCutVideo.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtCutVideo_KeyUp);
            // 
            // lblCutVideo
            // 
            this.lblCutVideo.AutoSize = true;
            this.lblCutVideo.Location = new System.Drawing.Point(18, 10);
            this.lblCutVideo.Name = "lblCutVideo";
            this.lblCutVideo.Size = new System.Drawing.Size(35, 12);
            this.lblCutVideo.TabIndex = 34;
            this.lblCutVideo.Text = "同屏:";
            // 
            // panel5
            // 
            this.panel5.AutoSize = true;
            this.panel5.Controls.Add(this.button1);
            this.panel5.Location = new System.Drawing.Point(3, 236);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(90, 33);
            this.panel5.TabIndex = 4;
            // 
            // panel7
            // 
            this.panel7.AutoSize = true;
            this.panel7.Controls.Add(this.txtCutImg);
            this.panel7.Controls.Add(this.label2);
            this.panel7.Location = new System.Drawing.Point(3, 160);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(173, 32);
            this.panel7.TabIndex = 6;
            // 
            // txtCutImg
            // 
            this.txtCutImg.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtCutImg.Location = new System.Drawing.Point(70, 3);
            this.txtCutImg.Name = "txtCutImg";
            this.txtCutImg.Size = new System.Drawing.Size(100, 26);
            this.txtCutImg.TabIndex = 35;
            this.txtCutImg.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbWelKey_KeyPress);
            this.txtCutImg.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtCutImg_KeyUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 34;
            this.label2.Text = "截屏:";
            // 
            // tbWelKey
            // 
            this.tbWelKey.Font = new System.Drawing.Font("宋体", 12F);
            this.tbWelKey.Location = new System.Drawing.Point(70, 7);
            this.tbWelKey.Name = "tbWelKey";
            this.tbWelKey.Size = new System.Drawing.Size(100, 26);
            this.tbWelKey.TabIndex = 27;
            this.tbWelKey.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbWelKey_KeyPress);
            this.tbWelKey.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbWelKey_KeyUp);
            // 
            // HotKeyFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 276);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HotKeyFrm";
            this.Text = "设置快捷键";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HotKeyFrm_FormClosed);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbVoiceKey;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbEvalKey;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbPauseKey;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private control.TextBoxXP tbWelKey;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox txtCutVideo;
        private System.Windows.Forms.Label lblCutVideo;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.TextBox txtCutImg;
        private System.Windows.Forms.Label label2;
    }
}