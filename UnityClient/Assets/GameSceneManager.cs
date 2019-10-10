using Overmind.Tactics.Data;
using Overmind.Tactics.Model;
using Overmind.Tactics.UnityClient.Unity;
using System;
using UnityEngine;

namespace Overmind.Tactics.UnityClient
{
	public class GameSceneManager : MonoBehaviour
	{
		[SerializeField]
		private GameObject gamePrefab;
		[SerializeField]
		private GameView gameView;

		[SerializeField]
		private GameObject replayControllerPrefab;
		[SerializeField]
		private ReplayController replayController;

		public string GameSavePath;
		public string GameScenarioPath;
		public string GameReplayPath;

		// The application initialization when running for release is model first.
		// We load a game data, create a model for it and then apply it to the view which initializes itself accordingly.
		// For development, the Unity editor is used to test things out and create scenarios, so we need view first initialization as well.
		// A view expects an already initialized model, otherwise it creates one from the data serialized by Unity.

		// The application maintains several game states:
		//   ActiveData, the current game state according to the underlying game model
		//   SaveData, a copy of the initial game state with a command history tracking changes to get to the active state
		//   The scene, the game state corresponding to game objects instantiated by Unity
		//
		// In game, the active state is the the state the game relies upon.
		// The save state can be used to replay commands to get to any past point in the game.
		// The scene can be modified in the editor directly and used to update the model to create game scenarios.

		private void Awake()
		{
			Debug.LogFormat(this, "[GameSceneManager] Awake");

			if (gameView != null) // view first
			{
				gameView.UpdateModelFromScene();
			}
			else // model first
			{
				if (UnityApplication.GameLoadRequest != null)
				{
					GameSavePath = UnityApplication.GameLoadRequest;
				}
				else if (UnityApplication.ReplayLoadRequest != null)
				{
					GameReplayPath = UnityApplication.ReplayLoadRequest;
				}
				else if (UnityApplication.ScenarioLoadRequest != null)
				{
					GameScenarioPath = UnityApplication.ScenarioLoadRequest;
				}

				UnityApplication.GameLoadRequest = null;
				UnityApplication.ReplayLoadRequest = null;
				UnityApplication.ScenarioLoadRequest = null;

				if (String.IsNullOrEmpty(GameSavePath) == false)
				{
					LoadGame(GameSavePath);
				}
				else if (String.IsNullOrEmpty(GameReplayPath) == false)
				{
					LoadReplay(GameReplayPath);
				}
				else if (String.IsNullOrEmpty(GameScenarioPath) == false)
				{
					LoadScenario(GameScenarioPath);
				}
				else
				{
					SetGame(new GameData());
				}
			}
		}

		public void SaveScenario(string path)
		{
			Debug.LogFormat(this, "[GameSceneManager] Saving scenario to {0}", path);
			UnityApplication.DataProvider.SaveScenario(path, gameView.Model.ActiveData);
		}

		public void SaveGame(string path)
		{
			Debug.LogFormat(this, "[GameSceneManager] Saving game to {0}", path);
			UnityApplication.DataProvider.SaveGame(path, gameView.Model.ActiveData);
		}

		public void SaveReplay(string path)
		{
			Debug.LogFormat(this, "[GameSceneManager] Saving replay to {0}", path);
			UnityApplication.DataProvider.SaveReplay(path, gameView.Model.ReplayData);
		}

		public void LoadScenario(string path)
		{
			Debug.LogFormat(this, "[GameSceneManager] Loading scenario from {0}", path);
			GameData gameData = UnityApplication.DataProvider.LoadScenario(path);
			SetGame(gameData);
		}

		public void LoadGame(string path)
		{
			Debug.LogFormat(this, "[GameSceneManager] Loading game from {0}", path);
			GameData gameData = UnityApplication.DataProvider.LoadGame(path);
			SetGame(gameData);
		}

		public void LoadReplay(string path)
		{
			Debug.LogFormat(this, "[GameSceneManager] Loading replay from {0}", path);

			if (replayController != null)
			{
				DestroyImmediate(replayController.gameObject);
				replayController = null;
			}

			GameData gameData = UnityApplication.DataProvider.LoadReplay(path);
			SetGame(gameData);

			replayController = GameObjectExtensions.Instantiate(replayControllerPrefab, null).GetComponent<ReplayController>();
			replayController.GameView = gameView;
			replayController.Resume();
		}

		private void SetGame(GameData gameData)
		{
			if (gameView != null)
			{
				DestroyImmediate(gameView.gameObject);
				gameView = null;
			}

			GameModel model = new GameModel(gameData, UnityApplication.DataProvider);
			gameView = GameObjectExtensions.Instantiate(gamePrefab, null).GetComponent<GameView>();
			gameView.gameObject.name = "Game";
			gameView.Data = gameData;
			gameView.Model = model;
			gameView.ApplyModelToScene();
		}
	}
}
