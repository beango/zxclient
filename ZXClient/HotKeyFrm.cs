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
            Dictionary<string, string> keyDict = db_KeyConfig.getKeyConfig(MainStaticData.USERCARD);
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
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            List<string> d = new List<string>();
            if (tbWelKey.Text != "")
            {
                if (d.Contains(tbWelKey.Text))
                {
                    MessageBox.Show("按键设置重复!");
                    return;
                }
                else
                    d.Add(tbWelKey.Text);
            }
            if (tbPauseKey.Text != "")
            {
                if (d.Contains(tbPauseKey.Text))
                {
                    MessageBox.Show("按键设置重复!");
                    return;
                }
                else
                    d.Add(tbPauseKey.Text);
            }
            if (tbEvalKey.Text != "")
            {
                if (d.Contains(tbEvalKey.Text))
                {
                    MessageBox.Show("按键设置重复!");
                    return;
                }
                else
                    d.Add(tbEvalKey.Text);
            }
            if (tbVoiceKey.Text != "")
            {
                if (d.Contains(tbVoiceKey.Text))
                {
                    MessageBox.Show("按键设置重复!");
                    return;
                }
                else
                    d.Add(tbVoiceKey.Text);
            }

            if (db_KeyConfig.addIfNoExist(MainStaticData.USERCARD, "欢迎光临", tbWelKey.Text) == 0)
            {
                db_KeyConfig.update(tbWelKey.Text, MainStaticData.USERCARD, "欢迎光临");
            }
            if (db_KeyConfig.addIfNoExist(MainStaticData.USERCARD, "暂停服务", tbPauseKey.Text) == 0)
            {
                db_KeyConfig.update(tbPauseKey.Text, MainStaticData.USERCARD, "暂停服务");
            }
            if (db_KeyConfig.addIfNoExist(MainStaticData.USERCARD, "服务评价", tbEvalKey.Text) == 0)
            {
                db_KeyConfig.update(tbEvalKey.Text, MainStaticData.USERCARD, "服务评价");
            }
            if (db_KeyConfig.addIfNoExist(MainStaticData.USERCARD, "语音播报", tbVoiceKey.Text) == 0)
            {
                db_KeyConfig.update(tbVoiceKey.Text, MainStaticData.USERCARD, "语音播报");
            }
            MessageBox.Show("配置保存成功,将自动重启!");
            Application.ExitThread();
            MainStaticData.Restart();
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
            if (null != MainStaticData.wf)
                MainStaticData.wf.Visible = true;
        }
    }
}
