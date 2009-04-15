#include "head.h"
#include "mysocket.h"
#include "mythread.h"
#include "ClientManager.h"
#include "GameProtocol.h"
#include "MyDataBase.h"
using namespace std;

pthread_mutex_t DD_ClientMgr_Mutex[MAXCLIENT];
MySocket SockConn;
MySocket sockSrv;
int  g_MyPort;
int  g_MaxRoommate;
char g_mySqlUserlName[21];
char g_mySqlUserlPass[21];
char g_mySqlDatabase[128];
bool g_isConnect[MAXCLIENT]; // 心跳检测
ClientManager clientMgr;
void HeartBeatFun(int id);
int main()
{
	/* 初始化协议 */ 
    StartSocket();
    GameProtocal gameProtocal;
	clientMgr.AttachProtocal(&gameProtocal);
    gameProtocal.AttachManager(&clientMgr);
    /* 开启监听 */ 
    sockSrv.Bind(MYPORT);
    sockSrv.Listen(); 
    clientMgr.AddServer(&sockSrv);
    cout << "Now Listenning..." << endl;

    // 开启服务器发包线程，传送数据到
    ThreadParam para;
    PThreadParam pp = &para;
    pp->mgr = &clientMgr;

#ifdef WIN32    
    DWORD  threadID;
    CreateThread(0, 0, LPTHREAD_START_ROUTINE(ThreadSend), pp,NULL, &threadID);
#else 
    pthread_t threadID;
    pthread_create(&threadID, NULL, ThreadSend, (void*)pp);
    //signal(SIGALRM   ,HeartBeatFun );       /*   alarm   clock   timeout   */  
#endif
	
	// 主循环
	while ( true )
    {  
        int len = sizeof(sockaddr);
        int sockConn; 
        /*  测试accept返回的socket  */
        if ( -1 == (sockConn = sockSrv.Accept(&SockConn)) ){
            exit(1);
        }

        /* 连接成功，创建新的客户socket以及线程,传送数据到thread */
    	ThreadParam para;
		PThreadParam pp = &para; 
		para.sock = &SockConn;
        para.mgr  = &clientMgr;

        /* 开启客户线程 */
#ifdef WIN32
		CloseHandle(CreateThread(0, 0, LPTHREAD_START_ROUTINE(ThreadClient),
            pp,NULL, &threadID) );
#else
        pthread_t threadID;
        pthread_create(&threadID, NULL, ThreadClient, (void*)pp);
#endif
	}

    /* 清场 */
	sockSrv.Close();
	DestroySocket();
    return 0;
}


void HeartBeatFun(int id)
{
    for (int i = 0; i < clientMgr.GetNum(); i++)
    {
        MySocket *pSock = clientMgr.GetClient(i);
        if (pSock->IsLiving())
        {  
            pSock->m_isLiving = false;
        }
        else
        {
            clientMgr.GetProtocal()->UserExit(pSock);
        }
    }
    cout << "HeartBeat!" << endl;
}




