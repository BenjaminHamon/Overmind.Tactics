using Overmind.Tactics.Data;
using Overmind.Tactics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overmind.Tactics.UnityClient
{
	public class PlayerView : MonoBehaviour
	{
		[SerializeField]
		private GameObject selectionIndicator;
		public GameObject LocalController;

		public PlayerModel Model;
		public PlayerData Data;
		public string PlayerName { get { return Model.Name; } }

		public Func<IEnumerable<CharacterView>> GetCharacterCollection;

		private CharacterView selection;
		public CharacterView Selection
		{
			get { return selection; }
			set
			{
				if (selection == value)
					return;
				selection = value;

				if (selection == null)
				{
					selectionIndicator.SetActive(false);
					selectionIndicator.transform.SetParent(transform, false);
				}
				else
				{
					selectionIndicator.transform.SetParent(Selection.transform, false);
					selectionIndicator.GetComponent<SpriteRenderer>().color = Selection.Model.Owner == Model ? Color.green : Color.red;
					selectionIndicator.SetActive(true);
				}
			}
		}

		private void Start()
		{
			if (Model == null)
				Model = new PlayerModel(Data);

			name = String.Format("Player ({0})", Model.Name);

			Model.TurnStarted += _ => Enable();
			Model.TurnEnded += _ => Disable();
		}

		public void Enable()
		{
			LocalController?.SetActive(true);
			Selection = GetCharacterCollection().FirstOrDefault();
		}

		public void Disable()
		{
			Selection = null;
			LocalController?.SetActive(false);
		}
		
		public void SelectPrevious()
		{
			List<CharacterView> characterCollection = GetCharacterCollection().ToList();

			int newIndex = characterCollection.IndexOf(Selection);
			if (newIndex == -1)
				newIndex = characterCollection.Count - 1;
			else
			{
				newIndex -= 1;
				if (newIndex < 0)
					newIndex = characterCollection.Count - 1;
			}
			
			Selection = characterCollection.ElementAtOrDefault(newIndex);
		}

		public void SelectNext()
		{
			List<CharacterView> characterCollection = GetCharacterCollection().ToList();

			int newIndex = characterCollection.IndexOf(Selection);
			if (newIndex == -1)
				newIndex = 0;
			else
			{
				newIndex += 1;
				if (newIndex >= characterCollection.Count)
					newIndex = 0;
			}

			Selection = characterCollection.ElementAtOrDefault(newIndex);
		}
	}
}
