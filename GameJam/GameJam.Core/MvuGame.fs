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
            GameplayModel : GameplayScene.Model
            ScoreModel : ScoreScene.Model
            CurrentScene : CurrentScene
            StartTime: TimeSpan
            CurrentTime: TimeSpan list
            BestTime: TimeSpan list
        }

    type MvuGame() =
        inherit Game()

        let init () =
            { GameplayModel = GameplayScene.empty (); ScoreModel = ScoreScene.empty (); CurrentScene = Title; StartTime = TimeSpan.Zero; CurrentTime = []; BestTime = []}, []

        let view (state : GameModel) (gameTime : GameTime) =
            match state.CurrentScene with
            | Title -> 
                ()
            | GamePlay -> 
                GameplayScene.view state.GameplayModel gameTime
            | Score -> 
                ScoreScene.view state.ScoreModel gameTime
            
        let mutable State, Messages = init ()
        let mutable NextMessages : GameMsg list = []
        
        override this.BeginRun () = 
            let titleScene = this.Content.Load<Scene>("TitleScene")
            this.SceneSystem.SceneInstance.RootScene.Children.Add(titleScene)
            ()

        member private this.GameUpdate (cmds : GameMsg list) (state : GameModel) (gameTime : GameTime) =
            let scoreScene = this.Content.Load<Scene>("ScoreScene")

            let GameUpdateFold ((state, msgs) : GameModel * GameMsg list) cmd  = 
                match cmd with
                | TitleSceneMsg(m) ->
                    match m with
                    | Start -> 
                        let titleScene = this.Content.Load<Scene>("TitleScene")
                        this.SceneSystem.SceneInstance.RootScene.Children.Remove(titleScene) |> ignore
                        this.Content.Unload(titleScene)
                        let gameplayScene = this.Content.Load<Scene>("GameplayScene")
                        this.SceneSystem.SceneInstance.RootScene.Children.Add(gameplayScene)
                        GameJam.Events.MusicEventKey.Broadcast("Gameplay");
                        this.Input.LockMousePosition()
                        this.IsMouseVisible <- false
                        let (gameplaySceneModel,gameplaySceneInitMessage) = GameplayScene.init gameplayScene this.Input
                        { state with CurrentScene = GamePlay; GameplayModel = gameplaySceneModel; StartTime = gameTime.Total; CurrentTime = [] }, gameplaySceneInitMessage
                | GameplaySceneMsg(m) ->
                    let (gameplaySceneModel,gameplaySceneInitMessage) = GameplayScene.update m state.GameplayModel (float32 gameTime.Elapsed.TotalSeconds)
                    { state with GameplayModel = gameplaySceneModel }, msgs @ gameplaySceneInitMessage
                | ScoreSceneMsg(m) ->
                    state, msgs
                | Collect ->
                    let newTime = state.CurrentTime @ [gameTime.Total - state.StartTime]
                    { state with CurrentTime = newTime }, msgs
                | Goal when state.GameplayModel.PlayerModel.Counter = 14 ->
                    this.Input.UnlockMousePosition()
                    this.IsMouseVisible <- true
                    
                    let gameplayScene = this.Content.Load<Scene>("GameplayScene")
                    this.SceneSystem.SceneInstance.RootScene.Children.Remove(gameplayScene) |> ignore
                    this.Content.Unload(gameplayScene)
                    let scoreScene = this.Content.Load<Scene>("ScoreScene")
                    this.SceneSystem.SceneInstance.RootScene.Children.Add(scoreScene)
                    GameJam.Events.MusicEventKey.Broadcast("Gameplay");
                    let newTime = state.CurrentTime @ [gameTime.Total - state.StartTime]
                    let (scoreSceneModel,scoreSceneInitMessage) = ScoreScene.init newTime state.BestTime scoreScene
                    { state with CurrentScene = Score; ScoreModel = scoreSceneModel; CurrentTime = newTime }, []  // Clear msgs since score scene doesn't have any messages at initialization
                | Goal ->            
                    state, msgs
                | Restart -> 
                    let scoreScene = this.Content.Load<Scene>("ScoreScene")
                    this.SceneSystem.SceneInstance.RootScene.Children.Remove(scoreScene) |> ignore
                    this.Content.Unload(scoreScene)
                    let titleScene = this.Content.Load<Scene>("TitleScene")
                    this.SceneSystem.SceneInstance.RootScene.Children.Add(titleScene)
                    let bestTime = 
                        if state.BestTime.Length > 0 then
                            if state.CurrentTime.Item(1) > state.BestTime.Item(1) then state.CurrentTime else state.BestTime
                        else
                            state.CurrentTime
                    { state with CurrentScene = Title; BestTime = bestTime}, []

            let newState, nextMessages = List.fold GameUpdateFold (State, []) cmds
            State <- newState
            NextMessages <- NextMessages @ (List.filter(fun m -> match m with | Empty -> false | _ -> true ) (List.distinct nextMessages))

        
        override this.Update gametime =
            base.Update(gametime);
            
            Messages <- Messages @ Event.ProcessAllEvent ()

            this.DebugTextSystem.Print(sprintf "X:%f\n\nY:%f\n\nZ:%f" State.GameplayModel.PlayerModel.Entity.Transform.Position.X State.GameplayModel.PlayerModel.Entity.Transform.Position.Y State.GameplayModel.PlayerModel.Entity.Transform.Position.Z, new Int2(50,200))
            this.GameUpdate Messages State gametime
            view State gametime


            Messages <- NextMessages
            NextMessages <- []


        override this.Destroy () =
            ()