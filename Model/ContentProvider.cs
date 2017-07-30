using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Overmind.Tactics.Model
{
	public class ContentProvider
	{
		public ContentProvider(JsonSerializer serializer, string contentDirectory, string userDirectory)
		{
			this.serializer = serializer;
			this.contentDirectory = contentDirectory;
			this.userDirectory = userDirectory;
		}

		private readonly JsonSerializer serializer;
		private readonly string contentDirectory;
		private readonly string userDirectory;
		private readonly Dictionary<string, CharacterClass> characterClassCollection = new Dictionary<string, CharacterClass>();

		public CharacterClass GetCharacterClass(string name)
		{
			if (characterClassCollection.ContainsKey(name) == false)
				characterClassCollection[name] = Load<CharacterClass>(contentDirectory, "Characters/" + name);
			return characterClassCollection[name];
		}

		public GameState LoadScenario(string path) { return Load<GameState>(contentDirectory, "Scenarios/" + path); }
		public void SaveScenario(string path, GameState gameState) { Save(contentDirectory, "Scenarios/" + path, gameState); }

		public GameState LoadGame(string path) { return Load<GameState>(userDirectory, "Saves/" + path); }
		public void SaveGame(string path, GameState gameState) { Save(userDirectory, "Saves/" + path, gameState); }

		private TData Load<TData>(string directory, string path)
		{
			path = Path.Combine(directory, path + ".json");
			using (StreamReader streamReader = new StreamReader(path))
			using (JsonReader jsonReader = new JsonTextReader(streamReader))
				return serializer.Deserialize<TData>(jsonReader);
		}

		private void Save<TData>(string directory, string path, TData data)
		{
			path = Path.Combine(directory, path + ".json");
			Directory.CreateDirectory(Path.GetDirectoryName(path));

			using (StreamWriter streamWriter = new StreamWriter(path))
			using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
				serializer.Serialize(jsonWriter, data);
		}

		/// <summary>Creates a object deep copy by serializing and deserializing it.</summary>
		public TData Copy<TData>(TData data)
		{
			string serializedState;
			using (StringWriter stringWriter = new StringWriter())
			using (JsonWriter jsonWriter = new JsonTextWriter(stringWriter))
			{
				serializer.Serialize(jsonWriter, data);
				serializedState = stringWriter.ToString();
			}

			using (StringReader stringReader = new StringReader(serializedState))
			using (JsonReader jsonReader = new JsonTextReader(stringReader))
				return serializer.Deserialize<TData>(jsonReader);
		}
	}
}
