#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Unity
{
	public static class GameObjectExtensions
	{
		public static GameObject Instantiate(GameObject prefab, Transform parent)
		{
#if UNITY_EDITOR
			GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
			instance.transform.SetParent(parent);
			return instance;
#else
			return GameObject.Instantiate(prefab, parent);
#endif
		}
	}
}
