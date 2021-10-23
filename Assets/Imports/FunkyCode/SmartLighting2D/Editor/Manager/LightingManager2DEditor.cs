using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LightingManager2D))]
public class LightingManager2DEditor : Editor
{
	static string[] sceneLayer = new string[]{"Scene Layer", "Unity Layer"};
	static string[] gameLayer = new string[]{"Game Layer", "Unity Layer"};

	LightingManager2D lightingManager;

	private void OnEnable() {
		lightingManager = target as LightingManager2D;
	}

	override public void OnInspectorGUI() {
		DrawProfile();
		
		EditorGUILayout.Space();

		ResizeCameras(lightingManager.cameras);

		EditorGUILayout.Space();

		DrawCameras(lightingManager, lightingManager.cameras);

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("version " + Lighting2D.VERSION_STRING);
		
		string buttonName = "";
		if (lightingManager.version < Lighting2D.VERSION) {
			
			buttonName += "Re-Initialize (Outdated)";
			GUI.backgroundColor = Color.red;

			Reinitialize(lightingManager);

			return;
		} else {
			buttonName += "Re-Initialize";
		}
		
		if (GUILayout.Button(buttonName))
		{
			Lighting2DGizmoFiles.Initialize();

			Reinitialize(lightingManager);
		}

		if (GUI.changed)
		{
			Light2D.ForceUpdateAll();
			LightingManager2D.ForceUpdate();

			if (!EditorApplication.isPlaying)
			{	
				EditorUtility.SetDirty(target);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());	
			}
		}
	}

	public void DrawProfile() {
		LightingSettings.Profile newProfile = (LightingSettings.Profile)EditorGUILayout.ObjectField("Profile", lightingManager.setProfile, typeof(LightingSettings.Profile), true);
		
		if (newProfile != lightingManager.setProfile) {
			lightingManager.setProfile = newProfile;

			lightingManager.UpdateProfile();

			// LightMainBuffer2D.Clear();
			// Light2D.ForceUpdateAll();
		}
	}

	public void Reinitialize(LightingManager2D manager) {
		Debug.Log("Lighting Manager 2D: reinitialized");

		if (manager.version > 0 && manager.version < Lighting2D.VERSION) {
			Debug.Log("Lighting Manager 2D: version update from " + manager.version_string + " to " + Lighting2D.VERSION_STRING);
		}

		foreach(Transform transform in manager.transform) {
			DestroyImmediate(transform.gameObject);
		}
			
		manager.version_string = Lighting2D.VERSION_STRING;
		manager.version = Lighting2D.VERSION;

		Light2D.ForceUpdateAll();

		LightingManager2D.ForceUpdate();

		if (EditorApplication.isPlaying == false) {
			
			EditorUtility.SetDirty(target);
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			
		}
	}

	public void ResizeCameras(LightingCameras cameras) {
		int oldCount = cameras.Length;
		int newCount = EditorGUILayout.IntSlider("Camera Count", oldCount, 0, 10);
		
		if (oldCount == newCount) {
			return;
		}

		CameraSettings[] cameraSettings = cameras.cameraSettings;

		System.Array.Resize(ref cameras.cameraSettings, newCount);

		if (newCount > oldCount)
		{
			for(int i = 0; i < oldCount; i++)
			{
				cameras.cameraSettings[i] = cameraSettings[i];
			}
		}

		for(int i = 0; i < newCount; i++)
		{
			cameras.cameraSettings[i].id = i;
		}
	}

	public void DrawCameras(LightingManager2D script, LightingCameras cameras) {
		for(int id = 0; id < cameras.Length; id++) {
			CameraSettings cameraSetting = cameras.Get(id);

			script.foldout_cameras[id] = EditorGUILayout.Foldout(script.foldout_cameras[id], "Camera " + (cameraSetting.id + 1) + " (" + cameraSetting.GetTypeName() + ")");

			if (script.foldout_cameras[id] == false) {
				EditorGUILayout.Space();
				continue;
			}

			EditorGUI.indentLevel++;

			EditorGUILayout.Space();

			CameraSettings.CameraType oldType = cameraSetting.cameraType;
			CameraSettings.CameraType newType = (CameraSettings.CameraType)EditorGUILayout.EnumPopup("Camera Type", cameraSetting.cameraType);

			if (newType != oldType) {
				cameraSetting.cameraType = newType;

				cameras.Set(id, cameraSetting);
			}

			if (cameraSetting.cameraType == CameraSettings.CameraType.Custom) {
				Camera oldCamera = cameraSetting.customCamera;
				Camera newCamera = (Camera)EditorGUILayout.ObjectField(cameraSetting.customCamera, typeof(Camera), true);

				if (oldCamera != newCamera) {
					cameraSetting.customCamera = newCamera;

					cameras.Set(id, cameraSetting);
				}
			}

			EditorGUILayout.Space();

			ResizeLightmaps(cameras, id, cameraSetting);

			EditorGUILayout.Space();

			DrawLightmaps(script, cameras, id);

			EditorGUI.indentLevel--;

			EditorGUILayout.Space();
		}
	}

	public void ResizeLightmaps(LightingCameras cameras, int id, CameraSettings cameraSettings) {
		CameraLightmap[] cameraLightmaps = cameraSettings.Lightmaps;
		
		int oldCount = cameraLightmaps.Length;
		int newCount = EditorGUILayout.IntSlider("Lightmap Count", oldCount, 0, 10);

		if (oldCount == newCount) {
			return;
		}

		System.Array.Resize(ref cameraLightmaps, newCount);

		cameraSettings.Lightmaps = cameraLightmaps;

		for(int i = 0; i < newCount; i++) {
			cameraSettings.Lightmaps[i].id = i;
		}

		cameraSettings.id = id;

		cameras.Set(id, cameraSettings);
	}

	public void DrawLightmaps(LightingManager2D script, LightingCameras cameras, int id) {
		CameraSettings cameraSetting = cameras.Get(id);

		for(int index = 0; index < cameraSetting.Lightmaps.Length; index++) {
			CameraLightmap cameraLightmap = cameraSetting.Lightmaps[index];

			string presetName = Lighting2D.Profile.lightmapPresets.GetLightmapLayers()[cameraLightmap.bufferID];

			script.foldout_lightmapPresets[id, index] = EditorGUILayout.Foldout(script.foldout_lightmapPresets[id, index], "Lightmap " + (cameraLightmap.id + 1) + " (" + presetName + ")");

			if (script.foldout_lightmapPresets[id, index] == false) {
				EditorGUILayout.Space();
				continue;
			}

			EditorGUI.indentLevel++;

			EditorGUILayout.Space();

			cameraLightmap.bufferID = EditorGUILayout.Popup("Lightmap Preset", (int)cameraLightmap.bufferID, Lighting2D.Profile.lightmapPresets.GetLightmapLayers());

			EditorGUILayout.Space();

			cameraLightmap.renderMode = (CameraLightmap.RenderMode)EditorGUILayout.EnumPopup("Mode", cameraLightmap.renderMode);

			if (cameraLightmap.renderMode == CameraLightmap.RenderMode.Draw) {
				cameraLightmap.renderShader = (CameraLightmap.RenderShader)EditorGUILayout.EnumPopup("Material", cameraLightmap.renderShader);
			
				if (cameraLightmap.renderShader == CameraLightmap.RenderShader.Custom) {
					cameraLightmap.customMaterial = (Material)EditorGUILayout.ObjectField(cameraLightmap.customMaterial, typeof(Material), true);
				}
				
				if (cameraSetting.cameraType == CameraSettings.CameraType.SceneView) {
					cameraLightmap.renderLayerType = (CameraLightmap.RenderLayerType)EditorGUILayout.Popup("Type", (int)cameraLightmap.renderLayerType, sceneLayer); 
				
				} else {
					cameraLightmap.renderLayerType = (CameraLightmap.RenderLayerType)EditorGUILayout.Popup("Type", (int)cameraLightmap.renderLayerType, gameLayer); 
				
				}

				if (cameraLightmap.renderLayerType == CameraLightmap.RenderLayerType.UnityLayer) {
					cameraLightmap.renderLayerId = EditorGUILayout.LayerField("Layer", cameraLightmap.renderLayerId);
				}
			}

			EditorGUILayout.Space();

			cameraLightmap.output = (CameraLightmap.Output)EditorGUILayout.EnumPopup("Output", cameraLightmap.output);

			switch(cameraLightmap.output) {
				case CameraLightmap.Output.Materials:
							
					script.foldout_lightmapMaterials[id, index] = EditorGUILayout.Foldout(script.foldout_lightmapMaterials[id, index], "Materials");

					if (script.foldout_lightmapMaterials[id, index]) {
						EditorGUI.indentLevel++;

						CameraMaterials materials = cameraLightmap.GetMaterials();

						int matCount = materials.materials.Length;
						matCount = EditorGUILayout.IntField("Count", matCount);
						if (matCount !=  materials.materials.Length) {
							System.Array.Resize(ref materials.materials, matCount);
						}

						for(int i = 0; i < materials.materials.Length; i++) {
							materials.materials[i] = (Material)EditorGUILayout.ObjectField(materials.materials[i], typeof(Material), true);
						
						}

						EditorGUI.indentLevel--;
					}

				break;
			}

			cameraSetting.Lightmaps[index] = cameraLightmap;

			EditorGUI.indentLevel--;

			EditorGUILayout.Space();
		}

		cameras.Set(id, cameraSetting);
	}
}