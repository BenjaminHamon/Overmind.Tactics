using Overmind.Tactics.Model;
using Overmind.Tactics.UnityClient.UserInterface;
using System.Collections;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Tests.UserInterface
{
	internal class Test_CharacterPanel : MonoBehaviour
	{
		[SerializeField]
		private CharacterPanel characterPanel;

		public void Start()
		{
			StartCoroutine(Run());
		}

		public IEnumerator Run()
		{
			Debug.LogFormat(this, "[Test] Starting {0}", nameof(Test_CharacterPanel));

			Debug.LogFormat(this, "[Test] Running {0}", nameof(Test_Update));
			yield return Test_Update();

			Debug.LogFormat(this, "[Test] Done");
		}

		private IEnumerator Test_Update()
		{
			characterPanel.Character = null;
			yield return new WaitForSeconds(1);

			characterPanel.Character = new Character()
			{
				Owner = new Player() { Name = "TestPlayer" },
				CharacterClass = new CharacterClass()
				{
					Name = "TestCharacterClass",
					CharacterSprite = "Character_Example_8",
					HealthPoints = 10,
					ActionPoints = 5,
				},
				HealthPoints = 10,
				ActionPoints = 5,
			};
			yield return new WaitForSeconds(3);

			characterPanel.Character.ActionPoints = 2;
			yield return new WaitForSeconds(1);
			characterPanel.Character.HealthPoints = 7;
			yield return new WaitForSeconds(1);

			characterPanel.Character = null;
			yield return new WaitForSeconds(1);
		}
	}
}
