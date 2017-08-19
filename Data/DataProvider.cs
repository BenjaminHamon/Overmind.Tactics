using Newtonsoft.Json;
using System.IO;

namespace Overmind.Tactics.Data
{
	public class DataProvider
	{
		public DataProvider(JsonSerializer serializer, string contentDirectory, string userDirectory)
		{
			this.serializer = serializer;
			this.contentDirectory = contentDirectory;
			this.userDirectory = userDirectory;
		}

		protected readonly JsonSerializer serializer;
		private readonly string contentDirectory;
		private readonly string userDirectory;

		protected virtual TData LoadContent<TData>(string path) { return Load<TData>(contentDirectory, path); }
		protected virtual void SaveContent<TData>(string path, TData data) { Save(contentDirectory, path, data); }

		protected virtual TData LoadUserData<TData>(string path) { return Load<TData>(userDirectory, path); }
		protected virtual void SaveUserData<TData>(string path, TData data) { Save(userDirectory, path, data); }

		protected virtual TData Load<TData>(string directory, string path)
		{
			path = Path.Combine(directory, path + ".json");
			using (StreamReader streamReader = new StreamReader(path))
			using (JsonReader jsonReader = new JsonTextReader(streamReader))
				return serializer.Deserialize<TData>(jsonReader);
		}

		protected virtual void Save<TData>(string directory, string path, TData data)
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
