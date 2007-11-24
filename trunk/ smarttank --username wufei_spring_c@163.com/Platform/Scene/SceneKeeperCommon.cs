using System;
using System.Collections.Generic;
using System.Text;
using Platform.GameObjects;
using GameBase.DataStructure;
using Platform.PhisicalCollision;
using Platform.Shelter;
using Platform.GameDraw;
using Platform.Update;
using Platform.Senses.Vision;
using EyeableSet = System.Collections.Generic.KeyValuePair<Platform.Senses.Vision.IEyeableObj, Platform.Senses.Vision.GetEyeableInfoHandler>;

namespace Platform.Scene
{
    /*
     * һ���������ݴ����ࡣ
     * 
     * ���廯��ײ��ϵ���ڵ���ϵ������һ�������
     * 
     * */
    public class SceneKeeperCommon : ISceneKeeper
    {
        #region Keepers

        /// <summary>
        /// �������弯������ͳһ����Updata�����ͻ��ƺ�����
        /// </summary>
        MultiLinkedList<IGameObj> SenceObjs = new MultiLinkedList<IGameObj>();

        /// <summary>
        /// ����������弯������������º���ײ�����PhiColManager����
        /// </summary>
        MultiLinkedList<IPhisicalObj> phisicalObjs = new MultiLinkedList<IPhisicalObj>();

        /*
         * �ڵ���ϵ���棬����ֻ����һ�����͵��״��̹���״
         * 
         * */

        /// <summary>
        /// ̹���״�������߼���
        /// </summary>
        MultiLinkedList<IRaderOwner> tankRaderOwners = new MultiLinkedList<IRaderOwner>();

        /// <summary>
        /// �ڵ�̹���״�����弯��
        /// </summary>
        MultiLinkedList<IShelterObj> tankRaderShelters = new MultiLinkedList<IShelterObj>();


        /*
         * ��������ξ��廯��ײ��ϵ��
         * 
         * 
         * ��ײ��ϵ������ʾ��
         * 
         *              ����        ��ͻ��      ��ͻ��      �Ϳ�        �߿�        �߽�        
         * ������������������������������������������������������������������������������
         * ����                     ��ײ        ��ײ                    
         * ������������������������������������������������������������������������������
         * ��ͻ��       ��ײ                    ��ײ        
         * ������������������������������������������������������������������������������
         * ��ͻ��       ��ײ        ��ײ        ��ײ        ��ײ        �ص�        ��ײ
         * ������������������������������������������������������������������������������
         * �Ϳ�                                 ��ײ                    �ص�        �ص�
         * ������������������������������������������������������������������������������
         * �߿�                                 �ص�        �ص�                        
         * ������������������������������������������������������������������������������
         * �߽�                                 ��ײ        �ص�       
         * ������������������������������������������������������������������������������
         * 
         * 
         * ���У�̹�����ڸ�ͻ�������ڵ����ڵͿ����壬��������������Է����ڸ߿������С�
         * 
         * ���������������ݱ����ƵĶ���ֱ�����ڰ��ݡ���ͻ���͸�ͻ���С�
         * 
         * 
         * ��Щ��ϵ�ǿ�����SenceKeeper�ļ̳������޸ĵġ�
         * 
         * */


        /// <summary>
        /// �������弯��
        /// </summary>
        MultiLinkedList<ICollideObj> concaveObjs = new MultiLinkedList<ICollideObj>();

        /// <summary>
        /// ��ͻ�����弯��
        /// </summary>
        MultiLinkedList<ICollideObj> lowBulgeObjs = new MultiLinkedList<ICollideObj>();

        /// <summary>
        /// ��ͻ�����弯��
        /// </summary>
        MultiLinkedList<ICollideObj> highBulgeObjs = new MultiLinkedList<ICollideObj>();

        /// <summary>
        /// �Ϳ����弯��
        /// </summary>
        MultiLinkedList<ICollideObj> lowFlyingObjs = new MultiLinkedList<ICollideObj>();

        /// <summary>
        /// �߿����弯
        /// </summary>
        MultiLinkedList<ICollideObj> highFlyingObjs = new MultiLinkedList<ICollideObj>();



        /// <summary>
        /// �����߽硣
        /// </summary>
        Border border;

        /// <summary>
        /// ��̹�˿ɼ���
        /// </summary>
        Dictionary<IEyeableObj, GetEyeableInfoHandler> visibleObjs = new Dictionary<IEyeableObj, GetEyeableInfoHandler>();

        #endregion

        int assignedID = -1;

        #region RegistGroups

        public void RegistPhiCol ( PhiColManager manager )
        {
            ICollideObj[] boders = new ICollideObj[] { border };

            manager.AddPhiGroup( phisicalObjs );

            manager.AddColGroup( concaveObjs, lowBulgeObjs );
            manager.AddColGroup( concaveObjs, highBulgeObjs );
            manager.AddColGroup( lowBulgeObjs, highBulgeObjs );
            manager.AddColGroup( highBulgeObjs );
            manager.AddColGroup( highBulgeObjs, lowFlyingObjs );
            manager.AddOverlapColGroup( highBulgeObjs, highFlyingObjs );
            manager.AddColGroup( highBulgeObjs, boders );
            manager.AddOverlapColGroup( lowFlyingObjs, boders );
        }

        public void RegistShelter ( ShelterManager manager )
        {
            manager.AddRaderShelterGroup( tankRaderOwners, new MultiLinkedList<IShelterObj>[] { tankRaderShelters } );
        }

