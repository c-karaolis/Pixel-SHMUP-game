using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

#if UNITY_EDITOR
	using UnityEditor;
#endif

[ExecuteInEditMode] 
public class LightingManager2D : LightingMonoBehaviour {
	private static LightingManager2D instance;

	[SerializeField]
	public LightingCameras cameras = new LightingCameras();

	public bool debug = false;
	public int version = 0;
	public string version_string = "";

	public LightingSettings.Profile setProfile;
    public LightingSettings.Profile profile;

	// editor foldouts (avoid reseting after compiling script)
	public bool[] foldout_cameras = new bool[10];

	public bool[,] foldout_lightmapPresets = new bool[10, 10];
	public bool[,] foldout_lightmapMaterials= new bool[10, 10];

	// Sets Lighting Main Profile Settings for Lighting2D at the start of the scene
	private static bool initialized = false; 

	public Camera GetCamera(int id) {
		if (cameras.Length <= id) {
			return(null);
		}

		return(cameras.Get(id).GetCamera());
	}

	public static void ForceUpdate() {
	}
	
	static public LightingManager2D Get() {
		if (instance != null) {
			return(instance);
		}

		foreach(LightingManager2D manager in UnityEngine.Object.FindObjectsOfType(typeof(LightingManager2D))) {
			instance = manager;
			return(instance);
		}

		// Create New Light Manager
		GameObject gameObject = new GameObject();
		gameObject.name = "Lighting Manager 2D";

		instance = gameObject.AddComponent<LightingManager2D>();

		instance.transform.position = Vector3.zero;

		instance.version = Lighting2D.VERSION;

		instance.version_string = Lighting2D.VERSION_STRING;

		return(instance);
	}

	public void Awake() {
		if (cameras == null) {
			cameras = new LightingCameras();
		}
		
		if (instance != null && instance != this) {

			switch(Lighting2D.ProjectSettings.managerInstance) {
				case LightingSettings.ManagerInstance.Static:
				case LightingSettings.ManagerInstance.DontDestroyOnLoad:
					
					Debug.LogWarning("Smart Lighting2D: Lighting Manager duplicate was found, new instance destroyed.", gameObject);

					foreach(LightingManager2D manager in UnityEngine.Object.FindObjectsOfType(typeof(LightingManager2D))) {
						if (manager != instance) {
							manager.DestroySelf();
						}
					}

					return; // Cancel Initialization

				case LightingSettings.ManagerInstance.Dynamic:
					instance = this;
					
					Debug.LogWarning("Smart Lighting2D: Lighting Manager duplicate was found, old instance destroyed.", gameObject);

					foreach(LightingManager2D manager in UnityEngine.Object.FindObjectsOfType(typeof(LightingManager2D))) {
						if (manager != instance) {
							manager.DestroySelf();
						}
					}
				break;
			}
		}

		LightingManager2D.initialized = false;
		SetupProfile();

		if (Application.isPlaying) {
			if (Lighting2D.ProjectSettings.managerInstance == LightingSettings.ManagerInstance.DontDestroyOnLoad) {
				DontDestroyOnLoad(instance.gameObject);
			}
		}
		
		Buffers.Get();
	}

	private void Update() {
		if (Lighting2D.disable) {
			return;
		}

		ForceUpdate(); // For Late Update Method?

		if (profile != null) {
			if (Lighting2D.Profile != profile) {
				Lighting2D.UpdateByProfile(profile);
			}
        }
	}

	void LateUpdate() {
		if (Lighting2D.disable) {
			return;
		}

		Camera camera = Buffers.Get().GetCamera();

		UpdateInternal();
		
		if (Lighting2D.Profile.qualitySettings.updateMethod == LightingSettings.UpdateMethod.LateUpdate) {
			RenderLoop();
			
			camera.enabled = false;
		} else {
			camera.enabled = true;
		}
	}

	public void SetupProfile() {
		if (LightingManager2D.initialized) {
			return;
		}

		LightingManager2D.initialized = true;

		LightingSettings.Profile profile = Lighting2D.Profile;
		Lighting2D.UpdateByProfile(profile);

		Lighting2D.materials.Reset();
	}

	public void UpdateInternal() {
		if (Lighting2D.disable) {
			return;
		}

		foreach(CameraTransform cameraTransform in CameraTransform.list) {
			cameraTransform.Update();
		}

		SetupProfile();

		UpdateMaterials();

		UpdateMainBuffers();
	}

