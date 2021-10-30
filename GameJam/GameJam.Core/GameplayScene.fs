namespace GameJam.Core

open Stride.Core.Collections
open Stride.Core.Mathematics
open Stride.Engine
open Stride.UI;
open Stride.UI.Controls;
open Stride.Games;
open Stride.Physics
open System.Linq
open Messages
open Stride.Rendering.Sprites
open Stride.Input
open System

module GameplayScene =
    type Model =
        {
            PlayerModel : Player.Model
            PlatformModel : Platform.Model
            GoalModel : Goal.Model
            GameplayUiModel : GameplayUI.Model
        }
    
    let empty () =
        { PlayerModel = Player.empty (); PlatformModel = Platform.empty (); GoalModel = Goal.empty (); GameplayUiModel = GameplayUI.empty () }

    let init (scene : Scene) (input : InputManager) =
        let (newPlayerModel,playerMessage) = Player.init (scene) (input)
        let (newPlatformodel,platformMessage) = Platform.init (scene)
        let (newGoalmodel,GoalMessage) = Goal.init (scene)
        let (newUiModel,UiMessage) = GameplayUI.init (scene)
        { PlayerModel = newPlayerModel; PlatformModel = newPlatformodel; GoalModel = newGoalmodel; GameplayUiModel = newUiModel }, [playerMessage; platformMessage; GoalMessage; UiMessage]
    
    let update cmd state (deltaTime : float32) =
        let mapToGameplaySceneMsg msgs =
            List.map (fun m -> GameplaySceneMsg(m)) msgs
        match cmd with
        | PlayerMsg(m) -> 
            let newModel, newMsg = Player.update m state.PlayerModel (deltaTime)
            { state with PlayerModel = newModel }, mapToGameplaySceneMsg newMsg
        | PlatformMsg(m) -> 
            let newModel, newMsg = Platform.update m state.PlatformModel (deltaTime)
            { state with PlatformModel = newModel }, mapToGameplaySceneMsg newMsg
        | UiMsg(m) -> 
            let newModel, newMsg = GameplayUI.update m state.GameplayUiModel (deltaTime)
            { state with GameplayUiModel = newModel }, mapToGameplaySceneMsg newMsg
        | GoalMsg(m) ->
            let newModel, newMsg = Goal.update m state.GoalModel (deltaTime)
            { state with GoalModel = newModel }, mapToGameplaySceneMsg newMsg

    let view (state : Model) (gameTime : GameTime) =
        Player.view state.PlayerModel
        Platform.view state.PlatformModel (float32 gameTime.Elapsed.TotalSeconds)
        GameplayUI.view state.GameplayUiModel (float32 gameTime.Elapsed.TotalSeconds)
        Goal.view state.GoalModel (float32 gameTime.Elapsed.TotalSeconds)

