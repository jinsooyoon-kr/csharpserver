using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int cnt = 0;
            while (true)
            {

                new Thread(new ThreadStart(delegate
                {


                    TcpClient clnt = new TcpClient();
                    clnt.Connect("127.0.0.1", 8000);
                    String data = DateTime.Now.ToString();

                    MemoryStream ms = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(ms);
                    bw.Write(1);
                    bw.Write(2);
                    bw.Write(data);
                    clnt.Client.Send(ms.GetBuffer());

                })).Start();
                Console.WriteLine(cnt++);
                Thread.Sleep(10);
            }
        }
    }
}
