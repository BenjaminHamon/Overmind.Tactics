using Overmind.Tactics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overmind.Tactics.UnityClient
{
	public class PlayerView : MonoBehaviour
	{
		public Player Model;
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

		[SerializeField]
		private GameObject selectionIndicator;

		public void UpdateFromModel()
		{
			name = String.Format("Player ({0})", Model.Name);
		}

		public void Start()
		{
			Model.TurnStarted += _ => Enable();
			Model.TurnEnded += _ => Disable();
		}

		public void Enable()
		{
			gameObject.SetActive(true);
			Selection = GetCharacterCollection().FirstOrDefault();
		}

		public void Disable()
		{
			Selection = null;
			gameObject.SetActive(false);
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
