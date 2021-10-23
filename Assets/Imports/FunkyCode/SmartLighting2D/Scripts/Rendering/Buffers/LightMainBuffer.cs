using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering {

    public class LightMainBuffer {

        public class Check {
                    
            static public void RenderTexture(LightMainBuffer2D buffer) {
                Vector2Int screen = GetScreenResolution(buffer);

                if (screen.x > 0 && screen.y > 0) {
                    Camera camera = buffer.cameraSettings.GetCamera();

                    if (buffer.renderTexture == null || screen.x != buffer.renderTexture.width || screen.y != buffer.renderTexture.height) {

                        switch(camera.cameraType) {
                            case CameraType.Game:
                                Rendering.LightMainBuffer.InitializeRenderTexture(buffer);
                            
                            break;

                            case CameraType.SceneView:
                                // Scene view pixel rect is constantly changing (Unity Bug?)
                                int differenceX = Mathf.Abs(screen.x - buffer.renderTexture.width);
                                int differenceY = Mathf.Abs(screen.y - buffer.renderTexture.height);
                                
                                if (differenceX > 5 || differenceY > 5) {
                                    Rendering.LightMainBuffer.InitializeRenderTexture(buffer);
                                }
                            
                            break;

                        }
                    }
                }
            }

            static public bool CameraSettings (LightMainBuffer2D buffer) {
                LightingManager2D manager = LightingManager2D.Get();
                int settingsID = buffer.cameraSettings.id;

                if (settingsID >= manager.cameras.Length) {
                    return(false);
                }

                CameraSettings cameraSetting = manager.cameras.Get(settingsID);

                int bufferId = buffer.cameraLightmap.id;

                if (bufferId >= cameraSetting.Lightmaps.Length) {
                   return(false);
                }

                CameraLightmap cameraBufferPreset = cameraSetting.GetLightmap(bufferId);

                if (false == (cameraBufferPreset.bufferID == buffer.cameraLightmap.bufferID && cameraSetting.cameraType == buffer.cameraSettings.cameraType)) {
                    return(false);
                }

                //buffer.cameraBufferPreset = cameraBufferPreset;

                return(true);
            }
        }

        public static void Update(LightMainBuffer2D buffer) {
            LightmapPreset bufferPreset = buffer.GetLightmapPreset();

            if (bufferPreset == null) {
                buffer.DestroySelf();
                return;
            }

            if (Rendering.LightMainBuffer.Check.CameraSettings(buffer) == false) {
                buffer.DestroySelf();
                return;
            }
            
            Camera camera = buffer.cameraSettings.GetCamera();

            if (camera == null) {
                return;
            }

            Rendering.LightMainBuffer.Check.RenderTexture(buffer);
        }

        public static void DrawPost(LightMainBuffer2D buffer) {			
			if (buffer.cameraLightmap.renderMode != CameraLightmap.RenderMode.Draw) {
				return;
			}

            if (Lighting2D.RenderingMode != RenderingMode.OnPostRender) {
				return;
			}

			LightingRender2D.PostRender(buffer);
        }

        public static void DrawOn(LightMainBuffer2D buffer) {
			if (buffer.cameraLightmap.renderMode != CameraLightmap.RenderMode.Draw) {
				return;
			}
                
            switch(Lighting2D.RenderingMode) {
                case RenderingMode.OnRender:
                    LightingRender2D.OnRender(buffer);
                break;

                case RenderingMode.OnPreRender:
                    LightingRender2D.PreRender(buffer);
                break;
            }
        }

        public static void Render(LightMainBuffer2D buffer) {
            Camera camera = buffer.cameraSettings.GetCamera();
            if (camera == null) {
                return;
            }

            float cameraRotation = LightingPosition.GetCameraRotation(camera);
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, cameraRotation), Vector3.one);

            float sizeY = camera.orthographicSize;
            float sizeX = sizeY * ( (float)camera.pixelWidth / camera.pixelHeight );

            LightmapPreset lightmapPreset = buffer.GetLightmapPreset();

            GL.Clear(true, true, Rendering.Night.Main.ClearColor(camera, lightmapPreset)); // Clear Darkness Color
            
            GL.LoadPixelMatrix( -sizeX, sizeX, -sizeY, sizeY );
            GL.MultMatrix(matrix);

            GL.PushMatrix();
            
            Rendering.Day.Main.Draw(camera, lightmapPreset);
        
            Rendering.Night.Main.Draw(camera, lightmapPreset);

            GL.PopMatrix();
        }

        static public Vector2Int GetScreenResolution(LightMainBuffer2D buffer) {
            LightmapPreset lightmapPreset = buffer.GetLightmapPreset();

            if (lightmapPreset == null) {
                return(Vector2Int.zero);
            }

            Camera camera = buffer.cameraSettings.GetCamera();

            if (camera == null) {
                return(Vector2Int.zero);
            }

            float resolution = lightmapPreset.lightingResolution;

            int screenWidth = (int)(camera.pixelRect.width * resolution);
            int screenHeight = (int)(camera.pixelRect.height * resolution);

            return(new Vector2Int(screenWidth, screenHeight));
        }

        static public void InitializeRenderTexture(LightMainBuffer2D buffer) {
            Vector2Int screen = GetScreenResolution(buffer);
            
            if (screen.x > 0 && screen.y > 0) {
                string idName = "";

                int bufferID = buffer.cameraLightmap.bufferID;
                
                if (bufferID < Lighting2D.LightmapPresets.Length) {
                    idName = Lighting2D.LightmapPresets[bufferID].name + ", ";
                }

                Camera camera = buffer.cameraSettings.GetCamera();

                buffer.name = "Camera Buffer (" + idName +"Id: " + (bufferID  + 1) + ", Camera: " + camera.name + " )";
    
                RenderTextureFormat format = RenderTextureFormat.Default;

                switch(Lighting2D.QualitySettings.HDR) {
                    case LightingSettings.HDR.Half:
                        format = RenderTextureFormat.RGB111110Float;
                    break;

                    case LightingSettings.HDR.Float:
                        format = RenderTextureFormat.DefaultHDR;
                    break;

                    case LightingSettings.HDR.Off:
                        if (SystemInfo.IsFormatSupported(UnityEngine.Experimental.Rendering.GraphicsFormat.R5G6B5_UNormPack16, UnityEngine.Experimental.Rendering.FormatUsage.Render)) {
                            format = RenderTextureFormat.RGB565;
                        }
                    break;
                }
 
                buffer.renderTexture = new LightTexture (screen.x, screen.y, 0, format);
                buffer.renderTexture.Create ();
            }
        }
    }
}
