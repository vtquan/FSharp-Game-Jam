namespace GameJam.Core.Message
open Stride.Engine

type PlatformMsg = 
    | Countdown
    | Countup
    | AttachPlayer of Entity
    | DetachPlayer of Entity