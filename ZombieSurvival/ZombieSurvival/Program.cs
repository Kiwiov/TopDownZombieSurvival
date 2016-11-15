using System;

namespace ZombieSurvival
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// private States gameStates;

        
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();

        }
    }
#endif
}
