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
    public class PlaySfx : AsyncScript
    {
        // Declared public member fields and properties will show in the game studio

        public override async Task Execute()
        {
            while(Game.IsRunning)
            {
                // Load the sound
                Sound musicSound = Content.Load<Sound>("Audio/sfx");

                // Create a sound instance
                SoundInstance music = musicSound.CreateInstance();

                // Loop
                music.IsLooping = false;

                // Set the volume
                music.Volume = 1f;

                await music.ReadyToPlay();

                await Events.sfxEvent.ReceiveAsync();
                music.Play();
            }
        }
    }
}
