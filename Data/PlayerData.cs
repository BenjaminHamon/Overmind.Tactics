using System;
using System.Runtime.Serialization;

namespace Overmind.Tactics.Data
{
	[DataContract]
	[Serializable]
	public class PlayerData
	{
		[DataMember]
		public string Id = Guid.NewGuid().ToString();
		[DataMember]
		public string Name;
		[DataMember]
		public bool IsAlive = true;
	}
}
