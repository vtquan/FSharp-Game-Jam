using Stride.Engine;
using Stride.UI.Controls;
using Stride.UI;

namespace GameJam
{
    public class ScoreSceneScript : SyncScript
    {
        // Declared public member fields and properties will show in the game studio

        public override void Start()
        {
            // Initialization of the script.
            var uiComponent = Entity.Get<UIComponent>();
            var startButton = uiComponent.Page.RootElement.FindVisualChildOfType<ButtonBase>("RestartButton");

            startButton.Click += delegate
            {
                Events.GameEventKey.Broadcast("Restart");
            };
        }

        public override void Update()
        {
            // Do stuff every new frame
        }
    }
}
