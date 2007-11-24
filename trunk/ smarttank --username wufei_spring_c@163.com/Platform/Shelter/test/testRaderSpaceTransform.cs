using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platform.Shelter.test
{
    public class testRaderSpaceTransform
    {
        public static void Run ()
        {
            Rader rader = new Rader( MathHelper.PiOver4, 50, new Vector2( 50, 50 ), MathHelper.PiOver4, Color.Yellow );
            Vector2[] pInWorlds = new Vector2[]
            {
                new Vector2(50,50),
                new Vector2(100,0),
                new Vector2(75,25),
                new Vector2(25,75),
                new Vector2(50,25),
                new Vector2(50,75),
                new Vector2(25,50),
                new Vector2(75,50),
                new Vector2(50,0),
                new Vector2(100,50)

            };

            foreach (Vector2 p in pInWorlds)
            {
                Console.WriteLine( rader.TranslateToRaderSpace( p ) );
            }
        }
    }
}
