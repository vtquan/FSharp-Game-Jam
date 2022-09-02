namespace GameJam.Core

open Stride.Core.Collections
open Stride.Core.Mathematics
open Stride.Engine
open Stride.UI;
open Stride.UI.Controls;
open Stride.Games;
open Stride.Physics
open System.Linq
open Stride.Rendering.Sprites
open Stride.Input
open System
open Stride.Core.Serialization.Contents

module SceneManager =    
    type CurrentScene =
        | Title
        | GamePlay
        | Score
        | Load
    
    type Model =
        { Game : Game; CurrentScene: CurrentScene; NewScene : CurrentScene option}

    type SceneManagerMsg =
        | SwitchScene of CurrentScene
    
    let map message = 
        match message with
        | "Title" -> [SwitchScene(Title)]
        | "GamePlay" -> [SwitchScene(GamePlay)]
        | "Score" -> [SwitchScene(Score)]
        | "Load" -> [SwitchScene(Load)]
        | _ -> []

    let empty = 
        { Game = new Game(); CurrentScene = Title; NewScene = None; }

    let init game = 
        { empty with Game = game }
    
    let update cmd model (deltaTime : float32) =
        match cmd with
        | SwitchScene(s) -> 
            { model with NewScene = Some(s) }, []

    let view model =
        match model.NewScene with
        | Some(s) -> 
            let oldScene = model.Game.Content.Load<Scene>(model.CurrentScene.ToString())
            let newScene = model.Game.Content.Load<Scene>(s.ToString())
            let model = { model with CurrentScene = s; NewScene = None }

            model.Game.SceneSystem.SceneInstance.RootScene.Children.Remove(oldScene) |> ignore
            model.Game.Content.Unload(oldScene)
            model.Game.SceneSystem.SceneInstance.RootScene.Children.Add(newScene)
            match s with
            | Title ->
               ()
            | GamePlay ->
                GameJam.Events.MusicEventKey.Broadcast("Gameplay");
                model.Game.Input.LockMousePosition()
                model.Game.IsMouseVisible <- false
            | Score ->
                GameJam.Events.MusicEventKey.Broadcast("Score");
                model.Game.Input.UnlockMousePosition()
                model.Game.IsMouseVisible <- true
            | _ -> ()
        | _ -> ()