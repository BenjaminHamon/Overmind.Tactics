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

		public GameModel Model { get; set; }

		[SerializeField]
		private List<PlayerView> playerCollection;
		public PlayerView ActivePlayer;

		public List<CharacterView> CharacterCollection;

		// About game initialization
		// We want to handle two cases: loading from a saved state and starting from an existing scene.
		//
		// The load case follows these steps:
		//		Game.Awake
		//			Game.Load
		//				Instantiate character (Character.Awake)
		//				Character.UpdateFromModel
		//		Game.Start / Character.Start (Undefined order)
		//
		// Character.UpdateFromModel is not called for an existing scene,
		// thus the character objects must be already fully initialized in the scene.
		//
		// Game.Start starts the game logic and assumes all entities are ready.

		public void Start()
		{
			Debug.Log("[GameView] Start");

			if (Model == null)
				UpdateModelFromScene();
			else
				ApplyModelToScene();
			Model.Initialize(new UnityCharacterFinder(targetLayerMask), new AstarNavigation(GameView.IsTileAccessible));

			foreach (PlayerView playerView in playerCollection)
				playerView.GetCharacterCollection = () => CharacterCollection.Where(c => c.Model.Owner == playerView.Model);
			if (Model.ActivePlayer != null)
				ActivePlayer = playerCollection.Single(p => p.Model == Model.ActivePlayer);
			
			Model.ActivePlayerChanged += _ => ActivePlayer = playerCollection.Single(p => p.Model == Model.ActivePlayer);
			foreach (PlayerModel player in Model.PlayerCollection)
				player.Defeated += p => Debug.LogFormat(this, "[GameView] {0} is defeated", p.Name);
			foreach (CharacterModel character in Model.CharacterCollection)
				character.Died += characterModel => CharacterCollection.RemoveAll(characterView => characterView.Model == characterModel);
			Model.GameEnded += OnGameEnded;

			Model.Start();
			if (ActivePlayer != null)
				ActivePlayer.Enable();
		}

		// The editor maintains several game states:
		//   ActiveState, the current game state according to the underlying game model
		//   SaveState, a copy of the initial game state with a command history tracking changes to get to the active state
		//   The scene, the game state corresponding to game objects instantiated by Unity
		//
		// In game, the active state is the the state the game relies upon.
		// The save state can be used to replay commands to get to any past point in the game.
		// The scene can be modified in the editor directly and used to update the model to create saves for game scenarios.

		public void ResetScene()
		{
			playerCollection = new List<PlayerView>();
			CharacterCollection = new List<CharacterView>();
			mapGroup.DestroyAllChildren();
			playerGroup.DestroyAllChildren();
			characterGroup.DestroyAllChildren();
		}

		public void UpdateModelFromScene()
		{
			throw new NotImplementedException();
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
				characterView.Model = characterModel;
				characterView.UpdateFromModel();
				CharacterCollection.Add(characterView);
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
