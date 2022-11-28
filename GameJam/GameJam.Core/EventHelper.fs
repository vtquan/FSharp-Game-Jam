namespace GameJam.Core

open Stride.Core.Mathematics
open Stride.Engine;
open Stride.Engine.Events
open Stride.Games;
open System
open System.Linq

module EventHelper =
    let public recieveEvent (eventReceiver : EventReceiver<'a>) (eventMap : 'a -> 'b list) =   
        let eventList = (Seq.empty).ToList()
        let numEvent = eventReceiver.TryReceiveAll(eventList)
        let events = Seq.toList eventList
        let messages =
            [
                for e in eventList do
                    yield! eventMap e
            ]
        messages

    let public recieveEventMap mapType (eventReceiver : EventReceiver<'a>) (eventMap : 'a -> 'b list)  =   
        let messages = recieveEvent eventReceiver eventMap
        List.map mapType messages