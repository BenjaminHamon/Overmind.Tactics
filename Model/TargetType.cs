using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

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
}
