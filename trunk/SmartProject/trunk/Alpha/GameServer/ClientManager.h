/************************************************************
FileName: ClientManager.cpp
Author:   DD.Li     Version :   1.0       Date:2009-2-26
Description:     // 管理，维护用户套接字  
***********************************************************/
#ifndef _CLIENTMANAGER_H_
#define _CLIENTMANAGER_H_

#include "head.h"
#include "mysocket.h"
#include "GameProtocol.h"
#include "ClientManager.h"
#include "MyRoom.h"
#include <vector>
using namespace std;

class MyRoom;

class ClientManager
{
public:
    ClientManager();
    ~ClientManager();
    int  GetClient();
    MySocket *AddServer( MySocket *psock);  // 记录服务器.返回socket指针
    MySocket *AddClient( MySocket *psock);  // 新用户加入.返回socket指针
    MySocket *GetClient( int iID);          // 得到某用户.返回socket指针
    MySocket *GetSafeClient(int iID);       // garbage
    void SetTimer(int nID);                 // 设置定时器
    bool DelClient( MySocket *psock);       // 删除用户
    bool DelClientAll();                    // 删除all

	GameProtocal* GetProtocal(){				 // 取得协议分析器
		return m_protocol;
	};
	void AttachProtocal( GameProtocal* pProtocal){// 添加协议
		m_protocol = pProtocal;
	}
    int GetNum(){       //  返回客户总数
        return (int)m_pClient.size();
    }
    void HeartBeatFun(MySocket *psock);

	GameProtocal *m_protocol;		// 协议分析器
    MyRoom *m_room;                 // Rooms
    int m_numOfRoom;                // number of Rooms
    MyTimer myTimer[MAXCLIENT];    //定义Timer类型的数组，用来保存所有的定时器
protected:
    vector<MySocket*> m_pClient;   // 客户池<已有>
    vector<MySocket*> m_pEmpty;    // 客户池<可用>
    MySocket *m_pServer;           // 服务器
};






#endif









