/*
#pragma comment(lib, "D:\\Program Files\\MySQL\\MySQL Server 5.1\\lib\\opt\\libmysql.lib")
#pragma comment(lib, "D:\\Program Files\\MySQL\\MySQL Server 5.1\\lib\\opt\\mysqlclient.lib")
#pragma comment(lib, "D:\\Program Files\\MySQL\\MySQL Server 5.1\\lib\\opt\\mysys.lib")
#pragma comment(lib, "D:\\Program Files\\MySQL\\MySQL Server 5.1\\lib\\opt\\regex.lib")
#pragma comment(lib, "D:\\Program Files\\MySQL\\MySQL Server 5.1\\lib\\opt\\strings.lib")
#pragma comment(lib, "D:\\Program Files\\MySQL\\MySQL Server 5.1\\lib\\opt\\zlib.lib")
#include <windows.h>
*/
#include <cstdlib>
#include <mysql.h>

struct RankInfo {
	int Rank;
	char Name[21];
	int Score;
};

struct UserInfo {
	int ID;
	char Name[21];
	char Password[21];
	int Score;
	int Rank;
};

class Team5_DB {
private:
	MYSQL *mysql;
	MYSQL_RES *result;
	MYSQL_ROW row;
	char buffer[256];
public:
	//功  能: 连接数据库
	//返回值: 0 -------------正常结束
	//        -1 ------------异常
	int Connect();

	//功  能: 断开与数据库的连接
	void Disconnect();

	//功  能: 添加一条用户信息
	//参  数: Name ----------用户名
	//        Password-------密码
	//        StartScore-----初始积分
	//返回值: 0 -------------正常结束
	//        -1 ------------异常
	int AddUserItem(char *Name, char *Password, int StartScore);

	//功  能: 删除一条用户信息
	//参  数: Name ----------用户名
	//返回值: 0 -------------正常结束
	//        -1 ------------异常
	int DeleteUserItem(char *Name);

	//功  能: 更改用户名
	//参  数: Name ----------用户名
	//        NewName--------新用户名
	//返回值: 0 -------------正常结束
	//        -1 ------------异常
	int SetUserName(char *Name, char *NewName);

	//功  能: 更改用户密码
	//参  数: Name ----------用户名
	//        NewPassword----新密码
	//返回值: 0 -------------正常结束
	//        -1 ------------异常
	int SetUserPassword(char *Name, char *NewPassword);

	//功  能: 更改用户积分
	//参  数: Name ----------用户名
	//        Score----------积分
	//返回值: 0 -------------正常结束
	//        -1 ------------异常
	int SetUserScore(char *Name, int Score);

	//功  能: 获得用户ID
	//参  数: Name ----------用户名
	//返回值: >=0 -----------用户ID
	//        -1 ------------异常
	int GetUserID(char *Name);

	//功  能: 获得用户密码
	//参  数: Name ----------用户名
	//        RetPassword----返回的密码写入 RetPassword 所指向的空间
	//返回值: 0 -------------正常结束
	//        -1 ------------异常
	int GetUserPassword(char *Name, char *RetPassword);

	//功  能: 获得用户积分
	//参  数: Name ----------用户名
	//返回值: >=0 -----------用户的积分
	//        -1 ------------异常
	int GetUserScore(char *Name);

	//功  能: 获得用户排名
	//参  数: Name ----------用户名
	//返回值: >=0 -----------用户的排名
	//        -1 ------------异常
	int GetUserRank(char *Name);

	//功  能: 获得用户完整信息
	//参  数: Name ----------用户名
	//        RetInfo -------返回的用户信息写入 RetInfo 中
	//返回值: 0 -------------正常结束
	//        -1 ------------异常
	int GetUserInfo(char *Name, UserInfo &RetInfo);

	//功  能: 获得用户总数
	//参  数: Name ----------用户名
	//返回值: >=0 -----------用户总数
	//        -1 ------------异常
	int GetUserCount();

	//功  能: 获得积分排名列表
	//参  数: m -------------从第 m + 1 名开始
	//        n ------------- n 个用户
	//        RetPage -------将排名在 [m + 1, m + n] 区间内的 RankInfo 写入 RetPage
	//返回值: >=0 -----------实际获得的 RankInfo 数量
	//        -1 ------------异常
	int GetRankList(int m, int n, RankInfo *RetPage);
	

	//功  能: 获得用户数据列表（按注册先后顺序）
	//参  数: m -------------从第 m + 1 个开始
	//        n ------------- n 个用户
	//        RetPage -------将注册顺序在 [m + 1, m + n] 区间内的 UserInfo 写入 RetPage
	//返回值: >=0 -----------实际获得的 UserInfo 数量
	//        -1 ------------异常
	int GetUserList(int m, int n, UserInfo *RetPage);

	//功  能: 数据库复位
	//返回值: 0 -------------正常结束
	//        -1 ------------异常
	int Reset();
};
