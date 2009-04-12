/************************************************************
FileName: GameProtocal.h
Author:   DD.Li     Version :   1.0       Date:2009-4-6
Description: ��ϷЭ��
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
    void AttachManager( ClientManager* pMgr){// ���ӹ�����
        pManager = pMgr;
    }	
    bool AnalysePacketHead(MySocket* pSockClient, PacketHead &packHead); // ��ʱû��...

    /* ���ݰ�ͷ������ָ����С���� */
    int RecvPacketBody(MySocket* pSockClient, PacketHead &packHead, Packet &pack); 

    /*  ���͸������ˣ������Լ�    */
    bool SendToAll(MySocket* pSockClient, PacketHead &packHead);

    /*  ���͸�pSockClient���    */
    bool SendToOne(MySocket* pSockClient, PacketHead &packHead);

    /*  ���͸��������    */
    bool SendToOthers(MySocket* pSockClient, PacketHead &packHead);

    /*  ���Ͱ�ͷ    */
    bool SendHead(MySocket* pSockClient, PacketHead &packHead);

    /*  �û�ע��    */
    bool UserRegist(MySocket *pSockClient, PacketHead &packHead);

    /*  �û���¼    */
    bool UserLogin(MySocket *pSockClient, PacketHead &packHead);

    /*  �û��б�    */
    bool ListUser(MySocket *pSockClient, PacketHead &packHead);

    /*  �����б�    */
    bool ListRank(MySocket *pSockClient, PacketHead &packHead);

    Team5_DB    m_SQL;       // ���ݿ�   
private:
    ClientManager *pManager; // ������    
    static long m_recvPackNum;
    ofstream outLog;
};


#endif








