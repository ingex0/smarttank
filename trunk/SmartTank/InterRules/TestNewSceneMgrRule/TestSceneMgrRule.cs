using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Rule;
using SmartTank.Scene;
using SmartTank.GameObjs.Tank.SinTur;

namespace InterRules.TestNewSceneMgrRule
{
    class TestSceneMgrRule : IGameRule
    {
        SceneMgr sceneMgr;

        public TestSceneMgrRule ()
        {
            sceneMgr = new SceneMgr();

            //TankSinTur tank1 = sceneMgr.GetGameObj( @"Tank\tank1" );
            //TankSinTur tank2 = sceneMgr.GetGameObj( @"Tank\tank2" );

            //tank1.
            
        }

        #region IGameRule 成员

        public string RuleIntroduction
        {
            get { return "this class is to test SceneMgr."; }
        }

        public string RuleName
        {
            get { return "TestSceneMgrRule"; }
        }

        #endregion

        #region IGameScreen 成员

        public void Render ()
        {
            
        }

        public bool Update ( float second )
        {
            return true;
        }

        #endregion
    }
}
