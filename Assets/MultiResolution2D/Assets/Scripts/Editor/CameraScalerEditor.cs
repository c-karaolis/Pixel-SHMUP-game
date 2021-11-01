using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace MultiResolution2D
{
    [CustomEditor(typeof(CameraScaler))]
    public class CameraScalerEditor : Editor
    {
        // Colors for the preview camera projection in Scene Window
        private Color designColor = new Color32(0, 0, 255, 102);
        private Color targetColor = new Color32(0, 255, 105, 102);
        private Texture2D activeTexture;

        // The editor target object
        CameraScaler cameraScaler;
        Camera unityCamera;

        // Properties
        SerializedProperty designScreenSizeProp;
        SerializedProperty spritePixelsPerUnitProp;
        SerializedProperty showPreviewTitleProp;

        // Auto scaling groups list
        ReorderableList autoScalingGroupReordList;

        // selected scaling group
        SerializedProperty selectedAutoScalingGroupIndexProp;
        AutoScalingGroupEditor selectedAutoScalingGroupEditor;

        #region Init

        void FindProperties()
        {
            this.designScreenSizeProp = serializedObject.FindProperty("designScreenSize");
            this.spritePixelsPerUnitProp = serializedObject.FindProperty("spritePixelsPerUnit");
            this.showPreviewTitleProp = serializedObject.FindProperty("showPreviewTitle");
            this.selectedAutoScalingGroupIndexProp = serializedObject.FindProperty("selectedAutoScalingGroupIndex");
        }


        void OnEnable()
        {
            // Target object
            this.cameraScaler = (CameraScaler)target;
            this.unityCamera = this.cameraScaler.GetComponent<Camera>();
            this.activeTexture = CameraScalerEditor.MakeTex(1, 1, new Color32(113, 238, 128, 255));

            // Find and assign properties
            FindProperties();

            // create reorderable list
            this.autoScalingGroupReordList = CreateAutoScalingGroupReorderableList();
            this.autoScalingGroupReordList.index = this.selectedAutoScalingGroupIndexProp.intValue;
        }

        void OnDisable()
        {
            this.selectedAutoScalingGroupEditor = null;

            // Destroy the texture owned by this component
            DestroyImmediate(this.activeTexture);
        }

        #endregion



        #region InspectorGUI

        public override void OnInspectorGUI()
        {
            if (!this.unityCamera.orthographic)
            {
                EditorGUILayout.HelpBox("The Camera projection must be Orthographic to use the Camera Scaler component.", MessageType.Warning);
                return;
            }

            serializedObject.Update();

            // Clamp index to be valid (if we remove the last one, the index do not update) 
            this.selectedAutoScalingGroupIndexProp.intValue = Mathf.Clamp(this.selectedAutoScalingGroupIndexProp.intValue, -1, this.cameraScaler.AutoScalingGroups.Count - 1);

            DrawDesignReferenceGUI();
            DrawAutoScalingGroupsGUI();

            if (!IsPrefabAssetSelected())
            {
                DrawStatsGUI();
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws the native reference GU.
        /// </summary>
        void DrawDesignReferenceGUI()
        {
            // Title
            GUILayout.Label("Design Reference", EditorStyles.boldLabel);

            // Native screen size
            EditorGUI.BeginChangeCheck();
            Vector2 designResolution = EditorGUILayout.Vector2Field(new GUIContent("Screen Size (px)", "The resolution in pixels the game is designed for."), this.cameraScaler.designScreenSize);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this.cameraScaler, "Update Design Screen Size");
                this.designScreenSizeProp.vector2Value = new Vector2((int)designResolution.x, (int)designResolution.y);
            }

            EditorGUILayout.Space();

            // Sprite PPU
            EditorGUILayout.PropertyField(this.spritePixelsPerUnitProp, new GUIContent("Sprite Pixels Per Unit", "How many pixels in sprite assets correspond to one unit in the world. Set the same value used on sprite textures."));

            EditorGUILayout.Space();

            // Show camera bounds title in scene view
            EditorGUILayout.PropertyField(this.showPreviewTitleProp, new GUIContent("Show Preview Title", "Show the camera preview title in Scene view."));
        }

        /// <summary>
        /// Draws the auto scaling groups GUI.
        /// </summary>
        void DrawAutoScalingGroupsGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("The first group where at least one of the criteria is met will be used (from top to bottom).", MessageType.Info);

            // Draw Screen Scaler List
            this.autoScalingGroupReordList.index = this.selectedAutoScalingGroupIndexProp.intValue;
            this.autoScalingGroupReordList.DoLayoutList();

            EditorGUILayout.Space();

            // Draw Active Auto Scaling Group Editor
            UpdateActiveAutoScalingGroupEditor();
            if (this.selectedAutoScalingGroupEditor != null)
            {
                this.selectedAutoScalingGroupEditor.OnInspectorGUI();
            }
        }

        /// <summary>
        /// Draws the camera preview.
        /// </summary>
        void DrawStatsGUI()
        {
            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(true);
            {
                // Preview Size in pixels (Target Screen Size)
                {
                    EditorGUILayout.LabelField(
                        new GUIContent("Target Screen Size"),
                        new GUIContent(string.Format("{0:0} x {1:0} (pixels)", this.cameraScaler.TargetScreenSize.x, this.cameraScaler.TargetScreenSize.y)));
                }

                // Scale
                {
                    var activeAutoScalingGroup = this.cameraScaler.GetActiveAutoScalingGroup();
                    float scale = this.cameraScaler.EditorGetScaleForAutoScalingGroup(activeAutoScalingGroup, this.cameraScaler.designScreenSize, this.cameraScaler.TargetScreenSize);
                    EditorGUILayout.LabelField(new GUIContent("Scale"), new GUIContent(string.Format("{0}x", scale)));

                    // Pixel Perfect
                    // Test is scale is an integer
                    bool scaleIsInteger = Mathf.Abs(scale % 1) <= (System.Double.Epsilon * 100);
                    string pixelPerfect = "";
                    if (scaleIsInteger)
                    {
                        pixelPerfect = "Yes";
                    }
                    else
                    {
                        pixelPerfect = scale < 1.0f ? "No (downscaling)" : "No (upscaling)";
                    }

                    EditorGUILayout.LabelField(new GUIContent("Pixel Perfect", "The rendering is pixel perfect?"), new GUIContent(pixelPerfect));
                }

                // Coverage
                {
                    EditorGUILayout.LabelField(
                        new GUIContent("Coverage", "How much of the design world space is visible."),
                        new GUIContent(string.Format("{0:P} x {1:P}", this.cameraScaler.Coverage.x, this.cameraScaler.Coverage.y)));
                }

                //                // Screen DPI
                //                {
                //                    EditorGUILayout.LabelField(new GUIContent("Screen DPI"), new GUIContent(Screen.dpi.ToString()));
                //                }

            }
            EditorGUI.EndDisabledGroup();
        }

        #endregion


        #region Scene View GUI

        void OnSceneGUI()
        {
            // Do not show scene gui for object selected in project view (prefab)
            if (IsPrefabAssetSelected())
            {
                return;
            }

            // Draw camera bounds (design / target) on scene view
            Camera cam = this.unityCamera;
            if (cam.orthographic)
            {
                // Design resolution (fixed and according to the PPU).
                Vector2 designSize = this.cameraScaler.designScreenSize / this.cameraScaler.spritePixelsPerUnit;
                DrawCameraBounds(designSize, this.designColor, "Design");

                // Target resolution (based on current orthographic size (PPU already taken into account in orthographic size calculation)).
                Vector2 targetResolution = this.cameraScaler.TargetScreenSize;
                float targetAspect = targetResolution.x / targetResolution.y;

                DrawCameraBounds(new Vector2(cam.orthographicSize * 2.0f * targetAspect, cam.orthographicSize * 2.0f), this.targetColor, "Target");
            }

            SceneView.RepaintAll();
        }

        void DrawCameraBounds(Vector2 cameraSize, Color color, string title)
        {
            Camera cam = unityCamera;

            // http://answers.unity3d.com/questions/401718/draw-camera-frustum.html
            Matrix4x4 temp = Handles.matrix;
            Handles.matrix = Matrix4x4.TRS(this.cameraScaler.transform.position, this.cameraScaler.transform.rotation, Vector3.one);

            Vector3 cameraCenter = new Vector3(0, 0, (cam.farClipPlane + cam.nearClipPlane) * 0.5f);
            float cameraLength = cam.farClipPlane - cam.nearClipPlane;

            Handles.color = color;
            Handles.DrawWireCube(cameraCenter, new Vector3(cameraSize.x, cameraSize.y, cameraLength));

            if (this.showPreviewTitleProp.boolValue)
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;

                Handles.BeginGUI();
                Vector3 pos = new Vector3(-cameraSize.x, cameraSize.y, 0) * 0.5f;
                Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos);

                GUI.backgroundColor = color;

                GUIContent content = new GUIContent(title); // + ": " + resolution.x + "x" + resolution.y);
                Vector2 size = style.CalcSize(content);

                GUI.Box(new Rect(pos2D.x, pos2D.y, size.x + 4, size.y + 4), "");
                GUI.Label(new Rect(pos2D.x + 2, pos2D.y + 2, size.x, size.y), content, style);
                Handles.EndGUI();
            }

            Handles.matrix = temp;
            Handles.color = Color.white;
        }

        #endregion

        #region Auto Scaling Groups

        ReorderableList CreateAutoScalingGroupReorderableList()
        {
            ReorderableList reordList = new ReorderableList(serializedObject, serializedObject.FindProperty("autoScalingGroupDatas"), true, true, true, true);

            // Draw header
            reordList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Auto Scaling Groups", EditorStyles.boldLabel);
            };

            // Draw element
            reordList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = reordList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                Rect rGroupName = new Rect(rect.x, rect.y, rect.width * 0.7f, EditorGUIUtility.singleLineHeight);
                Rect rScaleMode = new Rect(rect.x + rGroupName.width + 5.0f, rect.y, rect.width - rGroupName.width - 5.0f, EditorGUIUtility.singleLineHeight);

                // Group name
                EditorGUI.PropertyField(rGroupName, element.FindPropertyRelative("autoScalingGroupName"), GUIContent.none);

                // Active label
                if (!IsPrefabAssetSelected() && index == this.cameraScaler.GetActiveAutoScalingGroupIndex())
                {
                    GUIStyle activeStyle = new GUIStyle(EditorStyles.boldLabel);
                    activeStyle.alignment = TextAnchor.MiddleCenter;
                    activeStyle.normal.textColor = new Color32(11, 98, 22, 255);
                    activeStyle.normal.background = this.activeTexture;

                    EditorGUI.LabelField(rScaleMode, new GUIContent("ACTIVE"), activeStyle);
                }

                if (GUI.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            };

            // Add an element
            reordList.onAddCallback = (ReorderableList list) =>
            {
                int index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;

                // Set Auto Scaling group default values
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("autoScalingGroupName").stringValue = "Group Name";
                element.FindPropertyRelative("scaleMode").enumValueIndex = (int)AutoScalingGroupData.ScaleMode.ClosestWholeNumber;
                element.FindPropertyRelative("manualScale").floatValue = 1.0f;
                element.FindPropertyRelative("matchingRules").ClearArray();

                serializedObject.ApplyModifiedProperties();
            };

            // Select an element
            reordList.onSelectCallback = (ReorderableList list) =>
            {
                this.selectedAutoScalingGroupIndexProp.intValue = list.index;
                serializedObject.ApplyModifiedProperties();
                GUI.changed = true;
            };

            // Remove selected element
            reordList.onRemoveCallback = (ReorderableList list) =>
            {
                list.serializedProperty.DeleteArrayElementAtIndex(list.index);

                serializedObject.ApplyModifiedProperties();

                this.selectedAutoScalingGroupIndexProp.intValue = -1;
                GUI.changed = true;
            };

            return reordList;

        }

        /// <summary>
        /// Create and return the Editor for the selected Auto Scaling group in the list
        /// </summary>
        /// <returns>The auto scaling group editor.</returns>
        private void UpdateActiveAutoScalingGroupEditor()
        {
            if (this.cameraScaler.SelectedAutoScalingGroup != null)
            {
                SerializedProperty groupToShowProp = this.autoScalingGroupReordList.serializedProperty.GetArrayElementAtIndex(this.autoScalingGroupReordList.index);

                // Create editor or update editor only if selection changed
                if (this.selectedAutoScalingGroupEditor == null || this.selectedAutoScalingGroupEditor.AutoScalingGroupProp.propertyPath != groupToShowProp.propertyPath)
                {
                    this.selectedAutoScalingGroupEditor = new AutoScalingGroupEditor(groupToShowProp);
                }
            }
            else
            {
                this.selectedAutoScalingGroupEditor = null;
            }
        }

        // Create a texture
        private static Texture2D MakeTex(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = color;
            }

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pix);
            texture.Apply();
            texture.hideFlags = HideFlags.HideAndDontSave;

            return texture;
        }

        /// <summary>
        /// Return true if the target object selected is a prefab asset (selected in project view)
        /// </summary>
        private bool IsPrefabAssetSelected()
        {
            return PrefabUtility.IsPartOfPrefabAsset(target);
        }
        #endregion


    }
}