using System.Collections.Generic;

namespace Overmind.Tactics.Model.Navigation
{
	public static class NavigationRules
	{
		public static readonly List<Vector2> AllowedMoveCollection = new List<Vector2>() { Vector2.Up, Vector2.Right, Vector2.Down, Vector2.Left };
	}
}
