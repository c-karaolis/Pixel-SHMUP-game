using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.LightSource {

    public static class Main {

         public static void Draw(Light2D light) {
            if (light == null) {
                return;
            }

            UnityEngine.Sprite lightSprite = light.GetSprite();

            if (lightSprite == null) {
                return;
            }

            if (lightSprite.texture == null) {
                return;
            }

            Vector2 position = Vector2.zero;
            Vector2 size = new Vector2(light.size, light.size);
            float z = 0;

            Material material = Lighting2D.materials.GetMultiplyHDR();
            material.mainTexture = lightSprite.texture;

            if (light.IsPixelPerfect()) {
                position = ShadowEngine.drawOffset;
            }

            if (light.applyRotation) {
                material.color = Color.white;

                material.SetPass(0);

                LightSprite.Sprite.Draw(position, size, light.transform.rotation.eulerAngles.z, light.spriteFlipX, light.spriteFlipY);

                material.color = Color.black;

                Bounds.Draw(light, position, material, z);
                
            } else {
                material.color = Color.white;

                material.SetPass (0); 

                LightSprite.Sprite.Draw(position, size, 0, light.spriteFlipX, light.spriteFlipY);
            }

            if (light.spotAngle != 360) {

                Lighting2D.materials.shadow.GetLegacyCPUShadow().SetPass(0);

                GL.Begin(GL.TRIANGLES);

                Angle.Draw(light, z);

                GL.End ();

            }
        }
    }
}