namespace Overmind.Tactics.Model.Commands
{
	public interface IGameCommand
	{
		bool TryExecute(GameState state);
	}
}
