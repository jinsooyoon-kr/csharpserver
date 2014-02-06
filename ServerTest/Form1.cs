using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using wjfeo_dksruqclsms_spdlatmvpdltm.core;
using wjfeo_dksruqclsms_spdlatmvpdltm;
using System.IO;
using System.Threading;
using System.Diagnostics;
namespace ServerTest
{
    /// <summary>
    /// HTTP Server Example
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public void setLabel(){
            this.BeginInvoke(new MethodInvoker(delegate
            {
                this.label1.Text = "Connection : " + connector.connection.ToString();
            }));
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(
                delegate
                {
                    while (true)
                    {
                        setLabel();
                        Thread.Sleep(1000);
                    }
                }
                )).Start();

            connector.frm = this;

                PacketPolicy policy = new PacketPolicy();
                customFilter gf = new customFilter(policy);
                AsyncServer server = new AsyncServer(8888, new PacketConv(), gf);
                server.setExceptionHandler(new ExceptionHandler((c, ex) =>
                {
                    if (ex is HTTPException)
                    {
                        HTTPException he = ex as HTTPException;
                        String msg = "";
                        switch (he.code)
                        {
                            case 404:
                                msg = "HTTP/1.1 404 Not Found\r\nConnection: Close\r\nContent-Length: 8\r\nContent-Type:text/html; charset=UTF-8\r\n\r\n404Error";
                                break;
                        }
                        customPacket cp = new customPacket();


                        cp.str = msg;

                        c.Send(cp);
                        c.Close();
                    }
                }));

                server.addFilter(new Filter((hd) =>
                {
                        bool ret =  File.Exists((hd as customHeader).path.Substring(1));
                        if (!ret) throw new HTTPException(404);
                        return ret;
                },
                (ct) =>
                {
                        customPacket c = ct.packet as customPacket;
                        String d = File.ReadAllText(((customHeader)c.getHeader()).path.Substring(1));
                        String value = "<h6>current directory is " + ((customHeader)c.getHeader()).path + "</h6><br>" + d;
                        int contentLength = value.Length;
                        String data = String.Format("HTTP/1.1 200 OK\r\ncontent-length: {0}\r\nContent-Type:text/html; charset=UTF-8\r\n\r\n" + value, contentLength);
                        customPacket send = new customPacket();
                        Thread.Sleep(1000);
                        send.str = data;
                        ct.Send(send);
                        ct.Close();
                    return true;
                }));

//                server.Map(c => { c.socket.Send(new byte[3]); }, uid => uid != 1);
                server.run();

                // continous packet test
                AsyncServer conServer = new AsyncServer(8000, new conPacketConv(), gf);

                conServer.addFilter(new Filter(Header => {

                    return true;
                }
                , context => {
                    Debug.WriteLine("CONTINOUS RECV PACKET - " + ((conPacket)context.packet).str);
                    return true;
                }

                ));

                conServer.run();

        }
    }
    public class conHeader : Header
    {
        public int data;
    }
    public class conPacket : Packet
    {
        public int data;
        public String str;
        conHeader header;
        public conPacket() { header = new conHeader(); }
        public Header getHeader() { return header; }

    }
    public class conPacketConv : PacketConverter
    {
        public Packet deserialize(BinaryReader br)
        {
            conPacket pack = new conPacket();

            ((conHeader)pack.getHeader()).data = br.ReadInt32();
            pack.data = br.ReadInt32();
            pack.str = br.ReadString();

            return pack;
        }
        public byte[] serialize(Packet packet)
        {
            MemoryStream ms = new MemoryStream();

            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(1);
            bw.Write(1);
            bw.Write("agsag");

            return ms.GetBuffer();
        }

    }

    public class customFilter : GlobalFilter
    {
        override public long onAccept(Client Client)
        {
            long uid = wjfeo_dksruqclsms_spdlatmvpdltm.Utility.UID.uid;
            connector.connection++;
            connector.updateLabel();
            Debug.WriteLine("OnAccept" + uid);
            return uid;
            

        }
        override public void onClose(long uid)
        {
            Debug.WriteLine("OnClose" + uid.ToString());
            connector.connection--;
        }

        public customFilter(PacketPolicy pp)
            : base(pp)
        {
        }

    }
    public class HTTPException : Exception
    {
        public int code{get; private set;}
        public HTTPException(int code)
        {
            this.code = code;
        }
        
    }
    public class customHeader : Header
    {
        public String method;
        public String path;
        public String ver;
        public Dictionary<String, String> headers = new Dictionary<string, string>();

    }
    public class customPacket : Packet
    {
        public String str;
        public customHeader header = new customHeader();
        public Header getHeader() { return header; }
    }
    public static class connector
    {
        public static void updateLabel()
        {
            frm.setLabel();
        }
        public static Form1 frm;
        public static int connection = 0;
        
    }
    public class PacketConv : PacketConverter
    {
       public Packet deserialize(System.IO.BinaryReader br)
        {
            customPacket ret = new customPacket();
            BinaryReader sr = new BinaryReader(br.BaseStream);

            String getdata = sr.ReadString();
            String fHeader = getdata.Split('\n')[0];
            ((customHeader)ret.getHeader()).method = fHeader.Split(' ')[0];
            ((customHeader)ret.getHeader()).path = fHeader.Split(' ')[1];
            ((customHeader)ret.getHeader()).ver = fHeader.Split(' ')[2];
           foreach (var t in from str in getdata.Split('\n').Reverse().Skip(1).Reverse().Skip(1) select new {
               Key = str.Split(':')[0],
               Val = str.Split(':')[1]
           }){
                ((customHeader)ret.getHeader()).headers[t.Key.Trim()] = t.Val.Trim();
           }

           
           ret.str = getdata.Split('\n').Last();
            return ret;
        }

       public byte[] serialize(Packet packet)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            customPacket cp = packet as customPacket;
            bw.Write(cp.str);

            return ms.GetBuffer();
        }
    }
    
}
