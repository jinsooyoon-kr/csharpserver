using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;
using System.IO;
/**************************
 * 
 *     2014. 01. 09
 *     
 * 
 * 
 * 
 * 
 * 
 *      BSD
 * ***********************/


namespace wjfeo_dksruqclsms_spdlatmvpdltm
{

    public static class define
    {
        public enum SOCKETTYPE
        {
            TCP,
            UDP
        }
        public enum PACKETRULE
        {
            RECV,
            SEND
        }
        public static int BUFFERSIZE = 1024;

    }

    namespace core
    {
        #region unused
        /*
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public unsafe struct OVERLAPPED
        {
            UInt32* ulpInternal;
            UInt32* ulpInternalHigh;
            Int32 lOffset;
            Int32 lOffsetHigh;
            UInt32 hEvent;
        }
        public sealed class IOCPThreadPool
        {
            [DllImport("Kernel32", CharSet = CharSet.Auto)]
            private unsafe static extern UInt32 CreateIoCompletionPort(UInt32 hFile, UInt32 hExistingCompletionPort, UInt32* puiCompletionKey, UInt32 uiNumberOfConcurrentThreads);
            [DllImport("Kernel32", CharSet = CharSet.Auto)]
            private unsafe static extern Boolean CloseHandle(UInt32 hObject);
            [DllImport("Kernel32", CharSet = CharSet.Auto)]
            private unsafe static extern Boolean PostQueuedCompletionStatus(UInt32 hCompletionPort, UInt32 uiSizeOfArgument, UInt32* puiUserArg, OVERLAPPED* pOverlapped);
            [DllImport("Kernel32", CharSet = CharSet.Auto)]
            private unsafe static extern Boolean GetQueuedCompletionStatus(UInt32 hCompletionPort, UInt32* pSizeOfArgument, UInt32* puiUserArg, OVERLAPPED** ppOverlapped, UInt32 uiMilliseconds);
            private const UInt32 INVALID_HANDLE_VALUE = 0xffffffff;
            private const UInt32 INFINITE = 0xffffffff;
            private const Int32 SHUTDOWN_IOCPTHREAD = 0x7fffffff;

            ///<summary> thread pool 파라미터용 함수</summary>
            public delegate void func(Int32 iValue);
            private UInt32 m_hHandle;
            private UInt32 GetHandle { get { return m_hHandle; } set { m_hHandle = value; } }
            private Int32 m_uiMaxConcurrency;
            private Int32 GetMaxConcurrency { get { return m_uiMaxConcurrency; } set { m_uiMaxConcurrency = value; } }
            private Int32 m_iMinThreadsInPool;
            private Int32 GetMinThreadsInPool { get { return m_iMinThreadsInPool; } set { m_iMinThreadsInPool = value; } }
            private Int32 m_iMaxThreadsInPool;
            private Int32 GetMaxThreadsInPool { get { return m_iMaxThreadsInPool; } set { m_iMaxThreadsInPool = value; } }
            private Object m_pCriticalSection;
            private Object GetCriticalSection { get { return m_pCriticalSection; } set { m_pCriticalSection = value; } }
            private func m_pfnWorkerFunction;
            private func GetUserFunction { get { return m_pfnWorkerFunction; } set { m_pfnWorkerFunction = value; } }
            private Boolean m_bDisposeFlag;
            private Boolean IsDisposed { get { return m_bDisposeFlag; } set { m_bDisposeFlag = value; } }
            private Int32 m_iCurThreadsInPool;
            public Int32 GetCurThreadsInPool { get { return m_iCurThreadsInPool; } set { m_iCurThreadsInPool = value; } }
            private Int32 IncreaseCurThreadsInPool() { return Interlocked.Increment(ref m_iCurThreadsInPool); }
            private Int32 DecreaseCurThreadsInPool() { return Interlocked.Decrement(ref m_iCurThreadsInPool); }
            private Int32 m_iActThreadsInPool;
            public Int32 GetActThreadsInPool { get { return m_iActThreadsInPool; } set { m_iActThreadsInPool = value; } }
            private Int32 IncreaseActThreadsInPool() { return Interlocked.Increment(ref m_iActThreadsInPool); }
            private Int32 DecreaseActThreadsInPool() { return Interlocked.Decrement(ref m_iActThreadsInPool); }
            private Int32 m_iCurWorkInPool;
            public Int32 GetCurWorkInPool { get { return m_iCurWorkInPool; } set { m_iCurWorkInPool = value; } }
            private Int32 IncreaseCurWorkInPool() { return Interlocked.Increment(ref m_iCurWorkInPool); }
            private Int32 DecreaseCurWorkInPool() { return Interlocked.Decrement(ref m_iCurWorkInPool); }
            /// <summary> Constructor </summary>

            /// <param name = "MaxConcurrency"> 최대 쓰레드 허용 갯수 </param>
            /// <param name = "MinThreadsInPool"> 쓰레드풀 쓰레드 최소 갯수 </param>
            /// <param name = "MaxThreadsInPool"> 쓰레드풀 쓰레드 최대 갯수 </param>
            /// <param name = "UserFunction"> 워커쓰레드 함수 </param>
            /// <exception cref = "Exception"> Unhandled Exception </exception>
            /// 
            public IOCPThreadPool(Int32 MaxConcurrency, Int32 MinThreadsInPool, Int32 MaxThreadsInPool, func UserFunction)
            {

                try
                {
                    GetMaxConcurrency = MaxConcurrency;
                    GetMinThreadsInPool = MinThreadsInPool;
                    GetMaxThreadsInPool = MaxThreadsInPool;
                    GetUserFunction = UserFunction;


                    GetCurThreadsInPool = 0;
                    GetActThreadsInPool = 0;
                    GetCurWorkInPool = 0;
                    GetCriticalSection = new Object();
                    IsDisposed = false;

                    unsafe
                    {
                        GetHandle = CreateIoCompletionPort(INVALID_HANDLE_VALUE, 0, null, (UInt32)GetMaxConcurrency);
                    }

                    if (GetHandle == 0) throw new Exception("Unable To Create IO Completion Port"); // GetLastError() 매핑 필요
                    Int32 iStartingCount = GetCurThreadsInPool;
                    ThreadStart tsThread = new ThreadStart(() => { return; }); // 뭔가 넣어야함

                    for (Int32 iThread = 0; iThread < GetMinThreadsInPool; ++iThread)
                    {

                        Thread thThread = new Thread(tsThread);

                        thThread.Name = "IOCP_" + iThread;

                        thThread.Start();

                        IncreaseCurThreadsInPool();

                    }

                }

                catch
                {
                    throw new Exception("Unhandled Exception");
                }
            }

            ~IOCPThreadPool()
            {

                if (!IsDisposed)

                    Dispose();

            }

            public void Dispose()
            {
                IsDisposed = true;
                Int32 iCurThreadsInPool = GetCurThreadsInPool;
                for (Int32 iThread = 0; iThread < iCurThreadsInPool; ++iThread)
                    unsafe
                    {
                        bool bret = PostQueuedCompletionStatus(GetHandle, 4, (UInt32*)SHUTDOWN_IOCPTHREAD, null);
                    }
                while (GetCurThreadsInPool != 0) Thread.Sleep(100);

                unsafe
                {
                    CloseHandle(GetHandle);
                }

            }
        }
        public class IOCPServer // 나중에 사용
        {
            private int m_cpuNumber { get; set; }
            private int m_threadNumber { get; set; }

            private void __Init()
            {

            }
            private void __Work()
            {

            }
            private void __NativeSend()
            {

            }
            private void __NativeRecv()
            {

            }
            public IOCPServer()
            {

            }




        }
         * */
        #endregion

        
        
//        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public interface Header
        {
            
        }
//        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public interface Packet
        {
            Header getHeader();
        }
        public interface PacketConverter
        {
             byte[] serialize(Packet packet);
             Packet deserialize(BinaryReader br);
        }
        public class Context
        {
            public Packet packet;
            private PacketConverter packetconverter;
            private GlobalFilter gf;
            private AsyncServer.Client clnt;
            public void Send(Packet packet)
            {
                byte[] bArray = packetconverter.serialize(packet);
                if(gf.policy.beforeSend != null)
                  gf.policy.beforeSend(this, bArray);
                clnt.tmpbuffer = bArray;
                
                clnt.socket.BeginSend(bArray, 0, bArray.Length, 0, new AsyncCallback(__sendCallback), clnt);
 

            }
            public void Close()
            {
                clnt.onClose();

            }
            private void __sendCallback(IAsyncResult r)
            {
                AsyncServer.Client clnt = (AsyncServer.Client)r.AsyncState;

                Context ct = new Context(packetconverter, gf, clnt);
                
                if (gf.policy.afterSend != null)
                    gf.policy.afterSend(ct, clnt.tmpbuffer);
            }
            private Context() { }
            public Context(PacketConverter pc, GlobalFilter gf, AsyncServer.Client client)
            {
                this.clnt = client;
                this.gf = gf;
                this.packetconverter = pc;
            }
        }
        
