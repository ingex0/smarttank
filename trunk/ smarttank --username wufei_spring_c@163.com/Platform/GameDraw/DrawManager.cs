using System;
using System.Collections.Generic;
using System.Text;
using GameBase.Graphics;
using Platform.GameDraw.SceneEffects;

namespace Platform.GameDraw
{
    /*
     * ��ǰ�����ֻ��һ���򵥵İ汾������Ҫ����µ����ݡ�
     * 
     * */

    public delegate bool DrawCondition ( IDrawableObj obj );

    public class DrawManager
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

            EffectsManager.Draw(condition);
        }
    }
}
