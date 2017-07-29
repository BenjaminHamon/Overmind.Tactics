using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Overmind.Tactics.Model
{
	[DataContract]
	[Serializable]
	public class Player
	{
		[DataMember]
		public string Id = Guid.NewGuid().ToString();
		[DataMember]
		public string Name;

		[DataMember]
		public bool IsAlive = true;
		public event Action<Player> Defeated;

		public event Action<Player> TurnStarted;
		public event Action<Player> TurnEnded;

		internal void StartTurn()
		{
			TurnStarted?.Invoke(this);
		}

		public void EndTurn()
		{
			TurnEnded?.Invoke(this);
		}

		public void CheckState(GameState gameState)
		{
			IEnumerable<Character> characterCollection = gameState.CharacterCollection.Where(c => c.Owner == this);
			if (characterCollection.Any() == false)
			{
				IsAlive = false;
				Defeated?.Invoke(this);
			}
		}
	}
}
