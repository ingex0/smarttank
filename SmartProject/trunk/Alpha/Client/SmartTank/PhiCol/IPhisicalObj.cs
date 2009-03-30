using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.PhiCol
{
    /// <summary>
    /// 表示一个物理更新器
    /// </summary>
    public interface IPhisicalUpdater
    {
        /// <summary>
        /// 计算下一个物理状态，并不生效
        /// </summary>
        /// <param name="seconds"></param>
        void CalNextStatus ( float seconds );

        /// <summary>
        /// 生效下一个物理状态
        /// </summary>
        void Validated ();
    }

    /// <summary>
    /// 表示一个能更新物理状态的物体
    /// </summary>
    public interface IPhisicalObj
    {
        /// <summary>
        /// 获得物理状态更新器
        /// </summary>
        IPhisicalUpdater PhisicalUpdater { get;}
    }
}
