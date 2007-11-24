using System;
using System.Collections.Generic;
using System.Text;
using Platform.GameObjects;

namespace Platform.Scene
{
    public class SceneCommonObjInfo : IGameObjSceneInfo
    {
        public readonly bool isTankShelter;
        public readonly bool isTankObstacle;

        public SceneCommonObjInfo ( bool isTankShelter, bool isTankObstacle )
        {
            this.isTankShelter = isTankShelter;
            this.isTankObstacle = isTankObstacle;
        }
    }
}
