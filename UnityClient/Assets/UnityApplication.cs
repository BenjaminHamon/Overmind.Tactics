using Newtonsoft.Json;

namespace Overmind.Tactics.UnityClient
{
	public static class UnityApplication
	{
		static UnityApplication()
		{
			JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };

#if UNITY_EDITOR
			DataProvider = new UnityDataProvider(serializer, "Assets/Resources", "UserData");
#else
			DataProvider = new UnityDataProvider(serializer, null, UnityEngine.Application.persistentDataPath);
#endif
		}

		public static readonly UnityDataProvider DataProvider;

		public static string GameLoadRequest;
		public static string ReplayLoadRequest;
		public static string ScenarioLoadRequest;
	}
}
