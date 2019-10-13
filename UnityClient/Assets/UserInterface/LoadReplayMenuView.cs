using UnityEngine;
using UnityEngine.SceneManagement;

namespace Overmind.Tactics.UnityClient.UserInterface
{
	public class LoadReplayMenuView : MonoBehaviour
	{
		[SerializeField]
		private ListView listView;

		private void Start()
		{
			listView.ItemsSource = () => UnityApplication.DataProvider.ListReplays();
			listView.UpdateItems();
		}

		public void LoadReplay()
		{
			if (listView.SelectedItem == null)
				return;

			UnityApplication.ReplayLoadRequest = (string)listView.SelectedValue;
			SceneManager.LoadScene("GameScene");
		}
	}
}
