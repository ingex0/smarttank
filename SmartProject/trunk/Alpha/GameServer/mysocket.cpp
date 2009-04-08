#include "mysocket.h"


#ifdef WIN32
    extern CRITICAL_SECTION DD_ClientMgr_Mutex[MAXCLIENT];  
#else
    extern int errno;
    extern pthread_mutex_t DD_ClientMgr_Mutex[MAXCLIENT];  
#endif
int MySocket::m_sinSize = sizeof(struct sockaddr_in);

MySocket::MySocket()
{
    m_socket = SOCKET_ERROR;
    memset(&m_addr, 0, m_sinSize);
}

//////////////////////////////////////////////////////////////////////////
// 析构并且关闭
MySocket::~MySocket()
{
    Close();
}
//////////////////////////////////////////////////////////////////////////
// 绑定套接字
void MySocket::Bind(int port)
{   
    // 创建套接字
    if ( ( m_socket = socket(AF_INET, SOCK_STREAM, 0)) == -1)
    {
        perror("socket");
        exit(1);
    }

    m_addr.sin_family = AF_INET;
    m_addr.sin_port   = htons(port);
    m_addr.sin_addr.s_addr   =  htonl(INADDR_ANY);
    memset(&m_addr.sin_zero, 0, 8);

    // bind套接字
    if ( bind( m_socket, (sockaddr*)(&m_addr), sizeof(sockaddr) ) == -1)
    {
        int errsv = errno;
        cout << errsv << endl;
        cin >> errsv;
        perror("Bind");
        exit(1);
    }

}
//////////////////////////////////////////////////////////////////////////
// 监听
void MySocket::Listen()
{
    // listen
    if ( listen( m_socket, MAXCLIENT ) == -1)
    {
        perror("listen");
        exit(1);
    }
}
//////////////////////////////////////////////////////////////////////////
// 接受套接字
int MySocket::Accept(MySocket *pSockConn)
{
    int sockConn;
    sockaddr_in addrClient;
    int len = sizeof(sockaddr);
    
    if ( -1 == ( sockConn = accept(m_socket, (sockaddr*)&addrClient,(socklen_t*)&len) ) )
    {
        perror("accept");
        return -1;
    }
    else
    {
        pSockConn->m_socket = sockConn;
        memcpy(&pSockConn->m_addr, &addrClient, MySocket::m_sinSize);
        sprintf(pSockConn->m_sIP,"%s", inet_ntoa((in_addr)addrClient.sin_addr));    
        return sockConn;
    }
}
//////////////////////////////////////////////////////////////////////////
// 连接 
bool MySocket::Connect( char* chIP, long port)
{
    m_addr.sin_family = AF_INET;
    m_addr.sin_port   = htons(MYPORT);
    m_addr.sin_addr.s_addr   =  (inet_addr(IP));
    memset(&m_addr.sin_zero, 0, 8);
    int err = connect( m_socket, (struct sockaddr*)(&m_addr), m_sinSize) ;
    
    if ( err == -1)
    {
        perror("connect"); 
        return false;
    }
    else
    {
        return true;
    }
    
}
//////////////////////////////////////////////////////////////////////////
// 发送
bool MySocket::Send( char* msg, long leng)
{
    if ( send( m_socket, msg, leng, 0) == -1)
    {
        perror("Send");
        return false;
    }
    else
    {
        return true;
    }
}
/************************************************************************/
/* 发送数据包                           
/************************************************************************/
bool MySocket::SendPacket(Packet& pack)
{
    /* 直接转发 */
    /*int realSize = pack.length + 8;
    if ( -1 == send( m_socket, (char*)&pack, realSize, MSG_NOSIGNAL) )
    {
        perror("SendPacket");
        cout << "Oh, 狗日的掉线了." << endl;
        return false;
    }*/
    return true;
}
bool MySocket::SendPacket( char *msg, long leng)
{
   /* Packet packOut;
    packOut.iStyle = CHAT;
    packOut.length = sizeof(Packet);
    memset(packOut.data, 0, PACKSIZE);
    strcpy(packOut.data, msg);
    if ( -1 == send( m_socket, (char*)&packOut, sizeof(Packet), MSG_NOSIGNAL) )
    {
        perror("SendPacket");
        cout << "Oh, 狗日的掉线了." << endl;
        return false;
    }*/

    return true;
}
/************************************************************************/
/* 接收数据包                           
/************************************************************************/
long MySocket::RecvPacket( Packet& pack )
{
    long ret = 0;
    // 收包
    if ( -1 == ( ret = recv(m_socket, (char*)&pack, sizeof(Packet), MSG_NOSIGNAL) ) )
    {
        cout << errno << " Receive: 哪个禽兽掉线了." << endl;
    }
    return ret;
}

