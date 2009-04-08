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

                // ������Ϣ������
                SyncCasheReader.ReadCashe(sceneMgr);

                GameManager.UpdataComponent(second);

                // �㲥ͬ����Ϣ
                outputCashe.SendPackage();
                SyncCasheWriter.Update(second);

            }
            else
            {
                /* 1.����ͬ����Ϣ������
                 * 1.���³�������  ���л������Ҫ���͵�ͬ����Ϣ
                 * 2.�����������
                 * 3.����ͬ����Ϣ
                 * */
                GameManager.UpdateMgr.Update(second);
                GameManager.UpdataComponent(second);
                // ������Ϣ������
                SyncCasheReader.ReadCashe(sceneMgr);
                // ����ͬ����Ϣ
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