	public void UpdateLoop() {
		// Colliders

		if (DayLightCollider2D.List.Count > 0) {
			foreach(DayLightCollider2D dayLightCollider2D in DayLightCollider2D.List) {
				dayLightCollider2D.UpdateLoop();
			}
		}
		
		if (LightCollider2D.List.Count > 0) {
			foreach(LightCollider2D lightCollider2D in LightCollider2D.List) {
				lightCollider2D.UpdateLoop();
			}
		}

		// Lights

		if (LightSprite2D.List.Count > 0) {
			foreach(LightSprite2D lightSprite2D in LightSprite2D.List) {
				lightSprite2D.UpdateLoop();
			}
		}
		
		if (Light2D.List.Count > 0) {
			foreach(Light2D light2D in Light2D.List) {
				light2D.UpdateLoop();
			}
		}

		// Mesh Renderers

		if (OnRenderMode.List.Count > 0) {
			foreach(OnRenderMode render in OnRenderMode.List) {
				render.UpdateLoop();
			}
		}
	}

	public void RenderLoop() {
		if (Lighting2D.disable) {
			return;
		}

		if (cameras.Length < 1) {
			return;
		}

		UpdateLoop();
		
		if (LightBuffer2D.List.Count > 0) {
			foreach(LightBuffer2D buffer in LightBuffer2D.List) {
				buffer.Render();
			}
		}
		
		if (LightMainBuffer2D.List.Count > 0) {
			foreach(LightMainBuffer2D buffer in LightMainBuffer2D.List) {
				buffer.Render();
			}
		}
	}

	public void UpdateMainBuffers()
	{
		// should reset materials
		CameraMaterials.ResetShaders();
	
		for(int i = 0; i < cameras.Length; i++) {
			CameraSettings cameraSetting = cameras.Get(i);

			for(int b = 0; b < cameraSetting.Lightmaps.Length; b++) {
				CameraLightmap bufferPreset = cameraSetting.GetLightmap(b);

				//if (bufferPreset.renderMode == CameraBufferPreset.RenderMode.Disabled) {
				//	continue;
				//}
						
				LightMainBuffer2D buffer = LightMainBuffer2D.Get(cameraSetting, bufferPreset);

				if (buffer != null) {
					
					buffer.cameraLightmap.renderMode = bufferPreset.renderMode;

					buffer.cameraLightmap.renderLayerId = bufferPreset.renderLayerId;

					if (buffer.cameraLightmap.customMaterial != bufferPreset.customMaterial) {
						buffer.cameraLightmap.customMaterial = bufferPreset.customMaterial;

						buffer.ClearMaterial();
					}

					if (buffer.cameraLightmap.renderShader != bufferPreset.renderShader) {
						buffer.cameraLightmap.renderShader = bufferPreset.renderShader;

						buffer.ClearMaterial();
					}

					Camera camera = cameraSetting.GetCamera();

					switch(bufferPreset.output) {
						case CameraLightmap.Output.Materials:

							foreach(Material material in bufferPreset.GetMaterials().materials) {
								if (material == null) {
									continue;
								}

								
								if (cameraSetting.cameraType == CameraSettings.CameraType.SceneView) {
									CameraMaterials.SetMaterial(2, material, camera, buffer.renderTexture);
								} else {
									CameraMaterials.SetMaterial(1, material, camera, buffer.renderTexture);
								}
								
							}

						break;

						case CameraLightmap.Output.Shaders:

							if (cameraSetting.cameraType == CameraSettings.CameraType.SceneView) {
								CameraMaterials.SetShaders(2, camera, buffer.renderTexture);
							} else {
								CameraMaterials.SetShaders(1, camera, buffer.renderTexture);
							}

						break;
					}
				}
			}
		}

		// Update Main Buffers

		if (LightMainBuffer2D.List.Count > 0) {
			
			for(int i = 0; i < LightMainBuffer2D.List.Count; i++) {
				LightMainBuffer2D buffer = LightMainBuffer2D.List[i];

				if (buffer != null) {
					buffer.Update();
				}
			}

			foreach(LightMainBuffer2D buffer in LightMainBuffer2D.List) {
				if (Lighting2D.disable) {
					buffer.updateNeeded = false;	
					return;
				}

				CameraSettings cameraSettings = buffer.cameraSettings;
				CameraLightmap cameraBufferPreset = buffer.cameraLightmap;
				
				bool render = cameraBufferPreset.renderMode != CameraLightmap.RenderMode.Disabled;

				if (render && cameraSettings.GetCamera() != null) {
					buffer.updateNeeded = true;
				
				} else {
					buffer.updateNeeded = false;
				}
			}
		}
	}
	
	public void UpdateMaterials() {
		if (Lighting2D.materials.Initialize(Lighting2D.QualitySettings.HDR)) {
			LightMainBuffer2D.Clear();
			LightBuffer2D.Clear();

			Light2D.ForceUpdateAll();
		}
	}

