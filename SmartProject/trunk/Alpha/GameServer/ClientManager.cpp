#include "ClientManager.h"

extern pthread_mutex_t DD_ClientMgr_Mutex[MAXCLIENT];
extern int g_MyPort;
extern int g_MaxRoommate;
extern char g_mySqlUserlName[21];
extern char g_mySqlUserlPass[21];


ClientManager::ClientManager()
{

    // Ԥ����
    /**/for ( int i = 0; i < MAXCLIENT; i++)
    {
        MySocket* pSock = new MySocket;
        pSock->m_ID = -1;
        pSock->m_socket = -1;
        m_pEmpty.push_back( pSock );
    }
    m_numOfRoom = 0;
    m_room = new MyRoom;
    m_room->SetManager(this);
    //SetTimer(5);
}

ClientManager::~ClientManager()
{
    vector<MySocket*>::iterator it = m_pClient.begin();
    vector<MySocket*>::iterator itEnd = m_pClient.end();
    for(; it != itEnd; it++)
    {
        delete (*it);
        *it = NULL;
        itEnd = m_pClient.end();
    }
    /**/it = m_pEmpty.begin();
    itEnd = m_pEmpty.end();
    for(; it != itEnd; it++)
    {
        delete (*it);
        *it = NULL;
        itEnd = m_pEmpty.end();
    }
    delete m_room;
    m_room = NULL;
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
    cout << "AddClient --- ";
    int p = m_pEmpty.size();
    if ( p > 0 )
    {
        psock->m_ID = GetNum();
        int nID = psock->m_ID;
        LockThreadBegin

        MySocket *newSock = m_pEmpty[p-1];
        memcpy(newSock,psock,sizeof(MySocket));
        m_pClient.push_back( newSock );
        m_pClient.back()->m_isLiving = true;
        m_pEmpty.pop_back();

        LockThreadEnd
        cout << "Success." << endl;
        return  newSock;
    }
    else
    {
        cout << "�ﵽ��������������.." << endl;
        return NULL;
    }
    //LockThreadEnd
    
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
MySocket *ClientManager::GetSafeClient( int iID)
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
    if (NULL == psock)
        return true;
   int nID = psock->m_ID;
    LockThreadBegin
    vector<MySocket*>::iterator it = m_pClient.begin();
    for(; it != m_pClient.end(); it++)
    {
        // Close�ͻ��׽���
        if ( psock->m_ID == (*it)->m_ID)
        {  
            m_pEmpty.push_back( *it );
            m_pClient.erase(it);
            cout << psock->GetIP() << "�뿪��Ϸ.��ǰ�ͻ���:" << GetNum() << endl;
            m_room->RemovePlayer(psock);
            if( m_room->GetPlayers() == 0)
                m_numOfRoom = 0;
            psock->Close();
            // cout << "Delete socket success." << endl;
            break;
        }
    }
    LockThreadEnd

    return true;
}

/************************************************************************/
/* ɾ���û�                                                                     */
/************************************************************************/
bool ClientManager::DelClientAll( )
{
    for (int i = 0; i < (int)m_pClient.size(); i++)
    {
        DelClient(m_pClient[i]);
    }
    return true;
}
/************************************************************************/
/* ��ʱ��                                                                     */
/************************************************************************/
void ClientManager::SetTimer(int t )
{
#ifdef WIN32

    //::SetTimer(NULL, nID, 10000, HeartBeatFun);
#else
    struct itimerval itv, oldtv;
    itv.it_interval.tv_sec = t;
    itv.it_interval.tv_usec = 0;
    itv.it_value.tv_sec = t;
    itv.it_value.tv_usec = 0;

    setitimer(ITIMER_REAL, &itv, &oldtv);
#endif

}

void ClientManager::HeartBeatFun(MySocket *psock)
{
    psock->m_isLiving = true;
    cout << psock->m_name << " is Living..." << endl;
}




