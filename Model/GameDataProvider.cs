using Newtonsoft.Json;
using Overmind.Tactics.Data;
using System.Collections.Generic;

namespace Overmind.Tactics.Model
{
	public class GameDataProvider : DataProvider
	{
		public GameDataProvider(JsonSerializer serializer, string contentDirectory, string userDirectory)
			: base(serializer, contentDirectory, userDirectory)
		{ }

		private readonly Dictionary<string, CharacterClass> characterClassCollection = new Dictionary<string, CharacterClass>();

		public CharacterClass GetCharacterClass(string name)
		{
			if (characterClassCollection.ContainsKey(name) == false)
				characterClassCollection[name] = LoadContent<CharacterClass>("Characters/" + name);
			return characterClassCollection[name];
		}

		public GameData LoadScenario(string path) { return LoadContent<GameData>("Scenarios/" + path); }
		public void SaveScenario(string path, GameData gameData)
		{
			if (gameData.Turn == 0)
			{
				// Reset some data set by the game initialization so that it is not serialized
				gameData = Copy(gameData);
				foreach (CharacterData character in gameData.CharacterCollection)
				{
					character.HealthPoints = 0;
					character.ActionPoints = 0;
				}
			}

			SaveContent("Scenarios/" + path, gameData);
		}

		public GameData LoadGame(string path) { return LoadUserData<GameData>("Saves/" + path); }
		public void SaveGame(string path, GameData gameData) { SaveUserData("Saves/" + path, gameData); }
	}
}
