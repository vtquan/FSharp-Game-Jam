namespace GameJam.Core.Message
open Stride.Engine

type GameMsg =
    | TitleSceneMsg of TitleSceneMsg
    | GameplaySceneMsg of GameplaySceneMsg
    | ScoreSceneMsg of ScoreSceneMsg
    | Collect
    | Restart 
    | Goal
    | Empty