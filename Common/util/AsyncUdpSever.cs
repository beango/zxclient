using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Common.util
{
    // 定义 UdpState类
    public class UdpState
    {
        public UdpClient udpClient;
        public IPEndPoint ipEndPoint;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public int counter = 0;
    }
    // 异步UDP类
    public class AsyncUdpSever
    {
        // 定义节点
        private IPEndPoint ipEndPoint = null;
        //private IPEndPoint remoteEP = null;
        // 定义UDP发送和接收
        private UdpClient udpReceive = null;
        //private UdpClient udpSend = null;
        // 定义端口
        //private const int listenPort = 8001;
        //private const int remotePort = 1101;
        UdpState udpReceiveState = null;
        UdpState udpSendState = null;
        // 异步状态同步
        private ManualResetEvent sendDone = new ManualResetEvent(false);
        private ManualResetEvent receiveDone = new ManualResetEvent(false);

        public delegate void MyDelegate(String strInfo, UdpClient client, IPEndPoint remoteIpep);
        MyDelegate myde;

        public AsyncUdpSever(MyDelegate _myde, int listenPort)
        {
            myde = _myde;

            ipEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
            udpReceive = new UdpClient();
            //udpReceive.ExclusiveAddressUse = false;
            //udpReceive.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpReceive.Client.Bind(ipEndPoint);


            udpReceiveState = new UdpState();
            udpReceiveState.udpClient = udpReceive;
            udpReceiveState.ipEndPoint = ipEndPoint;

            udpSendState = new UdpState();
        }

        public void ReceiveMsg()
        {
            if (true)
            {
                lock (this)
                {
                    // 调用接收回调函数
                    IAsyncResult iar = udpReceive.BeginReceive(new AsyncCallback(ReceiveCallback), udpReceiveState);
                    receiveDone.WaitOne();
                    //Thread.Sleep(1000);
                }
            }
        }

        // 接收回调函数
        private void ReceiveCallback(IAsyncResult iar)
        {
            UdpState udpReceiveState = iar.AsyncState as UdpState;
            if (iar.IsCompleted)
            {
                Byte[] receiveBytes = udpReceiveState.udpClient.EndReceive(iar, ref udpReceiveState.ipEndPoint);
                string receiveString = Encoding.Default.GetString(receiveBytes);
                Console.WriteLine("Received: {0}", receiveString);
                //Thread.Sleep(100);
                receiveDone.Set();
                //SendMsg();
                udpReceive.BeginReceive(new AsyncCallback(ReceiveCallback), udpReceiveState);
                if(null != myde)
                    myde(receiveString, udpReceiveState.udpClient, udpReceiveState.ipEndPoint);
            }
        }

        // 发送函数
        private void SendMsg()
        {
            //udpSend.Connect(udpSendState.ipEndPoint);
            //udpSendState.udpClient = udpSend;
            //udpSendState.counter++;

            //string message = string.Format("第{0}个UDP请求处理完成！", udpSendState.counter);
            //Byte[] sendBytes = Encoding.Unicode.GetBytes(message);
            //udpSend.BeginSend(sendBytes, sendBytes.Length, new AsyncCallback(SendCallback), udpSendState);
            //sendDone.WaitOne();
        }
        // 发送回调函数
        private void SendCallback(IAsyncResult iar)
        {
            UdpState udpState = iar.AsyncState as UdpState;
            Console.WriteLine("第{0}个请求处理完毕！", udpState.counter);
            Console.WriteLine("number of bytes sent: {0}", udpState.udpClient.EndSend(iar));
            sendDone.Set();
        }
        // 主函数
        public static void Main1()
        {
            AsyncUdpSever aus = new AsyncUdpSever(null, 8001);
            Thread t = new Thread(new ThreadStart(aus.ReceiveMsg));
            t.Start();
            Console.Read();
        }
    }
}
