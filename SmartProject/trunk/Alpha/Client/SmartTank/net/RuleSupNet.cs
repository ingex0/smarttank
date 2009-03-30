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

        #region IGameRule ��Ա

        public string RuleName
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string RuleIntroduction
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IGameScreen ��Ա

        public bool Update(float second)
        {
            if (isMainHost)
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
            }

            return false;
        }

        public void Render()
        {
            
        }

        #endregion
    }
}
