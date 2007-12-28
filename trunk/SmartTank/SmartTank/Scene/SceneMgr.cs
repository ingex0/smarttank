using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.GameObjects;

namespace SmartTank.Scene
{
    /*
     * 对场景路径的说明：
     * 
     * 若TankGroup是SceneMgr的直接子组之一。
     * 
     * 而有一个名为Tank3的物体位于TankGroup下AlliedGroup中，
     * 则该物体的路径为"TankGroup\\AlliedGroup\\Tank3"
     * 
     * AlliedGroup组的路径则为"TankGroup\\AlliedGroup"。
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
        /// 制定场景物体的存放路径而获得场景物体
        /// </summary>
        /// <param name="objPath">物体的场景路径</param>
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
