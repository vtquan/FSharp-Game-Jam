using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Audio;

namespace GameJam
{
    public class PlayAudio : SyncScript
    {
        // Declared public member fields and properties will show in the game studio

        public override void Start()
        {
            // Initialization of the script.
            AudioEmitterComponent audioEmitterComponent = Entity.Get<AudioEmitterComponent>();
            AudioEmitterSoundController mySound1Controller = audioEmitterComponent["ConstantEmitter"];

            mySound1Controller.IsLooping = true;
            mySound1Controller.Pitch = 2.0f;
            mySound1Controller.Volume = 0.5f;
            mySound1Controller.Play();
        }

        public override void Update()
        {
            // Do stuff every new frame
        }
    }
}
