using System;
using System.Collections.Generic;
using System.Text;
using GameBase.Graphics;
using GameBase.Helpers;
using Platform.Update;

namespace Platform.PhisicalCollision
{
    /*
     * 考虑到游戏设计的需求，当前在碰撞处理支持两种模式：
     * 
     *      不允许重叠的碰撞检测。
     * 
     *      允许重叠的碰撞检测。
     * 
     * */
    public class PhiColManager : IUpdater
    {
        #region private type

        struct BinGroup
        {
            public IEnumerable<ICollideObj> group1;
            public IEnumerable<ICollideObj> group2;

            public BinGroup ( IEnumerable<ICollideObj> group1, IEnumerable<ICollideObj> group2 )
            {
                this.group1 = group1;
                this.group2 = group2;
            }
        }

        struct CollisionResultGroup
        {
            public ICollideObj colA;
            public ICollideObj colB;
            public CollisionResult result;

            public CollisionResultGroup ( ICollideObj colA, ICollideObj colB, CollisionResult result )
            {
                this.colA = colA;
                this.colB = colB;
                this.result = result;
            }
        }

        #endregion

        #region Variables

        List<IEnumerable<IPhisicalObj>> PhisicalGroups = new List<IEnumerable<IPhisicalObj>>();

        List<IEnumerable<ICollideObj>> ColliderGroups_Single = new List<IEnumerable<ICollideObj>>();
        List<BinGroup> ColliderGroups_Binary = new List<BinGroup>();

        /// <summary>
        /// 允许碰撞物体重叠的组
        /// </summary>
        List<IEnumerable<ICollideObj>> ColliderGroups_CanOverlap_Single = new List<IEnumerable<ICollideObj>>();
        List<BinGroup> ColliderGroups_CanOverlap_Binary = new List<BinGroup>();

        #endregion

        #region Public Methods

        /// <summary>
        /// 添加需要管理成员彼此间的碰撞的组。
        /// </summary>
        /// <param name="group"></param>
        public void AddColGroup ( IEnumerable<ICollideObj> group )
        {
            ColliderGroups_Single.Add( group );
        }

        /// <summary>
        /// 添加需要管理彼此之间的碰撞的两个组。不检查两个组内部成员的碰撞。
        /// </summary>
        /// <param name="group1"></param>
        /// <param name="group2"></param>
        public void AddColGroup ( IEnumerable<ICollideObj> group1, IEnumerable<ICollideObj> group2 )
        {
            ColliderGroups_Binary.Add( new BinGroup( group1, group2 ) );
        }

        /// <summary>
        /// 添加需要管理成员彼此间的碰撞的组，允许重叠。
        /// </summary>
        /// <param name="group"></param>
        public void AddOverlapColGroup ( IEnumerable<ICollideObj> group )
        {
            ColliderGroups_CanOverlap_Single.Add( group );
        }

        /// <summary>
        /// 添加需要管理彼此之间的碰撞的两个组。不检查两个组内部成员的碰撞，允许重叠。
        /// </summary>
        /// <param name="group1"></param>
        /// <param name="group2"></param>
        public void AddOverlapColGroup ( IEnumerable<ICollideObj> group1, IEnumerable<ICollideObj> group2 )
        {
            ColliderGroups_CanOverlap_Binary.Add( new BinGroup( group1, group2 ) );
        }


        /// <summary>
        /// 清空所有已添加的组。
        /// </summary>
        public void ClearGroups ()
        {
            ColliderGroups_Single.Clear();
            ColliderGroups_Binary.Clear();
            ColliderGroups_CanOverlap_Single.Clear();
            ColliderGroups_CanOverlap_Binary.Clear();
            PhisicalGroups.Clear();
        }

        /// <summary>
        /// 添加物理更新物体组。
        /// </summary>
        /// <param name="phisicals"></param>
        public void AddPhiGroup ( IEnumerable<IPhisicalObj> phisicals )
        {
            PhisicalGroups.Add( phisicals );
        }

        #endregion

        #region Update

        public void Update ( float seconds )
        {
            // 计算每一个物理更新物体的下一个状态。
            foreach (IEnumerable<IPhisicalObj> group in PhisicalGroups)
            {
                foreach (IPhisicalObj phiObj in group)
                {
                    phiObj.PhisicalUpdater.CalNextStatus( seconds );
                }
            }

            // 检查可重叠物体的碰撞情况，如果碰撞，调用处理函数。
            CheckOverlap();

            // 检查下一个状态的碰撞情况,并进行处理。确保新状态中任何物体不碰撞。
            iterDepth = 0;
            HandleCollision( true );

            // 应用物体的新状态。
            foreach (IEnumerable<IPhisicalObj> group in PhisicalGroups)
            {
                foreach (IPhisicalObj phiObj in group)
                {
                    phiObj.PhisicalUpdater.Validated();
                }
            }

        }

