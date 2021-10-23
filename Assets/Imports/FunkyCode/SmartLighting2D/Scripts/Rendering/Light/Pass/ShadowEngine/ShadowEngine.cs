using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightSettings;
using FunkyCode.Utilities;

namespace Rendering.Light {

    public struct UVRect {
        public float x0;
        public float y0;
        public float x1;
        public float y1;

        public UVRect(float x0, float y0, float x1, float y1) {
            this.x0 = x0;
            this.y0 = y0;
            this.x1 = x1;
            this.y1 = y1;
        }
    }

    public static class ShadowEngine {
        public static Light2D light;

        public static Vector2 lightOffset = Vector2.zero;
        public static Vector2 drawOffset = Vector2.zero;

        public static float lightSize = 0;
        public static bool ignoreInside = false;

        public static Vector2 objectOffset = Vector2.zero;
        public static float shadowDistanceMax = 1;
        public static float shadowDistanceMin = 0;
        public static bool flipX = false;
        public static bool flipY = false;

        public static Sprite spriteProjection = null;

        public static bool perpendicularIntersection;
        public static int effectLayer = 0;

        public static UVRect penumbraRect = new UVRect(0, 0, 1, 1);
        public static UVRect fillRect = new UVRect(0, 1, 1, 1);

        // Layer Effect
        public static List<List<Polygon2>> effectPolygons = new List<List<Polygon2>>();

        // public static float shadowDistance;
        
        public static bool softShadow = false;

        public static int drawMode = 0;

        public const int DRAW_MODE_LEGACY_CPU = 0;
        public const int DRAW_MODE_LEGACY_GPU = 4;
        public const int DRAW_MODE_SOFT_CONVEX = 1;
        public const int DRAW_MODE_SOFT_VERTEX = 5;
        public const int DRAW_MODE_PERPENDICULAR = 2;
        public const int DRAW_MODE_SPRITEPROJECTION = 3;

        public static Material GetMaterial() {
            Material material = null;

            switch(drawMode) {
                case DRAW_MODE_LEGACY_CPU:
                    material = Lighting2D.materials.shadow.GetLegacyCPUShadow();
                    material.mainTexture = Lighting2D.materials.shadow.GetPenumbraSprite().texture;
                break;

                case DRAW_MODE_LEGACY_GPU:
                    material = Lighting2D.materials.shadow.GetLegacyGPUShadow();
                break;

                case DRAW_MODE_SOFT_CONVEX:
                case DRAW_MODE_SOFT_VERTEX:
                    material = Lighting2D.materials.shadow.GetSoftShadow();
                    material.SetFloat("_CoreSize", light.coreSize);
                    material.SetFloat("_FallOff", light.falloff);
                    
                break;

                case DRAW_MODE_PERPENDICULAR:
                case DRAW_MODE_SPRITEPROJECTION:
                    material = Lighting2D.materials.GetAlphaBlend();

                break;
            }

            return(material);
        }
        
        public static void Draw(List<Polygon2> polygons, float shadowDistance, float shadowTranslucency) {
            switch(ShadowEngine.drawMode) {
                case DRAW_MODE_LEGACY_CPU:
                    Shadow.LegacyCPU.Draw(polygons, shadowDistance, shadowTranslucency);
                break;

                case DRAW_MODE_LEGACY_GPU:
                    Shadow.LegacyGPU.Draw(polygons, shadowDistance, shadowTranslucency);
                break;
            
                case DRAW_MODE_SOFT_CONVEX:
                case DRAW_MODE_SOFT_VERTEX:
                    // Does not support Shadow Distance
                    Shadow.Soft.Draw(polygons, shadowTranslucency); 
                break;

                case DRAW_MODE_PERPENDICULAR:
                    // Does not support Translucency + Shadow Distance after intersection)
                    Shadow.PerpendicularIntersection.Draw(polygons, shadowDistance);
                break;

                case DRAW_MODE_SPRITEPROJECTION:
                    Shadow.SpriteProjection.Draw(polygons, shadowDistance, shadowTranslucency);
                break;

            }
        }

        public static void SetPass(Light2D lightObject, LayerSetting layer) {
            light = lightObject;
            lightSize = Mathf.Sqrt(light.size * light.size + light.size * light.size);
            lightOffset = -light.transform2D.position;

            effectLayer = layer.shadowEffectLayer;

            objectOffset = Vector2.zero;

            effectPolygons.Clear();

            softShadow = layer.shadowEffect == LightLayerShadowEffect.SoftConvex || layer.shadowEffect == LightLayerShadowEffect.SoftVertex;

            if (lightObject.IsPixelPerfect()) {
                Camera camera = Camera.main;

                Vector2 pos = LightingPosition.GetPosition2D(-camera.transform.position);

                drawOffset = light.transform2D.position + pos;
            } else {
                drawOffset = Vector2.zero;
            }

            switch(layer.shadowEffect) {
                case LightLayerShadowEffect.PerpendicularProjection:
                    drawMode = DRAW_MODE_PERPENDICULAR;
                    GenerateEffectLayers();
                break;

                case LightLayerShadowEffect.SoftConvex:
                    drawMode = DRAW_MODE_SOFT_CONVEX;
                break;

                case LightLayerShadowEffect.SoftVertex:
                    drawMode = DRAW_MODE_SOFT_VERTEX;
                break;

                case LightLayerShadowEffect.SpriteProjection:
                    drawMode = DRAW_MODE_SPRITEPROJECTION;
                break;

                case LightLayerShadowEffect.LegacyCPU:
                    drawMode = DRAW_MODE_LEGACY_CPU;
                break;

                case LightLayerShadowEffect.LegacyGPU:
                    drawMode = DRAW_MODE_LEGACY_GPU;
                break;
            }
        }

        public static void GenerateEffectLayers() {
            int layerID = (int)ShadowEngine.effectLayer;

            foreach(LightCollider2D c in LightCollider2D.GetShadowList((layerID))) {
                List<Polygon2> polygons = c.mainShape.GetPolygonsWorld();

                if (polygons == null) {
                    continue;
                }
    
                if (c.InLight(light)) {
                    effectPolygons.Add(polygons);
                }
            }
        }
        
        public static void Prepare(Light2D light) {
   
            ignoreInside = light.ignoreWhenInsideCollider;
        }

 
    }
}