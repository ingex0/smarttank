
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.net
{
    static public class InfoRePath
    {
        

        static public void CallEvent(MulticastDelegate delgt, params object[] evenetParams)
        {
            if (PurviewMgr.IsMainHost)
                delgt.DynamicInvoke(evenetParams);
            else
            {
                // TODO ͨ������Э�鴫�ݸ�����
            }
        }
    }
}
