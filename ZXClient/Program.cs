using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using log4net;
using ZXClient.dao;
using ZXClient.model;
using ZXClient.NotifyWin;
using ZXClient.Updater;
using ZXClient.util;
using System.Runtime.InteropServices;

namespace ZXClient
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                //设置应用程序处理异常方式：ThreadException处理
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                //处理非UI线程异常
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                if (args == null || args.Length==0)
                    GlobalMutex();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                bool bHasError = false;
                AutoUpdater autoUpdater = new AutoUpdater();
                try
                {
                    //autoUpdater.Update();
                }
                catch (WebException exp)
                {
                    Debug.WriteLine(exp.Message + exp.StackTrace);
                    Console.WriteLine("无法找到指定资源");
                    bHasError = true;
                }
                catch (XmlException exp)
                {
                    Debug.WriteLine(exp);
                    bHasError = true;
                    Console.WriteLine("下载的升级文件有错误");
                }
                catch (NotSupportedException exp)
                {
                    Debug.WriteLine(exp);
                    bHasError = true;
                    Console.WriteLine("升级地址配置错误");
                }
                catch (ArgumentException exp)
                {
                    Debug.WriteLine(exp);
                    bHasError = true;
                    Console.WriteLine("下载的升级文件有错误");
                }
                catch (Exception exp)
                {
                    bHasError = true;
                    Console.WriteLine(exp.Message + exp.StackTrace);
                    Console.WriteLine("升级过程中发生错误");
                }
                finally
                {
                    if (bHasError == true)
                    {
                        try
                        {
                            autoUpdater.RollBack();
                        }
                        catch (Exception exp)
                        {
                            Debug.WriteLine(exp);
                        }
                    }
                }
                initDB();
                if (MainData.cbNoLogin)
                {
                    Application.Run(new WorkForm());
                }
                else
                {
                    LoginWindow login = new LoginWindow();
                    login.ShowDialog();

                    if (login.DialogResult == DialogResult.OK)
                    {
                        Application.Run(new WorkForm());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                LogHelper.WriteError(typeof(Program), ex);
            }
        }
        
        static void initDB()
        {
            try
            {
                SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
                connstr.DataSource = MainData.datasource;

                MainData.conn.ConnectionString = connstr.ToString();
                MainData.conn.Open();

                db_ConfigDao.UpdateSchema();
                int r = db_ConfigDao.addIfNoExist("http://localhost:8080/", "localhost", "8080", "192.168.1.123", MainData.ConnTypeData[0]);//不存在则添加

                object[] configs = db_ConfigDao.getConfig();
                MainData.ServerAddr = configs[0].ToString();
                MainData.ServerIP = configs[1].ToString();
                MainData.ServerPort = configs[2].ToString();
                MainData.DeviceIP = configs[3].ToString();
                MainData.ConnType = configs[4].ToString();

                MainData.FtpIP = configs[5].ToString();
                MainData.FtpPort = configs[6].ToString();
                MainData.FtpUserName = configs[7].ToString();
                MainData.FtpPwd = configs[8].ToString();

                if (null!= configs[9])
                {
                    MainData.cbNoLogin = Boolean.Parse(configs[9].ToString());
                }

                MainData.isNetwork = MainData.ConnType == MainData.ConnTypeData[0];

                if (MainData.FtpIP != "" && MainData.FtpUserName != null)
                MainData.ftpHelper = new FtpHelper(MainData.FtpIP, "/", MainData.FtpUserName, MainData.FtpPwd);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                //LogHelper.WriteError(typeof(Program), ex);
            }
        }
        
        private static Mutex mutex = null;

        private static void GlobalMutex()
        {
            // 是否第一次创建mutex
            bool newMutexCreated = false;
            string mutexName = "Global\\" + "WareHouseMis";//系统名称，Global为全局，表示即使通过通过虚拟桌面连接过来，也只是允许运行一次
            try
            {
                mutex = new Mutex(false, mutexName, out newMutexCreated);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                System.Threading.Thread.Sleep(1000);
                Environment.Exit(1);
            }

            // 第一次创建mutex
            if (newMutexCreated)
            {
                LogHelper.WriteInfo(typeof(Program), "程序已启动");
                //todo:此处为要执行的任务
            }
            else
            {
                MessageBox.Show("另一个窗口已在运行，不能重复运行。");
                
                System.Threading.Thread.Sleep(1000);
                Environment.Exit(1);//退出程序
            }
        }

        /// <summary>
        /// 弹出提示消息窗口
        /// </summary>
        public static void Notify(string caption, string content)
        {
            Notify(caption, content, 150, 100, 2000);
        }

        /// <summary>
        /// 弹出提示消息窗口
        /// </summary>
        public static void Notify(string caption, string content, int width, int height, int waitTime)
        {
            NotifyWindow notifyWindow = new NotifyWindow(caption, content);
            notifyWindow.TitleClicked += new System.EventHandler(notifyWindowClick);
            notifyWindow.TextClicked += new EventHandler(notifyWindowClick);
            notifyWindow.SetDimensions(width, height);
            notifyWindow.WaitTime = waitTime;
            notifyWindow.Notify();
        }

        private static void notifyWindowClick(object sender, EventArgs e)
        {
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.Exception, e.ToString());
            
            LogHelper.WriteError(typeof(Program), e.Exception);
            MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
            
            LogHelper.WriteError(typeof(Program), e.ExceptionObject as Exception);
            MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 生成自定义异常消息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="backStr">备用异常消息：当ex为null时有效</param>
        /// <returns>异常字符串文本</returns>
        static string GetExceptionMsg(Exception ex, string backStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****************************异常文本****************************");
            sb.AppendLine("【生成时间】：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine("【异常类型】：" + ex.GetType().Name);
                sb.AppendLine("【异常信息】：" + ex.Message);
                sb.AppendLine("【堆栈调用】：" + ex.StackTrace);
            }
            else
            {
                sb.AppendLine("【未处理异常】：" + backStr);
            }
            sb.AppendLine("***************************************************************");
            return sb.ToString();
        }
    }
}
