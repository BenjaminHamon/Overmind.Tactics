using Overmind.Tactics.Data;

namespace Overmind.Tactics.Model.Abilities
{
	public interface IAbility
	{
		string Name { get; }
		string Icon { get; }
		int ActionPoints { get; }
		int Range { get; }

		int GetRotation(Vector2 casterPosition, Vector2 targetPosition);
		bool Cast(ICharacterFinder characterFinder, CharacterModel caster, Vector2 targetCenter);
	}
}
