using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.GameObjects;

namespace SmartTank.Scene
{
    /*
     * �Գ���·����˵����
     * 
     * ��TankGroup��SceneMgr��ֱ������֮һ��
     * 
     * ����һ����ΪTank3������λ��TankGroup��AlliedGroup�У�
     * ��������·��Ϊ"TankGroup\\AlliedGroup\\Tank3"
     * 
     * AlliedGroup���·����Ϊ"TankGroup\\AlliedGroup"��
     * 
     * */

    public class SceneMgr
    {
        protected Group groups;


        public bool Initialize ( string sceneFilePath )
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupPath"></param>
        /// <returns></returns>
        public Group GetGroup ( string groupPath )
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="groupPath"></param>
        /// <returns></returns>
        public TypeGroup<T> GetTypeGroup<T> ( string groupPath ) where T : class, IGameObj
        {
            return null;
        }

        /// <summary>
        /// �ƶ���������Ĵ��·������ó�������
        /// </summary>
        /// <param name="objPath">����ĳ���·��</param>
        /// <returns></returns>
        public IGameObj GetGameObj ( string objPath )
        {
            return null;
        }

        public bool AddGroup ( string fatherPath, Group group )
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullGroupPath"></param>
        /// <returns></returns>
        public Group AddGroup ( string groupPath )
        {
            return null;
        }

        public bool AddGameObj ( string groupPath, IGameObj obj )
        {
            return true;
        }

        public bool DelGameObj ( string objPath )
        {
            return true;
        }
    }
}
