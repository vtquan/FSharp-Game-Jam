namespace GameJam.Core

open Stride.Core.Collections
open Stride.Core.Mathematics
open Stride.Engine
open Stride.UI;
open Stride.UI.Controls;
open Stride.Games;
open Stride.Physics
open System.Linq
open Messages
open Stride.Rendering.Sprites
open Stride.Input
open System
open Stride.Core.Serialization.Contents

module SceneManager =
    type CurrentScene =
        | Title
        | GamePlay
        | Score