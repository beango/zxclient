using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZXClient.dao;
using ZXClient.model;
using ZXClient.util;

namespace ZXClient
{
    public partial class HotKeyFrm : Form
    {
        public HotKeyFrm()
        {
            InitializeComponent();
            CommonHelper.SetMid(this);

            //lblCutVideo.Visible = txtCutVideo.Visible = !MainData.isNetwork;

            Dictionary<string, string> keyDict = db_KeyConfig.getKeyConfig(MainData.USERCARD);
            if (keyDict != null && keyDict.ContainsKey("欢迎光临"))
            {
                tbWelKey.Text = keyDict["欢迎光临"];
            }
            if (keyDict != null && keyDict.ContainsKey("暂停服务"))
            {
                tbPauseKey.Text = keyDict["暂停服务"];
            }
            if (keyDict != null && keyDict.ContainsKey("服务评价"))
            {
                tbEvalKey.Text = keyDict["服务评价"];
            }
            if (keyDict != null && keyDict.ContainsKey("语音播报"))
            {
                tbVoiceKey.Text = keyDict["语音播报"];
            }
            if (keyDict != null && keyDict.ContainsKey("截屏"))
            {
                txtCutImg.Text = keyDict["截屏"];
            }
            if (keyDict != null && keyDict.ContainsKey("同屏"))
            {
                txtCutVideo.Text = keyDict["同屏"];
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            List<string> d = new List<string>();
            if (tbWelKey.Text != "")
            {
                if (d.Contains(tbWelKey.Text))
                {
                    MessageBox.Show("欢迎光临按键设置重复!");
                    return;
                }
                else
                    d.Add(tbWelKey.Text);
            }
            if (tbPauseKey.Text != "")
            {
                if (d.Contains(tbPauseKey.Text))
                {
                    MessageBox.Show("暂停按键设置重复!");
                    return;
                }
                else
                    d.Add(tbPauseKey.Text);
            }
            if (tbEvalKey.Text != "")
            {
                if (d.Contains(tbEvalKey.Text))
                {
                    MessageBox.Show("评价按键设置重复!");
                    return;
                }
                else
                    d.Add(tbEvalKey.Text);
            }
            if (tbVoiceKey.Text != "")
            {
                if (d.Contains(tbVoiceKey.Text))
                {
                    MessageBox.Show("语音按键设置重复!");
                    return;
                }
                else
                    d.Add(tbVoiceKey.Text);
            }
            if (txtCutVideo.Text != "")
            {
                if (d.Contains(txtCutVideo.Text))
                {
                    MessageBox.Show("同屏按键设置重复!");
                    return;
                }
                else
                    d.Add(txtCutVideo.Text);
            }
            if (txtCutImg.Text != "")
            {
                if (d.Contains(txtCutImg.Text))
                {
                    MessageBox.Show("截图按键设置重复!");
                    return;
                }
                else
                    d.Add(txtCutImg.Text);
            }
            if (db_KeyConfig.addIfNoExist(MainData.USERCARD, "欢迎光临", tbWelKey.Text) == 0)
            {
                db_KeyConfig.update(tbWelKey.Text, MainData.USERCARD, "欢迎光临");
            }
            if (db_KeyConfig.addIfNoExist(MainData.USERCARD, "暂停服务", tbPauseKey.Text) == 0)
            {
                db_KeyConfig.update(tbPauseKey.Text, MainData.USERCARD, "暂停服务");
            }
            if (db_KeyConfig.addIfNoExist(MainData.USERCARD, "服务评价", tbEvalKey.Text) == 0)
            {
                db_KeyConfig.update(tbEvalKey.Text, MainData.USERCARD, "服务评价");
            }
            if (db_KeyConfig.addIfNoExist(MainData.USERCARD, "语音播报", tbVoiceKey.Text) == 0)
            {
                db_KeyConfig.update(tbVoiceKey.Text, MainData.USERCARD, "语音播报");
            }
            if (db_KeyConfig.addIfNoExist(MainData.USERCARD, "同屏", txtCutVideo.Text) == 0)
            {
                db_KeyConfig.update(txtCutVideo.Text, MainData.USERCARD, "同屏");
            }
            if (db_KeyConfig.addIfNoExist(MainData.USERCARD, "截屏", txtCutImg.Text) == 0)
            {
                db_KeyConfig.update(txtCutImg.Text, MainData.USERCARD, "截屏");
            }
            MessageBox.Show("配置保存成功,将自动重启!");
            Application.ExitThread();
            MainData.Restart();
        }

        private void tbWelKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers.CompareTo(Keys.Control) >= 0 && e.KeyCode != Keys.ControlKey)//Ctrl=0, Alt=1
            {
                tbWelKey.Text = HotKey.GetStringByKey(e);
            }
        }

        private void tbWelKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'a' && e.KeyChar <= 'z') || (e.KeyChar >= 'A' && e.KeyChar <= 'Z')
                || (e.KeyChar >= '0' && e.KeyChar <= '9'))
            {
                e.Handled = true;
            }
        }

        private void tbPauseKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers.CompareTo(Keys.Control) >= 0 && e.KeyCode != Keys.ControlKey)//Ctrl=0, Alt=1
            {
                tbPauseKey.Text = HotKey.GetStringByKey(e);
            }
        }

        private void tbEvalKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers.CompareTo(Keys.Control) >= 0 && e.KeyCode != Keys.ControlKey)//Ctrl=0, Alt=1
            {
                tbEvalKey.Text = HotKey.GetStringByKey(e);
            }
        }

        private void tbVoiceKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers.CompareTo(Keys.Control) >= 0 && e.KeyCode != Keys.ControlKey)
            {
                tbVoiceKey.Text = HotKey.GetStringByKey(e);
            }
        }

        private void HotKeyFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (null != MainData.wf)
                MainData.wf.Visible = true;
        }

        private void txtCutVideo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers.CompareTo(Keys.Control) >= 0 && e.KeyCode != Keys.ControlKey)
            {
                txtCutVideo.Text = HotKey.GetStringByKey(e);
            }
        }

        private void txtCutImg_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers.CompareTo(Keys.Control) >= 0 && e.KeyCode != Keys.ControlKey)
            {
                txtCutImg.Text = HotKey.GetStringByKey(e);
            }
        }
    }
}
