using UnityEngine;

namespace Overmind.Tactics.UnityClient.UserInterface
{
	public class MainMenuView : MonoBehaviour
	{
		[SerializeField]
		private GameObject activeMenu;

		public void Navigate(GameObject destinationMenu)
		{
			Debug.LogFormat(this, "[MainMenuView] Navigating from {0} to {1}", activeMenu?.name, destinationMenu?.name);
			activeMenu?.SetActive(false);
			activeMenu = destinationMenu;
			activeMenu?.SetActive(true);
		}

		public void Exit()
		{
			Debug.LogFormat(this, "[MainMenuView] Exit");
			Application.Quit();
		}
	}
}
