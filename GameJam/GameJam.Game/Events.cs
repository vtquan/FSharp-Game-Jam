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
        public static readonly EventKey<string> GameEventKey = new();
        public static readonly EventReceiver<string> gameEvent = new(GameEventKey, EventReceiverOptions.Buffered);

        public static readonly EventKey<Tuple<string, Entity>> PlayerEventKey = new();
        public static readonly EventReceiver<Tuple<string, Entity>> playerEvent = new(PlayerEventKey, EventReceiverOptions.Buffered);
        
        public static readonly EventKey<Tuple<string, Entity>> PlatformEventKey = new();
        public static readonly EventReceiver<Tuple<string, Entity>> platformEvent = new(PlatformEventKey, EventReceiverOptions.Buffered);
        
        public static readonly EventKey<string> UiEventKey = new();
        public static readonly EventReceiver<string> uiEvent = new(UiEventKey, EventReceiverOptions.Buffered);
        
        public static readonly EventKey<string> CollectibleEventKey = new();
        public static readonly EventReceiver<string> collectibleEvent = new(CollectibleEventKey, EventReceiverOptions.Buffered);
        
        public static readonly EventKey<string> ScoreEventKey = new();
        public static readonly EventReceiver<string> scoreEvent = new(ScoreEventKey, EventReceiverOptions.Buffered);
        
        public static readonly EventKey<string> GoalEventKey = new();
        public static readonly EventReceiver<string> goalEvent = new(GoalEventKey, EventReceiverOptions.Buffered);

        public static readonly EventKey<string> MusicEventKey = new();
        public static readonly EventReceiver<string> musicEvent = new(MusicEventKey);

        public static readonly EventKey<string> SceneManagerEventKey = new();
        public static readonly EventReceiver<string> sceneManagerEvent = new(SceneManagerEventKey);

        public static readonly EventKey<string> TitleSceneEventKey = new();
        public static readonly EventReceiver<string> titleSceneEvent = new(TitleSceneEventKey);

        public static readonly EventKey<string> ScoreSceneEventKey = new();
        public static readonly EventReceiver<string> scoreSceneEvent = new(ScoreSceneEventKey);

        public static readonly EventKey SfxEventKey = new();
        public static readonly EventReceiver sfxEvent = new(SfxEventKey);

        public static readonly EventKey<Vector3> MoveDirectionEventKey = new EventKey<Vector3>();
        public static readonly EventReceiver<Vector3> moveDirectionEvent = new EventReceiver<Vector3>(MoveDirectionEventKey);

        public static readonly EventKey<Vector2> CameraDirectionEventKey = new EventKey<Vector2>();
        public static readonly EventReceiver<Vector2> cameraDirectionEvent = new(CameraDirectionEventKey);

        public static readonly EventKey<float> RunSpeedEventKey = new EventKey<float>();
        public static readonly EventReceiver<float> runSpeedEvent = new EventReceiver<float>(RunSpeedEventKey);

        public static readonly EventKey<bool> IsGroundedEventKey = new EventKey<bool>();
        public static readonly EventReceiver<bool> isGroundedEvent = new EventReceiver<bool>(IsGroundedEventKey);
        
        public static readonly EventKey<bool> JumpEventKey = new();
        public static readonly EventReceiver<bool> jumpEvent = new EventReceiver<bool>(JumpEventKey);
    }
}
