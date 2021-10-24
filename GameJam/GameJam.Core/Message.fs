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
        | Vertical of Entity
        | Clear of Entity
        | Rise of Entity
        | Lower of Entity
        | AttachPlayer of Entity
        | DetachPlayer of Entity

    type GameMsg =
        | PlayerMsg of PlayerMsg
        | PlatformMsg of PlatformMsg
        | Start
        | Restart 
        | Empty
