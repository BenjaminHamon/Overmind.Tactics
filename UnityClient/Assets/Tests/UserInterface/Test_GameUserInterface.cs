using System.Collections;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Tests.UserInterface
{
	internal class Test_GameUserInterface : MonoBehaviour
	{
		[SerializeField]
		private PlayerView player;
		[SerializeField]
		private CharacterView character;

		public void Start()
		{
			character.Model.Owner = player.Model;
			character.UpdateFromModel();

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
			player.Selection = null;
			yield return new WaitForSeconds(3);

			player.Selection = character;
			yield return new WaitForSeconds(3);

			player.Selection = null;
			yield return new WaitForSeconds(3);
		}
	}
}
