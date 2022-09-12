using System;

namespace Project_Fisticuffs
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();

            // TO-DO:
            // Implement basic finite state machine for menu to move through placeholder screens of:
                // main menu
                // character select (?)
                // settings menu
                // game level
                // pause menu
        }
    }
}
