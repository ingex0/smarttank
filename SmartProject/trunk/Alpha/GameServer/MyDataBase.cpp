#include "MyDataBase.h"
extern int g_MyPort;
extern int g_MaxRoommate;
extern char g_mySqlUserlName[21];
extern char g_mySqlUserlPass[21];
extern char g_mySqlDatabase[128];
Team5_DB::Team5_DB()
{
    cout << "读取配置文件..." << endl;
    ifstream inFile;
    inFile.open("config.txt");
    char buff[255];
    char seps[] = "=";
    char *token;

    inFile.getline(buff, 255);
    token = strtok( buff, seps );
    token = strtok( NULL, seps );
    g_MyPort = atoi(token); 
    cout << "服务器端口号为:" << g_MyPort << endl;

    inFile.getline(buff, 255);
    token = strtok( buff, seps );
    token = strtok( NULL, seps );
    strcpy(g_mySqlUserlName, token); 
    cout << "MySql服务器用户名为:" << g_mySqlUserlName << endl;

    inFile.getline(buff, 255);
    token = strtok( buff, seps );
    token = strtok( NULL, seps );
    strcpy(g_mySqlUserlPass, token); 
    cout << "MySql服务器密码为:" << g_mySqlUserlPass << endl;

    inFile.getline(buff, 255);
    token = strtok( buff, seps );
    token = strtok( NULL, seps );
    strcpy(g_mySqlDatabase, token); 
    cout << "MySql数据库为:" << g_mySqlDatabase << endl;

    inFile.getline(buff, 255);
    token = strtok( buff, seps );
    token = strtok( NULL, seps );
    g_MaxRoommate = atoi(token); 
    cout << "房间人数上限:" << g_MaxRoommate << endl;
    inFile.close();
    Connect();
}
int Team5_DB::Connect()
{
    mysql = mysql_init(NULL);
#ifndef WIN32
    g_mySqlUserlName[strlen(g_mySqlUserlName)-1] = '\0';
    g_mySqlUserlPass[strlen(g_mySqlUserlPass)-1] = '\0';
    g_mySqlDatabase[strlen(g_mySqlDatabase)-1] = '\0';
#endif
    if ( 0 == strcmp(g_mySqlUserlPass, "NULL") )
    {
        if (!mysql_real_connect(mysql, NULL, "root", 0,  NULL, 0, NULL, 0))
        {
            printf(mysql_error(mysql));
            cout << "连接数据库失败." << endl;
            return -1;	
        }
    }
    else
    {
        if (!mysql_real_connect(mysql, NULL, g_mySqlUserlName, g_mySqlUserlPass, NULL, 0, NULL, 0))
        {
            cout << "连接数据库失败." << endl;
            printf(mysql_error(mysql));
            return -1;	
        }
    }
    
    if (mysql_select_db(mysql, g_mySqlDatabase))
    {
        perror("connect sql");
        return -1;
    }
    return 0;
}

void Team5_DB::Disconnect()
{
    mysql_close(mysql);
    mysql_library_end();
}

int Team5_DB::Reset()
{
    mysql_query(mysql, "DROP TABLE UserData");
    if (mysql_query(mysql, "CREATE TABLE UserData (ID BIGINT(12) NOT NULL AUTO_INCREMENT PRIMARY KEY, Name char(50) unique, Password char(50), Score int) ENGINE=INNODB AUTO_INCREMENT = 0"))
    {
        printf("Reset Error\n");
        return -1;
    }
    return 0;
}

int Team5_DB::AddUserItem(char *Name, char *Password, int StartScore)
{
    sprintf(buffer, "INSERT INTO UserData (Name, Password, Score) VALUES ('%s', '%s', %d)", Name, Password, StartScore);
    if (mysql_query(mysql, buffer))
    {
        printf("AddUserItem Error\n");
        return -1;
    }
    return 0;
}

