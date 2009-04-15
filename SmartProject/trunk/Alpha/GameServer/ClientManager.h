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
    MySocket *AddServer( MySocket *psock);  // ��¼������.����socketָ��
    MySocket *AddClient( MySocket *psock);  // ���û�����.����socketָ��
    MySocket *GetClient( int iID);          // �õ�ĳ�û�.����socketָ��
    MySocket *GetSafeClient(int iID);       // garbage
    void SetTimer(int nID);                 // ���ö�ʱ��
    bool DelClient( MySocket *psock);       // ɾ���û�
    bool DelClientAll();                    // ɾ��all

	GameProtocal* GetProtocal(){				 // ȡ��Э�������
		return m_protocol;
	};
	void AttachProtocal( GameProtocal* pProtocal){// ���Э��
		m_protocol = pProtocal;
	}
    int GetNum(){       //  ���ؿͻ�����
        return (int)m_pClient.size();
    }
    void HeartBeatFun(MySocket *psock);

	GameProtocal *m_protocol;		// Э�������
    MyRoom *m_room;                 // Rooms
    int m_numOfRoom;                // number of Rooms
    MyTimer myTimer[MAXCLIENT];    //����Timer���͵����飬�����������еĶ�ʱ��
protected:
    vector<MySocket*> m_pClient;   // �ͻ���<����>
    vector<MySocket*> m_pEmpty;    // �ͻ���<����>
    MySocket *m_pServer;           // ������
};






#endif









