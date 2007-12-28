using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Shelter;
using SmartTank.GameObjects;
using Microsoft.Xna.Framework;
using SmartTank.PhiCol;
using TankEngine2D.DataStructure;
using TankEngine2D.Helpers;
using SmartTank.Senses.Memory;
using TankEngine2D.Graphics;

namespace SmartTank.Senses.Vision
{
    public delegate IEyeableInfo GetEyeableInfoHandler ( IRaderOwner raderOwner, IEyeableObj obj );

    public class VisionManager
    {
        #region Private type

        [Obsolete]
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

        struct BinGroupBeta
        {
            public IEnumerable<IRaderOwner> raderOwners;
            public IEnumerable<IEyeableObj> eyeableObjs;

            public BinGroupBeta ( IEnumerable<IRaderOwner> raderOwners, IEnumerable<IEyeableObj> eyeableObjs )
            {
                this.raderOwners = raderOwners;
                this.eyeableObjs = eyeableObjs;
            }
        }

        #endregion

        #region Variables

        [Obsolete]
        List<BinGroup> groups = new List<BinGroup>();

        List<BinGroupBeta> groupsBeta = new List<BinGroupBeta>();

        #endregion

        #region Public Methods

        [Obsolete( "�޸���IEyeableObj�ӿڣ����ڽ���IEyeableObj����GetEyeableInfoHandler" )]
        public void AddVisionGroup ( IEnumerable<IRaderOwner> raderOwners, IEnumerable<KeyValuePair<IEyeableObj, GetEyeableInfoHandler>> set )
        {
            groups.Add( new BinGroup( raderOwners, set ) );
        }

        public void AddVisionGroup ( IEnumerable<IRaderOwner> raderOwners, IEnumerable<IEyeableObj> eyeableObjs )
        {
            groupsBeta.Add( new BinGroupBeta( raderOwners, eyeableObjs ) );
        }

        public void ClearGroups ()
        {
            groups.Clear();
            groupsBeta.Clear();
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

        [Obsolete]
        private static void CheckVisible ( BinGroup group, IRaderOwner raderOwner )
        {
            List<IEyeableInfo> inRaderObjInfos = new List<IEyeableInfo>();

            //List<IHasBorderObj> inRaderHasBorderNonShelterObjs = new List<IHasBorderObj>();

            List<EyeableBorderObjInfo> EyeableBorderObjs = new List<EyeableBorderObjInfo>();

            foreach (KeyValuePair<IEyeableObj, GetEyeableInfoHandler> set in group.eyeableSets)
            {
                if (raderOwner == set.Key)
                    continue;

                // ����Ƿ�Ϊ��ǰ�״���ڵ�����

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

                // �����ڵ������Ƿ�ɼ�
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

        /*
         * ����ΪCheckVisible���������δ���ԡ�
         * */
        private static void CheckVisibleBeta( BinGroupBeta group, IRaderOwner raderOwner )
        {
            List<IEyeableInfo> inRaderObjInfos = new List<IEyeableInfo>();

            List<EyeableBorderObjInfo> EyeableBorderObjs = new List<EyeableBorderObjInfo>();

            foreach (IEyeableObj obj in group.eyeableObjs)
            {
                if (raderOwner == obj)
                    continue;

                // ����Ƿ�Ϊ��ǰ�״���ڵ�����

                bool isShelter = false;
                foreach (ObjVisiBorder objBorder in raderOwner.Rader.ShelterVisiBorders)
                {
                    if (objBorder.Obj == obj)
                    {
                        IEyeableInfo eyeableInfo = obj.GetEyeableInfoHandler( raderOwner, obj );
                        inRaderObjInfos.Add( eyeableInfo );

                        EyeableBorderObjs.Add( new EyeableBorderObjInfo( eyeableInfo, objBorder ) );

                        isShelter = true;
                        break;
                    }
                }

                // �����ڵ������Ƿ�ɼ�
                if (!isShelter)
                {
                    foreach (Vector2 keyPoint in obj.KeyPoints)
                    {
                        if (raderOwner.Rader.PointInRader( Vector2.Transform( keyPoint, obj.TransMatrix ) ))
                        {
                            IEyeableInfo eyeableInfo = obj.GetEyeableInfoHandler( raderOwner, obj );
                            inRaderObjInfos.Add( eyeableInfo );

                            if (obj is IHasBorderObj)
                            {
                                ObjVisiBorder border = CalNonShelterVisiBorder( (IHasBorderObj)obj, raderOwner.Rader );
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

        private static ObjVisiBorder CalNonShelterVisiBorder( IHasBorderObj obj, Rader rader )
        {
            CircleListNode<BorderPoint> curNode = obj.BorderData.First;
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