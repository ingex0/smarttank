using System;
using System.Collections.Generic;
using System.Text;
using GameEngine.Shelter;
using Microsoft.Xna.Framework;
using GameEngine.PhiCol;
using GameEngine.Senses.Memory;
using GameEngine.Graphics;
using Common.DataStructure;
using Common.Helpers;

namespace GameEngine.Senses.Vision
{
    public delegate IEyeableInfo GetEyeableInfoHandler( IRaderOwner raderOwner, IEyeableObj obj );

    public class VisionMgr
    {
        #region Private type

        //[Obsolete]
        //struct BinGroup
        //{
        //    public IEnumerable<IRaderOwner> raderOwners;
        //    public IEnumerable<KeyValuePair<IEyeableObj, GetEyeableInfoHandler>> eyeableSets;
        //    public BinGroup ( IEnumerable<IRaderOwner> raderOwners, IEnumerable<KeyValuePair<IEyeableObj, GetEyeableInfoHandler>> eyeableSets )
        //    {
        //        this.raderOwners = raderOwners;
        //        this.eyeableSets = eyeableSets;
        //    }
        //}

        struct BinGroup
        {
            public IEnumerable<IRaderOwner> raderOwners;
            public IEnumerable<IEyeableObj>[] eyeableObjs;

            public BinGroup( IEnumerable<IRaderOwner> raderOwners, IEnumerable<IEyeableObj>[] eyeableObjs )
            {
                this.raderOwners = raderOwners;
                this.eyeableObjs = eyeableObjs;
            }
        }

        #endregion

        #region Variables

        //[Obsolete]
        //List<BinGroup> groups = new List<BinGroup>();

        List<BinGroup> groups = new List<BinGroup>();

        #endregion

        #region Public Methods

        //[Obsolete( "�޸���IEyeableObj�ӿڣ����ڽ���IEyeableObj����GetEyeableInfoHandler" )]
        //public void AddVisionGroup ( IEnumerable<IRaderOwner> raderOwners, IEnumerable<KeyValuePair<IEyeableObj, GetEyeableInfoHandler>> set )
        //{
        //    groups.Add( new BinGroup( raderOwners, set ) );
        //}

        public void AddVisionGroup( IEnumerable<IRaderOwner> raderOwners, IEnumerable<IEyeableObj>[] eyeableObjs )
        {
            groups.Add( new BinGroup( raderOwners, eyeableObjs ) );
        }

        public void ClearGroups()
        {
            //groups.Clear();
            groups.Clear();
        }

        #endregion

        #region Update

        public void Update()
        {
            foreach (BinGroup group in groups)
            {
                foreach (IRaderOwner raderOwner in group.raderOwners)
                {
                    CheckVisible( group, raderOwner );
                }
            }

        }

        //[Obsolete]
        //private static void CheckVisible ( BinGroup group, IRaderOwner raderOwner )
        //{
        //    List<IEyeableInfo> inRaderObjInfos = new List<IEyeableInfo>();

        //    //List<IHasBorderObj> inRaderHasBorderNonShelterObjs = new List<IHasBorderObj>();

        //    List<EyeableBorderObjInfo> EyeableBorderObjs = new List<EyeableBorderObjInfo>();

        //    foreach (KeyValuePair<IEyeableObj, GetEyeableInfoHandler> set in group.eyeableSets)
        //    {
        //        if (raderOwner == set.Key)
        //            continue;

        //        // ����Ƿ�Ϊ��ǰ�״���ڵ�����

        //        bool isShelter = false;
        //        foreach (ObjVisiBorder objBorder in raderOwner.Rader.ShelterVisiBorders)
        //        {
        //            if (objBorder.Obj == set.Key)
        //            {
        //                IEyeableInfo eyeableInfo = set.Value( raderOwner, set.Key );
        //                inRaderObjInfos.Add( eyeableInfo );

        //                EyeableBorderObjs.Add( new EyeableBorderObjInfo( eyeableInfo, objBorder ) );

        //                isShelter = true;
        //                break;
        //            }
        //        }

        //        // �����ڵ������Ƿ�ɼ�
        //        if (!isShelter)
        //        {
        //            foreach (Vector2 keyPoint in set.Key.KeyPoints)
        //            {
        //                if (raderOwner.Rader.PointInRader( Vector2.Transform( keyPoint, set.Key.TransMatrix ) ))
        //                {
        //                    IEyeableInfo eyeableInfo = set.Value( raderOwner, set.Key );
        //                    inRaderObjInfos.Add( eyeableInfo );

        //                    if (set.Key is IHasBorderObj)
        //                    {
        //                        ObjVisiBorder border = CalNonShelterVisiBorder( (IHasBorderObj)set.Key, raderOwner.Rader );
        //                        if (border != null)
        //                            EyeableBorderObjs.Add( new EyeableBorderObjInfo( eyeableInfo, border ) );
        //                    }
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    raderOwner.Rader.CurEyeableObjs = inRaderObjInfos;
        //    raderOwner.Rader.EyeableBorderObjInfos = EyeableBorderObjs.ToArray();
        //}

        /*
         * ����ΪCheckVisible���������δ���ԡ�
         * */
        private static void CheckVisible( BinGroup group, IRaderOwner raderOwner )
        {
            List<IEyeableInfo> inRaderObjInfos = new List<IEyeableInfo>();

            List<EyeableBorderObjInfo> EyeableBorderObjs = new List<EyeableBorderObjInfo>();

            foreach (IEnumerable<IEyeableObj> eyeGroup in group.eyeableObjs)
            {
                foreach (IEyeableObj obj in eyeGroup)
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
            }

            raderOwner.Rader.CurEyeableObjs = inRaderObjInfos;
            raderOwner.Rader.EyeableBorderObjInfos = EyeableBorderObjs.ToArray();
        }

        private static ObjVisiBorder CalNonShelterVisiBorder( IHasBorderObj obj, Rader rader )
        {
            CircleListNode<BorderPoint> curNode = obj.BorderData.First;
            CircleList<VisiBordPoint> points = new CircleList<VisiBordPoint>();
            for (int i = 0; i < obj.BorderData.Length; i++)
            {
                if (rader.PointInRader( Vector2.Transform( ConvertHelper.PointToVector2( curNode.value.p ), obj.WorldTrans ) ))
                {
                    points.AddLast( new VisiBordPoint( i, curNode.value.p ) );
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
