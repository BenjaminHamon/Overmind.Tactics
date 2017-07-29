using Overmind.Tactics.Model;
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
				EditorGUI.BeginDisabledGroup(String.IsNullOrEmpty(gameView.GameSavePath));

				if (GUILayout.Button("Save"))
					Save(gameView, false);
				if (GUILayout.Button("Save with history"))
					Save(gameView, true);
				if (GUILayout.Button("Load"))
					gameView.Load(gameView.GameSavePath);

				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();
			}
		}

		private void Save(GameView gameView, bool withHistory)
		{
			if (Application.isPlaying == false)
			{
				gameView.InitializeSerialization(gameView.Model);

				foreach (Character character in gameView.Model.ActiveState.CharacterCollection)
				{
					character.HealthPoints = 0;
					character.ActionPoints = 0;
				}
			}

			gameView.Save(gameView.GameSavePath, withHistory);
		}
	}
}
