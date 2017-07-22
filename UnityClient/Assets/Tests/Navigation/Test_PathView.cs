using Overmind.Tactics.UnityClient.Navigation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Tests.Navigation
{
	internal class Test_PathView : MonoBehaviour
	{
		public PathView PathView;

		public void Start()
		{
			StartCoroutine(Run());
		}

		public IEnumerator Run()
		{
			PathView.ShowPath(Vector2.zero, new List<Vector2>());
			yield return new WaitForSeconds(3);
			PathView.Reset();
			yield return new WaitForSeconds(1);

			PathView.ShowPath(Vector2.zero, new List<Vector2>() { new Vector2(1, 0), new Vector2(1, 1)});
			yield return new WaitForSeconds(3);
			PathView.Reset();
			yield return new WaitForSeconds(1);

			PathView.ShowPath(Vector2.zero, new List<Vector2>()
			{
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1),
				new Vector2(-1, 1),
				new Vector2(-1, 0),
				new Vector2(-1, -1),
				new Vector2(0, -1),
				new Vector2(1, -1),
			});
		}
	}
}