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
     * 一个场景数据储存类。
     * 
     * 具体化碰撞关系和遮挡关系。适用一般情况。
     * 
     * */
    public class SceneKeeperCommon : ISceneKeeper
    {
        #region Keepers

        /// <summary>
        /// 场景物体集，用于统一调用Updata函数和绘制函数。
        /// </summary>
        MultiLinkedList<IGameObj> SenceObjs = new MultiLinkedList<IGameObj>();

        /// <summary>
        /// 物理更新物体集，用于物理更新和碰撞组件（PhiColManager）。
        /// </summary>
        MultiLinkedList<IPhisicalObj> phisicalObjs = new MultiLinkedList<IPhisicalObj>();

        /*
         * 遮挡关系方面，暂且只定义一种类型的雷达——坦克雷达。
         * 
         * */

        /// <summary>
        /// 坦克雷达的所有者集。
        /// </summary>
        MultiLinkedList<IRaderOwner> tankRaderOwners = new MultiLinkedList<IRaderOwner>();

        /// <summary>
        /// 遮挡坦克雷达的物体集。
        /// </summary>
        MultiLinkedList<IShelterObj> tankRaderShelters = new MultiLinkedList<IShelterObj>();


        /*
         * 按场景层次具体化碰撞关系。
         * 
         * 
         * 碰撞关系如下所示：
         * 
         *              凹陷        低突出      高突出      低空        高空        边界        
         * ———————————————————————————————————————
         * 凹陷                     碰撞        碰撞                    
         * ———————————————————————————————————————
         * 低突出       碰撞                    碰撞        
         * ———————————————————————————————————————
         * 高突出       碰撞        碰撞        碰撞        碰撞        重叠        碰撞
         * ———————————————————————————————————————
         * 低空                                 碰撞                    重叠        重叠
         * ———————————————————————————————————————
         * 高空                                 重叠        重叠                        
         * ———————————————————————————————————————
         * 边界                                 碰撞        重叠       
         * ———————————————————————————————————————
         * 
         * 
         * 其中，坦克属于高突出，而炮弹属于低空物体，场景交互物体可以放置在高空物体中。
         * 
         * 地形限制物体依据被限制的对象分别放置在凹陷、低突出和高突出中。
         * 
         * 
         * 这些关系是可以在SenceKeeper的继承类中修改的。
         * 
         * */


        /// <summary>
        /// 凹陷物体集。
        /// </summary>
        MultiLinkedList<ICollideObj> concaveObjs = new MultiLinkedList<ICollideObj>();

        /// <summary>
        /// 低突出物体集。
        /// </summary>
        MultiLinkedList<ICollideObj> lowBulgeObjs = new MultiLinkedList<ICollideObj>();

        /// <summary>
        /// 高突出物体集。
        /// </summary>
        MultiLinkedList<ICollideObj> highBulgeObjs = new MultiLinkedList<ICollideObj>();

        /// <summary>
        /// 低空物体集。
        /// </summary>
        MultiLinkedList<ICollideObj> lowFlyingObjs = new MultiLinkedList<ICollideObj>();

        /// <summary>
        /// 高空物体集
        /// </summary>
        MultiLinkedList<ICollideObj> highFlyingObjs = new MultiLinkedList<ICollideObj>();



        /// <summary>
        /// 场景边界。
        /// </summary>
        Border border;

        /// <summary>
        /// 对坦克可见物
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
        /// 定义场景的不同层。
        /// </summary>
        public enum GameObjLayer
        {
            /// <summary>
            /// 凹陷物体
            /// </summary>
            Convace,
            /// <summary>
            /// 低突出物体
            /// </summary>
            LowBulge,
            /// <summary>
            /// 高突出物体
            /// </summary>
            HighBulge,
            /// <summary>
            /// 低空物体
            /// </summary>
            lowFlying,
            /// <summary>
            /// 高空物体
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
