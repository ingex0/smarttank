#include "GameProtocol.h"
#include "mythread.h"

long GameProtocal::m_recvPackNum = 0;
extern pthread_mutex_t DD_ClientMgr_Mutex[MAXCLIENT]; 
extern int  g_MaxRoommate;

GameProtocal::GameProtocal()
{
    outLog.open("log.txt");
}

GameProtocal::~GameProtocal()
{
    outLog.close();
}


/************************************************************************/
/* ����������
/************************************************************************/ 
bool GameProtocal::SendToOthers(MySocket* pSockClient, Packet &pack)
{
    MySocket *pSock;
    int realSize = pack.length + HEADSIZE;

    //outLog.write((char*)&pack, realSize);
    //char buffLog[PACKSIZE];
    /**/int nClient = pManager->GetNum();
    for ( int i = 0; i < nClient; i++)
    {
        if ( pSockClient == (pSock = pManager->GetClient(i) ) )
        {
            continue;
        }
        if ( false == pSock->SendPacket(pack, realSize) )
        {
            pManager->DelClient(pSockClient);
        }
    } 
    
    return true;
}
/************************************************************************/
/* ����all
/************************************************************************/ 
bool GameProtocal::SendToAll(MySocket* pSockClient, Packet &pack)
{
    MySocket *pSock;
    int realSize = pack.length + HEADSIZE;

    for ( int i = 0; i < pManager->GetNum(); i++)
    {
        pSock = pManager->GetClient(i);
        if ( false == pSock->SendPacket(pack, realSize) )
        {
            pManager->DelClient( pSock );
        }
    } 
    return true;
}

/************************************************************************/
/* ����ĳ��
/************************************************************************/ 
bool GameProtocal::SendToOne(MySocket* pSockClient, Packet &pack)
{
    int realSize = pack.length + HEADSIZE;

    if ( false == pSockClient->SendPacket(pack, realSize) )
    {
        pManager->DelClient( pSockClient );
    }
    return true;
}
/************************************************************************/
/* ֱ�ӷ��Ͱ�ͷ
/************************************************************************/ 
bool GameProtocal::SendHead(MySocket* pSockClient, PacketHead &packHead)
{
    if (false == pSockClient->SendPacketHead(packHead) )
    {
        pManager->DelClient(pSockClient);
    }
    return true;
}
/************************************************************************/
/*  �û���¼ 
/************************************************************************/      
bool GameProtocal::UserLogin(MySocket *pSockClient, Packet &pack)
{
    char buffPass[21];
    PacketHead packHead;
    packHead.length = 0;

    UserLoginPack sLogin;
    memcpy(&sLogin, (char*)&pack+HEADSIZE, sizeof(UserLoginPack));
    
    cout << pSockClient->GetIP() << "���Ե�¼:" << sLogin.Name << ":" 
        << sLogin.Password;
    /* ��ѯ�Ƿ��ظ���¼ */
    for ( int i = 0; i < pManager->GetNum(); i++)
    {
        MySocket *p = pManager->GetClient(i);
        if ( p == pSockClient )
            continue;
        if ( 0 == strcmp(pManager->GetClient(i)->m_name, sLogin.Name) )
        {
            cout << " --- �ܾ��ص�¼." << endl;
            packHead.iStyle = LOGIN_FAIL;
            SendHead(pSockClient, packHead);
            return false;
        }
    }

    /* ��ѯ���ݿ� */
    if ( -1 == m_SQL.GetUserPassword(sLogin.Name, buffPass) 
       || 0 != strcmp(buffPass, sLogin.Password))
    {
        // Wrong user name or password.
        cout << " --- ʧ��." << endl;
        packHead.iStyle = LOGIN_FAIL;
        SendHead(pSockClient, packHead);
    }
    else
    { 
        // Login success
        cout << " --- OK." << endl;
        pSockClient->BindName(sLogin.Name);
        pSockClient->m_rank = m_SQL.GetUserRank(sLogin.Name);
        pSockClient->m_score = m_SQL.GetUserScore(sLogin.Name);
        packHead.iStyle = LOGIN_SUCCESS;
        SendHead(pSockClient, packHead);
        return false;
    }

    return true;
}
/************************************************************************/
/*  �û��뿪<...>
/************************************************************************/
bool GameProtocal::UserExit(MySocket *pSockClient)
{
    PacketHead head;
    head.iStyle = USER_EXIT;
    head.length = 0;
    SendHead(pSockClient, head);
    pManager->DelClient(pSockClient);     
    return true;
}
/************************************************************************/
/*  �û�ע�� 
/************************************************************************/      
bool GameProtocal::UserRegist(MySocket *pSockClient, Packet &pack)
{
    return true;
}
/************************************************************************/
/*  �û����а�
/************************************************************************/ 
bool GameProtocal::ListRank(MySocket *pSockClient, Packet &pack)
{
    int realSize, realPlayers, rankBegin;
    RankInfo  riOut[NUM_OF_RANK]; 

    memcpy(&rankBegin, (char*)&pack.length+HEADSIZE, sizeof(int));
    realPlayers = m_SQL.GetRankList(rankBegin, rankBegin+NUM_OF_RANK, riOut);
    
    pack.length = realPlayers*sizeof(RankInfo);
    realSize = pack.length + HEADSIZE;

    //cout << setw(20) << "Name" << setw(10) << "Rank" << setw(10) << "Score" << endl;
    for ( int i = 0;   i < realPlayers;   i++)
    {
        /* �������а� */
        memcpy( (char*)&pack + HEADSIZE + i*sizeof(RankInfo) ,
                &riOut[i], sizeof(RankInfo));
        //cout << setw(20) << riOut[i].Name << setw(10)  
        //    << riOut[i].Rank << setw(10) << riOut[i].Score << endl;
    }
    if ( false == pSockClient->SendPacket(pack, realSize) )
        pManager->DelClient(pSockClient);

    cout << "ˢ�����а�ɹ�." << endl;
    return true;
}
/************************************************************************/
/*  �û��б�
/************************************************************************/ 
bool GameProtocal::ListUser(MySocket *pSockClient, Packet &pack)
{   
    int realSize, realPlayers;
    UserInfo  riOut[MAXROOMMATE]; 
    realPlayers = pManager->m_room->GetPlayers();

    pack.iStyle = ROOM_LISTUSER; 
    pack.length = realPlayers*sizeof(UserInfo);


    realSize = pack.length + HEADSIZE;
    pManager->m_room->GetPlayesInfo(riOut);

    //cout << setw(-20)  << "�����" << setw(10) << "�ܷ�"<< endl;

    for ( int i = 0;   i < realPlayers; i++)
    {
        /* ���Ʒ�����Ա��Ϣ */
        memcpy( (char*)&pack + HEADSIZE + i*sizeof(UserInfo) ,
            &riOut[i], sizeof(UserInfo));
        //cout << setw(-20) << riOut[i].Name ;
        /*if ( riOut[i].ID == 1){
            cout << "(����)" ;
        }
        cout << setw(10) << riOut[i].Score <<  endl;*/
    }
    if ( false == pSockClient->SendPacket(pack, realSize) )
        pManager->DelClient(pSockClient);

    return true;
}

