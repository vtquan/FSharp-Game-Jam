using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;

namespace GameJam
{
    public class ScoreSceneScript : SyncScript
    {
        // Declared public member fields and properties will show in the game studio

        public override void Start()
        {
            // Initialization of the script.
            var uiComponent = Entity.Get<UIComponent>();
            var restartButton = uiComponent.Page.RootElement.FindVisualChildOfType<ButtonBase>("RestartButton");

            restartButton.Click += delegate
            {
                Events.GameEventKey.Broadcast(new Tuple<string, Entity>("Restart", Entity));
            };
        }

        public override void Update()
        {
            // Do stuff every new frame
        }
    }
}
