using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ZXClient.util
{
    public class HttpUtil
    {
        /// <summary>
        /// 提交数据请求  方法一
        /// </summary>
        /// <param name="POSTURL">请求提交的地址 如：http://xxx.xxx.xxx/interface/TestPostRequest</param>
        /// <param name="PostData">提交的数据(字符串)</param>
        /// <returns></returns>
        public static string PostData(string POSTURL, string PostData)
        {
            try
            {
                Console.WriteLine(POSTURL + ", " + PostData);
                StreamReader streamReader = RequestStream(POSTURL, PostData);
                String rst = streamReader.ReadToEnd();
                return rst;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(typeof(HttpUtil), ex);
                return null;
            }
        }
        public static string RequestData(string POSTURL, string PostData)
        {
            try
            {
                Console.WriteLine("RequestData, "+ POSTURL + ", " + PostData);
                StreamReader streamReader = GetStream(POSTURL + "?" + PostData);
                if (streamReader == null)
                    return null;
                String rst = streamReader.ReadToEnd();
                LogHelper.WriteInfo(typeof(HttpUtil), "请求结果：" + rst);
                return rst;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(typeof(HttpUtil), ex);
                return null;
            }
        }
        public static string GetData(string POSTURL)
        {
            try
            {
                StreamReader streamReader = GetStream(POSTURL);
                if (streamReader == null)
                    return null;
                String rst = streamReader.ReadToEnd();
                LogHelper.WriteInfo(typeof(HttpUtil), "请求结果：" + rst);
                return rst;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(typeof(HttpUtil), ex);
                return null;
            }
        }

        /// <summary>
        /// 提交数据请求  方法一
        /// </summary>
        /// <param name="POSTURL">请求提交的地址 如：http://xxx.xxx.xxx/interface/TestPostRequest</param>
        /// <param name="PostData">提交的数据(字符串)</param>
        /// <returns></returns>
        public static string RequestFile(string filename, string POSTURL, string PostData)
        {
            StreamReader streamReader = RequestStream(POSTURL, PostData);

            FileStream fs = new FileStream(filename, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(streamReader);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
            return "";
        }

        public static bool UploadFile(string POSTURL, FileInfo fileinfo)
        {
            Uri _uri = new Uri(POSTURL);  //创建请求地址

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(_uri);  //创建请求对象

            ASCIIEncoding encoding = new ASCIIEncoding(); //初始化编码对象

            string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线

            request.Method = "POST"; //请求方式为Post类型

            request.ContentType = "multipart/form-data;boundary=" + boundary;   //请求类型 对照fiddler Content-Type: multipart/form-data; boundary=----------acebdf13572468

            byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("--" + boundary + "--\r\n"); //仿照上面的fiddler格式建立最后一个分割线

            Stream requestStream = request.GetRequestStream(); //创建即将post给对方的请求流
            
                StringBuilder sb = new StringBuilder();
                sb.Append("--");
                sb.Append(boundary);
                sb.Append("\r\n");
                sb.Append("Content-Disposition: form-data; name=\"");
                sb.Append("file");
                sb.Append("\"; filename=\"");
                sb.Append(fileinfo.Name);
                sb.Append("\"");
                sb.Append("\r\n");
                sb.Append("Content-Type: ");
                sb.Append("application/octet-stream");
                sb.Append("\r\n");
                sb.Append("\r\n");

                string strPostHeader = sb.ToString();
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);
                requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);  //把头部转为数据流放入到请求流中去
            
                byte[] fileByte = File.ReadAllBytes(fileinfo.FullName);
                requestStream.Write(fileByte, 0, fileByte.Length);   // 将文件流转为数据流放入到请求流中去，这部分对应fiddler中的
                byte[] temp = Encoding.UTF8.GetBytes("\r\n");
                requestStream.Write(temp, 0, temp.Length);        //结尾加了一个换行\r\n的目的是因为确保下一个分割线另起一行，这个分隔符一定要换行展示不然会报错
            
            requestStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);  //结尾加上结束分隔符对应fiddler的---acebdf13572468--
            requestStream.Close();
            HttpWebResponse _response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(_response.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd();
            return true;
        }

        public static bool ExecuteMultipartRequest(string url, List<KeyValue> nvc)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url, UriKind.RelativeOrAbsolute));
            webRequest.Method = "POST";
            webRequest.Timeout = 1800000;
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webRequest.KeepAlive = true;
            webRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;
            using (var rs = webRequest.GetRequestStream())
            {
                // 普通参数模板
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                //带文件的参数模板
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                foreach (KeyValue keyValue in nvc)
                {
                    //如果是普通参数
                    if (keyValue.FilePath == null)
                    {
                        rs.Write(boundarybytes, 0, boundarybytes.Length);
                        string formitem = string.Format(formdataTemplate, keyValue.Key, keyValue.Value);
                        byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                        rs.Write(formitembytes, 0, formitembytes.Length);
                    }
                    //如果是文件参数,则上传文件
                    else
                    {
                        rs.Write(boundarybytes, 0, boundarybytes.Length);
                        string header = string.Format(headerTemplate, keyValue.Key, keyValue.Value, keyValue.ContentType);
                        byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                        rs.Write(headerbytes, 0, headerbytes.Length);
                        using (var fileStream = new FileStream(keyValue.FilePath, FileMode.Open, FileAccess.Read))
                        {
                            byte[] buffer = new byte[4096];
                            int bytesRead = 0;
                            long total = 0;
                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                rs.Write(buffer, 0, bytesRead);
                                total += bytesRead;
                            }
                        }
                    }

                }
                byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();
            }

            // 获取响应
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                {
                    string body = reader.ReadToEnd();
                    reader.Close();
                    return true;
                }
            }
        }
      
        public static bool DownloadFile(string url, string path)
        {
            string tempFile = "update.temp"; //临时文件
            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);    //存在则删除
            }
            try
            {
                FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //发送请求并获取相应回应数据
                Console.WriteLine("文件下载 " + url);
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                //创建本地文件写入流
                //Stream stream = new FileStream(tempFile, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    //stream.Write(bArr, 0, size);
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                //stream.Close();
                fs.Close();
                responseStream.Close();
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);//存在则删除
                }
                System.IO.File.Move(tempFile, path);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(typeof(HttpUtil), ex);
                return false;
            }
        }

        /// <summary>
        /// 提交数据请求  方法一
        /// </summary>
        /// <param name="POSTURL">请求提交的地址 如：http://xxx.xxx.xxx/interface/TestPostRequest</param>
        /// <param name="PostData">提交的数据(字符串)</param>
        /// <returns></returns>
        public static StreamReader RequestStream(string POSTURL, string PostData)
        {
            try
            {
                //发送请求的数据
                WebRequest myHttpWebRequest = WebRequest.Create(POSTURL);
                myHttpWebRequest.Timeout = 5000;
                myHttpWebRequest.Method = "POST";
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] byte1 = encoding.GetBytes(PostData);
                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
                myHttpWebRequest.ContentLength = byte1.Length;
                Stream newStream = myHttpWebRequest.GetRequestStream();
                newStream.Write(byte1, 0, byte1.Length);
                newStream.Close();

                //发送成功后接收返回的XML信息
                HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse();
                string lcHtml = string.Empty;
                Encoding enc = Encoding.GetEncoding("UTF-8");
                Stream stream = response.GetResponseStream();
                return new StreamReader(stream, enc);
            }
            catch(Exception ex)
            {
                LogHelper.WriteError(typeof(HttpUtil), ex);
                return null;
            }
        }

        /// <summary>
        /// 提交数据请求  方法一
        /// </summary>
        /// <param name="POSTURL">请求提交的地址 如：http://xxx.xxx.xxx/interface/TestPostRequest</param>
        /// <param name="PostData">提交的数据(字符串)</param>
        /// <returns></returns>
        public static StreamReader GetStream(string POSTURL)
        {
            try
            {
                LogHelper.WriteInfo(typeof(HttpUtil), POSTURL);
                WebRequest myHttpWebRequest = WebRequest.Create(POSTURL);
                myHttpWebRequest.Timeout = 10000;
                myHttpWebRequest.Method = "GET";
                UTF8Encoding encoding = new UTF8Encoding();

                if (myHttpWebRequest == null)
                {
                    Console.WriteLine("接口请求错误" + POSTURL);
                    return null;
                }
                //发送成功后接收返回的XML信息
                HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse();
                string lcHtml = string.Empty;
                Encoding enc = Encoding.GetEncoding("UTF-8");
                Stream stream = response.GetResponseStream();
                return new StreamReader(stream, enc);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(typeof(HttpUtil), ex);
                return null;
            }
        }
    }

    public class KeyValue
      {
          public string Key;
          public string Value;
          public string FilePath;
          public string ContentType = "*/*";
          public KeyValue(string key, string value, string filePath, string contentType)
          {
              Key = key;
              Value = value;
              FilePath = filePath;
              ContentType = contentType;
          }
          public KeyValue() { }
          public KeyValue(string key, string value, string filePath)
          {
              Key = key;
              Value = value;
              FilePath = filePath;
          }
          public KeyValue(string key, string value)
          {
              Key = key;
              Value = value;
          }
      }
}
