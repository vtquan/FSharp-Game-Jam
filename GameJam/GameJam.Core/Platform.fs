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

module Platform =
    let private speed = 3f
    let private duration = 5f

    type Direction =
        | Forward
        | Reverse

    type MovingPlatform = 
        { Entity: Entity; PlayerAttached : bool }

    type Model =
        { Timer: float32; Direction: Direction; Platforms : MovingPlatform list; Player : Entity }

    type PlatformMsg = 
        | Countdown
        | Countup
        | AttachPlayer of Entity
        | DetachPlayer of Entity

    let empty =
        { Timer = 0f; Direction = Forward; Platforms = []; Player = new Entity() }
        
    let init (scene : Scene) =
        let player = scene.Entities.FirstOrDefault(fun x -> x.Name = "PlayerCharacter")    
        let platforms = scene.Entities.Where(fun x -> x.Name.Contains("Platform"))    
        let movingPlatforms = Seq.map (fun x -> { Entity = x; PlayerAttached = false }) platforms

        { empty with Platforms = List.ofSeq movingPlatforms; Player = player }, [Countup]
    
    let mapPlatformAttach (attachedPlatform : Entity) (currentPlatform : MovingPlatform)  =
        match currentPlatform.Entity.GetChild(0) = attachedPlatform with    //Platform is a prefab so the the collision trigger belong to the child entity
        | true -> { currentPlatform with PlayerAttached = true }
        | false -> currentPlatform        
            
    let mapPlatformDetach (attachedPlatform : Entity) (currentPlatform : MovingPlatform)  =
        match currentPlatform.Entity.GetChild(0) = attachedPlatform with    //Platform is a prefab so the the collision trigger belong to the child entity
        | true -> { currentPlatform with PlayerAttached = false }
        | false -> currentPlatform

    let update msg (model : Model) (deltaTime : float32) =
        match msg with        
        | Countup when model.Timer > duration -> 
            { model with Timer = model.Timer - deltaTime; Direction = Reverse }, [Countdown]
            
        | Countup -> 
            { model with Timer = model.Timer + deltaTime }, [Countup]
            
        | Countdown when model.Timer < 0f -> 
            { model with Timer = model.Timer + deltaTime; Direction = Forward }, [Countup]
            
        | Countdown -> 
            { model with Timer = model.Timer - deltaTime }, [Countdown]
            
        | AttachPlayer(e) ->
            let newPlatforms = List.map (mapPlatformAttach e) model.Platforms
            { model with Timer = model.Timer - deltaTime; Platforms = newPlatforms }, []
            
        | DetachPlayer(e) ->
            let newPlatforms = List.map (mapPlatformDetach e) model.Platforms
            { model with Timer = model.Timer - deltaTime; Platforms = newPlatforms }, []

    
    let view (model : Model) (deltaTime : float32) =
        let movingPlatformIter (movingPlatform : MovingPlatform) =
            //movingPlatform.Entity.Transform.Position <- Vector3.Lerp(movingPlatform.Entity.Transform.Position, movingPlatform.Entity.Transform.Position + movingPlatform.Velocity)

            let deltaSpeed = 
                match model.Direction with
                | Forward -> speed * deltaTime
                | Reverse -> -speed * deltaTime

            // Find starting direction for each platforms
            let newVelocity = 
                match movingPlatform.Entity.Name.Substring(0,4) with 
                | "PosX" -> Vector3(deltaSpeed, 0f, 0f) 
                | "NegX" -> Vector3(-deltaSpeed, 0f, 0f) 
                | "PosY" -> Vector3(0f, deltaSpeed, 0f) 
                | "NegY" -> Vector3(0f, -deltaSpeed, 0f) 
                | "PosZ" -> Vector3(0f, 0f, deltaSpeed) 
                | "NegZ" -> Vector3(0f, 0f, -deltaSpeed) 
                | _ -> Vector3(speed * deltaSpeed, 0f, 0f) 

            //let newVelocity = if model.Direction = Forward then new Vector3(speed * deltaTime, 0f, 0f) else new Vector3(-speed * deltaTime, 0f, 0f)

            movingPlatform.Entity.Transform.Position <- movingPlatform.Entity.Transform.Position + newVelocity           
            movingPlatform.Entity.Transform.UpdateWorldMatrix()
            let physicComponents = movingPlatform.Entity.GetChild(0).GetAll<PhysicsComponent>() //Platform is a prefab so the the collision box belong to the child entity
            for pc in physicComponents do
                pc.UpdatePhysicsTransformation()

            match movingPlatform.PlayerAttached with
            | true ->
                model.Player.Transform.Position <- model.Player.Transform.Position + newVelocity
                model.Player.Transform.UpdateWorldMatrix()
                let playerPhysicComponent = model.Player.Get<PhysicsComponent>()
                playerPhysicComponent.UpdatePhysicsTransformation()
            | false -> ()


        List.iter movingPlatformIter model.Platforms
    
    let map ((message, entity) : string * Entity) : PlatformMsg list = 
        match message with
        | "AttachPlayer" -> [AttachPlayer(entity)]
        | "DetachPlayer" -> [DetachPlayer(entity)]
        | _ -> []
    
    let getMsg () = 
        EventHelper.recieveEvent GameJam.Events.platformEvent map