using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Graphics
{
    /// <summary>
    /// ���еĶ����Ĺ�����
    /// </summary>
    public class AnimatedMgr
    {
        List<IAnimated> animates = new List<IAnimated>();

        /// <summary>
        /// ���Ƶ�ǰ������ж�����
        /// ��������еĶ�������ĳ��������ʱ�����Զ��ӱ�����б���ɾ����
        /// </summary>
        public void Draw ()
        {
            for (int i = 0; i < animates.Count; i++)
            {
                if (animates[i].IsStart)
                {
                    animates[i].DrawCurFrame();
                }

                if (animates[i].IsEnd)
                {
                    animates.Remove( animates[i] );
                    i--;
                }
            }
        }

        /// <summary>
        /// ��ձ�������ж���
        /// </summary>
        public void Clear ()
        {
            animates.Clear();
        }

        internal void Add ( IAnimated animatedSprite )
        {
            animates.Add( animatedSprite );
        }
    }
}
