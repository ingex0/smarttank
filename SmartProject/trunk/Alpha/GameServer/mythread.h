#ifndef _MYTHREAD_H_
#define _MYTHREAD_H_

#include "mysocket.h"
#include "ClientManager.h"


#ifdef WIN32 
    #define THREADHEAD int
#else
    #define THREADHEAD void *
#endif

THREADHEAD   ThreadClient(void* pparam);
THREADHEAD   ThreadSend  (void* pparam);
bool  DoServercommand(ClientManager *pManager, char* msg);

/* 线程参数 */
typedef struct _ThreadParam_Target
{
    MySocket *sock;     // 服务器套接字
    ClientManager *mgr; // 用户管理器
}ThreadParam, *PThreadParam;




#endif






