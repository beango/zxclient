using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Serialization;
using ZXClient.control;
using ZXClient.dao;
using ZXClient.model;
using ZXClient.Properties;
using ZXClient.util;

namespace ZXClient
{
    public partial class WorkForm : ParentForm
    {
        private Type TAG = typeof(WorkForm);
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Button btnCut2;
        private System.Windows.Forms.Button btnCut;
        private System.Windows.Forms.Button btnVoice;
        private System.Windows.Forms.Button btnEval;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnWel;
        SynchronizationContext m_SyncContext = null;
        Object locker = new Object();
        ADBClient MyClient;
        IList<string> DeviceList;
        public delegate void SendCompletedEventHandler(string op, string message, List<String> data2, Boolean isPJQ, UdpClient client, IPEndPoint ipendpoint);
        event SendCompletedEventHandler USBSendCompleted;
        System.Timers.Timer GetEvalResultTimer;

        private string mID = "";
        private string mDeviceVer = "";
        private string mDeviceNVer = "";
        private bool isConnected = false;
        internal static KeyboardHook KEYKOOK;
        IPEndPoint ipendpoint;
        UdpClient udpclient;
        Thread UdpThread;
        static bool isRect = false;//是否处于吸附状态
        System.Timers.Timer HeartTimer;
        System.Timers.Timer ConnTimer;
        static bool isCuttingVideo = false, isCuttingVideo2 = false;//是否正在同屏
        static bool isForwardPort = false;//本地端口是否转发成功

        public WorkForm()
        {
            InitializeComponent();

            ConnTimer = new System.Timers.Timer() { Interval = 10 * 1000 };
            ConnTimer.Elapsed += new System.Timers.ElapsedEventHandler(ConnTimer_Tick);

            HeartTimer = new System.Timers.Timer();
            HeartTimer.Interval = 10000;
            HeartTimer.Elapsed += new System.Timers.ElapsedEventHandler(HeartTimer_Tick);

            m_SyncContext = SynchronizationContext.Current;
            KEYKOOK = new KeyboardHook();
            KEYKOOK.Start(OnKeyPress);//安装键盘钩子

            InitRightMenu();
            InitUI();
        }
        
