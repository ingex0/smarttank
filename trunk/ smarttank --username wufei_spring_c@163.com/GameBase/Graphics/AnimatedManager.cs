using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GameBase.Graphics
{
    /// <summary>
    /// ���еĶ����Ĺ�����
    /// </summary>
    public class AnimatedManager
    {
        static List<IAnimated> sCartoons = new List<IAnimated>();

        /// <summary>
        /// ���Ƶ�ǰ������ж�����
        /// ��������еĶ�������ĳ��������ʱ�����Զ��ӱ�����б���ɾ����
        /// </summary>
        static public void Draw ()
        {
            for (int i = 0; i < sCartoons.Count; i++)
            {
                if (sCartoons[i].IsStart)
                {
                    sCartoons[i].DrawCurFrame();
                }

                if (sCartoons[i].IsEnd)
                {
                    sCartoons.Remove( sCartoons[i] );
                    i--;
                }
            }
        }

        /// <summary>
        /// ��ձ�������ж���
        /// </summary>
        static public void Clear ()
        {
            sCartoons.Clear();
        }

        internal static void Add ( IAnimated animatedSprite )
        {
            sCartoons.Add( animatedSprite );
        }
    }
}
