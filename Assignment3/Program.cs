using System;

namespace Assignment3
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Assignment3())
                game.Run();
        }
    }
}
