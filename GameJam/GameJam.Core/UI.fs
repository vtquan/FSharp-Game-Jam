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

module UI =
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
            { model with Counter = model.Counter + 1 }, Empty
    
    let view (model : Model) (deltaTime : float32) =
        let bonusCounter = model.Page.RootElement.FindVisualChildOfType<TextBlock>("CollectionCounter");
        bonusCounter.Text <- sprintf "%i/14" model.Counter
        ()