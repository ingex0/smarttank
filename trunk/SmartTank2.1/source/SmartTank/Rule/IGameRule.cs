using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Screens;

namespace SmartTank.Rule
{
    /*
     * ������ƽ̨������Ҫ�̳еĽӿ�
     * 
     * */
    public interface IGameRule : IGameScreen
    {
        string RuleName { get;}
        string RuleIntroduction { get;}
    }
}
