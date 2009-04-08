#include "head.h"
#include "mythread.h"
#include "ClientManager.h"
#include <iomanip>

extern pthread_mutex_t DD_ClientMgr_Mutex[MAXCLIENT];
#ifndef WIN32
pthread_cond_t cond = PTHREAD_COND_INITIALIZER;
#endif 

/************************************************************************/
/* 服务器发送线程<传送控制台输入字符，可有可无的线程>         
/************************************************************************/
THREADHEAD ThreadSend(void* para)
{
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
    long   recvPackets = 0;
    int ret;
    PThreadParam pp;
    pp = (PThreadParam)pparam;
    MySocket *pSockClient = pp->sock;
    ClientManager *pManager = pp->mgr;
    cout << "ThreadClient." << endl;

    /* Main Loop */
    while (1)
    {
        PacketHead packHead;
        memset(&packHead, 0, sizeof(PacketHead));
        /* 收包 */
        if ( -1 == ( ret = recv(pSockClient->m_socket, (char*)&packHead, HEADSIZE, MSG_NOSIGNAL) ) )
        {   
            // 失败，删除该客户
            cout << " Recv Error." << endl;
            pManager->DelClient( pSockClient );
            return 0;
        }

        /* 由GameProtocal定义的协议分析器来分析数据包 */

        switch(packHead.iStyle)
        {     
        case USER_LIST:  
            break;

        /*  用户注册    */
        case USER_REGIST:     
            break;

        /*  用户登录    */
        case LOGIN:     
            pManager->GetProtocal()->UserLogin(pSockClient, packHead);
            break;

        /*  正常游戏包  */
        case USER_DATA:  
            {
                if ( ++recvPackets%100 == 0)
                    cout << "包头:" << packHead.iStyle << ',' << packHead.length << endl;

                MySocket *pSock;
                int ret;
                Packet pack;
                int realSize = packHead.length+HEADSIZE;
                memset(&pack, 0, sizeof(Packet));
                memcpy(&pack, &packHead, HEADSIZE);

                if ( -1 == ( ret = recv(pSockClient->m_socket, (char*)(&pack)+HEADSIZE,
                    packHead.length, MSG_NOSIGNAL) ) ) 
                {   // 失败，删除该客户
                    pManager->DelClient( pSockClient );
                    cout << "Wrong RecvPacketBody" << endl;
                    return 0;
                }
                for ( int i = 0; i < pManager->GetNum(); i++)
                {
                    pSock = pManager->GetClient(i);
                    /*if ( pSockClient == ( ) )
                    {
                        continue;
                    }*/
                    int nID = pSock->m_ID;

                    LockThreadBegin
//#ifndef  WIN32
//                    pthread_cond_wait(&cond, &DD_ClientMgr_Mutex[nID]);
//#endif
                    /**/ if ( -1 == send(pSock->m_socket, (char*)(&pack), realSize, MSG_NOSIGNAL))
                    {
                        cout << errno << ":聊天包." << endl;
                        pManager->DelClient( pSock );
                    }
//#ifndef  WIN32
//                    pthread_cond_broadcast(&cond);
//#endif
                    LockThreadEnd
                } 
                // pManager->GetProtocal()->SendToOthers(pSockClient, packHead);
                //Sleep(1);
                break;
            }

        /*  用户离开  */
        case USER_EXIT:  
            pManager->GetProtocal()->SendHead(pSockClient, packHead);
            pManager->DelClient(pSockClient);
            
            return 0;
            break;

        default:
            cout << "Unknow Packet with pack-head:" << packHead.iStyle << endl;
            break; 
        }

    }// while
    pSockClient->Close();
    return 0;
}


bool  DoServercommand(ClientManager *pManager, char* msg)
{
    if ( 0 == strcmp(msg, "-q") )
    {
        exit(-1);
    }else
        if ( 0 == strcmp(msg, "-t") )
        {
            cout << "当前连接总数:" << pManager->GetNum() << endl;
            for ( int i = 0; i < pManager->GetNum(); i++)
            {
                cout << pManager->GetClient(i)->GetIP()  << " : " 
                    << pManager->GetClient(i)->GetPort() << endl;
            }
        }else
            if ( 0 == strcmp(msg, "-top") )
            {
                UserInfo ui[10]; 
                cout << setw(-10) << "ID" << setw(10) << "Name" << setw(10) << "Password" << 
                    setw(10) << "Rank" << setw(10) << "Score" << endl;
                for ( int i = 0; i < pManager->m_protocol->m_SQL.GetUserList(1, 10, ui); i++)
                {
                    cout << setw(-10) << ui[i].ID << setw(10) << ui[i].Name << setw(10) << ui[i].Password 
                        << setw(10)  << ui[i].Rank << setw(10) << ui[i].Score << endl;
                }
            }
            else
            {
                /* Bad command */
                return false;
            }
            return true;
}












