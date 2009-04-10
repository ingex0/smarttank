/************************************************************
FileName: MyDataBase.H
Author:   Seek.Yao     Version :   1.0       Date:2009-2-26
Description:     // ����MySql
***********************************************************/
#ifndef _MYDATABASE_H_
#define _MYDATABASE_H_

#ifdef WIN32
    #include <windows.h>
    #pragma comment(lib, "D:\\Program Files\\MySQL\\MySQL Server 5.1\\lib\\opt\\libmysql.lib")
    #pragma comment(lib, "D:\\Program Files\\MySQL\\MySQL Server 5.1\\lib\\opt\\mysqlclient.lib")
    #pragma comment(lib, "D:\\Program Files\\MySQL\\MySQL Server 5.1\\lib\\opt\\mysys.lib")
    #pragma comment(lib, "D:\\Program Files\\MySQL\\MySQL Server 5.1\\lib\\opt\\regex.lib")
    #pragma comment(lib, "D:\\Program Files\\MySQL\\MySQL Server 5.1\\lib\\opt\\strings.lib")
    #pragma comment(lib, "D:\\Program Files\\MySQL\\MySQL Server 5.1\\lib\\opt\\zlib.lib")
#endif

#include <mysql.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "config.h"

class Team5_DB {
private:
    MYSQL *mysql;
    MYSQL_RES *result;
    MYSQL_ROW row;
    char buffer[256];
public:
    //��  ��: �������ݿ�
    //����ֵ: 0 -------------��������
    //        -1 ------------�쳣
    Team5_DB();
    int Connect();

    //��  ��: �Ͽ������ݿ������
    void Disconnect();

    //��  ��: ���һ���û���Ϣ
    //��  ��: Name ----------�û���
    //        Password-------����
    //        StartScore-----��ʼ����
    //����ֵ: 0 -------------��������
    //        -1 ------------�쳣
    int AddUserItem(char *Name, char *Password, int StartScore);

    //��  ��: ɾ��һ���û���Ϣ
    //��  ��: Name ----------�û���
    //����ֵ: 0 -------------��������
    //        -1 ------------�쳣
    int DeleteUserItem(char *Name);

    //��  ��: �����û���
    //��  ��: Name ----------�û���
    //        NewName--------���û���
    //����ֵ: 0 -------------��������
    //        -1 ------------�쳣
    int SetUserName(char *Name, char *NewName);

    //��  ��: �����û�����
    //��  ��: Name ----------�û���
    //        NewPassword----������
    //����ֵ: 0 -------------��������
    //        -1 ------------�쳣
    int SetUserPassword(char *Name, char *NewPassword);

    //��  ��: �����û�����
    //��  ��: Name ----------�û���
    //        Score----------����
    //����ֵ: 0 -------------��������
    //        -1 ------------�쳣
    int SetUserScore(char *Name, int Score);

    //��  ��: ����û�ID
    //��  ��: Name ----------�û���
    //����ֵ: >=0 -----------�û�ID
    //        -1 ------------�쳣
    int GetUserID(char *Name);

    //��  ��: ����û�����
    //��  ��: Name ----------�û���
    //        RetPassword----���ص�����д�� RetPassword ��ָ��Ŀռ�
    //����ֵ: 0 -------------��������
    //        -1 ------------�쳣
    int GetUserPassword(char *Name, char *RetPassword);

    //��  ��: ����û�����
    //��  ��: Name ----------�û���
    //����ֵ: >=0 -----------�û��Ļ���
    //        -1 ------------�쳣
    int GetUserScore(char *Name);

    //��  ��: ����û�����
    //��  ��: Name ----------�û���
    //����ֵ: >=0 -----------�û�������
    //        -1 ------------�쳣
    int GetUserRank(char *Name);

    //��  ��: ����û�������Ϣ
    //��  ��: Name ----------�û���
    //        RetInfo -------���ص��û���Ϣд�� RetInfo ��
    //����ֵ: 0 -------------��������
    //        -1 ------------�쳣
    int GetUserInfo(char *Name, UserInfo &RetInfo);

    //��  ��: ����û�����
    //��  ��: Name ----------�û���
    //����ֵ: >=0 -----------�û�����
    //        -1 ------------�쳣
    int GetUserCount();

    //��  ��: ��û��������б�
    //��  ��: m -------------�ӵ� m + 1 ����ʼ
    //        n ------------- n ���û�
    //        RetPage -------�������� [m + 1, m + n] �����ڵ� RankInfo д�� RetPage
    //����ֵ: >=0 -----------ʵ�ʻ�õ� RankInfo ����
    //        -1 ------------�쳣
    int GetRankList(int m, int n, RankInfo *RetPage);


    //��  ��: ����û������б���ע���Ⱥ�˳��
    //��  ��: m -------------�ӵ� m + 1 ����ʼ
    //        n ------------- n ���û�
    //        RetPage -------��ע��˳���� [m + 1, m + n] �����ڵ� UserInfo д�� RetPage
    //����ֵ: >=0 -----------ʵ�ʻ�õ� UserInfo ����
    //        -1 ------------�쳣
    int GetUserList(int m, int n, UserInfo *RetPage);

    //��  ��: ���ݿ⸴λ
    //����ֵ: 0 -------------��������
    //        -1 ------------�쳣
    int Reset();
};



#endif




