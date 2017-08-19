using System;

namespace Overmind.Tactics.Data
{
	[Serializable]
	public class PlayerData
	{
		public string Id = Guid.NewGuid().ToString();
		public string Name;
		public bool IsAlive = true;
	}
}
