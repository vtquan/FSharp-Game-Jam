namespace GameJam.Core

open Stride.Engine
open Stride.Engine.Events
open System.Linq
open Messages

module Event =
    let private TryReceiveAllEvent (eventReceiver : EventReceiver<'a>) =   
        let eventList = (Seq.empty).ToList()
        let m = eventReceiver.TryReceiveAll(eventList)
        let a = Seq.toList eventList
        m, Seq.toList eventList
        
    let private TryReceiveEvent (eventReceiver : EventReceiver) =
        eventReceiver.TryReceive()


    // Could be expand for larger project by creating function of each scene
    let private ProcessGameEvent ((message, entity) : string * Entity) : GameMsg = 
        match message with
        | "Collect" -> PlayerMsg(Collision(entity))
        | "Left" -> PlayerMsg(MoveLeft)
        | "Right" -> PlayerMsg(MoveRight)
        | "Up" -> PlayerMsg(MoveUp)
        | "Down" -> PlayerMsg(MoveDown)
        | "Jump" -> PlayerMsg(Jump)
        | "Grounded" -> PlayerMsg(Grounded)
        | "Airborne" -> PlayerMsg(Airborne)
        | "NoMovement" -> PlayerMsg(NoMovement)
        | "AttachPlayer" -> PlatformMsg(AttachPlayer(entity))
        | "DetachPlayer" -> PlatformMsg(DetachPlayer(entity))
        | "Camera"| "Camera2"| "Camera3"| "Camera4" -> Empty    //for example all these message can be process by ProcessCameraEvent message
        | _ -> Empty

    let ProcessAllGameEvent (eventReceiver : EventReceiver<string * Entity>) : GameMsg list =
        let numEvent, events = TryReceiveAllEvent eventReceiver
        match numEvent with
        | 0 -> []
        | _ ->
            let msgSeq =
                seq {
                    for e in events do
                        match ProcessGameEvent e with
                        | Empty -> ()
                        | m -> yield m
                }
            List.ofSeq msgSeq

    // Could be expand for larger project by creating function of each scene
    let private ProcessOtherEvent (num : float32) : GameMsg = 
        Empty
            
    let ProcessAllOtherEvent (eventReceiver : EventReceiver<float32>) : GameMsg list=        
        let numEvent, events = TryReceiveAllEvent eventReceiver
        match numEvent with
        | 0 -> []
        | _ ->
            let msgSeq =
                seq {
                    for e in events do
                        match ProcessOtherEvent e with
                        | Empty -> ()
                        | m -> yield m
                }
            List.ofSeq msgSeq    
    
    let ProcessAllEvent () : GameMsg list=        
        let msgSeq =
            seq {
                yield! ProcessAllGameEvent GameJam.Events.gameListener
            }

        List.ofSeq (Seq.filter(fun m -> match m with | Empty -> false | _ -> true ) (Seq.distinct msgSeq))