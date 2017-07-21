namespace Overmind.Tactics.Model.Navigation
{
	public class PathNode
	{
		public Vector2 Position;
		public float DirectDistanceToDestination;
		public PathNode Predecessor;
		public int PathCost;
		public bool Accessible;
		public bool Explored;
	}
}
