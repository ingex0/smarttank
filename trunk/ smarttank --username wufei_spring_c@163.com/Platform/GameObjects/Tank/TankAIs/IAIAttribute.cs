using System;
using System.Collections.Generic;
using System.Text;

namespace Platform.GameObjects.Tank.TankAIs
{
    /*
     * 设置该属性的目的在于使低层AI可以用作高层AI
     * 
     * 但同时保证这样的使用局限在这样一种情况下：
     * 
     * 高层AI使用的OrderServer类是从低层AI使用的OrderServer类中继承而来的。
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
