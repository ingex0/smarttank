#include "ClientManager.h"

extern pthread_mutex_t DD_ClientMgr_Mutex[MAXCLIENT];


ClientManager::ClientManager()
{

}

ClientManager::~ClientManager()
{
    vector<MySocket*>::iterator it = m_pClient.begin();
    for(; it != m_pClient.end(); it++)
    {
        delete (*it);
        *it = 0;
    }

}

//////////////////////////////////////////////////////////////////////////
// 记录服务器.返回socket指针
MySocket *ClientManager::AddServer( MySocket *psock)
{
    //#ifdef WIN32 
    //EnterCriticalSection(&DD_ClientMgr_Mutex);
    //#else
    //pthread_mutex_lock(&DD_ClientMgr_Mutex);
    //#endif

    if ( NULL == psock)
    {
        perror("AddServer");
    }
    m_pServer = psock;

//#ifdef WIN32 
//    LeaveCriticalSection(&DD_ClientMgr_Mutex);
//#else
//    pthread_mutex_unlock(&DD_ClientMgr_Mutex);
//#endif
    return m_pServer;
}


//////////////////////////////////////////////////////////////////////////
// 新用户加入
MySocket *ClientManager::AddClient( MySocket *psock)
{
    //LockThreadBegin
    cout << "AddClient." << endl;
    m_pClient.push_back(psock);
    cout << "AddClient success." << endl;
    //LockThreadEnd
    return NULL;
}

//////////////////////////////////////////////////////////////////////////
// 得到某用户.返回socket指针
MySocket *ClientManager::GetClient( int iID)
{
    if ( iID<0 || iID>GetNum())
    {
        perror("AddServer");
    }
    return m_pClient[iID];
}

//////////////////////////////////////////////////////////////////////////
// 删除用户
bool ClientManager::DelClient( MySocket *psock)
{
    //LockThreadBegin

    vector<MySocket*>::iterator it = m_pClient.begin();
    for(; it != m_pClient.end(); it++)
    {
        // Close客户套接字，释放资源
        if ( psock == *it)
        {
            cout << psock->GetIP() << "离开游戏.当前客户数:" << GetNum()-1 << endl;
            psock->Close();
            delete psock;
            *it = 0;
            m_pClient.erase(it);
            break;
        }
    }
    //LockThreadEnd
    return true;
}



