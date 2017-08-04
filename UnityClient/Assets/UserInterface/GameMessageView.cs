using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Overmind.Tactics.UnityClient.UserInterface
{
	public class GameMessageView : MonoBehaviour
	{
		[SerializeField]
		private GameObject messagePrefab;

		[SerializeField]
		private int limit;
		[SerializeField]
		private float messageDuration;

		private List<Text> messageCollection = new List<Text>();

		public void AddMessage(string message)
		{
			if (messageCollection.Count >= limit)
			{
				Text oldestMessage = messageCollection.First();
				messageCollection.Remove(oldestMessage);
				Destroy(oldestMessage.gameObject);
			}

			Text messageObject = Instantiate(messagePrefab, transform).GetComponent<Text>();
			messageObject.text = message;
			messageCollection.Add(messageObject);
			StartCoroutine(UpdateMessage(messageObject));
		}

		private IEnumerator UpdateMessage(Text messageObject)
		{
			yield return new WaitForSeconds(messageDuration - 1);
			
			if (messageObject == null)
				yield break;

			Color messageColor = messageObject.color;
			Color transparent = messageColor;
			transparent.a = 0;

			for (float fadingTime = 0; fadingTime < 1; fadingTime += Time.deltaTime)
			{
				messageObject.color = Color.Lerp(messageColor, transparent, fadingTime);
				yield return new WaitForEndOfFrame();
				if (messageObject == null)
					yield break;
			}

			messageCollection.Remove(messageObject);
			Destroy(messageObject.gameObject);
		}
	}
}
