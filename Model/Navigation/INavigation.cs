using System.Collections.Generic;

namespace Overmind.Tactics.Model.Navigation
{
	public interface INavigation
	{
		List<Vector2> FindPath(Vector2 start, Vector2 end);
	}
}
