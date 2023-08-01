using log4net;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace ZXClient.util
{
    public static class LogHelper
    {
        //private static readonly ILog logInfo = LogManager.GetLogger("Log");
        //private static readonly ILog logErr = LogManager.GetLogger("RollingLogFileAppender");
        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        #region static void WriteLog(Type t, Exception ex)

        public static void WriteInfo(Type t, string msg)
        {
            Console.WriteLine(msg);
            //log4net.ILog log = log4net.LogManager.GetLogger("ERR");
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Info(msg);
        }

        public static void WriteError(Type t, Exception ex)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger("ERR");
            //log.Error("Error", ex);
            Console.WriteLine(ex.Message + ex.StackTrace);
        }

        #endregion

        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        #region static void WriteLog(Type t, string msg)

        public static void WriteError(Type t, string msg)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger("ERR");
            //log.Error(msg);
        }

        #endregion
       
    }
}
