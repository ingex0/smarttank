/************************************************************
FileName: head.h
Author:   DD.Li     Version :   1.0       Date:2009-2-26
Description:     // MySocket类，封装socket api实现平台无关
***********************************************************/
#ifndef _MYSOCKET_H_
#define _MYSOCKET_H_

#include "head.h"

class MySocket
{
public:
    
    MySocket();
    ~MySocket();         // 析构，关闭套接字

    void Bind(int port);
    void Listen();    // 监听
    void Close();     // 关闭
    int  Accept(MySocket *pSockConn);       // 接受连接<警告：不可传递MySocket引用,原因未知>
    bool Connect( char* chIP, long port);   // 连接
    bool Send(char *msg, long leng);        // 发送

    char* GetIP();                    // 返回IP地址
    char* GetPort(char *outPort);     // 返回端口号(字符串形式)
    long  GetPort();                  // 返回端口号
    bool  SendPacket( Packet& pack, int realSize);              // 发送数据包
    bool  SendPacketHead( PacketHead& packHead);                // 发送数据包头
    bool  SendPacketChat( char *msg, long leng );               // 发送chat包
    long  RecvPacket( Packet& pack);                            // 接受数据,返回接收数据长度

    void  BindName(char* name){         // 绑定name
        strcpy( m_name, name);
    }
public:
    int m_socket;    // 兼容Linux Socket
    int m_ID;        // 标示ID
    char m_sIP[16];  // IP address  
    char m_name[21]; // 用户名
    struct sockaddr_in m_addr;

    static int m_sinSize; // 固定大小

};

// 其他全局socket函数
void  StartSocket();
void  DestroySocket();









#endif







