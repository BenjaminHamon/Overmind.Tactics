using System.Collections;
using UnityEngine;

namespace Overmind.Tactics.UnityClient
{
	public class StatusText : MonoBehaviour
	{
		public TextMesh TextElement;

		private const float duration = 1;

		public void Start()
		{
			StartCoroutine(DestroyAfterDuration());
		}

		public IEnumerator DestroyAfterDuration()
		{
			yield return new WaitForSeconds(duration);
			Destroy(gameObject);
		}
	}
}
