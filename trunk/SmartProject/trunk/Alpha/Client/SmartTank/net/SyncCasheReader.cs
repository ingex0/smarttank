using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.net
{
    static class SyncCasheReader
    {

        static SyncCashe inputCashe;
        static public SyncCashe InputCashe
        {
            get { return inputCashe; }
            set { inputCashe = value; }
        }

    }
}
