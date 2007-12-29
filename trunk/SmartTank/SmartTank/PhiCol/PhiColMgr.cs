using System;
using System.Collections.Generic;
using System.Text;
using TankEngine2D.Graphics;
using TankEngine2D.Helpers;

namespace SmartTank.PhiCol
{

    /// <summary>
    /// ������ºͳ�ͻ������
    /// </summary>
    public class PhiColMgr
    {
        const int maxIterDepth = 10;

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

        /*
         * ��������ײ�����ص�����
         * */
        List<IEnumerable<ICollideObj>> ColliderGroups_Single = new List<IEnumerable<ICollideObj>>();
        List<BinGroup> ColliderGroups_Binary = new List<BinGroup>();

        /*
         * ������ײ�����ص�����
         * */
        List<IEnumerable<ICollideObj>> ColliderGroups_CanOverlap_Single = new List<IEnumerable<ICollideObj>>();
        List<BinGroup> ColliderGroups_CanOverlap_Binary = new List<BinGroup>();

        #endregion

        #region Public Methods

        /// <summary>
        /// �����Ҫ�����Ա�˴˼����ײ���飬�������ص���
        /// </summary>
        /// <param name="group"></param>
        public void AddCollideGroup ( IEnumerable<ICollideObj> group )
        {
            ColliderGroups_Single.Add( group );
        }

        /// <summary>
        /// �����Ҫ����˴�֮�����ײ�������顣������������ڲ���Ա����ײ���������ص���
        /// </summary>
        /// <param name="group1"></param>
        /// <param name="group2"></param>
        public void AddCollideGroup ( IEnumerable<ICollideObj> group1, IEnumerable<ICollideObj> group2 )
        {
            ColliderGroups_Binary.Add( new BinGroup( group1, group2 ) );
        }

        /// <summary>
        /// �����Ҫ�����Ա�˴˼����ײ���飬�����ص���
        /// </summary>
        /// <param name="group"></param>
        public void AddOverlapGroup ( IEnumerable<ICollideObj> group )
        {
            ColliderGroups_CanOverlap_Single.Add( group );
        }

        /// <summary>
        /// �����Ҫ����˴�֮�����ײ�������顣������������ڲ���Ա����ײ�������ص���
        /// </summary>
        /// <param name="group1"></param>
        /// <param name="group2"></param>
        public void AddOverlapGroup ( IEnumerable<ICollideObj> group1, IEnumerable<ICollideObj> group2 )
        {
            ColliderGroups_CanOverlap_Binary.Add( new BinGroup( group1, group2 ) );
        }


        /// <summary>
        /// �����������ӵ��顣
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
        /// ���������������顣
        /// </summary>
        /// <param name="phisicals"></param>
        public void AddPhiGroup ( IEnumerable<IPhisicalObj> phisicals )
        {
            PhisicalGroups.Add( phisicals );
        }

        #endregion

        #region Update

        /// <summary>
        /// ����ע�����������״̬��ִ�г�ͻ����봦��
        /// </summary>
        /// <param name="seconds">��ǰ֡����һ֡��ʱ����������Ϊ��λ</param>
        public void Update ( float seconds )
        {
            // ����ÿһ����������������һ��״̬��
            foreach (IEnumerable<IPhisicalObj> group in PhisicalGroups)
            {
                foreach (IPhisicalObj phiObj in group)
                {
                    phiObj.PhisicalUpdater.CalNextStatus( seconds );
                }
            }

            // �����ص��������ײ����������ײ�����ô�������
            CheckOverlap();

            // �����һ��״̬����ײ���,�����д���ȷ����״̬���κ����岻��ײ��
            iterDepth = 0;
            HandleCollision( true );

            // Ӧ���������״̬��
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
                        }
                    }
                }

            }

            foreach (CollisionResultGroup group in colResults)
            {
                group.colA.ColChecker.HandleOverlap( group.result, group.colB );
                group.colB.ColChecker.HandleOverlap( new CollisionResult( group.result.InterPos, -group.result.NormalVector ), group.colA );
            }

        }

        int iterDepth = 0;
        private void HandleCollision ( bool CallHandleCollision )
        {
            List<CollisionResultGroup> colResults = new List<CollisionResultGroup>();


            List<ICollideObj> Collideds = new List<ICollideObj>();

            #region ����singleGroups

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

            #region ����binaryGroups

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
                group.colA.ColChecker.HandleCollision( group.result, group.colB );
                group.colB.ColChecker.HandleCollision( new CollisionResult( group.result.InterPos, -group.result.NormalVector ), group.colA );
            }


            iterDepth++;
            if (iterDepth > maxIterDepth)
            {
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

        #endregion
    }
}
