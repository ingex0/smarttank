using System;

namespace GameTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main( string[] args )
        {
            using (GameTest game = new GameTest())
            {
                game.Run();
            }
        }
    }
}

