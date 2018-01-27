using Newtonsoft.Json;
using Overmind.Tactics.Data;
using Overmind.Tactics.Model;
using Overmind.Tactics.Model.Navigation;
using Overmind.Tactics.UnityClient.Unity;
using UnityEngine;

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
			DataProvider = new UnityDataProvider(serializer, null, Application.persistentDataPath);
#endif
		}

		public static readonly UnityDataProvider DataProvider;

		public static string GameLoadRequest;
		public static string ScenarioLoadRequest;
	}
}
