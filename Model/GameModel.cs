using Overmind.Tactics.Data;
using Overmind.Tactics.Model.Commands;
using Overmind.Tactics.Model.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overmind.Tactics.Model
{
	public class GameModel
	{
		public GameModel(GameData gameData, GameDataProvider dataProvider)
		{
			this.dataProvider = dataProvider;

			ActiveData = gameData;
			SaveData = dataProvider.Copy(gameData);

			PlayerCollection = new List<PlayerModel>(gameData.PlayerCollection.Select(playerData => new PlayerModel(playerData)));
			CharacterCollection = new List<CharacterModel>(gameData.CharacterCollection
				.Select(characterData => new CharacterModel(characterData, dataProvider.GetCharacterClass(characterData.CharacterClass),
					PlayerCollection.Single(player => player.Id == characterData.Owner), ActiveData.Turn == 0)));

			if (String.IsNullOrEmpty(ActiveData.ActivePlayer) == false)
				activePlayerField = PlayerCollection.Single(player => player.Id == ActiveData.ActivePlayer);
		}

		private readonly GameDataProvider dataProvider;
		public ICharacterFinder CharacterFinder { get; private set; }
		public INavigation Navigation { get; private set; }

		public GameData ActiveData { get; }
		public GameData SaveData { get; }
		
		public string Map { get { return ActiveData.Map; } }
		public List<PlayerModel> PlayerCollection { get; } = new List<PlayerModel>();
		public List<CharacterModel> CharacterCollection { get; } = new List<CharacterModel>();
		
		public int Turn {  get { return ActiveData.Turn; } }
		private PlayerModel activePlayerField;
		public PlayerModel ActivePlayer
		{
			get { return activePlayerField; }
			private set
			{
				activePlayerField = value;
				ActiveData.ActivePlayer = value?.Id;
				ActivePlayerChanged?.Invoke(this);
			}
		}
		public event Action<GameModel> ActivePlayerChanged;

		private bool isRunning = false;
		public event Action<GameModel, PlayerModel> GameEnded;

		public void Initialize(ICharacterFinder characterFinder, INavigation navigation)
		{
			this.CharacterFinder = characterFinder;
			this.Navigation = navigation;
		}

		public void ExecuteCommand(IGameCommand command)
		{
			if (isRunning == false)
				return;

			if (command.TryExecute(this))
				SaveData.CommandHistory.Add(command);
		}

		public void Start()
		{
			isRunning = true;
			foreach (PlayerModel player in PlayerCollection)
				player.CheckState(this);
			CheckGameEnd();
			if (isRunning == false)
				return;

			foreach (PlayerModel player in PlayerCollection)
			{
				player.TurnEnded += OnPlayerTurnEnded;
				player.Defeated += _ => CheckGameEnd();
			}

			foreach (CharacterModel character in CharacterCollection)
				character.Died += OnCharacterDied;

			if (ActiveData.Turn == 0)
			{
				ActiveData.Turn = 1;
				ActivePlayer = PlayerCollection.First();
				StartTurn();
			}
		}

		private void StartTurn()
		{
			foreach (CharacterModel character in CharacterCollection.Where(c => c.Owner == ActivePlayer))
				character.StartTurn();
			ActivePlayer.StartTurn();
		}

		private void OnPlayerTurnEnded(PlayerModel player)
		{
			List<PlayerModel> alivePlayerCollection = PlayerCollection.Where(p => p.IsAlive).ToList();
			if (ActivePlayer != alivePlayerCollection.Last())
			{
				ActivePlayer = alivePlayerCollection[alivePlayerCollection.IndexOf(ActivePlayer) + 1];
			}
			else
			{
				ActiveData.Turn += 1;
				ActivePlayer = alivePlayerCollection.First();
			}
			
			StartTurn();
		}

		private void OnCharacterDied(CharacterModel character)
		{
			CharacterCollection.Remove(character);
			character.Owner.CheckState(this);
		}

		private void CheckGameEnd()
		{
			List<PlayerModel> alivePlayerCollection = PlayerCollection.Where(p => p.IsAlive).ToList();
			if (alivePlayerCollection.Count <= 1)
			{
				PlayerModel winner = alivePlayerCollection.SingleOrDefault();
				isRunning = false;
				GameEnded?.Invoke(this, winner);
			}
		}
	}
}
