using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Rule;
using SmartTank.Screens;
using SmartTank.net;

namespace InterRules.Starwar
{
    class StarwarRule : IGameRule
    {
        #region IGameRule 成员

        public string RuleIntroduction
        {
            get { return "编写组成员：xxx"; }
        }

        public string RuleName
        {
            get { return "Starwar"; }
        }

        #endregion

        #region IGameScreen 成员

        public void OnClose()
        {
        }

        public void Render()
        {

        }

        public bool Update(float second)
        {
            return false;
        }

        #endregion
    }


    class StarwarLogic : RuleSupNet, IGameScreen
    {
        #region IGameScreen 成员

        void IGameScreen.OnClose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IGameScreen.Render()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool IGameScreen.Update(float second)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
