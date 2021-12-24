using System;

namespace Assignment2
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Assignment2())
                game.Run();
        }
    }
}
