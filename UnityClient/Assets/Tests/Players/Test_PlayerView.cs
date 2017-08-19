using Overmind.Tactics.Data;
using Overmind.Tactics.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Tests
{
	internal class Test_PlayerView : MonoBehaviour
	{
		[SerializeField]
		private GameObject playerPrefab;
		[SerializeField]
		private GameObject characterPrefab;

		public void Start()
		{
			StartCoroutine(Run());
		}

		public IEnumerator Run()
		{
			Debug.LogFormat(this, "[Test] Starting {0}", nameof(Test_PlayerView));

			Debug.LogFormat(this, "[Test] Running {0}", nameof(Test_Select));
			yield return Test_Select();

			Debug.LogFormat(this, "[Test] Done");
		}

		private IEnumerator Test_Select()
		{
			PlayerView player = Instantiate(playerPrefab).GetComponent<PlayerView>();
			player.Model = new PlayerModel(new PlayerData() { Name = "TestPlayer" });
			List<CharacterView> characterCollection = new List<CharacterView>();
			CharacterClass characterClass = new CharacterClass() { Name = "TestCharacterClass", HealthPoints = 10, CharacterSprite = "Character_Example_8" };
			for (int i = 0; i < 3; i++)
			{
				CharacterView character = Instantiate(characterPrefab).GetComponent<CharacterView>();
				character.Model = new CharacterModel(new CharacterData() { Position = new Data.Vector2(i, 0) }, characterClass, player.Model, true);
				characterCollection.Add(character);
			}
			player.GetCharacterCollection = () => characterCollection;
			yield return new WaitForSeconds(1);

			player.Selection = characterCollection.First();
			yield return new WaitForSeconds(3);
			player.Selection = null;
			yield return new WaitForSeconds(1);

			player.SelectNext();
			yield return new WaitForSeconds(1);
			player.SelectNext();
			yield return new WaitForSeconds(1);
			player.SelectNext();
			yield return new WaitForSeconds(1);
			player.SelectNext();
			yield return new WaitForSeconds(1);
			player.SelectNext();
			yield return new WaitForSeconds(1);
			player.Selection = null;
			yield return new WaitForSeconds(1);

			player.SelectPrevious();
			yield return new WaitForSeconds(1);
			player.SelectPrevious();
			yield return new WaitForSeconds(1);
			player.SelectPrevious();
			yield return new WaitForSeconds(1);
			player.SelectPrevious();
			yield return new WaitForSeconds(1);
			player.SelectPrevious();
			yield return new WaitForSeconds(1);
			player.Selection = null;
			yield return new WaitForSeconds(1);

			foreach (CharacterView character in characterCollection)
				Destroy(character.gameObject);
			Destroy(player.gameObject);
		}
	}
}
