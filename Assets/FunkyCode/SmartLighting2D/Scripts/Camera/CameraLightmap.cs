using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CameraLightmap {
	public enum RenderMode { Draw, Hidden, Disabled }
	public enum RenderShader { Multiply, Additive, Custom };
	public enum RenderLayerType { LightingLayer, UnityLayer };

	public enum Output {None, Shaders, Materials}

	public Output output;


	public RenderLayerType renderLayerType;
	public RenderMode renderMode;
	public RenderShader renderShader;
	public Material customMaterial;

	public Material customMaterialInstance;

	public CameraMaterials materials;

	public int renderLayerId;

	public int id;

	public int bufferID;

	public CameraLightmap(int id = 0) {
		this.id = id;

		this.bufferID = 0;

		this.renderMode = RenderMode.Draw;

		this.renderShader = RenderShader.Multiply;

		this.renderLayerType = RenderLayerType.LightingLayer;

		this.customMaterial = null;

		this.customMaterialInstance = null;

		this.renderLayerId = 0;

		this.output = Output.None;
		
		this.materials = new CameraMaterials();
	}

	public CameraMaterials GetMaterials() {
		if (materials == null) {
			materials = new CameraMaterials();
		}

		return(materials);
	}

	public Material GetMaterial() {
		if (customMaterialInstance == null) {
			if (customMaterial != null) {
				customMaterialInstance = new Material(customMaterial);
			}
		}

		return(customMaterialInstance);
	}
}
