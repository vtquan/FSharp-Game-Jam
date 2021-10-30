namespace GameJam.Core
open Stride.Engine



module Messages =   
    
    type PlayerMsg = 
        | MoveLeft
        | MoveRight
        | MoveUp
        | MoveDown
        | NoMovement
        | NewVelocity
        | Jump
        | Grounded
        | Airborne
        | Collision of Entity
        
    type PlatformMsg = 
        | Countdown
        | Countup
        | AttachPlayer of Entity
        | DetachPlayer of Entity

    type GoalMsg = 
        | Activate
        | Rotate
        
    type UiMsg = 
        | Increment
        
    type TitleSceneMsg =
        | Start

    type GameplaySceneMsg =
        | PlayerMsg of PlayerMsg
        | PlatformMsg of PlatformMsg
        | GoalMsg of GoalMsg
        | UiMsg of UiMsg
        
    type ScoreSceneMsg =
        | Restart 

    type GameMsg =
        | TitleSceneMsg of TitleSceneMsg
        | GameplaySceneMsg of GameplaySceneMsg
        | ScoreSceneMsg of ScoreSceneMsg
        | Collect
        | Restart 
        | Goal
        | Empty
        