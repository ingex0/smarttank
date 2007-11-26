using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GameBase.Graphics
{
    /// <summary>
    /// 所有的动画的管理类
    /// </summary>
    public class AnimatedManager
    {
        static List<IAnimated> sCartoons = new List<IAnimated>();

        /// <summary>
        /// 绘制当前活动的所有动画。
        /// 会更新所有的动画。但某动画结束时，会自动从保存的列表中删除。
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
        /// 清空保存的所有动画
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
