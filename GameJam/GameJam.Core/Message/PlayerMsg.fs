namespace GameJam.Core.Message
open Stride.Engine

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