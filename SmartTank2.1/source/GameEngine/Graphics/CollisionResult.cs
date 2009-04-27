using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngine.Graphics
{
    /// <summary>
    /// ��ʾ��ײ���Ľ��
    /// </summary>
    public class CollisionResult
    {
        /// <summary>
        /// �Ƿ������ײ
        /// </summary>
        public bool IsCollided;

        /// <summary>
        /// ��ײλ�ã��߼�����
        /// </summary>
        public Vector2 InterPos;

        /// <summary>
        /// ��ײ��λ��������ָ������
        /// </summary>
        public Vector2 NormalVector;

        /// <summary>
        /// �չ��캯��
        /// </summary>
        public CollisionResult ()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isCollided">�Ƿ�����ײ</param>
        public CollisionResult ( bool isCollided )
        {
            this.IsCollided = isCollided;
        }

        /// <summary>
        /// ��������ײ����µĹ��캯��
        /// </summary>
        /// <param name="interPos">��ײλ�ã��߼�����</param>
        /// <param name="normalVector">��ײ��λ��������ָ������</param>
        public CollisionResult ( Vector2 interPos, Vector2 normalVector )
        {
            IsCollided = true;
            InterPos = interPos;
            NormalVector = normalVector;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isCollided">�Ƿ�����ײ</param>
        /// <param name="interPos">��ײλ�ã��߼�����</param>
        /// <param name="nornalVector">��ײ��λ��������ָ������</param>
        public CollisionResult ( bool isCollided, Vector2 interPos, Vector2 nornalVector )
        {
            this.IsCollided = isCollided;
            this.InterPos = interPos;
            this.NormalVector = nornalVector;
        }
    }
}
