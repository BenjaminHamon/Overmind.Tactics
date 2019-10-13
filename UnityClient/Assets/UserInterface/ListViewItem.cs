using UnityEngine;
using UnityEngine.UI;

namespace Overmind.Tactics.UnityClient.UserInterface
{
	public class ListViewItem : MonoBehaviour
	{
		public Image Background;
		public Text Text;
		public Button Button;

		public object Value { get; set; }
	}
}
