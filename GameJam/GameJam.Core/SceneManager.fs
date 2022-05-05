namespace GameJam.Core

open Stride.Core.Collections
open Stride.Core.Mathematics
open Stride.Engine
open Stride.UI;
open Stride.UI.Controls;
open Stride.Games;
open Stride.Physics
open System.Linq
open GameJam.Core.Message
open Stride.Rendering.Sprites
open Stride.Input
open System
open Stride.Core.Serialization.Contents

module SceneManager =
    
    
    type Model =
        {Game : Game; CurrentScene: CurrentScene; NewScene : CurrentScene option}

    let init = {Game = new Game(); CurrentScene = Title; NewScene = None;}
    
    let update cmd model (deltaTime : float32) =
        match cmd with
        | SwitchScene(s) -> 
            { model with NewScene = Some(s)}

    let view model =
        match model.NewScene with
        | Some(s) -> 
            let oldScene = model.Game.Content.Load<Scene>(model.CurrentScene.ToString())
            model.Game.SceneSystem.SceneInstance.RootScene.Children.Remove(oldScene) |> ignore
            model.Game.Content.Unload(oldScene)
            let newScene = model.Game.Content.Load<Scene>(s.ToString())
            model.Game.SceneSystem.SceneInstance.RootScene.Children.Add(newScene)
            match s with
            | GamePlay ->
                GameJam.Events.MusicEventKey.Broadcast("Gameplay");
                model.Game.Input.LockMousePosition()
                model.Game.IsMouseVisible <- false
            | _ -> ()
        | _ -> ()