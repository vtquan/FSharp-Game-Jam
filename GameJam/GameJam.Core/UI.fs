namespace GameJam.Core

open Stride.Core.Collections
open Stride.Core.Mathematics
open Stride.Engine
open Stride.UI;
open Stride.UI.Controls;
open Stride.Games;
open Stride.Physics
open System.Linq
open GameJam.Core.Message
open Stride.Rendering.Sprites
open Stride.Input
open System
open Stride.UI.Panels

module GameplayUI =
    type Model =
        { Counter: int; Page : UIPage }
    
    let empty () =
        { Counter = 0; Page = new UIPage() }
        
    let init (scene : Scene) =
        let page = scene.Entities.FirstOrDefault(fun x -> x.Name = "UI").Get<UIComponent>().Page
        { Counter = 0; Page = page }, Empty

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
        { CurrentTime = currentTime; BestTime = bestTime; Page = page }, Empty

    let update msg (model : Model) (deltaTime : float32) =
        match msg with        
        | Increment ->
            model, []
    
    let view (model : Model) (deltaTime : float32) =
        let timeCounter = model.Page.RootElement.FindVisualChildOfType<Grid>("Times");
        timeCounter.FindVisualChildOfType<TextBlock>("Time1").Text <- sprintf "%s" <| model.CurrentTime.Item(0).ToString("hh\:mm\:ss\:fff")
        timeCounter.FindVisualChildOfType<TextBlock>("Time2").Text <- sprintf "%s" <| model.CurrentTime.Item(1).ToString("hh\:mm\:ss\:fff")
        timeCounter.FindVisualChildOfType<TextBlock>("Time3").Text <- sprintf "%s" <| model.CurrentTime.Item(2).ToString("hh\:mm\:ss\:fff")
        timeCounter.FindVisualChildOfType<TextBlock>("Time4").Text <- sprintf "%s" <| model.CurrentTime.Item(3).ToString("hh\:mm\:ss\:fff")
        timeCounter.FindVisualChildOfType<TextBlock>("Time5").Text <- sprintf "%s" <| model.CurrentTime.Item(4).ToString("hh\:mm\:ss\:fff")
        timeCounter.FindVisualChildOfType<TextBlock>("Time6").Text <- sprintf "%s" <| model.CurrentTime.Item(5).ToString("hh\:mm\:ss\:fff")
        timeCounter.FindVisualChildOfType<TextBlock>("Time7").Text <- sprintf "%s" <| model.CurrentTime.Item(6).ToString("hh\:mm\:ss\:fff")
        timeCounter.FindVisualChildOfType<TextBlock>("Time8").Text <- sprintf "%s" <| model.CurrentTime.Item(7).ToString("hh\:mm\:ss\:fff")
        timeCounter.FindVisualChildOfType<TextBlock>("Time9").Text <- sprintf "%s" <| model.CurrentTime.Item(8).ToString("hh\:mm\:ss\:fff")
        timeCounter.FindVisualChildOfType<TextBlock>("Time10").Text <- sprintf "%s" <| model.CurrentTime.Item(9).ToString("hh\:mm\:ss\:fff")
        timeCounter.FindVisualChildOfType<TextBlock>("Time11").Text <- sprintf "%s" <| model.CurrentTime.Item(10).ToString("hh\:mm\:ss\:fff")
        timeCounter.FindVisualChildOfType<TextBlock>("Time12").Text <- sprintf "%s" <| model.CurrentTime.Item(11).ToString("hh\:mm\:ss\:fff")
        timeCounter.FindVisualChildOfType<TextBlock>("Time13").Text <- sprintf "%s" <| model.CurrentTime.Item(12).ToString("hh\:mm\:ss\:fff")
        timeCounter.FindVisualChildOfType<TextBlock>("Time14").Text <- sprintf "%s" <| model.CurrentTime.Item(13).ToString("hh\:mm\:ss\:fff")
        timeCounter.FindVisualChildOfType<TextBlock>("Time15").Text <- sprintf "%s" <| model.CurrentTime.Item(14).ToString("hh\:mm\:ss\:fff")

        //let bestTimeCounter = model.Page.RootElement.FindVisualChildOfType<Grid>("BestTimes");
        //if model.BestTime.Length > 0 then
        //    bestTimeCounter.FindVisualChildOfType<TextBlock>("Time1").Text <- sprintf "%s" <| model.BestTime.Item(0).ToString("hh\:mm\:ss\:fff")
        //    bestTimeCounter.FindVisualChildOfType<TextBlock>("Time2").Text <- sprintf "%s" <| model.BestTime.Item(1).ToString("hh\:mm\:ss\:fff")
        //else
        //    bestTimeCounter.FindVisualChildOfType<TextBlock>("Time1").Text <- sprintf "%s" <| TimeSpan.Zero.ToString("hh\:mm\:ss\:fff")
        //    bestTimeCounter.FindVisualChildOfType<TextBlock>("Time2").Text <- sprintf "%s" <| TimeSpan.Zero.ToString("hh\:mm\:ss\:fff")