/************************************************************************/
/* list���� 
/************************************************************************/
bool GameProtocal::ListRoom(MySocket *pSockClient, Packet &pack)
{
    int numOfRoom = pManager->m_numOfRoom;
    if ( 0 == numOfRoom )
    {
        pack.length = 0;
        pSockClient->SendPacket(pack, HEADSIZE);
        //cout << "ˢ�·���,������Ŀ:" << numOfRoom << endl;
    }
    else
    {
        RoomInfo ri;
        pack.length = sizeof(RoomInfo);
        int realSize = HEADSIZE + sizeof(RoomInfo);
        pManager->m_room->GetRoomInfo(ri);
        memcpy((char*)&pack+HEADSIZE, &ri, sizeof(RoomInfo));
        pSockClient->SendPacket(pack, realSize);
        //cout << "ˢ�·���,������Ŀ:" << numOfRoom << endl;
    }
    return true;
}


/************************************************************************/
/*  ��������    */
/************************************************************************/
bool GameProtocal::CreateRoom(MySocket *pSockClient, Packet &pack)
{
    PacketHead head;
    int numOfRoom = pManager->m_numOfRoom;
    head.length = 0;
    if ( 0 == numOfRoom )
    {            
        cout << "��" << pSockClient->m_name << "�������˷���." << endl;
        pManager->m_room->CreateRoom(pSockClient);
        head.iStyle = CREATE_SUCCESS;
        pManager->m_numOfRoom = 1;
    }
    else
    { 
        head.iStyle = CREATE_FAIL;
        cout << "��" << pSockClient->m_name << "����������ʧ��." << endl;
    }
    SendHead(pSockClient, head);
    return true;
}
/************************************************************************/
/*  ���뷿��    */
/************************************************************************/
bool GameProtocal::EnterRoom(MySocket *pSockClient, Packet &pack)
{
    PacketHead head;
    RoomInfo ri;
    pManager->m_room->GetRoomInfo(ri);

    head.length = 0;
    if ( g_MaxRoommate == ri.players || 0==pManager->m_numOfRoom || ri.bBegin)
    {
        head.iStyle = JOIN_FAIL;
        cout << "��" << pSockClient->m_name << "�����뷿��ʧ��." << endl;
        SendHead(pSockClient, head);
    }
    else
    { 
        cout << "��" << pSockClient->m_name << "�����뷿��ɹ�." << endl;

        pManager->m_room->EnterRoom(pSockClient);
        head.iStyle = JOIN_SUCCESS;
        SendHead(pSockClient, head);

        // ˢ�·�����Ϣ
        Packet pack;
        for ( int i = 0; i < pManager->m_room->GetPlayers(); i++)
        {
            ListUser(pManager->m_room->GetPlayer(i), pack);
        }
    }
    return true;
}
/************************************************************************/
/*  �뿪����    */
/************************************************************************/
bool GameProtocal::ExitRoom(MySocket *pSockClient, Packet &pack)
{
    pack.iStyle = ROOM_LISTUSER;
    if ( true == pManager->m_room->RemovePlayer(pSockClient) )
    {
        if ( pManager->m_room->GetPlayers() == 0)
        {
            pManager->m_numOfRoom--;
        }
        // ˢ�·�����Ϣ
        for ( int i = 0; i < pManager->m_room->GetPlayers(); i++)
        {
            ListUser(pManager->m_room->GetPlayer(i), pack);
        }
    }
    else
    {
        cout << "��ţB,û������Ҳ���˷�." << endl;
    }

    return true;
}
/************************************************************************/
/*  �û��Լ���Ϣ    */
/************************************************************************/
bool GameProtocal::MyInfo(MySocket *pSockClient, Packet &pack)
{
    int realSize;
    UserInfo  riOut; 
    pack.length = sizeof(UserInfo);
    realSize = pack.length + HEADSIZE;
    pSockClient->GetUserInfo(riOut);
    
    cout << "������Ϣ" << endl;
    if ( riOut.ID == 1){
        cout << "(����)" ;
    }
    cout << riOut.Name << " : �ܷ�"<<riOut.Rank << 
        ",������ " << riOut.Rank << " ��." << endl;
    /* ������Ϣ */
    memcpy( (char*)&pack + HEADSIZE,  &riOut, sizeof(UserInfo));
    

    if ( false == pSockClient->SendPacket(pack, realSize) )
        pManager->DelClient(pSockClient);
    return true;
}
/************************************************************************/
/*  ��Ϸ��ʼ  */
/************************************************************************/
bool GameProtocal::GameGo(MySocket *pSockClient, Packet &pack)
{
    pack.length = 0;
    if ( pManager->m_room->GetPlayers() > 1)
    {
        pManager->m_room->SetRoomState(true);
        cout << "��Ϸ��ʼ." << endl;
        pack.iStyle = GAME_GO;
    }
    else
    {
        cout << "1�������ë." << endl;
        pack.iStyle = GAME_START_FAIL;
    }
    // ���͸��������������
    pManager->m_room->SendToAll(pack);
    return true;
}
/************************************************************************/
/*  ������Ϸ��������ݿ�  */
/************************************************************************/
bool GameProtocal::SaveGameResult(MySocket *pSockClient, Packet &pack)
{
    int realPlayers = pack.length/sizeof(RankInfo);
    RankInfo ri[MAXROOMMATE];
    memcpy(ri, (char*)&pack + HEADSIZE, pack.length);
    for (int i = 0; i < realPlayers; i++)
    {
        // ���ӵ÷�
        int newScore = m_SQL.GetUserScore(ri[i].Name) + ri[i].Score;
        cout << ri[i].Name << " �÷�: " << ri[i].Score << endl;
        if ( -1 ==  m_SQL.SetUserScore(ri[i].Name, newScore) )
            cout << "�޸� " << ri[i].Name << " ʧ��." << endl;
    }
    pManager->m_room->SetRoomState(false);
    return true;
}

/************************************************************************/
/*  ������Ϸ��������ݿ�  */
/************************************************************************/
bool GameProtocal::SendGameData(MySocket *pSockClient, Packet &pack)
{
    pManager->m_room->SendToOthers(pSockClient, pack);
    return true;
}








