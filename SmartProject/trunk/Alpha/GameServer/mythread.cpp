#include "head.h"
#include "mythread.h"
#include "ClientManager.h"


extern pthread_mutex_t DD_ClientMgr_Mutex[MAXCLIENT];
extern bool g_isRuning;
extern int  g_MaxRoommate;
void LogOut(char *buff, int length);

/************************************************************************/
/* 服务器发送线程<传送控制台输入字符，可有可无的线程>         
/************************************************************************/
THREADHEAD ThreadSend(void* para)
{
    cout << "输入-?查看控制台命令." << endl;
    PThreadParam pp = (PThreadParam)para;
    ClientManager *pManager = pp->mgr;
    while (1)
    {    
        char buffOut[300] = "GameServer说:";
        char buff[255];
        memset( buff, 0, 255);

        /* 服务器io输入 */
        cin.getline(buff, sizeof(buff));
        if ( true == DoServercommand( pManager, buff) )
            continue;

        strcat( buffOut, buff);

        // 给所有客户广播消息包
        /*for ( int i = 0; i < pManager->GetNum(); i++)
        {
            if ( false == pManager->GetClient(i)->SendPacket(buffOut, strlen(buffOut) ) )
            {
                cout << errno << " : ThreadSend." << endl;
                pManager->DelClient( pManager->GetClient(i) );
            }
        }*/
    }
}
/************************************************************************/
/* Client接收线程         
/************************************************************************/
THREADHEAD ThreadClient(void* pparam)
{
#ifndef WIN32 
    pthread_detach(pthread_self());
#endif
    
    long recvPackets = 0;
    int ret;
    char buff[255];
    PThreadParam pp;
    pp = (PThreadParam)pparam; 
    ClientManager *pManager = pp->mgr;
    
    MySocket * pSockClient = pManager->AddClient(pp->sock);
    if ( NULL == pSockClient)
    {
        return 0;
    }
    sprintf(buff, "欢迎加入SmartTand聊天室!总用户:%d个,IP是%s : %d\n", 
        pManager->GetNum(),
        inet_ntoa(pSockClient->m_addr.sin_addr), pSockClient->m_addr.sin_port);
    //pSockClient->SendPacket(buff, long(strlen(buff)));
    cout << buff << endl;  

    /* Main Loop */
    while (1)
    {
        PacketHead packHead;
        Packet pack;
        memset(&packHead, 0, sizeof(PacketHead));
        memset(&pack, 0, sizeof(Packet));
        /* 收包头 */
        if ( 0 >= ( ret = recv(pSockClient->m_socket, (char*)&packHead, HEADSIZE, MSG_NOSIGNAL) ) )
        {   
            // 失败，删除该客户
            cout << "Recv Error." << endl;
            pManager->DelClient(pSockClient); 
            return 0;
        }
        // 确保包体长度有效性
        if ( packHead.length > PACKSIZE || packHead.length < 0)
        {
            char buff[255];
            sprintf( buff, "Error Length of Head : %d\n",GetMyError);
            LogOut(buff,strlen(buff));
            cout << "Error Length of Head : " << packHead.length << endl;
            continue;
        }
        // 继续收包体  
        if ( false == ( pSockClient->RecvPacket(packHead, pack) ) )
        {   // 失败，删除该客户
            char buff[255];
            sprintf( buff, "%d\n",GetMyError);
            LogOut(buff,strlen(buff));
            pManager->DelClient( pSockClient );
            cout << "Wrong recv pack body" << endl;
        }
        pack.iStyle = packHead.iStyle;

        /* 由GameProtocal定义的协议分析器来分析数据包 */
        switch(packHead.iStyle)
        { 
        
        /*  心跳包,暂时不用  */
        case HEART_BEAT:  
            //pManager->HeartBeatFun(pSockClient);
            break;

        /*  正常游戏包  */
        case USER_DATA:  
            if ( ++recvPackets%100 == 0)
                cout << "包头:" << packHead.iStyle << ',' << packHead.length << endl;
            pManager->GetProtocal()->SendToOthers(pSockClient, pack);
            break;

         /*  创建房间    */
        case ROOM_CREATE:  
            pManager->GetProtocal()->CreateRoom(pSockClient, pack);
            break;

        /*  进入房间    */
        case ROOM_JOIN:  
            pManager->GetProtocal()->EnterRoom(pSockClient, pack);
            break;

        /*  房间列表    */
        case ROOM_LIST:  
            pManager->GetProtocal()->ListRoom(pSockClient, pack);
            break;

        /*  房间玩家列表    */
        case ROOM_LISTUSER:  
            pManager->GetProtocal()->ListUser(pSockClient, pack);
            break;

        /*  房间有人离开  */
        case ROOM_EXIT:  
            pManager->GetProtocal()->ExitRoom(pSockClient, pack);       
            break;

        /*  排行榜列表    */
        case USER_RANK:  
            pManager->GetProtocal()->ListRank(pSockClient, pack);
            break;

        /*  用户注册    */
        case USER_REGIST:     
            break;

        /*  用户登录    */
        case LOGIN:    
            pManager->GetProtocal()->UserLogin(pSockClient, pack);
            break;

        /*  用户自己信息  */
        case USER_INFO:  
            pManager->GetProtocal()->MyInfo(pSockClient, pack);
            break;

        /*  游戏开始!  */
        case GAME_GO:  
            pManager->GetProtocal()->GameGo(pSockClient, pack);
            break;
        
        /*  保存游戏结果到数据库  */
        case GAME_RESULT:  
            pManager->GetProtocal()->SaveGameResult(pSockClient, pack);
            break;

        /*  用户离开  */
        case USER_EXIT:  
            pManager->GetProtocal()->UserExit(pSockClient);       
            return 0;// 退出线程

        /*  Unknow  */
        default:
            {
                cout << pSockClient->GetIP() <<" : Unknow Head: ";
                cout << ret << "byte: " << packHead.iStyle << endl;
                char buff[9];            
                sprintf(buff, (char*)&packHead);
                buff[8] = '\0';
                cout << buff << endl;
                break; 
            }
        }
    }// while

    pSockClient->Close();
    return 0;
}


