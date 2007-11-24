using System;
using System.Collections.Generic;
using System.Text;
using Platform.Shelter;
using Platform.PhisicalCollision;
using Platform.Senses.Vision;

namespace Platform.Senses.Memory
{
    public class ObjMemoryKeeper
    {
        Dictionary<IHasBorderObj, EyeableBorderObjInfo> memoryObjs;

        NavigateMap naviMap;

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

        public void UpdateNavigationMap ( float spaceForTank )
        {
            EyeableBorderObjInfo[] eyeableInfos = GetEyeableBorderObjInfos();

            if (naviMap == null)
                naviMap = new NavigateMap( eyeableInfos, spaceForTank );
            else
                naviMap.BuildMap( eyeableInfos, spaceForTank );
        }

        public NavigateMap NavigateMap
        {
            get { return naviMap; }
        }

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
