namespace GameJam.Core

open Stride.Engine
open GameJam.Core.Message

module GameEvent =
    let map ((message, entity) : string * Entity) : GameMsg list = 
        match message with
        | "Collect" -> 
            [GameplaySceneMsg(PlayerMsg(Collision(entity))); GameplaySceneMsg(UiMsg(Increment)); Collect]
        | "Left" -> [GameplaySceneMsg(PlayerMsg(MoveLeft))]
        | "Right" -> [GameplaySceneMsg(PlayerMsg(MoveRight))]
        | "Up" -> [GameplaySceneMsg(PlayerMsg(MoveUp))]
        | "Down" -> [GameplaySceneMsg(PlayerMsg(MoveDown))]
        | "Jump" -> [GameplaySceneMsg(PlayerMsg(Jump))]
        | "Grounded" -> [GameplaySceneMsg(PlayerMsg(Grounded))]
        | "Airborne" -> [GameplaySceneMsg(PlayerMsg(Airborne))]
        | "NoMovement" -> [GameplaySceneMsg(PlayerMsg(NoMovement))]
        | "AttachPlayer" -> [GameplaySceneMsg(PlatformMsg(AttachPlayer(entity)))]
        | "DetachPlayer" -> [GameplaySceneMsg(PlatformMsg(DetachPlayer(entity)))]
        | "Goal" -> [Goal]
        | "Start" -> [TitleSceneMsg(Start)]
        | "Restart" -> [Restart]
        | _ -> []