using Overmind.Tactics.Data;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Overmind.Tactics.Model.Commands
{
	[DataContract]
	public class MoveCommand : IGameCommand
	{
		[DataMember(Name = nameof(Character))]
		public string CharacterId;
		public CharacterModel Character;

		[DataMember]
		public List<Vector2> Path;

		public bool TryExecute(GameModel state)
		{
			if (Character == null)
				Character = state.CharacterCollection.Single(c => c.Id == CharacterId);

			return Character.Move(Path);
		}
	}
}
