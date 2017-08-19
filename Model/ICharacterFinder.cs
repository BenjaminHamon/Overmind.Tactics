using Overmind.Tactics.Data;
using System.Collections.Generic;

namespace Overmind.Tactics.Model
{
	public interface ICharacterFinder
	{
		CharacterModel GetCharacter(Vector2 position);
		IEnumerable<CharacterModel> GetCharactersAround(Vector2 center, int width, int height, int rotation);
		IEnumerable<CharacterModel> GetCharactersOnLine(Vector2 origin, Vector2 end);
	}
}
