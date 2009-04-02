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

        #region IGameScreen ��Ա

        public virtual bool Update(float second)
        {
            if (PurviewMgr.IsMainHost)
            {
                /* 1.���³�������
                 * 2.������Ϣ������
                 * 3.�����������
                 * 4.�㲥ͬ����Ϣ
                 * */
                GameManager.UpdateMgr.Update(second);

                // TODO : ������Ϣ������
                GameManager.PhiColManager.Update(second);
                GameManager.ShelterMgr.Update();
                GameManager.VisionMgr.Update();
                GameManager.ObjMemoryMgr.Update();
                EffectsMgr.Update(second);

                outputCashe.SendPackage();
                SyncCasheWriter.Update(second);
                // TODO : �㲥ͬ����Ϣ
            }
            else
            {
                /* 1.����ͬ����Ϣ������
                 * 1.���³�������  ���л������Ҫ���͵�ͬ����Ϣ
                 * 2.�����������
                 * 3.����ͬ����Ϣ
                 * */

                GameManager.UpdataComponent(second);
                // TODO : ������Ϣ������
                // TODO : ����ͬ����Ϣ
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
