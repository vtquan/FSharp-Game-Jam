namespace GameJam.Core.Message

type GameplaySceneMsg =
    | PlayerMsg of PlayerMsg
    | PlatformMsg of PlatformMsg
    | CollectibleMsg of CollectibleMsg
    | GoalMsg of GoalMsg
    | UiMsg of UiMsg