#ifndef _CONFIG_H_
#define _CONFIG_H_

/* 服务器基本配置 */
#define IP "192.168.35.225"
#define MYPORT 9999
#define MAXCLIENT 100

/* 协议配置 */
#define HEART_BEAT   0    // 心跳包
#define CHAT         1    // 聊天消息包(系统消息包)
#define LOGIN           10    // 登录
#define LOGIN_SUCCESS   11    // 登录成功
#define LOGIN_FAIL      12    // 登录失败
#define USER_REGIST     15    // 注册帐号
#define USER_DELETE     16    // 删除帐号
#define USER_JOIN 20    // 玩家加入
#define USER_EXIT 21    // 玩家退出
#define USER_LIST 22    // 列举房间用户信息
#define USER_DATA 100   // 游戏数据：正常游戏时的数据交换


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

/* 用户数据库 */
struct UserLoginPack{
    char Name[21];
    char Password[21];
}; // 用户登录用此包

struct UserInfo {
    int ID;
    char Name[21];
    char Password[21];
    int Score;
    int Rank;
};// 用户数据库信息

struct RankInfo {
    int Rank;
    char Name[21];
    int Score;
};// 排行榜数据信息


#endif









