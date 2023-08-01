using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using ZXClient.util;

namespace ZXClient.service
{
    public class LYLWListen
    {
        public Thread th;
        private TcpListener tcpl;
        public bool listenerRun = true;

        public LYLWListen()
        {
            
        }

        public void Listener()
        {
            th = new Thread(new ThreadStart(Listen));
            th.Start();
        }

        public void Stop()
        {
            tcpl.Stop();
            th.Abort();
            th = null;
        }

        private void Listen()
        {
            try
            {
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                tcpl = new TcpListener(ip, 9900);
                tcpl.Start();
                Console.WriteLine("started listening..");
                while (listenerRun)
                {
                    Socket s = tcpl.AcceptSocket();
                    string remote = s.RemoteEndPoint.ToString();
                    Byte[] stream = new Byte[10240];
                    int read = 0;
                    while ((read = s.Receive(stream)) > 0)
                    {
                        String orgurl = System.Text.Encoding.UTF8.GetString(stream, 0, read);
                        Console.WriteLine("orgurl=" + orgurl);

                        orgurl = "http://localhost/" + orgurl;
                        Uri uri = new Uri(orgurl);
                        string queryString = uri.Query;
                        NameValueCollection col = GetQueryString(queryString);
                        string hcpurl = col["TYPE"];
                        Console.WriteLine("hcpurl=" + hcpurl);
                        NameValueCollection col2 = GetQueryString(hcpurl);
                        String SBLSH = col2.Get("SBLSH");

                        /*hcpurl = @"http://10.194.253.88:8082/main/evaluatereq/pad/score?acceptDate=2020-09-25%2009:32:39&certType=10&SBLSH=lw0074936372009250297513&proDepartId=007493637&proStatus=%E5%8A%9E%E7%BB%93&sign=459eb3ce196dc815f8996bf1baa76c2c&resultDate=2020-10-13%2015:16:15&hallCode=23144327010058265&taskType=2&areaName=%E8%8D%94%E6%B9%BE%E5%8C%BA&proDepart=%E8%8D%94%E6%B9%BE%E5%8C%BA%E5%8D%AB%E7%94%9F%E5%81%A5%E5%BA%B7%E5%B1%80&phonumber=38f081fd68ce405dc77ad68aecd00e4c&appId=006429D4E0&userCert=9b7f802475421a4436c081967da51ba043da81fae7587d7a&subMatter=1&timestamp=1602576836398&userName=%E6%9D%A8%E5%85%89%E5%86%9B&areaId=440103&pf=4&taskName=%E5%8C%BB%E5%B8%88%E6%89%A7%E4%B8%9A%E8%AF%81%E4%B9%A6%EF%BC%88%E5%8F%98%E6%9B%B4%EF%BC%89&projectId=11440103007493637K4440120012002202009250310&taskId=11440103007493637K4440120012002&deptCode=11440103007493637K&proManager=%E8%91%A3%E5%AE%81&proManagerNo=11191&windowNo=31 HTTP/1.1
                        Host: 127.0.0.1:9900
                        Connection: Keep-Alive
                        Accept-Encoding: gzip, deflate
                        Accept-Language: zh-CN,en,*
                        User-Agent: Mozilla/5.0";*/

                        hcpurl = hcpurl.Split(' ')[0];
                        Tools.ShowInfo2("好差评地址:" + hcpurl);
                        Tools.ShowInfo2("业务流水号:" + SBLSH);
                        String cmd = "{\"command\":\"hcp-lw\",\"sno\":\"\",\"transcodeid\":\"" + SBLSH + "\",\"hcpurl\":\"" + hcpurl + "\"}";
                        Console.WriteLine(cmd);
                        Tools.ShowInfo2("好差评cmd:" + cmd);
                        Tools.SendUDP(cmd);


                        try
                        {
                            s.Shutdown(SocketShutdown.Both);
                        }
                        catch
                        {
                        }

                        try
                        {
                            s.Close();
                        }
                        catch
                        {
                        }
                        s = null;
                        return;
                    }
                    
                    
                }
            }
            catch (System.Security.SecurityException ex)
            {
                Console.WriteLine("firewall says no no to application - application cries..");
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("stoped listening..");
            }
        }

        private static string Format(string msg, params object[] ps)
        {
            if (ps.Length > 0)
            {
                msg = string.Format(msg, ps);
            }
            return msg;
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
