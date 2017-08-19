using System.Linq;
using System.Runtime.Serialization;

namespace Overmind.Tactics.Model.Commands
{
	[DataContract]
	public class EndTurnCommand : IGameCommand
	{
		[DataMember(Name = nameof(Player))]
		public string PlayerId;
		public PlayerModel Player;

		public bool TryExecute(GameModel state)
		{
			if (Player == null)
				Player = state.PlayerCollection.Single(p => p.Id == PlayerId);

			Player.EndTurn();
			return true;
		}
	}
}
