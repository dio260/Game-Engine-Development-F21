using System;

namespace Assignment5
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Assignment5())
                game.Run();
        }
    }
}
