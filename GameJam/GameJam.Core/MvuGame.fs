namespace GameJam.Core

open Stride.Core.Mathematics
open Stride.Engine;
open Stride.Games;
open Messages
open SceneManager
open System

module Game =    
    
    type GameModel =
        {
            PlayerModel : Player.Model
            PlatformModel : Platform.Model
            GoalModel : Goal.Model
            UiModel : UI.Model
            CurrentScene : CurrentScene
            StartTime: TimeSpan
            CurrentTime: TimeSpan list
            BestTime: TimeSpan list
        }

    type MvuGame() =
        inherit Game()

        let init () =
            { PlayerModel = Player.empty (); PlatformModel = Platform.empty (); GoalModel = Goal.empty (); UiModel = UI.empty (); CurrentScene = Title; StartTime = TimeSpan.Zero; CurrentTime = []; BestTime = []}, []

        let view (state : GameModel) (gameTime : GameTime) =
            match state.CurrentScene with
            | Title -> 
                ()
            | GamePlay -> 
                Player.view state.PlayerModel
                Platform.view state.PlatformModel (float32 gameTime.Elapsed.TotalSeconds)
                UI.view state.UiModel (float32 gameTime.Elapsed.TotalSeconds)
                Goal.view state.GoalModel (float32 gameTime.Elapsed.TotalSeconds)
            | Score -> 
                ()
            
        let mutable State, Messages = init ()
        let mutable NextMessages : GameMsg list = []
        
        override this.BeginRun () = 
            let titleScene = this.Content.Load<Scene>("TitleScene")
            this.SceneSystem.SceneInstance.RootScene.Children.Add(titleScene)
            ()

        member private this.GameUpdate (cmds : GameMsg list) (state : GameModel) (gameTime : GameTime) =
            let GameUpdateFold ((state, msgs) : GameModel * GameMsg list) cmd  = 
                match state.CurrentScene with
                | Title -> 
                    match cmd with
                    | Start -> 
                        let gameplayScene = this.Content.Load<Scene>("GameplayScene")
                        this.SceneSystem.SceneInstance.RootScene.Children.Clear()
                        this.SceneSystem.SceneInstance.RootScene.Children.Add(gameplayScene)
                        let (newPlayerModel,playerMessage) = Player.init (gameplayScene) (this.Input)
                        let (newPlatformodel,platformMessage) = Platform.init (gameplayScene)
                        let (newGoalmodel,GoalMessage) = Goal.init (gameplayScene)
                        let (newUiModel,UiMessage) = UI.init (gameplayScene)
                        { state with CurrentScene = GamePlay; PlayerModel = newPlayerModel; PlatformModel = newPlatformodel; GoalModel = newGoalmodel; UiModel = newUiModel; StartTime = gameTime.Total }, msgs@ [playerMessage; platformMessage; GoalMessage; UiMessage]
                    | _ -> state, msgs
                | GamePlay ->      
                    match cmd with
                    | PlayerMsg(m) -> 
                        let newModel, newMsg = Player.update m state.PlayerModel (float32 gameTime.Elapsed.TotalSeconds)
                        { state with PlayerModel = newModel }, msgs @ [newMsg]
                    | PlatformMsg(m) -> 
                        let newModel, newMsg = Platform.update m state.PlatformModel (float32 gameTime.Elapsed.TotalSeconds)
                        { state with PlatformModel = newModel }, msgs @ [newMsg]
                    | UiMsg(m) -> 
                        let newModel, newMsg = UI.update m state.UiModel (float32 gameTime.Elapsed.TotalSeconds)
                        { state with UiModel = newModel }, msgs @ [newMsg]
                    | GoalMsg(m) ->
                        let newModel, newMsg = Goal.update m state.GoalModel (float32 gameTime.Elapsed.TotalSeconds)
                        { state with GoalModel = newModel }, msgs @ [newMsg]
                    | Collect ->
                        let newTime = state.CurrentTime @ [gameTime.Total - state.StartTime]
                        { state with CurrentTime = newTime }, msgs
                    | Goal when state.PlayerModel.Counter = 2 ->
                        this.Input.UnlockMousePosition()
                        this.IsMouseVisible <- true
                        let scoreScene = this.Content.Load<Scene>("ScoreScene")
                        this.SceneSystem.SceneInstance.RootScene.Children.Clear()
                        this.SceneSystem.SceneInstance.RootScene.Children.Add(scoreScene)
                        let newTime = state.CurrentTime @ [gameTime.Total - state.StartTime]
                        { state with CurrentScene = Score; CurrentTime = newTime }, msgs
                    | Goal ->                        
                        state, msgs
                    | Empty -> state, msgs
                    | _ -> state, msgs
                | Score -> 
                    state, msgs

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