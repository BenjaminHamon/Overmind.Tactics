# Overmind Tactics

Overmind Tactics is a turn-based tactics video game.

The project requires Visual Studio 2017 (or newer) and Unity 2019.2.

The solution `Overmind.Tactics.sln` includes libraries and dependencies external to Unity. It must be built from Visual Studio so that assemblies are copied to the Unity project `UnityClient`.

The Unity project includes a scene `MainMenuScene` as the main entry point.  
Alternatively, `GameScene` has a component `GameSceneManager` which is able to load scenarios (from `Assets/Resources/Scenarios`).
