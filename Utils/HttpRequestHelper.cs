using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Utils
{
    public static class HttpRequestHelper
    {
        #region HttpWebRequest异步GET
        public static void AsyncGetWithWebRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.BeginGetResponse(new AsyncCallback(ReadCallback), request);
        }
        private static void ReadCallback(IAsyncResult asynchronousResult)
        {
            var request = (HttpWebRequest)asynchronousResult.AsyncState;
            var response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var resultString = streamReader.ReadToEnd();
                Console.WriteLine(resultString);
            }
        }
        #endregion
        #region WebClient异步GET
        public static void AsyncGetWithWebClient(string url, DownloadStringCompletedEventHandler act)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += act;
            webClient.DownloadStringAsync(new Uri(url));
        }
        private static void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            //Console.WriteLine(e.Cancelled);
            Console.WriteLine(e.Error != null ? "WebClient异步GET发生错误！" : e.Result);
        }
        #endregion
        #region WebClient的OpenReadAsync测试

        public static void TestGetWebResponseAsync(string url)
        {
            var webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);
            webClient.OpenReadAsync(new Uri(url));
        }

        private static void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                var streamReader = new StreamReader(e.Result);
                var result = streamReader.ReadToEnd();
            }
            else
            {
                Console.WriteLine("执行WebClient的OpenReadAsync出错：" + e.Error);
            }
        }
        #endregion
    }
}
