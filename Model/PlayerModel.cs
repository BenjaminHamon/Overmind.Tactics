using Overmind.Tactics.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overmind.Tactics.Model
{
	public class PlayerModel
	{
		public PlayerModel(PlayerData data)
		{
			this.data = data;
		}

		private readonly PlayerData data;
		
		public string Id { get { return data.Id; } }
		public string Name { get { return data.Name; } }

		public bool IsAlive { get { return data.IsAlive; } }
		public event Action<PlayerModel> Defeated;

		public event Action<PlayerModel> TurnStarted;
		public event Action<PlayerModel> TurnEnded;

		internal void StartTurn()
		{
			TurnStarted?.Invoke(this);
		}

		public void EndTurn()
		{
			TurnEnded?.Invoke(this);
		}

		public void CheckState(GameModel gameState)
		{
			IEnumerable<CharacterModel> characterCollection = gameState.CharacterCollection.Where(c => c.Owner == this);
			if (characterCollection.Any() == false)
			{
				data.IsAlive = false;
				Defeated?.Invoke(this);
			}
		}
	}
}
