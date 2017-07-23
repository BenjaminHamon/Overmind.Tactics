using Overmind.Tactics.Model;
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

			abilityPanel.Character = new Character()
			{
				CharacterClass = new CharacterClass()
				{
					Abilities = new List<Ability>(),
				},
			};
			yield return new WaitForSeconds(3);

			abilityPanel.Character = new Character()
			{
				CharacterClass = new CharacterClass()
				{
					Abilities = new List<Ability>()
					{
						new Ability() { Icon = "Ability_Attack" },
						new Ability() { Icon = "Ability_RangedAttack" },
					},
				},
			};
			yield return new WaitForSeconds(3);

			abilityPanel.Character = null;
			yield return new WaitForSeconds(1);
		}
	}
}
