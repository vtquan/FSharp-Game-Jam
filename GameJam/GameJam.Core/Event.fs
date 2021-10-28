namespace GameJam.Core

open Stride.Engine
open Stride.Engine.Events
open System.Linq
open Messages

module Event =
    let private ProcessGameEvent ((message, entity) : string * Entity) : GameMsg list = 
        match message with
        | "Collect" -> [PlayerMsg(Collision(entity));UiMsg(Increment)]
        | "Left" -> [PlayerMsg(MoveLeft)]
        | "Right" -> [PlayerMsg(MoveRight)]
        | "Up" -> [PlayerMsg(MoveUp)]
        | "Down" -> [PlayerMsg(MoveDown)]
        | "Jump" -> [PlayerMsg(Jump)]
        | "Grounded" -> [PlayerMsg(Grounded)]
        | "Airborne" -> [PlayerMsg(Airborne)]
        | "NoMovement" -> [PlayerMsg(NoMovement)]
        | "AttachPlayer" -> [PlatformMsg(AttachPlayer(entity))]
        | "DetachPlayer" -> [PlatformMsg(DetachPlayer(entity))]
        | _ -> []
        
    let private TryReceiveAllEvent (eventReceiver : EventReceiver<'a>) =   
        let eventList = (Seq.empty).ToList()
        let m = eventReceiver.TryReceiveAll(eventList)
        let a = Seq.toList eventList
        m, Seq.toList eventList
            
    let private TryReceiveEvent (eventReceiver : EventReceiver) =
        eventReceiver.TryReceive()

    let ProcessAllGameEvent (eventReceiver : EventReceiver<string * Entity>) : GameMsg list =
        let numEvent, events = TryReceiveAllEvent eventReceiver
        match numEvent with
        | 0 -> []
        | _ ->
            let msgSeq =
                seq {
                    for e in events do
                        match ProcessGameEvent e with
                        | [] -> ()
                        | m -> yield! m
                }
            List.ofSeq msgSeq
    
    let ProcessAllEvent () : GameMsg list=        
        let msgSeq =
            seq {
                yield! ProcessAllGameEvent GameJam.Events.gameListener  //Can be expanded with additional yield! process listener
            }

        List.ofSeq (Seq.filter(fun m -> match m with | Empty -> false | _ -> true ) (Seq.distinct msgSeq))