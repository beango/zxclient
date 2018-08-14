using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Net.Sockets;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZXClient.Properties;
using ZXClient.util;

namespace ZXClient.model
{
    public static class MainData
    {
        public static SQLiteConnection conn = new SQLiteConnection();
        public static String datasource = System.Environment.CurrentDirectory + "\\db\\d.db";
        public static Encoding encode = Encoding.UTF8;

        public static string INET_EMPLOYEESERVERTIME = "employeeServerTime.action";
        public static string INET_EMPLOYEEHEART = "employeeHeart.action";
        public static string INET_GETUPDATEVERSION = "getUpdateVersion.action";
        public static string INTE_EMPLOYEEGETINFO = "employeeGetInfo.action";
        public static string INTE_EMPLOYEEINFOSETDOWN = "employeeInfoSetDownload";
        public static string INTE_EVALDATA = "appriesAddSpByPantryn.action";
        public static string INTE_APPRIESADDCONTACT = "appriesAddContact.action";
        public static string INTE_EMPLOYEELOGIN = "employeeLogin.action";
        public static string INTE_GETDEPTVIDEO = "getDeptVideo.action";//录音录像权限
        public static string INTE_GETDEPTNOTICE = "getDeptNotice.action";//获取设备游屏信息
        public static string INTE_GETDISSATISFIEDKEY = "getDissatisfiedKey.action";//获取不满意按键
        public static string INTE_WEBURLDOWNLOAD = "weburlDownload";//信息查询资源
        public static string INTE_NOTICEDOWNLOAD = "noticeDownload";//通知公告资源
        public static string INTE_ADVICEDOWNLOAD = "adviceDownload";//意见调查
        public static string INTE_ADVICEANSWER = "advicesaveAnswer";//意见调查结果
        public static string INTE_GETEVALBUTTONS = "getevalbuttons.action";//获取评价按键
        public static string INTE_EMPLOYEECHANGEPSW = "employeeChangePsw.action";//修改密码
        public static string INTE_GETKEYALL = "getKeyAll.action";//获取所有评价按钮
        public static string INTE_APPRIESFILEUPLOAD = "appries/appriesFileupload.action";

        public static bool? autoLoginSucc = null;//是否自动登录
        public static string AccessToken { get; set; }
        internal static string ServerAddr { get; set; }//服务器地址
        internal static string ServerIP { get; set; }//服务器地址
        internal static string ServerPort { get; set; }//服务器地址接口
        internal static string DeviceIP { get; set; }//评价器地址
        internal static bool cbNoLogin { get; set; }//是否允许免登
        
        internal static int devicePort = 8000, forwardPort = 8011;
        internal static int udpPort=8001, udpRecivePort=7791;
        
        public static string AdbExePath = "AdbBin\\adb.exe"; //ADb文件路径
        public static double CUTVIDEOPERSECOND = 800;
        public static int waittimes = 6;//自动登录等待时间，5秒

        public static string loginSuccess = "loginSuccess";
        public static string[] ConnTypeData = new string[] { "网络连接", "USB连接" };
        public static bool isNetwork = ConnType == ConnTypeData[0];
        public static string ConnType = ConnTypeData[0];

        public static KeyboardHook k_hook, k_hook2;
        public static Dictionary<string, KeyEventArgs> keyDict { get; set; }

        public static string SDCARD = "/sdcard/d5/";

        internal static string FtpIP { get; set; }//评价器地址
        internal static string FtpPort { get; set; }//评价器地址
        internal static string FtpUserName { get; set; }//评价器地址
        internal static string FtpPwd { get; set; }//评价器地址
        public static FtpHelper ftpHelper;
        public static WorkForm wf;

        public static Socket UsbSocket;
        public static Boolean isPJQ = false;
        internal static string USERCARD;


        internal static ResourceManager resource = new ResourceManager(typeof(Resources));

        /// <summary>
        /// Application.ExecutablePath
        /// </summary>
        /// <param name="ExecutablePath"></param>
        public static void Restart()//string ExecutablePat
        {
            if (null != WorkForm.ctimer)
            {
                WorkForm.ctimer.Enabled = false;
                WorkForm.ctimer.Stop();
            }
            if (null != WorkForm.KEYKOOK)
                WorkForm.KEYKOOK.Stop();
            if (isNetwork)
            {

            }
            Application.ExitThread();
            Thread thtmp = new Thread(new ParameterizedThreadStart(run));
            object appName = Application.ExecutablePath;
            Thread.Sleep(1000);
            thtmp.Start(appName);
        }

        private static void run(Object obj)
        {
            Process ps = new Process();
            ps.StartInfo.FileName = obj.ToString();
            ps.StartInfo.Arguments = "NEWCONFIG-RESTART";
            ps.Start();
        }
    }
}
