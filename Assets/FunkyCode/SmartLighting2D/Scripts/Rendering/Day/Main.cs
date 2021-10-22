using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;
using LightSettings;

namespace Rendering.Day
{	
	public static class Main
	{
		static Pass pass = new Pass();

		public static bool IsDrawing(Camera camera, LightmapPreset lightmapPreset) {
			if (Lighting2D.DayLightingSettings.alpha == 0) {
				return(false);
			}

			if (lightmapPreset == null) {
				return(false);
			}

			LightingLayerSetting[] layerSettings = lightmapPreset.dayLayers.Get();

			if (layerSettings.Length < 1) {
				return(false);
			}

			return(true);
		}

		public static void Draw(Camera camera, LightmapPreset lightmapPreset) {
			if (!IsDrawing(camera, lightmapPreset)) {
				return;
			}

			LightingLayerSetting[] layerSettings = lightmapPreset.dayLayers.Get();
		
			for(int i = 0; i < layerSettings.Length; i++) {
				LightingLayerSetting dayLayer = layerSettings[i];

				LayerSorting sorting = dayLayer.sorting;

				if (!pass.Setup(dayLayer, camera)) {
					continue;
				}

				if (sorting == LayerSorting.None) {
					NoSort.Draw(pass);

				} else {
					pass.SortObjects();

					Sorted.Draw(pass);
				}
			}
			
			ShadowAlpha(camera);
		}

		private static void ShadowAlpha(Camera camera) {
			Color color = new Color(0, 0, 0,  (1f - Lighting2D.DayLightingSettings.alpha));

			if (color.a > 0) {
				color.r = 1f;
				color.g = 1f;
				color.b = 1f;
					
				Material material = Lighting2D.materials.GetAlphaBlend();
				material.mainTexture = null;		
								
				GLExtended.color = color;

				Universal.Texture.Quad.Draw(material, Vector2.zero, LightingRender2D.GetSize(camera), camera.transform.eulerAngles.z, 0);
			}
		}
	}
}