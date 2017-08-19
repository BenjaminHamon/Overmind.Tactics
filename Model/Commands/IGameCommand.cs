using Overmind.Tactics.Data;

namespace Overmind.Tactics.Model.Commands
{
	public interface IGameCommand : IGameCommandData
	{
		bool TryExecute(GameModel state);
	}
}
