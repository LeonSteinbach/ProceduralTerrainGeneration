using System;

namespace PTG
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new PTGGame())
                game.Run();
        }
    }
}
