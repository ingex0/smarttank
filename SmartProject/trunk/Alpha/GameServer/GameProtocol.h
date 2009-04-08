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
    int RecvPacketBody(MySocket* pSockClient, PacketHead &packHead, Packet &pack); // 接受包体
    bool AnalysePacketHead(MySocket* pSockClient, PacketHead &packHead); // 分析协议包
    bool SendToAll(MySocket* pSockClient, PacketHead &packHead);
    bool SendToOne(MySocket* pSockClient, PacketHead &packHead);
    bool SendToOthers(MySocket* pSockClient, PacketHead &packHead);
    bool SendHead(MySocket* pSockClient, PacketHead &packHead);

    /*  用户注册    */
    bool UserRegist(MySocket *pSockClient, PacketHead &packHead);

    /*  用户登录    */
    bool UserLogin(MySocket *pSockClient, PacketHead &packHead);

    /*  用户列表    */
    bool ListUser(MySocket *pSockClient, PacketHead &packHead);

   Team5_DB    m_SQL;       // 数据库   
private:
    ClientManager *pManager; // 管理者    
    static long m_recvPackNum;
};


#endif









