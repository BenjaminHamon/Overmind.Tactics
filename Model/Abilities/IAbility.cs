using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overmind.Tactics.Model.Abilities
{
	public interface IAbility
	{
		string Name { get; }
		string Icon { get; }
		int ActionPoints { get; }

		int GetRotation(Vector2 casterPosition, Vector2 targetPosition);
		bool Cast(GameState game, Character caster, Vector2 targetCenter);
	}
}
