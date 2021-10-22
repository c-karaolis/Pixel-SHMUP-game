using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraMaterials {
	public Material[] materials = new Material[1];

	public static void ClearMaterial(Material material) {
		material.SetTexture("_Cam1_Texture", null);
		material.SetVector("_Cam1_Rect", new Vector4(0, 0, 0, 0));
		material.SetFloat("_Cam1_Rotation", 0);

		material.SetTexture("_Cam2_Texture", null);
		material.SetVector("_Cam2_Rect", new Vector4(0, 0, 0, 0));
		material.SetFloat("_Cam2_Rotation", 0);
	}

	public static void ResetShaders() {
		Shader.SetGlobalTexture("_Cam1_Texture", null);
		Shader.SetGlobalVector("_Cam1_Rect", new Vector4(0, 0, 0, 0));
		Shader.SetGlobalFloat("_Cam1_Rotation", 0);

		Shader.SetGlobalTexture("_Cam2_Texture", null);
		Shader.SetGlobalVector("_Cam2_Rect", new Vector4(0, 0, 0, 0));
		Shader.SetGlobalFloat("_Cam2_Rotation", 0);
	}

	public static void SetShaders(int id, Camera camera, LightTexture lightTexture) {
		float ratio = (float)camera.pixelRect.width / camera.pixelRect.height;

		float x = camera.transform.position.x;
		float y = camera.transform.position.y;

		// z = size x ; w = size y
		float w = camera.orthographicSize * 2;
		float z = w * ratio;
	
		float rotation = camera.transform.eulerAngles.z * Mathf.Deg2Rad;

		Vector4 rect = new Vector4(x, y, z, w);

		switch(id) {
			case 1:
				Shader.SetGlobalTexture("_Cam1_Texture", lightTexture.renderTexture);

				Shader.SetGlobalVector("_Cam1_Rect", rect);

				Shader.SetGlobalFloat("_Cam1_Rotation", rotation);
			break;

			case 2:
			Debug.Log(lightTexture.renderTexture);
				Shader.SetGlobalTexture("_Cam2_Texture", lightTexture.renderTexture);

				Shader.SetGlobalVector("_Cam2_Rect", rect);

				Shader.SetGlobalFloat("_Cam2_Rotation", rotation);
			break;
		}
		
	}

	public static void SetMaterial(int id, Material material, Camera camera, LightTexture lightTexture) {
		float ratio = (float)camera.pixelRect.width / camera.pixelRect.height;

		float x = camera.transform.position.x;
		float y = camera.transform.position.y;

		// z = size x ; w = size y
		float w = camera.orthographicSize * 2;
		float z = w * ratio;

		float rotation = camera.transform.eulerAngles.z * Mathf.Deg2Rad;

		Vector4 rect = new Vector4(x, y, z, w);

		switch(id) {
			case 1:
				material.SetTexture("_Cam1_Texture", lightTexture.renderTexture);

				material.SetVector("_Cam1_Rect", rect);

				material.SetFloat("_Cam1_Rotation", rotation);
			break;

			case 2:
				material.SetTexture("_Cam2_Texture", lightTexture.renderTexture);

				material.SetVector("_Cam2_Rect", rect);

				material.SetFloat("_Cam2_Rotation", rotation);
			break;
		}
		
	}

	public void Add(Material material) {
		foreach(Material m in  materials) {
			if (m == material) {
				Debug.Log("Lighting Manager 2D: Failed to add material (material already added!");
				return;
			}
		}

		for(int i = 0 ; i < materials.Length; i++) {
			if (materials[i] != null) {
				continue;
			}
			materials[i] = material;

			return;
		}

		System.Array.Resize(ref materials, materials.Length + 1);

		materials[materials.Length - 1] = material;
	}

	public void Remove(Material material) {
		for(int i = 0 ; i < materials.Length; i++) {
			if (materials[i] != material) {
				continue;
			}
			materials[i] = null;

			return;
		}

		Debug.LogWarning("Lighting Manager 2D: Removing material that does not exist");
	}

	public void SetCamera(int id) {

	}

}