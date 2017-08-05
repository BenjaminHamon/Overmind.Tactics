using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overmind.Tactics.Model
{
	public interface ICharacterFinder
	{
		Character GetCharacter(Vector2 position);
		IEnumerable<Character> GetCharactersAround(Vector2 center, int width, int height, int rotation);
	}
}
