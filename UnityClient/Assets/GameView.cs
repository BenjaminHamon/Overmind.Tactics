using Newtonsoft.Json;
using Overmind.Tactics.Model;
using Overmind.Tactics.Model.Navigation;
using Overmind.Tactics.UnityClient.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overmind.Tactics.UnityClient
{
	public class GameView : MonoBehaviour, ISerializationCallbackReceiver
	{
		[SerializeField]
		private Transform mapGroup;
		[SerializeField]
		private Transform characterGroup;

		[SerializeField]
		private GameObject playerPrefab;
		[SerializeField]
		private GameObject characterPrefab;

		[SerializeField]
		private GameObject humanPlayerControllerPrefab;

		[SerializeField]
		private LayerMask targetLayerMask;

		public string GameScenarioPath;
		public string GameSavePath;
		public Game Model;

		[SerializeField]
		private List<PlayerView> playerCollection;
		public PlayerView ActivePlayer;

		public List<CharacterView> CharacterCollection;
		
		public int Turn { get { return Model.ActiveState.Turn; } }

		private readonly ContentProvider contentProvider;

		public GameView()
		{
			JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
#if UNITY_EDITOR
			contentProvider = new UnityContentProvider(serializer, "Assets/Resources", "UserData");
#else
			contentProvider = new UnityContentProvider(serializer, null, Application.persistentDataPath);
#endif
		}

		public void OnBeforeSerialize() { }
		public void OnAfterDeserialize() { Model.Initialize(contentProvider); }

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

		public void Awake()
		{
			Debug.Log("[GameView] Awake");

			if (String.IsNullOrEmpty(GameSavePath) == false)
				Load(GameSavePath, false);
			else if (String.IsNullOrEmpty(GameScenarioPath) == false)
				Load(GameScenarioPath, true);
			else
				UpdateModelFromScene();
		}

		public void Start()
		{
			Debug.Log("[GameView] Start");

			foreach (PlayerView playerView in playerCollection)
				playerView.GetCharacterCollection = () => CharacterCollection.Where(c => c.Model.Owner == playerView.Model);
			if (Model.ActiveState.ActivePlayer != null)
				ActivePlayer = playerCollection.Single(p => p.Model.Id == Model.ActiveState.ActivePlayer.Id);
			
			Model.ActiveState.ActivePlayerChanged += _ => ActivePlayer = playerCollection.Single(p => p.Model.Id == Model.ActiveState.ActivePlayer.Id);
			foreach (Player player in Model.ActiveState.PlayerCollection)
				player.Defeated += p => Debug.LogFormat(this, "[GameView] {0} is defeated", p.Name);
			foreach (Character character in Model.ActiveState.CharacterCollection)
				character.Died += characterModel => CharacterCollection.RemoveAll(characterView => characterView.Model == characterModel);
			Model.ActiveState.GameEnded += OnGameEnded;

			Model.ActiveState.Start();
			if (ActivePlayer != null)
				ActivePlayer.Enable();
		}

		/// <summary>Gets the exact tile position from a position in the world.</summary>
		public static UnityEngine.Vector2 GetTilePosition(UnityEngine.Vector2 position)
		{
			return new UnityEngine.Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
		}

		// The editor maintains several game states:
		//   ActiveState, the current game state according to the underlying game model
		//   SaveState, a copy of the initial game state with a command history tracking changes to get to the active state
		//   The scene, the game state corresponding to game objects instantiated by Unity
		//
		// In game, the active state is the the state the game relies upon.
		// The save state can be used to replay commands to get to any past point in the game.
		// The scene can be modified in the editor directly and used to update the model to create saves for game scenarios.

		public void SaveScenario(string path)
		{
			Model.SaveScenario(path, Model.ActiveState);
			Debug.LogFormat(this, "[GameView] Saved to {0}", path);
		}

		public void SaveGame(string path, bool withHistory)
		{
			Model.SaveGame(path, withHistory ? Model.SaveState : Model.ActiveState);
			Debug.LogFormat(this, "[GameView] Saved to {0}", path);
		}

		public void Load(string path, bool asScenario)
		{
			if (asScenario)
				Model.LoadScenario(path);
			else
				Model.LoadGame(path);

			Debug.LogFormat(this, "[GameView] Loaded from {0}", path);

			Model.ActiveState.Initialize(contentProvider, new UnityCharacterFinder(targetLayerMask), new AstarNavigation(IsTileAccessible));
			ApplyModelToScene();
		}

		public void ResetScene()
		{
			playerCollection = new List<PlayerView>();
			CharacterCollection = new List<CharacterView>();
			mapGroup.DestroyAllChildren();
			transform.DestroyAllChildren();
			characterGroup.DestroyAllChildren();
		}

		public void UpdateModelFromScene()
		{
			GameState gameState = new GameState();

			if (mapGroup.childCount > 0)
				gameState.Map = mapGroup.GetChild(0).name;

			List<PlayerView> playerViewCollection = transform.GetComponentsInChildren<PlayerView>(true).ToList();
			gameState.PlayerCollection = playerViewCollection.Select(p => p.Model).ToList();
			
			List<CharacterView> characterViewCollection = characterGroup.GetComponentsInChildren<CharacterView>(true).ToList();
			foreach (CharacterView characterView in characterViewCollection)
			{
				// Prefer relying on the name so that you need to set the name instead of the id in the scene
				characterView.Model.Owner = gameState.PlayerCollection.Single(p => p.Name == characterView.Model.Owner.Name);
				characterView.Model.OwnerId = characterView.Model.Owner.Id;
				characterView.Model.Position = ((UnityEngine.Vector2)characterView.transform.localPosition).ToModelVector();
				characterView.Model.CharacterClass = contentProvider.GetCharacterClass(characterView.Model.CharacterClass_Key);
				gameState.CharacterCollection.Add(characterView.Model);
			}

			Model.SetState(gameState);
			this.playerCollection = playerViewCollection;
			this.CharacterCollection = characterViewCollection;
			
			Model.ActiveState.Initialize(contentProvider, new UnityCharacterFinder(targetLayerMask), new AstarNavigation(IsTileAccessible));
			foreach (PlayerView playerView in playerViewCollection)
				playerView.UpdateFromModel();
			foreach (CharacterView characterView in characterViewCollection)
				characterView.UpdateFromModel();

			Debug.LogFormat(this, "[GameView] Updated model from scene");
		}

		public void ApplyModelToScene()
		{
			ResetScene();
			
			GameObject mapPrefab = Resources.Load<GameObject>("Maps/" + Model.ActiveState.Map);
			GameObject mapObject = GameObjectExtensions.Instantiate(mapPrefab, mapGroup);
			mapObject.name = Model.ActiveState.Map;

			foreach (Player playerModel in Model.ActiveState.PlayerCollection)
			{
				PlayerView playerView = GameObjectExtensions.Instantiate(playerPrefab, transform).GetComponent<PlayerView>();
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

			foreach (Character characterModel in Model.ActiveState.CharacterCollection)
			{
				CharacterView characterView = GameObjectExtensions.Instantiate(characterPrefab, characterGroup).GetComponent<CharacterView>();
				characterView.Model = characterModel;
				characterView.UpdateFromModel();
				CharacterCollection.Add(characterView);
			}

			Debug.LogFormat(this, "[GameView] Applied model to scene");
		}

		private void OnGameEnded(GameState state, Player winner)
		{
			if (winner == null)
				Debug.LogFormat(this, "[GameView] Game ended with no winner");
			else
				Debug.LogFormat(this, "[GameView] {0} is victorious", winner.Name);
		}

		private bool IsTileAccessible(Model.Vector2 position)
		{
			int accessibleLayerMask = LayerMask.GetMask("Ground");
			RaycastHit2D hit = Physics2D.Raycast(position.ToUnityVector(), UnityEngine.Vector2.zero);
			return (hit.collider != null) && (accessibleLayerMask == (accessibleLayerMask | (1 << hit.collider.gameObject.layer)));
		}
	}
}
