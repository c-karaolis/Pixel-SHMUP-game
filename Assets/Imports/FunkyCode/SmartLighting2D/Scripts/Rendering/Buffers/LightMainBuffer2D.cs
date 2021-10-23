using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

[ExecuteInEditMode] 
public class LightMainBuffer2D {
	public string name = "Uknown";

	private LightingMaterial material = null;

	public bool updateNeeded = false;

	public LightTexture renderTexture;
	public CameraSettings cameraSettings;
	public CameraLightmap cameraLightmap;

	public static List<LightMainBuffer2D> List = new List<LightMainBuffer2D>();

	public LightMainBuffer2D() {
		List.Add(this);
	}

	public static void Clear() {
		foreach(LightMainBuffer2D buffer in new List<LightMainBuffer2D>(List)) {
			buffer.DestroySelf();
		}

		List.Clear();
	}

	public void DestroySelf() {
		if (renderTexture != null) {
			if (renderTexture.renderTexture != null) {
				if (Application.isPlaying) {
					UnityEngine.Object.Destroy (renderTexture.renderTexture);
				} else {
					UnityEngine.Object.DestroyImmediate (renderTexture.renderTexture);
				}
			}
		}

		List.Remove(this);
	}

	public bool IsActive() {
		return(List.IndexOf(this) > -1);
	}

	static public LightMainBuffer2D Get(CameraSettings cameraSettings, CameraLightmap lightmap) {
		if (cameraSettings.GetCamera() == null) {
			return(null);
		}

		foreach(LightMainBuffer2D mainBuffer in List) {
			if (mainBuffer.cameraSettings.GetCamera() == cameraSettings.GetCamera() && mainBuffer.cameraLightmap.bufferID == lightmap.bufferID) {
				return(mainBuffer);
			}
		}

		if (Lighting2D.LightmapPresets.Length <= lightmap.bufferID) {
			Debug.LogWarning("Lighting2D: Not enough buffer settings initialized");

			return(null);
		}

		Buffers.Get();

		LightMainBuffer2D buffer = new LightMainBuffer2D();
		buffer.cameraSettings = cameraSettings;
		buffer.cameraLightmap = lightmap;

		Rendering.LightMainBuffer.InitializeRenderTexture(buffer);

		return(buffer);
	}

	public LightmapPreset GetLightmapPreset() {
		if (Lighting2D.LightmapPresets.Length <= cameraLightmap.bufferID) {
			Debug.LogWarning("Lighting2D: Not enough buffer settings initialized");

			return(null);
		}

		return(Lighting2D.LightmapPresets[cameraLightmap.bufferID]);
	}

	public void ClearMaterial() {
		material = null;
	}

	public Material GetMaterial() {
		if (material == null || material.Get() == null) {
			switch(cameraLightmap.renderShader) {

				case CameraLightmap.RenderShader.Multiply:
				
					material = LightingMaterial.Load("Light2D/Internal/Multiply");
					
				break;

				case CameraLightmap.RenderShader.Additive:
					// Use Light 2D Shader?
					material = LightingMaterial.Load("Legacy Shaders/Particles/Additive");
					
				break;

				case CameraLightmap.RenderShader.Custom:

					material = LightingMaterial.Load(cameraLightmap.GetMaterial());

				break;
			}
		}

		material.SetTexture(renderTexture.renderTexture);

		return(material.Get());
	}

	public void Update() {
		Rendering.LightMainBuffer.Update(this);
	}

	public void Render() {
		if (cameraLightmap.renderMode == CameraLightmap.RenderMode.Disabled) {
			return;
		}

		if (updateNeeded) {

			Camera camera = Camera.current;
			if (camera != null) {
				// return;	
			}
		
			RenderTexture previous = RenderTexture.active;

			RenderTexture.active = renderTexture.renderTexture;

			Rendering.LightMainBuffer.Render(this);

			RenderTexture.active = previous;
		}


		Rendering.LightMainBuffer.DrawOn(this);
	}

	// Apply Render to Specified Camera (Post Render Mode)
	public void OnRenderObject() {
		if (Lighting2D.disable) {
			return;
		}
	}
}