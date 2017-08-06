﻿using Overmind.Tactics.Model;
using Overmind.Tactics.Model.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Tests
{
	internal class Test_CharacterView : MonoBehaviour
	{
		[SerializeField]
		private CharacterView character;
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
			character.Model.Move(new List<Model.Vector2>() { new Model.Vector2(1, 0), new Model.Vector2(1, 1) });
			yield return new WaitForSeconds(3);
			character.Model.Move(new List<Model.Vector2>() { Model.Vector2.Zero });
			yield return new WaitForSeconds(1);

			character.Model.Move(new List<Model.Vector2>()
			{
				new Model.Vector2(1, 0),
				new Model.Vector2(1, 1),
				new Model.Vector2(0, 1),
				new Model.Vector2(-1, 1),
				new Model.Vector2(-1, 0),
				new Model.Vector2(-1, -1),
				new Model.Vector2(0, -1),
				new Model.Vector2(1, -1),
			});
			yield return new WaitForSeconds(3);
			character.Model.Move(new List<Model.Vector2>() { Model.Vector2.Zero });
			yield return new WaitForSeconds(1);
		}

		private IEnumerator Test_DamageSelf()
		{
			character.Model.CharacterClass = new CharacterClass() { HealthPoints = 10, CharacterSprite = "Character_Example_8" };
			character.Model.HealthPoints = 10;
			character.Model.Position = new Model.Vector2(1, 0);
			character.UpdateFromModel();

			GameState gameState = new GameState();
			gameState.Initialize(null, new UnityCharacterFinder(targetLayerMask), null);

			IAbility ability = new AreaAbility() { Power = 2, TargetWidth = 1, TargetHeight = 1,
				TargetTypes = new List<TargetType>() { TargetType.Self } };
			ability.Cast(gameState, character.Model, character.Model.Position);
			yield return new WaitForSeconds(3);
		}

		private IEnumerator Test_Die()
		{
			character.Model.CharacterClass = new CharacterClass() { HealthPoints = 10, CharacterSprite = "Character_Example_8" };
			character.Model.HealthPoints = 10;
			character.Model.Position = new Model.Vector2(1, 0);
			character.UpdateFromModel();

			GameState gameState = new GameState();
			gameState.Initialize(null, new UnityCharacterFinder(targetLayerMask), null);
			
			IAbility ability = new AreaAbility() { Power = 20, TargetWidth = 1, TargetHeight = 1,
				TargetTypes = new List<TargetType>() { TargetType.Self } };
			ability.Cast(gameState, character.Model, character.Model.Position);
			yield return new WaitForSeconds(3);
		}
	}
}
