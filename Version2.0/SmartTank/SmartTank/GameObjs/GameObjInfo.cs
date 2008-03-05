using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.GameObjs
{
    [Serializable]
    public class GameObjInfo
    {
        public readonly string ObjClass;
        public readonly string Script;

        public GameObjInfo( string objClass, string script )
        {
            this.ObjClass = objClass;
            this.Script = script;
        }
    }
}
