using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lighting2DMaterials {
	private LightingMaterial occlusionEdge = null;
	private LightingMaterial occlusionBlur = null;

	private LightingMaterial additive = null;
	private LightingMaterial light = null;
	private LightingMaterial multiplyHDR = null;
	private LightingMaterial alphablend = null;
	private LightingMaterial lightSprite = null;

	public Lighting2DMaterial.Mask mask = new Lighting2DMaterial.Mask();
	public Lighting2DMaterial.BumpMask bumpMask = new Lighting2DMaterial.BumpMask();
	public Lighting2DMaterial.Shadow shadow = new Lighting2DMaterial.Shadow();
	public Lighting2DMaterial.Room room = new Lighting2DMaterial.Room();

	public LightingSettings.HDR hdr = LightingSettings.HDR.Half;
	private bool initialized = false;

	public bool Initialize(LightingSettings.HDR allowHDR) {
		if (initialized == true) {
			if (allowHDR == hdr) {
				return(false);
			}
		}

		hdr = allowHDR;

		Reset();

		mask.Reset();
		shadow.Reset();
		room.Reset();
		bumpMask.Reset();

		initialized = true;

		mask.Initialize();
		shadow.Initialize();
		room.Initialize();
		bumpMask.Initialize();

		GetAdditive();
		GetLight();

		GetOcclusionBlur();
		GetOcclusionEdge();

		return(true);
	}

	public void Reset() {
		initialized = false; // is it the best way?

		occlusionEdge = null;
		occlusionBlur = null;

		additive = null;
		multiplyHDR = null;
		alphablend = null;
	}
	
	public Material GetLightSprite() {
		if (lightSprite == null || lightSprite.Get() == null) {
			lightSprite = LightingMaterial.Load("Light2D/Internal/LightSprite");
		}
		return(lightSprite.Get());
	}
		
	public Material GetLight() {
		if (light == null || light.Get() == null) {
			light = LightingMaterial.Load("Light2D/Internal/Light");
		}

		if (Lighting2D.ProjectSettings.colorSpace == LightingSettings.ColorSpace.Linear) {
			light.Get().SetFloat("_LinearColor", 1);
		} else {
			light.Get().SetFloat("_ColorSpace", 0);
		}
	
		return(light.Get());
	}
	
	public Material GetAdditive() {
		if (additive == null || additive.Get() == null) {
			additive = LightingMaterial.Load("Light2D/Internal/Additive");
		}
		return(additive.Get());
	}

	public Material GetMultiplyHDR() {
		if (multiplyHDR == null || multiplyHDR.Get() == null) {
			if (hdr != LightingSettings.HDR.Off) {
				multiplyHDR = LightingMaterial.Load("Light2D/Internal/Multiply HDR");
			} else {
				multiplyHDR = LightingMaterial.Load("Light2D/Internal/Multiply");
			}
		}
		return(multiplyHDR.Get());
	}

	public Material GetAlphaBlend() {
		if (alphablend == null || alphablend.Get() == null) {
			alphablend = LightingMaterial.Load("Light2D/Internal/AlphaBlended");

			alphablend.SetTexture("textures/white");
		}
		return(alphablend.Get());
	}

	public Material GetOcclusionEdge() {
		if (occlusionEdge == null || occlusionEdge.Get() == null) {
			if (hdr != LightingSettings.HDR.Off) {
				occlusionEdge = LightingMaterial.Load("Light2D/Internal/Multiply HDR");
			} else {
				occlusionEdge = LightingMaterial.Load("Light2D/Internal/Multiply");
			}
			
			occlusionEdge.SetTexture("textures/occlusionedge");
		}
		return(occlusionEdge.Get());
	}


	public Material GetOcclusionBlur() {
		if (occlusionBlur == null || occlusionBlur.Get() == null) {
			if (hdr != LightingSettings.HDR.Off) {
				occlusionBlur = LightingMaterial.Load("Light2D/Internal/Multiply HDR");
			} else {
				occlusionBlur = LightingMaterial.Load("Light2D/Internal/Multiply");
			}
			
			occlusionBlur.SetTexture("textures/occlussionblur");
		}
		return(occlusionBlur.Get());
	}
}