using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Rule;
using SmartTank.Effects.SceneEffects;
using SmartTank.Screens;
using System.Threading;
using SmartTank.Scene;
using TankEngine2D.Input;

namespace SmartTank.net
{
    public class RuleSupNet : IGameScreen
    {
        SyncCashe inputCashe;
        SyncCashe outputCashe;

        protected SceneMgr sceneMgr;

        public RuleSupNet()
        {
            inputCashe = new SyncCashe();
            outputCashe = new SyncCashe();
            SyncCasheWriter.OutPutCashe = outputCashe;
            SyncCasheReader.InputCashe = inputCashe;

            //temp 
            PurviewMgr.IsMainHost = true;

            SocketMgr.Initial();
            SocketMgr.ConnectToServer();

            SocketMgr.StartReceiveThread(inputCashe);

        }

        #region IGameScreen 成员

        public virtual bool Update(float second)
        {
            if (PurviewMgr.IsMainHost)
            {
                /* 1.更新场景物体
                 * 2.处理消息缓冲区
                 * 3.更新其他组件
                 * 4.广播同步消息
                 * */
                GameManager.UpdateMgr.Update(second);

                // 处理消息缓冲区
                SyncCasheReader.ReadCashe(sceneMgr);

                GameManager.UpdataComponent(second);

                // 广播同步消息
                outputCashe.SendPackage();
                SyncCasheWriter.Update(second);

            }
            else
            {
                /* 1.处理同步消息缓冲区
                 * 1.更新场景物体  其中会添加需要发送的同步消息
                 * 2.更新其他组件
                 * 3.发送同步消息
                 * */
                GameManager.UpdateMgr.Update(second);
                GameManager.UpdataComponent(second);
                // 处理消息缓冲区
                SyncCasheReader.ReadCashe(sceneMgr);
                // 发送同步消息
                outputCashe.SendPackage();
                SyncCasheWriter.Update(second);
            }

            return false;
        }

        public virtual void Render()
        {

        }

        public void OnClose()
        {
            SocketMgr.Close();
            PurviewMgr.Close();
        }

        #endregion
    }
}
