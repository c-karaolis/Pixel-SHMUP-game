using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lighting2DMaterial {

	[System.Serializable]
	public class Shadow {
		private Sprite penumbraSprite;

		private LightingMaterial softShadow = null;
		private LightingMaterial legacyGPUShadow = null;
		private LightingMaterial legacyCPUShadow = null;

		private LightingMaterial spriteProjection = null;

		private LightingMaterial dayCPUShadow = null;
		private LightingMaterial spriteShadow = null;

		public void Reset() {
			penumbraSprite = null;

			softShadow = null;
			legacyGPUShadow = null;
			legacyCPUShadow = null;
				
			dayCPUShadow = null;
			spriteProjection = null;

			spriteShadow = null;
		}

		public void Initialize() {
			GetSoftShadow();
			GetLegacyGPUShadow();
			GetLegacyCPUShadow();

			GetPenumbraSprite();

			GetDayCPUShadow();
			GetSpriteShadow();
		}
	
		public Material GetSoftShadow() {
			if (softShadow == null || softShadow.Get() == null) {
				softShadow = LightingMaterial.Load("Light2D/Internal/SoftShadow");
			}
			return(softShadow.Get());
		}

		public Material GetLegacyGPUShadow() {
			if (legacyGPUShadow == null || legacyGPUShadow.Get() == null) {
				legacyGPUShadow = LightingMaterial.Load("Light2D/Internal/Shadow/LegacyGPU");

				if (legacyGPUShadow.Get() != null) {
					legacyGPUShadow.Get().mainTexture = GetPenumbraSprite().texture;
				}
			}
			return(legacyGPUShadow.Get());
		}

		public Material GetLegacyCPUShadow() {
			if (legacyCPUShadow == null || legacyCPUShadow.Get() == null) {
				legacyCPUShadow = LightingMaterial.Load("Light2D/Internal/Shadow/LegacyCPU");

				if (legacyCPUShadow.Get() != null) {
					legacyCPUShadow.Get().mainTexture = GetPenumbraSprite().texture;
				}
			}
			return(legacyCPUShadow.Get());
		}


		public Sprite GetPenumbraSprite() {
			if (penumbraSprite == null) {
				penumbraSprite = Resources.Load<Sprite>("textures/penumbra"); 
			}
			return(penumbraSprite);
		}

		public Material GetDayCPUShadow() {
			if (dayCPUShadow == null || dayCPUShadow.Get() == null) {
				dayCPUShadow = LightingMaterial.Load("Light2D/Internal/AlphaBlended");
			}
			return(dayCPUShadow.Get());
		}

		
		public Material GetSpriteShadow() {
			if (spriteShadow == null || spriteShadow.Get() == null) {
				spriteShadow = LightingMaterial.Load("Light2D/Internal/SpriteShadow");
			}
			return(spriteShadow.Get());
		}

		
		public Material GetSpriteProjectionMaterial() {
			if (spriteProjection == null || spriteProjection.Get() == null) {
				spriteProjection = LightingMaterial.Load("Light2D/Internal/SpriteProjection");
			}
			
			return(spriteProjection.Get());
		}

	}
}