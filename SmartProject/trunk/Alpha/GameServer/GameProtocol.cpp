#include "GameProtocol.h"
#include "mythread.h"

long GameProtocal::m_recvPackNum = 0;
extern pthread_mutex_t DD_ClientMgr_Mutex[MAXCLIENT]; 


GameProtocal::GameProtocal()
{
    outLog.open("log.txt",ios::app);
}

GameProtocal::~GameProtocal()
{
    outLog.close();
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
    outLog.write((char*)&pack, realSize);
    char buffLog[PACKSIZE];
    /**/int nClient = pManager->GetNum();
    for ( int i = 0; i < nClient; i++)
    {
        if ( pSockClient == (pSock = pManager->GetClient(i) ) )
        {
            continue;
        }
        int nID = pSock->m_ID;
        if ( false == pSock->SendPacket(pack, realSize) )
        {
            pManager->DelClient( pSock );
        }
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
    if (false == pSockClient->SendPacketHead(packHead) )
    {
        pManager->DelClient( pSockClient );
    }
    return true;
}
/************************************************************************/
/*  �û���¼ 
/************************************************************************/      
bool GameProtocal::UserLogin(MySocket *pSockClient, PacketHead &packHead)
{
    char buffPass[21];
    UserLoginPack sLogin;
    int realSize = 0;
    if (-1 == (realSize = recv(pSockClient->m_socket, (char*)(&sLogin), sizeof(UserLoginPack), MSG_NOSIGNAL))) 
        return false;
    packHead.length = 0;
    cout << pSockClient->GetIP() << "���Ե�½:" << sLogin.Name << ":" << sLogin.Password << endl;
    /* ��ѯ���ݿ� */
    if ( -1 == m_SQL.GetUserPassword(sLogin.Name, buffPass) 
       || 0 != strcmp(buffPass, sLogin.Password))
    {
        // Wrong user name or password.
        packHead.iStyle = LOGIN_FAIL;
        SendHead(pSockClient, packHead);
        pManager->DelClient(pSockClient);
    }
    else
    { // Login success
        pSockClient->BindName(sLogin.Name);
        packHead.iStyle = LOGIN_SUCCESS;
        SendHead(pSockClient, packHead);
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
/************************************************************************/
/*  �û����а�
/************************************************************************/ 
bool GameProtocal::ListRank(MySocket *pSockClient, PacketHead &packHead)
{
    int realSize, realPlayers, rankBegin;
    Packet pack;
    memset(&pack, 0, SIZEOFPACKET);
    RankInfo  riOut[NUM_OF_RANK]; 

    rankBegin = packHead.length - 1;
    realPlayers = m_SQL.GetRankList(rankBegin, rankBegin+NUM_OF_RANK, riOut);
    packHead.length = sizeof(RankInfo) * NUM_OF_RANK; 
    
    pack.length = realPlayers*sizeof(RankInfo);
    realSize = pack.length + HEADSIZE;
    memcpy( &pack, &packHead, HEADSIZE);

    for ( int i = 0;   i < realPlayers;   i++)
    {
        /**/
        memcpy( (char*)&pack + HEADSIZE + i*sizeof(RankInfo) ,
                &riOut[i], sizeof(RankInfo));
        cout << setw(10) << riOut[i].Name << setw(10)  
            << riOut[i].Rank << setw(10) << riOut[i].Score << endl;
    }
    if ( false == pSockClient->SendPacket(pack, realSize) )
    {
        pManager->DelClient(pSockClient);
    }
    return true;
}
/************************************************************************/
/*  �û��б�
/************************************************************************/ 
bool GameProtocal::ListUser(MySocket *pSockClient, PacketHead &packHead)
{
    int realSize, realPlayers, rankBegin;
    Packet pack;
    memset(&pack, 0, SIZEOFPACKET);
    RankInfo  riOut[NUM_OF_RANK]; 

    rankBegin = packHead.length - 1;
    realPlayers = m_SQL.GetRankList(rankBegin, rankBegin+NUM_OF_RANK, riOut);
    packHead.length = sizeof(RankInfo) * NUM_OF_RANK; 

    pack.length = realPlayers*sizeof(RankInfo);
    realSize = pack.length + HEADSIZE;
    memcpy( &pack, &packHead, HEADSIZE);

    for ( int i = 0;   i < realPlayers;   i++)
    {
        /**/
        memcpy( (char*)&pack + HEADSIZE + i*sizeof(RankInfo) ,
            &riOut[i], sizeof(RankInfo));
        cout << setw(10) << riOut[i].Name << setw(10)  
            << riOut[i].Rank << setw(10) << riOut[i].Score << endl;
    }
    if ( false == pSockClient->SendPacket(pack, realSize) )
    {
        pManager->DelClient(pSockClient);
    }
    return true;
}







