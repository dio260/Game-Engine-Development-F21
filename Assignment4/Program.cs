using System;

namespace Assignment4
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Assignment4())
                game.Run();
        }
    }
}
