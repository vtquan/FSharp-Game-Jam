using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam
{
    public static class Events
    {

        public static readonly EventKey<Vector3> MoveDirectionEventKey = new EventKey<Vector3>();
        public static readonly EventKey<Vector2> CameraDirectionEventKey = new EventKey<Vector2>();
        public static readonly EventKey<bool> IsGroundedEventKey = new EventKey<bool>();
        public static readonly EventKey<float> RunSpeedEventKey = new EventKey<float>();
        public static readonly EventKey<bool> JumpEventKey = new();

        public static readonly EventKey<Tuple<string, Entity>> GameEventKey = new();
        public static readonly EventReceiver<Tuple<string, Entity>> gameListener = new(GameEventKey, EventReceiverOptions.Buffered);

        public static readonly EventKey<string> MusicEventKey = new();
        public static readonly EventReceiver<string> musicEvent = new EventReceiver<string>(MusicEventKey);

        public static readonly EventKey SfxEventKey = new();
        public static readonly EventReceiver sfxEvent = new EventReceiver(SfxEventKey);


        public static readonly EventReceiver<Vector3> moveDirectionEvent = new EventReceiver<Vector3>(MoveDirectionEventKey);
        public static readonly EventReceiver<float> runSpeedEvent = new EventReceiver<float>(RunSpeedEventKey);
        public static readonly EventReceiver<bool> isGroundedEvent = new EventReceiver<bool>(IsGroundedEventKey);

        public static readonly EventReceiver<bool> jumpEvent = new EventReceiver<bool>(JumpEventKey);
    }
}
