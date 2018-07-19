using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ZXClient.control;
using ZXClient.dao;
using ZXClient.model;
using ZXClient.util;
using Newtonsoft.Json.Serialization;
using ZXClient.Properties;
using System.Runtime.InteropServices;

namespace ZXClient
{
    public partial class LoginWindow : Form
    {
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public LoginWindow()
        {
            InitializeComponent();
            SetForegroundWindow(this.Handle);//当到最前端
            if (MainData.autoLoginSucc==false)
            {
                MessageBox.Show("自动登录失败，请重新登录！");
            }
            this.notifyIcon1.Text = Resources.CLIENTNAME;
            this.TopMost = true;
            this.Focus();
            this.tbCard.Focus();
        }

        int waittimes = 6;//5秒后自动登录
        private delegate void DelegateName();
        System.Timers.Timer autoLoginTimer;
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private void AutoLogin()
        {
            string[] lastEmployee = db_EmployeeLoginDao.getLastAutoLogin();
            if (lastEmployee != null && lastEmployee[2] == "1") //如果最后一次登录的账号为自动登录
            {
                this.BeginInvoke(new Action(delegate () { cbMember.Checked = true; btnLogin.Enabled = false; tbCard.Text = lastEmployee[0]; }));

                IAsyncResult r = new DelegateName(new Action(delegate
                {
                    autoLoginTimer = new System.Timers.Timer();
                    autoLoginTimer.Interval = 1000;
                    autoLoginTimer.Elapsed += (sender, e) =>
                    {
                        if (waittimes > 0)
                        {
                            this.BeginInvoke(new Action(delegate () { _ShowInfo((waittimes-1) + "秒后自动登录，取消勾选则自动登录取消．", Color.Blue); waittimes--; }));
                        }
                        else
                        {
                            allDone.Set();
                        }
                    };
                    autoLoginTimer.Start();
                })).BeginInvoke(null,null);
                allDone.WaitOne();
                llgin(lastEmployee);
            }
            if (lastEmployee == null || lastEmployee[2] == "0" || MainData.autoLoginSucc != true) //没有自动登录的账户,弹出登录框, 或者自动登录没有成功，也弹出登录框
            {
                this.Invoke(new Action(delegate () { btnLogin.Enabled = true;if (lastEmployee != null && lastEmployee.Length>0) { tbCard.Text = lastEmployee[0]; } })); 
            }
        }

