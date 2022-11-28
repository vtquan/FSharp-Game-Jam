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

    let view (gameModel : Model) (gameTime : GameTime) =
        let deltaTime = float32 gameTime.Elapsed.TotalSeconds
        SceneManager.view gameModel.SceneManagerModel gameTime

    let update (gameModel : Model) (cmd : Msg) (gameTime : GameTime) (game : Game) =
        let deltaTime = float32 gameTime.Elapsed.TotalSeconds
        match cmd with
        | SceneManagerMsg(m) -> 
            let (model,msg) = SceneManager.update m gameModel.SceneManagerModel gameTime
            { gameModel with SceneManagerModel = model }, (List.map SceneManagerMsg msg)
        | Restart -> 
            GameJam.Events.SceneManagerEventKey.Broadcast("Title")
            let bestTime = 
                if gameModel.BestTime.Length > 0 then
                    if gameModel.SceneManagerModel.GameplaySceneModel.ScoreModel.Records.Item(14) < gameModel.BestTime.Item(14) then gameModel.SceneManagerModel.GameplaySceneModel.ScoreModel.Records else gameModel.BestTime
                else
                    gameModel.CurrentTime
            GameJam.Events.SceneManagerEventKey.Broadcast("Title")
            { gameModel with BestTime = bestTime}, []

    let map message = 
        match message with
        | "Restart" -> [Restart]
        | _ -> []

    let getMsg () : Msg list =
        [
            yield! EventHelper.recieveEvent GameJam.Events.gameEvent map
            yield! List.map SceneManagerMsg (SceneManager.getMsg ())
        ]