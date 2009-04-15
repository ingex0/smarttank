#include "head.h"
#include "mythread.h"
#include "ClientManager.h"


extern pthread_mutex_t DD_ClientMgr_Mutex[MAXCLIENT];
extern bool g_isRuning;
extern int  g_MaxRoommate;
void LogOut(char *buff, int length);

/************************************************************************/
/* �����������߳�<���Ϳ���̨�����ַ������п��޵��߳�>         
/************************************************************************/
THREADHEAD ThreadSend(void* para)
{
    cout << "����-?�鿴����̨����." << endl;
    PThreadParam pp = (PThreadParam)para;
    ClientManager *pManager = pp->mgr;
    while (1)
    {    
        char buffOut[300] = "GameServer˵:";
        char buff[255];
        memset( buff, 0, 255);

        /* ������io���� */
        cin.getline(buff, sizeof(buff));
        if ( true == DoServercommand( pManager, buff) )
            continue;

        strcat( buffOut, buff);

        // �����пͻ��㲥��Ϣ��
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
/* Client�����߳�         
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
    sprintf(buff, "��ӭ����SmartTand������!���û�:%d��,IP��%s : %d\n", 
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
        /* �հ�ͷ */
        if ( 0 >= ( ret = recv(pSockClient->m_socket, (char*)&packHead, HEADSIZE, MSG_NOSIGNAL) ) )
        {   
            // ʧ�ܣ�ɾ���ÿͻ�
            cout << "Recv Error." << endl;
            pManager->DelClient(pSockClient); 
            return 0;
        }
        // ȷ�����峤����Ч��
        if ( packHead.length > PACKSIZE || packHead.length < 0)
        {
            char buff[255];
            sprintf( buff, "Error Length of Head : %d\n",GetMyError);
            LogOut(buff,strlen(buff));
            cout << "Error Length of Head : " << packHead.length << endl;
            continue;
        }
        // �����հ���  
        if ( false == ( pSockClient->RecvPacket(packHead, pack) ) )
        {   // ʧ�ܣ�ɾ���ÿͻ�
            char buff[255];
            sprintf( buff, "%d\n",GetMyError);
            LogOut(buff,strlen(buff));
            pManager->DelClient( pSockClient );
            cout << "Wrong recv pack body" << endl;
        }
        pack.iStyle = packHead.iStyle;

        /* ��GameProtocal�����Э����������������ݰ� */
        switch(packHead.iStyle)
        { 
        
        /*  ������,��ʱ����  */
        case HEART_BEAT:  
            //pManager->HeartBeatFun(pSockClient);
            break;

        /*  ������Ϸ��  */
        case USER_DATA:  
            if ( ++recvPackets%100 == 0)
                cout << "��ͷ:" << packHead.iStyle << ',' << packHead.length << endl;
            pManager->GetProtocal()->SendToOthers(pSockClient, pack);
            break;

         /*  ��������    */
        case ROOM_CREATE:  
            pManager->GetProtocal()->CreateRoom(pSockClient, pack);
            break;

        /*  ���뷿��    */
        case ROOM_JOIN:  
            pManager->GetProtocal()->EnterRoom(pSockClient, pack);
            break;

        /*  �����б�    */
        case ROOM_LIST:  
            pManager->GetProtocal()->ListRoom(pSockClient, pack);
            break;

        /*  ��������б�    */
        case ROOM_LISTUSER:  
            pManager->GetProtocal()->ListUser(pSockClient, pack);
            break;

        /*  ���������뿪  */
        case ROOM_EXIT:  
            pManager->GetProtocal()->ExitRoom(pSockClient, pack);       
            break;

        /*  ���а��б�    */
        case USER_RANK:  
            pManager->GetProtocal()->ListRank(pSockClient, pack);
            break;

        /*  �û�ע��    */
        case USER_REGIST:     
            break;

        /*  �û���¼    */
        case LOGIN:    
            pManager->GetProtocal()->UserLogin(pSockClient, pack);
            break;

        /*  �û��Լ���Ϣ  */
        case USER_INFO:  
            pManager->GetProtocal()->MyInfo(pSockClient, pack);
            break;

        /*  ��Ϸ��ʼ!  */
        case GAME_GO:  
            pManager->GetProtocal()->GameGo(pSockClient, pack);
            break;
        
        /*  ������Ϸ��������ݿ�  */
        case GAME_RESULT:  
            pManager->GetProtocal()->SaveGameResult(pSockClient, pack);
            break;

        /*  �û��뿪  */
        case USER_EXIT:  
            pManager->GetProtocal()->UserExit(pSockClient);       
            return 0;// �˳��߳�

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
    else   /* ������Ϣ */
        if ( 0 == strcmp(msg, "-r") )
        {
            if (pManager->m_numOfRoom > 0)
            {
                int realPlayers;
                UserInfo  riOut[MAXROOMMATE]; 
                realPlayers = pManager->m_room->GetPlayers();
                pManager->m_room->GetPlayesInfo(riOut);
                cout << setw(-20)  << "�����" << setw(10) << "�ܷ�"<< endl;
                for ( int i = 0;   i < realPlayers; i++)
                {
                    cout << setw(-20) << riOut[i].Name ;
                    if ( riOut[i].ID == 1){
                        cout << "(����)" ;
                    }
                    cout << setw(10) << riOut[i].Score <<  endl;
                }
            }
            else
            {
                cout <<"��ǰ���÷���:" << pManager->m_numOfRoom  << endl;
            }
        }
        else   /* Look current clents */
            if ( 0 == strcmp(msg, "-t") )
            {
                cout << "��ǰ��������:" << pManager->GetNum() << endl;
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
                        cout << "������Ҷ���������." << endl;
                        pManager->DelClientAll();
                    }
                    else   /* Look for MySql */
                        if ( 0 == strcmp(msg, "-add") )
                        {
                            char buffname[21], buffPass[21];
                            cout << "�û���:";          cin >> buffname;
                            cout << endl << "����:";    cin >> buffPass;
                            if ( 0 == pManager->GetProtocal()->m_SQL.AddUserItem(buffname, buffPass,0) )
                                cout << "���ӳɹ�." << endl;
                        }
                        else   /* Look for MySql */
                            if ( 0 == strcmp(msg, "-del") )
                            {
                                char buffname[21];
                                cout << "�û���:";          cin >> buffname;
                                if ( 0 == pManager->GetProtocal()->m_SQL.DeleteUserItem(buffname) )
                                    cout << "ɾ���ɹ�.";
                            }
                            else   /* Look for MySql */
                                if ( 0 == strcmp(msg, "-?") )
                                {
                                    cout << "-q: ǿ�ƹر�." << endl;
                                    cout << "-t: �鿴��ǰ������Ϣ." << endl;
                                    cout << "-r: �鿴��ǰ������Ϣ." << endl;
                                    cout << "-all: �鿴���ݿ�." << endl;
                                    cout << "-add: �����ʺ�." << endl;
                                    cout << "-del: ɾ���ʺ�." << endl;
                                    cout << "-rank: �鿴���ְ�." << endl;
                                    cout << "-kill: ɱ���������." << endl;
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









