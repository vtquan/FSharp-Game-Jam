namespace GameJam.Core

open Stride.Core.Collections
open Stride.Core.Mathematics
open Stride.Engine
open Stride.Games;
open Stride.Physics
open System.Linq
open Messages
open Stride.Rendering.Sprites
open Stride.Input
open System

module Platform =
    let private speed = 1f
    let private duration = 5f
    let private deadZone = 0.25f
    let private jumpReactionThreshold = 0.3f

    type Direction =
        | Up
        | Down

    type MovingPlatform = 
        { Velocity: Vector3; Entity: Entity; PlayerAttached : bool }

    type Model =
        { Timer: float32; Direction: Direction; Platforms : MovingPlatform list; Player : Entity }
    
    let empty () =
        { Timer = 0f; Direction = Up; Platforms = []; Player = new Entity() }
        
    let init (sceneSystem : SceneSystem) =
        let player = sceneSystem.SceneInstance.RootScene.Entities.FirstOrDefault(fun x -> x.Name = "PlayerCharacter")    
        let platforms = sceneSystem.SceneInstance.RootScene.Entities.Where(fun x -> x.Name = "MovingPlatform")    
        let movingPlatforms = Seq.map (fun x -> { Velocity = Vector3.Zero; Entity = x; PlayerAttached = false }) platforms

        { Timer = 0f; Direction = Up; Platforms = List.ofSeq movingPlatforms; Player = player }, Empty

    
    let mapPlatformAttach (attachedPlatform : Entity) (currentPlatform : MovingPlatform)  =
        match currentPlatform.Entity.Name = attachedPlatform.Name with
        | true -> { currentPlatform with PlayerAttached = true }
        | false -> currentPlatform
        
            
    let mapPlatformDetach (attachedPlatform : Entity) (currentPlatform : MovingPlatform)  =
        match currentPlatform.Entity.Name = attachedPlatform.Name with
        | true -> { currentPlatform with PlayerAttached = false }
        | false -> currentPlatform
        
            
    let mapPlatformVelocity (attachedPlatform : Entity) (velocity : Vector3) (currentPlatform : MovingPlatform)   =
        match currentPlatform.Entity.Name = attachedPlatform.Name with
        | true -> { currentPlatform with Velocity = velocity }
        | false -> currentPlatform

    let update msg (model : Model) (deltaTime : float32) =
        match msg with        
        | Vertical(e) when model.Direction = Up -> 
            model, PlatformMsg(Rise(e))

        | Vertical(e) when model.Direction = Down -> 
            model, PlatformMsg(Lower(e))

        | Rise(e) when model.Timer > duration -> 
            let newVelocity = new Vector3(speed * deltaTime, 0f, 0f)
            let movingPlatform = List.map (mapPlatformVelocity e newVelocity) model.Platforms
            { model with Timer = model.Timer - deltaTime; Direction = Down; Platforms = movingPlatform }, PlatformMsg(Clear(e))
            
        | Rise(e) -> 
            let newVelocity = new Vector3(speed * deltaTime, 0f, 0f)
            let movingPlatform = List.map (mapPlatformVelocity e newVelocity) model.Platforms
            { model with Timer = model.Timer + deltaTime; Platforms = movingPlatform }, PlatformMsg(Clear(e))
            
        | Lower(e) when model.Timer < 0f -> 
            let newVelocity = new Vector3(-speed * deltaTime, 0f, 0f)
            let movingPlatform = List.map (mapPlatformVelocity e newVelocity) model.Platforms
            { model with Timer = model.Timer + deltaTime; Direction = Up; Platforms = movingPlatform }, PlatformMsg(Clear(e))
            
        | Lower(e) -> 
            let newVelocity = new Vector3(-speed * deltaTime, 0f, 0f)
            let movingPlatform = List.map (mapPlatformVelocity e newVelocity) model.Platforms
            { model with Timer = model.Timer - deltaTime; Platforms = movingPlatform }, PlatformMsg(Clear(e))
            
        | AttachPlayer(e) ->
            let newPlatforms = List.map (mapPlatformAttach e) model.Platforms
            { model with Timer = model.Timer - deltaTime; Platforms = newPlatforms }, Empty
            
        | DetachPlayer(e) ->
            let newPlatforms = List.map (mapPlatformDetach e) model.Platforms
            { model with Timer = model.Timer - deltaTime; Platforms = newPlatforms }, Empty

        | Clear(e) -> 
            model, Empty

    
    let view model =
        let movingPlatformIter (movingPlatform : MovingPlatform) =
            //movingPlatform.Entity.Transform.Position <- Vector3.Lerp(movingPlatform.Entity.Transform.Position, movingPlatform.Entity.Transform.Position + movingPlatform.Velocity)

            movingPlatform.Entity.Transform.Position <- movingPlatform.Entity.Transform.Position + movingPlatform.Velocity

            match movingPlatform.PlayerAttached with
            | true ->
                model.Player.Transform.Position <- model.Player.Transform.Position + movingPlatform.Velocity
                model.Player.Transform.UpdateWorldMatrix()
                let playerPhysicComponent = model.Player.Get<PhysicsComponent>()
                playerPhysicComponent.UpdatePhysicsTransformation()
            | false -> ()

            movingPlatform.Entity.Transform.UpdateWorldMatrix()
            let physicComponents = movingPlatform.Entity.GetAll<PhysicsComponent>()
            for pc in physicComponents do
                pc.UpdatePhysicsTransformation()


        List.iter movingPlatformIter model.Platforms