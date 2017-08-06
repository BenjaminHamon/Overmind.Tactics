using System.Collections.Generic;
using System.Linq;

namespace Overmind.Tactics.Model.Abilities
{
	public class ProjectileAbility : IAbility
	{
		public string Name { get; set; }
		public string Icon { get; set; }
		public int ActionPoints { get; set; }
		public int Range { get; set; }

		public int Power;
		public int TargetLimit;
		public List<TargetType> TargetTypes;

		public int GetRotation(Vector2 casterPosition, Vector2 targetPosition)
		{
			return 0;
		}

		public bool Cast(GameState game, Character caster, Vector2 targetCenter)
		{
			if ((caster.ActionPoints < ActionPoints) || ((targetCenter - caster.Position).Norm > Range))
				return false;

			IEnumerable<Character> targetCollection = game.CharacterFinder.GetCharactersOnLine(caster.Position, targetCenter)
				.Where(target => TargetTypeExtensions.IsTargetAllowed(TargetTypes, caster, target))
				.OrderBy(target => (caster.Position - target.Position).Norm)
				.Take(TargetLimit);

			foreach (Character currentTarget in targetCollection)
				Apply(currentTarget);
			
			caster.ActionPoints -= ActionPoints;
			return true;
		}

		private void Apply(Character target)
		{
			target.HealthPoints -= Power;
		}
	}
}
