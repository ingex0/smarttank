using System;
using System.Collections.Generic;
using System.Text;
using Platform.Shelter;
using Platform.GameObjects;
using Microsoft.Xna.Framework;
using Platform.PhisicalCollision;
using GameBase.DataStructure;
using GameBase.Helpers;
using GameBase;
using Platform.Senses.Memory;

namespace Platform.Senses.Vision
{
    public delegate IEyeableInfo GetEyeableInfoHandler ( IRaderOwner raderOwner, IEyeableObj obj );

    public class VisionManager
    {
        #region Private type

        struct BinGroup
        {
            public IEnumerable<IRaderOwner> raderOwners;
            public IEnumerable<KeyValuePair<IEyeableObj, GetEyeableInfoHandler>> eyeableSets;
            public BinGroup ( IEnumerable<IRaderOwner> raderOwners, IEnumerable<KeyValuePair<IEyeableObj, GetEyeableInfoHandler>> eyeableSets )
            {
                this.raderOwners = raderOwners;
                this.eyeableSets = eyeableSets;
            }
        }

        #endregion

        #region Variables

        List<BinGroup> groups = new List<BinGroup>();

        #endregion

        #region Public Methods

        public void AddVisionGroup ( IEnumerable<IRaderOwner> raderOwners, IEnumerable<KeyValuePair<IEyeableObj, GetEyeableInfoHandler>> set )
        {
            groups.Add( new BinGroup( raderOwners, set ) );
        }

        public void ClearGroups ()
        {
            groups.Clear();
        }

        #endregion

        #region Update

        public void Update ()
        {
            foreach (BinGroup group in groups)
            {
                foreach (IRaderOwner raderOwner in group.raderOwners)
                {
                    CheckVisible( group, raderOwner );
                }
            }

        }

        private static void CheckVisible ( BinGroup group, IRaderOwner raderOwner )
        {
            List<IEyeableInfo> inRaderObjInfos = new List<IEyeableInfo>();

            //List<IHasBorderObj> inRaderHasBorderNonShelterObjs = new List<IHasBorderObj>();

            List<EyeableBorderObjInfo> EyeableBorderObjs = new List<EyeableBorderObjInfo>();

            foreach (KeyValuePair<IEyeableObj, GetEyeableInfoHandler> set in group.eyeableSets)
            {
                if (raderOwner == set.Key)
                    continue;

                // 检查是否为当前雷达的遮挡物体

                bool isShelter = false;
                foreach (ObjVisiBorder objBorder in raderOwner.Rader.ShelterVisiBorders)
                {
                    if (objBorder.Obj == set.Key)
                    {
                        IEyeableInfo eyeableInfo = set.Value( raderOwner, set.Key );
                        inRaderObjInfos.Add( eyeableInfo );

                        EyeableBorderObjs.Add( new EyeableBorderObjInfo( eyeableInfo, objBorder ) );

                        isShelter = true;
                        break;
                    }
                }

                // 检查非遮挡物体是否可见
                if (!isShelter)
                {
                    foreach (Vector2 keyPoint in set.Key.KeyPoints)
                    {
                        if (raderOwner.Rader.PointInRader( Vector2.Transform( keyPoint, set.Key.TransMatrix ) ))
                        {
                            IEyeableInfo eyeableInfo = set.Value( raderOwner, set.Key );
                            inRaderObjInfos.Add( eyeableInfo );

                            if (set.Key is IHasBorderObj)
                            {
                                ObjVisiBorder border = CalNonShelterVisiBorder( (IHasBorderObj)set.Key, raderOwner.Rader );
                                if (border != null)
                                    EyeableBorderObjs.Add( new EyeableBorderObjInfo( eyeableInfo, border ) );
                            }
                            break;
                        }
                    }
                }
            }

            raderOwner.Rader.CurEyeableObjs = inRaderObjInfos;

            raderOwner.Rader.EyeableBorderObjInfos = EyeableBorderObjs.ToArray();



        }

        private static ObjVisiBorder CalNonShelterVisiBorder ( IHasBorderObj obj, Rader rader )
        {
            CircleListNode<GameBase.Graphics.Border> curNode = obj.BorderData.First;
            CircleList<BordPoint> points = new CircleList<BordPoint>();
            for (int i = 0; i < obj.BorderData.Length; i++)
            {
                if (rader.PointInRader( Vector2.Transform( ConvertHelper.PointToVector2( curNode.value.p ), obj.WorldTrans ) ))
                {
                    points.AddLast( new BordPoint( i, curNode.value.p ) );
                }
                curNode = curNode.next;
            }
            if (points.Length != 0)
                return new ObjVisiBorder( obj, points );
            else
                return null;
        }

        #endregion

    }
}