        private void llgin(string[] lastEmployee)
        {
            this.Invoke(new Action(delegate () { _ShowInfo("正在登录!", Color.Blue); }));
            MainData.USERCARD = lastEmployee[0];
            string loginRst = UserLogin(lastEmployee[0], lastEmployee[1], lastEmployee[2] == "1" ? 1 : 0);
            if (loginRst == MainData.loginSuccess)
            {
                MainData.autoLoginSucc = true;
                this.Invoke(new Action(delegate () { _ShowInfo("登录成功,正在跳转!", Color.Blue); }));
                Thread.Sleep(1000);
            }
            else
            {
                this.Invoke(new Action(delegate () { _ShowInfo("登录失败!", Color.Blue); }));
            }
            if (MainData.autoLoginSucc == true)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(delegate () {
                        this.Close();
                        this.notifyIcon1.Dispose();
                    }));
                }
                else
                {
                    this.Close();
                    this.notifyIcon1.Dispose();
                }
                WorkForm wf = new WorkForm();
                wf.Focus();
                wf.Activate();
                Application.Run(wf);
            }
        }

        /// <summary>
        /// 取消自动登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbMember_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbMember.Checked && autoLoginTimer != null)
            {
                autoLoginTimer.Enabled = false;
                autoLoginTimer.Close();
                this.BeginInvoke(new Action(delegate () { _ShowInfo("自动登录已取消．", Color.Blue); this.btnLogin.Enabled = true; waittimes--; }));
            }
        }

        /// <summary>
        /// 登录按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string card = tbCard.Text;
            string pw = tbPwd.Text;
            if (card=="")
            {
                lblErrMsg.ForeColor = Color.Red;
                lblErrMsg.Text = "用户名不能为空!";
                return;
            }
            if (pw == "")
            {
                lblErrMsg.ForeColor = Color.Red;
                lblErrMsg.Text = "密码不能为空!";
                return;
            }
            string isLogin = UserLogin(card, pw, cbMember.Checked ? 1 : 0);
            if (isLogin != MainData.loginSuccess)
            {
                lblErrMsg.ForeColor = Color.Red;
                lblErrMsg.Text = isLogin;
            }
            else
            {
                MainData.USERCARD = card;
                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pw"></param>
        internal static string UserLogin(string card, string pw, int isMember)
        {
            String ReturnDatastr = null;
            try
            {
                string data = String.Format("cardnum={0}&psw={1}", card, EDncryptHelper.MD5Encrypt16(pw));
                ReturnDatastr = HttpUtil.RequestData(MainData.ServerAddr + MainData.INTE_EMPLOYEELOGIN, data);
                if (ReturnDatastr==null)
                {
                    return "服务器连接失败!";
                }
            }
            catch(Exception ex)
            {
                LogHelper.WriteError(typeof(LoginWindow), ex);
                return "服务器连接失败!";
            }
            LogHelper.WriteInfo(typeof(LoginWindow), "登录结果：　" + ReturnDatastr);
            if (ReturnDatastr == MainData.loginSuccess)
            {
                MainData.USERCARD = card;
                if (db_EmployeeLoginDao.existsByCard(card)) //如果存在记录则修改
                {
                    db_EmployeeLoginDao.update(card, pw, isMember);
                }
                else
                {
                    db_EmployeeLoginDao.addIfNoExist(card, pw, isMember);
                }
                return ReturnDatastr;
            }
            else
            {
                return ReturnDatastr;
            }
        }
        
        private void _ShowInfo(String msg, Color foreColor)
        {
            this.lblErrMsg.Text = msg;
            this.lblErrMsg.ForeColor = foreColor;
        }
        
        private void LoginWindow_Load(object sender, EventArgs e)
        {
            CommonHelper.SetMid(this);
            Win32.AnimateWindow(this.Handle, 2000, Win32.AW_BLEND);
            contextMenuStrip1.Items.Add("参数配置");
            contextMenuStrip1.Items[0].Click += configSet_Click;

            contextMenuStrip1.Items.Add("退出");
            contextMenuStrip1.Items[1].Click += tmClose_Click;

            new Thread(AutoLogin).Start();
        }

        private void configSet_Click(object sender, EventArgs e)
        {
            ConfigSet setForm = new ConfigSet();
            setForm.StartPosition = FormStartPosition.Manual;
            int xWidth = SystemInformation.PrimaryMonitorSize.Width;//获取显示器屏幕宽度
            int yHeight = SystemInformation.PrimaryMonitorSize.Height;//高度
            setForm.Location = new Point(xWidth / 2, yHeight / 2);//这里需要再减去窗体本身的宽度和高度的一半
            setForm.ShowDialog();
            this.Hide();
        }

        /// <summary>
        /// 关闭程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmClose_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("确定要退出吗?", "退出系统", messButton);

            if (dr == DialogResult.OK)//如果点击“确定”按钮
            {
                this.notifyIcon1.Dispose();
                System.Environment.Exit(0);
            }
        }

        private void LoginWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Console.WriteLine("登录框关闭");
            this.notifyIcon1.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.notifyIcon1.Dispose();
            System.Environment.Exit(0);
        }

        private Point mPoint = new Point();

        private void LoginWindow_MouseDown(object sender, MouseEventArgs e)
        {
            mPoint.X = e.X;
            mPoint.Y = e.Y;
        }

        private void LoginWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point myPosittion = MousePosition;
                myPosittion.Offset(-mPoint.X, -mPoint.Y);
                Location = myPosittion;
            }
        }

        private void tbCard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                tbPwd.Focus();
        }

        private void tbPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnLogin_Click(sender, e);
        }
        
    }
}
