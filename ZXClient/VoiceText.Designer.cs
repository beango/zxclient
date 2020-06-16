namespace ZXClient
{
    partial class VoiceText
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VoiceText));
            this.txtContent = new System.Windows.Forms.TextBox();
            this.btnVoice = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtContent
            // 
            this.txtContent.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtContent.Location = new System.Drawing.Point(45, 2);
            this.txtContent.Margin = new System.Windows.Forms.Padding(6);
            this.txtContent.MaxLength = 100;
            this.txtContent.Name = "txtContent";
            this.txtContent.Size = new System.Drawing.Size(365, 29);
            this.txtContent.TabIndex = 0;
            this.txtContent.WordWrap = false;
            this.txtContent.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtContent_KeyDown);
            // 
            // btnVoice
            // 
            this.btnVoice.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnVoice.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnVoice.Font = new System.Drawing.Font("宋体", 9F);
            this.btnVoice.Location = new System.Drawing.Point(419, 2);
            this.btnVoice.Name = "btnVoice";
            this.btnVoice.Size = new System.Drawing.Size(42, 29);
            this.btnVoice.TabIndex = 1;
            this.btnVoice.Text = "确定";
            this.btnVoice.UseVisualStyleBackColor = true;
            this.btnVoice.Click += new System.EventHandler(this.btnVoice_Click);
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cancel.Location = new System.Drawing.Point(467, 2);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(43, 29);
            this.cancel.TabIndex = 2;
            this.cancel.Text = "取消";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F);
            this.label1.Location = new System.Drawing.Point(1, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "内容:";
            // 
            // VoiceText
            // 
            this.AcceptButton = this.btnVoice;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleTurquoise;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(517, 43);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.btnVoice);
            this.Controls.Add(this.txtContent);
            this.Font = new System.Drawing.Font("宋体", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "VoiceText";
            this.Text = "请输入播报内容";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Button btnVoice;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label label1;
    }
}