using System;

namespace SmartTank
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main( string[] args )
        {
            using (GameManager game = new GameManager())
            {
                game.Run();
            }
        }
    }
}

