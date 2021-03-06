﻿using Newtonsoft.Json;
using Overmind.Tactics.Data;
using System;
using System.Collections.Generic;

namespace Overmind.Tactics.Model
{
	public class GameDataProvider : DataProvider
	{
		public GameDataProvider(JsonSerializer serializer, string contentDirectory, string userDirectory)
			: base(serializer, contentDirectory, userDirectory)
		{ }

		private readonly Dictionary<string, CharacterClass> characterClassCollection = new Dictionary<string, CharacterClass>();

		public CharacterClass LoadCharacterClass(string name)
		{
			if (String.IsNullOrEmpty(name))
				throw new ArgumentException("Name must not be empty", nameof(name));

			if (characterClassCollection.ContainsKey(name) == false)
				characterClassCollection[name] = LoadContent<CharacterClass>("Characters/" + name);
			return characterClassCollection[name];
		}

		public IEnumerable<string> ListScenarios() { return ListContentFiles("Scenarios"); }
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

		public IEnumerable<string> ListGames() { return ListUserFiles("Saves"); }
		public GameData LoadGame(string path) { return LoadUserData<GameData>("Saves/" + path); }
		public void SaveGame(string path, GameData gameData) { SaveUserData("Saves/" + path, gameData); }

		public IEnumerable<string> ListReplays() { return ListUserFiles("Replays"); }
		public GameData LoadReplay(string path) { return LoadUserData<GameData>("Replays/" + path); }
		public void SaveReplay(string path, GameData gameData) { SaveUserData("Replays/" + path, gameData); }
	}
}
