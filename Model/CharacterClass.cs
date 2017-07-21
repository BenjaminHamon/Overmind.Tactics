using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Overmind.Tactics.Model
{
	[DataContract]
	[Serializable]
	public class CharacterClass
	{
		[DataMember]
		public string Name;
		[DataMember]
		public string CharacterSprite;

		[DataMember]
		public int HealthPoints;
		[DataMember]
		public int ActionPoints;
		[DataMember]
		public int MoveSpeed;

		[DataMember(Name = nameof(DefaultAbility))]
		public string DefaultAbilityName;
		public Ability DefaultAbility { get { return Abilities.SingleOrDefault(a => a.Name == DefaultAbilityName); } }
		[DataMember]
		public List<Ability> Abilities;
	}
}