        public delegate bool HeaderVerify(Header header);
        public delegate bool PacketHandleFunction(Context context);
        public delegate Packet packetRules(byte[] data);
        public delegate void ExceptionHandler(Context context, Exception exception);
        public class AsyncServer
        {
            private ExceptionHandler __eh;
            private PacketConverter __pc;
            public class Client
            {
                bool isClose = false;
                public long uid { get; set; }
                public byte[] buffer = new byte[wjfeo_dksruqclsms_spdlatmvpdltm.define.BUFFERSIZE];
                public int __recvsize;
                public byte[] tmpbuffer = new byte[wjfeo_dksruqclsms_spdlatmvpdltm.define.BUFFERSIZE];
                public Socket socket;
                private GlobalFilter gf;
                public void onClose()
                {
                    if (!isClose)
                    {
                        try
                        {
                            isClose = true;
                            gf.onClose(uid);
                            socket.Close();
                        }
                        catch (Exception)
                        {
                            // socket 이미 닫힘
                        }
                    }
                }
                public void recv_init()
                {
                    __recvsize = 0;
                    
                }
                public Client(Socket _socket, long uid, GlobalFilter globalFilter) { socket = _socket; this.uid = uid; gf = globalFilter; }
                private Client() { }
            }
            private Socket __temp;
            private Socket __listener;
            public int port { private set; get; }
            public void setExceptionHandler(ExceptionHandler exceptionHandler)
            {
                __eh = exceptionHandler;
            }
            private AsyncServer() { }
            public AsyncServer(PacketConverter packetconverter, int port, GlobalFilter globalFilter)
            {
                this.port = port;
                __pc = packetconverter;
                globalhandler = globalFilter;
            }
            private Dictionary<long, Client> client = new Dictionary<long, Client>();
//            private packetRules packetrules;
            private GlobalFilter globalhandler = new GlobalFilter(new PacketPolicy());
            private List<Filter> rules = new List<Filter>();
            public void setGlobalHandler(GlobalFilter globalHandler)
            {
                globalhandler = globalHandler;
            }
            private void __nativeAcceptCallback(IAsyncResult r)
            {
                __listener = r.AsyncState as Socket;
                __temp = __listener.EndAccept(r);
                Client clnt = new Client(__temp, -1, this.globalhandler);
                long uid = globalhandler.onAccept(clnt);
                clnt.uid = uid;
                client[uid] = clnt;
                __temp.BeginReceive(client[uid].tmpbuffer, 0, client[uid].tmpbuffer.Length, SocketFlags.None, new AsyncCallback(__nativeRecvCallback), client[uid]);
                __listener.BeginAccept(new AsyncCallback(__nativeAcceptCallback), __listener);
            }
            private void __nativeRecvCallback(IAsyncResult r)
            {
                
                    Client clnt = (Client)r.AsyncState;
                try
                {

                    int recvsize = clnt.socket.EndReceive(r);
                    if (recvsize <= 0)
                    {
                        clnt.onClose();

                        return;
                    }
                    if (recvsize > 0)
                    {
                        int bufferlen = clnt.__recvsize;
                        clnt.__recvsize += recvsize;
                        Array.Resize(ref clnt.buffer, recvsize + bufferlen);
                        System.Buffer.BlockCopy(clnt.tmpbuffer, 0, clnt.buffer, bufferlen, recvsize);

                    }
                    if (recvsize == define.BUFFERSIZE)
                        clnt.socket.BeginReceive(clnt.tmpbuffer, 0, clnt.tmpbuffer.Length, SocketFlags.None, new AsyncCallback(__nativeRecvCallback), clnt);
                    else
                    {
                        Context context = new Context(__pc, this.globalhandler, clnt);

                        context.packet = __pc.deserialize(new BinaryReader(new MemoryStream(clnt.buffer)));
                        __onRecv(context);
                        clnt.recv_init();
                        clnt.socket.BeginReceive(clnt.tmpbuffer, 0, clnt.tmpbuffer.Length, SocketFlags.None, new AsyncCallback(__nativeRecvCallback), clnt);
                    }
                }
                catch (Exception exc)
                {
                    if (this.__eh != null) __eh(new Context(__pc, this.globalhandler, clnt), exc);
                    clnt.onClose();
                }

            }
            private void __nativeStart()
            {
                
                __listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                __listener.UseOnlyOverlappedIO = true;
                __listener.Bind(new IPEndPoint(IPAddress.Any, port));
                __listener.Listen(10);
                    IAsyncResult result = __listener.BeginAccept(new AsyncCallback(__nativeAcceptCallback), __listener);

            }
            public void run()
            {
                __nativeStart();
            }
            public void addFilter(Filter rule)
            {
                rules.Add(rule);
            }

