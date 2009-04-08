#include "GameProtocol.h"
#include "mythread.h"

long GameProtocal::m_recvPackNum = 0;
#ifdef WIN32
   extern CRITICAL_SECTION DD_ClientMgr_Mutex[MAXCLIENT];  
#else
    extern pthread_mutex_t DD_ClientMgr_Mutex[MAXCLIENT]; 
#endif

GameProtocal::GameProtocal()
{

}

GameProtocal::~GameProtocal()
{

}
/************************************************************************/
/* �����հ���
/************************************************************************/ 
int GameProtocal::RecvPacketBody(MySocket* pSockClient,PacketHead &packHead,  Packet &pack)
{
    int ret;
    int realSize = packHead.length+HEADSIZE;
    memset(&pack, 0, sizeof(Packet));
    memcpy(&pack, &packHead, HEADSIZE);

    if ( -1 == ( ret = recv(pSockClient->m_socket, (char*)(&pack)+HEADSIZE,
        packHead.length, MSG_NOSIGNAL) ) ) 
    {   // ʧ�ܣ�ɾ���ÿͻ�
        pManager->DelClient( pSockClient );
        cout << "Wrong RecvPacketBody" << endl;
        return -1;
    }
            //char sChatHead[300];
            //char sIP[16];
            //char sPort[8]; 
            //pSockClient->GetIP(sIP);
            //pSockClient->GetPort(sPort);
            //sprintf(sChatHead, "%s:%s ˵:%s ", sIP, sPort, pack.data);
            //strcpy( pack.data, sChatHead);
            //cout << sChatHead << endl;

    return realSize;
}
/************************************************************************/
/* ����Э���
/************************************************************************/ 
bool GameProtocal::SendToOthers(MySocket* pSockClient, PacketHead &packHead)
{
    MySocket *pSock;
    Packet pack;
    int realSize;
    if (-1 == (realSize = RecvPacketBody(pSockClient, packHead, pack) ) )
        return false;
    
    /**/int nClient = pManager->GetNum();
    for ( int i = 0; i < nClient; i++)
    {
        if ( pSockClient == (pSock = pManager->GetClient(i) ) )
        {
            continue;
        }
        int nID = pSock->m_ID;
        LockThreadBegin
        if ( -1 == send(pSock->m_socket, (char*)(&pack), realSize, MSG_NOSIGNAL))
        {
            cout << errno << ":�����." << endl;
            pManager->DelClient( pSock );
        }
        LockThreadEnd
    } 
    
    return true;
}
/************************************************************************/
/* ����all
/************************************************************************/ 
bool GameProtocal::SendToAll(MySocket* pSockClient, PacketHead &packHead)
{
    MySocket *pSock;
    Packet pack;
    int realSize;
    if (-1 == (realSize = RecvPacketBody(pSockClient, packHead, pack) ) )
        return false;

    //LockThreadBegin
    for ( int i = 0; i < pManager->GetNum(); i++)
    {
        pSock = pManager->GetClient(i);
        if ( -1 == send(pSock->m_socket, (char*)(&pack), realSize, MSG_NOSIGNAL))
        {
            //LockThreadEnd
            cout << errno << ":�����." << endl;
            pManager->DelClient( pSock );
        }
        //LockThreadEnd
    } 
    return true;
}

/************************************************************************/
/* ����ĳ��
/************************************************************************/ 
bool GameProtocal::SendToOne(MySocket* pSockClient, PacketHead &packHead)
{
    Packet pack;
    int realSize;
    if (-1 == (realSize = RecvPacketBody(pSockClient, packHead, pack) ) )
        return false;

    //LockThreadBegin
        if ( -1 == send(pSockClient->m_socket, (char*)(&pack), realSize, MSG_NOSIGNAL))
        {
            //LockThreadEnd
                cout << errno << ":�����." << endl;
            pManager->DelClient( pSockClient );
        }
    //LockThreadEnd
    return true;
}
/************************************************************************/
/* ֱ�ӷ��Ͱ�ͷ
/************************************************************************/ 
bool GameProtocal::SendHead(MySocket* pSockClient, PacketHead &packHead)
{
   // LockThreadBegin
    if ( -1 == send(pSockClient->m_socket, (char*)(&packHead), HEADSIZE, MSG_NOSIGNAL))
    {
       // LockThreadEnd
        cout << errno << ":SendHead." << endl;
        pManager->DelClient( pSockClient );
    }
   // LockThreadEnd
    return true;
}
/************************************************************************/
/*  �û���¼ 
/************************************************************************/      
bool GameProtocal::UserLogin(MySocket *pSockClient, PacketHead &packHead)
{
    /**/
    char buffPass[21];
    UserLoginPack sLogin;
    int realSize = 0;
    if (-1 == (realSize = recv(pSockClient->m_socket, (char*)(&sLogin), sizeof(UserLoginPack), MSG_NOSIGNAL))) 
        return false;
    packHead.length = 0;

    m_SQL.GetUserPassword(sLogin.Name, buffPass);
    if ( 0 == strcmp(buffPass, sLogin.Password) )
    {
        // Login success
        packHead.iStyle = LOGIN_SUCCESS;
    }
    else
    {
        // Wrong user name or password.
        packHead.iStyle = LOGIN_FAIL;
        SendHead(pSockClient, packHead);
        pManager->DelClient(pSockClient);
        return false;

    }

    return true;
}
/************************************************************************/
/*  �û�ע�� 
/************************************************************************/      
bool GameProtocal::UserRegist(MySocket *pSockClient, PacketHead &packHead)
{
    return true;
}
