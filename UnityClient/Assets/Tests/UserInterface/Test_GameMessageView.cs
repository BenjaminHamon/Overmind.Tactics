using Overmind.Tactics.UnityClient.UserInterface;
using System;
using System.Collections;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Tests.UserInterface
{
	internal class Test_GameMessageView : MonoBehaviour
	{
		[SerializeField]
		private GameMessageView messageView;

		public void Start()
		{
			StartCoroutine(Run());
		}

		public IEnumerator Run()
		{
			Debug.LogFormat(this, "[Test] Starting {0}", nameof(Test_GameUserInterface));

			Debug.LogFormat(this, "[Test] Running {0}", nameof(Test_AddMessage));
			yield return Test_AddMessage();

			Debug.LogFormat(this, "[Test] Done");
		}

		private IEnumerator Test_AddMessage()
		{
			messageView.AddMessage("Test message");
			yield return new WaitForSeconds(3);

			for (int messageIndex = 0; messageIndex < 3; messageIndex++)
			{
				messageView.AddMessage(String.Format("Test message ({0})", messageIndex));
				yield return new WaitForSeconds(1);
			}
			yield return new WaitForSeconds(3);

			for (int messageIndex = 0; messageIndex < 10; messageIndex++)
			{
				messageView.AddMessage(String.Format("Test message ({0})", messageIndex));
				yield return new WaitForSeconds(1);
			}
			yield return new WaitForSeconds(3);
		}
	}
}
