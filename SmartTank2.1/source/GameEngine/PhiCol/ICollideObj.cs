using System;
using System.Collections.Generic;
using System.Text;
using GameEngine.Graphics;

namespace GameEngine.PhiCol
{
    /// <summary>
    /// ��ʾ�ɽ��г�ͻ��������
    /// </summary>
    public interface ICollideObj
    {
        /// <summary>
        /// ��ó�ͻ�����
        /// </summary>
        IColChecker ColChecker { get;}
    }

    /// <summary>
    /// ��ʾ��ͻ����ߡ�
    /// ���ܹ�ͨ��IColMethod����ͻ���������ͻ��
    /// �����ײ���������һ���˶����壬����������һ��״̬���г�ͻ��⡣
    /// </summary>
    public interface IColChecker
    {
        /// <summary>
        /// ��ü���ͻ�ľ��巽��
        /// </summary>
        IColMethod CollideMethod { get;}

        /// <summary>
        /// ���������ص�ʽ��ײʱ�������������
        /// </summary>
        /// <param name="result">��ͻ���</param>
        /// <param name="objB">��ͻ����</param>
        void HandleCollision ( CollisionResult result, ICollideObj objA, ICollideObj objB );

        /// <summary>
        /// ��������½���ɲ�������ص�ʱ���뽫��һ״̬�ĳ������뵱ǰ״̬һ����
        /// </summary>
        void ClearNextStatus ();

        /// <summary>
        /// �������ص�ʽ��ײʱ����øô�������
        /// </summary>
        /// <param name="result">��ͻ���</param>
        /// <param name="objB">��ͻ����</param>
        void HandleOverlap( CollisionResult result, ICollideObj objA, ICollideObj objB );

    }

    /// <summary>
    /// ��ʾ����ͻ�ľ��巽����
    /// ��ǰ��֧���������͵ĳ�ͻ��⣺
    ///     �����뾫������ؼ�ⷽ����
    ///     ������߽�ļ�ⷽ����
    /// </summary>
    public interface IColMethod
    {
        /// <summary>
        /// �������һ�����Ƿ��ͻ
        /// </summary>
        /// <param name="colB"></param>
        /// <returns></returns>
        CollisionResult CheckCollision ( IColMethod colB );

        /// <summary>
        /// ����뾫������Ƿ��ͻ
        /// </summary>
        /// <param name="spriteChecker"></param>
        /// <returns></returns>
        CollisionResult CheckCollisionWithSprites ( SpriteColMethod spriteChecker );

        /// <summary>
        /// �����߽�����Ƿ��ͻ
        /// </summary>
        /// <param name="Border"></param>
        /// <returns></returns>
        CollisionResult CheckCollisionWithBorder ( BorderColMethod Border );
    }

    public delegate void OnCollidedEventHandler (CollisionResult result,ICollideObj objA, ICollideObj objB);
}
