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
/* 发给其他人
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
/* 发给all
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
/* 发给某人
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
/* 直接发送包头
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
/*  用户登录 
/************************************************************************/      
bool GameProtocal::UserLogin(MySocket *pSockClient, Packet &pack)
{
    char buffPass[21];
    PacketHead packHead;
    packHead.length = 0;

    UserLoginPack sLogin;
    memcpy(&sLogin, (char*)&pack+HEADSIZE, sizeof(UserLoginPack));
    
    cout << pSockClient->GetIP() << "尝试登录:" << sLogin.Name << ":" 
        << sLogin.Password;
    /* 查询是否重复登录 */
    for ( int i = 0; i < pManager->GetNum(); i++)
    {
        MySocket *p = pManager->GetClient(i);
        if ( p == pSockClient )
            continue;
        if ( 0 == strcmp(pManager->GetClient(i)->m_name, sLogin.Name) )
        {
            cout << " --- 拒绝重登录." << endl;
            packHead.iStyle = LOGIN_FAIL;
            SendHead(pSockClient, packHead);
            return false;
        }
    }

    /* 查询数据库 */
    if ( -1 == m_SQL.GetUserPassword(sLogin.Name, buffPass) 
       || 0 != strcmp(buffPass, sLogin.Password))
    {
        // Wrong user name or password.
        cout << " --- 失败." << endl;
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
/*  用户离开<...>
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
/*  用户注册 
/************************************************************************/      
bool GameProtocal::UserRegist(MySocket *pSockClient, Packet &pack)
{
    return true;
}
/************************************************************************/
/*  用户排行榜
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
        /* 复制排行榜 */
        memcpy( (char*)&pack + HEADSIZE + i*sizeof(RankInfo) ,
                &riOut[i], sizeof(RankInfo));
        //cout << setw(20) << riOut[i].Name << setw(10)  
        //    << riOut[i].Rank << setw(10) << riOut[i].Score << endl;
    }
    if ( false == pSockClient->SendPacket(pack, realSize) )
        pManager->DelClient(pSockClient);

    cout << "刷新排行榜成功." << endl;
    return true;
}
/************************************************************************/
/*  用户列表
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

    //cout << setw(-20)  << "玩家名" << setw(10) << "总分"<< endl;

    for ( int i = 0;   i < realPlayers; i++)
    {
        /* 复制房间人员信息 */
        memcpy( (char*)&pack + HEADSIZE + i*sizeof(UserInfo) ,
            &riOut[i], sizeof(UserInfo));
        //cout << setw(-20) << riOut[i].Name ;
        /*if ( riOut[i].ID == 1){
            cout << "(房主)" ;
        }
        cout << setw(10) << riOut[i].Score <<  endl;*/
    }
    if ( false == pSockClient->SendPacket(pack, realSize) )
        pManager->DelClient(pSockClient);

    return true;
}

/************************************************************************/
/* list房间 
/************************************************************************/
bool GameProtocal::ListRoom(MySocket *pSockClient, Packet &pack)
{
    int numOfRoom = pManager->m_numOfRoom;
    if ( 0 == numOfRoom )
    {
        pack.length = 0;
        pSockClient->SendPacket(pack, HEADSIZE);
        //cout << "刷新房间,房间数目:" << numOfRoom << endl;
    }
    else
    {
        RoomInfo ri;
        pack.length = sizeof(RoomInfo);
        int realSize = HEADSIZE + sizeof(RoomInfo);
        pManager->m_room->GetRoomInfo(ri);
        memcpy((char*)&pack+HEADSIZE, &ri, sizeof(RoomInfo));
        pSockClient->SendPacket(pack, realSize);
        //cout << "刷新房间,房间数目:" << numOfRoom << endl;
    }
    return true;
}


