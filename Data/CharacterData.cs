using System;
using System.Runtime.Serialization;

namespace Overmind.Tactics.Data
{
	[DataContract]
	[Serializable]
	public class CharacterData
	{
		[DataMember]
		public string Id = Guid.NewGuid().ToString();
		[DataMember]
		public string Owner;
		[DataMember]
		public string CharacterClass;
		[DataMember]
		public Vector2 Position;
		[DataMember(EmitDefaultValue = false)]
		public int HealthPoints;
		[DataMember(EmitDefaultValue = false)]
		public int ActionPoints;
	}
}
