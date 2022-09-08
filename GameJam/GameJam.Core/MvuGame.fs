namespace GameJam.Core

open Stride.Core.Mathematics
open Stride.Engine;
open Stride.Games;
open SceneManager
open System

type MvuGame() =
    inherit Game()
            
    let mutable State, Messages = Game.empty, []
        
    override this.BeginRun () = 
        let mainScene = this.Content.Load<Scene>("MainScene")
        let titleScene = this.Content.Load<Scene>("TitleScene")
        mainScene.Children.Add(titleScene)
        let model, messages = Game.init this
        State <- model
        Messages <- messages    

    override this.Update gameTime =
        base.Update(gameTime);
            
        Messages <- Messages @ Game.mapAllEvent ()

        this.DebugTextSystem.Print(sprintf "%A" Messages, new Int2(50,200))

        let model, messages = Game.update State Messages gameTime this
        State <- model
        Messages <- messages

        Game.view State gameTime |> ignore

    override this.Destroy () =
        ()