/************************************************************************/
/*  创建房间    */
/************************************************************************/
bool GameProtocal::CreateRoom(MySocket *pSockClient, Packet &pack)
{
    PacketHead head;
    int numOfRoom = pManager->m_numOfRoom;
    head.length = 0;
    if ( 0 == numOfRoom )
    {            
        cout << "【" << pSockClient->m_name << "】创建了房间." << endl;
        pManager->m_room->CreateRoom(pSockClient);
        head.iStyle = CREATE_SUCCESS;
        pManager->m_numOfRoom = 1;
    }
    else
    { 
        head.iStyle = CREATE_FAIL;
        cout << "【" << pSockClient->m_name << "】创建房间失败." << endl;
    }
    SendHead(pSockClient, head);
    return true;
}
/************************************************************************/
/*  进入房间    */
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
        cout << "【" << pSockClient->m_name << "】进入房间失败." << endl;
        SendHead(pSockClient, head);
    }
    else
    { 
        cout << "【" << pSockClient->m_name << "】进入房间成功." << endl;

        pManager->m_room->EnterRoom(pSockClient);
        head.iStyle = JOIN_SUCCESS;
        SendHead(pSockClient, head);

        // 刷新房间信息
        Packet pack;
        for ( int i = 0; i < pManager->m_room->GetPlayers(); i++)
        {
            ListUser(pManager->m_room->GetPlayer(i), pack);
        }
    }
    return true;
}
/************************************************************************/
/*  离开房间    */
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
        // 刷新房间信息
        for ( int i = 0; i < pManager->m_room->GetPlayers(); i++)
        {
            ListUser(pManager->m_room->GetPlayer(i), pack);
        }
    }
    else
    {
        cout << "你牛B,没进房你也能退房." << endl;
    }

    return true;
}
/************************************************************************/
/*  用户自己信息    */
/************************************************************************/
bool GameProtocal::MyInfo(MySocket *pSockClient, Packet &pack)
{
    int realSize;
    UserInfo  riOut; 
    pack.length = sizeof(UserInfo);
    realSize = pack.length + HEADSIZE;
    pSockClient->GetUserInfo(riOut);
    
    cout << "个人信息" << endl;
    if ( riOut.ID == 1){
        cout << "(房主)" ;
    }
    cout << riOut.Name << " : 总分"<<riOut.Rank << 
        ",排名第 " << riOut.Rank << " 名." << endl;
    /* 复制信息 */
    memcpy( (char*)&pack + HEADSIZE,  &riOut, sizeof(UserInfo));
    

    if ( false == pSockClient->SendPacket(pack, realSize) )
        pManager->DelClient(pSockClient);
    return true;
}
/************************************************************************/
/*  游戏开始  */
/************************************************************************/
bool GameProtocal::GameGo(MySocket *pSockClient, Packet &pack)
{
    pack.length = 0;
    if ( pManager->m_room->GetPlayers() > 1)
    {
        pManager->m_room->SetRoomState(true);
        cout << "游戏开始." << endl;
        pack.iStyle = GAME_GO;
    }
    else
    {
        cout << "1个人玩个毛." << endl;
        pack.iStyle = GAME_START_FAIL;
    }
    // 发送给房间里的所有人
    pManager->m_room->SendToAll(pack);
    return true;
}
/************************************************************************/
/*  保存游戏结果到数据库  */
/************************************************************************/
bool GameProtocal::SaveGameResult(MySocket *pSockClient, Packet &pack)
{
    int realPlayers = pack.length/sizeof(RankInfo);
    RankInfo ri[MAXROOMMATE];
    memcpy(ri, (char*)&pack + HEADSIZE, pack.length);
    for (int i = 0; i < realPlayers; i++)
    {
        // 增加得分
        int newScore = m_SQL.GetUserScore(ri[i].Name) + ri[i].Score;
        cout << ri[i].Name << " 得分: " << ri[i].Score << endl;
        if ( -1 ==  m_SQL.SetUserScore(ri[i].Name, newScore) )
            cout << "修改 " << ri[i].Name << " 失败." << endl;
    }
    pManager->m_room->SetRoomState(false);
    return true;
}

/************************************************************************/
/*  保存游戏结果到数据库  */
/************************************************************************/
bool GameProtocal::SendGameData(MySocket *pSockClient, Packet &pack)
{
    pManager->m_room->SendToOthers(pSockClient, pack);
    return true;
}








