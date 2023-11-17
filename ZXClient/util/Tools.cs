using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using ZXClient.model;

namespace ZXClient.util
{
    public class Tools
    {
        private static Type TAG = typeof(Tools);
        //设置鼠标位置
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        //设置鼠标按键和动作
        [DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo); //UIntPtr指针多句柄类型
        [Flags]
        enum MouseEventFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }
        public static bool isUsbConnected = false;

        /// <summary>
        /// 点击客户端屏幕
        /// </summary>
        /// <param name="pointXY">坐标,未分隔</param>
        public static void ClickPrint(String pointXY)
        {
            //获取窗体大小
            int PringW = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            int PrintH = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            int remotex = int.Parse(pointXY.Split(',')[0].Split('.')[0]);
            int remotey = int.Parse(pointXY.Split(',')[1].Split('.')[0]);
            int movex = Convert.ToInt32(PringW * (remotex * 1.0 / 1280 * 1.0));
            int movey = Convert.ToInt32(PrintH * (remotey * 1.0 / 800 * 1.0));
            SetCursorPos(movex, movey);
            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
            mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
        }

        /// <summary>
        /// 设备连接心跳
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void HeartTicket()
        {
            if (MainData.isNetwork)
            {
                String recvData = Tools.SendUDP("S95E");
                Tools.ShowInfo2("发送心跳命令：S95E, 返回：" + recvData);
                WorkForm.isConnected = recvData != String.Empty;// "S95OK";
                
            }
            else
            {
                bool isconn = false;
                List<String> data2 = USBSendData("S99||0||E", "getDeviceInfo");//获取设备信息
                if (data2 == null || data2.Count == 0)
                {
                    Tools.ShowInfo2("评价器连不上了");
                    isconn = false;
                }
                else
                {
                    isconn = false;
                    foreach (var item in data2)
                    {
                        if (item == "RecvCmdOK")
                        {
                            Tools.ShowInfo2("设备连接心跳:成功");
                        }
                    }
                    String[] d = data2[data2.Count - 1].Split(new string[] { "||" }, StringSplitOptions.None);
                    if (d.Length > 2)
                    {
                        WorkForm.mID = d[2];
                        WorkForm.mDeviceVer = d[1].Replace("Version=", "");
                        WorkForm.mDeviceNVer = d[7];
                        WorkForm.mAndroidVer = d[8];
                        WorkForm.mPrdCode = d[9];
                        Tools.ShowInfo2(data2[data2.Count - 1] + "获取设备mac地址:" + WorkForm.mID + "，设备资源版本号：" + WorkForm.mDeviceVer + "，设备更新资源版本号：" + WorkForm.mDeviceNVer);
                        isconn = true;
                    }
                }

                if (isconn)
                {
                    try
                    {
                        #region 心跳方法
                        String data = String.Format("mac={0}&cardNum={1}", WorkForm.mID, MainData.USERCARD);
                        String ret = HttpUtil.RequestData(MainData.ServerAddr + MainData.INET_EMPLOYEEHEART, data);
                        Tools.ShowInfo2(DateTime.Now + "心跳：" + ret + WorkForm.mID);
                        #endregion
                        isconn = ret == "online";
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteError(TAG, ex);
                        Tools.ShowInfo2("服务器连接失败！" + MainData.ServerAddr + MainData.INTE_EMPLOYEEGETINFO);
                        isconn = false;
                    }
                }
                WorkForm.isConnected = isconn;
            }
        }

