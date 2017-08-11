using System;
using System.Collections.Generic;
using System.Linq;

namespace Overmind.Tactics.Model.Abilities
{
	public class AreaAbility : IAbility
	{
		public string Name { get; set; }
		public string Icon { get; set; }
		public int ActionPoints { get; set; }
		public int Range { get; set; }

		public int Power;
		public int TargetWidth;
		public int TargetHeight;
		public bool TargetRequired;
		public List<TargetType> TargetTypes;

		public int GetRotation(Vector2 casterPosition, Vector2 targetPosition)
		{
			Vector2 targetRelativeToCaster = targetPosition - casterPosition;
			return Math.Abs(targetRelativeToCaster.Y) < Math.Abs(targetRelativeToCaster.X) ? 90 : 0;
		}

		public bool Cast(GameState game, Character caster, Vector2 targetCenter)
		{
			if ((targetCenter - caster.Position).Norm > Range)
				return false;

			List<Character> targetCollection = game.CharacterFinder
				.GetCharactersAround(targetCenter, TargetWidth, TargetHeight, GetRotation(caster.Position, targetCenter))
				.Where(target => TargetTypeExtensions.IsTargetAllowed(TargetTypes, caster, target)).ToList();
			if (TargetRequired && (targetCollection.Any() == false))
				return false;

			foreach (Character currentTarget in targetCollection)
				Apply(currentTarget);
			
			return true;
		}

		private void Apply(Character target)
		{
			target.HealthPoints -= Power;
		}
	}
}
