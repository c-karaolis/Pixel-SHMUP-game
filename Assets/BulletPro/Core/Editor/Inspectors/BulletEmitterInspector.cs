using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// This script is part of the BulletPro package for Unity.
// Author : Simon Albou <albou.simon@gmail.com>

namespace BulletPro.EditorScripts
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(BulletEmitter))]
	public class BulletEmitterInspector : Editor
	{
		SerializedProperty emitterProfile, patternOrigin, playAtStart;
		SerializedProperty useDefaultGizmoColor, gizmoColor, gizmoSize;

		public void OnEnable()
		{
			emitterProfile = serializedObject.FindProperty("emitterProfile");
			patternOrigin = serializedObject.FindProperty("patternOrigin");
			playAtStart = serializedObject.FindProperty("playAtStart");

			useDefaultGizmoColor = serializedObject.FindProperty("useDefaultGizmoColor");
			gizmoColor = serializedObject.FindProperty("gizmoColor");
			gizmoSize = serializedObject.FindProperty("gizmoSize");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Gameplay", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(emitterProfile);
			EditorGUILayout.PropertyField(patternOrigin);
			EditorGUILayout.PropertyField(playAtStart);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Gizmo", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(useDefaultGizmoColor);
			if (!useDefaultGizmoColor.boolValue)
				EditorGUILayout.PropertyField(gizmoColor);
			EditorGUILayout.PropertyField(gizmoSize);

			serializedObject.ApplyModifiedProperties();

			if (!EditorApplication.isPlaying) return;
			if (targets.Length > 1) return;

			EditorGUILayout.Space();

			BulletEmitter be = target as BulletEmitter;
			Bullet rb = be.rootBullet;
			
			EditorGUILayout.LabelField("Shortcuts for Play Mode:");
			
			EditorGUILayout.ObjectField("Current Root Bullet", rb, typeof(Bullet), true);

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Play", EditorStyles.miniButton))
				be.Play();

			if (GUILayout.Button("Pause", EditorStyles.miniButton))
				be.Pause();

			if (GUILayout.Button("Stop", EditorStyles.miniButton))
				be.Stop();

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Reset", EditorStyles.miniButton))
				be.Reinitialize();

			if (GUILayout.Button("Kill", EditorStyles.miniButton))
				be.Kill();

			EditorGUILayout.EndHorizontal();			
		}
	}
}
