using System;
using System.Collections.Generic;
using System.Text;
using Platform.Update;
using Platform.GameDraw;
using Platform.GameScreens;

namespace Platform.GameRules
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
