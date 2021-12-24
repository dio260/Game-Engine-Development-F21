using System;

namespace HonorsContract
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new HonorsGame())
                game.Run();
        }
    }
}
