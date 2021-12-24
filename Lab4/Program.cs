using System;

namespace Lab4
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Lab4())
                game.Run();
        }
    }
}
