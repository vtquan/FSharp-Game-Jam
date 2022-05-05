namespace GameJam.Core

open Stride.Engine
open GameJam.Core.Message

module StringEvent =
    let private map (message : string) : GameMsg list = 
        match message with
        //| "Left" -> [GameplaySceneMsg(PlayerMsg(MoveLeft))]
        //| "Right" -> [GameplaySceneMsg(PlayerMsg(MoveRight))]
        //| "Up" -> [GameplaySceneMsg(PlayerMsg(MoveUp))]
        //| "Down" -> [GameplaySceneMsg(PlayerMsg(MoveDown))]
        //| "Jump" -> [GameplaySceneMsg(PlayerMsg(Jump))]
        | _ -> []