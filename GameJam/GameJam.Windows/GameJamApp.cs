using Stride.Engine;

namespace GameJam
{
    class GameJamApp
    {
        static void Main(string[] args)
        {
            using (var game = new Core.MvuGame())
            {
                game.Run();
            }
        }
    }
}