	public bool IsSceneView() {
		for(int i = 0; i < cameras.Length; i++) {
			CameraSettings cameraSetting = cameras.Get(i);

			if (cameraSetting.cameraType == CameraSettings.CameraType.SceneView) {
				for(int b = 0; b < cameraSetting.Lightmaps.Length; b++) {
					CameraLightmap bufferPreset = cameraSetting.GetLightmap(b);
			
					if (bufferPreset.renderMode == CameraLightmap.RenderMode.Draw) {
						return(true);
					}
				}
			}
		}
		
		return(false);
	}

	private void OnDisable() {
		if (profile != null) {
			if (Application.isPlaying) {
				if (setProfile != profile) {
					if (Lighting2D.Profile == profile) {
						Lighting2D.RemoveProfile();
					}
				}
			}
		}

		#if UNITY_EDITOR
			#if UNITY_2019_1_OR_NEWER
				SceneView.beforeSceneGui -= OnSceneView;
				//SceneView.duringSceneGui -= OnSceneView;
			#else
				SceneView.onSceneGUIDelegate -= OnSceneView;
			#endif
		#endif
	}

	public void UpdateProfile() {
		if (setProfile == null) {
            setProfile = Lighting2D.ProjectSettings.Profile;
        } 

		if (Application.isPlaying == true) {
			profile = UnityEngine.Object.Instantiate(setProfile);
		} else {
			profile = setProfile;
		}
	}

	private void OnEnable() {
		foreach(OnRenderMode onRenderMode in UnityEngine.Object.FindObjectsOfType(typeof(OnRenderMode))) {
			onRenderMode.DestroySelf();
		}

		Scriptable.LightSprite2D.List.Clear();

		UpdateProfile();
		UpdateMaterials();

		for(int i = 0; i < cameras.Length; i++) {
			CameraSettings cameraSetting = cameras.Get(i);

			for(int b = 0; b < cameraSetting.Lightmaps.Length; b++) {
				CameraLightmap bufferPreset = cameraSetting.GetLightmap(b);
		
				foreach(Material material in bufferPreset.GetMaterials().materials) {
					if (material == null) {
						continue;
					}

					Camera camera = cameraSetting.GetCamera();
					if (cameraSetting.cameraType == CameraSettings.CameraType.SceneView) {
						CameraMaterials.ClearMaterial(material);
					}
					
				}
			}
		}
	
		Update();
		LateUpdate();
	
		#if UNITY_EDITOR
			#if UNITY_2019_1_OR_NEWER
				SceneView.beforeSceneGui += OnSceneView;
				//SceneView.duringSceneGui += OnSceneView;
			#else
				SceneView.onSceneGUIDelegate += OnSceneView;
			#endif	
		#endif	
	}

	public void OnRenderObject() {
		if (Lighting2D.RenderingMode != RenderingMode.OnPostRender) {
			return;
		}
		
		foreach(LightMainBuffer2D buffer in LightMainBuffer2D.List) {
			Rendering.LightMainBuffer.DrawPost(buffer);
		}
	}

	private void OnDrawGizmos() {

		if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Always) {
			return;
		}

		DrawGizmos();
	}
	
	private void DrawGizmos() {
	
		if (isActiveAndEnabled == false) {
			return;
		}

		Gizmos.color = new Color(0, 1f, 1f);

		if (Lighting2D.ProjectSettings.editorView.drawGizmosBounds == EditorGizmosBounds.Rectangle) {
			for(int i = 0; i < cameras.Length; i++) {
				CameraSettings cameraSetting = cameras.Get(i);

				Camera camera = cameraSetting.GetCamera();

				if (camera != null) {
					Rect cameraRect = CameraTransform.GetWorldRect(camera);

					GizmosHelper.DrawRect(transform.position, cameraRect);
				}
			}
		}

		for(int i = 0; i < Scriptable.LightSprite2D.List.Count; i++) {
			Scriptable.LightSprite2D light = Scriptable.LightSprite2D.List[i];

			Rect rect = light.lightSpriteShape.GetWorldRect();

			Gizmos.color = new Color(1f, 0.5f, 0.25f);

			GizmosHelper.DrawPolygon(light.lightSpriteShape.GetSpriteWorldPolygon(), transform.position);

			Gizmos.color = new Color(0, 1f, 1f);
			GizmosHelper.DrawRect(transform.position, rect);
		}
	}

	#if UNITY_EDITOR
		static public void OnSceneView(SceneView sceneView) {
			LightingManager2D manager = LightingManager2D.Get();
	
			if (manager.IsSceneView() == false) {
				return;
			}

			ForceUpdate();

			manager.UpdateLoop();
			manager.RenderLoop();

			Buffers lightingCamera = Buffers.Get();;

			if (lightingCamera != null) {
				lightingCamera.enabled = false;
				lightingCamera.enabled = true;
			}
		}
	#endif
}