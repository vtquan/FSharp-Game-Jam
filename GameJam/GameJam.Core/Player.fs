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

module Player =
    let private speed = 20f
    let private deadZone = 0.25f
    let private jumpReactionThreshold = 0.3f

    type Model =
        { Velocity : Vector3; MoveDirection : Vector3; NewMoveDirection: Vector3; Jumped : bool; JumpReactionRemaining : float32; Counter : int; Entity : Entity; AppearanceModel : Entity; AttachedPlatform : Entity; Input : InputManager; Camera : CameraComponent }
    
    let empty () =
        { Velocity = new Vector3(0f, 0.f, 0f); MoveDirection = Vector3.Zero; NewMoveDirection = Vector3.Zero; Jumped = false; JumpReactionRemaining = 0f; Counter = 0; Entity = new Entity(); AppearanceModel = new Entity(); AttachedPlatform = new Entity(); Input = new InputManager(); Camera = new CameraComponent() }
    
    let init (sceneSystem : SceneSystem) (input : InputManager) =
        let entity = sceneSystem.SceneInstance.RootScene.Entities.FirstOrDefault(fun x -> x.Name = "PlayerCharacter")        
        let cameraComponent = entity.GetChild(1).GetChild(0).Get<CameraComponent>()
        { Velocity = new Vector3(0f, 0f, 0f); MoveDirection = Vector3.Zero; NewMoveDirection = Vector3.Zero; Jumped = false; JumpReactionRemaining = jumpReactionThreshold; Counter = 0; Entity = entity; AppearanceModel = entity.GetChild(0); AttachedPlatform = new Entity(); Input = input; Camera = cameraComponent }, Empty
    
    
    // Cap x and z movement speed
    let capHorizontalVelocity (velocity : Vector3) speedLimit =
        let horizontalVelocity = new Vector3(velocity.X, 0f, velocity.Z)
        match horizontalVelocity.Length() > speedLimit with
        | true -> 
            let cappedHorizontalVelocity = Vector3.Normalize(velocity)
            let cappedVelocity = new Vector3(cappedHorizontalVelocity.X, velocity.Y, cappedHorizontalVelocity.Z)
            cappedVelocity
        | false -> velocity

    let update msg model (deltaTime : float32) =
        match msg with
        | Collision -> 
            { model with Counter = model.Counter + 1 }, Empty
            
        | MoveLeft -> 
            { model with NewMoveDirection = model.NewMoveDirection - Vector3.UnitX }, PlayerMsg(NewVelocity)
                        
        | MoveRight -> 
            { model with NewMoveDirection = model.NewMoveDirection + Vector3.UnitX }, PlayerMsg(NewVelocity)
            
        | MoveUp -> 
            { model with NewMoveDirection = model.NewMoveDirection + Vector3.UnitZ }, PlayerMsg(NewVelocity)
            
        | MoveDown -> 
            { model with NewMoveDirection = model.NewMoveDirection - Vector3.UnitZ }, PlayerMsg(NewVelocity)

        | Jump when model.Jumped = false && model.JumpReactionRemaining > 0f  -> 
            GameJam.Events.IsGroundedEventKey.Broadcast(false)
            let characterComponent = model.Entity.Get<CharacterComponent>()
            characterComponent.Jump()
            { model with Velocity = new Vector3(model.Velocity.X, 1.3f, model.Velocity.Z); Jumped = true; JumpReactionRemaining = 0f }, Empty
            
        | Jump -> 
            model, Empty
            
        | NewVelocity -> 
            let leftStickInput = model.Input.GetLeftThumbAny(deadZone)
            let leftStickInputDirection = new Vector3(leftStickInput.X, 0f, leftStickInput.Y)
            let moveDirection = leftStickInputDirection + model.NewMoveDirection
            let worldSpeed = Utils.LogicDirectionToWorldDirection(new Vector2(moveDirection.X, moveDirection.Z), model.Camera, Vector3.UnitY)
            let moveLength = moveDirection.Length()
            let isDeadZoneLeft = moveLength < deadZone
            let newMoveDirection =
                match isDeadZoneLeft with
                | true -> Vector3.Zero
                | false ->
                    match moveLength > 1f with
                    | true -> worldSpeed
                    | false -> worldSpeed * ((moveLength - deadZone) / (1f - deadZone))
            let moveVelocity = model.MoveDirection*0.90f + newMoveDirection *0.1f;
            { model with Velocity = new Vector3(model.Velocity.X, model.Velocity.Y - 0.03f, model.Velocity.Z) + model.MoveDirection * 0.02f; MoveDirection  = moveVelocity; NewMoveDirection = Vector3.Zero }, Empty  
            
        | Grounded when model.Jumped = true -> 
            GameJam.Events.IsGroundedEventKey.Broadcast(true)
            { model with Jumped = false; JumpReactionRemaining = jumpReactionThreshold }, Empty
            
        | Grounded -> 
            GameJam.Events.IsGroundedEventKey.Broadcast(true)
            { model with Jumped = false; JumpReactionRemaining = jumpReactionThreshold }, Empty

        | Airborne ->
            { model with JumpReactionRemaining = model.JumpReactionRemaining - deltaTime }, Empty
            
        | NoMovement -> 
            { model with NewMoveDirection = Vector3.Zero }, PlayerMsg(NewVelocity)
    
    let view model =
        let characterComponent = model.Entity.Get<CharacterComponent>()
        characterComponent.SetVelocity( model.MoveDirection  * speed)

        GameJam.Events.RunSpeedEventKey.Broadcast(model.MoveDirection.Length());
        GameJam.Events.MoveDirectionEventKey.Broadcast(model.MoveDirection)

        if model.MoveDirection.Length() > 0.01f then   // Prevent player from rotating to default rotation when not moving
            let rotationRad = (float32) (Math.Atan2((float) -model.MoveDirection.Z,(float) model.MoveDirection.X))
            let piOverTwo = (float32) (MathUtil.PiOverTwo)
        
            let yawOrientation = if (model.MoveDirection.Length() > 0.001f) then MathUtil.RadiansToDegrees(rotationRad + piOverTwo) else 0f
            model.AppearanceModel.Transform.Rotation <- Quaternion.RotationYawPitchRoll(MathUtil.DegreesToRadians(yawOrientation), 0f, 0f)
        else
            ()