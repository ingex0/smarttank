using System;
using System.Collections.Generic;
using System.Text;
using Platform.Shelter;
using Platform.PhisicalCollision;
using Platform.Senses.Vision;
using GameBase.DataStructure;

namespace Platform.Senses.Memory
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

        internal void ApplyEyeableBorderObjInfo ( EyeableBorderObjInfo objInfo )
        {
            if (memoryObjs.ContainsKey( objInfo.Obj ))
            {
                memoryObjs[objInfo.Obj].Combine( objInfo );
            }
            else
            {
                memoryObjs.Add( objInfo.Obj, objInfo );
            }
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
