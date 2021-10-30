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

module ScoreScene =
    type Model =
        {
            ScoreUiModel : ScoreUI.Model
        }
    
    let empty () =
        { ScoreUiModel = ScoreUI.empty () }

    let init currentTime bestTime (scene : Scene) =
        let (newUiModel,UiMessage) = ScoreUI.init currentTime bestTime (scene)
        { ScoreUiModel = newUiModel }, [UiMessage]
    
    let update cmd state (deltaTime : float32) =
        let mapToGameplaySceneMsg msgs =
            List.map (fun m -> GameplaySceneMsg(m)) msgs
        match cmd with
        | Restart -> state, []

    let view (state : Model) (gameTime : GameTime) =
        ScoreUI.view state.ScoreUiModel (float32 gameTime.Elapsed.TotalSeconds)