        private void CheckOverlap ()
        {
            List<CollisionResultGroup> colResults = new List<CollisionResultGroup>();

            foreach (IEnumerable<ICollideObj> singleGroup in ColliderGroups_CanOverlap_Single)
            {
                ICollideObj[] temp = GetArray( singleGroup );
                for (int i = 0; i < temp.Length - 1; i++)
                {
                    for (int j = i + 1; j < temp.Length; j++)
                    {
                        CollisionResult result = temp[i].ColChecker.CollideMethod.CheckCollision( temp[j].ColChecker.CollideMethod );
                        if (result.IsCollided)
                        {
                            colResults.Add( new CollisionResultGroup( temp[i], temp[j], result ) );
                            //temp[i].ColChecker.HandleOverlap( result, temp[j].ObjInfo );
                            //temp[j].ColChecker.HandleOverlap( new CollisionResult( result.InterPos, -result.NormalVector ), temp[i].ObjInfo );
                        }
                    }
                }
            }

            foreach (BinGroup binaryGroup in ColliderGroups_CanOverlap_Binary)
            {
                foreach (ICollideObj colA in binaryGroup.group1)
                {
                    foreach (ICollideObj colB in binaryGroup.group2)
                    {
                        CollisionResult result = colA.ColChecker.CollideMethod.CheckCollision( colB.ColChecker.CollideMethod );
                        if (result.IsCollided)
                        {
                            colResults.Add( new CollisionResultGroup( colA, colB, result ) );
                            //colA.ColChecker.HandleOverlap( result, colB.ObjInfo );
                            //colB.ColChecker.HandleOverlap( new CollisionResult( result.InterPos, -result.NormalVector ), colA.ObjInfo );
                        }
                    }
                }

            }

            foreach (CollisionResultGroup group in colResults)
            {
                group.colA.ColChecker.HandleOverlap( group.result, group.colB.ObjInfo );
                group.colB.ColChecker.HandleOverlap( new CollisionResult( group.result.InterPos, -group.result.NormalVector ), group.colA.ObjInfo );
            }

        }

        // 暂时使用损失时间的方法。
        int iterDepth = 0;
        private void HandleCollision ( bool CallHandleCollision )
        {
            List<CollisionResultGroup> colResults = new List<CollisionResultGroup>();


            List<ICollideObj> Collideds = new List<ICollideObj>();

            #region 处理singleGroups

            foreach (IEnumerable<ICollideObj> singleGroup in ColliderGroups_Single)
            {
                ICollideObj[] temp = GetArray( singleGroup );
                for (int i = 0; i < temp.Length - 1; i++)
                {
                    for (int j = i + 1; j < temp.Length; j++)
                    {
                        CollisionResult result = temp[i].ColChecker.CollideMethod.CheckCollision( temp[j].ColChecker.CollideMethod );
                        if (result.IsCollided)
                        {
                            if (CallHandleCollision)
                            {
                                colResults.Add( new CollisionResultGroup( temp[i], temp[j], result ) );
                                //temp[i].ColChecker.HandleCollision( result, temp[j].ObjInfo );
                                //temp[j].ColChecker.HandleCollision( new CollisionResult( result.InterPos, -result.NormalVector ), temp[i].ObjInfo );
                            }
                            Collideds.Add( temp[i] );
                            Collideds.Add( temp[j] );
                        }
                    }
                }
            }

            #endregion

            #region 处理binaryGroups

            foreach (BinGroup binaryGroup in ColliderGroups_Binary)
            {
                foreach (ICollideObj colA in binaryGroup.group1)
                {
                    foreach (ICollideObj colB in binaryGroup.group2)
                    {
                        CollisionResult result = colA.ColChecker.CollideMethod.CheckCollision( colB.ColChecker.CollideMethod );
                        if (result.IsCollided)
                        {
                            if (CallHandleCollision)
                            {
                                colResults.Add( new CollisionResultGroup( colA, colB, result ) );
                            }
                            Collideds.Add( colA );
                            Collideds.Add( colB );
                        }
                    }
                }

            }
            #endregion

            foreach (CollisionResultGroup group in colResults)
            {
                group.colA.ColChecker.HandleCollision( group.result, group.colB.ObjInfo );
                group.colB.ColChecker.HandleCollision( new CollisionResult( group.result.InterPos, -group.result.NormalVector ), group.colA.ObjInfo );
                //group.colA.
            }


            iterDepth++;
            if (iterDepth > 20)
            {
                //throw new Exception( "there must be an error!" );
                Log.Write( "PhiColManager: iterDepth > 20!" );
                return;
            }

            if (Collideds.Count != 0)
            {
                foreach (ICollideObj col in Collideds)
                {
                    col.ColChecker.ClearNextStatus();
                }
                HandleCollision( false );
            }
        }

        private ICollideObj[] GetArray ( IEnumerable<ICollideObj> group )
        {
            List<ICollideObj> temp = new List<ICollideObj>( 64 );
            IEnumerator<ICollideObj> iter = group.GetEnumerator();
            if (iter.Current != null)
            {
                temp.Add( iter.Current );
            }
            while (iter.MoveNext())
            {
                temp.Add( iter.Current );
            }
            return temp.ToArray();
        }

        #region OldCode
        //private CollisionResult DetectCollision ( Sprite[] spritesA, Sprite[] spritesB )
        //{
        //    foreach (Sprite spriteA in spritesA)
        //    {
        //        foreach (Sprite spriteB in spritesB)
        //        {
        //            CollisionResult result = Sprite.IntersectPixels( spriteA, spriteB );
        //            if (result.IsCollided)
        //                return result;
        //        }
        //    }
        //    return new CollisionResult( false );
        //} 
        #endregion

        #endregion
    }
}
