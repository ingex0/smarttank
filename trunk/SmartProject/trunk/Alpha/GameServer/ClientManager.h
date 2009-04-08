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
#include <vector>
using namespace std;

class GameProtocal;
class ClientManager
{
public:
    ClientManager();
    ~ClientManager();
    int  GetClient();
    MySocket *AddServer( MySocket *psock);  // 记录服务器.返回socket指针
    MySocket *AddClient( MySocket *psock);  // 新用户加入.返回socket指针
    MySocket *GetClient( int iID);          // 得到某用户.返回socket指针
    bool DelClient( MySocket *psock);       // 删除用户

	GameProtocal* GetProtocal(){				 // 取得协议分析器
		return m_protocol;
	};
	void AttachProtocal( GameProtocal* pProtocal){// 添加协议
		m_protocol = pProtocal;
	}
    int GetNum(){       //  返回客户总数
        return (int)m_pClient.size();
    }
	GameProtocal *m_protocol;		// 协议分析器

protected:
    vector<MySocket*> m_pClient;    // 服务器
    MySocket *m_pServer;            // 客户池
};


#endif









