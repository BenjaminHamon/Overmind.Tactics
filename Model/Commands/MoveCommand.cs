using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace Overmind.Tactics.Model.Commands
{
	[DataContract]
	public class MoveCommand : IGameCommand
	{
		[DataMember(Name = nameof(Character))]
		public Guid CharacterId;
		public Character Character;

		[DataMember]
		public List<Vector2> Path;

		public bool TryExecute(GameState state)
		{
			if (Character == null)
				Character = state.CharacterCollection.Single(c => c.Id == CharacterId);

			return Character.Move(Path);
		}
	}
}
