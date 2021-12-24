using System;

namespace Assignment1
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Assignment1())
                game.Run();
        }
    }
}
