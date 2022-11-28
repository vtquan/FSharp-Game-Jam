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
open EventHelper

module GameplayScene =
    type Model =
        {
            PlayerModel : Player.Model
            PlatformModel : Platform.Model
            CollectibleModel : Collectible.Model
            ScoreModel : Score.Model
            GameplayUiModel : GameplayUI.Model
            GoalModel : Goal.Model
        }

    type GameplaySceneMsg =
        | PlayerMsg of Player.PlayerMsg
        | PlatformMsg of Platform.PlatformMsg
        | CollectibleMsg of Collectible.CollectibleMsg
        | ScoreMsg of Score.ScoreMsg
        | GameplayUiMsg of GameplayUI.GameplayUiMsg
        | GoalMsg of Goal.GoalMsg

    let empty =
        { PlayerModel = Player.empty; PlatformModel = Platform.empty; CollectibleModel = Collectible.empty; ScoreModel = Score.empty; GoalModel = Goal.empty; GameplayUiModel = GameplayUI.empty }

    let init (scene : Scene) (input : InputManager) : Model * GameplaySceneMsg list =
        let playerModel,playerMessages = Player.init scene input
        let platformodel,platformMessages = Platform.init scene
        let collectibleModel,collectibleMessages = Collectible.init scene
        let scoreModel,scoreMessages = Score.init ()
        let gameplayUiModel,gameplayUiMessages = GameplayUI.init scene
        let goalmodel,goalMessages = Goal.init scene
        let initMessages = 
            (List.map PlayerMsg playerMessages) @ 
            (List.map PlatformMsg platformMessages) @ 
            (List.map CollectibleMsg collectibleMessages) @ 
            (List.map ScoreMsg scoreMessages) @ 
            (List.map GameplayUiMsg gameplayUiMessages) @ 
            (List.map GoalMsg goalMessages)
        { empty with PlayerModel = playerModel; PlatformModel = platformodel; CollectibleModel = collectibleModel; ScoreModel = scoreModel; GameplayUiModel = gameplayUiModel; GoalModel = goalmodel }, initMessages

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
        | GameplayUiMsg(m) -> 
            let newModel, newMsg = GameplayUI.update m state.GameplayUiModel (deltaTime)
            { state with GameplayUiModel = newModel }, (List.map GameplayUiMsg newMsg)
        | GoalMsg(m) ->
            let newModel, newMsg = Goal.update m state.GoalModel (deltaTime)
            { state with GoalModel = newModel }, (List.map GoalMsg newMsg)

    let view (state : Model) (gameTime : GameTime) =
        let deltaTime = float32 gameTime.Elapsed.TotalSeconds

        Player.view state.PlayerModel
        Platform.view state.PlatformModel deltaTime
        Collectible.view state.CollectibleModel deltaTime
        Goal.view state.GoalModel deltaTime
        GameplayUI.view state.GameplayUiModel deltaTime

    let getMsg () : GameplaySceneMsg list =
        [
            yield! List.map PlatformMsg (Platform.getMsg ())
            yield! List.map PlayerMsg (Player.getMsg ())
            yield! List.map CollectibleMsg (Collectible.getMsg ())
            yield! List.map GameplayUiMsg (GameplayUI.getMsg ())
            yield! List.map ScoreMsg (Score.getMsg ())
            yield! List.map GoalMsg (Goal.getMsg ())
        ]

