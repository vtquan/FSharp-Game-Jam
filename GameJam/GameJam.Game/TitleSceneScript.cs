using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.UI.Panels;
using Stride.UI.Controls;
using Stride.UI;

namespace GameJam
{
    public class TitleSceneScript : SyncScript
    {
        // Declared public member fields and properties will show in the game studio

        public override void Start()
        {
            // Initialization of the script.
            var uiComponent = Entity.Get<UIComponent>();
            var startButton = uiComponent.Page.RootElement.FindVisualChildOfType<ButtonBase>("StartButton");

            startButton.Click += delegate
            {
                Events.TitleSceneEventKey.Broadcast("Start");
            };
        }

        public override void Update()
        {
            // Do stuff every new frame
        }
    }
}
