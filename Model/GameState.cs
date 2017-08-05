using Newtonsoft.Json;
using Overmind.Tactics.Model.Commands;
using Overmind.Tactics.Model.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Overmind.Tactics.Model
{
	[DataContract]
	[Serializable]
	public class GameState
	{
		[DataMember]
		public string Map;
		[DataMember]
		public List<Player> PlayerCollection = new List<Player>();
		[DataMember]
		public List<Character> CharacterCollection = new List<Character>();
		[DataMember, JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
		public List<IGameCommand> CommandHistory = new List<IGameCommand>();

		[DataMember(EmitDefaultValue = false)]
		public int Turn;
		[DataMember(Name = nameof(ActivePlayer), EmitDefaultValue = false)]
		public string ActivePlayerId;
		public Player ActivePlayer;
		public event Action<GameState> ActivePlayerChanged;

		public bool IsRunning = false;
		public event Action<GameState, Player> GameEnded;

		public ICharacterFinder CharacterFinder { get; private set; }
		public INavigation Navigation { get; private set; }

		public void Initialize(ContentProvider contentProvider, ICharacterFinder characterFinder, INavigation navigation)
		{
			this.CharacterFinder = characterFinder;
			this.Navigation = navigation;

			Dictionary<string, CharacterClass> characterClassCollection = new Dictionary<string, CharacterClass>();
			foreach (Character character in CharacterCollection)
			{
				character.Owner = PlayerCollection.Single(player => player.Id == character.OwnerId);
				character.CharacterClass = contentProvider.GetCharacterClass(character.CharacterClass_Key);

				if (Turn == 0)
					character.Initialize();
			}

			if (String.IsNullOrEmpty(ActivePlayerId) == false)
				ActivePlayer = PlayerCollection.Single(player => player.Id == ActivePlayerId);
		}

		public void Start()
		{
			foreach (Player player in PlayerCollection)
			{
				player.TurnEnded += OnPlayerTurnEnded;
				player.Defeated += _ => CheckGameEnd();
			}

			foreach (Character character in CharacterCollection)
				character.Died += OnCharacterDied;

			IsRunning = true;
			CheckGameEnd();
			if (IsRunning == false)
				return;

			if (Turn == 0)
			{
				Turn = 1;
				ActivePlayer = PlayerCollection.First();
				ActivePlayerId = ActivePlayer.Id;
				ActivePlayerChanged?.Invoke(this);
				StartTurn();
			}
		}

		private void StartTurn()
		{
			foreach (Character character in CharacterCollection.Where(c => c.Owner == ActivePlayer))
				character.StartTurn();
			ActivePlayer.StartTurn();
		}

		private void OnPlayerTurnEnded(Player player)
		{
			List<Player> alivePlayerCollection = PlayerCollection.Where(p => p.IsAlive).ToList();
			if (ActivePlayer != alivePlayerCollection.Last())
			{
				ActivePlayer = alivePlayerCollection[alivePlayerCollection.IndexOf(ActivePlayer) + 1];
			}
			else
			{
				Turn += 1;
				ActivePlayer = alivePlayerCollection.First();
			}

			ActivePlayerId = ActivePlayer.Id;
			ActivePlayerChanged?.Invoke(this);
			StartTurn();
		}

		private void OnCharacterDied(Character character)
		{
			CharacterCollection.Remove(character);
			character.Owner.CheckState(this);
		}

		private void CheckGameEnd()
		{
			List<Player> alivePlayerCollection = PlayerCollection.Where(p => p.IsAlive).ToList();
			if (alivePlayerCollection.Count <= 1)
			{
				Player winner = alivePlayerCollection.SingleOrDefault();
				IsRunning = false;
				GameEnded?.Invoke(this, winner);
			}
		}
	}
}
