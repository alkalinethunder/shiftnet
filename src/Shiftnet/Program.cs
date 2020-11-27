using System;
using AlkalineThunder.Pandemic;

namespace Shiftnet
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // NEW ENGINE
            using var app = new ShiftnetGameApp();
            app.Run();
        }
    }
}
