﻿using Newtonsoft.Json.Serialization;
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
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Serialization;
using ZXClient.control;
using ZXClient.dao;
using ZXClient.model;
using ZXClient.Properties;
using ZXClient.util;
using System.ComponentModel;
using ZXClient.service;

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
        //public static ADBClient MyClient = new ADBClient();
        IList<string> DeviceList;
        
        public static System.Timers.Timer GetEvalResultTimer;

        public static string mID = ""; //mac地址
        public static string mDeviceVer = "";
        public static string mDeviceNVer = "";
        public static string mAndroidVer = "";
        public static string mPrdCode = "";
        public static bool isConnected = false;
        internal static KeyboardHook KEYKOOK;
        IPEndPoint ipendpoint;
        internal static UdpClient udpclient;
        Thread UdpThread;
        static bool isRect = false;//是否处于吸附状态
        System.Timers.Timer workLoopTimer;
        public static System.Timers.Timer heartTimer;
        static bool isCuttingVideo = false, isCuttingVideo2 = false;//是否正在同屏
        static bool isForwardPort = false;//本地端口是否转发成功
        
        public WorkForm()
        {
            InitializeComponent();
            this.TopMost = true;
            CommonHelper.SetMid(this);

            m_SyncContext = SynchronizationContext.Current;
            KEYKOOK = new KeyboardHook();
            KEYKOOK.Start(OnKeyPress);//安装键盘钩子

            InitRightMenu();
            InitUI();

            this.Text = Resources.CLIENTNAME;
            this.notifyIcon1.Text = Resources.CLIENTNAME;

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            SetVisibleCore(false);

            //自动吸附
            MainData.RECTTIMER = new System.Timers.Timer();
            MainData.RECTTIMER.Interval = 300;
            MainData.RECTTIMER.Elapsed += StopRectTimer_Tick;
            MainData.RECTTIMER.Start();
        }

        private void WorkForm_Load(object sender, EventArgs e)
        {
            //同屏
            Cut2Timer = new System.Timers.Timer(MainData.CUTVIDEOPERSECOND);//实例化Timer类，设置间隔时间为10000毫秒； 
            Cut2Timer.Elapsed += new System.Timers.ElapsedEventHandler(CutVideo);//到达时间的时候执行事件； 
            Cut2Timer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)； 
            Cut2Timer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件； 

            workLoopTimer = new System.Timers.Timer();
            workLoopTimer.Interval = 10000;
            workLoopTimer.Elapsed += new System.Timers.ElapsedEventHandler(workLoop_Tick);
            workLoopTimer.Start();

            try
            {
                Tools.killadb();
                MainData.udpHelper = new AsyncUdpSever(USBPJQListener2, MainData.udpPort);
                Thread t = new Thread(new ThreadStart(MainData.udpHelper.ReceiveMsg));
                t.Start();
            }
            catch (SocketException ex)
            {
                Tools.ShowInfo2("呼叫器监听建立失败1:" + ex.Message);
                MessageBox.Show(this, "呼叫器监听失败:端口" + MainData.udpPort + "被占用,请与管理员联系");
                LogHelper.WriteError(TAG, ex);
            }
            catch (Exception ex)
            {
                Tools.ShowInfo2("呼叫器监听建立失败1:" + ex.Message);
                MessageBox.Show(this, "呼叫器监听建立失败");
                LogHelper.WriteError(TAG, ex);
            }
            new Thread(ConnDevice).Start(false);//连接设备
            new LYLWListen();
        }

        private void listenQueue()
        {
            if (!MainData.isNetwork)
            {
                try
                {
                    Tools.ShowInfo2("监听 (软件呼叫器) 端口:" + MainData.udpPort);
                    ipendpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), MainData.udpPort);
                    udpclient = new UdpClient(ipendpoint);
                    //udpclient.ExclusiveAddressUse = false;
                    //udpclient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    //udpclient.Client.Bind(ipendpoint);

                    IsUdpcRecvStart = true;
                    UdpThread = new Thread(USBPJQListener);
                    UdpThread.IsBackground = true;
                    UdpThread.Start(udpclient);
                }
                catch (SocketException ex)
                {
                    Tools.ShowInfo2("呼叫器监听建立失败1:" + ex.Message);
                    MessageBox.Show(this, "呼叫器监听失败:端口" + MainData.udpPort + "被占用,请与管理员联系");
                    LogHelper.WriteError(TAG, ex);
                }
                catch (Exception ex)
                {
                    Tools.ShowInfo2("呼叫器监听建立失败1:" + ex.Message);
                    MessageBox.Show(this, "呼叫器监听建立失败");
                    LogHelper.WriteError(TAG, ex);
                }
            }
        }

        int OnKeyPressinTimer = 0;
        private void OnKeyPress(KeyEventArgs e, out bool handle)
        {
            handle = false;
            try
            {
                string eStr = HotKey.GetStringByKey(e);
                if (null != MainData.keyDict)
                {
                    foreach (var item in MainData.keyDict)
                    {
                        KeyEventArgs hotkey = item.Value;
                        if (e.KeyValue == hotkey.KeyValue && ModifierKeys == hotkey.Modifiers)
                        {
                            handle = true;
                            if (OnKeyPressinTimer != 0)
                            {
                                return;
                            }
                            if (0 == 0)
                            {
                                OnKeyPressinTimer = 1;
                                #region ACTION
                                switch (item.Key)
                                {
                                    case "欢迎光临":
                                        if (this.btnWel.Enabled)
                                        {
                                            btnWel_Click(null, null);
                                            handle = true;
                                        }
                                        break;
                                    case "暂停服务":
                                        if (this.btnPause.Enabled)
                                        {
                                            btnPause_Click(null, null); handle = true;
                                        }
                                        break;
                                    case "服务评价":
                                        if (btnEval.Enabled)
                                        {
                                            btnEval_Click(null, null); handle = true;
                                        }
                                        break;
                                    case "语音播报":
                                        if (this.btnVoice.Enabled)
                                        {
                                            btnVoice_Click(null, null); handle = true;
                                        }
                                        break;
                                    case "截屏":
                                        if (this.btnCut.Enabled)
                                        {
                                            btnCut_Click(null, null); handle = true;
                                        }
                                        break;
                                    case "同屏":
                                        if (this.btnCut2.Enabled)
                                        {
                                            btnCut2_Click(null, null); handle = true;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                #endregion
                                new DelegateName(new System.Action(delegate
                                {
                                    Thread.Sleep(1000);
                                    OnKeyPressinTimer = 0;
                                })).BeginInvoke(null, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                handle = false;
            }
        }

        /// <summary>
        /// 通知区域右键菜单
        /// </summary>
        private void InitRightMenu()
        {
            IDictionary<string,string> keyD = db_KeyConfig.getKeyConfig(MainData.USERCARD);
            
            if (null != keyD)
            {
                MainData.keyDict = new Dictionary<string, KeyEventArgs>();
                foreach (var item in keyD)
                {
                    KeyEventArgs hotkey = HotKey.GetKeyByString(item.Value);
                    MainData.keyDict.Add(item.Key, hotkey);
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

            contextMenuStrip1.Items.Add("当前版本-" + System.Configuration.ConfigurationManager.AppSettings["version"]);
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

            System.Action caller = new System.Action(delegate
            {
                String data = String.Format("mac={0}&config=true&androidVer={1}&prdcode={2}", mID, WorkForm.mAndroidVer, WorkForm.mPrdCode); //检查资源版本号
                String ret = HttpUtil.RequestData(MainData.ServerAddr + MainData.INET_GETUPDATEVERSION, data).Replace("Version=", "");
                Tools.ShowInfo2("强制更新-评价器更新资源版本：" + mDeviceNVer + ", 服务器最新版本：" + ret);
                int mServerVer;//服务器版本号
                if (int.TryParse(ret, out mServerVer))
                {
                    db_ResUpdateDao.update(mID, mServerVer, 0);
                    mDeviceNVer = "0";
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
                    DialogResult dr = MessageBox.Show(this, "确定要退出吗?", "退出系统", messButton);

                    if (dr == DialogResult.OK)//如果点击“确定”按钮
                    {
                        if (isCuttingVideo)
                        {
                            isCuttingVideo = false;
                            while (isCuttingVideo2)
                                Thread.Sleep(100);
                            this.Cut2Timer.Enabled = false;
                            Tools.USBSendData("S08||STOP||E", "StopCutPrint");
                        }
                        this.notifyIcon1.Dispose();
                        IsUdpcRecvStart = false;
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
                            Tools.USBSendData("S08||STOP||E", "StopCutPrint");
                        }
                        //USBSendData("S08||STOP||E", "StopCutPrint");
                        //db_EmployeeLoginDao.logout(MainStaticData.USERCARD);
                        MainData.Restart();
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
            if (MainData.cbNoLogin)
            {
                MessageBox.Show(this, "免登录模式，不允许修改密码");
                return;
            }
            EditPwdFrm frm = new EditPwdFrm();
            frm.Show();
            this.Visible = false;
            MainData.wf = this;
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
            MainData.wf = this;
        }
        
        /// <summary>
        /// 配置页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void configSet_Click(object sender, EventArgs e)
        {
            ConfigSet setForm = new ConfigSet();
            DialogResult dr = setForm.ShowDialog();
            MainData.wf = this;
        }

        private void ConnDevice(object isreConn)
        {
            if (MainData.isNetwork)
            {
                if (!IsUdpcRecvStart)
                {
                    try
                    {
                        ipendpoint = new IPEndPoint(IPAddress.Any, MainData.udpRecivePort);
                        udpclient = new UdpClient(ipendpoint);
                        IsUdpcRecvStart = true;
                        UdpThread = new Thread(NetworkListener);
                        UdpThread.IsBackground = true;
                        UdpThread.Start(udpclient);
                    }
                    catch (SocketException ex)
                    {
                        Tools.ShowInfo2("评价器监听失败:" + ex.Message);
                        isConnected = false;
                        LogHelper.WriteError(TAG, ex);
                        //this.Invoke(new System.Action(delegate()
                        //{
                            MessageBox.Show(this, "评价器监听失败:端口" + MainData.udpRecivePort + "被占用,请与管理员联系");
                        //}));
                        return;
                    }
                    catch (Exception ex)
                    {
                        Tools.ShowInfo2("端口监听失败3！");
                        isConnected = false;
                        LogHelper.WriteError(TAG, ex);
                        return;
                    }
                    Tools.ShowInfo2("端口监听成功");
                }
                Tools.ShowInfo2("网络方式--心跳");
                //ConnTimer_Tick(null, null);
                //ConnTimer.Enabled = true;
                while (!isConnected)
                {
                    Tools.ShowInfo2("网络方式--设备连接状态=" + isConnected);
                    Thread.Sleep(1000);
                }
                Tools.ShowInfo2("网络方式--设备连接状态=" + isConnected);
                String recvData = Tools.SendUDP(String.Format("S98{0},{1},{2},{3},{4},{5}E", 1, MainData.ServerIP, MainData.FtpIP, MainData.FtpPort, MainData.FtpUserName, MainData.FtpPwd)); //修改连接方式为网络连接1
                Tools.ShowInfo2("S98返回=" + recvData);
                if (recvData == "S98OK")
                {
                    EmployeeLogin(false, null, null);
                }
            }
            else //USB连接
            {
                ADBClient MyClient = new ADBClient();
                MyClient.AdbPath = MainData.AdbExePath;
                MyClient.StartServer();

                DeviceList = MyClient.Devices();
                if (DeviceList.Count == 0)
                {
                    if (IsHandleCreated && InvokeRequired)
                    {
                        this.Invoke(new System.Action(delegate
                        {
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

                    Tools.ShowInfo2("没有检测到评价器！");
                    isConnected = false;
                    if (!isConnected)
                    {
                        Thread.Sleep(5000);
                        ConnDevice(isreConn);
                    }
                }
                else
                {
                    Tools.ShowInfo2("检测到USB设备,可以进行端口转发了.");
                    if (!isForwardPort)
                    {
                        MyClient.Forward(MainData.forwardPort, MainData.devicePort);
                        Tools.ShowInfo2(String.Format("设备端口{0}重定向到本地端口{1}",
                            MainData.devicePort, MainData.forwardPort));
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        try
                        {
                            socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), MainData.forwardPort));
                            Tools.ShowInfo2("测试连接重定向端口成功");
                            Tools.isUsbConnected = true;
                            isForwardPort = true;
                            isConnected = true;
                            socket.Close();
                            socket = null;
                        }
                        catch (SocketException)
                        {
                            Tools.ShowInfo2("***测试连接重定向端口失败***");
                            Tools.isUsbConnected = false;
                        }
                        finally
                        {
                            if (socket != null)
                            {
                                socket.Close();
                                socket = null;
                            }
                        }
                    }
                    while (!isConnected)
                    {
                        Thread.Sleep(100);
                    }
                    Tools.ShowInfo2("设备连接成功");
                    if (isConnected)
                    {
                        List<string> data2 = Tools.USBSendData("S99||0||E", "getDeviceInfo");//获取设备信息
                        if (data2 != null && data2.Count > 0)
                        {
                            String[] d = data2[data2.Count - 1].Split(new string[] { "||" }, StringSplitOptions.None);
                            if (d.Length > 2)
                            {
                                mID = d[2];
                                mDeviceVer = d[1].Replace("Version=", "");
                                mDeviceNVer = d[7];
                                WorkForm.mAndroidVer = d[8];
                                WorkForm.mPrdCode = d[9];
                                Tools.ShowInfo2(data2[data2.Count - 1] + "获取设备mac地址:" + mID + "，设备资源版本号：" + mDeviceVer + "，设备更新资源版本号：" + mDeviceNVer + ", len:" + d.Length);
                                isConnected = true;
                                BtnEnable(true);
                            }
                        }

                        if (!(bool)isreConn)
                        {
                            bool islogin = EmployeeLogin(false, null, null);
                            while (!islogin && !MainData.cbNoLogin)
                            {
                                Thread.Sleep(10000);
                                ConnDevice(isreConn);
                            }
                        }

                        Tools.USBSendData(String.Format("S98||{0}{1}||E", 0, MainData.ServerIP), "changeConnType");//修改连接方式
                        Tools.ShowInfo2("开始心跳" + IsUdpcRecvStart);
                        workLoopTimer.Enabled = true;
                    }
                }
            }
        }
        
        Socket USBDeviceSocket;

        private Boolean BindSocketUSB()
        {
            if(USBDeviceSocket!=null && USBDeviceSocket.Connected)
            {
                USBDeviceSocket.Shutdown(SocketShutdown.Both);
                USBDeviceSocket.Close();
            }
            USBDeviceSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                USBDeviceSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), MainData.forwardPort));
                return true;
            }
            catch (SocketException)
            {
                Tools.ShowInfo2("评价器连接失败1");
                return false;
            }
        }

        private Boolean EmployeeLogin(Boolean isPJQ, UdpClient client, IPEndPoint ipendpoint)
        {
            String d = "";
            if (MainData.isNetwork)
            {
                if (String.IsNullOrEmpty(MainData.USERCARD))
                {
                    return false;
                }
                else
                {
                    Tools.SendUDP(String.Format("S04{0}E", MainData.USERCARD));
                    return true;
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(MainData.USERCARD))
                {
                    String employeeData = HttpUtil.RequestData(MainData.ServerAddr + MainData.INTE_EMPLOYEEGETINFO, "cardnum=" + MainData.USERCARD);
                    UserModel model = null;
                    try
                    {
                        model = JsonHelper.DeserializeJsonToObject<UserModel>(employeeData);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteError(TAG, ex);
                        Tools.ShowInfo2("用户登录失败-服务器出错！");
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
                            bool down = HttpUtil.DownloadFile(MainData.ServerAddr + model.picture, "picture/user_" + model.ext2 + ".jpg");
                            if (down)
                            {
                                string picpath = System.Environment.CurrentDirectory + "\\picture\\user_" + model.ext2 + ".jpg";
                                new ADBClient().Push(picpath, MainData.SDCARD + "user_" + model.ext2 + ".jpg");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteError(TAG, ex);
                        Tools.ShowInfo2("用户照片下载失败！");
                        return false;
                    }

                    try
                    {
                        if (null != model)
                        {
                            StreamReader infosetdown = HttpUtil.GetStream(MainData.ServerAddr + MainData.INTE_EMPLOYEEINFOSETDOWN);

                            d = MainData.USERCARD;
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
                            List<string> data2 = Tools.USBSendData(String.Format("S04||{0}||E", d), "login", isPJQ, client, ipendpoint);
                            if (data2.Count > 0 && data2[data2.Count - 1] == "RecvCmdOK")
                                return true;
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteError(TAG, ex);
                        Tools.ShowInfo2("用户配置信息下载失败！");
                        return false;
                    }
                }
                
                
                return false;
            }
        }

        public bool IsUdpcRecvStart = false;

        /// <summary>
        /// 监听UDP返回消息－网络方式
        /// </summary>
        public void NetworkListener(object obj)
        {
            UdpClient client = obj as UdpClient;
            try
            {
                IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 0);
                Tools.ShowInfo2("开始监听（UDP）评价器返回数据，端口：7791," + IsUdpcRecvStart);
                while (IsUdpcRecvStart && client != null)
                {
                    if (client.Available <= 0) continue;
                   
                    byte[] bytes = client.Receive(ref remoteIpep);
                    string strInfo = MainData.encode.GetString(bytes, 0, bytes.Length);
                    Tools.ShowInfo2("评价器返回：" + strInfo);
                    if (strInfo == "DisConnect")
                    {
                        isConnected = false;
                        return;
                    }
                    if (strInfo == "S04OK")//登录成功
                    {
                        Tools.ShowInfo2("登录成功!");
                        isConnected = true;
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new System.Action(delegate()
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
                        Tools.ShowInfo2("退出成功！");
                        IsUdpcRecvStart = false;
                        new Thread(StopUdpListen).Start();
                    }
                    if (strInfo == "S98OK")//修改评价器配置成功
                    {
                        isConnected = true;
                        Tools.ShowInfo2("连接评价器成功！");
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
                if (null != client)
                {
                    client.Close();
                    client = null;
                }
                
            }
        }

        public void USBPJQListener(object obj)
        {
            UdpClient client = obj as UdpClient;
            try
            {
                //IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 8001);
                //IPEndPoint remoteIpep = (IPEndPoint)client.Client.LocalEndPoint;
                Tools.ShowInfo2("开始监听（排队叫号器）评价器返回数据，端口：8001" + IsUdpcRecvStart);
                while (IsUdpcRecvStart && client !=null && client.Client.Poll(-1, SelectMode.SelectRead))
                {
                    try
                    {
                        IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 0);
                        byte[] bytes = client.Receive(ref remoteIpep);
                        string strInfo = MainData.encode.GetString(bytes, 0, bytes.Length);
                        Tools.ShowInfo2("排队叫号器指令：" + strInfo);
                        PjqModel o = JsonHelper.DeserializeJsonToObject<PjqModel>(strInfo);
                        Tools.ShowInfo2("排队叫号器指令：" + o.command);

                        byte[] data = MainData.encode.GetBytes(JsonHelper.SerializeObject(new { succ = "0" }));
                        int rr = client.Send(data, data.Length, remoteIpep);

                        if (o.command == "login")
                        {
                            MainData.USERCARD = o.sno;
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
                    catch (Exception e)
                    {

                        LogHelper.WriteError(TAG, e);
                        Tools.ShowInfo2(e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteError(TAG, e);
                Tools.ShowInfo2(e.ToString());
            }
            finally
            {
                if (null != client)
                {
                    client.Close();
                    client = null;
                    UdpThread.Abort();
                }
            }
        }

        private string encode2UTF8(string str)
        {
            Encoding utf8 = Encoding.UTF8;
            Encoding ISO = Encoding.Default;
            byte[] temp = ISO.GetBytes(str);
            return utf8.GetString(temp);
        }

        public void USBPJQListener2(String strInfo, UdpClient client, IPEndPoint remoteIpep)
        {
            try
            {
                strInfo = encode2UTF8(strInfo);
                Tools.ShowInfo2("排队叫号器指令：" + strInfo);
                PjqModel o = JsonHelper.DeserializeJsonToObject<PjqModel>(strInfo);
                Tools.ShowInfo2("排队叫号器指令：" + o.command);

                byte[] data = MainData.encode.GetBytes(JsonHelper.SerializeObject(new { succ = "0" }));
                int rr = client.Send(data, data.Length, remoteIpep);

                if (o.command == "login")
                {
                    MainData.USERCARD = o.sno;
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
            catch (Exception e)
            {
                LogHelper.WriteError(TAG, e);
                Tools.ShowInfo2(e.ToString());
            }
        }

        private void StopUdpListen()
        {
            Tools.ShowInfo2("销毁udp监听进程");
            if (udpclient != null)
                udpclient.Close();
            if(UdpThread!=null)
                UdpThread.Abort();
            this.notifyIcon1.Dispose();
            System.Environment.Exit(0);
        }

        /// <summary>
        ///  
        /// </summary>
        private void BeginGetEvalResult(Boolean isPJQ, UdpClient client, IPEndPoint endPoint)
        {
            GetEvalResultTimer = new System.Timers.Timer();
            GetEvalResultTimer.Interval = 1000;
            GetEvalResultTimer.Elapsed += (sender, e) => { Tools.USBSendData("SE", "evalwait", isPJQ, client, endPoint); };//GetEvalResult_Tick;
            GetEvalResultTimer.Enabled = true;
        }
        
        //定义与方法同签名的委托  
        private delegate void DelegateName();
        int inTimer = 0;
        int inTimer2 = 0;

        private void workLoop_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Interlocked.Exchange(ref inTimer, 1) == 0)
                {
                    if (!MainData.isNetwork)
                    {
                        ADBClient MyClient = new ADBClient();
                        MyClient.AdbPath = MainData.AdbExePath;
                        DeviceList = MyClient.Devices();
                        Console.WriteLine("DeviceList="+DeviceList.Count);
                        if (DeviceList.Count ==0)
                        {
                            isForwardPort = false;
                            BtnEnable(false);
                            ConnDevice(true);
                        }
                    }
                    Tools.HeartTicket();
                    BtnEnable(WorkForm.isConnected);

                    #region USB
                    if (!MainData.isNetwork && isConnected) {
                        #region 检查资源更新
                        _checkupdateres();
                        #endregion

                        #region 检查游屏信息
                        String data = String.Format("mac={0}", mID);
                        String ret = HttpUtil.RequestData(MainData.ServerAddr + MainData.INTE_GETDEPTNOTICE, data);
                        Tools.USBSendData(String.Format("S15||{0}||E", ret), "Notice");
                        #endregion

                        #region 不满意按键
                        data = String.Format("mac={0}", mID);
                        ret = HttpUtil.RequestData(MainData.ServerAddr + MainData.INTE_GETDISSATISFIEDKEY, data);
                        Tools.USBSendData(String.Format("S18||{0}||E", ret), "DISSATISKEY");
                        #endregion

                        #region 信息查询更新
                        data = String.Format("?mac={0}", mID);
                        HttpUtil.DownloadFile(MainData.ServerAddr + MainData.INTE_WEBURLDOWNLOAD + data, "quary.xml");
                        Tools.USBSendFile("", System.Environment.CurrentDirectory + "\\quary.xml", "quary.xml");
                        #endregion

                        #region 通知公告更新
                        HttpUtil.DownloadFile(MainData.ServerAddr + MainData.INTE_NOTICEDOWNLOAD, "notice.xml");
                        Tools.USBSendFile("", System.Environment.CurrentDirectory + "\\notice.xml", "notice.xml");
                        #endregion

                        #region 评价按键更新
                        if(HttpUtil.DownloadFile(MainData.ServerAddr + MainData.INTE_GETEVALBUTTONS, "button.xml"))
                        {
                            Tools.USBSendFile("", System.Environment.CurrentDirectory + "\\button.xml", "eval/button.xml");
                        }
                        else
                        {
                            #region 评价按钮
                            ret = HttpUtil.RequestData(MainData.ServerAddr + MainData.INTE_GETKEYALL, "");
                            Tools.USBSendData(String.Format("S51||{0}||E", ret), "GETKEYALL");
                            #endregion
                        }
                        #endregion

                        #region 意见调查
                        HttpUtil.DownloadFile(MainData.ServerAddr + MainData.INTE_ADVICEDOWNLOAD, "advice.db");
                        Tools.USBSendFile("", System.Environment.CurrentDirectory + "\\advice.db", "advise.ItemListener");
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
                String data = String.Format("mac={0}&config=true&androidVer={1}&prdcode={2}", mID, WorkForm.mAndroidVer, WorkForm.mPrdCode); //检查资源版本号
                String ret = HttpUtil.RequestData(MainData.ServerAddr + MainData.INET_GETUPDATEVERSION, data).Replace("Version=", "");
                Tools.ShowInfo2("评价器更新资源版本：" + mDeviceNVer + ", 服务器最新版本：" + ret);
                int mServerVer;//服务器版本号
                if (mDeviceNVer != ret && int.TryParse(ret, out mServerVer))
                {
                    int? laststate = db_ResUpdateDao.selectlast(mID, mServerVer);
                    if (laststate == null)
                        db_ResUpdateDao.add(mID, mServerVer, 0);//资源未下载

                    if (laststate == null || laststate == 0)//未下载
                    {
                        data = String.Format("mac={0}&config=false", mID); //下载更新资源包
                        bool isDown = HttpUtil.DownloadFile(MainData.ServerAddr + MainData.INET_GETUPDATEVERSION + "?" + data, "M7Update.zip");
                        if (isDown)
                        {
                            if (db_ResUpdateDao.update(mID, mServerVer, 1) == 1)//资源下载成功
                            {
                                laststate = 1;
                            }
                            else
                            {
                                Tools.ShowInfo2("升级资源下载到本地后，更新本地数据库失败。");
                            }
                        }
                    }
                    String filename = "M7Update.zip";
                    FileInfo updateFile = new FileInfo(filename);
                    if (laststate == 1)
                    {
                        if (!updateFile.Exists)
                        {
                            Tools.ShowInfo2("升级资源文件发送失败，文件不存在。");
                            db_ResUpdateDao.update(mID, mServerVer, 0);
                            return;
                        }
                        Tools.USBSendFile("", filename);
                        Tools.USBSendData("S97||" + mServerVer + "," + (mServerVer - 1) + "||E", "changeResVersion");
                        if (db_ResUpdateDao.update(mID, mServerVer, 2) == 1)//推送成功
                        {
                            laststate = 2;
                        }
                        else
                        {
                            Tools.ShowInfo2("更新到推送到评价器成功，但更新本地数据库失败。");
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
                this.Show();
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

        System.Timers.Timer Cut2Timer;
        
        internal AnchorStyles stopAnchor = AnchorStyles.None;

        private void StopRectTimer_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.Invoke(new System.Action(delegate()
                {
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
                this.Invoke(new System.Action(delegate()
                {
                    this.btnPause.Text = "暂停";
                }));
            }
            else
            {
                this.btnPause.Text = "暂停";
            }
            AsyncBtn(this.btnWel, new System.Action(delegate() { ActionWel(false, null, null); }));
        }

        private void ActionWel(bool isPJQ, UdpClient client, IPEndPoint remoteIpep)
        {
            if (MainData.isNetwork)
                Tools.SendUDP("S01E");
            else
                Tools.USBSendData("S01E", "welcome", isPJQ, client, remoteIpep);
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
                MessageBox.Show(this, Resources.ZHCN_DEVICENOCONN);
                return;
            }
            AsyncBtn(this.btnPause, new System.Action(delegate
            {
                ActionPause(null, null);
            }), 1000);
        }

        private void ActionPauseReset(UdpClient client, IPEndPoint remoteIpep)
        {
            if (MainData.isNetwork)
            {
                if (btnPause.Text.ToString() == "取消暂停")
                {
                    Tools.SendUDP("S06E");
                    if (this.InvokeRequired)
                        this.Invoke(new System.Action(delegate
                        {
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
                    List<String> rst = Tools.USBSendData("S25E", "pause", client != null, client, remoteIpep);
                    foreach (var item in rst)
                    {
                        if (item == "RecvCmdOK")
                        {
                            this.InvokeMethod(new System.Action(delegate { btnPause.Text = "暂停"; }));
                        }
                    }

                }
            }
        }

        private void ActionPauseOnly(UdpClient client, IPEndPoint remoteIpep)
        {
            if (MainData.isNetwork)
            {
                if (this.btnPause.Text.ToString() == "暂停")
                {
                    string rst = Tools.SendUDP("S03E");
                    Console.WriteLine("命令返回结果:" + rst);
                    if (this.InvokeRequired)
                        this.Invoke(new System.Action(delegate
                        {
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
                    List<String> rst = Tools.USBSendData("S03E", "pause", client != null, client, remoteIpep);
                    foreach (var item in rst)
                    {
                        if (item == "RecvCmdOK")
                        {
                            this.InvokeMethod(new System.Action(delegate { btnPause.Text = "取消暂停"; }));
                        }
                    }
                }
            }
        }

        private void ActionPause(UdpClient client, IPEndPoint remoteIpep)
        {
            if (MainData.isNetwork)
            {
                if (this.btnPause.Text.ToString() == "暂停")
                {
                    string rst = Tools.SendUDP("S03E");
                    if(this.InvokeRequired)
                        this.Invoke(new System.Action(delegate
                        {
                            btnPause.Text = "取消暂停";
                        }));
                    else
                        btnPause.Text = "取消暂停";
                }
                else if (btnPause.Text.ToString() == "取消暂停")
                {
                    Tools.SendUDP("S06E");
                    if (this.InvokeRequired)
                        this.Invoke(new System.Action(delegate
                        {
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
                    List<String> rst = Tools.USBSendData("S03E", "pause", client != null, client, remoteIpep);
                    if (rst!=null)
                    {
                        foreach (var item in rst)
                        {
                            if (item == "RecvCmdOK")
                            {
                                this.InvokeMethod(new System.Action(delegate { btnPause.Text = "取消暂停"; }));
                            }
                        }
                    }
                    if (btnCut2.Text == "停止同屏" && btnCut2.Enabled)
                    {
                        btnCut2_Click(null, null);
                    }
                }
                else if (btnPause.Text.ToString() == "取消暂停")
                {
                    List<String> rst = Tools.USBSendData("S25E", "pause", client != null, client, remoteIpep);
                    if (rst!=null)
                    {
                        foreach (var item in rst)
                        {
                            if (item == "RecvCmdOK")
                            {
                                this.InvokeMethod(new System.Action(delegate { btnPause.Text = "暂停"; }));
                            }
                        }
                    }
                }
            }
        }

        private void InvokeMethod(System.Action method) {
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
                MessageBox.Show(this, Resources.ZHCN_DEVICENOCONN);
                return;
            }
            if (this.InvokeRequired)
            {
                this.Invoke(new System.Action(delegate()
                {
                    this.btnPause.Text = "暂停";
                }));
            }
            else
            {
                this.btnPause.Text = "暂停";
            }
            AsyncBtn(this.btnEval, new System.Action(delegate
            {
                ActionEval(false, null, null);
            }));
        }

        private void AsyncBtn(Button btn, System.Action action, int sleep = 5000)
        {
            new DelegateName(new System.Action(delegate
            {
                if (btn.InvokeRequired == false)
                {
                    btn.ForeColor = ColorTranslator.FromHtml("#d5d5d5");
                    btn.Enabled = false;
                }
                else
                {
                    btn.Invoke(new System.Action(delegate
                    {
                        btn.ForeColor = ColorTranslator.FromHtml("#d5d5d5");
                        btn.Enabled = false;
                    }));
                }
            })).BeginInvoke(null, null);
            
            new DelegateName(action).BeginInvoke(null, null);

            new DelegateName(new System.Action(delegate
            {
                Thread.Sleep(sleep);
                if (btn.InvokeRequired == false)
                {
                    if (isConnected)
                    {
                        btn.Enabled = true;
                        btn.ForeColor = Color.White;
                    }
                }
                else
                {
                    btn.Invoke(new System.Action(delegate
                    {
                        if (isConnected)
                        {
                            btn.Enabled = true;
                            btn.ForeColor = Color.White;
                        }
                    }));
                }
            })).BeginInvoke(null, null);
        }

        private void ActionEval(Boolean isPJQ, UdpClient client, IPEndPoint endPoint)
        {
            if (MainData.isNetwork)
            {
                Tools.SendUDP("S02E");
            }
            else
            {
                
                Tools.USBSendData("S02E", "eval", isPJQ, client, endPoint);
                if (GetEvalResultTimer != null && GetEvalResultTimer.Enabled)
                    GetEvalResultTimer.Stop();
                BeginGetEvalResult(isPJQ, client, endPoint);
            }
        }

        private void ActionCall(string number, string servicename, Boolean isPJQ, 
            UdpClient client, IPEndPoint endPoint)
        {
            if (MainData.isNetwork)
            {
                
            }
            else
            {
                Tools.USBSendData("S96||" + number + "," + servicename + "||E", "call", isPJQ, client, endPoint);
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
            if (MainData.isNetwork)
            {
                Tools.SendUDP("S94STOPE");
                if (isConnected)
                    Tools.SendUDP("S05E");
                if (!isPJQ)
                    StopUdpListen();
            }
            else
            {
                if (isCuttingVideo)
                {
                    isCuttingVideo = false;
                    while (isCuttingVideo2)
                        Thread.Sleep(100);
                    this.Cut2Timer.Enabled = false;
                    Tools.USBSendData("S08||STOP||E", "StopCutPrint");
                }
                this.IsUdpcRecvStart = false;
                if (!isPJQ && null != udpclient)
                {
                    udpclient.Close();
                    udpclient = null;
                    if (udpclient != null)
                        udpclient.Close();
                    if (UdpThread != null)
                        UdpThread.Abort();
                }

                Tools.USBSendData("S08||STOP||E", "StopCutPrint");
                List<string> receiveData = Tools.USBSendData("S05E", "logout", isPJQ, client, endPoint);
                if (null != receiveData && receiveData.Count>0 && receiveData[0].Contains("RecvCmdOK"))//退出成功
                {
                    Tools.ShowInfo2("用户退出成功！");
                }
                if (!isPJQ)
                {
                    Tools.killadb();
                    Thread.Sleep(1500);
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
                MessageBox.Show(this, Resources.ZHCN_DEVICENOCONN);
                return;
            }
            if (this.InvokeRequired)
            {
                this.Invoke(new System.Action(delegate()
                {
                    this.btnPause.Text = "暂停";
                }));
            }
            else
            {
                this.btnPause.Text = "暂停";
            }
            VoiceText frm = VoiceText.GetInstance();
            if (frm != null)
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.Focus();
                DialogResult dr = frm.ShowDialog();
                if (dr == DialogResult.OK && frm.Value.Length > 0)
                {
                    if (MainData.isNetwork)
                    {
                        Tools.SendUDP("S09" + frm.Value + "E");
                    }
                    else
                    {
                        Tools.USBSendData(String.Format("S06||{0}||E", frm.Value), "Voice");
                    }
                    frm.Close();
                    frm.Dispose();
                }
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
                MessageBox.Show(this, Resources.ZHCN_DEVICENOCONN);
                return;
            }
            if (this.InvokeRequired)
            {
                this.Invoke(new System.Action(delegate()
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
                btnCut.Text = "取消截屏";
                GetScreenOnFocus();

                CutPopUp cut_form = new CutPopUp(CutScreen(screen_on_focus.Bounds.Left, screen_on_focus.Bounds.Top, screen_on_focus.Bounds.Width, screen_on_focus.Bounds.Height), screen_on_focus,previous_selection, this, 1);
                cut_form.OnCut += (s, e) =>
                {
                    if (!isRect)
                    {
                        this.Show(); this.Focus();
                    }
                    previous_selection = e.Selection;
                    //imgsrc = e.img; 

                    if (MainData.isNetwork)
                    {
                        //string imgName = imgsrc.Substring(imgsrc.IndexOf(@"\") + 1);
                        NetWorkCutImg(e.img);
                        //Tools.SendUDP("S94"+ imgName + "E");
                    }
                    else
                    {
                        byte[] fssize = ImageHelper.ImageToBytes(e.img);
                        
                        byte[] bLength = Encoding.Default.GetBytes(("S08" + fssize.Length).PadRight(100, ' '));
                        object[] sendStream = new Object[2] {bLength, fssize};

                        Tools.USBSendData(sendStream, "USBCutVideo");

                        //string imgName = imgsrc.Substring(imgsrc.IndexOf(@"\") + 1);
                        //USBSendFile("CutImg", imgName);
                        //FileInfo f = new FileInfo("CutImg/" + imgName);
                        //if (f.Exists)
                        //{
                        //    f.Delete();
                        //}
                        //USBSendData(String.Format("S08||{0}||E", "CutImg/" + imgName), "CutPrint");
                    }
                };
                cut_form.Show();
            }
            else
            {
                if (MainData.isNetwork)
                {
                    Tools.SendUDP("S94STOPE");
                }
                else
                {
                    Tools.USBSendData("S08||STOP||E", "StopCutPrint");
                }
                btnCut.Text = "截屏";
            }
        }

        #region 截图
        Screen screen_on_focus;
        Rectangle previous_selection;
        //private Image imgsrc;

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
            if (MainData.isNetwork && 1==2)
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
            if (!MainData.isNetwork || 1==1)
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
            btnCut.Enabled = btnCut2.Enabled = (bool)enable;
            //btnCut.Enabled = btnCut2.Enabled = true;
            if (contextMenuStrip1.Items.Count > 4 && contextMenuStrip1.Items[3].Text != "正在更新")
            {
                contextMenuStrip1.Items[3].Enabled = (bool)enable;
            }
            if (!MainData.isNetwork)
            {
                
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
        
        //3.判断输入键值（实现KeyDown事件）
        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            string eStr = HotKey.GetStringByKey(e);
            if (null != MainData.keyDict)
            {
                foreach (var item in MainData.keyDict)
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
        
        /// <summary>
        /// 窗口关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (null != KEYKOOK)
                KEYKOOK.Stop();
            this.notifyIcon1.Dispose();
            if (MainData.isNetwork)
            {
                new Thread(StopUdpListen).Start();
            }
            IsUdpcRecvStart = false;
            Tools.ShowInfo2("窗口关闭。");
        }

        /// <summary>
        /// 清除播放列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string videourl = "http://111.230.14.84:8080/8.mp4";
            Tools.USBSendData(String.Format("S09E", videourl), "CutPrint");
        }

        private void btnCut2_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show(this, Resources.ZHCN_DEVICENOCONN);
                return;
            }
            if (this.InvokeRequired)
            {
                this.Invoke(new System.Action(delegate()
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
                GetScreenOnFocus();
                if (MainData.isNetwork) //网络同屏，全屏
                {
                    cutvideo_x = 0;
                    cutvideo_y = 0;
                    cutvideo_width = Screen.PrimaryScreen.Bounds.Width; 
                    cutvideo_height = Screen.PrimaryScreen.Bounds.Height;
                    isCuttingVideo = true;
                    btnCut2.Text = "停止同屏";
                    new Thread(NetWorkCutVideo).Start();
                    //new Thread(NetWorkCutVideoListen).Start();
                    //Tools.SendUDP("S90CutVideo2/cutvideo2.jpgE");
                }
                else //划定区域同屏
                {
                    CutPopUp cut_form = new CutPopUp(CutScreen(screen_on_focus.Bounds.Left, screen_on_focus.Bounds.Top, screen_on_focus.Bounds.Width, screen_on_focus.Bounds.Height), screen_on_focus, previous_selection, this, 2);
                    cut_form.OnCut += (s, e2) =>
                    {
                        if (!isRect)
                        {
                            this.Show(); this.Focus();
                        }
                        previous_selection = e2.Selection;
                        cutvideo_x = previous_selection.X;
                        cutvideo_y = previous_selection.Y;
                        cutvideo_width = previous_selection.Width;
                        cutvideo_height = previous_selection.Height;
                        isCuttingVideo = true;
                        btnCut2.Text = "停止同屏";
                        this.CutVideo(null, null);
                        this.Cut2Timer.Start();
                        if (!isRect)//最小化
                        {
                            this.WindowState = FormWindowState.Minimized;
                            this.ShowInTaskbar = false;
                            SetVisibleCore(false);
                        }
                    };
                    cut_form.Show();
                }
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
                    if (MainData.isNetwork)
                    {
                        Tools.SendUDP("S94STOPE");
                        if (NetWorkCutVideoListenSocket != null && NetWorkCutVideoListenSocket.Connected)
                        {
                            NetWorkCutVideoListenSocket.Shutdown(SocketShutdown.Both);
                            NetWorkCutVideoListenSocket.Close();
                        }
                    }
                    else
                        Tools.USBSendData("S08||STOP||E", "StopCutPrint");
                }
            }
        }
       
        private delegate void returnStrDelegate();
        private delegate void returnStrDelegate2(object param);
        private int cutvideo_x=0, cutvideo_y = 0, cutvideo_width = 100, cutvideo_height = 100;

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
        public static Thread thread;
        Socket NetWorkCutVideoListenSocket = null;
        private void NetWorkCutVideoListen()
        {
            if (NetWorkCutVideoListenSocket != null && NetWorkCutVideoListenSocket.Connected)
            {
                NetWorkCutVideoListenSocket.Shutdown(SocketShutdown.Both);
                NetWorkCutVideoListenSocket.Close();
                
            }
            Console.WriteLine("建立19999监听连接：");
            NetWorkCutVideoListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipendpiont = new IPEndPoint(IPAddress.Any, 19999);
            NetWorkCutVideoListenSocket.Bind(ipendpiont);
            NetWorkCutVideoListenSocket.Listen(10);
            NetWorkCutVideoListenSocket.BeginAccept(new AsyncCallback((IAsyncResult ar) => {
                Socket server1 = (Socket)ar.AsyncState;
                Socket Client = server1.EndAccept(ar);
                while (true)
                {
                    byte[] byteDateLine = new byte[1024];
                    int recv = Client.Receive(byteDateLine);
                    string stringdata = Encoding.ASCII.GetString(byteDateLine, 0, recv);
                    Console.WriteLine("网络连接，点击坐标：" + stringdata);
                }
            }), null);
        }

        /*接收来自服务器上的信息*/
        public void targett(Object sock)
        {
            Socket serverSocket = (Socket)sock;
            Console.WriteLine("已经建立连接准备接受数据");

            while (true && isCuttingVideo)
            {
                if (serverSocket.Available <= 0) continue;
                if (serverSocket == null) return;

                EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号
                byte[] buffer2 = new byte[1024];
                int length = serverSocket.ReceiveFrom(buffer2, ref point);//接收数据报
                string message = Encoding.UTF8.GetString(buffer2, 0, length);
                if (message.StartsWith("S94||"))
                {
                    String pointXY = message.Substring(5, message.Length - 5 - 3);
                    Console.WriteLine("网络连接，点击坐标：" + message + ",截取：" + pointXY);
                    Tools.ClickPrint(pointXY);
                }
            }
        }

        private void NetWorkCutVideo()
        {
            try
            {
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipendpiont = new IPEndPoint(IPAddress.Parse(MainData.DeviceIP), 20000);
                serverSocket.Connect(ipendpiont);

                Thread thread = new Thread(targett);
                thread.Start(serverSocket);

                new Thread(() =>
                {
                    try
                    {
                        while (isCuttingVideo)
                        {
                            Image img = ScreenCapture.captureScreen(cutvideo_x, cutvideo_y, cutvideo_width, cutvideo_height);
                            byte[] fssize = ImageHelper.ImageToBytes(img);


                            byte[] bLength = IntToByteArray(fssize.Length);
                            serverSocket.Send(bLength);
                            serverSocket.Send(fssize);

                            byte[] filenamebytes = Encoding.UTF8.GetBytes("");
                            int sendlen = serverSocket.Send(filenamebytes, filenamebytes.Length, SocketFlags.None);
                            Console.WriteLine("发送截屏图片" + sendlen);
                            Thread.Sleep(1000);
                        }
                        serverSocket.Shutdown(SocketShutdown.Both);
                        serverSocket.Close();
                    }
                    catch { }
                }).Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
        }

        private void NetWorkCutImg(Image img)
        {
            try
            {
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipendpiont = new IPEndPoint(IPAddress.Parse(MainData.DeviceIP), 20000);
                serverSocket.Connect(ipendpiont);

                byte[] fssize = ImageHelper.ImageToBytes(img);
                byte[] bLength = IntToByteArray(fssize.Length);
                serverSocket.Send(bLength);
                serverSocket.Send(fssize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
        }
        
        /// <summary>
        /// 把int32类型的数据转存到4个字节的byte数组中
        /// </summary>
        /// <param name="m">int32类型的数据</param>
        /// <param name="arry">4个字节大小的byte数组</param>
        /// <returns></returns>
        static byte[] IntToByteArray(Int32 m)
        {
            byte[] arry = new byte[4]; 

            arry[3] = (byte)(m & 0xFF);
            arry[2] = (byte)((m & 0xFF00) >> 8);
            arry[1] = (byte)((m & 0xFF0000) >> 16);
            arry[0] = (byte)((m >> 24) & 0xFF);

            return arry;
        }

        private void CutVideo(object sender, ElapsedEventArgs e1)
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
                
                Image img = ScreenCapture.captureScreen(cutvideo_x, cutvideo_y, cutvideo_width, cutvideo_height);
               
                if (MainData.isNetwork)
                {
                }
                else
                {
                    byte[] fssize = ImageHelper.ImageToBytes(img);
                    byte[] bLength = Encoding.Default.GetBytes(("S08" + fssize.Length).PadRight(100, ' '));
                    object[] sendStream = new Object[2] { bLength, fssize };

                    Tools.USBSendData(sendStream, "USBCutVideo");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
            finally
            {
                isCuttingVideo2 = false;
            }
        }

        private void UsbsendFile(int idx, String imgpath)
        {
            new Thread(() => {
                try
                {
                    FileInfo f = new FileInfo(imgpath);
                    System.IO.FileStream fs = new System.IO.FileStream(f.FullName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read);
                    byte[] fssize = new byte[(int)fs.Length];
                    fs.Read(fssize, 0, fssize.Length);

                    byte[] bLength = Encoding.Default.GetBytes(("S08" + fssize.Length).PadRight(100, ' '));
                    USBDeviceSocket.Send(bLength);
                    USBDeviceSocket.Send(fssize);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + e.StackTrace);
                }
            }).Start();
        }

        private FileInfo CutPrint()
        {
            String dir = "CutVideo";
            Image img = ScreenCapture.captureScreen(cutvideo_x, cutvideo_y, cutvideo_width, cutvideo_height);

            //以JPG文件格式来保存
            Size screenSize = Screen.PrimaryScreen.Bounds.Size;
            string imgName = string.Format(@"captureimg.{0:yyyyMMddHHmmss.ffff}.jpg", DateTime.Now);
            //img.Save(dir + "/" + imgName, ImageFormat.Jpeg);
            Common.GetPicThumbnail(img, dir + "/" + imgName, Convert.ToInt32(screenSize.Height*0.7), Convert.ToInt32(screenSize.Width * 0.7), 90);
            FileInfo f = new FileInfo(dir + "/" + imgName);
            return f;
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
