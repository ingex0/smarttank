using System;
using System.Collections.Generic;
using System.Text;
using GameBase.Helpers;

namespace GameBase.DataStructure.test
{
    public class MultiListTest
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

            MultiLinkedList<B> list = new MultiLinkedList<B>();
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

            Console.WriteLine();
            Console.WriteLine();

            IEnumerable<A> listA = list.GetConvertList<A>();

            foreach (A a in listA)
            {
                Console.WriteLine( a.i );
            }

            //list.Remove( list.FindFirst(
            //    delegate( B b )
            //    {
            //        if (b.i == 1)
            //            return true;
            //        else
            //            return false;
            //    } ) );
            //list.ForEach(
            //    delegate( ref B b )
            //    {
            //        b.i *= 2;
            //    } );
            list.Remove( b1 );
            list.Remove( b1 );


            Console.WriteLine();
            Console.WriteLine();

            foreach (B b in list)
            {
                Console.WriteLine( b.i );
            }

            Console.WriteLine();
            Console.WriteLine();

            foreach (A a in listA)
            {
                Console.WriteLine( a.i );
            }

            // 这个调用会引发异常，这是合理的。
            //list.GetConvertList<C>();
            Console.WriteLine( "test empty list" );
            Console.WriteLine();


            MultiLinkedList<A> empty = new MultiLinkedList<A>();
            foreach (A a in empty)
            {
                Console.WriteLine( a.i );
            }
            A tempA = new A( 1 );
            empty.AddLast( tempA );
            empty.Remove( tempA );
            foreach (A a in empty)
            {
                Console.WriteLine( a.i );
            }
            A tempA2 = new A(2);
            empty.AddLast( tempA2 );
            foreach (A a in empty)
            {
                Console.WriteLine( a.i );
            }
        }
    }
}
