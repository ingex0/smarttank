/************************************************************
FileName: ClientManager.cpp
Author:   DD.Li     Version :   1.0       Date:2009-2-26
Description:     // ����ά���û��׽���  
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
    MySocket *AddServer( MySocket *psock);  // ��¼������.����socketָ��
    MySocket *AddClient( MySocket *psock);  // ���û�����.����socketָ��
    MySocket *GetClient( int iID);          // �õ�ĳ�û�.����socketָ��
    bool DelClient( MySocket *psock);       // ɾ���û�

	GameProtocal* GetProtocal(){				 // ȡ��Э�������
		return m_protocol;
	};
	void AttachProtocal( GameProtocal* pProtocal){// ���Э��
		m_protocol = pProtocal;
	}
    int GetNum(){       //  ���ؿͻ�����
        return (int)m_pClient.size();
    }
	GameProtocal *m_protocol;		// Э�������

protected:
    vector<MySocket*> m_pClient;    // ������
    MySocket *m_pServer;            // �ͻ���
};


#endif









