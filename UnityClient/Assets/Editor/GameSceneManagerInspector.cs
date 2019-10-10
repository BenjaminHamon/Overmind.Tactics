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

			//EditorGUILayout.BeginHorizontal();
			//if (GUILayout.Button("Update from scene"))
			//	manager.UpdateModelFromScene();
			//if (GUILayout.Button("Apply to scene"))
			//	manager.ApplyModelToScene();
			//if (GUILayout.Button("Reset scene"))
			//	manager.ResetScene();
			//EditorGUILayout.EndHorizontal();

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
			if (GUILayout.Button("Save"))
				manager.SaveGame(manager.GameSavePath, false);
			if (GUILayout.Button("Save with history"))
				manager.SaveGame(manager.GameSavePath, true);
			if (GUILayout.Button("Load"))
				manager.LoadGame(manager.GameSavePath);
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}
	}
}
