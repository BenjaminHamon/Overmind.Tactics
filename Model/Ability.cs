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

		/// <summary>Gets the rotation angle for the target area, in degrees.</summary>
		public int GetRotation(Vector2 casterPosition, Vector2 targetPosition)
		{
			Vector2 targetRelativeToCaster = targetPosition - casterPosition;
			return Math.Abs(targetRelativeToCaster.Y) < Math.Abs(targetRelativeToCaster.X) ? 90 : 0;
		}
	}
}
