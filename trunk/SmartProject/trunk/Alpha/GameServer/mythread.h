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

/* �̲߳��� */
typedef struct _ThreadParam_Target
{
    MySocket *sock;     // �������׽���
    ClientManager *mgr; // �û�������
}ThreadParam, *PThreadParam;




#endif






