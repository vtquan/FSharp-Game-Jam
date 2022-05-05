namespace GameJam.Core.Message

type CurrentScene =
    | Title
    | GamePlay
    | Score
    | Load

type SceneManagerMsg =
    | SwitchScene of CurrentScene