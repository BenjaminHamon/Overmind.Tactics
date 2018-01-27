using Overmind.Tactics.UnityClient.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Overmind.Tactics.UnityClient.UserInterface
{
	public class LoadGameMenuView : MonoBehaviour
	{
		[SerializeField]
		private Transform gameListTransform;
		[SerializeField]
		private GameObject gameListItemPrefab;

		[SerializeField]
		private Color defaultColor;
		[SerializeField]
		private Color selectionColor;

		private string selectedScenario;

		private void OnEnable()
		{
			UpdateGameList();
		}

		private void UpdateGameList()
		{
			TransformExtensions.DestroyAllChildren(gameListTransform);

			foreach (string scenario in UnityApplication.DataProvider.ListScenarios())
			{
				GameObject scenarioItem = GameObjectExtensions.Instantiate(gameListItemPrefab, gameListTransform);
				scenarioItem.GetComponentInChildren<Text>().text = scenario;
				scenarioItem.GetComponent<Button>().onClick.AddListener(() => SelectScenario(scenarioItem));
			}
		}

		private void SelectScenario(GameObject scenario)
		{
			List<GameObject> scenarioItemList = gameListTransform.Cast<Transform>().Select(t => t.gameObject).ToList();
			foreach (GameObject scenarioItem in scenarioItemList)
				scenarioItem.GetComponent<Image>().color = defaultColor;

			selectedScenario = scenario.GetComponentInChildren<Text>().text;
			scenario.GetComponent<Image>().color = selectionColor;
		}

		public void LoadSelectedScenario()
		{
			if (selectedScenario == null)
				return;

			UnityApplication.ScenarioLoadRequest = selectedScenario;
			SceneManager.LoadScene("GameScene");
		}
	}
}
