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

    let init = {Game = new Game()}
    
    let update cmd model (deltaTime : float32) =
        match cmd with
        | Start -> 
            let titleScene = model.Game.Content.Load<Scene>("TitleScene")
            model.Game.SceneSystem.SceneInstance.RootScene.Children.Remove(titleScene) |> ignore
            model.Game.Content.Unload(titleScene)
            let gameplayScene = model.Game.Content.Load<Scene>("GameplayScene")
            model.Game.SceneSystem.SceneInstance.RootScene.Children.Add(gameplayScene)
            GameJam.Events.MusicEventKey.Broadcast("Gameplay");
            model.Game.Input.LockMousePosition()
            model.Game.IsMouseVisible <- false