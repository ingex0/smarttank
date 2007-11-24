using System;
using System.Collections.Generic;
using System.Text;
using GameBase.Helpers;

namespace GameBase.DataStructure.test
{
    public class TestFastLinkedList
    {
        class A
        {
            public int i;
            public A ( int i )
            {
                this.i = i;
            }
        }

        class B : A
        {
            public B ( int i )
                : base( i )
            {

            }
        }

        class C
        {
            public float f;
        }

        public static void Run ()
        {
            Log.Initialize();

            FastLinkedList<B> list = new FastLinkedList<B>();
            B b1 = new B( 1 );
            list.AddLast( b1 );
            list.AddLast( b1 );

            list.AddLast( new B( 2 ) );
            list.AddLast( new B( 3 ) );
            list.AddLast( new B( 4 ) );
            list.AddLast( new B( 5 ) );
            list.AddLast( new B( 6 ) );
            list.AddLast( new B( 7 ) );
            foreach (B b in list)
            {
                Console.WriteLine( b.i );
            }

            list.Remove( b1 );

            foreach (B b in list)
            {
                Console.WriteLine( b.i );
            }
        }

    }
}