        private void OnKeyPress(KeyEventArgs e, out bool handle)
        {
            handle = false;
            string eStr = HotKey.GetStringByKey(e);
            if (null != MainStaticData.keyDict)
            {
                foreach (var item in MainStaticData.keyDict)
                {
                    KeyEventArgs hotkey = item.Value;
                    if (e.KeyValue == hotkey.KeyValue && Control.ModifierKeys == hotkey.Modifiers)
                    {
                        switch (item.Key)
                        {
                            case "欢迎光临":
                                btnWel_Click(null, null);
                                handle = true;
                                break;
                            case "暂停服务":
                                btnPause_Click(null, null); handle = true;
                                break;
                            case "服务评价":
                                btnEval_Click(null, null); handle = true;
                                break;
                            case "语音播报":
                                btnVoice_Click(null, null); handle = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 通知区域右键菜单
        /// </summary>
        private void InitRightMenu()
        {
            IDictionary<string,string> keyD = db_KeyConfig.getKeyConfig(MainStaticData.USERCARD);
            
            if (null != keyD)
            {
                MainStaticData.keyDict = new Dictionary<string, KeyEventArgs>();
                foreach (var item in keyD)
                {
                    KeyEventArgs hotkey = HotKey.GetKeyByString(item.Value);
                    MainStaticData.keyDict.Add(item.Key, hotkey);
                }
            }
            
            contextMenuStrip1.Items.Add("参数配置");
            contextMenuStrip1.Items[0].Click += configSet_Click;

            contextMenuStrip1.Items.Add("修改密码");
            contextMenuStrip1.Items[1].Click += tmEditPwd_Click;

            contextMenuStrip1.Items.Add("快捷键设置");
            contextMenuStrip1.Items[2].Click += tmHotKey_Click;

            contextMenuStrip1.Items.Add("强制更新");
            contextMenuStrip1.Items[3].Click += TmUpdateRes_Click;

            contextMenuStrip1.Items.Add("退出");
            contextMenuStrip1.Items[4].Click += btnLogout_Click;

            contextMenuStrip1.Items.Add("当前版本-"+ ConfigurationManager.AppSettings["version"]);

            //tmEditPwd.Click += tmEditPwd_Click;//修改密码
            //tmClose.Click += TmLogout_Click;//关闭
            //tmHotKey.Click += tmHotKey_Click;//快捷键
            //tmLogout.Click += TmLogout_Click;//注销
            //tmLogout.Text = tmLogout.Text + MainStaticData.USERCARD;
            //tmUpdateRes.Click += TmUpdateRes_Click;
        }

        /// <summary>
        /// 强制更新资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmUpdateRes_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Items[3].Text = "正在更新";
            contextMenuStrip1.Items[3].Enabled = false;

            Action caller = new Action(delegate
            {
                String data = String.Format("mac={0}&config=true", this.mID); //检查资源版本号
                String ret = HttpUtil.RequestData(MainStaticData.ServerAddr + MainStaticData.INET_GETUPDATEVERSION, data).Replace("Version=", "");
                ShowInfo2("强制更新-评价器更新资源版本：" + this.mDeviceNVer + ", 服务器最新版本：" + ret);
                int mServerVer;//服务器版本号
                if (int.TryParse(ret, out mServerVer))
                {
                    db_ResUpdateDao.update(this.mID, mServerVer, 0);
                    this.mDeviceNVer = "0";
                    _checkupdateres();
                    contextMenuStrip1.Items[3].Text = "强制更新";
                    contextMenuStrip1.Items[3].Enabled = true;
                }
            }); 
            m_SyncContext.Post(delegate {
                IAsyncResult result = caller.BeginInvoke(null, null);
            }, null);
        }
        
        private void TmLogout_Click(object sender, EventArgs e)
        {
            if (true)//(sender is ToolStripMenuItem)
            {
                string senderName = "关闭";//((Button)sender).Text;
                if (senderName == "关闭")//任务栏通知区域的退出
                {
                    MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                    DialogResult dr = MessageBox.Show("确定要退出吗?", "退出系统", messButton);

                    if (dr == DialogResult.OK)//如果点击“确定”按钮
                    {
                        if (isCuttingVideo)
                        {
                            isCuttingVideo = false;
                            while (isCuttingVideo2)
                                Thread.Sleep(100);
                            this.Cut2Timer.Enabled = false;
                            USBSendData("S08||STOP||E", "StopCutPrint");
                        }
                        //USBSendData("S08||STOP||E", "StopCutPrint");
                        this.notifyIcon1.Dispose();
                        System.Environment.Exit(0);
                    }
                }
                if (senderName.StartsWith("注销"))//操作菜单"注销"
                {
                    //tmLogout.Text = "正在注销";
                    //tmLogout.Enabled = false;
                    this.notifyIcon1.Dispose();
                    new Thread(new ThreadStart(delegate {m_SyncContext.Post(delegate {
                        if (isCuttingVideo)
                        {
                            isCuttingVideo = false;
                            while (isCuttingVideo2)
                                Thread.Sleep(100);
                            this.Cut2Timer.Enabled = false;
                            USBSendData("S08||STOP||E", "StopCutPrint");
                        }
                        //USBSendData("S08||STOP||E", "StopCutPrint");
                        //db_EmployeeLoginDao.logout(MainStaticData.USERCARD);
                        MainStaticData.Restart();
                    },"");})).Start();
                }
            }
        }

        /// <summary>
        /// 修改密码页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmEditPwd_Click(object sender, EventArgs e)
        {
            if (MainStaticData.cbNoLogin)
            {
                MessageBox.Show("免登录模式，不允许修改密码");
                return;
            }
            EditPwdFrm frm = new EditPwdFrm();
            frm.Show();
            this.Visible = false;
            MainStaticData.wf = this;
        }

        /// <summary>
        /// 修改快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmHotKey_Click(object sender, EventArgs e)
        {
            HotKeyFrm frm = new HotKeyFrm();
            frm.Show();
            this.Visible = false;
            MainStaticData.wf = this;
        }
        
        /// <summary>
        /// 配置页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void configSet_Click(object sender, EventArgs e)
        {
            ConfigSet setForm = new ConfigSet();
            setForm.Show();
            this.Visible = false;
            MainStaticData.wf = this;
        }

        private void ShowInfo2(string strInfo)
        {
            try
            {
                Console.WriteLine(DateTime.Now + strInfo);
                LogHelper.WriteInfo(TAG, strInfo);
            }
            catch (Exception)
            {
                
            }
        }
        
        private void ConnDevice()
        {
            if (MainStaticData.isNetwork)
            {
                if (!IsUdpcRecvStart)
                {
                    try
                    {
                        ipendpoint = new IPEndPoint(IPAddress.Any, MainStaticData.udpRecivePort);
                        udpclient = new UdpClient(ipendpoint);
                        IsUdpcRecvStart = true;
                        UdpThread = new Thread(NetworkListener);
                        UdpThread.IsBackground = true;
                        UdpThread.Start(udpclient);
                    }
                    catch (SocketException ex)
                    {
                        ShowInfo2("评价器监听失败:" + ex.Message);
                        isConnected = false;
                        LogHelper.WriteError(TAG, ex);
                        MessageBox.Show("评价器监听失败:端口" + MainStaticData.udpRecivePort + "被占用,请与管理员联系");
                        return;
                    }
                    catch (Exception ex)
                    {
                        ShowInfo2("端口监听失败3！");
                        isConnected = false;
                        LogHelper.WriteError(TAG, ex);
                        return;
                    }
                    ShowInfo2("端口监听成功");
                }
                ShowInfo2("网络方式--心跳");
                ConnTimer_Tick(null, null);
                ConnTimer.Enabled = true;
                while (!isConnected)
                {
                    Thread.Sleep(1000);
                }
                String recvData = NetWorkSendData(String.Format("S98{0},{1},{2},{3},{4},{5}E", 1, MainStaticData.ServerIP, MainStaticData.FtpIP, MainStaticData.FtpPort, MainStaticData.FtpUserName, MainStaticData.FtpPwd)); //修改连接方式为网络连接1
            }
            else //USB连接
            {
                MyClient = new ADBClient();
                MyClient.AdbPath = MainStaticData.AdbExePath;
                MyClient.StartServer();
                
                DeviceList = MyClient.Devices();
                if (DeviceList.Count == 0)
                {
                    if (IsHandleCreated && InvokeRequired)
                    {
                        this.Invoke(new Action(delegate {
                            if (WindowState == FormWindowState.Minimized)
                            {
                                this.Visible = true;
                                //还原窗体显示    
                                WindowState = FormWindowState.Normal;
                                //激活窗体并给予它焦点
                                this.Activate();
                                //任务栏区显示图标
                                this.ShowInTaskbar = true;
                                CommonHelper.SetMid(this);
                                Program.Notify("提醒", "没有检测到评价器！");
                            }
                        }));
                    }
                        
                    ShowInfo2("没有检测到评价器！");
                    this.isConnected = false;
                    if (!isConnected)
                    {
                        Thread.Sleep(5000);
                        ConnDevice();
                    }
                }
                else
                {
                    ShowInfo2("检测到USB设备,可以进行端口转发了.");
                    if (!isForwardPort)
                    {
                        MyClient.Forward(MainStaticData.forwardPort, MainStaticData.devicePort);
                        ShowInfo2(String.Format("设备端口{0}重定向到本地端口{1}",
                            MainStaticData.devicePort, MainStaticData.forwardPort));
                        isForwardPort = true;
                        Thread.Sleep(200);
                    }
                    ShowInfo2("USB端口转发了,可以发送数据包了.");
                    if (!ConnTimer.Enabled) {
                        ConnTimer_Tick(null, null);
                        ConnTimer.Enabled = true;
                        ConnTimer.Start();
                    }
                        
                    while (!isConnected)
                    {
                        Thread.Sleep(100);
                    }
                    ShowInfo2("设备连接成功");
                    if (isConnected)
                    {
                        List<string> data2 = USBSendData("S99||0||E", "getDeviceInfo");//获取设备信息
                        if (data2 != null && data2.Count > 0)
                        {
                            String[] d = data2[data2.Count - 1].Split(new string[] { "||" }, StringSplitOptions.None);
                            if (d.Length > 2)
                            {
                                this.mID = d[2];
                                this.mDeviceVer = d[1].Replace("Version=", "");
                                this.mDeviceNVer = d[d.Length - 2];
                                ShowInfo2("获取设备mac地址:" + this.mID + "，设备资源版本号：" + this.mDeviceVer + "，设备更新资源版本号：" + this.mDeviceNVer);
                                isConnected = true;
                                BtnEnable(true);
                            }
                        }

                        bool islogin = EmployeeLogin(false, null, null);
                        while (!islogin && !MainStaticData.cbNoLogin)
                        {
                            Thread.Sleep(10000);
                            ConnDevice();
                        }

                        USBSendData(String.Format("S98||{0}{1}||E", 0, MainStaticData.ServerIP), "changeConnType");//修改连接方式
                        ShowInfo2("开始心跳" + IsUdpcRecvStart);
                        HeartTimer.Enabled = true;

                        #region 监听排队叫号器
                        if (!IsUdpcRecvStart)
                        {
                            try
                            {
                                ipendpoint = new IPEndPoint(IPAddress.Any, MainStaticData.udpPort);
                                udpclient = new UdpClient(ipendpoint);
                                IsUdpcRecvStart = true;
                                UdpThread = new Thread(USBPJQListener);
                                UdpThread.IsBackground = true;
                                UdpThread.Start(udpclient);
                            }
                            catch (SocketException ex)
                            {
                                ShowInfo2("呼叫器监听建立失败1:" + ex.Message);
                                MessageBox.Show("呼叫器监听失败:端口" + MainStaticData.udpPort + "被占用,请与管理员联系");
                                LogHelper.WriteError(TAG, ex);
                            }
                            catch (Exception ex)
                            {
                                ShowInfo2("呼叫器监听建立失败1:" + ex.Message);
                                MessageBox.Show("呼叫器监听建立失败");
                                LogHelper.WriteError(TAG, ex);
                            }
                        }
                        #endregion
                    }
                }
            }
        }
        
        private Boolean EmployeeLogin(Boolean isPJQ, UdpClient client, IPEndPoint ipendpoint)
        {
            String d = "";
            if (MainStaticData.isNetwork)
            {
                if (String.IsNullOrEmpty(MainStaticData.USERCARD))
                {
                    return false;
                }
                else
                {
                    this.NetWorkSendData(String.Format("S04{0}E", MainStaticData.USERCARD));
                    return true;
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(MainStaticData.USERCARD))
                {
                    String employeeData = HttpUtil.RequestData(MainStaticData.ServerAddr + MainStaticData.INTE_EMPLOYEEGETINFO, "cardnum=" + MainStaticData.USERCARD);
                    UserModel model = null;
                    try
                    {
                        model = JsonHelper.DeserializeJsonToObject<UserModel>(employeeData);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteError(TAG, ex);
                        ShowInfo2("用户登录失败-服务器出错！");
                        return false;
                    }
                    DirectoryInfo d1 = new DirectoryInfo("picture");
                    if (!d1.Exists)
                    {
                        d1.Create();
                    }
                    try
                    {
                        if (null != model && null != model.picture && "" != model.picture)
                        {
                            string picpath = "picture/user_" + model.ext2 + ".jpg";
                            bool down = HttpUtil.DownloadFile(MainStaticData.ServerAddr + model.picture, "picture/user_" + model.ext2 + ".jpg");
                            if (down)
                            {
                                USBSendFile("", picpath);
                                MyClient.Push(picpath, MainStaticData.SDCARD + "/user_" + model.ext2 + ".jpg");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteError(TAG, ex);
                        ShowInfo2("用户照片下载失败！");
                        return false;
                    }

                    try
                    {
                        if (null != model)
                        {
                            StreamReader infosetdown = HttpUtil.RequestStream(MainStaticData.ServerAddr + MainStaticData.INTE_EMPLOYEEINFOSETDOWN, "");

                            d = MainStaticData.USERCARD;
                            XmlSerializer ser = new XmlSerializer(typeof(InfoSetModel));
                            model.InfoSetModel infoset = ser.Deserialize(infosetdown) as InfoSetModel;
                            if (infoset.employeeName)
                            {
                                d += "|name,姓名," + model.name;
                            }
                            if (infoset.job_desc)
                            {
                                d += "|jobDesc,职务," + model.job_desc;
                            }
                            if (infoset.employeeJobNum)
                            {
                                d += "|jobNum,工号," + model.ext2;
                            }
                            if (infoset.star)
                            {
                                d += "|star,星级," + model.star;
                            }
                            if (infoset.phone)
                            {
                                d += "|phone,联系方式," + model.phone;
                            }
                            if (infoset.windowName)
                            {
                                d += "|window,窗口名称," + model.showWindowName;
                            }
                            if (infoset.deptname)
                            {
                                d += "|department,部门名称," + model.showDeptName;
                            }
                            if (infoset.unitName) //company
                            {
                                d += "|company,单位名称," + model.companyName;
                            }
                            List<string> data2 = USBSendData(String.Format("S04||{0}||E", d), "login", isPJQ, client, ipendpoint);
                            if (data2.Count > 0 && data2[data2.Count - 1] == "RecvCmdOK")
                                return true;
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteError(TAG, ex);
                        ShowInfo2("用户配置信息下载失败！");
                        return false;
                    }
                }
                
                
                return false;
            }
        }

        private bool IsUdpcRecvStart = false;

        /// <summary>
        /// 监听UDP返回消息－网络方式
        /// </summary>
        public void NetworkListener(object obj)
        {
            try
            {
                UdpClient client = obj as UdpClient;
                IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 0);
                ShowInfo2("开始监听（UDP）评价器返回数据，端口：7791," + IsUdpcRecvStart);
                while (IsUdpcRecvStart)
                {
                    byte[] bytes = client.Receive(ref remoteIpep);
                    string strInfo = MainStaticData.encode.GetString(bytes, 0, bytes.Length);
                    ShowInfo2("评价器返回：" + strInfo);
                    if (strInfo == "DisConnect")
                    {
                        isConnected = false;
                        return;
                    }
                    if (strInfo == "S04OK")//登录成功
                    {
                        ShowInfo2("登录成功!");
                        isConnected = true;
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(delegate ()
                            {
                                this._BtnEnable(true);
                            }));
                        }
                        else
                        {
                            this._BtnEnable(true);
                        }
                    }
                    if (strInfo == "S05OK")//退出成功
                    {
                        ShowInfo2("退出成功！");
                        IsUdpcRecvStart = false;
                        new Thread(StopUdpListen).Start();
                    }
                    if (strInfo == "S98OK")//修改评价器配置成功
                    {
                        isConnected = true;
                        ShowInfo2("连接评价器成功！");
                        EmployeeLogin(false, null, null);
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteError(TAG, e);
            }
            finally
            {
                udpclient.Close();
            }
        }

        public void USBPJQListener(object obj)
        {
            try
            {
                UdpClient client = obj as UdpClient;
                IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 8001);
                ShowInfo2("开始监听（排队叫号器）评价器返回数据，端口：8001"+ IsUdpcRecvStart);
                while (IsUdpcRecvStart)
                {
                    byte[] bytes = client.Receive(ref remoteIpep);
                    string strInfo = MainStaticData.encode.GetString(bytes, 0, bytes.Length);
                    ShowInfo2("排队叫号器指令：" + strInfo);
                    PjqModel o = JsonHelper.DeserializeJsonToObject<PjqModel>(strInfo);
                    ShowInfo2("排队叫号器指令："+o.command);

                    byte[] data = MainStaticData.encode.GetBytes(JsonHelper.SerializeObject(new { succ = "0" }));
                    int rr = client.Send(data, data.Length, remoteIpep);
                    ShowInfo2("评价器应答命令:" + rr);

                    if (o.command == "login")
                    {
                        MainStaticData.USERCARD = o.sno;
                        EmployeeLogin(true, client, remoteIpep);
                    }
                    if (o.command == "unlogin")
                    {
                        ActionLogout(true, client, remoteIpep);
                    }
                    if (o.command == "pause")
                    {
                        ActionPauseOnly(client, remoteIpep);
                    }
                    if (o.command == "reset")
                    {
                        ActionPauseReset(client, remoteIpep);
                    }
                    if (o.command == "welcome")
                    {
                        ActionWel(true, client, remoteIpep);
                    }
                    if (o.command == "apprise")
                    {
                        ActionEval(true, client, remoteIpep);
                    }
                    if (o.command == "call")
                    {
                        ActionCall(o.number, o.servicename, true, client, remoteIpep);
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteError(TAG, e);
                ShowInfo2(e.ToString());
            }
        }

        private void StopUdpListen()
        {
            //db_EmployeeLoginDao.logout(MainStaticData.USERCARD);
            ShowInfo2("销毁udp监听进程");
            if(UdpThread!=null)
                UdpThread.Abort();
            if(udpclient!=null)
                udpclient.Close();
            this.notifyIcon1.Dispose();
            System.Environment.Exit(0);
        }

        /// <summary>
        /// 
        /// </summary>
        private void BeginGetEvalResult(Boolean isPJQ, UdpClient client, IPEndPoint endPoint)
        {
            GetEvalResultTimer = new System.Timers.Timer();
            GetEvalResultTimer.Interval = 2000;
            GetEvalResultTimer.Elapsed += (sender, e) => { USBSendData("SE", "evalwait", isPJQ, client, endPoint); };//GetEvalResult_Tick;
            GetEvalResultTimer.Enabled = true;
        }
        
        private void USBSendFile(string path, String filename, String removeName)
        {
            ShowInfo2("发送文件：" + filename);
            if(path == "" || path == null)
            {
                MyClient.Push(filename, MainStaticData.SDCARD + "/" + removeName);
            }
            else
                MyClient.Push(path + "/" + filename, MainStaticData.SDCARD + path + "/" + removeName);
        }

        private void USBSendFile(string path, String filename)
        {
            USBSendFile(path, filename, filename);
        }

        private List<String> USBSendData(Object response, String op)
        {
            return this.USBSendData(response, op, false, null, null);
        }

        private List<String> USBSendData(Object response, String op, Boolean isPJQ, UdpClient client, IPEndPoint ipendpoint)
        {
            ShowInfo2("USBSendData:" + response.ToString());
            if (!isConnected && !response.ToString().StartsWith("S99||"))
            {
                ShowInfo2("评价器连接断开.");
                if (btnWel.Enabled)
                {
                    MessageBox.Show(Resources.ZHCN_DEVICEDISCONN);
                    BtnEnable(false);
                }
                return null;
            }
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), MainStaticData.forwardPort));
            }
            catch (SocketException)
            {
                ShowInfo2("评价器连接失败,重定向连接");
                MyClient.Forward(MainStaticData.forwardPort, MainStaticData.devicePort);
                try
                {
                    socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), MainStaticData.forwardPort));
                }
                catch (SocketException)
                {
                    ShowInfo2("评价器再次连接失败,不连了");
                    this.isConnected = false;
                    BtnEnable(false);
                    return null;
                }
            }
            
            try
            {
                byte[] data = MainStaticData.encode.GetBytes(response.ToString());
                ShowInfo2("(USB)发送内容：" + response.ToString());
                socket.Send(data, 0, data.Length, SocketFlags.None);
                byte[] buffer = new byte[1024];
                List<byte> cdata = new List<byte>();
                int length = 0;
                List<String> mList = new List<string>();
                try
                {
                    while ((length = socket.Receive(buffer, buffer.Length, SocketFlags.None)) > 0)
                    {
                        for (int j = 0; j < length; j++)
                        {
                            cdata.Add(buffer[j]);
                        }
                        mList.Add(MainStaticData.encode.GetString(buffer, 0, length));
                    }
                    String receiveData = MainStaticData.encode.GetString(cdata.ToArray(), 0, cdata.Count).Trim();

                    USBSendCompleted(response.ToString(), receiveData, mList, isPJQ, client, ipendpoint);
                    
                }
                catch (SocketException ex)
                {
                    LogHelper.WriteError(TAG, ex);
                    ShowInfo2("设备连接失败！");
                    isConnected = false;
                    BtnEnable(false);

                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(TAG, ex);
                    ShowInfo2("设备连接失败！");
                    isConnected = false;
                    BtnEnable(false);
                }
                return mList;
            }
            catch (SocketException e)
            {
                LogHelper.WriteError(TAG, e);
                ShowInfo2("网络错误,请检查评价器是否连接正常.");
                this.isConnected = false;
                BtnEnable(false);
            }
            catch (Exception e)
            {
                LogHelper.WriteError(TAG, e);
                ShowInfo2("发送失败,请检查评价器是否连接正常.");
                this.isConnected = false;
                BtnEnable(false);
            }
            finally
            {
                socket.Close();
            }
            return null;
        }
        
