using System;
using System.Collections.Generic;
using System.Text;
using GameBase.Graphics;

namespace Platform.GameDraw.SceneEffects
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
            AnimatedManager.Draw();

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
            AnimatedManager.Clear();
            managedEffects.Clear();
        }


    }
}
