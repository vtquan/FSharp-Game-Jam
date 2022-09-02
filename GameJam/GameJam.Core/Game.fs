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
            StartTime: TimeSpan
            CurrentTime: TimeSpan list
            BestTime: TimeSpan list
        }

    type Msg =
        | SceneManagerMsg of SceneManager.SceneManagerMsg
        | Restart

    let empty =
        { SceneManagerModel = SceneManager.empty; StartTime = TimeSpan.Zero; CurrentTime = []; BestTime = []}
        
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
                //yield! mapEvent GameJam.Events.sceneManagerEvent SceneManager.map (List.map SceneManagerMsg)
                //yield! mapEvent GameJam.Events.sceneManagerEvent SceneManager.map (List.map SceneManagerMsg)
                //yield! mapEvent GameJam.Events.titleSceneEvent TitleScene.map (List.map TitleSceneMsg)
                yield! List.map SceneManagerMsg (SceneManager.mapAllEvent ())
            ] |> List.distinct
        messages

    let view (gameModel : Model) (gameTime : GameTime) =
        let deltaTime = float32 gameTime.Elapsed.TotalSeconds
        SceneManager.view gameModel.SceneManagerModel gameTime

    let update (gameModel : Model) (cmds : Msg list) (gameTime : GameTime) (game : Game) =
        let deltaTime = float32 gameTime.Elapsed.TotalSeconds

        let updateFold ((gameModel, msgs) : Model * Msg list) cmd = 
            match cmd with
            | SceneManagerMsg(m) -> 
                let (model,msg) = SceneManager.update m gameModel.SceneManagerModel gameTime
                { gameModel with SceneManagerModel = model }, msgs @ (List.map SceneManagerMsg msg)
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