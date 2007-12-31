using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Screens;

namespace SmartTank.Rule
{
    /*
     * 该对象将SceneKeeper和GameObjs通过一个游戏规则联系起来。
     * 
     * 同时也使以上两者之间的耦合性消失。
     * 
     * */
    public interface IGameRule : IGameScreen
    {
        string RuleName { get;}
        string RuleIntroduction { get;}
    }
}
