using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Utils
{
    public class Utils
    {
        /// <summary>
        /// 杀掉FoxitReader进程
        /// </summary>
        /// <param name="strProcessesByName"></param>
        public static Exception KillProcess(string processName) 
        { 
            Exception ex = new Exception("没有找到进程");
            foreach (Process p in Process.GetProcesses())            
            {
                if (p.ProcessName.Contains(processName))
                {
                    try
                    {
                        p.Kill();
                        p.WaitForExit(); // possibly with a timeout
                        Console.WriteLine("已杀掉{processName}进程！！！");
                        ex = null;
                    }
                    catch (Win32Exception e)
                    {
                        ex = e;
                    }
                    catch (InvalidOperationException e)
                    {
                        ex = e;
                    }
                }
            }
            return ex;
        }
    }
}
