using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Rule;
using SmartTank.Effects.SceneEffects;

namespace SmartTank.net
{
    class RuleSupNet: IGameRule
    {
        bool isMainHost;
        public bool IsMainHost
        {
            get { return isMainHost; }
            set { isMainHost = value; }
        }

        #region IGameRule 成员

        public string RuleName
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string RuleIntroduction
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IGameScreen 成员

        public bool Update(float second)
        {
            if (isMainHost)
            {
                /* 1.更新场景物体
                 * 2.处理消息缓冲区
                 * 3.更新其他组件
                 * 4.广播同步消息
                 * */
                GameManager.UpdateMgr.Update(second);
                // TODO : 处理消息缓冲区
                GameManager.PhiColManager.Update(second);
                GameManager.ShelterMgr.Update();
                GameManager.VisionMgr.Update();
                GameManager.ObjMemoryMgr.Update();
                EffectsMgr.Update(second);

                // TODO : 广播同步消息
            }
            else
            {
                /* 1.处理同步消息缓冲区
                 * 1.更新场景物体  其中会添加需要发送的同步消息
                 * 2.更新其他组件
                 * 3.发送同步消息
                 * */

                GameManager.UpdataComponent(second);
                // TODO : 处理消息缓冲区
                // TODO : 发送同步消息
            }

            return false;
        }

        public void Render()
        {
            
        }

        #endregion
    }
}
