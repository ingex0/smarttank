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
    bool Send(char *msg, long leng);        // ����

    char* GetIP();                    // ����IP��ַ
    char* GetPort(char *outPort);     // ���ض˿ں�(�ַ�����ʽ)
    long  GetPort();                  // ���ض˿ں�
    bool  SendPacket( Packet& pack );          // �������ݰ�
    bool  SendPacket( char *msg, long leng );  // �������ݰ�
    long  RecvPacket( Packet& pack);           // ��������,���ؽ������ݳ���

public:
    int m_socket; // ����Linux Socket
    int m_ID;     // ��ʾID
    struct sockaddr_in m_addr;
    char m_sIP[16]; //IP address  
    
    static int m_sinSize; // �̶���С

};

// ����ȫ��socket����
void  StartSocket();
void  DestroySocket();









#endif







