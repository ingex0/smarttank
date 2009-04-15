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
#include "MyRoom.h"
class ClientManager;

class GameProtocal
{
public:
    GameProtocal();
    ~GameProtocal();
    void AttachManager( ClientManager* pMgr){// 添加管理者
        pManager = pMgr;
    }	
    /*  发送给所有人，包括自己    */
    bool SendToAll(MySocket* pSockClient, Packet &pack);

    /*  发送给pSockClient玩家    */
    bool SendToOne(MySocket* pSockClient, Packet &pack);

    /*  发送给其他玩家    */
    bool SendToOthers(MySocket* pSockClient, Packet &pack);

    /*  发送包头    */
    bool SendHead(MySocket* pSockClient, PacketHead &packHead);

    /*  正常游戏数据    */
    bool SendGameData(MySocket *pSockClient, Packet &pack);

    /*  用户注册    */
    bool UserRegist(MySocket *pSockClient, Packet &pack);

    /*  用户登录    */
    bool UserLogin(MySocket *pSockClient, Packet &pack);

    /*  用户自己信息    */
    bool MyInfo(MySocket *pSockClient, Packet &pack);

    /*  用户列表    */
    bool ListUser(MySocket *pSockClient, Packet &pack);

    /*  排行列表    */
    bool ListRank(MySocket *pSockClient, Packet &pack);

    /*  创建房间    */
    bool CreateRoom(MySocket *pSockClient, Packet &pack);

    /*  刷新房间    */
    bool ListRoom(MySocket *pSockClient, Packet &pack);

    /*  进入房间 */
    bool EnterRoom(MySocket *pSockClient, Packet &pack);

    /*  离开房间 */
    bool ExitRoom(MySocket *pSockClient, Packet &pack);

    /*  游戏开始 */
    bool GameGo(MySocket *pSockClient, Packet &pack);

    /*  保存游戏结果到数据库 */
    bool SaveGameResult(MySocket *pSockClient, Packet &pack);

    /* 用户离开 */
    bool UserExit(MySocket *pSockClient);

    Team5_DB    m_SQL;       // 数据库   
private:
    ClientManager *pManager; // 管理者    
    static long m_recvPackNum;
    ofstream outLog;
};


#endif