//////////////////////////////////////////////////////////////////////////
// 关闭套接字
void MySocket::Close()
{
    // 清理工作
#ifdef WIN32 
    closesocket(m_socket);  
#else
    close(m_socket);
#endif

}
//////////////////////////////////////////////////////////////////////////
// 返回IP地址
char* MySocket::GetIP()
{
    return m_sIP;
}
//////////////////////////////////////////////////////////////////////////
// 返回端口号
long MySocket::GetPort()
{
    return (long)m_addr.sin_port;
}
// 返回端口号(字符串形式)
char* MySocket::GetPort(char *outPort)
{
    if( !outPort )
    {
        perror("outBuff is NULL");
        exit(1);
    }
    sprintf( outPort,"%d", m_addr.sin_port );    
    return outPort;
}



//////////////////////////////////////////////////////////////////////////
#ifndef WIN32
void signalProc(int sig)
{
    switch(sig)
    {
    case SIGPIPE:
       cout << "SIGPIPE";
        break;
    default:
        cout << "Unknow signal!";
        break;
    }
}
#endif
//////////////////////////////////////////////////////////////////////////
// 初始化套接字
void StartSocket()
{
#ifdef WIN32
    //初始化协议
    WSADATA wsaData;
    int err;    
    err = WSAStartup( MAKEWORD(2,2), &wsaData );
    if ( err != 0 ) {
        cout << "ERROR = " << err << endl;
        return;
	}
    for ( int i=0; i<MAXCLIENT; i++)
    { 
        InitializeCriticalSection(&DD_ClientMgr_Mutex[i]);
    }
#else
    for ( int i=0; i<MAXCLIENT; i++)
    { 
        pthread_mutex_init(&DD_ClientMgr_Mutex[i], NULL);
    }
    {

    
    // 断开的管道和非正常关闭socket有关 不进行信号处理
    signal(SIGHUP     ,SIG_IGN   );       /*   hangup,   generated   when   terminal   disconnects   */   
    signal(SIGINT     ,SIG_IGN   );       /*   interrupt,   generated   from   terminal   special   char   */   
    signal(SIGQUIT   ,SIG_IGN   );       /*   (*)   quit,   generated   from   terminal   special   char   */   
    signal(SIGILL     ,SIG_IGN   );       /*   (*)   illegal   instruction   (not   reset   when   caught)*/   
    signal(SIGTRAP   ,SIG_IGN   );       /*   (*)   trace   trap   (not   reset   when   caught)   */   
    signal(SIGABRT   ,SIG_IGN   );       /*   (*)   abort   process   */   
#ifdef   D_AIX   
    signal(SIGEMT     ,SIG_IGN   );       /*   EMT   intruction   */   
#endif   
    signal(SIGFPE     ,SIG_IGN   );       /*   (*)   floating   point   exception   */   
    signal(SIGKILL   ,SIG_IGN   );       /*   kill   (cannot   be   caught   or   ignored)   */   
    signal(SIGBUS     ,SIG_IGN   );       /*   (*)   bus   error   (specification   exception)   */   
    signal(SIGSEGV   ,SIG_IGN   );       /*   (*)   segmentation   violation   */   
    signal(SIGSYS     ,SIG_IGN   );       /*   (*)   bad   argument   to   system   call   */   
    signal(SIGPIPE   ,SIG_IGN   );       /*   write   on   a   pipe   with   no   one   to   read   it   */   
    signal(SIGALRM   ,SIG_IGN   );       /*   alarm   clock   timeout   */   
    //signal(SIGTERM   ,stopproc   );     /*   software   termination   signal   */   
    signal(SIGURG     ,SIG_IGN   );       /*   (+)   urgent   contition   on   I/O   channel   */   
    signal(SIGSTOP   ,SIG_IGN   );       /*   (@)   stop   (cannot   be   caught   or   ignored)   */   
    signal(SIGTSTP   ,SIG_IGN   );       /*   (@)   interactive   stop   */   
    signal(SIGCONT   ,SIG_IGN   );       /*   (!)   continue   (cannot   be   caught   or   ignored)   */   
    signal(SIGCHLD   ,SIG_IGN);         /*   (+)   sent   to   parent   on   child   stop   or   exit   */   
    signal(SIGTTIN   ,SIG_IGN);         /*   (@)   background   read   attempted   from   control   terminal*/   
    signal(SIGTTOU   ,SIG_IGN);         /*   (@)   background   write   attempted   to   control   terminal   */   
    signal(SIGIO       ,SIG_IGN);         /*   (+)   I/O   possible,   or   completed   */   
    signal(SIGXCPU   ,SIG_IGN);         /*   cpu   time   limit   exceeded   (see   setrlimit())   */   
    signal(SIGXFSZ   ,SIG_IGN);         /*   file   size   limit   exceeded   (see   setrlimit())   */   
    
#ifdef   D_AIX   
    signal(SIGMSG     ,SIG_IGN);         /*   input   data   is   in   the   ring   buffer   */   
#endif   
    
    signal(SIGWINCH,SIG_IGN);         /*   (+)   window   size   changed   */   
    signal(SIGPWR     ,SIG_IGN);         /*   (+)   power-fail   restart   */   
    //signal(SIGUSR1   ,stopproc);       /*   user   defined   signal   1   */   
    //signal(SIGUSR2   ,stopproc);       /*   user   defined   signal   2   */   
    signal(SIGPROF   ,SIG_IGN);         /*   profiling   time   alarm   (see   setitimer)   */   
    
#ifdef   D_AIX   
    signal(SIGDANGER,SIG_IGN);       /*   system   crash   imminent;   free   up   some   page   space   */   
#endif   
    
    signal(SIGVTALRM,SIG_IGN);       /*   virtual   time   alarm   (see   setitimer)   */   
    
#ifdef   D_AIX   
    signal(SIGMIGRATE,SIG_IGN);     /*   migrate   process   */   
    signal(SIGPRE     ,SIG_IGN);         /*   programming   exception   */   
    signal(SIGVIRT   ,SIG_IGN);         /*   AIX   virtual   time   alarm   */     
    signal(SIGALRM1,SIG_IGN);         /*   m:n   condition   variables   -   RESERVED   -   DON'T   USE   */   
    signal(SIGWAITING,SIG_IGN);     /*   m:n   scheduling   -   RESERVED   -   DON'T   USE   */   
    signal(SIGCPUFAIL   ,SIG_IGN);   /*   Predictive   De-configuration   of   Processors   -   */   
    signal(SIGKAP,SIG_IGN);             /*   keep   alive   poll   from   native   keyboard   */   
    signal(SIGRETRACT,SIG_IGN);     /*   monitor   mode   should   be   relinguished   */   
    signal(SIGSOUND     ,SIG_IGN);     /*   sound   control   has   completed   */   
    signal(SIGSAK         ,SIG_IGN);     /*   secure   attention   key   */   
#endif   
    }
#endif
}
//////////////////////////////////////////////////////////////////////////
void DestroySocket()
{
#ifdef WIN32
    WSACleanup();
     for ( int i=0; i<MAXCLIENT; i++)
     {
         DeleteCriticalSection(&DD_ClientMgr_Mutex[i]);
     }
#else
    for ( int i=0; i<MAXCLIENT; i++)
    {
    pthread_mutex_destroy(&DD_ClientMgr_Mutex[i]);
    }
#endif
}





