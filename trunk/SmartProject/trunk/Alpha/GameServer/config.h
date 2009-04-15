#ifndef _CONFIG_H_
#define _CONFIG_H_

/* 服务器基本配置 */
#define IP "192.168.35.225"     // 服务器IP地址
#define MYPORT 9999             // 服务器端口号
#define MYSQL_USERNAME "root"   // 服务器MySql数据库用户名
#ifdef WIN32
    #define MYSQL_PASS "123456"
#else
    #define MYSQL_PASS NULL     // 公司服务器MySql密码为空
#endif

#define MAXCLIENT 100       // 总玩家上限
#define PACKSIZE 10240      // 单次收包上限
#define MAXROOMMATE 2       // 房间人数上限
#define NUM_OF_RANK 10      // 排行榜一页

/* 协议配置 : 登录部分 */
#define LOGIN           10    // 登录包标志
#define LOGIN_SUCCESS   11    // 登录成功反馈
#define LOGIN_FAIL      12    // 登录失败反馈
#define USER_REGIST     15    // 注册帐号
#define USER_DELETE     16    // 删除帐号

/* 协议配置 : 游戏部分 */
#define HEART_BEAT   0  // 心跳包
#define CHAT         1  // 聊天消息包(系统消息包)
#define USER_EXIT 21    // 退出应答包

#define ROOM_CREATE    30    // 创建房间
#define ROOM_JOIN      31    // 加入房间
#define ROOM_DESTROY   32    // 销毁房间
#define ROOM_LIST      33    // 列举房间信息
#define ROOM_LISTUSER  34    // 列举房间所有用户信息
#define CREATE_SUCCESS 35    // 创建房间成功
#define CREATE_FAIL    36    // 创建房间失败
#define JOIN_SUCCESS   37    // 加入房间成功
#define JOIN_FAIL      38    // 加入房间失败
#define ROOM_EXIT      39    // 房间有人退出

#define USER_INFO 40    // 查询自己的帐号信息
#define USER_RANK 50    // 列举排行榜信息(M-N)

#define GAME_GO           70    // 游戏正式开始
#define GAME_START_FAIL   71    // 开始请求失败
#define GAME_OVER         72    // 开始请求失败
#define GAME_RESULT       80    // 游戏结果保存
#define USER_DATA         100   // 游戏数据：正常游戏时的数据交换

typedef struct _PacketHead_Target
{
    int  iStyle ;        // 数据包类型
    int  length;         // 包体长度(不包括包头!)
}PacketHead, *PPacketHead;

// 变长数据包: 前8个字节为包头，后面可任意填充数据
typedef struct _Packet_Target
{
    int iStyle;          // 数据包类型
    int length;          // 包体长度(不包括包头!)
    char data[PACKSIZE]; // msg
}Packet, *PPacket;

#define SIZEOFPACKET sizeof(Packet)
#define HEADSIZE sizeof(PacketHead)

/* 用户数据库 */

// 用户登录用此包
struct UserLoginPack{
    char Name[21];
    char Password[21];
}; 

// 用户数据库信息
struct UserInfo {
    int ID;
    char Name[21];
    char Password[21];
    int Score;
    int Rank;
};

// 排行榜数据信息
struct RankInfo {
    int Rank;      // 排名
    int Score;     // 得分
    char Name[21]; // 用户名
};

// 房间数据信息 
struct RoomInfo {
    int nID;        // 房间号
    int players;    // 该房间有几个人
    int bBegin;     // 表示该房间游戏是否开始，0:没开始可以加入,1:已经开始禁止进入 
    char Name[21];  // 房间名字
};




#endif









