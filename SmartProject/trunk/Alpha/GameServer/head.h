#ifndef _HEAD_H_
#define _HEAD_H_

#include "config.h"
#include <iostream>
using namespace std;

#pragma warning( disable:  4996)
#pragma warning( disable:  4267)
#pragma warning( disable:  4244)
#ifdef WIN32
    typedef int socklen_t; 
    typedef int ssize_t; 
    #define MSG_NOSIGNAL 0 // send最后参数:linux最好设为MSG_NOSIGNAL否则在发送出错后可能会导致程序退出
    #include <winsock2.h>  
    #include <windows.h>
    #define pthread_mutex_t  CRITICAL_SECTION
    #define LockThreadBegin  EnterCriticalSection(&DD_ClientMgr_Mutex[nID]);
    #define LockThreadEnd    LeaveCriticalSection(&DD_ClientMgr_Mutex[nID]);
#else
    typedef int SOCKET; 
    typedef unsigned char BYTE; 
    typedef unsigned long DWORD; 
    #define FALSE 0 
    #define SOCKET_ERROR (-1) 

    #include <sys/types.h>
    #include <sys/socket.h>
    #include <sys/time.h>
    #include <stdio.h>
    #include <netinet/in.h>
    #include <arpa/inet.h>
    #include <unistd.h>
    #include <pthread.h>         
    #include <errno.h>           // 错误处理
    #include <signal.h>          // 信号机制
    #define LockThreadBegin  pthread_mutex_lock(&DD_ClientMgr_Mutex[nID]);
    #define LockThreadEnd  pthread_mutex_unlock(&DD_ClientMgr_Mutex[nID]);
#endif





#endif



 

