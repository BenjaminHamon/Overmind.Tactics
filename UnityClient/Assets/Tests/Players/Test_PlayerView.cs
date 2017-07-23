using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Tests
{
	internal class Test_PlayerView : MonoBehaviour
	{
		[SerializeField]
		private PlayerView player;
		[SerializeField]
		private List<CharacterView> characterCollection;

		public void Start()
		{
			player.GetCharacterCollection = () => characterCollection;

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
		}
	}
}
