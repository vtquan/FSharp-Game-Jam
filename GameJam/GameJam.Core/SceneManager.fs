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
open Stride.Engine.Events
open EventHelper

module SceneManager =    
    type CurrentScene =
        | Title
        | Gameplay
        | Score
        | Load
    
    type Model =
        { 
            Game : Game; 
            CurrentScene: CurrentScene; 
            NewScene : CurrentScene option
            TitleSceneModel : TitleScene.Model
            GameplaySceneModel : GameplayScene.Model
            ScoreSceneModel : ScoreScene.Model
        }

    type SceneManagerMsg =
        | SwitchScene of CurrentScene
        | SceneSwitched
        | TitleSceneMsg of TitleScene.TitleSceneMsg
        | GameplaySceneMsg of GameplayScene.GameplaySceneMsg
        | ScoreSceneMsg of ScoreScene.ScoreSceneMsg

    let empty = 
        { Game = new Game(); CurrentScene = Title; NewScene = None; TitleSceneModel = TitleScene.empty; GameplaySceneModel = GameplayScene.empty; ScoreSceneModel = ScoreScene.empty; }

    let init game = 
        { empty with Game = game }

    let update cmd model (gameTime: GameTime) =
        let deltaTime = float32 gameTime.Elapsed.TotalSeconds
        match cmd with
        | SwitchScene(s) -> 
            { model with NewScene = Some(s) }, [SceneSwitched]
        | SceneSwitched -> 
            match model.NewScene with
            | Some(s) -> 
                match s with
                | Title -> 
                    let titleScene = model.Game.Content.Load<Scene>("TitleScene")
                    let (titleSceneModel, titleSceneInitMessage) = TitleScene.init model.Game
                    { model with TitleSceneModel = titleSceneModel; CurrentScene = Title; NewScene = None }, (List.map TitleSceneMsg titleSceneInitMessage)    
                | Gameplay ->                     
                    let gameplayScene = model.Game.Content.Load<Scene>("GameplayScene")
                    let (gameplaySceneModel,gameplaySceneInitMessage) = GameplayScene.init gameplayScene model.Game.Input
                    { model with GameplaySceneModel = gameplaySceneModel; CurrentScene = Gameplay; NewScene = None }, (List.map GameplaySceneMsg gameplaySceneInitMessage)    
                | Score ->            
                    let scoreScene = model.Game.Content.Load<Scene>("ScoreScene")
                    let (scoreSceneModel, scoreSceneInitMessage) = ScoreScene.init model.GameplaySceneModel.ScoreModel.Records model.GameplaySceneModel.ScoreModel.Records scoreScene
                    { model with ScoreSceneModel = scoreSceneModel; CurrentScene = Score; NewScene = None }, (List.map ScoreSceneMsg scoreSceneInitMessage)    
                | Load -> 
                    model, []
            | None ->
                model, []
        | TitleSceneMsg(m) ->            
            let (newModel,msg) = TitleScene.update m model.TitleSceneModel deltaTime
            { model with TitleSceneModel = newModel }, (List.map TitleSceneMsg msg)
        | GameplaySceneMsg(m) -> 
            let (newModel,msg) = GameplayScene.update m model.GameplaySceneModel gameTime
            { model with GameplaySceneModel = newModel }, (List.map GameplaySceneMsg msg)
        | ScoreSceneMsg(m) -> 
            let (newModel,msg) = ScoreScene.update m model.ScoreSceneModel deltaTime model.Game
            { model with ScoreSceneModel = newModel }, (List.map ScoreSceneMsg msg)

    let view model gameTime =
        match model.NewScene with
        | Some(s) -> 
            let oldSceneName = model.CurrentScene.ToString() + "Scene"
            let newSceneName = s.ToString() + "Scene"
            let oldScene = model.Game.Content.Load<Scene>(oldSceneName)
            let newScene = model.Game.Content.Load<Scene>(newSceneName)
            
            
            model.Game.SceneSystem.SceneInstance.RootScene.Children.Remove(oldScene) |> ignore
            model.Game.Content.Unload(oldScene)
            oldScene.Dispose()
            model.Game.SceneSystem.SceneInstance.RootScene.Children.Add(newScene)

            match s with
            | Title ->
                GameJam.Events.MusicEventKey.Broadcast("Title");
            | Gameplay ->         
                GameJam.Events.MusicEventKey.Broadcast("Gameplay");
                model.Game.Input.LockMousePosition()
                model.Game.IsMouseVisible <- false
            | Score ->
                GameJam.Events.MusicEventKey.Broadcast("Score");
                model.Game.Input.UnlockMousePosition()
                model.Game.IsMouseVisible <- true
            | _ -> 
                ()
        | _ -> 
            match model.CurrentScene with
            | Title -> 
                ()
            | Gameplay -> 
                GameplayScene.view model.GameplaySceneModel gameTime
            | Score -> 
                ScoreScene.view model.ScoreSceneModel gameTime
            | Load -> 
                ()
    
    let map message = 
        match message with
        | "Title" -> [SwitchScene(Title)]
        | "Gameplay" -> [SwitchScene(Gameplay)]
        | "Score" -> [SwitchScene(Score)]
        | "Load" -> [SwitchScene(Load)]
        | _ -> []

    let getMsg () : SceneManagerMsg list =
        [
            yield! recieveEvent GameJam.Events.sceneManagerEvent map
            yield! List.map TitleSceneMsg (TitleScene.getMsg ())
            yield! List.map GameplaySceneMsg  <| GameplayScene.getMsg ()
        ]
            