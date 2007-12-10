using System;
using System.Collections.Generic;
using System.Text;

namespace MapEditer
{
    class ObjClassInfo
    {
        public readonly string className;
        public readonly string classRemark;
        public readonly Type classType;

        public ObjClassInfo ( string className, string classRemark, Type classType )
        {
            this.className = className;
            this.classRemark = classRemark;
            this.classType = classType;
        }
    }
}
