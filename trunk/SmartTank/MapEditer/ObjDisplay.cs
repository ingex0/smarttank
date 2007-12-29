using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmartTank.GameObjs;

namespace MapEditer
{
    class ObjDisplay
    {
        string name;
        string objDataPath;
        Type objClassType;

        IGameObj example;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string ObjDataPath
        {
            get { return objDataPath; }
            set { objDataPath = value; }
        }

        public ObjDisplay ( string name, Type objClassType, string objDataPath )
        {
            this.name = name;
            this.objClassType = objClassType;
            this.objDataPath = objDataPath;

            example = CreateInstance();
        }

        public IGameObj CreateInstance ()
        {
            ConstructorInfo constructer = objClassType.GetConstructor( new Type[] { objDataPath.GetType() } );
            if (constructer == null)
            {
                throw new Exception( "��������������������ֻ��һ��ָ��GameObjData�ļ���·�������Ĺ��캯��" );
            }
            else
            {
                return (IGameObj)(constructer.Invoke( new object[] { objDataPath } ));
            }
        }

        public IGameObj CreateInstance ( Vector2 pos )
        {
            IGameObj result = CreateInstance();
            result.Pos = pos;
            return result;
        }

        public void DrawExampleAt ( Vector2 pos )
        {
            example.Pos = pos;
            example.Draw();
        }
    }
}