        public void RegistDrawables ( DrawManager manager )
        {
            manager.AddGroup( SenceObjs.GetConvertList<IDrawableObj>() );
        }

        public void RegistUpdaters ( UpdateManager manager )
        {
            manager.AddGroup( SenceObjs.GetConvertList<IUpdater>() );
        }

        public void RegistVision ( VisionManager manager )
        {
            manager.AddVisionGroup( tankRaderOwners, visibleObjs );
        }

        #endregion

        #region Add && Remove Obj

        /// <summary>
        /// ���峡���Ĳ�ͬ�㡣
        /// </summary>
        public enum GameObjLayer
        {
            /// <summary>
            /// ��������
            /// </summary>
            Convace,
            /// <summary>
            /// ��ͻ������
            /// </summary>
            LowBulge,
            /// <summary>
            /// ��ͻ������
            /// </summary>
            HighBulge,
            /// <summary>
            /// �Ϳ�����
            /// </summary>
            lowFlying,
            /// <summary>
            /// �߿�����
            /// </summary>
            highFlying
        }

        public void AddGameObj ( IGameObj obj, bool asPhi, bool asShe, bool asRaderOwner, GameObjLayer layer )
        {
            if (obj == null)
                throw new NullReferenceException( "GameObj is null!" );

            if (asPhi && !(obj is IPhisicalObj))
                throw new Exception( "obj isn't a IPhisicalObj!" );
            if (!(obj is ICollideObj))
                throw new Exception( "obj isn't a ICollideObj!" );
            if (asShe && !(obj is IShelterObj))
                throw new Exception( "obj isn't a IShelterObj!" );
            if (asRaderOwner && !(obj is IRaderOwner))
                throw new Exception( "obj isn't a IRaderOwner!" );


            SenceObjs.AddLast( obj );

            if (asPhi)
                phisicalObjs.AddLast( (IPhisicalObj)obj );
            if (asShe)
                tankRaderShelters.AddLast( (IShelterObj)obj );
            if (asRaderOwner)
                tankRaderOwners.AddLast( (IRaderOwner)obj );


            if (layer == GameObjLayer.Convace)
                concaveObjs.AddLast( (ICollideObj)obj );
            else if (layer == GameObjLayer.LowBulge)
                lowBulgeObjs.AddLast( (ICollideObj)obj );
            else if (layer == GameObjLayer.HighBulge)
                highBulgeObjs.AddLast( (ICollideObj)obj );
            else if (layer == GameObjLayer.lowFlying)
                lowFlyingObjs.AddLast( (ICollideObj)obj );
            else if (layer == GameObjLayer.highFlying)
                highFlyingObjs.AddLast( (ICollideObj)obj );

            assignedID++;
            obj.ObjInfo.SetID( assignedID );

            bool isTankObstacle = layer == GameObjLayer.HighBulge || layer == GameObjLayer.Convace || layer == GameObjLayer.LowBulge;
            obj.ObjInfo.SetSceneInfo( new SceneCommonObjInfo( asShe, isTankObstacle ) );

        }

        public void AddGameObj ( IGameObj obj, bool asPhi, bool asShe, bool asRaderOwner, GameObjLayer layer, GetEyeableInfoHandler getEyeableInfo )
        {
            AddGameObj( obj, asPhi, asShe, asRaderOwner, layer );

            visibleObjs.Add( (IEyeableObj)obj, getEyeableInfo );
        }

        public void RemoveGameObj ( IGameObj obj, bool asPhi, bool asShe, bool asRaderOwner, bool asEyeable, GameObjLayer layer )
        {
            if (obj == null)
                throw new NullReferenceException( "GameObj is null!" );

            if (asPhi && !(obj is IPhisicalObj))
                throw new Exception( "obj isn't a IPhisicalObj!" );
            if (!(obj is ICollideObj))
                throw new Exception( "obj isn't a ICollideObj!" );
            if (asShe && !(obj is IShelterObj))
                throw new Exception( "obj isn't a IShelterObj!" );
            if (asRaderOwner && !(obj is IRaderOwner))
                throw new Exception( "obj isn't a IRaderOwner!" );
            if (asEyeable && !(obj is IEyeableObj))
                throw new Exception( "obj isn't a IEyeableObj!" );

            SenceObjs.Remove( obj );

            if (asPhi)
                phisicalObjs.Remove( (IPhisicalObj)obj );
            if (asShe)
                tankRaderShelters.Remove( (IShelterObj)obj );
            if (asRaderOwner)
                tankRaderOwners.Remove( (IRaderOwner)obj );

            if (layer == GameObjLayer.Convace)
                concaveObjs.Remove( (ICollideObj)obj );
            else if (layer == GameObjLayer.LowBulge)
                lowBulgeObjs.Remove( (ICollideObj)obj );
            else if (layer == GameObjLayer.HighBulge)
                highBulgeObjs.Remove( (ICollideObj)obj );
            else if (layer == GameObjLayer.lowFlying)
                lowFlyingObjs.Remove( (ICollideObj)obj );
            else if (layer == GameObjLayer.highFlying)
                highFlyingObjs.Remove( (ICollideObj)obj );

            if (asEyeable)
            {
                visibleObjs.Remove( (IEyeableObj)obj );
            }

        }

        #endregion

        #region SetBorder

        public void SetBorder ( float minX, float maxX, float minY, float maxY )
        {
            border = new Border( minX, maxX, minY, maxY );
        }

        #endregion
    }
}