        /// <summary>
        /// 发送UDP数据－网络方式
        /// </summary>
        /// <param name="str"></param>
        private String NetWorkSendData(String str)
        {
            String recvdata = "";
            try
            {
                byte[] data = new byte[1024];
                //设置服务IP，设置TCP端口号
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(MainStaticData.DeviceIP), MainStaticData.udpPort);

                //定义网络类型，数据连接类型和网络协议UDP
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                data = MainStaticData.encode.GetBytes(str);
                server.SendTimeout = 3000;
                server.SendTo(data, data.Length, SocketFlags.None, ipep);

                IPEndPoint sender = new IPEndPoint(IPAddress.Parse(MainStaticData.DeviceIP), 0);
                Console.WriteLine("MainStaticData.DeviceIP:" + MainStaticData.DeviceIP + ",发送内容：" + str);
                EndPoint Remote = sender;

                try
                {
                    data = new byte[1024];
                    //发送接收信息
                    server.ReceiveTimeout = 10000;
                    int recv = server.ReceiveFrom(data, ref Remote);
                    recvdata = MainStaticData.encode.GetString(data, 0, recv);
                }
                catch (SocketException e)
                {
                    LogHelper.WriteError(TAG, e);
                    recvdata = String.Empty;
                    isConnected = false;
                    BtnEnable(false);
                    if(!ConnTimer.Enabled)
                        ConnTimer.Enabled = true;
                }
                catch (Exception e)
                {
                    LogHelper.WriteError(TAG, e);
                    recvdata = String.Empty;
                }

                server.Close();
                return recvdata;
            }
            catch (Exception ex)
            {
                ShowInfo2("UDP发送失败" + ex.Message);
                LogHelper.WriteError(TAG, ex);
                return String.Empty;
            }
        }

