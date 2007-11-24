using System;
using System.Collections.Generic;
using System.Text;

namespace Platform.GameObjects.Tank.TankAIs
{
    /*
     * ���ø����Ե�Ŀ������ʹ�Ͳ�AI���������߲�AI
     * 
     * ��ͬʱ��֤������ʹ�þ���������һ������£�
     * 
     * �߲�AIʹ�õ�OrderServer���ǴӵͲ�AIʹ�õ�OrderServer���м̳ж����ġ�
     * 
     * */

    [AttributeUsage( AttributeTargets.Interface, Inherited = false )]
    public class IAIAttribute : Attribute
    {
        public Type OrderServerType;
        public Type CommonServerType;

        public IAIAttribute ( Type orderServerType, Type commonServerType )
        {
            this.OrderServerType = orderServerType;
            this.CommonServerType = commonServerType;
        }
    }
}
