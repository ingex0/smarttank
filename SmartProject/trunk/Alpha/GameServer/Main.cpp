#include "head.h"
#include "mysocket.h"
#include "mythread.h"
#include "ClientManager.h"
#include "GameProtocol.h"
#include "MyDataBase.h"
using namespace std;

pthread_mutex_t DD_ClientMgr_Mutex[MAXCLIENT];

int main()
{
   
	/* ��ʼ��Э�� */ 
    StartSocket();
    ClientManager clientMgr;
    GameProtocal gameProtocal;
	clientMgr.AttachProtocal(&gameProtocal);
    gameProtocal.AttachManager(&clientMgr);
    /* �������� */ 
    MySocket sockSrv;
    sockSrv.Bind(MYPORT);
    sockSrv.Listen(); 
    clientMgr.AddServer(&sockSrv);
    cout << "Now Listenning..." << endl;

    // ���������������̣߳��������ݵ�
    ThreadParam para;
    PThreadParam pp = &para;
    pp->mgr = &clientMgr;

    MySocket *pSockConn = &sockSrv;
#ifdef WIN32    
    DWORD  threadID;
    CreateThread(0, 0, LPTHREAD_START_ROUTINE(ThreadSend), pp,NULL, &threadID);
#else 

    pthread_t threadID;
    pthread_create(&threadID, NULL, ThreadSend, (void*)pp);
#endif
	
	// ��ѭ��
	while (1)
    {  
        int len = sizeof(sockaddr);
        char buff[255];

        int sockConn; 
        MySocket *pSockConn = new MySocket();
        /*  ����accept���ص�socket  */
        if ( -1 == (sockConn = sockSrv.Accept(pSockConn)) ){
            exit(1);
        }

        /* ���ӳɹ��������µĿͻ�socket�Լ��߳�,�������ݵ�thread */
    	ThreadParam para;
		PThreadParam pp = &para; 
		para.sock = pSockConn;
        para.mgr  = &clientMgr;

        pSockConn->m_ID = clientMgr.GetNum();
        clientMgr.AddClient(pSockConn);
        sprintf(buff, "��ӭ����SmartTand������!���û�:%d��,���IP��%s\n",clientMgr.GetNum(),inet_ntoa(pSockConn->m_addr.sin_addr));
        //pSockConn->SendPacket(buff, long(strlen(buff)));

        cout << buff << endl;   
        linger ling;
        ling.l_onoff=0;
        ling.l_linger=1;
        ///*******************Xiangbin Modified this**************************/
        //if(0 != setsockopt(pSockConn->m_socket, SOL_SOCKET, SO_LINGER, (char *)&ling, sizeof(ling)))
        //{
        //    return 1;
        //}

        /* �����ͻ��߳� */
#ifdef WIN32
		CreateThread(0, 0, LPTHREAD_START_ROUTINE(ThreadClient),pp,NULL, &threadID);
#else
        pthread_t threadID;
        pthread_create(&threadID, NULL, ThreadClient, (void*)pp);
#endif
	}//while

    /* �峡 */
	sockSrv.Close();
	DestroySocket();
    return 0;
}


