using UnityEditor;
using UnityEngine;

namespace Overmind.Tactics.UnityClient.Editor
{
	[CustomEditor(typeof(GameView))]
	public class GameViewInspector : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			GameView gameView = (GameView)target;

			DrawDefaultInspector();

			EditorGUILayout.Separator();

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Update from scene"))
				gameView.UpdateModelFromScene();
			if (GUILayout.Button("Apply to scene"))
				gameView.ApplyModelToScene();
			if (GUILayout.Button("Reset scene"))
				gameView.ResetScene();
			EditorGUILayout.EndHorizontal();
		}
	}
}
