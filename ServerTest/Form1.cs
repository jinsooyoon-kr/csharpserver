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
            try
            {
                PacketPolicy policy = new PacketPolicy();
                customFilter gf = new customFilter(policy);                                
                AsyncServer server = new AsyncServer(new PacketConv(), 8888, gf);
                server.setExceptionHandler(new ExceptionHandler((c, ex) =>
                {
                    if (ex is HTTPException)
                    {
                        HTTPException he = ex as HTTPException;
                        String msg = "";
                        switch (he.code)
                        {
                            case 404:
                                msg = "HTTP/1.1 404 Not Found\r\nConnection: Close\r\nContent-Length: 7\r\nContent-Type:text/html; charset=UTF-8\r\n\r\n死0死";
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

                        send.str = data;
                        ct.Send(send);
                        ct.Close();
                    return true;
                }));

//                server.ForEach(c => { c.socket.Send(new byte[3]); }, uid => uid != 1);
                // foreach : function, condition


                server.run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);

            }
        }
    }
    public class customFilter : GlobalFilter
    {
        override public long onAccept(AsyncServer.Client Client)
        {
            connector.connection++;
            connector.updateLabel();
            return wjfeo_dksruqclsms_spdlatmvpdltm.Utility.UID.uid;
            

        }
        override public void onClose(long uid)
        {
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
