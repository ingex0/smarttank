using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.Screens
{
    public interface IGameScreen
    {
        /// <summary>
        /// 当需要退出该屏幕时返回true
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        bool Update ( float second );

        void Render ();

        /// <summary>
        /// 当从该规则中退出前该方法被调用
        /// </summary>
        void OnClose();
    }
}