int Team5_DB::DeleteUserItem(char *Name)
{
    sprintf(buffer, "DELETE FROM UserData WHERE Name = '%s'", Name);
    if (mysql_query(mysql, buffer))
    {
        printf("DeleteUserItem Error\n");
        return -1;
    }
    return 0;
}

int Team5_DB::SetUserName(char *Name, char *NewName)
{
    sprintf(buffer, "UPDATE UserData SET Name = '%s' WHERE Name = '%s'", NewName, Name);
    if (mysql_query(mysql, buffer))
    {
        printf("SetUserName Error\n");
        return -1;
    }
    return 0;
}

int Team5_DB::SetUserPassword(char *Name, char *NewPassword)
{
    sprintf(buffer, "UPDATE UserData SET Password = '%s' WHERE Name = '%s'", NewPassword, Name);
    if (mysql_query(mysql, buffer))
    {
        printf("SetUserPassword Error\n");
        return -1;
    }
    return 0;
}

int Team5_DB::SetUserScore(char *Name, int Score)
{
    sprintf(buffer, "UPDATE UserData SET Score = %d WHERE Name = '%s'", Score, Name);
    if (mysql_query(mysql, buffer))
    {
        printf("SetUserScore Error\n");
        return -1;
    }
    return 0;
}

int Team5_DB::GetUserPassword(char *Name, char *RetPassword)
{
    sprintf(buffer, "select Password from UserData where Name = '%s'", Name);
    if (mysql_query(mysql, buffer))
    {
        printf("User is not exist...\n");
        return -1;
    }

    result = mysql_use_result(mysql);
    if (!result)
    {
        printf("GetUserPassword Error\n");
        return -1;
    }

    row = mysql_fetch_row(result) ;
    if (!row)
    {
        printf("GetUserPassword Error\n");
        return -1;
    }

    strcpy(RetPassword, row[0]);
    mysql_free_result(result);
    return 0;
}

int Team5_DB::GetUserScore(char *Name)
{
    int score;

    sprintf(buffer, "select Score from UserData where Name = '%s'", Name);
    if (mysql_query(mysql, buffer))
    {
        printf("User is not exist...\n");
        return -1;
    }

    result = mysql_use_result(mysql);
    if (!result)
    {
        printf("GetUserScore Error\n");
        return -1;
    }

    row = mysql_fetch_row(result) ;
    if (!row)
    {
        printf("GetUserScore Error\n");
        return -1;
    }

    score = atoi(row[0]);
    mysql_free_result(result);
    return score;
}

int Team5_DB::GetUserID(char *Name)
{
    int id;

    sprintf(buffer, "select ID from UserData where Name = '%s'", Name);
    if (mysql_query(mysql, buffer))
    {
        printf("User is not exist...\n");
        return -1;
    }

    result = mysql_use_result(mysql);
    if (!result)
    {
        printf("GetUserID Error\n");
        return -1;
    }

    row = mysql_fetch_row(result) ;
    if (!row)
    {
        printf("GetUserID Error\n");
        return -1;
    }

    id = atoi(row[0]);
    mysql_free_result(result);
    return id;
}

int Team5_DB::GetUserCount()
{
    int count;

    if (mysql_query(mysql, "SELECT COUNT(*) FROM UserData"))
    {
        printf("GetUserCount Error\n");
        return -1;
    }

    result = mysql_use_result(mysql);
    if (!result)
    {
        printf("GetUserCount Error\n");
        return -1;
    }

    row = mysql_fetch_row(result) ;
    if (!row)
    {
        printf("GetUserCount Error\n");
        return -1;
    }

    count = atoi(row[0]);
    mysql_free_result(result);
    return count;
}

