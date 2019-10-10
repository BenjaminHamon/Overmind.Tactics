using System;
using UnityEditor;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Editor
{
	[CustomEditor(typeof(GameSceneManager))]
	public class GameSceneManagerInspector : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			GameSceneManager manager = (GameSceneManager)target;

			DrawDefaultInspector();

			EditorGUILayout.Separator();

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup(String.IsNullOrEmpty(manager.GameScenarioPath));
			if (GUILayout.Button("Save scenario"))
				manager.SaveScenario(manager.GameScenarioPath);
			if (GUILayout.Button("Load scenario"))
				manager.LoadScenario(manager.GameScenarioPath);
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup(String.IsNullOrEmpty(manager.GameSavePath));
			if (GUILayout.Button("Save game"))
				manager.SaveGame(manager.GameSavePath);
			if (GUILayout.Button("Load game"))
				manager.LoadGame(manager.GameSavePath);
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup(String.IsNullOrEmpty(manager.GameReplayPath));
			if (GUILayout.Button("Save replay"))
				manager.SaveGame(manager.GameReplayPath);
			if (GUILayout.Button("Load replay"))
				manager.LoadGame(manager.GameReplayPath);
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}
	}
}
