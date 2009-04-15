#ifndef _CONFIG_H_
#define _CONFIG_H_

/* �������������� */
#define IP "192.168.35.225"     // ������IP��ַ
#define MYPORT 9999             // �������˿ں�
#define MYSQL_USERNAME "root"   // ������MySql���ݿ��û���
#ifdef WIN32
    #define MYSQL_PASS "123456"
#else
    #define MYSQL_PASS NULL     // ��˾������MySql����Ϊ��
#endif

#define MAXCLIENT 100       // ���������
#define PACKSIZE 10240      // �����հ�����
#define MAXROOMMATE 2       // ������������
#define NUM_OF_RANK 10      // ���а�һҳ

/* Э������ : ��¼���� */
#define LOGIN           10    // ��¼����־
#define LOGIN_SUCCESS   11    // ��¼�ɹ�����
#define LOGIN_FAIL      12    // ��¼ʧ�ܷ���
#define USER_REGIST     15    // ע���ʺ�
#define USER_DELETE     16    // ɾ���ʺ�

/* Э������ : ��Ϸ���� */
#define HEART_BEAT   0  // ������
#define CHAT         1  // ������Ϣ��(ϵͳ��Ϣ��)
#define USER_EXIT 21    // �˳�Ӧ���

#define ROOM_CREATE    30    // ��������
#define ROOM_JOIN      31    // ���뷿��
#define ROOM_DESTROY   32    // ���ٷ���
#define ROOM_LIST      33    // �оٷ�����Ϣ
#define ROOM_LISTUSER  34    // �оٷ��������û���Ϣ
#define CREATE_SUCCESS 35    // ��������ɹ�
#define CREATE_FAIL    36    // ��������ʧ��
#define JOIN_SUCCESS   37    // ���뷿��ɹ�
#define JOIN_FAIL      38    // ���뷿��ʧ��
#define ROOM_EXIT      39    // ���������˳�

#define USER_INFO 40    // ��ѯ�Լ����ʺ���Ϣ
#define USER_RANK 50    // �о����а���Ϣ(M-N)

#define GAME_GO           70    // ��Ϸ��ʽ��ʼ
#define GAME_START_FAIL   71    // ��ʼ����ʧ��
#define GAME_OVER         72    // ��ʼ����ʧ��
#define GAME_RESULT       80    // ��Ϸ�������
#define USER_DATA         100   // ��Ϸ���ݣ�������Ϸʱ�����ݽ���

typedef struct _PacketHead_Target
{
    int  iStyle ;        // ���ݰ�����
    int  length;         // ���峤��(��������ͷ!)
}PacketHead, *PPacketHead;

// �䳤���ݰ�: ǰ8���ֽ�Ϊ��ͷ������������������
typedef struct _Packet_Target
{
    int iStyle;          // ���ݰ�����
    int length;          // ���峤��(��������ͷ!)
    char data[PACKSIZE]; // msg
}Packet, *PPacket;

#define SIZEOFPACKET sizeof(Packet)
#define HEADSIZE sizeof(PacketHead)

/* �û����ݿ� */

// �û���¼�ô˰�
struct UserLoginPack{
    char Name[21];
    char Password[21];
}; 

// �û����ݿ���Ϣ
struct UserInfo {
    int ID;
    char Name[21];
    char Password[21];
    int Score;
    int Rank;
};

// ���а�������Ϣ
struct RankInfo {
    int Rank;      // ����
    int Score;     // �÷�
    char Name[21]; // �û���
};

// ����������Ϣ 
struct RoomInfo {
    int nID;        // �����
    int players;    // �÷����м�����
    int bBegin;     // ��ʾ�÷�����Ϸ�Ƿ�ʼ��0:û��ʼ���Լ���,1:�Ѿ���ʼ��ֹ���� 
    char Name[21];  // ��������
};




#endif