int Team5_DB::GetRankList(int m, int n, RankInfo *RetPage)
{
    int count = 0;

    sprintf(buffer, "SELECT Name, Score FROM UserData ORDER BY Score DESC limit %d,%d", m, n);
    if (mysql_query(mysql, buffer))
    {
        printf("GetRankList Error\n");
        return -1;
    }

    result = mysql_use_result(mysql);
    if (!result)
    {
        printf("GetRankList Error\n");
        return -1;
    }

    row = mysql_fetch_row(result);

    for (int r = m + count + 1; row != NULL; ++count, ++r)
    {
        strcpy(RetPage[count].Name, (char *)row[0]);
        RetPage[count].Score = atoi(row[1]);
        if (count != 0 && RetPage[count].Score == RetPage[count - 1].Score)
        {
            RetPage[count].Rank = RetPage[count - 1].Rank;
        }
        else
        {
            RetPage[count].Rank = r;
        }
        row = mysql_fetch_row(result);
    }

    mysql_free_result(result);
    return count;
}

int Team5_DB::GetUserList(int m, int n, UserInfo *RetPage)
{
    int count = 0;

    sprintf(buffer, "SELECT ID, Name, Password, Score FROM UserData limit %d, %d", m, n);
    if (mysql_query(mysql, buffer))
    {
        printf("GetUserList Error\n");
        return -1;
    }

    result = mysql_use_result(mysql);
    if (!result)
    {
        printf("GetUserList Error\n");
        return -1;
    }

    row = mysql_fetch_row(result);

    for (; row != NULL; ++count)
    {
        RetPage[count].ID = atoi(row[0]);
        strcpy(RetPage[count].Name, (char *)row[1]);
        strcpy(RetPage[count].Password, (char *)row[2]);
        RetPage[count].Score = atoi(row[3]);
        row = mysql_fetch_row(result);
    }

    mysql_free_result(result);

    for (int i = 0; i < count; i++)
    {
        RetPage[i].Rank = GetUserRank(RetPage[i].Name);
        if (RetPage[i].Rank < 0)
        {
            printf("GetUserList Error\n");
            return -1;
        }
    }

    return count;
}

int Team5_DB::GetUserRank(char *Name)
{
    int Rank;

    sprintf(buffer, "select count(*) from UserData where Score > (select Score from UserData where Name = '%s')", Name);

    if (mysql_query(mysql, buffer))
    {
        printf("GetUserRank Error\n");
        return -1;
    }

    result = mysql_use_result(mysql);
    if (!result)
    {
        printf("GetUserRank Error\n");
        return -1;
    }

    row = mysql_fetch_row(result) ;
    if (!row)
    {
        printf("GetUserRank Error\n");
        return -1;
    }

    Rank = atoi(row[0]) + 1;
    mysql_free_result(result);
    return Rank;
}

int Team5_DB::GetUserInfo(char *Name, UserInfo &RetInfo)
{
    sprintf(buffer, "select ID, Password, Score from UserData where Name = '%s'", Name);
    if (mysql_query(mysql, buffer))
    {
        printf("User is not exist...\n");
        return -1;
    }

    result = mysql_use_result(mysql);
    if (!result)
    {
        printf("GetUserInfo Error\n");
        return -1;
    }

    row = mysql_fetch_row(result) ;
    if (!row)
    {
        printf("GetUserInfo Error\n");
        return -1;
    }

    RetInfo.ID = atoi(row[0]);
    if (RetInfo.ID < 0)
    {
        printf("GetUserInfo Error\n");
        return -1;
    }

    strcpy(RetInfo.Password, row[1]);

    RetInfo.Score = atoi(row[2]);
    if (RetInfo.Score < 0)
    {
        printf("GetUserInfo Error\n");
        return -1;
    }

    mysql_free_result(result);

    RetInfo.Rank = GetUserRank(Name);
    if (RetInfo.Rank < 0)
    {
        printf("GetUserInfo Error\n");
        return -1;
    }

    if (Name != RetInfo.Name)
    {
        strcpy(RetInfo.Name, Name);
    }

    return 0;
}






