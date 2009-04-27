using System;
using System.Collections.Generic;
using System.Text;
using GameEngine.Shelter;
using GameEngine.PhiCol;
using GameEngine.Senses.Vision;
using Common.DataStructure;

namespace GameEngine.Senses.Memory
{
    /// <summary>
    /// 如果要在生成导航图的时候考虑该物体，返回true，否则返回false。
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public delegate bool NaviMapConsiderObj ( EyeableBorderObjInfo obj );

    public class ObjMemoryKeeper
    {
        Dictionary<IHasBorderObj, EyeableBorderObjInfo> memoryObjs;

        //NavigateMap naviMap;

        public ObjMemoryKeeper ()
        {
            memoryObjs = new Dictionary<IHasBorderObj, EyeableBorderObjInfo>();
        }

        public EyeableBorderObjInfo[] GetEyeableBorderObjInfos ()
        {
            EyeableBorderObjInfo[] result = new EyeableBorderObjInfo[memoryObjs.Values.Count];
            memoryObjs.Values.CopyTo( result, 0 );
            return result;
        }

        public NavigateMap CalNavigationMap ( NaviMapConsiderObj selectFun, Rectanglef mapBorder, float spaceForTank )
        {
            List<EyeableBorderObjInfo> eyeableInfos = new List<EyeableBorderObjInfo>();

            foreach (KeyValuePair<IHasBorderObj, EyeableBorderObjInfo> pair in memoryObjs)
            {
                if (selectFun( pair.Value ))
                {
                    eyeableInfos.Add( pair.Value );
                }
            }

            return new NavigateMap( eyeableInfos.ToArray(), mapBorder, spaceForTank );
        }

        //public NavigateMap NavigateMap
        //{
        //    get { return naviMap; }
        //}

        internal bool ApplyEyeableBorderObjInfo ( EyeableBorderObjInfo objInfo )
        {
            bool objUpdated = false;
            if (memoryObjs.ContainsKey( objInfo.Obj ))
            {
                if (memoryObjs[objInfo.Obj].Combine( objInfo ))
                    objUpdated = true;
            }
            else
            {
                memoryObjs.Add( objInfo.Obj, objInfo );
                objUpdated = true;
            }

            return objUpdated;
        }

        internal Dictionary<IHasBorderObj, EyeableBorderObjInfo> MemoryObjs
        {
            get { return memoryObjs; }
        }

        internal void HandlerObjDisappear ( List<IHasBorderObj> disappearObjs )
        {
            foreach (IHasBorderObj obj in disappearObjs)
            {
                memoryObjs[obj].SetIsDisappeared( true );
            }
        }
    }
}
