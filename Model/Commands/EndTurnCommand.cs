using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Overmind.Tactics.Model.Commands
{
	[DataContract]
	public class EndTurnCommand : IGameCommand
	{
		[DataMember(Name = nameof(Player))]
		public Guid PlayerId;
		public Player Player;

		public bool TryExecute(GameState state)
		{
			if (Player == null)
				Player = state.PlayerCollection.Single(p => p.Id == PlayerId);

			Player.EndTurn();
			return true;
		}
	}
}
