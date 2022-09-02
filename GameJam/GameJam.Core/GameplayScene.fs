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
open Stride.Engine.Events

module GameplayScene =
    type Model =
        {
            PlayerModel : Player.Model
            PlatformModel : Platform.Model
            CollectibleModel : Collectible.Model
            ScoreModel : Score.Model
            GoalModel : Goal.Model
            GameplayUiModel : UI.Model
        }

    type GameplaySceneMsg =
        | PlayerMsg of Player.PlayerMsg
        | PlatformMsg of Platform.PlatformMsg
        | CollectibleMsg of Collectible.CollectibleMsg
        | ScoreMsg of Score.ScoreMsg
        | UiMsg of UI.UiMsg
        | GoalMsg of Goal.GoalMsg

    let empty =
        { PlayerModel = Player.empty; PlatformModel = Platform.empty; CollectibleModel = Collectible.empty; ScoreModel = Score.empty; GoalModel = Goal.empty; GameplayUiModel = UI.empty }

    let init (scene : Scene) (input : InputManager) : Model * GameplaySceneMsg list =
        let (newPlayerModel,playerMessages) = Player.init scene input
        let (newPlatformodel,platformMessages) = Platform.init scene
        let (newCollectibleModel,collectibleMessages) = Collectible.init scene
        let (newScoreModel,scoreMessages) = Score.init ()
        let (newUiModel,uiMessages) = UI.init scene
        let (newGoalmodel,goalMessages) = Goal.init scene
        let initMessages = (List.map PlayerMsg playerMessages) @ (List.map PlatformMsg platformMessages) @ (List.map CollectibleMsg collectibleMessages) @ (List.map ScoreMsg scoreMessages) @ (List.map UiMsg uiMessages) @ (List.map GoalMsg goalMessages)
        { empty with PlayerModel = newPlayerModel; PlatformModel = newPlatformodel; CollectibleModel = newCollectibleModel; ScoreModel = newScoreModel; GoalModel = newGoalmodel; GameplayUiModel = newUiModel }, initMessages
    
    let private mapEvent (eventReceiver : EventReceiver<'a>) (eventMap : 'a -> 'b list) (listMap : 'b list -> GameplaySceneMsg list) =   
        let eventList = (Seq.empty).ToList()
        let numEvent = eventReceiver.TryReceiveAll(eventList)
        let events = Seq.toList eventList
        let messages =
            [
                for e in events do
                    yield! listMap (eventMap e)
            ]
        messages

    let mapAllEvent () : GameplaySceneMsg list =
        let messages =
            [
                yield! mapEvent GameJam.Events.playerEvent Player.map (List.map PlayerMsg)
                yield! mapEvent GameJam.Events.platformEvent Platform.map (List.map PlatformMsg)
                yield! mapEvent GameJam.Events.collectibleEvent Collectible.map (List.map CollectibleMsg)
                yield! mapEvent GameJam.Events.uiEvent UI.map (List.map UiMsg)
                yield! mapEvent GameJam.Events.scoreEvent Score.map (List.map ScoreMsg)
                yield! mapEvent GameJam.Events.goalEvent Goal.map (List.map GoalMsg)
            ] |> List.distinct
        messages

    let update cmd state (gameTime : GameTime) =
        let deltaTime = float32 gameTime.Elapsed.TotalSeconds
        match cmd with
        | PlayerMsg(m) -> 
            let newModel, newMsg = Player.update m state.PlayerModel (deltaTime)
            { state with PlayerModel = newModel }, (List.map PlayerMsg newMsg)
        | PlatformMsg(m) -> 
            let newModel, newMsg = Platform.update m state.PlatformModel (deltaTime)
            { state with PlatformModel = newModel }, (List.map PlatformMsg newMsg)
        | CollectibleMsg(m) -> 
            let newModel, newMsg = Collectible.update m state.CollectibleModel (deltaTime)
            { state with CollectibleModel = newModel }, (List.map CollectibleMsg newMsg)
        | ScoreMsg(m) -> 
            let newModel, newMsg = Score.update m state.ScoreModel gameTime
            { state with ScoreModel = newModel }, (List.map ScoreMsg newMsg)
        | UiMsg(m) -> 
            let newModel, newMsg = UI.update m state.GameplayUiModel (deltaTime)
            { state with GameplayUiModel = newModel }, (List.map UiMsg newMsg)
        | GoalMsg(m) ->
            let newModel, newMsg = Goal.update m state.GoalModel (deltaTime)
            { state with GoalModel = newModel }, (List.map GoalMsg newMsg)

    let view (state : Model) (gameTime : GameTime) =
        let deltaTime = float32 gameTime.Elapsed.TotalSeconds

        Player.view state.PlayerModel
        Platform.view state.PlatformModel deltaTime
        Collectible.view state.CollectibleModel deltaTime
        Goal.view state.GoalModel deltaTime
        UI.view state.GameplayUiModel deltaTime

