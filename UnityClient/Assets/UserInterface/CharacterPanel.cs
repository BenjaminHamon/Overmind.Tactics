using Overmind.Tactics.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Overmind.Tactics.UnityClient.UserInterface
{
	public class CharacterPanel : MonoBehaviour
	{
		[NonSerialized]
		public Character Character;

		[SerializeField]
		private Text NameText;
		[SerializeField]
		private Text HealthPoints;
		[SerializeField]
		private Text ActionPoints;

		public void Update()
		{
			if (Character == null)
			{
				NameText.text = null;
				HealthPoints.text = null;
				ActionPoints.text = null;
			}
			else
			{
				NameText.text = Character.CharacterClass.Name;
				HealthPoints.text = Character.HealthPoints + "/" + Character.CharacterClass.HealthPoints;
				ActionPoints.text = Character.ActionPoints + "/" + Character.CharacterClass.ActionPoints;
			}
		}
	}
}
