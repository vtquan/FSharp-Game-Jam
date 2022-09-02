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

module ScoreScene =
    type Model =
        {
            ScoreUiModel : ScoreUI.Model
        }
    
    type ScoreSceneMsg =
        | Start
        | Restart
    
    let map message = 
        match message with
        | "Start" -> [Start]
        | "Restart" -> [Start]
        | _ -> []

    let empty =
        { ScoreUiModel = ScoreUI.empty () }

    let init currentTime bestTime (scene : Scene) =
        let (newUiModel,UiMessage) = ScoreUI.init currentTime bestTime (scene)
        { empty with ScoreUiModel = newUiModel }, [UiMessage]
    
    let update cmd state (deltaTime : float32) (game : Game) =
        match cmd with
        | Start -> 
            game.Input.UnlockMousePosition()
            game.IsMouseVisible <- true
                    
            let gameplayScene = game.Content.Load<Scene>("GameplayScene")
            game.SceneSystem.SceneInstance.RootScene.Children.Remove(gameplayScene) |> ignore
            game.Content.Unload(gameplayScene)
            let scoreScene = game.Content.Load<Scene>("ScoreScene")
            game.SceneSystem.SceneInstance.RootScene.Children.Add(scoreScene)
            GameJam.Events.MusicEventKey.Broadcast("Score");
            state, []  
        | Restart -> state, []

    let view (state : Model) (gameTime : GameTime) =
        ScoreUI.view state.ScoreUiModel (float32 gameTime.Elapsed.TotalSeconds)

