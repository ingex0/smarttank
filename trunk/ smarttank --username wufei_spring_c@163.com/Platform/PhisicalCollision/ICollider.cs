using System;
using System.Collections.Generic;
using System.Text;
using GameBase.Graphics;
using Platform.GameObjects;

namespace Platform.PhisicalCollision
{
    public interface ICollideObj : IHasBorderObj
    {
        GameObjInfo ObjInfo { get;}
        IColChecker ColChecker { get;}
    }

    public interface IColChecker
    {
        // �µĴ�����ײ�İ취��������������ײ��ⷽ����ϡ�
        IColMethod CollideMethod { get;}

        /// <summary>
        /// ���������ص�ʽ��ײʱ�������������
        /// </summary>
        /// <param name="result"></param>
        /// <param name="objB"></param>
        void HandleCollision ( CollisionResult result, GameObjInfo objB );

        /// <summary>
        /// ��������½���ɲ�������ص�ʱ���뽫��һ״̬�ĳ������뵱ǰ״̬һ����
        /// </summary>
        void ClearNextStatus ();

        /// <summary>
        /// �������ص�ʽ��ײʱ����øô�������
        /// </summary>
        /// <param name="result"></param>
        /// <param name="gameObjInfo"></param>
        void HandleOverlap ( CollisionResult result, GameObjInfo objB );

    }

    public interface IColMethod
    {
        CollisionResult CheckCollision ( IColMethod colB );
        CollisionResult CheckCollisionWithSprites ( SpriteColMethod spriteChecker );
        CollisionResult CheckCollisionWithBorder ( BorderMethod Border );
    }

    public delegate void OnCollidedEventHandler ( IGameObj Sender, CollisionResult result, GameObjInfo objB );
}
