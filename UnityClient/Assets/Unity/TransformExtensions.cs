using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Unity
{
	public static class TransformExtensions
	{
		public static void DestroyAllChildren(this Transform transform)
		{
			List<GameObject> children = transform.Cast<Transform>().Select(t => t.gameObject).ToList();
			foreach (GameObject child in children)
			{
				if (Application.isPlaying)
					UnityEngine.Object.Destroy(child);
				else
					UnityEngine.Object.DestroyImmediate(child);
			}
		}
	}
}
