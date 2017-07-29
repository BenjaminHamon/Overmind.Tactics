using System;
using UnityEditor;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Editor
{
	[CustomEditor(typeof(GameView))]
	public class GameInspector : UnityEditor.Editor
	{
		private static bool isModelGroupVisible = true;

		public void Awake()
		{
			GameView gameView = (GameView)target;
			gameView.InitializeSerialization(gameView.Model);
		}

		public void Start()
		{
			Debug.Log("GameInspector.Start");
		}

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
					gameView.Save(gameView.GameSavePath, false);
				if (GUILayout.Button("Save with history"))
					gameView.Save(gameView.GameSavePath, true);
				if (GUILayout.Button("Load"))
					gameView.Load(gameView.GameSavePath);

				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}
