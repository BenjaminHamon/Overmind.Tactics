using System;
using System.Collections.Generic;

namespace Overmind.Tactics.Model.Navigation
{
	public class BasicNavigation : INavigation
	{
		public BasicNavigation(Func<Vector2, bool> isTileAccessible)
		{
			this.isTileAccessible = isTileAccessible;
		}

		private readonly Func<Vector2, bool> isTileAccessible;

		public List<Vector2> FindPath(Vector2 start, Vector2 end)
		{
			start = new Vector2(Convert.ToSingle(Math.Round(start.X)), Convert.ToSingle(Math.Round(start.Y)));
			end = new Vector2(Convert.ToSingle(Math.Round(end.X)), Convert.ToSingle(Math.Round(end.Y)));

			List<Vector2> path = new List<Vector2>();
			Vector2 current = start;

			// Try to move towards the end point in a direct line
			// Iterate to find each unit step to follow the direct line.
			while (current != end)
			{
				Vector2 bestNext = Vector2.Zero;
				float bestDistance = Single.PositiveInfinity;

				foreach (Vector2 allowedMove in NavigationRules.AllowedMoveCollection)
				{
					Vector2 next = current + allowedMove;
					if (path.Contains(next) == false)
					{
						if (isTileAccessible(next))
						{
							float distance = (end - next).Norm;
							if (distance < bestDistance)
							{
								bestDistance = distance;
								bestNext = next;
							}
						}
					}
				}

				if (bestNext == Vector2.Zero)
				{
					//Debug.LogWarning("[Navigation] FindPath failed: no next move");
					break;
				}

				path.Add(bestNext);
				current = bestNext;

				if (path.Count > 100)
				{
					//Debug.LogWarning("[Navigation] FindPath failed: path too long");
					break;
				}
			}

			return path;
		}
	}
}
