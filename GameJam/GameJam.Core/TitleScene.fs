namespace GameJam.Core

open Stride.Engine

module TitleScene =
    type Model =
        {Game : Game}

    type TitleSceneMsg =
        | Start    
    
    let map message = 
        match message with
        | "Start" -> [Start]
        | _ -> []

    let empty = {Game = new Game()}

    let init game = 
        {Game = game}, []
    
    let update cmd model (deltaTime : float32) =
        match cmd with
        | Start -> 
            GameJam.Events.SceneManagerEventKey.Broadcast("Gameplay")
            model, []