using Overmind.Tactics.UnityClient.Unity;
using System;
using System.Collections;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.UserInterface
{
	public class ListView : MonoBehaviour
	{
		[SerializeField]
		private Transform listTransform;
		[SerializeField]
		private GameObject itemPrefab;

		[SerializeField]
		private Color defaultColor;
		[SerializeField]
		private Color selectionColor;

		public Func<IEnumerable> ItemsSource { get; set; }
		public Func<object, string> ItemTextConverter { get; set; }

		private GameObject selectedItemField;
		public GameObject SelectedItem
		{
			get { return selectedItemField; }
			set
			{
				if (selectedItemField != null)
					selectedItemField.GetComponent<ListViewItem>().Background.color = defaultColor;

				selectedItemField = value;

				if (selectedItemField != null)
					selectedItemField.GetComponent<ListViewItem>().Background.color = selectionColor;
			}
		}

		public object SelectedValue { get { return SelectedItem?.GetComponent<ListViewItem>().Value; } }

		public void UpdateItems()
		{
			TransformExtensions.DestroyAllChildren(listTransform);

			if (ItemsSource != null)
			{
				foreach (object value in ItemsSource())
				{
					GameObject newItem = GameObjectExtensions.Instantiate(itemPrefab, listTransform);
					newItem.GetComponent<ListViewItem>().Value = value;
					newItem.GetComponent<ListViewItem>().Text.text = ItemTextConverter != null ? ItemTextConverter(value) : value?.ToString();
					newItem.GetComponent<ListViewItem>().Button.onClick.AddListener(() => SelectedItem = newItem);
				}
			}
		}
	}
}
