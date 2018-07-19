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
            this.tbWelKey = new ZXClient.control.TextBoxXP();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(96, 165);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 36);
            this.button1.TabIndex = 34;
            this.button1.Text = "保存";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbVoiceKey
            // 
            this.tbVoiceKey.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbVoiceKey.Location = new System.Drawing.Point(96, 123);
            this.tbVoiceKey.Name = "tbVoiceKey";
            this.tbVoiceKey.Size = new System.Drawing.Size(100, 26);
            this.tbVoiceKey.TabIndex = 33;
            this.tbVoiceKey.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbVoiceKey_KeyUp);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 133);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 32;
            this.label6.Text = "语音播报:";
            // 
            // tbEvalKey
            // 
            this.tbEvalKey.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbEvalKey.Location = new System.Drawing.Point(96, 86);
            this.tbEvalKey.Name = "tbEvalKey";
            this.tbEvalKey.Size = new System.Drawing.Size(100, 26);
            this.tbEvalKey.TabIndex = 31;
            this.tbEvalKey.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbEvalKey_KeyUp);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 96);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 30;
            this.label5.Text = "服务评价:";
            // 
            // tbPauseKey
            // 
            this.tbPauseKey.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbPauseKey.Location = new System.Drawing.Point(96, 49);
            this.tbPauseKey.Name = "tbPauseKey";
            this.tbPauseKey.Size = new System.Drawing.Size(100, 26);
            this.tbPauseKey.TabIndex = 29;
            this.tbPauseKey.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbPauseKey_KeyUp);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 28;
            this.label4.Text = "暂停服务:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 26;
            this.label1.Text = "欢迎光临:";
            // 
            // tbWelKey
            // 
            this.tbWelKey.Font = new System.Drawing.Font("宋体", 12F);
            this.tbWelKey.Location = new System.Drawing.Point(96, 11);
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
            this.ClientSize = new System.Drawing.Size(308, 226);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbVoiceKey);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbEvalKey);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbPauseKey);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbWelKey);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HotKeyFrm";
            this.Text = "设置快捷键";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HotKeyFrm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}