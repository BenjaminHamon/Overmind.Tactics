using System;
using System.Collections.Generic;
using System.Linq;

namespace Overmind.Tactics.Model.Navigation
{
	public class AstarNavigation : INavigation
	{
		public AstarNavigation(Func<Vector2, bool> isTileAccessible)
		{
			this.isTileAccessible = isTileAccessible;
		}

		private readonly Func<Vector2, bool> isTileAccessible;

		public List<Vector2> FindPath(Vector2 start, Vector2 end)
		{
			start = new Vector2(Convert.ToSingle(Math.Round(start.X)), Convert.ToSingle(Math.Round(start.Y)));
			end = new Vector2(Convert.ToSingle(Math.Round(end.X)), Convert.ToSingle(Math.Round(end.Y)));

			if (start == end)
				return new List<Vector2>();

			List<PathNode> knownNodes = new List<PathNode>();
			PathNode startNode = new PathNode()
			{
				Position = start,
				Accessible = true,
				DirectDistanceToDestination = (end - start).Norm,
			};
			knownNodes.Add(startNode);

			PathNode currentNode = startNode;
			PathNode endNode = null;

			int loopIndex = 0;
			while ((endNode == null) && (currentNode != null))
			{
				foreach (Vector2 allowedMove in NavigationRules.AllowedMoveCollection)
				{
					Vector2 nextPosition = currentNode.Position + allowedMove;
					PathNode nextNode = knownNodes.FirstOrDefault(n => n.Position == nextPosition);
					if (nextNode == null)
					{
						nextNode = new PathNode()
						{
							Position = nextPosition,
							Accessible = isTileAccessible(nextPosition),
							DirectDistanceToDestination = (end - nextPosition).Norm,
							PathCost = Int32.MaxValue,
						};
						knownNodes.Add(nextNode);
					}

					if (nextNode.Accessible)
					{
						if (nextNode.PathCost > currentNode.PathCost + 1)
						{
							nextNode.PathCost = currentNode.PathCost + 1;
							nextNode.Predecessor = currentNode;
							if (nextNode.Position == end)
							{
								endNode = nextNode;
								break;
							}
						}
					}
				}

				currentNode.Explored = true;

				currentNode = knownNodes.Where(n => (n.Explored == false) && n.Accessible).OrderBy(n => n.DirectDistanceToDestination).FirstOrDefault();
				loopIndex += 1;
				if (loopIndex > 1000)
				{
					//Debug.LogWarning("[Navigation] FindPath failed: took too long");
					break;
				}
			}

			if (endNode == null)
			{
				//Debug.LogWarning("[Navigation] FindPath failed: could not reach destination");
				return new List<Vector2>();
			}

			List<Vector2> path = new List<Vector2>();
			currentNode = endNode;
			while (currentNode != startNode)
			{
				path.Add(currentNode.Position);
				currentNode = currentNode.Predecessor;
			}
			path.Reverse();
			return path;
		}
	}
}
