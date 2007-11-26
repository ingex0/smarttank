using System;
using System.Collections.Generic;
using System.Text;

namespace GameBase.Graphics
{
    /// <summary>
    /// 定义可切帧对象
    /// </summary>
    interface IAnimated
    {
        /// <summary>
        /// 获取是否已经开始显示
        /// </summary>
        bool IsStart { get;}

        /// <summary>
        /// 获取动画是否已经结束
        /// </summary>
        bool IsEnd { get;}

        /// <summary>
        /// 绘制当前帧，并准备下一帧
        /// </summary>
        void DrawCurFrame ();
    }
}
