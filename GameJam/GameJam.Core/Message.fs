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
        | Collision
        
    type PlatformMsg = 
        | Countdown
        | Countup
        | AttachPlayer of Entity
        | DetachPlayer of Entity

    type GameMsg =
        | PlayerMsg of PlayerMsg
        | PlatformMsg of PlatformMsg
        | Start
        | Restart 
        | Empty
