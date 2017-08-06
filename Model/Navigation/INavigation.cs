using System.Collections.Generic;

namespace Overmind.Tactics.Model.Navigation
{
	public interface INavigation
	{
		/// <summary>Finds a path to navigate from a start point to an end point.</summary>
		/// <exception cref="System.TimeoutException">Thrown if the navigation could not find a path in a timely manner.</exception>
		List<Vector2> FindPath(Vector2 start, Vector2 end);
	}
}
