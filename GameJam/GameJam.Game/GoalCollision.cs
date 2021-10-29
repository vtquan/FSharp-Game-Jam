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
    public class GoalCollision : AsyncScript
    {
        // Declared public member fields and properties will show in the game studio
        protected Game MvuGame;

        public override async Task Execute()
        {
            MvuGame = (Game)Services.GetService<IGame>();

            var trigger = Entity.Get<PhysicsComponent>();
            trigger.ProcessCollisions = true; // Only the second static collider will attach the player. Don't attach player for running into the side of the platform

            while (Game.IsRunning)
            {
                var firstCollision = await trigger.NewCollision();
                Events.GameEventKey.Broadcast(new Tuple<string, Entity>("Goal", Entity));
                //await Script.NextFrame();
            }
        }
    }
}
