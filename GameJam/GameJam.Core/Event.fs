namespace GameJam.Core

open Stride.Engine.Events
open System.Linq
open GameJam.Core.Message

module Event =
    let private parseEvent (eventReceiver : EventReceiver<'a>) (eventMap : 'a -> GameMsg list) =   
        let eventList = (Seq.empty).ToList()
        let numEvent = eventReceiver.TryReceiveAll(eventList)
        let events = Seq.toList eventList
        [
            for e in events do
                yield! eventMap e   //Could use a map instead but this will allow returning multiple messages for an event
        ]
                
    let private tryReceiveEvent (eventReceiver : EventReceiver) =
        eventReceiver.TryReceive()
    
    let ProcessAllEvent () : GameMsg list=        
        let msgSeq =
            [
                yield! parseEvent GameJam.Events.gameEvent GameEvent.map
                //yield! parseEvent GameJam.Events.stringEvent StringEvent.mapStringEvent
            ]
        msgSeq
        |> List.distinct