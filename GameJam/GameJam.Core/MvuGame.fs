namespace GameJam.Core

open System;
open System.Collections
open System.Linq
open Stride.Core.Mathematics
open Stride.Core.Diagnostics
open Stride.Engine;
open Stride.Engine.Events
open Stride.Games;
open Stride.Profiling
open Messages

module Game =    
    type CurrentScene =
        | Title
        | GamePlay
    
    type GameModel =
        {
            PlayerModel : Player.Model
            PlatformModel : Platform.Model
            CurrentScene : CurrentScene
        }

    type MvuGame() =
        inherit Game()

        let init () =
            { PlayerModel = Player.empty (); PlatformModel = Platform.empty (); CurrentScene = GamePlay}, []

        let view (state : GameModel) (gameTime : GameTime) =
            match state.CurrentScene with
            | GamePlay -> 
                Player.view state.PlayerModel
                Platform.view state.PlatformModel (float32 gameTime.Elapsed.TotalSeconds)
            | _ -> ()
            
        let mutable State, Messages = init ()
        let mutable NextMessages : GameMsg list = []
        
        override this.BeginRun () =      
            let (newPlayerModel,playerMessage) = Player.init (this.SceneSystem) (this.Input)
            let (newPlatformodel,platformMessage) = Platform.init (this.SceneSystem)
            State <- { State with PlayerModel = newPlayerModel; PlatformModel = newPlatformodel }
            Messages <- Messages @ [playerMessage;platformMessage]
            ()

        member private this.GameUpdate (cmds : GameMsg list) (state : GameModel) (gameTime : GameTime) =
            let GameUpdateFold ((state, msgs) : GameModel * GameMsg list) cmd  = 
                match cmd with
                | PlayerMsg(m) -> 
                    let newModel, newMsg = Player.update m state.PlayerModel (float32 gameTime.Elapsed.TotalSeconds)
                    { state with PlayerModel = newModel }, msgs @ [newMsg]
                | PlatformMsg(m) -> 
                    let newModel, newMsg = Platform.update m state.PlatformModel (float32 gameTime.Elapsed.TotalSeconds)
                    { state with PlatformModel = newModel }, msgs @ [newMsg]
                | Start -> state, msgs
                | Restart -> state, msgs
                | Empty -> state, msgs

            let newState, nextMessages = List.fold GameUpdateFold (State, []) cmds
            State <- newState
            NextMessages <- NextMessages @ (List.filter(fun m -> match m with | Empty -> false | _ -> true ) (List.distinct nextMessages))

        
        override this.Update gametime =
            base.Update(gametime);
            
            Messages <- Messages @ Event.ProcessAllEvent ()

            this.DebugTextSystem.Print(sprintf "%i\n\n%f" State.PlayerModel.Counter State.PlatformModel.Timer, new Int2(50,50))
            this.GameUpdate Messages State gametime
            view State gametime


            Messages <- NextMessages
            NextMessages <- []


        override this.Destroy () =
            ()