﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ZXClient
{
    public partial class VoiceText : ParentForm
    {
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public VoiceText()
        {
            InitializeComponent();
            SetForegroundWindow(this.Handle);//当到最前端
            this.TopMost = true;
            btnVoice.FlatStyle = cancel.FlatStyle = FlatStyle.Popup;
            txtContent.BorderStyle = BorderStyle.FixedSingle;
            
            this.Focus();
            txtContent.Focus();
        }
        
        public string Value { get; set; }

        //确定
        private void btnVoice_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Value = txtContent.Text;
            if (Value=="")
            {
                this.DialogResult = DialogResult.None;
                MessageBox.Show("播报内容不能为空！");
                return;
            }
            this.Close();
        }

        //取消
        private void cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnVoice_Click(sender, e);
            }
        }
    }
}
