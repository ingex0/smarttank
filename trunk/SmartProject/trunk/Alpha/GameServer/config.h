#ifndef _CONFIG_H_
#define _CONFIG_H_

/* �������������� */
#define IP "192.168.35.225"
#define MYPORT 9999
#define MAXCLIENT 100

/* Э������ */
#define HEART_BEAT   0    // ������
#define CHAT         1    // ������Ϣ��(ϵͳ��Ϣ��)
#define LOGIN           10    // ��¼
#define LOGIN_SUCCESS   11    // ��¼�ɹ�
#define LOGIN_FAIL      12    // ��¼ʧ��
#define USER_REGIST     15    // ע���ʺ�
#define USER_DELETE     16    // ɾ���ʺ�
#define USER_JOIN 20    // ��Ҽ���
#define USER_EXIT 21    // ����˳�
#define USER_LIST 22    // �оٷ����û���Ϣ
#define USER_DATA 100   // ��Ϸ���ݣ�������Ϸʱ�����ݽ���


typedef struct _PacketHead_Target
{
    int  iStyle ;        //     
    int  length;         // length of the packet
}PacketHead, *PPacketHead;

#define PACKSIZE 3000
typedef struct _Packet_Target
{
    int iStyle;
    int length;
    char data[PACKSIZE]; // msg
}Packet, *PPacket;

#define SIZEOFPACKET sizeof(Packet)
#define HEADSIZE sizeof(PacketHead)

/* �û����ݿ� */
struct UserLoginPack{
    char Name[21];
    char Password[21];
}; // �û���¼�ô˰�

struct UserInfo {
    int ID;
    char Name[21];
    char Password[21];
    int Score;
    int Rank;
};// �û����ݿ���Ϣ

struct RankInfo {
    int Rank;
    char Name[21];
    int Score;
};// ���а�������Ϣ


#endif