bool  DoServercommand(ClientManager *pManager, char* msg)
{
    if ( 0 == strcmp(msg, "-q") )   /* exit */
    {
        DestroySocket();
        exit(0);
    }
    else   /* 房间信息 */
        if ( 0 == strcmp(msg, "-r") )
        {
            if (pManager->m_numOfRoom > 0)
            {
                int realPlayers;
                UserInfo  riOut[MAXROOMMATE]; 
                realPlayers = pManager->m_room->GetPlayers();
                pManager->m_room->GetPlayesInfo(riOut);
                cout << setw(-20)  << "玩家名" << setw(10) << "总分"<< endl;
                for ( int i = 0;   i < realPlayers; i++)
                {
                    cout << setw(-20) << riOut[i].Name ;
                    if ( riOut[i].ID == 1){
                        cout << "(房主)" ;
                    }
                    cout << setw(10) << riOut[i].Score <<  endl;
                }
            }
            else
            {
                cout <<"当前可用房间:" << pManager->m_numOfRoom  << endl;
            }
        }
        else   /* Look current clents */
            if ( 0 == strcmp(msg, "-t") )
            {
                cout << "当前连接总数:" << pManager->GetNum() << endl;
                for ( int i = 0; i < pManager->GetNum(); i++)
                {
                    cout << pManager->GetClient(i)->m_name << " : "
                        << pManager->GetClient(i)->GetIP()  << " : " 
                        << pManager->GetClient(i)->GetPort() << endl;
                }
            }else   /* Look for MySql */
                if ( 0 == strcmp(msg, "-all") )
                {
                    setiosflags(ios::left);
                    UserInfo ui[MAXCLIENT]; 
                    int nUsers = pManager->GetProtocal()->m_SQL.GetUserCount();
                    cout << setw(-10) << "ID" << setw(20) << "Name" << setw(10) << "Password" << 
                        setw(10) << "Rank" << setw(10) << "Score" << endl;
                    for ( int i = 0; i < pManager->m_protocol->m_SQL.GetUserList(0, nUsers, ui); i++)
                    {
                        cout << setw(-10) << ui[i].ID << setw(20) << ui[i].Name << setw(10) << ui[i].Password 
                            << setw(10)  << ui[i].Rank << setw(10) << ui[i].Score << endl;
                    }
                }
                if ( 0 == strcmp(msg, "-rank") )
                {
                    setiosflags(ios::left);
                    RankInfo ui[MAXCLIENT]; 
                    cout << setw(10) << "Rank"   
                         << setw(20) << "Name"<< setw(10) << "Score" << endl;
                    for ( int i = 0; i < pManager->GetProtocal()->m_SQL.GetRankList(0, MAXCLIENT, ui); i++)
                    {
                        cout <<  setw(10)  << ui[i].Rank 
                            << setw(20) << ui[i].Name << setw(10) << ui[i].Score << endl;
                    }
                }
                else   /* Look for MySql */
                    if ( 0 == strcmp(msg, "-kill") )
                    {
                        cout << "所有玩家都可以死了." << endl;
                        pManager->DelClientAll();
                    }
                    else   /* Look for MySql */
                        if ( 0 == strcmp(msg, "-add") )
                        {
                            char buffname[21], buffPass[21];
                            cout << "用户名:";          cin >> buffname;
                            cout << endl << "密码:";    cin >> buffPass;
                            if ( 0 == pManager->GetProtocal()->m_SQL.AddUserItem(buffname, buffPass,0) )
                                cout << "增加成功." << endl;
                        }
                        else   /* Look for MySql */
                            if ( 0 == strcmp(msg, "-del") )
                            {
                                char buffname[21];
                                cout << "用户名:";          cin >> buffname;
                                if ( 0 == pManager->GetProtocal()->m_SQL.DeleteUserItem(buffname) )
                                    cout << "删除成功.";
                            }
                            else   /* Look for MySql */
                                if ( 0 == strcmp(msg, "-?") )
                                {
                                    cout << "-q: 强制关闭." << endl;
                                    cout << "-t: 查看当前连接信息." << endl;
                                    cout << "-r: 查看当前房间信息." << endl;
                                    cout << "-all: 查看数据库." << endl;
                                    cout << "-add: 增加帐号." << endl;
                                    cout << "-del: 删除帐号." << endl;
                                    cout << "-rank: 查看积分榜." << endl;
                                    cout << "-kill: 杀灭所有玩家." << endl;
                                }
                                else    /* Bad command */
                                {
                                    return false;
                                }
                                return true;
}
void LogOut(char *buff, int length)
{
    ofstream  outLog;
    outLog.open("ThreadLog.txt", ios_base::app);
    outLog.write(buff, length);
    outLog.write("\n", 2);
    outLog.close();
}