        //定义与方法同签名的委托  
        private delegate void DelegateName();
        int inTimer = 0;
        int inTimer2 = 0;

        /// <summary>
        /// 设备连接心跳
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnTimer_Tick(object sender, EventArgs e)
        {
            if (MainStaticData.isNetwork)
            {
                String recvData = NetWorkSendData("S95E");
                isConnected = recvData == "S95OK";
                BtnEnable(isConnected);
            }
            else
            {
                List<String> data2 = USBSendData("S99||0||E", "getDeviceInfo");//获取设备信息
                if (data2 == null || data2.Count ==0)
                {
                    ShowInfo2("评价器连不上了");
                    isConnected = false;
                }
                else
                {
                    isConnected = false;
                    foreach (var item in data2)
                    {
                        if (item == "RecvCmdOK")
                        {
                            ShowInfo2("设备连接心跳:成功");
                        }
                    }
                    String[] d = data2[data2.Count - 1].Split(new string[] { "||" }, StringSplitOptions.None);
                    if (d.Length > 2)
                    {
                        this.mID = d[2];
                        this.mDeviceVer = d[1].Replace("Version=", "");
                        this.mDeviceNVer = d[d.Length - 2];
                        ShowInfo2("获取设备mac地址:" + this.mID + "，设备资源版本号：" + this.mDeviceVer + "，设备更新资源版本号：" + this.mDeviceNVer);
                        isConnected = true;
                    }
                }

                if (isConnected)
                {
                    try
                    {
                        #region 心跳方法
                        String data = String.Format("mac={0}&cardNum={1}", this.mID, MainStaticData.USERCARD);
                        String ret = HttpUtil.RequestData(MainStaticData.ServerAddr + MainStaticData.INET_EMPLOYEEHEART, data);
                        ShowInfo2(DateTime.Now + "心跳：" + ret + this.mID);
                        #endregion
                        isConnected = ret == "online";
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteError(TAG, ex);
                        ShowInfo2("服务器连接失败！" + MainStaticData.ServerAddr + MainStaticData.INTE_EMPLOYEEGETINFO);
                        isConnected = false;
                    }
                }
                BtnEnable(isConnected);
            }
        }

