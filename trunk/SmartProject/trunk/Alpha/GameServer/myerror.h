
void myError()
{
    
#ifdef WIN32
    perror("Socket Error.");
#else
    perror("Socket Error.");
#endif
}

