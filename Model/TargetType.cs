using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overmind.Tactics.Model
{
	[JsonConverter(typeof(StringEnumConverter))]
	[Serializable]
	public enum TargetType
	{
		Self,
		Allied,
		Enemy,
	}

	public static class TargetTypeExtensions
	{
		public static bool IsTargetAllowed(IEnumerable<TargetType> targetTypes, CharacterModel caster, CharacterModel target)
		{
			if (targetTypes.Any() == false)
				return false;
			return targetTypes.All(type => IsTargetAllowed(type, caster, target));
		}

		private static bool IsTargetAllowed(TargetType type, CharacterModel caster, CharacterModel target)
		{
			switch (type)
			{
				case TargetType.Self: return caster == target;
				case TargetType.Allied: return caster.Owner == target.Owner;
				case TargetType.Enemy: return caster.Owner != target.Owner;
				default: throw new ArgumentException("[TargetType] Unhandled value: " + type);
			}
		}
	}
}
