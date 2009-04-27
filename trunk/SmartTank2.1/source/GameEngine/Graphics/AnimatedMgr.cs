using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Graphics
{
    /// <summary>
    /// 所有的动画的管理类
    /// </summary>
    public class AnimatedMgr
    {
        List<IAnimated> animates = new List<IAnimated>();

        /// <summary>
        /// 绘制当前活动的所有动画。
        /// 会更新所有的动画。但某动画结束时，会自动从保存的列表中删除。
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
        /// 清空保存的所有动画
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
