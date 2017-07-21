using System;
using System.Collections.Generic;

namespace Overmind.Tactics.Model
{
	[Serializable]
	public class Ability
	{
		public string Name;
		public string Icon;

		public int Power;
		public int ActionPoints;

		public int Range;
		public int TargetWidth;
		public int TargetHeight;

		public bool TargetRequired;
		public List<TargetType> TargetTypes;
	}
}
