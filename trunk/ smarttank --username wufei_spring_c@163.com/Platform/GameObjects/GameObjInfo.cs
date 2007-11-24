using System;
using System.Collections.Generic;
using System.Text;

namespace Platform.GameObjects
{
    [Serializable]
    public class GameObjInfo
    {
        public readonly string Name;
        public readonly string Script;

        IGameObjSceneInfo sceneInfo;

        /// <summary>
        /// 全局物体ID，由场景管理类分配
        /// </summary>
        int id;

        public GameObjInfo ( string name, string script )
        {
            this.Name = name;
            this.Script = script;
            this.id = -1;
        }

        public IGameObjSceneInfo SceneInfo
        {
            get { return sceneInfo; }
        }

        public int ID
        {
            get { return id; }
        }

        internal void SetID ( int id )
        {
            this.id = id;
        }

        internal void SetSceneInfo ( IGameObjSceneInfo sceneInfo )
        {
            this.sceneInfo = sceneInfo;
        }
    }
}
