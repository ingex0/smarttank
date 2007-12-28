using System;
using System.Collections.Generic;
using System.Text;
using TankEngine2D.Graphics;
using SmartTank.Draw;
using SmartTank.Effects.TextEffects;

namespace SmartTank.Effects.SceneEffects
{
    class EffectsManager
    {
        static List<IManagedEffect> managedEffects = new List<IManagedEffect>();

        public static void AddManagedEffect ( IManagedEffect effect )
        {
            managedEffects.Add( effect );
        }

        public static void Update ( float seconds )
        {
            for (int i = 0; i < managedEffects.Count; i++)
            {
                managedEffects[i].Update( seconds );
                if (managedEffects[i].IsEnd)
                {
                    managedEffects.Remove( managedEffects[i] );
                    i--;
                }
            }
        }

        internal static void Draw ( DrawCondition condition )
        {
            BaseGame.AnimatedMgr.Draw();

            TextEffect.Draw();

            foreach (IManagedEffect effect in managedEffects)
            {
                if (condition != null)
                {
                    if (condition( effect ))
                        effect.Draw();
                }
                else
                    effect.Draw();
            }
        }

        internal static void Clear ()
        {
            BaseGame.AnimatedMgr.Clear();
            managedEffects.Clear();
        }


    }
}
