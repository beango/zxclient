using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace ConsoleApplication1
{
    class Program
    {
        static String xtoken="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VyVHlwZSI6IkhDQUxMIiwiVVVJRCI6IjAwMDAwMDAwLTAwMDAtMDAwMC0wMDAwLTAwMDAwMDAwMDAwMCIsIklEIjowLCJVc2VybmFtZSI6IiIsIk5pY2tOYW1lIjoi5YaF56eR5LiA6K-K5a6k77yI5p2O5LiW5rCR77yJIiwiQXV0aG9yaXR5SWQiOiIiLCJEZXB0SWQiOjAsIkRlcHRBbmRDaGlsZCI6WzBdLCJCdWZmZXJUaW1lIjoxMjAsIkxvZ2luSVAiOiIxOTIuMTY4LjEuNDQiLCJFeHQiOnsiY2FsbF9pZCI6MSwiY2FsbF9uYW1lIjoi5YaF56eR5LiA6K-K5a6kIiwiZG9jX2lkIjoyLCJkb2NfbmFtZSI6IuadjuS4luawkSIsImxvZ2luX3R5cGUiOjB9LCJleHAiOjE2OTQ4MzA2OTAsImlzcyI6InFtUGx1cyIsIm5iZiI6MTY5NDc0MzI5MH0.BHq2oC44c9NDH8ubyjBp31X3U1Hcccspw99ltcNbNW4";
 
        static void Main(string[] args)
        {
            while (true)
            {
                String PostUrl = "http://192.168.1.41/api/hcall/info";
                string resultData = Post1("", PostUrl, "application/json", "application/json");
                Console.WriteLine(resultData);
                PostUrl = "http://192.168.1.41/api/hcall/heart";
                resultData = Post1("", PostUrl, "application/json", "application/json");
                Console.WriteLine(resultData);
                PostUrl = "http://192.168.1.41/api/hcall/pdhgl";
                resultData = Post1("{}", PostUrl, "application/json", "application/json");
                Console.WriteLine(resultData);
                Thread.Sleep(5000);
            }
            Console.ReadKey();
        }

        public static string Post1(string parameterData, string serviceUrl, string ContentType = "application/json", string Accept = "application/json")
         {
            //先根据用户请求的uri构造请求地址
            //string serviceUrl = string.Format("{0}/{1}", this.BaseUri, uri);

            //创建Web访问对象
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            //把用户传过来的数据转成“UTF-8”的字节流
            byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(parameterData);

            myRequest.Method = "POST";
            //myRequest.Accept = "application/json";
            //myRequest.ContentType = "application/json";  // //Content-Type: application/x-www-form-urlencoded 
            myRequest.AutomaticDecompression = DecompressionMethods.GZip;
            myRequest.Accept = Accept;
            //myRequest.ContentType = ContentType;
            myRequest.ContentType = "application/json; charset=UTF-8";
            myRequest.ContentLength = buf.Length;
            myRequest.MaximumAutomaticRedirections = 1;
            myRequest.AllowAutoRedirect = true;

            //myRequest.Headers.Add("content-type", "application/json");
            //myRequest.Headers.Add("accept-encoding", "gzip");
            myRequest.Headers.Add("x-token", xtoken);

            //发送请求
            Stream stream = myRequest.GetRequestStream();
            stream.Write(buf, 0, buf.Length);
            //stream.Close();

            //通过Web访问对象获取响应内容
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            //通过响应内容流创建StreamReader对象，因为StreamReader更高级更快
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            //string returnXml = HttpUtility.UrlDecode(reader.ReadToEnd());//如果有编码问题就用这个方法
            string returnData = reader.ReadToEnd();//利用StreamReader就可以从响应内容从头读到尾

            //reader.Close();
            //myResponse.Close();

            return returnData;
        }
        public static string RequestData(string POSTURL, string PostData)
        {
            StreamReader streamReader = RequestStream(POSTURL, PostData);
            String rst = streamReader.ReadToEnd();
            return rst;
        }

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 将查询字符串解析转换为名值集合.
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static NameValueCollection GetQueryString(string queryString)
        {
            return GetQueryString(queryString, null, true);
        }

        /// <summary>
        /// 将查询字符串解析转换为名值集合.
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="encoding"></param>
        /// <param name="isEncoded"></param>
        /// <returns></returns>
        public static NameValueCollection GetQueryString(string queryString, Encoding encoding, bool isEncoded)
        {
            queryString = queryString.Replace("?", "");
            NameValueCollection result = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(queryString))
            {
                int count = queryString.Length;
                for (int i = 0; i < count; i++)
                {
                    int startIndex = i;
                    int index = -1;
                    while (i < count)
                    {
                        char item = queryString[i];
                        if (item == '=')
                        {
                            if (index < 0)
                            {
                                index = i;
                            }
                        }
                        else if (item == '&')
                        {
                            break;
                        }
                        i++;
                    }
                    string key = null;
                    string value = null;
                    if (index >= 0)
                    {
                        key = queryString.Substring(startIndex, index - startIndex);
                        value = queryString.Substring(index + 1, (i - index) - 1);
                    }
                    else
                    {
                        key = queryString.Substring(startIndex, i - startIndex);
                    }
                    if (isEncoded)
                    {
                        result[MyUrlDeCode(key, encoding)] = MyUrlDeCode(value, encoding);
                    }
                    else
                    {
                        result[key] = value;
                    }
                    if ((i == (count - 1)) && (queryString[i] == '&'))
                    {
                        result[key] = string.Empty;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 解码URL.
        /// </summary>
        /// <param name="encoding">null为自动选择编码</param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MyUrlDeCode(string str, Encoding encoding)
        {
            if (encoding == null)
            {
                Encoding utf8 = Encoding.UTF8;
                //首先用utf-8进行解码                     
                string code = HttpUtility.UrlDecode(str.ToUpper(), utf8);
                //将已经解码的字符再次进行编码.
                string encode = HttpUtility.UrlEncode(code, utf8).ToUpper();
                if (str == encode)
                    encoding = Encoding.UTF8;
                else
                    encoding = Encoding.GetEncoding("gb2312");
            }
            return HttpUtility.UrlDecode(str, encoding);
        }

    }
}
