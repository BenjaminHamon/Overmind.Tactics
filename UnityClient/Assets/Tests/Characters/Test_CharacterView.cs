using Overmind.Tactics.Data;
using Overmind.Tactics.Model;
using Overmind.Tactics.Model.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Tests
{
	internal class Test_CharacterView : MonoBehaviour
	{
		[SerializeField]
		private GameObject characterPrefab;
		[SerializeField]
		private LayerMask targetLayerMask;

		public void Start()
		{
			StartCoroutine(Run());
		}

		public IEnumerator Run()
		{
			Debug.LogFormat(this, "[Test] Starting {0}", nameof(Test_CharacterView));

			Debug.LogFormat(this, "[Test] Running {0}", nameof(Test_Move));
			yield return Test_Move();
			Debug.LogFormat(this, "[Test] Running {0}", nameof(Test_DamageSelf));
			yield return Test_DamageSelf();
			Debug.LogFormat(this, "[Test] Running {0}", nameof(Test_Die));
			yield return Test_Die();

			Debug.LogFormat(this, "[Test] Done");
		}

		private IEnumerator Test_Move()
		{
			CharacterClass characterClass = new CharacterClass() { Name = "TestCharacterClass", HealthPoints = 10, CharacterSprite = "Character_Example_8" };
			CharacterView character = Instantiate(characterPrefab).GetComponent<CharacterView>();
			character.Model = new CharacterModel(new CharacterData(), characterClass, null, true);
			yield return new WaitForSeconds(1);

			character.Model.Move(new List<Data.Vector2>() { new Data.Vector2(1, 0), new Data.Vector2(1, 1) });
			yield return new WaitForSeconds(3);
			character.Model.Move(new List<Data.Vector2>() { Data.Vector2.Zero });
			yield return new WaitForSeconds(1);

			character.Model.Move(new List<Data.Vector2>()
			{
				new Data.Vector2(1, 0),
				new Data.Vector2(1, 1),
				new Data.Vector2(0, 1),
				new Data.Vector2(-1, 1),
				new Data.Vector2(-1, 0),
				new Data.Vector2(-1, -1),
				new Data.Vector2(0, -1),
				new Data.Vector2(1, -1),
			});
			yield return new WaitForSeconds(3);

			Destroy(character.gameObject);
			yield return new WaitForSeconds(1);
		}

		private IEnumerator Test_DamageSelf()
		{
			CharacterView character = Instantiate(characterPrefab).GetComponent<CharacterView>();
			CharacterClass characterClass = new CharacterClass() { Name = "TestCharacterClass", HealthPoints = 10, CharacterSprite = "Character_Example_8" };
			character.Model = new CharacterModel(new CharacterData(), characterClass, null, true);
			yield return new WaitForSeconds(1);
			
			IAbility ability = new AreaAbility() { Power = 2, TargetWidth = 1, TargetHeight = 1,
				TargetTypes = new List<TargetType>() { TargetType.Self } };
			ability.Cast(new UnityCharacterFinder(targetLayerMask), character.Model, character.Model.Position);
			yield return new WaitForSeconds(1);

			Destroy(character.gameObject);
			yield return new WaitForSeconds(1);
		}

		private IEnumerator Test_Die()
		{
			CharacterView character = Instantiate(characterPrefab).GetComponent<CharacterView>();
			CharacterClass characterClass = new CharacterClass() { Name = "TestCharacterClass", HealthPoints = 10, CharacterSprite = "Character_Example_8" };
			character.Model = new CharacterModel(new CharacterData(), characterClass, null, true);
			yield return new WaitForSeconds(1);

			IAbility ability = new AreaAbility() { Power = 20, TargetWidth = 1, TargetHeight = 1,
				TargetTypes = new List<TargetType>() { TargetType.Self } };
			ability.Cast(new UnityCharacterFinder(targetLayerMask), character.Model, character.Model.Position);
			yield return new WaitForSeconds(3);
		}
	}
}
