namespace GameJam.Core

open Stride.Core.Mathematics
open Stride.Engine;
open Stride.Engine.Events
open Stride.Games;
open System
open System.Linq

module EventHelper =
    let public parseEvent (eventReceiver : EventReceiver<'a>) (eventMap : 'a -> 'b list) =   
        let eventList = (Seq.empty).ToList()
        let numEvent = eventReceiver.TryReceiveAll(eventList)
        let events = Seq.toList eventList
        let messages =
            [
                for e in events do
                    yield! eventMap e
            ]
        messages

    let public parseEventMap (eventReceiver : EventReceiver<'a>) (eventMap : 'a -> 'b list) (listMap : 'b list -> 'c list) =   
        let events = 
            parseEvent eventReceiver eventMap
            |> listMap
        events