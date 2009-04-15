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
#include "MyRoom.h"
class ClientManager;

class GameProtocal
{
public:
    GameProtocal();
    ~GameProtocal();
    void AttachManager( ClientManager* pMgr){// ��ӹ�����
        pManager = pMgr;
    }	
    /*  ���͸������ˣ������Լ�    */
    bool SendToAll(MySocket* pSockClient, Packet &pack);

    /*  ���͸�pSockClient���    */
    bool SendToOne(MySocket* pSockClient, Packet &pack);

    /*  ���͸��������    */
    bool SendToOthers(MySocket* pSockClient, Packet &pack);

    /*  ���Ͱ�ͷ    */
    bool SendHead(MySocket* pSockClient, PacketHead &packHead);

    /*  ������Ϸ����    */
    bool SendGameData(MySocket *pSockClient, Packet &pack);

    /*  �û�ע��    */
    bool UserRegist(MySocket *pSockClient, Packet &pack);

    /*  �û���¼    */
    bool UserLogin(MySocket *pSockClient, Packet &pack);

    /*  �û��Լ���Ϣ    */
    bool MyInfo(MySocket *pSockClient, Packet &pack);

    /*  �û��б�    */
    bool ListUser(MySocket *pSockClient, Packet &pack);

    /*  �����б�    */
    bool ListRank(MySocket *pSockClient, Packet &pack);

    /*  ��������    */
    bool CreateRoom(MySocket *pSockClient, Packet &pack);

    /*  ˢ�·���    */
    bool ListRoom(MySocket *pSockClient, Packet &pack);

    /*  ���뷿�� */
    bool EnterRoom(MySocket *pSockClient, Packet &pack);

    /*  �뿪���� */
    bool ExitRoom(MySocket *pSockClient, Packet &pack);

    /*  ��Ϸ��ʼ */
    bool GameGo(MySocket *pSockClient, Packet &pack);

    /*  ������Ϸ��������ݿ� */
    bool SaveGameResult(MySocket *pSockClient, Packet &pack);

    /* �û��뿪 */
    bool UserExit(MySocket *pSockClient);

    Team5_DB    m_SQL;       // ���ݿ�   
private:
    ClientManager *pManager; // ������    
    static long m_recvPackNum;
    ofstream outLog;
};


#endif









