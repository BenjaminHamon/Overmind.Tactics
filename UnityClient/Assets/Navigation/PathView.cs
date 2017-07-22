using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Navigation
{
	public class PathView : MonoBehaviour
	{
		public GameObject NodePrefab;
		public GameObject LinkPrefab;

		public void ShowPath(Vector2 origin, List<Vector2> path)
		{
			Reset();

			Transform parent = transform;
			Instantiate(NodePrefab, parent).transform.localPosition = origin;

			for (int pathIndex = 0; pathIndex < path.Count; pathIndex++)
			{
				Vector2 move = (pathIndex == 0 ? origin : path[pathIndex - 1]) - path[pathIndex];
				Transform link = Instantiate(LinkPrefab, parent).transform;
				link.localPosition = path[pathIndex] + move / 2;
				if (move.x == 0)
					link.Rotate(Vector3.forward * 90);
				Instantiate(NodePrefab, parent).transform.localPosition = path[pathIndex];
			}
		}

		public void Reset()
		{
			List<GameObject> children = transform.Cast<Transform>().Select(t => t.gameObject).ToList();
			foreach (GameObject child in children)
				Destroy(child);
		}
	}
}
