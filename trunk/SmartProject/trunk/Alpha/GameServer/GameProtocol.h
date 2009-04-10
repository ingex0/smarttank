/************************************************************
FileName: GameProtocal.h
Author:   DD.Li     Version :   1.0       Date:2009-4-6
Description: 游戏协议
***********************************************************/
#ifndef _GAMEPROTOCAL_H_
#define _GAMEPROTOCAL_H_

#include "head.h"
#include "mysocket.h"
#include "ClientManager.h"
#include "MyDataBase.h"

class ClientManager;

class GameProtocal
{
public:
    GameProtocal();
    ~GameProtocal();
    void AttachManager( ClientManager* pMgr){// 添加管理者
        pManager = pMgr;
    }	
    bool AnalysePacketHead(MySocket* pSockClient, PacketHead &packHead); // 暂时没用...

    /* 根据包头，接受指定大小包体 */
    int RecvPacketBody(MySocket* pSockClient, PacketHead &packHead, Packet &pack); 

    /*  发送给所有人，包括自己    */
    bool SendToAll(MySocket* pSockClient, PacketHead &packHead);

    /*  发送给pSockClient玩家    */
    bool SendToOne(MySocket* pSockClient, PacketHead &packHead);

    /*  发送给其他玩家    */
    bool SendToOthers(MySocket* pSockClient, PacketHead &packHead);

    /*  发送包头    */
    bool SendHead(MySocket* pSockClient, PacketHead &packHead);

    /*  用户注册    */
    bool UserRegist(MySocket *pSockClient, PacketHead &packHead);

    /*  用户登录    */
    bool UserLogin(MySocket *pSockClient, PacketHead &packHead);

    /*  用户列表    */
    bool ListUser(MySocket *pSockClient, PacketHead &packHead);

    /*  排行列表    */
    bool ListRank(MySocket *pSockClient, PacketHead &packHead);

    Team5_DB    m_SQL;       // 数据库   
private:
    ClientManager *pManager; // 管理者    
    static long m_recvPackNum;
    ofstream outLog;
};


#endif









