namespace GameJam.Core

open Stride.Core.Mathematics
open Stride.Engine;
open Stride.Games;
open SceneManager
open System

type MvuGame() =
    inherit Game()
    
    let mutable Model, Messages = Game.empty, []
        
    override this.BeginRun () = 
        let mainScene = this.Content.Load<Scene>("MainScene")
        let titleScene = this.Content.Load<Scene>("TitleScene")
        mainScene.Children.Add(titleScene)
        let model, messages = Game.init this
        Model <- model
        Messages <- messages
        
    member this.updateMessage (updateFunc) (gameModel : Game.Model) (cmds : Game.Msg list) (gameTime : GameTime) (game : Game) =
        let updateFold ((gameModel, messages) : Game.Model * Game.Msg list) cmd = 
            let (model, message) = updateFunc gameModel cmd gameTime game
            model, messages @ message

        let newModel, newMessages = List.fold updateFold (gameModel, []) cmds
        newModel , newMessages

    member this.updateGame gameTime (gameModel, messages) cmd = 
        let (model,message) = Game.update gameModel cmd gameTime this
        model, messages @ message

    override this.Update gameTime =
        base.Update(gameTime);
            
        Messages <- Messages @ Game.getMsg ()

        this.DebugTextSystem.Print(sprintf "%A" Messages, new Int2(50,200))

        let model, messages = List.fold (this.updateGame gameTime) (Model, []) Messages

        Model <- model
        Messages <- (List.distinct messages)

        Game.view Model gameTime |> ignore

    override this.Destroy () =
        ()