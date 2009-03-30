using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Screens;

namespace SmartTank.Rule
{
    /*
     * 定义了平台规则需要继承的接口
     * 
     * */
    public interface IGameRule : IGameScreen
    {
        string RuleName { get;}
        string RuleIntroduction { get;}
    }
}
