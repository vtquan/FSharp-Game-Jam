namespace GameJam.Core

open Stride.Core.Collections
open Stride.Core.Mathematics
open Stride.Engine
open Stride.Games;
open Stride.Physics
open System.Linq
open Stride.Rendering.Sprites
open Stride.Input
open System

module Collectible =
    let private rotationSpeed = 100f

    type Model =
        { Rotation: float32; Collectibles : Entity list }

    type CollectibleMsg = 
        | Rotate

    let empty =
        { Rotation = 0f; Collectibles = [] }
        
    let init (scene : Scene) =
        let collectibles = scene.Entities.Where(fun x -> x.Name.Contains("Collectible"))
        { empty with Collectibles = List.ofSeq collectibles }, [Rotate]

    let update (msg : CollectibleMsg) (model : Model) (deltaTime : float32) =
        match msg with
        | Rotate ->
            { model with Rotation = (model.Rotation + rotationSpeed * deltaTime) % 720f }, [Rotate]
    
    let view (model : Model) (deltaTime : float32) =
        let collectibleIter (collectible : Entity) =
            collectible.Transform.Scale <- new Vector3(2f, 2f, 2f)
            collectible.Transform.Rotation <- Quaternion.RotationYawPitchRoll(MathUtil.DegreesToRadians(model.Rotation), 0f, 0f)

        List.iter collectibleIter model.Collectibles
    
    let map message = 
        match message with
        | "Rotate" -> [Rotate]
        | _ -> []
    
    let getMsg () = 
        EventHelper.recieveEvent GameJam.Events.collectibleEvent map