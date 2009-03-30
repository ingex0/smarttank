using System;
using System.Collections.Generic;
using System.Text;
using TankEngine2D.Graphics;
using SmartTank.Effects.SceneEffects;
//using SmartTank.GameDraw.SceneEffects;

namespace SmartTank.Draw
{
    /*
     * ��ǰ�����ֻ��һ���򵥵İ汾������Ҫ����µ����ݡ�
     * 
     * */

    public delegate bool DrawCondition ( IDrawableObj obj );

    public class DrawMgr
    {
        private static DrawCondition condition;

        List<IEnumerable<IDrawableObj>> drawableGroups = new List<IEnumerable<IDrawableObj>>();

        public void AddGroup ( IEnumerable<IDrawableObj> group )
        {
            drawableGroups.Add( group );
        }

        public void ClearGroups ()
        {
            drawableGroups.Clear();
        }

        public static void SetCondition ( DrawCondition condi )
        {
            condition = condi;
        }

        public void Draw ()
        {
            foreach (IEnumerable<IDrawableObj> group in drawableGroups)
            {
                foreach (IDrawableObj drawable in group)
                {
                    if (condition != null)
                    {
                        if (condition( drawable ))
                            drawable.Draw();
                    }
                    else
                        drawable.Draw();
                }
            }

            EffectsMgr.Draw(condition);
        }
    }
}
