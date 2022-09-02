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
open Stride.Core.Serialization.Contents

module Score =
    let private rotationSpeed = 60f

    type Model =
        { StartTime : TimeSpan; Records: TimeSpan list;  }

    type ScoreMsg = 
        | Start
        | Collect
        | Goal
    
    let map message = 
        match message with
        | "Start" -> [Start]
        | "Collect" -> [Collect]
        | "Goal" -> [Goal]
        | _ -> []

    let empty =
        { StartTime = new TimeSpan(); Records = [] }
    
    let init () =
        empty, []

    let update msg (model : Model) (gameTime : GameTime) =
        match msg with     
        | Start ->
            { model with StartTime = gameTime.Total }, []
        | Collect ->
            if model.Records.Length = 13 then
                GameJam.Events.GoalEventKey.Broadcast("Activate")
            let records = model.Records @ [gameTime.Total - model.StartTime]
            { model with Records = records }, []
        | Goal when model.Records.Length = 14 ->
            GameJam.Events.SceneManagerEventKey.Broadcast("Score")
            let records = model.Records @ [gameTime.Total - model.StartTime]
            { model with Records = records }, []
        | Goal ->            
            model, []

