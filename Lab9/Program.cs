using System;

namespace Lab9
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Lab9())
                game.Run();
        }
    }
}
