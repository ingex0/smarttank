using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Screens;

namespace SmartTank.Rule
{
    /*
     * �ö���SceneKeeper��GameObjsͨ��һ����Ϸ������ϵ������
     * 
     * ͬʱҲʹ��������֮����������ʧ��
     * 
     * */
    public interface IGameRule : IGameScreen
    {
        string RuleName { get;}
        string RuleIntroduction { get;}
    }
}