        private void HeartTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Interlocked.Exchange(ref inTimer, 1) == 0)
                {
                    #region USB
                    if (!MainStaticData.isNetwork && isConnected) {
                        #region 检查资源更新
                        _checkupdateres();
                        #endregion

                        #region 检查游屏信息
                        String data = String.Format("mac={0}", this.mID);
                        String ret = HttpUtil.RequestData(MainStaticData.ServerAddr + MainStaticData.INTE_GETDEPTNOTICE, data);
                        USBSendData(String.Format("S15||{0}||E", ret), "Notice");
                        #endregion

                        #region 不满意按键
                        data = String.Format("mac={0}", this.mID);
                        ret = HttpUtil.RequestData(MainStaticData.ServerAddr + MainStaticData.INTE_GETDISSATISFIEDKEY, data);
                        USBSendData(String.Format("S18||{0}||E", ret), "DISSATISKEY");
                        #endregion

                        #region 信息查询更新
                        data = String.Format("?mac={0}", this.mID);
                        HttpUtil.DownloadFile(MainStaticData.ServerAddr + MainStaticData.INTE_WEBURLDOWNLOAD + data, "quary.xml");
                        ShowInfo2("信息查询更新");
                        USBSendFile("", "quary.xml");
                        #endregion

                        #region 通知公告更新
                        HttpUtil.DownloadFile(MainStaticData.ServerAddr + MainStaticData.INTE_NOTICEDOWNLOAD, "notice.xml");
                        ShowInfo2("通知公告更新");
                        USBSendFile("", "notice.xml");
                        #endregion

                        #region 意见调查
                        HttpUtil.DownloadFile(MainStaticData.ServerAddr + MainStaticData.INTE_ADVICEDOWNLOAD, "advice.db");
                        USBSendFile("", "advice.db", "advise.ItemListener");
                        #endregion

                        #region 评价按钮
                        ret = HttpUtil.RequestData(MainStaticData.ServerAddr + MainStaticData.INTE_GETKEYALL, "");
                        USBSendData(String.Format("S51||{0}||E", ret), "GETKEYALL");
                        #endregion
                    }
                    #endregion
                    Interlocked.Exchange(ref inTimer, 0);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(TAG, ex);
            }
        }

        /// <summary>
        /// 更新资源
        /// </summary>
        private void _checkupdateres()
        {
            if (Interlocked.Exchange(ref inTimer2, 1) == 0)
            {
                String data = String.Format("mac={0}&config=true", this.mID); //检查资源版本号
                String ret = HttpUtil.RequestData(MainStaticData.ServerAddr + MainStaticData.INET_GETUPDATEVERSION, data).Replace("Version=", "");
                ShowInfo2("评价器更新资源版本：" + this.mDeviceNVer + ", 服务器最新版本：" + ret);
                int mServerVer;//服务器版本号
                if (this.mDeviceNVer != ret && int.TryParse(ret, out mServerVer))
                {
                    int? laststate = db_ResUpdateDao.selectlast(this.mID, mServerVer);
                    ShowInfo2("本地当前版本号更新状态：" + laststate);
                    if (laststate == null)
                        db_ResUpdateDao.add(this.mID, mServerVer, 0);//资源未下载

                    if (laststate == null || laststate == 0)//未下载
                    {
                        data = String.Format("mac={0}&config=false", this.mID); //下载更新资源包
                        bool isDown = HttpUtil.DownloadFile(MainStaticData.ServerAddr + MainStaticData.INET_GETUPDATEVERSION + "?" + data, "M7Update.zip");
                        ShowInfo2("升级资源下载结果：" + isDown);
                        if (isDown)
                        {
                            if (db_ResUpdateDao.update(this.mID, mServerVer, 1) == 1)//资源下载成功
                            {
                                laststate = 1;
                            }
                            else
                            {
                                ShowInfo2("升级资源下载到本地后，更新本地数据库失败。");
                            }
                        }
                    }
                    String filename = "M7Update.zip";
                    FileInfo updateFile = new FileInfo(filename);
                    if (laststate == 1)
                    {
                        if (!updateFile.Exists)
                        {
                            ShowInfo2("升级资源文件发送失败，文件不存在。");
                            db_ResUpdateDao.update(this.mID, mServerVer, 0);
                            return;
                        }

                        ShowInfo2("通知评价器准备接收文件：" + filename);
                        USBSendFile("", filename);
                        USBSendData("S97||" + mServerVer + "," + (mServerVer - 1) + "||E", "changeResVersion");
                        if (db_ResUpdateDao.update(this.mID, mServerVer, 2) == 1)//推送成功
                        {
                            laststate = 2;
                        }
                        else
                        {
                            ShowInfo2("更新到推送到评价器成功，但更新本地数据库失败。");
                        }
                    }
                }
                Interlocked.Exchange(ref inTimer2, 0);
            }
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            ConfigSet setForm = new ConfigSet();

            setForm.ShowDialog();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized || isRect)
            {
                this.Visible = true;
                this.ShowIcon = this.ShowInTaskbar = true;
                this.stopAnchor = AnchorStyles.None;
                //还原窗体显示    
                WindowState = FormWindowState.Normal;
                //激活窗体并给予它焦点
                this.Activate();
                //任务栏区显示图标
                this.ShowInTaskbar = true;
                CommonHelper.SetMid(this);
            }
        }

        private void WorkForm_Deactivate(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                //this.Visible = false;
            }
        }

        System.Timers.Timer Cut2Timer;
        
        private void WorkForm_Load(object sender, EventArgs e)
        {
            this.Text = Resources.CLIENTNAME;
            this.notifyIcon1.Text = Resources.CLIENTNAME;
            this.TopMost = true;
            CommonHelper.SetMid(this);
            //Win32.AnimateWindow(this.Handle, 1000, Win32.AW_BLEND);

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            SetVisibleCore(false);

            //Program.Notify("提醒", "程序启动成功");

            //同屏
            Cut2Timer = new System.Timers.Timer(MainStaticData.CUTVIDEOPERSECOND);//实例化Timer类，设置间隔时间为10000毫秒； 
            Cut2Timer.Elapsed += new System.Timers.ElapsedEventHandler(CutVideo);//到达时间的时候执行事件； 
            Cut2Timer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)； 
            Cut2Timer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件； 

            USBSendCompleted = delegate (string op, string message, List<String> data2, Boolean isPJQ, UdpClient client, IPEndPoint ipendpoint)
            {
                ShowInfo2(op + isPJQ + "接收数据:" + message);
                if (isPJQ)
                {
                    //byte[] data = MainStaticData.encode.GetBytes(JsonHelper.SerializeObject(new { succ ="0"}));
                    //int rr = client.Send(data, data.Length, ipendpoint);
                    //ShowInfo2("评价器应答命令:" + rr);
                }
                for (int i = 0; i < data2.Count; i++)
                {
                    ShowInfo2(op + "------------------:" + data2[i]);
                    if (data2[i].StartsWith("S02||"))//评价数据
                    {
                        String[] d = data2[i].Split(new string[] { "||" }, StringSplitOptions.None);
                        if (isPJQ)
                        {
                            byte[] data1 = MainStaticData.encode.GetBytes(JsonHelper.SerializeObject(new { appriseresult = d[2] }));
                            int rr = client.Send(data1, data1.Length, ipendpoint);
                            ShowInfo2("评价器应答命令:" + rr + ", "+ JsonHelper.SerializeObject(new { appriseresult = d[2] }));
                        }
                        
                        String data = String.Format("mac={0}&tt={1}&cardnum={2}&pj={3}&demo=(NULL)&businessType=1&videofile={4}&businessTime={5}&imgfile={6}&busRst={7}&videofile2={8}"
                            , mID, d[5], d[1], d[2], d[7], d[4], d[6], d[8], d[7]);
                        String rst = HttpUtil.RequestData(MainStaticData.ServerAddr + MainStaticData.INTE_EVALDATA, data);
                        //if (rst == "success")
                        {
                            if (op == "SE" && GetEvalResultTimer != null && GetEvalResultTimer.Enabled)
                                GetEvalResultTimer.Stop();
                            MyClient.Pull(MainStaticData.SDCARD + "recorder/", "recorder/");//下载录音录像文件
                            MyClient.Execute("rm " + MainStaticData.SDCARD + "recorder/*", true);
                            
                            #region 上传录音录像文件到FTP
                            try
                            {
                                String ftpDir = "recorder";
                                List<String> listFiles = CommonHelper.ListFiles(new DirectoryInfo(ftpDir));
                                foreach (var item in listFiles)
                                {
                                    FileInfo f = new FileInfo(item);
                                    if (HttpUtil.UploadFile(MainStaticData.ServerAddr + MainStaticData.INTE_APPRIESFILEUPLOAD, f))
                                    {
                                        f.Delete();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LogHelper.WriteError(TAG, ex);
                            }
                            #endregion
                        }
                    }
                    if (op.StartsWith("S98||") && data2[i].StartsWith("RecvCmdOK"))//修改评价器配置成功
                    {
                        DateTime now = DateTime.Now;
                        USBSendData(String.Format("S14||{0}||{1}||{2}||{3}||{4}||{5}||E", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second), "SyncDateTime");//同步设备时间

                        String inteResp = HttpUtil.RequestData(MainStaticData.ServerAddr + MainStaticData.INTE_GETDEPTVIDEO, "mac=" + this.mID);
                        ShowInfo2("获取设备" + this.mID + "录音录像权限:" + inteResp);

                        USBSendData("S17||" + inteResp + "||E", "Recorder");//设置录音录像
                    }
                }
                if (data2.Count > 0 && data2[0].StartsWith("S24||"))
                {
                    ShowInfo2("获取意见调查数据:" + data2[0]);
                    String adviceData = data2[0].Substring(5, data2[0].Length-8);
                    ShowInfo2("获取意见调查数据:" + adviceData.Split(',')[0]);
                    HttpUtil.RequestData(MainStaticData.ServerAddr + MainStaticData.INTE_ADVICEANSWER, "answer=" + adviceData.Split(',')[0]);
                }
            };

            new Thread(ConnDevice).Start();//连接设备

            //自动吸附
            ctimer = new System.Timers.Timer();
            ctimer.Interval = 300;
            ctimer.Elapsed += StopRectTimer_Tick;
            ctimer.Start();
        }

        internal AnchorStyles stopAnchor = AnchorStyles.None;
        //计时器 通过win32api实时获取鼠标位置c
        internal static System.Timers.Timer ctimer;

        private void StopRectTimer_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.Invoke(new Action(delegate () {
                    //如果鼠标在窗体上，则根据停靠位置显示整个窗体
                    if (this.Bounds.Contains(Cursor.Position))
                    {
                        switch (this.stopAnchor)
                        {
                            case AnchorStyles.Top:
                                this.Location = new Point(this.Location.X, 0);
                                break;
                            case AnchorStyles.Bottom:
                                this.Location = new Point(this.Location.X, Screen.PrimaryScreen.Bounds.Height - this.Height);
                                break;
                            case AnchorStyles.Left:
                                this.Location = new Point(0, this.Location.Y);
                                break;
                            case AnchorStyles.Right:
                                this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Width, this.Location.Y);
                                break;
                        }
                    }
                    else //如果鼠标离开窗体，则根据停靠位置隐藏窗体，但须留出部分窗体边缘以便鼠标选中窗体
                    {
                        switch (this.stopAnchor)
                        {
                            case AnchorStyles.Top:
                                this.Location = new Point(this.Location.X, (this.Height - 3) * (-1));
                                break;
                            case AnchorStyles.Bottom:
                                this.Location = new Point(this.Location.X, Screen.PrimaryScreen.Bounds.Height - 5);
                                break;
                            case AnchorStyles.Left:
                                this.Location = new Point((-1) * (this.Width - 3), this.Location.Y);
                                break;
                            case AnchorStyles.Right:
                                this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - 2, this.Location.Y);
                                break;
                        }
                    }
                }));
            }
            catch(  Exception ex)
            {
                LogHelper.WriteError(TAG, ex);
            }
        }
        
        /// <summary>
        /// 欢迎光临
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWel_Click(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(delegate ()
                {
                    this.btnPause.Text = "暂停";
                }));
            }
            else
            {
                this.btnPause.Text = "暂停";
            }
            AsyncBtn(this.btnWel, new Action(delegate() { ActionWel(false, null, null); }));
        }

        private void ActionWel(bool isPJQ, UdpClient client, IPEndPoint remoteIpep)
        {
            if (MainStaticData.isNetwork)
                this.NetWorkSendData("S01E");
            else
                USBSendData("S01E", "welcome", isPJQ, client, remoteIpep);
        }

        /// <summary>
        /// 服务暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPause_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show(MainStaticData.resource.GetString("ZHCN_DEVICENOCONN"));
                return;
            }
            ActionPause(null,null);
        }

        private void ActionPauseReset(UdpClient client, IPEndPoint remoteIpep)
        {
            if (MainStaticData.isNetwork)
            {
                if (btnPause.Text.ToString() == "取消暂停")
                {
                    NetWorkSendData("S06E");
                    if (this.InvokeRequired)
                        this.Invoke(new Action(delegate {
                            btnPause.Text = "暂停";
                        }));
                    else
                        btnPause.Text = "暂停";
                }
            }
            else
            {
                if (btnPause.Text.ToString() == "取消暂停")
                {
                    List<String> rst = USBSendData("S25E", "pause", client != null, client, remoteIpep);
                    foreach (var item in rst)
                    {
                        if (item == "RecvCmdOK")
                        {
                            this.InvokeMethod(new Action(delegate { btnPause.Text = "暂停"; }));
                        }
                    }

                }
            }
        }

        private void ActionPauseOnly(UdpClient client, IPEndPoint remoteIpep)
        {
            if (MainStaticData.isNetwork)
            {
                if (this.btnPause.Text.ToString() == "暂停")
                {
                    string rst = this.NetWorkSendData("S03E");
                    Console.WriteLine("命令返回结果:" + rst);
                    if (this.InvokeRequired)
                        this.Invoke(new Action(delegate {
                            btnPause.Text = "取消暂停";
                        }));
                    else
                        btnPause.Text = "取消暂停";
                }
            }
            else
            {
                if (btnPause.Text.ToString() == "暂停")
                {
                    List<String> rst = USBSendData("S03E", "pause", client != null, client, remoteIpep);
                    foreach (var item in rst)
                    {
                        if (item == "RecvCmdOK")
                        {
                            this.InvokeMethod(new Action(delegate { btnPause.Text = "取消暂停"; }));
                        }
                    }
                }
            }
        }

        private void ActionPause(UdpClient client, IPEndPoint remoteIpep)
        {
            if (MainStaticData.isNetwork)
            {
                if (this.btnPause.Text.ToString() == "暂停")
                {
                    string rst = this.NetWorkSendData("S03E");
                    Console.WriteLine("命令返回结果:" + rst);
                    if(this.InvokeRequired)
                        this.Invoke(new Action(delegate {
                            btnPause.Text = "取消暂停";
                        }));
                    else
                        btnPause.Text = "取消暂停";
                }
                else if (btnPause.Text.ToString() == "取消暂停")
                {
                    NetWorkSendData("S06E");
                    if (this.InvokeRequired)
                        this.Invoke(new Action(delegate {
                            btnPause.Text = "暂停";
                        }));
                    else
                        btnPause.Text = "暂停";
                }
            }
            else
            {
                if (btnPause.Text.ToString() == "暂停")
                {
                    List <String> rst = USBSendData("S03E", "pause", client != null, client, remoteIpep);
                    foreach (var item in rst)
                    {
                        if (item == "RecvCmdOK")
                        {
                            this.InvokeMethod(new Action(delegate { btnPause.Text = "取消暂停"; }));
                        }
                    }
                }
                else if (btnPause.Text.ToString() == "取消暂停")
                {
                    List<String> rst = USBSendData("S25E", "pause", client != null, client, remoteIpep);
                    foreach (var item in rst)
                    {
                        if (item == "RecvCmdOK")
                        {
                            this.InvokeMethod(new Action(delegate { btnPause.Text = "暂停"; }));
                        }
                    }
                    
                }
            }
        }

        private void InvokeMethod(Action method) {
            if (this.InvokeRequired)
                this.Invoke(method);
            else
                method();
        }
        /// <summary>
        /// 请评价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEval_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show(MainStaticData.resource.GetString("ZHCN_DEVICENOCONN"));
                return;
            }
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(delegate ()
                {
                    this.btnPause.Text = "暂停";
                }));
            }
            else
            {
                this.btnPause.Text = "暂停";
            }
            AsyncBtn(this.btnEval, new Action(delegate
            {
                ActionEval(false, null, null);
            }));
        }

        private void AsyncBtn(Button btn,Action action)
        {
            new DelegateName(new Action(delegate
            {
                if (btn.InvokeRequired == false)
                {
                    btn.ForeColor = ColorTranslator.FromHtml("#d5d5d5");
                    btn.Enabled = false;
                }
                else
                {
                    btn.Invoke(new Action(delegate {
                        btn.ForeColor = ColorTranslator.FromHtml("#d5d5d5");
                        btn.Enabled = false;
                    }));
                }

            })).BeginInvoke(null, null);
            
            new DelegateName(action).BeginInvoke(null, null);

            new DelegateName(new Action(delegate
            {
                Thread.Sleep(5000);
                if (btn.InvokeRequired == false)
                {
                    btn.Enabled = true;
                    btn.ForeColor = Color.White;
                }
                else
                {
                    btn.Invoke(new Action(delegate
                    {
                        btn.Enabled = true;
                        btn.ForeColor = Color.White;
                    }));
                }
            })).BeginInvoke(null, null);
        }

        private void ActionEval(Boolean isPJQ, UdpClient client, IPEndPoint endPoint)
        {
            if (MainStaticData.isNetwork)
            {
                this.NetWorkSendData("S02E");
            }
            else
            {
                USBSendData("S02E", "eval", isPJQ, client, endPoint);
                if (GetEvalResultTimer != null && GetEvalResultTimer.Enabled)
                    GetEvalResultTimer.Stop();
                BeginGetEvalResult(isPJQ, client, endPoint);
            }
        }

        private void ActionCall(string number, string servicename, Boolean isPJQ, 
            UdpClient client, IPEndPoint endPoint)
        {
            if (MainStaticData.isNetwork)
            {
                
            }
            else
            {
                USBSendData("S96||"+ number +","+ servicename + "||E", "call", isPJQ, client, endPoint);
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogout_Click(object sender, EventArgs e)
        {
            ActionLogout(false, null, null);
        }

        private void ActionLogout(Boolean isPJQ, UdpClient client, IPEndPoint endPoint)
        {
            if (MainStaticData.isNetwork)
            {
                if (!isConnected)
                    StopUdpListen();
                else
                    this.NetWorkSendData("S05E");
            }
            else
            {
                if (isCuttingVideo)
                {
                    isCuttingVideo = false;
                    while (isCuttingVideo2)
                        Thread.Sleep(100);
                    this.Cut2Timer.Enabled = false;
                    USBSendData("S08||STOP||E", "StopCutPrint");
                }
                USBSendData("S08||STOP||E", "StopCutPrint");
                List<string> receiveData = USBSendData("S05E", "logout", isPJQ, client, endPoint);
                if (null != receiveData && receiveData.Count>0 && receiveData[0].Contains("RecvCmdOK"))//退出成功
                {
                    ShowInfo2("用户退出成功！");
                    //db_EmployeeLoginDao.logout(MainStaticData.USERCARD);
                }
                if (!isPJQ)
                {
                    this.notifyIcon1.Dispose();
                    System.Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// 语音播报
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVoice_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show(MainStaticData.resource.GetString("ZHCN_DEVICENOCONN"));
                return;
            }
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(delegate ()
                {
                    this.btnPause.Text = "暂停";
                }));
            }
            else
            {
                this.btnPause.Text = "暂停";
            }
            VoiceText inp = new VoiceText();
            inp.StartPosition = FormStartPosition.CenterParent;
            inp.Focus();
            DialogResult dr = inp.ShowDialog();
            if (dr == DialogResult.OK && inp.Value.Length > 0)
            {
                if (MainStaticData.isNetwork)
                {
                    this.NetWorkSendData("S09" + inp.Value + "E");
                }
                else
                {
                    USBSendData(String.Format("S06||{0}||E", inp.Value), "Voice");
                }
                inp.Close();
                inp.Dispose();
            }
        }

        /// <summary>
        /// 截屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCut_Click(object sender, EventArgs ea)
        {
            if (!isConnected)
            {
                MessageBox.Show(Resources.ZHCN_DEVICENOCONN);
                return;
            }
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(delegate ()
                {
                    this.btnPause.Text = "暂停";
                }));
            }
            else
            {
                this.btnPause.Text = "暂停";
            }
            if (btnCut.Text == "截屏")
            {
                if (!isRect)
                {
                    this.Hide();
                }
                GetScreenOnFocus();

                CutPopUp cut_form = new CutPopUp(
                    CutScreen(screen_on_focus.Bounds.Left, screen_on_focus.Bounds.Top, screen_on_focus.Bounds.Width, screen_on_focus.Bounds.Height),
                    screen_on_focus,
                    previous_selection, this
                );
                cut_form.OnCut += (s, e) =>
                {
                    if (!isRect)
                    {
                        this.Show(); this.Focus();
                    }
                    previous_selection = e.Selection;
                    imgsrc = e.imgsrc;

                    if (MainStaticData.isNetwork)
                    {
                        this.NetWorkSendData("S08E");
                    }
                    else
                    {
                        string imgName = imgsrc.Substring(imgsrc.IndexOf(@"\") + 1);
                        USBSendFile("CutImg", imgName);
                        List<string> receiveData = USBSendData(String.Format("S08||{0}||E", "CutImg/" + imgName), "CutPrint");
                    }
                };
                cut_form.Show();
            }
            else
            {
                btnCut.Text = "截屏";
                USBSendData("S08||STOP||E", "StopCutPrint");
            }
        }

        #region 截图
        Screen screen_on_focus;
        Rectangle previous_selection;
        private string imgsrc;

        private void GetScreenOnFocus()
        {
            screen_on_focus = Screen.FromPoint(new Point(Cursor.Position.X, Cursor.Position.Y));
        }

        private Bitmap CutScreen(int x, int y, int width, int height)
        {
            IntPtr handle_desktop, dc_source, dc_dest, handle_bmp, handle_oldbmp;

            handle_desktop = Imports.user32.NativeMethods.GetDesktopWindow();
            dc_source = Imports.user32.NativeMethods.GetWindowDC(handle_desktop);
            dc_dest = Imports.gdi32.NativeMethods.CreateCompatibleDC(dc_source);
            handle_bmp = Imports.gdi32.NativeMethods.CreateCompatibleBitmap(dc_source, width, height);
            handle_oldbmp = Imports.gdi32.NativeMethods.SelectObject(dc_dest, handle_bmp);

            Imports.gdi32.NativeMethods.BitBlt(dc_dest, 0, 0, width, height, dc_source, x, y, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
            Bitmap screen_bmp = Bitmap.FromHbitmap(handle_bmp);

            Imports.gdi32.NativeMethods.SelectObject(dc_dest, handle_oldbmp);
            Imports.gdi32.NativeMethods.DeleteObject(handle_bmp);
            Imports.gdi32.NativeMethods.DeleteDC(dc_dest);
            Imports.user32.NativeMethods.ReleaseDC(handle_desktop, dc_source);

            return screen_bmp;
        }

        private void CenterOnScreen()
        {
            this.Location = new Point(
                screen_on_focus.WorkingArea.Location.X + (screen_on_focus.Bounds.Width / 2) - (this.Bounds.Width / 2),
                screen_on_focus.WorkingArea.Location.Y + (screen_on_focus.Bounds.Height / 2) - (this.Bounds.Height / 2)
            );
        }
        
        //声明一个API函数
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern bool BitBlt(
            IntPtr hdcDest, // 目标 DC的句柄
            int nXDest,
            int nYDest,
            int nWidth,
            int nHeight,
            IntPtr hdcSrc, // 源DC的句柄
            int nXSrc,
            int nYSrc,
            System.Int32 dwRop // 光栅的处理数值
        );
        #endregion

        private void InitUI()
        {
            int row = 1, col = 7;
            if (MainStaticData.isNetwork)
            {
                col = 5;
                this.Width -= 150;
            }
                
            DynamicLayout(this.tblPanel, row, col);

            this.btnWel = new Button();
            this.btnWel.Text = "欢迎光临";
            this.btnWel.Click += new System.EventHandler(this.btnWel_Click);
            initButtonStyle(this.btnWel);
            this.tblPanel.Controls.Add(btnWel);
            this.tblPanel.SetRow(btnWel, 0);
            this.tblPanel.SetColumn(btnWel, 0);

            this.btnPause = new Button();
            btnPause.Text = "暂停";
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            initButtonStyle(this.btnPause);
            this.tblPanel.Controls.Add(btnPause);
            this.tblPanel.SetRow(btnPause, 0);
            this.tblPanel.SetColumn(btnPause, 1);

            this.btnEval = new Button();
            btnEval.Text = "请评价";
            this.btnEval.Click += new System.EventHandler(this.btnEval_Click);
            initButtonStyle(this.btnEval);
            this.tblPanel.Controls.Add(btnEval);
            this.tblPanel.SetRow(btnEval, 0);
            this.tblPanel.SetColumn(btnEval, 2);

            this.btnVoice = new Button();
            btnVoice.Text = "语音播报";
            this.btnVoice.Click += new System.EventHandler(this.btnVoice_Click);
            initButtonStyle(this.btnVoice);
            this.tblPanel.Controls.Add(btnVoice);
            this.tblPanel.SetRow(btnVoice, 0);
            this.tblPanel.SetColumn(btnVoice, 3);

            int next = 4;
            if (!MainStaticData.isNetwork)
            {
                this.btnCut = new Button();
                btnCut.Text = "截屏";
                this.btnCut.Click += new System.EventHandler(this.btnCut_Click);
                initButtonStyle(this.btnCut);
                this.tblPanel.Controls.Add(btnCut);
                this.tblPanel.SetRow(btnCut, 0);
                this.tblPanel.SetColumn(btnCut, next++);

                this.btnCut2 = new Button();
                this.btnCut2.Text = "同屏";
                this.btnCut2.Click += new System.EventHandler(this.btnCut2_Click);
                initButtonStyle(this.btnCut2);
                this.tblPanel.Controls.Add(btnCut2);
                this.tblPanel.SetRow(btnCut2, 0);
                this.tblPanel.SetColumn(btnCut2, next++);
            }
            this.btnLogout = new Button();
            this.btnLogout.Text = "退出";
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            initButtonStyle(this.btnLogout);
            this.tblPanel.Controls.Add(btnLogout);
            this.tblPanel.SetRow(btnLogout, 0);
            this.tblPanel.SetColumn(btnLogout, next++);

            BtnEnable(false);
        }

        private void initButtonStyle(Button btn)
        {
            btn.Font = new Font("微软雅黑", 9, FontStyle.Regular);
            btn.Dock = DockStyle.Fill;
            btn.ForeColor = Color.White;
            btn.BackgroundImage = System.Drawing.Image.FromFile("images/buttonbg2.png");
            btn.BackgroundImageLayout = ImageLayout.Stretch;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            
            btn.MouseHover += new System.EventHandler(this.btnWel_MouseHover);
            btn.MouseLeave += new System.EventHandler(this.btnWel_MouseLeave);
        }
        private void btnWel_MouseHover(Object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.FlatStyle = FlatStyle.Popup;
        }

        private void btnWel_MouseLeave(Object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
        }

        /// <summary>  
        /// 动态布局  
        /// </summary>  
        /// <param name="layoutPanel">布局面板</param>  
        /// <param name="row">行</param>  
        /// <param name="col">列</param>  
        private void DynamicLayout(TableLayoutPanel layoutPanel, int row, int col)
        {
            layoutPanel.RowCount = row;    //设置分成几行  
            for (int i = 0; i < row; i++)
            {
                layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            }
            layoutPanel.ColumnCount = col;    //设置分成几列  
            for (int i = 0; i < col; i++)
            {
                layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            }
        }

        private void BtnEnable(Boolean enable)
        {
            returnCB2(_BtnEnable, enable);
        }

        private void _BtnEnable(object enable)
        {
            btnWel.Enabled = btnPause.Enabled = btnVoice.Enabled = btnLogout.Enabled = btnEval.Enabled = btnLogout.Enabled = (bool)enable;
            if (contextMenuStrip1.Items.Count > 4 && contextMenuStrip1.Items[3].Text != "正在更新")
            {
                contextMenuStrip1.Items[3].Enabled = (bool)enable;
            }
            if (!MainStaticData.isNetwork)
            {
                btnCut.Enabled = btnCut2.Enabled = (bool)enable;
            }
            else
            {
                if(contextMenuStrip1.Items.Count > 4)
                    contextMenuStrip1.Items[3].Visible = false;
            }
        }

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        /// <summary>
        /// 为了是主界面能够移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private Point mPoint = new Point();

        private void WorkForm_Activated(object sender, EventArgs e)
        {
            //注册热键Shift+S，Id号为100。HotKey.KeyModifiers.Shift也可以直接使用数字4来表示。
            //HotKey.RegisterHotKey(Handle, 100, HotKey.KeyModifiers.Shift, Keys.S);
            //注册热键Ctrl+B，Id号为101。HotKey.KeyModifiers.Ctrl也可以直接使用数字2来表示。
            //HotKey.RegisterHotKey(Handle, 101, HotKey.KeyModifiers.Ctrl, Keys.B);
            //注册热键Alt+D，Id号为102。HotKey.KeyModifiers.Alt也可以直接使用数字1来表示。
            //HotKey.RegisterHotKey(Handle, 102, HotKey.KeyModifiers.Alt, Keys.D);
        }

        //3.判断输入键值（实现KeyDown事件）
        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            string eStr = HotKey.GetStringByKey(e);
            if (null != MainStaticData.keyDict)
            {
                foreach (var item in MainStaticData.keyDict)
                {
                    KeyEventArgs hotkey = item.Value;
                    if (e.KeyValue == hotkey.KeyValue && Control.ModifierKeys == hotkey.Modifiers)
                    {
                        switch (item.Key)
                        {
                            case "欢迎光临":
                                btnWel_Click(null, null);
                                break;
                            case "暂停服务":
                                btnPause_Click(null, null); 
                                break;
                            case "服务评价":
                                btnEval_Click(null, null);
                                break;
                            case "语音播报":
                                btnVoice_Click(null, null); 
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void WorkForm_Leave(object sender, EventArgs e)
        {

        }
        
        /// <summary>
        /// 窗口关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(ctimer != null)
            {
                ctimer.Enabled = false;
                ctimer.Close();
            }
            if (null != KEYKOOK)
                KEYKOOK.Stop();
            this.notifyIcon1.Dispose();
            if (MainStaticData.isNetwork)
            {
                new Thread(StopUdpListen).Start();
            }
        }

        /// <summary>
        /// 清除播放列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string videourl = "http://111.230.14.84:8080/8.mp4";
            USBSendData(String.Format("S09E", videourl), "CutPrint");
        }

        private void btnCut2_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show(Resources.ZHCN_DEVICENOCONN);
                return;
            }
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(delegate ()
                {
                    this.btnPause.Text = "暂停";
                }));
            }
            else
            {
                this.btnPause.Text = "暂停";
            }
            if (btnCut2.Text == "同屏")
            {
                isCuttingVideo = true;
                btnCut2.Text = "停止同屏";
                this.Cut2Timer.Start();
            }
            else //停止
            {
                btnCut2.Text = "同屏";
                if (isCuttingVideo)
                {
                    isCuttingVideo = false;
                    while (isCuttingVideo2)
                        Thread.Sleep(100);
                    this.Cut2Timer.Enabled = false;
                    USBSendData("S08||STOP||E", "StopCutPrint");
                }
            }
        }
        
        private delegate void returnStrDelegate();
        private delegate void returnStrDelegate2(object param);

        //判断一下是不是该用Invoke滴~，不是就直接返回~
        private void returnCB(returnStrDelegate myDelegate)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(myDelegate);
            }
            else
            {
                myDelegate();
            }
        }

        private void returnCB2(returnStrDelegate2 myDelegate, object param)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(myDelegate, param);
            }
            else
            {
                myDelegate(param);
            }
        }
        
        private void CutVideo(object sender, ElapsedEventArgs e)
        {
            Thread.CurrentThread.IsBackground = false;
            try
            {
                isCuttingVideo2 = true;
                if (!isCuttingVideo)
                {
                    isCuttingVideo2 = false;
                    return;
                }

                String dir = "CutVideo";

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                Image img = ScreenCapture.captureScreen();
                //以JPG文件格式来保存
                string imgName = string.Format(@"captureimg.{0:yyyyMMddHHmmss.ffff}.jpg", DateTime.Now);
                img.Save(dir + "/" + imgName, ImageFormat.Jpeg);

                USBSendFile(dir, imgName);

                USBSendData(String.Format("S08||{0}||E", dir + "/" + imgName), "CutPrint");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                isCuttingVideo2 = false;
            }
        }

        private void WorkForm_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                //隐藏任务栏区图标
                this.ShowInTaskbar = false;
                //图标显示在托盘区
                notifyIcon1.Visible = true;
            }
        }
       
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click_2(object sender, EventArgs e)
        {
            TmLogout_Click(sender, e);
        }

        /// <summary>
        /// 最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mPoint.X = e.X;
            mPoint.Y = e.Y;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point myPosittion = MousePosition;
                myPosittion.Offset(-mPoint.X, -mPoint.Y);
                Location = myPosittion;
            }
        }
        
        private void WorkForm_LocationChanged(object sender, EventArgs e)
        {
            if (this.Top <= 0)
            {
                isRect = true;
                this.ShowIcon = this.ShowInTaskbar = false;
                this.stopAnchor = AnchorStyles.Top;
            }
            else if (this.Bottom >= Screen.PrimaryScreen.Bounds.Height)
            {
                isRect = true; this.ShowIcon = this.ShowInTaskbar = false;
                this.stopAnchor = AnchorStyles.Bottom;
            }
            else if (this.Left <= 0)
            {
                isRect = true;
                this.ShowIcon = this.ShowInTaskbar = false;
                this.stopAnchor = AnchorStyles.Left;
            }
            else if (this.Left >= Screen.PrimaryScreen.Bounds.Width - this.Width)
            {
                isRect = true;
                this.ShowIcon = this.ShowInTaskbar = false;
                this.stopAnchor = AnchorStyles.Right;
            }
            else
            {
                isRect = false; this.ShowIcon = this.ShowInTaskbar =true;
                this.stopAnchor = AnchorStyles.None;
            }
        }
    }
}
