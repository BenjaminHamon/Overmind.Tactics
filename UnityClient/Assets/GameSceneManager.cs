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

		public string GameScenarioPath;
		public string GameSavePath;

		public void Start()
		{
			if (String.IsNullOrEmpty(GameSavePath) == false)
				Load(GameSavePath, false);
			else if (String.IsNullOrEmpty(GameScenarioPath) == false)
				Load(GameScenarioPath, true);
			else
				SetGame(new GameData());
		}

		public void SaveScenario(string path)
		{
			UnityApplication.DataProvider.SaveScenario(path, gameView.Model.ActiveData);
			Debug.LogFormat(this, "[GameSceneManager] Saved to {0}", path);
		}

		public void SaveGame(string path, bool withHistory)
		{
			UnityApplication.DataProvider.SaveGame(path, withHistory ? gameView.Model.SaveData : gameView.Model.ActiveData);
			Debug.LogFormat(this, "[GameSceneManager] Saved to {0}", path);
		}

		public void Load(string path, bool asScenario)
		{
			GameData gameData = asScenario ? UnityApplication.DataProvider.LoadScenario(path) : UnityApplication.DataProvider.LoadGame(path);
			SetGame(gameData);
			Debug.LogFormat(this, "[GameSceneManager] Loaded from {0}", path);
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
			gameView.Model = model;

			if (Application.isPlaying == false)
				gameView.ApplyModelToScene();
		}
	}
}
