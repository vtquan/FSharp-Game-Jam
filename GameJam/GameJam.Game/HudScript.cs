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
    public class HudScript : SyncScript
    {
        // Declared public member fields and properties will show in the game studio
        int Counter = 0;

        public override void Start()
        {
            // Initialization of the script.
        }

        public override void Update()
        {
            // Do stuff every new frame
            //List<Tuple<string, Entity>> result = new List<Tuple<string, Entity>>();
            //int result2 = Events.gameListener.TryReceiveAll(collection: result);
            //if (result2 > 0)
            //{
            //    if(result.Exists(x => x.Item1 == "Collect"))
            //    {
            //        Counter++;

            //        var page = Entity.Get<UIComponent>().Page;
            //        var rootElement = page.RootElement;
            //        var bonusCounter = rootElement.FindVisualChildOfType<TextBlock>("CollectionCounter");
            //        bonusCounter.Text = Counter.ToString();
            //    }
            //}
        }
    }
}
