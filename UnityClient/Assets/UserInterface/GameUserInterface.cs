using Overmind.Tactics.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Overmind.Tactics.UnityClient.UserInterface
{
	public class GameUserInterface : MonoBehaviour
	{
		public PlayerView Player;
		[NonSerialized]
		public Game Game;

		[SerializeField]
		private Text activePlayerText;
		[SerializeField]
		private Text turnText;
		[SerializeField]
		public CharacterPanel characterPanel;

		public Button EndTurnButton;
		public Button SelectPreviousCharacterButton;
		public Button SelectNextCharacterButton;
		public AbilityPanel AbilityPanel;

		public void Update()
		{
			activePlayerText.text = Game?.ActiveState.ActivePlayer?.Name;
			turnText.text = Game != null ? "Turn " + Game.ActiveState.Turn : null;

			characterPanel.gameObject.SetActive(Player?.Selection != null);
			characterPanel.Character = Player?.Selection?.Model;
			AbilityPanel.gameObject.SetActive(Player?.Selection != null);
			AbilityPanel.Character = Player?.Selection?.Model;
		}
	}
}
