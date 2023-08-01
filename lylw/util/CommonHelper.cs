using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ZXClient.util
{
    public class CommonHelper
    {
        internal static byte[] ReadFile(String img)
        {
            FileInfo fileinfo = new FileInfo(img);
            byte[] buf = new byte[fileinfo.Length];
            FileStream fs = new FileStream(img, FileMode.Open, FileAccess.Read);
            fs.Read(buf, 0, buf.Length);
            fs.Close();
            //fileInfo.Delete ();
            GC.ReRegisterForFinalize(fileinfo);
            GC.ReRegisterForFinalize(fs);
            return buf;
        }

        public static void ShowProcessing(string msg, Form owner, ParameterizedThreadStart work, object workArg = null)
        {
            FrmProcessing processingForm = new FrmProcessing(msg);
            //dynamic expObj = new ExpandoObject();
            //expObj.Form = processingForm;
            //expObj.WorkArg = workArg;
            processingForm.SetWorkAction(work, null);
            processingForm.ShowDialog(owner);
            if (processingForm.WorkException != null)
            {
                throw processingForm.WorkException;
            }
        }

        public static List<string> ListFiles(FileSystemInfo info)
        {
            List<String> fileList = new List<string>();
            if (!info.Exists) return null;
            DirectoryInfo dir = info as DirectoryInfo;
            //不是目录   
            if (dir == null) return null;
            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i] as FileInfo;
                //是文件   
                if (file != null)
                    fileList.Add(file.FullName);
                //对于子目录，进行递归调用   
                else
                    ListFiles(files[i]);
            }
            return fileList;
        }

        /**//// <summary>
        /// 页面居中
        /// </summary>
        public static void SetMid(Form form)
        {
            // Center the Form on the user's screen everytime it requires a Layout.
            form.SetBounds((Screen.GetBounds(form).Width / 2) - (form.Width / 2),
                (Screen.GetBounds(form).Height / 2) - (form.Height / 2),
                form.Width, form.Height, BoundsSpecified.Location);
        }

        public static byte[] GetPictureBytes(string filename)       //filename填写图片路径
        {
            FileInfo fileInfo = new FileInfo(filename);
            byte[] buffer = new byte[fileInfo.Length];
            using (FileStream stream = fileInfo.OpenRead())
            {
                stream.Read(buffer, 0, buffer.Length);
            }
            return buffer;
        }
    }
}
