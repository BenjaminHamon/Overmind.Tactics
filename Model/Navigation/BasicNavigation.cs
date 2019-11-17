using Overmind.Tactics.Data;
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
			List<Vector2> path = new List<Vector2>();
			Vector2 current = start;

			// Try to move towards the end point in a direct line
			// Iterate to find each unit step to follow the direct line.
			int loopIndex = 0;
			while (current != end)
			{
				Vector2? bestNext = null;
				float bestDistance = Single.PositiveInfinity;

				foreach (Vector2 allowedMove in NavigationRules.AllowedMoveCollection)
				{
					Vector2 next = current + allowedMove;

					if ((next != start) && (path.Contains(next) == false))
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

				// Found no path to destination
				if (bestNext == null)
					return new List<Vector2>();

				path.Add(bestNext.Value);
				current = bestNext.Value;

				loopIndex += 1;
				if (loopIndex > 1000)
					throw new TimeoutException("[BasicNavigation] FindPath timed out");
			}

			return path;
		}
	}
}
