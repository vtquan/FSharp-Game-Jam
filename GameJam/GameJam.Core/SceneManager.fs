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
open Stride.Core.Serialization.Contents

module SceneManager =
    type CurrentScene =
        | Title
        | GamePlay
        | Score

    //type TitleScene =
    //    { Page : UIPage }
        
    //let TitleSceneEmpty () =
    //    { Page = new UIPage() }
        
    //let TitleSceneInit () =
    //    { Page = new UIPage() }

    //type GameplayScene =
    //    { Page : UIPage; Counter : int }
        
    //let GameplaySceneEmpty () =
    //    { Page = new UIPage(); Counter = 0 }
        
    //let GameplaySceneInit () =
    //    { Page = new UIPage(); Counter = 0 }
        
    //type ScoreScene =
    //    { Page : UIPage; CurrentTime: TimeSpan list; BestTime: TimeSpan list }
        
    //let ScoreSceneEmpty () =
    //    { Page = new UIPage(); CurrentTime = []; BestTime = [] }
        
    //let ScoreSceneInit () =
    //    { Page = new UIPage(); CurrentTime = []; BestTime = [] }

    //type Model =
    //    { CurrentScene: CurrentScene; RootScene: SceneInstance; TitleScene : TitleScene; GameplayScene: GameplayScene; ScoreScene: ScoreScene }
    
    let private titleScene = "TitleScene"
    let private gameplayScene = "GameplayScene"
    let private ScoreScene = "ScoreScene"

    //type Model =
    //    { CurrentScene: CurrentScene  }

    //let empty () =
    //    { CurrentScene = Title }
    
    //let init (scene : Scene) =
    //    let page = scene.Entities.FirstOrDefault(fun x -> x.Name = "UI").Get<UIComponent>().Page
    //    { Counter = 0; Page = page }, Empty

    //let update msg (model : Model) (deltaTime : float32) =
    //    match msg with        
    //    | Increment ->
    //        { model with Counter = model.Counter + 1 }, Empty

    //let view (model : Model) (deltaTime : float32) =
    //    let bonusCounter = model.Page.RootElement.FindVisualChildOfType<TextBlock>("CollectionCounter");
    //    bonusCounter.Text <- sprintf "%i/14" model.Counter
    //    ()

