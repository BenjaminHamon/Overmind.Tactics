using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Overmind.Tactics.Data
{
	[DataContract]
	[Serializable]
	public class GameData
	{
		[DataMember]
		public string Map;

		[DataMember]
		public List<PlayerData> PlayerCollection = new List<PlayerData>();

		[DataMember]
		public List<CharacterData> CharacterCollection = new List<CharacterData>();

		[DataMember(EmitDefaultValue = false)]
		public int Turn;

		[DataMember(EmitDefaultValue = false)]
		public string ActivePlayer;

		[DataMember]
		[JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
		public List<IGameCommandData> CommandHistory = new List<IGameCommandData>();
	}
}
