using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TankEngine2D.Helpers;

namespace SmartTank.Effects.TextEffects
{
    /// <summary>
    /// 字体效果的管理类
    /// </summary>
    public static class TextEffect
    {
        static LinkedList<ITextEffect> sEffects = new LinkedList<ITextEffect>();

        /// <summary>
        /// 绘制当前的特效字体并更新他们
        /// </summary>
        static public void Draw ()
        {
            LinkedListNode<ITextEffect> node;
            for (node = sEffects.First; node != null; )
            {
                if (node.Value.Ended)
                {
                    node = node.Next;
                    if (node != null)
                        sEffects.Remove( node.Previous );
                    else
                        sEffects.RemoveLast();
                }
                else
                {
                    node.Value.Draw();
                    node = node.Next;
                }
            }
        }

        /// <summary>
        /// 指定逻辑坐标添加一个RiseFade文字特效。
        /// 注意，如果文字内容中包含中文，必须选用中文字体
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">显示的逻辑位置</param>
        /// <param name="Scale">大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">绘制深度</param>
        /// <param name="fontType">字体</param>
        /// <param name="existedFrame">保持在屏幕中的时间循环次数</param>
        /// <param name="step">每次时间循环中，上升的高度，以像素为单位</param>
        static public void AddRiseFade ( string text, Vector2 pos, float Scale, Color color, float layerDepth,string fontType, float existedFrame, float step )
        {
            sEffects.AddLast( new FadeUpEffect( text, true, pos, Scale, color, layerDepth, fontType, existedFrame, step ) );
        }

        /// <summary>
        /// 在屏幕坐标中添加一个RiseFade文字特效。
        /// 注意，如果文字内容中包含中文，必须选用中文字体
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">显示的屏幕位置</param>
        /// <param name="Scale">大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">绘制深度</param>
        /// <param name="fontType">字体</param>
        /// <param name="existedFrame">保持在屏幕中的时间循环次数</param>
        /// <param name="step">每次时间循环中，上升的高度，以像素为单位</param>
        static public void AddRiseFadeInScrnCoordin ( string text, Vector2 pos, float Scale, Color color, float layerDepth, string fontType, float existedFrame, float step )
        {
            sEffects.AddLast( new FadeUpEffect( text, false, pos, Scale, color, layerDepth, fontType, existedFrame, step ) );
        }

        /// <summary>
        /// 清楚所有的文字特效
        /// </summary>
        public static void Clear ()
        {
            sEffects.Clear();
        }

    }

    /// <summary>
    /// 定义文字特效的一般方法
    /// </summary>
    public interface ITextEffect
    {
        /// <summary>
        /// 绘制并更新自己
        /// </summary>
        void Draw ();

        /// <summary>
        /// 是否已经结束
        /// </summary>
        bool Ended { get;}
    }
    
}
