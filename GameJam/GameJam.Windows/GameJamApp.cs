using Stride.Engine;

namespace GameJam
{
    class GameJamApp
    {
        static void Main(string[] args)
        {
            using (var game = new Core.Game.MvuGame())
            {
                game.Run();
            }
        }
    }
}
