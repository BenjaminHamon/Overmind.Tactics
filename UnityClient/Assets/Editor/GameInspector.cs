using System;
using UnityEditor;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Editor
{
	[CustomEditor(typeof(GameView))]
	public class GameInspector : UnityEditor.Editor
	{
		private static bool isModelGroupVisible = true;

		public override void OnInspectorGUI()
		{
			GameView gameView = (GameView)target;

			DrawDefaultInspector();

			EditorGUILayout.Separator();
			isModelGroupVisible = EditorGUILayout.Foldout(isModelGroupVisible, "Model", true);
			
			if (isModelGroupVisible)
			{
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Update from scene"))
					gameView.UpdateModelFromScene();
				if (GUILayout.Button("Apply to scene"))
					gameView.ApplyModelToScene();
				if (GUILayout.Button("Reset scene"))
					gameView.ResetScene();
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginDisabledGroup(String.IsNullOrEmpty(gameView.GameScenarioPath));
				if (GUILayout.Button("Save scenario"))
					gameView.SaveScenario(gameView.GameScenarioPath);
				if (GUILayout.Button("Load scenario"))
					gameView.Load(gameView.GameScenarioPath, true);
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginDisabledGroup(String.IsNullOrEmpty(gameView.GameSavePath));
				if (GUILayout.Button("Save"))
					gameView.SaveGame(gameView.GameScenarioPath, false);
				if (GUILayout.Button("Save with history"))
					gameView.SaveGame(gameView.GameScenarioPath, true);
				if (GUILayout.Button("Load"))
					gameView.Load(gameView.GameSavePath, false);
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}
