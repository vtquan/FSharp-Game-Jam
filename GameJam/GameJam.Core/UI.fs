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
open Stride.UI.Panels

module UI =
    type Model =
        { Counter: int; Page : UIPage }

    type UiMsg = 
        | Increment
    
    let map message = 
        match message with
        | "Increment" -> [Increment]
        | _ -> []
    
    let empty =
        { Counter = 0; Page = new UIPage() }
        
    let init (scene : Scene) =
        let page = scene.Entities.FirstOrDefault(fun x -> x.Name = "UI").Get<UIComponent>().Page
        { empty with Page = page }, []

    let update msg (model : Model) (deltaTime : float32) =
        match msg with        
        | Increment ->
            { model with Counter = model.Counter + 1 }, []
    
    let view (model : Model) (deltaTime : float32) =
        let bonusCounter = model.Page.RootElement.FindVisualChildOfType<TextBlock>("CollectionCounter");
        bonusCounter.Text <- sprintf "%i/14" model.Counter
        ()



module ScoreUI =
    type Model =
        { CurrentTime: TimeSpan list; BestTime: TimeSpan list; Page : UIPage }
    
    let empty () =
        { CurrentTime = []; BestTime = []; Page = new UIPage() }
        
    let init currentTime bestTime (scene : Scene) =
        let page = scene.Entities.FirstOrDefault(fun x -> x.Name = "UI").Get<UIComponent>().Page
        { CurrentTime = currentTime; BestTime = bestTime; Page = page }, []

    let update msg (model : Model) (deltaTime : float32) =
        match msg with        
        | Increment ->
            model, []
    
    let view (model : Model) (deltaTime : float32) =
        let timeCounter = model.Page.RootElement.FindVisualChildOfType<Grid>("Times");
        timeCounter.FindVisualChildOfType<TextBlock>("Time1").Text <- sprintf "%s" <| model.CurrentTime.Item(0).ToString("hh\:mm\:ss\:fff")
        timeCounter.FindVisualChildOfType<TextBlock>("Time2").Text <- sprintf "%s" <| model.CurrentTime.Item(1).ToString("hh\:mm\:ss\:fff")

        //let bestTimeCounter = model.Page.RootElement.FindVisualChildOfType<Grid>("BestTimes");
        //if model.BestTime.Length > 0 then
        //    bestTimeCounter.FindVisualChildOfType<TextBlock>("Time1").Text <- sprintf "%s" <| model.BestTime.Item(0).ToString("hh\:mm\:ss\:fff")
        //    bestTimeCounter.FindVisualChildOfType<TextBlock>("Time2").Text <- sprintf "%s" <| model.BestTime.Item(1).ToString("hh\:mm\:ss\:fff")
        //else
        //    bestTimeCounter.FindVisualChildOfType<TextBlock>("Time1").Text <- sprintf "%s" <| TimeSpan.Zero.ToString("hh\:mm\:ss\:fff")
        //    bestTimeCounter.FindVisualChildOfType<TextBlock>("Time2").Text <- sprintf "%s" <| TimeSpan.Zero.ToString("hh\:mm\:ss\:fff")