            private void __nativeSend(byte[] sendData, Socket client)
            {

            }

            
            private void __onRecv(Context context)
            {
                bool is_filter = false;

                if(globalhandler.policy.beforeRecv != null)
                    globalhandler.policy.beforeRecv(context); 
                foreach (Filter r in rules)
                {
                    if (r.condition(context.packet.getHeader()))
                    {
                        is_filter = true;
                        r.rule(context);
                    }

                }
                if (!is_filter)
                {
                    if(globalhandler.policy.errorHandler != null)
                        globalhandler.policy.errorHandler(context);
                }
                

                if(globalhandler.policy.afterRecv != null)
                    globalhandler.policy.afterRecv(context);
            }
            private void __onSend(Context context)
            {
            }

            private void __init()
            {

            }
            private void __cleanup()
            {

            }
            private void __onClose(long uid)
            {
                globalhandler.onClose(uid);
                client[uid].socket.Close();

            }
            public Client getClientByUID(long uid) { return client[uid]; }
            public void ForEach(ForeachFunction f, Func<long, bool> where)
            {


                foreach (var key in client.Keys.Where(where)) { f(client[key]); }


            }
        }

        public delegate void ForeachFunction(AsyncServer.Client client);
        public delegate bool SendHandleFunction(Context context, byte[] sendbuffer);
        public class PacketPolicy
        {
            public SendHandleFunction beforeSend;
            public SendHandleFunction afterSend;
            public PacketHandleFunction afterRecv;
            public PacketHandleFunction beforeRecv;

            public PacketHandleFunction errorHandler; // 에러가 아니라 패킷 필터링 규칙에 걸리지 않는 패킷들 처리
        }

        public class GlobalFilter
        {
            public PacketPolicy policy;
            public virtual long onAccept(AsyncServer.Client client) { return Utility.UID.uid; } // must return uid
            public virtual void onClose(long uid) { }
            public GlobalFilter(PacketPolicy packetPolicy) { policy = packetPolicy; }
        }
        public class Handler
        {
            public HeaderVerify conditionFunction; // 이 조건과 Header가 맞아야 패킷 핸들링
            public PacketHandleFunction recvHandler;
            public PacketHandleFunction sendHandler;
        }
        
        public class Filter
        {
            public PacketHandleFunction rule;
            public HeaderVerify condition;

            public Filter(HeaderVerify condition, PacketHandleFunction rule)
            {
                this.rule = rule;
                this.condition = condition;
            }
            private Filter() { }
        }


    }
    namespace Utility
    {
        public static class UID
        {
            private static long __lastuid;
            public static long uid { get { long ret = DateTime.Now.Ticks; if (ret <= __lastuid) while (ret > __lastuid) ret++; __lastuid = ret; return ret; } } // 버그 쩌는데 테스트용
        }
    }
}