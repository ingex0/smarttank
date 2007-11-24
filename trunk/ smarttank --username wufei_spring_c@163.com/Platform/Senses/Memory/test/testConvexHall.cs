using System;
using System.Collections.Generic;
using System.Text;
using GameBase.DataStructure;
using Microsoft.Xna.Framework;

namespace Platform.Senses.Memory.test
{
    public class testConvexHall
    {
        static public void Run ()
        {
            CircleList<BordPoint> testBord = new CircleList<BordPoint>();

            testBord.AddLast( new BordPoint( 0, new Point( 3, 3 ) ) );
            testBord.AddLast( new BordPoint( 1, new Point( 0, 2 ) ) );
            testBord.AddLast( new BordPoint( 2, new Point( 1, 5 ) ) );
            testBord.AddLast( new BordPoint( 3, new Point( 3, 4 ) ) );
            testBord.AddLast( new BordPoint( 4, new Point( 4, 5 ) ) );
            testBord.AddLast( new BordPoint( 5, new Point( 5, 4 ) ) );
            testBord.AddLast( new BordPoint( 6, new Point( 6, 3 ) ) );
            testBord.AddLast( new BordPoint( 7, new Point( 5, 2 ) ) );
            testBord.LinkLastAndFirst();

            ConvexHall test = new ConvexHall( testBord, 0 );

        }
    }
}
