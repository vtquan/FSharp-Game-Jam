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
            GameplaySceneModel : GameplayScene.Model
            ScoreSceneModel : ScoreScene.Model
        }

    type SceneManagerMsg =
        | SwitchScene of CurrentScene
        | SceneSwitched
        | TitleSceneMsg of TitleScene.TitleSceneMsg
        | GameplaySceneMsg of GameplayScene.GameplaySceneMsg
        | ScoreSceneMsg of ScoreScene.ScoreSceneMsg
    
    let map message = 
        match message with
        | "Title" -> [SwitchScene(Title)]
        | "Gameplay" -> [SwitchScene(Gameplay)]
        | "Score" -> [SwitchScene(Score)]
        | "Load" -> [SwitchScene(Load)]
        | _ -> []

    let empty = 
        { Game = new Game(); CurrentScene = Title; NewScene = None; GameplaySceneModel = GameplayScene.empty; ScoreSceneModel = ScoreScene.empty; }

    let init game = 
        { empty with Game = game }

    let private mapEvent2 (eventReceiver : EventReceiver<'a>) (eventMap : 'a -> 'b list) =   
        let eventList = (Seq.empty).ToList()
        let numEvent = eventReceiver.TryReceiveAll(eventList)
        let events = Seq.toList eventList
        let messages =
            [
                for e in events do
                    yield! eventMap e
            ]
        messages

    let private mapEvent (eventReceiver : EventReceiver<'a>) (eventMap : 'a -> 'b list) (listMap : 'b list -> SceneManagerMsg list) =   
        let eventList = (Seq.empty).ToList()
        let numEvent = eventReceiver.TryReceiveAll(eventList)
        let events = Seq.toList eventList
        let messages =
            [
                for e in events do
                    yield! listMap (eventMap e)
            ]
        messages

    let mapAllEvent () : SceneManagerMsg list =
        let messages =
            [
                yield! mapEvent GameJam.Events.titleSceneEvent TitleScene.map (List.map TitleSceneMsg)
                yield! mapEvent2 GameJam.Events.sceneManagerEvent map
                yield! List.map GameplaySceneMsg (GameplayScene.mapAllEvent ())
            ] |> List.distinct
        messages

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
                    model, []
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
            match m with
            | Start -> 
                GameJam.Events.SceneManagerEventKey.Broadcast("Gameplay")
                model, []
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
            model.Game.SceneSystem.SceneInstance.RootScene.Children.Add(newScene)
            match s with
            | Title ->
               ()
            | Gameplay ->         
                GameJam.Events.MusicEventKey.Broadcast("Gameplay");
                model.Game.Input.LockMousePosition()
                model.Game.IsMouseVisible <- false
            | Score ->
                GameJam.Events.MusicEventKey.Broadcast("Score");
                model.Game.Input.UnlockMousePosition()
                model.Game.IsMouseVisible <- true
            | _ -> ()
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
            