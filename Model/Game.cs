using Overmind.Tactics.Model.Commands;
using System;
using System.Collections.Generic;

namespace Overmind.Tactics.Model
{
	[Serializable]
	public class Game
	{
		public void Initialize(ContentProvider contentProvider)
		{
			this.contentProvider = contentProvider;
		}

		private ContentProvider contentProvider;

		public GameState ActiveState = new GameState();
		public GameState SaveState = new GameState();

		public void ExecuteCommand(IGameCommand command)
		{
			if (ActiveState.IsRunning == false)
				return;

			if (command.TryExecute(ActiveState))
				SaveState.CommandHistory.Add(command);
		}

		public void SetState(GameState state)
		{
			SaveState = contentProvider.Copy(state);
			ActiveState = state;
			ActiveState.CommandHistory = new List<IGameCommand>();
		}

		public void LoadScenario(string path) { SetState(contentProvider.LoadScenario(path)); }
		public void SaveScenario(string path, GameState gameState)
		{
			if (gameState.Turn == 0)
			{
				// Reset some data set by the game initialization so that it is not serialized
				foreach (Character character in gameState.CharacterCollection)
				{
					character.HealthPoints = 0;
					character.ActionPoints = 0;
				}
			}

			contentProvider.SaveScenario(path, gameState);
		}
		
		public void LoadGame(string path) { SetState(contentProvider.LoadGame(path)); }
		public void SaveGame(string path, GameState gameState) { contentProvider.SaveGame(path, gameState); }
	}
}
