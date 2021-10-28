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
    public class PlayMusic : AsyncScript
    {
        // Declared public member fields and properties will show in the game studio

        public override async Task Execute()
        {
            // Load the sound
            Sound musicSound = Content.Load<Sound>("Audio/GameplayMusic");

            // Create a sound instance
            SoundInstance music = musicSound.CreateInstance();

            // Loop
            music.IsLooping = true;

            // Set the volume
            music.Volume = 0.15f;

            await music.ReadyToPlay();

            // Play the music
            music.Play();
        }
    }
}
