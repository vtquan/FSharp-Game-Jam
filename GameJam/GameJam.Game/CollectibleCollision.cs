using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Games;

namespace GameJam
{
    public class CollectibleCollision : AsyncScript
    {
        // Declared public member fields and properties will show in the game studio
        protected Game MvuGame;

        public override async Task Execute()
        {
            MvuGame = (Game)Services.GetService<IGame>();

            var trigger = Entity.Get<PhysicsComponent>();

            while (Game.IsRunning)
            {
                var firstCollision = await trigger.NewCollision();
                Events.PlayerEventKey.Broadcast(new Tuple<string, Entity>("Collect", Entity));
                //await Script.NextFrame();
            }
        }
    }
}
