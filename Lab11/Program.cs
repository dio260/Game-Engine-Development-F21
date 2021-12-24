using System;

namespace Lab11
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Lab11())
                game.Run();
        }
    }
}
