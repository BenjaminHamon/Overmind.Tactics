﻿using Overmind.Tactics.Data;
using Overmind.Tactics.Model;
using Overmind.Tactics.Model.Abilities;
using Overmind.Tactics.UnityClient.UserInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Tests.UserInterface
{
	internal class Test_AbilityPanel : MonoBehaviour
	{
		[SerializeField]
		private AbilityPanel abilityPanel;

		public void Start()
		{
			StartCoroutine(Run());
		}

		public IEnumerator Run()
		{
			Debug.LogFormat(this, "[Test] Starting {0}", nameof(Test_AbilityPanel));

			Debug.LogFormat(this, "[Test] Running {0}", nameof(Test_Update));
			yield return Test_Update();

			Debug.LogFormat(this, "[Test] Done");
		}

		private IEnumerator Test_Update()
		{
			abilityPanel.Character = null;
			yield return new WaitForSeconds(1);

			CharacterClass characterClass = new CharacterClass() { Abilities = new List<IAbility>() };
			abilityPanel.Character = new CharacterModel(new CharacterData(), characterClass, null, false);
			yield return new WaitForSeconds(3);

			characterClass = new CharacterClass()
			{
				Abilities = new List<IAbility>()
				{
					new AreaAbility() { Icon = "Ability_Attack" },
					new AreaAbility() { Icon = "Ability_RangedAttack" },
				},
			};
			abilityPanel.Character = new CharacterModel(new CharacterData(), characterClass, null, false);
			yield return new WaitForSeconds(3);

			abilityPanel.Character = null;
			yield return new WaitForSeconds(1);
		}
	}
}