        /// <summary>
        /// 发送UDP数据－网络方式
        /// </summary>
        /// <param name="str"></param>
        public static String SendUDP(String str)
        {
            String recvdata = "";
            try
            {
                byte[] data = new byte[1024];
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(MainData.DeviceIP), MainData.udpPort);
                ShowInfo2("SendUDP：" + MainData.DeviceIP + ":" + MainData.udpPort + ", " + str);
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                data = MainData.encode.GetBytes(str);
                server.SendTimeout = 3000;
                server.SendTo(data, data.Length, SocketFlags.None, ipep);

                IPEndPoint sender = new IPEndPoint(IPAddress.Parse(MainData.DeviceIP), 0);
                EndPoint Remote = sender;

                try
                {
                    data = new byte[1024];
                    //发送接收信息
                    server.ReceiveTimeout = 10000;
                    int recv = server.ReceiveFrom(data, ref Remote);
                    recvdata = MainData.encode.GetString(data, 0, recv);
                }
                catch (SocketException e)
                {
                    LogHelper.WriteError(typeof(Tools), e);
                    recvdata = String.Empty;
                    WorkForm.isConnected = false;
                    //BtnEnable(false);
                    if (!WorkForm.heartTimer.Enabled)
                        WorkForm.heartTimer.Enabled = true;
                }
                catch (Exception e)
                {
                    LogHelper.WriteError(typeof(Tools), e);
                    recvdata = String.Empty;
                }

                server.Close();
                return recvdata;
            }
            catch (Exception ex)
            {
                ShowInfo2("UDP发送失败" + ex.Message);
                LogHelper.WriteError(typeof(Tools), ex);
                return String.Empty;
            }
        }

        public static List<String> USBSendData(Object response, String op)
        {
            return USBSendData(response, op, false, null, null);
        }

        public static List<String> USBSendData(Object response, String op, Boolean isPJQ, UdpClient client, IPEndPoint ipendpoint)
        {
            //if (!isUsbConnected)
            //    return null;
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), MainData.forwardPort));
            }
            catch (SocketException ex)
            {
                Tools.ShowInfo2("评价器连接失败,重定向连接" + ex.Message + ex.StackTrace);
                isUsbConnected = false;
                return null;
            }

            try
            {
                byte[] data = null;
                if (op == "USBCutVideo")
                {
                    object[] o = (object[])response;
                    socket.Send((byte[])o[0]);
                    socket.Send((byte[])o[1]);
                }
                else
                {
                    Tools.ShowInfo2("USB发送数据：" + response.ToString() + ", op=" + op);
                    data = MainData.encode.GetBytes(response.ToString());
                    socket.Send(data, 0, data.Length, SocketFlags.None);
                }

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
                        mList.Add(MainData.encode.GetString(buffer, 0, length));
                    }
                    String receiveData = MainData.encode.GetString(cdata.ToArray(), 0, cdata.Count).Trim();
                    USBSendCompleted(response.ToString(), receiveData, mList, isPJQ, client, ipendpoint);
                }
                catch (SocketException ex)
                {
                    LogHelper.WriteError(TAG, ex);
                    Tools.ShowInfo2("设备连接失败！");
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(TAG, ex);
                    Tools.ShowInfo2("设备连接失败！");
                }
                return mList;
            }
            catch (SocketException e)
            {
                LogHelper.WriteError(TAG, e);
                Tools.ShowInfo2("网络错误,请检查评价器是否连接正常.");
                //this.isConnected = false;
                //BtnEnable(false);
            }
            catch (Exception e)
            {
                LogHelper.WriteError(TAG, e);
                Tools.ShowInfo2("发送失败,请检查评价器是否连接正常.");
            }
            finally
            {
                socket.Close();
            }
            return null;
        }

        public static void USBSendFile(string path, String filename, String removeName)
        {
            if (path == "" || path == null)
            {
                new ADBClient().Push(filename, MainData.SDCARD + "/" + removeName);
            }
            else
                new ADBClient().Push(path + "/" + filename, MainData.SDCARD + path + "/" + removeName);
        }

        public static void USBSendFile(string path, String filename)
        {
            USBSendFile(path, filename, filename);
        }

        delegate void SendCompletedEventHandler(string op, string message, List<String> data2, Boolean isPJQ, UdpClient client, IPEndPoint ipendpoint);

        static event SendCompletedEventHandler USBSendCompleted = delegate(string op, string message, List<String> data2, Boolean isPJQ, UdpClient client, IPEndPoint ipendpoint)
            {
                for (int i = 0; i < data2.Count; i++)
                {
                    if (data2[i].StartsWith("S02||"))//评价数据
                    {
                        String[] d = data2[i].Split(new string[] { "||" }, StringSplitOptions.None);
                        Tools.ShowInfo2("*********************评价器应答命令, isPJQ: " + isPJQ + "d.length:" + d.Length + "~~~" + data2[i] + ">>>" + Thread.CurrentThread.ManagedThreadId.ToString());
                        if (d.Length >=10)
                        {
                            if (isPJQ)
                            {
                                byte[] data1 = MainData.encode.GetBytes(JsonHelper.SerializeObject(new { appriseresult = d[2] }));
                                int rr = client.Send(data1, data1.Length, ipendpoint);
                            }

                            String data = String.Format("mac={0}&tt={1}&cardnum={2}&pj={3}&demo=(NULL)&businessType=1&videofile={4}&businessTime={5}&imgfile={6}&busRst={7}&videofile2={8}"
                                , WorkForm.mID, d[5], d[1], d[2], d[7], d[4], d[6], d[8], d[7]);
                            String rst = HttpUtil.RequestData(MainData.ServerAddr + MainData.INTE_EVALDATA, data);
                            //if (rst == "success")
                            {
                                if (op == "SE" && WorkForm.GetEvalResultTimer != null && WorkForm.GetEvalResultTimer.Enabled)
                                    WorkForm.GetEvalResultTimer.Stop();
                                new ADBClient().Pull(MainData.SDCARD + "recorder/", "recorder/");//下载录音录像文件
                                new ADBClient().Execute("rm " + MainData.SDCARD + "recorder/*", true);

                                #region 上传录音录像文件到FTP
                                try
                                {
                                    String ftpDir = "recorder";
                                    List<String> listFiles = CommonHelper.ListFiles(new DirectoryInfo(ftpDir));
                                    foreach (var item in listFiles)
                                    {
                                        FileInfo f = new FileInfo(item);
                                        Tools.ShowInfo2("上传录音文件：" + item + ", " + MainData.ServerAddr);
                                        List<KeyValue> keyValues = new List<KeyValue>();
                                        keyValues.Add(new KeyValue("uploaddir", "download/recorder"));
                                        keyValues.Add(new KeyValue("file", new FileInfo(item).Name, item));
                                        if (HttpUtil.ExecuteMultipartRequest(MainData.ServerAddr + MainData.INTE_APPRIESFILEUPLOAD, keyValues))
                                        {
                                            f.Delete();
                                        }
                                        //if (HttpUtil.UploadFile(MainData.ServerAddr + MainData.INTE_APPRIESFILEUPLOAD, new FileInfo(item)))
                                        //{
                                        //    f.Delete();
                                        //}
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.WriteError(TAG, ex);
                                }
                                #endregion
                            }
                        }
                        if (d.Length==7)
                        {
                            String data = String.Format("mac={0}&tt={1}&name={2}&phone={3}&remark={4}", d[1], d[2], d[3], d[4], d[5]);
                            String rst = HttpUtil.RequestData(MainData.ServerAddr + MainData.INTE_APPRIESADDCONTACT, data);
                        }
                        if (WorkForm.GetEvalResultTimer != null && WorkForm.GetEvalResultTimer.Enabled)
                            WorkForm.GetEvalResultTimer.Stop();
                    }
                    if (op.StartsWith("S98||") && data2[i].StartsWith("RecvCmdOK"))//修改评价器配置成功
                    {
                        DateTime now = DateTime.Now;
                        Tools.USBSendData(String.Format("S14||{0}||{1}||{2}||{3}||{4}||{5}||E", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second), "SyncDateTime");//同步设备时间

                        String inteResp = HttpUtil.RequestData(MainData.ServerAddr + MainData.INTE_GETDEPTVIDEO, "mac=" + WorkForm.mID);
                        Tools.ShowInfo2("获取设备" + WorkForm.mID + "录音录像权限:" + inteResp);

                        USBSendData("S17||" + inteResp + "||E", "Recorder");//设置录音录像
                    }
                }
                if (data2.Count > 0 && data2[0].StartsWith("S24||"))
                {
                    String adviceData = data2[0].Substring(5, data2[0].Length-8);
                    HttpUtil.RequestData(MainData.ServerAddr + MainData.INTE_ADVICEANSWER, "answer=" + adviceData.Split(',')[0]);
                }
                if (data2.Count > 1 && data2[1]== "RecvCmdOK" && data2[0].StartsWith("S94||"))
                {
                    String pointXY = data2[0].Substring(5, data2[0].Length-5-3);
                    Tools.ClickPrint(pointXY);
                }
            };

       
        static Boolean EmployeeLogin(Boolean isPJQ, UdpClient client, IPEndPoint ipendpoint)
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
                            string picpath = "picture/user_" + model.ext2 + ".jpg";
                            bool down = HttpUtil.DownloadFile(MainData.ServerAddr + model.picture, "picture/user_" + model.ext2 + ".jpg");
                            if (down)
                            {
                                USBSendFile("", picpath);
                                new ADBClient().Push(picpath, MainData.SDCARD + "/user_" + model.ext2 + ".jpg");
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
                            StreamReader infosetdown = HttpUtil.RequestStream(MainData.ServerAddr + MainData.INTE_EMPLOYEEINFOSETDOWN, "");

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

        public static void ShowInfo2(string strInfo)
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

        public static void killadb()
        {
            ADBClient MyClient = new ADBClient();
            MyClient.AdbPath = MainData.AdbExePath;
            MyClient.KillServer();
        }

    }
}
