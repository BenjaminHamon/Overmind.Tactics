﻿using Overmind.Tactics.Model;
using Overmind.Tactics.UnityClient.Unity;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Overmind.Tactics.UnityClient.UserInterface
{
	public class AbilityPanel : MonoBehaviour
	{
		public event Action<Character, Ability> AbilityButtonClick;

		private Character character;
		public Character Character
		{
			get { return character; }
			set
			{
				if (character == value)
					return;
				character = value;
				UpdateState();
			}
		}

		[SerializeField]
		private Button MoveButton;
		[SerializeField]
		private Transform SpecialAbilityGroup;
		[SerializeField]
		private GameObject AbilityButtonPrefab;

		public void Start()
		{
			UpdateState();
		}

		private void UpdateState()
		{
			MoveButton.onClick.RemoveAllListeners();
			SpecialAbilityGroup.DestroyAllChildren();

			if (character == null)
				return;

			MoveButton.onClick.AddListener(() => AbilityButtonClick?.Invoke(character, null));

			foreach (Ability ability in character.CharacterClass.Abilities)
			{
				GameObject abilityButton = Instantiate(AbilityButtonPrefab, SpecialAbilityGroup);
				abilityButton.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Abilities/" + ability.Icon);
				abilityButton.GetComponent<Button>().onClick.AddListener(() => AbilityButtonClick?.Invoke(character, ability));
			}
		}
	}
}