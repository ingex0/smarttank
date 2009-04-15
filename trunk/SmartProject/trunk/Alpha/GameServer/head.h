#ifndef _HEAD_H_
#define _HEAD_H_

#include "config.h"
#include <iostream>
#include <iomanip>
#include <fstream>
#include <vector>
#include <time.h>
#include "assert.h"
using namespace std;

#pragma warning( disable:  4996)
#pragma warning( disable:  4267)
#pragma warning( disable:  4244)

struct MyTimer //Timer�ṹ�壬��������һ����ʱ������Ϣ
{    
    int total_time;  //ÿ��total_time��
    int left_time;   //��ʣleft_time��
    int func;        //�ö�ʱ����ʱ��Ҫִ�еĴ���ı�־
};

#ifdef WIN32
    typedef int socklen_t; 
    typedef int ssize_t; 
    #define MSG_NOSIGNAL 0 // send������:linux�����ΪMSG_NOSIGNAL�����ڷ��ͳ������ܻᵼ�³����˳�
    #include <winsock2.h>  
    #include <windows.h>
    #define GetMyError       GetLastError()
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
    #include <errno.h>           // ������
    #include <signal.h>          // �źŻ���
    #define GetMyError       errno
    #define LockThreadBegin  pthread_mutex_lock(&DD_ClientMgr_Mutex[nID]);
    #define LockThreadEnd  pthread_mutex_unlock(&DD_ClientMgr_Mutex[nID]);
#endif





#endif



 

