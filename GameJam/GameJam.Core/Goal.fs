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

module Goal =
    let rotationSpeed = 30f
    let growthRate = 50f

    type Model =
        { Activated : bool; Rotation: float32; Entity: Entity  }

    let empty () =
        { Activated = false; Rotation = 0f; Entity = new Entity() }
    
    let init (scene : Scene) =
        let entity = scene.Entities.FirstOrDefault(fun x -> x.Name = "Goal").FindChild("Core")
        { Activated = false; Rotation = 0f; Entity = entity }, Empty

    let update msg (model : Model) (deltaTime : float32) =
        match msg with        
        | Activate ->
            { model with Activated = true }, GoalMsg(Rotate)
        | Rotate when model.Rotation > 720f ->
            { model with Rotation = model.Rotation + rotationSpeed * deltaTime - 720f }, GoalMsg(Rotate)
        | Rotate ->
            { model with Rotation = model.Rotation + rotationSpeed * deltaTime }, GoalMsg(Rotate)

    let view (model : Model) (deltaTime : float32) =
        match model.Activated with
        | true ->
            model.Entity.Transform.Scale <- new Vector3(2f, 2f, 2f)
            model.Entity.Transform.Rotation <- Quaternion.RotationYawPitchRoll(MathUtil.DegreesToRadians(model.Rotation), 0f, 0f)
        | false ->             
            model.Entity.Transform.Scale <- new Vector3(0f, 0f, 0f)

