using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ZXClient.service
{
    public class LYLWListen
    {
        private Thread th;
        private TcpListener tcpl;
        public bool listenerRun = true;

        public LYLWListen()
        {
            th = new Thread(new ThreadStart(Listen));
            th.Start();
        }

        private void Listen()
        {
            try
            {
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                tcpl = new TcpListener(ip, 9901);
                tcpl.Start();
                Console.WriteLine("started listening..");
                while (listenerRun)
                {
                    Socket s = tcpl.AcceptSocket();
                    string remote = s.RemoteEndPoint.ToString();
                    Byte[] stream = new Byte[1024];
                    int read = 0;
                    while ((read = s.Receive(stream)) > 0)
                    {
                        string msg = "<" + remote + ">" + System.Text.Encoding.UTF8.GetString(stream, 0, read);
                        string msg2 = "<" + remote + ">" + stream.Length;
                        Console.WriteLine(read + msg);
                    }
                }
            }
            catch (System.Security.SecurityException)
            {
                Console.WriteLine("firewall says no no to application - application cries..");
            }
            catch (Exception)
            {
                Console.WriteLine("stoped listening..");
            }
        }
    }
}
