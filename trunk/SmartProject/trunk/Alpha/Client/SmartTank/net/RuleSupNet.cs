using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Rule;
using SmartTank.Effects.SceneEffects;
using SmartTank.Screens;

namespace SmartTank.net
{
    public class RuleSupNet: IGameScreen
    {
        SyncCashe inputCashe;
        SyncCashe outputCashe;

        public RuleSupNet()
        {
            GameManager.OnExiting += new EventHandler(GameManager_OnExiting);
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

        void GameManager_OnExiting(object sender, EventArgs e)
        {
            SocketMgr.CloseConnect();
            //SocketMgr.CloseThread();
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

                // TODO : 处理消息缓冲区
                GameManager.PhiColManager.Update(second);
                GameManager.ShelterMgr.Update();
                GameManager.VisionMgr.Update();
                GameManager.ObjMemoryMgr.Update();
                EffectsMgr.Update(second);

                outputCashe.SendPackage();
                SyncCasheWriter.Update(second);
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
                outputCashe.SendPackage();
                SyncCasheWriter.Update(second);
            }

            return false;
        }

        public virtual void Render()
        {
            
        }

        #endregion
    }
}
