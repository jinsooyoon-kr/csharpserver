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
using System.Diagnostics;
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
        */
        #endregion
        
        
        
//        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public interface Header
        {
            
        }
//        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        /// <summary>
        /// <remarks>getHeader() 무조건 구현해야 함</remarks>
        /// </summary>
        public interface Packet
        {
            Header getHeader();
        }

        /// <summary>
        /// serialize : packet -> byte array
        /// deserialize : binaryReader -> Packet
        /// </summary>
        public interface PacketConverter
        {
            /// <summary>
            /// 패킷을 시리얼라이징하는 함수
            /// </summary>
            /// <param name="packet">시리얼라이징 할 패킷</param>
            /// <returns>byte[] 화 된 패킷</returns>
             byte[] serialize(Packet packet);
            /// <summary>
            /// 패킷을 디시리얼라이징하는 함수
            /// </summary>
            /// <param name="br">Binary Reader Stream</param>
            /// <returns>패킷</returns>
            /// <remarks>한번에 패킷 여러개가 오면 패킷이 겹칠 수 있으므로 패킷겹침현상 처리 필요</remarks>
             Packet deserialize(BinaryReader br);
        }

        public class Context
        {
            public Packet packet;
            private PacketConverter packetconverter;
            private GlobalFilter gf;
            private Client clnt;
            /// <summary>
            /// 패킷 Send
            /// </summary>
            /// <param name="packet">보낼 패킷</param>
            public void Send(Packet packet)
            {
                byte[] bArray = packetconverter.serialize(packet);
                if(gf.policy.beforeSend != null)
                  gf.policy.beforeSend(this, packet);
                clnt.tmpbuffer = bArray;
                clnt.socket.BeginSend(bArray, 0, bArray.Length, 0, new AsyncCallback(__sendCallback), new ClientWrapper(clnt, null, packet));
            }

            /// <summary>
            /// 패킷 Send 후 Callback 호출
            /// </summary>
            /// <param name="packet">패킷</param>
            /// <param name="callback">콜백함수. 추가로 콜백이 필요하면 2번째 파라미터에 PacketCallbackFunction을 넣고 아니면 null을 넣어서 처리(미구현)</param>
            public void Send(Packet packet, PacketCallbackFunction callback)
            {
                byte[] bArray = packetconverter.serialize(packet);
                if (gf.policy.beforeSend != null)
                    gf.policy.beforeSend(this, packet);
                clnt.tmpbuffer = bArray;
                clnt.socket.BeginSend(bArray, 0, bArray.Length, 0, new AsyncCallback(__sendCallbackWithCallbackFunction), new ClientWrapper(clnt, callback, packet));
            }
            private class ClientWrapper
            {
                public Client client;
                public PacketCallbackFunction callback;
                public Packet packet;
                public ClientWrapper(Client Client, PacketCallbackFunction Callback, Packet packet) { client = Client; callback = Callback; this.packet = packet; }
            }
            /// <summary>
            /// 현재 컨텍스트 클라이언트의 연결을 끊는다
            /// </summary>
            public void Close()
            {
                clnt.onClose();
            }
            private void __sendCallbackWithCallbackFunction(IAsyncResult r)
            {
                ClientWrapper wrapper = (ClientWrapper)r.AsyncState;
                Context ct = new Context(packetconverter, gf, clnt);
                if (gf.policy.afterSend != null)
                    gf.policy.afterSend(ct, wrapper.packet);
                wrapper.callback(packet, null);                

            }
            private void __sendCallback(IAsyncResult r)
            {
                ClientWrapper wrapper = (ClientWrapper)r.AsyncState;
                Context ct = new Context(packetconverter, gf, wrapper.client);                
                if (gf.policy.afterSend != null)
                    gf.policy.afterSend(ct, wrapper.packet);
            }
            private Context() { }
            internal Context(PacketConverter pc, GlobalFilter gf, Client client)
            {
                this.clnt = client;
                this.gf = gf;
                this.packetconverter = pc;
            }
        }
        
        /// <summary>
        /// 헤더를 이용해 조건 판별. true 리턴시 PacketHandleFunction Filter 작동
        /// </summary>
        /// <param name="header">Packet.getHeader()로 받아온 헤더</param>
        /// <returns>성공시 true, 실패시 false 구현</returns>
        public delegate bool HeaderVerify(Header header);
        /// <summary>
        /// PacketHandleFunction에 대한 Filter
        /// </summary>
        /// <param name="context">현재 Context</param>
        /// <returns>성공시 true, 실패시 false</returns>
        public delegate bool PacketHandleFunction(Context context);
        public delegate Packet packetRules(byte[] data);
        public delegate void PacketCallbackFunction(Packet packet, PacketCallbackFunction callback);
        /// <summary>
        /// Exception Handler가 필요할 경우 구현
        /// </summary>
        /// <param name="context">현재 Context</param>
        /// <param name="exception">발생한 Exception</param>
        public delegate void ExceptionHandler(Context context, Exception exception);
        public class Client
        {
            bool isClose = false;
            /// <summary>
            /// onAccept()에서 받아온 UID
            /// </summary>
            public long uid { get; internal set; }
            internal byte[] buffer = new byte[wjfeo_dksruqclsms_spdlatmvpdltm.define.BUFFERSIZE];
            internal int __recvsize;
            internal byte[] tmpbuffer = new byte[wjfeo_dksruqclsms_spdlatmvpdltm.define.BUFFERSIZE];
            internal Socket socket;
            private GlobalFilter gf;
            /// <summary>
            /// 해당 클라이언트 Close()
            /// </summary>
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
            internal void recv_init()
            {
                buffer = new byte[wjfeo_dksruqclsms_spdlatmvpdltm.define.BUFFERSIZE];
                tmpbuffer = new byte[wjfeo_dksruqclsms_spdlatmvpdltm.define.BUFFERSIZE];
                __recvsize = 0;

            }
            internal Client(Socket _socket, long uid, GlobalFilter globalFilter) { socket = _socket; this.uid = uid; gf = globalFilter; }
            private Client() { }

            public void setNoDelay(bool isNoDelay)
            {                
                socket.NoDelay = isNoDelay;
            }
            public void sendSync(byte[] buffer)
            {                
                socket.Send(buffer);
            }
            public byte[] recvSync(int size)
            {
                byte[] buffer = new byte[size];
                socket.Receive(buffer);
                return buffer;
            }

        }
        public class AsyncServer
        {
            private ExceptionHandler __eh;
            private PacketConverter __pc;
            private Socket __temp;
            private Socket __listener;
            public int port { private set; get; }
            public void setExceptionHandler(ExceptionHandler exceptionHandler)
            {
                __eh = exceptionHandler;
            }
            private AsyncServer() { }
            public AsyncServer(int port, PacketConverter packetconverter, GlobalFilter globalFilter)
            {
                this.port = port;
                __pc = packetconverter;
                globalhandler = globalFilter;
            }
            public AsyncServer(int port, PacketConverter packetconverter)
            {
                this.port = port;
                __pc = packetconverter;
                globalhandler = new GlobalFilter(new PacketPolicy());
            }

            private Dictionary<long, Client> client = new Dictionary<long, Client>();
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
                        int bufferlen = clnt.__recvsize;
                        clnt.__recvsize += recvsize;
                        Array.Resize(ref clnt.buffer, recvsize + bufferlen);
                        System.Buffer.BlockCopy(clnt.tmpbuffer, 0, clnt.buffer, bufferlen, recvsize);
                        if (recvsize != define.BUFFERSIZE)
                        {
                            Context context = new Context(__pc, this.globalhandler, clnt);
                            context.packet = __pc.deserialize(new BinaryReader(new MemoryStream(clnt.buffer)));
                            __onRecv(context);
                            clnt.recv_init();
                            bufferlen = 0;
                        }
                        IAsyncResult result = clnt.socket.BeginReceive(clnt.tmpbuffer, 0, define.BUFFERSIZE, SocketFlags.None, new AsyncCallback(__nativeRecvCallback), clnt);

                    }
                    catch (ObjectDisposedException)
                    {
                        // 중간에 socket delete
                        clnt.onClose();

                    }
                    catch (Exception exc)
                    {
                        if (this.__eh != null) __eh(new Context(__pc, this.globalhandler, clnt), exc);
                    }

            }
            private void __nativeStart()
            {
                
                __listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                __listener.UseOnlyOverlappedIO = true;
                __listener.Bind(new IPEndPoint(IPAddress.Any, port));
                __listener.Listen(500);
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
                    if(globalhandler.policy.noFilterHandler != null)
                        globalhandler.policy.noFilterHandler(context);
                }
                if(globalhandler.policy.afterRecv != null)
                    globalhandler.policy.afterRecv(context);
            }
            private void __cleanup()
            {

            }
            private void __onClose(long uid)
            {
                globalhandler.onClose(uid);
                client[uid].socket.Close();
            }

            /// <summary>
            /// UID를 이용해 Client를 얻는다
            /// </summary>
            /// <param name="uid">UID</param>
            /// <returns>Client. 없으면 null</returns>
            public Client getClientByUID(long uid) { return client[uid]; }
            
            /// <summary>
            /// where 조건에 맞는 클라이언트들에 function()
            /// </summary>
            /// <param name="function">실행할 델리게이트</param>
            /// <param name="where">조건</param>
            /// <example>server.Map(c => { c.socket.Send(new byte[3]); }, uid => uid != 1);</example>
            public void Map(ForeachFunction function, Func<long, bool> where)
            {
                foreach (var key in client.Keys.Where(where)) { function(client[key]); }
            }
        }

        public delegate void ForeachFunction(Client client);
        public delegate bool SendHandleFunction(Context context, Packet packet);
        public class PacketPolicy
        {
            public SendHandleFunction beforeSend;
            public SendHandleFunction afterSend;
            public PacketHandleFunction afterRecv;
            public PacketHandleFunction beforeRecv;

            public PacketHandleFunction noFilterHandler; // 에러가 아니라 패킷 필터링 규칙에 걸리지 않는 패킷들 처리
        }

        public class GlobalFilter
        {
            public PacketPolicy policy;
            public virtual long onAccept(Client client) { return Utility.UID.uid; } // must return uid
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
            public static long uid { get { long ret = DateTime.Now.Ticks; if (ret <= __lastuid) while (ret > __lastuid) ret++; __lastuid = ret; return ret; } } // 기본 UID
        }
    }
}