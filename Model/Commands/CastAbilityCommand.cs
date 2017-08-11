using Overmind.Tactics.Model.Abilities;
using System.Linq;
using System.Runtime.Serialization;

namespace Overmind.Tactics.Model.Commands
{
	[DataContract]
	public class CastAbilityCommand : IGameCommand
	{
		[DataMember(Name = nameof(Character))]
		public string CharacterId;
		public Character Character;

		[DataMember(Name = nameof(Ability))]
		public string AbilityName;
		public IAbility Ability;

		[DataMember]
		public Vector2 Target;

		public bool TryExecute(GameState state)
		{
			if (Character == null)
				Character = state.CharacterCollection.Single(c => c.Id == CharacterId);
			if (Ability == null)
				Ability = Character.CharacterClass.Abilities.Single(a => a.Name == AbilityName);

			return Character.Cast(state, Ability, Target);
		}
	}
}
