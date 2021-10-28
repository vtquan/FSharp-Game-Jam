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
        
    type UiMsg = 
        | Increment

    type GameMsg =
        | PlayerMsg of PlayerMsg
        | PlatformMsg of PlatformMsg
        | UiMsg of UiMsg
        | Start
        | Restart 
        | Empty
