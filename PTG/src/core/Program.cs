using System;

namespace PTG.core
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
			using var game = new PtgGame();
			game.Run();
		}
    }
}
