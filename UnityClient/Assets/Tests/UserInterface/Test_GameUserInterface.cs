using Overmind.Tactics.Data;
using Overmind.Tactics.Model;
using Overmind.Tactics.UnityClient.UserInterface;
using System.Collections;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Tests.UserInterface
{
	internal class Test_GameUserInterface : MonoBehaviour
	{
		[SerializeField]
		private GameObject playerPrefab;
		[SerializeField]
		private GameObject characterPrefab;
		[SerializeField]
		private GameUserInterface userInterface;

		public void Start()
		{
			StartCoroutine(Run());
		}

		public IEnumerator Run()
		{
			Debug.LogFormat(this, "[Test] Starting {0}", nameof(Test_GameUserInterface));

			Debug.LogFormat(this, "[Test] Running {0}", nameof(Test_Update));
			yield return Test_Update();

			Debug.LogFormat(this, "[Test] Done");
		}

		private IEnumerator Test_Update()
		{
			PlayerView player = Instantiate(playerPrefab).GetComponent<PlayerView>();
			player.Model = new PlayerModel(new PlayerData() { Name = "TestPlayer" });
			CharacterClass characterClass = new CharacterClass() { Name = "TestCharacterClass", HealthPoints = 10, CharacterSprite = "Character_Example_8" };
			CharacterView character = Instantiate(characterPrefab).GetComponent<CharacterView>();
			character.Model = new CharacterModel(new CharacterData(), characterClass, player.Model, true);
			userInterface.Player = player;
			yield return new WaitForSeconds(1);

			player.Selection = null;
			yield return new WaitForSeconds(3);

			player.Selection = character;
			yield return new WaitForSeconds(3);

			player.Selection = null;
			yield return new WaitForSeconds(3);

			Destroy(character.gameObject);
			Destroy(player.gameObject);
			yield return new WaitForSeconds(1);
		}
	}
}
