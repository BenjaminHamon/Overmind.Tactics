using Newtonsoft.Json;
using Overmind.Tactics.Model.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Overmind.Tactics.Model
{
	[Serializable]
	public class Game
	{
		public void Initialize(JsonSerializer serializer)
		{
			this.serializer = serializer;
		}

		private JsonSerializer serializer;

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
			string serializedState;
			using (StringWriter stringWriter = new StringWriter())
			using (JsonWriter jsonWriter = new JsonTextWriter(stringWriter))
			{
				serializer.Serialize(jsonWriter, state);
				serializedState = stringWriter.ToString();
			}

			using (StringReader stringReader = new StringReader(serializedState))
			using (JsonReader jsonReader = new JsonTextReader(stringReader))
				SaveState = serializer.Deserialize<GameState>(jsonReader);

			ActiveState = state;
			ActiveState.CommandHistory = new List<IGameCommand>();
		}

		public void Save(string path, GameState state)
		{
			using (StreamWriter streamWriter = new StreamWriter(path))
			using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
				serializer.Serialize(jsonWriter, state);
		}

		public void Load(string path)
		{
			string serializedState = File.ReadAllText(path);

			using (StringReader stringReader = new StringReader(serializedState))
			using (JsonReader jsonReader = new JsonTextReader(stringReader))
				SaveState = serializer.Deserialize<GameState>(jsonReader);

			using (StringReader stringReader = new StringReader(serializedState))
			using (JsonReader jsonReader = new JsonTextReader(stringReader))
				ActiveState = serializer.Deserialize<GameState>(jsonReader);

			Dictionary<string, CharacterClass> characterClassCollection = new Dictionary<string, CharacterClass>();
			foreach (Character character in ActiveState.CharacterCollection)
			{
				if (characterClassCollection.ContainsKey(character.CharacterClass_Key) == false)
				{
					using (StreamReader streamReader = new StreamReader("Assets/Characters/" + character.CharacterClass_Key + ".json"))
					using (JsonReader jsonReader = new JsonTextReader(streamReader))
						characterClassCollection[character.CharacterClass_Key] = serializer.Deserialize<CharacterClass>(jsonReader);
				}

				character.Owner = ActiveState.PlayerCollection.Single(player => player.Id == character.OwnerId);
				character.CharacterClass = characterClassCollection[character.CharacterClass_Key];
			}

			if (ActiveState.ActivePlayerId != Guid.Empty)
				ActiveState.ActivePlayer = ActiveState.PlayerCollection.Single(player => player.Id == ActiveState.ActivePlayerId);

			ActiveState.CommandHistory = new List<IGameCommand>();
		}
	}
}
