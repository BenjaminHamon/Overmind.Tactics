using Overmind.Tactics.Data;
using Overmind.Tactics.Model;
using Overmind.Tactics.Model.Navigation;
using Overmind.Tactics.UnityClient.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overmind.Tactics.UnityClient
{
	public class GameView : MonoBehaviour
	{
		[SerializeField]
		private Transform playerGroup;
		[SerializeField]
		private Transform mapGroup;
		[SerializeField]
		private Transform characterGroup;

		[SerializeField]
		private GameObject playerPrefab;
		[SerializeField]
		private GameObject humanPlayerControllerPrefab;
		[SerializeField]
		private GameObject characterPrefab;

		[SerializeField]
		private LayerMask targetLayerMask;

		[SerializeField]
		private List<PlayerView> playerCollection;
		[SerializeField]
		private List<CharacterView> characterCollection;

		public GameModel Model { get; set; }
		public GameData Data;

		private void Start()
		{
			Debug.Log("[GameView] Start");

			if (Model == null)
				Model = new GameModel(Data, UnityApplication.DataProvider);
			Model.Initialize(new UnityCharacterFinder(targetLayerMask), new AstarNavigation(GameView.IsTileAccessible));
			foreach (PlayerView playerView in playerCollection)
				playerView.GetCharacterCollection = () => characterCollection.Where(c => c.Model.Owner == playerView.Model);
			
			foreach (PlayerModel player in Model.PlayerCollection)
				player.Defeated += p => Debug.LogFormat(this, "[GameView] {0} is defeated", p.Name);
			foreach (CharacterModel character in Model.CharacterCollection)
				character.Died += characterModel => characterCollection.RemoveAll(characterView => characterView.Model == characterModel);
			Model.GameEnded += OnGameEnded;

			Model.Start();
			if (Model.ActivePlayer != null)
				playerCollection.Single(p => p.Model == Model.ActivePlayer).Enable();
		}

		public void ResetScene()
		{
			playerCollection = new List<PlayerView>();
			characterCollection = new List<CharacterView>();
			mapGroup.DestroyAllChildren();
			playerGroup.DestroyAllChildren();
			characterGroup.DestroyAllChildren();
		}

		public void UpdateModelFromScene()
		{
			GameData gameData = new GameData();

			if (mapGroup.childCount > 0)
				gameData.Map = mapGroup.GetChild(0).name;

			List<PlayerView> playerViewCollection = playerGroup.GetComponentsInChildren<PlayerView>(true).ToList();
			foreach (PlayerView playerView in playerViewCollection)
			{
				if (String.IsNullOrEmpty(playerView.Data.Id))
					playerView.Data.Id = Guid.NewGuid().ToString();
				gameData.PlayerCollection.Add(playerView.Data);
			}

			List<CharacterView> characterViewCollection = characterGroup.GetComponentsInChildren<CharacterView>(true).ToList();
			foreach (CharacterView characterView in characterViewCollection)
			{
				if (String.IsNullOrEmpty(characterView.Data.Id))
					characterView.Data.Id = Guid.NewGuid().ToString();
				PlayerData ownerByName = gameData.PlayerCollection.SingleOrDefault(player => player.Name == characterView.Data.Owner);
				if (ownerByName != null)
					characterView.Data.Owner = ownerByName.Id;
				characterView.Data.Position = ((UnityEngine.Vector2)characterView.transform.localPosition).ToModelVector();
				gameData.CharacterCollection.Add(characterView.Data);
			}

			GameModel model = new GameModel(gameData, UnityApplication.DataProvider);
			foreach (PlayerView playerView in playerViewCollection)
				playerView.Model = model.PlayerCollection.Single(player => player.Id == playerView.Data.Id);
			foreach (CharacterView characterView in characterViewCollection)
				characterView.Model = model.CharacterCollection.Single(character => character.Id == characterView.Data.Id);

			this.Model = model;
			this.playerCollection = playerViewCollection;
			this.characterCollection = characterViewCollection;

			Debug.LogFormat(this, "[GameView] Updated model from scene");
		}

		public void ApplyModelToScene()
		{
			ResetScene();

			if (String.IsNullOrEmpty(Model.Map) == false)
			{
				GameObject mapPrefab = Resources.Load<GameObject>("Maps/" + Model.Map);
				GameObject mapObject = GameObjectExtensions.Instantiate(mapPrefab, mapGroup);
				mapObject.name = Model.Map;
			}

			foreach (PlayerModel playerModel in Model.PlayerCollection)
			{
				PlayerView playerView = GameObjectExtensions.Instantiate(playerPrefab, playerGroup).GetComponent<PlayerView>();
				playerView.Data = Data.PlayerCollection.Single(p => p.Id == playerModel.Id);
				playerView.Model = playerModel;
				playerView.UpdateFromModel();
				playerCollection.Add(playerView);

				HumanPlayerController playerController
					= GameObjectExtensions.Instantiate(humanPlayerControllerPrefab, playerView.transform).GetComponent<HumanPlayerController>();
				playerController.Game = this;
				playerController.Player = playerView;

				playerView.LocalController = playerController.gameObject;
				playerView.Disable();
			}

			foreach (CharacterModel characterModel in Model.CharacterCollection)
			{
				CharacterView characterView = GameObjectExtensions.Instantiate(characterPrefab, characterGroup).GetComponent<CharacterView>();
				characterView.Data = Data.CharacterCollection.Single(c => c.Id == characterModel.Id);
				characterView.Model = characterModel;
				characterView.UpdateFromModel();
				characterCollection.Add(characterView);
			}

			Debug.LogFormat(this, "[GameView] Applied model to scene");
		}

		private void OnGameEnded(GameModel model, PlayerModel winner)
		{
			if (winner == null)
				Debug.LogFormat(this, "[GameView] Game ended with no winner");
			else
				Debug.LogFormat(this, "[GameView] {0} is victorious", winner.Name);
		}


		/// <summary>Gets the exact tile position from a position in the world.</summary>
		public static UnityEngine.Vector2 GetTilePosition(UnityEngine.Vector2 position)
		{
			return new UnityEngine.Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
		}

		private static bool IsTileAccessible(Data.Vector2 position)
		{
			int accessibleLayerMask = LayerMask.GetMask("Ground");
			RaycastHit2D hit = Physics2D.Raycast(position.ToUnityVector(), UnityEngine.Vector2.zero);
			return (hit.collider != null) && (accessibleLayerMask == (accessibleLayerMask | (1 << hit.collider.gameObject.layer)));
		}
	}
}
