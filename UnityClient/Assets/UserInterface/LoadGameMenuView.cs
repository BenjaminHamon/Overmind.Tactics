﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace Overmind.Tactics.UnityClient.UserInterface
{
	public class LoadGameMenuView : MonoBehaviour
	{
		[SerializeField]
		private ListView listView;

		private void Start()
		{
			listView.ItemsSource = () => UnityApplication.DataProvider.ListGames();
			listView.UpdateItems();
		}

		public void LoadGame()
		{
			if (listView.SelectedItem == null)
				return;

			UnityApplication.GameLoadRequest = (string)listView.SelectedValue;
			SceneManager.LoadScene("GameScene");
		}
	}
}
