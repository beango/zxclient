using System;
using System.IO;
using System.Windows.Forms;
using ZXClient.util;
using ZXClient.model;
using ZXClient.dao;
using System.Drawing;

namespace ZXClient
{
    public partial class ConfigSet : Form
    {
        public ConfigSet()
        {
            InitializeComponent();

            this.TopMost = true;
            CommonHelper.SetMid(this);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            cbConnType.Items.AddRange(MainStaticData.ConnTypeData);
            tbServerIP.Text = MainStaticData.ServerIP;
            cbConnType.SelectedText = MainStaticData.ConnType;
            tbDeviceIP.Text = MainStaticData.DeviceIP;
            tbServerPort.Text = MainStaticData.ServerPort;
            cbNoLogin.Checked = MainStaticData.cbNoLogin;
        }
        
        private static byte[] ReadFile(String img)
        {
            FileInfo fileinfo = new FileInfo(img);
            byte[] buf = new byte[fileinfo.Length];
            FileStream fs = new FileStream(img, FileMode.Open, FileAccess.Read);
            fs.Read(buf, 0, buf.Length);
            fs.Close();
            GC.ReRegisterForFinalize(fileinfo);
            GC.ReRegisterForFinalize(fs);
            return buf;
        }
        
        private delegate void returnStrDelegate(string str);

        private void ShowInfo2(string strInfo)
        {
            returnCB(_ShowInfo, strInfo);
        }

        private void returnCB(returnStrDelegate myDelegate, string strInfo)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(myDelegate, strInfo);
            }
            else
            {
                myDelegate(strInfo);
            }
        }
        
        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if(tbServerIP.Text == "")
            {
                MessageBox.Show("服务器IP不能为空!");
                return;
            }
            if (tbDeviceIP.Text == "" && cbConnType.Text == MainStaticData.ConnTypeData[0]) //网络连接
            {
                MessageBox.Show("评价器IP不能为空!");
                return;
            }
            db_ConfigDao.updateByKey("ServerAddr", String.Format("http://{0}:8080/", tbServerIP.Text));
            db_ConfigDao.updateByKey("ServerIP", tbServerIP.Text);
            db_ConfigDao.updateByKey("DeviceIP", tbDeviceIP.Text);
            db_ConfigDao.updateByKey("ConnType", cbConnType.Text); 
            db_ConfigDao.updateByKey("ServerPort", tbServerPort.Text);
            db_ConfigDao.updateByKey("isNoLogin", cbNoLogin.Checked);
            
            MessageBox.Show("配置保存成功,将自动重启!");
            MainStaticData.Restart();
        }

        private void cbConnType_TextChanged(object sender, EventArgs e)
        {
            if (cbConnType.Text == MainStaticData.ConnTypeData[0])
            {
                tbDeviceIP.Visible = lblDevice.Visible = true;
                ShowInfo2("切换到网络连接方式！");
            }
            else
            {
                tbDeviceIP.Visible = lblDevice.Visible = false;
                _ShowInfo("切换到USB连接方式！");
            }
        }

        /// <summary>
        /// 显示日志
        /// </summary>
        /// <param name="info"></param>
        private void _ShowInfo(string info)
        {
            Console.WriteLine(string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + "  " + info);
        }
       
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            SolidBrush back = new SolidBrush(Color.FromArgb(45, 45, 48));
            SolidBrush white = new SolidBrush(Color.FromArgb(122, 193, 255));
            Rectangle rec = tabControl1.GetTabRect(0);
            e.Graphics.FillRectangle(back, rec);
            Rectangle rec1 = tabControl1.GetTabRect(1);
            e.Graphics.FillRectangle(back, rec1);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                Rectangle rec2 = tabControl1.GetTabRect(i);
                e.Graphics.DrawString(tabControl1.TabPages[i].Text, new Font("微软雅黑", 9), white, rec2, sf);
            }
        }
        
        private void ConfigSet_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(null!= MainStaticData.wf)
                MainStaticData.wf.Visible = true;
        }
    }
}
