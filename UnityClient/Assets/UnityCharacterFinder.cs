using Overmind.Tactics.Model;
using Overmind.Tactics.UnityClient.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overmind.Tactics.UnityClient
{
	public class UnityCharacterFinder : ICharacterFinder
	{
		public UnityCharacterFinder(LayerMask layerMask)
		{
			this.layerMask = layerMask;
		}

		private readonly LayerMask layerMask;

		public Character GetCharacter(Model.Vector2 position)
		{
			//Debug.LogFormat("[UnityCharacterFinder] GetCharacter (Position: {0})", position);
			//Debug.DrawLine(position.ToUnityVector() - new UnityEngine.Vector2(0.5f, 0.5f),
			//	position.ToUnityVector() + new UnityEngine.Vector2(0.5f, 0.5f), Color.red, 3);
			
			RaycastHit2D hit = Physics2D.Raycast(position.ToUnityVector(), UnityEngine.Vector2.zero, 0, layerMask);
			return hit.collider?.GetComponentInParent<CharacterView>()?.Model;
		}

		public IEnumerable<Character> GetCharactersAround(Model.Vector2 center, int width, int height, int rotation)
		{
			UnityEngine.Vector2 bottomLeft = new UnityEngine.Vector2(- width / 2f, -height / 2f);
			UnityEngine.Vector2 topRight = new UnityEngine.Vector2(width / 2f, height / 2f);
			bottomLeft = Quaternion.AngleAxis(rotation, UnityEngine.Vector3.forward) * bottomLeft;
			topRight = Quaternion.AngleAxis(rotation, UnityEngine.Vector3.forward) * topRight;
			bottomLeft += center.ToUnityVector();
			topRight += center.ToUnityVector();

			//Debug.LogFormat("[UnityCharacterFinder] GetCharactersAround (BottomLeft: {0}, TopRight: {1})",
			//	bottomLeft.ToModelVector(), topRight.ToModelVector());
			//Debug.DrawLine(bottomLeft, topRight, Color.red, 3);

			return Physics2D.OverlapAreaAll(bottomLeft, topRight, layerMask)
				.Select(target => target.GetComponentInParent<CharacterView>()?.Model).Where(target => (target != null));
		}
	}
}
