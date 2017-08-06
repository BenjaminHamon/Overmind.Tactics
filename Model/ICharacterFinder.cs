using System.Collections.Generic;

namespace Overmind.Tactics.Model
{
	public interface ICharacterFinder
	{
		Character GetCharacter(Vector2 position);
		IEnumerable<Character> GetCharactersAround(Vector2 center, int width, int height, int rotation);
		IEnumerable<Character> GetCharactersOnLine(Vector2 origin, Vector2 end);
	}
}
