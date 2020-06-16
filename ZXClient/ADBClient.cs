using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using ZXClient.util;

namespace ZXClient
{
    public partial class ADBClient// : Component
    {
        // ----------------------------------------- Adb.exe path, leave blank if in same directory as app or included in PATH
        private string adbPath = System.Environment.CurrentDirectory + "\\AdbBin\\adb.exe";
        public string AdbPath
        {
            get { return adbPath; }
            set
            {
                if (System.IO.File.Exists(value)) adbPath = value;
                else adbPath = "\"" + adbPath + "\"";
            }
        }

        // ----------------------------------------- Adb command timeout - usable in push and pull to avoid hanging while executing
        private int adbTimeout;
        public int AdbTimeout
        {
            get { return adbTimeout > 0 ? adbTimeout : 5000; }
            set { adbTimeout = value; }
        }

        // ----------------------------------------- Create our emulated shell here and assign events

        // Create a background thread an assign work event to our emulated shell method
        BackgroundWorker CMD = new BackgroundWorker();
       
        public ADBClient()
        {
            CMD.DoWork += new DoWorkEventHandler(CMD_Send);
        }

        // Needed data types for our emulated shell
        public string Command = "";
        bool Complete = false;

        // Create an emulated shell for executing commands
        private void CMD_Send(object sender, DoWorkEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "cmd.exe",
                Arguments = "/C \"" + Command + "\""
            };

            using (Process process = Process.Start(startInfo))
            {
                if (Command.StartsWith("\"" + adbPath + "\" logcat"))
                {
                    Complete = true;
                    process.WaitForExit();
                    return;
                }

                if (!process.WaitForExit(AdbTimeout))
                    process.Kill();

                Output = process.StandardOutput.ReadToEnd();
                Complete = true;
            };
        }

        // Send a command to emulated shell
        public void SendCommand(string command)
        {
            try
            {
                Console.WriteLine("adb命令" + command);
                CMD.WorkerSupportsCancellation = true;
                Command = command;
                CMD.RunWorkerAsync();
                while (!Complete) Sleep(500);
                Complete = false;
            }
            catch (Exception ex)
            {
                Tools.ShowInfo2(ex.Message + ex.StackTrace);
                LogHelper.WriteError(typeof(ADBClient), ex);
            }
        }

        // Sleep until output
        public void Sleep(int milliseconds)
        {
            DateTime delayTime = DateTime.Now.AddMilliseconds(milliseconds);
            while (DateTime.Now < delayTime)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }

        // Bootstate for rebooting
        public enum BootState
        {
            System, Bootloader, Recovery
        }

        // ----------------------------------------- Allow public modifiers to get output

        public string Output { get; private set; }

        // ----------------------------------------- Functions

        public void Connect(string ip)
        {
            SendCommand("\"" + adbPath + "\" connect " + ip);
        }

        public void Disconnect(decimal ip)
        {
            SendCommand("\"" + adbPath + "\" disconnect " + ip);
        }

        public void StartServer()
        {
            SendCommand("\"" + adbPath + "\" start-server");
        }

        public void KillServer()
        {
            SendCommand("\"" + adbPath + "\" kill-server");
        }

        public void Forward(int localPort, int remotePort)
        {
            string cmd = String.Format("\"" + adbPath + "\" " + " forward tcp:{0} tcp:{1}", localPort, remotePort);
            SendCommand(cmd);
        }

        public List<string> Devices()
        {
            SendCommand("\"" + adbPath + "\" devices");

            string[] outLines = Output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            List<string> l = new List<string>();
            for (var i = 1; i < outLines.Length; i++)
                l.Add(outLines[i]);
            return l;
        }

        public void Execute(string command, bool asroot)
        {
            if (asroot) SendCommand("\"" + adbPath + "\" shell su -c \"" + command + "\"");
            else SendCommand("\"" + adbPath + "\" shell " + command);
        }

        public void Remount()
        {
            SendCommand("\"" + adbPath + "\" shell su -c \"mount -o rw,remount /system\"");
        }

        public void Reboot(BootState boot)
        {
            if (boot == BootState.System) SendCommand("\"" + adbPath + "\" shell su -c \"reboot\"");
            if (boot == BootState.Bootloader) SendCommand("\"" + adbPath + "\" shell su -c \"reboot bootloader\"");
            if (boot == BootState.Recovery) SendCommand("\"" + adbPath + "\" shell su -c \"reboot recovery\"");
        }

        public void Push(string input, string output)
        {
            try { SendCommand("\"" + adbPath + "\" push \"" + input + "\" \"" + output + "\""); } catch { try { SendCommand("\"" + adbPath + "\" push \"" + input.Replace("/", "\\") + "\" \"" + output + "\""); } 
            catch (Exception e) {
                Tools.ShowInfo2(e.Message + e.StackTrace);
            } }
        }

        public void rm(string dir)
        {
            try { SendCommand("" + adbPath + " shell rm " + dir); } catch { }
        }

        public void Pull(string input, string output)
        {
            if (output != null && !string.IsNullOrEmpty(output)) try { SendCommand("\"" + adbPath + "\" pull \"" + input + "\" \"" + output + "\""); } catch { try { SendCommand("\"" + adbPath + "\" pull \"" + input + "\" \"" + output.Replace("/", "\\") + "\""); } catch { } }
            else try { SendCommand("\"" + adbPath + "\" pull \"" + input + "\""); } catch { }
        }

        public void Install(string application)
        {
            try { SendCommand("\"" + adbPath + "\" install \"" + application + "\""); } catch { try { SendCommand("\"" + adbPath + "\" install \"" + application.Replace("/", "\\") + "\""); } catch { } }
        }

        public void Uninstall(string packageName)
        {
            SendCommand("\"" + adbPath + "\" uninstall \"" + packageName + "\"");
        }

        public void Backup(string backupPath, string backupArgs)
        {
            if (backupArgs != null && !string.IsNullOrEmpty(backupArgs)) SendCommand("\"" + adbPath + "\" backup \"" + backupPath + "\" " + "\"" + backupArgs + "\"");
            else SendCommand("\"" + adbPath + "\" backup \"" + backupPath + "\"");
        }

        public void Restore(string backupPath)
        {
            try { SendCommand("\"" + adbPath + "\" restore \"" + backupPath + "\""); } catch { try { SendCommand("\"" + adbPath + "\" restore \"" + backupPath.Replace("/", "\\") + "\""); } catch { } }
        }

        public void Logcat(string logPath, bool overWrite)
        {
            if (overWrite == true) try { SendCommand("\"" + adbPath + "\" logcat > \"" + logPath + "\""); } catch { try { SendCommand("\"" + adbPath + "\" logcat > \"" + logPath.Replace("/", "\\") + "\""); } catch { } }
            else try { SendCommand("\"" + adbPath + "\" logcat >> \"" + logPath + "\""); } catch { try { SendCommand("\"" + adbPath + "\" logcat >> \"" + logPath.Replace("/", "\\") + "\""); } catch { } }
        }
    }
}
