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
// ��¼������.����socketָ��
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
// ���û�����
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
// �õ�ĳ�û�.����socketָ��
MySocket *ClientManager::GetClient( int iID)
{
    if ( iID<0 || iID>GetNum())
    {
        perror("AddServer");
    }
    return m_pClient[iID];
}

//////////////////////////////////////////////////////////////////////////
// ɾ���û�
bool ClientManager::DelClient( MySocket *psock)
{
    //LockThreadBegin

    vector<MySocket*>::iterator it = m_pClient.begin();
    for(; it != m_pClient.end(); it++)
    {
        // Close�ͻ��׽��֣��ͷ���Դ
        if ( psock == *it)
        {
            cout << psock->GetIP() << "�뿪��Ϸ.��ǰ�ͻ���:" << GetNum()-1 << endl;
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



