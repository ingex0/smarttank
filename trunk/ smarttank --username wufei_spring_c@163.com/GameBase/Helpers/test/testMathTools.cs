using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameBase.Helpers.test
{
    public class testMathTools
    {
        public static void Run ()
        {
            float[] test = new float[]
            {
                370f,
                -370f,
                190f,
                -190f,
            };

            foreach (float degree in test)
            {
                Console.WriteLine( MathHelper.ToDegrees( MathTools.AngTransInPI( MathHelper.ToRadians( degree ) ) ) );
            }
        }

    }
}
