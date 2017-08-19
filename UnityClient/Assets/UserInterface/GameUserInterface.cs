using Overmind.Tactics.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Overmind.Tactics.UnityClient.UserInterface
{
	public class GameUserInterface : MonoBehaviour
	{
		public PlayerView Player;
		public GameModel Game { get; set; }

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
		public GameMessageView GameMessageView;

		public void Update()
		{
			activePlayerText.text = Game?.ActivePlayer?.Name;
			turnText.text = Game != null ? "Turn " + Game.Turn : null;

			characterPanel.gameObject.SetActive(Player?.Selection != null);
			characterPanel.Character = Player?.Selection?.Model;
			AbilityPanel.gameObject.SetActive(Player?.Selection != null);
			AbilityPanel.Character = Player?.Selection?.Model;
		}
	}
}
