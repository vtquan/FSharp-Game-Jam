namespace GameJam.Core

open Stride.Core.Mathematics
open Stride.Engine;
open Stride.Engine.Events
open Stride.Games;
open SceneManager
open System
open System.Linq

module Game =
    type Model =
        {
            SceneManagerModel : SceneManager.Model
            GameplayModel : GameplayScene.Model
            ScoreModel : ScoreScene.Model
            StartTime: TimeSpan
            CurrentTime: TimeSpan list
            BestTime: TimeSpan list
        }

    type Msg =
        | SceneManagerMsg of SceneManager.SceneManagerMsg
        | TitleSceneMsg of TitleScene.TitleSceneMsg
        | GameplaySceneMsg of GameplayScene.GameplaySceneMsg
        | ScoreSceneMsg of ScoreScene.ScoreSceneMsg
        | Restart

    let empty =
        { SceneManagerModel = SceneManager.empty; GameplayModel = GameplayScene.empty; ScoreModel = ScoreScene.empty; StartTime = TimeSpan.Zero; CurrentTime = []; BestTime = []}
        
    let init (game : Game) : Model * Msg list =
        { empty with SceneManagerModel = SceneManager.init game }, []

    let private mapEvent (eventReceiver : EventReceiver<'a>) (eventMap : 'a -> 'b list) (listMap : 'b list -> Msg list) =   
        let eventList = (Seq.empty).ToList()
        let numEvent = eventReceiver.TryReceiveAll(eventList)
        let events = Seq.toList eventList
        let messages =
            [
                for e in events do
                    yield! listMap (eventMap e)
            ]
        messages

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

    let mapAllEvent () : Msg list =
        let messages =
            [
                yield! mapEvent GameJam.Events.sceneManagerEvent SceneManager.map (List.map SceneManagerMsg)
                yield! mapEvent GameJam.Events.titleSceneEvent TitleScene.map (List.map TitleSceneMsg)
                yield! List.map GameplaySceneMsg (GameplayScene.mapAllEvent ())
            ] |> List.distinct
        messages

    let view (gameModel : Model) (gameTime : GameTime) =
        let deltaTime = float32 gameTime.Elapsed.TotalSeconds
        SceneManager.view gameModel.SceneManagerModel
        match gameModel.SceneManagerModel.CurrentScene with
        | Title -> 
            ()
        | GamePlay -> 
            GameplayScene.view gameModel.GameplayModel gameTime
        | Score -> 
            ScoreScene.view gameModel.ScoreModel gameTime
        | Load -> 
            ()

    let update (gameModel : Model) (cmds : Msg list) (gameTime : GameTime) (game : Game) =
        let deltaTime = float32 gameTime.Elapsed.TotalSeconds

        let updateFold ((gameModel, msgs) : Model * Msg list) cmd = 
            match cmd with
            | SceneManagerMsg(m) -> 
                let (model,msg) = SceneManager.update m gameModel.SceneManagerModel deltaTime
                { gameModel with SceneManagerModel = model }, msgs @ (List.map SceneManagerMsg msg)
            | TitleSceneMsg(m) ->
                match m with
                | Start -> 
                    GameJam.Events.SceneManagerEventKey.Broadcast("GamePlay")
                    let gameplayScene = game.Content.Load<Scene>("GameplayScene")
                    let (gameplaySceneModel,gameplaySceneInitMessage) = GameplayScene.init gameplayScene game.Input
                    { gameModel with GameplayModel = gameplaySceneModel; StartTime = gameTime.Total; CurrentTime = [] }, msgs @ (List.map GameplaySceneMsg gameplaySceneInitMessage)           
            | GameplaySceneMsg(m) -> 
                let (model,msg) = GameplayScene.update m gameModel.GameplayModel gameTime
                { gameModel with GameplayModel = model }, msgs @ (List.map GameplaySceneMsg msg)
            | ScoreSceneMsg(m) -> 
                let (model,msg) = ScoreScene.update m gameModel.ScoreModel deltaTime game
                { gameModel with ScoreModel = model }, msgs @ (List.map ScoreSceneMsg msg)
            | Restart -> 
                let scoreScene = game.Content.Load<Scene>("ScoreScene")
                game.SceneSystem.SceneInstance.RootScene.Children.Remove(scoreScene) |> ignore
                game.Content.Unload(scoreScene)
                let titleScene = game.Content.Load<Scene>("TitleScene")
                game.SceneSystem.SceneInstance.RootScene.Children.Add(titleScene)
                let bestTime = 
                    if gameModel.BestTime.Length > 0 then
                        if gameModel.CurrentTime.Item(1) > gameModel.BestTime.Item(1) then gameModel.CurrentTime else gameModel.BestTime
                    else
                        gameModel.CurrentTime
                GameJam.Events.SceneManagerEventKey.Broadcast("Title")
                { gameModel with BestTime = bestTime}, []

        let newModel, newMessages = List.fold updateFold (gameModel, []) cmds
        newModel , List.distinct newMessages