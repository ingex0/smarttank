/************************************************************
FileName: head.h
Author:   DD.Li     Version :   1.0       Date:2009-2-26
Description:     // MySocket�࣬��װsocket apiʵ��ƽ̨�޹�
***********************************************************/
#ifndef _MYSOCKET_H_
#define _MYSOCKET_H_

#include "head.h"

class MySocket
{
public:
    
    MySocket();
    ~MySocket();         // �������ر��׽���

    void Bind(int port);
    void Listen();    // ����
    void Close();     // �ر�
    int  Accept(MySocket *pSockConn);       // ��������<���棺���ɴ���MySocket����,ԭ��δ֪>
    bool Connect( char* chIP, long port);   // ����

    char* GetIP();                    // ����IP��ַ
    char* GetPort(char *outPort);     // ���ض˿ں�(�ַ�����ʽ)
    long  GetPort();                  // ���ض˿ں�
    bool  GetUserInfo(UserInfo &ui);  // �����û���Ϣ
    bool  SendPacket( Packet& pack, int realSize);              // �������ݰ�
    bool  SendPacketHead( PacketHead& packHead);                // �������ݰ�ͷ
    bool  SendPacketChat( char *msg, long leng );               // ����chat��
    bool  RecvPacket( PacketHead& packHead, Packet& pack);      // ��������,���ؽ������ݳ���
    bool  IsLiving(){
        return m_isLiving;
    }
    void  BindName(char* name){         // ��name
        strcpy( m_name, name);
    }
public:
    int m_socket;    // ����Linux Socket
    int m_ID;        // ��ʾID
    char m_sIP[16];  // IP address  
    char m_name[21]; // �û���
    bool m_bHost;      // �Ƿ�����
    bool m_bInRoom;    // �ڷ�����
    int  m_score;      // ����
    int  m_rank;       // ����

    bool m_isLiving;
    struct sockaddr_in m_addr;
    static int m_sinSize; // �̶���С

private:
    bool Send(char *msg, long leng);        

};

// ����ȫ��socket����
void  StartSocket();
void  DestroySocket();









#endif







