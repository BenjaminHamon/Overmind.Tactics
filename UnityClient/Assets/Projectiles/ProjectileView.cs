using System.Collections;
using UnityEngine;

namespace Overmind.Tactics.UnityClient
{
	public class ProjectileView : MonoBehaviour
	{
		public Animator Animator;
		public Vector2 Target;
		public float Speed;

		public void Start()
		{
			Vector2 vectorToTarget = Target - (Vector2)transform.localPosition;
			transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg);

			StartCoroutine(Move());
		}

		private IEnumerator Move()
		{
			Vector2 origin = transform.localPosition;
			Vector2 vectorToTarget = Target - origin;
			float totalTime = vectorToTarget.magnitude / Speed;
			
			for (float travelRatio = 0; travelRatio < 1; travelRatio += Time.deltaTime / totalTime)
			{
				transform.localPosition = Vector2.Lerp(origin, Target, travelRatio);
				yield return new WaitForEndOfFrame();
			}
			
			Destroy(gameObject);
		}
	}
}
